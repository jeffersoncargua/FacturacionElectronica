using AutoMapper;
using FacturacionElectronicaSRI.Data.Context;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using FacturacionElectronicaSRI.Repository.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Net;
using ZXing;
using ZXing.OneD;
using ZXing.Rendering;

namespace FacturacionElectronicaSRI.Repository.Service
{
    public class VentaService : IVentaService
    {
        private readonly IServiceSRI _serviceSRI;
        private readonly IProductoRepository _productoRepository;
        private readonly IDetalleVentaRepository _detalleVentaRepository;
        private readonly IComprobanteVentaRepository _comprobanteVentaRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IRutasFacturacionRepository _rutasFacturacionRepository;
        private readonly IHostingEnvironment _webHostingEnvironment;
        private readonly IEmailService _emailService;
        private readonly IKushkiService _kushkiService;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;
        protected Response _response;
        public VentaService(IServiceSRI serviceSRI, IProductoRepository productoRepository, IDetalleVentaRepository detalleVentaRepository, IComprobanteVentaRepository comprobanteVentaRepository, IClienteRepository clienteRepository, IEmpresaRepository empresaRepository, IRutasFacturacionRepository rutasFacturacionRepository, IHostingEnvironment webHostingEnvironment, IEmailService emailService, IKushkiService kushkiService, IMapper mapper, ApplicationDbContext db)
        {
            _serviceSRI = serviceSRI;
            _productoRepository = productoRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _comprobanteVentaRepository = comprobanteVentaRepository;
            _clienteRepository = clienteRepository;
            _empresaRepository = empresaRepository;
            _rutasFacturacionRepository = rutasFacturacionRepository;
            _mapper = mapper;
            _emailService = emailService;
            _webHostingEnvironment = webHostingEnvironment;
            _db = db;
            _kushkiService = kushkiService;
            _response = new();
        }

        public async Task<Response> GenerarComprobanteVenta(ComprobanteVentaDto comprobanteVentaDto)
        {
            try
            {
                if (comprobanteVentaDto != null)
                {
                    var response = await _comprobanteVentaRepository.CreateComprobanteVentaAsync(comprobanteVentaDto);

                    if (response.IsSuccess)
                    {
                        var comprobantesNuevos = await _comprobanteVentaRepository.GetAllAsync(includeProperties: "Empresa,Cliente");

                        if (comprobantesNuevos.Count > 0)
                        {
                            var comprobantesRegistrados = comprobantesNuevos;
                            var comprobanteGenerado = comprobantesRegistrados.OrderByDescending(x => x.Id).First(u => u.IdCliente == comprobanteVentaDto.IdCliente);

                            _response.IsSuccess = true;
                            _response.StatusCode = HttpStatusCode.OK;
                            _response.Result = _mapper.Map<ComprobanteVentaDto>(comprobanteGenerado);
                            _response.Message = "Se genero el comprobante y se remitío para continuar con el proceso de facturación";

                            return _response;
                        }

                        _response.IsSuccess = false;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.Message = "No se encontró el comprobante generado";

                        return _response;
                    }

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = response.Message;

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = $"El comprobante a registrar esta vacío";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";

                return _response;
            }
        }

        public async Task<Response> GenerarDetalleVenta(int comprobanteId, List<ShoppingCartDto> productsInCart)
        {
            try
            {
                foreach (var item in productsInCart)
                {
                    var producto = await _productoRepository.GetAsync(u => u.CodigoPrincipal == item.CodigoPrincipal, tracked: false);

                    if (producto != null)
                    {
                        DetalleVentaDto detalleVenta = new()
                        {
                            IdComprobanteVenta = comprobanteId,
                            IdProducto = producto.Id,
                            Cantidad = item.Cantidad,
                            PrecioUnitario = item.PrecioUnitario,
                            Estado = "abierto",
                            Descuento = item.Descuento,
                            VentaIva = item.VentaIva,
                            Total = item.Total,
                            Detalle = item.Descripcion,
                        };

                        await _detalleVentaRepository.CreateDetalleVentaAsync(detalleVenta);
                    }
                }

                var productosDetalleVenta = await _detalleVentaRepository.GetAllAsync(u => u.IdComprobanteVenta == comprobanteId, includeProperties: "ComprobanteVenta,Producto");

                if (productosDetalleVenta != null && productosDetalleVenta.Count > 0)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Message = "Registro exitoso";
                    _response.Result = _mapper.Map<List<DetalleVentaDto>>(productosDetalleVenta);

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = "No se encontraron los artículos para realizar la transacción";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";

                return _response;
            }
        }

        public async Task<Response> GenerarRideYPdf(ComprobanteVentaDto comprobanteDto, string pathXml, int plazos)
        {
            var detallesDb = await _detalleVentaRepository.GetAllAsync(u => u.IdComprobanteVenta == comprobanteDto.Id, includeProperties: "ComprobanteVenta,Producto");
            if (detallesDb.Count == 0)
            {
                _response.IsSuccess = false;
                _response.Message = $"No se encontro el detalle de la venta de este comprobante {comprobanteDto.Id} + clave de acceso {comprobanteDto.DocSri}. No se pudo generar el documento PDF";
                _response.StatusCode = HttpStatusCode.NotFound;

                return _response;
            }

            var documentPdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.Content()
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Spacing(3f, Unit.Millimetre);

                        // Logo e Informacion del comprobante
                        column.Item()
                        .AlignCenter()
                        .Row(row =>
                        {
                            row.Spacing(6f, Unit.Millimetre);

                            row.RelativeItem()
                            .Column(column =>
                            {
                                column.Spacing(6f, Unit.Millimetre);

                                // Logo de la empresa para el comprobante
                                column.Item()
                                .Width(230)
                                .Height(180)

                                .Image(comprobanteDto.Empresa!.PathLogo);

                                // Informacion de la empresa
                                column.Item()
                                .Border(1)
                                .Padding(2f, Unit.Millimetre)
                                .Column(y =>
                                {
                                    y.Spacing(4f, Unit.Millimetre);

                                    y.Item()
                                    .Text(comprobanteDto.Empresa!.RazonSocial) // Aqui va el nombre de la empresa
                                    .FontSize(11)
                                    .ExtraBold();

                                    y.Item()
                                    .Row(x =>
                                    {
                                        x.AutoItem()
                                        .Text("Direccion Matriz : ")
                                        .FontSize(9)
                                        .Bold();

                                        x.ConstantItem(15);

                                        // x.AutoItem()
                                        x.RelativeItem()
                                        .Text(comprobanteDto.Empresa.DireccionMatriz) // Aqui va la direccion principal de la empresa
                                        .FontSize(9);
                                    });

                                    y.Item()
                                    .Row(row =>
                                    {
                                        row.AutoItem()
                                        .Text("Direccion Sucursal : ")
                                        .FontSize(9)
                                        .Bold();

                                        row.ConstantItem(15);

                                        // row.AutoItem()
                                        row.RelativeItem()
                                        .Text(comprobanteDto.Empresa.DireccionMatriz) // Aqui va la direccion de la sucursal de la empresa
                                        .FontSize(9);
                                    });

                                    y.Item()
                                    .Row(row =>
                                    {
                                        row.AutoItem()
                                        .Text("Obligado a llevar contabilidad : ")
                                        .FontSize(9)
                                        .Bold();

                                        row.ConstantItem(15);

                                        row.AutoItem()
                                        .Text(comprobanteDto.Empresa.ObligadoLlevarContabilidad) // Aqui va si la empresa lleva o no contabilidad
                                        .FontSize(9);
                                    });
                                });
                            });

                            // Informacion del comprobante de la venta como clave de acceso, etc.
                            row.AutoItem()
                            .Border(1)
                            .Padding(4f, Unit.Millimetre)
                            .Column(column =>
                            {
                                column.Spacing(4f, Unit.Millimetre);

                                column.Item()
                                .Row(x =>
                                {
                                    x.AutoItem()
                                    .Text("R.U.C. : ")
                                    .FontSize(11)
                                    .Bold();

                                    x.ConstantItem(15);

                                    x.AutoItem()
                                    .Text(comprobanteDto.Empresa!.Ruc) // Aqui va el RUC de la empresa
                                    .FontSize(11);
                                });

                                column.Item()
                                .Row(row =>
                                {
                                    row.AutoItem()
                                    .Text("FACTURA Nº : ")
                                    .FontSize(11)
                                    .Bold();

                                    row.ConstantItem(15);

                                    row.AutoItem()
                                    .Text(comprobanteDto.NumeroComprobante) // Aqui va la el numero del comprobante de la factura
                                    .FontSize(11);
                                });

                                column.Item()
                                .Row(row =>
                                {
                                    row.AutoItem()
                                    .Text("NÚMERO AUTORIZACIÓN : ")
                                    .FontSize(11)
                                    .Bold();
                                });

                                column.Item()
                                .Row(row =>
                                {
                                    row.AutoItem()
                                    .Padding(-3f, Unit.Millimetre)
                                    .Text(comprobanteDto.DocSri) // Aqui va la clave de acceso
                                    .FontSize(8);
                                });

                                column.Item()
                                .Row(row =>
                                {
                                    row.ConstantItem(90)
                                    .Text("FECHA Y HORA DE AUTORIZACIÓN : ")
                                    .FontSize(11)
                                    .Bold();

                                    row.ConstantItem(15);

                                    row.AutoItem()
                                    .Text(comprobanteDto.FechaEmision.ToShortDateString()) // Aqui va la fecha de emision del comprobante
                                    .FontSize(11);
                                });

                                column.Item()
                                .Row(row =>
                                {
                                    row.AutoItem()
                                    .Text("AMBIENTE : ")
                                    .FontSize(11)
                                    .Bold();

                                    row.ConstantItem(15);

                                    row.AutoItem()
                                    .Text(comprobanteDto.Empresa!.Ambiente == 1 ? "Prueba" : "Producción") // Aqui va el ambiente con la que se emite facturas en la empresa
                                    .FontSize(11);
                                });

                                column.Item()
                                .Row(row =>
                                {
                                    row.AutoItem()
                                    .Text("EMISIÓN : ")
                                    .FontSize(11)
                                    .Bold();

                                    row.ConstantItem(15);

                                    row.AutoItem()
                                    .Text("Normal")
                                    .FontSize(11);
                                });

                                column.Item()
                                .AlignCenter()
                                .Row(row =>
                                {
                                    row.AutoItem()
                                    .Text("CLAVE DE ACCESO : ")
                                    .FontSize(11)
                                    .Bold();
                                });

                                column.Item()
                                .AlignCenter()
                                .Row(row =>
                                {
                                    row.AutoItem()
                                    .Padding(-4f, Unit.Millimetre)
                                    .Background(Colors.White)
                                    .AlignCenter()
                                    .AlignMiddle()
                                    .Width(200)
                                    .Height(75)
                                    .Svg(size =>
                                    {
                                        var content = comprobanteDto.DocSri; // Aqui va la clave de acceso

                                        var writer = new Code128Writer();
                                        var eanCode = writer.encode(content, BarcodeFormat.CODE_128, (int)size.Width, (int)size.Height);
                                        var renderer = new SvgRenderer { FontName = "Lato", FontSize = 12 };
                                        return renderer.Render(eanCode, BarcodeFormat.CODE_128, content).Content;
                                    });
                                });
                            });
                        });

                        // Informacion del cliente como el nombre, direccion, identificacion, etc.
                        column.Item()
                        .Border(1)
                        .Padding(2f, Unit.Millimetre)
                        .Row(row =>
                        {
                            row.Spacing(32);

                            row.AutoItem()
                            .Column(column =>
                            {
                                column.Spacing(3f, Unit.Millimetre);

                                column.Item()
                                .Row(x =>
                                {
                                    x.AutoItem()
                                    .Text("Razón Social/ Nombres y Apellidos :")
                                    .FontSize(9)
                                    .SemiBold();

                                    x.ConstantItem(15);

                                    x.AutoItem()
                                    .Text(comprobanteDto.Cliente!.Nombres) // Aqui va el nombre del cliente o razon social del cliente
                                    .FontSize(9);
                                });

                                column.Item()
                                .Row(x =>
                                {
                                    x.AutoItem()
                                    .Text("Fecha Emisión :")
                                    .FontSize(9)
                                    .SemiBold();

                                    x.ConstantItem(15);

                                    x.AutoItem()
                                    .Text(comprobanteDto.FechaEmision.ToString("dd/MM/yyyy")) // Aqui va la fecha de emision del comprobante
                                    .FontSize(9);
                                });

                                column.Item()
                                .Row(x =>
                                {
                                    x.AutoItem()
                                    .Text("Dirección :")
                                    .FontSize(9)
                                    .SemiBold();

                                    x.ConstantItem(15);

                                    x.AutoItem()
                                    .Text(comprobanteDto.Cliente!.Direccion) // Aqui va la direccion del cliente
                                    .FontSize(9);
                                });
                            });

                            row.AutoItem()
                            .AlignCenter()
                            .Column(column =>
                            {
                                column.Spacing(4f);

                                column.Item()
                                .Row(x =>
                                {
                                    x.AutoItem()
                                    .Text("Identificación :")
                                    .FontSize(9)
                                    .SemiBold();

                                    x.ConstantItem(15);

                                    x.AutoItem()
                                    .Text(comprobanteDto.Cliente!.Identificacion) // Aqui va la identificacion del cliente
                                    .FontSize(9);
                                });

                                column.Item()
                                .Text(string.Empty);

                                column.Item()
                                .Row(x =>
                                {
                                    x.AutoItem()
                                    .Text("Guía Remisión :")
                                    .FontSize(9)
                                    .SemiBold();
                                });
                            });
                        });

                        // Tabla con los detalles de la venta
                        column.Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(55);
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(55);
                                columns.RelativeColumn();
                                columns.ConstantColumn(50);
                                columns.ConstantColumn(60);
                                columns.ConstantColumn(50);
                            });

                            table.Header(header =>
                            {
                                header.Cell().BorderBottom(2).Element(CellStyle).Text("Codigo Principal").FontSize(8).SemiBold();
                                header.Cell().BorderBottom(2).Element(CellStyle).Text("Codigo Auxiliar").FontSize(8).SemiBold();
                                header.Cell().BorderBottom(2).Element(CellStyle).Text("Cantidad").FontSize(8).SemiBold();
                                header.Cell().BorderBottom(2).Element(CellStyle).Text("Descripcion").FontSize(8).SemiBold();
                                header.Cell().BorderBottom(2).Element(CellStyle).Text("Precio Unitario").FontSize(8).SemiBold();
                                header.Cell().BorderBottom(2).Element(CellStyle).Text("Descuento").FontSize(8).SemiBold();
                                header.Cell().BorderBottom(2).Element(CellStyle).Text("Total").FontSize(8).SemiBold();
                            });

                            var detalles = _mapper.Map<List<DetalleVentaDto>>(detallesDb); // Aqui va la lista del detalle de la venta
                            foreach (var item in detalles)
                            {
                                table.Cell().Element(CellStyle).Text(item.Producto!.CodigoPrincipal).FontSize(8).AlignCenter();
                                table.Cell().Element(CellStyle).Text(item.Producto.CodigoAuxiliar).FontSize(8).AlignCenter();
                                table.Cell().Element(CellStyle).Text(item.Cantidad.ToString()).FontSize(8).AlignRight();
                                table.Cell().Element(CellStyle).Text(item.Detalle).FontSize(8).AlignLeft();
                                table.Cell().Element(CellStyle).Text($"$ {item.Producto.PrecioUnitario}").FontSize(8).AlignRight();
                                table.Cell().Element(CellStyle).Text($"$ {item.Descuento}").FontSize(8).AlignRight();
                                table.Cell().Element(CellStyle).Text($"$ {item.Total}").FontSize(8).AlignRight();
                            }
                        });

                        // Informacion Adicional + Detalle de la venta
                        column.Item()
                        .Row(row =>
                        {
                            row.Spacing(2f, Unit.Millimetre);

                            // Informacion Adicional
                            row.AutoItem()
                            .Column(x =>
                            {
                                x.Spacing(2f, Unit.Millimetre);

                                x.Item()
                                .Border(1)
                                .Column(column =>
                                {
                                    column.Item()
                                    .Border(1)
                                    .Padding(7)
                                    .Text("Informacion Adicional")
                                    .FontSize(9)
                                    .SemiBold()
                                    .AlignCenter();

                                    column.Item()
                                    .Padding(2f, Unit.Millimetre)
                                    .Row(y =>
                                    {
                                        y.AutoItem()
                                        .Text("Email :")
                                        .FontSize(8)
                                        .SemiBold();

                                        y.ConstantItem(15);

                                        y.AutoItem()
                                        .Text(comprobanteDto.Cliente!.Email) // Aqui va el correo del cliente
                                        .FontSize(8);
                                    });

                                    column.Item()
                                    .Padding(2f, Unit.Millimetre)
                                    .Row(y =>
                                    {
                                        y.AutoItem()
                                        .Text("Teléfono :")
                                        .FontSize(8)
                                        .SemiBold();

                                        y.ConstantItem(15);

                                        y.AutoItem()
                                        .Text(comprobanteDto.Cliente!.Telefono) // Aqui va el numero de telefono del cliente
                                        .FontSize(8);
                                    });
                                });

                                x.Item()
                                .Column(z =>
                                {
                                    // Forma de pago
                                    z.Item()
                                   .Border(1)
                                   .Column(x =>
                                   {
                                       x.Item()
                                       .Border(1)
                                       .Padding(7)
                                       .Text("Forma de pago")
                                       .FontSize(9)
                                       .SemiBold()
                                       .AlignCenter();

                                       x.Item()
                                       .Table(table =>
                                       {
                                           table.ColumnsDefinition(columns =>
                                           {
                                               if (plazos > 0)
                                               {
                                                   columns.ConstantColumn(90);
                                                   columns.ConstantColumn(50);
                                                   columns.ConstantColumn(55);
                                                   columns.ConstantColumn(55);
                                               }
                                               else
                                               {
                                                   columns.ConstantColumn(150);
                                                   columns.ConstantColumn(100);
                                               }
                                           });

                                           table.Header(header =>
                                           {
                                               if (plazos > 0)
                                               {
                                                   header.Cell().Element(CellStyle).Text("Forma de pago").FontSize(8).AlignCenter().SemiBold();
                                                   header.Cell().Element(CellStyle).Text("Total").FontSize(8).AlignCenter().SemiBold();
                                                   header.Cell().Element(CellStyle).Text("Plazo").FontSize(8).AlignCenter().SemiBold();
                                                   header.Cell().Element(CellStyle).Text("Tiempo").FontSize(8).AlignCenter().SemiBold();
                                               }
                                               else
                                               {
                                                   header.Cell().Element(CellStyle).Text("Forma de pago").FontSize(8).AlignCenter().SemiBold();
                                                   header.Cell().Element(CellStyle).Text("Total").FontSize(8).AlignCenter().SemiBold();
                                               }
                                           });

                                           if (plazos > 0)
                                           {
                                               table.Cell().Element(CellStyle).Text($"{comprobanteDto.FormaPago} - {FormaPagoExist(comprobanteDto.FormaPago)}").FontSize(8).AlignLeft(); // Aqui el codigo y la forma de pago con la que se realizo la venta
                                               table.Cell().Element(CellStyle).Text(comprobanteDto.Total.ToString()).FontSize(8).AlignRight(); // Aqui va el valor total del pago por la venta que incluye el los impuesto y descuentos
                                               table.Cell().Element(CellStyle).Text($"{plazos}").FontSize(8).AlignCenter().AlignRight();
                                               table.Cell().Element(CellStyle).Text("meses").FontSize(8).AlignCenter().AlignRight();
                                           }
                                           else
                                           {
                                               table.Cell().Element(CellStyle).Text($"{comprobanteDto.FormaPago} - {FormaPagoExist(comprobanteDto.FormaPago)}").FontSize(8).AlignLeft(); // Aqui el codigo y la forma de pago con la que se realizo la venta
                                               table.Cell().Element(CellStyle).Text(comprobanteDto.Total.ToString()).FontSize(8).AlignRight(); // Aqui va el valor total del pago por la venta que incluye el los impuesto y descuentos
                                           }
                                       });
                                   });
                                });
                            });

                            row.AutoItem()
                            .Border(1)
                            .Column(column =>
                            {
                                column.Item()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(150);
                                        columns.ConstantColumn(75);
                                    });

                                    // SUBTOTAL 15 %
                                    table.Cell().Element(CellStyle).Text("SUBTOTAL 15%").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text($"$ {comprobanteDto.Subtotal15}").AlignRight().FontSize(8); // Aqui va el valor de la venta sin iva + los descuentos

                                    // SUBTOTAL 0 %
                                    table.Cell().Element(CellStyle).Text("SUBTOTAL 0 %").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text("$0.00").AlignRight().FontSize(8); // Aqui va el valor de la venta del subtotal 0 que casi siempre es cero

                                    // SUBTOTAL NO OBJETO IVA
                                    table.Cell().Element(CellStyle).Text("SUBTOTAL NO OBJETO IVA").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text("$0.00").AlignRight().FontSize(8); // Aqui va el valor de los productos que no son objeto de iva

                                    // SUBTOTAL SIN IMPUESTOS
                                    table.Cell().Element(CellStyle).Text("SUBTOTAL SIN IMPUESTOS").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text($"$ {comprobanteDto.Subtotal15}").AlignRight().FontSize(8); // Aqui va el valor de la venta sin iva + los descuentos

                                    // SUBTOTAL EXENTO DE IVA
                                    table.Cell().Element(CellStyle).Text("SUBTOTAL EXENTO DE IVA").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text("$0.00").AlignRight().FontSize(8); // Aqui va el valor de la venta que esta exento de iva por lo general es cero

                                    // TOTAL DESCUENTO
                                    table.Cell().Element(CellStyle).Text("TOTAL DESCUENTO").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text($"$ {comprobanteDto.Descuento}").AlignRight().FontSize(8); // Aqui va el valor de los descuentos de los productos en caso de tenerlos

                                    // ICE
                                    table.Cell().Element(CellStyle).Text("ICE").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text("$0.00").AlignRight().FontSize(8); // Aqui va el valor del ICE (Impuesto contribucion especial) que por lo general es cero

                                    // IVA 15%
                                    table.Cell().Element(CellStyle).Text("IVA 15%").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text($"$ {comprobanteDto.TotalIva}").AlignRight().FontSize(8); // Aqui va el valor de la suma del iva de los productos que llevan iba del 15%

                                    // IRBPNR
                                    table.Cell().Element(CellStyle).Text("IRBPNR").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text("$0.00").AlignRight().FontSize(8); // Aqui va el valor del impuesto que se cobra en botellas no retornables o biodegradables

                                    // PROPINA
                                    table.Cell().Element(CellStyle).Text("PROPINA").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text("$0.00").AlignRight().FontSize(8); // Aqui va el valor de la propina que casi siempre es cero

                                    // VALOR TOTAL
                                    table.Cell().Element(CellStyle).Text("VALOR TOTAL").AlignCenter().FontSize(8);
                                    table.Cell().Element(CellStyle).Text($"$ {comprobanteDto.Total}").AlignRight().FontSize(8); // Aqui va el valor total de la venta que incluye la sumatoria de los valores subtotales, ivas, descuentso, etc.
                                });
                            });
                        });
                    });

                    page.Footer()
                    .PaddingTop(2f, Unit.Millimetre)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            var ruta = Path.Combine(_webHostingEnvironment.ContentRootPath + "\\Archivos", @"FacturasPDF");

            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }

            using var stream = new FileStream(Path.Combine(ruta, $"Factura-{comprobanteDto.DocSri}" + ".pdf"), FileMode.Create);

            documentPdf.GeneratePdf(stream);

            var rutaDocumentoPdf = ruta + @$"\Factura-{comprobanteDto.DocSri}.pdf";

            _response.IsSuccess = true;
            _response.Message = "Se genero el documento PDF de la factura";
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = rutaDocumentoPdf;

            return _response;
        }

        /// <summary>
        /// Este método permite obtener el estilo de las celdas de la tabla para la factura en pdf.
        /// </summary>
        /// <param name="container">es el contendor de la celda que se va a utilizar en la tabla.</param>
        /// <returns>Retorna el estilo del contenedor de la celda de la tabla.</returns>
        private static IContainer CellStyle(IContainer container)
        {
            return container.Border(1).Padding(7);
        }

        public async Task<Response> GenerarVenta(VentaDto ventaDto)
        {
            using var transaccion = await _db.Database.BeginTransactionAsync();

            try
            {
                var clienteExist = await _clienteRepository.GetAsync(u => u.Identificacion == ventaDto.IdentificacionCliente, tracked: false);
                var empresaExist = await _empresaRepository.GetAsync(u => u.Ruc == ventaDto.RucEmpresa, tracked: false);
                var productsInCart = JsonConvert.DeserializeObject<List<ShoppingCartDto>>(ventaDto.ShoppingCart);
                if (empresaExist == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "La empresa no esta registrada.";
                    _response.Result = null;

                    return _response;
                }

                if (clienteExist == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Message = "Cliente no registrado. Regístrese por favor!.";
                    _response.Result = null;

                    return _response;
                }

                if (productsInCart == null || productsInCart.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = "El carrito esta vacío.";
                    _response.Result = null;

                    return _response;
                }

                // 1. Se genera el comprobante
                if (string.IsNullOrEmpty(FormaPagoExist(ventaDto.FormaPago)))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = "No está registrada la forma de pago";
                    _response.Result = null;

                    return _response;
                }

                var formaPago = ventaDto.FormaPago; // Se almacena el codigo de la forma de pago de acuerdo al sri
                var tipoComprobante = "01"; // Se almacena el codigo del comprobante que acepta el sri
                var comprobantes = await _comprobanteVentaRepository.GetAllAsync();
                int ultimoComprobante = comprobantes.Count > 0 ? comprobantes.OrderByDescending(x => x.Id).First().Id : 0;

                // Se genera el codigo de 9 digitos
                string comprobanteid = "000000000";
                int comprobanteidNum = "000000000".Length;

                int comprobante = ultimoComprobante.ToString().Length;

                // Esta seccion permite generar el numero de comprobante con 9 digitos, considerando la longitud del Id del ultimo comprobante
                string comprobanteNum9 = comprobanteid.Insert(comprobanteidNum - comprobante, (ultimoComprobante + 1).ToString());
                string comprobanteNumero9 = comprobanteNum9.Substring(0, 9); // permite rellenar con una subcadena de 0 hasta completar 9 digitos que incluye el valor del ultimo comprobante + 1

                // Proceso para generar la clave de acceso(DocSri)
                Random generator = new();
                string numeroGenerado = generator.Next(0, 10000000).ToString("D8"); // permite generar un numero aleatorio de 8 digitos enteros
                string numeroComprobante = "001-" + "001-" + comprobanteNumero9; // es el formato del numero de comprobante que acepta el SRI considerando el punto de emision
                                                                                 // y el establecimiento que para este caso es 001 en ambos casos debido a que solo existe una sola tienda
                var secuencial = numeroComprobante.Split("-");
                var fechaEmision = DateTime.Now;

                // Para la clave de acceso basta con obtener el valor de "Ambiente" del objeto empresa para gestionar
                // la clave de acceso, asi como, realizar la gestion de los documentos xml necesarios para generar la
                // factura electronica
                string claveAcceso = _serviceSRI.GenerarClaveAcceso(fechaEmision.ToString("ddMMyyyy"), tipoComprobante, empresaExist.Ruc, empresaExist.Ambiente.ToString(), secuencial[0].ToString(), secuencial[1].ToString(), secuencial[2].ToString(), numeroGenerado);

                if (string.IsNullOrEmpty(claveAcceso))
                {
                    _response.IsSuccess = false;
                    _response.Message = "No se pudo generar la clave de acceso correctamente";
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Result = null;

                    return _response;
                }

                ComprobanteVentaDto comprobanteVenta = new()
                {
                    IdEmpresa = empresaExist.Id,
                    IdCliente = clienteExist.Id,
                    FechaEmision = fechaEmision,
                    TipoComprobante = "01", // Es el tipo de comprobante que para este caso es de factura
                    NumeroComprobante = numeroComprobante,
                    FormaPago = ventaDto.FormaPago,
                    Descuento = ventaDto.Descuento,
                    Subtotal0 = 0.00M,
                    Subtotal15 = ventaDto.SubTotal15,
                    TotalIva = ventaDto.TotalIVA,
                    Total = ventaDto.Total,
                    DocSri = claveAcceso,
                    Propina = 0.00M,
                };

                var comprobanteGeneradoResponse = await GenerarComprobanteVenta(comprobanteVenta);

                if (!comprobanteGeneradoResponse.IsSuccess)
                {
                    await transaccion.RollbackAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = comprobanteGeneradoResponse.Message;
                    _response.Result = null;

                    return _response;
                }

                // 2. Se generan los detalles de la venta
                var comprobanteTransaccion = (ComprobanteVentaDto)comprobanteGeneradoResponse.Result;
                var responseGenerarDetalle = await GenerarDetalleVenta(comprobanteTransaccion!.Id, productsInCart);

                if (!responseGenerarDetalle.IsSuccess)
                {
                    await transaccion.RollbackAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = responseGenerarDetalle.Message;
                    _response.Result = null;

                    return _response;
                }

                var productosDetalleVenta = (List<DetalleVentaDto>)responseGenerarDetalle.Result;

                // 3. Se genera el pago con kushki
                // Primero se verifica que se haya generado el comprobante para la factura
                var comprobanteConDocSri = await _comprobanteVentaRepository.GetAsync(u => u.Id == comprobanteTransaccion.Id, tracked: false);
                if (comprobanteConDocSri == null)
                {
                    await transaccion.RollbackAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = "No se encontró el comprobante de venta con el DocSri para la facturacion. Intentelo nuevamente.";
                    _response.Result = null;

                    return _response;
                }

                // Se realiza el cobro mediante la plataforma de kushki antes de generar el archivo xml
                var responseTransactionKushki = await _kushkiService.GenerarPagoKushki(ventaDto, productsInCart, _mapper.Map<ClienteDto>(clienteExist), comprobanteConDocSri.NumeroComprobante);

                if (!responseTransactionKushki.IsSuccess)
                {
                    await transaccion.RollbackAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = responseTransactionKushki.Message;
                    _response.Result = null;

                    return _response;
                }

                // 4. Se genera el archivo xml de la factura
                var isXmlGenerated = _serviceSRI.GenerarXML(_mapper.Map<EmpresaDto>(empresaExist), _mapper.Map<ComprobanteVentaDto>(comprobanteConDocSri), _mapper.Map<ClienteDto>(clienteExist), _mapper.Map<List<DetalleVentaDto>>(productosDetalleVenta), ventaDto.Plazos);

                if (!isXmlGenerated.IsSuccess)
                {
                    await transaccion.RollbackAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = "No se genero el xml la venta para la facturacion. Intentelo nuevamente.";
                    _response.Result = null;

                    return _response;
                }

                // Permite obtener la ruta de un documento (factura) xml para comprobar si existe o no, y en caso de que exista la ruta con la clave de acceso, se debe realizar una nueva gestion.
                var rutaXml = await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == comprobanteConDocSri!.DocSri, tracked: false);

                // Permite verificar si existe la ruta del comprobante generado con la clave de acceso para la facturacion electronica con el SRI
                if (rutaXml != null)
                {
                    await transaccion.RollbackAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = "Ya existe un archivo xml con una clave de acceso similar. Intentelo nuevamente.";
                    _response.Result = null;

                    return _response;
                }

                // 5. Se firma el archivo xml
                var firmaResponse = _serviceSRI.FirmarXML(_mapper.Map<EmpresaDto>(empresaExist), comprobanteConDocSri.DocSri, isXmlGenerated.PathXML);
                if (!firmaResponse.IsSuccess)
                {
                    await transaccion.RollbackAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = "No se pudo firmar el archivo xml. Intentelo nuevamente.";
                    _response.Result = null;

                    return _response;
                }

                // 6. Se realiza la recepcion del documento firmado en el sri
                // var responseRecepcion = _serviceSRI.RecepcionSRI(firmaResponse.RutaXmlFirmado, comprobanteConDocSri.DocSri);
                var responseRecepcion = await _serviceSRI.RecepcionSRI(firmaResponse.RutaXmlFirmado, comprobanteConDocSri.DocSri);

                if (!responseRecepcion.Estado.Equals("RECIBIDA"))
                {
                    RutasFacturacionDto rutaNoRecibida = new()
                    {
                        IdEmpresa = empresaExist.Id,
                        ClaveAcceso = comprobanteConDocSri.DocSri,
                        EstadoRecepcion = responseRecepcion.Estado,
                        PathXMLPDF = null,
                        RutaGenerados = firmaResponse.RutaXmlGenerado,
                        RutaFirmados = firmaResponse.RutaXmlFirmado,
                        RutaAutorizados = null,
                    };

                    await _rutasFacturacionRepository.CreateRutasFacturacionAsync(rutaNoRecibida);

                    await transaccion.CommitAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = $"La factura no ha sido recibida en el SRI. El error esta en: {responseRecepcion.Estado}";
                    _response.Result = null;

                    return _response;
                }

                // 7. Se realiza la autorizacion del documento en el sri
                // var responseAutorizacion = _serviceSRI.AutorizacionSRI(_mapper.Map<EmpresaDto>(empresaExist), comprobanteConDocSri.DocSri);
                var responseAutorizacion = await _serviceSRI.AutorizacionSRI(_mapper.Map<EmpresaDto>(empresaExist), comprobanteConDocSri.DocSri);

                if (!responseAutorizacion.Estado.Equals("AUTORIZADO"))
                {
                    // Se registra las rutas de los archivos generados , firmados y no autorizados
                    RutasFacturacionDto rutaNoAutorizada = new()
                    {
                        IdEmpresa = empresaExist.Id,
                        ClaveAcceso = comprobanteConDocSri.DocSri,
                        EstadoRecepcion = responseAutorizacion.Estado,
                        PathXMLPDF = null,
                        RutaGenerados = firmaResponse.RutaXmlGenerado,
                        RutaFirmados = firmaResponse.RutaXmlFirmado,
                        RutaAutorizados = responseAutorizacion.PathXMLAutorizado,
                    };

                    await _rutasFacturacionRepository.CreateRutasFacturacionAsync(rutaNoAutorizada);

                    await transaccion.CommitAsync();

                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = $"La factura no ha sido autorizada en el SRI. El error esta en: {responseRecepcion.Estado}";
                    _response.Result = null;

                    return _response;
                }

                // 8. Se genera el RIDE y PDF
                var generarFacturaPdf = await GenerarRideYPdf(comprobanteTransaccion, isXmlGenerated.PathXML, ventaDto.Plazos);
                if (!generarFacturaPdf.IsSuccess)
                {
                    _response.Message = generarFacturaPdf.Message;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = generarFacturaPdf.IsSuccess;
                    _response.Result = null;

                    return _response;
                }

                // 9. Se guardan la rutas de los archivos generados, firmados, y autorizados
                RutasFacturacionDto rutaAutorizada = new()
                {
                    IdEmpresa = empresaExist.Id,
                    ClaveAcceso = comprobanteConDocSri.DocSri,
                    EstadoRecepcion = responseAutorizacion.Estado,
                    PathXMLPDF = (string)generarFacturaPdf.Result,
                    RutaGenerados = firmaResponse.RutaXmlGenerado,
                    RutaFirmados = firmaResponse.RutaXmlFirmado,
                    RutaAutorizados = responseAutorizacion.PathXMLAutorizado,
                };

                await _rutasFacturacionRepository.CreateRutasFacturacionAsync(rutaAutorizada);

                await transaccion.CommitAsync();

                // 10. Se envía la factura al cliente por correo electrónico
                var emailResponse = await EnviarFactura(comprobanteTransaccion.Id);

                if (!emailResponse.IsSuccess)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = emailResponse.Message;
                    _response.Result = null;

                    return _response;
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = "Se ha generado la factura electrónica con éxito.";
                _response.Result = null;

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";
                _response.Result = null;

                return _response;
            }
        }

        public async Task<Response> GetAllComprobanteVenta(string? query = null, string? startDate = null, string? endDate = null)
        {
            try
            {
                List<TblComprobanteVenta> comprobantes;

                if (!string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var start = JsonConvert.DeserializeObject<DateTime>(startDate);
                    var end = JsonConvert.DeserializeObject<DateTime>(endDate);
                    comprobantes = await _comprobanteVentaRepository.GetAllAsync(u => (u.Cliente!.Identificacion.Contains(query) || u.NumeroComprobante.Contains(query)) && u.FechaEmision <= end && u.FechaEmision >= start, includeProperties: "Empresa,Cliente");
                }
                else if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    var start = JsonConvert.DeserializeObject<DateTime>(startDate);
                    var end = JsonConvert.DeserializeObject<DateTime>(endDate);
                    comprobantes = await _comprobanteVentaRepository.GetAllAsync(u => u.FechaEmision <= end && u.FechaEmision >= start, includeProperties: "Empresa,Cliente");
                }
                else
                {
                    comprobantes = !string.IsNullOrEmpty(query)
                        ? await _comprobanteVentaRepository.GetAllAsync(u => u.Cliente!.Identificacion == query || u.NumeroComprobante.Contains(query), includeProperties: "Empresa,Cliente")
                        : await _comprobanteVentaRepository.GetAllAsync(includeProperties: "Empresa,Cliente");
                }

                if (comprobantes.Count > 0)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Message = "Se han obtenido los registros solicitados";
                    _response.Result = _mapper.Map<List<ComprobanteVentaDto>>(comprobantes);

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Message = "No se han obtenido los registros solicitados";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";

                return _response;
            }
        }

        public async Task<Response> GetAllDetalleVenta(int comprobanteId = 0)
        {
            try
            {
                List<TblDetalleVenta> detallesVenta = comprobanteId != 0
                    ? await _detalleVentaRepository.GetAllAsync(u => u.IdComprobanteVenta == comprobanteId, includeProperties: "ComprobanteVenta,Producto")
                    : await _detalleVentaRepository.GetAllAsync(includeProperties: "ComprobanteVenta,Producto");

                if (detallesVenta.Count > 0)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Message = "Se han obtenido los registros solicitados";
                    _response.Result = _mapper.Map<List<DetalleVentaDto>>(detallesVenta);

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Message = "No se han obtenido los registros solicitados";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";

                return _response;
            }
        }

        public async Task<Response> GetAllDocumentoXML(string? query = null)
        {
            try
            {
                List<TblRutasXML> rutasXml = query != null ? await _rutasFacturacionRepository.GetAllAsync(u => u.EstadoRecepcion == query, includeProperties: "Empresa")
                : await _rutasFacturacionRepository.GetAllAsync(includeProperties: "Empresa");

                if (rutasXml.Count > 0)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Message = "Se han obtenido los registros solicitados";
                    _response.Result = _mapper.Map<List<RutasFacturacionDto>>(rutasXml);

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Message = "No se han obtenido los registros solicitados";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error inesperado. Error: {ex.Message}";

                return _response;
            }
        }

        public async Task<Response> GetComprobanteVenta(int id, string? query = null)
        {
            try
            {
                TblComprobanteVenta comprobante;

                if (!string.IsNullOrEmpty(query))
                {
                    comprobante = await _comprobanteVentaRepository.GetAsync(u => u.Cliente!.Identificacion.Contains(query) || u.NumeroComprobante.Contains(query) || u.DocSri == query, tracked: false, includeProperties: "Empresa,Cliente");
                }
                else if (id != 0)
                {
                    comprobante = await _comprobanteVentaRepository.GetAsync(u => u.Id == id, tracked: false, includeProperties: "Empresa,Cliente");
                }
                else
                {
                    comprobante = await _comprobanteVentaRepository.GetAsync(tracked: false, includeProperties: "Empresa,Cliente");
                }

                if (comprobante != null)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Message = "Se han obtenido los registros solicitados";
                    _response.Result = _mapper.Map<ComprobanteVentaDto>(comprobante);

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Message = "No se han obtenido el registros solicitado";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error. Error: {ex.Message}";

                return _response;
            }
        }

        public async Task<Response> GetDocumentoXML(int id, string? query = null)
        {
            try
            {
                TblRutasXML rutaXml = !string.IsNullOrEmpty(query)
                    ? await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == query, tracked: false, includeProperties: "Empresa")
                    : await _rutasFacturacionRepository.GetAsync(u => u.Id == id, tracked: false, includeProperties: "Empresa");

                if (rutaXml != null)
                {
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Message = "Se han obtenido los registros solicitados";
                    _response.Result = _mapper.Map<RutasFacturacionDto>(rutaXml);

                    return _response;
                }

                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.Message = "No se han obtenido los registros solicitados";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.Message = $"Ha ocurrido un error inesperado. Error: {ex.Message}";

                return _response;
            }
        }

        /// <summary>
        /// Este metodo permite enviar la factura electronica por correo al cliente registrado.
        /// </summary>
        /// <returns>Retorna un mensaje afirmativo o negativo segun se haya o no enviado el correo.</returns>
        private async Task<Response> EnviarFactura(int comprobanteId) // Aqui se deben cambiar las rutas porque aun no esta validada la firma en los documentos xml, Se debe revisar
        {
            try
            {
                var comprobanteDb = await _comprobanteVentaRepository.GetAsync(u => u.Id == comprobanteId, tracked: false, includeProperties: "Empresa,Cliente");

                if (comprobanteDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = $"No se pudo enviar el mensaje";

                    return _response;
                }

                var rutaXmlDb = await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == comprobanteDb.DocSri, tracked: false);

                if (rutaXmlDb == null || (string.IsNullOrEmpty(rutaXmlDb.RutaGenerados) && string.IsNullOrEmpty(rutaXmlDb.PathXMLPDF)))
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Message = $"No se pudo encontrar las rutas de los archivos";

                    return _response;
                }

                var message = new Message([comprobanteDb.Cliente!.Email], "Es una prueba del correo", "Si lees esto, es porque salió bien =)", [rutaXmlDb.RutaAutorizados!, rutaXmlDb.PathXMLPDF!]);
                _emailService.SendEmail(message);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Message = $"Se envío el mensaje.";

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Message = $"No se pudo enviar el mensaje. Error por: {ex.Message}";

                return _response;
            }
        }

        private static string FormaPagoExist(string formaPago)
        {
            Dictionary<string, string> formaDePago = new()
            {
                { "01", "SIN UTILIZACION DEL SISTEMA FINANCIERO" },
                { "15", "COMPENSACIÓN DE DEUDAS" },
                { "16", "TARJETA DE DÉBITO" },
                { "17", "DINERO ELECTRÓNICO" },
                { "18", "TARJETA PREPAGO" },
                { "19", "TARJETA DE CRÉDITO" },
                { "20", "OTROS CON UTILIZACIÓN DEL SISTEMA FINANCIERO" },
                { "21", "ENDOSO DE TÍTULOS" },
            };

            return formaDePago[formaPago] ?? string.Empty;
        }
    }
}
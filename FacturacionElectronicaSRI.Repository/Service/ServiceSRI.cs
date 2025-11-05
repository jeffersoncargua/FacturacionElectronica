using AutoMapper;
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using FacturacionElectronicaSRI.Repository.Service.IService;
using FacturacionElectronicaSRI.Repository.Service.SRIWebServices;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Repository.Service
{
    public class ServiceSRI : IServiceSRI
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IComprobanteVentaRepository _comprobanteVenta;
        private readonly IProductoRepository _productoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IComprobanteVentaRepository _comprobanteVentaRepository;
        private readonly IDetalleVentaRepository _detalleVentaRepository;
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly IRutasFacturacionRepository _rutasFacturacionRepository;
        private readonly IMapper _mapper;
        private readonly ICertificadoService _certificadoService;
        internal CSRIws _cSriWebService;
        public ServiceSRI(
            IEmpresaRepository empresaRepository,
            IComprobanteVentaRepository comprobanteVenta,
            IProductoRepository productoRepository,
            IClienteRepository clienteRepository,
            IComprobanteVentaRepository comprobanteVentaRepository,
            IDetalleVentaRepository detalleVentaRepository,
            IHostingEnvironment webHostEnvironment,
            IAlmacenadorArchivos almacenadorArchivos,
            IRutasFacturacionRepository rutasFacturacionRepository,
            IMapper mapper,
            ICertificadoService certificadoService)
        {
            _empresaRepository = empresaRepository;
            _comprobanteVenta = comprobanteVenta;
            _productoRepository = productoRepository;
            _clienteRepository = clienteRepository;
            _comprobanteVentaRepository = comprobanteVentaRepository;
            _detalleVentaRepository = detalleVentaRepository;
            _webHostEnvironment = webHostEnvironment;
            _almacenadorArchivos = almacenadorArchivos;
            _rutasFacturacionRepository = rutasFacturacionRepository;
            _mapper = mapper;
            _certificadoService = certificadoService;
            this._cSriWebService = new ();
        }

        /// <summary>
        /// Este medoto permite realizar la solicitud para la aprobacion del xml firmado para continuar el proceso de facturacion electronica.
        /// </summary>
        /// <param name="claveAcceso">Es el parametro que se va a utilizar en la solictud de la autorizacion del xml firmado para la facturacion electronica.</param>
        /// <returns>Retorna la respuesta de la solicitud de autorizacion del sri que puede ser "AUTORIZADO" de estar correctamente el proceso, caso contrario recibirimos el mensaje de error.</returns>
        public CRespuestaAutorizacion AutorizacionComprobante(string claveAcceso)
        {
            CRespuestaAutorizacion respuestaAutorizacion = _cSriWebService.AutorizacionComprobanteOnLinePrueba(claveAcceso);

            return respuestaAutorizacion;
        }

        /// <summary>
        /// Este metodo permite empezar con el proceso de autorizacion del elemento xml firmado para continuar con el procesos de facturacion electronica.
        /// Tambien se realiza la generacion del xml firmado y autorizado para realizar la facturacion electronica del sri.
        /// </summary>
        /// <param name="claveAcceso">Este parametro permite obtener el registro del xml para la facturacion electronica.</param>
        /// <param name="rucEmpresa">Este parametro se utiliza para armar la ruta para el xml autorizado para continuar con la facturacion electronica.</param>
        /// <returns>Retorna una respuesta satisfactoria en caso de realizar el proceso de la autorizacion correctamente, caso contrario se indica que algo salio mal y se realizada el proceso correctivo.</returns>
        public async Task<ResponseAutorizacionSRI> AutorizacionSRI(string claveAcceso, string rucEmpresa)
        {
            string carpetaXMLAutorizado = Path.Combine(_webHostEnvironment.WebRootPath, "DocumentosAutorizados"); // ruta para los xml autorizados

            // Esta funcion permite generar la carpeta de los xml autorizados en caso de que no exista
            if (!Directory.Exists(carpetaXMLAutorizado))
            {
                Directory.CreateDirectory(carpetaXMLAutorizado);
            }

            string carpetaXMLNoAutorizado = Path.Combine(_webHostEnvironment.WebRootPath, "DocumentosNoAutorizados"); // ruta para los xml no autorizados

            // Esta funcion permite generar la carpeta de los xml no autorizados en caso de que no exista
            if (!Directory.Exists(carpetaXMLAutorizado))
            {
                Directory.CreateDirectory(carpetaXMLNoAutorizado);
            }

            var rutaXmlDb = await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == claveAcceso);
            if (rutaXmlDb == null)
            {
                return new ResponseAutorizacionSRI { Estado = "No se encontro el xml firmado" };
            }

            var autorizacion = AutorizacionComprobante(claveAcceso);
            if (autorizacion.Comprobantes[0].Estado.Equals("EN PROCESO"))
            {
                return new ResponseAutorizacionSRI
                {
                    Estado = autorizacion.Estado,
                    ClaveAcceso = claveAcceso,
                    Code = 201,
                };
            }

            if (autorizacion.Comprobantes[0].Estado.Equals("AUTORIZADO"))
            {
                // Si esta autorizado se modifica el archivo XML firmado y la ruta del archivo para continuar con el proceso de facturacion electronica.
                var autoriza = XMLAutorizado(
                    autorizacion.Comprobantes[0].Comprobante,
                    _webHostEnvironment.WebRootPath + @"/FacturacionElectronicaEmpresa-" + rucEmpresa + @"/DocumentosAutorizados/" + autorizacion.ClaveAcceso + ".xml",
                    autorizacion.Comprobantes[0].Estado,
                    autorizacion.ClaveAcceso,
                    autorizacion.Comprobantes[0].FechaAutorizacion);

                if (autoriza)
                {
                    string rutaXmlAutorizada = _webHostEnvironment.WebRootPath + @"/FacturacionElectronicaEmpresa-" + rucEmpresa + @"/DocumentosAutorizados/" + autorizacion.ClaveAcceso + ".xml";

                    RutasFacturacionDto rutaAutorizada = new()
                    {
                        Id = rutaXmlDb.Id,
                        IdEmpresa = rutaXmlDb.IdEmpresa,
                        ClaveAcceso = rutaXmlDb.ClaveAcceso,
                        EstadoRecepcion = autorizacion.Comprobantes[0].Estado,
                        PathXMLPDF = rutaXmlDb.PathXMLPDF,
                        RutaGenerados = rutaXmlDb.RutaGenerados,
                        RutaFirmados = rutaXmlDb.RutaFirmados,
                        RutaAutorizados = rutaXmlAutorizada,
                    };

                    await _rutasFacturacionRepository.UpdateRutasFacturacionAsync(rutaXmlDb.Id, rutaAutorizada);

                    return new ResponseAutorizacionSRI { Estado = autorizacion.Comprobantes[0].Estado };
                }

                return new ResponseAutorizacionSRI
                {
                    Estado = $"{autorizacion.Comprobantes[0].Mensajes[0].InformacionAdicional?.ToString()} /InformacionAdicional: {autorizacion.Comprobantes[0].Mensajes[0].InformacionAdicional?.ToString()}",
                    ClaveAcceso = claveAcceso,
                    Code = 500,
                };
            }

            string rutaXmlNoAutorizada = _webHostEnvironment.WebRootPath + @"/FacturacionElectronicaEmpresa-" + rucEmpresa + @"/DocumentosNoAutorizados/" + autorizacion.ClaveAcceso + ".xml";

            RutasFacturacionDto rutaNoAutorizada = new()
            {
                Id = rutaXmlDb.Id,
                IdEmpresa = rutaXmlDb.IdEmpresa,
                ClaveAcceso = rutaXmlDb.ClaveAcceso,
                EstadoRecepcion = autorizacion.Comprobantes[0].Estado,
                PathXMLPDF = rutaXmlDb.PathXMLPDF,
                RutaGenerados = rutaXmlDb.RutaGenerados,
                RutaFirmados = rutaXmlDb.RutaFirmados,
                RutaAutorizados = rutaXmlNoAutorizada,
            };

            XMLNoAutorizado(
                    autorizacion.Comprobantes[0].Comprobante,
                    _webHostEnvironment.WebRootPath + @"/FacturacionElectronicaEmpresa-" + rucEmpresa + @"/DocumentosNoAutorizados/" + autorizacion.ClaveAcceso + ".xml");

            await _rutasFacturacionRepository.UpdateRutasFacturacionAsync(rutaXmlDb.Id, rutaNoAutorizada);

            return new ResponseAutorizacionSRI
            {
                Estado = $"{autorizacion.Comprobantes[0].Mensajes[0].InformacionAdicional?.ToString()} /InformacionAdicional: {autorizacion.Comprobantes[0].Mensajes[0].InformacionAdicional?.ToString()}",
                ClaveAcceso = claveAcceso,
                Code = 500,
            };
        }

        /// <summary>
        /// Este metodo permite firmar el xml para realizar el proceso de la Facturacion Electronica del SRI.
        /// </summary>
        /// <param name="claveAcceso">Este parametro permite realizar la busqueda del xml generado para luego firmalo y continuar con el proceso de facturacion electronica.</param>
        /// <param name="rucEmpresa">Este parametro permite realizar la busqueda de la empresa para la facturacion electronica.</param>
        /// <returns>Retorna una respuesta indicando si se realizo la firma en el documento o no.</returns>
        public async Task<ViewXmlDto> FirmarXML(string claveAcceso, string rucEmpresa)
        {
            string carpetaXmlFirmados = Path.Combine(_webHostEnvironment.ContentRootPath, "FacturacionElectronicaEmpresa-" + rucEmpresa, "DocumentosFirmados"); // ruta para los xml firmados
            if (!Directory.Exists(carpetaXmlFirmados))
            {
                Directory.CreateDirectory(carpetaXmlFirmados);
            }

            try
            {
                var empresaDb = await _empresaRepository.GetAsync(u => u.Ruc == rucEmpresa, tracked: false);
                if (empresaDb == null)
                {
                    return new ViewXmlDto() { Id = 0, RucEmpresa = string.Empty, RutaXmlGenerado = string.Empty, RutaXmlFirmado = string.Empty, Mensaje = $"No exite la empresa" };
                }

                var rutaXmlDb = await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == claveAcceso);
                if (rutaXmlDb != null)
                {
                    _certificadoService.CargarDesdeP12(empresaDb.PathCertificado, empresaDb.Contraseña); // Se carga el x509certificate con el certificado p12 para la firma del documento xml
                    var xml = _certificadoService.FirmarDocumentoXml(rutaXmlDb.RutaGenerados!); // se firma el documento y nos lo devuelve

                    string rutaXmlFirmado = _webHostEnvironment.WebRootPath + @"/FacturacionElectronicaEmpresa-" + empresaDb.Ruc + @"/DocumentosFirmados/" + claveAcceso + ".xml";
                    xml.Save(rutaXmlFirmado); // se almacena el xml firmado en el path de la rutaXmlFirmado

                    RutasFacturacionDto rutaConFirma = new()
                    {
                        Id = rutaXmlDb.Id,
                        IdEmpresa = rutaXmlDb.IdEmpresa,
                        ClaveAcceso = rutaXmlDb.ClaveAcceso,
                        RutaGenerados = rutaXmlDb.RutaGenerados,
                        RutaFirmados = rutaXmlFirmado,
                        RutaAutorizados = null,
                        EstadoRecepcion = null,
                        PathXMLPDF = null,
                    };

                    // Se actualiza la ruta con el xml firmado
                    await _rutasFacturacionRepository.UpdateRutasFacturacionAsync(rutaXmlDb.Id, rutaConFirma);

                    return new ViewXmlDto() { Id = rutaXmlDb.Id, RucEmpresa = empresaDb.Ruc, RutaXmlGenerado = rutaXmlDb.RutaGenerados, RutaXmlFirmado = rutaXmlFirmado, Mensaje = "Se firmo correctamente" };
                }

                return new ViewXmlDto() { Id = 0, RucEmpresa = string.Empty, RutaXmlGenerado = string.Empty, RutaXmlFirmado = string.Empty, Mensaje = $"No exite la ruta del xml generado" };
            }
            catch (Exception ex)
            {
                return new ViewXmlDto() { Id = 0, RucEmpresa = string.Empty, RutaXmlGenerado = string.Empty, RutaXmlFirmado = string.Empty, Mensaje = $"No se firmo correctamente {ex}" };
            }
        }

        public string GenerarClaveAcceso(string fecha, string tipoComprobante, string rucEmpresa, string ambiente, string estab, string ptoEmi, string secuencial, string idCod)
        {
            int suma = 0, factor = 7;
            var claveAcceso = fecha + tipoComprobante + rucEmpresa + ambiente + estab + ptoEmi + secuencial + idCod;
            var clave = claveAcceso.ToCharArray();
            foreach (var item in clave)
            {
                suma += Convert.ToInt32(item.ToString()) * factor;
                factor--;
                if (factor == 1)
                {
                    factor = 7;
                }
            }

            var digitoVerificador = suma % 11;
            digitoVerificador = 11 - digitoVerificador;

            if (digitoVerificador == 11)
            {
                digitoVerificador = 0;
            }
            else if (digitoVerificador == 10)
            {
                digitoVerificador = 1;
            }

            return $"{claveAcceso}{digitoVerificador}";
        }

        /// <summary>
        /// La tarea GenerarXML permite generar un archivo xml para realizar la facturacion electronica del SRI.
        /// </summary>
        /// <param name="rucEmpresa">Este parametro permite obtener la informacion de la empresa para la facturacion.</param>
        /// <param name="idComprobanteVenta">Este parametro permite obtener la informacion del comprobante para la facturacion.</param>
        /// <returns>Retorna verdadero si se genera el archivo xml caso contrario retorna false.</returns>
        public async Task<bool> GenerarXML(string rucEmpresa, int idComprobanteVenta)
        {
            string carpetaXMLProcesado = Path.Combine(_webHostEnvironment.WebRootPath, "FacturaElectronicaProcesados");

            // Esta funcion permite generar la carpeta de los xml generados en caso de que no exista
            if (!Directory.Exists(carpetaXMLProcesado))
            {
                Directory.CreateDirectory(carpetaXMLProcesado);
            }

            // Se obtiene valida la existencia del empresa
            var empresaDb = await _empresaRepository.GetAsync(u => u.Ruc == rucEmpresa, tracked: false);
            if (empresaDb == null)
            {
                return false;
            }

            // Se obtiene el XML
            var facturaXML = await GetXMLFactura(idComprobanteVenta, empresaDb!.Ambiente, empresaDb.Ruc);
            if (facturaXML == null)
            {
                return false;
            }

            /*// Se serializa el xmlDocument recibido desde GetXMLFactura para alamacenarlo en caso de ser necesario
            // string xmlSerializado = SerializarAXML(facturaXML);*/

            // Guardar XML en un archivo
            // GuardarXmlEnArchivo(xmlSerializado, "modelo.xml");

            // if(facturaXml == null)
            // return BadRequest(error: "No existe xml")
            var factura = facturaXML.InnerXml;
            factura = factura.Replace("&lt;", "<");

            factura = factura.Replace("&gt;", ">");

            var xmlByte = Encoding.UTF8.GetBytes(factura);

            // Se obtiene el comprobante con el DocSRI generado en GenerarFacturaXML para continuar con el procedimiento de la facturacion electronica
            var comprobanteDb = await _comprobanteVenta.GetAsync(u => u.Id == idComprobanteVenta);

            // Permite crear la ruta de almacenamiento del archivo XML proceso o generado para agregarlo en la base de datos
            var ruta = await _almacenadorArchivos.GuardarArchivo(xmlByte, ".xml", carpetaXMLProcesado, comprobanteDb.DocSri);

            var rutasXml = await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == comprobanteDb.DocSri);

            // Permite verificar si existe la ruta del comprobante generado con la clave de acceso para la facturacion electronica con el SRI
            if (rutasXml == null)
            {
                var rutaXml = new RutasFacturacionDto()
                {
                    RutaFirmados = null,
                    RutaAutorizados = null,
                    RutaGenerados = ruta,
                    ClaveAcceso = comprobanteDb.DocSri,
                    IdEmpresa = empresaDb.Id,
                    PathXMLPDF = null,
                    EstadoRecepcion = null,
                };

                await _rutasFacturacionRepository.CreateAsyn(_mapper.Map<TblRutasXML>(rutaXml));
            }

            return true;
        }

        /// <summary>
        /// Este metodo permite obtener un documento XML a partir de un XDOcument para realizar la facturacion electronica.
        /// </summary>
        /// <param name="document">Este parametro es el XDocument que se va a convertir a XML documento.</param>
        /// <returns>Retorna un elemento XML si esta bien, caso contrario retorna null.</returns>
        public XmlDocument GetXmlDocument(XDocument? document)
        {
            if (document == null)
            {
                return null;
            }

            using XmlReader xmlReader = document.CreateReader();
            XmlDocument xmlDoc = new();
            xmlDoc.Load(xmlReader); // permite cargar la informacion leida de un Xdocument en el xmlDoc creado
            if (document.Declaration != null)
            {
                XmlDeclaration dec = xmlDoc.CreateXmlDeclaration(document.Declaration.Version, document.Declaration.Encoding, document.Declaration.Standalone);
                xmlDoc.InsertBefore(dec, xmlDoc.DocumentElement); // permite insertar el nuevo node luego del ultimo nodo de xmlDoc
            }

            return xmlDoc;
        }

        public async Task<XmlDocument> GetXMLFactura(int idComprobanteVenta, int ambiente, string rucEmpresa)
        {
            XDocument? m_doc = new();

            // Se comprueba que el comprobante de venta exista
            var comprobanteDb = await _comprobanteVentaRepository.GetAsync(u => u.Id == idComprobanteVenta, tracked: false);
            if (comprobanteDb == null)
            {
                m_doc = null;
                return GetXmlDocument(m_doc);
            }

            // Se comprueba que la empresa exista
            var empresaDb = await _empresaRepository.GetAsync(u => u.Ruc == rucEmpresa, tracked: false);
            if (empresaDb == null)
            {
                m_doc = null;
                return GetXmlDocument(m_doc);
            }

            // Se comprueba que el cliente exista
            var clienteDb = await _clienteRepository.GetAsync(u => u.Id == comprobanteDb.IdCliente, tracked: false);
            if (clienteDb == null)
            {
                m_doc = null;
                return GetXmlDocument(m_doc);
            }

            // Se comprueba que el detalle de venta exista
            var detalleVentaDb = await _detalleVentaRepository.GetAllAsync(u => u.IdComprobanteVenta == idComprobanteVenta, tracked: false);
            if (detalleVentaDb == null)
            {
                m_doc = null;
                return GetXmlDocument(m_doc);
            }

            string xmlClaveAcceso;

            // Codigo de 8 digitos
            string comprobanteid = "00000000";
            int comprobanteidNum = "00000000".Length;
            int comprobante = idComprobanteVenta.ToString().Length;
            string comprobanteNum8 = comprobanteid.Insert(comprobanteidNum - comprobante, idComprobanteVenta.ToString());
            string comprobanteNumero8 = comprobanteNum8.Substring(0, 8);

            Random generator = new();
            string numeroGenerado = generator.Next(0, 100000000).ToString("D8");
            var secuencial = comprobanteDb.NumeroComprobante.Split("-");

            // Para la clave de acceso basta con obtener el valor de "Ambiente" del objeto empresa para gestionar
            // la clave de acceso, asi como, realizar la gestion de los documentos xml necesarios para generar la
            // factura electronica
            xmlClaveAcceso = GenerarClaveAcceso(comprobanteDb.FechaEmision.ToString("ddMMyyyy"), "01", empresaDb.Ruc, ambiente.ToString(), secuencial[0].ToString(), secuencial[1].ToString(), secuencial[2].ToString(), numeroGenerado);

            ComprobanteVentaDto comprobanteConDocSRI = new()
            {
                Id = comprobanteDb.Id,
                IdCliente = comprobanteDb.IdCliente,
                IdEmpresa = comprobanteDb.IdEmpresa,
                TipoComprobante = comprobanteDb.TipoComprobante,
                NumeroComprobante = comprobanteDb.NumeroComprobante,
                FechaEmision = comprobanteDb.FechaEmision,
                Subtotal12 = comprobanteDb.Subtotal12,
                Subtotal0 = comprobanteDb.Subtotal0,
                FormaPago = comprobanteDb.FormaPago,
                Descuento = comprobanteDb.Descuento,
                Subtotal = comprobanteDb.Subtotal,
                TotalIva = comprobanteDb.TotalIva,
                DocSri = xmlClaveAcceso,
            };

            await _comprobanteVentaRepository.UpdateComprobanteVentaAsync(idComprobanteVenta, comprobanteConDocSRI);

            if (xmlClaveAcceso.Length == 49)
            {
                XElement m_comprobante = new("factura", new XAttribute("version", "1.0.0"), new XAttribute("id", "comprobante"));
                XElement m_info_tributaria = new("infoTributaria",
                    new XElement("ambiente", empresaDb.Ambiente),
                    new XElement("tipoEmision", "1"),
                    new XElement("razonSocial", empresaDb.RazonSocial),
                    new XElement("nombreComercial", empresaDb.NombreComercial),
                    new XElement("ruc", empresaDb.Ruc),
                    new XElement("claveAcceso", xmlClaveAcceso),
                    new XElement("codDoc", "01"),
                    new XElement("estab", secuencial[0].ToString()),
                    new XElement("ptoEmi", secuencial[1].ToString()),
                    new XElement("secuencial", secuencial[2].ToString()),
                    new XElement("dirMatriz", empresaDb.DireccionMatriz));

                m_comprobante.Add(m_info_tributaria);

                string identificacionCliente; // Es la identificacion que proporcione el cliente que puede ser ruc o cedula
                string tipoIdentificacionComprador; // Se utiliza para asignarle un codigo segun la identificacion que proporcione el cliente que puede ser cedula o ruc.

                if (clienteDb.Identificacion.Length == 10 && !string.IsNullOrWhiteSpace(clienteDb.Identificacion))
                {
                    tipoIdentificacionComprador = "05";
                    identificacionCliente = clienteDb.Identificacion;
                }
                else if (clienteDb.Identificacion.Length == 13 && !string.IsNullOrWhiteSpace(clienteDb.Identificacion))
                {
                    tipoIdentificacionComprador = "04";
                    identificacionCliente = clienteDb.Identificacion;
                }
                else
                {
                    m_doc = null;
                    return GetXmlDocument(m_doc);
                }

                XElement infoFactura = new("infoFactura",
                    new XElement("fechaEmision", comprobanteDb.FechaEmision.ToString("dd/MM/yyyy")),
                    new XElement("dirEstablecimiento", empresaDb.DireccionMatriz),
                    new XElement("obligadoContabilidad", empresaDb.ObligadoLlevarContabilidad),
                    new XElement("tipoIdentificacionComprador", tipoIdentificacionComprador),
                    new XElement("razonSocialComprador", clienteDb.Nombres),
                    new XElement("identificacionComprador", identificacionCliente),
                    new XElement("totalSinImpuestos", comprobanteDb.Subtotal.ToString("F2")),
                    new XElement("totalDescuento", comprobanteDb.Descuento.ToString("F2")));

                if (comprobanteDb.Subtotal12 > 0.00M && comprobanteDb.TotalIva > 0.00M && comprobanteDb.Subtotal0 == 0.00M)
                {
                    XElement totalConImpuestos = new("totalConImpuestos");
                    XElement totalImpuesto = new("totalImpuesto");
                    totalImpuesto.Add(new XElement("codigo", "2"));
                    totalImpuesto.Add(new XElement("codigoPorcentaje", "2"));
                    totalImpuesto.Add(new XElement("baseImponible", comprobanteDb.Subtotal12.ToString("F2")));
                    totalImpuesto.Add(new XElement("valor", comprobanteDb.TotalIva.ToString("F2")));

                    totalConImpuestos.Add(totalImpuesto);
                    infoFactura.Add(totalConImpuestos);
                }

                infoFactura.Add(new XElement("propina", "0.00"));
                infoFactura.Add(new XElement("importeTotal", comprobanteDb.Subtotal.ToString("F2")));
                infoFactura.Add(new XElement("moneda", "DOLAR"));

                XElement pagos = new("pagos");
                XElement pago = new("pago");
                pago.Add(new XElement("formaPago", "01"));
                pago.Add(new XElement("total", comprobanteDb.Subtotal));
                pagos.Add(pago);
                infoFactura.Add(pagos);

                m_comprobante.Add(infoFactura);

                XElement detalles = new("detalles");
                XElement detalle;

                foreach (var item in detalleVentaDb)
                {
                    var productoDb = await _productoRepository.GetAsync(u => u.Id == item.IdProducto, tracked: false);
                    if (productoDb != null)
                    {
                        detalle = new("detalle",
                            new XElement("codigoPrincipal", productoDb.CodigoPrincipal),
                            new XElement("codigoAuxiliar", productoDb.CodigoAuxiliar),
                            new XElement("descripcion", productoDb.Descripcion),
                            new XElement("cantidad", item.Cantidad.ToString("F2")),
                            new XElement("precioUnitario", item.PrecioUnitario.ToString("F2")),
                            new XElement("descuento", item.Descuento.ToString("F2")),
                            new XElement("precioTotalSinImpuesto", item.Total));
                        XElement impuestos = new("impuestos");
                        XElement impuesto = new("impuesto");
                        impuesto.Add(new XElement("codigo", "2"));
                        if (item.VentaIva == 0.00M)
                        {
                            impuesto.Add(new XElement("codigoPorcentaje", "0"));
                            impuesto.Add(new XElement("tarifa", "0"));
                            impuesto.Add(new XElement("baseImponible", item.Total));
                            impuesto.Add(new XElement("valor", "0.00"));
                        }
                        else
                        {
                            impuesto.Add(new XElement("codigoPorcentaje", "2"));
                            impuesto.Add(new XElement("tarifa", "12"));
                            impuesto.Add(new XElement("baseImponible", item.Total));
                            impuesto.Add(new XElement("valor", item.VentaIva));
                        }

                        impuestos.Add(impuesto);
                        detalle.Add(impuestos);
                        detalles.Add(detalle);
                    }
                }

                m_comprobante.Add(detalles);

                XElement m_infoAdicional = new("infoAdicional");
                XElement m_campo_adicional = new("campoAdicional");
                m_campo_adicional.Add(new XAttribute("nombre", "Email"));

                // if (!string.IsNullOrEmpty(clienteDb.Email))
                // {
                //    m_campo_adicional.Value = clienteDb.Email;
                // }
                // else
                // {
                //    m_campo_adicional.Value = string.Empty;
                // }
                m_campo_adicional.Value = clienteDb.Email;

                m_infoAdicional.Add(m_campo_adicional);
                m_comprobante.Add(m_infoAdicional);

                m_doc.Add(m_comprobante);
                return GetXmlDocument(m_doc);
            }

            m_doc = null;
            return GetXmlDocument(m_doc);
        }

        /// <summary>
        /// Este metodo permite realizar el proceso de recepcion del xml para la factuacion electronica del SRI.
        /// Tambien permite almacenar el nuevo xml que se genere debido la recepcion del documento xml firmado.
        /// </summary>
        /// <param name="path">Es la ruta del xml firmado para continuar con el proceso de facturacion electronica.</param>
        /// <returns>Retorna la respuesta de la solicitud de recepcion del xml firmado que puede "RECIBIDO" o alguna mensaje de error de no hacer sido procesada con exito.</returns>
        public CRespuestaRecepcion RecepcionComprobante(string path)
        {
            var xmlByte = File.ReadAllBytes(path);

            CRespuestaRecepcion respuestaRecepcion = _cSriWebService.RecepcionComprobanteOnLinePrueba(Convert.ToBase64String(xmlByte));

            return respuestaRecepcion;
        }

        /// <summary>
        /// Este metodo permite realizar el proceso de recepcion del documento xml firmado para continuar con el proceso de facturacion electronica.
        /// </summary>
        /// <param name="claveAcceso">Este parametro permite buscar el registro del xml generado para la facturacion electronica.</param>
        /// <param name="rucEmpresa">Este parametro permite buscar la empresa por el ruc o para establecer el path con el que se va a guardar el archivo xml.</param>
        /// <returns>Retorna una respuesta de acuerdo al proceso de recpcion del sri para continuar con el proceso de la facturacion electronica o realizar correcciones en caso de ser necesario.</returns>
        public async Task<ResponseRecepcionSRI> RecepcionSRI(string claveAcceso, string rucEmpresa)
        {
            // Se obtiene la ruta del xml firmado para continuar con el proceso de facturacion electronica
            var rutaXmlDb = await _rutasFacturacionRepository.GetAsync(u => u.ClaveAcceso == claveAcceso);
            if (rutaXmlDb == null)
            {
                return new ResponseRecepcionSRI();
            }

            var recepcion = RecepcionComprobante(rutaXmlDb.RutaFirmados!);
            if (recepcion.Estado.Equals("RECIBIDA"))
            {
                RutasFacturacionDto rutaRecibida = new()
                {
                    Id = rutaXmlDb.Id,
                    IdEmpresa = rutaXmlDb.IdEmpresa,
                    ClaveAcceso = rutaXmlDb.ClaveAcceso,
                    EstadoRecepcion = recepcion.Estado,
                    PathXMLPDF = rutaXmlDb.PathXMLPDF,
                    RutaGenerados = rutaXmlDb.RutaGenerados,
                    RutaFirmados = rutaXmlDb.RutaFirmados,
                    RutaAutorizados = rutaXmlDb.RutaAutorizados,
                };

                await _rutasFacturacionRepository.UpdateRutasFacturacionAsync(rutaXmlDb.Id, rutaRecibida);

                return new ResponseRecepcionSRI { Estado = recepcion.Estado, ClaveAcceso = claveAcceso };
            }

            RutasFacturacionDto rutaNoRecibida = new()
            {
                Id = rutaXmlDb.Id,
                IdEmpresa = rutaXmlDb.IdEmpresa,
                ClaveAcceso = rutaXmlDb.ClaveAcceso,
                EstadoRecepcion = recepcion.Estado,
                PathXMLPDF = rutaXmlDb.PathXMLPDF,
                RutaGenerados = rutaXmlDb.RutaGenerados,
                RutaFirmados = rutaXmlDb.RutaFirmados,
                RutaAutorizados = rutaXmlDb.RutaAutorizados,
            };

            await _rutasFacturacionRepository.UpdateRutasFacturacionAsync(rutaXmlDb.Id, rutaNoRecibida);

            return new ResponseRecepcionSRI
            {
                Estado = $"{recepcion.Estado} / Mensaje: {recepcion.Comprobantes[0].Mensajes[0].mensaje?.ToString()} / " +
                $"Informacion Adicional: {recepcion.Comprobantes[0].Mensajes[0].InformacionAdicional?.ToString()}",
                ClaveAcceso = claveAcceso,
            };
        }

        public string SerializarAXML<T>(T objeto)
        {
            XmlSerializer serializer = new(typeof(T));
            using StringWriter writer = new();
            serializer.Serialize(writer, objeto);
            return writer.ToString();
        }

        public bool XMLAutorizado(string patchCData, string patchOut, string estadoAutorizado, string numeroAutorizado, string fechaAutorizado)
        {
            try
            {
                if (!string.IsNullOrEmpty(estadoAutorizado) && !string.IsNullOrEmpty(numeroAutorizado) && !string.IsNullOrEmpty(fechaAutorizado))
                {
                    XmlDocument doc = new();

                    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    XmlElement root = doc.DocumentElement;
                    doc.InsertBefore(xmlDeclaration, root);

                    XmlElement raizAutorizacion = doc.CreateElement("autorizacion");
                    doc.AppendChild(raizAutorizacion);

                    XmlElement estado = doc.CreateElement("estado");
                    estado.AppendChild(doc.CreateTextNode(estadoAutorizado));
                    raizAutorizacion.AppendChild(estado);

                    XmlElement numeroAutorizacion = doc.CreateElement("numeroAutorizacion");
                    numeroAutorizacion.AppendChild(doc.CreateTextNode(numeroAutorizado));
                    raizAutorizacion.AppendChild(numeroAutorizacion);

                    XmlElement fechaAutorizacion = doc.CreateElement("fechaAutorizacion");
                    fechaAutorizacion.SetAttribute("class", "fechaAutorizacion");
                    fechaAutorizacion.AppendChild(doc.CreateTextNode(fechaAutorizado));
                    raizAutorizacion.AppendChild(fechaAutorizacion);

                    XmlElement comprobante = doc.CreateElement("comprobante");
                    comprobante.AppendChild(doc.CreateCDataSection(patchCData));
                    raizAutorizacion.AppendChild(comprobante);

                    XmlElement mensaje = doc.CreateElement("mensajes");
                    raizAutorizacion.AppendChild(mensaje);
                    doc.Save(patchOut);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                var sss = ex.ToString();
                return false;
            }
        }

        public void XMLNoAutorizado(string patchCData, string patchOut)
        {
            CRespuestaAutorizacion respuestaAutorizacion = new();

            foreach (var resAutorizacion in respuestaAutorizacion.Comprobantes)
            {
                if (resAutorizacion.Estado.Equals("NO AUTORIZADO"))
                {
                    XmlDocument doc = new();

                    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    XmlElement root = doc.DocumentElement;
                    doc.InsertBefore(xmlDeclaration, root);
                    XmlElement raizAutorizacion = doc.CreateElement("autorizacion");
                    doc.AppendChild(raizAutorizacion);

                    XmlElement estado = doc.CreateElement("estado");
                    estado.AppendChild(doc.CreateTextNode(resAutorizacion.Estado));
                    raizAutorizacion.AppendChild(estado);

                    XmlElement fechaAutorizacion = doc.CreateElement("fechaAutorizacion");
                    fechaAutorizacion.SetAttribute("class", "fechaAutorizacion");
                    fechaAutorizacion.AppendChild(doc.CreateTextNode(resAutorizacion.FechaAutorizacion));
                    raizAutorizacion.AppendChild(fechaAutorizacion);

                    string xmlText = File.ReadAllText(patchCData);
                    var cData = new XmlDocument();
                    cData.LoadXml(xmlText);

                    XmlElement comprobante = doc.CreateElement("comprobante");
                    comprobante.AppendChild(doc.CreateCDataSection(xmlText.ToString()));
                    raizAutorizacion.AppendChild(comprobante);

                    XmlNode nodoMensaje = doc.DocumentElement;
                    XmlNode mensajes = doc.CreateElement("mensajes");
                    XmlNode mensaje = doc.CreateElement("mensaje");
                    XmlNode mensaje1 = doc.CreateElement("mensaje");

                    doc.Save(patchOut);
                }
            }
        }
    }
}

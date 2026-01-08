using AutoMapper;
using FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;
using FacturacionElectronicaSRI.Repository.Service.IService;
using FacturacionElectronicaSRI.Repository.Service.SRIWebServices;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Repository.Service
{
    public class ServiceSRI : IServiceSRI
    {
        private readonly IAlmacenadorArchivos _almacenadorArchivos;
        private readonly IMapper _mapper;
        private readonly ICertificadoService _certificadoService;
        private readonly IHostingEnvironment _webHostEnvironment;
        internal CSRIws _cSriWebService;
        public ServiceSRI(
            IAlmacenadorArchivos almacenadorArchivos,
            IMapper mapper,
            ICertificadoService certificadoService,
            IHostingEnvironment webHostEnvironment)
        {
            _almacenadorArchivos = almacenadorArchivos;
            _mapper = mapper;
            _certificadoService = certificadoService;
            _webHostEnvironment = webHostEnvironment;
            this._cSriWebService = new ();
        }

        /// <summary>
        /// Este medoto permite realizar la solicitud para la aprobacion del xml firmado para continuar el proceso de facturacion electronica.
        /// </summary>
        /// <param name="claveAcceso">Es el parametro que se va a utilizar en la solictud de la autorizacion del xml firmado para la facturacion electronica.</param>
        /// <returns>Retorna la respuesta de la solicitud de autorizacion del sri que puede ser "AUTORIZADO" de estar correctamente el proceso, caso contrario recibirimos el mensaje de error.</returns>
        //public CRespuestaAutorizacion AutorizacionComprobante(string claveAcceso)
        //{
        //    CRespuestaAutorizacion respuestaAutorizacion = _cSriWebService.AutorizacionComprobanteOnLinePrueba(claveAcceso);

        //    return respuestaAutorizacion;
        //}
        public async Task<CRespuestaAutorizacion> AutorizacionComprobante(string claveAcceso)
        {
            CRespuestaAutorizacion respuestaAutorizacion = await _cSriWebService.AutorizacionComprobanteOnLinePrueba(claveAcceso);

            return respuestaAutorizacion;
        }

        /// <summary>
        /// Este metodo permite empezar con el proceso de autorizacion del elemento xml firmado para continuar con el procesos de facturacion electronica.
        /// Tambien se realiza la generacion del xml firmado y autorizado para realizar la facturacion electronica del sri.
        /// </summary>
        /// <param name="empresaDto">permite obtener la informacion  de la empresa, necesaria para la autorización del SRI.</param>
        /// <param name="claveAcceso">permite obtener la informacion de la clave de acceso para la autorización del SRI.</param>
        /// <returns>Retorna la respuesta de la autorización del SRI que puede ser: No Autorizada o Autorizada.</returns>
        //public ResponseAutorizacionSRI AutorizacionSRI(EmpresaDto empresaDto, string claveAcceso)
        //{
        //    string carpetaFacturaElectronicaEmpresa = Path.Combine(_webHostEnvironment.ContentRootPath, @"Archivos", @"FacturacionElectronicaEmpresa-" + empresaDto.Ruc);

        //    // Esta funcion permite generar la carpeta de las facturas en el caso de que no exista.
        //    if (!Directory.Exists(carpetaFacturaElectronicaEmpresa))
        //    {
        //        Directory.CreateDirectory(carpetaFacturaElectronicaEmpresa);
        //    }

        //    string carpetaXMLAutorizado = Path.Combine(carpetaFacturaElectronicaEmpresa, @"DocumentosAutorizados"); // ruta para los xml autorizados

        //    // Esta funcion permite generar la carpeta de los xml autorizados en caso de que no exista
        //    if (!Directory.Exists(carpetaXMLAutorizado))
        //    {
        //        Directory.CreateDirectory(carpetaXMLAutorizado);
        //    }

        //    string carpetaXMLNoAutorizado = Path.Combine(carpetaFacturaElectronicaEmpresa, @"DocumentosNoAutorizados"); // ruta para los xml no autorizados

        //    // Esta funcion permite generar la carpeta de los xml no autorizados en caso de que no exista
        //    if (!Directory.Exists(carpetaXMLNoAutorizado))
        //    {
        //        Directory.CreateDirectory(carpetaXMLNoAutorizado);
        //    }

        //    var autorizacion = AutorizacionComprobante(claveAcceso);
        //    if (autorizacion.Comprobantes![0].Estado!.Equals("EN PROCESO"))
        //    {
        //        return new ResponseAutorizacionSRI
        //        {
        //            Estado = autorizacion.Estado!,
        //            ClaveAcceso = claveAcceso,
        //            Code = 201,
        //        };
        //    }

        //    if (autorizacion.Comprobantes![0].Estado!.Equals("AUTORIZADO"))
        //    {
        //        // Si esta autorizado se modifica el archivo XML firmado y la ruta del archivo para continuar con el proceso de facturacion electronica.
        //        var autoriza = XMLAutorizado(
        //            autorizacion.Comprobantes[0].Comprobante,
        //            carpetaXMLAutorizado + $@"\autorizacion{autorizacion.ClaveAcceso}" + ".xml",
        //            autorizacion.Comprobantes[0].Estado,
        //            autorizacion.ClaveAcceso,
        //            autorizacion.Comprobantes[0].FechaAutorizacion);

        //        if (autoriza)
        //        {
        //            // Se genera la ruta de un xml autorizado por el SRI para almacenarlo en la carpeta de autorizados.
        //            string rutaXmlAutorizada = carpetaXMLAutorizado + $@"\autorizacion{autorizacion.ClaveAcceso}" + ".xml";

        //            return new ResponseAutorizacionSRI { Estado = autorizacion.Comprobantes![0].Estado!, PathXMLAutorizado = rutaXmlAutorizada };
        //        }

        //        return new ResponseAutorizacionSRI
        //        {
        //            Estado = $"{autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()} /InformacionAdicional: {autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()}",
        //            ClaveAcceso = claveAcceso,
        //            Code = 500,
        //        };
        //    }

        //    // Se genera la ruta de un xml no autorizado por el SRI para almacenarlo en la carpeta de no autorizados.
        //    string rutaXmlNoAutorizada = carpetaXMLNoAutorizado + $@"\Noautorizada{autorizacion.ClaveAcceso}" + ".xml";

        //    XMLNoAutorizado(
        //            autorizacion.Comprobantes[0].Comprobante,
        //            rutaXmlNoAutorizada,
        //            autorizacion.Comprobantes[0].Estado,
        //            autorizacion.Comprobantes[0].FechaAutorizacion);

        //    return new ResponseAutorizacionSRI
        //    {
        //        Estado = $"{autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()} /InformacionAdicional: {autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()}",
        //        ClaveAcceso = claveAcceso,
        //        Code = 500,
        //        PathXMLAutorizado = rutaXmlNoAutorizada,
        //    };
        //}
        public async Task<ResponseAutorizacionSRI> AutorizacionSRI(EmpresaDto empresaDto, string claveAcceso)
        {
            string carpetaFacturaElectronicaEmpresa = Path.Combine(_webHostEnvironment.ContentRootPath, @"Archivos", @"FacturacionElectronicaEmpresa-" + empresaDto.Ruc);

            // Esta funcion permite generar la carpeta de las facturas en el caso de que no exista.
            if (!Directory.Exists(carpetaFacturaElectronicaEmpresa))
            {
                Directory.CreateDirectory(carpetaFacturaElectronicaEmpresa);
            }

            string carpetaXMLAutorizado = Path.Combine(carpetaFacturaElectronicaEmpresa, @"DocumentosAutorizados"); // ruta para los xml autorizados

            // Esta funcion permite generar la carpeta de los xml autorizados en caso de que no exista
            if (!Directory.Exists(carpetaXMLAutorizado))
            {
                Directory.CreateDirectory(carpetaXMLAutorizado);
            }

            string carpetaXMLNoAutorizado = Path.Combine(carpetaFacturaElectronicaEmpresa, @"DocumentosNoAutorizados"); // ruta para los xml no autorizados

            // Esta funcion permite generar la carpeta de los xml no autorizados en caso de que no exista
            if (!Directory.Exists(carpetaXMLNoAutorizado))
            {
                Directory.CreateDirectory(carpetaXMLNoAutorizado);
            }

            var autorizacion = await AutorizacionComprobante(claveAcceso);
            if (autorizacion.Comprobantes![0].Estado!.Equals("EN PROCESO"))
            {
                return new ResponseAutorizacionSRI
                {
                    Estado = autorizacion.Estado!,
                    ClaveAcceso = claveAcceso,
                    Code = 201,
                };
            }

            if (autorizacion.Comprobantes![0].Estado!.Equals("AUTORIZADO"))
            {
                // Si esta autorizado se modifica el archivo XML firmado y la ruta del archivo para continuar con el proceso de facturacion electronica.
                var autoriza = XMLAutorizado(
                    autorizacion.Comprobantes[0].Comprobante,
                    carpetaXMLAutorizado + $@"\autorizacion{autorizacion.ClaveAcceso}" + ".xml",
                    autorizacion.Comprobantes[0].Estado,
                    autorizacion.ClaveAcceso,
                    autorizacion.Comprobantes[0].FechaAutorizacion);

                if (autoriza)
                {
                    // Se genera la ruta de un xml autorizado por el SRI para almacenarlo en la carpeta de autorizados.
                    string rutaXmlAutorizada = carpetaXMLAutorizado + $@"\autorizacion{autorizacion.ClaveAcceso}" + ".xml";

                    return new ResponseAutorizacionSRI { Estado = autorizacion.Comprobantes![0].Estado!, PathXMLAutorizado = rutaXmlAutorizada };
                }

                return new ResponseAutorizacionSRI
                {
                    Estado = $"{autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()} /InformacionAdicional: {autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()}",
                    ClaveAcceso = claveAcceso,
                    Code = 500,
                };
            }

            // Se genera la ruta de un xml no autorizado por el SRI para almacenarlo en la carpeta de no autorizados.
            string rutaXmlNoAutorizada = carpetaXMLNoAutorizado + $@"\Noautorizada{autorizacion.ClaveAcceso}" + ".xml";

            XMLNoAutorizado(
                    autorizacion.Comprobantes[0].Comprobante,
                    rutaXmlNoAutorizada,
                    autorizacion.Comprobantes[0].Estado,
                    autorizacion.Comprobantes[0].FechaAutorizacion);

            return new ResponseAutorizacionSRI
            {
                Estado = $"{autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()} /InformacionAdicional: {autorizacion.Comprobantes![0].Mensajes![0].InformacionAdicional?.ToString()}",
                ClaveAcceso = claveAcceso,
                Code = 500,
                PathXMLAutorizado = rutaXmlNoAutorizada,
            };
        }

        /// <summary>
        /// Este método permite firmar el xml para realizar el proceso de la Facturacion Electronica del SRI.
        /// </summary>
        /// <param name="empresaDto">permite enviar la información de la empresa para generar el xml firmado.</param>
        /// <param name="claveAcceso">permite enviar la información de la clave de acceso para generar el xml firmado.</param>
        /// <param name="pathXmlGenerated">permite enviar la ruta del xml generado.</param>
        /// <returns>Retorna las rutas de xml procesado y firmado.</returns>
        public ViewXmlDto FirmarXML(EmpresaDto empresaDto, string claveAcceso, string pathXmlGenerated)
        {
            // Se genera la ruta para almacenar los xml firmados
            string carpetaXmlFirmados = Path.Combine(_webHostEnvironment.ContentRootPath, @"Archivos", "FacturacionElectronicaEmpresa-" + empresaDto.Ruc, "DocumentosFirmados"); // ruta para los xml firmados
            if (!Directory.Exists(carpetaXmlFirmados))
            {
                Directory.CreateDirectory(carpetaXmlFirmados);
            }

            try
            {
                if (empresaDto != null)
                {
                    _certificadoService.CargarDesdeP12(empresaDto.PathCertificado, empresaDto.Contraseña); // Se carga el x509Certificate con el certificado p12 para la firma del documento xml

                    var xml = _certificadoService.FirmarDocumentoXml(pathXmlGenerated); // se firma el documento y nos lo devuelve

                    string rutaXmlFirmado = _webHostEnvironment.ContentRootPath + @"\Archivos" + @"\FacturacionElectronicaEmpresa-" + empresaDto.Ruc + @"\DocumentosFirmados\" + claveAcceso + ".xml";
                    xml.Save(rutaXmlFirmado); // se almacena el xml firmado en el path de la rutaXmlFirmado

                    return new ViewXmlDto() { Id = 0, RucEmpresa = empresaDto.Ruc, RutaXmlGenerado = pathXmlGenerated, RutaXmlFirmado = rutaXmlFirmado, Mensaje = "Se firmo correctamente", IsSuccess = true };
                }

                return new ViewXmlDto() { Id = 0, RucEmpresa = string.Empty, RutaXmlGenerado = string.Empty, RutaXmlFirmado = string.Empty, Mensaje = $"No exite la ruta del xml generado", IsSuccess = false };
            }
            catch (Exception ex)
            {
                return new ViewXmlDto() { Id = 0, RucEmpresa = string.Empty, RutaXmlGenerado = string.Empty, RutaXmlFirmado = string.Empty, Mensaje = $"No se firmo correctamente {ex}", IsSuccess = false };
            }
        }

        public string GenerarClaveAcceso(string fecha, string tipoComprobante, string rucEmpresa, string ambiente, string estab, string ptoEmi, string secuencial, string idCod)
        {
            int suma = 0, factor = 7;
            string tipoEmision = "1";
            var claveAcceso = fecha + tipoComprobante + rucEmpresa + ambiente + estab + ptoEmi + secuencial + idCod + tipoEmision;

            foreach (char item in claveAcceso)
            {
                suma += int.Parse(item.ToString()) * factor;

                if (factor == 2)
                {
                    factor = 7;
                }
                else
                {
                    factor--;
                }
            }

            int residuo = suma % 11;
            int digitoVerificador = 11 - residuo;

            if (digitoVerificador == 11)
            {
                return claveAcceso + "0";
            }

            return digitoVerificador == 10 ? claveAcceso + "1" : claveAcceso + digitoVerificador.ToString();
        }

        /// <summary>
        /// La tarea GenerarXML permite generar un archivo xml para realizar la facturacion electronica del SRI.
        /// </summary>
        /// <param name="empresa">Este parametro permite obtener la informacion de la empresa para la facturacion.</param>
        /// <param name="comprobanteVentaDto">Este parametro permite obtener la informacion del comprobante para la facturacion.</param>
        /// <param name="clienteDto">Este parametro permite obtener la informacion del cliente para la facturacion.</param>
        /// <param name="detalleVentas">Este parametro permite obtener la informacion de los detalles de la venta para la facturacion.</param>
        /// <param name="plazos">Este parametro permite obtener la informacion de los plazos de pago de la venta para la facturación.</param>
        /// <returns>Retorna verdadero si se genera el archivo xml caso contrario retorna false.</returns>
        public ResponseXmlDto GenerarXML(EmpresaDto empresa, ComprobanteVentaDto comprobanteVentaDto, ClienteDto clienteDto, List<DetalleVentaDto> detalleVentas, int plazos)
        {
            // Se genera la ruta de la carpeta para almacenar los xml procesados
            string carpetaXMLProcesado = Path.Combine(_webHostEnvironment.ContentRootPath + "\\Archivos", @"FacturaElectronicaProcesados");

            // Esta funcion permite generar la carpeta de los xml generados en caso de que no exista
            if (!Directory.Exists(carpetaXMLProcesado))
            {
                Directory.CreateDirectory(carpetaXMLProcesado);
            }

            // Se obtiene el XML
            var facturaXML = GetXMLFactura(empresa, comprobanteVentaDto, clienteDto, detalleVentas, plazos);
            if (facturaXML == null)
            {
                return new ResponseXmlDto { IsSuccess = false, Message = "No se pudo generar la factura" };
            }

            var factura = facturaXML.InnerXml;
            factura = factura.Replace("&lt;", "<");

            factura = factura.Replace("&gt;", ">");

            var xmlByte = Encoding.UTF8.GetBytes(factura);

            // Permite crear la ruta de almacenamiento del archivo XML proceso o generado para agregarlo en la base de datos
            var ruta = _almacenadorArchivos.GuardarArchivo(xmlByte, ".xml", carpetaXMLProcesado, comprobanteVentaDto.DocSri);

            return new ResponseXmlDto { IsSuccess = true, Message = "Se genero la factura con exito", PathXML = ruta.Result };
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

        public XmlDocument GetXMLFactura(EmpresaDto empresa, ComprobanteVentaDto comprobanteVentaDto, ClienteDto clienteDto, List<DetalleVentaDto> detalleVentas, int plazos)
        {
            XDocument? m_doc = new();

            var secuencial = comprobanteVentaDto.NumeroComprobante.Split("-"); // permite obtener el estab, ptoEmi y el secuencial para generar el xml de la factura

            if (comprobanteVentaDto.DocSri!.Length == 49)
            {
                XElement m_comprobante = new("factura", new XAttribute("id", "comprobante"), new XAttribute("version", "1.1.0"));
                XElement m_info_tributaria = new("infoTributaria",
                    new XElement("ambiente", empresa.Ambiente),
                    new XElement("tipoEmision", "1"),
                    new XElement("razonSocial", empresa.RazonSocial),
                    new XElement("nombreComercial", empresa.NombreComercial),
                    new XElement("ruc", empresa.Ruc),
                    new XElement("claveAcceso", comprobanteVentaDto.DocSri),
                    new XElement("codDoc", "01"),
                    new XElement("estab", secuencial[0].ToString()),
                    new XElement("ptoEmi", secuencial[1].ToString()),
                    new XElement("secuencial", secuencial[2].ToString()),
                    new XElement("dirMatriz", empresa.DireccionMatriz));

                m_comprobante.Add(m_info_tributaria);

                string identificacionCliente; // Es la identificacion que proporcione el cliente que puede ser ruc o cedula
                string tipoIdentificacionComprador; // Se utiliza para asignarle un codigo segun la identificacion que proporcione el cliente que puede ser cedula o ruc.

                if (clienteDto.Identificacion.Length == 10 && !string.IsNullOrWhiteSpace(clienteDto.Identificacion))
                {
                    tipoIdentificacionComprador = "05";
                    identificacionCliente = clienteDto.Identificacion;
                }
                else if (clienteDto.Identificacion.Length == 13 && !string.IsNullOrWhiteSpace(clienteDto.Identificacion))
                {
                    tipoIdentificacionComprador = "04";
                    identificacionCliente = clienteDto.Identificacion;
                }
                else
                {
                    m_doc = null;
                    return GetXmlDocument(m_doc);
                }

                XElement infoFactura = new("infoFactura",
                    new XElement("fechaEmision", comprobanteVentaDto.FechaEmision.ToString("dd/MM/yyyy")),
                    new XElement("dirEstablecimiento", empresa.DireccionMatriz),
                    new XElement("obligadoContabilidad", empresa.ObligadoLlevarContabilidad),
                    new XElement("tipoIdentificacionComprador", tipoIdentificacionComprador),
                    new XElement("razonSocialComprador", clienteDto.Nombres),
                    new XElement("identificacionComprador", identificacionCliente),
                    new XElement("totalSinImpuestos", ((double)comprobanteVentaDto.Subtotal15).ToString("F2", CultureInfo.InvariantCulture)),
                    new XElement("totalDescuento", ((double)comprobanteVentaDto.Descuento).ToString("F2", CultureInfo.InvariantCulture)));

                if (comprobanteVentaDto.Subtotal15 > 0.00M && comprobanteVentaDto.TotalIva > 0.00M && comprobanteVentaDto.Subtotal0 == 0.00M)
                {
                    XElement totalConImpuestos = new("totalConImpuestos");
                    XElement totalImpuesto = new("totalImpuesto");
                    totalImpuesto.Add(new XElement("codigo", "2"));
                    totalImpuesto.Add(new XElement("codigoPorcentaje", "4"));
                    totalImpuesto.Add(new XElement("baseImponible", ((double)comprobanteVentaDto.Subtotal15).ToString("F2", CultureInfo.InvariantCulture)));
                    totalImpuesto.Add(new XElement("valor", ((double)comprobanteVentaDto.TotalIva).ToString("F2", CultureInfo.InvariantCulture)));

                    totalConImpuestos.Add(totalImpuesto);
                    infoFactura.Add(totalConImpuestos);
                }

                infoFactura.Add(new XElement("propina", "0.00"));
                infoFactura.Add(new XElement("importeTotal", ((double)comprobanteVentaDto.Total).ToString("F2", CultureInfo.InvariantCulture)));
                infoFactura.Add(new XElement("moneda", "DOLAR"));

                XElement pagos = new("pagos");
                XElement pago = new("pago");
                pago.Add(new XElement("formaPago", comprobanteVentaDto.FormaPago));
                pago.Add(new XElement("total", ((double)comprobanteVentaDto.Total).ToString("F2", CultureInfo.InvariantCulture)));
                if (plazos > 0)
                {
                    pago.Add(new XElement("plazo", plazos));
                    pago.Add(new XElement("unidadTiempo", "meses"));
                }

                pagos.Add(pago);
                infoFactura.Add(pagos);

                m_comprobante.Add(infoFactura);

                XElement detalles = new("detalles");
                XElement detalle;

                if (detalleVentas.Count > 0)
                {
                    foreach (var item in detalleVentas)
                    {
                        detalle = new("detalle",
                                new XElement("codigoPrincipal", item.Producto!.CodigoPrincipal),
                                new XElement("codigoAuxiliar", item.Producto!.CodigoAuxiliar),
                                new XElement("descripcion", item.Producto!.Descripcion),
                                new XElement("cantidad", ((double)item.Cantidad).ToString("F2", CultureInfo.InvariantCulture)),
                                new XElement("precioUnitario", ((double)item.PrecioUnitario).ToString("F2", CultureInfo.InvariantCulture)),
                                new XElement("descuento", ((double)item.Descuento).ToString("F2", CultureInfo.InvariantCulture)),
                                new XElement("precioTotalSinImpuesto", ((double)item.Total).ToString("F2", CultureInfo.InvariantCulture)));
                        XElement impuestos = new("impuestos");
                        XElement impuesto = new("impuesto");
                        impuesto.Add(new XElement("codigo", "2"));
                        if (item.VentaIva == 0.00M)
                        {
                            impuesto.Add(new XElement("codigoPorcentaje", "4"));
                            impuesto.Add(new XElement("tarifa", "15.00"));
                            impuesto.Add(new XElement("baseImponible", ((double)item.Total).ToString("F2", CultureInfo.InvariantCulture)));
                            impuesto.Add(new XElement("valor", "0.00"));
                        }
                        else
                        {
                            impuesto.Add(new XElement("codigoPorcentaje", "4"));
                            impuesto.Add(new XElement("tarifa", "15.00"));
                            impuesto.Add(new XElement("baseImponible", ((double)item.Total).ToString("F2", CultureInfo.InvariantCulture)));
                            impuesto.Add(new XElement("valor", ((double)item.VentaIva).ToString("F2", CultureInfo.InvariantCulture)));
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

                m_campo_adicional.Value = clienteDto.Email;

                m_infoAdicional.Add(m_campo_adicional);
                m_comprobante.Add(m_infoAdicional);

                m_doc.Add(m_comprobante);
                return GetXmlDocument(m_doc);
            }

            m_doc = null;
            return GetXmlDocument(m_doc);
        }

        /// <summary>
        /// Este método permite realizar el proceso de recepción del xml para la factuación electrónica del SRI.
        /// También permite almacenar el nuevo xml que se genere debido la recepción del documento xml firmado.
        /// </summary>
        /// <param name="path">Es la ruta del xml firmado para continuar con el proceso de facturacion electrónica.</param>
        /// <returns>Retorna la respuesta de la solicitud de recepción del xml firmado que puede "RECIBIDO" o alguna mensaje de error de no hacer sido procesada con exito.</returns>
        //public CRespuestaRecepcion RecepcionComprobante(string path)
        //{
        //    var xmlByte = File.ReadAllBytes(path);

        //    CRespuestaRecepcion respuestaRecepcion = _cSriWebService.RecepcionComprobanteOnLinePrueba(Convert.ToBase64String(xmlByte));

        //    return respuestaRecepcion;
        //}
        public async Task<CRespuestaRecepcion> RecepcionComprobante(string path)
        {
            var xmlByte = File.ReadAllBytes(path);

            CRespuestaRecepcion respuestaRecepcion = await _cSriWebService.RecepcionComprobanteOnLinePrueba(Convert.ToBase64String(xmlByte));

            return respuestaRecepcion;
        }

        /// <summary>
        /// Este método permite realizar el proceso de recepción del documento xml firmado para continuar con el proceso de facturacion electronica.
        /// </summary>
        /// <param name="pathSigned">es la información de la ruta del xml firmado para continuar con la recepción del SRI.</param>
        /// <param name="claveAcceso">contiene la informacion de la clave de acceso necesaria para la recepción del SRI.</param>
        /// <returns>Retorna la respuesta de la operación de Recepcion del SRI que puede ser: DEVUELTA O RECIBIDA.</returns>
        //public ResponseRecepcionSRI RecepcionSRI(string pathSigned, string claveAcceso)
        //{
        //    // Se obtiene la ruta del xml firmado para continuar con el proceso de facturacion electronica
        //    if (string.IsNullOrEmpty(pathSigned))
        //    {
        //        return new ResponseRecepcionSRI();
        //    }

        //    var recepcion = RecepcionComprobante(pathSigned);

        //    if (recepcion.Estado!.Equals("RECIBIDA"))
        //    {
        //        return new ResponseRecepcionSRI { Estado = recepcion.Estado, ClaveAcceso = claveAcceso };
        //    }

        //    return new ResponseRecepcionSRI
        //    {
        //        Estado = $"{recepcion.Estado} / Mensaje: {recepcion.Comprobantes![0].Mensajes![0].mensaje} / " +
        //        $"Informacion Adicional: {recepcion.Comprobantes![0].Mensajes![0].InformacionAdicional}",
        //        ClaveAcceso = claveAcceso,
        //    };
        //}
        public async Task<ResponseRecepcionSRI> RecepcionSRI(string pathSigned, string claveAcceso)
        {
            // Se obtiene la ruta del xml firmado para continuar con el proceso de facturacion electronica
            if (string.IsNullOrEmpty(pathSigned))
            {
                return new ResponseRecepcionSRI();
            }

            var recepcion = await RecepcionComprobante(pathSigned);

            if (recepcion.Estado!.Equals("RECIBIDA"))
            {
                return new ResponseRecepcionSRI { Estado = recepcion.Estado, ClaveAcceso = claveAcceso };
            }

            return new ResponseRecepcionSRI
            {
                Estado = $"{recepcion.Estado} / Mensaje: {recepcion.Comprobantes![0].Mensajes![0].mensaje} / " +
                $"Informacion Adicional: {recepcion.Comprobantes![0].Mensajes![0].InformacionAdicional}",
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

                    XmlReader reader = XmlReader.Create(new StringReader(patchCData));

                    XmlElement comprobante = doc.CreateElement("comprobante");
                    comprobante.AppendChild(doc.ReadNode(reader));
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

        public void XMLNoAutorizado(string patchCData, string patchOut, string estadoNoAutorizado, string fechaNoAutorizado)
        {
            CRespuestaAutorizacion respuestaAutorizacion = new();

            if (estadoNoAutorizado.Equals("NO AUTORIZADO"))
            {
                XmlDocument doc = new();

                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);
                XmlElement raizAutorizacion = doc.CreateElement("autorizacion");
                doc.AppendChild(raizAutorizacion);

                XmlElement estado = doc.CreateElement("estado");
                estado.AppendChild(doc.CreateTextNode(estadoNoAutorizado));
                raizAutorizacion.AppendChild(estado);

                XmlElement fechaAutorizacion = doc.CreateElement("fechaAutorizacion");
                fechaAutorizacion.SetAttribute("class", "fechaAutorizacion");
                fechaAutorizacion.AppendChild(doc.CreateTextNode(fechaNoAutorizado));
                raizAutorizacion.AppendChild(fechaAutorizacion);

                XmlReader reader = XmlReader.Create(new StringReader(patchCData));

                XmlElement comprobante = doc.CreateElement("comprobante");
                comprobante.AppendChild(doc.ReadNode(reader));
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
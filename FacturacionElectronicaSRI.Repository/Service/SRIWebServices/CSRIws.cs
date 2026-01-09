using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Repository.Service.SRIWebServices
{
    public class CSRIws
    {
        /// <summary>
        /// Este metodo es el antiguo que trabaja con HttpWebRequest que es funcional pero esta obsoleto
        /// </summary>
        /// <param name="xml">es la xml que contiene la informacion que necesita para enviar a Recepción del SRI.</param>
        /// <returns>Retorna la respuesta de la recepción del SRI. que puede ser Recibida o Devuelta.</returns>
        //public CRespuestaRecepcion RecepcionComprobanteOnLinePrueba(string xml)
        //{
        //    string respuesta = string.Empty;
        //    CRespuestaRecepcion respuestaRecepcionPrueba = new();
        //    XmlDocument soapEnvelopeXml = new();

        //    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl");

        //    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        //    webRequest.Accept = "text/xml";
        //    webRequest.Method = "POST";

        //    soapEnvelopeXml.LoadXml(new CSoapXML().RecepcionComprobanteSoap(xml));

        //    using (Stream requestStream = webRequest.GetRequestStream())
        //    {
        //        soapEnvelopeXml.Save(requestStream); // permite que se sobreescriba el archivo xml que estaba en la carpeta de xmlfirmados
        //    }

        //    // Esta funcion permite enviar la respuesta de la solicitud realizada al sri en la recepcion del documento para continuar con la facturacion electronica
        //    using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
        //    {
        //        using StreamReader rd = new(response.GetResponseStream());
        //        respuesta = rd.ReadToEnd();

        //        var soapResult = XDocument.Parse(respuesta);

        //        var responseXml = soapResult.Descendants("RespuestaRecepcionComprobante").ToList();
        //        foreach (var xmlDoc in responseXml)
        //        {
        //            respuestaRecepcionPrueba = (CRespuestaRecepcion)DeserializeFromXElement(xmlDoc, typeof(CRespuestaRecepcion));
        //        }
        //    }

        //    return respuestaRecepcionPrueba;
        //}

        /// <summary>
        /// Este método esta actualizado para trabajar con HTTPClient 
        /// </summary>
        /// <param name="xml">es el xml para enviar a la recepción del SRI.</param>
        /// <returns>Retorna la respuesta de la recepción del SRI. que puede ser Recibida o Devuelta.</returns>
        public async Task<CRespuestaRecepcion> RecepcionComprobanteOnLinePrueba(string xml)
        {
            string respuesta = string.Empty;
            CRespuestaRecepcion respuestaRecepcionPrueba = new();
            XmlDocument soapEnvelopeXml = new();

            using var client = new HttpClient();
            client.BaseAddress = new Uri("https://celcer.sri.gob.ec");
            client.DefaultRequestHeaders.Add("Accept", "application/xml"); // Es la cabecera necesaria para que pueda aceptar la petición del xml en la recepción del SRI.

            soapEnvelopeXml.LoadXml(new CSoapXML().RecepcionComprobanteSoap(xml)); // permite cargar el xml el archivo SOAP necesario para realizar la operación de la recepción del SRI.

            var request = new HttpRequestMessage(HttpMethod.Post, "/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl");
            request.Content = new StringContent(soapEnvelopeXml.OuterXml, Encoding.UTF8, "application/xml");

            // Esta funcion permite enviar la respuesta de la solicitud realizada al sri en la recepción del documento para continuar con la facturacion electrónica.
            HttpResponseMessage response = client.Send(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            var soapResult = XDocument.Parse(responseContent);
            var responseXml = soapResult.Descendants("RespuestaRecepcionComprobante").ToList();
            foreach (var xmlDoc in responseXml)
            {
                respuestaRecepcionPrueba = (CRespuestaRecepcion)DeserializeFromXElement(xmlDoc, typeof(CRespuestaRecepcion));
            }

            client.Dispose();

            return respuestaRecepcionPrueba;
        }

        /// <summary>
        /// Este método es el antiguo que permite trabajar con httpWebRequest que es funcional pero esta obsoleto.
        /// </summary>
        /// <param name="claveAcceso">es la informacion de la clave de acceso necesaria para la autorizacion del SRI.</param>
        /// <returns>Retorna la respuesta de la autorización del SRI que puede ser: Autorizada o no Autorizado.</returns>
        //public CRespuestaAutorizacion AutorizacionComprobanteOnLinePrueba(string claveAcceso)
        //{
        //    CRespuestaAutorizacion respuestaAutorizacion = new();
        //    try
        //    {
        //        string respuesta = string.Empty;

        //        XmlDocument soapEnvelopeXml = new();

        //        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl ");

        //        webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        //        webRequest.Accept = "text/xml";
        //        webRequest.Method = "POST";

        //        soapEnvelopeXml.LoadXml(new CSoapXML().AutorizacionComprobanteSoap(claveAcceso));

        //        using (Stream requestStream = webRequest.GetRequestStream())
        //        {
        //            soapEnvelopeXml.Save(requestStream); // permite sobreescribir el xml firmado pero con la solicitud de autorizacion del SRI
        //        }

        //        using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
        //        {
        //            using StreamReader rd = new(response.GetResponseStream());
        //            respuesta = rd.ReadToEnd();
        //            var soapResult = XDocument.Parse(respuesta);
        //            var responseXml = soapResult.Descendants("RespuestaAutorizacionComprobante").ToList();

        //            foreach (var xmlDoc in responseXml)
        //            {
        //                respuestaAutorizacion = (CRespuestaAutorizacion)DeserializeFromXElement(xmlDoc, typeof(CRespuestaAutorizacion));
        //            }
        //        }

        //        return respuestaAutorizacion;
        //    }
        //    catch (Exception)
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// Este método es la actualizada para trabajar con httpclient 
        /// </summary>
        /// <param name="claveAcceso">Es la informacion de la clave de acceso para realizar la operación de autorización del SRI.</param>
        /// <returns>Retorna la respuesta de la autorización del SRI que puede ser: Autorizada o no Autorizado.</returns>
        public async Task<CRespuestaAutorizacion> AutorizacionComprobanteOnLinePrueba(string claveAcceso)
        {
            CRespuestaAutorizacion respuestaAutorizacion = new();
            try
            {
                string respuesta = string.Empty;
                XmlDocument soapEnvelopeXml = new();

                using var client = new HttpClient();

                client.BaseAddress = new Uri("https://celcer.sri.gob.ec");
                client.DefaultRequestHeaders.Add("Accept", "application/xml");

                soapEnvelopeXml.LoadXml(new CSoapXML().AutorizacionComprobanteSoap(claveAcceso));

                var request = new HttpRequestMessage(HttpMethod.Post, "/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl");

                request.Content = new StringContent(soapEnvelopeXml.OuterXml, Encoding.UTF8, "application/xml");

                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                var soapResult = XDocument.Parse(responseContent);
                var responseXml = soapResult.Descendants("RespuestaAutorizacionComprobante").ToList();

                foreach (var xmlDoc in responseXml)
                {
                    respuestaAutorizacion = (CRespuestaAutorizacion)DeserializeFromXElement(xmlDoc, typeof(CRespuestaAutorizacion));
                }

                client.Dispose();

                return respuestaAutorizacion;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object DeserializeFromXElement(XElement element, Type t)
        {
            try
            {
                using XmlReader reader1 = element.CreateReader();
                XmlSerializer serializer = new (t);
                return serializer.Deserialize(reader1);
            }
            catch (Exception ex)
            {
                string g = ex.Message;
                string k = ex.ToString();
                return null;
            }
        }
    }
}
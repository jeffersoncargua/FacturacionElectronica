using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FacturacionElectronicaSRI.Repository.Service.SRIWebServices
{
    public class CSRIws
    {
        public CRespuestaRecepcion RecepcionComprobanteOnLinePrueba(string xml)
        {
            string respuesta = string.Empty;
            CRespuestaRecepcion respuestaRecepcionPrueba = new ();
            XmlDocument soapEnvelopeXml = new ();

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline?wsdl");

            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            soapEnvelopeXml.LoadXml(new CSoapXML().RecepcionComprobanteSoap(xml));

            using (Stream requestStream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(requestStream); // permite que se sobreescriba el archivo xml que estaba en la carpeta de xmlfirmados
            }

            // Esta funcion permite enviar la respuesta de la solucitud realizada al sri en la recepcion del documento para continuar con la facturacion electronica
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            {
                using StreamReader rd = new(response.GetResponseStream());
                respuesta = rd.ReadToEnd();

                var soapResult = XDocument.Parse(respuesta);

                var responseXml = soapResult.Descendants("RespuestaRecepcionComprobante").ToList();
                foreach (var xmlDoc in responseXml)
                {
                    respuestaRecepcionPrueba = (CRespuestaRecepcion)DeserializeFromXElement(xmlDoc, typeof(CRespuestaRecepcion));
                }
            }

            return respuestaRecepcionPrueba;
        }

        public CRespuestaAutorizacion AutorizacionComprobanteOnLinePrueba(string claveAcceso)
        {
            CRespuestaAutorizacion respuestaAutorizacion = new ();
            try
            {
                string respuesta = string.Empty;

                XmlDocument soapEnvelopeXml = new ();

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl ");

                webRequest.ContentType = "text/xml;charset=\"utf-8\"";
                webRequest.Accept = "text/xml";
                webRequest.Method = "POST";

                soapEnvelopeXml.LoadXml(new CSoapXML().AutorizacionComprobanteSoap(claveAcceso));

                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    soapEnvelopeXml.Save(requestStream); // permite sobreescribir el xml firmado pero con la solicitud de autorizacion del SRI
                }

                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                {
                    using StreamReader rd = new (response.GetResponseStream());
                    respuesta = rd.ReadToEnd();
                    var soapResult = XDocument.Parse(respuesta);
                    var responseXml = soapResult.Descendants("RespuestaAutorizacionComprobante").ToList();

                    foreach (var xmlDoc in responseXml)
                    {
                        respuestaAutorizacion = (CRespuestaAutorizacion)DeserializeFromXElement(xmlDoc, typeof(CRespuestaAutorizacion));
                    }
                }

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
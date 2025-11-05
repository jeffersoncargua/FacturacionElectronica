using FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML;
using FacturacionElectronicaSRI.Repository.Service.SRIWebServices;
using System.Xml;
using System.Xml.Linq;

namespace FacturacionElectronicaSRI.Repository.Service.IService
{
    public interface IServiceSRI
    {
        Task<ResponseAutorizacionSRI> AutorizacionSRI(string claveAcceso, string rucEmpresa);

        Task<ViewXmlDto> FirmarXML(string claveAcceso, string rucEmpresa);

        string GenerarClaveAcceso(string fecha, string tipoComprobante, string rucEmpresa, string ambiente, string estab, string ptoEmi, string secuencial, string idCod);

        Task<XmlDocument> GetXMLFactura(int idComprobanteVenta, int ambiente, string rucEmpresa);

        Task<bool> GenerarXML(string rucEmpresa, int idComprobanteVenta);

        XmlDocument GetXmlDocument(XDocument document);

        Task<ResponseRecepcionSRI> RecepcionSRI(string claveAcceso, string rucEmpresa);

        CRespuestaRecepcion RecepcionComprobante(string path);

        bool XMLAutorizado(string patchCData, string patchOut, string estadoAutorizado, string numeroAutorizado, string fechaAutorizado);

        void XMLNoAutorizado(string patchCData, string patchOut);

        string SerializarAXML<T>(T objeto);

        CRespuestaAutorizacion AutorizacionComprobante(string claveAcceso);
    }
}

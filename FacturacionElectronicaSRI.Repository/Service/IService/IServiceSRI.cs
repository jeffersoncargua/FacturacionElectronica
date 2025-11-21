using FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository.Service.SRIWebServices;
using System.Xml;
using System.Xml.Linq;

namespace FacturacionElectronicaSRI.Repository.Service.IService
{
    public interface IServiceSRI
    {
        // Task<ResponseAutorizacionSRI> AutorizacionSRI(string claveAcceso, string rucEmpresa);
        ResponseAutorizacionSRI AutorizacionSRI(RutasFacturacionDto rutasFacturacionDto);

        // Task<ViewXmlDto> FirmarXML(string claveAcceso, string rucEmpresa);
        ViewXmlDto FirmarXML(RutasFacturacionDto rutasFacturacionDto);

        string GenerarClaveAcceso(string fecha, string tipoComprobante, string rucEmpresa, string ambiente, string estab, string ptoEmi, string secuencial, string idCod);

        // Task<XmlDocument> GetXMLFactura(int idComprobanteVenta, int ambiente, string rucEmpresa);
        XmlDocument GetXMLFactura(EmpresaDto empresa, ComprobanteVentaDto comprobanteVentaDto, ClienteDto clienteDto, List<DetalleVentaDto> detalleVentas);

        // Task<ResponseXmlDto> GenerarXML(string rucEmpresa, int idComprobanteVenta);
        ResponseXmlDto GenerarXML(EmpresaDto empresa, ComprobanteVentaDto comprobanteVentaDto, ClienteDto clienteDto, List<DetalleVentaDto> detalleVentas);

        XmlDocument GetXmlDocument(XDocument document);

        // Task<ResponseRecepcionSRI> RecepcionSRI(string claveAcceso, string rucEmpresa);
        ResponseRecepcionSRI RecepcionSRI(RutasFacturacionDto rutasFacturacionDto);

        CRespuestaRecepcion RecepcionComprobante(string path);

        bool XMLAutorizado(string patchCData, string patchOut, string estadoAutorizado, string numeroAutorizado, string fechaAutorizado);

        void XMLNoAutorizado(string patchCData, string patchOut);

        string SerializarAXML<T>(T objeto);

        CRespuestaAutorizacion AutorizacionComprobante(string claveAcceso);
    }
}
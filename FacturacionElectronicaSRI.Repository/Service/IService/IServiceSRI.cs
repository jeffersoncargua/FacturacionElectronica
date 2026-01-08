using FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using FacturacionElectronicaSRI.Repository.Service.SRIWebServices;
using System.Xml;
using System.Xml.Linq;

namespace FacturacionElectronicaSRI.Repository.Service.IService
{
    public interface IServiceSRI
    {
        // ResponseAutorizacionSRI AutorizacionSRI(EmpresaDto empresaDto, string claveAcceso);
        Task<ResponseAutorizacionSRI> AutorizacionSRI(EmpresaDto empresaDto, string claveAcceso);

        ViewXmlDto FirmarXML(EmpresaDto empresaDto, string claveAcceso, string pathXmlGenerated);

        string GenerarClaveAcceso(string fecha, string tipoComprobante, string rucEmpresa, string ambiente, string estab, string ptoEmi, string secuencial, string idCod);

        XmlDocument GetXMLFactura(EmpresaDto empresa, ComprobanteVentaDto comprobanteVentaDto, ClienteDto clienteDto, List<DetalleVentaDto> detalleVentas, int plazos);

        ResponseXmlDto GenerarXML(EmpresaDto empresa, ComprobanteVentaDto comprobanteVentaDto, ClienteDto clienteDto, List<DetalleVentaDto> detalleVentas, int plazos);

        XmlDocument GetXmlDocument(XDocument document);

        // ResponseRecepcionSRI RecepcionSRI(string pathSigned, string claveAcceso);
        Task<ResponseRecepcionSRI> RecepcionSRI(string pathSigned, string claveAcceso);

        // CRespuestaRecepcion RecepcionComprobante(string path);
        Task<CRespuestaRecepcion> RecepcionComprobante(string path);

        bool XMLAutorizado(string patchCData, string patchOut, string estadoAutorizado, string numeroAutorizado, string fechaAutorizado);

        void XMLNoAutorizado(string patchCData, string patchOut, string estadoNoAutorizado, string fechaNoAutorizado);

        string SerializarAXML<T>(T objeto);

        // CRespuestaAutorizacion AutorizacionComprobante(string claveAcceso);
        Task<CRespuestaAutorizacion> AutorizacionComprobante(string claveAcceso);
    }
}
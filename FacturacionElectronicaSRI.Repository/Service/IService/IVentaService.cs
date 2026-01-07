using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;

namespace FacturacionElectronicaSRI.Repository.Service.IService
{
    public interface IVentaService
    {
        // Task<bool> CostumerExist(string identificacion);
        Task<Response> GenerarDetalleVenta(int comprobanteId, List<ShoppingCartDto> productsInCart);

        Task<Response> GenerarVenta(VentaDto ventaDto);

        Task<Response> GenerarComprobanteVenta(ComprobanteVentaDto comprobanteVentaDto);

        Task<Response> GetAllDetalleVenta(int comprobanteId = 0);

        // Task<Response> GenerarRideYPdf(ComprobanteVentaDto comprobanteDto);
        Task<Response> GenerarRideYPdf(ComprobanteVentaDto comprobanteDto, string pathXml, int plazos);

        Task<Response> GetAllComprobanteVenta(string? query = null, string? startDate = null, string? endDate = null);

        Task<Response> GetComprobanteVenta(int id, string? query = null);

        Task<Response> GetAllDocumentoXML(string? query = null);

        Task<Response> GetDocumentoXML(int id, string? query = null);

        // Task<Response> GenerarTokenKushki(RequestTokenDto requestTokenDto);

        // Task<Response> GenerarPagoKushki(string token, string identificacion, VentaDto ventaDto);

        // Task<bool> GenerarXML(string rucEmpresa, string idComprobante);

        // Task<ViewXmlDto> FirmarXML(string claveAcceso, string rucEmpresa);

        // Task<ResponseAutorizacionSRI> AutorizacionSRI(string claveAcceso, string rucEmpresa);

        // Task<ResponseRecepcionSRI> RecepcionSRI(string claveAcceso, string rucEmpresa);
    }
}
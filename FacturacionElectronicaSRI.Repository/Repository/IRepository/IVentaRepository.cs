using FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IVentaRepository
    {
        Task<bool> CostumerExist(string identificacion);

        Task<Response> GenerarDetalleVenta(string shoopingCart);

        Task<Response> GenerarVenta(VentaDto ventaDto);

        Task<bool> GenerarXML(string rucEmpresa, string idComprobante);

        Task<ViewXmlDto> FirmarXML(string claveAcceso, string rucEmpresa);

        Task<ResponseAutorizacionSRI> AutorizacionSRI(string claveAcceso, string rucEmpresa);

        Task<ResponseRecepcionSRI> RecepcionSRI(string claveAcceso, string rucEmpresa);
    }
}
using FacturacionElectronicaSRI.Data.FormatosXML.FacturacionSRIXML;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;
using FacturacionElectronicaSRI.Repository.Repository.IRepository;

namespace FacturacionElectronicaSRI.Repository.Repository
{
    public class VentaRepository : IVentaRepository
    {
        public VentaRepository()
        {
        }

        public Task<ResponseAutorizacionSRI> AutorizacionSRI(string claveAcceso, string rucEmpresa)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CostumerExist(string identificacion)
        {
            throw new NotImplementedException();
        }

        public Task<ViewXmlDto> FirmarXML(string claveAcceso, string rucEmpresa)
        {
            throw new NotImplementedException();
        }

        public Task<Response> GenerarDetalleVenta(string shoopingCart)
        {
            throw new NotImplementedException();
        }

        public Task<Response> GenerarVenta(VentaDto ventaDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GenerarXML(string rucEmpresa, string idComprobante)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseRecepcionSRI> RecepcionSRI(string claveAcceso, string rucEmpresa)
        {
            throw new NotImplementedException();
        }
    }
}
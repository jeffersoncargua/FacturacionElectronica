using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.DetalleVenta.DTO;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IDetalleVentaRepository : IRepository<TblDetalleVenta>
    {
        Task<Response> UpdateDetalleVentaAsync(int id, DetalleVentaDto detalleVentaDto);

        Task<Response> GetAllDetalleVentaAsync(int idComprobante);

        Task<Response> GetDetalleVentaAsync(int id);

        Task<Response> RemoveDetalleVentaAsync(int id);

        Task<Response> CreateDetalleVentaAsync(DetalleVentaDto detalleVentaDto);
    }
}
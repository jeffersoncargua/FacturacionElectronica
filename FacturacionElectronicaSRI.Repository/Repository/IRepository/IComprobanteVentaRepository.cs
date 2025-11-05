using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.ComprobanteVenta.DTO;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IComprobanteVentaRepository : IRepository<TblComprobanteVenta>
    {
        Task<Response> UpdateComprobanteVentaAsync(int id, ComprobanteVentaDto comprobanteVentaDto);

        Task<Response> GetAllComprobanteVentaAsync(string? query = null, string? startDate = null, string? endDate = null);

        Task<Response> GetComprobanteVentaAsync(int id, string? query = null);

        Task<Response> RemoveComprobanteVentaAsync(int id);

        Task<Response> CreateComprobanteVentaAsync(ComprobanteVentaDto comprobanteVentaDto);
    }
}
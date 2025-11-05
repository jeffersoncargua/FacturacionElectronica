using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Venta.DTO;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IRutasFacturacionRepository : IRepository<TblRutasXML>
    {
        Task<Response> UpdateRutasFacturacionAsync(int id, RutasFacturacionDto rutasFacturacionDto);

        Task<Response> GetAllRutasFacturacionAsync(string? query = null);

        Task<Response> GetRutasFacturacionAsync(int id, string? query = null);

        Task<Response> RemoveRutasFacturacionAsync(int id);

        Task<Response> CreateRutasFacturacionAsync(RutasFacturacionDto rutasFacturacionDto);
    }
}
using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IEmpresaRepository : IRepository<TblEmpresa>
    {
        Task<Response> UpdateEmpresaAsync(int id, EmpresaDto empresaDto);

        Task<Response> GetAllEmpresaAsync(string? query = null, string? includeProperties = null);

        Task<Response> GetEmpresaAsync(int id, string? query = null, string? includeProperties = null);

        Task<Response> RemoveEmpresaAsync(int id);

        Task<Response> CreateEmpresaAsync(EmpresaDto empresaDto);
    }
}
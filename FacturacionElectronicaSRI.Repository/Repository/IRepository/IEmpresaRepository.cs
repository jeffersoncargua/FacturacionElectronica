using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Empresa.DTO;
using Microsoft.AspNetCore.Http;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IEmpresaRepository : IRepository<TblEmpresa>
    {
        Task<Response> UpdateEmpresaAsync(int id, EmpresaDto empresaDto);

        // Task<Response> UpdatePathFileAsync(string ruc, string tipoArchivo, IFormFile? file = null);

        Task<Response> GetAllEmpresaAsync(string? query = null);

        Task<Response> GetEmpresaAsync(int id, string? query = null);

        Task<Response> RemoveEmpresaAsync(int id);

        Task<Response> CreateEmpresaAsync(EmpresaDto empresaDto);
    }
}
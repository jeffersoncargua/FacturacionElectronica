using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Cliente.DTO;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IClienteRepository : IRepository<TblCliente>
    {
        Task<Response> UpdateClienteAsync(int id, ClienteDto clienteDto);

        Task<Response> GetAllClienteAsync(string? query = null);

        Task<Response> GetClienteAsync(int id, string? query = null);

        Task<Response> RemoveClienteAsync(int id);

        Task<Response> CreateClienteAsync(ClienteDto clienteDto);
    }
}
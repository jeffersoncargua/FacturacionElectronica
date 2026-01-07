using FacturacionElectronicaSRI.Data.Entity;
using FacturacionElectronicaSRI.Data.Model.Producto.DTO;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IProductoRepository : IRepository<TblProductos>
    {
        Task<Response> UpdateProductoAsync(int id, ProductoDto productoDto);

        Task<Response> GetAllProductoAsync(string? query = null, int pageSize = 0, int pageNumber = 0);

        Task<Response> GetCantProductoAsync();

        Task<Response> GetProductoAsync(int id, string? query = null);

        Task<Response> RemoveProductoAsync(int id);

        Task<Response> CreateProductoAsync(ProductoDto productoDto);
    }
}
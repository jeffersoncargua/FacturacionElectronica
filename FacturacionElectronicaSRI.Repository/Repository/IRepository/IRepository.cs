using System.Linq.Expressions;

namespace FacturacionElectronicaSRI.Repository.Repository.IRepository
{
    public interface IRepository<T>
        where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 0);

        Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null);

        Task CreateAsyn(T entity);

        Task DeleteAsync(T entity);

        Task SaveAsync();
    }
}
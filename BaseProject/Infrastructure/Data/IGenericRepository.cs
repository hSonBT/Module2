using BaseProject.Core.Abstractions;

namespace BaseProject.Infrastructure.Data;

public interface IGenericRepository<T> where T: BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default); 
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<T> GetQueryable();

}
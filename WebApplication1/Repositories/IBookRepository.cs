using WebApplication1.Models;
namespace WebApplication1.Repositories;
public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default);
    Task<int> DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

using WebApplication1.Contracts;
using WebApplication1.Models;
namespace WebApplication1.Services;
public interface IBookService
{
    Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Book> CreateAsync(BookRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Book>> CreateManyAsync(IEnumerable<BookRequest> requests, CancellationToken cancellationToken = default);
    Task<Book?> UpdateAsync(Guid id, BookRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

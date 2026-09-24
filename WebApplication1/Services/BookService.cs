using WebApplication1.Contracts;
using WebApplication1.Models;
using WebApplication1.Repositories;
namespace WebApplication1.Services;
public sealed class BookService(IBookRepository repository) : IBookService
{
    public Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default) 
        => repository.GetAllAsync(cancellationToken);
    public Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) 
        => repository.GetByIdAsync(id, cancellationToken);
    public async Task<Book> CreateAsync(BookRequest request, CancellationToken cancellationToken = default) { var book = Map(Guid.NewGuid(), request); await repository.AddRangeAsync([book], cancellationToken); return book; }
    public async Task<IReadOnlyList<Book>> CreateManyAsync(IEnumerable<BookRequest> requests, CancellationToken cancellationToken = default) { var books = requests.Select(request => Map(Guid.NewGuid(), request)).ToList(); if (books.Count == 0) throw new ArgumentException("At least one book is required.", nameof(requests)); await repository.AddRangeAsync(books, cancellationToken); return books; }
    public async Task<Book?> UpdateAsync(Guid id, BookRequest request, CancellationToken cancellationToken = default) { var book = Map(id, request); return await repository.UpdateAsync(book, cancellationToken) ? book : null; }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) => await repository.DeleteRangeAsync([id], cancellationToken) > 0;
    public Task<int> DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default) => repository.DeleteRangeAsync(ids.Distinct(), cancellationToken);
    private static Book Map(Guid id, BookRequest request) => new() { Id = id, Title = request.Title.Trim(), Isbn = request.Isbn.Trim(), PublishedYear = request.PublishedYear, Stock = request.Stock, Price = request.Price, Author = new Author { Name = request.Author.Trim() }, Categories = request.Categories.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => new Category { Name = value.Trim() }).DistinctBy(value => value.Name.ToLowerInvariant()).ToList() };
}

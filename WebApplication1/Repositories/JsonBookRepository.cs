using System.Text.Json;
using WebApplication1.Models;
namespace WebApplication1.Repositories;
public sealed class JsonBookRepository : IBookRepository
{
    private static readonly SemaphoreSlim FileLock = new(1, 1);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    private readonly string _filePath;
    public JsonBookRepository(IWebHostEnvironment environment) => _filePath = Path.Combine(environment.ContentRootPath, "Data", "books.json");
    public async Task<IReadOnlyList<Book>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try { return (await ReadUnsafeAsync(cancellationToken)).Select(Clone).ToList(); }
        finally { FileLock.Release(); }
    }
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => (await GetAllAsync(cancellationToken)).FirstOrDefault(book => book.Id == id);
    public async Task AddRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try { var current = await ReadUnsafeAsync(cancellationToken); current.AddRange(books.Select(Clone)); await WriteUnsafeAsync(current, cancellationToken); }
        finally { FileLock.Release(); }
    }
    public async Task<bool> UpdateAsync(Book book, CancellationToken cancellationToken = default)
    {
        await FileLock.WaitAsync(cancellationToken);
        try { var current = await ReadUnsafeAsync(cancellationToken); var index = current.FindIndex(item => item.Id == book.Id); if (index < 0) return false; current[index] = Clone(book); await WriteUnsafeAsync(current, cancellationToken); return true; }
        finally { FileLock.Release(); }
    }
    public async Task<int> DeleteRangeAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idSet = ids.ToHashSet();
        await FileLock.WaitAsync(cancellationToken);
        try { var current = await ReadUnsafeAsync(cancellationToken); var removed = current.RemoveAll(book => idSet.Contains(book.Id)); if (removed > 0) await WriteUnsafeAsync(current, cancellationToken); return removed; }
        finally { FileLock.Release(); }
    }
    private async Task<List<Book>> ReadUnsafeAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath)) return [];
        await using var stream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<List<Book>>(stream, JsonOptions, cancellationToken) ?? [];
    }
    private async Task WriteUnsafeAsync(List<Book> books, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!); var temporaryPath = $"{_filePath}.tmp";
        await using (var stream = File.Create(temporaryPath)) await JsonSerializer.SerializeAsync(stream, books, JsonOptions, cancellationToken);
        File.Move(temporaryPath, _filePath, true);
    }
    private static Book Clone(Book book) => new() { Id = book.Id, Title = book.Title, Isbn = book.Isbn, PublishedYear = book.PublishedYear, Stock = book.Stock, Price = book.Price, Author = new Author { Name = book.Author.Name }, Categories = book.Categories.Select(category => new Category { Name = category.Name }).ToList() };
}

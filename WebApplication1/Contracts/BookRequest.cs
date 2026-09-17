using System.ComponentModel.DataAnnotations;
namespace WebApplication1.Contracts;
public sealed record BookRequest(
    [param: Required, StringLength(160)] string Title,
    [param: Required, StringLength(30)] string Isbn,
    [param: Range(1000, 3000)] int PublishedYear,
    [param: Range(0, 100000)] int Stock,
    [param: Range(typeof(decimal), "0", "1000000")] decimal Price,
    [param: Required, StringLength(120)] string Author,
    [param: Required, MinLength(1)] IReadOnlyCollection<string> Categories);
public sealed record DeleteBooksRequest([param: Required, MinLength(1)] IReadOnlyCollection<Guid> Ids);

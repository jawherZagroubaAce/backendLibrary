namespace WebApplication1.Models;

public sealed class Book
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Isbn { get; set; }
    public int PublishedYear { get; set; }
    public int Stock { get; set; }
    public decimal Price { get; set; }
    public required Author Author { get; set; }
    public List<Category> Categories { get; set; } = [];
}
public sealed class Author { public required string Name { get; set; } }
public sealed class Category { public required string Name { get; set; } }

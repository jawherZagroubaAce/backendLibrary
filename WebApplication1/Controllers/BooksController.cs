using Microsoft.AspNetCore.Mvc;
using WebApplication1.Contracts;
using WebApplication1.Models;
using WebApplication1.Services;
namespace WebApplication1.Controllers;
[ApiController]
[Route("api/[controller]")]
public sealed class BooksController(IBookService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<IReadOnlyList<Book>>> GetAll(CancellationToken cancellationToken) => Ok(await service.GetAllAsync(cancellationToken));
    [HttpGet("{id:guid}")] public async Task<ActionResult<Book>> GetById(Guid id, CancellationToken cancellationToken) { var book = await service.GetByIdAsync(id, cancellationToken); return book is null ? NotFound() : Ok(book); }
    [HttpPost] public async Task<ActionResult<Book>> Create(BookRequest request, CancellationToken cancellationToken) { var book = await service.CreateAsync(request, cancellationToken); return CreatedAtAction(nameof(GetById), new { id = book.Id }, book); }
    [HttpPost("bulk")] public async Task<ActionResult<IReadOnlyList<Book>>> CreateMany([FromBody] List<BookRequest> requests, CancellationToken cancellationToken) { if (requests.Count == 0) return BadRequest(new { message = "At least one book is required." }); return Ok(await service.CreateManyAsync(requests, cancellationToken)); }
    [HttpPut("{id:guid}")] public async Task<ActionResult<Book>> Update(Guid id, BookRequest request, CancellationToken cancellationToken) { var book = await service.UpdateAsync(id, request, cancellationToken); return book is null ? NotFound() : Ok(book); }
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken) => await service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    [HttpPost("bulk-delete")] public async Task<ActionResult<object>> DeleteMany(DeleteBooksRequest request, CancellationToken cancellationToken) => Ok(new { deleted = await service.DeleteManyAsync(request.Ids, cancellationToken) });
}

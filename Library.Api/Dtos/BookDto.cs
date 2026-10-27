
namespace Library.Api.Dtos;

public record BookDto
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public string? Description { get; init; }
    public required string Genre { get; init; }
    public required int PublicationYear { get; init; }
    public required int PageCount { get; init; }
}

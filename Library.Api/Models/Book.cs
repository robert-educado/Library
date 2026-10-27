using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    public required string Genre { get; set; }
    public required int PublicationYear { get; set; }
    public required int PageCount { get; set; }
}


using Library.Api.Data;
using Library.Api.Dtos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// GET /api/books[?
//  genre=Fantasy&
//  sortBy=author&
//  sortOrder=asc&
//  page=2&
//  pageSize=10]
// Vi ber om att få en instans av ApplicationDbContext från DI-containern
app.MapGet("/api/books", async (
    ApplicationDbContext db, 
    string? genre,
    string? sortBy,
    string? sortOrder,
    int page = 1,
    int pageSize = 10) =>
{
    if (page < 1 || pageSize < 1 || pageSize > 50)
    {
        return Results.BadRequest(
          "page must be 1 or greater and pageSize must be between 1 and  100");
    }

    // Dynamiskt fråga som vi kan bygga vidare på
    var query = db.Books.AsQueryable();

    if (!string.IsNullOrEmpty(genre))
    {
        query = query.Where(x => x.Genre == genre);
    }

    // sortBy - Title, Author, PublicationYear
    // sortOrder - asc, desc

    query = (sortBy, sortOrder) switch
    {
        // /api/books?sortBy=title&sortOrder=desc
        // Sortera på titel, i fallande ordning
        ("title", "desc") =>
            query.OrderByDescending(b => b.Title),

        // /api/books?sortBy=author&sortOrder=desc
        // Sortera på titel, i fallande ordning
        ("author", "desc") =>
            query.OrderByDescending(b => b.Author),

        ("author", _) =>
        query.OrderBy(b => b.Author),

        // /api/books?sortBy=publicationyear&sortOrder=desc
        // Sortera på publiseringsår, i fallande ordning
        ("publicationyear", "desc") =>
            query.OrderByDescending(b => b.PublicationYear),

        // /api/books?sortBy=publicationyear&sortOrder=vad-som-helst-förutom-desc
        // Sortera på publiseringsår, i stigande ordning
        ("publicationyear", _) =>
            query.OrderBy(b => b.PublicationYear),

        // /api/books?sortBy=vad-som-helst-förutom-title-author-publicationYear&sortOrder=desc
        // Om man inte angivit title, author eller publicationYear, ignorera värdet
        // Om man skrivit sortOrder=desc, då matchar denna - den sorterar då som standard på Title
        (_, "desc") =>
            query.OrderByDescending(b => b.Title),

        // Annars den här!
        _ => query.OrderBy(b => b.Title)
    };



    var skip = (page - 1) * pageSize;

    query = query
        .Skip(skip)
        .Take(pageSize);

    var totalCount = await db.Books.CountAsync();

    var bookDtos = await query.Select(book => new BookDto
    {
        Id = book.Id,
        Title = book.Title,
        Description = book.Description,
        Genre = book.Genre,
        Author = book.Author,
        PublicationYear = book.PublicationYear,
        PageCount = book.PageCount
    }).ToListAsync();

    //  {
    //      "items": [
    //        // Böcker här...
    //      ],
    //"page": 1,
    //"pageSize": 10,
    //"totalCount": 30,
    //"totalPages": 3
    //  }

    return Results.Ok(new
    {
        Items = bookDtos,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
    });
});

app.Run();


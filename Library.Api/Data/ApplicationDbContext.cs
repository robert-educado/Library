using Library.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
      DbContextOptions<ApplicationDbContext> options)
      : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(b => b.Title)
                .HasMaxLength(200);

            entity.Property(b => b.Author)
                .HasMaxLength(200);

            entity.Property(b => b.Genre)
                .HasMaxLength(50);
        });
    }

    public DbSet<Book> Books { get; set; }
}

using LibrarySystemWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystemWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Book> Books => Set<Book>();
        public DbSet<User> Users => Set<User>();
        public DbSet<BookLoan> BookLoans => Set<BookLoan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookLoan>()
                .HasOne(l => l.Book)
                .WithMany(b => b.BookLoans)
                .HasForeignKey(l => l.BookId);

            modelBuilder.Entity<BookLoan>()
                .HasOne(l => l.User)
                .WithMany(u => u.BookLoans)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "Yellow",
                    Author = "Colour Man",
                    ISBN = "9780743273565",
                    CopiesAvailable = 5,
                    ChartColor = "#FF5733",
                    ChartBorderColor = "#C74422"

                },
                new Book
                {
                    Id = 2,
                    Title = "Orange",
                    Author = "Colour Man",
                    ISBN = "9780743273501",
                    CopiesAvailable = 3,
                    ChartColor = "#33FFAA",
                    ChartBorderColor = "#22C788"
                },
                new Book
                {
                    Id = 3,
                    Title = "Green",
                    Author = "Colour Man",
                    ISBN = "9780743273200",
                    CopiesAvailable = 8,
                    ChartColor = "#3385FF",
                    ChartBorderColor = "#2260C7"
                },
                new Book
                {
                    Id = 4,
                    Title = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    ISBN = "9780743273309",
                    CopiesAvailable = 7,
                    ChartColor = "#FF8C42",
                    ChartBorderColor = "#CC6F33"

                },
                new Book
                {
                    Id = 5,
                    Title = "Clean Code",
                    Author = "Some Guy",
                    ISBN = "9780743273624",
                    CopiesAvailable = 5,
                    ChartColor = "#6A5ACD",
                    ChartBorderColor = "#5246A1"
                }
                );
        }
    }
}

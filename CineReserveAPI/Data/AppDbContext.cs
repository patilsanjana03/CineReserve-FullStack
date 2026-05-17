using Microsoft.EntityFrameworkCore;
using CineReserveAPI.Models;

namespace CineReserveAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<SeatBooking> SeatBookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ═══ UNIQUE CONSTRAINT — prevents double booking (concurrency hard-stop)
        modelBuilder.Entity<SeatBooking>()
            .HasIndex(sb => new { sb.ShowtimeId, sb.RowLetter, sb.SeatNumber })
            .IsUnique()
            .HasDatabaseName("UX_SeatBooking_Showtime_Row_Seat");

        // ═══ NON-CLUSTERED INDEXES — fast seat map loading when showtime changes
        modelBuilder.Entity<SeatBooking>()
            .HasIndex(sb => sb.ShowtimeId)
            .HasDatabaseName("IX_SeatBookings_ShowtimeId");

        modelBuilder.Entity<Showtime>()
            .HasIndex(st => st.MovieId)
            .HasDatabaseName("IX_Showtimes_MovieId");

        // ═══ DECIMAL PRECISION
        modelBuilder.Entity<User>()
            .Property(u => u.CreditBalance)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Showtime>()
            .Property(st => st.BasePrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<SeatBooking>()
            .Property(sb => sb.SeatPrice)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Movie>()
            .Property(m => m.Rating)
            .HasColumnType("decimal(3,1)");

        // ═══ TIMESPAN → TIME in SQL
        modelBuilder.Entity<Showtime>()
            .Property(st => st.ShowTime)
            .HasColumnType("time");

        // ═══ SEED MOVIES
        modelBuilder.Entity<Movie>().HasData(
            new Movie
            {
                Id = 1,
                Title = "Inception",
                Genre = "Sci-Fi / Thriller",
                Duration = 148,
                Rating = 8.8m,
                Description = "A thief who steals corporate secrets through dream-sharing technology.",
                PosterUrl = "https://picsum.photos/seed/inception/300/450"
            },
            new Movie
            {
                Id = 2,
                Title = "Interstellar",
                Genre = "Sci-Fi / Adventure",
                Duration = 169,
                Rating = 8.6m,
                Description = "A team of explorers travel through a wormhole in space to ensure humanity's survival.",
                PosterUrl = "https://picsum.photos/seed/interstellar/300/450"
            },
            new Movie
            {
                Id = 3,
                Title = "The Dark Knight",
                Genre = "Action / Drama",
                Duration = 152,
                Rating = 9.0m,
                Description = "Batman faces the Joker, a criminal mastermind who creates chaos in Gotham City.",
                PosterUrl = "https://picsum.photos/seed/darkknight/300/450"
            },
            new Movie
            {
                Id = 4,
                Title = "Avengers: Endgame",
                Genre = "Action / Adventure",
                Duration = 181,
                Rating = 8.4m,
                Description = "The Avengers assemble once more to reverse Thanos' actions.",
                PosterUrl = "https://picsum.photos/seed/avengers/300/450"
            }
        );

        // ═══ SEED SHOWTIMES
        var today = DateTime.Today;
        modelBuilder.Entity<Showtime>().HasData(
            // Inception
            new Showtime { Id = 1, MovieId = 1, ShowDate = today, ShowTime = new TimeSpan(10, 0, 0), HallName = "Hall A", BasePrice = 250m },
            new Showtime { Id = 2, MovieId = 1, ShowDate = today, ShowTime = new TimeSpan(14, 30, 0), HallName = "Hall B", BasePrice = 300m },
            new Showtime { Id = 3, MovieId = 1, ShowDate = today.AddDays(1), ShowTime = new TimeSpan(18, 0, 0), HallName = "Hall A", BasePrice = 350m },
            // Interstellar
            new Showtime { Id = 4, MovieId = 2, ShowDate = today, ShowTime = new TimeSpan(11, 0, 0), HallName = "Hall C", BasePrice = 280m },
            new Showtime { Id = 5, MovieId = 2, ShowDate = today, ShowTime = new TimeSpan(16, 0, 0), HallName = "Hall A", BasePrice = 320m },
            // The Dark Knight
            new Showtime { Id = 6, MovieId = 3, ShowDate = today, ShowTime = new TimeSpan(12, 30, 0), HallName = "Hall B", BasePrice = 260m },
            new Showtime { Id = 7, MovieId = 3, ShowDate = today.AddDays(1), ShowTime = new TimeSpan(20, 0, 0), HallName = "Hall C", BasePrice = 380m },
            // Avengers
            new Showtime { Id = 8, MovieId = 4, ShowDate = today, ShowTime = new TimeSpan(15, 0, 0), HallName = "Hall A", BasePrice = 300m },
            new Showtime { Id = 9, MovieId = 4, ShowDate = today.AddDays(1), ShowTime = new TimeSpan(19, 30, 0), HallName = "Hall B", BasePrice = 350m }
        );
    }
}
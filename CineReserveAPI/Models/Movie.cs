namespace CineReserveAPI.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Duration { get; set; } // minutes
    public string PosterUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Rating { get; set; }

    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
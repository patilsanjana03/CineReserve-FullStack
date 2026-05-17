using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineReserveAPI.Data;
using CineReserveAPI.Models;

namespace CineReserveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowtimesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ShowtimesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Showtimes/movie/5
        [HttpGet("movie/{movieId}")]
        public async Task<ActionResult<IEnumerable<Showtime>>> GetShowtimes(int movieId)
        {
            return await _db.Showtimes
                .Where(st => st.MovieId == movieId)
                .OrderBy(st => st.ShowDate)
                .ThenBy(st => st.ShowTime)
                .ToListAsync();
        }

        // GET: api/Showtimes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Showtime>> GetShowtime(int id)
        {
            var showtime = await _db.Showtimes.FindAsync(id);
            if (showtime == null)
            {
                return NotFound(new { message = "Showtime profile not found." });
            }
            return showtime;
        }

        // GET: api/Showtimes/5/seats
        [HttpGet("{id}/seats")]
        public async Task<ActionResult<IEnumerable<string>>> GetBookedSeats(int id)
        {
            var seats = await _db.SeatBookings
                .Where(sb => sb.ShowtimeId == id)
                .Select(sb => $"{sb.RowLetter}-{sb.SeatNumber}")
                .ToListAsync();

            return Ok(seats);
        }

        // POST: api/Showtimes
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Showtime>> AddShowtime([FromBody] Showtime showtime)
        {
            if (showtime == null)
            {
                return BadRequest(new { message = "Showtime payload data cannot be null." });
            }

            try
            {
                _db.Showtimes.Add(showtime);
                await _db.SaveChangesAsync();

                return CreatedAtAction(nameof(GetShowtimes), new { movieId = showtime.MovieId }, showtime);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while scheduling showtimes.", error = ex.Message });
            }
        }
    }
}
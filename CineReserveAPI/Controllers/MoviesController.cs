using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CineReserveAPI.Data;
using CineReserveAPI.Models;

namespace CineReserveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public MoviesController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _db.Movies.OrderByDescending(m => m.Id).ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound(new { message = "Movie profile not found." });
            }
            return movie;
        }

        // POST: api/Movies
        [HttpPost]
        [Authorize] // Restricts creation access to validated admin login sessions
        public async Task<ActionResult<Movie>> AddMovie([FromBody] Movie movie)
        {
            if (movie == null)
            {
                return BadRequest(new { message = "Movie data payload cannot be null." });
            }

            try
            {
                _db.Movies.Add(movie);
                await _db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the movie entry.", error = ex.Message });
            }
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        [Authorize] // Restricts deletion execution to validated admin login sessions
        public async Task<IActionResult> DeleteMovie(int id)
        {
            // 1. Locate the targeted record inside the SQL database
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound(new { message = $"Movie with ID {id} not found." });
            }

            try
            {
                // 2. Remove the tracked entity instance 
                _db.Movies.Remove(movie);

                // 3. Persist transaction changes safely to the relational tables
                await _db.SaveChangesAsync();

                return Ok(new { message = $"Success! Movie '{movie.Title}' was permanently deleted from the database repository." });
            }
            catch (Exception ex)
            {
                // Gracefully catches foreign key violations if the movie is linked to active showtimes
                return StatusCode(500, new
                {
                    message = "Cannot delete this movie because it is actively linked to scheduled showtimes or customer bookings. Clear its showtimes first!",
                    error = ex.Message
                });
            }
        }
    }
}
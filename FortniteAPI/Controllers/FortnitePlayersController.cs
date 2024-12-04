using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FortniteAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;


namespace FortniteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FortnitePlayersController : ControllerBase
    {
        private readonly ILogger<FortnitePlayersController> _logger;
        private readonly FortniteContext _context;

        public FortnitePlayersController(FortniteContext context, ILogger<FortnitePlayersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/FortnitePlayers
        //public means it is accessible publicly
        //async makes it run asynchronously
        //Task is an async action that returns a value
        //<ActionResult > is a type in .NET that wraps arround different HTTP responses
        //IEnumerable iterates over the FortnitePlayers?
        //_context references the DbContext which is the db connection
        //FortnitePlayers is a DbSet property in the DbContext class that represents the FortnitePlayers Table
        //ToListAsync is an async method that retrieves all records from FortnitePlayers into a list
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FortnitePlayer>>> GetFortnitePlayers()
        {
            return await _context.FortnitePlayers.ToListAsync();
        }


        // GET: api/FortnitePlayers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FortnitePlayer>> GetFortnitePlayer(long id)
        {
            var fortnitePlayer = await _context.FortnitePlayers.FindAsync(id);

            if (fortnitePlayer == null)
            {
                return NotFound();
            }

            return fortnitePlayer;
        }

        // GET: api/FortnitePlayers/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FortnitePlayer>>> SearchFortnitePlayers([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var results = await _context.FortnitePlayers
                .Where(fp => fp.Name.Contains(term) || fp.Server.Contains(term)) // Adjust fields as necessary
                .ToListAsync();

            if (!results.Any())
            {
                return NotFound("No players match the search term.");
            }

            return results;
        }


        // PUT: api/FortnitePlayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
       [Authorize]
        public async Task<IActionResult> PutFortnitePlayer(long id, FortnitePlayer fortnitePlayer)
        {
            if (id != fortnitePlayer.ID)
            {
                return BadRequest();
            }

            _context.Entry(fortnitePlayer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FortnitePlayerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(fortnitePlayer);
        }

        // POST: api/FortnitePlayers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<FortnitePlayer>> PostFortnitePlayer(FortnitePlayer fortnitePlayer)
        {
            _logger.LogInformation("POST request to create a new Fortnite player received. Player data: {@Player}", fortnitePlayer);
            _context.FortnitePlayers.Add(fortnitePlayer);
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetFortnitePlayer", new { id = fortnitePlayer.ID }, fortnitePlayer);
        }

        // DELETE: api/FortnitePlayers/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFortnitePlayer(long id)
        {
            var fortnitePlayer = await _context.FortnitePlayers.FindAsync(id);
            if (fortnitePlayer == null)
            {
                return NotFound();
            }

            _context.FortnitePlayers.Remove(fortnitePlayer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FortnitePlayerExists(long id)
        {
            return _context.FortnitePlayers.Any(e => e.ID == id);
        }
    }
}

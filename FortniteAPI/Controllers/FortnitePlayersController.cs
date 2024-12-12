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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FortnitePlayer>>> GetFortnitePlayers()
        {
            var players = await _context.FortnitePlayers
                .Include(p => p.Team) // Include the related Team data
                .ToListAsync();

            Console.WriteLine($"Players Retrieved: {players.Count()}");

            // Project to a simpler object without circular references and unwanted properties
            var playerResults = players.Select(player => new
            {
                player.ID,
                player.Name,
                player.Earnings,
                player.Server,
                player.Age,
                player.TeamID,
                TeamName = player.Team?.TeamName // Only return Team name to avoid nested data
            }).ToList();

            return Ok(playerResults);
        }






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

        
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<FortnitePlayer>>> SearchFortnitePlayers([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term cannot be empty.");
            }

            var results = await _context.FortnitePlayers
                .Where(fp => fp.Name.Contains(term) || fp.Server.Contains(term)) 
                .ToListAsync();

            if (!results.Any())
            {
                return NotFound("No players match the search term.");
            }

            return results;
        }


      
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


        [HttpPost]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FortnitePlayer>>> PostFortnitePlayers(IEnumerable<FortnitePlayer> fortnitePlayers)
        {
            _logger.LogInformation("POST request to create new Fortnite players received. Players data: {@Players}", fortnitePlayers);

           
            _context.FortnitePlayers.AddRange(fortnitePlayers);
            await _context.SaveChangesAsync();

            
            return CreatedAtAction("GetFortnitePlayers", fortnitePlayers);
        }


        
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

using System;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FortniteAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using SendGrid; 
using SendGrid.Helpers.Mail;


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
        [Authorize(Roles = "Admin")]
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

        [HttpPost("actions/request-admin-access")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestAdminAccess([FromBody] AdminRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Message))
            {
                return BadRequest("Email and message fields are required.");
            }

            try
            {
                var sendgridApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
                if (string.IsNullOrEmpty(sendgridApiKey))
                {
                    throw new Exception("SendGrid API key not set.");
                }

                var client = new SendGridClient(sendgridApiKey);
                var from = new EmailAddress("noreply@milxzy.dev", "Fortnite API");
                var subject = "Admin Access Request";
                var to = new EmailAddress("milxzy@milxzy.dev");
                var plainTextContent = $"User Email: {request.Email}\nMessage: {request.Message}";
                var htmlContent = $"<strong>User Email:</strong> {request.Email}<br/><strong>Message:</strong> {request.Message}";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

                var response = await client.SendEmailAsync(msg);

                if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return Ok("Admin access request sent successfully.");
                }

                return StatusCode((int)response.StatusCode, "Failed to send admin request.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending admin request: {0}", ex.Message);
                return StatusCode(500, "An error occurred while sending the email.");
            }
        }

        public class AdminRequestDto
        {
            public string Email { get; set; }
            public string Message { get; set; }
        }



// DTO (Data Transfer Object) to hold admin request input



[HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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

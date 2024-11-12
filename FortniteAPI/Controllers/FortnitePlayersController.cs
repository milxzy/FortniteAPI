﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FortniteAPI.Models;

namespace FortniteAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FortnitePlayersController : ControllerBase
    {
        private readonly FortniteContext _context;

        public FortnitePlayersController(FortniteContext context)
        {
            _context = context;
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

        // PUT: api/FortnitePlayers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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

            return NoContent();
        }

        // POST: api/FortnitePlayers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FortnitePlayer>> PostFortnitePlayer(FortnitePlayer fortnitePlayer)
        {
            _context.FortnitePlayers.Add(fortnitePlayer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFortnitePlayer", new { id = fortnitePlayer.ID }, fortnitePlayer);
        }

        // DELETE: api/FortnitePlayers/5
        [HttpDelete("{id}")]
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
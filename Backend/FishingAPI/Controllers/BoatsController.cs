using FishingAPI.Data;
using FishingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoatsController : ControllerBase
{
    private readonly HarbourDbContext _context;

    public BoatsController(HarbourDbContext context)
    {
        _context = context;
    }

    // GET: api/boats
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Boat>>> GetBoats()
    {
        return await _context.Boats.ToListAsync();
    }

    // POST: api/boats
    [HttpPost]
    public async Task<ActionResult<Boat>> AddBoat(Boat boat)
    {
        _context.Boats.Add(boat);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBoats), new { id = boat.Id }, boat);
    }
}
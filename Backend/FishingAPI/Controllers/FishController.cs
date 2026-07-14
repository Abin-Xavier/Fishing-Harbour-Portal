using FishingAPI.Data;
using FishingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FishController : ControllerBase
{
    private readonly HarbourDbContext _context;

    public FishController(HarbourDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Fish>>> GetFish()
    {
        return await _context.Fish.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Fish>> GetFish(int id)
    {
        var fish = await _context.Fish.FindAsync(id);

        if (fish == null)
            return NotFound();

        return fish;
    }

    [HttpPost]
    public async Task<ActionResult<Fish>> PostFish(Fish fish)
    {
        _context.Fish.Add(fish);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFish), new { id = fish.Id }, fish);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutFish(int id, Fish fish)
    {
        if (id != fish.Id)
            return BadRequest();

        _context.Entry(fish).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFish(int id)
    {
        var fish = await _context.Fish.FindAsync(id);

        if (fish == null)
            return NotFound();

        _context.Fish.Remove(fish);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
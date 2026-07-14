using FishingAPI.Data;
using FishingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HarbourStatusController : ControllerBase
{
    private readonly HarbourDbContext _context;

    public HarbourStatusController(HarbourDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HarbourStatus>>> GetStatus()
    {
        return await _context.HarbourStatuses.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<HarbourStatus>> PostStatus(HarbourStatus status)
    {
        _context.HarbourStatuses.Add(status);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStatus), new { id = status.Id }, status);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutStatus(int id, HarbourStatus status)
    {
        if (id != status.Id)
            return BadRequest();

        _context.Entry(status).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStatus(int id)
    {
        var status = await _context.HarbourStatuses.FindAsync(id);

        if (status == null)
            return NotFound();

        _context.HarbourStatuses.Remove(status);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
using FishingAPI.Data;
using FishingAPI.Dtos;
using FishingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishingAPI.Controllers;

[ApiController]
[Route("api/fish")]
public class FishController : ControllerBase
{
    private readonly HarbourDbContext _db;

    public FishController(HarbourDbContext db)
    {
        _db = db;
    }

    // GET /api/fish
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FishDetailResponse>>> GetAll()
    {
        var fish = await _db.FishDetails
            .OrderBy(f => f.FishName)
            .Select(f => ToResponse(f))
            .ToListAsync();

        return Ok(fish);
    }

    // GET /api/fish/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FishDetailResponse>> GetById(int id)
    {
        var fish = await _db.FishDetails.FindAsync(id);
        if (fish is null) return NotFound(new { message = $"No fish lot found with id {id}." });
        return Ok(ToResponse(fish));
    }

    // POST /api/fish  — add a new lot to the market board
    [HttpPost]
    public async Task<ActionResult<FishDetailResponse>> Create([FromBody] FishDetailRequest request)
    {
        var fish = new FishDetail
        {
            FishName = request.FishName.Trim(),
            QuantityKg = request.QuantityKg,
            PricePerKg = request.PricePerKg,
            UpdatedAt = DateTime.UtcNow
        };

        _db.FishDetails.Add(fish);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = fish.Id }, ToResponse(fish));
    }

    // PUT /api/fish/5 — update quantity/price as the day's auction moves
    [HttpPut("{id:int}")]
    public async Task<ActionResult<FishDetailResponse>> Update(int id, [FromBody] FishDetailRequest request)
    {
        var fish = await _db.FishDetails.FindAsync(id);
        if (fish is null) return NotFound(new { message = $"No fish lot found with id {id}." });

        fish.FishName = request.FishName.Trim();
        fish.QuantityKg = request.QuantityKg;
        fish.PricePerKg = request.PricePerKg;
        fish.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(ToResponse(fish));
    }

    // DELETE /api/fish/5 — pull a lot once it's sold out
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fish = await _db.FishDetails.FindAsync(id);
        if (fish is null) return NotFound(new { message = $"No fish lot found with id {id}." });

        _db.FishDetails.Remove(fish);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static FishDetailResponse ToResponse(FishDetail f) => new()
    {
        Id = f.Id,
        FishName = f.FishName,
        QuantityKg = f.QuantityKg,
        PricePerKg = f.PricePerKg,
        UpdatedAt = f.UpdatedAt
    };
}

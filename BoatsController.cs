using FishingAPI.Data;
using FishingAPI.Dtos;
using FishingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishingAPI.Controllers;

[ApiController]
[Route("api/boats")]
public class BoatsController : ControllerBase
{
    private readonly HarbourDbContext _db;

    public BoatsController(HarbourDbContext db)
    {
        _db = db;
    }

    // GET /api/boats
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoatResponse>>> GetAll()
    {
        var boats = await _db.Boats
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => ToResponse(b))
            .ToListAsync();

        return Ok(boats);
    }

    // GET /api/boats/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BoatResponse>> GetById(int id)
    {
        var boat = await _db.Boats.FindAsync(id);
        if (boat is null) return NotFound(new { message = $"No boat found with id {id}." });
        return Ok(ToResponse(boat));
    }

    // POST /api/boats
    [HttpPost]
    public async Task<ActionResult<BoatResponse>> Register([FromBody] BoatRegistrationRequest request)
    {
        var alreadyExists = await _db.Boats.AnyAsync(b => b.BoatNumber == request.BoatNumber);
        if (alreadyExists)
        {
            return Conflict(new { message = $"Boat number '{request.BoatNumber}' is already registered." });
        }

        var boat = new Boat
        {
            BoatName = request.BoatName.Trim(),
            Owner = request.Owner.Trim(),
            BoatNumber = request.BoatNumber.Trim(),
            FishingType = request.FishingType.Trim(),
            Contact = request.Contact.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Boats.Add(boat);
        await _db.SaveChangesAsync();

        var response = ToResponse(boat);
        return CreatedAtAction(nameof(GetById), new { id = boat.Id }, response);
    }

    private static BoatResponse ToResponse(Boat b) => new()
    {
        Id = b.Id,
        BoatName = b.BoatName,
        Owner = b.Owner,
        BoatNumber = b.BoatNumber,
        FishingType = b.FishingType,
        Contact = b.Contact,
        CreatedAt = b.CreatedAt
    };
}

using FishingAPI.Data;
using FishingAPI.Dtos;
using FishingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FishingAPI.Controllers;

[ApiController]
[Route("api/harbour-status")]
public class HarbourStatusController : ControllerBase
{
    private readonly HarbourDbContext _db;

    public HarbourStatusController(HarbourDbContext db)
    {
        _db = db;
    }

    // GET /api/harbour-status
    // Active boats and registered fishermen are live counts;
    // today's catch and cold storage come from the single harbour_status row.
    [HttpGet]
    public async Task<ActionResult<HarbourStatusResponse>> Get()
    {
        var activeBoats = await _db.Boats.CountAsync();
        var registeredFishermen = await _db.Users.CountAsync(u => u.Role == "fisherman");

        var status = await _db.HarbourStatusRows.OrderByDescending(h => h.UpdatedAt).FirstOrDefaultAsync();
        status ??= new HarbourStatusEntity { TodaysCatchTons = 0, ColdStorageAvailable = true, UpdatedAt = DateTime.UtcNow };

        return Ok(new HarbourStatusResponse
        {
            ActiveBoats = activeBoats,
            RegisteredFishermen = registeredFishermen,
            TodaysCatchTons = status.TodaysCatchTons,
            ColdStorageAvailable = status.ColdStorageAvailable,
            UpdatedAt = status.UpdatedAt
        });
    }

    // PUT /api/harbour-status — harbour office updates today's catch / cold storage flag
    [HttpPut]
    public async Task<ActionResult<HarbourStatusResponse>> Update([FromBody] HarbourStatusUpdateRequest request)
    {
        var status = await _db.HarbourStatusRows.OrderByDescending(h => h.UpdatedAt).FirstOrDefaultAsync();
        if (status is null)
        {
            status = new HarbourStatusEntity();
            _db.HarbourStatusRows.Add(status);
        }

        status.TodaysCatchTons = request.TodaysCatchTons;
        status.ColdStorageAvailable = request.ColdStorageAvailable;
        status.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        var activeBoats = await _db.Boats.CountAsync();
        var registeredFishermen = await _db.Users.CountAsync(u => u.Role == "fisherman");

        return Ok(new HarbourStatusResponse
        {
            ActiveBoats = activeBoats,
            RegisteredFishermen = registeredFishermen,
            TodaysCatchTons = status.TodaysCatchTons,
            ColdStorageAvailable = status.ColdStorageAvailable,
            UpdatedAt = status.UpdatedAt
        });
    }
}

namespace FishingAPI.Models;

public class Boat
{
    public int Id { get; set; }

    public string BoatName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string RegistrationNumber { get; set; } = string.Empty;

    public int Capacity { get; set; }
}
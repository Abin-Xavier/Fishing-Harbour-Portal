namespace FishingAPI.Models;

public class Fish
{
    public int Id { get; set; }

    public string FishName { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }
}
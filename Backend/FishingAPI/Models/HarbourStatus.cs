namespace FishingAPI.Models;

public class HarbourStatus
{
    public int Id { get; set; }

    public string Weather { get; set; } = string.Empty;

    public string SeaCondition { get; set; } = string.Empty;

    public bool HarbourOpen { get; set; }
}
namespace VUP.Core.Entities;

public class UnknownVerb
{
    public int Id { get; set; }
    public required string RawAction { get; set; } 
    public required int DetectedType { get; set; }
    public int Frequency { get; set; } = 1; 
    public string Status { get; set; } = "Pending"; 
    public DateTime FirstSeenAt { get; set; } = DateTime.UtcNow;
    public DateTime LastSeenAt { get; set; } = DateTime.UtcNow;
}
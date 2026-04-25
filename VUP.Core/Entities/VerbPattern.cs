namespace VUP.Core.Entities;

public class VerbPattern
{
    public int Id { get; set; }
    public int VerbId { get; set; }

    public string? Particle { get; set; } 
    public string? Preposition { get; set; } 

    public required int TypeId { get; set; } 

    public Verb Verb { get; set; } = null!;
}
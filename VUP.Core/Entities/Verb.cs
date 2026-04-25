namespace VUP.Core.Entities;

public class Verb
{
    public int Id { get; set; }
    public required string Lemma { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<VerbPattern> Patterns { get; set; } = new List<VerbPattern>();
}
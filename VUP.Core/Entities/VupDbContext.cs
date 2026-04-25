using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace VUP.Core.Entities;

public class VupDbContext : DbContext
{
    public VupDbContext(DbContextOptions<VupDbContext> options) : base(options) { }

    public DbSet<Verb> Verbs { get; set; }
    public DbSet<VerbPattern> VerbPatterns { get; set; }
    public DbSet<UnknownVerb> UnknownVerbs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Verb>()
            .HasIndex(v => v.Lemma)
            .IsUnique();

        modelBuilder.Entity<UnknownVerb>()
            .HasIndex(u => u.RawAction);
    }
}
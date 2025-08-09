using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Code_Problem_Fetcher.Data;

public class ProblemDbContext : DbContext
{
    public ProblemDbContext(DbContextOptions<ProblemDbContext> options) : base(options)
    {
    }
    
    // Only Problems table - TestCases are stored as JSON within Problems
    public DbSet<Problem> Problems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Problem entity
        modelBuilder.Entity<Problem>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).HasMaxLength(50);
            entity.Property(p => p.Title).IsRequired().HasMaxLength(200);
            entity.Property(p => p.Description).IsRequired().HasMaxLength(2000);
            entity.Property(p => p.Difficulty).HasMaxLength(50);
            entity.Property(p => p.Category).HasMaxLength(100);
            entity.Property(p => p.Link).HasMaxLength(500);
            
            // Convert string array to comma-separated string
            entity.Property(p => p.Tags)
                .HasConversion(
                    tags => string.Join(',', tags),
                    tagsString => string.IsNullOrEmpty(tagsString) 
                        ? Array.Empty<string>() 
                        : tagsString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                );
                
            // Store constraints as JSON
            entity.Property(p => p.Constraints)
                .HasConversion(
                    constraints => JsonSerializer.Serialize(constraints, (JsonSerializerOptions?)null),
                    constraintsJson => JsonSerializer.Deserialize<Dictionary<string, object>>(constraintsJson, (JsonSerializerOptions?)null) 
                        ?? new Dictionary<string, object>()
                );

           entity.Property(p => p.Input)
                .HasConversion(
                    input => JsonSerializer.Serialize(input, (JsonSerializerOptions?)null),
                    inputJson => JsonSerializer.Deserialize<Dictionary<string, object>>(inputJson, (JsonSerializerOptions?)null) 
                        ?? new Dictionary<string, object>()
                );

            entity.Property(p => p.ExpectedOutput)
                .HasConversion(
                    output => JsonSerializer.Serialize(output, (JsonSerializerOptions?)null),
                    outputJson => JsonSerializer.Deserialize<object>(outputJson, (JsonSerializerOptions?)null) 
                        ?? new object()
                );
        });

        // No longer need TestCase entity configuration since it's stored as JSON
    }
}
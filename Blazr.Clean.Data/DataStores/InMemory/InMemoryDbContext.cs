/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public class InMemoryDbContext : DbContext
{
    public DbSet<WeatherForecast>? WeatherForecast { get; set; }

    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : base(options) { }
}

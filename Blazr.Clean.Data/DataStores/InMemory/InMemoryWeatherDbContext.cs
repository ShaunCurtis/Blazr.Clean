/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public class InMemoryWeatherDbContext : DbContext, IWeatherDbContext
{
    public DbSet<WeatherForecast>? WeatherForecast { get; set; }

    public InMemoryWeatherDbContext(DbContextOptions<InMemoryWeatherDbContext> options) : base(options) { }
}

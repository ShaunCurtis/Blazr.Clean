/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public interface IWeatherDbContext
{
    public DbSet<WeatherForecast>? WeatherForecast { get; }
}

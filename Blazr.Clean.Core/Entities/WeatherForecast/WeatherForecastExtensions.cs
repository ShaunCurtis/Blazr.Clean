/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public static class WeatherForecastExtensions
{
    public static string TemperatureF(this WeatherForecast value) 
        => (32 + (value.TemperatureC / 0.5556)).ToString();
}

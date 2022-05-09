/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.WASM.Dual;

/// <summary>
/// This startic class holds the extension methods for configuring 
/// the services for the various SPA/Web Server instances we want to run
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Builds the Blazor WASM SPA services
    /// </summary>
    /// <param name="services"></param>
    public static void AddBlazorWASMAppServices(this IServiceCollection services)
    {
        services.AddScoped<IDataBroker, APIDataBroker>();
        services.AddScoped<IViewService<WeatherForecast>, WeatherForecastViewService>();
    }
}

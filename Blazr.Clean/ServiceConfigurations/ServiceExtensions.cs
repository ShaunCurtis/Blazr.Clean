
using Microsoft.AspNetCore.Mvc.ApplicationParts;
/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean;

/// <summary>
/// This startic class holds the extension methods for configuring 
/// the services for the various SPA/Web Server instances we want to run
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Builds the test In-Memory Blazor Server configuration
    /// </summary>
    /// <param name="services"></param>
    public static void AddBlazorInMemoryServerAppServices(this IServiceCollection services)
        => AddBlazorServerAppServices<InMemoryDbContext>(services, options => options.UseInMemoryDatabase("TestDb"));

    /// <summary>
    /// Builds any Blazor Server configuration that uses a EF database.
    /// You pass in the configuration and define the DbContext to use through generics
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    public static void AddBlazorServerAppServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
        where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(optionsAction);
        services.AddDbContextFactory<InMemoryDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        services.AddSingleton<IDataBroker, ServerDataBroker>();
        services.AddScoped<IViewService<WeatherForecast>, WeatherForecastViewService>();
    }

    /// <summary>
    /// Builds the NetCore Web server  configuration
    /// </summary>
    /// <typeparam name="TDbContext"></typeparam>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    public static void AddServerAppServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
        where TDbContext : DbContext
    {
        services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(Blazr.Clean.Controllers.WeatherForecastController).Assembly));
        services.AddDbContextFactory<TDbContext>(optionsAction);
        services.AddDbContextFactory<InMemoryDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        services.AddSingleton<IDataBroker, ServerDataBroker>();
    }

    /// <summary>
    /// Builds the Blazor WASM SPA services
    /// </summary>
    /// <param name="services"></param>
    public static void AddBlazorWASMAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataBroker, APIDataBroker>();
        services.AddScoped<IViewService<WeatherForecast>, WeatherForecastViewService>();
    }
}

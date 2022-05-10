/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Clean.Core;
using Blazr.Clean.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Clean.Server.Configurations;

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
        services.AddDbContextFactory<TDbContext>(optionsAction);
        services.AddSingleton<IDataBroker, ServerDataBroker>();
    }
}

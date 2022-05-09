# Applications

So far we've built a set of libraries, there's no actual application.  These are implemented in separate projects.

Before we jump into the projects, we set up a `ServicesExtensions` static class in *Blazr.Clean* to hold the server application configurations.  This keeps all the server configurations in one place.

```csharp
public static class ServiceExtensions
{
    public static void AddBlazorInMemoryServerAppServices(this IServiceCollection services)
        => AddBlazorServerAppServices<InMemoryDbContext>(services, options => options.UseInMemoryDatabase("TestDb"));

    public static void AddBlazorServerAppServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
        where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(optionsAction);
        services.AddSingleton<IDataBroker, ServerDataBroker>();
        services.AddScoped<IViewService<WeatherForecast>, WeatherForecastViewService>();
    }

    public static void AddServerAppServices<TDbContext>(this IServiceCollection services, Action<DbContextOptionsBuilder>? optionsAction)
        where TDbContext : DbContext
    {
        services.AddDbContextFactory<TDbContext>(optionsAction);
        services.AddSingleton<IDataBroker, ServerDataBroker>();
    }
}
```

We can't keep the WASM configurations here due to WASM compilation incompatibilities.  These live in a separate *Blazr.Clean.WASM.Configurations* project.

```csharp
public static class ServiceExtensions
{
    public static void AddBlazorWASMAppServices(this IServiceCollection services)
    {
        services.AddScoped<IDataBroker, APIDataBroker>();
        services.AddScoped<IViewService<WeatherForecast>, WeatherForecastViewService>();
    }
}
```

## Blazor Server Application (Blazr.Clean.Server.Web)

The project is a Blazor Server template project with all thw Blazor component stuff removed.  Set this project as the startup project to run the Blazor Server application.

![Project](blazr-clean-server-web-project.png)

All we need do is set the correct `App` component we want to start as the root component and make sure we have the necessary services loaded.

#### _Hosts.cshtml

The `component` tag needs to point to the correct startup `App`. 

```html
<component type="typeof(Blazr.Clean.UI.App)" render-mode="ServerPrerendered" />
```

The full `Program.cs`
```csharp
using Blazr.Clean;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazorInMemoryServerAppServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

## Blazor WASM Application (Blazr.Clean.WASM)

This is a Blazor WASM template project with all the Blazor code removed.  This project builds the WASM executables and support files needed to the run the WASM application. 

![Project](blazr-clean-wasm-project.png)

`Program.cs`, pointing the root component to the correct `App`, and loading the correcvt services through the Service Extensions.

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Blazr.Clean.UI.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazorWASMAppServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
```

## Blazor WASM Web Server

The is the web server for launching the WASM SPA.  It's a DotNetCore Web Server project with almost everything removed.

![Projecr](blazr-clean-wasm-web-project.png)

`Program` is configured as follows:

```csharp
using Blazr.Clean;
using Blazr.Clean.Data;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerAppServices<InMemoryDbContext>(options => options.UseInMemoryDatabase("TestDb"));
// Add the controllers from the controllers project
builder.Services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(Blazr.Clean.Controllers.WeatherForecastController).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// Add the framework files middleware
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
// Map the controllers
app.MapControllers();
// Default endpoint pointing to the WASM startup page
app.MapFallbackToFile("index.html");

app.Run();
```
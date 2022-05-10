using Blazr.Clean.Server.Configurations;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddBlazorInMemoryServerAppServices();
builder.Services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(Blazr.Clean.Controllers.WeatherForecastController).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/wasm"), app1 =>
{
    app1.UseBlazorFrameworkFiles("/wasm");
    app1.UseRouting();
    app1.UseEndpoints(endpoints =>
    {
        endpoints.MapFallbackToPage("/wasm/{*path:nonfile}", "/_WASM");
    });

});


app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapControllers();
app.MapFallbackToPage("/_Host");

app.Run();

using Blazr.Clean.Core;
using Microsoft.EntityFrameworkCore;

namespace Blazr.Clean.Data;

public class InMemoryDbContext : DbContext
{
    public DbSet<WeatherForecast>? WeatherForecast { get; set; }

    public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : base(options) { }
}

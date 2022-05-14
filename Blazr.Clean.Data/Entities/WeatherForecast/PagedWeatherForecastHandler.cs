namespace Blazr.Clean.Data;

public class PagedWeatherForecastHandler
    : IRecordQueryHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private IDbContextFactory<InMemoryWeatherDbContext> _dbContextFactory { get; set; }
    private PagedWeatherForecastsQuery _query;

    public PagedWeatherForecastHandler(IDbContextFactory<InMemoryWeatherDbContext> factory, PagedWeatherForecastsQuery query )
    { 
        _dbContextFactory = factory;
        _query = query;
    }

    public async ValueTask<IEnumerable<WeatherForecast>> ExecuteAsync()
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.WeatherForecast is not null
            ? await context.WeatherForecast
                .Skip(_query.Request.StartIndex)
                .Take(_query.Request.Count)
                .ToListAsync() 
            : new List<WeatherForecast>();
   }
}

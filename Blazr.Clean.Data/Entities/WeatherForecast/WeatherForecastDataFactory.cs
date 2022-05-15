/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public class WeatherForecastDataFactory : IWeatherForecastDataFactory
{
    private readonly IDbContextFactory<InMemoryWeatherDbContext> _dbContext;

    public WeatherForecastDataFactory(IDbContextFactory<InMemoryWeatherDbContext> dbContext)
       => _dbContext = dbContext;

    public IRecordActionHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>> GetPagedRecordsQueryHandler(PagedWeatherForecastsQuery query)
        => new PagedWeatherForecastHandler(_dbContext, query);

    public IRecordActionHandler<UpdateWeatherForecastCommand, CommandResponse> UpdateRecordCommandHandler(UpdateWeatherForecastCommand command)
        => new UpdateWeatherForecastHandler(_dbContext, command);
}

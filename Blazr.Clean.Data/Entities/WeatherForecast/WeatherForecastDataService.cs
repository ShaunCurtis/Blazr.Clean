/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public class WeatherForecastDataService : IWeatherForecastDataService
{
    private readonly IDbContextFactory<InMemoryWeatherDbContext> _dbContext;

    public WeatherForecastDataService(IDbContextFactory<InMemoryWeatherDbContext> dbContext)
       => _dbContext = dbContext;

    public IRecordQueryHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>> GetPagedRecordsQueryHandler(PagedWeatherForecastsQuery query)
        => new PagedWeatherForecastHandler(_dbContext, query);

    public IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse> UpdateRecordCommandHandler(UpdateWeatherForecastCommand command)
        => new UpdateWeatherForecastHandler(_dbContext, command);
}

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public class WeatherForecastAPIDataFactory : IWeatherForecastDataFactory
{
    private readonly HttpClient _httpClient;

    public WeatherForecastAPIDataFactory(HttpClient httpClient)
       => _httpClient = httpClient;

    public IRecordActionHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>> GetPagedRecordsQueryHandler(PagedWeatherForecastsQuery query)
        => new PagedWeatherForecastAPIHandler(_httpClient, query);

    public IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse> UpdateRecordCommandHandler(UpdateWeatherForecastCommand command)
        => new UpdateWeatherForecastAPIHandler(_httpClient, command);
}

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public interface IWeatherForecastDataFactory
{
    IRecordActionHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>> GetPagedRecordsQueryHandler(PagedWeatherForecastsQuery query);

    IRecordCommandHandler<UpdateWeatherForecastCommand, CommandResponse> UpdateRecordCommandHandler(UpdateWeatherForecastCommand command);
}

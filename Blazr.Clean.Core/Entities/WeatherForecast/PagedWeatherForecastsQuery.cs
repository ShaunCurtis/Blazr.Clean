/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public class PagedWeatherForecastsQuery : IRecordAction<IEnumerable<WeatherForecast>>
{
    public ItemsProviderRequest Request { get; private set; }

    public PagedWeatherForecastsQuery(ItemsProviderRequest request)
        => Request = request;
}

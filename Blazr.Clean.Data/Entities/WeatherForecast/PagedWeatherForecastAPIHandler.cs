using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Blazr.Clean.Data;

public class PagedWeatherForecastAPIHandler
    : IRecordActionHandler<PagedWeatherForecastsQuery, IEnumerable<WeatherForecast>>
{
    private PagedWeatherForecastsQuery _query;
    private HttpClient _httpClient;

    public PagedWeatherForecastAPIHandler(HttpClient httpClient, PagedWeatherForecastsQuery query )
    { 
        _httpClient = httpClient;
        _query = query;
    }

    public async ValueTask<IEnumerable<WeatherForecast>> ExecuteAsync()
    {
        IEnumerable<WeatherForecast>? result = null;
        var response = await _httpClient.PostAsJsonAsync<ItemsProviderRequest>($"/api/weatherforecast/list/", _query.Request);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<IEnumerable<WeatherForecast>>();
        return result ?? new List<WeatherForecast>();
    }
}

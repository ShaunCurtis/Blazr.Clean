/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

/// <summary>
/// This is a simple concrete implementation of ViewServiceBase fixing TRecord 
/// We only need to declare a new and pass the IDataBroker to the base class
/// </summary>
public class WeatherForecastViewService : ViewServiceBase<WeatherForecast>
{
    private readonly IWeatherForecastDataFactory _weatherForecastDataService;
    
    public WeatherForecastViewService(IDataBroker dataBroker, IWeatherForecastDataFactory dataService)
        : base(dataBroker)
    { 
        _weatherForecastDataService = dataService;
    }

    public override async ValueTask GetRecordsAsync(ListOptions options)
    {
        var cancel = new CancellationToken();
        var request = new ItemsProviderRequest(options.StartIndex, options.PageSize, cancel);
        // Gets the record collection
        var handler =  _weatherForecastDataService.GetPagedRecordsQueryHandler(new PagedWeatherForecastsQuery(request));
        this.Records = await handler.ExecuteAsync() ;
    }
}

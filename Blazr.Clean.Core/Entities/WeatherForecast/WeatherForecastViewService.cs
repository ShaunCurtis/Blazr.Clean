namespace Blazr.Clean.Core;

public class WeatherForecastViewService : ViewServiceBase<WeatherForecast>
{
    public WeatherForecastViewService(IDataBroker dataBroker) 
        : base(dataBroker)
    { }
}

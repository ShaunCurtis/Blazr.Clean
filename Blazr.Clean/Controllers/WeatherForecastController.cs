/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Controllers;

[ApiController]
public class WeatherForecastController : AppControllerBase<WeatherForecast>
{
    public WeatherForecastController(IDataBroker dataBroker) 
        : base(dataBroker)
    {}
}

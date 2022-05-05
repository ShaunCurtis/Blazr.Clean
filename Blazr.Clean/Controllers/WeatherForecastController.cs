/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Controllers
{
    [ApiController]
    public class WeatherForecastController : Mvc.ControllerBase
    {
        private IDataBroker _dataBroker;

        public WeatherForecastController(IDataBroker dataBroker)
            => _dataBroker = dataBroker;

        [Mvc.Route("/api/[controller]/list")]
        [Mvc.HttpPost]
        public async Task<IEnumerable<WeatherForecast>> GetRecordsAsync([FromBody] ListOptions options)
            => await _dataBroker.GetRecordsAsync<WeatherForecast>(options);

        [Mvc.Route("/api/[controller]/add")]
        [Mvc.HttpPost]
        public async Task<bool> AddRecordAsync([FromBody] WeatherForecast record)
            => await _dataBroker.AddRecordAsync<WeatherForecast>(record);
    }
}

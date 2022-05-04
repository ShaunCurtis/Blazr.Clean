using Blazr.Clean.Core;
using Microsoft.AspNetCore.Mvc;

namespace Blazr.Clean.Controllers
{
    [ApiController]
    public class WeatherForecastController : ControllerBase
    {
        private IDataBroker _dataBroker;

        public WeatherForecastController(IDataBroker dataBroker)
            => _dataBroker = dataBroker;

        [Route("/api/[controller]/list")]
        [HttpPost]
        public async Task<IEnumerable<WeatherForecast>> GetRecordsAsync([FromBody] ListOptions options)
            => await _dataBroker.GetRecordsAsync<WeatherForecast>(options);

        [Route("/api/[controller]/add")]
        [HttpPost]
        public async Task<bool> AddRecordAsync([FromBody] WeatherForecast record)
            => await _dataBroker.AddRecordAsync<WeatherForecast>(record);
    }
}

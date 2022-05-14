/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Core;

public class UpdateWeatherForecastCommand : IRecordCommand<CommandResponse>
{
    public WeatherForecast Record { get; private set; }

    public UpdateWeatherForecastCommand(WeatherForecast item)
        => this.Record = item;
}

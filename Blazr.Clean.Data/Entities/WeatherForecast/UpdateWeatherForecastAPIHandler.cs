namespace Blazr.Clean.Data;

public class UpdateWeatherForecastAPIHandler
    : IRecordActionHandler<UpdateWeatherForecastCommand, CommandResponse>
{
    private readonly HttpClient _httpClient;
    private readonly UpdateWeatherForecastCommand _command;

    public UpdateWeatherForecastAPIHandler(HttpClient httpClient, UpdateWeatherForecastCommand command )
    { 
        _httpClient = httpClient;
        _command = command;
    }

    public async ValueTask<CommandResponse> ExecuteAsync()
    {
        CommandResponse? result = null;
        var response = await _httpClient.PostAsJsonAsync<WeatherForecast>($"/api/add/", _command.Record);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<CommandResponse>();

        return result 
            ?? new CommandResponse { Id = Guid.Empty, Message = "Error updating Record", Success = false };
   }
}

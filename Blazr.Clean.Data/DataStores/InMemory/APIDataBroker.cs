/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Clean.Data;

public class APIDataBroker : IDataBroker
{
    private readonly HttpClient? _httpClient;
    private HttpClient httpClient => _httpClient!;

    public APIDataBroker(HttpClient httpClient)
        => this._httpClient = httpClient!;

    public async ValueTask<bool> AddRecordAsync<TRecord>(TRecord record) where TRecord : class, new()
    {
        var result = false;
        var response = await this.httpClient.PostAsJsonAsync<TRecord>($"/api/add/{record?.GetType().Name}", record!);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<bool>();
        return result;
    }

    public async ValueTask<IEnumerable<TRecord>> GetRecordsAsync<TRecord>(ListOptions options) where TRecord : class, new()
    {
        IEnumerable<TRecord>? result = null;
        var rec = new TRecord();
        var response = await this.httpClient.PostAsJsonAsync<ListOptions>($"/api/list/{rec.GetType().Name}", options);
        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadFromJsonAsync<IEnumerable<TRecord>>();
        return result ?? new List<TRecord>();
    }
}

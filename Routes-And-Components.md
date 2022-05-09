# Routes and Components

The UI domain consists principally of components.  Routes are just components with one or more `Route` attributes.

The UI project is organised into "Entities": logical data sets with their components and routes.  WeatherForecast is an entity: Base is the bin where all the higher level components reside.

### WeatherList

`WeastherList` is a modified version of `FetchData`.  It's a component with no route.  We define the route separately.

```csharp
@namespace Blazr.Clean.UI
@inject IViewService<WeatherForecast> ViewService
@implements IDisposable

<div>

    @if (this.ViewService.Records == null)
    {
        <p><em>Loading...</em></p>
    }
    else
    {
        <table class="table">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var forecast in this.ViewService.Records)
                {
                    <tr>
                        <td>@forecast.Date.ToShortDateString()</td>
                        <td>@forecast.TemperatureC</td>
                        <td>@forecast.TemperatureF</td>
                        <td>@forecast.Summary</td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    protected override async Task OnInitializedAsync()
    {
        await this.ViewService.GetRecordsAsync(new ListOptions { PageSize = 1000 });
        this.ViewService.ListUpdated += this.OnListChanged;
    }

    private void OnListChanged(object? sender, EventArgs e)
        => this.InvokeAsync(this.StateHasChanged);

    public void Dispose()
        => this.ViewService.ListUpdated -= this.OnListChanged;
}
```

The key points to note are:

1. There's no data in the component.  It's all moved into the view service.
2. The View service is injected via it's `IViewService` implementation: the component uses the `IViewService` interface to consume the service.
3. The component calls `GetRecordsAsync` on the view service in `OnInitializedAsync` to get the record collection.
3. The component registers an event handler with the service's `OnListChanged` event and re-renders the component when the event is raised.
4. We implement `IDisposable` to unhook the event handler when the component is destroyed.

## FetchData

FetchData is the route.

```csharp
@page "/fetchdata"
@namespace Blazr.Clean.UI

<PageTitle>Weather forecasts</PageTitle>

@inject IViewService<WeatherForecast> ViewService

<h1>Weather forecasts</h1>

<p>This component demonstrates fetching data from a service.</p>

<div class="p-2 text-end">
    <button class="btn btn-primary" @onclick=this.AddRecord>Add Record</button>
</div>

<WeatherList />

@code {

    // Demo code to add a test record
    private async Task AddRecord()
    {
        // Emulate editing the a new record in a UI Edit form
        var rec = WeatherForecastData.GetForecast();
        this.ViewService.Record.Date = rec.Date;
        this.ViewService.Record.Id = rec.Id;
        this.ViewService.Record.TemperatureC = rec.TemperatureC;
        this.ViewService.Record.Summary = rec.Summary;

        await this.ViewService.AddRecordAsync();
    }
}
```

1. All the key functionality is built into the reusable `WeatherList` component.
2. The add functionality is here to demonstrate the Notification process.  This component adds a record through the View.  The View triggers the `ListChanged` event and `WeatherList` updates the displayed list.

The UI also contains the other routes (Index and Counter) and the Shared components in **Base**, along with `App` in **App**.

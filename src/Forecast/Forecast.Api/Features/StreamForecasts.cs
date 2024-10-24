using Forecast.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Forecast.Api.Features;

public class StreamForecasts : Hub<IStreamForecasts>
{
    public async Task SendMessage(GetForecastResponse[] forecasts)
    {
        // Instead of calling SendAsync, we use the typed hub method from the interface
        await Clients.All.SendMessage(forecasts);  // Call the method defined in IStreamForecasts
    }
}

public interface IStreamForecasts
{
    Task SendMessage(GetForecastResponse[] forecasts);
}

using Forecast.Contracts;
using System.Net.Http.Json;

namespace Forecast.Web.Client.Services;

public class ForecastApiService(
    HttpClient httpClient) : IForecastApiService
{
    public Task<GetForecastResponse[]> GetForecastAsync()
    {
        return httpClient.GetFromJsonAsync<GetForecastResponse[]>("api/forecasts")!;
    }
}

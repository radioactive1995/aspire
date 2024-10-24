using Forecast.Contracts;
using Microsoft.AspNetCore.SignalR.Client;

namespace Forecast.Web.Client.Services;

public interface IForecastApiService
{
    Task<GetForecastResponse[]> GetForecastAsync();
}

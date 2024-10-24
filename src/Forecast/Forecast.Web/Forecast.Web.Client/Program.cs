using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor.Services;
using Forecast.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddHttpClient<ForecastApiService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7153/");
});


await builder.Build().RunAsync();

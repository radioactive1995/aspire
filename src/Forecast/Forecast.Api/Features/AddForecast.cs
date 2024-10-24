using FastEndpoints;
using Forecast.Api.Entities;
using Forecast.Api.Persistance;
using Forecast.Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Forecast.Api.Features;

public static class AddForecast
{
    private static readonly string[]  summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public record Event();

    public class Endpoint(
        ForecastContext db,
        IPublishEndpoint eventBus) : EndpointWithoutRequest<GetForecastResponse[]>
    {
        public override void Configure()
        {
            Post("api/forecasts");
            AllowAnonymous();
        }
        override public async Task HandleAsync(CancellationToken cancellationToken)
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);

            var entity = new ForecastEntity
            {
                Id = Guid.NewGuid(),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)],
                Date = DateTime.Now.AddDays(Random.Shared.Next(summaries.Length)),
            };

            await db.AddAsync(entity, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);

            await eventBus.Publish(new Event());

            await SendOkAsync();
        }
    }

    public class EventHandler(
        ForecastContext db,
        IHubContext<StreamForecasts> hubContext) : IConsumer<Event>
    {
        public async Task Consume(ConsumeContext<Event> context)
        {
            // Fetch all forecasts from the database
            var forecasts = await db.Forecasts
                .Select(f => new GetForecastResponse(DateOnly.FromDateTime(f.Date), f.TemperatureC, f.Summary)).ToArrayAsync();

            // Invoke the SignalR Hub method to send the forecasts
            await hubContext.Clients.All.SendAsync("UpdateForecasts", forecasts.ToArray());
        }
    }
}

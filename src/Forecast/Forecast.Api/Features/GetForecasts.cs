using Azure.Core;
using FastEndpoints;
using Forecast.Api.Entities;
using Forecast.Api.Persistance;
using Forecast.Contracts;
using Microsoft.EntityFrameworkCore;
using System;

namespace Forecast.Api.Features;


public static class GetForecasts
{
    public class Endpoint(ForecastContext db) : EndpointWithoutRequest<GetForecastResponse[], Mapper>
    {
        public override void Configure()
        {
            Get("api/forecasts");
            AllowAnonymous();
        }
        override public async Task HandleAsync(CancellationToken cancellationToken)
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);

            var forecasts = await db.Forecasts
                .OrderBy(f => f.Date)
                .Select(f => Map.FromEntity(f))
                .ToArrayAsync(cancellationToken);
            
            await SendOkAsync(forecasts);
        }
    }

    public class Mapper : ResponseMapper<GetForecastResponse, ForecastEntity>
    {
        public override GetForecastResponse FromEntity(ForecastEntity e)
        {
            return new GetForecastResponse(DateOnly.FromDateTime(e.Date), e.TemperatureC, e.Summary);
        }
    }
}

using FastEndpoints;
using Forecast.Api.Features;
using Forecast.Api.Persistance;
using Forecast.Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//add cosmos dbcontext
builder.Services.AddDbContext<ForecastContext>(options =>
{
    options.UseCosmos(
        connectionString: "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
        databaseName: "ForecastDb"
    );
});

builder.Services.AddMassTransit(e =>
{
    e.AddConsumer<AddForecast.EventHandler>();
    e.AddConsumer<DeleteForecasts.EventHandler>();

    e.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", 5672, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });

    e.AddInMemoryInboxOutbox();
});


builder.Services.AddSignalR();
builder.Services.AddOpenApi();

builder.Services.AddFastEndpoints();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Weather Forecast API",
        Version = "v1",
        Description = "An API to retrieve weather forecasts",
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.AllowAnyOrigin() // Add allowed origins
              .AllowAnyMethod()   // Allows GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader();   // Allows any headers (like Authorization, Content-Type)
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapFastEndpoints();

app.UseCors("AllowSpecificOrigins");

app.MapHub<StreamForecasts>("/forecasts-stream");

app.Run();


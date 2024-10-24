using Forecast.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forecast.Api.Persistance;

public class ForecastContext : DbContext
{
    public ForecastContext(DbContextOptions<ForecastContext> options) : base(options)
    {
    }

    public DbSet<ForecastEntity> Forecasts { get; set; } = null!;

    //define cosmos modelbuilder ovveride
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ForecastEntity>(entity =>
        {
            entity.ToContainer("Forecasts");
            entity.Property(e => e.Id).ToJsonProperty("id");
            entity.Property(e => e.Date).ToJsonProperty("date");
            entity.Property(e => e.Summary).ToJsonProperty("summary");
            entity.Property(e => e.TemperatureC).ToJsonProperty("temperatureC");
            entity.HasPartitionKey(e => e.Id);
        });
    }
}

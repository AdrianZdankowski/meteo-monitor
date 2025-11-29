using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public DashboardController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpGet]
    public async Task<ActionResult<List<object>>> Get()
    {
        var sensorsCollection = _mongoDbService.GetCollection<Sensor>("sensors");
        var readingsCollection = _mongoDbService.GetCollection<SensorReading>("sensor_readings");
        
        var sensors = await sensorsCollection.Find(_ => true).ToListAsync();
        var dashboardData = new List<object>();

        foreach (var sensor in sensors)
        {
            var lastReading = await readingsCollection.Find(r => r.SensorId == sensor.SensorId)
                .SortByDescending(r => r.Timestamp)
                .FirstOrDefaultAsync();

            var last100Readings = await readingsCollection.Find(r => r.SensorId == sensor.SensorId)
                .SortByDescending(r => r.Timestamp)
                .Limit(100)
                .ToListAsync();

            var average = last100Readings.Any() 
                ? last100Readings.Average(r => r.Value) 
                : 0;

            dashboardData.Add(new
            {
                SensorId = sensor.SensorId,
                SensorType = sensor.SensorType,
                LastValue = lastReading?.Value ?? 0,
                LastTimestamp = lastReading?.Timestamp ?? 0,
                AverageLast100 = average
            });
        }

        return Ok(dashboardData);
    }
}

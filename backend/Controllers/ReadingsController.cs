using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public ReadingsController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SensorReading>>> Get(
        [FromQuery] string? sensorId,
        [FromQuery] string? sensorType,
        [FromQuery] double? from,
        [FromQuery] double? to,
        [FromQuery] string? sort = "desc")
    {
        var collection = _mongoDbService.GetCollection<SensorReading>("sensor_readings");
        var builder = Builders<SensorReading>.Filter;
        var filter = builder.Empty;

        if (!string.IsNullOrEmpty(sensorId))
        {
            filter &= builder.Eq(r => r.SensorId, sensorId);
        }

        if (!string.IsNullOrEmpty(sensorType))
        {
            filter &= builder.Eq(r => r.SensorType, sensorType);
        }

        if (from.HasValue)
        {
            filter &= builder.Gte(r => r.Timestamp, from.Value);
        }

        if (to.HasValue)
        {
            filter &= builder.Lte(r => r.Timestamp, to.Value);
        }

        var sortDefinition = sort == "asc"
            ? Builders<SensorReading>.Sort.Ascending(r => r.Timestamp)
            : Builders<SensorReading>.Sort.Descending(r => r.Timestamp);

        var readings = await collection.Find(filter).Sort(sortDefinition).Limit(1000).ToListAsync();

        return Ok(readings);
    }
}

using backend.Constants;
using backend.Models;
using backend.Repositories.Interfaces;
using backend.Services.Interfaces;
using MongoDB.Driver;

namespace backend.Repositories;

public class ReadingRepository : IReadingRepository
{
    private readonly IMongoCollection<SensorReading> _collection;

    public ReadingRepository(IMongoDbService mongoDbService)
    {
        _collection = mongoDbService.GetCollection<SensorReading>(CollectionNames.SensorReadings);
    }

    public async Task<List<SensorReading>> GetAsync(
        string? sensorId = null,
        string? sensorType = null,
        double? from = null,
        double? to = null,
        string sort = "desc",
        int limit = 1000)
    {
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

        return await _collection.Find(filter).Sort(sortDefinition).Limit(limit).ToListAsync();
    }

    public async Task<SensorReading?> GetLatestBySensorIdAsync(string sensorId)
    {
        return await _collection
            .Find(r => r.SensorId == sensorId)
            .SortByDescending(r => r.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<List<SensorReading>> GetLatestBySensorIdAsync(string sensorId, int count)
    {
        return await _collection
            .Find(r => r.SensorId == sensorId)
            .SortByDescending(r => r.Timestamp)
            .Limit(count)
            .ToListAsync();
    }

    public async Task<SensorReading> CreateAsync(SensorReading reading)
    {
        await _collection.InsertOneAsync(reading);
        return reading;
    }
}

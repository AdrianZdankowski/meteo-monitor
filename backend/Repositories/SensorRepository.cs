using backend.Constants;
using backend.Models;
using backend.Repositories.Interfaces;
using backend.Services.Interfaces;
using MongoDB.Driver;

namespace backend.Repositories;

public class SensorRepository : ISensorRepository
{
    private readonly IMongoCollection<Sensor> _collection;

    public SensorRepository(IMongoDbService mongoDbService)
    {
        _collection = mongoDbService.GetCollection<Sensor>(CollectionNames.Sensors);
    }

    public async Task<List<Sensor>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<Sensor?> GetBySensorIdAsync(string sensorId)
    {
        var filter = Builders<Sensor>.Filter.Eq(s => s.SensorId, sensorId);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Sensor> CreateAsync(Sensor sensor)
    {
        await _collection.InsertOneAsync(sensor);
        return sensor;
    }

    public async Task<bool> ExistsAsync(string sensorId)
    {
        var filter = Builders<Sensor>.Filter.Eq(s => s.SensorId, sensorId);
        return await _collection.Find(filter).AnyAsync();
    }
}

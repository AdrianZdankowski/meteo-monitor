using backend.Models;

namespace backend.Repositories.Interfaces;

public interface IReadingRepository
{
    Task<List<SensorReading>> GetAsync(
        string? sensorId = null,
        string? sensorType = null,
        double? from = null,
        double? to = null,
        string sort = "desc",
        int limit = 1000);
    
    Task<SensorReading?> GetLatestBySensorIdAsync(string sensorId);
    Task<List<SensorReading>> GetLatestBySensorIdAsync(string sensorId, int count);
    Task<SensorReading> CreateAsync(SensorReading reading);
}

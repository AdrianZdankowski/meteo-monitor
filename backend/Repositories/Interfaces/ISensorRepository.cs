using backend.Models;

namespace backend.Repositories.Interfaces;

public interface ISensorRepository
{
    Task<List<Sensor>> GetAllAsync();
    Task<Sensor?> GetBySensorIdAsync(string sensorId);
    Task<Sensor> CreateAsync(Sensor sensor);
    Task<bool> ExistsAsync(string sensorId);
}

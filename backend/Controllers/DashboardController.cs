using backend.Models;
using backend.Models.DTOs;
using backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ISensorRepository _sensorRepository;
    private readonly IReadingRepository _readingRepository;

    public DashboardController(
        ISensorRepository sensorRepository,
        IReadingRepository readingRepository)
    {
        _sensorRepository = sensorRepository;
        _readingRepository = readingRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<DashboardSensorDto>>> Get()
    {
        var sensors = await _sensorRepository.GetAllAsync();
        var dashboardData = new List<DashboardSensorDto>();

        foreach (var sensor in sensors)
        {
            var lastReading = await _readingRepository.GetLatestBySensorIdAsync(sensor.SensorId);
            var last100Readings = await _readingRepository.GetLatestBySensorIdAsync(sensor.SensorId, 100);

            var average = last100Readings.Any() 
                ? last100Readings.Average(r => r.Value) 
                : 0;

            dashboardData.Add(new DashboardSensorDto
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

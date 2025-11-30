using backend.Models;
using backend.Models.DTOs;
using backend.Repositories.Interfaces;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly ILogger<SensorsController> _logger;
    private readonly ISensorRepository _sensorRepository;
    private readonly IBlockchainService _blockchainService;

    public SensorsController(
        ILogger<SensorsController> logger,
        ISensorRepository sensorRepository,
        IBlockchainService blockchainService)
    {
        _logger = logger;
        _sensorRepository = sensorRepository;
        _blockchainService = blockchainService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SensorWithBalanceDto>>> Get()
    {
        try
        {
            var sensors = await _sensorRepository.GetAllAsync();

            var sensorsWithBalances = new List<SensorWithBalanceDto>();

            foreach (var sensor in sensors)
            {
                var balance = 0m;
                if (!string.IsNullOrEmpty(sensor.WalletAddress))
                {
                    balance = await _blockchainService.GetBalanceAsync(sensor.WalletAddress);
                }

                sensorsWithBalances.Add(new SensorWithBalanceDto
                {
                    Id = sensor.Id,
                    SensorId = sensor.SensorId,
                    SensorType = sensor.SensorType,
                    WalletAddress = sensor.WalletAddress,
                    TokenBalance = balance
                });
            }

            return Ok(sensorsWithBalances);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sensors");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{sensorId}/balance")]
    public async Task<ActionResult<decimal>> GetSensorBalance(string sensorId)
    {
        try
        {
            var sensor = await _sensorRepository.GetBySensorIdAsync(sensorId);

            if (sensor == null)
            {
                return NotFound($"Sensor {sensorId} not found");
            }

            if (string.IsNullOrEmpty(sensor.WalletAddress))
            {
                return Ok(0);
            }

            var balance = await _blockchainService.GetBalanceAsync(sensor.WalletAddress);
            return Ok(balance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching balance for sensor {SensorId}", sensorId);
            return StatusCode(500, "Internal server error");
        }
    }
}

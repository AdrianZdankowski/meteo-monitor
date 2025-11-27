using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly ILogger<SensorsController> _logger;
    private readonly MongoDbService _mongoDbService;
    private readonly BlockchainService _blockchainService;

    public SensorsController(
        ILogger<SensorsController> logger,
        MongoDbService mongoDbService,
        BlockchainService blockchainService)
    {
        _logger = logger;
        _mongoDbService = mongoDbService;
        _blockchainService = blockchainService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SensorWithBalance>>> Get()
    {
        try
        {
            var collection = _mongoDbService.GetCollection<Sensor>("sensors");
            var sensors = await collection.Find(_ => true).ToListAsync();

            var sensorsWithBalances = new List<SensorWithBalance>();

            foreach (var sensor in sensors)
            {
                var balance = 0m;
                if (!string.IsNullOrEmpty(sensor.WalletAddress))
                {
                    balance = await _blockchainService.GetBalanceAsync(sensor.WalletAddress);
                }

                sensorsWithBalances.Add(new SensorWithBalance
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
            var collection = _mongoDbService.GetCollection<Sensor>("sensors");
            var filter = Builders<Sensor>.Filter.Eq(s => s.SensorId, sensorId);
            var sensor = await collection.Find(filter).FirstOrDefaultAsync();

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

public class SensorWithBalance
{
    public string? Id { get; set; }
    public string SensorId { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public decimal TokenBalance { get; set; }
}

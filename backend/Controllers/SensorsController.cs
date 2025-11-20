using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly MongoDbService _mongoDbService;

    public SensorsController(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Sensor>>> Get()
    {
        var collection = _mongoDbService.GetCollection<Sensor>("sensors");
        var sensors = await collection.Find(_ => true).ToListAsync();
        return Ok(sensors);
    }
}

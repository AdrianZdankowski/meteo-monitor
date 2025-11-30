using backend.Models;
using backend.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReadingsController : ControllerBase
{
    private readonly IReadingRepository _readingRepository;

    public ReadingsController(IReadingRepository readingRepository)
    {
        _readingRepository = readingRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<SensorReading>>> Get(
        [FromQuery] string? sensorId,
        [FromQuery] string? sensorType,
        [FromQuery] double? from,
        [FromQuery] double? to,
        [FromQuery] string? sort = "desc")
    {
        var readings = await _readingRepository.GetAsync(
            sensorId,
            sensorType,
            from,
            to,
            sort ?? "desc");

        return Ok(readings);
    }
}

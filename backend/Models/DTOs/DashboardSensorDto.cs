namespace backend.Models.DTOs;

public class DashboardSensorDto
{
    public string SensorId { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public double LastValue { get; set; }
    public double LastTimestamp { get; set; }
    public double AverageLast100 { get; set; }
}

namespace backend.Models.DTOs;

public class SensorWithBalanceDto
{
    public string? Id { get; set; }
    public string SensorId { get; set; } = string.Empty;
    public string SensorType { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public decimal TokenBalance { get; set; }
}

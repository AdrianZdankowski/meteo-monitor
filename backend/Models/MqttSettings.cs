namespace backend.Models;

public class MqttSettings
{
    public string BrokerHost { get; set; } = "localhost";
    public int BrokerPort { get; set; } = 1883;
    public string ClientId { get; set; } = "backend-service";
    public string[] Topics { get; set; } = Array.Empty<string>();
}

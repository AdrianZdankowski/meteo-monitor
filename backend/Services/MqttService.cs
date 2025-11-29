using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Microsoft.Extensions.Options;
using backend.Models;
using System.Text;
using System.Text.Json;
using MongoDB.Driver;

namespace backend.Services;

using backend.Hubs;
using Microsoft.AspNetCore.SignalR;

public class MqttService : IHostedService, IDisposable
{
    private readonly ILogger<MqttService> _logger;
    private readonly MqttSettings _settings;
    private IManagedMqttClient? _mqttClient;
    private readonly MongoDbService _mongoDb;
    private readonly IHubContext<DashboardHub> _hubContext;
    private readonly BlockchainService _blockchainService;

    public MqttService(
        ILogger<MqttService> logger,
        IOptions<MqttSettings> settings,
        MongoDbService mongoDb,
        IHubContext<DashboardHub> hubContext,
        BlockchainService blockchainService)
    {
        _logger = logger;
        _settings = settings.Value;
        _mongoDb = mongoDb;
        _hubContext = hubContext;
        _blockchainService = blockchainService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateManagedMqttClient();

        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_settings.BrokerHost, _settings.BrokerPort)
            .WithClientId(_settings.ClientId)
            .Build();

        var managedOptions = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(clientOptions)
            .Build();

        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;

        await _mqttClient.StartAsync(managedOptions);
        _logger.LogInformation("Connected with MQTT broker at {Host}:{Port}", _settings.BrokerHost, _settings.BrokerPort);

        foreach (var topic in _settings.Topics)
        {
            await _mqttClient.SubscribeAsync(topic);
            _logger.LogInformation("Subscribed to topic: {Topic}", topic);
        }
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        var topic = e.ApplicationMessage.Topic;

        _logger.LogInformation("Received a message from topic {Topic}: {Payload}", topic, payload);

        try
        {
            var jsonDocument = JsonDocument.Parse(payload);
            var root = jsonDocument.RootElement;

            var sensorId = root.GetProperty("sensor_id").GetString() ?? string.Empty;
            var sensorType = root.GetProperty("sensor_type").GetString() ?? string.Empty;

            await RegisterSensorIfNewAsync(sensorId, sensorType);

            var sensorReading = new SensorReading
            {
                SensorId = sensorId,
                SensorType = sensorType,
                Timestamp = root.GetProperty("timestamp").GetDouble(),
                Value = root.GetProperty("value").GetDouble(),
            };

            var readingsCollection = _mongoDb.GetCollection<SensorReading>("sensor_readings");
            await readingsCollection.InsertOneAsync(sensorReading);

            _logger.LogInformation("Saved a reading from sensor {SensorId} to the database", sensorReading.SensorId);

            await _hubContext.Clients.All.SendAsync("ReceiveSensorUpdate", sensorReading.SensorId, sensorReading.Value, sensorReading.Timestamp);
            _ = Task.Run(async () =>
            {
                try
                {
                    var sensorsCollection = _mongoDb.GetCollection<Sensor>("sensors");
                    var filter = Builders<Sensor>.Filter.Eq(s => s.SensorId, sensorId);
                    var sensor = await sensorsCollection.Find(filter).FirstOrDefaultAsync();
                    
                    if (sensor != null && !string.IsNullOrEmpty(sensor.WalletAddress))
                    {
                        await _blockchainService.RewardSensorAsync(sensor.WalletAddress);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error rewarding sensor {SensorId} in background", sensorId);
                }
            });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error while parsing JSON from topic: {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving data to MongoDB in topic: {Topic}", topic);
        }
    }

    private async Task RegisterSensorIfNewAsync(string sensorId, string sensorType)
    {
        try
        {
            var sensorsCollection = _mongoDb.GetCollection<Sensor>("sensors");

            var filter = Builders<Sensor>.Filter.Eq(s => s.SensorId, sensorId);
            var existingSensor = await sensorsCollection.Find(filter).FirstOrDefaultAsync();

            if (existingSensor == null)
            {
                var newSensor = new Sensor
                {
                    SensorId = sensorId,
                    SensorType = sensorType,
                    WalletAddress = _blockchainService.GenerateNewWallet()
                };

                await sensorsCollection.InsertOneAsync(newSensor);
                _logger.LogInformation("Registered a new sensor: {SensorId} ({SensorType}) with wallet: {Wallet}", 
                    sensorId, sensorType, newSensor.WalletAddress);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while registering sensor {SensorId}", sensorId);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient != null)
        {
            await _mqttClient.StopAsync();
            _logger.LogInformation("Disconnected from the MQTT broker");
        }
    }

    public void Dispose()
    {
        _mqttClient?.Dispose();
    }
}

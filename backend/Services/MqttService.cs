using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using Microsoft.Extensions.Options;
using backend.Models;
using System.Text;
using System.Text.Json;
using MongoDB.Bson;
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

        // Konfiguracja opcji klienta
        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_settings.BrokerHost, _settings.BrokerPort)
            .WithClientId(_settings.ClientId)
            .Build();

        var managedOptions = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(clientOptions)
            .Build();

        // Obsługa otrzymanych wiadomości
        _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;

        // Połączenie z brokerem
        await _mqttClient.StartAsync(managedOptions);
        _logger.LogInformation("Połączono z MQTT brokerem na {Host}:{Port}", _settings.BrokerHost, _settings.BrokerPort);

        // Subskrypcja tematów
        foreach (var topic in _settings.Topics)
        {
            await _mqttClient.SubscribeAsync(topic);
            _logger.LogInformation("Zasubskrybowano temat: {Topic}", topic);
        }
    }

    private async Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        var topic = e.ApplicationMessage.Topic;

        _logger.LogInformation("Otrzymano wiadomość z tematu {Topic}: {Payload}", topic, payload);

        try
        {
            // Parsowanie JSON z payload
            var jsonDocument = JsonDocument.Parse(payload);
            var root = jsonDocument.RootElement;

            var sensorId = root.GetProperty("sensor_id").GetString() ?? string.Empty;
            var sensorType = root.GetProperty("sensor_type").GetString() ?? string.Empty;

            // Rejestracja sensora (jeśli pierwszy raz)
            await RegisterSensorIfNewAsync(sensorId, sensorType);

            // Tworzenie obiektu SensorReading
            var sensorReading = new SensorReading
            {
                SensorId = sensorId,
                SensorType = sensorType,
                Timestamp = root.GetProperty("timestamp").GetDouble(),
                Value = root.GetProperty("value").GetDouble(),
            };

            // Zapisywanie do MongoDB
            var readingsCollection = _mongoDb.GetCollection<SensorReading>("sensor_readings");
            await readingsCollection.InsertOneAsync(sensorReading);

            _logger.LogInformation("Zapisano odczyt z sensora {SensorId} do bazy danych", sensorReading.SensorId);

            // Reward sensor with tokens
            var sensorsCollection = _mongoDb.GetCollection<Sensor>("sensors");
            var filter = Builders<Sensor>.Filter.Eq(s => s.SensorId, sensorId);
            var sensor = await sensorsCollection.Find(filter).FirstOrDefaultAsync();
            
            if (sensor != null && !string.IsNullOrEmpty(sensor.WalletAddress))
            {
                await _blockchainService.RewardSensorAsync(sensor.WalletAddress);
            }

            // Broadcast update via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveSensorUpdate", sensorReading.SensorId, sensorReading.Value, sensorReading.Timestamp);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Błąd parsowania JSON z tematu {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd zapisywania danych do MongoDB z tematu {Topic}", topic);
        }
    }

    private async Task RegisterSensorIfNewAsync(string sensorId, string sensorType)
    {
        try
        {
            var sensorsCollection = _mongoDb.GetCollection<Sensor>("sensors");

            // Sprawdź czy sensor już istnieje
            var filter = Builders<Sensor>.Filter.Eq(s => s.SensorId, sensorId);
            var existingSensor = await sensorsCollection.Find(filter).FirstOrDefaultAsync();

            if (existingSensor == null)
            {
                // Utwórz nowy sensor z wygenerowanym adresem portfela
                var newSensor = new Sensor
                {
                    SensorId = sensorId,
                    SensorType = sensorType,
                    WalletAddress = _blockchainService.GenerateNewWallet()
                };

                await sensorsCollection.InsertOneAsync(newSensor);
                _logger.LogInformation("Zarejestrowano nowy sensor: {SensorId} ({SensorType}) z portfelem: {Wallet}", 
                    sensorId, sensorType, newSensor.WalletAddress);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd rejestracji sensora {SensorId}", sensorId);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient != null)
        {
            await _mqttClient.StopAsync();
            _logger.LogInformation("Rozłączono z MQTT brokerem");
        }
    }

    public void Dispose()
    {
        _mqttClient?.Dispose();
    }
}

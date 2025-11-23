using backend.Models;
using backend.Services;
using backend.Hubs;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Required for SignalR
        });     
});

// Configure MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));
builder.Services.AddSingleton<MongoDbService>();

// Configure MQTT
builder.Services.Configure<MqttSettings>(
    builder.Configuration.GetSection("Mqtt"));
builder.Services.AddHostedService<MqttService>();

// Configure Blockchain
builder.Services.Configure<BlockchainSettings>(
    builder.Configuration.GetSection("Blockchain"));
builder.Services.AddSingleton<BlockchainService>();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors();

// Initialize blockchain service
var blockchainService = app.Services.GetRequiredService<BlockchainService>();
await blockchainService.InitializeAsync();

var api = app.MapGroup("/api");

app.MapControllers();
app.MapHub<DashboardHub>("/dashboardHub");

app.Run();

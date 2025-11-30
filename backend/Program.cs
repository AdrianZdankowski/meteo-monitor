using backend.Models;
using backend.Services;
using backend.Services.Interfaces;
using backend.Repositories;
using backend.Repositories.Interfaces;
using backend.Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });     
});

// Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));
builder.Services.Configure<MqttSettings>(
    builder.Configuration.GetSection("Mqtt"));
builder.Services.Configure<BlockchainSettings>(
    builder.Configuration.GetSection("Blockchain"));

// Services
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();
builder.Services.AddSingleton<IBlockchainService, BlockchainService>();
builder.Services.AddHostedService<MqttService>();

// Repositories
builder.Services.AddSingleton<ISensorRepository, SensorRepository>();
builder.Services.AddSingleton<IReadingRepository, ReadingRepository>();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors();

var blockchainService = app.Services.GetRequiredService<IBlockchainService>();
await blockchainService.InitializeAsync();

app.MapControllers();
app.MapHub<DashboardHub>("/dashboardHub");

app.Run();

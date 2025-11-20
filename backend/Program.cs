using backend.Models;
using backend.Services;
using backend.Hubs;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Allow Vite frontend
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

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();
app.UseCors();

var api = app.MapGroup("/api");

app.MapControllers();
app.MapHub<DashboardHub>("/dashboardHub");

app.Run();

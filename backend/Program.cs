using backend.Models;
using backend.Services;

// Add services to the container.

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
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

var app = builder.Build();
app.UseCors();

var api = app.MapGroup("/api");

//builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

app.Run();

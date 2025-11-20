using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class SensorReading
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("sensor_id")]
    public string SensorId { get; set; } = string.Empty;

    [BsonElement("sensor_type")]
    public string SensorType { get; set; } = string.Empty;

    [BsonElement("timestamp")]
    public double Timestamp { get; set; }

    [BsonElement("value")]
    public double Value { get; set; }

}

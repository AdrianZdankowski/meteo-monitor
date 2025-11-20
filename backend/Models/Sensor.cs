using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace backend.Models;

public class Sensor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("sensor_id")]
    public string SensorId { get; set; } = string.Empty;

    [BsonElement("sensor_type")]
    public string SensorType { get; set; } = string.Empty;

}

using MongoDB.Driver;

namespace backend.Services.Interfaces;

public interface IMongoDbService
{
    IMongoDatabase Database { get; }
    IMongoCollection<T> GetCollection<T>(string collectionName);
}

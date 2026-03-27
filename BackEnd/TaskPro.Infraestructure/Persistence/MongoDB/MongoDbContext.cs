using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using TaskPro.Infraestructure.Settings;

namespace TaskPro.Infraestructure.Persistence.MongoDB
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Guid)))
            {
                BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            }

            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }
        public IMongoCollection<T> GetCollection<T>(string collectionName)
            => _database.GetCollection<T>(collectionName);
    }
}


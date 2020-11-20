using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Security.Authentication;

namespace HyperTaskTools
{
    public sealed class MongoConnector
    {
        public MongoClient mongoClient;

        public MongoConnector(string mongoConnectionString)
        {
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(
                  new MongoUrl(mongoConnectionString)
                );
                settings.SslSettings = new SslSettings() { 
                    EnabledSslProtocols = SslProtocols.Tls12,
                };
                settings.RetryWrites = false;
                mongoClient = new MongoClient(settings);
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to create MongoConnector", ex);
            }
        }

        public string GetLatestRequestCharge(string dataBaseName)
        {
            try
            {
                return this.mongoClient
                           .GetDatabase(dataBaseName)
                           .RunCommand<BsonDocument>(new BsonDocument { { "getLastRequestStatistics", 1 } })["RequestCharge"].AsBsonValue.AsDouble.ToString();
            } 
            catch (Exception ex)
            {
                return "n/a";
            }
        }
    }
}

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
                settings.SslSettings =
                  new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
                mongoClient = new MongoClient(settings);
                settings.RetryWrites = false;
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to create MongoConnector", ex);
            }
        }
    }
}

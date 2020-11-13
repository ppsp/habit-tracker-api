using HyperTaskCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;

namespace HyperTaskServices.Models.Mongo
{
    public class MongoUserConfig
    {
        [BsonElement]
        public MongoKeyValuePair[] Configs { get; set; } = new MongoKeyValuePair[0];
        public MongoUserConfig()
        {

        }

        public static MongoUserConfig fromConfig(UserConfig config)
        {
            MongoUserConfig newConfig = new MongoUserConfig();
            newConfig.Configs = config.Configs.Select(p => new MongoKeyValuePair(p)).ToArray();
            return newConfig;
        }

        public UserConfig ToConfig()
        {
            UserConfig config = new UserConfig();
            config.Configs = this.Configs.Select(p => p.ToConfigKeyValuePair()).ToArray();
            return config;
        }
    }
}

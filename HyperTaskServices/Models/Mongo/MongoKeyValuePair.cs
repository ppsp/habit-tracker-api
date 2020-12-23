using HyperTaskCore.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace HyperTaskServices.Models.Mongo
{
    public class MongoKeyValuePair
    {
        [BsonElement]
        public string key { get; set; }
        [BsonElement]
        public object value { get; set; }

        public ConfigKeyValuePair ToConfigKeyValuePair()
        {
            return new ConfigKeyValuePair()
            {
                key = this.key,
                value = this.value
            };
        }

        public MongoKeyValuePair()
        {

        }

        public MongoKeyValuePair(ConfigKeyValuePair keyValuePair)
        {
            this.key = keyValuePair.key;
            this.value = keyValuePair.value;
        }
    }
}

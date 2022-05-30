using HyperTaskCore.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace HyperTaskServices.Models.Mongo
{
    [BsonIgnoreExtraElements]
    public class MongoUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement]
        public string UserId { get; set; }
        [BsonElement]
        public MongoUserConfig Config { get; set; }
        [BsonElement]
        public DateTime? LastActivityDate { get; set; }
        [BsonElement]
        public DateTime InsertDate { get; set; }

        public MongoUser()
        {

        }

        public MongoUser(IUser user)
        {
            this.Id = user.Id;
            this.UserId = user.UserId;
            this.LastActivityDate = user.LastActivityDate;
            this.Config = MongoUserConfig.fromConfig(user.Config ?? new UserConfig());
            this.InsertDate = user.InsertDate;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = this.Id;
            user.UserId = this.UserId;
            user.LastActivityDate = this.LastActivityDate;
            user.Config = this.Config == null ? new UserConfig() : this.Config.ToConfig();
            user.InsertDate = this.InsertDate;
            return user;
        }
    }
}

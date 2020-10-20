using Google.Cloud.Firestore;
using HyperTaskCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HyperTaskServices.Models.Firestore
{
    [FirestoreData]
    public class FireUser
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public FireUserConfig Config { get; set; }
        [FirestoreProperty]
        public DateTime? LastActivityDate { get; set; }
        [FirestoreProperty]
        public DateTime InsertDate { get; set; }

        public FireUser()
        {

        }

        public FireUser(IUser user)
        {
            this.Id = user.Id;
            this.UserId = user.UserId;
            this.LastActivityDate = user.LastActivityDate;
            this.Config = FireUserConfig.fromConfig(user.Config ?? new UserConfig());
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

using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerServices.Models.Firestore
{
    [FirestoreData]
    public class FireUser : IUser
    {
        public string Id { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }
        [FirestoreProperty]
        public eLanguage PreferedLanguage { get; set; }

        public FireUser()
        {

        }

        public FireUser(IUser user)
        {
            this.Id = user.Id;
            this.UserId = user.UserId;
            this.PreferedLanguage = user.PreferedLanguage;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = this.Id;
            user.UserId = this.UserId;
            user.PreferedLanguage = this.PreferedLanguage;
            return user;
        }
    }
}

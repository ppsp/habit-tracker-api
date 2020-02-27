using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerServices.Models.Firestore
{
    public class FireUser : IUser
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public eLanguage PreferedLanguage { get; set; }

        public FireUser(IUser user)
        {
            this.Id = user.Id;
            this.PreferedLanguage = user.PreferedLanguage;
        }

        public User ToUser()
        {
            var user = new User();
            user.Id = this.Id;
            user.PreferedLanguage = this.PreferedLanguage;
            return user;
        }
    }
}

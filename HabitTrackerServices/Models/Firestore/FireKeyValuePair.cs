using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerServices.Models.Firestore
{
    [FirestoreData]
    public class FireKeyValuePair
    {
        [FirestoreProperty]
        public string key { get; set; }

        [FirestoreProperty]
        public object value { get; set; }

        public ConfigKeyValuePair ToConfigKeyValuePair()
        {
            return new ConfigKeyValuePair()
            {
                key = this.key,
                value = this.value
            };
        }

        public FireKeyValuePair()
        {

        }

        public FireKeyValuePair(ConfigKeyValuePair keyValuePair)
        {
            this.key = keyValuePair.key;
            this.value = keyValuePair.value;
        }
    }
}

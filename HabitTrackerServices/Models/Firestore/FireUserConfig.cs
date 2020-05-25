using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HabitTrackerServices.Models.Firestore
{
    [FirestoreData]
    public class FireUserConfig
    {
        [FirestoreProperty]
        public FireKeyValuePair[] Configs { get; set; } = new FireKeyValuePair[0];
        public FireUserConfig()
        {

        }

        public static FireUserConfig fromConfig(UserConfig config)
        {
            FireUserConfig newConfig = new FireUserConfig();
            newConfig.Configs = config.Configs.Select(p => new FireKeyValuePair(p)).ToArray();
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

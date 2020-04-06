using Google.Cloud.Firestore;
using HabitTrackerCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerServices.Models.Firestore
{
    [FirestoreData]
    public class FireUserConfig
    {
        [FirestoreProperty]
        public eLanguage PreferedLanguage { get; set; }
        [FirestoreProperty]
        public string EndOfDayTime { get; set; }
        [FirestoreProperty]
        public string DefaultAfterTaskName { get; set; }

        public FireUserConfig()
        {

        }

        public static FireUserConfig fromConfig(UserConfig config)
        {
            FireUserConfig newConfig = new FireUserConfig();
            newConfig.DefaultAfterTaskName = config.DefaultAfterTaskName;
            newConfig.EndOfDayTime = config.EndOfDayTime;
            newConfig.PreferedLanguage = config.PreferedLanguage;
            return newConfig;
        }

        public UserConfig ToConfig()
        {
            UserConfig config = new UserConfig();
            config.DefaultAfterTaskName = this.DefaultAfterTaskName;
            config.EndOfDayTime = this.EndOfDayTime;
            config.PreferedLanguage = this.PreferedLanguage;
            return config;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public class User : IUser
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public eLanguage PreferedLanguage { get; set; }
        public string EndOfDayTime { get; set; }
    }

    public class NULLUser : IUser
    {
        private static NULLUser _instance = new NULLUser();

        public static NULLUser Instance
        {
            get
            {
                if (_instance == null)
                    return new NULLUser();
                return _instance;
            }
        }

        public string Id { get; set; }
        public string UserId { get; set; }
        public eLanguage PreferedLanguage { get; set; }
        public string EndOfDayTime { get; set; }
    }
}

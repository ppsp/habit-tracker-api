using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public class UserConfig
    {
        public eLanguage PreferedLanguage { get; set; } = eLanguage.English;
        public string EndOfDayTime { get; set; } = "00:00";
        public string DefaultAfterTaskName { get; set; }
    }
}

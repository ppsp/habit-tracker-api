using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public interface IUser
    {
        string Id { get; set; }
        string UserId { get; set; }
        eLanguage PreferedLanguage { get; set; }
    }
}

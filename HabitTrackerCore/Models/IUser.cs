﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public interface IUser
    {
        string Id { get; set; }
        eLanguage PreferedLanguage { get; set; }
    }
}

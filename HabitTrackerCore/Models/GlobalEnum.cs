using System;
using System.Collections.Generic;
using System.Text;

namespace HabitTrackerCore.Models
{
    public enum eTaskFrequency
    {
        Daily,
        Weekly,
        Monthly,
        BiWeekly,
        Custom,
    }

    public enum eResultType
    {
        Binary,
        Decimal,
        Time,
    }
}

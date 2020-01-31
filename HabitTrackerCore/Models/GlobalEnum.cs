namespace HabitTrackerCore.Models
{
    public enum eTaskFrequency
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        BiWeekly = 3,
        Custom = 4,
        Once = 5,
        UntilDone = 6
    }

    public enum eResultType
    {
        Binary,
        Decimal,
        Time,
    }

    public enum eBugReportType
    {
        Bug,
        Suggestion
    }

    public enum eStatType
    {
        Regular = 0,
        TimeUp = 1,
        TimeSleep = 2,
        TimeNonWaterStart = 3,
        TimeNonWaterStop = 4,
        TimeEatStart = 5,
        TimeEatStop = 6,
    }
}


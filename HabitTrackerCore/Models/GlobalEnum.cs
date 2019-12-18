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
}


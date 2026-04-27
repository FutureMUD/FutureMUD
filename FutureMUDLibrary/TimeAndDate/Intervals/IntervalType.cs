namespace MudSharp.TimeAndDate.Intervals
{
    public enum IntervalType
    {
        Daily = 0,
        Weekly,
        Monthly,
        Yearly,
        SpecificWeekday,
        Hourly,
        Minutely,
        OrdinalDayOfMonth,
        OrdinalWeekdayOfMonth
    }

    public enum OrdinalFallbackMode
    {
        ExactOnly = 0,
        OrLast = 1
    }
}

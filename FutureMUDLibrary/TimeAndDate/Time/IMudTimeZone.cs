using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Time
{
    public interface IMudTimeZone : IFrameworkItem
    {
        int OffsetHours { get; }
        int OffsetMinutes { get; }
        string Description { get; }
        string Alias { get; }
        IClock Clock { get; }
    }
}
using MudSharp.Framework;

namespace MudSharp.TimeAndDate.Time
{
    public interface IMudTimeZone : IHaveMultipleNames
    {
        int OffsetHours { get; }
        int OffsetMinutes { get; }
        string Description { get; }
        string Alias { get; }
        IClock Clock { get; }
    }

    public interface IEditableMudTimeZone : IMudTimeZone
    {
	    new int OffsetHours { set; }
	    new int OffsetMinutes { set; }
	    new string Description { set; }

	}
}
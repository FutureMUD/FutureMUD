#nullable enable

namespace MudSharp.TimeAndDate.Date;

public interface IRegnalPeriod
{
	string Key { get; }
	string ShortName { get; }
	string FullName { get; }
	MudDate StartDate { get; }
	MudDate? EndDate { get; }
	bool IsOpenEnded { get; }
	bool Contains(MudDate date);
}

public sealed record RegnalDateInfo(
	IRegnalPeriod Period,
	int RegnalYear,
	MudDate Date,
	MudDate RegnalYearStart)
{
	public string CanonicalYearText => $"RY{RegnalYear} @{Period.Key}";
}

using System;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;

namespace MudSharp.Form.Audio.HearingProfiles;

/// <summary>
///     A WeekdayHearingProfile is an implementation of IHearingProfile that provides a different TemporalHearingProfile
///     for different days of the week
/// </summary>
public class WeekdayHearingProfile : HearingProfile
{
	private readonly CircularRange<IHearingProfile> _weekdayRanges = new();

	private ICalendar _calendar;

	public WeekdayHearingProfile(MudSharp.Models.HearingProfile profile)
		: base(profile)
	{
	}

	public override string FrameworkItemType => "WeekdayHearingProfile";

	public override IHearingProfile CurrentProfile(ILocation location)
	{
		return _weekdayRanges.Get(location.Date(_calendar).WeekdayIndex);
	}

	public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
	{
		return CurrentProfile(location).AudioDifficulty(location, volume, proximity);
	}

	public override void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game)
	{
		var root = XElement.Parse(profile.Definition);
		var element = root.Element("CalendarID");
		if (element != null)
		{
			_calendar = game.Calendars.Get(long.Parse(element.Value));
		}

		element = root.Element("Weekdays");
		if (element != null)
		{
			foreach (var sub in element.Elements("Weekday"))
			{
				_weekdayRanges.Add(
					new BoundRange<IHearingProfile>(
						game.HearingProfiles.Get(long.Parse(sub.Attribute("ProfileID").Value)),
						Convert.ToDouble(sub.Attribute("Lower").Value),
						Convert.ToDouble(sub.Attribute("Upper").Value)));
			}

			_weekdayRanges.Sort();
		}
	}
}
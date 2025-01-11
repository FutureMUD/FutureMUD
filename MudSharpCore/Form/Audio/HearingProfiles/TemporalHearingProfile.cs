using System;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Form.Audio.HearingProfiles;

/// <summary>
///     A TemporalHearingProfile is an implementation of IHearingProfile that provides a different HearingProfile result
///     based on the local time of day
/// </summary>
public class TemporalHearingProfile : HearingProfile
{
	private readonly CircularRange<SimpleHearingProfile> _timeRanges = new();

	private IClock _clock;

	public TemporalHearingProfile(MudSharp.Models.HearingProfile profile)
		: base(profile)
	{
	}

	public override string FrameworkItemType => "TemporalHearingProfile";

	public override IHearingProfile CurrentProfile(ILocation location)
	{
		return _timeRanges.Get(location.Time(_clock).TimeFraction);
	}

	public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
	{
		return CurrentProfile(location).AudioDifficulty(location, volume, proximity);
	}

	public override void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game)
	{
		var root = XElement.Parse(profile.Definition);
		var element = root.Element("ClockID");
		if (element != null)
		{
			_clock = game.Clocks.Get(long.Parse(element.Value));
		}

		element = root.Element("Times");
		if (element != null)
		{
			foreach (var sub in element.Elements("Time"))
			{
				_timeRanges.Add(
					new BoundRange<SimpleHearingProfile>(_timeRanges,
						(SimpleHearingProfile)
						game.HearingProfiles.Get(long.Parse(sub.Attribute("ProfileID").Value)),
						Convert.ToDouble(sub.Attribute("Lower").Value),
						Convert.ToDouble(sub.Attribute("Upper").Value)));
			}

			_timeRanges.Sort();
		}
	}
}
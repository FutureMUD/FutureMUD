using System;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Audio.HearingProfiles;

public abstract class HearingProfile : FrameworkItem, IHearingProfile
{
	protected HearingProfile(MudSharp.Models.HearingProfile profile)
	{
		_id = profile.Id;
		_name = profile.Name;
		SurveyDescription = profile.SurveyDescription;
	}

	public abstract void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game);

	public static HearingProfile LoadProfile(MudSharp.Models.HearingProfile profile)
	{
		switch (profile.Type)
		{
			case "Simple":
				return new SimpleHearingProfile(profile);
			case "Temporal":
				return new TemporalHearingProfile(profile);
			case "Weekday":
				return new WeekdayHearingProfile(profile);
			default:
				throw new NotSupportedException("Invalid HearingProfile type in HearingProfile.LoadProfile");
		}
	}

	#region IHearingProfile Members

	public abstract Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity);

	public string SurveyDescription { get; set; }

	public virtual IHearingProfile CurrentProfile(ILocation location)
	{
		return this;
	}

	#endregion
}
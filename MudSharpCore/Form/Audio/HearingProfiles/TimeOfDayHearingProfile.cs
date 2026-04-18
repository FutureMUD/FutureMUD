#nullable enable

using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Form.Audio.HearingProfiles;

/// <summary>
/// 	A TimeOfDayHearingProfile is an implementation of IHearingProfile that provides a different SimpleHearingProfile
/// 	result based on the local time of day enum for the location.
/// </summary>
public class TimeOfDayHearingProfile : HearingProfile
{
	private readonly Dictionary<TimeOfDay, SimpleHearingProfile> _timeOfDayProfiles = new();

	public TimeOfDayHearingProfile(MudSharp.Models.HearingProfile profile, IFuturemud game)
		: base(profile, game)
	{
	}

	public TimeOfDayHearingProfile(IFuturemud game, string name)
		: base(game, name, "TimeOfDay")
	{
	}

	private TimeOfDayHearingProfile(TimeOfDayHearingProfile rhs, string name)
		: base(rhs.Gameworld, name, rhs.Type)
	{
		CopyBaseSettingsFrom(rhs);
		foreach (var item in rhs._timeOfDayProfiles)
		{
			_timeOfDayProfiles[item.Key] = item.Value;
		}

		Changed = true;
	}

	public override string FrameworkItemType => "TimeOfDayHearingProfile";

	public override string Type => "TimeOfDay";

	private static TimeOfDay GetCurrentTimeOfDay(ILocation location)
	{
		return location switch
		{
			ICell cell => cell.CurrentTimeOfDay,
			IRoom room => room.CurrentTimeOfDay,
			IZone zone => zone.CurrentTimeOfDay,
			IArea area => area.CurrentTimeOfDay,
			_ => TimeOfDay.Night
		};
	}

	private SimpleHearingProfile? GetCurrentProfile(ILocation location)
	{
		return _timeOfDayProfiles.GetValueOrDefault(GetCurrentTimeOfDay(location));
	}

	public override IHearingProfile CurrentProfile(ILocation location)
	{
		var profile = GetCurrentProfile(location);
		return profile is not null ? profile : this;
	}

	public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
	{
		return GetCurrentProfile(location)?.AudioDifficulty(location, volume, proximity) ?? Difficulty.Automatic;
	}

	public override HearingProfile Clone(string name)
	{
		return new TimeOfDayHearingProfile(this, name);
	}

	public override bool DependsOn(IHearingProfile profile)
	{
		return base.DependsOn(profile) || _timeOfDayProfiles.Values.Any(x => x.DependsOn(profile));
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
				new XElement("TimeOfDays",
					from item in _timeOfDayProfiles.OrderBy(x => x.Key)
					select new XElement("TimeOfDay",
						new XAttribute("Value", (int)item.Key),
						new XAttribute("ProfileID", item.Value.Id))))
			.ToString();
	}

	public override string HelpText => base.HelpText + @"

	#3timeofday set <timeofday> <profile>#0 - sets the profile used for a specific time of day
	#3timeofday clear <timeofday>#0 - clears the profile used for a specific time of day";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "timeofday":
			case "time":
				return BuildingCommandTimeOfDay(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandTimeOfDay(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "set":
			case "add":
				return BuildingCommandTimeOfDaySet(actor, command);
			case "clear":
			case "remove":
				return BuildingCommandTimeOfDayClear(actor, command);
			default:
				actor.OutputHandler.Send("You must specify set or clear.");
				return false;
		}
	}

	private bool TryGetTimeOfDay(ICharacter actor, StringStack command, out TimeOfDay timeOfDay)
	{
		timeOfDay = default;
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which time of day do you want to use? Valid options are {Enum.GetValues<TimeOfDay>().Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		if (!command.PopSpeech().TryParseEnum<TimeOfDay>(out timeOfDay))
		{
			actor.OutputHandler.Send(
				$"That is not a valid time of day. Valid options are {Enum.GetValues<TimeOfDay>().Select(x => x.DescribeColour()).ListToString()}.");
			return false;
		}

		return true;
	}

	private bool BuildingCommandTimeOfDaySet(ICharacter actor, StringStack command)
	{
		if (!TryGetTimeOfDay(actor, command, out var timeOfDay))
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which simple hearing profile should be used for that time of day?");
			return false;
		}

		var profile = actor.Gameworld.HearingProfiles.GetByIdOrName(command.SafeRemainingArgument) as SimpleHearingProfile;
		if (profile is null)
		{
			actor.OutputHandler.Send("You must specify a simple hearing profile.");
			return false;
		}

		_timeOfDayProfiles[timeOfDay] = profile;
		Changed = true;
		actor.OutputHandler.Send(
			$"This profile now uses {profile.Name.ColourName()} during {timeOfDay.DescribeColour()}.");
		return true;
	}

	private bool BuildingCommandTimeOfDayClear(ICharacter actor, StringStack command)
	{
		if (!TryGetTimeOfDay(actor, command, out var timeOfDay))
		{
			return false;
		}

		if (_timeOfDayProfiles.Remove(timeOfDay))
		{
			Changed = true;
			actor.OutputHandler.Send($"This profile no longer has a specific mapping for {timeOfDay.DescribeColour()}.");
			return true;
		}

		actor.OutputHandler.Send($"There was no specific mapping for {timeOfDay.DescribeColour()}.");
		return false;
	}

	public override string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.Append(base.Show(actor));
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from time in Enum.GetValues<TimeOfDay>()
			select new List<string>
			{
				time.DescribeColour(),
				_timeOfDayProfiles.TryGetValue(time, out var profile) ? profile.Name.ColourName() : "None".ColourError()
			},
			new List<string>
			{
				"Time Of Day",
				"Profile"
			},
			actor,
			Telnet.Green
		));
		return sb.ToString();
	}

	public override void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game)
	{
		var root = XElement.Parse(profile.Definition);
		var element = root.Element("TimeOfDays");
		if (element is null)
		{
			return;
		}

		foreach (var sub in element.Elements("TimeOfDay"))
		{
			var profileIdText = sub.Attribute("ProfileID")?.Value;
			var timeOfDayText = sub.Attribute("Value")?.Value ?? sub.Attribute("TimeOfDay")?.Value;
			if (!long.TryParse(profileIdText, out var profileId) || !int.TryParse(timeOfDayText, out var value))
			{
				continue;
			}

			var loadedProfile = game.HearingProfiles.Get(profileId) as SimpleHearingProfile;
			if (loadedProfile is null)
			{
				continue;
			}

			_timeOfDayProfiles[(TimeOfDay)value] = loadedProfile;
		}
	}
}

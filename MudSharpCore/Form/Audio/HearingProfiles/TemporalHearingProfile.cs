using System;
using System.Xml.Linq;
using System.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Character;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Time;
using System.Text;

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

        public TemporalHearingProfile(IFuturemud game, string name)
                : base(game, name, "Temporal")
        {
        }

        public override string FrameworkItemType => "TemporalHearingProfile";

        public override string Type => "Temporal";

	public override IHearingProfile CurrentProfile(ILocation location)
	{
		return _timeRanges.Get(location.Time(_clock).TimeFraction);
	}

        public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
        {
                return CurrentProfile(location).AudioDifficulty(location, volume, proximity);
        }

        protected override string SaveDefinition()
        {
                return new XElement("Definition",
                                new XElement("ClockID", _clock?.Id ?? 0),
                                new XElement("Times",
                                        from range in _timeRanges.Ranges
                                        select new XElement("Time",
                                                new XAttribute("ProfileID", range.Value.Id),
                                                new XAttribute("Lower", range.LowerLimit),
                                                new XAttribute("Upper", range.UpperLimit))))
                        .ToString();
        }

        public override string HelpText => base.HelpText + @"

        #3clock <which>#0 - sets the clock used
        #3time add <lower> <upper> <profile>#0 - adds a time range
        #3time remove <##>#0 - removes a time range";

        public override bool BuildingCommand(ICharacter actor, StringStack command)
        {
                switch (command.PopForSwitch())
                {
                        case "clock":
                                return BuildingCommandClock(actor, command);
                        case "time":
                                return BuildingCommandTime(actor, command);
                        default:
                                return base.BuildingCommand(actor, command.GetUndo());
                }
        }

        private bool BuildingCommandClock(ICharacter actor, StringStack command)
        {
                if (command.IsFinished)
                {
                        actor.OutputHandler.Send("Which clock should this profile use?");
                        return false;
                }

                var clock = actor.Gameworld.Clocks.GetByIdOrName(command.SafeRemainingArgument);
                if (clock == null)
                {
                        actor.OutputHandler.Send("There is no such clock.");
                        return false;
                }

                _clock = clock;
                Changed = true;
                actor.OutputHandler.Send($"This profile now uses the {_clock.Name.ColourValue()} clock.");
                return true;
        }

        private bool BuildingCommandTime(ICharacter actor, StringStack command)
        {
                var action = command.PopForSwitch();
                switch (action)
                {
                        case "add":
                                return BuildingCommandTimeAdd(actor, command);
                        case "remove":
                                return BuildingCommandTimeRemove(actor, command);
                        default:
                                actor.OutputHandler.Send("You must specify add or remove.");
                                return false;
                }
        }

        private bool BuildingCommandTimeAdd(ICharacter actor, StringStack command)
        {
                if (command.CountRemainingArguments() < 3)
                {
                        actor.OutputHandler.Send("You must specify a lower bound, upper bound and profile.");
                        return false;
                }

                if (!double.TryParse(command.PopSpeech(), out var lower))
                {
                        actor.OutputHandler.Send("Invalid lower bound.");
                        return false;
                }

                if (!double.TryParse(command.PopSpeech(), out var upper))
                {
                        actor.OutputHandler.Send("Invalid upper bound.");
                        return false;
                }

                var profile = actor.Gameworld.HearingProfiles.GetByIdOrName(command.SafeRemainingArgument) as SimpleHearingProfile;
                if (profile == null)
                {
                        actor.OutputHandler.Send("You must specify a simple hearing profile.");
                        return false;
                }

                _timeRanges.Add(new BoundRange<SimpleHearingProfile>(_timeRanges, profile, lower, upper));
                _timeRanges.Sort();
                Changed = true;
                actor.OutputHandler.Send("Time range added.");
                return true;
        }

        private bool BuildingCommandTimeRemove(ICharacter actor, StringStack command)
        {
                actor.OutputHandler.Send("Removing time ranges is not currently supported.");
                return false;
        }

        public override string Show(ICharacter actor)
        {
                var sb = new StringBuilder();
                sb.Append(base.Show(actor));
                sb.AppendLine($"Clock: {_clock?.Name ?? "None"}");
                var i = 1;
                foreach (var range in _timeRanges.Ranges)
                {
                        sb.AppendLine($"{i++,3}. {range.LowerLimit:F2}-{range.UpperLimit:F2} -> {range.Value.Name.ColourValue()}");
                }
                return sb.ToString();
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
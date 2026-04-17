using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Form.Audio.HearingProfiles;

/// <summary>
///     A TemporalHearingProfile is an implementation of IHearingProfile that provides a different HearingProfile result
///     based on the local time of day
/// </summary>
public class TemporalHearingProfile : HearingProfile
{
    private readonly CircularRange<SimpleHearingProfile> _timeRanges = new();

    private IClock _clock;

    public TemporalHearingProfile(MudSharp.Models.HearingProfile profile, IFuturemud game)
            : base(profile, game)
    {
    }

    public TemporalHearingProfile(IFuturemud game, string name)
            : base(game, name, "Temporal")
    {
    }

    private TemporalHearingProfile(TemporalHearingProfile rhs, string name)
            : base(rhs.Gameworld, name, rhs.Type)
    {
        CopyBaseSettingsFrom(rhs);
        _clock = rhs._clock;
        foreach (BoundRange<SimpleHearingProfile> range in rhs._timeRanges.Ranges)
        {
            _timeRanges.Add(new BoundRange<SimpleHearingProfile>(_timeRanges, range.Value, range.LowerLimit,
                range.UpperLimit));
        }

        if (_timeRanges.Ranges.Any())
        {
            _timeRanges.Sort();
        }

        Changed = true;
    }

    public override string FrameworkItemType => "TemporalHearingProfile";

    public override string Type => "Temporal";

    public override IHearingProfile CurrentProfile(ILocation location)
    {
        if (_clock is null || !_timeRanges.Ranges.Any())
        {
            return this;
        }

        return _timeRanges.Get(location.Time(_clock)?.TimeFraction ?? 0.0);
    }

    public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
    {
        if (_clock is null || !_timeRanges.Ranges.Any())
        {
            return Difficulty.Automatic;
        }

        return CurrentProfile(location).AudioDifficulty(location, volume, proximity);
    }

    public override HearingProfile Clone(string name)
    {
        return new TemporalHearingProfile(this, name);
    }

    public override bool DependsOn(IHearingProfile profile)
    {
        return base.DependsOn(profile) || _timeRanges.Ranges.Any(x => x.Value.DependsOn(profile));
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
	#3time add <lower> <upper> <profile>#0 - adds a time range using fractions of a day from 0.0 to 1.0
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

        IClock clock = actor.Gameworld.Clocks.GetByIdOrName(command.SafeRemainingArgument);
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
        string action = command.PopForSwitch();
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

        if (!double.TryParse(command.PopSpeech(), out double lower))
        {
            actor.OutputHandler.Send("Invalid lower bound.");
            return false;
        }

        if (!double.TryParse(command.PopSpeech(), out double upper))
        {
            actor.OutputHandler.Send("Invalid upper bound.");
            return false;
        }

        if (lower < 0.0 || lower > 1.0 || upper < 0.0 || upper > 1.0)
        {
            actor.OutputHandler.Send("Time bounds must both be between 0.0 and 1.0.");
            return false;
        }

        if (Math.Abs(lower - upper) < 0.000001)
        {
            actor.OutputHandler.Send("The lower and upper bounds must not be the same.");
            return false;
        }

        SimpleHearingProfile profile = actor.Gameworld.HearingProfiles.GetByIdOrName(command.SafeRemainingArgument) as SimpleHearingProfile;
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
        if (command.IsFinished || !int.TryParse(command.PopSpeech(), out int index))
        {
            actor.OutputHandler.Send("Which numbered time range do you want to remove?");
            return false;
        }

        List<BoundRange<SimpleHearingProfile>> ranges = _timeRanges.Ranges.ToList();
        if (index < 1 || index > ranges.Count)
        {
            actor.OutputHandler.Send("There is no such numbered time range.");
            return false;
        }

        BoundRange<SimpleHearingProfile> range = ranges[index - 1];
        _timeRanges.RemoveAt(index - 1);
        Changed = true;
        actor.OutputHandler.Send(
            $"You remove time range #{index.ToString("N0", actor)}, from {range.LowerLimit.ToString("N2", actor).ColourValue()} to {range.UpperLimit.ToString("N2", actor).ColourValue()}, which pointed at {range.Value.Name.ColourName()}.");
        return true;
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.Append(base.Show(actor));
        sb.AppendLine();
        sb.AppendLine($"Clock: {_clock?.Name.ColourName() ?? "None".ColourError()}");
        sb.AppendLine();
        if (_timeRanges.Ranges.Any())
        {
            sb.AppendLine(StringUtilities.GetTextTable(
                _timeRanges.Ranges.Select((range, index) => new List<string>
                {
                    (index + 1).ToString("N0", actor),
                    range.LowerLimit.ToString("N2", actor),
                    range.UpperLimit.ToString("N2", actor),
                    range.Value.Name.ColourName()
                }),
                new List<string>
                {
                    "#",
                    "Lower",
                    "Upper",
                    "Profile"
                },
                actor,
                Telnet.Green
            ));
        }
        else
        {
            sb.AppendLine("No time ranges are configured.");
        }

        return sb.ToString();
    }

    public override void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game)
    {
        XElement root = XElement.Parse(profile.Definition);
        XElement element = root.Element("ClockID");
        if (element != null)
        {
            _clock = game.Clocks.Get(long.Parse(element.Value));
        }

        element = root.Element("Times");
        if (element != null)
        {
            foreach (XElement sub in element.Elements("Time"))
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

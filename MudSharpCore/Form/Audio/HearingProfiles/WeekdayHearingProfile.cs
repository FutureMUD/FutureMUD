using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Form.Audio.HearingProfiles;

/// <summary>
///     A WeekdayHearingProfile is an implementation of IHearingProfile that provides a different TemporalHearingProfile
///     for different days of the week
/// </summary>
public class WeekdayHearingProfile : HearingProfile
{
    private readonly CircularRange<IHearingProfile> _weekdayRanges = new();

    private ICalendar _calendar;

    public WeekdayHearingProfile(MudSharp.Models.HearingProfile profile, IFuturemud game)
            : base(profile, game)
    {
    }

    public WeekdayHearingProfile(IFuturemud game, string name)
            : base(game, name, "Weekday")
    {
    }

    private WeekdayHearingProfile(WeekdayHearingProfile rhs, string name)
            : base(rhs.Gameworld, name, rhs.Type)
    {
        CopyBaseSettingsFrom(rhs);
        _calendar = rhs._calendar;
        foreach (BoundRange<IHearingProfile> range in rhs._weekdayRanges.Ranges)
        {
            _weekdayRanges.Add(new BoundRange<IHearingProfile>(_weekdayRanges, range.Value, range.LowerLimit,
                range.UpperLimit));
        }

        if (_weekdayRanges.Ranges.Any())
        {
            _weekdayRanges.Sort();
        }

        Changed = true;
    }

    public override string FrameworkItemType => "WeekdayHearingProfile";

    public override string Type => "Weekday";

    public override IHearingProfile CurrentProfile(ILocation location)
    {
        if (_calendar is null || !_weekdayRanges.Ranges.Any())
        {
            return this;
        }

        return _weekdayRanges.Get(location.Date(_calendar)?.WeekdayIndex ?? 0);
    }

    public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
    {
        if (_calendar is null || !_weekdayRanges.Ranges.Any())
        {
            return Difficulty.Automatic;
        }

        return CurrentProfile(location).AudioDifficulty(location, volume, proximity);
    }

    public override HearingProfile Clone(string name)
    {
        return new WeekdayHearingProfile(this, name);
    }

    public override bool DependsOn(IHearingProfile profile)
    {
        return base.DependsOn(profile) ||
               _weekdayRanges.Ranges.Any(x => x.Value is HearingProfile hearingProfile && hearingProfile.DependsOn(profile));
    }

    protected override string SaveDefinition()
    {
        return new XElement("Definition",
                        new XElement("CalendarID", _calendar?.Id ?? 0),
                        new XElement("Weekdays",
                                from range in _weekdayRanges.Ranges
                                select new XElement("Weekday",
                                        new XAttribute("ProfileID", range.Value.Id),
                                        new XAttribute("Lower", range.LowerLimit),
                                        new XAttribute("Upper", range.UpperLimit))))
                .ToString();
    }

    public override string HelpText => base.HelpText + @"

	#3calendar <which>#0 - sets the calendar used
	#3weekday add <lower> <upper> <profile>#0 - adds a weekday range using weekday indexes from the selected calendar
	#3weekday remove <##>#0 - removes a weekday range";

    public override bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "calendar":
                return BuildingCommandCalendar(actor, command);
            case "weekday":
                return BuildingCommandWeekday(actor, command);
            default:
                return base.BuildingCommand(actor, command.GetUndo());
        }
    }

    private bool BuildingCommandCalendar(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which calendar should this profile use?");
            return false;
        }

        ICalendar cal = actor.Gameworld.Calendars.GetByIdOrName(command.SafeRemainingArgument);
        if (cal == null)
        {
            actor.OutputHandler.Send("There is no such calendar.");
            return false;
        }

        _calendar = cal;
        Changed = true;
        actor.OutputHandler.Send($"This profile now uses the {_calendar.Name.ColourValue()} calendar.");
        return true;
    }

    private bool BuildingCommandWeekday(ICharacter actor, StringStack command)
    {
        string action = command.PopForSwitch();
        switch (action)
        {
            case "add":
                return BuildingCommandWeekdayAdd(actor, command);
            case "remove":
                return BuildingCommandWeekdayRemove(actor, command);
            default:
                actor.OutputHandler.Send("You must specify add or remove.");
                return false;
        }
    }

    private bool BuildingCommandWeekdayAdd(ICharacter actor, StringStack command)
    {
        if (_calendar is null)
        {
            actor.OutputHandler.Send("You must set a calendar before you can add weekday ranges.");
            return false;
        }

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

        if (lower < 0.0 || lower > _calendar.Weekdays.Count || upper < 0.0 || upper > _calendar.Weekdays.Count)
        {
            actor.OutputHandler.Send(
                $"Weekday bounds must both be between 0 and {_calendar.Weekdays.Count.ToString("N0", actor)}.");
            return false;
        }

        if (Math.Abs(lower - upper) < 0.000001)
        {
            actor.OutputHandler.Send("The lower and upper bounds must not be the same.");
            return false;
        }

        HearingProfile profile = actor.Gameworld.HearingProfiles.GetByIdOrName(command.SafeRemainingArgument) as HearingProfile;
        if (profile == null)
        {
            actor.OutputHandler.Send("There is no such hearing profile.");
            return false;
        }

        if (profile.DependsOn(this))
        {
            actor.OutputHandler.Send("You cannot create a cyclical hearing profile reference.");
            return false;
        }

        _weekdayRanges.Add(new BoundRange<IHearingProfile>(_weekdayRanges, profile, lower, upper));
        _weekdayRanges.Sort();
        Changed = true;
        actor.OutputHandler.Send("Weekday range added.");
        return true;
    }

    private bool BuildingCommandWeekdayRemove(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.PopSpeech(), out int index))
        {
            actor.OutputHandler.Send("Which numbered weekday range do you want to remove?");
            return false;
        }

        List<BoundRange<IHearingProfile>> ranges = _weekdayRanges.Ranges.ToList();
        if (index < 1 || index > ranges.Count)
        {
            actor.OutputHandler.Send("There is no such numbered weekday range.");
            return false;
        }

        BoundRange<IHearingProfile> range = ranges[index - 1];
        _weekdayRanges.RemoveAt(index - 1);
        Changed = true;
        actor.OutputHandler.Send(
            $"You remove weekday range #{index.ToString("N0", actor)}, from {range.LowerLimit.ToString("N2", actor).ColourValue()} to {range.UpperLimit.ToString("N2", actor).ColourValue()}, which pointed at {range.Value.Name.ColourName()}.");
        return true;
    }

    public override string Show(ICharacter actor)
    {
        StringBuilder sb = new();
        sb.Append(base.Show(actor));
        sb.AppendLine();
        sb.AppendLine($"Calendar: {_calendar?.Name.ColourName() ?? "None".ColourError()}");
        sb.AppendLine();
        if (_weekdayRanges.Ranges.Any())
        {
            sb.AppendLine(StringUtilities.GetTextTable(
                _weekdayRanges.Ranges.Select((range, index) => new List<string>
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
            sb.AppendLine("No weekday ranges are configured.");
        }

        return sb.ToString();
    }

    public override void Initialise(MudSharp.Models.HearingProfile profile, IFuturemud game)
    {
        XElement root = XElement.Parse(profile.Definition);
        XElement element = root.Element("CalendarID");
        if (element != null)
        {
            _calendar = game.Calendars.Get(long.Parse(element.Value));
        }

        element = root.Element("Weekdays");
        if (element != null)
        {
            foreach (XElement sub in element.Elements("Weekday"))
            {
                _weekdayRanges.Add(
                    new BoundRange<IHearingProfile>(_weekdayRanges,
                        game.HearingProfiles.Get(long.Parse(sub.Attribute("ProfileID").Value)),
                        Convert.ToDouble(sub.Attribute("Lower").Value),
                        Convert.ToDouble(sub.Attribute("Upper").Value)));
            }

            _weekdayRanges.Sort();
        }
    }
}

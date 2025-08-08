using System;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Character;
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

        public WeekdayHearingProfile(IFuturemud game, string name)
                : base(game, name, "Weekday")
        {
        }

        public override string FrameworkItemType => "WeekdayHearingProfile";

        public override string Type => "Weekday";

	public override IHearingProfile CurrentProfile(ILocation location)
	{
		return _weekdayRanges.Get(location.Date(_calendar).WeekdayIndex);
	}

        public override Difficulty AudioDifficulty(ILocation location, AudioVolume volume, Proximity proximity)
        {
                return CurrentProfile(location).AudioDifficulty(location, volume, proximity);
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
        #3weekday add <lower> <upper> <profile>#0 - adds a weekday range
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

                var cal = actor.Gameworld.Calendars.GetByIdOrName(command.SafeRemainingArgument);
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
                var action = command.PopForSwitch();
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

                var profile = actor.Gameworld.HearingProfiles.GetByIdOrName(command.SafeRemainingArgument);
                if (profile == null)
                {
                        actor.OutputHandler.Send("There is no such hearing profile.");
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
                actor.OutputHandler.Send("Removing weekday ranges is not currently supported.");
                return false;
        }

        public override string Show(ICharacter actor)
        {
                var sb = new StringBuilder();
                sb.Append(base.Show(actor));
                sb.AppendLine($"Calendar: {_calendar?.Name ?? "None"}");
                var i = 1;
                foreach (var range in _weekdayRanges.Ranges)
                {
                        sb.AppendLine($"{i++,3}. {range.LowerLimit}-{range.UpperLimit} -> {range.Value.Name.ColourValue()}");
                }
                return sb.ToString();
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
					new BoundRange<IHearingProfile>(_weekdayRanges,
						game.HearingProfiles.Get(long.Parse(sub.Attribute("ProfileID").Value)),
						Convert.ToDouble(sub.Attribute("Lower").Value),
						Convert.ToDouble(sub.Attribute("Upper").Value)));
			}

			_weekdayRanges.Sort();
		}
	}
}
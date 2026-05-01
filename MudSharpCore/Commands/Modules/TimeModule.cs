using JetBrains.Annotations;
using MudSharp.Accounts;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Commands.Helpers;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Commands.Modules;

internal class TimeModule : Module<ICharacter>
{
    private TimeModule()
        : base("Time")
    {
        IsNecessary = true;
    }

    public static TimeModule Instance { get; } = new();

    [PlayerCommand("Time", "time")]
    [CustomModuleName("World")]
    [RequiredCharacterState(CharacterState.Conscious)]
    protected static void Time(ICharacter actor, string input)
    {
        StringBuilder sb = new();
        sb.AppendLine("You know the following things about time:");
        sb.AppendLine();
        sb.AppendLine($"\tIt is currently {actor.Location.CurrentTimeOfDay.DescribeColour()}.");
        ISeason season = actor.Location.CurrentSeason(actor);
        if (season is not null)
        {
            sb.AppendLine($"\tIt is currently {season.DisplayName.ColourValue()}.");
        }

        IEnumerable<ICelestialObject> celestials = actor.Location.Celestials;
        foreach (CelestialInformation info in celestials.Select(x => actor.Location.GetInfo(x)))
        {
            if (actor.Body.CanSee(info.Origin))
            {
                string description = info.Origin.Describe(info).Fullstop().Wrap(actor.InnerLineFormatLength, "\t");
                if (actor.IsAdministrator())
                {
                    description += $" (Asc: {$"{info.LastAscensionAngle.RadiansToDegrees().ToString("N3", actor)}°".ColourValue()} Azi: {$"{info.LastAzimuthAngle.RadiansToDegrees().ToString("N3", actor)}°".ColourValue()} DN: {info.Origin.CurrentCelestialDay.ToString("N2", actor).ColourValue()})";
                }

                sb.AppendLine(description);
            }
        }

        IEnumerable<ICalendar> calendars = actor.Body.Location.Calendars;
        if (actor.Gameworld.GetCheck(CheckType.ExactTimeCheck).Check(actor, Difficulty.Automatic).IsPass())
        {
            foreach (ICalendar calendar in calendars)
            {
                MudDate date = actor.Location.Date(calendar);
                MudTime time = actor.Location.Time(calendar.FeedClock);
                sb.AppendLine($"It is {calendar.FeedClock.DisplayTime(time, TimeDisplayTypes.Long).ColourValue()} on {calendar.DisplayDate(date, CalendarDisplayMode.Long).ColourValue().Fullstop()}".Wrap(actor.InnerLineFormatLength, "\t"));
            }
        }
        else if (actor.Gameworld.GetCheck(CheckType.VagueTimeCheck).Check(actor, Difficulty.Automatic).IsPass())
        {
            foreach (ICalendar calendar in calendars)
            {
                MudDate date = actor.Location.Date(calendar);
                MudTime time = actor.Location.Time(calendar.FeedClock);
                sb.AppendLine($"It is about {calendar.FeedClock.DisplayTime(time, TimeDisplayTypes.Vague).ColourValue()} on {calendar.DisplayDate(date, CalendarDisplayMode.Long).ColourValue().Fullstop()}".Wrap(actor.InnerLineFormatLength, "\t"));
            }
        }
        else
        {
            foreach (ICalendar calendar in calendars)
            {
                MudDate date = actor.Location.Date(calendar);
                MudTime time = actor.Location.Time(calendar.FeedClock);
                sb.AppendLine($"It is {calendar.FeedClock.DisplayTime(time, TimeDisplayTypes.Crude).ColourValue()} on {calendar.DisplayDate(date, CalendarDisplayMode.Long).ColourValue().Fullstop()}".Wrap(actor.InnerLineFormatLength, "\t"));
            }
        }

        List<ITimePiece> visibleTimepieces = actor.Body.ExternalItems.Concat(actor.Location.LayerGameItems(actor.RoomLayer))
                                     .Where(x => actor.CanSee(x)).SelectNotNull(x => x.GetItemType<ITimePiece>())
                                     .ToList();
        foreach (ITimePiece item in visibleTimepieces)
        {
            sb.AppendLine(
                $"\t{item.Parent.HowSeen(actor, true)} shows the time as {item.Clock.DisplayTime(item.CurrentTime, item.TimeDisplayString).ColourValue()}.".Wrap(actor.InnerLineFormatLength, "\t"));
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    #region Calendars
    public const string CalendarAdminHelpText = @"This command is used to create and edit in-game calendars.

The syntax for this command is as follows:

#3calendar list#0 - lists all calendars
#3calendar new <alias> ""<short name>"" <clock> [""<full name>""]#0 - creates a scaffold calendar
#3calendar clone <old> <alias> ""<short name>"" [""<full name>""]#0 - clones an existing calendar
#3calendar edit <which>#0 - begins editing a calendar
#3calendar close#0 - stops editing a calendar
#3calendar show [<which>]#0 - shows calendar details
#3calendar set alias|shortname|fullname|desc|plane|clock|date|epoch ...#0 - edits metadata
#3calendar set weekday add|rename|remove ...#0 - edits weekdays
#3calendar set month add|rename|alias|short|days|order|remove|nonweekday|special ...#0 - edits months
#3calendar set intercalary day|month add|remove ...#0 - edits intercalary rules
#3calendar set preview [year]#0 - previews generated months for a year
#3calendar set validate#0 - validates calendar generation";

    public static void DisplayCalendarToCharacter(ICharacter actor, ICalendar calendar, [CanBeNull] Year year)
    {
        StringBuilder sb = new();
        if (year is not null)
        {
            sb.AppendLine(
                $"Calendar: {calendar.FullName.Colour(Telnet.Cyan)} for year {year.YearName.ToString("F0", actor).ColourValue()}");
        }
        else
        {
            sb.AppendLine($"Calendar: {calendar.FullName.Colour(Telnet.Cyan)}");
        }

        sb.AppendLine($"Ancient Epoch: {calendar.AncientEraLongString.Colour(Telnet.Green)}");
        sb.AppendLine($"Modern Epoch: {calendar.ModernEraLongString.Colour(Telnet.Green)}");
        sb.AppendLine($"Weekdays: {calendar.Weekdays.Select(x => x.Colour(Telnet.Green)).ListToString()}");
        sb.AppendLine(
            $"Days in Normal Year: {(calendar.Months.Sum(x => x.NormalDays) + calendar.Intercalaries.Where(x => x.Rule.DivisibleBy == 1).Sum(x => x.Month.NormalDays)).ToString("N0", actor).Colour(Telnet.Green)}");
        if (year is not null)
        {
            sb.AppendLine(
                $"Days in Year {year.YearName.ToString("F0", actor)}: {year.Months.Sum(x => x.Days).ToString("N0", actor).Colour(Telnet.Green)}");
        }

        sb.AppendLine();
        sb.AppendLine("Months:");
        if (year is not null)
        {
            sb.AppendLine(StringUtilities.GetTextTable(
                from month in year.Months
                select
                    new[]
                    {
                        month.NominalOrder.ToString("N0", actor),
                        month.FullName,
                        month.ShortName,
                        month.Days.ToString("N0", actor),
                        month.DayNames.Select(x => $"{x.Value.FullName.ColourValue()} ({x.Key.ToOrdinal()})")
                             .ListToCommaSeparatedValues(", ")
                    },
                new[] { "#", "Name", "Abbr", "Days", "Special Days" },
                actor,
                Telnet.Green
            ));
        }
        else
        {
            sb.AppendLine(StringUtilities.GetTextTable(
                from month in calendar.Months
                select
                    new[]
                    {
                        month.NominalOrder.ToString("N0", actor),
                        month.FullName,
                        month.ShortName,
                        month.NormalDays.ToString("N0", actor),
                        month.SpecialDayNames.Select(x => $"{x.Value.FullName.ColourValue()} ({x.Key.ToOrdinal()})")
                             .ListToCommaSeparatedValues(", ")
                    },
                new[] { "#", "Name", "Abbr", "Days", "Special Days" },
                actor,
                Telnet.Green
            ));
            sb.AppendLine("Intercalary Months:");
            sb.AppendLine(StringUtilities.GetTextTable(
                from month in calendar.Intercalaries
                select
                    new[]
                    {
                        month.Month.FullName,
                        month.Month.ShortName,
                        month.Month.NormalDays.ToString("N0", actor),
                        calendar.Months.FirstOrDefault(x => x.NominalOrder == month.Month.NominalOrder - 1)?.FullName ??
                        "None",
                        month.Month.SpecialDayNames
                             .Select(x => $"{x.Value.FullName.ColourValue()} ({x.Key.ToOrdinal()})")
                             .ListToCommaSeparatedValues(", "),
                        month.Rule.ToString()
                    },
                new[] { "Name", "Abbr", "Days", "After", "Special Days", "Rule" },
                actor,
                Telnet.Green
            ));
        }

        actor.Send(sb.ToString());
    }

    private const string CalendarPlayerHelpText = @"The calendar command allows you to view information about the in-game calendar for your location. 

The syntax is either #3calendar#0 to view information about the calendar in general terms, or #3calendar <year>#0 to view the calendar for a specific year.";

    protected static void PlayerCalendar(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());
        ICalendar calendar = actor.Location.Calendars.First();
        if (ss.IsFinished)
        {
            DisplayCalendarToCharacter(actor, calendar, null);
            return;
        }

        if (!int.TryParse(ss.PopSpeech(), out int year))
        {
            actor.Send(
                "You must either specify nothing to view the calendar definition, or specify a year for which to see the calendar.");
            return;
        }

        Year trueYear = calendar.CreateYear(year);
        DisplayCalendarToCharacter(actor, calendar, trueYear);
    }


    [PlayerCommand("Calendar", "calendar")]
    [CustomModuleName("World")]
    [HelpInfo("calendar", CalendarPlayerHelpText, AutoHelp.HelpArg, CalendarAdminHelpText)]
    protected static void Calendar(ICharacter actor, string input)
    {
        if (!actor.IsAdministrator(PermissionLevel.HighAdmin))
        {
            PlayerCalendar(actor, input);
            return;
        }

        BaseBuilderModule.GenericBuildingCommand(actor, new StringStack(input.RemoveFirstWord()), EditableItemHelper.CalendarHelper);
    }

    #endregion

    #region Clocks
    public const string ClockHelpText = @"This command is used to create and edit in-game clocks.

The syntax for this command is as follows:

#3clock list#0 - lists all of the clocks
#3clock new <alias> ""<name>""#0 - creates a new clock
#3clock edit <which>#0 - begins editing a clock
#3clock clone <old> <alias> ""<name>""#0 - clones a clock
#3clock close#0 - stops editing a clock
#3clock show <which>#0 - shows information about a clock
#3clock show#0 - shows information about the currently edited clock
#3clock set primary <timezone>#0 - sets the primary timezone
#3clock set time <time>#0 - sets the current clock time
#3clock set crude add <lower> <upper> <text>#0 - adds a crude time display band";

    [PlayerCommand("Clock", "clock")]
    [CommandPermission(PermissionLevel.HighAdmin)]
    [HelpInfo("clock", ClockHelpText, AutoHelp.HelpArgOrNoArg)]
    protected static void Clock(ICharacter actor, string input)
    {
        BaseBuilderModule.GenericBuildingCommand(actor, new StringStack(input.RemoveFirstWord()), EditableItemHelper.ClockHelper);
    }

    #endregion

    #region Timezones
    public const string TimeZoneHelp = @"This command is used to create and edit in-game timezones for a particular clock. Timezones are relative to a standard default timezone for that clock - a real world example would be UTC.

The syntax for this command is as follows:

	#3timezone list [<clock>]#0 - lists all of the time zones
	#3timezone create <clock> <alias> ""<name>"" <hoursoffset> [<minutesoffset>]#0 - create a new timezone
	#3timezone edit <clock> <alias> ""<name>"" <hoursoffset> [<minutesoffset>]#0 - legacy edit syntax
	#3timezone new <clock> <alias> ""<name>"" <hoursoffset> [<minutesoffset>]#0 - creates and opens a timezone
	#3timezone open <id|alias|clock:alias>#0 - opens a timezone for editing
	#3timezone set alias|name|offset|hours|minutes ...#0 - edits an opened timezone
	#3timezone clone <old> <alias> [<clock>] [""<name>""]#0 - clones a timezone";

    [PlayerCommand("Timezone", "timezone")]
    [CommandPermission(PermissionLevel.Admin)]
    [HelpInfo("timezone", TimeZoneHelp, AutoHelp.HelpArgOrNoArg)]
    protected static void Timezone(ICharacter actor, string input)
    {
        StringStack ss = new(input.RemoveFirstWord());

        switch (ss.PeekSpeech().ToLowerInvariant())
        {
            case "list":
            case "create":
            case "update":
                break;
            default:
                BaseBuilderModule.GenericBuildingCommand(actor, ss, EditableItemHelper.TimezoneHelper);
                return;
        }

        switch (ss.PopForSwitch())
        {
            case "list":
                if (!ss.IsFinished)
                {
                    var clock = actor.Gameworld.Clocks.GetByIdOrNames(ss.SafeRemainingArgument);
                    if (clock is null)
                    {
                        actor.OutputHandler.Send("There is no such clock.");
                        return;
                    }

                    actor.OutputHandler.Send(StringUtilities.GetTextTable(
                        from timezone in clock.Timezones
                        select new List<string>
                        {
                            timezone.Id.ToString("N0", actor),
                            timezone.Alias,
                            timezone.Description,
                            new TimeSpan(0, timezone.OffsetHours, timezone.OffsetMinutes, 0).Describe(),
                            (clock.PrimaryTimezone == timezone).ToColouredString()
                        },
                        new List<string> { "Id", "Alias", "Name", "Offset", "Primary?" },
                        actor.Account.LineFormatLength,
                        colour: Telnet.Green));
                    return;
                }

                BaseBuilderModule.GenericBuildingCommand(actor, new StringStack("list"), EditableItemHelper.TimezoneHelper);
                return;
            case "create":
                TimeZoneCreate(actor, ss);
                return;
            case "edit":
            case "update":
                if (ss.CountRemainingArguments() >= 4)
                {
                    TimeZoneEdit(actor, ss);
                    return;
                }

                BaseBuilderModule.GenericBuildingCommand(actor, new StringStack($"edit {ss.SafeRemainingArgument}"), EditableItemHelper.TimezoneHelper);
                return;
            default:
                actor.OutputHandler.Send(TimeZoneHelp.SubstituteANSIColour());
                return;
        }
    }

    private static void TimeZoneList(ICharacter actor, StringStack ss)
    {
        ShowModule.Show_Timezones(actor, ss);
    }

    private static void TimeZoneEdit(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.Send("Which clock do you want to edit a timezone for?");
            return;
        }

        IClock clock = actor.Gameworld.Clocks.GetByIdOrNames(ss.PopSpeech());
        if (clock is null)
        {
            actor.Send("There is no such clock.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("What is the alias of the timezone you want to edit (e.g. #6GMT#0)?".SubstituteANSIColour());
            return;
        }

        string alias = ss.PopSpeech();
        if (!clock.Timezones.Any(x => x.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.Send($"There is no timezone for the {clock.Name.ColourName()} clock with that alias.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("What name do you want to set for the timezone (e.g. #6\"Pacific Standard Time\"#0)?".SubstituteANSIColour());
            return;
        }

        string name = ss.PopSpeech().TitleCase();

        if (ss.IsFinished)
        {
            actor.Send("How many hours offset from the base should this timezone be?");
            return;
        }

        if (!int.TryParse(ss.PopSpeech(), out int hoursoffset))
        {
            actor.Send("You must enter a number of hours for this timezone to be offset.");
            return;
        }

        int minutesoffset = 0;
        if (!ss.IsFinished)
        {
            if (!int.TryParse(ss.SafeRemainingArgument, out minutesoffset))
            {
                actor.Send(
                    "You must enter a number of minutes for this timezone to be offset, or enter nothing at all.");
                return;
            }
        }

        IMudTimeZone timezone = clock.Timezones.GetByIdOrNames(alias)!;
        IEditableMudTimeZone eTimezone = (IEditableMudTimeZone)timezone;
        eTimezone.Description = name;
        eTimezone.OffsetHours = hoursoffset;
        eTimezone.OffsetMinutes = minutesoffset;
        actor.Send("You set the timezone {0} from the {1} clock to be called ({2}) and have offset {3}.",
            timezone.Alias.ColourValue(),
            clock.Name.ColourName(),
            timezone.Description.ColourName(),
            new TimeSpan(0, hoursoffset, minutesoffset, 0, 0).Describe().ColourValue()
        );
    }

    private static void TimeZoneCreate(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.Send("Which clock do you want to create a timezone for?");
            return;
        }

        IClock clock = actor.Gameworld.Clocks.GetByIdOrNames(ss.PopSpeech());
        if (clock is null)
        {
            actor.Send("There is no such clock.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("What alias do you want to give to your timezone (e.g. #6GMT#0)?".SubstituteANSIColour());
            return;
        }

        string alias = ss.PopSpeech();
        if (clock.Timezones.Any(x => x.Alias.Equals(alias, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.Send("There is already a timezone with that alias for that clock. The alias must be unique.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.Send("What name do you want to give to your timezone (e.g. #6\"Pacific Standard Time\"#0)?".SubstituteANSIColour());
            return;
        }

        string name = ss.PopSpeech().TitleCase();

        if (ss.IsFinished)
        {
            actor.Send("How many hours offset from the base should this timezone be?");
            return;
        }

        if (!int.TryParse(ss.PopSpeech(), out int hoursoffset))
        {
            actor.Send("You must enter a number of hours for this timezone to be offset.");
            return;
        }

        int minutesoffset = 0;
        if (!ss.IsFinished)
        {
            if (!int.TryParse(ss.SafeRemainingArgument, out minutesoffset))
            {
                actor.Send(
                    "You must enter a number of minutes for this timezone to be offset, or enter nothing at all.");
                return;
            }
        }

        MudTimeZone timezone = new(clock, hoursoffset, minutesoffset, name, alias);
        clock.AddTimezone(timezone);
        actor.Send("You create timezone #{0:N0} - {1} ({2}), offset {3}.",
            timezone.Id,
            timezone.Description.ColourName(),
            timezone.Alias.ColourValue(),
            new TimeSpan(0, hoursoffset, minutesoffset, 0, 0).Describe().ColourValue()
        );
    }

    #endregion
}
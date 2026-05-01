using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Commands.Helpers;

public partial class EditableItemHelper
{
    private static IMudTimeZone GetTimezoneByIdOrName(ICharacter actor, string input)
    {
        if (long.TryParse(input, out var id))
        {
            return actor.Gameworld.Clocks.SelectMany(x => x.Timezones).FirstOrDefault(x => x.Id == id);
        }

        if (input.Contains(':'))
        {
            var split = input.Split(':', 2);
            var clock = actor.Gameworld.Clocks.GetByIdOrNames(split[0]);
            return clock?.Timezones.GetByIdOrNames(split[1]);
        }

        var matches = actor.Gameworld.Clocks
                           .SelectMany(x => x.Timezones)
                           .Where(x => x.Alias.EqualTo(input) || x.Description.EqualTo(input) || x.Name.EqualTo(input))
                           .ToList();
        return matches.Count == 1 ? matches[0] : null;
    }

    public static EditableItemHelper ClockHelper { get; } = new()
    {
        ItemName = "Clock",
        ItemNamePlural = "Clocks",
        SetEditableItemAction = (actor, item) =>
        {
            actor.RemoveAllEffects<BuilderEditingEffect<IClock>>();
            if (item == null)
            {
                return;
            }

            actor.AddEffect(new BuilderEditingEffect<IClock>(actor) { EditingItem = (IClock)item });
        },
        GetEditableItemFunc = actor =>
            actor.CombinedEffectsOfType<BuilderEditingEffect<IClock>>().FirstOrDefault()?.EditingItem,
        GetAllEditableItems = actor => actor.Gameworld.Clocks.ToList(),
        GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Clocks.Get(id),
        GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Clocks.GetByIdOrNames(input),
        AddItemToGameWorldAction = item => item.Gameworld.Add((IClock)item),
        CastToType = typeof(IClock),
        EditableNewAction = (actor, input) =>
        {
            string alias = input.PopSpeech();
            if (string.IsNullOrEmpty(alias))
            {
                actor.OutputHandler.Send("You must specify an alias for the clock.");
                return;
            }

            if (actor.Gameworld.Clocks.Any(x => x.Alias.EqualTo(alias)))
            {
                actor.OutputHandler.Send($"There is already a clock with the alias {alias.ColourValue()}.");
                return;
            }

            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a name for the clock.");
                return;
            }

            string name = input.SafeRemainingArgument.TitleCase();
            Clock clock = new(actor.Gameworld, name, alias);
            actor.Gameworld.Add(clock);
            actor.RemoveAllEffects<BuilderEditingEffect<IClock>>();
            actor.AddEffect(new BuilderEditingEffect<IClock>(actor) { EditingItem = clock });
            actor.OutputHandler.Send($"You create clock #{clock.Id.ToStringN0(actor)} ({clock.Name.ColourName()}), which you are now editing.");
        },
        EditableCloneAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("Which clock do you want to clone?");
                return;
            }

            IClock original = actor.Gameworld.Clocks.GetByIdOrNames(input.PopSpeech());
            if (original is null)
            {
                actor.OutputHandler.Send("There is no such clock to clone.");
                return;
            }

            string alias = input.PopSpeech();
            if (string.IsNullOrEmpty(alias))
            {
                actor.OutputHandler.Send("You must specify an alias for the cloned clock.");
                return;
            }

            if (actor.Gameworld.Clocks.Any(x => x.Alias.EqualTo(alias)))
            {
                actor.OutputHandler.Send($"There is already a clock with the alias {alias.ColourValue()}.");
                return;
            }

            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a name for the cloned clock.");
                return;
            }

            string name = input.SafeRemainingArgument.TitleCase();
            IClock clone = original.Clone(name, alias);
            actor.Gameworld.Add(clone);
            actor.RemoveAllEffects<BuilderEditingEffect<IClock>>();
            actor.AddEffect(new BuilderEditingEffect<IClock>(actor) { EditingItem = clone });
            actor.OutputHandler.Send($"You clone clock {original.Name.ColourName()} to new clock #{clone.Id.ToStringN0(actor)} ({clone.Name.ColourName()}), which you are now editing.");
        },
        GetListTableHeaderFunc = character => new List<string>
        {
            "Id",
            "Alias",
            "Name",
            "Primary TZ",
            "Current Time"
        },
        GetListTableContentsFunc = (character, items) => from clock in items.OfType<IClock>()
                                                         select new List<string>
                                {
                                    clock.Id.ToString("N0", character),
                                    clock.Alias,
                                    clock.Name,
                                    clock.PrimaryTimezone?.Alias ?? string.Empty,
                                    clock.DisplayTime(clock.CurrentTime, TimeDisplayTypes.Immortal)
                                },
        DefaultCommandHelp = TimeModule.ClockHelpText,
        GetEditHeader = item => $"Clock #{item.Id:N0} ({item.Name})"
    };

    public static EditableItemHelper TimezoneHelper { get; } = new()
    {
        ItemName = "Timezone",
        ItemNamePlural = "Timezones",
        SetEditableItemAction = (actor, item) =>
        {
            actor.RemoveAllEffects<BuilderEditingEffect<IMudTimeZone>>();
            if (item == null)
            {
                return;
            }

            actor.AddEffect(new BuilderEditingEffect<IMudTimeZone>(actor) { EditingItem = (IMudTimeZone)item });
        },
        GetEditableItemFunc = actor =>
            actor.CombinedEffectsOfType<BuilderEditingEffect<IMudTimeZone>>().FirstOrDefault()?.EditingItem,
        GetAllEditableItems = actor => actor.Gameworld.Clocks.SelectMany(x => x.Timezones).Cast<IEditableItem>().ToList(),
        GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Clocks.SelectMany(x => x.Timezones).FirstOrDefault(x => x.Id == id),
        GetEditableItemByIdOrNameFunc = (actor, input) => GetTimezoneByIdOrName(actor, input),
        AddItemToGameWorldAction = item => { },
        CastToType = typeof(IMudTimeZone),
        EditableNewAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("Which clock should this timezone belong to?");
                return;
            }

            var clock = actor.Gameworld.Clocks.GetByIdOrNames(input.PopSpeech());
            if (clock is null)
            {
                actor.OutputHandler.Send("There is no such clock.");
                return;
            }

            var alias = input.PopSpeech();
            if (string.IsNullOrEmpty(alias))
            {
                actor.OutputHandler.Send("You must specify an alias for the timezone.");
                return;
            }

            if (clock.Timezones.Any(x => x.Alias.EqualTo(alias)))
            {
                actor.OutputHandler.Send($"There is already a timezone with the alias {alias.ColourValue()} for that clock.");
                return;
            }

            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a name for the timezone.");
                return;
            }

            var name = input.PopSpeech().TitleCase();
            if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var hours))
            {
                actor.OutputHandler.Send("You must specify an hour offset.");
                return;
            }

            var minutes = 0;
            if (!input.IsFinished && !int.TryParse(input.PopSpeech(), out minutes))
            {
                actor.OutputHandler.Send("The minute offset must be a valid number.");
                return;
            }

            var timezone = new MudTimeZone(clock, hours, minutes, name, alias);
            clock.AddTimezone(timezone);
            actor.RemoveAllEffects<BuilderEditingEffect<IMudTimeZone>>();
            actor.AddEffect(new BuilderEditingEffect<IMudTimeZone>(actor) { EditingItem = timezone });
            actor.OutputHandler.Send($"You create timezone #{timezone.Id.ToStringN0(actor)} ({timezone.Alias.ColourValue()}) for {clock.Name.ColourName()}, which you are now editing.");
        },
        EditableCloneAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("Which timezone do you want to clone?");
                return;
            }

            var original = GetTimezoneByIdOrName(actor, input.PopSpeech());
            if (original is null)
            {
                actor.OutputHandler.Send("There is no such timezone to clone. Use clock:timezone if the alias is ambiguous.");
                return;
            }

            if (input.IsFinished)
            {
                actor.OutputHandler.Send("What alias should the cloned timezone have?");
                return;
            }

            var alias = input.PopSpeech();
            var clock = original.Clock;
            if (!input.IsFinished && actor.Gameworld.Clocks.GetByIdOrNames(input.PeekSpeech()) is { } targetClock)
            {
                clock = targetClock;
                input.PopSpeech();
            }

            if (clock.Timezones.Any(x => x.Alias.EqualTo(alias)))
            {
                actor.OutputHandler.Send($"There is already a timezone with the alias {alias.ColourValue()} for {clock.Name.ColourName()}.");
                return;
            }

            var name = input.IsFinished ? original.Description : input.SafeRemainingArgument.TitleCase();
            var timezone = new MudTimeZone(clock, original.OffsetHours, original.OffsetMinutes, name, alias);
            clock.AddTimezone(timezone);
            actor.RemoveAllEffects<BuilderEditingEffect<IMudTimeZone>>();
            actor.AddEffect(new BuilderEditingEffect<IMudTimeZone>(actor) { EditingItem = timezone });
            actor.OutputHandler.Send($"You clone timezone {original.Alias.ColourValue()} to #{timezone.Id.ToStringN0(actor)} ({timezone.Alias.ColourValue()}), which you are now editing.");
        },
        GetListTableHeaderFunc = character => new List<string>
        {
            "Id",
            "Clock",
            "Alias",
            "Name",
            "Offset",
            "Primary?"
        },
        GetListTableContentsFunc = (character, items) => from timezone in items.OfType<IMudTimeZone>()
                                                         select new List<string>
                                                         {
                                                             timezone.Id.ToString("N0", character),
                                                             timezone.Clock.Alias,
                                                             timezone.Alias,
                                                             timezone.Description,
                                                             new TimeSpan(0, timezone.OffsetHours, timezone.OffsetMinutes, 0).Describe(),
                                                             (timezone.Clock.PrimaryTimezone == timezone).ToColouredString()
                                                         },
        DefaultCommandHelp = TimeModule.TimeZoneHelp,
        GetEditHeader = item => $"Timezone #{item.Id:N0} ({item.Name})"
    };

    public static EditableItemHelper CalendarHelper { get; } = new()
    {
        ItemName = "Calendar",
        ItemNamePlural = "Calendars",
        SetEditableItemAction = (actor, item) =>
        {
            actor.RemoveAllEffects<BuilderEditingEffect<ICalendar>>();
            if (item == null)
            {
                return;
            }

            actor.AddEffect(new BuilderEditingEffect<ICalendar>(actor) { EditingItem = (ICalendar)item });
        },
        GetEditableItemFunc = actor =>
            actor.CombinedEffectsOfType<BuilderEditingEffect<ICalendar>>().FirstOrDefault()?.EditingItem,
        GetAllEditableItems = actor => actor.Gameworld.Calendars.ToList(),
        GetEditableItemByIdFunc = (actor, id) => actor.Gameworld.Calendars.Get(id),
        GetEditableItemByIdOrNameFunc = (actor, input) => actor.Gameworld.Calendars.GetByIdOrNames(input),
        AddItemToGameWorldAction = item => item.Gameworld.Add((ICalendar)item),
        CastToType = typeof(ICalendar),
        EditableNewAction = (actor, input) =>
        {
            var alias = input.PopSpeech();
            if (string.IsNullOrEmpty(alias))
            {
                actor.OutputHandler.Send("You must specify an alias for the calendar.");
                return;
            }

            if (actor.Gameworld.Calendars.Any(x => x.Alias.EqualTo(alias)))
            {
                actor.OutputHandler.Send($"There is already a calendar with the alias {alias.ColourValue()}.");
                return;
            }

            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a short name for the calendar.");
                return;
            }

            var shortName = input.PopSpeech().TitleCase();
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify the feed clock for the calendar.");
                return;
            }

            var clock = actor.Gameworld.Clocks.GetByIdOrNames(input.PopSpeech());
            if (clock is null)
            {
                actor.OutputHandler.Send("There is no such clock.");
                return;
            }

            var fullName = input.IsFinished ? shortName : input.SafeRemainingArgument.TitleCase();
            var calendar = new Calendar(actor.Gameworld, alias, shortName, fullName, clock);
            actor.Gameworld.Add(calendar);
            actor.RemoveAllEffects<BuilderEditingEffect<ICalendar>>();
            actor.AddEffect(new BuilderEditingEffect<ICalendar>(actor) { EditingItem = calendar });
            actor.OutputHandler.Send($"You create calendar #{calendar.Id.ToStringN0(actor)} ({calendar.Name.ColourName()}), which you are now editing.");
        },
        EditableCloneAction = (actor, input) =>
        {
            if (input.IsFinished)
            {
                actor.OutputHandler.Send("Which calendar do you want to clone?");
                return;
            }

            var original = actor.Gameworld.Calendars.GetByIdOrNames(input.PopSpeech());
            if (original is not Calendar concrete)
            {
                actor.OutputHandler.Send("There is no such calendar to clone.");
                return;
            }

            var alias = input.PopSpeech();
            if (string.IsNullOrEmpty(alias))
            {
                actor.OutputHandler.Send("You must specify an alias for the cloned calendar.");
                return;
            }

            if (actor.Gameworld.Calendars.Any(x => x.Alias.EqualTo(alias)))
            {
                actor.OutputHandler.Send($"There is already a calendar with the alias {alias.ColourValue()}.");
                return;
            }

            if (input.IsFinished)
            {
                actor.OutputHandler.Send("You must specify a short name for the cloned calendar.");
                return;
            }

            var shortName = input.PopSpeech().TitleCase();
            var fullName = input.IsFinished ? shortName : input.SafeRemainingArgument.TitleCase();
            var clone = concrete.Clone(alias, shortName, fullName);
            actor.Gameworld.Add(clone);
            actor.RemoveAllEffects<BuilderEditingEffect<ICalendar>>();
            actor.AddEffect(new BuilderEditingEffect<ICalendar>(actor) { EditingItem = clone });
            actor.OutputHandler.Send($"You clone calendar {original.Name.ColourName()} to #{clone.Id.ToStringN0(actor)} ({clone.Name.ColourName()}), which you are now editing.");
        },
        GetListTableHeaderFunc = character => new List<string>
        {
            "Id",
            "Alias",
            "Name",
            "Clock",
            "Current Date",
            "Months",
            "Weekdays"
        },
        GetListTableContentsFunc = (character, items) => from calendar in items.OfType<ICalendar>()
                                                         select new List<string>
                                                         {
                                                             calendar.Id.ToString("N0", character),
                                                             calendar.Alias,
                                                             calendar.Name,
                                                             calendar.FeedClock?.Alias ?? string.Empty,
                                                             calendar.CurrentDate?.GetDateString() ?? string.Empty,
                                                             calendar.Months.Count.ToString("N0", character),
                                                             calendar.Weekdays.Count.ToString("N0", character)
                                                         },
        DefaultCommandHelp = TimeModule.CalendarAdminHelpText,
        GetEditHeader = item => $"Calendar #{item.Id:N0} ({item.Name})"
    };
}


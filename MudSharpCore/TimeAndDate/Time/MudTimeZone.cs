using MudSharp.Database;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.TimeAndDate.Time;

public class MudTimeZone : SaveableItem, IMudTimeZone
{
    protected string _alias;
    protected string _description;
    protected int _offsetHours;
    protected int _offsetMinutes;
    public IClock Clock { get; }

    /// <inheritdoc />
    public override void Save()
    {
        Timezone dbitem = FMDB.Context.Timezones.Find(Id);
        dbitem.Name = Alias;
        dbitem.Description = Description;
        dbitem.OffsetHours = OffsetHours;
        dbitem.OffsetMinutes = OffsetMinutes;
        Changed = false;
    }

    public MudTimeZone(IClock clock, int offsethours, int offsetminutes, string description, string alias)
    {
        Gameworld = clock.Gameworld;
        Clock = clock;
        using (new FMDB())
        {
            Timezone dbitem = new();
            FMDB.Context.Timezones.Add(dbitem);
            dbitem.Name = alias;
            dbitem.Description = description;
            dbitem.OffsetHours = offsethours;
            dbitem.OffsetMinutes = offsetminutes;
            dbitem.ClockId = clock.Id;
            FMDB.Context.SaveChanges();
            LoadFromDB(dbitem);
        }
    }

    public MudTimeZone(int id, int offsethours, int offsetminutes, string description, string alias)
    {
        _id = id;
        _offsetHours = offsethours;
        _offsetMinutes = offsetminutes;
        _description = description;
        _alias = alias;
        _name = Alias;
    }

    public MudTimeZone(int id, int offsethours, int offsetminutes, string description, string alias, IClock clock)
        : this(id, offsethours, offsetminutes, description, alias)
    {
        Clock = clock;
        Gameworld = clock?.Gameworld;
    }

    public MudTimeZone(Timezone zone, IClock clock, IFuturemud game)
    {
        Gameworld = game;
        Clock = clock;
        LoadFromDB(zone);
    }

    public override string FrameworkItemType => "MudTimeZone";

    public int OffsetHours
    {
        get => _offsetHours;
        set
        {
            _offsetHours = value;
            Changed = true;
        }
    }

    public int OffsetMinutes
    {
        get => _offsetMinutes;
        set { _offsetMinutes = value; Changed = true; }
    }

    public string Description
    {
        get => _description;
        set { _description = value; Changed = true; }
    }

    public string Alias
    {
        get => _alias;
        set
        {
            _alias = value;
            _name = value;
            Changed = true;
        }
    }

    IEnumerable<string> IHaveMultipleNames.Names => [Name, Alias];

    private void LoadFromDB(Timezone zone)
    {
        _id = zone.Id;
        _offsetHours = zone.OffsetHours;
        _offsetMinutes = zone.OffsetMinutes;
        _description = zone.Description;
        _alias = zone.Name;
        _name = Alias;
    }

    public override string ToString()
    {
        return $"Timezone {Alias} {OffsetHours}h {OffsetMinutes}m";
    }

    public const string BuildingHelpText = @"You can use the following options with this command:

	#3alias <alias>#0 - changes the timezone alias
	#3name <name>#0 - changes the timezone display name
	#3desc <name>#0 - changes the timezone display name
	#3offset <hours> [<minutes>]#0 - sets the timezone offset
	#3hours <hours>#0 - sets only the hour offset
	#3minutes <minutes>#0 - sets only the minute offset";

    public bool BuildingCommand(ICharacter actor, StringStack command)
    {
        switch (command.PopForSwitch())
        {
            case "alias":
                return BuildingCommandAlias(actor, command);
            case "name":
            case "desc":
            case "description":
                return BuildingCommandDescription(actor, command);
            case "offset":
                return BuildingCommandOffset(actor, command);
            case "hours":
            case "hour":
                return BuildingCommandHours(actor, command);
            case "minutes":
            case "minute":
                return BuildingCommandMinutes(actor, command);
            default:
                actor.OutputHandler.Send(BuildingHelpText.SubstituteANSIColour());
                return false;
        }
    }

    private bool BuildingCommandAlias(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What alias should this timezone have?");
            return false;
        }

        var alias = command.PopSpeech();
        if (Clock.Timezones.Any(x => x != this && x.Alias.EqualTo(alias)))
        {
            actor.OutputHandler.Send($"There is already a timezone with the alias {alias.ColourValue()} for this clock.");
            return false;
        }

        Alias = alias;
        actor.OutputHandler.Send($"This timezone now has the alias {alias.ColourValue()}.");
        return true;
    }

    private bool BuildingCommandDescription(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What display name should this timezone have?");
            return false;
        }

        Description = command.SafeRemainingArgument.TitleCase();
        actor.OutputHandler.Send($"This timezone is now called {Description.ColourName()}.");
        return true;
    }

    private bool BuildingCommandOffset(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var hours))
        {
            actor.OutputHandler.Send("You must specify an hour offset.");
            return false;
        }

        var minutes = 0;
        if (!command.IsFinished && !int.TryParse(command.PopSpeech(), out minutes))
        {
            actor.OutputHandler.Send("The minute offset must be a valid number.");
            return false;
        }

        if (Math.Abs(minutes) >= Clock.MinutesPerHour)
        {
            actor.OutputHandler.Send($"The minute offset must be between {(-Clock.MinutesPerHour + 1).ToStringN0(actor)} and {(Clock.MinutesPerHour - 1).ToStringN0(actor)}.");
            return false;
        }

        OffsetHours = hours;
        OffsetMinutes = minutes;
        actor.OutputHandler.Send($"This timezone is now offset by {new TimeSpan(0, hours, minutes, 0).Describe().ColourValue()}.");
        return true;
    }

    private bool BuildingCommandHours(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var hours))
        {
            actor.OutputHandler.Send("You must specify an hour offset.");
            return false;
        }

        OffsetHours = hours;
        actor.OutputHandler.Send($"This timezone now has an hour offset of {hours.ToStringN0(actor).ColourValue()}.");
        return true;
    }

    private bool BuildingCommandMinutes(ICharacter actor, StringStack command)
    {
        if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var minutes))
        {
            actor.OutputHandler.Send("You must specify a minute offset.");
            return false;
        }

        if (Math.Abs(minutes) >= Clock.MinutesPerHour)
        {
            actor.OutputHandler.Send($"The minute offset must be between {(-Clock.MinutesPerHour + 1).ToStringN0(actor)} and {(Clock.MinutesPerHour - 1).ToStringN0(actor)}.");
            return false;
        }

        OffsetMinutes = minutes;
        actor.OutputHandler.Send($"This timezone now has a minute offset of {minutes.ToStringN0(actor).ColourValue()}.");
        return true;
    }

    public string Show(ICharacter actor)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Timezone #{Id.ToStringN0(actor)} - {Alias}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine($"Clock: {Clock.Name.ColourName()} (#{Clock.Id.ToStringN0(actor)})");
        sb.AppendLine($"Alias: {Alias.ColourValue()}");
        sb.AppendLine($"Name: {Description.ColourName()}");
        sb.AppendLine($"Offset Hours: {OffsetHours.ToStringN0(actor).ColourValue()}");
        sb.AppendLine($"Offset Minutes: {OffsetMinutes.ToStringN0(actor).ColourValue()}");
        sb.AppendLine($"Offset: {new TimeSpan(0, OffsetHours, OffsetMinutes, 0).Describe().ColourValue()}");
        sb.AppendLine($"Primary For Clock: {(Clock.PrimaryTimezone == this).ToColouredString()}");
        return sb.ToString();
    }

    // TODO - Daylight savings times
}

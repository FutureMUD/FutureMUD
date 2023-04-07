using System.Xml.Linq;
using System.Linq;
using MoreLinq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.GameItems.Prototypes;

public class TimePieceGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "TimePiece";

	public IClock Clock { get; protected set; }
	public IMudTimeZone TimeZone { get; protected set; }

	public bool PlayersCanSetTime { get; protected set; }

	public string TimeDisplayString { get; protected set; }

	#region Constructors

	protected TimePieceGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"TimePiece")
	{
		Clock = Gameworld.Clocks.Get(Gameworld.GetStaticLong("DefaultTimepieceClock"));
		TimeZone = Clock?.PrimaryTimezone;
		PlayersCanSetTime = false;
		TimeDisplayString = Clock?.ShortDisplayString ?? "$h:$m:$s";
	}

	protected TimePieceGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Clock = Gameworld.Clocks.Get(long.Parse(root.Element("Clock").Value));
		PlayersCanSetTime = bool.Parse(root.Element("PlayersCanSetTime").Value);
		TimeDisplayString = root.Element("TimeDisplayString").Value;
		TimeZone = Clock?.Timezones.Get(long.Parse(root.Element("TimeZone")?.Value ?? "0"));
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Clock", Clock.Id), new XElement("TimeZone", TimeZone?.Id ?? 0),
				new XElement("PlayersCanSetTime", PlayersCanSetTime),
				new XElement("TimeDisplayString", TimeDisplayString))
			.ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new TimePieceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new TimePieceGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("TimePiece".ToLowerInvariant(), true,
			(gameworld, account) => new TimePieceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("TimePiece",
			(proto, gameworld) => new TimePieceGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"TimePiece",
			$"Turns the item into a {"[timepiece]".Colour(Telnet.Yellow)} that displays the time",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TimePieceGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText = @"You can use the following options with this component:
	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3clock <which>#0 - the clock which this timepiece is based on
	#3timezone <which>#0 - sets the timezone for this timepiece
	#3playercanset#0 - toggles whether players can set the time on this item
	#3display <text>#0 - sets the time display string.

Note: With the DISPLAY subcommand you can use the following markup in the text:

    #5$s#0 - the seconds as a number, e.g. 21
    #5$S#0 - the seconds as a word, e.g. twenty one
    #5$m#0 - the minutes as a number, e.g. 08
    #5$M#0 - the minutes as a word, e.g. eight
    #5$h#0 - the hours as a number. Note, this ignores time intervals (e.g. 24 hour time). E.g. 15
    #5$H#0 - the hours as a word. Note, this ignores time intervals (e.g. 24 hour time). E.g. fifteen
    #5$j#0 - the hours as a number, respecting time intervals (e.g. a.m / p.m), e.g. 3
    #5$J#0 - the hours as a word, respecting time intervals (e.g. a.m / p.m), e.g. three
    #5$t#0 - the timezone abbreviation, e.g. GMT
    #5$T#0 - the timezone description, e.g. Grenwich Mean Time
    #5$c#0 - the crude time, e.g. four o'clock
    #5$i#0 - the interval abbreviation, e.g. a.m or p.m
    #5$I#0 - the interval description, e.g. in the afternoon, in the morning";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "clock":
				return BuildingCommandClock(actor, command);
			case "player":
			case "settable":
			case "playercanset":
			case "canset":
			case "set":
				return BuildingCommandPlayerCanSet(actor, command);
			case "string":
			case "display":
			case "time":
				return BuildingCommandDisplayString(actor, command);
			case "timezone":
			case "tz":
			case "zone":
				return BuildingCommandTimezone(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandDisplayString(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send(
				"What time string do you want to set for this time piece? See COMP SET STRING HELP for more info.");
			return false;
		}

		if (command.Peek().EqualTo("help"))
		{
			actor.OutputHandler.Send(@"You can use the following special characters in the string:

    #5$s#0 - the seconds as a number, e.g. 21
    #5$S#0 - the seconds as a word, e.g. twenty one
    #5$m#0 - the minutes as a number, e.g. 08
    #5$M#0 - the minutes as a word, e.g. eight
    #5$h#0 - the hours as a number. Note, this ignores time intervals (e.g. 24 hour time). E.g. 15
    #5$H#0 - the hours as a word. Note, this ignores time intervals (e.g. 24 hour time). E.g. fifteen
    #5$j#0 - the hours as a number, respecting time intervals (e.g. a.m / p.m), e.g. 3
    #5$J#0 - the hours as a word, respecting time intervals (e.g. a.m / p.m), e.g. three
    #5$t#0 - the timezone abbreviation, e.g. GMT
    #5$T#0 - the timezone description, e.g. Grenwich Mean Time
    #5$c#0 - the crude time, e.g. four o'clock
    #5$i#0 - the interval abbreviation, e.g. a.m or p.m
    #5$I#0 - the interval description, e.g. in the afternoon, in the morning".SubstituteANSIColour());
			return false;
		}

		TimeDisplayString = command.SafeRemainingArgument;
		Changed = true;
		actor.Send(
			$"This clock now uses the string {TimeDisplayString.Colour(Telnet.Cyan)} to display time. This would show a time of {Clock.DisplayTime(Clock.CurrentTime.GetTimeByTimezone(TimeZone), TimeDisplayString).Colour(Telnet.Green)} right now.");
		return true;
	}

	private bool BuildingCommandClock(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which clock do you want to set this timepiece to use? You can specify an ID or name.");
			return false;
		}

		var clock = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Clocks.Get(value)
			: Gameworld.Clocks.GetByName(command.Last);
		if (clock == null)
		{
			actor.Send("There is no such clock.");
			return false;
		}

		Clock = clock;
		TimeZone = clock.PrimaryTimezone;
		Changed = true;
		actor.Send($"This time piece will now be tied to the {clock.Name.TitleCase().Colour(Telnet.Green)} clock.");
		return true;
	}

	private bool BuildingCommandTimezone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You can select from the following timezones:\n\n{Clock.Timezones.OrderBy(x => x.OffsetHours).ThenBy(x => x.OffsetMinutes).ThenBy(x => x.Alias).Select(x => $"{x.Alias.ColourValue()} - {x.Description}").ListToLines(true)}");
			return false;
		}

		var text = command.SafeRemainingArgument;
		var timezone =
			Clock.Timezones.FirstOrDefault(x => x.Description.EqualTo(text)) ??
			Clock.Timezones.FirstOrDefault(x => x.Alias.EqualTo(text));
		if (timezone is null)
		{
			actor.OutputHandler.Send(
				$"There is no such timezone. You can select from the following timezones:\n\n{Clock.Timezones.OrderBy(x => x.OffsetHours).ThenBy(x => x.OffsetMinutes).ThenBy(x => x.Alias).Select(x => $"{x.Alias.ColourValue()} - {x.Description}").ListToLines(true)}");
			return false;
		}

		TimeZone = timezone;
		Changed = true;
		actor.OutputHandler.Send(
			$"This timepiece will now use the {TimeZone.Alias.ColourName()} timezone ({TimeZone.Description}).");
		return true;
	}

	private bool BuildingCommandPlayerCanSet(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			PlayersCanSetTime = !PlayersCanSetTime;
		}
		else if (bool.TryParse(command.Pop(), out var value))
		{
			PlayersCanSetTime = value;
		}
		else
		{
			actor.Send("You can either toggle this value by supplying no argument, or supply a true or false.");
			return false;
		}

		Changed = true;
		actor.Send($"Players can {(PlayersCanSetTime ? "now" : "no longer")} set the time on this time piece.");
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a time piece that keeps perfect time. It is set for the {4} clock and {8} timezone, and {5} have its time manually adjusted by players. It uses the string {6} to display time, which would currently be {7} for the appropriate clock.",
			"TimePiece Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			$"{Clock.Name} (#{Clock.Id:N0})",
			PlayersCanSetTime ? "can" : "cannot",
			TimeDisplayString.Colour(Telnet.Cyan),
			Clock.DisplayTime(Clock.CurrentTime.GetTimeByTimezone(TimeZone), TimeDisplayString).Colour(Telnet.Green),
			TimeZone.Alias.ColourName()
		);
	}
}
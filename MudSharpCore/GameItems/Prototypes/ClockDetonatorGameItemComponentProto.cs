#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.GameItems.Prototypes;

public class ClockDetonatorGameItemComponentProto : GameItemComponentProto,
	IArmableExplosiveTriggerPrototype, IGameItemComponentPrototypeRequirementProvider
{
	private static readonly IReadOnlyCollection<GameItemComponentPrototypeRequirement> Requirements =
	[
		new(typeof(IDetonatable), "it needs an explosive payload to detonate at the selected world time")
	];

	protected ClockDetonatorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "ClockDetonator")
	{
		var defaultClock = Gameworld.Clocks.Get(Gameworld.GetStaticLong("DefaultTimepieceClock"));
		Calendar = Gameworld.Calendars.FirstOrDefault(x => x.FeedClock == defaultClock) ?? Gameworld.Calendars.First();
		Clock = Calendar.FeedClock;
		TimeZone = Clock.PrimaryTimezone;
		CanBeDisarmed = true;
		ArmEmote = "@ arm|arms the clock detonator on $1.";
		DisarmEmote = "@ disarm|disarms the clock detonator on $1.";
	}

	protected ClockDetonatorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "ClockDetonator";
	public IReadOnlyCollection<GameItemComponentPrototypeRequirement> RequiredSiblingComponents => Requirements;
	public ICalendar Calendar { get; protected set; } = null!;
	public IClock Clock { get; protected set; } = null!;
	public IMudTimeZone TimeZone { get; protected set; } = null!;
	public bool CanBeDisarmed { get; protected set; }
	public string ArmEmote { get; protected set; } = string.Empty;
	public string DisarmEmote { get; protected set; } = string.Empty;

	protected override void LoadFromXml(XElement root)
	{
		Calendar = Gameworld.Calendars.Get(long.Parse(root.Element("Calendar")?.Value ?? "0")) ??
		           Gameworld.Calendars.First();
		Clock = Gameworld.Clocks.Get(long.Parse(root.Element("Clock")?.Value ?? "0")) ?? Calendar.FeedClock;
		if (Clock != Calendar.FeedClock)
		{
			Clock = Calendar.FeedClock;
		}
		TimeZone = Clock.Timezones.Get(long.Parse(root.Element("TimeZone")?.Value ?? "0")) ??
		           Clock.PrimaryTimezone;
		CanBeDisarmed = bool.Parse(root.Element("CanBeDisarmed")?.Value ?? "true");
		ArmEmote = root.Element("ArmEmote")?.Value ?? "@ arm|arms the clock detonator on $1.";
		DisarmEmote = root.Element("DisarmEmote")?.Value ?? "@ disarm|disarms the clock detonator on $1.";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Calendar", Calendar.Id),
			new XElement("Clock", Clock.Id),
			new XElement("TimeZone", TimeZone.Id),
			new XElement("CanBeDisarmed", CanBeDisarmed),
			new XElement("ArmEmote", new XCData(ArmEmote)),
			new XElement("DisarmEmote", new XCData(DisarmEmote))).ToString();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "calendar": return BuildingCommandCalendar(actor, command);
			case "clock": return BuildingCommandClock(actor, command);
			case "timezone":
			case "tz": return BuildingCommandTimezone(actor, command);
			case "disarmable":
				CanBeDisarmed = !CanBeDisarmed;
				Changed = true;
				actor.Send($"This detonator is {(CanBeDisarmed ? "now" : "no longer")} disarmable once armed.");
				return true;
			case "armemote": return BuildingCommandEmote(actor, command, true);
			case "disarmemote": return BuildingCommandEmote(actor, command, false);
			default: return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandCalendar(ICharacter actor, StringStack command)
	{
		var calendar = command.IsFinished ? null : Gameworld.Calendars.GetByIdOrNames(command.SafeRemainingArgument);
		if (calendar is null)
		{
			actor.Send("Which calendar should this clock detonator use?");
			return false;
		}

		Calendar = calendar;
		Clock = calendar.FeedClock;
		TimeZone = Clock.PrimaryTimezone;
		Changed = true;
		actor.Send($"This detonator now uses {Calendar.FullName.ColourName()} and the {Clock.Name.ColourName()} clock.");
		return true;
	}

	private bool BuildingCommandClock(ICharacter actor, StringStack command)
	{
		var clock = command.IsFinished ? null : Gameworld.Clocks.GetByIdOrNames(command.SafeRemainingArgument);
		if (clock is null)
		{
			actor.Send("Which world clock should this detonator use?");
			return false;
		}

		var calendar = Gameworld.Calendars.FirstOrDefault(x => x.FeedClock == clock);
		if (calendar is null)
		{
			actor.Send("No calendar is driven by that clock, so it cannot be used for an exact datetime trigger.");
			return false;
		}

		Clock = clock;
		Calendar = calendar;
		TimeZone = clock.PrimaryTimezone;
		Changed = true;
		actor.Send($"This detonator now uses the {Clock.Name.ColourName()} clock and {Calendar.FullName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandTimezone(ICharacter actor, StringStack command)
	{
		var timezone = command.IsFinished ? null : Clock.Timezones.GetByIdOrNames(command.SafeRemainingArgument);
		if (timezone is null)
		{
			actor.Send($"Valid timezones are {Clock.Timezones.Select(x => x.Alias.ColourValue()).ListToString()}.");
			return false;
		}

		TimeZone = timezone;
		Changed = true;
		actor.Send($"This detonator now interprets and displays target times in {TimeZone.Alias.ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command, bool arming)
	{
		if (command.IsFinished)
		{
			actor.Send("You must specify an emote using @ for the actor and $1 for the explosive item.");
			return false;
		}
		var emote = new Emote(command.SafeRemainingArgument, actor, actor, new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.Send(emote.ErrorMessage);
			return false;
		}
		if (arming) ArmEmote = command.SafeRemainingArgument;
		else DisarmEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"The {(arming ? "arming" : "disarming")} emote is now {command.SafeRemainingArgument.ColourCommand()}.");
		return true;
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	#3calendar <which>#0 - sets the world calendar and its feed clock
	#3clock <which>#0 - selects a clock and its first compatible calendar
	#3timezone <which>#0 - sets the timezone used for input and display
	#3disarmable#0 - toggles whether an armed clock trigger can be stopped
	#3armemote <emote>#0 - sets the arming emote
	#3disarmemote <emote>#0 - sets the disarming emote";

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n{BuildingHelpText}";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Clock Detonator Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nCalendar: {Calendar.FullName.ColourName()} (#{Calendar.Id.ToString("N0", actor)})\nClock: {Clock.Name.ColourName()} (#{Clock.Id.ToString("N0", actor)})\nTimezone: {TimeZone.Alias.ColourValue()}\nDisarmable: {CanBeDisarmed.ToColouredString()}\nArm Emote: {ArmEmote.ColourCommand()}\nDisarm Emote: {DisarmEmote.ColourCommand()}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("clockdetonator", true,
			(gameworld, account) => new ClockDetonatorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("clock detonator", false,
			(gameworld, account) => new ClockDetonatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ClockDetonator",
			(proto, gameworld) => new ClockDetonatorGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("ClockDetonator",
			$"An {"[armable]".Colour(Telnet.Yellow)} exact in-game datetime trigger for an explosive payload",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ClockDetonatorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ClockDetonatorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ClockDetonatorGameItemComponentProto(proto, gameworld));
	}
}

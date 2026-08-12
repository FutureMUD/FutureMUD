#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class CountdownDetonatorGameItemComponentProto : GameItemComponentProto,
	IArmableExplosiveTriggerPrototype, IGameItemComponentPrototypeRequirementProvider
{
	private static readonly IReadOnlyCollection<GameItemComponentPrototypeRequirement> Requirements =
	[
		new(typeof(IDetonatable), "it needs an explosive payload to detonate when the countdown expires")
	];

	protected CountdownDetonatorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "CountdownDetonator")
	{
		DefaultDelay = TimeSpan.FromSeconds(10);
		MinimumDelay = TimeSpan.FromSeconds(1);
		MaximumDelay = TimeSpan.FromHours(24);
		PlayersCanSetDelay = true;
		CanBeDisarmed = true;
		ArmEmote = "@ arm|arms the countdown detonator on $1.";
		DisarmEmote = "@ disarm|disarms the countdown detonator on $1.";
	}

	protected CountdownDetonatorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "CountdownDetonator";
	public IReadOnlyCollection<GameItemComponentPrototypeRequirement> RequiredSiblingComponents => Requirements;
	public TimeSpan DefaultDelay { get; protected set; }
	public TimeSpan MinimumDelay { get; protected set; }
	public TimeSpan MaximumDelay { get; protected set; }
	public bool PlayersCanSetDelay { get; protected set; }
	public bool CanBeDisarmed { get; protected set; }
	public string ArmEmote { get; protected set; } = string.Empty;
	public string DisarmEmote { get; protected set; } = string.Empty;

	protected override void LoadFromXml(XElement root)
	{
		DefaultDelay = TimeSpan.FromSeconds(double.Parse(root.Element("DefaultDelaySeconds")?.Value ?? "10"));
		MinimumDelay = TimeSpan.FromSeconds(double.Parse(root.Element("MinimumDelaySeconds")?.Value ?? "1"));
		MaximumDelay = TimeSpan.FromSeconds(double.Parse(root.Element("MaximumDelaySeconds")?.Value ?? "86400"));
		PlayersCanSetDelay = bool.Parse(root.Element("PlayersCanSetDelay")?.Value ?? "true");
		CanBeDisarmed = bool.Parse(root.Element("CanBeDisarmed")?.Value ?? "true");
		ArmEmote = root.Element("ArmEmote")?.Value ?? "@ arm|arms the countdown detonator on $1.";
		DisarmEmote = root.Element("DisarmEmote")?.Value ?? "@ disarm|disarms the countdown detonator on $1.";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("DefaultDelaySeconds", DefaultDelay.TotalSeconds),
			new XElement("MinimumDelaySeconds", MinimumDelay.TotalSeconds),
			new XElement("MaximumDelaySeconds", MaximumDelay.TotalSeconds),
			new XElement("PlayersCanSetDelay", PlayersCanSetDelay),
			new XElement("CanBeDisarmed", CanBeDisarmed),
			new XElement("ArmEmote", new XCData(ArmEmote)),
			new XElement("DisarmEmote", new XCData(DisarmEmote))).ToString();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "default":
				return BuildingCommandDelay(actor, command, "default");
			case "minimum":
			case "min":
				return BuildingCommandDelay(actor, command, "minimum");
			case "maximum":
			case "max":
				return BuildingCommandDelay(actor, command, "maximum");
			case "playerdelay":
			case "playerset":
				PlayersCanSetDelay = !PlayersCanSetDelay;
				Changed = true;
				actor.Send($"Players can {(PlayersCanSetDelay ? "now" : "no longer")} choose this detonator's countdown.");
				return true;
			case "disarmable":
				CanBeDisarmed = !CanBeDisarmed;
				Changed = true;
				actor.Send($"This detonator is {(CanBeDisarmed ? "now" : "no longer")} disarmable once armed.");
				return true;
			case "armemote":
				return BuildingCommandEmote(actor, command, true);
			case "disarmemote":
				return BuildingCommandEmote(actor, command, false);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command, string which)
	{
		if (command.IsFinished || !TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value) ||
		    value <= TimeSpan.Zero)
		{
			actor.Send("You must specify a positive duration.");
			return false;
		}

		switch (which)
		{
			case "default": DefaultDelay = value; break;
			case "minimum": MinimumDelay = value; break;
			case "maximum": MaximumDelay = value; break;
		}

		Changed = true;
		actor.Send($"The {which} countdown delay is now {value.Describe(actor).ColourValue()}.");
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

	public override bool CanSubmit()
	{
		return MinimumDelay > TimeSpan.Zero && MinimumDelay <= DefaultDelay && DefaultDelay <= MaximumDelay &&
		       ExplosiveDeadlineScheduler.TryGetDeadline(DateTime.UtcNow, MaximumDelay, out _) &&
		       base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (MinimumDelay <= TimeSpan.Zero) return "The minimum countdown must be positive.";
		if (MinimumDelay > DefaultDelay) return "The minimum countdown cannot exceed the default countdown.";
		if (DefaultDelay > MaximumDelay) return "The default countdown cannot exceed the maximum countdown.";
		if (!ExplosiveDeadlineScheduler.TryGetDeadline(DateTime.UtcNow, MaximumDelay, out _))
			return "The maximum countdown is too long to schedule safely.";
		return base.WhyCannotSubmit();
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	#3default <duration>#0 - sets the default countdown
	#3minimum <duration>#0 - sets the shortest permitted countdown
	#3maximum <duration>#0 - sets the longest permitted countdown
	#3playerdelay#0 - toggles whether players may choose a countdown
	#3disarmable#0 - toggles whether an armed countdown can be stopped
	#3armemote <emote>#0 - sets the arming emote; @ is the actor and $1 is the item
	#3disarmemote <emote>#0 - sets the disarming emote";

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n{BuildingHelpText}";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Countdown Detonator Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nDefault: {DefaultDelay.Describe(actor).ColourValue()}\nRange: {MinimumDelay.Describe(actor).ColourValue()} to {MaximumDelay.Describe(actor).ColourValue()}\nPlayer Settable: {PlayersCanSetDelay.ToColouredString()}\nDisarmable: {CanBeDisarmed.ToColouredString()}\nArm Emote: {ArmEmote.ColourCommand()}\nDisarm Emote: {DisarmEmote.ColourCommand()}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("countdowndetonator", true,
			(gameworld, account) => new CountdownDetonatorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("countdown detonator", false,
			(gameworld, account) => new CountdownDetonatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("CountdownDetonator",
			(proto, gameworld) => new CountdownDetonatorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("CountdownDetonator",
			$"An {"[armable]".Colour(Telnet.Yellow)} countdown trigger for a sibling explosive payload",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new CountdownDetonatorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CountdownDetonatorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new CountdownDetonatorGameItemComponentProto(proto, gameworld));
	}
}

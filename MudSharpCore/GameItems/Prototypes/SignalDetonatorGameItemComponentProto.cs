#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class SignalDetonatorGameItemComponentProto : GameItemComponentProto,
	IArmableExplosiveTriggerPrototype, IRuntimeConfigurableSignalSinkComponentPrototype, IConsumePowerPrototype,
	IGameItemComponentPrototypeRequirementProvider
{
	private static readonly IReadOnlyCollection<GameItemComponentPrototypeRequirement> Requirements =
	[
		new(typeof(IDetonatable), "it needs an explosive payload to detonate when its control signal activates")
	];

	protected SignalDetonatorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "SignalDetonator")
	{
		SourceComponentName = string.Empty;
		SourceEndpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
		ActivationThreshold = 0.5;
		ActiveWhenAboveThreshold = true;
		ActivationMode = ExplosiveSignalActivationMode.Edge;
		RequiresPower = true;
		PowerConsumptionInWatts = 0.1;
		CanBeDisarmed = true;
		ArmEmote = "@ arm|arms the signal detonator on $1.";
		DisarmEmote = "@ disarm|disarms the signal detonator on $1.";
	}

	protected SignalDetonatorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "SignalDetonator";
	public IReadOnlyCollection<GameItemComponentPrototypeRequirement> RequiredSiblingComponents => Requirements;
	public long SourceComponentId { get; protected set; }
	public string SourceComponentName { get; protected set; } = string.Empty;
	public string SourceEndpointKey { get; protected set; } = SignalComponentUtilities.DefaultLocalSignalEndpointKey;
	public double ActivationThreshold { get; protected set; }
	public bool ActiveWhenAboveThreshold { get; protected set; }
	public ExplosiveSignalActivationMode ActivationMode { get; protected set; }
	public bool RequiresPower { get; protected set; }
	public double PowerConsumptionInWatts { get; protected set; }
	public bool CanBeDisarmed { get; protected set; }
	public string ArmEmote { get; protected set; } = string.Empty;
	public string DisarmEmote { get; protected set; } = string.Empty;

	protected override void LoadFromXml(XElement root)
	{
		SourceComponentId = long.TryParse(root.Element("SourceComponentId")?.Value, out var sourceId) ? sourceId : 0L;
		SourceComponentName = root.Element("SourceComponentName")?.Value ?? string.Empty;
		SourceEndpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(root.Element("SourceEndpointKey")?.Value);
		ActivationThreshold = double.TryParse(root.Element("ActivationThreshold")?.Value, out var threshold) &&
		                      double.IsFinite(threshold) ? threshold : 0.5;
		ActiveWhenAboveThreshold = bool.Parse(root.Element("ActiveWhenAboveThreshold")?.Value ?? "true");
		ActivationMode = Enum.TryParse<ExplosiveSignalActivationMode>(root.Element("ActivationMode")?.Value, true,
			out var mode) ? mode : ExplosiveSignalActivationMode.Edge;
		RequiresPower = bool.Parse(root.Element("RequiresPower")?.Value ?? "true");
		PowerConsumptionInWatts = double.TryParse(root.Element("PowerConsumptionInWatts")?.Value, out var watts) &&
		                          double.IsFinite(watts) && watts >= 0.0 ? watts : 0.1;
		CanBeDisarmed = bool.Parse(root.Element("CanBeDisarmed")?.Value ?? "true");
		ArmEmote = root.Element("ArmEmote")?.Value ?? "@ arm|arms the signal detonator on $1.";
		DisarmEmote = root.Element("DisarmEmote")?.Value ?? "@ disarm|disarms the signal detonator on $1.";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("SourceComponentId", SourceComponentId),
			new XElement("SourceComponentName", new XCData(SourceComponentName)),
			new XElement("SourceEndpointKey", new XCData(SourceEndpointKey)),
			new XElement("ActivationThreshold", ActivationThreshold),
			new XElement("ActiveWhenAboveThreshold", ActiveWhenAboveThreshold),
			new XElement("ActivationMode", ActivationMode),
			new XElement("RequiresPower", RequiresPower),
			new XElement("PowerConsumptionInWatts", PowerConsumptionInWatts),
			new XElement("CanBeDisarmed", CanBeDisarmed),
			new XElement("ArmEmote", new XCData(ArmEmote)),
			new XElement("DisarmEmote", new XCData(DisarmEmote))).ToString();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "source": return BuildingCommandSource(actor, command);
			case "threshold": return BuildingCommandThreshold(actor, command);
			case "mode": return BuildingCommandMode(actor, command);
			case "activation": return BuildingCommandActivation(actor, command);
			case "power":
			case "powered":
				RequiresPower = !RequiresPower;
				Changed = true;
				actor.Send($"This signal detonator {(RequiresPower ? "now requires" : "no longer requires")} electrical power.");
				return true;
			case "watts":
			case "wattage": return BuildingCommandWattage(actor, command);
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

	private bool BuildingCommandSource(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which signal-source component prototype should drive this detonator?");
			return false;
		}
		var identifier = command.PopSpeech();
		if (!SignalComponentUtilities.TryResolveSignalComponentPrototype(Gameworld, identifier, out var prototype) ||
		    prototype is not ISignalSourceComponentPrototype)
		{
			actor.Send("There is no such signal-source component prototype.");
			return false;
		}
		SourceComponentId = prototype.Id;
		SourceComponentName = prototype.Name;
		SourceEndpointKey = SignalComponentUtilities.NormaliseSignalEndpointKey(
			command.IsFinished ? null : command.PopSpeech());
		Changed = true;
		actor.Send($"This detonator now listens to {SourceComponentName.ColourName()} on the {SourceEndpointKey.ColourCommand()} endpoint.");
		return true;
	}

	private bool BuildingCommandThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) ||
		    !double.IsFinite(value))
		{
			actor.Send("What finite numeric threshold should activate this detonator?");
			return false;
		}
		ActivationThreshold = value;
		Changed = true;
		actor.Send($"This detonator now uses a threshold of {value.ToString("N2", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandMode(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Should this detonator activate above or below its threshold?");
			return false;
		}
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "above":
			case "high": ActiveWhenAboveThreshold = true; break;
			case "below":
			case "low": ActiveWhenAboveThreshold = false; break;
			default:
				actor.Send("You must specify either above or below.");
				return false;
		}
		Changed = true;
		actor.Send($"This detonator now activates {(ActiveWhenAboveThreshold ? "at or above" : "below")} its threshold.");
		return true;
	}

	private bool BuildingCommandActivation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.PopSpeech().TryParseEnum(out ExplosiveSignalActivationMode mode))
		{
			actor.Send("You must specify edge or level activation.");
			return false;
		}
		ActivationMode = mode;
		Changed = true;
		actor.Send($"This detonator now uses {ActivationMode.DescribeEnum().ToLowerInvariant().ColourValue()} activation.");
		return true;
	}

	private bool BuildingCommandWattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) ||
		    !double.IsFinite(value) || value < 0.0)
		{
			actor.Send("How many non-negative watts should this detonator consume while armed?");
			return false;
		}
		PowerConsumptionInWatts = value;
		Changed = true;
		actor.Send($"This detonator now consumes {value.ToString("N3", actor).ColourValue()} watts while armed.");
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
		return SourceComponentId > 0 && double.IsFinite(ActivationThreshold) &&
		       double.IsFinite(PowerConsumptionInWatts) && PowerConsumptionInWatts >= 0.0 && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (SourceComponentId <= 0) return "You must select a signal-source component prototype.";
		if (!double.IsFinite(ActivationThreshold)) return "The activation threshold must be finite.";
		if (!double.IsFinite(PowerConsumptionInWatts) || PowerConsumptionInWatts < 0.0)
			return "Power consumption must be a non-negative finite number.";
		return base.WhyCannotSubmit();
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	#3source <component> [<endpoint>]#0 - sets the signal source and endpoint
	#3threshold <number>#0 - sets the activation threshold
	#3mode <above|below>#0 - chooses which side of the threshold is active
	#3activation <edge|level>#0 - chooses transition-only or active-level triggering
	#3power#0 - toggles whether the armed detonator requires electrical power
	#3watts <number>#0 - sets armed power consumption
	#3disarmable#0 - toggles whether the armed detonator can be stopped
	#3armemote <emote>#0 - sets the arming emote
	#3disarmemote <emote>#0 - sets the disarming emote";

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n{BuildingHelpText}";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Signal Detonator Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nSource: {SignalComponentUtilities.DescribeSignalComponent(Gameworld, SourceComponentId, SourceComponentName, SourceEndpointKey).ColourName()}\nThreshold: {ActivationThreshold.ToString("N2", actor).ColourValue()} ({(ActiveWhenAboveThreshold ? "above" : "below")})\nActivation: {ActivationMode.DescribeEnum().ColourValue()}\nRequires Power: {RequiresPower.ToColouredString()}\nArmed Draw: {PowerConsumptionInWatts.ToString("N3", actor).ColourValue()} watts\nDisarmable: {CanBeDisarmed.ToColouredString()}\nArm Emote: {ArmEmote.ColourCommand()}\nDisarm Emote: {DisarmEmote.ColourCommand()}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("signaldetonator", true,
			(gameworld, account) => new SignalDetonatorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("signal detonator", false,
			(gameworld, account) => new SignalDetonatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("SignalDetonator",
			(proto, gameworld) => new SignalDetonatorGameItemComponentProto(proto, gameworld));
		manager.AddModernTypeHelpInfo("SignalDetonator",
			$"An {SignalComponentUtilities.SignalConsumerTag} {"[armable]".Colour(Telnet.Yellow)} trigger for electronic or physical control signals",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new SignalDetonatorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new SignalDetonatorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new SignalDetonatorGameItemComponentProto(proto, gameworld));
	}
}

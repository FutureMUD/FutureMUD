#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class PinPullDetonatorGameItemComponentProto : GameItemComponentProto,
	IPinPullExplosiveTriggerPrototype, IGameItemComponentPrototypeRequirementProvider
{
	private static readonly IReadOnlyCollection<GameItemComponentPrototypeRequirement> Requirements =
	[
		new(typeof(IDetonatable), "it needs an explosive payload to detonate after the pin is pulled")
	];

	protected PinPullDetonatorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "PinPullDetonator")
	{
		Delay = TimeSpan.FromSeconds(5);
		PullPinEmote = "@ pull|pulls the safety pin from $1, starting its irreversible countdown.";
	}

	protected PinPullDetonatorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "PinPullDetonator";
	public IReadOnlyCollection<GameItemComponentPrototypeRequirement> RequiredSiblingComponents => Requirements;
	public TimeSpan Delay { get; protected set; }
	public string PullPinEmote { get; protected set; } = string.Empty;

	protected override void LoadFromXml(XElement root)
	{
		Delay = TimeSpan.FromSeconds(double.Parse(root.Element("DelaySeconds")?.Value ?? "5"));
		PullPinEmote = root.Element("PullPinEmote")?.Value ??
		               "@ pull|pulls the safety pin from $1, starting its irreversible countdown.";
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("DelaySeconds", Delay.TotalSeconds),
			new XElement("PullPinEmote", new XCData(PullPinEmote))).ToString();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "delay":
			case "time":
				return BuildingCommandDelay(actor, command);
			case "emote":
			case "pullemote":
				return BuildingCommandEmote(actor, command);
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	private bool BuildingCommandDelay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !TimeSpan.TryParse(command.SafeRemainingArgument, actor, out var value) ||
		    value <= TimeSpan.Zero)
		{
			actor.Send("You must specify a positive delay after the pin is pulled.");
			return false;
		}

		Delay = value;
		Changed = true;
		actor.Send($"This pin-pull detonator will now explode after {Delay.Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandEmote(ICharacter actor, StringStack command)
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

		PullPinEmote = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"The pin-pull emote is now {PullPinEmote.ColourCommand()}.");
		return true;
	}

	public override bool CanSubmit()
	{
		return ExplosiveDeadlineScheduler.TryGetDeadline(DateTime.UtcNow, Delay, out _) && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		if (Delay <= TimeSpan.Zero) return "The pin-pull delay must be positive.";
		if (!ExplosiveDeadlineScheduler.TryGetDeadline(DateTime.UtcNow, Delay, out _))
			return "The pin-pull delay is too long to schedule safely.";
		return base.WhyCannotSubmit();
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	#3delay <duration>#0 - sets the irreversible delay after the pin is pulled
	#3emote <emote>#0 - sets the pull-pin emote; @ is the actor and $1 is the item";

	public override string ShowBuildingHelp => $"{base.ShowBuildingHelp}\n{BuildingHelpText}";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$"{"Pin-Pull Detonator Item Component".ColourName()} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nDelay: {Delay.Describe(actor).ColourValue()}\nPull Emote: {PullPinEmote.ColourCommand()}\nOnce pulled, the pin cannot be replaced and the countdown cannot be stopped.";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("pinpulldetonator", true,
			(gameworld, account) => new PinPullDetonatorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("pin pull detonator", false,
			(gameworld, account) => new PinPullDetonatorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PinPullDetonator",
			(proto, gameworld) => new PinPullDetonatorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("PinPullDetonator",
			$"An irreversible {"[pin-pull]".Colour(Telnet.Yellow)} countdown trigger for an explosive payload",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new PinPullDetonatorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PinPullDetonatorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PinPullDetonatorGameItemComponentProto(proto, gameworld));
	}
}

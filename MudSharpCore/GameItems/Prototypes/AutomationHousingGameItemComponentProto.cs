#nullable enable

using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Prototypes;

public class AutomationHousingGameItemComponentProto : LockingContainerGameItemComponentProto, IAutomationHousingPrototype
{
	private const string AutomationHousingBuildingHelpText = @"
	#3cables <true|false>#0 - whether this housing can conceal signal cable segments
	#3modules <true|false>#0 - whether this housing can conceal automation module items
	#3signalitems <true|false>#0 - whether this housing can conceal other signal-capable items

#6Notes:#0

	This component is itself the service housing. It reuses locking-container behaviour for access, locking, and legal handling.";

	private static readonly string CombinedBuildingHelpText =
		$@"{BuildingHelpText}{AutomationHousingBuildingHelpText}";

	protected AutomationHousingGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Automation Housing")
	{
		AllowCableSegments = true;
		AllowMountableModules = true;
		AllowSignalItems = true;
	}

	protected AutomationHousingGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public bool AllowCableSegments { get; protected set; }
	public bool AllowMountableModules { get; protected set; }
	public bool AllowSignalItems { get; protected set; }
	public override string TypeDescription => "Automation Housing";

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
		AllowCableSegments = bool.Parse(root.Element("AllowCableSegments")?.Value ?? "true");
		AllowMountableModules = bool.Parse(root.Element("AllowMountableModules")?.Value ?? "true");
		AllowSignalItems = bool.Parse(root.Element("AllowSignalItems")?.Value ?? "true");
	}

	protected override string SaveToXml()
	{
		var root = XElement.Parse(base.SaveToXml());
		root.Add(new XElement("AllowCableSegments", AllowCableSegments));
		root.Add(new XElement("AllowMountableModules", AllowMountableModules));
		root.Add(new XElement("AllowSignalItems", AllowSignalItems));
		return root.ToString();
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{AutomationHousingBuildingHelpText}";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "cables":
			case "cable":
				return BuildingCommandToggle(actor, command, value => AllowCableSegments = value,
					() => AllowCableSegments, "signal cable segments");
			case "modules":
			case "module":
				return BuildingCommandToggle(actor, command, value => AllowMountableModules = value,
					() => AllowMountableModules, "automation module items");
			case "signalitems":
			case "signal":
			case "devices":
				return BuildingCommandToggle(actor, command, value => AllowSignalItems = value,
					() => AllowSignalItems, "other signal-capable items");
			default:
				return base.BuildingCommand(actor, command.GetUndo());
		}
	}

	public override bool CanSubmit()
	{
		return (AllowCableSegments || AllowMountableModules || AllowSignalItems) && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		return !AllowCableSegments && !AllowMountableModules && !AllowSignalItems
			? "You must allow at least one category of automation item to be concealed in this housing."
			: base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return
			$@"{"Automation Housing Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

This item is a lockable automation housing or junction. It can contain {Gameworld.UnitManager.Describe(WeightLimit, UnitType.Mass, actor)} and up to {MaximumContentsSize.Describe().ColourValue()} size objects, described as {ContentsPreposition.ColourCommand()} the housing. It {(Transparent ? "is".ColourValue() : "is not".ColourError())} transparent when closed.
It uses a built-in lock of type {(LockType ?? "None").ColourValue()}, with pick difficulty {PickDifficulty.DescribeColoured()} and force difficulty {ForceDifficulty.DescribeColoured()}.

Allows Cables: {AllowCableSegments.ToColouredString()}
Allows Modules: {AllowMountableModules.ToColouredString()}
Allows Signal Items: {AllowSignalItems.ToColouredString()}
Lock Emote: {LockEmote.ColourCommand()}
Unlock Emote: {UnlockEmote.ColourCommand()}
Lock (No Actor): {LockEmoteNoActor.ColourCommand()}
Unlock (No Actor): {UnlockEmoteNoActor.ColourCommand()}";
	}

	public new static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("automationhousing", true,
			(gameworld, account) => new AutomationHousingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("automation housing", false,
			(gameworld, account) => new AutomationHousingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("automationjunction", false,
			(gameworld, account) => new AutomationHousingGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("junction housing", false,
			(gameworld, account) => new AutomationHousingGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Automation Housing",
			(proto, gameworld) => new AutomationHousingGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Automation Housing",
			$"Makes an item a {"[lockable automation housing]".Colour(Telnet.BoldGreen)} for concealed service hardware",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new AutomationHousingGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new AutomationHousingGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new AutomationHousingGameItemComponentProto(proto, gameworld));
	}

	private bool BuildingCommandToggle(ICharacter actor, StringStack command, Action<bool> setter,
		Func<bool> getter, string label)
	{
		if (command.IsFinished)
		{
			actor.Send($"Do you want this automation housing to allow {label}? Use true or false.");
			return false;
		}

		if (!bool.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.Send("You must enter either true or false.");
			return false;
		}

		setter(value);
		Changed = true;
		actor.Send(
			$"This automation housing will {(getter() ? "now" : "no longer")} allow {label.ColourCommand()}.");
		return true;
	}
}

#nullable enable

using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class AutomationHousingGameItemComponentProto : GameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	cables <true|false> - whether this housing can conceal signal cable segments
	modules <true|false> - whether this housing can conceal automation module items
	signalitems <true|false> - whether this housing can conceal other signal-capable items

Automation housings rely on existing container/openable/lockable components on the same parent item for their physical service access.";

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
		AllowCableSegments = bool.Parse(root.Element("AllowCableSegments")?.Value ?? "true");
		AllowMountableModules = bool.Parse(root.Element("AllowMountableModules")?.Value ?? "true");
		AllowSignalItems = bool.Parse(root.Element("AllowSignalItems")?.Value ?? "true");
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("AllowCableSegments", AllowCableSegments),
			new XElement("AllowMountableModules", AllowMountableModules),
			new XElement("AllowSignalItems", AllowSignalItems)
		).ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
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
				return base.BuildingCommand(actor, command);
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
			$"{"Automation Housing Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})\n\nAllows Cables: {AllowCableSegments.ToColouredString()}\nAllows Modules: {AllowMountableModules.ToColouredString()}\nAllows Signal Items: {AllowSignalItems.ToColouredString()}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
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
			$"Marks an item as an {"[automation housing or junction]".Colour(Telnet.BoldGreen)} for concealed service hardware",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
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

	private bool BuildingCommandToggle(ICharacter actor, StringStack command, System.Action<bool> setter,
		System.Func<bool> getter, string label)
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

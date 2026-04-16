#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public sealed record AutomationMountBayDefinition(string Name, string MountType);

public class AutomationMountHostGameItemComponentProto : GameItemComponentProto
{
	private const string BuildingHelpText = @"You can use the following options with this component:
	name <name> - sets the name of the component
	desc <desc> - sets the description of the component
	bay add <name> <mounttype> - adds a named automation mount bay
	bay remove <name> - removes a named automation mount bay
	access none - clears any maintenance access panel requirement
	access <componentproto> - sets a sibling openable/container component prototype that must be open for service";

	private readonly List<AutomationMountBayDefinition> _bays = [];

	public AutomationMountHostGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Automation Mount Host")
	{
	}

	protected AutomationMountHostGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public IReadOnlyCollection<AutomationMountBayDefinition> Bays => _bays;
	public long AccessPanelPrototypeId { get; protected set; }
	public string AccessPanelPrototypeName { get; protected set; } = string.Empty;
	public override string TypeDescription => "Automation Mount Host";

	protected override void LoadFromXml(XElement root)
	{
		_bays.Clear();
		foreach (var element in root.Element("Bays")?.Elements("Bay") ?? Enumerable.Empty<XElement>())
		{
			var name = element.Attribute("name")?.Value ?? string.Empty;
			var mountType = element.Attribute("mounttype")?.Value ?? string.Empty;
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(mountType))
			{
				continue;
			}

			_bays.Add(new AutomationMountBayDefinition(name, mountType));
		}

		AccessPanelPrototypeId = long.TryParse(root.Element("AccessPanelPrototypeId")?.Value, out var accessId)
			? accessId
			: 0L;
		AccessPanelPrototypeName = root.Element("AccessPanelPrototypeName")?.Value ?? string.Empty;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Bays",
				from bay in _bays
				select new XElement("Bay",
					new XAttribute("name", bay.Name),
					new XAttribute("mounttype", bay.MountType)
				)),
			new XElement("AccessPanelPrototypeId", AccessPanelPrototypeId),
			new XElement("AccessPanelPrototypeName", new XCData(AccessPanelPrototypeName))
		).ToString();
	}

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "bay":
			case "bays":
				return BuildingCommandBay(actor, command);
			case "access":
			case "panel":
				return BuildingCommandAccess(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandBay(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Do you want to add or remove a mount bay?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				return BuildingCommandBayAdd(actor, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandBayRemove(actor, command);
			default:
				actor.Send("Do you want to add or remove a mount bay?");
				return false;
		}
	}

	private bool BuildingCommandBayAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What name should this automation mount bay use?");
			return false;
		}

		var name = command.PopSpeech().Trim();
		if (_bays.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
		{
			actor.Send("There is already a mount bay with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.Send("What mount type should that bay accept?");
			return false;
		}

		var mountType = command.SafeRemainingArgument.Trim();
		if (string.IsNullOrWhiteSpace(mountType))
		{
			actor.Send("You must specify a mount type.");
			return false;
		}

		_bays.Add(new AutomationMountBayDefinition(name, mountType));
		Changed = true;
		actor.Send(
			$"This host now has a bay named {name.ColourName()} that accepts {mountType.ColourCommand()} modules.");
		return true;
	}

	private bool BuildingCommandBayRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which mount bay do you want to remove?");
			return false;
		}

		var bay = _bays.FirstOrDefault(x => x.Name.Equals(command.SafeRemainingArgument.Trim(),
			StringComparison.InvariantCultureIgnoreCase));
		if (bay is null)
		{
			actor.Send("There is no such mount bay.");
			return false;
		}

		_bays.Remove(bay);
		Changed = true;
		actor.Send($"You remove the {bay.Name.ColourName()} mount bay.");
		return true;
	}

	private bool BuildingCommandAccess(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Which sibling openable/container component prototype should gate service access, or NONE?");
			return false;
		}

		if (command.SafeRemainingArgument.Equals("none", StringComparison.InvariantCultureIgnoreCase))
		{
			AccessPanelPrototypeId = 0L;
			AccessPanelPrototypeName = string.Empty;
			Changed = true;
			actor.Send("This automation host no longer requires a maintenance access panel.");
			return true;
		}

		if (!SignalComponentUtilities.TryResolveSignalComponentPrototype(Gameworld, command.SafeRemainingArgument,
			    out var prototype) || prototype is null)
		{
			prototype = long.TryParse(command.SafeRemainingArgument, out var protoId)
				? Gameworld.ItemComponentProtos.Get(protoId)
				: Gameworld.ItemComponentProtos.GetByName(command.SafeRemainingArgument);
		}

		if (prototype is null)
		{
			actor.Send("There is no such item component prototype.");
			return false;
		}

		AccessPanelPrototypeId = prototype.Id;
		AccessPanelPrototypeName = prototype.Name;
		Changed = true;
		actor.Send(
			$"This automation host now requires the sibling component prototype {prototype.Name.ColourName()} to be open for service access.");
		return true;
	}

	public override bool CanSubmit()
	{
		return _bays.Any() && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		return !_bays.Any()
			? "You must define at least one automation mount bay first."
			: base.WhyCannotSubmit();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var bays = _bays.Any()
			? _bays.Select(x => $"{x.Name.ColourName()} ({x.MountType.ColourCommand()})").ListToString()
			: "none".ColourError();
		var access = AccessPanelPrototypeId > 0 || !string.IsNullOrWhiteSpace(AccessPanelPrototypeName)
			? $"{AccessPanelPrototypeName.ColourName()} (#{AccessPanelPrototypeId.ToString("N0", actor)})"
			: "none".ColourError();
		return
			$"{"Automation Mount Host Item Component".Colour(Telnet.Cyan)} (#{Id:N0}r{RevisionNumber:N0}, {Name})\n\nBays: {bays}\nAccess Panel: {access}";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("automationmounthost", true,
			(gameworld, account) => new AutomationMountHostGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("automation host", false,
			(gameworld, account) => new AutomationMountHostGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("autohost", false,
			(gameworld, account) => new AutomationMountHostGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Automation Mount Host",
			(proto, gameworld) => new AutomationMountHostGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Automation Mount Host",
			$"Adds named {"[automation bays]".Colour(Telnet.BoldGreen)} for installable modules such as microcontrollers",
			BuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new AutomationMountHostGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new AutomationMountHostGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new AutomationMountHostGameItemComponentProto(proto, gameworld));
	}
}

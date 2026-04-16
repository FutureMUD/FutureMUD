#nullable enable

using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public class AutomationMountHostGameItemComponent : GameItemComponent, IAutomationMountHost, IConnectable
{
	private readonly Dictionary<string, IConnectable> _mountedByBay = new(StringComparer.InvariantCultureIgnoreCase);
	private readonly List<(string BayName, long ItemId)> _pendingLoadMountedItems = [];
	private AutomationMountHostGameItemComponentProto _prototype;

	public AutomationMountHostGameItemComponent(AutomationMountHostGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public AutomationMountHostGameItemComponent(MudSharp.Models.GameItemComponent component,
		AutomationMountHostGameItemComponentProto proto, IGameItem parent)
		: base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public AutomationMountHostGameItemComponent(AutomationMountHostGameItemComponent rhs, IGameItem newParent,
		bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public IReadOnlyCollection<AutomationMountBay> Bays => _prototype.Bays
		.Select(x => new AutomationMountBay(x.Name, x.MountType, _mountedByBay.ContainsKey(x.Name),
			_mountedByBay.TryGetValue(x.Name, out var mounted) ? mounted.Parent : null))
		.ToList();
	public IEnumerable<ConnectorType> Connections => _prototype.Bays
		.Select(x => ConnectorForBay(x))
		.ToList();
	public IEnumerable<Tuple<ConnectorType, IConnectable>> ConnectedItems => _mountedByBay
		.Select(x => Tuple.Create(ConnectorForBay(x.Key), x.Value))
		.ToList();
	public IEnumerable<ConnectorType> FreeConnections => _prototype.Bays
		.Where(x => !_mountedByBay.ContainsKey(x.Name))
		.Select(ConnectorForBay)
		.ToList();
	public bool Independent => true;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new AutomationMountHostGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
		PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return description;
		}

		var sb = new StringBuilder(description);
		sb.AppendLine();
		sb.AppendLine();
		sb.AppendLine($"Its automation bays are {(CanAccessMounts(voyeur as ICharacter, out _) ? "accessible".ColourValue() : "sealed behind a maintenance panel".ColourName())}:");
		foreach (var bay in Bays)
		{
			sb.AppendLine(
				$"\t{bay.Name.ColourName()} ({bay.MountType.ColourCommand()}) - {(bay.Occupied ? bay.MountedItem!.HowSeen(voyeur).ColourName() : "empty".ColourError())}");
		}

		return sb.ToString();
	}

	public bool CanAccessMounts(ICharacter actor, out string error)
	{
		error = string.Empty;
		if (_prototype.AccessPanelPrototypeId <= 0 && string.IsNullOrWhiteSpace(_prototype.AccessPanelPrototypeName))
		{
			return true;
		}

		var accessPanel = ResolveAccessPanel();
		if (accessPanel is null)
		{
			return true;
		}

		if (accessPanel.Parent.GetItemType<IOpenable>() is IOpenable openable && !openable.IsOpen)
		{
			var voyeur = actor as IPerceiver ?? accessPanel.Parent;
			error = $"You need to open {accessPanel.Parent.HowSeen(voyeur, true)} to service those automation bays.";
			return false;
		}

		return true;
	}

	public bool CanInstallModule(ICharacter actor, IAutomationMountable module, string bayName, out string error)
	{
		if (!CanAccessMounts(actor, out error))
		{
			return false;
		}

		var bay = _prototype.Bays.FirstOrDefault(x => x.Name.Equals(bayName, StringComparison.InvariantCultureIgnoreCase));
		if (bay is null)
		{
			error = "There is no such automation bay.";
			return false;
		}

		if (_mountedByBay.ContainsKey(bay.Name))
		{
			error = "That automation bay is already occupied.";
			return false;
		}

		if (!bay.MountType.Equals(module.MountType, StringComparison.InvariantCultureIgnoreCase))
		{
			error = $"That bay only accepts {bay.MountType.ColourCommand()} modules.";
			return false;
		}

		if (module.IsMounted)
		{
			error = "That module is already installed somewhere else.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	public bool InstallModule(ICharacter actor, IAutomationMountable module, string bayName, out string error)
	{
		if (!CanInstallModule(actor, module, bayName, out error))
		{
			return false;
		}

		var connectable = module as IConnectable;
		if (connectable is null)
		{
			error = "That module does not support installation.";
			return false;
		}

		var bay = _prototype.Bays.First(x => x.Name.Equals(bayName, StringComparison.InvariantCultureIgnoreCase));
		_mountedByBay[bay.Name] = connectable;
		connectable.RawConnect(this, ConnectorForBay(bay));
		RemoveItemFromWorld(module.Parent);
		Parent.ConnectedItem(connectable, ConnectorForBay(bay));
		Changed = true;
		error = string.Empty;
		return true;
	}

	public bool RemoveModule(ICharacter actor, string bayName, out IGameItem? moduleItem, out string error)
	{
		moduleItem = null;
		if (!CanAccessMounts(actor, out error))
		{
			return false;
		}

		var bay = _prototype.Bays.FirstOrDefault(x => x.Name.Equals(bayName, StringComparison.InvariantCultureIgnoreCase));
		if (bay is null)
		{
			error = "There is no such automation bay.";
			return false;
		}

		if (!_mountedByBay.TryGetValue(bay.Name, out var connectable))
		{
			error = "That automation bay is already empty.";
			return false;
		}

		_mountedByBay.Remove(bay.Name);
		connectable.RawDisconnect(this, false);
		Parent.DisconnectedItem(connectable, ConnectorForBay(bay));
		moduleItem = connectable.Parent;
		if (actor?.Body.CanGet(moduleItem, 0) == true)
		{
			actor.Body.Get(moduleItem, silent: true);
		}
		else
		{
			(actor?.Location ?? Parent.TrueLocations.FirstOrDefault())?.Insert(moduleItem);
		}

		Changed = true;
		error = string.Empty;
		return true;
	}

	public string? GetBayNameForMountedItem(IGameItem item)
	{
		return _mountedByBay.FirstOrDefault(x => ReferenceEquals(x.Value.Parent, item)).Key;
	}

	public bool CanBeConnectedTo(IConnectable other)
	{
		return other is IAutomationMountable mountable &&
		       _prototype.Bays.Any(x =>
			       !_mountedByBay.ContainsKey(x.Name) &&
			       x.MountType.Equals(mountable.MountType, StringComparison.InvariantCultureIgnoreCase));
	}

	public bool CanConnect(ICharacter actor, IConnectable other)
	{
		return other is IAutomationMountable mountable &&
		       _prototype.Bays.Any(x =>
			       !_mountedByBay.ContainsKey(x.Name) &&
			       x.MountType.Equals(mountable.MountType, StringComparison.InvariantCultureIgnoreCase)) &&
		       other.FreeConnections.Any(x => Connections.Any(y => y.CompatibleWith(x)));
	}

	public void Connect(ICharacter actor, IConnectable other)
	{
		var bay = _prototype.Bays.FirstOrDefault(x =>
			!_mountedByBay.ContainsKey(x.Name) &&
			other is IAutomationMountable mountable &&
			x.MountType.Equals(mountable.MountType, StringComparison.InvariantCultureIgnoreCase));
		if (bay is null)
		{
			return;
		}

		_mountedByBay[bay.Name] = other;
		other.RawConnect(this, ConnectorForBay(bay));
		RemoveItemFromWorld(other.Parent);
		Parent.ConnectedItem(other, ConnectorForBay(bay));
		Changed = true;
	}

	public void RawConnect(IConnectable other, ConnectorType type)
	{
		var bay = _prototype.Bays.FirstOrDefault(x =>
			!_mountedByBay.ContainsKey(x.Name) &&
			ConnectorForBay(x).CompatibleWith(type));
		if (bay is null)
		{
			return;
		}

		_mountedByBay[bay.Name] = other;
		Parent.ConnectedItem(other, ConnectorForBay(bay));
		Changed = true;
	}

	public string WhyCannotConnect(ICharacter actor, IConnectable other)
	{
		return $"{Parent.HowSeen(actor)} has no compatible free automation mount bays.";
	}

	public bool CanBeDisconnectedFrom(IConnectable other)
	{
		return true;
	}

	public bool CanDisconnect(ICharacter actor, IConnectable other)
	{
		return _mountedByBay.Values.Any(x => ReferenceEquals(x, other));
	}

	public void Disconnect(ICharacter actor, IConnectable other)
	{
		var bay = _mountedByBay.FirstOrDefault(x => ReferenceEquals(x.Value, other));
		if (string.IsNullOrEmpty(bay.Key))
		{
			return;
		}

		_mountedByBay.Remove(bay.Key);
		other.RawDisconnect(this, false);
		Parent.DisconnectedItem(other, ConnectorForBay(bay.Key));
		if (actor?.Body.CanGet(other.Parent, 0) == true)
		{
			actor.Body.Get(other.Parent, silent: true);
		}
		else
		{
			(actor?.Location ?? Parent.TrueLocations.FirstOrDefault())?.Insert(other.Parent);
		}

		Changed = true;
	}

	public void RawDisconnect(IConnectable other, bool handleEvents)
	{
		var bay = _mountedByBay.FirstOrDefault(x => ReferenceEquals(x.Value, other));
		if (string.IsNullOrEmpty(bay.Key))
		{
			return;
		}

		_mountedByBay.Remove(bay.Key);
		if (handleEvents)
		{
			other.RawDisconnect(this, false);
			Parent.DisconnectedItem(other, ConnectorForBay(bay.Key));
			other.Parent.DisconnectedItem(this, ConnectorForBay(bay.Key));
		}

		Changed = true;
	}

	public string WhyCannotDisconnect(ICharacter actor, IConnectable other)
	{
		return $"{other.Parent.HowSeen(actor)} is not currently installed in {Parent.HowSeen(actor)}.";
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (AutomationMountHostGameItemComponentProto)newProto;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MountedItems",
				from bay in _mountedByBay
				select new XElement("Mounted",
					new XAttribute("bay", bay.Key),
					new XAttribute("item", bay.Value.Parent.Id)))
		).ToString();
	}

	public override void FinaliseLoad()
	{
		foreach (var pending in _pendingLoadMountedItems.ToList())
		{
			var item = Gameworld.TryGetItem(pending.ItemId, true);
			var connectable = item?.GetItemTypes<IConnectable>()
				.FirstOrDefault(x => x is IAutomationMountable mountable &&
				                     _prototype.Bays.Any(y =>
					                     y.Name.Equals(pending.BayName, StringComparison.InvariantCultureIgnoreCase) &&
					                     y.MountType.Equals(mountable.MountType, StringComparison.InvariantCultureIgnoreCase)));
			if (connectable is null)
			{
				continue;
			}

			_mountedByBay[pending.BayName] = connectable;
		}
	}

	private void LoadFromXml(XElement root)
	{
		foreach (var element in root.Element("MountedItems")?.Elements("Mounted") ?? Enumerable.Empty<XElement>())
		{
			var bayName = element.Attribute("bay")?.Value ?? string.Empty;
			if (!long.TryParse(element.Attribute("item")?.Value, out var itemId) || itemId <= 0 ||
			    string.IsNullOrWhiteSpace(bayName))
			{
				continue;
			}

			_pendingLoadMountedItems.Add((bayName, itemId));
		}
	}

	private IGameItemComponent? ResolveAccessPanel()
	{
		if (_prototype.AccessPanelPrototypeId <= 0 && string.IsNullOrWhiteSpace(_prototype.AccessPanelPrototypeName))
		{
			return null;
		}

		return Parent.Components.FirstOrDefault(x =>
			(_prototype.AccessPanelPrototypeId > 0 &&
			 (x.Prototype.Id == _prototype.AccessPanelPrototypeId || x.Id == _prototype.AccessPanelPrototypeId)) ||
			(!string.IsNullOrWhiteSpace(_prototype.AccessPanelPrototypeName) &&
			 x.Name.Equals(_prototype.AccessPanelPrototypeName, StringComparison.InvariantCultureIgnoreCase)) ||
			(!string.IsNullOrWhiteSpace(_prototype.AccessPanelPrototypeName) &&
			 x.Prototype.Name.Equals(_prototype.AccessPanelPrototypeName, StringComparison.InvariantCultureIgnoreCase)));
	}

	private static void RemoveItemFromWorld(IGameItem item)
	{
		if (item.GetItemType<IHoldable>()?.HeldBy != null)
		{
			item.GetItemType<IHoldable>()!.HeldBy.Take(item);
			return;
		}

		if (item.ContainedIn != null)
		{
			item.ContainedIn.Take(item);
			return;
		}

		item.Location?.Extract(item);
	}

	private ConnectorType ConnectorForBay(string bayName)
	{
		var bay = _prototype.Bays.First(x => x.Name.Equals(bayName, StringComparison.InvariantCultureIgnoreCase));
		return ConnectorForBay(bay);
	}

	private static ConnectorType ConnectorForBay(AutomationMountBayDefinition bay)
	{
		return new ConnectorType(Gender.Female, $"Automation:{bay.MountType}", false);
	}
}

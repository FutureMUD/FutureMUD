using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using DbOutfitTemplate = MudSharp.Models.OutfitTemplate;
using DbOutfitTemplateItem = MudSharp.Models.OutfitTemplateItem;

namespace MudSharp.GameItems.Inventory;

public sealed class TemplateOutfit : SaveableItem, IOutfitTemplate
{
	private readonly List<IOutfitTemplateItem> _items = new();
	private OutfitExclusivity _exclusivity;
	private string _description;

	public TemplateOutfit(DbOutfitTemplate dbitem, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		_description = dbitem.Description;
		_exclusivity = (OutfitExclusivity)dbitem.Exclusivity;
		_items.AddRange(dbitem.OutfitTemplateItems
		                      .OrderBy(x => x.WearOrder)
		                      .Select(x => new TemplateOutfitItem(x, gameworld)));
	}

	public TemplateOutfit(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		_description = "An undescribed outfit template.";
		_exclusivity = OutfitExclusivity.NonExclusive;

		using (new FMDB())
		{
			var dbitem = new DbOutfitTemplate
			{
				Name = Name,
				Description = Description,
				Exclusivity = (int)Exclusivity
			};
			FMDB.Context.OutfitTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	private TemplateOutfit(TemplateOutfit rhs, string newName)
		: this(rhs, newName, persist: true)
	{
	}

	internal TemplateOutfit(IFuturemud gameworld, string name, string description, OutfitExclusivity exclusivity, IEnumerable<IOutfitTemplateItem> items)
	{
		Gameworld = gameworld;
		_name = name;
		_description = description;
		_exclusivity = exclusivity;
		_items.AddRange(items.OrderBy(x => x.WearOrder));
		_noSave = true;
	}

	internal TemplateOutfit(TemplateOutfit rhs, string newName, bool persist)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		_description = rhs.Description;
		_exclusivity = rhs.Exclusivity;
		_items.AddRange(rhs.Items
		                   .OrderBy(x => x.WearOrder)
		                   .Select(x => new TemplateOutfitItem(x)));

		if (!persist)
		{
			_noSave = true;
			return;
		}

		using (new FMDB())
		{
			var dbitem = new DbOutfitTemplate
			{
				Name = Name,
				Description = Description,
				Exclusivity = (int)Exclusivity
			};
			foreach (var item in _items)
			{
				dbitem.OutfitTemplateItems.Add(ToDbItem(item));
			}
			FMDB.Context.OutfitTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "OutfitTemplate";

	public override string Name => _name;

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			Changed = true;
		}
	}

	public OutfitExclusivity Exclusivity
	{
		get => _exclusivity;
		set
		{
			_exclusivity = value;
			Changed = true;
		}
	}

	public IEnumerable<IOutfitTemplateItem> Items => _items;

	public IEnumerable<string> ValidationWarnings => ValidateTemplate();

	public IOutfitTemplate Clone(string newName)
	{
		return new TemplateOutfit(this, newName);
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.OutfitTemplates.Find(Id) ??
			             throw new ApplicationException($"Unable to find outfit template #{Id:N0} to save.");
			dbitem.Name = Name;
			dbitem.Description = Description;
			dbitem.Exclusivity = (int)Exclusivity;
			FMDB.Context.OutfitTemplateItems.RemoveRange(FMDB.Context.OutfitTemplateItems.Where(x => x.OutfitTemplateId == Id));
			foreach (var item in _items.OrderBy(x => x.WearOrder))
			{
				var dbtemplateItem = ToDbItem(item);
				dbtemplateItem.OutfitTemplateId = Id;
				FMDB.Context.OutfitTemplateItems.Add(dbtemplateItem);
			}
			FMDB.Context.SaveChanges();
		}
		Changed = false;
	}

	private static DbOutfitTemplateItem ToDbItem(IOutfitTemplateItem item)
	{
		return new DbOutfitTemplateItem
		{
			TemplateKey = item.TemplateKey,
			GameItemProtoId = item.GameItemProto.Id,
			WearProfileId = item.DesiredProfile?.Id,
			Placement = (int)item.Placement,
			ContainerKey = item.ContainerKey,
			LoadArguments = item.LoadArguments ?? string.Empty,
			WearOrder = item.WearOrder
		};
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Outfit Template #{Id.ToString("N0", actor).ColourValue()}: {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Exclusivity: {Exclusivity.Describe().ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Items:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _items.OrderBy(x => x.WearOrder)
			select new List<string>
			{
				item.WearOrder.ToString("N0", actor),
				item.TemplateKey,
				item.GameItemProto is null ? "Missing".ColourError() : $"#{item.GameItemProto.Id.ToString("N0", actor)} {item.GameItemProto.ShortDescription}",
				PlacementDescription(item),
				string.IsNullOrWhiteSpace(item.LoadArguments) ? "" : item.LoadArguments
			},
			new List<string> { "Order", "Key", "Prototype", "Placement", "Load Args" },
			actor,
			Telnet.Cyan));
		var warnings = ValidationWarnings.ToList();
		if (warnings.Count > 0)
		{
			sb.AppendLine();
			sb.AppendLine("Validation Warnings:".ColourError());
			foreach (var warning in warnings)
			{
				sb.AppendLine($"\t{warning.ColourError()}");
			}
		}
		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor, command);
			case "exclusivity":
			case "exclusive":
				return BuildingCommandExclusivity(actor, command);
			case "item":
				return BuildingCommandItem(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this template:

	#3name <name>#0 - renames this outfit template
	#3description <description>#0 - sets the builder description
	#3exclusivity none|below|all#0 - sets the created outfit exclusivity
	#3item add <key> <prototype> [worn <profile>|inventory|room|container <key>] [args <load args>]#0 - adds an item
	#3item remove <key>#0 - removes an item
	#3item key <old> <new>#0 - renames an item key
	#3item proto <key> <prototype>#0 - changes an item prototype
	#3item placement <key> worn <profile>|inventory|room|container <key>#0 - changes placement
	#3item args <key> <load args|clear>#0 - changes load arguments
	#3item swap <key1> <key2>#0 - swaps item order".SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give this outfit template?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.OutfitTemplates.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an outfit template called {name.ColourName()}.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This outfit template is now called {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description do you want to give this outfit template?");
			return false;
		}

		Description = command.SafeRemainingArgument;
		actor.OutputHandler.Send("You update the description for this outfit template.");
		return true;
	}

	private bool BuildingCommandExclusivity(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "none":
			case "non":
			case "nonexclusive":
				Exclusivity = OutfitExclusivity.NonExclusive;
				break;
			case "below":
				Exclusivity = OutfitExclusivity.ExcludeItemsBelow;
				break;
			case "all":
			case "exclusive":
				Exclusivity = OutfitExclusivity.ExcludeAllItems;
				break;
			default:
				actor.OutputHandler.Send("You must specify an exclusivity of none, below or all.");
				return false;
		}

		actor.OutputHandler.Send($"Created outfits will use the {Exclusivity.Describe().ColourValue()} exclusivity setting.");
		return true;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
				return BuildingCommandItemAdd(actor, command);
			case "remove":
			case "delete":
			case "rem":
				return BuildingCommandItemRemove(actor, command);
			case "key":
				return BuildingCommandItemKey(actor, command);
			case "proto":
			case "prototype":
				return BuildingCommandItemProto(actor, command);
			case "placement":
			case "place":
				return BuildingCommandItemPlacement(actor, command);
			case "args":
			case "loadargs":
				return BuildingCommandItemArgs(actor, command);
			case "swap":
				return BuildingCommandItemSwap(actor, command);
			default:
				actor.OutputHandler.Send("You must specify add, remove, key, proto, placement, args or swap.");
				return false;
		}
	}

	private bool BuildingCommandItemAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What key should this template item use?");
			return false;
		}

		var key = command.PopSpeech().ToLowerInvariant();
		if (!ValidKey(key))
		{
			actor.OutputHandler.Send("Template keys may only contain letters, numbers, underscores and hyphens.");
			return false;
		}

		if (_items.Any(x => x.TemplateKey.EqualTo(key)))
		{
			actor.OutputHandler.Send($"There is already a template item with the key {key.ColourName()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which item prototype should this template item load?");
			return false;
		}

		var proto = Gameworld.ItemProtos.GetByIdOrUniqueNameOrName(command.PopSpeech());
		if (!CanUsePrototype(actor, proto))
		{
			return false;
		}

		var item = new TemplateOutfitItem
		{
			TemplateKey = key,
			GameItemProto = proto,
			Placement = OutfitTemplateItemPlacement.Inventory,
			WearOrder = _items.Select(x => x.WearOrder).DefaultIfEmpty(-1).Max() + 1,
			LoadArguments = string.Empty
		};

		if (!ApplyPlacement(actor, item, command, allowArgs: true))
		{
			return false;
		}

		_items.Add(item);
		Changed = true;
		actor.OutputHandler.Send($"You add {proto.ShortDescription.ColourName()} as template item {key.ColourName()}.");
		return true;
	}

	private bool BuildingCommandItemRemove(ICharacter actor, StringStack command)
	{
		var item = GetTemplateItem(actor, command);
		if (item is null)
		{
			return false;
		}

		_items.Remove(item);
		foreach (var containerReference in _items.Where(x => x.ContainerKey?.EqualTo(item.TemplateKey) == true))
		{
			containerReference.ContainerKey = null;
			containerReference.Placement = OutfitTemplateItemPlacement.Room;
		}
		NormaliseWearOrder();
		Changed = true;
		actor.OutputHandler.Send($"You remove template item {item.TemplateKey.ColourName()}.");
		return true;
	}

	private bool BuildingCommandItemKey(ICharacter actor, StringStack command)
	{
		var item = GetTemplateItem(actor, command);
		if (item is null)
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new key should this item have?");
			return false;
		}

		var newKey = command.PopSpeech().ToLowerInvariant();
		if (!ValidKey(newKey))
		{
			actor.OutputHandler.Send("Template keys may only contain letters, numbers, underscores and hyphens.");
			return false;
		}

		if (_items.Any(x => x != item && x.TemplateKey.EqualTo(newKey)))
		{
			actor.OutputHandler.Send($"There is already a template item with the key {newKey.ColourName()}.");
			return false;
		}

		var oldKey = item.TemplateKey;
		item.TemplateKey = newKey;
		foreach (var containerReference in _items.Where(x => x.ContainerKey?.EqualTo(oldKey) == true))
		{
			containerReference.ContainerKey = newKey;
		}
		Changed = true;
		actor.OutputHandler.Send($"You rename template item {oldKey.ColourName()} to {newKey.ColourName()}.");
		return true;
	}

	private bool BuildingCommandItemProto(ICharacter actor, StringStack command)
	{
		var item = GetTemplateItem(actor, command);
		if (item is null)
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prototype should this template item load?");
			return false;
		}

		var proto = Gameworld.ItemProtos.GetByIdOrUniqueNameOrName(command.SafeRemainingArgument);
		if (!CanUsePrototype(actor, proto))
		{
			return false;
		}

		var oldProto = item.GameItemProto;
		item.GameItemProto = proto;
		if (!ValidateItemPlacement(actor, item))
		{
			item.GameItemProto = oldProto;
			return false;
		}
		Changed = true;
		actor.OutputHandler.Send($"Template item {item.TemplateKey.ColourName()} will now load {proto.ShortDescription.ColourName()}.");
		return true;
	}

	private bool BuildingCommandItemPlacement(ICharacter actor, StringStack command)
	{
		var item = GetTemplateItem(actor, command);
		if (item is null)
		{
			return false;
		}

		if (!ApplyPlacement(actor, item, command, allowArgs: false))
		{
			return false;
		}

		Changed = true;
		actor.OutputHandler.Send($"Template item {item.TemplateKey.ColourName()} placement is now {PlacementDescription(item)}.");
		return true;
	}

	private bool BuildingCommandItemArgs(ICharacter actor, StringStack command)
	{
		var item = GetTemplateItem(actor, command);
		if (item is null)
		{
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What load arguments should this template item use?");
			return false;
		}

		if (command.PeekSpeech().EqualTo("clear"))
		{
			command.PopSpeech();
			item.LoadArguments = string.Empty;
			actor.OutputHandler.Send($"Template item {item.TemplateKey.ColourName()} will no longer use load arguments.");
		}
		else
		{
			item.LoadArguments = command.SafeRemainingArgument;
			actor.OutputHandler.Send($"Template item {item.TemplateKey.ColourName()} will use those load arguments.");
		}
		Changed = true;
		return true;
	}

	private bool BuildingCommandItemSwap(ICharacter actor, StringStack command)
	{
		var item1 = GetTemplateItem(actor, command);
		if (item1 is null)
		{
			return false;
		}

		var item2 = GetTemplateItem(actor, command);
		if (item2 is null)
		{
			return false;
		}

		(item1.WearOrder, item2.WearOrder) = (item2.WearOrder, item1.WearOrder);
		Changed = true;
		actor.OutputHandler.Send($"You swap the order of {item1.TemplateKey.ColourName()} and {item2.TemplateKey.ColourName()}.");
		return true;
	}

	private IOutfitTemplateItem GetTemplateItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which template item key do you want to target?");
			return null;
		}

		var key = command.PopSpeech();
		var item = _items.FirstOrDefault(x => x.TemplateKey.EqualTo(key));
		if (item is null)
		{
			actor.OutputHandler.Send($"There is no template item with the key {key.ColourName()}.");
		}
		return item;
	}

	private bool ApplyPlacement(ICharacter actor, IOutfitTemplateItem item, StringStack command, bool allowArgs)
	{
		var oldPlacement = item.Placement;
		var oldProfile = item.DesiredProfile;
		var oldContainerKey = item.ContainerKey;
		var oldLoadArguments = item.LoadArguments;

		bool Fail()
		{
			item.Placement = oldPlacement;
			item.DesiredProfile = oldProfile;
			item.ContainerKey = oldContainerKey;
			item.LoadArguments = oldLoadArguments;
			return false;
		}

		while (!command.IsFinished)
		{
			switch (command.PopForSwitch())
			{
				case "worn":
				case "wear":
					if (command.IsFinished)
					{
						actor.OutputHandler.Send("Which wear profile should this item use?");
						return false;
					}

					var profile = Gameworld.WearProfiles.GetByIdOrName(command.PopSpeech());
					if (profile is null)
					{
						actor.OutputHandler.Send("There is no such wear profile.");
						return false;
					}
					item.Placement = OutfitTemplateItemPlacement.Worn;
					item.DesiredProfile = profile;
					item.ContainerKey = null;
					break;
				case "inventory":
				case "inv":
				case "held":
					item.Placement = OutfitTemplateItemPlacement.Inventory;
					item.DesiredProfile = null;
					item.ContainerKey = null;
					break;
				case "room":
				case "cell":
					item.Placement = OutfitTemplateItemPlacement.Room;
					item.DesiredProfile = null;
					item.ContainerKey = null;
					break;
				case "container":
				case "in":
					if (command.IsFinished)
					{
						actor.OutputHandler.Send("Which template item key is the container?");
						return false;
					}

					item.Placement = OutfitTemplateItemPlacement.Container;
					item.DesiredProfile = null;
					item.ContainerKey = command.PopSpeech().ToLowerInvariant();
					break;
				case "args":
					if (!allowArgs)
					{
						actor.OutputHandler.Send("Use the separate item args command to change load arguments.");
						return Fail();
					}
					item.LoadArguments = command.SafeRemainingArgument;
					return ValidateItemPlacement(actor, item) ? true : Fail();
				default:
					actor.OutputHandler.Send("Valid placements are worn <profile>, inventory, room or container <key>.");
					return Fail();
			}
		}

		return ValidateItemPlacement(actor, item) ? true : Fail();
	}

	private bool ValidateItemPlacement(ICharacter actor, IOutfitTemplateItem item)
	{
		var warning = ValidateItem(item).FirstOrDefault();
		if (warning is null)
		{
			return true;
		}

		actor.OutputHandler.Send(warning.ColourError());
		return false;
	}

	private bool CanUsePrototype(ICharacter actor, IGameItemProto proto)
	{
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (proto.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send("Template outfits can only use current item prototypes.");
			return false;
		}

		if (proto.PreventManualLoad || proto.Components.Any(x => x.PreventManualLoad))
		{
			actor.OutputHandler.Send("That prototype prevents manual loading and cannot be used in an outfit template.");
			return false;
		}

		return true;
	}

	private IEnumerable<string> ValidateTemplate()
	{
		foreach (var duplicate in _items.GroupBy(x => x.TemplateKey, StringComparer.InvariantCultureIgnoreCase).Where(x => x.Count() > 1))
		{
			yield return $"Template key {duplicate.Key} is used more than once.";
		}

		foreach (var item in _items)
		{
			foreach (var warning in ValidateItem(item))
			{
				yield return warning;
			}
		}

		foreach (var item in _items.Where(x => x.Placement == OutfitTemplateItemPlacement.Container))
		{
			if (ContainerReferenceHasCycle(item, new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)))
			{
				yield return $"Template item {item.TemplateKey} has a cyclic container reference.";
			}
		}
	}

	private IEnumerable<string> ValidateItem(IOutfitTemplateItem item)
	{
		if (!ValidKey(item.TemplateKey))
		{
			yield return $"Template item {item.TemplateKey} has an invalid key.";
		}

		if (item.GameItemProto is null)
		{
			yield return $"Template item {item.TemplateKey} refers to a missing item prototype.";
			yield break;
		}

		if (item.GameItemProto.Status != RevisionStatus.Current)
		{
			yield return $"Template item {item.TemplateKey} uses a non-current item prototype.";
		}

		if (item.GameItemProto.PreventManualLoad || item.GameItemProto.Components.Any(x => x.PreventManualLoad))
		{
			yield return $"Template item {item.TemplateKey} uses a prototype that prevents manual loading.";
		}

		switch (item.Placement)
		{
			case OutfitTemplateItemPlacement.Worn:
				if (!item.GameItemProto.IsItemType<IWearablePrototype>())
				{
					yield return $"Template item {item.TemplateKey} is marked worn but its prototype is not wearable.";
				}

				if (item.DesiredProfile is null)
				{
					yield return $"Template item {item.TemplateKey} is marked worn but does not specify a wear profile.";
				}
				break;
			case OutfitTemplateItemPlacement.Container:
				if (string.IsNullOrWhiteSpace(item.ContainerKey))
				{
					yield return $"Template item {item.TemplateKey} is marked as contained but has no container key.";
					break;
				}

				var container = _items.FirstOrDefault(x => x.TemplateKey.EqualTo(item.ContainerKey));
				if (container is null)
				{
					yield return $"Template item {item.TemplateKey} refers to missing container key {item.ContainerKey}.";
					break;
				}

				if (ReferenceEquals(container, item))
				{
					yield return $"Template item {item.TemplateKey} cannot contain itself.";
				}

				if (container.GameItemProto?.IsItemType<IContainerPrototype>() != true)
				{
					yield return $"Template item {item.TemplateKey} refers to {item.ContainerKey} as a container, but that prototype is not a container.";
				}
				break;
		}
	}

	private bool ContainerReferenceHasCycle(IOutfitTemplateItem item, HashSet<string> seen)
	{
		if (!seen.Add(item.TemplateKey))
		{
			return true;
		}

		if (item.Placement != OutfitTemplateItemPlacement.Container || string.IsNullOrWhiteSpace(item.ContainerKey))
		{
			return false;
		}

		var container = _items.FirstOrDefault(x => x.TemplateKey.EqualTo(item.ContainerKey));
		return container is not null && ContainerReferenceHasCycle(container, seen);
	}

	private static bool ValidKey(string key)
	{
		return !string.IsNullOrWhiteSpace(key) && key.All(x => char.IsLetterOrDigit(x) || x is '_' or '-');
	}

	private void NormaliseWearOrder()
	{
		var i = 0;
		foreach (var item in _items.OrderBy(x => x.WearOrder).ToList())
		{
			item.WearOrder = i++;
		}
	}

	private static string PlacementDescription(IOutfitTemplateItem item)
	{
		return item.Placement switch
		{
			OutfitTemplateItemPlacement.Worn => $"worn as {item.DesiredProfile?.Name ?? "missing"}",
			OutfitTemplateItemPlacement.Inventory => "inventory",
			OutfitTemplateItemPlacement.Room => "room",
			OutfitTemplateItemPlacement.Container => $"in {item.ContainerKey}",
			_ => "unknown"
		};
	}

	public IOutfit Materialise(ICharacter target, string outfitNameOverride = null)
	{
		if (target is null)
		{
			throw new ArgumentNullException(nameof(target));
		}

		if (target.Location is null)
		{
			throw new InvalidOperationException("Cannot materialise an outfit template for a character without a location.");
		}

		var warning = ValidationWarnings.FirstOrDefault();
		if (warning is not null)
		{
			throw new InvalidOperationException(warning);
		}

		var createdItems = new Dictionary<string, IGameItem>(StringComparer.InvariantCultureIgnoreCase);
		foreach (var templateItem in _items.OrderBy(x => x.WearOrder))
		{
			var item = templateItem.GameItemProto.CreateNew(target, null, 1, templateItem.LoadArguments ?? string.Empty).First();
			Gameworld.Add(item);
			item.RoomLayer = target.RoomLayer;
			target.Location.Insert(item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			item.Login();
			createdItems[templateItem.TemplateKey] = item;
		}

		foreach (var templateItem in _items.OrderBy(x => x.WearOrder).Where(x => x.Placement == OutfitTemplateItemPlacement.Container))
		{
			if (!createdItems.TryGetValue(templateItem.TemplateKey, out var item) ||
			    !createdItems.TryGetValue(templateItem.ContainerKey ?? string.Empty, out var containerItem))
			{
				continue;
			}

			var container = containerItem.GetItemType<IContainer>();
			if (container?.CanPut(item) == true)
			{
				item.Get(null);
				container.Put(target, item, allowMerge: false);
			}
		}

		foreach (var templateItem in _items.OrderBy(x => x.WearOrder).Where(x => x.Placement != OutfitTemplateItemPlacement.Container))
		{
			if (!createdItems.TryGetValue(templateItem.TemplateKey, out var item))
			{
				continue;
			}

			switch (templateItem.Placement)
			{
				case OutfitTemplateItemPlacement.Worn:
					if (target.Body.CanWear(item, templateItem.DesiredProfile))
					{
						target.Body.Wear(item, templateItem.DesiredProfile, silent: true);
					}
					else if (target.Body.CanGet(item, 0))
					{
						target.Body.Get(item, silent: true);
					}
					break;
				case OutfitTemplateItemPlacement.Inventory:
					if (target.Body.CanGet(item, 0))
					{
						target.Body.Get(item, silent: true);
					}
					break;
				case OutfitTemplateItemPlacement.Room:
					break;
			}
		}

		var outfitName = UniqueOutfitName(target, string.IsNullOrWhiteSpace(outfitNameOverride) ? Name : outfitNameOverride);
		var outfit = new Outfit(target, outfitName)
		{
			Description = Description,
			Exclusivity = Exclusivity
		};

		foreach (var templateItem in _items.OrderBy(x => x.WearOrder))
		{
			if (!createdItems.TryGetValue(templateItem.TemplateKey, out var item))
			{
				continue;
			}

			var container = templateItem.Placement == OutfitTemplateItemPlacement.Container &&
			                createdItems.TryGetValue(templateItem.ContainerKey ?? string.Empty, out var containerItem)
				? containerItem
				: null;
			outfit.AddItem(item, container, templateItem.DesiredProfile, templateItem.WearOrder);
		}

		target.AddOutfit(outfit);
		return outfit;
	}

	private static string UniqueOutfitName(ICharacter target, string baseName)
	{
		if (target.Outfits.All(x => !x.Name.EqualTo(baseName)))
		{
			return baseName;
		}

		var i = 2;
		string candidate;
		do
		{
			candidate = $"{baseName} ({i++})";
		}
		while (target.Outfits.Any(x => x.Name.EqualTo(candidate)));
		return candidate;
	}
}

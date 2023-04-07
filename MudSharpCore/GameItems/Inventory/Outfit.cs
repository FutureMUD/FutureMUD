using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Inventory;

public class Outfit : IOutfit
{
	public Outfit(ICharacter owner, XElement root)
	{
		Owner = owner;
		_name = root.Element("Name").Value;
		Description = root.Element("Description").Value;
		Exclusivity = (OutfitExclusivity)int.Parse(root.Element("Exclusivity").Value);
		foreach (var item in root.Element("Items").Elements())
		{
			_items.Add(new OutfitItem(item, owner.Gameworld));
		}
	}

	public Outfit(ICharacter owner, Outfit rhs, string name)
	{
		Owner = owner;
		_name = name;
		_description = rhs._description;
		_exclusivity = rhs._exclusivity;
		_items.AddRange(rhs._items.Select(x => new OutfitItem(x)));
	}

	public Outfit(ICharacter owner, string name)
	{
		Owner = owner;
		_name = name;
		_description = "An undescribed outfit.";
		_exclusivity = OutfitExclusivity.NonExclusive;
	}

	public ICharacter Owner { get; set; }

	public string Name
	{
		get => _name;
		set
		{
			_name = value;
			Owner.OutfitsChanged = true;
		}
	}

	public string Description
	{
		get => _description;
		set
		{
			_description = value;
			Owner.OutfitsChanged = true;
		}
	}

	public OutfitExclusivity Exclusivity
	{
		get => _exclusivity;
		set
		{
			_exclusivity = value;
			Owner.OutfitsChanged = true;
		}
	}

	private readonly List<IOutfitItem> _items = new();
	public IEnumerable<IOutfitItem> Items => _items;


	private string _name;
	private string _description;
	private OutfitExclusivity _exclusivity;

	public IOutfitItem AddItem(IGameItem item, IGameItem preferredContainer, IWearProfile desiredProfile,
		int wearOrder = -1)
	{
		var newItem = new OutfitItem
		{
			Id = item.Id,
			PreferredContainerId = preferredContainer?.Id,
			PreferredContainerDescription = preferredContainer?.HowSeen(Owner, colour: false),
			DesiredProfile = desiredProfile,
			WearOrder = wearOrder == -1 ? _items.Max(x => x.WearOrder) + 1 : wearOrder,
			ItemDescription = item.HowSeen(Owner, colour: false)
		};
		_items.Add(newItem);
		for (var i = 0; i < _items.Count; i++)
		{
			_items[i].WearOrder = i;
		}

		_items.Sort((x1, x2) => x1.WearOrder.CompareTo(x2.WearOrder));
		Owner.OutfitsChanged = true;
		return newItem;
	}

	public void RemoveItem(long id)
	{
		_items.RemoveAll(x => x.Id == id);
		for (var i = 0; i < _items.Count; i++)
		{
			_items[i].WearOrder = i;
		}

		_items.Sort((x1, x2) => x1.WearOrder.CompareTo(x2.WearOrder));
		Owner.OutfitsChanged = true;
	}

	public XElement SaveToXml()
	{
		return new XElement("Outfit",
			new XElement("Name", new XCData(_name)),
			new XElement("Description", new XCData(_description)),
			new XElement("Exclusivity", (int)_exclusivity),
			new XElement("Items",
				from item in _items
				select item.SaveToXml()
			)
		);
	}

	public IOutfit CopyOutfit(ICharacter newOwner, string newName)
	{
		return new Outfit(newOwner, this, newName);
	}

	public string Show(ICharacter voyeur)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Outfit [{Name.Colour(Telnet.Cyan)}]");
		sb.AppendLine($"Exclusivity: {Exclusivity.Describe().Colour(Telnet.Green)}");
		sb.AppendLine($"Description: {Description.Fullstop()}");
		sb.AppendLine();
		if (voyeur.IsAdministrator())
		{
			foreach (var item in Items.OrderBy(x => x.WearOrder))
			{
				sb.AppendLine(
					$"{item.ItemDescription.Colour(Telnet.Green)} (#{item.Id.ToString("N0", voyeur)}){(item.PreferredContainerId.HasValue ? $" [{item.PreferredContainerDescription.Colour(Telnet.Green)} (#{item.PreferredContainerId.Value.ToString("N0", voyeur)})]" : "")}");
			}
		}
		else
		{
			foreach (var item in Items.OrderBy(x => x.WearOrder))
			{
				sb.AppendLine(
					$"{item.ItemDescription.Colour(Telnet.Green)}{(item.PreferredContainerId.HasValue ? $" [{item.PreferredContainerDescription.Colour(Telnet.Green)}]" : "")}");
			}
		}

		return sb.ToString();
	}

	public bool BuildingCommand(ICharacter builder, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "name":
				return BuildingCommandName(builder, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(builder, command);
			case "exclusivity":
			case "exclusive":
				return BuildingCommandExclusivity(builder, command);
			case "add":
				return BuildingCommandAdd(builder, command);
			case "addworn":
				return BuildingCommandAddWorn(builder, command);
			case "remove":
				return BuildingCommandRemove(builder, command);
			case "container":
				return BuildingCommandContainer(builder, command);
			case "swap":
				return BuildingCommandSwap(builder, command);
			default:
				builder.OutputHandler.Send(
					"That is not a valid option. Valid options are NAME, DESCRIPTION, EXCLUSIVITY, ADD, ADDWORN, REMOVE, CONTAINER, SWAP.");
				return false;
		}
	}

	private bool BuildingCommandAddWorn(ICharacter builder, StringStack command)
	{
		IGameItem container = null;
		if (!command.IsFinished)
		{
			container = builder.TargetItem(command.PopSpeech());
			if (container == null)
			{
				builder.OutputHandler.Send("There is no such container to prefer to store those items in.");
				return false;
			}
		}

		foreach (var item in builder.Body.DirectWornItems)
		{
			if (!_items.Any(x => x.Id == item.Id))
			{
				_items.Add(new OutfitItem
				{
					Id = item.Id,
					DesiredProfile = item.GetItemType<IWearable>().CurrentProfile,
					ItemDescription = item.HowSeen(builder, colour: false),
					PreferredContainerId = container?.Id,
					PreferredContainerDescription = container?.HowSeen(builder, colour: false),
					WearOrder = 0
				});
			}
			else
			{
				// Move it to the back of the list to keep the order consistent with worn order
				var entry = _items.First(x => x.Id == item.Id);
				_items.Remove(entry);
				_items.Add(entry);
			}
		}

		for (var i = 0; i < _items.Count; i++)
		{
			_items[i].WearOrder = i;
		}

		_items.Sort((x1, x2) => x1.WearOrder.CompareTo(x2.WearOrder));
		builder.OutputHandler.Send(
			$"You add all the items that you are currently wearing to this outfit{(container == null ? "" : $", with a preference to store them in container {container.HowSeen(builder)}")}.");
		Owner.OutfitsChanged = true;
		return true;
	}

	private bool BuildingCommandName(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What name do you want to rename this outfit to?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Owner.Outfits.Except(this).Any(x => x.Name.EqualTo(name)))
		{
			builder.OutputHandler.Send(
				$"There is already an outfit with the name {name.Colour(Telnet.Cyan)}. Outfit names must be unique.");
			return false;
		}

		builder.OutputHandler.Send($"You rename the outfit {_name.Colour(Telnet.Cyan)} to {name.Colour(Telnet.Cyan)}.");
		_name = name;
		Owner.OutfitsChanged = true;
		return true;
	}

	private bool BuildingCommandDescription(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What description do you want to give this outfit?");
			return false;
		}

		Description = command.SafeRemainingArgument;
		builder.OutputHandler.Send(
			$"You change the description of outfit {_name.Colour(Telnet.Cyan)} to: {_description.Colour(Telnet.Yellow)}.");
		Owner.OutfitsChanged = true;
		return true;
	}

	private bool BuildingCommandExclusivity(ICharacter builder, StringStack command)
	{
		if (command.IsFinished || command.Peek().EqualToAny("?", "help"))
		{
			builder.OutputHandler.Send(
				"The valid options are NONE, BELOW, and ALL. NONE will not remove any other items. BELOW will remove any items that are already worn below the existing outfit. ALL will remove all other items that are not part of the outfit.");
			return true;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "none":
				Exclusivity = OutfitExclusivity.NonExclusive;
				Owner.OutfitsChanged = true;
				builder.OutputHandler.Send("Wearing this outfit will no longer remove any other items that are worn.");
				return true;
			case "below":
				Exclusivity = OutfitExclusivity.ExcludeItemsBelow;
				Owner.OutfitsChanged = true;
				builder.OutputHandler.Send(
					"Wearing this outfit will now remove any items that would be below the outfit.");
				return true;
			case "all":
				Exclusivity = OutfitExclusivity.ExcludeAllItems;
				Owner.OutfitsChanged = true;
				builder.OutputHandler.Send(
					"Wearing this outfit will now remove all other items that are not part of the outfit.");
				return true;
			default:
				builder.OutputHandler.Send(
					"The valid options are NONE, BELOW, and ALL. NONE will not remove any other items. BELOW will remove any items that are already worn below the existing outfit. ALL will remove all other items that are not part of the outfit.");
				return true;
		}
	}

	private bool BuildingCommandAdd(ICharacter builder, StringStack command)
	{
		if (command.IsFinished || command.Peek().EqualToAny("?", "help"))
		{
			builder.OutputHandler.Send(
				"The correct syntax is OUTFIT SET ADD <item> [<wearprofile>] [*<container>], e.g. OUTFIT SET ADD red.scarf head *wardrobe");
			return false;
		}

		var item = builder.TargetItem(command.PopSpeech());
		if (item == null)
		{
			builder.OutputHandler.Send("You don't see anything like that.");
			return false;
		}

		if (_items.Any(x => x.Id == item.Id))
		{
			builder.OutputHandler.Send($"{item.HowSeen(builder, true)} is already a part of that outfit.");
			return false;
		}

		var wearable = item.GetItemType<IWearable>();
		if (wearable == null)
		{
			builder.OutputHandler.Send($"{item.HowSeen(builder, true)} is not something that can be worn.");
			return false;
		}

		IGameItem container = null;
		IWearProfile wearProfile = null;

		while (!command.IsFinished)
		{
			var cmd = command.PopSpeech();
			if (cmd[0] == '*')
			{
				container = builder.TargetItem(cmd.Substring(1));
				if (container == null)
				{
					builder.OutputHandler.Send("There is no such container to prefer to store that item in.");
					return false;
				}
			}
			else
			{
				wearProfile = wearable.Profiles.FirstOrDefault(x => x.Name.EqualTo(cmd)) ??
				              wearable.Profiles.FirstOrDefault(x => x.Name.StartsWith(cmd));
				if (wearProfile == null)
				{
					builder.OutputHandler.Send($"There is no such wear profile for {item.HowSeen(builder)}.");
					return false;
				}
			}
		}

		_items.Add(new OutfitItem
		{
			Id = item.Id,
			DesiredProfile = wearProfile,
			ItemDescription = item.HowSeen(builder, colour: false),
			PreferredContainerId = container?.Id,
			PreferredContainerDescription = container?.HowSeen(builder, colour: false),
			WearOrder = 0
		});
		for (var i = 0; i < _items.Count; i++)
		{
			_items[i].WearOrder = i;
		}

		_items.Sort((x1, x2) => x1.WearOrder.CompareTo(x2.WearOrder));
		builder.OutputHandler.Send(
			$"You add {item.HowSeen(builder)}{(wearProfile != null ? $" in profile {wearProfile.Name.TitleCase().Colour(Telnet.Cyan)}" : "")} to the outfit{(container != null ? $" (preferred container {container.HowSeen(builder)})" : "")}.");
		builder.OutfitsChanged = true;
		return true;
	}

	private bool BuildingCommandRemove(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which item do you want to remove from the outfit?");
			return false;
		}

		var item = Items.GetFromItemListByKeyword(command.PopSpeech(), builder);
		if (item == null)
		{
			builder.OutputHandler.Send("There is no such item associated with this outfit.");
			return false;
		}

		_items.Remove(item);
		for (var i = 0; i < _items.Count; i++)
		{
			_items[i].WearOrder = i;
		}

		_items.Sort((x1, x2) => x1.WearOrder.CompareTo(x2.WearOrder));
		Owner.OutfitsChanged = true;
		builder.OutputHandler.Send(
			$"The item {item.ItemDescription.Colour(Telnet.Green)} is no longer associated with this outfit.");
		return true;
	}

	private bool BuildingCommandContainer(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("Which item do you want to change the container settings for?");
			return false;
		}

		var item = Items.GetFromItemListByKeyword(command.PopSpeech(), builder);
		if (item == null)
		{
			builder.OutputHandler.Send("There is no such item associated with this outfit.");
			return false;
		}

		if (command.IsFinished)
		{
			builder.OutputHandler.Send(
				"You must specify a new container, or use CLEAR to clear an existing container.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("clear"))
		{
			item.PreferredContainerId = null;
			item.PreferredContainerDescription = string.Empty;
			Owner.OutfitsChanged = true;
			builder.OutputHandler.Send(
				$"The item {item.ItemDescription.Colour(Telnet.Green)} will no longer have a preferred container.");
			return true;
		}

		var container = builder.TargetItem(command.PopSpeech());
		if (container == null)
		{
			builder.OutputHandler.Send("You do not see anything like that.");
			return false;
		}

		if (!container.IsItemType<IContainer>())
		{
			builder.OutputHandler.Send($"{container.HowSeen(builder, true)} is not a container.");
			return false;
		}

		item.PreferredContainerDescription = container.HowSeen(builder, colour: false);
		item.PreferredContainerId = container.Id;
		Owner.OutfitsChanged = true;
		builder.OutputHandler.Send(
			$"The item {item.ItemDescription.Colour(Telnet.Green)} will now preferably be put back into container {item.PreferredContainerDescription.Colour(Telnet.Green)}.");
		return true;
	}

	private bool BuildingCommandSwap(ICharacter builder, StringStack command)
	{
		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What is the first item you want to swap?");
			return false;
		}

		var item1 = Items.GetFromItemListByKeyword(command.PopSpeech(), builder);
		if (item1 == null)
		{
			builder.OutputHandler.Send("There is no such item associated with this outfit.");
			return false;
		}

		if (command.IsFinished)
		{
			builder.OutputHandler.Send("What is the second item you want to swap?");
			return false;
		}

		var item2 = Items.GetFromItemListByKeyword(command.PopSpeech(), builder);
		if (item2 == null)
		{
			builder.OutputHandler.Send("There is no such item associated with this outfit.");
			return false;
		}

		if (item1 == item2)
		{
			builder.OutputHandler.Send("You cannot swap an item with itself.");
			return false;
		}

		_items.Swap(item1, item2);
		for (var i = 0; i < _items.Count; i++)
		{
			_items[i].WearOrder = i;
		}

		_items.Sort((x1, x2) => x1.WearOrder.CompareTo(x2.WearOrder));

		builder.OutputHandler.Send(
			$"You swap the positions of {item1.ItemDescription.ColourObject()} and {item2.ItemDescription.ColourObject()} in the wear order list.");
		Owner.OutfitsChanged = true;
		return true;
	}

	public void SwapItems(IOutfitItem item1, IOutfitItem item2)
	{
		_items.Swap(item1, item2);
		for (var i = 0; i < _items.Count; i++)
		{
			_items[i].WearOrder = i;
		}

		_items.Sort((x1, x2) => x1.WearOrder.CompareTo(x2.WearOrder));
		Owner.OutfitsChanged = true;
	}

	#region IFutureProgVariable Implementation

	public FutureProgVariableTypes Type => FutureProgVariableTypes.Outfit;
	public object GetObject => this;

	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "owner":
				return Owner;
			case "name":
				return new TextVariable(Name);
			case "desc":
			case "description":
				return new TextVariable(Description);
			case "items":
				return new CollectionVariable(_items.ToList(), FutureProgVariableTypes.OutfitItem);
		}

		throw new NotImplementedException();
	}

	private static FutureProgVariableTypes DotReferenceHandler(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "owner":
				return FutureProgVariableTypes.Character;
			case "name":
				return FutureProgVariableTypes.Text;
			case "desc":
				return FutureProgVariableTypes.Text;
			case "description":
				return FutureProgVariableTypes.Text;
			case "items":
				return FutureProgVariableTypes.OutfitItem | FutureProgVariableTypes.Collection;
			default:
				return FutureProgVariableTypes.Error;
		}
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "owner", FutureProgVariableTypes.Character },
			{ "name", FutureProgVariableTypes.Text },
			{ "desc", FutureProgVariableTypes.Text },
			{ "description", FutureProgVariableTypes.Text },
			{ "items", FutureProgVariableTypes.OutfitItem | FutureProgVariableTypes.Collection }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "owner", "" },
			{ "name", "" },
			{ "desc", "" },
			{ "description", "" },
			{ "items", "" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Outfit, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}
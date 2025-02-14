using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;

namespace MudSharp.Work.Crafts.Products;

public class SimpleProduct : BaseProduct
{
	protected SimpleProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		Quantity = int.Parse(root.Element("Quantity")?.Value ?? "1");
		ProductProducedId = long.Parse(root.Element("ProductProducedId").Value);
		Skin = gameworld.ItemSkins.Get(long.Parse(root.Element("Skin")?.Value ?? "0"));
	}

	protected SimpleProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
		Quantity = 1;
	}

	public long ProductProducedId { get; set; }
	public int Quantity { get; set; }
	public IGameItemSkin Skin { get; set; }

	/// <inheritdoc />
	public override bool IsItem(IGameItem item)
	{
		return ProductProducedId == item.Prototype.Id && Skin?.Id == item.Skin?.Id;
	}

	/// <inheritdoc />
	public override bool RefersToItemProto(long id)
	{
		return ProductProducedId == id;
	}

	#region Overrides of BaseProduct

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var proto = Gameworld.ItemProtos.Get(ProductProducedId);
		if (proto == null)
		{
			throw new ApplicationException("Couldn't find a valid proto for craft product to load.");
		}

		var material = DetermineOverrideMaterial(component);

		if (Quantity > 1 && proto.IsItemType<StackableGameItemComponentProto>())
		{
			var newItem = proto.CreateNew();
			newItem.Skin = Skin;
			newItem.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(newItem);

			if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
			{
				newItem.Quality = referenceQuality;
			}

			if (material != null)
			{
				newItem.Material = material;
			}

			newItem.GetItemType<IStackable>().Quantity = Quantity;
			newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
			return new SimpleProductData(new[] { newItem });
		}

		var items = new List<IGameItem>();
		for (var i = 0; i < Quantity; i++)
		{
			var item = proto.CreateNew();
			item.Skin = Skin;
			item.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(item);
			
			if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
			{
				item.Quality = referenceQuality;
			}

			if (material != null)
			{
				item.Material = material;
			}
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			items.Add(item);
		}

		return new SimpleProductData(items);
	}

	public override string ProductType => "SimpleProduct";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ProductProducedId", ProductProducedId),
			new XElement("Quantity", Quantity),
			new XElement("Skin", Skin?.Id ?? 0)
		).ToString();
	}

	#endregion

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("SimpleProduct",
			(product, craft, game) => new SimpleProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("simple",
			(craft, game, fail) => new SimpleProduct(craft, game, fail));
	}

	public override string Name =>
		$"{Quantity}x {Skin?.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ?? "an unspecified item".Colour(Telnet.Red)}";

	public override string HowSeen(IPerceiver voyeur)
	{
		if (Skin is not null && voyeur is ICharacter ch && ch.IsAdministrator())
		{
			return
				$"{Quantity}x {(Skin?.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription).ConcatIfNotEmpty($" [re-skinned: #{Skin.Id.ToString("N0", voyeur)}]") ?? "an unspecified item".Colour(Telnet.Red)}";
		}

		return
			$"{Quantity}x {Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ?? "an unspecified item".Colour(Telnet.Red)}";
	}

	protected override string BuildingHelpText => $@"{base.BuildingHelpText}
	#3item <id|name>#0 - sets the item prototype to be loaded
	#3skin <id|name>#0 - sets a skin to go with the item prototype
	#3skin clear#0 - clears the skin
	#3quantity <amount>#0 - sets the quantity of that item to be loaded";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "item":
				return BuildingCommandItem(actor, command);
			case "quantity":
			case "amount":
			case "number":
			case "num":
				return BuildingCommandQuantity(actor, command);
			case "skin":
				return BuildingCommandSkin(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandSkin(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify a skin to go with this product or use {"clear".ColourCommand()} to clear an existing one.");
			return false;
		}

		if (ProductProducedId == 0)
		{
			actor.OutputHandler.Send("You must first set an item prototype before you can set any skins.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "delete", "remove", "none"))
		{
			Skin = null;
			ProductChanged = true;
			actor.OutputHandler.Send("This product will no longer apply any skin to the loaded item.");
			return true;
		}

		IGameItemSkin skin;
		if (long.TryParse(command.SafeRemainingArgument, out var value))
		{
			skin = Gameworld.ItemSkins.Get(value);
		}
		else
		{
			skin = Gameworld.ItemSkins.Where(x => x.ItemProto.Id == ProductProducedId)
			                .GetByNameOrAbbreviation(command.SafeRemainingArgument);
		}

		if (skin is null)
		{
			actor.OutputHandler.Send("There is no such item skin.");
			return false;
		}

		if (skin.ItemProto.Id != ProductProducedId)
		{
			actor.OutputHandler.Send(
				$"{skin.EditHeader().ColourName()} is not designed to work with {Gameworld.ItemProtos.Get(ProductProducedId).EditHeader().ColourObject()}.");
			return false;
		}

		if (skin.Status != RevisionStatus.Current)
		{
			actor.OutputHandler.Send($"{skin.EditHeader().ColourName()} is not approved for use.");
			return false;
		}

		Skin = skin;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now apply {skin.EditHeader().ColourName()} to the loaded item.");
		return true;
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid amount of this item to be loaded.");
			return false;
		}

		Quantity = value;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now produce {Quantity} of the target item.");
		return true;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What item prototype should this item input load?");
			return false;
		}

		var proto = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
		if (proto == null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		ProductProducedId = proto.Id;
		Skin = null;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now target item prototype #{ProductProducedId.ToString("N0", actor)} ({proto.ShortDescription.ColourObject()}).");
		return true;
	}

	public override bool IsValid()
	{
		return ProductProducedId != 0;
	}

	public override string WhyNotValid()
	{
		return "You must first set an item prototype for this product to load.";
	}
}
using MudSharp.Accounts;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Work.Crafts.Products;

public class LiquidProduct : BaseProduct
{
	protected LiquidProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		Quantity = int.Parse(root.Element("Quantity")?.Value ?? "1");
		ProductProducedId = long.Parse(root.Element("ProductProducedId")?.Value ?? "0");
		Liquid = gameworld.Liquids.Get(long.Parse(root.Element("Liquid")?.Value ?? "0"));
		LiquidVolume = double.Parse(root.Element("LiquidVolume")?.Value ?? "0.0");
		Skin = gameworld.ItemSkins.Get(long.Parse(root.Element("Skin")?.Value ?? "0"));
	}

	protected LiquidProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
		Quantity = 1;
	}

	public long ProductProducedId { get; set; }
	public int Quantity { get; set; }
	public ILiquid? Liquid { get; set; }
	public double LiquidVolume { get; set; }
	public IGameItemSkin? Skin { get; set; }

	public override string ProductType => "LiquidProduct";

	public override string Name =>
		$"{Quantity}x {Skin?.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ?? "an unspecified liquid container".Colour(Telnet.Red)} filled with {LiquidDescription(DummyAccount.Instance)}";

	public override bool RefersToItemProto(long id)
	{
		return ProductProducedId == id;
	}

	public override bool RefersToLiquid(ILiquid liquid)
	{
		return Liquid == liquid;
	}

	public override bool IsItem(IGameItem item)
	{
		if (ProductProducedId != item.Prototype.Id || Skin?.Id != item.Skin?.Id)
		{
			return false;
		}

		var container = item.GetItemType<ILiquidContainer>();
		if (container?.LiquidMixture is null || Liquid is null)
		{
			return false;
		}

		var countsAs = container.LiquidMixture.CountsAs(Liquid);
		return countsAs.Truth && container.LiquidVolume >= LiquidVolume;
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var proto = Gameworld.ItemProtos.Get(ProductProducedId);
		if (proto is null)
		{
			throw new ApplicationException("Couldn't find a valid proto for liquid craft product to load.");
		}

		var material = DetermineOverrideMaterial(component);
		var items = new List<IGameItem>();
		for (var i = 0; i < Quantity; i++)
		{
			var item = CreateOneItem(component, proto, referenceQuality, material);
			try
			{
				FillContainer(item);
			}
			catch
			{
				item.Delete();
				throw;
			}

			Gameworld.Add(item);
			items.Add(item);
		}

		return new SimpleProductData(items);
	}

	private IGameItem CreateOneItem(IActiveCraftGameItemComponent component, IGameItemProto proto, ItemQuality referenceQuality,
		ISolid? material)
	{
		var item = proto.CreateNew();
		item.Skin = Skin;
		item.RoomLayer = component.Parent.RoomLayer;
		if (!Gameworld.GetStaticBool("DisableCraftQualityCalculation"))
		{
			item.Quality = referenceQuality;
		}

		if (material is not null)
		{
			item.Material = material;
		}

		return item;
	}

	private void FillContainer(IGameItem item)
	{
		var container = item.GetItemType<ILiquidContainer>();
		if (container is null)
		{
			throw new ApplicationException(
				$"LiquidProduct target #{ProductProducedId:N0} ({item.Prototype.ShortDescription}) is not a liquid container.");
		}

		if (Liquid is null)
		{
			throw new ApplicationException("LiquidProduct did not have a valid liquid configured.");
		}

		if (LiquidVolume <= 0.0)
		{
			throw new ApplicationException("LiquidProduct did not have a positive liquid volume configured.");
		}

		var mixture = new LiquidMixture(Liquid, LiquidVolume, Gameworld);
		if (container.LiquidMixture?.CanMerge(mixture) == false)
		{
			throw new ApplicationException(
				$"LiquidProduct target #{ProductProducedId:N0} already contains a liquid that cannot mix with {Liquid.Name}.");
		}

		if (container.LiquidCapacity - container.LiquidVolume < LiquidVolume)
		{
			throw new ApplicationException(
				$"LiquidProduct target #{ProductProducedId:N0} only has {Gameworld.UnitManager.DescribeMostSignificantExact(container.LiquidCapacity - container.LiquidVolume, UnitType.FluidVolume, DummyAccount.Instance)} free, but needs {LiquidDescription(DummyAccount.Instance)}.");
		}

		container.MergeLiquid(mixture, null, "craft");
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		var itemText = Skin is not null && voyeur is ICharacter ch && ch.IsAdministrator()
			? $"{Skin.ShortDescription} [re-skinned: #{Skin.Id.ToString("N0", voyeur)}]"
			: Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ?? "an unspecified liquid container".Colour(Telnet.Red);
		return $"{Quantity}x {itemText} filled with {LiquidDescription(voyeur)}";
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ProductProducedId", ProductProducedId),
			new XElement("Quantity", Quantity),
			new XElement("Liquid", Liquid?.Id ?? 0),
			new XElement("LiquidVolume", LiquidVolume),
			new XElement("Skin", Skin?.Id ?? 0)
		).ToString();
	}

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("LiquidProduct",
			(product, craft, game) => new LiquidProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("liquid",
			(craft, game, fail) => new LiquidProduct(craft, game, fail));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("liquidproduct",
			(craft, game, fail) => new LiquidProduct(craft, game, fail));
	}

	protected override string BuildingHelpText => $@"{base.BuildingHelpText}
	#3item <id|name>#0 - sets the liquid-container item prototype to be loaded
	#3skin <id|name>#0 - sets a skin to go with the item prototype
	#3skin clear#0 - clears the skin
	#3quantity <amount>#0 - sets the number of containers to load
	#3liquid <id|name>#0 - sets the liquid to put in each container
	#3volume <amount>#0 - sets how much liquid goes in each container";

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
			case "liquid":
				return BuildingCommandLiquid(actor, command);
			case "volume":
			case "vol":
				return BuildingCommandVolume(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What liquid-container item prototype should this product load?");
			return false;
		}

		var proto = Gameworld.ItemProtos.GetByIdOrUniqueNameOrName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (!proto.IsItemType<ILiquidContainerPrototype>())
		{
			actor.OutputHandler.Send("That item prototype does not have a liquid container component.");
			return false;
		}

		ProductProducedId = proto.Id;
		Skin = null;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now target liquid-container prototype #{ProductProducedId.ToString("N0", actor)} ({proto.ShortDescription.ColourObject()}).");
		return true;
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid number of containers to load.");
			return false;
		}

		Quantity = value;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now produce {Quantity.ToString("N0", actor).ColourValue()} containers.");
		return true;
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

		var skin = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ItemSkins.Get(value)
			: Gameworld.ItemSkins.Where(x => x.ItemProto.Id == ProductProducedId)
			           .GetByNameOrAbbreviation(command.SafeRemainingArgument);
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

	private bool BuildingCommandLiquid(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which liquid should this product put in each container?");
			return false;
		}

		var liquid = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.Liquids.Get(value)
			: Gameworld.Liquids.GetByIdOrName(command.SafeRemainingArgument);
		if (liquid is null)
		{
			actor.OutputHandler.Send("There is no such liquid.");
			return false;
		}

		Liquid = liquid;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now fill containers with {liquid.Name.Colour(liquid.DisplayColour)}.");
		return true;
	}

	private bool BuildingCommandVolume(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much liquid should go into each container?");
			return false;
		}

		var amount = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var success);
		if (!success || amount <= 0.0)
		{
			actor.OutputHandler.Send("That is not a valid positive liquid volume.");
			return false;
		}

		LiquidVolume = amount;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now fill each container with {LiquidDescription(actor).ColourValue()}.");
		return true;
	}

	public override bool IsValid()
	{
		return ProductProducedId != 0 &&
		       Liquid is not null &&
		       LiquidVolume > 0.0 &&
		       Gameworld.ItemProtos.Get(ProductProducedId)?.IsItemType<ILiquidContainerPrototype>() == true;
	}

	public override string WhyNotValid()
	{
		if (ProductProducedId == 0)
		{
			return "You must first set a liquid-container item prototype for this product to load.";
		}

		if (Gameworld.ItemProtos.Get(ProductProducedId)?.IsItemType<ILiquidContainerPrototype>() != true)
		{
			return "The selected item prototype must have a liquid container component.";
		}

		if (Liquid is null)
		{
			return "You must first set a liquid for this product to load.";
		}

		if (LiquidVolume <= 0.0)
		{
			return "You must set a positive liquid volume for this product to load.";
		}

		throw new ApplicationException("Unknown WhyNotValid reason in LiquidProduct.");
	}

	private string LiquidDescription(IAccount voyeur)
	{
		return Liquid is null
			? "an unspecified liquid".Colour(Telnet.Red)
			: $"{Gameworld.UnitManager.DescribeMostSignificantExact(LiquidVolume, UnitType.FluidVolume, voyeur)} of {Liquid.Name.Colour(Liquid.DisplayColour)}";
	}

	private string LiquidDescription(IPerceiver voyeur)
	{
		return Liquid is null
			? "an unspecified liquid".Colour(Telnet.Red)
			: $"{Gameworld.UnitManager.DescribeMostSignificantExact(LiquidVolume, UnitType.FluidVolume, voyeur)} of {Liquid.Name.Colour(Liquid.DisplayColour)}";
	}
}

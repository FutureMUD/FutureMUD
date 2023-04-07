using System;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;

namespace MudSharp.Work.Crafts.Products;

public class CommodityProduct : BaseProduct
{
	public ISolid Material { get; set; }
	public double Weight { get; set; }
	public ITag Tag { get; set; }

	protected CommodityProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
		gameworld)
	{
		var root = XElement.Parse(product.Definition);
		Weight = double.Parse(root.Element("Weight")?.Value ?? "0");
		Material = Gameworld.Materials.Get(long.Parse(root.Element("Material")?.Value ?? "0"));
		Tag = Gameworld.Tags.Get(long.Parse(root.Element("Tag")?.Value ?? "0"));
	}

	protected CommodityProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld,
		failproduct)
	{
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Material", Material?.Id ?? 0),
			new XElement("Weight", Weight),
			new XElement("Tag", Tag?.Id ?? 0)
		).ToString();
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var newitem = GameItems.Prototypes.CommodityGameItemComponentProto.CreateNewCommodity(Material, Weight, Tag);
		newitem.RoomLayer = component.Parent.RoomLayer;
		Gameworld.Add(newitem);
		return new SimpleProductData(new[]
		{
			newitem
		});
	}

	public override string ProductType => "CommodityProduct";

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("CommodityProduct",
			(product, craft, game) => new CommodityProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("commodity",
			(craft, game, fail) => new CommodityProduct(craft, game, fail));
	}

	protected override string BuildingHelpText => $@"{base.BuildingHelpText}
	#3commodity <material>#0 - sets the target material
	#3weight <weight>#0 - sets the required weight of material
	#3tag <which>|none#0 - sets or clears the tag of the commodity";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "commodity":
			case "commoditymaterial":
			case "commoditymat":
			case "commodity mat":
			case "commodity_mat":
			case "commodity material":
			case "commodity_material":
				return BuildingCommandMaterial(actor, command);
			case "weight":
			case "amount":
			case "quantity":
				return BuildingCommandQuantity(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a tag for the commodity or use {"none".ColourCommand()} to clear it.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			Tag = null;
			Changed = true;
			actor.OutputHandler.Send("This product will now not set any tag on the commodity pile.");
			return true;
		}

		var tag = Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
		if (tag is null)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		Tag = tag;
		Changed = true;
		actor.OutputHandler.Send(
			$"This product will set the {Tag.FullName.ColourName()} tag on the loaded commodity pile.");
		return true;
	}

	private bool BuildingCommandQuantity(ICharacter actor, StringStack command)
	{
		if (Material == null)
		{
			actor.OutputHandler.Send("You must first set a material before you set a weight.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much of the material do you want this product to produce?");
			return false;
		}

		var amount = Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, Framework.Units.UnitType.Mass,
			out var success);
		if (!success)
		{
			actor.OutputHandler.Send("That is not a valid weight.");
			return false;
		}

		Weight = amount;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now produce {Gameworld.UnitManager.DescribeExact(Weight, Framework.Units.UnitType.Mass, actor).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}.");
		return true;
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material did you want this product to produce?");
			return false;
		}

		var material = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.Materials.Get(value)
			: Gameworld.Materials.GetByName(command.Last);
		if (material == null)
		{
			actor.OutputHandler.Send("There is no such material.");
			return false;
		}

		Material = material;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now produce the {Material.Name.Colour(Material.ResidueColour)} material.");
		return true;
	}

	public override bool IsValid()
	{
		return Material != null && Weight > 0.0;
	}

	public override string WhyNotValid()
	{
		if (Material == null)
		{
			return "You must first set a material for this product to produce.";
		}

		if (Weight <= 0.0)
		{
			return "You must set a positive weight of material for this product to produce.";
		}

		throw new ApplicationException("Unknown WhyNotValid reason in CommodityProduct.");
	}

	public override string Name => Material != null
		? $"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, DummyAccount.Instance).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}{(Tag is not null ? $" {Tag.Name.Pluralise()}" : "")} commodity"
		: "An unspecified amount of an unspecified commodity";

	public override string HowSeen(IPerceiver voyeur)
	{
		if (Material != null)
		{
			return
				$"{Gameworld.UnitManager.DescribeMostSignificantExact(Weight, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}{(Tag is not null ? $" {Tag.Name.Pluralise()}" : "")} commodity";
		}

		return "An unspecified amount of an unspecified commodity";
	}
}
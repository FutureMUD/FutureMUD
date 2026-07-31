using C5;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;

#nullable enable

namespace MudSharp.Work.Crafts.Products;

/// <summary>
/// A cooked-food product whose FutureProg selects one or more prepared-food prototypes.
/// The selected prototypes are created through <see cref="CookedFoodProduct"/>, so the
/// consumed-input ingredient ledger is applied consistently to every result.
/// </summary>
public sealed class ProgCookedFoodProduct : CookedFoodProduct
{
	private ProgCookedFoodProduct(CraftProduct product, ICraft craft, IFuturemud gameworld)
		: base(product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		ItemProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ItemProg")?.Value ?? "0"));
	}

	private ProgCookedFoodProduct(ICraft craft, IFuturemud gameworld, bool failproduct)
		: base(craft, gameworld, failproduct)
	{
	}

	public IFutureProg? ItemProg { get; private set; }

	public override string ProductType => "ProgCookedFoodProduct";

	public override string Name =>
		$"cooked food selected by the {ItemProg?.MXPClickableFunctionName() ?? "Unknown".ColourError()} prog";

	public static new void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("ProgCookedFoodProduct",
			(product, craft, game) => new ProgCookedFoodProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("progcookedfood",
			(craft, game, fail) => new ProgCookedFoodProduct(craft, game, fail));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("prog cooked food",
			(craft, game, fail) => new ProgCookedFoodProduct(craft, game, fail));
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		if (ItemProg is null)
		{
			return new SimpleProductData([]);
		}

		CollectionDictionary<string, IGameItem> items = new();
		CollectionDictionary<string, LiquidMixture> liquids = new();
		var inputList = Craft.Inputs.ToList();
		foreach (var input in Craft.Inputs)
		{
			var inputNumber = inputList.IndexOf(input) + 1;
			if (!component.ConsumedInputs.TryGetValue(input, out var consumed))
			{
				continue;
			}

			items.AddRange(inputNumber.ToString("F0"), ExtractItems(input, consumed.Data));

			if (consumed.Data is ICraftInputConsumeLiquidData liquidData)
			{
				liquids.Add(inputNumber.ToString("F0"), liquidData.ConsumedMixture);
			}
		}

		IEnumerable<IGameItem> selected = ItemProg.ReturnType.CompatibleWith(ProgVariableTypes.Collection | ProgVariableTypes.Item)
			? ItemProg.ExecuteCollection<IGameItem>(items, liquids) ?? []
			: new[] { ItemProg.Execute<IGameItem>(items, liquids) }.OfType<IGameItem>();
		var products = new List<IGameItem>();
		var consumedItems = component.ConsumedInputs
			.SelectMany(x => ExtractItems(x.Key, x.Value.Data))
			.ToHashSet();
		foreach (var selectedItem in selected)
		{
			try
			{
				if (selectedItem.Prototype.IsItemType<PreparedFoodGameItemComponentProto>())
				{
					if (ProduceProductForPrototype(component, referenceQuality, selectedItem.Prototype)
						is ICraftProductDataWithItems produced)
					{
						products.AddRange(produced.Products);
					}
				}
			}
			finally
			{
				if (!consumedItems.Contains(selectedItem))
				{
					selectedItem.Delete();
				}
			}
		}

		return new SimpleProductData(products);
	}

	public override bool IsItem(IGameItem item) => false;

	public override bool RefersToItemProto(long id) => false;

	public override string HowSeen(IPerceiver voyeur) =>
		$"cooked food selected by the {ItemProg?.MXPClickableFunctionName() ?? "Unknown".ColourError()} prog";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ItemProg", ItemProg?.Id ?? 0),
			new XElement("Quantity", Quantity),
			new XElement("Skin", Skin?.Id ?? 0),
			new XElement("RemoveDrugsAndFoodEffects", RemoveDrugsAndFoodEffects),
			new XElement("IngredientSlots",
				IngredientSlots.Select(x => new XElement("Slot",
					new XAttribute("input", x.InputId),
					new XAttribute("role", x.Role))))
		).ToString();
	}

	protected override string SaveDefinitionForRevision(Dictionary<long, long> inputIdMap, Dictionary<long, long> toolIdMap)
	{
		return new XElement("Definition",
			new XElement("ItemProg", ItemProg?.Id ?? 0),
			new XElement("Quantity", Quantity),
			new XElement("Skin", Skin?.Id ?? 0),
			new XElement("RemoveDrugsAndFoodEffects", RemoveDrugsAndFoodEffects),
			new XElement("IngredientSlots",
				IngredientSlots.Select(x => new XElement("Slot",
					new XAttribute("input", inputIdMap.ValueOrDefault(x.InputId, x.InputId)),
					new XAttribute("role", x.Role))))
		).ToString();
	}

	public override bool IsValid()
	{
		return ItemProg is not null &&
			(ItemProg.ReturnType.CompatibleWith(ProgVariableTypes.Item) ||
			 ItemProg.ReturnType.CompatibleWith(ProgVariableTypes.Collection | ProgVariableTypes.Item));
	}

	public override string WhyNotValid()
	{
		if (ItemProg is null)
		{
			return "You must set an item-selection prog.";
		}

		return IsValid()
			? string.Empty
			: "The item-selection prog must return a prepared-food item or a collection of prepared-food items.";
	}

	protected override string BuildingHelpText => @"This product uses a FutureProg to select one prepared-food prototype from the catalogue group.
	#3prog <prog>#0 - sets the prog that selects the prepared-food prototype
	#3purify [on|off]#0 - toggles whether input drugs and transferable food effects are removed
	#3ingredient add <input index> <role>#0 - maps a consumed craft input into the food ledger with a role
	#3ingredient clear#0 - uses all consumed inputs as generic ingredients";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		if (!command.IsFinished && command.PeekSpeech().EqualToAny("item", "quantity", "amount", "number", "num", "skin"))
		{
			actor.OutputHandler.Send("This product's selector prog determines the prepared-food prototype; item, quantity, and skin settings are not applicable.");
			return false;
		}

		if (command.PopForSwitch() is "prog")
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send("You must specify an item-selection prog.");
				return false;
			}

			var prog = new ProgLookupFromBuilderInputMultipleReturnTypes(actor, command.SafeRemainingArgument,
				[ProgVariableTypes.Item, ProgVariableTypes.Collection | ProgVariableTypes.Item],
				[
					[ProgVariableTypes.CollectionDictionary | ProgVariableTypes.Item],
					[ProgVariableTypes.CollectionDictionary | ProgVariableTypes.Item,
						ProgVariableTypes.CollectionDictionary | ProgVariableTypes.LiquidMixture]
				]).LookupProg();
			if (prog is null)
			{
				return false;
			}

			ItemProg = prog;
			ProductChanged = true;
			actor.OutputHandler.Send($"This product will now use the {prog.MXPClickableFunctionName()} prog to select prepared-food prototypes.");
			return true;
		}

		return base.BuildingCommand(actor, command);
	}
}

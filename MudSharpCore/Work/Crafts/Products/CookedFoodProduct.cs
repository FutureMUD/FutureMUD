using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Work.Crafts.Products;

public class CookedFoodProduct : BaseProduct
{
	private readonly List<(long InputId, string Role)> _ingredientSlots = new();

	protected CookedFoodProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		Quantity = int.Parse(root.Element("Quantity")?.Value ?? "1");
		ProductProducedId = long.Parse(root.Element("ProductProducedId")?.Value ?? "0");
		Skin = gameworld.ItemSkins.Get(long.Parse(root.Element("Skin")?.Value ?? "0"));
		RemoveDrugsAndFoodEffects =
			bool.TryParse(root.Element("RemoveDrugsAndFoodEffects")?.Value, out var removeEffects) && removeEffects;
		foreach (var element in root.Element("IngredientSlots")?.Elements("Slot") ?? Enumerable.Empty<XElement>())
		{
			_ingredientSlots.Add((long.Parse(element.Attribute("input")?.Value ?? "0"), element.Attribute("role")?.Value ?? "ingredient"));
		}
	}

	protected CookedFoodProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
		Quantity = 1;
	}

	public long ProductProducedId { get; set; }
	public int Quantity { get; set; }
	public IGameItemSkin? Skin { get; set; }
	public bool RemoveDrugsAndFoodEffects { get; set; }

	public override string ProductType => "CookedFoodProduct";

	public override string Name =>
		$"{Quantity}x cooked {Skin?.ShortDescription ?? Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ?? "an unspecified prepared food".Colour(Telnet.Red)}";

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("CookedFoodProduct",
			(product, craft, game) => new CookedFoodProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("cookedfood",
			(craft, game, fail) => new CookedFoodProduct(craft, game, fail));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("cooked food",
			(craft, game, fail) => new CookedFoodProduct(craft, game, fail));
	}

	public override bool IsItem(IGameItem item)
	{
		return ProductProducedId == item.Prototype.Id && Skin?.Id == item.Skin?.Id;
	}

	public override bool RefersToItemProto(long id)
	{
		return ProductProducedId == id;
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var proto = Gameworld.ItemProtos.Get(ProductProducedId);
		if (proto is null)
		{
			throw new ApplicationException("Couldn't find a valid proto for cooked food craft product to load.");
		}

		var material = DetermineOverrideMaterial(component);
		if (Quantity > 1 && proto.IsItemType<StackableGameItemComponentProto>())
		{
			var item = CreateOneItem(component, proto, referenceQuality, material);
			item.GetItemType<IStackable>().Quantity = Quantity;
			InitialisePreparedFood(component, item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
			return new SimpleProductData(new[] { item });
		}

		var items = new List<IGameItem>();
		for (var i = 0; i < Quantity; i++)
		{
			var item = CreateOneItem(component, proto, referenceQuality, material);
			InitialisePreparedFood(component, item);
			item.HandleEvent(EventType.ItemFinishedLoading, item);
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
		Gameworld.Add(item);
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

	private void InitialisePreparedFood(IActiveCraftGameItemComponent component, IGameItem item)
	{
		var prepared = item.GetItemType<IPreparedFood>();
		if (prepared is null)
		{
			return;
		}

		foreach (var entry in SelectedInputData(component))
		{
			if (entry.Data is ICraftInputConsumeLiquidData liquidData)
			{
				prepared.AbsorbLiquid(liquidData.ConsumedMixture.Clone(), entry.Role, !RemoveDrugsAndFoodEffects);
			}

			foreach (var inputItem in ExtractItems(entry.Input, entry.Data))
			{
				TransferItemToFood(inputItem, prepared, entry.Role);
			}
		}
	}

	private IEnumerable<(ICraftInput Input, ICraftInputData Data, string Role)> SelectedInputData(IActiveCraftGameItemComponent component)
	{
		foreach (var pair in component.ConsumedInputs)
		{
			if (_ingredientSlots.Count == 0)
			{
				yield return (pair.Key, pair.Value.Data, "ingredient");
				continue;
			}

			foreach (var slot in _ingredientSlots.Where(x => x.InputId == pair.Key.Id))
			{
				yield return (pair.Key, pair.Value.Data, slot.Role);
			}
		}
	}

	private static IEnumerable<IGameItem> ExtractItems(ICraftInput input, ICraftInputData data)
	{
		if (data is ICraftInputDataWithItems itemData)
		{
			return itemData.ConsumedItems;
		}

		return data.Perceivable switch
		{
			IGameItem item => new[] { item },
			PerceivableGroup group => group.Members.OfType<IGameItem>(),
			_ => Enumerable.Empty<IGameItem>()
		};
	}

	private void TransferItemToFood(IGameItem inputItem, IPreparedFood prepared, string role)
	{
		var servingMultiplier = inputItem.GetItemType<IPreparedFood>() is { ServingScope: FoodServingScope.PerStackUnit } ? inputItem.Quantity : 1;
		if (inputItem.GetItemType<IPreparedFood>() is { } preparedInput)
		{
			foreach (var ingredient in preparedInput.Ingredients)
			{
				var clone = ingredient.Clone();
				clone.Role = string.IsNullOrWhiteSpace(clone.Role) ? role : clone.Role;
				clone.Weight *= servingMultiplier;
				clone.Volume *= servingMultiplier;
				prepared.AddIngredient(clone);
			}

			if (!RemoveDrugsAndFoodEffects)
			{
				foreach (var dose in preparedInput.DrugDoses)
				{
					prepared.AddDrugDose(dose.Clone(servingMultiplier));
				}
			}
		}
		else
		{
			prepared.AddIngredient(new FoodIngredientInstance
			{
				Role = role,
				Description = inputItem.Prototype.ShortDescription,
				TasteText = inputItem.Prototype.ShortDescription,
				SourceItemProtoId = inputItem.Prototype.Id,
				MaterialId = inputItem.Material?.Id ?? 0,
				Weight = inputItem.Prototype.Weight * inputItem.Quantity,
				Quality = inputItem.Quality
			});
		}

		if (RemoveDrugsAndFoodEffects)
		{
			return;
		}

		foreach (var effect in inputItem.EffectsOfType<IIngredientTransferEffect>())
		{
			effect.TransferToFood(prepared, servingMultiplier);
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (Skin is not null && voyeur is ICharacter ch && ch.IsAdministrator())
		{
			return
				$"{Quantity}x {Skin.ShortDescription.ConcatIfNotEmpty($" [re-skinned: #{Skin.Id.ToString("N0", voyeur)}]")}";
		}

		return
			$"{Quantity}x {Gameworld.ItemProtos.Get(ProductProducedId)?.ShortDescription ?? "an unspecified prepared food".Colour(Telnet.Red)}";
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ProductProducedId", ProductProducedId),
			new XElement("Quantity", Quantity),
			new XElement("Skin", Skin?.Id ?? 0),
			new XElement("RemoveDrugsAndFoodEffects", RemoveDrugsAndFoodEffects),
			new XElement("IngredientSlots",
				_ingredientSlots.Select(x => new XElement("Slot",
					new XAttribute("input", x.InputId),
					new XAttribute("role", x.Role)
				))
			)
		).ToString();
	}

	protected override string SaveDefinitionForRevision(Dictionary<long, long> inputIdMap, Dictionary<long, long> toolIdMap)
	{
		return new XElement("Definition",
			new XElement("ProductProducedId", ProductProducedId),
			new XElement("Quantity", Quantity),
			new XElement("Skin", Skin?.Id ?? 0),
			new XElement("RemoveDrugsAndFoodEffects", RemoveDrugsAndFoodEffects),
			new XElement("IngredientSlots",
				_ingredientSlots.Select(x => new XElement("Slot",
					new XAttribute("input", inputIdMap.ValueOrDefault(x.InputId, x.InputId)),
					new XAttribute("role", x.Role)
				))
			)
		).ToString();
	}

	protected override string BuildingHelpText => $@"{base.BuildingHelpText}
	#3item <id|name>#0 - sets the prepared-food item prototype to be loaded
	#3skin <id|name>#0 - sets a skin to go with the item prototype
	#3skin clear#0 - clears the skin
	#3quantity <amount>#0 - sets the quantity of that item to be loaded
	#3purify [on|off]#0 - toggles whether input drugs and transferable food effects are removed
	#3ingredient add <input index> <role>#0 - maps a consumed craft input into the food ledger with a role
	#3ingredient clear#0 - uses all consumed inputs as generic ingredients";

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
			case "purify":
			case "removedrugs":
			case "removeeffects":
			case "cleanse":
				return BuildingCommandPurify(actor, command);
			case "ingredient":
				return BuildingCommandIngredient(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prepared-food item prototype should this product load?");
			return false;
		}

		var proto = long.TryParse(command.SafeRemainingArgument, out var value)
			? Gameworld.ItemProtos.Get(value)
			: Gameworld.ItemProtos.GetByName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (!proto.IsItemType<PreparedFoodGameItemComponentProto>())
		{
			actor.OutputHandler.Send("That item prototype does not have a PreparedFood component.");
			return false;
		}

		ProductProducedId = proto.Id;
		Skin = null;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will now target prepared-food prototype #{ProductProducedId.ToString("N0", actor)} ({proto.ShortDescription.ColourObject()}).");
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
		actor.OutputHandler.Send($"This product will now produce {Quantity.ToString("N0", actor).ColourValue()} of the target item.");
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

	private bool BuildingCommandPurify(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			RemoveDrugsAndFoodEffects = !RemoveDrugsAndFoodEffects;
		}
		else
		{
			switch (command.PopSpeech().ToLowerInvariant())
			{
				case "true":
				case "yes":
				case "on":
					RemoveDrugsAndFoodEffects = true;
					break;
				case "false":
				case "no":
				case "off":
					RemoveDrugsAndFoodEffects = false;
					break;
				default:
					actor.OutputHandler.Send("Use ON or OFF to specify whether this product removes input drugs and transferable food effects.");
					return false;
			}
		}

		ProductChanged = true;
		actor.OutputHandler.Send(RemoveDrugsAndFoodEffects
			? "This product will now remove drugs and transferable food effects from consumed inputs."
			: "This product will now preserve drugs and transferable food effects from consumed inputs.");
		return true;
	}

	private bool BuildingCommandIngredient(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				if (command.IsFinished || !int.TryParse(command.PopSpeech(), out var value))
				{
					actor.OutputHandler.Send("Which input number should be treated as an ingredient?");
					return false;
				}

				var input = Craft.Inputs.ElementAtOrDefault(value - 1);
				if (input is null)
				{
					actor.OutputHandler.Send("There is no such input for this craft.");
					return false;
				}

				if (command.IsFinished)
				{
					actor.OutputHandler.Send("What role should this input have in the food ledger?");
					return false;
				}

				var role = command.SafeRemainingArgument.ToLowerInvariant();
				_ingredientSlots.Add((input.Id, role));
				ProductChanged = true;
				actor.OutputHandler.Send($"Input {value.ToString("N0", actor).ColourValue()} now transfers to the food ledger as {role.ColourName()}.");
				return true;
			case "clear":
				_ingredientSlots.Clear();
				ProductChanged = true;
				actor.OutputHandler.Send("This product will now use all consumed craft inputs as generic food ingredients.");
				return true;
			default:
				actor.OutputHandler.Send("Use INGREDIENT ADD <input index> <role> or INGREDIENT CLEAR.");
				return false;
		}
	}

	public override bool IsValid()
	{
		return ProductProducedId != 0 &&
		       Gameworld.ItemProtos.Get(ProductProducedId)?.IsItemType<PreparedFoodGameItemComponentProto>() == true;
	}

	public override string WhyNotValid()
	{
		if (ProductProducedId == 0)
		{
			return "You must first set a prepared-food item prototype for this product to load.";
		}

		return "The selected item prototype must have a PreparedFood component.";
	}
}

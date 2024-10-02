using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Migrations;
using MudSharp.Models;
using System.Xml.Linq;
using C5;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Work.Crafts.Products;
#nullable enable
public class ProgProduct : BaseProduct
{
	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("Prog",
			(product, craft, game) => new ProgProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("prog",
			(craft, game, fail) => new ProgProduct(craft, game, fail));
	}

	protected ProgProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		ItemProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("ItemProg").Value));
	}

	protected ProgProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld, failproduct)
	{
	}

	/// <inheritdoc />
	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component, ItemQuality referenceQuality)
	{
		if (ItemProg is null)
		{
			return new SimpleProductData([]);
		}
		var items = new CollectionDictionary<string, IGameItem>();
		var liquids = new CollectionDictionary<string, LiquidMixture>();
		var inputList = Craft.Inputs.ToList();
		foreach (var input in Craft.Inputs)
		{
			var inputNumber = inputList.IndexOf(input) + 1;
			if (!component.ConsumedInputs.ContainsKey(input))
			{
				continue;
			}

			var data = component.ConsumedInputs[input].Data;
			if (data is ICraftInputDataWithItems itemData)
			{
				items.AddRange(inputNumber.ToString("F0"), itemData.ConsumedItems);
			}

			if (data is ICraftInputConsumeLiquidData liquidData)
			{
				liquids.Add(inputNumber.ToString("F0"), liquidData.ConsumedMixture);
			}
		}

		var loadedItems = new List<IGameItem>();
		if (ItemProg.ReturnType.CompatibleWith(FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item))
		{
			loadedItems.AddRange(ItemProg.ExecuteCollection<IGameItem>(items, liquids) ?? []);
		}
		else
		{
			loadedItems.AddNotNull(ItemProg.Execute<IGameItem>(items,liquids));
		}

		return new SimpleProductData(loadedItems);
	}

	/// <inheritdoc />
	public override string Name => $"items determined by the {ItemProg?.MXPClickableFunctionName() ?? "Unknown".ColourError()} prog";

	/// <inheritdoc />
	public override string HowSeen(IPerceiver voyeur)
	{
		return $"items determined by the {ItemProg?.MXPClickableFunctionName() ?? "Unknown".ColourError()} prog";
	}

	/// <inheritdoc />
	public override string ProductType => "Prog";

	/// <inheritdoc />
	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("ItemProg", ItemProg?.Id ?? 0)
		).ToString();
	}

	/// <inheritdoc />
	public override bool IsValid()
	{
		return ItemProg is not null;
	}

	/// <inheritdoc />
	public override string WhyNotValid()
	{
		if (ItemProg is null)
		{
			return "You must set an item prog.";
		}
		throw new ApplicationException("Invalid WhyNotValid reason in ProgProduct");
	}

	public IFutureProg? ItemProg { get; protected set; }

	/// <inheritdoc />
	protected override string BuildingHelpText => @"You can use the following options with this command:

	#3prog <prog>#0 - sets the prog that controls which item is loaded";

	/// <inheritdoc />
	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "prog":
				return BuildingCommandProg(actor, command);
		}
		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog.");
			return false;
		}

		var prog = new ProgLookupFromBuilderInputMultipleReturnTypes(actor, command.SafeRemainingArgument, [FutureProgVariableTypes.Item, FutureProgVariableTypes.Collection | FutureProgVariableTypes.Item], [
			[FutureProgVariableTypes.CollectionDictionary | FutureProgVariableTypes.Item],
			[FutureProgVariableTypes.CollectionDictionary | FutureProgVariableTypes.Item, FutureProgVariableTypes.CollectionDictionary | FutureProgVariableTypes.LiquidMixture]
		]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		ItemProg = prog;
		Changed = true;
		actor.OutputHandler.Send($"This product will now use the {prog.MXPClickableFunctionName()} prog to determine the item it loads.");
		return true;
	}
}

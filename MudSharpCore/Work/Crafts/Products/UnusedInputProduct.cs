using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Products;

public class UnusedInputProduct : BaseProduct
{
	internal class UnusedInputProductData : ICraftProductData
	{
		public List<IGameItem> Products { get; } = new();
		public IPerceivable Perceivable { get; }

		public void FinaliseLoadTimeTasks()
		{
			foreach (var product in Products)
			{
				product.FinaliseLoadTimeTasks();
			}
		}

		public XElement SaveToXml()
		{
			return new XElement("Data",
				from product in Products
				select new XElement("Item", product.Id)
			);
		}

		public UnusedInputProductData(IEnumerable<IGameItem> items)
		{
			Products.AddRange(items);
			Perceivable = new PerceivableGroup(Products);
		}

		public UnusedInputProductData(XElement root, IFuturemud gameworld)
		{
			foreach (var element in root.Elements("Item"))
			{
				var item = gameworld.TryGetItem(long.Parse(element.Value), true);
				if (item != null)
				{
					Products.Add(item);
				}
			}

			Perceivable = new PerceivableGroup(Products);
		}

		public void ReleaseProducts(ICell location, RoomLayer layer)
		{
			foreach (var item in Products)
			{
				item.RoomLayer = layer;
				location.Insert(item, true);
				item.HandleEvent(EventType.ItemFinishedLoading, item);
				item.Login();
			}
		}

		public void Delete()
		{
			foreach (var item in Products)
			{
				item.Delete();
			}
		}

		public void Quit()
		{
			foreach (var item in Products)
			{
				item.Quit();
			}
		}
	}

	protected UnusedInputProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
		gameworld)
	{
		var root = XElement.Parse(product.Definition);
		WhichInputId = long.Parse(root.Element("WhichInputId").Value);
		PercentageRecovered = double.Parse(root.Element("PercentageRecovered").Value);
	}

	protected UnusedInputProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld,
		failproduct)
	{
		PercentageRecovered = 1.0;
	}

	public long WhichInputId { get; set; }
	public double PercentageRecovered { get; set; }

	#region Overrides of BaseProduct

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var input = component.ConsumedInputs.FirstOrDefault(x => x.Key.Id == WhichInputId);
		if (input.Key == null)
		{
			throw new ApplicationException("Couldn't find a valid input for craft product to load.");
		}

		IGameItem referenceItem;
		int quantity;
		if (input.Key is ICraftInputConsumesGameItem)
		{
			referenceItem = (IGameItem)input.Value.Data.Perceivable;
			quantity = (int)Math.Min(referenceItem.Quantity,
				Math.Max(1, Math.Ceiling(referenceItem.Quantity * PercentageRecovered)));
		}
		else
		{
			var refItems = ((PerceivableGroup)input.Value.Data.Perceivable).Members.OfType<IGameItem>().ToList();
			referenceItem = refItems.First();
			quantity = (int)Math.Min(refItems.Sum(x => x.Quantity),
				Math.Max(1, Math.Ceiling(refItems.Sum(x => x.Quantity) * PercentageRecovered)));
		}

		if (quantity > 1 && referenceItem.IsItemType<IStackable>())
		{
			var newItem = referenceItem.Prototype.CreateNew();
			newItem.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(newItem);
			newItem.Quality = referenceQuality;
			newItem.GetItemType<IStackable>().Quantity = quantity;
			return new UnusedInputProductData(new[] { newItem });
		}

		var items = new List<IGameItem>();
		for (var i = 0; i < quantity; i++)
		{
			var item = referenceItem.Prototype.CreateNew();
			item.RoomLayer = component.Parent.RoomLayer;
			Gameworld.Add(item);
			item.Quality = referenceQuality;
			items.Add(item);
		}

		return new UnusedInputProductData(items);
	}

	public override string ProductType => "UnusedInput";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("WhichInputId", WhichInputId),
			new XElement("PercentageRecovered", PercentageRecovered)
		).ToString();
	}

	protected override string BuildingHelpText =>
		"You can use the following options with this product:\n\tinput <#> - specifies that the target input is the one to be returned\n\tpercentage <x%> - specifies the amount returned (rounds down)";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "input":
			case "target":
			case "pair":
				return BuildingCommandInput(actor, command);
			case "percentage":
			case "amount":
			case "return":
				return BuildingCommandPercentage(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandInput(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify an input to pair this product with.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var ivalue))
		{
			actor.OutputHandler.Send("Which input number do you want to pair this product with?");
			return false;
		}

		var input = Craft.Inputs.ElementAtOrDefault(ivalue - 1);
		if (input == null)
		{
			actor.OutputHandler.Send("There is no such input for this craft.");
			return false;
		}

		if (!(input is ICraftInputConsumesGameItem) && !(input is ICraftInputConsumesGameItemGroup))
		{
			actor.OutputHandler.Send(
				$"The input {input.Name} is not an appropriate type of input to target with this product.");
			return false;
		}

		WhichInputId = input.Id;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now load up copies of the consumed input of {input.Name}.");
		return true;
	}

	private bool BuildingCommandPercentage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !NumberUtilities.TryParsePercentage(command.SafeRemainingArgument, out var perc))
		{
			actor.OutputHandler.Send("You must specify a valid percentage of the original item to be returned.");
			return false;
		}

		PercentageRecovered = perc;
		actor.OutputHandler.Send(
			$"This product will now return {PercentageRecovered.ToString("P2", actor)} of the original item quantity.");
		ProductChanged = true;
		return true;
	}

	public override bool IsValid()
	{
		return Craft.Inputs.Any(x =>
			x.Id == WhichInputId && (x is ICraftInputConsumesGameItem || x is ICraftInputConsumesGameItemGroup));
	}

	public override string WhyNotValid()
	{
		if (Craft.Inputs.All(x => x.Id != WhichInputId))
		{
			return "You must first set a valid input to target.";
		}

		return "The targeted input is not a valid type of input for this product";
	}

	#endregion

	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("UnusedInput",
			(product, craft, game) => new UnusedInputProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("unusedinput",
			(craft, game, fail) => new UnusedInputProduct(craft, game, fail));
	}

	public override string Name
	{
		get
		{
			var input = Craft.Inputs.FirstOrDefault(x => x.Id == WhichInputId);
			if (input == null || !(input is ICraftInputConsumesGameItem || input is ICraftInputConsumesGameItemGroup))
			{
				return $"{PercentageRecovered:P2} of {"an invalid input".Colour(Telnet.Red)}";
			}

			return $"{PercentageRecovered:P2} of {input.Name} ($i{Craft.Inputs.ToList().IndexOf(input) + 1})";
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		var input = Craft.Inputs.FirstOrDefault(x => x.Id == WhichInputId);
		if (input == null || !(input is ICraftInputConsumesGameItem || input is ICraftInputConsumesGameItemGroup))
		{
			return $"{PercentageRecovered:P2} of {"an invalid input".Colour(Telnet.Red)}";
		}

		return $"{PercentageRecovered:P2} of {input.Name} ($i{Craft.Inputs.ToList().IndexOf(input) + 1})";
	}
}
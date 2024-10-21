using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Lists;

namespace MudSharp.Work.Crafts.Products;

public class ScrapInputProduct : BaseProduct
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

	protected ScrapInputProduct(CraftProduct product, ICraft craft, IFuturemud gameworld) : base(product, craft,
		gameworld)
	{
		var root = XElement.Parse(product.Definition);
		WhichInputId = long.Parse(root.Element("WhichInputId").Value);
		PercentageRecovered = double.Parse(root.Element("PercentageRecovered").Value);
		Tag = Gameworld.Tags.Get(long.Parse(root.Element("Tag")?.Value ?? "0"));
	}

	protected ScrapInputProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(craft, gameworld,
		failproduct)
	{
		PercentageRecovered = 1.0;
	}

	public long WhichInputId { get; set; }
	public double PercentageRecovered { get; set; }
	public ITag Tag { get; set; }

	/// <inheritdoc />
	public override bool RefersToTag(ITag tag)
	{
		return Tag?.IsA(tag) == true;
	}

	#region Overrides of BaseProduct

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var input = component.ConsumedInputs.FirstOrDefault(x => x.Key.Id == WhichInputId);
		if (input.Key == null)
		{
			throw new ApplicationException("Couldn't find a valid input for craft product to load.");
		}

		ISolid material;
		double totalWeight;
		if (input.Key is ICraftInputConsumesGameItemGroup)
		{
			var items = ((PerceivableGroup)input.Value.Data.Perceivable).Members.OfType<IGameItem>().ToList();
			material = items.First().Material;
			totalWeight = items.Sum(x => x.Weight);
		}
		else
		{
			var item = (IGameItem)input.Value.Data.Perceivable;
			material = item.Material;
			totalWeight = item.Weight;
		}

		var newItem =
			GameItems.Prototypes.CommodityGameItemComponentProto.CreateNewCommodity(material,
				totalWeight * PercentageRecovered, null);
		newItem.RoomLayer = component.Parent.RoomLayer;
		Gameworld.Add(newItem);
		return new UnusedInputProductData(new[] { newItem });
	}

	public override string ProductType => "ScrapInput";

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("WhichInputId", WhichInputId),
			new XElement("PercentageRecovered", PercentageRecovered),
			new XElement("Tag", Tag?.Id ?? 0)
		).ToString();
	}

	protected override string SaveDefinitionForRevision(Dictionary<long, long> inputIdMap, Dictionary<long, long> toolIdMap)
	{
		return new XElement("Definition",
			new XElement("WhichInputId", inputIdMap.ContainsKey(WhichInputId) ? inputIdMap[WhichInputId] : 0L),
			new XElement("PercentageRecovered", PercentageRecovered),
			new XElement("Tag", Tag?.Id ?? 0)
		).ToString();
	}

	protected override string BuildingHelpText => @$"{base.BuildingHelpText}
	#3input <#>#0 - specifies that the target input is the one to be returned
	#3percentage <x%>#0 - specifies the amount returned (rounds down)
	#3tag <which>|none#0 - sets or clears the tag of the commodity";

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
			ProductChanged = true;
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
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This product will set the {Tag.FullName.ColourName()} tag on the loaded commodity pile.");
		return true;
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
		actor.OutputHandler.Send($"This product will now turn the input {input.Name} into raw commodities.");
		return true;
	}

	private bool BuildingCommandPercentage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParsePercentage(out var perc))
		{
			actor.OutputHandler.Send(
				"You must specify a valid percentage of the weight original item to be commodified.");
			return false;
		}

		PercentageRecovered = perc;
		actor.OutputHandler.Send(
			$"This product will now commodify {PercentageRecovered.ToString("P2", actor).ColourValue()} of the weight of the original item.");
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
		CraftProductFactory.RegisterCraftProductType("ScrapInput",
			(product, craft, game) => new ScrapInputProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("scrap",
			(craft, game, fail) => new ScrapInputProduct(craft, game, fail));
	}

	public override string Name
	{
		get
		{
			var input = Craft.Inputs.FirstOrDefault(x => x.Id == WhichInputId);
			if (!(input is ICraftInputConsumesGameItem || input is ICraftInputConsumesGameItemGroup))
			{
				return $"{PercentageRecovered:P2} by weight of {"an invalid input".Colour(Telnet.Red)}";
			}

			return $"{PercentageRecovered:P2} by weight of {input.Name} ($i{Craft.Inputs.ToList().IndexOf(input) + 1})";
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		var input = Craft.Inputs.FirstOrDefault(x => x.Id == WhichInputId);
		if (!(input is ICraftInputConsumesGameItem || input is ICraftInputConsumesGameItemGroup))
		{
			return $"{PercentageRecovered:P2} by weight of {"an invalid input".Colour(Telnet.Red)}";
		}

		return $"{PercentageRecovered:P2} by weight of {input.Name} ($i{Craft.Inputs.ToList().IndexOf(input) + 1})";
	}
}
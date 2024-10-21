using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Work.Crafts.Products;

public class DNATestProduct : BaseProduct
{
	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("DNATest",
			(product, craft, game) => new DNATestProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("dnatest",
			(craft, game, fail) => new DNATestProduct(craft, game, fail));
	}

	internal class DNATestProductData : ICraftProductData
	{
		public DNATestProductData(LiquidMixture mixture1, LiquidMixture mixture2, bool isErrorProduct)
		{
			Mixture1 = mixture1;
			Mixture2 = mixture2;
			IsErrorProduct = isErrorProduct;
			Gameworld = Mixture1.Gameworld;
		}

		public DNATestProductData(XElement root, IFuturemud gameworld)
		{
			Mixture1 = new LiquidMixture(root.Element("Mix1").Element("Mix"), gameworld);
			Mixture2 = new LiquidMixture(root.Element("Mix2").Element("Mix"), gameworld);
			IsErrorProduct = bool.Parse(root.Element("IsErrorProduct").Value);
			Gameworld = gameworld;
		}

		public IFuturemud Gameworld { get; }

		public LiquidMixture Mixture1 { get; }
		public LiquidMixture Mixture2 { get; }

		public bool IsErrorProduct { get; }

		#region Implementation of ICraftProductData

		public IPerceivable Perceivable =>
			new DummyPerceivable("the results of the DNA test", "the results of the DNA test");

		public XElement SaveToXml()
		{
			return new XElement("Data", new XElement("Mix1", Mixture1.SaveToXml()),
				new XElement("Mix2", Mixture2.SaveToXml()), new XElement("IsErrorProduct", IsErrorProduct));
		}

		public void FinaliseLoadTimeTasks()
		{
			// Do nothing
		}

		public void ReleaseProducts(ICell location, RoomLayer layer)
		{
			var bloods1 = Mixture1.Instances
			                      .OfType<BloodLiquidInstance>()
			                      .ToList();
			var bloods2 = Mixture2.Instances
			                      .OfType<BloodLiquidInstance>()
			                      .ToList();
			var match = bloods1.Any(x => bloods2.Any(y => x.SourceId == y.SourceId));

			if (IsErrorProduct &&
			    RandomUtilities.DoubleRandom(0.0,
				    Gameworld.GetStaticDouble("DNATestingProductFailureErrorChanceSides")) < 1.0)
			{
				match = !match;
			}

			var result = match
				? "The DNA test results show that the two samples are a genetic match."
				: "The DNA test results show that the two samples are not a genetic match.";
			foreach (var ch in location.LayerCharacters(layer))
			{
				ch.OutputHandler.Send(result);
			}
		}

		public void Delete()
		{
			// Do nothing
		}

		public void Quit()
		{
			// Do nothing
		}

		#endregion
	}

	protected DNATestProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(
		craft, gameworld, failproduct)
	{
	}

	protected DNATestProduct(Models.CraftProduct product, ICraft craft, IFuturemud gameworld) : base(
		product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		WhichInputId1 = long.Parse(root.Element("WhichInputId1").Value);
		WhichInputId2 = long.Parse(root.Element("WhichInputId2").Value);
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("WhichInputId1", WhichInputId1),
			new XElement("WhichInputId2", WhichInputId2)
		).ToString();
	}

	protected override string SaveDefinitionForRevision(Dictionary<long, long> inputIdMap, Dictionary<long, long> toolIdMap)
	{
		return new XElement("Definition",
			new XElement("WhichInputId1", inputIdMap.ContainsKey(WhichInputId1) ? inputIdMap[WhichInputId1] : 0L),
			new XElement("WhichInputId2", inputIdMap.ContainsKey(WhichInputId2) ? inputIdMap[WhichInputId2] : 0L)
		).ToString();
	}

	public long WhichInputId1 { get; set; }
	public long WhichInputId2 { get; set; }

	#region Building Commands

	protected override string BuildingHelpText =>
		"You can use the following options with this product:\n\tinput1 <#> - specifies that the target input which contains the first blood tested\n\tinput2 <#> - specifies the target input which contains the second blood tested";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "input1":
			case "target1":
			case "pair1":
				return BuildingCommandInput1(actor, command);
			case "input2":
			case "target2":
			case "pair2":
				return BuildingCommandInput2(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandInput1(ICharacter actor, StringStack command)
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

		if (!(input is ICraftInputConsumeLiquid))
		{
			actor.OutputHandler.Send(
				$"The input {input.Name} is not an appropriate type of input to target with this product.");
			return false;
		}

		WhichInputId1 = input.Id;
		ProductChanged = true;
		if (WhichInputId1 == WhichInputId2)
		{
			WhichInputId2 = 0;
		}

		actor.OutputHandler.Send(
			$"The first liquid tested by this product will now be the consumed input of {input.Name}.");
		return true;
	}

	private bool BuildingCommandInput2(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a second input to pair this product with.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var ivalue))
		{
			actor.OutputHandler.Send("Which second input number do you want to pair this product with?");
			return false;
		}

		var input = Craft.Inputs.ElementAtOrDefault(ivalue - 1);
		if (input == null)
		{
			actor.OutputHandler.Send("There is no such second input for this craft.");
			return false;
		}

		if (!(input is ICraftInputConsumeLiquid))
		{
			actor.OutputHandler.Send(
				$"The input {input.Name} is not an appropriate type of input to target with this product.");
			return false;
		}

		if (WhichInputId1 == 0)
		{
			actor.OutputHandler.Send("You must set the first input for this product before you set the second.");
			return false;
		}

		if (WhichInputId1 == input.Id)
		{
			actor.OutputHandler.Send("The two inputs cannot be the same.");
			return false;
		}

		WhichInputId2 = input.Id;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"The second liquid tested by this product will now be the consumed input of {input.Name}.");
		return true;
	}

	#endregion

	#region Overrides of BaseProduct

	public override string HowSeen(IPerceiver voyeur)
	{
		return Name;
	}

	public override string Name
	{
		get
		{
			var input1 = Craft.Inputs.FirstOrDefault(x => x.Id == WhichInputId1);
			var input2 = Craft.Inputs.FirstOrDefault(x => x.Id == WhichInputId2);
			var input1text = input1 == null || !(input1 is ICraftInputConsumeLiquid)
				? "an invalid input".Colour(Telnet.Red)
				: $"{input1.Name} ($i{Craft.Inputs.ToList().IndexOf(input1) + 1})";
			var input2text = input2 == null || !(input2 is ICraftInputConsumeLiquid)
				? "an invalid input".Colour(Telnet.Red)
				: $"{input2.Name} ($i{Craft.Inputs.ToList().IndexOf(input2) + 1})";
			return $"DNA test to compare {input1text} with {input2text}";
		}
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var input1 = component.ConsumedInputs.FirstOrDefault(x => x.Key.Id == WhichInputId1);
		if (input1.Key == null)
		{
			throw new ApplicationException("Couldn't find a valid input for craft product to load.");
		}

		var input2 = component.ConsumedInputs.FirstOrDefault(x => x.Key.Id == WhichInputId2);
		if (input2.Key == null)
		{
			throw new ApplicationException("Couldn't find a valid input for craft product to load.");
		}

		if (input1.Value.Data is not ICraftInputConsumeLiquidData liquidInput1)
		{
			throw new ApplicationException("Couldn't find a valid consume liquid product to load.");
		}

		if (input1.Value.Data is not ICraftInputConsumeLiquidData liquidInput2)
		{
			throw new ApplicationException("Couldn't find a valid consume liquid product to load.");
		}

		return new DNATestProductData(new LiquidMixture(liquidInput1.ConsumedMixture),
			new LiquidMixture(liquidInput2.ConsumedMixture), component.HasFailed);
	}

	public override string ProductType => "DNATest";

	public override bool IsValid()
	{
		return Craft.Inputs.Any(x => x.Id == WhichInputId1 && x is ICraftInputConsumeLiquid) &&
		       Craft.Inputs.Any(x => x.Id == WhichInputId2 && x is ICraftInputConsumeLiquid);
	}

	public override string WhyNotValid()
	{
		if (WhichInputId1 == 0)
		{
			return "You must set a first liquid-consuming input for this product to target.";
		}

		if (WhichInputId2 == 0)
		{
			return "You must set a second liquid-consuming input for this product to target.";
		}

		if (!Craft.Inputs.Any(x => x.Id == WhichInputId1 && x is ICraftInputConsumeLiquid))
		{
			return "The first input that this product is targeting is not a liquid-consuming input.";
		}

		return "The second input that this product is targeting is not a liquid-consuming input.";
	}

	#endregion
}
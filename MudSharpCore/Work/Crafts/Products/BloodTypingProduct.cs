using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

namespace MudSharp.Work.Crafts.Products;

public class BloodTypingProduct : BaseProduct
{
	public static void RegisterCraftProduct()
	{
		CraftProductFactory.RegisterCraftProductType("BloodTyping",
			(product, craft, game) => new BloodTypingProduct(product, craft, game));
		CraftProductFactory.RegisterCraftProductTypeForBuilders("bloodtyping",
			(craft, game, fail) => new BloodTypingProduct(craft, game, fail));
	}

	internal class BloodTypingProductData : ICraftProductData
	{
		public BloodTypingProductData(LiquidMixture mixture, bool isErrorProduct, IBloodModel model)
		{
			Mixture = mixture;
			IsErrorProduct = isErrorProduct;
			Model = model;
			Gameworld = Mixture.Gameworld;
		}

		public BloodTypingProductData(XElement root, IFuturemud gameworld)
		{
			Mixture = new LiquidMixture(root.Element("Mix"), gameworld);
			IsErrorProduct = bool.Parse(root.Element("IsErrorProduct").Value);
			Model = gameworld.BloodModels.Get(long.Parse(root.Element("Model").Value));
			Gameworld = gameworld;
		}

		public IFuturemud Gameworld { get; }

		public LiquidMixture Mixture { get; }

		public bool IsErrorProduct { get; }

		public IBloodModel Model { get; }

		#region Implementation of ICraftProductData

		public IPerceivable Perceivable =>
			new DummyPerceivable("the results of the blood type test", "the results of the blood type test");

		public XElement SaveToXml()
		{
			return new XElement("Data", Mixture.SaveToXml(), new XElement("IsErrorProduct", IsErrorProduct),
				new XElement("Model", Model.Id));
		}

		public void FinaliseLoadTimeTasks()
		{
			// Do nothing
		}

		public void ReleaseProducts(ICell location, RoomLayer layer)
		{
			var bloods = Mixture.Instances
			                    .OfType<BloodLiquidInstance>()
			                    .Where(x => Model.Bloodtypes.Contains(x.BloodType))
			                    .ToList();
			var antigens = bloods
			               .SelectMany(x => x.BloodType.Antigens)
			               .Distinct()
			               .ToList();

			if (IsErrorProduct && RandomUtilities.DoubleRandom(0.0,
				    Gameworld.GetStaticDouble("BloodTypingProductFailureErrorChanceSides")) < 1.0)
			{
				if (RandomUtilities.Random(0, 1) == 0)
				{
					var missingAntigens = Model.Antigens.Where(x => !antigens.Contains(x)).ToList();
					if (missingAntigens.Any())
					{
#if DEBUG
						Gameworld.DebugMessage("Blood Typing failed and added a false positive antigen");
#endif
						antigens.Add(missingAntigens.GetRandomElement());
					}
				}
				else
				{
					if (antigens.Any())
					{
#if DEBUG
						Gameworld.DebugMessage("Blood Typing failed and removed a real positive antigen");
#endif
						antigens.Remove(antigens.GetRandomElement());
					}
				}
			}

			var sb = new StringBuilder();
			if (antigens.Any())
			{
				sb.AppendLine(
					$"The markers for the {antigens.Select(x => x.Name.Colour(Telnet.BoldRed)).ListToString()} blood antigen{(antigens.Count == 1 ? "" : "s")} were positive.");
			}
			else
			{
				sb.AppendLine("None of the markers for blood antigens were positive.");
			}

			var match = Model.Bloodtypes.FirstOrDefault(
				x => x.Antigens.All(y => antigens.Contains(y)) &&
				     antigens.All(y => x.Antigens.Contains(y)));
			if (match == null)
			{
				sb.AppendLine("Based on the antigen results, this sample is not a match for any blood type.");
			}
			else
			{
				sb.AppendLine(
					$"Based on the antigen results, this sample is a match for the {match.Name.Colour(Telnet.BoldRed)} blood type.");
			}

			foreach (var ch in location.LayerCharacters(layer))
			{
				ch.OutputHandler.Send(sb.ToString());
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

	protected BloodTypingProduct(ICraft craft, IFuturemud gameworld, bool failproduct) : base(
		craft, gameworld, failproduct)
	{
	}

	protected BloodTypingProduct(Models.CraftProduct product, ICraft craft, IFuturemud gameworld) : base(
		product, craft, gameworld)
	{
		var root = XElement.Parse(product.Definition);
		WhichInputId = long.Parse(root.Element("WhichInputId").Value);
		Model = Gameworld.BloodModels.Get(long.Parse(root.Element("BloodModel").Value));
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("WhichInputId", WhichInputId),
			new XElement("BloodModel", Model?.Id ?? 0)
		).ToString();
	}

	public long WhichInputId { get; set; }
	public IBloodModel Model { get; set; }

	#region Building Commands

	protected override string BuildingHelpText =>
		"You can use the following options with this product:\n\tinput <#> - specifies that the target input which contains the blood tested";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "input":
			case "target":
			case "pair":
				return BuildingCommandInput(actor, command);
			case "model":
			case "blood":
			case "bloodmodel":
			case "blood_model":
			case "blood model":
				return BuildingCommandBloodModel(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandBloodModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a blood model that this blood typing product tests for.");
			return false;
		}

		var model = long.TryParse(command.PopSpeech(), out var value)
			? Gameworld.BloodModels.Get(value)
			: Gameworld.BloodModels.GetByName(command.Last);
		if (model == null)
		{
			actor.OutputHandler.Send("There is no such blood model.");
			return false;
		}

		Model = model;
		ProductChanged = true;
		actor.OutputHandler.Send(
			$"This blood typing product will now work with the {Model.Name.Colour(Telnet.BoldRed)} blood model.");
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

		if (!(input is ICraftInputConsumeLiquid))
		{
			actor.OutputHandler.Send(
				$"The input {input.Name} is not an appropriate type of input to target with this product.");
			return false;
		}

		WhichInputId = input.Id;
		ProductChanged = true;
		actor.OutputHandler.Send($"This product will now test the consumed input of {input.Name}.");
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
			var input = Craft.Inputs.FirstOrDefault(x => x.Id == WhichInputId);
			if (input == null || !(input is ICraftInputConsumeLiquid))
			{
				return
					$"test {"an invalid input".Colour(Telnet.Red)} against {(Model != null ? $"the {Model.Name.Colour(Telnet.BoldRed)} blood model" : "an unknown blood model".Colour(Telnet.Red))}";
			}

			return
				$"test {input.Name} ($i{Craft.Inputs.ToList().IndexOf(input) + 1}) against {(Model != null ? $"the {Model.Name.Colour(Telnet.BoldRed)} blood model" : "an unknown blood model".Colour(Telnet.Red))}";
		}
	}

	public override ICraftProductData ProduceProduct(IActiveCraftGameItemComponent component,
		ItemQuality referenceQuality)
	{
		var input = component.ConsumedInputs.FirstOrDefault(x => x.Key.Id == WhichInputId);
		if (input.Key == null)
		{
			throw new ApplicationException("Couldn't find a valid input for craft product to load.");
		}

		if (input.Value.Data is not ICraftInputConsumeLiquidData liquidInput)
		{
			throw new ApplicationException("Couldn't find a valid consume liquid product to load.");
		}

		return new BloodTypingProductData(new LiquidMixture(liquidInput.ConsumedMixture), component.HasFailed, Model);
	}

	public override string ProductType => "BloodTyping";

	public override bool IsValid()
	{
		if (Model == null)
		{
			return false;
		}

		return Craft.Inputs.Any(x => x.Id == WhichInputId && x is ICraftInputConsumeLiquid);
	}

	public override string WhyNotValid()
	{
		if (Model == null)
		{
			return "You must set a blood model for this blood type to test against.";
		}

		if (WhichInputId == 0)
		{
			return "You must set a liquid-consuming input for this product to target.";
		}

		return "The input that this product is targeting is not a liquid-consuming input.";
	}

	#endregion
}
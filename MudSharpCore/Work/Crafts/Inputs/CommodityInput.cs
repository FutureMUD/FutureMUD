using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Work.Crafts.Inputs;

public class CommodityInput : BaseInput, ICraftInputConsumesGameItem
{
	internal class CommodityInputData : ICraftInputData
	{
		public CommodityInputData(XElement root, IFuturemud gameworld)
		{
			OriginalItem = gameworld.TryGetItem(long.Parse(root.Element("Original").Value), true);
			ConsumedInput = gameworld.TryGetItem(long.Parse(root.Element("Consumed").Value), true);
			Weight = double.Parse(root.Element("Weight").Value);
		}

		public CommodityInputData(IGameItem item, double weight)
		{
			OriginalItem = item;
			Weight = weight;
			ConsumeInput();
		}

		public XElement SaveToXml()
		{
			return new XElement("Data",
				new XElement("Original", OriginalItem.Id),
				new XElement("Consumed", ConsumedInput.Id),
				new XElement("Weight", Weight)
			);
		}

		public void FinaliseLoadTimeTasks()
		{
			ConsumedInput.FinaliseLoadTimeTasks();
			OriginalItem.FinaliseLoadTimeTasks();
		}

		public double Weight { get; set; }

		public ItemQuality InputQuality => ConsumedInput.Quality;

		public IPerceivable Perceivable => ConsumedInput;

		public IGameItem ConsumedInput { get; set; }

		public IGameItem OriginalItem { get; set; }

		public void ConsumeInput()
		{
			ConsumedInput = OriginalItem.DropsWholeByWeight(Weight)
				? OriginalItem
				: OriginalItem.GetByWeight(null, Weight);
			OriginalItem.InInventoryOf?.Take(ConsumedInput);
			OriginalItem.Location?.Extract(ConsumedInput);
			OriginalItem.ContainedIn?.Take(ConsumedInput);
			var originalConnectable = ConsumedInput.GetItemType<IConnectable>();
			foreach (var attached in ConsumedInput.AttachedAndConnectedItems)
			{
				attached.GetItemType<IConnectable>()?.RawDisconnect(originalConnectable, true);
			}
		}

		public void Delete()
		{
			ConsumedInput.Delete();
		}

		public void Quit()
		{
			ConsumedInput.Quit();
		}
	}

	public override string InputType => "Commodity";

	public ISolid Material { get; set; }
	public ITag? CommodityPileTag { get; set; }

	public double Weight { get; set; }

	protected CommodityInput(Models.CraftInput input, ICraft craft, IFuturemud gameworld) : base(input, craft,
		gameworld)
	{
		var root = XElement.Parse(input.Definition);
		Material = Gameworld.Materials.Get(long.Parse(root.Element("Material")?.Value ?? "0"));
		CommodityPileTag = Gameworld.Tags.Get(long.Parse(root.Element("CommodityPileTag")?.Value ?? "0"));
		Weight = double.Parse(root.Element("Weight").Value);
	}

	protected CommodityInput(ICraft craft, IFuturemud gameworld) : base(craft, gameworld)
	{
	}

	public static void RegisterCraftInput()
	{
		CraftInputFactory.RegisterCraftInputType("Commodity",
			(input, craft, game) => new CommodityInput(input, craft, game));
		CraftInputFactory.RegisterCraftInputTypeForBuilders("commodity",
			(craft, game) => new CommodityInput(craft, game));
	}

	protected override string SaveDefinition()
	{
		return new XElement("Definition",
			new XElement("Material", Material?.Id ?? 0),
			new XElement("CommodityPileTag", CommodityPileTag?.Id ?? 0),
			new XElement("Weight", Weight)
		).ToString();
	}

	public override bool IsValid()
	{
		return Material != null && Weight > 0.0;
	}

	public override string WhyNotValid()
	{
		if (Material == null)
		{
			return "You must first set a material for this input to consume.";
		}

		if (Weight <= 0.0)
		{
			return "You must set a positive weight of material for this input to consume.";
		}

		throw new ApplicationException("Unknown WhyNotValid reason in CommodityInput.");
	}

	public override string Name
	{
		get
		{
			if (Material == null)
			{
				return "An unspecified amount of an unspecified material".Colour(Telnet.Red);
			}

			return
				$"{Gameworld.UnitManager.DescribeExact(Weight, Framework.Units.UnitType.Mass, DummyAccount.Instance).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}{(CommodityPileTag is not null ? $" {CommodityPileTag.Name.Pluralise().ColourName()}" : "")}";
		}
	}

	public override string HowSeen(IPerceiver voyeur)
	{
		if (Material == null)
		{
			return "An unspecified amount of an unspecified material".Colour(Telnet.Red);
		}

		return
			$"{Gameworld.UnitManager.DescribeExact(Weight, Framework.Units.UnitType.Mass, voyeur).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}{(CommodityPileTag is not null ? $" {CommodityPileTag.Name.Pluralise().ColourName()}" : "")}";
	}

	private bool MatchesCommodityTag(ICommodity item)
	{
		return (CommodityPileTag is null && item.Tag is null) ||
		       (item.Tag?.IsA(CommodityPileTag) == true);
	}

	public override IEnumerable<IPerceivable> ScoutInput(ICharacter character)
	{
		return character.DeepContextualItems
		                .Except(character.Body.WornItems)
		                .SelectNotNull(x => x.GetItemType<ICommodity>())
		                .Where(x => 
			                x.Material == Material && 
			                x.Weight >= Weight &&
			                MatchesCommodityTag(x))
		                .Select(x => x.Parent)
		                .ToList();
	}

	public override bool IsInput(IPerceivable item)
	{
		return item is IGameItem gi &&
		       gi.GetItemType<ICommodity>() is ICommodity ic &&
		       ic.Material == Material &&
			   MatchesCommodityTag(ic) &&
		       ic.Weight >= Weight;
	}

	public override void UseInput(IPerceivable item, ICraftInputData data)
	{
		((CommodityInputData)data).ConsumedInput.Delete();
	}

	public override double ScoreInputDesirability(IPerceivable item)
	{
		return 1.0;
	}

	public override ICraftInputData ReserveInput(IPerceivable input)
	{
		return new CommodityInputData((IGameItem)input, Weight);
	}

	public override ICraftInputData LoadDataFromXml(XElement root, IFuturemud gameworld)
	{
		return new CommodityInputData(root, gameworld);
	}

	protected override string BuildingHelpString =>
		@"You can use the following options with this input type:

	#3material <material>#0 - sets the target material
	#3piletag <tag>|none#0 - sets or clears the commodity tag the pile must have
	#3weight <weight>#0 - sets the required weight of material";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "tag":
			case "piletag":
				return BuildingCommandTag(actor, command);
			case "material":
				return BuildingCommandMaterial(actor, command);
			case "weight":
			case "amount":
			case "quantity":
				return BuildingCommandQuantity(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which secondary tag did you want this input to require commodity piles to have?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CommodityPileTag = null;
			InputChanged = true;
			actor.OutputHandler.Send("This craft input will now require un-tagged raw commodity piles.");
			return true;
		}

		var matchedtags = actor.Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (matchedtags.Count == 0)
		{
			actor.OutputHandler.Send("There is no such tag.");
			return false;
		}

		if (matchedtags.Count > 1)
		{
			actor.OutputHandler.Send(
				$@"Your text matched multiple tags. Please specify one of the following tags:

{matchedtags.Select(x => $"\t[{x.Id.ToString("N0", actor)}] {x.FullName.ColourName()}").ListToLines()}");
			return false;
		}

		var tag = matchedtags.Single();
		CommodityPileTag = tag;
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input now requires commodities with a secondary tag of {tag.FullName.Colour(Telnet.Cyan)}.");
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
			actor.OutputHandler.Send("How much of the material do you want this input to consume?");
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
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input will now consume {Gameworld.UnitManager.DescribeExact(Weight, Framework.Units.UnitType.Mass, actor).Colour(Telnet.Green)} of {Material.Name.Colour(Material.ResidueColour)}.");
		return true;
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which material did you want this input to consume?");
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
		InputChanged = true;
		actor.OutputHandler.Send(
			$"This input will now consume the {Material.Name.Colour(Material.ResidueColour)} material.");
		return true;
	}
}
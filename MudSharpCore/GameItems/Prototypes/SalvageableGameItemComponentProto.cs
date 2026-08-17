using MudSharp.Accounts;
using MudSharp.Body.Traits;
using MudSharp.Form.Material;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;
using System.Globalization;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public sealed record SalvageCommodityProduct(
	ISolid Material,
	ITag? Tag,
	bool IsFraction,
	double SuccessAmount,
	double FailureAmount)
{
	public double Weight(double sourceBaseWeight, bool success)
	{
		var amount = success ? SuccessAmount : FailureAmount;
		return IsFraction ? sourceBaseWeight * amount : amount;
	}
}

public sealed class SalvageItemProduct
{
	private readonly IFuturemud _gameworld;

	public SalvageItemProduct(IFuturemud gameworld, long itemPrototypeId, int itemPrototypeRevision,
		int successQuantity, int failureQuantity, double successChance, double failureChance)
	{
		_gameworld = gameworld;
		ItemPrototypeId = itemPrototypeId;
		ItemPrototypeRevision = itemPrototypeRevision;
		SuccessQuantity = successQuantity;
		FailureQuantity = failureQuantity;
		SuccessChance = successChance;
		FailureChance = failureChance;
	}

	public SalvageItemProduct(IFuturemud gameworld, IGameItemProto itemPrototype,
		int successQuantity, int failureQuantity, double successChance, double failureChance)
		: this(gameworld, itemPrototype.Id, itemPrototype.RevisionNumber, successQuantity, failureQuantity,
			failureChance: failureChance, successChance: successChance)
	{
	}

	public long ItemPrototypeId { get; }
	public int ItemPrototypeRevision { get; }
	public IGameItemProto? ItemPrototype => _gameworld.ItemProtos.Get(ItemPrototypeId, ItemPrototypeRevision);
	public int SuccessQuantity { get; }
	public int FailureQuantity { get; }
	public double SuccessChance { get; }
	public double FailureChance { get; }
	public int Quantity(bool success) => success ? SuccessQuantity : FailureQuantity;
	public double Chance(bool success) => success ? SuccessChance : FailureChance;
}

public sealed record SalvageProductPlan(
	IReadOnlyList<(SalvageCommodityProduct Product, double Weight)> Commodities,
	IReadOnlyList<(SalvageItemProduct Product, int Quantity)> Items);

public class SalvageableGameItemComponentProto : GameItemComponentProto, ISalvageablePrototype
{
	public const int MaximumItemProductsPerSalvage = 100;

	public override string TypeDescription => "Salvageable";
	public ITraitDefinition? Trait { get; private set; }
	public Difficulty Difficulty { get; private set; } = Difficulty.Normal;
	public ITag? RequiredToolTag { get; private set; }
	public IReadOnlyList<(string Emote, double Delay)> Stages => _stages;
	public IReadOnlyList<SalvageCommodityProduct> CommodityProducts => _commodityProducts;
	public IReadOnlyList<SalvageItemProduct> ItemProducts => _itemProducts;
	public IInventoryPlanTemplate ToolTemplate { get; private set; } = null!;

	private readonly List<(string Emote, double Delay)> _stages = [];
	private readonly List<SalvageCommodityProduct> _commodityProducts = [];
	private readonly List<SalvageItemProduct> _itemProducts = [];

	protected SalvageableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Salvageable")
	{
		RecalculateInventoryPlan();
	}

	protected SalvageableGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
		RecalculateInventoryPlan();
	}

	protected override void LoadFromXml(XElement root)
	{
		Trait = Gameworld.Traits.Get(long.Parse(root.Element("Trait")?.Value ?? "0"));
		Difficulty = (Difficulty)int.Parse(root.Element("Difficulty")?.Value ?? ((int)Difficulty.Normal).ToString());
		RequiredToolTag = Gameworld.Tags.Get(long.Parse(root.Element("ToolTag")?.Value ?? "0"));

		foreach (var element in root.Element("Stages")?.Elements("Stage") ?? [])
		{
			_stages.Add((element.Value, double.Parse(element.Attribute("delay")!.Value, CultureInfo.InvariantCulture)));
		}

		foreach (var element in root.Element("CommodityProducts")?.Elements("Product") ?? [])
		{
			var material = Gameworld.Materials.Get(long.Parse(element.Attribute("material")!.Value)) as ISolid;
			if (material is null)
			{
				continue;
			}

			_commodityProducts.Add(new SalvageCommodityProduct(
				material,
				Gameworld.Tags.Get(long.Parse(element.Attribute("tag")?.Value ?? "0")),
				bool.Parse(element.Attribute("fraction")?.Value ?? "false"),
				double.Parse(element.Attribute("success")!.Value, CultureInfo.InvariantCulture),
				double.Parse(element.Attribute("failure")!.Value, CultureInfo.InvariantCulture)));
		}

		foreach (var element in root.Element("ItemProducts")?.Elements("Product") ?? [])
		{
			_itemProducts.Add(new SalvageItemProduct(
				Gameworld,
				long.Parse(element.Attribute("id")!.Value),
				int.Parse(element.Attribute("revision")!.Value),
				int.Parse(element.Attribute("successQuantity")!.Value),
				int.Parse(element.Attribute("failureQuantity")!.Value),
				double.Parse(element.Attribute("successChance")!.Value, CultureInfo.InvariantCulture),
				double.Parse(element.Attribute("failureChance")!.Value, CultureInfo.InvariantCulture)));
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Trait", Trait?.Id ?? 0L),
			new XElement("Difficulty", (int)Difficulty),
			new XElement("ToolTag", RequiredToolTag?.Id ?? 0L),
			new XElement("Stages",
				_stages.Select(x => new XElement("Stage", new XAttribute("delay", x.Delay), new XCData(x.Emote)))),
			new XElement("CommodityProducts",
				_commodityProducts.Select(x => new XElement("Product",
					new XAttribute("material", x.Material.Id),
					new XAttribute("tag", x.Tag?.Id ?? 0L),
					new XAttribute("fraction", x.IsFraction),
					new XAttribute("success", x.SuccessAmount),
					new XAttribute("failure", x.FailureAmount)))),
			new XElement("ItemProducts",
				_itemProducts.Select(x => new XElement("Product",
					new XAttribute("id", x.ItemPrototypeId),
					new XAttribute("revision", x.ItemPrototypeRevision),
					new XAttribute("successQuantity", x.SuccessQuantity),
					new XAttribute("failureQuantity", x.FailureQuantity),
					new XAttribute("successChance", x.SuccessChance),
					new XAttribute("failureChance", x.FailureChance))))).ToString();
	}

	private void RecalculateInventoryPlan()
	{
		ToolTemplate = new InventoryPlanTemplate(Gameworld,
		[
			new InventoryPlanPhaseTemplate(1, RequiredToolTag is null
				? []
				: [new InventoryPlanActionHold(Gameworld, RequiredToolTag.Id, 0, _ => true, null, 1)])
		]);
	}

	public double MaximumOutputWeight(double sourceBaseWeight, bool success)
	{
		return _commodityProducts.Sum(x => x.Weight(sourceBaseWeight, success)) +
		       _itemProducts.Sum(x => (x.ItemPrototype?.Weight ?? double.PositiveInfinity) * x.Quantity(success));
	}

	public SalvageProductPlan CreateProductPlan(double sourceBaseWeight, bool success, Func<double>? random = null)
	{
		random ??= () => RandomUtilities.DoubleRandom(0.0, 1.0);
		var commodities = _commodityProducts
			.Select(x => (Product: x, Weight: x.Weight(sourceBaseWeight, success)))
			.Where(x => x.Weight > 0.0)
			.ToList();
		var items = _itemProducts
			.Select(x => (Product: x, Quantity: x.Quantity(success)))
			.Where(x => x.Quantity > 0 && x.Product.Chance(success) > 0.0 &&
			            random() < x.Product.Chance(success))
			.ToList();
		return new SalvageProductPlan(commodities, items);
	}

	public bool ConfigurationIsComplete(out string reason)
	{
		if (Trait is null)
		{
			reason = "it has no salvage trait configured";
			return false;
		}

		if (_stages.Count == 0)
		{
			reason = "it has no salvage stages configured";
			return false;
		}

		if (_stages.Any(x => !double.IsFinite(x.Delay) || x.Delay <= 0.0))
		{
			reason = "it has a salvage stage with an invalid delay";
			return false;
		}

		var invalidEmote = _stages
			.Select(x => new Emote(x.Emote, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
				new DummyPerceivable()))
			.FirstOrDefault(x => !x.Valid);
		if (invalidEmote is not null)
		{
			reason = $"it has an invalid salvage stage emote: {invalidEmote.ErrorMessage}";
			return false;
		}

		if (_commodityProducts.Count + _itemProducts.Count == 0)
		{
			reason = "it has no salvage products configured";
			return false;
		}

		if (_commodityProducts.Any(x =>
			!double.IsFinite(x.SuccessAmount) ||
			!double.IsFinite(x.FailureAmount) ||
			x.SuccessAmount < 0.0 ||
			x.FailureAmount < 0.0 ||
			x.IsFraction && (x.SuccessAmount > 1.0 || x.FailureAmount > 1.0)))
		{
			reason = "it has an invalid commodity product amount";
			return false;
		}

		if (_itemProducts.Any(x =>
			!double.IsFinite(x.SuccessChance) ||
			!double.IsFinite(x.FailureChance) ||
			x.SuccessChance < 0.0 ||
			x.SuccessChance > 1.0 ||
			x.FailureChance < 0.0 ||
			x.FailureChance > 1.0 ||
			x.SuccessQuantity < 0 ||
			x.FailureQuantity < 0))
		{
			reason = "it has an invalid item product quantity or chance";
			return false;
		}

		if (_itemProducts.Sum(x => (long)Math.Max(x.SuccessQuantity, x.FailureQuantity)) >
			MaximumItemProductsPerSalvage)
		{
			reason = $"it has more than {MaximumItemProductsPerSalvage:N0} possible item products";
			return false;
		}

		var unresolvedProduct = _itemProducts.FirstOrDefault(x => x.ItemPrototype is null);
		if (unresolvedProduct is not null)
		{
			reason = $"its configured item product #{unresolvedProduct.ItemPrototypeId:N0}r{unresolvedProduct.ItemPrototypeRevision:N0} is unavailable";
			return false;
		}

		if (_commodityProducts.Any(x => x.FailureAmount > x.SuccessAmount) ||
		    _itemProducts.Any(x => x.FailureQuantity > x.SuccessQuantity ||
		                           x.FailureChance > x.SuccessChance))
		{
			reason = "its failure products are not reduced from its success products";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
		=> new SalvageableGameItemComponent(this, parent, temporary);

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
		=> new SalvageableGameItemComponent(component, this, parent);

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
		=> CreateNewRevision(initiator, (proto, gameworld) => new SalvageableGameItemComponentProto(proto, gameworld));

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("salvageable", true,
			(gameworld, account) => new SalvageableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Salvageable",
			(proto, gameworld) => new SalvageableGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("Salvageable",
			"Makes an ordinary item explicitly eligible for staged salvage into authored commodity and item products",
			BuildingHelpText);
	}

	private const string BuildingHelpText = """
		You can use the following options with this component:
			name <name> - sets the name
			desc <description> - sets the description
			trait <trait> - sets the skill or attribute used by the salvage check
			difficulty <difficulty> - sets the salvage check difficulty
			tool <tag|none> - sets or clears the required held-tool tag
		stage add <seconds> <emote> - adds a visible salvage stage; use $0 for the salvager, $1 for the source and $2 for the held tool
			stage remove <number> - removes a stage
			commodity fixed <material> <success weight> <failure weight> [<tag>] - adds fixed commodity output
			commodity fraction <material> <success percent> <failure percent> [<tag>] - adds source-base-mass fractional output
			item <prototype> <success quantity> <failure quantity> <success chance> <failure chance> - adds item output
			product remove <number> - removes a product from the displayed combined list
		""";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		var verb = command.PopSpeech().ToLowerInvariant();
		return verb switch
		{
			"trait" => BuildingCommandTrait(actor, command),
			"difficulty" => BuildingCommandDifficulty(actor, command),
			"tool" => BuildingCommandTool(actor, command),
			"stage" => BuildingCommandStage(actor, command),
			"commodity" => BuildingCommandCommodity(actor, command),
			"item" => BuildingCommandItem(actor, command),
			"product" => BuildingCommandProduct(actor, command),
			_ => base.BuildingCommand(actor, new StringStack($"{verb} {command.RemainingArgument}"))
		};
	}

	private bool BuildingCommandTrait(ICharacter actor, StringStack command)
	{
		var trait = Gameworld.Traits.GetByIdOrName(command.SafeRemainingArgument);
		if (trait is null)
		{
			actor.OutputHandler.Send("There is no such trait.");
			return false;
		}

		Trait = trait;
		Changed = true;
		actor.OutputHandler.Send($"This component now uses the {trait.Name.ColourName()} trait.");
		return true;
	}

	private bool BuildingCommandDifficulty(ICharacter actor, StringStack command)
	{
		if (!command.SafeRemainingArgument.TryParseEnum(out Difficulty difficulty))
		{
			actor.OutputHandler.Send("That is not a valid difficulty.");
			return false;
		}

		Difficulty = difficulty;
		Changed = true;
		actor.OutputHandler.Send($"The salvage check is now {difficulty.Describe().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandTool(ICharacter actor, StringStack command)
	{
		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
		{
			RequiredToolTag = null;
			RecalculateInventoryPlan();
			Changed = true;
			actor.OutputHandler.Send("This component no longer requires a tool.");
			return true;
		}

		var tags = Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
		if (tags.Count != 1)
		{
			actor.OutputHandler.Send(tags.Count == 0 ? "There is no such tag." : "That text matches more than one tag.");
			return false;
		}

		RequiredToolTag = tags.Single();
		RecalculateInventoryPlan();
		Changed = true;
		actor.OutputHandler.Send($"Salvage now requires a held tool tagged {RequiredToolTag.FullName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandStage(ICharacter actor, StringStack command)
	{
		var action = command.PopSpeech().ToLowerInvariant();
		if (action.EqualToAny("remove", "delete", "del"))
		{
			if (!int.TryParse(command.SafeRemainingArgument, out var index) || index < 1 || index > _stages.Count)
			{
				actor.OutputHandler.Send("Which valid stage number do you want to remove?");
				return false;
			}

			_stages.RemoveAt(index - 1);
			Changed = true;
			actor.OutputHandler.Send($"Stage {index:N0} has been removed.");
			return true;
		}

		if (!action.EqualToAny("add", "new") || !double.TryParse(command.PopSpeech(), out var delay) ||
			!double.IsFinite(delay) || delay <= 0.0 || command.IsFinished)
		{
			actor.OutputHandler.Send("Use stage add <seconds> <emote> or stage remove <number>.");
			return false;
		}

		var emoteText = command.SafeRemainingArgument;
		var emote = new Emote(emoteText, new DummyPerceiver(), new DummyPerceivable(), new DummyPerceivable(),
			new DummyPerceivable());
		if (!emote.Valid)
		{
			actor.OutputHandler.Send(emote.ErrorMessage);
			return false;
		}

		_stages.Add((emoteText, delay));
		Changed = true;
		actor.OutputHandler.Send($"A {delay:N1}-second salvage stage has been added.");
		return true;
	}

	private bool BuildingCommandCommodity(ICharacter actor, StringStack command)
	{
		var mode = command.PopSpeech().ToLowerInvariant();
		if (!mode.EqualToAny("fixed", "fraction"))
		{
			actor.OutputHandler.Send("Specify either fixed or fraction.");
			return false;
		}

		var material = Gameworld.Materials.GetByIdOrName(command.PopSpeech()) as ISolid;
		if (material is null)
		{
			actor.OutputHandler.Send("There is no such solid material.");
			return false;
		}

		double success;
		double failure;
		if (mode == "fixed")
		{
			if (!Gameworld.UnitManager.TryGetBaseUnits(command.PopSpeech(), UnitType.Mass, actor, out success) ||
			    !Gameworld.UnitManager.TryGetBaseUnits(command.PopSpeech(), UnitType.Mass, actor, out failure))
			{
				actor.OutputHandler.Send("Specify valid success and failure weights.");
				return false;
			}
		}
		else if (!command.PopSpeech().TryParsePercentage(out success) || !command.PopSpeech().TryParsePercentage(out failure))
		{
			actor.OutputHandler.Send("Specify valid success and failure percentages.");
			return false;
		}

		if (!double.IsFinite(success) || !double.IsFinite(failure) || success < 0.0 || failure < 0.0 ||
			mode == "fraction" && (success > 1.0 || failure > 1.0))
		{
			actor.OutputHandler.Send(mode == "fraction"
				? "Fractional product amounts must be finite percentages from 0% to 100%."
				: "Product amounts must be finite and cannot be negative.");
			return false;
		}

		ITag? tag = null;
		if (!command.IsFinished)
		{
			var tags = Gameworld.Tags.FindMatchingTags(command.SafeRemainingArgument);
			if (tags.Count != 1)
			{
				actor.OutputHandler.Send("The optional commodity tag must identify exactly one tag.");
				return false;
			}

			tag = tags.Single();
		}

		_commodityProducts.Add(new SalvageCommodityProduct(material, tag, mode == "fraction", success, failure));
		Changed = true;
		actor.OutputHandler.Send($"A {material.Name.ColourName()} commodity product has been added.");
		return true;
	}

	private bool BuildingCommandItem(ICharacter actor, StringStack command)
	{
		var itemProto = Gameworld.ItemProtos.GetByIdOrName(command.PopSpeech());
		if (itemProto is null)
		{
			actor.OutputHandler.Send("There is no such item prototype.");
			return false;
		}

		if (!int.TryParse(command.PopSpeech(), out var successQuantity) || successQuantity < 0 ||
		    !int.TryParse(command.PopSpeech(), out var failureQuantity) || failureQuantity < 0 ||
		    !command.PopSpeech().TryParsePercentage(out var successChance) ||
		    !command.PopSpeech().TryParsePercentage(out var failureChance) ||
		    !double.IsFinite(successChance) || !double.IsFinite(failureChance) ||
		    successChance is < 0.0 or > 1.0 || failureChance is < 0.0 or > 1.0 ||
		    _itemProducts.Sum(x => (long)Math.Max(x.SuccessQuantity, x.FailureQuantity)) +
		    Math.Max(successQuantity, failureQuantity) > MaximumItemProductsPerSalvage)
		{
			actor.OutputHandler.Send($"Specify non-negative success/failure quantities, chances from 0% to 100%, and no more than {MaximumItemProductsPerSalvage:N0} possible item products.");
			return false;
		}

		_itemProducts.Add(new SalvageItemProduct(Gameworld, itemProto, successQuantity, failureQuantity, successChance, failureChance));
		Changed = true;
		actor.OutputHandler.Send($"Item prototype {itemProto.EditHeader().ColourName()} has been added as a product.");
		return true;
	}

	private bool BuildingCommandProduct(ICharacter actor, StringStack command)
	{
		if (!command.PopSpeech().EqualToAny("remove", "delete", "del") ||
		    !int.TryParse(command.SafeRemainingArgument, out var index) || index < 1 ||
		    index > _commodityProducts.Count + _itemProducts.Count)
		{
			actor.OutputHandler.Send("Use product remove <valid product number>.");
			return false;
		}

		if (index <= _commodityProducts.Count)
		{
			_commodityProducts.RemoveAt(index - 1);
		}
		else
		{
			_itemProducts.RemoveAt(index - _commodityProducts.Count - 1);
		}

		Changed = true;
		actor.OutputHandler.Send($"Product {index:N0} has been removed.");
		return true;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Salvageable Component #{Id:N0}r{RevisionNumber:N0} ({Name})");
		sb.AppendLine($"Check: {Trait?.Name ?? "None"} / {Difficulty.Describe()}");
		sb.AppendLine($"Tool Tag: {RequiredToolTag?.FullName ?? "None"}");
		sb.AppendLine("Stages:");
		foreach (var (stage, index) in _stages.Select((x, i) => (x, i + 1)))
		{
			sb.AppendLine($"  {index:N0}. {stage.Delay:N1}s - {stage.Emote}");
		}
		sb.AppendLine("Products:");
		var productIndex = 1;
		foreach (var product in _commodityProducts)
		{
			sb.AppendLine($"  {productIndex++:N0}. commodity {product.Material.Name}; success {DescribeAmount(product, true, actor)}, failure {DescribeAmount(product, false, actor)}");
		}
		foreach (var product in _itemProducts)
		{
			var productDescription = product.ItemPrototype?.EditHeader() ??
			                         $"unresolved item #{product.ItemPrototypeId:N0}r{product.ItemPrototypeRevision:N0}";
			sb.AppendLine($"  {productIndex++:N0}. item {productDescription}; success {product.SuccessQuantity:N0} @ {product.SuccessChance:P2}, failure {product.FailureQuantity:N0} @ {product.FailureChance:P2}");
		}
		return sb.ToString();
	}

	private string DescribeAmount(SalvageCommodityProduct product, bool success, ICharacter actor)
	{
		var amount = success ? product.SuccessAmount : product.FailureAmount;
		return product.IsFraction ? amount.ToString("P2", actor) : Gameworld.UnitManager.DescribeExact(amount, UnitType.Mass, actor);
	}
}

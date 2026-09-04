using MudSharp.Accounts;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Units;
using MudSharp.GameItems.Components;
using MudSharp.Health;
using System.Globalization;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class PreparedFoodGameItemComponentProto : GameItemComponentProto, IPreparedFoodPrototype,
	IItemTimeRateSensitivePrototype
{
	public PreparedFoodProfile Profile { get; private set; } = new();

	public override string TypeDescription => "PreparedFood";

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator, (proto, gameworld) => new PreparedFoodGameItemComponentProto(proto, gameworld));
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("preparedfood", true,
			(gameworld, account) => new PreparedFoodGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("prepared food", false,
			(gameworld, account) => new PreparedFoodGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PreparedFood", (proto, gameworld) => new PreparedFoodGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"PreparedFood",
			"Turns an item into new-style prepared food with freshness, ingredient ledgers and drug doses",
			BuildingHelpText
		);
	}

	public override bool CanSubmit()
	{
		return Profile.Bites > 0.0;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"{ "Prepared Food Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})");
		sb.AppendLine();
		sb.AppendLine($"Scope: {Profile.ServingScope.DescribeEnum().ColourName()}");
		sb.AppendLine($"Nutrition: {Profile.SatiationPoints.ToString("N1", actor).ColourValue()} hunger hours, {Profile.ThirstPoints.ToString("N1", actor).ColourValue()} thirst hours, {actor.Gameworld.UnitManager.Describe(Profile.WaterLitres / actor.Gameworld.UnitManager.BaseFluidToLitres, UnitType.FluidVolume, actor).ColourValue()} water, {actor.Gameworld.UnitManager.Describe(Profile.AlcoholLitres / actor.Gameworld.UnitManager.BaseFluidToLitres, UnitType.FluidVolume, actor).ColourValue()} alcohol");
		sb.AppendLine($"Bites per serving: {Profile.Bites.ToString("N2", actor).ColourValue()}");
		sb.AppendLine($"Quality nutrition scaling: {(Profile.QualityNutritionMultiplierPerStep * 100.0).ToString("N1", actor).ColourValue()}% per quality step");
		sb.AppendLine($"Freshness: stale after {DescribeTime(Profile.StaleAfter, actor).ColourValue()}, spoiled after {DescribeTime(Profile.SpoilAfter, actor).ColourValue()}");
		sb.AppendLine($"Stale nutrition: {(Profile.StaleNutritionMultiplier * 100.0).ToString("N1", actor).ColourValue()}%, spoiled nutrition: {(Profile.SpoiledNutritionMultiplier * 100.0).ToString("N1", actor).ColourValue()}%");
		sb.AppendLine($"Liquid absorption: {actor.Gameworld.UnitManager.Describe(Profile.LiquidAbsorptionLitres / actor.Gameworld.UnitManager.BaseFluidToLitres, UnitType.FluidVolume, actor).ColourValue()}");
		sb.AppendLine($"Decorator: {(Profile.Decorator is null ? "none".ColourError() : Profile.Decorator.Name.ColourName())}");
		sb.AppendLine($"Taste template: {Profile.TasteTemplate.ColourCommand()}");
		sb.AppendLine($"Short template: {(string.IsNullOrWhiteSpace(Profile.ShortDescriptionTemplate) ? "none".ColourError() : Profile.ShortDescriptionTemplate.ColourCommand())}");
		sb.AppendLine($"Full template: {(string.IsNullOrWhiteSpace(Profile.FullDescriptionTemplate) ? "none".ColourError() : Profile.FullDescriptionTemplate.ColourCommand())}");
		sb.AppendLine($"On-eat prog: {(Profile.OnEatProg is null ? "none".ColourError() : $"#{Profile.OnEatProg.Id.ToString("N0", actor)} ({Profile.OnEatProg.Name})".ColourName())}");
		sb.AppendLine($"On-stale prog: {(Profile.OnStaleProg is null ? "none".ColourError() : $"#{Profile.OnStaleProg.Id.ToString("N0", actor)} ({Profile.OnStaleProg.Name})".ColourName())}");
		sb.AppendLine();
		sb.AppendLine($"Default ingredients: {(Profile.Ingredients.Any() ? Profile.Ingredients.Select(x => $"{x.Role.ColourName()}: {x.Description.ColourValue()}").ListToString() : "none".ColourError())}");
		sb.AppendLine($"Major allergens: {(Profile.Ingredients.SelectMany(x => x.Allergens).Distinct().Any() ? Profile.Ingredients.SelectMany(x => x.Allergens).Distinct().Select(x => x.DescribeEnum().ColourName()).ListToString() : "none".ColourError())}");
		sb.AppendLine($"Factual dietary contents: {(Profile.DietaryContents.Any() ? Profile.DietaryContents.Select(x => x.DescribeEnum().ColourName()).ListToString() : "none".ColourError())}");
		sb.AppendLine($"Animal-feed purposes: {(Profile.AnimalFeedPurposes.Any() ? Profile.AnimalFeedPurposes.Select(x => x.DescribeEnum().ColourName()).ListToString() : "none".ColourError())}");
		sb.AppendLine($"Default drug doses: {(Profile.DrugDoses.Any() ? Profile.DrugDoses.Select(x => $"{x.Drug?.Name.ColourName() ?? "unknown"} @ {$"{Gameworld.UnitManager.DescribeDecimal(x.Grams, UnitType.Mass, actor)}".ColourValue()}").ListToString() : "none".ColourError())}");
		sb.AppendLine($"Stale drug doses: {(Profile.StaleDrugDoses.Any() ? Profile.StaleDrugDoses.Select(x => $"{x.Drug?.Name.ColourName() ?? "unknown"} @ {$"{Gameworld.UnitManager.DescribeDecimal(x.Grams, UnitType.Mass, actor)}".ColourValue()}").ListToString() : "none".ColourError())}");
		return sb.ToString();
	}

	private static string DescribeTime(TimeSpan? span, IPerceiver voyeur)
	{
		return span is null ? "never" : span.Value.Describe(voyeur);
	}

	protected override void LoadFromXml(XElement root)
	{
		var profile = new PreparedFoodProfile
		{
			ServingScope = LoadServingScope(root.Attribute("ServingScope")?.Value),
			SatiationPoints = LoadDouble(root, "Satiation", 6.0),
			WaterLitres = LoadDouble(root, "Water", 0.05),
			ThirstPoints = LoadDouble(root, "Thirst", 0.0),
			AlcoholLitres = LoadDouble(root, "Alcohol", 0.0),
			Bites = Math.Max(1.0, LoadDouble(root, "Bites", 1.0)),
			QualityNutritionMultiplierPerStep = LoadDouble(root, "QualityScale", 0.08),
			StaleNutritionMultiplier = LoadDouble(root, "StaleMultiplier", 0.25),
			SpoiledNutritionMultiplier = LoadDouble(root, "SpoiledMultiplier", 0.0),
			LiquidAbsorptionLitres = LoadDouble(root, "LiquidAbsorption", 0.02),
			StaleAfter = LoadTime(root, "StaleAfterSeconds"),
			SpoilAfter = LoadTime(root, "SpoilAfterSeconds"),
			TasteTemplate = root.Element("Taste")?.Value ?? "It has an unremarkable taste",
			ShortDescriptionTemplate = root.Element("Short")?.Value ?? string.Empty,
			FullDescriptionTemplate = root.Element("Full")?.Value ?? string.Empty,
			Decorator = Gameworld.StackDecorators.Get(long.Parse(root.Attribute("Decorator")?.Value ?? "0")),
			OnEatProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnEatProg")?.Value ?? "0")),
			OnStaleProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnStaleProg")?.Value ?? "0"))
		};
		if (root.Element("Ingredients") is { } ingredients)
		{
			profile.Ingredients.AddRange(ingredients.Elements("Ingredient").Select(FoodIngredientInstance.LoadFromXml));
		}

		if (root.Element("DrugDoses") is { } doses)
		{
			profile.DrugDoses.AddRange(doses.Elements("Dose").Select(x => FoodDrugDose.LoadFromXml(x, Gameworld)));
		}

		if (root.Element("StaleDrugDoses") is { } staleDoses)
		{
			profile.StaleDrugDoses.AddRange(staleDoses.Elements("Dose").Select(x => FoodDrugDose.LoadFromXml(x, Gameworld)));
		}
		foreach (var element in root.Element("DietaryContents")?.Elements("Content") ?? [])
		{
			if (Enum.TryParse<DietaryContent>(element.Value, true, out var value)) profile.DietaryContents.Add(value);
		}
		foreach (var element in root.Element("AnimalFeedPurposes")?.Elements("Purpose") ?? [])
		{
			if (Enum.TryParse<AnimalFeedPurpose>(element.Value, true, out var value)) profile.AnimalFeedPurposes.Add(value);
		}

		Profile = profile;
	}

	private static double LoadDouble(XElement root, string attribute, double defaultValue)
	{
		return double.Parse(root.Attribute(attribute)?.Value ?? defaultValue.ToString("R", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
	}

	private static TimeSpan? LoadTime(XElement root, string attribute)
	{
		var text = root.Attribute(attribute)?.Value;
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		var seconds = double.Parse(text, CultureInfo.InvariantCulture);
		return seconds <= 0.0 ? null : TimeSpan.FromSeconds(seconds);
	}

	public static FoodServingScope LoadServingScope(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return FoodServingScope.WholeItem;
		}

		return text.Trim().ToLowerInvariant() switch
		{
			"item" or "whole" or "wholeitem" or "whole-item" => FoodServingScope.WholeItem,
			"stack" or "unit" or "perunit" or "per-unit" or "perstackunit" or "per-stack-unit" => FoodServingScope.PerStackUnit,
			_ when Enum.TryParse<FoodServingScope>(text, true, out var value) => value,
			_ => FoodServingScope.WholeItem
		};
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XAttribute("ServingScope", Profile.ServingScope),
			new XAttribute("Satiation", Profile.SatiationPoints.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("Water", Profile.WaterLitres.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("Thirst", Profile.ThirstPoints.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("Alcohol", Profile.AlcoholLitres.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("Bites", Profile.Bites.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("QualityScale", Profile.QualityNutritionMultiplierPerStep.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("StaleMultiplier", Profile.StaleNutritionMultiplier.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("SpoiledMultiplier", Profile.SpoiledNutritionMultiplier.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("LiquidAbsorption", Profile.LiquidAbsorptionLitres.ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("StaleAfterSeconds", (Profile.StaleAfter?.TotalSeconds ?? 0.0).ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("SpoilAfterSeconds", (Profile.SpoilAfter?.TotalSeconds ?? 0.0).ToString("R", CultureInfo.InvariantCulture)),
			new XAttribute("Decorator", Profile.Decorator?.Id ?? 0),
			new XElement("Taste", new XCData(Profile.TasteTemplate)),
			new XElement("Short", new XCData(Profile.ShortDescriptionTemplate)),
			new XElement("Full", new XCData(Profile.FullDescriptionTemplate)),
			new XElement("OnEatProg", Profile.OnEatProg?.Id ?? 0),
			new XElement("OnStaleProg", Profile.OnStaleProg?.Id ?? 0),
			new XElement("Ingredients", Profile.Ingredients.Select(x => x.SaveToXml())),
			new XElement("DrugDoses", Profile.DrugDoses.Select(x => x.SaveToXml())),
			new XElement("StaleDrugDoses", Profile.StaleDrugDoses.Select(x => x.SaveToXml())),
			new XElement("DietaryContents", Profile.DietaryContents.OrderBy(x => x).Select(x => new XElement("Content", x))),
			new XElement("AnimalFeedPurposes", Profile.AnimalFeedPurposes.OrderBy(x => x).Select(x => new XElement("Purpose", x)))
		).ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new PreparedFoodGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PreparedFoodGameItemComponent(component, this, parent);
	}

	protected PreparedFoodGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected PreparedFoodGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "PreparedFood")
	{
		var stringValue = gameworld.GetStaticConfiguration("DefaultFoodStackDecorator");
		if (stringValue is not null)
		{
			Profile.Decorator = Gameworld.StackDecorators.Get(long.Parse(stringValue));
		}

		Changed = true;
	}

	private const string BuildingHelpText = @"You can use the following options with this component:
	#3name <name>#0 - sets the name of the component
	#3desc <desc>#0 - sets the description of the component
	#3hunger <hours>#0 - the hours of food satiation from eating one serving
	#3thirst <hours>#0 - the hours of thirst satiation from eating one serving
	#3water <amount>#0 - the liquid volume of water from eating one serving
	#3alcohol <amount>#0 - the liquid volume of alcohol from eating one serving
	#3bites <#>#0 - the number of bites in one serving
	#3scope whole|stack#0 - whether this is one food item or one serving per stack unit
	#3quality <multiplier>#0 - sets nutrition scaling per quality step, e.g. 0.08
	#3stale <seconds|none>#0 - how long before it becomes stale
	#3spoil <seconds|none>#0 - how long before it becomes spoiled
	#3stalemult <multiplier>#0 - nutrition multiplier while stale
	#3spoiledmult <multiplier>#0 - nutrition multiplier while spoiled
	#3absorb <amount>#0 - liquid volume absorbed into the food before overflow contamination
	#3taste <template>#0 - taste text, with tokens like {quality}, {freshness}, {ingredients}, {var:name}
	#3short <template|clear>#0 - optional short-description template
	#3full <template|clear>#0 - optional full-description template
	#3ingredient add <role> <text>#0 - adds a default ingredient ledger entry
	#3ingredient category <#> <category>#0 - sets an ingredient's factual category
	#3ingredient allergen <#> <allergen>#0 - toggles a major allergen on an ingredient
	#3ingredient clear#0 - clears default ingredient entries
	#3dietary <content>#0 - toggles a factual dietary content (this is not a suitability label)
	#3feedpurpose <purpose|none>#0 - toggles an animal-feed purpose, or clears all purposes
	#3drug <drug> <grams>#0 - adds an ingested default drug dose
	#3drug clear#0 - clears default drug doses
	#3staledrug <drug> <grams>#0 - adds an ingested stale/spoiled drug dose
	#3staledrug clear#0 - clears stale/spoiled drug doses
	#3prog <which|clear>#0 - sets or clears the on-eat prog
	#3staleprog <which|clear>#0 - sets or clears the stale-eat prog";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "satiation":
			case "hunger":
			case "hours":
				return BuildingCommandSatiation(actor, command);
			case "thirst":
				return BuildingCommandThirst(actor, command);
			case "water":
				return BuildingCommandWater(actor, command);
			case "alcohol":
				return BuildingCommandAlcohol(actor, command);
			case "taste":
				return BuildingCommandTaste(actor, command);
			case "short":
			case "sdesc":
				return BuildingCommandShort(actor, command);
			case "full":
			case "descstring":
			case "fdesc":
				return BuildingCommandFull(actor, command);
			case "bites":
				return BuildingCommandBites(actor, command);
			case "scope":
			case "serving":
				return BuildingCommandScope(actor, command);
			case "quality":
			case "qualityscale":
				return BuildingCommandQuality(actor, command);
			case "stale":
			case "staleafter":
				return BuildingCommandStale(actor, command);
			case "spoil":
			case "spoilafter":
				return BuildingCommandSpoil(actor, command);
			case "stalemult":
			case "stalemultiplier":
				return BuildingCommandStaleMultiplier(actor, command);
			case "spoiledmult":
			case "spoiledmultiplier":
				return BuildingCommandSpoiledMultiplier(actor, command);
			case "absorb":
			case "absorption":
				return BuildingCommandAbsorb(actor, command);
			case "ingredient":
				return BuildingCommandIngredient(actor, command);
			case "dietary":
				return BuildingCommandMetadata(actor, command, Profile.DietaryContents, "dietary content");
			case "feedpurpose":
			case "feed":
				return BuildingCommandMetadata(actor, command, Profile.AnimalFeedPurposes, "animal-feed purpose");
			case "drug":
			case "dose":
				return BuildingCommandDrug(actor, command, false);
			case "staledrug":
			case "staledose":
				return BuildingCommandDrug(actor, command, true);
			case "prog":
				return BuildingCommandProg(actor, command, false);
			case "staleprog":
				return BuildingCommandProg(actor, command, true);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandSatiation(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("How many hours of satiation should one serving provide?");
			return false;
		}

		Profile.SatiationPoints = value;
		Changed = true;
		actor.Send($"One serving now provides {value.ToString("N1", actor).ColourValue()} hours of satiation.");
		return true;
	}

	private bool BuildingCommandThirst(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("How many hours of thirst satiation should one serving provide?");
			return false;
		}

		Profile.ThirstPoints = value;
		Changed = true;
		actor.Send($"One serving now provides {value.ToString("N1", actor).ColourValue()} hours of thirst satiation.");
		return true;
	}

	private bool BuildingCommandWater(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What volume of water should one serving provide?");
			return false;
		}

		var value = actor.Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var success);
		if (!success)
		{
			actor.Send("That is not a valid fluid volume.");
			return false;
		}

		Profile.WaterLitres = value * actor.Gameworld.UnitManager.BaseFluidToLitres;
		Changed = true;
		actor.Send($"One serving now provides {actor.Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).ColourValue()} of water.");
		return true;
	}

	private bool BuildingCommandAlcohol(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What volume of alcohol should one serving provide?");
			return false;
		}

		var value = actor.Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var success);
		if (!success)
		{
			actor.Send("That is not a valid fluid volume.");
			return false;
		}

		Profile.AlcoholLitres = value * actor.Gameworld.UnitManager.BaseFluidToLitres;
		Changed = true;
		actor.Send($"One serving now provides {actor.Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).ColourValue()} of alcohol.");
		return true;
	}

	private bool BuildingCommandBites(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value <= 0.0)
		{
			actor.Send("How many bites should one serving contain?");
			return false;
		}

		Profile.Bites = value;
		Changed = true;
		actor.Send($"One serving now contains {value.ToString("N2", actor).ColourValue()} bites.");
		return true;
	}

	private bool BuildingCommandScope(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("Should this food be scoped to the whole item or each stack unit?");
			return false;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "whole":
			case "item":
			case "wholeitem":
				Profile.ServingScope = FoodServingScope.WholeItem;
				break;
			case "stack":
			case "unit":
			case "perunit":
			case "per-stack-unit":
				Profile.ServingScope = FoodServingScope.PerStackUnit;
				break;
			default:
				actor.Send("The valid scopes are WHOLE and STACK.");
				return false;
		}

		Changed = true;
		actor.Send($"This food now uses {Profile.ServingScope.DescribeEnum().ColourName()} serving scope.");
		return true;
	}

	private bool BuildingCommandQuality(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value))
		{
			actor.Send("What nutrition multiplier per quality step should this food use? For example, 0.08.");
			return false;
		}

		Profile.QualityNutritionMultiplierPerStep = value;
		Changed = true;
		actor.Send($"Nutrition now scales by {(value * 100.0).ToString("N1", actor).ColourValue()}% per quality step.");
		return true;
	}

	private bool BuildingCommandStale(ICharacter actor, StringStack command)
	{
		return BuildingCommandTime(actor, command, true);
	}

	private bool BuildingCommandSpoil(ICharacter actor, StringStack command)
	{
		return BuildingCommandTime(actor, command, false);
	}

	private bool BuildingCommandTime(ICharacter actor, StringStack command, bool stale)
	{
		if (command.IsFinished)
		{
			actor.Send(stale ? "How many seconds before this food becomes stale? Use NONE for never." : "How many seconds before this food becomes spoiled? Use NONE for never.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "never", "clear", "0"))
		{
			if (stale)
			{
				Profile.StaleAfter = null;
			}
			else
			{
				Profile.SpoilAfter = null;
			}
			Changed = true;
			actor.Send(stale ? "This food will no longer become stale by age." : "This food will no longer become spoiled by age.");
			return true;
		}

		if (!double.TryParse(command.PopSpeech(), out var seconds) || seconds <= 0.0)
		{
			actor.Send("You must enter a positive number of seconds, or NONE.");
			return false;
		}

		if (stale)
		{
			Profile.StaleAfter = TimeSpan.FromSeconds(seconds);
		}
		else
		{
			Profile.SpoilAfter = TimeSpan.FromSeconds(seconds);
		}
		Changed = true;
		actor.Send(stale ? $"This food now becomes stale after {TimeSpan.FromSeconds(seconds).Describe(actor).ColourValue()}." : $"This food now becomes spoiled after {TimeSpan.FromSeconds(seconds).Describe(actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandStaleMultiplier(ICharacter actor, StringStack command)
	{
		return BuildingCommandMultiplier(actor, command, true);
	}

	private bool BuildingCommandSpoiledMultiplier(ICharacter actor, StringStack command)
	{
		return BuildingCommandMultiplier(actor, command, false);
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack command, bool stale)
	{
		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var value) || value < 0.0)
		{
			actor.Send("You must enter a non-negative multiplier.");
			return false;
		}

		if (stale)
		{
			Profile.StaleNutritionMultiplier = value;
		}
		else
		{
			Profile.SpoiledNutritionMultiplier = value;
		}
		Changed = true;
		actor.Send(stale ? $"Stale food now gives {(value * 100.0).ToString("N1", actor).ColourValue()}% nutrition." : $"Spoiled food now gives {(value * 100.0).ToString("N1", actor).ColourValue()}% nutrition.");
		return true;
	}

	private bool BuildingCommandAbsorb(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How much liquid can this food absorb before overflow contamination?");
			return false;
		}

		var value = actor.Gameworld.UnitManager.GetBaseUnits(command.SafeRemainingArgument, UnitType.FluidVolume, out var success);
		if (!success || value < 0.0)
		{
			actor.Send("That is not a valid non-negative fluid volume.");
			return false;
		}

		Profile.LiquidAbsorptionLitres = value * actor.Gameworld.UnitManager.BaseFluidToLitres;
		Changed = true;
		actor.Send($"This food can now absorb {actor.Gameworld.UnitManager.Describe(value, UnitType.FluidVolume, actor).ColourValue()} before overflow contamination.");
		return true;
	}

	private bool BuildingCommandTaste(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What taste template should this food use?");
			return false;
		}

		Profile.TasteTemplate = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This food now uses the taste template:\n\n{Profile.TasteTemplate.Wrap(80, "\t").ColourCommand()}");
		return true;
	}

	private bool BuildingCommandShort(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What short-description template should this food use? Use CLEAR for none.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "none", "remove"))
		{
			Profile.ShortDescriptionTemplate = string.Empty;
			Changed = true;
			actor.Send("This food will no longer override its short description.");
			return true;
		}

		Profile.ShortDescriptionTemplate = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This food now uses the short template:\n\n{Profile.ShortDescriptionTemplate.ColourCommand()}");
		return true;
	}

	private bool BuildingCommandFull(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("What full-description template should this food use? Use CLEAR for none.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("clear", "none", "remove"))
		{
			Profile.FullDescriptionTemplate = string.Empty;
			Changed = true;
			actor.Send("This food will no longer override its full description.");
			return true;
		}

		Profile.FullDescriptionTemplate = command.SafeRemainingArgument;
		Changed = true;
		actor.Send($"This food now uses the full template:\n\n{Profile.FullDescriptionTemplate.Wrap(80, "\t").ColourCommand()}");
		return true;
	}

	private bool BuildingCommandIngredient(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
				if (command.IsFinished)
				{
					actor.Send("What role should this ingredient have?");
					return false;
				}

				var role = command.PopSpeech().ToLowerInvariant();
				if (command.IsFinished)
				{
					actor.Send("What text should describe this ingredient?");
					return false;
				}

				Profile.Ingredients.Add(new FoodIngredientInstance
				{
					Role = role,
					Description = command.SafeRemainingArgument,
					TasteText = command.SafeRemainingArgument
				});
				Changed = true;
				actor.Send($"This food now includes {role.ColourName()} ingredient {Profile.Ingredients.Last().Description.ColourCommand()}.");
				return true;
			case "clear":
				Profile.Ingredients.Clear();
				Changed = true;
				actor.Send("This food no longer has any default ingredient ledger entries.");
				return true;
			case "category":
				if (!int.TryParse(command.PopSpeech(), out var categoryIndex) || categoryIndex < 1 || categoryIndex > Profile.Ingredients.Count)
				{
					actor.Send("Which ingredient number do you want to categorise?");
					return false;
				}
				if (!Enum.TryParse<FoodIngredientCategory>(command.SafeRemainingArgument.Replace(" ", string.Empty), true, out var category))
				{
					actor.Send($"That is not a valid ingredient category. Valid categories are {Enum.GetValues<FoodIngredientCategory>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return false;
				}
				Profile.Ingredients[categoryIndex - 1].Category = category;
				Changed = true;
				actor.Send($"Ingredient {categoryIndex.ToString("N0", actor).ColourValue()} is now categorised as {category.DescribeEnum().ColourName()}.");
				return true;
			case "allergen":
				if (!int.TryParse(command.PopSpeech(), out var allergenIndex) || allergenIndex < 1 || allergenIndex > Profile.Ingredients.Count)
				{
					actor.Send("Which ingredient number do you want to edit?");
					return false;
				}
				if (!Enum.TryParse<MajorFoodAllergen>(command.SafeRemainingArgument.Replace(" ", string.Empty), true, out var allergen))
				{
					actor.Send($"That is not a recognised major allergen. Valid allergens are {Enum.GetValues<MajorFoodAllergen>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
					return false;
				}
				var ingredient = Profile.Ingredients[allergenIndex - 1];
				var added = ingredient.Allergens.Add(allergen);
				if (!added) ingredient.Allergens.Remove(allergen);
				Changed = true;
				actor.Send($"{allergen.DescribeEnum().ColourName()} is {(added ? "now" : "no longer")} declared for ingredient {allergenIndex.ToString("N0", actor).ColourValue()}.");
				return true;
			default:
				actor.Send("Use INGREDIENT ADD <role> <text>, INGREDIENT CATEGORY <#> <category>, INGREDIENT ALLERGEN <#> <allergen>, or INGREDIENT CLEAR.");
				return false;
		}
	}

	private bool BuildingCommandMetadata<T>(ICharacter actor, StringStack command, HashSet<T> values, string label)
		where T : struct, Enum
	{
		if (command.IsFinished)
		{
			actor.Send($"Which {label} do you want to toggle? Valid values are {Enum.GetValues<T>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (command.PeekSpeech().EqualToAny("none", "clear"))
		{
			values.Clear();
			Changed = true;
			actor.Send($"This food no longer declares any {label.Pluralise()}.");
			return true;
		}

		if (!Enum.TryParse<T>(command.SafeRemainingArgument.Replace(" ", string.Empty), true, out var value))
		{
			actor.Send($"That is not a valid {label}. Valid values are {Enum.GetValues<T>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		var added = values.Add(value);
		if (!added) values.Remove(value);
		Changed = true;
		actor.Send($"{value.DescribeEnum().ColourName()} is {(added ? "now" : "no longer")} declared as {label.A_An()} for this food.");
		return true;
	}

	private bool BuildingCommandDrug(ICharacter actor, StringStack command, bool stale)
	{
		var list = stale ? Profile.StaleDrugDoses : Profile.DrugDoses;
		if (command.IsFinished)
		{
			actor.Send(stale ? "Which stale/spoiled drug should be added, or CLEAR?" : "Which default drug should be added, or CLEAR?");
			return false;
		}

		if (command.PeekSpeech().EqualTo("clear"))
		{
			list.Clear();
			Changed = true;
			actor.Send(stale ? "This food no longer applies any stale/spoiled drug doses." : "This food no longer applies any default drug doses.");
			return true;
		}

		var text = command.PopSpeech();
		var drug = long.TryParse(text, out var value) ? Gameworld.Drugs.Get(value) : Gameworld.Drugs.GetByName(text);
		if (drug is null)
		{
			actor.Send("There is no such drug.");
			return false;
		}

		if (!drug.DrugVectors.HasFlag(DrugVector.Ingested))
		{
			actor.Send("That drug does not support the ingested vector.");
			return false;
		}

		if (command.IsFinished || !double.TryParse(command.PopSpeech(), out var grams) || grams <= 0.0)
		{
			actor.Send("How many grams of this drug should one serving contain?");
			return false;
		}

		list.Add(new FoodDrugDose { Drug = drug, Grams = grams, Source = stale ? "stale food" : "default profile" });
		Changed = true;
		actor.Send(stale ? $"Stale/spoiled servings now apply {grams.ToString("N4", actor).ColourValue()}g of {drug.Name.ColourName()}." : $"Each serving now applies {grams.ToString("N4", actor).ColourValue()}g of {drug.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command, bool stale)
	{
		if (command.IsFinished)
		{
			actor.Send("You can either CLEAR to remove the existing prog, or specify a prog name or ID to add.");
			return false;
		}

		if (command.PeekSpeech().EqualTo("clear"))
		{
			if (stale)
			{
				Profile.OnStaleProg = null;
			}
			else
			{
				Profile.OnEatProg = null;
			}
			Changed = true;
			actor.Send(stale ? "This food will no longer execute a stale-eat prog." : "This food will no longer execute an on-eat prog.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.Send("There is no such prog.");
			return false;
		}

		var expected = stale
			? new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Text }
			: new[] { ProgVariableTypes.Character, ProgVariableTypes.Item, ProgVariableTypes.Number };
		if (!prog.MatchesParameters(expected))
		{
			actor.Send(stale ? "The stale prog should accept a character, item, number and text." : "The prog should accept a character, item and number.");
			return false;
		}

		if (stale)
		{
			Profile.OnStaleProg = prog;
		}
		else
		{
			Profile.OnEatProg = prog;
		}
		Changed = true;
		actor.Send(stale ? $"This food will now execute the {prog.Name.ColourName()} stale-eat prog." : $"This food will now execute the {prog.Name.ColourName()} on-eat prog.");
		return true;
	}
}

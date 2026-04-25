using MudSharp.Body;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Decorators;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Components;

public class PreparedFoodGameItemComponent : GameItemComponent, IPreparedFood
{
	private PreparedFoodGameItemComponentProto _prototype;
	private FoodServingScope _servingScope;
	private double _satiationPoints;
	private double _waterLitres;
	private double _thirstPoints;
	private double _alcoholLitres;
	private double _totalBites;
	private double _bitesRemaining;
	private double _qualityNutritionMultiplierPerStep;
	private double _staleNutritionMultiplier;
	private double _spoiledNutritionMultiplier;
	private double _liquidAbsorptionLitres;
	private TimeSpan? _staleAfter;
	private TimeSpan? _spoilAfter;
	private string _tasteTemplate = string.Empty;
	private string _shortDescriptionTemplate = string.Empty;
	private string _fullDescriptionTemplate = string.Empty;
	private IStackDecorator? _decorator;
	private IFutureProg? _onEatProg;
	private IFutureProg? _onStaleProg;
	private readonly List<FoodIngredientInstance> _ingredients = new();
	private readonly List<FoodDrugDose> _drugDoses = new();
	private readonly List<FoodDrugDose> _staleDrugDoses = new();

	public override IGameItemComponentProto Prototype => _prototype;

	public FoodServingScope ServingScope => _servingScope;

	public DateTime CreatedAt { get; set; }

	public FoodFreshness Freshness
	{
		get
		{
			var now = DateTime.UtcNow;
			if (_spoilAfter is not null && CreatedAt + _spoilAfter.Value <= now)
			{
				return FoodFreshness.Spoiled;
			}

			if (_staleAfter is not null && CreatedAt + _staleAfter.Value <= now)
			{
				return FoodFreshness.Stale;
			}

			return FoodFreshness.Fresh;
		}
	}

	public IEnumerable<FoodIngredientInstance> Ingredients => _ingredients;
	public IEnumerable<FoodDrugDose> DrugDoses => _drugDoses;

	public double SatiationPoints => _satiationPoints * QualityMultiplier * FreshnessNutritionMultiplier;
	public double WaterLitres => _waterLitres * QualityMultiplier * FreshnessNutritionMultiplier;
	public double ThirstPoints => _thirstPoints * QualityMultiplier * FreshnessNutritionMultiplier;
	public double AlcoholLitres => _alcoholLitres;
	public string TasteString => RenderTemplate(_tasteTemplate);
	public double TotalBites => Math.Max(1.0, _totalBites);

	public double BitesRemaining
	{
		get => Math.Min(_bitesRemaining, TotalBites);
		set
		{
			_bitesRemaining = Math.Max(0.0, Math.Min(value, TotalBites));
			if (_bitesRemaining > 0.0)
			{
				Changed = true;
				HandleDescriptionUpdate();
				return;
			}

			CompleteCurrentServing();
		}
	}

	private double QualityMultiplier
	{
		get
		{
			var multiplier = 1.0 + ((int)Parent.Quality - (int)ItemQuality.Standard) * _qualityNutritionMultiplierPerStep;
			return Math.Max(0.0, multiplier);
		}
	}

	private double FreshnessNutritionMultiplier
	{
		get
		{
			return Freshness switch
			{
				FoodFreshness.Stale => Math.Max(0.0, _staleNutritionMultiplier),
				FoodFreshness.Spoiled => Math.Max(0.0, _spoiledNutritionMultiplier),
				_ => 1.0
			};
		}
	}

	private static string DoubleString(double value)
	{
		return value.ToString("R", CultureInfo.InvariantCulture);
	}

	private static double LoadDouble(XElement root, string attribute, double defaultValue)
	{
		return double.Parse(root.Attribute(attribute)?.Value ?? DoubleString(defaultValue), CultureInfo.InvariantCulture);
	}

	private static TimeSpan? LoadTimeSpanSeconds(XElement root, string attribute)
	{
		var text = root.Attribute(attribute)?.Value;
		if (string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		var seconds = double.Parse(text, CultureInfo.InvariantCulture);
		return seconds <= 0.0 ? null : TimeSpan.FromSeconds(seconds);
	}

	private void ApplyProfileInternal(PreparedFoodProfile profile, bool replaceIngredientsAndDoses)
	{
		_servingScope = profile.ServingScope;
		_satiationPoints = profile.SatiationPoints;
		_waterLitres = profile.WaterLitres;
		_thirstPoints = profile.ThirstPoints;
		_alcoholLitres = profile.AlcoholLitres;
		_totalBites = Math.Max(1.0, profile.Bites);
		_qualityNutritionMultiplierPerStep = profile.QualityNutritionMultiplierPerStep;
		_staleNutritionMultiplier = profile.StaleNutritionMultiplier;
		_spoiledNutritionMultiplier = profile.SpoiledNutritionMultiplier;
		_liquidAbsorptionLitres = profile.LiquidAbsorptionLitres;
		_staleAfter = profile.StaleAfter;
		_spoilAfter = profile.SpoilAfter;
		_tasteTemplate = profile.TasteTemplate;
		_shortDescriptionTemplate = profile.ShortDescriptionTemplate;
		_fullDescriptionTemplate = profile.FullDescriptionTemplate;
		_decorator = profile.Decorator;
		_onEatProg = profile.OnEatProg;
		_onStaleProg = profile.OnStaleProg;
		if (!replaceIngredientsAndDoses)
		{
			return;
		}

		_ingredients.Clear();
		_ingredients.AddRange(profile.Ingredients.Select(x => x.Clone()));
		_drugDoses.Clear();
		_drugDoses.AddRange(profile.DrugDoses.Select(x => x.Clone()));
		_staleDrugDoses.Clear();
		_staleDrugDoses.AddRange(profile.StaleDrugDoses.Select(x => x.Clone()));
	}

	public void ApplyPreparedFoodProfile(PreparedFoodProfile profile, bool replaceIngredientsAndDoses = true)
	{
		ApplyProfileInternal(profile, replaceIngredientsAndDoses);
		_bitesRemaining = Math.Min(Math.Max(_bitesRemaining <= 0.0 ? _totalBites : _bitesRemaining, 0.0), TotalBites);
		Changed = true;
		HandleDescriptionUpdate();
	}

	public void AddIngredient(FoodIngredientInstance ingredient)
	{
		_ingredients.Add(ingredient.Clone());
		Changed = true;
		HandleDescriptionUpdate();
	}

	public void AddDrugDose(FoodDrugDose dose)
	{
		if (dose.Drug is null || dose.Grams <= 0.0)
		{
			return;
		}

		_drugDoses.Add(dose.Clone());
		Changed = true;
	}

	public void AddStaleDrugDose(FoodDrugDose dose)
	{
		if (dose.Drug is null || dose.Grams <= 0.0)
		{
			return;
		}

		_staleDrugDoses.Add(dose.Clone());
		Changed = true;
	}

	public void AbsorbLiquid(LiquidMixture mixture, string source, bool includeDrugDoses = true)
	{
		foreach (var instance in mixture.Instances)
		{
			_ingredients.Add(new FoodIngredientInstance
			{
				Role = source,
				Description = instance.Liquid.MaterialDescription,
				TasteText = instance.Liquid.TasteText,
				LiquidId = instance.Liquid.Id,
				Volume = instance.Amount,
				Quality = ItemQuality.Standard
			});

			if (!includeDrugDoses || instance.Liquid.Drug is null || !instance.Liquid.Drug.DrugVectors.HasFlag(DrugVector.Ingested))
			{
				continue;
			}

			var grams = instance.Amount * instance.Liquid.DrugGramsPerUnitVolume;
			if (grams > 0.0)
			{
				_drugDoses.Add(new FoodDrugDose
				{
					Drug = instance.Liquid.Drug,
					Grams = grams,
					Source = source
				});
			}
		}

		Changed = true;
		HandleDescriptionUpdate();
	}

	public void Eat(IBody body, double bites)
	{
		var proportion = Math.Max(0.0, Math.Min(bites, BitesRemaining)) / TotalBites;
		body.ApplyIngestedDrugDoses(_drugDoses, proportion, Parent);
		if (Freshness != FoodFreshness.Fresh)
		{
			body.ApplyIngestedDrugDoses(_staleDrugDoses, proportion, Parent);
			_onStaleProg?.Execute(body.Actor, Parent, bites, Freshness.DescribeEnum());
		}

		_onEatProg?.Execute(body.Actor, Parent, bites);
		BitesRemaining -= bites;
	}

	public override bool ExposeToLiquid(LiquidMixture mixture)
	{
		if (_liquidAbsorptionLitres <= 0.0 || mixture.TotalVolume <= 0.0)
		{
			return false;
		}

		var capacity = _liquidAbsorptionLitres / (Gameworld.UnitManager?.BaseFluidToLitres ?? 1.0);
		var absorbedVolume = Math.Min(mixture.TotalVolume, capacity);
		var absorbed = mixture.RemoveLiquidVolume(absorbedVolume);
		if (absorbed is not null && !absorbed.IsEmpty)
		{
			AbsorbLiquid(absorbed, "liquid exposure");
		}

		return mixture.TotalVolume <= 0.0;
	}

	private void CompleteCurrentServing()
	{
		if (_servingScope == FoodServingScope.PerStackUnit && Parent.GetItemType<IStackable>() is { } stackable &&
		    stackable.Quantity > 1)
		{
			stackable.Quantity -= 1;
			_bitesRemaining = TotalBites;
			Changed = true;
			HandleDescriptionUpdate();
			return;
		}

		Parent.Delete();
	}

	public override double ComponentWeightMultiplier
	{
		get
		{
			var biteMultiplier = BitesRemaining / TotalBites;
			if (_servingScope != FoodServingScope.PerStackUnit || Parent.GetItemType<IStackable>() is not { } stackable ||
			    stackable.Quantity <= 1)
			{
				return biteMultiplier;
			}

			return Math.Max(0.0, (stackable.Quantity - 1 + biteMultiplier) / stackable.Quantity);
		}
	}

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return component is PreparedFoodGameItemComponent other &&
		       (BitesRemaining != other.BitesRemaining ||
		        CreatedAt != other.CreatedAt ||
		        Freshness != other.Freshness ||
		        !_ingredients.Select(x => x.Description).SequenceEqual(other._ingredients.Select(x => x.Description)) ||
		        !_drugDoses.Select(x => (x.Drug?.Id ?? 0, x.Grams)).SequenceEqual(other._drugDoses.Select(x => (x.Drug?.Id ?? 0, x.Grams))));
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full;
	}

	public override int DecorationPriority => 100;

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				if (!string.IsNullOrWhiteSpace(_shortDescriptionTemplate))
				{
					return RenderTemplate(_shortDescriptionTemplate);
				}

				return _decorator is null ? description : _decorator.Describe(name, description, 100.0 * BitesRemaining / TotalBites);
			case DescriptionType.Full:
				var baseDescription = string.IsNullOrWhiteSpace(_fullDescriptionTemplate)
					? description
					: RenderTemplate(_fullDescriptionTemplate);
				var freshness = Freshness == FoodFreshness.Fresh
					? string.Empty
					: $"\n\nIt is {Freshness.DescribeEnum().ToLowerInvariant()}.";
				return string.Format(voyeur, "{0}\n\nIt has {1:N0} out of {2:N0} bites remaining.{3}", baseDescription,
					BitesRemaining, TotalBites, freshness);
		}

		return description;
	}

	private string RenderTemplate(string template)
	{
		if (string.IsNullOrWhiteSpace(template))
		{
			return string.Empty;
		}

		var result = template;
		result = ReplaceIgnoreCase(result, "{quality}", Parent.Quality.Describe());
		result = ReplaceIgnoreCase(result, "{freshness}", Freshness.DescribeEnum().ToLowerInvariant());
		result = ReplaceIgnoreCase(result, "{primary}", _ingredients.FirstOrDefault()?.Description ?? string.Empty);
		result = ReplaceIgnoreCase(result, "{ingredients}", _ingredients.Select(x => x.Description).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ListToString());
		result = ReplaceIgnoreCase(result, "{additives}", _ingredients.Where(x => x.Role.EqualTo("additive")).Select(x => x.Description).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ListToString());
		result = ReplaceIgnoreCase(result, "{bites}", BitesRemaining.ToString("N0", CultureInfo.InvariantCulture));
		result = ReplaceIgnoreCase(result, "{servings}", Parent.Quantity.ToString("N0", CultureInfo.InvariantCulture));

		result = Regex.Replace(result, @"\{ingredient:([^}]+)\}", match =>
		{
			var role = match.Groups[1].Value;
			return _ingredients.Where(x => x.Role.EqualTo(role))
			                   .Select(x => x.Description)
			                   .Where(x => !string.IsNullOrWhiteSpace(x))
			                   .Distinct()
			                   .ListToString();
		}, RegexOptions.IgnoreCase);

		result = Regex.Replace(result, @"\{var:([^}]+)\}", match =>
		{
			var name = match.Groups[1].Value;
			var variable = Parent.GetItemType<IVariable>();
			var definition = variable?.CharacteristicDefinitions.FirstOrDefault(x => x.Name.EqualTo(name));
			return definition is null ? string.Empty : variable!.GetCharacteristic(definition)?.GetValue ?? string.Empty;
		}, RegexOptions.IgnoreCase);

		return result;
	}

	private static string ReplaceIgnoreCase(string text, string oldValue, string newValue)
	{
		return Regex.Replace(text, Regex.Escape(oldValue), newValue.Replace("$", "$$"), RegexOptions.IgnoreCase);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PreparedFoodGameItemComponentProto)newProto;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PreparedFoodGameItemComponent(this, newParent, temporary);
	}

	public PreparedFoodGameItemComponent(PreparedFoodGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		CreatedAt = rhs.CreatedAt;
		_servingScope = rhs._servingScope;
		_satiationPoints = rhs._satiationPoints;
		_waterLitres = rhs._waterLitres;
		_thirstPoints = rhs._thirstPoints;
		_alcoholLitres = rhs._alcoholLitres;
		_totalBites = rhs._totalBites;
		_bitesRemaining = rhs._bitesRemaining;
		_qualityNutritionMultiplierPerStep = rhs._qualityNutritionMultiplierPerStep;
		_staleNutritionMultiplier = rhs._staleNutritionMultiplier;
		_spoiledNutritionMultiplier = rhs._spoiledNutritionMultiplier;
		_liquidAbsorptionLitres = rhs._liquidAbsorptionLitres;
		_staleAfter = rhs._staleAfter;
		_spoilAfter = rhs._spoilAfter;
		_tasteTemplate = rhs._tasteTemplate;
		_shortDescriptionTemplate = rhs._shortDescriptionTemplate;
		_fullDescriptionTemplate = rhs._fullDescriptionTemplate;
		_decorator = rhs._decorator;
		_onEatProg = rhs._onEatProg;
		_onStaleProg = rhs._onStaleProg;
		_ingredients.AddRange(rhs._ingredients.Select(x => x.Clone()));
		_drugDoses.AddRange(rhs._drugDoses.Select(x => x.Clone()));
		_staleDrugDoses.AddRange(rhs._staleDrugDoses.Select(x => x.Clone()));
	}

	public PreparedFoodGameItemComponent(PreparedFoodGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		CreatedAt = DateTime.UtcNow;
		ApplyProfileInternal(proto.Profile, true);
		_bitesRemaining = TotalBites;
	}

	public PreparedFoodGameItemComponent(MudSharp.Models.GameItemComponent component, PreparedFoodGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		ApplyProfileInternal(proto.Profile, true);
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	private void LoadFromXml(XElement root)
	{
		CreatedAt = DateTime.Parse(root.Attribute("created")?.Value ?? DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture),
			CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
		_servingScope = Enum.Parse<FoodServingScope>(root.Attribute("scope")?.Value ?? _servingScope.ToString());
		_satiationPoints = LoadDouble(root, "satiation", _satiationPoints);
		_waterLitres = LoadDouble(root, "water", _waterLitres);
		_thirstPoints = LoadDouble(root, "thirst", _thirstPoints);
		_alcoholLitres = LoadDouble(root, "alcohol", _alcoholLitres);
		_totalBites = Math.Max(1.0, LoadDouble(root, "totalBites", _totalBites));
		_bitesRemaining = Math.Min(_totalBites, LoadDouble(root, "bitesRemaining", _totalBites));
		_qualityNutritionMultiplierPerStep = LoadDouble(root, "qualityScale", _qualityNutritionMultiplierPerStep);
		_staleNutritionMultiplier = LoadDouble(root, "staleMultiplier", _staleNutritionMultiplier);
		_spoiledNutritionMultiplier = LoadDouble(root, "spoiledMultiplier", _spoiledNutritionMultiplier);
		_liquidAbsorptionLitres = LoadDouble(root, "absorbLitres", _liquidAbsorptionLitres);
		_staleAfter = LoadTimeSpanSeconds(root, "staleSeconds") ?? _staleAfter;
		_spoilAfter = LoadTimeSpanSeconds(root, "spoilSeconds") ?? _spoilAfter;
		_tasteTemplate = root.Element("Taste")?.Value ?? _tasteTemplate;
		_shortDescriptionTemplate = root.Element("Short")?.Value ?? _shortDescriptionTemplate;
		_fullDescriptionTemplate = root.Element("Full")?.Value ?? _fullDescriptionTemplate;
		_decorator = Gameworld.StackDecorators.Get(long.Parse(root.Attribute("decorator")?.Value ?? (_decorator?.Id ?? 0).ToString()));
		_onEatProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnEatProg")?.Value ?? (_onEatProg?.Id ?? 0).ToString()));
		_onStaleProg = Gameworld.FutureProgs.Get(long.Parse(root.Element("OnStaleProg")?.Value ?? (_onStaleProg?.Id ?? 0).ToString()));

		if (root.Element("Ingredients") is { } ingredients)
		{
			_ingredients.Clear();
			_ingredients.AddRange(ingredients.Elements("Ingredient").Select(FoodIngredientInstance.LoadFromXml));
		}

		if (root.Element("DrugDoses") is { } doses)
		{
			_drugDoses.Clear();
			_drugDoses.AddRange(doses.Elements("Dose").Select(x => FoodDrugDose.LoadFromXml(x, Gameworld)));
		}

		if (root.Element("StaleDrugDoses") is { } staleDoses)
		{
			_staleDrugDoses.Clear();
			_staleDrugDoses.AddRange(staleDoses.Elements("Dose").Select(x => FoodDrugDose.LoadFromXml(x, Gameworld)));
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XAttribute("created", CreatedAt.ToString("o", CultureInfo.InvariantCulture)),
			new XAttribute("scope", _servingScope),
			new XAttribute("satiation", DoubleString(_satiationPoints)),
			new XAttribute("water", DoubleString(_waterLitres)),
			new XAttribute("thirst", DoubleString(_thirstPoints)),
			new XAttribute("alcohol", DoubleString(_alcoholLitres)),
			new XAttribute("totalBites", DoubleString(_totalBites)),
			new XAttribute("bitesRemaining", DoubleString(_bitesRemaining)),
			new XAttribute("qualityScale", DoubleString(_qualityNutritionMultiplierPerStep)),
			new XAttribute("staleMultiplier", DoubleString(_staleNutritionMultiplier)),
			new XAttribute("spoiledMultiplier", DoubleString(_spoiledNutritionMultiplier)),
			new XAttribute("absorbLitres", DoubleString(_liquidAbsorptionLitres)),
			new XAttribute("staleSeconds", DoubleString(_staleAfter?.TotalSeconds ?? 0.0)),
			new XAttribute("spoilSeconds", DoubleString(_spoilAfter?.TotalSeconds ?? 0.0)),
			new XAttribute("decorator", _decorator?.Id ?? 0),
			new XElement("Taste", new XCData(_tasteTemplate)),
			new XElement("Short", new XCData(_shortDescriptionTemplate)),
			new XElement("Full", new XCData(_fullDescriptionTemplate)),
			new XElement("OnEatProg", _onEatProg?.Id ?? 0),
			new XElement("OnStaleProg", _onStaleProg?.Id ?? 0),
			new XElement("Ingredients", _ingredients.Select(x => x.SaveToXml())),
			new XElement("DrugDoses", _drugDoses.Select(x => x.SaveToXml())),
			new XElement("StaleDrugDoses", _staleDrugDoses.Select(x => x.SaveToXml()))
		).ToString();
	}
}

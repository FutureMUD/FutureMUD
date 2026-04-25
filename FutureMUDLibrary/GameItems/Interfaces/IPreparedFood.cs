using MudSharp.Body;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Decorators;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

public enum FoodServingScope
{
	WholeItem,
	PerStackUnit
}

public enum FoodFreshness
{
	Fresh,
	Stale,
	Spoiled
}

public class FoodIngredientInstance
{
	public string Role { get; set; } = "ingredient";
	public string Description { get; set; } = string.Empty;
	public string TasteText { get; set; } = string.Empty;
	public long SourceItemProtoId { get; set; }
	public long MaterialId { get; set; }
	public long LiquidId { get; set; }
	public double Weight { get; set; }
	public double Volume { get; set; }
	public ItemQuality Quality { get; set; } = ItemQuality.Standard;

	public FoodIngredientInstance Clone()
	{
		return new FoodIngredientInstance
		{
			Role = Role,
			Description = Description,
			TasteText = TasteText,
			SourceItemProtoId = SourceItemProtoId,
			MaterialId = MaterialId,
			LiquidId = LiquidId,
			Weight = Weight,
			Volume = Volume,
			Quality = Quality
		};
	}

	public XElement SaveToXml()
	{
		return new XElement("Ingredient",
			new XAttribute("role", Role),
			new XAttribute("source", SourceItemProtoId),
			new XAttribute("material", MaterialId),
			new XAttribute("liquid", LiquidId),
			new XAttribute("weight", Weight),
			new XAttribute("volume", Volume),
			new XAttribute("quality", (int)Quality),
			new XElement("Description", new XCData(Description)),
			new XElement("Taste", new XCData(TasteText))
		);
	}

	public static FoodIngredientInstance LoadFromXml(XElement root)
	{
		return new FoodIngredientInstance
		{
			Role = root.Attribute("role")?.Value ?? "ingredient",
			SourceItemProtoId = long.Parse(root.Attribute("source")?.Value ?? "0"),
			MaterialId = long.Parse(root.Attribute("material")?.Value ?? "0"),
			LiquidId = long.Parse(root.Attribute("liquid")?.Value ?? "0"),
			Weight = double.Parse(root.Attribute("weight")?.Value ?? "0", CultureInfo.InvariantCulture),
			Volume = double.Parse(root.Attribute("volume")?.Value ?? "0", CultureInfo.InvariantCulture),
			Quality = (ItemQuality)int.Parse(root.Attribute("quality")?.Value ?? ((int)ItemQuality.Standard).ToString()),
			Description = root.Element("Description")?.Value ?? string.Empty,
			TasteText = root.Element("Taste")?.Value ?? string.Empty
		};
	}
}

public class FoodDrugDose
{
	public IDrug? Drug { get; set; }
	public double Grams { get; set; }
	public string Source { get; set; } = string.Empty;

	public FoodDrugDose Clone()
	{
		return new FoodDrugDose
		{
			Drug = Drug,
			Grams = Grams,
			Source = Source
		};
	}

	public FoodDrugDose Clone(double multiplier)
	{
		return new FoodDrugDose
		{
			Drug = Drug,
			Grams = Grams * multiplier,
			Source = Source
		};
	}

	public XElement SaveToXml(string elementName = "Dose")
	{
		return new XElement(elementName,
			new XAttribute("drug", Drug?.Id ?? 0),
			new XAttribute("grams", Grams),
			new XAttribute("source", Source)
		);
	}

	public static FoodDrugDose LoadFromXml(XElement root, IFuturemud gameworld)
	{
		return new FoodDrugDose
		{
			Drug = gameworld.Drugs.Get(long.Parse(root.Attribute("drug")?.Value ?? "0")),
			Grams = double.Parse(root.Attribute("grams")?.Value ?? "0", CultureInfo.InvariantCulture),
			Source = root.Attribute("source")?.Value ?? string.Empty
		};
	}
}

public class PreparedFoodProfile
{
	public FoodServingScope ServingScope { get; set; } = FoodServingScope.WholeItem;
	public double SatiationPoints { get; set; } = 6.0;
	public double WaterLitres { get; set; } = 0.05;
	public double ThirstPoints { get; set; }
	public double AlcoholLitres { get; set; }
	public double Bites { get; set; } = 1.0;
	public double QualityNutritionMultiplierPerStep { get; set; } = 0.08;
	public double StaleNutritionMultiplier { get; set; } = 0.25;
	public double SpoiledNutritionMultiplier { get; set; }
	public double LiquidAbsorptionLitres { get; set; } = 0.02;
	public TimeSpan? StaleAfter { get; set; }
	public TimeSpan? SpoilAfter { get; set; }
	public string TasteTemplate { get; set; } = "It has an unremarkable taste";
	public string ShortDescriptionTemplate { get; set; } = string.Empty;
	public string FullDescriptionTemplate { get; set; } = string.Empty;
	public IStackDecorator? Decorator { get; set; }
	public IFutureProg? OnEatProg { get; set; }
	public IFutureProg? OnStaleProg { get; set; }
	public List<FoodIngredientInstance> Ingredients { get; } = new();
	public List<FoodDrugDose> DrugDoses { get; } = new();
	public List<FoodDrugDose> StaleDrugDoses { get; } = new();

	public PreparedFoodProfile Clone()
	{
		var profile = new PreparedFoodProfile
		{
			ServingScope = ServingScope,
			SatiationPoints = SatiationPoints,
			WaterLitres = WaterLitres,
			ThirstPoints = ThirstPoints,
			AlcoholLitres = AlcoholLitres,
			Bites = Bites,
			QualityNutritionMultiplierPerStep = QualityNutritionMultiplierPerStep,
			StaleNutritionMultiplier = StaleNutritionMultiplier,
			SpoiledNutritionMultiplier = SpoiledNutritionMultiplier,
			LiquidAbsorptionLitres = LiquidAbsorptionLitres,
			StaleAfter = StaleAfter,
			SpoilAfter = SpoilAfter,
			TasteTemplate = TasteTemplate,
			ShortDescriptionTemplate = ShortDescriptionTemplate,
			FullDescriptionTemplate = FullDescriptionTemplate,
			Decorator = Decorator,
			OnEatProg = OnEatProg,
			OnStaleProg = OnStaleProg
		};
		profile.Ingredients.AddRange(Ingredients.Select(x => x.Clone()));
		profile.DrugDoses.AddRange(DrugDoses.Select(x => x.Clone()));
		profile.StaleDrugDoses.AddRange(StaleDrugDoses.Select(x => x.Clone()));
		return profile;
	}
}

public interface IPreparedFood : IEdible
{
	FoodServingScope ServingScope { get; }
	FoodFreshness Freshness { get; }
	DateTime CreatedAt { get; set; }
	IEnumerable<FoodIngredientInstance> Ingredients { get; }
	IEnumerable<FoodDrugDose> DrugDoses { get; }
	void ApplyPreparedFoodProfile(PreparedFoodProfile profile, bool replaceIngredientsAndDoses = true);
	void AddIngredient(FoodIngredientInstance ingredient);
	void AddDrugDose(FoodDrugDose dose);
	void AddStaleDrugDose(FoodDrugDose dose);
	void AbsorbLiquid(LiquidMixture mixture, string source, bool includeDrugDoses = true);
}

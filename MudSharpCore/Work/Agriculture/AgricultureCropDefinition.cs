using MudSharp.Database;
using MudSharp.Framework.Save;

namespace MudSharp.Work.Agriculture;

public class AgricultureCropDefinition : SaveableItem, IAgricultureCropDefinition
{
	private readonly List<AgricultureCommodityYield> _yieldOutputs = new();
	private readonly List<AgricultureCommodityYield> _seedRequirements = new();
	private readonly List<AgriculturePlantingWindow> _plantingWindows = new();
	private readonly Dictionary<AgricultureScoreType, AgricultureScoreRange> _scoreRanges = new();

	public AgricultureCropDefinition(Models.AgricultureCropDefinition definition, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = definition.Id;
		_name = definition.Name;
		Description = definition.Description;
		Category = definition.Category;
		LoadDefinition(definition.Definition);
	}

	public AgricultureCropDefinition(IFuturemud gameworld, string name, string description, string category,
		int baseGrowthDays, int harvestWindowDays, int minimumMoisture, int maximumMoisture, int minimumTemperature,
		int maximumTemperature, IEnumerable<AgricultureCommodityYield> yieldOutputs = null,
		IEnumerable<AgricultureCommodityYield> seedRequirements = null, bool isPerennial = false,
		int harvestCycleDays = 0, IEnumerable<AgricultureScoreRange> scoreRanges = null,
		IEnumerable<AgriculturePlantingWindow> plantingWindows = null,
		AgriculturePollinationDependency pollinationDependency = AgriculturePollinationDependency.None,
		int pollinationHealthBonus = 0, int pollinationYieldBonus = 0)
	{
		Gameworld = gameworld;
		_name = name;
		Description = description;
		Category = category;
		BaseGrowthDays = baseGrowthDays;
		HarvestWindowDays = harvestWindowDays;
		MinimumMoisture = minimumMoisture;
		MaximumMoisture = maximumMoisture;
		MinimumTemperature = minimumTemperature;
		MaximumTemperature = maximumTemperature;
		PollinationDependency = pollinationDependency;
		PollinationHealthBonus = System.Math.Clamp(pollinationHealthBonus, 0, 1);
		PollinationYieldBonus = System.Math.Clamp(pollinationYieldBonus, 0, 2);
		IsPerennial = isPerennial;
		HarvestCycleDays = System.Math.Clamp(harvestCycleDays <= 0 ? baseGrowthDays : harvestCycleDays, 1, 10000);
		_yieldOutputs.AddRange(yieldOutputs ?? Enumerable.Empty<AgricultureCommodityYield>());
		_seedRequirements.AddRange(seedRequirements ?? Enumerable.Empty<AgricultureCommodityYield>());
		_plantingWindows.AddRange(plantingWindows ?? Enumerable.Empty<AgriculturePlantingWindow>());
		foreach (var range in scoreRanges ?? Enumerable.Empty<AgricultureScoreRange>())
		{
			_scoreRanges[range.Score] = range;
		}

		using (new FMDB())
		{
			var dbitem = new Models.AgricultureCropDefinition
			{
				Name = Name,
				Description = Description,
				Category = Category,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.AgricultureCropDefinitions.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "AgricultureCropDefinition";
	public string Description { get; private set; }
	public string Category { get; private set; }
	public int BaseGrowthDays { get; private set; }
	public int HarvestWindowDays { get; private set; }
	public int MinimumMoisture { get; private set; }
	public int MaximumMoisture { get; private set; }
	public int MinimumTemperature { get; private set; }
	public int MaximumTemperature { get; private set; }
	public AgriculturePollinationDependency PollinationDependency { get; private set; }
	public int PollinationHealthBonus { get; private set; }
	public int PollinationYieldBonus { get; private set; }
	public bool IsPerennial { get; private set; }
	public int HarvestCycleDays { get; private set; }
	public IReadOnlyCollection<AgriculturePlantingWindow> PlantingWindows => _plantingWindows;
	public IReadOnlyCollection<AgricultureScoreRange> ScoreRanges => _scoreRanges.Values;
	public IReadOnlyCollection<AgricultureCommodityYield> YieldOutputs => _yieldOutputs;
	public IReadOnlyCollection<AgricultureCommodityYield> SeedRequirements => _seedRequirements;

	public void BuildingSetName(string name)
	{
		_name = name;
		Changed = true;
	}

	public void BuildingSetDescription(string description)
	{
		Description = description;
		Changed = true;
	}

	public void BuildingSetCategory(string category)
	{
		Category = category;
		Changed = true;
	}

	public void BuildingSetGrowthDays(int value)
	{
		BaseGrowthDays = System.Math.Clamp(value, 1, 10000);
		Changed = true;
	}

	public void BuildingSetHarvestWindowDays(int value)
	{
		HarvestWindowDays = System.Math.Clamp(value, 1, 10000);
		Changed = true;
	}

	public void BuildingSetPerennial(bool value)
	{
		IsPerennial = value;
		Changed = true;
	}

	public void BuildingSetHarvestCycleDays(int value)
	{
		HarvestCycleDays = System.Math.Clamp(value, 1, 10000);
		Changed = true;
	}

	public void BuildingSetMoistureRange(int minimum, int maximum)
	{
		MinimumMoisture = minimum.ClampScore();
		MaximumMoisture = maximum.ClampScore();
		if (MaximumMoisture < MinimumMoisture)
		{
			(MaximumMoisture, MinimumMoisture) = (MinimumMoisture, MaximumMoisture);
		}

		Changed = true;
	}

	public void BuildingSetTemperatureRange(int minimum, int maximum)
	{
		MinimumTemperature = minimum;
		MaximumTemperature = maximum;
		if (MaximumTemperature < MinimumTemperature)
		{
			(MaximumTemperature, MinimumTemperature) = (MinimumTemperature, MaximumTemperature);
		}

		Changed = true;
	}

	public void BuildingSetPollination(AgriculturePollinationDependency dependency, int healthBonus, int yieldBonus)
	{
		PollinationDependency = dependency;
		PollinationHealthBonus = dependency == AgriculturePollinationDependency.None ? 0 : System.Math.Clamp(healthBonus, 0, 1);
		PollinationYieldBonus = dependency == AgriculturePollinationDependency.None ? 0 : System.Math.Clamp(yieldBonus, 0, 2);
		Changed = true;
	}

	public void BuildingSetScoreRange(AgricultureScoreType score, int minimum, int maximum)
	{
		_scoreRanges[score] = new AgricultureScoreRange(score, minimum, maximum);
		Changed = true;
	}

	public void BuildingRemoveScoreRange(AgricultureScoreType score)
	{
		if (_scoreRanges.Remove(score))
		{
			Changed = true;
		}
	}

	public void BuildingSetPlantingWindows(IEnumerable<AgriculturePlantingWindow> windows)
	{
		_plantingWindows.Clear();
		_plantingWindows.AddRange(windows
			.Where(x => !string.IsNullOrWhiteSpace(x.Value))
			.DistinctBy(x => (x.Type, NormalisePlantingWindowValue(x.Value))));
		Changed = true;
	}

	public void BuildingClearPlantingWindows()
	{
		if (_plantingWindows.Count == 0)
		{
			return;
		}

		_plantingWindows.Clear();
		Changed = true;
	}

	private void LoadDefinition(string definition)
	{
		var root = AgricultureXmlExtensions.RootOrDefault(definition, "Crop");
		BaseGrowthDays = System.Math.Clamp((int?)root.Attribute("growthDays") ?? 30, 1, 10000);
		HarvestWindowDays = System.Math.Clamp((int?)root.Attribute("harvestWindowDays") ?? 7, 1, 10000);
		IsPerennial = (bool?)root.Attribute("perennial") ?? false;
		HarvestCycleDays = System.Math.Clamp((int?)root.Attribute("harvestCycleDays") ?? BaseGrowthDays, 1, 10000);
		MinimumMoisture = ((int?)root.Attribute("minMoisture") ?? 20).ClampScore();
		MaximumMoisture = ((int?)root.Attribute("maxMoisture") ?? 85).ClampScore();
		MinimumTemperature = (int?)root.Attribute("minTemperature") ?? 0;
		MaximumTemperature = (int?)root.Attribute("maxTemperature") ?? 45;
		var pollination = root.Element("Pollination");
		PollinationDependency = System.Enum.TryParse<AgriculturePollinationDependency>(
			(string)pollination?.Attribute("dependency") ?? nameof(AgriculturePollinationDependency.None),
			true,
			out var dependency)
			? dependency
			: AgriculturePollinationDependency.None;
		PollinationHealthBonus = PollinationDependency == AgriculturePollinationDependency.None
			? 0
			: System.Math.Clamp((int?)pollination?.Attribute("healthBonus") ?? 0, 0, 1);
		PollinationYieldBonus = PollinationDependency == AgriculturePollinationDependency.None
			? 0
			: System.Math.Clamp((int?)pollination?.Attribute("yieldBonus") ?? 0, 0, 2);
		_plantingWindows.Clear();
		foreach (var element in root.Element("PlantingWindows")?.Elements("Window") ?? Enumerable.Empty<XElement>())
		{
			if (element.TryLoadPlantingWindow(out var window) && window != null)
			{
				_plantingWindows.Add(window);
			}
		}

		_scoreRanges.Clear();
		foreach (var range in root.Element("ScoreRanges")?.LoadScoreRanges().Values ?? Enumerable.Empty<AgricultureScoreRange>())
		{
			_scoreRanges[range.Score] = range;
		}

		_yieldOutputs.Clear();
		foreach (var element in root.Element("Outputs")?.Elements("Commodity") ?? Enumerable.Empty<XElement>())
		{
			var material = (string)element.Attribute("material");
			var weight = (double?)element.Attribute("weight") ?? 0.0;
			if (string.IsNullOrWhiteSpace(material) || weight <= 0.0)
			{
				continue;
			}

			_yieldOutputs.Add(new AgricultureCommodityYield(material, weight, (string)element.Attribute("tag") ?? string.Empty));
		}

		_seedRequirements.Clear();
		foreach (var element in root.Element("Seeds")?.Elements("Commodity") ?? Enumerable.Empty<XElement>())
		{
			var material = (string)element.Attribute("material");
			var weight = (double?)element.Attribute("weight") ?? 0.0;
			if (string.IsNullOrWhiteSpace(material) || weight <= 0.0)
			{
				continue;
			}

			_seedRequirements.Add(new AgricultureCommodityYield(material, weight, (string)element.Attribute("tag") ?? string.Empty));
		}
	}

	private XElement SaveDefinition()
	{
		return new XElement("Crop",
			new XAttribute("growthDays", BaseGrowthDays),
			new XAttribute("harvestWindowDays", HarvestWindowDays),
			new XAttribute("perennial", IsPerennial),
			new XAttribute("harvestCycleDays", HarvestCycleDays),
			new XAttribute("minMoisture", MinimumMoisture),
			new XAttribute("maxMoisture", MaximumMoisture),
			new XAttribute("minTemperature", MinimumTemperature),
			new XAttribute("maxTemperature", MaximumTemperature),
			new XElement("Pollination",
				new XAttribute("dependency", PollinationDependency.ToString()),
				new XAttribute("healthBonus", PollinationHealthBonus),
				new XAttribute("yieldBonus", PollinationYieldBonus)),
			new XElement("PlantingWindows",
				_plantingWindows.Select(x => x.SaveToXml())),
			new XElement("ScoreRanges",
				_scoreRanges.Values.OrderBy(x => x.Score).Select(x => new XElement("Score",
					new XAttribute("type", x.Score.ToString()),
					new XAttribute("min", x.Minimum),
					new XAttribute("max", x.Maximum)))),
			new XElement("Seeds",
				_seedRequirements.Select(x => new XElement("Commodity",
					new XAttribute("material", x.MaterialName),
					new XAttribute("weight", x.BaseWeight),
					string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName)))),
			new XElement("Outputs",
				_yieldOutputs.Select(x => new XElement("Commodity",
					new XAttribute("material", x.MaterialName),
					new XAttribute("weight", x.BaseWeight),
					string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName)))));
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AgricultureCropDefinitions.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.Category = Category;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}

	private static string NormalisePlantingWindowValue(string value)
	{
		return value.Trim().ToLowerInvariant();
	}
}

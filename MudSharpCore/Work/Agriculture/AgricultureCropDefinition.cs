using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Work.Agriculture;

public class AgricultureCropDefinition : SaveableItem, IAgricultureCropDefinition
{
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
		int maximumTemperature)
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

	private void LoadDefinition(string definition)
	{
		var root = AgricultureXmlExtensions.RootOrDefault(definition, "Crop");
		BaseGrowthDays = System.Math.Clamp((int?)root.Attribute("growthDays") ?? 30, 1, 10000);
		HarvestWindowDays = System.Math.Clamp((int?)root.Attribute("harvestWindowDays") ?? 7, 1, 10000);
		MinimumMoisture = ((int?)root.Attribute("minMoisture") ?? 20).ClampScore();
		MaximumMoisture = ((int?)root.Attribute("maxMoisture") ?? 85).ClampScore();
		MinimumTemperature = (int?)root.Attribute("minTemperature") ?? 0;
		MaximumTemperature = (int?)root.Attribute("maxTemperature") ?? 45;
	}

	private XElement SaveDefinition()
	{
		return new XElement("Crop",
			new XAttribute("growthDays", BaseGrowthDays),
			new XAttribute("harvestWindowDays", HarvestWindowDays),
			new XAttribute("minMoisture", MinimumMoisture),
			new XAttribute("maxMoisture", MaximumMoisture),
			new XAttribute("minTemperature", MinimumTemperature),
			new XAttribute("maxTemperature", MaximumTemperature));
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
}

using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.Work.Agriculture;

public class AgricultureWoodlandDefinition : SaveableItem, IAgricultureWoodlandDefinition
{
	public AgricultureWoodlandDefinition(Models.AgricultureWoodlandDefinition definition, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = definition.Id;
		_name = definition.Name;
		Description = definition.Description;
		WoodlandType = definition.WoodlandType;
		LoadDefinition(definition.Definition);
	}

	public AgricultureWoodlandDefinition(IFuturemud gameworld, string name, string description, string woodlandType,
		int establishmentDays, int harvestCycleDays)
	{
		Gameworld = gameworld;
		_name = name;
		Description = description;
		WoodlandType = woodlandType;
		EstablishmentDays = establishmentDays;
		HarvestCycleDays = harvestCycleDays;
		using (new FMDB())
		{
			var dbitem = new Models.AgricultureWoodlandDefinition
			{
				Name = Name,
				Description = Description,
				WoodlandType = WoodlandType,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.AgricultureWoodlandDefinitions.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "AgricultureWoodlandDefinition";
	public string Description { get; private set; }
	public string WoodlandType { get; private set; }
	public int EstablishmentDays { get; private set; }
	public int HarvestCycleDays { get; private set; }

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

	public void BuildingSetWoodlandType(string woodlandType)
	{
		WoodlandType = woodlandType;
		Changed = true;
	}

	public void BuildingSetEstablishmentDays(int value)
	{
		EstablishmentDays = System.Math.Clamp(value, 1, 100000);
		Changed = true;
	}

	public void BuildingSetHarvestCycleDays(int value)
	{
		HarvestCycleDays = System.Math.Clamp(value, 1, 100000);
		Changed = true;
	}

	private void LoadDefinition(string definition)
	{
		var root = AgricultureXmlExtensions.RootOrDefault(definition, "Woodland");
		EstablishmentDays = System.Math.Clamp((int?)root.Attribute("establishmentDays") ?? 90, 1, 100000);
		HarvestCycleDays = System.Math.Clamp((int?)root.Attribute("harvestCycleDays") ?? 365, 1, 100000);
	}

	private XElement SaveDefinition()
	{
		return new XElement("Woodland",
			new XAttribute("establishmentDays", EstablishmentDays),
			new XAttribute("harvestCycleDays", HarvestCycleDays));
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AgricultureWoodlandDefinitions.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.WoodlandType = WoodlandType;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}
}

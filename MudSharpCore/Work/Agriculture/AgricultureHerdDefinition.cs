using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.NPC.Templates;

namespace MudSharp.Work.Agriculture;

public class AgricultureHerdDefinition : SaveableItem, IAgricultureHerdDefinition
{
	private readonly List<AgricultureCommodityYield> _secondaryOutputs = new();
	private long _npcTemplateId;
	private int _npcTemplateRevisionNumber;
	private INPCTemplate _npcTemplate;

	public AgricultureHerdDefinition(Models.AgricultureHerdDefinition definition, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = definition.Id;
		_name = definition.Name;
		Description = definition.Description;
		_npcTemplateId = definition.NpcTemplateId ?? 0;
		_npcTemplateRevisionNumber = definition.NpcTemplateRevisionNumber ?? 0;
		LoadDefinition(definition.Definition);
	}

	public AgricultureHerdDefinition(IFuturemud gameworld, string name, string description, INPCTemplate npcTemplate,
		double animalUnits, double dailyGraze, int maximumCondition,
		IEnumerable<AgricultureCommodityYield> secondaryOutputs = null)
	{
		Gameworld = gameworld;
		_name = name;
		Description = description;
		_npcTemplate = npcTemplate;
		_npcTemplateId = npcTemplate?.Id ?? 0;
		_npcTemplateRevisionNumber = npcTemplate?.RevisionNumber ?? 0;
		AnimalUnits = animalUnits;
		DailyGraze = dailyGraze;
		MaximumCondition = maximumCondition;
		_secondaryOutputs.AddRange(secondaryOutputs ?? Enumerable.Empty<AgricultureCommodityYield>());
		using (new FMDB())
		{
			var dbitem = new Models.AgricultureHerdDefinition
			{
				Name = Name,
				Description = Description,
				NpcTemplateId = _npcTemplateId == 0 ? null : _npcTemplateId,
				NpcTemplateRevisionNumber = _npcTemplateRevisionNumber == 0 ? null : _npcTemplateRevisionNumber,
				Definition = SaveDefinition().ToString()
			};
			FMDB.Context.AgricultureHerdDefinitions.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public override string FrameworkItemType => "AgricultureHerdDefinition";
	public string Description { get; private set; }
	public double AnimalUnits { get; private set; }
	public double DailyGraze { get; private set; }
	public int MaximumCondition { get; private set; }
	public IReadOnlyCollection<AgricultureCommodityYield> SecondaryOutputs => _secondaryOutputs;

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

	public void BuildingSetAnimalUnits(double value)
	{
		AnimalUnits = System.Math.Max(0.01, value);
		Changed = true;
	}

	public void BuildingSetDailyGraze(double value)
	{
		DailyGraze = System.Math.Max(0.0, value);
		Changed = true;
	}

	public void BuildingSetMaximumCondition(int value)
	{
		MaximumCondition = value.ClampScore();
		Changed = true;
	}

	public void BuildingSetSecondaryOutputs(IEnumerable<AgricultureCommodityYield> outputs)
	{
		_secondaryOutputs.Clear();
		_secondaryOutputs.AddRange(outputs ?? Enumerable.Empty<AgricultureCommodityYield>());
		Changed = true;
	}

	public void BuildingSetNpcTemplate(INPCTemplate npcTemplate)
	{
		_npcTemplate = npcTemplate;
		_npcTemplateId = npcTemplate?.Id ?? 0;
		_npcTemplateRevisionNumber = npcTemplate?.RevisionNumber ?? 0;
		Changed = true;
	}

	public INPCTemplate NpcTemplate
	{
		get
		{
			if (_npcTemplate == null && _npcTemplateId != 0)
			{
				_npcTemplate = _npcTemplateRevisionNumber == 0
					? Gameworld.NpcTemplates.Get(_npcTemplateId)
					: Gameworld.NpcTemplates.Get(_npcTemplateId, _npcTemplateRevisionNumber);
			}

			return _npcTemplate;
		}
	}

	public bool CanMaterialise => NpcTemplate != null;

	private void LoadDefinition(string definition)
	{
		var root = AgricultureXmlExtensions.RootOrDefault(definition, "Herd");
		AnimalUnits = (double?)root.Attribute("animalUnits") ?? 1.0;
		DailyGraze = (double?)root.Attribute("dailyGraze") ?? 1.0;
		MaximumCondition = ((int?)root.Attribute("maximumCondition") ?? 100).ClampScore();
		_secondaryOutputs.Clear();
		foreach (var element in root.Element("SecondaryOutputs")?.Elements("Commodity") ?? Enumerable.Empty<XElement>())
		{
			var material = (string)element.Attribute("material");
			var weight = (double?)element.Attribute("weight") ?? 0.0;
			if (string.IsNullOrWhiteSpace(material) || weight <= 0.0)
			{
				continue;
			}

			_secondaryOutputs.Add(new AgricultureCommodityYield(material, weight,
				(string)element.Attribute("tag") ?? string.Empty));
		}
	}

	private XElement SaveDefinition()
	{
		return new XElement("Herd",
			new XAttribute("animalUnits", AnimalUnits),
			new XAttribute("dailyGraze", DailyGraze),
			new XAttribute("maximumCondition", MaximumCondition),
			new XElement("SecondaryOutputs",
				_secondaryOutputs.Select(x => new XElement("Commodity",
					new XAttribute("material", x.MaterialName),
					new XAttribute("weight", x.BaseWeight),
					string.IsNullOrWhiteSpace(x.TagName) ? null : new XAttribute("tag", x.TagName)))));
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.AgricultureHerdDefinitions.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.NpcTemplateId = _npcTemplateId == 0 ? null : _npcTemplateId;
		dbitem.NpcTemplateRevisionNumber = _npcTemplateRevisionNumber == 0 ? null : _npcTemplateRevisionNumber;
		dbitem.Definition = SaveDefinition().ToString();
		Changed = false;
	}
}

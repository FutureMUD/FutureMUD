using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Markets;

public class MarketInfluenceTemplate : SaveableItem, IMarketInfluenceTemplate
{
	public IMarketInfluenceTemplate Clone(string newName)
	{
		return new MarketInfluenceTemplate(this, newName);
	}

	private MarketInfluenceTemplate(MarketInfluenceTemplate rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		Description = rhs.Description;
		TemplateSummary = rhs.TemplateSummary;
		CharacterKnowsAboutInfluenceProg = rhs.CharacterKnowsAboutInfluenceProg;
		using (new FMDB())
		{
			var dbitem = new Models.MarketInfluenceTemplate
			{
				Name = Name,
				Description = Description,
				TemplateSummary = TemplateSummary,
				CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id,
				Impacts = rhs.SaveImpacts().ToString()
			};
			FMDB.Context.MarketInfluenceTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			foreach (var impact in XElement.Parse(dbitem.Impacts).Elements("Impact"))
			{
				_marketImpacts.Add(new MarketImpact
				{
					DemandImpact = double.Parse(impact.Attribute("demand").Value),
					SupplyImpact = double.Parse(impact.Attribute("supply").Value),
					MarketCategory = Gameworld.MarketCategories.Get(long.Parse(impact.Attribute("category").Value))
				});
			}
		}
	}

	public MarketInfluenceTemplate(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		Description = "An undescribed market influence";
		TemplateSummary = "This summary is only used for builders to understand what the template does";
		CharacterKnowsAboutInfluenceProg = Gameworld.AlwaysTrueProg;
		using (new FMDB())
		{
			var dbitem = new Models.MarketInfluenceTemplate
			{
				Name = Name,
				Description = Description,
				TemplateSummary = TemplateSummary,
				CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id,
				Impacts = SaveImpacts().ToString()
			};
			FMDB.Context.MarketInfluenceTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public MarketInfluenceTemplate(IFuturemud gameworld, Models.MarketInfluenceTemplate template)
	{
		Gameworld = gameworld;
		_id = template.Id;
		_name = template.Name;
		Description = template.Description;
		TemplateSummary = template.TemplateSummary;
		CharacterKnowsAboutInfluenceProg = Gameworld.FutureProgs.Get(template.CharacterKnowsAboutInfluenceProgId) ?? Gameworld.AlwaysFalseProg;
		foreach (var impact in XElement.Parse(template.Impacts).Elements("Impact"))
		{
			_marketImpacts.Add(new MarketImpact
			{
				DemandImpact = double.Parse(impact.Attribute("demand").Value),
				SupplyImpact = double.Parse(impact.Attribute("supply").Value),
				MarketCategory = Gameworld.MarketCategories.Get(long.Parse(impact.Attribute("category").Value))
			});
		}
	}

	/// <inheritdoc />
	public sealed override string FrameworkItemType => "MarketInfluenceTemplate";

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.MarketInfluenceTemplates.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.TemplateSummary = TemplateSummary;
		dbitem.CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id;
		dbitem.Impacts = SaveImpacts().ToString();
		Changed = false;
	}

	public XElement SaveImpacts()
	{
		return new XElement("Impacts",
			from impact in _marketImpacts
			select new XElement("Impact",
				new XAttribute("demand", impact.DemandImpact),
				new XAttribute("supply", impact.SupplyImpact),
				new XAttribute("category", impact.MarketCategory.Id)
			)
		);
	}

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new System.NotImplementedException();
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Market Influence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"About Template: {TemplateSummary.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Parameters Passed to Instances".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Character Knows Prog: {CharacterKnowsAboutInfluenceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Impacts:");
		sb.AppendLine();
		foreach (var impact in _marketImpacts)
		{
			sb.AppendLine($"\t{impact.MarketCategory.Name.ColourName()}: Supply {impact.SupplyImpact.ToBonusString()}, Demand {impact.DemandImpact.ToBonusString()}");
		}
		return sb.ToString();
	}

	/// <inheritdoc />
	public string TemplateSummary { get; set; }

	/// <inheritdoc />
	public string Description { get; set; }

	private readonly List<MarketImpact> _marketImpacts = new();

	/// <inheritdoc />
	public IEnumerable<MarketImpact> MarketImpacts => _marketImpacts;

	/// <inheritdoc />
	public IFutureProg CharacterKnowsAboutInfluenceProg { get; set; }
}
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Markets;

public class MarketInfluenceTemplate : SaveableItem, IMarketInfluenceTemplate
{
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
		throw new System.NotImplementedException();
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
		sb.AppendLine($"About Template: {TemplateSummary.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Parameters Passed to Instances".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Character Knows Prog: {CharacterKnowsAboutInfluenceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
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
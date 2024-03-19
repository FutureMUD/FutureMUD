using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using NCalc;
using Expression = ExpressionEngine.Expression;

namespace MudSharp.Economy.Markets;

internal class Market : SaveableItem, IMarket
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "Market";

	public Market(IFuturemud gameworld, string name, IEconomicZone zone)
	{
		Gameworld = gameworld;
		_name = name;
		EconomicZone = zone;
		Description = "An undescribed market.";
		MarketPriceFormula = new Expression("if(demand<=0,0,if(supply<=0,100,1 + (elasticity * min(1, max(-1, (demand-supply) / min(demand,supply))))))", EvaluateOptions.MatchStringsWithIgnoreCase);
	}

	/// <inheritdoc />
	public override void Save()
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Market #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine($"Price Formula: {MarketPriceFormula.OriginalExpression.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Influences:");
		sb.AppendLine();
		foreach (var influence in _marketInfluences)
		{
			sb.AppendLine(influence.TextForMarketShow(actor));
		}

		return sb.ToString();
	}

	/// <inheritdoc />
	public IEconomicZone EconomicZone { get; set; }

	/// <inheritdoc />
	public string Description { get; set; }

	private readonly List<IMarketInfluence> _marketInfluences = new List<IMarketInfluence>();

	/// <inheritdoc />
	public IEnumerable<IMarketInfluence> MarketInfluences => _marketInfluences;

	public Expression MarketPriceFormula { get; private set; }

	/// <inheritdoc />
	public decimal PriceMultiplierForCategory(IMarketCategory category)
	{
		var now = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		var influences = _marketInfluences
		                .Where(x => x.Applies(category, now))
		                .SelectMany(x => x.MarketImpacts)
		                .Where(x => x.MarketCategory == category)
		                .ToList();
		var supply = influences.Sum(x => x.SupplyImpact) + 1;
		var demand = influences.Sum(x => x.DemandImpact) + 1;
		var elasticity = supply > demand ? category.ElasticityFactorBelow : category.ElasticityFactorAbove;

		return MarketPriceFormula.EvaluateDecimalWith(
			("supply", supply),
			("demand", demand),
			("elasticity", elasticity)
		);
	}

	/// <inheritdoc />
	public void ApplyMarketInfluence(IMarketInfluence influence)
	{
		_marketInfluences.Add(influence);
	}

	/// <inheritdoc />
	public void RemoveMarketInfluence(IMarketInfluence influence)
	{
		_marketInfluences.Remove(influence);
	}
}
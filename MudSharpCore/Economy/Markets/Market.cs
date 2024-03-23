using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using NCalc;
using Expression = ExpressionEngine.Expression;

namespace MudSharp.Economy.Markets;

internal class Market : SaveableItem, IMarket
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "Market";

	public Market(IFuturemud gameworld, Models.Market dbitem)
	{
		Gameworld = gameworld;
		_id = dbitem.Id;
		_name = dbitem.Name;
		Description = dbitem.Description;
		EconomicZone = Gameworld.EconomicZones.Get(dbitem.EconomicZoneId);
		MarketPriceFormula = new Expression(dbitem.MarketPriceFormula, EvaluateOptions.IgnoreCase);
		foreach (var item in dbitem.MarketMarketCategories)
		{
			_marketCategories.AddNotNull(Gameworld.MarketCategories.Get(item.MarketCategoryId));
		}

		foreach (var item in dbitem.Influences)
		{
			_marketInfluences.Add(new MarketInfluence(this, item));
		}
	}

	public Market(Market rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		Description = rhs.Description;
		EconomicZone = rhs.EconomicZone;
		MarketPriceFormula = new Expression(rhs.MarketPriceFormula.OriginalExpression, EvaluateOptions.IgnoreCase);
		_marketCategories.AddRange(rhs.MarketCategories);
		foreach (var influence in _marketInfluences)
		{

		}
		using (new FMDB())
		{
			var dbitem = new Models.Market
			{
				Name = Name,
				Description = Description,
				EconomicZoneId = EconomicZone.Id,
				MarketPriceFormula = MarketPriceFormula.OriginalExpression
			};
			dbitem.MarketMarketCategories = new HashSet<MarketMarketCategory>(rhs.MarketCategories.Select(x =>
				new MarketMarketCategory
				{
					Market = dbitem,
					MarketCategoryId = x.Id
				}));
			foreach (var influence in rhs.MarketInfluences)
			{
				var dbinfluence = new Models.MarketInfluence
				{
					Name = influence.Name,
					Description = influence.Description,
					AppliesFrom = influence.AppliesFrom.GetDateTimeString(),
					AppliesUntil = influence.AppliesUntil?.GetDateTimeString(),
					CharacterKnowsAboutInfluenceProgId = influence.CharacterKnowsAboutInfluenceProg.Id,
					MarketInfluenceTemplateId = influence.MarketInfluenceTemplate?.Id,
					Market = dbitem,
					Impacts = influence.SaveImpacts().ToString()

				};
				dbitem.Influences.Add(dbinfluence);
			}
			FMDB.Context.Markets.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Market(IFuturemud gameworld, string name, IEconomicZone zone)
	{
		Gameworld = gameworld;
		_name = name;
		EconomicZone = zone;
		Description = "An undescribed market.";
		MarketPriceFormula = new Expression("if(demand<=0,0,if(supply<=0,100,1 + (elasticity * min(1, max(-1, (demand-supply) / min(demand,supply))))))", EvaluateOptions.IgnoreCase);
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
		sb.AppendLine("Categories:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _marketCategories
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.ElasticityFactorBelow.ToString("N3", actor),
				item.ElasticityFactorAbove.ToString("N3", actor),
				PriceMultiplierForCategory(item).ToString("P3", actor),
				Gameworld.ItemProtos.GetAllApprovedOrMostRecent().Count(x => item.BelongsToCategory(x)).ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"E(Under)",
				"E(Over)",
				"Current Price %"
			},
			actor,
			Telnet.BoldYellow
		));
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

	private readonly List<IMarketInfluence> _marketInfluences = new();

	/// <inheritdoc />
	public IEnumerable<IMarketInfluence> MarketInfluences => _marketInfluences;

	private readonly List<IMarketCategory> _marketCategories = new();
	public IEnumerable<IMarketCategory> MarketCategories => _marketCategories;

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
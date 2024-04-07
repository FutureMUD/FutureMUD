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
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
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
		foreach (var item in dbitem.MarketCategories)
		{
			_marketCategories.AddNotNull(Gameworld.MarketCategories.Get(item.Id));
		}

		foreach (var influence in dbitem.Influences)
		{
			var inf = new MarketInfluence(this, influence);
			_marketInfluences.Add(inf);
			Gameworld.Add(inf);
		}
	}

	public IMarket Clone(string newName)
	{
		return new Market(this, newName);
	}

	public Market(Market rhs, string name)
	{
		Gameworld = rhs.Gameworld;
		_name = name;
		Description = rhs.Description;
		EconomicZone = rhs.EconomicZone;
		MarketPriceFormula = new Expression(rhs.MarketPriceFormula.OriginalExpression, EvaluateOptions.IgnoreCase);
		_marketCategories.AddRange(rhs.MarketCategories);
		using (new FMDB())
		{
			var dbitem = new Models.Market
			{
				Name = Name,
				Description = Description,
				EconomicZoneId = EconomicZone.Id,
				MarketPriceFormula = MarketPriceFormula.OriginalExpression
			};
			dbitem.MarketCategories = new HashSet<Models.MarketCategory>(rhs.MarketCategories.Select(x => FMDB.Context.MarketCategories.Find(x.Id)));
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

			foreach (var influence in dbitem.Influences)
			{
				var inf = new MarketInfluence(this, influence);
				_marketInfluences.Add(inf);
				Gameworld.Add(inf);
			}
		}
	}

	public Market(IFuturemud gameworld, string name, IEconomicZone zone)
	{
		Gameworld = gameworld;
		_name = name;
		EconomicZone = zone;
		Description = "An undescribed market.";
		MarketPriceFormula = new Expression("if(demand<=0,0,if(supply<=0,100,1 + (elasticity * min(1, max(-1, (demand-supply) / min(demand,supply))))))", EvaluateOptions.IgnoreCase);
		using (new FMDB())
		{
			var dbitem = new Models.Market
			{
				Name = name,
				EconomicZoneId = zone.Id,
				Description = Description,
				MarketPriceFormula = MarketPriceFormula.OriginalExpression
			};
			FMDB.Context.Markets.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.Markets.Find(Id);
		dbitem.Name = Name;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.Description = Description;
		dbitem.MarketPriceFormula = MarketPriceFormula.OriginalExpression;
		dbitem.MarketCategories.Clear();
		foreach (var item in MarketCategories)
		{
			dbitem.MarketCategories.Add(FMDB.Context.MarketCategories.Find(item.Id));
		}
		Changed = false;
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
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
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
				NetSupply(item).ToString("P2", actor),
				NetDemand(item).ToString("P2", actor),
				PriceMultiplierForCategory(item).ToString("P3", actor),
				Gameworld.ItemProtos.GetAllApprovedOrMostRecent().Count(x => item.BelongsToCategory(x)).ToString("N0", actor)
			},
			new List<string>
			{
				"Id",
				"Name",
				"E(Under)",
				"E(Over)",
				"Supply",
				"Demand",
				"Current Price %",
				"# Items"
			},
			actor,
			Telnet.BoldYellow
		));
		sb.AppendLine();
		sb.AppendLine("Influences:");
		sb.AppendLine();
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in MarketInfluences
			select new List<string>
			{
				item.Id.ToString("N0", actor),
				item.Name,
				item.AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				item.AppliesUntil?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short) ?? "Until Removed",
				item.Applies(null, EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime).ToColouredString()
			},
			new List<string>
			{
				"Id",
				"Name",
				"From",
				"Until",
				"Active?"
			},
			actor,
			Telnet.BoldYellow
		));
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

	public IReadOnlyCollection<MarketImpact> ApplicableMarketImpacts(IMarketCategory category)
	{
		var now = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		return _marketInfluences
		       .Where(x => x.Applies(category, now))
		       .SelectMany(x => x.MarketImpacts)
		       .Where(x => x.MarketCategory == category)
		       .ToList();
	}

	public double NetDemand(IMarketCategory category)
	{
		return ApplicableMarketImpacts(category).Sum(x => x.DemandImpact) + 1;
	}

	public double NetSupply(IMarketCategory category)
	{
		return ApplicableMarketImpacts(category).Sum(x => x.SupplyImpact) + 1;
	}

	/// <inheritdoc />
	public decimal PriceMultiplierForCategory(IMarketCategory category)
	{
		var impacts = ApplicableMarketImpacts(category);
		var supply = impacts.Sum(x => x.SupplyImpact) + 1;
		var demand = impacts.Sum(x => x.DemandImpact) + 1;
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
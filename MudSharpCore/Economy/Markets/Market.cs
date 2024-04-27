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
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
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

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - changes the name
	#3ez <zone>#0 - changes the economic zone
	#3category <which>#0 - toggles a category as being part of the market
	#3desc#0 - drops you into an editor for the market's description
	#3formula <formula>#0 - edits the market's price formula

In the market price formula, you can use the following variables:

	#6elasticity#0 - the elasticity of the market good
	#6supply#0 - the net supply percentage
	#6demand#0 - the net demand percentage";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "ez":
			case "economiczone":
			case "zone":
				return BuildingCommandEconomicZone(actor, command);
			case "formula":
				return BuildingCommandFormula(actor, command);
			case "category":
				return BuildingCommandCategory(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor);

		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDescription(ICharacter actor)
	{
		actor.EditorMode(DescriptionPost, DescriptionCancel, suppliedArguments: [actor.InnerLineFormatLength]);
		return true;
	}

	private void DescriptionPost(string text, IOutputHandler handler, object[] args)
	{
		Description = text;
		Changed = true;
		handler.Send($"You set the description for this market to:\n\n{Description.Wrap((int)args[0], "\t")}");
	}

	private void DescriptionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the description.");
	}

	private bool BuildingCommandCategory(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which market category do you want to toggle as belonging to this market?");
			return false;
		}

		var category = Gameworld.MarketCategories.GetByIdOrName(command.SafeRemainingArgument);
		if (category is null)
		{
			actor.OutputHandler.Send("There is no such market category.");
			return false;
		}

		Changed = true;
		if (_marketCategories.Contains(category))
		{
			_marketCategories.Remove(category);
			actor.OutputHandler.Send($"This market will no longer contain the {category.Name.ColourValue()} market category.");
			return true;
		}

		_marketCategories.Add(category);
		actor.OutputHandler.Send($"This market will now contain the {category.Name.ColourValue()} market category.");
		return true;
	}

	private bool BuildingCommandFormula(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What formula would you like to use for prices for this market?");
			return false;
		}

		var formula = new Expression(command.SafeRemainingArgument, EvaluateOptions.IgnoreCase);
		if (formula.HasErrors())
		{
			actor.OutputHandler.Send(formula.Error);
			return false;
		}

		MarketPriceFormula = formula;
		Changed = true;
		actor.OutputHandler.Send($"The formula for this market's prices is now {formula.OriginalExpression.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this market?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.Markets.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a market called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename this market from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	private bool BuildingCommandEconomicZone(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which economic zone do you want to change this market to?");
			return false;
		}

		var ez = Gameworld.EconomicZones.GetByIdOrName(command.SafeRemainingArgument);
		if (ez is null)
		{
			actor.OutputHandler.Send("There is no such economic zones.");
			return false;
		}

		EconomicZone = ez;
		Changed = true;
		actor.OutputHandler.Send($"This market now belongs to the {ez.Name.ColourName()} economic zones.");
		return true;
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
		if (category is null)
		{
			return 1.0M;
		}

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

	public decimal PriceMultiplierForItem(IGameItem item)
	{
		return MarketCategories.Where(x => x.BelongsToCategory(item))
		                       .Select(x => PriceMultiplierForCategory(x))
		                       .DefaultIfEmpty(1.0M)
		                       .Max();
	}

	public decimal PriceMultiplierForItem(IGameItemProto item)
	{
		return MarketCategories.Where(x => x.BelongsToCategory(item))
		                       .Select(x => PriceMultiplierForCategory(x))
		                       .DefaultIfEmpty(1.0M)
		                       .Max();
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

	#region FutureProgs

	/// <inheritdoc />
	public FutureProgVariableTypes Type => FutureProgVariableTypes.Market;

	/// <inheritdoc />
	public object GetObject => this;

	/// <inheritdoc />
	public IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "categories":
				return new CollectionVariable(_marketCategories.ToList(), FutureProgVariableTypes.MarketCategory);
			case "influences":
				return new DictionaryVariable(_marketInfluences.ToDictionary<IMarketInfluence, string, IFutureProgVariable>(x => x.Name, x => new NumberVariable(x.Id)), FutureProgVariableTypes.Number);
		}

		throw new ArgumentOutOfRangeException(nameof(property));
	}

	private static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", FutureProgVariableTypes.Text },
			{ "id", FutureProgVariableTypes.Number },
			{ "categories", FutureProgVariableTypes.MarketCategory | FutureProgVariableTypes.Collection },
			{ "influences", FutureProgVariableTypes.Dictionary | FutureProgVariableTypes.Number }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", "The name of the market" },
			{ "id", "The Id of the market" },
			{ "categories", "The market categories that apply in this market" },
			{ "influences", "A dictionary with the names and IDs of influences effecting this market"}
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Market, DotReferenceHandler(),
			DotReferenceHelp());
	}
	#endregion
}
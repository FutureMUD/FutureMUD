using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using JetBrains.Annotations;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Currency;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

#nullable enable
namespace MudSharp.Economy.Markets;

internal class MarketPopulation : SaveableItem, IMarketPopulation
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "MarketPopulation";

	public MarketPopulation(IFuturemud gameworld, Models.MarketPopulation population)
	{
		Gameworld = gameworld;
		_id = population.Id;
		_name = population.Name;
		Market = Gameworld.Markets.Get(population.MarketId)!;
		Description = population.Description;
		PopulationScale = population.PopulationScale;
		LoadNeeds(XElement.Parse(population.MarketPopulationNeeds));
		LoadStresses(XElement.Parse(population.MarketStressPoints));
		RecalculateStress();
	}

	private void LoadNeeds(XElement root)
	{
		foreach (var item in root.Elements("Need"))
		{
			_marketPopulationNeeds.Add(new MarketPopulationNeed
			{
				MarketCategory = Gameworld.MarketCategories.Get(long.Parse(item.Attribute("category").Value)),
				BaseExpenditure = decimal.Parse(item.Attribute("expenditure").Value)
			});
		}
	}

	private void LoadStresses(XElement root)
	{
		foreach (var item in root.Elements("Stress"))
		{
			_marketStressPoints.Add(new MarketStressPoint
			{
				Name = item.Attribute("name").Value,
				Description = item.Value,
				StressThreshold = decimal.Parse(item.Attribute("stress").Value),
				ExecuteOnStart = Gameworld.FutureProgs.Get(long.Parse(item.Attribute("onstart").Value)),
				ExecuteOnEnd = Gameworld.FutureProgs.Get(long.Parse(item.Attribute("onend").Value))
			});
		}
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.MarketPopulations.Find(Id);
		dbitem!.Name = Name;
		dbitem.Description = Description;
		dbitem.PopulationScale = PopulationScale;
		dbitem.MarketStressPoints = SaveStresses().ToString();
		dbitem.MarketPopulationNeeds = SaveNeeds().ToString();
		Changed = false;
	}

	public XElement SaveNeeds()
	{
		return new XElement("Needs",
			from item in _marketPopulationNeeds
			select new XElement("Need",
				new XAttribute("category", item.MarketCategory.Id),
				new XAttribute("expenditure", item.BaseExpenditure)
			)
		);
	}

	public XElement SaveStresses()
	{
		return new XElement("Stresses",
			from item in _marketStressPoints
			select new XElement("Stress",
				new XAttribute("stress", item.StressThreshold),
				new XAttribute("onstart", item.ExecuteOnStart?.Id ?? 0L),
				new XAttribute("onend", item.ExecuteOnEnd?.Id ?? 0L),
				new XAttribute("name", item.Name),
				new XCData(item.Description)
			)
		);
	}

	public const string HelpText = @"";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor);
			case "population":
			case "scale":
			case "popscale":
			case "pop":
			case "populationscale":
				return BuildingCommandPopulationScale(actor, command);
			case "need":
				return BuildingCommandNeed(actor, command);
			case "stress":
			case "stressthreshold":
			case "threshold":
				return BuildingCommandStressThreshold(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return true;
	}

	private bool BuildingCommandStressThreshold(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which stress threshold did you want to add or edit?");
			return false;
		}

		if (command.PeekSpeech().EqualTo("add"))
		{
			return BuildingCommandStressThresholdAdd(actor, command);
		}

		if (!decimal.TryParse(command.PopSpeech(), out var value))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid number.");
			return false;
		}

		if (_marketStressPoints.All(x => x.StressThreshold != value))
		{
			actor.OutputHandler.Send("There is no market stress threshold with that value. You must add one first.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "remove":
				return BuildingCommandStressThresholdRemove(actor, value);
			case "name":
				return BuildingCommandStressTresholdName(actor, value, command);
			case "desc":
			case "description":
				return BuildingCommandStressThresholdDescription(actor, value);
			case "onstart":
				return BuildingCommandStressTresholdOnStart(actor, value, command);
			case "onend":
				return BuildingCommandStressThresholdOnEnd(actor, value, command);
			default:
				actor.OutputHandler.Send("You must specify either #3remove#0, #3name#0, #3desc#0, #3onstart#0 or #3onend#0.");
				return false;
		}
	}

	private bool BuildingCommandStressThresholdOnEnd(ICharacter actor, decimal result, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify a prog or use #3none#0 to clear the existing one.");
			return false;
		}

		var stress = _marketStressPoints.First(x => x.StressThreshold == result);

		if (command.SafeRemainingArgument.EqualToAny("none", "remove", "delete"))
		{
			_marketStressPoints[_marketStressPoints.IndexOf(stress)] = stress with { ExecuteOnEnd = null };
			Changed = true;
			actor.OutputHandler.Send("This stress threshold will no longer execute any prog when it ends.");
			return true;
		}

		//var prog = new FutureProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument, FutureProgVariableTypes.Void, [])
		throw new NotImplementedException();
	}

	private bool BuildingCommandStressTresholdOnStart(ICharacter actor, decimal result, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandStressThresholdDescription(ICharacter actor, decimal result)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandStressTresholdName(ICharacter actor, decimal result, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandStressThresholdRemove(ICharacter actor, decimal result)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandStressThresholdAdd(ICharacter actor, StringStack command)
	{
		throw new NotImplementedException();
	}

	private bool BuildingCommandNeed(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which market category do you want to create a population need for?");
			return false;
		}

		var category = Gameworld.MarketCategories.GetByIdOrName(command.PopSpeech());
		if (category is null)
		{
			actor.OutputHandler.Send("There is no such market category.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"How much money (in {Market.EconomicZone.Currency.Name.ColourValue()}) should this population spend on this need?");
			return false;
		}

		if (!Market.EconomicZone.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"That is not a valid amount of currency in {Market.EconomicZone.Currency.Name.ColourValue()}.");
			return false;
		}

		_marketPopulationNeeds.RemoveAll(x => x.MarketCategory == category);
		Changed = true;
		if (value <= 0.0M)
		{
			actor.OutputHandler.Send($"This population will no longer need any {category.Name.ColourName()}.");
			return true;
		}

		_marketPopulationNeeds.Add(new MarketPopulationNeed
		{
			MarketCategory = category,
			BaseExpenditure = value
		});
		
		actor.OutputHandler.Send($"This population will now prefer to spend {Market.EconomicZone.Currency.Describe(value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} on {category.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandPopulationScale(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What number of people should belong to this population?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("You must enter a valid whole number greater than zero.");
			return false;
		}

		PopulationScale = value;
		Changed = true;
		actor.OutputHandler.Send($"This market population now consists of {value.ToString("N0", actor).ColourValue()} {"individual".Pluralise(value != 1)}.");
		return true;
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
		handler.Send($"You set the description for this market population to:\n\n{Description.Wrap((int)args[0], "\t")}");
	}

	private void DescriptionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the description.");
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this market population?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MarketPopulations.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a market population called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename this market population from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Market Population #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Market: {Market.Name.ColourValue()}");
		sb.AppendLine($"Population Scale: {PopulationScale.ToString("N0", actor).ColourValue()}");
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine();
		sb.AppendLine("Needs:");
		sb.AppendLine();
		foreach (var need in MarketPopulationNeeds.OrderBy(x=> x.MarketCategory.Name))
		{
			sb.AppendLine($"\t{need.MarketCategory.Name.ColourName()}: {Market.EconomicZone.Currency.Describe(need.BaseExpenditure, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		}
		sb.AppendLine();
		sb.AppendLine("Stress Points:");
		sb.AppendLine();
		var active = CurrentStressPoint;
		foreach (var stress in MarketStressPoints.OrderBy(x => x.StressThreshold))
		{
			sb.AppendLine($">={stress.StressThreshold.ToString("N3", actor)}".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
			if (stress == active)
			{
				sb.AppendLine($">={stress.StressThreshold.ToString("N3", actor)} (Active)".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
			}
			else
			{
				sb.AppendLine($">={stress.StressThreshold.ToString("N3", actor)}".GetLineWithTitle(actor, Telnet.Red, Telnet.BoldWhite));
			}

			sb.AppendLine();
			sb.AppendLine(stress.Description.Wrap(actor.InnerLineFormatLength).ColourCommand());
			sb.AppendLine();
			sb.AppendLine($"On Start Prog: {stress.ExecuteOnStart?.MXPClickableFunctionName() ?? "None".ColourError()}");
			sb.AppendLine($"On End Prog: {stress.ExecuteOnEnd?.MXPClickableFunctionName() ?? "None".ColourError()}");
		}
		return sb.ToString();
	}

	/// <inheritdoc />
	public int PopulationScale { get; set; }

	public string Description { get; set; }

	/// <inheritdoc />
	public IMarket Market { get; set; }

	/// <inheritdoc />
	public decimal CurrentStress
	{
		get;
		private set;
	}

	private readonly List<MarketPopulationNeed> _marketPopulationNeeds = new();

	/// <inheritdoc />
	public IEnumerable<MarketPopulationNeed> MarketPopulationNeeds => _marketPopulationNeeds;

	private readonly List<MarketStressPoint> _marketStressPoints = new();

	/// <inheritdoc />
	public IEnumerable<MarketStressPoint> MarketStressPoints => _marketStressPoints;

	private void RecalculateStress()
	{
		var expectedSpend = MarketPopulationNeeds.Sum(x => x.BaseExpenditure);
		var actualSpend = MarketPopulationNeeds.Sum(x => Market.PriceMultiplierForCategory(x.MarketCategory) * x.BaseExpenditure);
		CurrentStress = (actualSpend / expectedSpend) - 1.0M;
	}

	private MarketStressPoint? CurrentStressPoint => _marketStressPoints
	                                                 .Where(x => x.StressThreshold <= CurrentStress)
	                                                 .FirstMax(x => x.StressThreshold);

	/// <inheritdoc />
	public void MarketPopulationHeartbeat()
	{
		var previous = CurrentStress;
		var old = CurrentStressPoint;
		RecalculateStress();
		var stress = CurrentStressPoint;
		if (old == stress)
		{
			return;
		}

		old?.ExecuteOnEnd?.Execute();
		stress?.ExecuteOnStart?.Execute();
	}
}
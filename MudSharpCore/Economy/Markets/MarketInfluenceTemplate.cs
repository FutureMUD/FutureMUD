using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
			Models.MarketInfluenceTemplate dbitem = new()
			{
				Name = Name,
				Description = Description,
				TemplateSummary = TemplateSummary,
				CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id,
				Impacts = rhs.SaveImpacts().ToString(),
				PopulationImpacts = rhs.SavePopulationImpacts().ToString()
			};
			FMDB.Context.MarketInfluenceTemplates.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
			LoadImpacts(dbitem.Impacts);
			LoadPopulationImpacts(dbitem.PopulationImpacts);
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
			Models.MarketInfluenceTemplate dbitem = new()
			{
				Name = Name,
				Description = Description,
				TemplateSummary = TemplateSummary,
				CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id,
				Impacts = SaveImpacts().ToString(),
				PopulationImpacts = SavePopulationImpacts().ToString()
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
		CharacterKnowsAboutInfluenceProg =
			Gameworld.FutureProgs.Get(template.CharacterKnowsAboutInfluenceProgId) ?? Gameworld.AlwaysTrueProg;
		LoadImpacts(template.Impacts);
		LoadPopulationImpacts(template.PopulationImpacts);
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
		dbitem.PopulationImpacts = SavePopulationImpacts().ToString();
		Changed = false;
	}

	public XElement SaveImpacts()
	{
		return new XElement("Impacts",
			from impact in _marketImpacts
			select new XElement("Impact",
				new XAttribute("demand", impact.DemandImpact),
				new XAttribute("supply", impact.SupplyImpact),
				new XAttribute("price", impact.FlatPriceImpact),
				new XAttribute("category", impact.MarketCategory.Id)
			)
		);
	}

	public XElement SavePopulationImpacts()
	{
		return new XElement("PopulationImpacts",
			from impact in _populationIncomeImpacts
			select new XElement("PopulationImpact",
				new XAttribute("population", impact.MarketPopulation.Id),
				new XAttribute("additive", impact.AdditiveIncomeImpact),
				new XAttribute("multiplier", impact.MultiplicativeIncomeImpact)
			)
		);
	}

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - sets a new name
	#3about#0 - drops you into an editor to write an about info for builders
	#3desc#0 - drops you into an editor to write a description for players
	#3know <prog>#0 - sets the prog that controls if players know about this
	#3impact <category> <supply%> <demand%> [<price%>]#0 - adds or edits an impact for a category
	#3remimpact <category>#0 - removes the impact for a category
	#3popimpact <population> <additive%> <multiplier>#0 - adds or edits an income impact for a population
	#3rempopimpact <population>#0 - removes an income impact for a population";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "about":
				return BuildingCommandAbout(actor);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor);
			case "prog":
			case "know":
			case "knows":
			case "knowprog":
			case "knowsprog":
				return BuildingCommandProg(actor, command);
			case "impact":
			case "addimpact":
				return BuildingCommandImpact(actor, command);
			case "removeimpact":
			case "remimpact":
			case "deleteimpact":
			case "delimpact":
				return BuildingCommandRemoveImpact(actor, command);
			case "popimpact":
			case "addpopimpact":
				return BuildingCommandPopulationImpact(actor, command);
			case "rempopimpact":
			case "rempopulationimpact":
			case "deletepopimpact":
			case "delpopimpact":
				return BuildingCommandRemovePopulationImpact(actor, command);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandImpact(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which category do you want to set an impact for?");
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
			actor.OutputHandler.Send("By what percentage should supply increase?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var supply))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid percentage.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("By what percentage should demand increase?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentage(actor.Account.Culture, out var demand))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid percentage.");
			return false;
		}

		double price = 0.0;
		if (!command.IsFinished && !command.PopSpeech().TryParsePercentage(actor.Account.Culture, out price))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid percentage.");
			return false;
		}

		_marketImpacts.RemoveAll(x => x.MarketCategory == category);
		_marketImpacts.Add(new MarketImpact
		{
			MarketCategory = category,
			SupplyImpact = supply,
			DemandImpact = demand,
			FlatPriceImpact = price
		});
		actor.OutputHandler.Send(
			$"You set the impact for the {category.Name.ColourValue()} market category to {supply.ToBonusPercentageString(actor)} supply, {demand.ToBonusPercentageString(actor)} demand and {price.ToBonusPercentageString(actor)} flat price.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandRemoveImpact(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which category would you like to remove impacts for?");
			return false;
		}

		var category = Gameworld.MarketCategories.GetByIdOrName(command.SafeRemainingArgument);
		if (category is null)
		{
			actor.OutputHandler.Send("There is no such market category.");
			return false;
		}

		if (_marketImpacts.All(x => x.MarketCategory != category))
		{
			actor.OutputHandler.Send("There is no impact to that market category.");
			return false;
		}

		_marketImpacts.RemoveAll(x => x.MarketCategory == category);
		actor.OutputHandler.Send(
			$"You remove all impacts associated with the {category.Name.ColourValue()} market category.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandPopulationImpact(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which market population do you want to set an income impact for?");
			return false;
		}

		var population = Gameworld.MarketPopulations.GetByIdOrName(command.PopSpeech());
		if (population is null)
		{
			actor.OutputHandler.Send("There is no such market population.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What additive income-factor impact should apply?");
			return false;
		}

		if (!command.PopSpeech().TryParsePercentageDecimal(actor.Account.Culture, out var additiveImpact))
		{
			actor.OutputHandler.Send($"The text {command.Last.ColourCommand()} is not a valid percentage.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What multiplicative income-factor should apply?");
			return false;
		}

		if (!TryParseDecimalOrPercentage(actor, command.PopSpeech(), out var multiplier) || multiplier < 0.0M)
		{
			actor.OutputHandler.Send("You must enter a non-negative decimal value or percentage.");
			return false;
		}

		_populationIncomeImpacts.RemoveAll(x => x.MarketPopulation == population);
		_populationIncomeImpacts.Add(new MarketPopulationIncomeImpact
		{
			MarketPopulation = population,
			AdditiveIncomeImpact = additiveImpact,
			MultiplicativeIncomeImpact = multiplier
		});
		actor.OutputHandler.Send(
			$"You set the income impact for {population.Name.ColourName()} to {additiveImpact.ToString("P2", actor).ColourValue()} additive income and {multiplier.ToString("P2", actor).ColourValue()} multiplicative income.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandRemovePopulationImpact(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which market population would you like to remove income impacts for?");
			return false;
		}

		var population = Gameworld.MarketPopulations.GetByIdOrName(command.SafeRemainingArgument);
		if (population is null)
		{
			actor.OutputHandler.Send("There is no such market population.");
			return false;
		}

		if (_populationIncomeImpacts.All(x => x.MarketPopulation != population))
		{
			actor.OutputHandler.Send("There is no income impact for that market population.");
			return false;
		}

		_populationIncomeImpacts.RemoveAll(x => x.MarketPopulation == population);
		actor.OutputHandler.Send(
			$"You remove all income impacts associated with the {population.Name.ColourValue()} market population.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What prog would you like to use to control whether players know about this market influence?");
			return false;
		}

		var prog = new ProgLookupFromBuilderInput(Gameworld, actor, command.SafeRemainingArgument,
			ProgVariableTypes.Boolean, [ProgVariableTypes.Character]).LookupProg();
		if (prog is null)
		{
			return false;
		}

		CharacterKnowsAboutInfluenceProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"Market influences created by this template will now use the {prog.MXPClickableFunctionName()} prog to control whether players know about them.");
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
		handler.Send($"You set the description for this template to:\n\n{Description.Wrap((int)args[0], "\t")}");
	}

	private void DescriptionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the description.");
	}

	private bool BuildingCommandAbout(ICharacter actor)
	{
		actor.EditorMode(AboutPost, AboutCancel, suppliedArguments: [actor.InnerLineFormatLength]);
		return true;
	}

	private void AboutPost(string text, IOutputHandler handler, object[] args)
	{
		TemplateSummary = text;
		Changed = true;
		handler.Send($"You set the about information for this template to:\n\n{TemplateSummary.Wrap((int)args[0], "\t")}");
	}

	private void AboutCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the about information.");
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this market influence template?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MarketInfluenceTemplates.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a market influence template called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename this market influence template from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		StringBuilder sb = new();
		sb.AppendLine($"Market Influence Template #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor,
			Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"About Template: {TemplateSummary.ColourCommand()}");
		sb.AppendLine();
		sb.AppendLine("Parameters Passed to Instances".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine(
			$"Character Knows Prog: {CharacterKnowsAboutInfluenceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Impacts:");
		sb.AppendLine();
		foreach (var impact in _marketImpacts)
		{
			sb.AppendLine(
				$"\t{impact.MarketCategory.Name.ColourName()}: Supply {impact.SupplyImpact.ToBonusString()}, Demand {impact.DemandImpact.ToBonusString()}, Flat Price {impact.FlatPriceImpact.ToBonusString()}");
		}

		if (_populationIncomeImpacts.Any())
		{
			sb.AppendLine();
			sb.AppendLine("Population Income Impacts:");
			sb.AppendLine();
			foreach (var impact in _populationIncomeImpacts)
			{
				sb.AppendLine(
					$"\t{impact.MarketPopulation.Name.ColourName()}: Additive {impact.AdditiveIncomeImpact.ToString("P2", actor).ColourValue()}, Multiplier {impact.MultiplicativeIncomeImpact.ToString("P2", actor).ColourValue()}");
			}
		}

		return sb.ToString();
	}

	/// <inheritdoc />
	public string TemplateSummary { get; set; }

	/// <inheritdoc />
	public string Description { get; set; }

	private readonly List<MarketImpact> _marketImpacts = [];

	/// <inheritdoc />
	public IEnumerable<MarketImpact> MarketImpacts => _marketImpacts;

	private readonly List<MarketPopulationIncomeImpact> _populationIncomeImpacts = [];

	/// <inheritdoc />
	public IEnumerable<MarketPopulationIncomeImpact> PopulationIncomeImpacts => _populationIncomeImpacts;

	/// <inheritdoc />
	public IFutureProg CharacterKnowsAboutInfluenceProg { get; set; }

	private void LoadImpacts(string impacts)
	{
		foreach (var impact in XElement.Parse(impacts).Elements("Impact"))
		{
			_marketImpacts.Add(new MarketImpact
			{
				DemandImpact = double.Parse(impact.Attribute("demand")!.Value, CultureInfo.InvariantCulture),
				SupplyImpact = double.Parse(impact.Attribute("supply")!.Value, CultureInfo.InvariantCulture),
				FlatPriceImpact = double.Parse(impact.Attribute("price")?.Value ?? "0", CultureInfo.InvariantCulture),
				MarketCategory = Gameworld.MarketCategories.Get(long.Parse(impact.Attribute("category")!.Value))
			});
		}
	}

	private void LoadPopulationImpacts(string? impacts)
	{
		if (string.IsNullOrWhiteSpace(impacts))
		{
			return;
		}

		foreach (var impact in XElement.Parse(impacts).Elements("PopulationImpact"))
		{
			var populationId = long.Parse(impact.Attribute("population")!.Value);
			var additiveImpact = decimal.Parse(impact.Attribute("additive")?.Value ?? "0",
				CultureInfo.InvariantCulture);
			var multiplicativeImpact = decimal.Parse(impact.Attribute("multiplier")?.Value ?? "1",
				CultureInfo.InvariantCulture);
			var population = Gameworld.MarketPopulations.Get(populationId);
			if (population is null)
			{
				_unresolvedPopulationIncomeImpacts.Add((populationId, additiveImpact, multiplicativeImpact));
				continue;
			}

			_populationIncomeImpacts.Add(new MarketPopulationIncomeImpact
			{
				MarketPopulation = population,
				AdditiveIncomeImpact = additiveImpact,
				MultiplicativeIncomeImpact = multiplicativeImpact
			});
		}
	}

	public void ResolvePopulationImpacts()
	{
		if (!_unresolvedPopulationIncomeImpacts.Any())
		{
			return;
		}

		foreach (var impact in _unresolvedPopulationIncomeImpacts.ToList())
		{
			var population = Gameworld.MarketPopulations.Get(impact.PopulationId);
			if (population is null)
			{
				continue;
			}

			_populationIncomeImpacts.Add(new MarketPopulationIncomeImpact
			{
				MarketPopulation = population,
				AdditiveIncomeImpact = impact.AdditiveIncomeImpact,
				MultiplicativeIncomeImpact = impact.MultiplicativeIncomeImpact
			});
			_unresolvedPopulationIncomeImpacts.Remove(impact);
		}
	}

	private bool TryParseDecimalOrPercentage(ICharacter actor, string text, out decimal value)
	{
		if (decimal.TryParse(text, NumberStyles.Number, actor.Account.Culture, out value))
		{
			return true;
		}

		return text.TryParsePercentageDecimal(actor.Account.Culture, out value);
	}

	private readonly List<(long PopulationId, decimal AdditiveIncomeImpact, decimal MultiplicativeIncomeImpact)>
		_unresolvedPopulationIncomeImpacts = [];
}

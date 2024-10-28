using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using JetBrains.Annotations;
using MoreLinq.Extensions;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.Economy.Markets;

public class MarketInfluence : SaveableItem, IMarketInfluence
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "MarketInfluence";

	public MarketInfluence(IMarket market, IMarketInfluenceTemplate template, string name, MudDateTime appliesFrom,
		[CanBeNull] MudDateTime appliesUntil)
	{
		Gameworld = market.Gameworld;
		Market = market;
		MarketInfluenceTemplate = template;
		_name = name;
		Description = template.Description;
		AppliesFrom = appliesFrom;
		_appliesUntil = appliesUntil;
		foreach (var impact in template.MarketImpacts)
		{
			_marketImpacts.Add(impact with {});
		}
		CharacterKnowsAboutInfluenceProg = template.CharacterKnowsAboutInfluenceProg;
		using (new FMDB())
		{
			var dbitem = new Models.MarketInfluence
			{
				Name = Name,
				Description = Description,
				MarketId = Market.Id,
				MarketInfluenceTemplateId = MarketInfluenceTemplate?.Id,
				AppliesFrom = AppliesFrom.GetDateTimeString(),
				AppliesUntil = AppliesUntil?.GetDateTimeString(),
				CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id,
				Impacts = SaveImpacts().ToString()
			};
			FMDB.Context.MarketInfluences.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public MarketInfluence(IMarket market, string name, string description,
		MudDateTime appliesFrom,
		[CanBeNull] MudDateTime appliesUntil)
	{
		Gameworld = market.Gameworld;
		Market = market;
		_name = name;
		Description = description;
		AppliesFrom = appliesFrom;
		_appliesUntil = appliesUntil;
		CharacterKnowsAboutInfluenceProg = Gameworld.AlwaysFalseProg;
		using (new FMDB())
		{
			var dbitem = new Models.MarketInfluence
			{
				Name = Name,
				Description = Description,
				MarketId = Market.Id,
				MarketInfluenceTemplateId = null,
				AppliesFrom = AppliesFrom.GetDateTimeString(),
				AppliesUntil = AppliesUntil?.GetDateTimeString(),
				CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id,
				Impacts = SaveImpacts().ToString()
			};
			FMDB.Context.MarketInfluences.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public IMarketInfluence Clone(string newName)
	{
		return new MarketInfluence(this, newName);
	}

	private MarketInfluence(MarketInfluence rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		Market = rhs.Market;
		MarketInfluenceTemplate = rhs.MarketInfluenceTemplate;
		_name = newName;
		Description = rhs.Description;
		AppliesFrom = rhs.AppliesFrom;
		_appliesUntil = rhs.AppliesUntil;
		foreach (var impact in rhs.MarketImpacts)
		{
			_marketImpacts.Add(impact with { });
		}
		CharacterKnowsAboutInfluenceProg = rhs.CharacterKnowsAboutInfluenceProg;
		using (new FMDB())
		{
			var dbitem = new Models.MarketInfluence
			{
				Name = Name,
				Description = Description,
				MarketId = Market.Id,
				MarketInfluenceTemplateId = MarketInfluenceTemplate?.Id,
				AppliesFrom = AppliesFrom.GetDateTimeString(),
				AppliesUntil = AppliesUntil?.GetDateTimeString(),
				CharacterKnowsAboutInfluenceProgId = CharacterKnowsAboutInfluenceProg.Id,
				Impacts = SaveImpacts().ToString()
			};
			FMDB.Context.MarketInfluences.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public MarketInfluence(IMarket market, Models.MarketInfluence influence)
	{
		Gameworld=market.Gameworld;
		_id = influence.Id;
		Market = market;
		MarketInfluenceTemplate = Gameworld.MarketInfluenceTemplates.Get(influence.MarketInfluenceTemplateId ?? 0);
		Description = influence.Description;
		_name = influence.Name;
		AppliesFrom = new MudDateTime(influence.AppliesFrom, Gameworld);
		_appliesUntil = influence.AppliesUntil is not null ?
			new MudDateTime(influence.AppliesUntil, Gameworld) :
			null;
		CharacterKnowsAboutInfluenceProg = Gameworld.FutureProgs.Get(influence.CharacterKnowsAboutInfluenceProgId);
		foreach (var impact in XElement.Parse(influence.Impacts).Elements("Impact"))
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
	public override void Save()
	{
		var dbitem = FMDB.Context.MarketInfluences.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.AppliesFrom = AppliesFrom.GetDateTimeString();
		dbitem.AppliesUntil = AppliesUntil?.GetDateTimeString();
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

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - sets a new name
	#3desc#0 - drops you into an editor to write a description for players
	#3know <prog>#0 - sets the prog that controls if players know about this
	#3impact <category> <supply%> <demand%>#0 - adds or edits an impact for a category
	#3remimpact <category>#0 - removes the impact for a category
	#3applies <date>#0 - the date that this impact applies from
	#3until <date>#0 - the date that this impact applies until
	#3until always#0 - removes the expiry date for this impact
	#3duration <timespan>#0 - an alternative way to set until based on duration";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor);
			case "applies":
				return BuildingCommandApplies(actor, command);
			case "until":
				return BuildingCommandUntil(actor, command);
			case "duration":
				return BuildingCommandDuration(actor, command);
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
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandDuration(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the duration of this market influence's impact?");
			return false;
		}

		if (!TimeSpan.TryParse(command.SafeRemainingArgument, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid timespan.");
			return false;
		}

		AppliesUntil = AppliesFrom + value;
		actor.OutputHandler.Send($"This influence now applies for {value.DescribePrecise(actor).ColourValue()}, until {AppliesUntil.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}.");
		Changed = true;
		return true;
	}

	private bool BuildingCommandUntil(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What date should this influence apply from?");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("never", "always"))
		{
			AppliesUntil = null;
			Changed = true;
			actor.OutputHandler.Send("This influence will now apply until manually removed.");
			return true;
		}

		if (!MudDateTime.TryParse(command.SafeRemainingArgument, Market.EconomicZone.FinancialPeriodReferenceCalendar, Market.EconomicZone.FinancialPeriodReferenceClock, out var date))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid date and time.{MudDateTime.TryParseHelpText(actor, Market.EconomicZone)}");
			return false;
		}

		if (date <= AppliesFrom)
		{
			actor.OutputHandler.Send("Applies Until cannot be before the Applies From.");
			return false;
		}

		AppliesUntil = date;
		Changed = true;
		actor.OutputHandler.Send($"This influence now applies until {date.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandApplies(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What date should this influence apply from?");
			return false;
		}

		if (!MudDateTime.TryParse(command.SafeRemainingArgument, Market.EconomicZone.FinancialPeriodReferenceCalendar, Market.EconomicZone.FinancialPeriodReferenceClock, out var date))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid date and time.{MudDateTime.TryParseHelpText(actor, Market.EconomicZone)}");
			return false;
		}

		if (AppliesUntil is not null && AppliesUntil <= date)
		{
			actor.OutputHandler.Send("Applies Until cannot be before the Applies From.");
			return false;
		}

		AppliesFrom = date;
		Changed = true;
		actor.OutputHandler.Send($"This influence now applies from {date.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}.");
		return true;
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

		_marketImpacts.RemoveAll(x => x.MarketCategory == category);
		_marketImpacts.Add(new MarketImpact
		{
			MarketCategory = category,
			SupplyImpact = supply,
			DemandImpact = demand
		});
		actor.OutputHandler.Send($"You set the impact for the {category.Name.ColourValue()} market category to {supply.ToBonusPercentageString(actor)} supply and {demand.ToBonusPercentageString(actor)} demand.");
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
		actor.OutputHandler.Send($"You remove all impacts associated with the {category.Name.ColourValue()} market category.");
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
			$"This influence will now use the {prog.MXPClickableFunctionName()} prog to control whether players know about them.");
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
		handler.Send($"You set the description for this influence to:\n\n{Description.Wrap((int)args[0], "\t")}");
	}

	private void DescriptionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the description.");
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this market influence?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();

		actor.OutputHandler.Send(
			$"You rename this market influence from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}


	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Market Influence #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine($"Market: {Market.Name.ColourValue()} (#{Market.Id.ToString("N0", actor).ColourValue()})");
		sb.AppendLine($"Template: {MarketInfluenceTemplate?.Name.ColourValue() ?? "None".ColourError()}");
		sb.AppendLine($"Character Knows Prog: {CharacterKnowsAboutInfluenceProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Applies From: {AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}");
		sb.AppendLine($"Applies Until: {AppliesUntil?.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue() ?? "Forever".Colour(Telnet.Magenta)}");
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
	public IMarket Market { get; set; }

	/// <inheritdoc />
	public IMarketInfluenceTemplate MarketInfluenceTemplate { get; set; }

	/// <inheritdoc />
	public string Description { get; set; }

	/// <inheritdoc />
	public MudDateTime AppliesFrom { get; set; }

	private MudDateTime _appliesUntil;

	/// <inheritdoc />
	public MudDateTime AppliesUntil
	{
		get => _appliesUntil;
		set
		{
			_appliesUntil = value;
			Changed = true;
		}
	}

	public void EndOrCancel()
	{
		var now = Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		if (AppliesFrom > now)
		{
			Delete();
			return;
		}

		AppliesUntil = now;
	}

	public void Delete()
	{
		Market.RemoveMarketInfluence(this);
		Gameworld.Destroy(this);
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.MarketInfluences.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.MarketInfluences.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	private readonly List<MarketImpact> _marketImpacts = new();

	/// <inheritdoc />
	public IEnumerable<MarketImpact> MarketImpacts => _marketImpacts;

	public IFutureProg CharacterKnowsAboutInfluenceProg { get; set; }

	/// <inheritdoc />
	public bool CharacterKnowsAboutInfluence(ICharacter character)
	{
		return CharacterKnowsAboutInfluenceProg?.ExecuteBool(character) != false;
	}

	/// <inheritdoc />
	public bool Applies([CanBeNull] IMarketCategory category, MudDateTime currentDateTime)
	{
		return AppliesFrom <= currentDateTime &&
		       (AppliesUntil is null || AppliesUntil >= currentDateTime) &&
		       (category is null || _marketImpacts.Any(x => x.MarketCategory == category));
	}

	/// <inheritdoc />
	public string TextForMarketShow(ICharacter actor)
	{
		var now = Market.EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		
		if (AppliesUntil is null)
		{
			if (AppliesFrom <= now)
			{
				return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Active Until Revoked";
			}

			return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Begins {AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()} Until Revoked";
		}

		if (AppliesFrom <= now)
		{
			return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Active Until {AppliesUntil.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}";
		}

		return $"\t#{Id.ToString("N0", actor)}) {Name.ColourName()} - Impacts {_marketImpacts.Count.ToString("N0", actor).ColourValue()} {"category".Pluralise(_marketImpacts.Count != 1)} - Begins {AppliesFrom.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()} Until {AppliesUntil.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Immortal).ColourValue()}";
	}
}
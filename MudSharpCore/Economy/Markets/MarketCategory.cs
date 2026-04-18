using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions.DateTime;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MudSharp.Economy.Markets;

public class MarketCategory : SaveableItem, IMarketCategory
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "MarketCategory";

	public IMarketCategory Clone(string newName)
	{
		return new MarketCategory(this, newName);
	}

	private MarketCategory(MarketCategory rhs, string newName)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		Description = rhs.Description;
		ElasticityFactorBelow = rhs.ElasticityFactorBelow;
		ElasticityFactorAbove = rhs.ElasticityFactorAbove;
		CategoryType = rhs.CategoryType;
		_tags.AddRange(rhs._tags);
		_combinationComponents.AddRange(rhs._combinationComponents.Select(x => x with { }));
		using (new FMDB())
		{
			Models.MarketCategory dbitem = new()
			{
				Name = Name,
				Description = Description,
				ElasticityFactorBelow = ElasticityFactorBelow,
				ElasticityFactorAbove = ElasticityFactorAbove,
				MarketCategoryType = (int)CategoryType,
				Tags = SaveTags().ToString(),
				CombinationCategories = SaveComponents().ToString()
			};
			FMDB.Context.MarketCategories.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public MarketCategory(IFuturemud gameworld, string name, ITag tag)
	{
		Gameworld = gameworld;
		_name = name;
		Description = $"Items of type {tag.Name}";
		_tags.Add(tag);
		ElasticityFactorAbove = 0.75;
		ElasticityFactorBelow = 0.75;
		CategoryType = MarketCategoryType.Standalone;
		using (new FMDB())
		{
			Models.MarketCategory dbitem = new()
			{
				Name = Name,
				Description = Description,
				ElasticityFactorBelow = ElasticityFactorBelow,
				ElasticityFactorAbove = ElasticityFactorAbove,
				MarketCategoryType = (int)CategoryType,
				Tags = SaveTags().ToString(),
				CombinationCategories = SaveComponents().ToString()
			};
			FMDB.Context.MarketCategories.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public MarketCategory(IFuturemud gameworld, Models.MarketCategory category)
	{
		Gameworld = gameworld;
		_id = category.Id;
		_name = category.Name;
		Description = category.Description;
		ElasticityFactorBelow = category.ElasticityFactorBelow;
		ElasticityFactorAbove = category.ElasticityFactorAbove;
		CategoryType = Enum.IsDefined(typeof(MarketCategoryType), category.MarketCategoryType)
			? (MarketCategoryType)category.MarketCategoryType
			: MarketCategoryType.Standalone;
		foreach (XElement tag in XElement.Parse(category.Tags).Elements("Tag"))
		{
			ITag gtag = Gameworld.Tags.Get(long.Parse(tag.Value));
			if (gtag is null)
			{
				continue;
			}

			_tags.Add(gtag);
		}

		LoadComponents(category.CombinationCategories);
	}

	/// <inheritdoc />
	public override void Save()
	{
		Models.MarketCategory dbitem = FMDB.Context.MarketCategories.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.ElasticityFactorBelow = ElasticityFactorBelow;
		dbitem.ElasticityFactorAbove = ElasticityFactorAbove;
		dbitem.MarketCategoryType = (int)CategoryType;
		dbitem.Tags = SaveTags().ToString();
		dbitem.CombinationCategories = SaveComponents().ToString();
		Changed = false;
	}

	private XElement SaveTags()
	{
		return new XElement("Tags",
			from tag in _tags
			select new XElement("Tag", tag.Id)
		);
	}

	private XElement SaveComponents()
	{
		return new XElement("Components",
			from component in _combinationComponents
			select new XElement("Component",
				new XAttribute("category", component.MarketCategory.Id),
				new XAttribute("weight", component.Weight.ToString(System.Globalization.CultureInfo.InvariantCulture))
			)
		);
	}

	private void LoadComponents(string? xml)
	{
		if (string.IsNullOrWhiteSpace(xml))
		{
			return;
		}

		foreach (XElement item in XElement.Parse(xml).Elements("Component"))
		{
			if (!long.TryParse(item.Attribute("category")?.Value, out var categoryId))
			{
				continue;
			}

			if (!decimal.TryParse(item.Attribute("weight")?.Value, NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture,
				    out var weight) ||
			    weight <= 0.0m)
			{
				continue;
			}

			_unresolvedCombinationComponents.Add((categoryId, weight));
		}
	}

	public void ResolveCombinationComponents()
	{
		if (!_unresolvedCombinationComponents.Any())
		{
			return;
		}

		foreach (var component in _unresolvedCombinationComponents.ToList())
		{
			var category = Gameworld.MarketCategories.Get(component.CategoryId);
			if (category is null)
			{
				ConsoleUtilities.WriteLine(
					$"#9Skipping unresolved combination component {component.CategoryId} for market category {Name}.#0");
				continue;
			}

			if (category == this || WouldCreateCycle(category))
			{
				ConsoleUtilities.WriteLine(
					$"#9Skipping cyclic combination component {category.Name} for market category {Name}.#0");
				continue;
			}

			_combinationComponents.Add(new MarketCategoryComponent
			{
				MarketCategory = category,
				Weight = component.Weight
			});
			_unresolvedCombinationComponents.Remove(component);
		}
	}

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - changes the name
	#3eover <%>#0 - changes the elasticity for oversupply
	#3eunder <%>#0 - changes the elasticity for undersupply
	#3type <standalone|combination>#0 - sets the category type
	#3component <category> <weight>#0 - adds or updates a combination component
	#3remcomponent <category>#0 - removes a combination component
	#3desc#0 - drops you into an editor to set the description
	#3tag <tag>#0 - toggles an item tag as being a part of this category";

	/// <inheritdoc />
	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "elasticityover":
			case "eover":
			case "overe":
				return BuildingCommandElasticityOversupply(actor, command);
			case "elasticityunder":
			case "eunder":
			case "undere":
				return BuildingCommandElasticityUndersupply(actor, command);
			case "type":
				return BuildingCommandType(actor, command);
			case "component":
			case "addcomponent":
				return BuildingCommandComponent(actor, command);
			case "remcomponent":
			case "removecomponent":
			case "deletecomponent":
				return BuildingCommandRemoveComponent(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor);
		}

		actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
		return false;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag do you want to toggle as belonging to this category?");
			return false;
		}

		ITag tag = Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
		if (tag is null)
		{
			actor.OutputHandler.Send("That is not a valid tag.");
			return false;
		}

		Changed = true;
		if (_tags.Contains(tag))
		{
			_tags.Remove(tag);
			actor.OutputHandler.Send(
				$"This market category will no longer include items with the {tag.FullName.ColourName()} tag.");
			InvalidateMarketCaches();
			return true;
		}

		_tags.Add(tag);
		actor.OutputHandler.Send(
			$"This market category will now include items with the {tag.FullName.ColourName()} tag.");
		InvalidateMarketCaches();
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Should this market category be standalone or combination?");
			return false;
		}

		switch (command.SafeRemainingArgument.ToLowerInvariant())
		{
			case "standalone":
			case "single":
				CategoryType = MarketCategoryType.Standalone;
				Changed = true;
				InvalidateMarketCaches();
				actor.OutputHandler.Send("This market category is now a standalone category.");
				return true;
			case "combination":
			case "combo":
			case "aggregate":
				CategoryType = MarketCategoryType.Combination;
				Changed = true;
				InvalidateMarketCaches();
				actor.OutputHandler.Send(
					"This market category is now a combination category and will price itself from its components.");
				return true;
			default:
				actor.OutputHandler.Send("You must specify either #3standalone#0 or #3combination#0."
					.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandComponent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which market category should be a component of this combination category?");
			return false;
		}

		var category = Gameworld.MarketCategories.GetByIdOrName(command.PopSpeech());
		if (category is null)
		{
			actor.OutputHandler.Send("There is no such market category.");
			return false;
		}

		if (category == this)
		{
			actor.OutputHandler.Send("A market category cannot include itself as a component.");
			return false;
		}

		if (WouldCreateCycle(category))
		{
			actor.OutputHandler.Send(
				"That component would create a circular dependency in the market category combination graph.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What weighting should this component contribute?");
			return false;
		}

		if (!TryParseWeight(actor, command.SafeRemainingArgument, out var weight) || weight <= 0.0m)
		{
			actor.OutputHandler.Send(
				"You must enter a positive decimal or percentage weighting for that combination component.");
			return false;
		}

		_combinationComponents.RemoveAll(x => x.MarketCategory == category);
		_combinationComponents.Add(new MarketCategoryComponent
		{
			MarketCategory = category,
			Weight = weight
		});
		Changed = true;
		InvalidateMarketCaches();
		actor.OutputHandler.Send(
			$"This market category now includes {category.Name.ColourName()} with a weighting of {weight.ToString("N3", actor).ColourValue()}.");
		return true;
	}

	private bool BuildingCommandRemoveComponent(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which component do you want to remove from this market category?");
			return false;
		}

		var category = Gameworld.MarketCategories.GetByIdOrName(command.SafeRemainingArgument);
		if (category is null)
		{
			actor.OutputHandler.Send("There is no such market category.");
			return false;
		}

		if (_combinationComponents.All(x => x.MarketCategory != category))
		{
			actor.OutputHandler.Send("That market category is not currently a component of this category.");
			return false;
		}

		_combinationComponents.RemoveAll(x => x.MarketCategory == category);
		Changed = true;
		InvalidateMarketCaches();
		actor.OutputHandler.Send(
			$"This market category no longer includes {category.Name.ColourName()} as a component.");
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
		InvalidateMarketCaches();
		handler.Send(
			$"You set the description for this market category to:\n\n{Description.Wrap((int)args[0], "\t")}");
	}

	private void DescriptionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to edit the description.");
	}

	private bool BuildingCommandElasticityUndersupply(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What elasticity do you want to use for undersupply of this category?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		ElasticityFactorBelow = value;
		Changed = true;
		InvalidateMarketCaches();
		actor.OutputHandler.Send(
			$"This market category will use an elasticity of {value.ToString("P2", actor).ColourValue()} for undersupply.");
		return true;
	}

	private bool BuildingCommandElasticityOversupply(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What elasticity do you want to use for oversupply of this category?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out double value))
		{
			actor.OutputHandler.Send(
				$"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		ElasticityFactorAbove = value;
		Changed = true;
		InvalidateMarketCaches();
		actor.OutputHandler.Send(
			$"This market category will use an elasticity of {value.ToString("P2", actor).ColourValue()} for oversupply.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this market category?");
			return false;
		}

		string name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MarketCategories.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"There is already a market category called {name.ColourName()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename this market category from {Name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}

	/// <inheritdoc />
	public string Show(ICharacter actor)
	{
		StringBuilder sb = new();
		sb.AppendLine(
			$"Market Category #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow,
				Telnet.BoldWhite));
		sb.AppendLine($"Type: {CategoryType.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Elasticity (Oversupply): {ElasticityFactorBelow.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Elasticity (Undersupply): {ElasticityFactorAbove.ToString("N3", actor).ColourValue()}");
		if (_combinationComponents.Any())
		{
			var total = _combinationComponents.Sum(x => x.Weight);
			sb.AppendLine();
			sb.AppendLine("Combination Components:");
			sb.AppendLine();
			foreach (var component in _combinationComponents.OrderBy(x => x.MarketCategory.Name))
			{
				var proportion = total > 0.0m ? component.Weight / total : 0.0m;
				sb.AppendLine(
					$"\t{component.MarketCategory.Name.ColourName()}: Weight {component.Weight.ToString("N3", actor).ColourValue()}, Normalized {proportion.ToString("P2", actor).ColourValue()}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Tags:");
		sb.AppendLine();
		foreach (ITag tag in _tags)
		{
			sb.AppendLine($"\t{tag.FullName.ColourName()}");
		}

		return sb.ToString();
	}

	/// <inheritdoc />
	public string Description { get; set; }

	/// <inheritdoc />
	public MarketCategoryType CategoryType { get; private set; }

	/// <inheritdoc />
	public IEnumerable<MarketCategoryComponent> CombinationComponents => _combinationComponents;

	/// <inheritdoc />
	public bool BelongsToCategory(IGameItem item)
	{
		return _tags.Any(x => item.Tags.Any(y => y.IsA(x)));
	}

	/// <inheritdoc />
	public bool BelongsToCategory(IGameItemProto proto)
	{
		return _tags.Any(x => proto.Tags.Any(y => y.IsA(x)));
	}

	/// <inheritdoc />
	public double ElasticityFactorAbove { get; set; }

	/// <inheritdoc />
	public double ElasticityFactorBelow { get; set; }

	private readonly List<ITag> _tags = [];
	private readonly List<MarketCategoryComponent> _combinationComponents = [];
	private readonly List<(long CategoryId, decimal Weight)> _unresolvedCombinationComponents = [];

	private void InvalidateMarketCaches()
	{
		foreach (var market in Gameworld.Markets.OfType<Market>())
		{
			market.InvalidatePricingCache();
		}
	}

	private bool WouldCreateCycle(IMarketCategory component)
	{
		return HasPathTo(component, Id, []);
	}

	private static bool HasPathTo(IMarketCategory current, long targetId, HashSet<long> visited)
	{
		if (!visited.Add(current.Id))
		{
			return false;
		}

		if (current.Id == targetId)
		{
			return true;
		}

		if (current.CategoryType != MarketCategoryType.Combination)
		{
			return false;
		}

		return current.CombinationComponents.Any(x => HasPathTo(x.MarketCategory, targetId, visited));
	}

	private bool TryParseWeight(ICharacter actor, string text, out decimal value)
	{
		if (decimal.TryParse(text, NumberStyles.Number, actor.Account.Culture, out value))
		{
			return true;
		}

		return text.TryParsePercentageDecimal(actor.Account.Culture, out value);
	}

	#region FutureProgs

	/// <inheritdoc />
	public ProgVariableTypes Type => ProgVariableTypes.MarketCategory;

	/// <inheritdoc />
	public object GetObject => this;

	/// <inheritdoc />
	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
		}

		throw new ArgumentOutOfRangeException(nameof(property));
	}

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", ProgVariableTypes.Text },
			{ "id", ProgVariableTypes.Number },
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", "The name of the market category" },
			{ "id", "The Id of the market category" },
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.MarketCategory, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion
}

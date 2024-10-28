using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
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
		_tags.AddRange(rhs._tags);
		using (new FMDB())
		{
			var dbitem = new Models.MarketCategory
			{
				Name = Name,
				Description = Description,
				ElasticityFactorBelow = ElasticityFactorBelow,
				ElasticityFactorAbove = ElasticityFactorAbove,
				Tags = SaveTags().ToString()
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
		using (new FMDB())
		{
			var dbitem = new Models.MarketCategory
			{
				Name = Name,
				Description = Description,
				ElasticityFactorBelow = ElasticityFactorBelow,
				ElasticityFactorAbove = ElasticityFactorAbove,
				Tags = SaveTags().ToString()
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
		foreach (var tag in XElement.Parse(category.Tags).Elements("Tag"))
		{
			var gtag = Gameworld.Tags.Get(long.Parse(tag.Value));
			if (gtag is null)
			{
				continue;
			}

			_tags.Add(gtag);
		}
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.MarketCategories.Find(Id);
		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.ElasticityFactorBelow = ElasticityFactorBelow;
		dbitem.ElasticityFactorAbove = ElasticityFactorAbove;
		dbitem.Tags = SaveTags().ToString();
		Changed = false;
	}

	private XElement SaveTags()
	{
		return new XElement("Tags",
			from tag in _tags select new XElement("Tag", tag.Id)
		);
	}

	public const string HelpText = @"You can use the following options with this command:

	#3name <name>#0 - changes the name
	#3eover <%>#0 - changes the elasticity for oversupply
	#3eunder <%>#0 - changes the elasticity for undersupply
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

		var tag = Gameworld.Tags.GetByIdOrName(command.SafeRemainingArgument);
		if (tag is null)
		{
			actor.OutputHandler.Send("That is not a valid tag.");
			return false;
		}

		Changed = true;
		if (_tags.Contains(tag))
		{
			_tags.Remove(tag);
			actor.OutputHandler.Send($"This market category will no longer include items with the {tag.FullName.ColourName()} tag.");
			return true;
		}

		_tags.Add(tag);
		actor.OutputHandler.Send($"This market category will now include items with the {tag.FullName.ColourName()} tag.");
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
		handler.Send($"You set the description for this market category to:\n\n{Description.Wrap((int)args[0], "\t")}");
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

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		ElasticityFactorBelow = value;
		Changed = true;
		actor.OutputHandler.Send($"This market category will use an elasticity of {value.ToString("P2", actor).ColourValue()} for undersupply.");
		return true;
	}

	private bool BuildingCommandElasticityOversupply(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What elasticity do you want to use for oversupply of this category?");
			return false;
		}

		if (!command.SafeRemainingArgument.TryParsePercentage(actor.Account.Culture, out var value))
		{
			actor.OutputHandler.Send($"The text {command.SafeRemainingArgument.ColourCommand()} is not a valid percentage.");
			return false;
		}

		ElasticityFactorAbove = value;
		Changed = true;
		actor.OutputHandler.Send($"This market category will use an elasticity of {value.ToString("P2", actor).ColourValue()} for oversupply.");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What new name do you want to give to this market category?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (Gameworld.MarketCategories.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a market category called {name.ColourName()}. Names must be unique.");
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
		var sb = new StringBuilder();
		sb.AppendLine($"Market Category #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine($"Elasticity (Oversupply): {ElasticityFactorBelow.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Elasticity (Undersupply): {ElasticityFactorAbove.ToString("N3", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t").ColourCommand());
		sb.AppendLine();
		sb.AppendLine("Tags:");
		sb.AppendLine();
		foreach (var tag in _tags)
		{
			sb.AppendLine($"\t{tag.FullName.ColourName()}");
		}
		return sb.ToString();
	}

	/// <inheritdoc />
	public string Description { get; set; }

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

	public readonly List<ITag> _tags = new();

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
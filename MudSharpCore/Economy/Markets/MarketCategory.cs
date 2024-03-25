using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg.Functions.DateTime;
using MudSharp.GameItems;

namespace MudSharp.Economy.Markets;

public class MarketCategory : SaveableItem, IMarketCategory
{
	/// <inheritdoc />
	public sealed override string FrameworkItemType => "MarketCategory";

	public MarketCategory(IFuturemud gameworld, string name, ITag tag)
	{
		Gameworld = gameworld;
		_name = name;
		Description = $"Items of type {tag.Name}";
		_tags.Add(tag);
		ElasticityFactorAbove = 0.75;
		ElasticityFactorBelow = 0.75;
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
		throw new System.NotImplementedException();
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
		sb.AppendLine($"Market Category #{Id.ToString("N0", actor)} - {Name}".GetLineWithTitle(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine($"Elasticity (Oversupply): {ElasticityFactorBelow.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Elasticity (Undersupply): {ElasticityFactorAbove.ToString("N3", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
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
}
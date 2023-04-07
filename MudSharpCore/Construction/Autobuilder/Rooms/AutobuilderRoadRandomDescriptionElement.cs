using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Rooms;

public class AutobuilderRoadRandomDescriptionElement : AutobuilderRandomDescriptionElement
{
	public static Regex ReplacementRegex = new("\\$(?<dash>dash)?(?<the>the)?directions");

	public Regex TagRegex { get; private set; }

	public AutobuilderRoadRandomDescriptionElement(IFuturemud gameworld) : base(gameworld)
	{
		TagRegex = new Regex("InvalidRegexRoadTags");
	}

	public AutobuilderRoadRandomDescriptionElement(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		if (Tags.Any())
		{
			TagRegex = new Regex($"{Tags.First()}=(?<directions>.+)");
		}
		else
		{
			TagRegex = new Regex("InvalidRegexRoadTags");
		}
	}

	public override IAutobuilderRandomDescriptionElement Clone()
	{
		return new AutobuilderRoadRandomDescriptionElement(Gameworld);
	}

	#region Overrides of AutobuilderRandomDescriptionElement

	protected override bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		var @base = base.BuildingCommandTag(actor, command);
		if (Tags.Any())
		{
			TagRegex = new Regex($"{Tags.First()}=(?<directions>.+)");
		}
		else
		{
			TagRegex = new Regex("InvalidRegexRoadTags");
		}

		return @base;
	}

	protected override bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		var @base = base.BuildingCommandName(actor, command);
		if (!@base)
		{
			actor.OutputHandler.Send(@"You can also use the following road-tag related options:

    $directions - a list of directions, e.g. ""North, West and South""
    $thedirections - a list of directions with articles, e.g. ""the North, the West and the South""
    $dashdirections - a list of directions separated by dashes, e.g. ""North-South""");
		}

		return @base;
	}

	protected override bool BuildingCommandText(ICharacter actor, StringStack command)
	{
		var @base = base.BuildingCommandText(actor, command);
		if (!@base)
		{
			actor.OutputHandler.Send(@"You can also use the following road-tag related options:

    $directions - a list of directions, e.g. ""North, West and South""
    $thedirections - a list of directions with articles, e.g. ""the North, the West and the South""
    $dashdirections - a list of directions separated by dashes, e.g. ""North-South""");
		}

		return @base;
	}

	public override string Show(ICharacter actor)
	{
		if (!Tags.Any())
		{
			return
				$"{base.Show(actor)}\n\n{"Note: This element type must always have at least one tag".Colour(Telnet.Red)}";
		}

		return
			$"{base.Show(actor)}\n\n{$"Note: Tries to match tags in the form {Tags.First()}=directions, e.g. {Tags.First()}=s,sw".ColourCommand()}";
	}

	#endregion

	public override XElement SaveToXml()
	{
		return new XElement("Description",
			new XAttribute("type", "road"),
			new XAttribute("mandatory", MandatoryIfValid),
			new XAttribute("fixedposition", MandatoryPosition),
			new XElement("Text", new XCData(Text)),
			new XElement("RoomNameText", new XCData(RoomNameText)),
			new XElement("Weight", Weight),
			new XElement("Tags", Tags.ListToCommaSeparatedValues()),
			new XElement("Terrains",
				from terrain in Terrains
				select new XElement("Terrain", terrain.Id)
			)
		);
	}

	public override bool Applies(ITerrain terrain, IEnumerable<string> tags)
	{
		return (!Terrains.Any() ||
		        Terrains.Contains(terrain)) && tags.Any(x => TagRegex.IsMatch(x)) &&
		       Tags.Skip(1).All(x => tags.Contains(x));
	}

	public override (string RoomName, string DescriptionText) TextForTags(ITerrain terrain, IEnumerable<string> tags)
	{
		var matchingTag = tags.FirstOrDefault(x => TagRegex.IsMatch(x));
		if (matchingTag == null)
		{
			return (RoomNameText, Text);
		}

		var directions = TagRegex.Match(matchingTag).Groups["directions"].Value.Split(',')
		                         .Select(x => Constants.CardinalDirectionStringToDirection[x]).ToList();

		string Evaluator(Match match)
		{
			if (match.Groups["dash"].Length > 0)
			{
				return directions.Select(x => x.Describe()).ListToCommaSeparatedValues("-");
			}

			if (match.Groups["the"].Length > 0)
			{
				return directions.Select(x => x.Describe()).ListToString("the ");
			}

			return directions.Select(x => x.Describe()).ListToString();
		}

		return (ReplacementRegex.Replace(RoomNameText, Evaluator), ReplacementRegex.Replace(Text, Evaluator));
	}
}
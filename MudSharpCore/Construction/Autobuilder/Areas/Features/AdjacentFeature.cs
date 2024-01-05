using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas.Features;

public class AdjacentFeature : Feature
{
	public bool IgnoreExits { get; private set; }
	public string AdjacentFeatureTag { get; private set; }

	public Dictionary<CardinalDirection, string> DirectionAdjacencyTags { get; } = new();

	public AdjacentFeature(IFuturemud gameworld, string name) : base(gameworld, name)
	{
		IgnoreExits = false;
		AdjacentFeatureTag = $"{name} adjacent";
	}

	public AdjacentFeature(XElement root, IFuturemud gameworld) : base(root, gameworld)
	{
		IgnoreExits = bool.Parse(root.Element("IgnoreExits")?.Value ?? "true");
		AdjacentFeatureTag = root.Element("AdjacentFeatureTag")?.Value ?? string.Empty;
		foreach (var element in root.Element("Adjacents")?.Elements() ?? Enumerable.Empty<XElement>())
		{
			var direction = (CardinalDirection)int.Parse(element.Attribute("direction")?.Value ??
			                                             throw new ApplicationException(
				                                             "There was no direction attribute for the Adjacent element for the AdjacentFeature."));
			DirectionAdjacencyTags[direction] = element.Value;
		}
	}

	public override string StringForParentShow(ICharacter builder)
	{
		return
			$"{(MinimumCount == MaximumCount ? MinimumCount.ToString("N0", builder) : $"{MinimumCount.ToString("N0", builder)}-{MaximumCount.ToString("N0", builder)}")} {Name.ColourName()} / {AdjacentFeatureTag.ColourName()} (adj) - {Terrains.Select(x => x.Name.ColourValue()).DefaultIfEmpty("All".Colour(Telnet.BoldGreen)).ListToString(conjunction: "or ")}";
	}

	public override XElement SaveToXml()
	{
		var @base = base.SaveToXml();
		@base.Add(new XElement("IgnoreExits", IgnoreExits));
		@base.Add(new XElement("AdjacentFeatureTag", new XCData(AdjacentFeatureTag)));
		var adjacent = new XElement("Adjacents");
		foreach (var item in DirectionAdjacencyTags)
		{
			adjacent.Add(new XElement("Adjacent", new XAttribute("direction", (int)item.Key), new XCData(item.Value)));
		}

		@base.Add(adjacent);
		return @base;
	}

	public override void ApplyFeature(ICell[,] cellMap, List<string>[,] features, int x, int y)
	{
		base.ApplyFeature(cellMap, features, x, y);
		if (IgnoreExits)
		{
			cellMap.ApplyActionToAdjacentsWithInfo(x, y, (cell, direction, xcoord, ycoord) =>
			{
				if (!string.IsNullOrEmpty(AdjacentFeatureTag))
				{
					features[xcoord, ycoord].Add(AdjacentFeatureTag);
				}

				if (DirectionAdjacencyTags.ContainsKey(direction))
				{
					features[xcoord, ycoord].Add(DirectionAdjacencyTags[direction]);
				}
			});
		}
		else
		{
			cellMap.ApplyActionToAdjacentsWithInfo(x, y, (cell, direction, xcoord, ycoord) =>
			{
				if (cellMap[x, y].ExitsFor(null).All(item => item.Destination != cell))
				{
					return;
				}

				if (!string.IsNullOrEmpty(AdjacentFeatureTag))
				{
					features[xcoord, ycoord].Add(AdjacentFeatureTag);
				}

				if (DirectionAdjacencyTags.ContainsKey(direction))
				{
					features[xcoord, ycoord].Add(DirectionAdjacencyTags[direction]);
				}
			});
		}
	}

	public override string BuildingHelpText => $@"{base.BuildingHelpText}
	ignoreexits - toggles whether to consider exits or merely coordinates
	adjacenttag <tag> - the tag to apply to adjacent rooms";

	public override bool BuildingCommand(ICharacter actor, TerrainFeatureGroup parent, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "ignoreexits":
				return BuildingCommandIgnoreExits(actor, command);
			case "adjacenttag":
				return BuildingCommandAdjacentTag(actor, command);
		}

		var direction =
			Constants.CardinalDirectionStringToDirection.ContainsKey(command.Last.ToLowerInvariant()
			                                                                .CollapseString())
				? Constants.CardinalDirectionStringToDirection[command.Last.ToLowerInvariant().CollapseString()]
				: CardinalDirection.Unknown;
		if (direction != CardinalDirection.Unknown)
		{
			return BuildingCommandCardinalDirection(actor, command, direction);
		}

		return base.BuildingCommand(actor, parent, command.GetUndo());
	}

	private bool BuildingCommandCardinalDirection(ICharacter actor, StringStack command, CardinalDirection direction)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"What specific tag override do you want to apply for the {direction.Describe().ColourValue()} direction?");
			return false;
		}

		DirectionAdjacencyTags[direction] = command.SafeRemainingArgument.TitleCase();
		actor.OutputHandler.Send(
			$"The {direction.DescribeEnum().ColourValue()} direction override is now {DirectionAdjacencyTags[direction].ColourName()}.");
		return true;
	}

	private bool BuildingCommandAdjacentTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"What tag would you like to apply to those locations adjacent to this feature?");
			return false;
		}

		AdjacentFeatureTag = command.SafeRemainingArgument;
		actor.OutputHandler.Send(
			$"This feature will now apply the {AdjacentFeatureTag.ColourName()} feature to adjacent rooms.");
		var matchingTag =
			actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(AdjacentFeatureTag));
		if (matchingTag != null)
		{
			actor.OutputHandler.Send($"Note: This matches the framework tag {matchingTag.FullName.ColourName()}.");
		}

		foreach (var direction in Constants.CardinalDirections.Except(CardinalDirection.Unknown))
		{
			DirectionAdjacencyTags[direction] = $"{AdjacentFeatureTag} {direction.DescribeBrief()}";
		}

		return true;
	}

	private bool BuildingCommandIgnoreExits(ICharacter actor, StringStack command)
	{
		IgnoreExits = !IgnoreExits;
		actor.OutputHandler.Send(
			$"This adjacent tag will {(IgnoreExits ? "now" : "no longer")} ignore exits and rely purely on coordinate location when deciding whether to apply to an adjacent location.");
		return true;
	}
}
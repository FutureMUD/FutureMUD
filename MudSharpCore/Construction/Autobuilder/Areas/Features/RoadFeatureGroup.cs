using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas.Features;

public class RoadFeatureGroup : TerrainFeatureGroup
{
	public RoadFeatureGroup(XElement root, IFuturemud gameworld)
	{
		BaseFeature = root.Element("BaseFeature")?.Value ??
		              throw new ApplicationException("There was no BaseFeature element for the RoadFeatureGroup.");
		StraightRoadFeature = root.Element("StraightRoadFeature")?.Value ??
		                      throw new ApplicationException(
			                      "There was no StraightRoadFeature element for the RoadFeatureGroup.");
		CrossRoadsFeature = root.Element("CrossRoadsFeature")?.Value ??
		                    throw new ApplicationException(
			                    "There was no CrossRoadsFeature element for the RoadFeatureGroup.");
		TeeIntersectionFeature = root.Element("TeeIntersectionFeature")?.Value ??
		                         throw new ApplicationException(
			                         "There was no TeeIntersectionFeature element for the RoadFeatureGroup.");
		IsolatedRoadFeature = root.Element("IsolatedRoadFeature")?.Value ??
		                      throw new ApplicationException(
			                      "There was no IsolatedRoadFeature element for the RoadFeatureGroup.");
		BendInTheRoadFeature = root.Element("BendInTheRoadFeature")?.Value ??
		                       throw new ApplicationException(
			                       "There was no BendInTheRoadFeature element for the RoadFeatureGroup.");
		EndOfTheRoadFeature = root.Element("EndOfTheRoadFeature")?.Value ??
		                      throw new ApplicationException(
			                      "There was no EndOfTheRoadFeature element for the RoadFeatureGroup.");

		var element = root.Element("Terrains");
		if (element == null)
		{
			throw new ApplicationException("There was no Terrains element for the RoadFeatureGroup.");
		}

		foreach (var sub in element.Elements())
		{
			var target = sub?.Value ?? "0";
			var terrain = long.TryParse(target, out var value)
				? gameworld.Terrains.Get(value)
				: gameworld.Terrains.GetByName(target);
			if (terrain == null)
			{
				throw new ApplicationException($"There was no such terrain as {target} found for RoadFeaturesGroup");
			}

			Terrains.Add(terrain);
		}
	}

	public RoadFeatureGroup()
	{
		IsolatedRoadFeature = "isolated road";
		EndOfTheRoadFeature = "end of road";
		BendInTheRoadFeature = "bend in road";
		TeeIntersectionFeature = "tee intersection road";
		CrossRoadsFeature = "crossroad road";
	}

	public override XElement SaveToXml()
	{
		return new XElement("Group",
			new XAttribute("type", "road"),
			new XElement("BaseFeature", new XCData(BaseFeature)),
			new XElement("StraightRoadFeature", new XCData(StraightRoadFeature)),
			new XElement("CrossRoadsFeature", new XCData(CrossRoadsFeature)),
			new XElement("TeeIntersectionFeature", new XCData(TeeIntersectionFeature)),
			new XElement("IsolatedRoadFeature", new XCData(IsolatedRoadFeature)),
			new XElement("BendInTheRoadFeature", new XCData(BendInTheRoadFeature)),
			new XElement("EndOfTheRoadFeature", new XCData(EndOfTheRoadFeature)),
			new XElement("Terrains",
				from terrain in Terrains
				select new XElement("Terrain", terrain.Id)
			)
		);
	}

	public HashSet<ITerrain> Terrains { get; } = new();
	public string BaseFeature { get; set; }
	public string CrossRoadsFeature { get; set; }
	public string TeeIntersectionFeature { get; set; }
	public string IsolatedRoadFeature { get; set; }
	public string StraightRoadFeature { get; set; }
	public string BendInTheRoadFeature { get; set; }
	public string EndOfTheRoadFeature { get; set; }

	public override List<Feature> Features => new();

	public override void ApplyTerrainFeatures(ICell[,] cellMap, List<string>[,] featureMap)
	{
		var width = cellMap.GetLength(0);
		var height = cellMap.GetLength(1);

		for (var x = 0; x < width; x++)
		for (var y = 0; y < height; y++)
		{
			var cell = cellMap[x, y];
			var list = new List<CardinalDirection>();
			var count = cellMap.ApplyFunctionToAdjacentsReturnCountWithDirection(x, y, (adj, dir) =>
			{
				if (adj == null)
				{
					return false;
				}

				if (adj.ExitsFor(null).All(exit => exit.Destination != cell))
				{
					return false;
				}

				if (!Terrains.Contains(adj.Terrain(null)))
				{
					return false;
				}

				list.Add(dir);
				return true;
			});

			featureMap[x, y].Add(BaseFeature);

			switch (count)
			{
				case 0:
					featureMap[x, y].Add(IsolatedRoadFeature);
					continue;
				case 1:
					featureMap[x, y]
						.Add(
							$"{EndOfTheRoadFeature}={list.Select(str => str.DescribeBrief()).ListToCommaSeparatedValues()}");
					featureMap[x, y].Add(EndOfTheRoadFeature);
					continue;
				case 2:
					if (list[0].IsOpposingDirection(list[1]))
					{
						featureMap[x, y]
							.Add(
								$"{StraightRoadFeature}={list.Select(str => str.DescribeBrief()).ListToCommaSeparatedValues()}");
						featureMap[x, y].Add(StraightRoadFeature);
						continue;
					}

					featureMap[x, y]
						.Add(
							$"{BendInTheRoadFeature}={list.Select(str => str.DescribeBrief()).ListToCommaSeparatedValues()}");
					featureMap[x, y].Add(BendInTheRoadFeature);
					continue;
				case 3:
					featureMap[x, y]
						.Add(
							$"{TeeIntersectionFeature}={list.Select(str => str.DescribeBrief()).ListToCommaSeparatedValues()}");
					featureMap[x, y].Add(TeeIntersectionFeature);
					continue;
				default:
					featureMap[x, y]
						.Add(
							$"{CrossRoadsFeature}={list.Select(str => str.DescribeBrief()).ListToCommaSeparatedValues()}");
					featureMap[x, y].Add(CrossRoadsFeature);
					continue;
			}
		}
	}

	public override string Show(ICharacter builder)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Road Feature Group - {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(
			$"This feature group looks at the whole area and for each room finds all adjacent rooms also meeting this feature's criteria. It then applies various tags relating to the feature, how many adjacent features there are, and even adds a special feature type that lists the directions in which the adjacent features can be found. Some of the room templates can then use this special feature to dynamically substitute these directions into their descriptions."
				.Wrap(builder.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Base Feature: {BaseFeature.ColourName()}");
		sb.AppendLine($"Isolated Feature: {IsolatedRoadFeature.ColourName()}");
		sb.AppendLine($"Start/End of Road Feature: {EndOfTheRoadFeature.ColourName()}");
		sb.AppendLine($"Straight Feature: {StraightRoadFeature.ColourName()}");
		sb.AppendLine($"Bend Feature: {BendInTheRoadFeature.ColourName()}");
		sb.AppendLine($"Tee Intersection Feature: {TeeIntersectionFeature.ColourName()}");
		sb.AppendLine($"Crossroads Feature: {CrossRoadsFeature.ColourName()}");
		sb.AppendLine();
		sb.AppendLine($"Applicable Terrains: {Terrains.Select(x => x.Name.ColourValue()).ListToString()}");
		return sb.ToString();
	}

	public override bool BuildingCommand(ICharacter actor, AutobuilderAreaTerrainRectangleRandomFeatures parent,
		StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, parent, command);
			case "base":
				return BuildingCommandBase(actor, command);
			case "isolated":
				return BuildingCommandIsolated(actor, command);
			case "start":
			case "end":
				return BuildingCommandEnd(actor, command);
			case "straight":
				return BuildingCommandStraight(actor, command);
			case "bend":
				return BuildingCommandBend(actor, command);
			case "tee":
				return BuildingCommandTee(actor, command);
			case "crossroads":
				return BuildingCommandCrossroads(actor, command);
			case "terrain":
			case "terrains":
				return BuildingCommandTerrains(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this command:

	name <name> - renames this feature group
	base <tag> - sets the base tag that applies to all matched rooms
	isolated <tag> - the tag that applies when there are 0 adjacent matching rooms
	end <tag> - the tag that applies when there is 1 adjacent matching rooms
	straight <tag> - the tag that applies when there are 2 adjacent matching rooms in a straight line
	bend <tag> - the tag that applies when there are 2 adjacent matching rooms not in a straight line
	tee <tag> - the tag that applies when there are 3 adjacent matching rooms
	crossroads <tag> - the tag that applies when there are 4+ adjacent matching rooms
	terrain <terrains...> - toggles a list of terrain for this group to apply to+");
		return false;
	}

	private bool BuildingCommandCrossroads(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the base tag applied when there are 4 or more adjacent rooms that match?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		CrossRoadsFeature = tag;
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		actor.OutputHandler.Send(
			$"This group will now apply the crossroads tag {tag.ColourName()}.\n{(framework != null ? $"Note: It matches the {framework.FullName.ColourName()} framework tag." : "Note: It does not match any framework tags.")}");
		return true;
	}

	private bool BuildingCommandTee(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the base tag applied when there are 3 adjacent rooms that match?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		TeeIntersectionFeature = tag;
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		actor.OutputHandler.Send(
			$"This group will now apply the tee intersection tag {tag.ColourName()}.\n{(framework != null ? $"Note: It matches the {framework.FullName.ColourName()} framework tag." : "Note: It does not match any framework tags.")}");
		return true;
	}

	private bool BuildingCommandBend(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the base tag applied when there are 2 adjacent rooms that are not in a straight line of exits?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		BendInTheRoadFeature = tag;
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		actor.OutputHandler.Send(
			$"This group will now apply the bend in the road tag {tag.ColourName()}.\n{(framework != null ? $"Note: It matches the {framework.FullName.ColourName()} framework tag." : "Note: It does not match any framework tags.")}");
		return true;
	}

	private bool BuildingCommandStraight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the base tag applied when there are 2 adjacent rooms in a straight line of exits?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		StraightRoadFeature = tag;
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		actor.OutputHandler.Send(
			$"This group will now apply the straight road tag {tag.ColourName()}.\n{(framework != null ? $"Note: It matches the {framework.FullName.ColourName()} framework tag." : "Note: It does not match any framework tags.")}");
		return true;
	}

	private bool BuildingCommandEnd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the base tag applied when there is only 1 adjacent room that matches?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		EndOfTheRoadFeature = tag;
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		actor.OutputHandler.Send(
			$"This group will now apply the start/end tag {tag.ColourName()}.\n{(framework != null ? $"Note: It matches the {framework.FullName.ColourName()} framework tag." : "Note: It does not match any framework tags.")}");
		return true;
	}

	private bool BuildingCommandIsolated(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to be the base tag applied when there are no matching adjacent rooms?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		IsolatedRoadFeature = tag;
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		actor.OutputHandler.Send(
			$"This group will now apply the isolated tag {tag.ColourName()}.\n{(framework != null ? $"Note: It matches the {framework.FullName.ColourName()} framework tag." : "Note: It does not match any framework tags.")}");
		return true;
	}

	private bool BuildingCommandTerrains(ICharacter actor, StringStack command)
	{
		var list = new List<ITerrain>();
		while (!command.IsFinished)
		{
			var terrain = actor.Gameworld.Terrains.GetByIdOrName(command.PopSpeech());
			if (terrain == null)
			{
				actor.OutputHandler.Send($"There is no terrain identified by {command.Last.ColourCommand()}.");
				return false;
			}

			list.Add(terrain);
		}

		if (!list.Any())
		{
			actor.OutputHandler.Send("You must specify some terrains to toggle.");
			return false;
		}

		foreach (var terrain in list)
		{
			if (Terrains.Contains(terrain))
			{
				Terrains.Remove(terrain);
			}
			else
			{
				Terrains.Add(terrain);
			}
		}

		if (Terrains.Count == 0)
		{
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} road feature group now applies to all terrains.\nNote: This is generally not the way this type of feature group is supposed to be used.");
			return true;
		}

		actor.OutputHandler.Send(
			$"The {Name.ColourName()} road feature group will now match all rooms with the {Terrains.Select(x => x.Name.ColourValue()).ListToString()} {"terrain".Pluralise(Terrains.Count != 1)}");
		return true;
	}

	private bool BuildingCommandBase(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to be the base tag applied to all rooms matched by this group?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		BaseFeature = tag;
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		actor.OutputHandler.Send(
			$"This group will now apply the base tag {tag.ColourName()}.\n{(framework != null ? $"Note: It matches the {framework.FullName.ColourName()} framework tag." : "Note: It does not match any framework tags.")}");
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, AutobuilderAreaTerrainRectangleRandomFeatures parent,
		StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this group?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (parent.TerrainFeatureGroups.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a feature group with that name. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send(
			$"You rename the feature group formerly called {Name.ColourName()} to {name.ColourName()}.");
		Name = name;
		return true;
	}

	public override string StringForParentShow(ICharacter builder)
	{
		return
			$"{Name.ColourName()} - Road Group - {BaseFeature.ColourName()} - {(Terrains.Any() ? Terrains.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ") : "All Terrains".Colour(Telnet.BoldGreen))}";
	}
}
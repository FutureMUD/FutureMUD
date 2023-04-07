using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas.Features;

public class UniformFeatureGroup : TerrainFeatureGroup
{
	public override string ToString()
	{
		return $"Uniform Feature Group with {Features.Select(x => x.Name).ListToString()}";
	}

	public UniformFeatureGroup(XElement root, IFuturemud gameworld)
	{
		NumberOfFeaturesPerRoom = int.Parse(root.Element("NumberOfFeaturesPerRoom")?.Value ?? "1");
		var element = root.Element("Features");
		if (element == null)
		{
			throw new ApplicationException("The UniformFeatureGroup had no Features element.");
		}

		foreach (var sub in element.Elements())
		{
			switch (sub.Attribute("type")?.Value ?? "")
			{
				case "adjacent":
					Features.Add(new AdjacentFeature(sub, gameworld));
					break;
				default:
					Features.Add(new Feature(sub, gameworld));
					break;
			}
		}
	}

	public override XElement SaveToXml()
	{
		return new XElement("Group",
			new XAttribute("type", "uniform"),
			new XElement("NumberOfFeaturesPerRoom", NumberOfFeaturesPerRoom),
			new XElement("Features",
				from feature in Features
				select feature.SaveToXml()
			)
		);
	}

	public UniformFeatureGroup()
	{
		NumberOfFeaturesPerRoom = 1;
	}

	public int NumberOfFeaturesPerRoom { get; set; }

	public IEnumerable<ITerrain> Terrains => Features.SelectMany(x => x.Terrains).Distinct();

	protected bool AppliesToCell(ICell cell)
	{
		return
			Features.Any() &&
			(!Terrains.Any() || Terrains.Contains(cell.CurrentOverlay?.Terrain));
	}

	public override List<Feature> Features { get; } = new();

	public override void ApplyTerrainFeatures(ICell[,] cellMap, List<string>[,] featureMap)
	{
		var width = cellMap.GetLength(0);
		var height = cellMap.GetLength(1);
		if (Features.Count <= NumberOfFeaturesPerRoom)
		{
			for (var i = 0; i < width; i++)
			for (var j = 0; j < height; j++)
			{
				if (!AppliesToCell(cellMap[i, j]))
				{
					continue;
				}

				foreach (var feature in Features.Where(x => x.CanApply(cellMap[i, j])))
				{
					feature.ApplyFeature(cellMap, featureMap, i, j);
				}
			}
		}
		else
		{
			for (var i = 0; i < width; i++)
			for (var j = 0; j < height; j++)
			{
				if (!AppliesToCell(cellMap[i, j]))
				{
					continue;
				}

				foreach (var feature in Features.Where(x => x.CanApply(cellMap[i, j]))
				                                .TakeRandom(NumberOfFeaturesPerRoom, x => x.Weighting))
				{
					feature.ApplyFeature(cellMap, featureMap, i, j);
				}
			}
		}
	}

	public override string Show(ICharacter builder)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Uniform Feature Group - {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(
			$"This feature group will check all rooms in the area and apply {NumberOfFeaturesPerRoom.ToString("N0", builder).ColourValue()} random feature{(NumberOfFeaturesPerRoom == 1 ? "" : "s")} to every one of them that is valid on a terrain basis."
				.Wrap(builder.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine(
			$"Terrains: {(Terrains.Any() ? Terrains.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ") : "All".Colour(Telnet.BoldGreen))}");
		sb.AppendLine();
		sb.AppendLine("Features:");
		var i = 1;
		var total = Features.Sum(x => x.Weighting);
		foreach (var feature in Features)
		{
			sb.AppendLine(
				$"\t{i++.ToString("N0", builder)}) {feature.StringForParentShow(builder)} - {(feature.Weighting / total).ToString("P3", builder).ColourValue()} chance");
		}

		return sb.ToString();
	}

	public override bool BuildingCommand(ICharacter actor, AutobuilderAreaTerrainRectangleRandomFeatures parent,
		StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
				return BuildingCommandName(actor, parent, command);
			case "count":
			case "number":
			case "featuresperroom":
			case "featurecount":
			case "maxfeatures":
			case "maximumfeatures":
				return BuildingCommandFeatureCount(actor, command);

			case "feature":
				return BuildingCommandFeature(actor, parent, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this command:

	name <name> - changes the name of this group
	density <min> <max> - sets the average features per room for this group
	max <#> - sets the maximum number of features from this group for a single room
	feature <#> ... - edits properties of a feature
	feature add simple|adjacent <name> - adds a new feature
	feature remove <#> - removes a feature");
		return false;
	}

	private bool BuildingCommandFeatureCount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How many features should be applied per room from this group?");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("That is not a valid number of features.");
			return false;
		}

		NumberOfFeaturesPerRoom = value;
		actor.OutputHandler.Send(
			$"This group will now apply {value.ToString("N0", actor).ColourValue()} {"feature".Pluralise(value != 1)} per applicable room.");
		return true;
	}

	private bool BuildingCommandFeature(ICharacter actor, AutobuilderAreaTerrainRectangleRandomFeatures parent,
		StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "add":
			case "new":
			case "create":
				return BuildingCommandFeatureAdd(actor, parent, command);
			case "remove":
			case "rem":
			case "delete":
			case "del":
				return BuildingCommandRemove(actor, parent, command);
		}

		if (!int.TryParse(command.Last, out var index) || index < 1 || index > Features.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a valid feature number to edit. There are {Features.Count.ToString("N0", actor).ColourValue()} to choose from.");
			return false;
		}

		return Features[index - 1].BuildingCommand(actor, this, command);
	}

	private bool BuildingCommandRemove(ICharacter actor, AutobuilderAreaTerrainRectangleRandomFeatures parent,
		StringStack command)
	{
		if (!int.TryParse(command.Last, out var index) || index < 1 || index > Features.Count)
		{
			actor.OutputHandler.Send(
				$"You must enter a valid feature number to remove. There are {Features.Count.ToString("N0", actor).ColourValue()} to choose from.");
			return false;
		}

		var which = Features[index - 1];
		actor.OutputHandler.Send(
			$"Are you sure you want to delete the {index.ToOrdinal()} feature ({which.Name.ColourName()})?\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			AcceptAction = text =>
			{
				actor.OutputHandler.Send($"You delete the {which.Name} feature.");
				Features.Remove(which);
				parent.Changed = true;
			},
			RejectAction = text => { actor.OutputHandler.Send($"You decide not to delete the {which.Name} feature."); },
			ExpireAction = () => { actor.OutputHandler.Send($"You decide not to delete the {which.Name} feature."); },
			DescriptionString = $"Deleting the {which.Name} feature",
			Keywords = new List<string> { "delete", "feature" }
		}));
		return true;
	}

	private bool BuildingCommandFeatureAdd(ICharacter actor, AutobuilderAreaTerrainRectangleRandomFeatures parent,
		StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"Which type of feature do you want to add? The valid choices are {"simple".ColourCommand()} or {"adjacent".ColourCommand()}.");
			return false;
		}

		var type = command.PopSpeech().ToLowerInvariant();
		switch (type)
		{
			case "simple":
			case "adjacent":
				break;
			default:
				actor.OutputHandler.Send(
					$"That is not a valid feature type. The valid choices are {"simple".ColourCommand()} or {"adjacent".ColourCommand()}.");
				return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What tag do you want to be applied with that feature?");
			return false;
		}

		var tag = command.SafeRemainingArgument.TitleCase();
		var framework = actor.Gameworld.Tags.FirstOrDefault(x => x.Name.CollapseString().EqualTo(tag));
		Feature item = null;
		switch (type)
		{
			case "simple":
				item = new Feature(actor.Gameworld, tag);
				Features.Add(item);
				break;
			case "adjacent":
				item = new AdjacentFeature(actor.Gameworld, tag);
				Features.Add(item);
				break;
		}

		actor.OutputHandler.Send(
			$"You create a new {type} feature called {tag.ColourName()}, which is the {Features.IndexOf(item).ToOrdinal().ColourValue()} feature in this group.");
		if (framework == null)
		{
			actor.OutputHandler.Send($"Note: That feature is not a framework tag.");
		}
		else
		{
			actor.OutputHandler.Send($"Note: That features matches the {framework.FullName.ColourName()} tag.");
		}

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
			$"{Name.ColourName()} - Uniform Group - {Features.Count.ToString("N0", builder)} {"feature".Pluralise(Features.Count != 1)} - {(Terrains.Any() ? Terrains.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ") : "All Terrains".Colour(Telnet.BoldGreen))}";
	}
}
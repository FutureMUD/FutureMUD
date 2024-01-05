using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas.Features;

public class SimpleFeatureGroup : TerrainFeatureGroup
{
	public override string ToString()
	{
		return $"Simple Feature Group with {Features.Select(x => x.Name).ListToString()}";
	}

	public override XElement SaveToXml()
	{
		return new XElement("Group",
			new XAttribute("type", "simple"),
			new XElement("MaximumFeatureDensity", MaximumFeatureDensity),
			new XElement("MinimumFeatureDensity", MinimumFeatureDensity),
			new XElement("MaximumFeaturesPerRoom", MaximumFeaturesPerRoom),
			new XElement("Features",
				from feature in Features
				select feature.SaveToXml()
			)
		);
	}

	public SimpleFeatureGroup(XElement root, IFuturemud gameworld)
	{
		MaximumFeatureDensity = double.Parse(root.Element("MaximumFeatureDensity")?.Value ??
		                                     throw new ApplicationException(
			                                     "The SimpleFeatureGroup had no MaximumFeatureDensity element."));
		MinimumFeatureDensity = double.Parse(root.Element("MinimumFeatureDensity")?.Value ??
		                                     throw new ApplicationException(
			                                     "The SimpleFeatureGroup had no MinimumFeatureDensity element."));
		MaximumFeaturesPerRoom = int.Parse(root.Element("MaximumFeaturesPerRoom")?.Value ??
		                                   throw new ApplicationException(
			                                   "The SimpleFeatureGroup had no MaximumFeaturesPerRoom element."));

		var element = root.Element("Features");
		if (element == null)
		{
			throw new ApplicationException("The SimpleFeatureGroup had no Features element.");
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

	public SimpleFeatureGroup()
	{
		MaximumFeatureDensity = 1.0;
		MinimumFeatureDensity = 0.25;
		MaximumFeaturesPerRoom = 2;
	}

	public double MaximumFeatureDensity { get; set; }
	public double MinimumFeatureDensity { get; set; }
	public int MaximumFeaturesPerRoom { get; set; }

	public IEnumerable<ITerrain> Terrains => Features.SelectMany(x => x.Terrains).Distinct();

	protected bool AppliesToCell(ICell cell)
	{
		return
			Features.Any() &&
			(!Terrains.Any() || Terrains.Contains(cell.CurrentOverlay?.Terrain));
	}

	public override void ApplyTerrainFeatures(ICell[,] cellMap, List<string>[,] featureMap)
	{
		var width = cellMap.GetLength(0);
		var height = cellMap.GetLength(1);
		var cells = cellMap.Cast<ICell>().WhereNotNull(x => x).Where(AppliesToCell).ToList();

		var howMany = (int)Math.Round(RandomUtilities.DoubleRandom(MinimumFeatureDensity, MaximumFeatureDensity) *
		                              height * width);
		var counter = new Counter<string>(StringComparer.InvariantCultureIgnoreCase);
		var groupCounter = new Counter<ICell>();
		var featuresInConsideration = Features.ToList();
		while (howMany > 0)
		{
			if (!featuresInConsideration.Any())
			{
				break;
			}

			var feature = Features.GetWeightedRandom(x => x.Weighting);
			if (feature.MaximumCount > 0)
			{
				var count = counter.Count(feature.Name);
				if (count >= feature.MaximumCount)
				{
					featuresInConsideration.Remove(feature);
					continue;
				}
			}

			var validCells = cells.Where(x => feature.CanApply(x)).ToList();
			if (!validCells.Any(x => groupCounter.Count(x) < MaximumFeaturesPerRoom))
			{
				featuresInConsideration.Remove(feature);
				continue;
			}

			var cell = validCells.GetRandomElement();
			var (X, Y) = cellMap.GetCoordsOfElement(cell);
			feature.ApplyFeature(cellMap, featureMap, X, Y);
			counter.Increment(feature.Name);
			groupCounter.Increment(cell);
			howMany--;
		}

		foreach (var feature in Features)
		{
			if (counter.Count(feature.Name) >= feature.MinimumCount)
			{
				continue;
			}

			var target = feature.MinimumCount - counter.Count(feature.Name);
			while (target > 0)
			{
				var validCells = cells.Where(x => feature.CanApply(x)).ToList();
				if (!validCells.Any(x => groupCounter.Count(x) > MaximumFeaturesPerRoom))
				{
					break;
				}

				var cell = validCells.GetRandomElement();
				var (X, Y) = cellMap.GetCoordsOfElement(cell);
				feature.ApplyFeature(cellMap, featureMap, X, Y);
				counter.Increment(feature.Name);
				groupCounter.Increment(cell);
				target--;
			}
		}
	}

	public override string Show(ICharacter builder)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Uniform Feature Group - {Name.ColourName()}");
		sb.AppendLine();
		sb.AppendLine(
			$"This feature group will apply a random spattering of its features across the whole area based on a range of feature densities (i.e. average number of features from this group per room)."
				.Wrap(builder.InnerLineFormatLength));
		sb.AppendLine();
		sb.AppendLine($"Maximum Features Per Room: {MaximumFeaturesPerRoom.ToString("N0", builder).ColourValue()}");
		sb.AppendLine(
			$"Maximum Feature Density: {MaximumFeatureDensity.ToString("N3", builder).ColourValue()} avg per room");
		sb.AppendLine(
			$"Minimum Feature Density: {MinimumFeatureDensity.ToString("N3", builder).ColourValue()} avg per room");
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
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, parent, command);
			case "density":
				return BuildingCommandDensity(actor, command);
			case "max":
			case "maxfeatures":
			case "featuresperroom":
			case "maxcount":
				return BuildingCommandMaxFeatures(actor, command);
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

	private bool BuildingCommandMaxFeatures(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the maximum number of features from this group that can be applied to any one room?");
			return false;
		}

		if (int.TryParse(command.SafeRemainingArgument, out var value) || value < 1)
		{
			actor.OutputHandler.Send("You must enter a valid number.");
			return false;
		}

		MaximumFeaturesPerRoom = value;
		actor.OutputHandler.Send(
			$"There will now be a maximum of {value.ToString("N0", actor).ColourValue()} features from this group applied to any one room.");
		return true;
	}

	private bool BuildingCommandDensity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the minimum density (average number per room) of features from this group applied across the area?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var min) || min < 0.0)
		{
			actor.OutputHandler.Send($"You must enter a valid number for the minimum density.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What should be the maximum density (average number per room) of features from this group applied across the area?");
			return false;
		}

		if (!double.TryParse(command.PopSpeech(), out var max) || min < 0.0)
		{
			actor.OutputHandler.Send($"You must enter a valid number for the maximum density.");
			return false;
		}

		if (min > max)
		{
			actor.OutputHandler.Send($"The minimum density must be lower than the maximum density.");
			return false;
		}

		MinimumFeatureDensity = min;
		MaximumFeatureDensity = max;

		actor.OutputHandler.Send(
			$"There will now be between {min.ToString("N3", actor).ColourValue()} and {max.ToString("N3", actor).ColourValue()} features from this group applied per room on average across the area.");
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

	public override List<Feature> Features { get; } = new();

	public override string StringForParentShow(ICharacter builder)
	{
		return
			$"{Name.ColourName()} - Simple Group - {Features.Count.ToString("N0", builder)} {"feature".Pluralise(Features.Count != 1)} - {(Terrains.Any() ? Terrains.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ") : "All Terrains".Colour(Telnet.BoldGreen))}";
	}
}
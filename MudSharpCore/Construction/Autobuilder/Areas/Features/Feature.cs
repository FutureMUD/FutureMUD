using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Areas.Features;

public class Feature
{
	public Feature(IFuturemud gameworld, string name)
	{
		Name = name;
		Weighting = 100.0;
		MinimumCount = 0;
		MaximumCount = 1;
		Terrains = new HashSet<ITerrain>();
	}

	public Feature(XElement root, IFuturemud gameworld)
	{
		Name = root.Element("Name")?.Value ??
		       throw new ApplicationException("There was no Name element for the Feature.");
		Weighting = double.Parse(root.Element("Weighting")?.Value ?? "1.0");
		MaximumCount = int.Parse(root.Element("MaximumCount")?.Value ?? int.MaxValue.ToString());
		MinimumCount = int.Parse(root.Element("MinimumCount")?.Value ?? "0");
		Terrains = (root.Element("Terrains")?
		                .Elements()
		                .Select(x =>
			                long.TryParse((string?)x.Value, out var value)
				                ? gameworld.Terrains.Get(value)
				                : gameworld.Terrains.GetByName(x.Value)
		                ) ?? Enumerable.Empty<ITerrain>())
			.ToHashSet();
	}

	public string Name { get; set; }
	public double Weighting { get; set; }
	public int MaximumCount { get; set; }
	public int MinimumCount { get; set; }
	public HashSet<ITerrain> Terrains { get; set; }

	public virtual string StringForParentShow(ICharacter builder)
	{
		return
			$"{(MinimumCount == MaximumCount ? MinimumCount.ToString("N0", builder) : $"{MinimumCount.ToString("N0", builder)}-{MaximumCount.ToString("N0", builder)}")} {Name.ColourName()} - {Terrains.Select(x => x.Name.ColourValue()).DefaultIfEmpty("All".Colour(Telnet.BoldGreen)).ListToString(conjunction: "or ")}";
	}

	public virtual bool CanApply(ICell cell)
	{
		return !Terrains.Any() || Terrains.Contains(cell.Terrain(null));
	}

	public virtual void ApplyFeature(ICell[,] cellMap, List<string>[,] features, int x, int y)
	{
		features[x, y].Add(Name);
	}

	public virtual XElement SaveToXml()
	{
		return new XElement("Feature",
			new XAttribute("type", "none"),
			new XElement("Name", new XCData(Name)),
			new XElement("Weighting", Weighting),
			new XElement("MaximumCount", MaximumCount),
			new XElement("MinimumCount", MinimumCount),
			new XElement("Terrains",
				from terrain in Terrains
				select new XElement("Terrain", terrain.Id)
			)
		);
	}

	public virtual string BuildingHelpText => @"You can use the following options:

	name <tag> - the name of the feature (or framework tag) that will be applied by this feature element
	weight <weight> - the relative weight of this feature in random selection
	max <#> - sets the maximum number of these features that can be applied to a whole area
	min <#> - sets the minimum number of these features that can be applied to a whole area
	terrain <terrains...> - toggles a list of terrains that this feature requires";

	public virtual bool BuildingCommand(ICharacter actor, TerrainFeatureGroup parent, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "name":
			case "tag":
			case "feature":
				return BuildingCommandName(actor, parent, command);
			case "weight":
			case "weighting":
			case "chance":
				return BuildingCommandWeight(actor, parent, command);
			case "maximumcount":
			case "maxcount":
			case "max":
			case "maximum":
				return BuildingCommandMaximumCount(actor, command);
			case "minimumcount":
			case "mincount":
			case "min":
			case "minimum":
				return BuildingCommandMinimumCount(actor, command);
			case "terrain":
			case "terrains":
				return BuildingCommandTerrain(actor, command);
			default:
				actor.OutputHandler.Send(BuildingHelpText);
				return false;
		}
	}

	private bool BuildingCommandTerrain(ICharacter actor, StringStack command)
	{
		var list = new List<ITerrain>();
		while (!command.IsFinished)
		{
			var terrain = actor.Gameworld.Terrains.GetByIdOrName(command.PopSpeech());
			if (terrain == null)
			{
				actor.OutputHandler.Send($"There was no terrain identified by {command.Last.ColourCommand()}.");
				return false;
			}

			list.Add(terrain);
		}

		if (list.Count == 0)
		{
			actor.OutputHandler.Send("You must enter some terrains to toggle.");
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

		if (!Terrains.Any())
		{
			actor.OutputHandler.Send($"The {Name.ColourName()} feature can now apply to any terrain.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"The {Name.ColourName()} feature now applies to the {Terrains.Select(x => x.Name.ColourValue()).ListToString()} terrain{(Terrains.Count == 1 ? "" : "s")}.");
		}

		return true;
	}

	private bool BuildingCommandMinimumCount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a minimum count of the number of these tags that can be applied to an entire area.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"You must enter a valid number 0 or greater.");
			return false;
		}

		MinimumCount = value;

		actor.OutputHandler.Send(
			$"There will now be a minimum of {value.ToString("N0", actor).ColourValue()} rooms that can have the {Name.ColourName()} feature applied in any generated area.");
		return true;
	}

	private bool BuildingCommandMaximumCount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must either specify a maximum count of the number of these tags that can be applied to an entire area, or use 0 to set no limit.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send($"You must enter a valid number, or 0 to set no limit.");
			return false;
		}

		MaximumCount = value;
		if (MaximumCount == 0)
		{
			actor.OutputHandler.Send(
				$"There will be no limit to the number of rooms in an area that can have the {Name.ColourName()} feature apply.");
			return true;
		}

		actor.OutputHandler.Send(
			$"There will now be a maximum of {value.ToString("N0", actor).ColourValue()} rooms that can have the {Name.ColourName()} feature applied in any generated area.");
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, TerrainFeatureGroup parent, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should be the relative weight of this feature being selected?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid number that is greater than zero.");
			return false;
		}

		Weighting = value;
		actor.OutputHandler.Send(
			$"This feature now has a relative weight of {Weighting.ToString("N3", actor).ColourValue()}, which gives it a {(Weighting / parent.Features.Sum(x => x.Weighting)).ToString("P2", actor).ColourValue()} chance to be selected.");
		throw new NotImplementedException();
	}

	private bool BuildingCommandName(ICharacter actor, TerrainFeatureGroup parent, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this feature?");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase();
		if (parent.Features.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a feature with that name. Features must be unique.");
			return false;
		}

		Name = name;
		actor.OutputHandler.Send($"This feature will now apply a tag named {Name.Colour(Telnet.Cyan)}.");
		if (actor.Gameworld.Tags.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send(
				$"Note: This matches framework tag {actor.Gameworld.Tags.First(x => x.Name.EqualTo(name)).FullName.ColourName()}.");
		}

		return true;
	}
}
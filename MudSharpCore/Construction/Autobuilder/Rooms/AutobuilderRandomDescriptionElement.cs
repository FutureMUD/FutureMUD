using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Rooms;

public class AutobuilderRandomDescriptionElement : IAutobuilderRandomDescriptionElement
{
	public IFuturemud Gameworld { get; }

	public AutobuilderRandomDescriptionElement(IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Weight = 100.0;
		MandatoryIfValid = false;
		MandatoryPosition = 100000;
		_tags = new List<string>();
		Terrains = new List<ITerrain>();
		Text = "There is a notable feature of some kind here";
		RoomNameText = "{0}";
	}

	public AutobuilderRandomDescriptionElement(XElement root, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Text = root.Element("Text")?.Value ??
		       throw new ApplicationException($"AutobuilderRandomDescriptionElement lacked a text tag:\n{root}");
		RoomNameText = root.Element("RoomNameText")?.Value;
		Weight = double.Parse(root.Element("Weight")?.Value ?? "1.0");
		_tags = root.Element("Tags")?.Value.Split(',').ToList();
		Terrains = (root.Element("Terrains")?.Elements().SelectNotNull(x =>
			long.TryParse(x.Value, out var value)
				? gameworld.Terrains.Get(value)
				: gameworld.Terrains.GetByName(x.Value)) ?? Enumerable.Empty<Terrain>()).ToList();
		MandatoryIfValid = bool.Parse(root.Attribute("mandatory")?.Value ?? "false");
		MandatoryPosition = int.Parse(root.Attribute("fixedposition")?.Value ?? "100000");
	}

	public virtual XElement SaveToXml()
	{
		return new XElement("Description",
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

	public double Weight { get; protected set; }
	public string Text { get; protected set; }
	public string RoomNameText { get; protected set; }
	public List<ITerrain> Terrains { get; }
	private readonly List<string> _tags;
	public IEnumerable<string> Tags => _tags;
	public bool MandatoryIfValid { get; protected set; }
	public int MandatoryPosition { get; protected set; }

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "weight":
				return BuildingCommandWeight(actor, command);
			case "text":
				return BuildingCommandText(actor, command);
			case "name":
			case "roomname":
				return BuildingCommandName(actor, command);
			case "mandatory":
				return BuildingCommandMandatory(actor, command);
			case "fixed":
			case "fixedposition":
				return BuildingCommandFixedPosition(actor, command);
			case "terrain":
				return BuildingCommandTerrain(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this command:

    weight <weight> - sets the relative weight of this being selected
    fixedposition <#> - sets the fixed position of the sentence in the paragraph
    mandatory - toggles being mandatory to apply to all descriptions if valid
    terrain <terrains...> - toggles terrains for which this description will apply
    tags <tags...> - toggles which tags are mandatory for this description to apply
    name <text> - sets the name of the room if this element is chosen
    text <text> - sets the sentence added to the room description if this element is chosen");
		return false;
	}

	protected virtual bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		var tags = new List<string>();
		while (!command.IsFinished)
		{
			tags.Add(command.PopSpeech().ToLowerInvariant());
		}

		if (!tags.Any())
		{
			actor.OutputHandler.Send("You must specify some tags to add or remove.");
			return false;
		}

		foreach (var tag in tags)
		{
			if (_tags.Contains(tag))
			{
				_tags.Remove(tag);
			}
			else
			{
				_tags.Add(tag);
			}
		}

		if (_tags.Count == 0)
		{
			actor.OutputHandler.Send(
				$"This description element no longer requires any particular tags, and can always apply.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"This description element now requires the {_tags.Select(x => x.ColourCommand()).ListToString()} tag{(_tags.Count == 1 ? "" : "s")}");
		}

		return true;
	}

	private bool BuildingCommandTerrain(ICharacter actor, StringStack command)
	{
		var terrains = new List<ITerrain>();
		while (!command.IsFinished)
		{
			var terrain = actor.Gameworld.Terrains.GetByIdOrName(command.PopSpeech());
			if (terrain == null)
			{
				actor.OutputHandler.Send($"There is no terrain that is identified by {command.Last.ColourCommand()}.");
				return false;
			}

			terrains.Add(terrain);
		}

		if (terrains.Count == 0)
		{
			actor.OutputHandler.Send("You must specify at least one terrain to toggle.");
			return false;
		}

		foreach (var terrain in terrains)
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
			actor.OutputHandler.Send("This description element can now apply to any terrain.");
		}
		else
		{
			actor.OutputHandler.Send(
				$"This description element can now apply for the terrains {Terrains.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ")}.");
		}

		return true;
	}

	private bool BuildingCommandFixedPosition(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"You must enter a relative position that this element must appear in the description paragraph, or use 0 to remove it from being in a fixed position.");
			return false;
		}

		if (!int.TryParse(command.SafeRemainingArgument, out var value) || value < 0)
		{
			actor.OutputHandler.Send(
				"You must enter a relative position that this element must appear in the description paragraph, or use 0 to remove it from being in a fixed position.");
			return false;
		}

		if (value == 0)
		{
			MandatoryPosition = 100000;
			actor.OutputHandler.Send("This description element no longer uses a fixed position within the paragraph.");
			return true;
		}

		MandatoryPosition = value;
		actor.OutputHandler.Send(
			$"This description element now orders itself at position {value.ToString("N0", actor).ColourValue()} in the paragraph.");
		return true;
	}

	private bool BuildingCommandMandatory(ICharacter actor, StringStack command)
	{
		MandatoryIfValid = !MandatoryIfValid;
		actor.OutputHandler.Send(
			$"This description element is {(MandatoryIfValid ? "now" : "no longer")} mandatory for all randomly generated descriptions if valid by tags and terrain.");
		return true;
	}

	protected virtual bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"What do you want to set as the room name when this element is used?\nYou can use {0} to substitute the auto-builder's room name for the terrain type, and {1} to substitute the terrain's name.");
			return false;
		}

		var name = command.SafeRemainingArgument.TitleCase().SanitiseExceptNumbered(1);
		RoomNameText = name;
		actor.OutputHandler.Send(
			$"This element will now contribute the following name to the generated room: {RoomNameText.ColourName()}");
		return true;
	}

	protected virtual bool BuildingCommandText(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set as the text used in the description?");
			return false;
		}

		var text = command.SafeRemainingArgument.TitleCase().Sanitise();
		Text = text;
		actor.OutputHandler.Send(
			$"This element will now contribute the following sentence to the description of to the generated room: {RoomNameText.ColourName()}");
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What relative weight should this element have of being picked?");
			return false;
		}

		if (!double.TryParse(command.SafeRemainingArgument, out var value) || value <= 0)
		{
			actor.OutputHandler.Send("The weight must be a number greater than zero.");
			return false;
		}

		Weight = value;
		actor.OutputHandler.Send(
			$"This element will have a relative weight of {Weight.ToString("N3", actor).ColourValue()} for being picked.");
		return true;
	}

	public virtual string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Description Element".Colour(Telnet.Cyan));
		sb.AppendLine($"Weight: {Weight.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Required Tags: {_tags.Select(x => x.ColourCommand()).ListToString()}");
		sb.AppendLine(
			$"Applicable Terrains: {Terrains.Select(x => x.Name.ColourValue()).ListToString(conjunction: "or ")}");
		sb.AppendLine($"Mandatory If Valid: {MandatoryIfValid.ToColouredString()}");
		sb.AppendLine(
			$"Fixed Sentence Position: {(MandatoryPosition == 100000 ? "None".Colour(Telnet.Red) : MandatoryPosition.ToString("N0", actor).ColourValue())}");
		sb.AppendLine();
		sb.AppendLine($"Room Name: {RoomNameText.ColourCommand()}");
		sb.AppendLine($"Description Text: {Text.ColourCommand()}");
		return sb.ToString();
	}

	public virtual IAutobuilderRandomDescriptionElement Clone()
	{
		return new AutobuilderRandomDescriptionElement(SaveToXml(), Gameworld);
	}

	public virtual bool Applies(ITerrain terrain, IEnumerable<string> tags)
	{
		return (Terrains.Contains(terrain) || !Terrains.Any()) && Tags.All(tags.Contains);
	}

	public virtual (string RoomName, string DescriptionText) TextForTags(ITerrain terrain, IEnumerable<string> tags)
	{
		return (RoomNameText, Text);
	}
}
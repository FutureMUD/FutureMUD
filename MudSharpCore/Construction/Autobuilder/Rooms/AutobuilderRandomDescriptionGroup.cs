using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Framework;

namespace MudSharp.Construction.Autobuilder.Rooms;

public class AutobuilderRandomDescriptionGroup : IAutobuilderRandomDescriptionElement
{
	public IFuturemud Gameworld { get; }

	public AutobuilderRandomDescriptionGroup(IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Weight = 100.0;
		MandatoryIfValid = false;
		MandatoryPosition = 100000;
	}

	public AutobuilderRandomDescriptionGroup(XElement root, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Weight = double.Parse(root.Element("Weight")?.Value ?? "1.0");
		MandatoryIfValid = bool.Parse(root.Attribute("mandatory")?.Value ?? "false");
		MandatoryPosition = int.Parse(root.Attribute("fixedposition")?.Value ?? "100000");
		foreach (var element in root.Elements("Description"))
		{
			_subElements.Add(AutobuilderRoomDescriptionElementFactory.LoadElement(element, gameworld));
		}
	}

	public XElement SaveToXml()
	{
		return new XElement("Description",
			new XAttribute("type", "group"),
			new XAttribute("mandatory", MandatoryIfValid),
			new XAttribute("fixedposition", MandatoryPosition),
			new XElement("Weight", Weight),
			from item in _subElements
			select item.SaveToXml()
		);
	}

	private readonly List<IAutobuilderRandomDescriptionElement> _subElements = new();
	public IEnumerable<IAutobuilderRandomDescriptionElement> SubElements => _subElements;

	#region Implementation of IAutobuilderRandomDescriptionElement

	public double Weight { get; protected set; }

	public bool Applies(ITerrain terrain, IEnumerable<string> tags)
	{
		return _subElements.Any(x => x.Applies(terrain, tags));
	}

	public (string RoomName, string DescriptionText) TextForTags(ITerrain terrain, IEnumerable<string> tags)
	{
		var valid = _subElements.Where(x => x.Applies(terrain, tags)).ToList();
		var mandatory = valid.Where(x => x.MandatoryIfValid).ToList();
		return (mandatory.GetWeightedRandom(x => x.Weight) ??
		        valid.GetWeightedRandom(x => x.Weight)).TextForTags(terrain, tags);
	}

	public IEnumerable<string> Tags => _subElements.SelectMany(x => x.Tags).Distinct();

	public bool MandatoryIfValid { get; private set; }
	public int MandatoryPosition { get; private set; }

	#endregion

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "weight":
				return BuildingCommandWeight(actor, command);
			case "mandatory":
				return BuildingCommandMandatory(actor, command);
			case "fixed":
			case "fixedposition":
				return BuildingCommandFixedPosition(actor, command);
			case "item":
			case "element":
			case "elem":
				return BuildingCommandElement(actor, command);
		}

		actor.OutputHandler.Send(@"You can use the following options with this command:

    weight <weight> - sets the relative weight of this being selected
    fixedposition <#> - sets the fixed position of the sentence in the paragraph
    mandatory - toggles being mandatory to apply to all descriptions if valid
    item add <type> - adds a new description element to this group
    item <#> - views detailed information about a description element
    item <#> remove - removes the specified description element
    item <#> weight <weight> - sets the weight of a description element
    item <#> mandatory - toggles an element being mandatory
    item <#> name <text> - sets the room name of a description element
    item <#> text <text> - sets the sentence added by the element
    item <#> tag <tags...> - toggles the list of tags for an element
    item <#> terrain <terrains...> - toggles the list of terrains for an element");

		return false;
	}

	private bool BuildingCommandElement(ICharacter actor, StringStack command)
	{
		var text = command.PopSpeech();
		if (text.EqualTo("add"))
		{
			if (command.IsFinished)
			{
				actor.OutputHandler.Send(
					$"What type of description element do you want to add? The valid options are {"simple".ColourCommand()} and {"road".ColourCommand()}");
				return false;
			}

			switch (command.PopSpeech().ToLowerInvariant())
			{
				case "simple":
					_subElements.Add(new AutobuilderRandomDescriptionElement(Gameworld));
					actor.OutputHandler.Send(
						$"You add a new simple description element at position {_subElements.Count.ToString("N0", actor).ColourValue()}.");
					return true;
				case "road":
					_subElements.Add(new AutobuilderRoadRandomDescriptionElement(Gameworld));
					actor.OutputHandler.Send(
						$"You add a new road description element at position {_subElements.Count.ToString("N0", actor).ColourValue()}.");
					return true;
				default:
					actor.OutputHandler.Send(
						$"That is not a valid type of description element. The valid options are {"simple".ColourCommand()} and {"road".ColourCommand()}");
					return false;
			}
		}

		if (!int.TryParse(text, out var value) || value < 1 || value > _subElements.Count)
		{
			actor.OutputHandler.Send(
				$"That is not a valid index for a description element. You must enter a number between {1.ToString("N0", actor).ColourValue()} and {_subElements.Count.ToString("N0", actor).ColourValue()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send(_subElements[value - 1].Show(actor));
			return false;
		}

		return _subElements[value - 1].BuildingCommand(actor, command);
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
		sb.AppendLine($"Description Group".Colour(Telnet.Cyan));
		sb.AppendLine($"Weight: {Weight.ToString("N3", actor).ColourValue()}");
		sb.AppendLine($"Mandatory If Valid: {MandatoryIfValid.ToColouredString()}");
		sb.AppendLine(
			$"Fixed Sentence Position: {(MandatoryPosition == 100000 ? "None".Colour(Telnet.Red) : MandatoryPosition.ToString("N0", actor).ColourValue())}");
		sb.AppendLine();
		sb.AppendLine($"Elements:");
		var i = 1;
		foreach (var element in _subElements)
		{
			if (element is AutobuilderRoadRandomDescriptionElement)
			{
				sb.AppendLine(
					$"\t{i++.ToString("N0", actor)}) Road Element with tags {element.Tags.Select(x => x.ColourCommand()).ListToString()}");
				continue;
			}

			sb.AppendLine(
				$"\t{i++.ToString("N0", actor)}) Simple Element with tags {element.Tags.Select(x => x.ColourCommand()).ListToString()}");
		}

		return sb.ToString();
	}

	public IAutobuilderRandomDescriptionElement Clone()
	{
		return new AutobuilderRandomDescriptionGroup(SaveToXml(), Gameworld);
	}
}
#nullable enable

using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Projects.ConcreteTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ProjectAction = MudSharp.Models.ProjectAction;

namespace MudSharp.Work.Projects.Actions;

public class CommodityOutputProjectAction : BaseAction
{
	private readonly Dictionary<ICharacteristicDefinition, ICharacteristicValue> _characteristics = new();
	private long _materialId;
	private long _tagId;

	public CommodityOutputProjectAction(ProjectAction action, IFuturemud gameworld) : base(action, gameworld)
	{
		var root = XElement.Parse(action.Definition);
		_materialId = GetLongElement(root, "MaterialId");
		_tagId = GetLongElement(root, "TagId");
		Material = Gameworld.Materials.Get(_materialId);
		Tag = _tagId == 0 ? null : Gameworld.Tags.Get(_tagId);
		Weight = GetDoubleElement(root, "Weight", 0.0);
		UseIndirectDescription = GetBoolElement(root, "UseIndirectDescription", false);
		Echo = root.Element("Echo")?.Value;
		if (string.IsNullOrWhiteSpace(Echo))
		{
			Echo = null;
		}

		foreach (var element in root.Element("Characteristics")?.Elements("Characteristic") ??
		                        Enumerable.Empty<XElement>())
		{
			var definitionId = GetLongAttribute(element, "definition");
			var valueId = GetLongAttribute(element, "value");
			var definition = Gameworld.Characteristics.Get(definitionId);
			var value = Gameworld.CharacteristicValues.Get(valueId);
			if (definition is null || value is null || !definition.IsValue(value))
			{
				continue;
			}

			_characteristics[definition] = value;
		}
	}

	public CommodityOutputProjectAction(IProjectPhase phase, IFuturemud gameworld) : base(phase, gameworld,
		"commodityoutput")
	{
		Description = "Create a commodity pile at the project location.";
		Weight = 1000.0;
		Changed = true;
	}

	public CommodityOutputProjectAction(CommodityOutputProjectAction rhs, IProjectPhase newPhase) : base(rhs, newPhase,
		"commodityoutput")
	{
		_materialId = rhs._materialId;
		_tagId = rhs._tagId;
		Material = rhs.Material;
		Weight = rhs.Weight;
		Tag = rhs.Tag;
		UseIndirectDescription = rhs.UseIndirectDescription;
		Echo = rhs.Echo;
		foreach (var characteristic in rhs._characteristics)
		{
			_characteristics[characteristic.Key] = characteristic.Value;
		}
	}

	public ISolid? Material { get; protected set; }

	public double Weight { get; protected set; }

	public ITag? Tag { get; protected set; }

	public bool UseIndirectDescription { get; protected set; }

	public string? Echo { get; protected set; }

	public IReadOnlyDictionary<ICharacteristicDefinition, ICharacteristicValue> Characteristics => _characteristics;

	protected override XElement SaveDefinition()
	{
		return new XElement("Action",
			new XElement("MaterialId", Material?.Id ?? _materialId),
			new XElement("Weight", Weight.ToString("R", System.Globalization.CultureInfo.InvariantCulture)),
			new XElement("TagId", Tag?.Id ?? _tagId),
			new XElement("UseIndirectDescription", UseIndirectDescription),
			new XElement("Echo", Echo ?? string.Empty),
			new XElement("Characteristics",
				from characteristic in _characteristics.OrderBy(x => x.Key.Id)
				select new XElement("Characteristic",
					new XAttribute("definition", characteristic.Key.Id),
					new XAttribute("value", characteristic.Value.Id)
				)
			)
		);
	}

	public override void CompleteAction(IActiveProject project)
	{
		var (valid, _) = CanSubmit();
		if (!valid || Material is null)
		{
			return;
		}

		var location = ResolveProjectLocation(project);
		if (location is null)
		{
			project.CharacterOwner?.OutputHandler.Send(
				$"Project action {Name.ColourName()} could not create commodity output because no project location was available.");
			return;
		}

		var roomLayer = ResolveRoomLayer(project, location);
		var item = CommodityGameItemComponentProto.CreateNewCommodity(Material, Weight, Tag, UseIndirectDescription,
			_characteristics.Select(x => (x.Key, x.Value)));
		if (project.CharacterOwner is not null)
		{
			item.SetOwner(project.CharacterOwner);
		}
		item.RoomLayer = roomLayer;
		Gameworld.Add(item);
		location.Insert(item, true);
		location.HandleRoomEcho(Echo ?? $"A new pile of {Material.Name.Colour(Material.ResidueColour)} is produced by the project.",
			roomLayer);
	}

	private static ICell? ResolveProjectLocation(IActiveProject project)
	{
		return (project as MudSharp.Work.Projects.ConcreteTypes.ActiveProject)?.Location ?? project.CharacterOwner?.Location;
	}

	private static RoomLayer ResolveRoomLayer(IActiveProject project, ICell location)
	{
		return project.ActiveLabour
		              .Select(x => x.Character)
		              .FirstOrDefault(x => x?.Location == location)
		              ?.RoomLayer ??
		       project.CharacterOwner?.RoomLayer ??
		       RoomLayer.GroundLevel;
	}

	public override IProjectAction Duplicate(IProjectPhase newPhase)
	{
		return new CommodityOutputProjectAction(this, newPhase);
	}

	public override string Show(ICharacter actor)
	{
		return
			$"[{Name}] {DescribeOutput(actor)} - {Description}";
	}

	public override string ShowToPlayer(ICharacter actor)
	{
		return Description;
	}

	private string DescribeOutput(ICharacter actor)
	{
		var materialText = Material?.Name.Colour(Material.ResidueColour) ?? "no material".ColourError();
		var weightText = Weight > 0.0
			? Gameworld.UnitManager.DescribeExact(Weight, UnitType.Mass, actor).ColourValue()
			: "no weight".ColourError();
		var tagText = Tag is null ? string.Empty : $" tagged {Tag.FullName.ColourName()}";
		var characteristicText = _characteristics.Count == 0
			? string.Empty
			: $" with {_characteristics.OrderBy(x => x.Key.Name).ThenBy(x => x.Key.Id).Select(x => $"{x.Key.Name.ColourName()} = {x.Value.GetValue.ColourValue()}").ListToString()}";
		return $"{weightText} of {materialText}{tagText}{characteristicText}";
	}

	public override (bool Truth, string Error) CanSubmit()
	{
		if (_materialId == 0 && Material is null)
		{
			return (false, "You must set a solid material for the commodity output.");
		}

		if (Material is null)
		{
			return (false, "The configured commodity output material no longer exists.");
		}

		if (Weight <= 0.0)
		{
			return (false, "The commodity output weight must be positive.");
		}

		if (_tagId != 0 && Tag is null)
		{
			return (false, "The configured commodity output tag no longer exists.");
		}

		return base.CanSubmit();
	}

	protected override string HelpText => $@"{base.HelpText}

	#3material <material>#0 - sets the solid material output
	#3weight <amount>#0 - sets the output mass
	#3tag <tag|none>#0 - sets or clears the commodity pile tag
	#3indirect#0 - toggles indirect quantity descriptions
	#3echo <text|none>#0 - sets or clears the completion echo
	#3characteristic <definition> <value|remove>#0 - sets or clears a fixed output commodity characteristic";

	public override bool BuildingCommand(ICharacter actor, StringStack command, IProjectPhase phase)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "material":
			case "mat":
				return BuildingCommandMaterial(actor, command);
			case "weight":
			case "mass":
			case "amount":
				return BuildingCommandWeight(actor, command);
			case "tag":
				return BuildingCommandTag(actor, command);
			case "indirect":
				return BuildingCommandIndirect(actor);
			case "echo":
				return BuildingCommandEcho(actor, command);
			case "characteristic":
			case "characteristics":
			case "ch":
			case "chars":
				return BuildingCommandCharacteristic(actor, command);
		}

		return base.BuildingCommand(actor, new StringStack($"\"{command.Last}\" {command.RemainingArgument}"), phase);
	}

	private bool BuildingCommandMaterial(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which solid material should this action create as a commodity output?");
			return false;
		}

		var materialText = command.SafeRemainingArgument;
		var material = Gameworld.Materials.GetByIdOrName(materialText);
		if (material is null)
		{
			actor.OutputHandler.Send($"There is no solid material identified by {materialText.ColourCommand()}.");
			return false;
		}

		Material = material;
		_materialId = material.Id;
		Changed = true;
		actor.OutputHandler.Send(
			$"This action will now create commodity output made of {material.Name.Colour(material.ResidueColour)}.");
		return true;
	}

	private bool BuildingCommandWeight(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("How much commodity mass should this action create?");
			return false;
		}

		if (!Gameworld.UnitManager.TryGetBaseUnits(command.SafeRemainingArgument, UnitType.Mass, actor, out var value) ||
		    value <= 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid positive mass.");
			return false;
		}

		Weight = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This action will now create {Gameworld.UnitManager.DescribeExact(Weight, UnitType.Mass, actor).ColourValue()} of commodity output.");
		return true;
	}

	private bool BuildingCommandTag(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which tag should be placed on the commodity pile? Use NONE to clear it.");
			return false;
		}

		var tagText = command.SafeRemainingArgument;
		if (tagText.EqualToAny("none", "clear", "delete", "remove"))
		{
			Tag = null;
			_tagId = 0;
			Changed = true;
			actor.OutputHandler.Send("This action will no longer place a tag on the output commodity pile.");
			return true;
		}

		var tags = Gameworld.Tags.FindMatchingTags(tagText);
		if (tags.Count == 0)
		{
			actor.OutputHandler.Send($"There is no tag identified by {tagText.ColourCommand()}.");
			return false;
		}

		if (tags.Count > 1)
		{
			actor.OutputHandler.Send(
				$"The tag text {tagText.ColourCommand()} is ambiguous. It could mean {tags.Select(x => x.FullName.ColourName()).ListToString()}.");
			return false;
		}

		Tag = tags[0];
		_tagId = Tag.Id;
		Changed = true;
		actor.OutputHandler.Send($"This action will now tag its output commodity pile as {Tag.FullName.ColourName()}.");
		return true;
	}

	private bool BuildingCommandIndirect(ICharacter actor)
	{
		UseIndirectDescription = !UseIndirectDescription;
		Changed = true;
		actor.OutputHandler.Send(
			$"This action will {(UseIndirectDescription ? "now" : "no longer")} use indirect quantity descriptions for its output pile.");
		return true;
	}

	private bool BuildingCommandEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What echo should be sent to the project location when the output is created? Use NONE to clear it.");
			return false;
		}

		var echoText = command.SafeRemainingArgument;
		if (echoText.EqualToAny("none", "clear", "delete", "remove"))
		{
			Echo = null;
			Changed = true;
			actor.OutputHandler.Send("This action will now use the generic commodity output echo.");
			return true;
		}

		Echo = echoText.SubstituteANSIColour().ProperSentences().Fullstop();
		Changed = true;
		actor.OutputHandler.Send($"This action will now echo: {Echo}");
		return true;
	}

	private bool BuildingCommandCharacteristic(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which fixed commodity characteristic should this action set? Use <definition> <value> or <definition> REMOVE.");
			return false;
		}

		var definitionText = command.PopSpeech();
		var definition = Gameworld.Characteristics.GetByIdOrName(definitionText);
		if (definition is null)
		{
			actor.OutputHandler.Send(
				$"There is no characteristic definition identified by {definitionText.ColourCommand()}.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Do you want to specify a characteristic value or REMOVE this output characteristic?");
			return false;
		}

		var valueText = command.SafeRemainingArgument;
		if (valueText.EqualToAny("remove", "delete", "clear"))
		{
			if (!_characteristics.Remove(definition))
			{
				actor.OutputHandler.Send(
					$"This action does not currently set a {definition.Name.ColourName()} commodity characteristic.");
				return false;
			}

			Changed = true;
			actor.OutputHandler.Send(
				$"This action will no longer set a {definition.Name.ColourName()} commodity characteristic.");
			return true;
		}

		var value = CommodityCharacteristicRequirement.GetCharacteristicValue(Gameworld, valueText);
		if (value is null || !definition.IsValue(value))
		{
			actor.OutputHandler.Send(
				$"There is no {definition.Name.ColourName()} characteristic value identified by {valueText.ColourCommand()}.");
			return false;
		}

		_characteristics[definition] = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This action will now set {definition.Name.ColourName()} to {value.GetValue.ColourValue()} on the output commodity pile.");
		return true;
	}

	private static long GetLongElement(XElement root, string name)
	{
		return long.TryParse(root.Element(name)?.Value, NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var value)
			? value
			: 0L;
	}

	private static long GetLongAttribute(XElement root, string name)
	{
		return long.TryParse(root.Attribute(name)?.Value, NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture,
			out var value)
			? value
			: 0L;
	}

	private static double GetDoubleElement(XElement root, string name, double defaultValue)
	{
		return double.TryParse(root.Element(name)?.Value, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var value)
			? value
			: defaultValue;
	}

	private static bool GetBoolElement(XElement root, string name, bool defaultValue)
	{
		return bool.TryParse(root.Element(name)?.Value, out var value) ? value : defaultValue;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;

namespace MudSharp.Form.Material;

public abstract class Material : SaveableItem, IMaterial
{
	protected Material(MudSharp.Models.Material material, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = material.Id;
		_name = material.Name;
		MaterialDescription = material.MaterialDescription;
		Density = material.Density;
		Organic = material.Organic;
		BehaviourType = (MaterialBehaviourType)(material.BehaviourType ?? 0);
		ThermalConductivity = material.ThermalConductivity;
		ElectricalConductivity = material.ElectricalConductivity;
		SpecificHeatCapacity = material.SpecificHeatCapacity;

		foreach (var tag in material.MaterialsTags)
		{
			_tags.Add(Gameworld.Tags.Get(tag.TagId));
		}
	}

	protected Material(MudSharp.Models.Liquid material, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = material.Id;
		_name = material.Name;
		MaterialDescription = material.Description;
		Density = material.Density;
		Organic = material.Organic;
		BehaviourType = MaterialBehaviourType.Liquid;
		ThermalConductivity = material.ThermalConductivity;
		ElectricalConductivity = material.ElectricalConductivity;
		SpecificHeatCapacity = material.SpecificHeatCapacity;

		foreach (var tag in material.LiquidsTags)
		{
			_tags.Add(Gameworld.Tags.Get(tag.TagId));
		}
	}

	protected Material(MudSharp.Models.Gas material, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = material.Id;
		_name = material.Name;
		MaterialDescription = material.Description;
		Density = material.Density;
		Organic = material.Organic;
		BehaviourType = MaterialBehaviourType.Gas;
		ThermalConductivity = material.ThermalConductivity;
		ElectricalConductivity = material.ElectricalConductivity;
		SpecificHeatCapacity = material.SpecificHeatCapacity;

		foreach (var tag in material.GasesTags)
		{
			_tags.Add(Gameworld.Tags.Get(tag.TagId));
		}
	}

	protected Material(Material rhs, string newName, MaterialBehaviourType behaviourType)
	{
		_name = newName;
		BehaviourType = behaviourType;
		Gameworld = rhs.Gameworld;
		MaterialDescription = rhs.MaterialDescription;
		ElectricalConductivity = rhs.ElectricalConductivity;
		ThermalConductivity = rhs.ThermalConductivity;
		Organic = rhs.Organic;
		Density = rhs.Density;
		SpecificHeatCapacity = rhs.SpecificHeatCapacity;
		_tags.AddRange(rhs._tags);
	}

	protected Material(string name, MaterialBehaviourType behaviour, IFuturemud gameworld)
	{
		_name = name;
		BehaviourType = behaviour;
		Gameworld = gameworld;
		MaterialDescription = name;
		ElectricalConductivity = 0.0001;
		ThermalConductivity = 0.14;
		Organic = false;
		Density = 500;
		SpecificHeatCapacity = 420;
		switch (behaviour)
		{
			case MaterialBehaviourType.Metal:
				ElectricalConductivity = 15000000;
				ThermalConductivity = 19;
				Density = 8000;
				break;
			case MaterialBehaviourType.Stone:
				Density = 3600;
				break;
			case MaterialBehaviourType.Ore:
				Density = 4000;
				break;
			case MaterialBehaviourType.Wood:
				Organic = true;
				break;
			case MaterialBehaviourType.Leather:
				Organic = true;
				break;
			case MaterialBehaviourType.Liquid:
				Density = 1000;
				break;
			case MaterialBehaviourType.Gas:
				Density = 1;
				break;
			case MaterialBehaviourType.Flesh:
				Organic = true;
				break;
			case MaterialBehaviourType.Muscle:
				Organic = true;
				break;
			case MaterialBehaviourType.Bone:
				Organic = true;
				break;
			case MaterialBehaviourType.Shell:
				Organic = true;
				break;
			case MaterialBehaviourType.Mana:
				break;
			case MaterialBehaviourType.Spirit:
				break;
			case MaterialBehaviourType.Energy:
				break;
			case MaterialBehaviourType.Fabric:
				break;
			case MaterialBehaviourType.Hair:
				Organic = true;
				break;
			case MaterialBehaviourType.Ceramic:
				Density = 2000;
				break;
			case MaterialBehaviourType.Food:
				Organic = true;
				break;
			case MaterialBehaviourType.Plant:
				Organic = true;
				break;
			case MaterialBehaviourType.Plastic:
				break;
			case MaterialBehaviourType.Blood:
				Organic = true;
				break;
			case MaterialBehaviourType.Feces:
				Organic = true;
				break;
			case MaterialBehaviourType.Feather:
				Organic = true;
				break;
			case MaterialBehaviourType.Meat:
				Organic = true;
				break;
			case MaterialBehaviourType.Remains:
				Organic = true;
				break;
			case MaterialBehaviourType.Soap:
				break;
			case MaterialBehaviourType.Wax:
				break;
			case MaterialBehaviourType.Powder:
				Density = 2000;
				break;
			case MaterialBehaviourType.Soil:
				Density = 2000;
				break;
		}
	}

	protected abstract string MaterialNoun { get; }

	#region IMaterial Members

	public virtual void FinaliseLoad(MudSharp.Models.Material material)
	{
		// Do nothing
	}

	public double Density { get; private set; }

	public bool Organic { get; private set; }

	public abstract MaterialType MaterialType { get; }

	public MaterialBehaviourType BehaviourType { get; set; }

	public double ThermalConductivity { get; private set; }

	public double ElectricalConductivity { get; private set; }

	public double SpecificHeatCapacity { get; private set; }

	public string MaterialDescription { get; private set; }

	#endregion

	#region IHaveTags

	private readonly List<ITag> _tags = new();
	public IEnumerable<ITag> Tags => _tags;

	public bool AddTag(ITag tag)
	{
		_tags.Add(tag);
		Changed = true;
		return true;
	}

	public bool RemoveTag(ITag tag)
	{
		_tags.Remove(tag);
		Changed = true;
		return true;
	}

	public bool IsA(ITag tag)
	{
		return tag == null || Tags.Any(x => x.IsA(tag));
	}

	#endregion

	#region IFutureProgVariable Members

	public abstract FutureProgVariableTypes Type { get; }
	public object GetObject => this;

	public virtual IFutureProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "name":
				return new TextVariable(Name);
			case "id":
				return new NumberVariable(Id);
			case "density":
				return new NumberVariable(Density);
			case "organic":
				return new BooleanVariable(Organic);
			case "tags":
				return new CollectionVariable(Tags.Select(x => x.Name).ToList(), FutureProgVariableTypes.Text);
			case "description":
				return new TextVariable(MaterialDescription);
		}

		throw new ApplicationException("Invalid property requested in Material.GetProperty");
	}

	protected static IReadOnlyDictionary<string, FutureProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, FutureProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", FutureProgVariableTypes.Text },
			{ "id", FutureProgVariableTypes.Number },
			{ "density", FutureProgVariableTypes.Number },
			{ "organic", FutureProgVariableTypes.Boolean },
			{ "tags", FutureProgVariableTypes.Collection | FutureProgVariableTypes.Text },
			{ "description", FutureProgVariableTypes.Text }
		};
	}

	protected static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "name", "The name of the material" },
			{ "id", "The Id of the material" },
			{ "density", "The density of the material in kg/m3" },
			{ "organic", "Whether this is an organic material" },
			{ "tags", "A collection of tags for this material" },
			{ "description", "The material description for this material" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		FutureProgVariable.RegisterDotReferenceCompileInfo(FutureProgVariableTypes.Material, DotReferenceHandler(),
			DotReferenceHelp());
	}

	#endregion

	#region Implementation of IEditableItem

	protected virtual string HelpText => $@"You can use the following options with this {MaterialNoun}:

	#3organic#0 - toggles counting as organic
	#3description <description>#0 - sets the description
	#3density <value>#0 - sets density in kg/m3
	#3electrical <value>#0 - sets electrical conductivity in siemens
	#3thermal <value>#0 - sets the thermal conductivity in watts/kelvin
	#3specificheat <value>#0 - sets the specific heat capacity in J/Kg.Kelvin";

	/// <inheritdoc />
	public virtual bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant().CollapseString())
		{
			case "organic":
				return BuildingCommandOrganic(actor);
			case "density":
				return BuildingCommandDensity(actor, command);
			case "electricalconductivity":
			case "electrical":
				return BuildingCommandElectricalConductivity(actor, command);
			case "thermal":
			case "thermalconductivity":
				return BuildingCommandThermalConductivity(actor, command);
			case "desc":
			case "description":
				return BuildingCommandDescription(actor, command);
			case "specific":
			case "heat":
			case "specificheat":
			case "specificheatcapacity":
			case "shc":
				return BuildingCommandSpecificHeatCapacity(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandSpecificHeatCapacity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid positive number.");
			return false;
		}

		SpecificHeatCapacity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {MaterialNoun} now has a specific heat capacity of {value.ToString("N3", actor).ColourValue()} Joules per Kg Kelvin.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What do you want to set the description to?");
			return false;
		}

		MaterialDescription = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send(
			$"You change the description of this {MaterialNoun} to {MaterialDescription.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandThermalConductivity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid positive number.");
			return false;
		}

		ThermalConductivity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {MaterialNoun} now has a thermal conductivity of {value.ToString("N3", actor).ColourValue()} Watts per Kelvin.");
		return true;
	}

	private bool BuildingCommandDensity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid positive number.");
			return false;
		}

		Density = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {MaterialNoun} now has a density of {value.ToString("N3", actor).ColourValue()} Kg per Cubic Metre.");
		return true;
	}

	private bool BuildingCommandElectricalConductivity(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, out var value) || value < 0.0)
		{
			actor.OutputHandler.Send("You must enter a valid positive number.");
			return false;
		}

		ElectricalConductivity = value;
		Changed = true;
		actor.OutputHandler.Send(
			$"This {MaterialNoun} now has an electrical conductivity of {value.ToString("N3", actor).ColourValue()} Siemens.");
		return true;
	}

	private bool BuildingCommandOrganic(ICharacter actor)
	{
		Organic = !Organic;
		Changed = true;
		actor.OutputHandler.Send($"This {MaterialNoun} is {(Organic ? "now" : "no longer")} considered organic.");
		return true;
	}

	/// <inheritdoc />
	public abstract string Show(ICharacter actor);

	#endregion
}
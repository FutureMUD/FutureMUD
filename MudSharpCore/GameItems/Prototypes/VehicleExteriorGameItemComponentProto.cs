using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Vehicles;
using System;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class VehicleExteriorGameItemComponentProto : GameItemComponentProto, IVehicleExteriorPrototype
{
	public long? VehiclePrototypeId { get; private set; }

	public VehicleExteriorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Vehicle Exterior")
	{
		Description = "Links an exterior item to a canonical vehicle instance";
	}

	protected VehicleExteriorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Vehicle Exterior";
	public override bool PreventManualLoad => true;

	public IVehiclePrototype VehiclePrototype => VehiclePrototypeId is null
		? null
		: Gameworld.VehiclePrototypes.Get(VehiclePrototypeId.Value);

	public void ConfigureForVehiclePrototype(IVehiclePrototype prototype)
	{
		VehiclePrototypeId = prototype.Id;
		_name = $"Vehicle Exterior - {prototype.Name}";
		Description = $"Vehicle exterior projection for {prototype.Name}";
		Changed = true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("vehicle exterior", true,
			(gameworld, account) => new VehicleExteriorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vehicle", false,
			(gameworld, account) => new VehicleExteriorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Vehicle Exterior",
			(proto, gameworld) => new VehicleExteriorGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"VehicleExterior",
			$"Makes an item the {"[exterior projection]".Colour(Telnet.BoldGreen)} of a canonical vehicle.",
			BuildingHelpText
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		VehiclePrototypeId = long.TryParse(root.Element("VehiclePrototypeId")?.Value, out var id) ? id : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VehiclePrototypeId", VehiclePrototypeId)
		).ToString();
	}

	public override bool CanSubmit()
	{
		return VehiclePrototype is not null;
	}

	public override string WhyCannotSubmit()
	{
		return "You must link this component to a vehicle prototype.";
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"Vehicle Exterior Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Linked Vehicle Prototype: {(VehiclePrototype is null ? "None".ColourError() : $"{VehiclePrototype.Name.ColourName()} (#{VehiclePrototype.Id.ToString("N0", actor)}r{VehiclePrototype.RevisionNumber.ToString("N0", actor)})")}

This component marks an item as the exterior item projection for a canonical vehicle. It prevents manual item loading; create vehicles through the vehicle factory or builder/admin vehicle commands.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new VehicleExteriorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VehicleExteriorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new VehicleExteriorGameItemComponentProto(proto, gameworld));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "vehicle":
			case "prototype":
			case "proto":
				return BuildingCommandVehicle(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandVehicle(ICharacter actor, StringStack command)
	{
		var proto = actor.Gameworld.VehiclePrototypes.GetByIdOrName(command.SafeRemainingArgument);
		if (proto is null)
		{
			actor.OutputHandler.Send("There is no such vehicle prototype.");
			return false;
		}

		ConfigureForVehiclePrototype(proto);
		actor.OutputHandler.Send($"This component now links to vehicle prototype {proto.Name.ColourName()} (#{proto.Id.ToString("N0", actor)}r{proto.RevisionNumber.ToString("N0", actor)}).");
		return true;
	}

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3vehicle <vehicle proto>#0 - links this exterior item component to a vehicle prototype";
}

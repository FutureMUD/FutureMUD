using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Vehicles;
using System.Linq;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class VehicleCargoSpaceGameItemComponentProto : GameItemComponentProto, IVehicleCargoSpaceItemPrototype
{
	public long? VehiclePrototypeId { get; private set; }
	public long? CargoSpacePrototypeId { get; private set; }

	public VehicleCargoSpaceGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Vehicle Cargo Space")
	{
		Description = "Projects a vehicle cargo space as a targetable item";
	}

	protected VehicleCargoSpaceGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Vehicle Cargo Space";
	public override bool PreventManualLoad => true;

	public IVehiclePrototype VehiclePrototype => VehiclePrototypeId is null
		? null
		: Gameworld.VehiclePrototypes.Get(VehiclePrototypeId.Value);

	public IVehicleCargoSpacePrototype CargoSpacePrototype => VehiclePrototype is null || CargoSpacePrototypeId is null
		? null
		: VehiclePrototype.CargoSpaces.FirstOrDefault(x => x.Id == CargoSpacePrototypeId.Value);

	public void ConfigureForCargoSpace(IVehiclePrototype vehicle, IVehicleCargoSpacePrototype cargoSpace)
	{
		VehiclePrototypeId = vehicle.Id;
		CargoSpacePrototypeId = cargoSpace.Id;
		_name = $"Vehicle Cargo - {vehicle.Name} - {cargoSpace.Name}";
		Description = $"Vehicle cargo projection for {vehicle.Name}: {cargoSpace.Name}";
		Changed = true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("vehicle cargo space", true,
			(gameworld, account) => new VehicleCargoSpaceGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vehiclecargo", false,
			(gameworld, account) => new VehicleCargoSpaceGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Vehicle Cargo Space",
			(proto, gameworld) => new VehicleCargoSpaceGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"VehicleCargoSpace",
			$"Makes an item the {"[cargo space projection]".Colour(Telnet.BoldGreen)} of a canonical vehicle.",
			BuildingHelpText
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		VehiclePrototypeId = long.TryParse(root.Element("VehiclePrototypeId")?.Value, out var vehicleId) ? vehicleId : null;
		CargoSpacePrototypeId = long.TryParse(root.Element("CargoSpacePrototypeId")?.Value, out var cargoId) ? cargoId : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VehiclePrototypeId", VehiclePrototypeId),
			new XElement("CargoSpacePrototypeId", CargoSpacePrototypeId)
		).ToString();
	}

	public override bool CanSubmit()
	{
		return CargoSpacePrototype is not null;
	}

	public override string WhyCannotSubmit()
	{
		return "You must link this component to a vehicle cargo space prototype.";
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"Vehicle Cargo Space Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Linked Vehicle Prototype: {(VehiclePrototype is null ? "None".ColourError() : $"{VehiclePrototype.Name.ColourName()} (#{VehiclePrototype.Id.ToString("N0", actor)}r{VehiclePrototype.RevisionNumber.ToString("N0", actor)})")}
Linked Cargo Space: {(CargoSpacePrototype is null ? "None".ColourError() : $"{CargoSpacePrototype.Name.ColourName()} (#{CargoSpacePrototype.Id.ToString("N0", actor)})")}

This component marks an item as a projected vehicle cargo space. Add a normal container component to the same item prototype to hold cargo.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new VehicleCargoSpaceGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VehicleCargoSpaceGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new VehicleCargoSpaceGameItemComponentProto(proto, gameworld));
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
		var vehicle = actor.Gameworld.VehiclePrototypes.GetByIdOrName(command.PopSpeech());
		if (vehicle is null)
		{
			actor.OutputHandler.Send("There is no such vehicle prototype.");
			return false;
		}

		if (!long.TryParse(command.PopSpeech(), out var cargoId))
		{
			actor.OutputHandler.Send("Which cargo space ID do you want to link?");
			return false;
		}

		var cargo = vehicle.CargoSpaces.FirstOrDefault(x => x.Id == cargoId);
		if (cargo is null)
		{
			actor.OutputHandler.Send("There is no such cargo space on that vehicle prototype.");
			return false;
		}

		ConfigureForCargoSpace(vehicle, cargo);
		actor.OutputHandler.Send($"This component now links to {cargo.Name.ColourName()} on {vehicle.Name.ColourName()}.");
		return true;
	}

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3vehicle <vehicle proto> <cargo id>#0 - links this component to a vehicle cargo space prototype";
}

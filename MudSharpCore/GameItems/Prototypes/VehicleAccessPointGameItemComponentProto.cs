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

public class VehicleAccessPointGameItemComponentProto : GameItemComponentProto, IVehicleAccessPointItemPrototype
{
	public long? VehiclePrototypeId { get; private set; }
	public long? AccessPointPrototypeId { get; private set; }

	public VehicleAccessPointGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Vehicle Access Point")
	{
		Description = "Projects a vehicle door, hatch or other access point as a targetable item";
	}

	protected VehicleAccessPointGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Vehicle Access Point";
	public override bool PreventManualLoad => true;

	public IVehiclePrototype VehiclePrototype => VehiclePrototypeId is null
		? null
		: Gameworld.VehiclePrototypes.Get(VehiclePrototypeId.Value);

	public IVehicleAccessPointPrototype AccessPointPrototype => VehiclePrototype is null || AccessPointPrototypeId is null
		? null
		: VehiclePrototype.AccessPoints.FirstOrDefault(x => x.Id == AccessPointPrototypeId.Value);

	public void ConfigureForAccessPoint(IVehiclePrototype vehicle, IVehicleAccessPointPrototype accessPoint)
	{
		VehiclePrototypeId = vehicle.Id;
		AccessPointPrototypeId = accessPoint.Id;
		_name = $"Vehicle Access - {vehicle.Name} - {accessPoint.Name}";
		Description = $"Vehicle access point projection for {vehicle.Name}: {accessPoint.Name}";
		Changed = true;
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("vehicle access point", true,
			(gameworld, account) => new VehicleAccessPointGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vehicleaccess", false,
			(gameworld, account) => new VehicleAccessPointGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Vehicle Access Point",
			(proto, gameworld) => new VehicleAccessPointGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"VehicleAccessPoint",
			$"Makes an item the {"[access point projection]".Colour(Telnet.BoldGreen)} of a canonical vehicle.",
			BuildingHelpText
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		VehiclePrototypeId = long.TryParse(root.Element("VehiclePrototypeId")?.Value, out var vehicleId) ? vehicleId : null;
		AccessPointPrototypeId = long.TryParse(root.Element("AccessPointPrototypeId")?.Value, out var accessId) ? accessId : null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("VehiclePrototypeId", VehiclePrototypeId),
			new XElement("AccessPointPrototypeId", AccessPointPrototypeId)
		).ToString();
	}

	public override bool CanSubmit()
	{
		return AccessPointPrototype is not null;
	}

	public override string WhyCannotSubmit()
	{
		return "You must link this component to a vehicle access point prototype.";
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"Vehicle Access Point Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Linked Vehicle Prototype: {(VehiclePrototype is null ? "None".ColourError() : $"{VehiclePrototype.Name.ColourName()} (#{VehiclePrototype.Id.ToString("N0", actor)}r{VehiclePrototype.RevisionNumber.ToString("N0", actor)})")}
Linked Access Point: {(AccessPointPrototype is null ? "None".ColourError() : $"{AccessPointPrototype.Name.ColourName()} (#{AccessPointPrototype.Id.ToString("N0", actor)})")}

This component marks an item as a projected vehicle access point. It prevents manual item loading; create vehicle projections through the vehicle factory.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new VehicleAccessPointGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VehicleAccessPointGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new VehicleAccessPointGameItemComponentProto(proto, gameworld));
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

		if (!long.TryParse(command.PopSpeech(), out var accessId))
		{
			actor.OutputHandler.Send("Which access point ID do you want to link?");
			return false;
		}

		var access = vehicle.AccessPoints.FirstOrDefault(x => x.Id == accessId);
		if (access is null)
		{
			actor.OutputHandler.Send("There is no such access point on that vehicle prototype.");
			return false;
		}

		ConfigureForAccessPoint(vehicle, access);
		actor.OutputHandler.Send($"This component now links to {access.Name.ColourName()} on {vehicle.Name.ColourName()}.");
		return true;
	}

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3vehicle <vehicle proto> <access id>#0 - links this component to a vehicle access point prototype";
}

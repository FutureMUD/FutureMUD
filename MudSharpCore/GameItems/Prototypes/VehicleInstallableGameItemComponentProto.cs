using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class VehicleInstallableGameItemComponentProto : GameItemComponentProto, IVehicleInstallablePrototype
{
	public string MountType { get; private set; } = "generic";
	public string Role { get; private set; } = string.Empty;

	public VehicleInstallableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Vehicle Installable")
	{
		Description = "Makes an item installable in a vehicle installation point";
	}

	protected VehicleInstallableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Vehicle Installable";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("vehicle installable", true,
			(gameworld, account) => new VehicleInstallableGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vehicleinstallable", false,
			(gameworld, account) => new VehicleInstallableGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Vehicle Installable",
			(proto, gameworld) => new VehicleInstallableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"VehicleInstallable",
			$"Makes an item an {"[installable vehicle module]".Colour(Telnet.BoldGreen)}.",
			BuildingHelpText
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		MountType = root.Element("MountType")?.Value ?? "generic";
		Role = root.Element("Role")?.Value ?? string.Empty;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("MountType", MountType),
			new XElement("Role", Role)
		).ToString();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"Vehicle Installable Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Mount Type: {MountType.ColourCommand()}
Role: {(string.IsNullOrWhiteSpace(Role) ? "none".ColourError() : Role.ColourCommand())}

This item can be installed in vehicle installation points with a matching mount type.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new VehicleInstallableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VehicleInstallableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new VehicleInstallableGameItemComponentProto(proto, gameworld));
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "mount":
			case "mounttype":
				return BuildingCommandMount(actor, command);
			case "role":
				return BuildingCommandRole(actor, command);
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private bool BuildingCommandMount(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What mount type should this module require?");
			return false;
		}

		MountType = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send($"This module now uses the {MountType.ColourCommand()} vehicle mount type.");
		return true;
	}

	private bool BuildingCommandRole(ICharacter actor, StringStack command)
	{
		Role = command.SafeRemainingArgument.ToLowerInvariant();
		Changed = true;
		actor.OutputHandler.Send(string.IsNullOrWhiteSpace(Role)
			? "This module no longer has a vehicle role."
			: $"This module now fulfils the {Role.ColourCommand()} vehicle role.");
		return true;
	}

	private const string BuildingHelpText =
		@"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3mount <type>#0 - sets the vehicle installation mount type
	#3role <role>#0 - sets the optional vehicle role this module fulfils";
}

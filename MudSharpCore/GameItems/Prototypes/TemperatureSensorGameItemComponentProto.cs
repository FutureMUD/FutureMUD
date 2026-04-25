#nullable enable

using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class TemperatureSensorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#6Notes:#0

	This powered sensor emits the current ambient temperature in degrees Celsius on the default #3signal#0 endpoint while switched on and powered.";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected TemperatureSensorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Temperature Sensor")
	{
		UseMountHostPowerSource = true;
		Wattage = 25.0;
	}

	protected TemperatureSensorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Temperature Sensor";

	protected override string ComponentDescriptionOLCByline => "This item is a powered temperature sensor";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return "Signal Output: current ambient temperature in degrees Celsius";
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("temperaturesensor", true,
			(gameworld, account) => new TemperatureSensorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("temperature sensor", false,
			(gameworld, account) => new TemperatureSensorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Temperature Sensor",
			(proto, gameworld) => new TemperatureSensorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"TemperatureSensor",
			$"A {"[powered]".Colour(Telnet.Magenta)} {SignalComponentUtilities.SignalGeneratorTag} that emits current ambient temperature in Celsius",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new TemperatureSensorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new TemperatureSensorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new TemperatureSensorGameItemComponentProto(proto, gameworld));
	}
}

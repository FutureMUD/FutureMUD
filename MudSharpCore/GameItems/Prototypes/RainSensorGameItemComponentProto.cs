#nullable enable

using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class RainSensorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto
{
	private const string SpecificBuildingHelpText = @"
	#6Notes:#0

	This powered sensor emits current rainfall intensity on the default #3signal#0 endpoint while switched on and powered.
	The signal mapping is: 0 = no rain, 1 = light rain, 2 = rain or sleet, 3 = heavy rain, 4 = torrential rain.
	Indoor sheltered locations currently read as 0.";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected RainSensorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Rain Sensor")
	{
		UseMountHostPowerSource = true;
		Wattage = 25.0;
	}

	protected RainSensorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Rain Sensor";

	protected override string ComponentDescriptionOLCByline => "This item is a powered rain sensor";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return "Signal Output: current rainfall intensity (0 none, 1 light rain, 2 rain or sleet, 3 heavy rain, 4 torrential rain)";
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("rainsensor", true,
			(gameworld, account) => new RainSensorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("rain sensor", false,
			(gameworld, account) => new RainSensorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Rain Sensor",
			(proto, gameworld) => new RainSensorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"RainSensor",
			$"A {"[powered]".Colour(Telnet.Magenta)} {SignalComponentUtilities.SignalGeneratorTag} that emits current rainfall intensity",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new RainSensorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new RainSensorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new RainSensorGameItemComponentProto(proto, gameworld));
	}
}

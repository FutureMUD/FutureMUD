#nullable enable

using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class LightSensorGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, ISignalSourceComponentPrototype
{
	private const string SpecificBuildingHelpText = @"
	#6Notes:#0

	This powered sensor emits the current ambient illumination of its location in lux on the default #3signal#0 endpoint while switched on and powered.";

	private static readonly string CombinedBuildingHelpText =
		$@"{PoweredMachineBaseGameItemComponentProto.BuildingHelpText}{SpecificBuildingHelpText}";

	protected LightSensorGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Light Sensor")
	{
		UseMountHostPowerSource = true;
		Wattage = 25.0;
	}

	protected LightSensorGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Light Sensor";

	protected override string ComponentDescriptionOLCByline => "This item is a powered light sensor";

	protected override string ComponentDescriptionOLCAddendum(ICharacter actor)
	{
		return "Signal Output: current ambient illumination in lux";
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		return root;
	}

	public override string ShowBuildingHelp => @$"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("lightsensor", true,
			(gameworld, account) => new LightSensorGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("light sensor", false,
			(gameworld, account) => new LightSensorGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Light Sensor",
			(proto, gameworld) => new LightSensorGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"LightSensor",
			$"A {"[powered]".Colour(Telnet.Magenta)} {SignalComponentUtilities.SignalGeneratorTag} that emits the current ambient illumination in lux",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new LightSensorGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new LightSensorGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LightSensorGameItemComponentProto(proto, gameworld));
	}
}

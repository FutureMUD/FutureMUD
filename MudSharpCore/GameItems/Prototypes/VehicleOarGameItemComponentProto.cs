#nullable enable

using MudSharp.Accounts;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class VehicleOarGameItemComponentProto : GameItemComponentProto, IVehicleOarPrototype
{
	public VehicleOarGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Vehicle Oar")
	{
		Description = "Makes an item usable as an oar for a rowed surface-water vehicle";
	}

	protected VehicleOarGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public double EfficiencyMultiplier { get; private set; } = 1.0;
	public override string TypeDescription => "Vehicle Oar";
	public override string ShowBuildingHelp => BuildingHelpText;

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("vehicle oar", true,
			(gameworld, account) => new VehicleOarGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("vehicleoar", false,
			(gameworld, account) => new VehicleOarGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("oar", false,
			(gameworld, account) => new VehicleOarGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Vehicle Oar",
			(proto, gameworld) => new VehicleOarGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo("VehicleOar",
			$"Makes an item an {"[oar]".Colour(Telnet.BoldGreen)} for rowed surface-water vehicles.",
			BuildingHelpText);
	}

	protected override void LoadFromXml(XElement root)
	{
		EfficiencyMultiplier = double.TryParse(root.Element("EfficiencyMultiplier")?.Value, out var value) &&
		                       double.IsFinite(value) && value > 0.0
			? value
			: 1.0;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("EfficiencyMultiplier", EfficiencyMultiplier)
		).ToString();
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return $@"{"Vehicle Oar Item Component".Colour(Telnet.Cyan)} (#{Id.ToString("N0", actor)}r{RevisionNumber.ToString("N0", actor)}, {Name})

Efficiency Multiplier: {EfficiencyMultiplier.ToString("N2", actor).ColourValue()}

An able occupant holding or wielding this item can contribute to a rowed vehicle from a propulsion slot.";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new VehicleOarGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new VehicleOarGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new VehicleOarGameItemComponentProto(proto, gameworld));
	}

	public override bool CanSubmit()
	{
		return double.IsFinite(EfficiencyMultiplier) && EfficiencyMultiplier > 0.0 && base.CanSubmit();
	}

	public override string WhyCannotSubmit()
	{
		return !double.IsFinite(EfficiencyMultiplier) || EfficiencyMultiplier <= 0.0
			? "The oar efficiency multiplier must be positive and finite."
			: base.WhyCannotSubmit();
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "efficiency":
			case "multiplier":
				if (!double.TryParse(command.SafeRemainingArgument, out var value) || !double.IsFinite(value) ||
			    value <= 0.0)
				{
					actor.OutputHandler.Send("You must specify a positive, finite efficiency multiplier.");
					return false;
				}

				EfficiencyMultiplier = value;
				Changed = true;
				actor.OutputHandler.Send(
					$"This oar now has an efficiency multiplier of {value.ToString("N2", actor).ColourValue()}.");
				return true;
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	private const string BuildingHelpText = @"You can use the following options with this component:

	#3name <name>#0 - sets the component name
	#3desc <description>#0 - sets the component description
	#3efficiency <multiplier>#0 - sets the oar's positive rowing-efficiency multiplier";
}

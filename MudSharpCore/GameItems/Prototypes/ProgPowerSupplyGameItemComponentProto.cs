using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class ProgPowerSupplyGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "ProgPowerSupply";

	public double Wattage { get; set; }

	#region Constructors

	protected ProgPowerSupplyGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ProgPowerSupply")
	{
		Wattage = 20;
	}

	protected ProgPowerSupplyGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		var element = root.Element("Wattage");
		if (element != null)
		{
			Wattage = double.Parse(element.Value);
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wattage", Wattage)
		).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ProgPowerSupplyGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ProgPowerSupplyGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ProgPowerSupply".ToLowerInvariant(), true,
			(gameworld, account) => new ProgPowerSupplyGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ProgPowerSupply",
			(proto, gameworld) => new ProgPowerSupplyGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"ProgPowerSupply",
			$"This item {"[provides power]".Colour(Telnet.BoldMagenta)} with no fuel source so long as switched on by a prog",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ProgPowerSupplyGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <#> - sets the watts provided by this component";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "wattage":
			case "watts":
			case "watt":
			case "power":
				return BuildingCommand_Wattage(actor, command);
		}

		return base.BuildingCommand(actor, command);
	}

	private bool BuildingCommand_Wattage(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.Send("How many watts should this power supply draw when in use?");
			return false;
		}

		if (!double.TryParse(command.Pop(), out var value))
		{
			actor.Send("How many watts should this power supply draw when in use?");
			return false;
		}

		if (value < 0)
		{
			actor.Send("You must enter a positive number of watts for this power supply draw when in use.");
			return false;
		}

		Wattage = value;
		Changed = true;

		actor.Send("This power supply will now draw {0:N2} watts when in use.", Wattage);
		return true;
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item provides {4:N2} watts of power when switched on by a prog.",
			"ProgPowerSupply Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Wattage
		);
	}
}
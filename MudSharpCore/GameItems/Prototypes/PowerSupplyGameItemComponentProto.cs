using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;

namespace MudSharp.GameItems.Prototypes;

public class PowerSupplyGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "PowerSupply";

	public double Wattage { get; set; }

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Wattage", Wattage)
		).ToString();
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item provides {4:N2} watts of power when plugged into a power source.",
			"PowerSupply Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			Wattage
		);
	}

	#region Constructors

	protected PowerSupplyGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "PowerSupply")
	{
		Wattage = 20;
	}

	protected PowerSupplyGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
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

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PowerSupplyGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PowerSupplyGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("PowerSupply".ToLowerInvariant(), true,
			(gameworld, account) => new PowerSupplyGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("PowerSupply",
			(proto, gameworld) => new PowerSupplyGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"PowerSupply",
			$"This component {"[supplies power]".Colour(Telnet.BoldMagenta)} to the item via other {"[connectable]".Colour(Telnet.BoldBlue)} components",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new PowerSupplyGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component\n\twatts <#> - the wattage drawn by the device";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "wattage":
			case "watts":
			case "watt":
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
}
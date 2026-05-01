#nullable enable

using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using System.Xml.Linq;

namespace MudSharp.GameItems.Prototypes;

public class ComputerTerminalGameItemComponentProto : PoweredMachineBaseGameItemComponentProto, IConnectablePrototype
{
	private const string SpecificBuildingHelpText = @"

#6Notes:#0

	Computer terminals connect to a computer host and let players use the #3programming terminal#0 command surface against that host or its mounted storage.";

	private static readonly string CombinedBuildingHelpText =
		$@"{BuildingHelpText}{SpecificBuildingHelpText}";

	public ComputerTerminalGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Computer Terminal")
	{
	}

	protected ComputerTerminalGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public override string TypeDescription => "Computer Terminal";
	protected override string ComponentDescriptionOLCByline => "This item is a powered terminal for a computer host";
	protected override string ComponentDescriptionOLCAddendum(ICharacter actor) => string.Empty;

	protected override void LoadFromXml(XElement root)
	{
		base.LoadFromXml(root);
	}

	protected override XElement SaveSubtypeToXml(XElement root)
	{
		return root;
	}

	public override string ShowBuildingHelp => $@"{base.ShowBuildingHelp}{SpecificBuildingHelpText}";

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("computerterminal", true,
			(gameworld, account) => new ComputerTerminalGameItemComponentProto(gameworld, account));
		manager.AddBuilderLoader("computer terminal", false,
			(gameworld, account) => new ComputerTerminalGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("Computer Terminal",
			(proto, gameworld) => new ComputerTerminalGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Computer Terminal",
			$"Makes an item a {"[computer terminal]".Colour(Telnet.BoldGreen)} {"[powered]".Colour(Telnet.BoldGreen)} user session endpoint for a computer host",
			CombinedBuildingHelpText);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new ComputerTerminalGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ComputerTerminalGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ComputerTerminalGameItemComponentProto(proto, gameworld));
	}
}

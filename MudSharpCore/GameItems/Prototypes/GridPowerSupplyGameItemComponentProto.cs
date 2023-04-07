using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class GridPowerSupplyGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "GridPowerSupply";

	#region Constructors

	protected GridPowerSupplyGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "GridPowerSupply")
	{
	}

	protected GridPowerSupplyGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		// TODO
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new[]
		{
			new XElement("Example")
		}).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new GridPowerSupplyGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new GridPowerSupplyGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("GridPowerSupply".ToLowerInvariant(), true,
			(gameworld, account) => new GridPowerSupplyGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("GridPowerSupply",
			(proto, gameworld) => new GridPowerSupplyGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"GridPowerSupply",
			$"This item {"[provides power]".Colour(Telnet.BoldMagenta)} directly from an {"[electric grid]".Colour(Telnet.BoldOrange)}. Can combine with a {"[connectable]".Colour(Telnet.BoldBlue)} to create a power point",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new GridPowerSupplyGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#endregion

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item supplies power directly from the grid.",
			"GridPowerSupply Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}
}
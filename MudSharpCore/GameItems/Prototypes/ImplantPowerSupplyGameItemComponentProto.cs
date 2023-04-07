using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class ImplantPowerSupplyGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "ImplantPowerSupply";

	#region Constructors

	protected ImplantPowerSupplyGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "ImplantPowerSupply")
	{
	}

	protected ImplantPowerSupplyGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto,
		IFuturemud gameworld) : base(proto, gameworld)
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
		return new XElement("Definition").ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new ImplantPowerSupplyGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ImplantPowerSupplyGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("ImplantPowerSupply".ToLowerInvariant(), true,
			(gameworld, account) => new ImplantPowerSupplyGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("ImplantPowerSupply",
			(proto, gameworld) => new ImplantPowerSupplyGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ImplantPowerSupply",
			$"Interfaces between an {"[implant]".Colour(Telnet.Pink)} and an implant power plant to {"[provide power]".Colour(Telnet.BoldMagenta)} for itself",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ImplantPowerSupplyGameItemComponentProto(proto, gameworld));
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
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item interfaces between a powered implant and an implant powerplant.",
			"ImplantPowerSupply Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}
}
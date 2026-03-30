#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class LiquidGridSupplierGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "LiquidGridSupplier";

	protected LiquidGridSupplierGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
		originator, "LiquidGridSupplier")
	{
	}

	protected LiquidGridSupplierGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	protected override void LoadFromXml(System.Xml.Linq.XElement root)
	{
	}

	protected override string SaveToXml()
	{
		return new System.Xml.Linq.XElement("Definition").ToString();
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new LiquidGridSupplierGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
	{
		return new LiquidGridSupplierGameItemComponent(component, this, parent);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddBuilderLoader("LiquidGridSupplier".ToLowerInvariant(), true,
			(gameworld, account) => new LiquidGridSupplierGameItemComponentProto(gameworld, account));
		manager.AddDatabaseLoader("LiquidGridSupplier",
			(proto, gameworld) => new LiquidGridSupplierGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"LiquidGridSupplier",
			$"Connects a sibling {"[liquid container]".Colour(Telnet.BoldBlue)} to a {"[liquid grid]".Colour(Telnet.BoldCyan)} as a supplier",
			BuildingHelpText
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new LiquidGridSupplierGameItemComponentProto(proto, gameworld));
	}

	private const string BuildingHelpText =
		"You can use the following options with this component:\n\tname <name> - sets the name of the component\n\tdesc <desc> - sets the description of the component";

	public override string ShowBuildingHelp => BuildingHelpText;

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		return base.BuildingCommand(actor, command);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item feeds a sibling liquid container into a liquid grid.",
			"LiquidGridSupplier Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}
}

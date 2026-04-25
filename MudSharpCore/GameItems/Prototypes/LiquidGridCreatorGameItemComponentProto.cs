#nullable enable
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class LiquidGridCreatorGameItemComponentProto : GameItemComponentProto
{
    public override string TypeDescription => "LiquidGridCreator";

    protected LiquidGridCreatorGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld,
        originator, "LiquidGridCreator")
    {
    }

    protected LiquidGridCreatorGameItemComponentProto(Models.GameItemComponentProto proto, IFuturemud gameworld)
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

    public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
    {
        return new LiquidGridCreatorGameItemComponent(this, parent, loader, temporary);
    }

    public override IGameItemComponent LoadComponent(Models.GameItemComponent component, IGameItem parent)
    {
        return new LiquidGridCreatorGameItemComponent(component, this, parent);
    }

    public static void RegisterComponentInitialiser(GameItemComponentManager manager)
    {
        manager.AddBuilderLoader("LiquidGridCreator".ToLowerInvariant(), true,
            (gameworld, account) => new LiquidGridCreatorGameItemComponentProto(gameworld, account));
        manager.AddDatabaseLoader("LiquidGridCreator",
            (proto, gameworld) => new LiquidGridCreatorGameItemComponentProto(proto, gameworld));
        manager.AddTypeHelpInfo(
            "LiquidGridCreator",
            $"When put in a room, creates a {"[liquid grid]".Colour(Telnet.BoldCyan)}",
            BuildingHelpText
        );
    }

    public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
    {
        return CreateNewRevision(initiator,
            (proto, gameworld) => new LiquidGridCreatorGameItemComponentProto(proto, gameworld));
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
            "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item creates a liquid grid by its presence.",
            "LiquidGridCreator Game Item Component".Colour(Telnet.Cyan),
            Id,
            RevisionNumber,
            Name
        );
    }
}

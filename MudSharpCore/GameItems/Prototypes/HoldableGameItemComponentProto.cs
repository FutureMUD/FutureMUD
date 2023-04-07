using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;

namespace MudSharp.GameItems.Prototypes;

public class HoldableGameItemComponentProto : GameItemComponentProto
{
	protected HoldableGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Holdable")
	{
	}

	protected HoldableGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		if (!gameworld.ItemComponentProtos.Any(x => x is HoldableGameItemComponentProto))
		{
			var comp = new HoldableGameItemComponentProto(gameworld, null);
			gameworld.Add(comp);
			comp.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
		}
	}

	public override bool ReadOnly => true;
	public override string TypeDescription => "Holdable";

	protected override void LoadFromXml(XElement root)
	{
		// Nothing to do
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		command.Pop();
		return base.BuildingCommand(actor, command);
	}

	public override string ShowBuildingHelp => "There are no parameters to set for a holdable.";

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item can be picked up and manipulated by players.",
			"Holdable Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name);
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddDatabaseLoader("Holdable",
			(proto, gameworld) => new HoldableGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"Holdable",
			$"Makes an item able to be picked up, moved, etc.",
			"This component should not be edited in any way."
		);
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new HoldableGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new HoldableGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		throw new NotSupportedException("Holdable components should not be edited.");
	}
}
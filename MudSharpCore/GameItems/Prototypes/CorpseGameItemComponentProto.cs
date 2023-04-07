using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Prototypes;

public class CorpseGameItemComponentProto : GameItemComponentProto
{
	protected CorpseGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Corpse")
	{
	}

	protected CorpseGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public static IGameItemProto ItemProto { get; set; }
	public override string TypeDescription => "Corpse";
	public override bool ReadOnly => true;
	public override bool PreventManualLoad => true;

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		ItemProto = gameworld.ItemProtos.Single(x => x.IsItemType<CorpseGameItemComponentProto>());
	}

	public static IGameItem CreateNewCorpse(ICharacter character, bool temporary = false)
	{
		var newItem = ItemProto.CreateNew();
		if (newItem.GetItemType<ICorpse>() is not CorpseGameItemComponent corpseItem)
		{
			throw new ApplicationException(
				"When trying to create a corpse, there was no corpse component on the corpse prototype.");
		}

		corpseItem.OriginalCharacter = character;
		corpseItem.Model = character.Race.CorpseModel;
		corpseItem.Parent.RoomLayer = character.RoomLayer;
		corpseItem.Changed = true;
		return newItem;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a corpse. It was created when a living being died.",
			"Corpse Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		// Do not register builder initialiser
		manager.AddDatabaseLoader("Corpse", (proto, gameworld) => new CorpseGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Corpse",
			$"Marks an item as a {"[system-generated]".Colour(Telnet.Green)} corpse. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
			"This component is used in auto-generated items only. It should not and cannot be used in any manually created items, nor should it be edited in any way."
		);
	}

	protected override void LoadFromXml(XElement root)
	{
		// Nothing to do
	}

	protected override string SaveToXml()
	{
		return "<Definition/>";
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CorpseGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CorpseGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		throw new NotSupportedException("Corpses should not be edited.");
	}
}
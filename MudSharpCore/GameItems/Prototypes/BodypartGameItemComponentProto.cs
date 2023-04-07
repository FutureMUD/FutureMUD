using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;

namespace MudSharp.GameItems.Prototypes;

public class BodypartGameItemComponentProto : GameItemComponentProto
{
	protected BodypartGameItemComponentProto(IFuturemud gameworld, IAccount originator)
		: base(gameworld, originator, "Bodypart")
	{
	}

	protected BodypartGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public static IGameItemProto ItemProto { get; set; }
	public override string TypeDescription => "Bodypart";
	public override bool ReadOnly => true;
	public override bool PreventManualLoad => true;

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		ItemProto = gameworld.ItemProtos.Single(x => x.IsItemType<BodypartGameItemComponentProto>());
	}

	public static IGameItem CreateNewSeveredBodypart(ICharacter character, IEnumerable<IBodypart> parts,
		IEnumerable<IGameItem> items, IEnumerable<IGameItem> implants, IEnumerable<IBone> bones,
		IEnumerable<ITattoo> tattoos, IEnumerable<IWound> wounds, bool temporary = false)
	{
		var newItem = ItemProto.CreateNew();
		if (newItem.GetItemType<ISeveredBodypart>() is not BodypartGameItemComponent severedItem)
		{
			throw new ApplicationException(
				"When trying to create a severed bodypart, there was no bodypart component on the bodypart prototype.");
		}

		severedItem.OriginalCharacterId = character.Id;
		severedItem.Parts = parts;
		severedItem.Implants = implants;
		severedItem.Bones = bones;
		severedItem.Contents = items;
		severedItem.Model = character.Race.CorpseModel;
		severedItem.Tattoos = tattoos;
		severedItem.Wounds = wounds;
		foreach (var wound in wounds)
		{
			wound.SetNewOwner(severedItem.Parent);
		}

		severedItem.Changed = true;
		return newItem;
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor,
			"{0} (#{1:N0}r{2:N0}, {3})\n\nThis item is a severed bodypart.",
			"Corpse Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		// Do not register building initialiser
		manager.AddDatabaseLoader("Bodypart",
			(proto, gameworld) => new BodypartGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Bodypart",
			$"Marks an item as a {"[system-generated]".Colour(Telnet.Green)} severed bodypart. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
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
		return new BodypartGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new BodypartGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		throw new NotSupportedException("Severed bodyparts should not be edited.");
	}
}
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
using MudSharp.Work.Crafts;

namespace MudSharp.GameItems.Prototypes;

public class ActiveCraftGameItemComponentProto : GameItemComponentProto
{
	public static IGameItemProto ItemProto { get; set; }

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		ItemProto = gameworld.ItemProtos.SingleOrDefault(x => x.IsItemType<ActiveCraftGameItemComponentProto>());
		if (ItemProto == null)
		{
			var comp = new ActiveCraftGameItemComponentProto(gameworld, null);
			gameworld.Add(comp);
			comp.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
			var proto = new GameItemProto(gameworld, null);
			gameworld.Add(proto);
			proto.AddComponent(comp);
			proto.Weight = 0;
			proto.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
			ItemProto = proto;
		}
	}

	public static ActiveCraftGameItemComponent LoadActiveCraft(ICraft craft)
	{
		var newItem = ItemProto?.CreateNew();
		var craftItem = newItem?.GetItemType<ActiveCraftGameItemComponent>();
		if (craftItem == null)
		{
			throw new ApplicationException("When trying to create an ActiveCraft item, there was no valid prototype.");
		}

		craftItem.Craft = craft;
		return craftItem;
	}

	public override string TypeDescription => "ActiveCraft";

	#region Constructors

	protected ActiveCraftGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"ActiveCraft")
	{
	}

	protected ActiveCraftGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
		base(proto, gameworld)
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
		return new ActiveCraftGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new ActiveCraftGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddDatabaseLoader("ActiveCraft",
			(proto, gameworld) => new ActiveCraftGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"ActiveCraft",
			$"Makes an item a {"[system-generated]".Colour(Telnet.Green)} item that represent crafts in progress. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
			"This component is used in auto-generated items only. It should not and cannot be used in any manually created items, nor should it be edited in any way."
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new ActiveCraftGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopSpeech().ToLowerInvariant())
		{
			default:
				return base.BuildingCommand(actor, command);
		}
	}

	#endregion

	public override bool PreventManualLoad => true;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item needs a description.",
			"ActiveCraft Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}

	#region Overrides of GameItemComponentProto

	/// <summary>
	///     If true, the specified component is read-only and cannot be changed by the user
	/// </summary>
	public override bool ReadOnly => true;

	#endregion
}
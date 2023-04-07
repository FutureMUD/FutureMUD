using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class CommodityGameItemComponentProto : GameItemComponentProto
{
	public static IGameItemProto ItemPrototype { get; set; }

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		ItemPrototype = gameworld.ItemProtos.SingleOrDefault(x => x.IsItemType<CommodityGameItemComponentProto>());
		if (ItemPrototype == null)
		{
			var comp = new CommodityGameItemComponentProto(gameworld, null);
			gameworld.Add(comp);
			comp.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
			var proto = new GameItemProto(gameworld, null);
			gameworld.Add(proto);
			HoldableGameItemComponentProto.InitialiseItemType(gameworld);
			proto.AddComponent(gameworld.ItemComponentProtos.Single(x => x is HoldableGameItemComponentProto));
			proto.AddComponent(comp);
			proto.Weight = 0;
			proto.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
			ItemPrototype = proto;
		}
	}

	public static IGameItem CreateNewCommodity(ISolid material, double weight, ITag tag, bool useIndirect = false)
	{
		var newItem = ItemPrototype.CreateNew();
		var commodity = newItem.GetItemType<ICommodity>();
		commodity.Material = material;
		commodity.Weight = weight;
		commodity.Tag = tag;
		commodity.UseIndirectQuantityDescription = useIndirect;
		return newItem;
	}

	public override string TypeDescription => "Commodity";

	#region Constructors

	protected CommodityGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Commodity")
	{
	}

	protected CommodityGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) :
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
		return new XElement("Definition", new[]
		{
			new XElement("Example")
		}).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new CommodityGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new CommodityGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddDatabaseLoader("Commodity",
			(proto, gameworld) => new CommodityGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Commodity",
			$"Makes an item a {"[system-generated]".Colour(Telnet.Green)} commodity pile. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
			"This component is used in auto-generated items only. It should not and cannot be used in any manually created items, nor should it be edited in any way."
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		return CreateNewRevision(initiator,
			(proto, gameworld) => new CommodityGameItemComponentProto(proto, gameworld));
	}

	#endregion

	#region Building Commands

	//public override string ShowBuildingHelp => $"You can use the following options:\n\texample - example";

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
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a commodity pile.",
			"Commodity Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}

	public override bool ReadOnly => true;
	public override bool PreventManualLoad => true;
}
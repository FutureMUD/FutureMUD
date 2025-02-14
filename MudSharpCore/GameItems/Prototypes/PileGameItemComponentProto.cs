using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Decorators;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Prototypes;

public class PileGameItemComponentProto : GameItemComponentProto
{
	public override string TypeDescription => "Pile";

	public static IGameItemProto ItemPrototype { get; set; }

	public IStackDecorator Decorator { get; private set; }

	public static IGameItem CreateNewBundle(IEnumerable<IGameItem> items)
	{
		var newItem = ItemPrototype.CreateNew();
		var newItemPile = newItem.GetItemType<PileGameItemComponent>();
		newItemPile.SetContents(items);
		newItem.Login();
		newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
		return newItem;
	}

	#region Constructors

	protected PileGameItemComponentProto(IFuturemud gameworld, IAccount originator) : base(gameworld, originator,
		"Pile")
	{
		Decorator = Gameworld.StackDecorators.FirstOrDefault(x => x is PileDecorator) ??
		            Gameworld.StackDecorators.First();
	}

	protected PileGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld) : base(
		proto, gameworld)
	{
	}

	protected override void LoadFromXml(XElement root)
	{
		Decorator = Gameworld.StackDecorators.Get(long.Parse(root.Element("Decorator").Value));
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Decorator", Decorator.Id)).ToString();
	}

	#endregion

	#region Component Instance Initialising Functions

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter loader = null, bool temporary = false)
	{
		return new PileGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new PileGameItemComponent(component, this, parent);
	}

	#endregion

	#region Initialisation Tasks

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddDatabaseLoader("Pile", (proto, gameworld) => new PileGameItemComponentProto(proto, gameworld));
		manager.AddTypeHelpInfo(
			"Pile",
			$"Makes an item a {"[system-generated]".Colour(Telnet.Green)} {"[container]".Colour(Telnet.BoldGreen)} of type pile. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
			"This component is used in auto-generated items only. It should not and cannot be used in any manually created items, nor should it be edited in any way."
		);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		throw new NotSupportedException("Piles should not be edited.");
	}

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		ItemPrototype = gameworld.ItemProtos.SingleOrDefault(x => x.IsItemType<PileGameItemComponentProto>());
		if (ItemPrototype == null)
		{
			var comp = new PileGameItemComponentProto(gameworld, null);
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

	public override bool PreventManualLoad => true;
	public override bool ReadOnly => true;

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\r\n\r\nThis item is a pile (aka temporary container).",
			"Pile Game Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name
		);
	}
}
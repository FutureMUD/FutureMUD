using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Components;
using System;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Prototypes;

public class StableTicketGameItemComponentProto : GameItemComponentProto
{
	private static readonly string _showString = "Stable Ticket Item Component".Colour(Telnet.Cyan) + "\n\n" +
	                                             "This item is a system-generated stable ticket.\n";

	private StableTicketGameItemComponentProto(IFuturemud gameworld, IAccount? originator)
		: base(gameworld, originator!, "Stable Ticket")
	{
		Description = "Marks an item as a system-generated stable ticket";
	}

	protected StableTicketGameItemComponentProto(MudSharp.Models.GameItemComponentProto proto, IFuturemud gameworld)
		: base(proto, gameworld)
	{
	}

	public static IGameItemProto? ItemPrototype { get; private set; }
	public override bool ReadOnly => true;
	public override bool PreventManualLoad => true;
	public override string TypeDescription => "Stable Ticket";

	public static void InitialiseItemType(IFuturemud gameworld)
	{
		var component = gameworld.ItemComponentProtos.OfType<StableTicketGameItemComponentProto>().SingleOrDefault();
		if (component is null)
		{
			component = new StableTicketGameItemComponentProto(gameworld, null);
			gameworld.Add(component);
			component.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
		}

		ItemPrototype = gameworld.ItemProtos.SingleOrDefault(x => x.IsItemType<StableTicketGameItemComponentProto>());
		if (ItemPrototype is not null)
		{
			ItemPrototype.ReadOnly = true;
			return;
		}

		HoldableGameItemComponentProto.InitialiseItemType(gameworld);
		GameItemProto proto = new(gameworld, null!, "stable ticket", isReadOnly: true);
		gameworld.Add(proto);
		if (!proto.IsItemType<HoldableGameItemComponentProto>())
		{
			proto.AddComponent(gameworld.ItemComponentProtos.Single(x => x is HoldableGameItemComponentProto));
		}

		proto.AddComponent(component);
		proto.Weight = 0;
		proto.ReadOnly = true;
		proto.ChangeStatus(RevisionStatus.Current, "Automatically generated", null);
		ItemPrototype = proto;
	}

	protected override void LoadFromXml(XElement root)
	{
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition").ToString();
	}

	public static void RegisterComponentInitialiser(GameItemComponentManager manager)
	{
		manager.AddDatabaseLoader("Stable Ticket",
			(proto, gameworld) => new StableTicketGameItemComponentProto(proto, gameworld));

		manager.AddTypeHelpInfo(
			"StableTicket",
			$"Marks an item as a {"[system-generated]".Colour(Telnet.Green)} stable ticket. {"Do not edit this component or use manually.".Colour(Telnet.Red)}",
			"This component is used in auto-generated items only. It should not and cannot be used in any manually created items, nor should it be edited in any way."
		);
	}

	public static IGameItem CreateNewStableTicket(IStableStay stay)
	{
		if (ItemPrototype is null)
		{
			InitialiseItemType(stay.Stable.Gameworld);
		}

		IGameItem newItem = ItemPrototype!.CreateNew();
		IStableTicket ticket = newItem.GetItemType<IStableTicket>();
		ticket.InitialiseTicket(stay);
		newItem.Login();
		newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
		return newItem;
	}

	public override IGameItemComponent CreateNew(IGameItem parent, ICharacter? loader = null, bool temporary = false)
	{
		return new StableTicketGameItemComponent(this, parent, temporary);
	}

	public override IGameItemComponent LoadComponent(MudSharp.Models.GameItemComponent component, IGameItem parent)
	{
		return new StableTicketGameItemComponent(component, this, parent);
	}

	public override IEditableRevisableItem CreateNewRevision(ICharacter initiator)
	{
		throw new NotSupportedException("Stable Tickets should not be edited.");
	}

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		command.PopSpeech();
		return base.BuildingCommand(actor, command);
	}

	public override string ComponentDescriptionOLC(ICharacter actor)
	{
		return string.Format(actor, "{0} (#{1:N0}r{2:N0}, {3})\n\n{4}",
			"Stable Ticket Item Component".Colour(Telnet.Cyan),
			Id,
			RevisionNumber,
			Name,
			_showString);
	}
}

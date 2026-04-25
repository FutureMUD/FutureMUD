using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Prototypes;
using System.Linq;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Components;

public class StableTicketGameItemComponent : GameItemComponent, IStableTicket
{
	private StableTicketGameItemComponentProto _prototype;
	private long? _stableStayId;
	private long? _ticketItemId;
	private string _ticketToken = string.Empty;

	public StableTicketGameItemComponent(StableTicketGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public StableTicketGameItemComponent(MudSharp.Models.GameItemComponent component,
		StableTicketGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public StableTicketGameItemComponent(StableTicketGameItemComponent rhs, IGameItem newParent, bool temporary = false)
		: base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_stableStayId = rhs._stableStayId;
		_ticketItemId = rhs._ticketItemId;
		_ticketToken = rhs._ticketToken;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public long? StableStayId => _stableStayId;
	public long? TicketItemId => _ticketItemId;
	public string TicketToken => _ticketToken;

	public IStableStay? StableStay => StableStayId is null
		? null
		: Gameworld.Stables.SelectMany(x => x.Stays).FirstOrDefault(x => x.Id == StableStayId.Value);

	public bool IsValid => StableStay?.TicketMatches(Parent, TicketToken) == true;

	public void InitialiseTicket(IStableStay stay)
	{
		_stableStayId = stay.Id;
		_ticketItemId = Parent.Id;
		_ticketToken = stay.TicketToken;
		stay.RegisterTicket(Parent.Id, TicketToken);
		Changed = true;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new StableTicketGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return "a stable ticket";
			case DescriptionType.Full:
				var stay = StableStay;
				if (stay is null)
				{
					return "This stable ticket does not correspond to any known stable stay.";
				}

				stay.Stable.AssessFees(stay);
				var mount = stay.Mount;
				var stable = stay.Stable;
				return
					$"This is a stable ticket for {(mount is null ? $"mount #{stay.MountId:N0}" : mount.HowSeen(voyeur, colour: colour))} at {stable.Name.TitleCase().ColourName()}.\n" +
					$"Status: {(IsValid ? "Valid".Colour(Telnet.Green) : "Invalid".Colour(Telnet.Red))}\n" +
					$"Outstanding Fees: {stable.Currency.Describe(stay.AmountOwing, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (StableTicketGameItemComponentProto)newProto;
	}

	private void LoadFromXml(XElement root)
	{
		_stableStayId = long.TryParse(root.Element("StayId")?.Value, out var stayId) ? stayId : null;
		_ticketItemId = long.TryParse(root.Element("TicketItemId")?.Value, out var itemId) ? itemId : null;
		_ticketToken = root.Element("TicketToken")?.Value ?? string.Empty;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("StayId", StableStayId),
			new XElement("TicketItemId", TicketItemId),
			new XElement("TicketToken", TicketToken)
		).ToString();
	}
}

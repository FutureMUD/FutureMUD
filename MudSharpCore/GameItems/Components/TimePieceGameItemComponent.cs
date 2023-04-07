using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.TimeAndDate.Time;

namespace MudSharp.GameItems.Components;

public class TimePieceGameItemComponent : GameItemComponent, ITimePiece
{
	protected TimePieceGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (TimePieceGameItemComponentProto)newProto;
	}

	#region Constructors

	public TimePieceGameItemComponent(TimePieceGameItemComponentProto proto, IGameItem parent, bool temporary = false) :
		base(parent, proto, temporary)
	{
		_prototype = proto;
		TimeZone = parent.Location?.Clocks.Contains(Clock) ?? false
			? parent.Location?.TimeZone(Clock)
			: Clock.PrimaryTimezone;
	}

	public TimePieceGameItemComponent(MudSharp.Models.GameItemComponent component,
		TimePieceGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public TimePieceGameItemComponent(TimePieceGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		SecondsOffset = rhs.SecondsOffset;
		TimeZone = rhs.TimeZone;
	}

	protected void LoadFromXml(XElement root)
	{
		TimeZone = Clock.Timezones.Get(long.Parse(root.Element("Timezone").Value));
		SecondsOffset = int.Parse(root.Element("SecondsOffset").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TimePieceGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("Timezone", TimeZone?.Id ?? 0),
			new XElement("SecondsOffset", SecondsOffset)).ToString();
	}

	#endregion

	#region ITimePiece Implementation

	public IClock Clock => _prototype.Clock;

	public IMudTimeZone TimeZone { get; set; }

	public int SecondsOffset { get; set; }

	public string TimeDisplayString => _prototype.TimeDisplayString;

	public MudTime CurrentTime
	{
		get
		{
			var time = Clock.CurrentTime.GetTimeByTimezone(TimeZone ?? Clock.PrimaryTimezone);
			time.AddSeconds(SecondsOffset);
			return time;
		}
	}

	public bool CanSetTime(ICharacter actor)
	{
		return _prototype.PlayersCanSetTime || actor.IsAdministrator();
	}

	public void SetTime(MudTime time)
	{
		if (time == null)
		{
			throw new ArgumentNullException(nameof(time),
				"The time parameter in TimePieceGameItemComponent.SetTime cannot be null.");
		}

		if (time.Clock != Clock)
		{
			throw new ArgumentOutOfRangeException(nameof(time), time.Clock,
				"The clock for the time parameter did not match the items prototype.");
		}

		TimeZone = time.Timezone;
		var realTime = Clock.CurrentTime.GetTimeByTimezone(TimeZone);
		SecondsOffset = realTime.SecondsDifference(time);
		Changed = true;
	}

	#endregion

	#region GameItemComponent Overrides

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return base.Decorate(voyeur, name, description, type, colour, flags);
		}

		return
			$"{description}\n\nThe time currently showing is {Clock.DisplayTime(CurrentTime, TimeDisplayString).Colour(Telnet.Green)}.";
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	#endregion
}
using System;
using System.Xml.Linq;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class BiroGameItemComponent : GameItemComponent, IWritingImplement
{
	protected BiroGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		var newPen = (BiroGameItemComponentProto)newProto;
		RemainingUses = Math.Max(0, RemainingUses - newPen.TotalUses + _prototype.TotalUses);
		_prototype = newPen;
	}

	#region Constructors

	public BiroGameItemComponent(BiroGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
		RemainingUses = _prototype.TotalUses;
	}

	public BiroGameItemComponent(MudSharp.Models.GameItemComponent component, BiroGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public BiroGameItemComponent(BiroGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		RemainingUses = rhs.RemainingUses;
	}

	protected void LoadFromXml(XElement root)
	{
		_remainingUses = int.Parse(root.Element("RemainingUses").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new BiroGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("RemainingUses", RemainingUses)
		).ToString();
	}

	#endregion

	#region IWritingImplement Implementation

	private int _remainingUses;

	public int RemainingUses
	{
		get => _remainingUses;
		set
		{
			_remainingUses = value;
			Changed = true;
		}
	}

	public void Use(int uses)
	{
		RemainingUses -= uses;
	}

	public WritingImplementType WritingImplementType => WritingImplementType.Biro;

	public IColour WritingImplementColour => _prototype.Colour ??
	                                         (Parent.GetItemType<VariableGameItemComponent>()
	                                                ?.GetCharacteristic(_prototype.ColourCharacteristic) as
		                                         ColourCharacteristicValue)?.Colour ??
	                                         Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultBiroColour"));

	public bool Primed => RemainingUses > 0;

	#endregion

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Full)
		{
			if (RemainingUses <= 0)
			{
				return
					$"{description}\n\nIt is a {WritingImplementColour.Name.Colour(Telnet.Cyan)} biro that has been completely used up.";
			}

			return
				$"{description}\n\nIt is a {WritingImplementColour.Name.Colour(Telnet.Cyan)} biro, approximately {1.0 - (double)RemainingUses / _prototype.TotalUses:P0} used up.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}
}
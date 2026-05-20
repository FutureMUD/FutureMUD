using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using System;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.GameItems.Components;

public class ScribingImplementGameItemComponent : GameItemComponent, IWritingImplement
{
	private ScribingImplementGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		var newImplement = (ScribingImplementGameItemComponentProto)newProto;
		if (newImplement.TotalUses <= 0)
		{
			_remainingUses = 0;
		}
		else if (_prototype.TotalUses <= 0)
		{
			RemainingUses = newImplement.TotalUses;
		}
		else
		{
			RemainingUses = Math.Max(0, RemainingUses + newImplement.TotalUses - _prototype.TotalUses);
		}

		_prototype = newImplement;
	}

	#region Constructors

	public ScribingImplementGameItemComponent(ScribingImplementGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		RemainingUses = Math.Max(0, _prototype.TotalUses);
	}

	public ScribingImplementGameItemComponent(MudSharp.Models.GameItemComponent component,
		ScribingImplementGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ScribingImplementGameItemComponent(ScribingImplementGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		RemainingUses = rhs.RemainingUses;
	}

	private void LoadFromXml(XElement root)
	{
		_remainingUses = int.Parse(root.Element("RemainingUses")?.Value ?? Math.Max(0, _prototype.TotalUses).ToString());
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ScribingImplementGameItemComponent(this, newParent, temporary);
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
		if (_prototype.TotalUses <= 0)
		{
			return;
		}

		RemainingUses = Math.Max(0, RemainingUses - Math.Max(0, uses));
	}

	public WritingImplementType WritingImplementType => _prototype.ImplementType;

	public IColour WritingImplementColour =>
		_prototype.Colour ??
		(_prototype.ColourCharacteristic is not null
			? (Parent.GetItemType<VariableGameItemComponent>()
			       ?.GetCharacteristic(_prototype.ColourCharacteristic) as ColourCharacteristicValue)?.Colour
			: null) ??
		Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultWritingColourInText")) ??
		Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultBiroColour"))!;

	public bool Primed => _prototype.TotalUses <= 0 || RemainingUses > 0;

	#endregion

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type != DescriptionType.Full)
		{
			return base.Decorate(voyeur, name, description, type, colour, flags);
		}

		var implementName = WritingImplementType.Describe().ToLowerInvariant();
		if (_prototype.TotalUses <= 0)
		{
			return
				$"{description}\n\nIt is a {implementName} writing implement that does not use ink or pigment.";
		}

		if (RemainingUses <= 0)
		{
			return
				$"{description}\n\nIt is a {WritingImplementColour.Name.Colour(Telnet.Cyan)} {implementName} writing implement that has been completely used up.";
		}

		return
			$"{description}\n\nIt is a {WritingImplementColour.Name.Colour(Telnet.Cyan)} {implementName} writing implement, approximately {1.0 - (double)RemainingUses / _prototype.TotalUses:P0} used up.";
	}
}

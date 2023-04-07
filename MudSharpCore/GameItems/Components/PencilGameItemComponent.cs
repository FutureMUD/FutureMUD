using System;
using System.Xml.Linq;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Colour;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class PencilGameItemComponent : GameItemComponent, IWritingImplement
{
	protected PencilGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		var newPencil = (PencilGameItemComponentProto)newProto;
		RemainingUses = Math.Max(0, RemainingUses - newPencil.TotalUses + _prototype.TotalUses);
		_prototype = newPencil;
	}

	#region Constructors

	public PencilGameItemComponent(PencilGameItemComponentProto proto, IGameItem parent, bool temporary = false) : base(
		parent, proto, temporary)
	{
		_prototype = proto;
		RemainingUses = _prototype.TotalUses;
		UsesSinceSharpening = 0;
	}

	public PencilGameItemComponent(MudSharp.Models.GameItemComponent component, PencilGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PencilGameItemComponent(PencilGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		RemainingUses = rhs.RemainingUses;
		UsesSinceSharpening = rhs.UsesSinceSharpening;
	}

	protected void LoadFromXml(XElement root)
	{
		_remainingUses = int.Parse(root.Element("RemainingUses").Value);
		_usesSinceSharpening = int.Parse(root.Element("UsesSinceSharpening").Value);
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PencilGameItemComponent(this, newParent, temporary);
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("RemainingUses", RemainingUses),
			new XElement("UsesSinceSharpening", UsesSinceSharpening)
		).ToString();
	}

	#endregion

	#region IWritingImplement Implementation

	private int _usesSinceSharpening;

	public int UsesSinceSharpening
	{
		get => _usesSinceSharpening;
		set
		{
			_usesSinceSharpening = value;
			Changed = true;
		}
	}

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
		UsesSinceSharpening += uses;
	}

	public WritingImplementType WritingImplementType => WritingImplementType.Pencil;

	public IColour WritingImplementColour => _prototype.Colour ??
	                                         (Parent.GetItemType<VariableGameItemComponent>()
	                                                ?.GetCharacteristic(_prototype.ColourCharacteristic) as
		                                         ColourCharacteristicValue)?.Colour ??
	                                         Gameworld.Colours.Get(Gameworld.GetStaticLong("DefaultPencilColour"));

	public bool Primed => RemainingUses > 0 && UsesSinceSharpening < _prototype.UsesBeforeSharpening;

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
					$"{description}\n\nIt is a {WritingImplementColour.Name.Colour(Telnet.Cyan)} pencil that has been completely used up.";
			}

			string sharpness;
			var ratio = UsesSinceSharpening / _prototype.UsesBeforeSharpening;
			if (ratio >= 1.0)
			{
				sharpness = "completely blunt";
			}
			else if (ratio >= 0.8)
			{
				sharpness = "in need of a good sharpen";
			}
			else if (ratio >= 0.6)
			{
				sharpness = "getting a little blunt";
			}
			else if (ratio >= 0.4)
			{
				sharpness = "servicably sharp";
			}
			else if (ratio >= 0.2)
			{
				sharpness = "well sharpened";
			}
			else
			{
				sharpness = "perfectly sharpened";
			}

			return
				$"{description}\n\nIt is a {WritingImplementColour.Name.Colour(Telnet.Cyan)} pencil, approximately {1.0 - (double)RemainingUses / _prototype.TotalUses:P0} used up. It is {sharpness}.";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}
}
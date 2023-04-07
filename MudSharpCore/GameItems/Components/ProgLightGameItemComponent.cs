using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ProgLightGameItemComponent : GameItemComponent, ILightable, IProduceLight
{
	protected bool _lit;
	protected ProgLightGameItemComponentProto _prototype;

	public virtual bool Lit
	{
		get => _lit;
		set
		{
			if (_lit != value)
			{
				_lit = value;
				Changed = true;
			}
		}
	}

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ProgLightGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short || type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return $"{description}{(Lit ? " (lit)".FluentColour(Telnet.Red, colour) : "")}";
			case DescriptionType.Full:
				return $"{description}\n\n{(Lit ? "It is currently lit." : "It is not currently lit.")}";
		}

		throw new NotSupportedException("Invalid Decorate type in ProgLightGameItemComponent.Decorate");
	}

	public override int DecorationPriority => int.MaxValue;

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemLightable = newItem?.GetItemType<ILightable>();
		if (newItemLightable != null)
		{
			newItemLightable.Lit = Lit;
		}

		return false;
	}

	#region IProduceLight Members

	public double CurrentIllumination => Lit ? _prototype.IlluminationProvided : 0.0;

	#endregion

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ProgLightGameItemComponentProto)newProto;
	}

	#region Constructors

	public ProgLightGameItemComponent(ProgLightGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		Lit = rhs.Lit;
	}

	public ProgLightGameItemComponent(ProgLightGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public ProgLightGameItemComponent(MudSharp.Models.GameItemComponent component,
		ProgLightGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new object[]
			{
				new XElement("Lit", Lit)
			}
		).ToString();
	}

	protected void LoadFromXml(XElement root)
	{
		Lit = bool.Parse(root.Element("Lit").Value);
	}

	#endregion

	#region ILightable Members

	public virtual bool CanLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		return false;
	}

	public virtual string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		return $"You cannot light {Parent.HowSeen(lightee)} because you do not know a way to.";
	}

	public bool Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)
	{
		if (!CanLight(lightee, ignitionSource))
		{
			lightee.Send(WhyCannotLight(lightee, ignitionSource));
			return false;
		}

		return true;
	}

	public bool CanExtinguish(ICharacter lightee)
	{
		return false;
	}

	public string WhyCannotExtinguish(ICharacter lightee)
	{
		return $"You cannot extinguish {Parent.HowSeen(lightee)} because you do not know a way to.";
	}

	public bool Extinguish(ICharacter lightee, IEmote playerEmote)
	{
		if (!CanExtinguish(lightee))
		{
			lightee.Send(WhyCannotExtinguish(lightee));
			return false;
		}

		return true;
	}

	#endregion
}
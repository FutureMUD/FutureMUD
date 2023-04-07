using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class PoweredPropGameItemComponent : PoweredMachineBaseGameItemComponent
{
	protected PoweredPropGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (PoweredPropGameItemComponentProto)newProto;
	}

	#region Constructors

	public PoweredPropGameItemComponent(PoweredPropGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(proto, parent, temporary)
	{
		_prototype = proto;
	}

	public PoweredPropGameItemComponent(MudSharp.Models.GameItemComponent component,
		PoweredPropGameItemComponentProto proto, IGameItem parent) : base(component, proto, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public PoweredPropGameItemComponent(PoweredPropGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new PoweredPropGameItemComponent(this, newParent, temporary);
	}

	protected override XElement SaveToXml(XElement root)
	{
		return root;
	}

	#endregion

	#region IConsumePower Implementation

	private void TenSecondEvent()
	{
		_prototype.TenSecondProg?.Execute(Parent);
	}

	protected override void OnPowerCutInAction()
	{
		if (_prototype.TenSecondProg != null)
		{
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondEvent;
			Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat += TenSecondEvent;
		}
	}

	protected override void OnPowerCutOutAction()
	{
		Gameworld.HeartbeatManager.FuzzyTenSecondHeartbeat -= TenSecondEvent;
	}

	#endregion

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		if (type == DescriptionType.Short)
		{
			return $"{description}{(_onAndPowered ? " (on)".FluentColour(Telnet.BoldWhite, colour) : "")}";
		}

		return base.Decorate(voyeur, name, description, type, colour, flags);
	}
}
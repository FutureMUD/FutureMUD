using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class UnlimitedGeneratorGameItemComponent : GameItemComponent, IProducePower, ISwitchable, IOnOff
{
	protected UnlimitedGeneratorGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (UnlimitedGeneratorGameItemComponentProto)newProto;
	}

	private readonly List<IConsumePower> _connectedConsumers = new();
	private readonly List<IConsumePower> _powerUsers = new();

	#region IOnOff Implementation

	private bool _switchedOn;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			_switchedOn = value;
			Changed = true;
			if (value)
			{
				CheckOn();
			}
			else
			{
				foreach (var item in _powerUsers.ToList())
				{
					item.OnPowerCutOut();
				}

				_spikeDrawdown = 0.0;
				_powerUsers.Clear();
				Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
				_heartbeatOn = false;
			}
		}
	}

	#endregion

	#region Constructors

	public UnlimitedGeneratorGameItemComponent(UnlimitedGeneratorGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public UnlimitedGeneratorGameItemComponent(Models.GameItemComponent component,
		UnlimitedGeneratorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public UnlimitedGeneratorGameItemComponent(UnlimitedGeneratorGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		SwitchedOn = rhs.SwitchedOn;
	}

	protected void LoadFromXml(XElement root)
	{
		var attr = root.Attribute("On");
		if (attr != null)
		{
			SwitchedOn = bool.Parse(attr.Value);
		}
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new UnlimitedGeneratorGameItemComponent(this, newParent, temporary);
	}

	public override void FinaliseLoad()
	{
		if (SwitchedOn)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
		}
	}

	#endregion

	#region Saving

	protected override string SaveToXml()
	{
		return new XElement("Definition",
				new XAttribute("On", SwitchedOn))
			.ToString();
	}

	#endregion

	#region Override of Base

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type == DescriptionType.Short ||
		       type == DescriptionType.Full;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		switch (type)
		{
			case DescriptionType.Short:
				return $"{description}{(SwitchedOn ? " (on)".FluentColour(Telnet.BoldWhite, colour) : "")}";
			case DescriptionType.Full:
				var sb = new StringBuilder();
				sb.Append(description);
				sb.AppendLine();
				sb.AppendLine($"It is{(SwitchedOn ? "" : " not")} switched on.".Colour(Telnet.Yellow));
				return sb.ToString();
		}

		throw new NotSupportedException("Invalid Decorate type in UnlimitedGeneratorGameItemComponent.Decorate");
	}

	public override int DecorationPriority => int.MaxValue;

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return true;
	}

	#endregion

	#region IProducePower

	public bool PrimaryLoadTimePowerProducer => true;
	public bool PrimaryExternalConnectionPowerProducer => false;
	public double FuelLevel => 1.0;
	public bool ProducingPower => SwitchedOn;

	public double MaximumPowerInWatts => ProducingPower ? _prototype.WattageProvided : 0.0;

	private double _spikeDrawdown;

	public bool CanBeginDrawDown(double wattage)
	{
		return SwitchedOn && ProducingPower &&
		       _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
		       _prototype.WattageProvided;
	}

	public bool CanDrawdownSpike(double wattage)
	{
		return SwitchedOn &&
		       _powerUsers.Sum(x => x.PowerConsumptionInWatts) + wattage + _spikeDrawdown <
		       _prototype.WattageProvided && ProducingPower;
	}

	public bool DrawdownSpike(double wattage)
	{
		if (!CanDrawdownSpike(wattage))
		{
			return false;
		}

		_spikeDrawdown += wattage;
		return true;
	}

	public void BeginDrawdown(IConsumePower item)
	{
		if (!_connectedConsumers.Contains(item))
		{
			_connectedConsumers.Add(item);

			if (SwitchedOn &&
			    _powerUsers.Sum(x => x.PowerConsumptionInWatts) + item.PowerConsumptionInWatts <=
			    _prototype.WattageProvided)
			{
				_powerUsers.Add(item);
				item.OnPowerCutIn();
			}
		}
	}

	public void EndDrawdown(IConsumePower item)
	{
		_connectedConsumers.Remove(item);
		if (_powerUsers.Contains(item))
		{
			item.OnPowerCutOut();
		}

		_powerUsers.Remove(item);
	}

	#endregion

	#region ISwitchable

	public IEnumerable<string> SwitchSettings => new[] { "on", "off" };

	public bool CanSwitch(ICharacter actor, string setting)
	{
		return (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn) ||
		       (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
			;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already on.";
		}

		if (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn)
		{
			return $"{Parent.HowSeen(actor)} is already off.";
		}

		return $"{Parent.HowSeen(actor)} cannot be switched to {setting} at this time.";
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			return false;
		}

		return setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase)
			? SwitchOn(actor)
			: SwitchOff(actor);
	}

	private bool SwitchOn(ICharacter actor)
	{
		SwitchedOn = true;
		_prototype.SwitchOnProg?.Execute(actor, Parent);
		return true;
	}

	private bool _heartbeatOn;

	private void CheckOn()
	{
		if (!_heartbeatOn && SwitchedOn)
		{
			_spikeDrawdown = 0.0;
			var cumulativeDraw = 0.0;
			foreach (var item in _connectedConsumers)
			{
				if (_prototype.WattageProvided - cumulativeDraw >= item.PowerConsumptionInWatts)
				{
					_powerUsers.Add(item);
					item.OnPowerCutIn();
					cumulativeDraw += item.PowerConsumptionInWatts;
				}
			}

			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
			_heartbeatOn = true;
		}
	}

	private void HeartbeatManager_SecondHeartbeat()
	{
		_spikeDrawdown = 0.0;
	}

	private bool SwitchOff(ICharacter actor)
	{
		_prototype.SwitchOffProg?.Execute(actor, Parent);
		SwitchedOn = false;
		return true;
	}

	#endregion
}
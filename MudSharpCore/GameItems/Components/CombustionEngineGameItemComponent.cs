#nullable enable

using MudSharp.Form.Audio;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class CombustionEngineGameItemComponent : GameItemComponent, IVehicleEngine, ISwitchable
{
	private CombustionEngineGameItemComponentProto _prototype;
	private bool _switchedOn;
	private bool _runtimeActive;
	private bool _heartbeatSubscribed;

	public CombustionEngineGameItemComponent(CombustionEngineGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
	}

	public CombustionEngineGameItemComponent(MudSharp.Models.GameItemComponent component,
		CombustionEngineGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		var root = XElement.Parse(component.Definition);
		_switchedOn = bool.TryParse(root.Element("SwitchedOn")?.Value, out var value) && value;
		_noSave = false;
	}

	private CombustionEngineGameItemComponent(CombustionEngineGameItemComponent rhs, IGameItem newParent,
		bool temporary) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		_switchedOn = rhs._switchedOn;
	}

	public override IGameItemComponentProto Prototype => _prototype;
	public string FormFactor => _prototype.FormFactor;
	public double MaximumPowerInWatts => _prototype.MaximumPowerInWatts;
	public AudioVolume NoiseLevel => _prototype.NoiseLevel;
	public bool IsRunning => SwitchedOn && HasFuel;
	public string WhyNotRunning => !SwitchedOn
		? "the engine is switched off"
		: _prototype.FuelLiquid is null
			? "the engine has no configured fuel"
			: !Parent.GetItemTypes<ILiquidContainer>().Any()
				? "the engine item has no liquid container"
				: !HasFuel
					? "the engine has run out of its configured fuel"
					: string.Empty;

	public bool SwitchedOn
	{
		get => _switchedOn;
		set
		{
			if (_switchedOn == value)
			{
				return;
			}

			_switchedOn = value;
			Changed = true;
			UpdateHeartbeat();
		}
	}

	private bool HasFuel => _prototype.FuelLiquid is not null &&
	                        Parent.GetItemTypes<ILiquidContainer>()
		                        .Any(x => x.LiquidMixture?.Instances.Any(y =>
			                        y.Amount > 0.0 && y.Liquid.LiquidCountsAs(_prototype.FuelLiquid)) == true);

	public IEnumerable<string> SwitchSettings => ["on", "off"];

	public bool CanSwitch(ICharacter actor, string setting)
	{
		if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase))
		{
			return !SwitchedOn && HasFuel;
		}

		return setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn;
	}

	public string WhyCannotSwitch(ICharacter actor, string setting)
	{
		if (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase))
		{
			return SwitchedOn
				? $"{Parent.HowSeen(actor, true)} is already switched on."
				: $"{Parent.HowSeen(actor, true)} cannot start because {WhyNotRunning}.";
		}

		return !SwitchedOn
			? $"{Parent.HowSeen(actor, true)} is already switched off."
			: $"{Parent.HowSeen(actor, true)} cannot be switched to {setting}.";
	}

	public bool Switch(ICharacter actor, string setting)
	{
		if (!CanSwitch(actor, setting))
		{
			return false;
		}

		SwitchedOn = setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase);
		Parent.Handle(new AudioOutput(
			SwitchedOn ? "@ roar|roars to life." : "@ rumble|rumbles to a stop.",
			NoiseLevel, Parent), OutputRange.Local);
		return true;
	}

	public void EmitOperatingNoise()
	{
		if (IsRunning && NoiseLevel != AudioVolume.Silent)
		{
			Parent.Handle(new AudioOutput("@ rumble|rumbles as it drives the vehicle.", NoiseLevel, Parent),
				OutputRange.Local);
		}
	}

	public override void Login()
	{
		_runtimeActive = true;
		if (_switchedOn && !HasFuel)
		{
			_switchedOn = false;
			Changed = true;
		}

		UpdateHeartbeat();
		base.Login();
	}

	public override void Quit()
	{
		_runtimeActive = false;
		StopHeartbeat();
		base.Quit();
	}

	public override void Delete()
	{
		_runtimeActive = false;
		StopHeartbeat();
		base.Delete();
	}

	private void UpdateHeartbeat()
	{
		if (_runtimeActive && IsRunning)
		{
			if (!_heartbeatSubscribed)
			{
				Gameworld.HeartbeatManager.SecondHeartbeat += SecondHeartbeat;
				_heartbeatSubscribed = true;
			}

			return;
		}

		StopHeartbeat();
	}

	private void StopHeartbeat()
	{
		if (!_heartbeatSubscribed)
		{
			return;
		}

		Gameworld.HeartbeatManager.SecondHeartbeat -= SecondHeartbeat;
		_heartbeatSubscribed = false;
	}

	private void SecondHeartbeat()
	{
		if (!ConsumeFuel(_prototype.FuelPerSecond))
		{
			_switchedOn = false;
			Changed = true;
			StopHeartbeat();
			Parent.Handle(new AudioOutput("@ sputter|sputters and fall|falls silent.", NoiseLevel, Parent),
				OutputRange.Local);
		}
	}

	private bool ConsumeFuel(double amount)
	{
		if (_prototype.FuelLiquid is null || amount <= 0.0)
		{
			return false;
		}

		var remaining = amount;
		foreach (var container in Parent.GetItemTypes<ILiquidContainer>())
		{
			var mixture = container.LiquidMixture;
			if (mixture is null)
			{
				continue;
			}

			foreach (var instance in mixture.Instances
				         .Where(x => x.Amount > 0.0 && x.Liquid.LiquidCountsAs(_prototype.FuelLiquid))
				         .ToList())
			{
				var removed = Math.Min(remaining, instance.Amount);
				mixture.RemoveLiquidVolume(instance, removed);
				remaining -= removed;
				if (remaining <= 0.0)
				{
					break;
				}
			}

			container.LiquidMixture = mixture;
			if (remaining <= 0.0)
			{
				return true;
			}
		}

		return false;
	}

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new CombustionEngineGameItemComponent(this, newParent, temporary);
	}

	public override bool DescriptionDecorator(DescriptionType type)
	{
		return type is DescriptionType.Short or DescriptionType.Evaluate;
	}

	public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
		bool colour, PerceiveIgnoreFlags flags)
	{
		return type switch
		{
			DescriptionType.Short => $"{description}{(IsRunning ? " (running)".FluentColour(Telnet.BoldWhite, colour) : "")}",
			DescriptionType.Evaluate =>
				$"{description}\n\nIt is a {FormFactor.ColourCommand()} combustion vehicle engine producing up to {MaximumPowerInWatts.ToString("N2", voyeur).ColourValue()}W. It is {(IsRunning ? "running".ColourValue() : WhyNotRunning.ColourError())}.",
			_ => description
		};
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (CombustionEngineGameItemComponentProto)newProto;
		UpdateHeartbeat();
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("SwitchedOn", SwitchedOn)).ToString();
	}
}

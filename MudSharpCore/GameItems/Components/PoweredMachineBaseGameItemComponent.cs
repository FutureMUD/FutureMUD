using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MudSharp.GameItems.Components;

public abstract class PoweredMachineBaseGameItemComponent : GameItemComponent, IConsumePower, ISwitchable, IOnOff
{
    private PoweredMachineBaseGameItemComponentProto _prototype;
	private IProducePower? _powerSource;
	private bool _drawingPower;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (PoweredMachineBaseGameItemComponentProto)newProto;
		RefreshPowerSourceConnection();
    }

    #region Constructors

    protected PoweredMachineBaseGameItemComponent(PoweredMachineBaseGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
        if (!_prototype.Switchable)
        {
            _switchedOn = true;
        }
    }

    protected PoweredMachineBaseGameItemComponent(MudSharp.Models.GameItemComponent component,
        PoweredMachineBaseGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    protected PoweredMachineBaseGameItemComponent(PoweredMachineBaseGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _switchedOn = rhs._switchedOn;
    }

    protected virtual void LoadFromXml(XElement root)
    {
        _switchedOn = bool.Parse(root.Element("SwitchedOn").Value);
    }

    public override void FinaliseLoad()
    {
    }

    #endregion

    #region Saving

    protected abstract XElement SaveToXml(XElement root);

    protected override string SaveToXml()
    {
        return SaveToXml(new XElement("Definition",
            new XElement("SwitchedOn", SwitchedOn)
        )).ToString();
    }

    #endregion

    public override void Delete()
    {
		EndPowerDrawdown();
        base.Delete();
    }

    public override void Quit()
    {
		EndPowerDrawdown();
        base.Quit();
    }

    public override void Login()
    {
        if (SwitchedOn)
        {
			BeginPowerDrawdown();
        }

        base.Login();
    }

    #region IConsumePower Implementation

    protected bool _onAndPowered;
	public bool IsPowered => _onAndPowered;

    public void OnPowerCutIn()
    {
        if (SwitchedOn)
        {
            Parent.Handle(new EmoteOutput(new Emote(_prototype.PowerOnEmote, Parent, Parent),
                flags: OutputFlags.SuppressObscured), OutputRange.Local);
            _prototype.OnPoweredProg?.Execute(Parent);
            _onAndPowered = true;
            OnPowerCutInAction();
        }
    }

    protected abstract void OnPowerCutInAction();
    protected abstract void OnPowerCutOutAction();

    public void OnPowerCutOut()
    {
        if (_onAndPowered)
        {
            Parent.Handle(new EmoteOutput(new Emote(_prototype.PowerOffEmote, Parent, Parent),
                flags: OutputFlags.SuppressObscured), OutputRange.Local);
            _prototype.OnUnpoweredProg?.Execute(Parent);
            OnPowerCutOutAction();
            _onAndPowered = false;
        }
    }

    public double PowerConsumptionInWatts =>
        SwitchedOn ? _prototype.Wattage - _prototype.WattageDiscountPerQuality * (int)Parent.Quality : 0.0;

    #endregion

    #region IOnOff Implementation

    private bool _switchedOn;

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
            if (value)
            {
				BeginPowerDrawdown();
            }
            else
            {
				EndPowerDrawdown();
            }
        }
    }

    #endregion

    #region ISwitchable Implementation

    public IEnumerable<string> SwitchSettings => _prototype.Switchable ? new[] { "on", "off" } : new string[] { };

    public bool CanSwitch(ICharacter actor, string setting)
    {
        if (_prototype.Switchable)
        {
            return (setting.StartsWith("on", StringComparison.InvariantCultureIgnoreCase) && !SwitchedOn) ||
                   (setting.StartsWith("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn)
                ;
        }

        return false;
    }

    public string WhyCannotSwitch(ICharacter actor, string setting)
    {
        if (!_prototype.Switchable)
        {
            return $"{Parent.HowSeen(actor)} is not something that can be switched to {setting}.";
        }

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

    private bool SwitchOn(ICharacter actor)
    {
        SwitchedOn = true;
        return true;
    }

    private bool SwitchOff(ICharacter actor)
    {
        SwitchedOn = false;
        return true;
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

    #endregion

	protected virtual IProducePower? ResolvePowerSource()
	{
		if (_prototype.UseMountHostPowerSource &&
		    this is IAutomationMountable { IsMounted: true, MountHost: not null } mountable)
		{
			return ResolvePowerSourceForItem(mountable.MountHost.Parent) ?? ResolvePowerSourceForItem(Parent);
		}

		return ResolvePowerSourceForItem(Parent);
	}

	private static IProducePower? ResolvePowerSourceForItem(IGameItem item)
	{
		return ResolvePowerSourceFromComponents(item)
		       ?? item.AttachedAndConnectedItems
			       .Select(ResolvePowerSourceFromComponents)
			       .FirstOrDefault(x => x is not null);
	}

	private static IProducePower? ResolvePowerSourceFromComponents(IGameItem item)
	{
		var producers = item.GetItemTypes<IProducePower>()
			.ToList();
		if (!producers.Any())
		{
			var producer = item.GetItemType<IProducePower>();
			if (producer is not null)
			{
				producers.Add(producer);
			}
		}

		return producers
			.Where(x => x.PrimaryLoadTimePowerProducer || x.PrimaryExternalConnectionPowerProducer ||
			            x.MaximumPowerInWatts > 0.0)
			.OrderByDescending(x => x.ProducingPower)
			.ThenByDescending(x => x.PrimaryExternalConnectionPowerProducer)
			.ThenByDescending(x => x.PrimaryLoadTimePowerProducer)
			.FirstOrDefault();
	}

	protected void RefreshPowerSourceConnection()
	{
		if (!SwitchedOn)
		{
			return;
		}

		SetPowerSource(ResolvePowerSource());
	}

	private void BeginPowerDrawdown()
	{
		SetPowerSource(ResolvePowerSource());
	}

	private void EndPowerDrawdown()
	{
		if (_powerSource is not null && _drawingPower)
		{
			_powerSource.EndDrawdown(this);
		}

		_powerSource = null;
		_drawingPower = false;
	}

	private void SetPowerSource(IProducePower? powerSource)
	{
		if (!ReferenceEquals(_powerSource, powerSource))
		{
			if (_powerSource is not null && _drawingPower)
			{
				_powerSource.EndDrawdown(this);
				_drawingPower = false;
			}

			_powerSource = powerSource;
		}

		if (_powerSource is null || _drawingPower)
		{
			return;
		}

		_powerSource.BeginDrawdown(this);
		_drawingPower = true;
	}
}

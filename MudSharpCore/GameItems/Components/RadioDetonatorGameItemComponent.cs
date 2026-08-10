using MudSharp.Communication.Language;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.GameItems.Components;

public class RadioDetonatorGameItemComponent : GameItemComponent, IReceive, IConsumePower, IOnOff, ISelectable,
    ISwitchable, IArmableExplosiveTrigger
{
    protected RadioDetonatorGameItemComponentProto _prototype;
    public override IGameItemComponentProto Prototype => _prototype;

    protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
    {
        _prototype = (RadioDetonatorGameItemComponentProto)newProto;
    }

    #region Constructors

    public RadioDetonatorGameItemComponent(RadioDetonatorGameItemComponentProto proto, IGameItem parent,
        bool temporary = false) : base(parent, proto, temporary)
    {
        _prototype = proto;
        _detonationSequence = string.Empty;
    }

    public RadioDetonatorGameItemComponent(MudSharp.Models.GameItemComponent component,
        RadioDetonatorGameItemComponentProto proto, IGameItem parent) : base(component, parent)
    {
        _prototype = proto;
        _noSave = true;
        LoadFromXml(XElement.Parse(component.Definition));
        _noSave = false;
    }

    public RadioDetonatorGameItemComponent(RadioDetonatorGameItemComponent rhs, IGameItem newParent,
        bool temporary = false) : base(rhs, newParent, temporary)
    {
        _prototype = rhs._prototype;
        _detonationSequence = rhs._detonationSequence;
        _switchedOn = rhs._switchedOn;
    }

    protected void LoadFromXml(XElement root)
    {
        _detonationSequence = root.Element("DetonationSequence").Value;
        _switchedOn = bool.Parse(root.Element("SwitchedOn").Value);
    }

    public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
    {
        return new RadioDetonatorGameItemComponent(this, newParent, temporary);
    }

    #endregion

    #region Saving

    protected override string SaveToXml()
    {
        return new XElement("Definition",
            new XElement("DetonationSequence", new XCData(DetonationSequence ?? string.Empty)),
            new XElement("SwitchedOn", SwitchedOn)).ToString();
    }

    #endregion

    private string _detonationSequence;

    public string DetonationSequence
    {
        get => _detonationSequence;
        set
        {
            _detonationSequence = value;
            Changed = true;
        }
    }

    #region IRecieve Implementation

    public void ReceiveTransmission(double frequency, SpokenLanguageInfo spokenLanguage, long encryption,
        ITransmit origin)
    {
        // Do nothing
    }

    public void ReceiveTransmission(double frequency, string dataTransmission, long encryption, ITransmit origin)
    {
        if (!SwitchedOn || !_powered || encryption != 0 || string.IsNullOrWhiteSpace(DetonationSequence))
        {
            return;
        }

        if (!dataTransmission.EqualTo(DetonationSequence, false))
        {
            return;
        }

        Parent.GetItemType<IDetonatable>()?.Detonate();
    }

    #endregion

    #region IConsumePower Implementation

    public double PowerConsumptionInWatts => _powered && SwitchedOn ? _prototype.PowerConsumptionInWatts : 0.0;
    private bool _powered;

    public void OnPowerCutIn()
    {
        _powered = true;
        if (SwitchedOn)
        {
            TurnOn();
        }
    }

    public void OnPowerCutOut()
    {
        _powered = false;
        if (SwitchedOn)
        {
            TurnOff();
        }
    }

    #endregion

    #region IOnOff Implementation

    private bool _switchedOn;

	public bool Armed => SwitchedOn;

	public bool CanArm(ICharacter actor, string argument)
	{
		return !Armed && string.IsNullOrWhiteSpace(argument) && Parent.IsItemType<IDetonatable>();
	}

	public string WhyCannotArm(ICharacter actor, string argument)
	{
		if (Armed)
		{
			return $"{Parent.HowSeen(actor, true)} is already armed.";
		}

		if (!Parent.IsItemType<IDetonatable>())
		{
			return $"{Parent.HowSeen(actor, true)} has no explosive payload to detonate.";
		}

		return !string.IsNullOrWhiteSpace(argument)
			? "A radio detonator does not take an arming duration or datetime."
			: $"{Parent.HowSeen(actor, true)} cannot be armed at this time.";
	}

	public bool Arm(ICharacter actor, string argument, IEmote playerEmote = null)
	{
		if (!CanArm(actor, argument))
		{
			actor.Send(WhyCannotArm(actor, argument));
			return false;
		}

		actor.OutputHandler.Handle(new MixedEmoteOutput(
			new Emote("@ arm|arms $1's radio detonator.", actor, actor, Parent)).Append(playerEmote));
		SwitchedOn = true;
		return true;
	}

	public bool CanDisarm(ICharacter actor)
	{
		return Armed;
	}

	public string WhyCannotDisarm(ICharacter actor)
	{
		return Armed
			? $"{Parent.HowSeen(actor, true)} cannot be disarmed at this time."
			: $"{Parent.HowSeen(actor, true)} is not armed.";
	}

	public bool Disarm(ICharacter actor, IEmote playerEmote = null)
	{
		if (!CanDisarm(actor))
		{
			actor.Send(WhyCannotDisarm(actor));
			return false;
		}

		actor.OutputHandler.Handle(new MixedEmoteOutput(
			new Emote("@ disarm|disarms $1's radio detonator.", actor, actor, Parent)).Append(playerEmote));
		SwitchedOn = false;
		return true;
	}

    public bool SwitchedOn
    {
        get => _switchedOn;
        set
        {
            _switchedOn = value;
            if (value)
            {
                Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
            }
            else
            {
                Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
            }

            Changed = true;
        }
    }

    #endregion

    private void TurnOff()
    {
        Parent.Handle(new EmoteOutput(new Emote(_prototype.OnPowerOffEmote, Parent, Parent)), OutputRange.Local);
    }

    private void TurnOn()
    {
        Parent.Handle(new EmoteOutput(new Emote(_prototype.OnPowerOnEmote, Parent, Parent)), OutputRange.Local);
    }

    #region ISelectable Implementation

    public bool CanSelect(ICharacter character, string argument)
    {
        StringStack ss = new(argument);
        if (!ss.PopSpeech().EqualTo("code"))
        {
            return false;
        }

        if (ss.IsFinished)
        {
            return false;
        }

        return true;
    }

    public bool Select(ICharacter character, string argument, IEmote playerEmote, bool silent = false)
    {
        StringStack ss = new(argument);
        if (!ss.PopSpeech().EqualTo("code"))
        {
            character.OutputHandler.Send(
                $"The only valid option to use with select for {Parent.HowSeen(character)} is {"code".ColourCommand()}.");
            return false;
        }

        if (ss.IsFinished)
        {
            character.OutputHandler.Send("You must specify a text code to set for this radio detonator.");
            return false;
        }

        DetonationSequence = ss.SafeRemainingArgument;
        character.OutputHandler.Handle(new MixedEmoteOutput(new Emote(
            $"@ select|selects the 'code' option on $1 and types in {DetonationSequence.ColourCommand()} as the new code",
            character, character, Parent)));
        return true;
    }

    #endregion

    #region ISwitchable Implementation

    public IEnumerable<string> SwitchSettings => new[] { "on", "off" };

    public bool CanSwitch(ICharacter actor, string setting)
    {
        // TODO - more reasons why something couldn't be switched on or off
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

    private bool SwitchOn(ICharacter actor)
    {
        SwitchedOn = true;
        Changed = true;
        return true;
    }

    private bool SwitchOff(ICharacter actor)
    {
        Changed = true;
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

    public override bool DescriptionDecorator(DescriptionType type)
    {
        return type == DescriptionType.Full || type == DescriptionType.Short;
        ;
    }

    public override string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type,
        bool colour, PerceiveIgnoreFlags flags)
    {
        switch (type)
        {
            case DescriptionType.Full:
                var state = !SwitchedOn
                    ? "disarmed".Colour(Telnet.Yellow)
                    : _powered
                        ? "armed and powered".Colour(Telnet.BoldRed)
                        : $"{"armed".Colour(Telnet.BoldRed)} but {"unpowered".ColourError()}";
                return
                    $"{description}\n\nThis item has a radio detonator.\nIt is currently {state}.\nYou can {"arm".ColourCommand()} or {"disarm".ColourCommand()} it; the existing {"switch".ColourCommand()} on/off controls are equivalent.\nYou can use {"select".ColourCommand()} to set the detonation code (see {"help select".FluentTagMXP("send", "href='help select' hint='show the helpfile for the select command'")}).";
            case DescriptionType.Short:
                if (_switchedOn)
                {
                    return _powered
                        ? $"{description} {"(armed)".Colour(Telnet.BoldRed)}"
                        : $"{description} {"(armed, unpowered)".Colour(Telnet.BoldRed)}";
                }

                break;
        }

        return description;
    }

    public override void Quit()
    {
        Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
        base.Quit();
    }

    public override void Delete()
    {
        Parent.GetItemType<IProducePower>()?.EndDrawdown(this);
        base.Delete();
    }

    public override void Login()
    {
        base.Login();
        if (SwitchedOn)
        {
            Parent.GetItemType<IProducePower>()?.BeginDrawdown(this);
        }
    }
}

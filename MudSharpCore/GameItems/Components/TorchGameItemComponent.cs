using System;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class TorchGameItemComponent : GameItemComponent, ILightable, IProduceLight
{
	protected bool _lit;
	protected TorchGameItemComponentProto _prototype;

	private int _remainingFuel;

	public int RemainingFuel
	{
		get => _remainingFuel;
		set
		{
			_remainingFuel = value;
			Changed = true;
		}
	}

	public virtual bool Lit
	{
		get => _lit;
		set
		{
			if (_lit != value)
			{
				_lit = value;
				Changed = true;
				if (_lit)
				{
					Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
				}
				else
				{
					Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
				}
			}
		}
	}

	public override void Delete()
	{
		base.Delete();
		if (Lit)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
		}
	}

	public override void Quit()
	{
		base.Quit();
		if (Lit)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManager_SecondHeartbeat;
		}
	}

	#region Overrides of GameItemComponent

	/// <inheritdoc />
	public override void Login()
	{
		base.Login();
		if (Lit)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManager_SecondHeartbeat;
		}
	}

	#endregion

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new TorchGameItemComponent(this, newParent, temporary);
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
				return
					$"{description}\n\n{(Lit ? string.Format(voyeur, "It is currently lit and looks to have {0:P0} fuel remaining.", (double)RemainingFuel / _prototype.SecondsOfFuel) : string.Format(voyeur, "It looks to have {0:P0} fuel remaining.", (double)RemainingFuel / _prototype.SecondsOfFuel))}";
		}

		throw new NotSupportedException("Invalid Decorate type in TorchGameItemComponent.Decorate");
	}

	public override int DecorationPriority => int.MaxValue;

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return RemainingFuel != _prototype.SecondsOfFuel || Lit;
	}

	public override bool HandleDieOrMorph(IGameItem newItem, ICell location)
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
		_prototype = (TorchGameItemComponentProto)newProto;
		if (_prototype.SecondsOfFuel > 0 && RemainingFuel > _prototype.SecondsOfFuel)
		{
			RemainingFuel = _prototype.SecondsOfFuel;
		}
	}

	private void HeartbeatManager_SecondHeartbeat()
	{
		RemainingFuel--;
		if (RemainingFuel == (int)(_prototype.SecondsOfFuel * 0.1))
		{
			Parent.Handle(new EmoteOutput(new Emote(_prototype.TenPercentFuelEcho, Parent, Parent)),
				OutputRange.Local);
		}

		if (RemainingFuel == 0)
		{
			Parent.Handle(new EmoteOutput(new Emote(_prototype.FuelExpendedEcho, Parent, Parent)), OutputRange.Local);
			Lit = false;
		}
	}

	#region Constructors

	public TorchGameItemComponent(TorchGameItemComponent rhs, IGameItem newParent, bool temporary = false) : base(rhs,
		newParent, temporary)
	{
		_prototype = rhs._prototype;
		RemainingFuel = rhs.RemainingFuel;
		_lit = rhs.Lit;
	}

	public TorchGameItemComponent(TorchGameItemComponentProto proto, IGameItem parent, bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		RemainingFuel = _prototype.SecondsOfFuel;
	}

	public TorchGameItemComponent(MudSharp.Models.GameItemComponent component, TorchGameItemComponentProto proto,
		IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	protected override string SaveToXml()
	{
		return
			new XElement("Definition", new XElement("RemainingFuel", RemainingFuel), new XElement("Lit", Lit))
				.ToString();
	}

	protected void LoadFromXml(XElement root)
	{
		RemainingFuel = int.Parse(root.Element("RemainingFuel").Value);
		_lit = bool.Parse(root.Element("Lit").Value);
	}

	#endregion

	#region ILightable Members

	public virtual bool CanLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		// TODO ignition sources
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return false;
		}

		return !Lit && RemainingFuel != 0;
	}

	public virtual string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if (Lit)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is already lit.";
		}

		if (RemainingFuel == 0)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is completely spent.";
		}

		throw new NotSupportedException("Invalid reason in TorchGameItemComponent.WhyCannotLight");
	}

	public bool Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)
	{
		if (!CanLight(lightee, ignitionSource))
		{
			lightee.Send(WhyCannotLight(lightee, ignitionSource));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.LightEmote, lightee, lightee, Parent, ignitionSource)).Append(
				playerEmote));
		Lit = true;
		return true;
	}

	public bool CanExtinguish(ICharacter lightee)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return false;
		}

		return Lit;
	}

	public string WhyCannotExtinguish(ICharacter lightee)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if (!Lit)
		{
			return $"You cannot extinguish {Parent.HowSeen(lightee)} because it is not lit.";
		}

		throw new NotSupportedException("Invalid reason in TorchGameItemComponent.WhyCannotExtinguish");
	}

	public bool Extinguish(ICharacter lightee, IEmote playerEmote)
	{
		if (!CanExtinguish(lightee))
		{
			lightee.Send(WhyCannotExtinguish(lightee));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote(_prototype.ExtinguishEmote, lightee, lightee, Parent)).Append(playerEmote));
		Lit = false;
		return true;
	}

	#endregion
}
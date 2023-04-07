using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Construction;

namespace MudSharp.GameItems.Components;

public class SmokeableGameItemComponent : GameItemComponent, ISmokeable
{
	protected bool _lit;
	protected SmokeableGameItemComponentProto _prototype;

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

	public override IGameItemComponentProto Prototype => _prototype;

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new SmokeableGameItemComponent(this, newParent, temporary);
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
				return
					$"{description}{(RemainingFuel > 0 ? Lit ? " (lit)".FluentColour(Telnet.Red, colour) : "" : " (spent)".FluentColour(Telnet.Yellow, colour))}";
			case DescriptionType.Full:
				return
					$"{description}\n\n{(Lit ? string.Format(voyeur, "It is currently lit and looks to have {0:P0} fuel remaining.", (double)RemainingFuel / _prototype.SecondsOfFuel) : string.Format(voyeur, "It looks to have {0:P0} fuel remaining.", (double)RemainingFuel / _prototype.SecondsOfFuel))}";
		}

		throw new NotSupportedException("Invalid Decorate type in SmokeableGameItemComponent.Decorate");
	}

	public override int DecorationPriority => int.MaxValue;

	public override bool PreventsMerging(IGameItemComponent component)
	{
		return ((component as SmokeableGameItemComponent)?.RemainingFuel ?? 0) != _prototype.SecondsOfFuel ||
		       RemainingFuel != _prototype.SecondsOfFuel || Lit;
	}

	public override bool Die(IGameItem newItem, ICell location)
	{
		var newItemLightable = newItem?.GetItemType<ILightable>();
		if (newItemLightable != null)
		{
			newItemLightable.Lit = Lit;
		}

		return false;
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (SmokeableGameItemComponentProto)newProto;
		if (RemainingFuel > _prototype.SecondsOfFuel)
		{
			RemainingFuel = _prototype.SecondsOfFuel;
		}
	}

	private void UseFuel(int amount)
	{
		var previousAmount = RemainingFuel;
		RemainingFuel -= amount;
		if (RemainingFuel <= (int)(_prototype.SecondsOfFuel * 0.1) &&
		    previousAmount > _prototype.SecondsOfFuel * 0.1)
		{
			Parent.Handle(new EmoteOutput(new Emote("@ have|has almost entirely burned up.", Parent, Parent)),
				OutputRange.Local);
		}

		if (RemainingFuel <= 0)
		{
			Parent.Handle(new EmoteOutput(new Emote("@ have|has completely burned up.", Parent, Parent)),
				OutputRange.Local);
			Lit = false;
			RemainingFuel = 0;
		}

		var character = Parent.GetItemType<IHoldable>()?.HeldBy?.Actor;
		if (character != null && !string.IsNullOrWhiteSpace(_prototype.PlayerDescriptionEffectString))
		{
			var playerEffect =
				character
					.EffectsOfType<IDescriptionAdditionEffect>()
					.FirstOrDefault(
						x => !x.PlayerSet && x.GetAdditionalText(character, false)
						                      .Equals(_prototype.PlayerDescriptionEffectString));
			if (playerEffect == null)
			{
				playerEffect = new DescriptionAddition(character, _prototype.PlayerDescriptionEffectString,
					false, Telnet.Yellow);
				character.AddEffect(playerEffect,
					TimeSpan.FromSeconds(amount * _prototype.SecondsOfEffectPerSecondOfFuel));
			}
			else
			{
				Gameworld.EffectScheduler.ExtendSchedule(playerEffect,
					TimeSpan.FromSeconds(amount * _prototype.SecondsOfEffectPerSecondOfFuel));
			}
		}

		var location = Parent.TrueLocations.FirstOrDefault();
		if (location != null && Parent.ContainedIn == null &&
		    !string.IsNullOrWhiteSpace(_prototype.RoomDescriptionEffectString))
		{
			var roomEffect = location.EffectsOfType<IDescriptionAdditionEffect>()
			                         .FirstOrDefault(x =>
				                         x.GetAdditionalText(character, false)
				                          .Equals(_prototype.RoomDescriptionEffectString));
			if (roomEffect == null)
			{
				roomEffect = new DescriptionAddition(location, _prototype.RoomDescriptionEffectString,
					false, Telnet.Yellow);
				location.AddEffect(roomEffect,
					TimeSpan.FromSeconds(amount * _prototype.SecondsOfEffectPerSecondOfFuel));
			}
			else
			{
				Gameworld.EffectScheduler.ExtendSchedule(roomEffect,
					TimeSpan.FromSeconds(amount * _prototype.SecondsOfEffectPerSecondOfFuel));
			}
		}
	}

	private void HeartbeatManager_SecondHeartbeat()
	{
		UseFuel(1);
	}

	#region Constructors

	public SmokeableGameItemComponent(SmokeableGameItemComponent rhs, IGameItem newParent, bool temporary = false) :
		base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		RemainingFuel = rhs.RemainingFuel;
		Lit = rhs.Lit;
	}

	public SmokeableGameItemComponent(SmokeableGameItemComponentProto proto, IGameItem parent,
		bool temporary = false)
		: base(parent, proto, temporary)
	{
		_prototype = proto;
		RemainingFuel = _prototype.SecondsOfFuel;
	}

	public SmokeableGameItemComponent(MudSharp.Models.GameItemComponent component,
		SmokeableGameItemComponentProto proto,
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
		RemainingFuel = int.Parse(root.Element("RemainingFuel")?.Value ?? "0");
		Lit = bool.Parse(root.Element("Lit")?.Value ?? "false");
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

		return (Parent.GetItemType<IStackable>()?.Quantity ?? 0) <= 1 && !Lit && RemainingFuel != 0;
	}

	public virtual string WhyCannotLight(ICharacter lightee, IPerceivable ignitionSource)
	{
		if (!(Parent.Location?.CanGetAccess(Parent, lightee) ?? true))
		{
			return Parent.Location.WhyCannotGetAccess(Parent, lightee);
		}

		if ((Parent.GetItemType<IStackable>()?.Quantity ?? 0) > 1)
		{
			return
				$"You cannot light {Parent.HowSeen(lightee)} until you have just one in your possession. Pull one off the stack.";
		}

		if (Lit)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is already lit.";
		}

		if (RemainingFuel == 0)
		{
			return $"You cannot light {Parent.HowSeen(lightee)} because it is completely spent.";
		}

		throw new NotSupportedException("Invalid reason in SmokeableGameItemComponent.WhyCannotLight");
	}

	public bool Light(ICharacter lightee, IPerceivable ignitionSource, IEmote playerEmote)
	{
		if (!CanLight(lightee, ignitionSource))
		{
			lightee.Send(WhyCannotLight(lightee, ignitionSource));
			return false;
		}

		lightee.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ light|lights $1$?2| with $2||$", lightee, lightee, Parent,
				ignitionSource)).Append(playerEmote));
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
			new MixedEmoteOutput(new Emote("@ put|puts $1 out", lightee, lightee, Parent)).Append(playerEmote));
		Lit = false;
		return true;
	}

	#endregion

	#region Implementation of ISmokeable

	public string WhyCannotSmoke(ICharacter character)
	{
		if (!Lit)
		{
			return $"You cannot smoke {Parent.HowSeen(character)} because it is not lit.";
		}

		throw new NotImplementedException("Invalid WhyCannotSmoke reason in SmokeableGameItemcomponent.");
	}

	public bool CanSmoke(ICharacter character)
	{
		return Lit;
	}

	public bool Smoke(ICharacter character, IEmote playerEmote)
	{
		if (!CanSmoke(character))
		{
			character.Send(WhyCannotSmoke(character));
			return false;
		}

		character.OutputHandler.Handle(
			new MixedEmoteOutput(new Emote("@ take|takes a drag from $0", character, Parent)).Append(playerEmote));
		_prototype.OnDragProg?.Execute(character, Parent);
		if (_prototype.Drug != null)
		{
			character.Body.Dose(_prototype.Drug, DrugVector.Inhaled, _prototype.GramsPerDrag);
		}

		UseFuel(_prototype.SecondsPerDrag);
		return true;
	}

	#endregion
}
#nullable enable
using System;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems.Components;

public class ConsumableHeaterCoolerGameItemComponent : ThermalSourceGameItemComponent
{
	public ConsumableHeaterCoolerGameItemComponent(ConsumableHeaterCoolerGameItemComponentProto proto, IGameItem parent,
		bool temporary = false) : base(parent, proto, temporary)
	{
		_prototype = proto;
		RemainingFuelSeconds = proto.SecondsOfFuel;
		SubscribeHeartbeat();
	}

	public ConsumableHeaterCoolerGameItemComponent(Models.GameItemComponent component,
		ConsumableHeaterCoolerGameItemComponentProto proto, IGameItem parent) : base(component, parent)
	{
		_prototype = proto;
		_noSave = true;
		LoadFromXml(XElement.Parse(component.Definition));
		_noSave = false;
	}

	public ConsumableHeaterCoolerGameItemComponent(ConsumableHeaterCoolerGameItemComponent rhs, IGameItem newParent,
		bool temporary = false) : base(rhs, newParent, temporary)
	{
		_prototype = rhs._prototype;
		RemainingFuelSeconds = rhs.RemainingFuelSeconds;
	}

	private ConsumableHeaterCoolerGameItemComponentProto _prototype;
	public override IGameItemComponentProto Prototype => _prototype;
	protected override ThermalSourceGameItemComponentProto ThermalPrototype => _prototype;
	protected override bool IsProducingHeat => RemainingFuelSeconds > 0;

	public int RemainingFuelSeconds { get; private set; }

	public override IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
	{
		return new ConsumableHeaterCoolerGameItemComponent(this, newParent, temporary);
	}

	protected override void UpdateComponentNewPrototype(IGameItemComponentProto newProto)
	{
		_prototype = (ConsumableHeaterCoolerGameItemComponentProto)newProto;
		if (RemainingFuelSeconds > _prototype.SecondsOfFuel)
		{
			RemainingFuelSeconds = _prototype.SecondsOfFuel;
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition", new XElement("RemainingFuelSeconds", RemainingFuelSeconds)).ToString();
	}

	private void LoadFromXml(XElement root)
	{
		RemainingFuelSeconds = int.Parse(root.Element("RemainingFuelSeconds")?.Value ?? _prototype.SecondsOfFuel.ToString());
	}

	public override void Delete()
	{
		UnsubscribeHeartbeat();
		base.Delete();
	}

	public override void Quit()
	{
		UnsubscribeHeartbeat();
		base.Quit();
	}

	public override void Login()
	{
		base.Login();
		SubscribeHeartbeat();
	}

	internal void AdvanceSeconds(int seconds)
	{
		if (RemainingFuelSeconds <= 0)
		{
			return;
		}

		RemainingFuelSeconds = Math.Max(0, RemainingFuelSeconds - seconds);
		Changed = true;
		if (RemainingFuelSeconds > 0)
		{
			return;
		}

		Parent.Handle(new EmoteOutput(new Emote(_prototype.FuelExpendedEcho, Parent, Parent)), OutputRange.Local);
		Expire();
	}

	private void SubscribeHeartbeat()
	{
		if (RemainingFuelSeconds > 0)
		{
			Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
			Gameworld.HeartbeatManager.SecondHeartbeat += HeartbeatManagerOnSecondHeartbeat;
		}
	}

	private void UnsubscribeHeartbeat()
	{
		Gameworld.HeartbeatManager.SecondHeartbeat -= HeartbeatManagerOnSecondHeartbeat;
	}

	private void HeartbeatManagerOnSecondHeartbeat()
	{
		AdvanceSeconds(1);
	}

	private void Expire()
	{
		UnsubscribeHeartbeat();
		if (_prototype.SpentItemProto is null)
		{
			Parent.Delete();
			return;
		}

		var newItem = _prototype.SpentItemProto.CreateNew();
		var location = Parent.TrueLocations.FirstOrDefault();
		Parent.InInventoryOf?.SwapInPlace(Parent, newItem);
		Parent.ContainedIn?.SwapInPlace(Parent, newItem);
		newItem.RoomLayer = Parent.RoomLayer;
		Parent.Location?.Insert(newItem);
		foreach (var effect in Parent.Effects)
		{
			var newEffect = effect.NewEffectOnItemMorph(Parent, newItem);
			if (newEffect == null)
			{
				continue;
			}

			if (Gameworld.EffectScheduler.IsScheduled(effect))
			{
				newItem.AddEffect(newEffect, Gameworld.EffectScheduler.RemainingDuration(effect));
			}
			else
			{
				newItem.AddEffect(newEffect);
			}
		}

		foreach (var comp in Parent.Components)
		{
			comp.HandleDieOrMorph(newItem, location);
		}

		newItem.Login();
		Parent.Delete();
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.GameItems;

public partial class GameItem : IHaveWounds
{
	#region IHaveWounds Members

	public IHealthStrategy HealthStrategy => Prototype.HealthStrategy;

	private IOverrideItemWoundBehaviour _overridingWoundBehaviourComponent;

	private readonly List<IWound> _wounds = new();

	public IEnumerable<IWound> Wounds =>
		_overridingWoundBehaviourComponent?.Wounds ??
		_wounds;

	public IEnumerable<IWound> VisibleWounds(IPerceiver voyeur, WoundExaminationType examinationType)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			return _overridingWoundBehaviourComponent.VisibleWounds(voyeur, examinationType);
		}

		switch (examinationType)
		{
			case WoundExaminationType.Glance:
				return _wounds.Where(x => x.Severity > WoundSeverity.Minor).ToList();
			case WoundExaminationType.Look:
				return _wounds.Where(x => x.Severity > WoundSeverity.Superficial).ToList();
		}

		return _wounds;
	}

	public void ProcessPassiveWound(IWound wound)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			_overridingWoundBehaviourComponent.ProcessPassiveWound(wound);
			OnWounded?.Invoke(this, wound);
			StartHealthTick();
			return;
		}

		OnWounded?.Invoke(this, wound);
		var pile = GetItemType<PileGameItemComponent>();
		if (pile != null)
		{
			OutputHandler.Handle(
				new EmoteOutput(new Emote("The cohesion of @ is disrupted by being damaged, and it separates.", this)));
			Die();
		}

		StartHealthTick();
	}

	public void AddWound(IWound wound)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			_overridingWoundBehaviourComponent.AddWound(wound);
			return;
		}

		if (!_wounds.Contains(wound))
		{
			_wounds.Add(wound);
		}
	}

	public void AddWounds(IEnumerable<IWound> wounds)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			_overridingWoundBehaviourComponent.AddWounds(wounds);
			return;
		}

		foreach (var wound in wounds)
		{
			if (_wounds.Contains(wound))
			{
				_wounds.Add(wound);
			}
		}

		Changed = true;
	}

	public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
	{
		if (damage == null)
		{
			return Enumerable.Empty<IWound>();
		}

		if (_overridingWoundBehaviourComponent != null)
		{
			return _overridingWoundBehaviourComponent.PassiveSufferDamage(damage);
		}

		var pile = GetItemType<PileGameItemComponent>();
		if (pile is not null)
		{
			return pile.Contents.GetRandomElement().PassiveSufferDamage(damage);
		}

		var destroyable = GetItemType<IDestroyable>();
		if (destroyable == null)
		{
			return Enumerable.Empty<IWound>();
		}

		var resistance = GetItemType<INaturalResistance>();
		if (resistance != null)
		{
			damage = resistance.SufferDamage(damage, new List<IWound>());
		}

		damage = destroyable?.GetActualDamage(damage) ?? damage;
		var wounds = new List<IWound>();
		var newWounds = HealthStrategy.SufferDamage(this, damage, null).ToList();
		foreach (var newWound in newWounds.ToArray())
		{
			if (!_wounds.Contains(newWound))
			{
				_wounds.Add(newWound);
			}

			wounds.Add(newWound);
		}

		return wounds;
	}

	public IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			return _overridingWoundBehaviourComponent.PassiveSufferDamage(damage, proximity, facing);
		}

		return PassiveSufferDamageViaContainedItem(damage, proximity, facing, null);
	}

	public IEnumerable<IWound> PassiveSufferDamageViaContainedItem(IExplosiveDamage damage, Proximity proximity,
		Facing facing, IGameItem source)
	{
		// Hard Cover has a chance of avoiding explosions entirely unless it's right up against the character
		if (source == null && proximity != Proximity.Intimate &&
		    Cover?.Cover.CoverType == MudSharp.Combat.CoverType.Hard)
		{
			switch (Cover.Cover.CoverExtent)
			{
				case MudSharp.Combat.CoverExtent.Marginal:
					if (RandomUtilities.Roll(1.0, Gameworld.GetStaticDouble("ExplosionMarginalCoverAbsorbChance")))
					{
						return Enumerable.Empty<IWound>();
					}

					break;
				case MudSharp.Combat.CoverExtent.Partial:
					if (RandomUtilities.Roll(1.0, Gameworld.GetStaticDouble("ExplosionPartialCoverAbsorbChance")))
					{
						return Enumerable.Empty<IWound>();
					}

					break;
				case MudSharp.Combat.CoverExtent.NearTotal:
					if (RandomUtilities.Roll(1.0, Gameworld.GetStaticDouble("ExplosionNearTotalCoverAbsorbChance")))
					{
						return Enumerable.Empty<IWound>();
					}

					break;
				case MudSharp.Combat.CoverExtent.Total:
					if (RandomUtilities.Roll(1.0, Gameworld.GetStaticDouble("ExplosionTotalCoverAbsorbChance")))
					{
						return Enumerable.Empty<IWound>();
					}

					break;
			}
		}

		var damageFactor = 1.0;
		var score = Size - damage.ExplosionSize + (int)proximity + ((int?)Cover?.Cover.CoverExtent ?? 0) *
			(Cover?.Cover.CoverType == MudSharp.Combat.CoverType.Hard ? 1 : 0);
		if (score >= 0)
		{
			switch (score)
			{
				case 0:
					damageFactor = 0.75;
					break;
				case 1:
					damageFactor = 0.6;
					break;
				case 2:
					damageFactor = 0.45;
					break;
				case 3:
					damageFactor = 0.3;
					break;
				case 4:
					damageFactor = 0.15;
					break;
				default:
					damageFactor = 0.05;
					break;
			}
		}

		// Note - facing is irrelevant for Game Items
		var damageToPassOn = new ExplosiveDamage(damage, damageFactor);
		damage = new ExplosiveDamage(damage, 1.0, null);

		var wounds = new List<IWound>();
		if (damageToPassOn.ExplodingFromInside)
		{
			if (GetItemType<IContainer>() is IContainer container &&
			    container.Contents.Contains(damageToPassOn.InternalExplosionSource))
			{
				foreach (var item in container.Contents)
				{
					if (item == source)
					{
						continue;
					}

					wounds.AddRange(
						item.PassiveSufferDamageViaContainedItem(damage, Proximity.Intimate, Facing.Front, null));
				}
			}

			if (GetItemType<IArmour>() is IArmour armour)
			{
				var damages = damageToPassOn.ReferenceDamages
				                            .SelectNotNull(x =>
					                            armour.ArmourType.AbsorbDamage(x, armour, this, ref wounds, true))
				                            .ToList();
				damage = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity);
				damageToPassOn = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity,
					damageToPassOn.ExplodingFromInside, damageToPassOn.InternalExplosionSource);
			}
			else
			{
				foreach (var subDamage in damage.ReferenceDamages)
				{
					wounds.AddRange(PassiveSufferDamage(subDamage));
				}
			}
		}

		foreach (var item in GetItemType<IBelt>()?.ConnectedItems.Where(x => x.Parent != source).ToList() ??
		                     Enumerable.Empty<IBeltable>())
		{
			wounds.AddRange(
				item.Parent.PassiveSufferDamageViaContainedItem(damage, Proximity.Intimate, Facing.Front, source));
		}

		if (GetItemType<IBeltable>() is IBeltable beltable && beltable.ConnectedTo != null &&
		    beltable.ConnectedTo?.Parent != source)
		{
			wounds.AddRange(beltable.ConnectedTo.Parent.PassiveSufferDamageViaContainedItem(damageToPassOn,
				Proximity.Intimate, Facing.Front, this));
		}

		foreach (var item in GetItemType<IConnectable>()?.ConnectedItems
		                                                .Where(x => !x.Item2.Independent && x.Item2.Parent != source)
		                                                .ToList() ??
		                     Enumerable.Empty<Tuple<ConnectorType, IConnectable>>())
		{
			wounds.AddRange(
				item.Item2.Parent.PassiveSufferDamageViaContainedItem(damage, Proximity.Intimate, Facing.Front,
					source));
		}

		foreach (var item in Wounds.SelectNotNull(x => x.Lodged).Where(x => x != source).ToList())
		{
			wounds.AddRange(item.PassiveSufferDamageViaContainedItem(damage, Proximity.Intimate, Facing.Front, source));
		}

		foreach (var theLock in GetItemType<ILockable>()?.Locks.Where(x => x.Parent != source) ??
		                        Enumerable.Empty<ILock>())
		{
			wounds.AddRange(
				theLock.Parent.PassiveSufferDamageViaContainedItem(damage, Proximity.Intimate, Facing.Front, source));
		}

		if (!damageToPassOn.ExplodingFromInside)
		{
			if (GetItemType<IArmour>() is IArmour armour)
			{
				var damages = damageToPassOn.ReferenceDamages
				                            .SelectNotNull(x =>
					                            armour.ArmourType.AbsorbDamage(x, armour, this, ref wounds, true))
				                            .ToList();
				damage = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity);
				damageToPassOn = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity,
					damageToPassOn.ExplodingFromInside, damageToPassOn.InternalExplosionSource);
			}
			else
			{
				foreach (var subDamage in damage.ReferenceDamages)
				{
					wounds.AddRange(PassiveSufferDamage(subDamage));
				}
			}
		}

		if (damageToPassOn.ExplodingFromInside)
		{
			// Exploding Outwards
			if (ContainedIn != null)
			{
				wounds.AddRange(ContainedIn.PassiveSufferDamageViaContainedItem(damageToPassOn, Proximity.Intimate,
					Facing.Front, this));
			}
			else if (InInventoryOf != null)
			{
				wounds.AddRange(InInventoryOf.InventoryExploded(this, damageToPassOn));
			}
			else if (TrueLocations.Any())
			{
				wounds.AddRange(ExplosionEmantingFromPerceivable(damageToPassOn));
			}
		}
		else
		{
			// Exploding Inwards
			if (GetItemType<IContainer>() is IContainer container)
			{
				foreach (var item in container.Contents)
				{
					wounds.AddRange(
						item.PassiveSufferDamageViaContainedItem(damage, Proximity.Intimate, Facing.Front, null));
				}
			}
		}

		return wounds;
	}

	public IEnumerable<IWound> SufferDamage(IDamage damage)
	{
		var wounds = PassiveSufferDamage(damage).ToArray();
		wounds.ProcessPassiveWounds();
		StartHealthTick();
		return wounds;
	}

	public WoundSeverity GetSeverityFor(IWound wound)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			return _overridingWoundBehaviourComponent.GetSeverityFor(wound);
		}

		return HealthStrategy.GetSeverityFor(wound, this);
	}

	public double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			return _overridingWoundBehaviourComponent.GetSeverityFloor(severity, usePercentageModel);
		}

		return HealthStrategy.GetSeverityFloor(severity, usePercentageModel);
	}

	public void EvaluateWounds()
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			_overridingWoundBehaviourComponent.EvaluateWounds();
			return;
		}

		if (Wounds.Any(x => x.Severity == WoundSeverity.None))
		{
			var woundsToRemove = Wounds.Where(x => x.Severity == WoundSeverity.None).ToList();
			foreach (var wound in woundsToRemove)
			{
				wound.Delete();
				_wounds.Remove(wound);
				OnRemoveWound?.Invoke(this, wound);
			}
		}

		if (!Wounds.Any())
		{
			EndHealthTick();
		}
	}

	public void StartHealthTick(bool initial = false)
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			_overridingWoundBehaviourComponent.StartHealthTick(initial);
			return;
		}

		if (Destroyed)
		{
			return;
		}

		if (Wounds.Any())
		{
			Gameworld.HeartbeatManager.TenSecondHeartbeat += HealthTick_TenSecondHeartbeat;
		}
	}

	private void HealthTick_TenSecondHeartbeat()
	{
		var result = HealthStrategy.PerformHealthTick(this);
		if (result == HealthTickResult.Dead)
		{
			Die();
		}
	}

	public void EndHealthTick()
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			_overridingWoundBehaviourComponent.EndHealthTick();
			return;
		}

		Gameworld.HeartbeatManager.TenSecondHeartbeat -= HealthTick_TenSecondHeartbeat;
	}

	public void CureAllWounds()
	{
		if (_overridingWoundBehaviourComponent != null)
		{
			_overridingWoundBehaviourComponent.CureAllWounds();
			return;
		}

		foreach (var wound in Wounds)
		{
			wound.Delete();
			OnHeal?.Invoke(this, wound);
			OnRemoveWound?.Invoke(this, wound);
		}

		_wounds.Clear();
		EndHealthTick();
	}

	#endregion

	#region IMortal Members

	public event WoundEvent OnWounded;
	public event WoundEvent OnHeal;
	public event WoundEvent OnRemoveWound;
	public event PerceivableEvent OnDeath;

	public IGameItem Die()
	{
		if (InInventoryOf == null)
		{
			OutputHandler.Handle(new EmoteOutput(new Emote("@ have|has been destroyed!", this)));
		}
		else
		{
			if (!AffectedBy<SupressWoundMessages>())
			{
				OutputHandler.Handle(new EmoteOutput(new Emote("$1's !0 $0|have|has been destroyed!", this, this,
					InInventoryOf.Actor)));
			}
			else
			{
				InInventoryOf.OutputHandler.Send(new EmoteOutput(new Emote("Your !0 has been destroyed!", this)));
			}
		}

		OnDeath?.Invoke(this);

		// TODO - customisable message
		var newItem = Prototype.LoadDestroyedItem(this) as GameItem;
		// Component.Die can affect TrueLocation, so save it beforehand
		var originalTrueLocation = TrueLocations.FirstOrDefault();
		if (originalTrueLocation == null)
		{
			Console.WriteLine($"Item {Id} ({HowSeen(this, colour: false)}) did not have a location.");
		}

		var locationChanged = false;
		foreach (var component in Components.OrderBy(x => x.ComponentDieOrder))
		{
			if (component.HandleDieOrMorph(newItem, originalTrueLocation))
			{
				locationChanged = true;
			}
		}

		Destroyed = true;
		EndHealthTick();

		if (newItem == null)
		{
			Delete();
			return null;
		}

		foreach (var target in TargetedBy.ToList())
		{
			target.SetTarget(newItem);
		}

		Gameworld.Add(newItem);
		if (locationChanged)
		{
			Delete();
			return newItem;
		}

		if (ContainedIn?.SwapInPlace(this, newItem) == true)
		{
			Delete();
			return newItem;
		}

		Delete();
		newItem.RoomLayer = RoomLayer;
		originalTrueLocation?.Insert(newItem);
		newItem.Login();
		newItem.HandleEvent(EventType.ItemFinishedLoading, newItem);
		return newItem;
	}

	public ICharacter Resurrect(ICell location)
	{
		return null;
	}

	public void CheckHealthStatus()
	{
		var result = HealthStrategy.EvaluateStatus(this);
		if (result == HealthTickResult.Dead)
		{
			Die();
		}
	}

	#endregion
}
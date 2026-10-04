using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;

namespace MudSharp.GameItems;

public partial class GameItem : IHaveWounds
{
	internal void NotifyImplantAttributeCapacityChange()
	{
		if (_components is not { Count: > 0 }) return;
		foreach (var implant in GetItemTypes<IImplantTraitChange>())
		{
			if (implant.InstalledBody?.Actor is { } actor) actor.Gameworld.MagicCasting?.NotifyCapacityChange(actor);
		}
	}

    #region IHaveWounds Members

    public IHealthStrategy HealthStrategy => Prototype.HealthStrategy;

    private IOverrideItemWoundBehaviour _overridingWoundBehaviourComponent;

    private bool _deferredInitialHealthTick;

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

        HandleEvent(EventType.ItemDamaged, this, wound.ToolOrigin, wound.ActorOrigin);
        foreach (IHandleEvents witness in TrueLocations.SelectMany(x => x.EventHandlersFor(this)))
        {
            witness.HandleEvent(EventType.ItemDamagedWitness, this, wound.ToolOrigin, wound.ActorOrigin, witness);
        }

        PileGameItemComponent pile = GetItemType<PileGameItemComponent>();
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
			NotifyImplantAttributeCapacityChange();
            return;
        }

        if (!_wounds.Contains(wound))
        {
            _wounds.Add(wound);
        }
		NotifyImplantAttributeCapacityChange();
    }

    public void AddWounds(IEnumerable<IWound> wounds)
    {
        if (_overridingWoundBehaviourComponent != null)
        {
            _overridingWoundBehaviourComponent.AddWounds(wounds);
			NotifyImplantAttributeCapacityChange();
            return;
        }

        foreach (IWound wound in wounds)
        {
            if (_wounds.Contains(wound))
            {
                _wounds.Add(wound);
            }
        }

        Changed = true;
		NotifyImplantAttributeCapacityChange();
    }

    public bool TryTransferWoundTo(IWound wound, IHaveWounds newOwner, IBodypart newBodypart,
        IBodypart newSeveredBodypart = null)
    {
        if (_overridingWoundBehaviourComponent != null)
        {
            return _overridingWoundBehaviourComponent.TryTransferWoundTo(wound, newOwner, newBodypart,
                newSeveredBodypart);
        }

        if (wound == null || newOwner == null || !_wounds.Contains(wound))
        {
            return false;
        }

        _wounds.Remove(wound);
		NotifyImplantAttributeCapacityChange();
        wound.RemapTo(newOwner, newBodypart, newSeveredBodypart);
        newOwner.AddWound(wound);
        Changed = true;
        newOwner.StartHealthTick();
        StartHealthTick();
        EvaluateWounds();
        newOwner.EvaluateWounds();
        return true;
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

        PileGameItemComponent pile = GetItemType<PileGameItemComponent>();
        if (pile is not null)
        {
            return pile.Contents.GetRandomElement().PassiveSufferDamage(damage);
        }

        IDestroyable destroyable = GetItemType<IDestroyable>();
        if (destroyable == null)
        {
            return Enumerable.Empty<IWound>();
        }

        INaturalResistance resistance = GetItemType<INaturalResistance>();
        if (resistance != null)
        {
            damage = resistance.SufferDamage(damage, new List<IWound>());
        }

        List<IWound> wounds = new();
        damage = ApplyMagicArmourEnhancements(damage, wounds, true);
        if (damage is null)
        {
            return wounds;
        }

        damage = destroyable?.GetActualDamage(damage) ?? damage;
        List<IWound> newWounds = HealthStrategy.SufferDamage(this, damage, null).ToList();
        foreach (IWound newWound in newWounds.ToArray())
        {
            if (!_wounds.Contains(newWound))
            {
                _wounds.Add(newWound);
            }

            wounds.Add(newWound);
        }

		NotifyImplantAttributeCapacityChange();
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
		return PassiveSufferDamageViaContainedItemInternal(damage, proximity, facing, source,
			new HashSet<IGameItem>(ReferenceEqualityComparer.Instance));
	}

	private static IEnumerable<IWound> RecurseExplosion(IGameItem item, IExplosiveDamage damage,
		Proximity proximity, Facing facing, IGameItem source, HashSet<IGameItem> visited)
	{
		return item is GameItem gameItem
			? gameItem.PassiveSufferDamageViaContainedItemInternal(damage, proximity, facing, source, visited)
			: item.PassiveSufferDamageViaContainedItem(damage, proximity, facing, source);
	}

	private IEnumerable<IWound> PassiveSufferDamageViaContainedItemInternal(IExplosiveDamage damage,
		Proximity proximity, Facing facing, IGameItem source, HashSet<IGameItem> visited)
    {
		if (!visited.Add(this))
		{
			return Enumerable.Empty<IWound>();
		}

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

        double damageFactor = 1.0;
        int score = Size - damage.ExplosionSize + (int)proximity + ((int?)Cover?.Cover.CoverExtent ?? 0) *
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
        ExplosiveDamage damageToPassOn = new(damage, damageFactor);
        damage = new ExplosiveDamage(damage, 1.0, null);

        List<IWound> wounds = new();
        if (damageToPassOn.ExplodingFromInside)
        {
            if (GetItemType<IContainer>() is IContainer container &&
                container.Contents.Contains(damageToPassOn.InternalExplosionSource))
            {
                foreach (IGameItem item in container.Contents)
                {
                    if (item == source)
                    {
                        continue;
                    }

                    wounds.AddRange(
						RecurseExplosion(item, damage, Proximity.Intimate, Facing.Front, null, visited));
                }
            }

            if (GetItemType<IArmour>() is IArmour armour)
            {
                List<IDamage> damages = damageToPassOn.ReferenceDamages
                                            .SelectNotNull(x =>
                                                armour.ArmourType.AbsorbDamage(x, armour, this, ref wounds, true))
                                            .SelectNotNull(x => ApplyMagicArmourEnhancements(x, wounds, true))
                                            .ToList();
                damage = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity);
                damageToPassOn = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity,
                    damageToPassOn.ExplodingFromInside, damageToPassOn.InternalExplosionSource);
            }
            else
            {
                foreach (IDamage subDamage in damage.ReferenceDamages)
                {
                    wounds.AddRange(PassiveSufferDamage(subDamage));
                }
            }
        }

        foreach (IBeltable item in GetItemType<IBelt>()?.ConnectedItems.Where(x => x.Parent != source).ToList() ??
                             Enumerable.Empty<IBeltable>())
        {
            wounds.AddRange(
				RecurseExplosion(item.Parent, damage, Proximity.Intimate, Facing.Front, source, visited));
        }

        if (GetItemType<IBeltable>() is IBeltable beltable && beltable.ConnectedTo != null &&
            beltable.ConnectedTo?.Parent != source)
        {
			wounds.AddRange(RecurseExplosion(beltable.ConnectedTo.Parent, damageToPassOn,
				Proximity.Intimate, Facing.Front, this, visited));
        }

        foreach (Tuple<ConnectorType, IConnectable> item in GetItemType<IConnectable>()?.ConnectedItems
                                                        .Where(x => !x.Item2.Independent && x.Item2.Parent != source)
                                                        .ToList() ??
                             Enumerable.Empty<Tuple<ConnectorType, IConnectable>>())
        {
            wounds.AddRange(
				RecurseExplosion(item.Item2.Parent, damage, Proximity.Intimate, Facing.Front, source, visited));
        }

        foreach (IGameItem item in Wounds.SelectNotNull(x => x.Lodged).Where(x => x != source).ToList())
        {
			wounds.AddRange(RecurseExplosion(item, damage, Proximity.Intimate, Facing.Front, source, visited));
        }

        foreach (ILock theLock in GetItemType<ILockable>()?.Locks.Where(x => x.Parent != source) ??
                                Enumerable.Empty<ILock>())
        {
            wounds.AddRange(
				RecurseExplosion(theLock.Parent, damage, Proximity.Intimate, Facing.Front, source, visited));
        }

        if (!damageToPassOn.ExplodingFromInside)
        {
            if (GetItemType<IArmour>() is IArmour armour)
            {
                List<IDamage> damages = damageToPassOn.ReferenceDamages
                                            .SelectNotNull(x =>
                                                armour.ArmourType.AbsorbDamage(x, armour, this, ref wounds, true))
                                            .SelectNotNull(x => ApplyMagicArmourEnhancements(x, wounds, true))
                                            .ToList();
                damage = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity);
                damageToPassOn = new ExplosiveDamage(damages, 0.0, damage.ExplosionSize, damage.MaximumProximity,
                    damageToPassOn.ExplodingFromInside, damageToPassOn.InternalExplosionSource);
            }
            else
            {
                foreach (IDamage subDamage in damage.ReferenceDamages)
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
				wounds.AddRange(RecurseExplosion(ContainedIn, damageToPassOn,
					Proximity.Intimate, Facing.Front, this, visited));
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
                foreach (IGameItem item in container.Contents)
                {
                    wounds.AddRange(
						RecurseExplosion(item, damage, Proximity.Intimate, Facing.Front, null, visited));
                }
            }
        }

        return wounds;
    }

    public IEnumerable<IWound> SufferDamage(IDamage damage)
    {
        IWound[] wounds = PassiveSufferDamage(damage).ToArray();
        wounds.ProcessPassiveWounds();
        StartHealthTick();
        return wounds;
    }

    private IDamage ApplyMagicArmourEnhancements(IDamage damage, List<IWound> wounds, bool passive)
    {
        foreach (IMagicArmourEnhancementEffect effect in EffectsOfType<IMagicArmourEnhancementEffect>(x => x.Applies()).ToList())
        {
            damage = passive
                ? effect.PassiveSufferDamage(damage, ref wounds)
                : effect.SufferDamage(damage, ref wounds);
            if (damage is null)
            {
                return null;
            }
        }

        return damage;
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
			NotifyImplantAttributeCapacityChange();
            return;
        }

        if (Wounds.Any(x => x.Severity == WoundSeverity.None))
        {
            List<IWound> woundsToRemove = Wounds.Where(x => x.Severity == WoundSeverity.None).ToList();
            foreach (IWound wound in woundsToRemove)
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
		NotifyImplantAttributeCapacityChange();
    }

    public void StartHealthTick(bool initial = false)
    {
        if (_overridingWoundBehaviourComponent != null)
        {
            _overridingWoundBehaviourComponent.StartHealthTick(initial);
			NotifyImplantAttributeCapacityChange();
            return;
        }

		NotifyImplantAttributeCapacityChange();
        if (Destroyed)
        {
            return;
        }

        if (!HealthStrategy.RequiresPeriodicHealthTick)
        {
            Gameworld.HeartbeatManager.TenSecondHeartbeat -= HealthTick_TenSecondHeartbeat;
            return;
        }

		if (Wounds.Any())
        {
            Gameworld.HeartbeatManager.TenSecondHeartbeat += HealthTick_TenSecondHeartbeat;
        }
    }

    private void HealthTick_TenSecondHeartbeat()
    {
        HealthTickResult result = HealthStrategy.PerformHealthTick(this);
		NotifyImplantAttributeCapacityChange();
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
			NotifyImplantAttributeCapacityChange();
            return;
        }

        foreach (IWound wound in Wounds)
        {
            wound.Delete();
            OnHeal?.Invoke(this, wound);
            OnRemoveWound?.Invoke(this, wound);
        }

        _wounds.Clear();
        EndHealthTick();
		NotifyImplantAttributeCapacityChange();
    }

    #endregion

    #region IMortal Members

    public event WoundEvent OnWounded;
    public event WoundEvent OnHeal;
    public event WoundEvent OnRemoveWound;
    public event PerceivableEvent OnDeath;

    public IGameItem Die()
    {
		if (Deleted || Destroyed) return null;
		if (Gameworld?.SpellOwnedCorpseAnimations is { } animations && GetItemType<ICorpse>() is not null && animations.IsBorrowedCorpse(Id)) return this;
		if (SpellCreationOrigin?.IsTemporary == true)
		{
			if (Gameworld.SpellOwnedItems?.TryPrepareRemoval(this, out _) != true) return this;
			if (!_spellOwnedDeathObserversNotified)
			{
				_spellOwnedDeathObserversNotified = true;
				OnDeath?.Invoke(this);
			}
			if (Gameworld.SpellOwnedItems.TryPrepareRemoval(this, out _) != true) return this;
			Delete();
			if (!Deleted) return this;
			Destroyed = true; EndHealthTick(); return null;
		}
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
        GameItem newItem = Prototype.LoadDestroyedItem(this) as GameItem;
        newItem?.CopyOwnerFrom(this);
        // Component.Die can affect TrueLocation, so save it beforehand
        ICell originalTrueLocation = TrueLocations.FirstOrDefault();
		var originalSpatialLocation = originalTrueLocation is null
			? (SpatialLocation?)null
			: CaptureComponentLifecycleSpatialLocation(originalTrueLocation);
        if (originalTrueLocation == null)
        {
            Console.WriteLine($"Item {Id} ({HowSeen(this, colour: false)}) did not have a location.");
        }

        bool locationChanged = false;
		if (!SurfaceLiquidState.ContaminatingLiquid.IsEmpty)
		{
			var remaining = SurfaceLiquidState.RemoveLiquidVolume(SurfaceLiquidState.LiquidVolume);
			if (remaining is not null)
			{
				if (newItem is not null) newItem.SurfaceLiquidState.AddLiquid(remaining);
				else if (originalTrueLocation is MudSharp.Construction.Cell cell && originalSpatialLocation is { } point)
					cell.AddLiquidToSurfaceAt(remaining, point.Layer, point.RoutePositionMetres);
				else originalTrueLocation?.AddLiquidToSurface(remaining, RoomLayer, LocationLevelPerceivable);
			}
		}
        foreach (IGameItemComponent component in Components.OrderBy(x => x.ComponentDieOrder))
        {
            if (component.HandleDieOrMorph(newItem, originalTrueLocation, originalSpatialLocation))
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

        foreach (IPerceivable target in TargetedBy.ToList())
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
		if (originalSpatialLocation.HasValue)
		{
			newItem.InsertAtSpatialLocation(originalSpatialLocation.Value);
		}
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
        HealthTickResult result = HealthStrategy.EvaluateStatus(this);
        if (result == HealthTickResult.Dead)
        {
            Die();
        }
    }

    #endregion
}

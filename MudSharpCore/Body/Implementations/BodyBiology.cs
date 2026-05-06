using MudSharp.Body.Disfigurements;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Health.Breathing;
using MudSharp.Health.Wounds;
using MudSharp.Logging;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Planes;
using MudSharp.RPG.AIStorytellers;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Work.Projects.Impacts;
using ExpressionEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Body.Implementations;

public partial class Body
{
    public event WoundEvent OnWounded;
    public event WoundEvent OnHeal;
    public event WoundEvent OnRemoveWound;

    private IHealthStrategy _healthStrategy = null;
    private int _suppressHealthFeedbackDepth;

    private readonly List<IInfection> _partInfections = new();
    private readonly List<IWound> _wounds = new();

    private double _currentBloodVolumeLitres;
    private bool _healthTickActive;

    public double CurrentBloodVolumeLitres
    {
        get => _currentBloodVolumeLitres;
        set
        {
            if (value != 0.0)
            {
                _currentBloodVolumeLitres = value;
                if (_currentBloodVolumeLitres < 0.0)
                {
                    _currentBloodVolumeLitres = 0.0;
                }

                if (_currentBloodVolumeLitres > TotalBloodVolumeLitres)
                {
                    _currentBloodVolumeLitres = TotalBloodVolumeLitres;
                }

                Changed = true;
            }
        }
    }

    public double TotalBloodVolumeLitres { get; set; }

    public double LiverAlcoholRemovalKilogramsPerHour => OrganFunction<LiverProto>() *
                                                         BaseLiverAlcoholRemovalKilogramsPerHour;

    public double BaseLiverAlcoholRemovalKilogramsPerHour { get; set; }
    public double WaterLossLitresPerHour { get; set; }
    public double CaloricConsumptionPerHour { get; set; }

    public ILiquid BloodLiquid => Race.BloodLiquid;

    public IBloodtype Bloodtype { get; set; }

    public event PerceivableEvent OnDeath;

    public IGameItem Die()
    {
        OnDeath?.Invoke(this);
        EndStaminaTick(true);
        EndDrugTick();
        EndHealthTickRegistration();
        _breathingStrategy = new NonBreather();

        // Wielded items merely become held items
        foreach (IGameItem item in WieldedItems.ToList())
        {
            Tuple<IGameItem, IWield> heldLocation = _wieldedItems.FirstOrDefault(x => x.Item1 == item);
            _wieldedItems.RemoveAll(x => x.Item1 == item);
            if (heldLocation?.Item2.SelfUnwielder() == true)
            {
                _heldItems.Add(Tuple.Create(heldLocation.Item1, heldLocation.Item2 as IGrab));
            }
            else
            {
                Get(item, silent: true);
            }

            InventoryChanged = true;
        }

        return null;
    }

    public ICharacter Resurrect(ICell location)
    {
        _breathingStrategy = Race.BreathingStrategy;
        CalculateOrganFunctions();
        StartStaminaTick();
        CurrentExertion = ExertionLevel.Low;
        LongtermExertion = ExertionLevel.Low;
        StartHealthTick();
        return Actor;
    }

    public void CheckHealthStatus()
    {
        if (_loading || !IsActiveCharacterBody)
        {
            return;
        }

        EvaluateWounds();
        RecheckStatus();
        CheckBodypartDamage();
        CalculateOrganFunctions();
    }

    internal void ExecuteWithSuppressedHealthFeedback(Action action)
    {
        _suppressHealthFeedbackDepth++;
        try
        {
            action();
        }
        finally
        {
            _suppressHealthFeedbackDepth--;
        }
    }

    private bool SuppressHealthFeedback => _suppressHealthFeedbackDepth > 0;

    public IEnumerable<IWound> GetWoundsForLimb(ILimb limb)
    {
        return limb == null
            ? Enumerable.Empty<IWound>()
            : Wounds.Where(x => limb.Parts.Any(y =>
                x.Bodypart == y || (x.Bodypart is IBone b && y.Bones.Contains(b)) ||
                (x.Bodypart is IOrganProto o && y.Organs.Contains(o)))).ToList();
    }

    public (List<ILimb> WorkingAppendages, List<ILimb> NonWorkingAppendages) GetArmAndAppendagesInformation()
    {
        List<ILimb> arms = Limbs.Where(x => x.LimbType.In(LimbType.Arm, LimbType.Appendage, LimbType.Wing)).ToList();
        List<ILimb> nonWorkingLimbs = new();
        List<ILimbIneffectiveEffect> effects = CombinedEffectsOfType<ILimbIneffectiveEffect>().ToList();
        foreach (ILimb arm in arms)
        {
            if (effects.Any(x => x.Applies(arm) && x.AppliesToLimb(arm)))
            {
                nonWorkingLimbs.Add(arm);
                continue;
            }

            List<IBodypart> parts = BodypartsForLimb(arm).Where(x => x.Significant).ToList();

            if (parts.Any(x => CanUseBodypart(x) != CanUseBodypartResult.CanUse))
            {
                nonWorkingLimbs.Add(arm);
                continue;
            }
        }

        return (arms.Except(nonWorkingLimbs).ToList(), nonWorkingLimbs);
    }

    public (List<ILimb> WorkingLegs, List<ILimb> NonWorkingLegs) GetLegInformation(bool ignoreAids)
    {
        List<ILimb> legs = Limbs.Where(x => x.LimbType == LimbType.Leg).ToList();
        List<ILimb> nonWorkingLimbs = new();
        List<ILimbIneffectiveEffect> effects = CombinedEffectsOfType<ILimbIneffectiveEffect>().ToList();
        foreach (ILimb leg in legs)
        {
            if (effects.Any(x => x.Applies(leg) && x.AppliesToLimb(leg)))
            {
                nonWorkingLimbs.Add(leg);
                continue;
            }

            List<IBodypart> parts = BodypartsForLimb(leg).ToList();
            if (!parts.Any(x => x is StandingBodypartProto))
            {
                nonWorkingLimbs.Add(leg);
                continue;
            }

            if (parts.Any(x => x.Significant && CanUseBodypart(x) != CanUseBodypartResult.CanUse))
            {
                nonWorkingLimbs.Add(leg);
                continue;
            }
        }

        if (!ignoreAids)
        {
            List<ICrutch> crutches = HeldItems.SelectNotNull(x => x.GetItemType<ICrutch>()).ToList();
            foreach (ILimb limb in nonWorkingLimbs.ToArray())
            {
                ICrutch matchingCrutch =
                    crutches.FirstOrDefault(x =>
                        _heldItems.First(y => y.Item1 == x.Parent).Item2.Alignment.LeftRightOnly() ==
                        limb.RootBodypart.Alignment.LeftRightOnly());
                if (matchingCrutch != null)
                {
                    crutches.Remove(matchingCrutch);
                    nonWorkingLimbs.Remove(limb);
                }
            }
        }

        return (legs.Except(nonWorkingLimbs).ToList(), nonWorkingLimbs);
    }

    public bool CanStand(bool ignoreAids)
    {
        (List<ILimb> workingLegs, List<ILimb> nonWorkingLegs) = GetLegInformation(ignoreAids);

        int workingLimbCount = workingLegs.Count;
        if (workingLimbCount >= Prototype.MinimumLegsToStand)
        {
            return true;
        }

        if (ignoreAids)
        {
            return false;
        }

        int targetLimbCount = workingLimbCount == 0
            ? Prototype.MinimumLegsToStand + 1
            : Prototype.MinimumLegsToStand;

        List<ICrutch> crutches = HeldItems.SelectNotNull(x => x.GetItemType<ICrutch>()).ToList();
        foreach (ILimb limb in nonWorkingLegs.ToArray())
        {
            ICrutch matchingCrutch =
                crutches.FirstOrDefault(x =>
                    _heldItems.First(y => y.Item1 == x.Parent).Item2.Alignment.LeftRightOnly() ==
                    limb.RootBodypart.Alignment.LeftRightOnly());
            if (matchingCrutch != null)
            {
                crutches.Remove(matchingCrutch);
                nonWorkingLegs.Remove(limb);
                workingLegs.Add(limb);
            }
        }

        workingLimbCount = workingLegs.Count;
        return workingLimbCount >= targetLimbCount;
    }

    public bool CanKneel(bool ignoreAids)
    {
        (List<ILimb> workingLegs, List<ILimb> nonWorkingLegs) = GetLegInformation(ignoreAids);
        (List<ILimb> workingArms, List<ILimb> _) = GetArmAndAppendagesInformation();

        int workingLimbCount = workingLegs.Count;
        if (workingLimbCount >= Prototype.MinimumLegsToStand ||
            (workingLimbCount >= Prototype.MinimumLegsToStand - 1 && workingArms.Count >= 1))
        {
            return true;
        }

        if (ignoreAids)
        {
            return false;
        }

        int targetLimbCount = workingLimbCount == 0
            ? Prototype.MinimumLegsToStand + 1
            : Prototype.MinimumLegsToStand;

        List<ICrutch> crutches = HeldItems.SelectNotNull(x => x.GetItemType<ICrutch>()).ToList();
        foreach (ILimb limb in nonWorkingLegs.ToArray())
        {
            ICrutch matchingCrutch =
                crutches.FirstOrDefault(x =>
                    _heldItems.First(y => y.Item1 == x.Parent).Item2.Alignment.LeftRightOnly() ==
                    limb.RootBodypart.Alignment.LeftRightOnly());
            if (matchingCrutch != null)
            {
                crutches.Remove(matchingCrutch);
                nonWorkingLegs.Remove(limb);
                workingLegs.Add(limb);
            }
        }

        workingLimbCount = workingLegs.Count;
        return workingLimbCount >= targetLimbCount ||
               (workingLimbCount >= Prototype.MinimumLegsToStand - 1 && workingArms.Count >= 1);
    }

    public bool CanSitUp()
    {
        (List<ILimb> workingLegs, List<ILimb> _) = GetLegInformation(true);
        (List<ILimb> workingArms, List<ILimb> _) = GetArmAndAppendagesInformation();
        return workingLegs.Count >= 1 || workingArms.Count >= 1;
    }

    private readonly DoubleCounter<Type> _cachedOrganFunctionsByType = new();
    private readonly DoubleCounter<IOrganProto> _cachedOrganFunctionsByOrgan = new();

    public void CalculateOrganFunctions(bool initialCalculation = false)
    {
        DoubleCounter<IOrganProto> oldCached = new(_cachedOrganFunctionsByOrgan);
        _cachedOrganFunctionsByOrgan.Clear();
        _cachedOrganFunctionsByType.Clear();
        CollectionDictionary<IOrganProto, double> merits = Actor.Merits.OfType<IOrganFunctionBonusMerit>()
                          .Where(x => x.Applies(Actor))
                          .SelectMany(x => x.OrganFunctionBonuses(this))
                          .ToCollectionDictionary();
        ILookup<IOrganProto, double> organs = Organs
                     .Select(x => (Organ: x, Factor: x.OrganFunctionFactor(this)))
                     .ToLookup(x => x.Organ, x => x.Factor);
        CollectionDictionary<IOrganProto, double> implantOrgans = Implants
                            .OfType<IOrganImplant>()
                            .Select(x => (Organ: x.TargetOrgan, Factor: x.FunctionFactor))
                            .ToCollectionDictionary();
        CollectionDictionary<IOrganProto, double> external = Implants
                                   .SelectNotNull(x => x.Parent.GetItemType<ICannula>())
                                   .SelectMany(x =>
                                           x.ConnectedItems.SelectNotNull(y => y.Item2.Parent.GetItemType<IExternalOrganFunction>()))
                                   .Distinct()
                                   .SelectMany(x => x.OrganFunctions.Select(y => (Organ: y.Organ, Factor: y.Function)))
                                   .ToCollectionDictionary();
        CollectionDictionary<IOrganProto, double> effectBonuses = CombinedEffectsOfType<IOrganFunctionEffect>()
                                 .SelectMany(x => x.OrganFunctionBonuses(this))
                                 .ToCollectionDictionary();

        // Pre-calculate spinal organs because other organs will depend on it
        foreach (IOrganProto organ in Organs.OrderByDescending(x => x is SpineProto))
        {
            if (organ.RequiresSpinalConnection && HealthStrategy.RequiresSpinalCord)
            {
                if (GetLimbFor(organ).SpineProtos.Any(x => _cachedOrganFunctionsByOrgan[x] <= 0.0))
                {
                    _cachedOrganFunctionsByOrgan[organ] = 0.0;
                    _cachedOrganFunctionsByType[organ.GetType()] = 0.0;
                    if (!initialCalculation && !SuppressHealthFeedback)
                    {
                        organ.HandleChangedOrganFunction(this, oldCached[organ], 0.0);
                    }

                    continue;
                }
            }

            double bonus =
                    merits[organ].Sum() +
                    organs[organ].Sum() +
                    implantOrgans[organ].Sum() +
                    external[organ].Sum() +
                    effectBonuses[organ].Sum();
            _cachedOrganFunctionsByOrgan[organ] = bonus;
            _cachedOrganFunctionsByType[organ.GetType()] += bonus;
            if (!initialCalculation && !SuppressHealthFeedback)
            {
                organ.HandleChangedOrganFunction(this, oldCached[organ], bonus);
            }
        }
    }

    public double OrganFunction<T>() where T : IOrganProto
    {
        return _cachedOrganFunctionsByType[typeof(T)];
    }

    public double OrganFunction(IOrganProto organ)
    {
        return _cachedOrganFunctionsByOrgan[organ];
    }

    public IHealthStrategy HealthStrategy => _healthStrategy ?? Race.DefaultHealthStrategy;
    public IEnumerable<IWound> Wounds => _wounds;

    private bool WoundIsVisible(IWound wound, IPerceiver voyeur, WoundExaminationType examinationType)
    {
        if (wound.Severity == WoundSeverity.None)
        {
            return false;
        }

        IBodypart external = wound.Bodypart;
        if (external is IBone bone)
        {
            if (wound is BoneFracture bf)
            {
                if ((examinationType == WoundExaminationType.Glance && wound.Severity < WoundSeverity.VerySevere) ||
                    (examinationType == WoundExaminationType.Look && wound.Severity < WoundSeverity.Severe) ||
                    (examinationType == WoundExaminationType.Self && wound.Severity < WoundSeverity.Small) ||
                    (examinationType == WoundExaminationType.Examination && wound.Severity < WoundSeverity.Minor)
                   )
                {
                    return false;
                }

                if ((examinationType == WoundExaminationType.Glance || examinationType == WoundExaminationType.Look) &&
                    bf.HasBeenRelocated)
                {
                    return false;
                }
            }

            external = Bodyparts.FirstOrDefault(x =>
                x.BoneInfo.Any(y => y.Key == bone && y.Value.IsPrimaryInternalLocation)) ?? bone;
        }

        switch (examinationType)
        {
            case WoundExaminationType.Glance:
            case WoundExaminationType.Look:
                return WornItemsProfilesFor(external).All(profile =>
                    profile.Item1.GetItemType<IWearable>()?.GloballyTransparent == true || profile.Item2.Transparent);
        }

        return true;
    }

    public IEnumerable<IWound> VisibleWounds(IPerceiver voyeur, WoundExaminationType examinationType)
    {
        List<IWound> result = _wounds.ToList();
        // Non-internal examinations
        switch (examinationType)
        {
            case WoundExaminationType.Glance:
            case WoundExaminationType.Look:
            case WoundExaminationType.Examination:
            case WoundExaminationType.Triage:
            case WoundExaminationType.Self:
                result = result.Where(x => !x.Internal).ToList();
                break;
            case WoundExaminationType.SurgicalExamination:
                result = result.Where(x => x.Internal).ToList();
                break;
        }

        result = result.Where(x => WoundIsVisible(x, voyeur, examinationType)).ToList();

        // TODO - effects that hide wounds
        return result;
    }

    public void AddWound(IWound wound)
    {
        if (wound == null)
        {
            throw new ArgumentNullException(nameof(wound));
        }

        if (!_wounds.Contains(wound))
        {
            _wounds.Add(wound);
            Changed = true;
        }
    }

    public void AddWounds(IEnumerable<IWound> wounds)
    {
        foreach (IWound wound in wounds)
        {
            if (!_wounds.Contains(wound))
            {
                _wounds.Add(wound);
            }
        }

        Changed = true;
    }

    public bool TryTransferWoundTo(IWound wound, IHaveWounds newOwner, IBodypart newBodypart,
        IBodypart newSeveredBodypart = null)
    {
        if (wound == null || newOwner == null || newBodypart == null || !_wounds.Contains(wound))
        {
            return false;
        }

        _wounds.Remove(wound);
        OnRemoveWound?.Invoke(this, wound);
        wound.RemapTo(newOwner, newBodypart, newSeveredBodypart);
        newOwner.AddWound(wound);
        Changed = true;
        newOwner.StartHealthTick();
        StartHealthTick();
        EvaluateWounds();
        newOwner.EvaluateWounds();
        return true;
    }

    public IEnumerable<IWound> InventoryExploded(IGameItem exploded, IExplosiveDamage damage)
    {
        double damageFactor = 1.0;
        int score = CurrentContextualSize(SizeContext.ExplosiveDamage) - damage.ExplosionSize;
        ExplosionDamageExtent scope = ExplosionDamageExtent.IndividualPart;
        if (score < 0)
        {
            scope = ExplosionDamageExtent.EntireBody;
        }
        else
        {
            switch (score)
            {
                case 0:
                    scope = ExplosionDamageExtent.EntireFacing;
                    break;
                case 1:
                    scope = ExplosionDamageExtent.EntireLimb;
                    break;
                case 2:
                    scope = ExplosionDamageExtent.AdjacentParts;
                    break;
                case 3:
                    scope = ExplosionDamageExtent.IndividualPart;
                    break;
                default:
                    damageFactor /= score - 2.0;
                    break;
            }
        }

        Facing facing = Facing.Front;
        List<IBodypart> bodyparts = new();
        IBodypart inventoryPart = TopLevelBodypart(BodypartLocationOfInventoryItem(exploded));

        switch (scope)
        {
            case ExplosionDamageExtent.IndividualPart:

                if (inventoryPart == null)
                {
                    break;
                }

                bodyparts.Add(inventoryPart);
                break;
            case ExplosionDamageExtent.AdjacentParts:
                if (inventoryPart == null)
                {
                    break;
                }

                bodyparts.Add(inventoryPart);
                foreach (IBodypart other in Bodyparts.Where(x => x.UpstreamConnection == inventoryPart))
                {
                    bodyparts.Add(other);
                }

                if (inventoryPart.UpstreamConnection != null)
                {
                    bodyparts.Add(inventoryPart.UpstreamConnection);
                }

                break;
            case ExplosionDamageExtent.EntireLimb:
                if (inventoryPart == null)
                {
                    break;
                }

                ILimb limb = GetLimbFor(inventoryPart);
                if (limb != null)
                {
                    bodyparts.AddRange(BodypartsForLimb(limb));
                }

                break;
            case ExplosionDamageExtent.EntireFacing:
                if (inventoryPart == null)
                {
                    break;
                }

                facing = inventoryPart.Alignment.ToFacing();
                HashSet<Alignment> permittedAlignements = new();
                switch (facing)
                {
                    case Facing.Front:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        break;
                    case Facing.RightFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.Rear:
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.LeftFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.RearLeft);
                        break;
                }

                bodyparts.AddRange(Bodyparts.Where(x =>
                    x.RelativeHitChance > 0 && permittedAlignements.Contains(x.Alignment)));
                break;
            case ExplosionDamageExtent.EntireBody:
                permittedAlignements = new HashSet<Alignment>();
                facing = inventoryPart.Alignment.ToFacing();
                switch (facing)
                {
                    case Facing.Front:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.RightFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.Rear:
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.LeftFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                }

                bodyparts.AddRange(Bodyparts.Where(x =>
                    x.RelativeHitChance > 0 && permittedAlignements.Contains(x.Alignment)));
                break;
        }

        ExplosiveDamage newDamage = new(damage, damageFactor, null);
        List<IWound> wounds = new();
        Actor.AddEffect(new SupressWoundMessages(Actor)
        { TargetTookWoundsEmote = "@ got caught up in the explosion!" });
        foreach (IBodypart part in bodyparts)
        {
            foreach (IDamage item in damage.ReferenceDamages)
            {
                wounds.AddRange(PassiveSufferDamage(new Damage(item, damageFactor) { Bodypart = part }));
            }

            if (part is IGrab)
            {
                foreach (IGameItem gi in HeldOrWieldedItemsFor(part).ToList())
                {
                    if (gi == exploded)
                    {
                        continue;
                    }

                    wounds.AddRange(gi.PassiveSufferDamage(newDamage, Proximity.Intimate, Facing.Front));
                }
            }
        }

        wounds.AddRange(ExplosionEmantingFromPerceivable(damage));
        return wounds;
    }

    public IEnumerable<IWound> PassiveSufferDamage(IExplosiveDamage damage, Proximity proximity, Facing facing)
    {
        // Hard Cover has a chance of avoiding explosions entirely unless it's right up against the character
        if (proximity != Proximity.Intimate && Cover?.Cover.CoverType == MudSharp.Combat.CoverType.Hard)
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
        int score = CurrentContextualSize(SizeContext.ExplosiveDamage) - damage.ExplosionSize + (int)proximity +
                    ((int?)Cover?.Cover.CoverExtent ?? 0) *
                    (Cover?.Cover.CoverType == MudSharp.Combat.CoverType.Hard ? 1 : 0);
        ExplosionDamageExtent scope = ExplosionDamageExtent.IndividualPart;
        if (score < 0)
        {
            scope = ExplosionDamageExtent.EntireBody;
        }
        else
        {
            switch (score)
            {
                case 0:
                    scope = ExplosionDamageExtent.EntireFacing;
                    break;
                case 1:
                    scope = ExplosionDamageExtent.EntireLimb;
                    damageFactor = 0.8;
                    break;
                case 2:
                    scope = ExplosionDamageExtent.AdjacentParts;
                    damageFactor = 0.66;
                    break;
                case 3:
                    scope = ExplosionDamageExtent.IndividualPart;
                    damageFactor = 0.5;
                    break;
                default:
                    damageFactor /= score - 1.0;
                    break;
            }
        }

        List<IBodypart> bodyparts = new();
        switch (scope)
        {
            case ExplosionDamageExtent.IndividualPart:
                IBodypart part = RandomBodyPartGeometry(
                    BodyExtensions.HeightToOrientation(damage.Elevation, Height, Orientation.Centre), Alignment.Front,
                    facing);
                if (part == null)
                {
                    break;
                }

                bodyparts.Add(part);
                break;
            case ExplosionDamageExtent.AdjacentParts:
                part = RandomBodyPartGeometry(
                    BodyExtensions.HeightToOrientation(damage.Elevation, Height, Orientation.Centre), Alignment.Front,
                    facing);
                if (part == null)
                {
                    break;
                }

                bodyparts.Add(part);
                foreach (IBodypart other in Bodyparts.Where(x => x.UpstreamConnection == part))
                {
                    bodyparts.Add(other);
                }

                if (part.UpstreamConnection != null)
                {
                    bodyparts.Add(part.UpstreamConnection);
                }

                break;
            case ExplosionDamageExtent.EntireLimb:
                part = RandomBodyPartGeometry(
                    BodyExtensions.HeightToOrientation(damage.Elevation, Height, Orientation.Centre), Alignment.Front,
                    facing);
                if (part == null)
                {
                    break;
                }

                ILimb limb = GetLimbFor(part);
                if (limb != null)
                {
                    bodyparts.AddRange(BodypartsForLimb(limb));
                }

                break;
            case ExplosionDamageExtent.EntireFacing:
                HashSet<Alignment> permittedAlignements = new();
                switch (facing)
                {
                    case Facing.Front:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        break;
                    case Facing.RightFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.Rear:
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.LeftFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.RearLeft);
                        break;
                }

                bodyparts.AddRange(Bodyparts.Where(x =>
                    x.RelativeHitChance > 0 && permittedAlignements.Contains(x.Alignment)));
                break;
            case ExplosionDamageExtent.EntireBody:
                permittedAlignements = new HashSet<Alignment>();
                switch (facing)
                {
                    case Facing.Front:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.RightFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.Rear:
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.FrontRight);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                    case Facing.LeftFlank:
                        permittedAlignements.Add(Alignment.Front);
                        permittedAlignements.Add(Alignment.RearLeft);
                        permittedAlignements.Add(Alignment.FrontLeft);
                        permittedAlignements.Add(Alignment.Rear);
                        permittedAlignements.Add(Alignment.RearRight);
                        break;
                }

                bodyparts.AddRange(Bodyparts.Where(x =>
                    x.RelativeHitChance > 0 && permittedAlignements.Contains(x.Alignment)));
                break;
        }

        List<IWound> wounds = new();
        Actor.AddEffect(new SupressWoundMessages(Actor)
        { TargetTookWoundsEmote = "@ got caught up in the explosion!" });
        foreach (IBodypart part in bodyparts)
        {
            foreach (IDamage item in damage.ReferenceDamages)
            {
                wounds.AddRange(PassiveSufferDamage(new Damage(item, damageFactor) { Bodypart = part }));
                if (part is IGrab)
                {
                    foreach (IGameItem gi in HeldOrWieldedItemsFor(part).ToList())
                    {
                        wounds.AddRange(gi.PassiveSufferDamage(new Damage(item, damageFactor)));
                    }
                }
            }
        }

        return wounds;
    }

    public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
    {
        StringBuilder sb = new();
        IEnumerable<IWound> result = PassiveSufferDamage(damage, sb);
        Actor.OutputHandler.Handle(new FilteredEmoteOutput(sb.ToString(), Actor, x => x.AffectedBy<DebugMode>(), flags: OutputFlags.WizOnly | OutputFlags.WideWrap));
        return result;
    }

    public IEnumerable<IWound> PassiveSufferDamage(IDamage damage, StringBuilder sb)
    {
        if (damage == null)
        {
            return Enumerable.Empty<IWound>();
        }

        if (!Bodyparts.Contains(damage.Bodypart) && !Organs.Contains(damage.Bodypart) &&
            !Bones.Contains(damage.Bodypart))
        {
            return Enumerable.Empty<IWound>();
        }

        if (damage.DamageType == DamageType.Cellular || damage.DamageType == DamageType.Hypoxia)
        {
            IWound existingWound = Wounds.FirstOrDefault(x => x.Bodypart == damage.Bodypart && damage.DamageType == x.DamageType);
            if (existingWound == null)
            {
                existingWound = HealthStrategy.SufferDamage(Actor, damage, damage.Bodypart).FirstOrDefault();
                if (existingWound == null)
                {
                    return Enumerable.Empty<IWound>();
                }

                Gameworld.LogManager.CustomLogEntry(LogEntryType.SufferHypoxiaCellularDamage, this, damage, existingWound);

                if (!_wounds.Contains(existingWound))
                {
                    _wounds.Add(existingWound);
                }

                return new[] { existingWound };
            }

            existingWound.OriginalDamage += damage.DamageAmount;
            existingWound.CurrentDamage += damage.DamageAmount;
            existingWound.CurrentPain += damage.PainAmount;
            Gameworld.LogManager.CustomLogEntry(LogEntryType.SufferHypoxiaCellularDamage, this, damage, existingWound);
            return new[] { existingWound };
        }

        sb.AppendLine($"Damage Suffered by {this.HowSeen(Actor, flags: PerceiveIgnoreFlags.TrueDescription)}".GetLineWithTitleInner(Actor, Telnet.Red, Telnet.BoldWhite));
        sb.AppendLine($"\nOriginal Damage: #2{damage.DamageAmount:N2} #3{damage.DamageType.Describe()}#0 to #2{damage.Bodypart?.FullDescription() ?? "Unknown"}#0".SubstituteANSIColour());


        List<IWound> wounds = new();
        bool isplainbodypart = !(damage.Bodypart is IOrganProto) && !(damage.Bodypart is IBone);
        if (isplainbodypart)
        {
            List<IArmour> armour =
                WornItemsProfilesFor(damage.Bodypart)
                    .Where(x => !x.Item2.NoArmour)
                    .SelectNotNull(x => x.Item1.GetItemType<IArmour>())
                    .Reverse() // Last items worn are the first hit
                    .ToList();

            foreach (IArmour item in armour)
            {
                damage = item.PassiveSufferDamage(damage, ref wounds);
                sb.AppendLine(
                    $"Armour ({item.Parent.HowSeen(item.Parent)}): Took {wounds.LastOrDefault(x => x.Parent == item.Parent)?.CurrentDamage.ToString("N2") ?? "None"}, Passed on: {(damage == null ? "Nothing" : $"#2{damage.DamageAmount:N2} #3{damage.DamageType.Describe()}#0".SubstituteANSIColour())}");

                if (damage == null)
                {
                    return wounds;
                }
            }

            if (damage.LodgableItem != null && wounds.Any(x => x.Lodged == damage.LodgableItem))
            {
                damage = new Damage(damage) { LodgableItem = null };
            }

            IProsthetic targetProsthetic = Prosthetics.FirstOrDefault(
                x => x.TargetBodypart == damage.Bodypart || damage.Bodypart.DownstreamOfPart(x.TargetBodypart));
            if (targetProsthetic != null)
            {
                sb.AppendLine($"Hit Prosthetic ({targetProsthetic.Parent.HowSeen(targetProsthetic.Parent)})");
                wounds.AddRange(targetProsthetic.Parent.PassiveSufferDamage(damage));
                return wounds;
            }
        }

        foreach (IMagicArmour magicarmour in CombinedEffectsOfType<IMagicArmour>())
        {
            if (!magicarmour.Applies() || !magicarmour.AppliesToPart(damage.Bodypart))
            {
                continue;
            }

            damage = magicarmour.PassiveSufferDamage(damage, ref wounds);
            if (damage == null)
            {
                sb.AppendLine($"Damage completely absorbed by {magicarmour.MagicArmourOriginDescription}");
                return wounds;
            }

            sb.AppendLine($"Damage reduced to #2{damage.DamageAmount:N2} #3{damage.DamageType.Describe()}#0 by {magicarmour.MagicArmourOriginDescription}".SubstituteANSIColour());
        }

        if (Race.NaturalArmourType is not null && isplainbodypart)
        {
            sb.Append($"Natural Racial Armour {Race.NaturalArmourType.Name.ColourName()} ({Race.NaturalArmourQuality.Describe()}/{Race.NaturalArmourMaterial?.Name}) ");
            damage = Race.NaturalArmourType.AbsorbDamage(damage, Race.NaturalArmourQuality, Race.NaturalArmourMaterial, Actor, ref wounds).PassThroughDamage;
            if (damage == null)
            {
                sb.AppendLine($"completely absorbed remaining damage");
                return wounds;
            }

            sb.AppendLine($"reduced damage to #2{damage.DamageAmount:N2} #3{damage.DamageType.Describe()}#0".SubstituteANSIColour());
        }

        IDamage internalDamage = damage;
        IArmourType bodypartNaturalArmourType = damage.Bodypart?.NaturalArmourType;
        if (bodypartNaturalArmourType != null)
        {
            (IDamage SufferedDamage, IDamage PassThroughDamage) damages = bodypartNaturalArmourType.AbsorbDamage(damage, Race.NaturalArmourQuality,
                GetMaterial(damage.Bodypart), Actor, ref wounds);
            damage = damages.SufferedDamage;
            internalDamage = damages.PassThroughDamage;
            sb.AppendLine($"Bodypart Natural Damage {bodypartNaturalArmourType.Name.ColourName()} reduced to #2{damage?.DamageAmount.ToString("N2") ?? "Nothing"}, Passed on: {(internalDamage == null ? "#1Nothing#0" : $"#2{internalDamage.DamageAmount:N2} #3{internalDamage.DamageType.Describe()}#0")}".SubstituteANSIColour());

        }

        if (damage == null)
        {
            return wounds;
        }

        if (damage.LodgableItem != null && wounds.Any(x => x.Lodged == damage.LodgableItem))
        {
            damage = new Damage(damage) { LodgableItem = null };
        }

        IDamage damage1 = damage;
        IBodypart severedPart = null;
        EffectHandler.RemoveAllEffects(
            x => x.GetSubtype<IEffectRemoveOnDamage>()?.RemovesWith(damage1) ?? false);
        if (isplainbodypart)
        {
            if (!CheckBoneDamage(ref damage, ref wounds, ref internalDamage, true, sb))
            {
                CheckOrganDamage(ref damage, wounds, ref internalDamage, true, sb);
            }

            if (damage.DamageAmount <= 0 && damage.StunAmount <= 0 && damage.PainAmount <= 0)
            {
                return wounds;
            }


            if (ShouldSever(damage))
            {
                severedPart = damage.Bodypart;
                damage = new Damage(damage) { Bodypart = damage.Bodypart.UpstreamConnection };
            }
        }

        IWound[] newWounds = HealthStrategy.SufferDamage(Actor, damage, damage.Bodypart).ToArray();
        IWound newWound = newWounds.FirstOrDefault(x => x is not BoneFracture);
        newWound?.SeveredBodypart = severedPart;

        if (!newWounds.Any())
        {
            return wounds;
        }

        foreach (IWound wound in newWounds)
        {
            if (!_wounds.Contains(wound))
            {
                _wounds.Add(wound);
                Changed = true;
            }
        }

        wounds.AddRange(newWounds);
        return wounds;
    }

    private bool ShouldSever(IDamage damage)
    {
        if (damage.Bodypart is null || !damage.Bodypart.CanSever)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(damage.Bodypart.SeverFormula))
        {
            IExpression expression = new Expression(damage.Bodypart.SeverFormula);
            return expression.EvaluateDoubleWith(("damage", damage.DamageAmount), ("damagetype", (int)damage.DamageType)) >= 1.0;
        }

        return damage.DamageAmount >= Race.ModifiedSeverthreshold(damage.Bodypart) &&
               damage.DamageType.CanSever();
    }

    private bool CheckOrganDamageSpecific(KeyValuePair<IOrganProto, BodypartInternalInfo> organInfo,
        IDamage internalDamage, bool damageAllOrgans, List<IWound> wounds, bool passive, bool highChance, StringBuilder sb)
    {
        IOrganProto organ = organInfo.Key;
        double baseChance = organInfo.Value.HitChance * (highChance ? 2.0 : 1.0) / 100.0;
        double roll = RandomUtilities.DoubleRandom(0.0, 1.0);
        sb.Append($"...rolled #2{roll:P2}#0 vs #2{baseChance:P2}#0 for organ #3{organ.Name}#0".SubstituteANSIColour());
        if (!damageAllOrgans &&
            roll > baseChance)
        {
            sb.AppendLine($"...missed");
            return false;
        }

        if (!damageAllOrgans && !highChance && Actor.Merits.OfType<IOrganHitReductionMerit>()
                                                    .Where(x => x.Applies(Actor)).Any(x =>
                                                        x.MissesOrgan(organInfo, internalDamage,
                                                            HealthStrategy.GetSeverity(internalDamage.DamageAmount))))
        {
            sb.AppendLine("...missed because of merit");
            return false;
        }

        sb.AppendLine("...hit");
        Damage newDamage = new(internalDamage) { Bodypart = organ, LodgableItem = null };
        if (passive)
        {
            wounds.AddRange(PassiveSufferDamage(newDamage, sb));
        }
        else
        {
            wounds.AddRange(SufferDamage(newDamage));
        }

        return true;
    }

    private void CheckOrganDamage(ref IDamage damage, List<IWound> wounds, ref IDamage internalDamage, bool passive, StringBuilder sb)
    {
        (bool damageOrgans, bool damageAllOrgans, bool highChance) = CanDamageOrgans(internalDamage);
        if (damage.Bodypart.Organs.Any() && internalDamage != null && damageOrgans)
        {
            foreach (KeyValuePair<IOrganProto, BodypartInternalInfo> organInfo in damage.Bodypart.OrganInfo.Where(x => Organs.Contains(x.Key)).Shuffle().ToList())
            {
                if (CheckOrganDamageSpecific(organInfo, internalDamage, damageAllOrgans, wounds, passive, highChance, sb))
                {
                    IGameItem item = damage.LodgableItem;
                    if (damage.LodgableItem != null && wounds.Any(x => x.Lodged == item))
                    {
                        damage = new Damage(damage) { LodgableItem = null };
                    }

                    if (!damageAllOrgans)
                    {
                        break;
                    }
                }
            }
        }
    }

    private bool CheckBoneDamage(ref IDamage damage, ref List<IWound> wounds, ref IDamage internalDamage, bool passive, StringBuilder sb)
    {
        (bool damageBones, bool damageAllBones, bool damageBoneGroups) = CanBreakBones(damage);
        List<IWound> boneWounds = Wounds.Where(x => x.Bodypart is IBone).ToList();
        if (damageBones && damage.Bodypart.Bones.Any())
        {
            bool boneWasHit = false;
            foreach (KeyValuePair<IBone, BodypartInternalInfo> boneInfo in damage.Bodypart.BoneInfo.Where(x => Bones.Contains(x.Key)).Shuffle().ToList())
            {
                IBone bone = boneInfo.Key;
                double roll = Constants.Random.NextDouble();
                double baseChance = boneInfo.Value.HitChance / 100.0;
                double actualChance = BoneHitChance(baseChance, internalDamage.DamageType);
                sb.Append($"...rolled #2{roll:P2}#0 vs #2{actualChance:P2}#0 for bone #3{bone.Name}#0".SubstituteANSIColour());
                if (!damageAllBones && roll > actualChance)
                {
                    sb.AppendLine($"...missed");
                    continue;
                }

                boneWasHit = true;
                IArmourType naturalArmour = bone.NaturalArmourType;
                IDamage organDamage;
                if (naturalArmour is null)
                {
                    sb.Append("...hit, no natural armour...");
                    organDamage = new Damage(internalDamage)
                    {
                        LodgableItem = null
                    };
                }
                else if (boneWounds.Any(x => x.Bodypart == bone && x.Severity >= WoundSeverity.Grievous))
                {
                    sb.Append("...hit, bone too damaged to act as protection...");
                    organDamage = new Damage(internalDamage)
                    {
                        LodgableItem = null
                    };
                }
                else
                {
                    (IDamage SufferedDamage, IDamage PassThroughDamage) damages = naturalArmour.AbsorbDamage(internalDamage, Race.NaturalArmourQuality,
                        GetMaterial(bone), Actor,
                        ref wounds);
                    if (damages.SufferedDamage == null)
                    {
                        sb.AppendLine($"...hit, {naturalArmour.Name.ColourName()} reduced damage to nothing.");
                        return true;
                    }

                    organDamage = damages.PassThroughDamage;
                    // Don't set boneDamage here to damages.SufferedDamage as it gets hit separately below


                    sb.AppendLine($"...hit");
                }

                Damage boneDamage = new(internalDamage)
                {
                    Bodypart = bone,
                    LodgableItem = null
                };

                if (passive)
                {
                    wounds.AddRange(PassiveSufferDamage(boneDamage, sb));
                }
                else
                {
                    wounds.AddRange(SufferDamage(boneDamage, sb));
                }

                if (organDamage != null && organDamage.DamageAmount > 0)
                {
                    foreach ((IOrganProto Organ, BodypartInternalInfo Info) organ in bone.CoveredOrgans.Where(x => Organs.Contains(x.Organ)))
                    {
                        if (CheckOrganDamageSpecific(
                                new KeyValuePair<IOrganProto, BodypartInternalInfo>(organ.Organ, organ.Info),
                                organDamage, false, wounds, passive, false, sb))
                        {
                            if (!damageAllBones)
                            {
                                return true;
                            }

                            break;
                        }
                    }
                }

                if (!damageAllBones)
                {
                    return true;
                }
            }

            return boneWasHit;
        }

        return false;
    }

    public void ProcessPassiveWound(IWound wound)
    {
        if (!Bodyparts.Contains(wound.Bodypart) && !Organs.Contains(wound.Bodypart) && !Bones.Contains(wound.Bodypart))
        {
            Gameworld.SaveManager.Abort(wound);
            return;
        }

        CalculateOrganFunctions();
        if (wound.SeveredBodypart != null)
        {
            IGameItem item = SeverBodypart(wound.SeveredBodypart);
            item.RoomLayer = RoomLayer;
            Location.Insert(item);
            item.Login();
            item.HandleEvent(EventType.ItemFinishedLoading, item);
        }

        OnWounded?.Invoke(this, wound);
        wound.OnWoundSuffered();
        HandleEvent(EventType.CharacterDamaged, Actor, wound.ToolOrigin, wound.ActorOrigin);
        foreach (IHandleEvents witness in Location.EventHandlers)
        {
            witness.HandleEvent(EventType.CharacterDamagedWitness, Actor, wound.ToolOrigin, wound.ActorOrigin, witness);
        }
    }

    public IEnumerable<IWound> SufferDamage(IDamage damage)
    {
        StringBuilder sb = new();
        IWound[] wounds = PassiveSufferDamage(damage, sb).ToArray();
        Actor.OutputHandler.Handle(new FilteredEmoteOutput(sb.ToString(), Actor, x => x.AffectedBy<DebugMode>(), flags: OutputFlags.WizOnly | OutputFlags.WideWrap));
        wounds.ProcessPassiveWounds();
        StartHealthTick();
        return wounds;
    }

    public IEnumerable<IWound> SufferDamage(IDamage damage, StringBuilder sb)
    {
        IWound[] wounds = PassiveSufferDamage(damage, sb).ToArray();
        wounds.ProcessPassiveWounds();
        StartHealthTick();
        return wounds;
    }

    public WoundSeverity GetSeverityFor(IWound wound)
    {
        return HealthStrategy.GetSeverityFor(wound, Actor);
    }

    public double GetSeverityFloor(WoundSeverity severity, bool usePercentageModel = false)
    {
        return HealthStrategy.GetSeverityFloor(severity, usePercentageModel);
    }

    public void EvaluateWounds()
    {
        List<IWound> woundsToRemove =
            Wounds.Where(
                      x =>
                          x.ShouldWoundBeRemoved())
                  .ToList();
        foreach (IWound wound in woundsToRemove)
        {
            ScarGeneration.TryApplyScar(Actor, wound);
            wound.Delete();
            _wounds.Remove(wound);
            OnRemoveWound?.Invoke(this, wound);
        }

        CalculateOrganFunctions();

        if (!Wounds.Any() && CurrentBloodVolumeLitres >= TotalBloodVolumeLitres && !PartInfections.Any() &&
            !ActiveDrugDosages.Any() && CanBreathe && _cachedOrganFunctionsByOrgan.All(x => x.Value >= 1.0))
        {
            EndHealthTick();
        }
    }

    public double ImmuneFatigueBonus => PartInfections.Sum(x => x.Intensity) *
                                        Gameworld.GetStaticDouble("BonusPerInfectionIntensityForSpread");

    public void EvaluateInfections()
    {
        List<IInfection> infectionsToRemove = PartInfections.Where(x => x.InfectionHealed()).ToList();

        foreach (IInfection infection in infectionsToRemove)
        {
            infection.Delete();
            _partInfections.Remove(infection);
        }
    }

    public void StartHealthTick(bool initial = false)
    {
        if (!IsActiveCharacterBody)
        {
            EndHealthTickRegistration();
            return;
        }

        CalculateOrganFunctions(initial);
        ReevaluateLimbAndPartDamageEffects();
        (double floor, double ceiling) = TolerableTemperatures(true);
        bool temperatureImbalanceEnabled =
            Gameworld.GetStaticBool(ThermalImbalanceConsequenceModel.EnabledStaticConfiguration);
        if (_healthTickActive ||
            (!Wounds.Any() &&
             !ActiveDrugDosages.Any() &&
             !PartInfections.Any() &&
             CurrentBloodVolumeLitres >= TotalBloodVolumeLitres &&
             CanBreathe &&
             (!temperatureImbalanceEnabled ||
              (HealthStrategy.CurrentTemperatureStatus(Actor) == BodyTemperatureStatus.NormalTemperature &&
               TemperatureExtensions.SubjectiveTemperature(Location.CurrentTemperature(Actor), floor, ceiling) ==
               Temperature.Temperate)) &&
             _cachedOrganFunctionsByOrgan.All(x => x.Value >= 1.0))
           )
        {
            return;
        }

        Gameworld.HeartbeatManager.TenSecondHeartbeat -= HealthTick_TenSecondHeartbeat;
        Gameworld.HeartbeatManager.MinuteHeartbeat -= HealingTick_MinuteHeartbeat;
        Gameworld.HeartbeatManager.TenSecondHeartbeat += HealthTick_TenSecondHeartbeat;
        Gameworld.HeartbeatManager.MinuteHeartbeat += HealingTick_MinuteHeartbeat;
        _healthTickActive = true;
    }

    public void EndHealthTick()
    {
        ReevaluateLimbAndPartDamageEffects();
        EndHealthTickRegistration();
    }

    private void EndHealthTickRegistration()
    {
        if (!_healthTickActive)
        {
            return;
        }

        Gameworld.HeartbeatManager.TenSecondHeartbeat -= HealthTick_TenSecondHeartbeat;
        Gameworld.HeartbeatManager.MinuteHeartbeat -= HealingTick_MinuteHeartbeat;
        _healthTickActive = false;
    }

    public void CureAllWounds()
    {
        foreach (IWound wound in Wounds.ToList())
        {
            wound.Delete();
        }

        _wounds.Clear();
        foreach (IInfection infection in PartInfections.ToList())
        {
            infection.Delete();
        }

        foreach (IInternalBleedingEffect bleed in EffectsOfType<IInternalBleedingEffect>().ToList())
        {
            RemoveEffect(bleed);
        }

        _partInfections.Clear();
        CurrentBloodVolumeLitres = TotalBloodVolumeLitres;
        RemoveAllEffects(x => x.IsEffectType<PassOutFromPain>());
        RecheckStatus();
        EndHealthTick();
        CalculateOrganFunctions();
    }

    public void DoOfflineHealing(TimeSpan timePassed)
    {
        List<IHealingRateEffect> healingEffects = CombinedEffectsOfType<IHealingRateEffect>().ToList();
        double healingMultiplier = healingEffects.Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum);
        double projectMultiplier = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
                                     .OfType<ILabourImpactHealing>()
                                     .Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum) ?? 1.0;
        double healingBonuses = healingEffects.Sum(x => x.HealingDifficultyStages) *
                             Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel");
        double projectBonus = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
                                .OfType<ILabourImpactHealing>().Sum(x => x.HealingCheckBonus) ?? 0.0;

        foreach (IWound wound in _wounds.ToList())
        {
            wound.DoOfflineHealing(timePassed, healingMultiplier * projectMultiplier, healingBonuses + projectBonus);
        }
    }

    public IEnumerable<IInfection> PartInfections => _partInfections;

    public void AddInfection(IInfection infection)
    {
        _partInfections.Add(infection);
    }

    public void RemoveInfection(IInfection infection)
    {
        _partInfections.Remove(infection);
    }

    private (bool Truth, bool All, bool Group) CanBreakBones(IDamage damage)
    {
        if (damage == null)
        {
            return (false, false, false);
        }

        switch (damage.DamageType)
        {
            case DamageType.Hypoxia:
            case DamageType.Electrical:
            case DamageType.Cellular:
                return (false, false, false);
            case DamageType.Shockwave:
                return (true, true, false);
            case DamageType.Crushing:
            case DamageType.Falling:
            case DamageType.Chopping:
                return (true, false, true);
            case DamageType.Ballistic:
            case DamageType.Piercing:
            case DamageType.Slashing:
            case DamageType.Claw:
            case DamageType.Shrapnel:
            case DamageType.BallisticArmourPiercing:
            case DamageType.ArmourPiercing:
            case DamageType.Bite:
            case DamageType.Shearing:
            case DamageType.Wrenching:
            case DamageType.Eldritch:
            case DamageType.Arcane:
                return (true, false, false);
            default:
                return (false, false, false);
        }
    }

    private double BoneHitChance(double original, DamageType damagetype)
    {
        switch (damagetype)
        {
            case DamageType.BallisticArmourPiercing:
            case DamageType.Piercing:
            case DamageType.ArmourPiercing:
                return Math.Pow(original, 1.4);
            case DamageType.Ballistic:
            case DamageType.Bite:
                return Math.Pow(original, 1.2);
        }

        return original;
    }

    private (bool Truth, bool All, bool HighChance) CanDamageOrgans(IDamage damage)
    {
        if (damage == null)
        {
            return (false, false, false);
        }

        WoundSeverity severity = HealthStrategy.GetSeverity(damage.DamageAmount);
        switch (damage.DamageType)
        {
            case DamageType.Hypoxia:
            case DamageType.Electrical:
            case DamageType.Shockwave:
            case DamageType.Cellular:
                return (true, true, true);
            case DamageType.Crushing:
            case DamageType.Falling:
                return (true, false, true);
            case DamageType.Ballistic:
            case DamageType.BallisticArmourPiercing:
            case DamageType.ArmourPiercing:
            case DamageType.Piercing:
            case DamageType.Eldritch:
            case DamageType.Arcane:
                return (severity >= WoundSeverity.Moderate, false, false);
            case DamageType.Burning:
                return (severity >= WoundSeverity.Moderate, severity >= WoundSeverity.VerySevere,
                    severity >= WoundSeverity.Severe);
            default:
                return (severity >= WoundSeverity.Severe, false, false);
        }
    }

    private void HealingTick_MinuteHeartbeat()
    {
        if (!IsActiveCharacterBody)
        {
            EndHealthTickRegistration();
            return;
        }

        if (Combat == null)
        {
            List<IHealingRateEffect> healingEffects = CombinedEffectsOfType<IHealingRateEffect>().ToList();
            double healingMultiplier = healingEffects.Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum);
            double projectMultiplier = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
                                         .OfType<ILabourImpactHealing>()
                                         .Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum) ?? 1.0;
            double healingBonuses = healingEffects.Sum(x => x.HealingDifficultyStages) *
                                 Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel");
            double projectBonus = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
                                    .OfType<ILabourImpactHealing>().Sum(x => x.HealingCheckBonus) ?? 0.0;

            foreach (IWound wound in Wounds.ToList())
            {
                if (wound.HealingTick(healingMultiplier * projectMultiplier, healingBonuses + projectBonus))
                {
                    OnHeal?.Invoke(this, wound);
                }
            }
        }

        foreach (IInfection infection in PartInfections.ToList())
        {
            infection.InfectionTick();
        }

        HealthStrategy.PerformBloodGain(Actor);
        PerformLiverBreakdown();
        PerformKidneyFunction();
        PerformSpleenFunction();
        EvaluateWounds();
        EvaluateInfections();
        ReevaluateLimbAndPartDamageEffects();
    }

    private void PerformKidneyFunction()
    {
        HealthStrategy.PerformKidneyFunction(this);
    }

    private void PerformLiverBreakdown()
    {
        HealthStrategy.PerformLiverFunction(this);
    }

    private void PerformSpleenFunction()
    {
        HealthStrategy.PerformSpleenFunction(this);
    }

    private void HandleTargetEvent(EventType type)
    {
        foreach (ICharacter ch in (Actor.Combat?.Combatants.OfType<ICharacter>().Where(x => x.CombatTarget == Actor) ??
                            Enumerable.Empty<ICharacter>()).ToList())
        {
            ch.HandleEvent(type, ch, Actor);
        }
    }

    private void RecheckStatus()
    {
        if (Actor.State.HasFlag(CharacterState.Dead) || !IsActiveCharacterBody)
        {
            return;
        }

        CalculateOrganFunctions();
        ReevaluateLimbAndPartDamageEffects();

        HealthTickResult result = ApplyForcedParalysis(HealthStrategy.EvaluateStatus(Actor));
        HandleHealthStatusResult(result);
    }

    private HealthTickResult ApplyForcedParalysis(HealthTickResult result)
    {
        if (result.In(HealthTickResult.Dead, HealthTickResult.Unconscious, HealthTickResult.PassOut))
        {
            return result;
        }

        return CombinedEffectsOfType<IForceParalysisEffect>().Any(x => x.Applies() && x.ShouldParalyse)
            ? HealthTickResult.Paralyzed
            : result;
    }

    private void HandleHealthStatusResult(HealthTickResult result)
    {
        switch (result)
        {
            case HealthTickResult.Paralyzed:
                if (Actor.State.HasFlag(CharacterState.Unconscious))
                {
                    Actor.State &= ~CharacterState.Unconscious;
                    if (Actor.State.HasFlag(CharacterState.Sleeping))
                    {
                        if (!SuppressHealthFeedback)
                        {
                            Actor.OutputHandler.Send(Gameworld.GetStaticString("ComeToSleepingStillParalysedEcho"));
                        }
                    }
                    else
                    {
                        if (!SuppressHealthFeedback)
                        {
                            Actor.OutputHandler.Handle(
                                new EmoteOutput(new Emote(Gameworld.GetStaticString("ComeToStillParalysedEcho"), Actor)));
                        }
                    }
                }

                if (Actor.State.HasFlag(CharacterState.Paralysed))
                {
                    return;
                }

                Actor.State |= CharacterState.Paralysed;
                if (!SuppressHealthFeedback)
                {
                    Actor.OutputHandler.Send("");
                    Actor.OutputHandler.Handle(
                        new EmoteOutput(new Emote(Gameworld.GetStaticString("ParalysedEcho"), Actor)));
                }
                break;
            case HealthTickResult.Dead:
                if (Actor.State.HasFlag(CharacterState.Dead))
                {
                    return;
                }

                Actor.Die();
                break;
            case HealthTickResult.Unconscious:
                if (Actor.State.HasFlag(CharacterState.Unconscious))
                {
                    break;
                }

                Actor.State |= CharacterState.Unconscious;
                if (!SuppressHealthFeedback)
                {
                    Actor.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("FallUnconsciousEcho"),
                        Actor)));
                }
                if (Actor.PositionState.Upright)
                {
                    Actor.DoFallOffHorse();
                    Actor.SetPosition(PositionSprawled.Instance, Actor.PositionModifier, Actor.PositionTarget, null);
                }

                HandleTargetEvent(EventType.TargetIncapacitated);
                Actor.HandleEvent(EventType.CharacterIncapacitated, Actor);
                foreach (IHandleEvents witness in Actor.Location.EventHandlers)
                {
                    if (witness == Actor)
                    {
                        continue;
                    }

                    witness.HandleEvent(EventType.CharacterIncapacitatedWitness, Actor, witness);
                }

                IPerceiver combatTarget = Actor.CombatTarget;
                Actor.CombatTarget = null;
                combatTarget?.CheckCombatStatus();
                if (Actor.Combat?.CanFreelyLeaveCombat(Actor) ?? false)
                {
                    Actor.Combat.LeaveCombat(Actor);
                }

                Actor.Stop(true);
                AIStoryteller.HandleCharacterStateInRoomEvent(Actor, AIStorytellerStateTriggerType.Unconscious);
                break;
            case HealthTickResult.PassOut:
                PainTolerance pt = EffectHandler.EffectsOfType<PainTolerance>().FirstOrDefault();
                if (pt == null)
                {
                    AddEffect(new PainTolerance(this, Math.Max(0.0, HealthStrategy.MaxPain(Actor) - 0.01),
                        ExertionLevel.VeryHeavy));
                }

                if (Actor.State.HasFlag(CharacterState.Unconscious))
                {
                    break;
                }

                Actor.State |= CharacterState.Unconscious;
                if (!SuppressHealthFeedback)
                {
                    Actor.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("PassOutEcho"), Actor)));
                }
                if (Actor.PositionState.Upright)
                {
                    if (Actor.RidingMount is null)
                    {
                        Actor.SetPosition(PositionSprawled.Instance, Actor.PositionModifier, Actor.PositionTarget, null);
                    }
                }

                HandleTargetEvent(EventType.TargetIncapacitated);
                combatTarget = Actor.CombatTarget;
                Actor.CombatTarget = null;
                combatTarget?.CheckCombatStatus();
                if (Actor.Combat?.CanFreelyLeaveCombat(Actor) ?? false)
                {
                    Actor.Combat.LeaveCombat(Actor);
                }

                Actor.Stop(true);
                EffectHandler.AddEffect(new PassOutFromPain(this), TimeSpan.FromMinutes(5.0));
                AIStoryteller.HandleCharacterStateInRoomEvent(Actor, AIStorytellerStateTriggerType.PassOut);
                break;
            case HealthTickResult.None:
                if (Actor.State.HasFlag(CharacterState.Paralysed))
                {
                    if (!SuppressHealthFeedback)
                    {
                        Actor.OutputHandler.Handle(
                            new EmoteOutput(new Emote(Gameworld.GetStaticString("NoLongerParalysedEcho"), Actor)));
                    }
                    Actor.State &= ~CharacterState.Paralysed;
                }

                if (Actor.State.HasFlag(CharacterState.Unconscious))
                {
                    Actor.State &= ~CharacterState.Unconscious;
                    if (Actor.State.HasFlag(CharacterState.Sleeping))
                    {
                        if (!SuppressHealthFeedback)
                        {
                            Actor.OutputHandler.Send(Gameworld.GetStaticString("ComeToSleepingEcho"));
                        }
                        return;
                    }

                    if (!SuppressHealthFeedback)
                    {
                        Actor.OutputHandler.Handle(
                            new EmoteOutput(new Emote(Gameworld.GetStaticString("ComeToEcho"), Actor)));
                    }
                }

                break;
        }
    }

    private void HealthTick_TenSecondHeartbeat()
    {
        if (!IsActiveCharacterBody)
        {
            EndHealthTickRegistration();
            return;
        }

        HandleHealthStatusResult(ApplyForcedParalysis(HealthStrategy.PerformHealthTick(Actor)));
        CalculateOrganFunctions();
        ReevaluateLimbAndPartDamageEffects();
    }

    public void GenderChanged()
    {
        Gender = Actor.Gender;
        Changed = true;
        List<IBodypart> oldParts = Bodyparts.ToList();
        RecalculatePartsAndOrgans();
        CalculateOrganFunctions();
        foreach (IBodypart removedPart in oldParts.Where(x => !Bodyparts.Contains(x)).ToList())
        {
            _wornItems.RemoveAll(x => x.Wearloc == removedPart && !x.Profile.Mandatory);
            foreach ((IGameItem Item, IWear Wearloc, IWearlocProfile Profile) mandatory in _wornItems.Where(x => x.Wearloc == removedPart).ToList())
            {
                OutputHandler.Handle(new EmoteOutput(
                    new Emote("@ can no longer wear $0, and so it falls to the ground.", Actor, mandatory.Item)));
                Take(mandatory.Item);
                mandatory.Item.RoomLayer = RoomLayer;
                Location.Insert(mandatory.Item);
            }
        }

        foreach (IWear newPart in Bodyparts.Where(x => !oldParts.Contains(x)).OfType<IWear>().ToList())
        {
            foreach ((IGameItem Item, IWear Wearloc, IWearlocProfile Profile) wear in _wornItems.Where(
                                           x => x.Item.GetItemType<IWearable>()?.CurrentProfile.AllProfiles
                                                 .ContainsKey(newPart) == true)
                                       .ToList())
            {
                _wornItems.Add(
                    (wear.Item, newPart, wear.Item.GetItemType<IWearable>()?.CurrentProfile.AllProfiles[newPart]));
                InventoryChanged = true;
            }
        }
    }

    #region IBreathe Implementation

    private IBreathingStrategy _breathingStrategy;
    private TimeSpan _heldBreathTime;

    public bool NeedsToBreathe => !Actor.IsAdministrator() && !this.SuspendsPhysicalContact() && _breathingStrategy.NeedsToBreathe;
    public bool IsBreathing => _breathingStrategy.IsBreathing(this);

    public IBreathingStrategy BreathingStrategy => _breathingStrategy;

    public void DoBreathing()
    {
        if (!IsActiveCharacterBody)
        {
            return;
        }

        _breathingStrategy.Breathe(this);
        if (!CanBreathe && _breathingStrategy.NeedsToBreathe)
        {
            StartHealthTick();
        }
    }

    public TimeSpan HeldBreathTime
    {
        get => _heldBreathTime;
        set
        {
            if (value < TimeSpan.Zero)
            {
                _heldBreathTime = TimeSpan.Zero;
            }
            else
            {
                _heldBreathTime = value;
            }

            Changed = true;
        }
    }

    public double HeldBreathPercentage =>
        Math.Max(0.0, 1.0 - HeldBreathTime.TotalSeconds / Race.HoldBreathLength(this).TotalSeconds);

    public bool CanBreathe => this.SuspendsPhysicalContact() || _breathingStrategy.CanBreathe(this);

    public IFluid BreathingFluid => _breathingStrategy.BreathingFluid(this);

    #endregion

    #region IHaveContextualSizeCategoryMembers

    public SizeCategory SizeStanding => Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
                        .Aggregate(Race.SizeStanding, (x, y) => y.ContextualSize(x, SizeContext.None));

    public SizeCategory SizeProne =>
        // TODO - effects and merits that impact on this
        Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
                        .Aggregate(Race.SizeProne, (x, y) => y.ContextualSize(x, SizeContext.None));

    public SizeCategory SizeSitting =>
        // TODO - effects and merits that impact on this
        Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
                        .Aggregate(Race.SizeSitting, (x, y) => y.ContextualSize(x, SizeContext.None));

    public override SizeCategory Size => CurrentContextualSize(SizeContext.None);

    public SizeCategory CurrentContextualSize(SizeContext context)
    {
        if (PositionState == PositionSwimming.Instance)
        {
            return Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this)).Aggregate(Race.SizeProne, (x, y) => y.ContextualSize(x, context));
        }

        if (PositionState.Upright)
        {
            return Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
                        .Aggregate(Race.SizeStanding, (x, y) => y.ContextualSize(x, context));
        }

        if (PositionState.CompareTo(PositionProne.Instance) == PositionHeightComparison.Higher)
        {
            return Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
                        .Aggregate(Race.SizeSitting, (x, y) => y.ContextualSize(x, context));
        }

        return Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
                    .Aggregate(Race.SizeProne, (x, y) => y.ContextualSize(x, context));
    }

    #endregion

    public (double Lower, double Upper) TolerableTemperatures(bool includeClothing)
    {
        // TODO - effects
        double clothingInsulation = 0.0, clothingReflection = 0.0;
        if (includeClothing)
        {
            List<IInsulating> insulatingItems = _wornItems
                                  .SelectNotNull(x => x.Item.GetItemType<IInsulating>())
                                  .Distinct()
                                  .ToList();

            clothingInsulation = insulatingItems.Sum(x => x.InsulatingDegrees);
            clothingReflection =
                Outerwear.SelectNotNull(x => x.GetItemType<IInsulating>()).Sum(x => x.ReflectingDegrees);
        }

        List<ITemperatureRangeChangingMerit> merits = Actor.Merits.OfType<ITemperatureRangeChangingMerit>().Where(x => x.Applies(Actor))
                          .Concat(Merits.OfType<ITemperatureRangeChangingMerit>().Where(x => x.Applies(Actor)))
                          .ToList();

        return (
            Race.TemperatureRangeFloor + Ethnicity.TolerableTemperatureFloorEffect +
            Actor.Culture.TolerableTemperatureFloorEffect + merits.Sum(x => x.FloorEffect) - clothingInsulation +
            clothingReflection,
            Race.TemperatureRangeCeiling + Ethnicity.TolerableTemperatureCeilingEffect +
            Actor.Culture.TolerableTemperatureCeilingEffect + merits.Sum(x => x.CeilingEffect) - clothingInsulation +
            clothingReflection
        );
    }
}

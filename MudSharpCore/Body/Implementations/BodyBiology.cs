using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Body.PartProtos;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.Events;
using MudSharp.Health.Wounds;
using MudSharp.Health.Breathing;
using MudSharp.Work.Projects.Impacts;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	public event WoundEvent OnWounded;
	public event WoundEvent OnHeal;
	public event WoundEvent OnRemoveWound;

	private readonly IHealthStrategy _healthStrategy = null;

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
		EndStaminaTick(false);
		EndDrugTick();
		_breathingStrategy = new NonBreather();

		// Wielded items merely become held items
		foreach (var item in WieldedItems.ToList())
		{
			var heldLocation = _wieldedItems.FirstOrDefault(x => x.Item1 == item);
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
		if (_loading)
		{
			return;
		}

		EvaluateWounds();
		RecheckStatus();
		CheckBodypartDamage();
		CalculateOrganFunctions();
	}

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
		var arms = Limbs.Where(x => x.LimbType.In(LimbType.Arm, LimbType.Appendage, LimbType.Wing)).ToList();
		var nonWorkingLimbs = new List<ILimb>();
		var effects = CombinedEffectsOfType<ILimbIneffectiveEffect>().ToList();
		foreach (var arm in arms)
		{
			if (effects.Any(x => x.Applies(arm) && x.AppliesToLimb(arm)))
			{
				nonWorkingLimbs.Add(arm);
				continue;
			}

			var parts = BodypartsForLimb(arm).Where(x => x.Significant).ToList();

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
		var legs = Limbs.Where(x => x.LimbType == LimbType.Leg).ToList();
		var nonWorkingLimbs = new List<ILimb>();
		var effects = CombinedEffectsOfType<ILimbIneffectiveEffect>().ToList();
		foreach (var leg in legs)
		{
			if (effects.Any(x => x.Applies(leg) && x.AppliesToLimb(leg)))
			{
				nonWorkingLimbs.Add(leg);
				continue;
			}

			var parts = BodypartsForLimb(leg).ToList();
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
			var crutches = HeldItems.SelectNotNull(x => x.GetItemType<ICrutch>()).ToList();
			foreach (var limb in nonWorkingLimbs.ToArray())
			{
				var matchingCrutch =
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
		var (workingLegs, nonWorkingLegs) = GetLegInformation(ignoreAids);

		var workingLimbCount = workingLegs.Count;
		if (workingLimbCount >= Prototype.MinimumLegsToStand)
		{
			return true;
		}

		if (ignoreAids)
		{
			return false;
		}

		var targetLimbCount = workingLimbCount == 0
			? Prototype.MinimumLegsToStand + 1
			: Prototype.MinimumLegsToStand;

		var crutches = HeldItems.SelectNotNull(x => x.GetItemType<ICrutch>()).ToList();
		foreach (var limb in nonWorkingLegs.ToArray())
		{
			var matchingCrutch =
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
		var (workingLegs, nonWorkingLegs) = GetLegInformation(ignoreAids);
		var (workingArms, _) = GetArmAndAppendagesInformation();

		var workingLimbCount = workingLegs.Count;
		if (workingLimbCount >= Prototype.MinimumLegsToStand ||
		    (workingLimbCount >= Prototype.MinimumLegsToStand - 1 && workingArms.Count >= 1))
		{
			return true;
		}

		if (ignoreAids)
		{
			return false;
		}

		var targetLimbCount = workingLimbCount == 0
			? Prototype.MinimumLegsToStand + 1
			: Prototype.MinimumLegsToStand;

		var crutches = HeldItems.SelectNotNull(x => x.GetItemType<ICrutch>()).ToList();
		foreach (var limb in nonWorkingLegs.ToArray())
		{
			var matchingCrutch =
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
		var (workingLegs, _) = GetLegInformation(true);
		var (workingArms, _) = GetArmAndAppendagesInformation();
		return workingLegs.Count >= 1 || workingArms.Count >= 1;
	}

	private readonly DoubleCounter<Type> _cachedOrganFunctionsByType = new();
	private readonly DoubleCounter<IOrganProto> _cachedOrganFunctionsByOrgan = new();

	public void CalculateOrganFunctions(bool initialCalculation = false)
	{
		var oldCached = new DoubleCounter<IOrganProto>(_cachedOrganFunctionsByOrgan);
		_cachedOrganFunctionsByOrgan.Clear();
		_cachedOrganFunctionsByType.Clear();
		var merits = Actor.Merits.OfType<IOrganFunctionBonusMerit>()
		                  .Where(x => x.Applies(Actor))
		                  .SelectMany(x => x.OrganFunctionBonuses(this))
		                  .ToCollectionDictionary();
		var organs = Organs
		             .Select(x => (Organ: x, Factor: x.OrganFunctionFactor(this)))
		             .ToLookup(x => x.Organ, x => x.Factor);
		var implantOrgans = Implants
		                    .OfType<IOrganImplant>()
		                    .Select(x => (Organ: x.TargetOrgan, Factor: x.FunctionFactor))
		                    .ToCollectionDictionary();
		var external = Implants
		               .SelectNotNull(x => x.Parent.GetItemType<ICannula>())
		               .SelectMany(x =>
			               x.ConnectedItems.SelectNotNull(y => y.Item2.Parent.GetItemType<IExternalOrganFunction>()))
		               .Distinct()
		               .SelectMany(x => x.OrganFunctions.Select(y => (Organ: y.Organ, Factor: y.Function)))
		               .ToCollectionDictionary();

		// Pre-calculate spinal organs because other organs will depend on it
		foreach (var organ in Organs.OrderByDescending(x => x is SpineProto))
		{
			if (organ.RequiresSpinalConnection && HealthStrategy.RequiresSpinalCord)
			{
				if (GetLimbFor(organ).SpineProtos.Any(x => _cachedOrganFunctionsByOrgan[x] <= 0.0))
				{
					_cachedOrganFunctionsByOrgan[organ] = 0.0;
					_cachedOrganFunctionsByType[organ.GetType()] = 0.0;
					if (!initialCalculation)
					{
						organ.HandleChangedOrganFunction(this, oldCached[organ], 0.0);
					}

					continue;
				}
			}

			var bonus =
				merits[organ].Sum() +
				organs[organ].Sum() +
				implantOrgans[organ].Sum() +
				external[organ].Sum();
			_cachedOrganFunctionsByOrgan[organ] = bonus;
			_cachedOrganFunctionsByType[organ.GetType()] += bonus;
			if (!initialCalculation)
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

		var external = wound.Bodypart;
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
		var result = _wounds.ToList();
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

		_wounds.Add(wound);
		Changed = true;
	}

	public IEnumerable<IWound> InventoryExploded(IGameItem exploded, IExplosiveDamage damage)
	{
		var damageFactor = 1.0;
		var score = CurrentContextualSize(SizeContext.ExplosiveDamage) - damage.ExplosionSize;
		var scope = ExplosionDamageExtent.IndividualPart;
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

		var facing = Facing.Front;
		var bodyparts = new List<IBodypart>();
		var inventoryPart = TopLevelBodypart(BodypartLocationOfInventoryItem(exploded));

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
				foreach (var other in Bodyparts.Where(x => x.UpstreamConnection == inventoryPart))
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

				var limb = GetLimbFor(inventoryPart);
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
				var permittedAlignements = new HashSet<Alignment>();
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

		var newDamage = new ExplosiveDamage(damage, damageFactor, null);
		var wounds = new List<IWound>();
		Actor.AddEffect(new SupressWoundMessages(Actor)
			{ TargetTookWoundsEmote = "@ got caught up in the explosion!" });
		foreach (var part in bodyparts)
		{
			foreach (var item in damage.ReferenceDamages)
			{
				wounds.AddRange(PassiveSufferDamage(new Damage(item, damageFactor) { Bodypart = part }));
			}

			if (part is IGrab)
			{
				foreach (var gi in HeldOrWieldedItemsFor(part).ToList())
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

		var damageFactor = 1.0;
		var score = CurrentContextualSize(SizeContext.ExplosiveDamage) - damage.ExplosionSize + (int)proximity +
		            ((int?)Cover?.Cover.CoverExtent ?? 0) *
		            (Cover?.Cover.CoverType == MudSharp.Combat.CoverType.Hard ? 1 : 0);
		var scope = ExplosionDamageExtent.IndividualPart;
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

		var bodyparts = new List<IBodypart>();
		switch (scope)
		{
			case ExplosionDamageExtent.IndividualPart:
				var part = RandomBodyPartGeometry(
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
				foreach (var other in Bodyparts.Where(x => x.UpstreamConnection == part))
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

				var limb = GetLimbFor(part);
				if (limb != null)
				{
					bodyparts.AddRange(BodypartsForLimb(limb));
				}

				break;
			case ExplosionDamageExtent.EntireFacing:
				var permittedAlignements = new HashSet<Alignment>();
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

		var wounds = new List<IWound>();
		Actor.AddEffect(new SupressWoundMessages(Actor)
			{ TargetTookWoundsEmote = "@ got caught up in the explosion!" });
		foreach (var part in bodyparts)
		foreach (var item in damage.ReferenceDamages)
		{
			wounds.AddRange(PassiveSufferDamage(new Damage(item, damageFactor) { Bodypart = part }));
			if (part is IGrab)
			{
				foreach (var gi in HeldOrWieldedItemsFor(part).ToList())
				{
					wounds.AddRange(gi.PassiveSufferDamage(new Damage(item, damageFactor)));
				}
			}
		}

		return wounds;
	}

	public IEnumerable<IWound> PassiveSufferDamage(IDamage damage)
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
			var existingWound =
				Wounds.FirstOrDefault(x => x.Bodypart == damage.Bodypart && damage.DamageType == x.DamageType);
			if (existingWound == null)
			{
				existingWound = HealthStrategy.SufferDamage(Actor, damage, damage.Bodypart);
				if (existingWound == null)
				{
					return Enumerable.Empty<IWound>();
				}

				if (!_wounds.Contains(existingWound))
				{
					_wounds.Add(existingWound);
				}

				return new[] { existingWound };
			}

			existingWound.OriginalDamage += damage.DamageAmount;
			existingWound.CurrentDamage += damage.DamageAmount;
			existingWound.CurrentPain += damage.PainAmount;
			return new[] { existingWound };
		}

#if DEBUG
		var sb = new StringBuilder();
		try
		{
			sb.AppendLine(
				$"Original Damage: {damage.DamageAmount:N2} {damage.DamageType.Describe()} to {damage.Bodypart?.FullDescription() ?? "Unknown"}");
#endif

			var wounds = new List<IWound>();
			var isplainbodypart = !(damage.Bodypart is IOrganProto) && !(damage.Bodypart is IBone);
			if (isplainbodypart)
			{
				var armour =
					WornItemsProfilesFor(damage.Bodypart)
						.Where(x => !x.Item2.NoArmour)
						.SelectNotNull(x => x.Item1.GetItemType<IArmour>())
						.Reverse() // Last items worn are the first hit
						.ToList();

				foreach (var item in armour)
				{
					damage = item.PassiveSufferDamage(damage, ref wounds);
#if DEBUG
					sb.AppendLine(
						$"Armour ({item.Parent.HowSeen(item.Parent)}): Took {wounds.LastOrDefault(x => x.Parent == item.Parent)?.CurrentDamage.ToString("N2") ?? "None"}, Passed on: {(damage == null ? "Nothing" : $"{damage.DamageAmount:N2} {damage.DamageType.Describe()}")}");
#endif
					if (damage == null)
					{
						return wounds;
					}
				}

				if (damage.LodgableItem != null && wounds.Any(x => x.Lodged == damage.LodgableItem))
				{
					damage = new Damage(damage) { LodgableItem = null };
				}

				var targetProsthetic = Prosthetics.FirstOrDefault(
					x => x.TargetBodypart == damage.Bodypart || damage.Bodypart.DownstreamOfPart(x.TargetBodypart));
				if (targetProsthetic != null)
				{
#if DEBUG
					sb.AppendLine($"Hit Prosthetic");
#endif
					wounds.AddRange(targetProsthetic.Parent.PassiveSufferDamage(damage));
					return wounds;
				}
			}

			foreach (var magicarmour in CombinedEffectsOfType<IMagicArmour>())
			{
				if (!magicarmour.Applies() || !magicarmour.AppliesToPart(damage.Bodypart))
				{
					continue;
				}

				damage = magicarmour.PassiveSufferDamage(damage, ref wounds);
				if (damage == null)
				{
					return wounds;
				}
			}

			var internalDamage = damage;
			var naturalArmour = damage.Bodypart?.NaturalArmourType ?? Race.NaturalArmourType;
			if (naturalArmour != null)
			{
				var damages = naturalArmour.AbsorbDamage(damage, Race.NaturalArmourQuality,
					GetMaterial(damage.Bodypart), Actor, ref wounds);
				damage = damages.partDamage;
				internalDamage = damages.organDamge;
#if DEBUG
				sb.AppendLine(
					$"Natural Armour: Final {damage?.DamageAmount.ToString("N2") ?? "Nothing"}, Passed on: {(internalDamage == null ? "Nothing" : $"{internalDamage.DamageAmount:N2} {internalDamage.DamageType.Describe()}")}");
#endif
			}

			if (damage == null)
			{
				return wounds;
			}

			if (damage.LodgableItem != null && wounds.Any(x => x.Lodged == damage.LodgableItem))
			{
				damage = new Damage(damage) { LodgableItem = null };
			}

			var damage1 = damage;
			IBodypart severedPart = null;
			EffectHandler.RemoveAllEffects(
				x => x.GetSubtype<IEffectRemoveOnDamage>()?.RemovesWith(damage1) ?? false);
			if (isplainbodypart)
			{
				if (!CheckBoneDamage(ref damage, ref wounds, ref internalDamage, true))
				{
					CheckOrganDamage(ref damage, wounds, ref internalDamage, true);
				}

				if (damage.DamageAmount <= 0 && damage.StunAmount <= 0 && damage.PainAmount <= 0)
				{
					return wounds;
				}


				if (damage.Bodypart.CanSever && damage.DamageAmount >= Race.ModifiedSeverthreshold(damage.Bodypart) &&
				    damage.DamageType.CanSever())
				{
					severedPart = damage.Bodypart;
					damage = new Damage(damage) { Bodypart = damage.Bodypart.UpstreamConnection };
				}
			}

			var newWound = HealthStrategy.SufferDamage(Actor, damage, damage.Bodypart);
			if (newWound != null)
			{
				newWound.SeveredBodypart = severedPart;
			}

			if (newWound == null)
			{
				return wounds;
			}

			if (!_wounds.Contains(newWound))
			{
				_wounds.Add(newWound);
				Changed = true;
			}

			wounds.Add(newWound);
			return wounds;
#if DEBUG
		}
		finally
		{
			Actor.OutputHandler.Handle(new FilteredEmoteOutput(sb.ToString(), Actor, x => x.AffectedBy<DebugMode>(),
				flags: OutputFlags.WizOnly));
		}
#endif
	}

	private bool CheckOrganDamageSpecific(KeyValuePair<IOrganProto, BodypartInternalInfo> organInfo,
		IDamage internalDamage, bool damageAllOrgans, List<IWound> wounds, bool passive, bool highChance)
	{
		var organ = organInfo.Key;
		if (!damageAllOrgans &&
		    RandomUtilities.Random(0, 100) > organInfo.Value.HitChance * (highChance ? 2.0 : 1.0))
		{
			return false;
		}

		if (!damageAllOrgans && !highChance && Actor.Merits.OfType<IOrganHitReductionMerit>()
		                                            .Where(x => x.Applies(Actor)).Any(x =>
			                                            x.MissesOrgan(organInfo, internalDamage,
				                                            HealthStrategy.GetSeverity(internalDamage.DamageAmount))))
		{
			return false;
		}

		var newDamage = new Damage(internalDamage) { Bodypart = organ, LodgableItem = null };
		if (passive)
		{
			wounds.AddRange(PassiveSufferDamage(newDamage));
		}
		else
		{
			wounds.AddRange(SufferDamage(newDamage));
		}

		return true;
	}

	private void CheckOrganDamage(ref IDamage damage, List<IWound> wounds, ref IDamage internalDamage, bool passive)
	{
		var (damageOrgans, damageAllOrgans, highChance) = CanDamageOrgans(internalDamage);
		if (damage.Bodypart.Organs.Any() && internalDamage != null && damageOrgans)
		{
			foreach (var organInfo in damage.Bodypart.OrganInfo.Where(x => Organs.Contains(x.Key)).Shuffle().ToList())
			{
				if (CheckOrganDamageSpecific(organInfo, internalDamage, damageAllOrgans, wounds, passive, highChance))
				{
					var item = damage.LodgableItem;
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

	private bool CheckBoneDamage(ref IDamage damage, ref List<IWound> wounds, ref IDamage internalDamage, bool passive)
	{
		var (damageBones, damageAllBones, damageBoneGroups) = CanBreakBones(damage);
		var boneWounds = Wounds.Where(x => x.Bodypart is IBone).ToList();
		if (damageBones && damage.Bodypart.Bones.Any())
		{
			IDamage boneDamage, organDamage;
			var boneWasHit = false;
			foreach (var boneInfo in damage.Bodypart.BoneInfo.Where(x => Bones.Contains(x.Key)).Shuffle().ToList())
			{
				var bone = boneInfo.Key;
				if (!damageAllBones && Constants.Random.NextDouble() >
				    BoneHitChance(boneInfo.Value.HitChance / 100.0, internalDamage.DamageType))
				{
					continue;
				}

				boneWasHit = true;
				var naturalArmour = bone.NaturalArmourType ?? Race.NaturalArmourType;
				if (naturalArmour != null &&
				    boneWounds.Any(x => x.Bodypart == bone && x.Severity >= WoundSeverity.Grievous))
				{
					var damages = naturalArmour.AbsorbDamage(internalDamage, Race.NaturalArmourQuality,
						GetMaterial(bone), Actor,
						ref wounds);
					if (damages.partDamage == null)
					{
						return true;
					}

					organDamage = damages.organDamge;
				}
				else
				{
					organDamage = new Damage(internalDamage)
					{
						LodgableItem = null
					};
				}

				boneDamage = new Damage(internalDamage)
				{
					Bodypart = bone,
					LodgableItem = null
				};

				if (passive)
				{
					wounds.AddRange(PassiveSufferDamage(boneDamage));
				}
				else
				{
					wounds.AddRange(SufferDamage(boneDamage));
				}

				if (organDamage != null && organDamage.DamageAmount > 0)
				{
					foreach (var organ in bone.CoveredOrgans.Where(x => Organs.Contains(x.Organ)))
					{
						if (CheckOrganDamageSpecific(
							    new KeyValuePair<IOrganProto, BodypartInternalInfo>(organ.Organ, organ.Info),
							    organDamage, false, wounds, passive, false))
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
			var item = SeverBodypart(wound.SeveredBodypart);
			item.RoomLayer = RoomLayer;
			Location.Insert(item);
		}

		OnWounded?.Invoke(this, wound);
		wound.OnWoundSuffered();
	}

	public IEnumerable<IWound> SufferDamage(IDamage damage)
	{
		if (!Bodyparts.Contains(damage.Bodypart) && !Organs.Contains(damage.Bodypart) &&
		    !Bones.Contains(damage.Bodypart))
		{
			return Enumerable.Empty<IWound>();
		}

		if (damage.DamageType == DamageType.Cellular || damage.DamageType == DamageType.Hypoxia)
		{
			var existingWound =
				Wounds.FirstOrDefault(x => x.Bodypart == damage.Bodypart && damage.DamageType == x.DamageType);
			if (existingWound == null)
			{
				existingWound = HealthStrategy.SufferDamage(Actor, damage, damage.Bodypart);
				if (existingWound == null)
				{
					return Enumerable.Empty<IWound>();
				}

				if (!_wounds.Contains(existingWound))
				{
					_wounds.Add(existingWound);
				}

				OnWounded?.Invoke(this, existingWound);
				StartHealthTick();
				return new[] { existingWound };
			}

			existingWound.OriginalDamage += damage.DamageAmount;
			existingWound.CurrentDamage += damage.DamageAmount;
			existingWound.CurrentPain += damage.PainAmount;
			OnWounded?.Invoke(this, existingWound);
			return new[] { existingWound };
		}

		var wounds = new List<IWound>();
		var isplainbodypart = !(damage.Bodypart is IOrganProto) && !(damage.Bodypart is IBone);
		if (isplainbodypart)
		{
			var armour =
				WornItemsProfilesFor(damage.Bodypart)
					.Where(x => !x.Item2.NoArmour)
					.SelectNotNull(x => x.Item1.GetItemType<IArmour>())
					.Reverse() // Last items worn are the first hit
					.ToList();
			var originalDamage = damage;
			foreach (var item in armour)
			{
				damage = item.SufferDamage(damage, ref wounds);
				if (damage == null)
				{
					return wounds;
				}
			}

			if (damage.LodgableItem != null && wounds.Any(x => x.Lodged == damage.LodgableItem))
			{
				damage = new Damage(damage) { LodgableItem = null };
			}

			var targetProsthetic = Prosthetics.FirstOrDefault(
				x => damage.Bodypart == x.TargetBodypart || damage.Bodypart.DownstreamOfPart(x.TargetBodypart));
			if (targetProsthetic != null)
			{
				wounds.AddRange(targetProsthetic.Parent.SufferDamage(damage));
				return wounds;
			}
		}

		foreach (var magicarmour in CombinedEffectsOfType<IMagicArmour>())
		{
			if (!magicarmour.Applies() || !magicarmour.AppliesToPart(damage.Bodypart))
			{
				continue;
			}

			damage = magicarmour.PassiveSufferDamage(damage, ref wounds);
			if (damage == null)
			{
				return wounds;
			}
		}

		var internalDamage = damage;
		var naturalArmour = damage.Bodypart?.NaturalArmourType ?? Race.NaturalArmourType;
		if (naturalArmour != null)
		{
			var damages = naturalArmour.AbsorbDamage(damage, Race.NaturalArmourQuality,
				GetMaterial(damage.Bodypart), Actor, ref wounds);
			damage = damages.partDamage;
			internalDamage = damages.organDamge;
		}

		if (damage == null)
		{
			return wounds;
		}

		if (damage.LodgableItem != null && wounds.Any(x => x.Lodged == damage.LodgableItem))
		{
			damage = new Damage(damage) { LodgableItem = null };
		}

		if (isplainbodypart)
		{
			if (!CheckBoneDamage(ref damage, ref wounds, ref internalDamage, false))
			{
				CheckOrganDamage(ref damage, wounds, ref internalDamage, false);
			}

			if (damage.DamageAmount <= 0 && damage.StunAmount <= 0 && damage.PainAmount <= 0)
			{
				return wounds;
			}

			if (damage.Bodypart.CanSever && damage.DamageAmount >= Race.ModifiedSeverthreshold(damage.Bodypart) &&
			    damage.DamageType.CanSever())
			{
				var bodypart = damage.Bodypart;
				var severedPart = SeverBodypart(bodypart);
				severedPart.RoomLayer = RoomLayer;
				Location.Insert(severedPart);
				damage = new Damage(damage) { Bodypart = bodypart.UpstreamConnection };
			}
		}

		var newWound = HealthStrategy.SufferDamage(Actor, damage, damage.Bodypart);
		if (newWound == null)
		{
			StartHealthTick();
			return wounds;
		}

		if (!_wounds.Contains(newWound))
		{
			_wounds.Add(newWound);
		}

		OnWounded?.Invoke(this, newWound);
		newWound.OnWoundSuffered();
		wounds.Add(newWound);
		// Note - you specifically don't want to EvaluateWounds in here otherwise people may die before combat messages
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
		var woundsToRemove =
			Wounds.Where(
				      x =>
					      x.ShouldWoundBeRemoved())
			      .ToList();
		foreach (var wound in woundsToRemove)
		{
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
		var infectionsToRemove = PartInfections.Where(x => x.InfectionHealed()).ToList();

		foreach (var infection in infectionsToRemove)
		{
			infection.Delete();
			_partInfections.Remove(infection);
		}
	}

	public void StartHealthTick(bool initial = false)
	{
		if (Actor.State.HasFlag(CharacterState.Stasis))
		{
			return;
		}

		CalculateOrganFunctions(initial);
		ReevaluateLimbAndPartDamageEffects();
		var (floor, ceiling) = TolerableTemperatures(true);
		if (_healthTickActive ||
		    (!Wounds.Any() &&
		     !ActiveDrugDosages.Any() &&
		     !PartInfections.Any() &&
		     CurrentBloodVolumeLitres >= TotalBloodVolumeLitres &&
		     CanBreathe &&
		     HealthStrategy.CurrentTemperatureStatus(Actor) == BodyTemperatureStatus.NormalTemperature &&
		     TemperatureExtensions.SubjectiveTemperature(Location.CurrentTemperature(Actor), floor, ceiling) ==
		     Temperature.Temperate &&
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
		Gameworld.HeartbeatManager.TenSecondHeartbeat -= HealthTick_TenSecondHeartbeat;
		Gameworld.HeartbeatManager.MinuteHeartbeat -= HealingTick_MinuteHeartbeat;
		_healthTickActive = false;
	}

	public void CureAllWounds()
	{
		foreach (var wound in Wounds.ToList())
		{
			wound.Delete();
		}

		_wounds.Clear();
		foreach (var infection in PartInfections.ToList())
		{
			infection.Delete();
		}

		foreach (var bleed in EffectsOfType<IInternalBleedingEffect>().ToList())
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
		var healingEffects = CombinedEffectsOfType<IHealingRateEffect>().ToList();
		var healingMultiplier = healingEffects.Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum);
		var projectMultiplier = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
		                             .OfType<ILabourImpactHealing>()
		                             .Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum) ?? 1.0;
		var healingBonuses = healingEffects.Sum(x => x.HealingDifficultyStages) *
		                     Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel");
		var projectBonus = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
		                        .OfType<ILabourImpactHealing>().Sum(x => x.HealingCheckBonus) ?? 0.0;

		foreach (var wound in _wounds.ToList())
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
				return (true, false, true);
			case DamageType.Ballistic:
			case DamageType.Piercing:
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
			case DamageType.ArmourPiercing:
			case DamageType.Piercing:
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

		var severity = HealthStrategy.GetSeverity(damage.DamageAmount);
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
		if (Combat == null)
		{
			var healingEffects = CombinedEffectsOfType<IHealingRateEffect>().ToList();
			var healingMultiplier = healingEffects.Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum);
			var projectMultiplier = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
			                             .OfType<ILabourImpactHealing>()
			                             .Aggregate(1.0, (sum, x) => x.HealingRateMultiplier * sum) ?? 1.0;
			var healingBonuses = healingEffects.Sum(x => x.HealingDifficultyStages) *
			                     Gameworld.GetStaticDouble("CheckBonusPerDifficultyLevel");
			var projectBonus = Actor.CurrentProject.Labour?.LabourImpacts.Where(x => x.Applies(Actor))
			                        .OfType<ILabourImpactHealing>().Sum(x => x.HealingCheckBonus) ?? 0.0;

			foreach (var wound in Wounds.ToList())
			{
				if (wound.HealingTick(healingMultiplier * projectMultiplier, healingBonuses + projectBonus))
				{
					OnHeal?.Invoke(this, wound);
				}
			}
		}

		foreach (var infection in PartInfections.ToList())
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
		foreach (var ch in (Actor.Combat?.Combatants.OfType<ICharacter>().Where(x => x.CombatTarget == Actor) ??
		                    Enumerable.Empty<ICharacter>()).ToList())
		{
			ch.HandleEvent(type, ch, Actor);
		}
	}

	private void RecheckStatus()
	{
		if (Actor.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		CalculateOrganFunctions();
		ReevaluateLimbAndPartDamageEffects();

		var result = HealthStrategy.EvaluateStatus(Actor);
		HandleHealthStatusResult(result);
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
						Actor.OutputHandler.Send(Gameworld.GetStaticString("ComeToSleepingStillParalysedEcho"));
					}
					else
					{
						Actor.OutputHandler.Handle(
							new EmoteOutput(new Emote(Gameworld.GetStaticString("ComeToStillParalysedEcho"), Actor)));
					}
				}

				if (Actor.State.HasFlag(CharacterState.Paralysed))
				{
					return;
				}

				Actor.State |= CharacterState.Paralysed;
				Actor.OutputHandler.Send("");
				Actor.OutputHandler.Handle(
					new EmoteOutput(new Emote(Gameworld.GetStaticString("ParalysedEcho"), Actor)));
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
				Actor.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("FallUnconsciousEcho"),
					Actor)));
				if (Actor.PositionState.Upright)
				{
					Actor.SetPosition(PositionSprawled.Instance, Actor.PositionModifier, Actor.PositionTarget, null);
				}

				HandleTargetEvent(EventType.TargetIncapacitated);
				Actor.HandleEvent(EventType.CharacterIncapacitated, Actor);
				foreach (var witness in Actor.Location.EventHandlers)
				{
					if (witness == Actor)
					{
						continue;
					}

					witness.HandleEvent(EventType.CharacterIncapacitatedWitness, Actor, witness);
				}

				var combatTarget = Actor.CombatTarget;
				Actor.CombatTarget = null;
				combatTarget?.CheckCombatStatus();
				if (Actor.Combat?.CanFreelyLeaveCombat(Actor) ?? false)
				{
					Actor.Combat.LeaveCombat(Actor);
				}

				Actor.Stop(true);
				break;
			case HealthTickResult.PassOut:
				var pt = EffectHandler.EffectsOfType<PainTolerance>().FirstOrDefault();
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
				Actor.OutputHandler.Handle(new EmoteOutput(new Emote(Gameworld.GetStaticString("PassOutEcho"), Actor)));
				if (Actor.PositionState.Upright)
				{
					Actor.SetPosition(PositionSprawled.Instance, Actor.PositionModifier, Actor.PositionTarget, null);
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
				break;
			case HealthTickResult.None:
				if (Actor.State.HasFlag(CharacterState.Paralysed))
				{
					Actor.OutputHandler.Handle(
						new EmoteOutput(new Emote(Gameworld.GetStaticString("NoLongerParalysedEcho"), Actor)));
					Actor.State &= ~CharacterState.Paralysed;
				}

				if (Actor.State.HasFlag(CharacterState.Unconscious))
				{
					Actor.State &= ~CharacterState.Unconscious;
					if (Actor.State.HasFlag(CharacterState.Sleeping))
					{
						Actor.OutputHandler.Send(Gameworld.GetStaticString("ComeToSleepingEcho"));
						return;
					}

					Actor.OutputHandler.Handle(
						new EmoteOutput(new Emote(Gameworld.GetStaticString("ComeToEcho"), Actor)));
				}

				break;
		}
	}

	private void HealthTick_TenSecondHeartbeat()
	{
		HandleHealthStatusResult(HealthStrategy.PerformHealthTick(Actor));
		CalculateOrganFunctions();
		ReevaluateLimbAndPartDamageEffects();
	}

	public void GenderChanged()
	{
		Gender = Actor.Gender;
		Changed = true;
		var oldParts = Bodyparts.ToList();
		RecalculatePartsAndOrgans();
		CalculateOrganFunctions();
		foreach (var removedPart in oldParts.Where(x => !Bodyparts.Contains(x)).ToList())
		{
			_wornItems.RemoveAll(x => x.Wearloc == removedPart && !x.Profile.Mandatory);
			foreach (var mandatory in _wornItems.Where(x => x.Wearloc == removedPart).ToList())
			{
				OutputHandler.Handle(new EmoteOutput(
					new Emote("@ can no longer wear $0, and so it falls to the ground.", Actor, mandatory.Item)));
				Take(mandatory.Item);
				mandatory.Item.RoomLayer = RoomLayer;
				Location.Insert(mandatory.Item);
			}
		}

		foreach (var newPart in Bodyparts.Where(x => !oldParts.Contains(x)).OfType<IWear>().ToList())
		foreach (var wear in _wornItems.Where(
			                               x => x.Item.GetItemType<IWearable>()?.CurrentProfile.AllProfiles
			                                     .ContainsKey(newPart) == true)
		                               .ToList())
		{
			_wornItems.Add(
				(wear.Item, newPart, wear.Item.GetItemType<IWearable>()?.CurrentProfile.AllProfiles[newPart]));
			InventoryChanged = true;
		}
	}

	#region IBreathe Implementation

	private IBreathingStrategy _breathingStrategy;
	private TimeSpan _heldBreathTime;

	public bool NeedsToBreathe => !Actor.IsAdministrator() && _breathingStrategy.NeedsToBreathe;
	public bool IsBreathing => _breathingStrategy.IsBreathing(this);

	public IBreathingStrategy BreathingStrategy => _breathingStrategy;

	public void DoBreathing()
	{
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

	public bool CanBreathe => _breathingStrategy.CanBreathe(this);

	public IFluid BreathingFluid => _breathingStrategy.BreathingFluid(this);

	#endregion

	#region IHaveContextualSizeCategoryMembers

	public SizeCategory SizeStanding
	{
		get
		{
			return Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
			            .Aggregate(Race.SizeStanding, (x, y) => y.ContextualSize(x, SizeContext.None));
		}
	}

	public SizeCategory SizeProne
	{
		get
		{
			// TODO - effects and merits that impact on this
			return Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
			            .Aggregate(Race.SizeProne, (x, y) => y.ContextualSize(x, SizeContext.None));
		}
	}

	public SizeCategory SizeSitting
	{
		get
		{
			// TODO - effects and merits that impact on this
			return Actor.Merits.OfType<IContextualSizeMerit>().Where(x => x.Applies(this))
			            .Aggregate(Race.SizeSitting, (x, y) => y.ContextualSize(x, SizeContext.None));
		}
	}

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
			var insulatingItems = _wornItems
			                      .Select(x => (x.Wearloc, Insulating: x.Item.GetItemType<IInsulating>()))
			                      .Where(x => x.Insulating != null)
			                      .ToList();

			clothingInsulation = insulatingItems.Sum(x => x.Insulating.InsulatingDegrees);
			clothingReflection =
				Outerwear.SelectNotNull(x => x.GetItemType<IInsulating>()).Sum(x => x.ReflectingDegrees);
		}

		var merits = Actor.Merits.OfType<ITemperatureRangeChangingMerit>().Where(x => x.Applies(Actor))
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
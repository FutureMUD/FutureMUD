using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body.Disfigurements;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	private readonly HashSet<IBodypart> _severedRoots = new();
	private readonly HashSet<IBodypart> _excludedParts = new();
	private HashSet<IBodypart> _bodyparts;
	private bool _bodypartsChanged;

	private HashSet<IOrganProto> _organs;
	private HashSet<IBone> _bones;

	#region Overrides of PerceivedItem

	public override IEnumerable<IWound> ExplosionEmantingFromPerceivable(IExplosiveDamage damage)
	{
		return Actor.ExplosionEmantingFromPerceivable(damage);
	}

	#endregion

	public bool BodypartsChanged
	{
		get => _bodypartsChanged;
		set
		{
			if (!_bodypartsChanged && value && !_noSave)
			{
				Changed = true;
			}

			_bodypartsChanged = value;
		}
	}

	public bool CanSeverBodypart(IBodypart bodypart)
	{
		if (!bodypart.CanSever)
		{
			return false;
		}

		if (!Bodyparts.Contains(bodypart))
		{
			return false;
		}

		return true;
	}

	public IGameItem SeverBodypart(IBodypart bodypart)
	{
		if (!bodypart.CanSever)
		{
			return null;
		}

		if (!Bodyparts.Contains(bodypart))
		{
			return null;
		}

		var limb = GetLimbFor(bodypart);
		if (!Actor.AffectedBy<SupressWoundMessages>())
		{
			if (limb != null && bodypart.Significant && !limb.Name.EqualTo(bodypart.Name))
			{
				OutputHandler.Handle(
					new EmoteOutput(new Emote($"$0's {limb.Name} is severed at the {bodypart.FullDescription()}!",
						Actor,
						Actor)));
			}
			else if (!(bodypart is IOrganProto))
			{
				OutputHandler.Handle(
					new EmoteOutput(new Emote($"$0's {bodypart.FullDescription()} is severed!", Actor,
						Actor)));
			}
		}

		var severedParts = Bodyparts.Where(x => x.DownstreamOfPart(bodypart)).ToHashSet();
		severedParts.Add(bodypart);
		_severedRoots.Add(bodypart);
		_severedRoots.RemoveWhere(x => x.DownstreamOfPart(bodypart));
		BodypartsChanged = true;
		RecalculatePartsAndOrgans();
		ReevaluateLimbAndPartDamageEffects();

		var heldOrWieldedItems =
			_wieldedItems.Where(x => severedParts.Any(y => y.CountsAs(x.Item2)))
			             .Select(x => x.Item1)
			             .Concat(_heldItems.Where(x => severedParts.Contains(x.Item2)).Select(x => x.Item1))
			             .ToHashSet();
		foreach (var item in heldOrWieldedItems)
		{
			Take(item);
			item.RoomLayer = RoomLayer;
			Actor.Location.Insert(item);
			Actor.OutputHandler.Handle(
				new EmoteOutput(new Emote("@ can no longer hold onto $0 and drop|drops it.", Actor, item)));
			if (Actor.Combat != null)
			{
				Actor.AddEffect(new CombatGetItemEffect(Actor, item));
				item.AddEffect(new CombatNoGetEffect(item, Actor.Combat), TimeSpan.FromSeconds(20));
			}
		}

		var wornItems =
			_wornItems.Where(x => severedParts.Contains(x.Item2) && x.Item3.Mandatory)
			          .Select(x => x.Item1)
			          .ToHashSet();
		foreach (var item in wornItems)
		{
			Take(item);
		}

		var removedWounds =
			Wounds.Where(
				      x => severedParts.Any(y => y.CountsAs(x.Bodypart)) ||
				           severedParts.Any(y => y.Organs.Any(z => z.CountsAs(x.Bodypart))))
			      .ToList();
		_wounds.RemoveAll(x => removedWounds.Contains(x));

		_partInfections.RemoveAll(x => severedParts.Any(y => y.CountsAs(x.Bodypart)));
		var severedPartsAndOrgans = severedParts.SelectMany(x => x.Organs).Distinct().Concat(severedParts).ToList();
		var severedBones = severedParts.SelectMany(x => x.Bones).Distinct().ToList();

		var removedImplants =
			_implants.Where(x => severedPartsAndOrgans.Any(y => y.CountsAs(x.TargetBodypart))).ToList();
		foreach (var implant in removedImplants)
		{
			RemoveImplant(implant);
		}

		var removedTattoos = _tattoos.Where(x => severedPartsAndOrgans.Any(y => y.CountsAs(x.Bodypart))).ToList();
		foreach (var tattoo in removedTattoos)
		{
			_tattoos.Remove(tattoo);
		}

		TattoosChanged = true;

		RemoveAllEffects(x =>
			severedPartsAndOrgans.Any(y => y.CountsAs(x.GetSubtype<IPertainToBodypartEffect>()?.Bodypart)));
		var newPart = BodypartGameItemComponentProto.CreateNewSeveredBodypart(Actor, severedParts, wornItems,
			removedImplants.Select(x => x.Parent), severedBones, removedTattoos, removedWounds);
		newPart.RoomLayer = Actor.Corpse?.Parent.RoomLayer ?? Actor.RoomLayer;
		if (Actor.Corpse != null)
		{
			newPart.GetItemType<ISeveredBodypart>().DecayPoints = Actor.Corpse.DecayPoints;
		}

		RecalculateItemHelpers();
		return newPart;
	}

	public void RestoreBodypart(IBodypart bodypart)
	{
		if (_severedRoots.Remove(bodypart))
		{
			BodypartsChanged = true;
			RecalculatePartsAndOrgans();

			var limb = GetLimbFor(bodypart);
			if (limb != null)
			{
				var bodypartIDs = Bodyparts.Where(x => limb.Parts.Contains(x)).Select(x => x.Id).ToHashSet();
				var baseBodyparts =
					Prototype.BodypartsFor(Race, Gender.Enum).Where(x => limb.Parts.Contains(x)).ToHashSet();
				var missingParts = baseBodyparts.Where(x => !bodypartIDs.Contains(x.Id)).ToHashSet();
				if (missingParts.All(x => !x.Significant))
				{
					EffectHandler.RemoveAllEffects(x => x.GetSubtype<LimbSeveredEffect>()?.Limb == limb);
				}
			}

			ReevaluateLimbAndPartDamageEffects();
			RecalculateItemHelpers();
		}
	}

	public void RestoreAllBodypartsOrgansAndBones()
	{
		_severedRoots.Clear();
		BodypartsChanged = true;
		EffectHandler.RemoveAllEffects<LimbSeveredEffect>();
		EffectHandler.RemoveAllEffects<LimbMissingBoneEffect>();
		RecalculatePartsAndOrgans();
		ReevaluateLimbAndPartDamageEffects();
		RecalculateItemHelpers();
	}

	public IGameItem ExciseOrgan(IOrganProto organ)
	{
		if (!Organs.Contains(organ))
		{
			return null;
		}

		_severedRoots.Add(organ);
		BodypartsChanged = true;
		RecalculatePartsAndOrgans();
		ReevaluateLimbAndPartDamageEffects();

		var removedWounds = Wounds.Where(x => x.Bodypart == organ).ToList();
		foreach (var wound in removedWounds)
		{
			wound.Delete();
		}

		_wounds.RemoveAll(x => removedWounds.Contains(x));
		RemoveAllEffects(x => organ == x.GetSubtype<IPertainToBodypartEffect>()?.Bodypart);
		var removedImplants = _implants.Where(x => x.TargetBodypart == organ).ToList();
		foreach (var implant in removedImplants)
		{
			RemoveImplant(implant);
		}

		var newPart = BodypartGameItemComponentProto.CreateNewSeveredBodypart(Actor, new[] { organ },
			Enumerable.Empty<IGameItem>(), removedImplants.Select(x => x.Parent), Enumerable.Empty<IBone>(),
			Enumerable.Empty<ITattoo>(), Enumerable.Empty<IWound>());
		if (Actor.Corpse != null)
		{
			newPart.GetItemType<ISeveredBodypart>().DecayPoints = Actor.Corpse.DecayPoints;
		}

		RecalculateItemHelpers();
		return newPart;
	}

	public void RestoreOrgan(IOrganProto organ)
	{
		if (_severedRoots.Remove(organ))
		{
			BodypartsChanged = true;
			RecalculatePartsAndOrgans();
			ReevaluateLimbAndPartDamageEffects();
			RecalculateItemHelpers();
		}
	}

	public IEnumerable<IBodypart> Bodyparts
	{
		get
		{
			if (_bodyparts == null)
			{
				RecalculatePartsAndOrgans();
			}

			return _bodyparts;
		}
	}

	public IEnumerable<IOrganProto> Organs
	{
		get
		{
			if (_organs == null)
			{
				RecalculatePartsAndOrgans();
			}

			return _organs;
		}
	}

	public IEnumerable<IBone> Bones
	{
		get
		{
			if (_bones == null)
			{
				RecalculatePartsAndOrgans();
			}

			return _bones;
		}
	}

	public void RecalculatePartsAndOrgans()
	{
		var merits = Merits.Concat(Actor.Merits)
		                   .OfType<IAdditionalBodypartsMerit>()
		                   .Where(x => x.Applies(Actor)).ToList();

		var severedParts = _severedRoots
		                   .Where(x => Prosthetics.All(y => y.TargetBodypart != x))
		                   .Concat(merits.SelectMany(x => x.RemovedBodyparts(Actor)))
		                   .Distinct()
		                   .ToList();

		var allParts = Prototype
		               .BodypartsFor(Race, Gender.Enum)
		               .Concat(merits.SelectMany(x => x.AddedBodyparts(Actor)))
		               .Distinct()
		               .PruneBodyparts(severedParts)
		               .ToHashSet();

		var newBodyparts = allParts.Where(x => x is IExternalBodypart).ToHashSet();
		var organs = allParts.OfType<IOrganProto>()
		                     .Where(x => !severedParts.Contains(x) &&
		                                 newBodyparts.Any(y =>
			                                 y.OrganInfo.Any(z => z.Key == x && z.Value.IsPrimaryInternalLocation)))
		                     .Distinct()
		                     .ToHashSet();

		var bones = allParts.OfType<IBone>()
		                    .Where(x => !severedParts.Contains(x) &&
		                                newBodyparts.Any(y =>
			                                y.BoneInfo.Any(z => z.Key == x && z.Value.IsPrimaryInternalLocation)))
		                    .Distinct()
		                    .ToHashSet();

		if (_bodyparts == null)
		{
			_bodyparts = newBodyparts;
			_organs = organs;
			_bones = bones;
			SetupStandardParts();
			return;
		}

		if (!newBodyparts.SequenceEqual(_bodyparts) || !organs.SequenceEqual(_organs) || !bones.SequenceEqual(_bones))
		{
			_bodyparts = newBodyparts;
			_organs = organs;
			_bones = bones;
			SetupStandardParts();
			BodypartsChanged = true;
		}
	}

	private List<ILimb> _limbs = new();
	public IEnumerable<ILimb> Limbs => _limbs;

	public IEnumerable<IBodypart> BodypartsForLimb(ILimb limb)
	{
		if (limb == null)
		{
			return Enumerable.Empty<IBodypart>();
		}

		return Bodyparts.Where(x => limb.Parts.Contains(x)).ToList();
	}

	public IEnumerable<IBodypart> SeveredRoots => _severedRoots;

	public CanUseBodypartResult CanUseBodypart(IBodypart part)
	{
		var limbResult = CanUseLimb(GetLimbFor(part));
		if (limbResult.HasFlag(CanUseLimbResult.CantUseDamage))
		{
			return CanUseBodypartResult.CantUseLimbDamage;
		}

		if (limbResult.HasFlag(CanUseLimbResult.CantUsePain))
		{
			return CanUseBodypartResult.CantUseLimbPain;
		}

		if (limbResult.HasFlag(CanUseLimbResult.CantUseSevered))
		{
			return CanUseBodypartResult.CantUseSevered;
		}

		if (limbResult.HasFlag(CanUseLimbResult.CantUseGrappled))
		{
			return CanUseBodypartResult.CantUseLimbGrappled;
		}

		if (limbResult.HasFlag(CanUseLimbResult.CantUseMissingBone))
		{
			return CanUseBodypartResult.CantUseMissingBone;
		}

		if (limbResult.HasFlag(CanUseLimbResult.CantUseSpinalDamage))
		{
			return CanUseBodypartResult.CantUseSpinalDamage;
		}

		if (EffectsOfType<IBodypartIneffectiveEffect>().Any(x => x.Applies() && x.Bodypart == part))
		{
			return CanUseBodypartResult.CantUsePartDamage;
		}

		return Prosthetics.Any(
			x => (!x.Functional && part.DownstreamOfPart(x.TargetBodypart)) || part == x.TargetBodypart)
			? CanUseBodypartResult.CantUseNonFunctionalProsthetic
			: CanUseBodypartResult.CanUse;

		// TODO - part pain
	}

	public CanUseLimbResult CanUseLimb(ILimb limb)
	{
		if (limb == null)
		{
			return CanUseLimbResult.CanUse;
		}

		var result = CanUseLimbResult.CanUse;
		var limbEffects = EffectsOfType<ILimbIneffectiveEffect>().Where(x => x.Applies(limb) && x.AppliesToLimb(limb))
		                                                         .ToList();
		if (limbEffects.Any(x => x.Reason == LimbIneffectiveReason.Severing))
		{
			result |= CanUseLimbResult.CantUseSevered;
		}

		if (limbEffects.Any(x => x.Reason == LimbIneffectiveReason.SpinalDamage))
		{
			result |= CanUseLimbResult.CantUseSpinalDamage;
		}

		if (limbEffects.Any(x => x.Reason == LimbIneffectiveReason.Grappling))
		{
			result |= CanUseLimbResult.CantUseGrappled;
		}

		if (limbEffects.Any(x => x.Reason == LimbIneffectiveReason.Restrained))
		{
			result |= CanUseLimbResult.CantUseRestrained;
		}

		if (limbEffects.Any(x => x.Reason == LimbIneffectiveReason.Damage))
		{
			result |= CanUseLimbResult.CantUseDamage;
		}

		if (limbEffects.Any(x => x.Reason == LimbIneffectiveReason.Pain))
		{
			result |= CanUseLimbResult.CantUsePain;
		}

		if (limb.Parts.SelectMany(x => x.Bones).Where(x => x.IsVital).Any(x => !_bones.Contains(x)))
		{
			result |= CanUseLimbResult.CantUseMissingBone;
		}

		return result;
	}

	public bool ReevaluateLimbAndPartDamageEffects()
	{
		var result = CheckBodypartDamage();

		foreach (var limb in Limbs)
		{
			var canUseLimbResult = EvaluateCanUseLimb(limb);
			if (canUseLimbResult.HasFlag(CanUseLimbResult.CantUseSevered))
			{
				if (EffectsOfType<LimbSeveredEffect>().All(x => x.Limb != limb))
				{
					result = LimbDamageEffects(limb, CanUseLimbResult.CantUseSevered) || result;
				}
			}
			else
			{
				RemoveAllEffects(x => x.GetSubtype<LimbSeveredEffect>()?.Limb == limb);
			}

			if (canUseLimbResult.HasFlag(CanUseLimbResult.CantUseDamage))
			{
				if (EffectsOfType<LimbDamageEffect>().All(x => x.Limb != limb))
				{
					result = LimbDamageEffects(limb, CanUseLimbResult.CantUseDamage) || result;
				}
			}
			else
			{
				RemoveAllEffects(x => x.GetSubtype<LimbDamageEffect>()?.Limb == limb);
			}

			if (canUseLimbResult.HasFlag(CanUseLimbResult.CantUsePain))
			{
				if (EffectsOfType<LimbPainEffect>().All(x => x.Limb != limb))
				{
					result = LimbDamageEffects(limb, CanUseLimbResult.CantUsePain) || result;
				}
			}
			else
			{
				RemoveAllEffects(x => x.GetSubtype<LimbPainEffect>()?.Limb == limb);
			}

			if (canUseLimbResult.HasFlag(CanUseLimbResult.CantUseMissingBone))
			{
				if (EffectsOfType<LimbMissingBoneEffect>().All(x => x.Limb != limb))
				{
					result = LimbDamageEffects(limb, CanUseLimbResult.CantUseMissingBone) || result;
				}
			}
			else
			{
				RemoveAllEffects(x => x.GetSubtype<LimbMissingBoneEffect>()?.Limb == limb);
			}

			if (canUseLimbResult.HasFlag(CanUseLimbResult.CantUseSpinalDamage))
			{
				if (EffectsOfType<LimbSpinalDamageEffect>().All(x => x.Limb != limb))
				{
					result = LimbDamageEffects(limb, CanUseLimbResult.CantUseSpinalDamage) || result;
				}
			}
			else
			{
				RemoveAllEffects(x => x.GetSubtype<LimbSpinalDamageEffect>()?.Limb == limb);
			}
		}

		return result;
	}

	private CanUseLimbResult EvaluateCanUseLimb(ILimb limb)
	{
		var result = CanUseLimbResult.CanUse;
		var merits = Merits.OfType<IAdditionalBodypartsMerit>().Where(x => x.Applies(this)).ToList();
		var baseBodyparts = Prototype
		                    .BodypartsFor(Race, Gender.Enum)
		                    .Concat(merits.SelectMany(x => x.AddedBodyparts(Actor)))
		                    .Distinct()
		                    .PruneBodyparts(merits.SelectMany(x => x.RemovedBodyparts(Actor)))
		                    .Where(x => limb.Parts.Contains(x)).ToHashSet();

		var bodypartIDs = Bodyparts.Where(x => limb.Parts.Contains(x)).Select(x => x.Id).ToHashSet();

		var missingParts = baseBodyparts.Where(x => !bodypartIDs.Contains(x.Id)).ToHashSet();
		if (missingParts.Any(x => x.Significant))
		{
			result |= CanUseLimbResult.CantUseSevered;
		}

		if (baseBodyparts.SelectMany(x => x.Bones).Where(x => x.CriticalBone && baseBodyparts.Contains(x))
		                 .Any(x => !_bones.Contains(x)))
		{
			result |= CanUseLimbResult.CantUseMissingBone;
		}

		var wounds = GetWoundsForLimb(limb).ToList();
		var maxHP = HealthStrategy.MaxHP(Actor);
		if (maxHP > 0 && wounds.Sum(x => x.CurrentDamage) >=
		    maxHP * Race.DamageToleranceModifier * limb.LimbDamageThresholdMultiplier)
		{
			result |= CanUseLimbResult.CantUseDamage;
		}

		if (wounds.Any(x =>
			    x.Bodypart is IBone b && x is BoneFracture bf && b.CriticalBone && x.Severity >= WoundSeverity.Severe &&
			    !bf.HasBeenSurgicallyReinforced && bf.Stage != BoneFractureStage.Ossification))
		{
			result |= CanUseLimbResult.CantUseDamage;
		}

		var maxPain = HealthStrategy.MaxPain(Actor);
		if (maxPain > 0 && wounds.Sum(x => x.CurrentPain) >=
		    maxPain * Race.PainToleranceModifier * limb.LimbPainThresholdMultiplier)
		{
			result |= CanUseLimbResult.CantUsePain;
		}

		if (HealthStrategy.RequiresSpinalCord && limb.SpineProtos.Any(x => OrganFunction(x) <= 0.0))
		{
			result |= CanUseLimbResult.CantUseSpinalDamage;
		}

		return result;
	}

	private bool LimbDamageEffects(ILimb limb, CanUseLimbResult why)
	{
		switch (why)
		{
			case CanUseLimbResult.CantUseDamage:
				AddEffect(new LimbDamageEffect(this, limb));
				break;
			case CanUseLimbResult.CantUsePain:
				AddEffect(new LimbPainEffect(this, limb));
				break;
			case CanUseLimbResult.CantUseSevered:
				AddEffect(new LimbSeveredEffect(this, limb));
				break;
			case CanUseLimbResult.CantUseMissingBone:
				AddEffect(new LimbMissingBoneEffect(this, limb));
				break;
			case CanUseLimbResult.CantUseSpinalDamage:
				AddEffect(new LimbSpinalDamageEffect(this, limb));
				break;
		}

		var result = false;
		foreach (var item in limb.Parts.OfType<IGrab>().SelectMany(HeldOrWieldedItemsFor).ToList())
		{
			switch (why)
			{
				case CanUseLimbResult.CantUseDamage:
					Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {limb.Name.ToLowerInvariant()} is too badly damaged.",
								Actor, Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseLimbResult.CantUsePain:
					Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because &0's {limb.Name.ToLowerInvariant()} is in too much pain.",
								Actor, Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseLimbResult.CantUseSevered:
					Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ can no longer hold onto $0 because &0's {limb.Name.ToLowerInvariant()} has been severed, and drop|drops it.",
								Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseLimbResult.CantUseGrappled:
					Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ can no longer hold onto $0 because &0's {limb.Name.ToLowerInvariant()} has been grappled.",
								Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseLimbResult.CantUseMissingBone:
					Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ can no longer hold onto $0 because &0's {limb.Name.ToLowerInvariant()} has gone unnaturally floppy, as if it was missing a key bone!",
								Actor, item), style: OutputStyle.NoNewLine));
					break;
				case CanUseLimbResult.CantUseSpinalDamage:
					Actor.OutputHandler.Handle(
						new EmoteOutput(
							new Emote(
								$"@ lose|loses &0's grip on $1 because #0 can no longer move &0's {limb.Name.ToLowerInvariant()}.",
								Actor, Actor, item), style: OutputStyle.NoNewLine));
					break;
			}

			result = true;
			Drop(item, silent: true);
		}

		if (limb.LimbType == LimbType.Leg)
		{
			CheckPositionStillValid();
		}

		if (limb.LimbType == LimbType.Wing)
		{
			Actor.CheckCanFly();
		}

		return result;
	}

	public bool CheckBodypartDamage()
	{
		var result = false;
		var woundsQuery = Wounds.Where(x => !(x.Bodypart is IOrganProto) && !(x.Bodypart is IBone))
		                        .GroupBy(x => x.Bodypart).ToList();
		foreach (var part in woundsQuery)
		{
			if (part.Key == null)
			{
				continue;
			}

			if (part.Sum(x => x.CurrentDamage) >= HitpointsForBodypart(part.Key))
			{
				result = part.Key.PartDamageEffects(this, CanUseBodypartResult.CantUsePartDamage) || result;
			}
			else
			{
				EffectHandler.RemoveAllEffects(
					x =>
						x.GetSubtype<BodypartExcessivelyDamaged>()?.Bodypart == part.Key ||
						x.GetSubtype<InsignificantBodypartExcessivelyDamaged>()?.Bodypart == part.Key);
			}

			if (part.Sum(x => x.CurrentPain) >= HitpointsForBodypart(part.Key))
			{
				result = part.Key.PartDamageEffects(this, CanUseBodypartResult.CantUsePartPain) || result;
			}
			else
			{
				EffectHandler.RemoveAllEffects(x => x.GetSubtype<BodypartExcessivelyPainful>()?.Bodypart == part.Key);
			}
		}

		EffectHandler.RemoveAllEffects(
			x =>
				(x.IsEffectType<BodypartExcessivelyDamaged>() || x.IsEffectType<BodypartExcessivelyPainful>() ||
				 x.IsEffectType<InsignificantBodypartExcessivelyDamaged>()) &&
				woundsQuery.All(y => y.Key != x.GetSubtype<IBodypartIneffectiveEffect>().Bodypart));
		return result;
	}

	public IBodypart RandomBodypart
	{
		get { return Bodyparts.GetWeightedRandom(x => x.RelativeHitChance); }
	}

	public IBodypart RandomBodypartOrOrgan
	{
		get { return Bodyparts.Concat(Organs).GetWeightedRandom(x => x.RelativeHitChance); }
	}

	public IBodypart RandomBodyPartGeometry(Orientation orientation, Alignment alignment, Facing facing,
		bool appendagesActive = false)
	{
		var roll = BodyExtensions.GetRandomHitLocation(orientation, alignment, facing, appendagesActive);
		var eligableBodyparts =
			Bodyparts.Where(
				         x => x.RelativeHitChance > 0 &&
				              x.Alignment.LeftRightOnly() == roll.Item2.LeftRightOnly() &&
				              x.Alignment.FrontRearOnly() == roll.Item2.FrontRearOnly() &&
				              x.Orientation == roll.Item1)
			         .ToList();
		if (!eligableBodyparts.Any())
		{
			if (!alignment.IsCompoundAlignment())
			{
				// Oh well, we tried
#if DEBUG
				Console.WriteLine(
					$"BodyProto {Id:N0} could not find a random bodypart for alignment {alignment.Describe()} and orientation {orientation.Describe()}.");
#endif
				return RandomBodypart;
			}

			if (RandomUtilities.Random(1, 3) != 1)
			{
				eligableBodyparts =
					Bodyparts.Where(
						         x => x.RelativeHitChance > 0 && x.Alignment == roll.Item2.LeftRightOnly() &&
						              x.Orientation == roll.Item1)
					         .ToList();
				if (!eligableBodyparts.Any())
				{
					eligableBodyparts =
						Bodyparts.Where(
							         x => x.RelativeHitChance > 0 && x.Alignment == roll.Item2.FrontRearOnly() &&
							              x.Orientation == roll.Item1)
						         .ToList();
				}
			}
			else
			{
				eligableBodyparts =
					Bodyparts.Where(
						         x => x.RelativeHitChance > 0 && x.Alignment == roll.Item2.FrontRearOnly() &&
						              x.Orientation == roll.Item1)
					         .ToList();
				if (!eligableBodyparts.Any())
				{
					eligableBodyparts =
						Bodyparts.Where(
							         x => x.RelativeHitChance > 0 && x.Alignment == roll.Item2.LeftRightOnly() &&
							              x.Orientation == roll.Item1)
						         .ToList();
				}
			}

			if (!eligableBodyparts.Any())
			{
#if DEBUG
				Console.WriteLine(
					$"BodyProto {Id:N0} could not find a random bodypart for alignment {alignment.Describe()} and orientation {orientation.Describe()}.");
#endif
				return RandomBodypart;
			}
		}

		return eligableBodyparts.GetWeightedRandom(x => x.RelativeHitChance) ??
		       RandomBodypart;
	}

	public IBodypart RandomVitalBodypart(Facing facing)
	{
		var vitals = Bodyparts.Where(x => x.RelativeHitChance > 0 && x.IsVital).ToList();
		if (!vitals.Any())
		{
			vitals = Bodyparts.Where(x => x.Organs.Any()).ToList();
			if (!vitals.Any())
			{
				vitals = Bodyparts.ToList();
			}
		}

		if (facing == Facing.Front)
		{
			if (vitals.Any(x => x.Alignment.FrontRearOnly().In(Alignment.Irrelevant, Alignment.Front)))
			{
				return vitals.Where(x => x.Alignment.FrontRearOnly() == Alignment.Front)
				             .GetWeightedRandom(x => x.RelativeHitChance) ?? vitals.GetRandomElement();
			}
		}

		if (facing == Facing.Rear)
		{
			if (vitals.Any(x => x.Alignment.FrontRearOnly().In(Alignment.Irrelevant, Alignment.Rear)))
			{
				return vitals.Where(x => x.Alignment.FrontRearOnly() == Alignment.Rear)
				             .GetWeightedRandom(x => x.RelativeHitChance) ?? vitals.GetRandomElement();
			}
		}

		return vitals.GetRandomElement();
	}

	public ILimb GetLimbFor(IBodypart bodypart)
	{
		return Prototype.Limbs.FirstOrDefault(x =>
			x.Parts.Contains(bodypart) || x.Parts.Any(y => y.Organs.Contains(bodypart)));
	}

	public IMaterial GetMaterial(IBodypart bodypart)
	{
		// todo - effects that change materials
		return bodypart.DefaultMaterial;
	}

	public IMaterial GetEffectiveMaterial(IBodypart bodypart)
	{
		// TODO - how is this ordered?
		return WornItemsFor(bodypart).Select(x => x.Material).DefaultIfEmpty(GetMaterial(bodypart)).FirstOrDefault();
	}

	public double HitpointsForBodypart(IBodypart part)
	{
		var merits = Merits.Concat(Actor.Merits).Where(x => x.Applies(Actor)).OfType<IBodypartHealthMerit>()
		                   .Where(x => x.AppliesToBodypart(part)).Select(x => x.MultiplierForBodypart(part))
		                   .DefaultIfEmpty(1).Sum();
		if (merits == 0.0)
		{
			return 1.0;
		}

		return Race.ModifiedHitpoints(part) * merits;
	}

	public IEnumerable<IBone> BonesForPart(IBodypart part)
	{
		return part.Bones.Where(x => Bones.Contains(x));
	}

	public IEnumerable<IOrganProto> OrgansForPart(IBodypart part)
	{
		return part.Organs.Where(x => Organs.Contains(x));
	}

	public IReadOnlyDictionary<IOrganProto, BodypartInternalInfo> OrganInfosForPart(IBodypart part)
	{
		return part.OrganInfo.Where(x => Organs.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
	}

	public IReadOnlyDictionary<IBone, BodypartInternalInfo> BoneInfosForPart(IBodypart part)
	{
		return part.BoneInfo.Where(x => Bones.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
	}

	#region Implants

	private bool _implantsChanged = false;

	public bool ImplantsChanged
	{
		get => _implantsChanged;
		set
		{
			if (value && !_implantsChanged)
			{
				Changed = true;
			}

			_implantsChanged = value;
		}
	}

	private readonly List<IImplant> _implants = new();
	public IEnumerable<IImplant> Implants => _implants;

	public void InstallImplant(IImplant implant)
	{
		_implants.Add(implant);
		implant.InstallImplant(this);
		ImplantsChanged = true;
		RecalculateItemHelpers();
	}

	public void RemoveImplant(IImplant implant)
	{
		_implants.Remove(implant);
		implant.RemoveImplant();
		ImplantsChanged = true;
		RecalculateItemHelpers();
	}

	private void SaveImplants(MudSharp.Models.Body body)
	{
		foreach (var item in body.BodiesImplants.Where(x => _implants.All(y => y.Parent.Id != x.ImplantId)).ToList())
		{
			body.BodiesImplants.Remove(item);
		}

		foreach (var dbitem in _implants.Where(x => body.BodiesImplants.All(y => y.ImplantId != x.Parent.Id)).ToList()
		                                .SelectNotNull(item => FMDB.Context.GameItems.Find(item.Parent.Id)))
		{
			body.BodiesImplants.Add(new Models.BodiesImplants { Body = body, ImplantId = dbitem.Id });
		}

		ImplantsChanged = false;
	}

	private void LoadImplants(MudSharp.Models.Body body)
	{
		foreach (var item in body.BodiesImplants)
		{
			var implant = Gameworld.TryGetItem(item.Implant, true)?.GetItemType<IImplant>();
			if (implant != null)
			{
				implant.InstallImplant(this);
				_implants.Add(implant);
			}
		}

		foreach (var item in Implants)
		{
			item.Parent.FinaliseLoadTimeTasks();
		}
	}

	#endregion

	#region Disfigurements

	private readonly List<ITattoo> _tattoos = new();
	public IEnumerable<ITattoo> Tattoos => _tattoos;

	public void AddTattoo(ITattoo tattoo)
	{
		_tattoos.Add(tattoo);
		TattoosChanged = true;
	}

	public void RemoveTattoo(ITattoo tattoo)
	{
		_tattoos.Remove(tattoo);
		TattoosChanged = true;
	}

	private bool _tattoosChanged;

	public bool TattoosChanged
	{
		get => _tattoosChanged;
		set
		{
			if (value && !_tattoosChanged)
			{
				Changed = true;
			}

			_tattoosChanged = value;
		}
	}

	private string SaveTattoos()
	{
		if (Tattoos.Any())
		{
			return new XElement("Tattoos",
				from tattoo in Tattoos
				select tattoo.SaveToXml()
			).ToString();
		}

		return "";
	}

	private void LoadTattoos(MudSharp.Models.Body body)
	{
		if (!string.IsNullOrWhiteSpace(body.Tattoos))
		{
			var root = XElement.Parse(body.Tattoos);
			foreach (var tattoo in root.Elements("Tattoo"))
			{
				_tattoos.Add(new Tattoo(tattoo, Gameworld));
			}
		}
	}

	private readonly List<IScar> _scars = new();
	public IEnumerable<IScar> Scars => _scars;

	public void AddScar(IScar scar)
	{
		_scars.Add(scar);
		ScarsChanged = true;
	}

	public void RemoveScar(IScar scar)
	{
		_scars.Remove(scar);
		ScarsChanged = true;
	}

	private bool _scarsChanged;

	public bool ScarsChanged
	{
		get => _scarsChanged;
		set
		{
			if (value && !_scarsChanged)
			{
				Changed = true;
			}

			_scarsChanged = value;
		}
	}

	private string SaveScars()
	{
		if (Scars.Any())
		{
			return new XElement("Scars",
				from scar in Scars
				select scar.SaveToXml()
			).ToString();
		}

		return "";
	}

	private void LoadScars(MudSharp.Models.Body body)
	{
		// TODO
		/*
		if (!string.IsNullOrWhiteSpace(body.Tattoos))
		{
		    var root = XElement.Parse(body.Tattoos);
		    foreach (var tattoo in root.Elements("Tattoo"))
		    {
		        _tattoos.Add(new Tattoo(tattoo, Gameworld));
		    }
		}
		*/
	}

	#endregion
}
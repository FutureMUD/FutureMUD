#nullable enable

using MudSharp.Body.Disfigurements;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal sealed class BodySwitchPlan
	{
		public required Body Source { get; init; }
		public required Body Target { get; init; }
		public Dictionary<IBodypart, IBodypart> PartMappings { get; } = new();
		public List<(IWound Wound, IBodypart TargetPart, IBodypart? TargetSeveredPart)> Wounds { get; init; } = new();
		public List<(IInfection Infection, IBodypart TargetPart)> PartInfections { get; init; } = new();
		public List<(IScar Scar, IBodypart TargetPart)> Scars { get; init; } = new();
		public List<(ITattoo Tattoo, IBodypart TargetPart)> Tattoos { get; init; } = new();
		public List<IBodypart> SeveredRoots { get; init; } = new();
		public List<(IImplant Implant, IBodypart TargetPart)> Implants { get; init; } = new();
		public List<IProsthetic> Prosthetics { get; init; } = new();
		public List<(IGameItem Item, IWearProfile Profile, bool IsRestraint)> WornItems { get; init; } = new();
		public List<(IGameItem Item, ItemCanWieldFlags Flags)> WieldedItems { get; init; } = new();
		public List<IGameItem> HeldItems { get; init; } = new();
		public List<(AntisepticProtection Effect, IBodypart TargetPart, TimeSpan Duration)> AntisepticEffects { get; init; } = new();
		public List<(AntiInflammatoryTreatment Effect, IBodypart TargetPart, TimeSpan Duration)> AntiInflammatoryEffects { get; init; } = new();
		public List<(InternalBleeding Effect, IBodypart TargetPart, TimeSpan Duration)> InternalBleedingEffects { get; init; } = new();
		public List<(ReplantedBodypartsEffect Effect, IBodypart TargetPart, TimeSpan Duration)> ReplantedEffects { get; init; } = new();
	}

	internal bool TryPrepareSwitchFrom(Body source, out BodySwitchPlan? plan, out string whyNot)
	{
		plan = null;
		if (source.Actor != Actor)
		{
			whyNot = "You can only switch into bodies that belong to the same character.";
			return false;
		}

		if (DirectItems.Any() || _implants.Any() || _prosthetics.Any() || _wounds.Any() || _partInfections.Any() ||
		    _severedRoots.Any() || _tattoos.Any() || _scars.Any())
		{
			whyNot = "That form is not a dormant shell and cannot be used for a phase 1 form switch.";
			return false;
		}

		var computedMappings = new Dictionary<IBodypart, IBodypart?>();

		IBodypart? MapBodypart(IBodypart? sourcePart)
		{
			if (sourcePart is null)
			{
				return null;
			}

			if (computedMappings.TryGetValue(sourcePart, out var existing))
			{
				return existing;
			}

			var targetPart = Prototype.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.CountsAs(sourcePart)) ??
			                 Prototype.AllBodypartsBonesAndOrgans.FirstOrDefault(x => x.Id == sourcePart.Id) ??
			                 Prototype.AllBodypartsBonesAndOrgans.FirstOrDefault(x =>
				                 x.BodypartType == sourcePart.BodypartType &&
				                 x.Alignment == sourcePart.Alignment &&
				                 x.Orientation == sourcePart.Orientation &&
				                 x.Shape.Id == sourcePart.Shape.Id);

			computedMappings[sourcePart] = targetPart;
			return targetPart;
		}

		var switchPlan = new BodySwitchPlan
		{
			Source = source,
			Target = this
		};

		foreach (var severedRoot in source._severedRoots)
		{
			var targetRoot = MapBodypart(severedRoot);
			if (targetRoot is null)
			{
				whyNot = $"Your severed {severedRoot.FullDescription()} has no matching location on that form.";
				return false;
			}

			switchPlan.SeveredRoots.Add(targetRoot);
		}

		foreach (var wound in source._wounds)
		{
			var targetPart = MapBodypart(wound.Bodypart);
			if (targetPart is null)
			{
				whyNot = $"The wound on your {wound.Bodypart?.FullDescription() ?? "body"} has no matching location on that form.";
				return false;
			}

			IBodypart? targetSeveredPart = null;
			if (wound.SeveredBodypart is not null)
			{
				targetSeveredPart = MapBodypart(wound.SeveredBodypart);
				if (targetSeveredPart is null)
				{
					whyNot = $"The severed state for your {wound.SeveredBodypart.FullDescription()} cannot be mapped to that form.";
					return false;
				}
			}

			switchPlan.Wounds.Add((wound, targetPart, targetSeveredPart));
		}

		foreach (var infection in source._partInfections)
		{
			var targetPart = MapBodypart(infection.Bodypart);
			if (targetPart is null)
			{
				whyNot = $"The infection affecting your {infection.Bodypart?.FullDescription() ?? "body"} has no matching location on that form.";
				return false;
			}

			switchPlan.PartInfections.Add((infection, targetPart));
		}

		foreach (var scar in source._scars)
		{
			var targetPart = MapBodypart(scar.Bodypart);
			if (targetPart is null)
			{
				whyNot = $"The scar on your {scar.Bodypart.FullDescription()} has no matching location on that form.";
				return false;
			}

			switchPlan.Scars.Add((scar, targetPart));
		}

		foreach (var tattoo in source._tattoos)
		{
			var targetPart = MapBodypart(tattoo.Bodypart);
			if (targetPart is null)
			{
				whyNot = $"The tattoo on your {tattoo.Bodypart.FullDescription()} has no matching location on that form.";
				return false;
			}

			switchPlan.Tattoos.Add((tattoo, targetPart));
		}

		foreach (var implant in source._implants)
		{
			var targetPart = MapBodypart(implant.TargetBodypart);
			if (targetPart is null)
			{
				whyNot = $"The implant in your {implant.TargetBodypart?.FullDescription() ?? "body"} cannot be mapped to that form.";
				return false;
			}

			if (!(Prototype.CountsAs(implant.TargetBody) || implant.TargetBody.CountsAs(Prototype)))
			{
				whyNot = $"{implant.Parent.HowSeen(Actor, true)} is not compatible with that form.";
				return false;
			}

			switchPlan.Implants.Add((implant, targetPart));
		}

		foreach (var prosthetic in source._prosthetics)
		{
			if (!(Prototype.CountsAs(prosthetic.TargetBody) || prosthetic.TargetBody.CountsAs(Prototype)) ||
			    !Prototype.AllBodypartsBonesAndOrgans.Contains(prosthetic.TargetBodypart))
			{
				whyNot = $"{prosthetic.Parent.HowSeen(Actor, true)} is not compatible with that form.";
				return false;
			}

			switchPlan.Prosthetics.Add(prosthetic);
		}

		foreach (var group in source._wornItems
		                            .GroupBy(x => x.Item)
		                            .Select(x => (Item: x.Key, Profile: x.Key.GetItemType<IWearable>()?.CurrentProfile))
		                            .ToList())
		{
			if (group.Profile is null)
			{
				whyNot = $"{group.Item.HowSeen(Actor, true)} does not have a valid wear profile to transfer.";
				return false;
			}

			var restraint = group.Item.GetItemType<IRestraint>();
			if (restraint?.RestraintType == RestraintType.Binding)
			{
				whyNot = "You cannot switch forms while bound.";
				return false;
			}

			if (!CanWear(group.Item, group.Profile))
			{
				if (restraint is not null)
				{
					whyNot = $"{group.Item.HowSeen(Actor, true)} cannot be transferred to that form while it restrains you.";
					return false;
				}

				continue;
			}

			switchPlan.WornItems.Add((group.Item, group.Profile, restraint is not null));
		}

		foreach (var item in source._wieldedItems.Select(x => x.Item1).Distinct())
		{
			var hands = source._wieldedItems.Count(x => x.Item1 == item);
			switchPlan.WieldedItems.Add((item, hands > 1 ? ItemCanWieldFlags.RequireTwoHands : ItemCanWieldFlags.None));
		}

		foreach (var item in source._heldItems.Select(x => x.Item1).Distinct().Except(switchPlan.WieldedItems.Select(x => x.Item)))
		{
			switchPlan.HeldItems.Add(item);
		}

		foreach (var effect in source.EffectsOfType<AntisepticProtection>().ToList())
		{
			var targetPart = MapBodypart(effect.Bodypart);
			if (targetPart is null)
			{
				whyNot = $"An antiseptic treatment on your {effect.Bodypart.FullDescription()} cannot be mapped to that form.";
				return false;
			}

			switchPlan.AntisepticEffects.Add((effect, targetPart, source.ScheduledDuration(effect)));
		}

		foreach (var effect in source.EffectsOfType<AntiInflammatoryTreatment>().ToList())
		{
			var targetPart = MapBodypart(effect.Bodypart);
			if (targetPart is null)
			{
				whyNot = $"An anti-inflammatory treatment on your {effect.Bodypart.FullDescription()} cannot be mapped to that form.";
				return false;
			}

			switchPlan.AntiInflammatoryEffects.Add((effect, targetPart, source.ScheduledDuration(effect)));
		}

		foreach (var effect in source.EffectsOfType<InternalBleeding>().ToList())
		{
			var targetPart = MapBodypart(effect.Organ);
			if (targetPart is not IOrganProto)
			{
				whyNot = $"Internal bleeding affecting your {effect.Organ.FullDescription()} cannot be mapped to that form.";
				return false;
			}

			switchPlan.InternalBleedingEffects.Add((effect, targetPart, source.ScheduledDuration(effect)));
		}

		foreach (var effect in source.EffectsOfType<ReplantedBodypartsEffect>().ToList())
		{
			var targetPart = MapBodypart(effect.Bodypart);
			if (targetPart is null)
			{
				whyNot = $"Your replanted {effect.Bodypart.FullDescription()} cannot be mapped to that form.";
				return false;
			}

			switchPlan.ReplantedEffects.Add((effect, targetPart, source.ScheduledDuration(effect)));
		}

		foreach (var mapping in computedMappings.Where(x => x.Value is not null))
		{
			switchPlan.PartMappings[mapping.Key] = mapping.Value!;
		}

		plan = switchPlan;
		whyNot = string.Empty;
		return true;
	}

	internal void ApplySwitchPlan(BodySwitchPlan plan)
	{
		var source = plan.Source;

		source.ClearTransferableEffects();
		ClearTransferableEffects();
		source.ClearDirectInventoryState();
		source.ClearImplantsAndProsthetics();
		ClearImplantsAndProsthetics();

		_wounds.Clear();
		_partInfections.Clear();
		_scars.Clear();
		_tattoos.Clear();
		_severedRoots.Clear();

		foreach (var severedRoot in plan.SeveredRoots)
		{
			_severedRoots.Add(severedRoot);
		}

		foreach (var woundPlan in plan.Wounds)
		{
			woundPlan.Wound.RemapTo(Actor, woundPlan.TargetPart, woundPlan.TargetSeveredPart);
			_wounds.Add(woundPlan.Wound);
		}

		foreach (var infectionPlan in plan.PartInfections)
		{
			infectionPlan.Infection.RemapTo(this, null!, infectionPlan.TargetPart);
			_partInfections.Add(infectionPlan.Infection);
		}

		foreach (var scarPlan in plan.Scars)
		{
			var root = scarPlan.Scar.SaveToXml();
			root.Element("Bodypart")!.Value = scarPlan.TargetPart.Id.ToString();
			_scars.Add(new Scar(root, Gameworld, Actor.Race));
		}

		foreach (var tattooPlan in plan.Tattoos)
		{
			var root = tattooPlan.Tattoo.SaveToXml();
			root.Element("Bodypart")!.Value = tattooPlan.TargetPart.Id.ToString();
			_tattoos.Add(new Tattoo(root, Gameworld));
		}

		_activeDrugDosages.Clear();
		_activeDrugDosages.AddRange(source._activeDrugDosages.Select(x => new DrugDosage
		{
			Drug = x.Drug,
			Grams = x.Grams,
			OriginalVector = x.OriginalVector,
			Originator = x.Originator
		}));
		_latentDrugDosages.Clear();
		_latentDrugDosages.AddRange(source._latentDrugDosages.Select(x => new DrugDosage
		{
			Drug = x.Drug,
			Grams = x.Grams,
			OriginalVector = x.OriginalVector,
			Originator = x.Originator
		}));
		DrugsChanged = true;

		Bloodtype = source.Bloodtype;
		TotalBloodVolumeLitres = MudSharp.Character.Character.TotalBloodVolume(Actor);
		BaseLiverAlcoholRemovalKilogramsPerHour = MudSharp.Character.Character.LiverFunction(Actor);
		_currentBloodVolumeLitres = Math.Min(source._currentBloodVolumeLitres, TotalBloodVolumeLitres);
		HeldBreathTime = source.HeldBreathTime;
		MaximumStamina = MudSharp.Character.Character.MaximumStaminaFor(Actor);
		_currentStamina = Math.Min(source.CurrentStamina, MaximumStamina);
		CurrentExertion = source.CurrentExertion;
		LongtermExertion = source.LongtermExertion;
		StaminaChanged = true;
		Changed = true;

		foreach (var implantPlan in plan.Implants)
		{
			implantPlan.Implant.TargetBodypart = implantPlan.TargetPart;
			InstallImplant(implantPlan.Implant);
		}

		foreach (var prosthetic in plan.Prosthetics)
		{
			InstallProsthetic(prosthetic);
		}

		RecalculatePartsAndOrgans();
		RecalculateItemHelpers();

		foreach (var wornPlan in plan.WornItems)
		{
			if (wornPlan.IsRestraint)
			{
				ApplyTransferredRestraint(wornPlan.Item, wornPlan.Profile);
				continue;
			}

			var holdable = wornPlan.Item.GetItemType<IHoldable>();
			if (holdable is not null)
			{
				holdable.HeldBy = null;
			}

			if (!LoadtimeWear(wornPlan.Item, wornPlan.Profile))
			{
				DropTransferredItem(wornPlan.Item);
			}
		}

		foreach (var wieldPlan in plan.WieldedItems)
		{
			wieldPlan.Item.Get(this);
			if (!LoadtimeWield(wieldPlan.Item, wieldPlan.Flags))
			{
				DropTransferredItem(wieldPlan.Item);
			}
		}

		foreach (var heldItem in plan.HeldItems)
		{
			heldItem.Get(this);
			if (!LoadtimeGet(heldItem))
			{
				DropTransferredItem(heldItem);
			}
		}

		ApplyTransferredEffects(plan);
		ReevaluateLimbAndPartDamageEffects();
		CheckDrugTick();
		CheckHealthStatus();
		CheckConsequences();

		source.ResetDormantFormState();
	}

	private void ApplyTransferredRestraint(IGameItem item, IWearProfile profile)
	{
		_wornItems.AddRange(profile.Profile(this).Select(x => (item, x.Key, x.Value)));
		_carriedItems.Add(item);
		item.GetItemType<IWearable>()?.UpdateWear(this, profile);
		UpdateDescriptionWorn(item);
		InventoryChanged = true;
		var restraint = item.GetItemType<IRestraint>();
		var limbs = Limbs.Where(x =>
			restraint?.Limbs.Contains(x.LimbType) == true &&
			profile.AllProfiles.Any(y => x.Parts.Contains(y.Key))).ToList();
		var effect = new RestraintEffect(this, limbs, restraint?.TargetItem, item);
		AddEffect(effect);
		if (restraint is not null)
		{
			restraint.Effect = effect;
		}
	}

	private void ApplyTransferredEffects(BodySwitchPlan plan)
	{
		foreach (var effectPlan in plan.AntisepticEffects)
		{
			var newEffect = new AntisepticProtection(this, effectPlan.TargetPart, effectPlan.Effect.ApplicabilityProg);
			AddTransferredEffect(newEffect, effectPlan.Duration);
		}

		foreach (var effectPlan in plan.AntiInflammatoryEffects)
		{
			var newEffect = new AntiInflammatoryTreatment(this, effectPlan.TargetPart,
				effectPlan.Effect.PainReductionMultiplier, effectPlan.Effect.FlatPainReductionAmount)
			{
				ApplicabilityProg = effectPlan.Effect.ApplicabilityProg
			};
			AddTransferredEffect(newEffect, effectPlan.Duration);
		}

		foreach (var effectPlan in plan.InternalBleedingEffects)
		{
			var newEffect = new InternalBleeding(this, (IOrganProto)effectPlan.TargetPart,
				effectPlan.Effect.BloodlossPerTick)
			{
				ApplicabilityProg = effectPlan.Effect.ApplicabilityProg,
				BloodlossTotal = effectPlan.Effect.BloodlossTotal
			};
			AddTransferredEffect(newEffect, effectPlan.Duration);
		}

		foreach (var effectPlan in plan.ReplantedEffects)
		{
			var newEffect = new ReplantedBodypartsEffect(this, effectPlan.TargetPart,
				effectPlan.Effect.ResistRejectionDifficulty)
			{
				ApplicabilityProg = effectPlan.Effect.ApplicabilityProg,
				ChecksAtCurrentDifficulty = effectPlan.Effect.ChecksAtCurrentDifficulty
			};
			AddTransferredEffect(newEffect, effectPlan.Duration);
		}
	}

	private void AddTransferredEffect(IEffect effect, TimeSpan duration)
	{
		if (duration > TimeSpan.Zero)
		{
			AddEffect(effect, duration);
			return;
		}

		AddEffect(effect);
	}

	private void ClearTransferableEffects()
	{
		foreach (var effect in EffectsOfType<AntisepticProtection>().ToList())
		{
			RemoveEffect(effect, true);
		}

		foreach (var effect in EffectsOfType<AntiInflammatoryTreatment>().ToList())
		{
			RemoveEffect(effect, true);
		}

		foreach (var effect in EffectsOfType<InternalBleeding>().ToList())
		{
			RemoveEffect(effect, true);
		}

		foreach (var effect in EffectsOfType<ReplantedBodypartsEffect>().ToList())
		{
			RemoveEffect(effect, true);
		}

		foreach (var effect in EffectsOfType<RestraintEffect>().ToList())
		{
			RemoveEffect(effect, true);
		}
	}

	private void ClearDirectInventoryState()
	{
		foreach (var item in _heldItems.Select(x => x.Item1).Distinct().ToList())
		{
			var holdable = item.GetItemType<IHoldable>();
			if (holdable is not null)
			{
				holdable.HeldBy = null;
			}
		}

		foreach (var item in _wieldedItems.Select(x => x.Item1).Distinct().ToList())
		{
			var holdable = item.GetItemType<IHoldable>();
			if (holdable is not null)
			{
				holdable.HeldBy = null;
			}

			var wieldable = item.GetItemType<IWieldable>();
			if (wieldable is not null)
			{
				wieldable.PrimaryWieldedLocation = null;
			}
		}

		foreach (var item in _wornItems.Select(x => x.Item).Distinct().ToList())
		{
			var holdable = item.GetItemType<IHoldable>();
			if (holdable is not null)
			{
				holdable.HeldBy = null;
			}

			item.GetItemType<IWearable>()?.UpdateWear(null, null);
		}

		_heldItems.Clear();
		_wieldedItems.Clear();
		_wornItems.Clear();
		_carriedItems.Clear();
		InventoryChanged = true;
		RecalculateItemHelpers();
	}

	private void ClearImplantsAndProsthetics()
	{
		foreach (var implant in _implants.ToList())
		{
			implant.RemoveImplant();
		}

		foreach (var prosthetic in _prosthetics.ToList())
		{
			prosthetic.RemoveProsthetic();
		}

		_implants.Clear();
		_prosthetics.Clear();
		ImplantsChanged = true;
		ProstheticsChanged = true;
		RecalculateItemHelpers();
	}

	private void DropTransferredItem(IGameItem item)
	{
		var holdable = item.GetItemType<IHoldable>();
		if (holdable is not null)
		{
			holdable.HeldBy = null;
		}

		item.GetItemType<IWearable>()?.UpdateWear(null, null);
		var wieldable = item.GetItemType<IWieldable>();
		if (wieldable is not null)
		{
			wieldable.PrimaryWieldedLocation = null;
		}

		item.Drop(Location);
		item.RoomLayer = RoomLayer;
		Location?.Insert(item);
	}

	private void ResetDormantFormState()
	{
		ClearTransferableEffects();
		ClearDirectInventoryState();
		ClearImplantsAndProsthetics();
		_wounds.Clear();
		_partInfections.Clear();
		_scars.Clear();
		_tattoos.Clear();
		_severedRoots.Clear();
		_activeDrugDosages.Clear();
		_latentDrugDosages.Clear();
		DrugsChanged = true;
		RecalculatePartsAndOrgans();
		ReevaluateLimbAndPartDamageEffects();
		TotalBloodVolumeLitres = MudSharp.Character.Character.TotalBloodVolume(Actor);
		BaseLiverAlcoholRemovalKilogramsPerHour = MudSharp.Character.Character.LiverFunction(Actor);
		_currentBloodVolumeLitres = TotalBloodVolumeLitres;
		HeldBreathTime = TimeSpan.Zero;
		MaximumStamina = MudSharp.Character.Character.MaximumStaminaFor(Actor);
		_currentStamina = MaximumStamina;
		CurrentExertion = ExertionLevel.Rest;
		LongtermExertion = ExertionLevel.Rest;
		StaminaChanged = true;
		CheckDrugTick();
		CheckHealthStatus();
		Changed = true;
	}
}

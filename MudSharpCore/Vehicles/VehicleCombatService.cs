#nullable enable

using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Vehicles;

public sealed class VehicleCombatService : IVehicleCombatService
{
	public static VehicleCombatService Instance { get; } = new();

	private VehicleCombatService()
	{
	}

	public IVehicle? VehicleFor(ICharacter character)
	{
		return character?.Gameworld?.Vehicles?.FirstOrDefault(x => x.IsOccupant(character));
	}

	public VehicleRangedCoverDirection ClassifyRangedCoverDirection(IPerceiver attacker, IVehicle vehicle)
	{
		if (attacker is ICharacter character && vehicle.IsOccupant(character))
		{
			return VehicleRangedCoverDirection.SameLevel;
		}

		if (attacker is ICharacter swimmer &&
		    vehicle.IsSurfaceWaterVehicle() &&
		    swimmer.RoomLayer == RoomLayer.GroundLevel &&
		    swimmer.Location?.IsSwimmingLayer(RoomLayer.GroundLevel) == true &&
		    !swimmer.IsSupportedBySurfaceWaterVehicle(out _))
		{
			return VehicleRangedCoverDirection.Below;
		}

		var attackerRank = LayerRank(attacker.RoomLayer);
		var vehicleRank = LayerRank(vehicle.RoomLayer);
		return attackerRank > vehicleRank
			? VehicleRangedCoverDirection.Above
			: attackerRank < vehicleRank
				? VehicleRangedCoverDirection.Below
				: VehicleRangedCoverDirection.SameLevel;
	}

	public VehicleRangedCoverResult? ResolveVehicleRangedCover(IPerceiver attacker, ICharacter target)
	{
		var vehicle = VehicleFor(target);
		if (vehicle is null || vehicle.Destroyed || vehicle.ExteriorItem is not { Deleted: false, Destroyed: false } ||
		    attacker is ICharacter attackerCharacter && vehicle.IsOccupant(attackerCharacter))
		{
			return null;
		}

		var occupancy = vehicle.Occupancies.FirstOrDefault(x => x.Occupant?.SamePhysicalInstance(target) == true);
		if (occupancy?.Slot is null)
		{
			return null;
		}

		var direction = ClassifyRangedCoverDirection(attacker, vehicle);
		var cover = direction switch
		{
			VehicleRangedCoverDirection.Above => occupancy.Slot.AboveRangedCover,
			VehicleRangedCoverDirection.Below => occupancy.Slot.BelowRangedCover,
			_ => occupancy.Slot.SameLevelRangedCover
		};
		return cover is null
			? null
			: new VehicleRangedCoverResult(cover, vehicle.ExteriorItem, true, vehicle, occupancy.Slot, direction);
	}

	public VehicleRangedCoverResult? ResolveEffectiveRangedCover(IPerceiver attacker, ICharacter target)
	{
		var vehicleCover = ResolveVehicleRangedCover(attacker, target);
		var personalCover = target.Cover?.Cover is { } cover
			? new VehicleRangedCoverResult(cover, target.Cover.CoverItem?.Parent, false, null, null,
				VehicleRangedCoverDirection.SameLevel)
			: null;

		if (vehicleCover is null)
		{
			return personalCover;
		}

		if (personalCover is null)
		{
			return vehicleCover;
		}

		if (vehicleCover.Cover.MinimumRangedDifficulty != personalCover.Cover.MinimumRangedDifficulty)
		{
			return vehicleCover.Cover.MinimumRangedDifficulty > personalCover.Cover.MinimumRangedDifficulty
				? vehicleCover
				: personalCover;
		}

		if (vehicleCover.Cover.CoverType != personalCover.Cover.CoverType)
		{
			return vehicleCover.Cover.CoverType == CoverType.Hard ? vehicleCover : personalCover;
		}

		return personalCover;
	}

	public bool CanCrossVehicleBoundary(ICharacter attacker, ICharacter target, bool rangedAttack,
		bool aquaticVehicleAttack, out string reason)
	{
		var targetVehicle = VehicleFor(target);
		if (targetVehicle is null || !targetVehicle.IsSurfaceWaterVehicle() || targetVehicle.Destroyed ||
		    targetVehicle.IsOccupant(attacker) || rangedAttack || aquaticVehicleAttack)
		{
			reason = string.Empty;
			return true;
		}

		var attackerUnsupportedInWater = attacker.Location == targetVehicle.Location &&
		                                 attacker.RoomLayer == RoomLayer.GroundLevel &&
		                                 attacker.Location?.IsSwimmingLayer(attacker.RoomLayer) == true &&
		                                 !attacker.IsSupportedBySurfaceWaterVehicle(out _);
		if (!attackerUnsupportedInWater)
		{
			reason = string.Empty;
			return true;
		}

		reason = $"You must board {targetVehicle.ExteriorItem?.HowSeen(attacker) ?? targetVehicle.Name} before using a contact attack against its occupants.";
		return false;
	}

	public VehicleOverboardResult ResolveDisplacement(ICharacter target,
		VehicleCombatDisplacementType displacementType, int successDegrees = 1, int additionalDifficultyStages = 0)
	{
		var vehicle = VehicleFor(target);
		if (vehicle is null || !vehicle.IsSurfaceWaterVehicle() || vehicle.Destroyed ||
		    vehicle.ExteriorItem is not { Deleted: false, Destroyed: false })
		{
			return new VehicleOverboardResult(false, false, Outcome.NotTested, Difficulty.Automatic, null, null);
		}

		var occupancy = vehicle.Occupancies.FirstOrDefault(x => x.Occupant?.SamePhysicalInstance(target) == true);
		if (occupancy?.Slot is null)
		{
			return new VehicleOverboardResult(false, false, Outcome.NotTested, Difficulty.Automatic, vehicle, null);
		}

		var difficulty = occupancy.Slot.BoatStabilityDifficulty
		                               .StageUp(Math.Max(0, successDegrees - 1))
		                               .StageUp(additionalDifficultyStages);
		var check = target.Gameworld.GetCheck(CheckType.BoatStabilityCheck);
		if (!IsConfiguredStabilityCheck(check))
		{
			check = target.Gameworld.GetCheck(CheckType.GenericSkillCheck);
			var fallbackTrait = target.Gameworld.Traits.FirstOrDefault(x =>
				x.Name.EqualToAny("Dodge", "Dodging", "Athletics"));
			var fallbackOutcome = check.Check(target, difficulty, fallbackTrait, vehicle.ExteriorItem).Outcome;
			return ApplyStabilityOutcome(target, vehicle, occupancy, difficulty, fallbackOutcome);
		}

		var outcome = check.Check(target, difficulty, vehicle.ExteriorItem).Outcome;
		return ApplyStabilityOutcome(target, vehicle, occupancy, difficulty, outcome);
	}

	private static VehicleOverboardResult ApplyStabilityOutcome(ICharacter target, IVehicle vehicle,
		IVehicleOccupancy occupancy, Difficulty difficulty, Outcome outcome)
	{
		if (outcome.IsPass())
		{
			return new VehicleOverboardResult(true, false, outcome, difficulty, vehicle, occupancy.Slot);
		}

		var closeContacts = target.EffectsOfType<ClinchEffect>()
		                          .Select(x => x.Target)
		                          .Concat(target.EffectsOfType<IGrappling>().Select(x => x.Target))
		                          .Concat(target.CombinedEffectsOfType<IBeingGrappled>()
		                                        .Select(x => x.Grappling.CharacterOwner))
		                          .Where(x => x is not null && !x.SamePhysicalInstance(target))
		                          .Distinct()
		                          .ToList();
		foreach (var other in closeContacts)
		{
			CombatForcedMovementUtilities.BreakCloseContact(other, target);
		}

		vehicle.ForceDisembark(target);
		target.MeleeRange = false;
		target.RemoveAllEffects(x => x.GetSubtype<ClinchEffect>() is not null ||
		                             x.GetSubtype<IGrappling>() is not null ||
		                             x.GetSubtype<IBeingGrappled>() is not null, true);
		target.Body.RemoveAllEffects(x => x.GetSubtype<IBeingGrappled>() is not null, true);
		return new VehicleOverboardResult(true, true, outcome, difficulty, vehicle, occupancy.Slot);
	}

	private static bool IsConfiguredStabilityCheck(ICheck check)
	{
		if (check.Type != CheckType.BoatStabilityCheck || check.TargetNumberExpression is not { } expression)
		{
			return false;
		}

		return expression.Parameters.Any() ||
		       expression.NonTraitParameters.Any(x => x.EqualTo("variable"));
	}

	private static int LayerRank(RoomLayer layer)
	{
		return layer switch
		{
			RoomLayer.VeryDeepUnderwater => -3,
			RoomLayer.DeepUnderwater => -2,
			RoomLayer.Underwater => -1,
			RoomLayer.GroundLevel => 0,
			RoomLayer.InTrees or RoomLayer.OnRooftops => 1,
			RoomLayer.HighInTrees or RoomLayer.InAir => 2,
			RoomLayer.HighInAir => 3,
			_ => 0
		};
	}
}

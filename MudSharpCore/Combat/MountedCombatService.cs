#nullable enable

using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat;

public sealed class MountedCombatService : IMountedCombatService
{
	private const double MinimumMomentum = 0.25;
	private const double MaximumMomentum = 12.0;

	public static MountedCombatService Instance { get; } = new();

	private MountedCombatService()
	{
	}

	public MountedCombatContext? ResolveContext(ICharacter combatant)
	{
		if (combatant.RidingMount is { } mount && mount.IsPrimaryRider(combatant))
		{
			var domain = ResolveMountDomain(mount);
			var moveTime = mount.MoveSpeed(null!);
			var mountMomentum = double.IsFinite(moveTime) && moveTime > 0.0
				? Math.Clamp(1.4 / moveTime, MinimumMomentum, MaximumMomentum)
				: MinimumMomentum;
			return new MountedCombatContext(
				combatant,
				mount,
				domain,
				mount.CurrentContextualSize(SizeContext.BeingRiddenAsMount),
				mountMomentum,
				mount);
		}

		var vehicle = VehicleCombatService.Instance.VehicleFor(combatant);
		if (vehicle is null || vehicle.Destroyed || vehicle.Disabled ||
		    !vehicle.IsOccupant(combatant) || vehicle.Controller?.SamePhysicalInstance(combatant) != true ||
		    vehicle.ExteriorItem is not { Deleted: false, Destroyed: false } exterior)
		{
			return null;
		}

		var profile = vehicle.MovementProfile;
		var propulsion = vehicle.ActivePropulsionProfile;
		var vehicleMomentum = profile?.RouteSpeedMetresPerSecond > 0.0
			? profile.RouteSpeedMetresPerSecond
			: propulsion?.BaseMoveTimeMilliseconds > 0.0
				? 1000.0 / propulsion.BaseMoveTimeMilliseconds
				: MinimumMomentum;
		return new MountedCombatContext(
			combatant,
			exterior,
			vehicle.IsSurfaceWaterVehicle()
				? MountedCombatDomain.AquaticVehicle
				: MountedCombatDomain.GroundVehicle,
			exterior.Size,
			Math.Clamp(vehicleMomentum, MinimumMomentum, MaximumMomentum),
			Vehicle: vehicle);
	}

	public BuiltInCombatMoveType ChargeMessageType(MountedCombatContext context)
	{
		return context.Domain switch
		{
			MountedCombatDomain.Aerial => BuiltInCombatMoveType.AerialMountedCharge,
			MountedCombatDomain.Aquatic => BuiltInCombatMoveType.AquaticMountedCharge,
			MountedCombatDomain.GroundVehicle => BuiltInCombatMoveType.VehicleCharge,
			MountedCombatDomain.AquaticVehicle => BuiltInCombatMoveType.AquaticVehicleCharge,
			_ => BuiltInCombatMoveType.MountedCharge
		};
	}

	public CheckType ChargeCheckType(MountedCombatContext context)
	{
		return context.Domain switch
		{
			MountedCombatDomain.Aerial => CheckType.AerialMountedChargeCheck,
			MountedCombatDomain.Aquatic => CheckType.AquaticMountedChargeCheck,
			MountedCombatDomain.GroundVehicle => CheckType.VehicleChargeCheck,
			MountedCombatDomain.AquaticVehicle => CheckType.AquaticVehicleChargeCheck,
			_ => CheckType.MountedChargeCheck
		};
	}

	public void ResolveMountSprawl(ICharacter mount, int knockdownSuccessDegrees)
	{
		foreach (var rider in mount.Riders.ToList())
		{
			ResolveRiderMountSprawl(mount, rider, knockdownSuccessDegrees);
		}
	}

	private static void ResolveRiderMountSprawl(ICharacter mount, ICharacter rider, int knockdownSuccessDegrees)
	{
		var sizeDifference = (int)mount.CurrentContextualSize(SizeContext.BeingRiddenAsMount) -
		                     (int)rider.CurrentContextualSize(SizeContext.RidingMount);
		var weightRatio = mount.Weight / Math.Max(1.0, rider.Weight);
		var difficulty = MountSprawlDifficulty(sizeDifference, weightRatio, knockdownSuccessDegrees);
		var profile = MountGearService.ProfileFor(mount, rider);
		var check = rider.Gameworld.GetCheck(CheckType.AvoidMountFallCheck);
		CheckOutcome result;
		if (check.Type != CheckType.AvoidMountFallCheck)
		{
			check = rider.Gameworld.GetCheck(CheckType.GenericSkillCheck);
			var fallbackTrait = rider.Gameworld.Traits.FirstOrDefault(x =>
				x.Name.EqualToAny("Riding", "Ride", "Balancing", "Balance", "Athletics"));
			result = check.Check(rider, difficulty, fallbackTrait, mount, profile.StabilityBonus);
		}
		else
		{
			result = check.Check(rider, difficulty, mount, externalBonus: profile.StabilityBonus);
		}

		var outcome = result.Outcome;
		mount.RemoveRider(rider);

		switch (outcome)
		{
			case Outcome.MajorPass:
			case Outcome.Pass:
				rider.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ throw|throws &0 clear as $1 sprawls, landing safely on &0's feet.", rider, rider,
						mount)));
				return;
			case Outcome.MinorPass:
				rider.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ tumble|tumbles clear as $1 sprawls, landing hard beside &1.", rider, rider, mount)));
				rider.DoCombatKnockdown();
				rider.DoFallDamage(0.25);
				return;
			case Outcome.MinorFail:
				rider.OutputHandler.Handle(new EmoteOutput(
					new Emote("@ fall|falls heavily from $1 as &1 sprawls.", rider, rider, mount)));
				rider.DoCombatKnockdown();
				rider.DoFallDamage(0.5);
				return;
			default:
				rider.OutputHandler.Handle(new EmoteOutput(
					new Emote("$1 topple|topples onto $0 as $1 sprawls, crushing $0 beneath $1!", rider, rider,
						mount)));
				rider.DoCombatKnockdown();
				ApplyMountCrushDamage(mount, rider,
					MountCrushDamage(sizeDifference, weightRatio, knockdownSuccessDegrees, outcome));
				return;
		}
	}

	public static Difficulty MountSprawlDifficulty(int sizeDifference, double weightRatio,
		int knockdownSuccessDegrees)
	{
		var stages = Math.Max(0, knockdownSuccessDegrees - 1) + Math.Max(0, sizeDifference - 1);
		if (weightRatio >= 4.0)
		{
			stages++;
		}

		if (weightRatio >= 10.0)
		{
			stages++;
		}

		return Difficulty.Normal.StageUp(Math.Min(4, stages));
	}

	public static double MountCrushDamage(int sizeDifference, double weightRatio, int knockdownSuccessDegrees,
		Outcome outcome)
	{
		var safeWeightRatio = Math.Max(1.0, weightRatio);
		var damage = 3.0 + 1.5 * Math.Max(1, knockdownSuccessDegrees) + 2.0 * Math.Max(0, sizeDifference) +
		             1.5 * Math.Log2(safeWeightRatio);
		if (outcome == Outcome.MajorFail)
		{
			damage *= 1.5;
		}

		return Math.Clamp(damage, 1.0, 50.0);
	}

	private static void ApplyMountCrushDamage(ICharacter mount, ICharacter rider, double amount)
	{
		var wounds = rider.PassiveSufferDamage(new Damage
		{
			ActorOrigin = mount,
			Bodypart = rider.Body.RandomBodypart,
			DamageAmount = amount,
			DamageType = DamageType.Crushing,
			PainAmount = amount * 1.2,
			ShockAmount = amount * 0.25,
			StunAmount = amount * 0.5,
			PenetrationOutcome = Outcome.NotTested
		});
		wounds.ProcessPassiveWounds();
	}

	private static MountedCombatDomain ResolveMountDomain(ICharacter mount)
	{
		if (mount.PositionState == PositionFlying.Instance)
		{
			return MountedCombatDomain.Aerial;
		}

		if (mount.PositionState.In(PositionSwimming.Instance, PositionFloatingInWater.Instance) ||
		    mount.Location?.IsSwimmingLayer(mount.RoomLayer) == true)
		{
			return MountedCombatDomain.Aquatic;
		}

		return MountedCombatDomain.Ground;
	}
}

#nullable enable

using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.GameItems;
using MudSharp.Framework.Scheduling;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp.Combat.Moves;

public sealed class MountedImpactNaturalAttackMove : NaturalAttackMove
{
	private readonly bool _isChargeImpact;
	private readonly SizeContext _sizeContext;
	private readonly bool _applyImpactAdvantage;

	public MountedImpactNaturalAttackMove(ICharacter owner, INaturalAttack attack, ICharacter target,
		bool isChargeImpact = false, SizeContext sizeContext = SizeContext.BeingRiddenAsMount,
		bool applyImpactAdvantage = false)
		: base(owner, attack, target)
	{
		Target = target;
		_isChargeImpact = isChargeImpact;
		_sizeContext = sizeContext;
		_applyImpactAdvantage = applyImpactAdvantage;
	}

	public ICharacter Target { get; }

	public override string Description => Attack.MoveType switch
	{
		BuiltInCombatMoveType.AerialSweepAttack => "sweeping through a target during an aerial charge",
		BuiltInCombatMoveType.AquaticChargeAttack => "driving through a target during an aquatic charge",
		BuiltInCombatMoveType.BehemothChargeAttack => "crashing bodily into a smaller target during a charge",
		_ => "trampling a smaller target during a mounted charge"
	};

	public override CombatMoveResult ResolveMove(ICombatMove defenderMove)
	{
		if (!_isChargeImpact)
		{
			return CombatMoveResult.Irrelevant;
		}

		var result = base.ResolveMove(defenderMove);
		if (!result.MoveWasSuccessful || !result.AttackerOutcome.IsPass() || Target.State.HasFlag(CharacterState.Dead))
		{
			return result;
		}

		var sizeDifference = (int)Assailant.CurrentContextualSize(_sizeContext) -
		                     (int)Target.CurrentContextualSize(SizeContext.GrappleDefense);
		var degrees = Math.Max(1, result.AttackerOutcome.SuccessDegrees());
		if (_applyImpactAdvantage)
		{
			Assailant.OffensiveAdvantage += 2.0 + degrees + Math.Max(0, sizeDifference - 1);
			Target.DefensiveAdvantage -= 1.0 + degrees;
		}
		var reelMilliseconds = Gameworld.GetStaticDouble(degrees >= 3
			? "StaggeringBlowReelTimeFailure"
			: "StaggeringBlowReelTimeMinorFailure");
		Gameworld.Scheduler.DelayScheduleType(Target, ScheduleType.Combat,
			TimeSpan.FromMilliseconds(reelMilliseconds));
		if (ShouldKnockDown(sizeDifference, degrees, Target.RidingMount is not null,
			    VehicleCombatService.Instance.VehicleFor(Target) is not null))
		{
			Target.OutputHandler.Handle(new EmoteOutput(new Emote(
				"@ are|is hurled from &0's footing by the force of the impact!", Target),
				style: OutputStyle.CombatMessage, flags: OutputFlags.InnerWrap));
			Target.DoCombatKnockdown(Math.Max(1, degrees - Math.Max(0, 2 - sizeDifference)));
		}

		return result;
	}

	internal static bool ShouldKnockDown(int sizeDifference, int successDegrees, bool targetMounted,
		bool targetInVehicle)
	{
		return sizeDifference >= 2 || successDegrees >= 3 || targetMounted || targetInVehicle;
	}
}

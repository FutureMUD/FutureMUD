#nullable enable

using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Accounts;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System;
using System.Linq;

namespace MudSharp.Arenas;

internal static class ArenaScoringHelper
{
	internal static void TryRecordCombatResolution(ICharacter attacker, ICombatMove move, ICombatMove? response,
		CombatMoveResult result)
	{
		ICharacter? defender = move.CharacterTargets.FirstOrDefault();
		if (defender is null || !HasScorableResolution(result))
		{
			return;
		}

		ArenaEvent? arenaEvent = ResolveSharedLiveArenaEvent(attacker, defender);
		if (arenaEvent is null ||
		    !arenaEvent.TryGetParticipantSideIndex(attacker, out int attackerSideIndex) ||
		    !arenaEvent.TryGetParticipantSideIndex(defender, out int defenderSideIndex))
		{
			return;
		}

		IBodypart? impactedBodypart = ResolveImpactedBodypart(move, result);
		arenaEvent.RecordScoringCandidate(new ArenaScoringSnapshot(
			attacker,
			defender,
			attackerSideIndex,
			defenderSideIndex,
			IsLandedHit(result, response) ? 1 : 0,
			IsUndefendedHit(result, response) ? 1 : 0,
			DescribeImpactLocation(impactedBodypart),
			DescribeImpactBodypart(impactedBodypart)
		));
	}

	internal static string DescribeImpactLocation(IBodypart? bodypart)
	{
		if (bodypart is null)
		{
			return "unknown";
		}

		switch (bodypart.BodypartType)
		{
			case BodypartTypeEnum.Brain:
			case BodypartTypeEnum.Eye:
			case BodypartTypeEnum.Ear:
			case BodypartTypeEnum.Esophagus:
			case BodypartTypeEnum.Tongue:
			case BodypartTypeEnum.Mouth:
			case BodypartTypeEnum.Trachea:
				return "head";
			case BodypartTypeEnum.Heart:
			case BodypartTypeEnum.Liver:
			case BodypartTypeEnum.Spleen:
			case BodypartTypeEnum.Intestines:
			case BodypartTypeEnum.Stomach:
			case BodypartTypeEnum.Lung:
			case BodypartTypeEnum.Kidney:
			case BodypartTypeEnum.Spine:
				return "torso";
			case BodypartTypeEnum.Wing:
			case BodypartTypeEnum.Fin:
				return "appendage";
		}

		string description = DescribeImpactBodypart(bodypart);
		if (ContainsKeyword(description, "head", "skull", "face", "jaw", "neck", "throat", "snout", "muzzle", "beak"))
		{
			return "head";
		}

		if (ContainsKeyword(description, "torso", "chest", "abdomen", "belly", "stomach", "back", "flank", "rib", "pelvis", "hip", "groin"))
		{
			return "torso";
		}

		if (ContainsKeyword(description, "arm", "hand", "paw", "claw"))
		{
			return "upper-limb";
		}

		if (ContainsKeyword(description, "leg", "foot", "hoof"))
		{
			return "lower-limb";
		}

		if (ContainsKeyword(description, "tail", "wing", "fin", "tentacle", "pincer", "mandible"))
		{
			return "appendage";
		}

		return "other";
	}

	internal static string DescribeImpactBodypart(IBodypart? bodypart)
	{
		return bodypart?.FullDescription(false, PermissionLevel.Any).ToLowerInvariant() ?? string.Empty;
	}

	private static ArenaEvent? ResolveSharedLiveArenaEvent(ICharacter attacker, ICharacter defender)
	{
		ArenaParticipationEffect? attackerEffect = attacker.CombinedEffectsOfType<ArenaParticipationEffect>()
			.FirstOrDefault();
		if (attackerEffect?.ArenaEvent is not ArenaEvent arenaEvent || arenaEvent.State != ArenaEventState.Live)
		{
			return null;
		}

		bool defenderInSameEvent = defender.CombinedEffectsOfType<ArenaParticipationEffect>()
			.Any(x => x.ArenaEventId == attackerEffect.ArenaEventId);
		return defenderInSameEvent ? arenaEvent : null;
	}

	private static bool HasScorableResolution(CombatMoveResult result)
	{
		return result.MoveWasSuccessful ||
		       result.AttackerOutcome != Outcome.NotTested ||
		       result.DefenderOutcome != Outcome.NotTested ||
		       result.WoundsCaused.Any() ||
		       result.SelfWoundsCaused.Any();
	}

	private static bool IsLandedHit(CombatMoveResult result, ICombatMove? response)
	{
		return result.WoundsCaused.Any() ||
		       (result.MoveWasSuccessful && IsUndefendedHit(result, response));
	}

	private static bool IsUndefendedHit(CombatMoveResult result, ICombatMove? response)
	{
		return response is null || result.DefenderOutcome == Outcome.NotTested;
	}

	private static IBodypart? ResolveImpactedBodypart(ICombatMove move, CombatMoveResult result)
	{
		IBodypart? woundedBodypart = result.WoundsCaused
			.Select(x => x.Bodypart)
			.FirstOrDefault(x => x is not null);
		if (woundedBodypart is not null)
		{
			return woundedBodypart;
		}

		if (move is IWeaponAttackMove weaponAttackMove)
		{
			return weaponAttackMove.TargetBodypart;
		}

		return move.GetType()
			.GetProperty("TargetBodypart")
			?.GetValue(move) as IBodypart;
	}

	private static bool ContainsKeyword(string text, params string[] keywords)
	{
		return keywords.Any(text.Contains);
	}
}

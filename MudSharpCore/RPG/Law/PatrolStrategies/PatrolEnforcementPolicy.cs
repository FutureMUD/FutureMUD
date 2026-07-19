#nullable enable

namespace MudSharp.RPG.Law.PatrolStrategies;

internal enum PatrolCandidateAction
{
	None,
	ResumeCustody,
	IssueWarning
}

internal static class PatrolEnforcementPolicy
{
	public static bool CanConsiderCandidate(bool isSelfOrPatrolMember, bool isDeadOrInStasis, bool canSee,
		bool isInCustody, bool wasWarned, bool isDraggedByAnother, bool isOnTrial)
	{
		return CanConsiderCandidate(
			isSelfOrPatrolMember,
			isDeadOrInStasis,
			() => canSee,
			() => isInCustody,
			() => wasWarned,
			() => isDraggedByAnother,
			() => isOnTrial);
	}

	public static bool CanConsiderCandidate(bool isSelfOrPatrolMember, bool isDeadOrInStasis, Func<bool> canSee,
		Func<bool> isInCustody, Func<bool> wasWarned, Func<bool> isDraggedByAnother, Func<bool> isOnTrial)
	{
		if (isSelfOrPatrolMember || isDeadOrInStasis || !canSee())
		{
			return false;
		}

		return !isInCustody() &&
		       !wasWarned() &&
		       !isDraggedByAnother() &&
		       !isOnTrial();
	}

	public static PatrolCandidateAction SelectCandidateAction(bool isBeingDraggedByPatrol, bool hasEligibleCrime)
	{
		if (!hasEligibleCrime)
		{
			return PatrolCandidateAction.None;
		}

		return isBeingDraggedByPatrol
			? PatrolCandidateAction.ResumeCustody
			: PatrolCandidateAction.IssueWarning;
	}

	public static bool ShouldBeginCustody(bool crimeIsArrestable, bool criminalIsHelpless)
	{
		return crimeIsArrestable && criminalIsHelpless;
	}

	public static bool ShouldWaitForSurrender(bool criminalWasWarned)
	{
		return criminalWasWarned;
	}
}

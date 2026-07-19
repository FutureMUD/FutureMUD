#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Law.PatrolStrategies;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PatrolEnforcementPolicyTests
{
	[DataTestMethod]
	[DataRow(false, false, true, false, false, false, false, true)]
	[DataRow(false, true, true, false, false, false, false, false)]
	[DataRow(false, false, false, false, false, false, false, false)]
	[DataRow(false, false, true, true, false, false, false, false)]
	[DataRow(false, false, true, false, true, false, false, false)]
	[DataRow(false, false, true, false, false, true, false, false)]
	[DataRow(false, false, true, false, false, false, true, false)]
	public void CanConsiderCandidate_EnforcesCustodyVisibilityAndLifecycleRules(bool isSelfOrMember,
		bool unavailable, bool visible, bool inCustody, bool warned, bool draggedByAnother, bool onTrial,
		bool expected)
	{
		Assert.AreEqual(expected, PatrolEnforcementPolicy.CanConsiderCandidate(
			isSelfOrMember, unavailable, visible, inCustody, warned, draggedByAnother, onTrial));
	}

	[TestMethod]
	public void CanConsiderCandidate_SelfOrPatrolMember_DoesNotEvaluatePerceptionOrEffects()
	{
		var result = PatrolEnforcementPolicy.CanConsiderCandidate(
			true,
			false,
			() => throw new AssertFailedException("Visibility should not be evaluated."),
			() => throw new AssertFailedException("Custody should not be evaluated."),
			() => throw new AssertFailedException("Warnings should not be evaluated."),
			() => throw new AssertFailedException("Dragging should not be evaluated."),
			() => throw new AssertFailedException("Trial state should not be evaluated."));

		Assert.IsFalse(result);
	}
	[DataTestMethod]
	[DataRow(false, false, 0)]
	[DataRow(true, false, 0)]
	[DataRow(false, true, 2)]
	[DataRow(true, true, 1)]
	public void SelectCandidateAction_ResumesExistingCustodyOrWarns(bool beingDragged, bool hasCrime,
		int expected)
	{
		Assert.AreEqual(expected, (int)PatrolEnforcementPolicy.SelectCandidateAction(beingDragged, hasCrime));
	}

	[DataTestMethod]
	[DataRow(false, false, false)]
	[DataRow(false, true, false)]
	[DataRow(true, false, false)]
	[DataRow(true, true, true)]
	public void ShouldBeginCustody_RequiresArrestableCrimeAndHelplessTarget(bool arrestable, bool helpless,
		bool expected)
	{
		Assert.AreEqual(expected, PatrolEnforcementPolicy.ShouldBeginCustody(arrestable, helpless));
	}

	[DataTestMethod]
	[DataRow(false, false)]
	[DataRow(true, true)]
	public void ShouldWaitForSurrender_TracksWarningState(bool warned, bool expected)
	{
		Assert.AreEqual(expected, PatrolEnforcementPolicy.ShouldWaitForSurrender(warned));
	}
}

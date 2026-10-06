#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests;

public partial class QueuedCommandAuthorityTests
{
	private sealed class CheckCostFrameMove(Func<CombatMoveResult> resolve) : CombatMoveBase
	{
		public override string Description => "Check callback cost checkpoint";
		public override double StaminaCost => 7;
		public override CombatMoveResult ResolveMove(ICombatMove defenderMove) => resolve();
	}

	[DataTestMethod]
	[DataRow("valid", true)]
	[DataRow("initial", false)]
	[DataRow("inside", false)]
	[DataRow("committed", true)]
	[DataRow("independent", false)]
	[DataRow("direct", true)]
	public void CheckCallbackRejection_OuterMoveStillActive_CostAlreadyReflectsCommittedWork(string change, bool costs)
	{
		var f = new Fixture();
		var calls = 0; var effects = 0;
		var nativeResult = new CombatMoveResult { MoveWasSuccessful = change != "committed" };
		var move = new CheckCostFrameMove(() =>
		{
			++calls;
			if (change == "committed") { ++effects; CommandExecutionScope.MarkCommitted(f.Actor.Object); }
			if (change == "independent")
			{
				using var independent = CommandExecutionScope.EnterIndependent();
				++effects; CommandExecutionScope.MarkCommitted();
			}
			if (change != "valid") f.Grant = null;
			CommandExecutionScope.TryContinue();
			return nativeResult;
		}) { Assailant = f.Actor.Object };
		if (change != "direct") CommandExecutionAuthority.Prepare(f.Actor.Object, f.Commander.Object, "selected attack", () => true).Bind(move);
		using var outer = CommandExecutionScope.EnterMove(move);
		if (change == "initial") f.Grant = null;
		var result = CommandExecutionScope.Resolve(move, null);
		Assert.AreSame(costs ? nativeResult : CombatMoveResult.Irrelevant, result);
		// CombatBase queries cost here, before the enclosing scope's Dispose.
		Assert.AreEqual(costs, move.UsesStaminaWithResult(result), "An active outer scope must not defer rejection until after charging stamina.");
		Assert.AreEqual(costs, CommandExecutionScope.HasCommitted);
		Assert.AreEqual(change == "initial" ? 0 : 1, calls);
		Assert.AreEqual(change is "committed" or "independent" ? 1 : 0, effects);
	}
}

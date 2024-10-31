using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CheckTests
{
	[TestMethod]
	public void TestOpposedOutcomeNoDraw()
	{
		var outcomes1 = new Dictionary<Difficulty, CheckOutcome> {
			{ Difficulty.Automatic, new CheckOutcome {Outcome = Outcome.MajorPass} },
			{ Difficulty.Trivial, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.ExtremelyEasy, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.VeryEasy, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.Easy, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.Normal, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.Hard, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.VeryHard, new CheckOutcome {Outcome = Outcome.Pass}},
			{ Difficulty.ExtremelyHard, new CheckOutcome {Outcome = Outcome.Pass}},
			{ Difficulty.Insane, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.Impossible, new CheckOutcome {Outcome = Outcome.MajorFail}},
		};

		var outcomes2 = new Dictionary<Difficulty, CheckOutcome> {
			{ Difficulty.Automatic, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.Trivial, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.ExtremelyEasy, new CheckOutcome {Outcome = Outcome.Pass}},
			{ Difficulty.VeryEasy, new CheckOutcome {Outcome = Outcome.Pass}},
			{ Difficulty.Easy, new CheckOutcome {Outcome = Outcome.Pass}},
			{ Difficulty.Normal, new CheckOutcome {Outcome = Outcome.Pass}},
			{ Difficulty.Hard, new CheckOutcome {Outcome = Outcome.MinorPass}},
			{ Difficulty.VeryHard, new CheckOutcome {Outcome = Outcome.Fail}},
			{ Difficulty.ExtremelyHard, new CheckOutcome {Outcome = Outcome.Fail}},
			{ Difficulty.Insane, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.Impossible, new CheckOutcome {Outcome = Outcome.MajorFail}},
		};

		var outcomes3 = new Dictionary<Difficulty, CheckOutcome> {
			{ Difficulty.Automatic, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.Trivial, new CheckOutcome {Outcome = Outcome.MajorPass}},
			{ Difficulty.ExtremelyEasy, new CheckOutcome {Outcome = Outcome.Pass}},
			{ Difficulty.VeryEasy, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.Easy, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.Normal, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.Hard, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.VeryHard, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.ExtremelyHard, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.Insane, new CheckOutcome {Outcome = Outcome.MajorFail}},
			{ Difficulty.Impossible, new CheckOutcome {Outcome = Outcome.MajorFail}},
		};

		var outcome = new OpposedOutcome(outcomes1, outcomes2, Difficulty.Normal, Difficulty.Normal);
		Assert.AreEqual(outcome.Outcome, OpposedOutcomeDirection.Proponent, "Expected Major Pass @ Normal vs Pass @ Normal to be proponent.");

		outcome = new OpposedOutcome(outcomes1, outcomes2, Difficulty.Automatic, Difficulty.Automatic);
		Assert.AreEqual(outcome.Outcome, OpposedOutcomeDirection.Proponent, "Expected Major Pass @ Extremely Easy vs Pass @ Extremely Easy to be proponent.");
		Assert.AreEqual(outcome.Degree, OpposedOutcomeDegree.Marginal, "Expected Major Pass @ Extremely Easy vs Pass @ Extremely Easy to be marginal.");

		outcome = new OpposedOutcome(outcomes2, outcomes3, Difficulty.Impossible, Difficulty.Impossible);
		Assert.AreEqual(outcome.Outcome, OpposedOutcomeDirection.Proponent, "Expected Major Pass @ Extremely Easy vs Pass @ Extremely Easy to be proponent.");
		Assert.AreEqual(outcome.Degree, OpposedOutcomeDegree.Marginal, "Expected Major Pass @ Extremely Easy vs Pass @ Extremely Easy to be marginal.");

		outcome = new OpposedOutcome(outcomes1, outcomes3, Difficulty.Hard, Difficulty.Easy);
		Assert.AreEqual(outcome.Outcome, OpposedOutcomeDirection.Proponent, "Expected Major Pass @ Hard vs Major Fail @ Easy to be proponent.");
		Assert.AreEqual(outcome.Degree, OpposedOutcomeDegree.Total, "Expected Major Pass @ Hard vs Major Fail @ Easy  to be total.");
	}
}
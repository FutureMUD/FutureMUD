#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FirearmMathTests
{
	[TestMethod]
	public void CombineModifiers_MixedAdditiveAndMultiplicativeValues_ComposesCorrectly()
	{
		var result = FirearmMath.CombineModifiers(
		[
			new FirearmAttachmentModifiers(1.0, 2.0, 1.2, 1.1, 0.8, 0.9, 1.1, 0.75, -2),
			new FirearmAttachmentModifiers(0.5, -0.5, 0.9, 1.0, 0.75, 1.0, 0.8, 0.8, -1)
		]);

		Assert.AreEqual(1.5, result.AccuracyBonus, 0.0001);
		Assert.AreEqual(1.5, result.AimBonus, 0.0001);
		Assert.AreEqual(1.08, result.DamageMultiplier, 0.0001);
		Assert.AreEqual(0.6, result.RecoilMultiplier, 0.0001);
		Assert.AreEqual(0.6, result.AimLossMultiplier, 0.0001);
		Assert.AreEqual(-3, result.LoudnessOffset);
	}

	[TestMethod]
	public void ProjectileOutcome_LaterBurstPellet_AppliesRoundAndSpreadPenalty()
	{
		var mode = new FirearmFireMode(FirearmFireModeType.Burst, 3, 1.0, 0.0, 0.0);

		var result = FirearmMath.ProjectileOutcome(Outcome.MajorPass, mode, 1, 2, 0.5, 1.0);

		Assert.AreEqual(Outcome.MinorPass, result);
	}

	[TestMethod]
	public void ProjectileOutcome_LargePenalty_ClampsAtMajorFail()
	{
		var mode = new FirearmFireMode(FirearmFireModeType.Automatic, 10, 10.0, 0.0, 0.0);

		var result = FirearmMath.ProjectileOutcome(Outcome.Pass, mode, 9, 31, 10.0, 2.0);

		Assert.AreEqual(Outcome.MajorFail, result);
	}
}

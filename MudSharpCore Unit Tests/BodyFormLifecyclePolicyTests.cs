#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyFormLifecyclePolicyTests
{
	[DataTestMethod]
	[DataRow(false, false, false, false, 0)]
	[DataRow(false, true, false, false, 2)]
	[DataRow(false, false, true, false, 3)]
	[DataRow(false, false, false, true, 4)]
	public void GetDeletionBlocker_ProtectsLiveAndReferencedForms(bool current, bool embodied, bool backup,
		bool physicalReference, int expected)
	{
		var blocker = BodyFormLifecyclePolicy.GetDeletionBlocker(current, embodied, backup, physicalReference);

		Assert.AreEqual(expected, (int)blocker);
		Assert.AreEqual(expected == (int)BodyFormDeletionBlocker.None,
			string.IsNullOrEmpty(BodyFormLifecyclePolicy.GetDeletionFailureMessage(blocker)));
	}

	[TestMethod]
	public void GetDeletionBlocker_CurrentForm_DoesNotEvaluateOtherReferences()
	{
		var blocker = BodyFormLifecyclePolicy.GetDeletionBlocker(
			true,
			() => throw new AssertFailedException("Embodied references should not be evaluated."),
			() => throw new AssertFailedException("Backup references should not be evaluated."),
			() => throw new AssertFailedException("Physical references should not be evaluated."));

		Assert.AreEqual(BodyFormDeletionBlocker.CurrentForm, blocker);
	}
	[DataTestMethod]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void ShouldAttachApparentAge_DoesNotDuplicatePersistentMetadata(bool alreadyAttached, bool expected)
	{
		Assert.AreEqual(expected, BodyFormLifecyclePolicy.ShouldAttachApparentAge(alreadyAttached));
	}

	[DataTestMethod]
	[DataRow(180.0, 170.0, 160.0, 180.0)]
	[DataRow(double.NaN, 170.0, 160.0, 170.0)]
	[DataRow(0.0, double.PositiveInfinity, 160.0, 160.0)]
	public void SelectProvisionedDimension_UsesCandidateTemplateThenFallback(double candidate, double template,
		double fallback, double expected)
	{
		Assert.AreEqual(expected,
			BodyFormLifecyclePolicy.SelectProvisionedDimension(candidate, template, fallback), 0.0001);
	}
}

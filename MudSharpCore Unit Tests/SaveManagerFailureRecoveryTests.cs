#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SaveManagerFailureRecoveryTests
{
	[TestMethod]
	[TestCategory("Y-T24")]
	public void RecoverFailedSaveBatch_RecoversAttemptedOwnersAndRequeuesEntireBatch()
	{
		var manager = new SaveManager();
		var attempted = new RecoverableSaveable();
		var notAttempted = new RecoverableSaveable();

		manager.RecoverFailedSaveBatch(
			[attempted, notAttempted],
			new HashSet<ISaveable> { attempted });

		Assert.AreEqual(1, attempted.RecoveryCalls);
		Assert.IsTrue(attempted.SpecialisedDirtyState);
		Assert.IsTrue(attempted.Changed);
		Assert.IsTrue(manager.IsQueued(attempted));
		Assert.AreEqual(0, notAttempted.RecoveryCalls,
			"An owner whose Save method was not reached must not restore stale state from an earlier save attempt.");
		Assert.IsFalse(notAttempted.SpecialisedDirtyState);
		Assert.IsTrue(notAttempted.Changed);
		Assert.IsTrue(manager.IsQueued(notAttempted));
	}

	private sealed class RecoverableSaveable : ISaveable, IRecoverableSaveFailure
	{
		public IFuturemud Gameworld { get; } = Mock.Of<IFuturemud>();
		public bool Changed { get; set; }
		public bool SpecialisedDirtyState { get; private set; }
		public int RecoveryCalls { get; private set; }

		public void Save()
		{
			Changed = false;
			SpecialisedDirtyState = false;
		}

		public void RecoverFromSaveFailure()
		{
			RecoveryCalls++;
			SpecialisedDirtyState = true;
		}
	}
}

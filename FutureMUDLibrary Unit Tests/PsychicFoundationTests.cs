#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Magic;
using MudSharp.RPG.Law;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PsychicFoundationTests
{
	public TestContext TestContext { get; set; } = null!;

	[TestMethod]
	public void LazyHistoryBenchmark_ObservationCostIsIndependentOfUnrelatedItemCount()
	{
		// An item payload is lazy. Unrelated items own no history until observation requires it.
		var measurements = new List<double>();
		foreach (var size in new[] { 10000, 100000 })
		{
			var world = new PsychometricHistory?[size];
			world[0] = new PsychometricHistory();
			var history = world[0]!;
			for (var i = 0; i < 10000; i++) history.ObserveCarrier(1, Now, "epoch");
			var timer = System.Diagnostics.Stopwatch.StartNew();
			for (var i = 0; i < 100000; i++)
			{
				history.ObserveCarrier(i % 2 == 0 ? 1 : null, Now.AddSeconds(i), "epoch");
				history.Record(new(ImpressionKind.Magic, 1, 2, Now.AddSeconds(i), Now.AddSeconds(i + 86400), "activity", 0, ""), true);
			}
			timer.Stop();
			measurements.Add(timer.Elapsed.TotalMilliseconds);
			TestContext.WriteLine($"{size:N0} lazy item slots, 100,000 updates: {timer.Elapsed.TotalMilliseconds:N2} ms; allocated histories: {world.Count(x => x is not null)}.");
			Assert.AreEqual(1, world.Count(x => x is not null));
			Assert.IsTrue(history.Impressions.Count <= 8);
			Assert.IsTrue(history.PreviousCarriers.Count <= 4);
		}
		// Wall-clock figures are reported, not asserted: CI scheduling is not a complexity test.
	}

	[TestMethod]
	public void WitnessPendingReports_RoundTripPreservesDecisionAndNeverRetractsDelivery()
	{
		var memory = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Virtual, SourceId = 7, LocationId = 8,
			ReportDueUtc = Now.AddSeconds(120), IdentityKnown = true, Reliability = 0.65 };
		memory.Forget(Now.AddSeconds(30), TimeSpan.FromMinutes(5), false, 42);
		var restored = CrimeWitnessMemory.Load(memory.Save());
		Assert.IsFalse(restored.CanRecall(Now.AddSeconds(121)));
		Assert.IsTrue(restored.CanRecall(Now.AddSeconds(330)));
		Assert.AreEqual(Now.AddSeconds(120), restored.ReportDueUtc);
		Assert.AreEqual(0.65, restored.Reliability);
		Assert.IsTrue(restored.IdentityKnown);
		restored.ReportDelivered = true;
		restored.Forget(Now.AddSeconds(331), TimeSpan.FromMinutes(1), true, 43);
		Assert.IsTrue(CrimeWitnessMemory.Load(restored.Save()).ReportDelivered);
	}
	private static readonly DateTime Now = new(2026, 9, 5, 0, 0, 0, DateTimeKind.Utc);

	[TestMethod]
	public void Custody_RearrangementPreservesStartAndDropStartsNewPeriod()
	{
		var history = new PsychometricHistory();
		history.ObserveCarrier(1, Now, "epoch");
		Assert.IsFalse(history.ObserveCarrier(1, Now.AddHours(1), "epoch"));
		Assert.AreEqual(Now, history.CurrentCarrier!.SinceUtc);
		history.ObserveCarrier(null, Now.AddHours(2), "epoch");
		history.ObserveCarrier(1, Now.AddHours(3), "epoch");
		Assert.AreEqual(Now.AddHours(3), history.CurrentCarrier!.SinceUtc);
		Assert.AreEqual(Now.AddHours(2), history.PreviousCarriers.Single().UntilUtc);
	}

	[TestMethod]
	public void Custody_DisabledIntervalIsNotInvented()
	{
		var history = new PsychometricHistory();
		history.ObserveCarrier(1, Now, "before");
		history.ObserveCarrier(1, Now.AddDays(2), "after");
		Assert.AreEqual(Now.AddDays(2), history.CurrentCarrier!.SinceUtc);
		Assert.IsTrue(history.CurrentCarrier.UnknownBeginning);
		Assert.AreEqual(0, history.PreviousCarriers.Count);
	}

	[TestMethod]
	public void History_BoundsCustodyAndImpressionsUnderLargeEventVolume()
	{
		var history = new PsychometricHistory();
		for (var i = 0; i < 100000; i++)
		{
			history.ObserveCarrier(i + 1, Now.AddSeconds(i), "epoch");
			history.Record(new(ImpressionKind.Violence, i, null, Now.AddSeconds(i), Now.AddDays(7), "violence", 0, ""), true);
		}
		Assert.AreEqual(4, history.PreviousCarriers.Count);
		Assert.AreEqual(8, history.Impressions.Count);
	}

	[TestMethod]
	public void Impressions_CoalesceButKeepDeathsAndExpireFeelings()
	{
		var history = new PsychometricHistory();
		var feeling = new PsychometricImpression(ImpressionKind.Feeling, 1, null, Now, Now.AddMinutes(10), new string('a', 300), 0, "");
		Assert.IsTrue(history.Record(feeling, false));
		Assert.IsFalse(history.Record(feeling with { CreatedUtc = Now.AddSeconds(1) }, false));
		Assert.AreEqual(256, history.Impressions[0].Text.Length);
		Assert.IsTrue(history.Record(feeling with { Kind = ImpressionKind.Death }, false));
		Assert.IsTrue(history.Record(feeling with { Kind = ImpressionKind.Death }, false));
		history.Prune(Now.AddMinutes(11));
		Assert.AreEqual(0, history.Impressions.Count);
	}

	[TestMethod]
	public void Witness_ForgettingNeverRetractsDeliveredEvidenceAndRoundTrips()
	{
		var witness = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Virtual, SourceId = 4,
			LocationId = 5, ReportDelivered = true, IdentityKnown = true, Reliability = 0.8 };
		witness.Forget(Now, TimeSpan.FromMinutes(5), false, 9);
		var loaded = CrimeWitnessMemory.Load(witness.Save());
		Assert.IsFalse(loaded.CanRecall(Now));
		Assert.IsTrue(loaded.CanRecall(Now.AddMinutes(5)));
		Assert.IsTrue(loaded.ReportDelivered);
		Assert.IsTrue(loaded.IdentityKnown);
		Assert.AreEqual(0.8, loaded.Reliability);
		Assert.AreEqual(1, loaded.Audit.Count);
	}

	[TestMethod]
	public void Witness_PermanentForgettingRequiresRestoration()
	{
		var witness = new CrimeWitnessMemory { Kind = CrimeWitnessSourceKind.Character, SourceId = 1 };
		witness.Forget(Now, TimeSpan.Zero, true, 2);
		witness.Forget(Now, TimeSpan.FromMinutes(1), false, 3);
		Assert.IsFalse(witness.CanRecall(Now.AddYears(10)));
		witness.Restore(Now.AddHours(1), 4);
		Assert.IsTrue(witness.CanRecall(Now.AddHours(1)));
		Assert.AreEqual(3, witness.Audit.Count);
	}

	[TestMethod]
	public void Transfer_ClampsBeforeDebitAndConservesAfterLoss()
	{
		var resource = new Mock<IMagicResource>();
		var donor = new Mock<IHaveMagicResource>();
		var recipient = new Mock<IHaveMagicResource>();
		donor.SetupGet(x => x.MagicResourceAmounts).Returns(new Dictionary<IMagicResource, double> { [resource.Object] = 50 });
		recipient.SetupGet(x => x.MagicResourceAmounts).Returns(new Dictionary<IMagicResource, double> { [resource.Object] = 95 });
		resource.Setup(x => x.ResourceCap(recipient.Object)).Returns(100);
		donor.Setup(x => x.UseResource(resource.Object, 10)).Returns(true);
		var result = MagicResourceTransfer.Transfer(donor.Object, recipient.Object, resource.Object, 40, 0.5);
		Assert.AreEqual(10, result.Removed);
		Assert.AreEqual(5, result.Received);
		recipient.Verify(x => x.AddResource(resource.Object, 5), Times.Once);
	}

	[TestMethod]
	public void Transfer_FailedDebitCannotCreateResources()
	{
		var resource = new Mock<IMagicResource>();
		var donor = new Mock<IHaveMagicResource>();
		var recipient = new Mock<IHaveMagicResource>();
		donor.SetupGet(x => x.MagicResourceAmounts).Returns(new Dictionary<IMagicResource, double> { [resource.Object] = 50 });
		recipient.SetupGet(x => x.MagicResourceAmounts).Returns(new Dictionary<IMagicResource, double>());
		resource.Setup(x => x.ResourceCap(recipient.Object)).Returns(100);
		Assert.AreEqual(default(MagicResourceTransferResult), MagicResourceTransfer.Transfer(donor.Object, recipient.Object, resource.Object, 40, 0));
		recipient.Verify(x => x.AddResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
	}
}

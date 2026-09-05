#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PsychometricRecorderTests
{
	public TestContext TestContext { get; set; } = null!;

	[TestMethod]
	public void SplitAndMerge_CopyIsIdempotentAndMixedProvenanceRetainsAuthoredClues()
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticBool(PsychometricRecorder.EnabledSetting)).Returns(true);
		var left = Item(world.Object, out var leftEffects);
		var right = Item(world.Object, out var rightEffects);
		var author = new Mock<ICharacter>();
		PsychometricRecorder.AuthorClue(left.Object, author.Object, "Left clue");
		PsychometricRecorder.CopyHistory(left.Object, right.Object);
		PsychometricRecorder.CopyHistory(left.Object, right.Object);
		Assert.AreEqual(1, rightEffects.Single().History.Impressions.Count);
		PsychometricRecorder.AuthorClue(right.Object, author.Object, "Right clue");
		PsychometricRecorder.MergeHistory(left.Object, right.Object);
		Assert.IsTrue(leftEffects.Single().History.MixedProvenance);
		Assert.AreEqual(2, leftEffects.Single().History.Impressions.Count);
	}

	[TestMethod]
	public void DisabledRecorder_PreservesStoredCluesAndDoesNotObserveCustody()
	{
		var world = new Mock<IFuturemud>();
		var item = Item(world.Object, out var effects);
		var author = new Mock<ICharacter>();
		var history = new PsychometricHistoryEffect(item.Object);
		history.History.Record(new(ImpressionKind.Authored, 1, null, DateTime.UtcNow, null, "An old clue", 0, ""), true);
		effects.Add(history);
		Assert.IsNull(PsychometricRecorder.Read(item.Object));
		Assert.IsFalse(PsychometricRecorder.AuthorClue(item.Object, author.Object, "new clue"));
		PsychometricRecorder.ObserveCustody(item.Object);
		Assert.AreEqual(1, history.History.Impressions.Count);
		item.VerifyGet(x => x.InInventoryOf, Times.Never);
		item.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never);
	}

	[TestMethod]
	public void Custody_RearrangementPreservesBeginningAndNewEpochMakesBeginningUnknown()
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticBool(PsychometricRecorder.EnabledSetting)).Returns(true);
		var epoch = "first";
		world.Setup(x => x.GetStaticConfiguration(PsychometricRecorder.EpochSetting)).Returns(() => epoch);
		var item = Item(world.Object, out var effects);
		var body = new Mock<IBody>();
		var carrier = new Mock<ICharacter>();
		carrier.SetupGet(x => x.Id).Returns(42);
		body.SetupGet(x => x.Actor).Returns(carrier.Object);
		item.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		PsychometricRecorder.ObserveCustody(item.Object);
		var first = effects.Single().History.CurrentCarrier;
		PsychometricRecorder.ObserveCustody(item.Object);
		Assert.AreEqual(first, effects.Single().History.CurrentCarrier);
		epoch = "after-disabled-gap";
		PsychometricRecorder.ObserveCustody(item.Object);
		Assert.IsTrue(effects.Single().History.CurrentCarrier!.UnknownBeginning);
		Assert.AreEqual(0, effects.Single().History.PreviousCarriers.Count);
		Assert.AreEqual(epoch, effects.Single().History.Epoch);
	}

	[DataTestMethod]
	[DataRow(10000)]
	[DataRow(100000)]
	[TestCategory("PsychicBenchmark")]
	public void Recording_LargeItemRegistryDoesNotEnumerateUnrelatedItems(int count)
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticBool(PsychometricRecorder.EnabledSetting)).Returns(true);
		// Allocate distinct item instances, but no history payloads for untouched items.
		var items = Enumerable.Range(0, count).Select(_ => new Mock<IGameItem>().Object).ToArray();
		var registry = new Mock<IUneditableAll<IGameItem>>(MockBehavior.Strict);
		registry.SetupGet(x => x.Count).Returns(items.Length);
		world.SetupGet(x => x.Items).Returns(registry.Object);
		var item = Item(world.Object, out var effects);
		items[0] = item.Object;
		var author = new Mock<ICharacter>();
		author.SetupGet(x => x.Id).Returns(1);
		PsychometricRecorder.AuthorClue(item.Object, author.Object, "warmup");
		var watch = Stopwatch.StartNew();
		for (var i = 0; i < 10000; i++) PsychometricRecorder.AuthorClue(item.Object, author.Object, "bounded clue");
		watch.Stop();
		Assert.AreEqual(8, effects.Single().History.Impressions.Count);
		world.VerifyGet(x => x.Items, Times.Never);
		TestContext.WriteLine($"{count:N0} distinct mock items: 10,000 recorder updates in {watch.Elapsed.TotalMilliseconds:F2} ms; no registry access.");
		GC.KeepAlive(items);
	}

	private static Mock<IGameItem> Item(IFuturemud world, out List<PsychometricHistoryEffect> effects)
	{
		var stored = new List<PsychometricHistoryEffect>();
		effects = stored;
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Gameworld).Returns(world);
		item.Setup(x => x.EffectsOfType<PsychometricHistoryEffect>(It.IsAny<Predicate<PsychometricHistoryEffect>?>()))
			.Returns<Predicate<PsychometricHistoryEffect>?>(predicate => predicate is null ? stored : stored.Where(x => predicate(x)));
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effect => stored.Add((PsychometricHistoryEffect)effect));
		return item;
	}
}

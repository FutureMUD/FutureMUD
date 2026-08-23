#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ScanTargetAcquisitionTests
{
	[TestMethod]
	public void AcquireVisibleCharacters_PassingScanRegistersVisibleTarget()
	{
		ScanFixture fixture = CreateFixture(Outcome.Pass, SizeCategory.Normal, RoomLayer.GroundLevel);

		IReadOnlyList<ICharacter> acquired = ScanTargetAcquisition.AcquireVisibleCharacters(
			fixture.Observer.Object, 5);

		CollectionAssert.Contains(acquired.ToList(), fixture.Target.Object);
		fixture.Observer.Verify(x => x.SeeTarget(fixture.Target.Object), Times.Once);
	}

	[TestMethod]
	public void AcquireVisibleCharacters_FailedSizeOrLayerCheckDoesNotRegisterTarget()
	{
		ScanFixture failedSize = CreateFixture(Outcome.MajorFail, SizeCategory.Normal, RoomLayer.GroundLevel);

		IReadOnlyList<ICharacter> sizeAcquired = ScanTargetAcquisition.AcquireVisibleCharacters(
			failedSize.Observer.Object, 5);

		Assert.AreEqual(0, sizeAcquired.Count);
		failedSize.Observer.Verify(x => x.SeeTarget(It.IsAny<IMortalPerceiver>()), Times.Never);

		ScanFixture hiddenLayer = CreateFixture(Outcome.MajorPass, SizeCategory.Titanic,
			RoomLayer.VeryDeepUnderwater);
		IReadOnlyList<ICharacter> layerAcquired = ScanTargetAcquisition.AcquireVisibleCharacters(
			hiddenLayer.Observer.Object, 5);

		Assert.AreEqual(0, layerAcquired.Count);
		hiddenLayer.Observer.Verify(x => x.SeeTarget(It.IsAny<IMortalPerceiver>()), Times.Never);
	}

	[TestMethod]
	public void IsCurrentVisibleRangedTarget_RequiresRegisteredCurrentVisibility()
	{
		ScanFixture fixture = CreateFixture(Outcome.Pass, SizeCategory.Normal, RoomLayer.GroundLevel);
		fixture.Observer.SetupGet(x => x.SeenTargets).Returns([fixture.Target.Object]);
		fixture.Observer.Setup(x => x.CanSee((IPerceivable)fixture.Target.Object,
			PerceiveIgnoreFlags.IgnoreObscured)).Returns(false);

		Assert.IsFalse(ScanTargetAcquisition.IsCurrentVisibleRangedTarget(fixture.Observer.Object,
			fixture.Target.Object, 5));

		fixture.Observer.Setup(x => x.CanSee((IPerceivable)fixture.Target.Object,
			PerceiveIgnoreFlags.IgnoreObscured)).Returns(true);

		Assert.IsTrue(ScanTargetAcquisition.IsCurrentVisibleRangedTarget(fixture.Observer.Object,
			fixture.Target.Object, 5));
	}

	[TestMethod]
	public void ScanTargetAcquisition_UsesBoundedDoorAndCornerAwareTraversalForOrdinaryCells()
	{
		string source = System.IO.File.ReadAllText(System.IO.Path.GetFullPath(System.IO.Path.Combine(
			AppContext.BaseDirectory, "..", "..", "..", "..", "MudSharpCore", "PerceptionEngine",
			"ScanTargetAcquisition.cs")));
		string animalAiSource = System.IO.File.ReadAllText(System.IO.Path.GetFullPath(System.IO.Path.Combine(
			AppContext.BaseDirectory, "..", "..", "..", "..", "MudSharpCore", "NPC", "AI", "AnimalAI.cs")));

		StringAssert.Contains(source, "CellsAndDistancesInVicinity(maximumRange, true, true)");
		StringAssert.Contains(animalAiSource,
			"Math.Min(Math.Max(0, EffectiveAwarenessRange), (int)character.MaximumPerceptionRange)");
		StringAssert.Contains(source, "candidate.Cell.SpotDifficulty(observer)");
		StringAssert.Contains(source, "candidate.Target.RoomLayer.CanBeSeenFromLayer(observer.RoomLayer)");
		StringAssert.Contains(source, "GetPerceivablesWithinAcrossLayers");
	}

	private static ScanFixture CreateFixture(Outcome outcome, SizeCategory targetSize, RoomLayer targetLayer)
	{
		var cell = new Mock<ICell>();
		var observer = new Mock<ICharacter>();
		var target = new Mock<ICharacter>();
		var check = new Mock<ICheck>();
		var gameworld = new Mock<IFuturemud>();

		cell.SetupGet(x => x.Characters).Returns([target.Object]);
		cell.Setup(x => x.ExitsFor(null!, true)).Returns([]);
		cell.Setup(x => x.SpotDifficulty(observer.Object)).Returns(Difficulty.Normal);
		observer.SetupGet(x => x.Location).Returns(cell.Object);
		observer.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		observer.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		observer.SetupGet(x => x.SeenTargets).Returns([]);
		observer.Setup(x => x.CanSee((IPerceivable)target.Object,
			PerceiveIgnoreFlags.IgnoreObscured)).Returns(true);
		observer.Setup(x => x.CurrentContextualSize(SizeContext.None)).Returns(SizeCategory.Normal);
		target.SetupGet(x => x.Location).Returns(cell.Object);
		target.SetupGet(x => x.RoomLayer).Returns(targetLayer);
		target.Setup(x => x.CurrentContextualSize(SizeContext.Scan)).Returns(targetSize);
		check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable?>(), It.IsAny<double>(),
			It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()))
			.Returns(Enum.GetValues<Difficulty>()
				.ToDictionary(x => x, x => CheckOutcome.SimpleOutcome(CheckType.ScanPerceptionCheck, outcome)));
		gameworld.Setup(x => x.GetCheck(CheckType.ScanPerceptionCheck)).Returns(check.Object);

		return new ScanFixture(observer, target);
	}

	private sealed record ScanFixture(Mock<ICharacter> Observer, Mock<ICharacter> Target);
}

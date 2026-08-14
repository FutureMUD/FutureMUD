#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AudioPerceptionTests
{
	[TestMethod]
	public void CanHear_UsesLocalHearingProfileDifficultyAndReturnsCheckOutcome()
	{
		var listener = new Mock<ICharacter>();
		var source = new Mock<ICharacter>();
		var location = new Mock<ICell>();
		var gameworld = new Mock<IFuturemud>();
		var check = new Mock<ICheck>();
		listener.SetupGet(x => x.Location).Returns(location.Object);
		listener.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		listener.Setup(x => x.IsSelf(source.Object)).Returns(false);
		listener.Setup(x => x.CanHear(source.Object)).Returns(true);
		location.Setup(x => x.LocalAudioDifficulty(
			listener.Object, AudioVolume.Quiet, Proximity.VeryDistant)).Returns(Difficulty.Hard);
		gameworld.Setup(x => x.GetCheck(CheckType.GenericListenCheck)).Returns(check.Object);
		check.Setup(x => x.Check(
			It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable?>(),
			It.IsAny<IUseTrait?>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericListenCheck, Outcome.Pass));

		var result = AudioPerception.CanHear(
			listener.Object, source.Object, AudioVolume.Quiet, Proximity.VeryDistant);

		Assert.IsTrue(result);
		location.Verify(x => x.LocalAudioDifficulty(
			listener.Object, AudioVolume.Quiet, Proximity.VeryDistant), Times.Once);
		check.Verify(x => x.Check(
			listener.Object, Difficulty.Hard, source.Object, null, 0.0, TraitUseType.Practical,
			It.IsAny<(string Parameter, object value)[]>()), Times.Once);
	}

	[TestMethod]
	public void CanHear_NativeAudibilityFailure_DoesNotRollListenCheck()
	{
		var listener = new Mock<ICharacter>();
		var source = new Mock<ICharacter>();
		var gameworld = new Mock<IFuturemud>();
		listener.Setup(x => x.IsSelf(source.Object)).Returns(false);
		listener.Setup(x => x.CanHear(source.Object)).Returns(false);
		listener.SetupGet(x => x.Gameworld).Returns(gameworld.Object);

		Assert.IsFalse(AudioPerception.CanHear(
			listener.Object, source.Object, AudioVolume.Loud, Proximity.Immediate));
		gameworld.Verify(x => x.GetCheck(It.IsAny<CheckType>()), Times.Never);
	}
}

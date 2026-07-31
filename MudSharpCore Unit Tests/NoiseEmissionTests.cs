#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NoiseEmissionTests
{
	[TestMethod]
	public void RaiseEvent_NonSilentNoise_FiresOnceOnOriginWithDocumentedPayload()
	{
		var origin = new Mock<ICell>();
		var source = new Mock<ICharacter>();
		object[]? payload = null;
		origin.Setup(x => x.HandleEvent(EventType.NoiseEmitted, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) => payload = arguments);

		var raised = NoiseEmission.RaiseEvent(
			origin.Object,
			source.Object,
			AudioVolume.Loud,
			" alarm ",
			"You hear an alarm {0}.");

		Assert.IsTrue(raised);
		origin.Verify(x => x.HandleEvent(EventType.NoiseEmitted, It.IsAny<object[]>()), Times.Once);
		Assert.IsNotNull(payload);
		Assert.AreEqual(5, payload.Length);
		Assert.AreSame(origin.Object, payload[0]);
		Assert.AreSame(source.Object, payload[1]);
		Assert.AreEqual((int)AudioVolume.Loud, payload[2]);
		Assert.AreEqual("alarm", payload[3]);
		Assert.AreEqual("You hear an alarm {0}.", payload[4]);
	}

	[TestMethod]
	public void RaiseEvent_SilentNoise_DoesNotFire()
	{
		var origin = new Mock<ICell>();
		var source = new Mock<ICharacter>();

		var raised = NoiseEmission.RaiseEvent(
			origin.Object,
			source.Object,
			AudioVolume.Silent,
			"alarm",
			"You hear an alarm {0}.");

		Assert.IsFalse(raised);
		origin.Verify(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void EmitNoise_ValidArguments_UsesSharedAudioPath()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var origin = new Mock<ICell>();
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.Location).Returns(origin.Object);
		source.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x =>
				x.FunctionName.EqualTo("emitnoise") &&
				x.Parameters.SequenceEqual([
					ProgVariableTypes.Perceivable,
					ProgVariableTypes.Number,
					ProgVariableTypes.Text,
					ProgVariableTypes.Text
				]));
		var function = compiler.CompilerFunction(
			[
				new ConstantFunction(source.Object),
				new ConstantFunction(new NumberVariable((int)AudioVolume.VeryLoud)),
				new ConstantFunction(new TextVariable("impact")),
				new ConstantFunction(new TextVariable("You hear an impact {0} at {1} volume."))
			],
			FutureProgTestBootstrap.Gameworld);

		var result = function.Execute(new Mock<IVariableSpace>().Object);

		Assert.AreEqual(StatementResult.Normal, result);
		Assert.AreEqual(true, function.Result?.GetObject);
		origin.Verify(x => x.HandleAudioEcho(
			"You hear an impact {0} at {1} volume.",
			AudioVolume.VeryLoud,
			source.Object,
			RoomLayer.GroundLevel,
			true,
			"impact"), Times.Once);
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Computers;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NoiseEmissionTests
{
	[TestMethod]
	public void MediaAudioVolume_SourceAndOutputSettings_ScaleAndMuteWithoutRemovingVideo()
	{
		var packet = new MediaPacket(Guid.NewGuid(), 1L, DateTime.UtcNow,
			MediaCapabilities.Audio | MediaCapabilities.Video, MediaEventKind.AudioVideo,
			new MediaEndpointAddress(1L, 2L, "camera"), [],
			new MediaTextPayload("A television programme plays.", true, true, (int)AudioVolume.Quiet));

		var amplified = MediaAudioPresentation.ApplyOutputVolume(packet, AudioVolume.VeryLoud);
		var muted = MediaAudioPresentation.ApplyOutputVolume(packet, AudioVolume.Silent);

		Assert.AreEqual(AudioVolume.Loud, MediaComponentUtilities.GetAudioVolume(amplified));
		Assert.AreEqual(MediaCapabilities.Audio | MediaCapabilities.Video, amplified.Capabilities);
		Assert.AreEqual(MediaCapabilities.Video, muted.Capabilities);
		Assert.AreEqual(AudioVolume.Silent, MediaComponentUtilities.GetAudioVolume(muted));
	}

	[TestMethod]
	public void MediaTextPayload_LegacyJsonWithoutVolume_DefaultsToUnitySourceVolume()
	{
		var payload = JsonSerializer.Deserialize<MediaTextPayload>(
			"""{"Text":"legacy audio","IsAudible":true,"IsVisual":false}""");

		Assert.IsNotNull(payload);
		var packet = CreateAudioMediaPacket(AudioVolume.Quiet) with { Payload = payload };
		Assert.AreEqual(AudioVolume.Decent, MediaComponentUtilities.GetAudioVolume(packet));
	}

	[TestMethod]
	public void IsLoudFeedbackLoop_RequiresLoudAudioAndRepeatedCaptureEndpoint()
	{
		var camera = new MediaEndpointAddress(1L, 2L, "camera");
		var monitor = new MediaEndpointAddress(3L, 4L, "monitor", MediaEndpointDirection.Input);
		var quietLoop = CreateAudioMediaPacket(AudioVolume.Quiet) with { Provenance = [camera, monitor] };
		var loudLoop = CreateAudioMediaPacket(AudioVolume.Loud) with { Provenance = [camera, monitor] };
		var loudFirstCapture = CreateAudioMediaPacket(AudioVolume.Loud) with
		{
			Source = new MediaEndpointAddress(5L, 6L, "other-camera"),
			Provenance = [monitor]
		};

		Assert.IsFalse(MediaComponentUtilities.IsLoudFeedbackLoop(quietLoop, camera));
		Assert.IsTrue(MediaComponentUtilities.IsLoudFeedbackLoop(loudLoop, camera));
		Assert.IsFalse(MediaComponentUtilities.IsLoudFeedbackLoop(loudFirstCapture, camera));
	}

	[TestMethod]
	public void MediaVolume_MuteAlias_SetsPlayerFacingAudioSinkToSilent()
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		var item = new Mock<IGameItem>();
		item.Setup(x => x.HowSeen(actor.Object, true, DescriptionType.Short, true, PerceiveIgnoreFlags.None))
			.Returns("a media monitor");
		var sink = new Mock<IMediaAudioSink>();
		var error = string.Empty;
		sink.Setup(x => x.SetOutputVolume(AudioVolume.Silent, out error)).Returns(true);
		var method = typeof(ElectronicsModule).GetMethod("MediaVolume",
			BindingFlags.Static | BindingFlags.NonPublic);

		Assert.IsNotNull(method);
		method.Invoke(null, [actor.Object, item.Object, sink.Object, new StringStack("mute")]);

		sink.Verify(x => x.SetOutputVolume(AudioVolume.Silent, out It.Ref<string>.IsAny), Times.Once);
	}

	[TestMethod]
	public void EmitPlaybackNoise_QuietAudio_RaisesNoiseEventWithoutNearbyPropagation()
	{
		var origin = new Mock<ICell>();
		var device = new Mock<IGameItem>();
		device.SetupGet(x => x.TrueLocations).Returns([origin.Object]);
		var packet = CreateAudioMediaPacket(AudioVolume.Quiet);

		MediaAudioPresentation.EmitPlaybackNoise(device.Object, packet);

		origin.Verify(x => x.HandleEvent(EventType.NoiseEmitted,
			It.Is<object[]>(args =>
				ReferenceEquals(args[0], origin.Object) &&
				ReferenceEquals(args[1], device.Object) &&
				(int)args[2] == (int)AudioVolume.Quiet &&
				(string)args[3] == "media playback")), Times.Once);
		origin.Verify(x => x.HandleAudioEcho(It.IsAny<string>(), It.IsAny<AudioVolume>(),
			It.IsAny<IPerceiver>(), It.IsAny<RoomLayer>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void EmitPlaybackNoise_LoudAudio_UsesNormalNearbyPropagationPath()
	{
		var origin = new Mock<ICell>();
		var device = new Mock<IGameItem>();
		device.SetupGet(x => x.TrueLocations).Returns([origin.Object]);
		device.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);

		MediaAudioPresentation.EmitPlaybackNoise(device.Object, CreateAudioMediaPacket(AudioVolume.Loud));

		origin.Verify(x => x.HandleAudioEcho(
			"You hear audio from an electronic media device {0}.", AudioVolume.Loud, device.Object,
			RoomLayer.GroundLevel, true, "media playback"), Times.Once);
	}

	[TestMethod]
	public void EmitFeedback_IsVeryLoudLocalNoiseThatCannotPropagateOrBeRecaptured()
	{
		var origin = new Mock<ICell>();
		var device = new Mock<IGameItem>();
		device.SetupGet(x => x.TrueLocations).Returns([origin.Object]);

		MediaAudioPresentation.EmitFeedback(device.Object);

		device.Verify(x => x.Handle(
			It.Is<AudioOutput>(output =>
				output.Volume == AudioVolume.VeryLoud &&
				output.Flags.HasFlag(OutputFlags.IgnoreWatchers) &&
				output.Flags.HasFlag(OutputFlags.PurelyAudible)),
			OutputRange.Local), Times.Once);
		origin.Verify(x => x.HandleEvent(EventType.NoiseEmitted,
			It.Is<object[]>(args =>
				(int)args[2] == (int)AudioVolume.VeryLoud &&
				(string)args[3] == "electronic feedback")), Times.Once);
		origin.Verify(x => x.HandleAudioEcho(It.IsAny<string>(), It.IsAny<AudioVolume>(),
			It.IsAny<IPerceiver>(), It.IsAny<RoomLayer>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
	}

	private static MediaPacket CreateAudioMediaPacket(AudioVolume volume)
	{
		return new MediaPacket(Guid.NewGuid(), 1L, DateTime.UtcNow, MediaCapabilities.Audio,
			MediaEventKind.Audio, new MediaEndpointAddress(1L, 2L, "camera"), [],
			new MediaTextPayload("Audio plays.", true, false, (int)volume));
	}

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

	[TestMethod]
	public void HandleEvent_SelfRecursiveHook_ExecutesOnlyOnce()
	{
		var target = new TestPerceivedItem(1);
		var calls = 0;
		var hook = CreateHook(EventType.NoiseEmitted, (type, arguments) =>
		{
			calls++;
			target.HandleEvent(type, arguments);
			return true;
		});
		target.InstallHook(hook.Object);

		target.HandleEvent(EventType.NoiseEmitted, target);

		Assert.AreEqual(1, calls);
	}

	[TestMethod]
	public void HandleEvent_MutuallyRecursiveHooks_ExecuteOnceEach()
	{
		var target = new TestPerceivedItem(1);
		var firstCalls = 0;
		var secondCalls = 0;
		var first = CreateHook(EventType.NoiseEmitted, (type, arguments) =>
		{
			firstCalls++;
			target.HandleEvent(type, arguments);
			return true;
		});
		var second = CreateHook(EventType.NoiseEmitted, (type, arguments) =>
		{
			secondCalls++;
			target.HandleEvent(type, arguments);
			return true;
		});
		target.InstallHook(first.Object);
		target.InstallHook(second.Object);

		target.HandleEvent(EventType.NoiseEmitted, target);

		Assert.AreEqual(1, firstCalls);
		Assert.AreEqual(1, secondCalls);
	}

	[TestMethod]
	public void HandleEvent_SameHookOnDifferentTargets_CanRunReentrantly()
	{
		var firstTarget = new TestPerceivedItem(1);
		var secondTarget = new TestPerceivedItem(2);
		var calls = 0;
		var hook = CreateHook(EventType.NoiseEmitted, (type, arguments) =>
		{
			calls++;
			if (ReferenceEquals(arguments[0], firstTarget))
			{
				secondTarget.HandleEvent(type, secondTarget);
			}

			return true;
		});
		firstTarget.InstallHook(hook.Object);
		secondTarget.InstallHook(hook.Object);

		firstTarget.HandleEvent(EventType.NoiseEmitted, firstTarget);

		Assert.AreEqual(2, calls);
	}

	[TestMethod]
	public void HandleEvent_SequentialHookCalls_ExecuteEachTime()
	{
		var target = new TestPerceivedItem(1);
		var calls = 0;
		var hook = CreateHook(EventType.NoiseEmitted, (_, _) =>
		{
			calls++;
			return true;
		});
		target.InstallHook(hook.Object);

		target.HandleEvent(EventType.NoiseEmitted, target);
		target.HandleEvent(EventType.NoiseEmitted, target);

		Assert.AreEqual(2, calls);
	}

	[TestMethod]
	public void HandleEvent_ThrowingHook_ReleasesReentrancyGuard()
	{
		var target = new TestPerceivedItem(1);
		var calls = 0;
		var hook = CreateHook(EventType.NoiseEmitted, (_, _) =>
		{
			calls++;
			if (calls == 1)
			{
				throw new InvalidOperationException();
			}

			return true;
		});
		target.InstallHook(hook.Object);

		Assert.ThrowsException<InvalidOperationException>(() => target.HandleEvent(EventType.NoiseEmitted, target));
		target.HandleEvent(EventType.NoiseEmitted, target);

		Assert.AreEqual(2, calls);
	}

	private static Mock<IHook> CreateHook(EventType type, Func<EventType, object[], bool> function)
	{
		var hook = new Mock<IHook>();
		hook.SetupGet(x => x.Type).Returns(type);
		hook.SetupGet(x => x.Function).Returns(function);
		return hook;
	}

	private sealed class TestPerceivedItem : PerceivedItem
	{
		public TestPerceivedItem(long id) : base(id)
		{
			_name = $"test item {id}";
			_keywords = new Lazy<List<string>>(() => ["test", "item"]);
		}

		public override string FrameworkItemType => "TestPerceivedItem";
		public override ProgVariableTypes Type => ProgVariableTypes.Perceivable;

		public override void Register(IOutputHandler handler)
		{
		}

		public override object DatabaseInsert()
		{
			return this;
		}

		public override void SetIDFromDatabase(object dbitem)
		{
		}
	}

	[TestMethod]
	public void RaiseReceivedEvent_UsesDocumentedReceiverPayload()
	{
		var origin = new Mock<ICell>();
		var source = new Mock<ICharacter>();
		var listener = new Mock<ICharacter>();
		object[]? payload = null;
		listener.Setup(x => x.HandleEvent(EventType.CharacterNoiseReceived, It.IsAny<object[]>()))
			.Callback<EventType, object[]>((_, arguments) => payload = arguments);

		NoiseEmission.RaiseReceivedEvent(
			listener.Object,
			origin.Object,
			source.Object,
			AudioVolume.Quiet,
			Proximity.VeryDistant,
			" impact ",
			"from the north",
			"A crash sounds {0}.");

		Assert.IsNotNull(payload);
		Assert.AreEqual(8, payload.Length);
		Assert.AreSame(listener.Object, payload[0]);
		Assert.AreSame(origin.Object, payload[1]);
		Assert.AreSame(source.Object, payload[2]);
		Assert.AreEqual((int)AudioVolume.Quiet, payload[3]);
		Assert.AreEqual((int)Proximity.VeryDistant, payload[4]);
		Assert.AreEqual("impact", payload[5]);
		Assert.AreEqual("from the north", payload[6]);
		Assert.AreEqual("A crash sounds {0}.", payload[7]);
	}

	[TestMethod]
	public void EmitNoise_ExtendedArguments_UseBoundedStructuredPath()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var origin = new Mock<ICell>();
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.Location).Returns(origin.Object);
		source.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x => x.FunctionName.EqualTo("emitnoise") &&
			             x.Parameters.SequenceEqual([
				             ProgVariableTypes.Perceivable,
				             ProgVariableTypes.Number,
				             ProgVariableTypes.Number,
				             ProgVariableTypes.Text,
				             ProgVariableTypes.Text,
				             ProgVariableTypes.Text
			             ]));
		var function = compiler.CompilerFunction(
			[
				new ConstantFunction(source.Object),
				new ConstantFunction(new NumberVariable((int)AudioVolume.VeryLoud)),
				new ConstantFunction(new NumberVariable(20)),
				new ConstantFunction(new TextVariable("coordinate")),
				new ConstantFunction(new TextVariable("impact")),
				new ConstantFunction(new TextVariable("A crash sounds {0} at {1} volume."))
			],
			FutureProgTestBootstrap.Gameworld);

		var result = function.Execute(new Mock<IVariableSpace>().Object);

		Assert.AreEqual(StatementResult.Normal, result);
		Assert.AreEqual(true, function.Result?.GetObject);
		origin.Verify(x => x.HandleAudioEcho(
			"A crash sounds {0} at {1} volume.",
			AudioVolume.VeryLoud,
			20.0,
			AudioPropagationMode.CoordinateAware,
			source.Object,
			RoomLayer.GroundLevel,
			true,
			"impact"), Times.Once);
	}

	[TestMethod]
	public void EmitNoise_ExtendedArguments_RejectInvalidBudgetAndMode()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var origin = new Mock<ICell>();
		var source = new Mock<ICharacter>();
		source.SetupGet(x => x.Location).Returns(origin.Object);
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x => x.FunctionName.EqualTo("emitnoise") && x.Parameters.Count() == 6);

		foreach (var (budget, mode) in new[] { (0, "topological"), (5, "geometric") })
		{
			var function = compiler.CompilerFunction(
				[
					new ConstantFunction(source.Object),
					new ConstantFunction(new NumberVariable((int)AudioVolume.Loud)),
					new ConstantFunction(new NumberVariable(budget)),
					new ConstantFunction(new TextVariable(mode)),
					new ConstantFunction(new TextVariable("impact")),
					new ConstantFunction(new TextVariable("A crash sounds {0}."))
				],
				FutureProgTestBootstrap.Gameworld);

			Assert.AreEqual(StatementResult.Normal, function.Execute(new Mock<IVariableSpace>().Object));
			Assert.AreEqual(false, function.Result?.GetObject);
		}

		origin.Verify(x => x.HandleAudioEcho(
			It.IsAny<string>(), It.IsAny<AudioVolume>(), It.IsAny<double>(),
			It.IsAny<AudioPropagationMode>(), It.IsAny<IPerceiver>(), It.IsAny<RoomLayer>(),
			It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void FutureProgContract_RegistersReceivedNoiseHearingDecision()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var compiler = FutureProg.GetFunctionCompilerInformations()
			.Single(x => x.FunctionName.EqualTo("canhearnoise"));

		CollectionAssert.AreEqual(
			new[]
			{
				ProgVariableTypes.Character,
				ProgVariableTypes.Perceivable,
				ProgVariableTypes.Number,
				ProgVariableTypes.Number
			},
			compiler.Parameters.ToArray());
		Assert.AreEqual(ProgVariableTypes.Boolean, compiler.ReturnType);
		Assert.AreEqual(146, (int)EventType.CharacterNoiseReceived);
	}
}

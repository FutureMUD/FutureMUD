#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;

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

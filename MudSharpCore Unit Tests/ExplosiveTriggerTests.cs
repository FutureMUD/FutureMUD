#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Save;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ExplosiveTriggerTests
{
	[TestMethod]
	public void GameItemComponentManager_RegistersAllExplosiveTriggerTypes()
	{
		var manager = new GameItemComponentManager();

		CollectionAssert.IsSubsetOf(
			new[] { "countdowndetonator", "clockdetonator", "signaldetonator", "pinpulldetonator" },
			manager.PrimaryTypes.ToList());
		CollectionAssert.IsSubsetOf(
			new[] { "CountdownDetonator", "ClockDetonator", "SignalDetonator", "PinPullDetonator" },
			manager.TypeHelpInfo.Select(x => x.Name).ToList());
	}

	[TestMethod]
	public void ExplosiveDeadlineScheduler_UsesInclusiveDeadlineComparison()
	{
		var deadline = new DateTime(638900000000000000L, DateTimeKind.Utc);

		Assert.IsFalse(ExplosiveDeadlineScheduler.IsDue(deadline, deadline.AddTicks(-1)));
		Assert.IsTrue(ExplosiveDeadlineScheduler.IsDue(deadline, deadline));
		Assert.IsTrue(ExplosiveDeadlineScheduler.IsDue(deadline, deadline.AddSeconds(10)));
	}

	[TestMethod]
	public void ExplosiveDeadlineScheduler_RejectsUnrepresentableDeadlines()
	{
		var now = DateTime.UtcNow;

		Assert.IsTrue(ExplosiveDeadlineScheduler.TryGetDeadline(now, TimeSpan.FromHours(24), out var deadline));
		Assert.AreEqual(now.AddHours(24), deadline);
		Assert.IsFalse(ExplosiveDeadlineScheduler.TryGetDeadline(now, TimeSpan.MaxValue, out _));
	}

	[TestMethod]
	public void ClockDetonatorScheduleEvaluator_DetonatesAfterSkippedWorldTicks()
	{
		var target = new MudInstant(1000L);

		Assert.IsFalse(ClockDetonatorScheduleEvaluator.IsDue(target, new MudInstant(999L)));
		Assert.IsTrue(ClockDetonatorScheduleEvaluator.IsDue(target, new MudInstant(1000L)));
		Assert.IsTrue(ClockDetonatorScheduleEvaluator.IsDue(target, new MudInstant(1500L)));
		Assert.IsTrue(ClockDetonatorScheduleEvaluator.CanRetainArmedTarget(1L, 2L, 1L, 2L));
		Assert.IsFalse(ClockDetonatorScheduleEvaluator.CanRetainArmedTarget(1L, 2L, 3L, 2L));
		Assert.IsFalse(ClockDetonatorScheduleEvaluator.CanRetainArmedTarget(1L, 2L, 1L, 4L));
	}

	[TestMethod]
	public void CountdownDetonator_ValidatesPlayerDelayAndPersistsArmedDeadline()
	{
		var gameworld = CreateGameworld();
		var detonatable = new Mock<IDetonatable>();
		var item = CreateExplosiveItem(gameworld.Object, detonatable.Object);
		var trigger = new CountdownDetonatorGameItemComponent(CreateCountdownPrototype(gameworld.Object), item.Object,
			true);
		var actor = CreateActor();

		Assert.IsFalse(trigger.CanArm(actor.Object, "00:00:00.500"));
		Assert.IsTrue(trigger.CanArm(actor.Object, "00:00:05"));
		Assert.IsTrue(trigger.Arm(actor.Object, "00:00:05", Mock.Of<IEmote>()));
		Assert.IsTrue(trigger.Armed);

		var definition = SaveDefinition(trigger);
		StringAssert.Contains(definition, "DetonationDeadlineUtcTicks");
		Assert.IsTrue(long.Parse(XElement.Parse(definition).Element("DetonationDeadlineUtcTicks")!.Value) > 0L);
	}

	[TestMethod]
	public void CountdownDetonator_ExpiredPersistedDeadlineDetonatesOnLogin()
	{
		var gameworld = CreateGameworld();
		var detonatable = new Mock<IDetonatable>();
		var item = CreateExplosiveItem(gameworld.Object, detonatable.Object);
		var component = new MudSharp.Models.GameItemComponent
		{
			Definition = new XElement("Definition",
				new XElement("DetonationDeadlineUtcTicks", DateTime.UtcNow.AddSeconds(-1).Ticks)).ToString()
		};
		var trigger = new CountdownDetonatorGameItemComponent(component,
			CreateCountdownPrototype(gameworld.Object), item.Object);

		trigger.Login();

		detonatable.Verify(x => x.Detonate(), Times.Once);
		Assert.IsFalse(trigger.Armed);
	}

	[TestMethod]
	public void SignalDetonator_EdgeMode_IgnoresInitialHighThenFiresOnRisingEdge()
	{
		var (trigger, detonatable, actor) = CreateSignalDetonator(ExplosiveSignalActivationMode.Edge, false);
		var source = Mock.Of<ISignalSource>();

		Assert.IsTrue(trigger.Arm(actor.Object, string.Empty, Mock.Of<IEmote>()));
		trigger.ReceiveSignal(new ComputerSignal(1.0, null, null), source);
		detonatable.Verify(x => x.Detonate(), Times.Never);

		trigger.ReceiveSignal(default, source);
		trigger.ReceiveSignal(new ComputerSignal(1.0, null, null), source);
		detonatable.Verify(x => x.Detonate(), Times.Once);
	}

	[TestMethod]
	public void SignalDetonator_LevelMode_FiresWheneverArmedInputIsActive()
	{
		var (trigger, detonatable, actor) = CreateSignalDetonator(ExplosiveSignalActivationMode.Level, false);

		Assert.IsTrue(trigger.Arm(actor.Object, string.Empty, Mock.Of<IEmote>()));
		trigger.ReceiveSignal(new ComputerSignal(1.0, null, null), Mock.Of<ISignalSource>());

		detonatable.Verify(x => x.Detonate(), Times.Once);
		Assert.IsFalse(trigger.Armed);
	}

	[TestMethod]
	public void SignalDetonator_PoweredEdgeMode_DoesNotReplaySignalReceivedWhileUnpowered()
	{
		var (trigger, detonatable, actor) = CreateSignalDetonator(ExplosiveSignalActivationMode.Edge, true);
		var source = Mock.Of<ISignalSource>();

		Assert.IsTrue(trigger.Arm(actor.Object, string.Empty, Mock.Of<IEmote>()));
		trigger.ReceiveSignal(new ComputerSignal(1.0, null, null), source);
		trigger.OnPowerCutIn();
		detonatable.Verify(x => x.Detonate(), Times.Never);

		trigger.ReceiveSignal(default, source);
		trigger.ReceiveSignal(new ComputerSignal(1.0, null, null), source);
		detonatable.Verify(x => x.Detonate(), Times.Once);
	}

	[TestMethod]
	public void SignalDetonator_DisarmPreventsLaterSignalDetonation()
	{
		var (trigger, detonatable, actor) = CreateSignalDetonator(ExplosiveSignalActivationMode.Level, false);

		Assert.IsTrue(trigger.Arm(actor.Object, string.Empty, Mock.Of<IEmote>()));
		Assert.IsTrue(trigger.Disarm(actor.Object, Mock.Of<IEmote>()));
		trigger.ReceiveSignal(new ComputerSignal(1.0, null, null), Mock.Of<ISignalSource>());

		detonatable.Verify(x => x.Detonate(), Times.Never);
	}

	[TestMethod]
	public void SignalDetonator_DisconnectedExplicitSourceCannotTriggerOrSubstituteAndReconnectsWhenAccessible()
	{
		var gameworld = CreateGameworld();
		var detonatable = new Mock<IDetonatable>();
		var item = CreateExplosiveItem(gameworld.Object, detonatable.Object);
		var sourceItem = CreateSignalSourceItem(gameworld.Object, 8101L, 9101L, out var source);
		var replacementItem = CreateSignalSourceItem(gameworld.Object, 8102L, 9102L, out var replacement);
		var connectedItems = new List<IGameItem> { sourceItem.Object };
		var sourceConnections = new List<IGameItem> { item.Object };
		item.Setup(x => x.AttachedAndConnectedItems).Returns(connectedItems);
		sourceItem.Setup(x => x.AttachedAndConnectedItems).Returns(sourceConnections);
		replacementItem.Setup(x => x.AttachedAndConnectedItems).Returns(Array.Empty<IGameItem>());
		gameworld.Setup(x => x.TryGetItem(8101L, true)).Returns(sourceItem.Object);
		gameworld.Setup(x => x.TryGetItem(8102L, true)).Returns(replacementItem.Object);

		var trigger = new SignalDetonatorGameItemComponent(
			CreateSignalPrototype(gameworld.Object, ExplosiveSignalActivationMode.Edge, false), item.Object, true);
		var binding = SignalComponentUtilities.CreateBinding(source.Object, "signal");
		Assert.AreEqual(8101L, binding.SourceItemId);
		Assert.AreEqual(9101L, binding.SourceComponentId);
		Assert.AreEqual("signal", binding.SourceEndpointKey);
		Assert.IsTrue(SignalComponentUtilities.ItemsAreSignalAccessible(item.Object, sourceItem.Object));
		Assert.AreSame(source.Object, SignalComponentUtilities.FindSignalSource(item.Object, binding));
		Assert.IsTrue(trigger.ConfigureSignalBinding(source.Object, "signal", out _));
		Assert.IsTrue(trigger.Arm(CreateActor().Object, string.Empty, Mock.Of<IEmote>()));
		Assert.IsNotNull(trigger.UpstreamSource);

		connectedItems.Clear();
		connectedItems.Add(replacementItem.Object);
		sourceConnections.Clear();
		replacementItem.Setup(x => x.AttachedAndConnectedItems).Returns(() => new[] { item.Object });
		source.Raise(x => x.SignalChanged += null, source.Object, new ComputerSignal(1.0, null, null));

		Assert.IsNull(trigger.UpstreamSource);
		replacement.Raise(x => x.SignalChanged += null, replacement.Object,
			new ComputerSignal(1.0, null, null));
		detonatable.Verify(x => x.Detonate(), Times.Never);

		connectedItems.Clear();
		connectedItems.Add(sourceItem.Object);
		sourceConnections.Add(item.Object);
		sourceItem.Raise(x => x.OnLocationChanged += null!, sourceItem.Object, Mock.Of<ICellExit>());

		Assert.AreSame(source.Object, trigger.UpstreamSource);
		source.Raise(x => x.SignalChanged += null, source.Object, new ComputerSignal(1.0, null, null));
		detonatable.Verify(x => x.Detonate(), Times.Once);
	}

	[TestMethod]
	public void ExplosiveTriggerDescriptions_ReportOperationalFacts()
	{
		var gameworld = CreateGameworld();
		var item = CreateExplosiveItem(gameworld.Object, Mock.Of<IDetonatable>());
		var voyeur = CreateActor();
		var radio = new RadioDetonatorGameItemComponent(CreateRadioPrototype(gameworld.Object), item.Object, true)
		{
			SwitchedOn = true
		};
		var countdown = new CountdownDetonatorGameItemComponent(CreateCountdownPrototype(gameworld.Object), item.Object,
			true);
		var signal = new SignalDetonatorGameItemComponent(
			CreateSignalPrototype(gameworld.Object, ExplosiveSignalActivationMode.Edge, false), item.Object, true);

		var radioFull = radio.Decorate(voyeur.Object, string.Empty, "a bomb", DescriptionType.Full, true,
			PerceiveIgnoreFlags.None);
		StringAssert.Contains(radioFull, "armed");
		StringAssert.Contains(radioFull, "unpowered");
		Assert.IsFalse(radioFull.Contains("currently disarmed", StringComparison.Ordinal));
		StringAssert.Contains(countdown.Decorate(voyeur.Object, string.Empty, "a bomb", DescriptionType.Evaluate,
			true, PerceiveIgnoreFlags.None), "permitted range");
		var signalEvaluation = signal.Decorate(voyeur.Object, string.Empty, "a bomb", DescriptionType.Evaluate, true,
			PerceiveIgnoreFlags.None);
		StringAssert.Contains(signalEvaluation, "at or above");
		StringAssert.Contains(signalEvaluation, "does not require electrical power");
		StringAssert.Contains(signalEvaluation, "can be disarmed");
	}

	[TestMethod]
	public void TriggerPrototypes_RejectUnrepresentableRealTimeDelays()
	{
		var gameworld = CreateGameworld();
		var unsafeSeconds = TimeSpan.FromDays(4_000_000).TotalSeconds;
		var countdown = CreatePrototype<CountdownDetonatorGameItemComponentProto>(gameworld.Object, 7201L,
			new XElement("Definition",
				new XElement("DefaultDelaySeconds", 10.0),
				new XElement("MinimumDelaySeconds", 1.0),
				new XElement("MaximumDelaySeconds", unsafeSeconds),
				new XElement("PlayersCanSetDelay", true),
				new XElement("CanBeDisarmed", true),
				new XElement("ArmEmote", new XCData("@ arm|arms $1")),
				new XElement("DisarmEmote", new XCData("@ disarm|disarms $1"))));
		var pinPull = CreatePrototype<PinPullDetonatorGameItemComponentProto>(gameworld.Object, 7202L,
			new XElement("Definition",
				new XElement("DelaySeconds", unsafeSeconds),
				new XElement("PullPinEmote", new XCData("@ pull|pulls the pin from $1"))));

		Assert.IsFalse(countdown.CanSubmit());
		Assert.IsFalse(pinPull.CanSubmit());
		StringAssert.Contains(countdown.WhyCannotSubmit(), "too long");
		StringAssert.Contains(pinPull.WhyCannotSubmit(), "too long");
	}

	[TestMethod]
	public void TriggerPrototypes_RequireDetonatableSibling()
	{
		var gameworld = CreateGameworld();
		var prototypes = new IGameItemComponentPrototypeRequirementProvider[]
		{
			CreateCountdownPrototype(gameworld.Object),
			CreatePinPullPrototype(gameworld.Object),
			CreateSignalPrototype(gameworld.Object, ExplosiveSignalActivationMode.Edge, false),
			CreateRadioPrototype(gameworld.Object)
		};

		foreach (var prototype in prototypes)
		{
			CollectionAssert.AreEqual(new[] { typeof(IDetonatable) },
				prototype.RequiredSiblingComponents.Select(x => x.Capability).ToArray());
		}
	}

	[TestMethod]
	public void TriggerPrototypeMarkers_ClassifyArmableAndPinPullRoles()
	{
		Assert.IsTrue(typeof(IArmableExplosiveTriggerPrototype)
			.IsAssignableFrom(typeof(CountdownDetonatorGameItemComponentProto)));
		Assert.IsTrue(typeof(IArmableExplosiveTriggerPrototype)
			.IsAssignableFrom(typeof(ClockDetonatorGameItemComponentProto)));
		Assert.IsTrue(typeof(IArmableExplosiveTriggerPrototype)
			.IsAssignableFrom(typeof(SignalDetonatorGameItemComponentProto)));
		Assert.IsTrue(typeof(IArmableExplosiveTriggerPrototype)
			.IsAssignableFrom(typeof(RadioDetonatorGameItemComponentProto)));
		Assert.IsTrue(typeof(IPinPullExplosiveTriggerPrototype)
			.IsAssignableFrom(typeof(PinPullDetonatorGameItemComponentProto)));
		Assert.IsTrue(typeof(IGameItemComponentPrototypeRequirementProvider)
			.IsAssignableFrom(typeof(ClockDetonatorGameItemComponentProto)));
	}

	private static (SignalDetonatorGameItemComponent Trigger, Mock<IDetonatable> Detonatable,
		Mock<ICharacter> Actor) CreateSignalDetonator(ExplosiveSignalActivationMode mode, bool requiresPower)
	{
		var gameworld = CreateGameworld();
		var detonatable = new Mock<IDetonatable>();
		var item = CreateExplosiveItem(gameworld.Object, detonatable.Object);
		var trigger = new SignalDetonatorGameItemComponent(
			CreateSignalPrototype(gameworld.Object, mode, requiresPower), item.Object, true);
		return (trigger, detonatable, CreateActor());
	}

	private static Mock<IFuturemud> CreateGameworld()
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<IHeartbeatManager>());
		gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		return gameworld;
	}

	private static Mock<IGameItem> CreateExplosiveItem(IFuturemud gameworld, IDetonatable detonatable)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(100L);
		item.SetupGet(x => x.Name).Returns("test bomb");
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		item.Setup(x => x.IsItemType<IDetonatable>()).Returns(true);
		item.Setup(x => x.GetItemType<IDetonatable>()).Returns(detonatable);
		item.Setup(x => x.GetItemTypes<IProducePower>()).Returns(Array.Empty<IProducePower>());
		item.SetupGet(x => x.AttachedAndConnectedItems).Returns(Array.Empty<IGameItem>());
		item.SetupGet(x => x.TrueLocations).Returns(Array.Empty<ICell>());
		item.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("a test bomb");
		return item;
	}

	private static Mock<IGameItem> CreateSignalSourceItem(IFuturemud gameworld, long itemId, long componentId,
		out Mock<ISignalSourceComponent> source)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(itemId);
		item.SetupGet(x => x.Name).Returns($"source {itemId}");
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		item.SetupGet(x => x.TrueLocations).Returns(Array.Empty<ICell>());
		var localSource = new Mock<ISignalSourceComponent>();
		localSource.SetupGet(x => x.Id).Returns(componentId);
		localSource.SetupGet(x => x.Parent).Returns(item.Object);
		localSource.SetupGet(x => x.LocalSignalSourceIdentifier).Returns(42L);
		localSource.SetupGet(x => x.CurrentSignal).Returns(default(ComputerSignal));
		localSource.As<IFrameworkItem>().SetupGet(x => x.Name).Returns("controller");
		localSource.As<ISignalSource>().SetupGet(x => x.Name).Returns("controller");
		localSource.As<ISignalSource>().SetupGet(x => x.EndpointKey).Returns("signal");
		item.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns(() => new[] { localSource.Object });
		source = localSource;
		return item;
	}

	private static Mock<ICharacter> CreateActor()
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		return actor;
	}

	private static string SaveDefinition(IGameItemComponent component)
	{
		return (string)component.GetType()
			.GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(component, null)!;
	}

	private static CountdownDetonatorGameItemComponentProto CreateCountdownPrototype(IFuturemud gameworld)
	{
		return CreatePrototype<CountdownDetonatorGameItemComponentProto>(gameworld, 7001L,
			new XElement("Definition",
				new XElement("DefaultDelaySeconds", 10.0),
				new XElement("MinimumDelaySeconds", 1.0),
				new XElement("MaximumDelaySeconds", 86400.0),
				new XElement("PlayersCanSetDelay", true),
				new XElement("CanBeDisarmed", true),
				new XElement("ArmEmote", new XCData("@ arm|arms $1")),
				new XElement("DisarmEmote", new XCData("@ disarm|disarms $1"))));
	}

	private static PinPullDetonatorGameItemComponentProto CreatePinPullPrototype(IFuturemud gameworld)
	{
		return CreatePrototype<PinPullDetonatorGameItemComponentProto>(gameworld, 7002L,
			new XElement("Definition",
				new XElement("DelaySeconds", 5.0),
				new XElement("PullPinEmote", new XCData("@ pull|pulls the pin from $1"))));
	}

	private static SignalDetonatorGameItemComponentProto CreateSignalPrototype(IFuturemud gameworld,
		ExplosiveSignalActivationMode mode, bool requiresPower)
	{
		return CreatePrototype<SignalDetonatorGameItemComponentProto>(gameworld, 7003L,
			new XElement("Definition",
				new XElement("SourceComponentId", 42L),
				new XElement("SourceComponentName", new XCData("controller")),
				new XElement("SourceEndpointKey", new XCData("signal")),
				new XElement("ActivationThreshold", 0.5),
				new XElement("ActiveWhenAboveThreshold", true),
				new XElement("ActivationMode", mode),
				new XElement("RequiresPower", requiresPower),
				new XElement("PowerConsumptionInWatts", 0.1),
				new XElement("CanBeDisarmed", true),
				new XElement("ArmEmote", new XCData("@ arm|arms $1")),
				new XElement("DisarmEmote", new XCData("@ disarm|disarms $1"))));
	}

	private static RadioDetonatorGameItemComponentProto CreateRadioPrototype(IFuturemud gameworld)
	{
		return CreatePrototype<RadioDetonatorGameItemComponentProto>(gameworld, 7004L,
			new XElement("Definition",
				new XElement("PowerConsumptionInWatts", 0.1),
				new XElement("OnPowerOnEmote", new XCData("@ light|lights")),
				new XElement("OnPowerOffEmote", new XCData("@ darken|darkens"))));
	}

	private static T CreatePrototype<T>(IFuturemud gameworld, long id, XElement definition)
		where T : IGameItemComponentProto
	{
		return (T)typeof(T)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = id,
					Name = typeof(T).Name,
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}
}

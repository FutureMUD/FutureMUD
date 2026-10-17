#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PeekInspectionTests
{
	private sealed class LearningProbe(MudSharp.Models.Check model, IFuturemud world) : StandardCheck(model, world)
	{
		public CheckOutcome Finish(ICharacter actor) => HandleStandardCheck(actor, null!, Outcome.Pass, Difficulty.Normal, null!);
	}

	[TestMethod]
	public void NativeLearningPolicy_AttemptImprovesOnceAndManyRevealChecksNeverTrainOrBranch()
	{
		var f = new Fixture();
		var expressions = new All<ITraitExpression>();
		f.World.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var trait = new Mock<ITrait>(); trait.SetupGet(x => x.Definition).Returns(f.Skill.Object);
		f.Actor.Setup(x => x.GetTrait(f.Skill.Object)).Returns(trait.Object);
		foreach (var improve in new[] { true, false })
		{
			var formula = improve ? f.Attempt.Object.TargetNumberExpression : f.Reveal.Object.TargetNumberExpression;
			Mock.Get(formula).SetupGet(x => x.Id).Returns(improve ? 50 : 51);
			expressions.Add(formula);
			var template = new MudSharp.Models.CheckTemplate
			{
				ImproveTraits = improve, FailIfTraitMissingMode = 1, CanBranchIfTraitMissing = false
			};
			foreach (var difficulty in Enum.GetValues<Difficulty>())
				template.CheckTemplateDifficulties.Add(new() { Difficulty = (int)difficulty });
			var check = new LearningProbe(new MudSharp.Models.Check
			{
				Type = (int)(improve ? CheckType.PeekCheck : CheckType.PeekRevealCheck), TraitExpressionId = formula.Id,
				CheckTemplate = template, MaximumDifficultyForImprovement = 10
			}, f.World.Object);
			for (var i = 0; i < (improve ? 1 : 5); i++) Assert.AreEqual(0, check.Finish(f.Actor.Object).AcquiredTraits.Count());
		}
		trait.Verify(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), Difficulty.Normal, TraitUseType.Practical,
			It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Once);
		Assert.IsFalse(f.Actor.Invocations.Any(x => x.Method.Name == "AddTrait"));
	}

	private sealed class Fixture
	{
		public Mock<IFuturemud> World = new();
		public Mock<IRoom> Room = new();
		public Mock<ICharacter> Actor = new();
		public Mock<ICharacter> Target = new();
		public Mock<ICharacter> Witness = new();
		public Mock<IBody> Body = new();
		public Mock<ITraitDefinition> Skill = new();
		public Mock<ICheck> Attempt = new();
		public Mock<ICheck> Reveal = new();
		public Mock<ICheck> Spot = new();
		public Outcome AttemptOutcome = Outcome.Pass, RevealOutcome = Outcome.Pass, SpotOutcome = Outcome.MajorFail;
		public int Attempts, Reveals;
		public Action? OnAttempt, OnReveal;
		public Action<IPerceivableHaveTraits>? OnSpot;
		public List<IGameItem> Worn = [], Held = [], Exposed = [];
		public List<IEffect> Effects = [];
		public List<TimeSpan> ActionDelays = [];
		public Fixture()
		{
			Actor.SetupGet(x => x.Effects).Returns(Effects);
			Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
				.Callback<IEffect, TimeSpan>((effect, delay) =>
				{
					Effects.Add(effect); ActionDelays.Add(delay); effect.InitialEffect();
				});
			Actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
				.Callback<IEffect, bool>((effect, fire) => { Effects.Remove(effect); if (fire) effect.RemovalEffect(); });
			Actor.Setup(x => x.TargetActor("target", PerceiveIgnoreFlags.None)).Returns(Target.Object);
			Skill.SetupGet(x => x.Id).Returns(10);
			Skill.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
			Skill.SetupGet(x => x.OwnerScope).Returns(TraitOwnerScope.Character);
			Actor.Setup(x => x.HasTrait(Skill.Object)).Returns(true);
			Actor.Setup(x => x.TraitRawValue(Skill.Object)).Returns(50);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.Setup(x => x.ColocatedWith(Target.Object)).Returns(true);
			Actor.Setup(x => x.IlluminationSightDifficulty()).Returns(Difficulty.Normal);
			Actor.Setup(x => x.IlluminationSightDifficulty(It.IsAny<IRoom>())).Returns(Difficulty.Normal);
			Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			foreach (var ch in new[] { Actor, Target, Witness })
			{
				ch.SetupGet(x => x.State).Returns(CharacterState.Awake);
				ch.SetupGet(x => x.Location).Returns(Room.Object);
				ch.SetupGet(x => x.Gameworld).Returns(World.Object);
				ch.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
				ch.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
				ch.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			}
			var actorBody = new Mock<IBody>();
			actorBody.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Body.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Actor.SetupGet(x => x.Body).Returns(actorBody.Object);
			Target.SetupGet(x => x.Body).Returns(Body.Object);
			Body.SetupGet(x => x.WornItems).Returns(() => Worn);
			Body.SetupGet(x => x.HeldItems).Returns(() => Held);
			Body.SetupGet(x => x.WieldedItems).Returns(Array.Empty<IGameItem>());
			Body.SetupGet(x => x.ExternalItemsForOtherActors).Returns(() => Exposed);
			Room.Setup(x => x.LayerCharacters(RoomLayer.GroundLevel)).Returns([Actor.Object, Target.Object, Witness.Object]);
			World.Setup(x => x.GetCheck(CheckType.PeekCheck)).Returns(Attempt.Object);
			World.Setup(x => x.GetCheck(CheckType.PeekRevealCheck)).Returns(Reveal.Object);
			World.Setup(x => x.GetCheck(CheckType.SpotStealthCheck)).Returns(Spot.Object);
			World.Setup(x => x.GetStaticConfiguration("PeekCoveredItemDifficulty")).Returns("6");
			World.Setup(x => x.GetStaticConfiguration("PeekContainerItemDifficulty")).Returns("5");
			World.Setup(x => x.GetStaticConfiguration("PeekCarriedItemDifficulty")).Returns("5");
			foreach (var (check, improve) in new[] { (Attempt, true), (Reveal, false) })
			{
				var formula = new Mock<ITraitExpression>();
				formula.SetupGet(x => x.Parameters).Returns(new Dictionary<string, TraitExpressionParameter>
				{
					["peek"] = new() { Trait = Skill.Object, CanImprove = improve, CanBranch = false }
				});
				check.SetupGet(x => x.TargetNumberExpression).Returns(formula.Object);
				check.SetupGet(x => x.ImproveTraits).Returns(improve);
			}
			SetupCheck(Attempt, () => { Attempts++; OnAttempt?.Invoke(); return AttemptOutcome; });
			SetupCheck(Reveal, () => { Reveals++; OnReveal?.Invoke(); return RevealOutcome; });
			SetupCheck(Spot, () => SpotOutcome, user => OnSpot?.Invoke(user));
		}
		private static void SetupCheck(Mock<ICheck> check, Func<Outcome> outcome, Action<IPerceivableHaveTraits>? onCheck = null) => check
			.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(),
				It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Callback<IPerceivableHaveTraits, Difficulty, IPerceivable, IUseTrait, double, TraitUseType, (string, object)[]>(
				(user, _, _, _, _, _, _) => onCheck?.Invoke(user))
			.Returns(() => CheckOutcome.SimpleOutcome(CheckType.PeekCheck, outcome()));
		public Mock<IGameItem> Item(bool exposed = false, bool worn = true)
		{
			var item = new Mock<IGameItem>();
			item.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(other, item.Object));
			item.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			item.SetupGet(x => x.Gameworld).Returns(World.Object);
			item.SetupGet(x => x.Location).Returns(Room.Object);
			if (worn) Worn.Add(item.Object);
			if (exposed) Exposed.Add(item.Object);
			return item;
		}
		public (Mock<IGameItem> Item, Mock<IContainer> Container, Mock<IOpenable> Openable) Container(bool open)
		{
			var item = Item(true);
			var container = new Mock<IContainer>();
			var openable = new Mock<IOpenable>();
			openable.SetupGet(x => x.IsOpen).Returns(open);
			item.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
			item.Setup(x => x.GetItemType<IOpenable>()).Returns(openable.Object);
			return (item, container, openable);
		}
		public PeekInspectionResult Inspect(IGameItem? container = null) => PeekInspection.Inspect(Actor.Object, Target.Object, container);
		public void Command(string command = "peek target") => typeof(StealthModule)
			.GetMethod("Peek", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [Actor.Object, command]);
		public static int Alerts(Mock<ICharacter> ch) => Mock.Get(ch.Object.OutputHandler).Invocations.Count(x =>
			x.Method.Name == "Send" && x.Arguments.OfType<string>().Any(s => s.Contains("trying to peek")));
	}

	[TestMethod]
	public void UnnoticedFailure_CanRevealLittleAndExposedEquipmentRemainsVisible()
	{
		var f = new Fixture { AttemptOutcome = Outcome.MinorFail, RevealOutcome = Outcome.Fail };
		var exposed = f.Item(true); f.Item(); f.Item();
		var result = f.Inspect();
		Assert.IsFalse(result.Interrupted);
		CollectionAssert.AreEquivalent(new[] { exposed.Object }, result.Items.ToArray());
		Assert.AreEqual(1, f.Attempts); Assert.AreEqual(2, f.Reveals);
		Assert.AreEqual(0, Fixture.Alerts(f.Target) + Fixture.Alerts(f.Witness));
	}

	[TestMethod]
	public void NoticedSuccess_RevealsCoveredItemsAndAlertsOnlyActualNoticers()
	{
		var f = new Fixture { AttemptOutcome = Outcome.Pass, SpotOutcome = Outcome.MajorPass };
		f.Target.Setup(x => x.CanSee(f.Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		var covered = f.Item();
		var result = f.Inspect();
		Assert.IsFalse(result.Interrupted);
		CollectionAssert.AreEqual(new[] { covered.Object }, result.Items.ToArray());
		Assert.AreEqual(0, Fixture.Alerts(f.Target)); Assert.AreEqual(1, Fixture.Alerts(f.Witness));
		Assert.IsFalse(f.World.Invocations.Any(x => x.Method.Name.Contains("Crime")));
		Assert.IsFalse(f.Body.Invocations.Any(x => new[] { "Get", "Take", "Put", "Drop", "Open" }.Contains(x.Method.Name)));
		Assert.IsFalse(Mock.Get(f.Actor.Object.Body).Invocations.Any(x => new[] { "Get", "Take", "Put", "Drop", "Open" }.Contains(x.Method.Name)));
	}

	[TestMethod]
	public void PerItemChecks_CanRevealOnlyPartOfCoveredInventory()
	{
		var f = new Fixture(); var first = f.Item(); f.Item();
		f.OnReveal = () => f.RevealOutcome = f.Reveals == 1 ? Outcome.Pass : Outcome.Fail;
		CollectionAssert.AreEqual(new[] { first.Object }, f.Inspect().Items.ToArray());
		Assert.AreEqual(1, f.Attempts); Assert.AreEqual(2, f.Reveals);
	}

	[TestMethod]
	public void BotchOrActiveInterruption_DoesNotEnumerateOrRevealInventory()
	{
		foreach (var botch in new[] { true, false })
		{
			var f = new Fixture { AttemptOutcome = botch ? Outcome.MajorFail : Outcome.Fail, SpotOutcome = Outcome.MajorPass };
			f.Item();
			Assert.IsTrue(f.Inspect().Interrupted); Assert.AreEqual(0, f.Reveals);
			f.Body.VerifyGet(x => x.WornItems, Times.Never);
			Assert.AreEqual(1, Fixture.Alerts(f.Target));
		}
	}

	[TestMethod]
	public void HelplessTarget_StillRequiresSkillButCannotActivelyInterrupt()
	{
		var f = new Fixture { AttemptOutcome = Outcome.Fail, SpotOutcome = Outcome.MajorPass };
		f.Target.SetupGet(x => x.IsHelpless).Returns(true); f.Item();
		Assert.IsFalse(f.Inspect().Interrupted);
		f.Actor.Setup(x => x.HasTrait(f.Skill.Object)).Returns(false);
		Assert.IsTrue(f.Inspect().Interrupted); Assert.AreEqual(1, f.Attempts);
		f.Actor.Setup(x => x.HasTrait(f.Skill.Object)).Returns(true);
		f.Target.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		Assert.IsFalse(f.Inspect().Interrupted);
	}

	[TestMethod]
	public void ClosedHiddenAndInaccessibleContainers_AreRejectedBeforeContentsAreRead()
	{
		var f = new Fixture(); var (item, container, openable) = f.Container(false);
		Assert.IsTrue(f.Inspect(item.Object).Interrupted);
		openable.SetupGet(x => x.IsOpen).Returns(true);
		f.Actor.Setup(x => x.CanSee(item.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Assert.IsTrue(f.Inspect(item.Object).Interrupted);
		f.Actor.Setup(x => x.CanSee(item.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		item.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2));
		Assert.IsTrue(f.Inspect(item.Object).Interrupted);
		container.VerifyGet(x => x.Contents, Times.Never); Assert.AreEqual(0, f.Attempts);
	}

	[TestMethod]
	public void OpenContainer_RevealsOnlyEligibleImmediateContentsWithoutTakePermission()
	{
		var f = new Fixture(); var (item, container, _) = f.Container(true);
		var visible = f.Item(worn: false); var invisible = f.Item(worn: false);
		var nested = new Mock<IContainer>(); visible.Setup(x => x.GetItemType<IContainer>()).Returns(nested.Object);
		container.SetupGet(x => x.Contents).Returns([visible.Object, invisible.Object]);
		f.Actor.Setup(x => x.CanSee(invisible.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		CollectionAssert.AreEqual(new[] { visible.Object }, f.Inspect(item.Object).Items.ToArray());
		nested.VerifyGet(x => x.Contents, Times.Never); Assert.AreEqual(1, f.Reveals);
		f.Target.Verify(x => x.WillingToPermitInventoryManipulation(f.Actor.Object), Times.Never);
		f.Body.Verify(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
		Mock.Get(f.Actor.Object.Body).Verify(x => x.CanGet(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void BroadPeek_DoesNotReadContainersOrProbeInvisibleItems()
	{
		var f = new Fixture(); var (_, container, _) = f.Container(true); var invisible = f.Item();
		f.Actor.Setup(x => x.CanSee(invisible.Object, PerceiveIgnoreFlags.IgnoreSpotting)).Returns(false);
		Assert.AreEqual(1, f.Inspect().Items.Count);
		Assert.AreEqual(0, f.Reveals); container.VerifyGet(x => x.Contents, Times.Never);
	}

	[TestMethod]
	public void LayerPlaneDarknessAndSelfGates_RunBeforeAnySkillCheck()
	{
		var f = new Fixture();
		Assert.IsTrue(PeekInspection.Inspect(f.Actor.Object, f.Actor.Object).Interrupted);
		f.Target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir);
		Assert.IsTrue(f.Inspect().Interrupted);
		f.Target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		f.Actor.Setup(x => x.IlluminationSightDifficulty()).Returns(Difficulty.Impossible);
		Assert.IsTrue(f.Inspect().Interrupted);
		f.Actor.Setup(x => x.IlluminationSightDifficulty()).Returns(Difficulty.Normal);
		f.Body.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2));
		Assert.IsTrue(f.Inspect().Interrupted); Assert.AreEqual(0, f.Attempts);
	}

	[TestMethod]
	public void LearningCallbacks_CloseContainerOrMoveTargetBeforeDisclosure()
	{
		var f = new Fixture(); var (item, container, openable) = f.Container(true);
		container.SetupGet(x => x.Contents).Returns([f.Item(worn: false).Object]);
		f.OnReveal = () => openable.SetupGet(x => x.IsOpen).Returns(false);
		Assert.IsTrue(f.Inspect(item.Object).Interrupted);
		openable.SetupGet(x => x.IsOpen).Returns(true);
		f.OnAttempt = () => f.Target.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		Assert.IsTrue(f.Inspect(item.Object).Interrupted);
	}

	[TestMethod]
	public void UnsafeRevealTemplate_DisablesActionRatherThanTrainingPerItem()
	{
		var f = new Fixture(); f.Reveal.SetupGet(x => x.ImproveTraits).Returns(true);
		Assert.IsTrue(f.Inspect().Interrupted); Assert.AreEqual(0, f.Attempts);
	}

	[TestMethod]
	public void BlindWitnessAndDistantTarget_CannotNoticeOrBeInspected()
	{
		var f = new Fixture { SpotOutcome = Outcome.MajorPass };
		f.Target.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
		f.Witness.Setup(x => x.IlluminationSightDifficulty()).Returns(Difficulty.Impossible);
		f.Item();
		Assert.IsFalse(f.Inspect().Interrupted); Assert.AreEqual(0, Fixture.Alerts(f.Witness));
		f.Actor.Setup(x => x.ColocatedWith(f.Target.Object)).Returns(false);
		Assert.IsTrue(f.Inspect().Interrupted); Assert.AreEqual(1, f.Attempts);
	}

	[TestMethod]
	public void CheckCallback_CoversPreviouslyExposedItem_NoStaleDisclosure()
	{
		var f = new Fixture(); var initiallyExposed = f.Item(true); var covered = f.Item();
		f.OnReveal = () => f.Exposed.Clear();
		CollectionAssert.AreEqual(new[] { covered.Object }, f.Inspect().Items.ToArray());
		Assert.AreEqual(1, f.Reveals);
	}

	[TestMethod]
	public void Command_InvalidContainerFailuresAndNoticeAlerts_DoNotNameConcealedBelongings()
	{
		var messages = new List<string>();
		foreach (var reason in new[] { "absent", "closed", "hidden", "plane" })
		{
			var f = new Fixture();
			f.Actor.Setup(x => x.TargetActor("target", PerceiveIgnoreFlags.None)).Returns(f.Target.Object);
			var (item, container, openable) = f.Container(reason != "closed");
			if (reason == "absent") f.Exposed.Clear();
			if (reason == "hidden") f.Actor.Setup(x => x.CanSee(item.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
			if (reason == "plane") item.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(2));
			typeof(StealthModule).GetMethod("Peek", BindingFlags.Static | BindingFlags.NonPublic)!
				.Invoke(null, [f.Actor.Object, "peek target concealed-marker"]);
			messages.Add(Mock.Get(f.Actor.Object.OutputHandler).Invocations.Single(x => x.Method.Name == "Send").Arguments.OfType<string>().Single());
			container.VerifyGet(x => x.Contents, Times.Never);
			Assert.IsFalse(item.Invocations.Any(x => x.Method.Name == "HowSeen"));
		}
		Assert.AreEqual(1, messages.Distinct().Count());
		Assert.IsFalse(messages.Any(x => x.Contains("concealed-marker")));
		var noticed = new Fixture { AttemptOutcome = Outcome.Pass, SpotOutcome = Outcome.MajorPass, RevealOutcome = Outcome.Fail };
		var concealed = noticed.Item();
		noticed.Actor.Setup(x => x.TargetActor("target", PerceiveIgnoreFlags.None)).Returns(noticed.Target.Object);
		typeof(StealthModule).GetMethod("Peek", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [noticed.Actor.Object, "peek target"]);
		noticed.Effects.OfType<PeekInspectionAction>().Single().ExpireEffect();
		Assert.AreEqual(1, Fixture.Alerts(noticed.Target)); Assert.AreEqual(1, Fixture.Alerts(noticed.Witness));
		Assert.IsFalse(concealed.Invocations.Any(x => x.Method.Name == "HowSeen"));
	}

	[TestMethod]
	public void ObserverCallback_MovesAnotherWitness_NoRemoteDetectionOrAlert()
	{
		var f = new Fixture { SpotOutcome = Outcome.MajorPass }; f.Item();
		f.OnSpot = user =>
		{
			if (ReferenceEquals(user, f.Target.Object)) f.Witness.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		};
		Assert.IsFalse(f.Inspect().Interrupted);
		Assert.AreEqual(1, Fixture.Alerts(f.Target)); Assert.AreEqual(0, Fixture.Alerts(f.Witness));
		Assert.AreEqual(1, f.Spot.Invocations.Count(x => x.Method.Name == "Check"));
	}

	[TestMethod]
	public void NonUnaryCarriedInventory_RequiresRevealAndCannotExposeHiddenContainers()
	{
		var f = new Fixture { RevealOutcome = Outcome.Fail };
		var item = f.Item(true, worn: false); f.Held.Add(item.Object);
		var grab = new Mock<IGrab>(); grab.Setup(x => x.ItemsVisible()).Returns(false);
		f.Body.Setup(x => x.HoldOrWieldLocFor(item.Object)).Returns(grab.Object);
		Assert.AreEqual(0, f.Inspect().Items.Count); Assert.AreEqual(1, f.Reveals);
		f.RevealOutcome = Outcome.Pass;
		CollectionAssert.AreEqual(new[] { item.Object }, f.Inspect().Items.ToArray());
		Assert.IsTrue(f.Reveal.Invocations.Where(x => x.Method.Name == "Check").All(x => (Difficulty)x.Arguments[1] == Difficulty.Normal));
		var (bag, container, _) = f.Container(true);
		f.Worn.Remove(bag.Object); f.Held.Add(bag.Object);
		f.Body.Setup(x => x.HoldOrWieldLocFor(bag.Object)).Returns(grab.Object);
		Assert.IsTrue(f.Inspect(bag.Object).Interrupted);
		container.VerifyGet(x => x.Contents, Times.Never);
	}

	[TestMethod]
	public void Command_DelaysEachAttemptFiveSecondsWithoutPublicDisclosureOrAdditionalCooldown()
	{
		var f = new Fixture(); f.Item(); f.Command();
		Assert.AreEqual(TimeSpan.FromSeconds(5), f.ActionDelays.Single());
		Assert.AreEqual(0, f.Attempts); Assert.AreEqual(0, f.Reveals);
		Assert.AreEqual(0, Fixture.Alerts(f.Target) + Fixture.Alerts(f.Witness));
		f.Body.VerifyGet(x => x.WornItems, Times.Never);
		var action = f.Effects.OfType<PeekInspectionAction>().Single();
		Assert.IsTrue(action.IsBlockingEffect("general"));
		Assert.AreEqual("general", typeof(StealthModule).GetMethod("Peek", BindingFlags.Static | BindingFlags.NonPublic)!
			.GetCustomAttribute<DelayBlock>()!.DelayType);
		Assert.IsTrue(action.IsBlockingEffect("movement"));
		Assert.IsTrue(action.IsBlockingEffect("combat"));
		Assert.IsFalse(action.SuffixApplies()); Assert.IsFalse(action.SavingEffect);
		action.ExpireEffect();
		Assert.AreEqual(1, f.Attempts); Assert.AreEqual(1, f.Reveals); Assert.AreEqual(0, f.Effects.Count);
		f.Command();
		Assert.AreEqual(TimeSpan.FromSeconds(5), f.ActionDelays[1]); Assert.AreEqual(1, f.Attempts);
		f.Effects.OfType<PeekInspectionAction>().Single().ExpireEffect();
		Assert.AreEqual(2, f.Attempts); Assert.AreEqual(0, f.Effects.Count);
	}

	[TestMethod]
	public void Command_CancelledOrQuitAttempt_CannotResolveFromStaleExpiry()
	{
		foreach (var quit in new[] { false, true })
		{
			var f = new Fixture(); f.Command();
			var action = f.Effects.OfType<PeekInspectionAction>().Single();
			Assert.IsTrue(action is IRemoveOnCombatStart);
			Assert.IsTrue(((IRemoveOnMovementEffect)action).ShouldRemove());
			Assert.IsTrue(action.ShouldRemove(CharacterState.Unconscious));
			if (quit) f.Actor.Raise(x => x.OnQuit += null, f.Actor.Object);
			else f.Actor.Object.RemoveEffect(action, true);
			action.ExpireEffect();
			Assert.AreEqual(0, f.Attempts); Assert.AreEqual(0, f.Effects.Count);
			Assert.AreEqual(0, Fixture.Alerts(f.Target) + Fixture.Alerts(f.Witness));
		}
	}

	[TestMethod]
	public void Command_DelayRevalidatesSkillTargetAndContainerBeforeAnyRollOrDisclosure()
	{
		foreach (var reason in new[] { "moved", "closed", "skill", "state", "combat" })
		{
			var f = new Fixture(); var (bag, contents, openable) = f.Container(true);
			f.Command("peek target #1");
			if (reason == "moved") f.Target.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
			if (reason == "skill") f.Actor.Setup(x => x.HasTrait(f.Skill.Object)).Returns(false);
			if (reason == "state") f.Actor.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
			if (reason == "closed") openable.SetupGet(x => x.IsOpen).Returns(false);
			if (reason == "combat") f.Actor.SetupGet(x => x.Combat).Returns(Mock.Of<MudSharp.Combat.ICombat>());
			f.Effects.OfType<PeekInspectionAction>().Single().ExpireEffect();
			Assert.AreEqual(0, f.Attempts); Assert.AreEqual(0, f.Reveals); Assert.AreEqual(0, f.Effects.Count);
			contents.VerifyGet(x => x.Contents, Times.Never);
		}
	}
}

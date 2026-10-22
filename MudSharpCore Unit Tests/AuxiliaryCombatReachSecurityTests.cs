#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Character.Heritage;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuxiliaryCombatReachSecurityTests
{
	[DataTestMethod]
	[DataRow("distant")]
	[DataRow("layer")]
	[DataRow("ranged")]
	[DataRow("other-combat")]
	[DataRow("unrelated-melee")]
	public void AuxiliaryAttack_UnreachableTarget_RejectsQueueAndResolution(string state)
	{
		var fixture = new Fixture();
		if (state == "distant") fixture.Actor.Setup(x => x.ColocatedWith(fixture.Target.Object)).Returns(false);
		if (state == "layer") fixture.Target.SetupGet(x => x.RoomLayer).Returns(RoomLayer.HighInAir);
		if (state == "ranged") fixture.Actor.SetupGet(x => x.MeleeRange).Returns(false);
		if (state == "other-combat") fixture.Target.SetupGet(x => x.Combat).Returns(Mock.Of<ICombat>());
		if (state == "unrelated-melee") fixture.Actor.SetupGet(x => x.CombatTarget).Returns(Mock.Of<ICharacter>());

		var result = ManualCombatCommandResolver.TryResolve(fixture.Actor.Object, fixture.Command.Object, fixture.Target.Object);

		Assert.IsFalse(result.Success);
		Assert.AreSame(CombatMoveResult.Irrelevant, fixture.Move.ResolveMove(null!));
		fixture.Actor.Verify(x => x.CanSpendStamina(It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void AuxiliaryAttack_UnseenEngagedTarget_RejectsManualSelectionButRemainsInPhysicalReach()
	{
		var fixture = new Fixture();
		fixture.Actor.Setup(x => x.CanSee(fixture.Target.Object)).Returns(false);

		Assert.IsFalse(ManualCombatCommandResolver.TryResolve(fixture.Actor.Object, fixture.Command.Object, fixture.Target.Object).Success);
		Assert.IsTrue(AuxiliaryMove.CanReachTarget(fixture.Actor.Object, fixture.Target.Object));
		fixture.Actor.SetupGet(x => x.CombatTarget).Returns(Mock.Of<ICharacter>());
		fixture.Target.SetupGet(x => x.CombatTarget).Returns(fixture.Actor.Object);
		fixture.Target.SetupGet(x => x.MeleeRange).Returns(true);
		Assert.IsTrue(AuxiliaryMove.CanReachTarget(fixture.Actor.Object, fixture.Target.Object));
	}

	[TestMethod]
	public void QueuedAuxiliaryAttack_TargetLeavesReach_ResolutionIsIrrelevant()
	{
		var fixture = new Fixture();
		Assert.IsTrue(AuxiliaryMove.CanReachTarget(fixture.Actor.Object, fixture.Target.Object));
		fixture.Actor.Setup(x => x.ColocatedWith(fixture.Target.Object)).Returns(false);

		Assert.AreSame(CombatMoveResult.Irrelevant, fixture.Move.ResolveMove(null!));
	}

	[TestMethod]
	public void CanReachTarget_SecondaryMeleeOpponent_IsReachable()
	{
		var fixture = new Fixture();
		fixture.Actor.SetupGet(x => x.CombatTarget).Returns(Mock.Of<ICharacter>());
		fixture.Target.SetupGet(x => x.CombatTarget).Returns(fixture.Actor.Object);
		fixture.Target.SetupGet(x => x.MeleeRange).Returns(true);

		Assert.IsTrue(AuxiliaryMove.CanReachTarget(fixture.Actor.Object, fixture.Target.Object));
	}

	[TestMethod]
	public void ContextualManualAuxiliaryAction_RejectsMissingSelectionBeforeChargingStamina()
	{
		var fixture = new Fixture();
		var effect = new Mock<IContextualAuxiliaryEffect>();
		fixture.Action.SetupGet(x => x.AuxiliaryEffects).Returns(new IAuxiliaryEffect[] { effect.Object });
		fixture.Race.Setup(x => x.UsableAuxiliaryMoves(fixture.Actor.Object, fixture.Target.Object, false))
			.Returns(new IAuxiliaryCombatAction[] { fixture.Action.Object });

		var result = ManualCombatCommandResolver.TryResolve(fixture.Actor.Object, fixture.Command.Object,
			fixture.Target.Object);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Error, "target location");
		fixture.Actor.Verify(x => x.CanSpendStamina(It.IsAny<double>()), Times.Never);
		Assert.AreEqual(0, fixture.Actor.Invocations.Count(x => x.Method.Name == nameof(ICharacter.CanSpendStamina)));
	}

	[TestMethod]
	public void ContextualManualAuxiliaryAction_RejectsAnatomyThatChangedBeforeQueueing()
	{
		var fixture = new Fixture();
		var context = new ManualCombatActionContext("throat");
		var effect = new Mock<IContextualAuxiliaryEffect>();
		effect.Setup(x => x.IsValidContext(fixture.Actor.Object, fixture.Target.Object, context, out It.Ref<string>.IsAny))
			.Returns((ICharacter _, ICharacter _, ManualCombatActionContext _, out string error) =>
			{
				error = "The target has no compatible organic throat anatomy.";
				return false;
			});
		fixture.Action.SetupGet(x => x.AuxiliaryEffects).Returns(new IAuxiliaryEffect[] { effect.Object });
		fixture.Race.Setup(x => x.UsableAuxiliaryMoves(fixture.Actor.Object, fixture.Target.Object, false))
			.Returns(new IAuxiliaryCombatAction[] { fixture.Action.Object });

		var result = ManualCombatCommandResolver.TryResolve(fixture.Actor.Object, fixture.Command.Object,
			fixture.Target.Object, context: context);

		Assert.IsFalse(result.Success);
		StringAssert.Contains(result.Error, "organic throat anatomy");
		fixture.Actor.Verify(x => x.CanSpendStamina(It.IsAny<double>()), Times.Never);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ICharacter> Target { get; } = new();
		public Mock<IManualCombatCommand> Command { get; } = new();
		public Mock<IAuxiliaryCombatAction> Action { get; } = new(MockBehavior.Loose);
		public Mock<IRace> Race { get; } = new();
		public AuxiliaryMove Move { get; }

		public Fixture()
		{
			var combat = new Mock<ICombat>();
			var action = Action;
			combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[] { Actor.Object, Target.Object });
			Actor.SetupGet(x => x.Combat).Returns(combat.Object);
			Actor.SetupGet(x => x.Race).Returns(Race.Object);
			Target.SetupGet(x => x.Combat).Returns(combat.Object);
			Actor.SetupGet(x => x.CombatTarget).Returns(Target.Object);
			Actor.SetupGet(x => x.MeleeRange).Returns(true);
			Actor.Setup(x => x.ColocatedWith(Target.Object)).Returns(true);
			Actor.Setup(x => x.CanSee(Target.Object)).Returns(true);
			Action.SetupGet(x => x.StaminaCost).Returns(1.0);
			Action.SetupGet(x => x.MaximumTargets).Returns(1);
			Command.SetupGet(x => x.ActionKind).Returns(ManualCombatActionKind.AuxiliaryAction);
			Command.SetupGet(x => x.AuxiliaryAction).Returns(Action.Object);
			Command.Setup(x => x.IsUsableBy(Actor.Object, Target.Object)).Returns(true);
			Move = new AuxiliaryMove(Actor.Object, Target.Object, Action.Object);
		}
	}
}

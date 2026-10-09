#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;

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

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ICharacter> Target { get; } = new();
		public Mock<IManualCombatCommand> Command { get; } = new();
		public AuxiliaryMove Move { get; }

		public Fixture()
		{
			var combat = new Mock<ICombat>();
			var action = new Mock<IAuxiliaryCombatAction>(MockBehavior.Strict);
			combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[] { Actor.Object, Target.Object });
			Actor.SetupGet(x => x.Combat).Returns(combat.Object);
			Target.SetupGet(x => x.Combat).Returns(combat.Object);
			Actor.SetupGet(x => x.CombatTarget).Returns(Target.Object);
			Actor.SetupGet(x => x.MeleeRange).Returns(true);
			Actor.Setup(x => x.ColocatedWith(Target.Object)).Returns(true);
			Actor.Setup(x => x.CanSee(Target.Object)).Returns(true);
			Command.SetupGet(x => x.ActionKind).Returns(ManualCombatActionKind.AuxiliaryAction);
			Command.SetupGet(x => x.AuxiliaryAction).Returns(action.Object);
			Command.Setup(x => x.IsUsableBy(Actor.Object, Target.Object)).Returns(true);
			Move = new AuxiliaryMove(Actor.Object, Target.Object, action.Object);
		}
	}
}

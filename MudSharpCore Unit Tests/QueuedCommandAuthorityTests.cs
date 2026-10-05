#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class QueuedCommandAuthorityTests
{
	[DataTestMethod]
	[DataRow("valid", true)]
	[DataRow("same-identity-repeated-roots", true)]
	[DataRow("expired", false)]
	[DataRow("revoked", false)]
	[DataRow("replacement", false)]
	[DataRow("same-guid-changed-deadline", false)]
	[DataRow("commander-unloaded", false)]
	[DataRow("commander-replaced", false)]
	[DataRow("actor-reloaded", false)]
	[DataRow("canonical-owner-replaced", false)]
	[DataRow("body-rebound", false)]
	[DataRow("cell-removed", false)]
	[DataRow("policy-revoked", false)]
	[DataRow("policy-error", false)]
	[DataRow("policy-replaced", false)]
	[DataRow("whitelist-revoked", false)]
	[DataRow("ai-removed", false)]
	[DataRow("ambiguous-owner", false)]
	[DataRow("ambiguous-commander", false)]
	[DataRow("policy-ambiguous-commander", false)]
	public void AcceptedOrder_QueuedMoveRevalidatesOriginalGrantAndCurrentRuntime(string change, bool permitted)
	{
		var f = new Fixture { MutatingAction = change.Contains("ambiguous") };
		f.Order();
		var action = f.Queued!;
		Assert.IsFalse(action.SavingEffect, "Accepted authority must never be persisted or replayed.");
		switch (change)
		{
			case "same-identity-repeated-roots": ((All<ICharacter>)f.World.Object.Characters).Add(f.Owner.Object); break;
			case "expired": f.Grant = null; break;
			case "revoked": f.Owned = false; break;
			case "replacement": f.Grant = f.Grant! with { Id = Guid.NewGuid() }; break;
			case "same-guid-changed-deadline": f.Grant = f.Grant! with { DeadlineUtc = f.Grant.DeadlineUtc!.Value.AddSeconds(1) }; break;
			case "commander-unloaded": f.Roots.RemoveRange(1, 1); Assert.IsFalse(f.Roots.Any(x => ReferenceEquals(x, f.Commander.Object))); break;
			case "commander-replaced": f.CommanderInstances.Clear(); break;
			case "actor-reloaded": f.ActorInstances.Remove((ICharacterInstance)f.Actor.Object); break;
			case "canonical-owner-replaced": f.Roots.RemoveRange(0, 1); Assert.IsFalse(f.Roots.Any(x => ReferenceEquals(x, f.Owner.Object))); break;
			case "body-rebound": f.Body.SetupGet(x => x.Actor).Returns(f.Commander.Object); break;
			case "cell-removed": f.CellCharacters.RemoveAll(x => ReferenceEquals(x, f.Actor.Object)); Assert.IsFalse(f.CellCharacters.Any(x => ReferenceEquals(x, f.Actor.Object))); break;
			case "policy-revoked": f.Allowed = false; break;
			case "policy-error": f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Throws(new InvalidOperationException("Policy unavailable")); break;
			case "policy-replaced": Set(f.Ai, "_canCommandProg", Mock.Of<IFutureProg>()); break;
			case "whitelist-revoked": Set(f.Ai, "_includedCommands", new List<string> { "follow" }); break;
			case "ai-removed": f.Ais.Clear(); break;
			case "ambiguous-owner": Ambiguous(f, 30); break;
			case "ambiguous-commander": Ambiguous(f, 2); break;
			case "policy-ambiguous-commander": f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() => Ambiguous(f, 2)).Returns(true); break;
		}
		var move = action.GetMove(f.Actor.Object);
		if (change.Contains("ambiguous")) f.Actor.VerifySet(x => x.CombatTarget = It.IsAny<IPerceiver>(), Times.Never);
		Assert.AreEqual(permitted, move is not null);
		if (permitted)
		{
			Assert.IsInstanceOfType(move, typeof(RetrieveItemMove));
			Assert.IsTrue(CommandExecutionAuthority.MayExecute(move, f.Actor.Object));
		}
		f.Body.Verify(x => x.Get(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void MoveSelectedBeforeExpiry_NativeCombatGateRefusesLaterResolution()
	{
		var f = new Fixture(); var combat = f.Engage(); f.Order();
		var move = f.Queued!.GetMove(f.Actor.Object);
		Assert.IsInstanceOfType(move, typeof(RetrieveItemMove));
		f.Grant = null;
		combat.CombatAction(f.Actor.Object, move);
		f.Body.Verify(x => x.Get(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void DefensiveCallbackRevokesAuthority_NativeCombatGateDoesNotResolveOrCharge()
	{
		var f = new Fixture(); var combat = f.Engage();
		var target = new Mock<ICharacter>();
		var move = new Mock<ICombatMove>(); move.SetupGet(x => x.CharacterTargets).Returns([target.Object]);
		using (CommandExecutionAuthority.Enter(f.Actor.Object, f.Commander.Object, "get goods", () => true))
			CommandExecutionAuthority.Capture(f.Actor.Object)!.Bind(move.Object);
		target.Setup(x => x.ResponseToMove(move.Object, f.Actor.Object)).Callback(() => f.Grant = null).Returns((ICombatMove)null!);
		combat.CombatAction(f.Actor.Object, move.Object);
		target.Verify(x => x.ResponseToMove(move.Object, f.Actor.Object), Times.Once);
		move.Verify(x => x.ResolveMove(It.IsAny<ICombatMove>()), Times.Never);
		f.Actor.Verify(x => x.SpendStamina(It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void OrdinaryOrderedAndDirectActions_PreserveNativeMoveTypeAndDoNotInheritScope()
	{
		var f = new Fixture { Owned = false, Grant = null };
		f.Order();
		Assert.IsInstanceOfType(f.Queued!.GetMove(f.Actor.Object), typeof(RetrieveItemMove));
		f.Allowed = false;
		Assert.IsNull(f.Queued.GetMove(f.Actor.Object), "Ordinary ordered commands must also respect later policy revocation.");
		var direct = SelectedCombatAction.GetEffectGetItem(f.Actor.Object, f.Item.Object, null);
		Assert.IsInstanceOfType(direct.GetMove(f.Actor.Object), typeof(RetrieveItemMove), "A completed order scope must not taint direct actions.");
	}

	[TestMethod]
	public void NestedOrderScopes_PreserveActorSpecificProvenanceAndRestoreOuterScope()
	{
		var f = new Fixture();
		using (CommandExecutionAuthority.Enter(f.Actor.Object, f.Commander.Object, "get goods", () => true))
		{
			var original = CommandExecutionAuthority.Capture(f.Actor.Object);
			Assert.IsNull(CommandExecutionAuthority.Capture(f.Commander.Object));
			using (CommandExecutionAuthority.Enter(f.Commander.Object, f.Actor.Object, "stand", () => false))
				Assert.IsNull(CommandExecutionAuthority.Capture(f.Actor.Object));
			Assert.AreSame(original, CommandExecutionAuthority.Capture(f.Actor.Object));
		}
		Assert.IsNull(CommandExecutionAuthority.Capture(f.Actor.Object));
	}

	[DataTestMethod]
	[DataRow("remove-ai")]
	[DataRow("replace-prog")]
	public void PolicyCallbackRevokesConfiguration_MoveConstructionCannotMutateTarget(string change)
	{
		var f = new Fixture { MutatingAction = true }; f.Order();
		f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() =>
		{
			if (change == "remove-ai") f.Ais.Clear(); else Set(f.Ai, "_canCommandProg", Mock.Of<IFutureProg>());
		}).Returns(true);
		Assert.IsNull(f.Queued!.GetMove(f.Actor.Object));
		f.Actor.VerifySet(x => x.CombatTarget = It.IsAny<IPerceiver>(), Times.Never);
	}

	[TestMethod]
	public void DefensiveCallbackRemovesParticipant_RejectionDoesNotRecreateIdleWork()
	{
		var f = new Fixture(); var combat = f.Engage();
		var target = new Mock<ICharacter>(); var move = new Mock<ICombatMove>();
		move.SetupGet(x => x.CharacterTargets).Returns([target.Object]);
		using (CommandExecutionAuthority.Enter(f.Actor.Object, f.Commander.Object, "get goods", () => true))
			CommandExecutionAuthority.Capture(f.Actor.Object)!.Bind(move.Object);
		target.Setup(x => x.ResponseToMove(move.Object, f.Actor.Object)).Callback(() => f.Actor.Object.Combat = null).Returns((ICombatMove)null!);
		Mock.Get(f.World.Object.Scheduler).Invocations.Clear(); f.Actor.Invocations.Clear();
		combat.CombatAction(f.Actor.Object, move.Object);
		move.Verify(x => x.ResolveMove(It.IsAny<ICombatMove>()), Times.Never);
		Assert.AreEqual(0, Mock.Get(f.World.Object.Scheduler).Invocations.Count);
		Assert.IsFalse(f.Actor.Invocations.Any(x => x.Method.Name == "AddEffect"));
	}

	[DataTestMethod]
	[DataRow("grant")]
	[DataRow("combat")]
	public void ManualMultiTargetAction_DefensiveRevocationPreventsEveryChildResolution(string revocation)
	{
		var f = new Fixture(); var combat = f.Engage();
		var primary = new Mock<ICharacter>(); var secondary = new Mock<ICharacter>();
		foreach (var target in new[] { primary, secondary })
		{
			target.SetupProperty(x => x.Combat); target.SetupGet(x => x.Location).Returns(f.Actor.Object.Location);
			target.SetupGet(x => x.CombatTarget).Returns(f.Actor.Object); target.SetupGet(x => x.MeleeRange).Returns(true);
			f.Actor.Setup(x => x.ColocatedWith(target.Object)).Returns(true); combat.JoinCombat(target.Object);
		}
		var auxiliary = new Mock<IAuxiliaryCombatAction>(); auxiliary.SetupGet(x => x.Id).Returns(42); auxiliary.SetupGet(x => x.MaximumTargets).Returns(2);
		var race = new Mock<MudSharp.Character.Heritage.IRace>();
		race.Setup(x => x.UsableAuxiliaryMoves(f.Actor.Object, It.IsAny<ICharacter>(), false)).Returns([auxiliary.Object]);
		f.Actor.SetupGet(x => x.Race).Returns(race.Object); f.Actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		var command = new Mock<IManualCombatCommand>(); command.SetupGet(x => x.ActionKind).Returns(ManualCombatActionKind.AuxiliaryAction);
		command.SetupGet(x => x.AuxiliaryAction).Returns(auxiliary.Object); command.Setup(x => x.IsUsableBy(f.Actor.Object, primary.Object)).Returns(true);
		var property = typeof(CombatBase).GetProperty("GraceMoveStaminaCost", BindingFlags.NonPublic | BindingFlags.Static)!;
		var previous = property.GetValue(null); property.SetValue(null, new MudSharp.Body.Traits.TraitExpression("1", f.World.Object));
		try
		{
			SelectedCombatAction selected;
			using (CommandExecutionAuthority.Enter(f.Actor.Object, f.Commander.Object, "bash primary", () => true))
				selected = SelectedCombatAction.GetEffectManualCombatCommand(f.Actor.Object, command.Object, primary.Object);
			var move = selected.GetMove(f.Actor.Object); Assert.IsInstanceOfType(move, typeof(MultiTargetCombatMove));
			primary.Setup(x => x.ResponseToMove(It.IsAny<ICombatMove>(), f.Actor.Object)).Callback(() => { if (revocation == "grant") f.Grant = null; else f.Actor.Object.Combat = null; }).Returns((ICombatMove)null!);
			var result = move.ResolveMove(null!);
			Assert.AreSame(CombatMoveResult.Irrelevant, result); Assert.IsFalse(move.UsesStaminaWithResult(result));
			Assert.AreEqual(0, ((MultiTargetCombatMove)move).Resolutions.Count);
			secondary.Verify(x => x.ResponseToMove(It.IsAny<ICombatMove>(), It.IsAny<IPerceiver>()), Times.Never);
			f.World.Verify(x => x.GetCheck(It.IsAny<MudSharp.RPG.Checks.CheckType>()), Times.Never);
		}
		finally { property.SetValue(null, previous); }
	}

	private static void Ambiguous(Fixture f, long id)
	{
		var identity = new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.Id).Returns(id);
		var duplicate = new Mock<ICharacter>(); duplicate.SetupGet(x => x.Id).Returns(id); duplicate.SetupGet(x => x.Identity).Returns(identity.Object); ((All<ICharacter>)f.World.Object.Characters).Add(duplicate.Object);
	}

	[TestMethod]
	public void AcceptancePolicyReplacesGrant_OldApprovalCannotBindReplacement()
	{
		var f = new Fixture();
		f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() => f.Grant = f.Grant! with { Id = Guid.NewGuid() }).Returns(true);
		Assert.IsTrue(f.Ai.HandleEvent(EventType.CommandIssuedToCharacter, f.Actor.Object, f.Commander.Object, "g goods"));
		Assert.IsNull(f.Queued);
		Assert.IsNull(CommandExecutionAuthority.Capture(f.Actor.Object));
	}

	[TestMethod]
	public void InitialAuthorityCallbackRemovesParticipant_NoIdleOrScheduleResurrection()
	{
		var f = new Fixture(); var combat = f.Engage(); f.Order(); var move = f.Queued!.GetMove(f.Actor.Object);
		f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() => f.Actor.Object.Combat = null).Returns(true);
		Mock.Get(f.World.Object.Scheduler).Invocations.Clear(); f.Actor.Invocations.Clear();
		combat.CombatAction(f.Actor.Object, move);
		Assert.AreEqual(0, Mock.Get(f.World.Object.Scheduler).Invocations.Count);
		Assert.IsFalse(f.Actor.Invocations.Any(x => x.Method.Name == "AddEffect"));
		f.Body.Verify(x => x.Get(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void LatePolicyLeavesCombat_BoundNativeMoveCannotResolveOrCreateWork()
	{
		var f = new Fixture(); var combat = f.Engage(); f.Order(); var move = f.Queued!.GetMove(f.Actor.Object);
		Assert.IsInstanceOfType(move, typeof(RetrieveItemMove));
		var calls = 0;
		f.Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback(() => { if (++calls == 2) f.Actor.Object.Combat = null; }).Returns(true);
		Mock.Get(f.World.Object.Scheduler).Invocations.Clear(); f.Actor.Invocations.Clear();
		combat.CombatAction(f.Actor.Object, move);
		Assert.AreEqual(2, calls);
		Assert.AreEqual(0, Mock.Get(f.World.Object.Scheduler).Invocations.Count);
		Assert.IsFalse(f.Actor.Invocations.Any(x => x.Method.Name == "AddEffect"));
		f.Body.Verify(x => x.Get(It.IsAny<IGameItem>(), It.IsAny<int>(), It.IsAny<IEmote>(), It.IsAny<bool>(), It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void SelectedActionCannotTransferIntoReplacementCombatEvenWithUnchangedGrant()
	{
		var f = new Fixture(); f.Order(); f.Engage();
		Assert.IsNull(f.Queued!.GetMove(f.Actor.Object));
	}

	private static void Set(object target, string name, object value) =>
		target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(target, value);

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IArtificialIntelligenceControlledCharacter> Actor { get; } = new();
		public Mock<ICharacterInstance> Commander { get; } = new();
		public Mock<ICharacter> Owner { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public Mock<IFutureProg> Prog { get; } = new();
		public All<ICharacter> Roots { get; } = new();
		public List<ICharacterInstance> ActorInstances { get; } = [];
		public List<ICharacterInstance> CommanderInstances { get; } = [];
		public List<ICharacter> CellCharacters { get; } = [];
		public List<IArtificialIntelligence> Ais { get; } = [];
		public CommandableAI Ai { get; }
		public SelectedCombatAction? Queued { get; private set; }
		public bool Allowed { get; set; } = true;
		public bool MutatingAction { get; set; }
		public bool Owned { get; set; } = true;
		public SpellLifecycleOrigin? Grant { get; set; } = new(Guid.NewGuid(), 1, 3, 2, "raise-servitor",
			SpellLifecycleMode.TemporaryCleanup, new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc),
			new DateTime(2026, 10, 5, 2, 0, 0, DateTimeKind.Utc), "immutable control provenance");

		public Fixture()
		{
			Actor.As<ICharacterInstance>();
			Actor.SetupGet(x => x.Id).Returns(7); Owner.SetupGet(x => x.Id).Returns(30); Commander.SetupGet(x => x.Id).Returns(2);
			var identity = new Mock<ICharacterIdentity>(); identity.SetupGet(x => x.Id).Returns(30);
			identity.SetupGet(x => x.Instances).Returns(ActorInstances);
			Actor.SetupGet(x => x.Identity).Returns(identity.Object);
			Owner.SetupGet(x => x.Identity).Returns(identity.Object);
			ActorInstances.Add((ICharacterInstance)Actor.Object);
			var commanderIdentity = new Mock<ICharacterIdentity>(); commanderIdentity.SetupGet(x => x.Id).Returns(2);
			commanderIdentity.SetupGet(x => x.Instances).Returns(CommanderInstances);
			Commander.SetupGet(x => x.Identity).Returns(commanderIdentity.Object); CommanderInstances.Add(Commander.Object);
			Roots.Add(Owner.Object); Roots.Add(Commander.Object);
			World.SetupGet(x => x.Actors).Returns(Roots);
			World.SetupGet(x => x.Characters).Returns(new All<ICharacter>());
			World.SetupGet(x => x.NPCs).Returns(new All<ICharacter>());
			World.SetupGet(x => x.CachedActors).Returns(new All<ICharacter>());
			World.SetupGet(x => x.Scheduler).Returns(Mock.Of<IScheduler>());
			var cell = new Mock<ICell>(); cell.SetupGet(x => x.Characters).Returns(CellCharacters);
			CellCharacters.Add(Actor.Object); CellCharacters.Add(Commander.Object);
			var commanderBody = new Mock<IBody>(); commanderBody.SetupGet(x => x.Actor).Returns(Commander.Object);
			Commander.SetupGet(x => x.Body).Returns(commanderBody.Object);
			Commander.SetupGet(x => x.Location).Returns(cell.Object); Commander.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Commander.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object); Body.SetupGet(x => x.Actor).Returns(Actor.Object);
			Actor.SetupGet(x => x.Location).Returns(cell.Object); Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.IsEmbodied).Returns(true); Actor.SetupGet(x => x.InstanceId).Returns(7);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.PermissionLevel).Returns(PermissionLevel.NPC);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			var initialCombat = new Mock<ICombat>(); initialCombat.SetupGet(x => x.Combatants).Returns([Actor.Object]);
			Actor.SetupGet(x => x.Combat).Returns(initialCombat.Object);
			Actor.Setup(x => x.CheckCombatStatus()).Returns(true);
			Actor.SetupGet(x => x.AIs).Returns(Ais);
			Actor.Setup(x => x.TakeOrQueueCombatAction(It.IsAny<MudSharp.Effects.Interfaces.ISelectedCombatAction>()))
				.Returns<MudSharp.Effects.Interfaces.ISelectedCombatAction>(x => { Queued = (SelectedCombatAction)x; return true; });
			var service = new Mock<ISpellOwnedCorpseAnimationService>();
			service.Setup(x => x.OwnsInstance(7)).Returns(() => Owned);
			service.Setup(x => x.CommandGrant(7, 2)).Returns(() => Grant);
			World.SetupGet(x => x.SpellOwnedCorpseAnimations).Returns(service.Object);
			Prog.SetupGet(x => x.Id).Returns(1); Prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(() => Allowed);
			var progs = new All<IFutureProg>(); progs.Add(Prog.Object); World.SetupGet(x => x.FutureProgs).Returns(progs);
			var manager = new CharacterCommandManager();
			manager.Add(["get", "g"], new Command<ICharacter>((actor, _) =>
				actor.TakeOrQueueCombatAction(MutatingAction ? SelectedCombatAction.GetEffectMoveToMelee(actor, Commander.Object) : SelectedCombatAction.GetEffectGetItem(actor, Item.Object, null)),
				states: CharacterState.Awake, name: "Get"));
			Actor.SetupGet(x => x.CommandTree).Returns(Mock.Of<ICharacterCommandTree>(x => x.Commands == manager));
			Ai = (CommandableAI)typeof(CommandableAI).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance,
				null, [typeof(MudSharp.Models.ArtificialIntelligence), typeof(IFuturemud)], null)!.Invoke([
				new MudSharp.Models.ArtificialIntelligence { Id = 1, Name = "orders", Type = "Commandable",
					Definition = "<Definition><CanCommandProg>1</CanCommandProg><IncludedCommands><Command>get</Command></IncludedCommands></Definition>" }, World.Object]);
			Ais.Add(Ai);
		}

		public SimpleMeleeCombat Engage()
		{
			Actor.SetupProperty(x => x.Combat);
			var combat = new SimpleMeleeCombat(World.Object); combat.JoinCombat(Actor.Object); return combat;
		}

		public void Order()
		{
			Assert.IsTrue(Ai.HandleEvent(EventType.CommandIssuedToCharacter, Actor.Object, Commander.Object, "g goods"));
			Assert.IsNotNull(Queued, "Real command dispatch must queue the selected native action.");
		}
	}
}

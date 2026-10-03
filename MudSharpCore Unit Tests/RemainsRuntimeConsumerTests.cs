#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Community;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Estates;
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;
using MudSharp.RPG.Law.PatrolStrategies;
using RuntimeBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

public partial class RemainsRuntimeBoundaryTests
{
	private static void InvokeCommand(Type module, string name, ICharacter actor, string input) =>
		module.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, [actor, input]);

	private static Mock<IBody> ResolveCorpse(Fixture f, bool matching = true)
	{
		var body = new Mock<IBody>(); body.SetupGet(x => x.Id).Returns(matching ? 1 : 2);
		body.SetupGet(x => x.ExternalItems).Returns(Array.Empty<IGameItem>());
		f.Corpse.SetupGet(x => x.Body).Returns(body.Object);
		f.Corpse.SetupGet(x => x.OriginalBody).Returns(body.Object);
		f.Corpse.SetupGet(x => x.OriginalBodyId).Returns(body.Object.Id);
		return body;
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void Recovery_UnresolvedOrDifferentBodyCorpse_PreservesAssignedReportAndAllCustodyState(int failure)
	{
		var f = new Fixture(); f.Item.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		if (failure == 1) { ResolveCorpse(f); f.Corpse.SetupGet(x => x.OriginalCharacter).Returns((ICharacter)null!); }
		if (failure == 2)
		{
			ResolveCorpse(f, false); var current = new Mock<IBody>(); current.SetupGet(x => x.Id).Returns(1);
			f.Actor.SetupGet(x => x.Body).Returns(current.Object);
		}
		var zone = new Mock<IEconomicZone>(); var report = new Mock<ICorpseRecoveryReport>(); var patrol = new Mock<IPatrol>();
		report.SetupGet(x => x.Corpse).Returns(f.Item.Object); report.SetupGet(x => x.SourceCell).Returns(f.Source.Object);
		report.SetupGet(x => x.EconomicZone).Returns(zone.Object); report.SetupProperty(x => x.Status, CorpseRecoveryReportStatus.Assigned);
		report.Setup(x => x.MarkCompleted()).Callback(() => report.Object.Status = CorpseRecoveryReportStatus.Completed);
		report.Setup(x => x.MarkFailed()).Callback(() => report.Object.Status = CorpseRecoveryReportStatus.Failed);
		patrol.SetupProperty(x => x.ActiveCorpseRecoveryReport, report.Object); patrol.SetupGet(x => x.PatrolLeader).Returns(f.Actor.Object);
		patrol.SetupProperty(x => x.PatrolPhase, PatrolPhase.Patrol); f.Actor.Setup(x => x.ColocatedWith(f.Item.Object)).Returns(true);
		var strategy = (CorpseRecoveryPatrolStrategy)RuntimeHelpers.GetUninitializedObject(typeof(CorpseRecoveryPatrolStrategy));
		typeof(PatrolStrategyBase).GetField("<Gameworld>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(strategy, f.World.Object);
		strategy.HandlePatrolTick(patrol.Object);
		Assert.AreEqual(CorpseRecoveryReportStatus.Assigned, report.Object.Status);
		Assert.AreSame(report.Object, patrol.Object.ActiveCorpseRecoveryReport); Assert.AreEqual(PatrolPhase.Patrol, patrol.Object.PatrolPhase);
		patrol.Verify(x => x.ConcludePatrol(), Times.Never); Assert.AreEqual(0, f.Output.Invocations.Count);
		f.World.VerifyGet(x => x.Estates, Times.Never); f.Source.Verify(x => x.Extract(f.Item.Object), Times.Never);
		f.Item.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Never); f.Item.VerifySet(x => x.RoomLayer = It.IsAny<RoomLayer>(), Times.Never);
	}

	[TestMethod]
	public void Morgue_ResolvedFinalCorpseWithoutEstate_StillStoresSuccessfully()
	{
		var f = new Fixture(); var body = ResolveCorpse(f); f.Actor.SetupGet(x => x.Body).Returns(body.Object);
		f.Item.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		var zone = new Mock<IEconomicZone>(); var storage = new Mock<ICell>(); zone.SetupGet(x => x.MorgueStorageCell).Returns(storage.Object);
		Assert.IsTrue(MorgueService.TryIntakeCorpse(zone.Object, f.Item.Object, out var estate)); Assert.IsNull(estate);
		f.Source.Verify(x => x.Extract(f.Item.Object), Times.Once); storage.Verify(x => x.Insert(f.Item.Object, true), Times.Once);
		f.Item.Verify(x => x.AddEffect(It.IsAny<IEffect>()), Times.Once);
	}

	[TestMethod]
	public void Recovery_ResolvedFinalCorpseWithoutEstate_CompletesAfterSuccessfulCustody()
	{
		var f = new Fixture(); var body = ResolveCorpse(f); f.Actor.SetupGet(x => x.Body).Returns(body.Object);
		f.Item.SetupGet(x => x.InInventoryOf).Returns((IBody)null!);
		var zone = new Mock<IEconomicZone>(); var storage = new Mock<ICell>(); zone.SetupGet(x => x.MorgueStorageCell).Returns(storage.Object);
		var report = new Mock<ICorpseRecoveryReport>(); report.SetupGet(x => x.Corpse).Returns(f.Item.Object);
		report.SetupGet(x => x.SourceCell).Returns(f.Source.Object); report.SetupGet(x => x.EconomicZone).Returns(zone.Object);
		report.SetupProperty(x => x.Status, CorpseRecoveryReportStatus.Assigned);
		report.Setup(x => x.MarkCompleted()).Callback(() => report.Object.Status = CorpseRecoveryReportStatus.Completed);
		var patrol = new Mock<IPatrol>(); patrol.SetupProperty(x => x.ActiveCorpseRecoveryReport, report.Object);
		patrol.SetupGet(x => x.PatrolLeader).Returns(f.Actor.Object); patrol.SetupProperty(x => x.PatrolPhase, PatrolPhase.Patrol);
		f.Actor.Setup(x => x.ColocatedWith(f.Item.Object)).Returns(true); f.World.Setup(x => x.GetStaticString("CorpseRecoveryPatrolEmote")).Returns("@ recover|recovers $1.");
		var strategy = (CorpseRecoveryPatrolStrategy)RuntimeHelpers.GetUninitializedObject(typeof(CorpseRecoveryPatrolStrategy));
		typeof(PatrolStrategyBase).GetField("<Gameworld>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(strategy, f.World.Object);
		strategy.HandlePatrolTick(patrol.Object);
		Assert.AreEqual(CorpseRecoveryReportStatus.Completed, report.Object.Status); Assert.IsNull(patrol.Object.ActiveCorpseRecoveryReport);
		storage.Verify(x => x.Insert(f.Item.Object, true), Times.Once); patrol.Verify(x => x.ConcludePatrol(), Times.Once);
	}

	[TestMethod]
	public void InstallImplant_UnresolvedCorpse_DoesNotTakeHeldImplant()
	{
		var f = new Fixture(); var surgeonBody = new Mock<IBody>(); var item = new Mock<IGameItem>(); var implant = new Mock<IImplant>();
		implant.SetupGet(x => x.TargetBodypart).Returns(Mock.Of<IBodypart>()); item.Setup(x => x.GetItemType<IImplant>()).Returns(implant.Object);
		f.Actor.SetupGet(x => x.Body).Returns(surgeonBody.Object); f.Actor.Setup(x => x.TargetHeldItem("implant")).Returns(item.Object);
		f.Actor.Setup(x => x.TargetCorpse("corpse", PerceiveIgnoreFlags.None)).Returns(f.Corpse.Object);
		InvokeCommand(typeof(HealthModule), "InstallImplant", f.Actor.Object, "installimplant implant corpse");
		surgeonBody.Verify(x => x.Take(item.Object), Times.Never); implant.VerifySet(x => x.TargetBodypart = It.IsAny<IBodypart>(), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Give_UnresolvedCorpse_RefusesBeforeItemsOrCurrencyArePrepared(bool currency)
	{
		var f = new Fixture(); var coins = new Mock<ICurrencyPile>(); var money = new Mock<ICurrency>();
		f.Item.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(coins.Object);
		typeof(RuntimeBody).GetField("_heldItems", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(f.Eater,
			new List<Tuple<IGameItem, IGrab>> { Tuple.Create(f.Item.Object, Mock.Of<IGrab>()) });
		if (currency)
		{
			Assert.IsFalse(f.Eater.CanGive(money.Object, f.Corpse.Object, 1.0M, true));
			StringAssert.Contains(f.Eater.WhyCannotGive(money.Object, f.Corpse.Object, 1.0M, true), "original body");
			f.Eater.Give(money.Object, f.Corpse.Object, 1.0M, true);
		}
		else
		{
			Assert.IsFalse(f.Eater.CanGive(f.Item.Object, f.Corpse.Object));
			StringAssert.Contains(f.Eater.WhyCannotGive(f.Item.Object, f.Corpse.Object), "original body");
			f.Eater.Give(f.Item.Object, f.Corpse.Object);
		}
		CollectionAssert.AreEqual(new[] { f.Item.Object }, f.Eater.HeldItems.ToArray());
		coins.Verify(x => x.RemoveCoins(It.IsAny<IEnumerable<Tuple<ICoin, int>>>()), Times.Never);
		f.Item.Verify(x => x.PeekSplit(It.IsAny<int>()), Times.Never); f.Item.Verify(x => x.Delete(), Times.Never);
		f.Item.Verify(x => x.Get(It.IsAny<IBody>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void Dress_UnresolvedOrDifferentBodyCorpse_RefusesBeforeGarmentSelection(int failure)
	{
		var f = new Fixture(); var current = new Mock<IBody>(); current.SetupGet(x => x.Id).Returns(1);
		f.Actor.SetupGet(x => x.Body).Returns(current.Object);
		if (failure != 0) ResolveCorpse(f, false);
		if (failure == 1) f.Corpse.SetupGet(x => x.OriginalCharacter).Returns((ICharacter)null!);
		var dresser = new Mock<ICharacter>(); dresser.SetupGet(x => x.OutputHandler).Returns(f.Output.Object);
		dresser.Setup(x => x.TargetItem("corpse")).Returns(f.Item.Object);
		InvokeCommand(typeof(InventoryModule), "Dress", dresser.Object, "dress corpse garment");
		dresser.Verify(x => x.TargetHeldItem(It.IsAny<string>()), Times.Never);
		current.Verify(x => x.Dress(It.IsAny<IGameItem>(), It.IsAny<ICharacter>(), It.IsAny<IWearProfile>(), It.IsAny<IEmote>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void Resurrection_UnresolvedOrDifferentBodyCorpse_RefusesAllBodyAndCharacterMutations(int failure)
	{
		var f = new Fixture(); var current = new Mock<IBody>(); current.SetupGet(x => x.Id).Returns(1); f.Actor.SetupGet(x => x.Body).Returns(current.Object);
		f.Actor.Setup(x => x.TargetItem("corpse")).Returns(f.Item.Object);
		var body = ResolveCorpse(f, false); if (failure == 0) f.Corpse.SetupGet(x => x.OriginalBody).Returns((IBody)null!);
		if (failure == 1) f.Corpse.SetupGet(x => x.OriginalCharacter).Returns((ICharacter)null!);
		var effect = (ResurrectionEffect)RuntimeHelpers.GetUninitializedObject(typeof(ResurrectionEffect)); effect.HealWounds = true; effect.RestoreSevers = true;
		effect.GetOrApplyEffect(f.Actor.Object, f.Item.Object, default, default, Mock.Of<IMagicSpellEffectParent>(), Array.Empty<SpellAdditionalParameter>());
		InvokeCommand(typeof(StorytellerModule), "Resurrect", f.Actor.Object, "resurrect corpse");
		body.Verify(x => x.CureAllWounds(), Times.Never); body.Verify(x => x.RestoreAllBodypartsOrgansAndBones(), Times.Never);
		f.Actor.Verify(x => x.Resurrect(It.IsAny<ICell>()), Times.Never); f.Actor.VerifySet(x => x.RoomLayer = It.IsAny<RoomLayer>(), Times.Never);
		f.Item.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void StaffResurrection_ById_SkipsUnresolvedAndDifferentBodyCorpses()
	{
		var f = new Fixture(); var current = ResolveCorpse(f); f.Actor.SetupGet(x => x.Body).Returns(current.Object);
		f.Actor.SetupGet(x => x.Id).Returns(101); f.Actor.SetupGet(x => x.State).Returns(CharacterState.Dead);
		f.Actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>()); f.Actor.Setup(x => x.Resurrect(f.Source.Object)).Returns(f.Actor.Object);
		f.World.Setup(x => x.TryGetCharacter(101, true)).Returns(f.Actor.Object);
		f.World.SetupGet(x => x.Boards).Returns(new All<IBoard>());
		var unrelated = new Mock<IGameItem>(); var unresolved = new Mock<ICorpse>(); unresolved.SetupGet(x => x.RepresentsFinalCharacterDeath).Returns(true);
		unrelated.Setup(x => x.GetItemType<ICorpse>()).Returns(unresolved.Object); var items = new All<IGameItem>(); unrelated.SetupGet(x => x.Id).Returns(2);
		items.Add(f.Item.Object); items.Add(unrelated.Object); f.World.SetupGet(x => x.Items).Returns(items);
		var differentItem = new Mock<IGameItem>(); differentItem.SetupGet(x => x.Id).Returns(3); var different = new Mock<ICorpse>();
		different.SetupGet(x => x.RepresentsFinalCharacterDeath).Returns(true); different.SetupGet(x => x.OriginalCharacter).Returns(f.Actor.Object);
		var oldBody = new Mock<IBody>(); oldBody.SetupGet(x => x.Id).Returns(2); different.SetupGet(x => x.OriginalBody).Returns(oldBody.Object);
		differentItem.Setup(x => x.GetItemType<ICorpse>()).Returns(different.Object); items.Add(differentItem.Object);
		InvokeCommand(typeof(StorytellerModule), "Resurrect", f.Actor.Object, "resurrect *101");
		f.Actor.Verify(x => x.Resurrect(f.Source.Object), Times.Once); f.Item.Verify(x => x.Delete(), Times.Once); unrelated.Verify(x => x.Delete(), Times.Never);
		differentItem.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void ArenaCleanup_UnrelatedMissingOwnerCorpse_DoesNotAbortValidNpcRemainsCleanup()
	{
		var f = new Fixture(); f.Corpse.SetupGet(x => x.OriginalCharacter).Returns((ICharacter)null!);
		var npc = new Mock<ICharacter>(); npc.SetupGet(x => x.Id).Returns(101);
		var validItem = new Mock<IGameItem>(); validItem.SetupGet(x => x.Id).Returns(2); var valid = new Mock<ICorpse>(); valid.SetupGet(x => x.OriginalCharacter).Returns(npc.Object);
		validItem.Setup(x => x.GetItemType<ICorpse>()).Returns(valid.Object); var items = new All<IGameItem>(); items.Add(f.Item.Object); items.Add(validItem.Object);
		f.World.SetupGet(x => x.Items).Returns(items); var arena = (ArenaEvent)RuntimeHelpers.GetUninitializedObject(typeof(ArenaEvent));
		typeof(ArenaEvent).GetProperty(nameof(ArenaEvent.Gameworld))!.SetValue(arena, f.World.Object);
		typeof(ArenaEvent).GetMethod("DeleteNpcRemains", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(arena, [new HashSet<long> { 101 }]);
		validItem.Verify(x => x.Delete(), Times.Once); f.Item.Verify(x => x.Delete(), Times.Never);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	public void WoundInspection_PersistedBleedingPart_UsesOnlyExactPartBody(int anatomy)
	{
		var f = new Fixture(); var exact = new Mock<IBody>(MockBehavior.Strict); var current = new Mock<IBody>(MockBehavior.Strict);
		f.Item.Setup(x => x.GetItemType<ISeveredBodypart>()).Returns(f.Part.Object);
		f.Part.SetupGet(x => x.OriginalCharacter).Returns(anatomy == 0 ? null! : f.Actor.Object); f.Actor.SetupGet(x => x.Body).Returns(current.Object);
		f.Part.SetupGet(x => x.OriginalBody).Returns(anatomy == 2 ? exact.Object : null!);
		var parts = new All<IBodypart>(); f.World.SetupGet(x => x.BodypartPrototypes).Returns(parts);
		f.Item.Setup(x => x.GetSeverityFor(It.IsAny<IWound>())).Returns(WoundSeverity.Severe);
		var stored = new MudSharp.Models.Wound { Id = 1, DamageType = (int)DamageType.Slashing, CurrentDamage = 10, OriginalDamage = 10,
			ExtraInformation = $"<Definition><DamageDescription>cut</DamageDescription><BleedStatus>{(int)BleedStatus.Bleeding}</BleedStatus></Definition>" };
		var wound = new SimpleOrganicWound(f.Item.Object, stored, f.World.Object);
		f.Item.Setup(x => x.VisibleWounds(f.Actor.Object, WoundExaminationType.Look)).Returns([wound]);
		// Strict current-body mock rejects any accidental survivor effects read. Exact-body effects are permitted.
		exact.Setup(x => x.AffectedBy<MudSharp.Effects.Interfaces.IAntisepticTreatmentEffect>(It.IsAny<object>())).Returns(false);
		exact.Setup(x => x.AffectedBy<MudSharp.Effects.Concrete.AntiInflammatoryTreatment>(It.IsAny<object>())).Returns(false);
		StringAssert.Contains(f.Eater.LookWoundsText(f.Item.Object), "cut");
		foreach (var type in new[] { WoundExaminationType.Look, WoundExaminationType.Self, WoundExaminationType.Omniscient })
			StringAssert.Contains(wound.Describe(type, Outcome.MajorPass), "cut");
		_ = wound.TextForAdminWoundsCommand;
		Assert.AreEqual(BleedStatus.Bleeding, wound.BleedStatus);
		StringAssert.Contains(wound.SaveExtras(), $"<BleedStatus>{(int)BleedStatus.Bleeding}</BleedStatus>");
		current.VerifyNoOtherCalls();
	}
}

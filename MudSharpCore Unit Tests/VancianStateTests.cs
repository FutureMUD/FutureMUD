using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianStateTests
{
	[TestMethod]
	public void Queries_DoNotCreateCapacityOrPersistence()
	{
		var f = new VancianTestFixture();
		Assert.AreEqual(3, f.Service.CasterLevel(f.Actor.Object, f.Capability.Object));
		Assert.AreEqual(3, f.Service.Candidates(f.Actor.Object, f.Capability.Object, f.Rules[0].Key).Count);
		Assert.AreEqual(0, f.Service.Slots(f.Actor.Object, f.Capability.Object).Count);
		Assert.IsFalse(f.Service.CanCast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, f.Spells[0]).Available);
		Assert.AreEqual(0, f.Store.Writes); Assert.AreEqual(0, f.Store.States.Count);
	}
	[TestMethod]
	public void WholeKnownCommit_NoOpReorderingSkipsHooks_StaleDraftRefuses()
	{
		var f = new VancianTestFixture(); f.Select(3, 2); var version = f.State.Version;
		var permission = f.Progs.Single(x => x.Object.Id == f.Policies["canchangeknown"]);
		var result = f.Service.CommitKnown(f.Actor.Object, f.Capability.Object, version, new Dictionary<Guid, IReadOnlyList<long>> { [f.Rules[0].Key] = [2,3] });
		Assert.IsTrue(result.Success); Assert.AreEqual(version, f.State.Version);
		permission.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Once);
		Assert.IsFalse(f.Service.CommitKnown(f.Actor.Object, f.Capability.Object, 0, new Dictionary<Guid, IReadOnlyList<long>>()).Success);
	}
	[TestMethod]
	public void KnownCallbackFailure_CommitsOnceAndQuarantinesFurtherChanges()
	{
		var f = new VancianTestFixture(); var callback = f.Prog("onchangeknown", _ => null); f.Policies["onchangeknown"] = callback.Object.Id;
		callback.Setup(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>())).Returns(false);
		f.Select(2); Assert.AreEqual(2L, f.State.Selections[f.Rules[0].Key].Single());
		Assert.AreEqual("NeedsReview", f.Store.Log.Values.Single().Status);
		Assert.IsFalse(f.Service.CommitKnown(f.Actor.Object, f.Capability.Object, f.State.Version, new Dictionary<Guid, IReadOnlyList<long>> { [f.Rules[0].Key] = [3] }).Success);
		callback.Verify(x => x.ExecuteWithStatus(out It.Ref<object>.IsAny, It.IsAny<object[]>()), Times.Once);
	}
	[TestMethod]
	public void CrossBucketTransfer_IsARealChangeEvenWhenAggregateSpellsMatch()
	{
		var f = new VancianTestFixture(); var second = f.Rules[0] with { Key = Guid.NewGuid(), Alias = "second" }; f.Rules.Add(second); f.Select(2);
		var permission = f.Progs.Single(x => x.Object.Id == f.Policies["canchangeknown"]);
		var result = f.Service.CommitKnown(f.Actor.Object, f.Capability.Object, f.State.Version, new Dictionary<Guid, IReadOnlyList<long>> { [f.Rules[0].Key] = [], [second.Key] = [2] });
		Assert.IsTrue(result.Success); permission.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Exactly(2));
		Assert.AreEqual(0, f.State.Selections[f.Rules[0].Key].Count); Assert.AreEqual(2L, f.State.Selections[second.Key].Single());
	}
	[TestMethod]
	public void ReentrantKnownMutation_IsRejectedBeforeHookCanReplaceOuterState()
	{
		var f = new VancianTestFixture(); var permission = f.Progs.Single(x => x.Object.Id == f.Policies["canchangeknown"]);
		permission.Setup(x => x.Execute(It.IsAny<object[]>())).Returns((object[] _) =>
		{
			Assert.IsFalse(f.Service.CommitKnown(f.Actor.Object, f.Capability.Object, f.State.Version, new Dictionary<Guid, IReadOnlyList<long>>()).Success); return true;
		});
		f.Select(2); Assert.AreEqual(1, f.Store.Writes);
	}
	[TestMethod]
	public void PlansAndLastPattern_AreIndependentOfSpentSlotsAndDeletedNames()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2,2); f.Refresh();
		Assert.AreEqual(2, f.State.Slots.Count(x => x.Status == VancianSlotStatus.Prepared));
		var state = f.State; state.Slots[0].Status = VancianSlotStatus.Spent; f.Store.Commit(state, state.Version);
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "delete", "daily").Success);
		Assert.AreEqual(2, f.State.LastPattern!.Count); Assert.AreEqual(0, f.State.Loadouts.Count);
		Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots[0].Status);
		var restored = VancianCapabilityState.Load(10,20,f.State.Version,f.State.Save().ToString());
		Assert.AreEqual(VancianSlotStatus.Spent, restored.Slots[0].Status); Assert.AreEqual(2, restored.LastPattern!.Count);
	}
	[TestMethod]
	public void CapacityDropAndReturn_DoesNotEraseSpentStateOrMintNewSlots()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2,2); f.Refresh();
		var state = f.State; state.Slots[1].Status = VancianSlotStatus.Spent; f.Store.Commit(state,state.Version);
		f.Count = 1; Assert.AreEqual(VancianSlotStatus.Suspended, f.Service.Slots(f.Actor.Object,f.Capability.Object)[1].Status);
		f.Count = 3; Assert.AreEqual(VancianSlotStatus.Spent, f.Service.Slots(f.Actor.Object,f.Capability.Object)[1].Status); Assert.AreEqual(2,f.State.Slots.Count);
		f.HasCapability = false; Assert.IsTrue(f.Service.Slots(f.Actor.Object,f.Capability.Object).All(x => x.Status == VancianSlotStatus.Suspended));
		f.HasCapability = true; Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots[1].Status);
	}
	[TestMethod]
	public void PreparedCasting_SurvivesSelectionAndBookLoss_ButNotStructuralEdit()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2); f.Refresh(); f.Select(); f.BookAccessible = false;
		Assert.IsTrue(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[1]).Available);
		f.Allowances[0] = f.Allowances[0] with { StructuralVersion = 2 };
		Assert.IsFalse(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[1]).Available);
	}
	[TestMethod]
	public void PatternChangesOnly_UsesMultisetPerRuleAndRequiresEveryFormulaAfterAChange()
	{
		var f = new VancianTestFixture(); f.Rules[0] = f.Rules[0] with { Source = VancianRepertoireSource.Spellbook, BookPolicy = VancianBookPolicy.PatternChangesOnly };
		f.Plan(2,3); f.Refresh(); f.BookAccessible = false;
		Assert.AreEqual(0, f.Service.ValidatePattern(f.Actor.Object,f.Capability.Object,f.State,f.State.LastPattern!).Count);
		var swapped = f.State.LastPattern!.Select(x => x with { Ordinal = 3 - x.Ordinal }).ToArray();
		Assert.IsTrue(VancianMagicService.SameBookPattern(swapped,f.State.LastPattern!,f.Rules[0].Key));
		var changed = f.State.LastPattern!.Select(x => x.Ordinal == 2 ? x with { SpellId = 2 } : x).ToArray();
		Assert.AreEqual(2,f.Service.ValidatePattern(f.Actor.Object,f.Capability.Object,f.State,changed).Count(x => x.Contains("spellbook")));
		f.Rules[0] = f.Rules[0] with { BookPolicy = VancianBookPolicy.EveryRefresh };
		Assert.AreEqual(2,f.Service.ValidatePattern(f.Actor.Object,f.Capability.Object,f.State,f.State.LastPattern!).Count(x => x.Contains("spellbook")));
	}
	[TestMethod]
	public void AtWillAndSpontaneous_AreDifferentAndLevelZeroDoesNotGrantItself()
	{
		var f = new VancianTestFixture(); f.Allowances[0] = f.Allowances[0] with { Mode = VancianAllowanceMode.Spontaneous, SlotLevel = 0, MaximumSpellLevel = 0 };
		f.Allowances.Add(new(Guid.NewGuid(),"cantrips","Cantrips",1,VancianAllowanceMode.AtWill,null,[f.Rules[0].Key],1,0,0));
		Assert.IsFalse(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[1].Key,f.Spells[0]).Available);
		f.Select(1); Assert.IsTrue(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[1].Key,f.Spells[0]).Available);
		Assert.IsFalse(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[0]).Available);
		f.Refresh(); Assert.AreEqual(2,f.State.Slots.Count(x => x.Status == VancianSlotStatus.AvailableSpontaneous));
		Assert.IsTrue(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[0]).Available);
	}
	[TestMethod]
	public void SavedPlanChangeDuringPreparation_RefusesWithoutPartialRefresh()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2); Assert.IsTrue(f.Service.RequestRefresh(f.Actor.Object,f.Capability.Object).Success);
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object,f.Capability.Object,"rename","daily","new name").Success);
		f.Clock.Advance(TimeSpan.FromSeconds(5)); f.Action!.ExpireEffect(); Assert.AreEqual(0,f.State.Generation); Assert.AreEqual(0,f.State.Slots.Count);
	}
	[TestMethod]
	public void PowerSaturatesWithoutClampingIntoTriggerRange()
	{
		var f = new VancianTestFixture(); Assert.AreEqual(SpellPower.Strong,VancianPolicy.Power(f.Capability.Object,0,1));
		Assert.AreEqual(SpellPower.RecklesslyPowerful,VancianPolicy.Power(f.Capability.Object,0,int.MaxValue));
		f.Select(1); f.Plan(1); f.Refresh(); Mock.Get((ICastMagicTrigger)f.Spells[0].Trigger).SetupGet(x => x.MaximumPower).Returns(SpellPower.Standard);
		Assert.IsFalse(f.Service.CanCast(f.Actor.Object,f.Capability.Object,f.Rules[0].Key,f.Allowances[0].Key,f.Spells[0]).Available);
	}
	[TestMethod]
	public void InvalidPersistedSchema_DisablesWithoutSilentlyCreatingEmptyState()
	{
		var state = VancianCapabilityState.Load(10,20,2,"<VancianState schema='77' />");
		Assert.IsNotNull(state.DataError); Assert.ThrowsException<InvalidOperationException>(() => state.Save());
	}
	[TestMethod]
	public void ScrollCeiling_IgnoresSpentSlotsButRequiresPositiveUsableCapacity()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2,2); f.Refresh(); var state = f.State;
		foreach (var slot in state.Slots) slot.Status = VancianSlotStatus.Spent; f.Store.Commit(state,state.Version);
		Assert.AreEqual(1,f.Service.NormalScrollCeiling(f.Actor.Object,f.Capability.Object,f.Spells[1]));
		f.Count = 0; Assert.AreEqual(-1,f.Service.NormalScrollCeiling(f.Actor.Object,f.Capability.Object,f.Spells[1]));
		Assert.AreEqual(Difficulty.Normal,f.Service.ScrollControlDifficulty(f.Actor.Object,f.Capability.Object,f.Spells[1],2,1));
		Assert.AreEqual(Difficulty.Impossible,f.Service.ScrollControlDifficulty(f.Actor.Object,f.Capability.Object,f.Spells[1],int.MaxValue,0));
	}
	[TestMethod]
	public void PersistedIndeterminateCallbackRemainsBlockedBeyondDisplayHistoryWithoutReplay()
	{
		var f = new VancianTestFixture(); f.Select(2);
		f.Store.Record(new(Guid.NewGuid(), 10, 20, "Known", "Invoking", f.State.Version, "unconfirmed callback", f.Clock.Now.UtcDateTime));
		for (var i = 0; i < 1100; i++) f.Store.Record(new(Guid.NewGuid(), 10, 20, "Cast", "Completed", f.State.Version, "", f.Clock.Now.UtcDateTime.AddSeconds(i + 1)));
		var reloaded = new VancianMagicService(f.World.Object, f.Store, f.Clock);
		var change = reloaded.CommitKnown(f.Actor.Object, f.Capability.Object, f.State.Version, new Dictionary<Guid, IReadOnlyList<long>> { [f.Rules[0].Key] = [3] });
		Assert.IsFalse(change.Success); Assert.AreEqual(2L, f.State.Selections[f.Rules[0].Key].Single());
		Assert.IsTrue(f.Store.Log.Values.Any(x => x.Kind == "Known" && x.Status == "Invoking"));
	}
	[TestMethod]
	public void AtWillCeilingIsSpellSpecificAndFiniteCantripUpcastDoesNotAlterAtWillPower()
	{
		var f = new VancianTestFixture(); f.Select(1); f.Plan(1); f.Refresh();
		var atWill = f.Allowances[0] with { Key = Guid.NewGuid(), Alias = "atwill", Mode = VancianAllowanceMode.AtWill, SlotLevel = null, MaximumSpellLevel = 0 };
		f.Allowances.Add(atWill);
		Assert.AreEqual(SpellPower.Strong, f.Service.CanCast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, f.Spells[0]).Power);
		Assert.AreEqual(SpellPower.Standard, f.Service.CanCast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, atWill.Key, f.Spells[0]).Power);
		f.Count = 0;
		Assert.AreEqual(0, f.Service.NormalScrollCeiling(f.Actor.Object, f.Capability.Object, f.Spells[0]));
		Assert.AreEqual(-1, f.Service.NormalScrollCeiling(f.Actor.Object, f.Capability.Object, f.Spells[1]));
		f.Policies["scrolldifficulty"] = f.Prog("scrolldifficulty", _ => (int)Difficulty.ExtremelyHard).Object.Id;
		Assert.AreEqual(Difficulty.ExtremelyHard, f.Service.ScrollControlDifficulty(f.Actor.Object, f.Capability.Object, f.Spells[1], 3, 0));
		f.Policies["scrolldifficulty"] = f.Prog("scrolldifficulty", _ => 1.5).Object.Id;
		Assert.ThrowsException<InvalidOperationException>(() => f.Service.ScrollControlDifficulty(f.Actor.Object, f.Capability.Object, f.Spells[1], 3, 0));
	}
}

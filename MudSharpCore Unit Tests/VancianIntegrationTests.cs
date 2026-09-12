using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using Prog = MudSharp.FutureProg.FutureProg;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianIntegrationTests
{
	[TestMethod]
	public void FixedRepertoireRequiresStaffAndRejectsInvalidWholeCapabilityChangesAtomically()
	{
		var f = new VancianTestFixture();
		f.Policies["canchangeknown"] = f.Prog("canchangeknown", _ => false).Object.Id;
		VancianResult Commit(params long[] ids) => f.Service.CommitKnown(f.Actor.Object, f.Capability.Object,
			f.State.Version, new Dictionary<Guid, IReadOnlyList<long>> { [f.Rules[0].Key] = ids });
		Assert.IsFalse(Commit(2).Success);
		Assert.AreEqual(0, f.Store.Writes);
		Assert.IsFalse(f.Service.AdministerKnown(f.Actor.Object, f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Spells[1], true).Success);
		var staff = new Mock<ICharacter>(); staff.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		Assert.IsTrue(f.Service.AdministerKnown(staff.Object, f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Spells[1], true).Success);
		Assert.IsFalse(Commit(2, 3).Success);
		Assert.IsFalse(Commit(3).Success);
		f.Policies["canchangeknown"] = f.Prog("canchangeknown", _ => true).Object.Id;
		var version = f.State.Version;
		Assert.IsFalse(Commit(2, 2).Success);
		Assert.IsFalse(Commit(999).Success);
		f.Limit = 1; Assert.IsFalse(Commit(2, 3).Success);
		Assert.AreEqual(version, f.State.Version);
		CollectionAssert.AreEqual(new long[] { 2 }, f.State.Selections[f.Rules[0].Key].ToArray());
	}

	[TestMethod]
	public void AllPlanEditsLeaveCurrentCopiesUntouchedAndEmptyPlanCreatesUnassignedSlots()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2, 2); f.Refresh();
		var current = f.State; current.Slots[0].Status = VancianSlotStatus.Spent; f.Store.Commit(current, current.Version);
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "copy", "daily", "travel").Success);
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "rename", "travel", "reserve").Success);
		Assert.IsTrue(f.Service.SelectLoadout(f.Actor.Object, f.Capability.Object, "reserve").Success);
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "clear", "reserve", allowance: f.Allowances[0].Key, ordinal: 1).Success);
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "delete", "daily").Success);
		Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots[0].Status);
		Assert.AreEqual(VancianSlotStatus.Prepared, f.State.Slots[1].Status);
		Assert.AreEqual(2, f.State.LastPattern!.Count);
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "new", "empty").Success);
		Assert.IsTrue(f.Service.SelectLoadout(f.Actor.Object, f.Capability.Object, "empty").Success);
		f.Refresh(); Assert.IsTrue(f.State.Slots.All(x => x.Status == VancianSlotStatus.Unassigned));
		Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "assign", "empty", assignment: current.LastPattern![0]).Success);
		Assert.IsFalse(f.Service.CanCast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, f.Spells[1]).Available);
	}

	[TestMethod]
	public void CapabilityIdentitiesHaveSeparateSelectionsAndGenerationEvenWithIdenticalRuleKeys()
	{
		var f = new VancianTestFixture(); f.Select(2); f.Plan(2); f.Refresh();
		var first = f.State; first.Slots[0].Status = VancianSlotStatus.Spent; f.Store.Commit(first, first.Version);
		f.Capability.SetupGet(x => x.Id).Returns(21);
		Assert.AreEqual(0, f.State.Selections.Count); Assert.AreEqual(0, f.State.Slots.Count);
		f.Select(3); Assert.AreEqual(0, f.State.Generation);
		f.Capability.SetupGet(x => x.Id).Returns(20);
		Assert.AreEqual(2L, f.State.Selections[f.Rules[0].Key].Single());
		Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots[0].Status);
		Assert.AreEqual(2, f.Store.States.Count);
	}

	[TestMethod]
	public void MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		var books = f.Rules[0] with { Key = Guid.NewGuid(), Alias = "books", Source = VancianRepertoireSource.Spellbook, BookPolicy = VancianBookPolicy.EveryRefresh };
		f.Rules.Add(books); f.Allowances[0] = f.Allowances[0] with { RepertoireKeys = [books.Key] };
		var atWill = f.Allowances[0] with { Key = Guid.NewGuid(), Alias = "cantrips", Mode = VancianAllowanceMode.AtWill, SlotLevel = null, RepertoireKeys = [f.Rules[0].Key], MaximumSpellLevel = 0 };
		f.Allowances.Add(atWill); f.Select(1);
		var one = items.Book(101); var two = items.Book(102);
		one.Component.AddFormula(2, f.Clock.Now.UtcDateTime, null); two.Component.AddFormula(3, f.Clock.Now.UtcDateTime, null);
		f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "new", "daily");
		for (var i = 0; i < 2; i++)
			Assert.IsTrue(f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "assign", "daily", assignment:
				new(f.Allowances[0].Key, 1, i + 1, books.Key, i + 2, 1, 1, SpellPower.Standard)).Success);
		f.Service.SelectLoadout(f.Actor.Object, f.Capability.Object, "daily");
		Assert.IsTrue(f.Service.CanCast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, atWill.Key, f.Spells.First(x => x.Id == 1)).Available);
		f.Refresh(); Assert.AreEqual(2, f.State.Slots.Count(x => x.Status == VancianSlotStatus.Prepared));
		items.Items.Clear();
		Assert.IsFalse(f.Service.CanRefresh(f.Actor.Object, f.Capability.Object));
		Assert.IsTrue(f.Service.CanCast(f.Actor.Object, f.Capability.Object, books.Key, f.Allowances[0].Key, items.Spell, 1).Available);
		f.Rules[1] = books with { BookPolicy = VancianBookPolicy.PatternChangesOnly };
		Assert.IsTrue(f.Service.CanRefresh(f.Actor.Object, f.Capability.Object));
		var replacement = items.Book(103); replacement.Component.AddFormula(2, f.Clock.Now.UtcDateTime, null);
		f.Service.EditLoadout(f.Actor.Object, f.Capability.Object, "assign", "daily", assignment: new(f.Allowances[0].Key, 1, 2, books.Key, 2, 1, 1, SpellPower.Standard));
		f.Refresh(); Assert.AreEqual(2, f.State.Slots.Count(x => x.Preparation?.SpellId == 2));
	}

	[TestMethod]
	public void ScriptReservationAndPlayerInscriptionUseTheSameGuardedTimedDebit()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		f.Select(2); f.Plan(2); f.Refresh(); var blank = items.Scroll(101);
		var begin = new Prog(f.World.Object, "begin", ProgVariableTypes.Text,
			new[] { Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.MagicCapability, "cap"), Tuple.Create(ProgVariableTypes.MagicSpell, "spell"), Tuple.Create(ProgVariableTypes.Item, "blank") },
			"return begininscribespellscroll(@actor, @cap, \"known\", \"first\", @spell, 1, @blank)");
		Assert.IsTrue(begin.Compile(), begin.CompileError);
		var token = begin.Execute(f.Actor.Object, f.Capability.Object, items.Spell, blank.Item.Object) as string;
		Assert.IsTrue(Guid.TryParse(token, out var operation));
		Assert.AreEqual(VancianSlotStatus.Reserved, f.State.Slots[0].Status);
		Assert.IsFalse(f.Service.BeginInscription(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, items.Spell, 1, blank.Item.Object).Success);
		var finish = new Prog(f.World.Object, "finish", ProgVariableTypes.Boolean,
			new[] { Tuple.Create(ProgVariableTypes.Character, "actor"), Tuple.Create(ProgVariableTypes.Text, "token") }, "return completevancianwriting(@actor, @token)");
		Assert.IsTrue(finish.Compile(), finish.CompileError);
		Assert.AreEqual(false, finish.Execute(f.Actor.Object, token!));
		f.Clock.Advance(TimeSpan.FromSeconds(1));
		Assert.AreEqual(true, finish.Execute(f.Actor.Object, token!));
		Assert.AreEqual(false, finish.Execute(f.Actor.Object, token!));
		Assert.IsTrue(blank.Component.IsCharged); Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots[0].Status);
		Assert.AreEqual("Completed", f.Store.Operation(operation)!.Status);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ScrollGroupWithFirstTargetResistedOrWardedStillAffectsLaterTargetsAndConsumesOnce(bool warded)
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		var spell = VancianSnapshotTests.Spell(f, "<Effect type='staminadelta'><Formula>source:1 + castinglevel</Formula></Effect>", triggerType: "party", triggerContent: "<TargetFilterProg>0</TargetFilterProg>");
		spell.OpposedTrait = f.Trait.Object; spell.OpposedDifficulty = Difficulty.Normal;
		var targets = Enumerable.Range(0, 3).Select(_ => new Mock<ICharacter> { DefaultValue = DefaultValue.Mock }).ToArray();
		foreach (var target in targets) target.SetupGet(x => x.Location).Returns(() => null!);
		var party = new Mock<IParty>(); party.SetupGet(x => x.CharacterMembers).Returns(targets.Select(x => x.Object)); f.Actor.SetupGet(x => x.Party).Returns(party.Object);
		var resistance = new Mock<ICheck>(); f.World.Setup(x => x.GetCheck(CheckType.ResistMagicSpellCheck)).Returns(resistance.Object);
		resistance.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(), f.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns((IPerceivableHaveTraits target, Difficulty _, ITraitDefinition _, IPerceivable _, double _, TraitUseType _, (string, object)[] _) => Enum.GetValues<Difficulty>().ToDictionary(d => d, _ => CheckOutcome.SimpleOutcome(CheckType.ResistMagicSpellCheck, ReferenceEquals(target, targets[0].Object) ? Outcome.MajorPass : Outcome.Fail)));
		if (warded)
		{
			var ward = new Mock<IMagicInterdictionEffect>(); ward.SetupGet(x => x.Coverage).Returns(MagicInterdictionCoverage.Incoming); ward.SetupGet(x => x.Mode).Returns(MagicInterdictionMode.Fail);
			ward.Setup(x => x.ShouldInterdict(f.Actor.Object, f.School.Object)).Returns(true);
			targets[0].Setup(x => x.EffectsOfType<IMagicInterdictionEffect>(It.IsAny<Predicate<IMagicInterdictionEffect>>())).Returns([ward.Object]);
		}
		f.Actor.Setup(x => x.TraitValue(f.Trait.Object, TraitBonusContext.None)).Returns(12);
		var scroll = items.Scroll(101); var token = Guid.NewGuid(); scroll.Component.Reserve(token);
		scroll.Component.Charge(token, StoredSpellSnapshot.Capture(spell, f.Actor.Object, f.Capability.Object, 1, SpellPower.Standard, 3, f.Clock.Now.UtcDateTime));
		f.Actor.Setup(x => x.TraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<TraitBonusContext>())).Returns(999);
		var result = f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack(""));
		Assert.IsTrue(result.Success, result.Message);
		targets[0].Verify(x => x.GainStamina(It.IsAny<double>()), Times.Never);
		foreach (var target in targets.Skip(1)) target.Verify(x => x.GainStamina(13), Times.Once);
		scroll.Item.Verify(x => x.Delete(), Times.Once);
		Assert.AreEqual(1, f.Store.Log.Count);
		f.Actor.Verify(x => x.UseResource(It.IsAny<IMagicResource>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void CharacterAndExitParametersSurviveActualScrollAndFiniteTargetParsing()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		var spell = VancianSnapshotTests.Spell(f, "<Effect type='forcedexitmovement'/>", triggerType: "characterexit", triggerContent: "<TargetFilterProg>0</TargetFilterProg><CanTargetSelf>true</CanTargetSelf>");
		var room = new Mock<ICell>(); var exit = new Mock<ICellExit>(); var target = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		f.Actor.SetupGet(x => x.Location).Returns(room.Object); room.Setup(x => x.GetExitKeyword("north", f.Actor.Object)).Returns(exit.Object);
		f.Actor.Setup(x => x.TargetActorOrCorpse(It.IsAny<string>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(() => null!);
		f.Actor.Setup(x => x.TargetActorOrCorpse("friend", It.IsAny<PerceiveIgnoreFlags>())).Returns(target.Object);
		target.SetupGet(x => x.Location).Returns(() => null!);
		target.Setup(x => x.CanMove(exit.Object, It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
		target.Setup(x => x.CanCross(exit.Object)).Returns((true, null!));
		var scroll = items.Scroll(101); var token = Guid.NewGuid(); scroll.Component.Reserve(token);
		scroll.Component.Charge(token, StoredSpellSnapshot.Capture(spell, f.Actor.Object, f.Capability.Object, 1, SpellPower.Standard, 3, f.Clock.Now.UtcDateTime));
		Assert.IsFalse(f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack("missing north")).Success);
		Assert.IsFalse(scroll.Item.Object.Deleted);
		var release = f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack("friend north"));
		Assert.IsTrue(release.Success, release.Message);
		target.Verify(x => x.Move(exit.Object, It.IsAny<MudSharp.PerceptionEngine.IEmote>(), true), Times.Once);
		f.Select(2); f.Plan(2); f.Refresh();
		var cast = f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, 1, new StringStack("friend north"));
		Assert.IsTrue(cast.Success, cast.Message);
		target.Verify(x => x.Move(exit.Object, It.IsAny<MudSharp.PerceptionEngine.IEmote>(), true), Times.Exactly(2));
	}

	[TestMethod]
	public void RemovedStoredPolicyReferenceRefusesBeforeAnyConsumption()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		var policy = f.Prog("candidates", _ => true);
		var spell = VancianSnapshotTests.Spell(f, triggerType: "party", triggerContent: $"<TargetFilterProg>{policy.Object.Id}</TargetFilterProg>");
		var scroll = items.Scroll(101); var token = Guid.NewGuid(); scroll.Component.Reserve(token);
		scroll.Component.Charge(token, StoredSpellSnapshot.Capture(spell, f.Actor.Object, f.Capability.Object, 1, SpellPower.Standard, 3, f.Clock.Now.UtcDateTime));
		f.Progs.Remove(policy);
		var result = f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack(""));
		Assert.IsFalse(result.Success); StringAssert.Contains(result.Message, "TargetFilterProg");
		Assert.IsTrue(scroll.Component.IsCharged); Assert.IsFalse(scroll.Item.Object.Deleted); Assert.AreEqual(0, f.Store.Log.Count);
	}

	[TestMethod]
	public void NonCasterAndRevokedPayloadRefuseButUnknownSpellWithAllSlotsSpentReleases()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; var scroll = items.Scroll(101); items.Charge(scroll.Component);
		f.HasCapability = false; Assert.IsFalse(f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack("")).Success);
		f.HasCapability = true; items.Spell.ScrollInscriptionAllowed = false;
		Assert.IsFalse(f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack("")).Success);
		Assert.IsFalse(scroll.Item.Object.Deleted); Assert.IsNull(scroll.Component.Reservation);
		items.Spell.ScrollInscriptionAllowed = true; f.Select(2); f.Plan(2); f.Refresh();
		var state = f.State; foreach (var slot in state.Slots) slot.Status = VancianSlotStatus.Spent; f.Store.Commit(state, state.Version); f.Select();
		Assert.IsFalse(f.Service.KnowsThroughVancian(f.Actor.Object, items.Spell));
		Assert.IsTrue(f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack("")).Success);
		Assert.IsTrue(f.State.Slots.All(x => x.Status == VancianSlotStatus.Spent));
	}

	[TestMethod]
	public void AvailabilityAndCastingUseLevelBasedCostsAndScrollReleaseNeverPaysThemAgain()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; f.Count = 1;
		var resource = new Mock<IMagicResource>(); resource.SetupGet(x => x.Id).Returns(7); resource.SetupGet(x => x.Name).Returns("mana");
		var expression = new TraitExpression(new MudSharp.Models.TraitExpression { Id = 9, Name = "level cost", Expression = "spelllevel + castinglevel + casterlevel" }, f.World.Object);
		f.World.SetupGet(x => x.MagicResources).Returns(VancianTestFixture.Collection<IMagicResource>(() => [resource.Object]));
		f.World.SetupGet(x => x.TraitExpressions).Returns(VancianTestFixture.Collection<ITraitExpression>(() => [expression]));
		var model = items.Spell.SnapshotModel(); var definition = XElement.Parse(model.Definition);
		definition.Element("Costs")!.Add(new XElement("Cost", new XAttribute("resource", 7), new XAttribute("expression", 9))); model.Definition = definition.ToString();
		var spell = new MagicSpell(model, f.World.Object); f.Spells.RemoveAll(x => x.Id == 2); f.Spells.Add(spell);
		f.Allowances[0] = f.Allowances[0] with { SlotLevel = 3 };
		f.Select(2); f.Plan(2); f.Refresh(); var power = f.State.Slots[0].Preparation!.Power;
		f.Actor.Setup(x => x.CanUseResource(resource.Object, It.IsAny<double>())).Returns((IMagicResource _, double cost) => cost <= 6);
		Assert.IsFalse(spell.CharacterCanCast(f.Actor.Object, f.Actor.Object, power));
		Assert.IsFalse(f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, 1, new StringStack("")).Success);
		Assert.AreEqual(VancianSlotStatus.Prepared, f.State.Slots[0].Status);
		f.Actor.Setup(x => x.CanUseResource(resource.Object, It.IsAny<double>())).Returns((IMagicResource _, double cost) => cost <= 7);
		Assert.IsTrue(spell.CharacterCanCast(f.Actor.Object, f.Actor.Object, power));
		Assert.IsTrue(f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, 1, new StringStack("")).Success);
		f.Actor.Verify(x => x.UseResource(resource.Object, 7), Times.Once);
		f.Refresh(); var scroll = items.Scroll(101);
		var started = f.Service.BeginInscription(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, spell, 1, scroll.Item.Object);
		Assert.IsTrue(started.Success, started.Message); f.Clock.Advance(TimeSpan.FromSeconds(1));
		Assert.IsTrue(f.Service.CompleteWriting(f.Actor.Object, started.OperationId!.Value).Success);
		f.Actor.Setup(x => x.CanUseResource(It.IsAny<IMagicResource>(), It.IsAny<double>())).Returns(false);
		Assert.IsTrue(f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack("")).Success);
		f.Actor.Verify(x => x.UseResource(resource.Object, 7), Times.Exactly(2));
		Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots.Single().Status);
	}

	[TestMethod]
	public void BookCopyPaysActualProductionMaterialOnceAndCancellationPaysNothing()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F;
		var source = items.Book(101); var destination = items.Book(102); var material = items.Item(103);
		source.Component.AddFormula(2, f.Clock.Now.UtcDateTime, null);
		material.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true); f.Actor.SetupGet(x => x.Body.HeldItems).Returns([material.Object]);
		var proto = (MudSharp.GameItems.Prototypes.SpellbookGameItemComponentProto)destination.Component.Prototype;
		proto.ProductionPlan.Phases.First().AddAction(new InventoryPlanActionConsume(f.World.Object, 1, 0, 0, item => item.Id == material.Object.Id, null!));
		var cancelled = f.Service.BeginTranscription(f.Actor.Object, f.Capability.Object, source.Item.Object, items.Spell, destination.Item.Object);
		Assert.IsTrue(cancelled.Success, cancelled.Message);
		Assert.IsTrue(f.Service.CancelWriting(f.Actor.Object, cancelled.OperationId!.Value).Success);
		material.Verify(x => x.Delete(), Times.Never); Assert.AreEqual(0, destination.Component.Formulae.Count);
		var copy = f.Service.BeginTranscription(f.Actor.Object, f.Capability.Object, source.Item.Object, items.Spell, destination.Item.Object);
		Assert.IsTrue(copy.Success, copy.Message); f.Clock.Advance(TimeSpan.FromSeconds(1));
		var completed = f.Service.CompleteWriting(f.Actor.Object, copy.OperationId!.Value);
		Assert.IsTrue(completed.Success, completed.Message);
		material.Verify(x => x.Delete(), Times.Once); Assert.AreEqual(1, source.Component.Formulae.Count); Assert.AreEqual(1, destination.Component.Formulae.Count);
		Assert.IsFalse(f.Service.CompleteWriting(f.Actor.Object, copy.OperationId.Value).Success);
		material.Verify(x => x.Delete(), Times.Once);
	}
	[TestMethod]
	public void PublicHelpAndScrollInspectionDoNotCreateStateOrActivateTheCharge()
	{
		var items = new VancianItemTests.ItemFixture(); var f = items.F; var scroll = items.Scroll(101); items.Charge(scroll.Component);
		var binding = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
		typeof(MagicModule).GetMethod("VancianPlayer", binding)!.Invoke(null, [f.Actor.Object, f.School.Object, new StringStack("help")]);
		typeof(MagicModule).GetMethod("SpellScrollCommand", binding)!.Invoke(null, [f.Actor.Object, "spellscroll show 101"]);
		f.Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		typeof(MagicModule).GetMethod("VancianAdmin", binding)!.Invoke(null, [f.Actor.Object, new StringStack("help")]);
		Assert.IsTrue(scroll.Component.IsCharged); Assert.IsNull(scroll.Component.Reservation); Assert.IsFalse(scroll.Item.Object.Deleted);
		Assert.AreEqual(0, f.Store.Writes); Assert.AreEqual(0, f.Store.Log.Count);
		f.World.Verify(x => x.GetCheck(It.IsAny<CheckType>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void EffectFailureAfterCommitLeavesDiagnosticAndNeverReplaysFiniteOrScrollCasting(bool scrollCast)
	{
		var items = new VancianItemTests.ItemFixture("<Effect type='staminadelta'><Formula>1</Formula></Effect>"); var f = items.F;
		f.Actor.Setup(x => x.GainStamina(1)).Throws(new InvalidOperationException("Injected effect failure"));
		f.Select(2); f.Plan(2); f.Refresh(); var scroll = items.Scroll(101);
		if (scrollCast) items.Charge(scroll.Component);
		VancianResult Cast() => scrollCast
			? f.Service.ActivateScroll(f.Actor.Object, f.Capability.Object, scroll.Item.Object, new StringStack(""))
			: f.Service.Cast(f.Actor.Object, f.Capability.Object, f.Rules[0].Key, f.Allowances[0].Key, items.Spell, 1, new StringStack(""));
		Assert.IsFalse(Cast().Success);
		var operation = f.Store.Log.Values.Single(x => x.Kind == (scrollCast ? "ScrollActivation" : "Cast"));
		Assert.AreEqual("NeedsReview", operation.Status); StringAssert.Contains(operation.Diagnostic, "Injected effect failure");
		Assert.IsFalse(Cast().Success); f.Actor.Verify(x => x.GainStamina(1), Times.Once);
		if (scrollCast) Assert.IsTrue(scroll.Item.Object.Deleted);
		else Assert.AreEqual(VancianSlotStatus.Spent, f.State.Slots[0].Status);
	}
}

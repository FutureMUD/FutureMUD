using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Health;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Vancian;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Law;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class VancianItemTests
{
	internal static MudSharp.Models.GameItemComponentProto Prototype(string type) => new()
	{
		Id = 1, Name = type, Type = type, Description = "Test magical writing", RevisionNumber = 0,
		Definition = "<Definition version='1'><Capacity>5</Capacity><Duration>1</Duration><Plan><Phase/></Plan></Definition>",
		EditableItem = new() { Id = 1, BuilderAccountId = 1, BuilderDate = DateTime.UtcNow, RevisionNumber = 0, RevisionStatus = 2 }
	};
	internal static string Serialize(IGameItemComponent component) => (string)component.GetType().GetMethod("SaveToXml",BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(component,null)!;
	internal sealed class ItemFixture
	{
		public VancianTestFixture F { get; } = new(actualBooks: true);
		public MagicSpell Spell { get; }
		public List<IGameItem> Items { get; } = [];
		public ItemFixture(string effects = "")
		{
			Spell = VancianSnapshotTests.Spell(F,effects);
			F.Actor.SetupGet(x => x.ContextualItems).Returns(() => Items);
			F.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(),It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			F.Actor.Setup(x => x.CanManipulateItem(It.IsAny<IGameItem>())).Returns((true,""));
			F.World.SetupGet(x => x.LegalAuthorities).Returns(VancianTestFixture.Collection<ILegalAuthority>(() => []));
			F.Actor.SetupGet(x => x.Location).Returns(() => null!);
		}
		public Mock<IGameItem> Item(long id)
		{
			var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(id); item.SetupGet(x => x.Name).Returns($"writing {id}");
			item.SetupGet(x => x.GetObject).Returns(() => item.Object); item.SetupGet(x => x.Type).Returns(MudSharp.FutureProg.ProgVariableTypes.Item);
			item.SetupGet(x => x.Gameworld).Returns(F.World.Object); var deleted = false; item.SetupGet(x => x.Deleted).Returns(() => deleted);
			item.Setup(x => x.Delete()).Callback(() => deleted = true); Items.Add(item.Object); return item;
		}
		public (Mock<IGameItem> Item, SpellScrollGameItemComponent Component) Scroll(long id)
		{
			var item = Item(id); var proto = new SpellScrollGameItemComponentProto(Prototype("SpellScroll"),F.World.Object);
			var component = new SpellScrollGameItemComponent(proto,item.Object,true); item.Setup(x => x.GetItemType<ISpellScroll>()).Returns(component);
			return (item,component);
		}
		public (Mock<IGameItem> Item, SpellbookGameItemComponent Component) Book(long id)
		{
			var item = Item(id); var proto = new SpellbookGameItemComponentProto(Prototype("Spellbook"),F.World.Object);
			var component = new SpellbookGameItemComponent(proto,item.Object,true); item.Setup(x => x.GetItemType<ISpellbook>()).Returns(component);
			return (item,component);
		}
		public void Charge(SpellScrollGameItemComponent scroll, int level = 1)
		{
			var token = Guid.NewGuid(); Assert.IsTrue(scroll.Reserve(token));
			scroll.Charge(token,StoredSpellSnapshot.Capture(Spell,F.Actor.Object,F.Capability.Object,level,SpellPower.Standard,3,F.Clock.Now.UtcDateTime));
		}
	}
	[TestMethod]
	public void InstancePayloadsRoundTrip_BookCopiesAreIndependent_ScrollCopiesStartBlank()
	{
		var f = new ItemFixture(); var book = f.Book(100); Assert.IsTrue(book.Component.AddFormula(2,f.F.Clock.Now.UtcDateTime,null));
		var copy = (SpellbookGameItemComponent)book.Component.Copy(f.Item(101).Object,true);
		Assert.IsTrue(copy.RemoveFormula(2)); Assert.AreEqual(1,book.Component.Formulae.Count);
		var restored = new SpellbookGameItemComponent(new() { Id = 50, Definition = Serialize(book.Component) },(SpellbookGameItemComponentProto)book.Component.Prototype,book.Item.Object);
		Assert.AreEqual(book.Component.Formulae.Single(),restored.Formulae.Single());
		var scroll = f.Scroll(200); f.Charge(scroll.Component);
		var scrollCopy = (SpellScrollGameItemComponent)scroll.Component.Copy(f.Item(201).Object,true); Assert.IsTrue(scrollCopy.IsBlank);
		var restoredScroll = new SpellScrollGameItemComponent(new() { Id = 51, Definition = Serialize(scroll.Component) },(SpellScrollGameItemComponentProto)scroll.Component.Prototype,scroll.Item.Object);
		Assert.IsTrue(restoredScroll.IsCharged); Assert.AreEqual(scroll.Component.ChargeId,restoredScroll.ChargeId);
	}
	[TestMethod]
	public void FiniteInscriptionReservesThenDebitsOnce_AndNeverReleasesSpellEffects()
	{
		var f = new ItemFixture(); f.F.Select(2); f.F.Plan(2); f.F.Refresh(); var scroll = f.Scroll(100);
		var check = new Mock<ICheck>(); f.F.World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(check.Object);
		var start = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,1,scroll.Item.Object);
		Assert.IsTrue(start.Success,start.Message); Assert.IsTrue(scroll.Component.IsBlank); Assert.AreEqual(VancianSlotStatus.Reserved,f.F.State.Slots[0].Status);
		Assert.IsFalse(f.F.Service.CompleteWriting(f.F.Actor.Object,start.OperationId!.Value).Success);
		f.F.Clock.Advance(TimeSpan.FromSeconds(1)); var result = f.F.Service.CompleteWriting(f.F.Actor.Object,start.OperationId.Value);
		Assert.IsTrue(result.Success,result.Message); Assert.IsTrue(scroll.Component.IsCharged); Assert.AreEqual(VancianSlotStatus.Spent,f.F.State.Slots[0].Status);
		Assert.IsFalse(f.F.Service.CompleteWriting(f.F.Actor.Object,start.OperationId.Value).Success);
		Assert.AreEqual(0,check.Invocations.Count);
	}
	[TestMethod]
	public void InscriptionCancellationKeepsBlankAndExactUnspentCasting()
	{
		var f = new ItemFixture(); f.F.Select(2); f.F.Plan(2); f.F.Refresh(); var scroll = f.Scroll(100);
		var start = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,1,scroll.Item.Object);
		Assert.IsTrue(start.Success,start.Message); Assert.IsTrue(f.F.Service.CancelWriting(f.F.Actor.Object,start.OperationId!.Value).Success);
		Assert.IsTrue(scroll.Component.IsBlank); Assert.IsNull(scroll.Component.Reservation); Assert.AreEqual(VancianSlotStatus.Prepared,f.F.State.Slots[0].Status);
	}
	[TestMethod]
	public void HighLevelScrollTranscriptionConsumesScrollWithoutSlotsOrActivationChecks()
	{
		var f = new ItemFixture(); f.Spell.SpellLevel = 5; var scroll = f.Scroll(100); f.Charge(scroll.Component,5); var book = f.Book(200);
		f.F.Count = 0; var check = new Mock<ICheck>(); f.F.World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(check.Object);
		var start = f.F.Service.BeginTranscription(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,f.Spell,book.Item.Object);
		Assert.IsTrue(start.Success,start.Message); Assert.IsNotNull(scroll.Component.Reservation); Assert.AreEqual(0,book.Component.Formulae.Count);
		Assert.IsFalse(f.F.Service.ActivateScroll(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,new StringStack("")).Success);
		f.F.Clock.Advance(TimeSpan.FromSeconds(1)); var completed = f.F.Service.CompleteWriting(f.F.Actor.Object,start.OperationId!.Value);
		Assert.IsTrue(completed.Success,completed.Message); Assert.AreEqual(2L,book.Component.Formulae.Single().SpellId); Assert.IsTrue(scroll.Item.Object.Deleted);
		Assert.AreEqual(0,f.F.State.Slots.Count); Assert.AreEqual(0,check.Invocations.Count); Assert.IsTrue(f.F.Store.ItemConsumed(scroll.Item.Object.Id,scroll.Component.ChargeId!.Value));
	}
	[TestMethod]
	public void DuplicateFormulaRefusalPreservesScroll_AndBorrowedBookCopyPreservesSource()
	{
		var f = new ItemFixture(); var source = f.Book(100); var destination = f.Book(200); source.Component.AddFormula(2,f.F.Clock.Now.UtcDateTime,null);
		var start = f.F.Service.BeginTranscription(f.F.Actor.Object,f.F.Capability.Object,source.Item.Object,f.Spell,destination.Item.Object);
		Assert.IsTrue(start.Success,start.Message); f.F.Clock.Advance(TimeSpan.FromSeconds(1)); Assert.IsTrue(f.F.Service.CompleteWriting(f.F.Actor.Object,start.OperationId!.Value).Success);
		Assert.AreEqual(1,source.Component.Formulae.Count); Assert.AreEqual(1,destination.Component.Formulae.Count); Assert.IsFalse(source.Item.Object.Deleted);
		var scroll = f.Scroll(300); f.Charge(scroll.Component);
		Assert.IsFalse(f.F.Service.BeginTranscription(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,f.Spell,destination.Item.Object).Success);
		Assert.IsFalse(scroll.Item.Object.Deleted); Assert.IsNull(scroll.Component.Reservation);
	}
	[TestMethod]
	public void ScrollReleaseConsumesBeforeEffects_AndTombstonePreventsStaleReloadReplay()
	{
		var f = new ItemFixture(); var scroll = f.Scroll(100); f.Charge(scroll.Component); var saved = Serialize(scroll.Component);
		var result = f.F.Service.ActivateScroll(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,new StringStack(""));
		Assert.IsTrue(result.Success,result.Message); Assert.IsTrue(scroll.Item.Object.Deleted); Assert.IsTrue(scroll.Component.Spent);
		var stale = new SpellScrollGameItemComponent(new() { Id = 20, Definition = saved },(SpellScrollGameItemComponentProto)scroll.Component.Prototype,scroll.Item.Object);
		scroll.Item.SetupGet(x => x.Deleted).Returns(false); scroll.Item.Setup(x => x.GetItemType<ISpellScroll>()).Returns(stale);
		Assert.IsFalse(f.F.Service.ActivateScroll(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,new StringStack("")).Success);
	}
	[TestMethod]
	public void ScrollOverLevelFailureDestroysChargeWithoutSpellEffects()
	{
		var f = new ItemFixture(); var scroll = f.Scroll(100); f.Charge(scroll.Component,5); var check = new Mock<ICheck>();
		f.F.World.Setup(x => x.GetCheck(CheckType.CastSpellCheck)).Returns(check.Object);
		check.Setup(x => x.Check(f.F.Actor.Object,It.IsAny<Difficulty>(),f.F.Trait.Object,It.IsAny<IPerceivable>(),It.IsAny<double>(),It.IsAny<TraitUseType>(),It.IsAny<(string,object)[]>())).Returns(CheckOutcome.SimpleOutcome(CheckType.CastSpellCheck,Outcome.Fail));
		var result = f.F.Service.ActivateScroll(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,new StringStack(""));
		Assert.IsTrue(result.Success,result.Message); StringAssert.Contains(result.Message,"fizzled"); Assert.IsTrue(scroll.Item.Object.Deleted); Assert.AreEqual(1,check.Invocations.Count);
	}
	[TestMethod]
	public void ProductionPlanDetectsSharedQuantityUnderpaymentBeforeAnyConsumption()
	{
		var f = new ItemFixture(); var material = f.Item(100); material.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		f.F.Actor.SetupGet(x => x.Body.HeldItems).Returns([material.Object]);
		var first = new InventoryPlanTemplate(f.F.World.Object,new InventoryPlanActionConsume(f.F.World.Object,1,0,0,_ => true,null!));
		var second = new InventoryPlanTemplate(f.F.World.Object,new InventoryPlanActionConsume(f.F.World.Object,1,0,0,_ => true,null!));
		using var plan = new VancianProductionPlan(f.F.Actor.Object,first,second);
		var validation = plan.Validate();
		Assert.IsNotNull(validation); StringAssert.Contains(validation,"quantity");
		Assert.IsFalse(material.Object.Deleted); material.Verify(x => x.Delete(),Times.Never);
	}
	[TestMethod]
	public void ScrollPrototypeRejectsContainerAndStackableCompositionsInEitherOrder()
	{
		var f = new ItemFixture(); var scroll = f.Scroll(100).Component.Prototype;
		var container = new Mock<IGameItemComponentProto>(); container.As<IContainerPrototype>();
		var stackable = new Mock<IGameItemComponentProto>(); stackable.As<IStackablePrototype>();
		Assert.IsFalse(GameItemComponentPrototypeExclusivity.CanAddComponent([scroll],container.Object,out _));
		Assert.IsFalse(GameItemComponentPrototypeExclusivity.CanAddComponent([stackable.Object],scroll,out _));
		Assert.AreEqual(1,GameItemComponentPrototypeExclusivity.FindConflicts([scroll,container.Object]).Count);
	}
	[DataTestMethod]
	[DataRow("state")] [DataRow("combat")] [DataRow("quit")] [DataRow("death")]
	public void RealActionEventsCancelInscriptionAndReleaseExactReservation(string interruption)
	{
		var f = new ItemFixture(); f.F.Count = 1; f.F.Select(2); f.F.Plan(2); f.F.Refresh(); var scroll = f.Scroll(100);
		var start = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,1,scroll.Item.Object);
		Assert.IsTrue(start.Success,start.Message);
		switch (interruption)
		{
			case "state": f.F.Actor.Raise(x => x.OnStateChanged += null,f.F.Actor.Object); break;
			case "combat": f.F.Actor.Raise(x => x.OnEngagedInMelee += null,f.F.Actor.Object); break;
			case "quit": f.F.Actor.Raise(x => x.OnQuit += null,f.F.Actor.Object); break;
			case "death": f.F.Actor.Raise(x => x.OnDeath += null,f.F.Actor.Object); break;
		}
		Assert.IsNull(f.F.Action); Assert.IsNull(scroll.Component.Reservation); Assert.IsTrue(scroll.Component.IsBlank);
		Assert.AreEqual(VancianSlotStatus.Prepared,f.F.State.Slots.Single().Status); Assert.AreEqual("Cancelled",f.F.Store.Operation(start.OperationId!.Value)!.Status);
	}
	[DataTestMethod]
	[DataRow("hidden")] [DataRow("closed")] [DataRow("unreadable")] [DataRow("full")]
	public void UnusableOrFullBooksRefuseWithoutFormulaOrPayment(string constraint)
	{
		var f = new ItemFixture(); var source = f.Book(100); var destination = f.Book(200); source.Component.AddFormula(2,f.F.Clock.Now.UtcDateTime,null);
		if (constraint == "hidden") f.F.Actor.Setup(x => x.CanSee(source.Item.Object,It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		if (constraint == "closed") { var open = new Mock<IOpenable>(); open.SetupGet(x => x.IsOpen).Returns(false); source.Item.Setup(x => x.GetItemType<IOpenable>()).Returns(open.Object); }
		if (constraint == "unreadable") source.Component.Prototype.BuildingCommand(f.F.Actor.Object,new StringStack("readable true"));
		if (constraint == "full") destination.Component.Prototype.BuildingCommand(f.F.Actor.Object,new StringStack("capacity 0"));
		Assert.IsFalse(f.F.Service.BeginTranscription(f.F.Actor.Object,f.F.Capability.Object,source.Item.Object,f.Spell,destination.Item.Object).Success);
		Assert.AreEqual(0,destination.Component.Formulae.Count); Assert.AreEqual(0,f.F.Store.Log.Count);
	}
	[TestMethod]
	public void AtWillInscriptionCanStockpileWhileSpontaneousInscriptionDebitsChosenSlot()
	{
		var f = new ItemFixture(); f.F.Select(2);
		var countProg = f.F.Allowances[0].SlotCountProgId;
		f.F.Allowances[0] = f.F.Allowances[0] with { Mode = VancianAllowanceMode.AtWill, SlotLevel = null, SlotCountProgId = 0 };
		foreach (var id in new long[] { 100,101 })
		{
			var scroll = f.Scroll(id); var begin = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,null,scroll.Item.Object);
			Assert.IsTrue(begin.Success,begin.Message); f.F.Clock.Advance(TimeSpan.FromSeconds(1));
			Assert.IsTrue(f.F.Service.CompleteWriting(f.F.Actor.Object,begin.OperationId!.Value).Success); Assert.IsTrue(scroll.Component.IsCharged);
		}
		Assert.AreEqual(0,f.F.State.Slots.Count);
		f.F.Allowances[0] = f.F.Allowances[0] with { Mode = VancianAllowanceMode.Spontaneous, SlotLevel = 1, SlotCountProgId = countProg };
		f.F.Refresh(); var blank = f.Scroll(102); var start = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,2,blank.Item.Object);
		Assert.IsTrue(start.Success,start.Message); f.F.Clock.Advance(TimeSpan.FromSeconds(1)); Assert.IsTrue(f.F.Service.CompleteWriting(f.F.Actor.Object,start.OperationId!.Value).Success);
		Assert.AreEqual(VancianSlotStatus.AvailableSpontaneous,f.F.State.Slots[0].Status); Assert.AreEqual(VancianSlotStatus.Spent,f.F.State.Slots[1].Status);
	}
	[TestMethod]
	public void InscriptionOfDamagingSpellNeverExecutesItsEffectAndMovedBlankCancels()
	{
		var f = new ItemFixture("<Effect type='damage'><DamageType>0</DamageType><DamageExpression>99</DamageExpression><Limb>-1</Limb></Effect>");
		f.F.Select(2); f.F.Plan(2,2); f.F.Refresh(); var scroll = f.Scroll(100);
		var begin = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,1,scroll.Item.Object);
		Assert.IsTrue(begin.Success,begin.Message); f.F.Clock.Advance(TimeSpan.FromSeconds(1)); Assert.IsTrue(f.F.Service.CompleteWriting(f.F.Actor.Object,begin.OperationId!.Value).Success);
		f.F.Actor.Verify(x => x.SufferDamage(It.IsAny<IDamage>()),Times.Never);
		var moved = f.Scroll(101); begin = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,2,moved.Item.Object);
		Assert.IsTrue(begin.Success,begin.Message); f.Items.RemoveAll(x => x.Id == moved.Item.Object.Id); f.F.Clock.Advance(TimeSpan.FromSeconds(1));
		Assert.IsFalse(f.F.Service.CompleteWriting(f.F.Actor.Object,begin.OperationId!.Value).Success); Assert.IsTrue(moved.Component.IsBlank); Assert.IsNull(moved.Component.Reservation);
		Assert.AreEqual(VancianSlotStatus.Prepared,f.F.State.Slots[1].Status);
	}
	[TestMethod]
	public void FailureAfterDurableInscriptionDebitCannotCreateOrRetryAFreeCharge()
	{
		var f = new ItemFixture(); f.F.Count = 1; f.F.Select(2); f.F.Plan(2); f.F.Refresh(); var scroll = f.Scroll(100);
		var begin = f.F.Service.BeginInscription(f.F.Actor.Object,f.F.Capability.Object,f.F.Rules[0].Key,f.F.Allowances[0].Key,f.Spell,1,scroll.Item.Object);
		Assert.IsTrue(begin.Success,begin.Message);
		f.F.Store.AfterCommit = operation => { if (operation is { Kind: "Inscription", Status: "Committing" }) throw new InvalidOperationException("Crash after durable debit"); };
		f.F.Clock.Advance(TimeSpan.FromSeconds(1)); Assert.IsFalse(f.F.Service.CompleteWriting(f.F.Actor.Object,begin.OperationId!.Value).Success);
		Assert.IsTrue(scroll.Component.IsBlank); Assert.AreEqual(VancianSlotStatus.Spent,f.F.State.Slots.Single().Status);
		Assert.AreEqual("Committing",f.F.Store.Operation(begin.OperationId!.Value)!.Status);
		Assert.IsFalse(f.F.Service.CompleteWriting(f.F.Actor.Object,begin.OperationId.Value).Success); Assert.IsFalse(f.F.Service.CancelWriting(f.F.Actor.Object,begin.OperationId.Value).Success);
	}
	[TestMethod]
	public void FailureAfterScrollConsumptionNeverResurrectsChargeOrDuplicatesFormula()
	{
		var f = new ItemFixture(); var scroll = f.Scroll(100); f.Charge(scroll.Component); var destination = f.Book(200); var saved = Serialize(scroll.Component);
		var begin = f.F.Service.BeginTranscription(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,f.Spell,destination.Item.Object); Assert.IsTrue(begin.Success,begin.Message);
		f.F.Persisted = component => { if (component is SpellScrollGameItemComponent { Spent: true }) throw new InvalidOperationException("Crash after consumption"); };
		f.F.Clock.Advance(TimeSpan.FromSeconds(1)); Assert.IsFalse(f.F.Service.CompleteWriting(f.F.Actor.Object,begin.OperationId!.Value).Success);
		Assert.AreEqual(0,destination.Component.Formulae.Count); Assert.AreEqual("NeedsReview",f.F.Store.Operation(begin.OperationId!.Value)!.Status);
		var reloaded = new SpellScrollGameItemComponent(new() { Id = 1, Definition = saved },(SpellScrollGameItemComponentProto)scroll.Component.Prototype,scroll.Item.Object);
		scroll.Item.Setup(x => x.GetItemType<ISpellScroll>()).Returns(reloaded);
		Assert.IsFalse(f.F.Service.ActivateScroll(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,new StringStack("")).Success);
		Assert.IsFalse(f.F.Service.BeginTranscription(f.F.Actor.Object,f.F.Capability.Object,scroll.Item.Object,f.Spell,destination.Item.Object).Success);
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class ItemMagicCastingTests
{
	private Func<XElement, IMagicSpell, IMagicSpellEffectTemplate>? _previous;
	private ItemMagicCastingFixture Fixture(bool glyph = false, bool material = true)
	{
		var f = new ItemMagicCastingFixture();
		_previous = ItemMagicCastingFixture.InstallRiskFactory(f.Risk);
		f.Load(glyph, material); return f;
	}
	[TestCleanup]
	public void RestoreFactory()
	{
		if (_previous is not null) ItemMagicCastingFixture.Factories["etherealitemattunement"] = _previous;
	}
	[TestMethod]
	[DataRow(true, 1)][DataRow(true, 4)][DataRow(true, 7)]
	[DataRow(false, 1)][DataRow(false, 4)][DataRow(false, 7)]
	public void FullCast_SelectedGrade_PaysAndConsumesOnceBeforeApplying(bool glyph, int grade)
	{
		var f = Fixture(glyph);
		var quote = f.Cast.Service.Quote(f.Intent(grade));
		Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(0, f.Events.Count); Assert.AreEqual(0, f.Risk.Draws);
		var result = f.Invoke(grade);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits); Assert.AreEqual(grade * 5.0, f.Debited);
		Assert.AreEqual(100 - grade * 5.0, f.Cast.Balances[f.Cast.Resources[1]]);
		Assert.AreEqual(1, f.Events.Count(x => x == "consume"));
		Assert.IsTrue(f.Events.IndexOf("debit") < f.Events.IndexOf("consume"));
		Assert.IsTrue(f.Events.IndexOf("consume") < f.Events.IndexOf("effect"));
		if (glyph)
		{
			var hazard = f.Effects.OfType<ItemPickupHazardEffect>().Single();
			Assert.AreEqual($"{grade}d4", hazard.DamageDice); Assert.AreEqual($"{grade}d6", hazard.StunDice);
			Assert.AreEqual(0, f.Risk.Draws);
		}
		else { Assert.AreEqual(1, f.Risk.Draws); Assert.IsTrue(f.Effects.OfType<EtherealItemAffinityEffect>().Single().IsReadyForTransition); }
		Assert.AreEqual("Completed", f.Cast.Store.Operation(result.OperationId!.Value)!.Stage);
	}
	[TestMethod]
	[DataRow(true, "hidden")][DataRow(false, "hidden")]
	[DataRow(true, "missing-material")][DataRow(false, "missing-material")]
	[DataRow(true, "funds")][DataRow(false, "funds")]
	[DataRow(true, "duplicate-specification")][DataRow(false, "duplicate-specification")]
	[DataRow(true, "already-enchanted")][DataRow(false, "already-enchanted")]
	public void FullCast_RejectedInput_LeavesResourcesMaterialsAndRiskUntouched(bool glyph, string invalid)
	{
		var f = Fixture(glyph);
		if (invalid == "hidden") f.Visible = false;
		if (invalid == "missing-material") f.Materials.Clear();
		if (invalid == "funds") f.Cast.Balances[f.Cast.Resources[1]] = 0;
		if (invalid == "already-enchanted")
			f.Effects.Add(glyph ? new ItemPickupHazardEffect(f.Target.Object, "1d4", "1d6") :
				new EtherealItemAffinityEffect(f.Target.Object, 9, true, true));
		f.AssertUnpaid(() => f.Cast.Service.Cast(f.Intent(target: invalid == "duplicate-specification" ? "focus focus" : "focus")));
	}
	[TestMethod]
	[DataRow(1, 3.0)][DataRow(4, 12.0)][DataRow(7, 21.0)]
	public void ShadowPassage_ExactWeightLimit_RefusesBeforePaymentAndRisk(int grade, double weight)
	{
		var f = Fixture(); f.Weight = weight; f.AssertUnpaid(() => f.Invoke(grade));
	}
	[TestMethod]
	[DataRow(1)][DataRow(4)][DataRow(7)]
	public void ShadowPassage_TwoStagesAndThirdRefusal_PayOnlyForEligibleStages(int grade)
	{
		var f = Fixture();
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Invoke(grade).Status);
		Assert.IsFalse(f.Effects.OfType<EtherealItemAffinityEffect>().Single().IsVisibleToThatPlane);
		f.AddMaterial(50);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Invoke(grade).Status);
		Assert.IsTrue(f.Effects.OfType<EtherealItemAffinityEffect>().Single().IsVisibleToThatPlane);
		f.AddMaterial(50); f.Risk.Value = 0;
		var before = f.Cast.Balances[f.Cast.Resources[1]];
		Assert.AreEqual(MagicCastingStatus.Refused, f.Invoke(grade).Status);
		Assert.AreEqual(2, f.Risk.Draws); Assert.IsFalse(f.Deleted);
		Assert.AreEqual(2, f.Debits); Assert.AreEqual(before, f.Cast.Balances[f.Cast.Resources[1]]);
		Assert.AreEqual(1, f.Materials.Count);
	}
	[TestMethod]
	[DataRow(0.0, true)][DataRow(0.039, true)][DataRow(0.04, false)]
	public void ShadowPassage_SelectedGradeDestructionRisk_IsSampledOnlyAfterPayment(double sample, bool destroyed)
	{
		var f = Fixture(); f.Risk.Value = sample;
		var result = f.Invoke(4);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(destroyed, f.Deleted); Assert.AreEqual(1, f.Risk.Draws);
		Assert.AreEqual(1, f.Debits); Assert.AreEqual(20.0, f.Debited);
		Assert.IsTrue(f.Events.IndexOf("consume") < f.Events.IndexOf("risk"));
	}
	[TestMethod]
	[DataRow("dispel")][DataRow("pickup")]
	public void Glyph_SpentSameItem_CanBeRecastWithFreshPayment(string spend)
	{
		var f = Fixture(true); Assert.AreEqual(MagicCastingStatus.Succeeded, f.Invoke().Status);
		var previous = f.Effects.OfType<ItemPickupHazardEffect>().Single();
		if (spend == "dispel") Assert.IsTrue(previous.TryDispel(3));
		else
		{
			// Native pickup spends the original grade dice even when this fixture has no HP strategy.
			Assert.IsFalse(ItemPickupAttempt.TryRun(f.Target.Object, f.Cast.Actor.Object, () => true));
		}
		Assert.IsFalse(previous.IsIntrinsicMagicActive);
		f.AddMaterial(50);
		var result = f.Invoke(4);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		var current = f.Effects.OfType<ItemPickupHazardEffect>().Single();
		Assert.AreNotSame(previous, current); Assert.IsTrue(current.IsIntrinsicMagicActive);
		Assert.AreEqual("4d4", current.DamageDice); Assert.AreEqual(2, f.Debits);
		Assert.AreEqual(25.0, f.Debited); Assert.AreEqual(2, f.Events.Count(x => x == "consume"));
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_ReplayedOrigin_NeverPaysOrAppliesTwice(bool glyph)
	{
		var f = Fixture(glyph); var intent = f.Intent(origin: Guid.NewGuid());
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Cast.Service.Cast(intent).Status);
		if (glyph) Assert.IsTrue(f.Effects.OfType<ItemPickupHazardEffect>().Single().TryDispel(3));
		f.AddMaterial(50);
		Assert.IsTrue(f.Cast.Service.Quote(intent).Allowed, "The spent glyph or next attunement stage is otherwise eligible.");
		Assert.AreEqual(MagicCastingStatus.Refused, f.Cast.Service.Cast(intent).Status);
		Assert.AreEqual(1, f.Debits); Assert.AreEqual(1, f.Materials.Count);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_MaterialChangesBeforePayment_RefusesWithoutDebit(bool glyph)
	{
		var f = Fixture(glyph);
		var material = Mock.Get(f.Materials.Single());
		f.Cast.Checkpoint = stage => { if (stage == "BeforePayment") material.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(false); };
		f.AssertUnpaid(() => f.Invoke());
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_MultipleMaterialsWouldExhaustTargetStack_RefusesBeforePayment(bool glyph)
	{
		var f = Fixture(glyph);
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		f.Cast.Body.SetupGet(x => x.Actor).Returns(f.Cast.Actor.Object);
		f.Target.SetupGet(x => x.InInventoryOf).Returns(f.Cast.Body.Object);
		f.Target.SetupGet(x => x.Location).Returns((MudSharp.Construction.IRoom)null!);
		f.Target.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		var stack = new Mock<IStackable>(); stack.SetupProperty(x => x.Quantity, 2);
		f.Target.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
		f.Target.SetupGet(x => x.Quantity).Returns(() => stack.Object.Quantity);
		var action = XElement.Parse("<Action state='consumed' tag='50' quantity='1' carriedonly='true'/>");
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			new[] { InventoryPlanAction.LoadAction(action, f.Cast.World.Object), InventoryPlanAction.LoadAction(action, f.Cast.World.Object) });
		f.AssertUnpaid(() => f.Invoke());
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_TargetIsConsumedComponent_RefusesBeforePayment(bool glyph)
	{
		var f = Fixture(glyph);
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		f.Cast.Body.SetupGet(x => x.Actor).Returns(f.Cast.Actor.Object);
		f.Target.SetupGet(x => x.InInventoryOf).Returns(f.Cast.Body.Object);
		f.Target.SetupGet(x => x.Location).Returns((MudSharp.Construction.IRoom)null!);
		f.Target.Setup(x => x.IsA(It.IsAny<MudSharp.Framework.ITag>())).Returns(true);
		f.AssertUnpaid(() => f.Invoke());
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_TargetIsRetainedComponent_RemainsEligible(bool glyph)
	{
		var f = Fixture(glyph);
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		f.Cast.Body.SetupGet(x => x.Actor).Returns(f.Cast.Actor.Object);
		f.Target.SetupGet(x => x.InInventoryOf).Returns(f.Cast.Body.Object);
		f.Target.SetupGet(x => x.Location).Returns((MudSharp.Construction.IRoom)null!);
		f.Target.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			InventoryPlanAction.LoadAction(XElement.Parse("<Action state='retained' tag='50' quantity='1' carriedonly='true'/>"), f.Cast.World.Object));
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits); Assert.IsFalse(f.Deleted);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_PartiallyConsumedTargetStack_RefusesBeforePayment(bool glyph)
	{
		var f = Fixture(glyph);
		CarryTarget(f);
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		f.Target.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		var stack = new Mock<IStackable>(); stack.SetupProperty(x => x.Quantity, 2);
		f.Target.Setup(x => x.GetItemType<IStackable>()).Returns(stack.Object);
		f.Target.SetupGet(x => x.Quantity).Returns(() => stack.Object.Quantity);
		f.Target.SetupGet(x => x.Weight).Returns(() => (double)stack.Object.Quantity);
		AssertMaterialTargetUnpaid(f);
		Assert.AreEqual(2, stack.Object.Quantity);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_ConsumedTargetLiquid_RefusesBeforePayment(bool glyph)
	{
		var f = Fixture(glyph);
		CarryTarget(f);
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		f.Target.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		var liquid = Mock.Of<ILiquid>(x => x.Id == 1);
		f.Cast.World.SetupGet(x => x.Liquids).Returns(MagicCastingFixture.Collection(() => new[] { liquid }));
		var contents = new LiquidMixture(new[] { new LiquidInstance { Liquid = liquid, Amount = 2 } }, f.Cast.World.Object);
		var requested = new LiquidMixture(new[] { new LiquidInstance { Liquid = liquid, Amount = 1 } }, f.Cast.World.Object);
		var container = new Mock<ILiquidContainer>();
		container.SetupGet(x => x.Parent).Returns(f.Target.Object);
		container.SetupGet(x => x.OwnsLiquidMixture).Returns(true);
		container.SetupGet(x => x.IsOpen).Returns(true);
		container.SetupProperty(x => x.LiquidMixture, contents);
		f.Target.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container.Object);
		f.Target.SetupGet(x => x.Weight).Returns(() => container.Object.LiquidMixture?.TotalVolume ?? 0);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			new InventoryPlanActionConsumeLiquid(f.Cast.World.Object, 0, 0, _ => true, _ => true, _ => true, requested));
		AssertMaterialTargetUnpaid(f);
		Assert.AreEqual(2.0, contents.TotalVolume);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_ConsumedTargetCommodity_RefusesBeforePayment(bool glyph)
	{
		var f = Fixture(glyph);
		CarryTarget(f);
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		f.Cast.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Parent).Returns(f.Target.Object);
		commodity.SetupGet(x => x.Material).Returns(solid);
		commodity.SetupGet(x => x.Weight).Returns(() => f.Weight);
		f.Target.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		f.Target.SetupSet(x => x.Weight = It.IsAny<double>()).Callback<double>(weight => f.Weight = weight);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true));
		AssertMaterialTargetUnpaid(f);
		Assert.AreEqual(2.0, f.Weight);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_ConsumedAncestorOfTarget_RefusesBeforePayment(bool glyph)
	{
		var f = Fixture(glyph);
		f.Materials.Clear();
		var parent = f.AddMaterial(50);
		parent.SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(1));
		var contents = new List<IGameItem> { f.Target.Object };
		var container = Mock.Of<IContainer>(x => x.Parent == parent.Object && x.Contents == contents);
		parent.Setup(x => x.GetItemType<IContainer>()).Returns(container);
		f.Cast.Body.SetupGet(x => x.Actor).Returns(f.Cast.Actor.Object);
		f.Target.SetupGet(x => x.ContainedIn).Returns(parent.Object);
		f.Target.SetupGet(x => x.Location).Returns((MudSharp.Construction.IRoom)null!);
		AssertMaterialTargetUnpaid(f);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_ConsumedMaterialWithinTarget_RefusesBeforePayment(bool glyph)
	{
		var f = Fixture(glyph);
		CarryTarget(f);
		f.Materials.Clear();
		var material = f.AddMaterial(50);
		var contents = new List<IGameItem> { material.Object };
		var container = Mock.Of<IContainer>(x => x.Parent == f.Target.Object && x.Contents == contents);
		f.Target.Setup(x => x.GetItemType<IContainer>()).Returns(container);
		material.SetupGet(x => x.InInventoryOf).Returns((MudSharp.Body.IBody)null!);
		material.SetupGet(x => x.ContainedIn).Returns(f.Target.Object);
		material.Setup(x => x.Delete()).Callback(() => { f.Events.Add("consume"); contents.Clear(); f.Weight = 1; });
		f.Materials.Clear(); f.Materials.Add(f.Target.Object);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			InventoryPlanAction.LoadAction(XElement.Parse("<Action state='consumed' tag='50' quantity='1'/>"), f.Cast.World.Object));
		AssertMaterialTargetUnpaid(f);
		Assert.AreEqual(1, contents.Count);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_ConsumedSiblingMaterial_PreservesTargetAndPaysNormally(bool glyph)
	{
		var f = Fixture(glyph);
		var material = Mock.Get(f.Materials.Single());
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(2001);
		parent.SetupGet(x => x.Gameworld).Returns(f.Cast.World.Object);
		parent.SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(1));
		parent.SetupGet(x => x.InInventoryOf).Returns(f.Cast.Body.Object);
		var contents = new List<IGameItem> { f.Target.Object, material.Object };
		var container = Mock.Of<IContainer>(x => x.Parent == parent.Object && x.Contents == contents);
		parent.Setup(x => x.GetItemType<IContainer>()).Returns(container);
		f.Cast.Body.SetupGet(x => x.Actor).Returns(f.Cast.Actor.Object);
		f.Target.SetupGet(x => x.ContainedIn).Returns(parent.Object);
		f.Target.SetupGet(x => x.Location).Returns((MudSharp.Construction.IRoom)null!);
		material.SetupGet(x => x.InInventoryOf).Returns((MudSharp.Body.IBody)null!);
		material.SetupGet(x => x.ContainedIn).Returns(parent.Object);
		f.Registry.Add(parent.Object);
		f.Materials.Clear(); f.Materials.Add(parent.Object);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			InventoryPlanAction.LoadAction(XElement.Parse("<Action state='consumed' tag='50' quantity='1'/>"), f.Cast.World.Object));
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits); Assert.AreEqual(1, f.Events.Count(x => x == "consume"));
		Assert.IsFalse(f.Deleted);
	}
	private static void AssertMaterialTargetUnpaid(ItemMagicCastingFixture f)
	{
		var message = "";
		f.AssertUnpaid(() => { var result = f.Invoke(); message = result.Message; return result; });
		StringAssert.Contains(message, "prepared item target");
	}
	private static void CarryTarget(ItemMagicCastingFixture f)
	{
		f.Cast.Body.SetupGet(x => x.Actor).Returns(f.Cast.Actor.Object);
		f.Target.SetupGet(x => x.InInventoryOf).Returns(f.Cast.Body.Object);
		f.Target.SetupGet(x => x.Location).Returns((MudSharp.Construction.IRoom)null!);
	}
	[TestMethod]
	public void Glyph_ConfiguredDispelThenRecast_UsesFreshGradeAndSeparatePayments()
	{
		var f = Fixture(true);
		Assert.AreEqual(MagicCastingStatus.Succeeded, f.Invoke().Status);
		var hazard = f.Effects.OfType<ItemPickupHazardEffect>().Single();
		var dispel = f.Cast.NewSpell(3, "Intrinsic Dispel", "<Effect type='dispelmagic'><Mode>0</Mode><CasterPolicy>1</CasterPolicy><AllowHostile>true</AllowHostile><EffectKey>any</EffectKey><IncludeIntrinsicItemEffects>true</IncludeIntrinsicItemEffects></Effect>");
		Assert.IsTrue(dispel.BuildingCommand(f.Cast.Actor.Object, new StringStack("trigger new item")));
		Assert.IsTrue(dispel.BuildingCommand(f.Cast.Actor.Object, new StringStack("grades fixture")));
		Assert.IsTrue(f.Cast.Earth.BuildingCommand(f.Cast.Actor.Object, new StringStack("casting entry add 3")));
		f.Cast.Store.Write(acquired: new(100, 3, 7, 1, f.Cast.Now, "fixture", DateTime.UnixEpoch, 0));
		var result = f.Cast.Service.Cast(new(f.Cast.Actor.Object, f.Cast.Earth.Id, 3, 3, false, "focus"));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.IsFalse(hazard.IsIntrinsicMagicActive);
		f.AddMaterial(50);
		result = f.Invoke(4);
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual("4d4", f.Effects.OfType<ItemPickupHazardEffect>().Single().DamageDice);
		Assert.AreEqual(3, f.Debits); Assert.AreEqual(40.0, f.Debited);
	}
	[TestMethod]
	[DataRow(true)][DataRow(false)]
	public void FullCast_FailedNativeCastingCheck_ConsumesPaymentWithoutEnchanting(bool glyph)
	{
		var f = Fixture(glyph); f.Cast.Outcome = Outcome.MajorFail;
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Failed, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits); Assert.AreEqual(1, f.Events.Count(x => x == "consume"));
		Assert.AreEqual(0, f.Effects.Count); Assert.AreEqual(0, f.Risk.Draws);
	}
}

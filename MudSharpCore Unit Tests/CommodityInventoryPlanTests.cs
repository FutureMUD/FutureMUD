#nullable enable

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass, DoNotParallelize]
public class CommodityInventoryPlanTests
{
	private static ItemMagicCastingFixture Fixture()
	{
		var fixture = new ItemMagicCastingFixture();
		fixture.Cast.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection<ITag>(() => []));
		return fixture;
	}
	[TestMethod]
	[DataRow(2.0, false)]
	[DataRow(1.0, true)]
	public void Plan_PreviewAndExecution_ConsumeTheRequestedMassExactlyOnce(double initialWeight, bool exhausted)
	{
		var f = Fixture();
		var item = f.AddMaterial(0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		var weight = initialWeight;
		item.SetupGet(x => x.Weight).Returns(() => weight);
		var commodity = Mock.Of<ICommodity>(x => x.Parent == item.Object && x.Material == solid);
		Mock.Get(commodity).SetupGet(x => x.Weight).Returns(() => weight);
		Mock.Get(commodity).SetupSet(x => x.Weight = It.IsAny<double>()).Callback<double>(value => { f.Events.Add("consume"); weight = value; });
		Mock.Get(commodity).SetupGet(x => x.Tag).Returns((ITag)null!);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity);
		var action = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true)
			{ OriginalReference = "component" };
		var plan = new InventoryPlanTemplate(f.Cast.World.Object, action).CreatePlan(f.Cast.Actor.Object);
		var preview = plan.PeekPlanResults().Single();
		Assert.AreEqual(DesiredItemState.ConsumeCommodity, preview.ActionState);
		Assert.AreSame(item.Object, preview.PrimaryTarget);
		Assert.AreEqual("component", preview.OriginalReference);
		Assert.AreEqual(initialWeight, weight);
		item.Verify(x => x.Delete(), Times.Never);
		var result = plan.ExecuteWholePlan().Single();
		Assert.AreEqual(DesiredItemState.Consumed, result.ActionState);
		Assert.AreSame(item.Object, result.PrimaryTarget);
		Assert.AreEqual("component", result.OriginalReference);
		item.Verify(x => x.Delete(), exhausted ? Times.Once : Times.Never);
		if (!exhausted) Assert.AreEqual(initialWeight - 1, weight);
		plan.FinalisePlanNoRestore();
	}

	[TestMethod]
	[DataRow("held")][DataRow("inventory-container")][DataRow("room")][DataRow("room-container")]
	public void ScoutTarget_AllLocations_RequireMaterialTagAndSufficientMass(string location)
	{
		var f = Fixture();
		var item = f.AddMaterial(0);
		var wanted = Mock.Of<ISolid>(x => x.Id == 1);
		var other = Mock.Of<ISolid>(x => x.Id == 2);
		var tag = Mock.Of<ITag>(x => x.Id == 3);
		f.Cast.World.SetupGet(x => x.Tags).Returns(MagicCastingFixture.Collection(() => new[] { tag }));
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Parent).Returns(item.Object);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		var containerItem = new Mock<IGameItem>();
		var container = Mock.Of<IContainer>(x => x.Parent == containerItem.Object && x.Contents == new[] { item.Object });
		containerItem.Setup(x => x.GetItemType<IContainer>()).Returns(container);
		f.Materials.Clear();
		f.Room.Setup(x => x.LayerGameItems(f.Cast.Actor.Object.RoomLayer)).Returns(
			location == "room" ? new[] { item.Object } : location == "room-container" ? new[] { containerItem.Object } : []);
		if (location == "held") f.Materials.Add(item.Object);
		if (location == "inventory-container") f.Materials.Add(containerItem.Object);
		var action = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, wanted, 0, _ => true, _ => true);
		commodity.SetupGet(x => x.Material).Returns(wanted);
		commodity.SetupGet(x => x.Weight).Returns(0.5);
		Assert.IsNull(action.ScoutTarget(f.Cast.Actor.Object), "Untagged piles must still have enough mass.");
		commodity.SetupGet(x => x.Weight).Returns(2);
		Assert.AreSame(item.Object, action.ScoutTarget(f.Cast.Actor.Object));
		commodity.SetupGet(x => x.Material).Returns(other);
		commodity.SetupGet(x => x.Tag).Returns(tag);
		Mock.Get(tag).Setup(x => x.IsA(tag)).Returns(true);
		action = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, wanted, tag.Id, _ => true, _ => true);
		Assert.IsNull(action.ScoutTarget(f.Cast.Actor.Object), "A matching tag must not admit another material.");
		commodity.SetupGet(x => x.Material).Returns(wanted);
		Assert.AreSame(item.Object, action.ScoutTarget(f.Cast.Actor.Object));
		commodity.SetupGet(x => x.Weight).Returns(0.5);
		Assert.IsNull(action.ScoutTarget(f.Cast.Actor.Object));
	}

	[TestMethod]
	[DataRow(2.0, false)][DataRow(1.0, true)]
	public void Cast_CommodityComponent_PaysAndConsumesBeforeApplyingGlyph(double initialWeight, bool exhausted)
	{
		var f = Fixture();
		f.Load(true);
		var item = f.AddMaterial(0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		f.Cast.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var weight = initialWeight;
		item.SetupGet(x => x.Weight).Returns(() => weight);
		var commodity = Mock.Of<ICommodity>(x => x.Parent == item.Object && x.Material == solid);
		Mock.Get(commodity).SetupGet(x => x.Weight).Returns(() => weight);
		Mock.Get(commodity).SetupSet(x => x.Weight = It.IsAny<double>()).Callback<double>(value => { f.Events.Add("consume"); weight = value; });
		Mock.Get(commodity).SetupGet(x => x.Tag).Returns((ITag)null!);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true));
		var quote = f.Cast.Service.Quote(f.Intent());
		Assert.IsTrue(quote.Allowed, quote.Reason);
		Assert.AreEqual(initialWeight, weight);
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits);
		Assert.AreEqual(1, f.Events.Count(x => x == "consume"));
		Assert.IsTrue(f.Events.IndexOf("debit") < f.Events.IndexOf("consume"));
		Assert.IsTrue(f.Events.IndexOf("consume") < f.Events.IndexOf("effect"));
		item.Verify(x => x.Delete(), exhausted ? Times.Once : Times.Never);
		if (!exhausted) Assert.AreEqual(initialWeight - 1, weight);
	}

	[TestMethod]
	public void SaveToXml_FractionalMass_ReloadsWithoutLoss()
	{
		var world = new MagicCastingFixture().World;
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		world.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var action = new InventoryPlanActionConsumeCommodity(world.Object, 0.25, solid, 0, _ => true, _ => true);
		var reloaded = new InventoryPlanActionConsumeCommodity(XElement.Parse(action.SaveToXml().ToString()), world.Object);
		Assert.AreEqual(0.25, reloaded.Weight);
	}

	[TestMethod]
	[DataRow(1.5, false, false)][DataRow(1.5, true, false)]
	[DataRow(2.0, false, true)][DataRow(2.0, true, true)]
	public void Cast_RepeatedCommodityRequirements_RequireTheCombinedMass(double initialWeight, bool separatePhases, bool sufficient)
	{
		var f = Fixture();
		f.Load(true);
		var item = f.AddMaterial(0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		f.Cast.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var weight = initialWeight;
		item.SetupGet(x => x.Weight).Returns(() => weight);
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Parent).Returns(item.Object);
		commodity.SetupGet(x => x.Material).Returns(solid);
		commodity.SetupGet(x => x.Weight).Returns(() => weight);
		commodity.SetupSet(x => x.Weight = It.IsAny<double>()).Callback<double>(value => { f.Events.Add("consume"); weight = value; });
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		var action = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true);
		var other = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true);
		f.Spell.InventoryPlanTemplate = separatePhases
			? new InventoryPlanTemplate(f.Cast.World.Object, new[] {
				new InventoryPlanPhaseTemplate(1, new[] { action }), new InventoryPlanPhaseTemplate(2, new[] { other }) })
			: new InventoryPlanTemplate(f.Cast.World.Object, new[] { action, other });
		if (!sufficient)
		{
			f.AssertUnpaid(() => f.Invoke());
			Assert.AreEqual(initialWeight, weight);
			item.Verify(x => x.Delete(), Times.Never);
			return;
		}
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(1, f.Debits);
		Assert.AreEqual(2, f.Events.Count(x => x == "consume"));
		item.Verify(x => x.Delete(), Times.Once);
	}

	[TestMethod]
	[DataRow(false)][DataRow(true)]
	public void Plan_CombinedMass_ReportsInfeasibleAcrossTheSelectedPhases(bool separatePhases)
	{
		var f = Fixture();
		var item = f.AddMaterial(0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Parent).Returns(item.Object);
		commodity.SetupGet(x => x.Material).Returns(solid);
		commodity.SetupGet(x => x.Weight).Returns(1.5);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		var first = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true);
		var second = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true);
		var template = separatePhases
			? new InventoryPlanTemplate(f.Cast.World.Object, new[] {
				new InventoryPlanPhaseTemplate(1, new[] { first }), new InventoryPlanPhaseTemplate(2, new[] { second }) })
			: new InventoryPlanTemplate(f.Cast.World.Object, new[] { first, second });
		var plan = template.CreatePlan(f.Cast.Actor.Object);
		Assert.AreEqual(InventoryPlanFeasibility.NotFeasibleMissingItems, plan.PlanIsFeasible());
		Assert.AreEqual(InventoryPlanFeasibility.NotFeasibleMissingItems, plan.PlanIsFeasible(1));
		Assert.AreEqual(InventoryPlanFeasibility.NotFeasibleMissingItems, plan.PlanIsFeasible(1, separatePhases ? 2 : 1));
		Assert.AreEqual(separatePhases ? InventoryPlanFeasibility.Feasible : InventoryPlanFeasibility.NotFeasibleMissingItems,
			plan.CurrentPhaseIsFeasible());
		Assert.AreEqual(2, plan.InfeasibleActions().Count());
		item.Verify(x => x.Delete(), Times.Never);
		plan.FinalisePlanNoRestore();
	}

	[TestMethod]
	public void Execute_ScoutedPileShrinks_RefusesWithoutDeletingOrClaimingConsumption()
	{
		var f = Fixture();
		var item = f.AddMaterial(0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		var weight = 2.0;
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Parent).Returns(item.Object);
		commodity.SetupGet(x => x.Material).Returns(solid);
		commodity.SetupGet(x => x.Weight).Returns(() => weight);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		var action = new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true);
		var plan = new InventoryPlanTemplate(f.Cast.World.Object, action).CreatePlan(f.Cast.Actor.Object);
		Assert.AreSame(item.Object, plan.PeekPlanResults().Single().PrimaryTarget);
		weight = 0.5;
		Assert.ThrowsExactly<InvalidOperationException>(() => plan.ExecuteWholePlan().ToArray());
		Assert.AreEqual(0.5, weight);
		item.Verify(x => x.Delete(), Times.Never);
		item.VerifySet(x => x.Weight = It.IsAny<double>(), Times.Never);
		plan.FinalisePlanNoRestore();
	}

	[TestMethod]
	[DataRow("BeforePayment", 1.5, MagicCastingStatus.Refused)]
	[DataRow("PaymentMutated", 0.5, MagicCastingStatus.NeedsReview)]
	public void Cast_CommodityChangesDuringPayment_NeverAppliesAnUnfundedGlyph(string checkpoint, double changedWeight, MagicCastingStatus expected)
	{
		var f = Fixture();
		f.Load(true);
		var item = f.AddMaterial(0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		f.Cast.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var weight = 2.0;
		item.SetupGet(x => x.Weight).Returns(() => weight);
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Parent).Returns(item.Object);
		commodity.SetupGet(x => x.Material).Returns(solid);
		commodity.SetupGet(x => x.Weight).Returns(() => weight);
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object,
			new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true));
		f.Cast.Checkpoint = stage => { if (stage == checkpoint) weight = changedWeight; };
		var result = f.Invoke();
		Assert.AreEqual(expected, result.Status, result.Message);
		Assert.AreEqual(expected == MagicCastingStatus.Refused ? 0 : 1, f.Debits);
		Assert.AreEqual(0, f.Effects.Count);
		item.Verify(x => x.Delete(), Times.Never);
		item.VerifySet(x => x.Weight = It.IsAny<double>(), Times.Never);
	}

	[TestMethod]
	[DataRow(0.3, true)][DataRow(0.299, false)]
	public void Cast_FractionalCommodityTotal_AllowsOnlyFloatingPointRoundoff(double initialWeight, bool sufficient)
	{
		var f = Fixture();
		f.Load(true);
		var item = f.AddMaterial(0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		f.Cast.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var weight = initialWeight;
		item.SetupGet(x => x.Weight).Returns(() => weight);
		var commodity = new Mock<ICommodity>();
		commodity.SetupGet(x => x.Parent).Returns(item.Object);
		commodity.SetupGet(x => x.Material).Returns(solid);
		commodity.SetupGet(x => x.Weight).Returns(() => weight);
		commodity.SetupSet(x => x.Weight = It.IsAny<double>()).Callback<double>(value => { f.Events.Add("consume"); weight = value; });
		item.Setup(x => x.GetItemType<ICommodity>()).Returns(commodity.Object);
		f.Spell.InventoryPlanTemplate = new InventoryPlanTemplate(f.Cast.World.Object, new[] {
			new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 0.1, solid, 0, _ => true, _ => true),
			new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 0.2, solid, 0, _ => true, _ => true) });
		if (!sufficient) { f.AssertUnpaid(() => f.Invoke()); return; }
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(2, f.Events.Count(x => x == "consume"));
		item.Verify(x => x.Delete(), Times.Once);
	}

	[TestMethod]
	public void Plan_NativeCommodityPartialPayment_ChangesPersistedComponentMass()
	{
		var f = Fixture();
		f.Cast.World.SetupGet(x => x.UnitManager.BaseWeightToKilograms).Returns(1.0);
		f.Cast.World.SetupGet(x => x.UnitManager.BaseFluidToLitres).Returns(1.0);
		var solid = Mock.Of<ISolid>(x => x.Id == 1);
		f.Cast.World.SetupGet(x => x.Materials).Returns(MagicCastingFixture.Collection(() => new[] { solid }));
		var itemProto = new Mock<IGameItemProto>();
		itemProto.SetupGet(x => x.Gameworld).Returns(f.Cast.World.Object);
		itemProto.SetupGet(x => x.Name).Returns("native commodity");
		itemProto.SetupGet(x => x.Components).Returns([]);
		var item = new GameItem(itemProto.Object);
		var proto = (CommodityGameItemComponentProto)Activator.CreateInstance(typeof(CommodityGameItemComponentProto),
			BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] {
				new MudSharp.Models.GameItemComponentProto { Id = 1, Name = "Commodity", Definition = "", EditableItem = new MudSharp.Models.EditableItem() },
				f.Cast.World.Object }, null)!;
		var commodity = new CommodityGameItemComponent(proto, item, true) { Material = solid, Weight = 2 };
		((List<IGameItemComponent>)typeof(GameItem).GetField("_components", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(item)!).Add(commodity);
		f.Materials.Add(item);
		Assert.AreEqual(2.0, item.Weight);
		item.Weight = 99;
		Assert.AreEqual(2.0, item.Weight, "Native item weight is derived; the component owns its persisted mass.");
		var plan = new InventoryPlanTemplate(f.Cast.World.Object,
			new InventoryPlanActionConsumeCommodity(f.Cast.World.Object, 1, solid, 0, _ => true, _ => true)).CreatePlan(f.Cast.Actor.Object);
		Assert.AreEqual(InventoryPlanFeasibility.Feasible, plan.PlanIsFeasible());
		Assert.AreEqual(DesiredItemState.Consumed, plan.ExecuteWholePlan().Single().ActionState);
		Assert.AreEqual(1.0, commodity.Weight);
		Assert.AreEqual(1.0, item.Weight);
		var xml = (string)typeof(CommodityGameItemComponent).GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(commodity, null)!;
		var loaded = new CommodityGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 1, Definition = xml }, proto, item);
		Assert.AreEqual(1.0, loaded.Weight);
		plan.FinalisePlanNoRestore();
	}
}

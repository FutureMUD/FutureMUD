#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellCreatedConsumableTests
{
	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void EmptyInventoryPlan_Finalization_DoesNotAccessActorEffects(bool restore)
	{
		var actor = new Mock<ICharacter>(MockBehavior.Strict);
		var template = new InventoryPlanTemplate(Mock.Of<IFuturemud>(), Array.Empty<IInventoryPlanAction>());
		var plan = template.CreatePlan(actor.Object);
		if (restore) plan.FinalisePlan();
		else plan.FinalisePlanNoRestore();
		actor.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void OwnedInventoryPlan_Finalization_RemovesItsEffectAndPreservesForeignEffects()
	{
		var actor = new Mock<ICharacter>(MockBehavior.Strict);
		actor.SetupGet(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		var template = new InventoryPlanTemplate(actor.Object.Gameworld, Array.Empty<IInventoryPlanAction>());
		var plan = template.CreatePlan(actor.Object);
		var owned = new InventoryPlanItemEffect(actor.Object, plan);
		var foreign = Mock.Of<IEffect>();
		var effects = new List<IEffect> { owned, foreign };
		actor.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), false))
			.Callback<Predicate<IEffect>, bool>((predicate, _) => effects.RemoveAll(x => predicate(x)));
		plan.FinalisePlanNoRestore();
		Assert.AreEqual(1, effects.Count);
		Assert.AreSame(foreign, effects[0]);
		actor.Verify(x => x.RemoveAllEffects(It.IsAny<Predicate<IEffect>>(), false), Times.Once);
	}

	[TestMethod]
	public void ItemTrigger_BuilderCloneAndReload_RetainsItemTargeting()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var spell = Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object);
		var (trigger, error) = SpellTriggerFactory.LoadTriggerFromBuilderInput("item", new StringStack(""), spell);
		Assert.IsTrue(string.IsNullOrEmpty(error));
		Assert.IsInstanceOfType(trigger.Clone(), typeof(CastingTriggerItem));
		var reloaded = SpellTriggerFactory.LoadTrigger(trigger.SaveToXml(), spell);
		Assert.IsInstanceOfType(reloaded, typeof(CastingTriggerItem));
		Assert.AreEqual("item", reloaded.TargetTypes);
	}

	[DataTestMethod]
	[DataRow(1.25)]
	[DataRow(0.0)]
	public void Food_LoadSavedFractionOrExhaustedRemainder_PreservesQuantityWithoutDeletingDuringLoad(double remaining)
	{
		var proto = (FoodGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(FoodGameItemComponentProto));
		typeof(FoodGameItemComponentProto).GetProperty(nameof(proto.Bites))!.SetValue(proto, 4.0);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var parent = new Mock<IGameItem>(); parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		var food = new FoodGameItemComponent(new MudSharp.Models.GameItemComponent
		{ Id = 21, Definition = new XElement("Definition", new XElement("Bites", remaining)).ToString() }, proto, parent.Object);
		Assert.AreEqual(remaining, food.BitesRemaining);
		parent.Verify(x => x.Delete(), Times.Never);
		var xml = (string)typeof(FoodGameItemComponent).GetMethod("SaveToXml", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(food, null)!;
		Assert.AreEqual(remaining, (double)XElement.Parse(xml).Element("Bites")!);
	}

	[TestMethod]
	public void Food_HeldDeletion_SchedulesExhaustedRemainderWithoutRestoringPortion()
	{
		var proto = (FoodGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(FoodGameItemComponentProto));
		typeof(FoodGameItemComponentProto).GetProperty(nameof(proto.Bites))!.SetValue(proto, 4.0);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var parent = new Mock<IGameItem>(); parent.SetupGet(x => x.Gameworld).Returns(world.Object);
		var food = new FoodGameItemComponent(proto, parent.Object, temporary: true);
		food.SetNoSave(false); food.Changed = false;
		food.Eat(Mock.Of<IBody>(), 4);
		Assert.AreEqual(0.0, food.BitesRemaining); Assert.IsTrue(food.Changed);
		parent.Verify(x => x.Delete(), Times.Once);
		world.Verify(x => x.SaveManager.Add(food), Times.Once);
	}

	[DataTestMethod]
	[DataRow(0.0, 0.0)]
	[DataRow(4.0, -1.0)]
	[DataRow(4.0, double.NaN)]
	[DataRow(double.PositiveInfinity, 1.0)]
	public void Body_InvalidFoodPortion_RefusesBeforeAccessOrNutrition(double remaining, double bites)
	{
		var body = (MudSharp.Body.Implementations.Body)RuntimeHelpers.GetUninitializedObject(typeof(MudSharp.Body.Implementations.Body));
		var food = Mock.Of<IEdible>(x => x.TotalBites == 4 && x.BitesRemaining == remaining);
		Assert.IsFalse(body.CanEat(food, null, null, bites));
	}

	[DataTestMethod]
	[DataRow(0.0, 10.0, 6.0)]
	[DataRow(8.0, 10.0, 2.0)]
	[DataRow(10.0, 10.0, 0.0)]
	public void LegacyLiquid_EmptyPartialAndFullContainers_InitializesOrClampsWithoutReplacingContents(double existing, double capacity, double added)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var liquid = Mock.Of<ILiquid>(x => x.Id == 1);
		var catalogue = new All<ILiquid>(); catalogue.Add(liquid);
		world.SetupGet(x => x.Liquids).Returns(catalogue);
		world.SetupGet(x => x.UnitManager).Returns(Mock.Of<IUnitManager>(x => x.BaseFluidToLitres == 0.001 && x.BaseWeightToKilograms == 1));
		var container = new Mock<ILiquidContainer>(); container.SetupGet(x => x.LiquidCapacity).Returns(capacity);
		var original = existing == 0 ? null : new LiquidMixture(liquid, existing, world.Object);
		container.SetupGet(x => x.LiquidMixture).Returns(original!);
		LiquidMixture? incoming = null;
		container.Setup(x => x.MergeLiquid(It.IsAny<LiquidMixture>(), It.IsAny<ICharacter>(), "spell")).Callback<LiquidMixture, ICharacter, string>((x, _, _) => incoming = x);
		var target = new Mock<IGameItem>(); target.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container.Object);
		var effect = new LiquidEffect(new XElement("Effect", new XAttribute("type", "createliquid"), new XElement("LiquidId", 1), new XElement("AmountFormula", "6")),
			Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object));
		effect.GetOrApplyEffect(Mock.Of<ICharacter>(), target.Object, default, default, null!, []);
		Assert.AreEqual(added, incoming?.TotalVolume ?? 0);
		Assert.AreSame(original, container.Object.LiquidMixture);
	}

	[DataTestMethod]
	[DataRow("version")]
	[DataRow("multiplier")]
	[DataRow("liquid")]
	public void Liquid_MalformedFillSchema_ClonesAsRefusedWithoutLegacyFallback(string field)
	{
		var xml = new XElement("Effect", new XAttribute("type", "createliquid"), new XElement("LiquidId", 1), new XElement("AmountFormula", new XCData("6")),
			new XElement("ContainerFill", new XAttribute("version", field == "version" ? 99 : 1), new XElement("Litres", "grade"),
				new XElement("CompatibleLiquid", field == "liquid" ? "bad" : "2"), new XElement("BonusPlane", new XAttribute("multiplier", field == "multiplier" ? "NaN" : "2"), 1)));
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var effect = new LiquidEffect(xml, Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object));
		Assert.IsNotNull(effect.DefinitionError); Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
		Assert.IsNotNull(((CreateLiquidEffect)effect.Clone()).DefinitionError);
		Assert.IsFalse(effect.TryPrepareApplication(Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), default, default, TimeSpan.Zero, out _, out _));
	}

	[DataTestMethod]
	[DataRow("Count", "eight")]
	[DataRow("Placement", "glow")]
	public void Item_MalformedCountOrPlacement_ClonesAsRefusedWithoutLegacyFallback(string field, string value)
	{
		var xml = new XElement("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", new XCData("base")), new XElement("ItemPrototypeId", 1),
			new XElement("ItemSkinId", 0), new XElement("Quantity", 1), new XElement("LoadString", new XCData("")),
			new XElement("Lifecycle", new XAttribute("version", 1), new XAttribute("mode", "TemporaryCleanup"), new XElement("Family", "food"), new XElement(field, value)));
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var effect = new ItemEffect(xml, Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object));
		Assert.IsNotNull(effect.DefinitionError); Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
		Assert.IsNotNull(((CreateItemEffect)effect.Clone()).DefinitionError);
	}

	private sealed class LiquidEffect(XElement root, IMagicSpell spell) : CreateLiquidEffect(root, spell);
	private sealed class ItemEffect(XElement root, IMagicSpell spell) : CreateItemEffect(root, spell);
}

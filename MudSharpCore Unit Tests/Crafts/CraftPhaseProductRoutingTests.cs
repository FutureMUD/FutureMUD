#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests.Crafts;

[TestClass]
public class CraftPhaseProductRoutingTests
{
	[DataTestMethod]
	[DataRow(false, 3)]
	[DataRow(true, 1)]
	public void HandleCraftPhase_ProducesOnlySelectedOutcomeAndDoesNotDuplicateProducts(bool failed, int quantity)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.Setup(value => value.Traits.Get(0)).Returns((ITraitDefinition)null!);
		var row = new MudSharp.Models.Craft
		{
			Id = 1, Name = "catalogue route", Category = "crafting", Blurb = "A product routing fixture.",
			ActionDescription = "making a product", ActiveCraftItemSdesc = "work in progress", QualityFormula = "5",
			CheckQualityWeighting = 1, FailPhase = 1, EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = 4 },
			CraftPhases = [new MudSharp.Models.CraftPhase { PhaseNumber = 1, PhaseLengthInSeconds = 1, Echo = "The work succeeds.", FailEcho = "The work fails." }]
		};
		var craft = new Craft(row, world.Object);
		var plan = new Mock<IInventoryPlan>();
		plan.Setup(value => value.PlanIsFeasible()).Returns(InventoryPlanFeasibility.Feasible);
		plan.Setup(value => value.ExecuteWholePlan()).Returns(Array.Empty<InventoryPlanActionResult>());
		var template = new Mock<IInventoryPlanTemplate>();
		var character = new Mock<MudSharp.Character.ICharacter> { DefaultValue = DefaultValue.Mock };
		template.Setup(value => value.CreatePlan(character.Object)).Returns(plan.Object);
		Field<Dictionary<int, IInventoryPlanTemplate>>(craft, "_phaseInventoryPlans")[1] = template.Object;
		Field<Dictionary<int, IInventoryPlanTemplate>>(craft, "_failPhaseInventoryPlans")[1] = template.Object;
		var component = new Mock<IActiveCraftGameItemComponent> { DefaultValue = DefaultValue.Mock };
		component.SetupProperty(value => value.HasFailed, failed);
		component.SetupProperty(value => value.QualityCheckOutcome, Outcome.Pass);
		component.SetupGet(value => value.ConsumedInputs).Returns(new Dictionary<ICraftInput, (IPerceivable, ICraftInputData)>());
		component.SetupGet(value => value.UsedToolQualities).Returns(new Dictionary<ICraftTool, (ItemQuality, double)>());
		var produced = new Dictionary<ICraftProduct, ICraftProductData>();
		component.SetupGet(value => value.ProducedProducts).Returns(produced);
		var successItems = Enumerable.Range(0, 3).Select(_ => new Mock<IGameItem>()).ToArray();
		var failureItems = new[] { new Mock<IGameItem>() };
		Mock<ICraftProduct> Product(long id, Mock<IGameItem>[] items)
		{
			var data = new Mock<ICraftProductDataWithItems>();
			data.SetupGet(value => value.Products).Returns(items.Select(item => item.Object));
			var product = new Mock<ICraftProduct>(); product.SetupGet(value => value.Id).Returns(id);
			product.Setup(value => value.ProduceProduct(component.Object, It.IsAny<ItemQuality>())).Returns(data.Object);
			return product;
		}
		var success = Product(1, successItems); var failure = Product(2, failureItems);
		Field<Dictionary<long, ICraftProduct>>(craft, "_products")[1] = success.Object;
		Field<Dictionary<long, ICraftProduct>>(craft, "_failProducts")[2] = failure.Object;
		Field<DictionaryWithDefault<long, int>>(craft, "_craftProductProducedPhases")[1] = 1;
		Field<DictionaryWithDefault<long, int>>(craft, "_craftFailProductProducedPhases")[2] = 1;
		var effect = new Mock<IActiveCraftEffect>();
		Assert.IsTrue(craft.HandleCraftPhase(character.Object, effect.Object, component.Object, 1));
		Assert.AreEqual(1, produced.Count);
		Assert.AreEqual(quantity, produced.Values.OfType<ICraftProductDataWithItems>().SelectMany(value => value.Products).Count());
		Assert.IsTrue(craft.HandleCraftPhase(character.Object, effect.Object, component.Object, 1));
		success.Verify(value => value.ProduceProduct(component.Object, It.IsAny<ItemQuality>()), failed ? Times.Never() : Times.Once());
		failure.Verify(value => value.ProduceProduct(component.Object, It.IsAny<ItemQuality>()), failed ? Times.Once() : Times.Never());
		foreach (var item in successItems) item.Verify(value => value.SetOwner(character.Object), failed ? Times.Never() : Times.Once());
		foreach (var item in failureItems) item.Verify(value => value.SetOwner(character.Object), failed ? Times.Once() : Times.Never());
	}

	private static T Field<T>(Craft craft, string name) => (T)typeof(Craft).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(craft)!;
}

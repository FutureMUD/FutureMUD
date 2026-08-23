#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Xml.Linq;
using DbEditableItem = MudSharp.Models.EditableItem;
using DbGameItemComponent = MudSharp.Models.GameItemComponent;
using DbGameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SalvageableComponentTests
{
	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FullDescriptionDecoration_LifecycleAndMovementVariants_AddOneStableDisclosure(bool preventsMovement)
	{
		var fixture = CreateFixture();
		var proto = (SalvageableGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreateDatabaseProto(), fixture.Gameworld.Object);
		var parent = CreateParent(fixture.Gameworld.Object, 10.0);
		parent.Setup(x => x.PreventsMovement()).Returns(preventsMovement);
		var component = (SalvageableGameItemComponent)proto.CreateNew(
			parent.Object, temporary: true);
		var copy = (SalvageableGameItemComponent)component.Copy(
			CreateParent(fixture.Gameworld.Object, 10.0).Object, temporary: true);
		var loaded = (SalvageableGameItemComponent)proto.LoadComponent(
			new DbGameItemComponent { Id = 99, Definition = "<Definition />" },
			CreateParent(fixture.Gameworld.Object, 10.0).Object);
		var voyeur = Mock.Of<IPerceiver>();
		const string baseDescription = "This is an otherwise identical test item.";

		Assert.IsTrue(component.DescriptionDecorator(DescriptionType.Full));
		Assert.IsFalse(component.DescriptionDecorator(DescriptionType.Short));
		Assert.IsFalse(component.DescriptionDecorator(DescriptionType.Long));
		Assert.IsFalse(component.DescriptionDecorator(DescriptionType.Evaluate));

		string Render(params IGameItemComponent[] components) => components
			.Where(x => x.DescriptionDecorator(DescriptionType.Full))
			.OrderBy(x => x.DecorationPriority)
			.Aggregate(baseDescription, (current, decorator) => decorator.Decorate(voyeur, "test item", current,
				DescriptionType.Full, colour: false, PerceiveIgnoreFlags.None));

		Assert.AreEqual(baseDescription, Render());
		foreach (var candidate in new[] { component, copy, loaded })
		{
			var firstRender = Render(candidate);
			var repeatedRender = Render(candidate);

			Assert.AreEqual($"{baseDescription}\n\nIt can be salvaged.", firstRender);
			Assert.AreEqual(firstRender, repeatedRender);
			Assert.AreEqual(1, firstRender.Split("It can be salvaged.").Length - 1);
		}

		parent.Verify(x => x.PreventsMovement(), Times.Never);
	}

	[TestMethod]
	public void ComponentRegistrationXmlCopyAndLoad_PreserveCanonicalContract()
	{
		var fixture = CreateFixture();
		var manager = new GameItemComponentManager();

		Assert.IsTrue(manager.PrimaryTypes.Any(x => x.EqualTo("salvageable")));
		var proto = (SalvageableGameItemComponentProto)manager.GetProto(CreateDatabaseProto(), fixture.Gameworld.Object);

		Assert.AreSame(fixture.Trait.Object, proto.Trait);
		Assert.AreEqual(Difficulty.Normal, proto.Difficulty);
		Assert.AreEqual(2, proto.Stages.Count);
		Assert.AreEqual(2, proto.CommodityProducts.Count);
		Assert.AreEqual(1, proto.ItemProducts.Count);
		Assert.IsInstanceOfType(proto, typeof(ISalvageablePrototype));

		var parent = CreateParent(fixture.Gameworld.Object, 10.0);
		var component = proto.CreateNew(parent.Object, temporary: true);
		var copy = component.Copy(CreateParent(fixture.Gameworld.Object, 10.0).Object, true);
		var loaded = proto.LoadComponent(new DbGameItemComponent { Id = 99, Definition = "<Definition />" }, parent.Object);

		Assert.IsInstanceOfType(component, typeof(ISalvageable));
		Assert.IsInstanceOfType(copy, typeof(ISalvageable));
		Assert.IsInstanceOfType(loaded, typeof(ISalvageable));

		var saveMethod = typeof(SalvageableGameItemComponentProto)
			.GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!;
		var saved = XElement.Parse((string)saveMethod.Invoke(proto, null)!);
		CollectionAssert.AreEqual(
			new[] { "Trait", "Difficulty", "ToolTag", "Stages", "CommodityProducts", "ItemProducts" },
			saved.Elements().Select(x => x.Name.LocalName).ToArray());
		CollectionAssert.AreEqual(new[] { "First stage", "Second stage" },
			saved.Element("Stages")!.Elements().Select(x => x.Value).ToArray());
		CollectionAssert.AreEqual(new long[] { 100, 101 },
			saved.Element("CommodityProducts")!.Elements().Select(x => (long)x.Attribute("material")!).ToArray());
	}

	[TestMethod]
	public void ProductPlan_SupportsFixedFractionItemAndDistinctSuccessFailureBranches()
	{
		var fixture = CreateFixture();
		var proto = (SalvageableGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreateDatabaseProto(), fixture.Gameworld.Object);

		var success = proto.CreateProductPlan(10.0, true, () => 0.25);
		var failure = proto.CreateProductPlan(10.0, false, () => 0.25);

		CollectionAssert.AreEqual(new[] { 2.0, 1.0 }, success.Commodities.Select(x => x.Weight).ToArray());
		Assert.AreEqual(1, success.Items.Count);
		Assert.AreEqual(2, success.Items[0].Quantity);
		CollectionAssert.AreEqual(new[] { 1.0, 0.5 }, failure.Commodities.Select(x => x.Weight).ToArray());
		Assert.AreEqual(0, failure.Items.Count);
		Assert.AreEqual(7.0, proto.MaximumOutputWeight(10.0, true), 1.0e-9);
		Assert.AreEqual(3.5, proto.MaximumOutputWeight(10.0, false), 1.0e-9);
	}

	[TestMethod]
	public void CleanLoad_ItemPrototypeProvisionedLater_RetainsExactReferenceForFirstConsumer()
	{
		var fixture = CreateFixture(addItemProto: false);
		var proto = (SalvageableGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreateDatabaseProto(), fixture.Gameworld.Object);
		var product = proto.ItemProducts.Single();

		Assert.AreEqual(200L, product.ItemPrototypeId);
		Assert.AreEqual(1, product.ItemPrototypeRevision);
		Assert.IsNull(product.ItemPrototype);
		Assert.IsFalse(proto.ConfigurationIsComplete(out var unresolvedReason));
		StringAssert.Contains(unresolvedReason, "#200r1 is unavailable");

		var component = (ISalvageable)proto.CreateNew(CreateParent(fixture.Gameworld.Object, 10.0).Object,
			temporary: true);
		Assert.IsFalse(component.CanSalvage(out var consumerReason));
		StringAssert.Contains(consumerReason, "#200r1 is unavailable");

		fixture.ItemProtos.Add(fixture.ItemProto.Object);

		Assert.AreSame(fixture.ItemProto.Object, product.ItemPrototype);
		Assert.IsTrue(component.CanSalvage(out var resolvedReason), resolvedReason);
		var plan = proto.CreateProductPlan(10.0, true, () => 0.0);
		Assert.AreSame(fixture.ItemProto.Object, plan.Items.Single().Product.ItemPrototype);
		Assert.AreEqual(2, plan.Items.Single().Quantity);
	}

	[TestMethod]
	public void ConfigurationIsComplete_RejectsNonFiniteAndOutOfRangeAuthoringPayloads()
	{
		var fixture = CreateFixture();
		var cases = new[]
		{
			(From: "delay=\"1\"", To: "delay=\"NaN\"", Expected: "invalid delay"),
			(From: "successChance=\"0.5\"", To: "successChance=\"1.1\"", Expected: "invalid item product"),
			(From: "successQuantity=\"2\"", To: "successQuantity=\"101\"", Expected: "more than 100")
		};

		foreach (var testCase in cases)
		{
			var databaseProto = CreateDatabaseProto();
			databaseProto.Definition = databaseProto.Definition.Replace(testCase.From, testCase.To,
				StringComparison.Ordinal);
			var proto = (SalvageableGameItemComponentProto)new GameItemComponentManager()
				.GetProto(databaseProto, fixture.Gameworld.Object);

			Assert.IsFalse(proto.ConfigurationIsComplete(out var reason));
			StringAssert.Contains(reason, testCase.Expected);
		}
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Eligibility_DoesNotDependOnCarryability(bool preventsMovement)
	{
		var fixture = CreateFixture();
		var proto = (SalvageableGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreateDatabaseProto(), fixture.Gameworld.Object);
		var parent = CreateParent(fixture.Gameworld.Object, 10.0);
		parent.Setup(x => x.PreventsMovement()).Returns(preventsMovement);
		var component = (ISalvageable)proto.CreateNew(parent.Object, temporary: true);

		Assert.IsTrue(component.CanSalvage(out var reason), reason);
		parent.Verify(x => x.PreventsMovement(), Times.Never);
		parent.Verify(x => x.CanGet(It.IsAny<ItemCanGetIgnore>()), Times.Never);
	}

	[TestMethod]
	public void Eligibility_RejectsOverBudgetContentsAttachmentsAndLiquid()
	{
		var fixture = CreateFixture();
		var proto = (SalvageableGameItemComponentProto)new GameItemComponentManager()
			.GetProto(CreateDatabaseProto(), fixture.Gameworld.Object);

		var overBudget = (ISalvageable)proto.CreateNew(CreateParent(fixture.Gameworld.Object, 6.0).Object, temporary: true);
		Assert.IsFalse(overBudget.CanSalvage(out var budgetReason));
		StringAssert.Contains(budgetReason, "mass budget");

		var containedParent = CreateParent(fixture.Gameworld.Object, 10.0);
		containedParent.SetupGet(x => x.DeepItems).Returns([containedParent.Object, Mock.Of<IGameItem>()]);
		AssertRejected(proto, containedParent, "contains another item");

		var attachedParent = CreateParent(fixture.Gameworld.Object, 10.0);
		attachedParent.SetupGet(x => x.AttachedAndConnectedItems).Returns([Mock.Of<IGameItem>()]);
		AssertRejected(proto, attachedParent, "contains another item");

		var liquid = new Mock<ILiquidContainer>();
		liquid.SetupGet(x => x.LiquidVolume).Returns(1.0);
		var liquidParent = CreateParent(fixture.Gameworld.Object, 10.0);
		liquidParent.Setup(x => x.GetItemTypes<ILiquidContainer>()).Returns([liquid.Object]);
		AssertRejected(proto, liquidParent, "contains liquid");
	}

	[TestMethod]
	public void ItemSalvaging_InterruptionAndRestartState_CreateNothingAndLeaveSource()
	{
		var (action, actor, source, salvageable, actorEffects, targetEffects) = CreateActionFixture();
		actorEffects.Add(action);

		Assert.IsFalse(action.SavingEffect, "An interrupted action must not persist as saved progress across restart.");
		action.RemovalEffect();

		salvageable.Verify(x => x.CreateProducts(It.IsAny<ICharacter>(), It.IsAny<bool>()), Times.Never);
		source.Verify(x => x.Delete(), Times.Never);
		source.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), false), Times.Once);
	}

	[DataTestMethod]
	[DataRow(Outcome.Pass, true)]
	[DataRow(Outcome.Fail, false)]
	public void ItemSalvaging_CompletionConsumesSourceOnSuccessAndFailure(Outcome outcome, bool success)
	{
		var (action, actor, source, salvageable, actorEffects, targetEffects) = CreateActionFixture(outcome);
		actorEffects.Add(action);

		action.ExpireEffect();

		salvageable.Verify(x => x.CreateProducts(actor.Object, success), Times.Once);
		source.Verify(x => x.Delete(), Times.Once);
		source.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), false), Times.Once);
	}

	[TestMethod]
	public void ItemSalvaging_ProductCreationFailure_PreservesSource()
	{
		var (action, actor, source, salvageable, actorEffects, targetEffects) = CreateActionFixture();
		actorEffects.Add(action);
		salvageable.Setup(x => x.CreateProducts(actor.Object, true))
			.Throws(new InvalidOperationException("product creation failed"));

		action.ExpireEffect();

		source.Verify(x => x.Delete(), Times.Never);
		source.Verify(x => x.RemoveEffect(It.IsAny<IEffect>(), false), Times.Once);
	}

	[TestMethod]
	public void ItemSalvaging_FailureCompletionIsVisiblyDistinct()
	{
		var success = ItemSalvaging.CompletionEmote(true, "some parts");
		var failure = ItemSalvaging.CompletionEmote(false, "fewer parts");

		Assert.IsFalse(success.Contains("rough work", StringComparison.Ordinal));
		StringAssert.Contains(failure, "rough work leaves much of it unusable");
		StringAssert.Contains(failure, "fewer parts");
	}

	[TestMethod]
	public void SalvageCommand_ComponentDispatchIsSeparateAndLegacyFallbackRemains()
	{
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		var target = new Mock<IGameItem>();
		actor.Setup(x => x.TargetLocalItem("target")).Returns(target.Object);

		InvokeSalvage(actor.Object, "salvage target");
		target.Verify(x => x.GetItemType<IButcherable>(), Times.Once,
			"An item without Salvageable must continue through the unchanged corpse/bodypart branch.");

		var salvageable = new Mock<ISalvageable>();
		target.Setup(x => x.GetItemType<ISalvageable>()).Returns(salvageable.Object);
		salvageable.Setup(x => x.CanSalvage(out It.Ref<string>.IsAny))
			.Returns((out string reason) => { reason = "test refusal"; return false; });
		target.Invocations.Clear();

		InvokeSalvage(actor.Object, "salvage target");
		salvageable.Verify(x => x.CanSalvage(out It.Ref<string>.IsAny), Times.Once);
		target.Verify(x => x.GetItemType<IButcherable>(), Times.Never,
			"An ordinary Salvageable item must not enter race/body butchery semantics.");

		salvageable.Invocations.Clear();
		InvokeSalvage(actor.Object, "salvage target engine");
		salvageable.Verify(x => x.CanSalvage(out It.Ref<string>.IsAny), Times.Never,
			"Ordinary item subcategories must be refused before starting work.");
	}

	[TestMethod]
	public void SalvageCommand_ExistingWorkLockPreventsCompetingStart()
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		var target = new Mock<IGameItem>();
		actor.Setup(x => x.TargetLocalItem("target")).Returns(target.Object);
		var salvageable = new Mock<ISalvageable>();
		target.Setup(x => x.GetItemType<ISalvageable>()).Returns(salvageable.Object);
		var workLock = new ItemSalvaging.BeingSalvaged(target.Object, actor.Object);
		target.Setup(x => x.EffectsOfType<ItemSalvaging.BeingSalvaged>(
			It.IsAny<Predicate<ItemSalvaging.BeingSalvaged>>())).Returns([workLock]);

		InvokeSalvage(actor.Object, "salvage target");

		salvageable.Verify(x => x.CanSalvage(out It.Ref<string>.IsAny), Times.Never);
	}

	private static void InvokeSalvage(ICharacter actor, string input)
	{
		var method = typeof(CraftModule).GetMethod("Salvage", BindingFlags.Static | BindingFlags.NonPublic)!;
		method.Invoke(null, [actor, input]);
	}

	private static void AssertRejected(SalvageableGameItemComponentProto proto, Mock<IGameItem> parent, string reasonText)
	{
		var component = (ISalvageable)proto.CreateNew(parent.Object, temporary: true);
		Assert.IsFalse(component.CanSalvage(out var reason));
		StringAssert.Contains(reason, reasonText);
	}

	private static (ItemSalvaging Action, Mock<ICharacter> Actor, Mock<IGameItem> Source,
		Mock<ISalvageable> Salvageable, List<IEffect> ActorEffects, List<IEffect> TargetEffects)
		CreateActionFixture(Outcome outcome = Outcome.Pass)
	{
		var gameworld = new Mock<IFuturemud>();
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(It.IsAny<ICharacter>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>())).Returns(new CheckOutcome { Outcome = outcome });
		gameworld.Setup(x => x.GetCheck(CheckType.ButcheryCheck)).Returns(check.Object);
		var actorEffects = new List<IEffect>();
		var targetEffects = new List<IEffect>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Body).Returns(Mock.Of<MudSharp.Body.IBody>());
		actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.SetupGet(x => x.Effects).Returns(() => actorEffects);
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, _) => actorEffects.Remove(effect));

		var source = new Mock<IGameItem>();
		source.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		source.SetupGet(x => x.Effects).Returns(() => targetEffects);
		source.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
			.Callback<IEffect>(effect =>
			{
				Assert.IsNotNull(effect, "The base CharacterActionWithTarget constructor invokes virtual setup before the derived constructor body.");
				targetEffects.Add(effect);
			});
		source.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>((effect, _) => targetEffects.Remove(effect));

		var salvageable = new Mock<ISalvageable>();
		salvageable.SetupGet(x => x.Parent).Returns(source.Object);
		salvageable.SetupGet(x => x.Stages).Returns([("@ work|works on $1.", 1.0)]);
		salvageable.SetupGet(x => x.Trait).Returns(Mock.Of<ITraitDefinition>());
		salvageable.SetupGet(x => x.Difficulty).Returns(Difficulty.Normal);
		salvageable.Setup(x => x.CanSalvage(out It.Ref<string>.IsAny))
			.Returns((out string reason) => { reason = string.Empty; return true; });
		salvageable.Setup(x => x.CreateProducts(actor.Object, It.IsAny<bool>()))
			.Returns(Array.Empty<IGameItem>());

		var action = new ItemSalvaging(actor.Object, salvageable.Object, null);
		return (action, actor, source, salvageable, actorEffects, targetEffects);
	}

	private static Mock<IGameItem> CreateParent(IFuturemud gameworld, double baseWeight)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Weight).Returns(baseWeight);
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		parent.SetupGet(x => x.Prototype).Returns(proto.Object);
		parent.SetupGet(x => x.Quantity).Returns(1);
		parent.SetupGet(x => x.DeepItems).Returns(() => [parent.Object]);
		parent.SetupGet(x => x.AttachedAndConnectedItems).Returns([]);
		parent.Setup(x => x.GetItemTypes<ILiquidContainer>()).Returns([]);
		parent.Setup(x => x.GetItemTypes<ISheath>()).Returns([]);
		parent.Setup(x => x.GetItemTypes<IRangedWeaponPlatform>()).Returns([]);
		parent.Setup(x => x.GetItemTypes<IAutomationHousing>()).Returns([]);
		parent.Setup(x => x.GetItemTypes<IArtilleryMount>()).Returns([]);
		parent.Setup(x => x.GetItemTypes<IWeaponCarrierAttachment>()).Returns([]);
		parent.Setup(x => x.GetItemTypes<IArtilleryChamber>()).Returns([]);
		return parent;
	}

	private static (Mock<IFuturemud> Gameworld, Mock<ITraitDefinition> Trait,
		RevisableAll<IGameItemProto> ItemProtos, Mock<IGameItemProto> ItemProto) CreateFixture(bool addItemProto = true)
	{
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.Id).Returns(50);
		trait.SetupGet(x => x.Name).Returns("Electronics");
		var traits = new All<ITraitDefinition>();
		traits.Add(trait.Object);

		var materialA = CreateMaterial(100, "electronic parts");
		var materialB = CreateMaterial(101, "plastic");
		var materials = new All<ISolid>();
		materials.Add(materialA.Object);
		materials.Add(materialB.Object);

		var itemProto = new Mock<IGameItemProto>();
		itemProto.SetupGet(x => x.Id).Returns(200);
		itemProto.SetupGet(x => x.RevisionNumber).Returns(1);
		itemProto.SetupGet(x => x.Name).Returns("battery");
		itemProto.SetupGet(x => x.Weight).Returns(2.0);
		itemProto.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		var itemProtos = new RevisableAll<IGameItemProto>();
		if (addItemProto)
		{
			itemProtos.Add(itemProto.Object);
		}

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Traits).Returns(traits);
		gameworld.SetupGet(x => x.Materials).Returns(materials);
		gameworld.SetupGet(x => x.Tags).Returns(new All<ITag>());
		gameworld.SetupGet(x => x.ItemProtos).Returns(itemProtos);
		return (gameworld, trait, itemProtos, itemProto);
	}

	private static Mock<ISolid> CreateMaterial(long id, string name)
	{
		var material = new Mock<ISolid>();
		material.SetupGet(x => x.Id).Returns(id);
		material.SetupGet(x => x.Name).Returns(name);
		return material;
	}

	private static DbGameItemComponentProto CreateDatabaseProto()
	{
		return new DbGameItemComponentProto
		{
			Id = 1,
			Name = "Test Salvageable",
			Description = "Test",
			Type = "Salvageable",
			RevisionNumber = 0,
			Definition = """
			             <Definition>
			               <Trait>50</Trait>
			               <Difficulty>5</Difficulty>
			               <ToolTag>0</ToolTag>
			               <Stages>
			                 <Stage delay="1"><![CDATA[First stage]]></Stage>
			                 <Stage delay="2"><![CDATA[Second stage]]></Stage>
			               </Stages>
			               <CommodityProducts>
			                 <Product material="100" tag="0" fraction="false" success="2" failure="1" />
			                 <Product material="101" tag="0" fraction="true" success="0.1" failure="0.05" />
			               </CommodityProducts>
			               <ItemProducts>
			                 <Product id="200" revision="1" successQuantity="2" failureQuantity="1" successChance="0.5" failureChance="0.1" />
			               </ItemProducts>
			             </Definition>
			             """,
			EditableItem = new DbEditableItem
			{
				Id = 1,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow,
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.Current
			}
		};
	}
}

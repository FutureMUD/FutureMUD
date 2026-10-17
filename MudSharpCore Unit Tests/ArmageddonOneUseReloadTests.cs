#nullable enable

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.Framework.Save;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public partial class ArmageddonOneUseReloadTests
{
	[TestMethod]
	public void SpentTopicalDose_ColdLoadDoesNotDeleteParentAgain()
	{
		var world = World();
		var parent = Parent(world.Object, null);
		var proto = new TopicalProto(world.Object,
			"<Definition><TotalGrams>1</TotalGrams><OnApplyProg>0</OnApplyProg><NativeHealingDose key=\"arm.item.restorative_mud\" grams=\"1\" budgetPerGrade=\"10\"/><Drugs /></Definition>");
		var model = new MudSharp.Models.GameItemComponent { Id = 3, Definition = "<Definition Grams=\"0\" LifecycleId=\"a4c16f26-f369-4d21-9736-9910e3d5dc5b\" SpellId=\"85\" Grade=\"2\" CreatorId=\"42\" CreatorInstanceId=\"43\" ReferenceGrams=\"1\" BudgetPerGrade=\"10\" Spent=\"true\" />" };

		_ = new TopicalCreamGameItemComponent(model, proto, parent.Object);

		parent.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void BoundFractionalBindings_ReloadWithInvariantNumbersUnderGermanCulture()
	{
		var priorCulture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
			var lifecycle = Guid.Parse("a4c16f26-f369-4d21-9736-9910e3d5dc5b");
			var deadline = new DateTime(2026, 10, 12, 12, 30, 0, DateTimeKind.Utc);
			var world = World();
			var origin = new SpellOwnedItemOrigin(lifecycle, SpellLifecycleMode.TemporaryCleanup, deadline, 42, 85, 3);
			var parent = Parent(world.Object, origin);
			var topicalProto = new TopicalProto(world.Object,
				"<Definition><TotalGrams>1.25</TotalGrams><OnApplyProg>0</OnApplyProg><NativeHealingDose key=\"arm.item.restorative_mud\" grams=\"1.25\" budgetPerGrade=\"10.5\"/><Drugs /></Definition>");
			var topical = new TopicalCreamGameItemComponent(new MudSharp.Models.GameItemComponent
			{
				Id = 4,
				Definition = $"<Definition Grams=\"1.25\" LifecycleId=\"{lifecycle}\" SpellId=\"85\" Grade=\"3\" CreatorId=\"42\" CreatorInstanceId=\"43\" ReferenceGrams=\"1.25\" BudgetPerGrade=\"10.5\" Spent=\"false\" />"
			}, topicalProto, parent.Object);

			Assert.IsNotNull(topical.DoseBinding);
			Assert.AreEqual(1.25, topicalProto.TotalGrams, 0.000001);
			Assert.AreEqual(1.25, topicalProto.NativeDoseProfile!.ReferenceDoseGrams, 0.000001);
			Assert.AreEqual(10.5, topicalProto.NativeDoseProfile.BudgetPerGrade, 0.000001);
			Assert.AreEqual(1.25, topical.DoseBinding!.ReferenceDoseGrams, 0.000001);
			Assert.AreEqual(10.5, topical.DoseBinding.BudgetPerGrade, 0.000001);
			Assert.AreEqual(31.5, topical.DoseBinding.HealingBudget, 0.000001);

			var emberProto = EmberPrototype(world.Object,
				"<Definition damagePerGrade=\"1.5\" />");
			var ember = new ArmageddonEmberSeedGameItemComponent(new MudSharp.Models.GameItemComponent
			{
				Id = 5,
				Definition = $"<Definition Spent=\"false\" LifecycleId=\"{lifecycle}\" SpellId=\"85\" Mode=\"TemporaryCleanup\" Grade=\"3\" CreatorId=\"42\" CreatorInstanceId=\"43\" DamagePerGrade=\"1.5\" ExpiresUtc=\"{deadline:O}\" />"
			}, emberProto, parent.Object);

			Assert.IsNotNull(ember.Binding);
			Assert.AreEqual(1.5, emberProto.DamagePerGrade, 0.000001);
			Assert.AreEqual(4.5, ember.Binding!.DamagePerEligibleTarget, 0.000001);
		}
		finally
		{
			CultureInfo.CurrentCulture = priorCulture;
		}
	}

	[DataTestMethod]
	[DataRow(1)] [DataRow(2)] [DataRow(3)] [DataRow(4)] [DataRow(5)] [DataRow(6)] [DataRow(7)]
	public void ConfirmedNativeDose_ConsumesOnceAndCapsHealingAtBoundGradeBudget(int grade)
	{
		var f = MudFixture(activeOrigin: true, confirmed: true, grade: grade);
		f.Component.Apply(f.Body.Object, f.Part.Object, 0.0, null!);
		Assert.AreEqual(80.0 - 10.0 * grade, f.Wound.Object.CurrentDamage, 0.000001);
		f.Service.Verify(x => x.TryCommitComponentConsumption(f.Component, f.Origin), Times.Once);
		f.Parent.Verify(x => x.Delete(), Times.Once);

		f.Component.Apply(f.Body.Object, f.Part.Object, 0.0, null!);
		Assert.AreEqual(80.0 - 10.0 * grade, f.Wound.Object.CurrentDamage, 0.000001, "A second application must not replay the dose.");
		f.Service.Verify(x => x.TryCommitComponentConsumption(f.Component, f.Origin), Times.Once);
	}

	[TestMethod]
	public void HeldOrUnconfirmedDose_DoesNotHeal()
	{
		var held = MudFixture(activeOrigin: false, confirmed: true);
		held.Component.Apply(held.Body.Object, held.Part.Object, 0.0, null!);
		Assert.AreEqual(80.0, held.Wound.Object.CurrentDamage, 0.000001);
		held.Service.Verify(x => x.TryCommitComponentConsumption(It.IsAny<IGameItemComponent>(), It.IsAny<SpellOwnedItemOrigin>()), Times.Never);

		var unconfirmed = MudFixture(activeOrigin: true, confirmed: false);
		unconfirmed.Component.Apply(unconfirmed.Body.Object, unconfirmed.Part.Object, 0.0, null!);
		Assert.AreEqual(80.0, unconfirmed.Wound.Object.CurrentDamage, 0.000001);
		unconfirmed.Service.Verify(x => x.TryCommitComponentConsumption(unconfirmed.Component, unconfirmed.Origin), Times.Once);
	}

	[DataTestMethod]
	[DataRow("body")]
	[DataRow("room")]
	public void TargetChangingDuringCarrierDelivery_DoesNotHeal(string changedTarget)
	{
		var f = MudFixture(activeOrigin: true, confirmed: true, mutateAfterCarrier: changedTarget);
		f.Component.Apply(f.Body.Object, f.Part.Object, 0.0, null!);
		Assert.AreEqual(80.0, f.Wound.Object.CurrentDamage, 0.000001);
		f.Service.Verify(x => x.TryCommitComponentConsumption(f.Component, f.Origin), Times.Once);
	}

	private static Mock<IFuturemud> World()
	{
		var world = new Mock<IFuturemud>();
		var progs = new Mock<IUneditableAll<MudSharp.FutureProg.IFutureProg>>();
		progs.Setup(x => x.Get(It.IsAny<long>())).Returns((MudSharp.FutureProg.IFutureProg?)null!);
		world.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		var units = new Mock<IUnitManager>();
		units.SetupGet(x => x.BaseWeightToKilograms).Returns(0.001);
		world.SetupGet(x => x.UnitManager).Returns(units.Object);
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		return world;
	}

	private sealed record MudFixtureData(TopicalCreamGameItemComponent Component, Mock<IBody> Body,
		Mock<IBodypart> Part, Mock<IWound> Wound, Mock<IGameItem> Parent,
		Mock<ISpellOwnedItemService> Service, SpellOwnedItemOrigin Origin);

	private static MudFixtureData MudFixture(bool activeOrigin, bool confirmed, string? mutateAfterCarrier = null, int grade = 2, bool permanent = false)
	{
		IBody? actorBody = null;
		IRoom? actorLocation = null;
		var world = World();
		var substances = new Mock<IUneditableAll<IMagicalSubstance>>();
		substances.As<IEnumerable<IMagicalSubstance>>().Setup(x => x.GetEnumerator()).Returns(() => CarrierEnumeration(() =>
		{
			if (mutateAfterCarrier == "body") actorBody = Mock.Of<IBody>();
			if (mutateAfterCarrier == "room") actorLocation = Mock.Of<IRoom>();
		}).GetEnumerator());
		world.SetupGet(x => x.MagicalSubstances).Returns(substances.Object);
		var origin = new SpellOwnedItemOrigin(Guid.Parse("a4c16f26-f369-4d21-9736-9910e3d5dc5b"),
			permanent ? SpellLifecycleMode.Permanent : SpellLifecycleMode.TemporaryCleanup,
			permanent ? null : new DateTime(2026, 10, 13, 0, 0, 0, DateTimeKind.Utc), 42, 85, grade);
		var service = new Mock<ISpellOwnedItemService>();
		service.Setup(x => x.HasActiveOrigin(It.IsAny<IGameItem>(), origin)).Returns(activeOrigin);
		service.Setup(x => x.TryCommitComponentConsumption(It.IsAny<IGameItemComponent>(), origin)).Returns(confirmed);
		world.SetupGet(x => x.SpellOwnedItems).Returns(service.Object);
		var part = new Mock<IBodypart>(); part.SetupGet(x => x.Id).Returns(8);
		var wound = new Mock<IWound>(); wound.SetupGet(x => x.Id).Returns(9); wound.SetupGet(x => x.Bodypart).Returns(part.Object);
		wound.SetupProperty(x => x.CurrentDamage, 80.0);
		var body = new Mock<IBody>();
		var actor = new Mock<ICharacter>();	var room = new Mock<IRoom>();
		actorBody = body.Object;
		actorLocation = room.Object;
		actor.SetupGet(x => x.Body).Returns(() => actorBody);
		actor.SetupGet(x => x.Location).Returns(() => actorLocation);
		actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		body.SetupGet(x => x.Gameworld).Returns(world.Object);
		body.SetupGet(x => x.Bodyparts).Returns([part.Object]);
		body.SetupGet(x => x.Wounds).Returns([wound.Object]);
		body.Setup(x => x.WornItemsProfilesFor(part.Object)).Returns([]);
		var parent = Parent(world.Object, origin);
		var itemProto = new Mock<IGameItemProto>(); itemProto.SetupGet(x => x.Id).Returns(303);
		parent.SetupGet(x => x.Prototype).Returns(itemProto.Object);
		var proto = new TopicalProto(world.Object,
			"<Definition><TotalGrams>1</TotalGrams><OnApplyProg>0</OnApplyProg><NativeHealingDose key=\"arm.item.restorative_mud\" grams=\"1\" budgetPerGrade=\"10\"/><Drugs /></Definition>");
		var bindingXml = $"<Definition Grams=\"1\" LifecycleId=\"{origin.LifecycleId}\" SpellId=\"85\" Grade=\"{grade}\" CreatorId=\"42\" CreatorInstanceId=\"43\" ReferenceGrams=\"1\" BudgetPerGrade=\"10\" Spent=\"false\" />";
		var component = new TopicalCreamGameItemComponent(new MudSharp.Models.GameItemComponent { Id = 4, Definition = bindingXml }, proto, parent.Object);
		parent.SetupGet(x => x.Components).Returns([component]);
		parent.Setup(x => x.GetItemType<IArmageddonTopicalDoseComponent>()).Returns(component);
		return new(component, body, part, wound, parent, service, origin);
	}

	private static IEnumerable<IMagicalSubstance> CarrierEnumeration(Action? callback)
	{
		callback?.Invoke();
		yield break;
	}

	private static Mock<IGameItem> Parent(IFuturemud world, SpellOwnedItemOrigin? origin)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(world);
		parent.SetupGet(x => x.SpellCreationOrigin).Returns(origin);
		return parent;
	}

	private sealed class TopicalProto(IFuturemud world, string definition) : TopicalCreamGameItemComponentProto(
		new MudSharp.Models.GameItemComponentProto
		{
			Id = 11, Name = "Test restorative dose", Type = "TopicalCream", Definition = definition,
			EditableItem = new MudSharp.Models.EditableItem { RevisionNumber = 1, RevisionStatus = 4 }
		}, world);

	private static ArmageddonEmberSeedGameItemComponentProto EmberPrototype(IFuturemud world, string definition)
	{
		var model = new MudSharp.Models.GameItemComponentProto
		{
			Id = 12, Name = "Test Ember Seed", Type = "ArmageddonEmberSeed", Definition = definition,
			EditableItem = new MudSharp.Models.EditableItem { RevisionNumber = 1, RevisionStatus = 4 }
		};
		return (ArmageddonEmberSeedGameItemComponentProto)Activator.CreateInstance(
			typeof(ArmageddonEmberSeedGameItemComponentProto), BindingFlags.Instance | BindingFlags.NonPublic,
			null, new object[] { model, world }, null)!;
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Database;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemLifecycleTests
{
	[TestMethod]
	[DoNotParallelize]
	public void CheckPrototypeForUpdate_ExistingRow_ChangesOnlyRevisionAndQuality()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var db = new FuturemudDatabaseContext(options);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var oldPrototype = Mock.Of<IGameItemProto>(x => x.Name == "item" && x.Id == 77 &&
			x.Status == RevisionStatus.Revised && x.HealthStrategy == Mock.Of<MudSharp.Health.IHealthStrategy>());
		var newPrototype = Mock.Of<IGameItemProto>(x => x.Id == 77 && x.RevisionNumber == 1 &&
			x.Status == RevisionStatus.Current && x.BaseItemQuality == ItemQuality.Standard);
		world.Setup(x => x.ItemProtos.Get(77, 0)).Returns(oldPrototype);
		world.Setup(x => x.ItemProtos.GetAll(77)).Returns([newPrototype]);
		db.GameItems.Add(new MudSharp.Models.GameItem
		{
			Id = 301, GameItemProtoId = 77, OwnerId = 6, OwnerType = "Character", Condition = 0.9,
			SkinId = 3, MorphTimeRemaining = 120, EffectData = "<Effects/>", ContainerId = 302,
			PositionModifier = 2, RoomLayer = 1
		});
		db.SaveChanges();
		db.ChangeTracker.Clear();
		var item = new GameItem(db.GameItems.AsNoTracking().Single(), world.Object);
		var ambient = typeof(FMDB).GetProperty(nameof(FMDB.Context))!;
		var previous = ambient.GetValue(null);
		ambient.SetValue(null, db);
		try
		{
			using var scope = new FMDB();
			Assert.IsTrue(item.CheckPrototypeForUpdate());
			CollectionAssert.AreEquivalent(new[] { "GameItemProtoRevision", "Quality" },
				db.ChangeTracker.Entries<MudSharp.Models.GameItem>().Single().Properties
					.Where(x => x.IsModified).Select(x => x.Metadata.Name).ToArray());
			db.SaveChanges();
			using var fresh = new FuturemudDatabaseContext(options);
			var updated = fresh.GameItems.AsNoTracking().Single();
			Assert.AreEqual(1, updated.GameItemProtoRevision);
			Assert.AreEqual((int)ItemQuality.Standard, updated.Quality);
			Assert.AreEqual(6L, updated.OwnerId);
			Assert.AreEqual(0.9, updated.Condition);
			Assert.AreEqual(3L, updated.SkinId);
			Assert.AreEqual(120, updated.MorphTimeRemaining);
			Assert.AreEqual(302L, updated.ContainerId);
		}
		finally
		{
			ambient.SetValue(null, previous);
		}
	}

	[TestMethod]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void DeepCopy_NestedSeveredPart_OwnsIndependentItemsAndWounds(bool publish, bool deleteCopyFirst)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var manager = new Mock<ISaveManager>();
		world.SetupGet(x => x.SaveManager).Returns(manager.Object);
		var prototype = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		var source = new TestItem(prototype);
		var nested = new TestItem(prototype);
		var clothing = new TestItem(prototype);
		var implant = new TestItem(prototype);
		var lodged = new TestItem(prototype);
		var bodypart = Mock.Of<IBodypart>(x => x.Id == 100);
		world.Setup(x => x.BodypartPrototypes.Get(100)).Returns(bodypart);
		world.Setup(x => x.TryGetCharacter(0, true)).Returns((MudSharp.Character.ICharacter)null!);
		var model = Mock.Of<ICorpseModel>();
		var tattooTemplate = Mock.Of<ITattooTemplate>(x => x.Id == 101);
		world.Setup(x => x.DisfigurementTemplates.Get(101)).Returns(tattooTemplate);
		var tattoo = new Tattoo(tattooTemplate, world.Object, null, 50, bodypart, MudDateTime.Never)
		{
			CompletionPercentage = 0.75
		};
		var wound = WoundFactory.LoadWound(new MudSharp.Models.Wound
		{
			WoundType = "SimpleOrganic", ExtraInformation = "<Definition><BleedStatus>2</BleedStatus></Definition>",
			BodypartProtoId = bodypart.Id, OriginalDamage = 12, CurrentDamage = 8, CurrentPain = 2,
			DamageType = (int)DamageType.Piercing
		}, nested, world.Object);
		wound.Lodged = lodged;
		var remains = new BodypartGameItemComponent(null!, nested, temporary: true)
		{
			Model = model, Parts = [bodypart], DecayPoints = 9, EatenWeight = 0.5,
			Contents = [clothing], Implants = [implant], Wounds = [wound], Tattoos = [tattoo]
		};
		nested.Attach(remains);
		clothing.LoadTimeSetContainedIn(nested);
		implant.LoadTimeSetContainedIn(nested);
		var containerProto = (ContainerGameItemComponentProto)new GameItemComponentManager().GetProto(
			new MudSharp.Models.GameItemComponentProto
			{
				Type = "Container", Name = "test container", Definition = "<Definition />",
				EditableItem = new MudSharp.Models.EditableItem()
			}, world.Object)!;
		var container = new ContainerGameItemComponent(containerProto, source, temporary: true);
		source.Attach(container);
		container.Put(null, nested, allowMerge: false);
		manager.Invocations.Clear();
		world.Invocations.Clear();

		var copiedBag = source.DeepCopy(publish, preserveMorphTime: true);
		var copiedPart = copiedBag.GetItemType<IContainer>().Contents.Single();
		var copiedRemains = copiedPart.GetItemType<ISeveredBodypart>();
		var copiedClothing = copiedRemains.Contents.Single();
		var copiedImplant = copiedRemains.Implants.Single();
		var copiedWound = copiedRemains.Wounds.Single();
		var copiedLodged = copiedWound.Lodged;
		Assert.AreNotSame(nested, copiedPart);
		Assert.AreNotSame(clothing, copiedClothing);
		Assert.AreNotSame(implant, copiedImplant);
		Assert.AreNotSame(lodged, copiedLodged);
		Assert.AreNotSame(wound, copiedWound);
		Assert.AreSame(copiedPart, copiedClothing.ContainedIn);
		Assert.AreSame(copiedPart, copiedImplant.ContainedIn);
		Assert.AreSame(copiedPart, copiedWound.Parent);
		Assert.IsNull(copiedLodged.ContainedIn);
		Assert.AreSame(bodypart, copiedWound.Bodypart);
		Assert.AreEqual(8.0, copiedWound.CurrentDamage);
		Assert.AreEqual(wound.BleedStatus, copiedWound.BleedStatus);
		Assert.AreEqual(9.0, copiedRemains.DecayPoints);
		Assert.AreEqual(0.5, copiedRemains.EatenWeight);
		var copiedTattoo = copiedRemains.Tattoos.Single();
		Assert.AreNotSame(tattoo, copiedTattoo);
		Assert.AreEqual(0.75, copiedTattoo.CompletionPercentage);
		copiedTattoo.CompletionPercentage = 1.0;
		Assert.AreEqual(0.75, tattoo.CompletionPercentage);
		world.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Exactly(publish ? 5 : 0));
		manager.Verify(x => x.AddInitialisation((ILateInitialisingItem)copiedWound), publish ? Times.Once() : Times.Never());

		var removed = deleteCopyFirst ? copiedBag : source;
		removed.Delete();
		Assert.AreEqual(deleteCopyFirst, copiedClothing.Deleted);
		Assert.AreEqual(deleteCopyFirst, copiedImplant.Deleted);
		Assert.AreEqual(deleteCopyFirst, copiedLodged.Deleted);
		Assert.AreEqual(!deleteCopyFirst, clothing.Deleted);
		Assert.AreEqual(!deleteCopyFirst, implant.Deleted);
		Assert.AreEqual(!deleteCopyFirst, lodged.Deleted);
		var survivingWound = deleteCopyFirst ? wound : copiedWound;
		Assert.AreSame(deleteCopyFirst ? nested : copiedPart, survivingWound.Parent);
		(deleteCopyFirst ? source : copiedBag).Delete();
	}


	[TestMethod]
	[DoNotParallelize]
	public void DeepCopy_SeveredPart_PersistsAndReloadsOwnedItemsAndFracture()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var db = new FuturemudDatabaseContext(options);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var manager = new SaveManager();
		world.SetupGet(x => x.SaveManager).Returns(manager);
		var prototype = Mock.Of<IGameItemProto>(x => x.Id == 77 && x.Gameworld == world.Object);
		world.Setup(x => x.ItemProtos.Get(77, 0)).Returns(prototype);
		var componentPrototype = (BodypartGameItemComponentProto)new GameItemComponentManager().GetProto(
			new MudSharp.Models.GameItemComponentProto
			{
				Id = 91, Type = "Bodypart", Name = "remains", Definition = "<Definition />",
				EditableItem = new MudSharp.Models.EditableItem()
			}, world.Object)!;
		world.Setup(x => x.ItemComponentProtos.Get(91, 0)).Returns(componentPrototype);
		var bone = Mock.Of<IBone>(x => x.Id == 19);
		var model = Mock.Of<ICorpseModel>(x => x.Id == 4);
		world.Setup(x => x.BodypartPrototypes.Get(19)).Returns(bone);
		world.Setup(x => x.CorpseModels.Get(4)).Returns(model);
		var source = new TestItem(prototype, temporary: true);
		var sourceClothing = new TestItem(prototype, temporary: true);
		var sourceLodged = new TestItem(prototype, temporary: true);
		var fracture = WoundFactory.LoadWound(new MudSharp.Models.Wound
		{
			WoundType = "BoneFracture", ExtraInformation = "<Definition><Stage>3</Stage></Definition>",
			BodypartProtoId = 19, OriginalDamage = 18, CurrentDamage = 12
		}, source, world.Object);
		var piercing = WoundFactory.LoadWound(new MudSharp.Models.Wound
		{
			WoundType = "SimpleOrganic", ExtraInformation = "<Definition />",
			BodypartProtoId = 19, OriginalDamage = 10, CurrentDamage = 6
		}, source, world.Object);
		piercing.Lodged = sourceLodged;
		var remains = new BodypartGameItemComponent(componentPrototype, source, temporary: true)
		{
			Model = model, Parts = [bone], Bones = [bone], Contents = [sourceClothing], Wounds = [fracture, piercing]
		};
		sourceClothing.LoadTimeSetContainedIn(source);
		source.Attach(remains);

		var ambient = typeof(FMDB).GetProperty(nameof(FMDB.Context))!;
		var previous = ambient.GetValue(null);
		ambient.SetValue(null, db);
		try
		{
			using var scope = new FMDB();
			var copy = source.DeepCopy(addToGameworld: true, preserveMorphTime: true);
			manager.Flush();
			var copyId = copy.Id;
			var copiedClothingId = copy.GetItemType<ISeveredBodypart>().Contents.Single().Id;
			var copiedLodgedId = copy.GetItemType<ISeveredBodypart>().Wounds.Single(x => x is SimpleOrganicWound).Lodged.Id;
			using var fresh = new FuturemudDatabaseContext(options);
			var row = fresh.GameItems.AsNoTracking().Include(x => x.GameItemComponents)
				.Include(x => x.WoundsGameItem).Single(x => x.Id == copyId);
			var clothingRow = fresh.GameItems.AsNoTracking().Single(x => x.Id == copiedClothingId);
			Assert.AreEqual(copyId, clothingRow.ContainerId);
			var fractureRow = row.WoundsGameItem.Single(x => x.WoundType == "BoneFracture");
			var piercingRow = row.WoundsGameItem.Single(x => x.WoundType == "SimpleOrganic");
			Assert.IsTrue(fractureRow.Id > 0);
			Assert.AreEqual(copiedLodgedId, piercingRow.LodgedItemId);
			StringAssert.Contains(row.GameItemComponents.Single().Definition,
				$"<Wound>{fractureRow.Id}</Wound>");
			world.Setup(x => x.TryGetItem(copiedClothingId, true)).Returns(new GameItem(clothingRow, world.Object));
			var lodgedRow = fresh.GameItems.AsNoTracking().Single(x => x.Id == copiedLodgedId);
			Assert.IsNull(lodgedRow.ContainerId);
			world.Setup(x => x.TryGetItem(copiedLodgedId, true)).Returns(new GameItem(lodgedRow, world.Object));

			var reloaded = new GameItem(row, world.Object);
			var reloadedRemains = reloaded.GetItemType<ISeveredBodypart>();
			var reloadedFracture = reloadedRemains.Wounds.OfType<BoneFracture>().Single();
			Assert.AreSame(reloadedFracture, reloaded.Wounds.OfType<BoneFracture>().Single());
			Assert.AreSame(reloaded, reloadedFracture.Parent);
			Assert.AreSame(reloaded, reloadedRemains.Contents.Single().ContainedIn);
			Assert.AreEqual(BoneFractureStage.Reparation, reloadedFracture.Stage);
			Assert.AreEqual(12.0, reloadedFracture.CurrentDamage);
			Assert.AreEqual(copiedLodgedId, reloadedRemains.Wounds.OfType<SimpleOrganicWound>().Single().Lodged.Id);
			reloadedRemains.SeveredBodypartWasInstalledInABody();
			Assert.IsEmpty(reloaded.Wounds);
		}
		finally
		{
			ambient.SetValue(null, previous);
		}
	}

	[TestMethod]
	public void Escrow_SuspendsOwnedItemsAndResumesEachEffectOnce()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var effects = new Mock<IEffectScheduler>();
		world.SetupGet(x => x.EffectScheduler).Returns(effects.Object);
		var prototype = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		var item = new TestItem(prototype);
		var child = new TestItem(prototype);
		child.LoadTimeSetContainedIn(item);
		var component = new Mock<IGameItemComponent>();
		component.Setup(x => x.Login()).Callback(child.Login);
		component.Setup(x => x.Quit()).Callback(child.Quit);
		item.Attach(component.Object);
		var timed = new Mock<IEffect>();
		var remaining = TimeSpan.FromSeconds(17);
		child.AddEffect(timed.Object);
		effects.Setup(x => x.IsScheduled(timed.Object)).Returns(true);
		effects.Setup(x => x.RemainingDuration(timed.Object)).Returns(remaining);
		item.Login();

		Assert.IsTrue(item.SuspendForAuctionEscrow());
		item.Login();
		child.Login();
		Assert.IsFalse(item.IsActive);
		Assert.IsFalse(child.IsActive);
		Assert.AreSame(item, child.ContainedIn);
		effects.Verify(x => x.Unschedule(timed.Object, false, false), Times.Once);
		world.Verify(x => x.Destroy(child), Times.Never);

		item.ReleaseFromAuctionEscrow();
		item.Login();
		item.Login();
		Assert.IsTrue(child.IsActive);
		component.Verify(x => x.Login(), Times.Exactly(2));
		timed.Verify(x => x.Login(), Times.Exactly(2));
		effects.Verify(x => x.Reschedule(timed.Object, remaining), Times.Once);
	}

	[TestMethod]
	public void Delete_AfterComponentFailure_RetrySkipsCompletedHooks()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var item = new TestItem(Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object));
		var first = new Mock<IGameItemComponent>();
		var second = new Mock<IGameItemComponent>();
		second.SetupSequence(x => x.Delete()).Throws(new InvalidOperationException("fixture fault")).Pass();
		item.Attach(first.Object);
		item.Attach(second.Object);
		var observed = 0;
		item.OnDeleted += _ => observed++;

		Assert.ThrowsExactly<InvalidOperationException>(item.Delete);
		Assert.IsFalse(item.Deleted);
		item.Delete();
		item.Delete();

		Assert.IsTrue(item.Deleted);
		Assert.AreEqual(1, observed);
		first.Verify(x => x.Delete(), Times.Once);
		second.Verify(x => x.Delete(), Times.Exactly(2));
		world.Verify(x => x.Destroy(item), Times.Once);
	}

	[TestMethod]
	public void Login_AfterComponentFailure_UnwindsEnteredHooksBeforeRetry()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var item = new TestItem(Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object));
		var first = new Mock<IGameItemComponent>();
		var second = new Mock<IGameItemComponent>();
		var third = new Mock<IGameItemComponent>();
		var cleanup = new List<string>();
		first.Setup(x => x.Quit()).Callback(() => cleanup.Add("first"));
		second.Setup(x => x.Quit()).Callback(() => cleanup.Add("second"));
		second.SetupSequence(x => x.Login()).Throws(new InvalidOperationException("fixture fault")).Pass();
		item.Attach(first.Object);
		item.Attach(second.Object);
		item.Attach(third.Object);

		Assert.ThrowsExactly<InvalidOperationException>(item.Login);
		Assert.IsFalse(item.IsActive);
		CollectionAssert.AreEqual(new[] { "second", "first" }, cleanup);
		third.Verify(x => x.Quit(), Times.Never);

		item.Login();
		item.Login();
		Assert.IsTrue(item.IsActive);
		first.Verify(x => x.Login(), Times.Exactly(2));
		third.Verify(x => x.Login(), Times.Once);
	}

	[TestMethod]
	public void NativeDeletionRecovery_IncompleteTeardownRequiresReconciliationBeforeAnyDelete()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var db = new FuturemudDatabaseContext(options);
		db.GameItems.AddRange(
			new MudSharp.Models.GameItem { Id = 1, NativeDeletionStage = MudSharp.Models.NativeItemDeletionStage.ReadyToDelete },
			new MudSharp.Models.GameItem { Id = 2, NativeDeletionStage = MudSharp.Models.NativeItemDeletionStage.TeardownInProgress },
			new MudSharp.Models.GameItem { Id = 3 });
		db.SaveChanges();

		var error = Assert.ThrowsExactly<InvalidOperationException>(() => GameItem.RecoverCheckpointedNativeDeletions(db));

		StringAssert.Contains(error.Message, "Teardown outcome is unknown for item IDs 2.");
		Assert.AreEqual(3, db.GameItems.Count());
	}

	[TestMethod]
	public void NativeContainerDeletionPlan_TakesExactChildrenInChildFirstOrder()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var itemProto = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		world.Setup(x => x.ItemProtos.Get(7, 0)).Returns(itemProto);
		var child = new GameItem(new MudSharp.Models.GameItem { Id = 2, GameItemProtoId = 7 }, world.Object);
		var other = new GameItem(new MudSharp.Models.GameItem { Id = 3, GameItemProtoId = 7 }, world.Object);
		world.Setup(x => x.TryGetItem(2, true)).Returns(child);
		world.Setup(x => x.TryGetItem(3, true)).Returns(other);
		var componentProto = new GameItemComponentManager().GetProto(new MudSharp.Models.GameItemComponentProto
		{
			Id = 44,
			Type = "Container",
			EditableItem = new MudSharp.Models.EditableItem(),
			Definition = "<Definition><WeightLimit>20</WeightLimit><MaximumContentsSize>6</MaximumContentsSize><Transparent>false</Transparent><Closable>false</Closable><OnceOnly>false</OnceOnly><ContentsPreposition>in</ContentsPreposition></Definition>"
		}, world.Object)!;
		world.Setup(x => x.ItemComponentProtos.Get(44, 0)).Returns(componentProto);
		var root = new GameItem(new MudSharp.Models.GameItem
		{
			Id = 1,
			GameItemProtoId = 7,
			GameItemComponents =
			{
				new MudSharp.Models.GameItemComponent
				{
					Id = 41,
					GameItemComponentProtoId = 44,
					Definition = "<Definition><Contained>2</Contained><Contained>3</Contained></Definition>"
				}
			}
		}, world.Object);
		other.LoadTimeSetContainedIn(child);

		var plan = root.BuildOrdinaryContainerDeletionPlan();

		Assert.IsNotNull(plan);
		CollectionAssert.AreEqual(new[] { child, root }, plan.Select(x => x.Item).ToArray());
		Assert.AreSame(root, plan[0].Container);
		child.OnDeleted += _ => { };
		Assert.IsNull(root.BuildOrdinaryContainerDeletionPlan());
	}

	[TestMethod]
	[DataRow(null)]
	[DataRow("foreign room custody")]
	[DataRow("competing wound")]
	[DataRow("extra stored child")]
	[DataRow("different lodged row")]
	public void NativeWoundDeletionGraph_RevalidatesExactPersistedLodgingBeforeAnyDelete(string? conflict)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var db = new FuturemudDatabaseContext(options);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var itemProto = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		world.Setup(x => x.ItemProtos.Get(7, 0)).Returns(itemProto);
		var childRow = new MudSharp.Models.GameItem { Id = 2, GameItemProtoId = 7 };
		var child = new GameItem(childRow, world.Object);
		world.Setup(x => x.TryGetItem(2, true)).Returns(child);
		var woundRow = new MudSharp.Models.Wound
		{
			Id = 11,
			GameItemId = 1,
			WoundType = "Simple",
			LodgedItemId = childRow.Id,
			CurrentDamage = 1
		};
		var rootRow = new MudSharp.Models.GameItem { Id = 1, GameItemProtoId = 7, WoundsGameItem = { woundRow } };
		var root = new GameItem(rootRow, world.Object);
		var plan = root.BuildOrdinaryContainerDeletionPlan();
		Assert.IsNotNull(plan);
		CollectionAssert.AreEqual(new[] { child, root }, plan.Select(x => x.Item).ToArray());
		Assert.AreSame(root.Wounds.Single(), plan[0].LodgedIn);
		Assert.AreSame(plan[0].LodgedIn, plan[1].Wounds.Single());

		db.GameItems.AddRange(rootRow, childRow);
		switch (conflict)
		{
			case "foreign room custody":
				db.RoomsGameItems.Add(new MudSharp.Models.RoomsGameItems { RoomId = 3, GameItemId = childRow.Id });
				break;
			case "competing wound":
				db.Wounds.Add(new MudSharp.Models.Wound { Id = 12, BodyId = 1, LodgedItemId = childRow.Id });
				break;
			case "extra stored child":
				db.GameItems.Add(new MudSharp.Models.GameItem { Id = 3, ContainerId = rootRow.Id });
				break;
			case "different lodged row":
				woundRow.LodgedItemId = null;
				break;
		}
		db.SaveChanges();
		if (conflict is null)
		{
			CollectionAssert.AreEquivalent(new long[] { 1, 2 }, GameItem.ValidateNativeDeletionCustody(db, plan).Keys.ToArray());
		}
		else
		{
			Assert.ThrowsExactly<InvalidOperationException>(() => GameItem.ValidateNativeDeletionCustody(db, plan));
		}
		Assert.IsTrue(db.GameItems.Any(x => x.Id == childRow.Id));
		Assert.IsTrue(db.Wounds.Any(x => x.Id == woundRow.Id));
	}

	[TestMethod]
	public void NativeWoundDeletion_ConservesALodgedItemThatHasMovedToAnotherOwner()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		var root = new TestItem(proto, temporary: true);
		var foreignContainer = new TestItem(proto, temporary: true);
		var lodged = new TestItem(proto, temporary: true);
		lodged.LoadTimeSetContainedIn(foreignContainer);
		var wound = new SimpleWound(root, new MudSharp.Models.Wound(), world.Object) { Lodged = lodged };

		root.DeleteNativeItemWound(wound);

		Assert.IsNull(wound.Lodged);
		Assert.IsFalse(lodged.Deleted);
		Assert.AreSame(foreignContainer, lodged.ContainedIn);
	}

	[TestMethod]
	public void NativeWoundDeletion_KeepsTheWoundIfItsOwnedLodgedItemRefusesDeletion()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var proto = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		var root = new TestItem(proto, temporary: true);
		var lodged = new Mock<IGameItem>();
		var wound = new SimpleWound(root, new MudSharp.Models.Wound { Id = 11 }, world.Object) { Lodged = lodged.Object };

		Assert.ThrowsExactly<InvalidOperationException>(() => root.DeleteNativeItemWound(wound));

		lodged.Verify(x => x.Delete(), Times.Once);
		Assert.AreSame(lodged.Object, wound.Lodged);
	}

	[TestMethod]
	public void RemoveExact_SupersededInstance_DoesNotUnregisterCurrentInstance()
	{
		var registry = new All<IGameItem>();
		var current = Mock.Of<IGameItem>(x => x.Id == 7);
		var detached = Mock.Of<IGameItem>(x => x.Id == 7);
		registry.Add(current);

		Assert.IsFalse(registry.RemoveExact(detached));
		Assert.AreSame(current, registry.Get(7));
		Assert.IsTrue(registry.RemoveExact(current));
		Assert.IsNull(registry.Get(7));
	}

	[TestMethod]
	public void Login_EffectDeletesItsItem_StopsDispatchingRemovedEffects()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var item = new TestItem(Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object));
		var first = new Mock<IEffect>();
		var second = new Mock<IEffect>();
		first.Setup(x => x.Login()).Callback(item.Delete);
		item.AddEffect(first.Object);
		item.AddEffect(second.Object);

		item.Login();
		item.Login();

		Assert.IsTrue(item.Deleted);
		first.Verify(x => x.Login(), Times.Once);
		second.Verify(x => x.Login(), Times.Never);
	}

	[TestMethod]
	public void FinaliseLoad_AfterComponentFailure_RetrySkipsCompletedHooks()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var prototype = Mock.Of<IGameItemProto>(x => x.Gameworld == world.Object);
		world.Setup(x => x.ItemProtos.Get(77, 0)).Returns(prototype);
		var firstRow = new MudSharp.Models.GameItemComponent { Id = 8, GameItemComponentProtoId = 11 };
		var secondRow = new MudSharp.Models.GameItemComponent { Id = 9, GameItemComponentProtoId = 11 };
		var componentPrototype = new Mock<IGameItemComponentProto>();
		world.Setup(x => x.ItemComponentProtos.Get(11, 0)).Returns(componentPrototype.Object);
		var first = new Mock<IGameItemComponent>();
		var second = new Mock<IGameItemComponent>();
		second.SetupSequence(x => x.FinaliseLoad()).Throws(new InvalidOperationException("fixture fault")).Pass();
		componentPrototype.Setup(x => x.LoadComponent(firstRow, It.IsAny<IGameItem>())).Returns(first.Object);
		componentPrototype.Setup(x => x.LoadComponent(secondRow, It.IsAny<IGameItem>())).Returns(second.Object);
		var item = new GameItem(new MudSharp.Models.GameItem
		{
			Id = 7, GameItemProtoId = 77, GameItemComponents = new List<MudSharp.Models.GameItemComponent>
			{
				firstRow, secondRow
			}
		}, world.Object);

		Assert.ThrowsExactly<InvalidOperationException>(item.FinaliseLoadTimeTasks);
		item.FinaliseLoadTimeTasks();
		item.FinaliseLoadTimeTasks();

		first.Verify(x => x.FinaliseLoad(), Times.Once);
		second.Verify(x => x.FinaliseLoad(), Times.Exactly(2));
		first.Verify(x => x.Login(), Times.Never);
	}

	[TestMethod]
	public void UnlimitedGenerator_PersistedSwitch_OnlyPublishesPowerDuringAnActiveInterval()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var heartbeat = new Mock<IHeartbeatManager>();
		world.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
		var parent = Mock.Of<IGameItem>(x => x.Gameworld == world.Object);
		var prototype = new GameItemComponentManager().GetProto(new MudSharp.Models.GameItemComponentProto
		{
			Id = 1,
			Type = "UnlimitedGenerator",
			EditableItem = new MudSharp.Models.EditableItem(),
			Definition = """
			             <Definition>
			               <SwitchOnEmote/><SwitchOffEmote/><WattageProvided>100</WattageProvided>
			               <SwitchOnProg>0</SwitchOnProg><SwitchOffProg>0</SwitchOffProg>
			             </Definition>
			             """
		}, world.Object)!;
		var generator = (UnlimitedGeneratorGameItemComponent)prototype.LoadComponent(new MudSharp.Models.GameItemComponent
		{
			Id = 7,
			Definition = "<Definition On=\"true\" />"
		}, parent);
		var consumer = new Mock<IConsumePower>();
		consumer.SetupGet(x => x.PowerConsumptionInWatts).Returns(10.0);
		generator.FinaliseLoad();
		generator.BeginDrawdown(consumer.Object);
		Assert.IsTrue(generator.SwitchedOn);
		Assert.IsFalse(generator.ProducingPower);
		consumer.Verify(x => x.OnPowerCutIn(), Times.Never);
		heartbeat.VerifyAdd(x => x.SecondHeartbeat += It.IsAny<HeartbeatManagerDelegate>(), Times.Never);

		generator.Login();
		generator.Login();
		Assert.AreEqual(100.0, generator.MaximumPowerInWatts);
		consumer.Verify(x => x.OnPowerCutIn(), Times.Once);
		heartbeat.VerifyAdd(x => x.SecondHeartbeat += It.IsAny<HeartbeatManagerDelegate>(), Times.Once);
		generator.Quit();
		generator.Quit();
		Assert.IsFalse(generator.ProducingPower);
		Assert.IsTrue(generator.SwitchedOn);
		consumer.Verify(x => x.OnPowerCutOut(), Times.Once);
		heartbeat.VerifyRemove(x => x.SecondHeartbeat -= It.IsAny<HeartbeatManagerDelegate>(), Times.Once);

		generator.Login();
		generator.Delete();
		Assert.IsFalse(generator.ProducingPower);
		consumer.Verify(x => x.OnPowerCutIn(), Times.Exactly(2));
		consumer.Verify(x => x.OnPowerCutOut(), Times.Exactly(2));
		heartbeat.VerifyRemove(x => x.SecondHeartbeat -= It.IsAny<HeartbeatManagerDelegate>(), Times.Exactly(2));
	}

	[TestMethod]
	public void FuelGenerator_PersistedSwitchAndFuel_OnlyConsumeDuringAnActiveInterval()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var fuel = new Mock<ILiquid>();
		fuel.Setup(x => x.LiquidCountsAs(fuel.Object)).Returns(true);
		world.Setup(x => x.Liquids.Get(22)).Returns(fuel.Object);
		var heartbeat = new Mock<IHeartbeatManager>();
		world.SetupGet(x => x.HeartbeatManager).Returns(heartbeat.Object);
		HeartbeatManagerDelegate? tick = null;
		heartbeat.SetupAdd(x => x.SecondHeartbeat += It.IsAny<HeartbeatManagerDelegate>())
			.Callback<HeartbeatManagerDelegate>(callback => tick += callback);
		heartbeat.SetupRemove(x => x.SecondHeartbeat -= It.IsAny<HeartbeatManagerDelegate>())
			.Callback<HeartbeatManagerDelegate>(callback => tick -= callback);
		var model = new MudSharp.Models.GameItemComponentProto
		{
			Id = 1,
			EditableItem = new MudSharp.Models.EditableItem(),
			Definition = """
			             <Definition>
			               <SwitchOnEmote></SwitchOnEmote><SwitchOffEmote></SwitchOffEmote>
			               <FuelExpendedEmote></FuelExpendedEmote><LiquidFuel>22</LiquidFuel>
			               <FuelPerSecond>1</FuelPerSecond><FuelCapacity>10</FuelCapacity>
			               <WattageProvided>100</WattageProvided><SwitchOnProg>0</SwitchOnProg>
			               <SwitchOffProg>0</SwitchOffProg><FuelOutProg>0</FuelOutProg>
			             </Definition>
			             """
		};
		var prototype = (FuelGeneratorGameItemComponentProto)typeof(FuelGeneratorGameItemComponentProto)
			.GetConstructor(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([model, world.Object]);
		var parent = Mock.Of<IGameItem>(x => x.Gameworld == world.Object);
		var generator = new FuelGeneratorGameItemComponent(new MudSharp.Models.GameItemComponent
		{
			Id = 7,
			Definition = "<Definition On=\"true\" Liquid=\"22\" LiquidQuantity=\"10\" />"
		}, prototype, parent);

		Assert.IsTrue(generator.SwitchedOn);
		Assert.IsFalse(generator.ProducingPower);
		Assert.IsNull(tick);
		Assert.AreEqual(10.0, generator.LiquidMixture.TotalVolume);
		generator.Login();
		generator.Login();
		Assert.AreEqual(100.0, generator.MaximumPowerInWatts);
		Assert.IsNotNull(tick);
		tick.Invoke();
		generator.Quit();
		generator.Quit();
		Assert.IsNull(tick);
		Assert.IsTrue(generator.SwitchedOn);
		Assert.AreEqual(9.0, generator.LiquidMixture.TotalVolume);
		generator.Login();
		Assert.IsNotNull(tick);
		tick.Invoke();
		Assert.AreEqual(8.0, generator.LiquidMixture.TotalVolume);
	}

	private sealed class TestItem(IGameItemProto prototype, bool temporary = false)
		: GameItem(prototype, null, ItemQuality.Standard, deferSpellInitialisation: temporary)
	{
		internal void Attach(IGameItemComponent component) => _components.Add(component);
	}
}

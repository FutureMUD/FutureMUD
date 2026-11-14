#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests;

[TestClass]
public class StartupGraphAndInitialisationTests
{
	private sealed class LateFixture(IFuturemud world, long identity) : LateInitialisingItem
	{
		public override string FrameworkItemType => "Fixture";
		public override string Name => "late fixture";
		public override object DatabaseInsert() => identity;
		public override void SetIDFromDatabase(object item) => _id = (long)item;
		public override void Save() => Changed = false;
		public void Attach() { Gameworld = world; world.SaveManager.AddInitialisation(this); }
	}

	[TestMethod]
	public void LateIdentityRead_CustomSaveManagerRetainsItsExistingFlushContract()
	{
		var manager = new Mock<ISaveManager>();
		var world = Mock.Of<IFuturemud>(x => x.SaveManager == manager.Object);
		var item = new LateFixture(world, 1); item.Attach();
		manager.Setup(x => x.Flush()).Callback(() => item.InitialiseWithoutPersistence(123));
		Assert.AreEqual(123L, item.Id);
		manager.Verify(x => x.Flush(), Times.Once);
	}

	[TestMethod]
	public void LateIdentityRead_StartupScopeDefersDirtySavesAndRestoresNormalFlushOnDisposal()
	{
		using var database = new InMemoryDatabaseTestScope();
		var manager = new SaveManager(); var world = Mock.Of<IFuturemud>(x => x.SaveManager == manager);
		var dirty = new Mock<ISaveable>(); manager.Add(dirty.Object);
		var first = new LateFixture(world, 101); first.Attach();
		using (manager.DeferUnrelatedIdentitySaves())
		{
			Assert.AreEqual(101L, first.Id);
			Assert.IsTrue(manager.IsQueued(dirty.Object)); dirty.Verify(x => x.Save(), Times.Never);
		}
		var second = new LateFixture(world, 102); second.Attach();
		Assert.AreEqual(102L, second.Id);
		dirty.Verify(x => x.Save(), Times.Once);
		Assert.IsFalse(manager.IsQueued(dirty.Object));
	}

	[TestMethod]
	public void CharacterGraph_LoadsOnlySelectedLivingRootsIncludingEmptyCollectionsAndAdditionalBodies()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var body = new MudSharp.Models.Body { Id = 10 };
		var other = new MudSharp.Models.Body { Id = 11 };
		var actor = new MudSharp.Models.Character { Id = 1, Body = body, BodyId = 10 };
		actor.CharacterBodies.Add(new MudSharp.Models.CharacterBody { CharacterId = 1, BodyId = 11, Body = other, Alias = "alternate" });
		context.Characters.AddRange(actor,
			new MudSharp.Models.Character { Id = 2, BodyId = 10, Body = body },
			new MudSharp.Models.Character { Id = 3, BodyId = 10, Body = body, IsArchived = true });
		context.SaveChanges(); context.ChangeTracker.Clear();
		var graph = CharacterBootGraph.Load(context, [1, 1, 3]);
		CollectionAssert.AreEqual(new long[] { 1 }, graph.Keys.ToArray());
		var loaded = graph[1];
		Assert.IsTrue(context.Entry(loaded).Collection(x => x.CharactersSignedLanguages).IsLoaded);
		Assert.IsTrue(context.Entry(loaded).Collection(x => x.CharacterInstances).IsLoaded);
		Assert.IsTrue(context.Entry(loaded.Body).Collection(x => x.BodiesImplants).IsLoaded);
		Assert.IsTrue(context.Entry(loaded.CharacterBodies.Single().Body).Collection(x => x.BodiesDrugDoses).IsLoaded);
		Assert.AreEqual(EntityState.Unchanged, context.Entry(loaded).State);
		loaded.Body.Weight = 42;
		Assert.IsTrue(context.ChangeTracker.HasChanges(), "The graph must remain tracked for secondary-instance and lifecycle repairs.");
	}

	[TestMethod]
	public void InitialisationQueue_CommitsNewIdentityWithoutDrainingUnrelatedDirtySaves()
	{
		using var database = new InMemoryDatabaseTestScope();
		var manager = new SaveManager();
		var dirty = new Mock<ISaveable>();
		manager.Add(dirty.Object);
		var initialised = false;
		var fresh = new Mock<ILateInitialisingItem>();
		fresh.SetupGet(x => x.InitialisationPhase).Returns(InitialisationPhase.First);
		fresh.Setup(x => x.InitialiseItem()).Returns(() => () => initialised = true);
		manager.AddInitialisation(fresh.Object);
		manager.FlushInitialisationQueue();
		Assert.IsTrue(initialised);
		Assert.IsTrue(manager.IsQueued(dirty.Object));
		dirty.Verify(x => x.Save(), Times.Never);
		Assert.IsFalse(manager.Flushing);
	}
}

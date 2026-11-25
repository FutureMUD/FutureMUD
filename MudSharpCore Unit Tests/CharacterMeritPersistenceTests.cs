#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.CharacterCreation.Resources;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class CharacterMeritPersistenceTests
{
	[TestMethod]
	public void Save_AddedCosts_ReloadsSpentAndRequiredAmounts()
	{
		using var fixture = new MeritFixture(Row(10));
		fixture.Edit(10, "cost 1 7");
		fixture.Edit(10, "cost 2 12");
		fixture.Edit(10, "cost 2 spend");
		fixture.SaveAndReload();

		Assert.AreEqual(7, fixture.Merit(10).ResourceCost(fixture.FirstResource));
		Assert.AreEqual(0, fixture.Merit(10).ResourceRequirement(fixture.FirstResource));
		Assert.AreEqual(0, fixture.Merit(10).ResourceCost(fixture.SecondResource));
		Assert.AreEqual(12, fixture.Merit(10).ResourceRequirement(fixture.SecondResource));
		Assert.AreEqual(2, fixture.Context.MeritsChargenResources.Count());
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Save_ChangedAmountAndRequirementMode_ReloadsReplacementKey(bool requirementOnly)
	{
		var row = Row(10);
		row.MeritsChargenResources.Add(Cost(10, 1, 3, requirementOnly));
		using var fixture = new MeritFixture(row);
		fixture.Edit(10, "cost 1 -4");
		fixture.SaveAndReload();
		AssertStoredCost(-4, requirementOnly);

		fixture.Edit(10, "cost 1 spend");
		fixture.SaveAndReload();
		AssertStoredCost(-4, !requirementOnly);

		void AssertStoredCost(int amount, bool required)
		{
			var stored = fixture.Context.MeritsChargenResources.AsNoTracking().Single();
			Assert.AreEqual(amount, stored.Amount);
			Assert.AreEqual(required, stored.RequirementOnly);
			Assert.AreEqual(required ? 0 : amount, fixture.Merit(10).ResourceCost(fixture.FirstResource));
			Assert.AreEqual(required ? amount : 0, fixture.Merit(10).ResourceRequirement(fixture.FirstResource));
		}
	}

	[TestMethod]
	public void Save_RemovedCost_DeletesOnlyThatMeritsResourceRow()
	{
		var row = Row(10);
		row.MeritsChargenResources.Add(Cost(10, 1, 5, false));
		row.MeritsChargenResources.Add(Cost(10, 2, 8, true));
		var unrelated = Row(11);
		unrelated.MeritsChargenResources.Add(Cost(11, 1, 15, false));
		using var fixture = new MeritFixture(row, unrelated);
		fixture.Edit(10, "cost 1 0");
		fixture.SaveAndReload();

		Assert.AreEqual(0, fixture.Merit(10).ResourceCost(fixture.FirstResource));
		Assert.AreEqual(0, fixture.Merit(10).ResourceRequirement(fixture.FirstResource));
		Assert.AreEqual(8, fixture.Merit(10).ResourceRequirement(fixture.SecondResource));
		Assert.AreEqual(15, fixture.Merit(11).ResourceCost(fixture.FirstResource));
		Assert.AreEqual(2, fixture.Context.MeritsChargenResources.Count());
		Assert.IsFalse(fixture.Context.MeritsChargenResources.Any(x => x.MeritId == 10 && x.ChargenResourceId == 1));
	}

	[TestMethod]
	public void Save_LegacySpentAndRequiredRowsForSameResource_PreservesBoth()
	{
		var row = Row(10);
		row.MeritsChargenResources.Add(Cost(10, 1, 5, false));
		row.MeritsChargenResources.Add(Cost(10, 1, 9, true));
		using var fixture = new MeritFixture(row);
		fixture.Edit(10, "name Renamed Merit");
		fixture.SaveAndReload();

		Assert.AreEqual(5, fixture.Merit(10).ResourceCost(fixture.FirstResource));
		Assert.AreEqual(9, fixture.Merit(10).ResourceRequirement(fixture.FirstResource));
		Assert.AreEqual(2, fixture.Context.MeritsChargenResources.Count());
	}

	[TestMethod]
	public void BuildingCommandCost_DualModeLegacyRows_RejectsToggleAndRemainsSaveable()
	{
		var row = Row(10);
		row.MeritsChargenResources.Add(Cost(10, 1, 5, false));
		row.MeritsChargenResources.Add(Cost(10, 1, 9, true));
		using var fixture = new MeritFixture(row);
		Assert.IsFalse(fixture.TryEdit(10, "cost 1 spend"));
		Assert.IsFalse(fixture.Merit(10).Changed);
		fixture.Edit(10, "name Preserved Merit");
		fixture.SaveAndReload();

		Assert.AreEqual(5, fixture.Merit(10).ResourceCost(fixture.FirstResource));
		Assert.AreEqual(9, fixture.Merit(10).ResourceRequirement(fixture.FirstResource));
		Assert.AreEqual(2, fixture.Context.MeritsChargenResources.Count());
	}

	[TestMethod]
	public void Save_RepeatedBeforeCommit_DoesNotDuplicateNewCostRows()
	{
		using var fixture = new MeritFixture(Row(10));
		fixture.Edit(10, "cost 1 5");
		using (new FMDB())
		{
			fixture.Merit(10).Save();
			fixture.Edit(10, "cost 1 9");
			fixture.Merit(10).Save();
			fixture.Context.SaveChanges();
		}
		fixture.Reload();

		Assert.AreEqual(9, fixture.Merit(10).ResourceCost(fixture.FirstResource));
		Assert.AreEqual(1, fixture.Context.MeritsChargenResources.Count());
	}

	[TestMethod]
	public void Save_ParentSetReparentAndClear_ReloadsBothSidesOfComboMembership()
	{
		using var fixture = new MeritFixture(Row(10), Row(20, "Combo"), Row(21, "Combo"));
		fixture.Edit(10, "parent 20");
		fixture.SaveAndReload();
		AssertMembership(20);

		fixture.Edit(10, "parent 21");
		fixture.SaveAndReload();
		AssertMembership(21);

		fixture.Edit(10, "parent none");
		fixture.SaveAndReload();
		AssertMembership(null);

		void AssertMembership(long? parentId)
		{
			Assert.AreEqual(parentId, fixture.Context.Merits.AsNoTracking().Single(x => x.Id == 10).ParentId);
			Assert.AreEqual(parentId, fixture.Merit(10).ParentMerit?.Id);
			foreach (var id in new[] { 20L, 21L })
			{
				var children = ((ComboMerit)fixture.Merit(id)).CharacterMerits.Select(x => x.Id).ToArray();
				CollectionAssert.AreEqual(parentId == id ? new[] { 10L } : Array.Empty<long>(), children);
			}
		}
	}

	[TestMethod]
	public void Save_UnresolvedLoadedParent_PreservesBacklinkDuringUnrelatedEdit()
	{
		var child = Row(10);
		child.ParentId = 20;
		var parent = Row(20, "Combo");
		var definition = XElement.Parse(parent.Definition);
		definition.Add(new XElement("Children", new XElement("Child", 10)));
		parent.Definition = definition.ToString();
		using var fixture = new MeritFixture(child, parent);
		// Do not access ParentMerit before saving: its lazy cache has not resolved yet.
		fixture.Edit(10, "cost 1 3");
		fixture.SaveAndReload();

		Assert.AreEqual(20L, fixture.Merit(10).ParentMerit?.Id);
		Assert.AreEqual(3, fixture.Merit(10).ResourceCost(fixture.FirstResource));
		CollectionAssert.AreEqual(new[] { 10L }, ((ComboMerit)fixture.Merit(20)).CharacterMerits.Select(x => x.Id).ToArray());
	}

	private static Merit Row(long id, string type = "Simple Character") => new()
	{
		Id = id,
		Name = $"Merit {id}",
		Type = type,
		MeritType = (int)MeritType.Merit,
		MeritScope = (int)MeritScope.Character,
		Definition = new XElement("Merit",
			new XElement("ChargenAvailableProg", 1),
			new XElement("ApplicabilityProg", 1),
			new XElement("ChargenBlurb", "Test merit"),
			new XElement("DescriptionText", "@ have|has a test merit")).ToString()
	};

	private static MeritsChargenResources Cost(long meritId, long resourceId, int amount, bool required) => new()
	{
		MeritId = meritId,
		ChargenResourceId = resourceId,
		Amount = amount,
		RequirementOnly = required
	};

	private sealed class MeritFixture : IDisposable
	{
		private readonly InMemoryDatabaseTestScope _database = new();
		private readonly Mock<IFuturemud> _world = new();
		private readonly Mock<ICharacter> _actor = new();
		private All<IMerit> _merits = new();

		public FuturemudDatabaseContext Context => _database.Context;
		public IChargenResource FirstResource { get; } = Resource(1, "Point", "Points");
		public IChargenResource SecondResource { get; } = Resource(2, "Credit", "Credits");

		public MeritFixture(params Merit[] rows)
		{
			SimpleCharacterMerit.RegisterMeritInitialiser();
			ComboMerit.RegisterMeritInitialiser();
			var resources = new All<IChargenResource>();
			resources.Add(FirstResource);
			resources.Add(SecondResource);
			var progs = new All<IFutureProg>();
			var prog = new Mock<IFutureProg>();
			prog.SetupGet(x => x.Id).Returns(1);
			progs.Add(prog.Object);
			_world.SetupGet(x => x.FutureProgs).Returns(progs);
			_world.SetupGet(x => x.ChargenResources).Returns(resources);
			_world.SetupGet(x => x.Merits).Returns(() => _merits);
			_world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			_actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			_actor.Setup(x => x.GetFormat(typeof(NumberFormatInfo))).Returns(CultureInfo.InvariantCulture.NumberFormat);
			Context.Merits.AddRange(rows);
			Context.SaveChanges();
			Reload();
		}

		public CharacterMeritBase Merit(long id) => (CharacterMeritBase)_merits.Get(id);

		public bool TryEdit(long id, string command) => Merit(id).BuildingCommand(_actor.Object, new StringStack(command));

		public void Edit(long id, string command)
		{
			Assert.IsTrue(TryEdit(id, command), command);
		}

		public void SaveAndReload()
		{
			using (new FMDB())
			{
				foreach (var merit in _merits.OfType<MeritBase>().Where(x => x.Changed).ToList())
				{
					merit.Save();
				}
				Context.SaveChanges();
			}
			Reload();
		}

		public void Reload()
		{
			Context.ChangeTracker.Clear();
			_merits = new All<IMerit>();
			foreach (var row in Context.Merits.Include(x => x.MeritsChargenResources).AsNoTracking().ToList())
			{
				_merits.Add(MeritFactory.LoadMerit(row, _world.Object));
			}
		}

		public void Dispose() => _database.Dispose();

		private static IChargenResource Resource(long id, string name, string plural)
		{
			var resource = new Mock<IChargenResource>();
			resource.SetupGet(x => x.Id).Returns(id);
			resource.SetupGet(x => x.Name).Returns(name);
			resource.SetupGet(x => x.PluralName).Returns(plural);
			resource.SetupGet(x => x.Alias).Returns(name);
			return resource.Object;
		}
	}
}

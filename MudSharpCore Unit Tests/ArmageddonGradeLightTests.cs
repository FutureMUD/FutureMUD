#nullable enable

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonGradeLightTests
{
	[TestMethod]
	[DataRow(1, 20.0)]
	[DataRow(7, 140.0)]
	public void GradedLight_ValidCreationBoundary_ProvidesExpectedLuxAndRetainsOffSwitch(int grade, double expected)
	{
		var world = World();
		var parent = Parent(world.Object, Origin(grade));
		var light = new ProgLightGameItemComponent(Prototype(world.Object, scale: true), parent.Object, temporary: true) { Lit = true };
		Assert.AreEqual(expected, light.CurrentIllumination, 0.000001);
		light.Lit = false;
		Assert.AreEqual(0.0, light.CurrentIllumination);
		light.Lit = true;
		Assert.AreEqual(expected, light.CurrentIllumination, 0.000001);
		world.VerifyGet(x => x.SpellOwnedItems, Times.Never, "Reading illumination must use the loaded immutable origin without database lookups.");
	}

	[TestMethod]
	[DataRow(-1)]
	[DataRow(0)]
	[DataRow(8)]
	public void GradedLight_InvalidCreationGrade_ProvidesNoIllumination(int grade)
	{
		var world = World();
		var light = new ProgLightGameItemComponent(Prototype(world.Object, scale: true), Parent(world.Object, Origin(grade)).Object, temporary: true) { Lit = true };
		Assert.AreEqual(0.0, light.CurrentIllumination);
		Assert.IsTrue(light.Lit, "Refused authority does not alter the native switch state.");
	}

	[TestMethod]
	public void GradedLight_UnboundParent_ProvidesNoIllumination()
	{
		var world = World();
		var light = new ProgLightGameItemComponent(Prototype(world.Object, scale: true), Parent(world.Object, null).Object, temporary: true) { Lit = true };
		Assert.AreEqual(0.0, light.CurrentIllumination);
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void GradedLight_ComponentCopy_CannotBorrowParentOriginEvenAfterXmlReload(bool temporary)
	{
		var world = World();
		var prototype = Prototype(world.Object, scale: true);
		var original = new ProgLightGameItemComponent(prototype, Parent(world.Object, Origin(7)).Object, temporary: true) { Lit = true };
		var copiedParent = Parent(world.Object, Origin(7));
		var copied = (ProgLightGameItemComponent)original.Copy(copiedParent.Object, temporary);
		Assert.AreEqual(140.0, original.CurrentIllumination);
		Assert.IsTrue(copied.Lit);
		Assert.AreEqual(0.0, copied.CurrentIllumination, "A copied parent's inherited origin is not authority for the copied component.");
		var row = new MudSharp.Models.GameItemComponent { Id = 401, Definition = ComponentXml(copied) };
		var reloaded = new ProgLightGameItemComponent(row, prototype, copiedParent.Object);
		Assert.AreEqual(0.0, reloaded.CurrentIllumination, "Saving and loading a copied component must retain its denial of borrowed grade authority.");
		Assert.AreEqual(140.0, original.CurrentIllumination);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void OrdinaryLight_DisabledOrLegacyScaling_PreservesFixedIlluminationAndCopies(bool omitFlag)
	{
		var world = World();
		var prototype = Prototype(world.Object, scale: false, omitFlag: omitFlag);
		Assert.IsFalse(prototype.MultiplyIlluminationBySpellGrade);
		foreach (var origin in new SpellOwnedItemOrigin?[] { null, Origin(0), Origin(7) })
		{
			var parent = Parent(world.Object, origin);
			var light = new ProgLightGameItemComponent(prototype, parent.Object, temporary: true) { Lit = true };
			Assert.AreEqual(20.0, light.CurrentIllumination);
			Assert.AreEqual(20.0, ((ProgLightGameItemComponent)light.Copy(parent.Object, temporary: true)).CurrentIllumination);
			light.Lit = false;
			Assert.AreEqual(0.0, light.CurrentIllumination);
		}
	}

	[TestMethod]
	public void BuilderToggle_PrototypeXmlRoundTrip_PreservesFlagAndLocalizedDisplay()
	{
		var world = World();
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.GetFormat(It.IsAny<Type>())).Returns((Type type) => CultureInfo.GetCultureInfo("de-DE").GetFormat(type));
		var prototype = Prototype(world.Object, scale: false);
		Assert.IsTrue(prototype.BuildingCommand(actor.Object, new StringStack("spellgrade")));
		Assert.IsTrue(prototype.MultiplyIlluminationBySpellGrade);
		var loaded = new LightPrototype(world.Object, prototype.ExportXml());
		Assert.IsTrue(loaded.MultiplyIlluminationBySpellGrade);
		Assert.AreEqual(20.0, loaded.IlluminationProvided);
		StringAssert.Contains(loaded.ShowBuildingHelp, "spellgrade");
		StringAssert.Contains(loaded.ComponentDescriptionOLC(actor.Object), "20,00 lux per creation spell grade");
		StringAssert.Contains(loaded.ComponentDescriptionOLC(actor.Object), "Unbound, copied, and invalid-grade");
		Assert.IsTrue(loaded.BuildingCommand(actor.Object, new StringStack("spellgrade")));
		Assert.IsFalse(new LightPrototype(world.Object, loaded.ExportXml()).MultiplyIlluminationBySpellGrade);
	}

	[TestMethod]
	public void SavedNativeItemAndComponent_ColdReloadUsesPersistedGradeWithoutDuplicatingRows()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString("N"), x => x.EnableNullChecks(false)).Options;
		using var db = new FuturemudDatabaseContext(options);
		var world = World();
		var prototype = new LightPrototype(world.Object, Prototype(world.Object, scale: true).ExportXml());
		var persistedOrigin = Origin(7);
		var sourceParent = Parent(world.Object, persistedOrigin);
		var source = new ProgLightGameItemComponent(prototype, sourceParent.Object, temporary: true) { Lit = true };
		var itemRow = new MudSharp.Models.GameItem { Id = 301, GameItemProtoId = 101, GameItemProtoRevision = 0, EffectData = "<Effects/>" };
		itemRow.GameItemComponents.Add(new MudSharp.Models.GameItemComponent
		{
			Id = 401, GameItemId = 301, GameItemComponentProtoId = 201, GameItemComponentProtoRevision = 0, Definition = ComponentXml(source)
		});
		var lifecycle = new MudSharp.Models.MagicSpellLifecycle
		{
			Id = persistedOrigin.LifecycleId, Grade = persistedOrigin.Grade, CreatorId = persistedOrigin.CreatorId, SpellId = persistedOrigin.SpellId,
			Mode = (int)persistedOrigin.Mode, DeadlineUtc = persistedOrigin.DeadlineUtc, CreatedUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
			Family = "light", Provenance = "grade-light-regression", Diagnostic = ""
		};
		lifecycle.Entities.Add(new MudSharp.Models.MagicSpellOwnedEntity { EntityId = 301, Kind = (int)SpellOwnedEntityKind.GameItem });
		db.GameItems.Add(itemRow); db.MagicSpellLifecycles.Add(lifecycle); db.SaveChanges(); db.ChangeTracker.Clear();
		var itemPrototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
		itemPrototype.SetupGet(x => x.Name).Returns("a native light");
		world.Setup(x => x.ItemProtos.Get(101, 0)).Returns(itemPrototype.Object);
		world.Setup(x => x.ItemComponentProtos.Get(201, 0)).Returns(prototype);
		var service = new Mock<ISpellOwnedItemService>();
		// Storage adapter reads the saved native lifecycle/claim. The actual GameItem load constructor
		// obtains its immutable origin through this service before loading its native component rows.
		service.Setup(x => x.FindOrigin(301)).Returns(() =>
		{
			var saved = db.MagicSpellLifecycles.AsNoTracking().Include(x => x.Entities)
				.Single(x => x.Entities.Any(e => e.Kind == (int)SpellOwnedEntityKind.GameItem && e.EntityId == 301));
			return new SpellOwnedItemOrigin(saved.Id, (SpellLifecycleMode)saved.Mode, saved.DeadlineUtc, saved.CreatorId, saved.SpellId, saved.Grade);
		});
		world.SetupGet(x => x.SpellOwnedItems).Returns(service.Object);
		var xmlBefore = itemRow.GameItemComponents.Single().Definition;
		for (var attempt = 0; attempt < 2; attempt++)
		{
			var saved = db.GameItems.AsNoTracking().Include(x => x.GameItemComponents).Single(x => x.Id == 301);
			var loaded = new GameItem(saved, world.Object);
			Assert.AreEqual(persistedOrigin, loaded.SpellCreationOrigin);
			Assert.AreEqual(1, loaded.Components.Count());
			var light = (ProgLightGameItemComponent)loaded.Components.Single();
			Assert.AreEqual(401L, light.Id);
			Assert.IsTrue(light.Lit);
			Assert.AreEqual(140.0, light.CurrentIllumination);
			Assert.IsFalse(light.Changed);
			Assert.AreEqual(xmlBefore, ComponentXml(light));
		}
		Assert.AreEqual(1, db.GameItems.Count()); Assert.AreEqual(1, db.GameItemComponents.Count());
		Assert.AreEqual(1, db.MagicSpellLifecycles.Count()); Assert.AreEqual(1, db.MagicSpellOwnedEntities.Count());
		Assert.AreEqual(0, db.SaveChanges());
		service.Verify(x => x.Create(It.IsAny<IGameItemProto>(), It.IsAny<ICharacter>(), It.IsAny<ItemQuality>(), It.IsAny<SpellLifecycleOrigin>()), Times.Never);
	}

	private static Mock<IFuturemud> World()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		return world;
	}
	private static SpellOwnedItemOrigin Origin(int grade) => new(Guid.Parse("94248ab5-6500-4f08-91e4-07d6d3515741"),
		SpellLifecycleMode.TemporaryCleanup, new DateTime(2099, 1, 1, 0, 0, 0, DateTimeKind.Utc), 51, 71, grade);
	private static Mock<IGameItem> Parent(IFuturemud world, SpellOwnedItemOrigin? origin)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(world); parent.SetupGet(x => x.SpellCreationOrigin).Returns(origin);
		return parent;
	}
	private static LightPrototype Prototype(IFuturemud world, bool scale, bool omitFlag = false) => new(world,
		new XElement("Definition", new XElement("IlluminationProvided", 20),
			omitFlag ? null : new XElement("MultiplyIlluminationBySpellGrade", scale)).ToString());
	private static string ComponentXml(ProgLightGameItemComponent light) => (string)typeof(ProgLightGameItemComponent)
		.GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(light, null)!;
	private sealed class LightPrototype(IFuturemud world, string xml) : ProgLightGameItemComponentProto(
		new MudSharp.Models.GameItemComponentProto
		{
			Id = 201, Name = "native light", Definition = xml, Type = "Prog Light",
			EditableItem = new MudSharp.Models.EditableItem { RevisionNumber = 0, RevisionStatus = 4 }
		}, world)
	{
		public string ExportXml() => SaveToXml();
	}
}

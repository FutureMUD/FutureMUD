#nullable enable

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellOwnedShelterTests
{
	[DataTestMethod]
	[DataRow(SpellShelterKind.SpringHaven, "arm.spell.spring_haven", "Spring Haven")]
	[DataRow(SpellShelterKind.BurrowRefuge, "arm.spell.burrow_refuge", "Burrow Refuge")]
	[DataRow(SpellShelterKind.SandShelter, "arm.spell.sand_shelter", "Sand Shelter")]
	public void DistinctStockDefinitions_RetainExplicitNativeBindings(SpellShelterKind kind, string key, string name)
	{
		var configuration = new SpellShelterConfiguration(kind, long.MaxValue - 2, [10001, 10002], long.MaxValue - 1,
			120, 8, kind == SpellShelterKind.SpringHaven ? 456 : 0, kind == SpellShelterKind.SpringHaven ? 789 : 0,
			kind == SpellShelterKind.SpringHaven ? 10 : 0);
		Assert.AreEqual(key, ArmageddonShelterStock.Key(kind)); Assert.AreEqual(name, ArmageddonShelterStock.Name(kind));
		var root = ArmageddonShelterStock.Definition(configuration, 5, 6);
		var effect = root.Descendants("Effect").Single(x => (string?)x.Attribute("type") == "createshelter");
		var native = new CreateShelterEffect(effect, Mock.Of<IMagicSpell>());
		var recovered = native.Configuration();
		Assert.AreEqual(configuration.Kind, recovered.Kind); Assert.AreEqual(configuration.TemplateRoomId, recovered.TemplateRoomId);
		Assert.AreEqual(configuration.FallbackRoomId, recovered.FallbackRoomId); Assert.AreEqual(configuration.SecondsPerGrade, recovered.SecondsPerGrade);
		CollectionAssert.AreEqual(configuration.AllowedTerrainIds.ToArray(), recovered.AllowedTerrainIds.ToArray());
		Assert.AreEqual(configuration.WaterPrototypeId, recovered.WaterPrototypeId); Assert.AreEqual(configuration.LitresPerGrade, recovered.LitresPerGrade);
		Assert.IsTrue(XNode.DeepEquals(effect, native.Clone().SaveToXml()));
	}

	[TestMethod]
	public void DefinitionErrors_RefuseUnrecognisedVersionOrGuessedReferenceFields()
	{
		var root = CreateShelterEffect.Definition(new(SpellShelterKind.SandShelter, 10, [11], 12, 60, 2));
		root.SetAttributeValue("version", 2);
		Assert.IsNotNull(new CreateShelterEffect(root, Mock.Of<IMagicSpell>()).DefinitionError);
		root.SetAttributeValue("version", 1); root.Add(new XElement("UnknownActorId", 12345));
		Assert.IsNotNull(new CreateShelterEffect(root, Mock.Of<IMagicSpell>()).DefinitionError);
	}

	[DataTestMethod]
	[DataRow(0.0, 8)]
	[DataRow(-1.0, 8)]
	[DataRow(double.NaN, 8)]
	[DataRow(double.PositiveInfinity, 8)]
	[DataRow(120.0, 0)]
	[DataRow(120.0, 129)]
	public void InvalidDurationOrCapacity_RefusesBeforeAnyPersistence(double seconds, int capacity)
	{
		var service = new SpellOwnedShelterService(Mock.Of<IFuturemud>());
		Assert.IsNotNull(service.AdmissionError(Mock.Of<ICharacter>(), Mock.Of<IRoom>(),
			new(SpellShelterKind.SandShelter, 1, [2], 3, seconds, capacity), 1));
	}

	[TestMethod]
	public void Occupancy_UnsavedDepartureUsesExactLoadedFrame_OfflineAndSecondaryInstancesRemainCounted()
	{
		var room = new Mock<IRoom>(); room.SetupGet(x => x.Characters).Returns([]);
		var outside = Mock.Of<IRoom>();
		var departed = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		var departedInstance = departed.As<ICharacterInstance>();
		departed.SetupGet(x => x.InstanceId).Returns(101); departed.SetupGet(x => x.Location).Returns(outside);
		var secondary = new Mock<ICharacter>(); secondary.SetupGet(x => x.InstanceId).Returns(102);
		var secondaryInstance = secondary.As<ICharacterInstance>();
		secondary.SetupGet(x => x.Location).Returns(room.Object);
		departed.SetupGet(x => x.Identity.Instances).Returns([departedInstance.Object, secondaryInstance.Object]);
		var (instances, legacy) = SpellOwnedShelterService.ResidentOccupancy(room.Object,
			[(101, 1), (102, 1), (201, 2)], [1, 3], id => id == 1 ? departed.Object : null);
		CollectionAssert.AreEquivalent(new long[] { 102, 201 }, instances.ToArray());
		Assert.AreEqual(1, legacy);
	}

	[TestMethod]
	public void Occupancy_LoadedLegacyResidentIsCountedOnceAcrossPersistedAndRuntimeMembership()
	{
		var room = new Mock<IRoom>(); var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.InstanceId).Returns(101); actor.SetupGet(x => x.Location).Returns(room.Object);
		room.SetupGet(x => x.Characters).Returns([actor.Object]);
		var (instances, legacy) = SpellOwnedShelterService.ResidentOccupancy(room.Object, [], [1], _ => actor.Object);
		Assert.AreEqual(1, instances.Count + legacy);
	}

	[TestMethod]
	public void ResidentAdmission_OnlyNativeLoginOrExistingMembershipCanRestorePersistedClosedResidence()
	{
		var room = new Mock<IRoom>(); room.SetupGet(x => x.Characters).Returns([]);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.InstanceId).Returns(101);
		actor.SetupGet(x => x.Location).Returns(room.Object);
		var instances = new System.Collections.Generic.HashSet<long> { 101 };
		Assert.IsFalse(SpellOwnedShelterService.IsResidentEntry(room.Object, actor.Object, instances, false));
		Assert.IsTrue(SpellOwnedShelterService.IsResidentEntry(room.Object, actor.Object, instances, true));
		instances.Clear();
		Assert.IsFalse(SpellOwnedShelterService.IsResidentEntry(room.Object, actor.Object, instances, true));
	}

	[TestMethod]
	public void Capacity_DistinctUnsavedNpcsNeverShareReconnectAuthorityOrOccupancy()
	{
		var room = new Mock<IRoom>(); var first = new Mock<ICharacter>(); var second = new Mock<ICharacter>();
		first.SetupGet(x => x.Location).Returns(room.Object); second.SetupGet(x => x.Location).Returns(room.Object);
		room.SetupGet(x => x.Characters).Returns([first.Object]);
		var (instances, anonymous) = SpellOwnedShelterService.ResidentOccupancy(room.Object, [], [], _ => null);
		Assert.AreEqual(1, instances.Count + anonymous);
		Assert.IsFalse(SpellOwnedShelterService.IsResidentEntry(room.Object, second.Object, instances, true));
		Assert.IsTrue(SpellOwnedShelterService.IsResidentEntry(room.Object, first.Object, instances, true));
		room.SetupGet(x => x.Characters).Returns([first.Object, second.Object, first.Object]);
		(instances, anonymous) = SpellOwnedShelterService.ResidentOccupancy(room.Object, [], [], _ => null);
		Assert.AreEqual(2, instances.Count + anonymous);
	}

	[TestMethod]
	public void Occupancy_StaleRoomMembershipCannotCountAnExactDepartedFrame()
	{
		var room = new Mock<IRoom>(); var departed = new Mock<ICharacter>();
		departed.SetupGet(x => x.InstanceId).Returns(101); departed.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		room.SetupGet(x => x.Characters).Returns([departed.Object]);
		var (instances, anonymous) = SpellOwnedShelterService.ResidentOccupancy(room.Object, [], [], _ => null);
		Assert.AreEqual(0, instances.Count + anonymous);
	}

	[TestMethod]
	public void SupplyCustody_InheritedLocationCannotAuthoriseForeignContainmentOrInventory()
	{
		var room = new Mock<IRoom>(); var supply = new Mock<IGameItem>();
		supply.SetupGet(x => x.Location).Returns(room.Object); room.SetupGet(x => x.GameItems).Returns([supply.Object]);
		Assert.IsTrue(SpellOwnedShelterService.HasDirectSupplyCustody(supply.Object, room.Object));
		supply.SetupGet(x => x.ContainedIn).Returns(Mock.Of<IGameItem>());
		Assert.IsFalse(SpellOwnedShelterService.HasDirectSupplyCustody(supply.Object, room.Object));
		supply.SetupGet(x => x.ContainedIn).Returns((IGameItem?)null);
		supply.SetupGet(x => x.InInventoryOf).Returns(Mock.Of<MudSharp.Body.IBody>());
		Assert.IsFalse(SpellOwnedShelterService.HasDirectSupplyCustody(supply.Object, room.Object));
	}

	[TestMethod]
	public void RoomPermitRetirement_RemovesOnlyExactNativePermitCell_UnknownNumbersAndOtherPermitsSurvive()
	{
		const string data = "<Effects><Effect><Type>PermitWork</Type><Remaining>500</Remaining><Effect><Property>0</Property><Cell>123</Cell></Effect></Effect><Effect><Type>PermitWork</Type><Effect><Cell>124</Cell></Effect></Effect><Effect><Type>Unknown</Type><Effect><Cell>123</Cell><Text>123</Text></Effect></Effect></Effects>";
		var cleaned = SpellOwnedShelterService.RemoveRoomPermitData(data, 123)!;
		var root = XElement.Parse(cleaned);
		Assert.AreEqual(2, root.Elements("Effect").Count());
		Assert.AreEqual("124", root.Elements("Effect").Single(x => x.Element("Type")!.Value == "PermitWork").Element("Effect")!.Element("Cell")!.Value);
		Assert.AreEqual("123", root.Elements("Effect").Single(x => x.Element("Type")!.Value == "Unknown").Element("Effect")!.Element("Cell")!.Value);
		Assert.AreEqual(cleaned, SpellOwnedShelterService.RemoveRoomPermitData(cleaned, 123));
		Assert.AreEqual(data, SpellOwnedShelterService.RemoveRoomPermitData(data, 999));
		Assert.ThrowsException<FormatException>(() => SpellOwnedShelterService.RemoveRoomPermitData("<Effects><Effect><Type>PermitWork</Type><Effect><Cell>bad</Cell></Effect></Effect></Effects>", 123));
	}

	[TestMethod]
	public void ShelterMovement_RefusalRetainsVisibleExitAndReturnsUsefulErrorBeforePhysicalChecks()
	{
		var actor = (MudSharp.Character.Character)RuntimeHelpers.GetUninitializedObject(typeof(MudSharp.Character.Character));
		var room = Mock.Of<IRoom>(x => x.Id == 123);
		var service = new Mock<ISpellOwnedShelterService>(); service.Setup(x => x.OwnsRoom(123)).Returns(true);
		var world = Mock.Of<IFuturemud>(x => x.SpellOwnedShelters == service.Object);
		typeof(MudSharp.Character.Character).GetProperty(nameof(ICharacter.Gameworld))!.SetValue(actor, world);
		var exit = Mock.Of<MudSharp.Construction.Boundary.IRoomExit>(x => x.Destination == room);
		var result = actor.CanMove(exit, MudSharp.Movement.CanMoveFlags.None);
		Assert.IsFalse(result.Result); Assert.AreEqual("That shelter is full or closing.", result.ErrorMessage);
	}

	[DataTestMethod]
	[DataRow("BodyBackup", "<DestinationCellId>123</DestinationCellId>")]
	[DataRow("SpellBodyBackup", "<DestinationCellId>123</DestinationCellId>")]
	[DataRow("AnimalHunt", "<Origin>123</Origin><LastCell>124</LastCell>")]
	[DataRow("MonsterIntent", "<Origin>124</Origin><LastCell>123</LastCell>")]
	[DataRow("AdminSpyMaster", "<Spy>123</Spy>")]
	[DataRow("NpcKnownThreatLocations", "<Cell id='123' utc='2026-10-10T00:00:00Z'/>")]
	[DataRow("NpcKnownWaterLocations", "<Cell>123</Cell>")]
	public void ConsequentialRoomReferences_HoldOnlyExactKnownEffectFields(string type, string fields)
	{
		var data = $"<Effects><Effect><Type>{type}</Type><Effect>{fields}</Effect></Effect></Effects>";
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireNoForeignRoomEffectData(data, 123));
		SpellOwnedShelterService.RequireNoForeignRoomEffectData(data, 999);
		SpellOwnedShelterService.RequireNoForeignRoomEffectData(data.Replace(type, "Unknown"), 123);
	}

	[TestMethod]
	public void NativeParentRoomReferences_InspectOnlyProvenChildEnvelopePaths()
	{
		const string children = "<Effect><Type>SpellBodyBackup</Type><Effect><DestinationCellId>123</DestinationCellId></Effect></Effect>";
		foreach (var type in new[] { "MagicSpellParent", "SubstanceExposure" })
		{
			var data = $"<Effects><Effect><Type>{type}</Type><Effect><Children>{children}</Children></Effect></Effect></Effects>";
			Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireNoForeignRoomEffectData(data, 123));
			SpellOwnedShelterService.RequireNoForeignRoomEffectData(data, 124);
			SpellOwnedShelterService.RequireNoForeignRoomEffectData(data.Replace(type, "Unknown"), 123);
		}
	}

	[TestMethod]
	public void BasinSurfaceMaterial_ConservesForeignLiquidAndResidueWithoutMutatingSource()
	{
		var world = Mock.Of<IFuturemud>(); var liquid = Mock.Of<MudSharp.Form.Material.ILiquid>();
		var solid = Mock.Of<MudSharp.Form.Material.ISolid>();
		var source = new MudSharp.Form.Material.SurfaceLiquidState(world);
		source.AddLiquid(new MudSharp.Form.Material.LiquidMixture(liquid, 4.0, world)); source.AddResidue(solid, liquid, 2.0);
		var destination = new MudSharp.Form.Material.SurfaceLiquidState(world);
		Room.ConserveShelterSurfaceMaterial(destination, source);
		Assert.AreEqual(4.0, destination.LiquidVolume); Assert.AreEqual(2.0, destination.ResidueWeight);
		Assert.AreSame(solid, destination.Residues.Single().Material);
		Assert.AreEqual(4.0, source.LiquidVolume); Assert.AreEqual(2.0, source.ResidueWeight);
	}

	[TestMethod]
	public void SupplyPersistence_ConflictingSecondRoomCustodyHoldsWithoutDeletingEitherLink()
	{
		var options = new DbContextOptionsBuilder<MudSharp.Database.FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var context = new MudSharp.Database.FuturemudDatabaseContext(options);
		context.GameItems.Add(new MudSharp.Models.GameItem { Id = 70001 });
		context.RoomsGameItems.Add(new MudSharp.Models.RoomsGameItems { GameItemId = 70001, RoomId = 123 });
		context.SaveChanges();
		SpellOwnedShelterService.RequireSupplyPersistence(context, 70001, 123);
		context.RoomsGameItems.Add(new MudSharp.Models.RoomsGameItems { GameItemId = 70001, RoomId = 124 }); context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireSupplyPersistence(context, 70001, 123));
		Assert.AreEqual(2, context.RoomsGameItems.Count()); Assert.IsNotNull(context.GameItems.Find(70001L));
		context.RoomsGameItems.Remove(context.RoomsGameItems.Single(x => x.RoomId == 124)); context.SaveChanges();
		var inventory = new MudSharp.Models.BodiesGameItems { GameItemId = 70001, BodyId = 90001 };
		context.BodiesGameItems.Add(inventory); context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireSupplyPersistence(context, 70001, 123));
		context.BodiesGameItems.Remove(inventory); context.SaveChanges();
		context.GameItems.Add(new MudSharp.Models.GameItem { Id = 70002, ContainerId = 70001 }); context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.RequireSupplyPersistence(context, 70001, 123));
		Assert.IsNotNull(context.GameItems.Find(70002L));
	}

	[TestMethod]
	public void UnregisteredOccupant_KnownUnsavedRoomReferenceHoldsBeforeEvacuation()
	{
		var room = new Mock<IRoom>(); room.SetupGet(x => x.Id).Returns(123);
		var occupant = new Mock<ICharacter>();
		occupant.Setup(x => x.SaveEffects()).Returns(XElement.Parse("<Effects><Effect><Type>NpcHomeBase</Type><Effect HomeCellId='123'/></Effect></Effects>"));
		room.SetupGet(x => x.Characters).Returns([occupant.Object]);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		using var context = new MudSharp.Database.FuturemudDatabaseContext(new DbContextOptionsBuilder<MudSharp.Database.FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		Assert.ThrowsException<InvalidOperationException>(() => new SpellOwnedShelterService(world.Object).RequireNoForeignRoomEffectReferences(context, room.Object));
		occupant.Verify(x => x.SaveEffects(), Times.Once);
	}

	[TestMethod]
	public void DeclaredReferenceQuery_UsesOnlyTheSuppliedModelPropertyIncludingNullableLinks()
	{
		var options = new DbContextOptionsBuilder<LinkContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
		using var context = new LinkContext(options);
		context.Links.AddRange(new Link { Id = 1, RoomForeignKey = 10, NullableRoomForeignKey = null, UnrelatedNumber = 999 },
			new Link { Id = 2, RoomForeignKey = 11, NullableRoomForeignKey = 999, UnrelatedNumber = 10 });
		context.SaveChanges();
		Assert.IsFalse(SpellOwnedShelterService.HasDeclaredReference(context, typeof(Link), nameof(Link.RoomForeignKey), typeof(long), 999));
		Assert.IsTrue(SpellOwnedShelterService.HasDeclaredReference(context, typeof(Link), nameof(Link.RoomForeignKey), typeof(long), 10));
		Assert.IsTrue(SpellOwnedShelterService.HasDeclaredReference(context, typeof(Link), nameof(Link.NullableRoomForeignKey), typeof(long?), 999));
		Assert.IsFalse(SpellOwnedShelterService.HasDeclaredReference(context, typeof(Link), nameof(Link.NullableRoomForeignKey), typeof(long?), 10));
	}

	public sealed class Link
	{
		public long Id { get; set; }
		public long RoomForeignKey { get; set; }
		public long? NullableRoomForeignKey { get; set; }
		public long UnrelatedNumber { get; set; }
	}
	private sealed class LinkContext(DbContextOptions<LinkContext> options) : DbContext(options)
	{ public DbSet<Link> Links => Set<Link>(); }

	[TestMethod]
	public void Evacuation_LeaveCallbackMovesVisitor_RespectsNewLocationAndStopsBeforeEntry()
	{
		var source = new Mock<IRoom>(); var target = new Mock<IRoom>(); var other = new Mock<IRoom>();
		IRoom location = source.Object;
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Location).Returns(() => location);
		source.SetupGet(x => x.Characters).Returns([actor.Object]);
		source.Setup(x => x.Leave(actor.Object)).Callback(() => location = other.Object);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var service = new SpellOwnedShelterService(world.Object);
		Assert.ThrowsException<InvalidOperationException>(() => service.Evacuate(actor.Object, source.Object,
			new SpatialLocation(target.Object, RoomLayer.GroundLevel)));
		Assert.AreSame(other.Object, actor.Object.Location);
		actor.Verify(x => x.MoveTo(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(), It.IsAny<MudSharp.Construction.Boundary.IRoomExit>(), It.IsAny<bool>()), Times.Never);
		target.Verify(x => x.Enter(It.IsAny<ICharacter>(), It.IsAny<MudSharp.Construction.Boundary.IRoomExit>(), It.IsAny<bool>(), It.IsAny<RoomLayer>()), Times.Never);
	}

	[TestMethod]
	public void ForcedRoomEntry_AdmissionRefusal_DoesNotInvokeNormalEntryOrOverwriteLocation()
	{
		var room = (Room)RuntimeHelpers.GetUninitializedObject(typeof(Room));
		var shelter = new Mock<ISpellOwnedShelterService>();
		var world = Mock.Of<IFuturemud>(x => x.SpellOwnedShelters == shelter.Object);
		typeof(Room).GetProperty(nameof(Room.Gameworld))!.SetValue(room, world);
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		var source = Mock.Of<IRoom>(); actor.SetupGet(x => x.Location).Returns(source);
		shelter.Setup(x => x.CanEnter(room, actor.Object)).Returns(false);
		shelter.Setup(x => x.OwnsRoom(room.Id)).Returns(true);
		room.Enter(actor.Object);
		Assert.AreSame(source, actor.Object.Location);
		shelter.Verify(x => x.CanEnter(room, actor.Object), Times.Once);
		shelter.Verify(x => x.ReturnRejectedEntrant(It.IsAny<IRoom>(), It.IsAny<ICharacter>()), Times.Never);
	}

	[TestMethod]
	public void ConservedWater_ActivationInterruptedAfterItemRegistration_ResumesEveryComponentOnce()
	{
		var item = (GameItem)RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
		var component = (HoldableGameItemComponent)RuntimeHelpers.GetUninitializedObject(typeof(HoldableGameItemComponent));
		var row = new MudSharp.Models.GameItem { Id = 70001 };
		var model = new MudSharp.Models.GameItemComponent { Id = 70002 };
		var itemNotifications = 0; var componentNotifications = 0;
		item.IdRegistered += _ => { itemNotifications++; throw new InvalidOperationException("Injected publication interruption"); };
		component.IdRegistered += _ => componentNotifications++;
		Assert.ThrowsException<InvalidOperationException>(() => item.ActivateCommittedOrdinaryItem(row, [(component, model)]));
		Assert.IsTrue(item.IdHasBeenRegistered); Assert.IsFalse(component.IdHasBeenRegistered);
		item.ActivateCommittedOrdinaryItem(row, [(component, model)]);
		item.ActivateCommittedOrdinaryItem(row, [(component, model)]);
		Assert.AreEqual(70001L, item.Id); Assert.AreEqual(70002L, component.Id);
		Assert.AreEqual(1, itemNotifications); Assert.AreEqual(1, componentNotifications);
	}

	[TestMethod]
	public void CachedVisitor_EvacuationMovesCanonicalLocationWithoutMaterialisingAnOfflineActor()
	{
		var source = new Mock<IRoom>(); var target = new Mock<IRoom>(); IRoom location = source.Object;
		source.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Location).Returns(() => location);
		actor.Setup(x => x.MoveTo(It.IsAny<SpatialLocation>(), It.IsAny<MudSharp.Construction.Boundary.IRoomExit>(), It.IsAny<bool>()))
			.Callback<SpatialLocation, MudSharp.Construction.Boundary.IRoomExit, bool>((point, _, _) => location = point.Room);
		var world = Mock.Of<IFuturemud>(x => x.Actors == new All<ICharacter>());
		new SpellOwnedShelterService(world).Evacuate(actor.Object, source.Object, new SpatialLocation(target.Object, RoomLayer.GroundLevel));
		Assert.AreSame(target.Object, actor.Object.Location);
		source.Verify(x => x.Leave(It.IsAny<ICharacter>()), Times.Never);
		target.Verify(x => x.Enter(It.IsAny<ICharacter>(), It.IsAny<MudSharp.Construction.Boundary.IRoomExit>(), It.IsAny<bool>(), It.IsAny<RoomLayer>()), Times.Never);
		actor.VerifySet(x => x.Changed = true, Times.Once);
	}

	[TestMethod]
	public void ForeignItem_ExtractionRelocatesToAnotherRoom_NeverOverwritesItsNewCustody()
	{
		var source = new Mock<IRoom>(); var target = new Mock<IRoom>(); var other = Mock.Of<IRoom>();
		source.As<ICustodyRollbackLocation>().Setup(x => x.CaptureCustodyMembershipRollback(It.IsAny<System.Collections.Generic.IReadOnlyCollection<IGameItem>>())).Returns(() => () => { });
		IRoom location = source.Object; var item = new Mock<IGameItem>(); item.SetupGet(x => x.Location).Returns(() => location);
		source.Setup(x => x.Extract(item.Object)).Callback(() => location = other);
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedShelterService.EvacuateForeignItem(item.Object, source.Object, new SpatialLocation(target.Object, RoomLayer.GroundLevel)));
		Assert.AreSame(other, item.Object.Location);
		item.Verify(x => x.MoveTo(It.IsAny<SpatialLocation>(), It.IsAny<MudSharp.Construction.Boundary.IRoomExit>(), It.IsAny<bool>()), Times.Never);
		target.Verify(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()), Times.Never);
	}
}

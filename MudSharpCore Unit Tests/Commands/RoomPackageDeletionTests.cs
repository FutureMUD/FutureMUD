#nullable enable

using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
[DoNotParallelize]
public class RoomPackageDeletionTests
{
	[TestMethod]
	public void Delete_PackageStillActiveForAnyRoom_LeavesWorldAndSelectionUntouched()
	{
		var f = new Fixture(true);
		f.Delete();
		Assert.AreSame(f.Package.Object, f.Actor.Object.CurrentOverlayPackage);
		f.Room.Verify(x => x.RemoveOverlay(It.IsAny<long>()), Times.Never);
		f.Save.Verify(x => x.Flush(), Times.Never);
		f.World.Verify(x => x.Destroy(It.IsAny<IRoomOverlayPackage>()), Times.Never);
	}

	[TestMethod]
	public void Delete_InactiveDraft_RemovesOnlyItsRevisionAndKeepsTheActiveRoomOverlay()
	{
		using var database = new InMemoryDatabaseTestScope();
		var f = new Fixture(false);
		var current = new MudSharp.Models.RoomOverlayPackage { Id = 1, RevisionNumber = 0, Name = "Current" };
		var draft = new MudSharp.Models.RoomOverlayPackage { Id = 1, RevisionNumber = 1, Name = "Draft" };
		database.Context.RoomOverlayPackages.AddRange(current, draft);
		database.Context.RoomOverlays.AddRange(Overlay(100, current), Overlay(101, draft));
		database.Context.SaveChanges();
		f.Delete();
		database.Context.ChangeTracker.Clear();
		Assert.AreEqual(0, database.Context.RoomOverlayPackages.AsNoTracking().Single().RevisionNumber);
		Assert.AreEqual(100L, database.Context.RoomOverlays.AsNoTracking().Single().Id);
		Assert.AreEqual(100L, f.Room.Object.CurrentOverlay.Id);
		f.Room.Verify(x => x.RemoveOverlay(101), Times.Once);
		f.Room.Verify(x => x.RemoveOverlay(100), Times.Never);
		Assert.IsNull(f.Actor.Object.CurrentOverlayPackage);
		f.World.Verify(x => x.Destroy(f.Package.Object), Times.Once);
	}

	private static MudSharp.Models.RoomOverlay Overlay(long id, MudSharp.Models.RoomOverlayPackage package) => new()
	{
		Id = id, RoomId = 10, RoomOverlayPackage = package, RoomOverlayPackageId = package.Id,
		RoomOverlayPackageRevisionNumber = package.RevisionNumber,
		Name = "Overlay", RoomName = "Test Room", RoomDescription = "A room.", AtmosphereType = "gas"
	};

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IRoom> Room { get; } = new();
		public Mock<IRoomOverlayPackage> Package { get; } = new();
		public Mock<ISaveManager> Save { get; } = new();

		public Fixture(bool active)
		{
			Package.SetupGet(x => x.Id).Returns(1);
			Package.SetupGet(x => x.RevisionNumber).Returns(1);
			Package.SetupGet(x => x.Status).Returns(RevisionStatus.UnderDesign);
			var current = active ? Package.Object : Mock.Of<IRoomOverlayPackage>(x => x.Id == 1 && x.RevisionNumber == 0);
			Room.SetupGet(x => x.Id).Returns(10);
			Room.SetupGet(x => x.CurrentOverlay).Returns(Mock.Of<IRoomOverlay>(x => x.Id == 100 && x.Package == current));
			var rooms = new All<IRoom>();
			rooms.Add(Room.Object);
			World.SetupGet(x => x.Rooms).Returns(rooms);
			World.SetupGet(x => x.SaveManager).Returns(Save.Object);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Actor.SetupProperty(x => x.CurrentOverlayPackage, Package.Object);
		}

		public void Delete() => typeof(RoomBuilderModule)
			.GetMethod("RoomPackageDelete", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [Actor.Object, new StringStack("")]);
	}
}

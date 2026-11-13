#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Movement;
using MudSharp.TimeAndDate;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TrackExitPersistenceTests
{
	[TestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void InsertAndSave_TransientExitReferences_AreOmitted(bool transientFrom, bool transientTo)
	{
		using var scope = new InMemoryDatabaseTestScope();
		var room = Mock.Of<IRoom>(x => x.Id == 7);
		var rooms = new Mock<IUneditableAll<IRoom>>();
		rooms.Setup(x => x.Get(7)).Returns(room);
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.Rooms).Returns(rooms.Object);
		IExit Exit(long id, bool transient)
		{
			// Positive IDs ensure the runtime-only marker, rather than ID sign, governs persistence.
			return transient ? Mock.Of<ITransientExit>(x => x.Id == id) : Mock.Of<IExit>(x => x.Id == id);
		}

		var from = Exit(101, transientFrom);
		var to = Exit(102, transientTo);
		world.Setup(x => x.ExitManager.GetExitByID(101)).Returns(from);
		world.Setup(x => x.ExitManager.GetExitByID(102)).Returns(to);
		var row = new DB.Track
		{
			Id = 701, RoomId = 7, CharacterId = 1, BodyPrototypeId = 2,
			FromDirectionExitId = 101, ToDirectionExitId = 102,
			MudDateTime = MudDateTime.Never.GetDateTimeString()
		};
		scope.Context.Tracks.Add(row);
		scope.Context.SaveChanges();
		var track = new Track(row, world.Object);

		var inserted = (DB.Track)track.DatabaseInsert();
		Assert.AreEqual(transientFrom ? null : (long?)101, inserted.FromDirectionExitId);
		Assert.AreEqual(transientTo ? null : (long?)102, inserted.ToDirectionExitId);
		track.Save();
		Assert.AreEqual(transientFrom ? null : (long?)101, row.FromDirectionExitId);
		Assert.AreEqual(transientTo ? null : (long?)102, row.ToDirectionExitId);
		Assert.AreSame(from, track.FromExit, "Saving must retain runtime movement descriptions.");
		Assert.AreSame(to, track.ToExit, "Saving must retain runtime movement descriptions.");
	}
}

using Moq;
using MudSharp.Body;
using MudSharp.Celestial;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

public class RoomStub
{
    public string Name { get; set; }
    public List<RoomExitStub> Exits { get; set; }
    public List<IPerceivable> Perceivables { get; set; } = new();
    public (int X, int Y, int Z) Coordinates { get; init; }
    public IFuturemud Gameworld { get; set; }
    public long Id { get; set; }

    public Mock<IRoom> ToMock()
    {
        Mock<IRoom> mock = new();
        mock.Setup(t => t.Name).Returns(Name);
        mock.Setup(t => t.StoredCoordinates).Returns(Coordinates);
        mock.Setup(t => t.X).Returns(Coordinates.X);
        mock.Setup(t => t.Y).Returns(Coordinates.Y);
        mock.Setup(t => t.Z).Returns(Coordinates.Z);
        mock.Setup(t => t.Id).Returns(Id);
        mock.Setup(t => t.Gameworld).Returns(() => Gameworld);
        mock.Name = Name;
        return mock;
    }

    public IRoom GetObject(IEnumerable<Mock<IRoom>> cellMocks)
    {
        Mock<IRoom> mock = cellMocks.First(x => x.Object.Name.Equals(Name));
        mock.Setup(t => t.ExitsFor(It.IsAny<IPerceiver>(), false)).Returns(Exits.Select(x => x.ToMock(cellMocks, mock.Object)));
        mock.Setup(t => t.ExitsFor(It.IsAny<IPerceiver>(), true)).Returns(Exits.Select(x => x.ToMock(cellMocks, mock.Object)));
        mock.Setup(t => t.Perceivables).Returns(Perceivables);
        mock.Setup(t => t.Characters).Returns(Perceivables.OfType<ICharacter>());
        return mock.Object;
    }
}

public class RoomExitStub
{
    public override string ToString()
    {
        return $"Cell Exit {OutboundDirection.Describe()} to {Destination.Name}";
    }
    public RoomStub Destination { get; init; }

    public ExitStub Exit { get; init; }

    public CardinalDirection OutboundDirection { get; init; }

    public IRoomExit ToMock(IEnumerable<Mock<IRoom>> cellMocks, IRoom origin)
    {
        Mock<IRoomExit> mock = new();
        mock.Setup(t => t.OutboundDirection).Returns(OutboundDirection);
        mock.Setup(t => t.Origin).Returns(origin);
        IRoom destination = cellMocks.First(x => x.Name == Destination.Name).Object;
        mock.Setup(t => t.Destination).Returns(destination);
        mock.Setup(t => t.Exit).Returns(Exit.ToMock(origin, destination));
        return mock.Object;
    }
}

public class DoorStub
{
    public bool CanFireThrough
    {
        get; init;
    }

    public bool CanPlayersSmash
    {
        get; init;
    }

    public ExitStub Exit
    {
        get; set;
    }

    public RoomStub HingeRoom
    {
        get; set;
    }

    public bool IsOpen { get; set; }

    public bool Locked { get; set; }

    public DoorState State
    {
        get; init;
    }

    public IDoor ToMock()
    {
        Mock<IDoor> mock = new();
        mock.SetupGet(t => t.CanFireThrough).Returns(CanFireThrough);
        mock.SetupGet(t => t.CanPlayersSmash).Returns(CanPlayersSmash);
        mock.SetupGet(t => t.IsOpen).Returns(() => IsOpen);
        mock.SetupGet(t => t.State).Returns(State);
        Mock<ILock> lockMock = new();
        lockMock.SetupGet(x => x.IsLocked).Returns(() => Locked);
        mock.SetupGet(t => t.Locks).Returns(new[] { lockMock.Object });
        mock.SetupProperty(t => t.InstalledExit);
        mock.SetupProperty(t => t.HingeRoom);
        mock.Setup(t => t.CanSeeThrough(It.IsAny<IBody>())).Returns(true);
        return mock.Object;
    }
}

public class ExitStub
{
    public DoorStub Door
    {
        get; init;
    }

    public bool AcceptsDoor { get; init; }

    public IExit ToMock(IRoom origin, IRoom destination)
    {
        Mock<IExit> mock = new();
        mock.SetupProperty(t => t.Door, Door?.ToMock());
        mock.Setup(t => t.AcceptsDoor).Returns(AcceptsDoor);
        mock.Setup(t => t.Rooms).Returns(new[] { origin, destination });
        return mock.Object;
    }
}

public class PerceivableStub
{
    public IRoom Location { get; init; }
    public IFuturemud Gameworld { get; set; }
    public IPerceivable ToMock()
    {
        Mock<IPerceivable> mock = new();
        mock.Setup(t => t.Location).Returns(Location);
        mock.Setup(t => t.Gameworld).Returns(() => Gameworld);
        return mock.Object;
    }
}

public class GameworldStub
{
    public IFuturemud ToMock()
    {
        Mock<IFuturemud> mock = new();
        return mock.Object;
    }
}

public class FrameworkItemStub : IFrameworkItem
{
    public string Name { get; set; } = string.Empty;
    public long Id { get; set; }
    public string FrameworkItemType => "Stub";
}

public class RevisableItemStub : FrameworkItemStub, IRevisableItem
{
    public int RevisionNumber { get; set; }
    public RevisionStatus Status { get; set; }
    public string IdAndRevisionFor(IPerceiver voyeur)
    {
        return $"{Id}:{RevisionNumber}";
    }
}

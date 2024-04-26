using System.Collections.Generic;
using System.Linq;
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
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests {
    public class RoomStub
    {
        public int X { get; init; }
        public int Y { get; init; }
        public int Z { get; init; }

        public IRoom ToMock()
        {
            var mock = new Mock<IRoom>();
            mock.Setup(t => t.X).Returns(X);
            mock.Setup(t => t.Y).Returns(Y);
            mock.Setup(t => t.Z).Returns(Z);
            return mock.Object;
        }
    }

    public class CellStub {
        public string Name { get; set; }
        public List<CellExitStub> Exits { get; set; }
        public IRoom Room { get; init; }
        public long Id { get; set; }

        public Mock<ICell> ToMock() {
            var mock = new Mock<ICell>();
            mock.Setup(t => t.Name).Returns(Name);
            mock.Setup(t => t.Room).Returns(Room);
            mock.Setup(t => t.Id).Returns(Id);
            mock.Name = Name;
            return mock;
        }

        public ICell GetObject(IEnumerable<Mock<ICell>> cellMocks) {
            var mock = cellMocks.First(x => x.Object.Name.Equals(Name));
            mock.Setup(t => t.ExitsFor(It.IsAny<IPerceiver>(), false)).Returns(Exits.Select(x => x.ToMock(cellMocks, mock.Object)));
            mock.Setup(t => t.ExitsFor(It.IsAny<IPerceiver>(), true)).Returns(Exits.Select(x => x.ToMock(cellMocks, mock.Object)));
            return mock.Object;
        }
    }

    public class CellExitStub {
        public override string ToString() {
            return $"Cell Exit {OutboundDirection.Describe()} to {Destination.Name}";
        }
        public CellStub Destination { get; init; }
        
        public ExitStub Exit { get; init; }

        public CardinalDirection OutboundDirection { get; init; }

        public ICellExit ToMock(IEnumerable<Mock<ICell>> cellMocks, ICell origin) {
            var mock = new Mock<ICellExit>();
            mock.Setup(t => t.OutboundDirection).Returns(OutboundDirection);
            var destination = cellMocks.First(x => x.Name == Destination.Name).Object;
            mock.Setup(t => t.Destination).Returns(destination);
            mock.Setup(t => t.Exit).Returns(Exit.ToMock(origin, destination));
            return mock.Object;
        }
    }

    public class DoorStub {
        public bool CanFireThrough {
            get; init;
        }

        public bool CanPlayersSmash {
            get;init;
        }

        public ExitStub Exit {
            get;set;
        }

        public CellStub HingeCell {
            get;set;
        }

        public bool IsOpen { get; init; }
        
        public DoorState State {
            get;init;
        }

        public IDoor ToMock() {
            var mock = new Mock<IDoor>();
            mock.SetupGet(t => t.CanFireThrough).Returns(CanFireThrough);
            mock.SetupGet(t => t.CanPlayersSmash).Returns(CanPlayersSmash);
            mock.SetupGet(t => t.IsOpen).Returns(IsOpen);
            mock.SetupGet(t => t.State).Returns(State);
            mock.SetupProperty(t => t.InstalledExit);
            mock.SetupProperty(t => t.HingeCell);
            return mock.Object;
        }
    }

    public class ExitStub {
        public DoorStub Door {
            get; init;
        }

        public bool AcceptsDoor { get; init; }

        public IExit ToMock(ICell origin, ICell destination) {
            var mock = new Mock<IExit>();
            mock.SetupProperty(t => t.Door, Door?.ToMock());
            mock.Setup(t => t.AcceptsDoor).Returns(AcceptsDoor);
            mock.Setup(t => t.Cells).Returns(new[] { origin, destination });
            return mock.Object;
        }
    }

    public class PerceivableStub {
        public ICell Location { get; init; }
        public IPerceivable ToMock() {
            var mock = new Mock<IPerceivable>();
            mock.Setup(t => t.Location).Returns(Location);
            return mock.Object;
        }
    }

    public class GameworldStub
    {
	    public IFuturemud ToMock()
	    {
		    var mock = new Mock<IFuturemud>();
            return mock.Object;
	    }
    }
}

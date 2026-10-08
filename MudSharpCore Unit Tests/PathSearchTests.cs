using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PathSearchTests
{

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
        TestRooms = GetTestRooms;
        TestRoomsNoDiagonals = GetTestRoomsNoDiagonals;
        Person1Flight = new PerceivableStub { Location = TestRoomsNoDiagonals[5, 5] }.ToMock();
        Person2Flight = new PerceivableStub { Location = TestRoomsNoDiagonals[12, 3] }.ToMock();
        Person3Flight = new PerceivableStub { Location = TestRoomsNoDiagonals[5, 6] }.ToMock();
        Person4Flight = new PerceivableStub { Location = TestRoomsNoDiagonals[5, 7] }.ToMock();
        Person5Flight = new PerceivableStub { Location = TestRoomsNoDiagonals[45, 17] }.ToMock();
    }

    public static IRoom[,] TestRooms { get; set; }
    public static IRoom[,] TestRoomsNoDiagonals { get; set; }

    public static IRoom[,] GetTestRooms
    {
        get
        {
            RoomStub[,] cellMap = new RoomStub[50, 50];
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    List<RoomExitStub> exits = new();
                    RoomStub room = new()
                    {
                        Id = i * 50 + j + 1,
                        Coordinates = (i, j, 0
                        )
                    };
                    if (i > 0)
                    {
                        exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.West,
                            Destination = cellMap[i - 1, j]
                        });
                        cellMap[i - 1, j].Exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.East,
                            Destination = room
                        });

                        if (j > 0)
                        {
                            exits.Add(new RoomExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.SouthWest,
                                Destination = cellMap[i - 1, j - 1]
                            });
                            cellMap[i - 1, j - 1].Exits.Add(new RoomExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.NorthEast,
                                Destination = room
                            });
                        }

                        if (j < 49)
                        {
                            exits.Add(new RoomExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.NorthWest,
                                Destination = cellMap[i - 1, j + 1]
                            });
                            cellMap[i - 1, j + 1].Exits.Add(new RoomExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.SouthEast,
                                Destination = room
                            });
                        }
                    }

                    if (j > 0)
                    {
                        exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.South,
                            Destination = cellMap[i, j - 1]
                        });
                        cellMap[i, j - 1].Exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.North,
                            Destination = room
                        });
                    }

                    room.Exits = exits;
                    cellMap[i, j] = room;
                    room.Name = $"Cell {i},{j}";
                }
            }

            IRoom[,] returnMap = new IRoom[50, 50];
            List<Mock<IRoom>> cellMocks = cellMap.OfType<RoomStub>().Select(x => x.ToMock()).ToList();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    returnMap[i, j] = cellMap[i, j].GetObject(cellMocks);
                }
            }
            return returnMap;
        }
    }

    public static IRoom[,] GetTestRoomsNoDiagonals
    {
        get
        {
            RoomStub[,] cellMap = new RoomStub[50, 50];
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    List<RoomExitStub> exits = new();
                    RoomStub room = new()
                    {
                        Id = i * 50 + j + 1,
                        Coordinates = (i, j, 0
                        )
                    };
                    if (i > 0)
                    {
                        exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.West,
                            Destination = cellMap[i - 1, j]
                        });
                        cellMap[i - 1, j].Exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.East,
                            Destination = room
                        });
                    }

                    if (j > 0)
                    {
                        exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.South,
                            Destination = cellMap[i, j - 1]
                        });
                        cellMap[i, j - 1].Exits.Add(new RoomExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.North,
                            Destination = room
                        });
                    }

                    room.Exits = exits;
                    cellMap[i, j] = room;
                    room.Name = $"Cell {i},{j}";
                }
            }

            IRoom[,] returnMap = new IRoom[50, 50];
            List<Mock<IRoom>> cellMocks = cellMap.OfType<RoomStub>().Select(x => x.ToMock()).ToList();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    returnMap[i, j] = cellMap[i, j].GetObject(cellMocks);
                }
            }
            return returnMap;
        }
    }

    public static IPerceivable Person1Flight { get; set; }
    public static IPerceivable Person2Flight { get; set; }
    public static IPerceivable Person3Flight { get; set; }
    public static IPerceivable Person4Flight { get; set; }
    public static IPerceivable Person5Flight { get; set; }

    private static (IFuturemud Gameworld, PathfindingService Service, IRoom[] Rooms) BuildLinearPath(int count)
    {
        All<IRoom> allRooms = new();
        Mock<IFuturemud> gameworld = new();
        Mock<IExitManager> exitManager = new();
        PathfindingService service = new(gameworld.Object);
        exitManager.Setup(x => x.PathfindingService).Returns(service);
        gameworld.Setup(x => x.Rooms).Returns(allRooms);
        gameworld.Setup(x => x.ExitManager).Returns(exitManager.Object);

        RoomStub[] cellStubs = Enumerable.Range(0, count)
                                         .Select(i => new RoomStub
                                         {
                                             Id = i + 1,
                                             Name = $"Linear {i}",
                                             Gameworld = gameworld.Object,
                                             Coordinates = (i, 0, 0 ),
                                             Exits = new List<RoomExitStub>()
                                         })
                                         .ToArray();
        for (int i = 0; i < count - 1; i++)
        {
            cellStubs[i].Exits.Add(new RoomExitStub
            {
                Destination = cellStubs[i + 1],
                Exit = new ExitStub(),
                OutboundDirection = CardinalDirection.East
            });
            cellStubs[i + 1].Exits.Add(new RoomExitStub
            {
                Destination = cellStubs[i],
                Exit = new ExitStub(),
                OutboundDirection = CardinalDirection.West
            });
        }

        List<Mock<IRoom>> cellMocks = cellStubs.Select(x => x.ToMock()).ToList();
        IRoom[] rooms = cellStubs.Select(x => x.GetObject(cellMocks)).ToArray();
        foreach (IRoom room in rooms)
        {
            allRooms.Add(room);
        }

        return (gameworld.Object, service, rooms);
    }

    private static IPerceivable PerceivableAt(IRoom room, IFuturemud gameworld)
    {
        return new PerceivableStub { Location = room, Gameworld = gameworld }.ToMock();
    }

    [TestMethod]
    public void TestASharpPath()
    {
        List<IRoomExit> path = Person1Flight.PathBetween(Person2Flight, 15, true).ToList();
        Assert.AreEqual(9, path.Count, $"It was expected that the two persons were 9 squares apart but they were {path.Count} apart instead. \n\nPath was: {path.Select(x => x.OutboundDirection.DescribeBrief()).ListToString()}.\n\n{path.Select(x => x.Destination.Name).ListToString()}");

        path = Person1Flight.PathBetween(Person3Flight, 1, true).ToList();
        Assert.AreEqual(1, path.Count, $"It was expected that the two persons were 1 square apart but they were {path.Count} apart instead. \n\nPath was: {path.Select(x => x.OutboundDirection.DescribeBrief()).ListToString()}.\n\n{path.Select(x => x.Destination.Name).ListToString()}");
    }

    [TestMethod]
    public void AutomaticPathBelowThresholdUsesExactSearch()
    {
        List<IRoomExit> path = Person1Flight.PathBetween(Person2Flight, 15, true,
            new PathSearchOptions { Algorithm = PathSearchAlgorithm.Automatic, HierarchicalThreshold = 100 }).ToList();

        Assert.AreEqual(9, path.Count, "Expected automatic mode below threshold to preserve exact pathing.");
    }

    [TestMethod]
    public void HierarchicalPathFindsLongLinearRouteAfterIdleBuild()
    {
        (IFuturemud gameworld, PathfindingService service, IRoom[] rooms) = BuildLinearPath(100);
        service.RequestIndexWarmup();
        service.DoIdleWork(TimeSpan.FromSeconds(5));

        IPerceivable source = PerceivableAt(rooms[0], gameworld);
        IPerceivable target = PerceivableAt(rooms[99], gameworld);
        List<IRoomExit> path = source.PathBetween(target, 150, _ => true, PathSearchOptions.Hierarchical).ToList();

        Assert.AreEqual(99, path.Count, "Expected hierarchical pathing to produce the full long route.");
        Assert.IsTrue(service.Diagnostics.CurrentSnapshotVersion > 0, "Expected idle work to publish an index snapshot.");
    }

    [TestMethod]
    public void HierarchicalPathValidatesDoorStateWithoutRebuildingIndex()
    {
        All<IRoom> allRooms = new();
        Mock<IFuturemud> gameworld = new();
        Mock<IExitManager> exitManager = new();
        PathfindingService service = new(gameworld.Object);
        exitManager.Setup(x => x.PathfindingService).Returns(service);
        gameworld.Setup(x => x.Rooms).Returns(allRooms);
        gameworld.Setup(x => x.ExitManager).Returns(exitManager.Object);

        DoorStub door = new() { IsOpen = true, Locked = false, State = DoorState.Open };
        RoomStub cellA = new()
        {
            Id = 1001,
            Name = "A",
            Gameworld = gameworld.Object,
            Coordinates = (0, 0, 0 ),
            Exits = new List<RoomExitStub>()
        };
        RoomStub cellB = new()
        {
            Id = 1002,
            Name = "B",
            Gameworld = gameworld.Object,
            Coordinates = (11, 0, 0 ),
            Exits = new List<RoomExitStub>()
        };
        RoomStub cellC = new()
        {
            Id = 1003,
            Name = "C",
            Gameworld = gameworld.Object,
            Coordinates = (0, 11, 0 ),
            Exits = new List<RoomExitStub>()
        };

        cellA.Exits.Add(new RoomExitStub
        {
            Destination = cellB,
            Exit = new ExitStub { Door = door },
            OutboundDirection = CardinalDirection.East
        });
        cellA.Exits.Add(new RoomExitStub
        {
            Destination = cellC,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.South
        });
        cellC.Exits.Add(new RoomExitStub
        {
            Destination = cellB,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });

        RoomStub[] stubs = [cellA, cellB, cellC];
        List<Mock<IRoom>> mocks = stubs.Select(x => x.ToMock()).ToList();
        IRoom[] rooms = stubs.Select(x => x.GetObject(mocks)).ToArray();
        foreach (IRoom room in rooms)
        {
            allRooms.Add(room);
        }

        service.RequestIndexWarmup();
        service.DoIdleWork(TimeSpan.FromSeconds(5));
        long snapshotVersion = service.Diagnostics.CurrentSnapshotVersion;
        door.IsOpen = false;
        door.Locked = true;

        IPerceivable source = PerceivableAt(rooms[0], gameworld.Object);
        IPerceivable target = PerceivableAt(rooms[1], gameworld.Object);
        List<IRoomExit> path = source.PathBetween(target, 10, true, PathSearchOptions.Hierarchical).ToList();

        Assert.AreEqual(snapshotVersion, service.Diagnostics.CurrentSnapshotVersion,
            "Changing door state should not rebuild the topology index.");
        Assert.AreEqual(2, path.Count, "Expected pathing to reject the now-locked direct door and use the alternate route.");
        Assert.AreSame(rooms[2], path[0].Destination);
        Assert.AreSame(rooms[1], path[1].Destination);
    }

    [TestMethod]
    public void HierarchicalPathRetriesAlternatePortalWhenFinalSegmentFails()
    {
        All<IRoom> allRooms = new();
        Mock<IFuturemud> gameworld = new();
        Mock<IExitManager> exitManager = new();
        PathfindingService service = new(gameworld.Object);
        exitManager.Setup(x => x.PathfindingService).Returns(service);
        gameworld.Setup(x => x.Rooms).Returns(allRooms);
        gameworld.Setup(x => x.ExitManager).Returns(exitManager.Object);

        RoomStub sourceRoom = new()
        {
            Id = 2001,
            Name = "Source",
            Gameworld = gameworld.Object,
            Coordinates = (0, 0, 0 ),
            Exits = new List<RoomExitStub>()
        };
        RoomStub badPortal = new()
        {
            Id = 2002,
            Name = "Bad Portal",
            Gameworld = gameworld.Object,
            Coordinates = (11, 0, 0 ),
            Exits = new List<RoomExitStub>()
        };
        RoomStub goodPortal = new()
        {
            Id = 2003,
            Name = "Good Portal",
            Gameworld = gameworld.Object,
            Coordinates = (12, 0, 0 ),
            Exits = new List<RoomExitStub>()
        };
        RoomStub targetRoom = new()
        {
            Id = 2004,
            Name = "Target",
            Gameworld = gameworld.Object,
            Coordinates = (13, 0, 0 ),
            Exits = new List<RoomExitStub>()
        };

        sourceRoom.Exits.Add(new RoomExitStub
        {
            Destination = badPortal,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });
        sourceRoom.Exits.Add(new RoomExitStub
        {
            Destination = goodPortal,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.SouthEast
        });
        goodPortal.Exits.Add(new RoomExitStub
        {
            Destination = targetRoom,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });

        RoomStub[] stubs = [sourceRoom, badPortal, goodPortal, targetRoom];
        List<Mock<IRoom>> mocks = stubs.Select(x => x.ToMock()).ToList();
        IRoom[] rooms = stubs.Select(x => x.GetObject(mocks)).ToArray();
        foreach (IRoom room in rooms)
        {
            allRooms.Add(room);
        }

        service.RequestIndexWarmup();
        service.DoIdleWork(TimeSpan.FromSeconds(5));

        IPerceivable source = PerceivableAt(rooms[0], gameworld.Object);
        IPerceivable target = PerceivableAt(rooms[3], gameworld.Object);
        List<IRoomExit> path = source
                               .PathBetween(target, 10, _ => true, new PathSearchOptions
                               {
                                   Algorithm = PathSearchAlgorithm.Hierarchical,
                                   MaximumHierarchicalRetries = 2
                               })
                               .ToList();

        Assert.AreEqual(2, path.Count,
            "Expected hierarchical pathing to retry the alternate target-cluster portal after the first final segment failed.");
        Assert.AreSame(rooms[2], path[0].Destination);
        Assert.AreSame(rooms[3], path[1].Destination);
    }

    [TestMethod]
    public void TransientExitRegistrationInvalidatesPathfindingTopology()
    {
        All<IRoom> allRooms = new();
        Mock<IFuturemud> gameworld = new();
        gameworld.Setup(x => x.Rooms).Returns(allRooms);
        ExitManager manager = new(gameworld.Object);
        manager.PathfindingService.DoIdleWork(TimeSpan.FromSeconds(1));
        Assert.IsFalse(manager.PathfindingService.Diagnostics.IsDirty,
            "Expected the empty topology build to clear the initial dirty state.");

        Mock<IRoom> cell1 = new();
        cell1.Setup(x => x.Id).Returns(3001);
        Mock<IRoom> cell2 = new();
        cell2.Setup(x => x.Id).Returns(3002);
        Mock<IExit> exit = new();
        exit.Setup(x => x.Rooms).Returns(new[] { cell1.Object, cell2.Object });

        manager.RegisterTransientExit(exit.Object);

        Assert.IsTrue(manager.PathfindingService.Diagnostics.IsDirty,
            "Expected transient exits to invalidate the topology snapshot.");
    }

    [TestMethod]
    public void IdleBuildPublishesOnlyAfterBudgetedSlicesComplete()
    {
        (_, PathfindingService service, _) = BuildLinearPath(3);
        service.MaximumRoomsPerIdleSlice = 1;
        service.RequestIndexWarmup();

        service.DoIdleWork(TimeSpan.FromSeconds(1));
        Assert.AreEqual(0, service.Diagnostics.CurrentSnapshotVersion,
            "Expected the first one-cell slice not to publish a partial snapshot.");
        Assert.IsTrue(service.Diagnostics.IsBuildQueued);

        service.DoIdleWork(TimeSpan.FromSeconds(1));
        service.DoIdleWork(TimeSpan.FromSeconds(1));
        service.DoIdleWork(TimeSpan.FromSeconds(1));

        Assert.IsTrue(service.Diagnostics.CurrentSnapshotVersion > 0,
            "Expected the snapshot to publish only after all queued cells were processed.");
    }

    [TestMethod]
    public void TestASharpFlight()
    {
        List<IRoom> cellsunder = Person1Flight.RoomsUnderneathFlight(Person2Flight, 15).ToList();
        List<IRoom> expectedRooms = new()
        {
            TestRoomsNoDiagonals[5,4],
            TestRoomsNoDiagonals[5,3],
            TestRoomsNoDiagonals[6,5],
            TestRoomsNoDiagonals[6,4],
            TestRoomsNoDiagonals[6,3],
            TestRoomsNoDiagonals[7,5],
            TestRoomsNoDiagonals[7,4],
            TestRoomsNoDiagonals[7,3],
            TestRoomsNoDiagonals[8,5],
            TestRoomsNoDiagonals[8,4],
            TestRoomsNoDiagonals[8,3],
            TestRoomsNoDiagonals[9,5],
            TestRoomsNoDiagonals[9,4],
            TestRoomsNoDiagonals[9,3],
            TestRoomsNoDiagonals[10,5],
            TestRoomsNoDiagonals[10,4],
            TestRoomsNoDiagonals[10,3],
            TestRoomsNoDiagonals[11,5],
            TestRoomsNoDiagonals[11,4],
            TestRoomsNoDiagonals[11,3],
            TestRoomsNoDiagonals[12,5],
            TestRoomsNoDiagonals[12,4],
        };

        Assert.AreEqual(false, cellsunder.Any(x => !expectedRooms.Contains(x)), $"Found cells underneath the flight path that we didn't expect: {cellsunder.Where(x => !expectedRooms.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedRooms.Any(x => !cellsunder.Contains(x)), $"Missing cells underneath the flight path that we expected: {expectedRooms.Where(x => !cellsunder.Contains(x)).Select(x => x.Name).ListToString()}");
    }

    [TestMethod]
    public void TestASharpShortFlight()
    {
        IEnumerable<IRoom> cellsunder = Person1Flight.RoomsUnderneathFlight(Person4Flight, 15);
        List<IRoom> expectedRooms = new()
        {
            TestRoomsNoDiagonals[5,6],
        };

        Assert.AreEqual(false, cellsunder.Any(x => !expectedRooms.Contains(x)), $"Found cells underneath the flight path that we didn't expect: {cellsunder.Where(x => !expectedRooms.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedRooms.Any(x => !cellsunder.Contains(x)), $"Missing cells underneath the flight path that we expected: {expectedRooms.Where(x => !cellsunder.Contains(x)).Select(x => x.Name).ListToString()}");
    }

    [TestMethod]
    public void TestASharpVicinity()
    {
        IEnumerable<IRoom> vicinity0 = Person1Flight.RoomsInVicinity(0, true, true);
        Assert.AreEqual(true, vicinity0.Count() == 1, $"Expected only 1 cell in Vicinity0, got {vicinity0.Count()}");
        Assert.AreEqual(true, vicinity0.First() == TestRoomsNoDiagonals[5, 5], $"Expected only cell 5,5 in Vicinity0, got {vicinity0.First()}");

        IEnumerable<IRoom> vicinity1 = Person1Flight.RoomsInVicinity(1, true, true);
        List<IRoom> expectedRooms1 = new()
        {
            TestRoomsNoDiagonals[5,5],
            TestRoomsNoDiagonals[5,6],
            TestRoomsNoDiagonals[6,5],
            TestRoomsNoDiagonals[4,5],
            TestRoomsNoDiagonals[5,4],
        };
        Assert.AreEqual(false, vicinity1.Any(x => !expectedRooms1.Contains(x)), $"Found cells in vicinity1 that we didn't expect: {vicinity1.Where(x => !expectedRooms1.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedRooms1.Any(x => !vicinity1.Contains(x)), $"Missing cells in vicinity1 that we expected: {expectedRooms1.Where(x => !vicinity1.Contains(x)).Select(x => x.Name).ListToString()}");

        IEnumerable<IRoom> vicinity2 = Person1Flight.RoomsInVicinity(2, true, true);
        List<IRoom> expectedRooms2 = new()
        {
            TestRoomsNoDiagonals[5,5],
            TestRoomsNoDiagonals[4,6],
            TestRoomsNoDiagonals[6,4],
            TestRoomsNoDiagonals[4,4],
            TestRoomsNoDiagonals[6,6],
            TestRoomsNoDiagonals[5,6],
            TestRoomsNoDiagonals[6,5],
            TestRoomsNoDiagonals[4,5],
            TestRoomsNoDiagonals[5,4],
            TestRoomsNoDiagonals[5,7],
            TestRoomsNoDiagonals[7,5],
            TestRoomsNoDiagonals[3,5],
            TestRoomsNoDiagonals[5,3],
        };
        Assert.AreEqual(false, vicinity2.Any(x => !expectedRooms2.Contains(x)), $"Found cells in vicinity2 that we didn't expect: {vicinity2.Where(x => !expectedRooms2.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedRooms2.Any(x => !vicinity2.Contains(x)), $"Missing cells in vicinity2 that we expected: {expectedRooms2.Where(x => !vicinity2.Contains(x)).Select(x => x.Name).ListToString()}");
    }

    [TestMethod]
    public void TestASharpTooLong()
    {
        IEnumerable<IRoomExit> path = Person1Flight.ExitsBetween(Person5Flight, 15);
        Assert.AreEqual(false, path.Any(), "Expected ExitsBetween not to find a path");
    }

    [TestMethod]
    public void TestASharpVeryLong()
    {
        IEnumerable<IRoomExit> path = Person1Flight.ExitsBetween(Person5Flight, 100);
        Assert.AreEqual(true, path.Any(), "Expected ExitsBetween to find a path");
    }

    [TestMethod]
    public void TestClosedDoorOpenDoorsFlag()
    {
        RoomStub cell1 = new()
        {
            Name = "A",
            Coordinates = (0, 0, 0 ),
            Exits = new List<RoomExitStub>(),
            Id = 1
        };
        RoomStub cell2 = new()
        {
            Name = "B",
            Coordinates = (1, 0, 0 ),
            Exits = new List<RoomExitStub>(),
            Id = 2
        };
        DoorStub door = new() { IsOpen = false, Locked = false, State = DoorState.Closed };
        RoomExitStub exit1 = new()
        {
            Destination = cell2,
            Exit = new ExitStub { Door = door },
            OutboundDirection = CardinalDirection.East
        };
        RoomExitStub exit2 = new()
        {
            Destination = cell1,
            Exit = new ExitStub { Door = door },
            OutboundDirection = CardinalDirection.West
        };
        cell1.Exits.Add(exit1);
        cell2.Exits.Add(exit2);

        List<Mock<IRoom>> cellMocks = new()
        { cell1.ToMock(), cell2.ToMock() };
        IRoom c1 = cell1.GetObject(cellMocks);
        IRoom c2 = cell2.GetObject(cellMocks);

        IPerceivable source = new PerceivableStub { Location = c1 }.ToMock();
        IPerceivable target = new PerceivableStub { Location = c2 }.ToMock();

        List<IRoomExit> path = source.PathBetween(target, 5, false).ToList();
        Assert.AreEqual(0, path.Count, "Expected closed door to block path when openDoors is false");

        path = source.PathBetween(target, 5, true).ToList();
        Assert.AreEqual(1, path.Count, "Expected path through closed but unlocked door when openDoors is true");
    }

    [TestMethod]
    public void PathBetweenMultipleTargetsRejectsUnsuitableDirectExit()
    {
        RoomStub cellA = new()
        {
            Name = "A",
            Coordinates = (0, 0, 0 ),
            Exits = new List<RoomExitStub>(),
            Id = 101
        };
        RoomStub cellB = new()
        {
            Name = "B",
            Coordinates = (1, 0, 0 ),
            Exits = new List<RoomExitStub>(),
            Id = 102
        };
        RoomStub cellC = new()
        {
            Name = "C",
            Coordinates = (0, 1, 0 ),
            Exits = new List<RoomExitStub>(),
            Id = 103
        };

        cellA.Exits.Add(new RoomExitStub
        {
            Destination = cellB,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });
        cellA.Exits.Add(new RoomExitStub
        {
            Destination = cellC,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.South
        });
        cellC.Exits.Add(new RoomExitStub
        {
            Destination = cellB,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });

        List<Mock<IRoom>> cellMocks = new()
        { cellA.ToMock(), cellB.ToMock(), cellC.ToMock() };
        IRoom cA = cellA.GetObject(cellMocks);
        IRoom cB = cellB.GetObject(cellMocks);
        IRoom cC = cellC.GetObject(cellMocks);

        IPerceivable source = new PerceivableStub { Location = cA }.ToMock();
        IPerceivable target = new PerceivableStub { Location = cB }.ToMock();

        List<IRoomExit> path = source
                               .PathBetween(new[] { target }, 5,
                                   exit => !(ReferenceEquals(exit.Origin, cA) && ReferenceEquals(exit.Destination, cB)))
                               .ToList();

        Assert.AreEqual(2, path.Count, "Expected path to avoid the unsuitable direct exit and use the alternate route.");
        Assert.AreSame(cC, path[0].Destination, "Expected the first step to go through the alternate cell.");
        Assert.AreSame(cB, path[1].Destination, "Expected the second step to reach the target.");
    }

    [TestMethod]
    public void AcquireAllTargetsAndPathsDoesNotPassNullForNonMatchingTypes()
    {
        RoomStub room = new()
        {
            Name = "A",
            Coordinates = (0, 0, 0 ),
            Exits = new List<RoomExitStub>(),
            Id = 201
        };

        List<Mock<IRoom>> cellMocks = new()
        { room.ToMock() };
        IRoom cA = room.GetObject(cellMocks);
        IPerceivable nonCharacter = new PerceivableStub { Location = cA }.ToMock();
        Mock<ICharacter> character = new();
        room.Perceivables.Add(nonCharacter);
        room.Perceivables.Add(character.Object);

        IPerceivable source = new PerceivableStub { Location = cA }.ToMock();
        int predicateCalls = 0;
        List<(ICharacter Target, IEnumerable<IRoomExit> Path)> results = source
                                                                        .AcquireAllTargetsAndPaths<ICharacter>(
                                                                            target =>
                                                                            {
                                                                                Assert.IsNotNull(target);
                                                                                predicateCalls++;
                                                                                return true;
                                                                            },
                                                                            0,
                                                                            _ => true)
                                                                        .ToList();

        Assert.AreEqual(1, results.Count, "Expected only the character target to be returned.");
        Assert.AreSame(character.Object, results[0].Target);
        Assert.AreEqual(1, predicateCalls, "Expected the predicate to run once for the character target only.");
    }
}

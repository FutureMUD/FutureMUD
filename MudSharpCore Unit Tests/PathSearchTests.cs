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
        TestCells = GetTestCells;
        TestCellsNoDiagonals = GetTestCellsNoDiagonals;
        Person1Flight = new PerceivableStub { Location = TestCellsNoDiagonals[5, 5] }.ToMock();
        Person2Flight = new PerceivableStub { Location = TestCellsNoDiagonals[12, 3] }.ToMock();
        Person3Flight = new PerceivableStub { Location = TestCellsNoDiagonals[5, 6] }.ToMock();
        Person4Flight = new PerceivableStub { Location = TestCellsNoDiagonals[5, 7] }.ToMock();
        Person5Flight = new PerceivableStub { Location = TestCellsNoDiagonals[45, 17] }.ToMock();
    }

    public static ICell[,] TestCells { get; set; }
    public static ICell[,] TestCellsNoDiagonals { get; set; }

    public static ICell[,] GetTestCells
    {
        get
        {
            CellStub[,] cellMap = new CellStub[50, 50];
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    List<CellExitStub> exits = new();
                    CellStub cell = new()
                    {
                        Id = i * 50 + j + 1,
                        Room = new RoomStub
                        {
                            X = i,
                            Y = j,
                            Z = 0
                        }.ToMock()
                    };
                    if (i > 0)
                    {
                        exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.West,
                            Destination = cellMap[i - 1, j]
                        });
                        cellMap[i - 1, j].Exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.East,
                            Destination = cell
                        });

                        if (j > 0)
                        {
                            exits.Add(new CellExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.SouthWest,
                                Destination = cellMap[i - 1, j - 1]
                            });
                            cellMap[i - 1, j - 1].Exits.Add(new CellExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.NorthEast,
                                Destination = cell
                            });
                        }

                        if (j < 49)
                        {
                            exits.Add(new CellExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.NorthWest,
                                Destination = cellMap[i - 1, j + 1]
                            });
                            cellMap[i - 1, j + 1].Exits.Add(new CellExitStub()
                            {
                                Exit = new ExitStub() { Door = null },
                                OutboundDirection = CardinalDirection.SouthEast,
                                Destination = cell
                            });
                        }
                    }

                    if (j > 0)
                    {
                        exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.South,
                            Destination = cellMap[i, j - 1]
                        });
                        cellMap[i, j - 1].Exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.North,
                            Destination = cell
                        });
                    }

                    cell.Exits = exits;
                    cellMap[i, j] = cell;
                    cell.Name = $"Cell {i},{j}";
                }
            }

            ICell[,] returnMap = new ICell[50, 50];
            List<Mock<ICell>> cellMocks = cellMap.OfType<CellStub>().Select(x => x.ToMock()).ToList();
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

    public static ICell[,] GetTestCellsNoDiagonals
    {
        get
        {
            CellStub[,] cellMap = new CellStub[50, 50];
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    List<CellExitStub> exits = new();
                    CellStub cell = new()
                    {
                        Id = i * 50 + j + 1,
                        Room = new RoomStub
                        {
                            X = i,
                            Y = j,
                            Z = 0
                        }.ToMock()
                    };
                    if (i > 0)
                    {
                        exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.West,
                            Destination = cellMap[i - 1, j]
                        });
                        cellMap[i - 1, j].Exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.East,
                            Destination = cell
                        });
                    }

                    if (j > 0)
                    {
                        exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.South,
                            Destination = cellMap[i, j - 1]
                        });
                        cellMap[i, j - 1].Exits.Add(new CellExitStub()
                        {
                            Exit = new ExitStub() { Door = null },
                            OutboundDirection = CardinalDirection.North,
                            Destination = cell
                        });
                    }

                    cell.Exits = exits;
                    cellMap[i, j] = cell;
                    cell.Name = $"Cell {i},{j}";
                }
            }

            ICell[,] returnMap = new ICell[50, 50];
            List<Mock<ICell>> cellMocks = cellMap.OfType<CellStub>().Select(x => x.ToMock()).ToList();
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

    private static (IFuturemud Gameworld, PathfindingService Service, ICell[] Cells) BuildLinearPath(int count)
    {
        All<ICell> allCells = new();
        Mock<IFuturemud> gameworld = new();
        Mock<IExitManager> exitManager = new();
        PathfindingService service = new(gameworld.Object);
        exitManager.Setup(x => x.PathfindingService).Returns(service);
        gameworld.Setup(x => x.Cells).Returns(allCells);
        gameworld.Setup(x => x.ExitManager).Returns(exitManager.Object);

        CellStub[] cellStubs = Enumerable.Range(0, count)
                                         .Select(i => new CellStub
                                         {
                                             Id = i + 1,
                                             Name = $"Linear {i}",
                                             Gameworld = gameworld.Object,
                                             Room = new RoomStub { X = i, Y = 0, Z = 0 }.ToMock(),
                                             Exits = new List<CellExitStub>()
                                         })
                                         .ToArray();
        for (int i = 0; i < count - 1; i++)
        {
            cellStubs[i].Exits.Add(new CellExitStub
            {
                Destination = cellStubs[i + 1],
                Exit = new ExitStub(),
                OutboundDirection = CardinalDirection.East
            });
            cellStubs[i + 1].Exits.Add(new CellExitStub
            {
                Destination = cellStubs[i],
                Exit = new ExitStub(),
                OutboundDirection = CardinalDirection.West
            });
        }

        List<Mock<ICell>> cellMocks = cellStubs.Select(x => x.ToMock()).ToList();
        ICell[] cells = cellStubs.Select(x => x.GetObject(cellMocks)).ToArray();
        foreach (ICell cell in cells)
        {
            allCells.Add(cell);
        }

        return (gameworld.Object, service, cells);
    }

    private static IPerceivable PerceivableAt(ICell cell, IFuturemud gameworld)
    {
        return new PerceivableStub { Location = cell, Gameworld = gameworld }.ToMock();
    }

    [TestMethod]
    public void TestASharpPath()
    {
        List<ICellExit> path = Person1Flight.PathBetween(Person2Flight, 15, true).ToList();
        Assert.AreEqual(9, path.Count, $"It was expected that the two persons were 9 squares apart but they were {path.Count} apart instead. \n\nPath was: {path.Select(x => x.OutboundDirection.DescribeBrief()).ListToString()}.\n\n{path.Select(x => x.Destination.Name).ListToString()}");

        path = Person1Flight.PathBetween(Person3Flight, 1, true).ToList();
        Assert.AreEqual(1, path.Count, $"It was expected that the two persons were 1 square apart but they were {path.Count} apart instead. \n\nPath was: {path.Select(x => x.OutboundDirection.DescribeBrief()).ListToString()}.\n\n{path.Select(x => x.Destination.Name).ListToString()}");
    }

    [TestMethod]
    public void AutomaticPathBelowThresholdUsesExactSearch()
    {
        List<ICellExit> path = Person1Flight.PathBetween(Person2Flight, 15, true,
            new PathSearchOptions { Algorithm = PathSearchAlgorithm.Automatic, HierarchicalThreshold = 100 }).ToList();

        Assert.AreEqual(9, path.Count, "Expected automatic mode below threshold to preserve exact pathing.");
    }

    [TestMethod]
    public void HierarchicalPathFindsLongLinearRouteAfterIdleBuild()
    {
        (IFuturemud gameworld, PathfindingService service, ICell[] cells) = BuildLinearPath(100);
        service.RequestIndexWarmup();
        service.DoIdleWork(TimeSpan.FromSeconds(5));

        IPerceivable source = PerceivableAt(cells[0], gameworld);
        IPerceivable target = PerceivableAt(cells[99], gameworld);
        List<ICellExit> path = source.PathBetween(target, 150, _ => true, PathSearchOptions.Hierarchical).ToList();

        Assert.AreEqual(99, path.Count, "Expected hierarchical pathing to produce the full long route.");
        Assert.IsTrue(service.Diagnostics.CurrentSnapshotVersion > 0, "Expected idle work to publish an index snapshot.");
    }

    [TestMethod]
    public void HierarchicalPathValidatesDoorStateWithoutRebuildingIndex()
    {
        All<ICell> allCells = new();
        Mock<IFuturemud> gameworld = new();
        Mock<IExitManager> exitManager = new();
        PathfindingService service = new(gameworld.Object);
        exitManager.Setup(x => x.PathfindingService).Returns(service);
        gameworld.Setup(x => x.Cells).Returns(allCells);
        gameworld.Setup(x => x.ExitManager).Returns(exitManager.Object);

        DoorStub door = new() { IsOpen = true, Locked = false, State = DoorState.Open };
        CellStub cellA = new()
        {
            Id = 1001,
            Name = "A",
            Gameworld = gameworld.Object,
            Room = new RoomStub { X = 0, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>()
        };
        CellStub cellB = new()
        {
            Id = 1002,
            Name = "B",
            Gameworld = gameworld.Object,
            Room = new RoomStub { X = 11, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>()
        };
        CellStub cellC = new()
        {
            Id = 1003,
            Name = "C",
            Gameworld = gameworld.Object,
            Room = new RoomStub { X = 0, Y = 11, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>()
        };

        cellA.Exits.Add(new CellExitStub
        {
            Destination = cellB,
            Exit = new ExitStub { Door = door },
            OutboundDirection = CardinalDirection.East
        });
        cellA.Exits.Add(new CellExitStub
        {
            Destination = cellC,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.South
        });
        cellC.Exits.Add(new CellExitStub
        {
            Destination = cellB,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });

        CellStub[] stubs = [cellA, cellB, cellC];
        List<Mock<ICell>> mocks = stubs.Select(x => x.ToMock()).ToList();
        ICell[] cells = stubs.Select(x => x.GetObject(mocks)).ToArray();
        foreach (ICell cell in cells)
        {
            allCells.Add(cell);
        }

        service.RequestIndexWarmup();
        service.DoIdleWork(TimeSpan.FromSeconds(5));
        long snapshotVersion = service.Diagnostics.CurrentSnapshotVersion;
        door.IsOpen = false;
        door.Locked = true;

        IPerceivable source = PerceivableAt(cells[0], gameworld.Object);
        IPerceivable target = PerceivableAt(cells[1], gameworld.Object);
        List<ICellExit> path = source.PathBetween(target, 10, true, PathSearchOptions.Hierarchical).ToList();

        Assert.AreEqual(snapshotVersion, service.Diagnostics.CurrentSnapshotVersion,
            "Changing door state should not rebuild the topology index.");
        Assert.AreEqual(2, path.Count, "Expected pathing to reject the now-locked direct door and use the alternate route.");
        Assert.AreSame(cells[2], path[0].Destination);
        Assert.AreSame(cells[1], path[1].Destination);
    }

    [TestMethod]
    public void HierarchicalPathRetriesAlternatePortalWhenFinalSegmentFails()
    {
        All<ICell> allCells = new();
        Mock<IFuturemud> gameworld = new();
        Mock<IExitManager> exitManager = new();
        PathfindingService service = new(gameworld.Object);
        exitManager.Setup(x => x.PathfindingService).Returns(service);
        gameworld.Setup(x => x.Cells).Returns(allCells);
        gameworld.Setup(x => x.ExitManager).Returns(exitManager.Object);

        CellStub sourceCell = new()
        {
            Id = 2001,
            Name = "Source",
            Gameworld = gameworld.Object,
            Room = new RoomStub { X = 0, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>()
        };
        CellStub badPortal = new()
        {
            Id = 2002,
            Name = "Bad Portal",
            Gameworld = gameworld.Object,
            Room = new RoomStub { X = 11, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>()
        };
        CellStub goodPortal = new()
        {
            Id = 2003,
            Name = "Good Portal",
            Gameworld = gameworld.Object,
            Room = new RoomStub { X = 12, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>()
        };
        CellStub targetCell = new()
        {
            Id = 2004,
            Name = "Target",
            Gameworld = gameworld.Object,
            Room = new RoomStub { X = 13, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>()
        };

        sourceCell.Exits.Add(new CellExitStub
        {
            Destination = badPortal,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });
        sourceCell.Exits.Add(new CellExitStub
        {
            Destination = goodPortal,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.SouthEast
        });
        goodPortal.Exits.Add(new CellExitStub
        {
            Destination = targetCell,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });

        CellStub[] stubs = [sourceCell, badPortal, goodPortal, targetCell];
        List<Mock<ICell>> mocks = stubs.Select(x => x.ToMock()).ToList();
        ICell[] cells = stubs.Select(x => x.GetObject(mocks)).ToArray();
        foreach (ICell cell in cells)
        {
            allCells.Add(cell);
        }

        service.RequestIndexWarmup();
        service.DoIdleWork(TimeSpan.FromSeconds(5));

        IPerceivable source = PerceivableAt(cells[0], gameworld.Object);
        IPerceivable target = PerceivableAt(cells[3], gameworld.Object);
        List<ICellExit> path = source
                               .PathBetween(target, 10, _ => true, new PathSearchOptions
                               {
                                   Algorithm = PathSearchAlgorithm.Hierarchical,
                                   MaximumHierarchicalRetries = 2
                               })
                               .ToList();

        Assert.AreEqual(2, path.Count,
            "Expected hierarchical pathing to retry the alternate target-cluster portal after the first final segment failed.");
        Assert.AreSame(cells[2], path[0].Destination);
        Assert.AreSame(cells[3], path[1].Destination);
    }

    [TestMethod]
    public void TransientExitRegistrationInvalidatesPathfindingTopology()
    {
        All<ICell> allCells = new();
        Mock<IFuturemud> gameworld = new();
        gameworld.Setup(x => x.Cells).Returns(allCells);
        ExitManager manager = new(gameworld.Object);
        manager.PathfindingService.DoIdleWork(TimeSpan.FromSeconds(1));
        Assert.IsFalse(manager.PathfindingService.Diagnostics.IsDirty,
            "Expected the empty topology build to clear the initial dirty state.");

        Mock<ICell> cell1 = new();
        cell1.Setup(x => x.Id).Returns(3001);
        Mock<ICell> cell2 = new();
        cell2.Setup(x => x.Id).Returns(3002);
        Mock<IExit> exit = new();
        exit.Setup(x => x.Cells).Returns(new[] { cell1.Object, cell2.Object });

        manager.RegisterTransientExit(exit.Object);

        Assert.IsTrue(manager.PathfindingService.Diagnostics.IsDirty,
            "Expected transient exits to invalidate the topology snapshot.");
    }

    [TestMethod]
    public void IdleBuildPublishesOnlyAfterBudgetedSlicesComplete()
    {
        (_, PathfindingService service, _) = BuildLinearPath(3);
        service.MaximumCellsPerIdleSlice = 1;
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
        List<ICell> cellsunder = Person1Flight.CellsUnderneathFlight(Person2Flight, 15).ToList();
        List<ICell> expectedCells = new()
        {
            TestCellsNoDiagonals[5,4],
            TestCellsNoDiagonals[5,3],
            TestCellsNoDiagonals[6,5],
            TestCellsNoDiagonals[6,4],
            TestCellsNoDiagonals[6,3],
            TestCellsNoDiagonals[7,5],
            TestCellsNoDiagonals[7,4],
            TestCellsNoDiagonals[7,3],
            TestCellsNoDiagonals[8,5],
            TestCellsNoDiagonals[8,4],
            TestCellsNoDiagonals[8,3],
            TestCellsNoDiagonals[9,5],
            TestCellsNoDiagonals[9,4],
            TestCellsNoDiagonals[9,3],
            TestCellsNoDiagonals[10,5],
            TestCellsNoDiagonals[10,4],
            TestCellsNoDiagonals[10,3],
            TestCellsNoDiagonals[11,5],
            TestCellsNoDiagonals[11,4],
            TestCellsNoDiagonals[11,3],
            TestCellsNoDiagonals[12,5],
            TestCellsNoDiagonals[12,4],
        };

        Assert.AreEqual(false, cellsunder.Any(x => !expectedCells.Contains(x)), $"Found cells underneath the flight path that we didn't expect: {cellsunder.Where(x => !expectedCells.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedCells.Any(x => !cellsunder.Contains(x)), $"Missing cells underneath the flight path that we expected: {expectedCells.Where(x => !cellsunder.Contains(x)).Select(x => x.Name).ListToString()}");
    }

    [TestMethod]
    public void TestASharpShortFlight()
    {
        IEnumerable<ICell> cellsunder = Person1Flight.CellsUnderneathFlight(Person4Flight, 15);
        List<ICell> expectedCells = new()
        {
            TestCellsNoDiagonals[5,6],
        };

        Assert.AreEqual(false, cellsunder.Any(x => !expectedCells.Contains(x)), $"Found cells underneath the flight path that we didn't expect: {cellsunder.Where(x => !expectedCells.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedCells.Any(x => !cellsunder.Contains(x)), $"Missing cells underneath the flight path that we expected: {expectedCells.Where(x => !cellsunder.Contains(x)).Select(x => x.Name).ListToString()}");
    }

    [TestMethod]
    public void TestASharpVicinity()
    {
        IEnumerable<ICell> vicinity0 = Person1Flight.CellsInVicinity(0, true, true);
        Assert.AreEqual(true, vicinity0.Count() == 1, $"Expected only 1 cell in Vicinity0, got {vicinity0.Count()}");
        Assert.AreEqual(true, vicinity0.First() == TestCellsNoDiagonals[5, 5], $"Expected only cell 5,5 in Vicinity0, got {vicinity0.First()}");

        IEnumerable<ICell> vicinity1 = Person1Flight.CellsInVicinity(1, true, true);
        List<ICell> expectedCells1 = new()
        {
            TestCellsNoDiagonals[5,5],
            TestCellsNoDiagonals[5,6],
            TestCellsNoDiagonals[6,5],
            TestCellsNoDiagonals[4,5],
            TestCellsNoDiagonals[5,4],
        };
        Assert.AreEqual(false, vicinity1.Any(x => !expectedCells1.Contains(x)), $"Found cells in vicinity1 that we didn't expect: {vicinity1.Where(x => !expectedCells1.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedCells1.Any(x => !vicinity1.Contains(x)), $"Missing cells in vicinity1 that we expected: {expectedCells1.Where(x => !vicinity1.Contains(x)).Select(x => x.Name).ListToString()}");

        IEnumerable<ICell> vicinity2 = Person1Flight.CellsInVicinity(2, true, true);
        List<ICell> expectedCells2 = new()
        {
            TestCellsNoDiagonals[5,5],
            TestCellsNoDiagonals[4,6],
            TestCellsNoDiagonals[6,4],
            TestCellsNoDiagonals[4,4],
            TestCellsNoDiagonals[6,6],
            TestCellsNoDiagonals[5,6],
            TestCellsNoDiagonals[6,5],
            TestCellsNoDiagonals[4,5],
            TestCellsNoDiagonals[5,4],
            TestCellsNoDiagonals[5,7],
            TestCellsNoDiagonals[7,5],
            TestCellsNoDiagonals[3,5],
            TestCellsNoDiagonals[5,3],
        };
        Assert.AreEqual(false, vicinity2.Any(x => !expectedCells2.Contains(x)), $"Found cells in vicinity2 that we didn't expect: {vicinity2.Where(x => !expectedCells2.Contains(x)).Select(x => x.Name).ListToString()}");
        Assert.AreEqual(false, expectedCells2.Any(x => !vicinity2.Contains(x)), $"Missing cells in vicinity2 that we expected: {expectedCells2.Where(x => !vicinity2.Contains(x)).Select(x => x.Name).ListToString()}");
    }

    [TestMethod]
    public void TestASharpTooLong()
    {
        IEnumerable<ICellExit> path = Person1Flight.ExitsBetween(Person5Flight, 15);
        Assert.AreEqual(false, path.Any(), "Expected ExitsBetween not to find a path");
    }

    [TestMethod]
    public void TestASharpVeryLong()
    {
        IEnumerable<ICellExit> path = Person1Flight.ExitsBetween(Person5Flight, 100);
        Assert.AreEqual(true, path.Any(), "Expected ExitsBetween to find a path");
    }

    [TestMethod]
    public void TestClosedDoorOpenDoorsFlag()
    {
        CellStub cell1 = new()
        {
            Name = "A",
            Room = new RoomStub { X = 0, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>(),
            Id = 1
        };
        CellStub cell2 = new()
        {
            Name = "B",
            Room = new RoomStub { X = 1, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>(),
            Id = 2
        };
        DoorStub door = new() { IsOpen = false, Locked = false, State = DoorState.Closed };
        CellExitStub exit1 = new()
        {
            Destination = cell2,
            Exit = new ExitStub { Door = door },
            OutboundDirection = CardinalDirection.East
        };
        CellExitStub exit2 = new()
        {
            Destination = cell1,
            Exit = new ExitStub { Door = door },
            OutboundDirection = CardinalDirection.West
        };
        cell1.Exits.Add(exit1);
        cell2.Exits.Add(exit2);

        List<Mock<ICell>> cellMocks = new()
        { cell1.ToMock(), cell2.ToMock() };
        ICell c1 = cell1.GetObject(cellMocks);
        ICell c2 = cell2.GetObject(cellMocks);

        IPerceivable source = new PerceivableStub { Location = c1 }.ToMock();
        IPerceivable target = new PerceivableStub { Location = c2 }.ToMock();

        List<ICellExit> path = source.PathBetween(target, 5, false).ToList();
        Assert.AreEqual(0, path.Count, "Expected closed door to block path when openDoors is false");

        path = source.PathBetween(target, 5, true).ToList();
        Assert.AreEqual(1, path.Count, "Expected path through closed but unlocked door when openDoors is true");
    }

    [TestMethod]
    public void PathBetweenMultipleTargetsRejectsUnsuitableDirectExit()
    {
        CellStub cellA = new()
        {
            Name = "A",
            Room = new RoomStub { X = 0, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>(),
            Id = 101
        };
        CellStub cellB = new()
        {
            Name = "B",
            Room = new RoomStub { X = 1, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>(),
            Id = 102
        };
        CellStub cellC = new()
        {
            Name = "C",
            Room = new RoomStub { X = 0, Y = 1, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>(),
            Id = 103
        };

        cellA.Exits.Add(new CellExitStub
        {
            Destination = cellB,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });
        cellA.Exits.Add(new CellExitStub
        {
            Destination = cellC,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.South
        });
        cellC.Exits.Add(new CellExitStub
        {
            Destination = cellB,
            Exit = new ExitStub(),
            OutboundDirection = CardinalDirection.East
        });

        List<Mock<ICell>> cellMocks = new()
        { cellA.ToMock(), cellB.ToMock(), cellC.ToMock() };
        ICell cA = cellA.GetObject(cellMocks);
        ICell cB = cellB.GetObject(cellMocks);
        ICell cC = cellC.GetObject(cellMocks);

        IPerceivable source = new PerceivableStub { Location = cA }.ToMock();
        IPerceivable target = new PerceivableStub { Location = cB }.ToMock();

        List<ICellExit> path = source
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
        CellStub cell = new()
        {
            Name = "A",
            Room = new RoomStub { X = 0, Y = 0, Z = 0 }.ToMock(),
            Exits = new List<CellExitStub>(),
            Id = 201
        };

        List<Mock<ICell>> cellMocks = new()
        { cell.ToMock() };
        ICell cA = cell.GetObject(cellMocks);
        IPerceivable nonCharacter = new PerceivableStub { Location = cA }.ToMock();
        Mock<ICharacter> character = new();
        cell.Perceivables.Add(nonCharacter);
        cell.Perceivables.Add(character.Object);

        IPerceivable source = new PerceivableStub { Location = cA }.ToMock();
        int predicateCalls = 0;
        List<(ICharacter Target, IEnumerable<ICellExit> Path)> results = source
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

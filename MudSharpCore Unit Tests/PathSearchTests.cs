using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests {
	[TestClass]
	public class PathSearchTests {

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
		
		public static ICell[,] GetTestCells { get {
				var cellMap = new CellStub[50,50];
				for (int i = 0; i < 50; i++) {
					for (int j = 0; j < 50; j++) {
						var exits = new List<CellExitStub>();
						var cell = new CellStub() {
							Room = new RoomStub
							{
								X = i,
								Y = j,
								Z = 0
							}.ToMock()
						};
						if (i > 0) {
							exits.Add(new CellExitStub() {
								Exit = new ExitStub() { Door = null },
								OutboundDirection = CardinalDirection.West,
								Destination = cellMap[i - 1, j]
							});
							cellMap[i - 1, j].Exits.Add(new CellExitStub() {
								Exit = new ExitStub() {Door = null },
								OutboundDirection = CardinalDirection.East,
								Destination = cell
							});

							if (j > 0) {
								exits.Add(new CellExitStub() {
									Exit = new ExitStub() { Door = null },
									OutboundDirection = CardinalDirection.SouthWest,
									Destination = cellMap[i - 1, j - 1]
								});
								cellMap[i - 1, j - 1].Exits.Add(new CellExitStub() {
									Exit = new ExitStub() { Door = null },
									OutboundDirection = CardinalDirection.NorthEast,
									Destination = cell
								});
							}

							if (j < 49) {
								exits.Add(new CellExitStub() {
									Exit = new ExitStub() { Door = null },
									OutboundDirection = CardinalDirection.NorthWest,
									Destination = cellMap[i - 1, j + 1]
								});
								cellMap[i - 1, j + 1].Exits.Add(new CellExitStub() {
									Exit = new ExitStub() { Door = null },
									OutboundDirection = CardinalDirection.SouthEast,
									Destination = cell
								});
							}
						}
						
						if (j > 0) {
							exits.Add(new CellExitStub() {
								Exit = new ExitStub() { Door = null },
								OutboundDirection = CardinalDirection.South,
								Destination = cellMap[i, j - 1]
							});
							cellMap[i, j - 1].Exits.Add(new CellExitStub() {
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

				var returnMap = new ICell[50, 50];
				var cellMocks = cellMap.OfType<CellStub>().Select(x => x.ToMock()).ToList();
				for (int i = 0; i < 50; i++) {
					for (int j = 0; j < 50; j++) {
						returnMap[i, j] = cellMap[i, j].GetObject(cellMocks);
					}
				}
				return returnMap;
			}
		}

		public static ICell[,] GetTestCellsNoDiagonals {
			get {
				var cellMap = new CellStub[50, 50];
				for (int i = 0; i < 50; i++) {
					for (int j = 0; j < 50; j++) {
						var exits = new List<CellExitStub>();
						var cell = new CellStub()
						{
							Room = new RoomStub
							{
								X = i,
								Y = j,
								Z = 0
							}.ToMock()
						};
						if (i > 0) {
							exits.Add(new CellExitStub() {
								Exit = new ExitStub() { Door = null },
								OutboundDirection = CardinalDirection.West,
								Destination = cellMap[i - 1, j]
							});
							cellMap[i - 1, j].Exits.Add(new CellExitStub() {
								Exit = new ExitStub() { Door = null },
								OutboundDirection = CardinalDirection.East,
								Destination = cell
							});
						}

						if (j > 0) {
							exits.Add(new CellExitStub() {
								Exit = new ExitStub() { Door = null },
								OutboundDirection = CardinalDirection.South,
								Destination = cellMap[i, j - 1]
							});
							cellMap[i, j - 1].Exits.Add(new CellExitStub() {
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

				var returnMap = new ICell[50, 50];
				var cellMocks = cellMap.OfType<CellStub>().Select(x => x.ToMock()).ToList();
				for (int i = 0; i < 50; i++) {
					for (int j = 0; j < 50; j++) {
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

		[TestMethod]
		public void TestASharpPath() {
			var path = Person1Flight.PathBetween(Person2Flight, 15, true).ToList();
			Assert.AreEqual(9, path.Count, $"It was expected that the two persons were 9 squares apart but they were {path.Count} apart instead. \n\nPath was: {path.Select(x => x.OutboundDirection.DescribeBrief()).ListToString()}.\n\n{path.Select(x => x.Destination.Name).ListToString()}");

			path = Person1Flight.PathBetween(Person3Flight, 1, true).ToList();
			Assert.AreEqual(1, path.Count, $"It was expected that the two persons were 1 square apart but they were {path.Count} apart instead. \n\nPath was: {path.Select(x => x.OutboundDirection.DescribeBrief()).ListToString()}.\n\n{path.Select(x => x.Destination.Name).ListToString()}");
		}

		[TestMethod]
		public void TestASharpFlight() {
			var cellsunder = Person1Flight.CellsUnderneathFlight(Person2Flight, 15).ToList();
			var expectedCells = new List<ICell> {
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
		public void TestASharpShortFlight() {
			var cellsunder = Person1Flight.CellsUnderneathFlight(Person4Flight, 15);
			var expectedCells = new List<ICell> {
				TestCellsNoDiagonals[5,6],
			};

			Assert.AreEqual(false, cellsunder.Any(x => !expectedCells.Contains(x)), $"Found cells underneath the flight path that we didn't expect: {cellsunder.Where(x => !expectedCells.Contains(x)).Select(x => x.Name).ListToString()}");
			Assert.AreEqual(false, expectedCells.Any(x => !cellsunder.Contains(x)), $"Missing cells underneath the flight path that we expected: {expectedCells.Where(x => !cellsunder.Contains(x)).Select(x => x.Name).ListToString()}");
		}
		
		[TestMethod]
		public void TestASharpVicinity() {
			var vicinity0 = Person1Flight.CellsInVicinity(0, true, true);
			Assert.AreEqual(true, vicinity0.Count() == 1, $"Expected only 1 cell in Vicinity0, got {vicinity0.Count()}");
			Assert.AreEqual(true, vicinity0.First() == TestCellsNoDiagonals[5,5], $"Expected only cell 5,5 in Vicinity0, got {vicinity0.First()}");

			var vicinity1 = Person1Flight.CellsInVicinity(1, true, true);
			var expectedCells1 = new List<ICell> {
				TestCellsNoDiagonals[5,5],
				TestCellsNoDiagonals[5,6],
				TestCellsNoDiagonals[6,5],
				TestCellsNoDiagonals[4,5],
				TestCellsNoDiagonals[5,4],
			};
			Assert.AreEqual(false, vicinity1.Any(x => !expectedCells1.Contains(x)), $"Found cells in vicinity1 that we didn't expect: {vicinity1.Where(x => !expectedCells1.Contains(x)).Select(x => x.Name).ListToString()}");
			Assert.AreEqual(false, expectedCells1.Any(x => !vicinity1.Contains(x)), $"Missing cells in vicinity1 that we expected: {expectedCells1.Where(x => !vicinity1.Contains(x)).Select(x => x.Name).ListToString()}");

			var vicinity2 = Person1Flight.CellsInVicinity(2, true, true);
			var expectedCells2 = new List<ICell> {
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
			var path = Person1Flight.ExitsBetween(Person5Flight, 15);
			Assert.AreEqual(false, path.Any(), "Expected ExitsBetween not to find a path");
		}

		[TestMethod]
		public void TestASharpVeryLong()
		{
			var path = Person1Flight.ExitsBetween(Person5Flight, 100);
			Assert.AreEqual(true, path.Any(), "Expected ExitsBetween to find a path");
		}
	}
}

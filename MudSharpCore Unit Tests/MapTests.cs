using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MapTests
{
        public static ICell[,] GetMapTestCells
        {
		get
		{
			var id = 457;
			var cellMap = new CellStub[9, 9];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					var cell = new CellStub()
					{
						Room = new RoomStub
						{
							X = i,
							Y = j,
							Z = 0
						}.ToMock()
					};
					cellMap[i, j] = cell;
					cell.Id = id++;
					cell.Name = $"Cell {i},{j}";
				}
			}
                
			// Changes for testing
			cellMap[6, 0] = null;
			cellMap[6, 1] = null;
			cellMap[7, 0] = null;
			cellMap[7, 1] = null;
			cellMap[8, 0] = null;
			cellMap[8, 1] = null;
			cellMap[8, 2] = null;
			// End changes

			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{

					if (cellMap[i, j] is null)
					{
						continue;
					}

					var exits = new List<CellExitStub>();
					if (i > 0)
					{
						if (cellMap[i - 1, j] is not null)
						{
							DoorStub door = null;
							if (i < 2)
							{
								door = new DoorStub
								{
									CanFireThrough = false,
									CanPlayersSmash = true,
									HingeCell = cellMap[i - 1, j],
									State = j % 2 == 0 ? DoorState.Closed : DoorState.Open,
									IsOpen = j % 2 != 0
								};
							}
							exits.Add(new CellExitStub()
							{
								Exit = new ExitStub() { Door = door, AcceptsDoor = door is not null },
								OutboundDirection = CardinalDirection.West,
								Destination = cellMap[i - 1, j]
							});

							cellMap[i - 1, j].Exits.Add(new CellExitStub()
							{
								Exit = new ExitStub() { Door = door, AcceptsDoor = door is not null },
								OutboundDirection = CardinalDirection.East,
								Destination = cellMap[i, j]
							});
						}
					}

					if (j > 0 && i > 1)
					{
						if (cellMap[i, j - 1] is not null)
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
								Destination = cellMap[i, j]
							});
						}
					}

					cellMap[i, j].Exits = exits;
				}
			}
                

			var returnMap = new ICell[9, 9];
			var cellMocks = cellMap.OfType<CellStub>().Select(x => x.ToMock()).ToList();
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					returnMap[i, j] = cellMap[i, j]?.GetObject(cellMocks);
				}
			}
                        return returnMap;
                }
        }

        [TestMethod]
        public void BasicExitLinking()
        {
                var map = GetMapTestCells;
                var cellA = map[2, 2];
                var cellB = map[3, 2];

                Assert.IsNotNull(cellA, "Cell 2,2 should exist");
                Assert.IsNotNull(cellB, "Cell 3,2 should exist");

                var exitA = cellA.ExitsFor(null, true).FirstOrDefault(x => x.OutboundDirection == CardinalDirection.East);
                Assert.IsNotNull(exitA, "Cell 2,2 should have an east exit");
                Assert.AreSame(cellB, exitA.Destination, "East exit from 2,2 should lead to 3,2");

                var exitB = cellB.ExitsFor(null, true).FirstOrDefault(x => x.OutboundDirection == CardinalDirection.West);
                Assert.IsNotNull(exitB, "Cell 3,2 should have a west exit");
                Assert.AreSame(cellA, exitB.Destination, "West exit from 3,2 should lead to 2,2");
        }

        [TestMethod]
        public void SimplePathFinding()
        {
                var map = GetMapTestCells;
                var source = new PerceivableStub { Location = map[2, 2] }.ToMock();
                var target = new PerceivableStub { Location = map[5, 2] }.ToMock();

                var path = source.PathBetween(target, 10, false).ToList();
                Assert.AreEqual(3, path.Count, $"Expected a path of length 3 but got {path.Count}. Path: {string.Join(", ", path.Select(x => x.OutboundDirection.DescribeBrief()))}");
        }
}
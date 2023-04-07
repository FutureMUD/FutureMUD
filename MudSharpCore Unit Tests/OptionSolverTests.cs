using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests
{
    [TestClass]
    public class OptionSolverTests
    {
        [TestMethod]
        public void TestOptionSolverSolutions() {
            var problemSet1 = new Dictionary<int, List<int>> {
                [1] = new List<int>(new[] {3, 5, 7}),
                [2] = new List<int>(new[] {2}),
                [3] = new List<int>(new[] {3, 5}),
                [4] = new List<int>(new[] {5})
            };

            var expectedSolution1 = new List<(int, int)> {
                (1, 7),
                (2, 2),
                (3, 3),
                (4, 5)
            };

            var problemSet2 = new Dictionary<int, List<int>> {
                [1] = new List<int>(new[] {2, 3}),
                [2] = new List<int>(new[] {2}),
                [3] = new List<int>(new[] {3}),
                [4] = new List<int>(new[] {1, 2})
            };

            var solver1 = new OptionSolver<int,int>(
                    from choice in problemSet1
                    select new Choice<int,int>(choice.Key, 
                                               from item in choice.Value
                                               select new Option<int>(item)
                    ) {
                        OptionScorer = option => 10.0 - option
                    }
                );

            var solution1 = solver1.SolveOptions();
            Assert.AreEqual(solution1.Success, true, "Expected the solver to find a solution to Problem Set 1");
            Assert.AreEqual(solution1.Solution.SequenceEqual(expectedSolution1), true, $"Expected the solver to find the solution 7, 2, 3, and 5 but the actual solution was {solution1.Solution.Select(x => x.Option.ToString()).ListToString()}");

            var solver2 = new OptionSolver<int, int>(
                from choice in problemSet2
                select new Choice<int, int>(choice.Key,
                                            from item in choice.Value
                                            select new Option<int>(item)
                ) {
                    OptionScorer = option => 10.0 - option
                }
            );

            var solution2 = solver2.SolveOptions();
            Assert.AreEqual(solution2.Success, false, "Expected the solver to not find a solution to Problem Set 2");
        }
    }
}

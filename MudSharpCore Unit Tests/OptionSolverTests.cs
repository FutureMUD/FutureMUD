using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace MudSharp_Unit_Tests;

[TestClass]
public class OptionSolverTests
{
    [TestMethod]
    public void TestOptionSolverSolutions()
    {
        Dictionary<int, List<int>> problemSet1 = new()
        {
            [1] = new List<int>(new[] { 3, 5, 7 }),
            [2] = new List<int>(new[] { 2 }),
            [3] = new List<int>(new[] { 3, 5 }),
            [4] = new List<int>(new[] { 5 })
        };

        List<(int, int)> expectedSolution1 = new()
        {
            (1, 7),
            (2, 2),
            (3, 3),
            (4, 5)
        };

        Dictionary<int, List<int>> problemSet2 = new()
        {
            [1] = new List<int>(new[] { 2, 3 }),
            [2] = new List<int>(new[] { 2 }),
            [3] = new List<int>(new[] { 3 }),
            [4] = new List<int>(new[] { 1, 2 })
        };

        OptionSolver<int, int> solver1 = new(
            from choice in problemSet1
            select new Choice<int, int>(choice.Key,
                from item in choice.Value
                select new Option<int>(item)
            )
            {
                OptionScorer = option => 10.0 - option
            }
        );

        (bool Success, List<(int Problem, int Option)> Solution, List<int> UnsolvableChoices) solution1 = solver1.SolveOptions();
        Assert.AreEqual(solution1.Success, true, "Expected the solver to find a solution to Problem Set 1");
        Assert.AreEqual(solution1.Solution.SequenceEqual(expectedSolution1), true, $"Expected the solver to find the solution 7, 2, 3, and 5 but the actual solution was {solution1.Solution.Select(x => x.Option.ToString()).ListToString()}");

        OptionSolver<int, int> solver2 = new(
            from choice in problemSet2
            select new Choice<int, int>(choice.Key,
                from item in choice.Value
                select new Option<int>(item)
            )
            {
                OptionScorer = option => 10.0 - option
            }
        );

        (bool Success, List<(int Problem, int Option)> Solution, List<int> UnsolvableChoices) solution2 = solver2.SolveOptions();
        Assert.AreEqual(solution2.Success, false, "Expected the solver to not find a solution to Problem Set 2");
    }
}
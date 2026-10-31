#nullable enable
using System;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonHerbBrewingTests
{
	[TestMethod]
	public void BrewingRecipes_PreserveOrderIndependenceWildcardsAndUnmatchedMixtures()
	{
		MudSharp.FutureProg.FutureProg.Initialise();
		var prog = new MudSharp.FutureProg.FutureProg(Mock.Of<IFuturemud>(), "BrewRecipeTest", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number, "a"), Tuple.Create(ProgVariableTypes.Number, "b"), Tuple.Create(ProgVariableTypes.Number, "c")],
			ArmageddonWorldfileWriter.BrewingRecipeProg());
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		foreach (var (a, b, c, expected) in new[]
		{
			(6,4,-1,1), (7,6,2,2), (8,7,1,3), (8,4,3,5), (5,3,1,6), (5,3,2,7),
			(5,1,-1,-1), (7,6,5,-2), (4,3,2,-3), (3,2,1,-5), (8,7,4,-7),
			(6,4,3,1), (5,1,0,-1), (6,4,5,0), (0,0,-1,0)
		})
		foreach (var order in new[] { new[] { a,b,c }, new[] { c,a,b }, new[] { b,c,a }, new[] { b,a,c }, new[] { c,b,a }, new[] { a,c,b } })
			Assert.AreEqual(expected, prog.ExecuteInt(new object[] { order[0], order[1], order[2] }), $"Ingredients {string.Join(',', order)}");
	}
}

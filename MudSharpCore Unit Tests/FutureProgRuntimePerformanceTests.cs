#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Discord;
using MudSharp.Framework;
using MudSharp.FutureProg;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgRuntimePerformanceTests
{
	private static IFuturemud _gameworld = null!;

	[ClassInitialize]
	public static void ClassInitialize(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		_gameworld = FutureProgTestBootstrap.Gameworld;
	}

	[TestMethod]
	public void FullyStatic_ValueReturnIsCachedAndCompileInvalidatesCache()
	{
		var prog = Compile(
			"StaticValueCache",
			ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number, "value")],
			"return @value");
		prog.StaticType = FutureProgStaticType.FullyStatic;

		Assert.AreEqual(11M, prog.ExecuteDecimal((object)11M));
		Assert.AreEqual(11M, prog.ExecuteDecimal((object)22M));

		prog.FunctionText = "return 99";
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		Assert.AreEqual(99M, prog.ExecuteDecimal());
	}

	[TestMethod]
	public void FullyStatic_NullReturnIsCached()
	{
		var prog = Compile(
			"StaticNullCache",
			ProgVariableTypes.Character,
			[Tuple.Create(ProgVariableTypes.Character, "character")],
			"return @character");
		prog.StaticType = FutureProgStaticType.FullyStatic;
		var character = new Mock<ICharacter>().Object;

		Assert.IsNull(prog.Execute((object?)null!));
		Assert.IsNull(prog.Execute(character));
	}

	[TestMethod]
	public void FullyStatic_ConcurrentFirstAccessPublishesOneCoherentResult()
	{
		var prog = Compile(
			"StaticConcurrentCache",
			ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number, "value")],
			"return @value");
		prog.StaticType = FutureProgStaticType.FullyStatic;
		var results = new ConcurrentBag<decimal>();

		Parallel.For(1, 65, value => results.Add(prog.ExecuteDecimal((object)(decimal)value)));

		Assert.AreEqual(64, results.Count);
		Assert.AreEqual(1, results.Distinct().Count());
		Assert.IsTrue(results.First() is >= 1M and <= 64M);
	}

	[TestMethod]
	public void StaticByParameters_RemainsUncached()
	{
		var prog = Compile(
			"StaticByParametersCompatibility",
			ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number, "value")],
			"return @value");
		prog.StaticType = FutureProgStaticType.StaticByParameters;

		Assert.AreEqual(11M, prog.ExecuteDecimal((object)11M));
		Assert.AreEqual(22M, prog.ExecuteDecimal((object)22M));
	}

	[TestMethod]
	public void RecursionProtection_SequentialCallsDoNotAccumulateDepth()
	{
		var prog = Compile("SequentialRecursionDepth", ProgVariableTypes.Number, [], "return 42");

		for (var i = 0; i < 500; i++)
		{
			Assert.AreEqual(42M, prog.ExecuteWithRecursionProtection());
		}
	}

	[TestMethod]
	public void RecursionProtection_GenuinelyRecursiveCallsHitActiveDepthLimitAndUnwind()
	{
		var progs = new List<IFutureProg>();
		var progRepository = new Mock<IUneditableAll<IFutureProg>>();
		progRepository.Setup(x => x.GetEnumerator()).Returns(() => progs.GetEnumerator());
		progRepository.SetupGet(x => x.Count).Returns(() => progs.Count);
		var discord = new Mock<IDiscordConnection>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progRepository.Object);
		gameworld.SetupGet(x => x.DiscordConnection).Returns(discord.Object);

		var first = new FutureProg(gameworld.Object, "RecursiveFirst", ProgVariableTypes.Number, [],
			"return @RecursiveSecond()");
		var second = new FutureProg(gameworld.Object, "RecursiveSecond", ProgVariableTypes.Number, [],
			"return @RecursiveFirst()");
		progs.Add(first);
		progs.Add(second);
		Assert.IsTrue(first.Compile(), first.CompileError);
		Assert.IsTrue(second.Compile(), second.CompileError);

		Assert.AreEqual(0M, first.ExecuteWithRecursionProtection());
		discord.Verify(x => x.NotifyProgError(It.IsAny<long>(), It.IsAny<string>(),
			It.Is<string>(message => message.Contains("excessive recursion"))), Times.Once);
		Assert.AreEqual(42M, Compile("DepthWasUnwound", ProgVariableTypes.Number, [], "return 42")
			.ExecuteWithRecursionProtection());
	}

	[TestMethod]
	public void MixedCaseParametersLocalsAndDotReferencesRemainCaseInsensitive()
	{
		var prog = Compile(
			"MixedCaseRuntime",
			ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Text, "InputText")],
			"""
			var LocalValue = @inputTEXT.UPPER.length
			return @localvalue
			""");

		Assert.AreEqual(9M, prog.ExecuteDecimal("FutureMUD"));
	}

	[TestMethod]
	public void ReusedLoopScopeClearsIterationLocals()
	{
		var prog = Compile(
			"LoopScopeReset",
			ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number | ProgVariableTypes.Collection, "values")],
			"""
			var total as number
			foreach (value in @values)
				var doubled = @value * 2
				total += @doubled
			end foreach
			return @total
			""");

		Assert.AreEqual(12M, prog.ExecuteDecimal(new List<decimal> { 1M, 2M, 3M }));
	}

	[TestMethod]
	public void ShortCircuitLogicDoesNotEvaluateUnneededBranch()
	{
		var falseAnd = Compile("ShortCircuitAnd", ProgVariableTypes.Boolean, [],
			"return false and (1 / 0 > 0)");
		var trueOr = Compile("ShortCircuitOr", ProgVariableTypes.Boolean, [],
			"return true or (1 / 0 > 0)");

		Assert.IsFalse(falseAnd.ExecuteBool());
		Assert.IsTrue(trueOr.ExecuteBool());
	}

	[TestMethod]
	public void DictionaryReturnAdapterUnwrapsPrimitiveProgVariables()
	{
		var prog = Compile(
			"DictionaryAdapter",
			ProgVariableTypes.Text | ProgVariableTypes.Dictionary,
			[Tuple.Create(ProgVariableTypes.Text | ProgVariableTypes.Dictionary, "values")],
			"return @values");
		var input = new Dictionary<string, string> { ["first"] = "alpha", ["second"] = "beta" };

		var result = prog.ExecuteDictionary<string>(input);

		Assert.AreEqual("alpha", result["first"]);
		Assert.AreEqual("beta", result["second"]);
	}

	[TestMethod]
	public void CollectionDictionaryReturnAdapterUnwrapsPrimitiveProgVariables()
	{
		var prog = Compile(
			"CollectionDictionaryAdapter",
			ProgVariableTypes.Text | ProgVariableTypes.CollectionDictionary,
			[Tuple.Create(ProgVariableTypes.Text | ProgVariableTypes.CollectionDictionary, "values")],
			"return @values");
		var input = new CollectionDictionary<string, string>();
		input.Add("first", "alpha");
		input.Add("first", "beta");

		var result = prog.ExecuteCollectionDictionary<string>(input);

		CollectionAssert.AreEqual(new[] { "alpha", "beta" }, result["first"].ToArray());
	}

	private static FutureProg Compile(string name, ProgVariableTypes returnType,
		IEnumerable<Tuple<ProgVariableTypes, string>> parameters, string source)
	{
		var prog = new FutureProg(_gameworld, name, returnType, parameters, source);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		return prog;
	}
}

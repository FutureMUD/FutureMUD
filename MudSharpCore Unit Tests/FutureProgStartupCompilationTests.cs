#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgStartupCompilationTests
{
	[TestInitialize]
	public void Initialise() => FutureProgTestBootstrap.EnsureInitialised();

	[TestMethod]
	public void Batch_CompilesForwardReferencesAndCaseInsensitiveOverloadsWithoutRepeatedEnumeration()
	{
		var progs = new List<IFutureProg>();
		var enumerations = 0;
		var repository = new Mock<IUneditableAll<IFutureProg>>();
		repository.Setup(x => x.GetEnumerator()).Returns(() => { enumerations++; return progs.GetEnumerator(); });
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(repository.Object);
		var number = new FutureProg(world.Object, "choice", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Number, "n")], "return @n");
		var text = new FutureProg(world.Object, "CHOICE", ProgVariableTypes.Number,
			[Tuple.Create(ProgVariableTypes.Text, "t")], "return 42");
		var caller = new FutureProg(world.Object, "caller", ProgVariableTypes.Number, [], "return @Choice(\"text\")");
		progs.AddRange([caller, number, text]);
		using (new FutureProgCompilationBatch(world.Object, progs))
		{
			Assert.IsTrue(caller.Compile(), caller.CompileError);
			Assert.IsTrue(number.Compile(), number.CompileError);
			Assert.IsTrue(text.Compile(), text.CompileError);
			Assert.AreEqual(42M, caller.ExecuteDecimal());
			Assert.AreSame(number, FutureProgCompilationBatch.Find(world.Object, "choice", [ProgVariableTypes.Number]));
		}

		Assert.AreEqual(0, enumerations, "Indexed resolution must not enumerate the world's entire prog repository.");
	}

	[TestMethod]
	public void Batch_PreservesFirstCompatibleAnyParameterOverloadAndDisposesAfterFailure()
	{
		var progs = new List<IFutureProg>();
		var repository = new Mock<IUneditableAll<IFutureProg>>();
		repository.Setup(x => x.GetEnumerator()).Returns(() => progs.GetEnumerator());
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(repository.Object);
		var first = new FutureProg(world.Object, "test", ProgVariableTypes.Number, [], "return 1") { AcceptsAnyParameters = true };
		var second = new FutureProg(world.Object, "test", ProgVariableTypes.Number, [], "return 2");
		progs.AddRange([first, second]);
		Assert.ThrowsExactly<InvalidOperationException>(() =>
		{
			using var batch = new FutureProgCompilationBatch(world.Object, progs);
			Assert.AreSame(first, FutureProgCompilationBatch.Find(world.Object, "TEST", []));
			Assert.AreSame(first, FutureProgCompilationBatch.Find(world.Object, "test", [ProgVariableTypes.Text]));
			throw new InvalidOperationException("Abort compilation batch");
		});
		first.FunctionName = "renamed";
		progs.Remove(second);
		Assert.IsNull(FutureProgCompilationBatch.Find(world.Object, "test", []));
		Assert.AreSame(first, FutureProgCompilationBatch.Find(world.Object, "renamed", []));
	}

	[TestMethod]
	public void ColourisedSource_IsLazyAndInvalidatesForEditsAndDarkMode()
	{
		var dark = false;
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticBool("DisplayProgsInDarkMode")).Returns(() => dark);
		var prog = new FutureProg(world.Object, "colour", ProgVariableTypes.Number, [], "return 77");
		world.Verify(x => x.GetStaticBool("DisplayProgsInDarkMode"), Times.Never);
		var light = prog.ColourisedFunctionText;
		Assert.AreSame(light, prog.ColourisedFunctionText);
		dark = true;
		Assert.AreNotEqual(light, prog.ColourisedFunctionText);
		prog.FunctionText = "return 88";
		StringAssert.Contains(prog.ColourisedFunctionText, "88");
		prog.FunctionText = "";
		Assert.AreEqual("", prog.ColourisedFunctionText);
	}
}

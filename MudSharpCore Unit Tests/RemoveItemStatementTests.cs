#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RemoveItemStatementTests
{
	[TestMethod]
	[DataRow("values", "number", "1", "1")]
	[DataRow("@values", "number", "1", "1")]
	[DataRow("values", "text", "\"Parcel\"", "\"parcel\"")]
	[DataRow("values", "boolean", "true", "true")]
	public void RemoveItem_RemovesFirstMatchingValueAndPreservesDuplicate(string variable, string type, string value, string target)
	{
		var prog = Compile($"var values as {type} collection\nadditem values {value}\nadditem values {value}\nremoveitem {variable} {target}\nreturn collectioncount(@values)");
		Assert.AreEqual(1m, prog.ExecuteDecimal());
	}

	[TestMethod]
	public void RemoveItem_AbsentValueLeavesCollectionUnchanged()
	{
		var prog = Compile("var values as number collection\nadditem values 1\nremoveitem values 2\nreturn collectioncount(@values)");
		Assert.AreEqual(1m, prog.ExecuteDecimal());
	}

	[TestMethod]
	public void RemoveItem_IncompatibleTypeIsRejected()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "remove_invalid", ProgVariableTypes.Number, [],
			"var values as number collection\nremoveitem values \"parcel\"\nreturn 0");
		Assert.IsFalse(prog.Compile());
		StringAssert.Contains(prog.CompileError, "not of the same type");
	}

	private static FutureProg Compile(string source)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "remove_item", ProgVariableTypes.Number, [], source);
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		return prog;
	}
}

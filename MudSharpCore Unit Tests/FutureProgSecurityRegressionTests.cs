#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Communication.Language;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgSecurityRegressionTests
{
	[TestMethod]
	public void DescribeProgVariable_TextCollectionOfProgVariables_DoesNotThrow()
	{
		Mock<ICharacter> actor = new();
		List<IProgVariable> values = [new TextVariable("north"), new TextVariable("east")];

		string result = ProgModule.DescribeProgVariable(actor.Object,
			ProgVariableTypes.Text | ProgVariableTypes.Collection, values);

		StringAssert.Contains(result, "north");
		StringAssert.Contains(result, "east");
	}

	[TestMethod]
	public void WritingTextProperty_DoesNotExposeStoredText()
	{
		SimpleWriting simple = TestObjectFactory.CreateUninitialized<SimpleWriting>();
		PrintedWriting printed = TestObjectFactory.CreateUninitialized<PrintedWriting>();
		CompositeWriting composite = TestObjectFactory.CreateUninitialized<CompositeWriting>();

		Assert.AreEqual(string.Empty, simple.GetProperty("text").GetObject);
		Assert.AreEqual(string.Empty, printed.GetProperty("text").GetObject);
		Assert.AreEqual(string.Empty, composite.GetProperty("text").GetObject);
	}
}

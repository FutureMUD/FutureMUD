#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ConditionalInputHookTests
{
	private static HookOnInput Hook(Mock<IFutureProg> prog, bool? conditional)
	{
		prog.SetupGet(x => x.Id).Returns(1); prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		var progs = new All<IFutureProg>(); progs.Add(prog.Object);
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.FutureProgs).Returns(progs);
		return new HookOnInput(new MudSharp.Models.Hooks { Id = 1, Type = "HookOnInput", Name = "Input",
			TargetEventType = (int)EventType.CommandInput,
			Definition = new XElement("Definition", new XElement("TargetCommand", "ask"), new XElement("FutureProg", 1),
				conditional.HasValue ? new XElement("UseProgResultForHandling", conditional.Value) : null).ToString() }, world.Object);
	}

	[TestMethod]
	public void LegacyInput_MissingOptInStillConsumesAndExecutesRegardlessOfResult()
	{
		var prog = new Mock<IFutureProg>(); var hook = Hook(prog, null);
		Assert.IsFalse(hook.UseProgResultForHandling);
		Assert.IsTrue(hook.Function(EventType.CommandInput, [Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), "ask", new StringStack("guard help")]));
		prog.Verify(x => x.Execute(It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void DecliningInput_PreservesOriginalTokensAndQuotedBoundariesForNormalDispatch()
	{
		var prog = new Mock<IFutureProg>(); var hook = Hook(prog, true);
		var input = new StringStack("\"other guard\" help"); string[]? args = null;
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback<object[]>(values => args = ((IEnumerable<string>)values[3]).ToArray()).Returns(false);
		Assert.IsFalse(hook.Function(EventType.CommandInput, [Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), "ask", input]));
		Assert.AreEqual("\"other guard\" help", input.RemainingArgument);
		Assert.IsFalse(input.Memory.Any());
		CollectionAssert.AreEqual(new[] { "other guard", "help" }, args);
	}

	[TestMethod]
	public void HandledInput_StopsAtFirstTrueAndRoundTripsTheOptIn()
	{
		var prog = new Mock<IFutureProg>(); var hook = Hook(prog, true);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(true);
		var second = new Mock<IFutureProg>(); second.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean); hook.AddProg(second.Object);
		Assert.IsTrue(hook.Function(EventType.CommandInput, [Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), "ask", new StringStack("guard help")]));
		second.Verify(x => x.ExecuteBool(It.IsAny<object[]>()), Times.Never);
		Assert.IsFalse(hook.Function(EventType.CommandInput, [Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), "look", new StringStack("")]));
		var xml = (XElement)typeof(HookOnInput).GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(hook, null)!;
		Assert.IsTrue((bool)xml.Element("UseProgResultForHandling")!);
	}

	[TestMethod]
	public void ConditionalWildcard_PassesOriginalCommandAndQuotesAndDeclinesWithoutConsuming()
	{
		var prog = new Mock<IFutureProg>(); var hook = Hook(prog, true); hook.TargetCommand = "*";
		string? raw = null;
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback<object[]>(values => raw = (string)values[2]).Returns(false);
		var input = new StringStack("\"(nods)\" kral stone on self via Earth");
		Assert.IsFalse(hook.Function(EventType.CommandInput, [Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), "whis", input]));
		Assert.AreEqual("whis \"(nods)\" kral stone on self via Earth", raw);
		Assert.AreEqual("\"(nods)\" kral stone on self via Earth", input.RemainingArgument);
		hook.UseProgResultForHandling = false;
		Assert.IsFalse(hook.Function(EventType.CommandInput, [Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), "whis", input]), "Legacy exact hooks do not gain wildcard matching.");
	}
}

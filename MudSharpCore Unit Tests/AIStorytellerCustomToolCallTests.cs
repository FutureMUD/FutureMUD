using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.FutureProg;
using MudSharp.RPG.AIStorytellers;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerCustomToolCallTests
{
	[TestMethod]
	public void IsValid_WhenProgHasCompileError_ReturnsFalse()
	{
		var namedParameters = new List<Tuple<ProgVariableTypes, string>>
		{
			Tuple.Create(ProgVariableTypes.Text, "Cue")
		};
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters).Returns(namedParameters);
		prog.SetupGet(x => x.CompileError).Returns("Compile failed");

		var tool = new AIStorytellerCustomToolCall("RecordCue", "Records narrative cues", prog.Object);

		Assert.IsFalse(tool.IsValid);
	}

	[TestMethod]
	public void RefreshParameterDescriptions_WhenProgChanges_AddsMissingEntries()
	{
		var namedParameters = new List<Tuple<ProgVariableTypes, string>>
		{
			Tuple.Create(ProgVariableTypes.Text, "Cue")
		};
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters).Returns(namedParameters);
		prog.SetupGet(x => x.CompileError).Returns(string.Empty);

		var tool = new AIStorytellerCustomToolCall("RecordCue", "Records narrative cues", prog.Object);
		namedParameters.Add(Tuple.Create(ProgVariableTypes.Number, "Priority"));
		tool.RefreshParameterDescriptions();

		Assert.IsTrue(tool.ParameterDescriptions.ContainsKey("Cue"));
		Assert.IsTrue(tool.ParameterDescriptions.ContainsKey("Priority"));
	}

	[TestMethod]
	public void SetParameterDescription_UpdatesExistingValue()
	{
		var namedParameters = new List<Tuple<ProgVariableTypes, string>>
		{
			Tuple.Create(ProgVariableTypes.Text, "Cue")
		};
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters).Returns(namedParameters);
		prog.SetupGet(x => x.CompileError).Returns(string.Empty);

		var tool = new AIStorytellerCustomToolCall("RecordCue", "Records narrative cues", prog.Object);
		tool.SetParameterDescription("Cue", "A concise narrative cue.");

		Assert.AreEqual("A concise narrative cue.", tool.ParameterDescriptions["Cue"]);
	}
}

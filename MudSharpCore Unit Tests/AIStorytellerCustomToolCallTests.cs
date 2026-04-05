using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.FutureProg;
using MudSharp.RPG.AIStorytellers;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerCustomToolCallTests
{
    [TestMethod]
    public void IsValid_WhenProgHasCompileError_ReturnsFalse()
    {
        List<Tuple<ProgVariableTypes, string>> namedParameters = new()
        {
            Tuple.Create(ProgVariableTypes.Text, "Cue")
        };
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters).Returns(namedParameters);
        prog.SetupGet(x => x.CompileError).Returns("Compile failed");

        AIStorytellerCustomToolCall tool = new("RecordCue", "Records narrative cues", prog.Object);

        Assert.IsFalse(tool.IsValid);
    }

    [TestMethod]
    public void RefreshParameterDescriptions_WhenProgChanges_AddsMissingEntries()
    {
        List<Tuple<ProgVariableTypes, string>> namedParameters = new()
        {
            Tuple.Create(ProgVariableTypes.Text, "Cue")
        };
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters).Returns(namedParameters);
        prog.SetupGet(x => x.CompileError).Returns(string.Empty);

        AIStorytellerCustomToolCall tool = new("RecordCue", "Records narrative cues", prog.Object);
        namedParameters.Add(Tuple.Create(ProgVariableTypes.Number, "Priority"));
        tool.RefreshParameterDescriptions();

        Assert.IsTrue(tool.ParameterDescriptions.ContainsKey("Cue"));
        Assert.IsTrue(tool.ParameterDescriptions.ContainsKey("Priority"));
    }

    [TestMethod]
    public void SetParameterDescription_UpdatesExistingValue()
    {
        List<Tuple<ProgVariableTypes, string>> namedParameters = new()
        {
            Tuple.Create(ProgVariableTypes.Text, "Cue")
        };
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters).Returns(namedParameters);
        prog.SetupGet(x => x.CompileError).Returns(string.Empty);

        AIStorytellerCustomToolCall tool = new("RecordCue", "Records narrative cues", prog.Object);
        tool.SetParameterDescription("Cue", "A concise narrative cue.");

        Assert.AreEqual("A concise narrative cue.", tool.ParameterDescriptions["Cue"]);
    }
}

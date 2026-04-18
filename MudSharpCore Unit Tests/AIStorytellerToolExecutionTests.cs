#nullable enable
#pragma warning disable OPENAI001
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Communication.Language;
using MudSharp.Community;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.AIStorytellers;
using MudSharp.RPG.Knowledge;
using MudSharp.RPG.Merits;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using ModelStoryteller = MudSharp.Models.AIStoryteller;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerToolExecutionTests
{
    [TestMethod]
    public void Constructor_ScopedModelAndReasoning_LoadsScopedValues()
    {
        AIStoryteller storyteller = CreateStoryteller();

        Assert.AreEqual("gpt-5", storyteller.Model);
        Assert.AreEqual("gpt-5-mini", storyteller.TimeModel);
        Assert.AreEqual("gpt-5-nano", storyteller.AttentionClassifierModel);
        Assert.AreEqual(ResponseReasoningEffortLevel.Medium, storyteller.ReasoningEffort);
        Assert.AreEqual(ResponseReasoningEffortLevel.Low, storyteller.TimeReasoningEffort);
        Assert.AreEqual(ResponseReasoningEffortLevel.Minimal, storyteller.AttentionClassifierReasoningEffort);
    }

    [TestMethod]
    public void Constructor_MissingScopedModelAndReasoning_FallsBackToEventDefaults()
    {
        ModelStoryteller model = CreateModel();
        model.TimeModel = null!;
        model.AttentionClassifierModel = null!;
        model.TimeReasoningEffort = null!;
        model.AttentionClassifierReasoningEffort = null!;
        Mock<IFuturemud> gameworld = CreateGameworld(Array.Empty<IFutureProg>(), Array.Empty<ICharacter>());
        AIStoryteller storyteller = new(model, gameworld.Object);

        Assert.AreEqual(storyteller.Model, storyteller.TimeModel);
        Assert.AreEqual(storyteller.Model, storyteller.AttentionClassifierModel);
        Assert.AreEqual(storyteller.ReasoningEffort, storyteller.TimeReasoningEffort);
        Assert.AreEqual(ResponseReasoningEffortLevel.Low, storyteller.AttentionClassifierReasoningEffort);
    }

    [TestMethod]
    public void Constructor_SpeechContextSettings_MissingValues_UsesDefaults()
    {
        AIStoryteller storyteller = CreateStoryteller();

        Assert.AreEqual(0, storyteller.SpeechContextEventCount);
        Assert.AreEqual(TimeSpan.FromMinutes(10), storyteller.SpeechContextMaximumSeparation);
    }

    [TestMethod]
    public void Constructor_SpeechContextSettings_LoadsConfiguredValues()
    {
        ModelStoryteller model = CreateModel();
        model.CustomToolCallsDefinition =
            """
			<ToolCalls>
			  <SpeechContextEventCount>5</SpeechContextEventCount>
			  <SpeechContextMaximumSeparationMilliseconds>180000</SpeechContextMaximumSeparationMilliseconds>
			</ToolCalls>
			""";
        Mock<IFuturemud> gameworld = CreateGameworld(Array.Empty<IFutureProg>(), Array.Empty<ICharacter>());
        AIStoryteller storyteller = new(model, gameworld.Object);

        Assert.AreEqual(5, storyteller.SpeechContextEventCount);
        Assert.AreEqual(TimeSpan.FromMinutes(3), storyteller.SpeechContextMaximumSeparation);
    }

    [TestMethod]
    public void NormalizeFunctionToolSchema_NullSchema_ReturnsClosedEmptyObjectSchema()
    {
        string normalized = AIStoryteller.NormalizeFunctionToolSchema(null!);
        JsonElement payload = JsonDocument.Parse(normalized).RootElement;

        Assert.AreEqual("object", payload.GetProperty("type").GetString());
        Assert.IsFalse(payload.GetProperty("additionalProperties").GetBoolean());
        Assert.AreEqual(0, payload.GetProperty("properties").EnumerateObject().Count());
        Assert.AreEqual(0, payload.GetProperty("required").GetArrayLength());
    }

    [TestMethod]
    public void NormalizeFunctionToolSchema_ObjectSchemasAreClosedForStrictMode()
    {
        string normalized = AIStoryteller.NormalizeFunctionToolSchema(
            """
			{
			  "type": "object",
			  "properties": {
			    "Title": {
			      "type": "string"
			    },
			    "Effect": {
			      "type": "object",
			      "properties": {
			        "EffectId": {
			          "type": "integer"
			        }
			      },
			      "required": ["EffectId"],
			      "additionalProperties": true
			    }
			  },
			  "required": ["Title", "Effect"]
			}
			""");

        JsonElement payload = JsonDocument.Parse(normalized).RootElement;
        JsonElement effect = payload.GetProperty("properties").GetProperty("Effect");

        Assert.IsFalse(payload.GetProperty("additionalProperties").GetBoolean());
        Assert.IsFalse(effect.GetProperty("additionalProperties").GetBoolean());
    }

    [TestMethod]
    public void NormalizeFunctionToolSchema_OptionalProperty_BecomesNullableAndRequired()
    {
        string normalized = AIStoryteller.NormalizeFunctionToolSchema(
            """
			{
			  "type": "object",
			  "properties": {
			    "Query": {
			      "type": "string"
			    }
			  },
			  "required": []
			}
			""");

        JsonElement payload = JsonDocument.Parse(normalized).RootElement;
        List<string?> required = payload.GetProperty("required").EnumerateArray().Select(x => x.GetString()).ToList();
        List<string?> queryTypes = payload.GetProperty("properties")
            .GetProperty("Query")
            .GetProperty("type")
            .EnumerateArray()
            .Select(x => x.GetString())
            .ToList();

        Assert.AreEqual(1, required.Count);
        Assert.AreEqual("Query", required[0]);
        CollectionAssert.Contains(queryTypes, "string");
        CollectionAssert.Contains(queryTypes, "null");
    }

    [TestMethod]
    public void ConfigureToolLoopResponseOptions_RequiredChoice_SetsToolChoiceAndTokenBudget()
    {
        AIStoryteller storyteller = CreateStoryteller();
        CreateResponseOptions options = new(new List<ResponseItem>())
        {
            Instructions = "Sentinel instructions"
        };

        storyteller.ConfigureToolLoopResponseOptions(options, includeEchoTools: false, requireToolCall: true,
            toolProfile: AIStoryteller.StorytellerToolProfile.Full);

        Assert.IsNotNull(options.ToolChoice);
        Assert.AreEqual(ResponseToolChoiceKind.Required, options.ToolChoice.Kind);
        Assert.IsTrue(options.ParallelToolCallsEnabled);
        Assert.AreEqual(1200, options.MaxOutputTokenCount);
        Assert.AreEqual("Sentinel instructions", options.Instructions);
        Assert.AreEqual(storyteller.ReasoningEffort, options.ReasoningOptions?.ReasoningEffortLevel);
        Assert.IsTrue(options.Tools.Count > 0);
    }

    [TestMethod]
    public void ConfigureToolLoopResponseOptions_AutoChoiceWhenNotRequired_SetsAutoChoice()
    {
        AIStoryteller storyteller = CreateStoryteller();
        CreateResponseOptions options = new(new List<ResponseItem>());

        storyteller.ConfigureToolLoopResponseOptions(options, includeEchoTools: false, requireToolCall: false,
            toolProfile: AIStoryteller.StorytellerToolProfile.Full);

        Assert.IsNotNull(options.ToolChoice);
        Assert.AreEqual(ResponseToolChoiceKind.Auto, options.ToolChoice.Kind);
    }

    [TestMethod]
    public void ConfigureToolLoopResponseOptions_EventFocusedProfile_ExcludesHeavyWorldTools()
    {
        AIStoryteller storyteller = CreateStoryteller();
        CreateResponseOptions options = new(new List<ResponseItem>());

        storyteller.ConfigureToolLoopResponseOptions(options, includeEchoTools: false, requireToolCall: true,
            toolProfile: AIStoryteller.StorytellerToolProfile.EventFocused);

        List<string> toolNames = options.Tools
            .OfType<FunctionTool>()
            .Select(x => x.FunctionName)
            .ToList();
        CollectionAssert.Contains(toolNames, "Noop");
        CollectionAssert.Contains(toolNames, "CreateSituation");
        CollectionAssert.Contains(toolNames, "ResolveSituation");
        CollectionAssert.Contains(toolNames, "CurrentDateTime");
        CollectionAssert.Contains(toolNames, "DateTimeForTarget");
        CollectionAssert.DoesNotContain(toolNames, "PathBetweenRooms");
        CollectionAssert.DoesNotContain(toolNames, "Landmarks");
        CollectionAssert.DoesNotContain(toolNames, "CharacterPlans");
        CollectionAssert.DoesNotContain(toolNames, "CalendarDefinition");
    }

    [TestMethod]
    public void ExecuteFunctionCall_UpdateSituation_WithCharacterScope_InvokesScopeUpdate()
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(777L);
        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object]);
        Mock<IAIStorytellerSituation> situation = new();
        situation.SetupGet(x => x.Id).Returns(28L);
        situation.SetupGet(x => x.Name).Returns("Scope test");
        situation.SetupGet(x => x.SituationText).Returns("Scope details");
        situation.SetupGet(x => x.IsResolved).Returns(false);
        situation.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
        storyteller.RegisterLoadedSituation(situation.Object);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("UpdateSituation",
            """{"Id":28,"Title":"Scope updated","Description":"Scoped details","CharacterId":777}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        situation.Verify(x => x.UpdateSituation("Scope updated", "Scoped details"), Times.Once);
        situation.Verify(x => x.SetScope(777L, null), Times.Once);
    }

    [TestMethod]
    public void ExecuteFunctionCall_CreateSituation_WithCharacterAndRoomScope_ReturnsError()
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(1L);
        Mock<ICell> room = new();
        room.SetupGet(x => x.Id).Returns(2L);
        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object], cells: [room.Object]);
        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CreateSituation",
            """{"Title":"Ambush","Description":"Bandits gather at dusk.","CharacterId":1,"RoomId":2}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
        StringAssert.Contains(payload.GetProperty("error").GetString(), "Specify either CharacterId or RoomId");
    }

    [TestMethod]
    public void ExecuteFunctionCall_UpdateSituation_WithNullRoomId_UsesCharacterScope()
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(1L);
        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object]);
        Mock<IAIStorytellerSituation> situation = new();
        situation.SetupGet(x => x.Id).Returns(29L);
        situation.SetupGet(x => x.Name).Returns("Scope test");
        situation.SetupGet(x => x.SituationText).Returns("Scope details");
        situation.SetupGet(x => x.IsResolved).Returns(false);
        situation.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
        storyteller.RegisterLoadedSituation(situation.Object);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("UpdateSituation",
            """{"Id":29,"Title":"Scope updated","Description":"Scoped details","CharacterId":1,"RoomId":null}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        situation.Verify(x => x.UpdateSituation("Scope updated", "Scoped details"), Times.Once);
        situation.Verify(x => x.SetScope(1L, null), Times.Once);
    }

    [TestMethod]
    public void ExecuteFunctionCall_CreateSituation_WithZeroCharacterId_ReturnsValidationError()
    {
        AIStoryteller storyteller = CreateStoryteller();
        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CreateSituation",
            """{"Title":"Ambush","Description":"Bandits gather at dusk.","CharacterId":0}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
        StringAssert.Contains(payload.GetProperty("error").GetString(), "CharacterId must be a positive integer");
    }

    [TestMethod]
    public void IsNoopOnlyFunctionCallBatch_DetectsOnlyNoop()
    {
        Assert.IsTrue(AIStoryteller.IsNoopOnlyFunctionCallBatch(["Noop"]));
        Assert.IsTrue(AIStoryteller.IsNoopOnlyFunctionCallBatch(["Noop", "noop"]));
        Assert.IsFalse(AIStoryteller.IsNoopOnlyFunctionCallBatch(Array.Empty<string>()));
        Assert.IsFalse(AIStoryteller.IsNoopOnlyFunctionCallBatch(["Noop", "CreateSituation"]));
    }

    [TestMethod]
    public void ExecuteFunctionCall_MalformedJson_ReturnsMalformedError()
    {
        AIStoryteller storyteller = CreateStoryteller();

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("Noop", "{", includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(result.MalformedJson);
        Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
        StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty, "Malformed tool-call JSON");
    }

    [TestMethod]
    public void ProcessFunctionCallBatch_MalformedJson_AddsRetryFeedback()
    {
        AIStoryteller storyteller = CreateStoryteller();
        List<ResponseItem> messages = new();

        (bool Continue, int MalformedRetries) result = storyteller.ProcessFunctionCallBatch(
            [("call-1", "Noop", "{")],
            includeEchoTools: false,
            messages,
            malformedRetries: 0);

        Assert.IsTrue(result.Continue);
        Assert.AreEqual(1, result.MalformedRetries);
        Assert.AreEqual(2, messages.Count);
    }

    [TestMethod]
    public void ProcessFunctionCallBatch_OverRetryBudget_LogsAndStops()
    {
        AIStoryteller storyteller = CreateStoryteller();
        string? loggedMessage = null;
        storyteller.ErrorLoggerOverride = message => loggedMessage = message;

        int retries = 0;
        List<ResponseItem> messages = new();
        (bool Continue, int MalformedRetries) result = (Continue: true, MalformedRetries: 0);
        for (int i = 0; i < 10; i++)
        {
            result = storyteller.ProcessFunctionCallBatch(
                [("call-1", "Noop", "{")],
                includeEchoTools: false,
                messages,
                retries);
            retries = result.MalformedRetries;
            if (!result.Continue)
            {
                break;
            }
        }

        Assert.IsFalse(result.Continue);
        Assert.IsFalse(string.IsNullOrWhiteSpace(loggedMessage));
        StringAssert.Contains(loggedMessage, "Malformed tool-call JSON retry budget exceeded.");
    }

    [TestMethod]
    public void ExecuteFunctionCall_UpdateSituation_InvokesUpdate()
    {
        AIStoryteller storyteller = CreateStoryteller();
        string title = "Old title";
        string details = "Old details";
        Mock<IAIStorytellerSituation> situation = new();
        situation.SetupGet(x => x.Id).Returns(25L);
        situation.SetupGet(x => x.Name).Returns(() => title);
        situation.SetupGet(x => x.SituationText).Returns(() => details);
        situation.SetupGet(x => x.IsResolved).Returns(false);
        situation.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
        situation.Setup(x => x.UpdateSituation(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((newTitle, newDetails) =>
            {
                title = newTitle;
                details = newDetails;
            });
        storyteller.RegisterLoadedSituation(situation.Object);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("UpdateSituation",
            """{"Id":25,"Title":"New title","Description":"Updated details"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(result.MalformedJson);
        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        situation.Verify(x => x.UpdateSituation("New title", "Updated details"), Times.Once);
    }

    [TestMethod]
    public void ExecuteFunctionCall_UpdateSituation_WithRoomScope_InvokesScopeUpdate()
    {
        Mock<ICell> room = new();
        room.SetupGet(x => x.Id).Returns(401L);

        AIStoryteller storyteller = CreateStoryteller(cells: [room.Object]);
        Mock<IAIStorytellerSituation> situation = new();
        situation.SetupGet(x => x.Id).Returns(27L);
        situation.SetupGet(x => x.Name).Returns("Scope test");
        situation.SetupGet(x => x.SituationText).Returns("Scope details");
        situation.SetupGet(x => x.IsResolved).Returns(false);
        situation.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
        storyteller.RegisterLoadedSituation(situation.Object);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("UpdateSituation",
            """{"Id":27,"Title":"Scope updated","Description":"Scoped details","RoomId":401}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(result.MalformedJson);
        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        situation.Verify(x => x.UpdateSituation("Scope updated", "Scoped details"), Times.Once);
        situation.Verify(x => x.SetScope(null, 401L), Times.Once);
    }

    [TestMethod]
    public void SituationMatchesTriggerScope_AppliesCharacterAndRoomScopeRules()
    {
        Mock<IAIStorytellerSituation> universal = new();
        universal.SetupGet(x => x.ScopeCharacterId).Returns((long?)null);
        universal.SetupGet(x => x.ScopeRoomId).Returns((long?)null);

        Mock<IAIStorytellerSituation> characterScoped = new();
        characterScoped.SetupGet(x => x.ScopeCharacterId).Returns(77L);
        characterScoped.SetupGet(x => x.ScopeRoomId).Returns((long?)null);

        Mock<IAIStorytellerSituation> roomScoped = new();
        roomScoped.SetupGet(x => x.ScopeCharacterId).Returns((long?)null);
        roomScoped.SetupGet(x => x.ScopeRoomId).Returns(88L);

        Assert.IsTrue(AIStoryteller.SituationMatchesTriggerScope(universal.Object, 1L, new List<long> { 2L }));
        Assert.IsTrue(AIStoryteller.SituationMatchesTriggerScope(characterScoped.Object, 1L, new List<long> { 77L }));
        Assert.IsFalse(AIStoryteller.SituationMatchesTriggerScope(characterScoped.Object, 1L, new List<long> { 2L }));
        Assert.IsTrue(AIStoryteller.SituationMatchesTriggerScope(roomScoped.Object, 88L, new List<long> { 2L }));
        Assert.IsFalse(AIStoryteller.SituationMatchesTriggerScope(roomScoped.Object, 99L, new List<long> { 2L }));
    }

    [TestMethod]
    public void ExecuteFunctionCall_ResolveSituation_InvokesUpdateAndResolve()
    {
        AIStoryteller storyteller = CreateStoryteller();
        string title = "Before";
        string details = "Before details";
        bool resolved = false;
        Mock<IAIStorytellerSituation> situation = new();
        situation.SetupGet(x => x.Id).Returns(26L);
        situation.SetupGet(x => x.Name).Returns(() => title);
        situation.SetupGet(x => x.SituationText).Returns(() => details);
        situation.SetupGet(x => x.IsResolved).Returns(() => resolved);
        situation.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
        situation.Setup(x => x.UpdateSituation(It.IsAny<string>(), It.IsAny<string>()))
            .Callback<string, string>((newTitle, newDetails) =>
            {
                title = newTitle;
                details = newDetails;
            });
        situation.Setup(x => x.Resolve()).Callback(() => resolved = true);
        storyteller.RegisterLoadedSituation(situation.Object);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("ResolveSituation",
            """{"Id":26,"Title":"After","Description":"After details"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(result.MalformedJson);
        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        situation.Verify(x => x.UpdateSituation("After", "After details"), Times.Once);
        situation.Verify(x => x.Resolve(), Times.Once);
    }

    [TestMethod]
    public void ExecuteFunctionCall_ForgetMemory_RemovesMemoryFromStoryteller()
    {
        AIStoryteller storyteller = CreateStoryteller();
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(100L);

        Mock<IAIStorytellerCharacterMemory> memory = new();
        memory.SetupGet(x => x.Id).Returns(11L);
        memory.SetupGet(x => x.MemoryTitle).Returns("Memory title");
        memory.SetupGet(x => x.MemoryText).Returns("Memory details");
        memory.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
        memory.SetupGet(x => x.Character).Returns(character.Object);
        storyteller.RegisterLoadedMemory(memory.Object);

        AIStoryteller.ToolExecutionResult forget = storyteller.ExecuteFunctionCall("ForgetMemory", """{"Id":11}""", includeEchoTools: false);
        JsonElement forgetPayload = JsonDocument.Parse(forget.OutputJson).RootElement;

        Assert.IsTrue(forgetPayload.GetProperty("ok").GetBoolean());
        memory.Verify(x => x.Forget(), Times.Once);

        AIStoryteller.ToolExecutionResult update = storyteller.ExecuteFunctionCall("UpdateMemory",
            """{"Id":11,"Title":"After","Details":"After details"}""",
            includeEchoTools: false);
        JsonElement updatePayload = JsonDocument.Parse(update.OutputJson).RootElement;
        Assert.IsFalse(updatePayload.GetProperty("ok").GetBoolean());
    }

    [TestMethod]
    public void ExecuteFunctionCall_CustomToolGenderConversion_ParsesAndExecutes()
    {
        AIStoryteller storyteller = CreateStoryteller();
        object[]? capturedArgs = null;
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters)
            .Returns(new List<Tuple<ProgVariableTypes, string>>
            {
                Tuple.Create(ProgVariableTypes.Gender, "SubjectGender")
            });
        prog.SetupGet(x => x.CompileError).Returns(string.Empty);
        prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
        prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
            .Callback((object[] args) => capturedArgs = args)
            .Returns("ok");

        storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomGenderTool", "desc", prog.Object));

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CustomGenderTool",
            """{"SubjectGender":"male"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsNotNull(capturedArgs);
        Assert.AreEqual(1, capturedArgs!.Length);
        Assert.AreEqual(Gender.Male, capturedArgs[0]);
    }

    [TestMethod]
    public void ExecuteFunctionCall_InvalidCustomToolCompileError_ReturnsError()
    {
        AIStoryteller storyteller = CreateStoryteller();
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters)
            .Returns(new List<Tuple<ProgVariableTypes, string>>
            {
                Tuple.Create(ProgVariableTypes.Text, "Cue")
            });
        prog.SetupGet(x => x.CompileError).Returns("Compile failure");
        prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
        storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("BadTool", "desc", prog.Object));

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("BadTool",
            """{"Cue":"x"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
        StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty, "does not compile");
    }

    [TestMethod]
    public void ExecuteFunctionCall_CustomToolOutfitConversion_ParsesAndExecutes()
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(200L);

        Mock<IOutfit> outfit = new();
        outfit.SetupGet(x => x.Name).Returns("Guard Uniform");
        character.SetupGet(x => x.Outfits).Returns([outfit.Object]);

        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object]);
        object[]? capturedArgs = null;
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters)
            .Returns(new List<Tuple<ProgVariableTypes, string>>
            {
                Tuple.Create(ProgVariableTypes.Outfit, "Wardrobe")
            });
        prog.SetupGet(x => x.CompileError).Returns(string.Empty);
        prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
            .Callback((object[] args) => capturedArgs = args)
            .Returns(true);
        storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomOutfitTool", "desc", prog.Object));

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CustomOutfitTool",
            """{"Wardrobe":{"OwnerCharacterId":200,"OutfitName":"Guard Uniform"}}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsNotNull(capturedArgs);
        Assert.AreEqual(1, capturedArgs!.Length);
        Assert.AreSame(outfit.Object, capturedArgs[0]);
    }

    [TestMethod]
    public void ExecuteFunctionCall_CustomToolOutfitItemConversion_ParsesAndExecutes()
    {
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(201L);

        Mock<IOutfitItem> outfitItem = new();
        outfitItem.SetupGet(x => x.Id).Returns(999L);

        Mock<IOutfit> outfit = new();
        outfit.SetupGet(x => x.Name).Returns("Harbour Kit");
        outfit.SetupGet(x => x.Items).Returns([outfitItem.Object]);
        character.SetupGet(x => x.Outfits).Returns([outfit.Object]);

        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object]);
        object[]? capturedArgs = null;
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters)
            .Returns(new List<Tuple<ProgVariableTypes, string>>
            {
                Tuple.Create(ProgVariableTypes.OutfitItem, "LoadoutItem")
            });
        prog.SetupGet(x => x.CompileError).Returns(string.Empty);
        prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
            .Callback((object[] args) => capturedArgs = args)
            .Returns(true);
        storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomOutfitItemTool", "desc", prog.Object));

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CustomOutfitItemTool",
            """{"LoadoutItem":{"OwnerCharacterId":201,"OutfitName":"Harbour Kit","ItemId":999}}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsNotNull(capturedArgs);
        Assert.AreEqual(1, capturedArgs!.Length);
        Assert.AreSame(outfitItem.Object, capturedArgs[0]);
    }

    [TestMethod]
    public void ExecuteFunctionCall_CustomToolEffectConversion_ParsesAndExecutes()
    {
        Mock<IEffect> effect = new();
        effect.SetupGet(x => x.Id).Returns(777L);

        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(202L);
        character.SetupGet(x => x.Effects).Returns([effect.Object]);

        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object]);
        object[]? capturedArgs = null;
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters)
            .Returns(new List<Tuple<ProgVariableTypes, string>>
            {
                Tuple.Create(ProgVariableTypes.Effect, "AppliedEffect")
            });
        prog.SetupGet(x => x.CompileError).Returns(string.Empty);
        prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
            .Callback((object[] args) => capturedArgs = args)
            .Returns(true);
        storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomEffectTool", "desc", prog.Object));

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CustomEffectTool",
            """{"AppliedEffect":{"EffectId":777,"OwnerType":"character","OwnerId":202}}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsNotNull(capturedArgs);
        Assert.AreEqual(1, capturedArgs!.Length);
        Assert.AreSame(effect.Object, capturedArgs[0]);
    }

    [TestMethod]
    public void ExecuteFunctionCall_CustomToolExpandedTypeParsers_ParsesAndExecutes()
    {
        Mock<IShard> shard = new();
        shard.SetupGet(x => x.Id).Returns(11L);
        shard.SetupGet(x => x.Name).Returns("Shard Alpha");

        Mock<IZone> zone = new();
        zone.SetupGet(x => x.Id).Returns(12L);
        zone.SetupGet(x => x.Name).Returns("Harbour Zone");

        Mock<IRace> race = new();
        race.SetupGet(x => x.Id).Returns(13L);
        race.SetupGet(x => x.Name).Returns("Human");

        Mock<ICulture> culture = new();
        culture.SetupGet(x => x.Id).Returns(14L);
        culture.SetupGet(x => x.Name).Returns("Imperial");

        Mock<ITraitDefinition> trait = new();
        trait.SetupGet(x => x.Id).Returns(15L);
        trait.SetupGet(x => x.Name).Returns("Swordsmanship");

        Mock<IEthnicity> ethnicity = new();
        ethnicity.SetupGet(x => x.Id).Returns(16L);
        ethnicity.SetupGet(x => x.Name).Returns("Coastal");

        Mock<IClan> clan = new();
        clan.SetupGet(x => x.Id).Returns(17L);
        clan.SetupGet(x => x.Name).Returns("Wardens");
        clan.SetupGet(x => x.Names).Returns(["Wardens"]);

        Mock<IRank> rank = new();
        rank.SetupGet(x => x.Id).Returns(18L);
        rank.SetupGet(x => x.Name).Returns("Captain");

        Mock<IAppointment> appointment = new();
        appointment.SetupGet(x => x.Id).Returns(19L);
        appointment.SetupGet(x => x.Name).Returns("Marshal");

        Mock<IPaygrade> paygrade = new();
        paygrade.SetupGet(x => x.Id).Returns(20L);
        paygrade.SetupGet(x => x.Name).Returns("Grade-3");
        paygrade.SetupGet(x => x.Abbreviation).Returns("G3");

        clan.SetupGet(x => x.Ranks).Returns([rank.Object]);
        clan.SetupGet(x => x.Appointments).Returns([appointment.Object]);
        clan.SetupGet(x => x.Paygrades).Returns([paygrade.Object]);

        Mock<ICurrency> currency = new();
        currency.SetupGet(x => x.Id).Returns(21L);
        currency.SetupGet(x => x.Name).Returns("Sovereign");

        Mock<IExit> exit = new();
        Mock<IExitManager> exitManager = new();
        exitManager.Setup(x => x.GetExitByID(22L)).Returns(exit.Object);

        Mock<ILanguage> language = new();
        language.SetupGet(x => x.Id).Returns(23L);
        language.SetupGet(x => x.Name).Returns("Anglic");

        Mock<IAccent> accent = new();
        accent.SetupGet(x => x.Id).Returns(24L);
        accent.SetupGet(x => x.Name).Returns("Harbour Drawl");

        Mock<IMerit> merit = new();
        merit.SetupGet(x => x.Id).Returns(25L);
        merit.SetupGet(x => x.Name).Returns("Quick Learner");

        Mock<ICalendar> calendar = new();
        calendar.SetupGet(x => x.Id).Returns(26L);
        calendar.SetupGet(x => x.Name).Returns("Solar Calendar");
        calendar.SetupGet(x => x.Names).Returns(["Solar Calendar", "solar"]);

        Mock<IClock> clock = new();
        clock.SetupGet(x => x.Id).Returns(27L);
        clock.SetupGet(x => x.Name).Returns("City Clock");
        clock.SetupGet(x => x.Names).Returns(["City Clock", "city"]);

        Mock<IKnowledge> knowledge = new();
        knowledge.SetupGet(x => x.Id).Returns(28L);
        knowledge.SetupGet(x => x.Name).Returns("Lore of Ports");

        Mock<IChargenRole> role = new();
        role.SetupGet(x => x.Id).Returns(29L);
        role.SetupGet(x => x.Name).Returns("Dockmaster");

        Mock<IDrug> drug = new();
        drug.SetupGet(x => x.Id).Returns(30L);
        drug.SetupGet(x => x.Name).Returns("Blackleaf");

        Mock<IShop> shop = new();
        shop.SetupGet(x => x.Id).Returns(31L);
        shop.SetupGet(x => x.Name).Returns("Harbour Outfitters");

        Mock<IFuturemud> gameworld = CreateGameworld(Array.Empty<IFutureProg>(), Array.Empty<ICharacter>());
        gameworld.SetupGet(x => x.Shards).Returns(BuildRepository([shard.Object]).Object);
        gameworld.SetupGet(x => x.Zones).Returns(BuildRepository([zone.Object]).Object);
        gameworld.SetupGet(x => x.Races).Returns(BuildRepository([race.Object]).Object);
        gameworld.SetupGet(x => x.Cultures).Returns(BuildRepository([culture.Object]).Object);
        gameworld.SetupGet(x => x.Traits).Returns(BuildRepository([trait.Object]).Object);
        gameworld.SetupGet(x => x.Clans).Returns(BuildRepository([clan.Object]).Object);
        gameworld.SetupGet(x => x.Ethnicities).Returns(BuildRepository([ethnicity.Object]).Object);
        gameworld.SetupGet(x => x.Currencies).Returns(BuildRepository([currency.Object]).Object);
        gameworld.SetupGet(x => x.Languages).Returns(BuildRepository([language.Object]).Object);
        gameworld.SetupGet(x => x.Accents).Returns(BuildRepository([accent.Object]).Object);
        gameworld.SetupGet(x => x.Merits).Returns(BuildRepository([merit.Object]).Object);
        gameworld.SetupGet(x => x.Calendars).Returns(BuildRepository([calendar.Object]).Object);
        gameworld.SetupGet(x => x.Clocks).Returns(BuildRepository([clock.Object]).Object);
        gameworld.SetupGet(x => x.Knowledges).Returns(BuildRepository([knowledge.Object]).Object);
        gameworld.SetupGet(x => x.Roles).Returns(BuildRepository([role.Object]).Object);
        gameworld.SetupGet(x => x.Drugs).Returns(BuildRepository([drug.Object]).Object);
        gameworld.SetupGet(x => x.Shops).Returns(BuildRepository([shop.Object]).Object);
        gameworld.SetupGet(x => x.ExitManager).Returns(exitManager.Object);

        AIStoryteller storyteller = new(CreateModel(), gameworld.Object);
        object[]? capturedArgs = null;
        Mock<IFutureProg> prog = new();
        prog.SetupGet(x => x.NamedParameters)
            .Returns(new List<Tuple<ProgVariableTypes, string>>
            {
                Tuple.Create(ProgVariableTypes.Shard, "ShardArg"),
                Tuple.Create(ProgVariableTypes.Zone, "ZoneArg"),
                Tuple.Create(ProgVariableTypes.Gender, "GenderArg"),
                Tuple.Create(ProgVariableTypes.Race, "RaceArg"),
                Tuple.Create(ProgVariableTypes.Culture, "CultureArg"),
                Tuple.Create(ProgVariableTypes.Trait, "TraitArg"),
                Tuple.Create(ProgVariableTypes.Clan, "ClanArg"),
                Tuple.Create(ProgVariableTypes.Ethnicity, "EthnicityArg"),
                Tuple.Create(ProgVariableTypes.ClanRank, "ClanRankArg"),
                Tuple.Create(ProgVariableTypes.ClanAppointment, "ClanAppointmentArg"),
                Tuple.Create(ProgVariableTypes.ClanPaygrade, "ClanPaygradeArg"),
                Tuple.Create(ProgVariableTypes.Currency, "CurrencyArg"),
                Tuple.Create(ProgVariableTypes.Exit, "ExitArg"),
                Tuple.Create(ProgVariableTypes.DateTime, "DateTimeArg"),
                Tuple.Create(ProgVariableTypes.TimeSpan, "TimeSpanArg"),
                Tuple.Create(ProgVariableTypes.Language, "LanguageArg"),
                Tuple.Create(ProgVariableTypes.Accent, "AccentArg"),
                Tuple.Create(ProgVariableTypes.Merit, "MeritArg"),
                Tuple.Create(ProgVariableTypes.MudDateTime, "MudDateTimeArg"),
                Tuple.Create(ProgVariableTypes.Calendar, "CalendarArg"),
                Tuple.Create(ProgVariableTypes.Clock, "ClockArg"),
                Tuple.Create(ProgVariableTypes.Knowledge, "KnowledgeArg"),
                Tuple.Create(ProgVariableTypes.Role, "RoleArg"),
                Tuple.Create(ProgVariableTypes.Drug, "DrugArg"),
                Tuple.Create(ProgVariableTypes.Shop, "ShopArg")
            });
        prog.SetupGet(x => x.CompileError).Returns(string.Empty);
        prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
        prog.Setup(x => x.ExecuteWithRecursionProtection(It.IsAny<object[]>()))
            .Callback((object[] args) => capturedArgs = args)
            .Returns(true);
        storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("CustomExpandedTool", "desc", prog.Object));

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CustomExpandedTool",
            """
			{
			  "ShardArg": {"ShardName":"Shard Alpha"},
			  "ZoneArg": "Harbour Zone",
			  "GenderArg": {"Gender":"female"},
			  "RaceArg": {"RaceName":"Human"},
			  "CultureArg": {"CultureName":"Imperial"},
			  "TraitArg": {"TraitName":"Swordsmanship"},
			  "ClanArg": {"ClanName":"Wardens"},
			  "EthnicityArg": {"EthnicityName":"Coastal"},
			  "ClanRankArg": {"Clan":"Wardens","RankName":"Captain"},
			  "ClanAppointmentArg": {"ClanName":"Wardens","AppointmentName":"Marshal"},
			  "ClanPaygradeArg": {"Clan":"Wardens","PaygradeAbbreviation":"G3"},
			  "CurrencyArg": {"CurrencyName":"Sovereign"},
			  "ExitArg": {"ExitId":22},
			  "DateTimeArg": {"Value":"2026-02-11T12:34:56Z"},
			  "TimeSpanArg": {"TotalSeconds":90},
			  "LanguageArg": {"LanguageName":"Anglic"},
			  "AccentArg": {"AccentName":"Harbour Drawl"},
			  "MeritArg": {"MeritName":"Quick Learner"},
			  "MudDateTimeArg": {"Value":"never"},
			  "CalendarArg": {"Alias":"solar"},
			  "ClockArg": {"Alias":"city"},
			  "KnowledgeArg": {"KnowledgeName":"Lore of Ports"},
			  "RoleArg": {"RoleName":"Dockmaster"},
			  "DrugArg": {"DrugName":"Blackleaf"},
			  "ShopArg": {"ShopName":"Harbour Outfitters"}
			}
			""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsNotNull(capturedArgs);
        Assert.AreEqual(25, capturedArgs!.Length);
        Assert.AreSame(shard.Object, capturedArgs[0]);
        Assert.AreSame(zone.Object, capturedArgs[1]);
        Assert.AreEqual(Gender.Female, capturedArgs[2]);
        Assert.AreSame(race.Object, capturedArgs[3]);
        Assert.AreSame(culture.Object, capturedArgs[4]);
        Assert.AreSame(trait.Object, capturedArgs[5]);
        Assert.AreSame(clan.Object, capturedArgs[6]);
        Assert.AreSame(ethnicity.Object, capturedArgs[7]);
        Assert.AreSame(rank.Object, capturedArgs[8]);
        Assert.AreSame(appointment.Object, capturedArgs[9]);
        Assert.AreSame(paygrade.Object, capturedArgs[10]);
        Assert.AreSame(currency.Object, capturedArgs[11]);
        Assert.AreSame(exit.Object, capturedArgs[12]);
        Assert.AreEqual(DateTime.Parse("2026-02-11T12:34:56Z").ToUniversalTime(),
            ((DateTime)capturedArgs[13]).ToUniversalTime());
        Assert.AreEqual(TimeSpan.FromSeconds(90), (TimeSpan)capturedArgs[14]);
        Assert.AreSame(language.Object, capturedArgs[15]);
        Assert.AreSame(accent.Object, capturedArgs[16]);
        Assert.AreSame(merit.Object, capturedArgs[17]);
        Assert.IsInstanceOfType(capturedArgs[18], typeof(MudDateTime));
        Assert.IsNull(((MudDateTime)capturedArgs[18]).Date);
        Assert.AreSame(calendar.Object, capturedArgs[19]);
        Assert.AreSame(clock.Object, capturedArgs[20]);
        Assert.AreSame(knowledge.Object, capturedArgs[21]);
        Assert.AreSame(role.Object, capturedArgs[22]);
        Assert.AreSame(drug.Object, capturedArgs[23]);
        Assert.AreSame(shop.Object, capturedArgs[24]);
    }

    [TestMethod]
    public void ExecuteFunctionCall_PathBetweenRooms_ReturnsDirectionsPayload()
    {
        Mock<ICell> room = new();
        room.SetupGet(x => x.Id).Returns(100L);
        AIStoryteller storyteller = CreateStoryteller(cells: [room.Object]);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("PathBetweenRooms",
            """{"OriginRoomId":100,"DestinationRoomId":100,"PathSearchFunction":"IgnorePresenceOfDoors"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;
        JsonElement directions = payload.GetProperty("result").GetProperty("Directions");

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsTrue(payload.GetProperty("result").GetProperty("HasPath").GetBoolean());
        Assert.AreEqual(0, directions.GetArrayLength());
    }

    [TestMethod]
    public void ExecuteFunctionCall_PathFromCharacterToRoom_ReturnsDirectionsPayload()
    {
        Mock<ICell> room = new();
        room.SetupGet(x => x.Id).Returns(101L);
        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(1L);
        character.SetupGet(x => x.Location).Returns(room.Object);
        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object], cells: [room.Object]);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("PathFromCharacterToRoom",
            """{"OriginCharacterId":1,"DestinationRoomId":101,"PathSearchFunction":"PathIncludeUnlockableDoors"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;
        JsonElement directions = payload.GetProperty("result").GetProperty("Directions");

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsTrue(payload.GetProperty("result").GetProperty("HasPath").GetBoolean());
        Assert.AreEqual(0, directions.GetArrayLength());
    }

    [TestMethod]
    public void ExecuteFunctionCall_PathBetweenCharacters_ReturnsDirectionsPayload()
    {
        Mock<ICell> room = new();
        room.SetupGet(x => x.Id).Returns(102L);
        Mock<ICharacter> origin = new();
        origin.SetupGet(x => x.Id).Returns(11L);
        origin.SetupGet(x => x.Location).Returns(room.Object);
        Mock<ICharacter> destination = new();
        destination.SetupGet(x => x.Id).Returns(12L);
        destination.SetupGet(x => x.Location).Returns(room.Object);
        AIStoryteller storyteller = CreateStoryteller(characters: [origin.Object, destination.Object], cells: [room.Object]);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("PathBetweenCharacters",
            """{"OriginCharacterId":11,"DestinationCharacterId":12,"PathSearchFunction":"PathIgnoreDoors"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;
        JsonElement directions = payload.GetProperty("result").GetProperty("Directions");

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.IsTrue(payload.GetProperty("result").GetProperty("HasPath").GetBoolean());
        Assert.AreEqual(0, directions.GetArrayLength());
    }

    [TestMethod]
    public void ExecuteFunctionCall_RecentCharacterPlans_ReturnsRecentPlans()
    {
        Mock<ICharacter> character = BuildCharacterWithPlans(
            id: 300L,
            name: "Alice Example",
            shortDescription: "a focused planner",
            shortPlan: "Secure dock contracts.",
            longPlan: "Expand guild influence.");
        RecentlyUpdatedPlan recentEffect = new(character.Object);
        character.Setup(x => x.AffectedBy<RecentlyUpdatedPlan>()).Returns(true);
        character.Setup(x => x.EffectsOfType<RecentlyUpdatedPlan>(It.IsAny<Predicate<RecentlyUpdatedPlan>>()))
            .Returns([recentEffect]);
        character.Setup(x => x.ScheduledDuration(It.IsAny<IEffect>())).Returns(TimeSpan.FromDays(3));
        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object]);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("RecentCharacterPlans", "{}", includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;
        JsonElement plans = payload.GetProperty("result").GetProperty("Plans");

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.AreEqual(1, plans.GetArrayLength());
        Assert.AreEqual(300L, plans[0].GetProperty("Id").GetInt64());
        Assert.AreEqual("Secure dock contracts.", plans[0].GetProperty("ShortTermPlan").GetString());
        Assert.AreEqual("Expand guild influence.", plans[0].GetProperty("LongTermPlan").GetString());
    }

    [TestMethod]
    public void ExecuteFunctionCall_CharacterPlans_ReturnsSpecificCharacterPlans()
    {
        Mock<ICharacter> character = BuildCharacterWithPlans(
            id: 301L,
            name: "Basil Planner",
            shortDescription: "a thoughtful strategist",
            shortPlan: "Recruit allies.",
            longPlan: "Stabilize trade routes.");
        RecentlyUpdatedPlan recentEffect = new(character.Object);
        character.Setup(x => x.EffectsOfType<RecentlyUpdatedPlan>(It.IsAny<Predicate<RecentlyUpdatedPlan>>()))
            .Returns([recentEffect]);
        character.Setup(x => x.ScheduledDuration(It.IsAny<IEffect>())).Returns(TimeSpan.FromDays(1));
        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object]);

        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CharacterPlans", """{"Id":301}""", includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;
        JsonElement details = payload.GetProperty("result");

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.AreEqual(301L, details.GetProperty("Id").GetInt64());
        Assert.AreEqual("Recruit allies.", details.GetProperty("ShortTermPlan").GetString());
        Assert.AreEqual("Stabilize trade routes.", details.GetProperty("LongTermPlan").GetString());
        Assert.IsTrue(details.GetProperty("RecentlyUpdated").GetBoolean());
    }


    [TestMethod]
    public void ExecuteFunctionCall_CurrentDateTime_MultipleContexts_ReturnsError()
    {
        Mock<IClock> clock = new();
        clock.SetupGet(x => x.Id).Returns(51L);
        Mock<IMudTimeZone> timezoneOne = new();
        timezoneOne.SetupGet(x => x.Id).Returns(1L);
        Mock<IMudTimeZone> timezoneTwo = new();
        timezoneTwo.SetupGet(x => x.Id).Returns(2L);
        Mock<ICalendar> calendar = new();
        calendar.SetupGet(x => x.Id).Returns(41L);
        calendar.SetupGet(x => x.FeedClock).Returns(clock.Object);

        Mock<ICell> roomOne = new();
        roomOne.SetupGet(x => x.Id).Returns(401L);
        roomOne.SetupGet(x => x.Calendars).Returns([calendar.Object]);
        roomOne.Setup(x => x.TimeZone(clock.Object)).Returns(timezoneOne.Object);
        Mock<ICell> roomTwo = new();
        roomTwo.SetupGet(x => x.Id).Returns(402L);
        roomTwo.SetupGet(x => x.Calendars).Returns([calendar.Object]);
        roomTwo.Setup(x => x.TimeZone(clock.Object)).Returns(timezoneTwo.Object);

        AIStoryteller storyteller = CreateStoryteller(cells: [roomOne.Object, roomTwo.Object]);
        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CurrentDateTime", "{}", includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
        StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty,
            "Multiple calendar/clock/timezone contexts are in use");
    }

    [TestMethod]
    public void ExecuteFunctionCall_DateTimeForTarget_Character_ReturnsDateTimePayload()
    {
        Mock<IClock> clock = new();
        clock.SetupGet(x => x.Id).Returns(52L);
        clock.SetupGet(x => x.Name).Returns("City Clock");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Long)).Returns("High Sun");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Short)).Returns("Noon");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Vague)).Returns("Around Noon");

        Mock<IMudTimeZone> timezone = new();
        timezone.SetupGet(x => x.Id).Returns(5L);
        timezone.SetupGet(x => x.Name).Returns("City Standard");

        Mock<ICalendar> calendar = new();
        calendar.SetupGet(x => x.Id).Returns(42L);
        calendar.SetupGet(x => x.FullName).Returns("Civic Calendar");
        calendar.SetupGet(x => x.FeedClock).Returns(clock.Object);
        calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Long)).Returns("3rd Rainfall 1200");
        calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Short)).Returns("3-RF-1200");

        Mock<ICell> room = new();
        room.SetupGet(x => x.Id).Returns(403L);
        room.SetupGet(x => x.Calendars).Returns([calendar.Object]);
        room.Setup(x => x.TimeZone(clock.Object)).Returns(timezone.Object);
        room.Setup(x => x.Date(calendar.Object)).Returns((MudDate)null!);
        room.Setup(x => x.Time(clock.Object)).Returns((MudTime)null!);
        room.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
                It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
            .Returns("Market Square");

        Mock<IPersonalName> personalName = new();
        personalName.Setup(x => x.GetName(NameStyle.FullName)).Returns("Alyx Ward");

        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(501L);
        character.SetupGet(x => x.Location).Returns(room.Object);
        character.SetupGet(x => x.PersonalName).Returns(personalName.Object);

        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object], cells: [room.Object]);
        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("DateTimeForTarget", """{"CharacterId":501}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;
        JsonElement resultPayload = payload.GetProperty("result");

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.AreEqual("High Sun on 3rd Rainfall 1200", resultPayload.GetProperty("DateTime").GetString());
        Assert.AreEqual(501L, resultPayload.GetProperty("CharacterId").GetInt64());
        Assert.AreEqual("Market Square", resultPayload.GetProperty("RoomName").GetString());
    }

    [TestMethod]
    public void ExecuteFunctionCall_DateTimeForTarget_WithCharacterAndRoom_ReturnsError()
    {
        Mock<IClock> clock = new();
        clock.SetupGet(x => x.Id).Returns(53L);
        clock.SetupGet(x => x.Name).Returns("City Clock");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Long)).Returns("High Sun");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Short)).Returns("Noon");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Vague)).Returns("Around Noon");

        Mock<IMudTimeZone> timezone = new();
        timezone.SetupGet(x => x.Id).Returns(6L);
        timezone.SetupGet(x => x.Name).Returns("City Standard");

        Mock<ICalendar> calendar = new();
        calendar.SetupGet(x => x.Id).Returns(43L);
        calendar.SetupGet(x => x.FullName).Returns("Civic Calendar");
        calendar.SetupGet(x => x.FeedClock).Returns(clock.Object);
        calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Long)).Returns("3rd Rainfall 1200");
        calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Short)).Returns("3-RF-1200");

        Mock<ICell> characterRoom = new();
        characterRoom.SetupGet(x => x.Id).Returns(405L);
        characterRoom.SetupGet(x => x.Calendars).Returns([calendar.Object]);
        characterRoom.Setup(x => x.TimeZone(clock.Object)).Returns(timezone.Object);
        characterRoom.Setup(x => x.Date(calendar.Object)).Returns((MudDate)null!);
        characterRoom.Setup(x => x.Time(clock.Object)).Returns((MudTime)null!);
        characterRoom.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
                It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
            .Returns("Market Square");

        Mock<ICell> otherRoom = new();
        otherRoom.SetupGet(x => x.Id).Returns(406L);
        otherRoom.SetupGet(x => x.Calendars).Returns([calendar.Object]);
        otherRoom.Setup(x => x.TimeZone(clock.Object)).Returns(timezone.Object);
        otherRoom.Setup(x => x.Date(calendar.Object)).Returns((MudDate)null!);
        otherRoom.Setup(x => x.Time(clock.Object)).Returns((MudTime)null!);
        otherRoom.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
                It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
            .Returns("Docks");

        Mock<IPersonalName> personalName = new();
        personalName.Setup(x => x.GetName(NameStyle.FullName)).Returns("Alyx Ward");

        Mock<ICharacter> character = new();
        character.SetupGet(x => x.Id).Returns(502L);
        character.SetupGet(x => x.Location).Returns(characterRoom.Object);
        character.SetupGet(x => x.PersonalName).Returns(personalName.Object);

        AIStoryteller storyteller = CreateStoryteller(characters: [character.Object], cells: [characterRoom.Object, otherRoom.Object]);
        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("DateTimeForTarget", """{"CharacterId":502,"RoomId":406}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
        StringAssert.Contains(payload.GetProperty("error").GetString(), "Specify either CharacterId or RoomId");
    }

    [TestMethod]
    public void ExecuteFunctionCall_DateTimeForTarget_WithZeroCharacterIdAndRoom_ReturnsError()
    {
        Mock<IClock> clock = new();
        clock.SetupGet(x => x.Id).Returns(54L);
        clock.SetupGet(x => x.Name).Returns("City Clock");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Long)).Returns("High Sun");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Short)).Returns("Noon");
        clock.Setup(x => x.DisplayTime(It.IsAny<MudTime>(), TimeDisplayTypes.Vague)).Returns("Around Noon");

        Mock<IMudTimeZone> timezone = new();
        timezone.SetupGet(x => x.Id).Returns(7L);
        timezone.SetupGet(x => x.Name).Returns("City Standard");

        Mock<ICalendar> calendar = new();
        calendar.SetupGet(x => x.Id).Returns(44L);
        calendar.SetupGet(x => x.FullName).Returns("Civic Calendar");
        calendar.SetupGet(x => x.FeedClock).Returns(clock.Object);
        calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Long)).Returns("3rd Rainfall 1200");
        calendar.Setup(x => x.DisplayDate(It.IsAny<MudDate>(), CalendarDisplayMode.Short)).Returns("3-RF-1200");

        Mock<ICell> room = new();
        room.SetupGet(x => x.Id).Returns(407L);
        room.SetupGet(x => x.Calendars).Returns([calendar.Object]);
        room.Setup(x => x.TimeZone(clock.Object)).Returns(timezone.Object);
        room.Setup(x => x.Date(calendar.Object)).Returns((MudDate)null!);
        room.Setup(x => x.Time(clock.Object)).Returns((MudTime)null!);
        room.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
                It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
            .Returns("Docks");

        AIStoryteller storyteller = CreateStoryteller(cells: [room.Object]);
        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("DateTimeForTarget", """{"CharacterId":0,"RoomId":407}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;

        Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
        StringAssert.Contains(payload.GetProperty("error").GetString(), "Specify either CharacterId or RoomId");
    }

    [TestMethod]
    public void ExecuteFunctionCall_CalendarDefinition_ReturnsCalendarMetadata()
    {
        Mock<ICalendar> calendar = new();
        calendar.SetupGet(x => x.Id).Returns(77L);
        calendar.SetupGet(x => x.Name).Returns("Harvest Calendar");
        calendar.SetupGet(x => x.FullName).Returns("Harvest Calendar");
        calendar.SetupGet(x => x.ShortName).Returns("Harvest");
        calendar.SetupGet(x => x.Alias).Returns("harvest");
        calendar.SetupGet(x => x.Description).Returns("Tracks agrarian seasons.");
        calendar.SetupGet(x => x.AncientEraLongString).Returns("Before Founding");
        calendar.SetupGet(x => x.ModernEraLongString).Returns("After Founding");
        calendar.SetupGet(x => x.Weekdays).Returns(["Primeday", "Dualday"]);
        calendar.SetupGet(x => x.Months).Returns([]);
        calendar.SetupGet(x => x.Intercalaries).Returns([]);
        calendar.SetupGet(x => x.Names).Returns(["Harvest Calendar", "harvest"]);

        AIStoryteller storyteller = CreateStoryteller(calendars: [calendar.Object]);
        AIStoryteller.ToolExecutionResult result = storyteller.ExecuteFunctionCall("CalendarDefinition", """{"Id":"harvest"}""",
            includeEchoTools: false);
        JsonElement payload = JsonDocument.Parse(result.OutputJson).RootElement;
        JsonElement resultPayload = payload.GetProperty("result");

        Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
        Assert.AreEqual(77L, resultPayload.GetProperty("Id").GetInt64());
        Assert.AreEqual("Harvest Calendar", resultPayload.GetProperty("Name").GetString());
        Assert.AreEqual("Tracks agrarian seasons.", resultPayload.GetProperty("Description").GetString());
    }

    [TestMethod]
    public void PassHeartbeatEventToAIStorytellerForTesting_MissingApiKey_DoesNotExecuteStatusProg()
    {
        (Futuremud? runtimeGame, bool disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
        try
        {
            Mock<IFutureProg> heartbeatProg = new();
            heartbeatProg.SetupGet(x => x.Id).Returns(123L);
            heartbeatProg.Setup(x => x.ExecuteString(It.IsAny<object[]>()))
                .Throws(new AssertFailedException("Heartbeat status prog should not execute without API key."));

            Mock<IFuturemud> gameworld = CreateGameworld(
                new[] { heartbeatProg.Object },
                Array.Empty<ICharacter>());
            ModelStoryteller model = CreateModel();
            model.HeartbeatStatus5mProgId = 123L;
            AIStoryteller storyteller = new(model, gameworld.Object);

            storyteller.PassHeartbeatEventToAIStorytellerForTesting("5m");

            heartbeatProg.Verify(x => x.ExecuteString(It.IsAny<object[]>()), Times.Never);
        }
        finally
        {
            if (disposeRuntimeGame)
            {
                runtimeGame.Dispose();
            }
        }
    }

    [TestMethod]
    public void CellOnRoomEchoForTesting_MissingApiKey_DoesNotParseEcho()
    {
        (Futuremud? runtimeGame, bool disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
        try
        {
            AIStoryteller storyteller = CreateStoryteller();
            Mock<ICell> cell = new();
            Mock<IEmoteOutput> emote = new();
            emote.Setup(x => x.ParseFor(It.IsAny<IPerceiver>()))
                .Throws(new AssertFailedException("Echo parse should not execute without API key."));

            storyteller.CellOnRoomEchoForTesting(cell.Object, emote.Object);

            emote.Verify(x => x.ParseFor(It.IsAny<IPerceiver>()), Times.Never);
        }
        finally
        {
            if (disposeRuntimeGame)
            {
                runtimeGame.Dispose();
            }
        }
    }

    [TestMethod]
    public void InvokeDirectAttention_MissingApiKey_ReturnsFalse()
    {
        (Futuremud? runtimeGame, bool disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
        try
        {
            AIStoryteller storyteller = CreateStoryteller();
            bool result = storyteller.InvokeDirectAttention("Direct invocation from FutureProg.");
            Assert.IsFalse(result);
        }
        finally
        {
            if (disposeRuntimeGame)
            {
                runtimeGame.Dispose();
            }
        }
    }

    [TestMethod]
    public void TryParseAttentionClassifierOutput_ValidInterestedJson_ParsesReason()
    {
        bool parsed = AIStoryteller.TryParseAttentionClassifierOutput(
            """{"Decision":"interested","Reason":"world-impacting event"}""",
            out bool interested,
            out string? reason,
            out string? error);

        Assert.IsTrue(parsed);
        Assert.IsTrue(interested);
        Assert.AreEqual("world-impacting event", reason);
        Assert.AreEqual(string.Empty, error);
    }

    [TestMethod]
    public void TryParseAttentionClassifierOutput_InvalidText_ReturnsContractError()
    {
        bool parsed = AIStoryteller.TryParseAttentionClassifierOutput(
            "interested because this matters",
            out bool interested,
            out string? reason,
            out string? error);

        Assert.IsFalse(parsed);
        Assert.IsFalse(interested);
        Assert.AreEqual(string.Empty, reason);
        StringAssert.Contains(error, "not valid JSON");
    }

    [TestMethod]
    public void TryInterpretAttentionClassifierOutputForTesting_InvalidText_LogsError()
    {
        AIStoryteller storyteller = CreateStoryteller();
        string? loggedMessage = null;
        storyteller.ErrorLoggerOverride = message => loggedMessage = message;

        bool parsed = storyteller.TryInterpretAttentionClassifierOutputForTesting(
            "interested because this matters",
            out bool interested,
            out string? reason);

        Assert.IsFalse(parsed);
        Assert.IsFalse(interested);
        Assert.AreEqual(string.Empty, reason);
        Assert.IsFalse(string.IsNullOrWhiteSpace(loggedMessage));
        StringAssert.Contains(loggedMessage, "Attention classifier contract violation");
    }

    private static AIStoryteller CreateStoryteller(IEnumerable<IFutureProg>? progs = null,
        IEnumerable<ICharacter>? characters = null, IEnumerable<ICell>? cells = null,
        IEnumerable<ICalendar>? calendars = null)
    {
        Mock<IFuturemud> gameworld = CreateGameworld(progs ?? Array.Empty<IFutureProg>(), characters ?? Array.Empty<ICharacter>(),
            cells ?? Array.Empty<ICell>(), calendars ?? Array.Empty<ICalendar>());
        return new AIStoryteller(CreateModel(), gameworld.Object);
    }

    private static ModelStoryteller CreateModel()
    {
        return new ModelStoryteller
        {
            Id = 1L,
            Name = "Test Storyteller",
            Description = "Test",
            Model = "gpt-5",
            TimeModel = "gpt-5-mini",
            AttentionClassifierModel = "gpt-5-nano",
            SystemPrompt = "System prompt",
            TimeSystemPrompt = "Time system prompt",
            AttentionAgentPrompt = "Attention prompt",
            SurveillanceStrategyDefinition = string.Empty,
            ReasoningEffort = "2",
            TimeReasoningEffort = "1",
            AttentionClassifierReasoningEffort = "0",
            CustomToolCallsDefinition = "<ToolCalls />",
            SubscribeToRoomEvents = false,
            SubscribeTo5mHeartbeat = false,
            SubscribeTo10mHeartbeat = false,
            SubscribeTo30mHeartbeat = false,
            SubscribeToHourHeartbeat = false,
            IsPaused = false
        };
    }

    private static Mock<IFuturemud> CreateGameworld(IEnumerable<IFutureProg> progs, IEnumerable<ICharacter> characters,
        IEnumerable<ICell>? cells = null, IEnumerable<ICalendar>? calendars = null)
    {
        List<IFutureProg> progList = progs.ToList();
        List<ICharacter> characterList = characters.ToList();
        List<ICell> cellList = (cells ?? Array.Empty<ICell>()).ToList();
        List<ICalendar> calendarList = (calendars ?? Array.Empty<ICalendar>()).ToList();

        Mock<IUneditableAll<IFutureProg>> progRepo = BuildRepository(progList);
        Mock<IUneditableAll<ICharacter>> characterRepo = BuildRepository(characterList);
        Mock<IUneditableAll<ICell>> cellRepo = BuildRepository(cellList);
        Mock<IUneditableAll<ICalendar>> calendarRepo = BuildRepository(calendarList);
        Mock<ISaveManager> saveManager = new();

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.FutureProgs).Returns(progRepo.Object);
        gameworld.SetupGet(x => x.Characters).Returns(characterRepo.Object);
        gameworld.SetupGet(x => x.Cells).Returns(cellRepo.Object);
        gameworld.SetupGet(x => x.Calendars).Returns(calendarRepo.Object);
        gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
        gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
            .Returns((long id, bool _) => characterList.FirstOrDefault(x => x.Id == id)!);
        return gameworld;
    }

    private static Mock<ICharacter> BuildCharacterWithPlans(long id, string name, string shortDescription,
        string shortPlan, string longPlan)
    {
        Mock<ICharacter> character = new();
        Mock<IPersonalName> personalName = new();
        personalName.Setup(x => x.GetName(NameStyle.FullName)).Returns(name);

        character.SetupGet(x => x.Id).Returns(id);
        character.SetupGet(x => x.PersonalName).Returns(personalName.Object);
        character.SetupGet(x => x.ShortTermPlan).Returns(shortPlan);
        character.SetupGet(x => x.LongTermPlan).Returns(longPlan);
        character.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
                It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
            .Returns(shortDescription);
        return character;
    }

    private static (Futuremud RuntimeGame, bool DisposeRuntimeGame) EnsureRuntimeGameWithMissingApiKey()
    {
        Futuremud? existing = Futuremud.Games.OfType<Futuremud>().FirstOrDefault();
        if (existing is not null)
        {
            existing.UpdateStaticConfiguration("GPT_Secret_Key", string.Empty);
            return (existing, false);
        }

        Futuremud created = new(null);
        created.UpdateStaticConfiguration("GPT_Secret_Key", string.Empty);
        return (created, true);
    }

    private static Mock<IUneditableAll<T>> BuildRepository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
    {
        List<T> list = items.ToList();
        Dictionary<long, T> byId = list.ToDictionary(x => x.Id, x => x);
        Mock<IUneditableAll<T>> repo = new();
        repo.Setup(x => x.Get(It.IsAny<long>()))
            .Returns((long id) => byId.TryGetValue(id, out T? value) ? value : null!);
        repo.Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
        repo.SetupGet(x => x.Count).Returns(list.Count);
        return repo;
    }
}

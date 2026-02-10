#nullable enable
#pragma warning disable OPENAI001
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Form.Shape;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.AIStorytellers;
using OpenAI.Responses;
using ModelStoryteller = MudSharp.Models.AIStoryteller;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerToolExecutionTests
{
	[TestMethod]
	public void ExecuteFunctionCall_MalformedJson_ReturnsMalformedError()
	{
		var storyteller = CreateStoryteller();

		var result = storyteller.ExecuteFunctionCall("Noop", "{", includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(result.MalformedJson);
		Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
		StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty, "Malformed tool-call JSON");
	}

	[TestMethod]
	public void ProcessFunctionCallBatch_MalformedJson_AddsRetryFeedback()
	{
		var storyteller = CreateStoryteller();
		var messages = new List<ResponseItem>();

		var result = storyteller.ProcessFunctionCallBatch(
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
		var storyteller = CreateStoryteller();
		string? loggedMessage = null;
		storyteller.ErrorLoggerOverride = message => loggedMessage = message;

		var retries = 0;
		var messages = new List<ResponseItem>();
		var result = (Continue: true, MalformedRetries: 0);
		for (var i = 0; i < 10; i++)
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
		var storyteller = CreateStoryteller();
		var title = "Old title";
		var details = "Old details";
		var situation = new Mock<IAIStorytellerSituation>();
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

		var result = storyteller.ExecuteFunctionCall("UpdateSituation",
			"""{"Id":25,"Title":"New title","Description":"Updated details"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsFalse(result.MalformedJson);
		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		situation.Verify(x => x.UpdateSituation("New title", "Updated details"), Times.Once);
	}

	[TestMethod]
	public void ExecuteFunctionCall_ResolveSituation_InvokesUpdateAndResolve()
	{
		var storyteller = CreateStoryteller();
		var title = "Before";
		var details = "Before details";
		var resolved = false;
		var situation = new Mock<IAIStorytellerSituation>();
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

		var result = storyteller.ExecuteFunctionCall("ResolveSituation",
			"""{"Id":26,"Title":"After","Description":"After details"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsFalse(result.MalformedJson);
		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		situation.Verify(x => x.UpdateSituation("After", "After details"), Times.Once);
		situation.Verify(x => x.Resolve(), Times.Once);
	}

	[TestMethod]
	public void ExecuteFunctionCall_ForgetMemory_RemovesMemoryFromStoryteller()
	{
		var storyteller = CreateStoryteller();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(100L);

		var memory = new Mock<IAIStorytellerCharacterMemory>();
		memory.SetupGet(x => x.Id).Returns(11L);
		memory.SetupGet(x => x.MemoryTitle).Returns("Memory title");
		memory.SetupGet(x => x.MemoryText).Returns("Memory details");
		memory.SetupGet(x => x.CreatedOn).Returns(new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc));
		memory.SetupGet(x => x.Character).Returns(character.Object);
		storyteller.RegisterLoadedMemory(memory.Object);

		var forget = storyteller.ExecuteFunctionCall("ForgetMemory", """{"Id":11}""", includeEchoTools: false);
		var forgetPayload = JsonDocument.Parse(forget.OutputJson).RootElement;

		Assert.IsTrue(forgetPayload.GetProperty("ok").GetBoolean());
		memory.Verify(x => x.Forget(), Times.Once);

		var update = storyteller.ExecuteFunctionCall("UpdateMemory",
			"""{"Id":11,"Title":"After","Details":"After details"}""",
			includeEchoTools: false);
		var updatePayload = JsonDocument.Parse(update.OutputJson).RootElement;
		Assert.IsFalse(updatePayload.GetProperty("ok").GetBoolean());
	}

	[TestMethod]
	public void ExecuteFunctionCall_CustomToolGenderConversion_ParsesAndExecutes()
	{
		var storyteller = CreateStoryteller();
		object[]? capturedArgs = null;
		var prog = new Mock<IFutureProg>();
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

		var result = storyteller.ExecuteFunctionCall("CustomGenderTool",
			"""{"SubjectGender":"male"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsTrue(payload.GetProperty("ok").GetBoolean());
		Assert.IsNotNull(capturedArgs);
		Assert.AreEqual(1, capturedArgs!.Length);
		Assert.AreEqual(Gender.Male, capturedArgs[0]);
	}

	[TestMethod]
	public void ExecuteFunctionCall_InvalidCustomToolCompileError_ReturnsError()
	{
		var storyteller = CreateStoryteller();
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.NamedParameters)
			.Returns(new List<Tuple<ProgVariableTypes, string>>
			{
				Tuple.Create(ProgVariableTypes.Text, "Cue")
			});
		prog.SetupGet(x => x.CompileError).Returns("Compile failure");
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
		storyteller.CustomToolCalls.Add(new AIStorytellerCustomToolCall("BadTool", "desc", prog.Object));

		var result = storyteller.ExecuteFunctionCall("BadTool",
			"""{"Cue":"x"}""",
			includeEchoTools: false);
		var payload = JsonDocument.Parse(result.OutputJson).RootElement;

		Assert.IsFalse(payload.GetProperty("ok").GetBoolean());
		StringAssert.Contains(payload.GetProperty("error").GetString() ?? string.Empty, "does not compile");
	}

	[TestMethod]
	public void PassHeartbeatEventToAIStorytellerForTesting_MissingApiKey_DoesNotExecuteStatusProg()
	{
		var (runtimeGame, disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
		try
		{
			var heartbeatProg = new Mock<IFutureProg>();
			heartbeatProg.SetupGet(x => x.Id).Returns(123L);
			heartbeatProg.Setup(x => x.ExecuteString(It.IsAny<object[]>()))
				.Throws(new AssertFailedException("Heartbeat status prog should not execute without API key."));

			var gameworld = CreateGameworld(
				new[] { heartbeatProg.Object },
				Array.Empty<ICharacter>());
			var model = CreateModel();
			model.HeartbeatStatus5mProgId = 123L;
			var storyteller = new AIStoryteller(model, gameworld.Object);

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
		var (runtimeGame, disposeRuntimeGame) = EnsureRuntimeGameWithMissingApiKey();
		try
		{
			var storyteller = CreateStoryteller();
			var cell = new Mock<ICell>();
			var emote = new Mock<IEmoteOutput>();
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

	private static AIStoryteller CreateStoryteller()
	{
		var gameworld = CreateGameworld(Array.Empty<IFutureProg>(), Array.Empty<ICharacter>());
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
			SystemPrompt = "System prompt",
			AttentionAgentPrompt = "Attention prompt",
			SurveillanceStrategyDefinition = string.Empty,
			ReasoningEffort = "2",
			CustomToolCallsDefinition = "<ToolCalls />",
			SubscribeToRoomEvents = false,
			SubscribeTo5mHeartbeat = false,
			SubscribeTo10mHeartbeat = false,
			SubscribeTo30mHeartbeat = false,
			SubscribeToHourHeartbeat = false,
			IsPaused = false
		};
	}

	private static Mock<IFuturemud> CreateGameworld(IEnumerable<IFutureProg> progs, IEnumerable<ICharacter> characters)
	{
		var progList = progs.ToList();
		var characterList = characters.ToList();

		var progRepo = BuildRepository(progList);
		var characterRepo = BuildRepository(characterList);
		var saveManager = new Mock<ISaveManager>();

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progRepo.Object);
		gameworld.SetupGet(x => x.Characters).Returns(characterRepo.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			.Returns((long id, bool _) => characterList.FirstOrDefault(x => x.Id == id));
		return gameworld;
	}

	private static (Futuremud RuntimeGame, bool DisposeRuntimeGame) EnsureRuntimeGameWithMissingApiKey()
	{
		var existing = Futuremud.Games.OfType<Futuremud>().FirstOrDefault();
		if (existing is not null)
		{
			existing.UpdateStaticConfiguration("GPT_Secret_Key", string.Empty);
			return (existing, false);
		}

		var created = new Futuremud(null);
		created.UpdateStaticConfiguration("GPT_Secret_Key", string.Empty);
		return (created, true);
	}

	private static Mock<IUneditableAll<T>> BuildRepository<T>(IEnumerable<T> items) where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var byId = list.ToDictionary(x => x.Id, x => x);
		var repo = new Mock<IUneditableAll<T>>();
		repo.Setup(x => x.Get(It.IsAny<long>()))
			.Returns((long id) => byId.TryGetValue(id, out var value) ? value : null!);
		repo.Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		repo.SetupGet(x => x.Count).Returns(list.Count);
		return repo;
	}
}

#nullable enable
#pragma warning disable OPENAI001
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.RPG.AIStorytellers;
using OpenAI.Responses;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AIStorytellerRuntimeTests
{
	private sealed class Clock : TimeProvider
	{
		public DateTimeOffset Now = DateTimeOffset.Parse("2026-10-28T00:00:00Z");
		public override DateTimeOffset GetUtcNow() => Now;
		private readonly List<ManualTimer> _timers = [];
		public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
		{
			var timer = new ManualTimer(this, callback, state, Now + dueTime);
			_timers.Add(timer);
			return timer;
		}
		public void Advance(TimeSpan duration)
		{
			Now += duration;
			foreach (var timer in _timers.ToArray()) timer.FireIfDue();
		}
		private sealed class ManualTimer(Clock clock, TimerCallback callback, object? state, DateTimeOffset due) : ITimer
		{
			private bool _disposed;
			public bool Change(TimeSpan dueTime, TimeSpan period) { due = clock.Now + dueTime; return !_disposed; }
			public void Dispose() => _disposed = true;
			public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
			public void FireIfDue() { if (!_disposed && clock.Now >= due) { _disposed = true; callback(state); } }
		}
	}

	private sealed class Adapter : IStorytellerRequestAdapter
	{
		public readonly List<JsonObject> Requests = [];
		public readonly Queue<Task<BinaryData>> Responses = [];
		public CancellationToken LastToken;
		public Task<BinaryData> SendAsync(string key, BinaryData request, CancellationToken token)
		{
			LastToken = token;
			Requests.Add(JsonNode.Parse(request.ToString())!.AsObject());
			return Responses.Count > 0 ? Responses.Dequeue() : Task.FromResult(Response("completed", "[]"));
		}
	}

	private static BinaryData Response(string status, string output, string extra = "") => BinaryData.FromString(
		$"{{\"status\":\"{status}\",\"output\":{output}{extra}}}");
	private const string Noop = """[{"type":"function_call","call_id":"c1","name":"Noop","arguments":"{}"}]""";
	private static Task<BinaryData> Decision(bool interested) => Task.FromResult(Response("completed",
		new JsonArray(new JsonObject { ["type"] = "message", ["content"] = new JsonArray(new JsonObject
		{ ["type"] = "output_text", ["text"] = interested ? "{\"Decision\":\"interested\",\"Reason\":null}" : "{\"Decision\":\"ignore\",\"Reason\":null}" }) }).ToJsonString()));

	private static (AIStoryteller Storyteller, Adapter Adapter, Clock Clock) Create(int batch = 5, long id = 1)
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		world.SetupGet(x => x.HeartbeatManager).Returns(Mock.Of<MudSharp.Framework.Scheduling.IHeartbeatManager>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		var storyteller = new AIStoryteller(new MudSharp.Models.AIStoryteller
		{
			Id = id, Name = "Test", Model = "gpt-6.1-terra", TimeModel = "gpt-6.1-terra", AttentionClassifierModel = "gpt-6-luna",
			SystemPrompt = "Narrative", TimeSystemPrompt = "Time", AttentionAgentPrompt = "Classify",
			ReasoningEffort = "low", TimeReasoningEffort = "medium", AttentionClassifierReasoningEffort = "none",
			CustomToolCallsDefinition = $"<ToolCalls><ExecutionOptions version=\"1\"><BatchWindowSeconds>{batch}</BatchWindowSeconds></ExecutionOptions></ToolCalls>",
			SurveillanceStrategyDefinition = ""
		}, world.Object);
		var adapter = new Adapter();
		var clock = new Clock();
		storyteller.RequestAdapter = adapter;
		storyteller.Clock = clock;
		return (storyteller, adapter, clock);
	}

	private static bool Enqueue(AIStoryteller s, long? room = 1, bool classify = true, string text = "event", bool time = false) =>
		s.EnqueuePrompt("not-a-real-key", "Test event", text, false, time ? s.TimeSystemPrompt : s.SystemPrompt,
			time ? s.TimeModel : s.Model, time ? s.TimeReasoningEffort : s.ReasoningEffort,
			AIStoryteller.StorytellerToolProfile.EventFocused, classify, roomId: room, sharedContext: "Room context\n", timeRole: time);

	[TestMethod]
	public void Batch_FixedWindow_ClassifiesOnceAndPreservesOrder()
	{
		var (s, a, clock) = Create();
		a.Responses.Enqueue(Decision(true));
		a.Responses.Enqueue(Task.FromResult(Response("completed", Noop)));
		Assert.IsTrue(Enqueue(s, text: "first"));
		clock.Now = clock.Now.AddSeconds(4);
		Assert.IsTrue(Enqueue(s, text: "second"));
		s.ProcessPendingWork();
		Assert.AreEqual(0, a.Requests.Count);
		clock.Now = clock.Now.AddSeconds(1);
		for (var i = 0; i < 4; i++) s.ProcessPendingWork();
		Assert.AreEqual(2, a.Requests.Count);
		var text = a.Requests[0]["input"]!.ToJsonString();
		Assert.IsTrue(text.IndexOf("first", StringComparison.Ordinal) < text.IndexOf("second", StringComparison.Ordinal));
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void Batch_RoomIsolationAndImmediateTimeRole_UseScopedSettings()
	{
		var (s, a, _) = Create();
		Enqueue(s, room: 1);
		Enqueue(s, room: 2);
		Enqueue(s, room: null, classify: false, time: true);
		a.Responses.Enqueue(Task.FromResult(Response("completed", Noop)));
		s.ProcessPendingWork();
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual("medium", (string?)a.Requests[0]["reasoning"]?["effort"]);
		Assert.AreEqual(4096, (int?)a.Requests[0]["max_output_tokens"]);
		s.ProcessPendingWork();
		Assert.AreEqual(2, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void Classifier_Ignore_DoesNotRunNarrative()
	{
		var (s, a, _) = Create(0);
		a.Responses.Enqueue(Decision(false));
		Enqueue(s);
		s.ProcessPendingWork(); s.ProcessPendingWork();
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual("json_schema", (string?)a.Requests[0]["text"]?["format"]?["type"]);
		Assert.AreEqual(256, (int?)a.Requests[0]["max_output_tokens"]);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	[DataRow("incomplete")]
	[DataRow("failed")]
	public void NonCompletedResponse_DoesNotExecuteOrRetry(string status)
	{
		var (s, a, _) = Create(0);
		a.Responses.Enqueue(Task.FromResult(Response(status, Noop, ",\"incomplete_details\":{\"reason\":\"max_output_tokens\"}")));
		Enqueue(s, classify: false);
		s.ProcessPendingWork(); s.ProcessPendingWork(); s.ProcessPendingWork();
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void Pause_CancelsInFlightAndDiscardsBatches_LateResponseCannotResume()
	{
		var (s, a, _) = Create(0);
		var completion = new TaskCompletionSource<BinaryData>();
		a.Responses.Enqueue(completion.Task);
		Enqueue(s); Enqueue(s);
		s.ProcessPendingWork();
		s.Pause();
		Assert.IsTrue(a.LastToken.IsCancellationRequested);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
		completion.SetResult(Response("completed", Noop));
		s.Unpause(); s.ProcessPendingWork();
		Assert.AreEqual(1, a.Requests.Count);
	}

	[TestMethod]
	public void Batch_FlushesAtTwenty_AndExpiresWhileCollecting()
	{
		var (s, a, clock) = Create();
		for (var i = 0; i < 20; i++) Enqueue(s);
		s.ProcessPendingWork();
		Assert.AreEqual(1, a.Requests.Count);
		s.Pause(); s.Unpause();
		Enqueue(s);
		clock.Now = clock.Now.AddSeconds(121);
		s.ProcessPendingWork();
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
		Assert.AreEqual(1, a.Requests.Count);
	}

	[TestMethod]
	public void Continuation_PreservesOpaqueReasoningAndToolResults()
	{
		var (s, a, _) = Create(0);
		a.Responses.Enqueue(Task.FromResult(Response("completed", """
		[{"type":"reasoning","id":"r1","encrypted_content":"opaque","summary":[]},{"type":"function_call","call_id":"c1","name":"ShowMemory","arguments":"{\"Id\":999}"}]
		""")));
		a.Responses.Enqueue(Task.FromResult(Response("completed", Noop)));
		Enqueue(s, classify: false);
		s.ProcessPendingWork(); s.ProcessPendingWork();
		Assert.AreEqual(2, a.Requests.Count);
		var history = a.Requests[1]["input"]!.AsArray();
		Assert.IsTrue(history.Any(x => (string?)x?["encrypted_content"] == "opaque"));
		Assert.IsTrue(history.Any(x => (string?)x?["type"] == "function_call_output" && (string?)x?["call_id"] == "c1"));
		Assert.AreEqual("auto", (string?)a.Requests[1]["tool_choice"]);
	}

	[TestMethod]
	public void CacheSerialization_OnlyStableDeveloperPrefixHasBreakpoint()
	{
		var (s, a, _) = Create(0);
		Enqueue(s, classify: false);
		s.ProcessPendingWork();
		var body = a.Requests.Single();
		Assert.IsNull(body["instructions"]);
		Assert.AreEqual("30m", (string?)body["prompt_cache_options"]?["ttl"]);
		Assert.AreEqual("developer", (string?)body["input"]?[0]?["role"]);
		Assert.IsNotNull(body["input"]?[0]?["content"]?[0]?["prompt_cache_breakpoint"]);
		Assert.IsFalse(body["input"]![1]!.ToJsonString().Contains("prompt_cache_breakpoint"));
		var legacy = s.CreateRequestTemplate("gpt-5", "stable", ResponseReasoningEffortLevel.Low, 4096, false, false, AIStoryteller.StorytellerToolProfile.Full);
		Assert.IsNull(legacy["prompt_cache_options"]);
	}

	[TestMethod]
	public void Retrieval_PaginatesMetadataAndChunksEscapedContent()
	{
		using var args = JsonDocument.Parse("{\"Offset\":1,\"Limit\":1}");
		var result = AIStoryteller.BoundToolResult("ListMemoriesForPlayer", args.RootElement, new("""
		{"ok":true,"result":{"Memories":[{"Id":1,"Details":"secret"},{"Id":2,"Details":"secret"},{"Id":3}]}}
		""", false));
		var page = JsonNode.Parse(result.OutputJson)!;
		Assert.AreEqual(2, (int?)page["result"]?["NextOffset"]);
		Assert.AreEqual(2, (int?)page["result"]?["Memories"]?[0]?["Id"]);
		Assert.IsFalse(result.OutputJson.Contains("secret"));
		using var empty = JsonDocument.Parse("{}");
		var longText = new string('\n', 20000);
		var envelope = JsonSerializer.Serialize(new { ok = true, result = new { Contents = longText } });
		var chunk = AIStoryteller.BoundToolResult("ShowReferenceDocument", empty.RootElement, new(envelope, false));
		Assert.IsTrue(chunk.OutputJson.Length <= 8000);
		Assert.IsTrue((int?)JsonNode.Parse(chunk.OutputJson)?["result"]?["NextOffset"] > 0);
	}
	[TestMethod]
	public void Deadline_CancelsProviderAndReleasesQueue()
	{
		var (s, a, clock) = Create(0);
		a.Responses.Enqueue(new TaskCompletionSource<BinaryData>().Task);
		Enqueue(s, classify: false);
		s.ProcessPendingWork();
		clock.Advance(TimeSpan.FromSeconds(120));
		s.ProcessPendingWork();
		Assert.IsTrue(a.LastToken.IsCancellationRequested);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void IncompleteAndDuplicateCall_DoNotReplayWorldMutation()
	{
		var (s, a, _) = Create(0);
		var situation = new Mock<IAIStorytellerSituation>();
		situation.SetupGet(x => x.Id).Returns(9);
		s.RegisterLoadedSituation(situation.Object);
		const string update = """[{"type":"function_call","call_id":"update1","name":"UpdateSituation","arguments":"{\"Id\":9,\"Title\":\"New\",\"Description\":\"Detail\"}"}]""";
		a.Responses.Enqueue(Task.FromResult(Response("completed", update)));
		a.Responses.Enqueue(Task.FromResult(Response("completed", update)));
		Enqueue(s, classify: false);
		s.ProcessPendingWork(); s.ProcessPendingWork(); s.ProcessPendingWork();
		situation.Verify(x => x.UpdateSituation("New", "Detail"), Times.Once);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
		a.Responses.Enqueue(Task.FromResult(Response("incomplete", update)));
		Enqueue(s, classify: false);
		s.ProcessPendingWork(); s.ProcessPendingWork();
		situation.Verify(x => x.UpdateSituation("New", "Detail"), Times.Once);
	}

	[TestMethod]
	public void Retrieval_CustomArgumentsDoNotAcquirePagingSemantics()
	{
		using var args = JsonDocument.Parse("{\"Offset\":-1,\"Limit\":1000}");
		var result = new AIStoryteller.ToolExecutionResult("{\"ok\":true,\"result\":{\"Value\":42}}", false);
		Assert.AreEqual(result, AIStoryteller.BoundToolResult("Custom", args.RootElement, result));
	}

	[TestMethod]
	public void Usage_ExposesNumbersWithoutNarrativePayload()
	{
		var (s, a, _) = Create(0);
		a.Responses.Enqueue(Task.FromResult(Response("completed", Noop,
			",\"usage\":{\"input_tokens\":1234,\"input_tokens_details\":{\"cached_tokens\":500,\"cache_write_tokens\":300},\"output_tokens\":200,\"output_tokens_details\":{\"reasoning_tokens\":100}}")));
		Enqueue(s, classify: false, text: "PRIVATE NARRATIVE");
		s.ProcessPendingWork(); s.ProcessPendingWork();
		var usage = s.ShowUsage(Mock.Of<ICharacter>());
		StringAssert.Contains(usage, "500");
		StringAssert.Contains(usage, "300");
		Assert.IsFalse(usage.Contains("PRIVATE NARRATIVE"));
	}

	[TestMethod]
	public void Batch_SizeBoundaryFlushesAndOversizedEventIsRejected()
	{
		var (s, a, _) = Create();
		Assert.IsTrue(Enqueue(s, text: new string('x', 13000)));
		Assert.IsTrue(Enqueue(s, text: new string('y', 13000)));
		s.ProcessPendingWork();
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual(2, s.PendingStorytellerWorkCount);
		Assert.IsFalse(Enqueue(s, text: new string('z', 25000)));
	}

	[TestMethod]
	public void Refusal_WithToolCall_DoesNotMutateOrRetry()
	{
		var (s, a, _) = Create(0);
		var situation = new Mock<IAIStorytellerSituation>();
		situation.SetupGet(x => x.Id).Returns(9);
		s.RegisterLoadedSituation(situation.Object);
		a.Responses.Enqueue(Task.FromResult(Response("completed", """
		[{"type":"message","content":[{"type":"refusal","refusal":"Cannot comply"}]},{"type":"function_call","call_id":"update1","name":"UpdateSituation","arguments":"{\"Id\":9,\"Title\":\"New\",\"Description\":\"Detail\"}"}]
		""")));
		Enqueue(s, classify: false);
		s.ProcessPendingWork(); s.ProcessPendingWork(); s.ProcessPendingWork();
		situation.Verify(x => x.UpdateSituation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void Delete_CancelsActiveRunAndRejectsLateAndNewWork()
	{
		var (s, a, _) = Create(0, id: 0); // Unsaved identity avoids a database operation.
		var completion = new TaskCompletionSource<BinaryData>();
		a.Responses.Enqueue(completion.Task);
		Enqueue(s); Enqueue(s);
		s.ProcessPendingWork();
		s.Delete();
		Assert.IsTrue(a.LastToken.IsCancellationRequested);
		completion.SetResult(Response("completed", Noop));
		s.ProcessPendingWork();
		Assert.IsFalse(Enqueue(s));
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void ToolCallBudget_RejectsOversizedBatchBeforeAnyMutation()
	{
		var (s, a, _) = Create(0);
		var situation = new Mock<IAIStorytellerSituation>();
		situation.SetupGet(x => x.Id).Returns(9);
		s.RegisterLoadedSituation(situation.Object);
		var calls = new JsonArray(Enumerable.Range(1, 65).Select(i => (JsonNode)new JsonObject
		{
			["type"] = "function_call", ["call_id"] = $"update{i}", ["name"] = "UpdateSituation",
			["arguments"] = "{\"Id\":9,\"Title\":\"New\",\"Description\":\"Detail\"}"
		}).ToArray());
		a.Responses.Enqueue(Task.FromResult(Response("completed", calls.ToJsonString())));
		Enqueue(s, classify: false);
		s.ProcessPendingWork(); s.ProcessPendingWork();
		situation.Verify(x => x.UpdateSituation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void RoundBudget_StopsBeforeSeventeenthRequest()
	{
		var (s, a, _) = Create(0);
		for (var i = 0; i < 17; i++)
			a.Responses.Enqueue(Task.FromResult(Response("completed", new JsonArray(new JsonObject
			{
				["type"] = "function_call", ["call_id"] = $"read{i}", ["name"] = "ShowMemory", ["arguments"] = "{\"Id\":999}"
			}).ToJsonString())));
		Enqueue(s, classify: false);
		for (var i = 0; i < 20; i++) s.ProcessPendingWork();
		Assert.AreEqual(16, a.Requests.Count);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
	}

	[TestMethod]
	public void OversizedContinuation_StopsWithoutResendingHistory()
	{
		var (s, a, _) = Create(0);
		var output = new JsonArray(
			new JsonObject { ["type"] = "reasoning", ["encrypted_content"] = new string('x', 128000) },
			new JsonObject { ["type"] = "function_call", ["call_id"] = "read1", ["name"] = "ShowMemory", ["arguments"] = "{\"Id\":999}" });
		a.Responses.Enqueue(Task.FromResult(Response("completed", output.ToJsonString())));
		Enqueue(s, classify: false);
		s.ProcessPendingWork(); s.ProcessPendingWork();
		Assert.AreEqual(1, a.Requests.Count);
		Assert.AreEqual(0, s.PendingStorytellerWorkCount);
		using var args = JsonDocument.Parse("{}");
		var result = AIStoryteller.BoundToolResult("Custom", args.RootElement, new(JsonSerializer.Serialize(
			new { ok = true, result = new { Value = new string('x', 9000) } }), false));
		Assert.IsFalse((bool)JsonNode.Parse(result.OutputJson)!["ok"]!);
		Assert.IsTrue(result.OutputJson.Length < 8000);
	}

}

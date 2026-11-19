#nullable enable
#pragma warning disable OPENAI001
using System;
using System.Collections.Generic;
using System.ClientModel.Primitives;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Editor;
using MudSharp.Framework;
using MudSharp.OpenAI;
using MudSharp.PerceptionEngine;
using MudSharp_Unit_Tests;
using OpenAI;
using OpenAI.Responses;

namespace MudSharpCore_Unit_Tests;

[TestClass]
public class OpenAIHandlerTests
{
	[TestMethod]
	public async Task RunGptRequestAsync_Success_InvokesSuccessCallback()
	{
		string? response = null;
		Exception? failure = null;

		await OpenAIHandler.RunGptRequestAsync(_ => Task.FromResult("response"),
			text => response = text,
			exception => failure = exception,
			TimeSpan.FromSeconds(1));

		Assert.AreEqual("response", response);
		Assert.IsNull(failure);
	}

	[TestMethod]
	public async Task RunGptRequestAsync_RequestThrows_InvokesErrorCallback()
	{
		bool successCalled = false;
		Exception? failure = null;

		await OpenAIHandler.RunGptRequestAsync<string>(_ => throw new InvalidOperationException("failure"),
			_ => successCalled = true,
			exception => failure = exception,
			TimeSpan.FromSeconds(1));

		Assert.IsFalse(successCalled);
		Assert.IsInstanceOfType(failure, typeof(InvalidOperationException));
	}

	[TestMethod]
	public async Task RunGptRequestAsync_RequestTimesOut_InvokesErrorCallback()
	{
		bool successCalled = false;
		Exception? failure = null;

		await OpenAIHandler.RunGptRequestAsync(
			async cancellationToken =>
			{
				await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
				return string.Empty;
			},
			_ => successCalled = true,
			exception => failure = exception,
			TimeSpan.FromMilliseconds(20));

		Assert.IsFalse(successCalled);
		Assert.IsInstanceOfType(failure, typeof(OperationCanceledException));
	}

	[TestMethod]
	public void DescribeGptFailure_NestedNetworkFailure_ReportsSecureConnectionProblem()
	{
		Exception exception = new AggregateException(
			new HttpRequestException("transport", new System.Security.Authentication.AuthenticationException("TLS")));

		string result = OpenAIHandler.DescribeGptFailure(exception, "client-reference");

		StringAssert.Contains(result, "secure network connection");
		StringAssert.Contains(result, "client-reference");
	}

	[TestMethod]
	[DataRow(401, "GPT_Secret_Key")]
	[DataRow(429, "rate limits and quota")]
	[DataRow(503, "Try again later")]
	public async Task TryGetGptModels_OpenAiHttpFailure_ReportsSafeErrorOnGameLoop(int status, string guidance)
	{
		const string credential = "expired-test-credential";
		var world = CreateWorld(credential);
		var sentRequest = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
		var providerResponse = new TaskCompletionSource<HttpResponseMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
		using var http = new HttpClient(new StubOpenAiTransport(request =>
		{
			sentRequest.TrySetResult(request.Headers.GetValues("X-Client-Request-Id").Single());
			return providerResponse.Task;
		}));
		var options = new OpenAIClientOptions
		{
			Transport = new HttpClientPipelineTransport(http),
			RetryPolicy = new ClientRetryPolicy(0)
		};
		string? error = null;
		bool successCalled = false;
		Assert.IsTrue(OpenAIHandler.TryGetGptModels(world, _ => successCalled = true, message => error = message, options));
		string clientRequestId = await sentRequest.Task.WaitAsync(TimeSpan.FromSeconds(5));
		OpenAIHandler.ProcessGptCompletions(world);
		Assert.IsFalse(successCalled);
		Assert.IsNull(error);

		const string serverRequestId = "req_test_openai";
		using var response = new HttpResponseMessage((HttpStatusCode)status)
		{
			Content = JsonContent($$$"""{"error":{"message":"Provider echoed {{{credential}}}","type":"invalid_request_error"}}""")
		};
		response.Headers.Add("x-request-id", serverRequestId);
		providerResponse.SetResult(response);
		await ProcessUntilAsync(world, () => error is not null || successCalled);

		Assert.IsFalse(successCalled);
		Assert.IsNotNull(error);
		StringAssert.Contains(error, $"HTTP status {status}");
		StringAssert.Contains(error, guidance);
		StringAssert.Contains(error, clientRequestId);
		StringAssert.Contains(error, serverRequestId);
		Assert.IsFalse(error.Contains(credential, StringComparison.Ordinal));
	}

	[TestMethod]
	public async Task TryGetGptModels_Success_DeliversNewestModelsFirst()
	{
		var world = CreateWorld("test-key");
		using var http = new HttpClient(new StubOpenAiTransport(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = JsonContent("""
				{"object":"list","data":[{"id":"older","object":"model","created":1,"owned_by":"test"},
				{"id":"newer","object":"model","created":2,"owned_by":"test"}]}
				""")
		})));
		IReadOnlyList<string>? models = null;
		string? failure = null;
		Assert.IsTrue(OpenAIHandler.TryGetGptModels(world, result => models = result, error => failure = error,
			new OpenAIClientOptions { Transport = new HttpClientPipelineTransport(http) }));
		await ProcessUntilAsync(world, () => models is not null || failure is not null);

		Assert.IsNull(failure);
		CollectionAssert.AreEqual(new[] { "newer", "older" }, models?.ToArray());
	}

	[TestMethod]
	public void GptRequests_MissingKey_DoNotQueueOrRunCallbacks()
	{
		var world = CreateWorld(" ");
		bool callbackCalled = false;
		Assert.IsFalse(OpenAIHandler.TryGetGptModels(world, _ => callbackCalled = true, _ => callbackCalled = true));
		Assert.IsFalse(OpenAIHandler.MakeGPTRequest(world, "Context", "Prompt", _ => callbackCalled = true,
			"test-model", _ => callbackCalled = true));
		Assert.IsFalse(OpenAIHandler.MakeGPTRequest(world, new MudSharp.Models.GPTThread(), "Prompt", null,
			_ => callbackCalled = true, errorCallback: _ => callbackCalled = true));
		OpenAIHandler.ProcessGptCompletions(world);
		Assert.IsFalse(callbackCalled);
	}

	[TestMethod]
	[DataRow("gpt create fresh 0.7 gpt-test")]
	[DataRow("gpt set existing model gpt-new")]
	[DataRow("gpt query existing test-prompt")]
	public void GptCommand_MissingKey_ReportsConfigurationErrorAndKeepsThreadState(string command)
	{
		using var db = new InMemoryDatabaseTestScope();
		db.Context.GPTThreads.Add(new MudSharp.Models.GPTThread { Name = "existing", Model = "gpt-old", Prompt = "test" });
		db.Context.SaveChanges();
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(CreateWorld(string.Empty));
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);

		typeof(ImplementorModule).GetMethod("GPT", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [actor.Object, command]);
		output.Verify(x => x.Send(It.Is<string>(message => message.Contains("GPT_Secret_Key")), true, false), Times.Once);
		actor.Verify(x => x.EditorMode(It.IsAny<Action<string, IOutputHandler, object[]>>(),
			It.IsAny<Action<IOutputHandler, object[]>>(), It.IsAny<double>(), It.IsAny<string?>(),
			It.IsAny<EditorOptions>(), It.IsAny<object[]?>()), Times.Never);
		Assert.AreEqual("gpt-old", db.Context.GPTThreads.Single().Model);
		Assert.IsFalse(db.Context.GPTMessages.Any());
	}

	[TestMethod]
	public async Task MakeGptRequest_ThreadAuthenticationFailure_DoesNotPersistOrCallSuccess()
	{
		using var db = new InMemoryDatabaseTestScope();
		var thread = new MudSharp.Models.GPTThread { Name = "test", Model = "gpt-test", Prompt = "test" };
		db.Context.GPTThreads.Add(thread);
		db.Context.SaveChanges();
		var world = CreateWorld("expired-test-key");
		using var http = new HttpClient(new StubOpenAiTransport(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
		{
			Content = JsonContent("""{"error":{"message":"Invalid key expired-test-key","type":"invalid_request_error"}}""")
		})));
		bool successCalled = false;
		string? error = null;
		Assert.IsTrue(OpenAIHandler.MakeGPTRequest(world, thread, "prompt", null, _ => successCalled = true,
			errorCallback: message => error = message, clientOptions: new ResponsesClientOptions { Transport = new HttpClientPipelineTransport(http) }));
		await ProcessUntilAsync(world, () => successCalled || error is not null);
		Assert.IsFalse(successCalled);
		StringAssert.Contains(error, "HTTP status 401");
		Assert.IsFalse(db.Context.GPTMessages.Any());
	}

	[TestMethod]
	[DataRow("completed", "text", true)]
	[DataRow("completed", "", false)]
	[DataRow("incomplete", "partial text", false)]
	public async Task MakeGptRequest_TextGeneration_RequiresCompletedUsableText(string status, string text, bool succeeds)
	{
		var world = CreateWorld("test-key");
		using var http = new HttpClient(new StubOpenAiTransport(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = JsonContent($$$"""
				{"id":"resp_test","object":"response","status":"{{{status}}}","output":[
				{"id":"msg_test","type":"message","status":"{{{status}}}","role":"assistant",
				"content":[{"type":"output_text","text":"{{{text}}}","annotations":[]}]}]}
				""")
		})));
		string? result = null;
		string? error = null;
		Assert.IsTrue(OpenAIHandler.MakeGPTRequest(world, "Context", "Prompt", response => result = response, "gpt-test",
			message => error = message, new ResponsesClientOptions { Transport = new HttpClientPipelineTransport(http) }));
		await ProcessUntilAsync(world, () => result is not null || error is not null);
		if (succeeds)
		{
			Assert.AreEqual(text, result);
			Assert.IsNull(error);
			return;
		}

		Assert.IsNull(result);
		StringAssert.Contains(error, "completed response with usable text");
	}

	private static IFuturemud CreateWorld(string credential)
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticConfiguration("GPT_Secret_Key")).Returns(credential);
		return world.Object;
	}

	private static async Task ProcessUntilAsync(IFuturemud world, Func<bool> isComplete)
	{
		using var deadline = new CancellationTokenSource(TimeSpan.FromSeconds(5));
		while (!isComplete())
		{
			OpenAIHandler.ProcessGptCompletions(world);
			if (!isComplete())
			{
				await Task.Delay(10, deadline.Token);
			}
		}
	}

	private static StringContent JsonContent(string json) => new(json, Encoding.UTF8, "application/json");

	private sealed class StubOpenAiTransport(Func<HttpRequestMessage, Task<HttpResponseMessage>> send) : HttpMessageHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return send(request).WaitAsync(cancellationToken);
		}
	}
}

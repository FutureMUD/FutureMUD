using Anthropic.SDK;
#nullable enable
#pragma warning disable OPENAI001
using Anthropic.SDK.Messaging;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Google;
using Mscc.GenerativeAI.Types;
using MudSharp.Character.Name;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using OpenAI;
using OpenAI.Models;
using OpenAI.Responses;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Threading;


namespace MudSharp.OpenAI;

internal static class OpenAIHandler
{
	private static readonly TimeSpan GptRequestTimeout = TimeSpan.FromSeconds(120);
	private static readonly ConditionalWeakTable<IFuturemud, ConcurrentQueue<Action>> GptCompletions = new();

	internal const string MissingGptKey = "The OpenAI API key (GPT_Secret_Key) is not configured.";

	internal static bool TryGetGptModels(IFuturemud gameworld, Action<IReadOnlyList<string>> callback,
		Action<string> errorCallback, OpenAIClientOptions? options = null)
	{
		string apiKey = gameworld.GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrWhiteSpace(apiKey))
		{
			return false;
		}

		QueueGptRequest<IReadOnlyList<string>>(gameworld, "GPT model lookup", async (clientRequestId, cancellationToken) =>
		{
			options ??= new OpenAIClientOptions();
			options.AddPolicy(new ClientRequestIdPolicy(clientRequestId), PipelinePosition.PerCall);
			OpenAIClient client = new(new ApiKeyCredential(apiKey), options);
			ClientResult<OpenAIModelCollection> models = await client.GetOpenAIModelClient()
				.GetModelsAsync(cancellationToken);
			return models.Value.OrderByDescending(x => x.CreatedAt).Select(x => x.Id).ToArray();
		}, callback, errorCallback);
		return true;
	}

	internal static void ProcessGptCompletions(IFuturemud gameworld)
	{
		if (!GptCompletions.TryGetValue(gameworld, out var completions))
		{
			return;
		}

		while (completions.TryDequeue(out var complete))
		{
			complete();
		}
	}

    public static bool MakeGeminiRequest(string context, string requestText, Action<string> callback, string model, double temperature = 0.7)
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("Gemini_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return false;
        }

        GoogleAI googleAi = new(apiKey);
        GenerativeModel api = googleAi.GenerativeModel(model,
            generationConfig: new GenerationConfig
            {
                Temperature = Convert.ToSingle(temperature)
            },
            safetySettings:
            [
                new SafetySetting{Category = HarmCategory.HarmCategoryUnspecified, Threshold = HarmBlockThreshold.BlockNone},
                new SafetySetting{Category = HarmCategory.HarmCategoryDangerousContent, Threshold = HarmBlockThreshold.BlockNone},
                new SafetySetting{Category = HarmCategory.HarmCategoryHarassment, Threshold = HarmBlockThreshold.BlockNone},
                new SafetySetting{Category = HarmCategory.HarmCategoryHateSpeech, Threshold = HarmBlockThreshold.BlockNone},
                new SafetySetting{Category = HarmCategory.HarmCategorySexuallyExplicit, Threshold = HarmBlockThreshold.BlockNone},
            ],
            systemInstruction: new Content(context)
            {
                Parts = [new Part { Text = context }]
            });
        ChatSession chat = api.StartChat();
#if DEBUG
        Futuremud.Games.First().SystemMessage($"Gemini Request:\n\n{context}\n\n{requestText}", true);
#endif
        $"#CGemini Request#0:\n\n#3{context}#0\n\n#2{requestText}#0".WriteLineConsole();
        Task task = Task.Run(async () =>
        {
            try
            {
                //var result = await chat.SendMessage(requestText);
                GenerateTextResponse result = await api.GenerateText(requestText);
                $"#CGemini Response#0\n\n{result.Text}".WriteLineConsole();
                callback(result.Text ?? string.Empty);
            }
            catch (BlockedPromptException e)
            {
                Futuremud.Games.First().SystemMessage($"BlockedPromptException in Gemini request:\n\n{e.Message}", true);
            }
            catch (ArgumentNullException e)
            {
                Futuremud.Games.First().SystemMessage($"ArgumentNullException in Gemini request:\n\n{e.Message}", true);
            }
            catch (StopCandidateException e)
            {
                Futuremud.Games.First().SystemMessage($"StopCandidateException in Gemini request:\n\n{e.Message}", true);
            }
            catch (Exception e)
            {
                Futuremud.Games.First().SystemMessage($"Exception in Gemini request:\n\n{e.Message}", true);
            }
        });
        return true;
    }

    public static bool MakeAnthropicRequest(string context, string requestText, Action<string> callback, string model = "claude-3-5-sonnet-20240620", double temperature = 0.7)
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("Anthropic_API_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return false;
        }
        AnthropicClient client = new(apiKey);
        List<Anthropic.SDK.Messaging.Message> messages = new()
        {
            new(RoleType.User, context),
            new(RoleType.User, requestText)
        };
        MessageParameters parameters = new()
        {
            Messages = messages,
            Model = model,
            Temperature = (decimal)temperature,
            Stream = false,
            MaxTokens = 2048
        };

#if DEBUG
        Futuremud.Games.First().SystemMessage($"Anthropic Request:\n\n{context}\n\n{requestText}", true);
#endif
        $"#CAnthropic Request#0:\n\n#3{context}#0\n\n#2{requestText}#0".WriteLineConsole();
        Task task = Task.Run(async () =>
        {
            MessageResponse result = await client.Messages.GetClaudeMessageAsync(parameters);
            $"#CAnthropic Response#0\n\n{result}".WriteLineConsole();
            callback(result.FirstMessage.ToString());
        });
        return true;
    }

	public static bool MakeGPTRequest(IFuturemud gameworld, string context, string requestText, Action<string> callback, string model,
		Action<string>? errorCallback = null, ResponsesClientOptions? clientOptions = null)
    {
        string apiKey = gameworld.GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

#if DEBUG
		gameworld.SystemMessage($"GPT Request:\n\n{context}\n\n{requestText}", true);
#endif
		QueueGptRequest(gameworld, "GPT request", async (clientRequestId, cancellationToken) =>
		{
			$"#CGPT Request [{clientRequestId}]#0:\n\n#3{context}#0\n\n#2{requestText}#0".WriteLineConsole();

			ResponsesClient client = CreateResponsesClient(apiKey, clientRequestId, clientOptions);
			CreateResponseOptions options = new(model,
			[
				ResponseItem.CreateUserMessageItem(requestText)
			])
			{
				Instructions = context,
				StoredOutputEnabled = false,
				Temperature = SupportsTemperature(model) ? 0.7f : null,
				ReasoningOptions = model.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase)
					? new ResponseReasoningOptions
					{
						ReasoningEffortLevel = ResponseReasoningEffortLevel.Low
					}
					: null
			};
			ClientResult<ResponseResult> result = await client.CreateResponseAsync(options, cancellationToken);
			string serverRequestId = GetServerRequestId(result);
			string responseText = GetRequiredOutputText(result);
			$"#CGPT Response [{clientRequestId}; server {serverRequestId}]#0\n\n{responseText}".WriteLineConsole();
			return responseText;
		}, callback, errorCallback);

        return true;
    }

    public static bool MakeGPTRequest(IFuturemud gameworld, Models.GPTThread thread, string messageText, ICharacter? character,
        Action<string> callback, int maximumHistory = -1, bool includeExtraContext = true,
		Action<string>? errorCallback = null, ResponsesClientOptions? clientOptions = null)
    {
        string apiKey = gameworld.GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return false;
        }

        string prompt = thread.Prompt;
        if (includeExtraContext && character is not null)
        {
            prompt = $"{thread.Prompt}. The time is {character.Location.DateTime().ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Immortal)}. The person you are talking to is called {character.PersonalName.GetName(NameStyle.FullName)} and they are described as {character.HowSeen(character, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee)}. They are at a location called {character.Location.HowSeen(character, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee)}.";
        }

        List<ResponseItem> chatHistory = [];

        GPTMessage[] messages = maximumHistory == -1
            ? thread.Messages.ToArray()
            : thread.Messages.TakeLast(maximumHistory).ToArray();
        long? characterIdentityId = character is null ? null : CharacterInstanceIdentityComparer.IdentityId(character);
        foreach (GPTMessage message in messages)
        {
            if (message.CharacterId != characterIdentityId)
            {
                continue;
            }

            chatHistory.Add(ResponseItem.CreateUserMessageItem(message.Message));
            chatHistory.Add(ResponseItem.CreateAssistantMessageItem(message.Response, []));
        }

        chatHistory.Add(ResponseItem.CreateUserMessageItem(messageText));
		long threadId = thread.Id;
		string model = thread.Model;
		double temperature = thread.Temperature;

		QueueGptRequest(gameworld, $"GPT thread #{threadId:N0}", async (clientRequestId, cancellationToken) =>
		{
			$"#CGPT Thread Request [{clientRequestId}]#0: thread #{threadId:N0}, model {model}".WriteLineConsole();
			ResponsesClient client = CreateResponsesClient(apiKey, clientRequestId, clientOptions);
			CreateResponseOptions options = new(model, chatHistory)
			{
				Instructions = prompt,
				StoredOutputEnabled = false,
				Temperature = SupportsTemperature(model) ? (float)temperature : null
			};
			ClientResult<ResponseResult> result = await client.CreateResponseAsync(options, cancellationToken);
			string responseText = GetRequiredOutputText(result);
			string serverRequestId = GetServerRequestId(result);
			$"#CGPT Thread Response [{clientRequestId}; server {serverRequestId}]#0".WriteLineConsole();
			return responseText;
		}, responseText =>
		{
			using (new FMDB())
			{
				FMDB.Context.GPTMessages.Add(new GPTMessage
				{
					GPTThreadId = threadId,
					Message = messageText,
					Response = responseText,
					CharacterId = characterIdentityId
				});
				FMDB.Context.SaveChanges();
			}

			callback(responseText);
		}, errorCallback);
        return true;
    }

	private static void QueueGptRequest<T>(IFuturemud gameworld, string context,
		Func<string, CancellationToken, Task<T>> request, Action<T> callback, Action<string>? errorCallback)
	{
		var completions = GptCompletions.GetOrCreateValue(gameworld);
		string clientRequestId = Guid.NewGuid().ToString("N");

		void ReportFailure(Exception exception)
		{
			HandleGptFailure(gameworld, exception, context, clientRequestId, errorCallback);
		}

		_ = Task.Run(() => RunGptRequestAsync(token => request(clientRequestId, token),
			result => completions.Enqueue(() =>
			{
				try
				{
					callback(result);
				}
				catch (Exception exception)
				{
					ReportFailure(exception);
				}
			}), exception => completions.Enqueue(() => ReportFailure(exception)), GptRequestTimeout));
	}

	internal static async Task RunGptRequestAsync<T>(Func<CancellationToken, Task<T>> request,
		Action<T> callback, Action<Exception> errorCallback, TimeSpan timeout)
	{
		using CancellationTokenSource cancellationTokenSource = new(timeout);
		try
		{
			T response = await request(cancellationTokenSource.Token).ConfigureAwait(false);
			callback(response);
		}
		catch (Exception e)
		{
			errorCallback(e);
		}
	}

	internal static string DescribeGptFailure(Exception exception, string? clientRequestId = null)
	{
		string reference = clientRequestId is null ? string.Empty : $" Request reference: {clientRequestId}.";
		string serverRequestId = GetServerRequestId(exception);
		if (serverRequestId != "unavailable")
		{
			reference += $" OpenAI request ID: {serverRequestId}.";
		}

		if (FindException<ClientResultException>(exception) is { Status: > 0 } clientException)
		{
			string detail = clientException.Status switch
			{
				401 => " OpenAI rejected the API key. Check the GPT_Secret_Key configuration.",
				403 => " OpenAI denied access. Check the API key permissions and model access.",
				404 => " OpenAI could not find the requested resource. Check the configured model.",
				429 => " Check the OpenAI rate limits and quota.",
				>= 500 => " OpenAI is having a service problem. Try again later.",
				_ => string.Empty
			};
			return $"The GPT request failed with HTTP status {clientException.Status}.{detail}{reference}";
		}

		if (FindException<OperationCanceledException>(exception) is not null)
		{
			return $"The GPT request timed out.{reference}";
		}

		if (FindException<HttpRequestException>(exception) is not null ||
		    FindException<AuthenticationException>(exception) is not null)
		{
			return $"The GPT request could not establish a secure network connection.{reference}";
		}

		if (FindException<UnusableGptResponseException>(exception) is not null)
		{
			return $"OpenAI did not return a completed response with usable text.{reference}";
		}

		return $"The GPT request could not be completed.{reference}";
	}

	private static ResponsesClient CreateResponsesClient(string apiKey, string clientRequestId, ResponsesClientOptions? options)
	{
		options ??= new ResponsesClientOptions();
		options.AddPolicy(new ClientRequestIdPolicy(clientRequestId), PipelinePosition.PerCall);
		return new ResponsesClient(new ApiKeyCredential(apiKey), options);
	}

	private static bool SupportsTemperature(string model)
	{
		return !model.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase) &&
		       !model.StartsWith("o1", StringComparison.OrdinalIgnoreCase) &&
		       !model.StartsWith("o3", StringComparison.OrdinalIgnoreCase) &&
		       !model.StartsWith("o4", StringComparison.OrdinalIgnoreCase);
	}

	private static string GetRequiredOutputText(ClientResult<ResponseResult> result)
	{
		string? responseText = result.Value.GetOutputText();
		if (result.Value.Status != ResponseStatus.Completed || string.IsNullOrWhiteSpace(responseText))
		{
			throw new UnusableGptResponseException(GetServerRequestId(result));
		}

		return responseText;
	}

	private sealed class UnusableGptResponseException(string serverRequestId) : Exception
	{
		public string ServerRequestId { get; } = serverRequestId;
	}

	private static void HandleGptFailure(IFuturemud gameworld, Exception exception, string context, string clientRequestId,
		Action<string>? errorCallback = null)
	{
		string message = DescribeGptFailure(exception, clientRequestId);
		$"#2GPT Error#0 {context}: {message} ({exception.GetType().Name})\n{exception.StackTrace}".WriteLineConsole();
		try
		{
			gameworld.DiscordConnection?.NotifyAdmins(
				ExternalIntegrationAlertHelper.BuildSafeGptErrorAlert(exception,
					$"{context}; client request {clientRequestId}; OpenAI request {GetServerRequestId(exception)}"));
		}
		catch
		{
			// A notification failure must not prevent delivery to the original caller.
		}

		try
		{
			errorCallback?.Invoke(message);
		}
		catch (Exception callbackException)
		{
			$"#2GPT Error#0 {context}: Failure callback threw {callbackException.GetType().Name}. {message}"
				.WriteLineConsole();
		}
	}

	private static string GetServerRequestId(ClientResult result)
	{
		return GetServerRequestId(result.GetRawResponse());
	}

	private static string GetServerRequestId(Exception exception)
	{
		if (FindException<UnusableGptResponseException>(exception) is { } unusableResponse)
		{
			return unusableResponse.ServerRequestId;
		}

		return GetServerRequestId(FindException<ClientResultException>(exception)?.GetRawResponse());
	}

	private static string GetServerRequestId(PipelineResponse? response)
	{
		return response?.Headers?.TryGetValue("x-request-id", out string? requestId) == true &&
		       requestId is { Length: > 0 and <= 128 } && requestId.All(x => char.IsAsciiLetterOrDigit(x) || x is '_' or '-')
			? requestId
			: "unavailable";
	}

	private static TException? FindException<TException>(Exception exception) where TException : Exception
	{
		if (exception is TException matchingException)
		{
			return matchingException;
		}

		if (exception is AggregateException aggregateException)
		{
			return aggregateException.Flatten().InnerExceptions
				.Select(FindException<TException>)
				.FirstOrDefault(x => x is not null);
		}

		return exception.InnerException is null
			? null
			: FindException<TException>(exception.InnerException);
	}

	private sealed class ClientRequestIdPolicy(string clientRequestId) : PipelinePolicy
	{
		public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline,
			int currentIndex)
		{
			message.Request.Headers.Set("X-Client-Request-Id", clientRequestId);
			ProcessNext(message, pipeline, currentIndex);
		}

		public override ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline,
			int currentIndex)
		{
			message.Request.Headers.Set("X-Client-Request-Id", clientRequestId);
			return ProcessNextAsync(message, pipeline, currentIndex);
		}
	}
}

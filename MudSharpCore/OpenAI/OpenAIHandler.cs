using Anthropic.SDK;
#nullable enable
#pragma warning disable OPENAI001
using Anthropic.SDK.Messaging;
using Google.Protobuf;
using Microsoft.EntityFrameworkCore;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Google;
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
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;


namespace MudSharp.OpenAI;

internal static class OpenAIHandler
{
	private static readonly TimeSpan GptRequestTimeout = TimeSpan.FromSeconds(120);

    public static async Task<IEnumerable<string>> GPTModels()
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return Enumerable.Empty<string>();
        }

        OpenAIClient client = new(apiKey);
        ClientResult<OpenAIModelCollection> models = await client.GetOpenAIModelClient().GetModelsAsync();
        return models.Value
                     .OrderByDescending(x => x.CreatedAt)
                     .Select(x => x.Id)
                     .ToArray();
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
                Parts = [new TextData { Text = context }]
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

    public static bool MakeGPTRequest(string context, string requestText, Action<string> callback, string model,
		Action<string>? errorCallback = null)
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return false;
        }

        string clientRequestId = Guid.NewGuid().ToString("N");
		_ = Task.Run(() => RunGptRequestAsync(async cancellationToken =>
		{
#if DEBUG
            Futuremud.Games.First().SystemMessage($"GPT Request:\n\n{context}\n\n{requestText}", true);
#endif
            $"#CGPT Request [{clientRequestId}]#0:\n\n#3{context}#0\n\n#2{requestText}#0".WriteLineConsole();

			ResponsesClient client = CreateResponsesClient(apiKey, clientRequestId);
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
			string responseText = GetRequiredOutputText(result.Value);
			$"#CGPT Response [{clientRequestId}; server {serverRequestId}]#0\n\n{responseText}".WriteLineConsole();
			return responseText;
		}, callback, e => HandleGptFailure(e, "GPT request", clientRequestId, errorCallback), GptRequestTimeout));

        return true;
    }

    public static bool MakeGPTRequest(Models.GPTThread thread, string messageText, ICharacter? character,
        Action<string> callback, int maximumHistory = -1, bool includeExtraContext = true,
		Action<string>? errorCallback = null)
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
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
		string clientRequestId = Guid.NewGuid().ToString("N");

		_ = Task.Run(() => RunGptRequestAsync(async cancellationToken =>
		{
			$"#CGPT Thread Request [{clientRequestId}]#0: thread #{thread.Id:N0}, model {thread.Model}".WriteLineConsole();
			ResponsesClient client = CreateResponsesClient(apiKey, clientRequestId);
			CreateResponseOptions options = new(thread.Model, chatHistory)
			{
				Instructions = prompt,
				StoredOutputEnabled = false,
				Temperature = SupportsTemperature(thread.Model) ? (float)thread.Temperature : null
			};
			ClientResult<ResponseResult> result = await client.CreateResponseAsync(options, cancellationToken);
			string responseText = GetRequiredOutputText(result.Value);
			string serverRequestId = GetServerRequestId(result);
			$"#CGPT Thread Response [{clientRequestId}; server {serverRequestId}]#0".WriteLineConsole();

                using (new FMDB())
                {
                    FMDB.Context.GPTMessages.Add(new GPTMessage
                    {
                        GPTThreadId = thread.Id,
                        Message = messageText,
                        Response = responseText,
                        CharacterId = characterIdentityId
                    });
                    FMDB.Context.SaveChanges();
                }

            return responseText;
		}, callback,
			e => HandleGptFailure(e, $"GPT thread #{thread.Id:N0}", clientRequestId, errorCallback),
			GptRequestTimeout));
        return true;
    }

	internal static async Task RunGptRequestAsync(Func<CancellationToken, Task<string>> request,
		Action<string> callback, Action<Exception> errorCallback, TimeSpan timeout)
	{
		using CancellationTokenSource cancellationTokenSource = new(timeout);
		try
		{
			string response = await request(cancellationTokenSource.Token).ConfigureAwait(false);
			callback(response);
		}
		catch (Exception e)
		{
			errorCallback(e);
		}
	}

	internal static string DescribeGptFailure(Exception exception, string clientRequestId)
	{
		if (FindException<OperationCanceledException>(exception) is not null)
		{
			return $"The GPT request timed out. Request reference: {clientRequestId}.";
		}

		if (FindException<HttpRequestException>(exception) is not null ||
		    FindException<AuthenticationException>(exception) is not null)
		{
			return $"The GPT request could not establish a secure network connection. Request reference: {clientRequestId}.";
		}

		if (FindException<ClientResultException>(exception) is { Status: > 0 } clientException)
		{
			string serverRequestId = GetServerRequestId(clientException);
			string serverReference = serverRequestId == "unavailable"
				? string.Empty
				: $" OpenAI request ID: {serverRequestId}.";
			return $"The GPT request failed with HTTP status {clientException.Status:N0}. Request reference: {clientRequestId}.{serverReference}";
		}

		return $"The GPT request failed before a response was received. Request reference: {clientRequestId}.";
	}

	private static ResponsesClient CreateResponsesClient(string apiKey, string clientRequestId)
	{
		ResponsesClientOptions options = new();
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

	private static string GetRequiredOutputText(ResponseResult response)
	{
		string? responseText = response.GetOutputText();
		if (string.IsNullOrWhiteSpace(responseText))
		{
			throw new InvalidOperationException("OpenAI returned a response without any output text.");
		}

		return responseText;
	}

	private static void HandleGptFailure(Exception exception, string context, string clientRequestId,
		Action<string>? errorCallback = null)
	{
		string serverRequestId = GetServerRequestId(exception);
		$"{exception}\nClient request ID: {clientRequestId}\nOpenAI request ID: {serverRequestId}"
			.Prepend("#2GPT Error#0\n")
			.WriteLineConsole();
		try
		{
			Futuremud.Games.FirstOrDefault()?.DiscordConnection?.NotifyAdmins(
				ExternalIntegrationAlertHelper.BuildSafeGptErrorAlert(exception,
					$"{context}; client request {clientRequestId}; OpenAI request {serverRequestId}"));
		}
		catch
		{
			// Best-effort notification only. The full exception is already in the console log.
		}

		errorCallback?.Invoke(DescribeGptFailure(exception, clientRequestId));
	}

	private static string GetServerRequestId(ClientResult result)
	{
		return result.GetRawResponse()?.Headers.TryGetValue("x-request-id", out string? requestId) == true
			? requestId ?? "unavailable"
			: "unavailable";
	}

	private static string GetServerRequestId(Exception exception)
	{
		ClientResultException? clientException = FindException<ClientResultException>(exception);
		return clientException is not null &&
		       clientException.GetRawResponse()?.Headers.TryGetValue("x-request-id", out string? requestId) == true
			? requestId ?? "unavailable"
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

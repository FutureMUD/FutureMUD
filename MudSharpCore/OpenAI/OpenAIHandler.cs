using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Anthropic.SDK;
using Anthropic.SDK.Messaging;
using Mscc.GenerativeAI;
using Mscc.GenerativeAI.Google;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using OpenAI_API;
using OpenAI_API.Chat;
using APIAuthentication = OpenAI_API.APIAuthentication;

namespace MudSharp.OpenAI;

internal static class OpenAIHandler
{
	public static async Task<IEnumerable<string>> GPTModels()
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return Enumerable.Empty<string>();
		}

		var api = new OpenAIAPI(new APIAuthentication(apiKey));
		var models = await api.Models.GetModelsAsync();
		return models.Select(x => x.ModelID).ToArray();
	}

	public static bool MakeGeminiRequest(string context, string requestText, Action<string> callback, string model, double temperature = 0.7)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("Gemini_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return false;
		}

		var googleAi = new GoogleAI(apiKey);
		var api = googleAi.GenerativeModel(model, 
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
				Parts = [ new TextData{Text = context}]
			});
		var chat = api.StartChat();
#if DEBUG
		Futuremud.Games.First().SystemMessage($"Gemini Request:\n\n{context}\n\n{requestText}", true);
#endif
		$"#CGemini Request#0:\n\n#3{context}#0\n\n#2{requestText}#0".WriteLineConsole();
		var task = Task.Run(async () =>
		{
			try
			{
				//var result = await chat.SendMessage(requestText);
				var result = await api.GenerateText(requestText);
				$"#CGemini Response#0\n\n{result.Text}".WriteLineConsole();
				callback(result.Text);
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
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("Anthropic_API_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return false;
		}
		var client = new AnthropicClient(apiKey);
		var messages = new List<Anthropic.SDK.Messaging.Message>()
		{
			new(RoleType.User, context),
			new(RoleType.User, requestText)
		};
		var parameters = new Anthropic.SDK.Messaging.MessageParameters
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
		var task = Task.Run(async () =>
		{
			var result = await client.Messages.GetClaudeMessageAsync(parameters);
			$"#CAnthropic Response#0\n\n{result}".WriteLineConsole();
			callback(result.FirstMessage.ToString());
		});
		return true;
	}

	public static bool MakeGPTRequest(string context, string requestText, Action<string> callback, string model, double temperature = 0.7)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return false;
		}

		var api = new OpenAIAPI(new APIAuthentication(apiKey));
		var chat = api.Chat.CreateConversation(new ChatRequest
		{
			Model = model,
			Temperature = temperature,
		});
		chat.AppendSystemMessage(context);
		chat.AppendUserInput(requestText);
#if DEBUG
		Futuremud.Games.First().SystemMessage($"GPT Request:\n\n{context}\n\n{requestText}", true);
#endif
		$"#CGPT Request#0:\n\n#3{context}#0\n\n#2{requestText}#0".WriteLineConsole();
		var task = Task.Run(async () =>
		{
			var result = await chat.GetResponseFromChatbotAsync();
			$"#CGPT Response#0\n\n{result}".WriteLineConsole();
			callback(result);
		});
		return true;
	}

	public static bool MakeGPTRequest(Models.GPTThread thread, string messageText, ICharacter character,
		Action<string> callback, int maximumHistory = -1, bool includeExtraContext = true)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return false;
		}

		var api = new OpenAIAPI(new APIAuthentication(apiKey));
		var chat = api.Chat.CreateConversation(new ChatRequest
		{
			Model = thread.Model,
			Temperature = thread.Temperature,
		});
		string prompt = thread.Prompt;
		if (includeExtraContext && character is not null)
		{
			prompt = $"{thread.Prompt}. The time is {character.Location.DateTime().ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Immortal)}. The person you are talking to is called {character.PersonalName.GetName(NameStyle.FullName)} and they are described as {character.HowSeen(character, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee)}. They are at a location called {character.Location.HowSeen(character, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee)}.";
		}
		chat.AppendSystemMessage(prompt);
		var messages = maximumHistory == -1
			? thread.Messages.ToArray()
			: thread.Messages.TakeLast(maximumHistory).ToArray();
		foreach (var message in messages)
		{
			if (message.CharacterId != character?.Id)
			{
				continue;
			}

			chat.AppendUserInput(message.Message);
			chat.AppendExampleChatbotOutput(message.Response);
		}
		chat.AppendMessage(ChatMessageRole.User, messageText);
		var task = Task.Run(async () =>
		{
			var result = await chat.GetResponseFromChatbotAsync();
			using (new FMDB())
			{
				FMDB.Context.GPTMessages.Add(new GPTMessage
				{
					GPTThreadId = thread.Id,
					Message = messageText,
					Response = result,
					CharacterId = character?.Id
				});
				FMDB.Context.SaveChanges();
			}
				
			callback(result);
		});
		return true;
	}
}
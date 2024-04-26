using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using OpenAI_API;
using OpenAI_API.Chat;

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
using System;
using System.ClientModel.Primitives;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Options;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.PerceptionEngine.Parsers;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public class AIStorytellerCustomToolCall
{
	public bool IsValid
	{
		get
		{
			if (Prog is null)
			{
				return false;
			}

			if (!string.IsNullOrEmpty(Prog.CompileError))
			{
				return false;
			}

			RecheckFunctionParameters();

			if (!Prog.NamedParameters.Select(x => x.Item2).SequenceEqual(ParameterDescriptions.Keys))
			{
				return false;
			}

			return true;
		}
	}

	private void RecheckFunctionParameters()
	{
		foreach (var parameter in Prog?.NamedParameters ?? [])
		{
			if (!ParameterDescriptions.ContainsKey(parameter.Item2))
			{
				_parameterDescriptions[parameter.Item2] = "";
			}
		}
	}

	public string Name { get; init; }
	public string Description { get; init; }
	public IFutureProg Prog { get; init; }
	private Dictionary<string, string> _parameterDescriptions;
	public IReadOnlyDictionary<string, string> ParameterDescriptions => _parameterDescriptions;

	public XElement SaveToXml(bool includeWithEcho)
	{
		return new XElement("ToolCall",
			new XElement("Name", new XCData(Name)),
			new XElement("Description", new XCData(Description)),
			new XElement("Prog", Prog.Id),
			new XElement("IncludeWithEcho", includeWithEcho),
			new XElement("ParameterDescriptions",
				from item in ParameterDescriptions
				select new XElement("Description", new XAttribute("name", item.Key), new XCData(item.Value))
			)
		);
	}

	public AIStorytellerCustomToolCall(XElement root, IFuturemud gameworld)
	{
		Name = root.Element("Name").Value;
		Description = root.Element("Description").Value;
		Prog = gameworld.FutureProgs.Get(long.Parse(root.Element("Prog").Value));
		var dictionary = new Dictionary<string, string>();
		foreach (var description in root.Element("ParameterDescriptions").Elements("Description"))
		{
			dictionary[description.Attribute("name").Value] = description.Value;
		}
		_parameterDescriptions = dictionary;
		RecheckFunctionParameters();
	}
}

public record AIStorytellerCustomToolCallParameterDefinition
{
	public string Name { get; }
	public string Type { get; }
	public string Description { get; }

	public string ToJSONElement()
	{
		return $$"""
	"{{Name.EscapeForJson()}}": {
		"type": "{{Type}}",
		"description": "{{Description.EscapeForJson()}}"
	}
""";
	}

	public AIStorytellerCustomToolCallParameterDefinition(string name, string description, ProgVariableTypes type)
	{
		Name = name;
		switch (type)
		{
			case ProgVariableTypes.Text:
				Type = "string";
				Description = description;
				break;
			case ProgVariableTypes.Number:
				Type = "double";
				Description = description;
				break;
			case ProgVariableTypes.Boolean:
				Type = "boolean";
				Description = description;
				break;
			case ProgVariableTypes.Character:
				Type = "int64";
				Description = $"The ID number of a character. {description}";
				break;
			case ProgVariableTypes.Location:
				Type = "int64";
				Description = $"The ID number of a room location. {description}";
				break;
			case ProgVariableTypes.Item:
				break;
			case ProgVariableTypes.Shard:
				break;
			case ProgVariableTypes.Error:
				break;
			case ProgVariableTypes.Gender:
				break;
			case ProgVariableTypes.Zone:
				break;
			case ProgVariableTypes.Collection:
				break;
			case ProgVariableTypes.Race:
				break;
			case ProgVariableTypes.Culture:
				break;
			case ProgVariableTypes.Chargen:
				break;
			case ProgVariableTypes.Trait:
				break;
			case ProgVariableTypes.Clan:
				break;
			case ProgVariableTypes.ClanRank:
				break;
			case ProgVariableTypes.ClanAppointment:
				break;
			case ProgVariableTypes.ClanPaygrade:
				break;
			case ProgVariableTypes.Currency:
				break;
			case ProgVariableTypes.Exit:
				break;
			case ProgVariableTypes.Literal:
				break;
			case ProgVariableTypes.DateTime:
				break;
			case ProgVariableTypes.TimeSpan:
				break;
			case ProgVariableTypes.Language:
				break;
			case ProgVariableTypes.Accent:
				break;
			case ProgVariableTypes.Merit:
				break;
			case ProgVariableTypes.MudDateTime:
				break;
			case ProgVariableTypes.Calendar:
				break;
			case ProgVariableTypes.Clock:
				break;
			case ProgVariableTypes.Effect:
				break;
			case ProgVariableTypes.Knowledge:
				break;
			case ProgVariableTypes.Role:
				break;
			case ProgVariableTypes.Ethnicity:
				break;
			case ProgVariableTypes.Drug:
				break;
			case ProgVariableTypes.WeatherEvent:
				break;
			case ProgVariableTypes.Shop:
				break;
			case ProgVariableTypes.Merchandise:
				break;
			case ProgVariableTypes.Outfit:
				break;
			case ProgVariableTypes.OutfitItem:
				break;
			case ProgVariableTypes.Project:
				break;
			case ProgVariableTypes.OverlayPackage:
				break;
			case ProgVariableTypes.Terrain:
				break;
			case ProgVariableTypes.Solid:
				break;
			case ProgVariableTypes.Liquid:
				break;
			case ProgVariableTypes.Gas:
				break;
			case ProgVariableTypes.Dictionary:
				break;
			case ProgVariableTypes.CollectionDictionary:
				break;
			case ProgVariableTypes.MagicSpell:
				break;
			case ProgVariableTypes.MagicSchool:
				break;
			case ProgVariableTypes.MagicCapability:
				break;
			case ProgVariableTypes.Bank:
				break;
			case ProgVariableTypes.BankAccount:
				break;
			case ProgVariableTypes.BankAccountType:
				break;
			case ProgVariableTypes.LegalAuthority:
				break;
			case ProgVariableTypes.Law:
				break;
			case ProgVariableTypes.Crime:
				break;
			case ProgVariableTypes.Market:
				break;
			case ProgVariableTypes.MarketCategory:
				break;
			case ProgVariableTypes.LiquidMixture:
				break;
			case ProgVariableTypes.Script:
				break;
			case ProgVariableTypes.Writing:
				break;
			case ProgVariableTypes.Area:
				break;
			case ProgVariableTypes.CollectionItem:
				break;
			case ProgVariableTypes.Perceivable:
				break;
			case ProgVariableTypes.Perceiver:
				break;
			case ProgVariableTypes.MagicResourceHaver:
				break;
			case ProgVariableTypes.ReferenceType:
				break;
			case ProgVariableTypes.ValueType:
				break;
			case ProgVariableTypes.Anything:
				break;
			case ProgVariableTypes.Toon:
				break;
			case ProgVariableTypes.Tagged:
				break;
			case ProgVariableTypes.Material:
				break;
		}
	}
}

public class AIStoryteller : SaveableItem, IAIStoryteller
{
	public override string FrameworkItemType => "AIStoryteller";

	public AIStoryteller(Models.AIStoryteller storyteller, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Id = storyteller.Id;
		_name = storyteller.Name;
		Description = storyteller.Description;
		Model = storyteller.Model;
		SystemPrompt = storyteller.SystemPrompt;
		AttentionAgentPrompt = storyteller.AttentionAgentPrompt;
		ReasoningEffort = (ResponseReasoningEffortLevel)storyteller.ReasoningEffort;
		SurveillanceStrategy = new AIStorytellerSurveillanceStrategy(gameworld, storyteller.SurveillanceStrategyDefinition);
		SubscribeToRoomEvents = storyteller.SubscribeToRoomEvents;
		SubscribeTo10mHeartbeat = storyteller.SubscribeTo10mHeartbeat;
		SubscribeTo30mHeartbeat = storyteller.SubscribeTo30mHeartbeat;
		SubscribeTo5mHeartbeat = storyteller.SubscribeTo5mHeartbeat;
		SubscribeToHourHeartbeat = storyteller.SubscribeToHourHeartbeat;
		HeartbeatStatus5mProg = gameworld.FutureProgs.Get(storyteller.HeartbeatStatus5mProgId ?? 0L);
		HeartbeatStatus10mProg = gameworld.FutureProgs.Get(storyteller.HeartbeatStatus10mProgId ?? 0L);
		HeartbeatStatus30mProg = gameworld.FutureProgs.Get(storyteller.HeartbeatStatus30mProgId ?? 0L);
		HeartbeatStatus1hProg = gameworld.FutureProgs.Get(storyteller.HeartbeatStatus1hProgId ?? 0L);
		IsPaused = storyteller.IsPaused;
		var root = XElement.Parse(storyteller.CustomToolCallsDefinition);
		foreach (var element in root.Elements("ToolCall"))
		{
			var toolCall = new AIStorytellerCustomToolCall(element, Gameworld);
			if (bool.Parse(element.Element("IncludeWithEcho").Value))
			{
				CustomToolCallsEchoOnly.Add(toolCall);
				continue;
			}
			CustomToolCalls.Add(toolCall);
		}
	}

	public string Description { get; private set; }
	public string Model { get; private set; }
	public string SystemPrompt { get; private set; }
	public string AttentionAgentPrompt { get; private set; }
	public bool SubscribeToRoomEvents { get; private set; }
	public bool SubscribeTo5mHeartbeat { get; private set; }
	public bool SubscribeTo10mHeartbeat { get; private set; }
	public bool SubscribeTo30mHeartbeat { get; private set; }
	public bool SubscribeToHourHeartbeat { get; private set; }
	public IFutureProg HeartbeatStatus5mProg { get; private set; }
	public IFutureProg HeartbeatStatus10mProg { get; private set; }
	public IFutureProg HeartbeatStatus30mProg { get; private set; }
	public IFutureProg HeartbeatStatus1hProg { get; private set; }
	public List<AIStorytellerCustomToolCall> CustomToolCalls { get; } = [];
	public List<AIStorytellerCustomToolCall> CustomToolCallsEchoOnly { get; } = [];
	public bool IsPaused { get; private set; }

	public ResponseReasoningEffortLevel ReasoningEffort { get; private set; }
	public IAIStorytellerSurveillanceStrategy SurveillanceStrategy { get; private set; }
	public IFutureProg CustomPlayerInformationProg { get; private set; }

	private readonly List<ICell> _subscribedCells = [];

	public void SubscribeEvents()
	{
		UnsubscribeEvents();
		if (SubscribeToHourHeartbeat)
		{
			Gameworld.HeartbeatManager.FuzzyHourHeartbeat += HeartbeatManager_FuzzyHourHeartbeat;
		}

		if (SubscribeTo5mHeartbeat)
		{
			Gameworld.HeartbeatManager.FuzzyFiveMinuteHeartbeat += HeartbeatManager_FiveMinuteHeartbeat;
		}

		if (SubscribeTo10mHeartbeat)
		{
			Gameworld.HeartbeatManager.FuzzyTenMinuteHeartbeat += HeartbeatManager_TenMinuteHeartbeat;
		}

		if (SubscribeTo30mHeartbeat)
		{
			Gameworld.HeartbeatManager.FuzzyThirtyMinuteHeartbeat += HeartbeatManager_ThirtyMinuteHeartbeat;
		}

		if (SubscribeToRoomEvents)
		{
			var cells = SurveillanceStrategy.GetCells(Gameworld).ToList();
			_subscribedCells.Clear();
			_subscribedCells.AddRange(cells);
			foreach (var cell in cells)
			{
				cell.OnRoomEmoteEcho += Cell_OnRoomEcho;
			}
		}
	}

	private void AddUniversalToolsToResponseOptions(CreateResponseOptions options)
	{
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "CreateSituation",
			functionDescription: "Creates a new situation for the AI Storyteller to manage",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Title": {
					  "type": "string",
					  "description": "The title of the situation"
					},
					"Description": {
					  "type": "string",
					  "description": "A description of the situation"
					}
				  },
				  "required": ["Title", "Description"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "UpdateSituation",
			functionDescription: "Updates an existing situation with fresh details",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "int32",
					  "description": "The id of the situation to update"
					},
					"Title": {
					  "type": "string",
					  "description": "The title of the situation"
					},
					"Description": {
					  "type": "string",
					  "description": "A description of the situation"
					}
				  },
				  "required": ["Id", "Title", "Description"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "ResolveSituation",
			functionDescription: "Resolves a situation and sends it to the archive",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "int32",
					  "description": "The id of the situation to resolve"
					},
					"Title": {
					  "type": "string",
					  "description": "The final title of the situation"
					},
					"Description": {
					  "type": "string",
					  "description": "A final description of the situation"
					}
				  },
				  "required": ["Id", "Title", "Description"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "Noop",
			functionDescription: "Use this tool call when it is mandatory for you to make a tool call response but you don't actually want to do anything",
			functionParameters: null,
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "OnlinePlayers",
			functionDescription: "Provides information about which players are online",
			functionParameters: null,
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "PlayerInformation",
			functionDescription: "Retrieves detailed information about a specific player character",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "long",
					  "description": "The id the player to retrieve information about"
					}
				  },
				  "required": ["Id"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "CreateMemory",
			functionDescription: "Creates a memory or fact about a particular player character",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "long",
					  "description": "The id of the character to create a memory about"
					},
					"Title": {
					  "type": "string",
					  "description": "The title of the memory"
					},
					"Details": {
					  "type": "string",
					  "description": "The details of the memory"
					}
				  },
				  "required": ["Id", "Title", "Details"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "UpdateMemory",
			functionDescription: "Creates a memory or fact about a particular player character",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "long",
					  "description": "The id of the memory to update"
					},
					"Title": {
					  "type": "string",
					  "description": "The title of the memory"
					},
					"Details": {
					  "type": "string",
					  "description": "The details of the memory"
					}
				  },
				  "required": ["Id", "Title", "Details"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "Landmarks",
			functionDescription: "Returns a collection of all the important landmark locations",
			functionParameters: null,
			strictModeEnabled: true
			));
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: "ShowLandmark",
			functionDescription: "Shows detailed information about a particular landmark",
			functionParameters: BinaryData.FromBytes(
				"""
				{
				"type": "object",
				  "properties": {
					"Id": {
					  "type": "string",
					  "description": "The name of the landmark to show information about"
					}
				  },
				  "required": ["Id"]
				}
				"""u8.ToArray()
			),
			strictModeEnabled: true
			));
	}

	private void AddCustomToolCallsToResponseOptions(CreateResponseOptions options, bool includeEchoes)
	{
		
		foreach (var toolCall in CustomToolCalls)
		{
			var sb = new StringBuilder();
			sb.AppendLine(
				$$"""
				{
				"type": "object",
				  "properties": {
				""");
			var addedAny = false;
			foreach (var parameter in toolCall.Prog.NamedParameters)
			{
				sb.Append(new AIStorytellerCustomToolCallParameterDefinition(parameter.Item2, toolCall.ParameterDescriptions[parameter.Item2], parameter.Item1).ToJSONElement());
				if (addedAny)
				{
					sb.Append(',');
				}
				addedAny = true;
				sb.AppendLine();
			}
			sb.AppendLine(@"},
""required"": [");
			sb.AppendLine(toolCall.Prog.NamedParameters.Select(x => x.Item2).ListToCommaSeparatedValues());
			sb.AppendLine(@"]
}");
			options.Tools.Add(ResponseTool.CreateFunctionTool(
				functionName: toolCall.Name,
				functionDescription: toolCall.Description,
				functionParameters: BinaryData.FromBytes(Encoding.UTF8.GetBytes(sb.ToString())
				),
				strictModeEnabled: true
			));
		}

		if (!includeEchoes)
		{
			return;
		}

		foreach (var toolCall in CustomToolCallsEchoOnly)
		{
			var sb = new StringBuilder();
			sb.AppendLine(
				$$"""
				{
				"type": "object",
				  "properties": {
				""");
			var addedAny = false;
			foreach (var parameter in toolCall.Prog.NamedParameters)
			{
				sb.Append(new AIStorytellerCustomToolCallParameterDefinition(parameter.Item2, toolCall.ParameterDescriptions[parameter.Item2], parameter.Item1).ToJSONElement());
				if (addedAny)
				{
					sb.Append(',');
				}
				addedAny = true;
				sb.AppendLine();
			}
			sb.AppendLine(@"},
""required"": [");
			sb.AppendLine(toolCall.Prog.NamedParameters.Select(x => x.Item2).ListToCommaSeparatedValues());
			sb.AppendLine(@"]
}");
			options.Tools.Add(ResponseTool.CreateFunctionTool(
				functionName: toolCall.Name,
				functionDescription: toolCall.Description,
				functionParameters: BinaryData.FromBytes(Encoding.UTF8.GetBytes(sb.ToString())
				),
				strictModeEnabled: true
			));
		}
	}

	private void PassHeartbeatEventToAIStoryteller(string heartbeatType)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		switch (heartbeatType)
		{
			case "5m":
				sb.AppendLine("Your five minute heartbeat has triggered.");
				var text = HeartbeatStatus5mProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text)) 
				{ 
					sb.AppendLine(text); 
				}
				break;
			case "10m":
				sb.AppendLine("Your ten minute heartbeat has triggered.");
				text = HeartbeatStatus10mProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text))
				{
					sb.AppendLine(text);
				}
				break;
			case "30m":
				sb.AppendLine("Your thirty minute heartbeat has triggered.");
				text = HeartbeatStatus30mProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text))
				{
					sb.AppendLine(text);
				}
				break;
			case "1h":
				sb.AppendLine("Your one hour heartbeat has triggered.");
				text = HeartbeatStatus1hProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text))
				{
					sb.AppendLine(text);
				}
				break;
		}

		ResponsesClient client = new(Model, apiKey);
		List<ResponseItem> messages = [
				ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
				ResponseItem.CreateUserMessageItem(sb.ToString()),
			];
		var options = new CreateResponseOptions(messages);
		options.ReasoningOptions.ReasoningEffortLevel = ReasoningEffort;

		AddUniversalToolsToResponseOptions(options);
		AddCustomToolCallsToResponseOptions(options, false);
		ExecuteToolCall(client, messages, options);
	}

	private void PassSituationToAIStoryteller(ICell location, PerceptionEngine.IEmoteOutput emote, string echo, string attentionReason)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		sb.AppendLine("Your attention subroutine has flagged something that has happened as relevant to you.");
		sb.AppendLine($"The relevant thing is taking place at a location called {location.HowSeen(null)}.");
		if (emote is not null && emote.DefaultSource is ICharacter ch)
		{
			sb.AppendLine($"The character that originated the thing is called {ch.PersonalName.GetName(Character.Name.NameStyle.FullName)} and their description is {ch.HowSeen(ch, flags: PerceiveIgnoreFlags.TrueDescription)}.");
		}
		sb.AppendLine("The thing that has been flagged to your attention is the following:");
		sb.AppendLine(echo.StripANSIColour().StripMXP());
		sb.AppendLine($"The attention subroutine thought that this was relevant for the following reason:");
		sb.AppendLine(attentionReason?.IfNullOrWhiteSpace("No reason provided"));

		ResponsesClient client = new(Model, apiKey);
		List<ResponseItem> messages = [
				ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
				ResponseItem.CreateUserMessageItem(sb.ToString()),
			];
		var options = new CreateResponseOptions(messages);
		options.ReasoningOptions.ReasoningEffortLevel = ReasoningEffort;

		AddUniversalToolsToResponseOptions(options);
		AddCustomToolCallsToResponseOptions(options, true);
		ExecuteToolCall(client, messages, options);
	}

	private void ExecuteToolCall(ResponsesClient client, List<ResponseItem> messages, CreateResponseOptions options)
	{
		var requiresAction = false;
		do
		{
			requiresAction = false;
			var task = Task.Run(async () =>
			{
				try
				{
					ResponseResult response = await client.CreateResponseAsync(options);
					messages.AddRange(response.OutputItems);
					foreach (ResponseItem outputItem in response.OutputItems)
					{
						if (outputItem is FunctionCallResponseItem functionCall)
						{
							using JsonDocument argumentsJson = JsonDocument.Parse(functionCall.FunctionArguments);
							switch (functionCall.FunctionName)
							{
								case "Noop":
									return;
								case "CreateSituation":
									{
										var title = argumentsJson.RootElement.GetProperty("Title").GetString();
										var description = argumentsJson.RootElement.GetProperty("Description").GetString();
										var situation = new AIStorytellers.AIStorytellerSituation(Gameworld, this, title, description);
										_situations.Add(situation);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Created new situation '{situation.Name}' with Id {situation.Id}"));
										break;
									}
								case "UpdateSituation":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt32();
										var situation = _situations.FirstOrDefault(x => x.Id == id);
										if (situation is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No situation with Id {id} found"));
											break;
										}
										var newTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var newDescription = argumentsJson.RootElement.GetProperty("Description").GetString();
										situation.UpdateSituation(newTitle, newDescription);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Updated situation '{situation.Name}' (Id {situation.Id})"));
										break;
									}
								case "ResolveSituation":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt32();
										var situation = _situations.FirstOrDefault(x => x.Id == id);
										if (situation is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No situation with Id {id} found"));
											break;
										}
										var newTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var newDescription = argumentsJson.RootElement.GetProperty("Description").GetString();
										situation.UpdateSituation(newTitle, newDescription);
										situation.Resolve();
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Resolved situation '{situation.Name}' (Id {situation.Id})"));
										break;
									}
								case "OnlinePlayers":
									var onlinePlayers = Gameworld.Characters.Where(x => x.IsPlayerCharacter).ToList();
									var sb = new StringBuilder();
									sb.Append("""{ "players": [""");
									foreach (var pc in onlinePlayers)
									{
										sb.AppendLine(
											$$"""
											{
												"Id" : {{pc.Id}},
												"Name" : "{{pc.PersonalName.GetName(NameStyle.FullName).EscapeForJson()}}",
												"Gender" : "{{pc.Gender.GenderClass().EscapeForJson()}}",
												"ShortDescription" : "{{pc.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}",
												"LocationId" : {{pc.Location?.Id ?? 0:N0}},
												"LocationName" : "{{pc.Location?.HowSeen(null, colour: false).EscapeForJson() ?? "Unknown"}}",
												"NumberOfMemories" : {{_characterMemories.Count(x => x.Character.Id == pc.Id):N0}}
											}
											"""
										);
									}
									sb.Append("]}");
									messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, sb.ToString()));
									break;
								case "PlayerInformation":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var pc = Gameworld.Characters.FirstOrDefault(x => x.IsPlayerCharacter && x.Id == id);
										if (pc is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No player character with Id {id} found"));
											break;
										}
										sb = new StringBuilder();
										sb.AppendLine("{");
										sb.AppendLine($$"""  "Id" : {{pc.Id}}, """);
										sb.AppendLine($$"""  "Name" : "{{pc.PersonalName.GetName(NameStyle.FullName).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Gender" : "{{pc.Gender.GenderClass()}}", """);
										sb.AppendLine($$"""  "Race" : "{{pc.Race.Name.EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Ethnicity" : "{{pc.Ethnicity.Name.EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Culture" : "{{pc.Culture.Name.EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Age" : {{pc.AgeInYears:N0}}, """);
										sb.AppendLine($$"""  "AgeCategory" : "{{pc.AgeCategory.DescribeEnum(true).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "Birthday" : "{{pc.Birthday.Display(TimeAndDate.Date.CalendarDisplayMode.Long).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "ShortDescription" : "{{pc.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "FullDescription" : "{{pc.HowSeen(null, type: Form.Shape.DescriptionType.Full, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}", """);
										sb.AppendLine($$"""  "LocationId" : {{pc.Location?.Id ?? 0:N0}}, """);
										sb.AppendLine($$"""  "LocationName" : "{{pc.Location?.HowSeen(null, colour: false).EscapeForJson() ?? "Unknown"}}", """);
										if (CustomPlayerInformationProg is not null)
										{
											var info = CustomPlayerInformationProg?.ExecuteDictionary<string>(pc);
											foreach (var kvp in info)
											{
												sb.AppendLine($$"""  "{{kvp.Key.EscapeForJson()}}" : "{{kvp.Value.EscapeForJson()}}", """);
											}
										}

										sb.AppendLine("""  "Memories" : [""");
										foreach (var item in _characterMemories.Where(x => x.Character.Id == pc.Id))
										{
											sb.AppendLine("    {");
											sb.AppendLine($$"""      "Id" : {{item.Id}}, """);
											sb.AppendLine($$"""      "Title" : "{{item.Name.EscapeForJson()}}", """);
											sb.AppendLine($$"""      "Details" : "{{item.MemoryText.EscapeForJson()}}", """);
											sb.AppendLine($$"""      "CreatedOn" : "{{item.CreatedOn.ToString("o")}}", """);
											sb.AppendLine("    },");
										}
										sb.AppendLine("""  ],""");
										sb.AppendLine("}");
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, sb.ToString()));
										break;
									}
								case "CreateMemory":
									{
										var id = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var player = Gameworld.TryGetCharacter(id, true);
										if (player is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No player with Id {id} found"));
											break;
										}
										var newTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var newDescription = argumentsJson.RootElement.GetProperty("Details").GetString();
										var newMemory = new AIStorytellerCharacterMemory(this, player, newTitle, newDescription);
										_characterMemories.Add(newMemory);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Created memory '{newTitle}' for character {id:N0} with memory Id {newMemory.Id:N0})"));
										break;
									}
								case "UpdateMemory":
									{
										var memoryId = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var memory = _characterMemories.FirstOrDefault(x => x.Id == memoryId);
										if (memory is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No memory with Id {memoryId} found"));
											break;
										}
										var updatedTitle = argumentsJson.RootElement.GetProperty("Title").GetString();
										var updatedDetails = argumentsJson.RootElement.GetProperty("Details").GetString();
										memory.UpdateMemory(updatedTitle, updatedDetails);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Updated memory '{updatedTitle}' (Id {memory.Id:N0})"));
										break;
									}
								case "ForgetMemory":
									{
										var memoryId = argumentsJson.RootElement.GetProperty("Id").GetInt64();
										var memory = _characterMemories.FirstOrDefault(x => x.Id == memoryId);
										if (memory is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No memory with Id {memoryId} found"));
											break;
										}

										memory.Forget();
										_characterMemories.Remove(memory);
										messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"Forgot memory '{memory.MemoryTitle}' (Id {memory.Id:N0})"));
										break;
									}
								case "Landmarks":
									{
										var landmarks = Gameworld.Cells.SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault()).ToList();
										sb = new StringBuilder();
										sb.Append("""{ "landmarks": [""");
										foreach (var item in landmarks)
										{
											var cell = (ICell)item.Owner;
											sb.AppendLine($$"""
												{
													"Id" : "{{item.Name.EscapeForJson()}}",
													"RoomId" : {{cell.Id:N0}},
													"RoomName" : "{{cell.HowSeen(null, colour: false).EscapeForJson()}}",
												}
												""");
										}
										sb.Append("""]}""");
										break;
									}
								case "ShowLandmark":
									{
										var landmarkId = argumentsJson.RootElement.GetProperty("Id").GetString();
										var landmark = Gameworld.Cells.SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
											.FirstOrDefault(x => x.Name.EqualTo(landmarkId));
										if (landmark is null)
										{
											messages.Add(ResponseItem.CreateFunctionCallOutputItem(functionCall.Id, $"No landmark with Id {landmarkId} found"));
											break;
										}

										var cell = (ICell)landmark.Owner;

										sb = new StringBuilder();

										sb.AppendLine("{");
										sb.AppendLine($$"""
													"Id" : "{{landmark.Name.EscapeForJson()}}",
													"RoomId" : { { cell.Id:N0} },
													"RoomName" : "{{cell.HowSeen(null, colour: false).EscapeForJson()}}",
													"RoomDescription" : "{{cell.ProcessedFullDescription(null, PerceiveIgnoreFlags.TrueDescription, cell.CurrentOverlay).EscapeForJson()}}",
													"Details" : "{{landmark.LandmarkDescriptionTexts.Select(x => x.Text).ListToCommaSeparatedValues("\n").StripANSIColour().EscapeForJson()}}",
													"Occupants": 
													[
													""");
										foreach (var occupant in cell.Characters)
										{
											sb.AppendLine($$"""
														{
															"Id" : {{occupant.Id:N0}},
															"Name" : "{{occupant.PersonalName.GetName(NameStyle.FullName).EscapeForJson()}}",
															"Gender" : "{{occupant.Gender.GenderClass()}}",
															"ShortDescription" : "{{occupant.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription).EscapeForJson()}}"
										}
										""");
										}
										sb.AppendLine($$"""
													]
													""");
										sb.AppendLine("}");
										break;
									}

								default:
									{
										// Handle other unexpected calls.
										break;
									}
							}

							requiresAction = true;
							break;
						}
					}
				}
				catch (Exception e)
				{
					e.ToString().Prepend("#2GPT Error#0\n").WriteLineConsole();
					Futuremud.Games.First().DiscordConnection.NotifyAdmins($"**GPT Error**\n\n```\n{e.ToString()}```");
				}
			});
		}
		while (requiresAction);
	}

	private void Cell_OnRoomEcho(ICell location, RoomLayer? layer, PerceptionEngine.IEmoteOutput emote)
	{
		if (IsPaused)
		{
			return;
		}

		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var echo = emote.ParseFor(null);


		ResponsesClient client = new(Model, apiKey);
		var options = new CreateResponseOptions([
				ResponseItem.CreateDeveloperMessageItem(AttentionAgentPrompt),
				ResponseItem.CreateUserMessageItem(echo),
			]);
		options.ReasoningOptions.ReasoningEffortLevel = ResponseReasoningEffortLevel.Minimal;
		var task = Task.Run(async () =>
		{
			try
			{
				ResponseResult attention = await client.CreateResponseAsync(options);
				var ss = new StringStack(attention.GetOutputText());
				switch (ss.Pop().ToLowerInvariant())
				{
					case "interested":
						PassSituationToAIStoryteller(location, emote, echo, ss.SafeRemainingArgument);
						return;

				}
			}
			catch (Exception e)
			{
				e.ToString().Prepend("#2GPT Error#0\n").WriteLineConsole();
				Futuremud.Games.First().DiscordConnection.NotifyAdmins($"**GPT Error**\n\n```\n{e.ToString()}```");
			}
		});
	}
	private void HeartbeatManager_FiveMinuteHeartbeat() { 
		PassHeartbeatEventToAIStoryteller("5m");
	}

	private void HeartbeatManager_TenMinuteHeartbeat()
	{
		PassHeartbeatEventToAIStoryteller("10m");
	}

	private void HeartbeatManager_ThirtyMinuteHeartbeat()
	{
		PassHeartbeatEventToAIStoryteller("30m");
	}

	private void HeartbeatManager_FuzzyHourHeartbeat()
	{
		PassHeartbeatEventToAIStoryteller("1h");
	}

	public void UnsubscribeEvents()
	{
		Gameworld.HeartbeatManager.FuzzyFiveMinuteHeartbeat -= HeartbeatManager_FiveMinuteHeartbeat;
		Gameworld.HeartbeatManager.FuzzyTenMinuteHeartbeat -= HeartbeatManager_TenMinuteHeartbeat;
		Gameworld.HeartbeatManager.FuzzyThirtyMinuteHeartbeat -= HeartbeatManager_ThirtyMinuteHeartbeat;
		Gameworld.HeartbeatManager.FuzzyHourHeartbeat -= HeartbeatManager_FuzzyHourHeartbeat;
		foreach (var cell in _subscribedCells)
		{
			cell.OnRoomEmoteEcho -= Cell_OnRoomEcho;
		}
	}

	public void Delete()
	{
		Changed = false;
		UnsubscribeEvents();
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.AIStorytellers.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.AIStorytellers.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public void Pause() { IsPaused = true; Changed = true; }
	public void Unpause() { IsPaused = false; Changed = true; }

	private readonly List<IAIStorytellerCharacterMemory> _characterMemories = new();
	public IEnumerable<IAIStorytellerCharacterMemory> CharacterMemories => _characterMemories;

	private readonly List<IAIStorytellerSituation> _situations = new();
	public IEnumerable<IAIStorytellerSituation> Situations => _situations;

	private const string HelpText = @"";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command) => throw new NotImplementedException();

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"AI Storyteller #{Id} - {Name.Colour(Telnet.Cyan)}");
		sb.AppendLine($"Model: {Model.ColourValue()}");
		sb.AppendLine($"Reasoning Effort: {ReasoningEffort.Describe().ColourValue()}");
		sb.AppendLine($"Is Paused: {IsPaused.ToColouredString()}");
		sb.AppendLine($"");
		sb.AppendLine("Description".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(Description.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("Surveillance and Events".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine($"Subscribe To Room Events: {SubscribeToRoomEvents.ToColouredString()}");
		sb.AppendLine($"Subscribe To 5m Tick: {SubscribeTo5mHeartbeat.ToColouredString()}");
		sb.AppendLine($"Subscribe To 10m Tick: {SubscribeTo10mHeartbeat.ToColouredString()}");
		sb.AppendLine($"Subscribe To 30m Tick: {SubscribeTo30mHeartbeat.ToColouredString()}");
		sb.AppendLine($"Subscribe To 1h Tick: {SubscribeToHourHeartbeat.ToColouredString()}");
		sb.AppendLine($"Status Prog for 5m: {HeartbeatStatus5mProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Status Prog for 10m: {HeartbeatStatus10mProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Status Prog for 30m: {HeartbeatStatus30mProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"Status Prog for 1h: {HeartbeatStatus1hProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
		sb.AppendLine($"");
		sb.AppendLine(SurveillanceStrategy.Show(actor));
		sb.AppendLine($"");
		sb.AppendLine("Attention Prompt".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(AttentionAgentPrompt.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("System Prompt".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(SystemPrompt.Wrap(actor.InnerLineFormatLength, "\t"));
		sb.AppendLine($"");
		sb.AppendLine("Custom Tool Calls".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		if (CustomToolCalls.Count == 0 && CustomToolCallsEchoOnly.Count == 0)
		{
			sb.AppendLine();
			sb.AppendLine("None");
		}
		else
		{
			foreach (var item in CustomToolCalls)
			{
				var valid = item.IsValid; // This call has side effects and so needs to happen before the foreach loop below to guarantee the keys all exist
				sb.AppendLine();
				sb.AppendLine($"\tFunction Name: {item.Name.ColourValue()}");
				sb.AppendLine($"\tFunction Description: {item.Description.ColourValue()}");
				sb.AppendLine($"\tFunction Prog: {item.Prog?.MXPClickableFunctionName() ?? "None".ColourError()}");
				sb.AppendLine($"\tAvailable Context: {"Always".ColourValue()}");
				sb.AppendLine($"\tFunction Parameters:");
				foreach (var par in item.Prog?.NamedParameters ?? [])
				{
					sb.AppendLine($"\t\t{par.Item1.Describe().Colour(Telnet.VariableGreen)} {par.Item2}: {item.ParameterDescriptions[par.Item2]}");
				}
				if (!valid)
				{
					sb.AppendLine("\t\tWarning - this tool call is not valid due to errors.".ColourError());
				}
			}

			foreach (var item in CustomToolCallsEchoOnly)
			{
				var valid = item.IsValid; // This call has side effects and so needs to happen before the foreach loop below to guarantee the keys all exist
				sb.AppendLine();
				sb.AppendLine($"\tFunction Name: {item.Name.ColourValue()}");
				sb.AppendLine($"\tFunction Description: {item.Description.ColourValue()}");
				sb.AppendLine($"\tFunction Prog: {item.Prog?.MXPClickableFunctionName() ?? "None".ColourError()}");
				sb.AppendLine($"\tAvailable Context: {"Echoes Only".ColourValue()}");
				sb.AppendLine($"\tFunction Parameters:");
				foreach (var par in item.Prog?.NamedParameters ?? [])
				{
					sb.AppendLine($"\t\t{par.Item1.Describe().Colour(Telnet.VariableGreen)} {par.Item2}: {item.ParameterDescriptions[par.Item2]}");
				}
				if (!valid)
				{
					sb.AppendLine("\t\tWarning - this tool call is not valid due to errors.".ColourError());
				}
			}
		}
			sb.AppendLine($"");
		sb.AppendLine("Current Situations".GetLineWithTitleInner(actor, Telnet.Blue, Telnet.BoldWhite));
		sb.AppendLine($"");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in _situations
			select new List<string> {
				item.Id.ToStringN0(actor),
				item.Name,
				item.CreatedOn.GetLocalDateString(actor, true)
			},
			[
				"Id",
				"Title",
				"Created"
			],
			actor,
			Telnet.Green
		));

		return sb.ToString();
	}
	public override void Save()
	{
		Changed = false;
		var dbitem = FMDB.Context.AIStorytellers.Find(Id);
		if (dbitem is null)
		{
			return;
		}

		dbitem.Name = Name;
		dbitem.Description = Description;
		dbitem.AttentionAgentPrompt = AttentionAgentPrompt;
		dbitem.SystemPrompt = SystemPrompt;
		dbitem.SurveillanceStrategyDefinition = SurveillanceStrategy.SaveDefinition();
		dbitem.HeartbeatStatus10mProgId = HeartbeatStatus10mProg?.Id;
		dbitem.HeartbeatStatus1hProgId = HeartbeatStatus1hProg?.Id;
		dbitem.HeartbeatStatus30mProgId = HeartbeatStatus30mProg?.Id;
		dbitem.HeartbeatStatus5mProgId = HeartbeatStatus5mProg?.Id;
		dbitem.SubscribeToHourHeartbeat = SubscribeToHourHeartbeat;
		dbitem.SubscribeTo30mHeartbeat = SubscribeTo30mHeartbeat;
		dbitem.SubscribeTo5mHeartbeat = SubscribeTo5mHeartbeat;
		dbitem.SubscribeTo10mHeartbeat = SubscribeTo10mHeartbeat;
		dbitem.SubscribeToRoomEvents = SubscribeToRoomEvents;
		dbitem.IsPaused = IsPaused;
		dbitem.CustomToolCallsDefinition = new XElement("ToolCalls",
				from item in CustomToolCalls select item.SaveToXml(false),
				from item in CustomToolCallsEchoOnly select item.SaveToXml(true)
			).ToString();
	}
}

public static class ReasoningExtensions
{
	public static string Describe(this ResponseReasoningEffortLevel level)
	{
		if (level == ResponseReasoningEffortLevel.Minimal)
		{
			return "Minimal";
		}
		if (level == ResponseReasoningEffortLevel.Low)
		{
			return "Low";
		}
		if (level == ResponseReasoningEffortLevel.Medium)
		{
			return "Medium";
		}
		if (level == ResponseReasoningEffortLevel.High)
		{
			return "High";
		}
		return "Unknown";
	}
}


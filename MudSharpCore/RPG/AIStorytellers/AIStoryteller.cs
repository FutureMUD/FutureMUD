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
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Form.Shape;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;

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

			var parameterNames = Prog.NamedParameters.Select(x => x.Item2).ToList();
			if (parameterNames.Count != ParameterDescriptions.Count)
			{
				return false;
			}

			if (parameterNames.Any(x => !ParameterDescriptions.ContainsKey(x)))
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

	public string Name { get; set; }
	public string Description { get; set; }
	public IFutureProg Prog { get; set; }
	private Dictionary<string, string> _parameterDescriptions;
	public IReadOnlyDictionary<string, string> ParameterDescriptions => _parameterDescriptions;

	public void RefreshParameterDescriptions()
	{
		RecheckFunctionParameters();
	}

	public void SetParameterDescription(string parameterName, string description)
	{
		_parameterDescriptions[parameterName] = description;
	}

	public XElement SaveToXml(bool includeWithEcho)
	{
		return new XElement("ToolCall",
			new XElement("Name", new XCData(Name)),
			new XElement("Description", new XCData(Description)),
			new XElement("Prog", Prog?.Id ?? 0L),
			new XElement("IncludeWithEcho", includeWithEcho),
			new XElement("ParameterDescriptions",
				from item in ParameterDescriptions
				select new XElement("Description", new XAttribute("name", item.Key), new XCData(item.Value))
			)
		);
	}

	public AIStorytellerCustomToolCall(string name, string description, IFutureProg prog)
	{
		Name = name;
		Description = description;
		Prog = prog;
		_parameterDescriptions = new Dictionary<string, string>();
		RecheckFunctionParameters();
	}

	public AIStorytellerCustomToolCall(XElement root, IFuturemud gameworld)
	{
		Name = root.Element("Name")?.Value ?? "UnnamedTool";
		Description = root.Element("Description")?.Value ?? string.Empty;
		Prog = gameworld.FutureProgs.Get(long.Parse(root.Element("Prog")?.Value ?? "0"));
		var dictionary = new Dictionary<string, string>();
		foreach (var description in root.Element("ParameterDescriptions")?.Elements("Description") ??
		                             Enumerable.Empty<XElement>())
		{
			var name = description.Attribute("name")?.Value;
			if (string.IsNullOrWhiteSpace(name))
			{
				continue;
			}

			dictionary[name] = description.Value;
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
	private const int MaxToolCallDepth = 16;
	private const int MaxMalformedToolCallRetries = 3;
	private static readonly TimeSpan MaxToolExecutionDuration = TimeSpan.FromSeconds(30);
	private const int MaxSituationTitlesInPrompt = 25;
	private const int MaxPromptCharacters = 24_000;

	public AIStoryteller(Models.AIStoryteller storyteller, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		Id = storyteller.Id;
		_name = storyteller.Name ?? "Unnamed Storyteller";
		Description = storyteller.Description ?? string.Empty;
		Model = storyteller.Model ?? "gpt-5";
		SystemPrompt = storyteller.SystemPrompt ?? string.Empty;
		AttentionAgentPrompt = storyteller.AttentionAgentPrompt ?? string.Empty;
		ReasoningEffort = ParseReasoningEffort(storyteller.ReasoningEffort);
		SurveillanceStrategy = new AIStorytellerSurveillanceStrategy(gameworld,
			storyteller.SurveillanceStrategyDefinition ?? string.Empty);
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
		var root = SafeLoadCustomToolRoot(storyteller.CustomToolCallsDefinition);
		foreach (var element in root.Elements("ToolCall"))
		{
			AIStorytellerCustomToolCall toolCall;
			try
			{
				toolCall = new AIStorytellerCustomToolCall(element, Gameworld);
			}
			catch
			{
				continue;
			}

			if (bool.TryParse(element.Element("IncludeWithEcho")?.Value, out var includeWithEcho) && includeWithEcho)
			{
				CustomToolCallsEchoOnly.Add(toolCall);
				continue;
			}
			CustomToolCalls.Add(toolCall);
		}

		if (long.TryParse(root.Element("CustomPlayerInformationProgId")?.Value, out var customPlayerProgId))
		{
			CustomPlayerInformationProg = gameworld.FutureProgs.Get(customPlayerProgId);
		}
	}

	public AIStoryteller(IFuturemud gameworld, string name)
	{
		Gameworld = gameworld;
		_name = name;
		Description = "An AI Storyteller";
		Model = "gpt-5";
		SystemPrompt = "You are an AI storyteller for an RPI MUD world.";
		AttentionAgentPrompt =
			"Reply with \"interested\" optionally followed by a short reason only when input matters for your narrative remit. Otherwise reply \"ignore\".";
		ReasoningEffort = ResponseReasoningEffortLevel.Medium;
		SurveillanceStrategy = new AIStorytellerSurveillanceStrategy(gameworld, string.Empty);
		SubscribeToRoomEvents = true;
		IsPaused = true;

		using (new FMDB())
		{
			var dbitem = new Models.AIStoryteller
			{
				Name = name,
				Description = Description,
				Model = Model,
				SystemPrompt = SystemPrompt,
				AttentionAgentPrompt = AttentionAgentPrompt,
				SurveillanceStrategyDefinition = SurveillanceStrategy.SaveDefinition(),
				ReasoningEffort = SerializeReasoningEffort(ReasoningEffort),
				CustomToolCallsDefinition = new XElement("ToolCalls").ToString(),
				SubscribeToRoomEvents = SubscribeToRoomEvents,
				SubscribeTo5mHeartbeat = false,
				SubscribeTo10mHeartbeat = false,
				SubscribeTo30mHeartbeat = false,
				SubscribeToHourHeartbeat = false,
				IsPaused = true
			};
			FMDB.Context.AIStorytellers.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
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
	private readonly List<IAIStorytellerCharacterMemory> _characterMemories = [];
	public IEnumerable<IAIStorytellerCharacterMemory> CharacterMemories => _characterMemories;

	private readonly List<IAIStorytellerSituation> _situations = [];
	public IEnumerable<IAIStorytellerSituation> Situations => _situations;

	public void RegisterLoadedSituation(IAIStorytellerSituation situation)
	{
		_situations.Add(situation);
	}

	public void RegisterLoadedMemory(IAIStorytellerCharacterMemory memory)
	{
		_characterMemories.Add(memory);
	}

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
		AddFunctionTool(
			options,
			"CreateSituation",
			"Creates a new situation for the AI storyteller to manage.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Title": { "type": "string", "description": "The title of the situation." },
			    "Description": { "type": "string", "description": "The detailed text of the situation." }
			  },
			  "required": ["Title", "Description"]
			}
			""");

		AddFunctionTool(
			options,
			"UpdateSituation",
			"Updates an existing situation.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the situation to update." },
			    "Title": { "type": "string", "description": "The updated title." },
			    "Description": { "type": "string", "description": "The updated detailed text." }
			  },
			  "required": ["Id", "Title", "Description"]
			}
			""");

		AddFunctionTool(
			options,
			"ResolveSituation",
			"Resolves an existing situation.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the situation to resolve." },
			    "Title": { "type": "string", "description": "The final title." },
			    "Description": { "type": "string", "description": "The final detailed text." }
			  },
			  "required": ["Id", "Title", "Description"]
			}
			""");

		AddFunctionTool(
			options,
			"ShowSituation",
			"Shows full details of a specific situation by id.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the situation to retrieve." }
			  },
			  "required": ["Id"]
			}
			""");

		AddFunctionTool(options, "Noop",
			"Use this when no side-effect is required but a tool response is needed.", null);
		AddFunctionTool(options, "OnlinePlayers",
			"Returns summary information about online player characters.", null);

		AddFunctionTool(
			options,
			"PlayerInformation",
			"Returns detailed information about a specific player character.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the player character." }
			  },
			  "required": ["Id"]
			}
			""");

		AddFunctionTool(
			options,
			"CreateMemory",
			"Creates a new memory about a player character.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the target character." },
			    "Title": { "type": "string", "description": "The memory title." },
			    "Details": { "type": "string", "description": "The memory details." }
			  },
			  "required": ["Id", "Title", "Details"]
			}
			""");

		AddFunctionTool(
			options,
			"UpdateMemory",
			"Updates a memory.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the memory." },
			    "Title": { "type": "string", "description": "The memory title." },
			    "Details": { "type": "string", "description": "The memory details." }
			  },
			  "required": ["Id", "Title", "Details"]
			}
			""");

		AddFunctionTool(
			options,
			"ForgetMemory",
			"Forgets (deletes) a memory.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the memory to forget." }
			  },
			  "required": ["Id"]
			}
			""");

		AddFunctionTool(options, "Landmarks",
			"Returns summary information about available landmarks.", null);
		AddFunctionTool(
			options,
			"ShowLandmark",
			"Returns detailed information about a specific landmark.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "string", "description": "The name of the landmark." }
			  },
			  "required": ["Id"]
			}
			""");

		AddFunctionTool(
			options,
			"SearchReferenceDocuments",
			"Searches AI storyteller reference documents.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Query": { "type": "string", "description": "Search text. Leave blank to list all." }
			  },
			  "required": []
			}
			""");

		AddFunctionTool(
			options,
			"ShowReferenceDocument",
			"Returns a specific reference document in detail by id.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the reference document." }
			  },
			  "required": ["Id"]
			}
			""");

		AddFunctionTool(
			options,
			"PathBetweenRooms",
			"Returns a list of movement commands to path between two rooms.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "OriginRoomId": { "type": "integer", "description": "The id of the origin room." },
			    "DestinationRoomId": { "type": "integer", "description": "The id of the destination room." },
			    "PathSearchFunction": { "type": "string", "description": "Path search mode. Valid values include RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors, IgnorePresenceOfDoors, PathIgnoreDoors, PathRespectClosedDoors and PathIncludeUnlockedDoors." }
			  },
			  "required": ["OriginRoomId", "DestinationRoomId", "PathSearchFunction"]
			}
			""");

		AddFunctionTool(
			options,
			"PathFromCharacterToRoom",
			"Returns a list of movement commands to path from a character's current room to a destination room.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "OriginCharacterId": { "type": "integer", "description": "The id of the origin character." },
			    "DestinationRoomId": { "type": "integer", "description": "The id of the destination room." },
			    "PathSearchFunction": { "type": "string", "description": "Path search mode. Valid values include IncludeUnlockableDoors, PathIncludeUnlockableDoors, PathIgnoreDoors, PathRespectClosedDoors, PathIncludeUnlockedDoors, RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors and IgnorePresenceOfDoors." }
			  },
			  "required": ["OriginCharacterId", "DestinationRoomId", "PathSearchFunction"]
			}
			""");

		AddFunctionTool(
			options,
			"PathBetweenCharacters",
			"Returns a list of movement commands to path between two characters using their current rooms.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "OriginCharacterId": { "type": "integer", "description": "The id of the origin character." },
			    "DestinationCharacterId": { "type": "integer", "description": "The id of the destination character." },
			    "PathSearchFunction": { "type": "string", "description": "Path search mode. Valid values include IncludeUnlockableDoors, PathIncludeUnlockableDoors, PathIgnoreDoors, PathRespectClosedDoors, PathIncludeUnlockedDoors, RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors and IgnorePresenceOfDoors." }
			  },
			  "required": ["OriginCharacterId", "DestinationCharacterId", "PathSearchFunction"]
			}
			""");

		AddFunctionTool(options, "RecentCharacterPlans",
			"Returns plans for online characters who recently updated plans (90 day window semantics).", null);

		AddFunctionTool(
			options,
			"CharacterPlans",
			"Returns plans for a specific character by id.",
			"""
			{
			  "type": "object",
			  "properties": {
			    "Id": { "type": "integer", "description": "The id of the character." }
			  },
			  "required": ["Id"]
			}
			""");
	}

	private void AddFunctionTool(CreateResponseOptions options, string functionName, string functionDescription,
		string functionParametersJson)
	{
		options.Tools.Add(ResponseTool.CreateFunctionTool(
			functionName: functionName,
			functionDescription: functionDescription,
			functionParameters: string.IsNullOrWhiteSpace(functionParametersJson)
				? null
				: BinaryData.FromString(functionParametersJson),
			strictModeEnabled: true
		));
	}

	private void AddCustomToolCallsToResponseOptions(CreateResponseOptions options, bool includeEchoes)
	{
		AddCustomToolCallsToResponseOptions(options, CustomToolCalls);
		if (includeEchoes)
		{
			AddCustomToolCallsToResponseOptions(options, CustomToolCallsEchoOnly);
		}
	}

	private void AddCustomToolCallsToResponseOptions(CreateResponseOptions options,
		IEnumerable<AIStorytellerCustomToolCall> toolCalls)
	{
		foreach (var toolCall in toolCalls)
		{
			if (!toolCall.IsValid)
			{
				continue;
			}

			var schema = BuildCustomToolCallSchema(toolCall);
			AddFunctionTool(options, toolCall.Name, toolCall.Description, schema);
		}
	}

	private static string BuildCustomToolCallSchema(AIStorytellerCustomToolCall toolCall)
	{
		var properties = new Dictionary<string, object>();
		var required = new List<string>();

		foreach (var (parameterType, parameterName) in toolCall.Prog.NamedParameters)
		{
			required.Add(parameterName);
			var description = toolCall.ParameterDescriptions.TryGetValue(parameterName, out var item)
				? item
				: string.Empty;
			properties[parameterName] = BuildJsonSchemaPropertyForProgType(parameterType, description);
		}

		var schema = new Dictionary<string, object>
		{
			["type"] = "object",
			["properties"] = properties,
			["required"] = required
		};

		return JsonSerializer.Serialize(schema);
	}

	private static object BuildJsonSchemaPropertyForProgType(ProgVariableTypes type, string description)
	{
		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			return new Dictionary<string, object>
			{
				["type"] = "array",
				["description"] = description,
				["items"] = BuildJsonSchemaPropertyForProgType(type ^ ProgVariableTypes.Collection,
					"Item value")
			};
		}

		return type switch
		{
			ProgVariableTypes.Boolean => new Dictionary<string, object>
			{
				["type"] = "boolean",
				["description"] = description
			},
			ProgVariableTypes.Number => new Dictionary<string, object>
			{
				["type"] = "number",
				["description"] = description
			},
			ProgVariableTypes.Gender => new Dictionary<string, object>
			{
				["type"] = "string",
				["description"] =
					$"{description} Valid values are male, female, neuter, non-binary or indeterminate."
			},
			ProgVariableTypes.Text => new Dictionary<string, object>
			{
				["type"] = "string",
				["description"] = description
			},
			_ => new Dictionary<string, object>
			{
				["type"] = "integer",
				["description"] = $"Engine object id. {description}"
			}
		};
	}

	private static XElement SafeLoadCustomToolRoot(string? xml)
	{
		if (string.IsNullOrWhiteSpace(xml))
		{
			return new XElement("ToolCalls");
		}

		try
		{
			var root = XElement.Parse(xml);
			if (root.Name == "ToolCalls")
			{
				return root;
			}

			return root.Element("ToolCalls") ?? new XElement("ToolCalls");
		}
		catch
		{
			return new XElement("ToolCalls");
		}
	}

	private static ResponseReasoningEffortLevel ParseReasoningEffort(string? persistedValue)
	{
		if (string.IsNullOrWhiteSpace(persistedValue))
		{
			return ResponseReasoningEffortLevel.Medium;
		}

		if (int.TryParse(persistedValue, out var numericValue))
		{
			return numericValue switch
			{
				0 => ResponseReasoningEffortLevel.Minimal,
				1 => ResponseReasoningEffortLevel.Low,
				2 => ResponseReasoningEffortLevel.Medium,
				3 => ResponseReasoningEffortLevel.High,
				_ => ResponseReasoningEffortLevel.Medium
			};
		}

		return persistedValue.Trim().ToLowerInvariant() switch
		{
			"minimal" => ResponseReasoningEffortLevel.Minimal,
			"low" => ResponseReasoningEffortLevel.Low,
			"medium" => ResponseReasoningEffortLevel.Medium,
			"high" => ResponseReasoningEffortLevel.High,
			_ => ResponseReasoningEffortLevel.Medium
		};
	}

	private static bool TryParseReasoningEffort(string text, out ResponseReasoningEffortLevel effortLevel)
	{
		effortLevel = ResponseReasoningEffortLevel.Medium;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (int.TryParse(text, out var numericValue))
		{
			if (numericValue is < 0 or > 3)
			{
				return false;
			}

			effortLevel = ParseReasoningEffort(numericValue.ToString());
			return true;
		}

		switch (text.Trim().ToLowerInvariant())
		{
			case "minimal":
				effortLevel = ResponseReasoningEffortLevel.Minimal;
				return true;
			case "low":
				effortLevel = ResponseReasoningEffortLevel.Low;
				return true;
			case "medium":
				effortLevel = ResponseReasoningEffortLevel.Medium;
				return true;
			case "high":
				effortLevel = ResponseReasoningEffortLevel.High;
				return true;
			default:
				return false;
		}
	}

	private static string SerializeReasoningEffort(ResponseReasoningEffortLevel level)
	{
		if (level == ResponseReasoningEffortLevel.Minimal)
		{
			return "0";
		}

		if (level == ResponseReasoningEffortLevel.Low)
		{
			return "1";
		}

		if (level == ResponseReasoningEffortLevel.Medium)
		{
			return "2";
		}

		if (level == ResponseReasoningEffortLevel.High)
		{
			return "3";
		}

		return "2";
	}

	private XElement BuildCustomToolRoot()
	{
		return new XElement("ToolCalls",
			from item in CustomToolCalls
			select item.SaveToXml(false),
			from item in CustomToolCallsEchoOnly
			select item.SaveToXml(true),
			CustomPlayerInformationProg is null
				? null
				: new XElement("CustomPlayerInformationProgId", CustomPlayerInformationProg.Id)
		);
	}

	private void PassHeartbeatEventToAIStoryteller(string heartbeatType)
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

		var sb = new StringBuilder();
		sb.AppendLine("A configured heartbeat event has triggered.");
		switch (heartbeatType)
		{
			case "5m":
				sb.AppendLine("Heartbeat type: five minute.");
				var text = HeartbeatStatus5mProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text))
				{
					sb.AppendLine(text);
				}

				break;
			case "10m":
				sb.AppendLine("Heartbeat type: ten minute.");
				text = HeartbeatStatus10mProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text))
				{
					sb.AppendLine(text);
				}

				break;
			case "30m":
				sb.AppendLine("Heartbeat type: thirty minute.");
				text = HeartbeatStatus30mProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text))
				{
					sb.AppendLine(text);
				}

				break;
			case "1h":
				sb.AppendLine("Heartbeat type: one hour.");
				text = HeartbeatStatus1hProg?.ExecuteString(this);
				if (!string.IsNullOrWhiteSpace(text))
				{
					sb.AppendLine(text);
				}

				break;
		}

		AppendOpenSituationTitles(sb);

		ResponsesClient client = new(Model, apiKey);
		List<ResponseItem> messages =
		[
			ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
			ResponseItem.CreateUserMessageItem(TrimPromptText(sb.ToString()))
		];
		ExecuteToolCall(client, messages, includeEchoTools: false);
	}

	internal void PassHeartbeatEventToAIStorytellerForTesting(string heartbeatType)
	{
		PassHeartbeatEventToAIStoryteller(heartbeatType);
	}

	private void PassSituationToAIStoryteller(ICell location, PerceptionEngine.IEmoteOutput emote, string echo,
		string attentionReason)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		AppendOpenSituationTitles(sb);
		sb.AppendLine("Your attention classifier flagged a world event as relevant.");
		sb.AppendLine($"Location: {location.HowSeen(null)}");
		if (emote?.DefaultSource is ICharacter ch)
		{
			sb.AppendLine($"Source Character: {ch.PersonalName.GetName(NameStyle.FullName)}");
			sb.AppendLine(
				$"Source Description: {ch.HowSeen(ch, flags: PerceiveIgnoreFlags.TrueDescription)}");
		}

		sb.AppendLine("Event Text:");
		sb.AppendLine(echo.StripANSIColour().StripMXP());
		sb.AppendLine("Attention Reason:");
		sb.AppendLine(attentionReason?.IfNullOrWhiteSpace("No reason provided"));

		ResponsesClient client = new(Model, apiKey);
		List<ResponseItem> messages =
		[
			ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
			ResponseItem.CreateUserMessageItem(TrimPromptText(sb.ToString()))
		];
		ExecuteToolCall(client, messages, includeEchoTools: true);
	}

	internal readonly record struct ToolExecutionResult(string OutputJson, bool MalformedJson);
	internal Action<string>? ErrorLoggerOverride { get; set; }
	internal Action<Exception>? ExceptionLoggerOverride { get; set; }
	private const string MalformedToolCallFeedbackMessage =
		"One or more tool calls used malformed JSON. Retry with valid JSON arguments that exactly match the declared tool schemas.";

	private void ExecuteToolCall(ResponsesClient client, List<ResponseItem> messages, bool includeEchoTools)
	{
		var started = DateTime.UtcNow;
		var malformedRetries = 0;

		for (var depth = 0; depth < MaxToolCallDepth; depth++)
		{
			if (DateTime.UtcNow - started > MaxToolExecutionDuration)
			{
				LogStorytellerError("Tool execution duration exceeded safety budget.");
				return;
			}

			try
			{
				var options = new CreateResponseOptions(messages);
				options.ReasoningOptions.ReasoningEffortLevel = ReasoningEffort;
				AddUniversalToolsToResponseOptions(options);
				AddCustomToolCallsToResponseOptions(options, includeEchoTools);

				var response = client.CreateResponseAsync(options).GetAwaiter().GetResult().Value;
				messages.AddRange(response.OutputItems);
				var functionCalls = response.OutputItems.OfType<FunctionCallResponseItem>().ToList();
				if (!functionCalls.Any())
				{
					return;
				}
				var (shouldContinue, retries) = ProcessFunctionCallBatch(
					functionCalls.Select(x => (x.Id, x.FunctionName, x.FunctionArguments.ToString())),
					includeEchoTools,
					messages,
					malformedRetries);
				malformedRetries = retries;
				if (!shouldContinue)
				{
					return;
				}
			}
			catch (Exception e)
			{
				LogStorytellerException(e);
				return;
			}
		}

		LogStorytellerError($"Tool call depth exceeded safety budget of {MaxToolCallDepth:N0} rounds.");
	}

	private void LogStorytellerError(string message)
	{
		ErrorLoggerOverride?.Invoke(message);
		var formattedMessage = $"Storyteller {Id:N0}: {message}";
		formattedMessage.Prepend("#2GPT Error#0\n").WriteLineConsole();
		try
		{
			Futuremud.Games.FirstOrDefault()?.DiscordConnection?.NotifyAdmins(
				$"**GPT Error**\n\n```\n{formattedMessage}```");
		}
		catch
		{
			// Best-effort logging only.
		}
	}

	private void LogStorytellerException(Exception e)
	{
		ExceptionLoggerOverride?.Invoke(e);
		e.ToString().Prepend("#2GPT Error#0\n").WriteLineConsole();
		try
		{
			Futuremud.Games.FirstOrDefault()?.DiscordConnection?.NotifyAdmins($"**GPT Error**\n\n```\n{e}```");
		}
		catch
		{
			// Best-effort logging only.
		}
	}

	internal (bool Continue, int MalformedRetries) ProcessFunctionCallBatch(
		IEnumerable<(string CallId, string FunctionName, string? ArgumentsJson)> functionCalls,
		bool includeEchoTools,
		List<ResponseItem> messages,
		int malformedRetries)
	{
		var malformedThisRound = false;
		foreach (var functionCall in functionCalls)
		{
			var result = ExecuteFunctionCall(functionCall.FunctionName, functionCall.ArgumentsJson, includeEchoTools);
			malformedThisRound |= result.MalformedJson;
			var callId = string.IsNullOrWhiteSpace(functionCall.CallId) ? Guid.NewGuid().ToString("N") : functionCall.CallId;
			messages.Add(ResponseItem.CreateFunctionCallOutputItem(callId, result.OutputJson));
		}

		if (!malformedThisRound)
		{
			return (true, 0);
		}

		malformedRetries++;
		if (malformedRetries > MaxMalformedToolCallRetries)
		{
			LogStorytellerError("Malformed tool-call JSON retry budget exceeded.");
			return (false, malformedRetries);
		}

		messages.Add(ResponseItem.CreateUserMessageItem(MalformedToolCallFeedbackMessage));
		return (true, malformedRetries);
	}

	internal ToolExecutionResult ExecuteFunctionCall(string functionName, string? argumentsText, bool includeEchoTools)
	{
		JsonDocument? argumentsJson = null;
		try
		{
			if (string.IsNullOrWhiteSpace(argumentsText))
			{
				argumentsText = "{}";
			}

			argumentsJson = JsonDocument.Parse(argumentsText);
			var arguments = argumentsJson.RootElement;
			return functionName switch
			{
				"Noop" => SuccessResult(new Dictionary<string, object>
				{
					["message"] = "No action taken."
				}),
				"CreateSituation" => HandleCreateSituation(arguments),
				"UpdateSituation" => HandleUpdateSituation(arguments),
				"ResolveSituation" => HandleResolveSituation(arguments),
				"ShowSituation" => HandleShowSituation(arguments),
				"OnlinePlayers" => HandleOnlinePlayers(),
				"PlayerInformation" => HandlePlayerInformation(arguments),
				"CreateMemory" => HandleCreateMemory(arguments),
				"UpdateMemory" => HandleUpdateMemory(arguments),
				"ForgetMemory" => HandleForgetMemory(arguments),
				"Landmarks" => HandleLandmarks(),
				"ShowLandmark" => HandleShowLandmark(arguments),
				"SearchReferenceDocuments" => HandleSearchReferenceDocuments(arguments),
				"ShowReferenceDocument" => HandleShowReferenceDocument(arguments),
				"PathBetweenRooms" => HandlePathBetweenRooms(arguments),
				"PathFromCharacterToRoom" => HandlePathFromCharacterToRoom(arguments),
				"PathBetweenCharacters" => HandlePathBetweenCharacters(arguments),
				"RecentCharacterPlans" => HandleRecentCharacterPlans(),
				"CharacterPlans" => HandleCharacterPlans(arguments),
				_ => HandleCustomFunctionCall(functionName, arguments, includeEchoTools)
			};
		}
		catch (JsonException e)
		{
			return ErrorResult($"Malformed tool-call JSON: {e.Message}", malformedJson: true);
		}
		catch (Exception e)
		{
			return ErrorResult($"Tool handler error: {e.Message}");
		}
		finally
		{
			argumentsJson?.Dispose();
		}
	}

	private ToolExecutionResult SuccessResult(object payload)
	{
		return new ToolExecutionResult(JsonSerializer.Serialize(new Dictionary<string, object>
		{
			["ok"] = true,
			["result"] = ConvertToToolOutputValue(payload)
		}), false);
	}

	private static ToolExecutionResult ErrorResult(string error, bool malformedJson = false)
	{
		return new ToolExecutionResult(JsonSerializer.Serialize(new Dictionary<string, object>
		{
			["ok"] = false,
			["error"] = error
		}), malformedJson);
	}

	private static bool TryReadId(JsonElement element, out long id)
	{
		id = 0L;
		switch (element.ValueKind)
		{
			case JsonValueKind.Number:
				return element.TryGetInt64(out id);
			case JsonValueKind.String:
				return long.TryParse(element.GetString(), out id);
			case JsonValueKind.Object:
				foreach (var name in new[] { "Id", "ID", "id" })
				{
					if (element.TryGetProperty(name, out var property) && TryReadId(property, out id))
					{
						return true;
					}
				}

				return false;
			default:
				return false;
		}
	}

	private static string? TryGetOptionalString(JsonElement arguments, string propertyName)
	{
		if (!arguments.TryGetProperty(propertyName, out var property))
		{
			return null;
		}

		if (property.ValueKind == JsonValueKind.String)
		{
			return property.GetString() ?? string.Empty;
		}

		return property.ToString();
	}

	private static bool TryGetRequiredString(JsonElement arguments, string propertyName, out string value,
		out string error)
	{
		value = string.Empty;
		if (!arguments.TryGetProperty(propertyName, out var property))
		{
			error = $"Missing required property '{propertyName}'.";
			return false;
		}

		value = property.ValueKind == JsonValueKind.String
			? property.GetString() ?? string.Empty
			: property.ToString();
		error = string.Empty;
		return true;
	}

	private static bool TryGetRequiredLong(JsonElement arguments, string propertyName, out long value, out string error)
	{
		value = 0L;
		if (!arguments.TryGetProperty(propertyName, out var property))
		{
			error = $"Missing required property '{propertyName}'.";
			return false;
		}

		if (TryReadId(property, out value))
		{
			error = string.Empty;
			return true;
		}

		error = $"Property '{propertyName}' must contain an integer id.";
		return false;
	}

	private ToolExecutionResult HandleCreateSituation(JsonElement arguments)
	{
		if (!TryGetRequiredString(arguments, "Title", out var title, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Description", out var description, out error))
		{
			return ErrorResult(error);
		}

		var situation = new AIStorytellerSituation(Gameworld, this, title, description);
		_situations.Add(situation);
		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = situation.Id,
			["Title"] = situation.Name,
			["CreatedOn"] = situation.CreatedOn.ToString("O"),
			["Resolved"] = situation.IsResolved
		});
	}

	private ToolExecutionResult HandleUpdateSituation(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Title", out var title, out error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Description", out var description, out error))
		{
			return ErrorResult(error);
		}

		var situation = _situations.FirstOrDefault(x => x.Id == id);
		if (situation is null)
		{
			return ErrorResult($"No situation with id {id:N0} exists.");
		}

		situation.UpdateSituation(title, description);
		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = situation.Id,
			["Title"] = situation.Name,
			["Resolved"] = situation.IsResolved
		});
	}

	private ToolExecutionResult HandleResolveSituation(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Title", out var title, out error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Description", out var description, out error))
		{
			return ErrorResult(error);
		}

		var situation = _situations.FirstOrDefault(x => x.Id == id);
		if (situation is null)
		{
			return ErrorResult($"No situation with id {id:N0} exists.");
		}

		situation.UpdateSituation(title, description);
		situation.Resolve();
		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = situation.Id,
			["Title"] = situation.Name,
			["Resolved"] = situation.IsResolved
		});
	}

	private ToolExecutionResult HandleShowSituation(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		var situation = _situations.FirstOrDefault(x => x.Id == id);
		if (situation is null)
		{
			return ErrorResult($"No situation with id {id:N0} exists.");
		}

		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = situation.Id,
			["Title"] = situation.Name,
			["Description"] = situation.SituationText,
			["CreatedOn"] = situation.CreatedOn.ToString("O"),
			["Resolved"] = situation.IsResolved
		});
	}

	private ToolExecutionResult HandleOnlinePlayers()
	{
		var players = Gameworld.Characters
			.Where(x => x.IsPlayerCharacter)
			.Select(x => new Dictionary<string, object>
			{
				["Id"] = x.Id,
				["Name"] = x.PersonalName.GetName(NameStyle.FullName),
				["Gender"] = x.Gender.GenderClass(),
				["ShortDescription"] = x.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
				["LocationId"] = x.Location?.Id ?? 0L,
				["LocationName"] = x.Location?.HowSeen(null, colour: false) ?? "Unknown",
				["NumberOfMemories"] = _characterMemories.Count(y => y.Character.Id == x.Id)
			})
			.ToList();

		return SuccessResult(new Dictionary<string, object>
		{
			["Players"] = players
		});
	}

	private ToolExecutionResult HandlePlayerInformation(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		var pc = Gameworld.Characters.FirstOrDefault(x => x.IsPlayerCharacter && x.Id == id);
		if (pc is null)
		{
			return ErrorResult($"No player character with id {id:N0} exists.");
		}

		var result = new Dictionary<string, object>
		{
			["Id"] = pc.Id,
			["Name"] = pc.PersonalName.GetName(NameStyle.FullName),
			["Gender"] = pc.Gender.GenderClass(),
			["Race"] = pc.Race.Name,
			["Ethnicity"] = pc.Ethnicity.Name,
			["Culture"] = pc.Culture.Name,
			["Age"] = pc.AgeInYears,
			["AgeCategory"] = pc.AgeCategory.DescribeEnum(true),
			["Birthday"] = pc.Birthday.Display(TimeAndDate.Date.CalendarDisplayMode.Long),
			["ShortDescription"] = pc.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
			["FullDescription"] =
				pc.HowSeen(null, type: Form.Shape.DescriptionType.Full, colour: false,
					flags: PerceiveIgnoreFlags.TrueDescription),
			["LocationId"] = pc.Location?.Id ?? 0L,
			["LocationName"] = pc.Location?.HowSeen(null, colour: false) ?? "Unknown"
		};

		if (CustomPlayerInformationProg is not null)
		{
			try
			{
				var customInfo = CustomPlayerInformationProg.ExecuteDictionary<string>(pc);
				foreach (var (key, value) in customInfo)
				{
					result[key] = value;
				}
			}
			catch (Exception e)
			{
				LogStorytellerError(
					$"Custom player information prog failed for storyteller {Id:N0}: {e.Message}");
			}
		}

		result["Memories"] = _characterMemories
			.Where(x => x.Character.Id == pc.Id)
			.Select(x => new Dictionary<string, object>
			{
				["Id"] = x.Id,
				["Title"] = x.MemoryTitle,
				["Details"] = x.MemoryText,
				["CreatedOn"] = x.CreatedOn.ToString("O")
			})
			.ToList();

		return SuccessResult(result);
	}

	private ToolExecutionResult HandleCreateMemory(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Title", out var title, out error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Details", out var details, out error))
		{
			return ErrorResult(error);
		}

		var player = Gameworld.TryGetCharacter(id, true);
		if (player is null)
		{
			return ErrorResult($"No character with id {id:N0} exists.");
		}

		var memory = new AIStorytellerCharacterMemory(this, player, title, details);
		_characterMemories.Add(memory);
		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = memory.Id,
			["CharacterId"] = player.Id,
			["Title"] = memory.MemoryTitle
		});
	}

	private ToolExecutionResult HandleUpdateMemory(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Title", out var title, out error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "Details", out var details, out error))
		{
			return ErrorResult(error);
		}

		var memory = _characterMemories.FirstOrDefault(x => x.Id == id);
		if (memory is null)
		{
			return ErrorResult($"No memory with id {id:N0} exists.");
		}

		memory.UpdateMemory(title, details);
		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = memory.Id,
			["Title"] = memory.MemoryTitle
		});
	}

	private ToolExecutionResult HandleForgetMemory(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		var memory = _characterMemories.FirstOrDefault(x => x.Id == id);
		if (memory is null)
		{
			return ErrorResult($"No memory with id {id:N0} exists.");
		}

		memory.Forget();
		_characterMemories.Remove(memory);
		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = id,
			["Forgot"] = true
		});
	}

	private ToolExecutionResult HandleLandmarks()
	{
		var landmarks = Gameworld.Cells
			.SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
			.Select(x =>
			{
				var cell = (ICell)x.Owner;
				return new Dictionary<string, object>
				{
					["Id"] = x.Name,
					["RoomId"] = cell.Id,
					["RoomName"] = cell.HowSeen(null, colour: false)
				};
			})
			.ToList();

		return SuccessResult(new Dictionary<string, object>
		{
			["Landmarks"] = landmarks
		});
	}

	private ToolExecutionResult HandleShowLandmark(JsonElement arguments)
	{
		if (!TryGetRequiredString(arguments, "Id", out var landmarkId, out var error))
		{
			return ErrorResult(error);
		}

		var landmark = Gameworld.Cells
			.SelectNotNull(x => x.EffectsOfType<LandmarkEffect>().FirstOrDefault())
			.FirstOrDefault(x => x.Name.EqualTo(landmarkId));
		if (landmark is null)
		{
			return ErrorResult($"No landmark with id '{landmarkId}' exists.");
		}

		var cell = (ICell)landmark.Owner;
		var occupants = cell.Characters.Select(x => new Dictionary<string, object>
		{
			["Id"] = x.Id,
			["Name"] = x.PersonalName.GetName(NameStyle.FullName),
			["Gender"] = x.Gender.GenderClass(),
			["ShortDescription"] = x.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)
		}).ToList();

		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = landmark.Name,
			["RoomId"] = cell.Id,
			["RoomName"] = cell.HowSeen(null, colour: false),
			["RoomDescription"] =
				cell.ProcessedFullDescription(null, PerceiveIgnoreFlags.TrueDescription, cell.CurrentOverlay),
			["Details"] = landmark.LandmarkDescriptionTexts.Select(x => x.Text).ToList(),
			["Occupants"] = occupants
		});
	}

	private ToolExecutionResult HandleSearchReferenceDocuments(JsonElement arguments)
	{
		var query = TryGetOptionalString(arguments, "Query") ?? string.Empty;
		var documents = Gameworld.AIStorytellerReferenceDocuments
			.Where(IsReferenceDocumentVisibleToStoryteller)
			.Where(x => x.ReturnForSearch(query))
			.Select(x => new Dictionary<string, object>
			{
				["Id"] = x.Id,
				["Name"] = x.Name,
				["Folder"] = (x as AIStorytellerReferenceDocument)?.FolderName ?? string.Empty,
				["Type"] = (x as AIStorytellerReferenceDocument)?.DocumentType ?? string.Empty,
				["Keywords"] = (x as AIStorytellerReferenceDocument)?.Keywords ?? string.Empty,
				["Description"] = (x as AIStorytellerReferenceDocument)?.Description ?? string.Empty
			})
			.ToList();

		return SuccessResult(new Dictionary<string, object>
		{
			["Query"] = query,
			["Documents"] = documents
		});
	}

	private ToolExecutionResult HandleShowReferenceDocument(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		var document = Gameworld.AIStorytellerReferenceDocuments.Get(id);
		if (document is null || !IsReferenceDocumentVisibleToStoryteller(document))
		{
			return ErrorResult($"No visible reference document with id {id:N0} exists.");
		}

		var concrete = document as AIStorytellerReferenceDocument;
		return SuccessResult(new Dictionary<string, object>
		{
			["Id"] = document.Id,
			["Name"] = document.Name,
			["Folder"] = concrete?.FolderName ?? string.Empty,
			["Type"] = concrete?.DocumentType ?? string.Empty,
			["Keywords"] = concrete?.Keywords ?? string.Empty,
			["Description"] = concrete?.Description ?? string.Empty,
			["Contents"] = concrete?.DocumentContents ?? string.Empty
		});
	}

	private static bool TryResolvePathSearchFunction(string pathSearchFunction, ICharacter? contextCharacter,
		out Func<ICellExit, bool> function, out string resolvedFunctionName, out string error)
	{
		function = PathSearch.IgnorePresenceOfDoors;
		resolvedFunctionName = pathSearchFunction;
		error = string.Empty;

		switch ((pathSearchFunction ?? string.Empty).Trim().ToLowerInvariant())
		{
			case "respectcloseddoors":
				function = PathSearch.RespectClosedDoors;
				resolvedFunctionName = nameof(PathSearch.RespectClosedDoors);
				return true;
			case "pathrespectcloseddoors":
				if (contextCharacter is null)
				{
					function = PathSearch.RespectClosedDoors;
					resolvedFunctionName = nameof(PathSearch.RespectClosedDoors);
				}
				else
				{
					function = PathSearch.PathRespectClosedDoors(contextCharacter);
					resolvedFunctionName = nameof(PathSearch.PathRespectClosedDoors);
				}

				return true;
			case "includeunlockeddoors":
				function = PathSearch.IncludeUnlockedDoors;
				resolvedFunctionName = nameof(PathSearch.IncludeUnlockedDoors);
				return true;
			case "pathincludeunlockeddoors":
				if (contextCharacter is null)
				{
					function = PathSearch.IncludeUnlockedDoors;
					resolvedFunctionName = nameof(PathSearch.IncludeUnlockedDoors);
				}
				else
				{
					function = PathSearch.PathIncludeUnlockedDoors(contextCharacter);
					resolvedFunctionName = nameof(PathSearch.PathIncludeUnlockedDoors);
				}

				return true;
			case "includefireabledoors":
				function = PathSearch.IncludeFireableDoors;
				resolvedFunctionName = nameof(PathSearch.IncludeFireableDoors);
				return true;
			case "ignorepresenceofdoors":
				function = PathSearch.IgnorePresenceOfDoors;
				resolvedFunctionName = nameof(PathSearch.IgnorePresenceOfDoors);
				return true;
			case "ignoredoors":
			case "pathignoredoors":
				if (contextCharacter is null)
				{
					function = PathSearch.IgnorePresenceOfDoors;
					resolvedFunctionName = nameof(PathSearch.IgnorePresenceOfDoors);
				}
				else
				{
					function = PathSearch.PathIgnoreDoors(contextCharacter);
					resolvedFunctionName = nameof(PathSearch.PathIgnoreDoors);
				}

				return true;
			case "includeunlockabledoors":
			case "pathincludeunlockabledoors":
				if (contextCharacter is null)
				{
					error =
						$"PathSearchFunction '{pathSearchFunction}' requires a character context. Use one of RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors or IgnorePresenceOfDoors.";
					return false;
				}

				function = PathSearch.PathIncludeUnlockableDoors(contextCharacter);
				resolvedFunctionName = nameof(PathSearch.PathIncludeUnlockableDoors);
				return true;
		}

		error =
			$"Unknown PathSearchFunction '{pathSearchFunction}'. Valid values are RespectClosedDoors, IncludeUnlockedDoors, IncludeFireableDoors, IgnorePresenceOfDoors, PathIgnoreDoors, PathRespectClosedDoors, PathIncludeUnlockedDoors and PathIncludeUnlockableDoors.";
		return false;
	}

	private static List<string> ConvertPathToDirectionCommands(IEnumerable<ICellExit> path)
	{
		return path.Select(x =>
			x is NonCardinalCellExit nonCardinalExit
				? $"{nonCardinalExit.Verb} {nonCardinalExit.PrimaryKeyword}".ToLowerInvariant()
				: x.OutboundDirection.DescribeBrief()).ToList();
	}

	private ToolExecutionResult HandlePathBetweenRooms(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "OriginRoomId", out var originRoomId, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredLong(arguments, "DestinationRoomId", out var destinationRoomId, out error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "PathSearchFunction", out var pathSearchFunction, out error))
		{
			return ErrorResult(error);
		}

		var origin = Gameworld.Cells.Get(originRoomId);
		if (origin is null)
		{
			return ErrorResult($"No room with id {originRoomId:N0} exists.");
		}

		var destination = Gameworld.Cells.Get(destinationRoomId);
		if (destination is null)
		{
			return ErrorResult($"No room with id {destinationRoomId:N0} exists.");
		}

		if (!TryResolvePathSearchFunction(pathSearchFunction, null, out var pathFunction, out var resolvedPathFunction,
			    out error))
		{
			return ErrorResult(error);
		}

		var path = origin.PathBetween(destination, 50, pathFunction).ToList();
		var sameRoom = origin == destination;

		return SuccessResult(new Dictionary<string, object>
		{
			["OriginRoomId"] = origin.Id,
			["DestinationRoomId"] = destination.Id,
			["PathSearchFunction"] = resolvedPathFunction,
			["HasPath"] = sameRoom || path.Any(),
			["Directions"] = ConvertPathToDirectionCommands(path)
		});
	}

	private ToolExecutionResult HandlePathFromCharacterToRoom(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "OriginCharacterId", out var originCharacterId, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredLong(arguments, "DestinationRoomId", out var destinationRoomId, out error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "PathSearchFunction", out var pathSearchFunction, out error))
		{
			return ErrorResult(error);
		}

		var originCharacter = Gameworld.TryGetCharacter(originCharacterId, true);
		if (originCharacter is null)
		{
			return ErrorResult($"No character with id {originCharacterId:N0} exists.");
		}

		if (originCharacter.Location is null)
		{
			return ErrorResult($"Character {originCharacterId:N0} has no location.");
		}

		var destination = Gameworld.Cells.Get(destinationRoomId);
		if (destination is null)
		{
			return ErrorResult($"No room with id {destinationRoomId:N0} exists.");
		}

		if (!TryResolvePathSearchFunction(pathSearchFunction, originCharacter, out var pathFunction,
			    out var resolvedPathFunction, out error))
		{
			return ErrorResult(error);
		}

		var path = originCharacter.PathBetween(destination, 50, pathFunction).ToList();
		var sameRoom = originCharacter.Location == destination;

		return SuccessResult(new Dictionary<string, object>
		{
			["OriginCharacterId"] = originCharacter.Id,
			["OriginRoomId"] = originCharacter.Location.Id,
			["DestinationRoomId"] = destination.Id,
			["PathSearchFunction"] = resolvedPathFunction,
			["HasPath"] = sameRoom || path.Any(),
			["Directions"] = ConvertPathToDirectionCommands(path)
		});
	}

	private ToolExecutionResult HandlePathBetweenCharacters(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "OriginCharacterId", out var originCharacterId, out var error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredLong(arguments, "DestinationCharacterId", out var destinationCharacterId, out error))
		{
			return ErrorResult(error);
		}

		if (!TryGetRequiredString(arguments, "PathSearchFunction", out var pathSearchFunction, out error))
		{
			return ErrorResult(error);
		}

		var originCharacter = Gameworld.TryGetCharacter(originCharacterId, true);
		if (originCharacter is null)
		{
			return ErrorResult($"No character with id {originCharacterId:N0} exists.");
		}

		if (originCharacter.Location is null)
		{
			return ErrorResult($"Character {originCharacterId:N0} has no location.");
		}

		var destinationCharacter = Gameworld.TryGetCharacter(destinationCharacterId, true);
		if (destinationCharacter is null)
		{
			return ErrorResult($"No character with id {destinationCharacterId:N0} exists.");
		}

		if (destinationCharacter.Location is null)
		{
			return ErrorResult($"Character {destinationCharacterId:N0} has no location.");
		}

		if (!TryResolvePathSearchFunction(pathSearchFunction, originCharacter, out var pathFunction,
			    out var resolvedPathFunction, out error))
		{
			return ErrorResult(error);
		}

		var path = originCharacter.PathBetween(destinationCharacter, 50, pathFunction).ToList();
		var sameRoom = originCharacter.Location == destinationCharacter.Location;

		return SuccessResult(new Dictionary<string, object>
		{
			["OriginCharacterId"] = originCharacter.Id,
			["OriginRoomId"] = originCharacter.Location.Id,
			["DestinationCharacterId"] = destinationCharacter.Id,
			["DestinationRoomId"] = destinationCharacter.Location.Id,
			["PathSearchFunction"] = resolvedPathFunction,
			["HasPath"] = sameRoom || path.Any(),
			["Directions"] = ConvertPathToDirectionCommands(path)
		});
	}

	private ToolExecutionResult HandleRecentCharacterPlans()
	{
		var plans = Gameworld.Characters
			.Where(x => x.AffectedBy<RecentlyUpdatedPlan>())
			.Select(x => (Character: x, Effect: x.EffectsOfType<RecentlyUpdatedPlan>().FirstOrDefault()))
			.Where(x => x.Effect is not null)
			.Select(x =>
			{
				var updatedAgo = x.Character.ScheduledDuration(x.Effect);
				var windowRemaining = TimeSpan.FromDays(90) - updatedAgo;
				return (x.Character, UpdatedAgo: updatedAgo, WindowRemaining: windowRemaining);
			})
			.OrderBy(x => x.WindowRemaining)
			.Select(x => new Dictionary<string, object>
			{
				["Id"] = x.Character.Id,
				["Name"] = x.Character.PersonalName.GetName(NameStyle.FullName),
				["ShortDescription"] =
					x.Character.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
				["ShortTermPlan"] = x.Character.ShortTermPlan ?? string.Empty,
				["LongTermPlan"] = x.Character.LongTermPlan ?? string.Empty,
				["UpdatedAgo"] = x.UpdatedAgo.ToString("c"),
				["WindowRemaining"] = x.WindowRemaining.ToString("c")
			})
			.ToList();

		return SuccessResult(new Dictionary<string, object>
		{
			["WindowDays"] = 90,
			["Plans"] = plans
		});
	}

	private ToolExecutionResult HandleCharacterPlans(JsonElement arguments)
	{
		if (!TryGetRequiredLong(arguments, "Id", out var id, out var error))
		{
			return ErrorResult(error);
		}

		var target = Gameworld.TryGetCharacter(id, true);
		if (target is null)
		{
			return ErrorResult($"No character with id {id:N0} exists.");
		}

		var recentPlanEffect = target.EffectsOfType<RecentlyUpdatedPlan>().FirstOrDefault();
		var updatedAgo = recentPlanEffect is not null ? target.ScheduledDuration(recentPlanEffect) : TimeSpan.Zero;
		var windowRemaining = recentPlanEffect is not null ? TimeSpan.FromDays(90) - updatedAgo : TimeSpan.Zero;

		return SuccessResult(new Dictionary<string, object?>
		{
			["Id"] = target.Id,
			["Name"] = target.PersonalName.GetName(NameStyle.FullName),
			["ShortDescription"] = target.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription),
			["ShortTermPlan"] = target.ShortTermPlan ?? string.Empty,
			["LongTermPlan"] = target.LongTermPlan ?? string.Empty,
			["RecentlyUpdated"] = recentPlanEffect is not null,
			["UpdatedAgo"] = recentPlanEffect is not null ? updatedAgo.ToString("c") : null,
			["WindowRemaining"] = recentPlanEffect is not null ? windowRemaining.ToString("c") : null
		});
	}

	private ToolExecutionResult HandleCustomFunctionCall(string functionName, JsonElement arguments,
		bool includeEchoTools)
	{
		var toolCall = CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(functionName));
		if (toolCall is null && includeEchoTools)
		{
			toolCall = CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(functionName));
		}

		if (toolCall is null)
		{
			return ErrorResult($"Unknown function '{functionName}'.");
		}

		if (toolCall.Prog is null)
		{
			return ErrorResult($"Custom tool '{functionName}' has no prog.");
		}

		if (!string.IsNullOrWhiteSpace(toolCall.Prog.CompileError))
		{
			return ErrorResult(
				$"Custom tool '{functionName}' is invalid because its prog does not compile.");
		}

		var progArguments = new List<object>();
		foreach (var (parameterType, parameterName) in toolCall.Prog.NamedParameters)
		{
			if (!arguments.TryGetProperty(parameterName, out var argumentValue))
			{
				return ErrorResult($"Missing required custom-tool parameter '{parameterName}'.");
			}

			if (!TryConvertJsonArgument(argumentValue, parameterType, out var convertedValue, out var error))
			{
				return ErrorResult(error);
			}

			progArguments.Add(convertedValue);
		}

		var result = toolCall.Prog.ExecuteWithRecursionProtection(progArguments.ToArray());
		return SuccessResult(new Dictionary<string, object>
		{
			["Function"] = functionName,
			["ReturnType"] = toolCall.Prog.ReturnType.Describe(),
			["Result"] = ConvertToToolOutputValue(result)
		});
	}

	private bool TryConvertJsonArgument(JsonElement element, ProgVariableTypes type, out object? convertedValue,
		out string error)
	{
		type &= ~ProgVariableTypes.Literal;
		if (type.HasFlag(ProgVariableTypes.Collection))
		{
			if (element.ValueKind != JsonValueKind.Array)
			{
				convertedValue = null;
				error = $"Expected array value for parameter type {type.Describe()}.";
				return false;
			}

			var innerType = type ^ ProgVariableTypes.Collection;
			var list = new List<object?>();
			foreach (var item in element.EnumerateArray())
			{
				if (!TryConvertJsonArgument(item, innerType, out var value, out error))
				{
					convertedValue = null;
					return false;
				}

				list.Add(value);
			}

			convertedValue = list;
			error = string.Empty;
			return true;
		}

		if (element.ValueKind == JsonValueKind.Null && ProgVariableTypes.ReferenceType.HasFlag(type))
		{
			convertedValue = null;
			error = string.Empty;
			return true;
		}

		switch (type)
		{
			case ProgVariableTypes.Boolean:
				if (element.ValueKind == JsonValueKind.True || element.ValueKind == JsonValueKind.False)
				{
					convertedValue = element.GetBoolean();
					error = string.Empty;
					return true;
				}

				if (element.ValueKind == JsonValueKind.String &&
				    bool.TryParse(element.GetString(), out var boolValue))
				{
					convertedValue = boolValue;
					error = string.Empty;
					return true;
				}

				convertedValue = null;
				error = "Expected boolean argument.";
				return false;
			case ProgVariableTypes.Number:
				if (element.ValueKind == JsonValueKind.Number && element.TryGetDecimal(out var numberValue))
				{
					convertedValue = numberValue;
					error = string.Empty;
					return true;
				}

				if (element.ValueKind == JsonValueKind.String &&
				    decimal.TryParse(element.GetString(), out numberValue))
				{
					convertedValue = numberValue;
					error = string.Empty;
					return true;
				}

				convertedValue = null;
				error = "Expected numeric argument.";
				return false;
			case ProgVariableTypes.Text:
				convertedValue = element.ValueKind == JsonValueKind.String
					? element.GetString() ?? string.Empty
					: element.ToString();
				error = string.Empty;
				return true;
			case ProgVariableTypes.Gender:
				if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var numericGender) &&
				    Enum.IsDefined(typeof(Gender), numericGender))
				{
					convertedValue = (Gender)numericGender;
					error = string.Empty;
					return true;
				}

				if (element.ValueKind == JsonValueKind.String)
				{
					switch ((element.GetString() ?? string.Empty).Trim().ToLowerInvariant())
					{
						case "male":
							convertedValue = Gender.Male;
							error = string.Empty;
							return true;
						case "female":
							convertedValue = Gender.Female;
							error = string.Empty;
							return true;
						case "neuter":
							convertedValue = Gender.Neuter;
							error = string.Empty;
							return true;
						case "non-binary":
						case "nonbinary":
						case "nb":
							convertedValue = Gender.NonBinary;
							error = string.Empty;
							return true;
						case "indeterminate":
							convertedValue = Gender.Indeterminate;
							error = string.Empty;
							return true;
					}
				}

				convertedValue = null;
				error = "Expected a gender argument.";
				return false;
			case ProgVariableTypes.TimeSpan:
				if (element.ValueKind == JsonValueKind.String &&
				    TimeSpan.TryParse(element.GetString(), out var timeSpan))
				{
					convertedValue = timeSpan;
					error = string.Empty;
					return true;
				}

				convertedValue = null;
				error = "Expected timespan text argument.";
				return false;
			case ProgVariableTypes.DateTime:
				if (element.ValueKind == JsonValueKind.String &&
				    DateTime.TryParse(element.GetString(), out var dateTime))
				{
					convertedValue = dateTime;
					error = string.Empty;
					return true;
				}

				convertedValue = null;
				error = "Expected datetime text argument.";
				return false;
			case ProgVariableTypes.MudDateTime:
				if (element.ValueKind == JsonValueKind.String &&
				    MudDateTime.TryParse(element.GetString(), Gameworld, out var mudDateTime))
				{
					convertedValue = mudDateTime;
					error = string.Empty;
					return true;
				}

				convertedValue = null;
				error = "Expected mud datetime text argument.";
				return false;
		}

		if (!TryReadId(element, out var id))
		{
			convertedValue = null;
			error = $"Expected an id argument for parameter type {type.Describe()}.";
			return false;
		}

		convertedValue = type switch
		{
			ProgVariableTypes.Character or ProgVariableTypes.Toon => Gameworld.TryGetCharacter(id, true),
			ProgVariableTypes.Item => Gameworld.Items.Get(id),
			ProgVariableTypes.Location => Gameworld.Cells.Get(id),
			ProgVariableTypes.Shard => Gameworld.Shards.Get(id),
			ProgVariableTypes.Zone => Gameworld.Zones.Get(id),
			ProgVariableTypes.Race => Gameworld.Races.Get(id),
			ProgVariableTypes.Culture => Gameworld.Cultures.Get(id),
			ProgVariableTypes.Ethnicity => Gameworld.Ethnicities.Get(id),
			ProgVariableTypes.Trait => Gameworld.Traits.Get(id),
			ProgVariableTypes.Clan => Gameworld.Clans.Get(id),
			ProgVariableTypes.ClanRank => Gameworld.Clans
				.SelectMany(x => x.Ranks)
				.FirstOrDefault(x => x.Id == id),
			ProgVariableTypes.ClanAppointment => Gameworld.Clans
				.SelectMany(x => x.Appointments)
				.FirstOrDefault(x => x.Id == id),
			ProgVariableTypes.ClanPaygrade => Gameworld.Clans
				.SelectMany(x => x.Paygrades)
				.FirstOrDefault(x => x.Id == id),
			ProgVariableTypes.Currency => Gameworld.Currencies.Get(id),
			ProgVariableTypes.Exit => Gameworld.ExitManager.GetExitByID(id),
			ProgVariableTypes.Language => Gameworld.Languages.Get(id),
			ProgVariableTypes.Accent => Gameworld.Accents.Get(id),
			ProgVariableTypes.Merit => Gameworld.Merits.Get(id),
			ProgVariableTypes.Calendar => Gameworld.Calendars.Get(id),
			ProgVariableTypes.Clock => Gameworld.Clocks.Get(id),
			ProgVariableTypes.Knowledge => Gameworld.Knowledges.Get(id),
			ProgVariableTypes.Role => Gameworld.Roles.Get(id),
			ProgVariableTypes.Drug => Gameworld.Drugs.Get(id),
			ProgVariableTypes.WeatherEvent => Gameworld.WeatherEvents.Get(id),
			ProgVariableTypes.Shop => Gameworld.Shops.Get(id),
			ProgVariableTypes.Merchandise => Gameworld.Shops
				.SelectMany(x => x.Merchandises)
				.FirstOrDefault(x => x.Id == id),
			ProgVariableTypes.Script => Gameworld.Scripts.Get(id),
			ProgVariableTypes.Writing => Gameworld.Writings.Get(id),
			ProgVariableTypes.OverlayPackage => Gameworld.CellOverlayPackages.Get(id),
			ProgVariableTypes.Terrain => Gameworld.Terrains.Get(id),
			ProgVariableTypes.Solid => Gameworld.Materials.Get(id),
			ProgVariableTypes.Liquid => Gameworld.Liquids.Get(id),
			ProgVariableTypes.Gas => Gameworld.Gases.Get(id),
			ProgVariableTypes.Material => (object?)Gameworld.Materials.Get(id) ?? (object?)Gameworld.Liquids.Get(id) ??
			                              Gameworld.Gases.Get(id),
			ProgVariableTypes.MagicSchool => Gameworld.MagicSchools.Get(id),
			ProgVariableTypes.MagicCapability => Gameworld.MagicCapabilities.Get(id),
			ProgVariableTypes.MagicSpell => Gameworld.MagicSpells.Get(id),
			ProgVariableTypes.Bank => Gameworld.Banks.Get(id),
			ProgVariableTypes.BankAccount => Gameworld.BankAccounts.Get(id),
			ProgVariableTypes.BankAccountType => Gameworld.BankAccountTypes.Get(id),
			ProgVariableTypes.Project => Gameworld.ActiveProjects.Get(id),
			ProgVariableTypes.Law => Gameworld.Laws.Get(id),
			ProgVariableTypes.LegalAuthority => Gameworld.LegalAuthorities.Get(id),
			ProgVariableTypes.Market => Gameworld.Markets.Get(id),
			ProgVariableTypes.MarketCategory => Gameworld.MarketCategories.Get(id),
			ProgVariableTypes.Crime => Gameworld.Crimes.Get(id),
			ProgVariableTypes.Area => Gameworld.Areas.Get(id),
			ProgVariableTypes.Tagged => (object?)Gameworld.Cells.Get(id) ?? (object?)Gameworld.Items.Get(id) ??
			                            Gameworld.Terrains.Get(id),
			ProgVariableTypes.Perceivable or ProgVariableTypes.Perceiver or ProgVariableTypes.MagicResourceHaver =>
				(object?)Gameworld.TryGetCharacter(id, true) ?? (object?)Gameworld.Items.Get(id) ??
				Gameworld.Cells.Get(id),
			_ => null
		};

		if (convertedValue is null)
		{
			error = type switch
			{
				ProgVariableTypes.Effect or ProgVariableTypes.Outfit or ProgVariableTypes.OutfitItem =>
					$"Parameter type {type.Describe()} is not yet supported for custom tools.",
				_ => $"No game object exists for id {id:N0} as required by {type.Describe()}."
			};
			return false;
		}

		error = string.Empty;
		return true;
	}

	private object? ConvertToToolOutputValue(object? value)
	{
		switch (value)
		{
			case null:
				return null;
			case string or bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or
				double or decimal:
				return value;
			case DateTime dateTime:
				return dateTime.ToString("O");
			case TimeSpan timeSpan:
				return timeSpan.ToString("c");
			case MudDateTime mudDateTime:
				return mudDateTime.ToString();
			case IDictionary dictionary:
			{
				var result = new Dictionary<string, object?>();
				foreach (DictionaryEntry entry in dictionary)
				{
					result[entry.Key?.ToString() ?? string.Empty] = ConvertToToolOutputValue(entry.Value);
				}

				return result;
			}
			case IEnumerable enumerable when value is not string:
			{
				var list = new List<object?>();
				foreach (var item in enumerable)
				{
					list.Add(ConvertToToolOutputValue(item));
				}

				return list;
			}
			case IPerceivable perceivable:
				return new Dictionary<string, object?>
				{
					["Id"] = perceivable.Id,
					["Name"] = perceivable.Name,
					["Type"] = perceivable.FrameworkItemType
				};
			case IFrameworkItem frameworkItem:
				return new Dictionary<string, object?>
				{
					["Id"] = frameworkItem.Id,
					["Name"] = frameworkItem.Name,
					["Type"] = frameworkItem.FrameworkItemType
				};
			default:
				return value.ToString() ?? string.Empty;
		}
	}

	private void AppendOpenSituationTitles(StringBuilder sb)
	{
		var openSituations = _situations
			.Where(x => !x.IsResolved)
			.Take(MaxSituationTitlesInPrompt)
			.ToList();

		if (!openSituations.Any())
		{
			sb.AppendLine("There are currently no unresolved situations.");
			return;
		}

		sb.AppendLine("Current unresolved situations:");
		foreach (var item in openSituations)
		{
			sb.AppendLine($"- #{item.Id:N0}: {item.Name}");
		}
	}

	private static string TrimPromptText(string text)
	{
		if (text.Length <= MaxPromptCharacters)
		{
			return text;
		}

		return $"{text[..MaxPromptCharacters]}\n\n[Prompt truncated due to context budget.]";
	}

	private bool IsReferenceDocumentVisibleToStoryteller(IAIStorytellerReferenceDocument document)
	{
		return document.IsVisibleTo(this);
	}

	internal void CellOnRoomEchoForTesting(ICell location, PerceptionEngine.IEmoteOutput emote)
	{
		Cell_OnRoomEcho(location, null, emote);
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

		try
		{
			ResponsesClient client = new(Model, apiKey);
			var options = new CreateResponseOptions([
				ResponseItem.CreateDeveloperMessageItem(AttentionAgentPrompt),
				ResponseItem.CreateUserMessageItem(TrimPromptText(echo))
			]);
			options.ReasoningOptions.ReasoningEffortLevel = ResponseReasoningEffortLevel.Minimal;
			ResponseResult attention = client.CreateResponseAsync(options).GetAwaiter().GetResult().Value;
			var ss = new StringStack(attention.GetOutputText());
			if (!ss.Pop().EqualTo("interested"))
			{
				return;
			}

			PassSituationToAIStoryteller(location, emote, echo, ss.SafeRemainingArgument);
		}
		catch (Exception e)
		{
			LogStorytellerException(e);
		}
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

		Gameworld.Destroy(this);
	}

	public void Pause() { IsPaused = true; Changed = true; }
	public void Unpause() { IsPaused = false; Changed = true; }

	private const string HelpText = @"You can use the following options:

	#3name <name>#0 - renames this storyteller
	#3description#0 - edits the description text in an editor
	#3model <model>#0 - sets the OpenAI model
	#3reasoning <minimal|low|medium|high>#0 - sets reasoning effort
	#3attention#0 - edits the attention prompt in an editor
	#3system#0 - edits the system prompt in an editor
	#3pause#0 - pauses all storyteller triggers
	#3unpause#0 - unpauses all storyteller triggers
	#3subscribe room|5m|10m|30m|1h#0 - toggles trigger subscriptions
	#3statusprog <5m|10m|30m|1h> <prog|none>#0 - sets a heartbeat status prog
	#3customplayerprog <prog|none>#0 - sets an optional custom player info prog
	#3surveillance <...>#0 - edits surveillance strategy details
	#3tool add <name> <prog> [echo]#0 - adds a custom tool
	#3tool remove <name>#0 - removes a custom tool
	#3tool description <name> <text>#0 - sets a tool description
	#3tool parameter <name> <parameter> <description>#0 - sets a parameter description
	#3tool prog <name> <prog>#0 - changes the prog bound to a tool
	#3tool echo <name>#0 - toggles whether a tool is echo-only
	#3refsearch <query>#0 - searches visible reference documents";

	public bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, command);
			case "description":
			case "desc":
				return BuildingCommandDescription(actor);
			case "model":
				return BuildingCommandModel(actor, command);
			case "reasoning":
			case "effort":
				return BuildingCommandReasoning(actor, command);
			case "attention":
			case "attentionprompt":
				return BuildingCommandAttentionPrompt(actor);
			case "system":
			case "systemprompt":
				return BuildingCommandSystemPrompt(actor);
			case "pause":
				if (IsPaused)
				{
					actor.OutputHandler.Send("This storyteller is already paused.");
					return false;
				}

				Pause();
				actor.OutputHandler.Send("This storyteller is now paused.");
				return true;
			case "unpause":
				if (!IsPaused)
				{
					actor.OutputHandler.Send("This storyteller is already unpaused.");
					return false;
				}

				Unpause();
				actor.OutputHandler.Send("This storyteller is now unpaused.");
				return true;
			case "subscribe":
			case "subscriptions":
				return BuildingCommandSubscribe(actor, command);
			case "statusprog":
			case "heartbeatprog":
				return BuildingCommandStatusProg(actor, command);
			case "customplayerprog":
			case "playerprog":
				return BuildingCommandCustomPlayerProg(actor, command);
			case "surveillance":
			case "watch":
				return BuildingCommandSurveillance(actor, command);
			case "tool":
			case "tools":
				return BuildingCommandTool(actor, command);
			case "refsearch":
			case "reference":
			case "references":
				return BuildingCommandReferenceSearch(actor, command);
			default:
				actor.OutputHandler.Send(HelpText.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandName(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What name should this storyteller have?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		if (Gameworld.AIStorytellers.Any(x => x.Id != Id && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already an AI storyteller named {name.ColourName()}.");
			return false;
		}

		_name = name;
		Changed = true;
		actor.OutputHandler.Send($"This storyteller is now named {Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandDescription(ICharacter actor)
	{
		actor.OutputHandler.Send("Enter the new description in the editor below.");
		actor.EditorMode(BuildingCommandDescriptionPost, BuildingCommandDescriptionCancel, 1.0, Description);
		return true;
	}

	private void BuildingCommandDescriptionCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the storyteller description.");
	}

	private void BuildingCommandDescriptionPost(string text, IOutputHandler handler, object[] args)
	{
		Description = text;
		Changed = true;
		handler.Send("You update the storyteller description.");
	}

	private bool BuildingCommandModel(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which OpenAI model should this storyteller use?");
			return false;
		}

		Model = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"This storyteller now uses the {Model.ColourValue()} model.");
		return true;
	}

	private bool BuildingCommandReasoning(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify one of {"minimal".ColourCommand()}, {"low".ColourCommand()}, {"medium".ColourCommand()} or {"high".ColourCommand()}.");
			return false;
		}

		if (!TryParseReasoningEffort(command.SafeRemainingArgument, out var effort))
		{
			actor.OutputHandler.Send(
				$"That is not a valid reasoning effort. Use {"minimal".ColourCommand()}, {"low".ColourCommand()}, {"medium".ColourCommand()} or {"high".ColourCommand()}.");
			return false;
		}

		ReasoningEffort = effort;
		Changed = true;
		actor.OutputHandler.Send(
			$"This storyteller now uses {ReasoningEffort.Describe().ColourValue()} reasoning effort.");
		return true;
	}

	private bool BuildingCommandAttentionPrompt(ICharacter actor)
	{
		actor.OutputHandler.Send("Enter the new attention prompt in the editor below.");
		actor.EditorMode(BuildingCommandAttentionPromptPost, BuildingCommandAttentionPromptCancel, 1.0,
			AttentionAgentPrompt);
		return true;
	}

	private void BuildingCommandAttentionPromptCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the attention prompt.");
	}

	private void BuildingCommandAttentionPromptPost(string text, IOutputHandler handler, object[] args)
	{
		AttentionAgentPrompt = text;
		Changed = true;
		handler.Send("You update the attention prompt.");
	}

	private bool BuildingCommandSystemPrompt(ICharacter actor)
	{
		actor.OutputHandler.Send("Enter the new system prompt in the editor below.");
		actor.EditorMode(BuildingCommandSystemPromptPost, BuildingCommandSystemPromptCancel, 1.0, SystemPrompt);
		return true;
	}

	private void BuildingCommandSystemPromptCancel(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to change the system prompt.");
	}

	private void BuildingCommandSystemPromptPost(string text, IOutputHandler handler, object[] args)
	{
		SystemPrompt = text;
		Changed = true;
		handler.Send("You update the system prompt.");
	}

	private bool BuildingCommandSubscribe(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				$"You must specify one of {"room".ColourCommand()}, {"5m".ColourCommand()}, {"10m".ColourCommand()}, {"30m".ColourCommand()} or {"1h".ColourCommand()}.");
			return false;
		}

		switch (command.PopForSwitch())
		{
			case "room":
			case "echo":
			case "echoes":
				SubscribeToRoomEvents = !SubscribeToRoomEvents;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send($"Subscribe To Room Events is now {SubscribeToRoomEvents.ToColouredString()}.");
				return true;
			case "5m":
			case "5":
				SubscribeTo5mHeartbeat = !SubscribeTo5mHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send($"Subscribe To 5m Tick is now {SubscribeTo5mHeartbeat.ToColouredString()}.");
				return true;
			case "10m":
			case "10":
				SubscribeTo10mHeartbeat = !SubscribeTo10mHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send(
					$"Subscribe To 10m Tick is now {SubscribeTo10mHeartbeat.ToColouredString()}.");
				return true;
			case "30m":
			case "30":
				SubscribeTo30mHeartbeat = !SubscribeTo30mHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send(
					$"Subscribe To 30m Tick is now {SubscribeTo30mHeartbeat.ToColouredString()}.");
				return true;
			case "1h":
			case "hour":
			case "60m":
				SubscribeToHourHeartbeat = !SubscribeToHourHeartbeat;
				Changed = true;
				SubscribeEvents();
				actor.OutputHandler.Send($"Subscribe To 1h Tick is now {SubscribeToHourHeartbeat.ToColouredString()}.");
				return true;
			default:
				actor.OutputHandler.Send(
					$"You must specify one of {"room".ColourCommand()}, {"5m".ColourCommand()}, {"10m".ColourCommand()}, {"30m".ColourCommand()} or {"1h".ColourCommand()}.");
				return false;
		}
	}

	private bool BuildingCommandStatusProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which heartbeat interval do you want to edit? (5m, 10m, 30m, 1h)");
			return false;
		}

		var which = command.PopForSwitch();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog or use #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		IFutureProg prog = null;
		if (!command.SafeRemainingArgument.EqualTo("none"))
		{
			prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
			if (prog is null)
			{
				actor.OutputHandler.Send("There is no such prog.");
				return false;
			}
		}

		switch (which)
		{
			case "5m":
			case "5":
				HeartbeatStatus5mProg = prog;
				break;
			case "10m":
			case "10":
				HeartbeatStatus10mProg = prog;
				break;
			case "30m":
			case "30":
				HeartbeatStatus30mProg = prog;
				break;
			case "1h":
			case "hour":
			case "60m":
				HeartbeatStatus1hProg = prog;
				break;
			default:
				actor.OutputHandler.Send("You must specify one of 5m, 10m, 30m or 1h.");
				return false;
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"That heartbeat status prog is now set to {prog?.MXPClickableFunctionName() ?? "None".ColourError()}.");
		return true;
	}

	private bool BuildingCommandCustomPlayerProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must specify a prog or use #3none#0 to clear it.".SubstituteANSIColour());
			return false;
		}

		if (command.SafeRemainingArgument.EqualTo("none"))
		{
			CustomPlayerInformationProg = null;
			Changed = true;
			actor.OutputHandler.Send("This storyteller will no longer use a custom player information prog.");
			return true;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		CustomPlayerInformationProg = prog;
		Changed = true;
		actor.OutputHandler.Send(
			$"This storyteller will now use {prog.MXPClickableFunctionName()} for custom player information.");
		return true;
	}

	private bool BuildingCommandSurveillance(ICharacter actor, StringStack command)
	{
		var result = SurveillanceStrategy.BuildingCommand(actor, command);
		if (!result)
		{
			return false;
		}

		Changed = true;
		SubscribeEvents();
		return true;
	}

	private bool BuildingCommandTool(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "add":
			case "new":
				return BuildingCommandToolAdd(actor, command);
			case "remove":
			case "delete":
				return BuildingCommandToolRemove(actor, command);
			case "description":
			case "desc":
				return BuildingCommandToolDescription(actor, command);
			case "parameter":
			case "param":
				return BuildingCommandToolParameter(actor, command);
			case "prog":
				return BuildingCommandToolProg(actor, command);
			case "echo":
				return BuildingCommandToolEcho(actor, command);
			default:
				actor.OutputHandler.Send(@"You can use the following options:

	#3tool add <name> <prog> [echo]#0 - adds a new custom tool
	#3tool remove <name>#0 - removes a custom tool
	#3tool description <name> <text>#0 - sets a custom tool description
	#3tool parameter <name> <parameter> <description>#0 - sets parameter description text
	#3tool prog <name> <prog>#0 - changes the prog bound to a tool
	#3tool echo <name>#0 - toggles whether a tool is echo-only".SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandToolAdd(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What should this tool be called?");
			return false;
		}

		var name = command.PopSpeech();
		if (CustomToolCalls.Any(x => x.Name.EqualTo(name)) || CustomToolCallsEchoOnly.Any(x => x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send("There is already a custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should this tool call?");
			return false;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.PopSpeech());
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		var includeWithEcho = !command.IsFinished && command.PopForSwitch().StartsWith("echo");
		var tool = new AIStorytellerCustomToolCall(name, $"Custom tool {name}", prog);
		if (includeWithEcho)
		{
			CustomToolCallsEchoOnly.Add(tool);
		}
		else
		{
			CustomToolCalls.Add(tool);
		}

		Changed = true;
		actor.OutputHandler.Send(
			$"You create the custom tool {name.ColourCommand()} using {prog.MXPClickableFunctionName()} ({(includeWithEcho ? "echo-only" : "always available").ColourValue()}).");
		return true;
	}

	private bool BuildingCommandToolRemove(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to remove?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var tool = CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCalls.Remove(tool);
			Changed = true;
			actor.OutputHandler.Send($"You remove the custom tool {tool.Name.ColourCommand()}.");
			return true;
		}

		tool = CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCallsEchoOnly.Remove(tool);
			Changed = true;
			actor.OutputHandler.Send($"You remove the custom tool {tool.Name.ColourCommand()}.");
			return true;
		}

		actor.OutputHandler.Send("There is no custom tool with that name.");
		return false;
	}

	private bool BuildingCommandToolDescription(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to edit?");
			return false;
		}

		var tool = FindCustomTool(command.PopSpeech());
		if (tool is null)
		{
			actor.OutputHandler.Send("There is no custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should this custom tool have?");
			return false;
		}

		tool.Description = command.SafeRemainingArgument;
		Changed = true;
		actor.OutputHandler.Send($"Tool {tool.Name.ColourCommand()} now has updated description text.");
		return true;
	}

	private bool BuildingCommandToolParameter(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to edit?");
			return false;
		}

		var tool = FindCustomTool(command.PopSpeech());
		if (tool is null)
		{
			actor.OutputHandler.Send("There is no custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which parameter do you want to edit?");
			return false;
		}

		var parameter = tool.Prog?.NamedParameters
			.Select(x => x.Item2)
			.FirstOrDefault(x => x.EqualTo(command.PopSpeech()));
		if (parameter is null)
		{
			actor.OutputHandler.Send("There is no such named parameter on that tool's prog.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("What description should this parameter have?");
			return false;
		}

		tool.SetParameterDescription(parameter, command.SafeRemainingArgument);
		Changed = true;
		actor.OutputHandler.Send(
			$"Tool {tool.Name.ColourCommand()} parameter {parameter.ColourCommand()} now has updated description text.");
		return true;
	}

	private bool BuildingCommandToolProg(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to edit?");
			return false;
		}

		var tool = FindCustomTool(command.PopSpeech());
		if (tool is null)
		{
			actor.OutputHandler.Send("There is no custom tool with that name.");
			return false;
		}

		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which prog should this tool call?");
			return false;
		}

		var prog = Gameworld.FutureProgs.GetByIdOrName(command.SafeRemainingArgument);
		if (prog is null)
		{
			actor.OutputHandler.Send("There is no such prog.");
			return false;
		}

		tool.Prog = prog;
		tool.RefreshParameterDescriptions();
		Changed = true;
		actor.OutputHandler.Send(
			$"Tool {tool.Name.ColourCommand()} now calls {prog.MXPClickableFunctionName()}.");
		return true;
	}

	private bool BuildingCommandToolEcho(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which custom tool do you want to toggle?");
			return false;
		}

		var name = command.SafeRemainingArgument;
		var tool = CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCalls.Remove(tool);
			CustomToolCallsEchoOnly.Add(tool);
			Changed = true;
			actor.OutputHandler.Send($"Tool {tool.Name.ColourCommand()} is now echo-only.");
			return true;
		}

		tool = CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(name));
		if (tool is not null)
		{
			CustomToolCallsEchoOnly.Remove(tool);
			CustomToolCalls.Add(tool);
			Changed = true;
			actor.OutputHandler.Send($"Tool {tool.Name.ColourCommand()} is now always available.");
			return true;
		}

		actor.OutputHandler.Send("There is no custom tool with that name.");
		return false;
	}

	private AIStorytellerCustomToolCall FindCustomTool(string name)
	{
		return CustomToolCalls.FirstOrDefault(x => x.Name.EqualTo(name)) ??
		       CustomToolCallsEchoOnly.FirstOrDefault(x => x.Name.EqualTo(name));
	}

	private bool BuildingCommandReferenceSearch(ICharacter actor, StringStack command)
	{
		var query = command.SafeRemainingArgument;
		var docs = Gameworld.AIStorytellerReferenceDocuments
			.Where(IsReferenceDocumentVisibleToStoryteller)
			.Where(x => x.ReturnForSearch(query))
			.ToList();

		if (!docs.Any())
		{
			actor.OutputHandler.Send("There are no matching reference documents.");
			return false;
		}

		actor.OutputHandler.Send(StringUtilities.GetTextTable(
			from item in docs
			let concrete = item as AIStorytellerReferenceDocument
			select new List<string>
			{
				item.Id.ToStringN0(actor),
				item.Name,
				concrete?.FolderName ?? string.Empty,
				concrete?.DocumentType ?? string.Empty,
				concrete?.Keywords ?? string.Empty
			},
			[
				"Id",
				"Name",
				"Folder",
				"Type",
				"Keywords"
			],
			actor,
			Telnet.Green));
		return false;
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"AI Storyteller #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan,
			Telnet.BoldWhite));
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
		sb.AppendLine(
			$"Custom Player Info Prog: {CustomPlayerInformationProg?.MXPClickableFunctionName() ?? "None".ColourError()}");
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
		dbitem.Model = Model;
		dbitem.AttentionAgentPrompt = AttentionAgentPrompt;
		dbitem.SystemPrompt = SystemPrompt;
		dbitem.SurveillanceStrategyDefinition = SurveillanceStrategy.SaveDefinition();
		dbitem.ReasoningEffort = SerializeReasoningEffort(ReasoningEffort);
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
		dbitem.CustomToolCallsDefinition = BuildCustomToolRoot().ToString();
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


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
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public partial class AIStoryteller : SaveableItem, IAIStoryteller
{
	public override string FrameworkItemType => "AIStoryteller";
	private const int MaxToolCallDepth = 16;
	private const int MaxMalformedToolCallRetries = 3;
	private static readonly TimeSpan MaxToolExecutionDuration = TimeSpan.FromSeconds(30);
	private const int MaxSituationTitlesInPrompt = 25;
	private const int MaxPromptCharacters = 24_000;
	private const int MaxDebugMessageCharacters = 32_000;
	private const string AttentionContractInstruction =
		"""Return only strict JSON in this shape: {"Decision":"interested","Reason":"optional short reason"} or {"Decision":"ignore"}. Do not include any additional text.""";

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
		SubscribeToSpeechEvents = storyteller.SubscribeToSpeechEvents;
		SubscribeToCrimeEvents = storyteller.SubscribeToCrimeEvents;
		SubscribeToStateEvents = storyteller.SubscribeToStateEvents;
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
		SubscribeToSpeechEvents = false;
		SubscribeToCrimeEvents = false;
		SubscribeToStateEvents = false;
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
				SubscribeToSpeechEvents = SubscribeToSpeechEvents,
				SubscribeToCrimeEvents = SubscribeToCrimeEvents,
				SubscribeToStateEvents = SubscribeToStateEvents,
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
	public bool SubscribeToSpeechEvents { get; private set; }
	public bool SubscribeToCrimeEvents { get; private set; }
	public bool SubscribeToStateEvents { get; private set; }
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

	private void DebugAIMessaging(string stage, string payload)
	{
		if (string.IsNullOrWhiteSpace(payload))
		{
			payload = "(empty)";
		}
		else if (payload.Length > MaxDebugMessageCharacters)
		{
			payload = $"{payload[..MaxDebugMessageCharacters]}\n[Debug payload truncated.]";
		}

		Gameworld.DebugMessage($"[AI Storyteller #{Id:N0} - {Name}] {stage}\n{payload}");
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

		var cells = SurveillanceStrategy.GetCells(Gameworld).ToList();
		_subscribedCells.Clear();
		_subscribedCells.AddRange(cells);
		if (SubscribeToRoomEvents)
		{
			foreach (var cell in cells)
			{
				cell.OnRoomEmoteEcho += Cell_OnRoomEcho;
			}
		}
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
		ExecuteStorytellerPrompt(apiKey, $"Heartbeat {heartbeatType}", sb.ToString(), includeEchoTools: false);
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
		ExecuteStorytellerPrompt(apiKey, "Room Echo", sb.ToString(), includeEchoTools: true);
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
		var attentionPrompt = TrimPromptText(echo);
		var classifierPrompt =
			$"{AttentionAgentPrompt.IfNullOrWhiteSpace(string.Empty).Trim()}\n\n{AttentionContractInstruction}";

		try
		{
			DebugAIMessaging("Engine -> Attention Classifier Request",
				$"""
Model: {Model}
Reasoning: {ResponseReasoningEffortLevel.Minimal.Describe()}
Prompt:
{classifierPrompt}

Echo:
{attentionPrompt}
""");
			ResponsesClient client = new(Model, apiKey);
			var options = new CreateResponseOptions([
				ResponseItem.CreateDeveloperMessageItem(classifierPrompt),
				ResponseItem.CreateUserMessageItem(attentionPrompt)
			]);
			options.ReasoningOptions ??= new();
			options.ReasoningOptions.ReasoningEffortLevel = ResponseReasoningEffortLevel.Minimal;
			ResponseResult attention = client.CreateResponseAsync(options).GetAwaiter().GetResult().Value;
			var attentionResponse = attention.GetOutputText();
			DebugAIMessaging("Attention Classifier -> Engine Response", attentionResponse);
			if (!TryInterpretAttentionClassifierOutput(attentionResponse, out var interested, out var reason))
			{
				DebugAIMessaging("Attention Classifier Decision",
					"Invalid response ignored due to contract violation.");
				return;
			}

			if (!interested)
			{
				DebugAIMessaging("Attention Classifier Decision", "Ignored event.");
				return;
			}

			DebugAIMessaging("Attention Classifier Decision",
				$"Interested. Reason: {reason.IfNullOrWhiteSpace("No reason provided")}");
			PassSituationToAIStoryteller(location, emote, echo, reason);
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
		dbitem.SubscribeToSpeechEvents = SubscribeToSpeechEvents;
		dbitem.SubscribeToCrimeEvents = SubscribeToCrimeEvents;
		dbitem.SubscribeToStateEvents = SubscribeToStateEvents;
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


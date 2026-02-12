using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using OpenAI.Responses;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

namespace MudSharp.RPG.AIStorytellers;

public enum AIStorytellerStateTriggerType
{
	Unconscious,
	PassOut,
	Dead
}

public partial class AIStoryteller
{
	public bool InvokeDirectAttention(string attentionText)
	{
		if (IsPaused || string.IsNullOrWhiteSpace(attentionText))
		{
			return false;
		}

		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return false;
		}

		var sb = new StringBuilder();
		AppendOpenSituationTitles(sb);
		sb.AppendLine("A direct storyteller attention event was invoked via FutureProg.");
		sb.AppendLine("Event Text:");
		sb.AppendLine(attentionText.Trim());
		ExecuteStorytellerPrompt(apiKey, "Direct Invocation", sb.ToString(), includeEchoTools: false);
		return true;
	}

	internal static void HandleCharacterSpeechInRoomEvent(ICharacter speaker, IPerceivable? target, string message,
		AudioVolume volume, ILanguage language, IAccent accent)
	{
		if (speaker.Location is not ICell location)
		{
			return;
		}

		foreach (var storyteller in speaker.Gameworld.AIStorytellers.OfType<AIStoryteller>())
		{
			if (!storyteller.SubscribeToSpeechEvents || storyteller.IsPaused || !storyteller.IsCellSurveilled(location))
			{
				continue;
			}

			storyteller.PassSpeechEventToAIStoryteller(location, speaker, target, message, volume, language, accent);
		}
	}

	internal static void HandleCrimeCommittedInRoomEvent(ICrime crime)
	{
		var location = crime.CrimeLocation ?? crime.Criminal.Location;
		if (location is null)
		{
			return;
		}

		foreach (var storyteller in crime.Criminal.Gameworld.AIStorytellers.OfType<AIStoryteller>())
		{
			if (!storyteller.SubscribeToCrimeEvents || storyteller.IsPaused || !storyteller.IsCellSurveilled(location))
			{
				continue;
			}

			storyteller.PassCrimeToAIStoryteller(location, crime);
		}
	}

	internal static void HandleCharacterStateInRoomEvent(ICharacter character, AIStorytellerStateTriggerType stateType)
	{
		if (character.Location is not ICell location)
		{
			return;
		}

		foreach (var storyteller in character.Gameworld.AIStorytellers.OfType<AIStoryteller>())
		{
			if (!storyteller.SubscribeToStateEvents || storyteller.IsPaused || !storyteller.IsCellSurveilled(location))
			{
				continue;
			}

			storyteller.PassCharacterStateToAIStoryteller(location, character, stateType);
		}
	}

	private bool IsCellSurveilled(ICell location)
	{
		return _subscribedCells.Contains(location);
	}

	private void PassSpeechEventToAIStoryteller(ICell location, ICharacter speaker, IPerceivable? target, string message,
		AudioVolume volume, ILanguage language, IAccent accent)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		AppendOpenSituationTitles(sb);
		sb.AppendLine("A character has spoken in a surveilled room.");
		sb.AppendLine($"Location: {location.GetFriendlyReference(null).StripANSIColour()}");
		sb.AppendLine($"Speaker: {speaker.PersonalName.GetName(NameStyle.FullName)}");
		sb.AppendLine(
			$"Speaker Description: {speaker.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
		sb.AppendLine($"Volume: {volume.Describe()}");
		sb.AppendLine($"Language: {language.Name}");
		sb.AppendLine($"Accent: {accent.Name}");
		if (target is not null)
		{
			sb.AppendLine($"Target: {target.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
		}

		sb.AppendLine("Spoken Text:");
		sb.AppendLine(message.StripANSIColour().StripMXP());
		ExecuteAttentionFilteredStorytellerPrompt(apiKey, "Character Speaks", sb.ToString(), includeEchoTools: true);
	}

	private void PassCrimeToAIStoryteller(ICell location, ICrime crime)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		AppendOpenSituationTitles(sb);
		sb.AppendLine("A character has committed a crime in a surveilled room.");
		sb.AppendLine($"Location: {location.GetFriendlyReference(null).StripANSIColour()}");
		sb.AppendLine($"Crime Id: {crime.Id:N0}");
		sb.AppendLine($"Crime Type: {crime.Law.CrimeType.DescribeEnum(true)}");
		sb.AppendLine($"Legal Authority: {crime.LegalAuthority.Name}");
		sb.AppendLine($"Criminal: {crime.Criminal.PersonalName.GetName(NameStyle.FullName)}");
		sb.AppendLine(
			$"Criminal Description: {crime.Criminal.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
		if (crime.Victim is not null)
		{
			sb.AppendLine($"Victim: {crime.Victim.PersonalName.GetName(NameStyle.FullName)}");
			sb.AppendLine(
				$"Victim Description: {crime.Victim.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
		}

		if (crime.ThirdPartyId is not null)
		{
			sb.AppendLine(
				$"Third Party: {crime.ThirdPartyFrameworkItemType.IfNullOrWhiteSpace("unknown")} #{crime.ThirdPartyId.Value:N0}");
		}

		sb.AppendLine($"Witness Count: {crime.WitnessIds.Count():N0}");
		sb.AppendLine($"Real-Time Stamp (UTC): {crime.RealTimeOfCrime:O}");
		ExecuteAttentionFilteredStorytellerPrompt(apiKey, "Character Crime", sb.ToString(), includeEchoTools: false);
	}

	private void PassCharacterStateToAIStoryteller(ICell location, ICharacter character, AIStorytellerStateTriggerType stateType)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var stateText = stateType switch
		{
			AIStorytellerStateTriggerType.Unconscious => "Unconscious",
			AIStorytellerStateTriggerType.PassOut => "Pass Out",
			AIStorytellerStateTriggerType.Dead => "Dead",
			_ => "Unknown"
		};
		var sb = new StringBuilder();
		AppendOpenSituationTitles(sb);
		sb.AppendLine("A character changed state in a surveilled room.");
		sb.AppendLine($"Location: {location.GetFriendlyReference(null).StripANSIColour()}");
		sb.AppendLine($"State Change: {stateText}");
		sb.AppendLine($"Character: {character.PersonalName.GetName(NameStyle.FullName)}");
		sb.AppendLine(
			$"Character Description: {character.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
		ExecuteStorytellerPrompt(apiKey, $"Character State {stateText}", sb.ToString(), includeEchoTools: false);
	}

	private static string AppendAttentionReasonToPrompt(string prompt, string attentionReason)
	{
		var sb = new StringBuilder();
		sb.AppendLine(prompt.TrimEnd());
		sb.AppendLine();
		sb.AppendLine("Attention Reason:");
		sb.AppendLine(attentionReason.IfNullOrWhiteSpace("No reason provided"));
		return sb.ToString();
	}

	private bool TryRunAttentionClassifier(string apiKey, string attentionPrompt, out string attentionReason)
	{
		attentionReason = string.Empty;
		var classifierPrompt =
			$"{AttentionAgentPrompt.IfNullOrWhiteSpace(string.Empty).Trim()}\n\n{AttentionContractInstruction}";
		var trimmedPrompt = TrimPromptText(attentionPrompt);
		DebugAIMessaging("Engine -> Attention Classifier Request",
			$"""
Model: {Model}
Reasoning: {ResponseReasoningEffortLevel.Low.Describe()}
Prompt:
{classifierPrompt}

Echo:
{trimmedPrompt}
""");

		ResponsesClient client = new(Model, apiKey);
		var options = new CreateResponseOptions([
			ResponseItem.CreateDeveloperMessageItem(classifierPrompt),
			ResponseItem.CreateUserMessageItem(trimmedPrompt)
		]);
		options.ReasoningOptions ??= new();
		options.ReasoningOptions.ReasoningEffortLevel = ResponseReasoningEffortLevel.Low;
		options.MaxOutputTokenCount = MaxAttentionClassifierOutputTokens;
		var attention = client.CreateResponseAsync(options).GetAwaiter().GetResult().Value;
		var attentionResponse = attention.GetOutputText();
		DebugAIMessaging("Attention Classifier -> Engine Response", attentionResponse);
		if (!TryInterpretAttentionClassifierOutput(attentionResponse, out var interested, out var reason))
		{
			DebugAIMessaging("Attention Classifier Decision",
				"Invalid response ignored due to contract violation.");
			return false;
		}

		if (!interested)
		{
			DebugAIMessaging("Attention Classifier Decision", "Ignored event.");
			return false;
		}

		attentionReason = reason.IfNullOrWhiteSpace("No reason provided");
		DebugAIMessaging("Attention Classifier Decision",
			$"Interested. Reason: {attentionReason}");
		return true;
	}

	private void QueueStorytellerWork(Func<Task> work)
	{
		_ = Task.Run(async () =>
		{
			await _storytellerWorkerSemaphore.WaitAsync().ConfigureAwait(false);
			try
			{
				await work().ConfigureAwait(false);
			}
			catch (Exception e)
			{
				LogStorytellerException(e);
			}
			finally
			{
				_storytellerWorkerSemaphore.Release();
			}
		});
	}

	private void ExecuteAttentionFilteredStorytellerPrompt(string apiKey, string trigger, string userPrompt,
		bool includeEchoTools, string? attentionPromptOverride = null)
	{
		QueueStorytellerWork(() =>
		{
			if (!TryRunAttentionClassifier(apiKey, attentionPromptOverride ?? userPrompt, out var attentionReason))
			{
				return Task.CompletedTask;
			}

			var finalPrompt = AppendAttentionReasonToPrompt(userPrompt, attentionReason);
			ExecuteStorytellerPromptImmediate(apiKey, trigger, finalPrompt, includeEchoTools);
			return Task.CompletedTask;
		});
	}

	private void ExecuteStorytellerPrompt(string apiKey, string trigger, string userPrompt, bool includeEchoTools)
	{
		QueueStorytellerWork(() =>
		{
			ExecuteStorytellerPromptImmediate(apiKey, trigger, userPrompt, includeEchoTools);
			return Task.CompletedTask;
		});
	}

	private void ExecuteStorytellerPromptImmediate(string apiKey, string trigger, string userPrompt,
		bool includeEchoTools)
	{
		var prompt = TrimPromptText(userPrompt);
		DebugAIMessaging("Engine -> Storyteller Request",
			$"""
Model: {Model}
Reasoning: {ReasoningEffort.Describe()}
Trigger: {trigger}
System Prompt:
{SystemPrompt}

User Prompt:
{prompt}
""");

		ResponsesClient client = new(Model, apiKey);
		List<ResponseItem> messages =
		[
			ResponseItem.CreateDeveloperMessageItem(SystemPrompt),
			ResponseItem.CreateUserMessageItem(prompt)
		];
		ExecuteToolCall(client, messages, includeEchoTools);
	}
}

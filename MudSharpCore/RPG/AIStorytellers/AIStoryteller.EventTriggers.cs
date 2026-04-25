using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Law;
using OpenAI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#nullable enable

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

        string apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return false;
        }

        StringBuilder sb = new();
        AppendOpenSituationTitles(sb);
        sb.AppendLine("A direct storyteller attention event was invoked via FutureProg.");
        sb.AppendLine("Event Text:");
        sb.AppendLine(attentionText.Trim());
        ExecuteStorytellerPrompt(apiKey, "Direct Invocation", sb.ToString(), includeEchoTools: false,
            systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
            toolProfile: StorytellerToolProfile.Full);
        return true;
    }

    internal static void HandleCharacterSpeechInRoomEvent(ICharacter speaker, IPerceivable? target, string message,
        AudioVolume volume, ILanguage language, IAccent accent)
    {
        if (speaker.Location is not ICell location)
        {
            return;
        }

        DateTime eventTimestampUtc = DateTime.UtcNow;
        RecordSpeechEventInContext(location, speaker, target, message, volume, language, accent, eventTimestampUtc);

        foreach (AIStoryteller storyteller in speaker.Gameworld.AIStorytellers.OfType<AIStoryteller>())
        {
            if (!storyteller.SubscribeToSpeechEvents || storyteller.IsPaused || !storyteller.IsCellSurveilled(location))
            {
                continue;
            }

            storyteller.PassSpeechEventToAIStoryteller(location, speaker, target, message, volume, language, accent,
                eventTimestampUtc);
        }
    }

    internal static void HandleCrimeCommittedInRoomEvent(ICrime crime)
    {
        ICell location = crime.CrimeLocation ?? crime.Criminal.Location;
        if (location is null)
        {
            return;
        }

        foreach (AIStoryteller storyteller in crime.Criminal.Gameworld.AIStorytellers.OfType<AIStoryteller>())
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

        foreach (AIStoryteller storyteller in character.Gameworld.AIStorytellers.OfType<AIStoryteller>())
        {
            if (!storyteller.SubscribeToStateEvents || storyteller.IsPaused || !storyteller.IsCellSurveilled(location))
            {
                continue;
            }

            storyteller.PassCharacterStateToAIStoryteller(location, character, stateType);
        }
    }

    private static void RecordSpeechEventInContext(ICell location, ICharacter speaker, IPerceivable? target,
        string message, AudioVolume volume, ILanguage language, IAccent accent, DateTime eventTimestampUtc)
    {
        IRecentSpeechContextEffect? contextEffect = location.EffectsOfType<IRecentSpeechContextEffect>().FirstOrDefault();
        if (contextEffect is null)
        {
            contextEffect = new RecentSpeechContextEffect(location);
            location.AddEffect(contextEffect);
        }

        contextEffect.RecordSpeechEvent(speaker, target, message, volume, language, accent, eventTimestampUtc);
    }

    private bool IsCellSurveilled(ICell location)
    {
        return _subscribedCells.Contains(location);
    }

    private void PassSpeechEventToAIStoryteller(ICell location, ICharacter speaker, IPerceivable? target, string message,
        AudioVolume volume, ILanguage language, IAccent accent, DateTime eventTimestampUtc)
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return;
        }

        StringBuilder sb = new();
        AppendOpenSituationTitles(sb, location, [speaker, target as ICharacter]);
        sb.AppendLine("A character has spoken in a surveilled room.");
        sb.AppendLine($"Location: {location.GetFriendlyReference(null).StripANSIColour()}");
        sb.AppendLine($"Speaker: {speaker.PersonalName.GetName(NameStyle.FullName)}");
        sb.AppendLine($"Language: {language.Name}");
        sb.AppendLine($"Accent: {accent.Name}");
        if (target is not null)
        {
            sb.AppendLine($"Target: {target.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
        }

        sb.AppendLine("Spoken Text:");
        sb.AppendLine(message.StripANSIColour().StripMXP());
        AppendRecentSpeechContext(sb, location, eventTimestampUtc);
        AppendAttentionBypassToolGuidance(sb);
        bool bypassAttention = TryGetAttentionBypassReason(location, [speaker, target as ICharacter],
            out string bypassReason);
        ExecuteAttentionFilteredStorytellerPrompt(apiKey, "Character Speaks", sb.ToString(), includeEchoTools: true,
            systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
            toolProfile: StorytellerToolProfile.EventFocused,
            bypassAttention: bypassAttention,
            bypassReason: bypassReason);
    }

    private void PassCrimeToAIStoryteller(ICell location, ICrime crime)
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return;
        }

        StringBuilder sb = new();
        AppendOpenSituationTitles(sb, location, [crime.Criminal, crime.Victim]);
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
        AppendAttentionBypassToolGuidance(sb);
        bool bypassAttention = TryGetAttentionBypassReason(location, [crime.Criminal, crime.Victim],
            out string bypassReason);
        ExecuteAttentionFilteredStorytellerPrompt(apiKey, "Character Crime", sb.ToString(), includeEchoTools: false,
            systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
            toolProfile: StorytellerToolProfile.EventFocused,
            bypassAttention: bypassAttention,
            bypassReason: bypassReason);
    }

    private void PassCharacterStateToAIStoryteller(ICell location, ICharacter character, AIStorytellerStateTriggerType stateType)
    {
        string apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
        if (string.IsNullOrEmpty(apiKey))
        {
            return;
        }

        string stateText = stateType switch
        {
            AIStorytellerStateTriggerType.Unconscious => "Unconscious",
            AIStorytellerStateTriggerType.PassOut => "Pass Out",
            AIStorytellerStateTriggerType.Dead => "Dead",
            _ => "Unknown"
        };
        StringBuilder sb = new();
        AppendOpenSituationTitles(sb, location, [character]);
        sb.AppendLine("A character changed state in a surveilled room.");
        sb.AppendLine($"Location: {location.GetFriendlyReference(null).StripANSIColour()}");
        sb.AppendLine($"State Change: {stateText}");
        sb.AppendLine($"Character: {character.PersonalName.GetName(NameStyle.FullName)}");
        sb.AppendLine(
            $"Character Description: {character.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
        ExecuteStorytellerPrompt(apiKey, $"Character State {stateText}", sb.ToString(), includeEchoTools: false,
            systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
            toolProfile: StorytellerToolProfile.Full);
    }

    private void AppendRecentSpeechContext(StringBuilder sb, ICell location, DateTime eventTimestampUtc)
    {
        IReadOnlyCollection<RecentSpeechContextEvent> priorEvents = GetPriorSpeechContextEvents(location, eventTimestampUtc);
        if (priorEvents.Count == 0)
        {
            return;
        }

        sb.AppendLine();
        sb.AppendLine("Recent Speech Context (oldest to newest):");
        foreach (RecentSpeechContextEvent item in priorEvents)
        {
            string targetDescription = item.TargetDescription.IfNullOrWhiteSpace("No direct target");
            string targetSuffix = item.TargetId.HasValue
                ? $" ({item.TargetFrameworkItemType.IfNullOrWhiteSpace("unknown")} #{item.TargetId.Value:N0})"
                : string.Empty;
            sb.AppendLine(
                $"- [{item.RealTimeTimestampUtc:O}] {item.SpeakerName} -> {targetDescription}{targetSuffix}: \"{item.Message}\" ({item.Volume.DescribeEnum(true)}, {item.LanguageName}, {item.AccentName})");
        }
    }

    private IReadOnlyCollection<RecentSpeechContextEvent> GetPriorSpeechContextEvents(ICell location,
        DateTime eventTimestampUtc)
    {
        if (SpeechContextEventCount <= 0)
        {
            return [];
        }

        IRecentSpeechContextEffect? contextEffect = location.EffectsOfType<IRecentSpeechContextEffect>().FirstOrDefault();
        if (contextEffect is null)
        {
            return [];
        }

        List<RecentSpeechContextEvent> events = contextEffect.GetRecentSpeechEvents(eventTimestampUtc, SpeechContextEventCount + 1,
            SpeechContextMaximumSeparation).ToList();
        if (events.Count <= 1)
        {
            return [];
        }

        events.RemoveAt(events.Count - 1);
        return events;
    }

    private static string AppendAttentionReasonToPrompt(string prompt, string attentionReason)
    {
        StringBuilder sb = new();
        sb.AppendLine(prompt.TrimEnd());
        sb.AppendLine();
        sb.AppendLine("Attention Reason:");
        sb.AppendLine(attentionReason.IfNullOrWhiteSpace("No reason provided"));
        return sb.ToString();
    }

    private static void AppendAttentionBypassToolGuidance(StringBuilder sb)
    {
        sb.AppendLine();
        sb.AppendLine("Attention Focus Guidance:");
        sb.AppendLine("If this event should stay in focus, call BypassAttention with either CharacterId or RoomId.");
        sb.AppendLine("When that focus is no longer needed, call EndBypassAttention for the same target.");
    }

    private bool TryGetAttentionBypassReason(ICell location, IEnumerable<ICharacter?> involvedCharacters,
        out string reason)
    {
        lock (_attentionBypassLock)
        {
            if (_bypassAttentionRoomIds.Contains(location.Id))
            {
                reason = $"Bypass attention is active for room #{location.Id:N0}.";
                return true;
            }

            foreach (ICharacter? character in involvedCharacters.Where(x => x is not null))
            {
                if (!_bypassAttentionCharacterIds.Contains(character!.Id))
                {
                    continue;
                }

                reason =
                    $"Bypass attention is active for character #{character.Id:N0} ({character.PersonalName.GetName(NameStyle.FullName)}).";
                return true;
            }
        }

        reason = string.Empty;
        return false;
    }

    private bool TryRunAttentionClassifier(string apiKey, string attentionPrompt, out string attentionReason)
    {
        attentionReason = string.Empty;
        string classifierPrompt =
            $"{AttentionAgentPrompt.IfNullOrWhiteSpace(string.Empty).Trim()}\n\n{AttentionContractInstruction}";
        string trimmedPrompt = TrimPromptText(attentionPrompt);
        DebugAIMessaging("Engine -> Attention Classifier Request",
            $"""
Model: {AttentionClassifierModel}
Reasoning: {AttentionClassifierReasoningEffort.Describe()}
Prompt:
{classifierPrompt}

Echo:
{trimmedPrompt}
""");

        ResponsesClient client = new(AttentionClassifierModel, apiKey);
        CreateResponseOptions options = new([
            ResponseItem.CreateUserMessageItem(trimmedPrompt)
        ]);
        options.Instructions = classifierPrompt;
        options.StoredOutputEnabled = true;
        options.TruncationMode = ResponseTruncationMode.Auto;
        options.ReasoningOptions ??= new();
        options.ReasoningOptions.ReasoningEffortLevel = AttentionClassifierReasoningEffort;
        options.MaxOutputTokenCount = MaxAttentionClassifierOutputTokens;
        ResponseResult attention = client.CreateResponseAsync(options).GetAwaiter().GetResult().Value;
        DebugResponseUsage("Attention Classifier -> Engine Usage", attention);
        string attentionResponse = attention.GetOutputText();
        DebugAIMessaging("Attention Classifier -> Engine Response", attentionResponse);
        if (!TryInterpretAttentionClassifierOutput(attentionResponse, out bool interested, out string reason))
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
        bool includeEchoTools, string systemPrompt, string model, ResponseReasoningEffortLevel reasoningEffort,
        StorytellerToolProfile toolProfile = StorytellerToolProfile.Full,
        string? attentionPromptOverride = null, bool bypassAttention = false, string? bypassReason = null)
    {
        QueueStorytellerWork(() =>
        {
            if (bypassAttention)
            {
                string bypassPrompt = AppendAttentionReasonToPrompt(userPrompt,
                    bypassReason.IfNullOrWhiteSpace("Bypass attention is active for this event."));
                ExecuteStorytellerPromptImmediate(apiKey, trigger, bypassPrompt, includeEchoTools, toolProfile,
                    systemPrompt, model, reasoningEffort);
                return Task.CompletedTask;
            }

            if (!TryRunAttentionClassifier(apiKey, attentionPromptOverride ?? userPrompt, out string attentionReason))
            {
                return Task.CompletedTask;
            }

            string finalPrompt = AppendAttentionReasonToPrompt(userPrompt, attentionReason);
            ExecuteStorytellerPromptImmediate(apiKey, trigger, finalPrompt, includeEchoTools, toolProfile,
                systemPrompt, model, reasoningEffort);
            return Task.CompletedTask;
        });
    }

    private void ExecuteStorytellerPrompt(string apiKey, string trigger, string userPrompt, bool includeEchoTools,
        string systemPrompt, string model, ResponseReasoningEffortLevel reasoningEffort,
        StorytellerToolProfile toolProfile = StorytellerToolProfile.Full)
    {
        QueueStorytellerWork(() =>
        {
            ExecuteStorytellerPromptImmediate(apiKey, trigger, userPrompt, includeEchoTools, toolProfile, systemPrompt,
                model, reasoningEffort);
            return Task.CompletedTask;
        });
    }

    private void ExecuteStorytellerPromptImmediate(string apiKey, string trigger, string userPrompt,
        bool includeEchoTools, StorytellerToolProfile toolProfile, string systemPrompt, string model,
        ResponseReasoningEffortLevel reasoningEffort)
    {
        string prompt = TrimPromptText(userPrompt);
        DebugAIMessaging("Engine -> Storyteller Request",
            $"""
Model: {model}
Reasoning: {reasoningEffort.Describe()}
Trigger: {trigger}
System Prompt:
{systemPrompt}

User Prompt:
{prompt}
""");

        ResponsesClient client = new(model, apiKey);
        List<ResponseItem> messages =
        [
            ResponseItem.CreateUserMessageItem(prompt)
        ];
        ExecuteToolCall(client, messages, includeEchoTools, toolProfile);
    }
}

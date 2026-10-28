using MudSharp.Character.Name;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Audio;
using MudSharp.RPG.Law;
using OpenAI.Responses;
using System.Threading;

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
        return ExecuteStorytellerPrompt(apiKey, "Direct Invocation", sb.ToString(), includeEchoTools: false,
            systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
            toolProfile: StorytellerToolProfile.Full);
    }

    internal static void HandleCharacterSpeechInRoomEvent(ICharacter speaker, IPerceivable? target, string message,
        AudioVolume volume, ILanguage language, IAccent accent)
    {
        if (speaker.Location is not IRoom location)
        {
            return;
        }

        DateTime eventTimestampUtc = DateTime.UtcNow;
        RecordSpeechEventInContext(location, speaker, target, message, volume, language, accent, eventTimestampUtc);

        foreach (AIStoryteller storyteller in speaker.Gameworld.AIStorytellers.OfType<AIStoryteller>())
        {
            if (!storyteller.SubscribeToSpeechEvents || storyteller.IsPaused || !storyteller.IsRoomSurveilled(location))
            {
                continue;
            }

            storyteller.PassSpeechEventToAIStoryteller(location, speaker, target, message, volume, language, accent,
                eventTimestampUtc);
        }
    }

	internal static void HandleCharacterSignInRoomEvent(ICharacter signer, IPerceivable? target, string message,
		ISignedLanguage language, ISignedLanguageVariety? variety)
	{
		if (signer.Location is not IRoom location)
		{
			return;
		}

		foreach (var storyteller in signer.Gameworld.AIStorytellers.OfType<AIStoryteller>())
		{
			if (!storyteller.SubscribeToSpeechEvents || storyteller.IsPaused || !storyteller.IsRoomSurveilled(location))
			{
				continue;
			}

			storyteller.PassSignEventToAIStoryteller(location, signer, target, message, language, variety);
		}
	}

    internal static void HandleCrimeCommittedInRoomEvent(ICrime crime)
    {
        IRoom location = crime.CrimeLocation ?? crime.Criminal.Location;
        if (location is null)
        {
            return;
        }

        foreach (AIStoryteller storyteller in crime.Criminal.Gameworld.AIStorytellers.OfType<AIStoryteller>())
        {
            if (!storyteller.SubscribeToCrimeEvents || storyteller.IsPaused || !storyteller.IsRoomSurveilled(location))
            {
                continue;
            }

            storyteller.PassCrimeToAIStoryteller(location, crime);
        }
    }

    internal static void HandleCharacterStateInRoomEvent(ICharacter character, AIStorytellerStateTriggerType stateType)
    {
        if (character.Location is not IRoom location)
        {
            return;
        }

        foreach (AIStoryteller storyteller in character.Gameworld.AIStorytellers.OfType<AIStoryteller>())
        {
            if (!storyteller.SubscribeToStateEvents || storyteller.IsPaused || !storyteller.IsRoomSurveilled(location))
            {
                continue;
            }

            storyteller.PassCharacterStateToAIStoryteller(location, character, stateType);
        }
    }

    private static void RecordSpeechEventInContext(IRoom location, ICharacter speaker, IPerceivable? target,
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

    private bool IsRoomSurveilled(IRoom location)
    {
        return _subscribedRooms.Contains(location);
    }

    private void PassSpeechEventToAIStoryteller(IRoom location, ICharacter speaker, IPerceivable? target, string message,
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
        bool bypassAttention = TryGetAttentionBypassReason(location, [speaker, target as ICharacter],
            out string bypassReason);
        ExecuteAttentionFilteredStorytellerPrompt(apiKey, "Character Speaks", sb.ToString(), includeEchoTools: true,
            systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
            toolProfile: StorytellerToolProfile.EventFocused,
            bypassAttention: bypassAttention,
            bypassReason: bypassReason, roomId: location.Id, sharedContext: BatchRoomContext(location));
    }

	private void PassSignEventToAIStoryteller(IRoom location, ICharacter signer, IPerceivable? target, string message,
		ISignedLanguage language, ISignedLanguageVariety? variety)
	{
		var apiKey = Futuremud.Games.First().GetStaticConfiguration("GPT_Secret_Key");
		if (string.IsNullOrEmpty(apiKey))
		{
			return;
		}

		var sb = new StringBuilder();
		AppendOpenSituationTitles(sb, location, [signer, target as ICharacter]);
		sb.AppendLine("A character has signed in a surveilled room.");

		sb.AppendLine($"Signer: {signer.PersonalName.GetName(NameStyle.FullName)}");
		sb.AppendLine($"Signed Language: {language.Name}");
		if (variety is not null)
		{
			sb.AppendLine($"Variety: {variety.Name}");
		}
		if (target is not null)
		{
			sb.AppendLine($"Target: {target.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)}");
		}
		sb.AppendLine("Signed Text:");
		sb.AppendLine(message.StripANSIColour().StripMXP());
		var bypassAttention = TryGetAttentionBypassReason(location, [signer, target as ICharacter],
			out var bypassReason);
		ExecuteAttentionFilteredStorytellerPrompt(apiKey, "Character Signs", sb.ToString(), includeEchoTools: true,
			systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
			toolProfile: StorytellerToolProfile.EventFocused,
			bypassAttention: bypassAttention, bypassReason: bypassReason, roomId: location.Id, sharedContext: BatchRoomContext(location));
	}

    private void PassCrimeToAIStoryteller(IRoom location, ICrime crime)
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
        bool bypassAttention = TryGetAttentionBypassReason(location, [crime.Criminal, crime.Victim],
            out string bypassReason);
        ExecuteAttentionFilteredStorytellerPrompt(apiKey, "Character Crime", sb.ToString(), includeEchoTools: false,
            systemPrompt: SystemPrompt, model: Model, reasoningEffort: ReasoningEffort,
            toolProfile: StorytellerToolProfile.EventFocused,
            bypassAttention: bypassAttention,
            bypassReason: bypassReason);
    }

    private void PassCharacterStateToAIStoryteller(IRoom location, ICharacter character, AIStorytellerStateTriggerType stateType)
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

    private void AppendRecentSpeechContext(StringBuilder sb, IRoom location, DateTime eventTimestampUtc)
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

    private IReadOnlyCollection<RecentSpeechContextEvent> GetPriorSpeechContextEvents(IRoom location,
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

    private bool TryGetAttentionBypassReason(IRoom location, IEnumerable<ICharacter?> involvedCharacters,
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
                var characterIdentityId = CharacterInstanceIdentityComparer.IdentityId(character);
                if (!_bypassAttentionCharacterIds.Contains(characterIdentityId))
                {
                    continue;
                }

                reason =
                    $"Bypass attention is active for character #{characterIdentityId:N0} ({character!.PersonalName.GetName(NameStyle.FullName)}).";
                return true;
            }
        }

        reason = string.Empty;
        return false;
    }

    private void ExecuteAttentionFilteredStorytellerPrompt(string apiKey, string trigger, string userPrompt,
        bool includeEchoTools, string systemPrompt, string model, ResponseReasoningEffortLevel reasoningEffort,
        StorytellerToolProfile toolProfile = StorytellerToolProfile.Full,
        string? attentionPromptOverride = null, bool bypassAttention = false, string? bypassReason = null,
        long? roomId = null, string sharedContext = "")
    {
        EnqueuePrompt(apiKey, trigger, userPrompt, includeEchoTools, systemPrompt, model, reasoningEffort,
            toolProfile, !bypassAttention, attentionPromptOverride, roomId, sharedContext);
    }

    private bool ExecuteStorytellerPrompt(string apiKey, string trigger, string userPrompt, bool includeEchoTools,
        string systemPrompt, string model, ResponseReasoningEffortLevel reasoningEffort,
        StorytellerToolProfile toolProfile = StorytellerToolProfile.Full, bool timeRole = false)
    {
        return EnqueuePrompt(apiKey, trigger, userPrompt, includeEchoTools, systemPrompt, model, reasoningEffort,
            toolProfile, timeRole: timeRole);
    }

    private static string BatchRoomContext(IRoom room) => $"Room #{room.Id}: {room.HowSeen(null, colour: false)}\n";
}

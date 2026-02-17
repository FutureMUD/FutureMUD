#nullable enable
using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Form.Audio;
using MudSharp.Framework;

namespace MudSharp.Effects.Interfaces;

public sealed record RecentSpeechContextEvent(
	DateTime RealTimeTimestampUtc,
	long SpeakerId,
	string SpeakerName,
	string Message,
	AudioVolume Volume,
	string LanguageName,
	string AccentName,
	long? TargetId,
	string? TargetFrameworkItemType,
	string? TargetDescription);

public interface IRecentSpeechContextEffect : IEffect
{
	void RecordSpeechEvent(ICharacter speaker, IPerceivable? target, string message, AudioVolume volume,
		ILanguage language, IAccent accent, DateTime realTimeTimestampUtc);
	IReadOnlyCollection<RecentSpeechContextEvent> GetRecentSpeechEvents(DateTime latestTimestampUtc, int maximumEvents,
		TimeSpan maximumSeparation);
}

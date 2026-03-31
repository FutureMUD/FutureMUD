#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Communication.Language;
using MudSharp.Form.Shape;
using MudSharp.RPG.Checks;

namespace MudSharp.Form.Audio;

public interface IAudioRecording
{
	IReadOnlyList<RecordedAudioSegment> Segments { get; }
	TimeSpan TotalDuration { get; }
	bool IsEmpty { get; }
}

public sealed record RecordedAudioSpeakerSnapshot(long? CharacterId, string Name, Gender Gender)
{
	public XElement SaveToXml()
	{
		return new XElement("Speaker",
			new XAttribute("character", CharacterId ?? 0L),
			new XAttribute("name", Name),
			new XAttribute("gender", (short)Gender));
	}

	public static RecordedAudioSpeakerSnapshot LoadFromXml(XElement element)
	{
		return new RecordedAudioSpeakerSnapshot(
			long.TryParse(element.Attribute("character")?.Value, out var characterId) && characterId > 0
				? characterId
				: null,
			element.Attribute("name")?.Value ?? "Unknown Speaker",
			short.TryParse(element.Attribute("gender")?.Value, out var genderValue)
				? (Gender)genderValue
				: Gender.Neuter);
	}
}

public sealed record RecordedAudioSegment(
	TimeSpan DelayBeforeSegment,
	TimeSpan EstimatedSegmentDuration,
	long LanguageId,
	long AccentId,
	string RawText,
	AudioVolume Volume,
	Outcome Outcome,
	RecordedAudioSpeakerSnapshot Speaker)
{
	public XElement SaveToXml()
	{
		return new XElement("Segment",
			new XAttribute("delayms", (long)DelayBeforeSegment.TotalMilliseconds),
			new XAttribute("durationms", (long)EstimatedSegmentDuration.TotalMilliseconds),
			new XAttribute("language", LanguageId),
			new XAttribute("accent", AccentId),
			new XAttribute("volume", (int)Volume),
			new XAttribute("outcome", (int)Outcome),
			new XElement("Text", new XCData(RawText)),
			Speaker.SaveToXml());
	}

	public static RecordedAudioSegment LoadFromXml(XElement element)
	{
		return new RecordedAudioSegment(
			TimeSpan.FromMilliseconds(long.TryParse(element.Attribute("delayms")?.Value, out var delayMs) ? delayMs : 0L),
			TimeSpan.FromMilliseconds(long.TryParse(element.Attribute("durationms")?.Value, out var durationMs) ? durationMs : 0L),
			long.TryParse(element.Attribute("language")?.Value, out var languageId) ? languageId : 0L,
			long.TryParse(element.Attribute("accent")?.Value, out var accentId) ? accentId : 0L,
			element.Element("Text")?.Value ?? string.Empty,
			int.TryParse(element.Attribute("volume")?.Value, out var volume) && Enum.IsDefined(typeof(AudioVolume), volume)
				? (AudioVolume)volume
				: AudioVolume.Decent,
			int.TryParse(element.Attribute("outcome")?.Value, out var outcome) && Enum.IsDefined(typeof(Outcome), outcome)
				? (Outcome)outcome
				: Outcome.Pass,
			RecordedAudioSpeakerSnapshot.LoadFromXml(
				element.Element("Speaker") ?? new XElement("Speaker", new XAttribute("name", "Unknown Speaker"))));
	}

	public static RecordedAudioSegment FromSpokenLanguage(
		SpokenLanguageInfo spokenLanguage,
		TimeSpan delayBeforeSegment,
		TimeSpan? estimatedSegmentDuration = null)
	{
		var speaker = spokenLanguage.Origin as MudSharp.Character.ICharacter;
		return new RecordedAudioSegment(
			delayBeforeSegment,
			estimatedSegmentDuration ?? EstimateDuration(spokenLanguage.RawText),
			spokenLanguage.Language.Id,
			spokenLanguage.Accent.Id,
			spokenLanguage.RawText,
			spokenLanguage.Volume,
			spokenLanguage.OriginOutcome,
			new RecordedAudioSpeakerSnapshot(
				speaker?.Id,
				speaker?.HowSeen(speaker, proper: true, colour: false) ?? spokenLanguage.Origin.Name,
				speaker?.ApparentGender(speaker).Enum ?? Gender.Neuter));
	}

	public static TimeSpan EstimateDuration(string rawText)
	{
		if (string.IsNullOrWhiteSpace(rawText))
		{
			return TimeSpan.FromSeconds(1);
		}

		var wordCount = rawText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length;
		return TimeSpan.FromMilliseconds(Math.Max(1000, wordCount * 450));
	}
}

public sealed class RecordedAudio : IAudioRecording
{
	private readonly IReadOnlyList<RecordedAudioSegment> _segments;

	public RecordedAudio(IEnumerable<RecordedAudioSegment> segments)
	{
		_segments = segments.ToList().AsReadOnly();
	}

	public IReadOnlyList<RecordedAudioSegment> Segments => _segments;
	public TimeSpan TotalDuration => TimeSpan.FromMilliseconds(_segments.Sum(x => (x.DelayBeforeSegment + x.EstimatedSegmentDuration).TotalMilliseconds));
	public bool IsEmpty => !_segments.Any();

	public XElement SaveToXml(string? elementName = null)
	{
		return new XElement(elementName ?? "Recording",
			new XAttribute("durationms", (long)TotalDuration.TotalMilliseconds),
			from segment in Segments
			select segment.SaveToXml());
	}

	public static RecordedAudio LoadFromXml(XElement? element)
	{
		return new RecordedAudio(element?.Elements("Segment").Select(RecordedAudioSegment.LoadFromXml) ??
		                         Enumerable.Empty<RecordedAudioSegment>());
	}
}

public sealed record StoredAudioRecording(string Name, RecordedAudio Recording, DateTime RecordedAtUtc)
{
	public XElement SaveToXml()
	{
		return new XElement("StoredRecording",
			new XAttribute("name", Name),
			new XAttribute("recordedatutc", RecordedAtUtc.ToString("O")),
			Recording.SaveToXml());
	}

	public static StoredAudioRecording LoadFromXml(XElement element)
	{
		return new StoredAudioRecording(
			element.Attribute("name")?.Value ?? string.Empty,
			RecordedAudio.LoadFromXml(element.Element("Recording")),
			DateTime.TryParse(element.Attribute("recordedatutc")?.Value, CultureInfo.InvariantCulture,
				DateTimeStyles.RoundtripKind,
				out var recordedAt)
				? recordedAt
				: DateTime.UtcNow);
	}
}

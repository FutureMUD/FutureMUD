#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Communication.Language;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.Effects.Concrete;

public class RecentSpeechContextEffect : Effect, IRecentSpeechContextEffect
{
	private const int MaximumStoredEvents = 200;
	private const int MaximumStoredTextLength = 400;
	private static readonly TimeSpan MaximumStoredAge = TimeSpan.FromHours(24);
	private readonly List<RecentSpeechContextEvent> _events = [];
	private readonly object _eventsLock = new();

	public RecentSpeechContextEffect(IPerceivable owner, IFutureProg? applicabilityProg = null)
		: base(owner, applicabilityProg)
	{
	}

	public RecentSpeechContextEffect(XElement effect, IPerceivable owner)
		: base(effect, owner)
	{
		var root = effect.Element("Effect")?.Element("Events");
		if (root is null)
		{
			return;
		}

		foreach (var element in root.Elements("Event"))
		{
			if (!TryParseEvent(element, out var parsed))
			{
				continue;
			}

			_events.Add(parsed);
		}
	}

	public static void InitialiseEffectType()
	{
		RegisterFactory("RecentSpeechContextEffect", (effect, owner) => new RecentSpeechContextEffect(effect, owner));
	}

	public override bool SavingEffect => true;

	public void RecordSpeechEvent(ICharacter speaker, IPerceivable? target, string message, AudioVolume volume,
		ILanguage language, IAccent accent, DateTime realTimeTimestampUtc)
	{
		var timestampUtc = EnsureUtc(realTimeTimestampUtc);
		var speakerName = speaker.PersonalName.GetName(NameStyle.FullName)
			.IfNullOrWhiteSpace(speaker.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription));
		var targetDescription = target switch
		{
			null => null,
			ICharacter character => character.PersonalName.GetName(NameStyle.FullName),
			_ => target.HowSeen(null, colour: false, flags: PerceiveIgnoreFlags.TrueDescription)
		};
		var entry = new RecentSpeechContextEvent(
			timestampUtc,
			speaker.Id,
			SanitizeText(speakerName),
			SanitizeText(message.StripANSIColour().StripMXP()),
			volume,
			SanitizeText(language.Name),
			SanitizeText(accent.Name),
			target?.Id,
			target?.FrameworkItemType,
			SanitizeText(targetDescription, permitNull: true)
		);
		lock (_eventsLock)
		{
			PruneStaleEvents(timestampUtc);
			_events.Add(entry);
			if (_events.Count > MaximumStoredEvents)
			{
				_events.RemoveRange(0, _events.Count - MaximumStoredEvents);
			}

			Changed = true;
		}
	}

	public IReadOnlyCollection<RecentSpeechContextEvent> GetRecentSpeechEvents(DateTime latestTimestampUtc,
		int maximumEvents, TimeSpan maximumSeparation)
	{
		if (maximumEvents <= 0)
		{
			return [];
		}

		if (maximumSeparation < TimeSpan.Zero)
		{
			maximumSeparation = TimeSpan.Zero;
		}

		var latestUtc = EnsureUtc(latestTimestampUtc);
		var earliestUtc = latestUtc - maximumSeparation;
		lock (_eventsLock)
		{
			PruneStaleEvents(latestUtc);
			var results = _events
				.Where(x => x.RealTimeTimestampUtc >= earliestUtc)
				.Where(x => x.RealTimeTimestampUtc <= latestUtc)
				.ToList();
			if (results.Count > maximumEvents)
			{
				results = results.Skip(results.Count - maximumEvents).ToList();
			}

			return results;
		}
	}

	public override string Describe(IPerceiver voyeur)
	{
		lock (_eventsLock)
		{
			return $"Tracking {_events.Count:N0} recent speech event{(_events.Count == 1 ? string.Empty : "s")}.";
		}
	}

	protected override XElement SaveDefinition()
	{
		lock (_eventsLock)
		{
			return new XElement("Effect",
				new XElement("Events",
					from entry in _events
					select SaveEvent(entry)
				)
			);
		}
	}

	protected override string SpecificEffectType => "RecentSpeechContextEffect";

	private static XElement SaveEvent(RecentSpeechContextEvent entry)
	{
		return new XElement("Event",
			new XElement("TimestampUtc", entry.RealTimeTimestampUtc.ToString("O")),
			new XElement("SpeakerId", entry.SpeakerId),
			new XElement("SpeakerName", new XCData(entry.SpeakerName)),
			new XElement("TargetId", entry.TargetId?.ToString() ?? string.Empty),
			new XElement("TargetFrameworkItemType", new XCData(entry.TargetFrameworkItemType ?? string.Empty)),
			new XElement("TargetDescription", new XCData(entry.TargetDescription ?? string.Empty)),
			new XElement("Message", new XCData(entry.Message)),
			new XElement("Volume", (int)entry.Volume),
			new XElement("LanguageName", new XCData(entry.LanguageName)),
			new XElement("AccentName", new XCData(entry.AccentName))
		);
	}

	private static bool TryParseEvent(XElement element, out RecentSpeechContextEvent parsed)
	{
		parsed = default!;
		if (!DateTime.TryParse(element.Element("TimestampUtc")?.Value, null, DateTimeStyles.RoundtripKind,
			    out var timestamp))
		{
			return false;
		}

		if (!long.TryParse(element.Element("SpeakerId")?.Value, out var speakerId))
		{
			return false;
		}

		var volume = AudioVolume.Decent;
		if (int.TryParse(element.Element("Volume")?.Value, out var volumeValue) &&
		    Enum.IsDefined(typeof(AudioVolume), volumeValue))
		{
			volume = (AudioVolume)volumeValue;
		}

		long? targetId = null;
		if (long.TryParse(element.Element("TargetId")?.Value, out var parsedTargetId))
		{
			targetId = parsedTargetId;
		}

		parsed = new RecentSpeechContextEvent(
			EnsureUtc(timestamp),
			speakerId,
			SanitizeText(element.Element("SpeakerName")?.Value),
			SanitizeText(element.Element("Message")?.Value),
			volume,
			SanitizeText(element.Element("LanguageName")?.Value),
			SanitizeText(element.Element("AccentName")?.Value),
			targetId,
			SanitizeText(element.Element("TargetFrameworkItemType")?.Value, permitNull: true),
			SanitizeText(element.Element("TargetDescription")?.Value, permitNull: true)
		);
		return true;
	}

	private void PruneStaleEvents(DateTime referenceUtc)
	{
		var earliest = referenceUtc - MaximumStoredAge;
		var removed = _events.RemoveAll(x => x.RealTimeTimestampUtc < earliest);
		if (removed > 0)
		{
			Changed = true;
		}
	}

	private static DateTime EnsureUtc(DateTime dateTime)
	{
		return dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
	}

	private static string? SanitizeText(string? text, bool permitNull = false)
	{
		if (permitNull && string.IsNullOrWhiteSpace(text))
		{
			return null;
		}

		var cleaned = text
			.IfNullOrWhiteSpace(string.Empty)
			.StripANSIColour()
			.StripMXP()
			.Trim();
		if (string.IsNullOrWhiteSpace(cleaned))
		{
			return permitNull ? null : "(none)";
		}

		if (cleaned.Length > MaximumStoredTextLength)
		{
			return $"{cleaned[..MaximumStoredTextLength]}...";
		}

		return cleaned;
	}
}

using System;

#nullable enable
namespace MudSharp.Communication.Language;

public enum SpeechOriginKind { PlayerInput, GeneratedCasting, Relay }

/// <summary>Correlation metadata for presentation and relays; it never grants executable authority.</summary>
public sealed record SpeechProvenance(Guid Id, long ActingInstanceId, SpeechOriginKind Kind)
{
	public SpeechProvenance AsRelay() => this with { Kind = SpeechOriginKind.Relay };
}

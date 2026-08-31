#nullable enable

using MudSharp.Communication.Language;
using MudSharp.Computers;
using MudSharp.Framework;

namespace MudSharp.PerceptionEngine;

/// <summary>
/// Exposes the structured language payload required by recording devices without requiring them to know a concrete
/// output implementation or scrape already-rendered player text.
/// </summary>
public interface IRecordableLanguageOutput : IOutput
{
	IPerceivable DefaultSource { get; }
	LanguageInfo LanguageInfo { get; }
	IEmote PreLanguageEmote { get; }
	IEmote? OptionalEmote { get; }
}

/// <summary>
/// Marks player-visible output generated from a media packet. A camera that sees this output preserves the packet's
/// stream provenance instead of inventing a new unbounded feedback stream.
/// </summary>
public interface IMediaPacketOutput : IOutput
{
	MediaPacket MediaPacket { get; }
	IPerceivable PresentationSource { get; }
}

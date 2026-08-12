#nullable enable

using System;
using System.Collections.Generic;

namespace MudSharp.Traps;

/// <summary>
/// A serialisable trigger module belonging to a trap template.
/// </summary>
public interface ITrapTrigger
{
	TrapTriggerType TriggerType { get; }
	IReadOnlySet<TrapSourceKind> CompatibleSourceKinds { get; }
	IReadOnlyDictionary<string, string> Parameters { get; }
	string SaveToXml();
}

/// <summary>
/// A serialisable payload module belonging to a trap template.
/// </summary>
public interface ITrapPayload
{
	TrapPayloadType PayloadType { get; }
	IReadOnlySet<TrapSourceKind> CompatibleSourceKinds { get; }
	TimeSpan Delay { get; }
	TrapTargetSelector TargetSelector { get; }
	IReadOnlyDictionary<string, string> Parameters { get; }
	string SaveToXml();
}

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Form.Audio;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Builder-authored vocabulary. POWER is historical; the other four categories retain their stated provenance.</summary>
public sealed record ControlledSpellIncantation(long LanguageId, string Reach, string Element, string Sphere,
	string Mood, IReadOnlyList<string> Aliases, IReadOnlyList<ControlledSpellDelivery> Deliveries,
	string VocabularyProvenance = "FutureMUD aliases")
{
	public IReadOnlyList<string> CategoryWords => [Reach, Element, Sphere, Mood];
}

/// <summary>One explicitly enabled native communication method and its tuning; no implicit mode combinations.</summary>
public sealed record ControlledSpellDelivery(string Method, AudioVolume Volume, double EnergyMultiplier,
	int DifficultySteps);

public sealed record MagicCastingSpeech(Guid OriginId, string Message, string Method, AudioVolume Volume,
	long LanguageId, string? FormulaText = null);

public sealed record MagicCastingDelivery(string Method, AudioVolume Volume, long LanguageId,
	double EnergyMultiplier, int DifficultySteps, string Incantation, bool UsesOriginalSpeech);

public static class ArmageddonPowerWords
{
	private static readonly IReadOnlyList<string> Words = Array.AsReadOnly(new[] { "wek", "yuqa", "kral", "een", "pav", "sul", "mon" });
	public static IReadOnlyList<string> All => Words;
	public static string ForGrade(int grade) => grade is >= 1 and <= 7 ? Words[grade - 1] : throw new ArgumentOutOfRangeException(nameof(grade));
	public static int? Grade(string word)
	{
		for (var i = 0; i < Words.Count; i++) if (Words[i].Equals(word, StringComparison.OrdinalIgnoreCase)) return i + 1;
		return null;
	}
}

/// <summary>Names refer to existing IBody communication methods; volumes are those methods' native volumes.</summary>
public static class MagicCastingMethods
{
	private static readonly IReadOnlyDictionary<string, AudioVolume> Methods = new Dictionary<string, AudioVolume>(StringComparer.OrdinalIgnoreCase)
	{
		["Say"] = AudioVolume.Decent, ["Whisper"] = AudioVolume.Quiet, ["Talk"] = AudioVolume.Quiet,
		["LoudSay"] = AudioVolume.Loud, ["Yell"] = AudioVolume.VeryLoud,
		["Shout"] = AudioVolume.ExtremelyLoud, ["Sing"] = AudioVolume.Loud
	};
	public static string? CanonicalName(string method) => Methods.Keys.FirstOrDefault(x => x.Equals(method, StringComparison.OrdinalIgnoreCase));
	public static bool TryVolume(string method, out AudioVolume volume) => Methods.TryGetValue(method, out volume);
}

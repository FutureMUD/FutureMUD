#nullable enable

using System.Diagnostics;
using MudSharp.Effects.Concrete;
using MudSharp.GameItems;
using MudSharp.Construction;
using System.Globalization;

namespace MudSharp.Magic;

public static class PsychometricRecorder
{
	public const string EnabledSetting = "EnablePsychometricImpressions";
	public const string EpochSetting = "PsychometricImpressionEpoch";
	private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<PsychometricHistoryEffect, object> Payloads = new();
	internal static void TrackPayload(PsychometricHistoryEffect effect) => Payloads.Add(effect, new object());
	public static int ActivePayloads => Payloads.Count();
	public static long Recorded { get; private set; }
	public static long Coalesced { get; private set; }
	public static long RecordingTicks { get; private set; }
	public static long Evictions { get; private set; }
	public static bool Enabled(IFuturemud world) => world.GetStaticBool(EnabledSetting);

	private static PsychometricHistoryEffect GetOrCreate(IPerceivable owner)
	{
		var effect = owner.EffectsOfType<PsychometricHistoryEffect>().FirstOrDefault();
		if (effect is not null) return effect;
		effect = new PsychometricHistoryEffect(owner);
		owner.AddEffect(effect);
		return effect;
	}

	public static PsychometricHistory? Read(IPerceivable owner)
	{
		if (!Enabled(owner.Gameworld)) return null;
		if (owner is IGameItem item) ObserveCustody(item);
		var history = owner.EffectsOfType<PsychometricHistoryEffect>().FirstOrDefault()?.History;
		history?.Prune(RuntimeClock.UtcNow);
		return history;
	}

	public static void ObserveCustody(IGameItem item)
	{
		if (!Enabled(item.Gameworld)) return;
		var carrier = item.InInventoryOf?.Actor;
		if (carrier is null && !item.EffectsOfType<PsychometricHistoryEffect>().Any()) return;
		var history = GetOrCreate(item).History;
		if (history.ObserveCarrier(carrier is null ? null : CharacterInstanceIdentityComparer.IdentityId(carrier),
			RuntimeClock.UtcNow, item.Gameworld.GetStaticConfiguration(EpochSetting))) item.EffectsChanged = true;
	}

	public static void Record(ICharacter source, ImpressionKind kind, string text, IPerceivable? target = null, long schoolId = 0, bool directItemOnly = false)
	{
		if (!Enabled(source.Gameworld) || source.Location is null) return;
		var start = Stopwatch.GetTimestamp();
		var now = RuntimeClock.UtcNow;
		var impression = new PsychometricImpression(kind, CharacterInstanceIdentityComparer.IdentityId(source),
			target is ICharacter ch ? CharacterInstanceIdentityComparer.IdentityId(ch) : target?.Id, now,
			now.Add(kind == ImpressionKind.Feeling ? TimeSpan.FromMinutes(10) : TimeSpan.FromHours(24)),
			text.Sanitise().RawText(), (int)source.RoomLayer,
			RouteSpatialService.Instance.GetEffectiveLocation(source).RoutePositionMetres?.ToString("R", CultureInfo.InvariantCulture) ?? "", schoolId);
		if (!directItemOnly) Append(source.Location, impression);
		if (target is IGameItem item) Append(item, impression);
		RecordingTicks += Stopwatch.GetTimestamp() - start;
	}

	private static void Append(IPerceivable owner, PsychometricImpression impression)
	{
		var history = GetOrCreate(owner).History;
		var before = history.Impressions.Count;
		if (history.Record(impression, owner is IGameItem))
		{
			Evictions += Math.Max(0, before + 1 - history.Impressions.Count);
			owner.EffectsChanged = true;
			Recorded++;
		}
		else Coalesced++;
	}

	public static bool IsLocal(ICharacter reader, PsychometricImpression impression)
	{
		if (impression.Layer != (int)reader.RoomLayer) return false;
		if (string.IsNullOrEmpty(impression.Position)) return true;
		var position = RouteSpatialService.Instance.GetEffectiveLocation(reader).RoutePositionMetres;
		return position.HasValue && double.TryParse(impression.Position, NumberStyles.Float, CultureInfo.InvariantCulture, out var recorded) &&
			Math.Abs(position.Value - recorded) <= RouteSpatialConfiguration.FromGameworld(reader.Gameworld).ProximateDistanceMetres;
	}

	public static void CopyHistory(IGameItem source, IGameItem destination)
	{
		// Copy stored facts even while recording is disabled; access remains gated.
		var original = source.EffectsOfType<PsychometricHistoryEffect>().FirstOrDefault()?.History;
		if (original is null) return;
		var copy = GetOrCreate(destination).History;
		if (ReferenceEquals(copy, original)) return;
		copy.RestoreCustody(original.Epoch, original.CurrentCarrier, original.PreviousCarriers);
		copy.MixedProvenance = original.MixedProvenance;
		copy.Impressions.Clear();
		copy.Impressions.AddRange(original.Impressions);
		destination.EffectsChanged = true;
	}

	public static void MergeHistory(IGameItem destination, IGameItem source)
	{
		var left = destination.EffectsOfType<PsychometricHistoryEffect>().FirstOrDefault()?.History;
		var right = source.EffectsOfType<PsychometricHistoryEffect>().FirstOrDefault()?.History;
		if (left is null && right is null) return;
		if (left is not null && right is not null && left.CurrentCarrier == right.CurrentCarrier &&
		    left.PreviousCarriers.SequenceEqual(right.PreviousCarriers) && left.Impressions.SequenceEqual(right.Impressions) && !right.MixedProvenance) return;
		var result = GetOrCreate(destination).History;
		result.MixedProvenance = true;
		result.RestoreCustody(result.Epoch, null, []);
		// A mixed stack cannot truthfully attribute custody or events to all its units.
		result.Impressions.RemoveAll(x => x.Kind != ImpressionKind.Authored);
		if (right is not null)
		{
			foreach (var clue in right.Impressions.Where(x => x.Kind == ImpressionKind.Authored).ToList())
			{
				if (!result.Impressions.Contains(clue)) result.Record(clue, true);
			}
		}
		destination.EffectsChanged = true;
	}

	public static bool AuthorClue(IPerceivable owner, ICharacter author, string text)
	{
		if (!Enabled(owner.Gameworld)) return false;
		Append(owner, new(ImpressionKind.Authored, CharacterInstanceIdentityComparer.IdentityId(author), null,
			RuntimeClock.UtcNow, null, text.Sanitise().RawText(), (int)author.RoomLayer, ""));
		return true;
	}
}

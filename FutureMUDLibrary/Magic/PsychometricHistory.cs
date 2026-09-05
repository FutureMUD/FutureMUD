#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Magic;

public enum ImpressionKind { Magic, Violence, Death, Feeling, Authored }

public sealed record PsychometricImpression(ImpressionKind Kind, long SourceId, long? TargetId,
	DateTime CreatedUtc, DateTime? ExpiresUtc, string Text, int Layer, string Position, long SchoolId = 0);
public sealed record CustodyPeriod(long CarrierId, DateTime SinceUtc, DateTime? UntilUtc, bool UnknownBeginning);

/// <summary>Bounded entity history. This type has no timers, global index, or persistence side effects.</summary>
public sealed class PsychometricHistory
{
	public List<PsychometricImpression> Impressions { get; } = [];
	public List<CustodyPeriod> PreviousCarriers { get; } = [];
	public CustodyPeriod? CurrentCarrier { get; private set; }
	public string Epoch { get; private set; } = "";
	public bool MixedProvenance { get; set; }

	public void RestoreCustody(string epoch, CustodyPeriod? current, IEnumerable<CustodyPeriod> previous)
	{
		Epoch = epoch;
		CurrentCarrier = current;
		PreviousCarriers.Clear();
		PreviousCarriers.AddRange(previous.TakeLast(4));
	}

	public bool ObserveCarrier(long? carrierId, DateTime now, string epoch)
	{
		if (Epoch != epoch)
		{
			// An unobserved interval cannot honestly be counted as uninterrupted custody.
			CurrentCarrier = carrierId is > 0 ? new(carrierId.Value, now, null, true) : null;
			Epoch = epoch;
			return true;
		}
		if (CurrentCarrier?.CarrierId == carrierId) return false;
		if (CurrentCarrier is not null)
		{
			PreviousCarriers.Add(CurrentCarrier with { UntilUtc = now });
			while (PreviousCarriers.Count > 4) PreviousCarriers.RemoveAt(0);
		}
		CurrentCarrier = carrierId is > 0 ? new(carrierId.Value, now, null, false) : null;
		return true;
	}

	public bool Record(PsychometricImpression impression, bool item)
	{
		Prune(impression.CreatedUtc);
		if (impression.Kind != ImpressionKind.Death && impression.Kind != ImpressionKind.Authored &&
		    Impressions.Any(x => x.Kind == impression.Kind && x.SourceId == impression.SourceId &&
		                         x.TargetId == impression.TargetId && x.Layer == impression.Layer &&
		                         x.Position == impression.Position && x.SchoolId == impression.SchoolId &&
		                         impression.CreatedUtc >= x.CreatedUtc &&
		                         impression.CreatedUtc - x.CreatedUtc < TimeSpan.FromSeconds(30))) return false;
		var kind = impression.Kind == ImpressionKind.Death ? ImpressionKind.Violence : impression.Kind;
		bool SameBucket(PsychometricImpression x) => item ? x.Kind != ImpressionKind.Authored :
			(x.Kind == ImpressionKind.Death ? ImpressionKind.Violence : x.Kind) == kind;
		var cap = impression.Kind == ImpressionKind.Authored ? 8 : item ? 8 : kind == ImpressionKind.Feeling ? 4 : 32;
		var bucket = Impressions.Where(impression.Kind == ImpressionKind.Authored ? x => x.Kind == ImpressionKind.Authored : SameBucket).ToList();
		while (bucket.Count >= cap)
		{
			Impressions.Remove(bucket[0]);
			bucket.RemoveAt(0);
		}
		Impressions.Add(impression with { Text = impression.Text[..Math.Min(impression.Text.Length, 256)] });
		return true;
	}

	public void Prune(DateTime now) => Impressions.RemoveAll(x => x.ExpiresUtc is { } expiry && expiry <= now);
}

#nullable enable

using System.Globalization;
using System.Text.Json;
using MudSharp.Economy;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private sealed record DigestIncident(string Code, long? Zone, long? Currency, long? LastScheduledSnapshotId);

	internal void ProcessEconomyDigest(Func<bool> connected, Action<string, string> send)
	{
		if (!connected()) { return; }
		var subscribers = _store.EconomyDigestSubscribers();
		if (subscribers.Count == 0) { return; }

		var current = _world.EconomyAnalytics.GetDigestRisks()
			.ToDictionary(x => new DigestIncident(x.Code, x.EconomicZoneId, x.CurrencyId, x.LastScheduledSnapshotId));
		foreach (var link in subscribers)
		{
			if (!connected()) { break; }
			if (!DiscordRemotePolicy.CanUse("economy.digest", _resolveAccount(link.AccountId))) { continue; }
			try
			{
				var observed = JsonSerializer.Deserialize<HashSet<DigestIncident>>(link.EconomyRiskDigestState!) ??
					throw new JsonException("Missing digest state.");
				var changes = current.Keys.Except(observed).Concat(observed.Except(current.Keys))
					.OrderBy(x => x.Code, StringComparer.Ordinal).ThenBy(x => x.Zone).ThenBy(x => x.Currency)
					.ToList();
				foreach (var batch in changes.Chunk(5))
				{
					if (!connected()) { return; }
					var lines = new List<string>();
					foreach (var incident in batch)
					{
						var active = current.TryGetValue(incident, out var risk);
						if (active) { observed.Add(incident); } else { observed.Remove(incident); }
						if (incident.Code == "stale-snapshot")
						{
							if (active) { lines.Add($"Snapshot overdue. {risk!.Description} See /economy snapshotstatus."); }
							continue;
						}
						var location = DigestLabel(_world.EconomicZones.Get(incident.Zone ?? 0)?.Name, "Zone", incident.Zone);
						var currency = DigestLabel(_world.Currencies.Get(incident.Currency ?? 0)?.Name, "Currency", incident.Currency);
						lines.Add(active
							? $"Reserve shortfall in {location}, {currency}. {risk!.Description}"
							: $"Reserve shortfall cleared in {location}, {currency}.");
					}
					// Claim this batch before writing. An ambiguous hand-off must not replay after restart.
					var state = JsonSerializer.Serialize(observed.OrderBy(x => x.Code, StringComparer.Ordinal)
						.ThenBy(x => x.Zone).ThenBy(x => x.Currency));
					if (!_store.TryAdvanceEconomyDigest(link, state)) { break; }
					link.EconomyRiskDigestState = state;
					if (lines.Count > 0)
					{
						send(link.DiscordUserId, "Economy risk digest\n" + string.Join("\n", lines) + "\nSee /economy risks for current conditions.");
					}
				}
			}
			catch (JsonException)
			{
				// Preserve the recipient's checkpoint instead of replaying their current risks as new.
				Console.WriteLine($"Invalid economy risk digest state for account #{link.AccountId}.");
			}
		}
	}

	private static string DigestLabel(string? name, string fallback, long? id)
	{
		var label = string.IsNullOrWhiteSpace(name) ? fallback : name.ReplaceLineEndings(" ");
		if (label.Length > 60) { label = label[..60]; }
		return $"{label} (#{id?.ToString(CultureInfo.InvariantCulture) ?? "unknown"})";
	}
}

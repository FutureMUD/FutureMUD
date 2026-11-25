#nullable enable

using System.Globalization;
using System.Text;
using System.Text.Json;
using MudSharp.Database;
using MudSharp.RPG.AIStorytellers;

namespace MudSharp.Discord;

internal sealed partial class DiscordRemoteService
{
	private IAIStoryteller FindStoryteller(JsonElement args)
	{
		var target = TextArgument(args, "target").Trim();
		if (long.TryParse(target, NumberStyles.None, CultureInfo.InvariantCulture, out var id))
		{
			return _world.AIStorytellers.Get(id) ?? throw new ArgumentException("No storyteller has that ID.");
		}

		var matches = _world.AIStorytellers.Where(x => x.Name.Equals(target, StringComparison.OrdinalIgnoreCase))
			.Take(2)
			.ToList();
		return matches.Count switch
		{
			1 => matches[0],
			0 => throw new ArgumentException("Supply a storyteller ID or exact name from /storyteller overview."),
			_ => throw new ArgumentException("Several storytellers share that name. Select one by ID.")
		};
	}

	private string StorytellerOverviewReport(long after)
	{
		if (after < 0) { throw new ArgumentException("Supply a non-negative storyteller ID for after."); }
		const int pageSize = 10;
		var page = _world.AIStorytellers.Where(x => x.Id > after)
			.OrderBy(x => x.Id)
			.Take(pageSize + 1)
			.ToList();
		if (page.Count == 0) { return after == 0 ? "No AI storytellers are configured." : "No further AI storytellers."; }

		var report = new StringBuilder("AI storytellers | usage and failures since startup\n");
		foreach (var storyteller in page.Take(pageSize))
		{
			var stats = storyteller.GetOverview();
			report.AppendLine($"#{storyteller.Id} {storyteller.Name} ({(storyteller.IsPaused ? "paused" : "enabled")}) | " +
				$"{stats.QueuedBatches} queued, active: {stats.ActiveRole ?? "none"} | {stats.Requests} requests | " +
				$"{stats.InputTokens} input / {stats.OutputTokens} output tokens | {stats.Failures} failures");
		}
		if (page.Count > pageSize) { report.AppendLine($"Continue with /storyteller overview after:{page[pageSize - 1].Id}"); }
		return report.ToString().TrimEnd();
	}

	private static string StorytellerStatus(IAIStoryteller target) =>
		$"#{target.Id} {target.Name} ({(target.IsPaused ? "paused" : "enabled")})\n" +
		target.DescribeUsage(CultureInfo.InvariantCulture).RawText();

	private static string SetStorytellerPaused(IAIStoryteller target, bool paused)
	{
		// Commit the switch before cancelling work and before the remote invocation is marked completed.
		using (new FMDB())
		{
			var saved = FMDB.Context.AIStorytellers.Find(target.Id) ??
				throw new ArgumentException("This storyteller no longer exists.");
			saved.IsPaused = paused;
			FMDB.Context.SaveChanges();
		}
		var state = paused ? "paused" : "enabled";
		if (target.IsPaused == paused) { return $"{target.Name} (#{target.Id}) is already {state}."; }
		if (paused) { target.Pause(); } else { target.Unpause(); }
		return $"{target.Name} (#{target.Id}) is now {state}.";
	}
}

using DSharpPlus.CommandsNext;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot;

public record CachedDiscordRequest()
{
	public static readonly TimeSpan Lifetime = TimeSpan.FromSeconds(45);
	public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
	public TimeSpan Timeout { get; init; } = Lifetime;
	public bool IsExpired(DateTime utcNow) => utcNow - CreatedUtc >= Timeout;

    private static ulong _nextId;
    private static readonly object NextIdLock = new();
    public static ulong NextId
    {
        get
        {
            lock (NextIdLock)
            {
                _nextId += 1;
                return _nextId;
            }
        }
    }

    public ulong RequestId { get; private init; } = NextId;
    public CommandContext Context { get; init; }
    public Func<string, CommandContext, Task> OnResponseAction { get; init; }
}

public sealed class CachedDiscordRequestCache : ConcurrentDictionary<ulong, CachedDiscordRequest>
{
	private const int MaximumEntries = 256;
	private readonly object _gate = new();

	public async Task<bool> TryAddRequestAsync(CachedDiscordRequest request)
	{
		await SweepExpiredAsync();
		lock (_gate)
		{
			return Count < MaximumEntries && TryAdd(request.RequestId, request);
		}
	}

	public async Task SweepExpiredAsync()
	{
		foreach (var pair in this)
		{
			if (pair.Value.IsExpired(DateTime.UtcNow) && TryRemove(pair.Key, out var expired))
			{
				await DiscordBot.SendPrivateCommandResponseAsync(expired.Context,
					"The game server did not respond in time. Check the relevant game state before retrying.");
			}
		}
	}

	public async Task FailAllAsync(string reason)
	{
		foreach (var pair in this)
		{
			if (TryRemove(pair.Key, out var pending))
			{
				await DiscordBot.SendPrivateCommandResponseAsync(pending.Context, reason);
			}
		}
	}

}

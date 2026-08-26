using System.Collections.Concurrent;

namespace TerrainPlanner.Server.Authentication;

public interface ILoginAttemptLimiter
{
	bool TryAcquire(string clientAddress, string accountName);
	void Reset(string clientAddress, string accountName);
}

public sealed class LoginAttemptLimiter : ILoginAttemptLimiter
{
	private static readonly TimeSpan Window = TimeSpan.FromMinutes(1);
	private const int MaximumAttempts = 5;
	private readonly ConcurrentDictionary<string, AttemptWindow> _attempts = new(StringComparer.Ordinal);
	private int _requestCount;

	public bool TryAcquire(string clientAddress, string accountName)
	{
		var now = DateTimeOffset.UtcNow;
		if ((Interlocked.Increment(ref _requestCount) & 0xff) == 0)
		{
			Prune(now);
		}

		var addressWindow = Acquire($"address:{clientAddress}", now);
		var accountWindow = Acquire($"account:{accountName.Trim().ToLowerInvariant()}", now);
		return addressWindow.Attempts <= MaximumAttempts && accountWindow.Attempts <= MaximumAttempts;
	}

	public void Reset(string clientAddress, string accountName)
	{
		_attempts.TryRemove($"address:{clientAddress}", out _);
		_attempts.TryRemove($"account:{accountName.Trim().ToLowerInvariant()}", out _);
	}

	private AttemptWindow Acquire(string key, DateTimeOffset now) =>
		_attempts.AddOrUpdate(key,
			_ => new AttemptWindow(now, 1),
			(_, current) => now - current.StartedAt >= Window
				? new AttemptWindow(now, 1)
				: current with { Attempts = current.Attempts + 1 });

	private void Prune(DateTimeOffset now)
	{
		foreach (var attempt in _attempts.Where(item => now - item.Value.StartedAt >= Window))
		{
			_attempts.TryRemove(attempt);
		}
	}

	private sealed record AttemptWindow(DateTimeOffset StartedAt, int Attempts);
}

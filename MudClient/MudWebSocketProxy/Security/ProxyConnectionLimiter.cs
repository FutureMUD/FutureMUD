using System.Collections.Concurrent;

namespace MudWebSocketProxy.Security;

public sealed class ProxyConnectionLimiter
{
	private readonly ProxyLimits _limits;
	private readonly ConcurrentDictionary<string, int> _connectionsByAddress = new(StringComparer.Ordinal);
	private int _totalConnections;

	public ProxyConnectionLimiter(ProxyLimits limits)
	{
		_limits = limits;
	}

	public IDisposable? TryAcquire(string address)
	{
		address = string.IsNullOrWhiteSpace(address) ? "unknown" : address;

		if (Interlocked.Increment(ref _totalConnections) > _limits.MaximumConcurrentConnections)
		{
			Interlocked.Decrement(ref _totalConnections);
			return null;
		}

		var addressCount = _connectionsByAddress.AddOrUpdate(address, 1, static (_, current) => current + 1);
		if (addressCount > _limits.MaximumConnectionsPerIp)
		{
			Release(address);
			return null;
		}

		return new Lease(this, address);
	}

	private void Release(string address)
	{
		Interlocked.Decrement(ref _totalConnections);
		_connectionsByAddress.AddOrUpdate(address, 0, static (_, current) => Math.Max(0, current - 1));
		if (_connectionsByAddress.TryGetValue(address, out var count) && count == 0)
		{
			_connectionsByAddress.TryRemove(new KeyValuePair<string, int>(address, 0));
		}
	}

	private sealed class Lease : IDisposable
	{
		private ProxyConnectionLimiter? _owner;
		private readonly string _address;

		public Lease(ProxyConnectionLimiter owner, string address)
		{
			_owner = owner;
			_address = address;
		}

		public void Dispose()
		{
			Interlocked.Exchange(ref _owner, null)?.Release(_address);
		}
	}
}

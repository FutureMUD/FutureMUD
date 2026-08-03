namespace MudWebSocketProxy.Security;

public sealed class ClientTrafficLimiter
{
	private readonly ProxyLimits _limits;
	private DateTimeOffset _windowStarted;
	private int _messages;
	private int _bytes;

	public ClientTrafficLimiter(ProxyLimits limits, DateTimeOffset now)
	{
		_limits = limits;
		_windowStarted = now;
	}

	public bool TryConsumeMessage(int bytes, DateTimeOffset now)
	{
		if (now - _windowStarted >= TimeSpan.FromSeconds(1))
		{
			_windowStarted = now;
			_messages = 0;
			_bytes = 0;
		}

		if (bytes < 0 ||
		    _messages + 1 > _limits.MaximumClientMessagesPerSecond ||
		    _bytes + bytes > _limits.MaximumClientBytesPerSecond)
		{
			return false;
		}

		_messages++;
		_bytes += bytes;
		return true;
	}
}

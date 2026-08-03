namespace MudWebSocketProxy.Security;

public sealed class ByteTrafficLimiter
{
	private readonly int _maximumBytesPerSecond;
	private DateTimeOffset _windowStarted;
	private int _bytes;

	public ByteTrafficLimiter(int maximumBytesPerSecond, DateTimeOffset now)
	{
		if (maximumBytesPerSecond < 1)
		{
			throw new ArgumentOutOfRangeException(nameof(maximumBytesPerSecond));
		}

		_maximumBytesPerSecond = maximumBytesPerSecond;
		_windowStarted = now;
	}

	public bool TryConsume(int bytes, DateTimeOffset now)
	{
		if (now - _windowStarted >= TimeSpan.FromSeconds(1))
		{
			_windowStarted = now;
			_bytes = 0;
		}

		if (bytes < 0 || _bytes + bytes > _maximumBytesPerSecond)
		{
			return false;
		}

		_bytes += bytes;
		return true;
	}
}

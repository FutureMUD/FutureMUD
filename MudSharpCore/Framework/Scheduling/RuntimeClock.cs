using System.Threading;

#nullable enable

namespace MudSharp.Framework.Scheduling;

/// <summary>
/// Supplies an ambient time provider for accelerated or otherwise isolated runtime work.
/// Ordinary engine execution continues to use <see cref="TimeProvider.System"/>.
/// </summary>
public static class RuntimeClock
{
	private static readonly AsyncLocal<TimeProvider?> AmbientProvider = new();

	public static TimeProvider TimeProvider => AmbientProvider.Value ?? System.TimeProvider.System;

	public static DateTime UtcNow => TimeProvider.GetUtcNow().UtcDateTime;

	public static IDisposable Push(TimeProvider timeProvider)
	{
		ArgumentNullException.ThrowIfNull(timeProvider);
		var previous = AmbientProvider.Value;
		AmbientProvider.Value = timeProvider;
		return new PopScope(previous);
	}

	private sealed class PopScope(TimeProvider? previous) : IDisposable
	{
		private bool _disposed;

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			AmbientProvider.Value = previous;
			_disposed = true;
		}
	}
}

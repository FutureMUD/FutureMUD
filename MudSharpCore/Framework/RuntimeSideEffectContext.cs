using System.Threading;

#nullable enable

namespace MudSharp.Framework;

/// <summary>
/// Supplies flow-local policy for runtime work that must suppress selected external side effects.
/// Ordinary engine execution permits all side effects.
/// </summary>
internal static class RuntimeSideEffectContext
{
	private static readonly AsyncLocal<bool> AmbientCrimeCreationSuppression = new();

	public static bool IsCrimeCreationSuppressed => AmbientCrimeCreationSuppression.Value;

	public static IDisposable SuppressCrimeCreation()
	{
		var previous = AmbientCrimeCreationSuppression.Value;
		AmbientCrimeCreationSuppression.Value = true;
		return new PopScope(previous);
	}

	private sealed class PopScope(bool previous) : IDisposable
	{
		private bool _disposed;

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			AmbientCrimeCreationSuppression.Value = previous;
			_disposed = true;
		}
	}
}

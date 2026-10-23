#nullable enable

using System.Collections.Concurrent;
using System.IO;

namespace MudSharp.Server;

/// <summary>Marshals background crash notifications to the game loop that owns the Discord connection.</summary>
internal sealed class ExceptionNotificationQueue
{
	private readonly ConcurrentQueue<string> _pending = new();

	internal void Enqueue(string report) => _pending.Enqueue(report);

	internal void Drain(Action<string> notify, TextWriter? fallback = null)
	{
		while (_pending.TryDequeue(out var report))
		{
			try { notify(report); }
			catch (Exception error)
			{
				ExceptionReporter.WriteFallback(fallback, $"Exception notification unavailable ({error.GetType().Name}).");
			}
		}
	}
}

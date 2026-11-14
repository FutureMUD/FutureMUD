#nullable enable

using System.Diagnostics;

namespace MudSharp.Framework.Diagnostics;

internal sealed class StartupDiagnostics
{
	private readonly Stopwatch _total = Stopwatch.StartNew();
	private bool _firstIterationRecorded;

	internal void Measure(string name, Action action)
	{
		ConsoleUtilities.WriteLine($"[BOOT] Starting {name}...");
		var start = Stopwatch.GetTimestamp();
		try { action(); }
		finally { ConsoleUtilities.WriteLine($"[BOOT] {name}: {Stopwatch.GetElapsedTime(start).TotalSeconds:F3}s"); }
	}

	internal void LoaderComplete() =>
		ConsoleUtilities.WriteLine($"[BOOT] Loader, completion callbacks and services: {_total.Elapsed.TotalSeconds:F3}s");

	internal void FirstIterationComplete()
	{
		if (_firstIterationRecorded) return;
		_firstIterationRecorded = true;
		ConsoleUtilities.WriteLine($"[BOOT] First game-loop iteration complete: {_total.Elapsed.TotalSeconds:F3}s since loader start");
	}
}

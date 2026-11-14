#nullable enable

namespace MudSharp.FutureProg;

/// <summary>Indexes user-defined overloads only for a synchronous compilation batch.</summary>
internal sealed class FutureProgCompilationBatch : IDisposable
{
	[ThreadStatic] private static FutureProgCompilationBatch? _current;
	private readonly FutureProgCompilationBatch? _previous;
	private readonly IFuturemud _gameworld;
	private readonly Dictionary<string, List<IFutureProg>> _overloads = new(StringComparer.InvariantCultureIgnoreCase);
	private bool _disposed;

	internal FutureProgCompilationBatch(IFuturemud gameworld, IEnumerable<IFutureProg> progs)
	{
		_gameworld = gameworld;
		foreach (var prog in progs)
		{
			if (!_overloads.TryGetValue(prog.FunctionName, out var overloads))
			{
				overloads = [];
				_overloads.Add(prog.FunctionName, overloads);
			}

			overloads.Add(prog);
		}

		_previous = _current;
		_current = this;
	}

	internal static IFutureProg? Find(IFuturemud gameworld, string name, IReadOnlyList<ProgVariableTypes> parameters)
	{
		if (_current is { } batch && ReferenceEquals(batch._gameworld, gameworld))
		{
			return batch._overloads.TryGetValue(name, out var overloads)
				? overloads.FirstOrDefault(x => x.MatchesParameters(parameters))
				: null;
		}

		return gameworld.FutureProgs.FirstOrDefault(x =>
			string.Equals(x.FunctionName, name, StringComparison.InvariantCultureIgnoreCase) &&
			x.MatchesParameters(parameters));
	}

	public void Dispose()
	{
		if (_disposed) return;
		if (!ReferenceEquals(_current, this)) throw new InvalidOperationException("Compilation batches must be disposed in nesting order.");
		_current = _previous;
		_disposed = true;
	}
}

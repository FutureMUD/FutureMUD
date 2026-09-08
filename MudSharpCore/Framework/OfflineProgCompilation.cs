#nullable enable

using System;
using System.Collections.Generic;
using MudSharp.FutureProg;

namespace MudSharp.Framework;

public sealed partial class Futuremud
{
	internal static Futuremud CreateOfflineProgCompilationWorld()
	{
		var world = new Futuremud(null!);
		world._staticConfigurations["DisplayProgsInDarkMode"] = "false";
		return world;
	}
}

/// <summary>
/// Uses the engine's real compiler and installed prog identities without opening a server,
/// loading a game database, or executing progs. Runtime execution still needs a loaded world.
/// </summary>
public sealed class OfflineProgCompilation : IDisposable
{
	private readonly Futuremud _world;
	private readonly Dictionary<long, IFutureProg> _progs = new();

	public OfflineProgCompilation(IEnumerable<Models.FutureProg> definitions)
	{
		MudSharp.FutureProg.FutureProg.Initialise();
		_world = Futuremud.CreateOfflineProgCompilationWorld();
		try
		{
			foreach (var definition in definitions)
			{
				var prog = new MudSharp.FutureProg.FutureProg(definition, _world);
				_progs.Add(definition.Id, prog);
				_world.Add(prog);
			}
		}
		catch
		{
			_world.Dispose();
			throw;
		}
	}

	public IFutureProg Compile(long id)
	{
		var prog = _progs[id];
		if (!prog.Compile())
		{
			throw new InvalidOperationException($"Prog {prog.Id} ({prog.FunctionName}) failed compilation: {prog.CompileError}");
		}
		return prog;
	}

	public void Dispose() => _world.Dispose();
}

extern alias EngineCompiler;

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal sealed partial class ArmageddonWorldfileWriter
{
	private FutureProg Prog(string name, string text, ProgVariableTypes returns,
		params (string Name, ProgVariableTypes Type)[] parameters)
	{
		if (_progs.Any(x => x.FunctionName.Equals(name, StringComparison.OrdinalIgnoreCase)) ||
			_db.FutureProgs.Any(x => x.FunctionName == name))
			throw new InvalidOperationException($"Prog name {name} already exists; no existing program may be replaced.");
		var prog = new FutureProg
		{
			Id = Next<FutureProg>(), FunctionName = name, FunctionText = text,
			ReturnTypeDefinition = returns.ToStorageString(), AcceptsAnyParameters = false, StaticType = 0,
			Category = "Armageddon", Subcategory = "Worldfile", Public = false,
			FunctionComment = "Generated during the one-shot historical world import. Source and adaptations are retained in seeder provenance."
		};
		foreach (var (parameter, index) in parameters.Select((value, index) => (value, index)))
			prog.FutureProgsParameters.Add(new()
			{
				ParameterIndex = index, ParameterName = parameter.Name,
				ParameterTypeDefinition = parameter.Type.ToStorageString()
			});
		_db.FutureProgs.Add(prog);
		_progs.Add(prog);
		return prog;
	}

	private void CompileProgs()
	{
		Flush();
		using var compiler = new EngineCompiler::MudSharp.Framework.OfflineProgCompilation(
			_db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray(),
			_db.VariableDefinitions.AsNoTracking().ToArray());
		var errors = new List<string>();
		foreach (var prog in _progs)
		{
			try { compiler.Compile(prog.Id); }
			catch (InvalidOperationException error) { errors.Add(error.Message); }
		}
		if (errors.Count != 0)
		{
			var examples = errors.GroupBy(x => x[(x.IndexOf("failed compilation:", StringComparison.Ordinal) + "failed compilation:".Length)..])
				.Select(x => $"{x.First()} ({x.Count()} occurrence(s))");
			throw new InvalidOperationException($"{errors.Count} generated progs failed native compilation:\n{string.Join("\n", examples.Take(25))}");
		}
		_counts["compiled progs"] = _progs.Count;
	}
}

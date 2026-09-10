#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using DbProg = MudSharp.Models.FutureProg;

namespace DatabaseSeeder.Seeders.CultureToolkit;

internal sealed record CultureProgDefinition(string Key, string Name, string Body, ProgVariableTypes Returns,
	IReadOnlyList<(ProgVariableTypes Type, string Name)> Parameters);

internal static class CultureToolkitProgSeeder
{
	public static DbProg Upsert(FuturemudDatabaseContext context, string era, string key, string name, string body,
		ProgVariableTypes returns, IReadOnlyList<(ProgVariableTypes Type, string Name)> parameters, ICollection<string> conflicts)
		=> UpsertMany(context, era, [new(key, name, body, returns, parameters)], conflicts)[key];

	public static IReadOnlyDictionary<string, DbProg> UpsertMany(FuturemudDatabaseContext context, string era,
		IReadOnlyList<CultureProgDefinition> definitions, ICollection<string> conflicts)
	{
		var result = new Dictionary<string, DbProg>();
		var freshKeys = new HashSet<string>();
		foreach (var (key, name, body, returns, parameters) in definitions)
		{
			if (Encoding.UTF8.GetByteCount(body) > 65535)
				throw new InvalidOperationException($"Generated prog {name} exceeds MySQL FunctionText capacity; partition it before activation.");
			var record = CultureToolkitManagedEntities.Find(context, "FutureProg", key);
			var fresh = record is null;
			var prog = fresh ? new DbProg
			{
				FunctionName = name, FunctionText = body, ReturnType = (long)returns, AcceptsAnyParameters = false,
				Category = "Chargen", Subcategory = "Culture Toolkit", FunctionComment = "Culture toolkit starting content; editable in game.",
				Public = false, StaticType = 0
			} : context.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == record!.LogicalId);
			if (fresh)
			{
				if (context.FutureProgs.Any(x => x.FunctionName == name)) throw new InvalidOperationException($"Unowned prog collision: {name}");
				for (var i = 0; i < parameters.Count; i++) prog.FutureProgsParameters.Add(new FutureProgsParameter
				{
					ParameterIndex = i, ParameterName = parameters[i].Name, ParameterType = (long)parameters[i].Type
				});
				context.FutureProgs.Add(prog);
				freshKeys.Add(key);
			}
			Validate(prog, returns, parameters.Select(x => x.Type).ToArray());
			result.Add(key, prog);
		}
		// Allocate all new identities together before recording their ownership baselines.
		if (freshKeys.Count > 0) context.SaveChanges();
		foreach (var (key, name, body, returns, parameters) in definitions)
		{
			var prog = result[key];
			var fresh = freshKeys.Contains(key);
			var merged = CultureToolkitManagedEntities.Reconcile(context, era, "FutureProg", key, prog.Id, fresh,
				new Dictionary<string, string> { ["name"] = prog.FunctionName, ["body"] = prog.FunctionText },
				new Dictionary<string, string> { ["name"] = name, ["body"] = body }, conflicts);
			prog.FunctionName = merged["name"];
			prog.FunctionText = merged["body"];
		}
		context.SaveChanges();
		return result;
	}

	public static void Validate(DbProg prog, ProgVariableTypes returns, params ProgVariableTypes[] parameters)
	{
		if (prog.AcceptsAnyParameters || prog.ReturnType != (long)returns ||
			!prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => x.ParameterType).SequenceEqual(parameters.Select(x => (long)x)))
			throw new InvalidOperationException($"Prog {prog.Id} ({prog.FunctionName}) has an incompatible signature; preserved without rewiring.");
	}
}

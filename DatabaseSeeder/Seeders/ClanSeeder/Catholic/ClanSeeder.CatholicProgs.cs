#nullable enable

using System;
using System.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ClanSeeder
{
	private static readonly string[] CatholicProvisioningProgNames =
		["CatholicClanStatus", "CatholicClanBind", "CatholicClanVariant", "CatholicClanActivate", "CatholicClanPreflight", "CatholicClanExport"];

	private static void EnsureCatholicProvisioningSuite(FuturemudDatabaseContext context)
	{
		foreach (var name in CatholicProvisioningProgNames)
		{
			var action = name["CatholicClan".Length..];
			if (context.FutureProgs.Any(x => x.FunctionName == name)) continue;
			var takesArguments = action is "Bind" or "Variant" or "Preflight";
			var prog = new FutureProg
			{
				FunctionName = name, Category = "Clan", Subcategory = "Catholic Templates",
				Public = false, AcceptsAnyParameters = false, StaticType = (int)FutureProgStaticType.NotStatic,
				ReturnType = (long)ProgVariableTypes.Text,
				FunctionComment = $"Catholic template provisioning: {action}. Run with prog execute {name} self <clan>" +
					(takesArguments ? " \"<arguments>\"." : ".") + " Uses the generic admin-only template-profile API. Existing author edits are preserved on rerun.",
				FunctionText = $"return ClanTemplateAction(@actor, @clan, \"{action.ToLowerInvariant()}\", " +
					(takesArguments ? "@arguments)" : "\"\")")
			};
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ ParameterIndex = 0, ParameterName = "actor", ParameterType = (long)ProgVariableTypes.Character });
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ ParameterIndex = 1, ParameterName = "clan", ParameterType = (long)ProgVariableTypes.Clan });
			if (takesArguments) prog.FutureProgsParameters.Add(new FutureProgsParameter
			{ ParameterIndex = 2, ParameterName = "arguments", ParameterType = (long)ProgVariableTypes.Text });
			context.FutureProgs.Add(prog);
		}
		context.SaveChanges();
	}
}

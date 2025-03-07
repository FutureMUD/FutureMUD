﻿using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class AddHookFunction : BuiltInFunction
{
	public AddHookFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		Gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	private IFuturemud Gameworld { get; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result;
		if (target == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var hookResult = ParameterFunctions[1].Result;
		if (hookResult?.GetObject == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var hook = hookResult.Type == ProgVariableTypes.Number
			? Gameworld.Hooks.Get((long)(decimal)hookResult.GetObject)
			: Gameworld.Hooks.GetByName(hookResult.GetObject.ToString());

		if (hook == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (target is not IPerceivable targetAsPerceivable)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(targetAsPerceivable.InstallHook(hook));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"addhook",
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Number },
				(pars, gameworld) => new AddHookFunction(pars, gameworld),
				new List<string> { "perceivable", "hookid" },
				new List<string>
				{
					"The perceivable for whom you want to install a hook",
					"The ID of the hook that you want to install."
				},
				"This function installs a 'hook' on a perceivable. See in game help for hooks for more info.",
				"Hooks",
				ProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"addhook",
				new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Text },
				(pars, gameworld) => new AddHookFunction(pars, gameworld),
				new List<string> { "perceivable", "hookname" },
				new List<string>
				{
					"The perceivable for whom you want to install a hook",
					"The name of the hook that you want to install."
				},
				"This function installs a 'hook' on a perceivable. See in game help for hooks for more info.",
				"Hooks",
				ProgVariableTypes.Boolean
			)
		);
	}
}
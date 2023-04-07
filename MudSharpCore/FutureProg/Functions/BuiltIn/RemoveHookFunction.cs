using System.Collections.Generic;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class RemoveHookFunction : BuiltInFunction
{
	public RemoveHookFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		Gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
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

		var hook = hookResult.Type == FutureProgVariableTypes.Number
			? Gameworld.Hooks.Get((long)(double)hookResult.GetObject)
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

		Result = new BooleanVariable(targetAsPerceivable.RemoveHook(hook));
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removehook",
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Number },
				(pars, gameworld) => new RemoveHookFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removehook",
				new[] { FutureProgVariableTypes.Perceivable, FutureProgVariableTypes.Text },
				(pars, gameworld) => new RemoveHookFunction(pars, gameworld)
			)
		);
	}
}
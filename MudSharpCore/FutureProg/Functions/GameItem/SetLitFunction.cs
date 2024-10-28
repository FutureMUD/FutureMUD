using System.Collections.Generic;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.GameItem;

internal class SetLitFunction : BuiltInFunction
{
	private SetLitFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not IGameItem itemFunction)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var lightable = itemFunction.GetItemType<ILightable>();
		if (lightable == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		lightable.Lit = (bool?)ParameterFunctions[1].Result.GetObject ?? true;
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"setlit",
			new[] { ProgVariableTypes.Item, ProgVariableTypes.Boolean },
			(pars, gameworld) => new SetLitFunction(pars)
		));
	}
}
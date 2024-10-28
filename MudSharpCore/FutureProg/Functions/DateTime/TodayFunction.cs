using System.Collections.Generic;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class TodayFunction : BuiltInFunction
{
	public TodayFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.DateTime;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		Result = new DateTimeVariable(System.DateTime.UtcNow.Date);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"today",
			new ProgVariableTypes[] { },
			(pars, gameworld) => new TodayFunction(pars)
		));
	}
}
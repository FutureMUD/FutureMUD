using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.Register;

internal class GetRegisterFunction : BuiltInFunction, IHaveFuturemud
{
	protected IFunction TargetFunction;
	protected string Variable;

	public GetRegisterFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		Gameworld = gameworld;
		Variable = parameters.ElementAt(1).Result.GetObject.ToString();
		TargetFunction = parameters.ElementAt(0);
		ReturnType = Gameworld.VariableRegister.GetType(TargetFunction.ReturnType, Variable);
	}

	#region IHaveFuturemud Members

	public IFuturemud Gameworld { get; set; }

	#endregion

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (TargetFunction.Execute(variables) == StatementResult.Error)
		{
			ErrorMessage = TargetFunction.ErrorMessage;
			return StatementResult.Error;
		}

		Result = Gameworld.VariableRegister.GetValue(TargetFunction.Result, Variable);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"getregister",
				new[]
				{
					ProgVariableTypes.CollectionItem,
					ProgVariableTypes.Text | ProgVariableTypes.Literal
				},
				(pars, gameworld) => new GetRegisterFunction(pars, gameworld),
				new[] { "thing", "variable" },
				new[]
				{
					"The thing whose register variables you want to retrieve",
					"The register variable that you want to get. Must be a string literal."
				},
				"This function retrieves the 'register variable' specified for a thing that you specify. Which register variables are available are defined on a per-type basis. See the REGISTER command in game for more information. The return type is as it appears in the register command, not actually a CollectionItem.",
				"Register",
				ProgVariableTypes.CollectionItem
			)
		);
	}
}
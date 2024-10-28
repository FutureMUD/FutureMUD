using System.Collections.Generic;
using System.Linq;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class LogicalNotFunction : BuiltInFunction
{
	protected IFunction InnerFunction;

	public LogicalNotFunction(IList<IFunction> parameters)
		: base(parameters)
	{
		InnerFunction = parameters.First();
	}

	public override IProgVariable Result
	{
		get
		{
			if (InnerFunction.Result.GetObject == null)
			{
				return new NullVariable(ProgVariableTypes.Boolean);
			}

			return new BooleanVariable(!(bool)InnerFunction.Result.GetObject);
		}
		protected set { }
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override string ErrorMessage
	{
		get => InnerFunction.ErrorMessage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		return InnerFunction.Execute(variables);
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"not",
				new[] { ProgVariableTypes.Boolean },
				(pars, gameworld) => new LogicalNotFunction(pars),
				new List<string> { "item" },
				new List<string> { "The boolean you want to change" },
				"This function takes a boolean and transforms it into the opposite value - e.g. True becomes False, False becomes True.",
				"Logical",
				ProgVariableTypes.Boolean
			)
		);
	}
}
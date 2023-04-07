using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Logical;

internal class InFunction : BuiltInFunction
{
	protected InFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[] { FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text },
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4",
					"Item5"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4",
					"Item5",
					"Item6"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4",
					"Item5",
					"Item6",
					"Item7"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4",
					"Item5",
					"Item6",
					"Item7",
					"Item8"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4",
					"Item5",
					"Item6",
					"Item7",
					"Item8",
					"Item9"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4",
					"Item5",
					"Item6",
					"Item7",
					"Item8",
					"Item9",
					"Item10"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"in",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text
				},
				(pars, gameworld) => new InFunction(pars),
				new List<string>
				{
					"Target",
					"Item1",
					"Item2",
					"Item3",
					"Item4",
					"Item5",
					"Item6",
					"Item7",
					"Item8",
					"Item9",
					"Item10",
					"Item11"
				},
				new List<string>
				{
					"The item you want to check is equal to any of the other supplied items",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare",
					"An item to compare"
				},
				"This function determines whether the first supplied argument is 'in' the list of other arguments. It will return true if any of the other supplied text items match the first.",
				"Logical",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result?.ToString();
		if (target == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		foreach (var parameter in ParameterFunctions.Skip(1))
		{
			var parameterValue = parameter.Result?.ToString();
			if (target.EqualTo(parameterValue))
			{
				Result = new BooleanVariable(true);
				return StatementResult.Normal;
			}
		}

		Result = new BooleanVariable(false);
		return StatementResult.Normal;
	}
}
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;
using System;
using System.Collections.Generic;

namespace MudSharp.FutureProg.Functions.DateTime;

internal class BetweenFunction : BuiltInFunction
{
	private readonly ProgVariableTypes _comparisonType;

	private BetweenFunction(IList<IFunction> parameters, ProgVariableTypes comparisonType)
		: base(parameters)
	{
		_comparisonType = comparisonType;
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

		if (ParameterFunctions[0].Result?.GetObject is null ||
		    ParameterFunctions[1].Result?.GetObject is null ||
		    ParameterFunctions[2].Result?.GetObject is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		Result = new BooleanVariable(_comparisonType.LegacyCode switch
		{
			ProgVariableTypeCode.Number => Between(
				(decimal)ParameterFunctions[0].Result.GetObject,
				(decimal)ParameterFunctions[1].Result.GetObject,
				(decimal)ParameterFunctions[2].Result.GetObject),
			ProgVariableTypeCode.TimeSpan => Between(
				(TimeSpan)ParameterFunctions[0].Result.GetObject,
				(TimeSpan)ParameterFunctions[1].Result.GetObject,
				(TimeSpan)ParameterFunctions[2].Result.GetObject),
			ProgVariableTypeCode.DateTime => Between(
				(System.DateTime)ParameterFunctions[0].Result.GetObject,
				(System.DateTime)ParameterFunctions[1].Result.GetObject,
				(System.DateTime)ParameterFunctions[2].Result.GetObject),
			ProgVariableTypeCode.MudDateTime => Between(
				(MudDateTime)ParameterFunctions[0].Result.GetObject,
				(MudDateTime)ParameterFunctions[1].Result.GetObject,
				(MudDateTime)ParameterFunctions[2].Result.GetObject),
			_ => false
		});

		return StatementResult.Normal;
	}

	private static bool Between(decimal value, decimal lower, decimal upper)
	{
		return lower <= upper
			? value >= lower && value <= upper
			: value >= upper && value <= lower;
	}

	private static bool Between(TimeSpan value, TimeSpan lower, TimeSpan upper)
	{
		return lower <= upper
			? value >= lower && value <= upper
			: value >= upper && value <= lower;
	}

	private static bool Between(System.DateTime value, System.DateTime lower, System.DateTime upper)
	{
		return lower <= upper
			? value >= lower && value <= upper
			: value >= upper && value <= lower;
	}

	private static bool Between(MudDateTime value, MudDateTime lower, MudDateTime upper)
	{
		return lower <= upper
			? value >= lower && value <= upper
			: value >= upper && value <= lower;
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterForType(ProgVariableTypes.Number);
		RegisterForType(ProgVariableTypes.TimeSpan);
		RegisterForType(ProgVariableTypes.DateTime);
		RegisterForType(ProgVariableTypes.MudDateTime);
	}

	private static void RegisterForType(ProgVariableTypes type)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"between",
			new[] { type, type, type },
			(pars, gameworld) => new BetweenFunction(pars, type),
			new[] { "Value", "Bound1", "Bound2" },
			new[]
			{
				"The value to test.",
				"One inclusive bound of the range.",
				"The other inclusive bound of the range."
			},
			"Returns true if the first value is inclusively between the other two values. The bounds may be supplied in either order.",
			"DateTime",
			ProgVariableTypes.Boolean
		));
	}
}

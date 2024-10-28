using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class UnitFromTextFunction : BuiltInFunction, IHaveFuturemud
{
	protected UnitType Type;

	public UnitFromTextFunction(IList<IFunction> parameters, IFuturemud gameworld, UnitType type)
		: base(parameters)
	{
		Gameworld = gameworld;
		Type = type;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override string ErrorMessage
	{
		get => ParameterFunctions.First().ErrorMessage;
		protected set { }
	}

	public IFuturemud Gameworld { get; protected set; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var result = Gameworld.UnitManager.GetBaseUnits((string)ParameterFunctions.First().Result.GetObject, Type,
			out var success);
		if (!success)
		{
			ErrorMessage = "The text " + (string)ParameterFunctions.First().Result.GetObject +
			               " is not a valid expression unit expression.";
			return StatementResult.Error;
		}

		Result = new NumberVariable(result);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"lengthfromtext",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Length)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"massfromtext",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Mass)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"fluidfromtext",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.FluidVolume)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"areafromtext",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Area)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"volumefromtext",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Volume)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tempfromtext",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new UnitFromTextFunction(pars, gameworld, UnitType.Temperature)
		));
	}
}
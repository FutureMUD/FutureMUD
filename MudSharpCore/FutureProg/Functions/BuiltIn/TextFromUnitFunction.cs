using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class TextFromUnitFunction : BuiltInFunction, IHaveFuturemud
{
	protected UnitType Type;

	public TextFromUnitFunction(IList<IFunction> parameters, IFuturemud gameworld, UnitType type)
		: base(parameters)
	{
		Gameworld = gameworld;
		Type = type;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Text;
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

		var result = Gameworld.UnitManager.Describe((double)(decimal)ParameterFunctions.First().Result.GetObject,
			Type, (string)ParameterFunctions.ElementAt(1).Result.GetObject);
		Result = new TextVariable(result);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"textfromlength",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Length)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"textfrommass",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Mass)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"textfromfluid",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.FluidVolume)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"textfromarea",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Area)
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"textfromvolume",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Volume)
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"textfromtemp",
			new[] { FutureProgVariableTypes.Number },
			(pars, gameworld) => new TextFromUnitFunction(pars, gameworld, UnitType.Temperature)
		));
	}
}
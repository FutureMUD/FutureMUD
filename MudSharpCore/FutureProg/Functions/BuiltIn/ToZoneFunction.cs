using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToZoneFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ToZoneFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Zone;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.Zones.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: _gameworld.Zones.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tozone",
			new[] { ProgVariableTypes.Number },
			(pars, gameworld) => new ToZoneFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.Zone
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tozone",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new ToZoneFunction(pars, gameworld),
			new List<string> { "name" },
			new List<string> { "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.Zone
		));
	}
}
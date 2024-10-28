using System;
using System.Collections.Generic;
using MudSharp.Commands.Modules;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToLocationFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;
	private readonly bool _useId;

	public ToLocationFunction(IList<IFunction> parameters, IFuturemud gameworld, bool useId)
		: base(parameters)
	{
		_gameworld = gameworld;
		_useId = useId;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Location;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (_useId)
		{
			Result = _gameworld.Cells.Get(Convert.ToInt64(ParameterFunctions[0].Result?.GetObject));
		}
		else
		{
			Result = RoomBuilderModule.LookupCell(_gameworld,
				ParameterFunctions[0].Result?.GetObject?.ToString() ?? "0");
		}

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tolocation",
			new[] { ProgVariableTypes.Number },
			(pars, gameworld) => new ToLocationFunction(pars, gameworld, true),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.Location
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tolocation",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new ToLocationFunction(pars, gameworld, false),
			new List<string> { "name" },
			new List<string> { "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.Location
		));
	}
}
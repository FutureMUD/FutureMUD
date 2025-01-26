using System.Collections.Generic;
using System.Linq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Echoes;

internal class SendTerrainFunction : BuiltInFunction
{
	protected SendTerrainFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		Gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	private IFuturemud Gameworld { get; }

	public bool FixedFormat { get; init; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result is not ITerrain target)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var textResult = ParameterFunctions[1].Result;
		if (textResult?.GetObject == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var text = textResult.GetObject.ToString().SubstituteANSIColour();

		var perceivables = new List<IPerceivable>();
		foreach (var parameter in ParameterFunctions.Skip(2))
		{
			if (parameter.Result is not IPerceivable perceivable)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			perceivables.Add(perceivable);
		}

		var output = FixedFormat
			? new EmoteOutput(new NoFormatEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()), flags: OutputFlags.IgnoreWatchers)
			: new EmoteOutput(new Emote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()), flags: OutputFlags.IgnoreWatchers);

		foreach (var cell in Gameworld.Cells)
		{
			if (cell.Terrain(null) == target)
			{
				cell.Handle(output);
			}
		}

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[] { ProgVariableTypes.Terrain, ProgVariableTypes.Text },
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrain",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld)
			)
		);


		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[] { ProgVariableTypes.Terrain, ProgVariableTypes.Text },
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainfixed",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainFunction(pars, gameworld) { FixedFormat = true }
			)
		);
	}
}
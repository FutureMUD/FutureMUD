using System.Collections.Generic;
using System.Linq;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;

namespace MudSharp.FutureProg.Functions.Echoes;

internal class SendTerrainLangFunction : BuiltInFunction
{
	protected SendTerrainLangFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		if (ParameterFunctions[2].Result?.GetObject is not ILanguage language)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[3].Result?.GetObject is not IAccent accent)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var perceivables = new List<IPerceivable>();
		foreach (var parameter in ParameterFunctions.Skip(4))
		{
			if (parameter.Result is not IPerceivable perceivable)
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			perceivables.Add(perceivable);
		}

		var output = new EmoteOutput(new FixedLanguageEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, language, accent, perceivables.ToArray()),
			flags: OutputFlags.IgnoreWatchers);
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
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendterrainlang",
				new[]
				{
					ProgVariableTypes.Terrain, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendTerrainLangFunction(pars, gameworld)
			)
		);
	}
}
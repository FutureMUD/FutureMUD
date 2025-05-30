﻿using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Echoes;

internal class SendLangFunction : BuiltInFunction
{
	protected SendLangFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		if (ParameterFunctions[0].Result is not IPerceiver target)
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

		if (ParameterFunctions[2].Result?.GetObject is not Language language)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[3].Result?.GetObject is not Accent accent)
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

		target.OutputHandler.Send(
			new EmoteOutput(new FixedLanguageEmote(text, target, language, accent, perceivables.ToArray())));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendlang",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Language,
					ProgVariableTypes.Accent, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendLangFunction(pars, gameworld)
			)
		);
	}
}
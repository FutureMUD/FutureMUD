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

internal class SendFunction : BuiltInFunction
{
	protected SendFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		target.OutputHandler.Handle(FixedFormat
			? new EmoteOutput(new NoFormatEmote(text, target, perceivables.ToArray()))
			: new EmoteOutput(new Emote(text, target, perceivables.ToArray())));

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[] { ProgVariableTypes.Perceiver, ProgVariableTypes.Text },
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"send",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld)
			)
		);


		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[] { ProgVariableTypes.Perceiver, ProgVariableTypes.Text },
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendfixed",
				new[]
				{
					ProgVariableTypes.Perceiver, ProgVariableTypes.Text,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendFunction(pars, gameworld) { FixedFormat = true }
			)
		);
	}
}
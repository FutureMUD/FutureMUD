using MudSharp.Construction;
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

internal class SendShardFunction : BuiltInFunction
{
	protected SendShardFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		if (ParameterFunctions[0].Result is not IShard target)
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
			if (!(parameter.Result is IPerceivable perceivable))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			perceivables.Add(perceivable);
		}

		target.Handle(FixedFormat
			? new EmoteOutput(new NoFormatEmote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()), flags: OutputFlags.IgnoreWatchers)
			: new EmoteOutput(new Emote(text, perceivables.ElementAtOrDefault(0) as IPerceiver, perceivables.ToArray()), flags: OutputFlags.IgnoreWatchers));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[] { ProgVariableTypes.Shard, ProgVariableTypes.Text },
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshard",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld)
			)
		);


		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[] { ProgVariableTypes.Shard, ProgVariableTypes.Text },
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"sendshardfixed",
				new[]
				{
					ProgVariableTypes.Shard, ProgVariableTypes.Text, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable,
					ProgVariableTypes.Perceivable
				},
				(pars, gameworld) => new SendShardFunction(pars, gameworld) { FixedFormat = true }
			)
		);
	}
}
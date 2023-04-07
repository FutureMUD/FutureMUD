using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Effects;

internal class AddHealingEffectFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private AddHealingEffectFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var perceivable = (IPerceivable)ParameterFunctions[0].Result.GetObject;
		if (perceivable == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var prog = ParameterFunctions[3].ReturnType.CompatibleWith(FutureProgVariableTypes.Number)
			? _gameworld.FutureProgs.Get(Convert.ToInt64(ParameterFunctions[3].Result?.GetObject ?? 0))
			: _gameworld.FutureProgs.GetByName((string)ParameterFunctions[3].Result?.GetObject ?? "");
		if (prog != null)
		{
			if (prog.ReturnType != FutureProgVariableTypes.Boolean ||
			    !prog.MatchesParameters(new[] { FutureProgVariableTypes.Character })
			   )
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}
		}

		var timespan = ParameterFunctions[4].Result?.GetObject as TimeSpan?;
		if (timespan == null)
		{
			timespan = TimeSpan.MinValue;
		}

		perceivable.AddEffect(
			new HealingRateEffect(perceivable, Convert.ToDouble(ParameterFunctions[1].Result?.GetObject ?? 0.0M),
				Convert.ToInt32(ParameterFunctions[2].Result?.GetObject ?? 0.0M), prog), timespan.Value);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addhealingeffect",
			new[]
			{
				FutureProgVariableTypes.Perceivable,
				FutureProgVariableTypes.Number,
				FutureProgVariableTypes.Number,
				FutureProgVariableTypes.Number,
				FutureProgVariableTypes.TimeSpan
			},
			(pars, gameworld) => new AddHealingEffectFunction(pars, gameworld),
			new List<string> { "Perceivable", "Multiplier", "Stages", "ProgId", "Duration" },
			new List<string>
			{
				"The perceivable to whom the healing effect is being added",
				"The healing multiplier for the effect",
				"The number of difficulty stages to stage the healing difficulty checks by",
				"The ID of the prog to whether the effect applies",
				"The duration of the effect"
			},
			"",
			"Effects",
			FutureProgVariableTypes.Boolean
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addhealingeffect",
			new[]
			{
				FutureProgVariableTypes.Perceivable,
				FutureProgVariableTypes.Number,
				FutureProgVariableTypes.Number,
				FutureProgVariableTypes.Text,
				FutureProgVariableTypes.TimeSpan
			},
			(pars, gameworld) => new AddHealingEffectFunction(pars, gameworld)
		));
	}
}
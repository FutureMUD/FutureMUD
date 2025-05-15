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

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Effect;
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
			Result = new NullVariable(ProgVariableTypes.Effect);
			return StatementResult.Normal;
		}

		var prog = ParameterFunctions[3].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? _gameworld.FutureProgs.Get(Convert.ToInt64(ParameterFunctions[3].Result?.GetObject ?? 0))
			: _gameworld.FutureProgs.GetByName((string)ParameterFunctions[3].Result?.GetObject ?? "");
		if (prog != null)
		{
			if (prog.ReturnType != ProgVariableTypes.Boolean ||
			    !prog.MatchesParameters(new[] { ProgVariableTypes.Character })
			   )
			{
				Result = new NullVariable(ProgVariableTypes.Effect);
				return StatementResult.Normal;
			}
		}

		var timespan = ParameterFunctions[4].Result?.GetObject as TimeSpan? ?? TimeSpan.MinValue;

		var effect = new HealingRateEffect(perceivable, Convert.ToDouble(ParameterFunctions[1].Result?.GetObject ?? 0.0M),
			Convert.ToInt32(ParameterFunctions[2].Result?.GetObject ?? 0.0M), prog);
		perceivable.AddEffect(effect, timespan);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addhealingeffect",
			new[]
			{
				ProgVariableTypes.Perceivable,
				ProgVariableTypes.Number,
				ProgVariableTypes.Number,
				ProgVariableTypes.Number,
				ProgVariableTypes.TimeSpan
			},
			(pars, gameworld) => new AddHealingEffectFunction(pars, gameworld),
			new List<string> { "Perceivable", "Multiplier", "Stages", "ProgId", "Duration" },
			new List<string>
			{
				"The perceivable to whom the healing effect is being added",
				"The healing multiplier for the effect",
				"The number of difficulty stages to stage the healing difficulty checks by. Negative is a bonus, positive is a penalty.",
				"The ID of the prog to whether the effect applies",
				"The duration of the effect"
			},
			"This function adds an effect to a perceivable (character or item) that changes the rate and difficulty of healing. Returns the effect it creates.",
			"Effects",
			ProgVariableTypes.Effect
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addhealingeffect",
			new[]
			{
				ProgVariableTypes.Perceivable,
				ProgVariableTypes.Number,
				ProgVariableTypes.Number,
				ProgVariableTypes.Text,
				ProgVariableTypes.TimeSpan
			},
			(pars, gameworld) => new AddHealingEffectFunction(pars, gameworld),
			new List<string> { "Perceivable", "Multiplier", "Stages", "ProgId", "Duration" },
			new List<string>
			{
				"The perceivable to whom the healing effect is being added",
				"The healing multiplier for the effect",
				"The number of difficulty stages to stage the healing difficulty checks by. Negative is a bonus, positive is a penalty.",
				"The name of the prog to whether the effect applies",
				"The duration of the effect"
			},
			"This function adds an effect to a perceivable (character or item) that changes the rate and difficulty of healing. Returns the effect it creates.",
			"Effects",
			ProgVariableTypes.Effect
		));
	}
}
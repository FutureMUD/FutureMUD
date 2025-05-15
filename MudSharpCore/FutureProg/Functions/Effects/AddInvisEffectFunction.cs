using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Functions.GameItem;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.FutureProg.Functions.Effects;

internal class AddInvisEffectFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private AddInvisEffectFunction(IList<IFunction> parameters, IFuturemud gameworld)
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

		var prog = (ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? _gameworld.FutureProgs.Get(Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0))
			: _gameworld.FutureProgs.GetByName((string)ParameterFunctions[1].Result?.GetObject ?? "")) ?? _gameworld.AlwaysTrueProg;

		if (prog.ReturnType != ProgVariableTypes.Boolean ||
		    !prog.MatchesParameters([ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable])
		   )
		{
			Result = new NullVariable(ProgVariableTypes.Effect);
			return StatementResult.Normal;
		}

		var effect = new Invis(perceivable, prog);
		perceivable.AddEffect(effect);
		Result = effect;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addinviseffect",
			new[]
			{
				ProgVariableTypes.Perceivable,
				ProgVariableTypes.Number
			},
			(pars, gameworld) => new AddInvisEffectFunction(pars, gameworld),
			[
				"Perceivable", 
				"Prog"
			],
			[
				"The perceivable to add the invisibility effect to",
				"The ID of a prog to control whether the effect applies. Parameters are perceivable,perceivable and returns boolean."
			],
			"This function adds an invisibility effect to a perceivable. Returns the effect it creates.",
			"Effects",
			ProgVariableTypes.Effect
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addinviseffect",
			new[]
			{
				ProgVariableTypes.Perceivable,
				ProgVariableTypes.Text
			},
			(pars, gameworld) => new AddInvisEffectFunction(pars, gameworld),
			[
				"Perceivable",
				"Prog"
			],
			[
				"The perceivable to add the invisibility effect to",
				"The name of a prog to control whether the effect applies. Parameters are perceivable,perceivable and returns boolean."
			],
			"This function adds an invisibility effect to a perceivable. Returns the effect it creates.",
			"Effects",
			ProgVariableTypes.Effect
		));
	}
}
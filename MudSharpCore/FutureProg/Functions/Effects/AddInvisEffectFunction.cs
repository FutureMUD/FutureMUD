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
		get => ProgVariableTypes.Boolean;
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

		var prog = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Number)
			? _gameworld.FutureProgs.Get(Convert.ToInt64(ParameterFunctions[1].Result?.GetObject ?? 0))
			: _gameworld.FutureProgs.GetByName((string)ParameterFunctions[1].Result?.GetObject ?? "");
		if (prog == null ||
		    prog.ReturnType != ProgVariableTypes.Boolean ||
		    !prog.MatchesParameters(new[] { ProgVariableTypes.Perceivable, ProgVariableTypes.Perceivable })
		   )
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		perceivable.AddEffect(new Invis(perceivable, prog));
		Result = new BooleanVariable(true);
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
			(pars, gameworld) => new AddInvisEffectFunction(pars, gameworld)
		));
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"addinviseffect",
			new[]
			{
				ProgVariableTypes.Perceivable,
				ProgVariableTypes.Text
			},
			(pars, gameworld) => new AddInvisEffectFunction(pars, gameworld)
		));
	}
}
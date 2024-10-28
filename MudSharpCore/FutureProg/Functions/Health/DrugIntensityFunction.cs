using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudSharp.FutureProg.Functions.Health;

internal class DrugIntensityFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private DrugIntensityFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Number;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var character = (ICharacter)ParameterFunctions[0].Result?.GetObject;
		if (character == null)
		{
			ErrorMessage = "Character was NULL in DrugIntensityFunction";
			return StatementResult.Error;
		}

		var drug = (IDrug)ParameterFunctions[1].Result?.GetObject;
		if (drug == null)
		{
			ErrorMessage = "Drug was NULL in DrugIntensityFunction";
			return StatementResult.Error;
		}

		Result = new NumberVariable(character.Body.ActiveDrugDosages.Where(x => x.Drug == drug).Sum(x => x.Grams) *
			0.001 * _gameworld.UnitManager.BaseWeightToKilograms / character.Weight);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"drugintensity",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Drug },
			(pars, gameworld) => new DrugIntensityFunction(pars, gameworld)
		));
	}
}
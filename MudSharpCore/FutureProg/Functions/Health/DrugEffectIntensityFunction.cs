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

internal class DrugEffectIntensityFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private DrugEffectIntensityFunction(IList<IFunction> parameters, IFuturemud gameworld)
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
			ErrorMessage = "Character was NULL in DrugEffectIntensityFunction";
			return StatementResult.Error;
		}

		var drug = (long?)(decimal?)ParameterFunctions[1].Result?.GetObject;
		if (drug == null)
		{
			ErrorMessage = "DrugEffect was NULL in DrugEffectIntensityFunction";
			return StatementResult.Error;
		}

		var drugtype = (DrugType)drug.Value;

		Result = new NumberVariable(
			character.Body.ActiveDrugDosages.Sum(x => x.Drug.IntensityForType(drugtype) * x.Grams) * 0.001 *
			_gameworld.UnitManager.BaseWeightToKilograms / character.Weight);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"drugeffectintensity",
			new[] { ProgVariableTypes.Character, ProgVariableTypes.Number },
			(pars, gameworld) => new DrugEffectIntensityFunction(pars, gameworld)
		));
	}
}
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudSharp.Character;
using MudSharp.CharacterCreation;

namespace MudSharp.FutureProg.Functions.Characteristics;

internal class SetCharacteristicRandomFunction : BuiltInFunction
{
	private IFuturemud _gameworld;

	private SetCharacteristicRandomFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set => base.ReturnType = value;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Number, FutureProgVariableTypes.Number,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "character", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The character whose characteristics you want to randomly set",
					"The ID number of the characteristic definition you want to use",
					"The ID number of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Text, FutureProgVariableTypes.Number,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "character", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The character whose characteristics you want to randomly set",
					"The name of the characteristic definition you want to use",
					"The ID number of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Number, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "character", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The character whose characteristics you want to randomly set",
					"The ID number of the characteristic definition you want to use",
					"The name of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "character", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The character whose characteristics you want to randomly set",
					"The name of the characteristic definition you want to use",
					"The name of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Item, FutureProgVariableTypes.Number, FutureProgVariableTypes.Number,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "item", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The item whose characteristics you want to randomly set",
					"The ID number of the characteristic definition you want to use",
					"The ID number of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Item, FutureProgVariableTypes.Text, FutureProgVariableTypes.Number,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "item", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The item whose characteristics you want to randomly set",
					"The name of the characteristic definition you want to use",
					"The ID number of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Item, FutureProgVariableTypes.Number, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "item", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The item whose characteristics you want to randomly set",
					"The ID number of the characteristic definition you want to use",
					"The name of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"setcharacteristicrandom",
				new[]
				{
					FutureProgVariableTypes.Item, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Boolean
				},
				(pars, gameworld) => new SetCharacteristicRandomFunction(pars, gameworld),
				new List<string> { "item", "definition", "profile", "forcenew" },
				new List<string>
				{
					"The item whose characteristics you want to randomly set",
					"The name of the characteristic definition you want to use",
					"The name of the characteristic profile that you want to use to give the range of possible values",
					"If true, excludes the current result from the outcome. I.e. the value must change. If false or if there is only 1 value to choose from on the profile, the result may still be the old value"
				},
				"Sets the intrinsic characteristic value for the characteristic definition on the supplied target to a random value selected from the profile you supply. Returns true if successful.",
				"Characteristics",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0]?.Result is not IHaveCharacteristics target)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var definition = ParameterFunctions[1].ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
			? _gameworld.Characteristics.GetByName(ParameterFunctions[1].Result?.GetObject as string ?? "")
			: _gameworld.Characteristics.Get((long)(ParameterFunctions[1].Result?.GetObject as decimal? ?? 0.0M));
		if (definition == null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var profile = ParameterFunctions[2].ReturnType.CompatibleWith(FutureProgVariableTypes.Text)
			? _gameworld.CharacteristicProfiles.GetByName(ParameterFunctions[2].Result?.GetObject as string ?? "")
			: _gameworld.CharacteristicProfiles.Get((long)(ParameterFunctions[2].Result?.GetObject as decimal? ??
			                                               0.0M));
		if (profile == null || !profile.IsProfileFor(definition))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		// Command can be set to force the characteristic that is selected to be a new one.              
		var forceNewCharacteristic = (bool?)ParameterFunctions[3].Result.GetObject ?? false;

		// If our specificed profile has 1 or less values, then we set the forceNewCharacteristic to false to avoid an infinite loop in our upcoming while
		if (profile.Values.Count() <= 1)
		{
			forceNewCharacteristic = false;
		}

		Func<ICharacteristicValue> getNewFunc;
		if (target is ICharacter tch)
		{
			getNewFunc = () => profile.GetRandomCharacteristic(tch);
		}
		else
		{
			getNewFunc = () => profile.GetRandomCharacteristic();
		}

		var newCharacteristic = getNewFunc();

		// If we're forcing a new characteristic, randomly get a new characteristic until we have one that doesn't match our existing characteristic.
		if (forceNewCharacteristic == true)
		{
			var currentCharacteristic = target.GetCharacteristic(definition, null);

			while (currentCharacteristic == newCharacteristic)
			{
				newCharacteristic = getNewFunc();
				;
			}
		}

		target.SetCharacteristic(definition, newCharacteristic);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
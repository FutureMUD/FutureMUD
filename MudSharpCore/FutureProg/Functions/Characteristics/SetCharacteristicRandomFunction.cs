using MudSharp.CharacterCreation;
using MudSharp.Form.Characteristics;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characteristics;

internal class SetCharacteristicRandomFunction : BuiltInFunction
{
    private IFuturemud _gameworld;

    private SetCharacteristicRandomFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
    {
        _gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => ProgVariableTypes.Boolean;
        protected set => base.ReturnType = value;
    }

    public static void RegisterFunctionCompiler()
    {
        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Number,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Text,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Number,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Number,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Item, ProgVariableTypes.Number, ProgVariableTypes.Text,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );

        FutureProg.RegisterBuiltInFunctionCompiler(
            new FunctionCompilerInformation(
                "setcharacteristicrandom",
                new[]
                {
                    ProgVariableTypes.Item, ProgVariableTypes.Text, ProgVariableTypes.Text,
                    ProgVariableTypes.Boolean
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
                ProgVariableTypes.Boolean
            )
        );
		foreach (var targetType in new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })
		foreach (var profileType in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
				"setcharacteristicrandom", [targetType, ProgVariableTypes.CharacteristicDefinition, profileType, ProgVariableTypes.Boolean],
				(pars, world) => new SetCharacteristicRandomFunction(pars, world),
				["target", "definition", "profile", "forcenew"],
				["The character or item to change.", "The resolved characteristic definition.", "The characteristic profile ID or name.", "Whether to prefer a different value."],
				"Sets an intrinsic characteristic from a compatible profile. Returns false if no valid value can be selected.",
				"Characteristics", ProgVariableTypes.Boolean));
		}

    }

    public override StatementResult Execute(IVariableSpace variables)
    {
        if (base.Execute(variables) == StatementResult.Error)
        {
            return StatementResult.Error;
        }

        if (ParameterFunctions[0]?.Result?.GetObject is not IHaveCharacteristics target)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        var definition = CharacteristicFunctionLookup.Definition(ParameterFunctions[1], _gameworld);
        if (definition == null)
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        ICharacteristicProfile profile = ParameterFunctions[2].ReturnType.CompatibleWith(ProgVariableTypes.Text)
            ? _gameworld.CharacteristicProfiles.GetByName(ParameterFunctions[2].Result?.GetObject as string ?? "")
            : _gameworld.CharacteristicProfiles.Get((long)(ParameterFunctions[2].Result?.GetObject as decimal? ??
                                                           0.0M));
        if (profile == null || !profile.IsProfileFor(definition))
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        // Command can be set to force the characteristic that is selected to be a new one.              
        bool forceNewCharacteristic = (bool?)ParameterFunctions[3].Result.GetObject ?? false;

        // If our specificed profile has 1 or less values, then we set the forceNewCharacteristic to false because there is no alternative value
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

        ICharacteristicValue newCharacteristic = getNewFunc();

        // Bound retries because character eligibility can reduce a multi-value profile to one possible result.
        if (forceNewCharacteristic == true)
        {
            ICharacteristicValue currentCharacteristic = target.GetCharacteristic(definition, null);

            for (var attempts = 0; currentCharacteristic == newCharacteristic && attempts < 100; attempts++)
            {
                newCharacteristic = getNewFunc();
            }

            if (currentCharacteristic == newCharacteristic)
            {
                Result = new BooleanVariable(false);
                return StatementResult.Normal;
            }
        }

        if (newCharacteristic is null || !definition.IsValue(newCharacteristic))
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        target.SetCharacteristic(definition, newCharacteristic);
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}
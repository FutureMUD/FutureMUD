using MudSharp.Form.Characteristics;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characteristics;

internal class SetCharacteristicFunction : BuiltInFunction
{
    private IFuturemud _gameworld;

    private SetCharacteristicFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
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
		foreach (var targetType in new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })
		foreach (var definitionType in CharacteristicFunctionLookup.DefinitionTypes)
		foreach (var valueType in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.CharacteristicValue })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
				"setcharacteristic", [targetType, definitionType, valueType],
				(pars, world) => new SetCharacteristicFunction(pars, world),
				["target", "definition", "value"],
				["The character or item to change.", "The characteristic definition, ID or name.", "The value, ID or name within that definition."],
				"Sets an intrinsic characteristic. Returns false if the target, definition or value is absent, or the value does not belong to the definition.",
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

        var value = ParameterFunctions[2].Result?.GetObject as ICharacteristicValue ??
            (ParameterFunctions[2].ReturnType.CompatibleWith(ProgVariableTypes.Text)
                ? _gameworld.CharacteristicValues.FirstOrDefault(x => definition.IsValue(x) &&
                    x.Name.EqualTo(ParameterFunctions[2].Result?.GetObject as string ?? ""))
                : ParameterFunctions[2].Result?.GetObject is decimal id
                    ? _gameworld.CharacteristicValues.Get((long)id)
                    : null);
        if (value == null || !definition.IsValue(value))
        {
            Result = new BooleanVariable(false);
            return StatementResult.Normal;
        }

        target.SetCharacteristic(definition, value);
        Result = new BooleanVariable(true);
        return StatementResult.Normal;
    }
}

using MudSharp.Form.Characteristics;
using MudSharp.CharacterCreation;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characteristics;

internal class CharacteristicValueFunction : BuiltInFunction
{
    private bool _returnIdOfCharacteristic;
    private IFuturemud _gameworld;

    private CharacteristicValueFunction(IList<IFunction> parameters, bool id, IFuturemud gameworld) : base(parameters)
    {
        _returnIdOfCharacteristic = id;
        _gameworld = gameworld;
    }

    public override ProgVariableTypes ReturnType
    {
        get => _returnIdOfCharacteristic ? ProgVariableTypes.Number : ProgVariableTypes.Text;
        protected set => base.ReturnType = value;
    }

	public static void RegisterFunctionCompiler()
	{
		foreach (var targetType in new[] { ProgVariableTypes.Character, ProgVariableTypes.Item })
		foreach (var definitionType in CharacteristicFunctionLookup.DefinitionTypes)
		foreach (var id in new[] { false, true })
		{
			var resultType = id ? ProgVariableTypes.Number : ProgVariableTypes.Text;
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
				id ? "characteristicid" : "characteristicvalue", [targetType, definitionType],
				(pars, world) => new CharacteristicValueFunction(pars, id, world),
				["target", "definition"], ["The character or item to query.", "The characteristic definition, ID or name."],
				id ? "Returns the intrinsic characteristic value ID, or zero if absent." : "Returns the intrinsic characteristic value name, or empty text if absent.",
				"Characteristics", resultType));
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
            Result = _returnIdOfCharacteristic ? (IProgVariable)new NumberVariable(0) : new TextVariable("");
            return StatementResult.Normal;
        }

        var definition = CharacteristicFunctionLookup.Definition(ParameterFunctions[1], _gameworld);
        if (definition == null)
        {
            Result = _returnIdOfCharacteristic ? (IProgVariable)new NumberVariable(0) : new TextVariable("");
            return StatementResult.Normal;
        }

        ICharacteristicValue value = target.GetCharacteristic(definition, null);
        Result = _returnIdOfCharacteristic
            ? (IProgVariable)new NumberVariable(value?.Id ?? 0L)
            : new TextVariable(value?.Name ?? "");
        return StatementResult.Normal;
    }
}

internal sealed class GetCharacteristicValueFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private GetCharacteristicValueFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.CharacteristicValue;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result?.GetObject;
		if (CharacteristicFunctionLookup.Definition(ParameterFunctions[1], _gameworld) is not { } definition)
		{
			Result = new NullVariable(ProgVariableTypes.CharacteristicValue);
			return StatementResult.Normal;
		}

		var value = target switch
		{
			IChargen chargen => chargen.SelectedCharacteristics.FirstOrDefault(x => x.Item1 == definition).Item2,
			IHaveCharacteristics characteristics => characteristics.GetCharacteristic(definition, null),
			_ => null
		};
		Result = value as IProgVariable ?? new NullVariable(ProgVariableTypes.CharacteristicValue);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		Register(ProgVariableTypes.Character, "The character whose characteristic value you want to retrieve.");
		Register(ProgVariableTypes.Item, "The item whose characteristic value you want to retrieve.");
		Register(ProgVariableTypes.Chargen, "The chargen whose selected characteristic value you want to retrieve.");
	}

	private static void Register(ProgVariableTypes targetType, string targetHelp)
	{
		foreach (var definitionType in CharacteristicFunctionLookup.DefinitionTypes)
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"getcharacteristicvalue",
			[targetType, definitionType],
			(pars, world) => new GetCharacteristicValueFunction(pars, world),
			["target", "definition"],
			[targetHelp, "The characteristic definition, ID or name to retrieve."],
			"Returns the selected characteristic value for a character or item, or null if it has no value for that definition.",
			"Characteristics",
			ProgVariableTypes.CharacteristicValue));
	}
}

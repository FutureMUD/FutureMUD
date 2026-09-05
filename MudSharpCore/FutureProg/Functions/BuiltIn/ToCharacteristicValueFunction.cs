#nullable enable

using MudSharp.Form.Characteristics;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class ToCharacteristicValueFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	private ToCharacteristicValueFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
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

		var identifier = ParameterFunctions[^1].Result?.GetObject;
		if (identifier is null)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		if (ParameterFunctions.Count == 1)
		{
			var globalResult = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
				? _gameworld.CharacteristicValues.GetByIdOrName((string)identifier)
				: _gameworld.CharacteristicValues.Get(Convert.ToInt64(identifier));
			Result = globalResult is not null ? globalResult : new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ICharacteristicDefinition definition)
		{
			Result = new NullVariable(ReturnType);
			return StatementResult.Normal;
		}

		var values = _gameworld.CharacteristicValues.Where(definition.IsValue);
		var result = ParameterFunctions[1].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? values.FirstOrDefault(x => x.Name.EqualTo((string)identifier))
			: values.Get((long)(decimal)identifier);
		Result = result is not null ? result : new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var name in new[] { "characteristicvalue", "tocharacteristicvalue" })
		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(name,
				[type], (pars, world) => new ToCharacteristicValueFunction(pars, world),
				[type == ProgVariableTypes.Number ? "id" : "name"],
				["The ID or name of the characteristic value to find."],
				"Returns a characteristic value by ID or name, or null.", "World Lookup", ProgVariableTypes.CharacteristicValue));
		}

		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation("tocharacteristicvalue",
				[ProgVariableTypes.CharacteristicDefinition, type], (pars, world) => new ToCharacteristicValueFunction(pars, world),
				["definition", "identifier"], ["The characteristic definition to search within, including inherited values.", "The ID or case-insensitive name to find within the definition."],
				"Returns a characteristic value within the supplied definition, or null if either is absent. Numeric arguments are IDs.",
				"Lookup", ProgVariableTypes.CharacteristicValue));
		}
	}
}

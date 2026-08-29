namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToSignedLanguageFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToSignedLanguageFunction(IList<IFunction> parameters, IFuturemud gameworld) : base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.SignedLanguage;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}
		Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.SignedLanguages.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: _gameworld.SignedLanguages.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		foreach (var type in new[] { ProgVariableTypes.Number, ProgVariableTypes.Text })
		{
			FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
				"tosignedlanguage", [type], (parameters, gameworld) => new ToSignedLanguageFunction(parameters, gameworld),
				[type == ProgVariableTypes.Number ? "id" : "name"],
				["The signed language ID or name to look up"],
				"Looks up a signed language independently from spoken languages.", "Lookup",
				ProgVariableTypes.SignedLanguage));
		}
	}
}

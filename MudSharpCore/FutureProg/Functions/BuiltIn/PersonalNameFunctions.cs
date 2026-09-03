using MudSharp.Character.Name;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class GetPersonalNameFunction : BuiltInFunction
{
	public GetPersonalNameFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.PersonalName;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var culture = ParameterFunctions[0].Result?.GetObject as INameCulture;
		var text = ParameterFunctions[1].Result?.GetObject?.ToString();
		var personalName = culture?.GetPersonalName(text, true);
		Result = personalName is null ? new NullVariable(ReturnType) : personalName;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"getpersonalname",
			[ProgVariableTypes.NameCulture, ProgVariableTypes.Text],
			(pars, _) => new GetPersonalNameFunction(pars),
			["nameculture", "name"],
			["The name culture whose validation and formatting rules apply", "The complete name text to validate and parse"],
			"Parses a complete name under a name culture and returns a personal name, or null when the text is invalid.",
			"Names",
			ProgVariableTypes.PersonalName
		));
	}
}

internal class RandomPersonalNameFunction : BuiltInFunction
{
	public RandomPersonalNameFunction(IList<IFunction> parameters)
		: base(parameters)
	{
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.PersonalName;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var profile = ParameterFunctions[0].Result?.GetObject as IRandomNameProfile;
		Result = profile?.IsReady == true
			? profile.GetRandomPersonalName(true)
			: new NullVariable(ReturnType);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"randompersonalname",
			[ProgVariableTypes.RandomNameProfile],
			(pars, _) => new RandomPersonalNameFunction(pars),
			["profile"],
			["The ready random-name profile from which to generate a name"],
			"Generates a personal name from a random-name profile, or returns null when the profile is not ready.",
			"Names",
			ProgVariableTypes.PersonalName
		));
	}
}

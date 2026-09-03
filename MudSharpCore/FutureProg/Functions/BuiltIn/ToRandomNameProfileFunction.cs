using MudSharp.Character.Name;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToRandomNameProfileFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToRandomNameProfileFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.RandomNameProfile;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		IRandomNameProfile result;
		if (ParameterFunctions.Count == 2)
		{
			var culture = ParameterFunctions[0].Result?.GetObject as INameCulture;
			var name = ParameterFunctions[1].Result?.GetObject?.ToString();
			result = culture is null || string.IsNullOrWhiteSpace(name)
				? null
				: _gameworld.RandomNameProfiles.FirstOrDefault(x => x.Culture == culture && x.Name.EqualTo(name));
		}
		else if (ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text))
		{
			var name = ParameterFunctions[0].Result?.GetObject?.ToString();
			result = string.IsNullOrWhiteSpace(name) ? null : _gameworld.RandomNameProfiles.GetByName(name);
		}
		else
		{
			result = _gameworld.RandomNameProfiles.Get((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0.0M));
		}

		Result = result is null ? new NullVariable(ReturnType) : result;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torandomnameprofile",
			[ProgVariableTypes.Number],
			(pars, gameworld) => new ToRandomNameProfileFunction(pars, gameworld),
			["id"],
			["The random-name profile ID to look up"],
			"Looks up a random-name profile by its ID, returning null if none exists.",
			"Names",
			ProgVariableTypes.RandomNameProfile
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torandomnameprofile",
			[ProgVariableTypes.Text],
			(pars, gameworld) => new ToRandomNameProfileFunction(pars, gameworld),
			["name"],
			["The random-name profile name to look up"],
			"Looks up the first random-name profile with this name, returning null if none exists.",
			"Names",
			ProgVariableTypes.RandomNameProfile
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"torandomnameprofile",
			[ProgVariableTypes.NameCulture, ProgVariableTypes.Text],
			(pars, gameworld) => new ToRandomNameProfileFunction(pars, gameworld),
			["nameculture", "name"],
			["The name culture that scopes the lookup", "The random-name profile name to look up"],
			"Looks up a random-name profile by name within a particular name culture, returning null if none exists.",
			"Names",
			ProgVariableTypes.RandomNameProfile
		));
	}
}

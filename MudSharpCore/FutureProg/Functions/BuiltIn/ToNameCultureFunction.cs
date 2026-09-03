using MudSharp.Character.Name;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToNameCultureFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	public ToNameCultureFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.NameCulture;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		INameCulture result;
		if (ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text))
		{
			var name = ParameterFunctions[0].Result?.GetObject?.ToString();
			result = string.IsNullOrWhiteSpace(name) ? null : _gameworld.NameCultures.GetByName(name);
		}
		else
		{
			result = _gameworld.NameCultures.Get((long)(decimal)(ParameterFunctions[0].Result?.GetObject ?? 0.0M));
		}

		Result = result is null ? new NullVariable(ReturnType) : result;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tonameculture",
			[ProgVariableTypes.Number],
			(pars, gameworld) => new ToNameCultureFunction(pars, gameworld),
			["id"],
			["The name culture ID to look up"],
			"Looks up a name culture by its ID, returning null if none exists.",
			"Names",
			ProgVariableTypes.NameCulture
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"tonameculture",
			[ProgVariableTypes.Text],
			(pars, gameworld) => new ToNameCultureFunction(pars, gameworld),
			["name"],
			["The name culture name to look up"],
			"Looks up a name culture by name, returning null if none exists.",
			"Names",
			ProgVariableTypes.NameCulture
		));
	}
}

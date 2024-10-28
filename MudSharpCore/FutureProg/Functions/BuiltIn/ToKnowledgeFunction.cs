using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class ToKnowledgeFunction : BuiltInFunction
{
	private readonly IFuturemud _gameworld;

	protected ToKnowledgeFunction(IList<IFunction> parameters, IFuturemud gameworld)
		: base(parameters)
	{
		_gameworld = gameworld;
	}

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Knowledge;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		Result = ParameterFunctions[0].ReturnType.CompatibleWith(ProgVariableTypes.Text)
			? _gameworld.Knowledges.Get((string)ParameterFunctions[0].Result.GetObject).FirstOrDefault()
			: _gameworld.Knowledges.Get((long)(decimal)ParameterFunctions[0].Result.GetObject);

		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toknowledge",
			new[] { ProgVariableTypes.Number },
			(pars, gameworld) => new ToKnowledgeFunction(pars, gameworld),
			new List<string> { "id" },
			new List<string> { "The ID to look up" },
			"Converts an ID number into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.Knowledge
		));

		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"toknowledge",
			new[] { ProgVariableTypes.Text },
			(pars, gameworld) => new ToKnowledgeFunction(pars, gameworld),
			new List<string> { "name" },
			new List<string> { "The name to look up" },
			"Converts a name into the specified type, if one exists",
			"Lookup",
			ProgVariableTypes.Knowledge
		));
	}
}
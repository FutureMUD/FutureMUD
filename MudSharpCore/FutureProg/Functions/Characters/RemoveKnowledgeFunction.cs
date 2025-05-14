using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Knowledge;

namespace MudSharp.FutureProg.Functions.Characters;

internal class RemoveKnowledgeFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"removeknowledge",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Knowledge }, // the parameters the function takes
				(pars, gameworld) => new RemoveKnowledgeFunction(pars, gameworld),
				new List<string> {
					"who",
					"knowledge"
				}, // parameter names
				new List<string> {
					"The character to remove the knowledge from",
					"The knowledge to remove from the character"
				}, // parameter help text
				"Takes a knowledge from a character. Returns true if taken, false if they didn't have it.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected RemoveKnowledgeFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.Boolean; }
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result as ICharacter;
		if (target == null)
		{
			ErrorMessage = "The target parameter in RemoveKnowledge returned null";
			return StatementResult.Error;
		}

		var knowledge = (IKnowledge)ParameterFunctions[1].Result;
		if (knowledge == null)
		{
			ErrorMessage = "The knowledge parameter in RemoveKnowledge returned null";
			return StatementResult.Error;
		}

		if (!target.Knowledges.Contains(knowledge))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		target.RemoveKnowledge(knowledge);
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
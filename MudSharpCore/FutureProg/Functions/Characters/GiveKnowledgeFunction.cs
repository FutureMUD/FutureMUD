using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Knowledge;

namespace MudSharp.FutureProg.Functions.Characters;

internal class GiveKnowledgeFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"giveknowledge",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Knowledge }, // the parameters the function takes
				(pars, gameworld) => new GiveKnowledgeFunction(pars, gameworld),
				new List<string> {
					"who",
					"knowledge"
				}, // parameter names
				new List<string> {
					"The character to give the knowledge to",
					"The knowledge to give the character"
				}, // parameter help text
				"Gives a knowledge to a character. Returns true if given, false if already had it.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected GiveKnowledgeFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.Number; }
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
			ErrorMessage = "The target parameter in GiveKnowledge returned null";
			return StatementResult.Error;
		}

		var knowledge = (IKnowledge)ParameterFunctions[1].Result;
		if (knowledge == null)
		{
			ErrorMessage = "The knowledge parameter in GiveKnowledge returned null";
			return StatementResult.Error;
		}

		if (target.Knowledges.Contains(knowledge))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		target.AddKnowledge(new CharacterKnowledge(target, knowledge, "Acquired"));
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
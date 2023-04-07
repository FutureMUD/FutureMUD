using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Character;
using MudSharp.FutureProg.Variables;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.Combat;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal class StartCombat : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }

	#region Static Initialisation

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"startcombat",
				new[]
				{
					FutureProgVariableTypes.Text, FutureProgVariableTypes.Text, FutureProgVariableTypes.Text,
					FutureProgVariableTypes.Boolean, FutureProgVariableTypes.Character,
					FutureProgVariableTypes.Character, FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal,
					FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal,
					FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal,
					FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal,
					FutureProgVariableTypes.Text | FutureProgVariableTypes.Literal
				},
				(pars, gameworld) => new StartCombat(pars, gameworld),
				new List<string>
				{
					"reference",
					"description",
					"gerund",
					"friendly",
					"combatant1",
					"combatant2",
					"joinprog",
					"leaveprog",
					"endprog",
					"moveprog",
					"hitprog"
				},
				new List<string>
				{
					"A unique reference that is passed to each of the progs when executed",
					"The description of the combat that appears in COMBAT STATUS",
					"The gerund used in combat descriptions, e.g. fighting, sparring, boxing, etc.",
					"Whether this combat counts as friendly and uses the spar rules or not",
					"The first combatant",
					"The second combatant",
					"A prog to execute when someone joins the combat. It must accept a character (the joiner) and a text parameter (the reference).",
					"A prog to execute when someone leaves the combat. It must accept a character (the leaver) and a text parameter (the reference).",
					"A prog to execute when the combat ends. It must accept a text parameter (the reference).",
					"A prog to execute when someone makes a move in combat. It must accept a character (the attacker), another character (the target) and a text parameter (the reference).",
					"A prog to execute when someone hits someone in combat. It must accept a character (the attacker), another character (the target) and a text parameter (the reference)."
				},
				"Starts a combat that executes progs when various things happen.",
				"Combat",
				FutureProgVariableTypes.Boolean
			)
		);
	}

	#endregion

	#region Constructors

	protected StartCombat(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
	{
		Gameworld = gameworld;
	}

	#endregion

	public override FutureProgVariableTypes ReturnType
	{
		get => FutureProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var reference = ParameterFunctions[0].Result?.GetObject?.ToString() ?? "";
		var description = ParameterFunctions[1].Result?.GetObject?.ToString() ??
		                  "This is a fight to the death, with no rules or holds barred";
		var gerund = ParameterFunctions[2].Result?.GetObject?.ToString() ?? "fighting";
		var friendly = Convert.ToBoolean(ParameterFunctions[3].Result?.GetObject ?? false);
		var fighter1 = (ICharacter)ParameterFunctions[4].Result?.GetObject;
		var fighter2 = (ICharacter)ParameterFunctions[5].Result?.GetObject;
		var progname = ParameterFunctions[6].Result?.GetObject?.ToString();
		var joinprog = Gameworld.FutureProgs.GetByName(progname ?? "");
		progname = ParameterFunctions[7].Result?.GetObject?.ToString();
		var leaveprog = Gameworld.FutureProgs.GetByName(progname ?? "");
		progname = ParameterFunctions[8].Result?.GetObject?.ToString();
		var endprog = Gameworld.FutureProgs.GetByName(progname ?? "");
		progname = ParameterFunctions[9].Result?.GetObject?.ToString();
		var moveprog = Gameworld.FutureProgs.GetByName(progname ?? "");
		progname = ParameterFunctions[10].Result?.GetObject?.ToString();
		var hitprog = Gameworld.FutureProgs.GetByName(progname ?? "");

		if (
			joinprog?.MatchesParameters(new List<FutureProgVariableTypes>
				{ FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }) == false ||
			leaveprog?.MatchesParameters(new List<FutureProgVariableTypes>
				{ FutureProgVariableTypes.Character, FutureProgVariableTypes.Text }) == false ||
			endprog?.MatchesParameters(new List<FutureProgVariableTypes> { FutureProgVariableTypes.Text }) == false ||
			moveprog?.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Text
			}) == false ||
			hitprog?.MatchesParameters(new List<FutureProgVariableTypes>
			{
				FutureProgVariableTypes.Character, FutureProgVariableTypes.Character, FutureProgVariableTypes.Text
			}) == false
		)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (fighter1 == null || fighter2 == null || fighter1.Combat != null || fighter2.Combat != null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var combat = new ProgCombat(description, gerund, reference, friendly, joinprog, leaveprog, endprog, moveprog,
			hitprog);
		combat.JoinCombat(fighter1, RPG.Checks.Difficulty.Automatic);
		combat.JoinCombat(fighter2, RPG.Checks.Difficulty.Automatic);

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}
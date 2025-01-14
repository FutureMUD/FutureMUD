using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp.FutureProg.Functions.Health;
internal class WoundFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Wound".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Perceiver,
					ProgVariableTypes.Text,
					ProgVariableTypes.Text
				}, // the parameters the function takes
				(pars, gameworld) => new WoundFunction(pars, gameworld),
			new List<string>
			{
				"thing",
				"type",
				"formula"
			}, // parameter names
				new List<string>
				{
					"The thing to be wounded",
					"The damage type to be applied",
					"The dice formula of how much damage to do"
				}, // parameter help text
				"Creates a wound on a target item or character", // help text for the function,

				"Health", // the category to which this function belongs,

				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Wound".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Perceiver,
					ProgVariableTypes.Text,
					ProgVariableTypes.Number
				}, // the parameters the function takes
				(pars, gameworld) => new WoundFunction(pars, gameworld),
				new List<string>
				{
					"thing",
					"type",
					"amount"
				}, // parameter names
				new List<string>
				{
					"The thing to be wounded",
					"The damage type to be applied",
					"The amount of damage to do"
				}, // parameter help text
				"Creates a wound on a target item or character", // help text for the function,

				"Health", // the category to which this function belongs,

				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Wound".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Text,
					ProgVariableTypes.Text,
					ProgVariableTypes.Text
				}, // the parameters the function takes
				(pars, gameworld) => new WoundFunction(pars, gameworld),
				new List<string>
				{
					"thing",
					"type",
					"formula",
					"bodypart"
				}, // parameter names
				new List<string>
				{
					"The thing to be wounded",
					"The damage type to be applied",
					"The dice formula of how much damage to do",
					"The bodypart to damage"
				}, // parameter help text
				"Creates a wound on a target character", // help text for the function,

				"Health", // the category to which this function belongs,

				ProgVariableTypes.Boolean // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"Wound".ToLowerInvariant(),
				new[]
				{
					ProgVariableTypes.Character,
					ProgVariableTypes.Text,
					ProgVariableTypes.Number,
					ProgVariableTypes.Text
				}, // the parameters the function takes
				(pars, gameworld) => new WoundFunction(pars, gameworld),
				new List<string>
				{
					"thing",
					"type",
					"amount",
					"bodypart"
				}, // parameter names
				new List<string>
				{
					"The thing to be wounded",
					"The damage type to be applied",
					"The amount of damage to do",
					"The bodypart to damage"
				}, // parameter help text
				"Creates a wound on a target character", // help text for the function,

				"Health", // the category to which this function belongs,

				ProgVariableTypes.Boolean // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected WoundFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld) : base(parameterFunctions)
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

		var thing = ParameterFunctions[0].Result as IMortalPerceiver;
		if (thing is null)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		if (!(ParameterFunctions[1].Result?.GetObject?.ToString() ?? "").TryParseEnum(out DamageType type))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		decimal damage;
		if (ParameterFunctions[2].ReturnType.CompatibleWith(ProgVariableTypes.Number))
		{
			damage = (decimal?)ParameterFunctions[2].Result?.GetObject ?? 0.0M;
		}
		else
		{
			var expr = ParameterFunctions[2].Result?.GetObject?.ToString() ?? "";
			if (!Dice.IsDiceExpression(expr))
			{
				Result = new BooleanVariable(false);
				return StatementResult.Normal;
			}

			damage = Dice.Roll(expr);
		}

		IBodypart part = null;

		if (thing is IHaveABody ihb)
		{
			if (ParameterFunctions.Count > 3)
			{
				var partText = ParameterFunctions[3].Result?.GetObject?.ToString() ?? "";
				part = ihb.Body.Bodyparts.GetByIdOrName(partText);
			}

			part ??= ihb.Body.RandomBodypart;
		}
			

		thing.SufferDamage(new Damage
		{
			DamageType = type,
			DamageAmount = (double)damage,
			PainAmount = (double)damage,
			ShockAmount = 0,
			StunAmount = (double)damage,
			Bodypart = part,
			ActorOrigin = null,
			ToolOrigin = null,
			LodgableItem = null,
			AngleOfIncidentRadians = Math.Tau,
			PenetrationOutcome = Outcome.None
		});

		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}
}

using System;
using System.Collections.Generic;
using System.Configuration;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal class TraitBonusFunction : BuiltInFunction
{
	public IFuturemud Gameworld { get; set; }
	#region Static Initialisation
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"traitbonus",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Trait, ProgVariableTypes.Number, ProgVariableTypes.Text }, // the parameters the function takes
				(pars, gameworld) => new TraitBonusFunction(pars, gameworld, false),
				new List<string> {
					"who",
					"trait",
					"bonus",
					"reference"
				}, // parameter names
				new List<string> {
					"The character who has the trait",
					"The trait to give the bonus to",
					"The bonus to give to the trait",
					"A reference text to group similar bonuses (will overwrite anything with the same)"
				}, // parameter help text
				"Adds an effect to a character that gives a bonus to a trait. You also supply a reference bit of text. Each character can only have one effect with a specific reference, so you can use this to ensure that bonuses don't get stacked. You also use it to remove the specific bonus later. Returns the effect that it adds.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Effect // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"traitbonus",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Trait, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text }, // the parameters the function takes
				(pars, gameworld) => new TraitBonusFunction(pars, gameworld, false),
				new List<string> {
					"who",
					"trait",
					"bonus",
					"reference",
					"prog"
				}, // parameter names
				new List<string> {
					"The character who has the trait",
					"The trait to give the bonus to",
					"The bonus to give to the trait",
					"A reference text to group similar bonuses (will overwrite anything with the same)",
					"The name of a prog that controls if the bonus applies"
				}, // parameter help text
				"Adds an effect to a character that gives a bonus to a trait. You also supply a reference bit of text. Each character can only have one effect with a specific reference, so you can use this to ensure that bonuses don't get stacked. You also use it to remove the specific bonus later by setting the trait/reference combination to zero bonus. Returns the effect that it adds.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Effect // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"traitbonus",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Trait, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Number }, // the parameters the function takes
				(pars, gameworld) => new TraitBonusFunction(pars, gameworld, false),
				new List<string> {
					"who",
					"trait",
					"bonus",
					"reference",
					"prog"
				}, // parameter names
				new List<string> {
					"The character who has the trait",
					"The trait to give the bonus to",
					"The bonus to give to the trait",
					"A reference text to group similar bonuses (will overwrite anything with the same)",
					"The id of a prog that controls if the bonus applies"
				}, // parameter help text
				"Adds an effect to a character that gives a bonus to a trait. You also supply a reference bit of text. Each character can only have one effect with a specific reference, so you can use this to ensure that bonuses don't get stacked. You also use it to remove the specific bonus later. Returns the effect that it adds.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Effect // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"temptraitbonus",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Trait, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.TimeSpan }, // the parameters the function takes
				(pars, gameworld) => new TraitBonusFunction(pars, gameworld, true),
				new List<string> {
					"who",
					"trait",
					"bonus",
					"reference",
					"duration"
				}, // parameter names
				new List<string> {
					"The character who has the trait",
					"The trait to give the bonus to",
					"The bonus to give to the trait",
					"A reference text to group similar bonuses (will overwrite anything with the same)",
					"The time for this effect to apply before expiring"
				}, // parameter help text
				"Adds an effect to a character that gives a bonus to a trait. You also supply a reference bit of text. Each character can only have one effect with a specific reference, so you can use this to ensure that bonuses don't get stacked. You also use it to remove the specific bonus later. Returns the effect that it adds.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Effect // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"temptraitbonus",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Trait, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.TimeSpan }, // the parameters the function takes
				(pars, gameworld) => new TraitBonusFunction(pars, gameworld, true),
				new List<string> {
					"who",
					"trait",
					"bonus",
					"reference",
					"prog",
					"duration"
				}, // parameter names
				new List<string> {
					"The character who has the trait",
					"The trait to give the bonus to",
					"The bonus to give to the trait",
					"A reference text to group similar bonuses (will overwrite anything with the same)",
					"The name of a prog that controls if the bonus applies",
					"The time for this effect to apply before expiring"
				}, // parameter help text
				"Adds an effect to a character that gives a bonus to a trait. You also supply a reference bit of text. Each character can only have one effect with a specific reference, so you can use this to ensure that bonuses don't get stacked. You also use it to remove the specific bonus later. Returns the effect that it adds.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Effect // the return type of the function
			)
		);

		FutureProg.RegisterBuiltInFunctionCompiler(
			new FunctionCompilerInformation(
				"temptraitbonus",
				new[] { ProgVariableTypes.Character, ProgVariableTypes.Trait, ProgVariableTypes.Number, ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.TimeSpan }, // the parameters the function takes
				(pars, gameworld) => new TraitBonusFunction(pars, gameworld, true),
				new List<string> {
					"who",
					"trait",
					"bonus",
					"reference",
					"prog",
					"duration"
				}, // parameter names
				new List<string> {
					"The character who has the trait",
					"The trait to give the bonus to",
					"The bonus to give to the trait",
					"A reference text to group similar bonuses (will overwrite anything with the same)",
					"The id of a prog that controls if the bonus applies",
					"The time for this effect to apply before expiring"
				}, // parameter help text
				"Adds an effect to a character that gives a bonus to a trait. You also supply a reference bit of text. Each character can only have one effect with a specific reference, so you can use this to ensure that bonuses don't get stacked. You also use it to remove the specific bonus later. Returns the effect that it adds.", // help text for the function,
				"Character",// the category to which this function belongs,
				ProgVariableTypes.Effect // the return type of the function
			)
		);
	}
	#endregion

	#region Constructors
	protected TraitBonusFunction(IList<IFunction> parameterFunctions, IFuturemud gameworld, bool useDuration) : base(parameterFunctions)
	{
		Gameworld = gameworld;
		UseDuration = useDuration;
	}
	#endregion

	public override ProgVariableTypes ReturnType
	{
		get { return ProgVariableTypes.Effect; }
		protected set { }
	}

	public bool UseDuration { get; }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var target = ParameterFunctions[0].Result as ICharacter;
		if (target == null)
		{
			ErrorMessage = "The target parameter in TraitBonusFunction returned null";
			return StatementResult.Error;
		}

		var trait = (ITraitDefinition)ParameterFunctions[1].Result;
		if (trait == null)
		{
			ErrorMessage = "The trait parameter in TraitBonusFunction returned null";
			return StatementResult.Error;
		}

		var dBonus = (decimal)(ParameterFunctions[2].Result?.GetObject ?? 0.0M);
		var reference = (string)(ParameterFunctions[3].Result?.GetObject ?? "");

		if (dBonus == 0.0M)
		{
			target.RemoveAllEffects<ProgTraitBonusEffect>(x => x.OriginalReference.EqualTo(reference), true);
			Result = new NullVariable(ProgVariableTypes.Effect);
			return StatementResult.Normal;
		}

		IFutureProg? filterProg = null;
		var duration = TimeSpan.Zero;

		if (UseDuration)
		{
			if (ParameterFunctions[4].ReturnType.CompatibleWith(ProgVariableTypes.TimeSpan))
			{
				duration = (TimeSpan)(ParameterFunctions[4].Result?.GetObject ?? TimeSpan.Zero);
			}
			else
			{
				duration = (TimeSpan)(ParameterFunctions[5].Result?.GetObject ?? TimeSpan.Zero);
				if (ParameterFunctions[4].ReturnType.CompatibleWith(ProgVariableTypes.Text))
				{
					filterProg = Gameworld.FutureProgs.GetByName((string)(ParameterFunctions[4].Result?.GetObject ?? ""));
				}
				else
				{
					filterProg = Gameworld.FutureProgs.Get((long)(decimal)(ParameterFunctions[4].Result?.GetObject ?? 0.0M));
				}
			}
		}
		else
		{
			if (ParameterFunctions.Count > 4)
			{
				if (ParameterFunctions[4].ReturnType.CompatibleWith(ProgVariableTypes.Text))
				{
					filterProg = Gameworld.FutureProgs.GetByName((string)(ParameterFunctions[4].Result?.GetObject ?? ""));
				}
				else
				{
					filterProg = Gameworld.FutureProgs.Get((long)(decimal)(ParameterFunctions[4].Result?.GetObject ?? 0.0M));
				}
			}
		}

		if (filterProg is not null)
		{
			if (!filterProg.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
			{
				filterProg = null;
			}
			else if (!filterProg.MatchesParameters([ProgVariableTypes.Character]))
			{
				filterProg = null;
			}
		}

		if (duration == TimeSpan.Zero)
		{
			target.RemoveAllEffects<ProgTraitBonusEffect>(x => x.OriginalReference.EqualTo(reference), true);
			var effect = new ProgTraitBonusEffect(target, trait, (double)dBonus, reference, filterProg);
			target.AddEffect(effect);
			Result = effect;
		}
		else
		{
			target.RemoveAllEffects<ProgTraitBonusEffect>(x => x.OriginalReference.EqualTo(reference), true);
			var effect = new ProgTraitBonusEffect(target, trait, (double)dBonus, reference, filterProg);
			target.AddEffect(effect, duration);
			Result = effect;
		} 
		
		return StatementResult.Normal;
	}
}
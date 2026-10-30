#nullable enable

using MudSharp.Body.Needs;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class FulfilNeedsFunction : BuiltInFunction
{
	private FulfilNeedsFunction(IList<IFunction> parameters) : base(parameters) { }

	public override ProgVariableTypes ReturnType
	{
		get => ProgVariableTypes.Boolean;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		if (ParameterFunctions[0].Result?.GetObject is not ICharacter character ||
		    ParameterFunctions.Skip(1).Any(x => x.Result?.GetObject is not decimal value || !double.IsFinite((double)value)))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var foodHours = (double)(decimal)ParameterFunctions[1].Result!.GetObject;
		var thirstHours = (double)(decimal)ParameterFunctions[2].Result!.GetObject;
		var waterLitres = (double)(decimal)ParameterFunctions[3].Result!.GetObject;
		character.Body.FulfilNeeds(new NeedFulfiller
		{
			SatiationPoints = foodHours,
			ThirstPoints = thirstHours,
			WaterLitres = waterLitres
		});
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			"fulfilneeds",
			[ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Number, ProgVariableTypes.Number],
			(pars, _) => new FulfilNeedsFunction(pars),
			["character", "foodhours", "drinkhours", "waterlitres"],
			["The character whose needs to fulfil.", "Food satiation hours to add.", "Drink satiation hours to add.", "Water litres to add."],
			"Adds finite food, drink, and water amounts through the character's native needs model. NoNeeds models naturally ignore the request.",
			"Character",
			ProgVariableTypes.Boolean));
	}
}

internal sealed class CharacterHungerStatusFunction : BuiltInFunction
{
	private CharacterHungerStatusFunction(IList<IFunction> parameters) : base(parameters) { }
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = ParameterFunctions[0].Result?.GetObject is ICharacter character
			? new TextVariable((character.NeedsModel.Status & NeedsResult.HungerOnly).Describe())
			: new TextVariable(string.Empty);
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"hungerstatus", [ProgVariableTypes.Character], (pars, _) => new CharacterHungerStatusFunction(pars),
		["character"], ["The character to query."], "Returns the character's current hunger category.", "Character", ProgVariableTypes.Text));
}

internal sealed class CharacterThirstStatusFunction : BuiltInFunction
{
	private CharacterThirstStatusFunction(IList<IFunction> parameters) : base(parameters) { }
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Text; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		Result = ParameterFunctions[0].Result?.GetObject is ICharacter character
			? new TextVariable((character.NeedsModel.Status & NeedsResult.ThirstOnly).Describe())
			: new TextVariable(string.Empty);
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"thirststatus", [ProgVariableTypes.Character], (pars, _) => new CharacterThirstStatusFunction(pars),
		["character"], ["The character to query."], "Returns the character's current thirst category.", "Character", ProgVariableTypes.Text));
}

internal sealed class SatisfyNeedsFunction : BuiltInFunction
{
	internal SatisfyNeedsFunction(IList<IFunction> parameters) : base(parameters) { }
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		if (ParameterFunctions[0].Result?.GetObject is not ICharacter character ||
		    ParameterFunctions[1].Result?.GetObject is not bool food ||
		    ParameterFunctions[2].Result?.GetObject is not bool drink)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var model = character.NeedsModel;
		if (model is NoNeedsModel)
		{
			Result = new BooleanVariable(true);
			return StatementResult.Normal;
		}

		if (model is not ChangingNeedsModelBase changingModel)
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var (hungerMultiplier, thirstMultiplier, _) = changingModel.GetFulfilmentMultipliers();
		var foodTarget = ChangingNeedsModelBase.GetEffectiveFoodSatiationLimit(character.Race.MaximumFoodSatiatedHours) * 0.5;
		var drinkTarget = ChangingNeedsModelBase.GetEffectiveDrinkSatiationLimit(character.Race.MaximumDrinkSatiatedHours) * 0.5;
		if (!TryGetSatisfactionAddition(food, model.FoodSatiatedHours, foodTarget, hungerMultiplier, out var foodHours) ||
		    !TryGetSatisfactionAddition(drink, model.DrinkSatiatedHours, drinkTarget, thirstMultiplier, out var drinkHours))
		{
			Result = new BooleanVariable(false);
			return StatementResult.Normal;
		}

		var waterLitres = drink
			? Math.Max(0.0, character.Body.CurrentBloodVolumeLitres / 12.0 - model.WaterLitres)
			: 0.0;
		character.Body.FulfilNeeds(new NeedFulfiller
		{
			SatiationPoints = foodHours,
			ThirstPoints = drinkHours,
			WaterLitres = waterLitres
		});
		Result = new BooleanVariable(true);
		return StatementResult.Normal;
	}

	internal static bool TryGetSatisfactionAddition(bool requested, double current, double target,
		double fulfilmentMultiplier, out double addition)
	{
		addition = 0.0;
		if (!requested)
		{
			return true;
		}

		var deficit = Math.Max(0.0, target - current);
		if (deficit <= 0.0)
		{
			return true;
		}

		if (!double.IsFinite(fulfilmentMultiplier) || fulfilmentMultiplier <= 0.0)
		{
			return false;
		}

		addition = deficit / fulfilmentMultiplier;
		return double.IsFinite(addition) && addition > 0.0;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
		"satisfyneeds", [ProgVariableTypes.Character, ProgVariableTypes.Boolean, ProgVariableTypes.Boolean],
		(pars, _) => new SatisfyNeedsFunction(pars), ["character", "food", "drink"],
		["The character to restore.", "Whether to restore hunger to Full.", "Whether to restore thirst to Not Thirsty."],
		"Restores selected needs to the comfortable 50 percent category thresholds using the character's native needs model and race capacities. A drink also restores water to half of the body's native water capacity. Already satisfied needs are unchanged; NoNeeds models are a successful no-op.",
		"Character", ProgVariableTypes.Boolean));
}

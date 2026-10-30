#nullable enable

using MudSharp.Body;
using MudSharp.Body.Needs;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Characters;

internal sealed class PhysicalStateFunction(IList<IFunction> parameters, string operation) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => operation == "drunkennesslevel" ? ProgVariableTypes.Number : ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var actor = ParameterFunctions[0].Result?.GetObject as ICharacter;
		if (operation == "drunkennesslevel")
		{
			var level = (actor?.NeedsModel.Status & NeedsResult.DrunkOnly) switch
			{
				NeedsResult.Buzzed => 1, NeedsResult.Tipsy => 2, NeedsResult.Drunk => 3,
				NeedsResult.VeryDrunk => 4, NeedsResult.BlackoutDrunk or NeedsResult.Paralytic => 5, _ => 0
			};
			Result = new NumberVariable(level); return StatementResult.Normal;
		}
		if (operation == "spendstamina")
		{
			var amount = (double)(decimal)(ParameterFunctions[1].Result?.GetObject ?? -1m);
			var allowed = actor?.Body is { } body && double.IsFinite(amount) && amount >= 0 && body.CanSpendStamina(amount);
			if (allowed) actor!.Body.SpendStamina(amount);
			Result = new BooleanVariable(allowed); return StatementResult.Normal;
		}
		var item = ParameterFunctions[1].Result?.GetObject as MudSharp.GameItems.IGameItem;
		var alignment = item is null ? Alignment.Irrelevant : actor?.Body.HoldOrWieldLocFor(item)?.Alignment.LeftRightOnly() ?? Alignment.Irrelevant;
		Result = new BooleanVariable(actor is not null && (alignment is Alignment.Left or Alignment.Right) && alignment != actor.Handedness.LeftRightOnly());
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler()
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new("drunkennesslevel", [ProgVariableTypes.Character],
			(p, _) => new PhysicalStateFunction(p, "drunkennesslevel"), ["character"], ["The character."],
			"Returns native intoxication severity 0 (sober) through 5 (blackout/paralytic).", "Character", ProgVariableTypes.Number));
		FutureProg.RegisterBuiltInFunctionCompiler(new("spendstamina", [ProgVariableTypes.Character, ProgVariableTypes.Number],
			(p, _) => new PhysicalStateFunction(p, "spendstamina"), ["character", "amount"], ["The character.", "Nonnegative stamina cost."],
			"Uses native stamina admission and expenditure. Returns false without spending when unaffordable or invalid.", "Character", ProgVariableTypes.Boolean));
		FutureProg.RegisterBuiltInFunctionCompiler(new("isoffhand", [ProgVariableTypes.Character, ProgVariableTypes.Item],
			(p, _) => new PhysicalStateFunction(p, "isoffhand"), ["character", "item"], ["The character.", "Held or wielded item."],
			"Returns whether an item's current left/right holding location is opposite the character's dominant hand. Non-hand inventory slots return false.", "Character", ProgVariableTypes.Boolean));
	}
}

#nullable enable

using MudSharp.Economy.Currency;
using MudSharp.FutureProg.Variables;

namespace MudSharp.FutureProg.Functions.Currency;

internal sealed class TransferCurrencyFunction(IList<IFunction> parameters, bool query = false) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Boolean; protected set { } }
	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var amount = ParameterFunctions[1].Result?.GetObject as decimal? ?? 0;
		var from = ParameterFunctions[2].Result?.GetObject as ICharacter;
		var to = ParameterFunctions[3].Result?.GetObject as ICharacter;
		Result = new BooleanVariable(amount > 0 && ParameterFunctions[0].Result?.GetObject is ICurrency currency &&
			from?.Body is { } source && to?.Body is { } destination && !ReferenceEquals(source, destination) &&
			(query ? source.CanGive(currency, destination, amount, true) : source.TryGiveCurrency(currency, destination, amount, true, true)));
		return StatementResult.Normal;
	}
	public static void RegisterFunctionCompiler()
	{
		foreach (var (name, query) in new[] { ("transfercurrency", false), ("cantransfercurrency", true) })
			FutureProg.RegisterBuiltInFunctionCompiler(new(name,
				[ProgVariableTypes.Currency, ProgVariableTypes.Number, ProgVariableTypes.Character, ProgVariableTypes.Character],
				(parameters, _) => new TransferCurrencyFunction(parameters, query), ["currency", "amount", "from", "to"],
				["The native currency.", "Positive exact base-unit amount.", "The payer.", "The recipient."],
				query ? "Pure ordinary exact held-cash Give preflight. Does not reserve, mint or move money." :
				"Transfers existing held cash through native prepared Give rules. Exact denomination, custody, ownership, body, proximity and capacity checks apply. Returns whether the transfer committed; never mints replacement money. Output is silent; native events still fire.", "Currency", ProgVariableTypes.Boolean));
	}
}

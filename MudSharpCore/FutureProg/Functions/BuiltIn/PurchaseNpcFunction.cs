#nullable enable

using MudSharp.Economy.Currency;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.Templates;

namespace MudSharp.FutureProg.Functions.BuiltIn;

internal sealed class PurchaseNpcFunction(IList<IFunction> parameters, IFuturemud gameworld) : BuiltInFunction(parameters)
{
	public override ProgVariableTypes ReturnType { get => ProgVariableTypes.Character; protected set { } }

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error) return StatementResult.Error;
		var buyer = ParameterFunctions[0].Result?.GetObject as ICharacter;
		var seller = ParameterFunctions[1].Result?.GetObject as ICharacter;
		var templateId = ParameterFunctions[2].Result?.GetObject as decimal?;
		var currency = ParameterFunctions[3].Result?.GetObject as ICurrency;
		var amount = ParameterFunctions[4].Result?.GetObject as decimal?;
		if (buyer?.Body is not { } buyerBody || seller?.Body is not { } sellerBody ||
			ReferenceEquals(buyerBody, sellerBody) || templateId is null || templateId <= 0 ||
			templateId > long.MaxValue || decimal.Truncate(templateId.Value) != templateId.Value || currency is null || amount is null || amount <= 0)
		{
			Result = null;
			return StatementResult.Normal;
		}
		var id = (long)templateId.Value;
		var template = gameworld.NpcTemplates.Get(id);
		if (template is null || template.Status != RevisionStatus.Current)
		{
			Result = null;
			return StatementResult.Normal;
		}
		using var participant = new NativeNpcPurchaseParticipant(buyer, seller, template);
		var committed = buyerBody.TryGiveCurrency(currency, sellerBody, amount.Value, true, participant, silent: true);
		Result = committed ? participant.DeliveredNpc : null;
		return StatementResult.Normal;
	}

	public static void RegisterFunctionCompiler() => FutureProg.RegisterBuiltInFunctionCompiler(new(
		"purchasenpc", [ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Number, ProgVariableTypes.Currency, ProgVariableTypes.Number],
		(parameters, gameworld) => new PurchaseNpcFunction(parameters, gameworld),
		["buyer", "seller", "templateId", "currency", "amount"],
		["The purchasing character.", "The seller receiving exact cash.", "The current NPC template id.", "The payment currency.", "The positive exact base-unit price."],
		"Purchases a native NPC from held cash. On success, returns the delivered NPC beside the buyer; otherwise returns null.",
		"NPCs", ProgVariableTypes.Character));
}

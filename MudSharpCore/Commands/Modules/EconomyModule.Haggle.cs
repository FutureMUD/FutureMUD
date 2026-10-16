#nullable enable
using System.Globalization;
using MudSharp.Economy;
using MudSharp.Economy.Shops;

namespace MudSharp.Commands.Modules;

internal partial class EconomyModule
{
	[PlayerCommand("Haggle", "haggle")]
	[RequiredCharacterState(CharacterState.Conscious)]
	[NoCombatCommand]
	[NoHideCommand]
	[HelpInfo("haggle", @"Negotiate shop-wide percentage terms with one timed skill check. No shopkeeper is required. Bad results can worsen prices. Each completed retry replaces the prior terms and makes the next attempt two difficulty stages harder; retries share the original real-time expiry. STOP cancels an unfinished action without a roll.

#3haggle#0 - negotiate both directions in Shared mode
#3haggle buy|sell#0 - buy from or sell to the shop in PerDirection mode
#3haggle status#0 - view current terms, next difficulty and UTC expiry
#3haggle terms <customer> <both|buy|sell> <sale-percent> <buyback-percent> <reason>#0 - pricing employees replace terms within shop bounds
#3haggle clear <customer> <reason>#0 - managers clear terms and retry history

Percentages are signed: negative sales percentages discount purchases; positive buyback percentages increase the shop payout. Manual terms preserve current expiry and retry difficulty. Shop merchandise exclusions and the safe buyback spread still apply.", AutoHelp.HelpArg)]
	protected static void Haggle(ICharacter actor, string input)
	{
		if (!DoShopCommandFindShop(actor, out var shop)) return;
		var ss = new StringStack(input.RemoveFirstWord());
		var option = ss.PopSpeech().ToLowerInvariant();
		if (option == "status")
		{
			actor.OutputHandler.Send(HaggleService.DescribeTerms(actor, shop));
			return;
		}
		if (option is "terms" or "clear")
		{
			var customer = actor.TargetActor(ss.PopSpeech());
			if (customer is null) { actor.OutputHandler.Send("Which present customer?"); return; }
			if (option == "clear")
			{
				HaggleService.TryClear(actor, customer, shop, ss.SafeRemainingArgument, out var clearMessage);
				actor.OutputHandler.Send(clearMessage);
				return;
			}
			var scope = ss.PopSpeech().ToLowerInvariant() switch
			{
				"both" => ShopDealApplicability.Both, "buy" => ShopDealApplicability.Sell,
				"sell" => ShopDealApplicability.Buy, _ => (ShopDealApplicability)0
			};
			if (!decimal.TryParse(ss.PopSpeech().TrimEnd('%'), NumberStyles.Number, actor, out var sale) ||
				!decimal.TryParse(ss.PopSpeech().TrimEnd('%'), NumberStyles.Number, actor, out var buy))
			{ actor.OutputHandler.Send("Supply signed sale and buyback percentages, then a reason."); return; }
			HaggleService.TryManual(actor, customer, shop, scope, sale / 100M, buy / 100M, ss.SafeRemainingArgument, out var termsMessage);
			actor.OutputHandler.Send(termsMessage);
			return;
		}
		var direction = option switch
		{
			"buy" => ShopDealApplicability.Sell, "sell" => ShopDealApplicability.Buy,
			"" => ShopDealApplicability.Both, _ => (ShopDealApplicability)0
		};
		if (direction == 0 || !ss.IsFinished) { actor.OutputHandler.Send("Use haggle [buy|sell], haggle status, haggle terms, or haggle clear."); return; }
		HaggleService.TryStart(actor, shop, direction, out var message);
		actor.OutputHandler.Send(message);
	}
}

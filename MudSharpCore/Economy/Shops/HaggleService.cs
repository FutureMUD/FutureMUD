#nullable enable
using System.Runtime.CompilerServices;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.RPG.Checks;

namespace MudSharp.Economy.Shops;

public static class HaggleService
{
	private static readonly ConditionalWeakTable<IFuturemud, Dictionary<long, HaggleNegotiationAction>> Pending = new();
	public static HaggleNegotiationScope Mode(IFuturemud world) =>
		Enum.TryParse<HaggleNegotiationScope>(world.GetStaticConfiguration("HaggleNegotiationScope"), true, out var mode) &&
		Enum.IsDefined(mode) ? mode : HaggleNegotiationScope.Shared;
	public static Difficulty BaseDifficulty(IShop shop) => shop.Haggling.Difficulty ??
		(Enum.TryParse<Difficulty>(shop.Gameworld.GetStaticConfiguration("HaggleDefaultDifficulty"), true, out var difficulty) &&
		 Enum.IsDefined(difficulty) ? difficulty : Difficulty.Normal);
	public static ICharacter Customer(ICharacter actor) =>
		actor.Gameworld.TryGetCharacter(CharacterInstanceIdentityComparer.IdentityId(actor), true) ?? actor;
	private static bool IsPending(ICharacter actor)
	{
		var pending = Pending.GetOrCreateValue(actor.Gameworld);
		var id = CharacterInstanceIdentityComparer.IdentityId(actor);
		if (!pending.TryGetValue(id, out var action)) return false;
		if (action.Owner.Effects.Contains(action)) return true;
		// Some cancellation paths remove transient effects without firing RemovalEffect.
		action.RemovalEffect();
		return false;
	}

	public static void CancelPending(ICharacter actor)
	{
		if (actor.Gameworld is { } world && Pending.TryGetValue(world, out var pending) &&
			pending.TryGetValue(CharacterInstanceIdentityComparer.IdentityId(actor), out var action))
			action.Owner.RemoveEffect(action, true);
	}
	public static ShopDealApplicability Scope(IShop shop, ShopDealApplicability direction) =>
		Mode(shop.Gameworld) == HaggleNegotiationScope.Shared ? ShopDealApplicability.Both : direction;

	/// <summary>Expired windows are pruned together. Mode changes preserve the strictest history and earliest expiry.</summary>
	public static void Reconcile(ICharacter customer, IShop shop)
	{
		var now = RuntimeClock.UtcNow;
		customer.RemoveAllEffects<HaggleShopNegotiationState>(x => x.ShopId == shop.Id && x.WindowExpiresAtUtc <= now);
		customer.RemoveAllEffects<HaggleShopPriceEffect>(x => x.ShopId == shop.Id && x.WindowExpiresAtUtc <= now);
		var states = customer.EffectsOfType<HaggleShopNegotiationState>().Where(x => x.ShopId == shop.Id).ToList();
		var mode = Mode(shop.Gameworld);
		if (states.Count == 0 || states.All(x => x.Mode == mode)) return;
		var prices = customer.EffectsOfType<HaggleShopPriceEffect>().Where(x => x.ShopId == shop.Id).ToList();
		var expiry = states.Min(x => x.WindowExpiresAtUtc);
		var difficulty = states.Max(x => x.NextDifficulty);
		var attempts = states.Max(x => x.CompletedAttempts);
		customer.RemoveAllEffects<HaggleShopNegotiationState>(x => x.ShopId == shop.Id);
		customer.RemoveAllEffects<HaggleShopPriceEffect>(x => x.ShopId == shop.Id);
		var scopes = mode == HaggleNegotiationScope.Shared
			? new[] { ShopDealApplicability.Both }
			: new[] { ShopDealApplicability.Sell, ShopDealApplicability.Buy };
		foreach (var scope in scopes)
		{
			customer.AddEffect(new HaggleShopNegotiationState(customer, shop.Id, scope, mode, difficulty, attempts, expiry));
			// Keep the existing signed terms for each side, including provenance, until the shared expiry.
			foreach (var price in prices.Where(x => (x.Scope & scope) != 0))
			{
				customer.AddEffect(new HaggleShopPriceEffect(customer, shop.Id, price.Scope & scope,
					price.SaleAdjustment, price.BuyAdjustment, expiry, price.IssuerId, price.Reason));
			}
		}
	}

	public static HaggleShopNegotiationState? State(ICharacter actor, IShop shop, ShopDealApplicability scope)
	{
		var customer = Customer(actor);
		Reconcile(customer, shop);
		return customer.EffectsOfType<HaggleShopNegotiationState>()
			.FirstOrDefault(x => x.ShopId == shop.Id && x.Scope == scope);
	}

	public static decimal Adjustment(ICharacter? actor, IShop shop, IMerchandise merchandise,
		ShopDealApplicability direction)
	{
		if (actor is null || !Enabled(shop) || shop.Haggling.ExcludedMerchandise.Contains(merchandise.Id)) return 0M;
		var customer = Customer(actor);
		Reconcile(customer, shop);
		var effect = customer.EffectsOfType<HaggleShopPriceEffect>()
			.LastOrDefault(x => x.ShopId == shop.Id && x.Scope.HasFlag(direction) && x.Applies());
		return shop.Haggling.Clamp(direction == ShopDealApplicability.Buy
			? effect?.BuyAdjustment ?? 0M : effect?.SaleAdjustment ?? 0M, direction);
	}

	public static bool Enabled(IShop shop) => shop.Gameworld.GetStaticBool("HagglingEnabled") && !shop.Haggling.Disabled;
	private static bool Eligible(ICharacter actor, IShop shop, out string reason)
	{
		reason = "You cannot haggle here.";
		return Enabled(shop) && shop.IsTrading && shop.IsReadyToDoBusiness &&
			shop.CurrentLocations.Contains(actor.Location) && actor.Combat is null && actor.Movement is null &&
			CharacterState.Able.HasFlag(actor.State) && shop.IsWelcomeCustomer(actor) &&
			(shop as ITransientShop)?.CurrentStall?.IsTrading != false;
	}

	public static bool TryStart(ICharacter actor, IShop shop, ShopDealApplicability direction, out string message)
	{
		if (!Eligible(actor, shop, out message)) return false;
		if (actor.Effects.Any(x => x.IsBlockingEffect("general")))
		{
			message = "You must stop your current action before negotiating.";
			return false;
		}
		var mode = Mode(shop.Gameworld);
		if (mode == HaggleNegotiationScope.PerDirection && direction is not (ShopDealApplicability.Sell or ShopDealApplicability.Buy))
		{
			message = "Specify buy (buying from the shop) or sell (selling to the shop).";
			return false;
		}
		var trait = actor.Gameworld.Traits.Get(actor.Gameworld.GetStaticLong("HaggleTraitId"));
		if (trait is null || !actor.HasTrait(trait))
		{
			message = "You do not have the configured Haggle skill.";
			return false;
		}
		if (IsPending(actor))
		{
			message = "You are already negotiating shop prices.";
			return false;
		}
		var scope = Scope(shop, direction);
		var state = State(actor, shop, scope);
		var difficulty = state?.NextDifficulty ?? BaseDifficulty(shop);
		if (difficulty >= Difficulty.Impossible)
		{
			message = "Further negotiation is impossible until the current window expires.";
			return false;
		}
		var id = CharacterInstanceIdentityComparer.IdentityId(actor);
		var pending = Pending.GetOrCreateValue(actor.Gameworld);
		var action = new HaggleNegotiationAction(actor, completedOwner =>
		{
			// Revalidate before the single learning/check opportunity. Never roll for a cancelled or stale action.
			if (!Eligible(actor, shop, out _) || Mode(shop.Gameworld) != mode || !actor.HasTrait(trait) ||
				state is not null && state.WindowExpiresAtUtc <= RuntimeClock.UtcNow)
			{
				actor.OutputHandler.Send("Your negotiation ends without changing your terms.");
				return;
			}
			var outcome = actor.Gameworld.GetCheck(CheckType.HaggleCheck).Check(actor, difficulty, trait).Outcome;
			Complete(actor, shop, scope, outcome, difficulty);
		}, () => pending.Remove(id));
		pending[id] = action;
		actor.AddEffect(action, TimeSpan.FromSeconds(Math.Clamp(actor.Gameworld.GetStaticDouble("HaggleActionDurationSeconds"), 1, 3600)));
		message = $"You begin negotiating prices with {shop.Name.TitleCase().ColourName()} at {difficulty.DescribeEnum().ColourName()} difficulty. Use STOP to cancel.";
		return true;
	}

	private static void Complete(ICharacter actor, IShop shop, ShopDealApplicability scope, Outcome outcome, Difficulty difficulty)
	{
		var customer = Customer(actor);
		var state = State(actor, shop, scope);
		var expiry = state?.WindowExpiresAtUtc ?? NewExpiry(shop);
		var factor = outcome switch
		{
			Outcome.MajorPass => 1M, Outcome.Pass => 0.65M, Outcome.MinorPass => 0.30M,
			Outcome.MinorFail => 0M, Outcome.Fail => -0.5M, Outcome.MajorFail => -1M, _ => 0M
		};
		var sale = factor >= 0 ? -shop.Haggling.SaleDiscount * factor : -shop.Haggling.SaleMarkup * factor;
		var buy = factor >= 0 ? shop.Haggling.BuyBonus * factor : shop.Haggling.BuyPenalty * factor;
		ReplaceState(customer, shop, scope, difficulty.StageUp(2), (state?.CompletedAttempts ?? 0) + 1, expiry);
		ReplacePrice(customer, shop, scope, sale, buy, expiry);
		actor.OutputHandler.Send($"You complete your negotiation ({outcome.DescribeEnum().ColourName()}). {DescribeTerms(customer, shop)}");
		ResultEvent(actor, shop, scope, outcome, sale, buy, null);
	}

	private static DateTime NewExpiry(IShop shop) => RuntimeClock.UtcNow.AddHours(
		Math.Clamp(shop.Gameworld.GetStaticDouble("HaggleWindowDurationHours"), 1.0 / 3600.0, 8760));
	private static void ReplaceState(ICharacter customer, IShop shop, ShopDealApplicability scope,
		Difficulty difficulty, int attempts, DateTime expiry)
	{
		customer.RemoveAllEffects<HaggleShopNegotiationState>(x => x.ShopId == shop.Id && x.Scope == scope);
		customer.AddEffect(new HaggleShopNegotiationState(customer, shop.Id, scope, Mode(shop.Gameworld), difficulty, attempts, expiry));
	}
	private static void ReplacePrice(ICharacter customer, IShop shop, ShopDealApplicability scope,
		decimal sale, decimal buy, DateTime expiry, long issuer = 0, string reason = "Skill negotiation")
	{
		customer.RemoveAllEffects<HaggleShopPriceEffect>(x => x.ShopId == shop.Id && (x.Scope & scope) != 0);
		customer.AddEffect(new HaggleShopPriceEffect(customer, shop.Id, scope,
			scope.HasFlag(ShopDealApplicability.Sell) ? shop.Haggling.Clamp(sale, ShopDealApplicability.Sell) : 0,
			scope.HasFlag(ShopDealApplicability.Buy) ? shop.Haggling.Clamp(buy, ShopDealApplicability.Buy) : 0,
			expiry, issuer, reason));
	}

	public static bool HasPricingAuthority(ICharacter issuer, IShop shop) => issuer.IsAdministrator() ||
		shop.IsManager(issuer) || shop.IsProprietor(issuer) || shop.IsEmployee(issuer) &&
		shop.Haggling.PricingEmployees.Contains(CharacterInstanceIdentityComparer.IdentityId(issuer));

	public static bool TryManual(ICharacter issuer, ICharacter actor, IShop shop, ShopDealApplicability scope,
		decimal sale, decimal buy, string reason, out string message)
	{
		message = "You need pricing authority, enabled haggling, valid shop percentage limits, and a reason; the customer must not be negotiating.";
		if (!Enabled(shop) || !HasPricingAuthority(issuer, shop) || IsPending(actor) || string.IsNullOrWhiteSpace(reason) ||
			Scope(shop, scope) != scope || Mode(shop.Gameworld) == HaggleNegotiationScope.PerDirection && scope == ShopDealApplicability.Both ||
			scope is not (ShopDealApplicability.Sell or ShopDealApplicability.Buy or ShopDealApplicability.Both) ||
			scope.HasFlag(ShopDealApplicability.Sell) && shop.Haggling.Clamp(sale, ShopDealApplicability.Sell) != sale ||
			scope.HasFlag(ShopDealApplicability.Buy) && shop.Haggling.Clamp(buy, ShopDealApplicability.Buy) != buy) return false;
		var customer = Customer(actor);
		var state = State(actor, shop, scope);
		var expiry = state?.WindowExpiresAtUtc ?? NewExpiry(shop);
		if (state is null) ReplaceState(customer, shop, scope, BaseDifficulty(shop), 0, expiry);
		ReplacePrice(customer, shop, scope, sale, buy, expiry, CharacterInstanceIdentityComparer.IdentityId(issuer), reason);
		Audit(shop, issuer, $"replace customer #{CharacterInstanceIdentityComparer.IdentityId(actor)} {scope}: sale {sale}, buy {buy}; {reason}");
		message = DescribeTerms(customer, shop);
		ResultEvent(actor, shop, scope, Outcome.NotTested, sale, buy, issuer);
		return true;
	}

	public static bool TryClear(ICharacter issuer, ICharacter actor, IShop shop, string reason, out string message)
	{
		message = "Only a manager, proprietor or administrator may clear negotiation history; give a reason and wait for pending negotiation to end.";
		if (!(issuer.IsAdministrator() || shop.IsManager(issuer) || shop.IsProprietor(issuer)) ||
			IsPending(actor) || string.IsNullOrWhiteSpace(reason)) return false;
		var customer = Customer(actor);
		customer.RemoveAllEffects<HaggleShopNegotiationState>(x => x.ShopId == shop.Id);
		customer.RemoveAllEffects<HaggleShopPriceEffect>(x => x.ShopId == shop.Id);
		Audit(shop, issuer, $"clear customer #{CharacterInstanceIdentityComparer.IdentityId(actor)}; {reason}");
		message = "Negotiated prices and retry history cleared.";
		return true;
	}

	public static void Audit(IShop shop, ICharacter issuer, string text)
	{
		shop.Haggling.Audit.Add($"{RuntimeClock.UtcNow:O} issuer #{CharacterInstanceIdentityComparer.IdentityId(issuer)}: {text}");
		if (shop.Haggling.Audit.Count > 100) shop.Haggling.Audit.RemoveRange(0, shop.Haggling.Audit.Count - 100);
		shop.Changed = true;
	}
	public static string DescribeTerms(ICharacter actor, IShop shop)
	{
		var customer = Customer(actor);
		Reconcile(customer, shop);
		var prices = customer.EffectsOfType<HaggleShopPriceEffect>().Where(x => x.ShopId == shop.Id).ToList();
		var states = customer.EffectsOfType<HaggleShopNegotiationState>().Where(x => x.ShopId == shop.Id).ToList();
		return prices.Select(x => $"{x.Scope.DescribeEnum()}: shop sales {x.SaleAdjustment.ToString("P1", actor)}, buybacks {x.BuyAdjustment.ToString("P1", actor)}; expires {x.WindowExpiresAtUtc:O}")
			.Concat(states.Select(x => $"{x.Scope.DescribeEnum()}: next {x.NextDifficulty.DescribeEnum()} ({x.CompletedAttempts.ToString("N0", actor)} completed)"))
			.DefaultIfEmpty("No current negotiated terms or retry history.").ListToString();
	}
	private static void ResultEvent(ICharacter customer, IShop shop, ShopDealApplicability scope, Outcome outcome,
		decimal sale, decimal buy, ICharacter? issuer)
	{
		customer.HandleEvent(EventType.HaggleResult, customer, shop, scope.ToString(), (int)outcome, sale, buy, issuer);
		foreach (var witness in customer.Location.EventHandlersFor(customer))
			witness.HandleEvent(EventType.WitnessHaggleResult, customer, witness, shop, scope.ToString(), (int)outcome, sale, buy, issuer);
	}
}

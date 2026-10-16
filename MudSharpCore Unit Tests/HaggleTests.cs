#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Shops;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class HaggleTests
{
	private Mock<IFuturemud> _world = null!;
	private Mock<ICharacter> _actor = null!;
	private Mock<IShop> _shop = null!;
	private Mock<ICheck> _check = null!;
	private List<IEffect> _effects = null!;
	private ManualTimeProvider _clock = null!;
	private IDisposable _clockScope = null!;
	private HaggleSettings _settings = null!;
	private string _mode = "Shared";
	private Outcome _outcome = Outcome.MajorPass;
	private readonly List<Difficulty> _rolls = new();

	[TestInitialize]
	public void Setup()
	{
		_clock = new ManualTimeProvider(new DateTimeOffset(2026, 10, 16, 0, 0, 0, TimeSpan.Zero));
		_clockScope = RuntimeClock.Push(_clock);
		_world = new Mock<IFuturemud>();
		_world.Setup(x => x.GetStaticBool("HagglingEnabled")).Returns(true);
		_world.Setup(x => x.GetStaticConfiguration("HaggleNegotiationScope")).Returns(() => _mode);
		_world.Setup(x => x.GetStaticDouble("HaggleWindowDurationHours")).Returns(6);
		_world.Setup(x => x.GetStaticDouble("HaggleActionDurationSeconds")).Returns(20);
		_world.Setup(x => x.GetStaticConfiguration("HaggleDefaultDifficulty")).Returns("Normal");
		_world.Setup(x => x.GetStaticLong("HaggleTraitId")).Returns(123);
		var traits = new Mock<IUneditableAll<ITraitDefinition>>();
		var trait = new Mock<ITraitDefinition>();
		trait.SetupGet(x => x.TraitType).Returns(TraitType.Skill);
		traits.Setup(x => x.Get(123)).Returns(trait.Object);
		_world.SetupGet(x => x.Traits).Returns(traits.Object);
		var progs = new Mock<IUneditableAll<IFutureProg>>();
		_world.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		_actor = new Mock<ICharacter>();
		_effects = HaggleTestEffects.Bind(_actor, _world.Object);
		_actor.SetupGet(x => x.Id).Returns(100);
		_actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		_actor.Setup(x => x.HasTrait(trait.Object)).Returns(true);
		_actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		var room = new Mock<IRoom>();
		room.SetupGet(x => x.EventHandlers).Returns(Array.Empty<IHandleEvents>());
		_actor.SetupGet(x => x.Location).Returns(room.Object);
		_settings = new HaggleSettings();
		_shop = new Mock<IShop>();
		_shop.SetupGet(x => x.Id).Returns(42);
		_shop.SetupGet(x => x.Name).Returns("Haggle Test Shop");
		_shop.SetupGet(x => x.Haggling).Returns(_settings);
		_shop.SetupGet(x => x.Gameworld).Returns(_world.Object);
		_shop.SetupGet(x => x.IsTrading).Returns(true);
		_shop.SetupGet(x => x.IsReadyToDoBusiness).Returns(true);
		_shop.Setup(x => x.IsWelcomeCustomer(_actor.Object)).Returns(true);
		_shop.SetupGet(x => x.CurrentLocations).Returns(new[] { room.Object });
		_check = new Mock<ICheck>();
		_check.Setup(x => x.Check(_actor.Object, It.IsAny<Difficulty>(), trait.Object, null, 0,
			TraitUseType.Practical, It.IsAny<(string Parameter, object value)[]>()))
			.Callback<IPerceivableHaveTraits, Difficulty, ITraitDefinition, IPerceivable, double, TraitUseType, (string, object)[]>((_, difficulty, _, _, _, _, _) => _rolls.Add(difficulty))
			.Returns(() => new CheckOutcome { Outcome = _outcome });
		_world.Setup(x => x.GetCheck(CheckType.HaggleCheck)).Returns(_check.Object);
	}

	[TestCleanup]
	public void Cleanup() => _clockScope.Dispose();
	private HaggleShopNegotiationState State(ShopDealApplicability scope = ShopDealApplicability.Both) =>
		_effects.OfType<HaggleShopNegotiationState>().Single(x => x.Scope == scope);
	private HaggleShopPriceEffect Price() => _effects.OfType<HaggleShopPriceEffect>().Single();
	private void Complete(ShopDealApplicability scope = ShopDealApplicability.Both)
	{
		Assert.IsTrue(HaggleService.TryStart(_actor.Object, _shop.Object, scope, out var message), message);
		_effects.OfType<HaggleNegotiationAction>().Single().ExpireEffect();
	}

	[TestMethod]
	public void Cancel_NoRollOrWindow_AndAllowsNewAttempt()
	{
		Assert.IsTrue(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		Assert.IsFalse(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		var action = _effects.OfType<HaggleNegotiationAction>().Single();
		_actor.Object.RemoveEffect(action, true);
		action.ExpireEffect(); // A cancelled schedule callback cannot complete later.
		Assert.AreEqual(0, _rolls.Count);
		Assert.AreEqual(0, _effects.Count);
		Complete();
		Assert.AreEqual(1, _rolls.Count);
	}

	[TestMethod]
	public void Retries_ReplaceFavourableWithAdverseAndNeutral_KeepOriginalExpiryUntilImpossible()
	{
		Complete();
		var expiry = State().WindowExpiresAtUtc;
		Assert.AreEqual(RuntimeClock.UtcNow.AddHours(6), expiry);
		Assert.AreEqual(-0.10M, Price().SaleAdjustment);
		_clock.Advance(TimeSpan.FromHours(1));
		_outcome = Outcome.MajorFail;
		Complete();
		Assert.AreEqual(0.20M, Price().SaleAdjustment);
		Assert.AreEqual(-0.20M, Price().BuyAdjustment);
		Assert.AreEqual(expiry, Price().WindowExpiresAtUtc);
		_outcome = Outcome.MinorFail;
		Complete();
		Assert.AreEqual(0M, Price().SaleAdjustment);
		Assert.AreEqual(0M, Price().BuyAdjustment);
		Assert.AreEqual(expiry, State().WindowExpiresAtUtc);
		Assert.AreEqual(Difficulty.Impossible, State().NextDifficulty);
		CollectionAssert.AreEqual(new[] { Difficulty.Normal, Difficulty.VeryHard, Difficulty.Insane }, _rolls);
		Assert.IsFalse(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		_actor.Verify(x => x.HandleEvent(EventType.HaggleResult, It.IsAny<object[]>()), Times.Exactly(3));
		_clock.SetUtcNow(new DateTimeOffset(expiry));
		Complete();
		Assert.AreEqual(1, State().CompletedAttempts);
		Assert.AreEqual(Difficulty.Normal, _rolls.Last());
	}

	[TestMethod]
	public void DisconnectAfterFocusChange_CancelsByIdentityWithoutRoll()
	{
		Assert.IsTrue(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		var focused = new Mock<ICharacter>();
		focused.SetupGet(x => x.Id).Returns(100); focused.SetupGet(x => x.Gameworld).Returns(_world.Object);
		HaggleService.CancelPending(focused.Object);
		Assert.AreEqual(0, _effects.Count); Assert.AreEqual(0, _rolls.Count);
		Complete(); Assert.AreEqual(1, State().CompletedAttempts);
	}

	[TestMethod]
	public void CancelRetryAndQuit_PreserveCompletedTermsAndDifficulty()
	{
		Complete();
		var state = State(); var price = Price();
		Assert.IsTrue(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		_actor.Raise(x => x.OnQuit += null, _actor.Object);
		Assert.AreSame(state, State()); Assert.AreSame(price, Price());
		Assert.AreEqual(1, _rolls.Count);
		Complete();
		Assert.AreEqual(2, State().CompletedAttempts);
	}

	[TestMethod]
	public void PerDirection_IndependentThenModeMigration_PreservesStrictestDifficultyAndEarliestExpiry()
	{
		_mode = "PerDirection";
		Assert.IsFalse(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		Complete(ShopDealApplicability.Sell);
		var expiry = State(ShopDealApplicability.Sell).WindowExpiresAtUtc;
		_clock.Advance(TimeSpan.FromHours(1));
		Complete(ShopDealApplicability.Buy);
		Complete(ShopDealApplicability.Buy);
		Assert.AreEqual(Difficulty.VeryHard, State(ShopDealApplicability.Sell).NextDifficulty);
		Assert.AreEqual(Difficulty.Insane, State(ShopDealApplicability.Buy).NextDifficulty);
		_mode = "Shared";
		HaggleService.Reconcile(_actor.Object, _shop.Object);
		Assert.AreEqual(Difficulty.Insane, State().NextDifficulty);
		Assert.AreEqual(expiry, State().WindowExpiresAtUtc);
		Assert.IsTrue(_effects.OfType<HaggleShopPriceEffect>().All(x => x.WindowExpiresAtUtc == expiry));
		_mode = "PerDirection";
		HaggleService.Reconcile(_actor.Object, _shop.Object);
		Assert.AreEqual(Difficulty.Insane, State(ShopDealApplicability.Sell).NextDifficulty);
		Assert.AreEqual(Difficulty.Insane, State(ShopDealApplicability.Buy).NextDifficulty);
	}

	[TestMethod]
	public void ExpiryOrModeChangeDuringAction_CancelsWithoutRoll()
	{
		Complete();
		Assert.IsTrue(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		_clock.Advance(TimeSpan.FromHours(6));
		_effects.OfType<HaggleNegotiationAction>().Single().ExpireEffect();
		Assert.AreEqual(1, _rolls.Count);
		Assert.IsTrue(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		_mode = "PerDirection";
		_effects.OfType<HaggleNegotiationAction>().Single().ExpireEffect();
		Assert.AreEqual(1, _rolls.Count);
		Assert.AreEqual(0, _effects.Count);
	}

	[TestMethod]
	public void ManualTerms_RequireExplicitPricingAuthority_PreserveRetryAndManagerClearIsAudited()
	{
		Complete();
		var state = State();
		Assert.IsFalse(HaggleService.TryManual(_actor.Object, _actor.Object, _shop.Object, ShopDealApplicability.Both, -.05M, .05M, "test", out _));
		_shop.Setup(x => x.IsEmployee(_actor.Object)).Returns(true);
		_settings.PricingEmployees.Add(100);
		Assert.IsFalse(HaggleService.TryManual(_actor.Object, _actor.Object, _shop.Object, ShopDealApplicability.Both, -.9M, .05M, "outside limit", out _));
		Assert.IsTrue(HaggleService.TryManual(_actor.Object, _actor.Object, _shop.Object, ShopDealApplicability.Both, -.05M, .05M, "customer goodwill", out _));
		Assert.AreSame(state, State()); Assert.AreEqual(state.WindowExpiresAtUtc, Price().WindowExpiresAtUtc);
		Assert.AreEqual(100, Price().IssuerId); Assert.AreEqual("customer goodwill", Price().Reason);
		Assert.IsFalse(HaggleService.TryClear(_actor.Object, _actor.Object, _shop.Object, "reset", out _));
		_shop.Setup(x => x.IsManager(_actor.Object)).Returns(true);
		Assert.IsTrue(HaggleService.TryClear(_actor.Object, _actor.Object, _shop.Object, "manager correction", out _));
		Assert.AreEqual(0, _effects.Count);
		Assert.IsTrue(_settings.Audit.Last().Contains("manager correction"));
		Assert.AreEqual(1, _rolls.Count);
		Assert.IsTrue(HaggleService.TryManual(_actor.Object, _actor.Object, _shop.Object, ShopDealApplicability.Both, 0, 0, "fresh manual window", out _));
		Assert.AreEqual(Difficulty.Normal, State().NextDifficulty); Assert.AreEqual(0, State().CompletedAttempts);
	}

	[TestMethod]
	public void PersistReload_AbsoluteUtcSurvivesOfflineAndExpiresAtBoundary()
	{
		Complete();
		HaggleShopNegotiationState.InitialiseEffectType(); HaggleShopPriceEffect.InitialiseEffectType();
		var xml = _effects.Select(x => x.SaveToXml(new())).ToList();
		var expiry = State().WindowExpiresAtUtc;
		_clock.Advance(TimeSpan.FromHours(5));
		_effects.Clear();
		_effects.AddRange(xml.Select(x => Effect.LoadEffect(x, _actor.Object)));
		Assert.AreEqual(expiry, State().WindowExpiresAtUtc); Assert.AreEqual(expiry, Price().WindowExpiresAtUtc);
		Assert.AreEqual(Difficulty.VeryHard, State().NextDifficulty);
		_clock.SetUtcNow(new DateTimeOffset(expiry));
		Assert.IsTrue(xml.Select(x => Effect.LoadEffect(x, _actor.Object)).All(x => x.LoadErrors));
		HaggleService.Reconcile(_actor.Object, _shop.Object);
		Assert.AreEqual(0, _effects.Count);
	}

	[TestMethod]
	public void OptInAndShopOptOut_RefuseWithoutStartingOrRolling()
	{
		Assert.AreEqual("false", DefaultStaticSettings.DefaultStaticConfigurations["HagglingEnabled"]);
		Assert.AreEqual("6", DefaultStaticSettings.DefaultStaticConfigurations["HaggleWindowDurationHours"]);
		Assert.AreEqual("Shared", DefaultStaticSettings.DefaultStaticConfigurations["HaggleNegotiationScope"]);
		_settings.Disabled = true;
		Assert.IsFalse(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		_settings.Disabled = false;
		_world.Setup(x => x.GetStaticBool("HagglingEnabled")).Returns(false);
		Assert.IsFalse(HaggleService.TryStart(_actor.Object, _shop.Object, ShopDealApplicability.Both, out _));
		Assert.AreEqual(0, _effects.Count); Assert.AreEqual(0, _rolls.Count);
	}
}

internal static class HaggleTestEffects
{
	internal static List<IEffect> Bind(Mock<ICharacter> actor, IFuturemud world)
	{
		var effects = new List<IEffect>();
		actor.SetupGet(x => x.Gameworld).Returns(world);
		actor.SetupGet(x => x.Effects).Returns(effects);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => { effects.Add(effect); effect.InitialEffect(); });
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, fire) => { effects.Remove(effect); if (fire) effect.RemovalEffect(); });
		BindType<HaggleShopNegotiationState>(actor, effects);
		BindType<HaggleShopPriceEffect>(actor, effects);
		return effects;
	}
	private static void BindType<T>(Mock<ICharacter> actor, List<IEffect> effects) where T : class, IEffect
	{
		actor.Setup(x => x.EffectsOfType(It.IsAny<Predicate<T>>())).Returns<Predicate<T>>(predicate => effects.OfType<T>().Where(x => predicate?.Invoke(x) != false));
		actor.Setup(x => x.RemoveAllEffects(It.IsAny<Predicate<T>>(), It.IsAny<bool>())).Returns<Predicate<T>, bool>((predicate, fire) =>
		{
			var removed = effects.OfType<T>().Where(x => predicate?.Invoke(x) != false).ToList();
			foreach (var effect in removed) { effects.Remove(effect); if (fire) effect.RemovalEffect(); }
			return removed.Count > 0;
		});
	}
}

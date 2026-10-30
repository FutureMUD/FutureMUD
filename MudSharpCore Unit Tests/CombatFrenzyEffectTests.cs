#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Globalization;
using MudSharp.Effects;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.RPG.Checks;
using MudSharp.Framework.Scheduling;
using ConcreteBody = MudSharp.Body.Implementations.Body;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatFrenzyEffectTests
{
	[TestMethod]
	public void CombatFrenzyEffect_ExposesConfiguredCombatAndNeedValues()
	{
		var character = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		body.Setup(x => x.Id).Returns(123L);
		character.Setup(x => x.Body).Returns(body.Object);
		character.Setup(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		var effect = new CombatFrenzyEffect(character.Object, 15.0, 15.0, 5.0, 4.0);

		Assert.IsTrue(effect.Applies());
		Assert.AreEqual(15.0, effect.CheckBonusFor(CheckType.NaturalWeaponAttack));
		Assert.AreEqual(15.0, effect.CheckBonusFor(CheckType.DodgeCheck));
		Assert.AreEqual(0.0, effect.CheckBonusFor(CheckType.GenericSkillCheck));
		Assert.AreEqual(5.0, effect.MeleeDamageBonus);
		Assert.AreEqual(4.0, effect.HungerMultiplier);
		Assert.AreEqual(1.0, effect.ThirstMultiplier);
		Assert.IsTrue(effect.IsRaging);
		Assert.IsFalse(effect.IsSuperRaging);
		Assert.IsTrue(effect.SavingEffect);
		Assert.IsTrue(effect is IPreventStunUnconsciousnessEffect);
		Assert.IsTrue(effect is INoMercyEffect);
	}

	[TestMethod]
	public void CombatFrenzyEffect_PreventsRestingAndSleepingAndIsScopedToItsBody()
	{
		var character = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		body.Setup(x => x.Id).Returns(123L);
		character.Setup(x => x.Body).Returns(body.Object);
		character.Setup(x => x.Gameworld).Returns(Mock.Of<IFuturemud>());
		var effect = new CombatFrenzyEffect(character.Object, 15.0, 15.0, 5.0, 4.0);

		Assert.IsTrue(effect.PreventsChange(PositionStanding.Instance, PositionSitting.Instance));
		Assert.IsTrue(effect.PreventsChange(PositionStanding.Instance, PositionLyingDown.Instance));
		Assert.IsFalse(effect.PreventsChange(PositionSitting.Instance, PositionStanding.Instance));
		Assert.IsFalse(string.IsNullOrEmpty(effect.SleepPreventionEcho));

		body.Setup(x => x.Id).Returns(124L);
		Assert.IsFalse(effect.Applies());
		Assert.IsFalse(effect.AppliesToCheck(CheckType.NaturalWeaponAttack));
		Assert.AreEqual(0.0, effect.CheckBonusFor(CheckType.NaturalWeaponAttack));
		Assert.AreEqual(0.0, effect.MeleeDamageBonus);
		Assert.AreEqual(1.0, effect.HungerMultiplier);
		Assert.IsFalse(effect.IsRaging);
		Assert.IsFalse(effect.PreventsChange(PositionStanding.Instance, PositionSitting.Instance));
		Assert.AreEqual(string.Empty, effect.SleepPreventionEcho);
	}

	[TestMethod]
	public void CombatFrenzyEffect_PersistsAndReloadsConfiguredValuesUsingInvariantXmlNumbers()
	{
		var originalCulture = CultureInfo.CurrentCulture;
		try
		{
			CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			var character = new Mock<ICharacter>();
			var body = new Mock<IBody>();
			body.Setup(x => x.Id).Returns(987654321L);
			character.Setup(x => x.Body).Returns(body.Object);
			var gameworld = new Mock<IFuturemud>();
			var futureProgs = new Mock<IUneditableAll<IFutureProg>>();
			futureProgs.Setup(x => x.Get(0)).Returns((IFutureProg?)null);
			gameworld.Setup(x => x.FutureProgs).Returns(futureProgs.Object);
			character.Setup(x => x.Gameworld).Returns(gameworld.Object);

			var original = new CombatFrenzyEffect(character.Object, 15.75, 14.25, 5.5, 4.125);
			CombatFrenzyEffect.InitialiseEffectType();
			var saved = original.SaveToXml(new Dictionary<IEffect, TimeSpan>());
			Assert.AreEqual("CombatFrenzy", saved.Element("Type")?.Value);
			Assert.IsNotNull(saved.Element("Effect"));
			var loaded = (CombatFrenzyEffect)Effect.LoadEffect(saved, character.Object);

			Assert.AreEqual(987654321L, loaded.SourceBodyId);
			Assert.AreEqual(15.75, loaded.OffenceBonus);
			Assert.AreEqual(14.25, loaded.DefenceBonus);
			Assert.AreEqual(5.5, loaded.MeleeDamageBonus);
			Assert.AreEqual(4.125, loaded.HungerMultiplier);
			Assert.IsTrue(loaded.SavingEffect);
		}
		finally
		{
			CultureInfo.CurrentCulture = originalCulture;
		}
	}

	[TestMethod]
	public void AddCombatFrenzyFunction_RejectsDeadProjectionAndExhaustedCharacters()
	{
		var world = new Mock<IFuturemud>();
		foreach (var (state, kind, stamina) in new[]
		         {
			         (CharacterState.Dead, CharacterInstanceKind.Primary, 50.0),
			         (CharacterState.Awake, CharacterInstanceKind.AstralProjection, 50.0),
			         (CharacterState.Awake, CharacterInstanceKind.Primary, 0.0)
		         })
		{
			var character = CreateCharacter(world.Object, 1, stamina);
			character.SetupGet(x => x.State).Returns(state);
			character.SetupGet(x => x.InstanceKind).Returns(kind);
			var function = CompileAddFrenzy(world.Object, character.Object);
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			Assert.IsInstanceOfType<NullVariable>(function.Result);
		}
	}

	[TestMethod]
	public void AddCombatFrenzyFunction_RechecksCurrentBodyAfterRemovingExistingEffect()
	{
		var world = new Mock<IFuturemud>();
		var character = CreateCharacter(world.Object, 1, 50.0);
		var originalBody = character.Object.Body;
		var existing = new CombatFrenzyEffect(character.Object, 15.0, 15.0, 5.0, 4.0);
		character.SetupGet(x => x.Effects).Returns(new IEffect[] { existing });
		var replacementBody = new Mock<IBody>();
		replacementBody.SetupGet(x => x.Id).Returns(2L);
		replacementBody.SetupGet(x => x.CurrentStamina).Returns(50.0);
		character.Setup(x => x.RemoveEffect(existing, false)).Callback(() => character.SetupGet(x => x.Body).Returns(replacementBody.Object));
		var function = CompileAddFrenzy(world.Object, character.Object);

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.IsInstanceOfType<NullVariable>(function.Result);
		Assert.AreNotSame(originalBody, character.Object.Body);
	}

	[TestMethod]
	public void AddCombatFrenzyFunction_PreservesCallbackCreatedReplacementEffect()
	{
		var world = new Mock<IFuturemud>();
		var character = CreateCharacter(world.Object, 1, 50.0);
		var existing = new CombatFrenzyEffect(character.Object, 10.0, 10.0, 2.0, 3.0);
		var callbackReplacement = new CombatFrenzyEffect(character.Object, 7.0, 8.0, 1.0, 2.0);
		character.SetupGet(x => x.Effects).Returns(new IEffect[] { existing });
		character.Setup(x => x.RemoveEffect(existing, false))
			.Callback(() => character.SetupGet(x => x.Effects).Returns(new IEffect[] { callbackReplacement }));
		var function = CompileAddFrenzy(world.Object, character.Object);

		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		Assert.IsInstanceOfType<NullVariable>(function.Result);
		Assert.AreSame(callbackReplacement, character.Object.Effects.Single());
		Assert.AreEqual(7.0, callbackReplacement.OffenceBonus);
	}

	[TestMethod]
	public void MaximumStaminaClampToZero_ExpiresFrenzyAfterCapacityGuardAndRechecksHealth()
	{
		var (actor, body, handler, _) = CreateStaminaHarness(901);
		var healthChecks = 0;
		actor.Setup(x => x.CheckHealthStatus()).Callback(() =>
		{
			healthChecks++;
			Assert.IsFalse((bool)typeof(ConcreteBody)
				.GetField("_resolvingStaminaCapacity", BindingFlags.Instance | BindingFlags.NonPublic)!
				.GetValue(body)!);
		});
		var frenzy = new CombatFrenzyEffect(actor.Object, 15.0, 15.0, 5.0, 4.0);
		handler.AddEffect(frenzy);
		body.MaximumStamina = 0.0;
		body.CurrentStamina = 25.0;

		Assert.AreEqual(0.0, body.CurrentStamina);
		Assert.IsFalse(handler.Effects.Contains(frenzy));
		Assert.AreEqual(1, healthChecks);
	}

	[TestMethod]
	public void DormantBodyStaminaClamp_DoesNotRemoveCurrentBodyFrenzy()
	{
		var (actor, dormantBody, handler, _) = CreateStaminaHarness(902);
		var activeBody = CreateConcreteBody(actor.Object, actor.Object.Gameworld, 903);
		actor.SetupGet(x => x.Body).Returns(activeBody);
		var healthChecks = 0;
		actor.Setup(x => x.CheckHealthStatus()).Callback(() => healthChecks++);
		var frenzy = new CombatFrenzyEffect(actor.Object, 15.0, 15.0, 5.0, 4.0);
		handler.AddEffect(frenzy);
		dormantBody.MaximumStamina = 0.0;
		dormantBody.CurrentStamina = 25.0;

		Assert.AreEqual(0.0, dormantBody.CurrentStamina);
		Assert.IsTrue(handler.Effects.Contains(frenzy));
		Assert.IsTrue(frenzy.Applies());
		Assert.AreEqual(0, healthChecks);
	}

	private static Mock<ICharacter> CreateCharacter(IFuturemud gameworld, long bodyId, double stamina)
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(bodyId);
		body.SetupGet(x => x.CurrentStamina).Returns(stamina);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.IsEmbodied).Returns(true);
		character.SetupGet(x => x.State).Returns(CharacterState.Awake);
		character.SetupGet(x => x.InstanceKind).Returns(CharacterInstanceKind.Primary);
		character.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
		return character;
	}

	private static IFunction CompileAddFrenzy(IFuturemud gameworld, ICharacter character)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var registration = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == "addcombatfrenzyeffect");
		var values = new object[] { character, 600m, 15m, 15m, 5m, 4m };
		var parameters = values.Select((value, index) =>
		{
			var variable = new Mock<IProgVariable>();
			variable.SetupGet(x => x.GetObject).Returns(value);
			var function = new Mock<IFunction>();
			function.SetupGet(x => x.ReturnType).Returns(registration.Parameters.ElementAt(index));
			function.SetupGet(x => x.Result).Returns(variable.Object);
			function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
			return function.Object;
		}).ToArray();
		return registration.CompilerFunction(parameters, gameworld);
	}

	private static (Mock<ICharacter> Actor, ConcreteBody Body, EffectHandler Handler, Mock<IFuturemud> World)
		CreateStaminaHarness(long bodyId)
	{
		var scheduler = new Mock<IEffectScheduler>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.EffectScheduler).Returns(scheduler.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Merits).Returns(Array.Empty<MudSharp.RPG.Merits.IMerit>());
		var body = CreateConcreteBody(actor.Object, world.Object, bodyId);
		actor.SetupGet(x => x.Body).Returns(body);
		var handler = new EffectHandler(actor.Object);
		actor.SetupGet(x => x.Effects).Returns(() => handler.Effects);
		actor.Setup(x => x.EffectsOfType<IExpireAtZeroStaminaEffect>(null))
			.Returns(() => handler.EffectsOfType<IExpireAtZeroStaminaEffect>());
		actor.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>()))
			.Callback<IEffect, bool>(handler.RemoveEffect);
		return (actor, body, handler, world);
	}

	private static ConcreteBody CreateConcreteBody(ICharacter actor, IFuturemud world, long id)
	{
		var body = TestObjectFactory.CreateUninitialized<ConcreteBody>();
		body.Actor = actor;
		typeof(FrameworkItem).GetField("_id", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, id);
		typeof(PerceivedItem).GetProperty("Gameworld")!.SetValue(body, world);
		typeof(PerceivedItem).GetProperty("EffectHandler")!.SetValue(body, new EffectHandler(body));
		typeof(PerceivedItem).GetField("_noSave", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(body, true);
		foreach (var name in new[] { "_traits", "_merits", "_implants", "_externalItems" })
		{
			var field = typeof(ConcreteBody).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!;
			field.SetValue(body, Activator.CreateInstance(field.FieldType));
		}
		return body;
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.Effects;
using MudSharp.FutureProg.Variables;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PersistentStatusProgTests
{
	[TestMethod]
	public void AdminInvisibility_ItemOnloadProg_CreatesOneSavedNativeEffect()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var world = new Mock<IFuturemud>();
		var item = new Mock<MudSharp.GameItems.IGameItem>();
		var effects = new List<IEffect>();
		item.SetupGet(x => x.Gameworld).Returns(world.Object);
		item.SetupGet(x => x.Type).Returns(ProgVariableTypes.Item);
		item.SetupGet(x => x.GetObject).Returns(item.Object);
		item.SetupGet(x => x.Effects).Returns(() => effects.ToArray());
		item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		var prog = new MudSharp.FutureProg.FutureProg(world.Object, "HiddenScenery", ProgVariableTypes.Boolean,
			[Tuple.Create(ProgVariableTypes.Item, "item")], "addadmininviseffect(@item)\nreturn true");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		for (var i = 0; i < 2; i++)
		{
			Assert.IsTrue(prog.ExecuteWithStatus(out var result, item.Object));
			Assert.AreEqual(true, result);
		}
		Assert.AreEqual(1, effects.Count);
		Assert.IsInstanceOfType<AdminInvis>(effects[0]);
		Assert.IsTrue(effects[0].SavingEffect);
	}

	[TestMethod]
	public void ExplicitPlanarPermissions_KeepLegacyDefaultsAndAllowFixedNpcStates()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var effects = new List<IEffect>();
		var world = new Mock<IFuturemud>();
		var shadow = new Mock<IPlane>(); shadow.SetupGet(x => x.Id).Returns(2); shadow.SetupGet(x => x.Name).Returns("Shadow");
		shadow.SetupGet(x => x.Names).Returns(["Shadow"]);
		shadow.SetupGet(x => x.Gameworld).Returns(world.Object);
		var planes = new All<IPlane>(); planes.Add(shadow.Object);
		world.SetupGet(x => x.Planes).Returns(planes);
		Assert.AreSame(shadow.Object, world.Object.Planes.GetByIdOrName("Shadow"));
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		actor.SetupGet(x => x.Type).Returns(ProgVariableTypes.Character);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		foreach (var fixedPermissions in new[] { false, true })
		{
			var prog = new MudSharp.FutureProg.FutureProg(world.Object, "PlanarPermissions", ProgVariableTypes.Boolean,
				[Tuple.Create(ProgVariableTypes.Character, "target")],
				"return applyplanarstate(@target, \"noncorporeal\", \"Shadow\"" + (fixedPermissions ? ", false, false, false)" : ")"));
			Assert.IsTrue(prog.Compile(), prog.CompileError);
			var executed = prog.ExecuteWithStatus(out var result, actor.Object);
			Assert.IsTrue(executed, $"Planar execution failed (fixed={fixedPermissions}); calls: {string.Join(", ", world.Invocations.Select(x => x.Method.Name))}");
			Assert.AreEqual(true, result, $"Planar result (fixed={fixedPermissions}); actor calls: {string.Join(", ", actor.Invocations.Select(x => x.Method.Name))}");
			var state = ((PlanarStateEffect)effects.Last()).PlanarPresenceDefinition;
			Assert.AreEqual(!fixedPermissions, state.CanCrossClosedDoors);
			Assert.AreEqual(!fixedPermissions, state.PlayerCanManifest);
			Assert.AreEqual(!fixedPermissions, state.PlayerCanDissipate);
			Assert.IsTrue(state.SuspendsPhysicalContact); Assert.IsTrue(state.VisibleToEtherealPerception);
			Assert.IsFalse(state.CanCrossMagicalBarriers);
		}
	}

	[TestMethod]
	public void StatusCreators_CreateSavedNativeEffectsWithoutSpellParentsAndDoNotStackOrRefresh()
	{
		var (actor, effects) = Actor();
		foreach (var kind in new[] { "blindness", "silence", "flight", "invisibility", "detectinvisible", "detectethereal", "detectmagick", "infravision", "sneak", "detectpoison", "climbing" })
		{
			var function = new AddStatusEffectFunction([new ConstantFunction(actor.Object)], kind);
			Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
			var effect = (IEffect)function.Result.GetObject;
			Assert.IsTrue(effect.SavingEffect);
			Assert.IsTrue(effects.Contains(effect));
			Assert.IsFalse(effect is IMagicSpellEffect, "Innate status must not have a missing spell parent.");
			var count = effects.Count;
			var timed = new AddStatusEffectFunction([new ConstantFunction(actor.Object), new ConstantFunction(new NumberVariable(10))], kind);
			timed.Execute(Mock.Of<IVariableSpace>());
			Assert.AreSame(effect, timed.Result.GetObject);
			Assert.AreEqual(count, effects.Count);
		}
		Assert.IsTrue(effects.Any(x => x is IBlindnessEffect));
		Assert.IsTrue(effects.Any(x => x is ISilencedEffect));
		Assert.IsTrue(effects.Any(x => x is IFlightEffect));
		Assert.IsTrue(effects.Any(x => x is IDarksightEffect));
		Assert.IsTrue(effects.Any(x => x is ISensePoisonEffect));
		Assert.IsTrue(effects.Any(x => x is IClimbingEffect));
		Assert.AreEqual(PerceptionTypes.DirectVisual, effects.OfType<PerceptionStatus>().Single(x => x.Key == "invisibility").Obscuring);
		Assert.IsFalse(effects.OfType<PerceptionStatus>().Single(x => x.Key == "invisibility").Obscuring.HasFlag(PerceptionTypes.Audible));
		Assert.AreEqual(PerceptionTypes.VisualMagical, effects.OfType<PerceptionStatus>().Single(x => x.Key == "detectinvisible").PerceptionGranting);
		Assert.AreEqual(PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal,
			effects.OfType<PerceptionStatus>().Single(x => x.Key == "detectethereal").PerceptionGranting);
		actor.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never,
			"An existing indefinite status must not be refreshed into a timed one.");
	}

	[TestMethod]
	public void StatusCreators_InvalidDurationOrNullTargetDoesNotMutateAndTimedEffectsUseNativeScheduler()
	{
		var (actor, effects) = Actor();
		foreach (var seconds in new[] { -1m, decimal.MaxValue })
		{
			var function = new AddStatusEffectFunction([new ConstantFunction(actor.Object), new ConstantFunction(new NumberVariable(seconds))], "blindness");
			function.Execute(Mock.Of<IVariableSpace>());
			Assert.IsNull(function.Result.GetObject); Assert.AreEqual(0, effects.Count);
		}
		var missing = new AddStatusEffectFunction([new ConstantFunction(new NullVariable(ProgVariableTypes.Character))], "blindness");
		missing.Execute(Mock.Of<IVariableSpace>()); Assert.IsNull(missing.Result.GetObject);
		var timed = new AddStatusEffectFunction([new ConstantFunction(actor.Object), new ConstantFunction(new NumberVariable(12.5m))], "blindness");
		timed.Execute(Mock.Of<IVariableSpace>());
		actor.Verify(x => x.AddEffect(It.IsAny<BlindnessStatus>(), TimeSpan.FromSeconds(12.5)), Times.Once);
	}

	[TestMethod]
	public void StatusEffects_RoundTripTheirNativeInterfacesAndSenseMasks()
	{
		BlindnessStatus.InitialiseEffectType(); SilenceStatus.InitialiseEffectType(); FlightStatus.InitialiseEffectType();
		PerceptionStatus.InitialiseEffectType(); InfravisionStatus.InitialiseEffectType(); Sneak.InitialiseEffectType();
		PoisonSenseStatus.InitialiseEffectType();
		ClimbingStatus.InitialiseEffectType(); PacifismStatus.InitialiseEffectType(); MovementTempoStatus.InitialiseEffectType(); CombatTempoStatus.InitialiseEffectType();
		SpottingAwarenessStatus.InitialiseEffectType(); ListeningAwarenessStatus.InitialiseEffectType();
		var (actor, _) = Actor();
		IEffect[] effects = [new BlindnessStatus(actor.Object), new SilenceStatus(actor.Object), new FlightStatus(actor.Object),
			new PerceptionStatus(actor.Object, "invisibility", PerceptionTypes.None, PerceptionTypes.DirectVisual),
			new PerceptionStatus(actor.Object, "detectethereal", PerceptionTypes.VisualEthereal | PerceptionTypes.SenseEthereal),
			new InfravisionStatus(actor.Object), new Sneak(actor.Object), new PoisonSenseStatus(actor.Object), new ClimbingStatus(actor.Object),
			new PacifismStatus(actor.Object), new MovementTempoStatus(actor.Object, 0.5), new CombatTempoStatus(actor.Object, 2.0),
			new SpottingAwarenessStatus(actor.Object, 10), new ListeningAwarenessStatus(actor.Object, 10)];
		foreach (var effect in effects)
		{
			var copy = Effect.LoadEffect(effect.SaveToXml(new Dictionary<IEffect, TimeSpan>()), actor.Object);
			Assert.AreEqual(effect.GetType(), copy.GetType());
			Assert.IsTrue(copy.SavingEffect);
			Assert.AreEqual(effect.PerceptionGranting, copy.PerceptionGranting);
			Assert.AreEqual(effect.Obscuring, copy.Obscuring);
			Assert.AreEqual(effect is IBlindnessEffect, copy is IBlindnessEffect);
			Assert.AreEqual(effect is IFlightEffect, copy is IFlightEffect);
			Assert.AreEqual(effect is ISilencedEffect, copy is ISilencedEffect);
		}
	}

	[TestMethod]
	public void CombatTempoProg_CreatesSavedMultiplierDeduplicatesAndRejectsInvalidInputs()
	{
		CombatTempoStatus.InitialiseEffectType();
		var (actor, effects) = Actor();
		AddCombatTempoEffectFunction Create(decimal multiplier) => new([new ConstantFunction(actor.Object), new ConstantFunction(new NumberVariable(multiplier))]);
		var doubled = Create(2m); doubled.Execute(Mock.Of<IVariableSpace>());
		var first = (CombatTempoStatus)doubled.Result.GetObject;
		Assert.AreEqual(2.0, first.Multiplier);
		Assert.IsTrue(first.SavingEffect);
		var repeated = Create(2m); repeated.Execute(Mock.Of<IVariableSpace>());
		Assert.AreSame(first, repeated.Result.GetObject);
		Assert.AreEqual(1, effects.OfType<CombatTempoStatus>().Count());
		foreach (var invalid in new[] { 0m, 0.09m, 10.01m, decimal.MaxValue })
		{
			var function = Create(invalid); function.Execute(Mock.Of<IVariableSpace>());
			Assert.IsNull(function.Result.GetObject);
		}
		Assert.AreEqual(1, effects.OfType<CombatTempoStatus>().Count());
		var copy = Effect.LoadEffect(first.SaveToXml(new Dictionary<IEffect, TimeSpan>()), actor.Object);
		Assert.IsInstanceOfType<CombatTempoStatus>(copy);
		Assert.AreEqual(2.0, ((CombatTempoStatus)copy).Multiplier);
	}

	[TestMethod]
	public void CombatDelay_AppliesOnlyApplicableCharacterTempoAfterNativeRecoveryAndLeavesNonCharactersAtBaseline()
	{
		var world = new Mock<IFuturemud>();
		var check = new Mock<ICheck>();
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		world.Setup(x => x.GetCheck(CheckType.CombatRecoveryCheck)).Returns(check.Object);
		var expression = new Mock<ITraitExpression>();
		expression.Setup(x => x.EvaluateWith(It.IsAny<IHaveTraits>(), null, TraitBonusContext.None,
			It.IsAny<(string Name, object Value)[]>())).Returns(3.0);
		var recovery = typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.NonPublic | BindingFlags.Static)!;
		var previousRecovery = recovery.GetValue(null);
		var previousSpeed = CombatBase.CombatSpeedMultiplier;
		recovery.SetValue(null, expression.Object);
		CombatBase.CombatSpeedMultiplier = 1.5;
		try
		{
			var character = new Mock<ICharacter>();
			var applicable = new Mock<ICombatDelayMultiplierEffect>();
			applicable.SetupGet(x => x.Multiplier).Returns(2.0);
			applicable.Setup(x => x.Applies()).Returns(true);
			var inapplicable = new Mock<ICombatDelayMultiplierEffect>();
			inapplicable.SetupGet(x => x.Multiplier).Returns(10.0);
			inapplicable.Setup(x => x.Applies()).Returns(false);
			character.Setup(x => x.CombinedEffectsOfType<ICombatDelayMultiplierEffect>())
				.Returns(new[] { applicable.Object, inapplicable.Object });
			var combat = new SimpleMeleeCombat(world.Object);
			var getDelay = typeof(CombatBase).GetMethod("GetCombatDelay", BindingFlags.NonPublic | BindingFlags.Instance)!;
			var characterDelay = (TimeSpan)getDelay.Invoke(combat, [character.Object, Difficulty.Normal, 2.0])!;
			Assert.AreEqual(18.0, characterDelay.TotalSeconds, 0.001,
				"Expected base time 2 x recovery expression 3 x global speed 1.5 x applicable tempo 2.");
			var nonCharacter = new Mock<IPerceiver>();
			var nonCharacterDelay = (TimeSpan)getDelay.Invoke(combat, [nonCharacter.Object, Difficulty.Normal, 2.0])!;
			Assert.AreEqual(10.0, nonCharacterDelay.TotalSeconds, 0.001);
		}
		finally
		{
			recovery.SetValue(null, previousRecovery);
			CombatBase.CombatSpeedMultiplier = previousSpeed;
		}
	}

	[TestMethod]
	public void TempoAndAwareness_UseOnlyTheirNativeMovementAndPerceptionContracts()
	{
		var (actor, effects) = Actor();
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		actor.Setup(x => x.Gameworld.GetStaticInt("CheckBonusPerDifficultyLevel")).Returns(10);
		foreach (var multiplier in new[] { 0.5m, 2m, 0.5m })
		{
			var tempo = new AddMovementTempoEffectFunction([new ConstantFunction(actor.Object), new ConstantFunction(new NumberVariable(multiplier))]);
			tempo.Execute(Mock.Of<IVariableSpace>()); Assert.IsNotNull(tempo.Result.GetObject);
		}
		Assert.AreEqual(2, effects.OfType<MovementTempoStatus>().Count());
		Assert.AreEqual(1.0, effects.OfType<MovementTempoStatus>().Aggregate(1.0, (value, effect) => value * effect.Multiplier));
		foreach (var listening in new[] { false, true })
		{
			var awareness = new AddAwarenessEffectFunction([new ConstantFunction(actor.Object)], listening);
			awareness.Execute(Mock.Of<IVariableSpace>());
			var effect = (SpottingAwarenessStatus)awareness.Result.GetObject;
			Assert.AreEqual(10.0, effect.CheckBonus);
			Assert.IsTrue(effect.AppliesToCheck(listening ? MudSharp.RPG.Checks.CheckType.GenericListenCheck : MudSharp.RPG.Checks.CheckType.SpotStealthCheck));
			Assert.IsFalse(effect.AppliesToCheck(MudSharp.RPG.Checks.CheckType.MeleeWeaponCheck));
			actor.SetupGet(x => x.State).Returns(CharacterState.Sleeping);
			Assert.IsFalse(effect.AppliesToCheck(listening ? MudSharp.RPG.Checks.CheckType.GenericListenCheck : MudSharp.RPG.Checks.CheckType.SpotStealthCheck));
			actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
		}
	}

	private static (Mock<ICharacter> Actor, List<IEffect> Effects) Actor()
	{
		var effects = new List<IEffect>();
		var world = new Mock<IFuturemud>(); world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.GetObject).Returns(actor.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		actor.SetupGet(x => x.Effects).Returns(effects);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(effects.Add);
		actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>())).Callback<IEffect, TimeSpan>((effect, _) => effects.Add(effect));
		return (actor, effects);
	}
}

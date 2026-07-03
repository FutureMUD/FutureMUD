#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Combat.Strategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic.Powers;
using MudSharp.Movement;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatStrategyRuntimeTests
{
	private static Mock<IFuturemud> CreateGameworld()
	{
		Mock<IFuturemud> gameworld = new();
		gameworld.Setup(x => x.GetStaticDouble("DodgeMoveStaminaCost")).Returns(10.0);
		gameworld.Setup(x => x.GetStaticDouble("DodgeRangeMoveStaminaCost")).Returns(1.0);
		gameworld.Setup(x => x.GetStaticDouble("StartClinchMoveStaminaCost")).Returns(1.0);
		gameworld.SetupGet(x => x.ManualCombatCommands).Returns(new All<IManualCombatCommand>());

		Mock<ICheck> check = new();
		check.Setup(x => x.TargetNumber(
				It.IsAny<IPerceivableHaveTraits>(),
				It.IsAny<Difficulty>(),
				It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(),
				It.IsAny<double>(),
				It.IsAny<(string Parameter, object value)[]>()))
			.Returns(50.0);
		gameworld.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);

		typeof(CombatBase)
			.GetProperty("GraceMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, new TraitExpression("1", gameworld.Object));
		typeof(CombatBase)
			.GetProperty("PowerMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, new TraitExpression("1", gameworld.Object));
		typeof(StandardCheck)
			.GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, 10);

		return gameworld;
	}

	private static Mock<ICharacter> CreateDefendingCharacter(IFuturemud gameworld)
	{
		Mock<IBody> body = new();
		body.SetupGet(x => x.WieldedItems).Returns(Array.Empty<IGameItem>());

		Mock<IRace> race = new();
		race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings
		{
			CanAttack = true,
			CanDefend = true,
			CanUseWeapons = true
		});

		Mock<ICharacterCombatSettings> combatSettings = new();

		Mock<ICharacter> character = new();
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.Race).Returns(race.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupProperty(x => x.State, CharacterState.Awake);
		character.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		character.SetupProperty(x => x.PreferredDefenseType, DefenseType.Dodge);
		character.SetupProperty(x => x.RidingMount, null);
		character.SetupProperty(x => x.CombatSettings, combatSettings.Object);
		character.SetupGet(x => x.Encumbrance).Returns(EncumbranceLevel.Unencumbered);
		character.SetupGet(x => x.EncumbrancePercentage).Returns(0.0);
		character.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
		character.Setup(x => x.CombinedEffectsOfType<IEffect>()).Returns(Array.Empty<IEffect>());
		character.Setup(x => x.EffectsOfType<ClinchEffect>(It.IsAny<Predicate<ClinchEffect>>()))
			.Returns(Array.Empty<ClinchEffect>());
		character.Setup(x => x.EffectsOfType<IWardBeatenEffect>(It.IsAny<Predicate<IWardBeatenEffect>>()))
			.Returns(Array.Empty<IWardBeatenEffect>());
		character.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		return character;
	}

	[TestMethod]
	public void StandardMelee_ResponseToStartClinch_CanDodgeWhenStandingAndHasStamina()
	{
		Mock<IFuturemud> gameworld = CreateGameworld();
		Mock<ICharacter> defender = CreateDefendingCharacter(gameworld.Object);
		Mock<ICharacter> attacker = CreateDefendingCharacter(gameworld.Object);
		StartClinchMove move = new(defender.Object) { Assailant = attacker.Object };

		ICombatMove response = StandardMeleeStrategy.Instance.ResponseToMove(move, defender.Object, attacker.Object);

		Assert.IsInstanceOfType(response, typeof(DodgeMove));
	}

	[TestMethod]
	public void StandardRange_ResponseToMagicPowerAttack_DelegatesToDodgeDefense()
	{
		Mock<IFuturemud> gameworld = CreateGameworld();
		Mock<ICharacter> defender = CreateDefendingCharacter(gameworld.Object);
		Mock<ICharacter> attacker = CreateDefendingCharacter(gameworld.Object);

		Mock<IDamageProfile> profile = new();
		profile.SetupProperty(x => x.BaseBlockDifficulty, Difficulty.Impossible);
		profile.SetupProperty(x => x.BaseParryDifficulty, Difficulty.Impossible);
		profile.SetupProperty(x => x.BaseDodgeDifficulty, Difficulty.Normal);

		Mock<IWeaponAttack> weaponAttack = new();
		weaponAttack.SetupGet(x => x.Profile).Returns(profile.Object);

		Mock<IMagicAttackPower> power = new();
		power.SetupGet(x => x.WeaponAttack).Returns(weaponAttack.Object);
		power.SetupGet(x => x.ValidDefenseTypes).Returns(new[] { DefenseType.Dodge });

		MagicPowerAttackMove move = new(attacker.Object, defender.Object, power.Object);

		ICombatMove response = StandardRangeStrategy.Instance.ResponseToMove(move, defender.Object, attacker.Object);

		Assert.IsInstanceOfType(response, typeof(DodgeMove));
	}

	[TestMethod]
	public void RangePathFunction_SwimOnly_RejectsAbjectFailureAndAllowsCapableSwimmers()
	{
		Mock<ICharacter> character = new();
		Mock<IFuturemud> gameworld = new();
		Mock<ICheck> swimCheck = new();
		Mock<IExit> exitData = new();
		Mock<ICellExit> exit = new();

		character.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		gameworld.Setup(x => x.GetCheck(CheckType.SwimStayAfloatCheck)).Returns(swimCheck.Object);
		exit.SetupGet(x => x.Exit).Returns(exitData.Object);
		exit.Setup(x => x.MovementTransition(character.Object))
			.Returns((CellMovementTransition.SwimOnly, RoomLayer.GroundLevel));
		Func<ICellExit, bool> predicate = StandardRangeStrategy.Instance.GetPathFunction(character.Object);

		swimCheck.Setup(x => x.WouldBeAbjectFailure(character.Object, null)).Returns(true);
		Assert.IsFalse(predicate(exit.Object));

		swimCheck.Setup(x => x.WouldBeAbjectFailure(character.Object, null)).Returns(false);
		Assert.IsTrue(predicate(exit.Object));
	}

	[TestMethod]
	public void RangePathFunction_FlyOnly_RequiresFlightCapability()
	{
		Mock<ICharacter> character = new();
		Mock<IExit> exitData = new();
		Mock<ICellExit> exit = new();

		character.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		exit.SetupGet(x => x.Exit).Returns(exitData.Object);
		exit.Setup(x => x.MovementTransition(character.Object))
			.Returns((CellMovementTransition.FlyOnly, RoomLayer.InAir));
		Func<ICellExit, bool> predicate = StandardRangeStrategy.Instance.GetPathFunction(character.Object);

		character.Setup(x => x.CanFly()).Returns((false, "cannot fly"));
		Assert.IsFalse(predicate(exit.Object));

		character.Setup(x => x.CanFly()).Returns((true, string.Empty));
		Assert.IsTrue(predicate(exit.Object));

		character.Object.PositionState = PositionFlying.Instance;
		character.Setup(x => x.CanFly()).Returns((false, "already airborne"));
		Assert.IsTrue(predicate(exit.Object));
	}

	[TestMethod]
	public void CombatStrategyModeExtensions_Swooper_IsValidMeleeAndRangedMode()
	{
		Assert.AreEqual("Swooper", CombatStrategyMode.Swooper.Describe());
		Assert.IsTrue(CombatStrategyMode.Swooper.DescribeWordy().Contains("swooping"));
		Assert.IsTrue(CombatStrategyMode.Swooper.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.Swooper.IsRangedStrategy());
		Assert.IsTrue(CombatStrategyMode.Swooper.IsRangedStartDesiringStrategy());
		Assert.IsFalse(CombatStrategyMode.Swooper.IsMeleeDesiredStrategy());
	}

	[TestMethod]
	public void CombatStrategyModeExtensions_NewPredatorModes_AreClassifiedForMeleeSelection()
	{
		Assert.AreEqual("Drowner", CombatStrategyMode.Drowner.Describe());
		Assert.AreEqual("Dropper", CombatStrategyMode.Dropper.Describe());
		Assert.AreEqual("Physical Avoider", CombatStrategyMode.PhysicalAvoider.Describe());
		Assert.AreEqual("Subdue", CombatStrategyMode.Subdue.Describe());

		Assert.IsTrue(CombatStrategyMode.Drowner.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.Dropper.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.PhysicalAvoider.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.Subdue.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.PhysicalAvoider.IsRangedStrategy());
		Assert.IsTrue(CombatStrategyMode.Drowner.IsMeleeDesiredStrategy());
		Assert.IsTrue(CombatStrategyMode.Dropper.IsMeleeDesiredStrategy());
		Assert.IsTrue(CombatStrategyMode.Subdue.IsMeleeDesiredStrategy());
		Assert.IsFalse(CombatStrategyMode.Subdue.IsRangedStrategy());
		Assert.IsTrue(CombatStrategyMode.PhysicalAvoider.IsRangedStartDesiringStrategy());
		Assert.IsInstanceOfType(CombatStrategyFactory.GetStrategy(CombatStrategyMode.Subdue), typeof(SubdueStrategy));
	}

	[TestMethod]
	public void CheckTypeExtensions_ForcedPositioningChecks_AreCombatPhysicalChecks()
	{
		foreach (var check in new[]
		         {
			         CheckType.PushbackCheck,
			         CheckType.OpposePushbackCheck,
			         CheckType.ForcedMovementCheck,
			         CheckType.OpposeForcedMovementCheck
		         })
		{
			Assert.IsTrue(check.IsPhysicalActivityCheck(), $"{check} should be a physical activity check.");
			Assert.IsTrue(check.IsTargettedHostileCheck(), $"{check} should be targeted and hostile.");
			Assert.IsTrue(check.IsVisionInfluencedCheck(), $"{check} should be vision influenced.");
		}

		Assert.IsTrue(CheckType.PushbackCheck.IsOffensiveCombatAction());
		Assert.IsTrue(CheckType.ForcedMovementCheck.IsOffensiveCombatAction());
		Assert.IsTrue(CheckType.OpposePushbackCheck.IsDefensiveCombatAction());
		Assert.IsTrue(CheckType.OpposeForcedMovementCheck.IsDefensiveCombatAction());
	}

	[TestMethod]
	public void CombatForcedMovementUtilities_ApplyPushback_ClearsMeleeAndSchedulesScaledDelay()
	{
		Mock<IScheduler> scheduler = new();
		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		gameworld.Setup(x => x.GetStaticDouble("PushbackCombatDelayBaseSeconds")).Returns(2.0);
		gameworld.Setup(x => x.GetStaticDouble("PushbackCombatDelayPerDegreeSeconds")).Returns(5.0);

		Mock<IBody> actorBody = new();
		Mock<IBody> targetBody = new();
		Mock<ICharacter> actor = new();
		Mock<ICharacter> target = new();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Body).Returns(actorBody.Object);
		actor.SetupProperty(x => x.MeleeRange, true);
		actor.SetupProperty(x => x.CombatTarget, target.Object);
		target.SetupGet(x => x.Body).Returns(targetBody.Object);
		target.SetupProperty(x => x.MeleeRange, true);

		CombatForcedMovementUtilities.ApplyPushback(actor.Object, target.Object, 3);

		Assert.IsFalse(actor.Object.MeleeRange);
		Assert.IsFalse(target.Object.MeleeRange);
		scheduler.Verify(x => x.DelayScheduleType(
			target.Object,
			ScheduleType.Combat,
			It.Is<TimeSpan>(delay => Math.Abs(delay.TotalSeconds - 17.0 * CombatBase.CombatSpeedMultiplier) < 0.001)),
			Times.Once);
	}

	[TestMethod]
	public void ForcedMovementMove_Description_UsesValidVerbParticiple()
	{
		Mock<ICharacter> attacker = new();
		Mock<ICharacter> target = new();
		Mock<IForcedMovementAttack> attack = new();

		ForcedMovementMove shove = new(attacker.Object, target.Object, attack.Object, ForcedMovementVerbs.Shove,
			RoomLayer.GroundLevel);
		ForcedMovementMove pull = new(attacker.Object, target.Object, attack.Object, ForcedMovementVerbs.Pull,
			RoomLayer.GroundLevel);

		StringAssert.StartsWith(shove.Description, "Shoving");
		StringAssert.StartsWith(pull.Description, "Pulling");
	}

	[TestMethod]
	public void ManualCombatResolver_AuxiliaryAction_ResolvesRaceMoveWithStamina()
	{
		Mock<IFuturemud> gameworld = CreateGameworld();
		Mock<IAuxiliaryCombatAction> action = new();
		Mock<IManualCombatCommand> command = new();
		Mock<IRace> race = new();
		Mock<ICombat> combat = new();
		Mock<ICharacter> actor = new();
		Mock<ICharacter> target = new();

		action.SetupGet(x => x.Id).Returns(42);
		action.SetupGet(x => x.Name).Returns("Bash");
		action.SetupGet(x => x.StaminaCost).Returns(3.0);
		command.SetupGet(x => x.ActionKind).Returns(ManualCombatActionKind.AuxiliaryAction);
		command.SetupGet(x => x.AuxiliaryAction).Returns(action.Object);
		command.Setup(x => x.IsUsableBy(actor.Object, target.Object)).Returns(true);
		race.Setup(x => x.UsableAuxiliaryMoves(actor.Object, target.Object, false))
		    .Returns(new[] { action.Object });
		combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[] { actor.Object, target.Object });
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Race).Returns(race.Object);
		actor.SetupProperty(x => x.Combat, combat.Object);
		actor.SetupProperty(x => x.CombatTarget, null);
		actor.SetupGet(x => x.Encumbrance).Returns(EncumbranceLevel.Unencumbered);
		actor.SetupGet(x => x.EncumbrancePercentage).Returns(0.0);
		actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);

		ManualCombatMoveResolution result =
			ManualCombatCommandResolver.TryResolve(actor.Object, command.Object, target.Object);

		Assert.IsTrue(result.Success, result.Error);
		Assert.AreEqual("AuxiliaryMove", result.Move.GetType().Name);
		Assert.AreSame(target.Object, actor.Object.CombatTarget);
		actor.Verify(x => x.CanSpendStamina(It.Is<double>(value => value > 0.0)), Times.Once);
	}

	[TestMethod]
	public void ManualCombatResolver_AuxiliaryAction_RejectsMissingActionAndCanReturnExhaustedMove()
	{
		Mock<IFuturemud> gameworld = CreateGameworld();
		Mock<IAuxiliaryCombatAction> action = new();
		Mock<IManualCombatCommand> command = new();
		Mock<IRace> race = new();
		Mock<ICombat> combat = new();
		Mock<ICharacter> actor = new();
		Mock<ICharacter> target = new();
		Mock<ICharacter> originalTarget = new();

		action.SetupGet(x => x.Id).Returns(42);
		action.SetupGet(x => x.StaminaCost).Returns(3.0);
		command.SetupGet(x => x.ActionKind).Returns(ManualCombatActionKind.AuxiliaryAction);
		command.SetupGet(x => x.AuxiliaryAction).Returns(action.Object);
		command.Setup(x => x.IsUsableBy(actor.Object, target.Object)).Returns(true);
		combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[] { actor.Object, target.Object });
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Race).Returns(race.Object);
		actor.SetupProperty(x => x.Combat, combat.Object);
		actor.SetupProperty(x => x.CombatTarget, originalTarget.Object);
		actor.SetupGet(x => x.Encumbrance).Returns(EncumbranceLevel.Unencumbered);
		actor.SetupGet(x => x.EncumbrancePercentage).Returns(0.0);

		race.Setup(x => x.UsableAuxiliaryMoves(actor.Object, target.Object, false))
		    .Returns(Array.Empty<IAuxiliaryCombatAction>());
		ManualCombatMoveResolution unavailable =
			ManualCombatCommandResolver.TryResolve(actor.Object, command.Object, target.Object);
		Assert.IsFalse(unavailable.Success);
		StringAssert.Contains(unavailable.Error, "Your race cannot use that auxiliary move");
		Assert.AreSame(originalTarget.Object, actor.Object.CombatTarget);

		race.Setup(x => x.UsableAuxiliaryMoves(actor.Object, target.Object, false))
		    .Returns(new[] { action.Object });
		actor.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(false);

		ManualCombatMoveResolution playerAttempt =
			ManualCombatCommandResolver.TryResolve(actor.Object, command.Object, target.Object);
		Assert.IsFalse(playerAttempt.Success);
		StringAssert.Contains(playerAttempt.Error, "too exhausted");
		Assert.AreSame(originalTarget.Object, actor.Object.CombatTarget);

		ManualCombatMoveResolution selectedActionAttempt =
			ManualCombatCommandResolver.TryResolve(actor.Object, command.Object, target.Object, true);
		Assert.IsTrue(selectedActionAttempt.Success, selectedActionAttempt.Error);
		Assert.IsInstanceOfType(selectedActionAttempt.Move, typeof(TooExhaustedMove));
		Assert.AreSame(target.Object, actor.Object.CombatTarget);
	}

	[TestMethod]
	public void AuxiliaryCombatActionSource_LoadsAndDocumentsExpandedEffectTypes()
	{
		string actionSource = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryCombatAction.cs"));
		string baseEffectSource = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "OpposedAuxiliaryEffectBase.cs"));

		foreach (string effectType in new[] { "targetdelay", "facing", "targetstamina", "positionchange", "disarm" })
		{
			StringAssert.Contains(actionSource, $"case \"{effectType}\"");
		}

		Dictionary<string, string> effectFiles = new(StringComparer.OrdinalIgnoreCase)
		{
			["targetdelay"] = "TargetDelay.cs",
			["facing"] = "FacingChange.cs",
			["targetstamina"] = "TargetStamina.cs",
			["positionchange"] = "PositionChange.cs",
			["disarm"] = "Disarm.cs"
		};
		foreach ((string effectType, string fileName) in effectFiles)
		{
			StringAssert.Contains(
				File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", fileName)),
				$"RegisterBuilderParser(\"{effectType}\"");
		}

		foreach (string sharedCommand in new[]
		         {
			         "trait", "difficulty", "minimum", "amount", "perdegree", "max", "successecho",
			         "failureecho", "clearecho"
		         })
		{
			StringAssert.Contains(baseEffectSource, $"case \"{sharedCommand}\"");
		}

		foreach (string attributeName in new[]
		         {
			         "defensetrait", "defensedifficulty", "minimumdegree", "flatamount",
			         "perdegreeamount", "maximumamount", "successecho", "failureecho"
		         })
		{
			StringAssert.Contains(baseEffectSource, $"\"{attributeName}\"");
		}
	}

	[TestMethod]
	public void AuxiliaryEffectSources_DefaultsAndStateChangingFlows_ArePresent()
	{
		string delay = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "TargetDelay.cs"));
		string stamina = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "TargetStamina.cs"));
		string facing = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "FacingChange.cs"));
		string position = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "PositionChange.cs"));
		string disarm = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "Disarm.cs"));

		StringAssert.Contains(delay, "1.5, 0.5, 6.0");
		StringAssert.Contains(delay, "DelayScheduleType(tch, ScheduleType.Combat");
		StringAssert.Contains(stamina, "3.0, 1.0, 10.0");
		StringAssert.Contains(stamina, "tch.SpendStamina(amount)");
		StringAssert.Contains(facing, "1.0, 0.0, 1.0");
		StringAssert.Contains(facing, "CombatPositioningUtilities.ImproveCombatPosition");
		StringAssert.Contains(facing, "CombatPositioningUtilities.WorsenCombatPosition");
		StringAssert.Contains(position, "1.5, 0.5, 5.0");
		StringAssert.Contains(position, "tch.DoCombatKnockdown()");
		StringAssert.Contains(position, "tch.SetPosition(Position, PositionModifier.None, null, null)");
		StringAssert.Contains(disarm, "90.0, 0.0, 90.0");
		StringAssert.Contains(disarm, "DisarmSelection.Best");
		StringAssert.Contains(disarm, "target.Body.WieldedItems");
		StringAssert.Contains(disarm, "new CombatNoGetEffect(item, tch.Combat)");
		StringAssert.Contains(disarm, "new CombatGetItemEffect(tch, item)");
	}

	[TestMethod]
	public void AuxiliaryAdvantageSources_DefenderTypeAndNumericParsingBugsStayFixed()
	{
		string attacker = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "AttackerAdvantage.cs"));
		string defender = File.ReadAllText(GetCoreSourcePath("Combat", "AuxiliaryEffects", "DefenderAdvantage.cs"));

		StringAssert.Contains(defender, "RegisterBuilderParser(\"defenderadvantage\"");
		StringAssert.Contains(defender, "return new DefenderAdvantage(");
		StringAssert.Contains(defender, "new XAttribute(\"defensedifficulty\", (int)DefenseDifficulty)");
		StringAssert.Contains(defender, "Defender Advantage Effect");
		Assert.IsFalse(attacker.Contains("if (double.TryParse"), "Attacker advantage builder should reject only invalid numeric bonuses.");
		Assert.IsFalse(defender.Contains("if (double.TryParse"), "Defender advantage builder should reject only invalid numeric bonuses.");
	}

	[TestMethod]
	public void CombatStrategySources_AuxiliaryPercentageFeedsSharedMeleeAndRangedSelection()
	{
		string strategyBase = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "StrategyBase.cs"));
		string melee = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "StandardMeleeStrategy.cs"));
		string ranged = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "RangeBaseStrategy.cs"));

		StringAssert.Contains(strategyBase, "protected virtual ICombatMove AttemptUseAuxilliaryAction");
		StringAssert.Contains(strategyBase, "UsableAuxiliaryMoves(combatant, tch, false)");
		StringAssert.Contains(strategyBase, "return new AuxiliaryMove(combatant, tch, move)");
		StringAssert.Contains(melee, "combatant.CombatSettings.AuxiliaryPercentage > 0.0");
		StringAssert.Contains(melee, "roll <= combatant.CombatSettings.AuxiliaryPercentage");
		StringAssert.Contains(melee, "return AttemptUseAuxilliaryAction(combatant);");
		StringAssert.Contains(ranged, "combatant.CombatSettings.AuxiliaryPercentage > 0.0");
		StringAssert.Contains(ranged, "roll <= combatant.CombatSettings.AuxiliaryPercentage");
		StringAssert.Contains(ranged, "return AttemptUseAuxilliaryAction(combatant);");
		Assert.IsTrue(
			melee.LastIndexOf("return AttemptUseAuxilliaryAction(combatant);", StringComparison.Ordinal) >
			melee.LastIndexOf("roll <= combatant.CombatSettings.AuxiliaryPercentage", StringComparison.Ordinal),
			"Standard melee should retain its legacy auxiliary fallback after the weighted roll.");
		Assert.IsTrue(
			ranged.LastIndexOf("return null;", StringComparison.Ordinal) >
			ranged.LastIndexOf("roll <= combatant.CombatSettings.AuxiliaryPercentage", StringComparison.Ordinal),
			"Ranged strategies should keep their no-move fallback if no weighted auxiliary roll is selected.");
	}

	[TestMethod]
	public void ManualCombatCommandSources_RegisterQueueResolveAndCooldownThroughSharedPath()
	{
		string contract = File.ReadAllText(GetSourcePath("FutureMUDLibrary", "Combat", "IManualCombatCommand.cs"));
		string registry = File.ReadAllText(GetCoreSourcePath("Combat", "ManualCombatCommandRegistry.cs"));
		string resolver = File.ReadAllText(GetCoreSourcePath("Combat", "ManualCombatCommandResolver.cs"));
		string selected = File.ReadAllText(GetCoreSourcePath("Effects", "Concrete", "SelectedCombatAction.cs"));
		string module = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "CombatModule.cs"));
		string loader = File.ReadAllText(GetCoreSourcePath("Framework", "FuturemudLoaders.cs"));

		StringAssert.Contains(contract, "ManualCombatActionKind");
		StringAssert.Contains(contract, "WeaponAttack = 0");
		StringAssert.Contains(contract, "AuxiliaryAction = 1");
		StringAssert.Contains(registry, "HasReservedCommandCollision");
		StringAssert.Contains(registry, "primary verb");
		StringAssert.Contains(registry, "collides with an existing command");
		StringAssert.Contains(registry, "duplicates");
		StringAssert.Contains(registry, "CombatModule.ManualCombatGeneric");
		StringAssert.Contains(registry, "CommandDisplayOptions.DisplayCommandWords");
		StringAssert.Contains(loader, "LoadManualCombatCommands();");
		StringAssert.Contains(loader, "ManualCombatCommandRegistry.Rebuild(this);");

		StringAssert.Contains(module, "public static void ManualCombatGeneric");
		StringAssert.Contains(module, "target = actor.CombatTarget as ICharacter");
		StringAssert.Contains(module, "actor.TargetActor(ss.PopSpeech(), PerceiveIgnoreFlags.IgnoreSelf)");
		StringAssert.Contains(module, "ManualCombatCommandResolver.TryResolve(actor, manualCommand, target)");
		StringAssert.Contains(module, "SelectedCombatAction.GetEffectManualCombatCommand(actor, manualCommand, target)");
		StringAssert.Contains(module, "new CommandDelay(actor, manualCommand.CommandWords, manualCommand.CooldownMessage)");

		StringAssert.Contains(selected, "internal class ManualCombatCommandAction");
		StringAssert.Contains(selected, "ManualCombatCommandResolver.TryResolve(actor, Command, Target, true)");
		StringAssert.Contains(selected, "GetEffectManualCombatCommand");
		StringAssert.Contains(resolver, "if (result.Success)");
		StringAssert.Contains(resolver, "actor.CombatTarget = target");
		StringAssert.Contains(resolver, "actor.Race.UsableAuxiliaryMoves(actor, target, false)");
		StringAssert.Contains(resolver, "new AuxiliaryMove(actor, target, available)");
		StringAssert.Contains(resolver, "AuxiliaryMove.MoveStaminaCost(actor, available)");
		StringAssert.Contains(resolver, "WeaponSourcesInPreferenceOrder(actor)");
		StringAssert.Contains(resolver, "CombatMoveFactory.CreateWeaponAttack");
		StringAssert.Contains(resolver, "CombatMoveFactory.CreateNaturalWeaponAttack");
	}

	[TestMethod]
	public void ManualCombatCommandSources_AiWeightingMultipliesExistingStrategyBuckets()
	{
		string settingsContract = File.ReadAllText(GetSourcePath("FutureMUDLibrary", "Combat", "ICharacterCombatSettings.cs"));
		string settings = File.ReadAllText(GetCoreSourcePath("Combat", "CharacterCombatSettings.cs"));
		string module = File.ReadAllText(GetCoreSourcePath("Commands", "Modules", "CombatModule.cs"));
		string strategyBase = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "StrategyBase.cs"));
		string melee = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "StandardMeleeStrategy.cs"));
		string ranged = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "RangeBaseStrategy.cs"));
		string avoider = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "PhysicalAvoiderStrategy.cs"));
		string subdue = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "SubdueStrategy.cs"));
		string clinch = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "ClinchStrategy.cs"));
		string swooper = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "SwooperStrategy.cs"));

		StringAssert.Contains(settingsContract, "ManualCombatCommandPreferences");
		StringAssert.Contains(settingsContract, "ManualCombatCommandWeightMultiplier");
		StringAssert.Contains(settings, "CharacterCombatSettingsManualCombatCommands");
		StringAssert.Contains(settings, "command.DefaultAiWeightMultiplier");
		StringAssert.Contains(settings, "_manualCombatCommandPreferences[command.Id] = Math.Max(0.0, multiplier)");
		StringAssert.Contains(module, "CombatConfigManual");
		StringAssert.Contains(module, "SetManualCombatCommandWeightMultiplier");
		StringAssert.Contains(module, "ClearManualCombatCommandWeightMultiplier");
		StringAssert.Contains(strategyBase, "AuxiliaryMove.MoveStaminaCost(combatant, x)");
		StringAssert.Contains(strategyBase, "ManualCombatCommandResolver.AiWeightMultiplier(combatant, x)");
		StringAssert.Contains(strategyBase, "List<IAuxiliaryCombatAction> weightedMoves");
		StringAssert.Contains(strategyBase, "return weightedMoves.Any() ? new TooExhaustedMove");
		StringAssert.Contains(melee, "combatant.CombatSettings.WeaponUsePercentage");
		StringAssert.Contains(melee, "combatant.CombatSettings.NaturalWeaponPercentage");
		StringAssert.Contains(melee, "combatant.CombatSettings.AuxiliaryPercentage");
		StringAssert.Contains(ranged, "combatant.CombatSettings.AuxiliaryPercentage");

		foreach (string source in new[] { strategyBase, melee, avoider, subdue, clinch, swooper })
		{
			StringAssert.Contains(source, "ManualCombatCommandResolver.AiWeightMultiplier");
			StringAssert.Contains(source, "GetWeightedRandom");
		}

		Assert.IsFalse(module.Contains("ManualCombatPercentage", StringComparison.Ordinal),
			"Manual combat commands should not add a separate strategy percentage bucket.");
		Assert.IsFalse(settingsContract.Contains("ManualCombatPercentage", StringComparison.Ordinal),
			"Manual combat commands should stay as multipliers over existing action categories.");
	}

	[TestMethod]
	public void SubdueStrategySources_ControlAndGrapplePriorities_ArePresent()
	{
		string subdue = File.ReadAllText(GetCoreSourcePath("Combat", "Strategies", "SubdueStrategy.cs"));

		StringAssert.Contains(subdue, "CombatStrategyMode.Subdue");
		StringAssert.Contains(subdue, "GrappleForControlStrategy.Instance.AttemptGrappleForControlOnly");
		StringAssert.Contains(subdue, "IsSecondaryCombatant");
		StringAssert.Contains(subdue, "IsTargetDisadvantaged");
		StringAssert.Contains(subdue, "IsTargetInjured");
		StringAssert.Contains(subdue, "BuiltInCombatMoveType.StaggeringBlow");
		StringAssert.Contains(subdue, "BuiltInCombatMoveType.UnbalancingBlow");
		StringAssert.Contains(subdue, "CombatMoveIntentions.Disarm");
		StringAssert.Contains(subdue, "UsableAuxiliaryMoves");
		Assert.IsFalse(subdue.Contains("GrappleForControlStrategy.Instance.ChooseMove", StringComparison.Ordinal));
	}

	[TestMethod]
	public void SubdueStrategy_ChooseMove_SecondaryTargetStartsClinchForControl()
	{
		var normalAttack = CreateWeaponAttack(BuiltInCombatMoveType.UseWeaponAttack, CombatMoveIntentions.None);
		var scenario = CreateSubdueMoveSelectionScenario(new[] { normalAttack.Object }, 1.0, _ => true);

		ICombatMove? move = SubdueStrategy.Instance.ChooseMove(scenario.Actor.Object);

		Assert.IsInstanceOfType(move, typeof(StartClinchMove));
	}

	[TestMethod]
	public void SubdueStrategy_ChooseMove_FailedControlUsesSubdualWeaponAttack()
	{
		var staggeringAttack = CreateWeaponAttack(BuiltInCombatMoveType.StaggeringBlow, CombatMoveIntentions.None);
		var scenario = CreateSubdueMoveSelectionScenario(new[] { staggeringAttack.Object }, 1000.0, value => value < 100.0);

		ICombatMove? move = SubdueStrategy.Instance.ChooseMove(scenario.Actor.Object);

		Assert.IsInstanceOfType(move, typeof(StaggeringBlowMove));
	}

	[TestMethod]
	public void SubdueStrategy_ChooseMove_FailedControlDoesNotFallBackToOrdinaryMelee()
	{
		var normalAttack = CreateWeaponAttack(BuiltInCombatMoveType.UseWeaponAttack, CombatMoveIntentions.None);
		var scenario = CreateSubdueMoveSelectionScenario(new[] { normalAttack.Object }, 1000.0, value => value < 100.0);

		ICombatMove? move = SubdueStrategy.Instance.ChooseMove(scenario.Actor.Object);

		Assert.IsNull(move);
	}

	private static Mock<IWeaponAttack> CreateWeaponAttack(BuiltInCombatMoveType moveType,
		CombatMoveIntentions intentions)
	{
		Mock<IWeaponAttack> attack = new();
		attack.SetupGet(x => x.Id).Returns((long)moveType);
		attack.SetupGet(x => x.MoveType).Returns(moveType);
		attack.SetupGet(x => x.Intentions).Returns(intentions);
		attack.SetupGet(x => x.StaminaCost).Returns(1.0);
		attack.SetupGet(x => x.Weighting).Returns(1.0);
		attack.SetupGet(x => x.BaseDelay).Returns(1.0);
		attack.SetupGet(x => x.RecoveryDifficultyFailure).Returns(Difficulty.Normal);
		attack.SetupGet(x => x.RecoveryDifficultySuccess).Returns(Difficulty.Easy);
		attack.SetupGet(x => x.ExertionLevel).Returns(ExertionLevel.Normal);
		return attack;
	}

	private static (Mock<ICharacter> Actor, Mock<ICharacter> Target) CreateSubdueMoveSelectionScenario(
		IEnumerable<IWeaponAttack> weaponAttacks,
		double startClinchCost,
		Func<double, bool> canSpendStamina)
	{
		var attackList = weaponAttacks.ToList();
		Mock<IFuturemud> gameworld = CreateGameworld();
		gameworld.Setup(x => x.GetStaticDouble("StartClinchMoveStaminaCost")).Returns(startClinchCost);

		Mock<ICombat> combat = new();
		combat.SetupGet(x => x.Friendly).Returns(false);

		Mock<ICharacterCombatSettings> settings = new();
		settings.SetupGet(x => x.InventoryManagement).Returns(AutomaticInventorySettings.FullyManual);
		settings.SetupGet(x => x.ManualPositionManagement).Returns(true);
		settings.SetupGet(x => x.MinimumStaminaToAttack).Returns(0.0);
		settings.SetupGet(x => x.AttackHelpless).Returns(true);
		settings.SetupGet(x => x.AttackCriticallyInjured).Returns(true);
		settings.SetupGet(x => x.AttackDisarmed).Returns(true);
		settings.SetupGet(x => x.WeaponUsePercentage).Returns(1.0);
		settings.SetupGet(x => x.NaturalWeaponPercentage).Returns(0.0);
		settings.SetupGet(x => x.AuxiliaryPercentage).Returns(0.0);
		settings.SetupGet(x => x.MagicUsePercentage).Returns(0.0);
		settings.SetupGet(x => x.PsychicUsePercentage).Returns(0.0);
		settings.SetupGet(x => x.FallbackToUnarmedIfNoWeapon).Returns(false);
		settings.SetupGet(x => x.RequiredIntentions).Returns(CombatMoveIntentions.None);
		settings.SetupGet(x => x.ForbiddenIntentions).Returns(CombatMoveIntentions.None);
		settings.SetupGet(x => x.PreferredIntentions).Returns(CombatMoveIntentions.None);
		settings.SetupGet(x => x.MeleeAttackOrderPreferences)
		        .Returns(new List<MeleeAttackOrderPreference> { MeleeAttackOrderPreference.Weapon });
		settings.Setup(x => x.ManualCombatCommandWeightMultiplier(It.IsAny<IManualCombatCommand>()))
		        .Returns(1.0);

		Mock<IRace> race = new();
		race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings
		{
			CanAttack = true,
			CanDefend = true,
			CanUseWeapons = true
		});
		race.Setup(x => x.UsableAuxiliaryMoves(It.IsAny<ICharacter>(), It.IsAny<IPerceiver>(), false))
		    .Returns(Array.Empty<IAuxiliaryCombatAction>());
		race.Setup(x => x.UsableNaturalWeaponAttacks(It.IsAny<ICharacter>(), It.IsAny<IPerceiver>(), false,
			      It.IsAny<BuiltInCombatMoveType[]>()))
		    .Returns(Array.Empty<INaturalAttack>());

		Mock<IGameItem> weaponItem = new();
		Mock<IMeleeWeapon> weapon = new();
		Mock<IWeaponType> weaponType = new();
		weaponItem.Setup(x => x.GetItemType<IMeleeWeapon>()).Returns(weapon.Object);
		weaponItem.Setup(x => x.IsItemType<IMeleeWeapon>()).Returns(true);
		weapon.SetupGet(x => x.Parent).Returns(weaponItem.Object);
		weapon.SetupGet(x => x.WeaponType).Returns(weaponType.Object);
		weapon.SetupGet(x => x.Classification).Returns(WeaponClassification.NonLethal);
		weaponType.Setup(x => x.UsableAttacks(
				It.IsAny<IPerceiver>(),
				It.IsAny<IGameItem>(),
				It.IsAny<IPerceiver>(),
				It.IsAny<AttackHandednessOptions>(),
				false,
				It.IsAny<BuiltInCombatMoveType[]>()))
			.Returns((IPerceiver attacker, IGameItem item, IPerceiver attackTarget,
				AttackHandednessOptions handedness, bool ignorePosition, BuiltInCombatMoveType[] types) =>
				attackList.Where(x => types.Contains(x.MoveType)));

		Mock<IBody> actorBody = new();
		actorBody.SetupGet(x => x.WieldedItems).Returns(new[] { weaponItem.Object });
		actorBody.SetupGet(x => x.HeldOrWieldedItems).Returns(new[] { weaponItem.Object });
		actorBody.SetupGet(x => x.Implants).Returns(Array.Empty<IImplant>());
		actorBody.SetupGet(x => x.Prosthetics).Returns(Array.Empty<IProsthetic>());
		actorBody.Setup(x => x.WieldedHandCount(weaponItem.Object)).Returns(1);
		actorBody.Setup(x => x.EffectsOfType<IPacifismEffect>(It.IsAny<Predicate<IPacifismEffect>>()))
		         .Returns(Array.Empty<IPacifismEffect>());

		Mock<IBody> targetBody = new();
		targetBody.SetupGet(x => x.HeldOrWieldedItems).Returns(Array.Empty<IGameItem>());
		targetBody.Setup(x => x.EffectsOfType<ILimbIneffectiveEffect>(It.IsAny<Predicate<ILimbIneffectiveEffect>>()))
		          .Returns(Array.Empty<ILimbIneffectiveEffect>());

		Mock<ICharacter> actor = new();
		Mock<ICharacter> target = new();
		Mock<ICharacter> ally = new();

		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Race).Returns(race.Object);
		actor.SetupGet(x => x.Body).Returns(actorBody.Object);
		actor.SetupProperty(x => x.CombatSettings, settings.Object);
		actor.SetupProperty(x => x.State, CharacterState.Awake);
		actor.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		actor.SetupProperty(x => x.CombatTarget, target.Object);
		actor.SetupProperty(x => x.Combat, combat.Object);
		actor.SetupProperty(x => x.MeleeRange, true);
		actor.SetupGet(x => x.CurrentStamina).Returns(100.0);
		actor.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
		actor.SetupGet(x => x.Encumbrance).Returns(EncumbranceLevel.Unencumbered);
		actor.SetupGet(x => x.EncumbrancePercentage).Returns(0.0);
		actor.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
		actor.Setup(x => x.CombinedEffectsOfType<IEffect>()).Returns(Array.Empty<IEffect>());
		actor.Setup(x => x.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers)).Returns(CanMoveResponse.True);
		actor.Setup(x => x.CanSpendStamina(It.IsAny<double>()))
		     .Returns((double value) => canSpendStamina(value));
		actor.Setup(x => x.ColocatedWith(target.Object)).Returns(true);
		SetupEmptyCombatEffects(actor);

		target.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		target.SetupGet(x => x.Race).Returns(race.Object);
		target.SetupGet(x => x.Body).Returns(targetBody.Object);
		target.SetupProperty(x => x.CombatSettings, settings.Object);
		target.SetupProperty(x => x.State, CharacterState.Awake);
		target.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		target.SetupProperty(x => x.CombatTarget, ally.Object);
		target.SetupProperty(x => x.MeleeRange, true);
		target.SetupGet(x => x.IsHelpless).Returns(false);
		target.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
		target.Setup(x => x.EffectsOfType<ClinchEffect>(It.IsAny<Predicate<ClinchEffect>>()))
		      .Returns(Array.Empty<ClinchEffect>());

		combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[] { actor.Object, target.Object, ally.Object });
		return (actor, target);
	}

	private static void SetupEmptyCombatEffects(Mock<ICharacter> character)
	{
		character.Setup(x => x.EffectsOfType<SelectedCombatAction>(It.IsAny<Predicate<SelectedCombatAction>>()))
		         .Returns(Array.Empty<SelectedCombatAction>());
		character.Setup(x => x.EffectsOfType<Rescue>(It.IsAny<Predicate<Rescue>>()))
		         .Returns(Array.Empty<Rescue>());
		character.Setup(x => x.EffectsOfType<IGuardCharacterEffect>(It.IsAny<Predicate<IGuardCharacterEffect>>()))
		         .Returns(Array.Empty<IGuardCharacterEffect>());
		character.Setup(x => x.EffectsOfType<ICombatGetItemEffect>(It.IsAny<Predicate<ICombatGetItemEffect>>()))
		         .Returns(Array.Empty<ICombatGetItemEffect>());
		character.Setup(x => x.EffectsOfType<ClinchEffect>(It.IsAny<Predicate<ClinchEffect>>()))
		         .Returns(Array.Empty<ClinchEffect>());
		character.Setup(x => x.EffectsOfType<ClinchCooldown>(It.IsAny<Predicate<ClinchCooldown>>()))
		         .Returns(Array.Empty<ClinchCooldown>());
		character.Setup(x => x.EffectsOfType<IGrappling>(It.IsAny<Predicate<IGrappling>>()))
		         .Returns(Array.Empty<IGrappling>());
		character.Setup(x => x.EffectsOfType<CombatCoupDeGrace>(It.IsAny<Predicate<CombatCoupDeGrace>>()))
		         .Returns(Array.Empty<CombatCoupDeGrace>());
		character.Setup(x => x.EffectsOfType<FixedCombatMoveType>(It.IsAny<Predicate<FixedCombatMoveType>>()))
		         .Returns(Array.Empty<FixedCombatMoveType>());
	}

	[TestMethod]
	public void SubdueStrategy_ShouldGrappleForControl_WhenTargetIsSecondaryDisadvantagedOrInjured()
	{
		Mock<ICharacter> actor = new();
		actor.Setup(x => x.EffectsOfType<IGrappling>(It.IsAny<Predicate<IGrappling>>()))
		     .Returns(Array.Empty<IGrappling>());

		Mock<IBody> body = new();
		body.Setup(x => x.EffectsOfType<ILimbIneffectiveEffect>(It.IsAny<Predicate<ILimbIneffectiveEffect>>()))
		    .Returns(Array.Empty<ILimbIneffectiveEffect>());

		Mock<IHealthStrategy> health = new();
		IEnumerable<IWound> wounds = Array.Empty<IWound>();

		Mock<ICharacter> target = new();
		target.SetupGet(x => x.Body).Returns(body.Object);
		target.SetupGet(x => x.HealthStrategy).Returns(health.Object);
		target.SetupGet(x => x.Wounds).Returns(() => wounds);
		target.SetupGet(x => x.IsHelpless).Returns(false);
		target.SetupProperty(x => x.CombatTarget, actor.Object);
		target.SetupProperty(x => x.State, CharacterState.Awake);
		target.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		target.SetupProperty(x => x.DefensiveAdvantage, 0.0);
		target.SetupProperty(x => x.OffensiveAdvantage, 0.0);
		target.Setup(x => x.EffectsOfType<Staggered>(It.IsAny<Predicate<Staggered>>()))
		      .Returns(Array.Empty<Staggered>());
		health.Setup(x => x.IsCriticallyInjured(target.Object)).Returns(false);

		Assert.IsFalse(SubdueStrategy.ShouldGrappleForControl(actor.Object, target.Object));

		Mock<ICharacter> ally = new();
		target.Object.CombatTarget = ally.Object;
		Assert.IsTrue(SubdueStrategy.ShouldGrappleForControl(actor.Object, target.Object));

		target.Object.CombatTarget = actor.Object;
		target.Object.DefensiveAdvantage = -1.0;
		Assert.IsTrue(SubdueStrategy.ShouldGrappleForControl(actor.Object, target.Object));

		target.Object.DefensiveAdvantage = 0.0;
		Mock<IWound> wound = new();
		wound.SetupGet(x => x.CurrentDamage).Returns(1.0);
		wounds = new[] { wound.Object };
		Assert.IsTrue(SubdueStrategy.ShouldGrappleForControl(actor.Object, target.Object));
	}

	private static string GetSourcePath(params string[] segments)
	{
		return Path.GetFullPath(Path.Combine(
			new[]
			{
				AppContext.BaseDirectory,
				"..",
				"..",
				"..",
				".."
			}.Concat(segments).ToArray()));
	}

	private static string GetCoreSourcePath(params string[] segments)
	{
		return Path.GetFullPath(Path.Combine(
			new[]
			{
				AppContext.BaseDirectory,
				"..",
				"..",
				"..",
				"..",
				"MudSharpCore"
			}.Concat(segments).ToArray()));
	}
}

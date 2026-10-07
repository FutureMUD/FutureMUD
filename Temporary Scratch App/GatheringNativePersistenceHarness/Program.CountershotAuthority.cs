#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using ExpressionEngine;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Movement;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed class CountershotFollowOn(ICharacter actor, ITraitDefinition trait) : Effect(actor), ICombatTacticEffect
	{
		internal int Calls { get; private set; }
		protected override string SpecificEffectType => "HarnessCountershotFollowOn";
		public override string Describe(IPerceiver voyeur) => "Countershot follow-on admission probe";
		public bool TrySelectMove(ICharacter character, out ICombatMove? move) { move = null; return false; }
		public void MoveResolved(ICombatMove move, CombatMoveResult result)
		{
			++Calls;
			Require(actor.SetTraitValue(trait, 55), "Admitted native tactic continuation must retain its exact trait write.");
		}
	}

	private static void RunFailedFirearmPlanControl(TestDatabase database, RetirementHost host,
		ICharacter caster, ICharacter defender, FixtureIds fixture, ITraitDefinition trait)
	{
		var world = host.Native.World; var body = (Body)defender.Body;
		var original = body.HeldItems.Concat(body.WieldedItems).Distinct().ToArray();
		foreach (var item in original) body.Drop(item, silent: true);
		GameItem New(string name)
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
			world.Add(item); caster.Location.Insert(item, true); item.Login(); item.SetOwner(caster); world.SaveManager.Flush(); return item;
		}
		var gunItem = New("ARMRegression gun"); var rounds = new[] { New("ARMRegression round"), New("ARMRegression round") };
		var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
		Require(ReferenceEquals(body.GetWithoutMerge(gunItem), gunItem), "Failed-plan control must actually acquire its gun.");
		body.Wield(gunItem, silent: true);
		var refusedAcquisitions = 0;
		foreach (var round in rounds)
		{
			var acquired = body.GetWithoutMerge(round);
			if (acquired is null) ++refusedAcquisitions;
			Console.WriteLine($"ARMFirearm-plan-acquisition=item:{round.Id} acquired:{acquired?.Id} held:{body.HeldItems.Count()} wield-hands:{body.WieldedHandCount(gunItem)}");
			gun.Load(defender);
		}
		Require(refusedAcquisitions == 1 && rounds[0].ContainedIn == gunItem && rounds[1].ContainedIn is null &&
			rounds[1].DirectLocation == caster.Location && caster.Location.GameItems.Contains(rounds[1]) &&
			rounds[1].GetItemType<IHoldable>()!.HeldBy is null && gun.MagazineContents.SequenceEqual(new[] { rounds[0] }) &&
			rounds.All(x => !x.Deleted && x.Quantity == 1 && x.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, caster.Identity.Id)),
			"Actual failed native wielded-load plan must leave its second exact round on the floor and conserve the first loaded round.");
		body.CurrentStamina = 100; Require(defender.SetTraitValue(trait, 40), "Failed-plan control trait must persist."); world.SaveManager.Flush();
		var canonical = ((ICharacter)defender.GetTrait(trait).Owner).Id;
		var items = rounds.Append(gunItem).Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference, x.GetItemType<IHoldable>()!.HeldBy?.Id,
			x.DirectLocation?.Id, x.ContainedIn?.Id, body.WieldedItems.Any(y => ReferenceEquals(x, y)))).ToArray();
		RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, canonical, body.Id,
			defender.CurrentStamina, trait.Id, defender.TraitRawValue(trait), body.Id, body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun),
			gunItem.Id, gunItem.Condition, null, gun.MagazineContents.Select(x => x.Id).ToArray(), items, "wielded-load-refusal"), "--firearm-authority-reader");
		Console.WriteLine("ARMFirearm-plan-refusal=passed actual-native-plan floor-cell-join-preserved exact-one-loaded no-false-detach cold-reader");
		body.Take(gunItem); gunItem.Delete(); foreach (var round in rounds.Where(x => !x.Deleted)) round.Delete();
		foreach (var item in original) body.GetWithoutMerge(item); world.SaveManager.Flush();
	}

	private static int RunCountershotAuthority(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter defender, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		using var globals = new CheckLearningGlobals();
		ConfigureRegressionP2Fixture(host, database);
		var native = host.Native; var world = native.World; var service = world.SpellOwnedCorpseAnimations!;
		native.WorldMock.SetupGet(x => x.CombatMessageManager).Returns(new CombatMessageManager(world));

		typeof(Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1000", world));
		foreach (var name in new[] { "PowerMoveStaminaCost", "GraceMoveStaminaCost", "RecoveryTimeExpression" })
			typeof(CombatBase).GetProperty(name, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1", world));
		typeof(RangedWeaponAttackBase).GetField("_targetExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new Expression("0"));
		foreach (var (name, value) in new[] { ("EncumbranceLimitRatioHeavy", 0.8), ("EncumbranceLimitRatioModerate", 0.5), ("EncumbranceLimitRatioLight", 0.25), ("ChargeToMeleeStaminaCost", 2.0) })
			native.WorldMock.Setup(x => x.GetStaticDouble(name)).Returns(value);
		Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		var settings = (CharacterCombatSettings)caster.CombatSettings;
		settings.WeaponUsePercentage = 1; settings.NaturalWeaponPercentage = settings.AuxiliaryPercentage = settings.MagicUsePercentage = settings.PsychicUsePercentage = 0;
		settings.PreferredRangedMode = CombatStrategyMode.FireNoCover; settings.PreferredMeleeMode = CombatStrategyMode.StandardMelee;
		foreach (var race in world.Races)
		{
			Mock.Get(race).SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true, CanUseWeapons = true, CanDefend = true, DefaultCombatSetting = settings });
			Mock.Get(race).SetupGet(x => x.RaceUsesStamina).Returns(true);
		}
		var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		RunFailedFirearmPlanControl(database, host, caster, defender, fixture, trait);
		var configured = new HashSet<CommandableAI>();
		foreach (var scenario in new[] { "ordered-valid", "queued-revoked", "defender-check-expiry", "direct-valid" })
		{
			var actor = scenario == "ordered-valid" ? animated : cast(); var body = (Body)actor.Body; var defenderBody = (Body)defender.Body;
			actor.CombatSettings = settings; defender.CombatSettings = settings;
			actor.PositionState = PositionStanding.Instance;
			body.CurrentSpeeds[PositionStanding.Instance] = new MoveSpeed(new MudSharp.Models.MoveSpeed
			{ Id = 1, Alias = "walk", PositionId = PositionStanding.Instance.Id, Multiplier = 1, StaminaMultiplier = 1,
				FirstPersonVerb = "walk", ThirdPersonVerb = "walks", PresentParticiple = "walking" });
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included charge")), "Allowlist the existing native Charge command for this fixture.");
			Require(actor.SetTraitValue(trait, 40) && defender.SetTraitValue(trait, 40), "Countershot native trait baselines must be exact.");
			var actorCanonical = ((ICharacter)actor.GetTrait(trait).Owner).Id;
			var defenderCanonical = ((ICharacter)defender.GetTrait(trait).Owner).Id;
			var originalGear = defenderBody.HeldItems.Concat(defenderBody.WieldedItems).Distinct().ToArray();
			foreach (var item in originalGear) defenderBody.Drop(item, silent: true);
			GameItem New(string name)
			{
				var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
				world.Add(item); caster.Location.Insert(item, true); item.Login(); item.SetOwner(caster); world.SaveManager.Flush(); return item;
			}
			var gunItem = New("ARMRegression gun"); var rounds = new[] { New("ARMRegression round"), New("ARMRegression round") };
			var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
			Require(ReferenceEquals(defenderBody.GetWithoutMerge(gunItem), gunItem), "Countershot preparation must actually acquire the exact gun.");
			foreach (var round in rounds)
			{
				Require(ReferenceEquals(defenderBody.GetWithoutMerge(round), round), "Countershot preparation must actually acquire each exact ordinary round.");
				gun.Load(defender);
				Require(round.ContainedIn == gunItem, "Countershot preparation must actually load its acquired round.");
			}
			Require(gun.Ready(defender), "Actual countershot gun must be loaded and readied.");
			defenderBody.Wield(gunItem, silent: true);
			Require(defenderBody.WieldedItems.Any(x => ReferenceEquals(x, gunItem)), "Actual automatic countershot must wield its exact native gun.");
			var shot = (GameItem)gun.ChamberedRound!.Parent; var spare = gun.MagazineContents.Single();
			Require(rounds.All(x => x.ContainedIn == gunItem) && shot != spare, "Countershot chamber and spare identities must remain distinct and contained.");
			body.CurrentStamina = defenderBody.CurrentStamina = 100;
			void Read(string stage)
			{
				world.SaveManager.Flush();
				var items = rounds.Append(gunItem).Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference, x.GetItemType<IHoldable>()!.HeldBy?.Id,
					x.DirectLocation?.Id, x.ContainedIn?.Id, defenderBody.WieldedItems.Any(y => ReferenceEquals(x, y)))).ToArray();
				using (var db = NewIndependentContext(database.ConnectionString))
					foreach (var item in items)
						Console.WriteLine($"ARMCountershot-custody={scenario}-{stage} item:{item.Id} container:{db.GameItems.Single(x => x.Id == item.Id).ContainerId}/{item.Container} body:{string.Join(',',db.BodiesGameItems.Where(x => x.GameItemId == item.Id).Select(x => x.BodyId))}/{item.HeldBody} cell:{string.Join(',',db.RoomsGameItems.Where(x => x.GameItemId == item.Id).Select(x => x.RoomId))}/{item.Room} wield:{item.Wielded}");
				RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, defenderCanonical, defenderBody.Id,
					defender.CurrentStamina, trait.Id, defender.TraitRawValue(trait), body.Id, body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun),
					gunItem.Id, gunItem.Condition, gun.ChamberedRound?.Parent.Id, gun.MagazineContents.Select(x => x.Id).ToArray(), items, "countershot-" + scenario + "-" + stage,
					OtherBody: body.Id, OtherStamina: actor.CurrentStamina, OtherCanonical: actorCanonical, OtherRaw: actor.TraitRawValue(trait)), "--firearm-authority-reader");
			}
			Read("loaded");
			void Expire()
			{
				var grant = service.CommandGrant(actor.InstanceId, caster.Id)!;
				var source = XElement.Parse(XElement.Parse(grant.Provenance).Element("Source")!.Value);
				clock.Advance(DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) - RuntimeClock.UtcNow);
				Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Expire only the original attacker's grant, retaining both physical combatants.");
			}
			order(actor, caster, "hit opponent"); actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
			actor.MeleeRange = false; defender.MeleeRange = false; defender.CombatStrategyMode = CombatStrategyMode.FireNoCover;
			defender.TargettedBodypart = body.Bodyparts.OfType<IExternalBodypart>().First();
			defender.Aim = new AimInformation(actor, defender, [], gun) { AimPercentage = 1 };
			var checks = 0; var expiries = 0; var resolving = false;
			Mock.Get(world.GetCheck(gun.WeaponType.FireCheck)).Setup(x => x.MultiDifficultyCheck(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<Difficulty>(),
				It.IsAny<IPerceivable>(), It.IsAny<ITraitDefinition>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(() =>
			{
				Require(resolving && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(ChargeToMeleeMove) && x.GetMethod()?.Name == nameof(ChargeToMeleeMove.ResolveMove)) &&
					new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(CommandExecutionScope) && x.GetMethod()?.Name == nameof(CommandExecutionScope.ResolveIndependent)) &&
					!CommandExecutionScope.IsOrdered(actor) && !CommandExecutionScope.IsOrdered(defender), "Actual automatic StandAndFire must execute inside the production independent response scope.");
				++checks;
				if (scenario == "defender-check-expiry") { ++expiries; Expire(); Require(defender.SetTraitValue(trait, 60), "Independent defender native write must survive original attacker expiry."); }
				return Tuple.Create(CheckOutcome.SimpleOutcome(gun.WeaponType.FireCheck, Outcome.MajorPass), CheckOutcome.SimpleOutcome(gun.WeaponType.FireCheck, Outcome.MajorPass));
			});
			Console.WriteLine($"ARMCountershot-movement={scenario} stamina:{actor.CurrentStamina} encumbrance:{actor.EncumbrancePercentage} terrain:{actor.Location.Terrain(actor).StaminaCost} speed:{actor.CurrentSpeed?.StaminaMultiplier}");
			Require(actor.CanMove(CanMoveFlags.IgnoreCancellableActionBlockers).Result, $"Actual charge movement eligibility must pass: {actor.WhyCannotMove()}.");
			var followOn = new CountershotFollowOn(actor, trait); actor.AddEffect(followOn);
			try
			{
				var direct = scenario == "direct-valid";
				if (direct) { Expire(); using var independent = CommandExecutionScope.EnterIndependent(); actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectCharge(actor, defender)); }
				else { order(actor, caster, "charge opponent"); if (scenario == "defender-check-expiry") order(actor, caster, "charge opponent"); }
				var move = actor.ChooseMove();
				Require(move is ChargeToMeleeMove && CommandExecutionAuthority.IsOrdered(move) == !direct, $"Actual ChooseMove must select Charge with exact original-order provenance: move:{move?.GetType().Name} ordered:{CommandExecutionAuthority.IsOrdered(move)} selected:{actor.EffectsOfType<ISelectedCombatAction>().Count()}.");
				if (scenario == "queued-revoked") Expire();
				var wounds = body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				body.CurrentStamina = defenderBody.CurrentStamina = 100; world.SaveManager.Flush();
				using var emotional = _emotionalHooks ? new EmotionalProbeLease(actor) : null;
				resolving = true; try { actor.Combat!.CombatAction(actor, move); } finally { resolving = false; }
				emotional?.Verify("countershot-" + scenario, scenario != "queued-revoked");
				var admitted = scenario != "queued-revoked"; var after = body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				Require(actor.IsEmbodied && ReferenceEquals(actor.Combat, defender.Combat) && actor.Combat is not null &&
					actor.Combat.Combatants.Any(x => ReferenceEquals(x, actor)) && actor.Combat.Combatants.Any(x => ReferenceEquals(x, defender)),
					"Authority controls must retain both exact physical combatants after native countershot damage.");
				Require(checks == (admitted ? 1 : 0) && expiries == (scenario == "defender-check-expiry" ? 1 : 0), "Countershot and expiry callback counts must be exact.");
				Require(Same(actor.CurrentStamina, admitted ? 98 : 100) && Same(defender.CurrentStamina, admitted ? 97 : 100) &&
					Same(gunItem.Condition, admitted ? 0.99 : 1), $"Truthful accepted/refused native costs: {scenario}, charge:{actor.CurrentStamina}, defender:{defender.CurrentStamina}, condition:{gunItem.Condition}.");
				Require(actor.MeleeRange == admitted && defender.MeleeRange == admitted, "Contact admitted before the independent response remains; a pre-revoked charge establishes no contact.");
				Require(admitted ? after > wounds : Same(after, wounds), "Only the real admitted defender shot may install native attacker wounds.");
				Require(gun.ChamberedRound?.Parent == (admitted ? null : shot) && gun.MagazineContents.Single() == spare && spare.ContainedIn == gunItem &&
					rounds.All(x => !x.Deleted && x.Quantity == 1 && x.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, caster.Identity.Id)) &&
					(admitted ? shot.DirectLocation == actor.Location && shot.ContainedIn is null && shot.GetItemType<IHoldable>()!.HeldBy is null : shot.ContainedIn == gunItem),
					"Countershot must conserve the exact spare and place its ordinary projectile once without changing title or quantity.");
				var continuing = scenario is "ordered-valid" or "direct-valid";
				Require(followOn.Calls == (continuing ? 1 : 0) && Same(actor.TraitRawValue(trait), continuing ? 55 : 40) &&
					Same(defender.TraitRawValue(trait), scenario == "defender-check-expiry" ? 60 : 40), "Expired originating order must not execute its tactic continuation or overwrite independent defender work.");
				if (scenario == "defender-check-expiry")
				{
					Require(actor.EffectsOfType<SelectedCombatAction>().Count() == 1, "Capture a second actual queued order before expiry.");
					var next = actor.ChooseMove();
					Require(!actor.EffectsOfType<SelectedCombatAction>().Any() && (next is null || !CommandExecutionAuthority.IsOrdered(next)) &&
						Same(actor.CurrentStamina, 98) && gun.MagazineContents.Single() == spare, "The captured queued order must refuse without an owned follow-on; ordinary autonomous selection may continue independently.");
				}
				Read("resolved");
				Console.WriteLine($"ARMCountershot={scenario} passed actual-Charge-ResponseToMove-StandAndFire-ResolveIndependent native-wounds:{after-wounds} charge-cost:{100-actor.CurrentStamina} defender-cost:{100-defender.CurrentStamina} condition:{gunItem.Condition} checks:{checks} expiries:{expiries} tactic:{followOn.Calls} contact:{actor.MeleeRange} independent-raw:{defender.TraitRawValue(trait)}");
			}
			finally { resolving = false; actor.RemoveEffect(followOn); }
			using (CommandExecutionScope.EnterIndependent())
			{
				Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var reason), reason); restored(actor);
				defenderBody.Take(gunItem); gunItem.Delete(); foreach (var round in rounds.Where(x => !x.Deleted)) round.Delete();
				foreach (var item in originalGear) defenderBody.GetWithoutMerge(item); world.SaveManager.Flush();
			}
		}
		return 0;
	}
}

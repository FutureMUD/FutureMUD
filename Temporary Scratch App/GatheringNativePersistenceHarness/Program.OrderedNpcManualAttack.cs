#nullable enable

using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Combat.Strategies;
using MudSharp.Commands.Helpers;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.NPC.AI;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static int RunOrderedNpcManualAttackCallbacks(TestDatabase database, RetirementHost host, HarnessClock clock,
		ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order)
	{
		var native = host.Native;
		var world = native.World;
		var service = world.SpellOwnedCorpseAnimations!;
		var encumbrance = typeof(MudSharp.Body.Implementations.Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!;
		encumbrance.SetValue(null, new TraitExpression("1000", world));
		foreach (var property in new[] { "PowerMoveStaminaCost", "GraceMoveStaminaCost" })
			typeof(CombatBase).GetProperty(property, BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1", world));
		native.WorldMock.Setup(x => x.GetStaticDouble("EncumbranceLimitRatioHeavy")).Returns(0.8);
		native.WorldMock.Setup(x => x.GetStaticDouble("EncumbranceLimitRatioModerate")).Returns(0.5);
		native.WorldMock.Setup(x => x.GetStaticDouble("EncumbranceLimitRatioLight")).Returns(0.25);
		ConfigureStormHands(native);
		world.GetCheck(CheckType.MeleeWeaponPenetrateCheck);
		Mock.Get(world.GetCheck(CheckType.MeleeWeaponPenetrateCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponPenetrateCheck, Outcome.Pass));
		var expressions = (All<ITraitExpression>)world.TraitExpressions;
		var attacks = new All<IWeaponAttack>();
		native.WorldMock.SetupGet(x => x.WeaponAttacks).Returns(attacks);
		native.WorldMock.SetupGet(x => x.BodypartShapes).Returns(new All<IBodypartShape>());
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var model in db.TraitExpressions.Where(x => x.Name.StartsWith("Flame declared")))
				if (!expressions.Has(model.Id)) expressions.Add(new TraitExpression(model, world));
			attacks.Add(WeaponAttack.LoadWeaponAttack(db.WeaponAttacks.Single(x => x.Name == "Flame declared burning thrust"), world));
		}
		var attack = attacks.Single();
		var weaponType = (WeaponType)world.WeaponTypes.Single(x => x.Name.StartsWith("ARM03C1"));
		weaponType.AddAttack(attack);
		var messages = new CombatMessageManager(world);
		native.WorldMock.SetupGet(x => x.CombatMessageManager).Returns(messages);
		var bindings = new All<IManualCombatCommand>();
		native.WorldMock.SetupGet(x => x.ManualCombatCommands).Returns(bindings);
		native.WorldMock.Setup(x => x.Add(It.IsAny<IManualCombatCommand>())).Callback<IManualCombatCommand>(x => bindings.Add(x));
		EditableItemHelper.ManualCombatCommandHelper.EditableNewAction(caster, new StringStack("callbackstrike Callback Strike"));
		var binding = bindings.Single();
		Require(binding.NpcUsable && binding.BuildingCommand(caster, new StringStack($"action weapon {attack.Id}")),
			"Native builder did not register the NPC manual weapon command.");
		var settings = (CharacterCombatSettings)caster.CombatSettings;
		settings.WeaponUsePercentage = 1.0;
		settings.NaturalWeaponPercentage = settings.AuxiliaryPercentage = settings.MagicUsePercentage = settings.PsychicUsePercentage = 0.0;
		settings.PreferredMeleeMode = CombatStrategyMode.StandardMelee;
		settings.ForbiddenIntentions = CombatMoveIntentions.None;
		// The archive host deliberately disables racial combat. This fixture exercises real
		// autonomous weapon selection while retaining its deterministic helpless defender.
		foreach (var race in world.Races)
			Mock.Get(race).SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings
				{ CanAttack = true, CanUseWeapons = true, CanDefend = false, DefaultCombatSetting = settings });
		var configuredAis = new HashSet<CommandableAI>();
		try
		{
			foreach (var change in new[] { "valid", "expire", "retire", "depart", "replacement", "autonomous", "direct" })
			{
				var actor = cast();
				actor.CombatSettings = settings;
				var body = actor.Body;
				var ai = actor.AIs.OfType<CommandableAI>().Single();
				if (configuredAis.Add(ai))
					Require(ai.BuildingCommand(caster, new StringStack("included callbackstrike")), "Builder could not allowlist the manual command.");
				var weapon = (GameItem)host.Prototypes.Values.Single(x => x.Name == "ARM03B2B flame knife").CreateNew(caster);
				world.Add(weapon); caster.Location.Insert(weapon, true); weapon.Login();
				body.Get(weapon, silent: true); body.Wield(weapon, silent: true);
				Require(body.WieldedItems.Any(x => ReferenceEquals(x, weapon)), "Native weapon setup failed to wield the exact item.");
				actor.TargettedBodypart = foe.Body.Bodyparts.OfType<IExternalBodypart>().First();
				order(actor, caster, "hit opponent");
				actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
				void Expire()
				{
					var origin = service.CommandGrant(actor.InstanceId, caster.Id)!;
					var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
					var until = DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
					clock.Advance(until - RuntimeClock.UtcNow);
					Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Manual attack did not expire only control.");
				}
				if (change is "autonomous" or "direct") Expire();
				else order(actor, caster, "callbackstrike opponent");
				if (change == "direct")
					actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectManualCombatCommand(actor, binding, foe));
				if (change == "autonomous")
				{
					// Controlled native fixture places the already-engaged actor at melee range;
					// route advancement is covered by the separate displacement acceptance lane.
					actor.MeleeRange = true;
					actor.CombatStrategyMode = CombatStrategyMode.StandardMelee;
					Require(((StrategyBase)StandardMeleeStrategy.Instance).WhyWontAttack(actor) == string.Empty,
						"Declared autonomous fixture policy refused this target.");
					Console.WriteLine($"ARMOrdered-autonomous-selection=strategy:{actor.CombatStrategyMode} target:{actor.CombatTarget?.Id} melee:{actor.MeleeRange} stamina:{actor.CurrentStamina} attacks:{weaponType.UsableAttacks(actor, weapon, foe, weapon.GetItemType<IMeleeWeapon>().HandednessForWeapon(actor), false, BuiltInCombatMoveType.UseWeaponAttack).Count()}");
				}
				var combat = actor.Combat!;
				var move = actor.ChooseMove();
				Require(move is MeleeWeaponAttack && CommandExecutionAuthority.IsOrdered(move) == (change is not "autonomous" and not "direct"),
					$"Native {(change == "autonomous" ? "autonomous" : "builder manual")} ChooseMove selected {move?.GetType().Name ?? "null"} or wrong provenance.");
				var callback = new Mock<IFutureProg>();
				var called = 0;
				callback.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Callback<object[]>(args =>
				{
					if (!ReferenceEquals(args[0], actor) || ++called != 1) return;
					if (change == "expire") Expire();
					if (change == "retire") Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
					if (change is "depart" or "replacement")
					{
						combat.LeaveCombat(actor);
						if (change == "replacement")
						{
							var replacement = new SimpleMeleeCombat(world);
							replacement.JoinCombat(actor);
						}
					}
				}).Returns(true);
				var message = (CombatMessage)messages.CombatMessages.Single(x => x.Type == BuiltInCombatMoveType.UseWeaponAttack);
				var previous = message.WeaponAttackProg;
				var before = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				message.WeaponAttackProg = callback.Object;
				try { combat.CombatAction(actor, move); }
				finally { message.WeaponAttackProg = previous; }
				Require(called > 0, "Native combat-message weapon prog was not exercised.");
				var permitted = change is "valid" or "autonomous" or "direct";
				var after = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				Require(permitted ? after > before : Same(before, after), "Native callback changed wounds after refusal or lost valid attack damage.");
				world.SaveManager.Flush();
				using (var db = NewIndependentContext(database.ConnectionString))
				{
					Require(Same(db.Wounds.AsNoTracking().Where(x => x.BodyId == foe.Body.Id).Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun), after),
						"Native manual/autonomous wound state did not persist.");
					if (permitted)
						Require(db.Wounds.AsNoTracking().Any(x => x.BodyId == foe.Body.Id && x.ActorOriginId == actor.Identity.Id),
							"Native instance wound attribution did not persist the canonical character identity.");
				}
				if (!permitted) Require(!move.UsesStaminaWithResult(CombatMoveResult.Irrelevant), "Refused native manual attack retained action cost.");
				if (change != "retire") Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
				restored(actor);
				body.Take(weapon); weapon.Delete(); world.SaveManager.Flush();
				Console.WriteLine($"ARMOrdered-attack-{change}=passed paid-stock builder-NpcUsable actual-AI-allowlist command ChooseMove CombatAction real-message-prog real-wounds database-wounds exact-canonical-body");
			}
		}
		finally { foreach (var item in bindings.ToArray()) bindings.Remove(item); ManualCombatCommandRegistry.Rebuild(world); }
		return 0;
	}
}

#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using ExpressionEngine;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Form.Audio;
using MudSharp.Magic;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void SeedFirearmAuthorityFixture(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var ammo = db.AmmunitionTypes.Single(x => x.Name == "ARMRegression ammunition");
		ammo.DamageExpression = "6"; ammo.PainExpression = "4"; ammo.StunExpression = "2";
		ammo.DamageType = (int)DamageType.Crushing; ammo.Loudness = (int)AudioVolume.Silent;
		ammo.BreakChanceOnHit = ammo.BreakChanceOnMiss = 0;
		var gun = db.GameItemComponentProtos.Single(x => x.Name == "ARMRegression InternalMagazineGun");
		var xml = XElement.Parse(gun.Definition);
		xml.Add(new XElement("ConditionDegradesOnUse", true), new XElement("ConditionUseFormula", "0.01"));
		gun.Definition = xml.ToString(); db.SaveChanges();
	}

	private sealed record FirearmSavedItem(long Id, ItemOwnershipReference? Title, long? HeldBody, long? Cell, long? Container, bool Wielded = false);
	private sealed record FirearmAuthorityReader(string Database, FixtureIds Fixture, DateTime Now, long Canonical,
		long Body, double Stamina, long Trait, double Raw, long VictimBody, double Wounds,
		long Gun, double Condition, long? Chamber, long[] Magazine, FirearmSavedItem[] Items, string Case, long? StaleChamber = null, long? OtherBody = null, double? OtherStamina = null, long? OtherCanonical = null, double? OtherRaw = null);

	private static int RunFirearmAuthorityReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<FirearmAuthorityReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true, corpseAnimationAnatomy: true);
		ConfigureRegressionP2Fixture(host, database);
		var world = host.Native.World; var source = CreateAreaCell(host.Native, database.ConnectionString, input.Fixture.CellId, create: false);
		SetPrivateMember(host.Native.Actor, "Location", source);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(Same(db.Bodies.AsNoTracking().Single(x => x.Id == input.Body).CurrentStamina, input.Stamina) &&
				Same(db.CharacterTraits.AsNoTracking().Single(x => x.CharacterId == input.Canonical && x.TraitDefinitionId == input.Trait).Value, input.Raw) &&
				Same(db.Wounds.AsNoTracking().Where(x => x.BodyId == input.VictimBody).Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun), input.Wounds),
				"Fresh firearm reader must observe exact native stamina, independent trait and wound rows.");
			if (input.OtherBody.HasValue)
				Require(Same(db.Bodies.AsNoTracking().Single(x => x.Id == input.OtherBody).CurrentStamina, input.OtherStamina!.Value) &&
					Same(db.CharacterTraits.AsNoTracking().Single(x => x.CharacterId == input.OtherCanonical && x.TraitDefinitionId == input.Trait).Value, input.OtherRaw!.Value),
					"Fresh countershot reader must retain original attacker stamina and canonical follow-on trait rows.");
			var storedGun = db.GameItemComponents.AsNoTracking().Where(x => x.GameItemId == input.Gun).ToArray()
				.Select(x => XElement.Parse(x.Definition)).Single(x => x.Element("RoundsInMagazine") is not null);
			Require(long.Parse(storedGun.Element("ChamberedRound")!.Value) == (input.StaleChamber ?? input.Chamber ?? 0) &&
				storedGun.Element("RoundsInMagazine")!.Elements().Select(x => long.Parse(x.Value)).SequenceEqual(input.Magazine) &&
				long.Parse(storedGun.Element("ChamberedCasing")!.Value) == 0, "Exact saved firearm slot XML must agree before any item load.");
			foreach (var saved in input.Items)
				Require(db.GameItems.AsNoTracking().Single(x => x.Id == saved.Id).ContainerId == saved.Container &&
				db.BodiesGameItems.AsNoTracking().Where(x => x.GameItemId == saved.Id).Select(x => x.BodyId).ToArray()
					.SequenceEqual(saved.HeldBody.HasValue ? [saved.HeldBody.Value] : Array.Empty<long>()) &&
				db.CellsGameItems.AsNoTracking().Where(x => x.GameItemId == saved.Id).Select(x => x.CellId).ToArray()
					.SequenceEqual(saved.Cell.HasValue ? [saved.Cell.Value] : Array.Empty<long>()), "Cold firearm exact container/body/cell SQL custody must agree before any item load.");
		}
		// Load the gun before the returned round's body membership, including the historical stale-XML control.
		world.TryGetItem(input.Gun, true)!.FinaliseLoadTimeTasks();
		var owner = world.TryGetCharacter(input.Canonical, true)!; SetPrivateMember(owner, "Location", source);
		using (var db = NewIndependentContext(database.ConnectionString))
			((Body)owner.Body).LoadInventory(db.Bodies.Include(x => x.BodiesGameItems).Single(x => x.Id == input.Body));
		foreach (var saved in input.Items)
		{
			var item = (GameItem)world.TryGetItem(saved.Id, true)!; item.FinaliseLoadTimeTasks();
			if (saved.Cell.HasValue) source.Insert(item, true);
			Require(!item.Deleted && item.Quantity == 1 && item.OwnershipReference == saved.Title &&
				item.GetItemType<IHoldable>()!.HeldBy?.Id == saved.HeldBody && item.DirectLocation?.Id == saved.Cell &&
				item.ContainedIn?.Id == saved.Container, $"Cold native firearm item:{item.Id} quantity:{item.Quantity}/1 title:{item.OwnershipReference}/{saved.Title} held:{item.GetItemType<IHoldable>()!.HeldBy?.Id}/{saved.HeldBody} cell:{item.DirectLocation?.Id}/{saved.Cell} container:{item.ContainedIn?.Id}/{saved.Container}.");
			if (saved.HeldBody.HasValue) Require((saved.Wielded ? owner.Body.WieldedItems : owner.Body.HeldItems).Any(x => ReferenceEquals(x, item)), "Cold native hand/wield membership must contain the exact firearm item.");
		}
		var gunItem = world.TryGetItem(input.Gun, true)!;
		var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
		Require(Same(gunItem.Condition, input.Condition) && gun.ChamberedRound?.Parent.Id == input.Chamber &&
			gun.MagazineContents.Select(x => x.Id).SequenceEqual(input.Magazine) && gun.ChamberedCasing is null,
			"Cold native firearm loader must reconstruct exact chamber, magazine, casing and condition.");
		Console.WriteLine($"ARMFirearm-reader={input.Case} passed native-body-inventory firearm-XML chamber:{input.Chamber} magazine:{input.Magazine.Length} stamina:{input.Stamina} condition:{input.Condition} wounds:{input.Wounds} raw:{input.Raw} no-order-replay");
		return 0;
	}

	private static int RunFirearmAuthority(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, Func<ScriptedAiCharacterInstance> cast,
		Action<ScriptedAiCharacterInstance> restored, Action<ScriptedAiCharacterInstance, ICharacter, string> order, FixtureIds fixture)
	{
		using var globals = new CheckLearningGlobals();
		ConfigureRegressionP2Fixture(host, database);
		var native = host.Native; var world = native.World; var service = world.SpellOwnedCorpseAnimations!;
		typeof(Body).GetField("_encumbranceLimitExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1000", world));
		typeof(CombatBase).GetProperty("RecoveryTimeExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new TraitExpression("1", world));
		typeof(RangedWeaponAttackBase).GetField("_targetExpression", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, new Expression("0"));
		foreach (var (name, value) in new[] { ("EncumbranceLimitRatioHeavy", 0.8), ("EncumbranceLimitRatioModerate", 0.5), ("EncumbranceLimitRatioLight", 0.25) }) native.WorldMock.Setup(x => x.GetStaticDouble(name)).Returns(value);
		var settings = (CharacterCombatSettings)caster.CombatSettings;
		settings.WeaponUsePercentage = 1; settings.NaturalWeaponPercentage = settings.AuxiliaryPercentage = settings.MagicUsePercentage = settings.PsychicUsePercentage = 0;
		foreach (var race in world.Races)
		{
			Mock.Get(race).SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings { CanAttack = true, CanUseWeapons = true, CanDefend = false, DefaultCombatSetting = settings });
			Mock.Get(race).SetupGet(x => x.RaceUsesStamina).Returns(true);
		}
		var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		Mock.Get(world.GetCheck(CheckType.CombatRecoveryCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.CombatRecoveryCheck, Outcome.Pass));
		var configured = new HashSet<CommandableAI>();
		foreach (var scenario in new[] { "ordered-valid", "queued-revoked", "component-policy-revoked", "postcommit-independent", "ordered-miss", "direct-valid" })
		{
			var actor = scenario == "ordered-valid" ? animated : cast(); var body = (Body)actor.Body; actor.CombatSettings = settings;
			var ai = actor.AIs.OfType<CommandableAI>().Single();
			if (configured.Add(ai)) Require(ai.BuildingCommand(caster, new StringStack("included fire")), "Allowlist the actual selected Fire command.");
			var unrelated = body.HeldItems.ToArray(); foreach (var item in unrelated) body.Drop(item, silent: true);
			GameItem New(string name)
			{
				var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
				world.Add(item); caster.Location.Insert(item, true); item.Login(); item.SetOwner(caster); world.SaveManager.Flush(); return item;
			}
			var gunItem = New("ARMRegression gun"); var rounds = new[] { New("ARMRegression round"), New("ARMRegression round") };
			var gun = gunItem.GetItemType<InternalMagazineGunGameItemComponent>()!;
			body.GetWithoutMerge(gunItem);
			foreach (var round in rounds) { body.GetWithoutMerge(round); gun.Load(actor); }
			Require(gun.MagazineContents.Count() == 2 && rounds.All(x => x.ContainedIn == gunItem), "Native Load must conserve and contain both exact ordinary rounds.");
			Require(gun.Ready(actor) && gun.ChamberedRound is not null, "Native Ready must chamber the exact first round.");
			world.SaveManager.Flush();
			Require(gun.Unready(actor) && gun.ChamberedRound is null, "Native Unready must return the exact chambered round to custody.");
			Require(actor.SetTraitValue(trait, 40), "Native reaction trait baseline setup failed.");
			var canonical = ((ICharacter)actor.GetTrait(trait).Owner).Id;
			body.CurrentStamina = 100; world.SaveManager.Flush();
			void Read(string stage, long? staleChamber = null)
			{
				world.SaveManager.Flush();
				var items = rounds.Append(gunItem).Select(x => new FirearmSavedItem(x.Id, x.OwnershipReference, x.GetItemType<IHoldable>()!.HeldBy?.Id, x.DirectLocation?.Id, x.ContainedIn?.Id)).ToArray();
				RunItemReaderProcess(new FirearmAuthorityReader(database.Name, fixture, RuntimeClock.UtcNow, canonical, body.Id, actor.CurrentStamina,
					trait.Id, actor.TraitRawValue(trait), foe.Body.Id, foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun),
					gunItem.Id, gunItem.Condition, gun.ChamberedRound?.Parent.Id, gun.MagazineContents.Select(x => x.Id).ToArray(), items, scenario + "-" + stage, staleChamber), "--firearm-authority-reader");
			}
			Read("unreadied");
			if (scenario == "ordered-valid")
			{
				var returned = rounds.Single(x => ReferenceEquals(x.GetItemType<IHoldable>()!.HeldBy, body));
				using (var db = NewIndependentContext(database.ConnectionString))
				{
					var stored = db.GameItemComponents.Single(x => x.Id == gun.Id);
					var stale = XElement.Parse(stored.Definition); stale.Element("ChamberedRound")!.Value = returned.Id.ToString(CultureInfo.InvariantCulture);
					stored.Definition = stale.ToString(); db.SaveChanges();
				}
				Read("historical-stale-chamber", returned.Id);
				gun.Changed = true; world.SaveManager.Flush();
			}

			gun.Load(actor); Require(gun.Ready(actor), "Native reload and re-chamber must succeed.");
			var shot = (GameItem)gun.ChamberedRound!.Parent; var spare = gun.MagazineContents.Single();
			Require(rounds.Contains(shot) && rounds.Contains(spare) && shot != spare && rounds.All(x => x.ContainedIn == gunItem), "Native reload must conserve exact distinct chamber and magazine identities.");
			Read("loaded-chambered");
			void Expire()
			{
				var origin = service.CommandGrant(actor.InstanceId, caster.Id)!;
				var source = XElement.Parse(XElement.Parse(origin.Provenance).Element("Source")!.Value);
				clock.Advance(DateTime.Parse(source.Element("ControlUntilUtc")!.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind) - RuntimeClock.UtcNow);
				Require(!service.CanCommand(actor.InstanceId, caster.Id) && actor.IsEmbodied, "Expire only original command control, preserving physical body.");
			}
			order(actor, caster, "hit opponent"); actor.RemoveAllEffects<IdleCombatant>(fireRemovalAction: true);
			actor.MeleeRange = false; foe.MeleeRange = false; actor.TargettedBodypart = foe.Body.Bodyparts.OfType<IExternalBodypart>().First();
			actor.Aim = new AimInformation(foe, actor, [], gun) { AimPercentage = 1 };
			var fireCheck = Mock.Get(world.GetCheck(gun.WeaponType.FireCheck)); var checks = 0;
			fireCheck.Setup(x => x.MultiDifficultyCheck(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(),
				It.IsAny<ITraitDefinition>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>())).Returns(() =>
				{ ++checks; return Tuple.Create(CheckOutcome.SimpleOutcome(gun.WeaponType.FireCheck, scenario == "ordered-miss" ? Outcome.MajorFail : Outcome.MajorPass), CheckOutcome.SimpleOutcome(gun.WeaponType.FireCheck, Outcome.MajorPass)); });
			var policyField = typeof(CommandableAI).GetField("_canCommandProg", BindingFlags.Instance | BindingFlags.NonPublic)!;
			var policy = (IFutureProg)policyField.GetValue(ai)!; var faults = 0; var reactions = 0; var resolving = false;
			var fault = new Mock<IFutureProg>();
			fault.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns<object[]>(arguments =>
			{
				if (resolving && scenario == "component-policy-revoked" && faults == 0 && new StackTrace().GetFrames().Any(x => x.GetMethod()?.DeclaringType == typeof(FirearmBaseGameItemComponent) && x.GetMethod()?.Name == nameof(FirearmBaseGameItemComponent.Fire)))
				{ ++faults; Expire(); }
				return policy.ExecuteBool(arguments);
			});
			WoundEvent reaction = (wounded, wound) =>
			{
				if (!resolving || scenario != "postcommit-independent" || reactions != 0 || wound.ActorOrigin?.Identity.Id != actor.Identity.Id) return;
				++reactions; Expire(); using var independent = CommandExecutionScope.EnterIndependent();
				Require(actor.SetTraitValue(trait, 60), "Real independent wound reaction must retain its native trait write.");
			};
			foe.Body.OnWounded += reaction; policyField.SetValue(ai, fault.Object);
			try
			{
				var direct = scenario == "direct-valid";
				if (direct) { Expire(); using var independent = CommandExecutionScope.EnterIndependent(); actor.TakeOrQueueCombatAction(SelectedCombatAction.GetEffectFireItem(actor, foe, gun)); }
				else order(actor, caster, "fire");
				var move = actor.ChooseMove();
				Require(move is RangedWeaponAttackMove && CommandExecutionAuthority.IsOrdered(move) == !direct, "Actual ChooseMove must select native ranged attack with exact controller provenance.");
				if (scenario == "queued-revoked") Expire();
				body.CurrentStamina = 100; world.SaveManager.Flush(); var before = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				resolving = true; try { actor.Combat!.CombatAction(actor, move); } finally { resolving = false; }
				var fired = scenario is not ("queued-revoked" or "component-policy-revoked");
				var hit = fired && scenario != "ordered-miss"; var after = foe.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
				Require(Same(actor.CurrentStamina, fired ? 97 : 100) && Same(gunItem.Condition, fired ? 0.99 : 1), $"Accepted shots must pay exactly3 stamina and0.01 condition once: {scenario} stamina:{actor.CurrentStamina} condition:{gunItem.Condition}.");
				Require(gun.ChamberedRound?.Parent == (fired ? null : shot) && gun.MagazineContents.Single() == spare &&
					checks == (scenario == "queued-revoked" ? 0 : 1) && faults == (scenario == "component-policy-revoked" ? 1 : 0), "Refused shots must retain the exact chamber, magazine and truthful callback counts.");
				Require(hit ? after > before : Same(after, before), "Only accepted native hits may install wounds.");
				Require(reactions == (scenario == "postcommit-independent" ? 1 : 0) && Same(actor.TraitRawValue(trait), reactions == 1 ? 60 : 40), "Independent real wound reaction must survive once without being overwritten.");
				Require(rounds.All(x => !x.Deleted && x.Quantity == 1 && x.OwnershipReference == new ItemOwnershipReference(caster.FrameworkItemType, caster.Identity.Id)) &&
					(fired ? shot.DirectLocation == foe.Location && shot.ContainedIn is null && shot.GetItemType<IHoldable>()!.HeldBy is null : shot.ContainedIn == gunItem && shot.DirectLocation is null), "Consumed projectile must have exactly one native placement and retain title/quantity.");
				using (CommandExecutionScope.EnterIndependent())
				{
					Require(gun.Unload(actor).SequenceEqual([spare]) && !gun.MagazineContents.Any() && spare.ContainedIn is null &&
						ReferenceEquals(spare.GetItemType<IHoldable>()!.HeldBy, body), "Actual component Unload must return the distinct surviving magazine round once.");
				}
				Read("shot-unloaded");
				Console.WriteLine($"ARMFirearm={scenario} passed actual-Load-Ready-Unready-ChooseMove-RangedAttack-CombatAction-Unload cost:{100-actor.CurrentStamina} condition:{gunItem.Condition} wound-delta:{after-before} reactions:{reactions} projectile:{shot.Id} raw:{actor.TraitRawValue(trait)}");
			}
			finally { resolving = false; policyField.SetValue(ai, policy); foe.Body.OnWounded -= reaction; }
			using (CommandExecutionScope.EnterIndependent())
			{
				Require(service.TryRetire(actor.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
				body.Take(gunItem); gunItem.Delete(); foreach (var round in rounds.Where(x => !x.Deleted)) { if (round.GetItemType<IHoldable>()!.HeldBy is { } holder) holder.Take(round); round.Delete(); }
				foreach (var item in unrelated) body.GetWithoutMerge(item); world.SaveManager.Flush(); restored(actor);
			}
		}
		return 0;
	}
}

#nullable enable

using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Commands.Helpers;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.Form.Shape;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Capabilities;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC;
using MudSharp.NPC.Templates;
using MudSharp.Planes;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record StormSpearReader(string Database, FixtureIds Fixture, DateTime Now, long Spell,
		long Item, Guid Lifecycle, DateTime Deadline, long Bag, long Sibling);
	private sealed record StormPlacementReader(string Database, FixtureIds Fixture, DateTime Now, long Item, Guid Invocation, long Spell, long Capability, double Balance);

	private static void SeedStormSpearFixture(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var spear = db.GameItemProtos.Single(x => x.Name == "ARM03B2B flame knife");
		spear.Name = "ARM03B2B Storm Spear"; spear.Keywords = "storm lightning spear";
		spear.ShortDescription = "a spear of lightning";
		spear.FullDescription = "A declared native electrical weapon profile; historical object 488 JavaScript statistics are unavailable.";
		var zero = db.Tags.Single(x => x.Name == "ARM03C1 creation component");
		var parent = zero;
		var token = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B creation token");
		long id = db.GameItemProtos.Max(x => x.Id);
		for (var rank = 1; rank <= 4; rank++)
		{
			var tag = new Db.Tag { Name = $"Storm Creation {rank}", ParentId = parent.Id };
			db.Tags.Add(tag); db.SaveChanges(); parent = tag;
			var proto = new Db.GameItemProto { Id = ++id, Name = $"ARM03B2B Storm token {rank}", Keywords = $"creation token rank{rank}",
				ShortDescription = $"a rank {rank} Creation token", FullDescription = "Declared native acceptance component hierarchy.",
				MaterialId = token.MaterialId, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard,
				EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
			foreach (var component in token.GameItemProtosGameItemComponentProtos)
				proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.GameItemComponentProtoId });
			proto.GameItemProtosTags.Add(new() { TagId = tag.Id }); db.GameItemProtos.Add(proto); db.SaveChanges();
		}
		var expressions = new[] { "6", "4", "2" }.Select((formula, index) =>
			new Db.TraitExpression { Name = $"Storm declared fixture channel {index}", Expression = formula }).ToArray();
		db.TraitExpressions.AddRange(expressions); db.SaveChanges();
		db.WeaponAttacks.Add(new Db.WeaponAttack { Name = "Storm declared electrical thrust", WeaponTypeId = db.WeaponTypes.Single(x => x.Name.StartsWith("ARM03C1")).Id,
			MoveType = (int)BuiltInCombatMoveType.UseWeaponAttack, DamageType = (int)DamageType.Electrical,
			DamageExpressionId = expressions[0].Id, PainExpressionId = expressions[1].Id, StunExpressionId = expressions[2].Id,
			BaseAttackerDifficulty = (int)Difficulty.Normal, BaseBlockDifficulty = (int)Difficulty.Normal,
			BaseParryDifficulty = (int)Difficulty.Normal, BaseDodgeDifficulty = (int)Difficulty.Normal,
			BaseAngleOfIncidence = Math.PI / 2, RecoveryDifficultySuccess = (int)Difficulty.Normal,
			RecoveryDifficultyFailure = (int)Difficulty.Normal, BaseDelay = 1, Weighting = 1, MaximumTargets = 1,
			StaminaCost = 1, ExertionLevel = (int)ExertionLevel.Heavy, Verb = (int)MeleeWeaponVerb.Stab,
			Orientation = (int)Orientation.Centre, Alignment = (int)Alignment.FrontRight,
			HandednessOptions = (int)AttackHandednessOptions.OneHandedOnly,
			Intentions = (long)(CombatMoveIntentions.Attack | CombatMoveIntentions.Wound),
			RequiredPositionStateIds = PositionStanding.Instance.Id.ToString(), AdditionalInfo = "" });
		db.CombatMessages.Add(new Db.CombatMessage { Type = (int)BuiltInCombatMoveType.UseWeaponAttack, Chance = 1,
			Message = "$0 thrust|thrusts $2 at $1", FailureMessage = "$0 thrust|thrusts $2 at $1" }); db.SaveChanges();
	}

	private static void ConfigureStormHands(NativeRuntime native)
	{
		native.Body.Handedness = Alignment.Right;
		var hands = native.Body.WieldLocs.OrderBy(x => x.Id).ToArray();
		Require(hands.Length == 2, "Stock needs the declared native two-hand fixture.");
		for (var index = 0; index < hands.Length; index++)
		{
			var hand = hands[index]; var mock = Mock.Get((IExternalBodypart)hand);
			mock.SetupGet(x => x.Alignment).Returns(index == 0 ? Alignment.Right : Alignment.Left);
			mock.Setup(x => x.FullDescription()).Returns(index == 0 ? "right hand" : "left hand");
			mock.As<IWield>().Setup(x => x.CanWield(It.IsAny<IGameItem>(), It.IsAny<IInventory>()))
				.Returns<IGameItem, IInventory>((item, inventory) => inventory.HeldItemsFor(hand).Concat(inventory.WieldedItemsFor(hand)).Any(x => x != item)
					? IWieldItemWieldResult.GrabbingWielderHoldOtherItem : IWieldItemWieldResult.Success);
			mock.As<IGrab>().Setup(x => x.CanGrab(It.IsAny<IGameItem>(), It.IsAny<IInventory>()))
				.Returns<IGameItem, IInventory>((item, inventory) => inventory.HeldItemsFor(hand).Concat(inventory.WieldedItemsFor(hand)).Any(x => x != item)
					? WearlocGrabResult.FailFull : WearlocGrabResult.Success);
		}
		Mock.Get(native.Body.Race).Setup(x => x.GetMaximumLiftWeight(It.IsAny<ICharacter>())).Returns(10000);
	}

	private static void FinaliseStormTags(TestDatabase database, IFuturemud world)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		foreach (var row in db.Tags.AsNoTracking()) ((ILoadingTag)world.Tags.Get(row.Id)!).FinaliseLoad(row);
	}

	private static Cell CreateStormCell(NativeRuntime native, TestDatabase database, long cellId)
	{
		_ = CreateAreaCell(native, database.ConnectionString, cellId, create: true);
		using var db = NewIndependentContext(database.ConnectionString);
		var model = db.Cells.Include(x => x.CellOverlays).Include(x => x.CellsMagicResources).AsNoTracking().Single(x => x.Id == cellId);
		var zoneModel = db.Zones.AsNoTracking().Single(x => x.Id == model.ZoneId);
		var shard = new Shard(db.Shards.AsNoTracking().Single(x => x.Id == zoneModel.ShardId), native.World);
		var shards = new All<IShard>(); shards.Add(shard); native.WorldMock.SetupGet(x => x.Shards).Returns(shards);
		native.WorldMock.SetupGet(x => x.WeatherControllers).Returns(new All<IWeatherController>());
		var zone = new Zone(zoneModel, native.World); var zones = new All<IZone>(); zones.Add(zone); native.WorldMock.SetupGet(x => x.Zones).Returns(zones);
		var cell = new Cell(model, zone); var cells = new All<ICell>(); cells.Add(cell); native.WorldMock.SetupGet(x => x.Cells).Returns(cells); cell.PostLoadTasks(model);
		return cell;
	}

	private static int RunStormSpearStockChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "storm_spear_stock", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedStormSpearFixture(database);
		var host = PrepareRetirementHost(database, fixture, clock, wielding: true, corpseAnimationAnatomy: true);
		var native = host.Native; var world = native.World; var caster = native.Actor; ConfigureStormHands(native); FinaliseStormTags(database, world);
		var effects = new EffectScheduler(world, clock); native.WorldMock.SetupGet(x => x.EffectScheduler).Returns(effects);
		var items = new SpellOwnedItemService(world); native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(items);
		var expressions = (All<ITraitExpression>)world.TraitExpressions;
		// An absent optional shape must resolve to null, as in the native catalogue.
		// The loose world's recursive default otherwise fabricates a shape for ID zero.
		native.WorldMock.SetupGet(x => x.BodypartShapes).Returns(new All<IBodypartShape>());
		var attacks = new All<IWeaponAttack>(); native.WorldMock.SetupGet(x => x.WeaponAttacks).Returns(attacks);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			foreach (var model in db.TraitExpressions.Where(x => x.Name.StartsWith("Storm declared")))
				if (!expressions.Has(model.Id)) expressions.Add(new TraitExpression(model, world));
			attacks.Add(WeaponAttack.LoadWeaponAttack(db.WeaponAttacks.Single(x => x.Name == "Storm declared electrical thrust"), world));
		}
		var weapon = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B Storm Spear");
		var weaponType = (WeaponType)world.WeaponTypes.Single(x => x.Name.StartsWith("ARM03C1"));
		foreach (var attack in attacks) if (!weaponType.Attacks.Contains(attack)) weaponType.AddAttack(attack);
		native.WorldMock.SetupGet(x => x.CombatMessageManager).Returns(new CombatMessageManager(world));
		Mock.Get(world.GetCheck(CheckType.MeleeWeaponPenetrateCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponPenetrateCheck, Outcome.Pass));
		native.WorldMock.Setup(x => x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(x => expressions.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => ((All<IMagicSpell>)world.MagicSpells).Add(x));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(world.FutureProgs.GetByName("AlwaysFalse")!);
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var ranks = new[] { world.Tags.GetByName("ARM03C1 creation component")! }.Concat(Enumerable.Range(1, 4).Select(x => world.Tags.GetByName($"Storm Creation {x}")!)).ToArray();
		var command = $"stock storm-spear {cap.School.Id} {trait.Id} {native.Resource.Id} {weapon.Id} {string.Join(' ', ranks.Select(x => x.Id))}";
		EditableItemHelper.MagicSpellHelper.EditableNewAction(caster, new StringStack(command));
		var spell = (MagicSpell)world.MagicSpells.Single(x => x.Name == ArmageddonStormSpearStock.Name);
		Require(spell.ReadyForGame && spell.GradeConfigurationErrors().Count == 0 && spell.StockIdentity == ArmageddonStormSpearStock.Key,
			"Stock is not ready: " + string.Join(';', spell.GradeConfigurationErrors()));
		var count = world.MagicSpells.Count(); EditableItemHelper.MagicSpellHelper.EditableNewAction(caster, new StringStack(command));
		Require(world.MagicSpells.Count() == count, "Duplicate stock authoring added content.");
		Require(!spell.BuildingCommand(caster, new StringStack("plan ranks 1 -2 1")), "Invalid rank replacement succeeded.");
		Require(spell.BuildingCommand(caster, new StringStack("plan carried 1 off")) && spell.BuildingCommand(caster, new StringStack("plan carried 1 on")) &&
			spell.BuildingCommand(caster, new StringStack($"plan ranks 1 -3 {string.Join(' ', ranks.Select(x => x.Id))}")), "Stock builder selection controls failed.");
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var loaded = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			Require(loaded.StockIdentity == spell.StockIdentity && loaded.GradeConfigurationErrors().Count == 0 &&
				((CreateItemEffect)loaded.SpellEffects.Single()).PrimaryHand &&
				loaded.InventoryPlanTemplate.Phases.First().Actions.Single().SaveToXml().Element("GradeRank")!.Attribute("offset")!.Value == "-3",
				"Stock reload lost primary placement or material ranks.");
			((All<IMagicSpell>)world.MagicSpells).Remove(spell); ((All<IMagicSpell>)world.MagicSpells).Add(loaded); spell = loaded;
		}
		Console.WriteLine("ARMStorm-stock-builder=passed stock-identity duplicate-refusal seven-grades builder-material-edit persisted-reload primaryhand rank-offset-minus-three all-grades-temporary declared-electrical-profile-no-historical-JS-parity");
		caster.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null, true);
		foreach (var value in new[] { $"casting trait {trait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive", $"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative", $"casting entry starting {spell.Id} on", "casting enable on" })
			Require(cap.BuildingCommand(caster, new StringStack(value)), "Capability refused " + value);
		caster.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); caster.SetTraitValue(trait, 90); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow, random: () => 0.1, flush: () => FlushCasting(native));
		native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object, caster, cap.Id, "Storm native stock acceptance").Allowed, "Stock enrolment failed.");
		new MagicCastingStateStore().Write(acquired: casting.Acquisition(caster, spell.Id)! with { ControlledGrade = 7 });
		caster.AddResource(native.Resource, 100); FlushCasting(native);
		GameItem New(string name)
		{
			var item = (GameItem)host.Prototypes.Values.Single(x => x.Name == name).CreateNew(caster);
			world.Add(item); caster.Location.Insert(item, true); item.Login(); world.SaveManager.Flush(); return item;
		}
		void Carry(IGameItem item)
		{
			native.Body.Get(item, silent: true); Require(native.Body.Swap(item, null!), "Native hand swap failed.");
			Require(ReferenceEquals(item.InInventoryOf, native.Body) && !native.Body.HeldItemsFor(native.Body.WieldLocs.OrderBy(x => x.Id).First()).Contains(item), "Component did not reach off hand.");
		}
		(int Items, int Origins) Rows() { using var db = NewIndependentContext(database.ConnectionString); return (db.GameItems.Count(), db.MagicSpellLifecycles.Count()); }
		void Refused(int grade, IGameItem component, string reason)
		{
			var before = Rows(); var balance = caster.MagicResourceAmounts[native.Resource];
			var result = casting.Cast(new(caster, cap.Id, spell.Id, grade, false, "self"));
			Require(result.Status == MagicCastingStatus.Refused && result.OperationId is null && !component.Deleted && Rows() == before && caster.MagicResourceAmounts[native.Resource] == balance,
				"Prepayment conservation failed for " + reason + ": " + result.Message);
		}
		var lowToken = New("ARM03B2B creation token"); Carry(lowToken); Refused(7, lowToken, "under-ranked component");
		native.Body.Drop(lowToken, silent: true); Refused(1, lowToken, "ground component");
		var foreignBag = New("ARM03B2B bag"); var bag = foreignBag.GetItemType<IContainer>()!;
		caster.Location.Extract(lowToken); bag.Put(caster, lowToken, false); Refused(1, lowToken, "contained component");
		foreignBag.Take(lowToken); caster.Location.Insert(lowToken, true); Carry(lowToken);
		var blocker = New("ARM03B2B goods"); native.Body.Get(blocker, silent: true); Refused(1, lowToken, "occupied primary hand");
		native.Body.Drop(blocker, silent: true);
		Console.WriteLine("ARMStorm-prepayment=passed real-low-rank direct-scope ground-container-and-occupied-primary-hand-refused zero-operation zero-debit zero-material-delete zero-output-or-lifecycle-row");
		GameItem Cast(int grade, IGameItem token, ICharacter? recipient = null)
		{
			caster.RemoveAllEffects<MagicSpellLockout>(null, true); caster.AddResource(native.Resource, 100);
			var balance = caster.MagicResourceAmounts[native.Resource]; var before = Rows(); var now = RuntimeClock.UtcNow;
			var receiver = recipient ?? caster;
			var result = casting.Cast(new(caster, cap.Id, spell.Id, grade, false, recipient is null ? "self" : "opponent"));
			Require(result.Status == MagicCastingStatus.Succeeded, "Paid stock cast failed: " + result.Message);
			var item = (GameItem)host.Items.Single(x => x.SpellCreationOrigin?.IsTemporary == true && !x.Deleted && x.InInventoryOf == receiver.Body);
			var life = host.Store.Find(item.SpellCreationOrigin!.LifecycleId)!; var seconds = (life.Origin.DeadlineUtc!.Value - now).TotalSeconds;
			Require(token.Deleted && caster.MagicResourceAmounts[native.Resource] == balance - spell.GradeProfile!.Efficiency!.Cost(7, grade) &&
				Rows().Items == before.Items && Rows().Origins == before.Origins + 1 && life.Origin.Grade == grade && life.Origin.Mode == SpellLifecycleMode.TemporaryCleanup &&
				seconds >= 1687.5 * grade && seconds <= 2025 * grade && receiver.Body.WieldedItems.Contains(item) &&
				item.GetItemType<IWieldable>()!.PrimaryWieldedLocation!.Alignment == Alignment.Right,
				"Paid stock output lost payment, exact ownership, selected grade, deadline or primary-hand wielding.");
			FlushCasting(native); Console.WriteLine($"ARMStorm-paid-grade{grade}=passed actual-resource-debit actual-component-delete exact-one-native-weapon primary-hand-wield persisted-grade absolute-source-nominal-deadline seconds:{seconds}"); return item;
		}
		var low = Cast(1, lowToken);
		var oldCell = caster.Location;
		var cell = CreateStormCell(native, database, fixture.CellId);
		foreach (var item in oldCell.GameItems.ToArray()) { oldCell.Extract(item); cell.Insert(item, true); }
		SetPrivateMember(caster, "Location", cell); ((List<ICharacter>)cell.Characters).Add(caster);
		Mock.Get(native.Body.Race).SetupGet(x => x.NaturalPerceptionTypes).Returns(PerceptionTypes.DirectVisual);
		Mock.Get(native.Body.Prototype).SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(world));
		var personalName = new PersonalName(new XElement("Name", new XAttribute("culture", 1), new XElement("Element", new XAttribute("usage", "BirthName"), "caster")), world);
		SetPrivateField(caster, "_personalName", personalName); SetPrivateField(caster, "_currentName", personalName);
		caster.CombatSettings = new CharacterCombatSettings(caster, "Storm fixture combat");
		var templates = new RevisableAll<INPCTemplate>(); native.WorldMock.SetupGet(x => x.NpcTemplates).Returns(templates);
		var data = new SimpleCharacterTemplate { Gameworld = world, SelectedName = new PersonalName(new XElement("Name", new XAttribute("culture", 1), new XElement("Element", new XAttribute("usage", "BirthName"), "opponent")), world),
			SelectedRace = native.Body.Race, SelectedEthnicity = native.Body.Ethnicity, SelectedCulture = caster.Culture,
			SelectedBirthday = world.Calendars.First().GetDate("1-month-2000"), SelectedStartingLocation = cell, SelectedGender = native.Body.Gender.Enum,
			SelectedHeight = 1.8, SelectedWeight = 80, SelectedSdesc = "an opponent", SelectedFullDesc = "A declared acceptance opponent.",
			SelectedAccents = [], SelectedAttributes = [], SelectedCharacteristics = [], SelectedEntityDescriptionPatterns = [], SkillValues = [], SelectedRoles = [], SelectedMerits = [],
			SelectedKnowledges = [], MissingBodyparts = [], SelectedDisfigurements = [], SelectedProstheses = [] };
		var template = new SimpleNPCTemplate(world, DummyAccount.Instance, data, "Storm opponent"); templates.Add(template);
		var opponent = (NPC)template.CreateNewCharacter(cell); world.Add(opponent, true); world.Add(opponent.Body); opponent.CombatSettings = caster.CombatSettings; cell.Enter(opponent);
		opponent.Body.Handedness = Alignment.Right;
		void Strike(GameItem item, int grade, ICharacter? attacker = null, ICharacter? target = null)
		{
			attacker ??= caster; target ??= opponent;
			attacker.TargettedBodypart = target.Body.Bodyparts.OfType<IExternalBodypart>().First();
			var before = target.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
			var move = new MeleeWeaponAttack(attacker, item.GetItemType<IMeleeWeapon>()!, attacks.Single(), target);
			var result = move.ResolveMove(new HelplessDefenseMove { Assailant = target });
			Require(result.MoveWasSuccessful && result.WoundsCaused.Any(x => x.DamageType == DamageType.Electrical && x.Parent == target && target.Body.Wounds.Contains(x)) &&
				target.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun) > before, "Native spear attack delivered no electrical wounds to the target's body.");
			world.SaveManager.Flush(); using var db = NewIndependentContext(database.ConnectionString);
			var persisted = db.Wounds.Where(x => x.BodyId == target.Body.Id && x.DamageType == (int)DamageType.Electrical).ToArray();
			var electrical = target.Body.Wounds.Where(x => x.DamageType == DamageType.Electrical).ToArray();
			Require(persisted.Length > 0 && Same(persisted.Sum(x => x.CurrentDamage), electrical.Sum(x => x.CurrentDamage)) &&
				Same(persisted.Sum(x => x.CurrentPain), electrical.Sum(x => x.CurrentPain)) && Same(persisted.Sum(x => x.CurrentStun), electrical.Sum(x => x.CurrentStun)),
				"Electrical wound channels did not persist for the target body.");
			Console.WriteLine($"ARMStorm-combat-grade{grade}=passed real-MeleeWeaponAttack actual-electrical-target-wounds persisted-native-wounds declared-6-damage-4-pain-2-stun helpless-defence controlled-roll no-historical-damage-parity");
		}
		Strike(low, 1);
		var lowOrigin = low.SpellCreationOrigin!; var lowDeadline = lowOrigin.DeadlineUtc!.Value;
		clock.Advance(lowDeadline - RuntimeClock.UtcNow - TimeSpan.FromTicks(1)); effects.CheckSchedules();
		Require(!caster.EffectsOfType<MagicSpellLockout>().Any(), "Native effect scheduler retained an elapsed casting lockout.");
		items.ReconcileRetirements(RuntimeClock.UtcNow); Require(!low.Deleted, "Stock expired before deadline.");
		clock.Advance(TimeSpan.FromTicks(1)); items.ReconcileRetirements(RuntimeClock.UtcNow);
		var lowRetired = host.Store.Find(lowOrigin.LifecycleId)!;
		Require(low.Deleted && !native.Body.WieldedItems.Contains(low) && !blocker.Deleted && !foreignBag.Deleted && lowRetired.State == SpellLifecycleState.Completed,
			$"Wielded expiry failed: deleted={low.Deleted} wielded={native.Body.WieldedItems.Contains(low)} blockerDeleted={blocker.Deleted} bagDeleted={foreignBag.Deleted} state={lowRetired.State} diagnostic={lowRetired.Diagnostic} bodyEffects={string.Join(',', native.Body.Effects.Select(x => x.GetType().Name))} actorEffects={string.Join(',', caster.Effects.Select(x => x.GetType().Name))} bodyPositionTarget={native.Body.PositionTarget?.FrameworkItemType} actorPositionTarget={caster.PositionTarget?.FrameworkItemType}.");
		Console.WriteLine("ARMStorm-wielded-expiry=passed exact-absolute-boundary native-Body-unwield native-item-Delete completed-once unrelated-goods-and-container-survive");
		var highToken = New("ARM03B2B Storm token 4"); Carry(highToken);
		caster.RemoveAllEffects<MagicSpellLockout>(null, true); caster.AddResource(native.Resource, 100);
		var recipientBlocker = New("ARM03B2B goods"); opponent.Body.Get(recipientBlocker, silent: true);
		var recipientBalance = caster.MagicResourceAmounts[native.Resource]; var recipientRows = Rows();
		var occupiedRecipient = casting.Cast(new(caster, cap.Id, spell.Id, 1, false, "opponent"));
		Require(occupiedRecipient.Status == MagicCastingStatus.Refused && occupiedRecipient.OperationId is null && !highToken.Deleted &&
			caster.MagicResourceAmounts[native.Resource] == recipientBalance && Rows() == recipientRows && native.Body.FunctioningFreeHands.Any(),
			"Occupied other recipient did not refuse before payment while caster had a free hand.");
		opponent.Body.Drop(recipientBlocker, silent: true);
		new MagicCastingStateStore().Write(acquired: casting.Acquisition(caster, spell.Id)! with { ControlledGrade = 6 });
		caster.SetTraitValue(trait, 81);
		var gateBalance = caster.MagicResourceAmounts[native.Resource]; var gateRows = Rows();
		var belowGate = casting.Cast(new(caster, cap.Id, spell.Id, 7, true, "opponent"));
		Require(belowGate.Status == MagicCastingStatus.Refused && belowGate.OperationId is null && !highToken.Deleted &&
			caster.MagicResourceAmounts[native.Resource] == gateBalance && Rows() == gateRows, "Grade-seven overreach bypassed its raw proficiency gate.");
		caster.SetTraitValue(trait, 85.5);
		var gateError = casting.Preflight(caster, cap.Id, spell.Id, 7, true); Require(gateError is null, "Grade-seven cap-relative overreach boundary preflight refused: " + gateError);
		new MagicCastingStateStore().Write(acquired: casting.Acquisition(caster, spell.Id)! with { ControlledGrade = 7 });
		Console.WriteLine("ARMStorm-recipient-grade-gate=passed other-recipient-occupied-primary-refused caster-free-hand-preserved no-payment native-cap90 overreach-raw81-refused overreach-raw85point5-preflight-admitted approved-95-percent-gate controlled-grade-retained-independently no-paid-overreach-claim");
		var high = Cast(7, highToken, opponent); Strike(high, 7, opponent, caster);
		var highOrigin = high.SpellCreationOrigin!;
		opponent.Body.Take(high); bag.Put(caster, high, false); var sibling = New("ARM03B2B goods"); cell.Extract(sibling); bag.Put(caster, sibling, false);
		world.SaveManager.Flush(); FlushCasting(native);
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			if (!db.CellsGameItems.Any(x => x.GameItemId == foreignBag.Id)) db.CellsGameItems.Add(new() { CellId = fixture.CellId, GameItemId = foreignBag.Id }); db.SaveChanges();
			Require(db.GameItems.Find(high.Id)!.ContainerId == foreignBag.Id && db.GameItems.Find(sibling.Id)!.ContainerId == foreignBag.Id, "Foreign containment fixture not persisted.");
		}
		RunItemReaderProcess(new StormSpearReader(database.Name, fixture, RuntimeClock.UtcNow, spell.Id, high.Id, highOrigin.LifecycleId,
			highOrigin.DeadlineUtc!.Value, foreignBag.Id, sibling.Id), "--storm-spear-stock-reader");
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.GameItems.Any(x => x.Id == high.Id) && db.GameItems.Find(sibling.Id)!.ContainerId == foreignBag.Id && db.GameItems.Any(x => x.Id == foreignBag.Id), "Restart expiry lost foreign topology.");
		Console.WriteLine("ARMStorm-restart-expiry=passed actual-fresh-process native-container-reload grade-seven-temporary unchanged-absolute-deadline exact-owned-leaf-removal foreign-container-and-sibling-survive");
		caster.RemoveAllEffects<MagicSpellLockout>(null, true);
		var faultToken = New("ARM03B2B creation token"); Carry(faultToken); caster.AddResource(native.Resource, 100);
		var faultBalance = caster.MagicResourceAmounts[native.Resource]; var invocation = Guid.NewGuid(); GameItem? redirected = null;
		void Redirect(InventoryState oldState, InventoryState newState, IGameItem changed)
		{
			if (newState != InventoryState.Held || changed.SpellCreationOrigin is null || redirected is not null) return;
			redirected = (GameItem)changed; native.Body.Drop(changed, silent: true);
		}
		native.Body.OnInventoryChange += Redirect;
		MagicCastingResult fault;
		try { fault = casting.Cast(new(caster, cap.Id, spell.Id, 1, false, "self", OriginId: invocation)); }
		finally { native.Body.OnInventoryChange -= Redirect; }
		Require(fault.Status == MagicCastingStatus.NeedsReview && fault.OperationId == invocation && faultToken.Deleted && redirected is not null &&
			redirected.InInventoryOf is null && !native.Body.WieldedItems.Contains(redirected) && cell.GameItems.Contains(redirected) &&
			caster.MagicResourceAmounts[native.Resource] == faultBalance - spell.GradeProfile!.Efficiency!.Cost(7, 1), "Placement callback was falsely accepted or created conflicting custody.");
		FlushCasting(native);
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.BodiesGameItems.Any(x => x.GameItemId == redirected!.Id) &&
			db.CellsGameItems.Count(x => x.GameItemId == redirected!.Id) == 1, "Normal save persisted conflicting body/cell placement.");
		RunItemReaderProcess(new StormPlacementReader(database.Name, fixture, RuntimeClock.UtcNow, redirected!.Id, invocation, spell.Id, cap.Id,
			caster.MagicResourceAmounts[native.Resource]), "--storm-spear-placement-reader");
		Console.WriteLine("ARMStorm-placement-callback=passed actual-Held-inventory-callback-drops-output post-Get-custody-revalidation paid-NeedsReview no-false-wield normal-save fresh-process single-cell-custody no-replay-or-refund");
		return 0;
	}

	private static int RunStormSpearPlacementReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<StormPlacementReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true);
		FinaliseStormTags(database, host.Native.World);
		var world = host.Native.World; var items = new SpellOwnedItemService(world); host.Native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(items);
		var item = world.TryGetItem(input.Item, true)!;
		using (var db = NewIndependentContext(database.ConnectionString)) Require(!db.BodiesGameItems.Any(x => x.GameItemId == input.Item) &&
			db.CellsGameItems.Count(x => x.GameItemId == input.Item) == 1 && item.InInventoryOf is null && !host.Native.Body.WieldedItems.Contains(item), "Fresh load restored conflicting custody.");
		var casting = new MagicCastingService(world, clock: () => RuntimeClock.UtcNow);
		var replay = casting.Cast(new(host.Native.Actor, input.Capability, input.Spell, 1, false, "self", OriginId: input.Invocation));
		Require(replay.Status == MagicCastingStatus.Refused && host.Native.Actor.MagicResourceAmounts[host.Native.Resource] == input.Balance, "Restart replayed or refunded paid placement uncertainty.");
		Console.WriteLine("ARMStorm-placement-reader=passed fresh-process real-item no-body-membership exact-one-cell-reference paid-operation-quarantined no-replay-or-refund"); return 0;
	}

	private static int RunStormSpearStockReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<StormSpearReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		var host = PrepareRetirementHost(database, input.Fixture, clock, wielding: true); ConfigureStormHands(host.Native);
		FinaliseStormTags(database, host.Native.World);
		var world = host.Native.World; var service = new SpellOwnedItemService(world); host.Native.WorldMock.SetupGet(x => x.SpellOwnedItems).Returns(service);
		var item = world.TryGetItem(input.Item, true)!;
		Require(item.SpellCreationOrigin?.DeadlineUtc == input.Deadline && item.SpellCreationOrigin.IsTemporary && !host.Items.Has(input.Bag), "Fresh loading lost deadline or preloaded custodian.");
		service.ReconcileRetirements(RuntimeClock.UtcNow); Require(!item.Deleted, "Fresh process expired output early.");
		clock.Advance(input.Deadline - RuntimeClock.UtcNow); service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(!item.Deleted && host.Store.Find(input.Lifecycle)!.State == SpellLifecycleState.Retiring, "Unloaded foreign custodian did not hold removal.");
		var bag = world.TryGetItem(input.Bag, true)!; bag.FinaliseLoadTimeTasks();
		Require(ReferenceEquals(item.ContainedIn, bag), "Native foreign bag did not reconnect exact spear.");
		service.ReconcileRetirements(RuntimeClock.UtcNow); service.ReconcileRetirements(RuntimeClock.UtcNow); world.SaveManager.Flush();
		using var db = NewIndependentContext(database.ConnectionString);
		Require(item.Deleted && host.Store.Find(input.Lifecycle)!.State == SpellLifecycleState.Completed && db.GameItems.Find(input.Sibling)!.ContainerId == input.Bag && db.GameItems.Any(x => x.Id == input.Bag), "Restart expiry removed foreign sibling or did not complete exactly once.");
		var definition = XElement.Parse(db.MagicSpells.AsNoTracking().Single(x => x.Id == input.Spell).Definition);
		Require(definition.Element("StockIdentity")?.Value == ArmageddonStormSpearStock.Key && definition.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!.Element("Seconds")!.Value == ArmageddonStormSpearStock.LifetimeSeconds, "Persisted stock formula drifted.");
		Console.WriteLine("ARMStorm-reader=passed fresh-process original-stock-formula persisted-deadline no-reset no-early-expiry cold-custody-hold native-custodian-load exact-expiry repeated-reconciliation foreign-bag-and-child-preserved"); return 0;
	}
}

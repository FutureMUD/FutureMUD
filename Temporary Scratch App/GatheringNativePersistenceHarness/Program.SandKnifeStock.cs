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
using MudSharp.Climate.WeatherEvents;
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

	private sealed record SandKnifeReader(string Database, FixtureIds Fixture, DateTime Now, long Spell, long Item, Guid Lifecycle, DateTime Deadline, long Bag, long Sibling, long Permanent, long PermanentPrototype);
	private static void SeedSandFixture(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);

		var componentTag = new Db.Tag { Name = "Sand Creation 6" }; db.Tags.Add(componentTag); db.SaveChanges();
		db.Tags.Add(new Db.Tag { Name = "Sand storm flag" }); db.SaveChanges();
		var higherTag = new Db.Tag { Name = "Sand Creation 7", ParentId = componentTag.Id }; db.Tags.Add(higherTag); db.SaveChanges();
		var knife = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B flame knife");
		var token = db.GameItemProtos.Include(x => x.GameItemProtosGameItemComponentProtos).Single(x => x.Name == "ARM03B2B creation token");
		long id = db.GameItemProtos.Max(x => x.Id);
		foreach (var name in Enumerable.Range(1,6).Select(x => $"ARM03B2B Sand knife {x}").Concat(Enumerable.Range(1,8).Select(x => $"ARM03B2B Sand staff {x}")).Concat(["ARM03B2B Sand token 6", "ARM03B2B Sand token 7"]))
		{
			var basis = name.Contains("Sand token") ? token : knife;
			var proto = new Db.GameItemProto { Id = ++id, Name = name, Keywords = name, ShortDescription = "a declared " + name,
				FullDescription = "Declared native piercing profile; historical object statistics unavailable.", MaterialId = basis.MaterialId, Size = 1, Weight = 1,
				BaseItemQuality = (int)ItemQuality.Standard, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)MudSharp.Framework.Revision.RevisionStatus.Current } };
			foreach (var component in basis.GameItemProtosGameItemComponentProtos) proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.GameItemComponentProtoId });
			if (name == "ARM03B2B Sand token 6") proto.GameItemProtosTags.Add(new() { TagId = componentTag.Id });
			if (name == "ARM03B2B Sand token 7") proto.GameItemProtosTags.Add(new() { TagId = higherTag.Id });
			db.GameItemProtos.Add(proto); db.SaveChanges();
		}
		var expressions = new[] { "6", "4", "2" }.Select((formula, index) =>
			new Db.TraitExpression { Name = $"Sand declared fixture channel {index}", Expression = formula }).ToArray();
		db.TraitExpressions.AddRange(expressions); db.SaveChanges();
		db.WeaponAttacks.Add(new Db.WeaponAttack { Name = "Sand declared piercing thrust", WeaponTypeId = db.WeaponTypes.Single(x => x.Name.StartsWith("ARM03C1")).Id,
			MoveType = (int)BuiltInCombatMoveType.UseWeaponAttack, DamageType = (int)DamageType.Piercing,
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

	private static int RunSandKnifeStockChecks()
	{
		using var database = TestDatabase.CreateFresh("futuremud_land_"); ConfigureNativeDatabase(database.ConnectionString);
		var fixture = FixtureSeed.Create(database, "sand_knife_stock", true);
		var clock = new HarnessClock(); using var time = RuntimeClock.Push(clock);
		using (var db = NewIndependentContext(database.ConnectionString)) db.Database.Migrate();
		var seed = NativeRuntime.Load(fixture, database.ConnectionString, true); ConfigureCastingWorld(seed, database.ConnectionString, true);
		SeedRetirementPrototypes(database, seed.World.Materials.First().Id); SeedCreatedWeaponPrototypes(database, seed.World.Materials.First().Id);
		SeedSandFixture(database);
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
			foreach (var model in db.TraitExpressions.Where(x => x.Name.StartsWith("Sand declared")))
				if (!expressions.Has(model.Id)) expressions.Add(new TraitExpression(model, world));
			attacks.Add(WeaponAttack.LoadWeaponAttack(db.WeaponAttacks.Single(x => x.Name == "Sand declared piercing thrust"), world));
		}

		var weaponType = (WeaponType)world.WeaponTypes.Single(x => x.Name.StartsWith("ARM03C1"));
		foreach (var attack in attacks) if (!weaponType.Attacks.Contains(attack)) weaponType.AddAttack(attack);
		native.WorldMock.SetupGet(x => x.CombatMessageManager).Returns(new CombatMessageManager(world));
		Mock.Get(world.GetCheck(CheckType.MeleeWeaponPenetrateCheck)).Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponPenetrateCheck, Outcome.Pass));
		native.WorldMock.Setup(x => x.Add(It.IsAny<ITraitExpression>())).Callback<ITraitExpression>(x => expressions.Add(x));
		native.WorldMock.Setup(x => x.Add(It.IsAny<IMagicSpell>())).Callback<IMagicSpell>(x => ((All<IMagicSpell>)world.MagicSpells).Add(x));
		native.WorldMock.SetupGet(x => x.AlwaysFalseProg).Returns(world.FutureProgs.GetByName("AlwaysFalse")!);
		native.WorldMock.Setup(x => x.Add(It.IsAny<MudSharp.FutureProg.IFutureProg>())).Callback<MudSharp.FutureProg.IFutureProg>(x => ((All<MudSharp.FutureProg.IFutureProg>)world.FutureProgs).Add(x));
		var cap = (SkillLevelBasedMagicCapability)native.Capability; var trait = world.Traits.GetByName("ARM02 Earth Proficiency")!;
		var knives = Enumerable.Range(1,6).Select(x => host.Prototypes.Values.Single(p => p.Name == $"ARM03B2B Sand knife {x}")).ToArray();
		var staffs = Enumerable.Range(1,8).Select(x => host.Prototypes.Values.Single(p => p.Name == $"ARM03B2B Sand staff {x}")).ToArray();
		var monTag = world.Tags.GetByName("Sand Creation 6")!;
		var sandstormTag = world.Tags.GetByName("Sand storm flag")!;
		var weatherEvents = new All<IWeatherEvent>(); native.WorldMock.SetupGet(x => x.WeatherEvents).Returns(weatherEvents);
		var sandstorm = new SimpleWeatherEvent(world, "Sand stock declared storm"); weatherEvents.Add(sandstorm);
		var otherWeather = new SimpleWeatherEvent(world, "Sand stock unmapped strong wind"); weatherEvents.Add(otherWeather);
		Require(otherWeather.BuildingCommand(caster,new StringStack("wind GaleWind")) && otherWeather.Wind == WindLevel.GaleWind,"Weather wind fixture refused.");
		var weatherController = new Mock<IWeatherController>(); weatherController.SetupGet(x=>x.CurrentWeatherEvent).Returns(sandstorm);
		var command = $"stock sand-knife {cap.School.Id} {trait.Id} {native.Resource.Id} {string.Join(' ', knives.Concat(staffs).Select(x => x.Id))} {monTag.Id} {sandstormTag.Id} {sandstorm.Id}";
		var originalOutput = caster.OutputHandler; var builderMessages = new List<string>();
		var builderOutput = new Mock<MudSharp.PerceptionEngine.IOutputHandler>();
		builderOutput.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Callback<string,bool,bool>((text,_,__) => builderMessages.Add(text)).Returns(true);
		SetPrivateMember(caster,"OutputHandler",builderOutput.Object);
		try { EditableItemHelper.MagicSpellHelper.EditableNewAction(caster, new StringStack(command)); }
		finally { SetPrivateMember(caster,"OutputHandler",originalOutput); }
		Require(world.MagicSpells.Any(x => x.Name == ArmageddonSandKnifeStock.Name), "Builder stock creation refused: " + string.Join("; ",builderMessages));
		var spell = (MagicSpell)world.MagicSpells.Single(x => x.Name == ArmageddonSandKnifeStock.Name);
		Require(spell.ReadyForGame && spell.GradeConfigurationErrors().Count == 0 && spell.StockIdentity == ArmageddonSandKnifeStock.Key, "Stock not ready: " + string.Join(';', spell.GradeConfigurationErrors()));
		var count = world.MagicSpells.Count(); EditableItemHelper.MagicSpellHelper.EditableNewAction(caster, new StringStack(command)); Require(world.MagicSpells.Count() == count, "Duplicate stock added content.");
		Require(spell.BuildingCommand(caster, new StringStack("plan grade 1 all")) && spell.BuildingCommand(caster, new StringStack("plan grade 1 7")), "Material grade builder failed.");
		var effect = (CreateItemEffect)spell.SpellEffects.Single();
		Require(effect.BuildingCommand(caster, new StringStack($"output 2 {knives[1].Id}")) && !effect.BuildingCommand(caster, new StringStack("output 2 99999999")), "Output pool edit/refusal failed.");
		using (new FMDB()) { spell.Save(); FMDB.Context.SaveChanges(); }
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var loaded = new MagicSpell(db.MagicSpells.AsNoTracking().Single(x => x.Id == spell.Id), world);
			var policy = (CreateItemEffect)loaded.SpellEffects.Single();
			Require(policy.GradeOutputs.Count == 7 && policy.GradeOutputs[7].Length == 8 && policy.PermanentGrade == 7 && !policy.PrimaryHand && policy.EligibilityProg is not null && policy.LifetimeMultiplierProg is null && loaded.InventoryPlanTemplate.Phases.First().Actions.Single().SaveToXml().Attribute("grade")!.Value == "7", "Stock reload lost grade/output/environment rules.");
			((All<IMagicSpell>)world.MagicSpells).Remove(spell); ((All<IMagicSpell>)world.MagicSpells).Add(loaded); spell = loaded;
		}
		Console.WriteLine("ARMSand-stock-builder=passed six-distinct-temporary-grades eight-permanent-staff-pool mon-only-component standard-placement sufficient-sand-prog authored-energy-floor-five source-C-minimum-zero persisted-reload duplicate-refusal invalid-edit-preserved");
		caster.RemoveAllEffects<BuilderEditingEffect<IMagicSpell>>(null,true);
		foreach (var value in new[] { $"casting trait {trait.Id}", $"casting resources {native.Resource.Id} {native.Resource.Id} passive", $"casting entry add {spell.Id}", $"casting entry skill {spell.Id} 30 90 relative", $"casting entry starting {spell.Id} on", "casting enable on" }) Require(cap.BuildingCommand(caster,new StringStack(value)), "Capability refused " + value);
		caster.SetMerits([NativeRuntime.NewCapabilityMerit(cap)]); caster.SetTraitValue(trait,90); world.SaveManager.Flush();
		var staff = new Mock<ICharacter>(); staff.SetupGet(x => x.Id).Returns(999); staff.Setup(x => x.IsAdministrator(PermissionLevel.JuniorAdmin)).Returns(true);
		var casting = new MagicCastingService(world,clock:()=>RuntimeClock.UtcNow,random:()=>0.1,flush:()=>FlushCasting(native)); native.WorldMock.SetupGet(x => x.MagicCasting).Returns(casting);
		Require(casting.Enrol(staff.Object,caster,cap.Id,"Sand stock acceptance").Allowed,"Stock enrolment failed.");
		new MagicCastingStateStore().Write(acquired:casting.Acquisition(caster,spell.Id)! with { ControlledGrade=7 });
		GameItem New(string name)
		{
			var item=(GameItem)host.Prototypes.Values.Single(x=>x.Name==name).CreateNew(caster); world.Add(item); caster.Location.Insert(item,true); item.Login(); world.SaveManager.Flush(); return item;
		}
		void Carry(IGameItem item) { native.Body.Get(item,silent:true); Require(native.Body.Swap(item,null!),"Component hand swap failed."); }
		(int Items,int Origins) Rows() { using var db=NewIndependentContext(database.ConnectionString); return (db.GameItems.Count(),db.MagicSpellLifecycles.Count()); }
		void Refused(int grade,string reason)
		{
			caster.RemoveAllEffects<MagicSpellLockout>(null,true); caster.AddResource(native.Resource,100); var before=Rows(); var balance=caster.MagicResourceAmounts[native.Resource];
			var result=casting.Cast(new(caster,cap.Id,spell.Id,grade,false,""));
			Require(result.Status==MagicCastingStatus.Refused && result.OperationId is null && Rows()==before && caster.MagicResourceAmounts[native.Resource]==balance,"Prepayment conservation failed " + reason + ": " + result.Message);
		}
		// Admit a sandy terrain before component-specific refusals.
		var oldCell = caster.Location;
		var cell = CreateStormCell(native, database, fixture.CellId);
		foreach (var item in oldCell.GameItems.ToArray()) { oldCell.Extract(item); cell.Insert(item, true); }
		SetPrivateMember(caster, "Location", cell); ((List<ICharacter>)cell.Characters).Add(caster);
		Mock.Get(native.Body.Race).SetupGet(x => x.NaturalPerceptionTypes).Returns(PerceptionTypes.DirectVisual);
		Mock.Get(native.Body.Prototype).SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(world));
		var initialTerrain=(Terrain)world.Terrains.Single(); Require(initialTerrain.BuildingCommand(caster,new StringStack("name Desert")),"Initial sandy terrain failed.");
		caster.AddResource(native.Resource,100);
		var baselineQuote=casting.Quote(new(caster,cap.Id,spell.Id,1,false,""));
		Require(baselineQuote.Allowed,"Sandy baseline admission failed before component-specific tests: " + baselineQuote.Reason + "; native terrain=" + cell.Terrain(caster).Name);
		Refused(7,"no mon component"); var lowToken=New("ARM03B2B creation token"); Carry(lowToken); Refused(7,"under-ranked wrong tag");
		((All<ITag>)world.Tags).Remove(monTag); Refused(7,"unresolved mon tag and unrelated actual carried item");
		Require(!lowToken.Deleted && spell.InventoryPlanTemplate.Phases.First().Actions.Single().SaveToXml().Attribute("tag")!.Value==monTag.Id.ToString(),"Missing tag consumed unrelated actual item or erased configured ID.");
		((All<ITag>)world.Tags).Add(monTag); native.Body.Drop(lowToken,silent:true);
		var monToken=New("ARM03B2B Sand token 6"); Refused(7,"ground mon component");
		var foreignBag=New("ARM03B2B bag"); var bag=foreignBag.GetItemType<IContainer>()!; caster.Location.Extract(monToken); bag.Put(caster,monToken,false); Refused(7,"contained mon component");
		foreignBag.Take(monToken); caster.Location.Insert(monToken,true);
		Console.WriteLine("ARMSand-prepayment=passed missing-low-rank-ground-contained-mon-component-refused zero-payment zero-operation zero-created-row");
		GameItem Cast(int grade)
		{
			caster.RemoveAllEffects<MagicSpellLockout>(null,true); caster.AddResource(native.Resource,100); var before=Rows(); var balance=caster.MagicResourceAmounts[native.Resource]; var now=RuntimeClock.UtcNow;
			var result=casting.Cast(new(caster,cap.Id,spell.Id,grade,false,"")); Require(result.Status==MagicCastingStatus.Succeeded,"Paid stock failed: " + result.Message);
			var item=(GameItem)host.Items.Single(x=>x.SpellCreationOrigin is not null && host.Store.Find(x.SpellCreationOrigin.LifecycleId)!.Origin.Grade==grade && host.Store.Find(x.SpellCreationOrigin.LifecycleId)!.Origin.Family=="sand-knife" && !x.Deleted);
			var life=host.Store.Find(item.SpellCreationOrigin!.LifecycleId)!;
			Require(caster.MagicResourceAmounts[native.Resource]==balance-spell.GradeProfile!.Efficiency!.Cost(7,grade) && Rows().Origins==before.Origins+1 && Rows().Items==before.Items+(grade==7?0:1),"Payment/count failed.");
			Require(grade==7 ? monToken.Deleted && life.Origin.Mode==SpellLifecycleMode.Permanent && life.Origin.DeadlineUtc is null && staffs.Any(x=>x.Id==item.Prototype.Id) : !monToken.Deleted && !lowToken.Deleted && life.Origin.Mode==SpellLifecycleMode.TemporaryCleanup && item.Prototype.Id==knives[grade-1].Id && (life.Origin.DeadlineUtc!.Value-now).TotalSeconds==(grade+1)*1125,"Source grade/permanence/component/lifetime rule failed.");
			Require(ReferenceEquals(item.InInventoryOf,native.Body) && native.Body.HeldItems.Contains(item) && !native.Body.WieldedItems.Contains(item),"Source standard inventory placement failed.");
			Require(native.Body.Wield(item,native.Body.WieldLocs.OrderBy(x => x.Id).First(),silent:true) && native.Body.WieldedItems.Contains(item),"Native wield of stock output failed.");
			FlushCasting(native); Console.WriteLine($"ARMSand-paid-grade{grade}=passed actual-payment component-only-at-mon prototype:{item.Prototype.Id} native-standard-get explicit-native-wield mode:{life.Origin.Mode} deadline:{life.Origin.DeadlineUtc:o}"); return item;
		}
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
		var template = new SimpleNPCTemplate(world, DummyAccount.Instance, data, "Sand opponent"); templates.Add(template);
		var opponent = (NPC)template.CreateNewCharacter(cell); world.Add(opponent, true); world.Add(opponent.Body); opponent.CombatSettings = caster.CombatSettings; cell.Enter(opponent);
		opponent.Body.Handedness = Alignment.Right;
		void Strike(GameItem item, int grade, ICharacter? attacker = null, ICharacter? target = null)
		{
			attacker ??= caster; target ??= opponent;
			attacker.TargettedBodypart = target.Body.Bodyparts.OfType<IExternalBodypart>().First();
			var before = target.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun);
			var move = new MeleeWeaponAttack(attacker, item.GetItemType<IMeleeWeapon>()!, attacks.Single(), target);
			var result = move.ResolveMove(new HelplessDefenseMove { Assailant = target });
			Require(result.MoveWasSuccessful && result.WoundsCaused.Any(x => x.DamageType == DamageType.Piercing && x.Parent == target && target.Body.Wounds.Contains(x)) &&
				target.Body.Wounds.Sum(x => x.CurrentDamage + x.CurrentPain + x.CurrentStun) > before, "Native sand weapon attack delivered no piercing wounds to the target's body.");
			world.SaveManager.Flush(); using var db = NewIndependentContext(database.ConnectionString);
			var persisted = db.Wounds.Where(x => x.BodyId == target.Body.Id && x.DamageType == (int)DamageType.Piercing).ToArray();
			var piercing = target.Body.Wounds.Where(x => x.DamageType == DamageType.Piercing).ToArray();
			Require(persisted.Length > 0 && Same(persisted.Sum(x => x.CurrentDamage), piercing.Sum(x => x.CurrentDamage)) &&
				Same(persisted.Sum(x => x.CurrentPain), piercing.Sum(x => x.CurrentPain)) && Same(persisted.Sum(x => x.CurrentStun), piercing.Sum(x => x.CurrentStun)),
				"Piercing wound channels did not persist for the target body.");
			Console.WriteLine($"ARMSand-combat-grade{grade}=passed real-MeleeWeaponAttack actual-piercing-target-wounds persisted-native-wounds declared-6-damage-4-pain-2-stun helpless-defence controlled-roll no-historical-damage-parity");
		}
		void TerrainName(string name)
		{
			var terrain=(Terrain)world.Terrains.Single();
			if(terrain.Name.EqualTo(name)) return;
			Require(terrain.BuildingCommand(caster,new StringStack($"name \"{name}\"")) && terrain.Name.EqualTo(name),"Terrain rename failed: " + name);
		}
		foreach(var name in new[]{"Desert","Earth Plane","Silt","Shallows","Salt Flats"})
		{
			TerrainName(name); Require(casting.Quote(new(caster,cap.Id,spell.Id,1,false,"")).Allowed,"Sandy terrain refused " + name);
		}
		Carry(monToken); Require(!monToken.Deleted && casting.Quote(new(caster,cap.Id,spell.Id,7,false,"")).Allowed,"Mon baseline admission failed before environment refusals.");
		TerrainName("Field"); Refused(1,"no terrain sand no storm no weather"); Refused(7,"no sand mon with actual eligible carried component");
		Require(cell.AddTag(sandstormTag) && casting.Quote(new(caster,cap.Id,spell.Id,1,false,"")).Allowed,"Actual storm room tag not admitted.");
		cell.RemoveTag(sandstormTag); var zone=(Zone)cell.Zone; SetPrivateField(zone, "_weather", weatherController.Object);
		Require(casting.Quote(new(caster,cap.Id,spell.Id,1,false,"")).Allowed,"Selected actual native weather not admitted.");
		weatherController.SetupGet(x=>x.CurrentWeatherEvent).Returns(otherWeather); Refused(1,"unmapped actual gale weather does not supply sand");
		weatherController.SetupGet(x=>x.CurrentWeatherEvent).Returns(sandstorm); cell.AddTag(sandstormTag);
		foreach(var name in new[]{"Inside","City"}) { TerrainName(name); Refused(1,"blocked terrain despite room flag and weather " + name); Refused(7,"blocked mon despite both storm mappings " + name); }
		cell.RemoveTag(sandstormTag); SetPrivateField(zone, "_weather", null!); TerrainName("Desert");
		Require(!monToken.Deleted,"Environment refusal consumed mon component."); native.Body.Drop(monToken,silent:true);
		Console.WriteLine("ARMSand-environment=passed five-native-terrains actual-room-storm-tag selected-native-weather null-unmapped-gale-weather-refused Inside-City-refuse-despite-both-storm-mappings no-payment controlled-controller actual-native-event-and-cell source-condition-not-inferred");
		GameItem? restartItem=null;
		for (var grade=1;grade<=6;grade++)
		{
			if(grade==2) { TerrainName("Field"); SetPrivateField(zone, "_weather", weatherController.Object); }
			if(grade==6) { TerrainName("Shadow Plane"); cell.AddTag(sandstormTag); }
			var item=Cast(grade); Strike(item,grade); if(grade==2) { SetPrivateField(zone, "_weather", null!); TerrainName("Desert"); }
			var deadline=item.SpellCreationOrigin!.DeadlineUtc!.Value;
			Require(!item.GetItemType<ISalvageable>()!.CanSalvage(out _),"Temporary output acquired ordinary salvage value.");
			if(grade==6) { restartItem=item; native.Body.Take(item); bag.Put(caster,item,false); cell.RemoveTag(sandstormTag); TerrainName("Desert"); break; }
			clock.Advance(deadline-RuntimeClock.UtcNow-TimeSpan.FromTicks(1)); effects.CheckSchedules(); items.ReconcileRetirements(RuntimeClock.UtcNow); Require(!item.Deleted,"Early temporary expiry.");
			clock.Advance(TimeSpan.FromTicks(1)); items.ReconcileRetirements(RuntimeClock.UtcNow); items.ReconcileRetirements(RuntimeClock.UtcNow);
			Require(item.Deleted && !native.Body.WieldedItems.Contains(item) && host.Store.Find(item.SpellCreationOrigin.LifecycleId)!.State==SpellLifecycleState.Completed && !foreignBag.Deleted && !lowToken.Deleted,"Temporary expiry lost exact ownership or foreign goods.");
		}
		Console.WriteLine("ARMSand-threshold-variants=passed all-six-paid-prototypes grades1-to5-exact-wielded-expiry tagged-shadow-grade6-full-duration no-sand-source-shadow-reduction immutable-deadline temporary-value-block");
		Carry(monToken); caster.RemoveAllEffects<MagicSpellLockout>(null,true); caster.AddResource(native.Resource,100);
		Require(casting.Quote(new(caster,cap.Id,spell.Id,7,false,"")).Allowed && !monToken.Deleted,"Exact rank-six boundary quote failed.");
		native.Body.Drop(monToken,silent:true); var exactSix = monToken; monToken = New("ARM03B2B Sand token 7"); Carry(monToken);
		var permanent=Cast(7); Strike(permanent,7); Require(!exactSix.Deleted,"Higher component cast consumed unrelated exact-six token.");
		Console.WriteLine("ARMSand-component-threshold=passed exact-six-prepayment-admitted higher-descendant-seven-paid-and-consumed wrong-lower-tags-refused exact-six-unrelated-preserved");
		Require(permanent.GetItemType<ISalvageable>()!.CanSalvage(out _) && host.Store.Find(permanent.SpellCreationOrigin!.LifecycleId)!.State==SpellLifecycleState.Completed,"Permanent staff retained temporary value/retirement state.");
		native.Body.Take(permanent); bag.Put(caster,permanent,false); var sibling=New("ARM03B2B goods"); cell.Extract(sibling); bag.Put(caster,sibling,false);
		world.SaveManager.Flush(); FlushCasting(native);
		using(var db=NewIndependentContext(database.ConnectionString))
		{
			if(!db.CellsGameItems.Any(x=>x.GameItemId==foreignBag.Id)) db.CellsGameItems.Add(new(){CellId=fixture.CellId,GameItemId=foreignBag.Id}); db.SaveChanges();
			Require(db.GameItems.Find(restartItem!.Id)!.ContainerId==foreignBag.Id && db.GameItems.Find(permanent.Id)!.ContainerId==foreignBag.Id,"Foreign containment was not persisted.");
		}
		RunItemReaderProcess(new SandKnifeReader(database.Name,fixture,RuntimeClock.UtcNow,spell.Id,restartItem!.Id,restartItem.SpellCreationOrigin!.LifecycleId,restartItem.SpellCreationOrigin.DeadlineUtc!.Value,foreignBag.Id,sibling.Id,permanent.Id,permanent.Prototype.Id),"--sand-knife-stock-reader");
		using(var db=NewIndependentContext(database.ConnectionString)) Require(!db.GameItems.Any(x=>x.Id==restartItem.Id) && db.GameItems.Any(x=>x.Id==permanent.Id) && db.GameItems.Find(sibling.Id)!.ContainerId==foreignBag.Id,"Fresh expiry lost permanent/foreign goods.");
		Console.WriteLine("ARMSand-restart-expiry=passed real-fresh-process sand-temporary-expiry exact-owned-leaf permanent-staff-foreign-bag-and-sibling-survive no-reset-or-replay");
		return 0;
	}

	private static int RunSandKnifeStockReader(string[] args)
	{
		var input = JsonSerializer.Deserialize<SandKnifeReader>(Encoding.UTF8.GetString(Convert.FromBase64String(args.Single())))!;
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
		var permanent = world.TryGetItem(input.Permanent, true)!;
		Require(permanent.Prototype.Id == input.PermanentPrototype && permanent.SpellCreationOrigin is { IsTemporary: false, DeadlineUtc: null } &&
			ReferenceEquals(permanent.ContainedIn, bag), "Fresh reload lost permanent staff prototype, lifetime or foreign custody.");
		Require(ReferenceEquals(item.ContainedIn, bag), "Native foreign bag did not reconnect exact sand-knife.");
		service.ReconcileRetirements(RuntimeClock.UtcNow); service.ReconcileRetirements(RuntimeClock.UtcNow); world.SaveManager.Flush();
		using var db = NewIndependentContext(database.ConnectionString);
		Require(item.Deleted && host.Store.Find(input.Lifecycle)!.State == SpellLifecycleState.Completed && db.GameItems.Find(input.Sibling)!.ContainerId == input.Bag && db.GameItems.Any(x => x.Id == input.Bag), "Restart expiry removed foreign sibling or did not complete exactly once.");
		clock.Advance(TimeSpan.FromDays(30)); service.ReconcileRetirements(RuntimeClock.UtcNow);
		Require(!permanent.Deleted && db.GameItems.Any(x => x.Id == input.Permanent) && db.GameItems.Find(input.Permanent)!.ContainerId == input.Bag,
			"Permanent output expired or changed foreign custody after restart.");
		var definition = XElement.Parse(db.MagicSpells.AsNoTracking().Single(x => x.Id == input.Spell).Definition);
		Require(definition.Element("StockIdentity")?.Value == ArmageddonSandKnifeStock.Key && definition.Element("Effects")!.Element("Effect")!.Element("Lifecycle")!.Element("Seconds")!.Value == ArmageddonSandKnifeStock.LifetimeSeconds, "Persisted stock formula drifted.");
		Console.WriteLine("ARMSand-reader=passed fresh-process original-stock-formula persisted-deadline no-reset no-early-expiry cold-custody-hold native-custodian-load exact-expiry repeated-reconciliation foreign-bag-and-child-preserved"); return 0;
	}
}

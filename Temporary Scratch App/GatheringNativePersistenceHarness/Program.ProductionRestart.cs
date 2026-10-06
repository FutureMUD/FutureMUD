#nullable enable

using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Effects.Interfaces;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Form.Characteristics;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Logging;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine.Light;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private sealed record ProductionRestartReader(string Database, FixtureIds Fixture, DateTime Now, Guid Origin,
		long Instance, long Corpse, long Owner, long Body, long Foreign, long PositiveNpc, long QueuedItem,
		DateTime Deadline, string Scenario, ItemOwnershipReference? ForeignTitle, int ForeignQuantity,
		string PaidEffectHash, int MorphSeconds);

	// These are disposable fixture repairs. No engine loader or gameplay behavior is changed.
	private static void SeedProductionLoaderFixture(TestDatabase database, FixtureIds fixture)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		foreach (var (key, value) in DefaultStaticSettings.DefaultStaticConfigurations)
			if (!db.StaticConfigurations.Any(x => x.SettingName == key)) db.StaticConfigurations.Add(new() { SettingName = key, Definition = value });
		foreach (var (key, value) in DefaultStaticSettings.DefaultStaticStrings)
			if (!db.StaticStrings.Any(x => x.Id == key)) db.StaticStrings.Add(new() { Id = key, Text = value });
		db.SaveChanges();
		long Prog(string name, string text, ProgVariableTypes result, bool character = false)
		{
			var row = db.FutureProgs.SingleOrDefault(x => x.FunctionName == name);
			if (row is null)
			{
				row = new Db.FutureProg { FunctionName = name, FunctionComment = "Owned production-loader fixture.", FunctionText = text,
					Category = "Harness", Subcategory = "Production Restart", ReturnType = (long)result, StaticType = 0 };
				if (character) row.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Character });
				db.FutureProgs.Add(row); db.SaveChanges();
			}
			return row.Id;
		}
		Prog("AlwaysTrue", "return true;", ProgVariableTypes.Boolean);
		Prog("AlwaysFalse", "return false;", ProgVariableTypes.Boolean);
		var zero = Prog("AlwaysZero", "return 0;", ProgVariableTypes.Number);
		Prog("AlwaysOne", "return 1;", ProgVariableTypes.Number);
		Prog("AlwaysOneHundred", "return 100;", ProgVariableTypes.Number);
		void Setting(string name, string value)
		{
			var row = db.StaticConfigurations.SingleOrDefault(x => x.SettingName == name);
			if (row is null) db.StaticConfigurations.Add(new() { SettingName = name, Definition = value });
			else row.Definition = value;
		}
		Setting("TotalBloodVolumeProg", Prog("ARMBOOT_Blood", "return 5;", ProgVariableTypes.Number, true).ToString());
		Setting("LiverFunctionProg", Prog("ARMBOOT_Liver", "return 0.01;", ProgVariableTypes.Number, true).ToString());
		Setting("MaximumStaminaProg", Prog("ARMBOOT_Stamina", "return 100;", ProgVariableTypes.Number, true).ToString());
		Setting("DefaultItemHealthStrategy", db.HealthStrategies.Single(x => x.Name == "ARM03B2B item health").Id.ToString());
		Setting("BaseWeightUOMToKilograms", "1"); Setting("BaseHeightUOMToMetres", "1"); Setting("BaseFluidUOMToLitres", "0.001");
		foreach (var (type, name, abbreviation) in new[] { (UnitType.Mass,"kilogram","kg"), (UnitType.Length,"metre","m"), (UnitType.FluidVolume,"millilitre","ml") })
			db.UnitsOfMeasure.Add(new() { Name = name, PrimaryAbbreviation = abbreviation, Abbreviations = abbreviation, BaseMultiplier = 1, Type = (int)type, Describer = true, SpaceBetween = true, System = "Metric", DefaultUnitForSystem = true });
		var checkTemplate = new Db.CheckTemplate { Name = "Skill Check", CheckMethod = "Standard", ImproveTraits = false, CanBranchIfTraitMissing = false };
		foreach (var difficulty in Enum.GetValues<Difficulty>()) checkTemplate.CheckTemplateDifficulties.Add(new() { Difficulty = (int)difficulty });
		db.CheckTemplates.Add(checkTemplate);
		foreach (var check in Enum.GetValues<CheckType>().Distinct().Where(x => x != CheckType.None))
			db.Checks.Add(new() { Type = (int)check, CheckTemplate = checkTemplate, TraitExpressionId = fixture.TraitExpressionId, MaximumDifficultyForImprovement = (int)Difficulty.Impossible });
		var decorator = new Db.TraitDecorator { Name = "Fixture numeric", Type = "SimpleNumeric", Contents = "<Definition />" };
		var improver = new Db.Improver { Name = "Fixture fixed traits", Type = "non-improving", Definition = "<Definition />" };
		db.TraitDecorators.Add(decorator); db.Improvers.Add(improver); db.SaveChanges();
		var relativeHeight = new Db.CharacteristicDefinition { Name = "Fixture relative height", Type = (int)CharacteristicType.RelativeHeight, Pattern = "relativeheight", Description = "Broad disposable fixture height descriptor.", Model = "standard", Definition = "<Definition />" };
		relativeHeight.CharacteristicValues.Add(new() { Name = "ordinary height", Value = "0", AdditionalValue = "1000000", Default = true });
		db.CharacteristicDefinitions.Add(relativeHeight);
		db.StackDecorators.Add(new() { Name = "Fixture suffix stacks", Type = "Suffix", Description = "Native default stack display.", Definition = "<Definition />" });
		db.AuthorityGroups.Add(new() { Name = "Player", AuthorityLevel = 2, AccountsLevel = 2, InformationLevel = 2, CharactersLevel = 2, ItemsLevel = 2, PlanesLevel = 2, RoomsLevel = 2 });
		foreach (var trait in db.TraitDefinitions) { trait.DecoratorId = decorator.Id; trait.ImproverId = improver.Id; }
		foreach (var row in db.FutureProgs.Where(x => x.FunctionName.EndsWith("_availability"))) row.ReturnType = (long)ProgVariableTypes.Number;
		foreach (var culture in db.Cultures) culture.SkillStartingValueProgId = zero;
		var material = db.Materials.First().Id;
		var shape = new Db.BodypartShape { Name = "ARMBOOT fixture part" }; db.BodypartShapes.Add(shape); db.SaveChanges();
		foreach (var proto in db.BodyProtos.Include(x => x.WearSizeParameter).ToArray())
		{
			proto.WielderDescriptionSingle = "hand"; proto.WielderDescriptionPlural = "hands";
			proto.LegDescriptionSingular = "limb"; proto.LegDescriptionPlural = "limbs";
			proto.MinimumLegsToStand = 0; proto.NameForTracking = "fixture body";
			proto.WearSizeParameter.BodyProtoId = proto.Id;
			const string ratios = "<Ratios><Ratio Item=\"2\" Min=\"0\" Max=\"1000000\"/></Ratios>";
			proto.WearSizeParameter.WeightVolumeRatios = ratios; proto.WearSizeParameter.TraitVolumeRatios = ratios; proto.WearSizeParameter.HeightLinearRatios = ratios;
			Db.BodypartProto Part(long id, string name, BodypartTypeEnum type, bool organ) => new() {
				Id = id, BodyId = proto.Id, Name = name, Description = name, BodypartType = (int)type, BodypartShapeId = shape.Id,
				DefaultMaterialId = material, IsOrgan = organ ? 1 : 0, IsCore = true, IsVital = organ, MaxLife = 100,
				RelativeHitChance = 1, PainModifier = 1, DamageModifier = 1, StunModifier = 1, BleedModifier = 1,
				Size = (int)SizeCategory.Normal, MaxSingleSize = (int)SizeCategory.Huge, WeightLimit = 1000, SeverFormula = "100", Unary = true };
			var hand = Part(fixture.BodypartId, "fixture hand", BodypartTypeEnum.GrabbingWielding, false);
			var brain = Part(fixture.BodypartId + 1, "fixture brain", BodypartTypeEnum.Brain, true);
			var heart = Part(fixture.BodypartId + 2, "fixture heart", BodypartTypeEnum.Heart, true);
			Require(!db.BodypartProtos.Any(x => x.Id == hand.Id), "Fixture anatomy must be newly persisted, not overwritten.");
			var secondHand = Part(fixture.BodypartId + 3, "second fixture hand", BodypartTypeEnum.GrabbingWielding, false);
			db.BodypartProtos.AddRange(hand, brain, heart, secondHand); db.SaveChanges();
			db.BodypartInternalInfos.AddRange(new() { BodypartProtoId = hand.Id, InternalPartId = brain.Id, HitChance = 1, IsPrimaryOrganLocation = true, ProximityGroup = "fixture" }, new() { BodypartProtoId = hand.Id, InternalPartId = heart.Id, HitChance = 1, IsPrimaryOrganLocation = true, ProximityGroup = "fixture" });
			var limb = new Db.Limb { Name = "fixture limb", RootBodyId = proto.Id, RootBodypartId = hand.Id, LimbType = (int)LimbType.Arm, LimbDamageThresholdMultiplier = 1, LimbPainThresholdMultiplier = 1 };
			db.Limbs.Add(limb); db.SaveChanges(); db.LimbsBodypartProto.Add(new() { LimbId = limb.Id, BodypartProtoId = hand.Id });
			var secondLimb = new Db.Limb { Name = "second fixture limb", RootBodyId = proto.Id, RootBodypartId = secondHand.Id, LimbType = (int)LimbType.Arm, LimbDamageThresholdMultiplier = 1, LimbPainThresholdMultiplier = 1 };
			db.Limbs.Add(secondLimb); db.SaveChanges(); db.LimbsBodypartProto.Add(new() { LimbId = secondLimb.Id, BodypartProtoId = secondHand.Id });
			foreach (var (position, alias) in new[] { (PositionStanding.Instance.Id, "walk"), (PositionFloatingInZeroGravity.Instance.Id, "float") })
			{
				db.BodyProtosPositions.Add(new() { BodyProtoId = proto.Id, Position = (int)position });
				db.MoveSpeeds.Add(new() { BodyProtoId = proto.Id, PositionId = position, Alias = alias, FirstPersonVerb = alias, ThirdPersonVerb = alias + "s", PresentParticiple = alias + "ing", Multiplier = 1, StaminaMultiplier = 1 });
			}
		}
		var modelXml = new XElement("Definition", new XElement("Ranges", new XElement("Range", new XAttribute("state", 0), new XAttribute("lower", 0), new XAttribute("upper", 1000000))),
			new XElement("Terrains", new XAttribute("default", 1)), new XElement("Descriptions", new[] { "ShortDescriptions", "FullDescriptions", "ContentsDescriptions", "PartDescriptions" }.Select(group => new XElement(group, new XElement("Description", new XAttribute("state", 0), "a native corpse")))), new XElement("CorpseMaterials", new XElement("CorpseMaterial", new XAttribute("state", 0), material))).ToString();
		foreach (var row in db.CorpseModels) { row.Type = "Standard"; row.Definition = modelXml; }
		if (!db.CorpseModels.Any(x => x.Id == 71)) db.CorpseModels.Add(new() { Id = 71, Name = "ARMBOOT native decay", Type = "Standard", Description = "Owned fixture corpse model.", Definition = modelXml });
		foreach (var race in db.Races)
		{
			race.CorpseModelId = 71; race.HandednessOptions = "0"; race.DefaultHandedness = 0;
			race.IlluminationPerceptionMultiplier = 1; race.NeedsToBreathe = false; race.BreathingModel = "non-breather";
			race.BreathingVolumeExpression = "0"; race.HoldBreathLengthExpression = "100";
			race.MaximumLiftWeightExpression = "1000"; race.MaximumDragWeightExpression = "1000";
			race.BodypartHealthMultiplier = 1; race.PainToleranceMultiplier = 1;
		}
		const string clockXml = "<Clock><Alias>UTC</Alias><Description>Fixture UTC</Description><ShortDisplayString>$j:$m:$s $i</ShortDisplayString><SuperDisplayString>$j:$m:$s $i $t</SuperDisplayString><LongDisplayString>$c $i</LongDisplayString><SecondsPerMinute>60</SecondsPerMinute><MinutesPerHour>60</MinutesPerHour><HoursPerDay>24</HoursPerDay><InGameSecondsPerRealSecond>1</InGameSecondsPerRealSecond><SecondFixedDigits>2</SecondFixedDigits><MinuteFixedDigits>2</MinuteFixedDigits><HourFixedDigits>0</HourFixedDigits><NoZeroHour>true</NoZeroHour><NumberOfHourIntervals>2</NumberOfHourIntervals><HourIntervalNames><HourIntervalName>a.m</HourIntervalName><HourIntervalName>p.m</HourIntervalName></HourIntervalNames><HourIntervalLongNames><HourIntervalLongName>morning</HourIntervalLongName><HourIntervalLongName>afternoon</HourIntervalLongName></HourIntervalLongNames><CrudeTimeIntervals><CrudeTimeInterval text=\"day\" Lower=\"0\" Upper=\"24\"/></CrudeTimeIntervals></Clock>";
		var clock = new Db.Clock { Definition = clockXml }; db.Clocks.Add(clock); db.SaveChanges();
		var zone = new Db.Timezone { ClockId = clock.Id, Name = "Fixture UTC", Description = "Zero-offset fixture timezone." }; db.Timezones.Add(zone); db.SaveChanges(); clock.PrimaryTimezoneId = zone.Id;
		const string calendarXml = "<calendar><alias>fixture</alias><shortname>Fixture</shortname><fullname>Fixture Calendar</fullname><description>Native loader fixture</description><shortstring>$dd/$mo/$yy</shortstring><longstring>$dd/$mo/$yy</longstring><wordystring>$dd/$mo/$yy</wordystring><plane>earth</plane><epochyear>2000</epochyear><weekdayatepoch>0</weekdayatepoch><ancienterashortstring>B</ancienterashortstring><ancienteralongstring>Before</ancienteralongstring><modernerashortstring>A</modernerashortstring><moderneralongstring>After</moderneralongstring><weekdays><weekday>Day</weekday></weekdays><months><month><alias>month</alias><shortname>M</shortname><fullname>Fixture Month</fullname><nominalorder>1</nominalorder><normaldays>30</normaldays><intercalarydays/><specialdays/><nonweekdays/></month></months><intercalarymonths/></calendar>";
		foreach (var calendar in db.Calendars) { calendar.Definition = calendarXml; calendar.Date = "1/month/2000"; calendar.FeedClockId = clock.Id; }
		db.SaveChanges();
		foreach (var shard in db.Shards.ToArray())
		{
			db.ShardsClocks.Add(new() { ShardId = shard.Id, ClockId = clock.Id });
			foreach (var calendar in db.Calendars.ToArray()) db.ShardsCalendars.Add(new() { ShardId = shard.Id, CalendarId = calendar.Id });
		}
		foreach (var location in db.Zones) db.ZonesTimezones.Add(new() { ZoneId = location.Id, ClockId = clock.Id, TimezoneId = zone.Id });
		var nameXml = new XElement("NameCulture", new XElement("Patterns", Enumerable.Range(0,6).Select(style => new XElement("Pattern", new XAttribute("Style",style), new XAttribute("Text","{0}"), new XAttribute("Params","0")))), new XElement("Elements", new XElement("Element", new XAttribute("Usage",0), new XAttribute("MinimumCount",1), new XAttribute("MaximumCount",1), new XAttribute("Name","Name"), "One fixture name.")), new XElement("NameEntryRegex", "^(?<birthname>[\\w '-]+)$")).ToString();
		if (!db.NameCultures.Any(x => x.Id == 1)) db.NameCultures.Add(new() { Id = 1, Name = "Fixture mononym", Definition = nameXml });
		foreach (var culture in db.Cultures) db.CulturesNameCultures.Add(new() { CultureId = culture.Id, NameCultureId = 1, Gender = (short)Gender.Male });
		foreach (var ethnicity in db.Ethnicities) db.EthnicitiesNameCultures.Add(new() { EthnicityId = ethnicity.Id, NameCultureId = 1, Gender = (short)Gender.Male });
		foreach (var character in db.Characters)
		{
			character.NeedsModel = "NoNeeds"; character.BirthdayDate = "1/month/2000";
			if (string.IsNullOrWhiteSpace(character.NameInfo)) character.NameInfo = new XElement("Names", new XElement("PersonalName", new XElement("Name", new XAttribute("culture",1), new XElement("Element", new XAttribute("usage","BirthName"), character.Name))), new XElement("Aliases"), new XElement("CurrentName",0)).ToString();
		}
		var revision = new Db.EditableItem { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current };
		var partComponent = new Db.GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, Name = "ARMBOOT severed part", Description = "Native special part prototype.", Type = "Bodypart", Definition = "<Definition />", EditableItem = revision };
		db.GameItemComponentProtos.Add(partComponent); db.SaveChanges();
		var partItem = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "ARMBOOT severed part", Keywords = "part", ShortDescription = "a severed part", FullDescription = "A fixture severed part.", MaterialId = material, Size = 1, Weight = 1, BaseItemQuality = (int)ItemQuality.Standard, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
		partItem.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = partComponent.Id }); db.GameItemProtos.Add(partItem);
		db.SaveChanges();
		// Match the already seeded production special-prototype boundary. An empty fixture
		// otherwise asks boot to author unrelated prototypes and revisions during this test.
		var stackId = db.StackDecorators.Single(x => x.Name == "Fixture suffix stacks").Id;
		var holdableId = db.GameItemComponentProtos.Single(x => x.Type == "Holdable").Id;
		foreach (var type in new[] { "Currency Pile", "ActiveCraft", "Commodity", "Pile", "Puddle", "Stable Ticket" })
		{
			var definition = type == "Currency Pile" ? $"<Definition Decorator=\"{stackId}\" />" : type == "Pile" ? $"<Definition><Decorator>{stackId}</Decorator></Definition>" : "<Definition />";
			var component = new Db.GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, Name = "ARMBOOT " + type, Description = "Native special fixture prototype.", Type = type, Definition = definition, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			db.GameItemComponentProtos.Add(component); db.SaveChanges();
			var item = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, Name = "ARMBOOT " + type, Keywords = "fixture", ShortDescription = "a fixture item", FullDescription = "A native special fixture prototype.", MaterialId = material, Size = 1, Weight = 0, ReadOnly = true, BaseItemQuality = (int)ItemQuality.Standard, EditableItem = new() { BuilderDate = RuntimeClock.UtcNow, RevisionStatus = (int)RevisionStatus.Current } };
			item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id });
			if (type is "Currency Pile" or "Commodity" or "Pile" or "Stable Ticket") item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = holdableId });
			db.GameItemProtos.Add(item); db.SaveChanges();
		}
		foreach (var name in new[] { "Puddles", "Blood Splatters", "Dried Liquid Residue" })
		{
			var group = new Db.ItemGroup { Name = name, Keywords = "fixture" };
			group.ItemGroupForms.Add(new() { Type = "Simple", Definition = "<Definition><Description>Fixture residues.</Description><RoomDescription>Fixture residues.</RoomDescription><ItemName>residue</ItemName></Definition>" });
			db.ItemGroups.Add(group);
		}
		db.SaveChanges();
		Console.WriteLine("ARMProductionRestart-fixture=seeded actual-persisted-anatomy-limbs-time-name-progs-corpse-special-proto defaults fixture-only no-production-change");
	}

	private static int RunProductionRestart(TestDatabase database, RetirementHost host, HarnessClock clock,
		ScriptedAiCharacterInstance animated, ICharacter caster, ICharacter foe, ICharacter owner, IGameItem corpse,
		IGameItem foreign, Func<ScriptedAiCharacterInstance> cast, Action<ScriptedAiCharacterInstance> restored,
		Action<ScriptedAiCharacterInstance,ICharacter,string> order, FixtureIds fixture)
	{
		ConfigureStormHands(host.Native);
		// Separate actual live retirement proves retained controller teardown; process exit is not that proof.
		var oldController = animated.Controller;
		Require(oldController is not null && ReferenceEquals(animated.Body.Actor, animated), "Actual producer must have an active animation controller/body binding.");
		order(animated, caster, "hit opponent");
		Require(animated.Combat is not null, "Native stock hit must establish the actual combat control.");
		Require(host.Native.World.SpellOwnedCorpseAnimations!.TryRetire(animated.InstanceId, SpellRetirementReason.Dismissal, out var why), why);
		restored(animated);
		Require(animated.Controller is null && (oldController as ICharacterController)?.Actor is null && !ReferenceEquals(owner.Body.Actor, animated) && !ReferenceEquals(owner.Body.Controller, oldController) && animated.QueuedMoveCommands.Count == 0 && !animated.EffectsOfType<ISelectedCombatAction>().Any(), "Actual retirement must release the captured owned controller, body focus and selected/queued roots.");
		Console.WriteLine("ARMProductionRestart-producer-teardown=passed actual-retire captured-owned-controller-cleared body-no-longer-bound no-selected-or-queued-work");
		SeedProductionLoaderFixture(database, fixture);
		var first = cast();
		var token = host.Prototypes.Values.Single(x => x.Name == "ARM03B2B goods").CreateNew(caster); host.Native.World.Add(token); caster.Location.Insert(token,true);
		void Queue(ScriptedAiCharacterInstance actor)
		{
			order(actor,caster,"hit opponent"); order(actor,caster,"get goods");
			Require(actor.EffectsOfType<ISelectedCombatAction>().OfType<SelectedCombatAction>().Any(x => typeof(SelectedCombatAction).GetField("_commandAuthority", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(x) is not null), "Actual ordered get must leave an authority-bound selected combat action before the restart checkpoint.");
			host.Native.World.SaveManager.Flush();
		}
		Queue(first);
		ProductionRestartReader Input(ScriptedAiCharacterInstance actor,string scenario)
		{
			var life = ReadCorpseAnimationLife(database,actor.InstanceId);
			using var db = NewIndependentContext(database.ConnectionString); var row = db.GameItems.AsNoTracking().Single(x => x.Id == corpse.Id);
			Require(row.MorphTimeRemaining > 0 && XElement.Parse(row.EffectData).Descendants("OwnedLifecycleId").Single().Value == life.Origin.Id.ToString(), "Paid native parent XML and positive corpse morph time must survive.");
			return new(database.Name,fixture,scenario == "active-overdue" ? life.Origin.DeadlineUtc!.Value.AddSeconds(1) : RuntimeClock.UtcNow,life.Origin.Id,actor.InstanceId,corpse.Id,owner.Id,owner.Body.Id,foreign.Id,foe.Id,token.Id,life.Origin.DeadlineUtc!.Value,scenario,foreign.OwnershipReference,foreign.Quantity,SavedEffectHash(row.EffectData),row.MorphTimeRemaining!.Value);
		}
		var scenario = Environment.GetEnvironmentVariable("FUTUREMUD_PRODUCTION_RESTART_CASE") ?? "active-future";
		Require(scenario is "active-future" or "active-overdue" or "pending-stale" or "completed-stale", "Unknown owned restart scenario.");
        var input = Input(first, scenario);
        if (scenario.EndsWith("-stale", StringComparison.Ordinal))
        {
            using (var db = NewIndependentContext(database.ConnectionString))
                db.Database.ExecuteSqlRaw($"CREATE TRIGGER armboot_complete_fault BEFORE UPDATE ON MagicSpellLifecycles FOR EACH ROW BEGIN IF OLD.Id='{input.Origin:D}' AND NEW.State=3 THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Owned restart completion checkpoint refusal'; END IF; END");
            try { Require(!host.Native.World.SpellOwnedCorpseAnimations!.TryRetire(first.InstanceId, SpellRetirementReason.Dismissal, out _), "Native completion checkpoint must hold the committed restoration."); }
            finally { using var db = NewIndependentContext(database.ConnectionString); db.Database.ExecuteSqlRaw("DROP TRIGGER armboot_complete_fault"); }
            using (var db = NewIndependentContext(database.ConnectionString))
                Require(!db.CharacterInstances.Any(x => x.Id == first.InstanceId) && db.CellsGameItems.Count(x => x.GameItemId == corpse.Id) == 1 && SavedEffectHash(db.GameItems.Single(x => x.Id == corpse.Id).EffectData) == input.PaidEffectHash && host.Store.Find(input.Origin)!.State == SpellLifecycleState.Retiring && host.Store.Find(input.Origin)!.Diagnostic.StartsWith("<CorpseRestore"), "Actual committed restoration must retain the genuine stale paid XML and held journal.");
            if (scenario == "completed-stale")
                Require(host.Native.World.SpellOwnedCorpseAnimations!.TryRetire(first.InstanceId, SpellRetirementReason.Dismissal, out var resume), resume);
            Console.WriteLine($"ARMProductionRestart-producer={scenario} passed actual-native-restoration completion-trigger-held{(scenario == "completed-stale" ? "-then-native-resumed" : "")} original-paid-XML-no-copy no-producer-Flush");
        }
        RunItemReaderProcess(input,"--production-restart-reader");
		// The original producer is quiescent after the reader changes durable state. A fresh producer
		// scenario is selected by the owned wrapper, rather than flushing this stale live graph again.
		Console.WriteLine($"ARMProductionRestart={scenario} passed paid-stock-actual-order-queue fresh-production-loader-collapse producer-quiescent");
		return 0;
	}

	private static Futuremud LoadProductionRestartWorld(HarnessClock clock)
	{
		using var session = new FMDB();
		PositionState.SetupPositions();
		var world = new Futuremud(null!); var loader = (IFuturemudLoader)world;
		// The scheduler implementations are native; only their injected UTC clock is controlled.
		SetPrivateField(world, "_scheduler", new Scheduler(clock));
		SetPrivateField(world, "_effectScheduler", new EffectScheduler(world, clock));
		world.SaveManager.MudBootingMode = true;
		world.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Disallowed);
		loader.LoadStaticValues(); SetPrivateMember(world,"LogManager",new LogManager(world)); SetPrivateMember(world,"GameStatistics",new GameStatistics(world));
		loader.LoadPlanes(); loader.LoadAuthorityGroups(); loader.LoadChargenResources();
		SetPrivateMember(world,"VariableRegister",new VariableRegister(world)); MudSharp.FutureProg.FutureProg.Initialise();
		loader.LoadFutureProgs(); loader.LoadTraitExpressions(); loader.LoadTags(); loader.LoadDrugs(); loader.LoadKnowledges(); loader.LoadMaterials(); loader.LoadHooks(); loader.LoadCurrencies(); loader.LoadEntityDescriptionPatterns(); loader.LoadEntityDescriptions(); loader.LoadColours(); loader.LoadCharacterCombatSettings();
		loader.LoadBodypartShapes(); loader.LoadCharacteristics(); loader.LoadArmourTypes(); loader.LoadBodies();
		loader.LoadClocks(); loader.LoadCalendars(); loader.LoadCelestials(); loader.LoadHearingProfiles(); loader.LoadCellOverlayPackages(); loader.LoadCover(); loader.LoadTerrains(); loader.LoadNonCardinalExitTemplates(); loader.LoadSkyDescriptionTemplates();
		world.InitialiseTypes(); Effect.InitialiseEffects(); world.PrimeGameItems(); loader.LoadClimate(); loader.LoadWorld();
		loader.LoadImprovementModels(); loader.LoadTraitDecorators(); loader.LoadTraits(); loader.LoadNPCSkillPackages(); loader.LoadChecks(); loader.LoadHeightWeightModels(); loader.LoadHealthStrategies(); loader.LoadCorpseModels(); loader.LoadButchering(); loader.LoadBloodtypes(); loader.LoadWeaponTypes(); loader.LoadRaces();
		loader.LoadBoards(); loader.LoadWearProfiles(); loader.LoadStackDecorators(); loader.LoadShieldTypes(); loader.LoadGameItemComponentProtos(); loader.LoadGameItemGroups(); loader.LoadGameItemProtos(); loader.LoadGameItemSkins();
		loader.LoadNameCultures(); loader.LoadEthnicities(); loader.LoadCultures(); loader.LoadClans(); loader.LoadLanguageDifficultyModels(); loader.LoadLanguages(); loader.LoadSignedLanguages(); loader.LoadScripts(); loader.LoadUnits(); loader.LoadMagic(); loader.LoadMerits(); loader.LoadAIs(); loader.LoadDisfigurements(); loader.LoadRoles(); loader.LoadNPCTemplates();
		loader.LoadWritings(); loader.LoadDrawings(); loader.LoadWritingCollections(); loader.LoadProjects();
		MudSharp.Character.Character.InitialiseCharacterClass(world); SetPrivateMember(world,"LightModel",LightModel.LoadLightModel(loader));
		loader.LoadWorldItems();
		world.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Allowed);
		loader.LoadNPCs(); world.FinalisePostCharacterLoadObjects(); world.ReleasePrimedGameItems(); world.SaveManager.MudBootingMode = false;
		return world;
	}

	private static int RunProductionRestartReader(string[] arguments)
	{
		var input = JsonSerializer.Deserialize<ProductionRestartReader>(Encoding.UTF8.GetString(Convert.FromBase64String(arguments.Single())))!;
		using var database = TestDatabase.OpenExistingOwned(input.Database); ConfigureNativeDatabase(database.ConnectionString);
		var clock = new HarnessClock(); clock.Advance(input.Now - clock.GetUtcNow().UtcDateTime); using var time = RuntimeClock.Push(clock);
		long[] identities,bodies,items,instances; Dictionary<long,double> resources;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var active = input.Scenario.StartsWith("active-");
            Require(db.CharacterInstances.Any(x => x.Id == input.Instance) == active && SavedEffectHash(db.GameItems.Single(x => x.Id == input.Corpse).EffectData) == input.PaidEffectHash, "Reader must receive the actual active or native committed-restoration checkpoint with unchanged paid XML.");
            var initial = new SpellOwnedLifecycleStore().Find(input.Origin)!;
            Require(initial.State == (active ? SpellLifecycleState.Active : input.Scenario == "pending-stale" ? SpellLifecycleState.Retiring : SpellLifecycleState.Completed), "Reader must start at the declared actual durable state.");
            Require(db.GameItems.Single(x => x.Id == input.Corpse).MorphTimeRemaining == input.MorphSeconds && input.MorphSeconds > 0, "Reader must start with the exact positive paid morph duration.");
			identities = db.Characters.OrderBy(x=>x.Id).Select(x=>x.Id).ToArray(); bodies = db.Bodies.OrderBy(x=>x.Id).Select(x=>x.Id).ToArray(); items = db.GameItems.OrderBy(x=>x.Id).Select(x=>x.Id).ToArray(); instances = db.CharacterInstances.OrderBy(x=>x.Id).Select(x=>x.Id).ToArray();
			resources = db.CharactersMagicResources.Where(x=>x.CharacterId == input.Owner || x.CharacterId == input.Fixture.CharacterId).ToDictionary(x=>x.CharacterId,x=>x.Amount);
		}
		using var world = LoadProductionRestartWorld(clock);
		Require(world.Actors.Any(x=>x.Id == input.PositiveNpc && x is MudSharp.NPC.NPC), "Actual production LoadNPCs must materialise the living positive-control NPC.");
		Require(!world.Actors.Concat(world.CachedActors).Concat(world.Characters).Any(x=>x.InstanceId == input.Instance), "Production loaders must not recreate the owned DespawnOnReboot secondary or its queued authority.");
		typeof(Futuremud).GetMethod("ReconcileSpellOwnedNpcDeaths",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(world,null);
		clock.Advance(TimeSpan.FromSeconds(1)); world.EffectScheduler.CheckSchedules(); world.SaveManager.Flush();
		var store = new SpellOwnedLifecycleStore(); var life = store.Find(input.Origin)!;
		Require(life.State == SpellLifecycleState.Completed && life.Reason == (input.Scenario == "active-overdue" ? SpellRetirementReason.Expiry : input.Scenario.EndsWith("-stale", StringComparison.Ordinal) ? SpellRetirementReason.Dismissal : SpellRetirementReason.Logout), "Actual reconciliation must collapse the active animation for the correct reason: " + life.Diagnostic);
		var corpse = world.TryGetItem(input.Corpse,true); var component = corpse?.GetItemType<ICorpse>();
		Require(corpse is not null && component is not null && component.OriginalCharacter.Id == input.Owner && component.OriginalBody.Id == input.Body && component.OriginalCharacter.State.IsDead(), "Actual corpse component must retain the same dead canonical owner and body.");
		Require(!component!.OriginalCharacter.Identity.Instances.Any(x=>x.InstanceId == input.Instance) && component.OriginalBody.Actor.InstanceId != input.Instance && (component.OriginalBody.Controller as ICharacterController)?.Actor?.InstanceId != input.Instance, "No owned secondary, body binding or captured animation controller may be recreated.");
		var gear = component.OriginalBody.AllItems.Single(x=>x.Id == input.Foreign);
		Require(gear.OwnershipReference == input.ForeignTitle && gear.Quantity == input.ForeignQuantity && !gear.Deleted && !gear.Destroyed, "Foreign goods must retain exact title and quantity through native corpse-body loading.");
		Require(corpse!.Location?.Id == input.Fixture.CellId && corpse.Location.GameItems.Count(x=>x.Id == input.Corpse) == 1 && !corpse.EffectsOfType<SpellAnimatedCorpseEffect>().Any() && !corpse.EffectsOfType<MagicSpellParent>().Any(), "Native restoration must place one exact corpse and remove stale parent/child effects.");
		var queued = world.TryGetItem(input.QueuedItem,true);
		Require(queued.InInventoryOf is null && queued.ContainedIn is null && queued.Location?.Id == input.Fixture.CellId && queued.Location.GameItems.Count(x => x.Id == queued.Id) == 1, "Queued ordered Get must not replay or displace its exact floor item during boot/reconciliation.");
		var morphDeadline = ((GameItem)corpse).MorphTime;
		Require(input.MorphSeconds > 0 && morphDeadline == input.Now.AddSeconds(input.MorphSeconds) && world.Scheduler.RemainingDuration(corpse, ScheduleType.Morph) == morphDeadline - RuntimeClock.UtcNow && morphDeadline > RuntimeClock.UtcNow, $"Actual production restoration must retain and run the positive native corpse morph timer without manual Login: state={life.State}, reason={life.Reason}, morph={morphDeadline:o}, cached={corpse.CachedMorphTime}, remaining={world.Scheduler.RemainingDuration(corpse, ScheduleType.Morph)}, saved={input.MorphSeconds}, corpse={corpse.Id}, canonical={component.OriginalCharacter.Id}, body={component.OriginalBody.Id}, foreign={gear.Id}.");
		var version = life.Version;
		typeof(Futuremud).GetMethod("ReconcileSpellOwnedNpcDeaths",BindingFlags.Instance|BindingFlags.NonPublic)!.Invoke(world,null);
		clock.Advance(TimeSpan.FromSeconds(1)); world.EffectScheduler.CheckSchedules(); world.SaveManager.Flush();
		Require(store.Find(input.Origin)!.Version == version && corpse.Location.GameItems.Count(x=>x.Id == input.Corpse) == 1, "Repeated production reconciliation must not restore or complete twice.");
		Require(((GameItem)corpse).MorphTime == morphDeadline && corpse.CachedMorphTime is null,
			"Repeated reconciliation must retain the original running morph deadline.");
		// MorphSaving dirties the item every thirty seconds; exercise its native schedule
		// before expecting Flush to persist elapsed time on an otherwise unchanged corpse.
		clock.Advance(TimeSpan.FromSeconds(30)); world.Scheduler.CheckSchedules();
		world.EffectScheduler.CheckSchedules(); world.SaveManager.Flush();
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			Require(identities.SequenceEqual(db.Characters.OrderBy(x=>x.Id).Select(x=>x.Id)) && bodies.SequenceEqual(db.Bodies.OrderBy(x=>x.Id).Select(x=>x.Id)) && items.SequenceEqual(db.GameItems.OrderBy(x=>x.Id).Select(x=>x.Id)) && instances.Where(x=>x != input.Instance).SequenceEqual(db.CharacterInstances.OrderBy(x=>x.Id).Select(x=>x.Id)), "Boot may remove only the claimed secondary; canonical/body/item identities must survive.");
			Require(db.CellsGameItems.Count(x=>x.GameItemId == input.Corpse && x.CellId == input.Fixture.CellId) == 1 && !XElement.Parse(db.GameItems.Single(x=>x.Id == input.Corpse).EffectData).Descendants("OwnedLifecycleId").Any(), "Normal native save must retain exact corpse placement and clear saved animation XML.");
			Require(db.GameItems.Single(x => x.Id == input.Corpse).MorphTimeRemaining == (int)(morphDeadline - RuntimeClock.UtcNow).TotalSeconds && db.GameItems.Single(x => x.Id == input.Corpse).MorphTimeRemaining > 0, "Native save must retain the remaining positive morph timer.");
			Require(db.CellsGameItems.Count(x => x.GameItemId == input.QueuedItem && x.CellId == input.Fixture.CellId) == 1 && !db.BodiesGameItems.Any(x => x.GameItemId == input.QueuedItem) && db.GameItems.Single(x => x.Id == input.QueuedItem).ContainerId is null && db.BodiesGameItems.Count(x => x.GameItemId == input.Foreign && x.BodyId == input.Body) == 1, "Cold durable queue and foreign gear custody must retain exact cell/body joins without container displacement.");
			Require(resources.OrderBy(x=>x.Key).SequenceEqual(db.CharactersMagicResources.Where(x=>x.CharacterId == input.Owner || x.CharacterId == input.Fixture.CharacterId).ToDictionary(x=>x.CharacterId,x=>x.Amount).OrderBy(x=>x.Key)), "Boot/reconcile must not replay payment or magic resource work.");
		}
		Console.WriteLine($"ARMProductionRestart-reader={input.Scenario} passed actual-Futuremud-production-loaders real-NPC-positive-control same-corpse-body-foreign-goods exact-secondary-collapse no-owned-controller-recreation no-queued-get/payment-replay idempotent native-Flush");
		return 0;
	}
}

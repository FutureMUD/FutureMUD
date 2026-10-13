#nullable enable
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MudSharp.Magic;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedStockAttunementAcceptance(TestDatabase database)
	{
		Func<MudSharp.Database.FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var players = TraditionPlayers(database);
		var before = PreparedDatabaseChecksum(database);
		var stopped = ArmageddonStockSpellInstaller.Install(factory, (_, stage) =>
		{
			if (stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new IOException("S4 contribution interruption before commit");
		});
		Require(stopped.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"S4 native precommit rollback changed tables or returned the wrong outcome.");
		var lostAck = ArmageddonStockSpellInstaller.Install(factory, (key, stage) =>
		{
			if (key == ArmageddonCreatureSpellContent.Contributions.Last().Key && stage == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("S4 contribution lost acknowledgement");
		});
		Require(lostAck.Status == ArmageddonInstallStatus.CommittedConfirmationFailed, string.Join("\n", lostAck.Messages));
		before = PreparedDatabaseChecksum(database);
		var rerun = ArmageddonStockSpellInstaller.Install(factory);
		Require(rerun.Status == ArmageddonInstallStatus.Completed && before == PreparedDatabaseChecksum(database) && players == TraditionPlayers(database),
			"S4 reconciliation changed installed/player tables: " + string.Join("\n", rerun.Messages));
		using var db = factory();
		// Use collection instance Contains; span-based array overloads cannot be evaluated by EF9's parameter interpreter.
		var capabilityKeys = new ArmageddonStockProgression(ArmageddonInstallation.Read(db)!.Mode).Capabilities.Select(x => x.Key).ToList();
		var packageCaps = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			capabilityKeys.Contains(x.StableKey) && x.EntityType == nameof(Db.MagicCapability) && !x.Retired).Select(x => x.LogicalId).ToList();
		Require(packageCaps.Count == 10 && packageCaps.All(x => x is > 0) && packageCaps.Distinct().Count() == 10,
			"S4 requires the ten distinct typed package capability identities.");
		Require(db.MagicCapabilities.AsNoTracking().Where(x => packageCaps.Contains(x.Id)).AsEnumerable().All(x =>
			XElement.Parse(x.Definition).Element("Casting") is { } casting && (bool?)casting.Attribute("enabled") == false && !casting.Elements("Admission").Any()),
			"S4 installed a partial active package roster.");
		Console.WriteLine("ARMS4-attunement-install=passed actual-MySQL precommit-all-table-rollback lost-ack-resume stable-children no-op-all-table-checksum no-player-writes all-ten-package-caps-disabled");
		if (Environment.GetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE") != "1") return;
		// This separate, explicitly disposable builder capability qualifies the installed spell.
		// It is never package-owned and never activates a partial Armageddon roster.
		var settings = ArmageddonInstallation.Read(db)!;
		var ids = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.LogicalId != null).ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var spell = db.MagicSpells.AsNoTracking().Single(x => x.Id == ids[ArmageddonStockSpellContent.ReadAttunementKey]);
		var reserve = ids["arm.resource.water"];
		var actor = db.Characters.AsNoTracking().Single(x => x.IsAdminAvatar && x.AccountId == settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount]);
		var cap = new Db.MagicCapability
		{
			Name = "S4 Attunement Qualification", CapabilityModel = "skilllevel", MagicSchoolId = spell.MagicSchoolId, PowerLevel = 1,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", spell.CastingTraitDefinitionId),
				new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
				new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()), new XAttribute("enabled", true),
					new XAttribute("trait", spell.CastingTraitDefinitionId), new XAttribute("source", ids[ArmageddonProgressionInstaller.SourceResourceKey]),
					new XAttribute("reserve", reserve), new XAttribute("passive", false), new XAttribute("startingVersion", 1),
					new XElement("Admission", new XAttribute("key", Guid.NewGuid()), new XAttribute("spell", spell.Id), new XAttribute("starting", true),
						new XAttribute("min", 1), new XAttribute("max", 7), new XAttribute("opening", 100), new XAttribute("rawCap", 100))),
				new XElement("Gathering", new XAttribute("version", 2), new XElement("Method", new XAttribute("key", Guid.NewGuid()), new XAttribute("alias", "draw"),
					new XAttribute("name", "S4 qualification paid Self gathering"), new XAttribute("kind", "Self"), new XAttribute("destination", reserve),
					new XAttribute("min", 1), new XAttribute("max", 100), new XAttribute("duration", 2), new XAttribute("ratio", 1), new XAttribute("stamina", 1),
					new XAttribute("minimumStamina", 0), new XAttribute("maximumHealthSeverity", "None")))).ToString()
		};
		db.Add(cap); db.SaveChanges();
		var merit = new Db.Merit
		{
			Name = "S4 Attunement Qualification Caster", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
				new XElement("ApplicabilityProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]), new XElement("ChargenBlurb", "Disposable S4 acceptance only"),
				new XElement("DescriptionText", "@ have|has an acceptance capability."), new XElement("Capabilities", new XElement("Capability", cap.Id))).ToString()
		};
		db.Add(merit); db.SaveChanges();
		var claims = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired).ToArray();
		var installed = (from claim in claims
			join native in db.MagicSpells.AsNoTracking() on claim.LogicalId equals (long?)native.Id
			where claim.Module == ArmageddonStockSpellInstaller.Module && claim.EntityType == nameof(Db.MagicSpell)
			orderby claim.StableKey
			select (Key: claim.StableKey, Spell: native)).ToArray();
		Require(installed.Length > 0 && installed.Select(x => x.Key).Distinct(StringComparer.Ordinal).Count() == installed.Length,
			"S4 qualification requires every unique installed canonical spell claim.");
		var stockInputs = installed.Select(x => PreparedStockDefinitionInput(db, x.Key, x.Spell)).ToArray();
		var candidateSpecs = new (string Key, string Target, double Seconds)[]
		{
			("arm.spell.night_eyes", "self", 60), ("arm.spell.second_breath", "self", 0),
			("arm.spell.daylight", "here", 60), ("arm.spell.quickened_healing", "self", 120),
			("arm.spell.earthen_strength", "self", 60)
		};
		var candidates = new List<object>
		{
			new { key = ArmageddonStockSpellContent.ReadAttunementKey, spell = spell.Id, target = "self" }
		};
		foreach (var spec in candidateSpecs)
		{
			var native = installed.SingleOrDefault(x => x.Key == spec.Key).Spell;
			if (native is null) continue; // Availability is declared by installed claims, never display names.
			var duration = db.TraitExpressions.AsNoTracking().Single(x => x.Id == native.EffectDurationExpressionId).Expression;
			Require(duration == (spec.Seconds == 0 ? "0" : spec.Seconds.ToString(CultureInfo.InvariantCulture) + "*grade"),
				"Campaign duration differs from the exact approved grade formula: " + spec.Key);
			var schoolClaim = claims.Single(x => x.EntityType == nameof(Db.MagicSchool) && x.LogicalId == native.MagicSchoolId &&
				x.StableKey.StartsWith("arm.school.", StringComparison.Ordinal));
			var resourceKey = schoolClaim.StableKey.Replace("arm.school.", "arm.resource.", StringComparison.Ordinal);
			var resourceClaim = claims.Single(x => x.StableKey == resourceKey && x.EntityType == nameof(Db.MagicResource) && x.LogicalId > 0);
			var nativeSchool = db.MagicSchools.AsNoTracking().Single(x => x.Id == native.MagicSchoolId);
			Require(!string.IsNullOrWhiteSpace(nativeSchool.SchoolVerb), "Campaign school has no native invoked command.");
			var root = XElement.Parse(native.Definition);
			var efficiency = root.Element("ControlledPower")!.Element("Efficiency")!;
			var curve = new ControlledSpellEfficiency((double)efficiency.Attribute("minimum")!, (double)efficiency.Attribute("scale")!);
			var cost = curve.Cost(1, 1);
			Require(double.IsFinite(cost) && cost is > 0 and <= 100, "Campaign cost exceeds its bounded native Self method.");
			var source = ids[ArmageddonProgressionInstaller.SourceResourceKey];
			Require((long)root.Element("Costs")!.Element("Cost")!.Attribute("resource")! == source,
				"Campaign source cost does not bind the exact synthetic source resource.");
			// The restore sample pays a real finite stamina cost while gathering, leaving
			// room for a positive restore despite ordinary heartbeat regeneration.
			var gatherStamina = spec.Key == "arm.spell.second_breath" ? 20.0 : 1.0;
			var fixture = PreparedStockCampaignRoute(db, settings, spec.Key, native, resourceClaim.LogicalId!.Value, source, gatherStamina);
			candidates.Add(new
			{
				key = spec.Key, spell = native.Id, target = spec.Target, capability = fixture.Capability, merit = fixture.Merit,
				capability_name = fixture.Name,
				trait = native.CastingTraitDefinitionId!.Value, school = native.MagicSchoolId,
				resource = resourceClaim.LogicalId.Value, source, invoked = nativeSchool.SchoolVerb.ToLowerInvariant(),
				grade_one_cost = cost, duration_seconds = spec.Seconds, gather_alias = "draw", gather_stamina_cost = gatherStamina
			});
		}
		var sqlSchema = ((IEnumerable<object>)InstalledSqlSchema(db)).ToList();
		sqlSchema.Add(PreparedStockSqlTable<Db.Room>(db, "Rooms", [nameof(Db.Room.Id), nameof(Db.Room.EffectData)]));
		sqlSchema.Add(PreparedStockSqlTable<Db.Body>(db, "Bodies", [nameof(Db.Body.Id), nameof(Db.Body.CurrentStamina)]));
		sqlSchema.Add(PreparedStockSqlTable<Db.MagicCapability>(db, "MagicCapabilities", [nameof(Db.MagicCapability.Id), nameof(Db.MagicCapability.Name)]));
		sqlSchema.Add(PreparedStockSqlTable<Db.CharacterCastingEnrolment>(db, "CharacterCastingEnrolments", [nameof(Db.CharacterCastingEnrolment.CharacterId),
			nameof(Db.CharacterCastingEnrolment.MagicCapabilityId), nameof(Db.CharacterCastingEnrolment.CompletedStartingGrantVersion)]));
		sqlSchema.Add(PreparedStockSqlTable<Db.MagicSpell>(db, "MagicSpells", [nameof(Db.MagicSpell.Id), nameof(Db.MagicSpell.Definition),
			nameof(Db.MagicSpell.MagicSchoolId), nameof(Db.MagicSpell.CastingTraitDefinitionId), nameof(Db.MagicSpell.EffectDurationExpressionId),
			nameof(Db.MagicSpell.SpellKnownProgId), nameof(Db.MagicSpell.ResistingTraitDefinitionId)]));
		sqlSchema.Add(PreparedStockSqlTable<Db.TraitExpression>(db, "TraitExpressions", [nameof(Db.TraitExpression.Id), nameof(Db.TraitExpression.Expression)]));
		var allReferences = installed.SelectMany(x => PreparedStockReferences(x.Spell)).Distinct().ToArray();
		var referenceSchema = allReferences.Select(x => x.Kind).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal)
			.Select(kind => PreparedStockReferenceSchema(db, kind)).ToArray();
		Require(players == TraditionPlayers(database), "S4 qualification preparation granted, refilled or changed a player.");
		Require(db.MagicCapabilities.AsNoTracking().Where(x => packageCaps.Contains(x.Id)).AsEnumerable().All(x =>
			XElement.Parse(x.Definition).Element("Casting") is { } casting && (bool?)casting.Attribute("enabled") == false && !casting.Elements("Admission").Any()),
			"Campaign preparation changed a package casting gate.");
		var input = Path.Combine(Directory.GetParent(CatalogueRepository)!.FullName, "prepared-stock-attunement-input_" + Guid.NewGuid().ToString("N") + ".json");
		var preparationSource = Path.Combine(CatalogueRepository, "tests", "ArmageddonPreparedSeederNativeHarness", "Program.StockAttunement.cs");
		File.WriteAllText(input, JsonSerializer.Serialize(new
		{
			sql_schema = sqlSchema, typed_reference_schema = referenceSchema,
			mode = settings.Mode.ToString(), classes_enabled = settings.ClassesEnabled,
			character = actor.Id, body = actor.BodyId, spell = spell.Id, resource = reserve, capability = cap.Id, merit = merit.Id,
			capability_name = cap.Name,
			trait = spell.CastingTraitDefinitionId, school = spell.MagicSchoolId, source = ids[ArmageddonProgressionInstaller.SourceResourceKey],
			definition = spell.Definition, duration = spell.EffectDurationExpressionId,
			stock_spells = stockInputs,
			representative_campaign = new { version = 1, seed = 20261013, sample_limit = 4, candidates,
				earth_mount = new { status = "PENDING", reason = "No explicit disposable paid Earth Mount admission and real-exit native follow observer." } },
			prepared_source = new { path = preparationSource, sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(preparationSource))) },
			package_capabilities = db.SeederManagedRecords.AsNoTracking().Where(x => x.EntityType == nameof(Db.MagicCapability) && x.Module == ArmageddonProgressionInstaller.Module).Select(x => x.LogicalId).ToArray(),
			limits = "Explicit separate builder qualification capability; native enrolment starts grade one, no stock roster activation, no S5 natural progression/affordability certificate."
		}, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }));
		Environment.SetEnvironmentVariable("FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT", input);
		Console.WriteLine("ARMS4-attunement-qualification-input=" + input + " separate-builder-fixture no-player-mutation");
	}

	private static (long Capability, long Merit, string Name) PreparedStockCampaignRoute(MudSharp.Database.FuturemudDatabaseContext db,
		ArmageddonInstallationSettings settings, string key, Db.MagicSpell spell, long reserve, long source, double gatherStamina)
	{
		var name = "S4 Campaign Qualification " + key;
		var cap = new Db.MagicCapability
		{
			Name = name, CapabilityModel = "skilllevel", MagicSchoolId = spell.MagicSchoolId, PowerLevel = 1,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", spell.CastingTraitDefinitionId),
				new XElement("ConcentrationCapabilityExpression", "1"), new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
				new XElement("Casting", new XAttribute("version", 1), new XAttribute("identity", Guid.NewGuid()), new XAttribute("enabled", true),
					new XAttribute("trait", spell.CastingTraitDefinitionId), new XAttribute("source", source), new XAttribute("reserve", reserve),
					new XAttribute("passive", false), new XAttribute("startingVersion", 1), new XElement("Admission", new XAttribute("key", Guid.NewGuid()),
						new XAttribute("spell", spell.Id), new XAttribute("starting", true), new XAttribute("min", 1), new XAttribute("max", 7),
						new XAttribute("opening", 100), new XAttribute("rawCap", 100))),
				new XElement("Gathering", new XAttribute("version", 2), new XElement("Method", new XAttribute("key", Guid.NewGuid()),
					new XAttribute("alias", "draw"), new XAttribute("name", "S4 campaign finite paid Self gathering"), new XAttribute("kind", "Self"),
					new XAttribute("destination", reserve), new XAttribute("min", 1), new XAttribute("max", 100), new XAttribute("duration", 2),
					new XAttribute("ratio", 1), new XAttribute("stamina", gatherStamina), new XAttribute("minimumStamina", 0), new XAttribute("maximumHealthSeverity", "None")))).ToString()
		};
		db.Add(cap); db.SaveChanges();
		var merit = new Db.Merit
		{
			Name = name + " Caster", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
				new XElement("ApplicabilityProg", settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]),
				new XElement("ChargenBlurb", "Disposable campaign acceptance only"), new XElement("DescriptionText", "@ have|has an acceptance capability."),
				new XElement("Capabilities", new XElement("Capability", cap.Id))).ToString()
		};
		db.Add(merit); db.SaveChanges();
		return (cap.Id, merit.Id, cap.Name);
	}

	private sealed record PreparedStockReference(string Kind, long Id);

	private static IReadOnlyList<PreparedStockReference> PreparedStockReferences(Db.MagicSpell spell)
	{
		var refs = new HashSet<PreparedStockReference>();
		void Add(string kind, long? id) { if (id is > 0) refs.Add(new(kind, id.Value)); }
		Add(nameof(Db.MagicSchool), spell.MagicSchoolId); Add(nameof(Db.TraitDefinition), spell.CastingTraitDefinitionId);
		Add(nameof(Db.TraitDefinition), spell.ResistingTraitDefinitionId); Add(nameof(Db.FutureProg), spell.SpellKnownProgId);
		Add(nameof(Db.TraitExpression), spell.EffectDurationExpressionId);
		var labels = new Dictionary<string, string>(StringComparer.Ordinal)
		{
			["TargetFilterProg"] = nameof(Db.FutureProg), ["FilterProg"] = nameof(Db.FutureProg), ["OnLoadProg"] = nameof(Db.FutureProg),
			["EligibilityProg"] = nameof(Db.FutureProg), ["LifetimeMultiplierProg"] = nameof(Db.FutureProg),
			["NPCPrototypeId"] = nameof(Db.NpcTemplate), ["ItemPrototypeId"] = nameof(Db.GameItemProto), ["ItemSkinId"] = nameof(Db.GameItemSkin),
			["LiquidId"] = nameof(Db.Liquid), ["Liquid"] = nameof(Db.Liquid), ["Resource"] = nameof(Db.MagicResource),
			["AutobuilderTemplate"] = nameof(Db.AutobuilderRoomTemplate), ["TemplateRoom"] = nameof(Db.Room), ["FallbackRoom"] = nameof(Db.Room),
			["WaterPrototype"] = nameof(Db.GameItemProto), ["Prototype"] = nameof(Db.GameItemProto), ["Terrain"] = nameof(Db.Terrain),
			["School"] = nameof(Db.MagicSchool), ["AI"] = nameof(Db.ArtificialIntelligence), ["BonusPlane"] = nameof(Db.Plane)
		};
		foreach (var node in XElement.Parse(spell.Definition).Descendants())
		{
			// Only authored native reference fields are read. Numeric literals, grades,
			// quantities, power and arbitrary XML values never become identity claims.
			if (labels.TryGetValue(node.Name.LocalName, out var kind) && long.TryParse(node.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id)) Add(kind, id);
			if (node.Name == "Template" && node.Attribute("grade") is not null) Add(nameof(Db.NpcTemplate), (long?)node);
			if (node.Name == "Cost") { Add(nameof(Db.MagicResource), (long?)node.Attribute("resource")); Add(nameof(Db.TraitExpression), (long?)node.Attribute("expression")); }
			Add(nameof(Db.TraitDefinition), (long?)node.Attribute("trait"));
			foreach (var field in new[] { "tag", "secondtag" }) Add(nameof(Db.Tag), (long?)node.Attribute(field));
			Add(nameof(Db.FutureProg), (long?)node.Attribute("predicate")); Add(nameof(Db.FutureProg), (long?)node.Attribute("eligibility"));
			Add(nameof(Db.Liquid), (long?)node.Attribute("liquid"));
			if (node.Name == "Liquid") Add(nameof(Db.Liquid), (long?)node.Attribute("id"));
			if (node.Name == "SourceScope") { Add(nameof(Db.Terrain), (long?)node.Attribute("silt")); Add(nameof(Db.Terrain), (long?)node.Attribute("shadow")); }
		}
		return refs.OrderBy(x => x.Kind, StringComparer.Ordinal).ThenBy(x => x.Id).ToArray();
	}

	private static object PreparedStockDefinitionInput(MudSharp.Database.FuturemudDatabaseContext db, string key, Db.MagicSpell spell)
	{
		var root = XElement.Parse(spell.Definition);
		Require(root.Element("StockIdentity")?.Value == key && spell.CastingTraitDefinitionId is > 0 && spell.EffectDurationExpressionId is > 0,
			"Canonical qualification input is missing exact native metadata: " + key);
		var cost = root.Element("Costs")!.Elements("Cost").Single();
		var expression = (long)cost.Attribute("expression")!;
		return new
		{
			key, id = spell.Id, name = spell.Name, definition = spell.Definition, school = spell.MagicSchoolId,
			trait = spell.CastingTraitDefinitionId!.Value, known_prog = spell.SpellKnownProgId,
			resisting_trait = spell.ResistingTraitDefinitionId ?? 0,
			duration_expression = spell.EffectDurationExpressionId!.Value,
			duration_formula = db.TraitExpressions.AsNoTracking().Single(x => x.Id == spell.EffectDurationExpressionId).Expression,
			cost_expression = expression, cost_formula = db.TraitExpressions.AsNoTracking().Single(x => x.Id == expression).Expression,
			cost_resource = (long)cost.Attribute("resource")!, typed_references = PreparedStockReferences(spell)
		};
	}

	private static readonly IReadOnlyDictionary<string, Type> PreparedStockReferenceTypes = new Dictionary<string, Type>(StringComparer.Ordinal)
	{
		[nameof(Db.MagicSchool)] = typeof(Db.MagicSchool), [nameof(Db.TraitDefinition)] = typeof(Db.TraitDefinition),
		[nameof(Db.TraitExpression)] = typeof(Db.TraitExpression), [nameof(Db.MagicResource)] = typeof(Db.MagicResource),
		[nameof(Db.FutureProg)] = typeof(Db.FutureProg), [nameof(Db.NpcTemplate)] = typeof(Db.NpcTemplate),
		[nameof(Db.GameItemProto)] = typeof(Db.GameItemProto), [nameof(Db.GameItemSkin)] = typeof(Db.GameItemSkin),
		[nameof(Db.Liquid)] = typeof(Db.Liquid), [nameof(Db.Tag)] = typeof(Db.Tag), [nameof(Db.Terrain)] = typeof(Db.Terrain),
		[nameof(Db.Room)] = typeof(Db.Room), [nameof(Db.AutobuilderRoomTemplate)] = typeof(Db.AutobuilderRoomTemplate),
		[nameof(Db.ArtificialIntelligence)] = typeof(Db.ArtificialIntelligence), [nameof(Db.Plane)] = typeof(Db.Plane)
	};

	private static object PreparedStockReferenceSchema(MudSharp.Database.FuturemudDatabaseContext db, string kind)
	{
		var entity = db.Model.FindEntityType(PreparedStockReferenceTypes[kind]) ?? throw new InvalidOperationException("Missing typed stock EF entity: " + kind);
		var table = entity.GetTableName() ?? throw new InvalidOperationException("Missing typed stock EF table: " + kind);
		var store = StoreObjectIdentifier.Table(table, entity.GetSchema());
		return new { kind, table, id_column = entity.FindProperty("Id")!.GetColumnName(store) };
	}

	private static object PreparedStockSqlTable<T>(MudSharp.Database.FuturemudDatabaseContext db, string alias, string[] columns)
	{
		var entity = db.Model.FindEntityType(typeof(T)) ?? throw new InvalidOperationException("Missing campaign EF entity.");
		var table = entity.GetTableName() ?? throw new InvalidOperationException("Missing campaign EF table.");
		var store = StoreObjectIdentifier.Table(table, entity.GetSchema());
		return new { alias, table, columns = columns.Select(name =>
		{
			var property = entity.FindProperty(name) ?? throw new InvalidOperationException("Missing campaign EF property: " + name);
			return new { alias = name, column = property.GetColumnName(store), store_type = property.GetRelationalTypeMapping().StoreType,
				clr_type = property.ClrType.FullName, nullable = property.IsNullable };
		}).ToArray() };
	}
}

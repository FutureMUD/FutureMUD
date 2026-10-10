#nullable enable

using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MySql.Data.MySqlClient;
using EditableItem = MudSharp.Models.EditableItem;
using MagicSchool = MudSharp.Models.MagicSchool;

// Invoked only by the repository's disposable-world orchestrator, while its MUD is stopped.
// Test selections are authored here and exported, never added as installer defaults.
if (args.Length != 3) throw new ArgumentException("Use <owned data directory> <receipt> provision|rerun|restore|failure-cases|armour-repair.");
var connection = Environment.GetEnvironmentVariable("FURY_CALM_SMOKE_CONNECTION")
	?? throw new InvalidOperationException("Missing owned connection.");
var cs = new MySqlConnectionStringBuilder(connection);
var expectedData = Path.GetFullPath(args[0]).TrimEnd(Path.DirectorySeparatorChar);
var receiptPath = Path.GetFullPath(args[1]);
var runRoot = Directory.GetParent(Directory.GetParent(expectedData)!.FullName)!.FullName;
var repository = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../"));
var allowedRoot = Path.Combine(repository, ".artifacts", "authored-celestials") + Path.DirectorySeparatorChar;
if (!runRoot.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase) ||
	!receiptPath.StartsWith(runRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) ||
	cs.Server != "127.0.0.1" || cs.Database != "authored_celestial_smoke" || cs.UserID != "root" || cs.Password.Length != 0)
	throw new InvalidOperationException("Only the orchestrator's owned loopback world and receipt directory are permitted.");
using var sql = new MySqlConnection(connection);
sql.Open();
void Identity()
{
	using var command = new MySqlCommand("SELECT @@datadir", sql);
	if (!Path.GetFullPath((string)command.ExecuteScalar()!).TrimEnd(Path.DirectorySeparatorChar)
		.Equals(expectedData, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Owned data directory mismatch.");
}
Identity();
var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>().UseMySql(connection, ServerVersion.AutoDetect(connection)).Options;
FuturemudDatabaseContext? lastContext = null;
Func<FuturemudDatabaseContext> factory = () => lastContext = new(options);
var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
if (args[2] == "shelter-fixtures")
{
	OwnedShelterFixtures.Run(options, receiptPath);
	return;
}
void Require(bool test, string reason) { if (!test) throw new InvalidOperationException(reason); }
Dictionary<string, string> TableChecksums()
{
	Identity();
	using var command = new MySqlCommand("SELECT TABLE_NAME FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() ORDER BY TABLE_NAME", sql);
	var tables = new List<string>();
	using (var reader = command.ExecuteReader()) while (reader.Read()) tables.Add(reader.GetString(0));
	var result = new Dictionary<string, string>();
	foreach (var table in tables)
	{
		using var check = new MySqlCommand($"CHECKSUM TABLE `{table.Replace("`", "``")}` EXTENDED", sql);
		using var reader = check.ExecuteReader(); reader.Read();
		result.Add(table, reader.GetValue(1).ToString()!);
	}
	return result;
}
string Digest() => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("\n",
	TableChecksums().Select(x => x.Key + ":" + x.Value)))));
void InstallerRows(string suffix)
{
	using var read = factory();
	var records = read.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package).OrderBy(x => x.StableKey).ToArray();
	var spells = records.Where(x => x.EntityType == "MagicSpell").Select(x => x.LogicalId!.Value).ToList();
	var expressions = records.Where(x => x.EntityType == "TraitExpression").Select(x => x.LogicalId!.Value).ToList();
	File.WriteAllText(receiptPath + suffix, JsonSerializer.Serialize(new { Records = records.Select(x => new { x.StableKey, x.LogicalId, x.SeedBaseline, x.AppliedFingerprint, x.AppliedAt }),
		Spells = read.MagicSpells.AsNoTracking().Where(x => spells.Contains(x.Id)).Select(x => new { x.Id, x.Name, x.Definition }).ToArray(),
		Expressions = read.TraitExpressions.AsNoTracking().Where(x => expressions.Contains(x.Id)).Select(x => new { x.Id, x.Expression }).ToArray(),
		Capabilities = read.MagicCapabilities.AsNoTracking().Where(x => x.Name.StartsWith("Armageddon partial ")).Select(x => new { x.Id, x.Definition }).ToArray() }, jsonOptions));
}
T Copy<T>(T source) where T : class, new()
{
	var target = new T();
	foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
		.Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string))))
		property.SetValue(target, property.GetValue(source));
	return target;
}
var bindingsPath = Path.Combine(runRoot, "fury-calm-bindings.json");
if (args[2] == "restore")
{
	// Production backup service embeds the source database name. Restore only to that same
	// disposable database on the verified instance, with the MUD stopped by the caller.
	var before = Digest();
	var service = new MySqlDatabaseBackupService();
	var backup = service.CreateBackup(connection, Path.Combine(runRoot, "fury-calm-backup"));
	Identity();
	service.RecreateEmptyDatabase(connection);
	service.RestoreBackup(connection, backup);
	sql.ChangeDatabase(cs.Database);
	var after = Digest();
	Require(before == after, "Full database checksums changed across production backup/restore.");
	File.WriteAllText(receiptPath, JsonSerializer.Serialize(new { Status = "PASS", Backup = backup,
		DigestBefore = before, DigestAfter = after, Target = cs.Database, OwnedData = expectedData }, jsonOptions));
	return;
}
if (args[2] == "armour-repair")
{
	OwnedBoneArmourRepair.Run(sql, runRoot, receiptPath, TableChecksums);
	return;
}
ArmageddonPreparedWorldBindings bindings;
using (var db = factory())
{
	if (args[2] == "provision")
	{
		Require(!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package), "Fixture requires a world without this package.");
		var admin = db.Accounts.Single(x => x.Name == "Admin").Id;
		var attribute = db.TraitDefinitions.Single(x => x.Type == 1 && x.OwnerScope == 0 && x.Name == "Constitution");
		var skillSeed = db.TraitDefinitions.First(x => x.Type == 0 && x.OwnerScope == 1 && x.ImproverId != null && x.ExpressionId != null);
		var numeric = new TraitDecorator { Name = "QA authored numeric skill display", Type = "SimpleNumeric", Contents = "" };
		db.TraitDecorators.Add(numeric); db.SaveChanges();
		var decorator = numeric.Id;
		var no = db.FutureProgs.First(x => x.FunctionText == "return false" && !x.FutureProgsParameters.Any());
		var yes = db.FutureProgs.First(x => x.FunctionText == "return true" && !x.FutureProgsParameters.Any());
		var eligibility = Copy(yes); eligibility.Id = 0; eligibility.FunctionName = "qaFuryCalmEligible";
		foreach (var name in new[] { "target", "caster" }) eligibility.FutureProgsParameters.Add(new()
		{ ParameterIndex = eligibility.FutureProgsParameters.Count, ParameterName = name, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(eligibility);
		var school = new MagicSchool { Name = "QA Emotional Magic", SchoolVerb = "qamagi", SchoolAdjective = "emotional", PowerListColour = "bold magenta" };
		db.MagicSchools.Add(school);
		var expression = new TraitExpression { Name = "QA authored reserve capacity", Expression = "variable*100" };
		db.TraitExpressions.Add(expression); db.SaveChanges();
		MagicResource Resource(string name, string shortName) => new() { Name = name, ShortName = shortName, Type = "simple",
			MagicResourceType = (int)MagicResourceType.PlayerResource, BottomColour = "red", MidColour = "yellow", TopColour = "green",
			Definition = $"<Definition><AttributeCapacity version='1' attribute='{attribute.Id}' expression='{expression.Id}' basis='effective'/></Definition>" };
		var source = Resource("QA source energy", "QAS"); var reserve = Resource("QA reserve energy", "QAR");
		db.MagicResources.AddRange(source, reserve);
		TraitDefinition Skill(string name)
		{
			var row = Copy(skillSeed); row.Id = 0; row.Name = name; row.Alias = name; row.TraitGroup = "QA Fixture";
			db.TraitDefinitions.Add(row); db.SaveChanges(); return row;
		}
		var gather = Skill("QA Gathering");
		var skills = ArmageddonMagicInstaller.Content(new(false, 0, 0, new Dictionary<string, long>(), 0, 0, 0, 0, 0, 0, 0, 0, 0))
			.ToDictionary(x => x.Key, x => Skill("QA " + x.Key).Id);
		db.SaveChanges();
		var template = new MagicCapability { Name = "QA authored gathering template", CapabilityModel = "skilllevel", MagicSchoolId = school.Id,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", gather.Id), new XElement("ConcentrationCapabilityExpression", "3"),
				new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
				new XElement("Gathering", new XAttribute("version", 2), new XElement("Method", new XAttribute("key", Guid.NewGuid()),
					new XAttribute("kind", "Self"), new XAttribute("alias", "self"), new XAttribute("name", "QA authored self gathering"),
					new XAttribute("destination", reserve.Id), new XAttribute("source", 0), new XAttribute("stamina", 1),
					new XAttribute("duration", 1), new XAttribute("min", 1), new XAttribute("max", 5)))).ToString() };
		db.MagicCapabilities.Add(template);
		var itemSeed = db.GameItemProtos.Include(x => x.EditableItem).First(x => x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		var componentSeed = db.GameItemComponentProtos.Include(x => x.EditableItem).First(x => x.Type == "Holdable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		EditableItem Editable() { var row = Copy(itemSeed.EditableItem); row.Id = 0; row.RevisionNumber = 0; row.BuilderAccountId = admin; return row; }
		long nextComponent = db.GameItemComponentProtos.Max(x => x.Id) + 1;
		GameItemComponentProto Component(string type, string definition)
		{
			var row = Copy(componentSeed); row.Id = nextComponent++; row.RevisionNumber = 0; row.Name = "QA " + type;
			row.Type = type; row.Definition = definition; row.EditableItemId = 0; row.EditableItem = Editable();
			db.GameItemComponentProtos.Add(row); return row;
		}
		var wear = db.WearProfiles.First(x => x.Type == "Direct" || x.Type == "Shape").Id;
		var wearable = Component("Wearable", $"<Definition><Profiles Default='{wear}'><Profile>{wear}</Profile></Profiles><WearableProg>0</WearableProg><WhyCannotWearProg>0</WhyCannotWearProg></Definition>");
		var lightComponent = Component("Prog Light", "<Definition><IlluminationProvided>40</IlluminationProvided></Definition>");
		var bites = new StackDecorator { Name = "QA authored bites", Type = "Bites", Definition = "<Definition><Range Min='0' Max='99' Item='partly eaten {0}'/></Definition>" };
		db.StackDecorators.Add(bites); db.SaveChanges();
		var foodComponent = Component("Food", $"<Definition Satiation='2' Water='0' Thirst='0' Alcohol='0' Bites='4' Decorator='{bites.Id}'><OnEatProg>0</OnEatProg></Definition>");
		long nextItem = db.GameItemProtos.Max(x => x.Id) + 1;
		GameItemProto Item(string name, params GameItemComponentProto[] components)
		{
			var row = Copy(itemSeed); row.Id = nextItem++; row.RevisionNumber = 0; row.Name = name; row.UniqueName = name;
			row.ShortDescription = "a " + name; row.Keywords = name; row.FullDescription = "A disposable native qualification fixture.";
			row.ReadOnly = false; row.MorphTimeSeconds = 0; row.MorphGameItemProtoId = null; row.EditableItemId = 0; row.EditableItem = Editable();
			foreach (var part in components) row.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = part.Id, GameItemComponentRevision = part.RevisionNumber });
			db.GameItemProtos.Add(row); return row;
		}
		var light = Item("QA native light", componentSeed, wearable, lightComponent);
		var foods = Enumerable.Range(1, 3).Select(i => Item("QA native food " + i, componentSeed, foodComponent)).ToArray();
		var terrainSeed = db.Terrains.First(); var terrains = new List<long>();
		foreach (var name in new[] { "Air", "City", "Inside", "Hills", "Mountain", "Thornlands", "Earth", "Silt", "Shadow" })
		{ var row = Copy(terrainSeed); row.Id = 0; row.Name = "QA authored " + name; db.Terrains.Add(row); db.SaveChanges(); terrains.Add(row.Id); }
		var tags = new List<long>();
		for (var i = 0; i < 5; i++)
		{ var row = new Tag { Name = "QA Divination " + i, ParentId = tags.Count == 0 ? null : tags[^1], ShouldSeeProgId = yes.Id }; db.Tags.Add(row); db.SaveChanges(); tags.Add(row.Id); }
		var water = db.Liquids.First(x => x.Name == "Water").Id;
		var wine = db.Liquids.First(x => x.AlcoholLitresPerLitre > 0).Id;
		db.SaveChanges();
		bindings = new(new(true, school.Id, source.Id, skills, no.Id, eligibility.Id, water, light.Id, 0,
			componentSeed.Id, componentSeed.RevisionNumber, itemSeed.MaterialId, admin), reserve.Id, decorator, yes.Id, template.Id,
			new Dictionary<string, long> { ["arm.support.gather"] = gather.Id }, attribute.Id, expression.Id, "effective",
			ArmageddonTraditionInstaller.Variants.ToDictionary(x => x, _ => (IReadOnlyList<MagicGatheringMethodKind>)new[] { MagicGatheringMethodKind.Self }))
		{ WaterSee = new([water], terrains[7], terrains[8], tags), Emotions = new(attribute.Id, 1, 1, eligibility.Id, gather.Id, 1, eligibility.Id,
			Enumerable.Repeat(Difficulty.Impossible, 7).ToArray(), new(terrains[0], terrains[1], terrains[2], terrains[3], terrains[4], terrains[5], terrains[6])) };
		var initial = ArmageddonPreparedWorldInstaller.Install(factory, bindings with { WaterSee = null });
		Require(initial.Status == ArmageddonInstallStatus.Completed, initial.Describe());
		long Owned(string key) => db.SeederManagedRecords.AsNoTracking().Single(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key).LogicalId!.Value;
		bindings = bindings with { Provisions = new(true, school.Id, source.Id, no.Id,
			Owned(ArmageddonReviewedProvisionContent.SustainMealKey + ".skill"), Owned(ArmageddonReviewedProvisionContent.DrawWineKey + ".skill"),
			[new(32, 0, foods.Select(x => new ArmageddonFoodPrototype(x.Id, 0)).ToArray())], wine, [new(32, 0, wine)]) };
		File.WriteAllText(bindingsPath, ArmageddonMagicSeeder.SerializeBindings(bindings));
	}
	else
	{
		Require(args[2] is "rerun" or "failure-cases", "Unknown mode.");
		bindings = ArmageddonMagicSeeder.ParseBindings(File.ReadAllText(bindingsPath));
	}
}
Identity();
var result = ArmageddonPreparedWorldInstaller.Install(factory, bindings);
Require(result.Status == ArmageddonInstallStatus.Completed, result.Describe());
object? failureEvidence = null;
if (args[2] == "failure-cases")
{
	long ownedFuryId = 0;
	double uncommittedUnits = 0;
	XElement FuryDefinition()
	{
		using var read = factory();
		var id = read.SeederManagedRecords.AsNoTracking().Single(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.StableKey == ArmageddonEmotionalInstaller.FuryKey).LogicalId!.Value;
		ownedFuryId = id;
		return XElement.Parse(read.MagicSpells.AsNoTracking().Single(x => x.Id == id).Definition);
	}
	_ = FuryDefinition();
	string originalBuilderXml;
	string managedBaselineXml;
	using (var prepare = factory())
	{
		var owned = prepare.SeederManagedRecords.Single(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.StableKey == ArmageddonEmotionalInstaller.FuryKey && x.LogicalId == ownedFuryId && !x.Retired);
		var row = prepare.MagicSpells.Single(x => x.Id == ownedFuryId);
		originalBuilderXml = row.Definition;
		File.WriteAllText(receiptPath + ".original-builder-fury.xml", originalBuilderXml);
		// Native spell saves author an edited Definition. The installer must preserve it,
		// so temporarily select the recorded managed baseline to exercise a real payload write.
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(owned.SeedBaseline!)!;
		managedBaselineXml = JsonSerializer.Deserialize<string>(baseline[nameof(MudSharp.Models.MagicSpell.Definition)])!;
	}
	try
	{
		using (var prepareWrite = factory())
		{
			prepareWrite.MagicSpells.Single(x => x.Id == ownedFuryId).Definition = managedBaselineXml;
			Identity();
			prepareWrite.SaveChanges();
		}
		var originalDefinition = FuryDefinition();
		var before = Digest();
		var playerTablesBefore = TableChecksums();
		var changed = bindings with { Emotions = bindings.Emotions! with { UnitsPerSourcePoint = bindings.Emotions!.UnitsPerSourcePoint + 1 } };
		long interruptedConnection = 0;
		var interrupted = ArmageddonPreparedWorldInstaller.Install(factory, changed, (module, point) =>
		{
			if (module != ArmageddonEmotionalInstaller.Module || point != ArmageddonInstallCheckpoint.BeforeCommit) return;
			// This context is created by this invocation on the already verified disposable
			// instance. Interrupt only its connection after the real uncommitted writes.
			var context = lastContext ?? throw new InvalidOperationException("Missing owned installer context.");
			using var inspect = context.Database.GetDbConnection().CreateCommand();
			inspect.Transaction = context.Database.CurrentTransaction!.GetDbTransaction();
			inspect.CommandText = "SELECT CONCAT(@@datadir, '|', CONNECTION_ID())";
			var identity = ((string)inspect.ExecuteScalar()!).Split('|');
			Require(Path.GetFullPath(identity[0]).TrimEnd(Path.DirectorySeparatorChar)
				.Equals(expectedData, StringComparison.OrdinalIgnoreCase), "Installer connection left the owned instance.");
			interruptedConnection = long.Parse(identity[1]);
			Require(interruptedConnection > 0 && interruptedConnection != sql.ServerThread, "Refusing to interrupt the control connection.");
			inspect.CommandText = $"SELECT Definition FROM MagicSpells WHERE Id={ownedFuryId}";
			uncommittedUnits = (double)XElement.Parse((string)inspect.ExecuteScalar()!).Descendants("SourceProfile").Single().Attribute("units")!;
			Require(uncommittedUnits == changed.Emotions!.UnitsPerSourcePoint, "The interrupted transaction did not write its requested Fury mapping.");
			Identity();
			using var kill = new MySqlCommand($"KILL CONNECTION {interruptedConnection}", sql);
			kill.ExecuteNonQuery();
		});
		Require(interruptedConnection > 0 && interrupted.Status == ArmageddonInstallStatus.CommitOutcomeUnknown, interrupted.Describe());
		var afterInterruption = Digest();
		Require(before == afterInterruption, "Interrupted native transaction changed committed database content.");
		var acknowledgementLost = ArmageddonPreparedWorldInstaller.Install(factory, changed, (module, point) =>
		{
			if (module == ArmageddonEmotionalInstaller.Module && point == ArmageddonInstallCheckpoint.AfterCommit)
				throw new InvalidOperationException("Disposable qualification: lost acknowledgement after the actual MySQL commit.");
		});
		Require(acknowledgementLost.Status == ArmageddonInstallStatus.CommittedConfirmationFailed, acknowledgementLost.Describe());
		var committed = Digest();
		Require(committed != before, "Lost-acknowledgement case did not commit its selected mapping change.");
		Require((double)FuryDefinition().Descendants("SourceProfile").Single().Attribute("units")! == changed.Emotions!.UnitsPerSourcePoint,
			"The acknowledged-loss commit did not retain its selected Fury mapping.");
		var retry = ArmageddonPreparedWorldInstaller.Install(factory, changed);
		Require(retry.Status == ArmageddonInstallStatus.Completed && Digest() == committed, "Fresh retry changed the acknowledged native identities/content.");
		var playerTablesAfter = TableChecksums();
		var unexpected = playerTablesBefore.Where(x => !x.Key.Equals("MagicSpells", StringComparison.OrdinalIgnoreCase) &&
			!x.Key.Equals("SeederManagedRecords", StringComparison.OrdinalIgnoreCase) && playerTablesAfter[x.Key] != x.Value).Select(x => x.Key).ToArray();
		Require(unexpected.Length == 0, "Failure qualification changed player or other unrelated tables: " + string.Join(", ", unexpected));
		var restored = ArmageddonPreparedWorldInstaller.Install(factory, bindings);
		Require(restored.Status == ArmageddonInstallStatus.Completed, restored.Describe());
		Require(XNode.DeepEquals(originalDefinition, FuryDefinition()), "The original owned Fury definition was not restored.");
		failureEvidence = new { InterruptedConnection = interruptedConnection, UncommittedUnits = uncommittedUnits, InterruptedStatus = interrupted.Status.ToString(),
			DigestBefore = before, DigestAfterInterruption = afterInterruption, LostAcknowledgementStatus = acknowledgementLost.Status.ToString(),
			DigestAfterCommit = committed, RetryStable = true, PlayerAndOtherTablesStable = true, OriginalMappingRestored = true,
			ManagedBaselineFixturePrepared = true, BuilderDefinitionRestored = true };
	}
	finally
	{
		// This is stopped-fixture restoration, not an installer adoption of a builder edit.
		Identity();
		using var restoreBuilder = factory();
		Require(restoreBuilder.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.StableKey == ArmageddonEmotionalInstaller.FuryKey && x.LogicalId == ownedFuryId && !x.Retired),
			"The fixture lost its exact owned Fury identity before restoration.");
		var row = restoreBuilder.MagicSpells.Single(x => x.Id == ownedFuryId);
		row.Definition = originalBuilderXml;
		restoreBuilder.SaveChanges();
	}
	using var confirmBuilder = factory();
	Require(confirmBuilder.MagicSpells.AsNoTracking().Single(x => x.Id == ownedFuryId).Definition == originalBuilderXml,
		"Stopped-fixture restoration did not preserve the exact original builder definition.");
}
using (var db = factory())
{
	var records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package).ToArray();
	Require(records.Length == 217, "The explicit all-options fixture must own 217 records.");
	Require(records.Count(x => x.EntityType == "MagicSpell") == 12, "The selected fixture must own 12 payloads.");
	Require(result.Availability.All(x => x.StoredAdmissions.Count == 12), "Each selected variant must retain 12 admissions.");
	var before = Digest();
	var tablesBefore = TableChecksums();
	InstallerRows(".before.json");
	var repeat = ArmageddonPreparedWorldInstaller.Install(factory, bindings);
	Require(repeat.Status == ArmageddonInstallStatus.Completed, repeat.Describe());
	var tablesAfter = TableChecksums();
	File.WriteAllText(receiptPath + ".tables.json", JsonSerializer.Serialize(new { Before = tablesBefore, After = tablesAfter,
		Changed = tablesBefore.Where(x => tablesAfter[x.Key] != x.Value).Select(x => x.Key).ToArray() }, jsonOptions));
	InstallerRows(".after.json");
	Require(before == Digest(), "Selected rerun changed database content.");
	var nullRepeat = ArmageddonPreparedWorldInstaller.Install(factory, bindings with { Emotions = null });
	Require(nullRepeat.Status == ArmageddonInstallStatus.Completed, nullRepeat.Describe());
	Require(before == Digest(), "Null emotional selection changed database content.");
	File.WriteAllText(receiptPath, JsonSerializer.Serialize(new { Status = "PASS", Mode = args[2], OwnedData = expectedData,
		Target = cs.Database, ManagedRecords = records.Length, Payloads = 12, AdmissionsPerVariant = 12,
		SelectedAndNullRerunsStable = true, Digest = before, Bindings = bindingsPath, FailureCases = failureEvidence,
		Spells = records.Where(x => x.EntityType == "MagicSpell").Select(x => new { x.StableKey, x.LogicalId }),
		Capabilities = db.MagicCapabilities.Where(x => x.Name.StartsWith("Armageddon partial ")).Select(x => new { x.Id, x.Name }).ToArray() }, jsonOptions));
}
Console.WriteLine("PASS: real MySQL installer and selected/null reruns, explicit 217/12/12 fixture.");

#nullable enable
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.FutureProg;
using MudSharp.Framework.Revision;
using MudSharp.Magic;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedInstalledWorldAcceptance(TestDatabase database, Assembly assembly)
	{
		Require(!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_REPLAY_BOOT_SCRIPT")),
			"Installed qualification requires the explicitly selected exact-source native smoke script; preparation alone is not acceptance.");
		var players = TraditionPlayers(database);
		var bindings = PrepareInstalledSenseDependencies(database, out var preparation);
		Require(players == TraditionPlayers(database), "Builder preparation changed a player.");
		var laneRoot = Directory.GetParent(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")))!.FullName;
		var preparationReceipt = Path.Combine(laneRoot, "prepared-installed-sense-preparation_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(preparationReceipt, JsonSerializer.Serialize(preparation, new JsonSerializerOptions { WriteIndented = true }));
		Console.WriteLine($"ARMPREP-installed-dependencies=passed no-player-mutation separate-receipt:{preparationReceipt}");
		var transcript = PreparedMenu(database, true, ArmageddonMagicSeeder.SerializeBindings(bindings));
		Require(transcript.Contains("Completed") && transcript.Contains("4/82"), "Actual installed menu did not complete the four-admission package.");
		Require(players == TraditionPlayers(database), "Menu installation changed a player before explicit attachment/enrolment.");
		var ids = PreparedIdentities(database);
		Require(ids.Count == 196, "Installed acceptance expected only the reviewed four-admission package.");
		using var db = NewIndependentContext(database.ConnectionString);
		Require(ArmageddonPreparedWorldInstaller.ReadAvailability(db, ids.Where(x => x.Key.StartsWith("arm.capability.") || x.Key.StartsWith("arm.merit.")).ToDictionary(x => x.Key, x => x.Value!.Value))
			.All(x => x.Enabled && x.StoredAdmissions.Count == 4), "Saved installed capability availability changed.");
		var actor = db.Characters.AsNoTracking().Single(x => x.IsAdminAvatar && x.AccountId == bindings.Utilities.BuilderAccount);
		var languageId = actor.NativeLanguageId ?? throw new InvalidOperationException("Seeded Admin has no native spoken language; do not manufacture one.");
		Require(db.CharactersLanguages.Any(x => x.CharacterId == actor.Id && x.LanguageId == languageId), "Seeded Admin does not know its native language.");
		var language = db.Languages.AsNoTracking().Single(x => x.Id == languageId);
		var capacity = db.Traits.AsNoTracking().Single(x => x.BodyId == actor.BodyId && x.TraitDefinitionId == bindings.CapacityAttribute).Value * 10;
		var gatherAmount = Math.Min(100, Math.Floor(capacity));
		Require(gatherAmount >= 1, "Seeded Admin has no positive authored native reserve capacity; do not alter its attributes.");
		var input = Path.Combine(laneRoot, "prepared-installed-sense-input_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(input, JsonSerializer.Serialize(new
		{
			preparation, character = actor.Id, body = actor.BodyId, language = language.Id, language_name = language.Name,
			school = bindings.Utilities.School, school_verb = "armsense", resource = bindings.ReserveResource,
			capability = ids["arm.capability.sorcerer"], merit = ids["arm.merit.sorcerer"],
			spell = ids[ArmageddonReviewedUtilityContent.SenseEnchantmentKey],
			source_skill = ids[ArmageddonReviewedUtilityContent.SenseEnchantmentKey + ".skill"],
			capacity_attribute = bindings.CapacityAttribute, gather_amount = gatherAmount, native_capacity = capacity,
			installer_owned_records = ids.OrderBy(x => x.Key).ToArray(), initial_players_unchanged = true,
			limits = "Builder-prepared four-admission partial package; no Water integration, no Fury/Calm/Mend/device acquisition closure. Native aliases are authored explicitly; no historical vocabulary is invented."
		}, new JsonSerializerOptions { WriteIndented = true }));
		var previous = Environment.GetEnvironmentVariable("FUTUREMUD_PREPARED_INSTALLED_INPUT");
		try
		{
			Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_INSTALLED_INPUT", input);
			Console.WriteLine($"ARMPREP-installed-preparation=passed separate-builder-dependencies receipt:{input} no-player-mutation 196-owned-package-records");
			RunPreparedReplayBoot(database, assembly, "PreparedInstalledMagicSmoke.py", "prepared-installed-sense-native");
		}
		finally { Environment.SetEnvironmentVariable("FUTUREMUD_PREPARED_INSTALLED_INPUT", previous); }
	}

	private static ArmageddonPreparedWorldBindings PrepareInstalledSenseDependencies(TestDatabase database, out object preparation)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		Require(!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package), "Installed preparation requires a fresh declined world.");
		using var transaction = db.Database.BeginTransaction();
		var account = db.Accounts.AsNoTracking().Single(x => x.Name == "Admin");
		var decorator = db.TraitDecorators.AsNoTracking().Single(x => x.Name == "General Skill" && x.Type == "Range");
		var attribute = db.TraitDefinitions.AsNoTracking().Single(x => x.Name == "Intelligence" && x.Type == 1 && x.OwnerScope == 0);
		var skillTemplate = ResolveInstalledForageTemplate(db, out var forageSelection);
		var no = db.FutureProgs.AsNoTracking().Single(x => x.FunctionName == "AlwaysFalse");
		var yes = db.FutureProgs.AsNoTracking().Single(x => x.FunctionName == "AlwaysTrue");
		var zero = db.FutureProgs.AsNoTracking().Single(x => x.FunctionName == "AlwaysZero");
		var water = db.Liquids.AsNoTracking().Single(x => x.Name == "water");
		var material = db.Materials.AsNoTracking().OrderBy(x => x.Id).First();
		var hold = db.GameItemComponentProtos.Include(x => x.EditableItem).AsNoTracking().Where(x => x.Name == "Holdable" && x.Type == "Holdable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current).OrderByDescending(x => x.RevisionNumber).First();
		var profile = db.WearProfiles.AsNoTracking().Where(x => x.Type == "Direct" || x.Type == "Shape").OrderBy(x => x.Id).First();
		var existing = new { account = account.Id, decorator = decorator.Id, attribute = attribute.Id, skill_template = skillTemplate.Id,
			cap_expression = skillTemplate.ExpressionId, use_improver = skillTemplate.ImproverId, always_false = no.Id, always_true = yes.Id,
			always_zero = zero.Id, water = water.Id, material = material.Id, material_name = material.Name, holdable = hold.Id, holdable_revision = hold.RevisionNumber, wear_profile = profile.Id };
		var authored = new List<object>();
		void Record(string entity, long id, string name, string definition) => authored.Add(new { entity, id, name, definition, owner = "explicit disposable builder configuration; not installer-owned" });
		var school = new Db.MagicSchool { Name = "Installed Sense Acceptance", SchoolVerb = "armsense", SchoolAdjective = "arcane", PowerListColour = "boldcyan" };
		db.Add(school);
		var expression = new Db.TraitExpression { Name = "Installed Sense native Intelligence capacity", Expression = "variable*10" }; db.Add(expression); db.SaveChanges();
		Record("MagicSchool", school.Id, school.Name, school.SchoolVerb); Record("TraitExpression", expression.Id, expression.Name, expression.Expression);
		var resource = new Db.MagicResource { Name = "Installed Sense Reserve", ShortName = "isms", Type = "simple", MagicResourceType = 1,
			BottomColour = "red", MidColour = "yellow", TopColour = "cyan", Definition = new XElement("Definition",
				new XElement("ResourceCapProg", zero.Id), new XElement("ShouldStartWithResourceCharacterProg", no.Id), new XElement("StartingResourceAmountCharacterProg", zero.Id),
				new XElement("AttributeCapacity", new XAttribute("version", 1), new XAttribute("attribute", attribute.Id), new XAttribute("expression", expression.Id), new XAttribute("basis", "raw"))).ToString() };
		db.Add(resource); db.SaveChanges(); Record("MagicResource", resource.Id, resource.Name, resource.Definition);
		Db.TraitDefinition Skill(string name)
		{
			var row = new Db.TraitDefinition { Name = name, Alias = "", Type = 0, OwnerScope = 1, TraitGroup = "Installed acceptance dependencies", DecoratorId = decorator.Id,
				ImproverId = skillTemplate.ImproverId, ExpressionId = skillTemplate.ExpressionId, ChargenBlurb = "Explicit builder definition; no character grant", ValueExpression = "" };
			db.Add(row); db.SaveChanges(); Record("TraitDefinition", row.Id, row.Name, $"Native seeded cap {row.ExpressionId}, use improver {row.ImproverId}; no character trait values authored."); return row;
		}
		var gather = Skill("Installed Sense Gather");
		var skills = ArmageddonMagicInstaller.Content(new(false, 0, 0, new Dictionary<string, long>(), 0, 0, 0, 0, 0, 0, 0, 0, 0))
			.ToDictionary(x => x.Key, x => Skill("Installed utility binding " + x.Name).Id);
		var mend = new Db.FutureProg { FunctionName = "InstalledSenseSelfMend", FunctionText = "return @target == @caster", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString(),
			Category = "Acceptance", Subcategory = "Explicit preparation", FunctionComment = "Self-only builder adaptation; no invented setting categories" };
		foreach (var name in new[] { "target", "caster" }) mend.FutureProgsParameters.Add(new() { ParameterIndex = mend.FutureProgsParameters.Count, ParameterName = name, ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.Add(mend); db.SaveChanges(); Record("FutureProg", mend.Id, mend.FunctionName, mend.FunctionText);
		var capability = new Db.MagicCapability { Name = "Installed Sense paid gathering template", CapabilityModel = "skilllevel", MagicSchoolId = school.Id, PowerLevel = 1,
			Definition = new XElement("Definition", new XElement("ConcentrationTrait", gather.Id), new XElement("ConcentrationCapabilityExpression", "3"), new XElement("ConcentrationDifficultyExpression", "5"), new XElement("Regenerators"),
				new XElement("Gathering", new XAttribute("version", 2), new XElement("Method", new XAttribute("key", Guid.NewGuid()), new XAttribute("alias", "draw"),
					new XAttribute("name", "Paid Self gathering"), new XAttribute("kind", "Self"), new XAttribute("destination", resource.Id), new XAttribute("min", 1), new XAttribute("max", 100),
					new XAttribute("duration", 2), new XAttribute("ratio", 1), new XAttribute("stamina", 1), new XAttribute("minimumStamina", 0), new XAttribute("maximumHealthSeverity", "None")))).ToString() };
		db.Add(capability); db.SaveChanges(); Record("MagicCapability", capability.Id, capability.Name, capability.Definition);
		Db.EditableItem Approved() => new() { RevisionStatus = (int)RevisionStatus.Current, BuilderAccountId = account.Id, ReviewerAccountId = account.Id, BuilderDate = DateTime.UtcNow,
			ReviewerDate = DateTime.UtcNow, BuilderComment = "Explicit disposable builder preparation", ReviewerComment = "User-authorised acceptance dependency only" };
		Db.GameItemComponentProto Component(string type, string definition)
		{
			var row = new Db.GameItemComponentProto { Id = db.GameItemComponentProtos.Max(x => x.Id) + 1, RevisionNumber = 0, Name = "Installed Sense " + type, Type = type, Description = "Native acceptance dependency", Definition = definition, EditableItem = Approved() };
			db.Add(row); db.SaveChanges(); Record("GameItemComponentProto", row.Id, row.Name, definition); return row;
		}
		var wearable = Component("Wearable", InstalledWearableDefinition(profile.Id));
		var glow = Component(InstalledLightDatabaseType, InstalledLightDefinition());
		var componentValidation = ValidateInstalledComponents(new[] { hold, wearable, glow }, profile.Id);
		var light = new Db.GameItemProto { Id = db.GameItemProtos.Max(x => x.Id) + 1, RevisionNumber = 0, Name = "Installed Sense approved light dependency", UniqueName = "Installed Sense approved light dependency",
			MaterialId = material.Id, Size = 1, Weight = 0.01, BaseItemQuality = 5, Keywords = "light", ShortDescription = "a configured light", LongDescription = "A configured light is here.", FullDescription = "A builder-authored native light dependency.",
			EditableItem = Approved(), PlanarData = "<Definition/>" };
		foreach (var property in typeof(Db.GameItemProto).GetProperties().Where(x => x.PropertyType == typeof(string))) if (property.GetValue(light) is null) property.SetValue(light, "");
		foreach (var part in new[] { hold, wearable, glow }) light.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = part.Id, GameItemComponentRevision = part.RevisionNumber });
		db.Add(light); db.SaveChanges(); Record("GameItemProto", light.Id, light.Name, "Approved Holdable+Wearable+Prog Light; no load/wear scripts, hooks, morphing or live item instances.");
		var bindings = new ArmageddonPreparedWorldBindings(new(true, school.Id, resource.Id, skills, no.Id, mend.Id, water.Id, light.Id, 0, hold.Id, hold.RevisionNumber, material.Id, account.Id),
			resource.Id, decorator.Id, yes.Id, capability.Id, new Dictionary<string, long> { ["arm.support.gather"] = gather.Id }, attribute.Id, expression.Id, "raw",
			ArmageddonTraditionInstaller.Variants.ToDictionary(x => x, _ => (IReadOnlyList<MagicGatheringMethodKind>)new[] { MagicGatheringMethodKind.Self }));
		var errors = ArmageddonPreparedWorldInstaller.Validate(db, bindings); Require(errors.Count == 0, string.Join("; ", errors));
		Require(!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package), "Builder dependencies were claimed as package records.");
		transaction.Commit(); preparation = new { seeded_bindings = existing, forage_selection = forageSelection, component_validation = componentValidation, authored_dependencies = authored, players_untouched = true, starting_reserve = 0,
			preparation_kind = "Explicit native builder-format configuration in owned disposable world; not a stock world readiness certificate", bindings };
		return bindings;
	}

	private static Db.TraitDefinition ResolveInstalledForageTemplate(MudSharp.Database.FuturemudDatabaseContext db, out object selection)
	{
		var types = new[] { (int)CheckType.ForageCheck, (int)CheckType.ForageSpecificCheck, (int)CheckType.ForageTimeCheck };
		var checks = db.Checks.AsNoTracking().Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters)
			.Where(x => types.Contains(x.Type)).OrderBy(x => x.Type).ToArray();
		Require(checks.Length == 3, "All three native forage check identities must exist; no arbitrary skill fallback.");
		var references = new List<long>();
		foreach (var check in checks)
		{
			var matches = MudSharp.Body.Traits.TraitExpression.TraitFormulaRegex.Matches(check.TraitExpression.Expression);
			Require(matches.Count == 1 && matches[0].Groups["name"].Value.Equals("forage", StringComparison.OrdinalIgnoreCase) &&
				long.TryParse(matches[0].Groups["reference"].Value, out var id) && id > 0,
				$"Native forage check {check.Type} has no unique forage trait reference; stop rather than infer a name.");
			var reference = long.Parse(matches[0].Groups["reference"].Value);
			var suffix = (CheckType)check.Type switch { CheckType.ForageCheck => "", CheckType.ForageSpecificCheck => "-20", _ => "+20" };
			Require(check.TraitExpression.Expression.Replace(" ", "").Equals("forage:" + reference + suffix, StringComparison.OrdinalIgnoreCase),
				$"Native forage check {check.Type} differs from the completed profile's declared semantics.");
			references.Add(reference);
		}
		Require(references.Distinct().Count() == 1, "The native forage checks disagree on trait identity; no fallback.");
		var trait = db.TraitDefinitions.AsNoTracking().Single(x => x.Id == references[0]);
		Require(trait.Type == (int)MudSharp.Body.Traits.TraitType.Skill && trait.OwnerScope == (int)MudSharp.Body.Traits.TraitOwnerScope.Character,
			"Native forage checks must reference an ordinary character-owned skill.");
		var cap = db.TraitExpressions.AsNoTracking().Include(x => x.TraitExpressionParameters).SingleOrDefault(x => x.Id == trait.ExpressionId);
		var improver = db.Improvers.AsNoTracking().SingleOrDefault(x => x.Id == trait.ImproverId);
		Require(cap is not null && !string.IsNullOrWhiteSpace(cap.Expression) && improver is not null &&
			(improver.Type == "classic" || improver.Type == "branching") && !string.IsNullOrWhiteSpace(improver.Definition),
			"The resolved native forage skill must have a real existing cap and use improver.");
		object Expression(Db.TraitExpression row) => new { row.Id, row.Name, row.Expression,
			parameters = row.TraitExpressionParameters.OrderBy(x => x.Parameter).Select(x => new { x.Parameter, x.TraitDefinitionId, x.CanImprove, x.CanBranch }).ToArray() };
		selection = new
		{
			resolution = "Actual three native check identities and common formula reference; no display-name or stale-ID assumption",
			trait = new { trait.Id, trait.Name, trait.Type, trait.OwnerScope, trait.Alias, trait.TraitGroup, trait.DecoratorId, trait.ExpressionId, trait.ImproverId },
			cap = Expression(cap!), improver = new { improver!.Id, improver.Name, improver.Type, improver.Definition },
			checks = checks.Select(x => new { x.Type, name = ((CheckType)x.Type).ToString(), x.CheckTemplateId, x.MaximumDifficultyForImprovement, expression = Expression(x.TraitExpression) }).ToArray(),
			seeded_trait_unchanged = true, player_values_not_authored = true, installer_owned = false
		};
		var laneRoot = Directory.GetParent(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")))!.FullName;
		var path = Path.Combine(laneRoot, "prepared-installed-sense-forage-selection_" + Guid.NewGuid().ToString("N") + ".json");
		File.WriteAllText(path, JsonSerializer.Serialize(selection, new JsonSerializerOptions { WriteIndented = true }));
		Console.WriteLine($"ARMPREP-installed-forage-selection=passed separate-receipt:{path}");
		return trait;
	}
}

extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Models;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Internal opt-in API for the future package owner; deliberately absent from the seeder menu.
/// Content and ownership commit together. This API never loads or refreshes players.
/// </summary>
public static partial class ArmageddonMagicInstaller
{
	public static ArmageddonInstallResult Install(FuturemudDatabaseContext db, ArmageddonMagicInstallPlan plan,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var messages = new List<string>();
		var identities = new Dictionary<string, long>(StringComparer.Ordinal);
		ArmageddonInstallResult Result(ArmageddonInstallStatus status) => new(status, messages.ToArray(), new Dictionary<string, long>(identities));
		if (!plan.Install) { messages.Add("Partial package declined; no database access or mutation."); return Result(ArmageddonInstallStatus.Declined); }
		if (db.Database.CurrentTransaction is not null || db.ChangeTracker.Entries().Any())
		{ messages.Add("Use a fresh dedicated context without tracked objects or an existing transaction."); return Result(ArmageddonInstallStatus.Blocked); }
		var originalTracked = db.ChangeTracker.Entries().Select(x => x.Entity).ToHashSet();
		bool committed = false, commitAttempted = false;
		try
		{
			// Identity allocation and collision checks run under the same serializable transaction.
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : db.Database.BeginTransaction();
			var contributions = Contributions(plan);
			var records = db.SeederManagedRecords.Where(x => x.Seeder == Package && x.Module == Module).ToDictionary(x => x.StableKey);
			Preflight(db, plan, contributions, records, messages);
			if (messages.Count != 0) return Result(ArmageddonInstallStatus.Blocked);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			WriteContent(db, plan, contributions, records, messages);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			foreach (var contribution in contributions) identities.Add(contribution.Key, records[contribution.Key].LogicalId!.Value);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			commitAttempted = true; transaction.Commit(); committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("Committed reviewed five utilities and two blank device templates. No admissions, characters, classes, instances or charge banks installed or refreshed.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Content and ownership committed; confirmation failed. Rerun resolves the same owned IDs. " : commitAttempted ?
				"Commit outcome is unknown. Reconnect with a fresh context and rerun; atomic ownership resolves committed identities without duplication. " : "Batch failed before commit and rolled back. ") + error.Message);
			if (!committed) identities.Clear();
			return Result(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : commitAttempted ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed);
		}
		finally
		{
			// The clean-context requirement permits detaching this API's rows after rollback.
			// Previously tracked, unchanged caller objects retain their identity.
			if (!committed) foreach (var entry in db.ChangeTracker.Entries().Where(x => !originalTracked.Contains(x.Entity)).ToArray()) entry.State = EntityState.Detached;
		}
	}
	private static void Preflight(FuturemudDatabaseContext db, ArmageddonMagicInstallPlan plan,
		List<Contribution> contributions, Dictionary<string, SeederManagedRecord> records, List<string> messages)
	{
		ValidateManifest(contributions);
		foreach (var contribution in contributions)
		{
			if (records.TryGetValue(contribution.Key, out var record))
			{
				if (record.EntityType != EntityType(contribution.Type) || record.Retired || Find(db, contribution, record) is null)
				{ messages.Add($"{contribution.Key}: owned row missing, retired or identity type invalid; preserved. Restore or explicitly retire/rebind its dependent module before rerunning."); continue; }
				if (record.SeedBaseline is not null) _ = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline) ?? throw new InvalidOperationException($"Invalid baseline {record.StableKey}");
				if (contribution.Type == typeof(GameItemProto) && db.GameItemProtos.Include(x => x.EditableItem).Single(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber).EditableItem.RevisionStatus != (int)RevisionStatus.Current ||
					contribution.Type == typeof(GameItemComponentProto) && db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber).EditableItem.RevisionStatus != (int)RevisionStatus.Current)
					messages.Add($"{contribution.Key}: builder revision status is no longer current; preserved, explicit rebind required.");
				if (db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId && x.RevisionNumber == record.RevisionNumber))
					messages.Add($"{contribution.Key}: competing ownership claim; refuse identity takeover.");
				if (contribution.Type == typeof(GameItemProto) && db.GameItemProtos.Any(x => x.Id == record.LogicalId && x.RevisionNumber > record.RevisionNumber) ||
					contribution.Type == typeof(GameItemComponentProto) && db.GameItemComponentProtos.Any(x => x.Id == record.LogicalId && x.RevisionNumber > record.RevisionNumber))
					messages.Add($"{contribution.Key}: a later builder revision exists; explicit rebind required.");
			}
			else
			{
				if (db.SeederManagedRecords.Any(x => x.Seeder == Package && x.StableKey == contribution.Key)) messages.Add($"{contribution.Key}: key exists outside this module; refuse reclaim.");
				if (contribution.Type == typeof(MagicSpell) && (db.MagicSpells.Any(x => x.MagicSchoolId == plan.School && x.Name.ToLower() == contribution.Name.ToLower()) ||
					db.MagicSpells.AsNoTracking().Select(x => x.Definition).AsEnumerable().Any(x => StockKey(x) == contribution.Key)))
					messages.Add($"{contribution.Key}: unowned spell name or stock-identity collision; no automatic adoption.");
				if (contribution.Type == typeof(GameItemProto) && db.GameItemProtos.Any(x => x.Name.ToLower() == contribution.Name.ToLower()) ||
					contribution.Type == typeof(GameItemComponentProto) && db.GameItemComponentProtos.Any(x => x.Name.ToLower() == contribution.Name.ToLower()))
					messages.Add($"{contribution.Key}: unowned prototype name collision; no automatic adoption.");
			}
		}
		if (new[] { plan.School, plan.Resource, plan.Water, plan.LightPrototype, plan.HoldableComponent, plan.Material, plan.BuilderAccount, plan.AlwaysFalseProg, plan.MendEligibilityProg }.Any(x => x <= 0))
			messages.Add("External bindings must use explicit positive IDs.");
		if (!db.MagicSchools.Any(x => x.Id == plan.School) || !db.MagicResources.Any(x => x.Id == plan.Resource) ||
			!db.Liquids.Any(x => x.Id == plan.Water) || !db.Materials.Any(x => x.Id == plan.Material) || !db.Accounts.Any(x => x.Id == plan.BuilderAccount))
			messages.Add("Select existing school, resource, clean-water liquid, material and builder account IDs.");
		if (plan.WaterBonusPlane is { } plane && !db.Planes.Any(x => x.Id == plane)) messages.Add("Selected Water bonus plane is missing.");
		foreach (var content in Content(plan))
			if (!plan.SpellSkills.TryGetValue(content.Key, out var skill) || skill <= 0 || !db.TraitDefinitions.Any(x => x.Id == skill && x.Type == (int)TraitType.Skill && x.OwnerScope == (int)TraitOwnerScope.Character))
				messages.Add($"{content.Key}: select an existing character skill.");
		if (plan.SpellSkills.Values.Distinct().Count() != Content(plan).Count) messages.Add("Each spell requires its own selected skill; this partial installer does not create skills or admissions.");
		var holdable = db.GameItemComponentProtos.Include(x => x.EditableItem).SingleOrDefault(x => x.Id == plan.HoldableComponent && x.RevisionNumber == plan.HoldableRevision);
		if (holdable?.Type != "Holdable" || holdable.EditableItem.RevisionStatus != (int)RevisionStatus.Current) messages.Add("Select an approved native holdable component revision.");
		ValidateLight(db, plan, messages);
		var progs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToList();
		using var compiler = new OfflineProgCompilation(progs);
		if (progs.SingleOrDefault(x => x.Id == plan.AlwaysFalseProg) is not { } no || no.FunctionText.Trim() != "return false") messages.Add("Select the ordinary always-false support prog.");
		else if (compiler.Compile(no.Id) is var compiled && (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([]))) messages.Add("Always-false support prog must return boolean without parameters.");
		if (progs.All(x => x.Id != plan.MendEligibilityProg)) messages.Add("Mend Flesh requires a selected eligibility prog.");
		else if (compiler.Compile(plan.MendEligibilityProg) is var mend && (mend.ReturnType != ProgVariableTypes.Boolean || !mend.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character]))) messages.Add("Mend eligibility requires boolean (target, caster).");
		var generated = Content(plan).Select((x, i) => { var row = x.EligibilityRow(0); if (row is not null) row.Id = -1 - i; return row; }).Where(x => x is not null).Cast<FutureProg>().ToArray();
		using var generatedCompiler = new OfflineProgCompilation(progs.Concat(generated));
		foreach (var prog in generated) generatedCompiler.Compile(prog.Id);
	}
	private static string? StockKey(string xml) { try { return (string?)XElement.Parse(xml).Element("StockIdentity"); } catch { return null; } }
	private static void ValidateLight(FuturemudDatabaseContext db, ArmageddonMagicInstallPlan plan, List<string> messages)
	{
		var light = db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos).Include(x => x.GameItemProtosOnLoadProgs)
			.AsNoTracking().SingleOrDefault(x => x.Id == plan.LightPrototype && x.RevisionNumber == plan.LightRevision);
		if (light is null || light.EditableItem.RevisionStatus != (int)RevisionStatus.Current || light.ReadOnly || light.MorphTimeSeconds != 0 ||
			light.GameItemProtosOnLoadProgs.Count != 0 || db.DefaultHooks.Any(x => x.PerceivableType.ToLower() == "gameitem"))
		{ messages.Add("Hovering Light requires a current loadable unscripted native light without default GameItem hooks or morphing."); return; }
		if (db.GameItemProtos.Any(x => x.Id == light.Id && x.RevisionNumber > light.RevisionNumber && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current))
			messages.Add("Select the current Hovering Light revision resolved by runtime ID.");
		var components = light.GameItemProtosGameItemComponentProtos.Select(x => db.GameItemComponentProtos.Include(y => y.EditableItem)
			.AsNoTracking().SingleOrDefault(y => y.Id == x.GameItemComponentProtoId && y.RevisionNumber == x.GameItemComponentRevision)).ToArray();
		if (components.Length != 3 || components.Any(x => x is null || x.EditableItem.RevisionStatus != (int)RevisionStatus.Current) ||
			!components.Select(x => x!.Type).Order().SequenceEqual(new[] { "Holdable", "Prog Light", "Wearable" }.Order()))
		{ messages.Add("Hovering Light requires exactly approved Holdable, Wearable and Prog Light components."); return; }
		var wearable = XElement.Parse(components.Single(x => x!.Type == "Wearable")!.Definition);
		var profiles = wearable.Element("Profiles")?.Elements("Profile").Select(x => (long)x).ToArray() ?? [];
		var defaultProfile = (long?)wearable.Element("Profiles")?.Attribute("Default");
		if (profiles.Length == 0 || defaultProfile is null || !profiles.Contains(defaultProfile.Value) || profiles.Any(x => !db.WearProfiles.Any(y => y.Id == x)) ||
			profiles.Any(x => !db.WearProfiles.Any(y => y.Id == x && (y.Type == "Direct" || y.Type == "Shape"))) ||
			(long?)wearable.Element("WearableProg") is > 0 || (long?)wearable.Element("WhyCannotWearProg") is > 0)
			messages.Add("Hovering Light requires existing native wear profiles and no wear scripts.");
		var illumination = (double?)XElement.Parse(components.Single(x => x!.Type == "Prog Light")!.Definition).Element("IlluminationProvided");
		if (illumination is null || !double.IsFinite(illumination.Value) || illumination <= 0) messages.Add("Hovering Light requires finite positive illumination.");
	}
}

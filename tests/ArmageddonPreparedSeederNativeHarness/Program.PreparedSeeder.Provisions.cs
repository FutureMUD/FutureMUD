#nullable enable
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Magic;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static string PreparedProvisionPolicy(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return JsonSerializer.Serialize(new
		{
			Records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonProvisionInstaller.Module).OrderBy(x => x.StableKey)
				.Select(x => new { x.StableKey, x.LogicalId, x.SeedBaseline, x.AppliedFingerprint }).ToArray(),
			Spells = db.MagicSpells.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonProvisionInstaller.Module && y.EntityType == "MagicSpell" && y.LogicalId == x.Id))
				.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Definition }).ToArray(),
			Expressions = db.TraitExpressions.AsNoTracking().Where(x => db.SeederManagedRecords.Any(y => y.Module == ArmageddonProvisionInstaller.Module && y.EntityType == "TraitExpression" && y.LogicalId == x.Id))
				.OrderBy(x => x.Id).Select(x => new { x.Id, x.Name, x.Expression }).ToArray()
		});
	}
	private static void QualifyPreparedNewProvisions(TestDatabase database, ArmageddonPreparedWorldBindings bindings, Dictionary<string, long?> retained)
	{
		Require(ArmageddonPreparedWorldInstaller.NewProvisionsQualified, "Optional provision gate not enabled.");
		var selected = bindings with { Provisions = PreparedProvisionSelections(database, bindings, retained) };
		var json = ArmageddonMagicSeeder.SerializeBindings(selected); var initialPolicy = PreparedCapabilityMeritPolicy(database);
		var emptyProvisions = PreparedProvisionPolicy(database);
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var stopped = RunPrepared(database, selected, (module, point) =>
			{ if (module == ArmageddonProvisionInstaller.Module && point == boundary) throw new IOException("New provision interruption"); });
			Require(stopped.Status == ArmageddonInstallStatus.Failed && stopped.Modules.Count == 3 && PreparedIdentities(database).Count == 196 &&
				initialPolicy == PreparedCapabilityMeritPolicy(database) && emptyProvisions == PreparedProvisionPolicy(database),
				"New provision SQL rollback changed prior complete policy or left partial content.");
			RunPreparedReader(new(database.Name, json, retained, initialPolicy, ReadOnly: true, ProvisionsPolicy: emptyProvisions));
		}
		var uncertain = RunPrepared(database, selected, (module, point) =>
		{ if (module == ArmageddonProvisionInstaller.Module && point == ArmageddonInstallCheckpoint.AfterCommit) throw new IOException("Lost new provision acknowledgement"); });
		Require(uncertain.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && uncertain.Modules.Count == 3 &&
			PreparedIdentities(database).Count == 203 && initialPolicy == PreparedCapabilityMeritPolicy(database),
			"Committed provision acknowledgement failure advanced partial policy or lost owned content.");
		var committed = PreparedIdentities(database);
		RunPreparedReader(new(database.Name, json, committed, initialPolicy, ReadOnly: true, ProvisionsPolicy: PreparedProvisionPolicy(database)));
		var menu = PreparedMenu(database, true, json);
		Require(menu.Contains("7/82") && menu.Contains("Completed"), "Actual explicit provision menu did not resume complete composition.");
		var complete = RunPrepared(database, selected); RequirePrepared(complete);
		Require(complete.Modules.Count == 5 && complete.Availability.All(x => x.StoredAdmissions.Count == 7) &&
			committed.OrderBy(x => x.Key).SequenceEqual(PreparedIdentities(database).OrderBy(x => x.Key)), "New provision composition identities/closure wrong.");
		Console.WriteLine("ARMPREP-new-provisions=passed actual-menu explicit-native-selections five-modules seven-admissions real-SQL-rollback lost-ack-no-partial-policy stable-identities fresh-readers");
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
		foreach (var key in new[] { ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.DrawWineKey })
		{
			string original;
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				var cap = db.MagicCapabilities.Single(x => x.Id == committed["arm.capability." + variant]); original = cap.Definition;
				var xml = XElement.Parse(original); xml.Element("Casting")!.Elements("Admission").Single(x => (long?)x.Attribute("spell") == committed[key]).Remove();
				cap.Definition = xml.ToString(); db.SaveChanges();
			}
			var policy = PreparedCapabilityMeritPolicy(database); var provision = PreparedProvisionPolicy(database);
			for (var i = 0; i < 2; i++)
			{
				var rerun = RunPrepared(database, selected); RequirePrepared(rerun); var actual = rerun.Availability.Single(x => x.Variant == variant);
				Require(actual.StoredAdmissions.Count == 6 && !actual.StoredAdmissions.Contains(key) && policy == PreparedCapabilityMeritPolicy(database) &&
					provision == PreparedProvisionPolicy(database), "Provision removal or stock baselines changed across complete-plan reruns.");
			}
			var stopped = RunPrepared(database, selected, (module, point) =>
			{ if (module == ArmageddonProvisionInstaller.Module && point == ArmageddonInstallCheckpoint.BeforeCommit) throw new IOException("Existing provision interruption"); });
			Require(stopped.Status == ArmageddonInstallStatus.Failed && policy == PreparedCapabilityMeritPolicy(database) && provision == PreparedProvisionPolicy(database),
				"Interrupted new composition changed builder policy/baselines.");
			RunPreparedReader(new(database.Name, json, committed, policy, ReadOnly: true, ProvisionsPolicy: provision));
			RunPreparedReader(new(database.Name, json, committed, policy, ProvisionsPolicy: provision));
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				// Explicit fixture builder edit restores stock policy for the later runtime scenario.
				db.MagicCapabilities.Single(x => x.Id == committed["arm.capability." + variant]).Definition = original; db.SaveChanges();
			}
			Console.WriteLine($"ARMPREP-provision-removal=passed variant:{variant} key:{key} exact-spell-id:{committed[key]} two-reruns six-reported-admissions unchanged-policy-and-stock-baselines interruption fresh-readers");
		}
		QualifyPreparedProvisionOverrides(database, selected, committed, json);
	}
	private static void QualifyPreparedProvisionOverrides(TestDatabase database, ArmageddonPreparedWorldBindings selected, Dictionary<string, long?> ids, string json)
	{
		string name, cost, definition;
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			var meal = db.MagicSpells.Single(x => x.Id == ids[ArmageddonReviewedProvisionContent.SustainMealKey]); name = meal.Name; meal.Name = "prepared builder meal";
			var expression = db.TraitExpressions.Single(x => x.Id == ids[ArmageddonReviewedProvisionContent.SustainMealKey + ".cost"]); cost = expression.Expression; expression.Expression = "9*grade";
			var cap = db.MagicCapabilities.Single(x => x.Id == ids["arm.capability.preserver"]); definition = cap.Definition;
			var xml = XElement.Parse(definition); xml.Element("Casting")!.SetAttributeValue("enabled", false); cap.Definition = xml.ToString(); db.SaveChanges();
		}
		var policy = PreparedCapabilityMeritPolicy(database); var provision = PreparedProvisionPolicy(database);
		for (var i = 0; i < 2; i++)
		{
			var result = RunPrepared(database, selected); RequirePrepared(result);
			Require(!result.Availability.Single(x => x.Variant == "preserver").Enabled && policy == PreparedCapabilityMeritPolicy(database) && provision == PreparedProvisionPolicy(database),
				"Builder meal/cost/disabled casting override or stock baseline changed.");
		}
		RunPreparedReader(new(database.Name, json, ids, policy, ProvisionsPolicy: provision));
		using (var db = NewIndependentContext(database.ConnectionString))
		{
			// Explicit fixture builder edits restore qualified stock inputs for real casting checks.
			db.MagicSpells.Single(x => x.Id == ids[ArmageddonReviewedProvisionContent.SustainMealKey]).Name = name;
			db.TraitExpressions.Single(x => x.Id == ids[ArmageddonReviewedProvisionContent.SustainMealKey + ".cost"]).Expression = cost;
			db.MagicCapabilities.Single(x => x.Id == ids["arm.capability.preserver"]).Definition = definition; db.SaveChanges();
		}
		Console.WriteLine("ARMPREP-provision-overrides=passed builder-name builder-cost disabled-casting two-reruns byte-stable-content-and-stock-baselines fresh-reader");
	}
}

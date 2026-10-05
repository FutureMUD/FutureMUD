#nullable enable
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Magic;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static string PreparedCapabilityMeritPolicy(TestDatabase database)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		return JsonSerializer.Serialize(new
		{
			Records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Module == ArmageddonTraditionInstaller.Module &&
				(x.EntityType == "MagicCapability" || x.EntityType == "Merit")).OrderBy(x => x.StableKey)
				.Select(x => new { x.StableKey, x.LogicalId, x.SeedBaseline, x.AppliedFingerprint }).ToArray(),
			Capabilities = db.MagicCapabilities.AsNoTracking().Where(x => x.Name.StartsWith("Armageddon partial "))
				.OrderBy(x => x.Id).Select(x => new { x.Id, x.Definition }).ToArray(),
			Merits = db.Merits.AsNoTracking().OrderBy(x => x.Id).Select(x => new { x.Id, x.Definition }).ToArray()
		});
	}
	private static void VerifyPreparedReaderPolicy(TestDatabase database, PreparedReader input)
	{
		if (input.ProvisionsPolicy is not null)
			Require(input.ProvisionsPolicy == PreparedProvisionPolicy(database), "Fresh reader changed provision content or stock baselines.");
		if (input.Policy is null) return;
		Require(input.Policy == PreparedCapabilityMeritPolicy(database), "Fresh reader changed capability/merit XML or stock baselines.");
		if (input.WithoutPierceVariant is null) return;
		using var db = NewIndependentContext(database.ConnectionString);
		var cap = db.MagicCapabilities.AsNoTracking().Single(x => x.Id == input.Identities["arm.capability." + input.WithoutPierceVariant]);
		var admissions = XElement.Parse(cap.Definition).Element("Casting")!.Elements("Admission").ToArray();
		Require(admissions.Length == 3 && admissions.All(x => (long?)x.Attribute("spell") != input.Pierce),
			"Fresh reader restored the exact removed Pierce admission.");
	}
	private static void QualifyFreshPreparedBootstrap(TestDatabase database, ArmageddonPreparedWorldBindings bindings, string json)
	{
		foreach (var boundary in new[] { ArmageddonInstallCheckpoint.ContentCreated, ArmageddonInstallCheckpoint.BeforeCommit })
		{
			var stopped = RunPrepared(database, bindings, (module, point) =>
			{
				if (module == ArmageddonPierceInstaller.Module && point == boundary) throw new IOException("Before first Pierce commit");
			});
			Require(stopped.Status == ArmageddonInstallStatus.Failed && stopped.Modules.Count == 3 && PreparedIdentities(database).Count == 186,
				"Fresh bootstrap/Pierce transaction did not retain exactly21+165 definitions.");
			using (var db = NewIndependentContext(database.ConnectionString))
				Require(!db.SeederManagedRecords.Any(x => x.EntityType == "MagicCapability" || x.EntityType == "Merit"),
					"Interrupted bootstrap created partial casting authority.");
			RunPreparedReader(new(database.Name, json, PreparedIdentities(database), PreparedCapabilityMeritPolicy(database), ReadOnly: true));
		}
		Console.WriteLine("ARMPREP-bootstrap=passed real-Pierce-rollback21-plus165 stable-owned-skills no-capability-or-merit-authority fresh-readers");
	}
	private static void QualifyPreparedComposition(TestDatabase database, ArmageddonPreparedWorldBindings bindings, string json,
		Dictionary<string, long?> retained)
	{
		var pierce = retained[ArmageddonReviewedPierceContent.Key]!.Value;
		foreach (var variant in ArmageddonTraditionInstaller.Variants)
		{
			string original;
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				var cap = db.MagicCapabilities.Single(x => x.Id == retained["arm.capability." + variant]); original = cap.Definition;
				var xml = XElement.Parse(original); xml.Element("Casting")!.Elements("Admission").Single(x => (long?)x.Attribute("spell") == pierce).Remove();
				cap.Definition = xml.ToString(); db.SaveChanges();
			}
			var policy = PreparedCapabilityMeritPolicy(database);
			for (var i = 0; i < 2; i++)
			{
				var result = RunPrepared(database, bindings); RequirePrepared(result);
				var availability = result.Availability.Single(x => x.Variant == variant);
				Require(availability.StoredAdmissions.Count == 3 && !availability.StoredAdmissions.Contains(ArmageddonReviewedPierceContent.Key) &&
					policy == PreparedCapabilityMeritPolicy(database), "Complete-plan rerun restored Pierce or adopted the removed policy as stock baseline.");
			}
			RunPreparedReader(new(database.Name, json, retained, policy, variant, pierce));
			var stopped = RunPrepared(database, bindings, (module, point) =>
			{
				if (module == ArmageddonPierceInstaller.Module && point == ArmageddonInstallCheckpoint.BeforeCommit) throw new IOException("Existing Pierce interruption");
			});
			Require(stopped.Status == ArmageddonInstallStatus.Failed && policy == PreparedCapabilityMeritPolicy(database),
				"Interrupted bootstrap changed existing capability/merit policy or baseline.");
			RunPreparedReader(new(database.Name, json, retained, policy, variant, pierce, ReadOnly: true));
			var resumed = RunPrepared(database, bindings); RequirePrepared(resumed);
			Require(resumed.Availability.Single(x => x.Variant == variant).StoredAdmissions.Count == 3 && policy == PreparedCapabilityMeritPolicy(database),
				"Resume restored removed casting authority.");
			using (var db = NewIndependentContext(database.ConnectionString))
			{
				// A separate explicit builder edit restores the fixture after the refusal checks.
				db.MagicCapabilities.Single(x => x.Id == retained["arm.capability." + variant]).Definition = original; db.SaveChanges();
			}
			Console.WriteLine($"ARMPREP-composition=passed variant:{variant} exact-Pierce-id:{pierce} two-reruns byte-stable-XML-and-stock-baselines reported-three-admissions interruption-before-Pierce-commit fresh-readers");
		}
	}
}

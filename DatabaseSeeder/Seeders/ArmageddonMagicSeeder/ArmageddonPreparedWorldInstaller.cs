#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonModuleReceipt(string Module, ArmageddonInstallStatus Status,
	IReadOnlyList<string> Messages, IReadOnlyDictionary<string, long> Identities);
public sealed record ArmageddonPersistedAvailability(string Variant, long Capability, bool Enabled,
	IReadOnlyList<string> StoredAdmissions, IReadOnlyList<string> WithoutStoredAdmission);
public sealed record ArmageddonPreparedWorldInstallResult(ArmageddonInstallStatus Status,
	IReadOnlyList<ArmageddonModuleReceipt> Modules, IReadOnlyList<ArmageddonPersistedAvailability> Availability,
	IReadOnlyList<string> Messages)
{
	public string Describe() => string.Join(Environment.NewLine,
		new[] { $"Armageddon partial prepared-world installer: {Status}." }
			.Concat(Modules.SelectMany(x => new[] { $"Module {x.Module}: {x.Status}; {x.Identities.Count} identity receipts." }.Concat(x.Messages)))
			.Concat(Availability.Select(x => $"Persisted {x.Variant} capability #{x.Capability}: enabled={x.Enabled}; " +
				$"{x.StoredAdmissions.Count}/82 stored source admissions, {x.WithoutStoredAdmission.Count} without stored admission. " +
				"These are definitions, not character acquisition or a gameplay validation receipt."))
			.Concat(Messages));
}

public static partial class ArmageddonPreparedWorldInstaller
{
	// Temporary runtime qualification gate. Lift only after a reviewed callback-free consumption
	// repair and native Active Sense eating/reload verification; keep module composition unchanged.
	public static bool NewProvisionsQualified => false;
	public const string ProvisionReadiness = "New provision installation is temporarily unavailable pending independently reviewed combined Active Sense eating/reload qualification. The prior defect left credited zero-bite food held in Retiring state; separate runtime repair clearance alone does not lift this gate. Existing owned provision definitions and admissions are preserved.";

	public static ArmageddonPreparedWorldInstallResult Install(Func<FuturemudDatabaseContext> freshContext,
		ArmageddonPreparedWorldBindings bindings, Action<string, ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		var receipts = new List<ArmageddonModuleReceipt>();
		var availability = new List<ArmageddonPersistedAvailability>();
		var messages = new List<string> { ProvisionReadiness };
		var definitionsCommitted = false;
		ArmageddonPreparedWorldInstallResult Result(ArmageddonInstallStatus status) =>
			new(status, receipts.ToArray(), availability.ToArray(), messages.Concat(status != ArmageddonInstallStatus.Completed && receipts.Count > 0
				? new[] { "Stopped at the first unsuccessful module/boundary; earlier completed modules remain committed. No automatic retry. Inspect ownership from a fresh connection before rerunning; do not replay player actions." }
				: Array.Empty<string>()).ToArray());
		bool Record(string module, ArmageddonInstallStatus status, IReadOnlyList<string> details, IReadOnlyDictionary<string, long> ids)
		{
			receipts.Add(new(module, status, details, new Dictionary<string, long>(ids)));
			return status == ArmageddonInstallStatus.Completed;
		}
		try
		{
			using (var db = freshContext())
			{
				var errors = Validate(db, bindings);
				if (errors.Count != 0) { messages.AddRange(errors); return Result(ArmageddonInstallStatus.Blocked); }
			}
			ArmageddonInstallResult utilities;
			using (var db = freshContext()) utilities = ArmageddonMagicInstaller.Install(db, bindings.Utilities,
				x => checkpoint?.Invoke(ArmageddonMagicInstaller.Module, x));
			if (!Record(ArmageddonMagicInstaller.Module, utilities.Status, utilities.Messages, utilities.Identities)) return Result(utilities.Status);
			var spells = utilities.Identities.Where(x => bindings.Utilities.SpellSkills.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value);
			using (var db = freshContext())
			{
				foreach (var spell in PreservedProvisionSpells(db)) spells.Add(spell.Key, spell.Value);
				messages.Add(spells.ContainsKey(ArmageddonReviewedProvisionContent.SustainMealKey) ? "Preserved existing owned provisions without running the gated provision module." : "No new provision definitions installed.");
			}
			var plan = new ArmageddonTraditionInstallPlan(true, bindings.Utilities.School, bindings.Utilities.Resource,
				bindings.ReserveResource, bindings.Decorator, bindings.Utilities.AlwaysFalseProg, bindings.AlwaysTrueProg,
				bindings.GatheringTemplate, spells, bindings.SupportSkills, bindings.AllowedMethods);
			ArmageddonTraditionInstallResult traditions;
			using (var db = freshContext()) traditions = ArmageddonTraditionInstaller.Install(db, plan,
				x => checkpoint?.Invoke(ArmageddonTraditionInstaller.Module, x));
			if (!Record(ArmageddonTraditionInstaller.Module, traditions.Status, traditions.Messages, traditions.Identities)) return Result(traditions.Status);
			// Reserved composition point for the reviewed consumption repair. Current readiness
			// validation rejects new selections; existing provisions are never reconciled here.
			if (bindings.Provisions is { } provision)
			{
				if (!NewProvisionsQualified) { messages.Add(ProvisionReadiness); return Result(ArmageddonInstallStatus.Blocked); }
				ArmageddonInstallResult provisions;
				using (var db = freshContext()) provisions = ArmageddonProvisionInstaller.Install(db, provision,
					x => checkpoint?.Invoke(ArmageddonProvisionInstaller.Module, x));
				if (!Record(ArmageddonProvisionInstaller.Module, provisions.Status, provisions.Messages, provisions.Identities)) return Result(provisions.Status);
				foreach (var key in new[] { ArmageddonReviewedProvisionContent.SustainMealKey, ArmageddonReviewedProvisionContent.DrawWineKey }) spells[key] = provisions.Identities[key];
			}
			ArmageddonInstallResult pierce;
			using (var db = freshContext()) pierce = ArmageddonPierceInstaller.Install(db,
				new(true, bindings.Utilities.School, bindings.Utilities.Resource, bindings.Utilities.AlwaysFalseProg,
					traditions.Identities[ArmageddonReviewedPierceContent.Key + ".skill"]),
				x => checkpoint?.Invoke(ArmageddonPierceInstaller.Module, x));
			if (!Record(ArmageddonPierceInstaller.Module, pierce.Status, pierce.Messages, pierce.Identities)) return Result(pierce.Status);
			spells.Add(ArmageddonReviewedPierceContent.Key, pierce.Identities[ArmageddonReviewedPierceContent.Key]);
			using (var db = freshContext()) traditions = ArmageddonTraditionInstaller.Install(db, plan with { ImplementedSpells = spells },
				x => checkpoint?.Invoke(ArmageddonTraditionInstaller.Module + ":admissions", x));
			if (!Record(ArmageddonTraditionInstaller.Module + ":admissions", traditions.Status, traditions.Messages, traditions.Identities)) return Result(traditions.Status);
			definitionsCommitted = true;
			using (var db = freshContext()) availability.AddRange(ReadAvailability(db, traditions.Identities));
			messages.Add("Mend Flesh's definition exists but its source prerequisite path is unavailable. Blank device templates contain no charges; stock Mend-only production/recharge remains unattainable through this partial source graph.");
			messages.Add("No new prerequisites, character acquisition/mastery, enrolment, classes, item instances, charge banks or reserve refills. Direct casting and device use still require live runtime eligibility, payment and current entitlement.");
			return Result(ArmageddonInstallStatus.Completed);
		}
		catch (Exception error)
		{
			messages.Add("Stopped between module boundaries: " + error.Message);
			return Result(definitionsCommitted ? ArmageddonInstallStatus.CommittedConfirmationFailed : ArmageddonInstallStatus.Failed);
		}
	}
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

/// <summary>Development-only prepared-world entry point. Each module owns its transaction and receipt.</summary>
public sealed partial class ArmageddonMagicSeeder : IDatabaseSeeder
{
	public const string ReleaseDisabledMessage = "Armageddon magic package is disabled in Release until completion. Existing world data and generic magic runtime remain available.";
#if DEBUG
	public bool Enabled => true;
#else
	public bool Enabled => false;
#endif
	public int SortOrder => 305;
	public string Name => "Armageddon Magic (partial, prepared world)";
	public string Tagline => Enabled ? "Development-only reviewed magic definitions; explicit existing-world bindings required" : ReleaseDisabledMessage;
	public string FullDescription => !Enabled ? ReleaseDisabledMessage : "Development-only: installs reviewed utilities, blank devices, source traditions and Pierce Magick. " +
		"This is a partial prepared-world package, not a complete preset. No character attachment, enrolment, " +
		"acquisition, reserve refill, charges or classes are created. Optional provisions require explicit native " +
		"food profiles, wine recipes and existing spell-skill bindings; unselected owned provisions are preserved. " +
		"Optional Water/See uses explicit native liquid, terrain and Divination-rank mappings with an owned prerequisite path; null preserves owned definitions. " +
		"Optional Fury/Calm uses authored emotional mappings and unambiguous ordinary-attribute inference or explicit override; native acceptance remains pending. " +
		"See Design Documents/Magic/Armageddon_Partial_Installer_Builder_Guide.md.";
	public bool SafeToRunMoreThanOnce => true;
	public SeederMetadata Metadata => SeederMetadataRegistry.GetMetadata(this);
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context) =>
		!Enabled ? ShouldSeedResult.PrerequisitesNotMet : context.SeederManagedRecords.AsNoTracking().Any(x => x.Seeder == ArmageddonMagicInstaller.Package)
			? ShouldSeedResult.ExtraPackagesAvailable : ShouldSeedResult.ReadyToInstall;
	public SeederAssessment AssessSeedData(FuturemudDatabaseContext context) => !Enabled
		? new(SeederAssessmentStatus.Blocked, ReleaseDisabledMessage, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>())
		: new(
		ShouldSeedData(context) == ShouldSeedResult.ReadyToInstall ? SeederAssessmentStatus.ReadyToInstall : SeederAssessmentStatus.UpdateAvailable,
		"Optional configuration/decline available. Opt-in requires a prepared world and validated explicit bindings; no full-preset readiness is implied.",
		Array.Empty<string>(), [ArmageddonPreparedWorldInstaller.ProvisionReadiness, ArmageddonPreparedWorldInstaller.WaterSeeReadiness, ArmageddonPreparedWorldInstaller.EmotionalReadiness],
		["Default No on every visit. Each completed module remains committed if a later module stops.",
		 "Bind an authored body attribute capacity, native gathering template, progs, skills and approved item revisions explicitly."]);

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!Enabled) return ReleaseDisabledMessage;
		if (!questionAnswers.TryGetValue(InstallQuestion, out var answer) || !IsYes(answer))
			return "Armageddon partial package declined. No package content or player state changed. Generic answer-memory housekeeping may still run.";
		if (!questionAnswers.TryGetValue(BindingsQuestion, out var json)) throw new InvalidOperationException("Explicit Armageddon bindings are missing.");
		var validation = ValidateBindings(json, context);
		if (!validation.Success) throw new InvalidOperationException(validation.error);
		if (context.Database.CurrentTransaction is not null || context.ChangeTracker.HasChanges())
			throw new InvalidOperationException("Use a clean caller context without pending changes or a transaction.");
		var options = context.GetService<IDbContextOptions>();
		if (options is not DbContextOptions<FuturemudDatabaseContext> typed ||
			options.Extensions.OfType<RelationalOptionsExtension>().Any(x => x.Connection is not null))
			throw new InvalidOperationException("Use connection-string context options, not a shared external connection, so every module gets an independent context.");
		// Interactive questions use lazy proxies. Module ownership serializers deliberately use
		// native model types; give each module plain-model contexts while preserving provider,
		// target, model and connection interceptors. Do not alter the caller's question context.
		var moduleOptions = new DbContextOptionsBuilder<FuturemudDatabaseContext>(typed).UseLazyLoadingProxies(false).Options;
		var result = ArmageddonPreparedWorldInstaller.Install(() => new FuturemudDatabaseContext(moduleOptions), ParseBindings(json));
		var report = result.Describe();
		if (result.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(report);
		return report;
	}
}

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

/// <summary>Development-only package settings and retained prepared-world APIs.</summary>
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
	public string Tagline => Enabled ? "Development-only package progression; automatic native foundation mapping" : ReleaseDisabledMessage;
	public string FullDescription => !Enabled ? ReleaseDisabledMessage : "Development-only: validates the approved stock graph and native foundations, then fixes " +
		"one game-wide Classic/Simple mode and the earlier Chargen class direction. Settings and their shared answers commit together before catalogue modules. " +
		"S2 creates native skills, ten disabled capability/merit pairs, eight reserves and conditional Class roles. S3 reconciles native supplies, recipes, weapons, provisions, creature templates/controllers and specialised stock in resumable batches. S4 begins with the canonical Read Attunement definition and its cost/duration children. Class availability and spell admissions await S4/S5 qualification. No existing player state is changed. " +
		"Existing prepared-world APIs remain available for historical packages without fixed full-package settings. " +
		"See Design Documents/Magic/Armageddon_Installation_Settings_S1_20261011.md.";
	public bool SafeToRunMoreThanOnce => true;
	public SeederMetadata Metadata => SeederMetadataRegistry.GetMetadata(this);
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context) =>
		!Enabled ? ShouldSeedResult.PrerequisitesNotMet : context.SeederManagedRecords.AsNoTracking().Any(x => x.Seeder == ArmageddonMagicInstaller.Package)
			? ShouldSeedResult.ExtraPackagesAvailable : ShouldSeedResult.ReadyToInstall;
	public SeederAssessment AssessSeedData(FuturemudDatabaseContext context) => !Enabled
		? new(SeederAssessmentStatus.Blocked, ReleaseDisabledMessage, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<string>())
		: new(
		ShouldSeedData(context) == ShouldSeedResult.ReadyToInstall ? SeederAssessmentStatus.ReadyToInstall : SeederAssessmentStatus.UpdateAvailable,
		"Optional fixed settings, progression and native catalogue. Opt-in requires validated native foundations; full spell hydration and final readiness remain pending.",
		Array.Empty<string>(), Array.Empty<string>(),
		["Default No on every visit. Fixed settings and shared answers commit atomically before catalogue work.",
		 "Classic/Simple and the earlier class direction cannot change on rerun. Ambiguous native primitives require explicit typed bindings."]);


	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (!Enabled) return ReleaseDisabledMessage;
		if (FullConsent(questionAnswers))
		{
			if (questionAnswers.TryGetValue(InstallQuestion, out var partial) && IsYes(partial)) throw new InvalidOperationException("Choose the package settings stage or the retained prepared-world API, not both.");
			var factory = ContextFactory(context);
			var prior = ArmageddonInstallation.Read(context);
			ArmageddonProgressionMode? mode = null;
			if (questionAnswers.TryGetValue(ModeQuestion, out var modeText))
			{
				if (!Enum.GetNames<ArmageddonProgressionMode>().Contains(modeText.Trim(), StringComparer.OrdinalIgnoreCase)) throw new InvalidOperationException("Choose Classic or Simple.");
				mode = Enum.Parse<ArmageddonProgressionMode>(modeText.Trim(), true);
			}
			else if (prior is null) throw new InvalidOperationException("First Armageddon installation requires an explicit Classic/Simple selection.");
			long? capacity = null;
			if (questionAnswers.TryGetValue(CapacityQuestion, out var capacityText))
			{
				if (!long.TryParse(capacityText, out var id) || id <= 0) throw new InvalidOperationException("Capacity attribute ID must be positive.");
				capacity = id;
			}
			var overrides = questionAnswers.TryGetValue(NativeBindingsQuestion, out var nativeJson) ? ArmageddonInstallation.ParseOverrides(nativeJson) : null;
			var settings = ArmageddonInstallation.Prepare(factory, mode, capacity, overrides);
			var progression = ArmageddonProgressionInstaller.Install(factory);
			var progressionReport = string.Join("\n", progression.Messages);
			if (progression.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(progressionReport);
			var catalogue = ArmageddonCatalogueInstaller.Install(factory);
			var catalogueReport = string.Join("\n", catalogue.Messages);
			if (catalogue.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(progressionReport + "\n" + catalogueReport);
			var spells = ArmageddonStockSpellInstaller.Install(factory);
			var spellReport = string.Join("\n", spells.Messages);
			if (spells.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(progressionReport + "\n" + catalogueReport + "\n" + spellReport);
			var focusDevices = ArmageddonStockFocusDeviceInstaller.Install(factory);
			var focusDeviceReport = string.Join("\n", focusDevices.Messages);
			if (focusDevices.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(focusDeviceReport);
			var completeProgression = ArmageddonProgressionInstaller.Install(factory, completeStock: true);
			var completeProgressionReport = string.Join("\n", completeProgression.Messages);
			if (completeProgression.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(completeProgressionReport);
			return $"Armageddon fixed settings ready: {settings.Mode}, classes {(settings.ClassesEnabled ? "enabled" : "disabled")}. Settings and shared answers are durable.\n{progressionReport}\n{catalogueReport}\n{spellReport}\n{focusDeviceReport}\n{completeProgressionReport}\nComplete stock admission/support defaults are installed with availability disabled. S6 gathering integration and whole-programme readiness remain pending; Release is disabled.";
		}
		if (!questionAnswers.TryGetValue(InstallQuestion, out var answer) || !IsYes(answer))
			return "Armageddon package declined. No package content or player state changed. Generic answer-memory housekeeping may still run.";
		if (!questionAnswers.TryGetValue(BindingsQuestion, out var json)) throw new InvalidOperationException("Explicit Armageddon bindings are missing.");
		if (ArmageddonInstallation.Read(context) is not null) throw new InvalidOperationException("Fixed package settings already exist. Resume the package installation rather than bypassing its mode through the prepared-world API.");
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

	private static Func<FuturemudDatabaseContext> ContextFactory(FuturemudDatabaseContext context)
	{
		if (context.Database.CurrentTransaction is not null || context.ChangeTracker.HasChanges()) throw new InvalidOperationException("Use a clean caller context without pending changes or a transaction.");
		if (context.GetService<IDbContextOptions>() is not DbContextOptions<FuturemudDatabaseContext> options || options.Extensions.OfType<RelationalOptionsExtension>().Any(x => x.Connection is not null))
			throw new InvalidOperationException("Use connection-string options so settings and content modules have independent contexts.");
		var independent = new DbContextOptionsBuilder<FuturemudDatabaseContext>(options).UseLazyLoadingProxies(false).Options;
		return () => new FuturemudDatabaseContext(independent);
	}
}

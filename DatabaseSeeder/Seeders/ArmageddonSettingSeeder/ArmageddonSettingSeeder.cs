#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

/// <summary>Opt-in v1 package settings and retained prepared-world APIs.</summary>
public sealed partial class ArmageddonSettingSeeder : IDatabaseSeeder
{
	public bool Enabled => true;
	public int SortOrder => 305;
	// Public installer identity; durable content ownership remains independent.
	public string Name => "Armageddon Setting";
	public string Tagline => "Classes, magic, psionics and heritage inspired by classical Armageddon";
	public string FullDescription => "Installs classes, subclasses, magic, psionics and game settings inspired by classical Armageddon, " +
		"along with its races, languages and equipment. Skills follow Armageddon's class-based branching system, " +
		"and powers of the Way are granted through class rules. " +
		"Use the Armageddon skill package with classes and subclasses enabled; the separate Psionics seeder is not required. " +
		"Optional regional weather templates use Armageddon time, Suk-Krath and shared weather foundations. " +
		"Builders create the world's cities, rooms and stories.";
	public bool SafeToRunMoreThanOnce => true;
	public SeederMetadata Metadata => SeederMetadataRegistry.GetMetadata(this);
	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context) =>
		context.SeederManagedRecords.AsNoTracking().Any(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module != ArmageddonSkillPackage.Module && x.Module != "setting-combat" && x.Module != ArmageddonMundaneBindings.Module && x.Module != ArmageddonSettingLinks.Module)
			? ShouldSeedResult.ExtraPackagesAvailable : ShouldSeedResult.ReadyToInstall;
	public SeederAssessment AssessSeedData(FuturemudDatabaseContext context)
    {
        var missing = new List<string>();
        try
        {
            var installed = ArmageddonInstallation.Read(context);
            var classes = ArmageddonInstallation.ClassesEnabled(context);
            ArmageddonInstallation.RequireSettingRoleDirection(context);
            var candidate = installed ?? new ArmageddonInstallationSettings(1, ArmageddonProgressionMode.Classic,
                classes, ArmageddonInstallation.ResolveCapacityAttribute(context), ArmageddonInstallation.ResolveNativeBindings(context));
            missing.AddRange(ArmageddonInstallation.ValidateFoundations(context, candidate, checkCapacity: false));
        }
        catch (InvalidOperationException error) { missing.Add(error.Message); }
        foreach (var name in new[] { "Cranequin", "Goat's Foot", "Lever", "Spanning Hook", "Windlass" })
        {
            try { CoreTagCatalogue.RequireUniqueStockName(context, name); }
            catch (InvalidOperationException error) { missing.Add(error.Message); }
        }
        if (!context.WeaponTypes.Any()) missing.Add("Combat weapon foundations must be installed before the full Armageddon package.");
        try { ArmageddonCatalogueSpecials.ResolveHeadShape(context); }
        catch (InvalidOperationException error) { missing.Add(error.Message); }
        return new SeederAssessment(missing.Count > 0 ? SeederAssessmentStatus.Blocked :
                ShouldSeedData(context) == ShouldSeedResult.ReadyToInstall ? SeederAssessmentStatus.ReadyToInstall : SeederAssessmentStatus.UpdateAvailable,
            missing.Count > 0 ? "Full Armageddon foundations are incomplete. Core, Time, attributes/skills, Human, Combat and Chargen supply the baseline." :
                "The Armageddon setting is ready to install or update.",
            missing, Array.Empty<string>(),
            ["Installation is optional. Reruns retain your original progression settings and builder customisations.",
             "Useful Items, Item, Health and Psionics are optional; this setting does not require them."]);
    }


	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (FullConsent(questionAnswers))
		{
			if (questionAnswers.TryGetValue(InstallQuestion, out var partial) && IsYes(partial)) throw new InvalidOperationException("Choose the package settings stage or the retained prepared-world API, not both.");
			var installWeather = questionAnswers.TryGetValue(WeatherQuestion, out var weatherAnswer)
                ? IsYes(weatherAnswer) : ArmageddonWeatherInstaller.DefaultAnswer(context, questionAnswers) == "yes";
            if (questionAnswers.TryGetValue(WeatherQuestion, out weatherAnswer) && !IsYes(weatherAnswer) && !weatherAnswer.Trim().Equals("no", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Answer yes or no for armageddon-weather.");
            if (installWeather) _ = ArmageddonWeatherInstaller.Require(context);
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
			var settings = ArmageddonInstallation.Prepare(factory, mode, capacity, overrides, enableStockForNewInstall: true);
			using (var module = factory())
            {
                if (ArmageddonSkillPackage.IsInstalled(module))
                {
                    var meritMessages = new List<string>();
                    ArmageddonMundaneProgression.Install(module, meritMessages);
                    ArmageddonHeritage.Install(module, meritMessages);
                    foreach (var message in meritMessages) Console.WriteLine(message);
                }
            }
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
			var gatheringFoundations = ArmageddonProgressionInstaller.InstallGatheringFoundations(factory);
			if (gatheringFoundations.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(string.Join("\n", gatheringFoundations.Messages));
			var gatheringPolicies = ArmageddonStockGatheringInstaller.Install(factory);
			if (gatheringPolicies.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(string.Join("\n", gatheringPolicies.Messages));
			var regionalLand = ArmageddonRegionalLandInstaller.Install(factory);
			if (regionalLand.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(string.Join("\n", regionalLand.Messages));
			var completeProgression = ArmageddonProgressionInstaller.Install(factory, completeStock: true, completeGathering: true);
			var completeProgressionReport = string.Join("\n", regionalLand.Messages.Concat(completeProgression.Messages));
			if (completeProgression.Status != ArmageddonInstallStatus.Completed) throw new InvalidOperationException(completeProgressionReport);
			using (var module = factory())
			{
				var templarMessages = new List<string>();
				if (ArmageddonSkillPackage.IsInstalled(module))
					ArmageddonTemplarInstaller.Install(module, templarMessages);
				else templarMessages.Add("Templar classes require the canonical Armageddon skill package; the existing magic-only installation was retained.");
				completeProgressionReport += "\n" + string.Join("\n", templarMessages);
			}
            if (installWeather)
            {
                var weather = ArmageddonWeatherInstaller.Install(factory);
                completeProgressionReport += "\n" + string.Join("\n", weather.Messages);
            }

			return $"Armageddon setting content installed. Magic progression: {settings.Mode}, classes {(settings.ClassesEnabled ? "enabled" : "disabled")}. Existing characters and builder customisations have been preserved.\n{progressionReport}\n{catalogueReport}\n{spellReport}\n{focusDeviceReport}\n{completeProgressionReport}\nMagic and gathering content installed. Availability: {(settings.StockAvailabilityByDefault ? "enabled for new installations" : "existing settings retained")}; preserved builder overrides are reported above. Builders must configure land-based gathering and grant access to existing characters.";
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

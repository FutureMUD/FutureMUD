#nullable enable

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace DatabaseSeeder.Seeders;

/// <summary>A fixed, one-shot historical world installation. Source preparation is a developer operation.</summary>
public sealed class ArmageddonWorldfileSeeder : IDatabaseSeeder
{
	internal const string PackageName = "ArmageddonWorldfile";
	internal const string CompletionKey = "installation";
	public string Name => "Armageddon Worldfile";
	public string Tagline => "The rooms, inhabitants and trade of historical Zalanthas";
	public string FullDescription => "Creates the configured historical Armageddon world, including rooms, equipment, " +
		"inhabitants, foraging, clans and permanent shops. Requires the Armageddon Setting and Animal foundations. " +
		"This package asks no questions and can be installed once. Its source report records historical defects and adaptations.";
	public int SortOrder => 310;
	public bool SafeToRunMoreThanOnce => false;
	public bool Enabled => true;
	public SeederMetadata Metadata => new(SeederRepeatabilityMode.OneShot, SeederUpdateCapability.None, [],
		RerunSummary: "One initial world installation; an installed world is never updated or overwritten.",
		DependencySeederTypes: [typeof(ArmageddonSettingSeeder), typeof(AnimalSeeder)]);
	public IEnumerable<(string Id, string Question,
		Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
		Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions => [];

	public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context) =>
		Installed(context) ? ShouldSeedResult.MayAlreadyBeInstalled :
		AssessSeedData(context).Status == SeederAssessmentStatus.Blocked
			? ShouldSeedResult.PrerequisitesNotMet : ShouldSeedResult.ReadyToInstall;

	private static bool Installed(FuturemudDatabaseContext context) =>
		context.SeederManagedRecords.AsNoTracking().Any(x => x.Seeder == PackageName);

	public SeederAssessment AssessSeedData(FuturemudDatabaseContext context)
	{
		if (Installed(context)) return new(SeederAssessmentStatus.InstalledCurrent,
			"The Armageddon world has already been installed. This one-shot package cannot run again.", [], [], []);
		if (!ArmageddonWorldfilePackage.FileAvailable)
			return new(SeederAssessmentStatus.Blocked, ArmageddonWorldfilePackage.MissingDataMessage,
				[ArmageddonWorldfilePackage.MissingDataMessage], [], []);
		var errors = ArmageddonWorldfileWriter.Preflight(context, ArmageddonWorldfilePackage.Load());
		return new(errors.Count == 0 ? SeederAssessmentStatus.ReadyToInstall : SeederAssessmentStatus.Blocked,
			errors.Count == 0 ? "The configured Armageddon world is ready to create." :
				"Install or repair the listed native foundations before creating the world.", errors, [],
			["No configuration questions. Source defects and adaptations are included in the installation report."]);
	}

	public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
	{
		if (context.ChangeTracker.HasChanges() || context.Database.CurrentTransaction is not null)
			throw new InvalidOperationException("Start the worldfile installation with a clean context and no enclosing transaction.");
		// Assessment/menu warnings are not a repeatability guard: protect direct calls too.
		if (Installed(context)) throw new InvalidOperationException("The Armageddon world is already installed; no data was changed.");
		var package = ArmageddonWorldfilePackage.Load();
		var errors = ArmageddonWorldfileWriter.Preflight(context, package);
		if (errors.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
		using var transaction = context.Database.IsRelational()
			? context.Database.BeginTransaction(IsolationLevel.Serializable) : null;
		if (Installed(context)) throw new InvalidOperationException("The Armageddon world was installed by another operation.");
		var detectChanges = context.ChangeTracker.AutoDetectChangesEnabled;
		context.ChangeTracker.AutoDetectChangesEnabled = false;
		try
		{
			var writer = new ArmageddonWorldfileWriter(context, package);
			writer.Install();
			transaction?.Commit();
			return writer.Report;
		}
		catch
		{
			transaction?.Rollback();
			context.ChangeTracker.Clear();
			throw;
		}
		finally
		{
			context.ChangeTracker.AutoDetectChangesEnabled = detectChanges;
		}
	}
}

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CultureSeeder
{
	/// <summary>
	/// Evaluates the retained procedural source in an isolated, disposable database. Only prerequisite
	/// scalar values are copied; no live culture/name/profile records enter its destructive legacy upserts.
	/// Importers must resolve identities and three-way reconcile these definitions into the destination.
	/// </summary>
	internal static FuturemudDatabaseContext BuildToolkitSource(FuturemudDatabaseContext installed, string sourcePack)
	{
		if (sourcePack is not ("earthantiquity" or "earthdarkagesandmedieval" or "earthrenaissanceeurope" or "earthrenaissanceworldexpansion"))
			throw new ArgumentException("Only the four retained historical source modules belong to the redesigned toolkits.", nameof(sourcePack));
		var stage = CreateToolkitPrerequisiteContext(installed);
		try
		{
			var seeder = new CultureSeeder { _context = stage };
			seeder.SeedSimple(stage);
			seeder.SeedCulturePacks(stage, new Dictionary<string, string>
			{
				["culturepacks"] = sourcePack, ["seednames"] = "yes", ["seedlanguages"] = "yes", ["seedheritage"] = "yes"
			});
			seeder.EnsureFallbackRandomNameProfiles();
			stage.SaveChanges();
			return stage;
		}
		catch { stage.Dispose(); throw; }
	}

	internal static FuturemudDatabaseContext CreateToolkitPrerequisiteContext(FuturemudDatabaseContext installed)
	{
		var stage = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase($"culture-source-{Guid.NewGuid():N}", new InMemoryDatabaseRoot(), x => x.EnableNullChecks(false))
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		try
		{
			var prerequisiteProgs = installed.FutureProgs.AsNoTracking()
				.Where(x => x.FunctionName == "AlwaysTrue" || x.FunctionName == "AlwaysFalse" || x.FunctionName == "SkillStartingValue");
			var progIds = prerequisiteProgs.Select(x => x.Id).ToList();
			CopyPrerequisites(installed, stage, prerequisiteProgs);
			CopyPrerequisites(installed, stage, installed.FutureProgsParameters.AsNoTracking().Where(x => progIds.Contains(x.FutureProgId)));
			CopyPrerequisites(installed, stage, installed.TraitDefinitions.AsNoTracking().Where(x => x.Type == 1 || x.Type == 3));
			var attributeExpressions = installed.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3).Select(x => x.ExpressionId).ToList();
			CopyPrerequisites(installed, stage, installed.TraitExpressions.AsNoTracking().Where(x => attributeExpressions.Contains(x.Id)));
			CopyPrerequisites(installed, stage, installed.TraitDecorators.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.Improvers.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.LanguageDifficultyModels.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.PopulationBloodModels.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.Races.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.CharacteristicDefinitions.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.CharacteristicProfiles.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.RacesAdditionalCharacteristics.AsNoTracking());
			CopyPrerequisites(installed, stage, installed.Calendars.AsNoTracking());
			stage.SaveChanges();
			return stage;
		}
		catch
		{
			stage.Dispose();
			throw;
		}
	}

	private static void CopyPrerequisites<T>(FuturemudDatabaseContext installed, FuturemudDatabaseContext stage,
		IQueryable<T> source) where T : class, new()
	{
		var properties = installed.Model.FindEntityType(typeof(T))!.GetProperties()
			.Where(x => x.PropertyInfo is not null).Select(x => x.PropertyInfo!).ToArray();
		foreach (var original in source)
		{
			var clone = new T();
			foreach (var property in properties) property.SetValue(clone, property.GetValue(original));
			stage.Set<T>().Add(clone);
		}
	}
}

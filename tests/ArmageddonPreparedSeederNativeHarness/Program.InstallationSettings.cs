#nullable enable
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PreparedInstallationSettingsAcceptance(TestDatabase database)
	{
		Func<FuturemudDatabaseContext> factory = () => NewIndependentContext(database.ConnectionString);
		var allBefore = PreparedDatabaseChecksum(database);
		var contentBefore = PreparedDatabaseChecksum(database, excludeInstallationMetadata: true);
		long capacityAttribute;
		using (var db = factory())
		{
			Require(ArmageddonInstallation.ResolveCapacityAttribute(db) == 0, "The LabMUD replay must require an explicit capacity attribute selection.");
			capacityAttribute = db.TraitDefinitions.Single(x => x.Name == "Willpower" && x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 && x.TraitGroup == "Mental").Id;
			var question = new ArmageddonSettingSeeder().Questions.Single(x => x.Id == ArmageddonSettingSeeder.CapacityQuestion);
			Require(question.Filter(db, new Dictionary<string, string> { [ArmageddonSettingSeeder.FullInstallQuestion] = "yes" }) &&
				question.Validator(capacityAttribute.ToString(System.Globalization.CultureInfo.InvariantCulture), db).Success,
				"The ordinary S1 capacity question did not accept the explicit native selection.");
		}
		try
		{
			ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Classic, capacityAttribute: capacityAttribute, checkpoint: stage =>
			{
				if (stage == ArmageddonInstallCheckpoint.BeforeCommit) throw new InvalidOperationException("S1 injected transaction interruption");
			});
			throw new InvalidOperationException("S1 rollback injection did not stop the operation.");
		}
		catch (InvalidOperationException error) when (error.Message == "S1 injected transaction interruption") { }
		Require(allBefore == PreparedDatabaseChecksum(database), "S1 settings/mirrors rollback changed table contents.");
		using (var db = factory()) Require(ArmageddonInstallation.Read(db) is null, "Rolled-back settings were visible from a fresh connection.");
		Console.WriteLine("ARMS1-rollback=passed actual-MySQL-before-commit all-table-content-unchanged");
		try
		{
			ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Simple, capacityAttribute: capacityAttribute, checkpoint: stage =>
			{
				if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("S1 injected lost acknowledgement");
			});
			throw new InvalidOperationException("S1 acknowledgement injection did not stop the caller.");
		}
		catch (InvalidOperationException error) when (error.Message == "S1 injected lost acknowledgement") { }
		long settingsId; DateTime appliedAt; int answers;
		using (var db = factory())
		{
			var settings = ArmageddonInstallation.Read(db)!;
			Require(settings.Mode == ArmageddonProgressionMode.Simple && settings.CapacityAttribute == capacityAttribute && settings.ClassesEnabled == ArmageddonInstallation.ClassesEnabled(db), "S1 durable choices were lost or changed.");
			var record = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonInstallation.SettingsKey);
			settingsId = record.Id; appliedAt = record.AppliedAt; answers = db.SeederChoices.Count();
			Require(record.LogicalId is null && db.SeederChoices.Count(x => x.Seeder == "__shared__" &&
				(x.Choice == ArmageddonInstallation.ModeAnswerKey || x.Choice == ArmageddonInstallation.ClassesAnswerKey || x.Choice == ArmageddonInstallation.BindingsAnswerKey)) == 3,
				"S1 settings and their exact three mirrors did not commit together.");
		}
		Require(contentBefore == PreparedDatabaseChecksum(database, excludeInstallationMetadata: true), "S1 changed a content/player table.");
		var committed = PreparedDatabaseChecksum(database);
		ArmageddonInstallation.Prepare(factory);
		Require(committed == PreparedDatabaseChecksum(database), "S1 rerun changed table contents.");
		try { ArmageddonInstallation.Prepare(factory, ArmageddonProgressionMode.Classic); throw new Exception("Contradictory S1 mode was accepted."); }
		catch (InvalidOperationException error) when (error.Message.Contains("fixed for this MUD", StringComparison.Ordinal)) { }
		Require(committed == PreparedDatabaseChecksum(database), "S1 contradictory mode changed table contents.");
		using (var db = factory()) Require(db.SeederManagedRecords.Single(x => x.Id == settingsId).AppliedAt == appliedAt && db.SeederChoices.Count() == answers,
			"S1 rerun changed identity/audit state.");
		Console.WriteLine($"ARMS1-settings=passed durable-Simple-mode class-source-agreement three-mirrors lost-acknowledgement rerun-no-churn contradiction-refused content-table-sha256:{contentBefore}");
	}
}

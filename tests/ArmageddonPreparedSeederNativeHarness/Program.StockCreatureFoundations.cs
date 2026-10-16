#nullable enable
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.Combat;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.NPC.Templates;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.TimeAndDate.Date;
using MudSharp.Work.Butchering;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void RunStockCreatureFoundations(TestDatabase database, Func<FuturemudDatabaseContext> factory)
	{
		var before = PreparedDatabaseChecksum(database); var faults = 0;
		var rollback = ArmageddonCatalogueCreatureFoundations.Install(factory, stage =>
		{ if (stage == ArmageddonInstallCheckpoint.BeforeCommit) { ++faults; throw new InvalidOperationException("S3 creature foundation injected precommit failure"); } });
		Require(faults == 1 && rollback.Status == ArmageddonInstallStatus.Failed && before == PreparedDatabaseChecksum(database),
			"Native creature foundation rollback failed: " + string.Join("\n", rollback.Messages));
		var ack = ArmageddonCatalogueCreatureFoundations.Install(factory, stage =>
		{ if (stage == ArmageddonInstallCheckpoint.AfterCommit) throw new InvalidOperationException("S3 creature foundation injected lost acknowledgement"); });
		Require(ack.Status == ArmageddonInstallStatus.CommittedConfirmationFailed && ack.Identities.Count == 22, string.Join("\n", ack.Messages));
		var committed = PreparedDatabaseChecksum(database); var rerun = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Require(rerun.Status == ArmageddonInstallStatus.Completed && committed == PreparedDatabaseChecksum(database) &&
			!rerun.Messages.Any(x => x.Contains("preserved builder edit")), "Native creature foundation no-op failed: " + string.Join("\n", rerun.Messages));
		RunStockAnatomyReader(database.Name);
		using (var db = factory())
		{
			var id = ack.Identities["arm.race.air_guardian"];
			db.Races.Single(x => x.Id == id).Name = "Builder Guardian Race";
			db.Races.Single(x => x.Id == id).HungerRate = 0.75;
			db.Cultures.Single(x => x.Id == ack.Identities["arm.culture.constructs"]).PersonWordNeuter = "construct";
			var merit = db.Merits.Single(x => x.Id == ack.Identities["arm.race.air_guardian.affinity"]);
			var xml = System.Xml.Linq.XElement.Parse(merit.Definition); xml.Element("ElementalAffinities")!.Add(new System.Xml.Linq.XElement("Affinity", "Water")); merit.Definition = xml.ToString();
			db.SaveChanges();
		}
		var edited = PreparedDatabaseChecksum(database); var preserved = ArmageddonCatalogueCreatureFoundations.Install(factory);
		Require(preserved.Status == ArmageddonInstallStatus.Completed && edited == PreparedDatabaseChecksum(database),
			"Native creature foundation builder preservation failed: " + string.Join("\n", preserved.Messages));
		RunStockAnatomyReader(database.Name);
		Console.WriteLine("ARMS3-creature-foundations=passed actual-MySQL seven-races seven-ethnicities culture affinities natural-attacks rollback lost-ack no-op-all-table-checksums builder-edits cold-factories; actual-NPC-spawn/combat:NOT_RUN complete-S3:NOT_QUALIFIED");
	}

	private static void ReadStockCreatureFoundations(FuturemudDatabaseContext db, Mock<IFuturemud> world)
	{
		var records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == "stock-catalogue").ToArray();
		long[] Ids(string type) => records.Where(x => x.EntityType == type).Select(x => x.LogicalId!.Value).ToArray();
		var raceIds = Ids(nameof(Db.Race)); if (raceIds.Length == 0) return;
		var traits = new All<ITraitDefinition>(); world.SetupGet(x => x.Traits).Returns(traits);
		foreach (var row in db.TraitDefinitions.Where(x => x.Type == 1 || x.Type == 3))
		{ var trait = new Mock<IAttributeDefinition>(); trait.SetupGet(x => x.Id).Returns(row.Id); traits.Add(trait.Object); }
		world.SetupGet(x => x.Gases).Returns(ReferenceRows<IGas>(db.Gases.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.Liquids).Returns(ReferenceRows<ILiquid>(db.Liquids.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.HealthStrategies).Returns(ReferenceRows<IHealthStrategy>(db.HealthStrategies.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.CorpseModels).Returns(ReferenceRows<ICorpseModel>(db.CorpseModels.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.BloodModels).Returns(ReferenceRows<IBloodModel>(db.BloodModels.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.PopulationBloodModels).Returns(ReferenceRows<IPopulationBloodModel>(db.PopulationBloodModels.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.RaceButcheryProfiles).Returns(new All<IRaceButcheryProfile>());
		world.SetupGet(x => x.HeightWeightModels).Returns(ReferenceRows<IHeightWeightModel>(db.HeightWeightModels.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.CharacterCombatSettings).Returns(ReferenceRows<ICharacterCombatSettings>(db.CharacterCombatSettings.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.NameCultures).Returns(ReferenceRows<INameCulture>(db.NameCultures.Select(x => x.Id).ToArray()));
		world.SetupGet(x => x.Calendars).Returns(ReferenceRows<ICalendar>(db.Calendars.Select(x => x.Id).ToArray()));
		var progs = ReferenceRows<IFutureProg>(db.FutureProgs.Select(x => x.Id).ToArray()); world.SetupGet(x => x.FutureProgs).Returns(progs);
		var expressions = new All<ITraitExpression>(); world.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var expressionIds = Ids(nameof(Db.TraitExpression));
		foreach (var row in db.TraitExpressions.Include(x => x.TraitExpressionParameters).Where(x => expressionIds.Contains(x.Id)))
			expressions.Add(new MudSharp.Body.Traits.TraitExpression(row, world.Object));
		var attacks = new All<IWeaponAttack>(); world.SetupGet(x => x.WeaponAttacks).Returns(attacks);
		var attackIds = Ids(nameof(Db.WeaponAttack));
		foreach (var row in db.WeaponAttacks.Where(x => attackIds.Contains(x.Id))) attacks.Add(MudSharp.Combat.WeaponAttack.LoadWeaponAttack(row, world.Object));
		var races = new All<IRace>(); world.SetupGet(x => x.Races).Returns(races);
		foreach (var row in db.Races.Include(x => x.RacesAttributes).Include(x => x.RacesBreathableGases).Include(x => x.RacesBreathableLiquids)
			.Include(x => x.RacesRemoveBreathableGases).Include(x => x.RacesRemoveBreathableLiquids)
			.Include(x => x.RacesWeaponAttacks).Where(x => raceIds.Contains(x.Id)).AsSplitQuery()) races.Add(new Race(row, world.Object));
		foreach (var race in races)
		{
			Require(race.BaseBody is not null && race.ParentRace is null && race.Attributes.Any() && race.DefaultHealthStrategy is not null,
				"Native race foundations failed to reload.");
			Require(race.NaturalWeaponAttacks.Count() == 2 && race.NaturalWeaponAttacks.All(x => x.Attack is not null && x.Bodypart is not null),
				"Native natural attack/bodypart relationships missing.");
			if (race.NeedsToBreathe) Require(race.BreathableFluids.Any(), "Native breathing links missing.");
		}
		var cultureIds = Ids(nameof(Db.Culture));
		foreach (var row in db.Cultures.Include(x => x.CulturesNameCultures).Where(x => cultureIds.Contains(x.Id)))
		{
			var culture = new Culture(row, world.Object);
			Require(culture.PrimaryCalendar is not null && culture.SkillStartingValueProg is not null &&
				Enum.GetValues<Gender>().All(g => culture.NameCultureForGender(g) is not null), "Native creature culture dependencies missing.");
		}
		var ethnicityIds = Ids(nameof(Db.Ethnicity));
		foreach (var row in db.Ethnicities.Where(x => ethnicityIds.Contains(x.Id)))
			Require(new Ethnicity(row, world.Object).ParentRace is not null, "Native creature ethnicity lost its parent race.");
		ElementalAffinityMerit.RegisterMeritInitialiser(); var meritIds = Ids(nameof(Db.Merit));
		foreach (var row in db.Merits.Where(x => meritIds.Contains(x.Id)))
			Require(MeritFactory.LoadMerit(row, world.Object) is IElementalAffinityMerit affinity && affinity.ElementalAffinities.Count != 0,
				"Native declared affinity merit failed to reload.");
		Console.WriteLine("ARMS3-creature-foundation-reader=passed native-race/culture/ethnicity/affinity-factories seven-natural-strikes typed-external-reference-registries; actual-NPC-spawn:NOT_RUN");
	}

	private static All<T> ReferenceRows<T>(IEnumerable<long> ids) where T : class, IFrameworkItem
	{
		var rows = new All<T>();
		foreach (var id in ids) { var row = new Mock<T>(); row.SetupGet(x => x.Id).Returns(id); rows.Add(row.Object); }
		return rows;
	}
}

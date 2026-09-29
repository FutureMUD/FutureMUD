#nullable enable

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Combat;
using MudSharp.Database;
using MySql.Data.MySqlClient;

try
{
	// Reconcile the real content helper only in an already-provisioned, owned smoke world.
	var connection = Environment.GetEnvironmentVariable("PREDATOR_SMOKE_CONNECTION")
		?? throw new InvalidOperationException("Missing owned smoke connection.");
	var builder = new MySqlConnectionStringBuilder(connection);
	if (args.Length != 2 || builder.Server != "127.0.0.1" || builder.Database != "authored_celestial_smoke" ||
		builder.UserID != "root" || builder.Password.Length != 0)
		throw new InvalidOperationException("Only the owned smoke instance is allowed.");
	using (var sql = new MySqlConnection(connection))
	{
		sql.Open();
		using var identity = new MySqlCommand("SELECT @@datadir", sql);
		if (!Path.GetFullPath((string)identity.ExecuteScalar()!).TrimEnd(Path.DirectorySeparatorChar)
			.Equals(Path.GetFullPath(args[0]).TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase))
			throw new InvalidOperationException("Owned data directory mismatch.");
	}
	var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseMySql(connection, ServerVersion.AutoDetect(connection)).Options;
	using var db = new FuturemudDatabaseContext(options);
	var ensure = typeof(AnimalSeeder).Assembly.GetType("DatabaseSeeder.Seeders.PredatorCombatSeederHelper")!
		.GetMethod("Ensure", BindingFlags.Public | BindingFlags.Static)!;
	ensure.Invoke(null, [db]);
	string Snapshot() => JsonSerializer.Serialize(new
	{
		Attacks = db.WeaponAttacks.Where(x => x.Name.StartsWith("Wildlife - ")).OrderBy(x => x.Id)
			.Select(x => new { x.Id, x.Name, x.AdditionalInfo, x.MoveType, x.StaminaCost, x.RequiredPositionStateIds }).ToList(),
		Links = db.RacesWeaponAttacks.OrderBy(x => x.RaceId).ThenBy(x => x.WeaponAttackId).ThenBy(x => x.BodypartId)
			.Select(x => new { x.RaceId, x.WeaponAttackId, x.BodypartId }).ToList(),
		Corpses = db.CorpseModels.OrderBy(x => x.Id).Select(x => new { x.Id, x.Definition }).ToList(),
		Breathing = db.RacesBreathableGases.OrderBy(x => x.RaceId).ThenBy(x => x.GasId)
			.Select(x => new { x.RaceId, x.GasId, x.Multiplier }).ToList(),
		Strategies = db.CharacterCombatSettings.Where(x => x.Name == "Beast Dropper" || x.Name == "Beast Drowner")
			.OrderBy(x => x.Id).Select(x => new { x.Id, x.PreferredMeleeMode, x.PreferredRangedMode }).ToList()
	});
	var first = Snapshot();
	ensure.Invoke(null, [db]);
	if (first != Snapshot()) throw new InvalidOperationException("Content reconciliation changed identities or definitions on the second run.");
	var corpses = db.CorpseModels.Where(x => x.Name == "Organic Animal Corpse" || x.Name == "Organic Human Corpse").ToList();
	foreach (var corpse in corpses)
	{
		var materials = XElement.Parse(corpse.Definition).Element("CorpseMaterials");
		if (materials?.Elements().Count() != 6) throw new InvalidOperationException("Missing stock organic corpse material states.");
	}
	if (!db.RacesBreathableGases.Any(x => x.Race.Name == "Viper" && x.Gas.Name == "Breathable Atmosphere"))
		throw new InvalidOperationException("Stock Viper cannot breathe the stock atmosphere.");
	foreach (var type in new[] { BuiltInCombatMoveType.EnvenomingAttack, BuiltInCombatMoveType.EnvenomingAttackClinch })
	{
		if (!db.RacesWeaponAttacks.Where(x => x.Race.Name == "Viper" && x.WeaponAttack.MoveType == (int)type)
			.Select(x => x.WeaponAttack.RequiredPositionStateIds).ToList().Any(x => x.Split(' ').Contains("6")))
			throw new InvalidOperationException($"Stock Viper has no prone {type} attack.");
	}
	foreach (var (name, mode) in new[] { ("Beast Dropper", CombatStrategyMode.Dropper), ("Beast Drowner", CombatStrategyMode.Drowner) })
	{
		if (!db.CharacterCombatSettings.Any(x => x.Name == name && x.PreferredMeleeMode == (int)mode && x.PreferredRangedMode == (int)mode))
			throw new InvalidOperationException($"Stock {name} has no predator approach strategy.");
	}
	File.WriteAllText(args[1], JsonSerializer.Serialize(new { Status = "PASS", StableRerun = true,
		CanonicalAir = true, ProneVenomAttacks = true, PredatorApproachStrategies = true,
		Corpses = corpses.Select(x => new { x.Id, x.Name }) }, new JsonSerializerOptions { WriteIndented = true }));
	Console.WriteLine("PASS: predator content reconciliation and stable native rerun.");
}
catch (Exception error)
{
	Console.Error.WriteLine(error);
	Environment.ExitCode = 1;
}

#nullable enable
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DatabaseSeeder.Seeders;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
namespace DatabaseSeeder;
internal sealed class ArmageddonFoundationIntegration : ISettingSeederIntegration
{
	[ModuleInitializer]
	internal static void Register() => SettingSeederIntegration.Profile = new ArmageddonFoundationIntegration();
	public SeederMetadata AdjustMetadata(IDatabaseSeeder seeder, SeederMetadata metadata) => seeder is EnvironmentalExposureSeeder
		? metadata with { OrderAfterSeederTypes = (metadata.OrderAfterSeederTypes ?? []).Append(typeof(ArmageddonSettingSeeder)).Distinct().ToArray() }
		: metadata;
	public bool IsInstalled(FuturemudDatabaseContext db) => ArmageddonSkillPackage.IsInstalled(db);
	public IReadOnlyDictionary<string, TraitDefinition> Resolve(FuturemudDatabaseContext db) => ArmageddonSkillPackage.Resolve(db);
	public IReadOnlyDictionary<string, TraitDefinition> EngineBindings(FuturemudDatabaseContext db) => ArmageddonSkillPackage.EngineBindings(db);
	public string SeedSkills(SkillPackageSeeder services, FuturemudDatabaseContext db, IReadOnlyDictionary<string, string> answers) => ArmageddonSkillSeeder.Seed(services, db, answers);
	public string? Formula(CheckType type, IReadOnlyDictionary<string, TraitDefinition> skills) => ArmageddonMundaneBindings.Formula(type, skills);
	public void EnsureCheck(FuturemudDatabaseContext db, CheckType type, string formula, long template, Difficulty maximum, ICollection<string> messages, bool rejectUnowned = false) => ArmageddonMundaneBindings.EnsureCheck(db, type, formula, template, maximum, messages, rejectUnowned);
	public void ValidateCombat(FuturemudDatabaseContext db, IReadOnlyDictionary<string, string> answers) => ArmageddonCombatFoundation.ValidateArmageddonCombat(db, answers);
	public void ValidateCombatIdentities(FuturemudDatabaseContext db) => ArmageddonCombatFoundation.ValidateArmageddonCombatIdentities(db);
	public void CompleteCombat(FuturemudDatabaseContext db) => ArmageddonCombatFoundation.CompleteArmageddonCombat(db);
	public bool HasFarmingFoundation(FuturemudDatabaseContext db) => ArmageddonAgricultureFoundation.HasFarmingFoundation(db);
	public TraitDefinition? ResolveFarmingTrait(FuturemudDatabaseContext db) => ArmageddonAgricultureFoundation.ResolveFarmingTrait(db);
	public TraitDefinition EnsureFarmingTrait(FuturemudDatabaseContext db) => ArmageddonAgricultureFoundation.EnsureFarmingTrait(db);
}

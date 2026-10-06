#nullable enable
namespace DatabaseSeeder.Seeders;

/// <summary>Explicit existing native dependencies; no skill, school or player identity is invented.</summary>
public sealed record ArmageddonPierceInstallPlan(bool Install, long School, long Resource, long AlwaysFalseProg, long PierceSkill);

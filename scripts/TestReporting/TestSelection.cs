namespace TestReporting;

internal static class TestSelection
{
	internal const string DefaultFilter = "TestCategory!=Integration&TestCategory!=Benchmark&TestCategory!=ClimateSeeder&TestCategory!=PsychicBenchmark";

	internal static string? EffectiveFilter(string? requested, bool includeExtended) => includeExtended
		? requested
		: requested is null ? DefaultFilter : $"({DefaultFilter})&({requested})";
}

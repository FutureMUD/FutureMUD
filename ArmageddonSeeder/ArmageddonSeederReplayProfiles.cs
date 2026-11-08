#if DEBUG
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
namespace DatabaseSeeder;
internal static class ArmageddonSeederReplayProfiles
{
	internal static ProductReplayRunner Runner { get; } = new([typeof(SkillSeeder)], context =>
		context.Accounts.Any() || context.SeederChoices.Any() ? "Armageddon replay requires a freshly migrated, unseeded database. No data was changed." : null);
	internal static IReadOnlyList<SeederReplayProfile> All { get; } = [Create()];
	internal static IReadOnlyList<SeederReplayProfile> PreparedWorldProfiles { get; } = DebugSeederReplayProfiles.All.Select(x => AddProductSteps(x, false)).ToArray();
	private static SeederReplayProfile Create()
	{
		var foundation = DebugSeederReplayProfiles.CreateProfile("armageddon-native", "Armageddon Native World",
			"Armageddon foundations, setting and one-shot worldfile installation.", "Debug Armageddon", "1", "Medieval Age", "medieval", "earthmedievaleurope", armageddon: true);
		return AddProductSteps(foundation, true);
	}
	private static SeederReplayProfile AddProductSteps(SeederReplayProfile foundation, bool install)
	{
		var steps = foundation.Steps.Concat(new[]
		{
			new SeederReplayStep(typeof(ArmageddonSettingSeeder), new[]
			{
				new SeederReplayAnswer(ArmageddonSettingSeeder.InstallQuestion, "no"),
				new SeederReplayAnswer(ArmageddonSettingSeeder.FullInstallQuestion, install ? "yes" : "no"),
				new SeederReplayAnswer(ArmageddonSettingSeeder.WeatherQuestion, "no"),
				new SeederReplayAnswer(ArmageddonSettingSeeder.ModeQuestion, "Classic"),
				new SeederReplayAnswer(ArmageddonSettingSeeder.CapacityQuestion, install ? "4" : "0"),
				new SeederReplayAnswer(ArmageddonSettingSeeder.NativeBindingsQuestion, "{}"),
				new SeederReplayAnswer(ArmageddonSettingSeeder.BindingsQuestion, "none")
			}),
			new SeederReplayStep(typeof(ArmageddonWorldfileSeeder), Array.Empty<SeederReplayAnswer>(), install ? SeederReplayDisposition.Execute : SeederReplayDisposition.NotApplicable, install ? null : "This prepared-world fixture declines the standalone setting.", install ? null : db => ArmageddonInstallation.Read(db) is null)
		});
		var plan = SeederPackagePlan.GetDependencyPlan(ArmageddonSeederProgram.GetEnabledSeeders());
		if (plan.Errors.Count > 0) throw new InvalidOperationException(string.Join("; ", plan.Errors));
		var order = plan.OrderedSeeders.Select((item, index) => (item, index)).ToDictionary(x => x.item.GetType(), x => x.index);
		return foundation with { Steps = steps.OrderBy(x => order[x.SeederType]).ToArray() };
	}
}
#endif

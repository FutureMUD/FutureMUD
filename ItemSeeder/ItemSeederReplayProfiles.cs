#if DEBUG
#nullable enable

using DatabaseSeeder.Seeders;
using MudSharp.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

internal static class ItemSeederReplayProfiles
{
	internal static ProductReplayRunner Runner { get; } = new([], context => context.Accounts.Any()
		? null
		: "Item replay requires a prepared local world. Run Database Seeder first.");

	internal static SeederReplayRunResult Run(SeederReplayProfile profile, IEnumerable<IDatabaseSeeder> seeders,
		Func<FuturemudDatabaseContext> contextFactory, Version version, Action<string>? progress = null) =>
		Runner.Run(profile, seeders, contextFactory, version, progress);

	internal static IReadOnlyList<SeederReplayProfile> All { get; } =
	[
		Create("medieval-standard", "Medieval Standard", "medieval"),
		Create("renaissance-standard", "Renaissance Standard", "medieval renaissance"),
		Create("early-modern-standard", "Early Modern Standard", "medieval renaissance earlymodern"),
		Create("industrial-neutral", "Industrial Neutral", "medieval renaissance earlymodern industrial"),
		Create("industrial-custom", "Industrial Custom", "medieval renaissance earlymodern industrial", "custom"),
		Create("armageddon-native", "Armageddon Native World", "medieval", armageddon: true)
	];

	private static SeederReplayProfile Create(string id, string name, string itemEras,
		string technologyProfile = "neutral", bool armageddon = false)
	{
		var items = new SeederReplayStep(typeof(ItemSeeder), [
			new("eras", itemEras), new("technologyprofile", technologyProfile),
			new("technologypower", "Connectable_Male_To_MainsPlug"), new("technologypaper", "A4, Letter"),
			new("technologytelecom", "Telephone_Standard"), new("technologynetworkmedia", "NetworkAdapter_Wired"),
			new("technologyvehicle", "AttachableConnectable_PowerLead"), new("scope", "all")]);
		if (armageddon)
		{
			items = items with
			{
				Disposition = SeederReplayDisposition.NotApplicable,
				NotApplicableReason = "The Armageddon skill package uses its worldfile inventory; the generic era recipe catalogue requires additional skills such as Pottery.",
				NotApplicableWhen = db => db.SeederChoices.Any(x => x.Seeder == "Skill Package" && x.Choice == "skillpreset" && x.Answer == "Armageddon")
			};
		}
		return new(id, name, armageddon ? items.NotApplicableReason! : $"Apply {itemEras} item stock using the {technologyProfile} technology profile.", [items]);
	}
}
#endif

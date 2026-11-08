#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DatabaseSeeder;

internal static class SeederCatalogue
{
	internal static IReadOnlyList<IDatabaseSeeder> GetEnabledSeeders()
	{
		return SeederPackageDiscovery.GetEnabledSeeders(typeof(Seeders.CoreDataSeeder).Assembly);
	}

	internal static SeederDependencyPlan GetDependencyPlan(IEnumerable<IDatabaseSeeder> seeders)
	{
		return SeederMetadataRegistry.GetDependencyPlan(seeders);
	}
}

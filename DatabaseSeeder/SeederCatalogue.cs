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
		return Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(x => !x.IsAbstract && typeof(IDatabaseSeeder).IsAssignableFrom(x))
			.Select(Activator.CreateInstance)
			.OfType<IDatabaseSeeder>()
			.Where(x => x.Enabled)
			.ToList();
	}

	internal static SeederDependencyPlan GetDependencyPlan(IEnumerable<IDatabaseSeeder> seeders)
	{
		return SeederMetadataRegistry.GetDependencyPlan(seeders);
	}
}

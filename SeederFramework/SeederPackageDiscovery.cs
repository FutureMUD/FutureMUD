#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DatabaseSeeder;

internal static class SeederPackageDiscovery
{
	internal static IReadOnlyList<IDatabaseSeeder> GetEnabledSeeders(Assembly assembly)
	{
		return assembly
			.GetTypes()
			.Where(x => !x.IsAbstract && typeof(IDatabaseSeeder).IsAssignableFrom(x))
			.Select(Activator.CreateInstance)
			.OfType<IDatabaseSeeder>()
			.Where(x => x.Enabled)
			.ToList();
	}

}

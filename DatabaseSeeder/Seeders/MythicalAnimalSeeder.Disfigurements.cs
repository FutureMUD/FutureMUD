#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	private static bool HasMissingMythicalDisfigurementTemplates(FuturemudDatabaseContext context)
	{
		return Templates.Values.Any(template =>
			SeederDisfigurementTemplateUtilities.HasMissingDefinitions(
				context,
				template.TattooTemplates,
				template.ScarTemplates));
	}

	private void SeedMythicalDisfigurementTemplates(
		IReadOnlyDictionary<string, BodyProto> bodyLookup,
		IEnumerable<MythicalRaceTemplate>? templates = null)
	{
		foreach (var template in templates ?? Templates.Values)
		{
			if (!HasDisfigurementDefinitions(template))
			{
				continue;
			}

			if (!bodyLookup.TryGetValue(template.BodyKey, out var body))
			{
				throw new InvalidOperationException(
					$"Could not resolve body key {template.BodyKey} while seeding disfigurements for mythical race {template.Name}.");
			}

			SeederDisfigurementTemplateUtilities.SeedTemplates(
				_context,
				body,
				template.TattooTemplates,
				template.ScarTemplates);
		}
	}

	private static bool HasDisfigurementDefinitions(MythicalRaceTemplate template)
	{
		return template.TattooTemplates?.Any() == true || template.ScarTemplates?.Any() == true;
	}
}

#nullable enable

using System.Collections.Generic;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
	private static readonly IReadOnlyList<SeederTattooTemplateDefinition> HumanTattooTemplates = [];
	private static readonly IReadOnlyList<SeederScarTemplateDefinition> HumanScarTemplates = [];

	internal static IReadOnlyList<SeederTattooTemplateDefinition> TattooTemplatesForTesting => HumanTattooTemplates;
	internal static IReadOnlyList<SeederScarTemplateDefinition> ScarTemplatesForTesting => HumanScarTemplates;

	private void SeedHumanDisfigurementTemplates(BodyProto body)
	{
		SeederDisfigurementTemplateUtilities.SeedTemplates(
			_context,
			body,
			HumanTattooTemplates,
			HumanScarTemplates);
	}

	private static bool HasMissingHumanDisfigurementTemplates(FuturemudDatabaseContext context)
	{
		return SeederDisfigurementTemplateUtilities.HasMissingDefinitions(
			context,
			HumanTattooTemplates,
			HumanScarTemplates);
	}
}

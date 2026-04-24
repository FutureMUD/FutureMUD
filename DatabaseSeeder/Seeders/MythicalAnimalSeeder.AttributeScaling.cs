#nullable enable

using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	private static int GetMythicalAttributeBonus(TraitDefinition attribute, MythicalRaceTemplate template)
	{
		return NonHumanAttributeScalingHelper.GetAttributeBonus(attribute, template.AttributeProfile);
	}
}

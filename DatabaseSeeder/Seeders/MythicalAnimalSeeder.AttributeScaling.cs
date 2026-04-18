#nullable enable

using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	private FutureProg CreateMythicalAttributeBonusProg(MythicalRaceTemplate template)
	{
		var progText = NonHumanAttributeScalingHelper.BuildAttributeBonusProgText(
			_context.TraitDefinitions.Where(x => x.Type == (int)TraitType.Attribute),
			template.AttributeProfile);

		var attributeBonusProg = new FutureProg
		{
			FunctionName = $"{template.Name.CollapseString()}AttributeBonus",
			FunctionComment = $"Racial attribute bonuses for the {template.Name} race",
			AcceptsAnyParameters = false,
			Category = "Character",
			Subcategory = "Attributes",
			ReturnType = (long)ProgVariableTypes.Number,
			FunctionText = progText
		};

		attributeBonusProg.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = attributeBonusProg,
			ParameterIndex = 0,
			ParameterName = "trait",
			ParameterType = (long)ProgVariableTypes.Trait
		});

		_context.FutureProgs.Add(attributeBonusProg);
		return attributeBonusProg;
	}
}

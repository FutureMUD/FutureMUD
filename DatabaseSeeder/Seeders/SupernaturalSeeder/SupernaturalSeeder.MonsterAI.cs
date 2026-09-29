#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class SupernaturalSeeder
{
	internal static IEnumerable<MonsterAIRecommendation> MonsterRecommendations => Templates.Values.Select(template =>
	{
		string[] profiles = template.Name switch
		{
			"Zombie" or "Skeleton" or "Mummy" => [MonsterAIStockTemplates.RelentlessPursuer],
			"Hellhound" or "Ghoul" => [MonsterAIStockTemplates.NightStalker],
			"Vampire" => [MonsterAIStockTemplates.NightStalker, MonsterAIStockTemplates.ArmedGuardian],
			"Lich" => [MonsterAIStockTemplates.PoweredGuardian, MonsterAIStockTemplates.ArmedGuardian],
			"Ghost" or "Wraith" => [MonsterAIStockTemplates.BoundHaunt],
			_ => template.Family switch
			{
				SupernaturalFamily.Spirit => [MonsterAIStockTemplates.BoundHaunt],
				SupernaturalFamily.Therianthrope => [MonsterAIStockTemplates.ConditionalHunter],
				SupernaturalFamily.Demon => [MonsterAIStockTemplates.ConditionalHunter, MonsterAIStockTemplates.PoweredGuardian],
				_ => [MonsterAIStockTemplates.LairGuardian, MonsterAIStockTemplates.PoweredGuardian]
			}
		};
		var limitations = new List<string> { "Supernatural natural-attack names are not evidence of attached magic powers." };
		if (template.Name == "Vampire") limitations.Add("No blood drinking economy or sunlight behaviour is supplied.");
		if (template.Name == "Lich") limitations.Add("No spellbook management, phylactery or automatic revival is supplied.");
		if (template.Family == SupernaturalFamily.Therianthrope) limitations.Add("No lunar condition or transformation is imposed. Form merits and hunting conditions remain separate.");
		if (template.PlanarProfile != SupernaturalPlanarProfile.Material) limitations.Add("Normal planar/corporeality targeting and movement restrictions remain authoritative.");
		if (template.Name == "Elemental Spirit") limitations.Add("No distinct elemental spell loadout is supplied.");
		return MonsterAIStockTemplates.Recommendation("Supernatural", template.Name,
			template.Family is SupernaturalFamily.Angel or SupernaturalFamily.Divine || template.Name is "Imp" or "Familiar"
				? "Builder-authored sapient role" : "Optional Monster encounter", profiles,
			"Review the race's existing movement, body and planar profile", template.CanUseWeapons,
			template.Attacks.Select(x => x.AttackName), limitations.ToArray());
	});
}

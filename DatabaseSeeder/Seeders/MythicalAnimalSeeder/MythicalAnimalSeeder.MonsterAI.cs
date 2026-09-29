#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
	internal static IEnumerable<MonsterAIRecommendation> MonsterRecommendations => Templates.Values.Select(template =>
	{
		string[] profiles = template.Name switch
		{
			"Dragon" or "Eastern Dragon" or "Dire-Bear" or "Huorn" => [MonsterAIStockTemplates.LairGuardian],
			"Warg" or "Dire-Wolf" or "Manticore" => [MonsterAIStockTemplates.NightStalker],
			"Griffin" or "Garuda" or "Giant Eagle" or "Wyvern" or "Fell Beast" => [MonsterAIStockTemplates.AerialHunter],
			"Bunyip" or "Yacumama" => [MonsterAIStockTemplates.AquaticAmbusher],
			"Giant Spider" => [MonsterAIStockTemplates.TrapAmbusher],
			"Giant Scorpion" or "Giant Centipede" => [MonsterAIStockTemplates.VenomAmbusher],
			"Ankheg" or "Giant Worm" or "Colossal Worm" => [MonsterAIStockTemplates.BurrowAmbusher],
			"Minotaur" or "Naga" or "Centaur" or "Ent" => [MonsterAIStockTemplates.ArmedGuardian],
			_ => []
		};
		var limitations = new List<string>();
		if (template.Name is "Phoenix") limitations.Add("No resurrection or automatic fire powers are implemented by this AI.");
		if (template.Name is "Basilisk" or "Cockatrice") limitations.Add("No petrification is supplied; these templates use their existing natural attacks.");
		if (template.Name is "Dragon" or "Eastern Dragon") limitations.Add("Existing breath attacks remain native combat attacks; no hoard accumulation or theft detection is supplied.");
		if (!template.WildlifeEligible) limitations.Add("Sapient NPC motivations and social roles remain builder-authored; this recommendation does not impose hostility.");
		if (profiles.Length == 0) limitations.Add("Retain the current Animal/Wildlife or sapient role choice; mythical origin alone is not a reason for proactive aggression.");
		var movement = profiles.Contains(MonsterAIStockTemplates.AerialHunter) ? "Fly" :
			profiles.Contains(MonsterAIStockTemplates.AquaticAmbusher) ? "Amphibious" : "Review the race's existing movement and habitat";
		return MonsterAIStockTemplates.Recommendation("Mythical", template.Name,
			!template.WildlifeEligible ? "Builder-authored sapient role" : profiles.Length == 0 ? "Retain Animal/wildlife" : "Optional Monster encounter",
			profiles, movement, template.CanUseWeapons, template.Attacks.Select(x => x.AttackName), limitations.ToArray());
	});
}

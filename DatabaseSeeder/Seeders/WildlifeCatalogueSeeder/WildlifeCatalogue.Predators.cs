#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

internal static partial class WildlifeCatalogue
{
	internal static string? PredatorProfileFor(string race) => race switch
	{
		"Leopard" or "Panther" => "Wildlife - Tree Ambush Hunter",
		"Jaguar" or "Tiger" => "Wildlife - Felid Ambush Hunter",
		"Crocodile" => "Wildlife - Crocodile Ambush Hunter",
		"Alligator" => "Wildlife - Alligator Ambush Hunter",
		"Caiman" => "Wildlife - Caiman Ambush Hunter",
		"Bunyip" or "Yacumama" => "Wildlife - Mythic Water Ambush Hunter",
		"Eagle" => "Wildlife - Aerial Drop Hunter",
		"Griffin" or "Garuda" or "Giant Eagle" or "Wyvern" or "Fell Beast" => "Wildlife - Mythic Aerial Drop Hunter",
		"Spider" => "Wildlife - Venom Web Hunter",
		"Tarantula" => "Wildlife - Web Hunter",
		"Scorpion" => "Wildlife - Venom Burrow Hunter",
		"Adder" or "Cobra" or "Coral Snake" or "Mamba" or "Moccasin" or "Rattlesnake" or "Viper" => "Wildlife - Venom Skirmisher",
		"Giant Spider" => "Wildlife - Mythic Venom Web Hunter",
		"Giant Scorpion" or "Giant Centipede" => "Wildlife - Mythic Venom Burrow Hunter",
		"Ankheg" or "Giant Worm" or "Colossal Worm" => "Wildlife - Mythic Burrow Hunter",
		_ => null
	};

	private static IReadOnlyList<WildlifeAnimalProfile> BuildIndividualProfiles()
	{
		var profiles = BuildBaseIndividualProfiles().ToList();
		var bases = profiles.ToDictionary(x => x.Name);
		void Add(string species, string basis, string opening, string followup, string people, string assessment,
			string? layer = null, string? shelter = null)
		{
			var profile = bases[basis];
			profiles.Add(profile with
			{
				Name = PredatorProfileFor(species)!,
				Description = $"A {opening} predator using {followup}; people prey {people}, {assessment} observable risk assessment.",
				HuntOpening = opening, HuntFollowup = followup, PeoplePrey = people,
				HuntAssessment = assessment, HuntLayer = layer,
				Senses = opening == "Direct" ? "Vigilant" : "Hiding", WanderChance = 0.10,
				ShelterKey = shelter ?? profile.ShelterKey,
				OrdinaryResponse = "Ignore", AttackedResponse = "Attack",
				EngageEmote = "@ focus|focuses on $1 and move|moves to attack."
			});
		}
		Add("Leopard", ArborealPredator, "Ambush", "Extract", "Desperate", "Balanced", "InTrees");
		Add("Jaguar", GroundStalkingPredator, "Ambush", "Fight", "Desperate", "Balanced", "GroundLevel");
		Add("Crocodile", RiverinePredator, "Ambush", "Extract", "Eligible", "Balanced", "Underwater");
		Add("Alligator", RiverinePredator, "Ambush", "Extract", "Desperate", "Balanced", "Underwater");
		Add("Caiman", RiverinePredator, "Ambush", "Extract", "Never", "Balanced", "Underwater");
		Add("Bunyip", RiverinePredator, "Ambush", "Extract", "Eligible", "Bold", "Underwater");
		Add("Eagle", Raptor, "Direct", "Fight", "Never", "Cautious");
		Add("Griffin", Raptor, "Direct", "Fight", "Eligible", "Bold");
		Add("Spider", AmbushPredator, "TrapWait", "VenomWithdrawal", "Never", "Cautious", shelter: "WebNest");
		Add("Tarantula", AmbushPredator, "TrapWait", "Fight", "Never", "Cautious", shelter: "WebNest");
		Add("Scorpion", AmbushPredator, "TrapWait", "VenomWithdrawal", "Never", "Cautious", shelter: "Burrow");
		Add("Adder", GroundStalkingPredator, "Ambush", "VenomWithdrawal", "Never", "Cautious", "GroundLevel");
		Add("Giant Spider", AmbushPredator, "TrapWait", "VenomWithdrawal", "Eligible", "Bold", shelter: "WebNest");
		Add("Giant Scorpion", AmbushPredator, "TrapWait", "VenomWithdrawal", "Eligible", "Bold", shelter: "Burrow");
		Add("Ankheg", AmbushPredator, "TrapWait", "Fight", "Eligible", "Bold", shelter: "Burrow");
		return profiles;
	}
}

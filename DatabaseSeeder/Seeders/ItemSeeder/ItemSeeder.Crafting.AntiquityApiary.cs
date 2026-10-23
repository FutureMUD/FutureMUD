using MudSharp.RPG.Checks;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private const string AncientApicultureKnowledge = "Ancient Apiculture";

	private static readonly string[] AntiquityApiaryStableReferences =
	[
		"antiquity_wicker_beehive",
		"antiquity_clay_tube_hive",
		"antiquity_wooden_hive_stand",
		"antiquity_bee_smoke_pot",
		"antiquity_honey_knife",
		"antiquity_honey_press",
		"antiquity_honey_strainer"
	];
}

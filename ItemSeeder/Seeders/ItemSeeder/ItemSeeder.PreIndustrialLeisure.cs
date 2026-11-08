#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedSharedPreIndustrialLeisureItems()
	{
		var tags = new[] { "Era / Pre-Industrial Era", "Market / Household Goods / Standard Wares" };
		SeedStraightforwardEraCatalogueItems(
			"Shared pre-industrial leisure catalogue",
			[
				new("preindustrial_leisure_bone_dice_pair", "dice", "a pair of bone dice", "Two small bone cubes are marked with dark pips for games, wagers, and casual table play.", SizeCategory.Small, ItemQuality.Standard, 30.0, 2.0m, "bone", tags, ["Holdable", "Destroyable_Misc", "Dice_d6"], "Shared pre-industrial dice using the existing die component."),
				new("preindustrial_leisure_dice_cup", "cup", "a leather dice cup", "A stiff leather cup has a weighted base and rolled rim for shaking dice before a throw.", SizeCategory.Small, ItemQuality.Standard, 140.0, 4.0m, "leather", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial dice accessory."),
				new("preindustrial_leisure_nine_mens_morris_board", "board", "a nine men's morris board", "A square wooden board is marked with nested lines and intersections for a traditional counter game.", SizeCategory.Normal, ItemQuality.Standard, 1400.0, 12.0m, "oak", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial board-game surface; game resolution remains social or builder-led."),
				new("preindustrial_leisure_draughts_board", "board", "a chequered draughts board", "A flat wooden board is divided into alternating dark and light squares for counters and simple positional play.", SizeCategory.Normal, ItemQuality.Standard, 1300.0, 12.0m, "oak", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial board-game surface; game resolution remains social or builder-led."),
				new("preindustrial_leisure_chess_board", "board", "a carved chess board", "A folding wooden board has a chequered face and shallow interior for a set of carved playing pieces.", SizeCategory.Normal, ItemQuality.Good, 1900.0, 28.0m, "oak", tags, ["Holdable", "Destroyable_Misc", "Container_PreIndustrial_CompartmentBox"], "Shared pre-industrial strategy-game board; game resolution remains social or builder-led."),
				new("preindustrial_leisure_backgammon_board", "board", "an inlaid race-game board", "A folding board is inlaid with pointed tracks and a divided interior for dice and race counters.", SizeCategory.Normal, ItemQuality.Good, 1800.0, 26.0m, "oak", tags, ["Holdable", "Destroyable_Misc", "Container_PreIndustrial_CompartmentBox"], "Shared pre-industrial race-game board; game resolution remains social or builder-led."),
				new("preindustrial_leisure_wooden_counters", "counters", "a pouch of wooden game counters", "A drawstring leather pouch holds smooth wooden counters for boards, races, tallies, and simple games.", SizeCategory.Small, ItemQuality.Standard, 260.0, 5.0m, "wood", tags, ["Holdable", "Destroyable_Misc", "Container_Pouch"], "Shared pre-industrial game counters."),
				new("preindustrial_leisure_spinning_top", "top", "a painted spinning top", "A small painted wooden top has a pointed iron tip and a wrapped cord for quick spinning games.", SizeCategory.Small, ItemQuality.Standard, 120.0, 3.0m, "wood", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial toy."),
				new("preindustrial_leisure_hoop", "hoop", "a willow play hoop", "A light willow hoop is bound into a smooth circle for rolling races and outdoor play.", SizeCategory.Normal, ItemQuality.Standard, 320.0, 4.0m, "willow", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial toy."),
				new("preindustrial_leisure_rag_doll", "doll", "a cloth rag doll", "A simple stuffed cloth doll has stitched features and a tied yarn head for a child's toy or comfort object.", SizeCategory.Small, ItemQuality.Standard, 180.0, 3.0m, "linen", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial toy."),
				new("preindustrial_leisure_carved_whistle", "whistle", "a carved wooden whistle", "A small carved wooden whistle gives a clear simple note when blown through its narrow mouthpiece.", SizeCategory.Small, ItemQuality.Standard, 80.0, 2.0m, "wood", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial toy and signal prop; no command or signal mechanic is implied."),
				new("preindustrial_leisure_pull_horse", "horse", "a carved pull horse", "A small carved wooden horse has four simple wheels and a cord for a child to pull across a floor or yard.", SizeCategory.Small, ItemQuality.Standard, 460.0, 6.0m, "oak", tags, ["Holdable", "Destroyable_Misc"], "Shared pre-industrial wheeled toy.")
			]);
	}
}

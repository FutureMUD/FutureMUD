#nullable enable

using System.Collections.Generic;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ProductionPlanStock
{
	public static IReadOnlyList<CatalogueToolProvider> RenaissanceToolProviders001() =>
	[
		new("Hammer", "historic_workshop_hammer", "Functions / Tools / Striking Tools / Hammer"),
		new("Saw", "preindustrial_tool_hand_saw", "Functions / Tools / Woodcrafting Tools / Saws / Hand Saw"),
		new("Sewing Needle", "historic_sewing_needle", "Functions / Tools / Textilecraft Tools / Sewing Needle"),
		new("Shears", "historic_textile_shears", "Functions / Separation / Shearing / Shears"),
		new("Awl Punch", "historic_awl_punch", "Functions / Tools / Leatherworking Tools / Awl Punch"),
		new("Anvil", "historic_workshop_anvil", "Functions / Tools / Metalworking Tools / Anvil"),
		new("Forge Tongs", "historic_forge_tongs", "Functions / Tools / Metalworking Tools / Forge Tongs"),
		new("Hot Fire", "historic_lit_workshop_hearth", "Functions / Material Functions / Hot Fire"),
	];
}

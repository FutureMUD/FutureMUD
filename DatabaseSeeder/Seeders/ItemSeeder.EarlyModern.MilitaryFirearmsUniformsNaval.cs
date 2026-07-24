#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedEarlyModernMilitaryFirearmsUniformsAndNaval()
	{
		const string eraTag = "Era / Early Modern Era";
		const string militaryTag = "Functions / Military Equipment";
		const string weaponTag = "Functions / Military Equipment / Military Weapons";
		const string toolMarketTag = "Market / Professional Tools / Standard Tools";
		const string spanningRoot = "Functions / Military Equipment / Crossbow Spanning Tools";

		foreach (var (reference, noun, shortDescription, fullDescription, weight, cost, material, toolTag,
			         destroyableComponent) in
		         new[]
		         {
			         ("earlymodern_military_tool_cranequin", "cranequin", "a steel cranequin",
				         "This compact rack-and-pinion cranequin hooks over a crossbow stock and turns through a geared handle to draw an exceptionally heavy prod.",
				         2900.0, 190.0m, "steel", "Cranequin", "Destroyable_HeavyMetal"),
			         ("earlymodern_military_tool_goats_foot", "lever", "an iron goat's-foot lever",
				         "This hinged iron goat's-foot lever braces against a crossbow stock and uses its hooked jaws to draw the string with one strong motion.",
				         1450.0, 80.0m, "wrought iron", "Goat's Foot", "Destroyable_HeavyMetal"),
			         ("earlymodern_military_tool_spanning_lever", "lever", "a wooden spanning lever",
				         "This stout wooden spanning lever has an iron hook and reinforced fulcrum, sized to draw a crossbow by controlled leverage.",
				         1800.0, 48.0m, "oak", "Lever", "Destroyable_WoodenHeavy"),
			         ("earlymodern_military_tool_spanning_hook", "hook", "an iron spanning hook",
				         "This belt-mounted iron spanning hook catches a crossbow string while the user straightens against the stock to draw it.",
				         620.0, 32.0m, "wrought iron", "Spanning Hook", "Destroyable_HeavyMetal"),
			         ("earlymodern_military_tool_windlass", "windlass", "a crossbow windlass",
				         "This twin-crank windlass uses cords, hooks, and geared drums to span a powerful crossbow steadily and with little wasted effort.",
				         3400.0, 150.0m, "wrought iron", "Windlass", "Destroyable_HeavyMetal")
		         })
		{
			CreateItem(reference, noun, shortDescription, null, fullDescription, SizeCategory.Normal,
				ItemQuality.Standard, weight, cost, false, false, material,
				[eraTag, militaryTag, toolMarketTag, $"{spanningRoot} / {toolTag}"],
				["Holdable", destroyableComponent], null, null, null, null,
				"Stock crossbow spanning tool for the dependency-ledger closure tranche.");
		}

		foreach (var (reference, shortDescription, fullDescription, component) in new[]
		         {
			         ("earlymodern_military_melee_plug_bayonet_service", "a serviceable plug bayonet",
				         "This broad iron plug bayonet has a tapered wooden grip made to seat directly in a musket muzzle, turning the unloaded arm into a short spear.",
				         "Bayonet_Plug"),
			         ("earlymodern_military_melee_socket_bayonet_service", "a serviceable socket bayonet",
				         "This offset iron socket bayonet twists onto a musket barrel while leaving the muzzle clear for loading and firing.",
				         "Bayonet_Socket"),
			         ("earlymodern_military_melee_sword_bayonet_service", "a serviceable sword bayonet",
				         "This long iron sword bayonet locks beside a musket muzzle and remains useful as a fighting blade after removal.",
				         "Bayonet_Sword")
		         })
		{
			CreateItem(reference, "bayonet", shortDescription, null, fullDescription, SizeCategory.Small,
				ItemQuality.Standard, 520.0, 105.0m, false, false, "wrought iron",
				[eraTag, militaryTag, weaponTag],
				["Holdable", "Melee_Bayonet", component, "Destroyable_Weapon", "Beltable"], null, null, null, null,
				"Stock functional bayonet using the musket attachment slots.");
		}
	}
}

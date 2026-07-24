#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	internal static IReadOnlyCollection<string> EarlyModernStandardsAndSignalsStableReferencesForTesting =>
		new[]
			{
				"infantry_colour", "cavalry_standard", "dragoon_guidon", "naval_ensign", "signal_flag",
				"lance_pennant", "field_drum", "kettle_drum", "military_fife", "speaking_trumpet"
			}
			.SelectMany(family => new[] { "issue", "reinforced", "ornate" }
				.Select(tier => $"earlymodern_military_accessory_{family}_{tier}"))
			.ToArray();

	private void SeedEarlyModernMilitaryFirearmsUniformsAndNaval()
	{
		const string eraTag = "Era / Early Modern Era";
		const string militaryTag = "Functions / Military Equipment";
		const string weaponTag = "Functions / Military Equipment / Military Weapons";
		const string signalTag = "Functions / Military Equipment / Military Signals";
		const string standardTag = "Functions / Military Equipment / Military Standards";
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

		foreach (var (family, noun, component, size, issueWeight, issueCost, reinforcedWeight, reinforcedCost,
			         ornateWeight, ornateCost) in new[]
		         {
			         ("infantry_colour", "colour", "MilitaryStandard_InfantryColour", SizeCategory.Large,
				         2800.0, 180.0m, 2910.0, 360.0m, 2860.0, 756.0m),
			         ("cavalry_standard", "standard", "MilitaryStandard_CavalryStandard", SizeCategory.Large,
				         2200.0, 190.0m, 2290.0, 380.0m, 2240.0, 798.0m),
			         ("dragoon_guidon", "guidon", "MilitaryStandard_Guidon", SizeCategory.Large,
				         1900.0, 170.0m, 1980.0, 340.0m, 1940.0, 714.0m),
			         ("naval_ensign", "ensign", "MilitaryStandard_NavalEnsign", SizeCategory.Large,
				         2100.0, 160.0m, 2180.0, 320.0m, 2140.0, 672.0m),
			         ("signal_flag", "flag", "MilitaryStandard_SignalFlag", SizeCategory.Normal,
				         850.0, 80.0m, 885.0, 160.0m, 865.0, 336.0m),
			         ("lance_pennant", "pennant", "MilitaryStandard_Pennant", SizeCategory.Small,
				         340.0, 48.0m, 355.0, 96.0m, 345.0, 202.0m)
		         })
		{
			var display = family.Replace('_', ' ');
			foreach (var (tier, adjective, material, quality, weight, cost) in new[]
			         {
				         ("issue", "plain", "canvas", ItemQuality.Standard, issueWeight, issueCost),
				         ("reinforced", "reinforced", "wool", ItemQuality.Good, reinforcedWeight, reinforcedCost),
				         ("ornate", "tooled", "silk", ItemQuality.VeryGood, ornateWeight, ornateCost)
			         })
			{
				CreateItem($"earlymodern_military_accessory_{family}_{tier}", noun,
					$"a {adjective} {display}", null,
					$"This {adjective} {display} is fixed to a stout staff and ready for a builder to assign its identity, lawful owner, and unit or ship association.",
					size, quality, weight, cost, false, false, material,
					[eraTag, militaryTag, standardTag],
					["Holdable", component, "Destroyable_Clothing"], null, null, null, null,
					"Stock military standard; new copies begin unowned, unclaimed, unassociated and with zero captures.");
			}
		}

		foreach (var (family, noun, component, size, issueMaterial, reinforcedMaterial, ornateMaterial,
			         issueWeight, issueCost, reinforcedWeight, reinforcedCost, ornateWeight, ornateCost,
			         wearable) in new[]
		         {
			         ("field_drum", "drum", "SignalInstrument_FieldDrum", SizeCategory.Normal,
				         "pine", "oak", "walnut", 4200.0, 180.0m, 4370.0, 360.0m, 4280.0, 756.0m, true),
			         ("kettle_drum", "drum", "SignalInstrument_KettleDrum", SizeCategory.Large,
				         "copper", "brass", "silver", 7800.0, 360.0m, 8100.0, 720.0m, 7950.0, 1512.0m, false),
			         ("military_fife", "fife", "SignalInstrument_Fife", SizeCategory.Small,
				         "boxwood", "ebony", "ivory", 180.0, 90.0m, 185.0, 180.0m, 185.0, 378.0m, false),
			         ("speaking_trumpet", "trumpet", "SignalInstrument_SpeakingTrumpet", SizeCategory.Normal,
				         "copper", "brass", "silver", 680.0, 120.0m, 705.0, 240.0m, 695.0, 504.0m, false)
		         })
		{
			var display = family.Replace('_', ' ');
			foreach (var (tier, adjective, material, quality, weight, cost) in new[]
			         {
				         ("issue", "plain", issueMaterial, ItemQuality.Standard, issueWeight, issueCost),
				         ("reinforced", "reinforced", reinforcedMaterial, ItemQuality.Good, reinforcedWeight,
					         reinforcedCost),
				         ("ornate", "tooled", ornateMaterial, ItemQuality.VeryGood, ornateWeight, ornateCost)
			         })
			{
				var components = wearable
					? new[] { "Holdable", component, "Wear_Shoulder", "Destroyable_Misc" }
					: new[] { "Holdable", component, "Destroyable_Misc" };
				CreateItem($"earlymodern_military_accessory_{family}_{tier}", noun,
					$"a {adjective} {display}", null,
					$"This {adjective} {display} is made for music and clear named military signals heard beyond its immediate position.",
					size, quality, weight, cost, false, false, material,
					[eraTag, militaryTag, signalTag], components, null, null, null, null,
					"Stock signal instrument with sustained performance and named-call support.");
			}
		}
	}
}

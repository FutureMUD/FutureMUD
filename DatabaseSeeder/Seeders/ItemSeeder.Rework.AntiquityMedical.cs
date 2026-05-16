using MudSharp.GameItems;
using System;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedAntiquityMedicalItems()
	{
		void AddMedicalItem(
			string stableReference,
			string noun,
			string shortDescription,
			string fullDescription,
			SizeCategory size,
			ItemQuality quality,
			double weightInGrams,
			decimal cost,
			string material,
			string[] tags,
			string[] components)
		{
			CreateItem(
				stableReference,
				noun,
				shortDescription,
				null,
				fullDescription,
				size,
				quality,
				weightInGrams,
				cost,
				false,
				false,
				material,
				tags,
				components,
				null,
				null,
				null,
				null);
		}

		string[] treatmentSupplyTags =
		[
			"Market / Medicine / Treatment Supplies"
		];
		string[] herbalMedicineTags =
		[
			"Functions / Medical Treatment / Herbal Remedy",
			"Market / Medicine / Herbal Medicine"
		];
		string[] surgicalSupplyTags =
		[
			"Market / Medicine / Surgical Supplies"
		];
		string[] prostheticTags =
		[
			"Functions / Medical Treatment / Prosthetic",
			"Market / Medicine / Prosthetics and Mobility"
		];

		AddMedicalItem(
			"antiquity_linen_bandage_roll",
			"roll",
			"a roll of clean linen bandage",
			"This narrow roll of linen has been washed, dried, and wound tight for wound binding. The weave is plain but even, and the end is left ready to tear into strips.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			4.0m,
			"linen",
			["Functions / Medical Treatment / Bandage", .. treatmentSupplyTags, "Functions / Material Functions / Salvagable Fabric"],
			["Holdable", "Stack_Number", "Destroyable_Misc", "Bandage_Good"]);

		AddMedicalItem(
			"antiquity_honeyed_linen_dressing",
			"dressing",
			"a honeyed linen wound dressing",
			"This folded linen dressing has been soaked with honey and pressed into a tidy pad. It is sticky, fragrant, and ready to cover a wound with the kind of sweet poultice ancient healers trusted.",
			SizeCategory.Small,
			ItemQuality.Good,
			90.0,
			8.0m,
			"linen",
			["Functions / Medical Treatment / Bandage", "Functions / Medical Treatment / Antiseptic Dressing", .. treatmentSupplyTags, "Market / Medicine / Herbal Medicine"],
			["Holdable", "Destroyable_Misc", "Bandage_Great", "TopicalCream_Honey_Poultice"]);

		AddMedicalItem(
			"antiquity_vinegar_wound_cloth",
			"cloth",
			"a vinegar-soaked wound cloth",
			"This small linen cloth is damp with sharp vinegar and wrapped in a waxed scrap to keep it from drying out. It is meant for wiping dirt and old blood away before a wound is dressed.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			45.0,
			3.0m,
			"linen",
			["Functions / Medical Treatment / Wound Cleaning", .. treatmentSupplyTags],
			["Holdable", "Destroyable_Misc", "Clean_Single"]);

		AddMedicalItem(
			"antiquity_yarrow_styptic_pad",
			"pad",
			"a yarrow styptic pad",
			"This compact pad is stuffed with crushed yarrow and bound in a clean linen wrap. It smells green and bitter, and is meant to be pressed firmly to bleeding cuts.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			5.0m,
			"yarrow",
			["Functions / Medical Treatment / Bandage", "Functions / Medical Treatment / Herbal Remedy", .. treatmentSupplyTags, "Market / Medicine / Herbal Medicine"],
			["Holdable", "Destroyable_Misc", "Bandage_Good", "TopicalCream_Yarrow_Styptic"]);

		AddMedicalItem(
			"antiquity_wool_compress",
			"compress",
			"a packed wool wound compress",
			"This dense pad of clean wool has been packed inside a linen wrapper. It is crude but absorbent, suited to pressure and tending rather than delicate surgery.",
			SizeCategory.Small,
			ItemQuality.Standard,
			120.0,
			4.0m,
			"wool",
			["Functions / Medical Treatment / Tend Kit", .. treatmentSupplyTags],
			["Holdable", "Destroyable_Misc", "Tend_Single"]);

		AddMedicalItem(
			"antiquity_wooden_splint_pair",
			"splints",
			"a pair of padded wooden splints",
			"These two straight willow splints are smoothed, padded with linen, and drilled for tie cords. They are light enough for a field kit but stiff enough to hold a limb still.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			10.0m,
			"willow",
			["Functions / Medical Treatment / Splint", .. treatmentSupplyTags],
			["Holdable", "Destroyable_Misc", "Limb_Immobilising"]);

		AddMedicalItem(
			"antiquity_linen_arm_sling",
			"sling",
			"a linen arm sling",
			"This broad linen sling is hemmed at the edges and fitted with simple ties. It is meant to cradle an injured arm against the body without much fuss.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			4.0m,
			"linen",
			["Functions / Medical Treatment / Splint", "Functions / Medical Treatment / Mobility Aid", .. treatmentSupplyTags],
			["Holdable", "Destroyable_Misc", "Limb_Immobilising"]);

		AddMedicalItem(
			"antiquity_linen_tending_kit",
			"kit",
			"a linen-wrapped tending kit",
			"This compact kit is rolled in linen and holds compresses, ties, small cloths, and simple wound-packing supplies. It is made for repeated routine tending rather than surgical work.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			16.0m,
			"linen",
			["Functions / Medical Treatment / Tend Kit", .. treatmentSupplyTags],
			["Holdable", "Destroyable_Misc", "Tend_Kit"]);

		AddMedicalItem(
			"antiquity_wound_cleaning_kit",
			"kit",
			"a wound cleaning kit",
			"This small kit holds linen swabs, a scraping spatula, and a stoppered vessel for water or vinegar. It is meant to clean dirt from wounds before heavier treatment begins.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			18.0m,
			"linen",
			["Functions / Medical Treatment / Wound Cleaning", .. treatmentSupplyTags],
			["Holdable", "Destroyable_Misc", "Clean_Kit"]);

		AddMedicalItem(
			"antiquity_honey_antiseptic_kit",
			"kit",
			"a honey and vinegar antiseptic kit",
			"This kit combines stoppered pots of honey and vinegar with linen pads and a small spatula. It reflects practical ancient wound care: clean, dress, and keep the wound covered.",
			SizeCategory.Normal,
			ItemQuality.Good,
			760.0,
			28.0m,
			"honey",
			["Functions / Medical Treatment / Antiseptic Dressing", .. treatmentSupplyTags, "Market / Medicine / Herbal Medicine"],
			["Holdable", "Destroyable_Misc", "Antiseptic_Kit", "TopicalCream_Honey_Poultice"]);

		AddMedicalItem(
			"antiquity_gut_suture_spool",
			"spool",
			"a spool of plain gut suture",
			"This small bone spool carries carefully twisted gut thread. The thread is uneven by modern standards, but it is strong enough for stitching flesh and binding small surgical repairs.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			45.0,
			8.0m,
			"gut",
			["Functions / Joining / Sewing / Surgical Suturing / Absorbable Suture / Plain Gut Suture", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc", "Suture_Single"]);

		AddMedicalItem(
			"antiquity_bone_suture_needle",
			"needle",
			"a polished bone suture needle",
			"This curved bone needle has been polished to a smooth point and drilled with a small eye. It is meant for coarse suturing where a bronze needle would be too dear.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			5.0m,
			"bone",
			["Functions / Tools / Surgical Tools / Surgical Suture Needle", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc"]);

		AddMedicalItem(
			"antiquity_suturing_kit",
			"kit",
			"a compact suturing kit",
			"This leather roll holds gut thread, bone and bronze needles, and tiny cloths for wiping the work. It is small enough for a physician's satchel but laid out for repeated use.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			32.0m,
			"leather",
			["Functions / Medical Treatment / Suture Kit", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc", "Suture_Kit", "Container_Pouch"]);

		AddMedicalItem(
			"antiquity_bronze_surgical_probe",
			"probe",
			"a bronze surgical probe",
			"This slender bronze probe has a rounded tip at one end and a flattened spatulate end at the other. It is meant for careful exploration, dressing, and application of salves.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			35.0,
			12.0m,
			"bronze",
			["Functions / Tools / Surgical Tools / Surgical Probe", "Functions / Tools / Medical Tools / Ointment Spatula", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc"]);

		AddMedicalItem(
			"antiquity_bronze_scalpel",
			"scalpel",
			"a bronze surgical scalpel",
			"This small scalpel has a leaf-shaped bronze blade set into a plain handle. The edge is keen enough for incision work, though it needs frequent honing.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			65.0,
			18.0m,
			"bronze",
			["Functions / Tools / Surgical Tools / Scalpel", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc"]);

		AddMedicalItem(
			"antiquity_bronze_forceps",
			"forceps",
			"a pair of bronze forceps",
			"These bronze forceps are springy and narrow, with flattened tips for gripping cloth, splinters, or tissue. They are simple but useful in a surgical roll.",
			SizeCategory.Small,
			ItemQuality.Standard,
			90.0,
			16.0m,
			"bronze",
			["Functions / Tools / Surgical Tools / Forceps", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc"]);

		AddMedicalItem(
			"antiquity_bronze_arterial_clamp",
			"clamp",
			"a bronze arterial clamp",
			"This bronze clamp has flattened jaws and a simple locking bend. It is crude, but intended to pinch and hold during difficult wound work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			125.0,
			20.0m,
			"bronze",
			["Functions / Tools / Surgical Tools / Arterial Clamp", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc"]);

		AddMedicalItem(
			"antiquity_bronze_cautery_iron",
			"iron",
			"a bronze cautery iron",
			"This bronze cautery iron has a long handle and a small flattened working end. It is made to be heated in coals before sealing or searing a wound.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			22.0m,
			"bronze",
			["Functions / Tools / Surgical Tools / Cautery Iron", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc"]);

		AddMedicalItem(
			"antiquity_bronze_bone_saw",
			"saw",
			"a bronze surgical bone saw",
			"This short bronze saw has a narrow toothed blade and a wrapped handle. It is a grim but practical tool for the hard medicine of broken or ruined limbs.",
			SizeCategory.Small,
			ItemQuality.Standard,
			310.0,
			28.0m,
			"bronze",
			["Functions / Tools / Surgical Tools / Bonesaw", .. surgicalSupplyTags],
			["Holdable", "Destroyable_Misc"]);

		AddMedicalItem(
			"antiquity_willow_bark_packets",
			"packets",
			"some willow bark tea packets",
			"These papyrus packets hold shaved willow bark and a little dried mint. They are meant to be steeped into a bitter draught for pain and fever.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			6.0m,
			"willow",
			herbalMedicineTags,
			["Holdable", "Stack_Number", "Destroyable_Misc", "Pill_Willow_Bark_Tea"]);

		AddMedicalItem(
			"antiquity_poppy_latex_draught",
			"draught",
			"a stoppered poppy latex draught",
			"This small fired-clay vial holds a dark, bitter draught prepared from poppy latex. It is marked with a warning knot on the stopper because it is both useful and dangerous.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			90.0,
			18.0m,
			"poppy latex",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "Pill_Poppy_Latex_Draught"]);

		AddMedicalItem(
			"antiquity_mandrake_draught_vial",
			"vial",
			"a mandrake draught vial",
			"This narrow fired-clay vial contains a murky mandrake draught. A black thread around its neck warns that the dose is for serious pain or sleep, not casual use.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			15.0m,
			"mandrake",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "Pill_Mandrake_Draught"]);

		AddMedicalItem(
			"antiquity_ephedra_brew_packets",
			"packets",
			"some ephedra brew packets",
			"These small packets hold dried ephedra stems and a little mint. The brew is sharp and bracing, used when a patient must be kept warm and breathing.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			7.0m,
			"ephedra",
			herbalMedicineTags,
			["Holdable", "Stack_Number", "Destroyable_Misc", "Pill_Ephedra_Brew"]);

		AddMedicalItem(
			"antiquity_foxglove_tincture_vial",
			"vial",
			"a foxglove tincture vial",
			"This tiny vial holds a concentrated foxglove tincture. The stopper is marked twice, because even a healer who trusts it should treat it with suspicion.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			45.0,
			20.0m,
			"foxglove",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "Pill_Foxglove_Tincture"]);

		AddMedicalItem(
			"antiquity_mint_infusion_bundle",
			"bundle",
			"a mint infusion bundle",
			"This bundle of dried mint has been tied with linen thread for steeping. It is a gentle stomach remedy, valued because it is easy to prepare and hard to misuse.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			45.0,
			4.0m,
			"mint",
			herbalMedicineTags,
			["Holdable", "Stack_Number", "Destroyable_Misc", "Pill_Mint_Infusion"]);

		AddMedicalItem(
			"antiquity_honey_poultice_pot",
			"pot",
			"a small honey poultice pot",
			"This little fired-clay pot holds honey mixed with a trace of resin and crushed herbs. It is sticky, fragrant, and made for dressing wounds rather than eating.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			10.0m,
			"honey",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "TopicalCream_Honey_Poultice"]);

		AddMedicalItem(
			"antiquity_garlic_salve_pot",
			"pot",
			"a garlic salve pot",
			"This fired-clay salve pot holds crushed garlic worked into oil and resin. The smell is fierce, but it is a trusted treatment for foul skin and stubborn wounds.",
			SizeCategory.Small,
			ItemQuality.Standard,
			170.0,
			8.0m,
			"garlic",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "TopicalCream_Garlic_Salve"]);

		AddMedicalItem(
			"antiquity_aloe_burn_salve_pot",
			"pot",
			"an aloe burn salve pot",
			"This small pot contains green aloe pulp worked into a cooling salve. It is meant for burns, rash, and angry skin where oil alone would feel too heavy.",
			SizeCategory.Small,
			ItemQuality.Standard,
			160.0,
			9.0m,
			"aloe vera",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "TopicalCream_Aloe_Burn_Salve"]);

		AddMedicalItem(
			"antiquity_henbane_fumigation_cone",
			"cone",
			"a henbane fumigation cone",
			"This thumb-sized cone is packed from henbane, charcoal dust, and resin. It is made to smoulder briefly, filling a cup or tent with a heavy medicinal smoke.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			9.0m,
			"henbane",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "Smokeable_Henbane_Smoke"]);

		AddMedicalItem(
			"antiquity_mandrake_smoke_cone",
			"cone",
			"a mandrake smoke cone",
			"This dark cone mixes dried mandrake, charcoal, and resin binder. It is intended for fumigation, though its bitter smoke is not something to treat lightly.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			38.0,
			10.0m,
			"mandrake",
			herbalMedicineTags,
			["Holdable", "Destroyable_Misc", "Smokeable_Mandrake_Draught"]);

		AddMedicalItem(
			"antiquity_herbalist_pouch",
			"pouch",
			"a labelled herbalist's pouch",
			"This small leather pouch has linen labels stitched to several inner pockets. It is made to keep dried remedies, bark, roots, and salve packets separate.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			18.0m,
			"leather",
			["Market / Medicine / Herbal Medicine", "Market / Medicine / Treatment Supplies"],
			["Holdable", "Destroyable_Misc", "Container_Pouch"]);

		AddMedicalItem(
			"antiquity_surgical_tool_roll",
			"roll",
			"a leather surgical tool roll",
			"This leather roll is slit and stitched to hold probes, blades, needles, and forceps. A long tie wraps around it to keep the tools from spilling out.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			24.0m,
			"leather",
			["Market / Medicine / Surgical Supplies"],
			["Holdable", "Destroyable_Misc", "Container_Pouch"]);

		AddMedicalItem(
			"antiquity_willow_crutch",
			"crutch",
			"a padded willow crutch",
			"This willow crutch has a forked upper rest padded with wrapped linen and a worn leather grip. It is light enough for daily use but solidly braced.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1450.0,
			16.0m,
			"willow",
			["Functions / Medical Treatment / Mobility Aid", "Market / Medicine / Prosthetics and Mobility"],
			["Holdable", "Destroyable_Misc", "Crutch"]);

		AddMedicalItem(
			"antiquity_simple_walking_cane",
			"cane",
			"a simple walking cane",
			"This straight cedar cane has a smoothed knob grip and a darkened tip. It is less supportive than a crutch, but useful for the lame, old, or recovering.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			650.0,
			8.0m,
			"cedar",
			["Functions / Medical Treatment / Mobility Aid", "Market / Medicine / Prosthetics and Mobility"],
			["Holdable", "Destroyable_Misc", "Crutch"]);

		AddMedicalItem(
			"antiquity_padded_wooden_peg_leg",
			"leg",
			"a padded wooden peg leg",
			"This cedar peg leg has a cupped and padded top, leather straps, and a blunt lower peg. It is simple, strong, and made for survival rather than grace.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			45.0m,
			"cedar",
			prostheticTags,
			["Holdable", "Destroyable_Misc", "Prosthetic_LLowerLeg", "Prosthetic_RLowerLeg"]);

		AddMedicalItem(
			"antiquity_carved_prosthetic_foot",
			"foot",
			"a carved wooden prosthetic foot",
			"This carved cedar foot is flattened underneath and drilled for strap fittings. It is stiff and plain, but shaped enough to pass under a sandal or long hem.",
			SizeCategory.Small,
			ItemQuality.Standard,
			820.0,
			28.0m,
			"cedar",
			prostheticTags,
			["Holdable", "Destroyable_Misc", "Prosthetic_LFoot", "Prosthetic_RFoot"]);

		AddMedicalItem(
			"antiquity_carved_prosthetic_hand",
			"hand",
			"a carved wooden prosthetic hand",
			"This wooden hand is carved with fixed fingers and a thumb-shaped brace. It is padded at the wrist and fitted with straps, useful more for appearance and bracing than fine work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			34.0m,
			"cedar",
			prostheticTags,
			["Holdable", "Destroyable_Misc", "Prosthetic_LHand_Functional", "Prosthetic_RHand_Functional"]);

		AddMedicalItem(
			"antiquity_bronze_hook_hand",
			"hook",
			"a bronze hook hand",
			"This bronze hook is mounted into a leather socket with stout ties. It is polished smooth inside the curve and shaped for catching, carrying, and simple leverage.",
			SizeCategory.Small,
			ItemQuality.Standard,
			760.0,
			42.0m,
			"bronze",
			prostheticTags,
			["Holdable", "Destroyable_Misc", "Prosthetic_LWrist", "Prosthetic_RWrist"]);

		AddMedicalItem(
			"antiquity_painted_clay_eye",
			"eye",
			"a painted clay prosthetic eye",
			"This tiny fired-clay eye has been smoothed, whitened, and painted with a dark iris. It is not lifelike up close, but it gives an empty socket a finished look.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			12.0m,
			"fired clay",
			prostheticTags,
			["Holdable", "Destroyable_Misc", "Prosthetic_LEye", "Prosthetic_REye"]);
	}
}

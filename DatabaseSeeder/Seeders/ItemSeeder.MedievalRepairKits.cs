#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalRepairKits()
	{
		// 3.7 Repair kits and repair supplies
		CreateItem(
			"medieval_repair_rough_cloth_mending_pouch",
			"pouch",
			"a rough cloth-mending pouch",
			null,
			"This small pouch holds coarse thread, mismatched patches, bone needles, and a lump of beeswax. It is poor but useful for keeping clothing and cloth gear together. The outer pouch is itself patched in two places.",
			SizeCategory.Small,
			ItemQuality.Poor,
			320.0,
			12.0m,
			true,
			false,
			"linen",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cloth_Poor",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_cloth_mending_roll",
			"roll",
			"a cloth-mending roll",
			null,
			"A linen roll opens to show thread, patches, needles, and small shears wrapped in ordered rows. The supplies are ordinary but clean and easy to find. It is suitable for household clothing, tents, and cloth packs.",
			SizeCategory.Small,
			ItemQuality.Standard,
			480.0,
			24.0m,
			true,
			false,
			"linen",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cloth",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_fine_cloth_mending_case",
			"case",
			"a fine cloth-mending case",
			null,
			"This neat leather case holds dyed silk thread, fine linen patches, polished needles, and a small smoothing bone. The interior loops keep the tools from tangling. It is fit for better garments and household textiles.",
			SizeCategory.Small,
			ItemQuality.Good,
			560.0,
			65.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cloth_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_rough_leather_repair_pouch",
			"pouch",
			"a rough leather repair pouch",
			null,
			"This pouch carries heavy thonging, scrap leather, a blunt awl, and waxed thread. The tools are crude and scarred, but they are enough for a field repair. It smells of oil, hide, and dust.",
			SizeCategory.Small,
			ItemQuality.Poor,
			620.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Leather_Poor",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_leather_repair_roll",
			"roll",
			"a leather repair roll",
			null,
			"A sturdy leather roll holds awls, thonging, waxed thread, spare patches, and a small burnisher. Each tool has a simple loop to keep it in place. It is a standard kit for belts, bags, shoes, and harness.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1100.0,
			45.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Leather",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_saddler_leather_case",
			"case",
			"a saddler’s leather repair case",
			null,
			"This better case is stocked with fine awls, strong waxed cord, dyed leather patches, and polished bone tools. The stitching is neat and the contents are chosen for more careful repair work. It suits a saddler or prosperous leatherworker.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1450.0,
			95.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Leather_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_simple_wood_repair_pouch",
			"pouch",
			"a simple wood-repair pouch",
			null,
			"Small pegs, wedges, glue scrap, and a blunt carving knife are kept in this rough pouch. The contents are cheap but useful for tightening loose wooden goods. Wood dust has settled into the seams.",
			SizeCategory.Small,
			ItemQuality.Poor,
			700.0,
			16.0m,
			true,
			false,
			"wood",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Wood_Poor",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_woodworker_repair_box",
			"box",
			"a woodworker’s repair box",
			null,
			"This small wooden box holds pegs, wedges, resin glue, clamps, cord, and a few small shaping tools. The lid is scarred by use as a work surface. It is meant for ordinary furniture, handles, and wooden gear.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			62.0m,
			true,
			false,
			"wood",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Wood",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_joiner_repair_chest",
			"chest",
			"a joiner’s fine repair chest",
			null,
			"A well-fitted oak chest contains matched pegs, wedges, glue cakes, clamps, chisels, and smoothing tools. The compartments are tidy and labelled by shape rather than writing. It is built for careful repair of valued wooden goods.",
			SizeCategory.Large,
			ItemQuality.Good,
			6200.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Wood_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_pot_menders_pouch",
			"pouch",
			"a pot-mender’s ceramic repair pouch",
			null,
			"This pouch contains cord clamps, resin, spare sherds, and small smoothing stones for cracked pots. The contents are humble and dusty. It looks like something carried by a travelling mender rather than a wealthy workshop.",
			SizeCategory.Small,
			ItemQuality.Standard,
			900.0,
			28.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Ceramic",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_fine_ceramic_mending_case",
			"case",
			"a fine ceramic mending case",
			null,
			"A padded case contains better resin, fine clamps, smooth sherd patches, and polishing cloths for glazed wares. The supplies are arranged to avoid scratching fragile surfaces. It is a careful kit for good household vessels.",
			SizeCategory.Small,
			ItemQuality.Good,
			1050.0,
			64.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Ceramic_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_stone_mason_patch_kit",
			"kit",
			"a stone mason’s patch kit",
			null,
			"A heavy kit holds wedges, stone dust, resin, small chisels, and rough cloth for patching chipped stone goods. It is too weighty for delicate work but sturdy enough for everyday repairs. The pouch is powdered grey inside.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			50.0m,
			true,
			false,
			"stone",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Stone",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_bone_horn_mending_roll",
			"roll",
			"a bone-and-horn mending roll",
			null,
			"A cloth roll holds small drills, sinew, resin, horn shavings, and polished bone slips. It is meant for combs, handles, fittings, and other hard organic pieces. The smell is dry, sharp, and faintly gluey.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			42.0m,
			true,
			false,
			"bone",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Hard_Organic",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_fine_hard_organic_case",
			"case",
			"a fine hard-organic repair case",
			null,
			"This neat case contains matched horn slivers, bone patches, fine cord, and smooth polishing tools. Each small piece is wrapped so it does not scratch the next. It is meant for valued horn, bone, shell, or antler goods.",
			SizeCategory.Small,
			ItemQuality.Good,
			720.0,
			88.0m,
			true,
			false,
			"horn",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Hard_Organic_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_blacksmith_metal_kit",
			"kit",
			"a blacksmith’s metal repair kit",
			null,
			"A heavy kit contains small clamps, rivets, wire, wedges, and dark repair stock for common metal goods. The pouch is scorched in places and leaves a soot smell on the hands. It is practical rather than portable elegance.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3100.0,
			70.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_metal_tool_repair_roll",
			"roll",
			"a metal tool repair roll",
			null,
			"This roll carries rivets, wedges, pins, a file, binding wire, and small grips for repairing metal tools. The leather is dark with oil and the file has its own protective flap. It is meant for tools, not weapons or armour.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			76.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / General Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Tool",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_fine_metal_tool_case",
			"case",
			"a fine metal tool repair case",
			null,
			"A sturdy case holds better files, fitted rivets, small clamps, and polished metal pins. Everything sits in tight loops so the edges do not damage the leather. It is a workshop-quality kit for valued tools.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2100.0,
			140.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Tool_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_armourers_field_kit",
			"kit",
			"an armourer’s field repair kit",
			null,
			"This heavy kit holds rivets, leather lacing, wire, buckles, and a small hammer wrapped in cloth. It is designed for damaged armour on campaign. The outside bears dents and dark metal stains.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			95.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Armour",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_weapon_mending_roll",
			"roll",
			"a weapon-mending repair roll",
			null,
			"A leather roll contains wedges, pins, grip cord, rivets, and small metal fittings for weapon repairs. The tools are compact and hard-worn. It is aimed at keeping field weapons usable, not forging new blades.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			85.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Weapon",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_fine_weapon_mending_case",
			"case",
			"a fine weapon-mending case",
			null,
			"This fitted case holds fine rivets, polished pins, strong binding wire, grip leather, and small finishing tools. The supplies are carefully sorted and better made than a field kit. It belongs to a serious armoury or weapon shop.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2100.0,
			160.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Metal_Weapon_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_glass_poor_lead_patch_pouch",
			"pouch",
			"a rough glass patching pouch",
			null,
			"This poor pouch holds scrap lead came, resin, cloth pads, and a few cloudy glass chips. It is enough for crude patches on small panes or vessels. The contents clink softly through the padding.",
			SizeCategory.Small,
			ItemQuality.Poor,
			850.0,
			30.0m,
			true,
			false,
			"lead",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Glass_Poor",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_glass_menders_case",
			"case",
			"a glass-mender’s repair case",
			null,
			"A padded case contains lead strips, resin, smooth cloth, spare glass pieces, and small setting tools. The compartments keep hard edges away from the glass. It is a standard kit for fragile vessels and panes.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			85.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Glass",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_fine_glass_menders_case",
			"case",
			"a fine glass-mender’s case",
			null,
			"This carefully padded case holds clean lead, fine resin, polished cloth, and clearer glass repair pieces. Every fragile part is wrapped separately. It is meant for costly glassware and careful shop work.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1700.0,
			160.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Glass_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_poor_paper_patch_packet",
			"packet",
			"a poor paper patch packet",
			null,
			"This packet contains rough scraps of paper, thin paste, and a little smoothing stick. The materials are mismatched, but they can patch a torn document or wrapper. Paste stains have stiffened one corner of the packet.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			160.0,
			10.0m,
			true,
			false,
			"paper",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Paper_Poor",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_document_repair_pouch",
			"pouch",
			"a document repair pouch",
			null,
			"A small pouch carries paper and parchment patches, paste, thread, and a polished bone smoother. The tools are light and carefully wrapped. It is meant for everyday repair of records, labels, and book leaves.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			38.0m,
			true,
			false,
			"parchment",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Paper",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_scriptorium_repair_case",
			"case",
			"a scriptorium repair case",
			null,
			"This fine case holds sorted parchment patches, smooth paste, linen thread, a bone folder, and small weights. The contents are dry, clean, and carefully protected from damp. It suits a scriptorium or chancery more than a road kit.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			96.0m,
			true,
			false,
			"parchment",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Paper_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_lacquer_patch_box",
			"box",
			"a lacquerware repair box",
			null,
			"A small wooden box contains resin, lacquer scrap, fine cloth, and a soft brush wrapped in paper. The inside is stained dark and glossy from old repairs. It is meant for bowls, boxes, and cases with lacquered surfaces.",
			SizeCategory.Small,
			ItemQuality.Standard,
			680.0,
			70.0m,
			true,
			false,
			"lacquer",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Lacquer",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_fine_lacquer_case",
			"case",
			"a fine lacquer repair case",
			null,
			"This lacquered case holds wrapped brushes, polished cloth, resin, and carefully kept lacquer stock. The supplies are protected from dust and sorted by colour and finish. It is a specialist kit for valued lacquerware.",
			SizeCategory.Small,
			ItemQuality.Good,
			760.0,
			145.0m,
			true,
			false,
			"lacquer",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Lacquer_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_rope_splicing_pouch",
			"pouch",
			"a rope-splicing repair pouch",
			null,
			"A tough pouch holds a wooden fid, wax, hemp fibres, and short lengths of cord. Tar and rope dust darken the seams. It is made for repairing lines, nets, and heavy cordage.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			32.0m,
			true,
			false,
			"hemp",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cordage",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_shipwright_cordage_case",
			"case",
			"a shipwright’s cordage repair case",
			null,
			"This larger case contains several fids, wax, tarred line, spare hemp, and binding cord. It smells of rope, pitch, and sea air. The tools are chosen for serious cordage work rather than small household string.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			90.0m,
			true,
			false,
			"hemp",
			[
				"Market / Repair Supplies / Specialist Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Cordage_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_rough_composite_bow_pouch",
			"pouch",
			"a rough composite-bow repair pouch",
			null,
			"This small pouch holds sinew, horn scraps, resin glue, and tight wrapping cord. The stock is crude and irregular, but chosen for emergency bow repairs. It smells of glue, hide, and old horn.",
			SizeCategory.Small,
			ItemQuality.Poor,
			520.0,
			40.0m,
			true,
			false,
			"sinew",
			[
				"Market / Repair Supplies / Specialist Repair Supplies",
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Composite_Bow_Poor",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_composite_bow_repair_roll",
			"roll",
			"a composite-bow repair roll",
			null,
			"A leather roll carries sinew backing, horn shims, glue cakes, clamps, and binding thread. The pieces are slim and carefully protected from damp. It is made for maintaining composite bows, not ordinary wooden self bows.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			110.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies",
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Composite_Bow",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_repair_master_bowyer_repair_case",
			"case",
			"a master bowyer’s repair case",
			null,
			"This fine case holds sorted horn slips, prepared sinew, resin glue, wrapping silk, and small clamps. The contents are dry, orderly, and expensive-looking. It is a specialist repair case for valued composite bows.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1500.0,
			220.0m,
			true,
			false,
			"leather",
			[
				"Market / Repair Supplies / Specialist Repair Supplies",
				"Market / Repair Supplies / Weapon and Armour Repair Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Repair_Composite_Bow_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);
	}
}

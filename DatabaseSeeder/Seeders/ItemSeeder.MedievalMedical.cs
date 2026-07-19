#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalMedicalAndApothecaryItems()
	{
		// 3.1 Raw stock and refills
		CreateItem(
			"medieval_medical_clean_linen_dressing_stock",
			"bundle",
			"a bundle of clean linen strips",
			null,
			"A tight bundle of narrow linen strips is tied with plain cord. The cloth has been washed, dried, and folded with the ends aligned for quick tearing into dressings. It looks like stock for a healer rather than ordinary rag cloth.",
			SizeCategory.Small,
			ItemQuality.Standard,
			220.0,
			5.0m,
			false,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Dressing Stock",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wool_wound_packing_stock",
			"bundle",
			"a bundle of clean wool packing",
			null,
			"This bundle holds soft locks of pale wool wrapped inside a linen cloth. The wool is combed loose and springy, suitable for pressure packing or padding heavier dressings. A faint sheepy smell remains under the cleaner cloth scent.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			4.0m,
			false,
			false,
			"wool",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Dressing Stock",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_cotton_dressing_stack",
			"stack",
			"a stack of cotton dressing squares",
			null,
			"Flat cotton squares are stacked into a neat packet and tied through the middle. The fabric is softer than plain linen and cut to an even size for covering grazes, burns, or salved wounds. The edges show a little fraying from knife-cut preparation.",
			SizeCategory.Small,
			ItemQuality.Good,
			140.0,
			8.0m,
			false,
			false,
			"cotton",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Dressing Stock",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_honeyed_poultice_cloth_stock",
			"packet",
			"a sticky packet of honeyed cloths",
			null,
			"Several folded cloths sit inside a waxed wrap, their fibres darkened by honey. The packet smells sweet and faintly floral, with enough tackiness that the folds pull apart reluctantly. It is clearly prepared as dressing stock rather than table linen.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			160.0,
			12.0m,
			false,
			false,
			"honey",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Poultice Stock",
				"Functions / Medical Treatment / Antiseptic Dressing"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_vinegar_cleaning_cloth_bundle",
			"bundle",
			"a bundle of vinegar-sharp cloths",
			null,
			"Small linen cloths are wrapped in a waxed outer rag to keep their sharp vinegar smell from fading. The cloth is damp but not dripping, and several corners are folded outward for quick grip. It is meant for wiping dirt, old blood, and crust from wounds.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			6.0m,
			false,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_willow_bark_bundle",
			"bundle",
			"a bundle of shaved willow bark",
			null,
			"Thin strips of willow bark are shaved into curling ribbons and tied with hemp thread. The inner bark shows pale tan fibres with a dry, faintly bitter smell. It is ready to steep, grind, or portion into simple medicine stock.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			5.0m,
			false,
			false,
			"willow",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_yarrow_styptic_bundle",
			"bundle",
			"a bundle of dried yarrow heads",
			null,
			"Small dried yarrow heads and feathered leaves are bundled with a thin linen tie. The herb has faded to dusty green and straw, but the bitter green scent is still present when handled. It is suitable for crushing into pads, powders, or styptic dressings.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			60.0,
			6.0m,
			false,
			false,
			"yarrow",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Styptic"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mandrake_root_packet",
			"packet",
			"a packet of cut mandrake root",
			null,
			"Pieces of pale mandrake root lie in a small parchment packet closed with thread. The roots are irregular, tough, and earthy, with a sharp bitter odour that clings to the paper. A red warning knot marks the packet as something to measure carefully.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			45.0,
			24.0m,
			false,
			false,
			"mandrake",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_henbane_leaf_packet",
			"packet",
			"a packet of dried henbane leaves",
			null,
			"Dried henbane leaves are packed into a narrow paper fold, dark and brittle against the pale wrapping. The packet smells musty and acrid, and a black tie marks it apart from ordinary kitchen herbs. It is prepared for smoke or fumigation rather than cooking.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			35.0,
			22.0m,
			false,
			false,
			"henbane",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Material Functions / Medical Craft Stock / Fumigation Stock"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_ephedra_twig_packet",
			"packet",
			"a packet of dried ephedra twigs",
			null,
			"Jointed green-brown twigs are bundled inside a small linen packet. The pieces are trimmed to a length that can be measured out by handful or pinch. The smell is dry, grassy, and faintly resinous.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			50.0,
			14.0m,
			false,
			false,
			"ephedra",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_foxglove_leaf_packet",
			"packet",
			"a warning-tied foxglove packet",
			null,
			"Several dark, crinkled foxglove leaves sit in a waxed paper packet bound with a red thread. The packet is small and carefully folded, as if meant for apothecary hands rather than common kitchen use. The dry leaves have a dusty, green-bitter smell.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			30.0,
			28.0m,
			false,
			false,
			"foxglove",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_alum_styptic_powder_packet",
			"packet",
			"a packet of pale alum powder",
			null,
			"A small parchment packet holds pale mineral powder that clumps slightly where it has drawn damp from the air. The fold is tight and clean, with a faint chalky dust along the crease. It is sized for careful pinches rather than bulk trade.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			40.0,
			10.0m,
			false,
			false,
			"alum",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Styptic"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_poppy_latex_resin_cake",
			"cake",
			"a dark poppy-latex resin cake",
			null,
			"This small resinous cake is wrapped in oiled cloth, dark brown and tacky at the edges. It has a heavy bitter smell and has been pressed into a flat disk for shaving tiny amounts. The careful wrapping suggests valued medicine rather than ordinary spice.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			35.0,
			40.0m,
			false,
			false,
			"resin",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_garlic_salve_herb_packet",
			"packet",
			"a pungent garlic-salve packet",
			null,
			"Crushed garlic and dry salve herbs are folded into a thick linen packet. The smell is strong, sharp, and savoury enough to cut through the beeswax around it. It is plainly a healer’s stock packet, not a kitchen bundle.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			5.0m,
			false,
			false,
			"garlic",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Salve Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mint_ginger_decoction_packet",
			"packet",
			"a mint-and-ginger decoction packet",
			null,
			"Dried mint leaves and sliced ginger are mixed in a neat paper packet. The packet smells bright, warming, and sweet-sharp, with the ingredients visible through the loose fold at the top. It is ready to steep into a small pot of tonic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			8.0m,
			false,
			false,
			"mint",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Decoction Stock",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_theriac_spice_electuary_stock",
			"jar",
			"a jar of theriac spice stock",
			null,
			"A small glazed jar contains a dark, sticky blend of honey, spice, and powdered medicine stock. The lid is cloth-capped and sealed around the rim with beeswax. Its smell is rich, bitter, sweet, and resinous all at once.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			220.0,
			70.0m,
			false,
			false,
			"spice",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock",
				"Functions / Medical Treatment / Antidote"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_gut_suture_hank",
			"hank",
			"a hank of cleaned gut suture",
			null,
			"Cleaned gut thread is wound into a careful hank and tied to a small bone toggle. The strands are pale, slightly translucent, and uneven in thickness, but twisted firmly enough for coarse surgical use. A linen wrap keeps the thread from tangling.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			10.0m,
			false,
			false,
			"gut",
			[
				"Market / Medicine / Surgical Supplies",
				"Functions / Material Functions / Medical Craft Stock / Suture Stock",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_willow_splint_blank_bundle",
			"bundle",
			"a bundle of willow splint blanks",
			null,
			"Straight pieces of willow are shaved smooth and tied into a narrow bundle. Each strip is drilled near the ends for future cord ties, but none have padding yet. They are raw stock for fracture splints rather than finished treatment gear.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			8.0m,
			false,
			false,
			"willow",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Material Functions / Medical Craft Stock / Splint Stock",
				"Functions / Medical Treatment / Splint"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy"
			],
			null,
			null,
			null,
			null
		);


		// 3.2 Single-use wound consumables
		CreateItem(
			"medieval_medical_rough_linen_bandage",
			"bandage",
			"a rough linen bandage",
			null,
			"This strip of linen has been torn rather than cut, leaving frayed edges and a few stubborn creases. It is serviceable enough for emergency binding but plainly not fine infirmary stock. Small knots in the weave make it feel coarse under the fingers.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			55.0,
			2.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Simple Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Poor"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_clean_linen_bandage",
			"bandage",
			"a clean linen bandage",
			null,
			"A narrow length of clean linen is rolled around itself into a compact bandage. The cloth is plain, pale, and closely woven, with enough length for wrapping an arm, hand, or smaller wound. Its loose end is tucked neatly under the roll.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Simple"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_narrow_linen_bandage_roll",
			"roll",
			"a narrow linen bandage roll",
			null,
			"This long, narrow bandage is rolled tight around a small reed core. The linen has been cut evenly and pressed flat, with only a few loose threads along one edge. It is suited to controlled wrapping rather than hurried field tearing.",
			SizeCategory.Small,
			ItemQuality.Good,
			140.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wool_pressure_compress",
			"compress",
			"a dense wool pressure compress",
			null,
			"Clean wool is packed into a thick pad and held inside a simple linen cover. The compress is springy but firm, made to be pressed hard over a bleeding or bruised place. Its corners are tied down so the wool does not spill loose.",
			SizeCategory.Small,
			ItemQuality.Standard,
			130.0,
			5.0m,
			true,
			false,
			"wool",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_cotton_wound_compress",
			"compress",
			"a soft cotton wound compress",
			null,
			"Several layers of cotton are folded into a soft square and lightly stitched at the edges. The pad is clean, pale, and more delicate than a wool compress, with enough body to absorb without feeling bulky. It has the look of careful apothecary preparation.",
			SizeCategory.Small,
			ItemQuality.Good,
			95.0,
			7.0m,
			true,
			false,
			"cotton",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_silk_wound_dressing",
			"dressing",
			"a fine silk wound dressing",
			null,
			"Fine silk is folded around a small inner pad and tied with two narrow threads. The fabric is smooth, light, and closely woven, making the dressing seem more courtly than field-made. It is compact enough to tuck into a physician’s case.",
			SizeCategory.VerySmall,
			ItemQuality.Great,
			50.0,
			24.0m,
			true,
			false,
			"silk",
			[
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Great"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_honeyed_linen_dressing",
			"dressing",
			"a honeyed linen wound dressing",
			null,
			"This folded dressing is darkened with honey and pressed into a tidy, sticky pad. A waxed wrap keeps it from smearing the healer’s pouch, and the sweet smell is obvious as soon as it is handled. The linen underneath remains visible at the corners.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			90.0,
			14.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Antiseptic Dressing",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Great",
				"TopicalCream_Honey_Poultice"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_vinegar_wound_cloth",
			"cloth",
			"a vinegar-soaked wound cloth",
			null,
			"A small square of linen is damp with sharp vinegar and folded inside a waxed scrap. The cloth is meant for wiping rather than wrapping, with one corner left dry enough to grip. It smells clean in a sour, practical way.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Clean_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wine_rinsed_wound_cloth",
			"cloth",
			"a wine-rinsed wound cloth",
			null,
			"This folded cloth is stained faint rose-brown from watered wine. It is wrapped in plain linen and smells of sour fruit and old cask, not perfume. The preparation is tidy enough for a travelling healer’s roll.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			50.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Clean_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_yarrow_styptic_pad",
			"pad",
			"a yarrow-stuffed styptic pad",
			null,
			"Crushed yarrow is packed between two small linen faces, making a green-bitter pad meant to be pressed down hard. Flecks of herb show through the weave near the seams. A simple thread tie keeps the pad from opening in a pouch.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			9.0m,
			true,
			false,
			"yarrow",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Yarrow_Styptic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_alum_styptic_pad",
			"pad",
			"an alum-dusted styptic pad",
			null,
			"A small folded pad has been dusted with pale alum powder and wrapped to keep the mineral from spilling. The cloth feels dry and chalky through the outer fold. It is made for quick pressure on a cut rather than broad bandaging.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			50.0,
			10.0m,
			true,
			false,
			"alum",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Alum_Styptic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_aloe_burn_dressing",
			"dressing",
			"an aloe-slick burn dressing",
			null,
			"This dressing is folded around a cool, slick herbal salve and sealed in waxed cloth. The surface has a greenish stain where the medicine has soaked into the linen. It is shaped for laying flat over a burned patch of skin.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			85.0,
			15.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Aloe_Burn_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_herbal_burn_dressing",
			"dressing",
			"a green herbal burn dressing",
			null,
			"A neat linen pad is filled with a soft green salve and folded into a waxed wrapper. Its smell is grassy, bitter, and faintly resinous. The cloth is broad enough to cover a palm-sized burn without being bulky.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			90.0,
			16.0m,
			true,
			false,
			"herb",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Herbal_Burn_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_garlic_salve_cloth",
			"cloth",
			"a garlic-salve wound cloth",
			null,
			"This small wound cloth has been smeared with a pungent garlic salve and folded under a beeswaxed cover. The smell is strong enough to announce it before the packet is opened. It is intended for laying over a small wound rather than wrapping a limb.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			7.0m,
			true,
			false,
			"garlic",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Garlic_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wound_packing_wad",
			"wad",
			"a linen wound-packing wad",
			null,
			"Strips of linen are folded and twisted into a soft wad that can be pushed into an awkward wound. The cloth is plain and unmedicated, with the fibres left loose enough to swell when damp. It is wrapped in another scrap to keep it clean.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			4.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_field_bandage_packet",
			"packet",
			"a field bandage packet",
			null,
			"A rough leather tie holds together a bandage, a small compress, and two short binding cords. The packet is compact and plain, made to fit a belt pouch or saddlebag. Dust marks on the outer cloth suggest hard campaign use.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			8.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Simple",
				"Tend_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_monastic_dressing_packet",
			"packet",
			"a monastic dressing packet",
			null,
			"Several careful dressings are folded inside a plain white linen cover marked with a small cross in brown thread. The packet is orderly rather than ornamental, with each pad stacked squarely. It smells faintly of honey, vinegar, and stored cloth.",
			SizeCategory.Small,
			ItemQuality.Good,
			170.0,
			12.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Standard Medicine",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"Clean_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_apothecary_folded_dressing",
			"dressing",
			"an apothecary folded dressing",
			null,
			"This dressing is folded into a crisp square and held by a small paper band. Herbal flecks and a faint resin smell mark it as something prepared behind an apothecary counter. The cloth is clean, even, and easy to open one-handed.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			75.0,
			13.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Honey_Poultice"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_horse_wound_bandage",
			"bandage",
			"a broad horse wound bandage",
			null,
			"A broad strip of strong linen is rolled around a stout wooden pin. It is wider and heavier than human dressings, with enough length to bind a leg or flank on a large animal. The cloth is plain but tough, made for stable use.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			10.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_plastered_poultice_pad",
			"pad",
			"a plastered herbal poultice pad",
			null,
			"A thick pad of cloth is plastered with a green-brown herbal paste and folded in on itself. The paste has dried enough not to drip, but it remains pliant under the fingers. The smell is earthy, sour, and medicinal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			10.0m,
			true,
			false,
			"herb",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single",
				"TopicalCream_Honey_Poultice"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_linen_suture_packet",
			"packet",
			"a packet of linen sutures",
			null,
			"Waxed linen thread is wound onto a small slip of wood and tucked inside a folded cloth. The thread is coarse but clean, with a needle-hole awl mark beside it for quick threading. It is a simple single-use surgical supply.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			25.0,
			6.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_silk_suture_card",
			"card",
			"a card of fine silk sutures",
			null,
			"Fine silk thread is wrapped around a thin parchment card and secured with a careful cross-tie. The thread is smoother and more regular than ordinary linen suture, with a soft sheen in the light. A small needle is tucked under the last loop.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			18.0m,
			true,
			false,
			"silk",
			[
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_gut_suture_spool",
			"spool",
			"a small gut suture spool",
			null,
			"Clean gut thread is wound around a little bone spool with care. The strand is pale, strong-looking, and slightly uneven, the sort of material used when a healer wants thread that belongs in the body rather than ordinary stitching. A linen cover protects the spool.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			40.0,
			14.0m,
			true,
			false,
			"gut",
			[
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bronze_suture_needle",
			"needle",
			"a curved bronze suture needle",
			null,
			"This small bronze needle has a smooth curve, a drilled eye, and a polished point. It is thicker than a seamstress needle and shaped for gripping with forceps or fingers. The metal has darkened slightly where it rests in its cloth wrap.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			8.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit",
				"Functions / Tools / Surgical Tools / Surgical Suture Needle"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Suture_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bone_suture_needle",
			"needle",
			"a polished bone suture needle",
			null,
			"A small curved needle has been carved from bone and rubbed smooth. The eye is broad and the point is serviceable rather than delicate, suited to coarse stitching. It rests inside a narrow scrap of leather to protect the tip.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			5.0m,
			true,
			false,
			"bone",
			[
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit",
				"Functions / Tools / Surgical Tools / Surgical Suture Needle"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Suture_Single"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_cautery_dressing_pad",
			"pad",
			"a thick cautery dressing pad",
			null,
			"This heavy pad is built from wool wrapped in linen, with darkened scorch marks along one face. It is meant to follow hot-iron work, not to serve as an ordinary bandage. The bulk and smoky smell make its purpose visible.",
			SizeCategory.Small,
			ItemQuality.Standard,
			150.0,
			6.0m,
			true,
			false,
			"wool",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single"
			],
			null,
			null,
			null,
			null
		);


		// 3.3 Treatment kits and cases
		CreateItem(
			"medieval_medical_wound_cleaning_kit",
			"kit",
			"a wound-cleaning kit",
			null,
			"This compact kit holds folded cloths, a blunt scraper, and a small spatula inside a leather roll. The pieces are plain but tidy, meant for removing dirt and old blood before heavier treatment begins. The roll ties closed with two waxed cords.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Clean_Kit",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_vinegar_wash_kit",
			"kit",
			"a vinegar wound-wash kit",
			null,
			"A leather pouch holds waxed cloths, wiping swabs, and space for a small sour-smelling wash vessel. Its outer flap is stained where damp cloth has rested against it. The kit is practical and compact, designed for repeated cleaning rather than display.",
			SizeCategory.Small,
			ItemQuality.Standard,
			620.0,
			22.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Clean_Kit"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_honey_vinegar_antiseptic_kit",
			"kit",
			"a honey-and-vinegar dressing kit",
			null,
			"This kit gathers honeyed pads, vinegar cloths, and a little wooden spatula in a waxed leather wrap. The mixture of sweet and sharp smells is unmistakable. Everything is arranged to clean a wound and then cover it quickly.",
			SizeCategory.Normal,
			ItemQuality.Good,
			850.0,
			34.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Antiseptic Dressing",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Antiseptic_Kit",
				"TopicalCream_Honey_Poultice"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_plain_tending_roll",
			"roll",
			"a plain wound-tending roll",
			null,
			"A roll of linen contains compresses, strips, packing cloths, and a few simple ties. It is not a surgical set, but it gives a healer enough supplies for routine tending. The outside is marked by use, with darker patches at the fold lines.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			540.0,
			18.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_travel_tending_roll",
			"roll",
			"a travel healer’s tending roll",
			null,
			"This leather-backed roll opens to reveal folded cloths, narrow bandages, a scraping spatula, and several waxed packets. It is built to travel, with loops that keep every piece from falling out on the road. The outer hide is scuffed from saddlebag use.",
			SizeCategory.Normal,
			ItemQuality.Good,
			900.0,
			36.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Clean_Kit"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_suturing_kit",
			"kit",
			"a compact suturing kit",
			null,
			"A small leather roll holds thread, needles, a bronze probe, and tiny wiping cloths. The needles sit in a stiffened flap and the thread is wound on bone and wood spools. It is plain but complete enough for ordinary stitching work.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			32.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Suture_Kit",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_fine_suturing_case",
			"case",
			"a fine suturing case",
			null,
			"This narrow case is covered in dark leather and fitted with small loops for silk, gut, and linen sutures. Bronze and bone needles sit separately so their points do not rub dull. It is a careful surgical supply, made for an infirmary or court physician.",
			SizeCategory.Small,
			ItemQuality.Good,
			420.0,
			72.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Suture_Kit_Good",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_field_surgeon_roll",
			"roll",
			"a field surgeon’s leather roll",
			null,
			"Heavy leather folds around bandages, sutures, a probe, and small metal tools. The roll is stained and repaired, but the straps still hold the contents firmly. It is meant to be opened beside a wounded person rather than displayed on a shelf.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1600.0,
			85.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"FieldMedkit",
				"Suture_Kit"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_battlefield_medkit",
			"kit",
			"a battlefield medicine kit",
			null,
			"This stout shoulder kit contains broad bandages, packed compresses, splints, sutures, and several waxed remedy packets. The leather is scuffed and darkened by travel, with a wide strap for carrying across a camp. It is built for trauma rather than household sickness.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			120.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Wound Cleaning"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"FieldMedkit",
				"Clean_Kit",
				"Tend_Kit"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_infirmary_chest",
			"chest",
			"a stocked infirmary medicine chest",
			null,
			"This wooden chest is divided into small compartments for bandages, thread, salve pots, and medicine vessels. The lid is plain but sturdy, with scratches from repeated opening. It is heavy enough to belong in an infirmary more than on a traveller’s belt.",
			SizeCategory.Large,
			ItemQuality.Good,
			9500.0,
			260.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Standard Medicine",
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Apothecary Goods"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"FieldMedkit",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_monastic_infirmary_chest",
			"chest",
			"a monastic infirmary chest",
			null,
			"A pale wooden chest holds ordered rows of cloth, simple salves, suture spools, and labelled packets. A modest cross is cut into the lid rather than painted brightly. The whole piece is careful, clean, and practical, suited to a monastery sickroom.",
			SizeCategory.Large,
			ItemQuality.Good,
			11000.0,
			300.0m,
			true,
			false,
			"oak",
			[
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Surgical Supplies",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"FieldMedkit",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_apothecary_remedy_case",
			"case",
			"an apothecary remedy case",
			null,
			"This case opens into small cells for vials, paper packets, pellets, and salve pots. The inner dividers are stained by herbs and resin, and the leather outer shell has a careful flap closure. It is meant for prepared remedies rather than surgery.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1350.0,
			96.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_caravan_medicine_roll",
			"roll",
			"a caravan medicine roll",
			null,
			"A long leather roll carries compact dressings, bitter herb packets, waxed salves, and a few empty vessels. The ties are reinforced for travel, and the outside smells of dust, horse, and old spice. It is a practical kit for long roads and poor access to town healers.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			70.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Clean_Kit",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_shipboard_wound_chest",
			"chest",
			"a shipboard wound chest",
			null,
			"This small chest is tar-darkened at the seams and packed with cloth rolls, cord ties, salve pots, and scraping tools. The wood is heavy for its size, made to survive damp decks. Salt marks show on the iron fittings.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7200.0,
			150.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Surgical Supplies"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"FieldMedkit",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_horse_physick_kit",
			"kit",
			"a horse physick kit",
			null,
			"Broad dressings, strong ties, hoof cloths, and large cleaning pads are packed inside this worn leather kit. It is scaled for animals rather than people, with stout thread and heavy linen. Stable smells of leather, vinegar, and horsehair cling to it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			72.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Kit",
				"Bandage_Good"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_burn_care_box",
			"box",
			"a burn-care dressing box",
			null,
			"This small wooden box carries slick salve cloths, soft pads, and waxed packets for covering burned skin. The contents smell of honey, green herbs, and beeswax. Its compartments are shallow so the dressings do not crush each other.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1550.0,
			80.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Tend Kit",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Tend_Kit",
				"TopicalCream_Herbal_Burn_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_styptic_box",
			"box",
			"a styptic powder box",
			null,
			"A palm-sized wooden box contains folded pads, yarrow bundles, and pale mineral powder packets. The lid fits tightly to keep the contents dry. Everything inside is arranged for small cuts, shaving wounds, and quick pressure rather than broad nursing.",
			SizeCategory.Small,
			ItemQuality.Good,
			360.0,
			45.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"TopicalCream_Yarrow_Styptic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_fracture_care_bundle",
			"bundle",
			"a fracture-care splint bundle",
			null,
			"Straight wooden splints, linen pads, and tie cords are bundled in a heavy cloth sleeve. The pieces are pre-smoothed and ready to bind around a limb after setting. It is bulky but far faster than carving splints after the injury.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			24.0m,
			true,
			false,
			"willow",
			[
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_cupping_kit",
			"kit",
			"a small cupping kit",
			null,
			"Several small cups, a scraping cloth, and a simple lamp plate are wrapped inside a padded pouch. The cups are visible through gaps in the cloth, their rims polished smooth from repeated use. It is a healer’s tool kit, though it carries no automatic treatment of its own.",
			SizeCategory.Small,
			ItemQuality.Standard,
			700.0,
			34.0m,
			true,
			false,
			"glass",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Medical Tools / Cupping Vessel"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_cautery_kit",
			"kit",
			"a wrapped cautery kit",
			null,
			"A narrow iron, thick pads, and a little charcoal packet are secured in a heavy leather wrap. Dark scorch marks stain the inner layer where the hot tool has been set down before. The kit looks harsh, deliberate, and practical.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1500.0,
			48.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Cautery Iron"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);


		// 3.4 Durable medical tools
		CreateItem(
			"medieval_medical_linen_medicine_strainer",
			"strainer",
			"a linen medicine strainer",
			null,
			"Tight linen is stretched over a small willow hoop and stitched in place. The weave is close enough to catch grit, resin flecks, and steeped herbs from medicine liquids. The hoop has a small cord loop for hanging beside a hearth or apothecary bench.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Herbal Medicine",
				"Functions / Tools / Medical Tools / Medicine Strainer"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_stone_mortar_and_pestle",
			"mortar",
			"a stone mortar and pestle",
			null,
			"This compact stone mortar is paired with a blunt pestle worn smooth by grinding. Its bowl is darkened by old herbs and mineral powders, with a few chips along the rim. It is heavy enough to stay steady while crushing medicine stock.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1450.0,
			12.0m,
			true,
			false,
			"stone",
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_ceramic_apothecary_mortar",
			"mortar",
			"a glazed ceramic apothecary mortar",
			null,
			"This small glazed mortar has a pale bowl and a matching pestle with a rounded end. Herbal stains have settled into hairline scratches inside the glaze. It is cleaner and finer than rough stone, suited to an apothecary counter.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			28.0m,
			true,
			false,
			"glazed ceramic",
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bronze_ointment_spatula",
			"spatula",
			"a bronze ointment spatula",
			null,
			"A slim bronze spatula has one flattened end for lifting salve and one rounded end for stirring. The handle is narrow, plain, and slightly dark from age. It is small enough to live inside a medicine roll.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			10.0m,
			true,
			false,
			"bronze",
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Medical Tools / Ointment Spatula"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bone_ointment_spatula",
			"spatula",
			"a polished bone ointment spatula",
			null,
			"This spatula is carved from pale bone and rubbed smooth along its rounded blade. A shallow groove down the handle gives the fingers purchase when working with slick salves. It is light, cheap, and easy to clean by scraping.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			5.0m,
			true,
			false,
			"bone",
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Apothecary Goods",
				"Functions / Tools / Medical Tools / Ointment Spatula"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_horn_dosing_spoon",
			"spoon",
			"a small horn dosing spoon",
			null,
			"A little spoon of polished horn has a shallow bowl and a short handle. The rim is smoothed thin enough for careful pouring, but not marked with exact measures. Its dark translucent colour gives it a humble apothecary look.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			6.0m,
			true,
			false,
			"horn",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_glass_cupping_vessel",
			"cup",
			"a small glass cupping vessel",
			null,
			"This rounded glass cup has a thick lip and a slightly smoky tint. The base is broad enough to sit upright, while the mouth is polished smooth. It is fragile but clear enough to show the skin beneath when pressed down.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			16.0m,
			true,
			false,
			"glass",
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Surgical Supplies",
				"Functions / Tools / Medical Tools / Cupping Vessel"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_clay_cupping_cup",
			"cup",
			"a clay cupping cup",
			null,
			"A small fired-clay cup has a rounded belly, a thick rim, and soot darkening near the base. It is opaque and sturdier than glass, with a rougher finish. A healer could warm it quickly over a flame.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			160.0,
			8.0m,
			true,
			false,
			"fired clay",
			[
				"Market / Professional Tools / Standard Tools",
				"Market / Medicine / Surgical Supplies",
				"Functions / Tools / Medical Tools / Cupping Vessel"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_leech_jar",
			"jar",
			"a small leech jar",
			null,
			"This glazed ceramic jar has a perforated lid tied down with cord. Water marks stain the inside, and the rim is broad enough for a hand to reach in carefully. It is a medical storage vessel rather than a drinking jar.",
			SizeCategory.Small,
			ItemQuality.Standard,
			750.0,
			22.0m,
			true,
			false,
			"glazed ceramic",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Surgical Supplies"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Pouch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bronze_scalpel",
			"scalpel",
			"a bronze surgical scalpel",
			null,
			"A small bronze blade is set into a plain handle and honed to a narrow working edge. The blade is leaf-shaped and darkened near the socket from age and oil. It is a surgical knife, not a weapon meant for battle.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			65.0,
			18.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Scalpel"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wrought_iron_surgical_knife",
			"knife",
			"a wrought-iron surgical knife",
			null,
			"This small wrought-iron knife has a narrow blade and a wrapped wooden grip. The edge is keen but plain, with tool marks still visible along the spine. It is built for incision and scraping rather than ornament.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			14.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Scalpel"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bronze_forceps",
			"forceps",
			"a pair of bronze forceps",
			null,
			"These bronze forceps are narrow, springy, and flattened at the tips. The metal shows polished bright places where fingers press them together. They are useful for grasping cloth, splinters, or small pieces during wound work.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			16.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Forceps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bronze_surgical_probe",
			"probe",
			"a bronze surgical probe",
			null,
			"A slender bronze probe has a rounded end at one side and a flattened spatulate end at the other. The shaft is straight, smooth, and slightly dark in the middle. It is made for feeling, lifting, and applying rather than cutting.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			12.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Surgical Probe"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bronze_arterial_clamp",
			"clamp",
			"a bronze arterial clamp",
			null,
			"This bronze clamp has flattened jaws and a crude locking bend behind them. It is not delicate, but the bite is firm and the spring returns when released. The tool is meant for serious wound work where hands alone are not enough.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			125.0,
			22.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Arterial Clamp"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_small_bonesaw",
			"saw",
			"a small surgical bonesaw",
			null,
			"A narrow metal saw blade is fixed into a wooden grip, with fine teeth running along one edge. The blade is shorter than a carpenter’s saw and easier to control close to the body. Oil darkens the join between metal and handle.",
			SizeCategory.Small,
			ItemQuality.Standard,
			360.0,
			34.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Bonesaw"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bronze_cautery_iron",
			"iron",
			"a bronze cautery iron",
			null,
			"A bronze working end sits on a long handle meant to keep the hand away from heat. The tip is flattened, dark, and smoke-stained, while the grip has been wrapped in old leather. It is a stern-looking surgical tool.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			28.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools",
				"Functions / Tools / Surgical Tools / Cautery Iron"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wrought_iron_cautery_brazier",
			"brazier",
			"a small cautery brazier",
			null,
			"This small wrought-iron brazier has a shallow fire bowl and three short feet. The rim is blackened by charcoal and ash, but there is no special incense fitting. It is meant to heat tools at the edge of a work space.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			36.0m,
			true,
			false,
			"wrought iron",
			[
				"Market / Medicine / Surgical Supplies",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_apothecary_balance_scale",
			"scale",
			"a small apothecary balance scale",
			null,
			"A compact balance scale folds into a little wooden case with two shallow pans. The beam is fine bronze and the weights sit in their own recesses. It is delicate enough for powders, resins, and measured medicine stock.",
			SizeCategory.Small,
			ItemQuality.Good,
			650.0,
			60.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_surgical_needle_case",
			"case",
			"a narrow surgical needle case",
			null,
			"This narrow case is carved from bone and fitted with a tight stopper. The inside is long enough for curved needles and slim probes, while the outside is polished by handling. A cord loop lets it hang inside a surgical roll.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			12.0m,
			true,
			false,
			"bone",
			[
				"Market / Medicine / Surgical Supplies",
				"Functions / Medical Treatment / Suture Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Container_Sachet"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_apothecary_sorting_tray",
			"tray",
			"a shallow apothecary sorting tray",
			null,
			"A shallow wooden tray is divided into small sections for herbs, powders, and tiny packets. The surface is stained by saffron, resin, and dark medicine dust. It is light enough to lift but broad enough to work on beside a mortar.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			850.0,
			18.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Professional Tools / Standard Tools"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Container_Tray"
			],
			null,
			null,
			null,
			null
		);


		// 3.5 Drug-bearing medicines, vessels, smoke, and fumigation
		CreateItem(
			"medieval_drug_honey_poultice_pot",
			"pot",
			"a pot of honey poultice",
			null,
			"A small fired-clay pot is capped with waxed cloth over a sticky honey poultice. Sweetness leaks through the wrap, mingled with a faint herbal smell. A flat spatula mark remains in the surface of the mixture.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			220.0,
			16.0m,
			true,
			false,
			"fired clay",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antiseptic Dressing"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Honey_Poultice"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_garlic_salve_pot",
			"pot",
			"a pungent garlic salve pot",
			null,
			"This thumb-sized ceramic pot is sealed with beeswax and cloth. The garlic smell pushes through the seal, sharp and earthy. Dark green flecks of herb cling beneath the rim where the salve has been stirred.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			190.0,
			14.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Garlic_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_aloe_burn_salve_jar",
			"jar",
			"a jar of aloe burn salve",
			null,
			"A small glazed jar contains a cool green salve under a neat cloth cap. The contents are glossy and smooth where the jar has been levelled. It smells mild, fresh, and faintly bitter.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			240.0,
			32.0m,
			true,
			false,
			"glazed ceramic",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Aloe_Burn_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_herbal_burn_salve_pot",
			"pot",
			"a pot of green burn salve",
			null,
			"This squat pot holds a thick green-brown salve with a resin sheen. A strip of linen is tied over the top and darkened by oil at the edges. The smell is grassy, bitter, and slightly smoky.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			230.0,
			36.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Herbal_Burn_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_yarrow_styptic_cake",
			"cake",
			"a pressed yarrow styptic cake",
			null,
			"Crushed yarrow has been pressed into a small greenish cake and wrapped in linen. The surface is rough and fibrous, with a bitter smell that clings to the cloth. It is shaped to be rubbed or broken into a dressing.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			40.0,
			8.0m,
			true,
			false,
			"yarrow",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Yarrow_Styptic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_alum_styptic_cone",
			"cone",
			"a pale alum styptic cone",
			null,
			"A small cone of pale mineral paste has dried hard and chalky. It is wrapped in parchment with the tip protected by a twist of cloth. The surface leaves faint white dust on the fingers when handled.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			10.0m,
			true,
			false,
			"alum",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Alum_Styptic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_yarrow_linen_styptic_roll",
			"roll",
			"a yarrow-linen styptic roll",
			null,
			"A narrow linen roll is dusted with crushed yarrow through its inner layers. The outside looks like a normal small bandage, but the green flecks show at the tucked end. It smells dry, bitter, and clean.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			75.0,
			11.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Bandage"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Good",
				"TopicalCream_Yarrow_Styptic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_alum_wool_pressure_pad",
			"pad",
			"an alum-dusted wool pressure pad",
			null,
			"Packed wool is wrapped in linen and dusted inside with pale alum powder. The pad is thicker than a common dressing and meant to be pressed down firmly. A dry mineral smell mixes with the wool.",
			SizeCategory.Small,
			ItemQuality.Standard,
			125.0,
			12.0m,
			true,
			false,
			"wool",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Styptic",
				"Functions / Medical Treatment / Tend Kit"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Tend_Single",
				"TopicalCream_Alum_Styptic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_honey_aloe_burn_dressing",
			"dressing",
			"a honey-and-aloe burn dressing",
			null,
			"This soft dressing is slick with a honeyed green salve and folded into a waxed wrapper. It is moist but not dripping, with a sweet smell beneath the plant bitterness. The pad is broad enough for a careful burn covering.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			95.0,
			28.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Bandage",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Bandage_Great",
				"TopicalCream_Aloe_Burn_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_garlic_honey_poultice_packet",
			"packet",
			"a garlic-honey poultice packet",
			null,
			"A waxed cloth packet holds a thick mixture of honey, garlic, and crushed herbs. The smell is both sweet and sharp, with a sticky stain darkening the fold. It is small enough for a single dressing rather than a jar of repeated use.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			65.0,
			9.0m,
			true,
			false,
			"honey",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Treatment Supplies",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"TopicalCream_Garlic_Salve"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_willow_bark_lozenge_packet",
			"packet",
			"a packet of willow-bark lozenges",
			null,
			"Several dark, bitter lozenges are wrapped in a small paper fold. Their surfaces are rough with powdered bark, honey, and a little spice to bind them. The packet is meant for measured swallowing rather than chewing as sweets.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			50.0,
			10.0m,
			true,
			false,
			"willow",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Willow_Bark_Tea"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mandrake_sleep_pellets",
			"packet",
			"a packet of mandrake pellets",
			null,
			"Tiny dark pellets sit inside a red-tied paper packet, each one rolled from powdered root and honey. The packet smells bitter and earthy even when closed. A careful hand has made the pellets nearly equal in size.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			35.0,
			36.0m,
			true,
			false,
			"mandrake",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Mandrake_Draught"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mint_lozenge_packet",
			"packet",
			"a packet of mint lozenges",
			null,
			"Pale green lozenges are folded into a little paper packet with mint crumbs caught at the creases. They smell fresh, sweet, and cooling. The pieces are plain medicine-shop fare rather than confectionery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			45.0,
			8.0m,
			true,
			false,
			"mint",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Mint_Infusion"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_ephedra_honey_cakes",
			"packet",
			"a packet of ephedra honey cakes",
			null,
			"Small honey-bound cakes are speckled with chopped ephedra and wrapped in oiled paper. The cakes are dry at the surface but still sticky at the edges. Their smell is grassy, sweet, and faintly resinous.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			18.0m,
			true,
			false,
			"ephedra",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Ephedra_Brew"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_foxglove_warning_beads",
			"packet",
			"a red-tied packet of foxglove beads",
			null,
			"Several tiny dark beads are tied into a warning-marked packet with red thread. The beads are very small, carefully rolled, and bitter-smelling. The presentation makes clear that these are not ordinary household lozenges.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			45.0m,
			true,
			false,
			"foxglove",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Foxglove_Tincture"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_poppy_latex_bolus",
			"bolus",
			"a dark poppy-latex bolus",
			null,
			"A small dark bolus is wrapped in oiled cloth and tied shut with thread. It feels resinous under the wrapping, and its bitter smell is heavy and unmistakable. The size is deliberately small, as though meant to be measured with care.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			25.0,
			50.0m,
			true,
			false,
			"resin",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Pill_Poppy_Latex_Draught"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mint_ginger_pastilles",
			"packet",
			"a packet of mint-ginger pastilles",
			null,
			"Round pastilles of mint, ginger, and honey are stacked in a paper fold. They smell warming and fresh at once, with ginger fibres visible in the surface. The packet is sized for a traveller or sickroom attendant.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			55.0,
			14.0m,
			true,
			false,
			"ginger",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Mint_and_Ginger_Tonic"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_theriac_electuary_ball",
			"ball",
			"a dark theriac electuary ball",
			null,
			"A glossy ball of dark electuary is wrapped in a square of oiled parchment. The smell is rich with honey, spice, bitterness, and resin. It is carefully rounded and sealed as a valued apothecary dose.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			40.0,
			75.0m,
			true,
			false,
			"spice",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antidote"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Theriac_Electuary"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_theriac_travel_packet",
			"packet",
			"a sealed theriac travel packet",
			null,
			"This small packet contains several pea-sized electuary doses separated by waxed folds. The outer wrapping is tied twice and smells strongly of spice. It is made for cautious travel use rather than an open shop jar.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			75.0,
			120.0m,
			true,
			false,
			"spice",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antidote"
			],
			[
				"Holdable",
				"Destroyable_Paper",
				"Pill_Theriac_Electuary"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_poppy_honey_sleep_bolus",
			"bolus",
			"a poppy-honey sleep bolus",
			null,
			"Honey, resin, and powdered herb have been rolled into a dark, tacky bolus. It is wrapped alone in oiled cloth and tied with a black thread. The smell is heavy, sweet, and bitter enough to discourage casual tasting.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			30.0,
			54.0m,
			true,
			false,
			"honey",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Pill_Poppy_Latex_Draught"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_empty_glass_medicine_vial",
			"vial",
			"an empty glass medicine vial",
			null,
			"This small glass vial has a narrow neck and a plain stopper fitted with wax around the rim. The glass is faintly green and thick for its size. It is empty, clean, and ready for a measured medicine.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			65.0,
			18.0m,
			true,
			false,
			"glass",
			[
				"Market / Medicine / Apothecary Goods"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Medicine_Vial_30ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_empty_ceramic_medicine_bottle",
			"bottle",
			"an empty ceramic medicine bottle",
			null,
			"A little ceramic bottle has a rounded shoulder, narrow mouth, and cork stopper under a cloth tie. The outer surface is unglazed except for a smooth lip. It is sturdy enough for travel and opaque enough to protect its contents from casual inspection.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			16.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Apothecary Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Bottle_100ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_empty_leather_medicine_flask",
			"flask",
			"an empty leather medicine flask",
			null,
			"This small leather flask has a wooden stopper and a waxed seam along one side. It is soft-sided, dark from oiling, and fitted with a cord loop. It is made for travelling medicine rather than table drink.",
			SizeCategory.Small,
			ItemQuality.Standard,
			260.0,
			20.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Apothecary Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Flask_250ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_horn_medicine_vial",
			"vial",
			"a small horn medicine vial",
			null,
			"A polished horn vial is capped with a tight wooden plug and sealed with a strip of cloth. The translucent brown walls hide most of the contents but catch light at the thin edges. It is a humble alternative to glass.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			14.0m,
			true,
			false,
			"horn",
			[
				"Market / Medicine / Apothecary Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Vial_30ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_willow_bark_tea_flask",
			"flask",
			"a flask of willow-bark tea",
			null,
			"A small stoppered flask is filled with a dark bitter tea. The outside is plain leather over a rigid liner, with a willow chip tied to the neck as a sign. The vessel smells faintly of bark and steeped water.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			22.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Willow_Bark_Tea_250ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mandrake_draught_bottle",
			"bottle",
			"a bottle of mandrake draught",
			null,
			"A small opaque ceramic bottle is stoppered and tied with a red warning thread. Bitter earthy smells cling around the waxed mouth. The bottle is compact and carefully sealed, meant for a measured draught rather than table use.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			190.0,
			58.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Mandrake_Draught_100ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mint_infusion_flask",
			"flask",
			"a flask of mint infusion",
			null,
			"This small leather flask carries a pale herbal infusion and a tied sprig of mint at the stopper. The vessel is travel-worn but clean, with wax pressed into the seam. A fresh green smell escapes when it is handled.",
			SizeCategory.Small,
			ItemQuality.Standard,
			270.0,
			18.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Mint_Infusion_250ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_ephedra_brew_flask",
			"flask",
			"a flask of ephedra brew",
			null,
			"A dark leather flask is stoppered tightly over a bitter herbal brew. Dried jointed twigs are tied to the neck for identification. The vessel is compact, practical, and clearly meant for a road kit or healer’s pack.",
			SizeCategory.Small,
			ItemQuality.Standard,
			285.0,
			34.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Ephedra_Brew_250ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_foxglove_tincture_vial",
			"vial",
			"a red-marked foxglove tincture vial",
			null,
			"This narrow glass vial is sealed with wax and tied with red thread around the neck. A dark tincture clings to the inside in a thin line. The small size and careful markings warn that it is a measured medicine.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			65.0m,
			true,
			false,
			"glass",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Medicine_Foxglove_Tincture_30ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_poppy_latex_draught_bottle",
			"bottle",
			"a bottle of poppy-latex draught",
			null,
			"A small stoppered bottle holds a darkened draught with a bitter resin smell at the mouth. The wax seal is thick and slightly stained. Its compact form suggests careful dosing rather than casual drinking.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			190.0,
			78.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Poppy_Latex_Draught_100ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mint_ginger_tonic_flask",
			"flask",
			"a flask of mint-and-ginger tonic",
			null,
			"This small flask is filled with a warming herbal tonic and tied with a narrow green cord. The stopper smells of mint and ginger even before opening. Its leather covering is worn smooth by travel.",
			SizeCategory.Small,
			ItemQuality.Standard,
			280.0,
			30.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Herbal Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Herbal Remedy"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Mint_and_Ginger_Tonic_250ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_theriac_syrup_bottle",
			"bottle",
			"a bottle of theriac syrup",
			null,
			"A small glazed bottle is sealed with wax over a dark, thick syrup. Spice, honey, and resin smells gather around the neck. The bottle is decorated only by a dark thread and a careful apothecary knot.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			210.0,
			120.0m,
			true,
			false,
			"glazed ceramic",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / High-Quality Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Antidote"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Theriac_Syrup_100ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_plain_medicine_bottle_set",
			"set",
			"a set of empty medicine bottles",
			null,
			"Three small ceramic bottles sit together in a padded linen wrap. Each bottle has a stopper, a cloth tie, and enough space for a simple mark or label. The set is empty and ready for an apothecary or infirmary shelf.",
			SizeCategory.Small,
			ItemQuality.Standard,
			520.0,
			42.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Apothecary Goods"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LContainer_Medicine_Bottle_100ml"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mandrake_smoke_cone",
			"cone",
			"a mandrake smoke cone",
			null,
			"A dark little cone of powdered root and resin is wrapped in thin paper. It smells earthy and bitter, with flecks of pale mandrake visible in the pressed surface. The cone is made for personal smoke rather than broad room fumigation.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			25.0,
			28.0m,
			true,
			false,
			"mandrake",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Fumigation"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Mandrake_Draught"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_henbane_smoke_roll",
			"roll",
			"a henbane smoke roll",
			null,
			"Dried henbane leaves are rolled into a narrow paper twist and tied at both ends. The roll is dark, brittle, and acrid-smelling even before it is lit. A black thread marks it apart from ordinary incense.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			30.0m,
			true,
			false,
			"henbane",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Fumigation"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Henbane_Smoke"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_bronchial_smoke_pellets",
			"packet",
			"a packet of bronchial smoke pellets",
			null,
			"Several small pellets of herb and resin sit in a waxed packet. They smell sharp, resinous, and green, with a little dust clinging to the paper. The pellets are meant to be smoked one at a time.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			35.0,
			22.0m,
			true,
			false,
			"resin",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Herbal Remedy",
				"Functions / Medical Treatment / Fumigation"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Bronchial_Smoke"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_soporific_smoke_cone",
			"cone",
			"a soporific smoke cone",
			null,
			"This small black cone is pressed from bitter herbs and resin and wrapped alone in oiled paper. Its smell is heavy, sweet, and unsettling. A dark warning knot closes the packet.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			42.0m,
			true,
			false,
			"resin",
			[
				"Market / Medicine / Apothecary Goods",
				"Market / Medicine / Herbal Medicine",
				"Functions / Medical Treatment / Fumigation"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Smokeable_Soporific_Fumes"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_plain_fumigation_pastilles",
			"packet",
			"a packet of fumigation pastilles",
			null,
			"Small brown pastilles of resin, herb, and charcoal are folded in a linen packet. They have a dry incense smell and crumble slightly at the edges. The packet is made as fuel stock for a fumigation vessel, not as a medicine by itself.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			80.0,
			12.0m,
			true,
			false,
			"resin",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Fumigation Stock",
				"Functions / Medical Treatment / Fumigation"
			],
			[
				"Holdable",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_resin_herb_fumigation_packet",
			"packet",
			"a resin-herb fumigation packet",
			null,
			"Chunks of resin and bitter herbs are mixed in a waxed paper packet. The pieces are irregular but dry, with charcoal dust darkening the fold. It is marked as fumigation stock rather than food or perfume.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			120.0,
			18.0m,
			true,
			false,
			"resin",
			[
				"Market / Medicine / Herbal Medicine",
				"Functions / Material Functions / Medical Craft Stock / Fumigation Stock",
				"Functions / Medical Treatment / Fumigation"
			],
			[
				"Holdable",
				"Destroyable_Paper"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_mandrake_fumigation_censer",
			"censer",
			"a bronze mandrake fumigation censer",
			null,
			"This small bronze censer has pierced vents and a darkened bowl for burning fumigation stock. Bitter earthy smoke stains the inner rim, and a short handle lets it be moved carefully. It is a drug-bearing fumigation vessel, not an ordinary incense ornament.",
			SizeCategory.Small,
			ItemQuality.Good,
			850.0,
			90.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"IncenseBurner_Mandrake_Draught"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_henbane_fumigation_bowl",
			"bowl",
			"a ceramic henbane fumigation bowl",
			null,
			"A squat ceramic bowl is pierced around the upper wall and blackened inside. The outside is plain, but a black cord around the foot warns that it is not ordinary household incense gear. Its shape keeps the smoke low and close.",
			SizeCategory.Small,
			ItemQuality.Good,
			780.0,
			86.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"IncenseBurner_Henbane_Smoke"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_bronchial_fumigation_censer",
			"censer",
			"a herbal bronchial fumigation censer",
			null,
			"This bronze censer is broad, low, and pierced with crescent vents around the lid. Old greenish smoke stains the seams where the lid lifts free. It is built for room fumigation with sharp herbal smoke.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			78.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"IncenseBurner_Bronchial_Smoke"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_soporific_sickroom_brazier",
			"brazier",
			"a soporific sickroom brazier",
			null,
			"A small bronze brazier stands on three feet with a perforated cover over the bowl. Dark, sweet-smelling soot marks the vents, and a black thread is tied to the handle. It is meant for controlled sickroom fumigation rather than cheerful household scent.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			130.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / High-Quality Medicine",
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"IncenseBurner_Soporific_Fumes"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_drug_portable_fumigation_casket",
			"casket",
			"a portable fumigation casket",
			null,
			"This small ceramic casket has holes cut through the lid and a fire-darkened cup inside. The outer walls are wrapped with a thin leather carrying strap. It is compact enough for travel while still functioning as a fumigation vessel.",
			SizeCategory.Small,
			ItemQuality.Standard,
			720.0,
			64.0m,
			true,
			false,
			"ceramic",
			[
				"Market / Medicine / Apothecary Goods",
				"Functions / Medical Treatment / Fumigation",
				"Market / Medicine / Herbal Medicine"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"IncenseBurner_Mandrake_Draught"
			],
			null,
			null,
			null,
			null
		);


		// 3.6 Mobility, casualty transport, and prosthetics
		CreateItem(
			"medieval_medical_padded_willow_splints",
			"splints",
			"a pair of padded willow splints",
			null,
			"Two straight willow boards are smoothed, drilled for ties, and padded with folded linen. The pair is light but stiff enough to hold a forearm or shin in place. Old pressure marks show where cords have been knotted before.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			420.0,
			14.0m,
			true,
			false,
			"willow",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_reed_finger_splints",
			"splints",
			"a bundle of reed finger splints",
			null,
			"Thin reed splints are bundled with narrow linen ties, each one trimmed for a finger or toe. The pieces are light, smooth, and slightly flexible. They look meant for small fractures rather than heavy limb binding.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			5.0m,
			true,
			false,
			"reed",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Limb_Immobilising"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_padded_leg_splints",
			"splints",
			"a pair of padded leg splints",
			null,
			"Long wooden splints are lined with wool padding and tied with broad linen straps. They are longer and heavier than arm splints, with enough stiffness for a lower leg. The padding is replaceable but worn smooth in places.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			950.0,
			24.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_linen_arm_sling",
			"sling",
			"a plain linen arm sling",
			null,
			"A broad triangle of linen is hemmed at the edges and fitted with two simple ties. The cloth is soft where it would cradle an arm against the body. It is plain enough for any infirmary or household chest.",
			SizeCategory.Small,
			ItemQuality.Standard,
			140.0,
			5.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Limb_Immobilising"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_leather_shoulder_sling",
			"sling",
			"a leather shoulder support sling",
			null,
			"This sling uses soft leather straps and a linen cradle to hold an injured arm tight against the chest. The shoulder band is broad, spreading weight more comfortably than a thin cord. It is made for longer wear than a simple cloth triangle.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			16.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Limb_Immobilising"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_bamboo_travel_splints",
			"splints",
			"a set of bamboo travel splints",
			null,
			"Split bamboo lengths are smoothed and stacked inside a cloth sleeve. The pieces are light, stiff, and easy to carry on the road. Linen cords threaded through the sleeve keep the set together.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			520.0,
			18.0m,
			true,
			false,
			"bamboo",
			[
				"Market / Medicine / Treatment Supplies",
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Splint",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Limb_Immobilising"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_forked_oak_crutch",
			"crutch",
			"a forked oak walking crutch",
			null,
			"This crutch is cut from oak with a natural fork padded by wrapped wool. The shaft is smoothed by hand and slightly polished where it is gripped. It is sturdy but heavier than a simple cane.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1250.0,
			18.0m,
			true,
			false,
			"oak",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Crutch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_padded_willow_crutch",
			"crutch",
			"a padded willow crutch",
			null,
			"A light willow crutch has a carved fork, a leather grip, and a linen pad tied across the top. The shaft flexes slightly but remains straight. It is easier to carry than oak and made for everyday convalescence.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			820.0,
			20.0m,
			true,
			false,
			"willow",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Crutch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_staff_crutch",
			"crutch",
			"a staff-like healer’s crutch",
			null,
			"This straight wooden crutch looks almost like a walking staff, with a side grip and a padded upper brace. The end is capped in worn leather to keep it from splitting. It is simple, portable, and useful on uneven roads.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			16.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Crutch"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_canvas_field_stretcher",
			"stretcher",
			"a canvas field stretcher",
			null,
			"Two wooden poles run through a broad linen-canvas bed stitched at the edges. The handles are worn smooth, and the cloth sags slightly between them. It is made for carrying wounded people over streets, camps, or fields.",
			SizeCategory.Large,
			ItemQuality.Standard,
			5200.0,
			28.0m,
			true,
			false,
			"linen",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"DragAid_Stretcher"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wooden_litter",
			"litter",
			"a simple wooden casualty litter",
			null,
			"This wooden litter has side rails, a slatted bed, and cord lashings to keep the frame from twisting. It is heavier than a canvas stretcher but steadier under a large patient. The rails show handwear from repeated carrying.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7600.0,
			45.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"DragAid_Stretcher"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_drag_sling",
			"sling",
			"a casualty drag sling",
			null,
			"A broad leather-and-linen sling is fitted with long hauling straps and a reinforced cradle. It is meant to pull someone clear when four carriers are not available. The underside is scuffed from contact with ground and floors.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			24.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"DragAid_Sling"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wooden_travois",
			"travois",
			"a wooden casualty travois",
			null,
			"Two long wooden poles are lashed together with crosspieces and a net of hemp cord. The frame narrows at one end for pulling by hand or beast. It is rough, field-made, and useful across open ground.",
			SizeCategory.Large,
			ItemQuality.Standard,
			6500.0,
			32.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"DragAid_Travois"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_winter_casualty_sled",
			"sled",
			"a low casualty sled",
			null,
			"This low sled has upturned wooden runners and a lashed linen bed between them. The front is fitted with a hauling rope, and the runners are polished by use. It is made for snow, mud, or smooth floors rather than stairs.",
			SizeCategory.Large,
			ItemQuality.Standard,
			8200.0,
			52.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"DragAid_Sled"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wounded_harness",
			"harness",
			"a wounded-man hauling harness",
			null,
			"A set of broad leather straps is fitted with buckles, loops, and a chest cradle. It lets rescuers drag or support a wounded person without gripping clothing alone. The leather is stiff but well oiled.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			36.0m,
			true,
			false,
			"leather",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"DragAid_Harness"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_wooden_peg_foot",
			"foot",
			"a simple wooden peg foot",
			null,
			"A carved wooden peg foot is shaped to strap below the leg with a leather cuff. The end is broad and worn flat from ground contact. It is functional in the simplest way, with no attempt to mimic toes.",
			SizeCategory.Small,
			ItemQuality.Standard,
			650.0,
			45.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_RFoot"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_left_lower_leg_prosthesis",
			"leg",
			"a left wooden lower-leg prosthesis",
			null,
			"This left lower-leg prosthesis is carved from wood and fitted with leather straps at the top. The shaft narrows toward a blunt walking end. Padding inside the cuff has been replaced more than once.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			95.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_LLowerLeg"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_right_lower_leg_prosthesis",
			"leg",
			"a right wooden lower-leg prosthesis",
			null,
			"This right lower-leg prosthesis combines a wooden shaft, leather cuff, and simple walking end. The fittings are practical rather than beautiful, with visible peg holes and cord repairs. It is meant for use, not display.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1800.0,
			95.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_RLowerLeg"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_left_plain_hand_prosthesis",
			"hand",
			"a plain left wooden hand",
			null,
			"A left wooden hand is carved into a fixed open shape and fitted with a leather wrist socket. The fingers do not move, but the palm is shaped enough to fill a sleeve or rest against an object. The surface is rubbed smooth but undecorated.",
			SizeCategory.Small,
			ItemQuality.Standard,
			420.0,
			60.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_LHand"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_right_hook_hand",
			"hook",
			"a right bronze hook hand",
			null,
			"A bronze hook is mounted into a leather wrist cuff with buckled straps. The hook is blunt at the inside curve and polished bright where it would catch cord or handle. It is one of the more practical artificial hands in the catalogue.",
			SizeCategory.Small,
			ItemQuality.Good,
			520.0,
			130.0m,
			true,
			false,
			"bronze",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Prosthetic_RHand_Functional"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_left_glass_eye",
			"eye",
			"a small left glass eye",
			null,
			"A small glass eye is shaped as a smooth convex piece with a dark painted centre. It is not lifelike in close inspection, but it has enough shine and colour to fill an empty socket under normal light. A padded scrap of cloth protects it.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			80.0m,
			true,
			false,
			"glass",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Prosthetic_LEye"
			],
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_medical_right_ornamental_foot",
			"foot",
			"an ornamental right prosthetic foot",
			null,
			"This right prosthetic foot is carved from wood with a stylised instep and simple toe marks. The shape is more presentable than practical, with leather straps set for fitting to a lower-leg piece. It has been polished to a dark sheen.",
			SizeCategory.Small,
			ItemQuality.Good,
			560.0,
			75.0m,
			true,
			false,
			"wood",
			[
				"Market / Medicine / Prosthetics and Mobility",
				"Functions / Medical Treatment / Prosthetic",
				"Functions / Medical Treatment / Mobility Aid"
			],
			[
				"Holdable",
				"Destroyable_WoodenHeavy",
				"Prosthetic_RFoot_Ornamental"
			],
			null,
			null,
			null,
			null
		);
	}
}

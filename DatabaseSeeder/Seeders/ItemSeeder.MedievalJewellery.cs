#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedMedievalJewelleryAndDevotionalGoods()
	{
		CreateItem(
			"medieval_jewellery_common_twisted_copper_ring",
			"ring",
			"a twisted copper ring",
			null,
			"A twisted copper ring is a compact band of warm reddish copper darkened in the recesses, made with a soft twist running around the outside rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. The worked parts of the twisted copper ring are decorative, but none suggest a moving catch or concealed compartment. The twisted copper ring reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			6.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_plain_bone_ring",
			"ring",
			"a plain bone ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale bone burnished by handling forms a steady circle, broken only by a shallow file mark near one edge. The worked parts of the plain bone ring are decorative, but none suggest a moving catch or concealed compartment. The piece would signal simple household taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			4.0m,
			true,
			false,
			"bone",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_horn_bead_bracelet",
			"bracelet",
			"a horn bead bracelet",
			null,
			"A horn bead bracelet is made to circle a limb, with horn with layered translucent streaks through it providing the visible body of the ornament. The edges are kept low and rounded, and beads of slightly different shade gives the piece a recognisable worked surface. Small handling marks cross the horn bead bracelet unevenly, especially where it would brush cloth or skin. The horn bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			34.0,
			8.0m,
			true,
			false,
			"horn",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_wooden_bead_necklace",
			"necklace",
			"a wooden bead necklace",
			null,
			"A wooden bead necklace is arranged for the throat or upper chest, where its line of wood would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The worked parts of the wooden bead necklace are decorative, but none suggest a moving catch or concealed compartment. The wooden bead necklace feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			6.0m,
			true,
			false,
			"wood",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_shell_chip_bracelet",
			"bracelet",
			"a shell chip bracelet",
			null,
			"A shell chip bracelet is made to circle a limb, with pale shell showing irregular growth lines providing the visible body of the ornament. The edges are kept low and rounded, and flat chips drilled close to the edge gives the piece a recognisable worked surface. Close inspection of the shell chip bracelet shows small tool marks rather than cast-perfect smoothness. The shell chip bracelet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			26.0,
			7.0m,
			true,
			false,
			"shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_cowrie_shell_anklet",
			"anklet",
			"a cowrie shell anklet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Small cowrie shells with smooth backs and pale mouths shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the anklet's highest places, leaving smooth shells strung mouth-outward easier to pick out. The cowrie shell anklet reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			9.0m,
			true,
			false,
			"cowrie shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_braided_hemp_bracelet",
			"bracelet",
			"a braided hemp bracelet",
			null,
			"A braided hemp bracelet is made to circle a limb, with coarse hemp cord with small stray fibres providing the visible body of the ornament. The edges are kept low and rounded, and three strands braided with a knotted end gives the piece a recognisable worked surface. Small handling marks cross the braided hemp bracelet unevenly, especially where it would brush cloth or skin. The braided hemp bracelet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			12.0,
			2.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_knotted_wool_wristband",
			"bracelet",
			"a knotted wool wristband",
			null,
			"A knotted wool wristband would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Close inspection of the knotted wool wristband shows small tool marks rather than cast-perfect smoothness. The knotted wool wristband carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			16.0,
			2.0m,
			true,
			false,
			"wool",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_small_wooden_child_ring",
			"ring",
			"a small wooden child ring",
			null,
			"At close range, small wooden child ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A rounded edge and very small opening draws the eye around the band. Small handling marks cross the small wooden child ring unevenly, especially where it would brush cloth or skin. The small wooden child ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			3.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Children's Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_clay_coloured_faience_bead_string",
			"necklace",
			"a clay-coloured faience bead string",
			null,
			"A clay-coloured faience bead string carries its value in the rhythm of its material: glazed faience with small pooling marks in the colour. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the necklace's highest places, leaving beads with glaze gathered in small dimples easier to pick out. The clay-coloured faience bead string carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			16.0m,
			true,
			false,
			"faience",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_little_shell_child_necklace",
			"necklace",
			"a little shell child necklace",
			null,
			"The little shell child necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale shell showing irregular growth lines, with small shells spaced on plain cord giving the eye a place to pause. The hidden side of the little shell child necklace is plainer, so attention stays on small shells spaced on plain cord and the visible finish. The piece would signal child or apprentice taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			28.0,
			5.0m,
			true,
			false,
			"shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Children's Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_rush_woven_finger_ring",
			"ring",
			"a rush-woven finger ring",
			null,
			"A rush-woven finger ring is a compact band of rush stems bent and plaited into springy loops, made with a woven overlap tucked beneath the band rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the rush-woven finger ring unevenly, especially where it would brush cloth or skin. The piece would signal temporary festival taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			3.0,
			1.0m,
			true,
			false,
			"rush",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_straw_festival_ring",
			"ring",
			"a straw festival ring",
			null,
			"A straw festival ring is a compact band of straw twisted into a pale, dry braid, made with dry stems twisted into a rough circle rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving dry stems twisted into a rough circle easier to pick out. The straw festival ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			2.0,
			1.0m,
			true,
			false,
			"straw",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_leather_token_bracelet",
			"bracelet",
			"a leather token bracelet",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A single punched hole near the knot breaks up the line of worked leather with creases at the folds. The hidden side of the leather token bracelet is plainer, so attention stays on a single punched hole near the knot and the visible finish. The piece would signal commoner taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			18.0,
			5.0m,
			true,
			false,
			"leather",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_jet_bead_keepsake_string",
			"necklace",
			"a jet bead keepsake string",
			null,
			"A jet bead keepsake string carries its value in the rhythm of its material: black jet polished until it gives back a quiet reflection. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the necklace's highest places, leaving black beads broken by one pale spacer easier to pick out. The jet bead keepsake string feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			44.0,
			22.0m,
			true,
			false,
			"jet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_common_small_amber_bead_charm",
			"pendant",
			"a small amber bead charm",
			null,
			"A small amber bead charm carries its value in the rhythm of its material: honey-coloured amber with cloudy threads caught inside. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Close inspection of the small amber bead charm shows small tool marks rather than cast-perfect smoothness. The small amber bead charm reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			18.0m,
			true,
			false,
			"amber",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_garland_dried_flower_neck_garland",
			"garland",
			"a dried flower neck garland",
			null,
			"The piece is light and pliant, built from dried flowers papery at the edges and brittle along the stems rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The dry pieces rustle faintly rather than bending back into shape. The dried flower neck garland leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_flower_neck_garland",
			"garland",
			"a wilted flower neck garland",
			null,
			"Fresh or faded, wilted flower neck garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The wilted flower neck garland leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_flower_neck_garland",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_flower_neck_garland",
			"garland",
			"a fresh flower neck garland",
			null,
			"Fresh or faded, fresh flower neck garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with a line of tiny punch marks interrupting the line of petals and leaves. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh flower neck garland reads as festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"fresh flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_flower_neck_garland",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_jasmine_garland",
			"garland",
			"a dried jasmine garland",
			null,
			"Fresh or faded, dried jasmine garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with a softened seam where the maker closed it interrupting the line of petals and leaves. The dry pieces rustle faintly rather than bending back into shape. The dried jasmine garland reads as temporary festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_jasmine_garland",
			"garland",
			"a wilted jasmine garland",
			null,
			"The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted jasmine garland is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_jasmine_garland",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_jasmine_garland",
			"garland",
			"a fresh jasmine garland",
			null,
			"The piece is light and pliant, built from jasmine flowers strung in small pale stars rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh jasmine garland feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"jasmine",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_jasmine_garland",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_marigold_garland",
			"garland",
			"a dried marigold garland",
			null,
			"The piece is light and pliant, built from dried flowers papery at the edges and brittle along the stems rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The dry pieces rustle faintly rather than bending back into shape. The piece would signal temporary festival taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_marigold_garland",
			"garland",
			"a wilted marigold garland",
			null,
			"The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted marigold garland feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_marigold_garland",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_marigold_garland",
			"garland",
			"a fresh marigold garland",
			null,
			"Fresh or faded, fresh marigold garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with a line of tiny punch marks interrupting the line of petals and leaves. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh marigold garland leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"marigold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_marigold_garland",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_rose_chaplet",
			"chaplet",
			"a dried rose chaplet",
			null,
			"A dried rose chaplet has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried rose chaplet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_rose_chaplet",
			"chaplet",
			"a wilted rose chaplet",
			null,
			"Fresh or faded, wilted rose chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a small flattened spot from long handling interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_rose_chaplet",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_rose_chaplet",
			"chaplet",
			"a fresh rose chaplet",
			null,
			"Fresh or faded, fresh rose chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh rose chaplet reads as festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"rose",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_rose_chaplet",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_ivy_wreath",
			"wreath",
			"a dried ivy wreath",
			null,
			"A dried ivy wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The piece would signal temporary festival taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"ivy",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Dried Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_ivy_wreath",
			"wreath",
			"a wilted ivy wreath",
			null,
			"Fresh or faded, wilted ivy wreath keeps the uneven look of something made by hand from living material. The binding is visible in places, with a softened seam where the maker closed it interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The wilted ivy wreath reads as temporary festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"ivy",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Flower Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_ivy_wreath",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_ivy_wreath",
			"wreath",
			"a fresh ivy wreath",
			null,
			"A fresh ivy wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh ivy wreath carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"ivy",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Fresh Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_ivy_wreath",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_laurel_wreath",
			"wreath",
			"a dried laurel wreath",
			null,
			"A dried laurel wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried laurel wreath feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"laurel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Dried Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_laurel_wreath",
			"wreath",
			"a wilted laurel wreath",
			null,
			"Fresh or faded, wilted laurel wreath keeps the uneven look of something made by hand from living material. The binding is visible in places, with a smooth hand-worn polish interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"laurel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Flower Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_laurel_wreath",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_laurel_wreath",
			"wreath",
			"a fresh laurel wreath",
			null,
			"The piece is light and pliant, built from laurel leaves laid in a clean repeating line rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh laurel wreath feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"laurel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Functions / Worn Items / Jewellery / Wreaths / Fresh Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_laurel_wreath",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_violet_wrist_garland",
			"garland",
			"a dried violet wrist garland",
			null,
			"A dried violet wrist garland has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried violet wrist garland leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_violet_wrist_garland",
			"garland",
			"a wilted violet wrist garland",
			null,
			"Fresh or faded, wilted violet wrist garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The wilted violet wrist garland carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_violet_wrist_garland",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_violet_wrist_garland",
			"garland",
			"a fresh violet wrist garland",
			null,
			"A fresh violet wrist garland is gathered from violets gathered into small dark-faced clusters, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh violet wrist garland reads as festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"violet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_violet_wrist_garland",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_blossom_anklet",
			"anklet",
			"a dried blossom anklet",
			null,
			"Fresh or faded, dried blossom anklet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a line of tiny punch marks interrupting the line of petals and leaves. The dry pieces rustle faintly rather than bending back into shape. The dried blossom anklet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Ankle_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_blossom_anklet",
			"anklet",
			"a wilted blossom anklet",
			null,
			"A wilted blossom anklet is gathered from wilted flowers with limp stems and darkening petal tips, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The drooping parts still show where the brighter fresh shape used to be. The wilted blossom anklet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Ankle_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_blossom_anklet",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_blossom_anklet",
			"anklet",
			"a fresh blossom anklet",
			null,
			"A fresh blossom anklet has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh blossom anklet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"blossom",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Ankle_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_blossom_anklet",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_daisy_head_garland",
			"garland",
			"a dried daisy head garland",
			null,
			"A dried daisy head garland is gathered from dried flowers papery at the edges and brittle along the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The dry pieces rustle faintly rather than bending back into shape. The dried daisy head garland leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_daisy_head_garland",
			"garland",
			"a wilted daisy head garland",
			null,
			"The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted daisy head garland feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_daisy_head_garland",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_daisy_head_garland",
			"garland",
			"a fresh daisy head garland",
			null,
			"The piece is light and pliant, built from daisies with pale petals radiating around yellow centres rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh daisy head garland reads as festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"daisy",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_daisy_head_garland",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_lotus_garland",
			"garland",
			"a dried lotus garland",
			null,
			"A dried lotus garland has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The dry pieces rustle faintly rather than bending back into shape. The dried lotus garland carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_lotus_garland",
			"garland",
			"a wilted lotus garland",
			null,
			"The piece is light and pliant, built from wilted flowers with limp stems and darkening petal tips rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The drooping parts still show where the brighter fresh shape used to be. The wilted lotus garland feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_lotus_garland",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_lotus_garland",
			"garland",
			"a fresh lotus garland",
			null,
			"A fresh lotus garland is gathered from lotus petals folded and tied into a broad soft chain, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh lotus garland leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"lotus flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_lotus_garland",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_lily_chaplet",
			"chaplet",
			"a dried lily chaplet",
			null,
			"Fresh or faded, dried lily chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a softened seam where the maker closed it interrupting the line of petals and leaves. The dry pieces rustle faintly rather than bending back into shape. The dried lily chaplet feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_lily_chaplet",
			"chaplet",
			"a wilted lily chaplet",
			null,
			"Fresh or faded, wilted lily chaplet keeps the uneven look of something made by hand from living material. The binding is visible in places, with a smooth hand-worn polish interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_lily_chaplet",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_lily_chaplet",
			"chaplet",
			"a fresh lily chaplet",
			null,
			"The piece is light and pliant, built from lily petals bound carefully so their long curves are not crushed rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh lily chaplet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"lily",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_lily_chaplet",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_garland_dried_chamomile_wrist_garland",
			"garland",
			"a dried chamomile wrist garland",
			null,
			"A dried chamomile wrist garland is gathered from dried flowers papery at the edges and brittle along the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The dry pieces rustle faintly rather than bending back into shape. The dried chamomile wrist garland reads as temporary festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Substandard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Dried Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			"Final dried morph target for a short-lived organic jewellery chain."
		);

		CreateItem(
			"medieval_jewellery_garland_wilted_chamomile_wrist_garland",
			"garland",
			"a wilted chamomile wrist garland",
			null,
			"Fresh or faded, wilted chamomile wrist garland keeps the uneven look of something made by hand from living material. The binding is visible in places, with faint tool chatter along the inner face interrupting the line of petals and leaves. The drooping parts still show where the brighter fresh shape used to be. The piece would signal temporary festival taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Poor,
			28.0,
			1.0m,
			true,
			false,
			"wilted flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Flower Garlands",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_dried_chamomile_wrist_garland",
			null,
			System.TimeSpan.FromDays(1.0),
			null,
			"Intermediate wilted morph target; can dry further if retained."
		);

		CreateItem(
			"medieval_jewellery_garland_fresh_chamomile_wrist_garland",
			"garland",
			"a fresh chamomile wrist garland",
			null,
			"A fresh chamomile wrist garland is gathered from chamomile, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. A few petals are already bruised at the edges, but the piece still reads as freshly made. The fresh chamomile wrist garland carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"chamomile",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Functions / Worn Items / Jewellery / Garlands / Fresh Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Wrist_Garland",
				"Destroyable_Clothing"
			],
			"medieval_jewellery_garland_wilted_chamomile_wrist_garland",
			null,
			System.TimeSpan.FromHours(6.0),
			null,
			"Fresh short-lived organic jewellery; use the listed timed morph target."
		);

		CreateItem(
			"medieval_jewellery_ring_thin_copper_ring",
			"ring",
			"a thin copper ring",
			null,
			"A thin copper ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as warm reddish copper darkened in the recesses, with the inner surface left plain and comfortable. Fine wear gathers around the ring's highest places, leaving a faint solder seam easier to pick out. The thin copper ring feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			7.0,
			5.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_hammered_bronze_ring",
			"ring",
			"a hammered bronze ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Brown-gold bronze with rubbed high points forms a steady circle, broken only by hammer facets that do not quite line up. Fine wear gathers around the ring's highest places, leaving hammer facets that do not quite line up easier to pick out. The hammered bronze ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			10.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_plain_pewter_ring",
			"ring",
			"a plain pewter ring",
			null,
			"A plain pewter ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as soft grey pewter with a low satin sheen, with the inner surface left plain and comfortable. Small handling marks cross the plain pewter ring unevenly, especially where it would brush cloth or skin. The plain pewter ring feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			7.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_brass_twist_ring",
			"ring",
			"a brass twist ring",
			null,
			"At close range, brass twist ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. Two visible strands crossing at the front draws the eye around the band. Close inspection of the brass twist ring shows small tool marks rather than cast-perfect smoothness. The brass twist ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			13.0,
			12.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_narrow_iron_ring",
			"ring",
			"a narrow iron ring",
			null,
			"A narrow iron ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as dark wrought iron with a waxed, workmanlike finish, with the inner surface left plain and comfortable. Close inspection of the narrow iron ring shows small tool marks rather than cast-perfect smoothness. The narrow iron ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			11.0,
			4.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_silver_wire_ring",
			"ring",
			"a silver wire ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by a wire loop wound twice around itself. Small handling marks cross the silver wire ring unevenly, especially where it would brush cloth or skin. The silver wire ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			9.0,
			42.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_sterling_silver_band",
			"ring",
			"a sterling silver band",
			null,
			"A sterling silver band keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as clean, pale silver with a crisp shine, with the inner surface left plain and comfortable. The worked parts of the sterling silver band are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			11.0,
			58.0m,
			true,
			false,
			"sterling silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_silver_gilt_finger_ring",
			"ring",
			"a silver-gilt finger ring",
			null,
			"At close range, silver-gilt finger ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A worn paler edge beneath the gilding draws the eye around the band. The worked parts of the silver-gilt finger ring are decorative, but none suggest a moving catch or concealed compartment. The silver-gilt finger ring feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			160.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_small_gold_ring",
			"ring",
			"a small gold ring",
			null,
			"A small gold ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the small gold ring without making it look neglected. The small gold ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			9.0,
			260.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_electrum_finger_ring",
			"ring",
			"an electrum finger ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale yellow electrum, neither fully silver nor fully gold in colour forms a steady circle, broken only by a pale yellow sheen. Fine wear gathers around the ring's highest places, leaving a pale yellow sheen easier to pick out. The electrum finger ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			210.0m,
			true,
			false,
			"electrum",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_carved_bone_ring",
			"ring",
			"a carved bone ring",
			null,
			"A carved bone ring is a compact band of pale bone burnished by handling, made with shallow cross-cut scoring rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving shallow cross-cut scoring easier to pick out. The piece would signal commoner taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			8.0m,
			true,
			false,
			"bone",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_dark_jet_ring",
			"ring",
			"a dark jet ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Black jet polished until it gives back a quiet reflection forms a steady circle, broken only by a black surface polished like still water. Close inspection of the dark jet ring shows small tool marks rather than cast-perfect smoothness. The dark jet ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			7.0,
			28.0m,
			true,
			false,
			"jet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_amber_set_bronze_ring",
			"ring",
			"an amber-set bronze ring",
			null,
			"An amber-set bronze ring is a compact band of brown-gold bronze with rubbed high points, made with a raised amber bead rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Close inspection of the amber-set bronze ring shows small tool marks rather than cast-perfect smoothness. The amber-set bronze ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			16.0,
			70.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_coral_set_silver_ring",
			"ring",
			"a coral-set silver ring",
			null,
			"A coral-set silver ring is a compact band of pale silver brightened at its raised surfaces, made with a small coral cabochon rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving a small coral cabochon easier to pick out. The coral-set silver ring feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			14.0,
			110.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_garnet_set_gold_ring",
			"ring",
			"a garnet-set gold ring",
			null,
			"A garnet-set gold ring is a compact band of deep yellow gold with a warm, steady shine, made with visible garnet detail worked into the public face rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the garnet-set gold ring unevenly, especially where it would brush cloth or skin. The garnet-set gold ring reads as noble adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			13.0,
			420.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_carnelisilver_ring",
			"ring",
			"a carnelian silver ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by visible carnelian detail worked into the public face. The hidden side of the carnelian silver ring is plainer, so attention stays on visible carnelian detail worked into the public face and the visible finish. The carnelian silver ring reads as merchant adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			15.0,
			95.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_turquoise_brass_ring",
			"ring",
			"a turquoise brass ring",
			null,
			"A turquoise brass ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as yellow brass polished brighter on the edges, with the inner surface left plain and comfortable. The hidden side of the turquoise brass ring is plainer, so attention stays on visible turquoise detail worked into the public face and the visible finish. The turquoise brass ring feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			14.0,
			55.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_rock_crystal_ring",
			"ring",
			"a rock crystal ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by visible rock crystal detail worked into the public face. Small handling marks cross the rock crystal ring unevenly, especially where it would brush cloth or skin. The rock crystal ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			14.0,
			150.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_lapis_signet_like_ring",
			"ring",
			"a lapis signet-like ring",
			null,
			"A lapis signet-like ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. The worked parts of the lapis signet-like ring are decorative, but none suggest a moving catch or concealed compartment. The lapis signet-like ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			130.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_jade_thumb_ring",
			"ring",
			"a jade thumb ring",
			null,
			"A jade thumb ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as green jade smoothed to a cool waxy finish, with the inner surface left plain and comfortable. Small handling marks cross the jade thumb ring unevenly, especially where it would brush cloth or skin. The jade thumb ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			22.0,
			120.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_agate_bead_ring",
			"ring",
			"an agate bead ring",
			null,
			"An agate bead ring is a compact band of agate, made with natural banding through the stone rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the agate bead ring unevenly, especially where it would brush cloth or skin. The agate bead ring reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			35.0m,
			true,
			false,
			"agate",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_bloodstone_signet_ring",
			"ring",
			"a bloodstone signet ring",
			null,
			"At close range, bloodstone signet ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A dark flat bezel draws the eye around the band. The worked parts of the bloodstone signet ring are decorative, but none suggest a moving catch or concealed compartment. The bloodstone signet ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			115.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_moonstone_gold_ring",
			"ring",
			"a moonstone gold ring",
			null,
			"A moonstone gold ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the moonstone gold ring without making it look neglected. The moonstone gold ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			13.0,
			360.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_onyx_silver_ring",
			"ring",
			"an onyx silver ring",
			null,
			"An onyx silver ring is a compact band of pale silver brightened at its raised surfaces, made with visible onyx detail worked into the public face rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. The worked parts of the onyx silver ring are decorative, but none suggest a moving catch or concealed compartment. The onyx silver ring reads as merchant adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			14.0,
			90.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_rose_quartz_love_ring",
			"ring",
			"a rose quartz love ring",
			null,
			"At close range, rose quartz love ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. Visible quartz detail worked into the public face draws the eye around the band. Fine wear gathers around the ring's highest places, leaving visible quartz detail worked into the public face easier to pick out. The rose quartz love ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			13.0,
			75.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_amethyst_court_ring",
			"ring",
			"an amethyst court ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Deep yellow gold with a warm, steady shine forms a steady circle, broken only by visible amethyst detail worked into the public face. Fine wear gathers around the ring's highest places, leaving visible amethyst detail worked into the public face easier to pick out. The amethyst court ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			15.0,
			480.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_sapphire_court_ring",
			"ring",
			"a sapphire court ring",
			null,
			"A sapphire court ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Small handling marks cross the sapphire court ring unevenly, especially where it would brush cloth or skin. The sapphire court ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			14.0,
			720.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_emerald_court_ring",
			"ring",
			"an emerald court ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Deep yellow gold with a warm, steady shine forms a steady circle, broken only by visible emerald detail worked into the public face. Small handling marks cross the emerald court ring unevenly, especially where it would brush cloth or skin. The emerald court ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			14.0,
			760.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_ruby_court_ring",
			"ring",
			"a ruby court ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Deep yellow gold with a warm, steady shine forms a steady circle, broken only by visible ruby detail worked into the public face. The hidden side of the ruby court ring is plainer, so attention stays on visible ruby detail worked into the public face and the visible finish. The ruby court ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			14.0,
			780.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_motif_bronze_ring",
			"ring",
			"a $motif bronze ring",
			null,
			"A $motif bronze ring is a compact band of brown-gold bronze with rubbed high points, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving the selected motif set into the outward-facing surface easier to pick out. The $motif bronze ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			24.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_motif_silver_ring",
			"ring",
			"a $motif silver ring",
			null,
			"A $motif silver ring is a compact band of pale silver brightened at its raised surfaces, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving the selected motif set into the outward-facing surface easier to pick out. The $motif silver ring feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			75.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_motif_silver_gilt_ring",
			"ring",
			"a $motif silver-gilt ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Gold-washed silver, brighter on the raised places and paler at worn edges forms a steady circle, broken only by the selected motif set into the outward-facing surface. Fine wear gathers around the ring's highest places, leaving the selected motif set into the outward-facing surface easier to pick out. The $motif silver-gilt ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			180.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_motif_gold_ring",
			"ring",
			"a $motif gold ring",
			null,
			"A $motif gold ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as deep yellow gold with a warm, steady shine, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the $motif gold ring without making it look neglected. The $motif gold ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			390.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_motif_brass_ring",
			"ring",
			"a $motif brass ring",
			null,
			"A $motif brass ring is a compact band of yellow brass polished brighter on the edges, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the $motif brass ring unevenly, especially where it would brush cloth or skin. The $motif brass ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			28.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_motif_copper_ring",
			"ring",
			"a $motif copper ring",
			null,
			"A $motif copper ring is a compact band of warm reddish copper darkened in the recesses, made with the selected motif set into the outward-facing surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the $motif copper ring unevenly, especially where it would brush cloth or skin. The $motif copper ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			12.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_plain_merchant_signet_ring",
			"ring",
			"a plain merchant signet ring",
			null,
			"A plain merchant signet ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. Rubbing has brightened the exposed parts of the plain merchant signet ring without making it look neglected. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			130.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_MerchantSignetRing"
			],
			null,
			null,
			null,
			null,
			"Functional signet row; seal device belongs in skin or component metadata."
		);

		CreateItem(
			"medieval_jewellery_ring_personal_seal_ring",
			"ring",
			"a personal seal ring",
			null,
			"A personal seal ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as brown-gold bronze with rubbed high points, with the inner surface left plain and comfortable. The hidden side of the personal seal ring is plainer, so attention stays on a flat face prepared for an impression and the visible finish. The personal seal ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			90.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_PersonalSignetRing"
			],
			null,
			null,
			null,
			null,
			"Functional signet row; seal device belongs in skin or component metadata."
		);

		CreateItem(
			"medieval_jewellery_ring_noble_signet_ring",
			"ring",
			"a noble signet ring",
			null,
			"At close range, noble signet ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A flat face prepared for an impression draws the eye around the band. Small handling marks cross the noble signet ring unevenly, especially where it would brush cloth or skin. The noble signet ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			520.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_NobleSignetRing"
			],
			null,
			null,
			null,
			null,
			"Functional signet row; seal device belongs in skin or component metadata."
		);

		CreateItem(
			"medieval_jewellery_ring_ring_signet_of_silver_gilt",
			"ring",
			"a ring signet of silver-gilt",
			null,
			"A ring signet of silver-gilt is a compact band of gold-washed silver, brighter on the raised places and paler at worn edges, made with a flat face prepared for an impression rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the ring signet of silver-gilt unevenly, especially where it would brush cloth or skin. The ring signet of silver-gilt reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			240.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_RingSignet"
			],
			null,
			null,
			null,
			null,
			"Functional signet row; seal device belongs in skin or component metadata."
		);

		CreateItem(
			"medieval_jewellery_ring_brass_trade_signet_ring",
			"ring",
			"a brass trade signet ring",
			null,
			"A brass trade signet ring is a compact band of yellow brass polished brighter on the edges, made with a flat face prepared for an impression rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. The worked parts of the brass trade signet ring are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			76.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_MerchantSignetRing"
			],
			null,
			null,
			null,
			null,
			"Functional signet row; seal device belongs in skin or component metadata."
		);

		CreateItem(
			"medieval_jewellery_ring_bronze_household_signet",
			"ring",
			"a bronze household signet",
			null,
			"A bronze household signet keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as brown-gold bronze with rubbed high points, with the inner surface left plain and comfortable. Fine wear gathers around the ring's highest places, leaving a flat face prepared for an impression easier to pick out. The bronze household signet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			85.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_PersonalSignetRing"
			],
			null,
			null,
			null,
			null,
			"Functional signet row; seal device belongs in skin or component metadata."
		);

		CreateItem(
			"medieval_jewellery_ring_child_sized_copper_ring",
			"ring",
			"a child-sized copper ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Warm reddish copper darkened in the recesses forms a steady circle, broken only by a tiny opening and rounded edge. Close inspection of the child-sized copper ring shows small tool marks rather than cast-perfect smoothness. The child-sized copper ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			3.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Children's Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_paired_courtship_ring",
			"ring",
			"a paired courtship ring",
			null,
			"A paired courtship ring is a compact band of pale silver brightened at its raised surfaces, made with two shallow grooves running side by side rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving two shallow grooves running side by side easier to pick out. The paired courtship ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			64.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_knotwork_love_ring",
			"ring",
			"a knotwork love ring",
			null,
			"A knotwork love ring is a compact band of pale silver brightened at its raised surfaces, made with interlaced lines around the face rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Small handling marks cross the knotwork love ring unevenly, especially where it would brush cloth or skin. The knotwork love ring reads as merchant adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			11.0,
			72.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_blackened_niello_ring",
			"ring",
			"a blackened niello ring",
			null,
			"A blackened niello ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. The worked parts of the blackened niello ring are decorative, but none suggest a moving catch or concealed compartment. The blackened niello ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			130.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_gilded_copper_ring",
			"ring",
			"a gilded copper ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Coppery red metal visible beneath worn gilt edges forms a steady circle, broken only by gold colour worn thin near the palm side. Fine wear gathers around the ring's highest places, leaving gold colour worn thin near the palm side easier to pick out. The gilded copper ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			38.0m,
			true,
			false,
			"gilded copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_gilded_bronze_ring",
			"ring",
			"a gilded bronze ring",
			null,
			"A gilded bronze ring is a compact band of bronze showing through a thin golden surface at nicks and bends, made with a small raised boss on the front rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Close inspection of the gilded bronze ring shows small tool marks rather than cast-perfect smoothness. The gilded bronze ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			13.0,
			46.0m,
			true,
			false,
			"gilded bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_mother_of_pearl_ring",
			"ring",
			"a mother-of-pearl ring",
			null,
			"A mother-of-pearl ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as mother-of-pearl flashing softly under the surface, with the inner surface left plain and comfortable. Close inspection of the mother-of-pearl ring shows small tool marks rather than cast-perfect smoothness. The mother-of-pearl ring reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			8.0,
			110.0m,
			true,
			false,
			"mother-of-pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_conch_shell_ring",
			"ring",
			"a conch shell ring",
			null,
			"At close range, conch shell ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. Pink-white shell smoothed into a narrow hoop draws the eye around the band. The worked parts of the conch shell ring are decorative, but none suggest a moving catch or concealed compartment. The conch shell ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			7.0,
			18.0m,
			true,
			false,
			"conch shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_cowrie_shell_ring",
			"ring",
			"a cowrie shell ring",
			null,
			"At close range, cowrie shell ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A shell cap lashed onto a small band draws the eye around the band. The hidden side of the cowrie shell ring is plainer, so attention stays on a shell cap lashed onto a small band and the visible finish. The cowrie shell ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			10.0m,
			true,
			false,
			"cowrie shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_double_wire_silver_ring",
			"ring",
			"a double wire silver ring",
			null,
			"A double wire silver ring keeps its decoration low to the finger, without raised parts that would snag easily. The material shows as pale silver brightened at its raised surfaces, with the inner surface left plain and comfortable. Small handling marks cross the double wire silver ring unevenly, especially where it would brush cloth or skin. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			70.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_heavy_gold_thumb_ring",
			"ring",
			"a heavy gold thumb ring",
			null,
			"At close range, heavy gold thumb ring shows the compromises of hand work: polish where it is touched, duller texture where it is not. A broad smooth curve draws the eye around the band. Small handling marks cross the heavy gold thumb ring unevenly, especially where it would brush cloth or skin. The heavy gold thumb ring feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			650.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_stamped_brass_ring",
			"ring",
			"a stamped brass ring",
			null,
			"A stamped brass ring is a compact band of yellow brass polished brighter on the edges, made with little punched triangles around the band rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Rubbing has brightened the exposed parts of the stamped brass ring without making it look neglected. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			18.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_finish_silver_ring",
			"ring",
			"a $finish silver ring",
			null,
			"A $finish silver ring is a compact band of pale silver brightened at its raised surfaces, made with the chosen finish left deliberately visible on the worked surface rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Fine wear gathers around the ring's highest places, leaving the chosen finish left deliberately visible on the worked surface easier to pick out. The $finish silver ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			82.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_MetalFinish"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ring_shape_gem_ring",
			"ring",
			"a $shape gem ring",
			null,
			"The ring is small, but the maker has given its outer face enough attention to keep it from looking like scrap. Pale silver brightened at its raised surfaces forms a steady circle, broken only by the chosen shape giving the object its main outline. Fine wear gathers around the ring's highest places, leaving the chosen shape giving the object its main outline easier to pick out. The $shape gem ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			14.0,
			145.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape",
				"Variable_Gem"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_blue_glass_bead_necklace",
			"necklace",
			"a blue glass bead necklace",
			null,
			"The blue glass bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is coloured glass polished smooth enough to glow at the edges, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. Close inspection of the blue glass bead necklace shows small tool marks rather than cast-perfect smoothness. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			34.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_faience_bead_necklace",
			"necklace",
			"a faience bead necklace",
			null,
			"The faience bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is glazed faience with small pooling marks in the colour, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The hidden side of the faience bead necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The faience bead necklace reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			28.0m,
			true,
			false,
			"faience",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_amber_bead_necklace",
			"necklace",
			"an amber bead necklace",
			null,
			"The amber bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is honey-coloured amber with cloudy threads caught inside, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The hidden side of the amber bead necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			120.0m,
			true,
			false,
			"amber",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_jet_bead_necklace",
			"necklace",
			"a jet bead necklace",
			null,
			"Seen flat, jet bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Rubbing has brightened the exposed parts of the jet bead necklace without making it look neglected. The jet bead necklace leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			42.0m,
			true,
			false,
			"jet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_coral_bead_necklace",
			"necklace",
			"a coral bead necklace",
			null,
			"A coral bead necklace is arranged for the throat or upper chest, where its line of coral beads with a warm sea-worn surface would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Close inspection of the coral bead necklace shows small tool marks rather than cast-perfect smoothness. The coral bead necklace feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			150.0m,
			true,
			false,
			"coral",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_bone_bead_necklace",
			"necklace",
			"a bone bead necklace",
			null,
			"Seen flat, bone bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the bone bead necklace are decorative, but none suggest a moving catch or concealed compartment. The bone bead necklace leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			12.0m,
			true,
			false,
			"bone",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_horn_bead_necklace",
			"necklace",
			"a horn bead necklace",
			null,
			"The horn bead necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is horn with layered translucent streaks through it, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The worked parts of the horn bead necklace are decorative, but none suggest a moving catch or concealed compartment. The horn bead necklace reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			14.0m,
			true,
			false,
			"horn",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_cowrie_shell_necklace",
			"necklace",
			"a cowrie shell necklace",
			null,
			"A cowrie shell necklace is arranged for the throat or upper chest, where its line of small cowrie shells with smooth backs and pale mouths would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the cowrie shell necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The cowrie shell necklace reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			16.0m,
			true,
			false,
			"cowrie shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_conch_shell_necklace",
			"necklace",
			"a conch shell necklace",
			null,
			"The conch shell necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is cut conch shell with creamy curves and warmer pinkish edges, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The worked parts of the conch shell necklace are decorative, but none suggest a moving catch or concealed compartment. The conch shell necklace leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			24.0m,
			true,
			false,
			"conch shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_mother_of_pearl_necklace",
			"necklace",
			"a mother-of-pearl necklace",
			null,
			"The mother-of-pearl necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is mother-of-pearl flashing softly under the surface, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The hidden side of the mother-of-pearl necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			130.0m,
			true,
			false,
			"mother-of-pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_small_pearl_necklace",
			"necklace",
			"a small pearl necklace",
			null,
			"The small pearl necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is small pearls with uneven but gentle lustre, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. The worked parts of the small pearl necklace are decorative, but none suggest a moving catch or concealed compartment. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			240.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_lapis_bead_necklace",
			"necklace",
			"a lapis bead necklace",
			null,
			"Seen flat, lapis bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the lapis bead necklace are decorative, but none suggest a moving catch or concealed compartment. The lapis bead necklace feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			180.0m,
			true,
			false,
			"lapis lazuli",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_carnelibead_necklace",
			"necklace",
			"a carnelian bead necklace",
			null,
			"Seen flat, carnelian bead necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Close inspection of the carnelian bead necklace shows small tool marks rather than cast-perfect smoothness. The carnelian bead necklace is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			90.0m,
			true,
			false,
			"carnelian",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_turquoise_bead_necklace",
			"necklace",
			"a turquoise bead necklace",
			null,
			"A turquoise bead necklace carries its value in the rhythm of its material: blue-green turquoise with fine darker lines. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the necklace's highest places, leaving beads chosen more for visible rhythm than identical size easier to pick out. The turquoise bead necklace reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			150.0m,
			true,
			false,
			"turquoise",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_jade_bead_necklace",
			"necklace",
			"a jade bead necklace",
			null,
			"A jade bead necklace is arranged for the throat or upper chest, where its line of green jade smoothed to a cool waxy finish would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the jade bead necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The jade bead necklace reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			210.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_rock_crystal_necklace",
			"necklace",
			"a rock crystal necklace",
			null,
			"Seen flat, rock crystal necklace shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The hidden side of the rock crystal necklace is plainer, so attention stays on beads chosen more for visible rhythm than identical size and the visible finish. The rock crystal necklace carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			170.0m,
			true,
			false,
			"rock crystal",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_agate_bead_string",
			"necklace",
			"an agate bead string",
			null,
			"An agate bead string carries its value in the rhythm of its material: agate. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Small handling marks cross the agate bead string unevenly, especially where it would brush cloth or skin. The agate bead string feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			46.0m,
			true,
			false,
			"agate",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_jasper_bead_string",
			"necklace",
			"a jasper bead string",
			null,
			"A jasper bead string carries its value in the rhythm of its material: jasper. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Close inspection of the jasper bead string shows small tool marks rather than cast-perfect smoothness. The jasper bead string feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			42.0m,
			true,
			false,
			"jasper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_chalcedony_bead_string",
			"necklace",
			"a chalcedony bead string",
			null,
			"A chalcedony bead string is arranged for the throat or upper chest, where its line of chalcedony would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Close inspection of the chalcedony bead string shows small tool marks rather than cast-perfect smoothness. The chalcedony bead string reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			48.0m,
			true,
			false,
			"chalcedony",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_quartz_bead_string",
			"necklace",
			"a quartz bead string",
			null,
			"The quartz bead string falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is quartz, with beads chosen more for visible rhythm than identical size giving the eye a place to pause. Fine wear gathers around the necklace's highest places, leaving beads chosen more for visible rhythm than identical size easier to pick out. The quartz bead string feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			34.0m,
			true,
			false,
			"quartz",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Bead Strings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_plain_bronze_chain",
			"chain",
			"a plain bronze chain",
			null,
			"Seen flat, plain bronze chain shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Close inspection of the plain bronze chain shows small tool marks rather than cast-perfect smoothness. The plain bronze chain reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			55.0,
			44.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_narrow_silver_chain",
			"chain",
			"a narrow silver chain",
			null,
			"The narrow silver chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with links or knots set at slightly irregular intervals giving the eye a place to pause. Close inspection of the narrow silver chain shows small tool marks rather than cast-perfect smoothness. The narrow silver chain leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			48.0,
			115.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_gold_link_chain",
			"chain",
			"a gold link chain",
			null,
			"A gold link chain is arranged for the throat or upper chest, where its line of deep yellow gold with a warm, steady shine would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Rubbing has brightened the exposed parts of the gold link chain without making it look neglected. The gold link chain carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			62.0,
			520.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_silver_gilt_collar_chain",
			"chain",
			"a silver-gilt collar chain",
			null,
			"A silver-gilt collar chain is arranged for the throat or upper chest, where its line of gold-washed silver, brighter on the raised places and paler at worn edges would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the silver-gilt collar chain is plainer, so attention stays on links or knots set at slightly irregular intervals and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			80.0,
			260.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_brass_merchant_chain",
			"chain",
			"a brass merchant chain",
			null,
			"The brass merchant chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is yellow brass polished brighter on the edges, with links or knots set at slightly irregular intervals giving the eye a place to pause. The hidden side of the brass merchant chain is plainer, so attention stays on links or knots set at slightly irregular intervals and the visible finish. The brass merchant chain carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			76.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_heavy_court_chain",
			"chain",
			"a heavy court chain",
			null,
			"Seen flat, heavy court chain shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the heavy court chain are decorative, but none suggest a moving catch or concealed compartment. The heavy court chain reads as courtly adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			160.0,
			1100.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_double_strand_silver_necklace",
			"necklace",
			"a double-strand silver necklace",
			null,
			"The double-strand silver necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with links or knots set at slightly irregular intervals giving the eye a place to pause. Close inspection of the double-strand silver necklace shows small tool marks rather than cast-perfect smoothness. The double-strand silver necklace is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			82.0,
			180.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_small_pendant_chain",
			"chain",
			"a small pendant chain",
			null,
			"The small pendant chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with links or knots set at slightly irregular intervals giving the eye a place to pause. Small handling marks cross the small pendant chain unevenly, especially where it would brush cloth or skin. The small pendant chain carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			42.0,
			88.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_leather_token_necklace",
			"necklace",
			"a leather token necklace",
			null,
			"The leather token necklace falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is worked leather with creases at the folds, with links or knots set at slightly irregular intervals giving the eye a place to pause. Rubbing has brightened the exposed parts of the leather token necklace without making it look neglected. The leather token necklace reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			28.0,
			8.0m,
			true,
			false,
			"leather",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_silk_cord_necklace",
			"necklace",
			"a silk cord necklace",
			null,
			"A silk cord necklace carries its value in the rhythm of its material: silk cord with a soft light across its twist. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The worked parts of the silk cord necklace are decorative, but none suggest a moving catch or concealed compartment. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			18.0,
			18.0m,
			true,
			false,
			"silk",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_hemp_cord_pendant_string",
			"necklace",
			"a hemp cord pendant string",
			null,
			"The hemp cord pendant string falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is coarse hemp cord with small stray fibres, with links or knots set at slightly irregular intervals giving the eye a place to pause. The worked parts of the hemp cord pendant string are decorative, but none suggest a moving catch or concealed compartment. The hemp cord pendant string feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			12.0,
			3.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_gold_pendant_necklace",
			"necklace",
			"a gold pendant necklace",
			null,
			"A gold pendant necklace is arranged for the throat or upper chest, where its line of deep yellow gold with a warm, steady shine would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the gold pendant necklace is plainer, so attention stays on links or knots set at slightly irregular intervals and the visible finish. The gold pendant necklace carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			55.0,
			460.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_silver_crescent_pendant",
			"pendant",
			"a silver crescent pendant",
			null,
			"A silver crescent pendant carries its value in the rhythm of its material: pale silver brightened at its raised surfaces. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Rubbing has brightened the exposed parts of the silver crescent pendant without making it look neglected. The silver crescent pendant reads as merchant adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			22.0,
			70.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_rock_crystal_pendant",
			"pendant",
			"a rock crystal pendant",
			null,
			"A rock crystal pendant carries its value in the rhythm of its material: clear rock crystal with tiny inner veils. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Small handling marks cross the rock crystal pendant unevenly, especially where it would brush cloth or skin. The rock crystal pendant carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			24.0,
			140.0m,
			true,
			false,
			"rock crystal",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_black_jet_choker",
			"choker",
			"a black jet choker",
			null,
			"The black jet choker falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is black jet polished until it gives back a quiet reflection, with a close line meant to rest high on the throat giving the eye a place to pause. The hidden side of the black jet choker is plainer, so attention stays on a close line meant to rest high on the throat and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			40.0,
			38.0m,
			true,
			false,
			"jet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_red_silk_choker",
			"choker",
			"a red silk choker",
			null,
			"The red silk choker falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is silk cord with a soft light across its twist, with a close line meant to rest high on the throat giving the eye a place to pause. Small handling marks cross the red silk choker unevenly, especially where it would brush cloth or skin. The red silk choker reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			16.0,
			20.0m,
			true,
			false,
			"silk",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_silver_close_collar",
			"choker",
			"a silver close collar",
			null,
			"Seen flat, silver close collar shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Rubbing has brightened the exposed parts of the silver close collar without making it look neglected. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			65.0,
			150.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_cowrie_shell_choker",
			"choker",
			"a cowrie shell choker",
			null,
			"Seen flat, cowrie shell choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Fine wear gathers around the choker's highest places, leaving a close line meant to rest high on the throat easier to pick out. The piece would signal commoner taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			38.0,
			14.0m,
			true,
			false,
			"cowrie shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_gold_bead_choker",
			"choker",
			"a gold bead choker",
			null,
			"A gold bead choker carries its value in the rhythm of its material: deep yellow gold with a warm, steady shine. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The worked parts of the gold bead choker are decorative, but none suggest a moving catch or concealed compartment. The gold bead choker leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			58.0,
			430.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_coral_choker",
			"choker",
			"a coral choker",
			null,
			"Seen flat, coral choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the coral choker are decorative, but none suggest a moving catch or concealed compartment. The coral choker leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			52.0,
			145.0m,
			true,
			false,
			"coral",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_beadpattern_glass_choker",
			"choker",
			"a $beadpattern glass choker",
			null,
			"Seen flat, $beadpattern glass choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Fine wear gathers around the choker's highest places, leaving the chosen bead pattern breaking the line into visible rhythm easier to pick out. The $beadpattern glass choker reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			46.0,
			34.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_Glassware",
				"Variable_BeadPattern"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_pearl_court_choker",
			"choker",
			"a pearl court choker",
			null,
			"Seen flat, pearl court choker shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Small handling marks cross the pearl court choker unevenly, especially where it would brush cloth or skin. The pearl court choker leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			50.0,
			360.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chokers",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Choker",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_bronze_neck_ring",
			"torc",
			"a bronze neck ring",
			null,
			"A bronze neck ring carries its value in the rhythm of its material: brown-gold bronze with rubbed high points. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Fine wear gathers around the torc's highest places, leaving terminals that are finished more boldly than the back curve easier to pick out. The bronze neck ring reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.Small,
			ItemQuality.Good,
			150.0,
			62.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_twisted_silver_torc",
			"torc",
			"a twisted silver torc",
			null,
			"Seen flat, twisted silver torc shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The hidden side of the twisted silver torc is plainer, so attention stays on terminals that are finished more boldly than the back curve and the visible finish. The twisted silver torc feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Small,
			ItemQuality.Good,
			220.0,
			280.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_heavy_gold_torc",
			"torc",
			"a heavy gold torc",
			null,
			"A heavy gold torc is arranged for the throat or upper chest, where its line of deep yellow gold with a warm, steady shine would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. Rubbing has brightened the exposed parts of the heavy gold torc without making it look neglected. The heavy gold torc leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			1250.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_iron_neck_ring",
			"torc",
			"an iron neck ring",
			null,
			"The iron neck ring falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is dark wrought iron with a waxed, workmanlike finish, with terminals that are finished more boldly than the back curve giving the eye a place to pause. Rubbing has brightened the exposed parts of the iron neck ring without making it look neglected. The iron neck ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			30.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_gilded_bronze_neck_ring",
			"torc",
			"a gilded bronze neck ring",
			null,
			"A gilded bronze neck ring carries its value in the rhythm of its material: bronze showing through a thin golden surface at nicks and bends. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The hidden side of the gilded bronze neck ring is plainer, so attention stays on terminals that are finished more boldly than the back curve and the visible finish. The gilded bronze neck ring reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.Small,
			ItemQuality.Good,
			170.0,
			170.0m,
			true,
			false,
			"gilded bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Neck Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Neck_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_plain_copper_torc",
			"torc",
			"a plain copper torc",
			null,
			"The plain copper torc falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is warm reddish copper darkened in the recesses, with terminals that are finished more boldly than the back curve giving the eye a place to pause. Fine wear gathers around the torc's highest places, leaving terminals that are finished more boldly than the back curve easier to pick out. The plain copper torc leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Small,
			ItemQuality.Good,
			140.0,
			46.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_amber_ended_silver_torc",
			"torc",
			"an amber-ended silver torc",
			null,
			"An amber-ended silver torc is arranged for the throat or upper chest, where its line of pale silver brightened at its raised surfaces would be plainly visible. A few beads, links, or turns sit out of strict order, making the hand assembly part of the charm. The hidden side of the amber-ended silver torc is plainer, so attention stays on visible amber detail worked into the public face and the visible finish. The amber-ended silver torc is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Small,
			ItemQuality.Good,
			210.0,
			360.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_bone_token_pendant",
			"pendant",
			"a bone token pendant",
			null,
			"Seen flat, bone token pendant shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the bone token pendant are decorative, but none suggest a moving catch or concealed compartment. The piece would signal commoner taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			7.0m,
			true,
			false,
			"bone",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_carved_amber_pendant",
			"pendant",
			"a carved amber pendant",
			null,
			"Seen flat, carved amber pendant shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The worked parts of the carved amber pendant are decorative, but none suggest a moving catch or concealed compartment. The carved amber pendant reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			34.0m,
			true,
			false,
			"amber",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_turquoise_pendant",
			"pendant",
			"a turquoise pendant",
			null,
			"A turquoise pendant carries its value in the rhythm of its material: blue-green turquoise with fine darker lines. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. Close inspection of the turquoise pendant shows small tool marks rather than cast-perfect smoothness. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			16.0,
			72.0m,
			true,
			false,
			"turquoise",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_jade_pendant",
			"pendant",
			"a jade pendant",
			null,
			"Seen flat, jade pendant shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. The hidden side of the jade pendant is plainer, so attention stays on a small suspension hole worn smooth at the top and the visible finish. The jade pendant is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			22.0,
			180.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_neck_niello_inlaid_pendant",
			"pendant",
			"a niello-inlaid pendant",
			null,
			"The niello-inlaid pendant falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with visible niello detail worked into the public face giving the eye a place to pause. Close inspection of the niello-inlaid pendant shows small tool marks rather than cast-perfect smoothness. The niello-inlaid pendant carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			150.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Functions / Worn Items / Jewellery / Pendants",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_copper_wire_bracelet",
			"bracelet",
			"a copper wire bracelet",
			null,
			"A copper wire bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the copper wire bracelet without making it look neglected. The copper wire bracelet feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			24.0,
			8.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_bronze_cuff_bracelet",
			"bracelet",
			"a bronze cuff bracelet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Brown-gold bronze with rubbed high points shows most clearly on the outer face, while the side that rests against the wearer is quieter. The worked parts of the bronze cuff bracelet are decorative, but none suggest a moving catch or concealed compartment. The bronze cuff bracelet feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			38.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_brass_bangle",
			"bangle",
			"a brass bangle",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of yellow brass polished brighter on the edges. Close inspection of the brass bangle shows small tool marks rather than cast-perfect smoothness. The brass bangle leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			55.0,
			30.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_silver_bracelet",
			"bracelet",
			"a silver bracelet",
			null,
			"A silver bracelet is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and a repeated rhythm of small surface marks gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the silver bracelet without making it look neglected. The silver bracelet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			48.0,
			92.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_silver_gilt_bracelet",
			"bracelet",
			"a silver-gilt bracelet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Gold-washed silver, brighter on the raised places and paler at worn edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. The hidden side of the silver-gilt bracelet is plainer, so attention stays on a repeated rhythm of small surface marks and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			62.0,
			210.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_gold_bracelet",
			"bracelet",
			"a gold bracelet",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of deep yellow gold with a warm, steady shine. Rubbing has brightened the exposed parts of the gold bracelet without making it look neglected. The gold bracelet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			60.0,
			440.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_bone_bead_bracelet",
			"bracelet",
			"a bone bead bracelet",
			null,
			"A bone bead bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the bone bead bracelet without making it look neglected. The bone bead bracelet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			22.0,
			8.0m,
			true,
			false,
			"bone",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_shell_bead_bracelet",
			"bracelet",
			"a shell bead bracelet",
			null,
			"A shell bead bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the shell bead bracelet are decorative, but none suggest a moving catch or concealed compartment. The shell bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			24.0,
			9.0m,
			true,
			false,
			"shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_coral_bead_bracelet",
			"bracelet",
			"a coral bead bracelet",
			null,
			"A coral bead bracelet is made to circle a limb, with coral beads with a warm sea-worn surface providing the visible body of the ornament. The edges are kept low and rounded, and a repeated rhythm of small surface marks gives the piece a recognisable worked surface. The worked parts of the coral bead bracelet are decorative, but none suggest a moving catch or concealed compartment. The coral bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			32.0,
			85.0m,
			true,
			false,
			"coral",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_jet_bead_bracelet",
			"bracelet",
			"a jet bead bracelet",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of black jet polished until it gives back a quiet reflection. The hidden side of the jet bead bracelet is plainer, so attention stays on a repeated rhythm of small surface marks and the visible finish. The jet bead bracelet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			26.0m,
			true,
			false,
			"jet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_glass_bead_bracelet",
			"bracelet",
			"a glass bead bracelet",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A repeated rhythm of small surface marks breaks up the line of coloured glass polished smooth enough to glow at the edges. Fine wear gathers around the bracelet's highest places, leaving a repeated rhythm of small surface marks easier to pick out. The glass bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			28.0,
			18.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_faience_bead_bracelet",
			"bracelet",
			"a faience bead bracelet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Glazed faience with small pooling marks in the colour shows most clearly on the outer face, while the side that rests against the wearer is quieter. Small handling marks cross the faience bead bracelet unevenly, especially where it would brush cloth or skin. The faience bead bracelet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			20.0m,
			true,
			false,
			"faience",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_leather_cord_bracelet",
			"bracelet",
			"a leather cord bracelet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Worked leather with creases at the folds shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bracelet's highest places, leaving a repeated rhythm of small surface marks easier to pick out. The leather cord bracelet reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			14.0,
			5.0m,
			true,
			false,
			"leather",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_silk_knot_bracelet",
			"bracelet",
			"a silk knot bracelet",
			null,
			"A silk knot bracelet is made to circle a limb, with silk cord with a soft light across its twist providing the visible body of the ornament. The edges are kept low and rounded, and a repeated rhythm of small surface marks gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the silk knot bracelet without making it look neglected. The silk knot bracelet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			10.0,
			16.0m,
			true,
			false,
			"silk",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_bronze_bracelet_set",
			"bracelet",
			"a paired bronze bracelet set",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Brown-gold bronze with rubbed high points shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the paired bronze bracelet set without making it look neglected. The paired bronze bracelet set is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			90.0,
			70.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_silver_bracelet_set",
			"bracelet",
			"a paired silver bracelet set",
			null,
			"A paired silver bracelet set would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the paired silver bracelet set are decorative, but none suggest a moving catch or concealed compartment. The paired silver bracelet set feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			84.0,
			180.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_motif_brass_bracelet",
			"bracelet",
			"a $motif brass bracelet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Yellow brass polished brighter on the edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the $motif brass bracelet without making it look neglected. The $motif brass bracelet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			48.0,
			42.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_finish_silver_bracelet",
			"bracelet",
			"a $finish silver bracelet",
			null,
			"A $finish silver bracelet is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and the chosen finish left deliberately visible on the worked surface gives the piece a recognisable worked surface. Small handling marks cross the $finish silver bracelet unevenly, especially where it would brush cloth or skin. The $finish silver bracelet feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			46.0,
			100.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal",
				"Variable_MetalFinish"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_narrow_glass_bangle",
			"bangle",
			"a narrow glass bangle",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Coloured glass polished smooth enough to glow at the edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bangle's highest places, leaving a continuous curve made to show as the hand moves easier to pick out. The narrow glass bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			35.0,
			18.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_blue_faience_bangle",
			"bangle",
			"a blue faience bangle",
			null,
			"A blue faience bangle would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Small handling marks cross the blue faience bangle unevenly, especially where it would brush cloth or skin. The blue faience bangle feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			24.0m,
			true,
			false,
			"faience",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_conch_shell_bangle",
			"bangle",
			"a conch shell bangle",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Cut conch shell with creamy curves and warmer pinkish edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bangle's highest places, leaving a continuous curve made to show as the hand moves easier to pick out. The conch shell bangle leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			38.0,
			22.0m,
			true,
			false,
			"conch shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_mother_of_pearl_bangle",
			"bangle",
			"a mother-of-pearl bangle",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Mother-of-pearl flashing softly under the surface shows most clearly on the outer face, while the side that rests against the wearer is quieter. Fine wear gathers around the bangle's highest places, leaving a continuous curve made to show as the hand moves easier to pick out. The mother-of-pearl bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			34.0,
			90.0m,
			true,
			false,
			"mother-of-pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_plain_gold_bangle",
			"bangle",
			"a plain gold bangle",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A continuous curve made to show as the hand moves breaks up the line of deep yellow gold with a warm, steady shine. Rubbing has brightened the exposed parts of the plain gold bangle without making it look neglected. The plain gold bangle feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			55.0,
			400.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_red_coral_bangle",
			"bangle",
			"a red coral bangle",
			null,
			"A red coral bangle would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the red coral bangle without making it look neglected. The red coral bangle leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			45.0,
			125.0m,
			true,
			false,
			"coral",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_glass_bangle_set",
			"bangle",
			"a paired glass bangle set",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Coloured glass polished smooth enough to glow at the edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Close inspection of the paired glass bangle set shows small tool marks rather than cast-perfect smoothness. The paired glass bangle set is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			70.0,
			34.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_gold_bangle_set",
			"bangle",
			"a paired gold bangle set",
			null,
			"A paired gold bangle set would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the paired gold bangle set are decorative, but none suggest a moving catch or concealed compartment. The paired gold bangle set is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			110.0,
			780.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_conch_bangle_set",
			"bangle",
			"a paired conch bangle set",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A continuous curve made to show as the hand moves breaks up the line of cut conch shell with creamy curves and warmer pinkish edges. The hidden side of the paired conch bangle set is plainer, so attention stays on a continuous curve made to show as the hand moves and the visible finish. The paired conch bangle set carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			44.0m,
			true,
			false,
			"conch shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelets",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_silver_gilt_bangle",
			"bangle",
			"a silver-gilt bangle",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Gold-washed silver, brighter on the raised places and paler at worn edges shows most clearly on the outer face, while the side that rests against the wearer is quieter. Close inspection of the silver-gilt bangle shows small tool marks rather than cast-perfect smoothness. The silver-gilt bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			52.0,
			180.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_gilded_copper_bangle",
			"bangle",
			"a gilded copper bangle",
			null,
			"A gilded copper bangle is made to circle a limb, with coppery red metal visible beneath worn gilt edges providing the visible body of the ornament. The edges are kept low and rounded, and a continuous curve made to show as the hand moves gives the piece a recognisable worked surface. The hidden side of the gilded copper bangle is plainer, so attention stays on a continuous curve made to show as the hand moves and the visible finish. The gilded copper bangle is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			50.0,
			48.0m,
			true,
			false,
			"gilded copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_jade_bangle",
			"bangle",
			"a jade bangle",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Green jade smoothed to a cool waxy finish shows most clearly on the outer face, while the side that rests against the wearer is quieter. The worked parts of the jade bangle are decorative, but none suggest a moving catch or concealed compartment. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			60.0,
			180.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_bronze_upper_arm_armlet",
			"armlet",
			"a bronze upper-arm armlet",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A slightly widened face meant to turn outward breaks up the line of brown-gold bronze with rubbed high points. The hidden side of the bronze upper-arm armlet is plainer, so attention stays on a slightly widened face meant to turn outward and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.Small,
			ItemQuality.Good,
			120.0,
			58.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_silver_armlet",
			"armlet",
			"a silver armlet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Pale silver brightened at its raised surfaces shows most clearly on the outer face, while the side that rests against the wearer is quieter. Close inspection of the silver armlet shows small tool marks rather than cast-perfect smoothness. The silver armlet reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.Small,
			ItemQuality.Good,
			105.0,
			220.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_gold_armlet",
			"armlet",
			"a gold armlet",
			null,
			"A gold armlet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Small handling marks cross the gold armlet unevenly, especially where it would brush cloth or skin. The gold armlet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Small,
			ItemQuality.Good,
			115.0,
			760.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_leather_upper_arm_band",
			"armlet",
			"a leather upper-arm band",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Worked leather with creases at the folds shows most clearly on the outer face, while the side that rests against the wearer is quieter. The worked parts of the leather upper-arm band are decorative, but none suggest a moving catch or concealed compartment. The leather upper-arm band carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Small,
			ItemQuality.Standard,
			35.0,
			10.0m,
			true,
			false,
			"leather",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_horn_armlet",
			"armlet",
			"a horn armlet",
			null,
			"A horn armlet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Fine wear gathers around the armlet's highest places, leaving a slightly widened face meant to turn outward easier to pick out. The horn armlet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Small,
			ItemQuality.Good,
			60.0,
			26.0m,
			true,
			false,
			"horn",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_gilded_bronze_armlet",
			"armlet",
			"a gilded bronze armlet",
			null,
			"A gilded bronze armlet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The hidden side of the gilded bronze armlet is plainer, so attention stays on a slightly widened face meant to turn outward and the visible finish. The gilded bronze armlet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Small,
			ItemQuality.Good,
			100.0,
			150.0m,
			true,
			false,
			"gilded bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_motif_silver_armlet",
			"armlet",
			"a $motif silver armlet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Pale silver brightened at its raised surfaces shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the $motif silver armlet without making it look neglected. The $motif silver armlet reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.Small,
			ItemQuality.Good,
			110.0,
			240.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_coral_set_armlet",
			"armlet",
			"a coral-set armlet",
			null,
			"A coral-set armlet is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and visible coral detail worked into the public face gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the coral-set armlet without making it look neglected. The coral-set armlet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Small,
			ItemQuality.Good,
			125.0,
			360.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_steppe_silver_arm_ring",
			"armlet",
			"a steppe silver arm ring",
			null,
			"A steppe silver arm ring would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Close inspection of the steppe silver arm ring shows small tool marks rather than cast-perfect smoothness. The steppe silver arm ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Small,
			ItemQuality.Good,
			130.0,
			260.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_heavy_bronze_arm_ring",
			"armlet",
			"a heavy bronze arm ring",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A slightly widened face meant to turn outward breaks up the line of brown-gold bronze with rubbed high points. Fine wear gathers around the armlet's highest places, leaving a slightly widened face meant to turn outward easier to pick out. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.Small,
			ItemQuality.Good,
			150.0,
			70.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Armlets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Armlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_copper_anklet",
			"anklet",
			"a copper anklet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Warm reddish copper darkened in the recesses shows most clearly on the outer face, while the side that rests against the wearer is quieter. Rubbing has brightened the exposed parts of the copper anklet without making it look neglected. The copper anklet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			10.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_brass_anklet_set",
			"anklet",
			"a paired brass anklet set",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. The inner side kept plainer than the visible outer arc breaks up the line of yellow brass polished brighter on the edges. Rubbing has brightened the exposed parts of the paired brass anklet set without making it look neglected. The paired brass anklet set carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			95.0,
			46.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_silver_anklet_set",
			"anklet",
			"a paired silver anklet set",
			null,
			"A paired silver anklet set is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and the inner side kept plainer than the visible outer arc gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the paired silver anklet set without making it look neglected. The paired silver anklet set is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			88.0,
			160.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklets",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_bell_less_bronze_anklet",
			"anklet",
			"a bell-less bronze anklet",
			null,
			"A bell-less bronze anklet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Rubbing has brightened the exposed parts of the bell-less bronze anklet without making it look neglected. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			54.0,
			28.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_cowrie_shell_anklet_string",
			"anklet",
			"a cowrie shell anklet string",
			null,
			"A cowrie shell anklet string would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The worked parts of the cowrie shell anklet string are decorative, but none suggest a moving catch or concealed compartment. The cowrie shell anklet string carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			38.0,
			12.0m,
			true,
			false,
			"cowrie shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_conch_shell_anklet",
			"anklet",
			"a conch shell anklet",
			null,
			"A conch shell anklet is made to circle a limb, with cut conch shell with creamy curves and warmer pinkish edges providing the visible body of the ornament. The edges are kept low and rounded, and the inner side kept plainer than the visible outer arc gives the piece a recognisable worked surface. Small handling marks cross the conch shell anklet unevenly, especially where it would brush cloth or skin. The conch shell anklet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			20.0m,
			true,
			false,
			"conch shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_gold_court_anklet",
			"anklet",
			"a gold court anklet",
			null,
			"A gold court anklet is made to circle a limb, with deep yellow gold with a warm, steady shine providing the visible body of the ornament. The edges are kept low and rounded, and the inner side kept plainer than the visible outer arc gives the piece a recognisable worked surface. Rubbing has brightened the exposed parts of the gold court anklet without making it look neglected. The gold court anklet leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			62.0,
			420.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_glass_bead_anklet",
			"anklet",
			"a glass bead anklet",
			null,
			"A glass bead anklet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Fine wear gathers around the anklet's highest places, leaving the inner side kept plainer than the visible outer arc easier to pick out. The glass bead anklet feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			36.0,
			18.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_coral_bead_anklet",
			"anklet",
			"a coral bead anklet",
			null,
			"This limb ornament is practical in scale even when it is decorative in intent. Coral beads with a warm sea-worn surface shows most clearly on the outer face, while the side that rests against the wearer is quieter. Small handling marks cross the coral bead anklet unevenly, especially where it would brush cloth or skin. The coral bead anklet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			40.0,
			80.0m,
			true,
			false,
			"coral",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_braided_hemp_anklet",
			"anklet",
			"a braided hemp anklet",
			null,
			"A braided hemp anklet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Fine wear gathers around the anklet's highest places, leaving the inner side kept plainer than the visible outer arc easier to pick out. The piece would signal simple household taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			18.0,
			2.0m,
			true,
			false,
			"hemp",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Anklets",
				"Market / Jewellery / Simple Jewellery"
			],
			[
				"Holdable",
				"Wear_Anklet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_plain_silver_toe_ring",
			"ring",
			"a plain silver toe ring",
			null,
			"A plain silver toe ring would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The hidden side of the plain silver toe ring is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The plain silver toe ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			22.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_paired_silver_toe_ring_set",
			"ring",
			"a paired silver toe-ring set",
			null,
			"A paired silver toe-ring set would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. The hidden side of the paired silver toe-ring set is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			42.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_copper_toe_ring",
			"ring",
			"a copper toe ring",
			null,
			"The form is simple enough to survive frequent wearing, but not so plain that it disappears. A small open gap where the band can be adjusted breaks up the line of warm reddish copper darkened in the recesses. The hidden side of the copper toe ring is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The copper toe ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			5.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_gold_toe_ring",
			"ring",
			"a gold toe ring",
			null,
			"A gold toe ring is made to circle a limb, with deep yellow gold with a warm, steady shine providing the visible body of the ornament. The edges are kept low and rounded, and a small open gap where the band can be adjusted gives the piece a recognisable worked surface. Fine wear gathers around the ring's highest places, leaving a small open gap where the band can be adjusted easier to pick out. The gold toe ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			5.0,
			90.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_shell_set_toe_ring",
			"ring",
			"a shell-set toe ring",
			null,
			"A shell-set toe ring is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and visible shell detail worked into the public face gives the piece a recognisable worked surface. The worked parts of the shell-set toe ring are decorative, but none suggest a moving catch or concealed compartment. The shell-set toe ring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			6.0,
			30.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_brass_toe_ring",
			"ring",
			"a brass toe ring",
			null,
			"A brass toe ring is made to circle a limb, with yellow brass polished brighter on the edges providing the visible body of the ornament. The edges are kept low and rounded, and a small open gap where the band can be adjusted gives the piece a recognisable worked surface. The hidden side of the brass toe ring is plainer, so attention stays on a small open gap where the band can be adjusted and the visible finish. The brass toe ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			8.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_tiny_pearl_toe_ring",
			"ring",
			"a tiny pearl toe ring",
			null,
			"A tiny pearl toe ring is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and visible pearl detail worked into the public face gives the piece a recognisable worked surface. The worked parts of the tiny pearl toe ring are decorative, but none suggest a moving catch or concealed compartment. The tiny pearl toe ring reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			5.0,
			70.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_limb_motif_toe_ring",
			"ring",
			"a $motif toe ring",
			null,
			"A $motif toe ring is made to circle a limb, with pale silver brightened at its raised surfaces providing the visible body of the ornament. The edges are kept low and rounded, and the selected motif set into the outward-facing surface gives the piece a recognisable worked surface. Small handling marks cross the $motif toe ring unevenly, especially where it would brush cloth or skin. The $motif toe ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			35.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Toe Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Toe_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_copper_hoop_earrings",
			"earring",
			"a pair of copper hoop earrings",
			null,
			"The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Warm reddish copper darkened in the recesses gives it colour and finish without making it look bulky. Close inspection of the pair of copper hoop earrings shows small tool marks rather than cast-perfect smoothness. The pair of copper hoop earrings carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			8.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_bronze_hoop_earrings",
			"earring",
			"a pair of bronze hoop earrings",
			null,
			"A pair of bronze hoop earrings is small enough to depend on close detail, not weight. The visible face uses brown-gold bronze with rubbed high points, while the back and fastening are plainer. Small handling marks cross the pair of bronze hoop earrings unevenly, especially where it would brush cloth or skin. The pair of bronze hoop earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			14.0,
			16.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_silver_hoop_earrings",
			"earring",
			"a pair of silver hoop earrings",
			null,
			"There is a deliberate economy to the piece: little material, but careful placement. Pale silver brightened at its raised surfaces stands out in the face of the ornament, with simpler work hidden behind it. Close inspection of the pair of silver hoop earrings shows small tool marks rather than cast-perfect smoothness. The pair of silver hoop earrings carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			58.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_gold_hoop_earrings",
			"earring",
			"a pair of gold hoop earrings",
			null,
			"A pair of gold hoop earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. The worked parts of the pair of gold hoop earrings are decorative, but none suggest a moving catch or concealed compartment. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			11.0,
			210.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_pearl_drop_earrings",
			"earring",
			"a pair of pearl drop earrings",
			null,
			"A pair of pearl drop earrings is small enough to depend on close detail, not weight. The visible face uses small pearls with uneven but gentle lustre, while the back and fastening are plainer. Small handling marks cross the pair of pearl drop earrings unevenly, especially where it would brush cloth or skin. The pair of pearl drop earrings leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			9.0,
			180.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_coral_drop_earrings",
			"earring",
			"a pair of coral drop earrings",
			null,
			"A pair of coral drop earrings is small enough to depend on close detail, not weight. The visible face uses coral beads with a warm sea-worn surface, while the back and fastening are plainer. Small handling marks cross the pair of coral drop earrings unevenly, especially where it would brush cloth or skin. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			120.0m,
			true,
			false,
			"coral",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_glass_bead_earrings",
			"earring",
			"a pair of glass bead earrings",
			null,
			"A pair of glass bead earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the pair of glass bead earrings shows small tool marks rather than cast-perfect smoothness. The pair of glass bead earrings reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			10.0,
			18.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_amber_bead_earrings",
			"earring",
			"a pair of amber bead earrings",
			null,
			"There is a deliberate economy to the piece: little material, but careful placement. Honey-coloured amber with cloudy threads caught inside stands out in the face of the ornament, with simpler work hidden behind it. Close inspection of the pair of amber bead earrings shows small tool marks rather than cast-perfect smoothness. The pair of amber bead earrings feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			48.0m,
			true,
			false,
			"amber",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_turquoise_earrings",
			"earring",
			"a pair of turquoise earrings",
			null,
			"The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Blue-green turquoise with fine darker lines gives it colour and finish without making it look bulky. Rubbing has brightened the exposed parts of the pair of turquoise earrings without making it look neglected. The pair of turquoise earrings feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			120.0m,
			true,
			false,
			"turquoise",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_garnet_earrings",
			"earring",
			"a pair of garnet earrings",
			null,
			"The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Dark red stones that brighten at their cut edges gives it colour and finish without making it look bulky. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of garnet earrings leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			8.0,
			260.0m,
			true,
			false,
			"garnet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_silver_wire_earrings",
			"earring",
			"a pair of silver wire earrings",
			null,
			"A pair of silver wire earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of silver wire earrings feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			7.0,
			50.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_gilded_bronze_earrings",
			"earring",
			"a pair of gilded bronze earrings",
			null,
			"A pair of gilded bronze earrings is small enough to depend on close detail, not weight. The visible face uses bronze showing through a thin golden surface at nicks and bends, while the back and fastening are plainer. The worked parts of the pair of gilded bronze earrings are decorative, but none suggest a moving catch or concealed compartment. The pair of gilded bronze earrings leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			44.0m,
			true,
			false,
			"gilded bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_single_bronze_earring",
			"earring",
			"a single bronze earring",
			null,
			"A single bronze earring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the single bronze earring shows small tool marks rather than cast-perfect smoothness. The single bronze earring reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			7.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Earring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_single_silver_earring",
			"earring",
			"a single silver earring",
			null,
			"A single silver earring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the single silver earring shows small tool marks rather than cast-perfect smoothness. The single silver earring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			24.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Earring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_single_gold_earring",
			"earring",
			"a single gold earring",
			null,
			"A single gold earring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The single gold earring leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			5.0,
			95.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_conch_shell_earrings",
			"earring",
			"a pair of conch shell earrings",
			null,
			"There is a deliberate economy to the piece: little material, but careful placement. Cut conch shell with creamy curves and warmer pinkish edges stands out in the face of the ornament, with simpler work hidden behind it. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of conch shell earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			9.0,
			18.0m,
			true,
			false,
			"conch shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_cowrie_earrings",
			"earring",
			"a pair of cowrie earrings",
			null,
			"A pair of cowrie earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the earring's highest places, leaving a small hanging point polished more brightly than the rest easier to pick out. The pair of cowrie earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			10.0m,
			true,
			false,
			"cowrie shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_mother_of_pearl_earrings",
			"earring",
			"a pair of mother-of-pearl earrings",
			null,
			"The ornament is light in the hand, with a small hanging point polished more brightly than the rest worked into the part most likely to catch the eye. Mother-of-pearl flashing softly under the surface gives it colour and finish without making it look bulky. The worked parts of the pair of mother-of-pearl earrings are decorative, but none suggest a moving catch or concealed compartment. The pair of mother-of-pearl earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			8.0,
			90.0m,
			true,
			false,
			"mother-of-pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_jade_earrings",
			"earring",
			"a pair of jade earrings",
			null,
			"A pair of jade earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Rubbing has brightened the exposed parts of the pair of jade earrings without making it look neglected. The pair of jade earrings carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			120.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_lapis_bead_earrings",
			"earring",
			"a pair of lapis bead earrings",
			null,
			"There is a deliberate economy to the piece: little material, but careful placement. Deep blue stone flecked with paler specks stands out in the face of the ornament, with simpler work hidden behind it. Close inspection of the pair of lapis bead earrings shows small tool marks rather than cast-perfect smoothness. The pair of lapis bead earrings leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			9.0,
			110.0m,
			true,
			false,
			"lapis lazuli",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_carneliearrings",
			"earring",
			"a pair of carnelian earrings",
			null,
			"A pair of carnelian earrings has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Rubbing has brightened the exposed parts of the pair of carnelian earrings without making it look neglected. The pair of carnelian earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			9.0,
			65.0m,
			true,
			false,
			"carnelian",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_rock_crystal_earrings",
			"earring",
			"a pair of rock crystal earrings",
			null,
			"A pair of rock crystal earrings is small enough to depend on close detail, not weight. The visible face uses clear rock crystal with tiny inner veils, while the back and fastening are plainer. The hidden side of the pair of rock crystal earrings is plainer, so attention stays on a small hanging point polished more brightly than the rest and the visible finish. The pair of rock crystal earrings carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			8.0,
			100.0m,
			true,
			false,
			"rock crystal",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_enamelled_earrings",
			"earring",
			"a pair of enamelled earrings",
			null,
			"The ornament is light in the hand, with visible enamel detail worked into the public face worked into the part most likely to catch the eye. Enamel gives it colour and finish without making it look bulky. The hidden side of the pair of enamelled earrings is plainer, so attention stays on visible enamel detail worked into the public face and the visible finish. The pair of enamelled earrings is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			130.0m,
			true,
			false,
			"enamel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_shape_earrings",
			"earring",
			"a pair of $shape earrings",
			null,
			"There is a deliberate economy to the piece: little material, but careful placement. Pale silver brightened at its raised surfaces stands out in the face of the ornament, with simpler work hidden behind it. Rubbing has brightened the exposed parts of the pair of $shape earrings without making it look neglected. The pair of $shape earrings leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			10.0,
			78.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_small_gold_nose_ring",
			"nose ring",
			"a small gold nose ring",
			null,
			"A small gold nose ring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Fine wear gathers around the nose ring's highest places, leaving faint tool chatter along the inner face easier to pick out. The small gold nose ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			4.0,
			110.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_silver_nose_ring",
			"nose ring",
			"a silver nose ring",
			null,
			"A silver nose ring has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Rubbing has brightened the exposed parts of the silver nose ring without making it look neglected. The silver nose ring carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			36.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pearl_nose_stud",
			"stud",
			"a pearl nose stud",
			null,
			"A pearl nose stud has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. The hidden side of the pearl nose stud is plainer, so attention stays on a softened seam where the maker closed it and the visible finish. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			3.0,
			70.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_tiny_turquoise_nose_stud",
			"stud",
			"a tiny turquoise nose stud",
			null,
			"A tiny turquoise nose stud is small enough to depend on close detail, not weight. The visible face uses blue-green turquoise with fine darker lines, while the back and fastening are plainer. Close inspection of the tiny turquoise nose stud shows small tool marks rather than cast-perfect smoothness. The tiny turquoise nose stud carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			3.0,
			58.0m,
			true,
			false,
			"turquoise",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_copper_nose_ring",
			"nose ring",
			"a copper nose ring",
			null,
			"There is a deliberate economy to the piece: little material, but careful placement. Warm reddish copper darkened in the recesses stands out in the face of the ornament, with simpler work hidden behind it. The worked parts of the copper nose ring are decorative, but none suggest a moving catch or concealed compartment. The copper nose ring is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			6.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_flower_shaped_nose_stud",
			"stud",
			"a flower-shaped nose stud",
			null,
			"There is a deliberate economy to the piece: little material, but careful placement. Deep yellow gold with a warm, steady shine stands out in the face of the ornament, with simpler work hidden behind it. The worked parts of the flower-shaped nose stud are decorative, but none suggest a moving catch or concealed compartment. The flower-shaped nose stud is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			3.0,
			95.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_motif_silver_nose_ring",
			"nose ring",
			"a $motif silver nose ring",
			null,
			"A $motif silver nose ring is small enough to depend on close detail, not weight. The visible face uses pale silver brightened at its raised surfaces, while the back and fastening are plainer. Rubbing has brightened the exposed parts of the $motif silver nose ring without making it look neglected. The $motif silver nose ring reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			4.0,
			45.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Piercings / Nose Rings",
				"Functions / Worn Items / Jewellery / Piercings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Nose_Ring",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_silver_ear_studs",
			"stud",
			"a pair of silver ear studs",
			null,
			"The ornament is light in the hand, with a softened seam where the maker closed it worked into the part most likely to catch the eye. Pale silver brightened at its raised surfaces gives it colour and finish without making it look bulky. Small handling marks cross the pair of silver ear studs unevenly, especially where it would brush cloth or skin. The pair of silver ear studs is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			5.0,
			42.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Functions / Worn Items / Jewellery / Piercings / Ear Studs",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_glass_ear_studs",
			"stud",
			"a pair of glass ear studs",
			null,
			"A pair of glass ear studs has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. The worked parts of the pair of glass ear studs are decorative, but none suggest a moving catch or concealed compartment. The pair of glass ear studs leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			5.0,
			12.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Functions / Worn Items / Jewellery / Piercings / Ear Studs",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_ear_pair_of_gold_ear_studs",
			"stud",
			"a pair of gold ear studs",
			null,
			"A pair of gold ear studs has a front meant for display and a reverse meant simply to sit against the wearer. The edges are softened so the small form reads as jewellery rather than loose hardware. Close inspection of the pair of gold ear studs shows small tool marks rather than cast-perfect smoothness. The pair of gold ear studs leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			5.0,
			120.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Earrings",
				"Functions / Worn Items / Jewellery / Piercings / Ear Studs",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Earrings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_plain_bronze_ring_brooch",
			"brooch",
			"a plain bronze ring brooch",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Rubbing has brightened the exposed parts of the plain bronze ring brooch without making it look neglected. The plain bronze ring brooch feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			28.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_silver_annular_brooch",
			"brooch",
			"a silver annular brooch",
			null,
			"A silver annular brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. The hidden side of the silver annular brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The silver annular brooch feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			38.0,
			85.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_gilded_copper_disc_brooch",
			"brooch",
			"a gilded copper disc brooch",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. The hidden side of the gilded copper disc brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			48.0,
			48.0m,
			true,
			false,
			"gilded copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_garnet_coloured_brooch",
			"brooch",
			"a garnet-coloured brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked deep yellow gold with a warm, steady shine, leaving the display surface clean. Small handling marks cross the garnet-coloured brooch unevenly, especially where it would brush cloth or skin. The garnet-coloured brooch reads as noble adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			46.0,
			420.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_glass_inlaid_brooch",
			"brooch",
			"a glass-inlaid brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked brown-gold bronze with rubbed high points, leaving the display surface clean. The hidden side of the glass-inlaid brooch is plainer, so attention stays on visible glass detail worked into the public face and the visible finish. The glass-inlaid brooch reads as merchant adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			50.0,
			90.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_penannular_silver_brooch",
			"brooch",
			"a penannular silver brooch",
			null,
			"A penannular silver brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. Small handling marks cross the penannular silver brooch unevenly, especially where it would brush cloth or skin. The penannular silver brooch carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			180.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_heavy_cloak_brooch",
			"brooch",
			"a heavy cloak brooch",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. The hidden side of the heavy cloak brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The heavy cloak brooch feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			85.0,
			50.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_paired_oval_brooch_set",
			"brooch",
			"a paired oval brooch set",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The paired oval brooch set carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			150.0,
			110.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooches",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_paired_silver_brooch_set",
			"brooch",
			"a paired silver brooch set",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked pale silver brightened at its raised surfaces, leaving the display surface clean. Small handling marks cross the paired silver brooch set unevenly, especially where it would brush cloth or skin. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			130.0,
			260.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooches",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_small_pewter_brooch",
			"brooch",
			"a small pewter brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked soft grey pewter with a low satin sheen, leaving the display surface clean. The hidden side of the small pewter brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The small pewter brooch reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			16.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_brass_ring_brooch",
			"brooch",
			"a brass ring brooch",
			null,
			"A brass ring brooch has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with a firm rim and decorated face set where it will show against cloth. The hidden side of the brass ring brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The brass ring brooch reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			40.0,
			24.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_enamel_disc_brooch",
			"brooch",
			"an enamel disc brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked enamel, leaving the display surface clean. Close inspection of the enamel disc brooch shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			44.0,
			130.0m,
			true,
			false,
			"enamel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_niello_inlaid_silver_brooch",
			"brooch",
			"a niello-inlaid silver brooch",
			null,
			"A niello-inlaid silver brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with visible niello detail worked into the public face set where it will show against cloth. The worked parts of the niello-inlaid silver brooch are decorative, but none suggest a moving catch or concealed compartment. The niello-inlaid silver brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			46.0,
			150.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_jet_mourning_brooch",
			"brooch",
			"a jet mourning brooch",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The jet mourning brooch leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			32.0,
			46.0m,
			true,
			false,
			"jet",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_amber_faced_brooch",
			"brooch",
			"an amber-faced brooch",
			null,
			"An amber-faced brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the amber-faced brooch without making it look neglected. The amber-faced brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			48.0,
			85.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_coral_set_brooch",
			"brooch",
			"a coral-set brooch",
			null,
			"A coral-set brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with visible coral detail worked into the public face set where it will show against cloth. The hidden side of the coral-set brooch is plainer, so attention stays on visible coral detail worked into the public face and the visible finish. The coral-set brooch carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			44.0,
			150.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_lapis_set_court_brooch",
			"brooch",
			"a lapis-set court brooch",
			null,
			"A lapis-set court brooch has a public face and a practical back, the two sides plainly made for different jobs. Deep yellow gold with a warm, steady shine forms the visible body, with visible lapis detail worked into the public face set where it will show against cloth. The hidden side of the lapis-set court brooch is plainer, so attention stays on visible lapis detail worked into the public face and the visible finish. The lapis-set court brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			50.0,
			520.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_rock_crystal_brooch",
			"brooch",
			"a rock crystal brooch",
			null,
			"A rock crystal brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. Fine wear gathers around the brooch's highest places, leaving visible rock crystal detail worked into the public face easier to pick out. The rock crystal brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			42.0,
			170.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_simple_bone_brooch",
			"brooch",
			"a simple bone brooch",
			null,
			"A simple bone brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale bone burnished by handling forms the visible body, with a firm rim and decorated face set where it will show against cloth. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The simple bone brooch leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			24.0,
			8.0m,
			true,
			false,
			"bone",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_horn_cloak_brooch",
			"brooch",
			"a horn cloak brooch",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A firm rim and decorated face gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the horn cloak brooch unevenly, especially where it would brush cloth or skin. The horn cloak brooch carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			28.0,
			12.0m,
			true,
			false,
			"horn",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_shell_faced_brooch",
			"brooch",
			"a shell-faced brooch",
			null,
			"A shell-faced brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale shell showing irregular growth lines forms the visible body, with a firm rim and decorated face set where it will show against cloth. Fine wear gathers around the brooch's highest places, leaving a firm rim and decorated face easier to pick out. The shell-faced brooch carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			22.0m,
			true,
			false,
			"shell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_mother_of_pearl_brooch",
			"brooch",
			"a mother-of-pearl brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked mother-of-pearl flashing softly under the surface, leaving the display surface clean. Close inspection of the mother-of-pearl brooch shows small tool marks rather than cast-perfect smoothness. The mother-of-pearl brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			28.0,
			95.0m,
			true,
			false,
			"mother-of-pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_motif_bronze_brooch",
			"brooch",
			"a $motif bronze brooch",
			null,
			"A $motif bronze brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Close inspection of the $motif bronze brooch shows small tool marks rather than cast-perfect smoothness. The $motif bronze brooch is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			36.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_shape_silver_brooch",
			"brooch",
			"a $shape silver brooch",
			null,
			"A $shape silver brooch is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the $shape silver brooch without making it look neglected. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			42.0,
			90.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_inlay_gold_brooch",
			"brooch",
			"a $inlay gold brooch",
			null,
			"A $inlay gold brooch has a public face and a practical back, the two sides plainly made for different jobs. Deep yellow gold with a warm, steady shine forms the visible body, with the chosen inlay style running through the visible lines set where it will show against cloth. The hidden side of the $inlay gold brooch is plainer, so attention stays on the chosen inlay style running through the visible lines and the visible finish. The $inlay gold brooch leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			46.0,
			560.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal",
				"Variable_InlayStyle"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_finish_brass_brooch",
			"brooch",
			"a $finish brass brooch",
			null,
			"A $finish brass brooch has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with the chosen finish left deliberately visible on the worked surface set where it will show against cloth. Close inspection of the $finish brass brooch shows small tool marks rather than cast-perfect smoothness. The $finish brass brooch feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			40.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal",
				"Variable_MetalFinish"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_trefoil_like_bronze_brooch",
			"brooch",
			"a trefoil-like bronze brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked brown-gold bronze with rubbed high points, leaving the display surface clean. Small handling marks cross the trefoil-like bronze brooch unevenly, especially where it would brush cloth or skin. The trefoil-like bronze brooch feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			45.0,
			38.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_broad_silver_cloak_brooch",
			"brooch",
			"a broad silver cloak brooch",
			null,
			"A broad silver cloak brooch has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with a firm rim and decorated face set where it will show against cloth. The hidden side of the broad silver cloak brooch is plainer, so attention stays on a firm rim and decorated face and the visible finish. The broad silver cloak brooch feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			80.0,
			210.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_little_copper_dress_brooch",
			"brooch",
			"a little copper dress brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked warm reddish copper darkened in the recesses, leaving the display surface clean. Small handling marks cross the little copper dress brooch unevenly, especially where it would brush cloth or skin. The little copper dress brooch carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			24.0,
			12.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_brooch_fine_gold_court_brooch",
			"brooch",
			"a fine gold court brooch",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked deep yellow gold with a warm, steady shine, leaving the display surface clean. Close inspection of the fine gold court brooch shows small tool marks rather than cast-perfect smoothness. The fine gold court brooch reads as courtly adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			52.0,
			760.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_bronze_cloak_pin",
			"pin",
			"a bronze cloak pin",
			null,
			"A bronze cloak pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Fine wear gathers around the pin's highest places, leaving a shaft kept straighter than the decorated head easier to pick out. The bronze cloak pin is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			30.0,
			18.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_silver_cloak_pin",
			"pin",
			"a silver cloak pin",
			null,
			"A silver cloak pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as pale silver brightened at its raised surfaces, with handling marks gathered near the fastening points. The worked parts of the silver cloak pin are decorative, but none suggest a moving catch or concealed compartment. The silver cloak pin leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			28.0,
			62.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_iron_dress_pin",
			"pin",
			"an iron dress pin",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked dark wrought iron with a waxed, workmanlike finish, leaving the display surface clean. The hidden side of the iron dress pin is plainer, so attention stays on a shaft kept straighter than the decorated head and the visible finish. The iron dress pin reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			5.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_bone_garment_pin",
			"pin",
			"a bone garment pin",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. The hidden side of the bone garment pin is plainer, so attention stays on a shaft kept straighter than the decorated head and the visible finish. The bone garment pin feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			16.0,
			4.0m,
			true,
			false,
			"bone",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_brass_ring_pin",
			"pin",
			"a brass ring pin",
			null,
			"A brass ring pin has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with a shaft kept straighter than the decorated head set where it will show against cloth. The worked parts of the brass ring pin are decorative, but none suggest a moving catch or concealed compartment. The brass ring pin is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			26.0,
			18.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_gold_headed_pin",
			"pin",
			"a gold-headed pin",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. The worked parts of the gold-headed pin are decorative, but none suggest a moving catch or concealed compartment. The gold-headed pin carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			180.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_pearl_headed_pin",
			"pin",
			"a pearl-headed pin",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked small pearls with uneven but gentle lustre, leaving the display surface clean. The worked parts of the pearl-headed pin are decorative, but none suggest a moving catch or concealed compartment. The pearl-headed pin leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			90.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_glass_headed_pin",
			"pin",
			"a glass-headed pin",
			null,
			"The fastening is visible enough to be understood, but the front is where the maker spent the attention. A pin, catch, or backing sits behind the worked coloured glass polished smooth enough to glow at the edges, leaving the display surface clean. Rubbing has brightened the exposed parts of the glass-headed pin without making it look neglected. The glass-headed pin reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			16.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_rock_crystal_pin",
			"pin",
			"a rock crystal pin",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. The worked parts of the rock crystal pin are decorative, but none suggest a moving catch or concealed compartment. The rock crystal pin carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			105.0m,
			true,
			false,
			"rock crystal",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_carved_horn_pin",
			"pin",
			"a carved horn pin",
			null,
			"A carved horn pin has a public face and a practical back, the two sides plainly made for different jobs. Horn with layered translucent streaks through it forms the visible body, with a shaft kept straighter than the decorated head set where it will show against cloth. Small handling marks cross the carved horn pin unevenly, especially where it would brush cloth or skin. The carved horn pin reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			8.0m,
			true,
			false,
			"horn",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_motif_silver_pin",
			"pin",
			"a $motif silver pin",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. The selected motif set into the outward-facing surface gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the $motif silver pin unevenly, especially where it would brush cloth or skin. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			22.0,
			70.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_finish_bronze_pin",
			"pin",
			"a $finish bronze pin",
			null,
			"A $finish bronze pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as brown-gold bronze with rubbed high points, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the $finish bronze pin without making it look neglected. The $finish bronze pin reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			24.0,
			24.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal",
				"Variable_MetalFinish"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_long_brass_veil_pin",
			"pin",
			"a long brass veil pin",
			null,
			"A long brass veil pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as yellow brass polished brighter on the edges, with handling marks gathered near the fastening points. The worked parts of the long brass veil pin are decorative, but none suggest a moving catch or concealed compartment. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			16.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_narrow_ivory_hair_pin",
			"pin",
			"a narrow ivory hair pin",
			null,
			"A narrow ivory hair pin is broad enough to read from a short distance but still small enough for personal dress. The material shows as close-grained ivory with a creamy, old polish, with handling marks gathered near the fastening points. The hidden side of the narrow ivory hair pin is plainer, so attention stays on a shaft kept straighter than the decorated head and the visible finish. The narrow ivory hair pin leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			18.0,
			80.0m,
			true,
			false,
			"ivory",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_copper_fastening_pin",
			"pin",
			"a copper fastening pin",
			null,
			"A copper fastening pin has a public face and a practical back, the two sides plainly made for different jobs. Warm reddish copper darkened in the recesses forms the visible body, with a shaft kept straighter than the decorated head set where it will show against cloth. Rubbing has brightened the exposed parts of the copper fastening pin without making it look neglected. The copper fastening pin reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			7.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_pin_silver_gilt_court_pin",
			"pin",
			"a silver-gilt court pin",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A shaft kept straighter than the decorated head gives the face definition, while the reverse remains plain and workmanlike. Rubbing has brightened the exposed parts of the silver-gilt court pin without making it look neglected. The silver-gilt court pin feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			26.0,
			210.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Pins",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Pin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_stamped_brass_badge",
			"badge",
			"a stamped brass badge",
			null,
			"A stamped brass badge has a public face and a practical back, the two sides plainly made for different jobs. Yellow brass polished brighter on the edges forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. Close inspection of the stamped brass badge shows small tool marks rather than cast-perfect smoothness. The piece would signal professional or guild taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			22.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Professional Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_pewter_household_badge",
			"badge",
			"a pewter household badge",
			null,
			"A pewter household badge has a public face and a practical back, the two sides plainly made for different jobs. Soft grey pewter with a low satin sheen forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. Fine wear gathers around the badge's highest places, leaving a face broad enough to carry a clear visible device easier to pick out. The pewter household badge carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			22.0,
			14.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_silver_guild_badge",
			"badge",
			"a silver guild badge",
			null,
			"A silver guild badge has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. The worked parts of the silver guild badge are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			80.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_copper_love_badge",
			"badge",
			"a copper love badge",
			null,
			"A copper love badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as warm reddish copper darkened in the recesses, with handling marks gathered near the fastening points. Small handling marks cross the copper love badge unevenly, especially where it would brush cloth or skin. The copper love badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			10.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_enamel_civic_badge",
			"badge",
			"an enamel civic badge",
			null,
			"An enamel civic badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as enamel, with handling marks gathered near the fastening points. The hidden side of the enamel civic badge is plainer, so attention stays on a face broad enough to carry a clear visible device and the visible finish. The enamel civic badge carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			26.0,
			90.0m,
			true,
			false,
			"enamel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_gilded_bronze_badge",
			"badge",
			"a gilded bronze badge",
			null,
			"A gilded bronze badge has a public face and a practical back, the two sides plainly made for different jobs. Bronze showing through a thin golden surface at nicks and bends forms the visible body, with a face broad enough to carry a clear visible device set where it will show against cloth. The worked parts of the gilded bronze badge are decorative, but none suggest a moving catch or concealed compartment. The gilded bronze badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			28.0,
			110.0m,
			true,
			false,
			"gilded bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_small_lead_token_badge",
			"badge",
			"a small lead token badge",
			null,
			"A small lead token badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as lead, with handling marks gathered near the fastening points. Rubbing has brightened the exposed parts of the small lead token badge without making it look neglected. The piece would signal commoner taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			20.0,
			6.0m,
			true,
			false,
			"lead",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_motif_court_badge",
			"badge",
			"a $motif court badge",
			null,
			"A $motif court badge has a public face and a practical back, the two sides plainly made for different jobs. Gold-washed silver, brighter on the raised places and paler at worn edges forms the visible body, with the selected motif set into the outward-facing surface set where it will show against cloth. The hidden side of the $motif court badge is plainer, so attention stays on the selected motif set into the outward-facing surface and the visible finish. The piece would signal courtly taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			30.0,
			240.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_shape_brass_badge",
			"badge",
			"a $shape brass badge",
			null,
			"A $shape brass badge is broad enough to read from a short distance but still small enough for personal dress. The material shows as yellow brass polished brighter on the edges, with handling marks gathered near the fastening points. Small handling marks cross the $shape brass badge unevenly, especially where it would brush cloth or skin. The piece would signal professional or guild taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			30.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Professional Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_mother_of_pearl_badge",
			"badge",
			"a mother-of-pearl badge",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A face broad enough to carry a clear visible device gives the face definition, while the reverse remains plain and workmanlike. The worked parts of the mother-of-pearl badge are decorative, but none suggest a moving catch or concealed compartment. The mother-of-pearl badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			80.0m,
			true,
			false,
			"mother-of-pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_niello_silver_badge",
			"badge",
			"a niello silver badge",
			null,
			"A niello silver badge has a public face and a practical back, the two sides plainly made for different jobs. Pale silver brightened at its raised surfaces forms the visible body, with visible niello detail worked into the public face set where it will show against cloth. The worked parts of the niello silver badge are decorative, but none suggest a moving catch or concealed compartment. The niello silver badge is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			26.0,
			125.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_badge_gold_household_badge",
			"badge",
			"a gold household badge",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. A face broad enough to carry a clear visible device gives the face definition, while the reverse remains plain and workmanlike. Rubbing has brightened the exposed parts of the gold household badge without making it look neglected. The gold household badge reads as noble adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			30.0,
			420.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_bronze_hairpin",
			"hairpin",
			"a bronze hairpin",
			null,
			"A bronze hairpin is made for the hair or the side of the head, where balance matters as much as ornament. Brown-gold bronze with rubbed high points is worked into the visible end, while the gripping part is simpler and more direct. The hidden side of the bronze hairpin is plainer, so attention stays on a narrow working end and a more decorative visible head and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			18.0,
			18.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_silver_hairpin",
			"hairpin",
			"a silver hairpin",
			null,
			"A silver hairpin is made for the hair or the side of the head, where balance matters as much as ornament. Pale silver brightened at its raised surfaces is worked into the visible end, while the gripping part is simpler and more direct. Rubbing has brightened the exposed parts of the silver hairpin without making it look neglected. The silver hairpin leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			16.0,
			65.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_gold_hairpin",
			"hairpin",
			"a gold hairpin",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Deep yellow gold with a warm, steady shine gives the visible portion its colour, and the working end is kept clean of fussy decoration. The worked parts of the gold hairpin are decorative, but none suggest a moving catch or concealed compartment. The gold hairpin leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			16.0,
			260.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_jade_hairpin",
			"hairpin",
			"a jade hairpin",
			null,
			"The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Rubbing has brightened the exposed parts of the jade hairpin without making it look neglected. The jade hairpin reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			140.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_tortoiseshell_hairpin",
			"hairpin",
			"a tortoiseshell hairpin",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Tortoiseshell mottled between honey, brown, and smoke-dark patches gives the visible portion its colour, and the working end is kept clean of fussy decoration. Close inspection of the tortoiseshell hairpin shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			12.0,
			70.0m,
			true,
			false,
			"tortoiseshell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_bamboo_hairpin",
			"hairpin",
			"a bamboo hairpin",
			null,
			"A bamboo hairpin is made for the hair or the side of the head, where balance matters as much as ornament. Smooth bamboo with the joints left subtly visible is worked into the visible end, while the gripping part is simpler and more direct. The worked parts of the bamboo hairpin are decorative, but none suggest a moving catch or concealed compartment. The bamboo hairpin is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			8.0,
			8.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpin",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_silver_hairpins",
			"hairpin",
			"a pair of silver hairpins",
			null,
			"The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Small handling marks cross the pair of silver hairpins unevenly, especially where it would brush cloth or skin. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			32.0,
			140.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpins",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_jade_hairpins",
			"hairpin",
			"a pair of jade hairpins",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Green jade smoothed to a cool waxy finish gives the visible portion its colour, and the working end is kept clean of fussy decoration. The hidden side of the pair of jade hairpins is plainer, so attention stays on a narrow working end and a more decorative visible head and the visible finish. The pair of jade hairpins is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			36.0,
			260.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpins",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_boxwood_hair_comb",
			"comb",
			"a boxwood hair comb",
			null,
			"The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Fine wear gathers around the comb's highest places, leaving a narrow working end and a more decorative visible head easier to pick out. The boxwood hair comb reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			25.0,
			20.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_tortoiseshell_hair_comb",
			"comb",
			"a tortoiseshell hair comb",
			null,
			"A tortoiseshell hair comb would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as tortoiseshell mottled between honey, brown, and smoke-dark patches. The worked parts of the tortoiseshell hair comb are decorative, but none suggest a moving catch or concealed compartment. The tortoiseshell hair comb carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			22.0,
			110.0m,
			true,
			false,
			"tortoiseshell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_mother_of_pearl_comb",
			"comb",
			"a mother-of-pearl comb",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Mother-of-pearl flashing softly under the surface gives the visible portion its colour, and the working end is kept clean of fussy decoration. Close inspection of the mother-of-pearl comb shows small tool marks rather than cast-perfect smoothness. The mother-of-pearl comb reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			24.0,
			120.0m,
			true,
			false,
			"mother-of-pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_ivory_hair_combs",
			"comb",
			"a pair of ivory hair combs",
			null,
			"A pair of ivory hair combs is made for the hair or the side of the head, where balance matters as much as ornament. Close-grained ivory with a creamy, old polish is worked into the visible end, while the gripping part is simpler and more direct. Fine wear gathers around the comb's highest places, leaving a narrow working end and a more decorative visible head easier to pick out. The pair of ivory hair combs carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			42.0,
			180.0m,
			true,
			false,
			"ivory",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Combs",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_gold_hair_ornament",
			"ornament",
			"a gold hair ornament",
			null,
			"The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Rubbing has brightened the exposed parts of the gold hair ornament without making it look neglected. The gold hair ornament reads as noble adornment rather than a tool, charm, or container.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			20.0,
			320.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_silver_hair_plaque",
			"ornament",
			"a silver hair plaque",
			null,
			"A silver hair plaque would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as pale silver brightened at its raised surfaces. Rubbing has brightened the exposed parts of the silver hair plaque without making it look neglected. The silver hair plaque feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			22.0,
			95.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_motif_hair_ornament",
			"ornament",
			"a $motif hair ornament",
			null,
			"A $motif hair ornament would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as gold-washed silver, brighter on the raised places and paler at worn edges. Close inspection of the $motif hair ornament shows small tool marks rather than cast-perfect smoothness. The $motif hair ornament feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Tiny,
			ItemQuality.Good,
			24.0,
			240.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Ornament",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_silk_tied_hair_ornament",
			"ornament",
			"a silk-tied hair ornament",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Silk cord with a soft light across its twist gives the visible portion its colour, and the working end is kept clean of fussy decoration. Rubbing has brightened the exposed parts of the silk-tied hair ornament without making it look neglected. The silk-tied hair ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.Tiny,
			ItemQuality.Standard,
			12.0,
			24.0m,
			true,
			false,
			"silk",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Ornament",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_lacquered_bamboo_hair_comb",
			"comb",
			"a lacquered bamboo hair comb",
			null,
			"The piece narrows or curves where it must hold, then broadens into the part meant to be seen. A narrow working end and a more decorative visible head keeps the ornament from looking like an ordinary fastening. Small handling marks cross the lacquered bamboo hair comb unevenly, especially where it would brush cloth or skin. The lacquered bamboo hair comb is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			20.0,
			55.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_bronze_temple_rings",
			"ring",
			"a pair of bronze temple rings",
			null,
			"The piece narrows or curves where it must hold, then broadens into the part meant to be seen. Small side loops arranged to frame the face rather than hang low keeps the ornament from looking like an ordinary fastening. Rubbing has brightened the exposed parts of the pair of bronze temple rings without making it look neglected. The pair of bronze temple rings carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			42.0,
			28.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_silver_temple_rings",
			"ring",
			"a pair of silver temple rings",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Pale silver brightened at its raised surfaces gives the visible portion its colour, and the working end is kept clean of fussy decoration. Fine wear gathers around the ring's highest places, leaving small side loops arranged to frame the face rather than hang low easier to pick out. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			40.0,
			120.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_wire_temple_rings",
			"ring",
			"a pair of wire temple rings",
			null,
			"A pair of wire temple rings would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as warm reddish copper darkened in the recesses. The hidden side of the pair of wire temple rings is plainer, so attention stays on small side loops arranged to frame the face rather than hang low and the visible finish. The pair of wire temple rings leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			32.0,
			14.0m,
			true,
			false,
			"copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_pearl_temple_rings",
			"ring",
			"a pair of pearl temple rings",
			null,
			"A pair of pearl temple rings would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as small pearls with uneven but gentle lustre. Fine wear gathers around the ring's highest places, leaving small side loops arranged to frame the face rather than hang low easier to pick out. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			34.0,
			170.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_gilt_temple_rings",
			"ring",
			"a pair of gilt temple rings",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Bronze showing through a thin golden surface at nicks and bends gives the visible portion its colour, and the working end is kept clean of fussy decoration. Close inspection of the pair of gilt temple rings shows small tool marks rather than cast-perfect smoothness. The pair of gilt temple rings leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			44.0,
			130.0m,
			true,
			false,
			"gilded bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pair_of_bead_hung_temple_rings",
			"ring",
			"a pair of bead-hung temple rings",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Coloured glass polished smooth enough to glow at the edges gives the visible portion its colour, and the working end is kept clean of fussy decoration. Small handling marks cross the pair of bead-hung temple rings unevenly, especially where it would brush cloth or skin. The pair of bead-hung temple rings is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			46.0,
			36.0m,
			true,
			false,
			"glass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_plain_brass_circlet",
			"circlet",
			"a plain brass circlet",
			null,
			"A plain brass circlet is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The hidden side of the plain brass circlet is plainer, so attention stays on a stronger front line and the visible finish. The piece would signal ordinary market taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			70.0,
			46.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_silver_circlet",
			"circlet",
			"a silver circlet",
			null,
			"The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. A stronger front line breaks the line before it runs away into plain side pieces. Rubbing has brightened the exposed parts of the silver circlet without making it look neglected. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			75.0,
			180.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pearl_circlet",
			"circlet",
			"a pearl circlet",
			null,
			"A pearl circlet is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The worked parts of the pearl circlet are decorative, but none suggest a moving catch or concealed compartment. The pearl circlet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			62.0,
			360.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Circlet",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_gold_circlet",
			"circlet",
			"a gold circlet",
			null,
			"A gold circlet is shaped to frame the head without covering the face. The front carries the strongest detail in deep yellow gold with a warm, steady shine, while the sides are softened for wear with hair, cap, or veil. Small handling marks cross the gold circlet unevenly, especially where it would brush cloth or skin. The gold circlet is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			80.0,
			620.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_garnet_set_diadem",
			"diadem",
			"a garnet-set diadem",
			null,
			"A garnet-set diadem is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The hidden side of the garnet-set diadem is plainer, so attention stays on visible garnet detail worked into the public face and the visible finish. The garnet-set diadem feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			95.0,
			820.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Diadems",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Diadem",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_enamelled_diadem",
			"diadem",
			"an enamelled diadem",
			null,
			"An enamelled diadem is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The worked parts of the enamelled diadem are decorative, but none suggest a moving catch or concealed compartment. The enamelled diadem is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			80.0,
			480.0m,
			true,
			false,
			"enamel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Diadems",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Diadem",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_silver_gilt_coronet",
			"coronet",
			"a silver-gilt coronet",
			null,
			"The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. A stronger front line breaks the line before it runs away into plain side pieces. The worked parts of the silver-gilt coronet are decorative, but none suggest a moving catch or concealed compartment. The silver-gilt coronet feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			140.0,
			900.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Coronets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Coronet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_small_gold_coronet",
			"coronet",
			"a small gold coronet",
			null,
			"The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. A stronger front line breaks the line before it runs away into plain side pieces. Rubbing has brightened the exposed parts of the small gold coronet without making it look neglected. The small gold coronet reads as regalia adornment rather than a tool, charm, or container.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			150.0,
			1400.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Coronets",
				"Market / Jewellery / Regalia"
			],
			[
				"Holdable",
				"Wear_Coronet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_court_crown",
			"crown",
			"a court crown",
			null,
			"Worn high, this piece would alter the outline of the head more than it would weigh it down. Its material shows as deep yellow gold with a warm, steady shine, with plainer work where it would be hidden by hair or cloth. Close inspection of the court crown shows small tool marks rather than cast-perfect smoothness. The court crown leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			450.0,
			3500.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Crowns",
				"Market / Jewellery / Regalia"
			],
			[
				"Holdable",
				"Wear_Crown",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_jade_forehead_ornament",
			"ornament",
			"a jade forehead ornament",
			null,
			"A jade forehead ornament is shaped to frame the head without covering the face. The front carries the strongest detail in green jade smoothed to a cool waxy finish, while the sides are softened for wear with hair, cap, or veil. Fine wear gathers around the ornament's highest places, leaving a stronger front line easier to pick out. The jade forehead ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			28.0,
			160.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_gold_forehead_pendant",
			"ornament",
			"a gold forehead pendant",
			null,
			"A gold forehead pendant is shaped to frame the head without covering the face. The front carries the strongest detail in deep yellow gold with a warm, steady shine, while the sides are softened for wear with hair, cap, or veil. Close inspection of the gold forehead pendant shows small tool marks rather than cast-perfect smoothness. The gold forehead pendant is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			35.0,
			320.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_pearl_forehead_ornament",
			"ornament",
			"a pearl forehead ornament",
			null,
			"A pearl forehead ornament is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The worked parts of the pearl forehead ornament are decorative, but none suggest a moving catch or concealed compartment. The pearl forehead ornament carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			30.0,
			240.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_silk_ribbon_chaplet",
			"chaplet",
			"a silk ribbon chaplet",
			null,
			"A silk ribbon chaplet is gathered from silk cord with a soft light across its twist, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. The worked parts of the silk ribbon chaplet are decorative, but none suggest a moving catch or concealed compartment. The silk ribbon chaplet carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			22.0,
			18.0m,
			true,
			false,
			"silk",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_fresh_flower_chaplet",
			"chaplet",
			"a fresh flower chaplet",
			null,
			"The piece is light and pliant, built from fresh flowers still full in the petals and green at the stems rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. Rubbing has brightened the exposed parts of the fresh flower chaplet without making it look neglected. The piece would signal festival taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			30.0,
			3.0m,
			true,
			false,
			"fresh flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_woven_straw_chaplet",
			"chaplet",
			"a woven straw chaplet",
			null,
			"A woven straw chaplet has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. Close inspection of the woven straw chaplet shows small tool marks rather than cast-perfect smoothness. The woven straw chaplet feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			16.0,
			1.0m,
			true,
			false,
			"straw",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Chaplets",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Chaplet",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_laurel_festival_wreath",
			"wreath",
			"a laurel festival wreath",
			null,
			"The piece is light and pliant, built from laurel leaves laid in a clean repeating line rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. The worked parts of the laurel festival wreath are decorative, but none suggest a moving catch or concealed compartment. The laurel festival wreath reads as festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			35.0,
			4.0m,
			true,
			false,
			"laurel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_ivy_head_wreath",
			"wreath",
			"an ivy head wreath",
			null,
			"An ivy head wreath has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The worked parts of the ivy head wreath are decorative, but none suggest a moving catch or concealed compartment. The ivy head wreath leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			34.0,
			3.0m,
			true,
			false,
			"ivy",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_fresh_blossom_head_garland",
			"garland",
			"a fresh blossom head garland",
			null,
			"A fresh blossom head garland has the slightly fragile look of festival jewellery, all colour, stem, and soft pressure. The maker has used the natural bends of the material rather than forcing it into a perfect circle. The hidden side of the fresh blossom head garland is plainer, so attention stays on a flexible line made to settle into hair or a veil and the visible finish. The fresh blossom head garland feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			32.0,
			4.0m,
			true,
			false,
			"blossom",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_jasmine_head_garland",
			"garland",
			"a jasmine head garland",
			null,
			"A jasmine head garland is gathered from jasmine flowers strung in small pale stars, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. Close inspection of the jasmine head garland shows small tool marks rather than cast-perfect smoothness. The jasmine head garland reads as festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			28.0,
			5.0m,
			true,
			false,
			"jasmine",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_marigold_head_garland",
			"garland",
			"a marigold head garland",
			null,
			"The piece is light and pliant, built from marigold heads crowded in warm ruffled knots rather than metal or stone. Its best-looking blossoms and leaves have been turned outward, while the knots and small crushed places hide nearer the wearer. Small handling marks cross the marigold head garland unevenly, especially where it would brush cloth or skin. The marigold head garland feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			34.0,
			5.0m,
			true,
			false,
			"marigold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_flower_head_garland",
			"garland",
			"a $flower head garland",
			null,
			"A $flower head garland is gathered from fresh flowers still full in the petals and green at the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. Small handling marks cross the $flower head garland unevenly, especially where it would brush cloth or skin. The $flower head garland feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			32.0,
			4.0m,
			true,
			false,
			"fresh flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Garlands",
				"Market / Jewellery / Festival Jewellery"
			],
			[
				"Holdable",
				"Wear_Head_Garland",
				"Destroyable_Clothing",
				"Variable_Flower"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_head_dried_flower_wreath",
			"wreath",
			"a dried flower wreath",
			null,
			"A dried flower wreath is gathered from dried flowers papery at the edges and brittle along the stems, the stems tucked back through the binding instead of hidden under a clasp. Small irregularities show where fingers have hurried the work, leaving some petals proud and others pressed down. Rubbing has brightened the exposed parts of the dried flower wreath without making it look neglected. The dried flower wreath reads as temporary festival adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			24.0,
			1.0m,
			true,
			false,
			"dried flower",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Wreaths",
				"Market / Jewellery / Ephemeral Jewellery"
			],
			[
				"Holdable",
				"Wear_Wreath",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_bronze_waist_chain",
			"chain",
			"a bronze waist chain",
			null,
			"A bronze waist chain reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Rubbing has brightened the exposed parts of the bronze waist chain without making it look neglected. The bronze waist chain carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			60.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_silver_waist_chain",
			"chain",
			"a silver waist chain",
			null,
			"A silver waist chain reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the chain's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The silver waist chain reads as wealthy adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			95.0,
			180.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_gold_waist_chain",
			"chain",
			"a gold waist chain",
			null,
			"A gold waist chain reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the chain's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The piece would signal noble taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			110.0,
			620.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_brass_merchant_waist_chain",
			"chain",
			"a brass merchant waist chain",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. The hidden side of the brass merchant waist chain is plainer, so attention stays on small plates or links spaced to move against the garment and the visible finish. The brass merchant waist chain is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			120.0,
			85.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_coral_hung_waist_chain",
			"chain",
			"a coral-hung waist chain",
			null,
			"The object has the careful rhythm of a worn display piece. Pale silver brightened at its raised surfaces catches most strongly on the raised or dangling parts, while the back remains workmanlike. Small handling marks cross the coral-hung waist chain unevenly, especially where it would brush cloth or skin. The coral-hung waist chain leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			105.0,
			240.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_bell_less_silver_girdle_chain",
			"chain",
			"a bell-less silver girdle chain",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Close inspection of the bell-less silver girdle chain shows small tool marks rather than cast-perfect smoothness. The bell-less silver girdle chain feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			100.0,
			190.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_finish_bronze_waist_chain",
			"chain",
			"a $finish bronze waist chain",
			null,
			"A $finish bronze waist chain is meant to sit at the belt line, where movement would make small details catch the eye. Brown-gold bronze with rubbed high points forms the visible parts, with the chosen finish left deliberately visible on the worked surface interrupting the line. Small handling marks cross the $finish bronze waist chain unevenly, especially where it would brush cloth or skin. The $finish bronze waist chain carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			110.0,
			70.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal",
				"Variable_MetalFinish"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_silk_tied_waist_chain",
			"chain",
			"a silk-tied waist chain",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Close inspection of the silk-tied waist chain shows small tool marks rather than cast-perfect smoothness. The silk-tied waist chain is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			35.0,
			24.0m,
			true,
			false,
			"silk",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_silver_girdle_ornament",
			"ornament",
			"a silver girdle ornament",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Rubbing has brightened the exposed parts of the silver girdle ornament without making it look neglected. The silver girdle ornament leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			45.0,
			120.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_brass_girdle_ornament",
			"ornament",
			"a brass girdle ornament",
			null,
			"A brass girdle ornament reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Small handling marks cross the brass girdle ornament unevenly, especially where it would brush cloth or skin. The brass girdle ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			50.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_gold_girdle_pendant",
			"ornament",
			"a gold girdle pendant",
			null,
			"A gold girdle pendant is meant to sit at the belt line, where movement would make small details catch the eye. Deep yellow gold with a warm, steady shine forms the visible parts, with small plates or links spaced to move against the garment interrupting the line. Rubbing has brightened the exposed parts of the gold girdle pendant without making it look neglected. The piece would signal noble taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			52.0,
			350.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_enamel_girdle_mount",
			"ornament",
			"an enamel girdle mount",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Rubbing has brightened the exposed parts of the enamel girdle mount without making it look neglected. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			42.0,
			110.0m,
			true,
			false,
			"enamel",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_coral_girdle_ornament",
			"ornament",
			"a coral girdle ornament",
			null,
			"A coral girdle ornament reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the ornament's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The coral girdle ornament is meant to be noticed in wear, but nothing about it implies armour or magic.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			36.0,
			95.0m,
			true,
			false,
			"coral",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_pewter_girdle_tag",
			"ornament",
			"a pewter girdle tag",
			null,
			"The object has the careful rhythm of a worn display piece. Soft grey pewter with a low satin sheen catches most strongly on the raised or dangling parts, while the back remains workmanlike. Close inspection of the pewter girdle tag shows small tool marks rather than cast-perfect smoothness. The piece would signal commoner taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			36.0,
			12.0m,
			true,
			false,
			"pewter",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_motif_girdle_ornament",
			"ornament",
			"a $motif girdle ornament",
			null,
			"The object has the careful rhythm of a worn display piece. Pale silver brightened at its raised surfaces catches most strongly on the raised or dangling parts, while the back remains workmanlike. The worked parts of the $motif girdle ornament are decorative, but none suggest a moving catch or concealed compartment. The piece would signal merchant taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			44.0,
			90.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryMotif"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_silver_gilt_girdle_jewel",
			"ornament",
			"a silver-gilt girdle jewel",
			null,
			"The object has the careful rhythm of a worn display piece. Gold-washed silver, brighter on the raised places and paler at worn edges catches most strongly on the raised or dangling parts, while the back remains workmanlike. The hidden side of the silver-gilt girdle jewel is plainer, so attention stays on small plates or links spaced to move against the garment and the visible finish. The silver-gilt girdle jewel carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			54.0,
			260.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_bronze_belt_ornament",
			"ornament",
			"a bronze belt ornament",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Rubbing has brightened the exposed parts of the bronze belt ornament without making it look neglected. The bronze belt ornament reads as ordinary market adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			32.0,
			24.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_brass_belt_jewel",
			"ornament",
			"a brass belt jewel",
			null,
			"A brass belt jewel reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Rubbing has brightened the exposed parts of the brass belt jewel without making it look neglected. The brass belt jewel leaves room for local marks or motifs without needing a different base object.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			30.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Merchant Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_silver_belt_ornament",
			"ornament",
			"a silver belt ornament",
			null,
			"A silver belt ornament reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Close inspection of the silver belt ornament shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			28.0,
			80.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_gilded_copper_belt_mount",
			"ornament",
			"a gilded copper belt mount",
			null,
			"The object has the careful rhythm of a worn display piece. Coppery red metal visible beneath worn gilt edges catches most strongly on the raised or dangling parts, while the back remains workmanlike. Fine wear gathers around the ornament's highest places, leaving small plates or links spaced to move against the garment easier to pick out. The gilded copper belt mount feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			34.0,
			40.0m,
			true,
			false,
			"gilded copper",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_shape_belt_ornament",
			"ornament",
			"a $shape belt ornament",
			null,
			"A $shape belt ornament is meant to sit at the belt line, where movement would make small details catch the eye. Gold-washed silver, brighter on the raised places and paler at worn edges forms the visible parts, with the chosen shape giving the object its main outline interrupting the line. The hidden side of the $shape belt ornament is plainer, so attention stays on the chosen shape giving the object its main outline and the visible finish. The $shape belt ornament reads as courtly adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			36.0,
			190.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Ornament",
				"Destroyable_HeavyMetal",
				"Variable_JewelleryShape"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_set_of_bronze_belt_plaques",
			"plaque",
			"a set of bronze belt plaques",
			null,
			"The object has the careful rhythm of a worn display piece. Brown-gold bronze with rubbed high points catches most strongly on the raised or dangling parts, while the back remains workmanlike. Close inspection of the set of bronze belt plaques shows small tool marks rather than cast-perfect smoothness. The set of bronze belt plaques feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			130.0,
			86.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Standard Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_set_of_silver_belt_plaques",
			"plaque",
			"a set of silver belt plaques",
			null,
			"A set of silver belt plaques reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. The hidden side of the set of silver belt plaques is plainer, so attention stays on small plates or links spaced to move against the garment and the visible finish. The set of silver belt plaques carries value through visible material and workmanship, not through hidden mechanism.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			120.0,
			240.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_set_of_gilded_belt_plaques",
			"plaque",
			"a set of gilded belt plaques",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Small handling marks cross the set of gilded belt plaques unevenly, especially where it would brush cloth or skin. The set of gilded belt plaques feels selected for appearance first, with practical wearing kept secondary.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			125.0,
			180.0m,
			true,
			false,
			"gilded bronze",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_steppe_belt_plaque_set",
			"plaque",
			"a steppe belt plaque set",
			null,
			"The waist ornament is spaced to show against cloth rather than cover it completely. Its practical side is simple; the outward face carries the polish, links, or plates. Close inspection of the steppe belt plaque set shows small tool marks rather than cast-perfect smoothness. The piece would signal wealthy taste while remaining straightforward jewellery.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			140.0,
			280.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Belt Plaques",
				"Market / Jewellery / Luxury Jewellery"
			],
			[
				"Holdable",
				"Wear_Belt_Plaques",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_waist_leather_waist_ornament",
			"ornament",
			"a leather waist ornament",
			null,
			"A leather waist ornament is meant to sit at the belt line, where movement would make small details catch the eye. Worked leather with creases at the folds forms the visible parts, with small plates or links spaced to move against the garment interrupting the line. The worked parts of the leather waist ornament are decorative, but none suggest a moving catch or concealed compartment. The leather waist ornament reads as commoner adornment rather than a tool, charm, or container.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			40.0,
			10.0m,
			true,
			false,
			"leather",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Ornaments",
				"Market / Jewellery / Commoner Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Ornament",
				"Destroyable_Clothing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_jewelled_gold_court_collar",
			"collar",
			"a jewelled gold court collar",
			null,
			"A jewelled gold court collar carries its value in the rhythm of its material: deep yellow gold with a warm, steady shine. The elements are not all identical, but the spacing is careful enough that the variation feels intentional. The hidden side of the jewelled gold court collar is plainer, so attention stays on craft details crowded into the most visible face of the piece and the visible finish. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			320.0,
			2400.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_pearl_and_gold_collar",
			"collar",
			"a pearl and gold collar",
			null,
			"Seen flat, pearl and gold collar shows a slight unevenness that would soften once worn. The front has the best polish and spacing, while the back is plainer and more practical. Fine wear gathers around the collar's highest places, leaving visible pearl detail worked into the public face easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			260.0,
			1900.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_gemstone_circlet",
			"circlet",
			"a gemstone circlet",
			null,
			"The ornament has a clear front, and it is there that the maker has concentrated the polish and pattern. Craft details crowded into the most visible face of the piece breaks the line before it runs away into plain side pieces. The worked parts of the gemstone circlet are decorative, but none suggest a moving catch or concealed compartment. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			120.0,
			1800.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Circlets",
				"Market / Jewellery / Regalia"
			],
			[
				"Holdable",
				"Wear_Circlet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_sapphire_diadem",
			"diadem",
			"a sapphire diadem",
			null,
			"Worn high, this piece would alter the outline of the head more than it would weigh it down. Its material shows as deep yellow gold with a warm, steady shine, with plainer work where it would be hidden by hair or cloth. Fine wear gathers around the diadem's highest places, leaving visible sapphire detail worked into the public face easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			110.0,
			2200.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Diadems",
				"Market / Jewellery / Regalia"
			],
			[
				"Holdable",
				"Wear_Diadem",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_ruby_coronet",
			"coronet",
			"a ruby coronet",
			null,
			"A ruby coronet is more about framing and status than bulk. The construction avoids armour-like coverage, keeping the ornament open, visible, and unmistakably decorative. The hidden side of the ruby coronet is plainer, so attention stays on visible ruby detail worked into the public face and the visible finish. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.Small,
			ItemQuality.VeryGood,
			170.0,
			2600.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Coronets",
				"Market / Jewellery / Regalia"
			],
			[
				"Holdable",
				"Wear_Coronet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_great_pearl_court_chain",
			"chain",
			"a great pearl court chain",
			null,
			"The great pearl court chain falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is small pearls with uneven but gentle lustre, with craft details crowded into the most visible face of the piece giving the eye a place to pause. Small handling marks cross the great pearl court chain unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			210.0,
			1600.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Necklaces",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Necklace",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_silver_gilt_noble_brooch",
			"brooch",
			"a silver-gilt noble brooch",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. Craft details crowded into the most visible face of the piece gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the silver-gilt noble brooch unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			95.0,
			520.0m,
			true,
			false,
			"silver-gilt",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_garnet_cloison_like_brooch",
			"brooch",
			"a garnet cloison-like brooch",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. Visible garnet detail worked into the public face gives the face definition, while the reverse remains plain and workmanlike. Close inspection of the garnet cloison-like brooch shows small tool marks rather than cast-perfect smoothness. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			88.0,
			900.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Brooches",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Brooch",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_lapis_court_badge",
			"badge",
			"a lapis court badge",
			null,
			"This is not merely a loose fitting; it is shaped to be noticed where it rests on a garment. Visible lapis detail worked into the public face gives the face definition, while the reverse remains plain and workmanlike. Small handling marks cross the lapis court badge unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			70.0,
			620.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Badges",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Badge",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_gold_temple_ring_set",
			"ring",
			"a gold temple-ring set",
			null,
			"A gold temple-ring set would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as deep yellow gold with a warm, steady shine. Rubbing has brightened the exposed parts of the gold temple-ring set without making it look neglected. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			80.0,
			740.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Temple Rings",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Temple_Rings",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_jade_court_hairpin_set",
			"hairpin",
			"a jade court hairpin set",
			null,
			"A jade court hairpin set would sit among hair, veil, or headcloth rather than hang freely. The public surface is polished more carefully than the hidden side, and the material reads as green jade smoothed to a cool waxy finish. The worked parts of the jade court hairpin set are decorative, but none suggest a moving catch or concealed compartment. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			48.0,
			420.0m,
			true,
			false,
			"jade",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Hairpins",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_tortoiseshell_court_comb",
			"comb",
			"a tortoiseshell court comb",
			null,
			"There is a quiet asymmetry to the ornament, useful for placing it by touch. Tortoiseshell mottled between honey, brown, and smoke-dark patches gives the visible portion its colour, and the working end is kept clean of fussy decoration. Fine wear gathers around the comb's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			32.0,
			220.0m,
			true,
			false,
			"tortoiseshell",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Hair Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Hair_Comb",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_heavy_silver_torc",
			"torc",
			"a heavy silver torc",
			null,
			"The heavy silver torc falls in a deliberate curve, not as a loose cord but as jewellery meant to frame the neckline. Its main body is pale silver brightened at its raised surfaces, with craft details crowded into the most visible face of the piece giving the eye a place to pause. Fine wear gathers around the torc's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			320.0,
			520.0m,
			true,
			false,
			"silver",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Torcs",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Torc",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_enamelled_gold_bracelet",
			"bracelet",
			"an enamelled gold bracelet",
			null,
			"An enamelled gold bracelet would show whenever a sleeve, hem, or hand moved aside. The maker has balanced ornament against comfort, keeping the underside smoother than the display face. Small handling marks cross the enamelled gold bracelet unevenly, especially where it would brush cloth or skin. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			85.0,
			650.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Bracelets",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Bracelet",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_pearl_forehead_chain",
			"ornament",
			"a pearl forehead chain",
			null,
			"Worn high, this piece would alter the outline of the head more than it would weigh it down. Its material shows as small pearls with uneven but gentle lustre, with plainer work where it would be hidden by hair or cloth. Close inspection of the pearl forehead chain shows small tool marks rather than cast-perfect smoothness. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			44.0,
			420.0m,
			true,
			false,
			"pearl",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Forehead Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Forehead_Ornament",
				"Destroyable_Misc"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_noble_gold_waist_chain",
			"chain",
			"a noble gold waist chain",
			null,
			"The object has the careful rhythm of a worn display piece. Deep yellow gold with a warm, steady shine catches most strongly on the raised or dangling parts, while the back remains workmanlike. Fine wear gathers around the chain's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			180.0,
			1300.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Waist Chains",
				"Market / Jewellery / Noble Jewellery"
			],
			[
				"Holdable",
				"Wear_Waist_Chain",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_court_girdle_jewel",
			"ornament",
			"a court girdle jewel",
			null,
			"A court girdle jewel reads as jewellery, not as a carrying belt. The pieces are decorative and visible, with no hardware that would suggest hidden storage or load-bearing use. Fine wear gathers around the ornament's highest places, leaving craft details crowded into the most visible face of the piece easier to pick out. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.VerySmall,
			ItemQuality.VeryGood,
			72.0,
			720.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Girdle Ornaments",
				"Market / Jewellery / Court Jewellery"
			],
			[
				"Holdable",
				"Wear_Girdle_Ornament",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_jewellery_elite_royal_seal_signet_ring",
			"ring",
			"a royal seal signet ring",
			null,
			"A royal seal signet ring is a compact band of deep yellow gold with a warm, steady shine, made with craft details crowded into the most visible face of the piece rather than a perfectly anonymous surface. The inside has been smoothed more carefully than the outside, so the hand feels more work than the eye first sees. Close inspection of the royal seal signet ring shows small tool marks rather than cast-perfect smoothness. It reads as court or treasury jewellery rather than a market trinket.",
			SizeCategory.Tiny,
			ItemQuality.VeryGood,
			26.0,
			1100.0m,
			true,
			false,
			"gold",
			[
				"Functions / Worn Items / Jewellery",
				"Functions / Worn Items / Jewellery / Rings",
				"Market / Jewellery / Regalia"
			],
			[
				"Holdable",
				"Wear_Ring",
				"Destroyable_HeavyMetal",
				"SealStamp_Medieval_NobleSignetRing"
			],
			null,
			null,
			null,
			null,
			null
		);

		CreateItem(
			"medieval_locking_religious_alms_chest",
			"chest",
			"a heavy alms chest",
			null,
			"This heavy alms chest is a large, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			34000.0,
			280.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Large locking chest for alms, gifts, and institutional funds."
		);

		CreateItem(
			"medieval_locking_religious_donation_box",
			"box",
			"a locking donation box",
			null,
			"This locking donation box is a small, workmanlike box built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			4200.0,
			80.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Built-in-lock donation box for coins or offerings."
		);

		CreateItem(
			"medieval_locking_religious_reliquary_lockbox",
			"lockbox",
			"a locked reliquary box",
			null,
			"This locked reliquary box is a small, well-made lockbox built from oak boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3200.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small built-in-lock box for relics, seals, or sacred keepsakes as stored contents."
		);

		CreateItem(
			"medieval_locking_religious_scripture_chest",
			"chest",
			"a locking scripture chest",
			null,
			"This locking scripture chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			28000.0,
			260.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for books, scrolls, tablets, or wrapped texts."
		);

		CreateItem(
			"medieval_locking_religious_treasury_chest",
			"chest",
			"a sanctuary treasury chest",
			null,
			"This sanctuary treasury chest is a large, well-made chest worked from wrought iron. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Good,
			68000.0,
			720.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Heavy safe-chest profile for a religious treasury or secure sacristy room."
		);

		CreateItem(
			"medieval_religious_angled_scripture_lectern",
			"lectern",
			"an angled scripture lectern",
			null,
			"This angled scripture lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8800.0,
			55.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Sloped reading surface for large sacred books, scrolls, tablets, or sermon notes."
		);

		CreateItem(
			"medieval_religious_bronze_incense_burner",
			"burner",
			"a bronze incense burner",
			null,
			"This bronze incense burner is a small, workmanlike burner worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1600.0,
			32.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Small solid-fuel burner for charcoal and incense; not a dry container while using brazier behaviour."
		);

		CreateItem(
			"medieval_religious_bronze_offering_tray",
			"tray",
			"a bronze offering tray",
			null,
			"This bronze offering tray is a medium-sized, workmanlike tray worked from bronze. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			1700.0,
			36.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Flat tray for ritual food portions, flowers, incense packets, tapers, or carried offerings."
		);

		CreateItem(
			"medieval_religious_glass_relic_case",
			"case",
			"a glass relic case",
			null,
			"This glass relic case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			90.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Display_Case"
			],
			null,
			null,
			null,
			null,
			"Transparent display case for reliquaries, memorial objects, sealed offerings, or other protected sacred pieces."
		);

		CreateItem(
			"medieval_religious_hanging_censer",
			"censer",
			"a hanging bronze censer",
			null,
			"This hanging bronze censer is a small, well-made censer worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1250.0,
			48.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Suspended censer for incense smoke, modelled as a small solid-fuel burner rather than as jewellery or a personal charm."
		);

		CreateItem(
			"medieval_religious_hanging_oil_lamp",
			"lamp",
			"a hanging oil lamp",
			null,
			"This hanging oil lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			42.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Suspended liquid-fuel lamp for a sanctuary, shrine, or religious hall."
		);

		CreateItem(
			"medieval_religious_incense_box",
			"box",
			"a cedar incense box",
			null,
			"This cedar incense box is a small, workmanlike box built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			24.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Lidded dry container for incense chips, resins, powdered aromatics, or fragrant woods."
		);

		CreateItem(
			"medieval_religious_incense_cabinet",
			"cabinet",
			"an incense-store cabinet",
			null,
			"This incense-store cabinet is a medium-sized, well-made cabinet built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			60.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Small cabinet for incense, tapers, ritual oil flasks, and other aromatic service supplies."
		);

		CreateItem(
			"medieval_religious_lamp_tray",
			"tray",
			"a bronze lamp tray",
			null,
			"This bronze lamp tray is a medium-sized, workmanlike tray worked from bronze. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Its formal proportions give it a public, ritual, and institutional presence. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2100.0,
			30.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray for arranging oil lamps, candle cups, wicks, and lighting supplies; inert unless separate lamps are placed on it."
		);

		CreateItem(
			"medieval_religious_long_altar_candle",
			"candle",
			"a long altar candle",
			null,
			"This long altar candle is a small, well-made candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			260.0,
			8.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
			null,
			null,
			null,
			null,
			"Longer beeswax candle suited to altar, shrine, memorial, or festival lighting."
		);

		CreateItem(
			"medieval_religious_long_congregation_bench",
			"bench",
			"a long congregation bench",
			null,
			"This long congregation bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			24000.0,
			60.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Long religious-hall bench with ordinary bench seating and a surface that can accept small placed objects."
		);

		CreateItem(
			"medieval_religious_low_offering_stand",
			"stand",
			"a low offering stand",
			null,
			"This low offering stand is a medium-sized, workmanlike stand built from beech boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			24.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Temple Offerings"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Low surface for offerings, lamps, incense bowls, small icons, tablets, or ritual vessels."
		);

		CreateItem(
			"medieval_religious_lustral_water_bucket",
			"bucket",
			"a lustral water bucket",
			null,
			"This lustral water bucket is a medium-sized, well-made bucket worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2800.0,
			52.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Handled liquid vessel for sprinkled, poured, or distributed sacred water."
		);

		CreateItem(
			"medieval_religious_oak_offering_table",
			"table",
			"an oak offering table",
			null,
			"This oak offering table is a large, workmanlike table built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			70.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Temple Offerings"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"General offering or service table for religious halls, chapels, shrines, and household ritual rooms."
		);

		CreateItem(
			"medieval_religious_offering_bowl",
			"bowl",
			"a bronze offering bowl",
			null,
			"This bronze offering bowl is a small, workmanlike bowl worked from bronze. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			28.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for grain, flowers, ashes, small offerings, or other solid ritual goods."
		);

		CreateItem(
			"medieval_religious_oil_cruet",
			"cruet",
			"a small ritual oil cruet",
			null,
			"This small ritual oil cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			220.0,
			28.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Small liquid container for lamp oil, chrism-like oil, perfumed oil, or anointing liquid."
		);

		CreateItem(
			"medieval_religious_open_devotional_shelves",
			"shelves",
			"open devotional shelves",
			null,
			"These open devotional shelves are large, workmanlike shelves built from oak boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			14500.0,
			48.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open shelves for religious books, lamps, offering bowls, memorial tablets, or displayed sacred objects."
		);

		CreateItem(
			"medieval_religious_painted_screen_panel",
			"screen",
			"a painted shrine screen",
			null,
			"This painted shrine screen is a large, workmanlike screen built from pine boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			45.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Portable screen panel for marking off a shrine, altar side, vestry corner, or sacred display area."
		);

		CreateItem(
			"medieval_religious_plain_oil_lamp",
			"lamp",
			"a plain ritual oil lamp",
			null,
			"This plain ritual oil lamp is a small, workmanlike lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			26.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Liquid-fuel lamp for altars, shrines, chapels, and religious household rooms."
		);

		CreateItem(
			"medieval_religious_plain_prayer_bench",
			"bench",
			"a plain prayer bench",
			null,
			"This plain prayer bench is a medium-sized, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5500.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Compact kneeling or seated prayer bench; a furnishing rather than a personal devotional object."
		);

		CreateItem(
			"medieval_religious_ritual_supply_cupboard",
			"cupboard",
			"a ritual supply cupboard",
			null,
			"This ritual supply cupboard is a large, workmanlike cupboard built from pine boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18500.0,
			55.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"General cupboard for candles, cloths, incense, oil flasks, basins, and other communal religious supplies."
		);

		CreateItem(
			"medieval_religious_ritual_washing_basin",
			"basin",
			"a ritual washing basin",
			null,
			"This ritual washing basin is a medium-sized, workmanlike basin formed from ceramic. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			26.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Water-holding basin for ablution, lustration, or ceremonial washing."
		);

		CreateItem(
			"medieval_religious_scripture_cabinet",
			"cabinet",
			"a scripture cabinet",
			null,
			"This scripture cabinet is a large, well-made cabinet built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			85.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cabinet for sacred books, scroll cases, tablets, commentaries, or ritual documents."
		);

		CreateItem(
			"medieval_religious_simple_reading_stand",
			"stand",
			"a simple reading stand",
			null,
			"This simple reading stand is a medium-sized, workmanlike stand built from pine boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			15.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Portable sloped stand for a book, scroll, scripture bundle, or ritual text."
		);

		CreateItem(
			"medieval_religious_standing_censer",
			"censer",
			"a standing iron censer",
			null,
			"This standing iron censer is a medium-sized, workmanlike censer worked from wrought iron. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5200.0,
			36.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Freestanding incense burner for shrines, halls, porches, or side chapels."
		);

		CreateItem(
			"medieval_religious_stone_offering_basin",
			"basin",
			"a stone offering basin",
			null,
			"This stone offering basin is a medium-sized, workmanlike basin cut from limestone. The eating surface is shallow and broad, with a raised rim and a flat underside. Wear is most visible where knives and fingers have crossed the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			9800.0,
			35.0m,
			true,
			false,
			"limestone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Broad solid-offering basin for dry offerings, flower heads, grains, salt, ash, or votive tokens."
		);

		CreateItem(
			"medieval_religious_votive_candle",
			"candle",
			"a beeswax votive candle",
			null,
			"This beeswax votive candle is a very small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.VerySmall,
			ItemQuality.Standard,
			80.0,
			2.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle"
			],
			null,
			null,
			null,
			null,
			"Small timed candle for votive, memorial, or offering use."
		);

		CreateItem(
			"medieval_religious_wall_devotional_panel",
			"panel",
			"a wall devotional panel",
			null,
			"This wall devotional panel is a medium-sized, workmanlike panel built from pine boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3200.0,
			22.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Wall-hung devotional panel suitable for painted, carved, inscribed, or skinned religious imagery."
		);

		CreateItem(
			"medieval_religious_wall_shrine_shelf",
			"shelf",
			"a wall shrine shelf",
			null,
			"This wall shrine shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2300.0,
			34.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Narrow wall shelf intended to hold lamps, offerings, icons, tablets, or small religious vessels."
		);

		CreateItem(
			"medieval_religious_water_ewer",
			"ewer",
			"a ritual water ewer",
			null,
			"This ritual water ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for water, diluted wine, or lustral liquid in a religious setting."
		);

		CreateItem(
			"medieval_church_altar_table",
			"altar",
			"an oak church altar",
			null,
			"This oak church altar is a large, well-made altar built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			160.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Substantial altar table with surface behaviour for chalices, books, candles, or offerings."
		);

		CreateItem(
			"medieval_church_aspersory_bucket",
			"bucket",
			"a bronze aspersory bucket",
			null,
			"This bronze aspersory bucket is a medium-sized, well-made bucket worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			60.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Portable holy-water vessel for sprinkling rites; no personal-wear or jewellery behaviour."
		);

		CreateItem(
			"medieval_church_baptismal_ewer",
			"ewer",
			"a brass baptismal ewer",
			null,
			"This brass baptismal ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for baptismal or washing water."
		);

		CreateItem(
			"medieval_church_bronze_thurible",
			"thurible",
			"a bronze thurible",
			null,
			"This bronze thurible is a small, well-made thurible worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			65.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Swinging incense censer represented as a small solid-fuel burner, not as a wearable or jewellery item."
		);

		CreateItem(
			"medieval_church_candle_pricket_stand",
			"stand",
			"a candle-pricket stand",
			null,
			"This candle-pricket stand is a medium-sized, workmanlike stand worked from wrought iron. A straight support rises from a steady base to a shallow socket at the top. Soot marks gather around the upper cup. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5600.0,
			42.0m,
			true,
			false,
			"wrought iron",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Iron candle stand surface for setting tapers or votive lights; separate candles provide actual light."
		);

		CreateItem(
			"medieval_church_choir_stall",
			"stall",
			"a carved choir stall",
			null,
			"This carved choir stall is a large, well-made stall built from oak boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			120.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Individual choir or clerical seat with carved timberwork; not an outfit or personal effect."
		);

		CreateItem(
			"medieval_church_eagle_lectern",
			"lectern",
			"a brass eagle lectern",
			null,
			"This brass eagle lectern is a large, well-made lectern worked from brass. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			210.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Recognisable church lectern form with a broad reading surface; the bird detail is decorative only."
		);

		CreateItem(
			"medieval_church_funeral_bier",
			"bier",
			"a plain funeral bier",
			null,
			"This plain funeral bier is a large, workmanlike bier built from oak boards. A long flat platform rests on carrying rails, with low side edges and a plain upper surface. The handles are smoothed where bearers grip them. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			16000.0,
			54.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Funerary Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Church bier or trestled surface for funerary display and procession preparation."
		);

		CreateItem(
			"medieval_church_gospel_book_stand",
			"stand",
			"a gospel book stand",
			null,
			"This gospel book stand is a medium-sized, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4200.0,
			55.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Portable stand for a large service book or gospel volume."
		);

		CreateItem(
			"medieval_church_holy_water_stoup",
			"stoup",
			"a holy-water stoup",
			null,
			"This holy-water stoup is a medium-sized, well-made stoup cut from marble. A cupped basin sits on a sturdy bracket, with a thick rim around the water hollow. The inner surface is polished from repeated touch. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8600.0,
			90.0m,
			true,
			false,
			"marble",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Amphora_Sextarius"
			],
			null,
			null,
			null,
			null,
			"Small stone holy-water vessel for a church entrance or chapel wall."
		);

		CreateItem(
			"medieval_church_incense_boat",
			"box",
			"an incense boat box",
			null,
			"This incense boat box is a very small, well-made box worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			420.0,
			36.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small lidded container for incense grains or aromatic resin beside a thurible."
		);

		CreateItem(
			"medieval_church_kneeling_bench",
			"kneeler",
			"a low kneeling bench",
			null,
			"This low kneeling bench is a medium-sized, workmanlike kneeler built from pine boards. A single seat is set between legs, with a back support rising behind it. The arms and front edge are smoothed where hands have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			18.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Low church kneeler or prayer bench for posture and room furnishing."
		);

		CreateItem(
			"medieval_church_oak_pew",
			"pew",
			"an oak church pew",
			null,
			"This oak church pew is a large, workmanlike pew built from oak boards. A long seat runs between upright ends, with a straight back board and a narrow ledge along the rear. The front edge is polished by use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			30000.0,
			85.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Long church seating form, using bench mechanics and a narrow surface for small placed objects."
		);

		CreateItem(
			"medieval_church_pulpit",
			"pulpit",
			"a carved wooden pulpit",
			null,
			"This carved wooden pulpit is a large, well-made pulpit built from oak boards. A raised speaking enclosure stands above a small base, with a front panel and a narrow ledge for a book. The upper rail is polished by hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			32000.0,
			150.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Luxury Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Raised preaching or reading furnishing with a sloped surface for notes or scripture."
		);

		CreateItem(
			"medieval_church_pyx_cupboard",
			"cupboard",
			"a pyx cupboard",
			null,
			"This pyx cupboard is a medium-sized, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			80.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Small church cupboard for pyxes, small vessels, folded cloths, and service goods."
		);

		CreateItem(
			"medieval_church_reliquary_display_case",
			"case",
			"a reliquary display case",
			null,
			"This reliquary display case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			120.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Display_Case"
			],
			null,
			null,
			null,
			null,
			"Transparent church display case for relic containers or visually important sacred objects."
		);

		CreateItem(
			"medieval_church_rood_screen_panel",
			"screen",
			"a carved rood-screen panel",
			null,
			"This carved rood-screen panel is a large, well-made screen built from oak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			130.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Decorative dividing screen panel for a church interior; inert unless a later pass gives it doorway behaviour."
		);

		CreateItem(
			"medieval_church_sacristy_cupboard",
			"cupboard",
			"a sacristy cupboard",
			null,
			"This sacristy cupboard is a large, well-made cupboard built from oak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			95.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Large service cupboard for candles, incense, linens, books, vessels, and other sacristy stores."
		);

		CreateItem(
			"medieval_church_sanctuary_lamp",
			"lamp",
			"a hanging sanctuary lamp",
			null,
			"This hanging sanctuary lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1400.0,
			70.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Hanging liquid-fuel sanctuary lamp suited to a church or chapel."
		);

		CreateItem(
			"medieval_church_stone_baptismal_font",
			"font",
			"a stone baptismal font",
			null,
			"This stone baptismal font is a large, workmanlike font cut from limestone. A cupped basin sits on a sturdy bracket, with a thick rim around the water hollow. The inner surface is polished from repeated touch. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Large,
			ItemQuality.Standard,
			58000.0,
			110.0m,
			true,
			false,
			"limestone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Large water-holding font for baptisms or ritual washing; liquid-container behaviour only."
		);

		CreateItem(
			"medieval_church_wall_crucifix",
			"crucifix",
			"a wall-hung crucifix",
			null,
			"This wall-hung crucifix is a medium-sized, workmanlike crucifix built from oak boards. A carved cross shape carries a raised central figure, with small fixing marks on the back. The front is smoothed around the most handled edges. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2400.0,
			36.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Wall-hung cruciform furnishing for churches, chapels, or domestic devotional rooms."
		);

		CreateItem(
			"medieval_locking_church_offertory_chest",
			"chest",
			"a locked offertory chest",
			null,
			"This locked offertory chest is a large, well-made chest built from oak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			32000.0,
			300.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking offertory chest for coin, gifts, or parish stores."
		);

		CreateItem(
			"medieval_locking_church_reliquary_casket",
			"casket",
			"a locked reliquary casket",
			null,
			"This locked reliquary casket is a small, well-made casket worked from bronze. A closed body surrounds a protected interior, and the lock plate is built directly into the front. The edges are reinforced where repeated opening has worn the finish. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			220.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockable casket for church relic storage and display-adjacent use."
		);

		CreateItem(
			"medieval_locking_church_sacristy_strongbox",
			"strongbox",
			"a sacristy strongbox",
			null,
			"This sacristy strongbox is a medium-sized, well-made strongbox built from oak boards. A heavy lid closes over a deep compartment, with a broad lock plate set into the front face. Reinforced corners and a thick base make the whole piece look difficult to force. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			18000.0,
			260.0m,
			true,
			false,
			"oak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking strongbox for a sacristy, vestry, or church office."
		);

		CreateItem(
			"medieval_eastern_christian_analogion_lectern",
			"lectern",
			"an angled analogion lectern",
			null,
			"This angled analogion lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			82.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Sloped lectern for service books or gospel texts."
		);

		CreateItem(
			"medieval_eastern_christian_baptismal_basin",
			"basin",
			"a bronze baptismal basin",
			null,
			"This bronze baptismal basin is a medium-sized, well-made basin worked from bronze. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			92.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Water-holding baptismal or washing basin for church use."
		);

		CreateItem(
			"medieval_eastern_christian_candle_sand_tray",
			"tray",
			"a candle-sand tray",
			null,
			"This candle-sand tray is a medium-sized, workmanlike tray worked from brass. Heat marks, soot-darkened edges, and a steady base make its use around flame immediately visible. Its formal proportions give it a public, ritual, and institutional presence. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3300.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray for sand, tapers, or votive candles; actual light comes from separate candle items."
		);

		CreateItem(
			"medieval_eastern_christian_chain_censer",
			"censer",
			"a chain-hung censer",
			null,
			"This chain-hung censer is a small, well-made censer worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1250.0,
			58.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Suspended church censer for charcoal and incense."
		);

		CreateItem(
			"medieval_eastern_christian_chanting_bench",
			"bench",
			"a chanting bench",
			null,
			"This chanting bench is a large, workmanlike bench built from beech boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			52.0m,
			true,
			false,
			"beech",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Two-place bench for singers or readers in a church interior."
		);

		CreateItem(
			"medieval_eastern_christian_chrism_cruet",
			"cruet",
			"a glass chrism cruet",
			null,
			"This glass chrism cruet is a very small, well-made cruet made from glass. A narrow neck rises from a compact body, with a fitted stopper seated in the mouth. The base is flat enough to stand on a shelf. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.VerySmall,
			ItemQuality.Good,
			180.0,
			30.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Flask"
			],
			null,
			null,
			null,
			null,
			"Small oil container for chrism-like or perfumed ritual oil."
		);

		CreateItem(
			"medieval_eastern_christian_gospel_lectern",
			"lectern",
			"a gospel lectern",
			null,
			"This gospel lectern is a medium-sized, well-made lectern built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			6800.0,
			70.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Reading surface for large sacred books and liturgical texts."
		);

		CreateItem(
			"medieval_eastern_christian_hanging_lamp",
			"lamp",
			"a hanging icon lamp",
			null,
			"This hanging icon lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			62.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Suspended liquid-fuel lamp suited to an icon or sanctuary area."
		);

		CreateItem(
			"medieval_eastern_christian_icon_stand",
			"stand",
			"a carved icon stand",
			null,
			"This carved icon stand is a medium-sized, well-made stand built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5600.0,
			70.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open display stand for one or more wall-sized icons or devotional panels."
		);

		CreateItem(
			"medieval_eastern_christian_iconostasis_panel",
			"panel",
			"an iconostasis panel",
			null,
			"This iconostasis panel is a large, well-made panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			17000.0,
			125.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Decorative screen panel used to define a sacred area, with skins able to supply exact painted imagery."
		);

		CreateItem(
			"medieval_eastern_christian_incense_casket",
			"casket",
			"an incense casket",
			null,
			"This incense casket is a small, well-made casket built from cedar boards. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			34.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Small dry container for incense, aromatic wood, resin, or grains."
		);

		CreateItem(
			"medieval_eastern_christian_prothesis_table",
			"table",
			"a prothesis service table",
			null,
			"This prothesis service table is a large, well-made table built from cypress boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			21000.0,
			120.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Side service table with surface behaviour for vessels, cloths, bread, or ritual equipment."
		);

		CreateItem(
			"medieval_eastern_christian_reliquary_icon_case",
			"case",
			"a reliquary icon case",
			null,
			"This reliquary icon case is a medium-sized, well-made case made from glass. A small framed compartment sits behind a guarded front, with the edges finished more carefully than the back. The base is steady enough for display. The surface catches light along the rim, base, and any raised edges.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3400.0,
			115.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Display_Case"
			],
			null,
			null,
			null,
			null,
			"Display case for an icon panel, relic container, or sealed devotional object."
		);

		CreateItem(
			"medieval_eastern_christian_tetrapod_table",
			"table",
			"a tetrapod offering table",
			null,
			"This tetrapod offering table is a large, well-made table built from walnut boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			110.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Four-legged service table for icons, books, candles, and offerings."
		);

		CreateItem(
			"medieval_eastern_christian_vigil_lamp",
			"lamp",
			"a bronze vigil lamp",
			null,
			"This bronze vigil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1050.0,
			54.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Small liquid-fuel vigil lamp for an icon stand, sanctuary corner, or shrine shelf."
		);

		CreateItem(
			"medieval_eastern_christian_wall_icon_panel",
			"panel",
			"a wall icon panel",
			null,
			"This wall icon panel is a medium-sized, workmanlike panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2600.0,
			34.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Wall devotional panel for sacred imagery; not a personal charm or jewellery piece."
		);

		CreateItem(
			"medieval_islamic_ablution_basin",
			"basin",
			"a ceramic ablution basin",
			null,
			"This ceramic ablution basin is a medium-sized, workmanlike basin formed from ceramic. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			6200.0,
			30.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Water-holding basin for ritual washing before prayer."
		);

		CreateItem(
			"medieval_islamic_ablution_ewer",
			"ewer",
			"a brass ablution ewer",
			null,
			"This brass ablution ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1500.0,
			32.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for ablution water."
		);

		CreateItem(
			"medieval_islamic_brass_hanging_lamp",
			"lamp",
			"a brass mosque lamp",
			null,
			"This brass mosque lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			60.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Metal liquid-fuel lamp for a mosque, madrasa, or prayer hall."
		);

		CreateItem(
			"medieval_islamic_folding_book_stand",
			"stand",
			"a folding book stand",
			null,
			"This folding book stand is a small, well-made stand built from boxwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			36.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Wares",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Low sloped stand for scripture reading or study; furnishing rather than personal jewellery."
		);

		CreateItem(
			"medieval_islamic_incense_burner",
			"burner",
			"a brass incense burner",
			null,
			"This brass incense burner is a small, well-made burner worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Small burner for fragrant wood or incense in a religious or ceremonial hall."
		);

		CreateItem(
			"medieval_islamic_madrasa_text_shelves",
			"shelves",
			"madrasa text shelves",
			null,
			"These madrasa text shelves are large, well-made shelves built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			17000.0,
			78.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Bookcase_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open shelves for religious study texts, legal works, and manuscript bundles."
		);

		CreateItem(
			"medieval_islamic_mihrab_panel",
			"panel",
			"a carved mihrab panel",
			null,
			"This carved mihrab panel is a large, well-made panel built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			14000.0,
			115.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Decorative niche-like direction panel for a mosque or prayer hall."
		);

		CreateItem(
			"medieval_islamic_mosque_glass_lamp",
			"lamp",
			"a glass mosque lamp",
			null,
			"This glass mosque lamp is a small, well-made lamp made from glass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			75.0m,
			true,
			false,
			"glass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Hanging liquid-fuel mosque lamp with fragile glass construction."
		);

		CreateItem(
			"medieval_islamic_qibla_direction_panel",
			"panel",
			"a qibla direction panel",
			null,
			"This qibla direction panel is a medium-sized, workmanlike panel built from cedar boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			4200.0,
			38.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Wall panel marking prayer direction without using figurative imagery."
		);

		CreateItem(
			"medieval_islamic_quran_cabinet",
			"cabinet",
			"a Qur'an cabinet",
			null,
			"This Qur'an cabinet is a large, well-made cabinet built from walnut boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			95.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cabinet for bound or wrapped sacred texts, commentaries, and recitation books."
		);

		CreateItem(
			"medieval_islamic_reed_mat_rack",
			"rack",
			"a reed mat rack",
			null,
			"This reed mat rack is a large, workmanlike rack built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			7000.0,
			28.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Narrow_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open rack for communal prayer mats without making the mats themselves personal gear."
		);

		CreateItem(
			"medieval_islamic_sandal_shelf",
			"shelf",
			"a mosque sandal shelf",
			null,
			"This mosque sandal shelf is a large, workmanlike shelf built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9600.0,
			30.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Simple Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wide_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open shelf for footwear at a prayer hall entrance."
		);

		CreateItem(
			"medieval_islamic_water_storage_jar",
			"jar",
			"an ablution water jar",
			null,
			"This ablution water jar is a large, workmanlike jar formed from terracotta. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			12000.0,
			24.0m,
			true,
			false,
			"terracotta",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Amphora_Urna"
			],
			null,
			null,
			null,
			null,
			"Large ceramic liquid vessel for stored washing water."
		);

		CreateItem(
			"medieval_islamic_wooden_minbar_steps",
			"minbar",
			"a carved wooden minbar",
			null,
			"This carved wooden minbar is a large, well-made minbar built from walnut boards. A raised speaking enclosure stands above a small base, with a front panel and a narrow ledge for a book. The upper rail is polished by hands. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			180.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Stepped mosque pulpit furnishing; no seat, container, or door mechanics are asserted."
		);

		CreateItem(
			"medieval_locking_islamic_charity_lockbox",
			"box",
			"a charity lockbox",
			null,
			"This charity lockbox is a small, well-made box built from cedar boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3600.0,
			120.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Lockable charity box for mosque or charitable-house settings."
		);

		CreateItem(
			"medieval_locking_islamic_quran_chest",
			"chest",
			"a locked Qur'an chest",
			null,
			"This locked Qur'an chest is a large, well-made chest built from cedar boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			260.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for wrapped books and religious texts."
		);

		CreateItem(
			"medieval_jewish_bimah_table",
			"table",
			"a bimah reading table",
			null,
			"This bimah reading table is a large, well-made table built from oak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			100.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Central reading table surface for scrolls, books, and service objects."
		);

		CreateItem(
			"medieval_jewish_bronze_lampstand",
			"lampstand",
			"a bronze seven-branched lampstand",
			null,
			"This bronze seven-branched lampstand is a medium-sized, well-made lampstand worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			5200.0,
			110.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal"
			],
			null,
			null,
			null,
			null,
			"Recognisable ritual lampstand furnishing; actual light should come from separate candle or lamp items."
		);

		CreateItem(
			"medieval_jewish_genizah_chest",
			"chest",
			"a genizah chest",
			null,
			"This genizah chest is a large, workmanlike chest built from cedar boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18500.0,
			65.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Funerary Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Trunk"
			],
			null,
			null,
			null,
			null,
			"Chest for worn religious papers, damaged texts, or ritually retired writing materials."
		);

		CreateItem(
			"medieval_jewish_memorial_candle",
			"candle",
			"a memorial beeswax candle",
			null,
			"This memorial beeswax candle is a small, workmanlike candle made from pale beeswax. A visible wick runs through the centre, and the body is smooth from moulding and hand dipping. The base is flattened so it can stand in a holder. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			180.0,
			4.0m,
			true,
			false,
			"beeswax",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Candles"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"Candle_Long"
			],
			null,
			null,
			null,
			null,
			"Long-burning candle for memorial or household religious observance."
		);

		CreateItem(
			"medieval_jewish_ner_tamid_lamp",
			"lamp",
			"a ner tamid lamp",
			null,
			"This ner tamid lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			70.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Hanging perpetual-style liquid-fuel lamp for a synagogue furnishing set."
		);

		CreateItem(
			"medieval_jewish_reading_lectern",
			"lectern",
			"a synagogue reading lectern",
			null,
			"This synagogue reading lectern is a medium-sized, well-made lectern built from walnut boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7200.0,
			68.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Sloped reading surface for scrolls or books."
		);

		CreateItem(
			"medieval_jewish_scroll_cabinet",
			"cabinet",
			"a Torah scroll cabinet",
			null,
			"This Torah scroll cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			120.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cabinet for scrolls, wrappings, and synagogue texts."
		);

		CreateItem(
			"medieval_jewish_scroll_rest",
			"rest",
			"a wooden scroll rest",
			null,
			"This wooden scroll rest is a small, well-made rest built from boxwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			30.0m,
			true,
			false,
			"boxwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Wares",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Small support for resting a scroll or rolled text during reading."
		);

		CreateItem(
			"medieval_jewish_synagogue_bench",
			"bench",
			"a synagogue bench",
			null,
			"This synagogue bench is a large, workmanlike bench built from oak boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			22000.0,
			58.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Triple",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Long seating bench for a synagogue or teaching house."
		);

		CreateItem(
			"medieval_jewish_wall_tablet_panel",
			"panel",
			"a wall tablet panel",
			null,
			"This wall tablet panel is a medium-sized, workmanlike panel formed from stoneware. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3400.0,
			32.0m,
			true,
			false,
			"stoneware",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Glassware"
			],
			null,
			null,
			null,
			null,
			"Wall panel for inscribed or symbolic religious display, with exact writing handled by skins or writing blocks."
		);

		CreateItem(
			"medieval_jewish_washing_basin",
			"basin",
			"a ceramic washing basin",
			null,
			"This ceramic washing basin is a medium-sized, workmanlike basin formed from ceramic. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			5600.0,
			28.0m,
			true,
			false,
			"ceramic",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding basin for washing water."
		);

		CreateItem(
			"medieval_jewish_washing_ewer",
			"ewer",
			"a brass washing ewer",
			null,
			"This brass washing ewer is a small, workmanlike ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1400.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for handwashing or ritual washing."
		);

		CreateItem(
			"medieval_locking_jewish_torah_ark_chest",
			"ark",
			"a wooden Torah ark",
			null,
			"This wooden Torah ark is a large, well-made ark built from cedar boards. Small doors close the front of the case, with a raised threshold and careful trim around the opening. The interior is protected and formal. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			46000.0,
			520.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_SafeChest"
			],
			null,
			null,
			null,
			null,
			"Large built-in-lock container profile for protected scroll storage."
		);

		CreateItem(
			"medieval_locking_jewish_tzedakah_box",
			"box",
			"a tzedakah box",
			null,
			"This tzedakah box is a small, well-made box built from walnut boards. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			3600.0,
			120.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small locking box for charity coin and offerings."
		);

		CreateItem(
			"medieval_hindu_abhisheka_basin",
			"basin",
			"a stone abhisheka basin",
			null,
			"This stone abhisheka basin is a medium-sized, well-made basin cut from soapstone. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Good,
			8200.0,
			70.0m,
			true,
			false,
			"soapstone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding basin for ceremonial pouring and washing."
		);

		CreateItem(
			"medieval_hindu_brass_offering_tray",
			"tray",
			"a brass offering tray",
			null,
			"This brass offering tray is a medium-sized, well-made tray worked from brass. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Normal,
			ItemQuality.Good,
			1600.0,
			45.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray for flowers, lamps, powders, food offerings, or ritual vessels."
		);

		CreateItem(
			"medieval_hindu_bronze_oil_lamp",
			"lamp",
			"a bronze temple oil lamp",
			null,
			"This bronze temple oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1000.0,
			52.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Liquid-fuel ritual lamp suitable for temple or household shrine use."
		);

		CreateItem(
			"medieval_hindu_carved_shrine_screen",
			"screen",
			"a carved shrine screen",
			null,
			"This carved shrine screen is a large, well-made screen built from teak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			16000.0,
			90.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Decorative screen panel for a temple niche, household shrine, or ritual alcove."
		);

		CreateItem(
			"medieval_hindu_floor_offering_stand",
			"stand",
			"a low floor offering stand",
			null,
			"This low floor offering stand is a medium-sized, well-made stand built from sandalwood. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3800.0,
			58.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Low floor stand for lamps, bowls, flower offerings, or small ritual goods."
		);

		CreateItem(
			"medieval_hindu_garland_rack",
			"rack",
			"a flower-garland rack",
			null,
			"This flower-garland rack is a medium-sized, workmanlike rack built from split bamboo. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			2200.0,
			18.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Simple Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Rack or shelf for garlands, flower strings, and offering preparation."
		);

		CreateItem(
			"medieval_hindu_household_shrine_cupboard",
			"cupboard",
			"a household shrine cupboard",
			null,
			"This household shrine cupboard is a medium-sized, well-made cupboard built from teak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			9500.0,
			95.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Cabinet"
			],
			null,
			null,
			null,
			null,
			"Small shrine cupboard for lamps, vessels, images, flowers, powders, or wrapped offerings."
		);

		CreateItem(
			"medieval_hindu_image_pedestal",
			"pedestal",
			"a carved image pedestal",
			null,
			"This carved image pedestal is a large, well-made pedestal built from teak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			18000.0,
			95.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Pedestal for a large sacred image or statue; the image itself is intentionally not specified."
		);

		CreateItem(
			"medieval_hindu_incense_burner",
			"burner",
			"a brass incense burner",
			null,
			"This brass incense burner is a small, well-made burner worked from brass. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			48.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Solid-fuel burner for incense and fragrant woods."
		);

		CreateItem(
			"medieval_hindu_kalasha_pot",
			"pot",
			"a brass kalasha pot",
			null,
			"This brass kalasha pot is a small, well-made pot worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1100.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Jug"
			],
			null,
			null,
			null,
			null,
			"Ritual liquid vessel suitable for water, scented water, or festival use."
		);

		CreateItem(
			"medieval_hindu_multi_wick_lamp",
			"lamp",
			"a multi-wick brass lamp",
			null,
			"This multi-wick brass lamp is a medium-sized, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			85.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Larger liquid-fuel lamp form for festival or altar lighting."
		);

		CreateItem(
			"medieval_hindu_puja_shelf",
			"shelf",
			"a carved puja shelf",
			null,
			"This carved puja shelf is a medium-sized, well-made shelf built from sandalwood. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2600.0,
			70.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Wall shelf for lamps, offerings, images, flowers, or household shrine goods."
		);

		CreateItem(
			"medieval_hindu_puja_water_ewer",
			"ewer",
			"a puja water ewer",
			null,
			"This puja water ewer is a small, well-made ewer worked from brass. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1350.0,
			38.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for ritual water or scented liquid."
		);

		CreateItem(
			"medieval_hindu_stone_offering_bowl",
			"bowl",
			"a stone offering bowl",
			null,
			"This stone offering bowl is a small, workmanlike bowl cut from soapstone. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			24.0m,
			true,
			false,
			"soapstone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for rice, flowers, ash, pigment, or other solid offerings."
		);

		CreateItem(
			"medieval_hindu_temple_altar_table",
			"altar",
			"a carved temple altar",
			null,
			"This carved temple altar is a large, well-made altar built from teak boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			30000.0,
			140.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Temple service table surface for lamps, offerings, vessels, and display objects."
		);

		CreateItem(
			"medieval_hindu_temple_supply_cupboard",
			"cupboard",
			"a temple supply cupboard",
			null,
			"This temple supply cupboard is a large, well-made cupboard built from teak boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			25000.0,
			115.0m,
			true,
			false,
			"teak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Large storage cupboard for lamps, trays, cloths, oils, incense, and vessels."
		);

		CreateItem(
			"medieval_locking_hindu_sandalwood_store_box",
			"box",
			"a sandalwood temple-store box",
			null,
			"This sandalwood temple-store box is a small, well-made box built from sandalwood. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			2400.0,
			170.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Misc",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small lockable box for fragrant powders, offerings, or temple stores."
		);

		CreateItem(
			"medieval_locking_hindu_temple_donation_chest",
			"chest",
			"a locked temple donation chest",
			null,
			"This locked temple donation chest is a large, well-made chest built from teak boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			36000.0,
			340.0m,
			true,
			false,
			"teak",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Large locking donation chest for temple or shrine precincts."
		);

		CreateItem(
			"medieval_buddhist_altar_table",
			"altar",
			"a lacquered altar table",
			null,
			"This lacquered altar table is a large, well-made altar built from walnut boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			140.0m,
			true,
			false,
			"walnut",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Altar table for lamps, incense, offerings, scrolls, and display objects."
		);

		CreateItem(
			"medieval_buddhist_incense_box",
			"box",
			"a sandalwood incense box",
			null,
			"This sandalwood incense box is a small, well-made box built from sandalwood. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			900.0,
			45.0m,
			true,
			false,
			"sandalwood",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Dry container for incense sticks, powdered incense, or fragrant wood chips."
		);

		CreateItem(
			"medieval_buddhist_incense_burner",
			"burner",
			"a bronze incense burner",
			null,
			"This bronze incense burner is a small, well-made burner worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1300.0,
			46.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Small burner for incense, charcoal, or fragrant wood."
		);

		CreateItem(
			"medieval_buddhist_lotus_oil_lamp",
			"lamp",
			"a lotus-shaped oil lamp",
			null,
			"This lotus-shaped oil lamp is a small, well-made lamp worked from bronze. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			1150.0,
			58.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Liquid-fuel lamp for Buddhist altar or shrine lighting."
		);

		CreateItem(
			"medieval_buddhist_meditation_bench",
			"bench",
			"a meditation bench",
			null,
			"This meditation bench is a medium-sized, workmanlike bench built from cypress boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			24.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Chair_Single"
			],
			null,
			null,
			null,
			null,
			"Low bench for seated or kneeling practice; a furnishing rather than an outfit or personal charm."
		);

		CreateItem(
			"medieval_buddhist_memorial_lamp",
			"lamp",
			"a memorial oil lamp",
			null,
			"This memorial oil lamp is a small, workmanlike lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Standard,
			850.0,
			34.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Small oil lamp for memorial, votive, or altar use."
		);

		CreateItem(
			"medieval_buddhist_memorial_tablet_shelf",
			"shelf",
			"a memorial tablet shelf",
			null,
			"This memorial tablet shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			55.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open shelf for memorial tablets, small plaques, offerings, or lamps."
		);

		CreateItem(
			"medieval_buddhist_offering_bowl",
			"bowl",
			"a porcelain offering bowl",
			null,
			"This porcelain offering bowl is a small, well-made bowl formed from porcelain. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. The surface is slightly uneven at the rim and base, with kiln marks visible in the finish.",
			SizeCategory.Small,
			ItemQuality.Good,
			620.0,
			32.0m,
			true,
			false,
			"porcelain",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Glassware",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for rice, fruit, flowers, water cups represented as solid offerings, or other altar goods."
		);

		CreateItem(
			"medieval_buddhist_offering_tray",
			"tray",
			"a lacquered offering tray",
			null,
			"This lacquered offering tray is a medium-sized, well-made tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			950.0,
			38.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Flat tray for arranging flowers, incense, offerings, lamps, and ritual supplies."
		);

		CreateItem(
			"medieval_buddhist_sutra_cabinet",
			"cabinet",
			"a sutra cabinet",
			null,
			"This sutra cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			22000.0,
			110.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cabinet for sutra scrolls, booklets, cases, and associated service materials."
		);

		CreateItem(
			"medieval_buddhist_sutra_lectern",
			"lectern",
			"a sutra reading lectern",
			null,
			"This sutra reading lectern is a medium-sized, workmanlike lectern built from split bamboo. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			3600.0,
			30.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Desk_Surface"
			],
			null,
			null,
			null,
			null,
			"Reading surface for sutras, commentaries, or ritual recitations."
		);

		CreateItem(
			"medieval_locking_buddhist_reliquary_box",
			"box",
			"a stupa reliquary box",
			null,
			"This stupa reliquary box is a small, well-made box worked from bronze. A fitted lid closes over a shallow compartment, and a small lock plate is set squarely into the front. The seams are tight, with finger-worn corners around the lid. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			4200.0,
			220.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LockingContainer_Lockbox"
			],
			null,
			null,
			null,
			null,
			"Small bronze lockbox for reliquary or shrine storage."
		);

		CreateItem(
			"medieval_locking_buddhist_sutra_chest",
			"chest",
			"a locked sutra chest",
			null,
			"This locked sutra chest is a large, well-made chest built from cypress boards. A hinged lid spans the top, and visible lock furniture anchors the front. The sides are reinforced at the corners, with enough depth for bundled goods. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			26000.0,
			280.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LockingContainer_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Locking chest for wrapped sutras, papers, or monastery texts."
		);

		CreateItem(
			"medieval_daoist_altar_lamp",
			"lamp",
			"a Daoist altar lamp",
			null,
			"This Daoist altar lamp is a small, well-made lamp worked from brass. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Small,
			ItemQuality.Good,
			950.0,
			44.0m,
			true,
			false,
			"brass",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Liquid-fuel lamp for altar or shrine lighting."
		);

		CreateItem(
			"medieval_daoist_altar_table",
			"altar",
			"a Daoist altar table",
			null,
			"This Daoist altar table is a large, well-made altar built from cypress boards. A broad formal top rests on a stable base, with the front face finished more carefully than the back. The edges are kept plain and deliberate. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			24000.0,
			115.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Table_Four",
				"Container_Table"
			],
			null,
			null,
			null,
			null,
			"Altar table for lamps, incense, tablets, offerings, and ritual documents."
		);

		CreateItem(
			"medieval_daoist_bronze_incense_urn",
			"urn",
			"a bronze incense urn",
			null,
			"This bronze incense urn is a medium-sized, well-made urn worked from bronze. A pierced bowl hangs from short chains, with darkened marks around the heat chamber. The lid is vented for smoke. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			4800.0,
			76.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Heating",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"SolidFuelHeaterCooler_Brazier"
			],
			null,
			null,
			null,
			null,
			"Freestanding incense urn represented as a solid-fuel burner."
		);

		CreateItem(
			"medieval_daoist_offering_tray",
			"tray",
			"a lacquered offering tray",
			null,
			"This lacquered offering tray is a medium-sized, workmanlike tray built from split bamboo. A shallow rim runs around the flat carrying surface, keeping objects from sliding away. The underside is plain and easy to grip. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			900.0,
			26.0m,
			true,
			false,
			"bamboo",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Tray for fruit, paper, incense, lamps, or dry ritual offerings."
		);

		CreateItem(
			"medieval_daoist_painted_altar_screen",
			"screen",
			"a painted altar screen",
			null,
			"This painted altar screen is a large, well-made screen built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			12000.0,
			70.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Painted screen panel for a shrine or altar backdrop."
		);

		CreateItem(
			"medieval_daoist_scripture_cabinet",
			"cabinet",
			"a Daoist scripture cabinet",
			null,
			"This Daoist scripture cabinet is a large, well-made cabinet built from cedar boards. Paneled doors close over shelves inside the body, with small pulls set into the front. The base is broad enough to keep the cabinet steady. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			19000.0,
			85.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Cupboard"
			],
			null,
			null,
			null,
			null,
			"Cabinet for scriptures, registers, talisman papers, and ritual manuscripts."
		);

		CreateItem(
			"medieval_daoist_tablet_shelf",
			"shelf",
			"a spirit-tablet shelf",
			null,
			"This spirit-tablet shelf is a medium-sized, well-made shelf built from cedar boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			48.0m,
			true,
			false,
			"cedar",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open shelf for tablets, plaques, lamps, and offerings."
		);

		CreateItem(
			"medieval_daoist_talisman_paper_chest",
			"chest",
			"a talisman-paper chest",
			null,
			"This talisman-paper chest is a medium-sized, well-made chest built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			7600.0,
			60.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture",
				"Market / Writing Materials / Document Containers"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Footlocker"
			],
			null,
			null,
			null,
			null,
			"Dry chest for paper talismans, ritual writing stock, scrolls, or folded documents."
		);

		CreateItem(
			"medieval_daoist_water_ewer",
			"ewer",
			"a ritual water ewer",
			null,
			"This ritual water ewer is a small, well-made ewer worked from bronze. A rounded belly narrows into a pouring mouth, with a handle set opposite the lip. The foot is broad enough to keep the vessel steady when full. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Good,
			1200.0,
			40.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"LContainer_Decanter"
			],
			null,
			null,
			null,
			null,
			"Pouring vessel for ritual water or cleansing liquid."
		);

		CreateItem(
			"medieval_shinto_kamidana_shelf",
			"shelf",
			"a kamidana shrine shelf",
			null,
			"This kamidana shrine shelf is a medium-sized, well-made shelf built from cypress boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			2200.0,
			50.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Luxury Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Wall_Shelf"
			],
			null,
			null,
			null,
			null,
			"Raised shrine shelf for offerings, small vessels, lamps, or plaques."
		);

		CreateItem(
			"medieval_shinto_offering_stand",
			"stand",
			"a whitewood offering stand",
			null,
			"This whitewood offering stand is a medium-sized, well-made stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3600.0,
			45.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Small_Table"
			],
			null,
			null,
			null,
			null,
			"Low offering stand for rice, salt, greenery, lamps, or vessels."
		);

		CreateItem(
			"medieval_shinto_purification_basin",
			"basin",
			"a stone purification basin",
			null,
			"This stone purification basin is a large, well-made basin cut from granite. The hollow is broad and open, with a thick rim and a stable foot. The inner surface slopes gently toward the centre. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Large,
			ItemQuality.Good,
			65000.0,
			120.0m,
			true,
			false,
			"granite",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Drum"
			],
			null,
			null,
			null,
			null,
			"Large liquid-holding basin for purification water."
		);

		CreateItem(
			"medieval_shinto_sake_vessel_stand",
			"stand",
			"a sake-vessel stand",
			null,
			"This sake-vessel stand is a small, workmanlike stand built from cypress boards. A sloped top is fixed to a steady support, with a narrow lip along the lower edge. The reading face is polished where books and tablets have rested. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Small,
			ItemQuality.Standard,
			1200.0,
			20.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Tray"
			],
			null,
			null,
			null,
			null,
			"Small stand or tray for liquid vessels and shrine offerings; vessels remain separate items."
		);

		CreateItem(
			"medieval_shinto_shrine_donation_box",
			"box",
			"a slatted shrine donation box",
			null,
			"This slatted shrine donation box is a large, workmanlike box built from cypress boards. A hinged lid sits over a boxed interior, with plain front boards and reinforced corners. The opening line is visible along the upper edge. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			18000.0,
			70.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Open_Bin"
			],
			null,
			null,
			null,
			null,
			"Open-lidded or slatted donation box for offerings and coins without built-in lock mechanics."
		);

		CreateItem(
			"medieval_shinto_shrine_screen_panel",
			"screen",
			"a pale shrine screen panel",
			null,
			"This pale shrine screen panel is a large, well-made screen built from cypress boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Good,
			11000.0,
			65.0m,
			true,
			false,
			"cypress",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Plain pale timber screen panel for marking a shrine area or ritual backdrop."
		);

		CreateItem(
			"medieval_shinto_stone_shrine_lamp",
			"lamp",
			"a stone shrine lamp",
			null,
			"This stone shrine lamp is a medium-sized, well-made lamp cut from granite. A small fuel chamber sits below the light opening, with a carrying loop fixed above. The sides shield the flame while still letting light spill through. Soot, scorch marks, and darkened handling points show where flame and heat have touched it.",
			SizeCategory.Normal,
			ItemQuality.Good,
			12000.0,
			80.0m,
			true,
			false,
			"granite",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Lighting",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Lighting / Lamps"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Lantern"
			],
			null,
			null,
			null,
			null,
			"Stone-bodied liquid-fuel lamp for shrine approaches or altar areas."
		);

		CreateItem(
			"medieval_shinto_votive_tablet_rack",
			"rack",
			"a votive tablet rack",
			null,
			"This votive tablet rack is a large, workmanlike rack built from pine boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			9000.0,
			36.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Open display rack for hung or rested votive tablets."
		);

		CreateItem(
			"medieval_northern_ancestor_tablet_shelf",
			"shelf",
			"an ancestor tablet shelf",
			null,
			"This ancestor tablet shelf is a medium-sized, well-made shelf built from yew boards. Open shelves are set between upright supports, leaving the stored objects visible. The front edges are rubbed smooth from repeated use. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Normal,
			ItemQuality.Good,
			3200.0,
			45.0m,
			true,
			false,
			"yew",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Furniture",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Furniture"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Container_Display_Shelves"
			],
			null,
			null,
			null,
			null,
			"Shelf for ancestor plaques, small vessels, and memorial offerings."
		);

		CreateItem(
			"medieval_northern_blot_offering_bowl",
			"bowl",
			"a bronze offering bowl",
			null,
			"This bronze offering bowl is a small, workmanlike bowl worked from bronze. The hollow centre is rounded and shallow, with a steady foot beneath it. The rim is smooth where fingers and spoons pass. Hammer marks, rubbed edges, and a dull working sheen remain visible across the metal.",
			SizeCategory.Small,
			ItemQuality.Standard,
			950.0,
			30.0m,
			true,
			false,
			"bronze",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_HeavyMetal",
				"Container_Plate"
			],
			null,
			null,
			null,
			null,
			"Open bowl for grain, salt, bloodless offerings, ash, or symbolic ritual goods."
		);

		CreateItem(
			"medieval_northern_libation_bowl",
			"bowl",
			"a stone libation bowl",
			null,
			"This stone libation bowl is a medium-sized, workmanlike bowl cut from sandstone. A rounded body rises to a narrow neck, with two small handles set high on the shoulders. The mouth is shaped for a fitted stopper. The stone has chipped edges, smoothed contact points, and a cool matte surface.",
			SizeCategory.Normal,
			ItemQuality.Standard,
			7200.0,
			28.0m,
			true,
			false,
			"sandstone",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Container / Watertight Container",
				"Functions / Household Items / Household Wares",
				"Market / Religious Goods / Ritual Supplies",
				"Market / Religious Goods / Temple Offerings",
				"Market / Household Goods / Standard Wares"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"LContainer_Amphora_Sextarius"
			],
			null,
			null,
			null,
			null,
			"Liquid-holding bowl for libations, washing, or poured offerings."
		);

		CreateItem(
			"medieval_northern_sacred_post_panel",
			"post",
			"a carved sacred post",
			null,
			"This carved sacred post is a large, workmanlike post built from oak boards. The front is more carefully finished than the back, giving it a formal display face. The base and edges are steady, plain, and easy to set in a public room. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			15000.0,
			42.0m,
			true,
			false,
			"oak",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Decorations",
				"Market / Religious Goods / Devotional Goods",
				"Market / Household Goods / Standard Decorations"
			],
			[
				"Holdable",
				"Destroyable_Furniture"
			],
			null,
			null,
			null,
			null,
			"Large carved cult or ancestor post for a hall, grove edge, or household sacred corner."
		);

		CreateItem(
			"medieval_northern_shrine_bench",
			"bench",
			"a shrine-hall bench",
			null,
			"This shrine-hall bench is a large, workmanlike bench built from pine boards. A long plank seat rests on simple supports, with enough length for several people. The front edge is worn smooth where legs have passed over it. The grain is visible beneath rubbed edges, small tool marks, and a practical plain finish.",
			SizeCategory.Large,
			ItemQuality.Standard,
			19000.0,
			48.0m,
			true,
			false,
			"pine",
			[
				"Functions / Household Items / Household Religious Items",
				"Functions / Household Items / Household Furniture",
				"Functions / Container",
				"Functions / Container / Open Container",
				"Market / Household Goods / Standard Furniture",
				"Market / Religious Goods / Devotional Goods"
			],
			[
				"Holdable",
				"Destroyable_Furniture",
				"Bench_Double",
				"Container_Bench_Surface"
			],
			null,
			null,
			null,
			null,
			"Simple two-place sacred-hall bench with ordinary bench behaviour."
		);
	}
}

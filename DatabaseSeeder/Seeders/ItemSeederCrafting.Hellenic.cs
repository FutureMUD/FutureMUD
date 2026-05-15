using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private bool ShouldSeedAntiquityCrafts()
	{
		return _questionAnswers?.TryGetValue("eras", out var eras) == true &&
		       eras.Contains("antiquity", StringComparison.InvariantCultureIgnoreCase);
	}

	private bool TryLookupReworkItem(string stableReference, out GameItemProto item)
	{
		if (_items.TryGetValue(stableReference, out item!))
		{
			return true;
		}

		if (!HellenicAntiquityClothingStableReferences.TryGetValue(stableReference, out var shortDescription) &&
		    !EgyptianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !RomanAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !CelticAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !GermanicAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !KushiteAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !PunicAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !PersianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !EtruscanAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !AnatolianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription) &&
		    !ScythianSarmatianAntiquityClothingStableReferences.TryGetValue(stableReference, out shortDescription))
		{
			item = null!;
			return false;
		}

		if (_items.TryGetValue(shortDescription, out item!))
		{
			_items[stableReference] = item;
			return true;
		}

		item = _context!.GameItemProtos.Local
			.FirstOrDefault(x => x.ShortDescription.Equals(shortDescription, StringComparison.OrdinalIgnoreCase)) ??
		       _context.GameItemProtos
			       .FirstOrDefault(x => x.ShortDescription.Equals(shortDescription, StringComparison.OrdinalIgnoreCase))!;
		if (item is null)
		{
			return false;
		}

		_items[stableReference] = item;
		return true;
	}

	private GameItemProto LookupReworkItem(string stableReference)
	{
		return TryLookupReworkItem(stableReference, out var item)
			? item
			: throw new ApplicationException($"Unknown rework item stable reference {stableReference}");
	}

	private string StableVariableProduct(string stableReference, bool fineColour)
	{
		var item = LookupReworkItem(stableReference);
		return fineColour
			? $"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i1, Fine Colour=$i1"
			: $"SimpleVariableProduct - 1x {item.ShortDescription} (#{item.Id}); variable Colour=$i1";
	}

	private string StableSimpleProduct(string stableReference)
	{
		var item = LookupReworkItem(stableReference);
		return $"SimpleProduct - 1x {item.ShortDescription} (#{item.Id})";
	}

	private void SeedAntiquityHellenicClothingCrafts()
	{
		if (!ShouldSeedAntiquityCrafts())
		{
			return;
		}

		const string ancientKnowledge = "Ancient Textile Production";
		const string hellenicKnowledge = "Hellenic Textilecraft";
		const string ancientKnowledgeDescription =
			"Shared ancient techniques for preparing textile fibres, spinning yarn, weaving cloth, dyeing, and fulling.";
		const string hellenicKnowledgeDescription =
			"Hellenic garment knowledge for assembling rectangular wool and linen dress such as chitons, peploi, chlamydes, himatia, and veils.";

		void AddAncientTextileCraft(string name, string category, string blurb, string action, string itemDescription,
			string traitName, int minimumTraitValue, Difficulty difficulty,
			IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
			IEnumerable<string> tools, IEnumerable<string> products, IEnumerable<string>? failProducts = null)
		{
			AddCraft(
				name,
				category,
				blurb,
				action,
				itemDescription,
				ancientKnowledge,
				traitName,
				minimumTraitValue,
				difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				phases,
				inputs,
				tools,
				products,
				failProducts ?? [],
				knowledgeSubtype: "Textiles",
				knowledgeDescription: ancientKnowledgeDescription,
				knowledgeLongDescription: ancientKnowledgeDescription);
		}

		void AddHellenicGarmentCraft(string name, string blurb, string action, string itemDescription,
			int minimumTraitValue, Difficulty difficulty,
			IEnumerable<(int Seconds, string Echo, string FailEcho)> phases, IEnumerable<string> inputs,
			IEnumerable<string> tools, IEnumerable<string> products)
		{
			AddCraft(
				name,
				"Tailoring",
				blurb,
				action,
				itemDescription,
				hellenicKnowledge,
				"Tailoring",
				minimumTraitValue,
				difficulty,
				Outcome.MinorFail,
				5,
				3,
				false,
				phases,
				inputs,
				tools,
				products,
				[],
				knowledgeSubtype: "Hellenic",
				knowledgeDescription: hellenicKnowledgeDescription,
				knowledgeLongDescription: hellenicKnowledgeDescription);
		}

		AddAncientTextileCraft(
			"sort flax stalks for retting",
			"Farming",
			"sort flax stalks into rettable fibre bundles",
			"sorting flax stalks",
			"flax stalks being sorted for retting",
			"Farming",
			1,
			Difficulty.Easy,
			[
				(25, "$0 sort|sorts through $i1, shaking away dirt and seed heads.", "$0 sort|sorts through $i1, but leave|leaves the bundles uneven and contaminated."),
				(25, "$0 bind|binds the usable stalks into loose, rettable bundles.", "$0 bind|binds the stalks too tightly, spoiling much of the fibre."),
				(20, "$0 set|sets aside $p1 for soaking.", "$0 salvage|salvages only $f1 from the poorly prepared flax.")
			],
			["Commodity - 1 kilogram 800 grams of flax"],
			[],
			["CommodityProduct - 1 kilogram 600 grams of flax commodity; tag Raw Textile Fibre"],
			["CommodityProduct - 800 grams of flax commodity; tag Raw Textile Fibre"]);

		AddAncientTextileCraft(
			"ret break and hackle flax fibre",
			"Weaving",
			"ret, break, and hackle raw flax into prepared linen fibre",
			"retting and hackling flax",
			"flax being retted, broken, and hackled",
			"Weaving",
			1,
			Difficulty.Normal,
			[
				(35, "$0 soak|soaks $i1 in $t1, turning the bundles through clean water.", "$0 soak|soaks $i1 unevenly in $t1."),
				(35, "$0 work|works the retted stalks through $t2 to crack away the woody core.", "$0 overwork|overworks the stalks through $t2 and shorten|shortens the fibre."),
				(35, "$0 draw|draws the fibre repeatedly through $t3, combing it into clean stricks.", "$0 draw|draws the fibre poorly through $t3, leaving tangles and shive behind."),
				(20, "$0 gather|gathers $p1 into neat prepared bundles.", "$0 gather|gathers only $f1 from the damaged flax.")
			],
			[
				"Commodity - 1 kilogram 600 grams of flax; piletag Raw Textile Fibre",
				"LiquidUse - 10 litres of Water"
			],
			[
				"TagTool - InRoom - an item with the Retting Trough tag",
				"TagTool - Held - an item with the Flax Break tag",
				"TagTool - Held - an item with the Hackle tag"
			],
			[
				"CommodityProduct - 900 grams of linen commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
			],
			[
				"CommodityProduct - 350 grams of linen commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
			]);

		AddAncientTextileCraft(
			"sort raw wool fleece",
			"Farming",
			"sort raw fleece into spinnable wool bundles",
			"sorting raw wool fleece",
			"raw wool fleece being sorted",
			"Farming",
			1,
			Difficulty.Easy,
			[
				(25, "$0 pick|picks burrs and coarse matter from $i1.", "$0 miss|misses many burrs and coarse patches in $i1."),
				(25, "$0 divide|divides the better fleece from greasy waste and matted locks.", "$0 mix|mixes good fleece with matted waste."),
				(20, "$0 roll|rolls $p1 into loose raw-wool bundles.", "$0 salvage|salvages only $f1 from the badly sorted fleece.")
			],
			["Commodity - 1 kilogram 400 grams of wool"],
			["TagTool - Held - an item with the Shears tag"],
			["CommodityProduct - 1 kilogram 200 grams of wool commodity; tag Raw Textile Fibre"],
			["CommodityProduct - 500 grams of wool commodity; tag Raw Textile Fibre"]);

		AddAncientTextileCraft(
			"scour and comb wool fleece",
			"Weaving",
			"scour and comb raw wool into prepared spinning fibre",
			"scouring and combing wool",
			"wool fleece being scoured and combed",
			"Weaving",
			1,
			Difficulty.Normal,
			[
				(30, "$0 wash|washes $i1 carefully, pressing grease and dirt out with clean water.", "$0 wash|washes $i1 too roughly, felting some of the locks."),
				(35, "$0 tease|teases the damp wool open by hand and begin|begins to comb it on $t1.", "$0 leave|leaves many locks tangled while combing on $t1."),
				(35, "$0 draw|draws the wool into aligned, airy bundles ready for spinning.", "$0 draw|draws only short and uneven bundles from the fleece."),
				(20, "$0 set|sets aside $p1.", "$0 set|sets aside only $f1.")
			],
			[
				"Commodity - 1 kilogram 200 grams of wool; piletag Raw Textile Fibre",
				"LiquidUse - 6 litres of Water"
			],
			["TagTool - Held - an item with the Fibre Comb tag"],
			[
				"CommodityProduct - 900 grams of wool commodity; tag Prepared Textile Fibre; characteristic Colour=brown; characteristic Fine Colour=light brown"
			],
			[
				"CommodityProduct - 350 grams of wool commodity; tag Prepared Textile Fibre; characteristic Colour=brown; characteristic Fine Colour=light brown"
			]);

		AddAncientTextileCraft(
			"sort cotton bolls for ginning",
			"Farming",
			"sort cotton bolls into usable fibre stock",
			"sorting cotton bolls",
			"cotton bolls being sorted for ginning",
			"Farming",
			1,
			Difficulty.Easy,
			[
				(25, "$0 pick|picks clean cotton bolls from $i1 and shake|shakes away leaves and field grit.", "$0 pick|picks through $i1, but leave|leaves too much brittle stalk and dirt among the bolls."),
				(25, "$0 sort|sorts the better bolls into loose baskets for fibre cleaning.", "$0 sort|sorts the bolls poorly, mixing damaged fibre through the usable stock."),
				(20, "$0 gather|gathers $p1 for ginning and combing.", "$0 salvage|salvages only $f1 from the badly sorted cotton.")
			],
			["Commodity - 1 kilogram 200 grams of cotton crop"],
			[],
			["CommodityProduct - 1 kilogram of cotton crop commodity; tag Raw Textile Fibre"],
			["CommodityProduct - 400 grams of cotton crop commodity; tag Raw Textile Fibre"]);

		AddAncientTextileCraft(
			"clean and comb cotton fibre",
			"Weaving",
			"clean seed and comb raw cotton into prepared spinning fibre",
			"cleaning cotton fibre",
			"cotton fibre being cleaned and combed",
			"Weaving",
			1,
			Difficulty.Normal,
			[
				(30, "$0 open|opens $i1 by hand, pulling seed and husk from the fibre.", "$0 open|opens $i1 unevenly and leave|leaves seed fragments in the fibre."),
				(35, "$0 comb|combs the cotton with $t1 into light, workable rolls.", "$0 comb|combs the cotton too roughly, tearing much of the fibre into knots."),
				(35, "$0 draw|draws the clean fibre into soft prepared bundles.", "$0 draw|draws only patchy, lumpy bundles from the fibre."),
				(20, "$0 set|sets aside $p1.", "$0 set|sets aside only $f1.")
			],
			["Commodity - 1 kilogram of cotton crop; piletag Raw Textile Fibre"],
			["TagTool - Held - an item with the Fibre Comb tag"],
			[
				"CommodityProduct - 600 grams of cotton commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
			],
			[
				"CommodityProduct - 220 grams of cotton commodity; tag Prepared Textile Fibre; characteristic Colour=white; characteristic Fine Colour=bone white"
			]);

		AddAncientTextileCraft(
			"spin linen yarn on a drop spindle",
			"Spinning",
			"spin prepared linen fibre into yarn",
			"spinning linen yarn",
			"linen fibre being spun into yarn",
			"Spinning",
			1,
			Difficulty.Normal,
			[
				(30, "$0 dress|dresses $i1 onto $t2 and draw|draws out a fine leader.", "$0 dress|dresses $i1 unevenly onto $t2."),
				(45, "$0 spin|spins $t1, drafting the fibres into a steady thread.", "$0 spin|spins $t1 with an uneven draft, snapping the thread repeatedly."),
				(45, "$0 wind|winds the new yarn back onto $t1 and continue|continues drafting.", "$0 wind|winds lumpy, weak yarn back onto $t1."),
				(20, "$0 finish|finishes $p1.", "$0 salvage|salvages only $f1 from the uneven spinning.")
			],
			["Commodity - 500 grams of linen; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any"],
			[
				"TagTool - Held - an item with the Drop Spindle tag",
				"TagTool - Held - an item with the Distaff tag"
			],
			["CommodityProduct - 450 grams of linen commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 150 grams of linen commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		AddAncientTextileCraft(
			"spin wool yarn on a drop spindle",
			"Spinning",
			"spin prepared wool fibre into yarn",
			"spinning wool yarn",
			"wool fibre being spun into yarn",
			"Spinning",
			1,
			Difficulty.Easy,
			[
				(30, "$0 arrange|arranges $i1 on $t2 and set|sets a twist with $t1.", "$0 arrange|arranges $i1 poorly on $t2."),
				(45, "$0 draft|drafts wool steadily as $t1 turns.", "$0 draft|drafts the wool unevenly as $t1 turns."),
				(45, "$0 wind|winds the growing yarn around $t1.", "$0 wind|winds a lumpy, weak thread around $t1."),
				(20, "$0 finish|finishes $p1.", "$0 salvage|salvages only $f1 from the uneven spinning.")
			],
			["Commodity - 500 grams of wool; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any"],
			[
				"TagTool - Held - an item with the Drop Spindle tag",
				"TagTool - Held - an item with the Distaff tag"
			],
			["CommodityProduct - 460 grams of wool commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 180 grams of wool commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		AddAncientTextileCraft(
			"spin cotton yarn on a drop spindle",
			"Spinning",
			"spin prepared cotton fibre into yarn",
			"spinning cotton yarn",
			"cotton fibre being spun into yarn",
			"Spinning",
			1,
			Difficulty.Normal,
			[
				(30, "$0 dress|dresses $i1 onto $t2 and twist|twists a soft leader onto $t1.", "$0 dress|dresses $i1 unevenly onto $t2."),
				(45, "$0 spin|spins $t1 steadily, drafting the cotton into a fine thread.", "$0 spin|spins $t1 with a ragged draft, breaking the thread again and again."),
				(45, "$0 wind|winds the cotton yarn back onto $t1 and continue|continues drafting.", "$0 wind|winds weak, slubby yarn back onto $t1."),
				(20, "$0 finish|finishes $p1.", "$0 salvage|salvages only $f1 from the uneven spinning.")
			],
			["Commodity - 450 grams of cotton; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any"],
			[
				"TagTool - Held - an item with the Drop Spindle tag",
				"TagTool - Held - an item with the Distaff tag"
			],
			["CommodityProduct - 400 grams of cotton commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 140 grams of cotton commodity; tag Spun Yarn; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		AddAncientTextileCraft(
			"weave linen garment cloth on a warp-weighted loom",
			"Weaving",
			"weave linen yarn into garment cloth",
			"weaving linen garment cloth",
			"linen yarn being woven into garment cloth",
			"Weaving",
			10,
			Difficulty.Normal,
			[
				(45, "$0 hang|hangs the warp on $t1 and set|sets $t2 along the lower threads.", "$0 hang|hangs the warp unevenly on $t1."),
				(45, "$0 pass|passes $t3 through the shed and beat|beats the weft into place with $t4.", "$0 pass|passes $t3 poorly and beat|beats the weft out of line with $t4."),
				(60, "$0 continue|continues weaving an even rectangular length of cloth.", "$0 continue|continues weaving, but the edges wander and tighten."),
				(30, "$0 cut|cuts down $p1 from the loom.", "$0 cut|cuts down only $f1 from the flawed weaving.")
			],
			["Commodity - 520 grams of linen; piletag Spun Yarn; characteristic Colour any; characteristic Fine Colour any"],
			[
				"TagTool - InRoom - an item with the Warp-Weighted Loom tag",
				"TagTool - InRoom - an item with the Loom Weight tag",
				"TagTool - Held - an item with the Shuttle tag",
				"TagTool - Held - an item with the Weaver's Sword tag"
			],
			["CommodityProduct - 430 grams of linen commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 160 grams of linen commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		AddAncientTextileCraft(
			"weave wool garment cloth on a warp-weighted loom",
			"Weaving",
			"weave wool yarn into garment cloth",
			"weaving wool garment cloth",
			"wool yarn being woven into garment cloth",
			"Weaving",
			10,
			Difficulty.Normal,
			[
				(45, "$0 hang|hangs the wool warp on $t1 and weight|weights it with $t2.", "$0 hang|hangs the wool warp unevenly on $t1."),
				(45, "$0 throw|throws $t3 through the shed and beat|beats the weft with $t4.", "$0 throw|throws $t3 unevenly and beat|beats the weft out of line with $t4."),
				(60, "$0 continue|continues working the broad woollen rectangle.", "$0 continue|continues weaving, but the cloth grows uneven and sleazy."),
				(30, "$0 cut|cuts down $p1 from the loom.", "$0 cut|cuts down only $f1 from the flawed weaving.")
			],
			["Commodity - 1 kilogram of wool; piletag Spun Yarn; characteristic Colour any; characteristic Fine Colour any"],
			[
				"TagTool - InRoom - an item with the Warp-Weighted Loom tag",
				"TagTool - InRoom - an item with the Loom Weight tag",
				"TagTool - Held - an item with the Shuttle tag",
				"TagTool - Held - an item with the Weaver's Sword tag"
			],
			["CommodityProduct - 870 grams of wool commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 320 grams of wool commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		AddAncientTextileCraft(
			"weave cotton garment cloth on a warp-weighted loom",
			"Weaving",
			"weave cotton yarn into garment cloth",
			"weaving cotton garment cloth",
			"cotton yarn being woven into garment cloth",
			"Weaving",
			10,
			Difficulty.Normal,
			[
				(45, "$0 hang|hangs the cotton warp on $t1 and weights the threads with $t2.", "$0 hang|hangs the cotton warp unevenly on $t1."),
				(45, "$0 pass|passes $t3 through the shed and beat|beats the weft into place with $t4.", "$0 pass|passes $t3 poorly and beat|beats the weft out of line with $t4."),
				(60, "$0 continue|continues weaving a light rectangular cotton length.", "$0 continue|continues weaving, but the cloth tightens and pulls out of square."),
				(30, "$0 cut|cuts down $p1 from the loom.", "$0 cut|cuts down only $f1 from the flawed weaving.")
			],
			["Commodity - 460 grams of cotton; piletag Spun Yarn; characteristic Colour any; characteristic Fine Colour any"],
			[
				"TagTool - InRoom - an item with the Warp-Weighted Loom tag",
				"TagTool - InRoom - an item with the Loom Weight tag",
				"TagTool - Held - an item with the Shuttle tag",
				"TagTool - Held - an item with the Weaver's Sword tag"
			],
			["CommodityProduct - 380 grams of cotton commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 140 grams of cotton commodity; tag Woven Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		void AddDyeCraft(string name, string material, string dyeMaterial, string basicColour, string fineColour)
		{
			AddAncientTextileCraft(
				name,
				"Dyeing",
				$"dye {material} garment cloth {fineColour}",
				$"dyeing {material} cloth",
				$"{material} cloth being dyed",
				"Dyeing",
				10,
				Difficulty.Normal,
				[
					(35, "$0 heat|heats water and mordant in $t2, stirring $i3 into the bath.", "$0 heat|heats the mordant bath unevenly in $t2."),
					(35, "$0 steep|steeps $i2 in $t1, drawing colour into the dye bath.", "$0 steep|steeps $i2 poorly in $t1, leaving weak colour."),
					(45, "$0 immerse|immerses $i1 in the dye bath, turning the cloth steadily.", "$0 immerse|immerses $i1 unevenly, leaving blotched patches."),
					(25, "$0 lift|lifts out $p1 and set|sets it to drain.", "$0 lift|lifts out only $f1 from the spoiled dyeing.")
				],
				[
					$"Commodity - 420 grams of {material}; piletag Garment Cloth; characteristic Colour any; characteristic Fine Colour any",
					$"Commodity - 100 grams of {dyeMaterial}",
					"Commodity - 40 grams of alum mordant",
					"LiquidUse - 8 litres of Water"
				],
				[
					"TagTool - InRoom - an item with the Dye Vat tag",
					"TagTool - InRoom - an item with the Mordant Cauldron tag"
				],
				[
					$"CommodityProduct - 390 grams of {material} commodity; tag Dyed Cloth; characteristic Colour={basicColour}; characteristic Fine Colour={fineColour}"
				],
				[
					$"CommodityProduct - 150 grams of {material} commodity; tag Dyed Cloth; characteristic Colour={basicColour}; characteristic Fine Colour={fineColour}"
				]);
		}

		AddDyeCraft("dye wool cloth madder red", "wool", "madder root", "red", "crimson");
		AddDyeCraft("dye wool cloth indigo blue", "wool", "indigo dye cake", "dark blue", "deep indigo");
		AddDyeCraft("dye linen cloth ochre yellow", "linen", "ochre pigment", "yellow", "ochre");
		AddDyeCraft("dye linen cloth saffron yellow", "linen", "saffron", "yellow", "golden yellow");
		AddDyeCraft("dye cotton cloth madder red", "cotton", "madder root", "red", "crimson");
		AddDyeCraft("dye cotton cloth indigo blue", "cotton", "indigo dye cake", "dark blue", "deep indigo");

		AddAncientTextileCraft(
			"full wool garment cloth",
			"Weaving",
			"full woven or dyed wool cloth for denser garments",
			"fulling wool cloth",
			"wool cloth being fulled",
			"Weaving",
			15,
			Difficulty.Normal,
			[
				(35, "$0 soak|soaks $i1 in $t1 and turn|turns the cloth through the water.", "$0 soak|soaks $i1 unevenly in $t1."),
				(45, "$0 pound|pounds the wet wool with $t2 until the weave thickens.", "$0 pound|pounds the wool too harshly with $t2."),
				(45, "$0 stretch|stretches the cloth across $t3 to dry at a useful width.", "$0 stretch|stretches the cloth poorly across $t3."),
				(25, "$0 take|takes down $p1.", "$0 take|takes down only $f1 from the misshapen cloth.")
			],
			[
				"Commodity - 900 grams of wool; piletag Garment Cloth; characteristic Colour any; characteristic Fine Colour any",
				"LiquidUse - 10 litres of Water"
			],
			[
				"TagTool - InRoom - an item with the Fuller's Trough tag",
				"TagTool - Held - an item with the Fuller's Mallet tag",
				"TagTool - InRoom - an item with the Cloth Tenter Frame tag"
			],
			["CommodityProduct - 820 grams of wool commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 300 grams of wool commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		AddAncientTextileCraft(
			"felt prepared wool fibre into garment felt",
			"Weaving",
			"felt prepared wool fibre into dense garment felt",
			"felting wool fibre",
			"prepared wool fibre being felted into garment felt",
			"Weaving",
			15,
			Difficulty.Normal,
			[
				(35, "$0 soak|soaks $i1 in $t1 and spread|spreads the fibre into even overlapping layers.", "$0 soak|soaks $i1 unevenly and leave|leaves thin patches in the fibre."),
				(45, "$0 pound|pounds and roll|rolls the wet wool with $t2 until the fibres mat together.", "$0 pound|pounds the wool too harshly with $t2, tearing weak patches through it."),
				(45, "$0 stretch|stretches the felt over $t3 to dry into usable garment sheets.", "$0 stretch|stretches the felt poorly across $t3."),
				(25, "$0 take|takes down $p1.", "$0 take|takes down only $f1 from the misshapen felt.")
			],
			[
				"Commodity - 700 grams of wool; piletag Prepared Textile Fibre; characteristic Colour any; characteristic Fine Colour any",
				"LiquidUse - 8 litres of Water"
			],
			[
				"TagTool - InRoom - an item with the Fuller's Trough tag",
				"TagTool - Held - an item with the Fuller's Mallet tag",
				"TagTool - InRoom - an item with the Cloth Tenter Frame tag"
			],
			["CommodityProduct - 520 grams of felt commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"],
			["CommodityProduct - 180 grams of felt commodity; tag Fulled Cloth; characteristic Colour from $i1; characteristic Fine Colour from $i1"]);

		var missingGarments = HellenicAntiquityClothingStableReferences.Keys
			.Where(x => !TryLookupReworkItem(x, out _))
			.ToList();
		if (missingGarments.Count > 0)
		{
			return;
		}

		var assemblyPhases = new (int Seconds, string Echo, string FailEcho)[]
		{
			(35, "$0 lay|lays out $i1 and measure|measures a rectangular garment length, trimming it with $t2.", "$0 lay|lays out $i1, but cut|cuts the rectangle unevenly with $t2."),
			(35, "$0 thread|threads $t1 and turn|turns the edges into a neat hem.", "$0 thread|threads $t1, but the hems pucker and wander."),
			(40, "$0 join|joins and reinforce|reinforces the folds, openings, or fastening points appropriate to the garment.", "$0 join|joins the cloth poorly, leaving weak folds and awkward openings."),
			(25, "$0 shake|shakes out $p1, checking the drape.", "$0 shake|shakes out only $f1 after the cloth is spoiled.")
		};
		var sewingTools = new[]
		{
			"TagTool - Held - an item with the Sewing Needle tag",
			"TagTool - Held - an item with the Shears tag"
		};

		foreach (var garment in new[]
		{
			(StableReference: "antiquity_short_wool_chiton", Name: "assemble a short wool chiton", Material: "wool", Cloth: 470, Yarn: 30, Fine: false, Minimum: 15, Difficulty: Difficulty.Easy),
			(StableReference: "antiquity_wool_himation", Name: "assemble a wool himation", Material: "wool", Cloth: 1000, Yarn: 40, Fine: false, Minimum: 15, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_linen_chiton", Name: "assemble a fine linen chiton", Material: "linen", Cloth: 340, Yarn: 25, Fine: true, Minimum: 35, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_wool_himation", Name: "assemble a fine wool himation", Material: "wool", Cloth: 900, Yarn: 35, Fine: true, Minimum: 35, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_short_wool_chlamys", Name: "assemble a short wool chlamys", Material: "wool", Cloth: 650, Yarn: 30, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_full_length_wool_peplos", Name: "assemble a full-length wool peplos", Material: "wool", Cloth: 780, Yarn: 35, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_full_wool_himation", Name: "assemble a full wool himation", Material: "wool", Cloth: 960, Yarn: 40, Fine: false, Minimum: 20, Difficulty: Difficulty.Normal),
			(StableReference: "antiquity_fine_long_linen_chiton", Name: "assemble a fine long linen chiton", Material: "linen", Cloth: 410, Yarn: 30, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_fine_full_wool_himation", Name: "assemble a fine full wool himation", Material: "wool", Cloth: 880, Yarn: 35, Fine: true, Minimum: 40, Difficulty: Difficulty.Hard),
			(StableReference: "antiquity_light_linen_head_veil", Name: "assemble a light linen head veil", Material: "linen", Cloth: 130, Yarn: 10, Fine: true, Minimum: 30, Difficulty: Difficulty.Normal)
		})
		{
			var characteristicRequirements = garment.Fine
				? "characteristic Colour any; characteristic Fine Colour any"
				: "characteristic Colour any";
			AddHellenicGarmentCraft(
				garment.Name,
				$"assemble {garment.Name["assemble a ".Length..]} from rectangular {garment.Material} cloth",
				garment.Name,
				$"{garment.Material} cloth being assembled into Hellenic clothing",
				garment.Minimum,
				garment.Difficulty,
				assemblyPhases,
				[
					$"Commodity - {garment.Cloth} grams of {garment.Material}; piletag Garment Cloth; {characteristicRequirements}",
					$"Commodity - {garment.Yarn} grams of {garment.Material}; piletag Spun Yarn; characteristic Colour any"
				],
				sewingTools,
				[StableVariableProduct(garment.StableReference, garment.Fine)]);
		}
	}
}

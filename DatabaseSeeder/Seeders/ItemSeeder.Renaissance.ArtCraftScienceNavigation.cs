#nullable enable

using MudSharp.GameItems;

namespace DatabaseSeeder.Seeders;

public partial class ItemSeeder
{
	private void SeedRenaissanceArtCraftScienceAndNavigation()
	{
		var artTags = new[] { "Era / Renaissance Era", "Market / Household Goods / Standard Wares" };
		SeedStraightforwardEraCatalogueItems(
			"Renaissance art, craft, science, and navigation catalogue",
			[
				new("renaissance_art_painter_easel", "easel", "a folding painter's easel", "A jointed oak easel has an adjustable ledge and rear leg for holding a panel or canvas in a workshop.", SizeCategory.Large, ItemQuality.Standard, 6400.0, 42.0m, "oak", artTags, ["Destroyable_Furniture"], "Renaissance workshop easel; fixed work position is descriptive only."),
				new("renaissance_art_prepared_canvas", "canvas", "a primed linen canvas", "A stretched linen canvas has a pale prepared face ready for paint, charcoal studies, or a builder-authored skin.", SizeCategory.Normal, ItemQuality.Standard, 820.0, 14.0m, "linen", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance painting surface."),
				new("renaissance_art_wooden_palette", "palette", "an oval wooden paint palette", "An oval hardwood palette has a thumb hole and a smooth face for arranging small pools of pigment.", SizeCategory.Small, ItemQuality.Standard, 280.0, 7.0m, "oak", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance painter's hand tool."),
				new("renaissance_art_hog_brush", "brush", "a hog-bristle paint brush", "A short wooden brush carries a tightly bound bundle of stiff bristles for working paint into a prepared surface.", SizeCategory.Small, ItemQuality.Standard, 90.0, 4.0m, "wood", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance painter's brush."),
				new("renaissance_art_pigment_shell", "shell", "a small pigment shell", "A shallow shell is clean enough to hold a little ground pigment, oil, or wash during careful painting work.", SizeCategory.Small, ItemQuality.Standard, 45.0, 2.0m, "shell", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance pigment-serving vessel."),
				new("renaissance_art_granite_muller", "muller", "a granite pigment muller", "A rounded granite hand stone has a flat working face for grinding pigments on a slab.", SizeCategory.Small, ItemQuality.Good, 1800.0, 10.0m, "granite", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance pigment-grinding hand tool."),
				new("renaissance_art_drawing_board", "board", "a smooth drawing board", "A thin smooth board gives an artist or surveyor a firm portable backing for paper and parchment sheets.", SizeCategory.Normal, ItemQuality.Standard, 900.0, 8.0m, "oak", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance drawing and field-note support."),
				new("renaissance_art_silverpoint_stylus", "stylus", "a silverpoint drawing stylus", "A slim handled stylus carries a soft silver point for fine lines on prepared paper or ground panels.", SizeCategory.Small, ItemQuality.Good, 85.0, 18.0m, "silver", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance drawing implement."),
				new("renaissance_art_proportional_dividers", "dividers", "a pair of proportional dividers", "Two brass arms pivot on a marked joint for transferring and comparing proportions in drawing, carving, and surveying.", SizeCategory.Small, ItemQuality.Good, 210.0, 24.0m, "brass", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance proportional-measurement tool; no automatic calculation is implied."),
				new("renaissance_art_carving_chisel", "chisel", "a narrow carving chisel", "A small steel-edged chisel set in an oak handle is ground for controlled cuts in wood, wax, and soft plaster.", SizeCategory.Small, ItemQuality.Standard, 240.0, 11.0m, "wrought iron", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance carving tool."),
				new("renaissance_art_wax_model", "model", "a small wax study model", "A palm-sized beeswax model preserves the rough planes and proportions of a proposed figure or ornament.", SizeCategory.Small, ItemQuality.Standard, 360.0, 9.0m, "beeswax", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance sculptural study; no morph or mould mechanic is claimed."),
				new("renaissance_art_plaster_cast", "cast", "a white plaster study cast", "A white plaster cast records a hand, face, or ornament in durable workshop form for drawing and carving practice.", SizeCategory.Normal, ItemQuality.Standard, 3400.0, 18.0m, "plaster", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance workshop study object."),
				new("renaissance_art_pattern_book", "book", "a bound pattern book", "A bound paper book leaves room for copied decorative patterns, workshop diagrams, and measured ornament.", SizeCategory.Small, ItemQuality.Standard, 540.0, 14.0m, "paper", artTags, ["Holdable", "Destroyable_Misc", "Book_Small_90_Page"], "Renaissance design reference; content remains builder-authored."),
				new("renaissance_science_brass_quadrant", "quadrant", "a graduated brass quadrant", "A quarter-circle brass instrument bears simple degree marks and a plumb line for visual observation and demonstration.", SizeCategory.Small, ItemQuality.Good, 720.0, 35.0m, "brass", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance observational instrument; no automatic measurement is implied."),
				new("renaissance_science_sector_rule", "sector", "a folding brass sector", "A folding pair of engraved brass rules provides proportional scales for workshop calculation and plotting.", SizeCategory.Small, ItemQuality.Good, 420.0, 28.0m, "brass", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance calculation aid; no automatic calculation is implied."),
				new("renaissance_science_sandglass", "sandglass", "a half-hour sandglass", "Fine sand runs between two clear glass bulbs held in a simple wooden frame for a visibly bounded interval.", SizeCategory.Small, ItemQuality.Standard, 390.0, 12.0m, "glass", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance timing prop; no timekeeping mechanics are claimed."),
				new("renaissance_science_horizontal_sundial", "sundial", "a brass horizontal sundial", "A flat brass plate and raised gnomon cast a shifting shadow across engraved hour lines in suitable sunlight.", SizeCategory.Small, ItemQuality.Good, 980.0, 30.0m, "brass", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance timekeeping prop; no automatic time mechanic is implied."),
				new("renaissance_science_armillary_sphere", "sphere", "a small brass armillary sphere", "Nested brass rings surround a small central globe in a display of celestial circles and axes.", SizeCategory.Normal, ItemQuality.Good, 2600.0, 85.0m, "brass", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance astronomical display; no observation bonus is implied."),
				new("renaissance_navigation_sounding_line", "line", "a coiled lead sounding line", "A tarred line is neatly coiled around a shaped lead weight for taking manual soundings from a boat or quay.", SizeCategory.Normal, ItemQuality.Standard, 4100.0, 16.0m, "lead", artTags, ["Holdable", "Destroyable_Misc"], "Renaissance maritime hand tool; no automatic depth measurement is implied.")
			]);
	}
}

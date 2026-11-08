#nullable enable

using MudSharp.GameItems;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public static class ClothingSkinsStock
{
	public static IReadOnlyList<CatalogueSkinDefinition> Items() =>
	[
		new("antiquity_skin_senatorial_broad_striped_tunica", "antiquity_fine_linen_tunica", "tunica", "a broad-striped white linen tunica", "This finely woven white linen tunica falls in measured folds from the shoulders to the knees. Two broad coloured stripes run vertically from the shoulder line toward the hem, standing out clearly against the pale body without changing the garment’s simple cut. The neck and sleeve edges are narrowly turned and stitched, and the cloth gathers cleanly beneath a formal waist tie. Its conspicuous bands and careful finish give an otherwise familiar tunica the unmistakable gravity of civic office.", ItemQuality.Good),
		new("antiquity_skin_vestal_white_priestly_veil", "adjacent_antiquity_priestly_linen_veil", "veil", "a long white linen priestly veil", "This long veil is cut from clean white linen with enough breadth to cover the hair and fall over both shoulders. Its upper edge is shaped to sit securely above a narrow ritual headband, while the side folds frame the face without obscuring it. Fine rolled hems keep the light cloth from fraying and give the hanging edges a controlled, formal line. The complete absence of coloured trim makes the carefully maintained whiteness the veil’s most striking feature.", ItemQuality.Good),
		new("renaissance_skin_venetian_senatorial_red_gown", "renaissance_western_furred_gown", "gown", "a scarlet fur-edged senatorial gown", "This long scarlet gown is cut with a full body and broad sleeves, allowing it to fall in weighty vertical folds from shoulder to ankle. A restrained line of pale fur borders the opening and cuffs, setting off the saturated cloth without overwhelming its severe formal shape. The front is intended to lie open over fitted underclothing, and the shoulders are tailored to hold a dignified square line. Its rich colour, sober proportions, and controlled edging make it suitable for a high civic officer rather than ordinary court display.", ItemQuality.VeryGood),
		new("earlymodern_skin_judicial_full_sleeved_robe", "renaissance_institution_academic_robe", "robe", "a long $colour full-sleeved judicial robe", "This open-fronted robe falls in long folds of smooth $colour broadcloth. Its full sleeves gather at reinforced shoulders, with narrow rows of stitching following the seams. Matching cloth facings lie flat along the front edges, their outer borders finished with closely worked braid. The deep hem and unbroken panels give it a straight outline, while the restrained trim carries no fur, contrasting badge or embroidered court insignia.", null),
		new("earlymodern_skin_formal_mourning_mantua", "earlymodern_western_clothing_plain_mantua_gown", "mantua", "a full black formal mourning mantua", "This full mantua is made from matte black cloth whose restrained surface absorbs light rather than displaying a glossy finish. The bodice is closely arranged over the torso before opening into a long trained skirt, with the outer folds drawn back just enough to reveal a matching dark petticoat beneath. Sleeves, cuffs, and edges are finished with plain self-coloured bands instead of bright lace or jewellery. Its fashionable construction remains evident, but every decorative choice has been deliberately subdued for formal mourning.", ItemQuality.VeryGood)
	];
}

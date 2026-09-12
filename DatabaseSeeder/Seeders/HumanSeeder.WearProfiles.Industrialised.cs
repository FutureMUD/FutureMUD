#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
	/// <summary>
	/// Authored coverage for the approved Industrialised wardrobe. These are body-location profiles,
	/// not proof of garment attachments, infant sizing, protection or completed outfit compatibility.
	/// Shared location sets describe geometry only; every profile's help text is authored in full.
	/// </summary>
	private static StockHumanWearProfileDefinition[] IndustrialisedHumanWearProfiles()
	{
		StockHumanWearProfileLocation[] Cover(params string[] parts) => parts.Select(x => Loc(x, true, false, false, true, false)).ToArray();
		StockHumanWearProfileLocation[] Optional(params string[] parts) => parts.Select(x => Loc(x, false, false, false, false, false)).ToArray();
		StockHumanWearProfileLocation[] Partial(params string[] parts) => parts.Select(x => Loc(x, true, false, true, false, false)).ToArray();
		StockHumanWearProfileLocation[] OptionalPartial(params string[] parts) => parts.Select(x => Loc(x, false, false, true, false, false)).ToArray();
		StockHumanWearProfileLocation[] Extremities(params string[] parts) => parts.Select(x => Loc(x, false, false, false, true, true)).ToArray();

		var torso = Cover("rbreast", "lbreast", "uback", "abdomen", "belly", "lback");
		var nipples = Extremities("rnipple", "lnipple");
		var shoulders = Cover("rshoulder", "lshoulder");
		var arms = Optional("rupperarm", "lupperarm", "relbow", "lelbow", "rforearm", "lforearm", "rwrist", "lwrist");
		var thighs = Optional("rthigh", "lthigh", "rthighback", "lthighback");
		var lowerLegs = Optional("rknee", "lknee", "rkneeback", "lkneeback", "rshin", "lshin", "rcalf", "lcalf", "rankle", "lankle");
		var hips = Cover("rhip", "lhip");
		var seat = Optional("rbuttock", "lbuttock");
		var genitals = Extremities("penis", "testicles");
		var neck = Cover("neck", "bneck", "throat");
		var toes = Extremities("rbigtoe", "lbigtoe", "rindextoe", "lindextoe", "rmiddletoe", "lmiddletoe", "rringtoe", "lringtoe", "rpinkytoe", "lpinkytoe");
		var fingers = Extremities("rthumb", "lthumb", "rindexfinger", "lindexfinger", "rmiddlefinger", "lmiddlefinger", "rringfinger", "lringfinger", "rpinkyfinger", "lpinkyfinger");

		return
		[
			StockDirectWearProfile("Garters", "tied around", "tie", "ties", "around",
				"Paired narrow bands tied around the knees to secure hose. They do not enclose the hips, thighs, lower legs or feet, and do not act as leggings.", false, false,
				[.. Partial("rknee", "lknee", "rkneeback", "lkneeback")]),
			StockDirectWearProfile("Split Drawers", "worn on", "put", "puts", "on",
				"Drawers with a divided crotch: the waist, seat and legs are covered, but the groin is not closed by this garment.", false, false,
				[.. Partial("belly", "lback"), .. hips, .. seat, .. thighs, .. Optional("rknee", "lknee", "rkneeback", "lkneeback")]),
			StockDirectWearProfile("Camisole", "worn on", "slip", "slips", "on",
				"A sleeveless underbodice covering the trunk, with narrow shoulder straps and no arm or neck coverage.", false, false,
				[.. torso, .. nipples, .. Partial("rshoulder", "lshoulder")]),
			StockDirectWearProfile("Nappy", "fastened around", "fasten", "fastens", "around",
				"A waist-fastened cloth nappy covering the seat and crotch. Garment size and any separate fasteners must be configured independently.", false, false,
				[.. Partial("belly", "lback"), .. hips, .. Cover("groin", "rbuttock", "lbuttock"), .. genitals]),
			StockDirectWearProfile("Infant Bodysuit", "worn on", "put", "puts", "on",
				"A short-sleeved bodysuit covering the trunk, hips, seat and crotch, leaving the legs bare. This profile does not impose infant sizing.", false, false,
				[.. torso, .. nipples, .. shoulders, .. Optional("rupperarm", "lupperarm"), .. hips, .. seat, .. Cover("groin"), .. genitals]),
			StockDirectWearProfile("Infant Gown", "worn on", "put", "puts", "on",
				"A long-sleeved, open-bottom gown covering the trunk and hanging over the legs. Infant size is an item-fit requirement, not an age restriction in this profile.", false, false,
				[.. torso, .. nipples, .. shoulders, .. arms, .. hips, .. seat, .. Optional("groin"), .. genitals, .. thighs, .. lowerLegs]),
			StockDirectWearProfile("Short Stays", "laced around", "lace", "laces", "around",
				"Short foundation stays enclosing the breasts and upper back, with narrow straps; the lower abdomen remains uncovered.", false, false,
				[.. Cover("rbreast", "lbreast", "uback"), .. nipples, .. Partial("rshoulder", "lshoulder")]),
			StockDirectWearProfile("Rear Skirt Support", "fastened behind", "fasten", "fastens", "behind",
				"A waist-supported rear bustle or half-hoop structure. Its partial coverage does not make an opaque skirt or cover the front of the legs.", false, true,
				[.. Partial("belly", "lback", "rhip", "lhip"), .. OptionalPartial("rbuttock", "lbuttock", "rthighback", "lthighback")]),
			StockDirectWearProfile("Girdle", "worn around", "pull", "pulls", "on",
				"An open-bottom elastic foundation enclosing the lower trunk, hips and seat without closing the crotch. Stocking connections require separate support.", false, false,
				[.. Cover("belly", "abdomen", "lback"), .. hips, .. seat, .. thighs]),
			StockDirectWearProfile("Stocking Support Belt", "fastened around", "fasten", "fastens", "around",
				"A narrow waist belt with descending stocking straps; it neither covers the crotch nor supplies equipment-belt or stocking-attachment mechanics.", false, false,
				[.. Partial("belly", "lback", "rhip", "lhip"), .. OptionalPartial("rthigh", "lthigh")]),
			StockDirectWearProfile("High-Neck Shirt", "worn on", "put", "puts", "on",
				"A closed high-neck shirt with full sleeves, covering the trunk and neck but not the head or hands.", false, false,
				[.. torso, .. nipples, .. neck, .. shoulders, .. arms]),
			StockDirectWearProfile("Bib Overalls", "worn on", "pull", "pulls", "on",
				"Trousers with a front bib and narrow shoulder straps. The upper back and arms remain uncovered; the bib does not act as a shirt.", false, false,
				[.. Cover("rbreast", "lbreast", "abdomen", "belly", "lback", "groin"), .. nipples, .. Partial("rshoulder", "lshoulder"),
				 .. hips, .. seat, .. genitals, .. thighs, .. lowerLegs]),
			StockDirectWearProfile("Trained Skirt", "worn around", "fasten", "fastens", "around",
				"A full-length skirt whose train trails behind the wearer. The train adds no foot coverage or separate movement mechanic; account for its mass on the item.", false, false,
				[.. Partial("belly", "lback"), .. hips, .. seat, .. Optional("groin"), .. genitals, .. thighs, .. lowerLegs]),
			StockDirectWearProfile("Cutaway Coat", "worn on", "put", "puts", "on",
				"A sleeved coat with a closed upper front and skirts cut away at the front of the hips; its back skirts hang over the seat and rear thighs.", false, false,
				[.. Cover("rbreast", "lbreast", "uback", "lback"), .. nipples, .. shoulders, .. arms, .. OptionalPartial("abdomen", "belly", "rhip", "lhip"),
				 .. seat, .. Optional("rthighback", "lthighback")]),
			StockDirectWearProfile("Tailcoat", "worn on", "put", "puts", "on",
				"An open-front evening coat with full sleeves and long rear tails. The front waist and thighs are not enclosed by the tails.", false, false,
				[.. Cover("uback", "lback"), .. Partial("rbreast", "lbreast"), .. shoulders, .. arms,
				 .. seat, .. Optional("rthighback", "lthighback")]),
			StockDirectWearProfile("Hooded Long Coat", "worn on", "put", "puts", "on",
				"A long coat with its hood raised, covering the scalp, rear head and neck as well as the torso and arms; the face remains open.", false, true,
				[.. torso, .. nipples, .. shoulders, .. arms, .. hips, .. seat, .. thighs, .. Optional("groin"), .. genitals, .. neck,
				 .. Cover("scalp", "bhead"), .. Optional("rear", "lear", "rtemple", "ltemple")]),
			StockDirectWearProfile("Hooded Long Coat Lowered", "worn on", "put", "puts", "on",
				"The same closed long coat with its hood lowered. Torso, arm, neck and skirt coverage is unchanged, while the head and ears are uncovered.", false, true,
				[.. torso, .. nipples, .. shoulders, .. arms, .. hips, .. seat, .. thighs, .. Optional("groin"), .. genitals, .. neck]),
			StockDirectWearProfile("Detachable Collar", "fastened around", "fasten", "fastens", "around",
				"A separate folded collar around the base of the neck. This locates the collar but does not attach it to an independently worn shirt.", false, false,
				[.. Partial("neck", "bneck", "throat")]),
			StockDirectWearProfile("Standing Collar", "fastened around", "fasten", "fastens", "around",
				"A separate upright collar enclosing the neck and throat. Shirt fasteners and removal relationships require independent configuration.", false, false,
				[.. neck]),
			StockDirectWearProfile("Detachable Cuffs", "fastened around", "fasten", "fastens", "around",
				"A pair of separate cuffs enclosing the wrists, without covering the hands. Sleeve attachment is not supplied by the wear profile.", false, false,
				[.. Cover("rwrist", "lwrist")]),
			StockDirectWearProfile("Shirtfront", "worn over", "put", "puts", "over",
				"A separate shirtfront panel over the chest and upper abdomen, with no back or sleeve coverage.", false, false,
				[.. Cover("rbreast", "lbreast", "abdomen"), .. nipples, .. OptionalPartial("throat")]),
			StockDirectWearProfile("Braces", "worn over", "put", "puts", "over",
				"Narrow trouser-support straps over both shoulders and along the trunk. Their partial coverage does not establish attachment to trousers.", false, false,
				[.. Partial("rshoulder", "lshoulder"), .. OptionalPartial("rbreast", "lbreast", "uback", "lback", "belly", "abdomen")]),
			StockDirectWearProfile("Rank Slides", "worn on", "slip", "slips", "on",
				"A pair of small shoulder-strap sleeves. This profile locates their visible insignia; compatible garment straps and attachment must be checked separately.", false, false,
				[.. Partial("rshoulder", "lshoulder")]),
			StockShapeWearProfile("Ribbon Bar", "worn on", "place", "places", "on",
				"A compact ribbon bar displayed on one side of the chest. It does not cover that entire location or confer institutional membership.", false, false,
				ShapeLoc("breast", 1, true, false, true, false, false)),
			StockDirectWearProfile("Long Gloves", "worn on", "pull", "pulls", "on",
				"A pair of long gloves enclosing the hands and extending past the elbows; any absent fingers or lower-arm segments remain optional locations.", false, false,
				[.. Cover("rhand", "lhand"), .. Optional("rwrist", "lwrist", "rforearm", "lforearm", "relbow", "lelbow"),
				 .. OptionalPartial("rupperarm", "lupperarm"), .. fingers]),
			StockDirectWearProfile("Bonnet", "worn on", "put", "puts", "on",
				"A bonnet enclosing the scalp and rear head, with side coverage and narrow ties beneath the chin; it leaves the face unobscured.", false, false,
				[.. Cover("scalp", "bhead"), .. Optional("rear", "lear", "rtemple", "ltemple"), .. OptionalPartial("chin")]),
			StockDirectWearProfile("Hairnet", "worn over", "put", "puts", "over",
				"An open mesh net over the scalp and rear head. Hair and underlying headwear remain visible through the net.", false, false,
				[.. Partial("scalp"), .. OptionalPartial("bhead", "rtemple", "ltemple")]),
			StockDirectWearProfile("Net Skirt", "worn around", "fasten", "fastens", "around",
				"A short layered-net skirt attached at the waist; its open mesh does not conceal the crotch or serve as an opaque underskirt.", false, false,
				[.. Partial("belly", "lback", "rhip", "lhip"), .. OptionalPartial("groin", "rbuttock", "lbuttock", "rthigh", "lthigh", "rthighback", "lthighback")]),
			StockDirectWearProfile("Waist Apron", "tied around", "tie", "ties", "around",
				"A waist-tied apron hanging across the front of the lower trunk and thighs. It has no chest panel and leaves the back uncovered.", false, false,
				[.. Partial("belly", "lback"), .. Optional("abdomen", "groin", "penis", "testicles", "rthigh", "lthigh")]),
			StockDirectWearProfile("Open-Back Gown", "worn on", "put", "puts", "on",
				"A short-sleeved gown with an open rear and ties at the neck. The front is covered while the back and buttocks remain exposed through the opening.", false, false,
				[.. Cover("rbreast", "lbreast", "abdomen", "belly"), .. nipples, .. shoulders, .. Optional("rupperarm", "lupperarm", "groin"),
				 .. genitals, .. Optional("rthigh", "lthigh", "rknee", "lknee"), .. OptionalPartial("bneck")]),
			StockDirectWearProfile("High-Neck Jacket", "worn on", "put", "puts", "on",
				"A closed, high-collared service jacket covering the neck, torso, arms and top of the hips, without head or hand coverage.", false, false,
				[.. torso, .. nipples, .. neck, .. shoulders, .. arms, .. Optional("rhip", "lhip")]),
			StockDirectWearProfile("Cutaway Jacket", "worn on", "put", "puts", "on",
				"A cropped, open-front jacket covering the shoulders, arms and upper back, with short breast panels and no lower abdominal or hip coverage.", false, false,
				[.. Cover("uback"), .. Partial("rbreast", "lbreast"), .. shoulders, .. arms]),
			StockDirectWearProfile("Low-Cut Shoes", "worn on", "put", "puts", "on",
				"Closed-toe low-cut shoes covering toes and heels but leaving much of the upper feet and both ankles exposed.", false, false,
				[.. Partial("rfoot", "lfoot"), .. Optional("rheel", "lheel"), .. toes]),
			StockDirectWearProfile("Backless Shoes", "worn on", "slip", "slips", "on",
				"Closed-toe backless shoes with exposed heels and ankles. Only the toe area and a partial upper-foot panel are enclosed.", false, false,
				[.. Partial("rfoot", "lfoot"), .. toes]),
			StockDirectWearProfile("Infant Footwear", "worn on", "put", "puts", "on",
				"Soft enclosed footwear covering the feet, toes, heels and ankles. Infant sizing and the absence of a rigid walking sole belong to the item definition.", false, false,
				[.. Cover("rfoot", "lfoot"), .. Optional("rheel", "lheel", "rankle", "lankle"), .. toes]),
			StockDirectWearProfile("High-Neck Dress", "worn on", "put", "puts", "on",
				"A high-necked, short-sleeved dress enclosing the torso and hanging over the knees, leaving the forearms, lower legs and feet bare.", false, false,
				[.. torso, .. nipples, .. neck, .. shoulders, .. Optional("rupperarm", "lupperarm"), .. hips, .. seat, .. Optional("groin"),
				 .. genitals, .. thighs, .. Optional("rknee", "lknee", "rkneeback", "lkneeback")]),
			StockDirectWearProfile("Short Wrap Jacket", "worn on", "put", "puts", "on",
				"A short overlapping jacket enclosing the chest, shoulders and arms, ending above the abdomen and leaving the lower trunk uncovered.", false, false,
				[.. Cover("rbreast", "lbreast", "uback"), .. nipples, .. shoulders, .. arms]),
			StockDirectWearProfile("High-Waisted Skirt", "worn around", "fasten", "fastens", "around",
				"A long skirt fastened beneath the chest, covering the abdomen and lower body without becoming a dress or covering the breasts.", false, false,
				[.. Cover("abdomen", "belly", "lback"), .. hips, .. seat, .. Optional("groin"), .. genitals, .. thighs, .. lowerLegs]),
			StockDirectWearProfile("Wide Over-Robe", "worn over", "slip", "slips", "over",
				"A loose over-robe draped from the shoulders over the torso, arms and legs. Its wide sleeves do not cover the hands or provide an additional container.", false, true,
				[.. torso, .. nipples, .. shoulders, .. arms, .. hips, .. seat, .. Optional("groin"), .. genitals, .. thighs, .. lowerLegs]),
			StockDirectWearProfile("Clerical Collar", "fastened around", "fasten", "fastens", "around",
				"A separate narrow collar band at the neck. Its colour and compatible shirt attachment are independent of this wear-location profile.", false, false,
				[.. Partial("neck", "bneck", "throat")]),
			StockDirectWearProfile("Academic Hood", "draped over", "drape", "drapes", "over",
				"An academic hood draped down the upper back from a narrow shoulder-and-neck band. This lowered arrangement does not cover the head.", false, false,
				[.. Partial("rshoulder", "lshoulder"), .. OptionalPartial("bneck", "throat"), .. Optional("uback")]),
			StockDirectWearProfile("Nappy Pins", "worn at", "place", "places", "at",
				"A pair of small guarded pins at the hips. This locates the pins; a separate nappy and supported fastening relationship are still required.", false, false,
				[.. Partial("rhip", "lhip")]),
			StockDirectWearProfile("Spats", "fastened around", "fasten", "fastens", "around",
				"Paired ankle coverings extending over the insteps with narrow underfoot straps. They leave the toes, heels and calves uncovered.", false, false,
				[.. Cover("rankle", "lankle"), .. OptionalPartial("rfoot", "lfoot")])
		];
	}
}

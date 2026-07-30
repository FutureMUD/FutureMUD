#nullable enable

using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
    private readonly Dictionary<string, BodypartProto> _bodyparts = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, BodypartProto> _bones = new(StringComparer.OrdinalIgnoreCase);

    private readonly List<(BodypartProto Child, BodypartProto Parent)> _cachedBodypartUpstreams = new();

    private readonly CollectionDictionary<string, BodypartProto> _cachedLimbs = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, BodypartProto> _organs = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, BodypartShape> _shapes = new(StringComparer.OrdinalIgnoreCase);
    private ArmourType _boneArmour = null!;

    private Material _compactBone = null!;
    private ArmourType _bodypartNaturalArmour = null!;
    private ArmourType _cranialNaturalArmour = null!;
    private ArmourType _organArmour = null!;
    private ArmourType _racialNaturalArmour = null!;

    private Material _spongyBone = null!;
    private Material _visceraMaterial = null!;

	private enum StockHumanWearProfileType
	{
		Direct,
		Shape
	}

	private sealed record StockHumanWearProfileLocation(
		string Location,
		int Count,
		bool Mandatory,
		bool NoArmour,
		bool Transparent,
		bool PreventsRemoval,
		bool HidesSevered);

	private sealed record StockHumanWearProfileDefinition(
		string Name,
		string WearStringInventory,
		string WearAction1st,
		string WearAction3rd,
		string WearAffix,
		string Description,
		bool RequireContainerIsEmpty,
		bool Bulky,
		StockHumanWearProfileType Type,
		IReadOnlyList<StockHumanWearProfileLocation> Locations);

	private sealed record StockHumanWearComponentDefinition(
		string Name,
		string DefaultProfileName,
		bool Bulky,
		IReadOnlyList<string> ProfileNames);

	private static StockHumanWearProfileLocation Loc(string location, bool mandatory, bool noArmour,
		bool transparent, bool preventsRemoval, bool hidesSevered)
	{
		return new StockHumanWearProfileLocation(location, 1, mandatory, noArmour, transparent, preventsRemoval,
			hidesSevered);
	}

	private static StockHumanWearProfileLocation ShapeLoc(string shape, int count, bool mandatory, bool noArmour,
		bool transparent, bool preventsRemoval, bool hidesSevered)
	{
		return new StockHumanWearProfileLocation(shape, count, mandatory, noArmour, transparent, preventsRemoval,
			hidesSevered);
	}

	private static StockHumanWearProfileDefinition StockDirectWearProfile(string name, string wearInv,
		string wear1st, string wear3rd, string wearAffix, string description, bool requireContainerIsEmpty,
		bool bulky, params StockHumanWearProfileLocation[] locations)
	{
		return new StockHumanWearProfileDefinition(name, wearInv, wear1st, wear3rd, wearAffix, description,
			requireContainerIsEmpty, bulky, StockHumanWearProfileType.Direct, locations);
	}

	private static StockHumanWearProfileDefinition StockShapeWearProfile(string name, string wearInv,
		string wear1st, string wear3rd, string wearAffix, string description, bool requireContainerIsEmpty,
		bool bulky, params StockHumanWearProfileLocation[] locations)
	{
		return new StockHumanWearProfileDefinition(name, wearInv, wear1st, wear3rd, wearAffix, description,
			requireContainerIsEmpty, bulky, StockHumanWearProfileType.Shape, locations);
	}

	private static readonly StockHumanWearProfileDefinition[] AdditionalHumanWearProfiles =
	[
		StockDirectWearProfile("Headband", "worn on", "put", "puts", "on",
			"Worn as a band around the forehead and temples", false, false,
			Loc("forehead", true, false, false, true, false),
			Loc("rtemple", true, false, false, true, false),
			Loc("ltemple", true, false, false, true, false)),
		StockShapeWearProfile("Brooch", "pinned to", "pin", "pins", "to",
			"Worn as a single decorative brooch, badge-like fastening or cloak ornament", false, false,
			ShapeLoc("breast", 1, true, false, true, true, false),
			ShapeLoc("shoulder", 1, false, false, true, true, false)),
		StockShapeWearProfile("Brooches", "pinned to", "pin", "pins", "to",
			"Worn as a matched pair of decorative brooches or garment ornaments", false, false,
			ShapeLoc("shoulder", 2, true, false, true, true, false),
			ShapeLoc("breast", 2, false, false, true, true, false)),
		StockShapeWearProfile("Pin", "pinned to", "pin", "pins", "to",
			"Worn as a decorative pin or visible garment-fastening ornament", false, false,
			ShapeLoc("breast", 1, true, false, true, true, false),
			ShapeLoc("shoulder", 1, false, false, true, true, false)),
		StockShapeWearProfile("Badge", "affixed to", "affix", "affixes", "to",
			"Worn as a decorative secular, civic, guild or household badge", false, false,
			ShapeLoc("breast", 1, true, false, true, true, false),
			ShapeLoc("shoulder", 1, false, false, true, true, false)),
		StockDirectWearProfile("Hairpin", "worn in", "place", "places", "in",
			"Worn as a single decorative hairpin or hair stick", false, false,
			Loc("scalp", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false),
			Loc("rtemple", false, false, true, true, false),
			Loc("ltemple", false, false, true, true, false)),
		StockDirectWearProfile("Hairpins", "worn in", "place", "places", "in",
			"Worn as a matched pair or set of decorative hairpins", false, false,
			Loc("scalp", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false),
			Loc("rtemple", false, false, true, true, false),
			Loc("ltemple", false, false, true, true, false)),
		StockDirectWearProfile("Hair Comb", "worn in", "place", "places", "in",
			"Worn as a single comb-backed decorative hair ornament", false, false,
			Loc("scalp", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false)),
		StockDirectWearProfile("Hair Combs", "worn in", "place", "places", "in",
			"Worn as a matched pair or set of comb-backed decorative hair ornaments", false, false,
			Loc("scalp", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false),
			Loc("rtemple", false, false, true, true, false),
			Loc("ltemple", false, false, true, true, false)),
		StockDirectWearProfile("Hair Ornament", "worn in", "place", "places", "in",
			"Worn as a generic decorative hair plaque, ring or hard-to-classify hair ornament", false, false,
			Loc("scalp", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false),
			Loc("rtemple", false, false, true, true, false),
			Loc("ltemple", false, false, true, true, false)),
		StockDirectWearProfile("Hair Ornaments", "worn in", "place", "places", "in",
			"Worn as a matched pair or set of generic decorative hair ornaments", false, false,
			Loc("scalp", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false),
			Loc("rtemple", false, false, true, true, false),
			Loc("ltemple", false, false, true, true, false)),
		StockDirectWearProfile("Temple Rings", "worn at", "place", "places", "at",
			"Worn as paired ornaments at the temples or suspended near the sides of the head", false, false,
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false)),
		StockDirectWearProfile("Circlet", "worn on", "place", "places", "on",
			"Worn as light head jewellery around the forehead and temples", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false)),
		StockDirectWearProfile("Diadem", "worn on", "place", "places", "on",
			"Worn as narrow elite head or forehead jewellery", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false)),
		StockDirectWearProfile("Coronet", "worn on", "place", "places", "on",
			"Worn as lesser noble head regalia rather than a full crown", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false),
			Loc("scalp", false, false, true, true, false)),
		StockDirectWearProfile("Crown", "worn on", "place", "places", "on",
			"Worn as true crown or royal regalia head jewellery", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false),
			Loc("scalp", true, false, true, true, false)),
		StockDirectWearProfile("Chaplet", "worn on", "place", "places", "on",
			"Worn as a ribbon, flower or simple festival chaplet around the head", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false)),
		StockDirectWearProfile("Wreath", "worn on", "place", "places", "on",
			"Worn as a leaf, laurel, ivy, flower or dried wreath around the head", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false)),
		StockDirectWearProfile("Head Garland", "worn on", "place", "places", "on",
			"Worn as a head-worn flower, blossom or festival garland", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", true, false, true, true, false),
			Loc("ltemple", true, false, true, true, false),
			Loc("bhead", false, false, true, true, false)),
		StockDirectWearProfile("Forehead Ornament", "worn on", "place", "places", "on",
			"Worn as a forehead pendant, brow ornament or regional head-jewellery piece", false, false,
			Loc("forehead", true, false, true, true, false),
			Loc("rtemple", false, false, true, true, false),
			Loc("ltemple", false, false, true, true, false)),
		StockDirectWearProfile("Neck Garland", "worn around", "place", "places", "around",
			"Worn as a neck-worn flower, blossom or festival garland", false, false,
			Loc("neck", true, false, true, true, false),
			Loc("bneck", true, false, true, true, false),
			Loc("throat", false, false, true, true, false)),
		StockDirectWearProfile("Torc", "worn around", "place", "places", "around",
			"Worn as a rigid torc or torc-like neck ring", false, false,
			Loc("neck", true, false, true, true, false),
			Loc("bneck", true, false, true, true, false),
			Loc("throat", false, false, true, true, false)),
		StockDirectWearProfile("Neck Ring", "worn around", "place", "places", "around",
			"Worn as a rigid neck ring or collar-like metal neck ornament", false, false,
			Loc("neck", true, false, true, true, false),
			Loc("bneck", true, false, true, true, false),
			Loc("throat", false, false, true, true, false)),
		StockShapeWearProfile("Wrist Garland", "worn around", "place", "places", "around",
			"Worn as a flower, herb or festival garland around one wrist", false, false,
			ShapeLoc("wrist", 1, true, false, true, true, false)),
		StockShapeWearProfile("Ankle Garland", "worn around", "place", "places", "around",
			"Worn as a blossom, flower or festival garland around one ankle", false, false,
			ShapeLoc("ankle", 1, true, false, true, true, false)),
		StockShapeWearProfile("Waist Chain", "worn around", "place", "places", "around",
			"Worn as a decorative waist chain without functional belt attachments", false, false,
			ShapeLoc("hip", 2, true, false, true, true, false),
			ShapeLoc("abdomen", 1, false, false, true, true, false)),
		StockShapeWearProfile("Girdle Ornament", "worn at", "place", "places", "at",
			"Worn as dangling girdle jewellery, girdle jewels or decorative girdle mounts", false, false,
			ShapeLoc("hip", 2, true, false, true, true, false),
			ShapeLoc("abdomen", 1, false, false, true, true, false)),
		StockShapeWearProfile("Belt Ornament", "worn at", "place", "places", "at",
			"Worn as a single decorative belt-mounted ornament or belt jewel", false, false,
			ShapeLoc("hip", 1, true, false, true, true, false),
			ShapeLoc("abdomen", 1, false, false, true, true, false)),
		StockShapeWearProfile("Belt Plaques", "worn at", "place", "places", "at",
			"Worn as a row or matched set of decorative belt plaques", false, false,
			ShapeLoc("hip", 2, true, false, true, true, false),
			ShapeLoc("abdomen", 1, false, false, true, true, false)),
		StockShapeWearProfile("Waist Ornament", "worn at", "place", "places", "at",
			"Worn as generic waist jewellery that is not a chain, belt plaque or girdle jewel", false, false,
			ShapeLoc("hip", 1, true, false, true, true, false),
			ShapeLoc("abdomen", 1, false, false, true, true, false)),
		StockDirectWearProfile("Turban", "worn on", "put", "puts", "on",
			"Worn as wrapped headcloth covering the scalp, back of head and forehead", false, false,
			Loc("scalp", true, false, false, true, false),
			Loc("bhead", true, false, false, true, false),
			Loc("forehead", true, false, false, true, false),
			Loc("rtemple", false, false, false, true, false),
			Loc("ltemple", false, false, false, true, false)),
		StockDirectWearProfile("Veil", "worn over", "put", "puts", "on",
			"Worn as a sheer veil over the face while leaving features visible", false, false,
			Loc("face", true, false, true, true, false),
			Loc("forehead", true, false, true, true, false),
			Loc("rcheek", true, false, true, true, false),
			Loc("lcheek", true, false, true, true, false),
			Loc("nose", false, false, true, true, false),
			Loc("mouth", false, false, true, true, false),
			Loc("chin", false, false, true, true, false),
			Loc("reyesocket", false, false, true, true, false),
			Loc("leyesocket", false, false, true, true, false)),
		StockDirectWearProfile("Blindfold", "worn over", "tie", "ties", "on",
			"Worn as an opaque covering across the eyes", false, false,
			Loc("reyesocket", true, false, false, true, true),
			Loc("leyesocket", true, false, false, true, true),
			Loc("reye", false, false, false, true, true),
			Loc("leye", false, false, false, true, true),
			Loc("rbrow", false, false, false, true, false),
			Loc("lbrow", false, false, false, true, false)),
		StockDirectWearProfile("Gag", "worn in", "put", "puts", "in",
			"Worn as a gag secured in or over the mouth", false, false,
			Loc("mouth", true, false, false, true, true),
			Loc("tongue", false, false, false, true, true),
			Loc("chin", false, false, false, true, false),
			Loc("rcheek", false, false, false, true, false),
			Loc("lcheek", false, false, false, true, false)),
		StockDirectWearProfile("Long Coat", "worn on", "put", "puts", "on",
			"Worn as a long coat covering the torso, arms and upper legs", false, true,
			Loc("lback", true, false, false, true, false),
			Loc("uback", true, false, false, true, false),
			Loc("belly", true, false, false, true, false),
			Loc("abdomen", true, false, false, true, false),
			Loc("rbreast", true, false, false, true, false),
			Loc("lbreast", true, false, false, true, false),
			Loc("rnipple", false, false, false, true, true),
			Loc("lnipple", false, false, false, true, true),
			Loc("rshoulder", true, false, false, true, false),
			Loc("lshoulder", true, false, false, true, false),
			Loc("rupperarm", false, false, false, true, false),
			Loc("lupperarm", false, false, false, true, false),
			Loc("relbow", false, false, false, true, false),
			Loc("lelbow", false, false, false, true, false),
			Loc("rforearm", false, false, false, true, false),
			Loc("lforearm", false, false, false, true, false),
			Loc("rbuttock", false, false, false, false, false),
			Loc("lbuttock", false, false, false, false, false),
			Loc("rthigh", false, false, false, false, false),
			Loc("lthigh", false, false, false, false, false),
			Loc("rthighback", false, false, false, false, false),
			Loc("lthighback", false, false, false, false, false)),
		StockDirectWearProfile("Robe", "worn on", "slip", "slips", "on",
			"Worn as a loose robe covering the torso, arms and legs", false, true,
			Loc("lback", true, false, false, true, false),
			Loc("uback", true, false, false, true, false),
			Loc("belly", true, false, false, true, false),
			Loc("abdomen", true, false, false, true, false),
			Loc("rbreast", true, false, false, true, false),
			Loc("lbreast", true, false, false, true, false),
			Loc("rnipple", false, false, false, true, true),
			Loc("lnipple", false, false, false, true, true),
			Loc("rshoulder", true, false, false, true, false),
			Loc("lshoulder", true, false, false, true, false),
			Loc("rupperarm", false, false, false, true, false),
			Loc("lupperarm", false, false, false, true, false),
			Loc("relbow", false, false, false, true, false),
			Loc("lelbow", false, false, false, true, false),
			Loc("rforearm", false, false, false, true, false),
			Loc("lforearm", false, false, false, true, false),
			Loc("groin", true, false, false, false, false),
			Loc("penis", false, false, false, false, true),
			Loc("testicles", false, false, false, false, true),
			Loc("rhip", true, false, false, false, false),
			Loc("lhip", true, false, false, false, false),
			Loc("rbuttock", false, false, false, false, false),
			Loc("lbuttock", false, false, false, false, false),
			Loc("rthigh", false, false, false, false, false),
			Loc("lthigh", false, false, false, false, false),
			Loc("rthighback", false, false, false, false, false),
			Loc("lthighback", false, false, false, false, false)),
		StockDirectWearProfile("Tabard", "worn over", "put", "puts", "on",
			"Worn as a sleeveless tabard over the chest and back", false, false,
			Loc("rshoulder", true, false, false, true, false),
			Loc("lshoulder", true, false, false, true, false),
			Loc("rbreast", true, false, false, true, false),
			Loc("lbreast", true, false, false, true, false),
			Loc("rnipple", false, false, false, true, true),
			Loc("lnipple", false, false, false, true, true),
			Loc("abdomen", true, false, false, true, false),
			Loc("belly", true, false, false, true, false),
			Loc("uback", true, false, false, true, false),
			Loc("lback", true, false, false, true, false)),
		StockDirectWearProfile("Apron", "worn over", "tie", "ties", "on",
			"Worn as an apron covering the front of the torso and upper legs", false, false,
			Loc("throat", false, false, true, true, false),
			Loc("rbreast", false, false, false, false, false),
			Loc("lbreast", false, false, false, false, false),
			Loc("rnipple", false, false, false, false, true),
			Loc("lnipple", false, false, false, false, true),
			Loc("abdomen", true, false, false, false, false),
			Loc("belly", true, false, false, false, false),
			Loc("groin", false, false, false, false, false),
			Loc("rthigh", false, false, false, false, false),
			Loc("lthigh", false, false, false, false, false)),
		StockDirectWearProfile("Poncho", "worn over", "pull", "pulls", "on",
			"Worn as a poncho draped over the shoulders, chest, back and upper arms", false, true,
			Loc("neck", true, false, false, true, false),
			Loc("bneck", true, false, false, true, false),
			Loc("rshoulder", true, false, false, true, false),
			Loc("lshoulder", true, false, false, true, false),
			Loc("rbreast", true, false, false, false, false),
			Loc("lbreast", true, false, false, false, false),
			Loc("rnipple", false, false, false, false, true),
			Loc("lnipple", false, false, false, false, true),
			Loc("abdomen", false, false, false, false, false),
			Loc("uback", true, false, false, false, false),
			Loc("lback", false, false, false, false, false),
			Loc("rupperarm", false, false, false, false, false),
			Loc("lupperarm", false, false, false, false, false)),
		StockDirectWearProfile("Leggings", "worn on", "slip", "slips", "on",
			"Worn as close-fitting legwear from waist to ankles, leaving the feet bare", false, false,
			Loc("groin", true, false, false, true, false),
			Loc("rhip", true, false, false, true, false),
			Loc("lhip", true, false, false, true, false),
			Loc("penis", false, false, false, true, true),
			Loc("testicles", false, false, false, true, true),
			Loc("rbuttock", true, false, false, true, false),
			Loc("lbuttock", true, false, false, true, false),
			Loc("rthigh", true, false, false, true, false),
			Loc("lthigh", true, false, false, true, false),
			Loc("rthighback", true, false, false, true, false),
			Loc("lthighback", true, false, false, true, false),
			Loc("rknee", true, false, false, true, false),
			Loc("lknee", true, false, false, true, false),
			Loc("rkneeback", true, false, false, true, false),
			Loc("lkneeback", true, false, false, true, false),
			Loc("rshin", true, false, false, true, false),
			Loc("lshin", true, false, false, true, false),
			Loc("rcalf", true, false, false, true, false),
			Loc("lcalf", true, false, false, true, false),
			Loc("rankle", false, false, false, true, false),
			Loc("lankle", false, false, false, true, false)),
		StockDirectWearProfile("Tights", "worn on", "slip", "slips", "on",
			"Worn as close-fitting legwear covering the legs and feet", false, false,
			Loc("groin", true, false, false, true, false),
			Loc("rhip", true, false, false, true, false),
			Loc("lhip", true, false, false, true, false),
			Loc("penis", false, false, false, true, true),
			Loc("testicles", false, false, false, true, true),
			Loc("rbuttock", true, false, false, true, false),
			Loc("lbuttock", true, false, false, true, false),
			Loc("rthigh", true, false, false, true, false),
			Loc("lthigh", true, false, false, true, false),
			Loc("rthighback", true, false, false, true, false),
			Loc("lthighback", true, false, false, true, false),
			Loc("rknee", true, false, false, true, false),
			Loc("lknee", true, false, false, true, false),
			Loc("rkneeback", true, false, false, true, false),
			Loc("lkneeback", true, false, false, true, false),
			Loc("rshin", true, false, false, true, false),
			Loc("lshin", true, false, false, true, false),
			Loc("rcalf", true, false, false, true, false),
			Loc("lcalf", true, false, false, true, false),
			Loc("rankle", true, false, false, true, false),
			Loc("lankle", true, false, false, true, false),
			Loc("rfoot", true, false, false, true, false),
			Loc("lfoot", true, false, false, true, false),
			Loc("rbigtoe", false, false, false, true, true),
			Loc("lbigtoe", false, false, false, true, true),
			Loc("rindextoe", false, false, false, true, true),
			Loc("lindextoe", false, false, false, true, true),
			Loc("rmiddletoe", false, false, false, true, true),
			Loc("lmiddletoe", false, false, false, true, true),
			Loc("rringtoe", false, false, false, true, true),
			Loc("lringtoe", false, false, false, true, true),
			Loc("rpinkytoe", false, false, false, true, true),
			Loc("lpinkytoe", false, false, false, true, true)),
		StockDirectWearProfile("Stays", "worn beneath", "lace", "laces", "beneath",
			"Worn as a structured torso underlayer beneath bodices, gowns and coats", false, false,
			Loc("rbreast", true, true, false, false, false),
			Loc("lbreast", true, true, false, false, false),
			Loc("uback", true, true, false, false, false),
			Loc("abdomen", true, true, false, false, false),
			Loc("belly", true, true, false, false, false),
			Loc("lback", false, true, false, false, false)),
		StockDirectWearProfile("Breeches", "worn on", "pull", "pulls", "on",
			"Worn as joined lower-body clothing from the waist through the thighs, ending at or below the knee", false,
			false,
			Loc("groin", true, true, false, false, false),
			Loc("rhip", true, true, false, false, false),
			Loc("lhip", true, true, false, false, false),
			Loc("rbuttock", true, true, false, false, false),
			Loc("lbuttock", true, true, false, false, false),
			Loc("rthigh", true, true, false, false, false),
			Loc("lthigh", true, true, false, false, false),
			Loc("rthighback", false, true, false, false, false),
			Loc("lthighback", false, true, false, false, false),
			Loc("rknee", false, true, false, false, false),
			Loc("lknee", false, true, false, false, false)),
		StockDirectWearProfile("Loincloth", "worn on", "tie", "ties", "on",
			"Worn as a loincloth covering the groin and hanging from the hips", false, false,
			Loc("groin", true, false, false, true, false),
			Loc("rhip", true, false, true, true, false),
			Loc("lhip", true, false, true, true, false),
			Loc("penis", false, false, false, true, true),
			Loc("testicles", false, false, false, true, true),
			Loc("rbuttock", false, false, false, false, false),
			Loc("lbuttock", false, false, false, false, false)),
		StockDirectWearProfile("Fingerless Gloves", "worn on", "put", "puts", "on",
			"Worn as gloves covering the hands and wrists while leaving fingers exposed", false, true,
			Loc("rhand", true, false, false, true, false),
			Loc("lhand", true, false, false, true, false),
			Loc("rwrist", true, false, false, true, false),
			Loc("lwrist", true, false, false, true, false)),
		StockDirectWearProfile("Mittens", "worn on", "put", "puts", "on",
			"Worn as mittens covering the hands, thumbs and fingers", false, true,
			Loc("rhand", true, false, false, true, false),
			Loc("lhand", true, false, false, true, false),
			Loc("rthumb", false, false, false, true, true),
			Loc("lthumb", false, false, false, true, true),
			Loc("rindexfinger", false, false, false, true, true),
			Loc("lindexfinger", false, false, false, true, true),
			Loc("rmiddlefinger", false, false, false, true, true),
			Loc("lmiddlefinger", false, false, false, true, true),
			Loc("rringfinger", false, false, false, true, true),
			Loc("lringfinger", false, false, false, true, true),
			Loc("rpinkyfinger", false, false, false, true, true),
			Loc("lpinkyfinger", false, false, false, true, true)),
		StockDirectWearProfile("Backplate", "worn on", "put", "puts", "on",
			"Worn as a backplate covering the upper and lower back", false, true,
			Loc("uback", true, false, false, true, false),
			Loc("lback", true, false, false, true, false),
			Loc("rshoulderblade", true, false, false, true, false),
			Loc("lshoulderblade", true, false, false, true, false)),
		StockDirectWearProfile("Sabatons", "worn on", "put", "puts", "on",
			"Worn as armoured foot protection covering the feet and toes", false, true,
			Loc("rfoot", true, false, false, true, false),
			Loc("lfoot", true, false, false, true, false),
			Loc("rheel", false, false, false, true, false),
			Loc("lheel", false, false, false, true, false),
			Loc("rbigtoe", false, false, false, true, true),
			Loc("lbigtoe", false, false, false, true, true),
			Loc("rindextoe", false, false, false, true, true),
			Loc("lindextoe", false, false, false, true, true),
			Loc("rmiddletoe", false, false, false, true, true),
			Loc("lmiddletoe", false, false, false, true, true),
			Loc("rringtoe", false, false, false, true, true),
			Loc("lringtoe", false, false, false, true, true),
			Loc("rpinkytoe", false, false, false, true, true),
			Loc("lpinkytoe", false, false, false, true, true)),
		StockDirectWearProfile("Tassets", "worn on", "buckle", "buckles", "on",
			"Worn as articulated thigh defences hanging from the waist and hips", false, true,
			Loc("rhip", true, false, false, true, false),
			Loc("lhip", true, false, false, true, false),
			Loc("groin", false, false, false, true, false),
			Loc("rthigh", true, false, false, true, false),
			Loc("lthigh", true, false, false, true, false),
			Loc("rthighback", false, false, false, true, false),
			Loc("lthighback", false, false, false, true, false)),
		StockShapeWearProfile("Sash", "worn across", "put", "puts", "on",
			"Worn as a decorative sash crossing from one shoulder to the opposite hip", false, false,
			ShapeLoc("shoulder", 1, true, false, true, true, false),
			ShapeLoc("breast", 1, false, false, true, true, false),
			ShapeLoc("hip", 1, true, false, true, true, false)),
		StockShapeWearProfile("Bandolier", "slung across", "sling", "slings", "on",
			"Slung as a bandolier across one shoulder and the torso", false, false,
			ShapeLoc("shoulder", 1, true, false, true, true, false),
			ShapeLoc("breast", 1, false, false, true, true, false),
			ShapeLoc("hip", 1, false, false, true, true, false)),
		StockShapeWearProfile("Armlet", "worn on", "put", "puts", "on", "Worn as a single armlet on an upper arm",
			false, false,
			ShapeLoc("upper arm", 1, true, false, true, true, false)),
		StockShapeWearProfile("Toe Ring", "worn on", "put", "puts", "on", "Worn as a ring on a toe", false, false,
			ShapeLoc("toe", 1, true, false, true, true, false)),
		StockDirectWearProfile("Leg Wraps", "wrapped around", "wrap", "wraps", "around",
			"Worn as paired cloth wraps around the lower legs", false, false,
			Loc("rshin", true, true, false, true, false),
			Loc("lshin", true, true, false, true, false),
			Loc("rcalf", true, true, false, true, false),
			Loc("lcalf", true, true, false, true, false),
			Loc("rankle", false, true, false, true, false),
			Loc("lankle", false, true, false, true, false)),
		StockDirectWearProfile("Overshoes", "worn over", "put", "puts", "over",
			"Worn as protective overshoes above ordinary footwear", false, true,
			Loc("rfoot", true, true, false, true, false),
			Loc("lfoot", true, true, false, true, false),
			Loc("rheel", true, true, false, true, false),
			Loc("lheel", true, true, false, true, false),
			Loc("rbigtoe", false, true, false, true, true),
			Loc("lbigtoe", false, true, false, true, true),
			Loc("rindextoe", false, true, false, true, true),
			Loc("lindextoe", false, true, false, true, true),
			Loc("rmiddletoe", false, true, false, true, true),
			Loc("lmiddletoe", false, true, false, true, true),
			Loc("rringtoe", false, true, false, true, true),
			Loc("lringtoe", false, true, false, true, true),
			Loc("rpinkytoe", false, true, false, true, true),
			Loc("lpinkytoe", false, true, false, true, true)),
		StockDirectWearProfile("Head Veil", "draped from", "drape", "drapes", "from",
			"Worn as a veil draped from the head without covering the face", false, false,
			Loc("scalp", true, true, true, false, false),
			Loc("bhead", true, true, false, false, false),
			Loc("forehead", false, true, true, false, false),
			Loc("rear", false, true, true, false, false),
			Loc("lear", false, true, true, false, false),
			Loc("bneck", false, true, true, false, false)),
		StockDirectWearProfile("Hood", "worn over", "pull", "pulls", "over",
			"Worn as a separate hood covering the head and neck while leaving the face open", false, true,
			Loc("scalp", true, true, false, true, false),
			Loc("bhead", true, true, false, true, false),
			Loc("forehead", false, true, false, true, false),
			Loc("rear", false, true, false, true, false),
			Loc("lear", false, true, false, true, false),
			Loc("neck", true, true, false, true, false),
			Loc("bneck", true, true, false, true, false)),
		StockDirectWearProfile("Detachable Sleeves", "laced onto", "lace", "laces", "onto",
			"Worn as paired detachable sleeves laced to another upper-body garment", false, false,
			Loc("rshoulder", true, true, true, false, false),
			Loc("lshoulder", true, true, true, false, false),
			Loc("rupperarm", true, true, false, false, false),
			Loc("lupperarm", true, true, false, false, false),
			Loc("relbow", true, true, false, false, false),
			Loc("lelbow", true, true, false, false, false),
			Loc("rforearm", true, true, false, false, false),
			Loc("lforearm", true, true, false, false, false),
			Loc("rwrist", false, true, false, false, false),
			Loc("lwrist", false, true, false, false, false)),
		StockDirectWearProfile("Skirt Support", "worn beneath", "fasten", "fastens", "beneath",
			"Worn as a structured support beneath a skirt or gown", false, true,
			Loc("abdomen", true, true, true, false, false),
			Loc("belly", true, true, true, false, false),
			Loc("rhip", true, true, true, false, false),
			Loc("lhip", true, true, true, false, false),
			Loc("rbuttock", false, true, true, false, false),
			Loc("lbuttock", false, true, true, false, false),
			Loc("rthigh", false, true, true, false, false),
			Loc("lthigh", false, true, true, false, false)),
		StockDirectWearProfile("Partlet", "worn over", "put", "puts", "over",
			"Worn as a close upper-chest and neck covering above a bodice or gown", false, false,
			Loc("throat", true, true, false, false, false),
			Loc("neck", true, true, false, false, false),
			Loc("bneck", true, true, false, false, false),
			Loc("rshoulder", false, true, false, false, false),
			Loc("lshoulder", false, true, false, false, false),
			Loc("rbreast", false, true, false, false, false),
			Loc("lbreast", false, true, false, false, false),
			Loc("uback", false, true, false, false, false)),
		StockDirectWearProfile("Long Open Robe", "worn open over", "slip", "slips", "over",
			"Worn as a long front-opening robe layered above other clothing", false, true,
			Loc("uback", true, true, false, true, false),
			Loc("lback", true, true, false, true, false),
			Loc("rshoulder", true, true, false, true, false),
			Loc("lshoulder", true, true, false, true, false),
			Loc("rbreast", false, true, true, false, false),
			Loc("lbreast", false, true, true, false, false),
			Loc("abdomen", false, true, true, false, false),
			Loc("belly", false, true, true, false, false),
			Loc("rupperarm", false, true, false, true, false),
			Loc("lupperarm", false, true, false, true, false),
			Loc("relbow", false, true, false, true, false),
			Loc("lelbow", false, true, false, true, false),
			Loc("rforearm", false, true, false, true, false),
			Loc("lforearm", false, true, false, true, false),
			Loc("rhip", false, true, true, false, false),
			Loc("lhip", false, true, true, false, false),
			Loc("rthigh", false, true, true, false, false),
			Loc("lthigh", false, true, true, false, false),
			Loc("rthighback", false, true, false, false, false),
			Loc("lthighback", false, true, false, false, false)),
		StockDirectWearProfile("ArmHarness", "worn on", "buckle", "buckles", "on",
			"Worn as a combined plate harness over both arms without torso coverage", false, true,
			Loc("rupperarm", true, false, false, true, false),
			Loc("lupperarm", true, false, false, true, false),
			Loc("relbow", true, false, false, true, false),
			Loc("lelbow", true, false, false, true, false),
			Loc("rforearm", true, false, false, true, false),
			Loc("lforearm", true, false, false, true, false)),
		StockDirectWearProfile("LegHarness", "worn on", "buckle", "buckles", "on",
			"Worn as a combined plate harness over both legs without torso coverage", false, true,
			Loc("rthigh", true, false, false, true, false),
			Loc("lthigh", true, false, false, true, false),
			Loc("rknee", true, false, false, true, false),
			Loc("lknee", true, false, false, true, false),
			Loc("rshin", true, false, false, true, false),
			Loc("lshin", true, false, false, true, false),
			Loc("rfoot", false, false, false, true, false),
			Loc("lfoot", false, false, false, true, false)),
		StockDirectWearProfile("ShoulderArmHarness", "worn on", "buckle", "buckles", "on",
			"Worn as reinforced shoulder and complete arm plate harness", false, true,
			Loc("rshoulder", true, false, false, true, false),
			Loc("lshoulder", true, false, false, true, false),
			Loc("rupperarm", true, false, false, true, false),
			Loc("lupperarm", true, false, false, true, false),
			Loc("relbow", true, false, false, true, false),
			Loc("lelbow", true, false, false, true, false),
			Loc("rforearm", true, false, false, true, false),
			Loc("lforearm", true, false, false, true, false)),
		StockDirectWearProfile("HalfArmourHarness", "worn over", "buckle", "buckles", "over",
			"Worn as half armour covering the torso, shoulders, upper arms and hips", false, true,
			Loc("rbreast", true, false, false, true, false),
			Loc("lbreast", true, false, false, true, false),
			Loc("uback", true, false, false, true, false),
			Loc("abdomen", true, false, false, true, false),
			Loc("rshoulder", true, false, false, true, false),
			Loc("lshoulder", true, false, false, true, false),
			Loc("rupperarm", false, false, false, true, false),
			Loc("lupperarm", false, false, false, true, false),
			Loc("rhip", false, false, false, true, false),
			Loc("lhip", false, false, false, true, false)),
		StockDirectWearProfile("ThreeQuarterHarness", "worn over", "buckle", "buckles", "over",
			"Worn as three-quarter armour covering the torso, arms, hands, hips and thighs", false, true,
			Loc("rbreast", true, false, false, true, false),
			Loc("lbreast", true, false, false, true, false),
			Loc("uback", true, false, false, true, false),
			Loc("abdomen", true, false, false, true, false),
			Loc("rupperarm", true, false, false, true, false),
			Loc("lupperarm", true, false, false, true, false),
			Loc("relbow", true, false, false, true, false),
			Loc("lelbow", true, false, false, true, false),
			Loc("rforearm", true, false, false, true, false),
			Loc("lforearm", true, false, false, true, false),
			Loc("rhand", false, false, false, true, false),
			Loc("lhand", false, false, false, true, false),
			Loc("rhip", true, false, false, true, false),
			Loc("lhip", true, false, false, true, false),
			Loc("rthigh", true, false, false, true, false),
			Loc("lthigh", true, false, false, true, false),
			Loc("rknee", false, false, false, true, false),
			Loc("lknee", false, false, false, true, false)),
		StockDirectWearProfile("FullPlateHarness", "worn over", "buckle", "buckles", "over",
			"Worn as a full plate harness covering the torso, limbs, hands and feet", false, true,
			Loc("rbreast", true, false, false, true, false),
			Loc("lbreast", true, false, false, true, false),
			Loc("uback", true, false, false, true, false),
			Loc("lback", true, false, false, true, false),
			Loc("abdomen", true, false, false, true, false),
			Loc("belly", true, false, false, true, false),
			Loc("rshoulder", true, false, false, true, false),
			Loc("lshoulder", true, false, false, true, false),
			Loc("rupperarm", true, false, false, true, false),
			Loc("lupperarm", true, false, false, true, false),
			Loc("relbow", true, false, false, true, false),
			Loc("lelbow", true, false, false, true, false),
			Loc("rforearm", true, false, false, true, false),
			Loc("lforearm", true, false, false, true, false),
			Loc("rhand", false, false, false, true, false),
			Loc("lhand", false, false, false, true, false),
			Loc("rhip", true, false, false, true, false),
			Loc("lhip", true, false, false, true, false),
			Loc("rthigh", true, false, false, true, false),
			Loc("lthigh", true, false, false, true, false),
			Loc("rknee", true, false, false, true, false),
			Loc("lknee", true, false, false, true, false),
			Loc("rshin", true, false, false, true, false),
			Loc("lshin", true, false, false, true, false),
			Loc("rfoot", false, false, false, true, false),
			Loc("lfoot", false, false, false, true, false)),
		StockDirectWearProfile("Breechcloth", "wrapped around", "wrap", "wraps", "around",
			"Worn as a wrapped breechcloth covering the groin and passing between the legs", false, false,
			Loc("groin", true, true, false, true, false),
			Loc("rhip", true, true, true, true, false),
			Loc("lhip", true, true, true, true, false),
			Loc("penis", false, true, false, true, true),
			Loc("testicles", false, true, false, true, true),
			Loc("rbuttock", false, true, false, false, false),
			Loc("lbuttock", false, true, false, false, false))
	];

	private static readonly StockHumanWearComponentDefinition[] AdditionalHumanWearComponents =
	[
		new("Wear_Bandana", "Headband", false, ["Headband", "Kerchief", "Armlet"])
	];

	internal static int HumanWearProfileBaselineCountForTesting => 130;

	internal static int HumanWearProfileExpansionCountForTesting => AdditionalHumanWearProfiles.Length;

	internal static IReadOnlyList<string> AdditionalHumanWearProfileNamesForTesting =>
		AdditionalHumanWearProfiles.Select(x => x.Name).ToArray();

	internal static IReadOnlyList<(string Name, string Type, IReadOnlyList<(string Location, int Count)> Locations)>
		AdditionalHumanWearProfileDefinitionsForTesting =>
		AdditionalHumanWearProfiles
			.Select(x => (
				x.Name,
				x.Type == StockHumanWearProfileType.Direct ? "Direct" : "Shape",
				(IReadOnlyList<(string Location, int Count)>)x.Locations
				                                               .Select(y => (y.Location, y.Count))
				                                               .ToArray()))
			.ToArray();

	internal static IReadOnlyList<(string Name, bool Bulky,
		IReadOnlyList<(string Location, bool Mandatory, bool NoArmour)> Locations)>
		AdditionalHumanWearProfileLayeringForTesting =>
		AdditionalHumanWearProfiles
			.Select(x => (
				x.Name,
				x.Bulky,
				(IReadOnlyList<(string Location, bool Mandatory, bool NoArmour)>)x.Locations
					.Select(y => (y.Location, y.Mandatory, y.NoArmour))
					.ToArray()))
			.ToArray();

	internal static IReadOnlyList<(string Name, string DefaultProfileName, IReadOnlyList<string> ProfileNames)>
		AdditionalHumanWearComponentDefinitionsForTesting =>
		AdditionalHumanWearComponents
			.Select(x => (x.Name, x.DefaultProfileName, x.ProfileNames))
			.ToArray();

	internal static IReadOnlyList<string> ValidateAdditionalHumanWearProfilesForTesting()
	{
		List<string> issues = new();
		HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);

		foreach (StockHumanWearProfileDefinition definition in AdditionalHumanWearProfiles)
		{
			if (string.IsNullOrWhiteSpace(definition.Name))
			{
				issues.Add("A wear profile has a blank name.");
			}

			if (!names.Add(definition.Name))
			{
				issues.Add($"Duplicate additional wear profile name {definition.Name}.");
			}

			if (definition.Locations.Count == 0)
			{
				issues.Add($"{definition.Name} has no wear locations.");
			}

			foreach (StockHumanWearProfileLocation location in definition.Locations)
			{
				if (string.IsNullOrWhiteSpace(location.Location))
				{
					issues.Add($"{definition.Name} has a blank wear location.");
				}

				if (location.Count < 1)
				{
					issues.Add($"{definition.Name} has an invalid count for {location.Location}.");
				}

				if (definition.Type == StockHumanWearProfileType.Direct && location.Count != 1)
				{
					issues.Add($"{definition.Name} direct location {location.Location} should use a count of 1.");
				}
			}
		}

		return issues;
	}

	internal static IReadOnlyList<string> ValidateAdditionalHumanWearComponentsForTesting()
	{
		List<string> issues = new();
		HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
		foreach (StockHumanWearComponentDefinition definition in AdditionalHumanWearComponents)
		{
			if (string.IsNullOrWhiteSpace(definition.Name))
			{
				issues.Add("A wear component has a blank name.");
			}

			if (!names.Add(definition.Name))
			{
				issues.Add($"Duplicate additional wear component name {definition.Name}.");
			}

			if (definition.ProfileNames.Count == 0)
			{
				issues.Add($"{definition.Name} has no profile names.");
			}

			if (string.IsNullOrWhiteSpace(definition.DefaultProfileName))
			{
				issues.Add($"{definition.Name} has a blank default profile.");
			}
			else if (!definition.ProfileNames.Contains(definition.DefaultProfileName, StringComparer.OrdinalIgnoreCase))
			{
				issues.Add($"{definition.Name} default profile {definition.DefaultProfileName} is not included in its profile list.");
			}

			foreach (string profileName in definition.ProfileNames)
			{
				if (string.IsNullOrWhiteSpace(profileName))
				{
					issues.Add($"{definition.Name} has a blank profile reference.");
				}
			}
		}

		return issues;
	}

	private static string WearComponentName(string profileName)
	{
		return $"Wear_{profileName.Replace(' ', '_')}";
	}

	private static string BuildWearlocProfileXml(StockHumanWearProfileDefinition definition)
	{
		if (definition.Type == StockHumanWearProfileType.Direct)
		{
			return new XElement("Profiles",
				from location in definition.Locations
				select new XElement("Profile",
					new XAttribute("Bodypart", location.Location),
					new XAttribute("Transparent", location.Transparent),
					new XAttribute("NoArmour", location.NoArmour),
					new XAttribute("PreventsRemoval", location.PreventsRemoval),
					new XAttribute("Mandatory", location.Mandatory),
					new XAttribute("HidesSevered", location.HidesSevered)
				)
			).ToString();
		}

		return new XElement("Profiles",
			from location in definition.Locations
			select new XElement("Shape",
				new XAttribute("ShapeId", location.Location),
				new XAttribute("Count", location.Count),
				new XAttribute("Transparent", location.Transparent),
				new XAttribute("NoArmour", location.NoArmour),
				new XAttribute("PreventsRemoval", location.PreventsRemoval),
				new XAttribute("Mandatory", location.Mandatory),
				new XAttribute("HidesSevered", location.HidesSevered)
			)
		).ToString();
	}

	private List<(WearProfile Profile, bool Bulky)> AddMissingHumanWearProfiles(BodyProto baseHumanoid,
		IEnumerable<StockHumanWearProfileDefinition> definitions)
	{
		HashSet<string> existingNames = _context.WearProfiles
		                                        .Where(x => x.BodyPrototypeId == baseHumanoid.Id)
		                                        .Select(x => x.Name)
		                                        .AsEnumerable()
		                                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
		long nextId = _context.WearProfiles
		                      .Select(x => x.Id)
		                      .AsEnumerable()
		                      .DefaultIfEmpty(0L)
		                      .Max() + 1;
		List<(WearProfile Profile, bool Bulky)> addedProfiles = new();

		foreach (StockHumanWearProfileDefinition definition in definitions)
		{
			if (!existingNames.Add(definition.Name))
			{
				continue;
			}

			WearProfile profile = new()
			{
				Id = nextId++,
				BodyPrototypeId = baseHumanoid.Id,
				Name = definition.Name,
				WearStringInventory = definition.WearStringInventory,
				WearAction1st = definition.WearAction1st,
				WearAction3rd = definition.WearAction3rd,
				WearAffix = definition.WearAffix,
				Description = definition.Description,
				RequireContainerIsEmpty = definition.RequireContainerIsEmpty,
				Type = definition.Type == StockHumanWearProfileType.Direct ? "Direct" : "Shape",
				WearlocProfiles = BuildWearlocProfileXml(definition)
			};
			_context.WearProfiles.Add(profile);
			addedProfiles.Add((profile, definition.Bulky));
		}

		if (addedProfiles.Count == 0)
		{
			return addedProfiles;
		}

		_context.SaveChanges();
		AddWearableComponentProtosForProfiles(addedProfiles);
		_context.SaveChanges();
		return addedProfiles;
	}

	private void AddWearableComponentProtosForProfiles(IReadOnlyList<(WearProfile Profile, bool Bulky)> profiles)
	{
		if (profiles.Count == 0)
		{
			return;
		}

		DateTime now = DateTime.UtcNow;
		Account dbaccount = _context.Accounts.First();
		long id = _context.GameItemComponentProtos
		                  .Select(x => x.Id)
		                  .AsEnumerable()
		                  .DefaultIfEmpty(0L)
		                  .Max() + 1;
		HashSet<string> existingComponentNames = _context.GameItemComponentProtos
		                                                 .Select(x => x.Name)
		                                                 .AsEnumerable()
		                                                 .ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach ((WearProfile profile, bool bulky) in profiles)
		{
			string componentName = WearComponentName(profile.Name);
			if (!existingComponentNames.Add(componentName))
			{
				continue;
			}

			GameItemComponentProto component = new()
			{
				Id = id++,
				RevisionNumber = 0,
				Name = componentName,
				Description = $"Permits the item to be worn in the {profile.Name} wear configuration",
				Type = "Wearable",
				Definition =
					$"<Definition DisplayInventoryWhenWorn=\"true\" Bulky=\"{bulky}\"><Profiles Default=\"{profile.Id}\"><Profile>{profile.Id}</Profile></Profiles></Definition>"
			};
			component.EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = dbaccount.Id,
				BuilderDate = now,
				BuilderComment = "Auto-generated by the system",
				ReviewerAccountId = dbaccount.Id,
				ReviewerComment = "Auto-generated by the system",
				ReviewerDate = now
			};
			_context.GameItemComponentProtos.Add(component);
		}
	}

	private bool EnsureAdditionalHumanWearComponents(BodyProto baseHumanoid)
	{
		Dictionary<string, WearProfile> profiles = _context.WearProfiles
		                                                   .Where(x => x.BodyPrototypeId == baseHumanoid.Id)
		                                                   .AsEnumerable()
		                                                   .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
		                                                   .ToDictionary(x => x.Key, x => x.First(),
			                                                   StringComparer.OrdinalIgnoreCase);
		Dictionary<string, GameItemComponentProto> components = _context.GameItemComponentProtos
		                                                                 .AsEnumerable()
		                                                                 .Where(x => !string.IsNullOrWhiteSpace(x.Name))
		                                                                 .GroupBy(x => x.Name,
			                                                                 StringComparer.OrdinalIgnoreCase)
		                                                                 .ToDictionary(x => x.Key, x => x.First(),
			                                                                 StringComparer.OrdinalIgnoreCase);
		Account dbaccount = _context.Accounts.First();
		DateTime now = DateTime.UtcNow;
		long id = _context.GameItemComponentProtos
		                  .Select(x => x.Id)
		                  .AsEnumerable()
		                  .DefaultIfEmpty(0L)
		                  .Max() + 1;
		bool dirty = false;

		foreach (StockHumanWearComponentDefinition definition in AdditionalHumanWearComponents)
		{
			if (!profiles.TryGetValue(definition.DefaultProfileName, out WearProfile? defaultProfile))
			{
				continue;
			}

			List<WearProfile> componentProfiles = [];
			bool missingProfile = false;
			foreach (string profileName in definition.ProfileNames)
			{
				if (!profiles.TryGetValue(profileName, out WearProfile? profile))
				{
					missingProfile = true;
					break;
				}

				componentProfiles.Add(profile);
			}

			if (missingProfile)
			{
				continue;
			}

			string componentDefinition = BuildWearableComponentDefinition(
				defaultProfile,
				componentProfiles,
				definition.Bulky);
			string description = $"Permits the item to be worn as {definition.ProfileNames.ListToCommaSeparatedValues(" or ")}";

			if (components.TryGetValue(definition.Name, out GameItemComponentProto? component))
			{
				if (!component.Type.Equals("Wearable", StringComparison.Ordinal) ||
				    component.Description != description ||
				    !XmlEquivalent(component.Definition, componentDefinition))
				{
					component.Type = "Wearable";
					component.Description = description;
					component.Definition = componentDefinition;
					dirty = true;
				}

				continue;
			}

			component = new GameItemComponentProto
			{
				Id = id++,
				RevisionNumber = 0,
				Name = definition.Name,
				Description = description,
				Type = "Wearable",
				Definition = componentDefinition,
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = 4,
					BuilderAccountId = dbaccount.Id,
					BuilderDate = now,
					BuilderComment = "Auto-generated by the system",
					ReviewerAccountId = dbaccount.Id,
					ReviewerComment = "Auto-generated by the system",
					ReviewerDate = now
				}
			};
			_context.GameItemComponentProtos.Add(component);
			components[definition.Name] = component;
			dirty = true;
		}

		if (dirty)
		{
			_context.SaveChanges();
		}

		return dirty;
	}

	private static string BuildWearableComponentDefinition(
		WearProfile defaultProfile,
		IEnumerable<WearProfile> profiles,
		bool bulky)
	{
		return new XElement("Definition",
			new XAttribute("DisplayInventoryWhenWorn", true),
			new XAttribute("Bulky", bulky),
			new XElement("Profiles",
				new XAttribute("Default", defaultProfile.Id),
				profiles.Select(x => new XElement("Profile", x.Id)))
		).ToString();
	}

	private static bool XmlEquivalent(string? lhs, string rhs)
	{
		if (string.IsNullOrWhiteSpace(lhs))
		{
			return false;
		}

		try
		{
			return XNode.DeepEquals(XElement.Parse(lhs), XElement.Parse(rhs));
		}
		catch
		{
			return false;
		}
	}

	private bool RefreshExistingHumanWearProfiles(BodyProto baseHumanoid)
	{
		bool repaired = RepairExistingHumanWearProfileAccuracy(baseHumanoid);
		bool added = AddMissingHumanWearProfiles(baseHumanoid, AdditionalHumanWearProfiles).Count > 0;
		bool addedComponents = EnsureAdditionalHumanWearComponents(baseHumanoid);
		if (repaired && !added && !addedComponents)
		{
			_context.SaveChanges();
		}

		return repaired || added || addedComponents;
	}

	private static bool HasMissingHumanWearProfiles(FuturemudDatabaseContext context)
	{
		BodyProto? baseHumanoid = context.BodyProtos.FirstOrDefault(x => x.Name == "Humanoid");
		if (baseHumanoid is null)
		{
			return false;
		}

		HashSet<string> existingNames = context.WearProfiles
		                                      .Where(x => x.BodyPrototypeId == baseHumanoid.Id)
		                                      .Select(x => x.Name)
		                                      .AsEnumerable()
		                                      .ToHashSet(StringComparer.OrdinalIgnoreCase);
		if (AdditionalHumanWearProfiles.Any(x => !existingNames.Contains(x.Name)))
		{
			return true;
		}

		HashSet<string> existingComponentNames = context.GameItemComponentProtos
		                                                .Select(x => x.Name)
		                                                .AsEnumerable()
		                                                .ToHashSet(StringComparer.OrdinalIgnoreCase);
		return AdditionalHumanWearComponents.Any(x => !existingComponentNames.Contains(x.Name));
	}

	private bool RepairExistingHumanWearProfileAccuracy(BodyProto baseHumanoid)
	{
		Dictionary<string, WearProfile> profiles = _context.WearProfiles
		                                                   .Where(x => x.BodyPrototypeId == baseHumanoid.Id)
		                                                   .AsEnumerable()
		                                                   .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
		                                                   .ToDictionary(x => x.Key, x => x.First(),
			                                                   StringComparer.OrdinalIgnoreCase);
		bool dirty = false;

		if (profiles.TryGetValue("Eyes", out WearProfile? eyes))
		{
			dirty |= SetWearlocAttribute(eyes, "Profile", "Bodypart", "reye", "Transparent", "True");
			dirty |= SetWearlocAttribute(eyes, "Profile", "Bodypart", "leye", "Transparent", "True");
		}

		string[] piercingProfiles =
		[
			"Earrings",
			"Nose Ring",
			"Earring",
			"Brow Ring",
			"Lip Ring",
			"Nipple Ring",
			"Penis Ring",
			"Bellybutton Ring",
			"Tongue Ring"
		];
		foreach (string profileName in piercingProfiles)
		{
			if (profiles.TryGetValue(profileName, out WearProfile? profile))
			{
				dirty |= SetWearAction(profile, "put", "puts", "in");
			}
		}

		if (profiles.TryGetValue("Lip Ring", out WearProfile? lipRing))
		{
			dirty |= SetDescription(lipRing, "Inserted into a lip piercing");
			StockHumanWearProfileDefinition lipRingDefinition = StockShapeWearProfile("Lip Ring", "worn in",
				"put", "puts", "in", "Inserted into a lip piercing", false, false,
				ShapeLoc("mouth", 1, true, false, true, true, false));
			dirty |= SetWearlocXml(lipRing, "Shape", BuildWearlocProfileXml(lipRingDefinition));
		}

		if (profiles.TryGetValue("Plackart", out WearProfile? plackart) &&
		    (plackart.Description.Equals("Worn as a plackart covering the back",
			     StringComparison.OrdinalIgnoreCase) ||
		     !plackart.WearlocProfiles.Contains("Bodypart=\"belly\"", StringComparison.OrdinalIgnoreCase)))
		{
			StockHumanWearProfileDefinition plackartDefinition = StockDirectWearProfile("Plackart", "worn on",
				"put", "puts", "on", "Worn as a plackart reinforcing the lower front torso", false, true,
				Loc("belly", true, false, false, true, false),
				Loc("abdomen", true, false, false, true, false),
				Loc("rbreast", false, false, false, true, false),
				Loc("lbreast", false, false, false, true, false));
			dirty |= SetDescription(plackart, plackartDefinition.Description);
			dirty |= SetWearlocXml(plackart, "Direct", BuildWearlocProfileXml(plackartDefinition));
		}

		string[] coveredTesticleProfiles =
		[
			"Bodysuit",
			"Bodysuit Thong",
			"Backless Bodysuit",
			"Briefs",
			"Shorts",
			"Capris",
			"Trousers",
			"Thong",
			"Skirt",
			"Short Skirt",
			"Long Skirt",
			"Mini Skirt",
			"Dress",
			"Gown",
			"Strapless Dress",
			"Strapless Gown",
			"Backless Dress",
			"Backless Gown",
			"Sleeveless Dress",
			"Sleeveless Gown",
			"Long-Sleeved Dress",
			"Long-Sleeved Gown",
			"Chausses"
		];
		foreach (string profileName in coveredTesticleProfiles)
		{
			if (profiles.TryGetValue(profileName, out WearProfile? profile))
			{
				dirty |= SetWearlocAttribute(profile, "Profile", "Bodypart", "testicles", "Transparent", "False");
			}
		}

		string[] pairedSeverProfiles =
		[
			"Tunic",
			"Breastplate",
			"Doublet",
			"Jerkin",
			"Cuirass",
			"Faulded Cuirass",
			"Culeted Cuirass",
			"Hauberk",
			"Haubergeon",
			"Spaulders",
			"Pauldrons"
		];
		foreach (string profileName in pairedSeverProfiles)
		{
			if (profiles.TryGetValue(profileName, out WearProfile? profile))
			{
				dirty |= SetWearlocAttribute(profile, "Profile", "Bodypart", "rnipple", "HidesSevered", "True");
				dirty |= SetWearlocAttribute(profile, "Profile", "Bodypart", "rshoulder", "HidesSevered", "False");
			}
		}

		return dirty;
	}

	private static bool SetWearAction(WearProfile profile, string wear1st, string wear3rd, string wearAffix)
	{
		bool dirty = false;
		if (!profile.WearAction1st.Equals(wear1st, StringComparison.Ordinal))
		{
			profile.WearAction1st = wear1st;
			dirty = true;
		}

		if (!profile.WearAction3rd.Equals(wear3rd, StringComparison.Ordinal))
		{
			profile.WearAction3rd = wear3rd;
			dirty = true;
		}

		if (!profile.WearAffix.Equals(wearAffix, StringComparison.Ordinal))
		{
			profile.WearAffix = wearAffix;
			dirty = true;
		}

		return dirty;
	}

	private static bool SetDescription(WearProfile profile, string description)
	{
		if (profile.Description.Equals(description, StringComparison.Ordinal))
		{
			return false;
		}

		profile.Description = description;
		return true;
	}

	private static bool SetWearlocXml(WearProfile profile, string type, string xml)
	{
		bool dirty = false;
		if (!profile.Type.Equals(type, StringComparison.Ordinal))
		{
			profile.Type = type;
			dirty = true;
		}

		if (!profile.WearlocProfiles.Equals(xml, StringComparison.Ordinal))
		{
			profile.WearlocProfiles = xml;
			dirty = true;
		}

		return dirty;
	}

	private static bool SetWearlocAttribute(WearProfile profile, string elementName, string selectorAttribute,
		string selectorValue, string targetAttribute, string targetValue)
	{
		XElement root;
		try
		{
			root = XElement.Parse(profile.WearlocProfiles);
		}
		catch
		{
			return false;
		}

		XElement? element = root.Elements(elementName)
		                        .FirstOrDefault(x => x.Attribute(selectorAttribute)?.Value.Equals(selectorValue,
			                        StringComparison.OrdinalIgnoreCase) == true);
		if (element is null)
		{
			return false;
		}

		XAttribute? attribute = element.Attribute(targetAttribute);
		if (attribute?.Value.Equals(targetValue, StringComparison.OrdinalIgnoreCase) == true)
		{
			return false;
		}

		element.SetAttributeValue(targetAttribute, targetValue);
		profile.WearlocProfiles = root.ToString();
		return true;
	}
}

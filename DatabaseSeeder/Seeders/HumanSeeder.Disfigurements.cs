#nullable enable

using MudSharp.Database;
using MudSharp.GameItems;
using MudSharp.Models;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class HumanSeeder
{
    private static readonly IReadOnlyList<SeederTattooTemplateDefinition> HumanTattooTemplates =
        [

        #region Generic Tattoos
            new SeederTattooTemplateDefinition(
        "Classic Rose",
        "a rose tattoo",
        "A classic rose tattoo blooms here, its petals carefully shaded and its thorny stem picked out in fine detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 4.0,
            ["green"] = 2.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder", "rthigh", "lthigh" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Dagger Through Rose",
        "a dagger-through-rose tattoo",
        "A slender dagger pierces a blooming rose in this dramatic flash-style tattoo.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 2.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Heart Banner",
        "a heart-and-banner tattoo reading $template{banner}",
        "A boldly inked heart tattoo is crossed by a flowing banner that bears the words $template{banner}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "banner",
                18,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "MOM",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "ornate lettering")
        },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Butterfly",
        "a butterfly tattoo",
        "A butterfly tattoo is inked here with spread wings and delicate patterned markings.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["blue"] = 1.0,
            ["purple"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rshoulder", "lshoulder", "rforearm", "lforearm", "rankle", "lankle" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Skull",
        "a skull tattoo",
        "A simple skull tattoo is inked here in bold dark lines with shaded eye sockets and teeth.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rhand", "lhand", "rcalf", "lcalf" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Coiled Snake",
        "a coiled-snake tattoo",
        "A coiled snake tattoo winds here in looping curves, its scales and head picked out in fine detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rthigh", "lthigh", "rcalf", "lcalf" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Lucky Clover",
        "a four-leaf clover tattoo",
        "A four-leaf clover tattoo is inked here in a neat lucky charm design.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 10.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 1.0,
            ["green"] = 2.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rankle", "lankle" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Armband",
        "a tribal armband tattoo",
        "Bold interlocking black shapes circle this spot in a stylised armband pattern.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Playing Cards",
        "a playing-cards tattoo",
        "A pair of playing cards is tattooed here in bold flash style with simple pip and border detail.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rhand", "lhand" },
        CanSelectInChargen: true
    ),

    new SeederTattooTemplateDefinition(
        "Flash Dragon",
        "a dragon tattoo",
        "A sinuous flash-style dragon is tattooed here with curling body, claws, and stylised scales.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 1.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "uback", "rthigh", "lthigh" },
        CanSelectInChargen: true
    ),

        new SeederTattooTemplateDefinition(
        "Paired Swallows",
        "a pair of swallow tattoos",
        "A matched pair of swallow tattoos is inked here in a traditional style, the birds shown in flight with swept wings and forked tails.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 25.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulderblade", "lshoulderblade", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Heart Banner Memorial",
        "a heart-and-banner tattoo reading $template{banner}",
        "A boldly inked heart tattoo is crossed by a flowing banner that bears the words $template{banner}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder", "rbreast", "lbreast" },
        TextSlots:
        [
            new SeederTattooTextSlotDefinition(
                "banner",
                16,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "Mother",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultMinimumSkill: 0.0,
                DefaultAlternateText: "a short name in ornate lettering")
        ]
    ),

    new SeederTattooTemplateDefinition(
        "Lucky Horseshoe",
        "a horseshoe tattoo",
        "A lucky horseshoe tattoo is inked here in a simple traditional style, its curved form outlined in dark ink with small decorative highlights.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 10.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "rshoulder", "lshoulder", "rankle", "lankle" }
    ),
        #endregion

        #region Nautical Tattoos
        new SeederTattooTemplateDefinition(
        "Traditional Anchor",
        "a traditional anchor tattoo",
        "A traditional sailor's anchor tattoo is inked here in bold lines with simple shaded detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 15.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["dark blue"] = 2.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Compass Rose",
        "a compass rose tattoo",
        "A compass rose tattoo is inked here in crisp lines, its points arranged around a central starburst.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 2.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rforearm", "lforearm", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Paired Swallows",
        "a pair of swallow tattoos",
        "A matched pair of swallow tattoos is inked here in a traditional style, the birds shown in flight with swept wings and forked tails.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 25.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulderblade", "lshoulderblade", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Ship's Wheel",
        "a ship's wheel tattoo",
        "A ship's wheel tattoo is inked here in dark lines, each spoke and rim carefully picked out in neat traditional detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Full-Rigged Ship",
        "a full-rigged ship tattoo",
        "A full-rigged sailing ship is tattooed here with dark hull lines and a lacework of masts, spars, and sails.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 35.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 6.0,
            ["dark blue"] = 2.0
        },
        BodypartAliases: new[] { "uback", "lback", "abdomen", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Lighthouse",
        "a lighthouse tattoo",
        "A lighthouse tattoo is inked here in a traditional style, its tower picked out in dark ink above stylised waves.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 2.0,
            ["light blue"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Mermaid Pinup",
        "a mermaid pinup tattoo",
        "A pinup-style mermaid tattoo is inked here with flowing hair, curved lines, and stylised scales and fins.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["green"] = 1.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Rope Knot Wristband",
        "a rope-knot wristband tattoo",
        "A looped rope-knot design circles this spot like a sailor's bracelet, its strands inked in interwoven lines.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 12.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist" }
    ),

    new SeederTattooTemplateDefinition(
        "Nautical Star",
        "a nautical star tattoo",
        "A bold nautical star tattoo is inked here in sharp points and alternating shaded facets.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 15.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 2.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Sailor Heart Banner",
        "a sailor heart-and-banner tattoo reading $template{banner}",
        "A traditional heart tattoo is crossed by a flowing banner that bears the words $template{banner}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "banner",
                18,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "Mother",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "ornate lettering")
        }
    ),

        #endregion

        #region Prison Tattoos
        new SeederTattooTemplateDefinition(
        "Five Dots Cluster",
        "a five-dot tattoo",
        "Five small inked dots are arranged here in a tight quincunx pattern.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 8.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 1.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rwrist", "lwrist" }
    ),

    new SeederTattooTemplateDefinition(
        "Spiderweb Elbow",
        "a spiderweb elbow tattoo",
        "A spiderweb tattoo spreads across this elbow in radiating strands and curved rings of dark ink.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "relbow", "lelbow" }
    ),

    new SeederTattooTemplateDefinition(
        "Teardrop Mark",
        "a teardrop tattoo",
        "A small teardrop tattoo is inked just beneath the eye in a stark dark mark.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 10.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 1.0
        },
        BodypartAliases: new[] { "reyesocket", "leyesocket" },
        OverrideCharacteristicPlain: "facially-tattooed",
        OverrideCharacteristicWith: "with a teardrop tattoo beneath one eye"
    ),

    new SeederTattooTemplateDefinition(
        "Neck Set Script",
        "a neck script tattoo reading $template{script}",
        "Blocky lettering is tattooed here in a stark script that reads $template{script}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "neck", "bneck", "throat" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "script",
                16,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "LOYALTY",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "block lettering")
        },
        OverrideCharacteristicPlain: "gang-marked",
        OverrideCharacteristicWith: "with tattooed script across the neck"
    ),

    new SeederTattooTemplateDefinition(
        "Crown Insignia",
        "a crown tattoo",
        "A crown tattoo is inked here in blunt dark lines, its points and jewels rendered in a simple hard-edged style.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "neck", "rhand", "lhand", "rtemple", "ltemple", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Crossed Blades",
        "a crossed-blades tattoo",
        "A pair of crossed knives is tattooed here in dark ink with simple hilts and tapering blades.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Barbed Wire Wristband",
        "a barbed-wire wristband tattoo",
        "Barbed wire is tattooed around this spot in a looping band of twisted strands and sharp barbs.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 12.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist" }
    ),

    new SeederTattooTemplateDefinition(
        "Loaded Dice",
        "a loaded-dice tattoo",
        "A pair of dice is tattooed here in dark ink, the faces rendered with bold pips and simple shading.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 12.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rforearm", "lforearm", "neck" }
    ),

    new SeederTattooTemplateDefinition(
        "Pistol Pair",
        "a pair of pistol tattoos",
        "A mirrored pair of pistols is inked here in stark dark lines with simple mechanical detailing.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Block-Letter Knuckles",
        "a block-letter hand tattoo reading $template{letters}",
        "Heavy block letters are tattooed across this hand to spell $template{letters}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rhand", "lhand" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "letters",
                8,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "HATE",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "block lettering")
        },
        OverrideCharacteristicPlain: "gang-marked",
        OverrideCharacteristicWith: "with tattooed lettering across the hands"
    ),
        #endregion

        #region Military Tattoos
        new SeederTattooTemplateDefinition(
        "Shoulder Unit Insignia",
        "a military unit insignia tattoo labelled $template{unit}",
        "A military insignia tattoo is inked here with a small scroll bearing the text $template{unit}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark red"] = 1.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "unit",
                20,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "3RD INFANTRY",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "military lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Rank Chevron",
        "a rank-chevron tattoo",
        "A set of military chevrons is tattooed here in crisp angular bands.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Crossed Rifles",
        "a crossed-rifles tattoo",
        "A pair of crossed rifles is tattooed here in neat dark lines with simple stock and barrel detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Airborne Wings",
        "an airborne wings tattoo",
        "A pair of military wings is tattooed here around a central badge in a neat formal design.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 1.0
        },
        BodypartAliases: new[] { "uback", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Combat Knife",
        "a combat-knife tattoo",
        "A military fighting knife is tattooed here with a straight blade, plain guard, and wrapped grip.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Dog Tags Memorial",
        "a memorial dog-tags tattoo reading $template{name}",
        "A pair of military dog tags is tattooed here, one tag bearing the name $template{name}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["gray"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "name",
                24,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "J. WALKER",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "small stamped lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Service Ribbon Stack",
        "a service-ribbons tattoo",
        "A neat stack of service ribbons is tattooed here in parallel bars of dark ink and subdued colour.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["dark red"] = 1.0,
            ["dark blue"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Battlefield Cross Memorial",
        "a battlefield-cross memorial tattoo",
        "A battlefield-cross memorial is tattooed here, showing a grounded rifle, boots, and a helmet in solemn silhouette.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "uback", "lback", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Battalion Scroll",
        "a military scroll tattoo reading $template{unit}",
        "A military scroll tattoo is inked here with dark formal lettering spelling $template{unit}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "unit",
                20,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "1ST BATTALION",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "formal lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Bomber Nose Art",
        "a nose-art pinup tattoo labelled $template{nickname}",
        "A military nose-art style pinup is tattooed here with a small banner bearing the words $template{nickname}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 1.0,
            ["dark blue"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "nickname",
                18,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "LUCKY LADY",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "painted script")
        }
    ),
        #endregion

        #region Occult Tattoos
        new SeederTattooTemplateDefinition(
        "Pentagram",
        "a pentagram tattoo",
        "A five-pointed star within a circle is tattooed here in sharp dark lines.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "uback", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Triple Moon",
        "a triple-moon tattoo",
        "A triple-moon symbol is tattooed here, showing a full moon flanked by waxing and waning crescents.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["silver grey"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "bneck", "lback", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Ouroboros",
        "an ouroboros tattoo",
        "A serpent devouring its own tail is tattooed here in a looping ring of scales and curved linework.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rankle", "lankle", "rforearm", "lforearm", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Eye in Triangle",
        "an eye-in-triangle tattoo",
        "An all-seeing eye within a triangle is tattooed here in fine dark lines and stark shading.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "forehead", "throat", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Horned Skull Sigil",
        "a horned-skull tattoo",
        "A horned skull is tattooed here in dark, ritualistic linework with curling horns and hollow sockets.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade", "uback", "rthigh", "lthigh", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Ritual Circle",
        "a ritual-circle tattoo",
        "A ritual circle is tattooed here in concentric rings, angular marks, and linked occult symbols.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "uback", "lback", "abdomen", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Alchemical Sun",
        "an alchemical sun tattoo",
        "A stylised alchemical sun is tattooed here as a radiant disk ringed with sharp narrow rays.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "forehead", "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Crescent Moon Sigil",
        "a crescent-moon sigil tattoo",
        "A sharp crescent moon and small surrounding marks are tattooed here in a clean occult motif.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["silver grey"] = 1.0
        },
        BodypartAliases: new[] { "bneck", "throat", "rshoulderblade", "lshoulderblade", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Black Sun",
        "a black-sun tattoo",
        "A dark solar wheel of radiating hooked rays is tattooed here in dense, heavy ink.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Sigil Banner",
        "an occult sigil tattoo bearing $template{motto}",
        "A complex occult sigil is tattooed here with a narrow band of script that reads $template{motto}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "bneck", "lback", "rupperarm", "lupperarm" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "motto",
                24,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "AS ABOVE SO BELOW",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "arcane script")
        }
    ),
        #endregion

        #region Tribal Tattoos
        new SeederTattooTemplateDefinition(
        "Tribal Upper Arm Band",
        "a tribal upper-arm band tattoo",
        "A bold band of interlocking tribal shapes encircles this spot in thick black ink.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Forearm Wrap",
        "a tribal forearm wrap tattoo",
        "A sleeve-like tribal wrap of hooked lines and sweeping black shapes coils around this forearm.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Shoulder Spiral",
        "a tribal shoulder spiral tattoo",
        "A broad spiral of tribal linework spreads across this shoulder in thick black curves and tapering points.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Shoulder Blade Panel",
        "a tribal shoulder-blade panel tattoo",
        "A panel of angular tribal patterning is inked here in layered black hooks and curved spikes.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Upper Back Crest",
        "a tribal upper-back crest tattoo",
        "A broad crest of symmetrical tribal linework spreads across the upper back in dense black geometry.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 6.0
        },
        BodypartAliases: new[] { "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Lower Leg Wrap",
        "a tribal lower-leg wrap tattoo",
        "A sleeve-like tribal wrap of black lines and spear-point shapes rings this lower leg.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "rcalf", "lcalf", "rshin", "lshin" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Thigh Band",
        "a tribal thigh-band tattoo",
        "A heavy band of tribal patterning circles this thigh in repeating black motifs.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "rthigh", "lthigh", "rthighback", "lthighback" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Hip Sweep",
        "a tribal hip-sweep tattoo",
        "A sweeping tribal design curves across this hip in hooked black lines and tapering blades.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rhip", "lhip" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Hand Knotwork",
        "a tribal hand tattoo",
        "A compact knot of tribal linework sprawls across this hand in dense black curves and sharp angles.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Tribal Neck Marking",
        "a tribal neck tattoo",
        "Angular tribal marks are tattooed here in stark black lines that follow the line of the neck.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "neck", "bneck" },
        OverrideCharacteristicPlain: "ritually-tattooed",
        OverrideCharacteristicWith: "with tribal tattoos on the neck"
    ),
        #endregion

        #region Tramp Stamps
        new SeederTattooTemplateDefinition(
        "Lower Back Butterfly",
        "a butterfly lower-back tattoo",
        "A large butterfly is tattooed across the lower back, its wings spread wide in decorative symmetry.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["blue"] = 1.0,
            ["purple"] = 1.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Tribal Wings",
        "a tribal-wing lower-back tattoo",
        "Stylised tribal wings spread across the lower back in mirrored black curves and pointed barbs.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Rose Cluster",
        "a rose-cluster lower-back tattoo",
        "A cluster of roses and leaves is tattooed across the lower back in an ornamental arrangement.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 2.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Knotwork",
        "a knotwork lower-back tattoo",
        "An intricate knotwork design spans the lower back in interwoven loops and mirrored turns.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Heart Wings",
        "a winged-heart lower-back tattoo",
        "A heart with outspread wings is tattooed across the lower back in bold, decorative lines.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 2.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Moon And Stars",
        "a moon-and-stars lower-back tattoo",
        "A crescent moon and a spray of small stars are tattooed across the lower back in a decorative sweep.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 15.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["silver grey"] = 1.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Swallows",
        "a pair of swallow lower-back tattoos",
        "A mirrored pair of swallows is tattooed across the lower back in sweeping traditional lines.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["dark blue"] = 2.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Vines",
        "a vinework lower-back tattoo",
        "Trailing vines and leaves curl across the lower back in a decorative, symmetrical pattern.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["green"] = 2.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Starburst",
        "a starburst lower-back tattoo",
        "A sharp ornamental starburst spreads across the lower back in layered points and curved flourishes.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 17.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Script Scroll",
        "a scripted lower-back tattoo reading $template{script}",
        "A decorative lower-back scroll bears the words $template{script} in ornate lettering.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "lback" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "script",
                24,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "wild at heart",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "ornate lower-back script")
        }
    ),
        #endregion

        #region Animals
        new SeederTattooTemplateDefinition(
        "Wolf Head",
        "a wolf-head tattoo",
        "A snarling wolf's head is tattooed here in bold linework and shaded fur detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["grey"] = 1.0 },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Howling Wolf",
        "a howling-wolf tattoo",
        "A wolf is tattooed here with its muzzle raised in a long, haunting howl.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["blue"] = 1.0 },
        BodypartAliases: new[] { "uback", "rforearm", "lforearm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Tiger",
        "a tiger tattoo",
        "A crouching tiger is tattooed here with striped flanks and a fixed, predatory stare.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["orange"] = 2.0, ["white"] = 1.0 },
        BodypartAliases: new[] { "uback", "abdomen", "rthigh", "lthigh", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Lion",
        "a lion tattoo",
        "A proud lion with a full mane is tattooed here in bold, regal linework.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["gold"] = 1.0, ["brown"] = 1.0 },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rupperarm", "lupperarm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Panther",
        "a panther tattoo",
        "A sleek black panther is tattooed here in a stalking pose with taut muscles and bared teeth.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double> { ["black"] = 5.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "uback", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Bear",
        "a bear tattoo",
        "A heavy-set bear is tattooed here in dark, rugged linework with thick fur and powerful limbs.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["brown"] = 2.0 },
        BodypartAliases: new[] { "uback", "lback", "rupperarm", "lupperarm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Fox",
        "a fox tattoo",
        "A fox is tattooed here with a sly expression and a sweeping brush of tail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double> { ["black"] = 2.0, ["orange"] = 2.0, ["white"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Stag",
        "a stag tattoo",
        "A stag with branching antlers is tattooed here in elegant linework and careful shading.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["brown"] = 1.0 },
        BodypartAliases: new[] { "uback", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Horse",
        "a horse tattoo",
        "A powerful horse is tattooed here in motion, its mane and tail flowing behind it.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["brown"] = 2.0 },
        BodypartAliases: new[] { "uback", "abdomen", "rthigh", "lthigh", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Eagle",
        "an eagle tattoo",
        "An eagle with spread wings is tattooed here in bold lines and layered feathers.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["brown"] = 1.0, ["gold"] = 1.0 },
        BodypartAliases: new[] { "uback", "rshoulderblade", "lshoulderblade", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Hawk",
        "a hawk tattoo",
        "A diving hawk is tattooed here with hooked beak, tucked wings, and sharp talons.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["brown"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rshoulder", "lshoulder", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Owl",
        "an owl tattoo",
        "An owl is tattooed here with wide eyes, layered feathers, and a silent, watchful posture.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["brown"] = 1.0 },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Raven",
        "a raven tattoo",
        "A raven is tattooed here in glossy dark ink, its beak and feathers rendered in sharp detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double> { ["black"] = 5.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rshoulderblade", "lshoulderblade", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Snake",
        "a snake tattoo",
        "A snake coils across this spot in looping curves, its scales and narrowed head picked out in fine detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["green"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Shark",
        "a shark tattoo",
        "A shark is tattooed here in a sleek, aggressive profile with jaws slightly parted.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["grey"] = 2.0, ["blue"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "uback", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Octopus",
        "an octopus tattoo",
        "An octopus sprawls across this spot, its mantle and curling tentacles rendered in dense flowing linework.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["purple"] = 1.0 },
        BodypartAliases: new[] { "uback", "abdomen", "rthigh", "lthigh", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Koi",
        "a koi tattoo",
        "A koi fish is tattooed here in a graceful swimming curve, its scales and fins carefully detailed.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 2.0, ["red"] = 2.0, ["gold"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Butterfly",
        "a butterfly tattoo",
        "A butterfly with broad patterned wings is tattooed here in a bright, decorative style.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double> { ["black"] = 2.0, ["blue"] = 1.0, ["purple"] = 1.0 },
        BodypartAliases: new[] { "rwrist", "lwrist", "rshoulder", "lshoulder", "rankle", "lankle", "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Moth",
        "a moth tattoo",
        "A moth is tattooed here with dusty wings spread wide in a symmetrical, eerie display.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["brown"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Scorpion",
        "a scorpion tattoo",
        "A scorpion is tattooed here with raised tail, grasping pincers, and segmented body.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rhand", "lhand", "rthigh", "lthigh", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Spider",
        "a spider tattoo",
        "A spider is tattooed here in stark dark ink, its legs spread outward from a compact body.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0 },
        BodypartAliases: new[] { "relbow", "lelbow", "rhand", "lhand", "neck", "bneck" }
    ),
        #endregion

        #region Mythical Animals
        new SeederTattooTemplateDefinition(
        "Dragon",
        "a dragon tattoo",
        "A long-bodied dragon is tattooed here with curling coils, claws, and a fierce scaled head.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["red"] = 1.0, ["green"] = 1.0 },
        BodypartAliases: new[] { "uback", "abdomen", "rthigh", "lthigh", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Phoenix",
        "a phoenix tattoo",
        "A phoenix rises here in a blaze of feathers and sweeping flames, wings flung wide.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["red"] = 2.0, ["gold"] = 1.0 },
        BodypartAliases: new[] { "uback", "rshoulderblade", "lshoulderblade", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Griffin",
        "a griffin tattoo",
        "A griffin is tattooed here with leonine hindquarters, a hooked beak, and spread feathered wings.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["brown"] = 1.0, ["gold"] = 1.0 },
        BodypartAliases: new[] { "uback", "rthigh", "lthigh", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Unicorn",
        "a unicorn tattoo",
        "A unicorn is tattooed here in elegant profile, its horn and flowing mane rendered in fine detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double> { ["black"] = 2.0, ["white"] = 2.0, ["silver grey"] = 1.0 },
        BodypartAliases: new[] { "rthigh", "lthigh", "rforearm", "lforearm", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Pegasus",
        "a pegasus tattoo",
        "A winged horse is tattooed here in mid-flight, its wings arched high and mane streaming behind it.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double> { ["black"] = 2.0, ["white"] = 2.0, ["blue"] = 1.0 },
        BodypartAliases: new[] { "uback", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Hydra",
        "a hydra tattoo",
        "A many-headed hydra is tattooed here with writhing necks and snapping jaws in a mass of scaled coils.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 32.0,
        InkColours: new Dictionary<string, double> { ["black"] = 5.0, ["green"] = 1.0 },
        BodypartAliases: new[] { "uback", "abdomen", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Kraken",
        "a kraken tattoo",
        "A monstrous kraken is tattooed here, its tentacles curling outward from a dark central body.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 32.0,
        InkColours: new Dictionary<string, double> { ["black"] = 5.0, ["blue"] = 1.0 },
        BodypartAliases: new[] { "uback", "abdomen", "lback", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Manticore",
        "a manticore tattoo",
        "A manticore is tattooed here with leonine body, barbed tail, and a monstrous human-like face.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["brown"] = 1.0, ["red"] = 1.0 },
        BodypartAliases: new[] { "uback", "rthigh", "lthigh", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Chimera",
        "a chimera tattoo",
        "A chimera is tattooed here as a fused beast of lion, goat, and serpent in snarling profusion.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 32.0,
        InkColours: new Dictionary<string, double> { ["black"] = 5.0, ["brown"] = 1.0, ["green"] = 1.0 },
        BodypartAliases: new[] { "uback", "abdomen", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Basilisk",
        "a basilisk tattoo",
        "A basilisk is tattooed here with serpent body, clawed limbs, and a fixed deadly stare.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["green"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rthigh", "lthigh" }
    ),
        #endregion

        #region Angels and Demons
        new SeederTattooTemplateDefinition(
        "Guardian Angel",
        "a guardian-angel tattoo",
        "A guardian angel is tattooed here with spread wings, flowing robes, and a solemn, protective posture.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["white"] = 2.0, ["gold"] = 1.0 },
        BodypartAliases: new[] { "uback", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Praying Angel",
        "a praying-angel tattoo",
        "An angel with bowed head and folded hands is tattooed here in fine devotional detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["white"] = 1.0, ["blue"] = 1.0 },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Archangel With Sword",
        "an archangel-with-sword tattoo",
        "An armoured archangel stands here with a lowered sword and outspread wings in a stern martial pose.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 32.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["silver grey"] = 1.0, ["gold"] = 1.0 },
        BodypartAliases: new[] { "uback", "abdomen", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Angel Wings",
        "an angel-wings tattoo",
        "A pair of feathered angel wings is tattooed here in layered, graceful detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double> { ["black"] = 3.0, ["white"] = 2.0 },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Cherub",
        "a cherub tattoo",
        "A cherub is tattooed here with small wings, soft features, and a decorative classical pose.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double> { ["black"] = 2.0, ["white"] = 1.0, ["gold"] = 1.0 },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Horned Demon",
        "a horned-demon tattoo",
        "A horned demon is tattooed here with a snarling face, heavy brow, and curling horns.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["dark red"] = 1.0 },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rthigh", "lthigh", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Demonic Skull",
        "a demonic-skull tattoo",
        "A skull with horns and a wicked grin is tattooed here in dense black and red linework.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["dark red"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Fallen Angel",
        "a fallen-angel tattoo",
        "A fallen angel is tattooed here with darkened wings, lowered head, and a posture of defeat or wrath.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["dark red"] = 1.0, ["grey"] = 1.0 },
        BodypartAliases: new[] { "uback", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Demon Wings",
        "a demon-wings tattoo",
        "A pair of leathery demon wings is tattooed here in dark, barbed shapes and ragged edges.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double> { ["black"] = 5.0, ["dark red"] = 1.0 },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Demon And Flame",
        "a demon-and-flame tattoo",
        "A grinning demon emerges here from stylised flames in a dramatic, infernal composition.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double> { ["black"] = 4.0, ["dark red"] = 2.0, ["orange"] = 1.0 },
        BodypartAliases: new[] { "rforearm", "lforearm", "rthigh", "lthigh", "abdomen", "uback" }
    ),
        #endregion

        #region Flowers
        new SeederTattooTemplateDefinition(
        "Single Rose Bloom",
        "a rose tattoo",
        "A single rose blooms here in careful linework, its petals shaded in rich depth above a short thorned stem.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 3.0,
            ["green"] = 2.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rshoulder", "lshoulder", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Rose Bouquet",
        "a rose-bouquet tattoo",
        "A small bouquet of roses and leaves is tattooed here in layered floral detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 3.0,
            ["green"] = 2.0
        },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade", "uback", "rthigh", "lthigh", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Lily Stem",
        "a lily tattoo",
        "A long-stemmed lily is tattooed here in elegant floral linework with open petals and narrow leaves.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["white"] = 2.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Lotus Blossom",
        "a lotus tattoo",
        "A lotus blossom is tattooed here in layered petals and calm symmetrical lines.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["pink"] = 2.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "abdomen", "uback", "lback", "rthigh", "lthigh", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Cherry Blossom Spray",
        "a cherry-blossom tattoo",
        "A spray of cherry blossoms is tattooed here in delicate clustered blooms along a slender branch.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["pink"] = 2.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh", "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Ivy Vine",
        "an ivy-vine tattoo",
        "A trailing ivy vine curls here in looping stems and pointed leaves.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["green"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rankle", "lankle", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Thorn Band",
        "a thorn-band tattoo",
        "A band of curling thorned vines encircles this spot in dark, decorative linework.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rupperarm", "lupperarm", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Sunflower",
        "a sunflower tattoo",
        "A sunflower is tattooed here with broad petals, a dark central disk, and a stout leafy stem.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["yellow"] = 3.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rthigh", "lthigh", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Fern Frond",
        "a fern tattoo",
        "A curling fern frond is tattooed here in fine leaf detail and graceful natural curvature.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["green"] = 2.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rcalf", "lcalf", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Floral Wreath",
        "a floral-wreath tattoo",
        "A circular wreath of blossoms and leaves is tattooed here in a neat ornamental arrangement.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["red"] = 1.0,
            ["green"] = 2.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh", "abdomen" }
    ),
        #endregion

        #region Religious Designs
        new SeederTattooTemplateDefinition(
        "Latin Cross",
        "a cross tattoo",
        "A simple Latin cross is tattooed here in dark, clean devotional linework.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 10.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "neck", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Crucifix",
        "a crucifix tattoo",
        "A crucifix is tattooed here in solemn detail, its form rendered in dark devotional ink.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Rosary",
        "a rosary tattoo",
        "A rosary with hanging crucifix is tattooed here in linked beads and devotional detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rhand", "lhand", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Sacred Heart",
        "a sacred-heart tattoo",
        "A sacred heart is tattooed here, crowned with flame and encircled by devotional ornament.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 2.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "abdomen", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Praying Hands",
        "a praying-hands tattoo",
        "A pair of praying hands is tattooed here in solemn devotional detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Saint Icon",
        "a saint-icon tattoo",
        "A haloed saint is tattooed here in an icon-like devotional style with careful robes and features.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "uback", "rthigh", "lthigh", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Haloed Dove",
        "a haloed-dove tattoo",
        "A dove bearing a small halo is tattooed here in calm, reverent linework.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["white"] = 2.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rforearm", "lforearm", "uback", "neck" }
    ),

    new SeederTattooTemplateDefinition(
        "Star Of David",
        "a star-of-david tattoo",
        "A six-pointed star is tattooed here in neat, devotional geometry.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 12.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rforearm", "lforearm", "neck" }
    ),

    new SeederTattooTemplateDefinition(
        "Crescent And Star",
        "a crescent-and-star tattoo",
        "A crescent embracing a small star is tattooed here in simple devotional linework.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 12.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["silver grey"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "neck", "bneck", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Devotional Scroll",
        "a devotional scroll tattoo reading $template{verse}",
        "A devotional scroll is tattooed here in formal script reading $template{verse}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "uback", "abdomen" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "verse",
                32,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "faith over fear",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "formal devotional script")
        }
    ),
        #endregion

        #region Script
        new SeederTattooTemplateDefinition(
        "Neck Script",
        "a neck-script tattoo reading $template{text}",
        "Elegant script is tattooed on the neck here, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "neck" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                18,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "loyal",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "fine neck script")
        },
        OverrideCharacteristicPlain: "tattooed",
        OverrideCharacteristicWith: "with script tattooed on the neck"
    ),

    new SeederTattooTemplateDefinition(
        "Back Of Neck Script",
        "a back-of-neck tattoo reading $template{text}",
        "A line of script is tattooed across the back of the neck here, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "bneck" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                20,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "never again",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "small script")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Throat Motto",
        "a throat tattoo reading $template{text}",
        "Compact lettering is tattooed at the throat here, spelling $template{text}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "throat" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                16,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "truth",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "compact throat lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Forearm Cursive Script",
        "a forearm tattoo reading $template{text}",
        "A flowing cursive phrase is tattooed along this forearm, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                32,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "stay strong",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "flowing forearm script")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Forearm Block Lettering",
        "a forearm tattoo reading $template{text}",
        "Bold block lettering is tattooed on this forearm, spelling $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                24,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "RESIST",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "bold block lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Wrist Name",
        "a wrist tattoo reading $template{name}",
        "A small name is tattooed on this wrist here, reading $template{name}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 12.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "name",
                14,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "Mia",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "a small name in script")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Hand Word",
        "a hand tattoo reading $template{text}",
        "A single emphatic word is tattooed across this hand, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rhand", "lhand" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                10,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "HOPE",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "a bold hand tattoo")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Upper Arm Scroll",
        "an upper-arm scroll tattoo reading $template{text}",
        "A narrow scroll tattoo wraps this upper arm and bears the words $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                24,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "born free",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "scroll lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Shoulder Motto",
        "a shoulder tattoo reading $template{text}",
        "A short motto is tattooed across this shoulder in neat lettering, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                20,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "no surrender",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "small shoulder text")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Upper Back Arched Script",
        "an upper-back tattoo reading $template{text}",
        "A broad arched line of script spans the upper back here, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "uback" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                40,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "only the dead have seen the end of war",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "arched back lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Back Script",
        "a lower-back tattoo reading $template{text}",
        "A decorative line of script is tattooed across the lower back here, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "lback" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                28,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "wild at heart",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "ornamental lower-back script")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Abdomen Motto",
        "an abdomen tattoo reading $template{text}",
        "A broad phrase is tattooed across the abdomen here, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "abdomen", "belly" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                32,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "still breathing",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "broad abdomen lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Thigh Script Panel",
        "a thigh tattoo reading $template{text}",
        "A vertical panel of script is tattooed on this thigh here, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rthigh", "lthigh", "rthighback", "lthighback" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                36,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "fortune favours the bold",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "a long panel of thigh script")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Lower Leg And Foot Text",
        "a lower-leg or foot tattoo reading $template{text}",
        "A line of text is tattooed here on the lower leg or foot, reading $template{text}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rcalf", "lcalf", "rankle", "lankle", "rfoot", "lfoot" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "text",
                20,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "keep moving",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "small lower-leg script")
        }
    ),
        #endregion

        #region Pinups
        new SeederTattooTemplateDefinition(
        "Classic Pinup Girl",
        "a pinup-girl tattoo",
        "A classic pinup girl is tattooed here in a flirtatious pose, with curled hair, shapely limbs, and bold traditional linework.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 1.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Sailor Pinup",
        "a sailor-pinup tattoo",
        "A pinup woman in sailor attire is tattooed here in bold, lively lines with a playful pose and traditional detailing.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 1.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Burlesque Dancer",
        "a burlesque-dancer tattoo",
        "A glamorous burlesque dancer is tattooed here in a theatrical pose with stockings, feathers, and bold decorative flourishes.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 1.0,
            ["purple"] = 1.0
        },
        BodypartAliases: new[] { "rthigh", "lthigh", "rupperarm", "lupperarm", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Lady Portrait",
        "a lady-portrait tattoo",
        "A finely shaded portrait of a woman is tattooed here, with careful features, styled hair, and an intent, poised expression.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["grey"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Gentleman Portrait",
        "a gentleman-portrait tattoo",
        "A formal portrait of a stern-faced gentleman is tattooed here in dark, carefully shaded detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Mourning Woman",
        "a mourning-woman tattoo",
        "A sorrowful woman with lowered head and flowing veil is tattooed here in sombre, elegant linework.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["grey"] = 1.0
        },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade", "uback", "rupperarm", "lupperarm" }
    ),

    new SeederTattooTemplateDefinition(
        "Crowned Queen Portrait",
        "a crowned-queen portrait tattoo",
        "A regal female portrait with crown and flowing hair is tattooed here in richly composed traditional detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 32.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["gold"] = 1.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rthigh", "lthigh", "rshoulderblade", "lshoulderblade", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Boxer Portrait",
        "a boxer-portrait tattoo",
        "A hard-eyed boxer with raised fists is tattooed here in bold athletic detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Lady With Roses",
        "a lady-with-roses tattoo",
        "A woman's portrait framed by roses and leaves is tattooed here in ornate, romantic linework.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["red"] = 2.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Skeleton Bride",
        "a skeleton-bride tattoo",
        "A skeletal bride in veil and finery is tattooed here in dramatic gothic detail, equal parts macabre and elegant.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["white"] = 1.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rthigh", "lthigh", "rshoulderblade", "lshoulderblade" }
    ),
        #endregion

        #region Sigils and Shapes
        new SeederTattooTemplateDefinition(
        "Mandala",
        "a mandala tattoo",
        "A symmetrical mandala of layered petals, rings, and radiating geometry is tattooed here in fine ornamental detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 26.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "lback", "rthigh", "lthigh", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Flower Of Life",
        "a flower-of-life tattoo",
        "Interlocking circles form a flower-of-life pattern here in precise, repeating sacred geometry.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Metatron Cube",
        "a metatron-cube tattoo",
        "A lattice of circles and straight lines forms a Metatron's Cube tattoo here in crisp geometric precision.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 30.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "rshoulderblade", "lshoulderblade", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Interlocking Triangles",
        "an interlocking-triangles tattoo",
        "A pattern of interlocking triangles is tattooed here in layered angular geometry.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "rhand", "lhand", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Lunar Phases Band",
        "a lunar-phases tattoo",
        "A sequence of moon phases is tattooed here in a clean, narrow band of crescents and dark disks.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["silver grey"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Runic Circle",
        "a runic-circle tattoo",
        "A ring of angular sigils surrounds an empty centre here in an austere occult design.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade", "uback", "abdomen", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Hexagram Seal",
        "a hexagram-seal tattoo",
        "A hexagram enclosed in a sharp ritual circle is tattooed here with clean ceremonial linework.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["dark blue"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "neck", "bneck" }
    ),

    new SeederTattooTemplateDefinition(
        "Alchemical Elements",
        "an alchemical-elements tattoo",
        "The elemental symbols of earth, air, fire, and water are tattooed here in a neat aligned series.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rwrist", "lwrist", "rhand", "lhand", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Ornamental Filigree",
        "an ornamental-filigree tattoo",
        "Curled ornamental filigree spreads here in layered loops, pointed leaves, and decorative symmetry.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rforearm", "lforearm", "lback", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Occult Sigil Panel",
        "an occult-sigil tattoo",
        "A dense occult sigil of intersecting lines, circles, and marks is tattooed here in stark ritual geometry.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 28.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "rforearm", "lforearm", "rthigh", "lthigh" }
    ),
        #endregion

        #region Eastern Religious Symbols
        new SeederTattooTemplateDefinition(
            "Om Symbol",
            "an om-symbol tattoo",
            "The sacred Om symbol is tattooed here in elegant, balanced devotional linework.",
            MinimumBodypartSize: SizeCategory.Tiny,
            MinimumSkill: 16.0,
            InkColours: new Dictionary<string, double>
            {
                ["black"] = 3.0,
                ["gold"] = 1.0
            },
            BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "rshoulder", "lshoulder", "neck" }
        ),

    new SeederTattooTemplateDefinition(
        "Dharma Wheel",
        "a dharma-wheel tattoo",
        "An eight-spoked Dharma wheel is tattooed here in crisp devotional symmetry.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rforearm", "lforearm", "uback", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Unalome",
        "an unalome tattoo",
        "A graceful unalome symbol is tattooed here in a flowing line that rises from coils into a straight path.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "bneck", "rforearm", "lforearm", "rupperarm", "lupperarm", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Endless Knot",
        "an endless-knot tattoo",
        "An endless knot of interwoven lines is tattooed here in a compact, devotional pattern.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rforearm", "lforearm", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Lotus Mandala",
        "a lotus-mandala tattoo",
        "A lotus framed by radiating devotional geometry is tattooed here in balanced ornamental detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 24.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["pink"] = 1.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "abdomen", "uback", "lback", "rthigh", "lthigh", "rshoulder", "lshoulder" }
    ),

    new SeederTattooTemplateDefinition(
        "Ensō Circle",
        "an enso-circle tattoo",
        "A single brush-like circle is tattooed here in an austere, meditative composition.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "rshoulder", "lshoulder", "bneck" }
    ),

    new SeederTattooTemplateDefinition(
        "Yin Yang",
        "a yin-yang tattoo",
        "A yin-yang symbol is tattooed here in clean, balanced contrast.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["white"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rforearm", "lforearm", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Torii Gate",
        "a torii-gate tattoo",
        "A simple torii gate is tattooed here in clean devotional silhouette.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Triratna",
        "a triratna tattoo",
        "The three-jewel emblem is tattooed here in compact, reverent linework.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "neck", "bneck" }
    ),

    new SeederTattooTemplateDefinition(
        "Vajra Emblem",
        "a vajra-emblem tattoo",
        "A stylised vajra emblem is tattooed here in symmetrical ritual detail.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rhand", "lhand", "rshoulder", "lshoulder", "uback" }
    ),
        #endregion

        #region Crude Tattoos
        new SeederTattooTemplateDefinition(
        "Crooked Heart",
        "a crooked heart tattoo",
        "A lopsided heart tattoo is scratched here in uneven lines, its fill patchy and its outline visibly shaky.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 6.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rhip", "lhip", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Uneven Star",
        "an uneven star tattoo",
        "A crude five-pointed star is tattooed here, its points mismatched and its lines wandering badly.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 5.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rforearm", "lforearm", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Shaky Cross",
        "a shaky cross tattoo",
        "A simple cross is tattooed here in wobbly dark lines, as though done by an unsteady hand.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 4.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "neck", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Amateur Anchor",
        "an amateur anchor tattoo",
        "A badly drawn anchor is tattooed here in thick uncertain lines, with uneven flukes and a crooked shank.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Wobbly Dice",
        "a wobbly dice tattoo",
        "A pair of dice is tattooed here in crude blocky outlines, the faces uneven and the pips badly placed.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rforearm", "lforearm", "rhip", "lhip" }
    ),

    new SeederTattooTemplateDefinition(
        "Blurry Skull",
        "a blurry skull tattoo",
        "A crude skull is tattooed here in muddy, overworked ink, its teeth and sockets blurred together.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 8.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rforearm", "lforearm", "rupperarm", "lupperarm", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Badly Lettered Name",
        "a badly lettered tattoo reading $template{name}",
        "A crudely lettered name is tattooed here in awkward uneven script, reading $template{name}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 6.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rwrist", "lwrist", "rupperarm", "lupperarm", "lback" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "name",
                18,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "Jenny",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "badly written lettering")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Crooked Crown",
        "a crooked crown tattoo",
        "A rough little crown is tattooed here in thick clumsy lines, its points uneven and its shape visibly off-centre.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 6.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["yellow"] = 1.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rtemple", "ltemple", "neck", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Drunk Smiley Face",
        "a drunk smiley-face tattoo",
        "A crude smiley face is tattooed here in childish, uneven strokes, one eye sitting higher than the other.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 3.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["yellow"] = 1.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rwrist", "lwrist", "rhip", "lhip", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Wonky Butterfly",
        "a wonky butterfly tattoo",
        "A badly done butterfly is tattooed here with mismatched wings, shaky lines, and blotchy fill.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["blue"] = 1.0,
            ["purple"] = 1.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rwrist", "lwrist", "lback", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Bad Tribal Band",
        "a bad tribal-band tattoo",
        "A supposed tribal band circles this spot in thick black shapes that fail to line up cleanly and wobble unevenly.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 8.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rupperarm", "lupperarm", "rforearm", "lforearm", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Shaky Spiderweb Elbow",
        "a shaky spiderweb tattoo",
        "A rough spiderweb tattoo sprawls over this elbow, its strands uneven and its rings badly spaced.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "relbow", "lelbow" }
    ),

    new SeederTattooTemplateDefinition(
        "Bent Horseshoe",
        "a bent horseshoe tattoo",
        "A small horseshoe is tattooed here in clumsy dark lines, its curve bent oddly and its nail holes uneven.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 5.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["brown"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Amateur Devil Face",
        "an amateur devil-face tattoo",
        "A crude devil face is tattooed here with a goofy grin, blunt little horns, and heavy amateur shading.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 8.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rhand", "lhand", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Crooked Neck Script",
        "a crooked neck tattoo reading $template{word}",
        "A short word is tattooed across the neck in ugly uneven lettering, reading $template{word}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "neck", "bneck" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "word",
                16,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "LOYAL",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "crooked lettering")
        },
        OverrideCharacteristicPlain: "badly-tattooed",
        OverrideCharacteristicWith: "with crude lettering tattooed on the neck"
    ),
        #endregion

        #region Stick and Poke
        new SeederTattooTemplateDefinition(
        "Stick-And-Poke Three Dots",
        "a three-dot stick-and-poke tattoo",
        "Three small dots are tattooed here in rough hand-poked ink, close-set and starkly simple.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 4.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rwrist", "lwrist", "reyesocket", "leyesocket" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Five Dots",
        "a five-dot stick-and-poke tattoo",
        "Five hand-poked dots are arranged here in a tiny quincunx pattern.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 4.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rwrist", "lwrist" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Small Cross",
        "a small stick-and-poke cross tattoo",
        "A small cross is tattooed here in sparse hand-poked dots and short dark lines.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 5.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "neck", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Barbed Wire",
        "a stick-and-poke barbed-wire tattoo",
        "A thin loop of barbed wire is tattooed here in simple, prickly hand-poked linework.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Spiderweb",
        "a stick-and-poke spiderweb tattoo",
        "A sparse spiderweb is tattooed here in thin radiating lines and uneven hand-poked rings.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "relbow", "lelbow", "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Razor Blade",
        "a stick-and-poke razor-blade tattoo",
        "A simple razor blade is tattooed here in stark hand-poked geometry.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rforearm", "lforearm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Dice",
        "a stick-and-poke dice tattoo",
        "A pair of tiny dice is tattooed here in hard-edged hand-poked outlines and dark pips.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 6.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rforearm", "lforearm", "rhip", "lhip" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Crown",
        "a stick-and-poke crown tattoo",
        "A tiny crown is tattooed here in sparse hand-poked strokes and simple points.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 6.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rtemple", "ltemple", "neck", "rforearm", "lforearm" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Tally Marks",
        "a stick-and-poke tally-mark tattoo",
        "A cluster of tally marks is tattooed here in thin hand-poked lines.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 5.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rthigh", "lthigh", "rshin", "lshin" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Initials",
        "a stick-and-poke tattoo reading $template{initials}",
        "Crude but deliberate initials are hand-poked here, reading $template{initials}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 5.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rwrist", "lwrist", "rforearm", "lforearm" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "initials",
                6,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "J.M.",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "tiny initials")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Date",
        "a stick-and-poke date tattoo reading $template{date}",
        "A short date is hand-poked here in plain, utilitarian lettering, reading $template{date}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 6.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["blue"] = 1.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rthigh", "lthigh", "lback" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "date",
                12,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "12.04.24",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "a short date")
        }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Tiny Dagger",
        "a stick-and-poke dagger tattoo",
        "A tiny dagger is tattooed here in spare hand-poked lines with a narrow blade and simple hilt.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rforearm", "lforearm", "rshin", "lshin" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Tiny Heart",
        "a stick-and-poke heart tattoo",
        "A tiny heart is hand-poked here in a simple, dark outline.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 4.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rhip", "lhip", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Small Star",
        "a stick-and-poke star tattoo",
        "A tiny star is tattooed here in hand-poked lines and sparse dark fill.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 5.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rhand", "lhand", "rwrist", "lwrist", "rtemple", "ltemple", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Stick-And-Poke Neck Word",
        "a stick-and-poke neck tattoo reading $template{word}",
        "A short word is hand-poked across the neck here, reading $template{word}.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 7.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "neck", "bneck" },
        TextSlots: new[]
        {
            new SeederTattooTextSlotDefinition(
                "word",
                12,
                DefaultLanguageName: "English",
                DefaultScriptName: "Latin",
                DefaultText: "LOYAL",
                RequiredCustomText: true,
                DefaultColourName: "black",
                DefaultAlternateText: "a short word")
        },
        OverrideCharacteristicPlain: "gang-marked",
        OverrideCharacteristicWith: "with hand-poked lettering on the neck"
    ),
        #endregion

        #region Line Art
        new SeederTattooTemplateDefinition(
        "Line-Art Rose",
        "a line-art rose tattoo",
        "A rose is tattooed here in clean, elegant line-art, its petals and stem defined without fill.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Lily",
        "a line-art lily tattoo",
        "A lily is tattooed here in graceful single-line floral contours and fine leaf detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rcalf", "lcalf", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Butterfly",
        "a line-art butterfly tattoo",
        "A butterfly is tattooed here in delicate, symmetrical linework with open wings and no fill.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rshoulder", "lshoulder", "rankle", "lankle", "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Moth",
        "a line-art moth tattoo",
        "A moth is tattooed here in crisp symmetrical line-art with fine wing patterning and a narrow body.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "lback", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Snake",
        "a line-art snake tattoo",
        "A snake winds here in clean continuous linework, its coils and head rendered with restrained precision.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Koi",
        "a line-art koi tattoo",
        "A koi fish is tattooed here in smooth, flowing line-art with finely defined fins and scales.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 20.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rthigh", "lthigh", "rcalf", "lcalf" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Swallow",
        "a line-art swallow tattoo",
        "A swallow in flight is tattooed here in swift, elegant line-art with outspread wings.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rforearm", "lforearm", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Mountain Range",
        "a line-art mountain tattoo",
        "A mountain range is tattooed here in clean horizon-like linework with sharp peaks and minimal detail.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "uback", "lback", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Wave",
        "a line-art wave tattoo",
        "A curling wave is tattooed here in fluid, single-colour line-art with graceful motion.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "rankle", "lankle", "rfoot", "lfoot" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Crescent Moon",
        "a line-art crescent-moon tattoo",
        "A crescent moon is tattooed here in spare, elegant line-art with a clean open curve.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 14.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 2.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "bneck", "rhand", "lhand", "rankle", "lankle" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Sun",
        "a line-art sun tattoo",
        "A stylised sun is tattooed here in crisp circular linework and narrow radiating rays.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rshoulder", "lshoulder", "rforearm", "lforearm", "rhand", "lhand" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Face Profile",
        "a line-art face-profile tattoo",
        "A minimalist face profile is tattooed here in a single elegant contour line.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rupperarm", "lupperarm", "rthigh", "lthigh" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Hands",
        "a line-art hands tattoo",
        "A pair of hands is tattooed here in clean contour linework, poised in a graceful, expressive gesture.",
        MinimumBodypartSize: SizeCategory.Small,
        MinimumSkill: 22.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0
        },
        BodypartAliases: new[] { "rforearm", "lforearm", "rthigh", "lthigh", "uback" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Botanical Sprig",
        "a line-art botanical tattoo",
        "A slender botanical sprig is tattooed here in clean, airy line-art with small leaves and stem detail.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 16.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rforearm", "lforearm", "rankle", "lankle", "rfoot", "lfoot" }
    ),

    new SeederTattooTemplateDefinition(
        "Line-Art Geometric Diamond",
        "a line-art geometric tattoo",
        "A geometric diamond motif is tattooed here in precise, balanced linework and open negative space.",
        MinimumBodypartSize: SizeCategory.Tiny,
        MinimumSkill: 18.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 3.0
        },
        BodypartAliases: new[] { "rwrist", "lwrist", "rhand", "lhand", "rforearm", "lforearm", "bneck" }
    ),
        #endregion

        #region Signature Pieces
        new SeederTattooTemplateDefinition(
        "Imperial Dragon Ascent",
        "an imperial dragon tattoo",
        "A magnificent dragon is tattooed here in a sweeping upward coil, its scaled body twisting through stylised cloud-bands and tongues of flame. Every claw, whisker, and plate of the creature's hide has been rendered with meticulous care, giving the whole piece a sense of motion and authority even at rest. The composition is bold and theatrical, but disciplined enough that the dense detail never collapses into muddle. It reads as the work of a master rather than a mere enthusiast.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 58.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["dark red"] = 2.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "uback", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Phoenix In Glory",
        "a phoenix tattoo",
        "A grand phoenix is tattooed here with wings flung wide and long tail-feathers trailing through ornamental flames. The feather work is exceptionally fine, shifting from broad structural lines into delicate internal detail that rewards close viewing. The whole design has been composed to feel radiant and rising, with the bird's body acting as the visual anchor and the flame-work carrying the eye outward. It is unapologetically showpiece tattooing.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 56.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["crimson"] = 2.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "uback", "belly" }
    ),

    new SeederTattooTemplateDefinition(
        "Storm Kraken",
        "a kraken tattoo",
        "A monstrous kraken is tattooed here in a dense composition of mantle, curling tentacles, and crashing stylised surf. The artist has used the full field of skin intelligently, letting the tentacles bend and interlock to create rhythm rather than chaos. Suckers, surface texture, and wave crests are all worked with extraordinary patience, producing a piece that feels both heavy and alive. It is the kind of tattoo that would dominate any description of the bodypart it occupies.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 60.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["dark blue"] = 2.0,
            ["silver grey"] = 1.0
        },
        BodypartAliases: new[] { "uback", "lback", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Celestial Mandala Masterwork",
        "a celestial mandala tattoo",
        "A vast celestial mandala is tattooed here in concentric rings of geometry, petals, stars, and carefully spaced linework. The piece balances ornate density with mathematical cleanliness, so that even its finest details remain legible instead of collapsing into ornament for ornament's sake. It gives the impression of ritual precision and contemplative symmetry rather than mere decoration. The execution is exacting enough that any wobble or weakness would immediately ruin it, yet none is visible.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 62.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["deep indigo"] = 1.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "belly" }
    ),

    new SeederTattooTemplateDefinition(
        "Saint In Radiant Iconography",
        "a saintly icon tattoo",
        "A haloed saint is tattooed here in a formal iconographic composition, surrounded by ornamental rays, drapery, and devotional framing elements. The facial rendering is careful and dignified rather than cartoonish, and the surrounding detail supports the central figure instead of competing with it. Fine lines, controlled shading, and judicious use of highlight tones give the whole piece a luminous quality. It is an ambitious religious work executed with impressive restraint.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 57.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["gold"] = 1.0,
            ["white"] = 1.0
        },
        BodypartAliases: new[] { "rbreast", "lbreast", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Archangel Judgement",
        "an archangel tattoo",
        "An armed archangel is tattooed here in a dramatic descending pose, with swept wings, layered armour, and a lowered sword. The composition makes excellent use of vertical movement, drawing the eye from the weapon through the torso and out into the wing structure. Feathers, folds, and polished surfaces are differentiated with real technical confidence rather than being flattened into generic dark fill. The result is imposing, theatrical, and unmistakably high-end work.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 61.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["silver grey"] = 1.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "rbreast", "lbreast" }
    ),

    new SeederTattooTemplateDefinition(
        "Battlefield Panorama",
        "a battlefield tattoo",
        "A panoramic battle scene is tattooed here, crowded with riders, banners, broken weapons, and drifting smoke. Despite the complexity, the artist has maintained a clear hierarchy of forms, with strong silhouettes and carefully grouped detail preventing the image from turning muddy. Secondary figures and background elements are still rendered with notable care, giving the impression of an entire story compressed into skin. It feels closer to an illustrated plate than to ordinary body art.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 65.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 6.0,
            ["brown"] = 1.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "uback", "lback", "belly" }
    ),

    new SeederTattooTemplateDefinition(
        "Japanese Wave And Koi Masterpiece",
        "a koi-and-wave tattoo",
        "A powerful koi is tattooed here amid sweeping wave forms, foam crests, and curling current lines that turn the whole composition into a single flowing mass. The fish itself is beautifully handled, with scale rows, fins, and facial structure rendered with disciplined detail instead of clutter. The water framing gives the piece movement and coherence, allowing it to feel much larger than the anatomy beneath it. It is a confident, mature work in a classic large-format style.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 54.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["dark blue"] = 2.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "rshoulderblade", "lshoulderblade", "abdomen", "belly" }
    ),

    new SeederTattooTemplateDefinition(
        "Tiger Through Bamboo",
        "a tiger tattoo",
        "A tiger is tattooed here in a prowling, half-turned pose, its musculature and striped hide emerging through a frame of bamboo and leaves. The anatomy is observed with unusual confidence, giving the beast real weight and coiled tension rather than a generic big-cat silhouette. The surrounding foliage is not filler; it is arranged to break the figure, frame the head, and keep the eye moving through the design. The whole tattoo feels deliberate, expensive, and expertly judged.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 55.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["orange"] = 2.0,
            ["green"] = 1.0
        },
        BodypartAliases: new[] { "rbreast", "lbreast", "abdomen", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Serpent Garden Tableau",
        "a serpent-garden tattoo",
        "A great serpent is tattooed here in looping coils through flowers, leaves, and carved ornamental borders. The artist has made the coils do real compositional work, dividing the space into rich chambers of detail without making the overall piece feel fragmented. Scales, blossoms, and negative space have all been used with intelligence, giving the design both density and breath. It is decorative in the best sense: lush, complex, and under precise control.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 57.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["emerald green"] = 1.0,
            ["red"] = 1.0
        },
        BodypartAliases: new[] { "abdomen", "belly", "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Twin Peacocks Ornamental Backpiece",
        "a twin-peacocks tattoo",
        "Two peacocks are tattooed here in a mirrored ornamental arrangement, their bodies elegant and their tail-feathers exploding into elaborate eye-marked fans. The feather treatment is lavish but controlled, with repeating motifs varied just enough to avoid mechanical stiffness. The birds read as noble centrepieces while the tails carry the grandeur of the whole design outward across the field. It is ostentatious work, but executed with enough discipline to remain tasteful.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 63.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 4.0,
            ["teal blue"] = 2.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "uback", "lback" }
    ),

    new SeederTattooTemplateDefinition(
        "Sacred Heart Baroque Chestpiece",
        "a baroque sacred-heart tattoo",
        "A sacred heart is tattooed here as the centre of a grand baroque composition, surrounded by rays, scrollwork, thorns, and devotional flourishes. The heart itself is rendered richly enough to anchor the piece, while the framing ornament gives it ceremonial weight and a sense of old-world artistry. Nothing about the design feels incidental; every curl and flare has been placed to support the visual hierarchy. It is a chestpiece meant to be noticed and remembered.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 59.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["crimson"] = 2.0,
            ["gold"] = 1.0
        },
        BodypartAliases: new[] { "rbreast", "lbreast", "abdomen" }
    ),

    new SeederTattooTemplateDefinition(
        "Cosmic Observatory",
        "a cosmic tattoo",
        "A cosmic observatory scene is tattooed here, combining stars, planetary arcs, instrument rings, and sweeping clouds into a grand astronomical composition. The linework is extraordinarily clean for so busy a design, with each circular element maintaining its integrity instead of drifting off true. Fine accent work in the background creates depth without muddying the main forms. The result feels intellectual, mysterious, and intensely deliberate.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 64.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["deep blue"] = 1.0,
            ["silver grey"] = 1.0
        },
        BodypartAliases: new[] { "uback", "abdomen", "belly" }
    ),

    new SeederTattooTemplateDefinition(
        "Warrior Queen Portrait",
        "a warrior-queen portrait tattoo",
        "A crowned warrior queen is tattooed here in a commanding portrait, framed by armour details, drapery, and ornamental motifs that elevate the piece beyond simple portraiture. The face is rendered with enough confidence to carry expression, while the surrounding details give the composition narrative weight and grandeur. The shading is controlled and elegant, never heavy-handed. It reads as a prestige commission rather than ordinary flash.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 60.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 5.0,
            ["gold"] = 1.0,
            ["dark red"] = 1.0
        },
        BodypartAliases: new[] { "rbreast", "lbreast", "rshoulderblade", "lshoulderblade" }
    ),

    new SeederTattooTemplateDefinition(
        "Infernal Cathedral",
        "an infernal cathedral tattoo",
        "A nightmarish infernal cathedral is tattooed here in spires, arches, flames, and looming demonic ornament. The artist has packed the space with gothic structure and infernal symbolism, yet the perspective and silhouette remain clear enough that the whole image still reads powerfully at a glance. Fine architectural detailing, smoke, and ember-like accents enrich the piece without breaking its coherence. It is dark, excessive, and extremely skilful work.",
        MinimumBodypartSize: SizeCategory.Normal,
        MinimumSkill: 66.0,
        InkColours: new Dictionary<string, double>
        {
            ["black"] = 6.0,
            ["dark red"] = 2.0,
            ["orange"] = 1.0
        },
        BodypartAliases: new[] { "uback", "lback", "abdomen" }
    ),
        #endregion
    ];

    private static readonly IReadOnlyList<SeederScarTemplateDefinition> HumanScarTemplates = [];

    internal static IReadOnlyList<SeederTattooTemplateDefinition> TattooTemplatesForTesting => HumanTattooTemplates;
    internal static IReadOnlyList<SeederScarTemplateDefinition> ScarTemplatesForTesting => HumanScarTemplates;

    private void SeedHumanDisfigurementTemplates(BodyProto body)
    {
        SeederDisfigurementTemplateUtilities.SeedTemplates(
            _context,
            body,
            HumanTattooTemplates,
            HumanScarTemplates);
    }

    private static bool HasMissingHumanDisfigurementTemplates(FuturemudDatabaseContext context)
    {
        return SeederDisfigurementTemplateUtilities.HasMissingDefinitions(
            context,
            HumanTattooTemplates,
            HumanScarTemplates);
    }
}

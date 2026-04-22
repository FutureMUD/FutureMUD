#nullable enable

using MudSharp.Body;
using MudSharp.Form.Characteristics;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class MythicalAnimalSeeder
{
    internal sealed record MythicalAgeProfile(
        int ChildAge,
        int YouthAge,
        int YoungAdultAge,
        int AdultAge,
        int ElderAge,
        int VenerableAge
    );

    internal sealed record MythicalAttackTemplate(
        string AttackName,
        ItemQuality Quality,
        IReadOnlyList<string> BodypartAliases
    );

    internal sealed record MythicalBodypartUsageTemplate(
        string BodypartAlias,
        string Usage
    );

    internal sealed record MythicalCharacteristicTemplate(
        string DefinitionName,
        IReadOnlyList<string> Values,
        string Usage = "base",
        CharacteristicType Type = CharacteristicType.Standard
    );

    internal sealed record MythicalRaceTemplate(
        string Name,
        string BodyKey,
        SizeCategory Size,
        string MaleHeightWeightModel,
        string FemaleHeightWeightModel,
        MythicalAgeProfile AgeProfile,
        NonHumanAttributeProfile AttributeProfile,
        double BodypartHealthMultiplier,
        bool HumanoidVariety,
        bool CanUseWeapons,
        bool CanClimb,
        bool CanSwim,
        bool Playable,
        string Description,
        string RoleDescription,
        IReadOnlyList<StockDescriptionVariant>? DescriptionVariants,
        IReadOnlyList<MythicalAttackTemplate> Attacks,
        IReadOnlyList<MythicalBodypartUsageTemplate>? BodypartUsages = null,
        IReadOnlyList<string>? PersonWords = null,
        string? FacialHairProfileName = null,
        IReadOnlyList<MythicalCharacteristicTemplate>? AdditionalCharacteristics = null,
        IReadOnlyList<StockDescriptionVariant>? OverlayDescriptionVariants = null,
        string CombatStrategyKey = "Beast Brawler",
        IReadOnlyList<SeederTattooTemplateDefinition>? TattooTemplates = null
    );

    internal static IReadOnlyDictionary<string, MythicalRaceTemplate> TemplatesForTesting => Templates;

    private static readonly IReadOnlyDictionary<string, MythicalRaceTemplate> Templates =
        new ReadOnlyDictionary<string, MythicalRaceTemplate>(
            BuildTemplates()
        );

    private static Dictionary<string, MythicalRaceTemplate> BuildTemplates()
    {
        static MythicalAgeProfile StandardHumanoid()
        {
            return new(2, 6, 12, 18, 55, 80);
        }

        static MythicalAgeProfile LongLivedHumanoid()
        {
            return new(4, 10, 20, 35, 120, 180);
        }

        static MythicalAgeProfile GreatBeast()
        {
            return new(2, 8, 16, 30, 120, 200);
        }

        static MythicalAgeProfile Beast()
        {
            return new(1, 4, 8, 16, 40, 70);
        }

        static MythicalAttackTemplate Attack(string name, ItemQuality quality, params string[] aliases)
        {
            return new(name, quality, aliases);
        }

        static MythicalBodypartUsageTemplate Usage(string alias, string usage)
        {
            return new(alias, usage);
        }

        static MythicalCharacteristicTemplate Characteristic(string name, params string[] values)
        {
            return new(name, values);
        }

        static NonHumanAttributeProfile Stats(int strength, int constitution, int agility, int dexterity)
        {
            return new(strength, constitution, agility, dexterity);
        }

        static IReadOnlyList<StockDescriptionVariant> Variants(
                    params (string ShortDescription, string FullDescription)[] variants)
        {
            return SeederDescriptionHelpers.BuildVariantList(variants);
        }

        static MythicalRaceTemplate BeastRace(
                    string name,
                    string bodyKey,
                    SizeCategory size,
                    string model,
                    MythicalAgeProfile ageProfile,
                    string description,
                    string roleDescription,
                    IReadOnlyList<StockDescriptionVariant> descriptionVariants,
                    IReadOnlyList<MythicalAttackTemplate> attacks,
                    IReadOnlyList<MythicalBodypartUsageTemplate>? usages = null,
                    NonHumanAttributeProfile? attributeProfile = null,
                    double bodypartHealthMultiplier = 1.0,
                    bool canClimb = false,
                    bool canSwim = true,
                    bool playable = false,
                    IReadOnlyList<MythicalCharacteristicTemplate>? additionalCharacteristics = null,
                    string combatStrategyKey = "Beast Brawler")
        {
            return new(
                        name,
                        bodyKey,
                        size,
                        model,
                        model,
                        ageProfile,
                        attributeProfile ?? Stats(0, 0, 0, 0),
                        bodypartHealthMultiplier,
                        false,
                        false,
                        canClimb,
                        canSwim,
                        playable,
                        description,
                        roleDescription,
                        descriptionVariants,
                        attacks,
                        usages,
                        null,
                        null,
                        additionalCharacteristics,
                        CombatStrategyKey: combatStrategyKey
                    );
        }

        static MythicalRaceTemplate HumanoidRace(
                    string name,
                    string bodyKey,
                    SizeCategory size,
                    string maleModel,
                    string femaleModel,
                    MythicalAgeProfile ageProfile,
                    string description,
                    string roleDescription,
                    IReadOnlyList<MythicalAttackTemplate> attacks,
                    IReadOnlyList<string> personWords,
                    IReadOnlyList<StockDescriptionVariant>? overlayDescriptionVariants = null,
                    IReadOnlyList<MythicalBodypartUsageTemplate>? usages = null,
                    NonHumanAttributeProfile? attributeProfile = null,
                    double bodypartHealthMultiplier = 1.0,
                    bool canClimb = false,
                    bool canSwim = true,
                    string? facialHairProfile = null,
                    string combatStrategyKey = "Melee (Auto)")
        {
            return new(
                        name,
                        bodyKey,
                        size,
                        maleModel,
                        femaleModel,
                        ageProfile,
                        attributeProfile ?? Stats(0, 0, 0, 0),
                        bodypartHealthMultiplier,
                        true,
                        true,
                        canClimb,
                        canSwim,
                        true,
                        description,
                        roleDescription,
                        null,
                        attacks,
                        usages,
                        personWords,
                        facialHairProfile,
                        null,
                        overlayDescriptionVariants,
                        CombatStrategyKey: combatStrategyKey
                    );
        }

        static MythicalRaceTemplate SapientRace(
                    string name,
                    string bodyKey,
                    SizeCategory size,
                    string maleModel,
                    string femaleModel,
                    MythicalAgeProfile ageProfile,
                    string description,
                    string roleDescription,
                    IReadOnlyList<StockDescriptionVariant> descriptionVariants,
                    IReadOnlyList<MythicalAttackTemplate> attacks,
                    IReadOnlyList<MythicalBodypartUsageTemplate>? usages = null,
                    NonHumanAttributeProfile? attributeProfile = null,
                    double bodypartHealthMultiplier = 1.0,
                    bool canClimb = false,
                    bool canSwim = true,
                    IReadOnlyList<MythicalCharacteristicTemplate>? additionalCharacteristics = null,
                    string combatStrategyKey = "Melee (Auto)")
        {
            return new(
                        name,
                        bodyKey,
                        size,
                        maleModel,
                        femaleModel,
                        ageProfile,
                        attributeProfile ?? Stats(0, 0, 0, 0),
                        bodypartHealthMultiplier,
                        false,
                        true,
                        canClimb,
                        canSwim,
                        true,
                        description,
                        roleDescription,
                        descriptionVariants,
                        attacks,
                        usages,
                        null,
                        null,
                        additionalCharacteristics,
                        CombatStrategyKey: combatStrategyKey
                    );
        }

        return new Dictionary<string, MythicalRaceTemplate>(StringComparer.OrdinalIgnoreCase)
        {
            ["Dragon"] = BeastRace(
                "Dragon",
                "Toed Quadruped",
                SizeCategory.VeryLarge,
                "Horse",
                GreatBeast(),
                "Dragons are immense, winged reptiles with claws, horns and a powerful tail.",
                "In most worlds dragons stand apart from ordinary beasts, looming over landscapes as apex predators, hoard-lords, or living catastrophes that lesser peoples must placate, evade, or confront with great preparation.",
                Variants(
                    ("a horn-crowned dragon",
                        "This colossal draconic beast combines a heavily muscled quadrupedal frame with broad wings, curving horns and a predator's jaws."),
                    ("a vast scale-armoured dragon",
                        "This immense reptile wears overlapping scales across a barrel chest and a sinuous tail, its whole frame built for terror, endurance and sudden violence.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Legendary, "mouth"),
                    Attack("Dragonfire Breath", ItemQuality.Legendary, "mouth"),
                    Attack("Bite", ItemQuality.Good, "mouth"),
                    Attack("Claw Swipe", ItemQuality.Great, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
                    Attack("Horn Gore", ItemQuality.Great, "rhorn", "lhorn"),
                    Attack("Tail Slap", ItemQuality.Good, "ltail"),
                    Attack("Wing Buffet", ItemQuality.Good, "rwingbase", "lwingbase")
                ],
                [
                    Usage("rhorn", "general"),
                    Usage("lhorn", "general"),
                    Usage("rwingbase", "general"),
                    Usage("lwingbase", "general"),
                    Usage("rwing", "general"),
                    Usage("lwing", "general")
                ],
                attributeProfile: Stats(12, 11, 0, -2),
                bodypartHealthMultiplier: 2.4,
                additionalCharacteristics:
                [
                    Characteristic("Scale Colour", "red", "green", "black", "gold")
                ],
                combatStrategyKey: "Beast Artillery"
            ),
            ["Griffin"] = BeastRace(
                "Griffin",
                "Griffin",
                SizeCategory.Large,
                "Big Felid",
                Beast(),
                "Griffins combine an eagle's head and wings with a leonine hindbody and foreclaws.",
                "Griffins often fill the role of proud sky-hunters and sacred or royal beasts, creatures that dominate high crags and open skies while also appearing in heraldry, legend and elite riding traditions.",
                Variants(
                    ("a hooked-beaked griffin",
                        "This imposing mythic predator bears a hooked avian beak and far-seeing eyes above a leonine quadruped body built for bounding flight and savage pounces."),
                    ("a broad-winged griffin",
                        "This hybrid beast balances raptorial foreparts and powerful leonine hindquarters, giving it the look of something bred equally for altitude and brutal close attack.")
                ),
                [
                    Attack("Beak Peck", ItemQuality.Good, "beak"),
                    Attack("Beak Bite", ItemQuality.Standard, "beak"),
                    Attack("Claw Swipe", ItemQuality.Good, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
                    Attack("Tail Slap", ItemQuality.Standard, "ltail"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                attributeProfile: Stats(7, 6, 3, 1),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Hippogriff"] = BeastRace(
                "Hippogriff",
                "Hippogriff",
                SizeCategory.Large,
                "Horse",
                Beast(),
                "Hippogriffs blend an eagle's forequarters and wings with an equine lower body.",
                "Hippogriffs often occupy the boundary between wild mount and dangerous aerial grazer, prized by riders who can tame them and feared by travellers who mistake their grace for docility.",
                Variants(
                    ("a keen-eyed hippogriff",
                        "This powerful hybrid has an avian head and beating wings set atop an equine frame, with hoofed legs built equally for galloping starts and airborne strikes."),
                    ("a feather-crested hippogriff",
                        "This large chimera marries a raptor's forequarters to an equine body, giving it both a war-mount's breadth and a predator's abrupt, striking violence.")
                ),
                [
                    Attack("Beak Peck", ItemQuality.Standard, "beak"),
                    Attack("Beak Bite", ItemQuality.Poor, "beak"),
                    Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                attributeProfile: Stats(6, 5, 2, 0),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Unicorn"] = BeastRace(
                "Unicorn",
                "Ungulate",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                "Unicorns are horse-like beings distinguished by a single spiralled horn and uncanny grace.",
                "Unicorns are usually treated less as livestock than as numinous presences, creatures tied to sanctuaries, omens, purity traditions, and stories in which the worthy may approach what the crude can never catch.",
                Variants(
                    ("a spiralled-horn unicorn",
                        "This elegant equine myth-beast carries itself with the poise of a fine horse, its singular horn and bright, intelligent eyes marking it as something far stranger."),
                    ("a moon-bright unicorn",
                        "This horned beast looks horse-like at a glance, yet its unnerving composure and faultless carriage make it seem more like an embodied blessing than an animal.")
                ),
                [
                    Attack("Bite", ItemQuality.Bad, "mouth"),
                    Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
                    Attack("Horn Gore", ItemQuality.Good, "horn")
                ],
                [
                    Usage("horn", "general")
                ],
                attributeProfile: Stats(7, 6, 2, 0),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Behemoth"
            ),
            ["Pegasus"] = BeastRace(
                "Pegasus",
                "Ungulate",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                "Pegasi are winged horses capable of powerful, sustained flight.",
                "Pegasi are commonly imagined as noble aerial mounts and heraldic wonders, but in the wild they remain strong-willed creatures whose flight ranges and herd instincts make them difficult to manage.",
                Variants(
                    ("a feather-winged pegasus",
                        "This broad-winged equine is all coiled athletic power, its feathered wings and strong hooves making it as dangerous in the air as on the ground."),
                    ("a sky-bred pegasus",
                        "This winged horse combines a rider's familiar silhouette with the deep chest, clean limbs and flight muscles of a creature meant to launch hard and stay aloft.")
                ),
                [
                    Attack("Bite", ItemQuality.Bad, "mouth"),
                    Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
                    Attack("Head Ram", ItemQuality.Standard, "head"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                [
                    Usage("rwingbase", "general"),
                    Usage("lwingbase", "general"),
                    Usage("rwing", "general"),
                    Usage("lwing", "general")
                ],
                attributeProfile: Stats(6, 5, 3, 0),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Warg"] = BeastRace(
                "Warg",
                "Toed Quadruped",
                SizeCategory.Large,
                "Large Canid",
                GreatBeast(),
                "Wargs are oversized, wolf-like predators bred toward savagery, endurance and a frightening, almost deliberate malice.",
                "Wargs usually occupy the role of war-beasts and terror-hounds, creatures associated with raiders, dark hunts and borderland fear rather than with any tame place in ordinary husbandry.",
                Variants(
                    ("a long-fanged warg",
                        "This huge canid carries the rangy, killing build of a wolf pushed beyond any natural limit, with long fangs, dense muscle and a stare full of ugly purpose."),
                    ("a battle-scarred warg",
                        "This brutal wolf-beast looks bred for pursuit and slaughter, its broad paws, heavy shoulders and predatory focus making it feel more like a campaign than an animal.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Claw Swipe", ItemQuality.Standard, "rfpaw", "lfpaw", "rrpaw", "lrpaw")
                ],
                attributeProfile: Stats(6, 5, 2, 0),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Dire-Wolf"] = BeastRace(
                "Dire-Wolf",
                "Toed Quadruped",
                SizeCategory.VeryLarge,
                "Large Canid",
                GreatBeast(),
                "Dire-wolves are colossal wolf-beasts, broader, heavier and markedly more dangerous than even wargs.",
                "Dire-wolves fill the mythic niche of winter pack-lords and devastating pursuit predators, the sort of beasts that turn tracks, howls and dark tree lines into immediate threats.",
                Variants(
                    ("a hulking dire-wolf",
                        "This immense wolf-beast has a chest like a battering ram and a head heavy with killing jaws, every part of it scaled toward overpowering prey by force."),
                    ("a frost-maned dire-wolf",
                        "This monstrous canid looks like a wolf reimagined as a siege animal, with great paws, savage jaws and the mass to bowl lesser creatures over outright.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.VeryGood, "mouth"),
                    Attack("Bite", ItemQuality.Good, "mouth"),
                    Attack("Claw Swipe", ItemQuality.Standard, "rfpaw", "lfpaw", "rrpaw", "lrpaw")
                ],
                attributeProfile: Stats(8, 7, 2, 0),
                bodypartHealthMultiplier: 1.95,
                combatStrategyKey: "Beast Brawler"
            ),
            ["Dire-Bear"] = BeastRace(
                "Dire-Bear",
                "Toed Quadruped",
                SizeCategory.VeryLarge,
                "Bear",
                GreatBeast(),
                "Dire-bears are towering ursine horrors of immense mass, deep fur and ruinous claw strength.",
                "Dire-bears usually stand in myth as ancient den-lords, forest tyrants and unstoppable wilderness threats whose mere presence can empty roads, camps and frontier holdings.",
                Variants(
                    ("a mountain-sized dire-bear",
                        "This gigantic bear looms like a moving wall of fur and shoulder power, its paws broad enough to break bone and its jaws built for finishing what the claws begin."),
                    ("a scar-heavy dire-bear",
                        "This monstrous bear wears old scars beneath its thick coat, its vast frame and crushing forelimbs making it look less like an animal than a mobile disaster.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.VeryGood, "mouth"),
                    Attack("Bite", ItemQuality.Good, "mouth"),
                    Attack("Claw Swipe", ItemQuality.VeryGood, "rfpaw", "lfpaw", "rrpaw", "lrpaw")
                ],
                attributeProfile: Stats(10, 9, 0, -1),
                bodypartHealthMultiplier: 2.2,
                combatStrategyKey: "Beast Behemoth"
            ),
            ["Minotaur"] = HumanoidRace(
                "Minotaur",
                "Horned Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                StandardHumanoid(),
                "Minotaurs are broad, horned humanoids with a bestial cast to their features and physiques.",
                "Minotaurs are commonly cast as warriors, guardians and dwellers of enclosed strongholds, their size and imposing presence making them natural figures of labour, soldiery and intimidation in mythic societies.",
                [
                    Attack("Horn Gore", ItemQuality.Standard, "rhorn", "lhorn"),
                    Attack("Head Ram", ItemQuality.Standard, "head"),
                    Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
                ],
                ["minotaur"],
                Variants(
                    ("a horned minotaur", "This horned minotaur has the towering frame and bull-cast features typical of the race, with a heavy brow, broad muzzle and deep chest."),
                    ("a broad-shouldered minotaur", "This minotaur's build is massively robust, its horned head and thick musculature giving it the intimidating silhouette of a born charger.")
                ),
                [
                    Usage("rhorn", "general"),
                    Usage("lhorn", "general")
                ],
                attributeProfile: Stats(5, 4, -1, -1),
                bodypartHealthMultiplier: 1.2
            ),
            ["Eastern Dragon"] = BeastRace(
                "Eastern Dragon",
                "Eastern Dragon",
                SizeCategory.VeryLarge,
                "Horse",
                GreatBeast(),
                "Eastern dragons are long, sinuous drakes that prowl on four clawed limbs without relying on wings for flight.",
                "Eastern dragons often appear as imperial, spiritual or elemental beings rather than mere monsters, entwined with rivers, weather, dynasties and the idea of ancient, watchful power.",
                Variants(
                    ("a long-bodied eastern dragon",
                        "This immense draconic predator combines a serpentine cast and a powerful quadrupedal frame, with taloned feet, a long body and a sweeping tail in place of wings."),
                    ("a whiskered eastern dragon",
                        "This drake's elongated body and flowing profile give it a more sinuous majesty than a western dragon, though its claws, jaws and sheer size remain openly dangerous.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Legendary, "mouth"),
                    Attack("Dragonfire Breath", ItemQuality.Legendary, "mouth"),
                    Attack("Bite", ItemQuality.Good, "mouth"),
                    Attack("Claw Swipe", ItemQuality.Great, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
                    Attack("Tail Slap", ItemQuality.Good, "ltail")
                ],
                attributeProfile: Stats(11, 10, 1, -1),
                bodypartHealthMultiplier: 2.3,
                additionalCharacteristics:
                [
                    Characteristic("Scale Colour", "red", "green", "black", "gold")
                ],
                combatStrategyKey: "Beast Artillery"
            ),
            ["Naga"] = HumanoidRace(
                "Naga",
                "Naga",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Naga are humanoid from the waist up, with serpentine lower bodies and a sinuous, coiled bearing.",
                "Naga usually occupy the role of riverine nobles, temple guardians or old marsh powers, creatures whose poise and patience make them feel as political and ceremonial as they are dangerous.",
                [
                    Attack("Carnivore Bite", ItemQuality.Standard, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Tail Slap", ItemQuality.Standard, "tail")
                ],
                ["naga"],
                overlayDescriptionVariants: Variants(
                    ("a coiled naga", "This naga presents a recognisably humanoid upper torso above a long serpentine lower body, the whole figure poised in smooth, deliberate coils."),
                    ("a serpent-bodied naga", "This naga's human-like arms and shoulders rise from a scaled, sinuous body whose coiling strength and low centre of gravity suggest sudden violence.")
                ),
                attributeProfile: Stats(1, 2, 1, 1),
                bodypartHealthMultiplier: 1.15,
                canClimb: true,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Mermaid"] = HumanoidRace(
                "Mermaid",
                "Mermaid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Merfolk have human torsos and arms paired with powerful piscine tails built for swimming.",
                "Merfolk usually fill the mythic niche of sea-dwellers, reef guardians and shoreline enigmas, engaging with landfolk through trade, song, rescue, raiding or careful distance depending on the setting.",
                [
                    Attack("Carnivore Bite", ItemQuality.Bad, "mouth"),
                    Attack("Bite", ItemQuality.Bad, "mouth"),
                    Attack("Tail Slap", ItemQuality.Good, "caudalfin")
                ],
                ["merfolk"],
                overlayDescriptionVariants: Variants(
                    ("a fin-tailed merfolk", "This merfolk body combines a humanoid upper torso with a powerful scaled tail, built more for darting turns and long swims than for any life on land."),
                    ("a sea-borne merfolk", "This merfolk's shoulders and arms are recognisably person-like, but the gleam of scales and the muscular sweep of the tail place them firmly in the water's domain.")
                ),
                attributeProfile: Stats(0, 1, 1, 1),
                bodypartHealthMultiplier: 1.0,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Manticore"] = BeastRace(
                "Manticore",
                "Manticore",
                SizeCategory.Large,
                "Big Felid",
                GreatBeast(),
                "Manticores are broad-winged leonine predators with a venomous tail-spike.",
                "Manticores occupy the role of nightmare apex predators, the sort of thing that turns caravan routes, borderlands and lonely heights into places where every rumour deserves to be believed.",
                Variants(
                    ("a stinger-tailed manticore",
                        "This winged predator couples a leonine quadruped body and raking claws with a barbed stinger poised at the end of its tail."),
                    ("a broad-winged manticore",
                        "This leonine chimera looks built to overwhelm prey by stages, first with speed and claws, then with its tail-spike and crushing mass.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Claw Swipe", ItemQuality.Good, "rfpaw", "lfpaw", "rrpaw", "lrpaw"),
                    Attack("Tail Slap", ItemQuality.Good, "ltail"),
                    Attack("Tail Spike", ItemQuality.Good, "stinger"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                [
                    Usage("rwingbase", "general"),
                    Usage("lwingbase", "general"),
                    Usage("rwing", "general"),
                    Usage("lwing", "general"),
                    Usage("stinger", "general")
                ],
                attributeProfile: Stats(8, 6, 2, 0),
                bodypartHealthMultiplier: 1.8,
                combatStrategyKey: "Beast Artillery"
            ),
            ["Wyvern"] = BeastRace(
                "Wyvern",
                "Wyvern",
                SizeCategory.Large,
                "Raptor",
                GreatBeast(),
                "Wyverns are draconic two-legged fliers, all leathery wings, grasping talons and snapping jaws.",
                "Wyverns are often treated as brutal cousins to true dragons, less regal but no less feared, and they commonly dominate cliffs, ruins and badlands where flight and aggression decide territory.",
                Variants(
                    ("a taloned wyvern",
                        "This draconic flyer stands on powerful taloned legs beneath a scaled torso, its jaws and whipping tail making close quarters especially dangerous."),
                    ("a leathery-winged wyvern",
                        "This wyvern's body is leaner and more openly predatory than a true dragon's, giving it the look of something evolved purely to dive, seize and tear.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
                    Attack("Dragonfire Breath", ItemQuality.Good, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Talon Strike", ItemQuality.Good, "rtalons", "ltalons"),
                    Attack("Tail Slap", ItemQuality.Standard, "tail"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                attributeProfile: Stats(8, 6, 2, 0),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Artillery"
            ),
            ["Phoenix"] = BeastRace(
                "Phoenix",
                "Avian",
                SizeCategory.Normal,
                "Raptor",
                GreatBeast(),
                "Phoenixes are radiant birds of fire and ash, here seeded without any resurrection-specific mechanics.",
                "Phoenixes fill the symbolic role of sacred fire, omen and renewal, but even stripped of miraculous mechanics they remain rare, intimidating creatures whose beauty does not make them safe.",
                Variants(
                    ("a radiant phoenix",
                        "This majestic firebird has an avian frame and proud bearing, its whole presence suggesting heat, renewal and dangerous beauty."),
                    ("an ember-bright phoenix",
                        "This great bird carries itself like a raptor haloed by furnace light, each movement suggesting heat, splendour and peril in equal measure.")
                ),
                [
                    Attack("Beak Peck", ItemQuality.Good, "beak"),
                    Attack("Beak Bite", ItemQuality.Standard, "beak"),
                    Attack("Talon Strike", ItemQuality.Good, "rtalons", "ltalons")
                ],
                attributeProfile: Stats(3, 3, 4, 2),
                bodypartHealthMultiplier: 1.1,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Basilisk"] = BeastRace(
                "Basilisk",
                "Serpentine",
                SizeCategory.Normal,
                "Serpent",
                GreatBeast(),
                "Basilisks are immense, sinister serpents famed for their malignant aspect and deadly bite.",
                "Basilisks are classic terror-beasts of ruins, tombs and desolate places, remembered less as animals to study than as lurking calamities whose presence can poison whole stretches of land or memory.",
                Variants(
                    ("a heavy-coiled basilisk",
                        "This huge serpent drapes itself in heavy coils and watches with an unsettling, predatory stillness that makes its sudden strikes all the worse."),
                    ("a malign-eyed basilisk",
                        "This monstrous serpent looks thicker, older and more deliberate than any natural snake, as though sheer age and venom had made it into something mythic.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Tail Slap", ItemQuality.Standard, "tail")
                ],
                attributeProfile: Stats(6, 6, 1, 0),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Clincher"
            ),
            ["Cockatrice"] = BeastRace(
                "Cockatrice",
                "Avian",
                SizeCategory.Small,
                "Small Bird",
                Beast(),
                "Cockatrices are vicious little reptilian birds with pecking beaks and slashing talons.",
                "Cockatrices fill the ecological niche of invasive, spiteful scavenger-predators in many stories, and their small size only makes them more troublesome around farms, granaries and rocky ruins.",
                Variants(
                    ("a reptilian cockatrice",
                        "This wiry, ill-tempered creature has an avian body and a reptilian cast to its features, all sharp beak, clawed feet and restless hostility."),
                    ("a spiteful cockatrice",
                        "This little monster looks like a bad dream of bird and lizard combined, with thin legs, a stabbing beak and a temperament that promises trouble.")
                ),
                [
                    Attack("Beak Peck", ItemQuality.Standard, "beak"),
                    Attack("Beak Bite", ItemQuality.Terrible, "beak"),
                    Attack("Talon Strike", ItemQuality.Standard, "rtalons", "ltalons")
                ],
                attributeProfile: Stats(0, 0, 3, 1),
                bodypartHealthMultiplier: 0.7,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Giant Beetle"] = BeastRace(
                "Giant Beetle",
                "Beetle",
                SizeCategory.Large,
                "Giant Insect",
                GreatBeast(),
                "Giant beetles are heavily armoured insects enlarged to the scale of mounts or siege vermin, with crushing mandibles and hard chitin plates.",
                "Giant beetles commonly fill the role of living battering creatures, dungeon vermin or uncanny beasts of burden, valued and feared for the same brute resilience that makes them hard to kill.",
                Variants(
                    ("a plate-backed giant beetle",
                        "This enormous beetle moves beneath a gleaming shell of layered chitin, its weighty body and crushing mandibles making every step feel deliberate and dangerous."),
                    ("a horned giant beetle",
                        "This outsized insect looks more like a walking cuirass than a living thing, its hard shell and snapping mouthparts suggesting endurance first and aggression second.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mandibles"),
                    Attack("Bite", ItemQuality.Standard, "mandibles")
                ],
                attributeProfile: Stats(5, 6, 0, -2),
                bodypartHealthMultiplier: 1.8,
                combatStrategyKey: "Beast Behemoth"
            ),
            ["Giant Ant"] = BeastRace(
                "Giant Ant",
                "Insectoid",
                SizeCategory.Large,
                "Giant Insect",
                GreatBeast(),
                "Giant ants are outsized eusocial insects with chitinous bodies, crushing mandibles and the relentless purpose of a colony made monstrous.",
                "Giant ants commonly fill the niche of hive-tunnel vermin, uncanny labour-beasts and living siege pests, creatures whose discipline and numbers can make even simple burrows feel militarised.",
                Variants(
                    ("an iron-jawed giant ant",
                        "This enormous ant looks built around work and violence in equal measure, its segmented body and oversized mandibles giving every movement a blunt sense of purpose."),
                    ("a tunnel-bred giant ant",
                        "This giant insect advances with the tireless determination of a colony thing enlarged beyond reason, antennae twitching above crushing jaws and a hard, purposeful frame.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mandibles"),
                    Attack("Bite", ItemQuality.Standard, "mandibles")
                ],
                attributeProfile: Stats(6, 6, 2, -2),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Clincher"
            ),
            ["Giant Mantis"] = BeastRace(
                "Giant Mantis",
                "Insectoid",
                SizeCategory.Large,
                "Giant Insect",
                GreatBeast(),
                "Giant mantises are towering predatory insects whose grasping forelegs and triangular heads make them look like ambush made flesh.",
                "Giant mantises usually occupy the niche of patient killer-beasts and temple-garden horrors, creatures remembered for stillness, sudden violence and an unnerving impression of intent.",
                Variants(
                    ("a scythe-limbed giant mantis",
                        "This massive mantis stands high on its rear legs with grasping forelimbs folded close, its narrow head and poised body radiating murderous patience."),
                    ("a leaf-green giant mantis",
                        "This towering insect looks all angles and restraint until it moves, the hooked forelegs and predatory posture promising a frighteningly fast strike.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Standard, "mandibles"),
                    Attack("Bite", ItemQuality.Standard, "mandibles"),
                    Attack("Claw Swipe", ItemQuality.Good, "rleg1", "lleg1")
                ],
                attributeProfile: Stats(6, 5, 4, 1),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Giant Spider"] = BeastRace(
                "Giant Spider",
                "Arachnid",
                SizeCategory.Large,
                "Giant Arachnid",
                GreatBeast(),
                "Giant spiders are monstrous web-spinners with long stabbing legs, clustered eyes and fangs large enough to punch through armour gaps.",
                "Giant spiders commonly fill the niche of ambush horrors and lair-predators, creatures that turn ruins, caverns and forest canopies into places people cross only with dread.",
                Variants(
                    ("a web-dark giant spider",
                        "This towering spider crouches low on long, stabbing legs, its swollen abdomen, clustered eyes and heavy fangs making it look built for patience and sudden murder."),
                    ("a ruin-haunting giant spider",
                        "This immense arachnid holds itself with unnerving stillness until it shifts, every pedipalp twitch and fang-glint suggesting venom, silk and a trap already laid.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "rfang", "lfang"),
                    Attack("Bite", ItemQuality.Standard, "rfang", "lfang"),
                    Attack("Claw Swipe", ItemQuality.Standard, "rclaw", "lclaw")
                ],
                attributeProfile: Stats(6, 5, 5, 1),
                bodypartHealthMultiplier: 1.6,
                canClimb: true,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Giant Scorpion"] = BeastRace(
                "Giant Scorpion",
                "Scorpion",
                SizeCategory.Large,
                "Giant Scorpion",
                GreatBeast(),
                "Giant scorpions are armoured arachnid horrors with grasping pedipalps and a venom-laden tail arched high above the body.",
                "Giant scorpions usually occupy the niche of desert apex vermin and ruin-haunters, monstrous ambush predators whose claws and tail make even open ground feel like a kill zone.",
                Variants(
                    ("a barbed giant scorpion",
                        "This massive scorpion creeps low beneath a hard shell, its heavy claws spread wide while a barbed tail hangs over the carapace like a drawn spear."),
                    ("a ruin-stalking giant scorpion",
                        "This outsized arachnid moves with deliberate menace, every click of claw and slow lift of the stinger promising a violent, venomous end.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Standard, "rfang", "lfang"),
                    Attack("Bite", ItemQuality.Standard, "rfang", "lfang"),
                    Attack("Claw Swipe", ItemQuality.Good, "rclaw", "lclaw"),
                    Attack("Tail Spike", ItemQuality.Good, "stinger")
                ],
                [
                    Usage("stinger", "general")
                ],
                attributeProfile: Stats(7, 7, 3, -1),
                bodypartHealthMultiplier: 1.8,
                combatStrategyKey: "Beast Brawler"
            ),
            ["Giant Centipede"] = BeastRace(
                "Giant Centipede",
                "Centipede",
                SizeCategory.Large,
                "Giant Centipede",
                GreatBeast(),
                "Giant centipedes are long, many-legged horrors with rippling segmented bodies and venomous-looking mandibles large enough to tear flesh.",
                "Giant centipedes fill the role of burrowing terror and ruin-crawler, the sort of subterranean menace that turns cracks, culverts and cellar dark into things worth fearing.",
                Variants(
                    ("a rippling giant centipede",
                        "This massive centipede moves in an unnerving wave of chitin and legs, its segmented body carrying it forward with horrible, tireless purpose."),
                    ("a tunnel-bred giant centipede",
                        "This elongated arthropod looks engineered for narrow dark places, with countless legs, twitching antennae and murderous mouthparts leading the way.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mandibles"),
                    Attack("Bite", ItemQuality.Standard, "mandibles")
                ],
                attributeProfile: Stats(7, 6, 3, 0),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Clincher"
            ),
            ["Giant Worm"] = BeastRace(
                "Giant Worm",
                "Vermiform",
                SizeCategory.Large,
                "Giant Worm",
                GreatBeast(),
                "Giant worms are immense burrowers of ringed flesh and grinding mouths, built to tunnel through earth and swallow prey whole.",
                "Giant worms fill the role of chthonic hazard and living natural disaster, the sort of subterranean terror that makes roads, farms and city foundations feel provisional at best.",
                Variants(
                    ("a burrowing giant worm",
                        "This enormous segmented worm pushes forward with dreadful muscular certainty, its ringed body and grinding mouth suggesting hunger without pause or pity."),
                    ("a tunnel-slick giant worm",
                        "This colossal annelid looks made for dark soil and collapsing galleries, every flex of its body promising ruin, suffocation and a very short struggle.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Tail Slap", ItemQuality.Standard, "tail")
                ],
                attributeProfile: Stats(8, 9, -2, -3),
                bodypartHealthMultiplier: 1.9,
                canSwim: false,
                combatStrategyKey: "Beast Clincher"
            ),
            ["Colossal Worm"] = BeastRace(
                "Colossal Worm",
                "Vermiform",
                SizeCategory.VeryLarge,
                "Colossal Worm",
                GreatBeast(),
                "Colossal worms are vast subterranean predators whose tunnel-boring bulk and circular jaws make them feel more like moving geology than living flesh.",
                "Colossal worms occupy the mythic niche of land-swallowing behemoths and siege-breaking burrowers, creatures large enough to turn settlement and battlefield alike into unstable ground.",
                Variants(
                    ("a land-swallowing colossal worm",
                        "This monstrous worm is all ringed mass and abyssal appetite, a subterranean behemoth whose circling maw and heaving body make the earth around it feel unsafe."),
                    ("a deep-bred colossal worm",
                        "This impossible annelid looks less like an animal than a tunnel given hunger, its endless-seeming body flexing with the slow, dreadful power of something that has never needed to hurry.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.VeryGood, "mouth"),
                    Attack("Bite", ItemQuality.Good, "mouth"),
                    Attack("Tail Slap", ItemQuality.Good, "tail")
                ],
                attributeProfile: Stats(12, 12, -3, -4),
                bodypartHealthMultiplier: 2.5,
                canSwim: false,
                combatStrategyKey: "Beast Behemoth"
            ),
            ["Ankheg"] = BeastRace(
                "Ankheg",
                "Centipede",
                SizeCategory.VeryLarge,
                "Giant Centipede",
                GreatBeast(),
                "Ankhegs are immense burrowing arthropods with powerful mandibles and corrosive acid they can spit at prey or intruders.",
                "Ankhegs usually occupy the niche of siege vermin and subterranean apex predators, monstrous tunnelers whose acid and sudden eruptions make farmland, roads and fortifications feel insecure.",
                Variants(
                    ("an acid-spitting ankheg",
                        "This gigantic burrowing horror heaves its segmented body forward behind huge mandibles, every plate of chitin suggesting digging strength and violent resilience."),
                    ("a trench-bursting ankheg",
                        "This massive arthropod looks made to erupt from below in a shower of soil and venom, its twitching antennae and oversized mandibles preceding a hiss of acid.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.VeryGood, "mandibles"),
                    Attack("Bite", ItemQuality.Good, "mandibles"),
                    Attack("Acid Spit", ItemQuality.Good, "mandibles")
                ],
                attributeProfile: Stats(9, 8, 1, -2),
                bodypartHealthMultiplier: 2.0,
                combatStrategyKey: "Beast Artillery"
            ),
            ["Hippocamp"] = BeastRace(
                "Hippocamp",
                "Hippocamp",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                "Hippocamps marry an equine forebody to a powerful fish-tail suited to open water.",
                "Hippocamps often serve as steeds, sacred sea-herd animals or elusive prizes for coastal peoples, their strange shape making them valuable to myth, pageantry and any culture that dreams of riding the surf.",
                Variants(
                    ("a sea-tailed hippocamp",
                        "This aquatic myth-beast bears a horse-like forebody and forelegs, but from the loins back it flows into a muscular fish-tail built for swift, powerful swimming."),
                    ("a surf-bred hippocamp",
                        "This creature looks as though a warhorse had been reworked for open water, its strong chest and lifted neck carried before a tail meant for thrust rather than gallop.")
                ),
                [
                    Attack("Herbivore Bite", ItemQuality.Standard, "mouth"),
                    Attack("Hoof Stomp", ItemQuality.Standard, "rfhoof", "lfhoof"),
                    Attack("Tail Slap", ItemQuality.Good, "caudalfin")
                ],
                attributeProfile: Stats(7, 6, 1, -1),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Behemoth"
            ),
            ["Selkie"] = HumanoidRace(
                "Selkie",
                "Organic Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Selkies are graceful seal-folk who can move comfortably between shore and sea.",
                "Selkies usually inhabit the role of liminal coastal people, bound to coves, islands and cold waters, where they are known through trade, kinship, secrecy and stories of departure and return.",
                [
                    Attack("Carnivore Bite", ItemQuality.Bad, "mouth"),
                    Attack("Bite", ItemQuality.Bad, "mouth")
                ],
                ["selkie"],
                overlayDescriptionVariants: Variants(
                    ("a seal-blooded selkie", "This selkie has a recognisably humanoid frame softened by an aquatic grace, the race's seal-blooded heritage evident in the smooth lines and sea-going poise."),
                    ("a sea-graceful selkie", "This selkie carries themself with the easy balance of someone more at home on wave-washed rock and in cold surf than on dry inland roads.")
                ),
                attributeProfile: Stats(0, 1, 1, 0),
                bodypartHealthMultiplier: 1.0,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Myconid"] = SapientRace(
                "Myconid",
                "Organic Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Myconids are humanoid fungal folk with broad caps, soft flesh and an unsettlingly quiet demeanor.",
                "Myconids usually occupy the mythic niche of hidden underworld communities, patient spore-keepers and eerie but not always hostile neighbours whose alien biology shapes every custom they keep.",
                Variants(
                    ("a cap-headed myconid",
                        "This stooped fungus-being has a humanoid shape but a cap-like head and an organic, spongy texture that marks it as something far removed from ordinary flesh."),
                    ("a spore-soft myconid",
                        "This fungal person looks less built from muscle and bone than from flexible growth, with a stillness that feels communal and subterranean rather than animal.")
                ),
                [
                    Attack("Jab", ItemQuality.Bad, "rhand", "lhand"),
                    Attack("Elbow", ItemQuality.Bad, "relbow", "lelbow")
                ],
                additionalCharacteristics:
                [
                    Characteristic("Fungus Colour", "white", "brown", "red", "purple")
                ],
                attributeProfile: Stats(-1, 2, -1, -2),
                bodypartHealthMultiplier: 1.1,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Plantfolk"] = SapientRace(
                "Plantfolk",
                "Organic Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Plantfolk are humanoid vegetative beings of bark, fibre and leaf.",
                "Plantfolk often stand in myth as wardens, gardeners, slow-speaking elders or embodiments of a place itself, with social rhythms shaped by season, light, soil and patient memory.",
                Variants(
                    ("a bark-skinned plantfolk",
                        "This plant-being stands in a recognisably humanoid form, but bark-like surfaces, fibrous growths and living greenery make every motion seem rooted in the natural world."),
                    ("a leaf-grown plantfolk",
                        "This vegetative person looks less like flesh made green than like a walking tangle of living wood and pliant stems coaxed into a humanoid shape.")
                ),
                [
                    Attack("Jab", ItemQuality.Bad, "rhand", "lhand"),
                    Attack("Elbow", ItemQuality.Bad, "relbow", "lelbow")
                ],
                attributeProfile: Stats(2, 4, -1, -2),
                bodypartHealthMultiplier: 1.2,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Ent"] = SapientRace(
                "Ent",
                "Organic Humanoid",
                SizeCategory.Large,
                "Large Ungulate",
                "Large Ungulate",
                LongLivedHumanoid(),
                "Ents are towering treefolk of bark, root and living wood whose movements feel ponderous only until they decide to act.",
                "Ents usually fill the role of ancient wardens, forest shepherds and patient but terrifying avengers, beings whose scale and age make most mortal concerns seem brief and hurried.",
                Variants(
                    ("a bark-armoured ent",
                        "This towering tree-being has a humanoid silhouette only in the loosest sense, its trunk-like limbs, root-heavy stance and rough bark hide making it look like a walking elder tree."),
                    ("a root-footed ent",
                        "This great plant person moves with slow deliberate power, every branch-like arm and knot-work limb suggesting an age measured in seasons rather than years.")
                ),
                [
                    Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
                    Attack("Elbow", ItemQuality.Bad, "relbow", "lelbow")
                ],
                attributeProfile: Stats(5, 8, -2, -2),
                bodypartHealthMultiplier: 1.6,
                canSwim: false,
                additionalCharacteristics:
                [
                    Characteristic("Bark Tone", "oak-brown", "ash-grey", "birch-pale", "yew-dark"),
                    Characteristic("Leaf Hue", "deep green", "silver-green", "gold", "rust red")
                ],
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Dryad"] = HumanoidRace(
                "Dryad",
                "Organic Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Dryads are graceful tree-spirits whose forms remain recognisably humanoid while still carrying the living marks of bark, bloom and leaf.",
                "Dryads usually occupy the niche of grove-keepers, emissaries of old forests and alluring but dangerous spirits of place, balancing beauty, patience and a fierce protectiveness toward their homes.",
                [
                    Attack("Jab", ItemQuality.Bad, "rhand", "lhand"),
                    Attack("Elbow", ItemQuality.Bad, "relbow", "lelbow")
                ],
                ["dryad"],
                overlayDescriptionVariants: Variants(
                    ("a blossom-haired dryad", "This dryad carries a largely humanoid form, but bark-soft skin, leaf-wrought hair and a faint scent of living wood mark the figure unmistakably as a spirit of the grove."),
                    ("a leaf-veiled dryad", "This dryad moves like a person taught by branches and wind, every line of the figure softened by petals, bark-grain and the quiet poise of old trees.")
                ),
                attributeProfile: Stats(0, 1, 1, 1),
                bodypartHealthMultiplier: 1.0,
                canClimb: true,
                canSwim: false,
                facialHairProfile: "No_Facial_Hair",
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Owlkin"] = HumanoidRace(
                "Owlkin",
                "Winged Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Owlkin are feathered, winged people with a keen gaze and a marked avian cast.",
                "Owlkin commonly fill the role of nocturnal scholars, hunters and sentries, their cultural identity often bound to keen perception, silence, altitude and a strong sense of territory.",
                [
                    Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
                    Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
                ],
                ["owlkin"],
                Variants(
                    ("a feathered owlkin", "This owlkin carries a humanoid frame beneath plumage and wings, with an intense gaze and avian lines that immediately set them apart from ordinary people."),
                    ("a keen-eyed owlkin", "This owlkin's feathered features and broad wings give them the look of a night hunter shaped into a mostly humanoid form.")
                ),
                [
                    Usage("rwingbase", "general"),
                    Usage("lwingbase", "general"),
                    Usage("rwing", "general"),
                    Usage("lwing", "general")
                ],
                attributeProfile: Stats(0, 0, 1, 1),
                bodypartHealthMultiplier: 1.0,
                canClimb: true,
                facialHairProfile: "No_Facial_Hair",
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Avian Person"] = HumanoidRace(
                "Avian Person",
                "Winged Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                "Avian people are broad-winged birdfolk whose forms remain largely humanoid aside from their wings and avian features.",
                "Avian peoples usually read as aerial citizens rather than monsters, with cultures oriented toward roosting space, wind, lookout duties and the practical consequences of living with wings and height.",
                [
                    Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
                    Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
                ],
                ["birdfolk", "avian"],
                Variants(
                    ("a broad-winged avian", "This avian person has a mostly humanoid build, but feathering, wings and a birdlike cast to the face shift the impression immediately skyward."),
                    ("a feather-cloaked birdfolk", "This birdfolk figure remains recognisably person-shaped, yet every line of the wings and plumage suggests roosts, thermals and open air.")
                ),
                [
                    Usage("rwingbase", "general"),
                    Usage("lwingbase", "general"),
                    Usage("rwing", "general"),
                    Usage("lwing", "general")
                ],
                attributeProfile: Stats(0, 0, 1, 1),
                bodypartHealthMultiplier: 1.0,
                canClimb: true,
                facialHairProfile: "No_Facial_Hair",
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Centaur"] = HumanoidRace(
                "Centaur",
                "Centaur",
                SizeCategory.Large,
                "Horse",
                "Horse",
                LongLivedHumanoid(),
                "Centaurs combine human torsos and arms with a four-legged equine lower body.",
                "Centaurs are frequently cast as nomads, outriders and peoples of open country, their societies shaped by speed, horizon, herd memory and a bodily scale that changes how they build, travel and fight.",
                [
                    Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
                    Attack("Head Ram", ItemQuality.Standard, "head"),
                    Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
                ],
                ["centaur"],
                overlayDescriptionVariants: Variants(
                    ("a deep-chested centaur", "This centaur combines a humanoid torso and arms with a powerful equine lower body, making the whole figure look fast, stable and difficult to dislodge."),
                    ("a long-striding centaur", "This centaur's human upper body rises from a broad horse-frame whose musculature and stance suggest endurance, mobility and hard impact.")
                ),
                attributeProfile: Stats(6, 5, 1, 0),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Pegacorn"] = BeastRace(
                "Pegacorn",
                "Ungulate",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                "Pegacorns combine the broad wings of a pegasus with the spiralled horn of a unicorn.",
                "Pegacorns occupy the rarest and most ceremonial niche of the winged equines, appearing in myths as omens, impossible mounts and embodiments of wonder that blend swiftness with sanctity.",
                Variants(
                    ("a horned winged pegacorn",
                        "This mythic equine bears both sweeping feathered wings and a singular horn, giving it the grace of a unicorn and the power of a pegasus."),
                    ("a sky-bright pegacorn",
                        "This creature looks impossibly ornate even by mythic standards, as though an already noble flying horse had been elevated into something sacred and untouchable.")
                ),
                [
                    Attack("Bite", ItemQuality.Bad, "mouth"),
                    Attack("Hoof Stomp", ItemQuality.Good, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
                    Attack("Horn Gore", ItemQuality.Good, "horn")
                ],
                [
                    Usage("horn", "general"),
                    Usage("rwingbase", "general"),
                    Usage("lwingbase", "general"),
                    Usage("rwing", "general"),
                    Usage("lwing", "general")
                ],
                attributeProfile: Stats(8, 7, 2, 0),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Swooper"
            )
        };
    }

    internal static string BuildRaceDescriptionForTesting(MythicalRaceTemplate template)
    {
        StockDescriptionVariant? supportingVariant = (template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.FirstOrDefault();
        return SeederDescriptionHelpers.JoinParagraphs(
            SeederDescriptionHelpers.EnsureTrailingPeriod(template.Description),
            supportingVariant?.FullDescription ?? $"This race is represented by the {template.Name} stock catalogue.",
            SeederDescriptionHelpers.EnsureTrailingPeriod(template.RoleDescription)
        );
    }

    internal static string BuildEthnicityDescriptionForTesting(MythicalRaceTemplate template)
    {
        return SeederDescriptionHelpers.JoinParagraphs(
            SeederDescriptionHelpers.EnsureTrailingPeriod(
                $"The stock {template.Name.ToLowerInvariant()} ethnicity represents the default lineage, upbringing and outward presentation seeded for this mythic race."),
            (template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.Skip(1).FirstOrDefault()?.FullDescription ??
            (template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.FirstOrDefault()?.FullDescription ??
            $"This stock heritage keeps the visual hallmarks and bodily proportions associated with {template.Name.ToLowerInvariant()} characters.",
            SeederDescriptionHelpers.EnsureTrailingPeriod(template.RoleDescription)
        );
    }

    internal static IReadOnlyList<string> ValidateTemplateCatalogForTesting()
    {
        List<string> issues = new();
        HashSet<string> validBodyKeys = new(StringComparer.OrdinalIgnoreCase)
        {
            "Toed Quadruped",
            "Griffin",
            "Hippogriff",
            "Ungulate",
            "Insectoid",
            "Arachnid",
            "Beetle",
            "Centipede",
            "Vermiform",
            "Horned Humanoid",
            "Eastern Dragon",
            "Naga",
            "Mermaid",
            "Manticore",
            "Wyvern",
            "Avian",
            "Serpentine",
            "Hippocamp",
            "Scorpion",
            "Organic Humanoid",
            "Winged Humanoid",
            "Centaur"
        };
        HashSet<string> validAttackNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Carnivore Bite",
            "Bite",
            "Claw Swipe",
            "Horn Gore",
            "Tail Slap",
            "Dragonfire Breath",
            "Beak Peck",
            "Beak Bite",
            "Hoof Stomp",
            "Head Ram",
            "Talon Strike",
            "Wing Buffet",
            "Tail Spike",
            "Herbivore Bite",
            "Acid Spit",
            "Jab",
            "Elbow"
        };
        HashSet<string> nonClinchAttackNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Carnivore Bite",
            "Claw Swipe",
            "Horn Gore",
            "Tail Slap",
            "Dragonfire Breath",
            "Beak Peck",
            "Head Ram",
            "Talon Strike",
            "Wing Buffet",
            "Tail Spike",
            "Acid Spit",
            "Jab"
        };
        HashSet<string> clinchAttackNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Bite",
            "Beak Bite",
            "Herbivore Bite",
            "Elbow"
        };

        if (Templates.Count != 36)
        {
            issues.Add($"Expected 36 mythical race templates but found {Templates.Count}.");
        }

        foreach ((string? name, MythicalRaceTemplate? template) in Templates)
        {
            if (string.IsNullOrWhiteSpace(template.CombatStrategyKey))
            {
                issues.Add($"Mythical race {name} is missing a combat strategy key.");
            }
            else if (!CombatStrategySeederHelper.IsKnownStrategyName(template.CombatStrategyKey))
            {
                issues.Add($"Mythical race {name} references unknown combat strategy {template.CombatStrategyKey}.");
            }

            if (!validBodyKeys.Contains(template.BodyKey))
            {
                issues.Add($"Race {name} uses unknown body key {template.BodyKey}.");
            }

            if (!template.Attacks.All(x => validAttackNames.Contains(x.AttackName)))
            {
                issues.Add($"Race {name} references an unsupported attack name.");
            }

            if (template.Attacks.Count == 0)
            {
                issues.Add($"Race {name} must expose at least one natural attack.");
            }
            else
            {
                if (!template.Attacks.Any(x => nonClinchAttackNames.Contains(x.AttackName)))
                {
                    issues.Add($"Race {name} must expose at least one non-clinch natural attack.");
                }

                if (!template.Attacks.Any(x => clinchAttackNames.Contains(x.AttackName)))
                {
                    issues.Add($"Race {name} must expose at least one clinch natural attack.");
                }
            }

            if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildRaceDescriptionForTesting(template)))
            {
                issues.Add($"Race {name} should build a three-paragraph race description.");
            }

            if (!SeederDescriptionHelpers.HasMinimumParagraphs(BuildEthnicityDescriptionForTesting(template)))
            {
                issues.Add($"Race {name} should build a three-paragraph stock ethnicity description.");
            }

            if (template.HumanoidVariety)
            {
                if (template.PersonWords == null || template.PersonWords.Count == 0)
                {
                    issues.Add($"Humanoid variety race {name} is missing person words.");
                }

                if (template.CanUseWeapons == false)
                {
                    issues.Add($"Humanoid variety race {name} should support weapon use.");
                }

                if (template.OverlayDescriptionVariants == null || template.OverlayDescriptionVariants.Count < 2)
                {
                    issues.Add($"Humanoid variety race {name} should define at least two overlay description variants.");
                }
            }
            else
            {
                if (template.DescriptionVariants == null || template.DescriptionVariants.Count < 2)
                {
                    issues.Add($"Bestial race {name} should define at least two stock description variants.");
                }
            }

            if ((template.DescriptionVariants ?? template.OverlayDescriptionVariants)?.Any(x =>
                    string.IsNullOrWhiteSpace(x.ShortDescription) || string.IsNullOrWhiteSpace(x.FullDescription)) == true)
            {
                issues.Add($"Race {name} has a blank stock description variant.");
            }
        }

        return issues;
    }
}

#nullable enable

using MudSharp.Body;
using MudSharp.Character.Heritage;
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
        IReadOnlyList<SeederTattooTemplateDefinition>? TattooTemplates = null,
		double MaximumFoodSatiatedHours = RacialSatiationDefaults.MaximumFoodSatiatedHours,
		double MaximumDrinkSatiatedHours = RacialSatiationDefaults.MaximumDrinkSatiatedHours
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

        static NonHumanAttributeProfile Stats(
            int strength,
            int constitution,
            int agility,
            int dexterity,
            int willpower = 0,
            int perception = 0,
            int aura = 0,
            string? intelligenceDiceExpression = null,
            string? auraDiceExpression = null)
        {
            return new(
                strength,
                constitution,
                agility,
                dexterity,
                willpower,
                perception,
                aura,
                intelligenceDiceExpression,
                AuraDiceExpression: auraDiceExpression);
        }

        static NonHumanAttributeProfile BestialStats(
            int strength,
            int constitution,
            int agility,
            int dexterity,
            int willpower,
            int perception,
            int aura = 0,
            string intelligenceDiceExpression = "2d3",
            string? auraDiceExpression = "1d2")
        {
            return Stats(
                strength,
                constitution,
                agility,
                dexterity,
                willpower,
                perception,
                aura,
                intelligenceDiceExpression,
                auraDiceExpression);
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

        Dictionary<string, MythicalRaceTemplate> templates = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Dragon"] = BeastRace(
                "Dragon",
                "Toed Quadruped",
                SizeCategory.VeryLarge,
                "Horse",
                GreatBeast(),
                """
                Dragons are immense horned reptiles whose wings, claws and scaled hides make them sovereign presences wherever they settle. Their size alone would make them dangerous, but the deeper threat lies in the sense that every movement is chosen by a mind older and colder than an ordinary beast's.

                Heat, hoarded treasure, scorched stone and old territorial memory gather naturally around them. A dragon's lair is rarely just a den; it becomes a centre of fear, tribute, rumour and calculation, because people must decide whether to placate it, hunt it or keep far beyond the reach of its shadow.

                They work best as apex powers rather than simple monsters. A dragon may be tyrant, oracle, disaster, patron or sleeping peril, but even a peaceful one makes the surrounding country feel negotiated rather than free.
                """,
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
                attributeProfile: Stats(12, 11, 0, -2, willpower: 6, perception: 3, aura: 5),
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
                """
                Griffins unite the hooked beak, bright eyes and broad wings of a raptor with the hindquarters and pouncing force of a great cat. The result is not a patched-together animal, but a proud aerial hunter whose body looks equally suited to cliff wind and violent impact.

                They claim high places with the confidence of creatures that see roads, herds and armies from above. Their talons make them dangerous in the first instant of contact, while the leonine part of the body gives them enough weight and courage to hold prey or intruder once grounded.

                Where griffins are known, they tend to gather meanings of vigilance, nobility and territorial wrath. They may serve as sacred beasts, heraldic emblems, royal mounts or wild threats, but they never feel tame in the ordinary sense.
                """,
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
                    Attack("Talon Carry", ItemQuality.Good, "rfpaw", "lfpaw"),
                    Attack("Tail Slap", ItemQuality.Standard, "ltail"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                attributeProfile: BestialStats(7, 6, 3, 1, willpower: 3, perception: 4),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Dropper"
            ),
            ["Hippogriff"] = BeastRace(
                "Hippogriff",
                "Hippogriff",
                SizeCategory.Large,
                "Horse",
                Beast(),
                """
                Hippogriffs are eagle-fronted, winged equine beasts, carrying the alert violence of a raptor on a frame large enough for the saddle. Their grace can make them look approachable until the beak, talons and sudden lunging balance remind the observer that this is no ordinary horse.

                Their power sits in transition: a gallop can become a launch, a turn of the neck can become a strike, and the animal's proud carriage can change without warning into prey-focused urgency. They seem made for open slopes, hard winds and riders who understand that respect matters more than reins.

                They occupy the border between wild mount and dangerous sky-beast. In settled hands they become prestige animals, scouts and cavalry wonders; in the wild they remain territorial grazers with enough predatory blood to punish careless confidence.
                """,
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
                attributeProfile: BestialStats(6, 5, 3, 1, willpower: 2, perception: 3),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Unicorn"] = BeastRace(
                "Unicorn",
                "Ungulate",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                """
                Unicorns are horse-like beings marked by a single spiralled horn and an unnerving purity of motion. The familiar equine outline makes the strangeness sharper, because the creature seems almost ordinary until its gaze, stillness and flawless poise begin to feel deliberate.

                The horn turns the body into a sign as much as a weapon. Around a unicorn, sanctuaries, moonlit glades and guarded springs feel less like scenery than rightful territory, as though the beast belongs to places where violence has to justify itself.

                They are rarely treated as livestock, even when they can be approached. A unicorn may be mount, omen, guardian or prize, but it carries a moral pressure into the scene: the crude reach for it, the wary give way, and the worthy are measured before they are allowed near.
                """,
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
                attributeProfile: Stats(6, 5, 4, 1, willpower: 4, perception: 3, aura: 5),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Pegasus"] = BeastRace(
                "Pegasus",
                "Ungulate",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                """
                Pegasi are winged horses built for both gallop and sky. Their deep chests, powerful shoulders and feathered wings give the familiar nobility of an equine body a startling vertical promise, as if a pasture animal had learned to refuse the ground.

                They are creatures of open air, long approaches and difficult capture. The wings are not decoration; they change everything about how the animal flees, threatens, courts and chooses its range, giving even a quiet pegasus a sense of stored lift.

                Among people, pegasi become noble mounts, courier beasts, sacred herd animals or symbols of freedom that are harder to keep than to admire. In the wild, they remain strong-willed sky-grazers whose beauty does not erase their strength.
                """,
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
                attributeProfile: Stats(5, 4, 5, 1, willpower: 2, perception: 3, aura: 3, intelligenceDiceExpression: "2d3"),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Warg"] = BeastRace(
                "Warg",
                "Toed Quadruped",
                SizeCategory.Large,
                "Large Canid",
                GreatBeast(),
                """
                Wargs are oversized wolf-like predators with harsh intelligence in the eyes and a body pushed toward endurance, pursuit and mauling force. They look close enough to wolves to be recognised, but too heavy, deliberate and malicious to be mistaken for natural pack animals.

                Their strength lies in discipline as much as size. A warg reads the ground, the herd and the frightened line of retreat with predatory economy, making it valuable to raiders and terrifying to travellers who hear howls beyond the firelight.

                They fit war camps, dark hunts and borderlands where fear is cultivated as a weapon. Whether bred, cursed or simply born monstrous, a warg makes the night feel organised against anyone moving through it.
                """,
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
                attributeProfile: BestialStats(6, 5, 2, 0, willpower: 4, perception: 3),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Dire-Wolf"] = BeastRace(
                "Dire-Wolf",
                "Toed Quadruped",
                SizeCategory.VeryLarge,
                "Large Canid",
                GreatBeast(),
                """
                Dire-wolves are colossal wolf-beasts, broader and heavier than wargs and built to bring down prey by weight as much as speed. Their heads are massive, their paws large enough to leave frightening tracks, and their presence turns a howl into a tactical fact.

                They belong to winter timber, deep ravines and long pursuit across country where exhaustion kills before the jaws arrive. A dire-wolf does not need supernatural ornament to feel mythic; scale, pack confidence and relentless motion are enough.

                They serve as pack-lords, wilderness punishments and the kind of predator that changes travel routes. A single dire-wolf is a crisis; a pack suggests that the land itself has chosen teeth.
                """,
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
                attributeProfile: BestialStats(8, 7, 2, 0, willpower: 4, perception: 3),
                bodypartHealthMultiplier: 1.95,
                combatStrategyKey: "Beast Brawler"
            ),
            ["Dire-Bear"] = BeastRace(
                "Dire-Bear",
                "Toed Quadruped",
                SizeCategory.VeryLarge,
                "Bear",
                GreatBeast(),
                """
                Dire-bears are immense ursine horrors, thick with fur, shoulder mass and ruinous claws. They retain the blunt solidity of ordinary bears, but expanded until every gesture feels capable of breaking timber, stone or bone.

                Their menace is not speed or cunning display, but occupying space with total physical certainty. Caves, old forests and mountain passes feel different when a dire-bear claims them, because retreat becomes the sensible shape of survival.

                They make excellent ancient den-lords, forest tyrants and frontier nightmares. People may speak of them as spirits of wilderness anger or simply as animals too large to reason with; either reading works once one is close enough to smell it.
                """,
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
                attributeProfile: BestialStats(10, 9, 0, -1, willpower: 5, perception: 2),
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
                """
                Minotaurs are broad horned humanoids whose bodies carry the force of a bull without surrendering personhood. Heavy brows, deep chests, thick necks and sweeping horns make them immediately imposing, especially in enclosed places where their charge has nowhere to dissipate.

                They are shaped for labour, intimidation and brutal close fighting, but their mythic strength is also architectural. Corridors, gates, arenas and maze-like strongholds suit them because their sheer build makes every narrow passage feel contested.

                A minotaur can be warrior, guardian, exile, citizen or monster depending on the society around it. What remains constant is the pressure of contained force: a person whose physical presence makes diplomacy, fear and violence share the same room.
                """,
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
                attributeProfile: Stats(6, 5, -1, -1, willpower: 3, aura: -1),
                bodypartHealthMultiplier: 1.2
            ),
            ["Eastern Dragon"] = BeastRace(
                "Eastern Dragon",
                "Eastern Dragon",
                SizeCategory.VeryLarge,
                "Horse",
                GreatBeast(),
                """
                Eastern dragons are long, sinuous drakes whose clawed limbs and flowing bodies carry majesty without the need for wings. Their motion suggests river current, cloud-drift and coiled strength, with ornament and scale arranged into a profile of ancient authority.

                They feel less like hoard-bound beasts than powers of movement and season. Waterways, storms, imperial courts and mountain cloud can all become natural theatres for them, because their shape implies rulership over change rather than simple predation.

                Where one appears, people tend to read omen before animal. An eastern dragon may bless, threaten, test, advise or destroy, but it arrives with the gravity of something that has watched dynasties, floods and vows outlive their makers.
                """,
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
                attributeProfile: Stats(10, 9, 2, 0, willpower: 6, perception: 3, aura: 5),
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
                """
                Nagas are serpent-bodied people, humanoid from the waist up and powerful through the coiling length below. Their posture is courtly and dangerous at once, because the same body that can recline in ceremony can also strike, bind and rise with sudden force.

                They suit river temples, marsh courts, old libraries and thresholds where patience is a political weapon. Scales, careful hands, still eyes and low-centred movement make them feel grounded in water, stone and ritual rather than ordinary settlement life.

                As a people, naga can be nobles, guardians, scholars, sorcerers or hidden neighbours. Their danger is not merely venom or strength, but the impression that they think in longer coils than those who bargain with them.
                """,
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
                attributeProfile: Stats(1, 3, 2, 1, willpower: 2, perception: 2, aura: 2),
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
                """
                Mermaids and merfolk join recognisably human torsos and arms to powerful piscine tails. The form is intimate and alien at once: a person from the shoulders upward, a creature of current, scale and depth below.

                For them, land feels temporary. Hands can trade, beckon, rescue or threaten, but the tail belongs to reefs, kelp-dark water and long movement beneath the surface, where ordinary human grace would fail almost immediately.

                They are shoreline enigmas, sea-dwellers, raiders, singers, traders and guardians depending on the culture that knows them. A merfolk presence turns any meeting at the water's edge into an exchange between two incompatible homes.
                """,
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
                attributeProfile: Stats(0, 1, 2, 1, willpower: 1, perception: 2, aura: 1),
                bodypartHealthMultiplier: 1.0,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Manticore"] = BeastRace(
                "Manticore",
                "Manticore",
                SizeCategory.Large,
                "Big Felid",
                GreatBeast(),
                """
                Manticores are leonine winged predators armed with a venomous tail-spike. The body promises layered danger: claws at the first rush, jaws at close range, wings for sudden approach, and a tail that keeps threatening even when the front of the beast is checked.

                They belong to badlands, lonely heights, caravan roads and ruined watchposts where rumour travels faster than certainty. Their silhouette is memorable enough that witnesses embroider it, but the practical facts are simple: a manticore can reach, maul and poison from too many angles.

                They work as nightmare apex predators rather than ordinary wildlife. Hunters, travellers and border guards learn to treat each reported sighting as serious, because underestimating one means being wrong only once.
                """,
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
                attributeProfile: BestialStats(8, 6, 3, 0, willpower: 4, perception: 3, aura: 1),
                bodypartHealthMultiplier: 1.8,
                combatStrategyKey: "Beast Artillery"
            ),
            ["Wyvern"] = BeastRace(
                "Wyvern",
                "Wyvern",
                SizeCategory.Large,
                "Raptor",
                GreatBeast(),
                """
                Wyverns are two-legged draconic fliers, leaner and more openly predatory than true dragons. Leathery wings, taloned legs, snapping jaws and a lashing tail give them a stripped-down brutality, as if every part of the body exists for diving, seizing and tearing.

                They favour cliffs, ruins, dry ranges and hard open air where altitude decides survival. A wyvern does not carry the patient authority of a dragon; it carries the hunger of something that wants the high ground and punishes anything crossing below it.

                They are raiders of herds, terror of lonely roads and vicious trophies for those who claim to master the sky. Their role is simple but potent: flight made savage enough to alter how people move through exposed country.
                """,
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
                    Attack("Talon Carry", ItemQuality.Good, "rtalons", "ltalons"),
                    Attack("Tail Slap", ItemQuality.Standard, "tail"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                attributeProfile: BestialStats(8, 6, 3, 0, willpower: 4, perception: 3, aura: 1),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Dropper"
            ),
            ["Fell Beast"] = BeastRace(
                "Fell Beast",
                "Wyvern",
                SizeCategory.Large,
                "Raptor",
                GreatBeast(),
                """
                Fell beasts are vast carrion-winged reptilian mounts with gaunt necks, grasping talons and a hateful downward focus. They look less like noble drakes than battlefield nightmares made to carry terror over walls and ranks.

                The silhouette suggests endurance under burden as much as predation. A fell beast can circle, stoop, snatch and bear a rider through smoke or storm, but nothing about it feels domesticated; obedience seems wrung from fear, cruelty or a darker bond.

                They suit black cavalry, mountain eyries and ominous flights over threatened lands. Even without a rider, one makes the sky feel occupied by hostile intent.
                """,
                "Fell beasts fit dark-rider, war-mount and aerial terror roles, giving builders a ready stock creature for evil cavalry, mountain eyries, siege scouting or ominous flights over threatened lands.",
                Variants(
                    ("a carrion-winged fell beast",
                        "This immense winged horror has a gaunt reptilian frame, hooked talons and a long head made for snapping at prey from above."),
                    ("a black-winged fell beast",
                        "This flying monster looks less like a noble dragon than a starving war-mount, all leathery wings, sinew and cruel downward focus.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Talon Strike", ItemQuality.Good, "rtalons", "ltalons"),
                    Attack("Talon Carry", ItemQuality.Good, "rtalons", "ltalons"),
                    Attack("Tail Slap", ItemQuality.Standard, "tail"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                attributeProfile: BestialStats(7, 6, 4, 0, willpower: 3, perception: 3, aura: 2),
                bodypartHealthMultiplier: 1.65,
                canSwim: false,
                combatStrategyKey: "Beast Dropper"
            ),
            ["Phoenix"] = BeastRace(
                "Phoenix",
                "Avian",
                SizeCategory.Normal,
                "Raptor",
                GreatBeast(),
                """
                Phoenixes are radiant firebirds whose beauty carries heat, ash and renewal in equal measure. They resemble great raptors or ceremonial birds, but light clings to them in a way that makes ordinary plumage seem dull and temporary.

                Their importance is symbolic before it is tactical. Feathers, ember-bright eyes and furnace-coloured wings make the phoenix a walking omen of endings that refuse to stay ended, even where no literal rebirth is assumed.

                They may be sacred beasts, fire-spirits, rare predators or living signs kept far from common touch. A phoenix is beautiful enough to draw reverence and dangerous enough to punish anyone who mistakes beauty for safety.
                """,
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
                attributeProfile: Stats(2, 2, 5, 3, willpower: 4, perception: 4, aura: 6, intelligenceDiceExpression: "2d3"),
                bodypartHealthMultiplier: 1.1,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Basilisk"] = BeastRace(
                "Basilisk",
                "Serpentine",
                SizeCategory.Normal,
                "Serpent",
                GreatBeast(),
                """
                Basilisks are immense sinister serpents whose stillness feels malignant rather than merely patient. Their coils have the weight of old venom, and their gaze makes the air around them feel unsafe even before the strike comes.

                They belong to ruins, tombs, dry wells and desolate ground where life has already learned to keep its distance. A basilisk does not need to be enormous in every telling; what matters is the concentrated impression of poison, curse and predatory certainty.

                They serve as lurking calamities, not animals to be studied casually. Tracks, shed skin or the silence of nearby creatures can be enough to make sensible travellers turn back.
                """,
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
                attributeProfile: BestialStats(5, 6, 2, 0, willpower: 4, perception: 2, aura: 2, auraDiceExpression: null),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Clincher"
            ),
            ["Cockatrice"] = BeastRace(
                "Cockatrice",
                "Avian",
                SizeCategory.Small,
                "Small Bird",
                Beast(),
                """
                Cockatrices are vicious little reptilian birds, all stabbing beak, clawed feet and spiteful motion. Their size makes them easy to underestimate, which is part of their danger around farms, granaries, yards and broken stone.

                They carry the nuisance energy of barnyard fowl twisted into something sharper and less forgiving. Scaled patches, stiff feathers and restless pecking give them a half-domestic, half-monstrous quality that makes them especially unwelcome near food stores or children.

                They work as invasive pests, cursed omens and small hazards that multiply into real trouble. A cockatrice rarely dominates a landscape, but it can make a familiar corner of that landscape feel treacherous.
                """,
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
                attributeProfile: BestialStats(0, 0, 3, 1, willpower: 1, perception: 2, aura: 1),
                bodypartHealthMultiplier: 0.7,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Giant Beetle"] = BeastRace(
                "Giant Beetle",
                "Beetle",
                SizeCategory.Large,
                "Giant Insect",
                GreatBeast(),
                """
                Giant beetles are armoured insects enlarged until chitin becomes architecture. Heavy shells, crushing mandibles and deliberate movement give them the presence of living siege equipment rather than ordinary vermin.

                Their strength is blunt resilience. Blades, claws and panic all meet the same hard casing, while the beetle continues forward with a slow insistence that makes cramped tunnels or courtyards feel smaller by the moment.

                They can be dungeon vermin, battlefield beasts, uncanny mounts or labour creatures kept by societies willing to live beside huge clicking mandibles. Whether feared or harnessed, they are valued for being difficult to stop.
                """,
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
                attributeProfile: BestialStats(5, 6, 0, -2, willpower: 2, perception: 0),
                bodypartHealthMultiplier: 1.8,
                combatStrategyKey: "Beast Behemoth"
            ),
            ["Giant Ant"] = BeastRace(
                "Giant Ant",
                "Insectoid",
                SizeCategory.Large,
                "Giant Insect",
                GreatBeast(),
                """
                Giant ants are eusocial insects made monstrous, their segmented bodies and crushing jaws driven by the tireless logic of a colony. An individual is alarming; the thought of many moving with the same purpose is worse.

                They turn soil, tunnels and earthen walls into organised territory. Antennae, mandibles and disciplined motion make them feel less like wandering beasts than extensions of a hidden command beneath the ground.

                They suit hive warrens, siege tunnels and landscapes where numbers matter more than heroics. People fear giant ants not only because they bite, but because their labour can reshape the battlefield before anyone notices.
                """,
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
                attributeProfile: BestialStats(6, 6, 2, -2, willpower: 3, perception: 1),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Clincher"
            ),
            ["Giant Mantis"] = BeastRace(
                "Giant Mantis",
                "Insectoid",
                SizeCategory.Large,
                "Giant Insect",
                GreatBeast(),
                """
                Giant mantises are towering predatory insects whose folded forelegs look like weapons held in prayer. Their triangular heads and rigid patience give them a terrible stillness, as though the entire body has become a trap waiting for permission to close.

                Camouflage, height and sudden reach define them. A giant mantis can vanish among trees, carved pillars or tall reeds until movement betrays prey, then convert silence into violence in a single precise strike.

                They work as garden horrors, temple guardians, jungle ambushers and sacred predators in cultures that respect patience as a form of cruelty. Their danger lies in how little warning they need.
                """,
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
                attributeProfile: BestialStats(6, 5, 4, 1, willpower: 2, perception: 2),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Giant Spider"] = BeastRace(
                "Giant Spider",
                "Arachnid",
                SizeCategory.Large,
                "Giant Arachnid",
                GreatBeast(),
                """
                Giant spiders are monstrous web-spinners with long stabbing legs, clustered eyes and fangs too large to dismiss as mere nuisance. The whole shape is built for patience, angles and the sudden conversion of stillness into murder.

                Silk changes the territory around them. Ruins, caverns and forest canopies become layered with paths, snares and hidden retreats, so that the spider's presence is felt before the spider itself appears.

                They are lair predators and ambush horrors, especially effective where visibility is broken and ceilings matter. A giant spider turns architecture and woodland into a question: what has already been prepared above you?
                """,
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
                attributeProfile: BestialStats(6, 5, 5, 1, willpower: 2, perception: 2),
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
                """
                Giant scorpions are armoured arachnid horrors with broad pincers and a venomous tail carried like a drawn spear. Every part of the animal advertises threat, from the low plated body to the slow deliberate lift of the stinger.

                They suit dry ruins, desert flats and stone hollows where a patient ambusher can wait in heat and shadow. Their claws make escape difficult, while the tail keeps danger poised even when the front of the creature is occupied.

                They are apex vermin of harsh places, useful as guardian beasts only to those willing to risk being remembered as fools. A giant scorpion makes open ground feel like a kill zone.
                """,
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
                attributeProfile: BestialStats(7, 7, 3, -1, willpower: 3, perception: 1),
                bodypartHealthMultiplier: 1.8,
                combatStrategyKey: "Beast Brawler"
            ),
            ["Giant Centipede"] = BeastRace(
                "Giant Centipede",
                "Centipede",
                SizeCategory.Large,
                "Giant Centipede",
                GreatBeast(),
                """
                Giant centipedes are elongated predators of chitin, rippling legs and venomous mouthparts. Their movement is often the most disturbing part: a wave of limbs carrying a narrow hunger forward through gaps that look too small for anything so large.

                They belong to cracks, culverts, root-tangles and buried passages where length is an advantage and panic favours the creature behind you. The head appears first, but the body keeps coming, making pursuit feel almost endless.

                They serve as tunnel terrors and ruin-crawlers, less majestic than dragons and more intimate in their horror. A giant centipede makes darkness feel crowded.
                """,
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
                attributeProfile: BestialStats(7, 6, 3, 0, willpower: 2, perception: 1),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Clincher"
            ),
            ["Giant Worm"] = BeastRace(
                "Giant Worm",
                "Vermiform",
                SizeCategory.Large,
                "Giant Worm",
                GreatBeast(),
                """
                Giant worms are immense burrowers of ringed flesh and grinding mouths, built for soil, pressure and appetite. They have little of a predator's elegance, but their simple forward hunger is frightening because the ground itself becomes their cover.

                Their passage leaves churned earth, collapsed galleries and sudden holes where safety used to be. The absence of eyes or expression makes them worse, turning them into a muscular event rather than an animal with moods to read.

                They work as subterranean hazards, chthonic monsters and living disasters beneath farms, roads and fortifications. A giant worm does not need malice to be ruinous; movement is enough.
                """,
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
                attributeProfile: BestialStats(8, 9, -2, -3, willpower: 4, perception: 0),
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
                """
                Colossal worms are vast subterranean predators whose bulk makes them feel like moving geology. Their circular jaws, endless ringed bodies and tunnel-boring strength place them beyond ordinary hunting into the realm of landscape-scale danger.

                Where one moves, roads buckle, wells vanish and fortifications discover that foundations are only promises made to the earth. Its body is so large that people may meet the effects of its passage long before they understand there is a creature beneath them.

                They are land-swallowing behemoths and siege-breaking terrors. A colossal worm changes the question from how to kill a monster to whether the ground can be trusted at all.
                """,
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
                attributeProfile: BestialStats(12, 12, -3, -4, willpower: 5, perception: 0),
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
                """
                Ankhegs are immense burrowing arthropods with plated bodies, powerful mandibles and corrosive acid. They combine the tunnelling threat of a subterranean beast with the ranged danger of something that can spit ruin before closing to bite.

                Their territory is marked by sudden sinkholes, cut roots, acrid residue and earthworks that look too purposeful for weather. When an ankheg erupts from below, armour and walls feel less reassuring than they did moments earlier.

                They fit farmland horror, siege vermin and buried border threats. An ankheg makes ordinary ground defensive terrain for the monster, not for those standing on it.
                """,
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
                attributeProfile: BestialStats(9, 8, 1, -2, willpower: 4, perception: 1, aura: 1),
                bodypartHealthMultiplier: 2.0,
                combatStrategyKey: "Beast Artillery"
            ),
            ["Hippocamp"] = BeastRace(
                "Hippocamp",
                "Hippocamp",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                """
                Hippocamps are aquatic myth-beasts with equine forebodies flowing into powerful fish-tails. Their lifted necks and horse-like chests suggest pride and trainability, while the tail places their true strength in surf, current and open water.

                They make the sea feel rideable without making it tame. A hippocamp can rear among waves, tow through foam or vanish beneath green water, carrying the familiar dignity of a horse into a medium that refuses ordinary hooves.

                They serve as sacred sea-herd animals, ceremonial mounts and elusive prizes for coastal peoples. Their presence turns harbours, reefs and beaches into places where pageantry and danger share the tide.
                """,
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
                attributeProfile: Stats(6, 5, 3, 0, willpower: 2, perception: 2, aura: 3, intelligenceDiceExpression: "2d3"),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Selkie"] = HumanoidRace(
                "Selkie",
                "Organic Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                """
                Selkies are graceful seal-folk who move between shore and sea, carrying the softness of human society and the sleek reserve of cold water in the same body. Even in human shape, they often seem oriented toward tide, rock and departure.

                Their strangeness is liminal rather than monstrous. Smooth movement, watchful eyes and sea-born composure mark them as people of coves and islands, at home in the spaces where landfolk become uncertain and the water takes over.

                They are traders, kin, lovers, exiles, rescuers or secretive neighbours depending on the coast that knows them. A selkie brings with them the ache of two homes and the knowledge that neither fully releases its claim.
                """,
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
                attributeProfile: Stats(0, 1, 2, 1, willpower: 1, perception: 2, aura: 2),
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
                """
                Myconids are humanoid fungal folk with cap-like heads, spongy flesh and a silence that feels communal rather than empty. They seem grown into personhood rather than born in the ordinary animal sense.

                Spores, damp darkness and patient exchange shape their presence. A myconid settlement is likely to feel less like a town than a living network of shared breath, hidden messages and slow decisions made beneath stone.

                They serve as underworld neighbours, spore-keepers, strange diplomats and eerie communities whose customs follow biology before politics. They are not automatically hostile, but they are never entirely familiar.
                """,
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
                attributeProfile: Stats(-1, 3, -1, -2, willpower: 2, aura: 2),
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
                """
                Plantfolk are humanoid vegetative beings of bark, fibre, stem and leaf. They stand close enough to personhood to speak, work and remember, yet every surface of them suggests growth rather than flesh.

                Season, light and soil matter to them in ways ordinary cultures cannot ignore. Their gestures may seem slow or deliberate, but the patience is not emptiness; it is the rhythm of something that measures change by rooting, blooming and withering.

                They can be gardeners, wardens, elders, wanderers or whole societies shaped by climate and place. A plantfolk presence makes the natural world feel less like a backdrop and more like a participant with a face.
                """,
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
                attributeProfile: Stats(2, 4, -1, -2, willpower: 2, aura: 2),
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
                """
                Ents are towering treefolk of bark, root and living wood, so large that personhood has to be read through the shape of trunk, branch and slow intent. Their movements feel ponderous only until one remembers how much mass is moving.

                They carry old forest memory in every knot, leaf and root-heavy limb. Knots, leaves, moss and root-heavy feet suggest seasons survived, fires remembered and paths watched long after shorter-lived peoples forgot why they mattered.

                They are wardens, shepherds and terrifying avengers of woodland places. An ent makes any forest negotiation feel unequal, because the other party may be speaking for centuries of patience.
                """,
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
                attributeProfile: Stats(7, 9, -3, -3, willpower: 5, perception: 1, aura: 4),
                bodypartHealthMultiplier: 1.6,
                canSwim: false,
                additionalCharacteristics:
                [
                    Characteristic("Bark Tone", "oak-brown", "ash-grey", "birch-pale", "yew-dark"),
                    Characteristic("Leaf Hue", "deep green", "silver-green", "gold", "rust red")
                ],
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Huorn"] = BeastRace(
                "Huorn",
                "Organic Humanoid",
                SizeCategory.Large,
                "Large Ungulate",
                GreatBeast(),
                """
                Huorns are dark half-wild tree-beings, less openly social than ents and harder to distinguish from hostile woodland until they move. The silhouette keeps the ambiguity of trunks and branches while adding hunger, anger and slow pursuit.

                They make stillness threatening. A huorn may stand like a tree for long stretches, then drag root and limb into motion with the terrible suggestion that the forest has decided to walk after all.

                They are old-wood hazards, angry guardians and marching trees for places where the boundary between plant and person has gone wrong. Travellers fear huorns because the safest-looking grove may already be watching.
                """,
                "Huorns work as dangerous old-forest hazards, angry woodland guardians and uncanny marching trees for games that want tree-shepherd ecology without making every awakened tree a social NPC.",
                Variants(
                    ("a shadowed huorn",
                        "This tree-being has a heavy trunk-like body and reaching limbs, its bark-dark outline almost still enough to be mistaken for ordinary woodland until it moves."),
                    ("a root-dragging huorn",
                        "This living tree drags itself forward on knotty roots and branchlike arms, carrying the oppressive patience of something that has waited through many seasons.")
                ),
                [
                    Attack("Jab", ItemQuality.Standard, "rhand", "lhand"),
                    Attack("Elbow", ItemQuality.Standard, "relbow", "lelbow")
                ],
                attributeProfile: Stats(8, 10, -4, -4, willpower: 4, perception: 1, aura: 3),
                bodypartHealthMultiplier: 1.75,
                canSwim: false,
                additionalCharacteristics:
                [
                    Characteristic("Bark Tone", "oak-brown", "ash-grey", "charcoal-black", "moss-dark"),
                    Characteristic("Leaf Hue", "deep green", "black-green", "rust red", "bare branches")
                ],
                combatStrategyKey: "Beast Behemoth"
            ),
            ["Dryad"] = HumanoidRace(
                "Dryad",
                "Organic Humanoid",
                SizeCategory.Normal,
                "Human Male",
                "Human Female",
                LongLivedHumanoid(),
                """
                Dryads are graceful tree-spirits whose humanoid forms carry bark-grain, leaf, blossom and the poise of living wood. Their beauty is not decorative alone; it is inseparable from place, season and the fierce intimacy of a chosen grove.

                They often seem less like visitors than expressions of the trees around them. A dryad's mood can make a glade feel welcoming, judging or dangerous, and even gentle movement may echo wind through branches rather than ordinary flesh.

                They are grove-keepers, emissaries, tempters, guardians and spirits of rooted memory. To deal with a dryad is rarely to deal with one person only; it is to meet a place in human shape.
                """,
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
                attributeProfile: Stats(-1, 1, 2, 2, willpower: 2, perception: 2, aura: 5),
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
                """
                Owlkin are feathered, winged people with broad faces, intense eyes and the quiet authority of nocturnal hunters. Their avian cast marks them without erasing their personhood, giving them a body suited to silence, height and sudden attention.

                They belong naturally to roosts, towers, cliff settlements, night libraries and sentry posts. Feathers soften their outline, but the eyes sharpen it again, making their stillness feel observant rather than passive.

                As a people, owlkin can be scholars, scouts, hunters, judges or watchful neighbours. Their presence brings the habits of the night into social space: patience, listening and the sense that little has gone unnoticed.
                """,
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
                attributeProfile: Stats(-1, 0, 2, 2, willpower: 1, perception: 3, aura: 1),
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
                """
                Avian people are broad-winged birdfolk whose forms remain largely humanoid while carrying feathers, wings and birdlike lines of face or limb. They read as citizens of height and air rather than monsters, but the body changes every assumption about movement.

                Roosting space, weather, lift and vantage shape how they live. Their wings are practical anatomy and social fact at once, influencing architecture, clothing, labour, defence and the politics of who controls the high places.

                They can be aerial scouts, couriers, artisans, soldiers or whole cultures organised around wind and vertical distance. An avian person makes a room feel built too low unless it has learned to include the sky.
                """,
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
                attributeProfile: Stats(-1, 0, 2, 2, willpower: 1, perception: 3, aura: 1),
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
                """
                Centaurs combine human torsos and arms with four-legged equine bodies, creating people whose scale, speed and stability differ sharply from ordinary humanoids. They are not riders and mounts fused for convenience; they are a complete body with its own balance and dignity.

                Open country suits them because distance answers the form. A centaur's stride changes travel, warfare, grazing, architecture and social etiquette, while the human upper body keeps tools, speech and gesture fully in play.

                They are natural nomads, outriders, herders, border guardians or settled peoples who must build the world to their own measure. A centaur makes the horizon feel socially relevant.
                """,
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
                attributeProfile: Stats(6, 5, 2, 0, willpower: 2, perception: 1),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Melee (Auto)"
            ),
            ["Pegacorn"] = BeastRace(
                "Pegacorn",
                "Ungulate",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                """
                Pegacorns combine the wings of a pegasus with the spiralled horn of a unicorn, giving the equine form both skyward power and sacred focus. They look almost impossible even beside other mythic horses, as if two separate omens had agreed to share one body.

                The wings promise speed, escape and command of open air; the horn adds judgment, sanctity and the danger of being chosen or refused. Around a pegacorn, ceremony comes easily because the creature already looks like a rare event.

                They serve as impossible mounts, heraldic wonders and signs of extraordinary favour. A pegacorn should feel scarce, coveted and difficult to approach without turning the encounter into a test.
                """,
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
                attributeProfile: Stats(7, 6, 4, 1, willpower: 4, perception: 3, aura: 6, intelligenceDiceExpression: "2d3"),
                bodypartHealthMultiplier: 1.7,
                combatStrategyKey: "Beast Swooper"
            ),
            ["Qilin"] = BeastRace(
                "Qilin",
                "Ungulate",
                SizeCategory.Large,
                "Horse",
                GreatBeast(),
                """
                Qilin are auspicious horned beasts of deerlike grace, scaled ornament and solemn presence. The form balances gentleness and authority, giving the impression that each step has been considered before the hoof touches ground.

                They carry the atmosphere of wise rule, sacred warning and restrained power. Horn, mane, scale and gaze make them beautiful, but not tame; a qilin's calm feels conditional on the moral shape of what stands before it.

                They are omens, guardian beasts and rare signs that a place or person has drawn numinous attention. A qilin encounter should feel less like spotting wildlife and more like being quietly weighed.
                """,
                "Qilin commonly fill the role of sacred omens, guardian beasts and symbols of wise rule, appearing less as ordinary mounts than as rare signs that a place or person has drawn the attention of the numinous world.",
                Variants(
                    ("a scale-maned qilin",
                        "This elegant mythic ungulate carries a staglike body, a singular horn and a mane that seems almost scaled or flame-touched along the neck."),
                    ("a solemn horned qilin",
                        "This sacred beast blends deerlike poise with draconic ornament, its calm gaze and measured steps making it seem more omen than animal.")
                ),
                [
                    Attack("Bite", ItemQuality.Bad, "mouth"),
                    Attack("Hoof Stomp", ItemQuality.Standard, "rfhoof", "lfhoof", "rrhoof", "lrhoof"),
                    Attack("Horn Gore", ItemQuality.Good, "horn")
                ],
                [
                    Usage("horn", "general")
                ],
                attributeProfile: Stats(5, 5, 4, 1, willpower: 4, perception: 3, aura: 6, intelligenceDiceExpression: "2d3"),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Skirmisher"
            ),
            ["Garuda"] = BeastRace(
                "Garuda",
                "Avian",
                SizeCategory.Large,
                "Raptor",
                GreatBeast(),
                """
                Garuda are immense mythic birds of prey with radiant bearing and devastating talons. They resemble raptors raised into sacred stature, every feathered line sharpened toward speed, height and violent purpose.

                Their nature is bound to sky guardianship and serpent-slaying force. A garuda does not merely fly above the world; it watches from a domain where coils, lies and ground-bound threats can be seen and struck from impossible angles.

                They may be royal messengers, divine hunters, protectors or terrifying enemies. Wherever they appear, the sky becomes an active power rather than empty distance.
                """,
                "Garudas are often cast as serpent-slayers, sky guardians and royal or sacred messengers, creatures whose aerial dominance makes them both protectors and devastating enemies.",
                Variants(
                    ("a radiant garuda",
                        "This huge raptorial beast has broad wings, a hooked beak and talons fit to carry prey from the earth with terrifying ease."),
                    ("a serpent-slaying garuda",
                        "This mythic bird looks built for high violent dives and crushing grips, its every feathered line sharpened toward the hunt.")
                ),
                [
                    Attack("Beak Peck", ItemQuality.Good, "beak"),
                    Attack("Beak Bite", ItemQuality.Standard, "beak"),
                    Attack("Talon Strike", ItemQuality.Good, "rtalons", "ltalons"),
                    Attack("Talon Carry", ItemQuality.Good, "rtalons", "ltalons")
                ],
                attributeProfile: BestialStats(6, 5, 5, 2, willpower: 4, perception: 5, aura: 4),
                bodypartHealthMultiplier: 1.5,
                combatStrategyKey: "Beast Dropper"
            ),
            ["Giant Eagle"] = BeastRace(
                "Giant Eagle",
                "Avian",
                SizeCategory.Large,
                "Raptor",
                GreatBeast(),
                """
                Giant eagles are immense keen-eyed raptors with the size and bearing to rule high thermals and remote eyries. Their wings can cast moving shadow over the ground, while their talons make the distance between sky and prey suddenly irrelevant.

                They carry a fierce intelligence without needing humanoid expression. The hooked beak, gold-bright gaze and broad shoulders suggest a creature that understands territory, debt and insult in its own uncompromising way.

                They can be noble allies, proud mounts, mountain powers or dangerous hunters. A giant eagle changes travel and warfare simply by existing above them.
                """,
                "Giant eagles are useful as noble aerial allies, remote mountain powers, dangerous rescue mounts or proud territorial hunters who can reshape travel and warfare wherever they nest.",
                Variants(
                    ("a broad-winged giant eagle",
                        "This enormous eagle has a commanding hooked beak, heavy talons and wings broad enough to cast a moving shadow over the ground below."),
                    ("a golden-eyed giant eagle",
                        "This great raptor carries itself with fierce intelligence, every feathered line suited to high thermals, sudden dives and the authority of the open sky.")
                ),
                [
                    Attack("Beak Peck", ItemQuality.Good, "beak"),
                    Attack("Beak Bite", ItemQuality.Standard, "beak"),
                    Attack("Talon Strike", ItemQuality.Good, "rtalons", "ltalons"),
                    Attack("Talon Carry", ItemQuality.Great, "rtalons", "ltalons"),
                    Attack("Wing Buffet", ItemQuality.Standard, "rwingbase", "lwingbase")
                ],
                attributeProfile: Stats(5, 4, 5, 2, willpower: 4, perception: 6, aura: 2, intelligenceDiceExpression: "2d4", auraDiceExpression: "1d2"),
                bodypartHealthMultiplier: 1.45,
                canSwim: false,
                additionalCharacteristics:
                [
                    Characteristic("Plumage Colour", "brown", "golden-brown", "white-headed", "storm-grey")
                ],
                combatStrategyKey: "Beast Dropper"
            ),
            ["Bunyip"] = BeastRace(
                "Bunyip",
                "Toed Quadruped",
                SizeCategory.Large,
                "Bear",
                Beast(),
                """
                Bunyips are ominous waterhole beasts of swamp, billabong and reed-choked dark. Their forms are usually heavy, wet and half-hidden, defined as much by what the water conceals as by what the witness manages to see.

                They make drinking places dangerous. Mud, ripples, sudden silence and the smell of stagnant water can all become part of the bunyip's presence, turning a practical stop on a journey into a boundary with the unknown.

                They work as regional monsters, warning stories and wetland predators tied to places where settlement thins out. A bunyip should make shallow water feel deep enough to have intentions.
                """,
                "Bunyips work well as regional monsters tied to dangerous wetlands, warning stories and places where ordinary drinking water becomes a boundary between settlement and the unknown.",
                Variants(
                    ("a mud-dark bunyip",
                        "This hulking wetland predator is shaggy, heavy-jawed and slick with dark water, its broad paws and low body suited to mud and ambush."),
                    ("a reed-stalking bunyip",
                        "This large beast carries itself with swamp-born menace, all blunt teeth, wet fur and sudden strength beneath a patient stillness.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Good, "mouth"),
                    Attack("Bite", ItemQuality.Standard, "mouth"),
                    Attack("Claw Swipe", ItemQuality.Standard, "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
                    Attack("Tail Slap", ItemQuality.Standard, "ltail")
                ],
                attributeProfile: BestialStats(6, 7, 0, -1, willpower: 4, perception: 2, aura: 1),
                bodypartHealthMultiplier: 1.6,
                combatStrategyKey: "Beast Drowner"
            ),
            ["Yacumama"] = BeastRace(
                "Yacumama",
                "Serpentine",
                SizeCategory.VeryLarge,
                "Giant Worm",
                GreatBeast(),
                """
                Yacumama are enormous river serpents so vast that the waterway around them can seem alive. Their coils and head belong to flood, depth and moving current rather than the scale of ordinary snakes.

                They are most frightening when only partly perceived: a swell against the current, a wake without a boat, a bank crumbling where no animal should be large enough to touch it. The river becomes both habitat and body.

                They are guardians, devourers and mythic terrors of great waters. A yacumama encounter turns travel by canoe, ferry or riverbank into the question of whether the river permits passage.
                """,
                "Yacumama fit as mythic guardians or terrors of Amazonian-scale rivers, embodying flood, depth and the fear that the water below a canoe might move with its own will.",
                Variants(
                    ("a river-vast yacumama",
                        "This colossal serpent has a body like a living current, its great head and endless coils suggesting a predator too large for ordinary banks to hold."),
                    ("a water-dark yacumama",
                        "This mythic river snake glistens with wet scales, its heavy coils and patient stare making the surrounding water feel suddenly unsafe.")
                ),
                [
                    Attack("Carnivore Bite", ItemQuality.Great, "mouth"),
                    Attack("Bite", ItemQuality.Good, "mouth"),
                    Attack("Tail Slap", ItemQuality.Good, "tail")
                ],
                attributeProfile: BestialStats(10, 9, 1, -1, willpower: 5, perception: 2, aura: 4),
                bodypartHealthMultiplier: 2.0,
                combatStrategyKey: "Beast Clincher"
            )
        };

		return templates
			.Select(x => ApplyMythicalSatiationLimits(x.Value))
			.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
    }

	private static MythicalRaceTemplate ApplyMythicalSatiationLimits(MythicalRaceTemplate template)
	{
		(double foodHours, double drinkHours) = GetMythicalSatiationCadence(template);
		(double maximumFood, double maximumDrink) =
			SatiationLimitSeederHelper.MaximumLimitsForCadence(foodHours, drinkHours);
		return template with
		{
			MaximumFoodSatiatedHours = maximumFood,
			MaximumDrinkSatiatedHours = maximumDrink
		};
	}

	private static (double FoodHours, double DrinkHours) GetMythicalSatiationCadence(MythicalRaceTemplate template)
	{
		return template.Name switch
		{
			"Dragon" or "Eastern Dragon" => (720.0, 168.0),
			"Griffin" or "Hippogriff" or "Pegasus" or "Pegacorn" or "Garuda" or "Giant Eagle" => (24.0, 12.0),
			"Unicorn" or "Qilin" => (48.0, 24.0),
			"Warg" => (12.0, 8.0),
			"Dire-Wolf" => (18.0, 8.0),
			"Dire-Bear" => (96.0, 36.0),
			"Minotaur" => (10.0, 6.0),
			"Naga" or "Basilisk" or "Cockatrice" => (720.0, 168.0),
			"Mermaid" or "Selkie" or "Hippocamp" => (24.0, 48.0),
			"Manticore" => (16.0, 8.0),
			"Wyvern" or "Fell Beast" => (168.0, 72.0),
			"Phoenix" => (48.0, 24.0),
			"Giant Beetle" or "Giant Ant" or "Giant Mantis" => (72.0, 24.0),
			"Giant Spider" => (336.0, 168.0),
			"Giant Scorpion" => (720.0, 336.0),
			"Giant Centipede" or "Ankheg" => (168.0, 72.0),
			"Giant Worm" => (336.0, 168.0),
			"Colossal Worm" => (720.0, 336.0),
			"Myconid" => (168.0, 72.0),
			"Plantfolk" => (96.0, 48.0),
			"Ent" or "Huorn" => (720.0, 168.0),
			"Dryad" => (72.0, 48.0),
			"Owlkin" or "Avian Person" => (10.0, 6.0),
			"Centaur" => (12.0, 8.0),
			"Bunyip" => (48.0, 24.0),
			"Yacumama" => (720.0, 168.0),
			_ when template.HumanoidVariety => (12.0, 6.0),
			_ when template.Size >= SizeCategory.Large => (24.0, 12.0),
			_ => (12.0, 6.0)
		};
	}

	internal static (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours) GetMythicalSatiationLimitsForTesting(
		MythicalRaceTemplate template)
	{
		return (template.MaximumFoodSatiatedHours, template.MaximumDrinkSatiatedHours);
	}

    internal static string BuildRaceDescriptionForTesting(MythicalRaceTemplate template)
    {
        return template.Description;
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
            "Talon Carry",
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
            "Talon Carry",
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

        if (Templates.Count != 43)
        {
            issues.Add($"Expected 43 mythical race templates but found {Templates.Count}.");
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

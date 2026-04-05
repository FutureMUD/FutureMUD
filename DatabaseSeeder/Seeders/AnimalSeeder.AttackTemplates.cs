#nullable enable

using MudSharp.Body;
using MudSharp.Combat;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.Health;
using System;
using System.Collections.Generic;

namespace DatabaseSeeder.Seeders;

public partial class AnimalSeeder
{
    private static Dictionary<string, AnimalAttackLoadoutTemplate> BuildAttackLoadouts()
    {
        static AnimalAttackGrant ShapeAttack(string key, ItemQuality quality)
        {
            return new(key, quality);
        }

        static AnimalAliasAttackGrant AliasAttack(string key, ItemQuality quality, params string[] aliases)
        {
            return new(key, quality, aliases);
        }

        static AnimalVenomAttackTemplate VenomAttack(
            string nameSuffix,
            string shape,
            string message,
            string venomProfile,
            double quantity,
            int woundSeverity,
            params string[] aliases)
        {
            return new AnimalVenomAttackTemplate(
                nameSuffix,
                shape,
                aliases,
                message,
                DamageType.Bite,
                venomProfile,
                quantity,
                woundSeverity
            );
        }

        return new Dictionary<string, AnimalAttackLoadoutTemplate>(StringComparer.OrdinalIgnoreCase)
        {
            ["nuisance-bite"] = new(
                [ShapeAttack("smallbite", ItemQuality.Terrible), ShapeAttack("smalllowbite", ItemQuality.Terrible), ShapeAttack("smallsmashbite", ItemQuality.Terrible)]
            ),
            ["small-herbivore"] = new(
                [ShapeAttack("smallbite", ItemQuality.ExtremelyBad), ShapeAttack("smallsmashbite", ItemQuality.Terrible)]
            ),
            ["small-predator"] = new(
                [
                    ShapeAttack("smallbite", ItemQuality.Standard),
                    ShapeAttack("smallsmashbite", ItemQuality.Standard),
                    ShapeAttack("smalllowbite", ItemQuality.Standard),
                    ShapeAttack("smalldownedbite", ItemQuality.Substandard)
                ],
                [
                    AliasAttack("clawswipe", ItemQuality.Poor, "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
                    AliasAttack("clawhighswipe", ItemQuality.Poor, "rfclaw", "lfclaw", "rrclaw", "lrclaw")
                ]
            ),
            ["cat"] = new(
                [
                    ShapeAttack("smallbite", ItemQuality.Bad),
                    ShapeAttack("smallsmashbite", ItemQuality.Bad),
                    ShapeAttack("smalldownedbite", ItemQuality.Terrible)
                ],
                [
                    AliasAttack("clawswipe", ItemQuality.Bad, "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
                    AliasAttack("clawhighswipe", ItemQuality.Bad, "rfclaw", "lfclaw", "rrclaw", "lrclaw")
                ]
            ),
            ["doglike"] = new(
                [
                    ShapeAttack("carnivorebite", ItemQuality.Poor),
                    ShapeAttack("carnivoresmashbite", ItemQuality.Poor),
                    ShapeAttack("carnivorelowbite", ItemQuality.Poor),
                    ShapeAttack("bite", ItemQuality.Poor)
                ],
                [
                    AliasAttack("clawlowswipe", ItemQuality.Bad, "rfclaw", "lfclaw", "rrclaw", "lrclaw")
                ]
            ),
            ["wolfpack"] = new(
                [
                    ShapeAttack("carnivorebite", ItemQuality.Standard),
                    ShapeAttack("carnivoreclinchbite", ItemQuality.Standard),
                    ShapeAttack("carnivorehighbite", ItemQuality.Standard),
                    ShapeAttack("carnivoredownbite", ItemQuality.Standard)
                ],
                [
                    AliasAttack("clawswipe", ItemQuality.Substandard, "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
                    AliasAttack("clawhighswipe", ItemQuality.Substandard, "rfclaw", "lfclaw", "rrclaw", "lrclaw")
                ]
            ),
            ["big-cat"] = new(
                [
                    ShapeAttack("carnivorebite", ItemQuality.VeryGood),
                    ShapeAttack("carnivoreclinchhighbite", ItemQuality.VeryGood),
                    ShapeAttack("carnivoreclinchhighestbite", ItemQuality.VeryGood),
                    ShapeAttack("carnivoredownbite", ItemQuality.Good)
                ],
                [
                    AliasAttack("clawswipe", ItemQuality.Standard, "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
                    AliasAttack("clawhighswipe", ItemQuality.Standard, "rfclaw", "lfclaw", "rrclaw", "lrclaw")
                ]
            ),
            ["bear"] = new(
                [
                    ShapeAttack("carnivorebite", ItemQuality.VeryGood),
                    ShapeAttack("bite", ItemQuality.Standard),
                    ShapeAttack("barge", ItemQuality.Standard),
                    ShapeAttack("bargesmash", ItemQuality.Standard)
                ],
                [
                    AliasAttack("clawswipe", ItemQuality.VeryGood, "rfclaw", "lfclaw", "rrclaw", "lrclaw"),
                    AliasAttack("clawhighswipe", ItemQuality.VeryGood, "rfclaw", "lfclaw", "rrclaw", "lrclaw")
                ]
            ),
            ["goat"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Standard),
                    ShapeAttack("barge", ItemQuality.Poor),
                    ShapeAttack("gorehorn", ItemQuality.Substandard),
                    ShapeAttack("hoofstomp", ItemQuality.Terrible)
                ]
            ),
            ["herbivore-charge"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Standard),
                    ShapeAttack("barge", ItemQuality.Poor),
                    ShapeAttack("bargesmash", ItemQuality.Poor),
                    ShapeAttack("hoofstomp", ItemQuality.Poor)
                ]
            ),
            ["tusked-herbivore"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Good),
                    ShapeAttack("barge", ItemQuality.Standard),
                    ShapeAttack("bargesmash", ItemQuality.Standard),
                    ShapeAttack("goretusk", ItemQuality.Substandard),
                    ShapeAttack("tusksweep", ItemQuality.Substandard)
                ]
            ),
            ["camelid-spitter"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Standard),
                    ShapeAttack("barge", ItemQuality.Poor),
                    ShapeAttack("bargesmash", ItemQuality.Poor),
                    ShapeAttack("llamaspit", ItemQuality.Standard)
                ]
            ),
            ["antlered-herbivore"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Standard),
                    ShapeAttack("barge", ItemQuality.Poor),
                    ShapeAttack("goreantler", ItemQuality.Standard),
                    ShapeAttack("hoofstomp", ItemQuality.Poor)
                ]
            ),
            ["bovid"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Good),
                    ShapeAttack("barge", ItemQuality.VeryGood),
                    ShapeAttack("bargesmash", ItemQuality.VeryGood),
                    ShapeAttack("gorehorn", ItemQuality.Standard),
                    ShapeAttack("hoofstomp", ItemQuality.Standard)
                ]
            ),
            ["hippo"] = new(
                [
                    ShapeAttack("carnivorebite", ItemQuality.Great),
                    ShapeAttack("bite", ItemQuality.Good),
                    ShapeAttack("barge", ItemQuality.VeryGood),
                    ShapeAttack("bargesmash", ItemQuality.VeryGood)
                ]
            ),
            ["rhino"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Poor),
                    ShapeAttack("barge", ItemQuality.Great),
                    ShapeAttack("bargesmash", ItemQuality.Great),
                    ShapeAttack("gorehorn", ItemQuality.Standard)
                ]
            ),
            ["elephant"] = new(
                [
                    ShapeAttack("herbivorebite", ItemQuality.Poor),
                    ShapeAttack("barge", ItemQuality.Great),
                    ShapeAttack("bargesmash", ItemQuality.Great),
                    ShapeAttack("goretusk", ItemQuality.Great),
                    ShapeAttack("tusksweep", ItemQuality.Good)
                ]
            ),
            ["fish"] = new(
                [
                    ShapeAttack("fishbite", ItemQuality.Substandard),
                    ShapeAttack("fishquickbite", ItemQuality.Substandard),
                    ShapeAttack("headram", ItemQuality.Terrible)
                ]
            ),
            ["shark"] = new(
                [
                    ShapeAttack("sharkbite", ItemQuality.Great),
                    ShapeAttack("sharkreelbite", ItemQuality.Great),
                    ShapeAttack("bite", ItemQuality.Standard)
                ]
            ),
            ["crab-small"] = new([ShapeAttack("crabpinch", ItemQuality.Bad), ShapeAttack("clawclamp", ItemQuality.Terrible)]),
            ["crab-large"] = new([ShapeAttack("crabpinch", ItemQuality.Substandard), ShapeAttack("clawclamp", ItemQuality.Poor)]),
            ["crab-giant"] = new([ShapeAttack("crabpinch", ItemQuality.Great), ShapeAttack("clawclamp", ItemQuality.Standard)]),
            ["cephalopod"] = new(
                [
                    ShapeAttack("fishbite", ItemQuality.Bad),
                    ShapeAttack("fishquickbite", ItemQuality.Bad),
                    ShapeAttack("headram", ItemQuality.Terrible)
                ]
            ),
            ["pinniped"] = new(
                [
                    ShapeAttack("carnivorebite", ItemQuality.Poor),
                    ShapeAttack("carnivoreclinchbite", ItemQuality.Poor)
                ]
            ),
            ["dolphin"] = new(
                [
                    ShapeAttack("fishbite", ItemQuality.Standard),
                    ShapeAttack("fishquickbite", ItemQuality.Standard)
                ],
                [AliasAttack("headram", ItemQuality.Poor, "head")]
            ),
            ["orca"] = new(
                [
                    ShapeAttack("sharkbite", ItemQuality.Heroic),
                    ShapeAttack("sharkreelbite", ItemQuality.Heroic),
                    ShapeAttack("bite", ItemQuality.Good)
                ],
                [AliasAttack("headram", ItemQuality.Standard, "head")]
            ),
            ["toothed-whale"] = new(
                [
                    ShapeAttack("sharkbite", ItemQuality.Great),
                    ShapeAttack("fishquickbite", ItemQuality.Standard)
                ],
                [AliasAttack("headram", ItemQuality.Standard, "head")]
            ),
            ["baleen-whale"] = new(
                [ShapeAttack("headbutt", ItemQuality.Good)],
                [
                    AliasAttack("headram", ItemQuality.Good, "head"),
                    AliasAttack("tailslap", ItemQuality.Good, "tail", "fluke", "peduncle")
                ]
            ),
            ["bird-small"] = new(
                [ShapeAttack("beakpeck", ItemQuality.Bad), ShapeAttack("talonstrike", ItemQuality.Bad), ShapeAttack("beakbite", ItemQuality.Terrible)]
            ),
            ["bird-fowl"] = new(
                [ShapeAttack("beakpeck", ItemQuality.Poor), ShapeAttack("talonstrike", ItemQuality.Poor), ShapeAttack("beakbite", ItemQuality.Terrible)]
            ),
            ["bird-raptor"] = new(
                [ShapeAttack("beakpeck", ItemQuality.Standard), ShapeAttack("talonstrike", ItemQuality.Standard), ShapeAttack("beakbite", ItemQuality.Poor)]
            ),
            ["bird-flightless"] = new(
                [ShapeAttack("beakpeck", ItemQuality.Substandard), ShapeAttack("talonstrike", ItemQuality.Substandard), ShapeAttack("beakbite", ItemQuality.Terrible)],
                [AliasAttack("headram", ItemQuality.Substandard, "head")]
            ),
            ["serpent-constrictor"] = new(
                [ShapeAttack("fangbite", ItemQuality.Standard), ShapeAttack("headram", ItemQuality.Bad)]
            ),
            ["serpent-neurotoxic"] = new(
                [ShapeAttack("fangbite", ItemQuality.Standard), ShapeAttack("headram", ItemQuality.Bad)],
                null,
                [
                    VenomAttack("Bite", "Fang", "@ strike|strikes with a quick bite at $1", "neurotoxic", 0.004, 2, "fangs")
                ]
            ),
            ["serpent-hemotoxic"] = new(
                [ShapeAttack("fangbite", ItemQuality.Standard), ShapeAttack("headram", ItemQuality.Bad)],
                null,
                [
                    VenomAttack("Bite", "Fang", "@ rear|rears back and drive|drives &0's fangs into $1", "hemotoxic", 0.006, 2, "fangs")
                ]
            ),
            ["serpent-cytotoxic"] = new(
                [ShapeAttack("fangbite", ItemQuality.Standard), ShapeAttack("headram", ItemQuality.Bad)],
                null,
                [
                    VenomAttack("Bite", "Fang", "@ lash|lashes forward and sink|sinks &0's fangs into $1", "cytotoxic", 0.005, 2, "fangs")
                ]
            ),
            ["jellyfish"] = new(
                [ShapeAttack("tendrillash", ItemQuality.Terrible)],
                null,
                [
                    new AnimalVenomAttackTemplate(
                        "Sting",
                        "Tendril",
                        [],
                        "@ drift|drifts close and brush|brushes a stinging tendril across $1",
                        DamageType.Cellular,
                        "irritant",
                        0.08,
                        0,
                        BuiltInCombatMoveType.EnvenomingAttackClinch,
                        MeleeWeaponVerb.Strike,
                        Alignment.Front,
                        Orientation.Low,
                        0.5,
                        1.5
                    )
                ]
            ),
            ["insect-mandible"] = new([ShapeAttack("mandiblebite", ItemQuality.Bad), ShapeAttack("headram", ItemQuality.Terrible)]),
            ["bombardier-beetle"] = new(
                [
                    ShapeAttack("mandiblebite", ItemQuality.Bad),
                    ShapeAttack("headram", ItemQuality.Terrible),
                    ShapeAttack("bombardierspray", ItemQuality.Standard)
                ]
            ),
            ["insect-stinger"] = new(
                [ShapeAttack("mandiblebite", ItemQuality.Poor), ShapeAttack("headram", ItemQuality.Terrible)],
                null,
                [
                    new AnimalVenomAttackTemplate(
                        "Sting",
                        "Stinger",
                        ["stinger"],
                        "@ dart|darts in and jab|jabs &0's stinger into $1",
                        DamageType.Piercing,
                        "irritant",
                        0.0015,
                        1,
                        BuiltInCombatMoveType.EnvenomingAttackClinch,
                        MeleeWeaponVerb.Stab,
                        Alignment.Front,
                        Orientation.Centre,
                        1.0,
                        0.8
                    )
                ]
            ),
            ["spider"] = new([ShapeAttack("fangbite", ItemQuality.Bad), ShapeAttack("arachnidclaw", ItemQuality.Bad)]),
            ["spider-venomous"] = new(
                [ShapeAttack("fangbite", ItemQuality.Bad), ShapeAttack("arachnidclaw", ItemQuality.Bad)],
                null,
                [
                    VenomAttack("Bite", "Fang", "@ dart|darts forward and sink|sinks &0's fangs into $1", "mixed", 0.0025, 1, "rfang", "lfang")
                ]
            ),
            ["tarantula"] = new(
                [ShapeAttack("fangbite", ItemQuality.Standard), ShapeAttack("arachnidclaw", ItemQuality.Poor)],
                null,
                [
                    VenomAttack("Bite", "Fang", "@ rear|rears up and lunge|lunges to bite $1 with &0's fangs", "irritant", 0.0035, 1, "rfang", "lfang")
                ]
            ),
            ["scorpion"] = new(
                [ShapeAttack("arachnidclaw", ItemQuality.Substandard), ShapeAttack("crabpinch", ItemQuality.Substandard)],
                null,
                [
                    new AnimalVenomAttackTemplate(
                        "Sting",
                        "Stinger",
                        ["stinger"],
                        "@ arch|arches &0's tail over and stab|stabs at $1 with &0's stinger",
                        DamageType.Piercing,
                        "neurotoxic",
                        0.003,
                        1,
                        BuiltInCombatMoveType.EnvenomingAttackClinch,
                        MeleeWeaponVerb.Stab,
                        Alignment.Rear,
                        Orientation.High
                    )
                ]
            ),
            ["reptile"] = new(
                [
                    ShapeAttack("carnivorebite", ItemQuality.Bad),
                    ShapeAttack("clawlowswipe", ItemQuality.Bad),
                    ShapeAttack("bite", ItemQuality.Terrible)
                ]
            ),
            ["crocodilian"] = new(
                [ShapeAttack("sharkbite", ItemQuality.Standard), ShapeAttack("bite", ItemQuality.Poor)],
                [AliasAttack("tailslap", ItemQuality.Standard, "utail", "mtail", "ltail", "tail")]
            ),
            ["chelonian"] = new(
                [
                    ShapeAttack("beakpeck", ItemQuality.Poor),
                    ShapeAttack("clawlowswipe", ItemQuality.Terrible),
                    ShapeAttack("beakbite", ItemQuality.Terrible)
                ]
            ),
            ["anuran"] = new(
                [
                    ShapeAttack("smallbite", ItemQuality.Terrible),
                    ShapeAttack("clawlowswipe", ItemQuality.Terrible)
                ]
            )
        };
    }

    private static Dictionary<string, AnimalVenomProfileTemplate> BuildVenomProfiles()
    {
        static AnimalVenomEffectTemplate Effect(DrugType type, double intensity, string additional = "")
        {
            return new(type, intensity, additional);
        }

        return new Dictionary<string, AnimalVenomProfileTemplate>(StringComparer.OrdinalIgnoreCase)
        {
            ["neurotoxic"] = new(
                "neurotoxic",
                1.2,
                0.05,
                DrugVector.Injected | DrugVector.Touched,
                LiquidInjectionConsequence.Harmful,
                "a thin, clear neurotoxic venom",
                "a thin, almost glassy venom that leaves a numb sting on contact",
                "It tastes sharply bitter and leaves your tongue tingling and numb.",
                "It tastes bitter and numbing.",
                "It smells faintly metallic and leaves an acrid sting in the nose.",
                "It smells acrid and metallic.",
                "bold cyan",
                [
                    Effect(DrugType.Paralysis, 0.8),
                    Effect(DrugType.VisionImpairment, 0.25),
                    Effect(DrugType.Nausea, 0.20)
                ]
            ),
            ["hemotoxic"] = new(
                "hemotoxic",
                1.0,
                0.04,
                DrugVector.Injected | DrugVector.Touched,
                LiquidInjectionConsequence.Harmful,
                "a dark, copper-scented hemotoxic venom",
                "a dark, slick venom with a coppery tang and a heavy scent of iron",
                "It tastes metallic and vile, and your gums instantly ache.",
                "It tastes metallic and foul.",
                "It smells like blood and wet copper.",
                "It smells like blood and metal.",
                "bold red",
                [
                    Effect(DrugType.BodypartDamage, 0.55,
                        new BodypartDamageAdditionalInfo
                        {
                            BodypartTypes = [BodypartTypeEnum.Heart, BodypartTypeEnum.Kidney, BodypartTypeEnum.Liver]
                        }.DatabaseString),
                    Effect(DrugType.Nausea, 0.20),
                    Effect(DrugType.ThermalImbalance, 0.15)
                ]
            ),
            ["cytotoxic"] = new(
                "cytotoxic",
                0.9,
                0.04,
                DrugVector.Injected | DrugVector.Touched,
                LiquidInjectionConsequence.Deadly,
                "a cloudy cytotoxic venom",
                "a cloudy, milky venom that burns where it touches exposed flesh",
                "It tastes sharp, sour and painfully caustic.",
                "It tastes painfully caustic.",
                "It smells chemical and faintly rotten.",
                "It smells chemical and rotten.",
                "bold magenta",
                [
                    Effect(DrugType.BodypartDamage, 0.65,
                        new BodypartDamageAdditionalInfo
                        {
                            BodypartTypes =
                            [
                                BodypartTypeEnum.Stomach,
                                BodypartTypeEnum.Intestines,
                                BodypartTypeEnum.Liver,
                                BodypartTypeEnum.Lung
                            ]
                        }.DatabaseString),
                    Effect(DrugType.Nausea, 0.25),
                    Effect(DrugType.VisionImpairment, 0.10)
                ]
            ),
            ["irritant"] = new(
                "irritant",
                0.6,
                0.06,
                DrugVector.Injected | DrugVector.Touched,
                LiquidInjectionConsequence.Harmful,
                "a sharp-smelling irritant venom",
                "a sharp-smelling irritant venom that leaves skin burning and inflamed",
                "It tastes bitter, oily and immediately irritating.",
                "It tastes bitter and irritating.",
                "It smells sharp enough to make your eyes water.",
                "It smells sharp and unpleasant.",
                "bold yellow",
                [
                    Effect(DrugType.Nausea, 0.25),
                    Effect(DrugType.VisionImpairment, 0.20),
                    Effect(DrugType.ThermalImbalance, 0.20)
                ]
            ),
            ["mixed"] = new(
                "mixed",
                1.1,
                0.05,
                DrugVector.Injected | DrugVector.Touched,
                LiquidInjectionConsequence.Deadly,
                "a complex mixed venom",
                "a complex mixed venom with both numbing and tissue-burning properties",
                "It tastes bitter, metallic and painfully sharp all at once.",
                "It tastes bitter, metallic and sharp.",
                "It smells acrid and metallic, with a faint rotten edge.",
                "It smells acrid, metallic and rotten.",
                "bold green",
                [
                    Effect(DrugType.Paralysis, 0.35),
                    Effect(DrugType.BodypartDamage, 0.35,
                        new BodypartDamageAdditionalInfo
                        {
                            BodypartTypes =
                            [
                                BodypartTypeEnum.Heart,
                                BodypartTypeEnum.Kidney,
                                BodypartTypeEnum.Liver,
                                BodypartTypeEnum.Lung
                            ]
                        }.DatabaseString),
                    Effect(DrugType.Nausea, 0.20),
                    Effect(DrugType.VisionImpairment, 0.15)
                ]
            )
        };
    }
}

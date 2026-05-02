using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class UsefulSeeder : IDatabaseSeeder
{
    private static readonly string[] StockAiExampleNames =
    [
        "CommandableOwner",
        "CommandableClanOutranks",
        "BasicDoorguard",
        "SparPartner",
        "RandomWanderer",
        "AggressiveToAllOtherSpecies",
        "RescueClanBrothers",
        "VerminScavenge",
        "TrackingAggressiveToAllOtherSpecies",
        "BasicSelfCare",
        "ExampleArenaParticipant",
        "ExampleArborealWanderer",
        "ExampleDenBuilder",
        "ExampleLairScavenger"
    ];

    private static readonly string[] StockItemMarkers =
    [
        "Container_Table",
        "Door_Normal_Tiny",
        "Insulation_Minor",
        "Destroyable_Misc",
        "Torch_Infinite",
        "Smokeable_Cigar",
        "DragAid_Stretcher",
        "Treatment_AntiInflammatory_Single",
        "TimePiece_PocketWatch"
    ];

    private static readonly string[] StockModernItemMarkers =
    [
        "Battery_AA",
        "BatteryPowered_4xAA",
        "BatteryPowered_4xAA_Connectable",
        "BatteryCharger_AA_4Bay",
        "ElectricGridCreator_Standard",
        "LiquidGridCreator_Standard",
        "TelecommunicationsGridCreator_Standard",
        "Telephone_Standard",
        "ElectricGridFeeder_Standard",
        "TelecommunicationsGridFeeder_Standard",
        "TelecommunicationsGridOutlet",
        "GridLiquidSource_Standard",
        "LiquidGridSupplier_Standard",
        "LiquidPump_Standard",
        "LiquidPump_Industrial",
        "LiquidConsumingProp_Standard",
        "LiquidConsumingProp_Basin",
        "BatteryPowered_LaptopStyle",
        "Battery_LiIon_Cell",
        "ElectricGridOutlet_Double",
        "GridPowerSupply_Standard",
        "UnlimitedGenerator_SetPiece",
        "PowerSocket_Mains_Double",
        "PowerSupply_60W",
        "ElectricLight_Medium",
        "HandheldRadio_Standard",
        "CellularPhone_Standard",
        "ComputerHost_Personal",
        "AutomationHousing_Panel",
        "SignalCableSegment_Standard",
        "GasContainer_OxygenSmall",
        "RcsThruster_Standard",
        "ZeroGravityTether_3Room",
        "ZeroGravityAnchor_SetPiece",
        "Defibrillator_AED",
        "ElectricHeaterCooler_SpaceHeater",
        "ElectricHeaterCooler_PortableCooler",
        "ConsumableHeaterCooler_SmallFire",
        "ConsumableHeaterCooler_Bonfire",
        "SolidFuelHeaterCooler_Fireplace",
        "SolidFuelHeaterCooler_WoodStove"
    ];

    public bool SafeToRunMoreThanOnce => true;

    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
        new List<(string Id, string Question,
            Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
            Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)>
        {
            ("ai",
                "Do you want to install the stock AI example package? This includes repeatable command, combat, scavenging and wandering examples, plus the newer arena, arboreal, den-builder and lair-scavenger samples.\n\nPlease answer #3yes#f or #3no#f: ",
                (context, questions) => ClassifyAiPackagePresence(context) != ShouldSeedResult.MayAlreadyBeInstalled,
                (answer, context) =>
                {
                    if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("covers",
                "Do you want to install a collection of simple ranged covers?\n\nPlease answer #3yes#f or #3no#f: ",
                (context, questions) => context.RangedCovers.Count() <= 1,
                (answer, context) =>
                {
                        if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("items",
                "#DItem Package 1#F\n\nDo you want to include a package of standard item definitions, which includes some commonly used item component types, including a wide selection of containers, liquid containers, doors, locks, keys, basic writing implements, insulation for clothing, components that let worn clothing hide or change characteristics (wigs, coloured contacts, etc), components that correct for myopia flaws, as well as identity obscurers (hoods, full helmets, niqabs, cloaks, etc.), destroyables, colour variables, further writing implements, tables and chairs, ranged covers, medical items, prosthetic limbs, dice, torches and lanterns, repair kits, water sources, smokeable tobacco, drag aids and timepieces.\n\nShall we install this package? Please answer #3yes#f or #3no#f: ",
                (context, questions) => true,
                (answer, context) =>
                {
                    if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("modernitems",
                "Do you want to install some common modern setting item component types like batteries, chargers, power plugs, powered lights, radios, electrical outlets and generators, telephones and cellular devices, computer and automation components, breathing and emergency medical gear, electric heaters and coolers, fireplaces, campfires, grid creators, liquid grids and fuel generators?\n\nPlease answer #3yes#f or #3no#f: ",
                (context, questions) => ClassifyModernPackagePresence(context) != ShouldSeedResult.MayAlreadyBeInstalled,
                (answer, context) =>
                {
                    if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("tags",
                "Do you want to install pre-made tags for use with items, crafts and projects? The main reason not to do this is if you are planning on an implementation that substantially differs from the one that comes with this seeder.\n\nPlease answer #3yes#f or #3no#f: ",
                (context, questions) => context.Tags.All(x => x.Name != "Aluminothermic Welding Portion"),
                (answer, context) =>
                {
                    if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("autobuilder",
                "Do you want to install the wilderness grouped autobuilder package for the stock terrain catalogue? This adds a terrain-aware grouped random-description room template, a random-features area template, and supporting terrain feature tags so builders can immediately generate wilderness-heavy areas that match the seeded terrains, especially when paired with Terrain Planner.\n\nPlease answer #3yes#f or #3no#f: ",
                (context, questions) => context.Terrains.Count() > 1 &&
                                        ClassifyAutobuilderPackagePresence(context) != ShouldSeedResult.MayAlreadyBeInstalled,
                (answer, context) =>
                {
                    if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("hints",
                "Do you want to install some newbie hints that will instruct new users about the key commands and engine concepts that they need to know?\n\nPlease answer #3yes#f or #3no#f: ",
                (context, questions) => context.NewPlayerHints.Count() == 0,
                (answer, context) =>
                {
                    if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("dreams",
                "Do you want to install some dream templates?\n\nPlease answer #3yes#f or #3no#f: ",
                (context, questions) => context.Dreams.Count() == 0,
                (answer, context) =>
                {
                    if (answer.EqualToAny("yes", "y", "no", "n")) { return (true, string.Empty); } return (false, "Invalid answer");
                }),
            ("dream-eras",
                """
                The dream seeder will install a number of universal human dreams that are not specific to one setting or time period. However, you can also seed a few additional setting or era specific packages as well.

                This could include the following:

                #Bmodern#0 - a collection of dreams from the modern day, including things like cars, clocks, mobile phones, computers, social media, etc.
                #Bold#0 - a collection of dreams from pre-modern periods, that include universal human experiences of that era like nature, harvests, famine, war, etc.

                Please list each of the extra packages you'd like to include separated by spaces, or simply a blank line to include only the universal ones: 
                """,
                (context, questions) => questions["dreams"].EqualToAny("yes", "y"),
                (answer, context) =>
                {
                    return (true, string.Empty);
                })
        };

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        _context = context;
        context.Database.BeginTransaction();
        List<string> errors = new();
        PrepareItemProtoCache(context);
        _tags = context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

        if (questionAnswers["tags"].EqualToAny("yes", "y"))
        {
            SeedTags(context, errors);
        }

        if (questionAnswers["ai"].EqualToAny("yes", "y"))
        {
            SeedAIExamples(context, errors);
        }

        if (questionAnswers["items"].EqualToAny("yes", "y"))
        {
            SeedItemsPart1(context, questionAnswers, errors);
            SeedItemsPart2(context, questionAnswers, errors);
            SeedItemsPart3(context, questionAnswers, errors);
            SeedItemsPart4(context, questionAnswers, errors);
        }

        if (questionAnswers["modernitems"].EqualToAny("yes", "y"))
        {
            SeedModernItems(context, errors);
        }

        if (questionAnswers["covers"].EqualToAny("yes", "y"))
        {
            SeedRangedCovers(context, errors);
        }

        if (questionAnswers["autobuilder"].EqualToAny("yes", "y"))
        {
            SeedTerrainAutobuilder(context, questionAnswers, errors);
        }

        if (questionAnswers["hints"].EqualToAny("yes", "y"))
        {
            SeedNewbieHints(context, errors);
        }

        if (questionAnswers["dreams"].EqualToAny("yes", "y"))
        {
            SeedDreams(context, questionAnswers);
        }

        context.Database.CommitTransaction();

        if (errors.Count == 0)
        {
            return "The operation completed successfully.";
        }

        return
            $"The operation completed with the following errors or warnings:\n\n{errors.ListToCommaSeparatedValues("\n")}";
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.Accounts.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        List<ShouldSeedResult> packageStates =
        [
            ClassifyAiPackagePresence(context),
            ClassifyItemPackagePresence(context),
            ClassifyModernPackagePresence(context),
            context.Tags.Any(x => x.Name == "Functions")
                ? ShouldSeedResult.MayAlreadyBeInstalled
                : ShouldSeedResult.ReadyToInstall
        ];

        if (context.Terrains.Count() > 1)
        {
            packageStates.Add(ClassifyAutobuilderPackagePresence(context));
        }

        return CombinePackageStates(packageStates.ToArray());
    }

    public int SortOrder => 200;
    public string Name => "Kickstart";
    public string Tagline => "A collection of useful stock items, AI, tags, covers and helpers";

    public string FullDescription =>
        @"This package gives options for a bunch of things that are not absolutely essential and that you might want to implement differently, but that I have already gone to the effort of having set up and think you might like to use.

This includes things like useful game item components, AI templates, helper tags, ranged covers, newbie hints and other building helpers.

Inside the package there are a few numbered #D""Core Item Packages""#3. The reason for this is that there have been updates to the useful seeder since its first release, and these sub-packages were for earlier adopters to update their existing MUDs with. I recommend that you install all of the Core Item Packages as they are appropriate for any MUD in nearly any setting.";

    private FuturemudDatabaseContext _context = null!;

    private Account _dbaccount => _context.Accounts.First();

    private Dictionary<string, GameItemComponentProto> _itemProtos = new(StringComparer.OrdinalIgnoreCase);

    internal static ShouldSeedResult ClassifyAiPackagePresence(FuturemudDatabaseContext context)
    {
        return SeederRepeatabilityHelper.ClassifyByPresence(
            StockAiExampleNames.Select(name => context.ArtificialIntelligences.Any(x => x.Name == name)));
    }

    internal static IReadOnlyCollection<string> StockAiExampleNamesForTesting => StockAiExampleNames;
    internal static IReadOnlyCollection<string> StockItemMarkersForTesting => StockItemMarkers;
    internal static IReadOnlyCollection<string> StockModernItemMarkersForTesting => StockModernItemMarkers;

    internal static ShouldSeedResult ClassifyItemPackagePresence(FuturemudDatabaseContext context)
    {
        return SeederRepeatabilityHelper.ClassifyByPresence(
            StockItemMarkers.Select(name => context.GameItemComponentProtos.Any(x => x.Name == name)));
    }

    internal static ShouldSeedResult ClassifyModernPackagePresence(FuturemudDatabaseContext context)
    {
        List<bool> presenceChecks = StockModernItemMarkers
            .Select(name => context.GameItemComponentProtos.Any(x => x.Name == name))
            .ToList();

        Tag? fuelTag = context.Tags.FirstOrDefault(x => x.Name == "Fuel");
        if (fuelTag is not null && context.LiquidsTags.Any(x => x.TagId == fuelTag.Id))
        {
            presenceChecks.Add(context.GameItemComponentProtos.Any(x => x.Type == "FuelHeaterCooler"));
            presenceChecks.Add(context.GameItemComponentProtos.Any(x => x.Type == "Fuel Generator"));
        }

        return SeederRepeatabilityHelper.ClassifyByPresence(presenceChecks);
    }

    private static ShouldSeedResult CombinePackageStates(params ShouldSeedResult[] packageStates)
    {
        if (packageStates.All(x => x == ShouldSeedResult.ReadyToInstall))
        {
            return ShouldSeedResult.ReadyToInstall;
        }

        if (packageStates.All(x => x == ShouldSeedResult.MayAlreadyBeInstalled))
        {
            return ShouldSeedResult.MayAlreadyBeInstalled;
        }

        return ShouldSeedResult.ExtraPackagesAvailable;
    }

    

    private void PrepareItemProtoCache(FuturemudDatabaseContext context)
    {
        _itemProtos = new Dictionary<string, GameItemComponentProto>(StringComparer.OrdinalIgnoreCase);
        foreach (GameItemComponentProto? item in context.GameItemComponentProtos.ToList())
        {
            if (item.EditableItem.RevisionStatus != 4)
            {
                continue;
            }

            _itemProtos[item.Name] = item;
        }
    }

    
    
    private RangedCover CreateOrGetRangedCover(FuturemudDatabaseContext context, string name, int coverType,
        int coverExtent, int highestPositionState, string descriptionString, string actionDescriptionString,
        int maximumSimultaneousCovers, bool coverStaysWhileMoving)
    {
        RangedCover? cover = context.RangedCovers.FirstOrDefault(x => x.Name == name);
        if (cover is not null)
        {
            return cover;
        }

        cover = new RangedCover
        {
            Name = name,
            CoverType = coverType,
            CoverExtent = coverExtent,
            HighestPositionState = highestPositionState,
            DescriptionString = descriptionString,
            ActionDescriptionString = actionDescriptionString,
            MaximumSimultaneousCovers = maximumSimultaneousCovers,
            CoverStaysWhileMoving = coverStaysWhileMoving
        };

        context.RangedCovers.Add(cover);
        return cover;
    }

    private void SeedTerrainAutobuilder(FuturemudDatabaseContext context,
            IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors)
    {
        SeedTerrainAutobuilderCore(context, errors);
    }

    private void SeedRangedCovers(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        if (context.RangedCovers.Any())
        {
            errors.Add("Detected that ranged covers were already installed. Did not seed any covers.");
            return;
        }

        List<(string Name, int Type, int Extent, int Position, string Desc, string Action, int Max, bool Moving)> covers = new()
        {
                        ("Uneven Ground", 0, 0, 6, "prone, using the uneven ground as cover", "$0 go|goes prone and begin|begins to use the uneven ground as cover", 0, true),
                        ("Corridor Doorway", 0, 0, 1, "using a doorway as cover", "$0 duck|ducks into a doorway and begin|begins to use it as cover", 0, false),
                        ("Large Crater", 0, 1, 6, "prone, using the edge of a large crater as cover", "$0 go|goes prone and begin|begins to use the edge of a large crater as cover", 0, false),
                        ("Chunk of Rubble", 1, 1, 12, "slumped up against $?0|$0|a large chunk of rubble|$ as cover", "$0 slump|slumps up against $?1|$1|a large chunk of rubble|$ and begin|begins to use it as cover", 2, false),
                        ("Tree", 1, 1, 1, "hiding behind $?0|$0|a tree|$ for cover", "$0 slip|slips behind $?1|$1|a tree|$ and use|uses it to protect &0's vital areas", 1, false),
                        ("Bush", 0, 1, 3, "hiding behind $?0|$0|a bush|$ for cover", "$0 take|takes position behind $?1|$1|a bush|$ and use|uses it to obscure &0's profile", 1, false),
                        ("Refuse Heap", 1, 1, 6, "half-hidden within $?0|$0|a pile of refuse|$, using it as cover", "$0 dive|dives into $?1|$1|a pile of refuse|$, using it to provide cover", 0, false),
                        ("Upright Table", 1, 0, 1, "using $?0|$0|a table|$ as cover", "$0 move|moves behind $?1|$1|a nearby table|$ and use|uses it to obscure &0's profile", 3, false),
                        ("Overturned Table", 1, 1, 3, "hiding behind $?0|$0|an overturned table|$ as cover", "$0 duck|ducks behind $?1|$1|an overturned table|$ and begin|begins to use it as cover", 3, false),
                        ("Smoke", 0, 1, 1, "obscured by $?0|$0|the smoke|$", "$0 move|moves into $?1|$1|the smoke|$ and uses it to obscure &0's form", 0, true),
                        ("Sandbag", 1, 1, 3, "hiding behind $?0|$0|a sandbag barricade|$, using it as cover", "$0 take|takes position behind $?1|$1|a sandbag barricade|$ and begin|begins to use it as cover", 5, false),
                        ("Stone Wall", 1, 2, 1, "hiding behind $?0|$0|a stone wall|$ for cover", "$0 slip|slips behind $?1|$1|a stone wall|$ and use|uses it for protection", 0, false),
                        ("Rubble Wall", 1, 2, 1, "hiding behind $?0|$0|a rubble wall|$ for cover", "$0 hide|hides behind $?1|$1|a rubble wall|$", 0, false),
                        ("Small Rock", 1, 0, 3, "crouched behind $?0|$0|a small rock|$", "$0 crouch|crouches behind $?1|$1|a small rock|$", 1, false),
                        ("Large Rock", 1, 1, 3, "hiding behind $?0|$0|a large rock|$", "$0 slip|slips behind $?1|$1|a large rock|$", 1, false),
                        ("Fallen Log", 1, 1, 3, "hiding behind $?0|$0|a fallen log|$", "$0 slip|slips behind $?1|$1|a fallen log|$", 1, false),
                        ("Pile of Crates", 1, 1, 1, "hiding behind $?0|$0|a pile of crates|$", "$0 hide|hides behind $?1|$1|a pile of crates|$", 2, false),
                        ("Barrel Stack", 1, 1, 1, "hiding behind $?0|$0|a stack of barrels|$", "$0 slip|slips behind $?1|$1|a stack of barrels|$", 2, false),
                        ("Low Hedge", 0, 0, 1, "hiding behind $?0|$0|a low hedge|$", "$0 duck|ducks behind $?1|$1|a low hedge|$", 1, false),
                        ("Thick Hedge", 0, 1, 1, "hiding behind $?0|$0|a thick hedge|$", "$0 take|takes cover behind $?1|$1|a thick hedge|$", 1, false),
                        ("Tall Grass", 0, 1, 6, "hiding in $?0|$0|tall grass|$", "$0 slip|slips into $?1|$1|tall grass|$", 2, true),
                        ("Shrubs", 0, 1, 3, "hiding behind $?0|$0|some shrubs|$", "$0 crouch|crouches behind $?1|$1|some shrubs|$", 1, false),
                        ("Vehicle", 1, 2, 1, "using $?0|$0|a vehicle|$ as cover", "$0 take|takes cover behind $?1|$1|a vehicle|$", 2, false),
                        ("Broken Vehicle", 1, 2, 3, "hiding behind $?0|$0|a broken vehicle|$", "$0 crouch|crouches behind $?1|$1|a broken vehicle|$", 2, false),
                        ("Collapsed Building", 1, 2, 5, "sheltering in $?0|$0|a collapsed building|$", "$0 dive|dives into $?1|$1|a collapsed building|$", 0, false),
                        ("Window Frame", 1, 0, 1, "using $?0|$0|a window frame|$ as cover", "$0 use|uses $?1|$1|a window frame|$ for cover", 1, false),
                        ("Ruined Wall", 1, 2, 1, "hiding behind $?0|$0|a ruined wall|$", "$0 take|takes cover behind $?1|$1|a ruined wall|$", 0, false),
                        ("Stalagmites", 1, 1, 1, "hiding among $?0|$0|stalagmites|$", "$0 dart|darts among $?1|$1|stalagmites|$", 2, false),
                        ("Pile of Junk", 1, 1, 3, "hiding behind $?0|$0|a pile of junk|$", "$0 crouch|crouches behind $?1|$1|a pile of junk|$", 1, false),
                        ("Pile of Bones", 1, 0, 3, "hiding behind $?0|$0|a pile of bones|$", "$0 crouch|crouches behind $?1|$1|a pile of bones|$", 1, false),
                        ("Dead Body", 1, 0, 3, "using $?0|$0|a dead body|$ as cover", "$0 crouch|crouches behind $?1|$1|a dead body|$", 1, false),
                        ("Sand Dune", 0, 1, 1, "hiding behind $?0|$0|a sand dune|$", "$0 use|uses $?1|$1|a sand dune|$ for cover", 0, true),
                        ("Snow Drift", 0, 1, 1, "hiding behind $?0|$0|a snow drift|$", "$0 hide|hides behind $?1|$1|a snow drift|$", 0, true),
                        ("Fallen Statue", 1, 1, 3, "hiding behind $?0|$0|a fallen statue|$", "$0 crouch|crouches behind $?1|$1|a fallen statue|$", 1, false),
                        ("Street Corner", 1, 2, 1, "using $?0|$0|a street corner|$ as cover", "$0 lean|leans around $?1|$1|a street corner|$", 0, false),
                        ("Alley Trash Bin", 1, 1, 1, "hiding behind $?0|$0|a trash bin|$", "$0 duck|ducks behind $?1|$1|a trash bin|$", 1, false),
                        ("Park Bench", 1, 0, 1, "using $?0|$0|a park bench|$ as cover", "$0 sit|sits behind $?1|$1|a park bench|$", 1, false),
                        ("Street Lamp", 1, 0, 1, "using $?0|$0|a street lamp|$ as cover", "$0 dodge|dodges behind $?1|$1|a street lamp|$", 1, false),
                        ("Old Well", 1, 2, 1, "using $?0|$0|an old well|$ as cover", "$0 hide|hides by $?1|$1|an old well|$", 1, false),
                        ("Rock Outcropping", 1, 2, 1, "hiding behind $?0|$0|a rock outcropping|$", "$0 slip|slips behind $?1|$1|a rock outcropping|$", 1, false),
                        ("Fallen Tree", 1, 1, 3, "hiding behind $?0|$0|a fallen tree|$", "$0 duck|ducks behind $?1|$1|a fallen tree|$", 1, false),
                        ("Fallen Pillar", 1, 1, 3, "hiding behind $?0|$0|a fallen pillar|$", "$0 duck|ducks behind $?1|$1|a fallen pillar|$", 1, false),
                        ("Thick Smoke", 0, 2, 1, "obscured by $?0|$0|thick smoke|$", "$0 move|moves into $?1|$1|thick smoke|$", 0, true),
                        ("Dense Fog", 0, 2, 1, "obscured by $?0|$0|dense fog|$", "$0 move|moves into $?1|$1|dense fog|$", 0, true),
                        ("Tall Reeds", 0, 1, 6, "hiding in $?0|$0|tall reeds|$", "$0 slip|slips into $?1|$1|tall reeds|$", 2, true),
                        ("Thick Seaweed", 0, 1, 6, "hiding in $?0|$0|thick seaweed|$", "$0 slip|slips into $?1|$1|thick seaweed|$", 2, true),
                        ("Dense Vegetation", 0, 2, 3, "hiding in $?0|$0|dense vegetation|$", "$0 move|moves into $?1|$1|dense vegetation|$", 2, true),
                        ("Boulder Cluster", 1, 2, 3, "hiding behind $?0|$0|a cluster of boulders|$", "$0 move|moves behind $?1|$1|a cluster of boulders|$", 2, false),
                        ("Abandoned Cart", 1, 1, 1, "hiding behind $?0|$0|an abandoned cart|$", "$0 duck|ducks behind $?1|$1|an abandoned cart|$", 1, false),
                        ("Bushy Tree", 1, 1, 1, "hiding behind $?0|$0|a bushy tree|$", "$0 slip|slips behind $?1|$1|a bushy tree|$", 1, false),
                        ("Shrubbery", 0, 1, 1, "hiding behind $?0|$0|some shrubbery|$", "$0 duck|ducks behind $?1|$1|some shrubbery|$", 1, false),
                        ("Counter", 1, 0, 1, "using $?0|$0|a counter|$ as cover", "$0 duck|ducks behind $?1|$1|a counter|$", 2, false),
                        ("Desk", 1, 0, 1, "using $?0|$0|a desk|$ as cover", "$0 duck|ducks behind $?1|$1|a desk|$", 1, false),
                        ("Staircase", 1, 1, 3, "hiding behind $?0|$0|a staircase|$", "$0 duck|ducks behind $?1|$1|a staircase|$", 1, false),
                        ("Corner", 1, 2, 1, "using $?0|$0|a corner|$ as cover", "$0 press|presses into $?1|$1|a corner|$", 0, false)
                };

        foreach ((string Name, int Type, int Extent, int Position, string Desc, string Action, int Max, bool Moving) item in covers)
        {
            context.RangedCovers.Add(new RangedCover
            {
                Name = item.Name,
                CoverType = item.Type,
                CoverExtent = item.Extent,
                HighestPositionState = item.Position,
                DescriptionString = item.Desc,
                ActionDescriptionString = item.Action,
                MaximumSimultaneousCovers = item.Max,
                CoverStaysWhileMoving = item.Moving
            });
        }
        context.SaveChanges();

        Dictionary<string, RangedCover> coversByName = context.RangedCovers.ToDictionary(x => x.Name, x => x);
        Dictionary<long, string> tagsById = context.Tags.ToDictionary(x => x.Id, x => x.Name);

        Dictionary<string, string[]> coversForTags = new()
        {
            ["Urban"] = new[]
                {
                                "Corridor Doorway", "Window Frame",
                                "Corner", "Street Corner", "Alley Trash Bin"
                        },
            ["Rural"] = new[]
                {
                                "Tree", "Bush", "Bushy Tree", "Shrubbery", "Fallen Log", "Fallen Tree",
                                "Old Well"
                        },
            ["Terrestrial"] = new[]
                {
                                "Uneven Ground", "Large Crater", "Stone Wall", "Rubble Wall", "Small Rock",
                                "Large Rock", "Boulder Cluster", "Rock Outcropping", "Sand Dune", "Snow Drift",
                                "Low Hedge", "Thick Hedge", "Tall Grass", "Shrubs", "Fallen Pillar"
                        },
            ["Aquatic"] = new[] { "Thick Seaweed" },
            ["Littoral"] = new[] { "Sand Dune", "Tall Reeds" },
            ["Riparian"] = new[] { "Tall Reeds", "Dense Vegetation" }
        };

        foreach (Terrain? terrain in context.Terrains.ToList())
        {
			List<string> tagNames = terrain.TagInformation?.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(x => long.TryParse(x, out long val) && tagsById.ContainsKey(val)
							? tagsById[val]
							: null)
					.OfType<string>()
					.ToList() ?? new List<string>();

            HashSet<long> coverIds = new();
			foreach (string tag in tagNames)
			{
				if (!coversForTags.TryGetValue(tag, out string[]? names))
                {
                    continue;
                }

                foreach (string name in names)
                {
                    if (coversByName.TryGetValue(name, out RangedCover? cover))
                    {
                        coverIds.Add(cover.Id);
                    }
                }
            }

            foreach (long id in coverIds)
            {
                context.TerrainsRangedCovers.Add(new TerrainsRangedCovers
                {
                    TerrainId = terrain.Id,
                    RangedCoverId = id
                });
            }
        }

        context.SaveChanges();
    }

    private void SeedAIExamples(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        FutureProg? alwaysTrue = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysTrue");
        FutureProg? alwaysFalse = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysFalse");
        if (alwaysTrue is null || alwaysFalse is null)
        {
            errors.Add(
                "Could not seed AI examples because the prerequisite AlwaysTrue or AlwaysFalse FutureProg was missing.");
            return;
        }

        EnsureVariableDefinition(context, ProgVariableTypes.Character, "npcownerid", ProgVariableTypes.Number);
        EnsureVariableDefault(context, ProgVariableTypes.Character, "npcownerid", "<var>0</var>");

        FutureProg ownerProg = EnsureAiProg(
            context,
            "IsOwnerCanCommand",
            "Commands",
            ProgVariableTypes.Boolean,
            "Determines if the character has been set as the owner of an NPC.",
            """
			var ownerid as number
			ownerid = ifnull(getregister(@tch, "npcownerid"), 0)
			return @ownerid == @ch.Id
			""",
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Character, "tch"),
            (ProgVariableTypes.Text, "cmd"));
        FutureProg cantCommandOwnerProg = EnsureAiProg(
            context,
            "WhyCantCommandNPCOwnerAI",
            "Commands",
            ProgVariableTypes.Text,
            "Returns an error message when a player cannot command an NPC they do not own.",
            @"return ""You are not the owner of "" + HowSeen(@ch, @tch, false, true) + "" and so you cannot issue commands.""",
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Character, "tch"),
            (ProgVariableTypes.Text, "cmd"));
        FutureProg outranksProg = EnsureAiProg(
            context,
            "OutranksCanCommand",
            "Commands",
            ProgVariableTypes.Boolean,
            "Determines if the character outranks the NPC in any clan and can therefore command them.",
            """
			foreach (clan in @tch.clans)
				if (outranks(@ch, @tch, @clan))
					return true
				end if
			end foreach
			return false
			""",
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Character, "tch"),
            (ProgVariableTypes.Text, "cmd"));
        FutureProg cantCommandOutrankProg = EnsureAiProg(
            context,
            "WhyCantCommandNPCClanOutranks",
            "Commands",
            ProgVariableTypes.Text,
            "Returns an error message when a player cannot command an NPC they do not outrank.",
            @"return ""You do not outrank "" + HowSeen(@ch, @tch, false, true) + "" in any clans.""",
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Character, "tch"),
            (ProgVariableTypes.Text, "cmd"));
        FutureProg doorguardWillOpen = EnsureAiProg(
            context,
            "DoorguardWillOpenDoor",
            "Doorguard",
            ProgVariableTypes.Boolean,
            "Determines whether a doorguard will open a door for a person.",
            "return isclanbrother(@guard, @ch)",
            (ProgVariableTypes.Character, "guard"),
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Exit, "exit"));
        FutureProg doorguardDelay = EnsureAiProg(
            context,
            "DoorguardActionDelay",
            "Doorguard",
            ProgVariableTypes.Number,
            "A delay in milliseconds between the action that triggers the doorguard and them taking the action.",
            "return 40+random(1,40)",
            (ProgVariableTypes.Character, "guard"),
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Exit, "exit"));
        FutureProg doorguardCloseDelay = EnsureAiProg(
            context,
            "DoorguardCloseDelay",
            "Doorguard",
            ProgVariableTypes.Number,
            "A delay in milliseconds between opening the door and closing the door.",
            "return 10000",
            (ProgVariableTypes.Character, "guard"),
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Exit, "exit"));
        FutureProg doorguardOpenDoor = EnsureAiProg(
            context,
            "DoorguardOpenDoor",
            "Doorguard",
            ProgVariableTypes.Void,
            "The actual action for the doorguard to take when opening the door.",
            """
			// Assumes doorguard has a key in their inventory
			force @guard ("emote move|moves to open the door for ~" + bestkeyword(@guard, @ch))
			force @guard ("unlock " + @exit.keyword)
			force @guard ("open " + @exit.keyword)
			""",
            (ProgVariableTypes.Character, "guard"),
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Exit, "exit"));
        FutureProg doorguardCloseDoor = EnsureAiProg(
            context,
            "DoorguardCloseDoor",
            "Doorguard",
            ProgVariableTypes.Void,
            "The actual action for the doorguard to take when closing the door.",
            """
			// Assumes doorguard has a key in their inventory
			force @guard ("emote move|moves to close the door")
			force @guard ("close " + @exit.keyword)
			force @guard ("lock " + @exit.keyword)
			""",
            (ProgVariableTypes.Character, "guard"),
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Exit, "exit"));
        FutureProg doorguardWontOpen = EnsureAiProg(
            context,
            "DoorguardWontOpen",
            "Doorguard",
            ProgVariableTypes.Void,
            "An action for the doorguard to take if someone nods or knocks but they cannot let them in.",
            """
			if (isnull(@exit) or @exit.Origin == @guard.Location)
				force @guard ("tell " + bestkeyword(@guard, @ch) + " I'm not allowed to let you through")
			else
				force @guard ("yell I'm not allowed to let you through")
			end if
			""",
            (ProgVariableTypes.Character, "guard"),
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Exit, "exit"));
        FutureProg doorguardWitnessStop = EnsureAiProg(
            context,
            "DoorguardWitnessStop",
            "Doorguard",
            ProgVariableTypes.Void,
            "An action for the doorguard to take if someone walks into a closed door.",
            """
			if (@DoorguardWillOpenDoor(@guard, @ch, @exit))
				force @guard ("tell " + bestkeyword(@guard, @ch) + " Give me a nod and I'll open the door for you")
			end if
			""",
            (ProgVariableTypes.Character, "guard"),
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Exit, "exit"));
        FutureProg aggressorWillAttack = EnsureAiProg(
            context,
            "TargetIsOtherRace",
            "Aggressor",
            ProgVariableTypes.Boolean,
            "Determines whether the aggressor will attack someone.",
            "return @ch.Race != @tch.Race",
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Character, "tch"));
        FutureProg rescuerWillRescue = EnsureAiProg(
            context,
            "RescuerWillRescue",
            "Combat",
            ProgVariableTypes.Boolean,
            "Determines whether a rescuer will rescue someone who is being attacked.",
            "return isclanbrother(@rescuer, @target)",
            (ProgVariableTypes.Character, "rescuer"),
            (ProgVariableTypes.Character, "target"));
        FutureProg verminWillScavenge = EnsureAiProg(
            context,
            "VerminWillScavenge",
            "Vermin",
            ProgVariableTypes.Boolean,
            "Determines whether a vermin AI will scavenge an item.",
            "return @item.isholdable and (@item.isfood or @item.iscorpse)",
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Item, "item"));
        FutureProg verminOnScavenge = EnsureAiProg(
            context,
            "VerminOnScavenge",
            "Vermin",
            ProgVariableTypes.Void,
            "Fires when a scavenger AI decides to scavenge an item.",
            """force @ch ("eat " + BestKeyword(@ch, @item))""",
            (ProgVariableTypes.Character, "ch"),
            (ProgVariableTypes.Item, "item"));
        FutureProg lairFallbackHome = EnsureAiProg(
            context,
            "LairScavengerFallbackHome",
            "Scavenger",
            ProgVariableTypes.Location,
            "Returns the NPC's current location as a fallback lair until it claims a den.",
            "return @ch.Location",
            (ProgVariableTypes.Character, "ch"));

        context.SaveChanges();

        EnsureArtificialIntelligence(
            context,
            "CommandableOwner",
            "Commandable",
            new XElement("Definition",
                new XElement("CanCommandProg", ownerProg.Id),
                new XElement("WhyCannotCommandProg", cantCommandOwnerProg.Id),
                new XElement("CommandIssuedEmote", "You issue the following command to $1: {0}"),
                new XElement("BannedCommands",
                    new XElement("BannedCommand", "ignoreforce"),
                    new XElement("BannedCommand", "return"))).ToString());
        EnsureArtificialIntelligence(
            context,
            "CommandableClanOutranks",
            "Commandable",
            new XElement("Definition",
                new XElement("CanCommandProg", outranksProg.Id),
                new XElement("WhyCannotCommandProg", cantCommandOutrankProg.Id),
                new XElement("CommandIssuedEmote", "You issue the following command to $1: {0}"),
                new XElement("BannedCommands",
                    new XElement("BannedCommand", "ignoreforce"),
                    new XElement("BannedCommand", "return"))).ToString());
        EnsureArtificialIntelligence(
            context,
            "BasicDoorguard",
            "Doorguard",
            new XElement("Definition",
                new XElement("WillOpenDoorForProg", doorguardWillOpen.Id),
                new XElement("WontOpenDoorForActionProg", doorguardWontOpen.Id),
                new XElement("OpenDoorActionProg", doorguardOpenDoor.Id),
                new XElement("CloseDoorActionProg", doorguardCloseDoor.Id),
                new XElement("BaseDelayProg", doorguardDelay.Id),
                new XElement("OpenCloseDelayProg", doorguardCloseDelay.Id),
                new XElement("OnWitnessDoorStopProg", doorguardWitnessStop.Id),
                new XElement("RespectGameRulesForOpeningDoors", false),
                new XElement("OwnSideOnly", false),
                new XElement("Social",
                    new XAttribute("Trigger", "nod"),
                    new XAttribute("TargettedOnly", true),
                    new XAttribute("Direction", false))).ToString());
        EnsureArtificialIntelligence(
            context,
            "SparPartner",
            "CombatEnd",
            new XElement("Definition",
                new XElement("WillAcceptTruce", alwaysTrue.Id),
                new XElement("WillAcceptTargetIncapacitated", alwaysTrue.Id),
                new XElement("OnOfferedTruce", 0),
                new XElement("OnTargetIncapacitated", 0),
                new XElement("OnNoNaturalTargets", 0)).ToString());
        EnsureArtificialIntelligence(
            context,
            "RandomWanderer",
            "Wanderer",
            new XElement("Definition",
                new XElement("FutureProg", alwaysTrue.Id),
                new XElement("WanderTimeDiceExpression", "1d40+100"),
                new XElement("TargetBody", 0),
                new XElement("TargetSpeed", 0),
                new XElement("EmoteText", new XCData(string.Empty))).ToString());
        EnsureArtificialIntelligence(
            context,
            "AggressiveToAllOtherSpecies",
            "Aggressor",
            new XElement("Definition",
                new XElement("WillAttackProg", aggressorWillAttack.Id),
                new XElement("EngageDelayDiceExpression", "1d200+200"),
                new XElement("EngageEmote", new XCData("@ move|moves aggressively towards $1"))).ToString());
        EnsureArtificialIntelligence(
            context,
            "RescueClanBrothers",
            "Rescuer",
            new XElement("Definition",
                new XElement("IsFriendProg", rescuerWillRescue.Id)).ToString());
        EnsureArtificialIntelligence(
            context,
            "VerminScavenge",
            "Scavenge",
            new XElement("Definition",
                new XElement("WillScavengeItemProg", verminWillScavenge.Id),
                new XElement("OnScavengeItemProg", verminOnScavenge.Id),
                new XElement("ScavengeDelayDiceExpression", "1d30+30")).ToString());
        EnsureArtificialIntelligence(
            context,
            "TrackingAggressiveToAllOtherSpecies",
            "TrackingAggressor",
            new XElement("Definition",
                new XElement("WillAttackProg", aggressorWillAttack.Id),
                new XElement("EngageDelayDiceExpression", "1d200+200"),
                new XElement("EngageEmote", new XCData("@ move|moves aggressively towards $1")),
                new XElement("MaximumRange", 2),
                new XElement("PathingEnabledProg", alwaysTrue.Id),
                new XElement("OpenDoors", false),
                new XElement("UseKeys", false),
                new XElement("SmashLockedDoors", false),
                new XElement("CloseDoorsBehind", false),
                new XElement("UseDoorguards", false),
                new XElement("MoveEvenIfObstructionInWay", false)).ToString());
        EnsureArtificialIntelligence(
            context,
            "BasicSelfCare",
            "SelfCare",
            new XElement("Definition",
                new XElement("BindingDelayDiceExpression", "3000+1d2000"),
                new XElement("BleedingEmoteDelayDiceExpression", "3000+1d2000"),
                new XElement("BleedingEmote", new XCData(@"@ shout|shouts out, ""I'm bleeding!"""))).ToString());
        EnsureArtificialIntelligence(
            context,
            "ExampleArenaParticipant",
            "ArenaParticipant",
            new XElement("Definition",
                new XElement("UseAmbushMode", true),
                new XElement("HideDuringPreparation", true),
                new XElement("UseSubtleSneak", true),
                new XElement("EngageDelayDiceExpression", new XCData("1d200+200")),
                new XElement("EngageEmote", new XCData("@ stalk|stalks into position against $1")),
                new XElement("OpenDoors", true),
                new XElement("UseKeys", false),
                new XElement("SmashLockedDoors", false),
                new XElement("CloseDoorsBehind", false),
                new XElement("UseDoorguards", false),
                new XElement("MoveEvenIfObstructionInWay", false)).ToString());
        EnsureArtificialIntelligence(
            context,
            "ExampleArborealWanderer",
            "ArborealWanderer",
            new XElement("Definition",
                new XElement("WillWanderIntoCellProg", alwaysTrue.Id),
                new XElement("IsWanderingProg", alwaysTrue.Id),
                new XElement("AllowDescentProg", alwaysFalse.Id),
                new XElement("WanderTimeDiceExpression", new XCData("1d180+180")),
                new XElement("EmoteText", new XCData("@ spring|springs from branch to branch.")),
                new XElement("PreferredTreeLayer", (int)RoomLayer.HighInTrees),
                new XElement("SecondaryTreeLayer", (int)RoomLayer.InTrees),
                new XElement("OpenDoors", false),
                new XElement("UseKeys", false),
                new XElement("SmashLockedDoors", false),
                new XElement("CloseDoorsBehind", false),
                new XElement("UseDoorguards", false),
                new XElement("MoveEvenIfObstructionInWay", false)).ToString());
        EnsureArtificialIntelligence(
            context,
            "ExampleDenBuilder",
            "DenBuilder",
            new XElement("Definition",
                new XElement("DenCraftId", 0),
                new XElement("DenSiteProg", alwaysTrue.Id),
                new XElement("BuildEnabledProg", alwaysTrue.Id),
                new XElement("WillDefendDenProg", alwaysFalse.Id),
                new XElement("AnchorItemProg", 0),
                new XElement("OpenDoors", false),
                new XElement("UseKeys", false),
                new XElement("SmashLockedDoors", false),
                new XElement("CloseDoorsBehind", false),
                new XElement("UseDoorguards", false),
                new XElement("MoveEvenIfObstructionInWay", false)).ToString());
        EnsureArtificialIntelligence(
            context,
            "ExampleLairScavenger",
            "LairScavenger",
            new XElement("Definition",
                new XElement("WillScavengeItemProg", verminWillScavenge.Id),
                new XElement("ScavengingEnabledProg", alwaysTrue.Id),
                new XElement("HomeLocationProg", lairFallbackHome.Id),
                new XElement("OpenDoors", false),
                new XElement("UseKeys", false),
                new XElement("SmashLockedDoors", false),
                new XElement("CloseDoorsBehind", false),
                new XElement("UseDoorguards", false),
                new XElement("MoveEvenIfObstructionInWay", false)).ToString());

        context.SaveChanges();
    }

    internal void SeedAIExamplesForTesting(FuturemudDatabaseContext context)
    {
        SeedAIExamples(context, new List<string>());
    }

    private static void EnsureVariableDefinition(FuturemudDatabaseContext context, ProgVariableTypes ownerType,
        string property, ProgVariableTypes containedType)
    {
        VariableDefinition? definition = context.VariableDefinitions.Local
                             .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property) ??
                         context.VariableDefinitions.AsEnumerable()
                             .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property);
        if (definition is null)
        {
            definition = new VariableDefinition
            {
                OwnerType = (long)ownerType,
                Property = property,
                ContainedType = (long)containedType
            };
            context.VariableDefinitions.Add(definition);
            return;
        }

        definition.OwnerType = (long)ownerType;
        definition.Property = property;
        definition.ContainedType = (long)containedType;
    }

    private static void EnsureVariableDefault(FuturemudDatabaseContext context, ProgVariableTypes ownerType,
        string property, string defaultValue)
    {
        VariableDefault? variableDefault = context.VariableDefaults.Local
                                  .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property) ??
                              context.VariableDefaults.AsEnumerable()
                                  .FirstOrDefault(x => x.OwnerType == (long)ownerType && x.Property == property);
        if (variableDefault is null)
        {
            variableDefault = new VariableDefault
            {
                OwnerType = (long)ownerType,
                Property = property,
                DefaultValue = defaultValue
            };
            context.VariableDefaults.Add(variableDefault);
            return;
        }

        variableDefault.OwnerType = (long)ownerType;
        variableDefault.Property = property;
        variableDefault.DefaultValue = defaultValue;
    }

    private static FutureProg EnsureAiProg(FuturemudDatabaseContext context, string functionName, string subcategory,
        ProgVariableTypes returnType, string comment, string text,
        params (ProgVariableTypes Type, string Name)[] parameters)
    {
        return SeederRepeatabilityHelper.EnsureProg(
            context,
            functionName,
            "AI",
            subcategory,
            returnType,
            comment,
            text,
            true,
            false,
            FutureProgStaticType.NotStatic,
            parameters);
    }

    private static void EnsureArtificialIntelligence(FuturemudDatabaseContext context, string name, string type,
        string definition)
    {
        ArtificialIntelligence ai = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.ArtificialIntelligences,
            name,
            x => x.Name,
            () =>
            {
                ArtificialIntelligence created = new();
                context.ArtificialIntelligences.Add(created);
                return created;
            });

        ai.Name = name;
        ai.Type = type;
        ai.Definition = definition;
    }

    private void SeedNewbieHints(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        if (context.NewPlayerHints.Any())
        {
            errors.Add("Detected that Newbie Hints were already installed. Did not seed any Newbie Hints.");
            return;
        }

        int index = 10000;

        void AddHint(string text, long? filterProgId = null, bool canRepeat = false)
        {
            context.NewPlayerHints.Add(new NewPlayerHint
            {
                Text = text,
                FilterProgId = filterProgId,
                Priority = index--,
                CanRepeat = canRepeat
            });
        }

        AddHint(@"You can use the #3help#0 command to get more information about the commands, the world, and many types of information.");
        AddHint(@"You can use the #3commands#0 command to get a list of all the commands that are available to you. In most cases, you can append #3?#0 to a command to get help about it, for example #3help ?#0 will show you the helpfile for the #3help#0 command.");
        AddHint(@"When describing command syntax, this engine uses #3<>#0 to show you text that you fill in, e.g. #3<target>#0 would be where you would put the keyword for something you're targeting.");
        AddHint(@"When describing command syntax, this engine uses #3[]#0 to show you that something is optional, e.g. #3[<target>]#0 would show you that supplying a target is optional, and you can leave it off.");
        AddHint(@"The #3score#0 command shows you many bits of key information about your character.");
        AddHint(@"The #3set#0 command allows you to edit both account settings and character settings. Use #3set#0 on its own to see your current settings, or #3set ?#0 to see help for the set command.");
        AddHint(@"The #3inventory#0 command allows you to see what you are carrying, wearing and wielding.");
        AddHint(@"The #3attributes#0 command allows you to see information about your attributes and their scores.");
        AddHint(@"The #3who#0 command shows you how many other players are currently online, as well as which game staff are online and available to assist you.");
        AddHint(@"If you need help from staff, you can use the #3petition#0 command. Petitions are shown to online staff, logged in a message board, and sent to their staff discord!");
        AddHint(@"You can use the #3notify#0 command to let people know that you're online and available for roleplay. Typically you would use this command when you're in a public place so that others could choose to have their characters also emerge into public for roleplay encounters. See #3notify ?#0 for information on how to use this command.");
        AddHint(@"There are numerous ways to roleplay with others: #3emote#0, #3say#0, #3tell#0, #3whisper#0, #3talk#0, and #3shout#0 are some of the key ones you should know about. See each of these command's respective helpfiles for more information.");
        AddHint(@"You can use the #3ooc#0 command to send an out of character message to everyone in your room, but use of this command is expected to be kept to a minimum.");
        AddHint(@"The #3look#0 command is used to look at the room you're in, as well as things and people that you can see. There are a few other extended uses of it like looking at graffiti, people's tattoos or scars, or inside things. See #3look ?#0 for more information and detailed syntax.");
        AddHint(@"The #3skills#0 command will show you which skills your character has and their current levels. You can alternatively use the #3skillcategories#0 command to see the same information, but with the skills grouped by category.");
        AddHint(@"You can use the #3skillevels#0 command to view the descriptors for a particular skill. Exact skill levels are typically hidden from you and you will instead see a descriptor representing a range.");
        AddHint(@"The #3survey#0 command will give you information about the location that you're currently in, such as terrain type, noise and light levels. You can also #3survey cover#0 to see available cover from ranged attacks in the location.");
        AddHint(@"There are several commands that let you see things in adjacent rooms. Having seen something with these commands is necessary before you can target things with a ranged attack.

The #3quickscan#0 (or #3qs#0) command is instant and can be used while moving, but only sees one room away and might miss details.
The #3scan#0 command takes time and cannot be used while moving, but sees up to two rooms away.
The #3longscan#0 command takes longer then scan but can potentially see much further, up to five rooms or more depending on your abilities.");
        AddHint("The #3point#0 command allows you to point out targets that you have seen with the various scan commands to other people in your room, so they can also target it.");
        AddHint("You can use the #3dub#0 command to remember a person or item with a custom keyword; for example, if you learn their name. You can use #3dubs#0 to see who and what you have dubbed. Some kinds of remotely-targeted commands like magical abilities require you to have dubbed your target first.");
        AddHint("You can use the #3alias#0 command to adopt a fake name that you may use in some contexts that would otherwise reveal your true name. You can use the #3names#0 command to see your real name and any aliases you have.");
        AddHint("You can use the #3introduce#0 command to mechanically let those present get a #3dub#0 on you with your currently adopted name. You should support the use of this command with some roleplay.");
        AddHint("The #3exits#0 command will show you what exits there are to leave the room you're currently in, and where they lead.");
        AddHint("You can use the #3count#0 command to count up the value of any money you are carrying on your person.");
        AddHint("The #3journal#0 command can be used to write in-character notes and observations for your character. Think of it like their mental diary of things they know and want to remember. Admins can view your journal and may use it to initiate plots for you.");
        AddHint("The #3plan#0 command allows you to set a short and a long term plan. These can remind you what you were up to between sessions, and staff can view these to help initiate plots.");
        AddHint("You can use the #3speed#0 command to set the speeds at which you will walk, crawl, swim, climb or even fly (if you're able).");
        AddHint("The #3socials#0 command will show you a list of special short-cut emotes that you can use to supplement your roleplay. The name of the social is the command to use it, and you can see more detailed information about it by using the #3social <which>#0 command.");
        AddHint("You can use the #3pmote#0 command to set a 'player emote' that others will see in your room description when they look at the room.");
        AddHint("You can use the #3omote#0 command to set an 'object emote' that others will see in a target item's room description when they look at the room.");
        AddHint("You can position yourself and items by using the #3position#0 command or one of its specific implementations (e.g. #3stand#0, #3kneel#0, #3sit#0, etc.). See #3position ?#0 for more information on this.");
    }
}

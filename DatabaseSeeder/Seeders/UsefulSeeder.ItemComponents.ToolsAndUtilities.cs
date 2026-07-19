#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System;

namespace DatabaseSeeder.Seeders;

public partial class UsefulSeeder
{
    private void SeedMedievalIndustryToolComponents(FuturemudDatabaseContext context, DateTime now, Account dbaccount,
        ref long nextId)
    {
        long currentId = nextId;

        XElement HandToolDefinition(double baseMultiplier = 1.5, double multiplierReductionPerQuality = 0.1,
            string durabilityExpression = "(1+quality) * 3600")
        {
            return new XElement("Definition",
                new XElement("MultiplierReductionPerQuality", multiplierReductionPerQuality),
                new XElement("BaseMultiplier", baseMultiplier),
                new XElement("ToolDurabilitySecondsExpression", durabilityExpression));
        }

        void AddToolComponent(string name, string description)
        {
            UpsertComponent(context, ref currentId, dbaccount, now, "HandTool", name, description,
                HandToolDefinition().ToString());
        }

        AddToolComponent("Tool_Blacksmithing_General",
            "Turns an item into a general hand-tool component for medieval blacksmithing and ordinary forge work.");
        AddToolComponent("Tool_Armouring_General",
            "Turns an item into a general hand-tool component for medieval armour shaping, forming and fitting.");
        AddToolComponent("Tool_Weaponsmithing_General",
            "Turns an item into a general hand-tool component for medieval weapon shaping, grinding and fitting.");
        AddToolComponent("Tool_Woodcrafting_General",
            "Turns an item into a general hand-tool component for medieval carpentry, joinery and wood shaping.");
        AddToolComponent("Tool_Coopering_General",
            "Turns an item into a general hand-tool component for medieval coopering and stave-vessel work.");
        AddToolComponent("Tool_Textilecraft_General",
            "Turns an item into a general hand-tool component for medieval spinning, weaving and textile preparation.");
        AddToolComponent("Tool_Dyeing_Fulling_General",
            "Turns an item into a general hand-tool component for medieval dyeing, fulling and cloth finishing.");
        AddToolComponent("Tool_Leatherworking_General",
            "Turns an item into a general hand-tool component for medieval leatherworking, tanning and hide preparation.");
        AddToolComponent("Tool_Parchmentmaking_General",
            "Turns an item into a general hand-tool component for medieval parchment scraping and stretching.");
        AddToolComponent("Tool_Papermaking_General",
            "Turns an item into a general hand-tool component for medieval papermaking and sheet forming.");
        AddToolComponent("Tool_Bookbinding_General",
            "Turns an item into a general hand-tool component for medieval bookbinding and codex assembly.");
        AddToolComponent("Tool_Pottery_General",
            "Turns an item into a general hand-tool component for medieval pottery shaping, trimming and finishing.");
        AddToolComponent("Tool_Masonry_General",
            "Turns an item into a general hand-tool component for medieval masonry, plastering and stone finishing.");
        AddToolComponent("Tool_Glassblowing_General",
            "Turns an item into a general hand-tool component for medieval glassblowing and hot-glass shaping.");
        AddToolComponent("Tool_Lapidary_General",
            "Turns an item into a general hand-tool component for medieval lapidary sawing, drilling and polishing.");
        AddToolComponent("Tool_Jewellery_General",
            "Turns an item into a general hand-tool component for medieval jewellery forming, setting and burnishing.");
        AddToolComponent("Tool_Apothecary_General",
            "Turns an item into a general hand-tool component for medieval apothecary preparation and dosing.");
        AddToolComponent("Tool_Medical_General",
            "Turns an item into a general hand-tool component for medieval medical and surgical tool use.");
        AddToolComponent("Tool_Printing_Woodblock_General",
            "Turns an item into a general hand-tool component for medieval woodblock carving, inking and impression work.");

        nextId = currentId;
    }

    private void SeedLighting(FuturemudDatabaseContext context, DateTime now, Account dbaccount, ref long nextId)
    {
        #region Lighting
        CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_Infinite",
                "Turns an item into an ever-burning torch.", 25, -1, false,
                "@ turn|turns on $1", "@ turn|turns off $1",
                "$0 begin|begins to flicker", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_1Hour",
                "Turns an item into a torch that burns for an hour.", 25, 3600, false,
                "@ turn|turns on $1", "@ turn|turns off $1",
                "$0 begin|begins to flicker", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_2Hour",
                "Turns an item into a torch that burns for two hours.", 25, 7200, false,
                "@ turn|turns on $1", "@ turn|turns off $1",
                "$0 begin|begins to flicker", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Torch_3Hour",
                "Turns an item into a torch that burns for 3 hours.", 25, 10800, false,
                "@ turn|turns on $1", "@ turn|turns off $1",
                "$0 begin|begins to flicker", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "SignalFire",
                "Turns an item into a bright signal fire that burns for 3 hours.", 500, 10800, true,
                "@ light|lights $1", "@ extinguish|extinguishes $1",
                "$0 begin|begins to flicker", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Match",
                "Turns an item into a match that burns dimly for only a few seconds.", 5, 20, false,
                "@ light|lights $1", "@ extinguish|extinguishes $1",
                "$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle",
                "Turns an item into a candle that burns dimly for 12 hours.", 5, 43200, false,
                "@ light|lights $1", "@ extinguish|extinguishes $1",
                "$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle_Long",
                "Turns an item into a candle that burns very dimly for 48 hours.", 3, 172800, false,
                "@ light|lights $1", "@ extinguish|extinguishes $1",
                "$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle_Bright",
                "Turns an item into a candle that burns a little dimly for 6 hours.", 8, 21600, false,
                "@ light|lights $1", "@ extinguish|extinguishes $1",
                "$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "Candle_Infinite",
                "Turns an item into an ever-burning candle.", 5, -1, false,
                "@ light|lights $1", "@ extinguish|extinguishes $1",
                "$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

        CreateTorchComponent(context, ref nextId, dbaccount, now, "BrightCandle_Infinite",
                "Turns an item into an ever-burning bright candle.", 8, -1, false,
                "@ light|lights on $1", "@ extinguish|extinguishes $1",
                "$0 begin|begins to flicker as it has almost totally burned down", "$0 have|has completely burned out");

        Liquid fuelLiquid = context.Liquids.FirstOrDefault(x => x.Name == "fuel") ??
            context.Liquids.First(x => x.Name == "water");
        CreateLanternComponent(context, ref nextId, dbaccount, now, "Lantern",
            "Turns an item into a lantern that burns any flammable fuel.",
            500, 0.2273046, false,
            "@ light|lights $1", "@ extinguish|extinguishes $1",
            "$0 begin|begins to splutter as the fuel runs low", "$0 have|has completely exhausted its fuel",
            fuelLiquid.Id, 0.000007892522);

        context.SaveChanges();
        #endregion
    }

    private void SeedWaterSources(FuturemudDatabaseContext context, DateTime now, Account dbaccount, ref long nextId)
    {
        #region Water Sources
        Liquid waterLiquid = context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_WaterSource",
            "Turns an item into a self-refilling source of water.",
            1000000, waterLiquid.Id, 0.8333333333333334, false);

        Liquid lakeLiquid = context.Liquids.FirstOrDefault(x => x.Name == "lake water") ??
            context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_LakeWaterSource",
            "Turns an item into a self-refilling source of lake water.",
            100000000, lakeLiquid.Id, 1000, false);

        Liquid springLiquid = context.Liquids.FirstOrDefault(x => x.Name == "spring water") ??
            context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_SpringWaterSource",
            "Turns an item into a self-refilling source of spring water.",
            100000000, springLiquid.Id, 1000, false);

        Liquid riverLiquid = context.Liquids.FirstOrDefault(x => x.Name == "river water") ??
            context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_RiverWaterSource",
            "Turns an item into a self-refilling source of river water.",
            100000000, riverLiquid.Id, 1000, false);

        Liquid liquid = context.Liquids.FirstOrDefault(x => x.Name == "swamp water") ??
            context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_SwampWaterSource",
            "Turns an item into a self-refilling source of swamp water.",
            100000000, liquid.Id, 1000, false);

        liquid = context.Liquids.FirstOrDefault(x => x.Name == "brackish water") ??
            context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_BrackishWaterSource",
            "Turns an item into a self-refilling source of brackish water.",
            100000000, liquid.Id, 1000, false);

        liquid = context.Liquids.FirstOrDefault(x => x.Name == "salt water") ??
            context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_SaltWaterSource",
            "Turns an item into a self-refilling source of salt water.",
            100000000, liquid.Id, 1000, false);

        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "WaterSource_Antiquity_PublicWell",
            "Turns an item into a stone public well or similar fixed potable water source.",
            1000000, waterLiquid.Id, 0.8333333333333334, false);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "WaterSource_Antiquity_Cistern",
            "Turns an item into a lined stone cistern with a large stored water supply.",
            50000, waterLiquid.Id, 0.0, false);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "WaterSource_Antiquity_Fountain",
            "Turns an item into a public fountain or flowing spout.",
            1000000, springLiquid.Id, 1000, false);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "WaterSource_Antiquity_BathPool",
            "Turns an item into a large bathhouse plunge pool.",
            5000, waterLiquid.Id, 10.0, false);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "WaterSource_Antiquity_RitualBasin",
            "Turns an item into a temple purification basin or other ritual water source.",
            100, waterLiquid.Id, 0.0, false);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "WaterSource_Antiquity_IrrigationOutlet",
            "Turns an item into an irrigation-channel outlet or agricultural water fixture.",
            100000000, riverLiquid.Id, 1000, false);

        Liquid tapWaterLiquid = context.Liquids.FirstOrDefault(x => x.Name == "tap water") ??
            context.Liquids.First(x => x.Name == "water");
        Liquid rainWaterLiquid = context.Liquids.FirstOrDefault(x => x.Name == "rain water") ??
            context.Liquids.First(x => x.Name == "water");
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_PublicTapWaterSource",
            "Turns an item into a public tap, hydrant-fed fixture or similar potable water point.",
            1000000, tapWaterLiquid.Id, 0.8333333333333334, true);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_DrinkingFountainWaterSource",
            "Turns an item into a public drinking fountain or bubbler.",
            1000000, tapWaterLiquid.Id, 0.25, true);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_PublicPumpWaterSource",
            "Turns an item into a hand pump, village pump or similar public potable water source.",
            1000000, springLiquid.Id, 0.5, true);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_StandpipeWaterSource",
            "Turns an item into a standpipe, yard spigot or high-flow public tap.",
            1000000, tapWaterLiquid.Id, 1.6666666666666667, true);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_PublicTroughWaterSource",
            "Turns an item into a public water trough with a steady self-refilling supply.",
            500, waterLiquid.Id, 0.5, false);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Infinite_PublicCisternWaterSource",
            "Turns an item into a large communal cistern with a managed refill supply.",
            10000, rainWaterLiquid.Id, 0.25, false);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Sink_5L",
            "Turns an item into a 5L sink that can be filled up.",
            5, tapWaterLiquid.Id, 0.8333333333333334, true);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Sink_20L",
            "Turns an item into a 20L sink that can be filled up.",
            20, tapWaterLiquid.Id, 0.8333333333333334, true);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Sink_50L",
            "Turns an item into a 50L sink that can be filled up.",
            50, tapWaterLiquid.Id, 0.8333333333333334, true);
        CreateWaterSourceComponent(context, ref nextId, dbaccount, now, "Bathtub",
            "Turns an item into a 500L bathtub that can be filled up.",
            500, tapWaterLiquid.Id, 0.8333333333333334, true);

        context.SaveChanges();
        #endregion
    }

    private void SeedRepairKits(FuturemudDatabaseContext context, DateTime now, Account dbaccount, ref long nextId)
    {
        #region Repair Kits

        long currentId = nextId;
        DictionaryWithDefault<string, Material> materials = context.Materials.AsEnumerable().DistinctBy(x => x.Name).ToDictionaryWithDefault(x => x.Name, StringComparer.OrdinalIgnoreCase);
        DictionaryWithDefault<string, TraitDefinition> skills = context.TraitDefinitions.AsEnumerable().DistinctBy(x => x.Name).ToDictionaryWithDefault(x => x.Name, StringComparer.OrdinalIgnoreCase);
        DamageType[] damagetypes = new DamageType[]
        {
            DamageType.Slashing,
            DamageType.Chopping,
            DamageType.Crushing,
            DamageType.Piercing,
            DamageType.Ballistic,
            DamageType.Burning,
            DamageType.Freezing,
            DamageType.Chemical,
            DamageType.Shockwave,
            DamageType.Bite,
            DamageType.Claw,
            DamageType.Shearing,
            DamageType.BallisticArmourPiercing,
            DamageType.Wrenching,
            DamageType.Shrapnel,
            DamageType.Falling,
            DamageType.ArmourPiercing
        };

        void AddRepairKitType(string name, string description, WoundSeverity maximumSeverity, double repairPoints, long? traitId, double checkBonus, string[] materialBehaviourTypes, string[] requiredTags)
        {
            AddRepairKitTypeWithMaterials(name, description, maximumSeverity, repairPoints, traitId, checkBonus, materialBehaviourTypes, [], requiredTags);
        }

        void AddRepairKitTypeWithMaterials(string name, string description, WoundSeverity maximumSeverity, double repairPoints, long? traitId, double checkBonus, string[] materialBehaviourTypes, string[] materialNames, string[] requiredTags)
        {
            List<Material> repairMaterials = materials.Values
                .Where(x => materialBehaviourTypes.Any(y => y.Equals(((MaterialBehaviourType)(x.BehaviourType ?? 0)).DescribeEnum(), StringComparison.OrdinalIgnoreCase)))
                .ToList();

            foreach (string materialName in materialNames)
            {
                Material? material = materials.Values
                    .FirstOrDefault(x => x.Name.Equals(materialName, StringComparison.OrdinalIgnoreCase));
                if (material is not null)
                {
                    repairMaterials.Add(material);
                }
            }

            repairMaterials = repairMaterials
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .ToList();

            UpsertComponent(context, ref currentId, dbaccount, now, "RepairKit", $"Repair_{name}", $"Turns an item into {description}",
                new XElement("Definition",
                        new XElement("MaximumSeverity", (int)maximumSeverity),
                        new XElement("RepairPoints", repairPoints),
                        new XElement("CheckTrait", traitId ?? skills.Values.First().Id),
                        new XElement("CheckBonus", checkBonus),
                        new XElement("Echoes",
                            new XElement("Echo", new XCData("$0 take|takes up $2, rifling through it for the necessary tools to fix $1")),
                            new XElement("Echo", new XCData("$0 begin|begins repairing $1 with $2")),
                            new XElement("Echo", new XCData("$0 continue|continues repairing $1 with $2")),
                            new XElement("Echo", new XCData("$0 finish|finishes repairing $1, then place|places the tools back within $2 and pack|packs it away."))
                        ),
                        new XElement("DamageTypes",
                            from type in damagetypes
                            select new XElement("DamageType", (int)type)
                        ),
                        new XElement("Materials",
                            from material in repairMaterials
                            select new XElement("Material", material.Id)
                        ),
                        new XElement("Tags",
                            from tag in requiredTags
                            select new XElement("Tag", _tags[tag].Id)
                        )
                    ).ToString());
        }

        AddRepairKitType("Cloth", "a repair kit that repairs cloth items", WoundSeverity.Grievous, 500, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 0.0, ["Fabric", "Hair", "Feather"], []);
        AddRepairKitType("Cloth_Good", "a good-quality repair kit that repairs cloth items", WoundSeverity.Horrifying, 750, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 1.0, ["Fabric", "Hair", "Feather"], []);
        AddRepairKitType("Cloth_Poor", "a poor-quality repair kit that repairs cloth items", WoundSeverity.Severe, 300, (skills["Tailoring"] ?? skills["Tailor"])?.Id, -1.0, ["Fabric", "Hair", "Feather"], []);

        AddRepairKitType("Leather", "a repair kit that repairs leather items", WoundSeverity.Grievous, 500, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 0.0, ["Leather", "Skin", "Flesh"], []);
        AddRepairKitType("Leather_Good", "a good-quality repair kit that repairs leather items", WoundSeverity.Horrifying, 750, (skills["Tailoring"] ?? skills["Tailor"])?.Id, 1.0, ["Leather", "Skin", "Flesh"], []);
        AddRepairKitType("Leather_Poor", "a poor-quality repair kit that repairs leather items", WoundSeverity.Severe, 300, (skills["Tailoring"] ?? skills["Tailor"])?.Id, -1.0, ["Leather", "Skin", "Flesh"], []);

        AddRepairKitType("Metal_Armour", "a repair kit that repairs metal armour", WoundSeverity.Grievous, 1000, (skills["Armourcrafting"] ?? skills["Armourer"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 0.0, ["Metal"], ["Armour"]);
        AddRepairKitType("Metal_Armour_Good", "a good-quality repair kit that repairs metal armour", WoundSeverity.Horrifying, 1500, (skills["Armourcrafting"] ?? skills["Armourer"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 1.0, ["Metal"], ["Armour"]);
        AddRepairKitType("Metal_Armour_Poor", "a poor-quality repair kit that repairs metal armour", WoundSeverity.Severe, 600, (skills["Armourcrafting"] ?? skills["Armourer"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, -1.0, ["Metal"], ["Armour"]);

        AddRepairKitType("Metal_Weapon", "a repair kit that repairs metal weapons", WoundSeverity.Grievous, 1000, (skills["Weaponcrafting"] ?? skills["Weaponsmith"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 0.0, ["Metal"], ["Weapons"]);
        AddRepairKitType("Metal_Weapon_Good", "a good-quality repair kit that repairs metal weapons", WoundSeverity.Horrifying, 1500, (skills["Weaponcrafting"] ?? skills["Weaponsmith"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 1.0, ["Metal"], ["Weapons"]);
        AddRepairKitType("Metal_Weapon_Poor", "a poor-quality repair kit that repairs metal weapons", WoundSeverity.Severe, 600, (skills["Weaponcrafting"] ?? skills["Weaponsmith"] ?? skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, -1.0, ["Metal"], ["Weapons"]);

        AddRepairKitType("Metal_Tool", "a repair kit that repairs metal tools", WoundSeverity.Grievous, 1000, (skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 0.0, ["Metal"], ["Tools"]);
        AddRepairKitType("Metal_Tool_Good", "a good-quality repair kit that repairs metal tools", WoundSeverity.Horrifying, 1500, (skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, 1.0, ["Metal"], ["Tools"]);
        AddRepairKitType("Metal_Tool_Poor", "a poor-quality repair kit that repairs metal tools", WoundSeverity.Severe, 600, (skills["Blacksmithing"] ?? skills["Blacksmith"])?.Id, -1.0, ["Metal"], ["Tools"]);

        AddRepairKitType("Wood", "a repair kit that repairs wooden items", WoundSeverity.Grievous, 1000, (skills["Carpentry"] ?? skills["Woodcraft"] ?? skills["Constructing"] ?? skills["Construction"])?.Id, 0.0, ["Wood"], []);
        AddRepairKitType("Wood_Good", "a good-quality repair kit that repairs wooden items", WoundSeverity.Horrifying, 1500, (skills["Carpentry"] ?? skills["Woodcraft"] ?? skills["Constructing"] ?? skills["Construction"])?.Id, 1.0, ["Wood"], []);
        AddRepairKitType("Wood_Poor", "a poor-quality repair kit that repairs wooden items", WoundSeverity.Severe, 600, (skills["Carpentry"] ?? skills["Woodcraft"] ?? skills["Constructing"] ?? skills["Construction"])?.Id, -1.0, ["Wood"], []);

        AddRepairKitTypeWithMaterials("Glass", "a repair kit that repairs glass items", WoundSeverity.Grievous, 1000, (skills["Glassworking"] ?? skills["Glasswork"] ?? skills["Masonry"] ?? skills["Stonecraft"] ?? skills["Crafting"])?.Id, 0.0, [], ["glass", "silicate glass", "soda-lime glass", "lead glass"], []);
        AddRepairKitTypeWithMaterials("Glass_Good", "a good-quality repair kit that repairs glass items", WoundSeverity.Horrifying, 1500, (skills["Glassworking"] ?? skills["Glasswork"] ?? skills["Masonry"] ?? skills["Stonecraft"] ?? skills["Crafting"])?.Id, 1.0, [], ["glass", "silicate glass", "soda-lime glass", "lead glass"], []);
        AddRepairKitTypeWithMaterials("Glass_Poor", "a poor-quality repair kit that repairs glass items", WoundSeverity.Severe, 600, (skills["Glassworking"] ?? skills["Glasswork"] ?? skills["Masonry"] ?? skills["Stonecraft"] ?? skills["Crafting"])?.Id, -1.0, [], ["glass", "silicate glass", "soda-lime glass", "lead glass"], []);

        AddRepairKitTypeWithMaterials("Paper", "a repair kit that repairs paper, parchment and papyrus items", WoundSeverity.Grievous, 1000, (skills["Papermaking"] ?? skills["Scribing"] ?? skills["Scribe"] ?? skills["Crafting"])?.Id, 0.0, [], ["paper", "parchment", "papyrus"], []);
        AddRepairKitTypeWithMaterials("Paper_Good", "a good-quality repair kit that repairs paper, parchment and papyrus items", WoundSeverity.Horrifying, 1500, (skills["Papermaking"] ?? skills["Scribing"] ?? skills["Scribe"] ?? skills["Crafting"])?.Id, 1.0, [], ["paper", "parchment", "papyrus"], []);
        AddRepairKitTypeWithMaterials("Paper_Poor", "a poor-quality repair kit that repairs paper, parchment and papyrus items", WoundSeverity.Severe, 600, (skills["Papermaking"] ?? skills["Scribing"] ?? skills["Scribe"] ?? skills["Crafting"])?.Id, -1.0, [], ["paper", "parchment", "papyrus"], []);

        AddRepairKitTypeWithMaterials("Lacquer", "a repair kit that repairs lacquerware items", WoundSeverity.Grievous, 1000, (skills["Lacquering"] ?? skills["Woodcraft"] ?? skills["Carpentry"] ?? skills["Crafting"])?.Id, 0.0, [], ["lacquer"], []);
        AddRepairKitTypeWithMaterials("Lacquer_Good", "a good-quality repair kit that repairs lacquerware items", WoundSeverity.Horrifying, 1500, (skills["Lacquering"] ?? skills["Woodcraft"] ?? skills["Carpentry"] ?? skills["Crafting"])?.Id, 1.0, [], ["lacquer"], []);
        AddRepairKitTypeWithMaterials("Lacquer_Poor", "a poor-quality repair kit that repairs lacquerware items", WoundSeverity.Severe, 600, (skills["Lacquering"] ?? skills["Woodcraft"] ?? skills["Carpentry"] ?? skills["Crafting"])?.Id, -1.0, [], ["lacquer"], []);

        AddRepairKitTypeWithMaterials("Cordage", "a repair kit that repairs cordage items", WoundSeverity.Grievous, 1000, (skills["Ropemaking"] ?? skills["Textilecraft"] ?? skills["Tailoring"] ?? skills["Crafting"])?.Id, 0.0, [], [], ["Cordage"]);
        AddRepairKitTypeWithMaterials("Cordage_Good", "a good-quality repair kit that repairs cordage items", WoundSeverity.Horrifying, 1500, (skills["Ropemaking"] ?? skills["Textilecraft"] ?? skills["Tailoring"] ?? skills["Crafting"])?.Id, 1.0, [], [], ["Cordage"]);
        AddRepairKitTypeWithMaterials("Cordage_Poor", "a poor-quality repair kit that repairs cordage items", WoundSeverity.Severe, 600, (skills["Ropemaking"] ?? skills["Textilecraft"] ?? skills["Tailoring"] ?? skills["Crafting"])?.Id, -1.0, [], [], ["Cordage"]);

        AddRepairKitTypeWithMaterials("Composite_Bow", "a repair kit that repairs composite bows", WoundSeverity.Grievous, 1000, (skills["Bowmaking"] ?? skills["Weaponcrafting"] ?? skills["Carpentry"] ?? skills["Crafting"])?.Id, 0.0, [], [], ["Composite Bow"]);
        AddRepairKitTypeWithMaterials("Composite_Bow_Good", "a good-quality repair kit that repairs composite bows", WoundSeverity.Horrifying, 1500, (skills["Bowmaking"] ?? skills["Weaponcrafting"] ?? skills["Carpentry"] ?? skills["Crafting"])?.Id, 1.0, [], [], ["Composite Bow"]);
        AddRepairKitTypeWithMaterials("Composite_Bow_Poor", "a poor-quality repair kit that repairs composite bows", WoundSeverity.Severe, 600, (skills["Bowmaking"] ?? skills["Weaponcrafting"] ?? skills["Carpentry"] ?? skills["Crafting"])?.Id, -1.0, [], [], ["Composite Bow"]);

        AddRepairKitType("Metal", "a repair kit that repairs metal items", WoundSeverity.Grievous, 1000, (skills["Blacksmithing"] ?? skills["Metalcraft"] ?? skills["Blacksmith"])?.Id, 0.0, ["Metal"], []);
        AddRepairKitType("Metal_Good", "a good-quality repair kit that repairs metal items", WoundSeverity.Horrifying, 1500, (skills["Blacksmithing"] ?? skills["Metalcraft"] ?? skills["Blacksmith"])?.Id, 1.0, ["Metal"], []);
        AddRepairKitType("Metal_Poor", "a poor-quality repair kit that repairs metal items", WoundSeverity.Severe, 600, (skills["Blacksmithing"] ?? skills["Metalcraft"] ?? skills["Blacksmith"])?.Id, -1.0, ["Metal"], []);

        AddRepairKitType("Stone", "a repair kit that repairs stone items", WoundSeverity.Grievous, 1000, (skills["Masonry"] ?? skills["Stonecraft"] ?? skills["Constructing"] ?? skills["Construction"])?.Id, 0.0, ["Stone"], []);
        AddRepairKitType("Stone_Good", "a good-quality repair kit that repairs stone items", WoundSeverity.Horrifying, 1500, (skills["Masonry"] ?? skills["Stonecraft"] ?? skills["Constructing"] ?? skills["Construction"])?.Id, 1.0, ["Stone"], []);
        AddRepairKitType("Stone_Poor", "a poor-quality repair kit that repairs stone items", WoundSeverity.Severe, 600, (skills["Masonry"] ?? skills["Stonecraft"] ?? skills["Constructing"] ?? skills["Construction"])?.Id, -1.0, ["Stone"], []);

        AddRepairKitType("Ceramic", "a repair kit that repairs ceramic items", WoundSeverity.Grievous, 1000, (skills["Pottery"] ?? skills["Potter"] ?? skills["Masonry"] ?? skills["Stonecraft"])?.Id, 0.0, ["Ceramic"], []);
        AddRepairKitType("Ceramic_Good", "a good-quality repair kit that repairs ceramic items", WoundSeverity.Horrifying, 1500, (skills["Pottery"] ?? skills["Potter"] ?? skills["Masonry"] ?? skills["Stonecraft"])?.Id, 1.0, ["Ceramic"], []);
        AddRepairKitType("Ceramic_Poor", "a poor-quality repair kit that repairs ceramic items", WoundSeverity.Severe, 600, (skills["Pottery"] ?? skills["Potter"] ?? skills["Masonry"] ?? skills["Stonecraft"])?.Id, -1.0, ["Ceramic"], []);

        AddRepairKitType("Hard_Organic", "a repair kit that repairs hard organic items", WoundSeverity.Grievous, 1000, (skills["Scrimshawing"] ?? skills["Scrimshaw"] ?? skills["Carpentry"] ?? skills["Woodcraft"])?.Id, 0.0, ["Bone", "Shell", "Horn", "Tooth", "Scale", "Claw", "Beak"], []);
        AddRepairKitType("Hard_Organic_Good", "a good-quality repair kit that repairs hard organic items", WoundSeverity.Horrifying, 1500, (skills["Scrimshawing"] ?? skills["Scrimshaw"] ?? skills["Carpentry"] ?? skills["Woodcraft"])?.Id, 1.0, ["Bone", "Shell", "Horn", "Tooth", "Scale", "Claw", "Beak"], []);
        AddRepairKitType("Hard_Organic_Poor", "a poor-quality repair kit that repairs hard organic items", WoundSeverity.Severe, 600, (skills["Scrimshawing"] ?? skills["Scrimshaw"] ?? skills["Carpentry"] ?? skills["Woodcraft"])?.Id, -1.0, ["Bone", "Shell", "Horn", "Tooth", "Scale", "Claw", "Beak"], []);

        AddRepairKitType("Universal", "a repair kit that repairs anything", WoundSeverity.Severe, 250, (skills["Salvaging"] ?? skills["Salvage"])?.Id, -1.0, [], []);
        AddRepairKitType("Universal_Good", "a good-quality repair kit that repairs anything", WoundSeverity.VerySevere, 350, (skills["Salvaging"] ?? skills["Salvage"])?.Id, 0.0, [], []);
        AddRepairKitType("Universal_Poor", "a poor-quality repair kit that repairs anything", WoundSeverity.Moderate, 150, (skills["Salvaging"] ?? skills["Salvage"])?.Id, -2.0, [], []);
        nextId = currentId;
        #endregion
    }

    private void SeedAdditionalBuilderExamples(FuturemudDatabaseContext context, DateTime now, Account dbaccount,
        ref long nextId)
    {
        #region Additional Builder Examples

        long currentId = nextId;
        Liquid waterLiquid = context.Liquids.First(x => x.Name == "water");

        GameItemComponentProto AddExtraComponent(string type, string name, string description, XElement definition)
        {
            return CreateComponent(context, ref currentId, dbaccount, now, type, name, description, definition.ToString());
        }

        XElement LocksmithingDefinition(int difficultyAdjustment, bool usableForInstallation,
            bool usableForConfiguration, bool usableForFabrication, bool breakable)
        {
            return new XElement("Definition",
                new XElement("DifficultyAdjustment", difficultyAdjustment),
                new XElement("UsableForInstallation", usableForInstallation),
                new XElement("UsableForConfiguration", usableForConfiguration),
                new XElement("UsableForFabrication", usableForFabrication),
                new XElement("Breakable", breakable));
        }

        XElement ShopStallDefinition(double weight, SizeCategory maximumSize, bool transparent, Difficulty forceDifficulty,
            Difficulty pickDifficulty, string lockType)
        {
            return new XElement("Definition",
                new XAttribute("Weight", weight),
                new XAttribute("MaxSize", (int)maximumSize),
                new XAttribute("Preposition", "on"),
                new XAttribute("Transparent", transparent),
                new XElement("ForceDifficulty", (int)forceDifficulty),
                new XElement("PickDifficulty", (int)pickDifficulty),
                new XElement("LockEmote", new XCData("@ secure|secures $1$?2| with $2||$.")),
                new XElement("UnlockEmote", new XCData("@ unfasten|unfastens $1$?2| with $2||$.")),
                new XElement("LockEmoteNoActor", new XCData("@ settle|settles into a secured state.")),
                new XElement("UnlockEmoteNoActor", new XCData("@ release|releases its fastening.")),
                new XElement("LockType", lockType));
        }

        XElement MarketGoodWeightDefinition(params (string CategoryName, decimal Multiplier)[] multipliers)
        {
            XElement multiplierElement = new("Multipliers");
            foreach ((string categoryName, decimal multiplier) in multipliers)
            {
                MarketCategory? category = context.MarketCategories.FirstOrDefault(x => x.Name == categoryName);
                if (category is null)
                {
                    continue;
                }

                multiplierElement.Add(new XElement("Multiplier",
                    new XAttribute("category", category.Id),
                    new XAttribute("value", multiplier)));
            }

            return new XElement("Definition", multiplierElement);
        }

        AddExtraComponent("LockingContainer", "LockingContainer_Lockbox",
            "Turns an item into a small lockbox with a built-in lever lock.",
            new XElement("Definition",
                new XAttribute("Weight", 2500),
                new XAttribute("MaxSize", (int)SizeCategory.Tiny),
                new XAttribute("Preposition", "in"),
                new XAttribute("Transparent", false),
                new XElement("ForceDifficulty", (int)Difficulty.Hard),
                new XElement("PickDifficulty", (int)Difficulty.Normal),
                new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$.")),
                new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$.")),
                new XElement("LockEmoteNoActor", new XCData("@ click|clicks shut.")),
                new XElement("UnlockEmoteNoActor", new XCData("@ click|clicks open.")),
                new XElement("LockType", "Lever Lock")));
        AddExtraComponent("LockingContainer", "LockingContainer_Footlocker",
            "Turns an item into a large locking footlocker or strongbox.",
            new XElement("Definition",
                new XAttribute("Weight", 25000),
                new XAttribute("MaxSize", (int)SizeCategory.Normal),
                new XAttribute("Preposition", "in"),
                new XAttribute("Transparent", false),
                new XElement("ForceDifficulty", (int)Difficulty.VeryHard),
                new XElement("PickDifficulty", (int)Difficulty.Hard),
                new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$.")),
                new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$.")),
                new XElement("LockEmoteNoActor", new XCData("@ thunk|thunks shut.")),
                new XElement("UnlockEmoteNoActor", new XCData("@ clunk|clunks open.")),
                new XElement("LockType", "Ward Lock")));
        AddExtraComponent("LockingContainer", "LockingContainer_SafeChest",
            "Turns an item into a heavy safe-style locking chest.",
            new XElement("Definition",
                new XAttribute("Weight", 125000),
                new XAttribute("MaxSize", (int)SizeCategory.Large),
                new XAttribute("Preposition", "in"),
                new XAttribute("Transparent", false),
                new XElement("ForceDifficulty", (int)Difficulty.ExtremelyHard),
                new XElement("PickDifficulty", (int)Difficulty.VeryHard),
                new XElement("LockEmote", new XCData("@ spin|spins the tumblers on $1 and lock|locks it.")),
                new XElement("UnlockEmote", new XCData("@ work|works the tumblers on $1 and unlock|unlocks it.")),
                new XElement("LockEmoteNoActor", new XCData("@ seal|seals itself with a heavy metallic clunk.")),
                new XElement("UnlockEmoteNoActor", new XCData("@ release|releases its locking bolts with a heavy clunk.")),
                new XElement("LockType", "Safe Lock")));

        AddExtraComponent("Keyring", "Keyring_Small",
            "Turns an item into a small keyring for a handful of keys.",
            new XElement("Definition", new XElement("MaximumNumberOfKeys", 4)));
        AddExtraComponent("Keyring", "Keyring_Large",
            "Turns an item into a large janitorial-style keyring.",
            new XElement("Definition", new XElement("MaximumNumberOfKeys", 20)));

        AddExtraComponent("Locksmithing Tool", "Locksmithing_Poor",
            "Turns an item into a poor set of breakable locksmithing tools.",
            new XElement("Definition",
                new XElement("DifficultyAdjustment", -2),
                new XElement("UsableForInstallation", true),
                new XElement("UsableForConfiguration", true),
                new XElement("UsableForFabrication", false),
                new XElement("Breakable", true)));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Standard",
            "Turns an item into a standard set of locksmithing tools.",
            new XElement("Definition",
                new XElement("DifficultyAdjustment", 0),
                new XElement("UsableForInstallation", true),
                new XElement("UsableForConfiguration", true),
                new XElement("UsableForFabrication", true),
                new XElement("Breakable", true)));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Fine",
            "Turns an item into a fine set of locksmithing tools.",
            new XElement("Definition",
                new XElement("DifficultyAdjustment", 2),
                new XElement("UsableForInstallation", true),
                new XElement("UsableForConfiguration", true),
                new XElement("UsableForFabrication", true),
                new XElement("Breakable", false)));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Installation",
            "Turns an item into locksmithing tools intended for installation work.",
            new XElement("Definition",
                new XElement("DifficultyAdjustment", 1),
                new XElement("UsableForInstallation", true),
                new XElement("UsableForConfiguration", false),
                new XElement("UsableForFabrication", false),
                new XElement("Breakable", false)));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Fabrication",
            "Turns an item into locksmithing tools intended for lock and key fabrication.",
            new XElement("Definition",
                new XElement("DifficultyAdjustment", 1),
                new XElement("UsableForInstallation", false),
                new XElement("UsableForConfiguration", false),
                new XElement("UsableForFabrication", true),
                new XElement("Breakable", false)));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Antiquity_BronzePoor",
            "Turns an item into a penalty-bearing, breakable set of low-tech bronze lockpicks.",
            LocksmithingDefinition(-2, false, false, false, true));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Antiquity_BronzeStandard",
            "Turns an item into a standard antiquity lockpick and probe roll.",
            LocksmithingDefinition(0, false, true, false, true));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Antiquity_FineSteel",
            "Turns an item into a rare fine steel lockpick set for elite low-tech locksmithing.",
            LocksmithingDefinition(2, false, true, false, false));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Antiquity_Installation",
            "Turns an item into an antiquity lock installation and configuration kit.",
            LocksmithingDefinition(1, true, true, false, false));
        AddExtraComponent("Locksmithing Tool", "Locksmithing_Antiquity_Fabrication",
            "Turns an item into an antiquity key filing and lock fabrication kit.",
            LocksmithingDefinition(1, false, false, true, false));

        AddExtraComponent("PencilSharpener", "PencilSharpener",
            "Turns an item into a pencil sharpener.",
            new XElement("Definition",
                new XElement("SharpenEmote", new XCData("$0 brace|braces $2 against $1 and sharpen|sharpens it to a fine point."))));

        RangedCover? uprightTableCover = context.RangedCovers.FirstOrDefault(x => x.Name == "Upright Table");
        RangedCover? overturnedTableCover = context.RangedCovers.FirstOrDefault(x => x.Name == "Overturned Table");
        if (uprightTableCover is not null && overturnedTableCover is not null)
        {
            AddExtraComponent("Bench", "Bench_Double",
                "Makes an item a compact bench with two seating positions and standard flippable cover behaviour.",
                new XElement("Definition",
                    new XAttribute("MaximumChairSlots", 2),
                    new XAttribute("Chair", 0),
                    new XAttribute("ChairCount", 0),
                    new XElement("Cover",
                        new XElement("Flipped", overturnedTableCover.Id),
                        new XElement("NotFlipped", uprightTableCover.Id),
                        new XElement("Expression", new XCData("0")),
                        new XElement("Message", new XCData("@ try|tries to flip $1, but are|is not strong enough.")))));
            AddExtraComponent("Bench", "Bench_Triple",
                "Makes an item a larger bench with three seating positions and standard flippable cover behaviour.",
                new XElement("Definition",
                    new XAttribute("MaximumChairSlots", 3),
                    new XAttribute("Chair", 0),
                    new XAttribute("ChairCount", 0),
                    new XElement("Cover",
                        new XElement("Flipped", overturnedTableCover.Id),
                        new XElement("NotFlipped", uprightTableCover.Id),
                        new XElement("Expression", new XCData("0")),
                        new XElement("Message", new XCData("@ try|tries to flip $1, but are|is not strong enough.")))));
        }

        AddExtraComponent("ClothingInsulation", "Insulation_Reflective_Strong",
            "Makes garment strongly reflective without adding much insulation.",
            new XElement("Definition",
                new XElement("InsulatingDegrees", 0.25),
                new XElement("ReflectingDegrees", 3.0)));
        AddExtraComponent("ClothingInsulation", "Insulation_Reflective_Extreme",
            "Makes garment extremely reflective for hot and bright environments.",
            new XElement("Definition",
                new XElement("InsulatingDegrees", 0.5),
                new XElement("ReflectingDegrees", 5.0)));
        AddExtraComponent("ClothingInsulation", "Insulation_Balanced_Warm",
            "Makes garment a warm and moderately reflective all-rounder.",
            new XElement("Definition",
                new XElement("InsulatingDegrees", 1.5),
                new XElement("ReflectingDegrees", 1.5)));
        AddExtraComponent("ClothingInsulation", "Insulation_Balanced_Heavy",
            "Makes garment heavily insulating but still somewhat reflective.",
            new XElement("Definition",
                new XElement("InsulatingDegrees", 3.0),
                new XElement("ReflectingDegrees", 1.25)));

        CharacteristicDefinition? eyeColour = context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == "Eye Colour");
        CharacteristicDefinition? hairColour = context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == "Hair Colour");
        CharacteristicDefinition? hairStyle = context.CharacteristicDefinitions.FirstOrDefault(x => x.Name == "Hair Style");
        CharacteristicProfile? allEyeColours = context.CharacteristicProfiles.FirstOrDefault(x => x.Name == "All Eye Colours");
        CharacteristicProfile? allHairColours = context.CharacteristicProfiles.FirstOrDefault(x => x.Name == "All Hair Colours");
        CharacteristicProfile? allHairStyles = context.CharacteristicProfiles.FirstOrDefault(x => x.Name == "All Hair Styles");
        WearProfile? hatWearProfile = context.WearProfiles.FirstOrDefault(x => x.Name == "Hat");
        WearProfile? glassesWearProfile = context.WearProfiles.FirstOrDefault(x => x.Name == "Glasses");

        if (hairColour is not null && hairStyle is not null && eyeColour is not null)
        {
            AddExtraComponent("IdentityObscurer", "CharacteristicMaskingObscurer",
                "Makes garment hide identity while also masking hair and eye characteristics.",
                new XElement("Definition",
                    new XElement("RemovalEcho", new XCData("revealing $haircolour $hairstyle hair and $eyecolour eyes.")),
                    new XElement("ShortDescription", new XCData("&a_an masked stranger")),
                    new XElement("FullDescription", new XCData("This individual is heavily disguised, leaving only a carefully controlled impression behind.")),
                    new XElement("Difficulty", (int)Difficulty.VeryHard),
                    new XElement("Keywords",
                        new XElement("Keyword", new XAttribute("key", "mask"), new XAttribute("value", "masked")),
                        new XElement("Keyword", new XAttribute("key", "hood"), new XAttribute("value", "hooded"))),
                    new XElement("Characteristics",
                        new XElement("Characteristic", new XAttribute("Definition", hairColour.Id), new XAttribute("Form", "dark")),
                        new XElement("Characteristic", new XAttribute("Definition", hairStyle.Id), new XAttribute("Form", "cropped")),
                        new XElement("Characteristic", new XAttribute("Definition", eyeColour.Id), new XAttribute("Form", "grey")))));
        }

        if (allHairColours is not null && allHairStyles is not null && hairColour is not null && hairStyle is not null)
        {
            AddExtraComponent("Variable Changer", "Wig_HatOnly",
                "Changes hair colour and style only when worn in a hat-like profile.",
                new XElement("Definition",
                    new XAttribute("TargetWearProfile", hatWearProfile?.Id.ToString() ?? string.Empty),
                    new XElement("Characteristic", new XAttribute("Profile", allHairColours.Id), new XAttribute("Value", hairColour.Id)),
                    new XElement("Characteristic", new XAttribute("Profile", allHairStyles.Id), new XAttribute("Value", hairStyle.Id))));
        }

        if (allEyeColours is not null && eyeColour is not null)
        {
            AddExtraComponent("Variable Changer", "ColouredContacts_GlassesProfile",
                "Changes eye colour only when worn in a glasses-style wear profile.",
                new XElement("Definition",
                    new XAttribute("TargetWearProfile", glassesWearProfile?.Id.ToString() ?? string.Empty),
                    new XElement("Characteristic", new XAttribute("Profile", allEyeColours.Id), new XAttribute("Value", eyeColour.Id))));
            AddExtraComponent("Obscurer", "Obscurer_Eyes",
                "Obscures a wearer's eye colour without changing their whole identity.",
                new XElement("Definition",
                    new XElement("RemovalEcho", new XCData("revealing $eyecolour eyes again.")),
                    new XElement("Characteristic", new XAttribute("Form", "shadowed"), new XAttribute("Definition", eyeColour.Id))));
        }

        TraitDefinition? bonusTrait = context.TraitDefinitions.FirstOrDefault(x => x.Name == "Medicine")
            ?? context.TraitDefinitions.FirstOrDefault(x => x.Name == "Search")
            ?? context.TraitDefinitions.FirstOrDefault(x => x.Name == "Stealth")
            ?? context.TraitDefinitions.FirstOrDefault();
        if (bonusTrait is not null)
        {
            AddExtraComponent("WornTraitChanger", "WornTraitChanger_Bonus",
                "Provides a small bonus to a common trait while worn.",
                new XElement("Definition",
                    new XElement("Modifier",
                        new XAttribute("trait", bonusTrait.Id),
                        new XAttribute("bonus", 1.5),
                        new XAttribute("context", (int)TraitBonusContext.None))));
            AddExtraComponent("WornTraitChanger", "WornTraitChanger_Penalty",
                "Provides a small penalty to a common trait while worn.",
                new XElement("Definition",
                    new XElement("Modifier",
                        new XAttribute("trait", bonusTrait.Id),
                        new XAttribute("bonus", -1.0),
                        new XAttribute("context", (int)TraitBonusContext.None))));
        }

        AddExtraComponent("Sheath", "Holster_Small",
            "Turns an item into a small firearm holster.",
            new XElement("Definition",
                new XAttribute("StealthDrawDifficulty", (int)Difficulty.Normal),
                new XAttribute("MaximumSize", (int)SizeCategory.Small),
                new XAttribute("DesignedForGuns", true)));
        AddExtraComponent("Sheath", "Holster_Large",
            "Turns an item into a large firearm holster.",
            new XElement("Definition",
                new XAttribute("StealthDrawDifficulty", (int)Difficulty.Hard),
                new XAttribute("MaximumSize", (int)SizeCategory.Normal),
                new XAttribute("DesignedForGuns", true)));

        AddExtraComponent("Restraint", "Restraint_ArmsOnly",
            "Turns an item into restraints intended for arms and appendages only.",
            new XElement("Definition",
                new XElement("MinimumCreatureSize", (int)SizeCategory.Small),
                new XElement("MaximumCreatureSize", (int)SizeCategory.Large),
                new XElement("BreakoutDifficulty", (int)Difficulty.Hard),
                new XElement("OverpowerDifficulty", (int)Difficulty.Hard),
                new XElement("LimbType", (int)LimbType.Arm),
                new XElement("LimbType", (int)LimbType.Appendage)));
        AddExtraComponent("Restraint", "Restraint_Hobbles",
            "Turns an item into leg restraints or hobbles.",
            new XElement("Definition",
                new XElement("MinimumCreatureSize", (int)SizeCategory.Small),
                new XElement("MaximumCreatureSize", (int)SizeCategory.VeryLarge),
                new XElement("BreakoutDifficulty", (int)Difficulty.Normal),
                new XElement("OverpowerDifficulty", (int)Difficulty.Hard),
                new XElement("LimbType", (int)LimbType.Leg)));
        AddExtraComponent("Restraint", "Restraint_Oversized",
            "Turns an item into oversized restraints for large creatures.",
            new XElement("Definition",
                new XElement("MinimumCreatureSize", (int)SizeCategory.Large),
                new XElement("MaximumCreatureSize", (int)SizeCategory.Gigantic),
                new XElement("BreakoutDifficulty", (int)Difficulty.Hard),
                new XElement("OverpowerDifficulty", (int)Difficulty.VeryHard),
                new XElement("LimbType", (int)LimbType.Arm),
                new XElement("LimbType", (int)LimbType.Leg),
                new XElement("LimbType", (int)LimbType.Appendage)));

        AddExtraComponent("DragAid", "DragAid_Sling",
            "Turns an item into a simple dragging sling.",
            new XElement("Definition",
                new XElement("MaximumUsers", 2),
                new XElement("EffortMultiplier", 1.5)));
        AddExtraComponent("DragAid", "DragAid_Harness",
            "Turns an item into a heavy dragging harness for team work.",
            new XElement("Definition",
                new XElement("MaximumUsers", 4),
                new XElement("EffortMultiplier", 2.5)));
        AddExtraComponent("DragAid", "DragAid_Stretcher",
            "Turns an item into a stretcher or litter for coordinated casualty dragging.",
            new XElement("Definition",
                new XElement("MaximumUsers", 4),
                new XElement("EffortMultiplier", 3.0)));
        AddExtraComponent("DragAid", "DragAid_Sled",
            "Turns an item into a hauling sled or skid.",
            new XElement("Definition",
                new XElement("MaximumUsers", 2),
                new XElement("EffortMultiplier", 2.0)));
        AddExtraComponent("DragAid", "DragAid_Travois",
            "Turns an item into a travois-style drag aid for rough travel.",
            new XElement("Definition",
                new XElement("MaximumUsers", 2),
                new XElement("EffortMultiplier", 2.25)));
        AddExtraComponent("DragAid", "DragAid_Antiquity_FieldStretcher",
            "Turns an item into an antiquity field stretcher for casualty movement.",
            new XElement("Definition",
                new XElement("MaximumUsers", 4),
                new XElement("EffortMultiplier", 3.25)));
        AddExtraComponent("DragAid", "DragAid_Antiquity_CorpseBier",
            "Turns an item into a funerary bier or corpse litter for team movement.",
            new XElement("Definition",
                new XElement("MaximumUsers", 6),
                new XElement("EffortMultiplier", 3.0)));
        AddExtraComponent("DragAid", "DragAid_Antiquity_CargoSled",
            "Turns an item into a low cargo sled for heavy antiquity hauling.",
            new XElement("Definition",
                new XElement("MaximumUsers", 4),
                new XElement("EffortMultiplier", 2.75)));
        AddExtraComponent("DragAid", "DragAid_Antiquity_PackTravois",
            "Turns an item into a pack travois for pastoral or frontier cargo support.",
            new XElement("Definition",
                new XElement("MaximumUsers", 2),
                new XElement("EffortMultiplier", 2.4)));
        AddExtraComponent("DragAid", "DragAid_Antiquity_CarryingSling",
            "Turns an item into a one-person carrying sling for smaller loads.",
            new XElement("Definition",
                new XElement("MaximumUsers", 1),
                new XElement("EffortMultiplier", 1.6)));

        AddExtraComponent("RidingGear", "RidingGear_Saddle",
            "Marks an item as a saddle that improves rider stability.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.Saddle),
                new XElement("ControlBonus", 0.0),
                new XElement("StabilityBonus", 5.0)));
        AddExtraComponent("RidingGear", "RidingGear_SaddlePad",
            "Marks an item as a saddle pad that removes the no-pad riding penalty.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.SaddlePad),
                new XElement("ControlBonus", 0.0),
                new XElement("StabilityBonus", 2.0)));
        AddExtraComponent("RidingGear", "RidingGear_Bridle",
            "Marks an item as a bridle that improves mounted control.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.Bridle),
                new XElement("ControlBonus", 5.0),
                new XElement("StabilityBonus", 0.0)));
        AddExtraComponent("RidingGear", "RidingGear_Reins",
            "Marks an item as reins or equivalent hand controls for a rider.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.Reins),
                new XElement("ControlBonus", 5.0),
                new XElement("StabilityBonus", 0.0)));
        AddExtraComponent("RidingGear", "RidingGear_Bit",
            "Marks an item as a bit that completes bitted bridle control.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.Bit),
                new XElement("ControlBonus", 5.0),
                new XElement("StabilityBonus", 0.0)));
        AddExtraComponent("RidingGear", "RidingGear_Stirrups",
            "Marks an item as stirrups that improve stay-mounted checks.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.Stirrups),
                new XElement("ControlBonus", 0.0),
                new XElement("StabilityBonus", 5.0)));
        AddExtraComponent("RidingGear", "RidingGear_PackSaddle",
            "Marks an item as a pack saddle that counts as a saddle for bareback penalties.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.PackSaddle),
                new XElement("ControlBonus", -2.0),
                new XElement("StabilityBonus", 2.0)));
        AddExtraComponent("RidingGear", "RidingGear_BitlessBridle",
            "Marks an item as bitless riding control gear that should not take the missing-bit penalty.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.Bridle | RidingGearRole.Reins | RidingGearRole.BitlessControl),
                new XElement("ControlBonus", 7.0),
                new XElement("StabilityBonus", 0.0)));
        AddExtraComponent("RidingGear", "RidingGear_RidingHarness",
            "Marks an item as harness-style riding control gear.",
            new XElement("Definition",
                new XElement("Roles", RidingGearRole.Harness | RidingGearRole.Bridle | RidingGearRole.Reins | RidingGearRole.BitlessControl),
                new XElement("ControlBonus", 5.0),
                new XElement("StabilityBonus", 2.0)));

        AddExtraComponent("HitchGear", "HitchGear_LeadRope",
            "Marks an item as a lead rope or light hitching rope.",
            new XElement("Definition",
                new XElement("Roles", HitchGearRole.LeadRope | HitchGearRole.Rope),
                new XElement("MaximumUsers", 1),
                new XElement("EffortMultiplier", 1.25),
                new XElement("MaximumTowedWeight", 0.0)));
        AddExtraComponent("HitchGear", "HitchGear_Yoke",
            "Marks an item as a yoke for hitching beasts to a tow point.",
            new XElement("Definition",
                new XElement("Roles", HitchGearRole.Yoke),
                new XElement("MaximumUsers", 2),
                new XElement("EffortMultiplier", 2.5),
                new XElement("MaximumTowedWeight", 0.0)));
        AddExtraComponent("HitchGear", "HitchGear_Harness",
            "Marks an item as a draft harness for hitching creatures to vehicles.",
            new XElement("Definition",
                new XElement("Roles", HitchGearRole.Harness | HitchGearRole.Traces),
                new XElement("MaximumUsers", 1),
                new XElement("EffortMultiplier", 2.0),
                new XElement("MaximumTowedWeight", 0.0)));
        AddExtraComponent("HitchGear", "HitchGear_Chain",
            "Marks an item as a chain suitable for chain-type hitch points.",
            new XElement("Definition",
                new XElement("Roles", HitchGearRole.Chain),
                new XElement("MaximumUsers", 2),
                new XElement("EffortMultiplier", 2.0),
                new XElement("MaximumTowedWeight", 0.0)));
        AddExtraComponent("HitchGear", "HitchGear_Rope",
            "Marks an item as a general rope suitable for rope-type hitch points.",
            new XElement("Definition",
                new XElement("Roles", HitchGearRole.Rope),
                new XElement("MaximumUsers", 2),
                new XElement("EffortMultiplier", 1.5),
                new XElement("MaximumTowedWeight", 0.0)));
        AddExtraComponent("HitchGear", "HitchGear_Traces",
            "Marks an item as traces used with a harness or yoke.",
            new XElement("Definition",
                new XElement("Roles", HitchGearRole.Traces),
                new XElement("MaximumUsers", 1),
                new XElement("EffortMultiplier", 1.5),
                new XElement("MaximumTowedWeight", 0.0)));
        AddExtraComponent("HitchGear", "HitchGear_TowBar",
            "Marks an item as a tow bar for vehicle-to-vehicle hitching.",
            new XElement("Definition",
                new XElement("Roles", HitchGearRole.TowBar),
                new XElement("MaximumUsers", 1),
                new XElement("EffortMultiplier", 1.0),
                new XElement("MaximumTowedWeight", 0.0)));

        AddExtraComponent("WaterSource", "WaterSource_Canteen",
            "Turns an item into a transparent closable refill-on-toggle canteen.",
            new XElement("Definition",
                new XAttribute("LiquidCapacity", 1.0),
                new XAttribute("Closable", true),
                new XAttribute("Transparent", true),
                new XAttribute("OnceOnly", false),
                new XAttribute("DefaultLiquid", waterLiquid.Id),
                new XAttribute("RefillRate", 0.25),
                new XAttribute("UseOnOffForRefill", true),
                new XAttribute("RefillingProg", 0),
                new XAttribute("CanBeEmptiedWhenInRoom", true)));
        AddExtraComponent("WaterSource", "WaterSource_DisposableBottle",
            "Turns an item into a single-use disposable water source.",
            new XElement("Definition",
                new XAttribute("LiquidCapacity", 0.6),
                new XAttribute("Closable", true),
                new XAttribute("Transparent", true),
                new XAttribute("OnceOnly", true),
                new XAttribute("DefaultLiquid", waterLiquid.Id),
                new XAttribute("RefillRate", 0.0),
                new XAttribute("UseOnOffForRefill", false),
                new XAttribute("RefillingProg", 0),
                new XAttribute("CanBeEmptiedWhenInRoom", false)));
        FutureProg? alwaysTrueProg = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "AlwaysTrue");
        if (alwaysTrueProg is not null)
        {
            AddExtraComponent("WaterSource", "WaterSource_ProgControlled",
                "Turns an item into a self-refilling water source controlled by a prog.",
                new XElement("Definition",
                    new XAttribute("LiquidCapacity", 10.0),
                    new XAttribute("Closable", false),
                    new XAttribute("Transparent", false),
                    new XAttribute("OnceOnly", false),
                    new XAttribute("DefaultLiquid", waterLiquid.Id),
                    new XAttribute("RefillRate", 1.0),
                    new XAttribute("UseOnOffForRefill", false),
                    new XAttribute("RefillingProg", alwaysTrueProg.Id),
                    new XAttribute("CanBeEmptiedWhenInRoom", true)));
        }

        AddExtraComponent("Destroyable", "Destroyable_Glassware",
            "Turns an item into a fragile glass object.",
            new XElement("Definition",
                new XElement("HpExpression", new XCData("4 * quality")),
                new XElement("DamageMultipliers",
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Crushing), new XAttribute("multiplier", 1.6)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Ballistic), new XAttribute("multiplier", 1.2)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 0.1)))));
        AddExtraComponent("Destroyable", "Destroyable_Paper",
            "Turns an item into a fragile paper or parchment object.",
            new XElement("Definition",
                new XElement("HpExpression", new XCData("2 * quality")),
                new XElement("DamageMultipliers",
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 2.5)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Piercing), new XAttribute("multiplier", 1.1)))));
        AddExtraComponent("Destroyable", "Destroyable_Shield",
            "Turns an item into a durable shield.",
            new XElement("Definition",
                new XElement("HpExpression", new XCData("12 * quality")),
                new XElement("DamageMultipliers",
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Chopping), new XAttribute("multiplier", 1.15)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Crushing), new XAttribute("multiplier", 0.9)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Piercing), new XAttribute("multiplier", 0.8)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Ballistic), new XAttribute("multiplier", 0.75)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 0.5)))));
        AddExtraComponent("Destroyable", "Destroyable_WoodenHeavy",
            "Turns an item into a sturdy wooden object.",
            new XElement("Definition",
                new XElement("HpExpression", new XCData("15 * quality")),
                new XElement("DamageMultipliers",
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Chopping), new XAttribute("multiplier", 1.35)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 1.75)))));
        AddExtraComponent("Destroyable", "Destroyable_HeavyMetal",
            "Turns an item into a heavy metal object.",
            new XElement("Definition",
                new XElement("HpExpression", new XCData("30 * quality")),
                new XElement("DamageMultipliers",
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Crushing), new XAttribute("multiplier", 0.65)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 0.15)))));

        AddExtraComponent("Treatment", "FieldMedkit",
            "Turns the item into a multi-purpose field medkit.",
            new XElement("Definition",
                new XElement("MaximumUses", 25),
                new XElement("Refillable", true),
                new XElement("DifficultyStages", 2),
                new XElement("TreatmentType", 1),
                new XElement("TreatmentType", 2),
                new XElement("TreatmentType", 3),
                new XElement("TreatmentType", 4),
                new XElement("TreatmentType", 11)));
        AddExtraComponent("Treatment", "Bandage_Poor",
            "Turns the item into a poor quality bandage example.",
            new XElement("Definition",
                new XElement("MaximumUses", 1),
                new XElement("Refillable", false),
                new XElement("DifficultyStages", -1),
                new XElement("TreatmentType", 2)));
        AddExtraComponent("Treatment", "Treatment_AdminUnlimited",
            "Turns the item into an unlimited all-purpose treatment kit.",
            new XElement("Definition",
                new XElement("MaximumUses", -1),
                new XElement("Refillable", false),
                new XElement("DifficultyStages", 5),
                new XElement("TreatmentType", 1),
                new XElement("TreatmentType", 2),
                new XElement("TreatmentType", 3),
                new XElement("TreatmentType", 4),
                new XElement("TreatmentType", 11)));
        AddExtraComponent("Treatment", "Treatment_AntiInflammatory_Single",
            "Turns the item into a single-use anti-inflammatory treatment example.",
            new XElement("Definition",
                new XElement("MaximumUses", 1),
                new XElement("Refillable", false),
                new XElement("DifficultyStages", 0),
                new XElement("TreatmentType", (int)TreatmentType.AntiInflammatory)));
        AddExtraComponent("Treatment", "Treatment_AntiInflammatory_Kit",
            "Turns the item into a refillable anti-inflammatory treatment kit.",
            new XElement("Definition",
                new XElement("MaximumUses", 12),
                new XElement("Refillable", true),
                new XElement("DifficultyStages", 2),
                new XElement("TreatmentType", (int)TreatmentType.AntiInflammatory)));

        BodyProto? prostheticBody = context.BodyProtos.FirstOrDefault(x => x.Name == "Organic Humanoid") ??
                            context.BodyProtos.FirstOrDefault(x => x.Name == "Humanoid");
        Race? humanRace = context.Races.FirstOrDefault(x => x.Name == "Human") ?? context.Races.FirstOrDefault();
        if (prostheticBody is not null)
        {
            BodypartProto? leftHand = context.BodypartProtos.FirstOrDefault(x => x.Name == "lhand");
            BodypartProto? rightFoot = context.BodypartProtos.FirstOrDefault(x => x.Name == "rfoot");
            if (leftHand is not null)
            {
                AddExtraComponent("Prosthetic", "Prosthetic_LHand_Functional",
                    "Turns the item into a functional prosthetic left hand.",
                    new XElement("Definition",
                        new XElement("Obvious", false),
                        new XElement("Functional", true),
                        new XElement("TargetBody", prostheticBody.Id),
                        new XElement("TargetBodypart", leftHand.Id),
                        new XElement("Gender", (int)Gender.Indeterminate),
                        new XElement("Race", humanRace?.Id ?? 0L)));
            }

            if (rightFoot is not null)
            {
                AddExtraComponent("Prosthetic", "Prosthetic_RFoot_Ornamental",
                    "Turns the item into an obvious ornamental prosthetic right foot.",
                    new XElement("Definition",
                        new XElement("Obvious", true),
                        new XElement("Functional", false),
                        new XElement("TargetBody", prostheticBody.Id),
                        new XElement("TargetBodypart", rightFoot.Id),
                        new XElement("Gender", (int)Gender.Female),
                        new XElement("Race", humanRace?.Id ?? 0L)));
            }
        }

        Clock? primaryClock = context.Clocks.FirstOrDefault();
        Timezone? defaultTimeZone = primaryClock is null
            ? null
            : context.Timezones.FirstOrDefault(x => x.Id == primaryClock.PrimaryTimezoneId) ??
              context.Timezones.FirstOrDefault(x => x.ClockId == primaryClock.Id);
        if (primaryClock is not null && defaultTimeZone is not null)
        {
            AddExtraComponent("TimePiece", "TimePiece_Standard",
                "Turns an item into a standard modern timepiece.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", false),
                    new XElement("TimeDisplayString", "$j:$m $i")));
            AddExtraComponent("TimePiece", "TimePiece_Adjustable",
                "Turns an item into an adjustable builder-facing timepiece.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", true),
                    new XElement("TimeDisplayString", "$h:$m:$s $t")));
            AddExtraComponent("TimePiece", "TimePiece_PocketWatch",
                "Turns an item into a pocket watch style timepiece.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", false),
                    new XElement("TimeDisplayString", "$j:$m $i")));
            AddExtraComponent("TimePiece", "TimePiece_WallClock",
                "Turns an item into a fixed wall clock style timepiece.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", false),
                    new XElement("TimeDisplayString", "$h:$m")));
            AddExtraComponent("TimePiece", "TimePiece_Antiquity_Sundial",
                "Turns an item into a non-settable sundial-style timepiece with crude daylight time display.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", false),
                    new XElement("TimeDisplayString", "$c")));
            AddExtraComponent("TimePiece", "TimePiece_Antiquity_WaterClock",
                "Turns an item into a non-settable water-clock style timepiece for temples, courts or watch posts.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", false),
                    new XElement("TimeDisplayString", "$j:$m $i")));
            AddExtraComponent("TimePiece", "TimePiece_Antiquity_MarkedCandle",
                "Turns an item into a coarse marked-candle timepiece.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", false),
                    new XElement("TimeDisplayString", "$c")));
            AddExtraComponent("TimePiece", "TimePiece_Antiquity_WatchBoard",
                "Turns an item into a public watch-board timepiece tied to the default clock and timezone.",
                new XElement("Definition",
                    new XElement("Clock", primaryClock.Id),
                    new XElement("TimeZone", defaultTimeZone.Id),
                    new XElement("PlayersCanSetTime", false),
                    new XElement("TimeDisplayString", "$j $i")));
        }

        AddExtraComponent("ShopStall", "ShopStall_Antiquity_OpenCounter",
            "Turns an item into an open antiquity market counter or stall surface.",
            ShopStallDefinition(200000, SizeCategory.Large, true, Difficulty.Normal, Difficulty.Normal, "Warded Lock"));
        AddExtraComponent("ShopStall", "ShopStall_Antiquity_LockableCounter",
            "Turns an item into a lockable antiquity market counter for unattended goods.",
            ShopStallDefinition(250000, SizeCategory.Large, false, Difficulty.VeryHard, Difficulty.Hard, "Warded Lock"));
        AddExtraComponent("ShopStall", "ShopStall_Antiquity_PortableBooth",
            "Turns an item into a lighter portable stall for fairs, camps and travelling merchants.",
            ShopStallDefinition(100000, SizeCategory.Normal, true, Difficulty.Hard, Difficulty.Normal, "Warded Lock"));

        AddExtraComponent("MarketGoodWeight", "MarketGoodWeight_Antiquity_StapleFood",
            "Turns an item into an antiquity market-good weight for staples such as grain, beer and wine.",
            MarketGoodWeightDefinition(
                ("Staple Food", 1.25m),
                ("Standard Food", 1.05m),
                ("Beer", 1.10m),
                ("Wine", 1.10m)));
        AddExtraComponent("MarketGoodWeight", "MarketGoodWeight_Antiquity_LuxuryCraft",
            "Turns an item into an antiquity market-good weight for luxury crafts and elite household wares.",
            MarketGoodWeightDefinition(
                ("Luxury Wares", 1.35m),
                ("Luxury Clothing", 1.25m),
                ("Luxury Furniture", 1.20m),
                ("Luxury Decorations", 1.25m)));
        AddExtraComponent("MarketGoodWeight", "MarketGoodWeight_Antiquity_MilitarySupply",
            "Turns an item into an antiquity market-good weight for military supply goods.",
            MarketGoodWeightDefinition(
                ("Military Goods", 1.20m),
                ("Weapons", 1.25m),
                ("Armour", 1.25m),
                ("Ammunition", 1.15m),
                ("Shields", 1.15m)));

        nextId = currentId;
        context.SaveChanges();
        #endregion
    }

    private void SeedSealAndMeasurementComponents(FuturemudDatabaseContext context, DateTime now, Account dbaccount,
        ref long nextId)
    {
        #region Seals and Measurement

        long currentId = nextId;

        GameItemComponentProto UpsertStockComponent(string type, string name, string description, XElement definition)
        {
            return UpsertComponent(context, ref currentId, dbaccount, now, type, name, description, definition.ToString());
        }

        GameItemComponentProto EnsureComponent(string type, string name, string description, XElement definition)
        {
            GameItemComponentProto? existing = context.GameItemComponentProtos.Local
                                         .FirstOrDefault(x => x.Name == name && x.EditableItem.RevisionStatus == 4) ??
                                     context.GameItemComponentProtos
                                         .FirstOrDefault(x => x.Name == name && x.EditableItem.RevisionStatus == 4);
            return existing ?? UpsertStockComponent(type, name, description, definition);
        }

        XElement SealStampDefinition(string design, string issuer, string owner, string clan, string office,
            string material, Difficulty forgeryDifficulty)
        {
            return new XElement("Definition",
                new XElement("SealDesign", new XCData(design)),
                new XElement("IssuerText", new XCData(issuer)),
                new XElement("OwnerText", new XCData(owner)),
                new XElement("ClanText", new XCData(clan)),
                new XElement("OfficeText", new XCData(office)),
                new XElement("StampMaterial", new XCData(material)),
                new XElement("ForgeryDifficulty", (int)forgeryDifficulty),
                new XElement("AuthorityProg", 0));
        }

        XElement SealableDefinition(Difficulty inspectionDifficulty, bool brokenSealLeavesResidue,
            params string[] allowedMedia)
        {
            return new XElement("Definition",
                new XElement("AllowedMedia",
                    from medium in allowedMedia
                    select new XElement("Medium", new XCData(medium))),
                new XElement("InspectionDifficulty", (int)inspectionDifficulty),
                new XElement("BrokenSealLeavesResidue", brokenSealLeavesResidue));
        }

        XElement PaperSheetDefinition(int maxCharacters)
        {
            return new XElement("Definition",
                new XElement("MaximumCharacterLengthOfText", maxCharacters));
        }

        XElement InscribableSurfaceDefinition(int maxCharacters, params WritingImplementType[] allowedImplements)
        {
            return new XElement("Definition",
                new XElement("MaximumCharacterLengthOfText", maxCharacters),
                new XElement("AllowedImplementTypes",
                    allowedImplements
                        .Distinct()
                        .Select(x => new XElement("Type", x.ToString()))));
        }

        XElement ScribingImplementDefinition(WritingImplementType implementType, string colourName, int totalUses)
        {
            var colour = context.Colours.Local
                                .FirstOrDefault(x => x.Name.Equals(colourName, StringComparison.OrdinalIgnoreCase)) ??
                         context.Colours
                                .AsEnumerable()
                                .FirstOrDefault(x => x.Name.Equals(colourName, StringComparison.OrdinalIgnoreCase));
            return new XElement("Definition",
                new XElement("ImplementType", implementType.ToString()),
                new XElement("Colour", colour?.Id ?? 0),
                new XElement("ColourCharacteristic", 0),
                new XElement("TotalUses", totalUses));
        }

        XElement BookDefinition(GameItemProto pageItem, int pages, string defaultTitle = "")
        {
            return new XElement("Definition",
                new XElement("PaperProto", pageItem.Id),
                new XElement("PageCount", pages),
                new XElement("DefaultTitle", new XCData(defaultTitle)),
                new XElement("InitialReadables"));
        }

        XElement ContainerDefinition(double weight, SizeCategory maxSize, bool closable, bool transparent,
            string preposition, bool onceOnly = false)
        {
            XElement definition = new("Definition",
                new XAttribute("Weight", weight),
                new XAttribute("MaxSize", (int)maxSize),
                new XAttribute("Preposition", preposition),
                new XAttribute("Closable", closable),
                new XAttribute("Transparent", transparent));
            if (onceOnly)
            {
                definition.Add(new XAttribute("OnceOnly", true));
            }

            return definition;
        }

        XElement MeasuringDefinition(string mode, double precision, double capacity, double baseDriftPerUse,
            double maximumDrift, double maximumWrongCalibration, Difficulty inspectionDifficulty)
        {
            return new XElement("Definition",
                new XElement("Mode", mode),
                new XElement("Precision", precision),
                new XElement("Capacity", capacity),
                new XElement("BaseDriftPerUse", baseDriftPerUse),
                new XElement("MaximumDrift", maximumDrift),
                new XElement("MaximumWrongCalibration", maximumWrongCalibration),
                new XElement("CalibrationInspectionDifficulty", (int)inspectionDifficulty));
        }

        Material EnsureMaterial(string name, MaterialBehaviourType behaviourType)
        {
            Material? material = context.Materials.Local.FirstOrDefault(x => x.Name.EqualTo(name)) ??
                                 context.Materials.FirstOrDefault(x => x.Name == name);
            if (material is not null)
            {
                return material;
            }

            material = new Material
            {
                Id = Math.Max(
                    context.Materials.Any() ? context.Materials.Max(x => x.Id) : 0L,
                    context.Materials.Local.Any() ? context.Materials.Local.Max(x => x.Id) : 0L) + 1,
                Name = name,
                MaterialDescription = name.ToLowerInvariant(),
                BehaviourType = (int)behaviourType,
                ResidueSdesc = string.Empty,
                ResidueDesc = string.Empty,
                ResidueColour = string.Empty
            };
            context.Materials.Add(material);
            return material;
        }

        long nextItemProtoId = context.GameItemProtos.Any() ? context.GameItemProtos.Max(x => x.Id) + 1 : 1;

        void EnsureComponentLink(GameItemProto item, GameItemComponentProto component)
        {
            if (item.GameItemProtosGameItemComponentProtos.Any(x =>
                    x.GameItemComponentProtoId == component.Id || x.GameItemComponent == component))
            {
                return;
            }

            item.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
            {
                GameItemProto = item,
                GameItemProtoId = item.Id,
                GameItemProtoRevision = item.RevisionNumber,
                GameItemComponent = component,
                GameItemComponentProtoId = component.Id,
                GameItemComponentRevision = component.RevisionNumber
            });
        }

        GameItemProto EnsureItemPrototype(string name, string keywords, Material material, SizeCategory size, double weight,
            string shortDescription, string fullDescription, params GameItemComponentProto[] components)
        {
            GameItemProto? item = context.GameItemProtos.Local
                                      .FirstOrDefault(x => x.ShortDescription == shortDescription) ??
                                  context.GameItemProtos
                                      .Include(x => x.GameItemProtosGameItemComponentProtos)
                                      .FirstOrDefault(x => x.ShortDescription == shortDescription);
            if (item is null)
            {
                item = new GameItemProto
                {
                    Id = nextItemProtoId++,
                    RevisionNumber = 0,
                    Name = name,
                    UniqueName = shortDescription,
                    BuilderNotes = "Auto-generated by the system",
                    Keywords = keywords,
                    MaterialId = material.Id,
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
                    },
                    Size = (int)size,
                    Weight = weight,
                    ReadOnly = false,
                    LongDescription = string.Empty,
                    BaseItemQuality = (int)ItemQuality.Standard,
                    CustomColour = string.Empty,
                    HighPriority = false,
                    MorphTimeSeconds = 0,
                    MorphEmote = string.Empty,
                    ShortDescription = shortDescription,
                    FullDescription = fullDescription,
                    PermitPlayerSkins = false,
                    CostInBaseCurrency = 0.0M,
                    IsHiddenFromPlayers = false,
                    PreserveRegisterVariables = false,
                    PlanarData = string.Empty
                };
                context.GameItemProtos.Add(item);
            }
            else
            {
                item.Name = name;
                item.UniqueName = shortDescription;
                item.Keywords = keywords;
                item.MaterialId = material.Id;
                item.Size = (int)size;
                item.Weight = weight;
                item.ShortDescription = shortDescription;
                item.FullDescription = fullDescription;
            }

            foreach (GameItemComponentProto component in components)
            {
                EnsureComponentLink(item, component);
            }

            return item;
        }

        GameItemComponentProto medievalParchmentSheetSurface = UpsertStockComponent("PaperSheet", "Medieval_Parchment_Sheet_Surface",
            "Turns a single parchment sheet into a writable medieval document surface.",
            PaperSheetDefinition(3600));
        _ = UpsertStockComponent("PaperSheet", "Medieval_Parchment_Bifolium_Surface",
            "Turns a folded parchment bifolium into a writable medieval document surface.",
            PaperSheetDefinition(7200));
        _ = UpsertStockComponent("PaperSheet", "Medieval_Parchment_Roll_Surface",
            "Turns a parchment roll into a long writable medieval document surface.",
            PaperSheetDefinition(15000));
        GameItemComponentProto medievalRagPaperSheetSurface = UpsertStockComponent("PaperSheet", "Medieval_Rag_Paper_Sheet_Surface",
            "Turns a rag-paper sheet into a writable medieval document surface.",
            PaperSheetDefinition(3600));
        _ = UpsertStockComponent("PaperSheet", "Medieval_Rag_Paper_Letter_Surface",
            "Turns a folded rag-paper letter into a compact writable medieval document surface.",
            PaperSheetDefinition(2400));
        _ = UpsertStockComponent("PaperSheet", "Medieval_Rag_Paper_Scroll_Surface",
            "Turns a rag-paper scroll into a long writable medieval document surface.",
            PaperSheetDefinition(12000));
        _ = UpsertStockComponent("PaperSheet", "Medieval_Papyrus_Sheet_Surface",
            "Turns a papyrus sheet into a writable pre-modern document surface.",
            PaperSheetDefinition(2400));
        _ = UpsertStockComponent("PaperSheet", "Medieval_Papyrus_Scroll_Surface",
            "Turns a papyrus scroll into a long writable pre-modern document surface.",
            PaperSheetDefinition(12000));
        _ = UpsertStockComponent("PaperSheet", "Medieval_East_Asian_Paper_Scroll_Surface",
            "Turns an East Asian paper scroll into a long writable medieval document surface.",
            PaperSheetDefinition(12000));
        GameItemComponentProto medievalEastAsianPaperSheetSurface = UpsertStockComponent("PaperSheet", "Medieval_East_Asian_Paper_Sheet_Surface",
            "Turns an East Asian paper sheet into a writable medieval document surface.",
            PaperSheetDefinition(3600));
        GameItemComponentProto medievalPalmLeafSurface = UpsertStockComponent("PaperSheet", "Medieval_Palm_Leaf_Manuscript_Surface",
            "Turns a palm-leaf manuscript strip into a writable pre-modern manuscript surface.",
            PaperSheetDefinition(1800));

        _ = UpsertStockComponent("InscribableSurface", "Medieval_Wax_Tablet_Surface",
            "Allows a wax writing tablet to take stylus marks.",
            InscribableSurfaceDefinition(1800, WritingImplementType.Stylus));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Wax_Diptych_Surface",
            "Allows paired wax tablets to take stylus marks.",
            InscribableSurfaceDefinition(3600, WritingImplementType.Stylus));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Wax_Triptych_Surface",
            "Allows tripled wax tablets to take stylus marks.",
            InscribableSurfaceDefinition(5400, WritingImplementType.Stylus));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Wooden_Tablet_Surface",
            "Allows a wooden writing tablet to take incised, stylus or charcoal marks.",
            InscribableSurfaceDefinition(2200, WritingImplementType.Stylus, WritingImplementType.Chisel,
                WritingImplementType.Charcoal));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Slate_Tablet_Surface",
            "Allows a slate writing tablet to take stylus or chisel marks.",
            InscribableSurfaceDefinition(1800, WritingImplementType.Stylus, WritingImplementType.Chisel));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Birch_Bark_Surface",
            "Allows birch-bark documents to take ink, brush, charcoal or incised marks.",
            InscribableSurfaceDefinition(1800, WritingImplementType.Quill, WritingImplementType.ReedPen,
                WritingImplementType.Brush, WritingImplementType.Charcoal, WritingImplementType.Stylus));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Bamboo_Slip_Surface",
            "Allows a bamboo slip to take brush or stylus writing.",
            InscribableSurfaceDefinition(800, WritingImplementType.Brush, WritingImplementType.Stylus));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Ostracon_Surface",
            "Allows an ostracon to take ink, brush or charcoal marks.",
            InscribableSurfaceDefinition(900, WritingImplementType.ReedPen, WritingImplementType.Quill,
                WritingImplementType.Brush, WritingImplementType.Charcoal));
        _ = UpsertStockComponent("InscribableSurface", "Medieval_Practice_Board_Surface",
            "Allows a reusable practice board to take common pre-modern writing and incising implements.",
            InscribableSurfaceDefinition(2500, WritingImplementType.Stylus, WritingImplementType.Quill,
                WritingImplementType.ReedPen, WritingImplementType.Brush, WritingImplementType.Charcoal,
                WritingImplementType.Chisel));

        _ = UpsertStockComponent("ScribingImplement", "Medieval_Quill_Pen",
            "Turns an item into a finite black quill writing implement.",
            ScribingImplementDefinition(WritingImplementType.Quill, "black", 9000));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Fine_Quill_Pen",
            "Turns an item into a fine finite black quill writing implement.",
            ScribingImplementDefinition(WritingImplementType.Quill, "black", 12000));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Reed_Pen",
            "Turns an item into a finite black reed writing implement.",
            ScribingImplementDefinition(WritingImplementType.ReedPen, "black", 7000));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Qalam",
            "Turns an item into a finite black qalam-style reed writing implement.",
            ScribingImplementDefinition(WritingImplementType.ReedPen, "black", 8000));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Calligraphy_Brush",
            "Turns an item into a finite black calligraphy brush writing implement.",
            ScribingImplementDefinition(WritingImplementType.Brush, "black", 6000));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_East_Asian_Writing_Brush",
            "Turns an item into a finite black East Asian writing brush.",
            ScribingImplementDefinition(WritingImplementType.Brush, "black", 7000));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Charcoal_Stick",
            "Turns an item into a finite charcoal writing stick.",
            ScribingImplementDefinition(WritingImplementType.Charcoal, "black", 2500));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Bone_Stylus",
            "Turns an item into a non-consuming bone stylus.",
            ScribingImplementDefinition(WritingImplementType.Stylus, "black", 0));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Bronze_Stylus",
            "Turns an item into a non-consuming bronze stylus.",
            ScribingImplementDefinition(WritingImplementType.Stylus, "black", 0));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Iron_Stylus",
            "Turns an item into a non-consuming iron stylus.",
            ScribingImplementDefinition(WritingImplementType.Stylus, "black", 0));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Reed_Stylus",
            "Turns an item into a non-consuming reed stylus.",
            ScribingImplementDefinition(WritingImplementType.Stylus, "black", 0));
        _ = UpsertStockComponent("ScribingImplement", "Medieval_Scribing_Chisel",
            "Turns an item into a non-consuming scribing chisel.",
            ScribingImplementDefinition(WritingImplementType.Chisel, "black", 0));

        GameItemComponentProto bronzeSignet = UpsertStockComponent("SealStamp", "SealStamp_Antiquity_BronzeSignet",
            "Turns an item into a bronze signet ring style seal stamp.",
            SealStampDefinition("a bronze signet showing a lion beneath a civic star", "Civic Magistracy",
                string.Empty, string.Empty, "Seal-Bearer", "bronze", Difficulty.VeryHard));
        GameItemComponentProto cylinderSeal = UpsertStockComponent("SealStamp", "SealStamp_Antiquity_CylinderSeal",
            "Turns an item into a cylinder seal for rolling an authority impression through clay or wax.",
            SealStampDefinition("a rolled procession of officials, reeds and account marks", "Temple Archive",
                string.Empty, "Temple Household", "Archive Steward", "stone", Difficulty.ExtremelyHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_BronzeSignet",
            "Turns an item into a medieval bronze signet seal stamp.",
            SealStampDefinition("a bronze signet bearing a personal device cut for wax", string.Empty,
                "Signet Owner", string.Empty, string.Empty, "bronze", Difficulty.VeryHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_RingSignet",
            "Turns a wearable ring into a medieval signet seal stamp.",
            SealStampDefinition("a ring signet bearing a simple personal device for wax", string.Empty,
                "Signet Ring Owner", string.Empty, string.Empty, "silver", Difficulty.VeryHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_PersonalSignetRing",
            "Turns a wearable ring into a personal medieval signet seal stamp.",
            SealStampDefinition("a personal signet ring cut with initials and a household device", string.Empty,
                "Personal Signet Owner", string.Empty, string.Empty, "silver-gilt", Difficulty.VeryHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_MerchantSignetRing",
            "Turns a wearable ring into a merchant medieval signet seal stamp.",
            SealStampDefinition("a merchant signet ring showing scales, coin and a trade mark", "Merchant House",
                "Merchant Signet Owner", "Merchant House", "Factor", "bronze", Difficulty.ExtremelyHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_NobleSignetRing",
            "Turns a wearable ring into a noble medieval signet seal stamp.",
            SealStampDefinition("a noble signet ring bearing a heraldic device and motto", string.Empty,
                "Noble Signet Owner", "Noble Household", "Seal-Bearer", "gold", Difficulty.ExtremelyHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_IronSealMatrix",
            "Turns an item into a medieval iron seal matrix.",
            SealStampDefinition("an iron matrix bearing a formal seal legend", string.Empty,
                string.Empty, "Chartered Household", "Seal Keeper", "iron", Difficulty.ExtremelyHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_BrassOfficeSeal",
            "Turns an item into a medieval brass office seal stamp.",
            SealStampDefinition("a brass office seal bearing an institutional device", "Chancery Office",
                string.Empty, "Chancery", "Authorised Clerk", "brass", Difficulty.ExtremelyHard));
        _ = UpsertStockComponent("SealStamp", "SealStamp_Medieval_LeadSealMatrix",
            "Turns an item into a medieval lead seal matrix for official packets.",
            SealStampDefinition("a lead seal matrix bearing a sober administrative mark", "Record Office",
                string.Empty, string.Empty, "Seal Officer", "lead", Difficulty.VeryHard));

        var sealableComponentDefinitions = new[]
        {
            (Name: "Sealable_Document_Wax",
                Description: "Lets a document be sealed with wax and leave broken-seal residue.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "wax" }),
            (Name: "Sealable_Document_Clay",
                Description: "Lets a document be sealed with clay and leave broken-seal residue.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "clay" }),
            (Name: "Sealable_Envelope",
                Description: "Lets an envelope be sealed with wax, clay or paste.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "wax", "clay", "paste" }),
            (Name: "Sealable_Scroll",
                Description: "Lets a scroll be sealed with wax, clay or paste.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "wax", "clay", "paste" }),
            (Name: "Sealable_Container_Wax",
                Description: "Lets a closable container be sealed with wax or clay as tamper evidence.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "clay" }),
            (Name: "Sealable_Antiquity_Clay_Tablet_Edge",
                Description: "Lets a clay tablet edge take a clay or wax authority seal.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "clay", "wax" }),
            (Name: "Sealable_Antiquity_Clay_Bulla",
                Description: "Lets a clay bulla close tied antiquity documents, packets and account tags.",
                Difficulty: Difficulty.VeryHard,
                LeavesResidue: true,
                Media: new[] { "clay" }),
            (Name: "Sealable_Antiquity_Papyrus_Letter",
                Description: "Lets a small papyrus letter be folded and sealed with wax, clay or paste.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "wax", "clay", "paste" }),
            (Name: "Sealable_Antiquity_Papyrus_Scroll",
                Description: "Lets a papyrus scroll be sealed after writing with reed pen or brush.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "wax", "clay", "paste" }),
            (Name: "Sealable_Antiquity_Papyrus_Packet",
                Description: "Lets a packet of papyrus sheets or labels be closed with a small tamper-evident seal.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "wax", "clay", "paste" }),
            (Name: "Sealable_Antiquity_Wax_Tablet_Diptych",
                Description: "Lets a wax tablet diptych or triptych be tied and sealed after stylus writing.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "clay" }),
            (Name: "Sealable_Antiquity_Linen_Document_Bundle",
                Description: "Lets a cloth-wrapped document bundle be tied and sealed for archive handling.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "clay" }),
            (Name: "Sealable_Antiquity_Archive_Jar_Cap",
                Description: "Lets a jar cap, amphora cover or archive vessel be sealed as tamper evidence.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "clay", "wax" }),
            (Name: "Sealable_Medieval_Parchment_Charter",
                Description: "Lets a parchment charter be sealed with wax or a suspended lead bulla.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "lead" }),
            (Name: "Sealable_Medieval_Parchment_Roll",
                Description: "Lets a long parchment roll be secured with a formal wax or lead seal.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "lead" }),
            (Name: "Sealable_Medieval_Rag_Paper_Letter",
                Description: "Lets a folded rag-paper letter be sealed after quill or reed-pen writing.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "wax", "paste" }),
            (Name: "Sealable_Medieval_Official_Writ",
                Description: "Lets an official writ, summons or chancery document carry a high-confidence seal.",
                Difficulty: Difficulty.VeryHard,
                LeavesResidue: true,
                Media: new[] { "wax", "lead" }),
            (Name: "Sealable_Medieval_East_Asian_Scroll",
                Description: "Lets an East Asian paper scroll or brush-written document be sealed.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "paste", "wax", "clay" }),
            (Name: "Sealable_Medieval_Palm_Leaf_Bundle",
                Description: "Lets a palm-leaf manuscript bundle be corded and sealed after inscription.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "clay", "paste" }),
            (Name: "Sealable_Medieval_Document_Pouch",
                Description: "Lets a document pouch be sealed for courier or archive custody.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "lead" }),
            (Name: "Sealable_Medieval_Archive_Box",
                Description: "Lets a small archive box or record chest take a wax or lead tamper seal.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "wax", "lead" }),
            (Name: "Sealable_Modern_Business_Envelope",
                Description: "Lets a modern business envelope be sealed with gummed paper or adhesive.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "gummed paper", "adhesive", "paste" }),
            (Name: "Sealable_Modern_Padded_Envelope",
                Description: "Lets a padded mailing envelope be sealed with adhesive or security tape.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: true,
                Media: new[] { "adhesive", "security tape" }),
            (Name: "Sealable_Modern_File_Folder",
                Description: "Lets a file folder or dossier be closed with a removable office seal.",
                Difficulty: Difficulty.Normal,
                LeavesResidue: false,
                Media: new[] { "adhesive", "security tape", "paper seal" }),
            (Name: "Sealable_Modern_Security_Envelope",
                Description: "Lets a security envelope show tampering through adhesive or security tape residue.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "adhesive", "security tape", "tamper tape" }),
            (Name: "Sealable_Modern_Evidence_Bag",
                Description: "Lets an evidence bag be sealed with numbered or tamper-evident evidence tape.",
                Difficulty: Difficulty.VeryHard,
                LeavesResidue: true,
                Media: new[] { "evidence tape", "security tape", "numbered seal" }),
            (Name: "Sealable_Modern_Registered_Mail_Pouch",
                Description: "Lets a registered mail pouch or locked satchel receive a numbered custody seal.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "numbered seal", "plastic seal", "security tape" }),
            (Name: "Sealable_Modern_Courier_Tube",
                Description: "Lets a modern map, blueprint or document courier tube carry an external seal.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "adhesive", "security tape", "plastic seal" }),
            (Name: "Sealable_Modern_Diplomatic_Pouch",
                Description: "Lets a diplomatic pouch or high-security courier bag carry formal custody seals.",
                Difficulty: Difficulty.ExtremelyHard,
                LeavesResidue: true,
                Media: new[] { "lead", "numbered seal", "security tape", "plastic seal" }),
            (Name: "Sealable_Modern_Archive_Box",
                Description: "Lets a records archive box be sealed for storage or chain-of-custody control.",
                Difficulty: Difficulty.Hard,
                LeavesResidue: true,
                Media: new[] { "security tape", "paper seal", "adhesive" })
        };

        var sealableComponents = new Dictionary<string, GameItemComponentProto>(StringComparer.OrdinalIgnoreCase);
        foreach (var definition in sealableComponentDefinitions)
        {
            sealableComponents[definition.Name] = UpsertStockComponent("Sealable", definition.Name,
                definition.Description,
                SealableDefinition(definition.Difficulty, definition.LeavesResidue, definition.Media));
        }

        GameItemComponentProto sealableEnvelope = sealableComponents["Sealable_Envelope"];
        GameItemComponentProto sealableScroll = sealableComponents["Sealable_Scroll"];

        UpsertStockComponent("MeasuringInstrument", "MeasuringInstrument_Antiquity_BalanceScale",
            "Turns an item into a weight-mode balance scale with modest use-based drift.",
            MeasuringDefinition("Weight", 1.0, 50000.0, 0.00025, 0.03, 0.25, Difficulty.Normal));
        UpsertStockComponent("MeasuringInstrument", "MeasuringInstrument_Antiquity_StandardWeights",
            "Turns an item into a standard weight set for comparing trade goods.",
            MeasuringDefinition("Weight", 0.5, 20000.0, 0.0001, 0.015, 0.20, Difficulty.Hard));
        UpsertStockComponent("MeasuringInstrument", "MeasuringInstrument_Antiquity_FalseWeights",
            "Turns an item into a weight set intended for biased trade measures.",
            MeasuringDefinition("Weight", 0.5, 20000.0, 0.0001, 0.015, 0.35, Difficulty.VeryHard));
        UpsertStockComponent("MeasuringInstrument", "MeasuringInstrument_Antiquity_GrainMeasure",
            "Turns an item into a grain measure interpreted as a weight-mode trade measure.",
            MeasuringDefinition("Weight", 10.0, 100000.0, 0.00075, 0.05, 0.30, Difficulty.Normal));
        UpsertStockComponent("MeasuringInstrument", "MeasuringInstrument_Antiquity_OilCup",
            "Turns an item into a fluid-volume measure cup for oil.",
            MeasuringDefinition("FluidVolume", 0.005, 1.0, 0.0005, 0.04, 0.25, Difficulty.Normal));
        UpsertStockComponent("MeasuringInstrument", "MeasuringInstrument_Antiquity_WineCup",
            "Turns an item into a fluid-volume measure cup for wine.",
            MeasuringDefinition("FluidVolume", 0.01, 2.0, 0.0005, 0.04, 0.25, Difficulty.Normal));
        UpsertStockComponent("MeasuringInstrument", "MeasuringInstrument_Antiquity_TaxAssessorKit",
            "Turns an item into a tax assessor's kit for weighing taxable goods.",
            MeasuringDefinition("Weight", 5.0, 250000.0, 0.0002, 0.025, 0.20, Difficulty.Hard));

        _ = UpsertStockComponent("Container", "Container_Document_Pouch",
            "A closable pouch sized for folded documents, tablet packets and small writing supplies.",
            ContainerDefinition(2000, SizeCategory.VerySmall, true, false, "in"));
        _ = UpsertStockComponent("Container", "Container_Scroll_Tube",
            "A closable tube sized for scrolls, maps, narrow rolls and document rods.",
            ContainerDefinition(10000, SizeCategory.Small, true, false, "in"));
        _ = UpsertStockComponent("Container", "Container_Seal_Box",
            "A closable box sized for seal matrices, signets, wax lumps and authentication tools.",
            ContainerDefinition(2500, SizeCategory.VerySmall, true, false, "in"));
        _ = UpsertStockComponent("Container", "Container_Archive_Box",
            "A closable archive box sized for bundled papers, charters and small ledgers.",
            ContainerDefinition(40000, SizeCategory.Normal, true, false, "in"));
        _ = UpsertStockComponent("Container", "Container_Document_Satchel",
            "A closable satchel sized for books, documents, tablets and portable writing kits.",
            ContainerDefinition(25000, SizeCategory.Small, true, false, "in"));
        _ = UpsertStockComponent("Container", "Container_Document_Bookcase_Shelves",
            "Allows bookcases and library shelves to hold books, scrolls and document boxes on them.",
            ContainerDefinition(175000, SizeCategory.Large, false, true, "on"));
        _ = UpsertStockComponent("Container", "Container_Writing_Desk_Surface",
            "Allows a writing desk surface to hold books, documents, seals and writing tools on it.",
            ContainerDefinition(75000, SizeCategory.Normal, false, true, "on"));
        _ = UpsertStockComponent("Container", "Container_Writing_Desk_Drawers",
            "A closable drawer container for writing desks, document desks and scribal work tables.",
            ContainerDefinition(30000, SizeCategory.Small, true, false, "in"));
        _ = UpsertStockComponent("Container", "Container_Archive_Chest",
            "A closable chest container for archive bundles, ledgers, tablets and sealed records.",
            ContainerDefinition(160000, SizeCategory.Large, true, false, "in"));

        Material paperMaterial = EnsureMaterial("Paper", MaterialBehaviourType.Plant);
        Material parchmentMaterial = EnsureMaterial("Parchment", MaterialBehaviourType.Skin);
        Material palmLeafMaterial = EnsureMaterial("Palm Leaf", MaterialBehaviourType.Plant);
        GameItemComponentProto holdable = EnsureComponent("Holdable", "Holdable",
            "Allows an item to be picked up and manipulated.",
            new XElement("Definition"));
        GameItemComponentProto envelopeContainer = EnsureComponent("Container", "Container_Envelope",
            "Allows an envelope to hold small flat contents.",
            new XElement("Definition",
                new XAttribute("Weight", 50.0),
                new XAttribute("MaxSize", (int)SizeCategory.Tiny),
                new XAttribute("Preposition", "in"),
                new XAttribute("Closable", true),
                new XAttribute("Transparent", false),
                new XAttribute("OnceOnly", false)));
        GameItemComponentProto envelopeWriteable = EnsureComponent("PaperSheet", "PaperSheet_Envelope",
            "Turns an envelope into a small writable paper surface.",
            new XElement("Definition",
                new XElement("MaximumCharacterLengthOfText", 1000)));
        GameItemComponentProto scrollWriteable = EnsureComponent("PaperSheet", "PaperSheet_Scroll",
            "Turns a scroll into a long writable paper or parchment surface.",
            new XElement("Definition",
                new XElement("MaximumCharacterLengthOfText", 8000)));
        GameItemComponentProto destroyablePaper = EnsureComponent("Destroyable", "Destroyable_Paper",
            "Turns an item into a fragile paper or parchment object.",
            new XElement("Definition",
                new XElement("HpExpression", new XCData("2 * quality")),
                new XElement("DamageMultipliers",
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Burning), new XAttribute("multiplier", 2.5)),
                    new XElement("DamageMultiplier", new XAttribute("type", (int)DamageType.Piercing), new XAttribute("multiplier", 1.1)))));

        GameItemProto parchmentCodexLeaf = EnsureItemPrototype("leaf", "leaf parchment codex page",
            parchmentMaterial, SizeCategory.Tiny, 3.0, "a parchment codex leaf",
            "This is a trimmed parchment leaf prepared for binding into a codex.",
            holdable, medievalParchmentSheetSurface, destroyablePaper);
        GameItemProto ragPaperCodexLeaf = EnsureItemPrototype("leaf", "leaf rag paper codex page",
            paperMaterial, SizeCategory.Tiny, 1.0, "a rag-paper codex leaf",
            "This is a trimmed rag-paper leaf prepared for binding into a codex or ledger.",
            holdable, medievalRagPaperSheetSurface, destroyablePaper);
        GameItemProto eastAsianPaperLeaf = EnsureItemPrototype("leaf", "leaf east asian paper stitched book page",
            paperMaterial, SizeCategory.Tiny, 1.0, "an East Asian stitched-book leaf",
            "This is a thin paper leaf prepared for binding into a stitched book.",
            holdable, medievalEastAsianPaperSheetSurface, destroyablePaper);
        GameItemProto palmLeafManuscriptStrip = EnsureItemPrototype("strip", "strip palm leaf manuscript page",
            palmLeafMaterial, SizeCategory.Tiny, 4.0, "a palm-leaf manuscript strip",
            "This is a trimmed palm-leaf strip prepared for inscription and bundling into a manuscript.",
            holdable, medievalPalmLeafSurface, destroyablePaper);

        _ = UpsertStockComponent("Book", "Medieval_Parchment_Codex_20_Page",
            "Turns an item into a 20 page medieval parchment codex.",
            BookDefinition(parchmentCodexLeaf, 20));
        _ = UpsertStockComponent("Book", "Medieval_Parchment_Codex_40_Page",
            "Turns an item into a 40 page medieval parchment codex.",
            BookDefinition(parchmentCodexLeaf, 40));
        _ = UpsertStockComponent("Book", "Medieval_Parchment_Codex_90_Page",
            "Turns an item into a 90 page medieval parchment codex.",
            BookDefinition(parchmentCodexLeaf, 90));
        _ = UpsertStockComponent("Book", "Medieval_Rag_Paper_Codex_40_Page",
            "Turns an item into a 40 page medieval rag-paper codex.",
            BookDefinition(ragPaperCodexLeaf, 40));
        _ = UpsertStockComponent("Book", "Medieval_Account_Ledger_90_Page",
            "Turns an item into a 90 page medieval account ledger.",
            BookDefinition(ragPaperCodexLeaf, 90, "Account Ledger"));
        _ = UpsertStockComponent("Book", "Medieval_East_Asian_Stitched_Book",
            "Turns an item into a 40 leaf medieval East Asian stitched book.",
            BookDefinition(eastAsianPaperLeaf, 40));
        _ = UpsertStockComponent("Book", "Medieval_Palm_Leaf_Manuscript_Bundle",
            "Turns an item into a 40 strip palm-leaf manuscript bundle.",
            BookDefinition(palmLeafManuscriptStrip, 40));

        EnsureItemPrototype("envelope", "envelope paper sealable writable", paperMaterial, SizeCategory.Tiny, 10.0,
            "a sealable envelope",
            "This is a folded paper envelope with a closable flap suitable for writing, contents and sealing.",
            holdable, envelopeContainer, envelopeWriteable, sealableEnvelope, destroyablePaper);
        EnsureItemPrototype("scroll", "scroll paper parchment sealable writable", paperMaterial, SizeCategory.Small,
            25.0, "a sealable scroll",
            "This is a rolled writing scroll with a blank surface and enough overlap to bear a tamper-evident seal.",
            holdable, scrollWriteable, sealableScroll, destroyablePaper);

        _ = bronzeSignet;
        _ = cylinderSeal;

        nextId = currentId;
        context.SaveChanges();

        #endregion
    }

    private void SeedOfferingAndIncenseComponents(FuturemudDatabaseContext context, DateTime now, Account dbaccount,
        ref long nextId)
    {
        #region Offering Receivers and Incense Burners

        long currentId = nextId;

        GameItemComponentProto UpsertStockComponent(string type, string name, string description, XElement definition)
        {
            return UpsertComponent(context, ref currentId, dbaccount, now, type, name, description, definition.ToString());
        }

        Tag EnsureTagPath(string path)
        {
            var parts = path.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            Tag? parent = null;
            foreach (var part in parts)
            {
                long? parentId = parent?.Id;
                _tags.TryGetValue(part, out var cachedTag);
                var tag = context.Tags.Local
                                 .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase) &&
                                                      x.ParentId == parentId) ??
                          context.Tags
                                 .AsEnumerable()
                                 .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase) &&
                                                      x.ParentId == parentId) ??
                          cachedTag ??
                          context.Tags.Local
                                 .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase)) ??
                          context.Tags
                                 .AsEnumerable()
                                 .FirstOrDefault(x => x.Name.Equals(part, StringComparison.OrdinalIgnoreCase));
                if (tag is null)
                {
                    var existing = context.Tags.Any() ? context.Tags.Max(x => x.Id) : 0L;
                    var local = context.Tags.Local.Any() ? context.Tags.Local.Max(x => x.Id) : 0L;
                    tag = new Tag
                    {
                        Id = Math.Max(existing, local) + 1L,
                        Name = part,
                        Parent = parent,
                        ParentId = parent?.Id
                    };
                    context.Tags.Add(tag);
                }

                _tags[tag.Name] = tag;
                parent = tag;
            }

            return parent!;
        }

        XElement IncenseBurnerDefinition(Tag fuelTag, double maximumFuelWeight, double secondsPerUnitWeight,
            int scentRange, string sourceScent, string distantScent, Difficulty scentDifficulty)
        {
            return new XElement("Definition",
                new XElement("FuelTag", fuelTag.Id),
                new XElement("MaximumFuelWeight", maximumFuelWeight),
                new XElement("SecondsPerUnitWeight", secondsPerUnitWeight),
                new XElement("ScentRange", scentRange),
                new XElement("DrugRange", 0),
                new XElement("DrugPulseSeconds", 10),
                new XElement("LingeringMultiplier", 5.0),
                new XElement("SourceScentDescription", new XCData(sourceScent)),
                new XElement("DistantScentDescription", new XCData(distantScent)),
                new XElement("ScentDifficulty", (int)scentDifficulty),
                new XElement("Drug", 0),
                new XElement("GramsPerPulse", 0.0));
        }

        XElement OfferingReceiverDefinition(double maximumContentsWeight, SizeCategory maximumItemSize,
            string consumptionMode, string acceptEcho, string burnEcho)
        {
            return new XElement("Definition",
                new XElement("AllowedTags"),
                new XElement("BlockedTags"),
                new XElement("MaximumContentsWeight", maximumContentsWeight),
                new XElement("MaximumItemSize", (int)maximumItemSize),
                new XElement("ConsumptionMode", consumptionMode),
                new XElement("ResidueItemProto", 0),
                new XElement("CanOfferProg", 0),
                new XElement("OnOfferProg", 0),
                new XElement("OnBurnProg", 0),
                new XElement("AcceptEcho", new XCData(acceptEcho)),
                new XElement("BurnEcho", new XCData(burnEcho)),
                new XElement("RejectEcho", new XCData("$2 rejects $1.")));
        }

        Tag incenseFuelTag = EnsureTagPath("Functions / Household Items / Household Religious Items / Incense Fuel");

        UpsertStockComponent("IncenseBurner", "IncenseBurner_Antiquity_BronzeCenser",
            "Turns an item into a bronze censer that burns tagged incense fuel into room-scale ambient scent.",
            IncenseBurnerDefinition(incenseFuelTag, 750.0, 45.0, 1,
                "Sweet resinous smoke curls from $0.",
                "A faint sweet resinous smoke drifts in from nearby.",
                Difficulty.Easy));

        UpsertStockComponent("OfferingReceiver", "OfferingReceiver_Antiquity_HouseholdAltar",
            "Turns an item into a household altar that receives broad offerings and supports explicit burning.",
            OfferingReceiverDefinition(50000.0, SizeCategory.VeryLarge, "ManualBurn",
                "@ place|places $1 on $2 as an offering.",
                "@ consign|consigns $1 to the flames on $2."));

        UpsertStockComponent("OfferingReceiver", "OfferingReceiver_Antiquity_VotiveBasin",
            "Turns an item into a votive basin that burns offerings immediately when offered.",
            OfferingReceiverDefinition(15000.0, SizeCategory.Normal, "BurnOnOffer",
                "@ lay|lays $1 in $2 as a votive offering.",
                "@ burn|burns $1 in $2 as a votive offering."));

        UpsertStockComponent("OfferingReceiver", "OfferingReceiver_Antiquity_FuneralTray",
            "Turns an item into a funeral offering tray that can hold and ritually burn broad item offerings.",
            OfferingReceiverDefinition(25000.0, SizeCategory.Large, "ManualBurn",
                "@ arrange|arranges $1 on $2 as a funeral offering.",
                "@ burn|burns $1 on $2 as a funeral offering."));

        nextId = currentId;
        context.SaveChanges();

        #endregion
    }

    private void SeedSmokeables(FuturemudDatabaseContext context, DateTime now, Account dbaccount, ref long nextId)
    {
        #region Smokeables

        long currentId = nextId;
        bool saveChangesRequired = false;
        string characterTypeDefinition = ProgVariableTypes.Character.ToStorageString();
        if (!context.VariableDefinitions.Any(x => x.OwnerTypeDefinition == characterTypeDefinition && x.Property == "nicotineuntil"))
        {
            context.VariableDefinitions.Add(new VariableDefinition
            {
                ContainedType = (long)ProgVariableTypes.DateTime,
                OwnerType = 8,
                Property = "nicotineuntil"
            });
            saveChangesRequired = true;
        }

        if (!context.VariableDefaults.Any(x => x.OwnerTypeDefinition == characterTypeDefinition && x.Property == "nicotineuntil"))
        {
            context.VariableDefaults.Add(new VariableDefault
            {
                OwnerType = 8,
                Property = "nicotineuntil",
                DefaultValue = "<var>01/01/0001 00:00:00</var>"
            });
            saveChangesRequired = true;
        }

        FutureProg? smokeProg = context.FutureProgs.FirstOrDefault(x => x.FunctionName == "OnSmokeCigarette");
        if (smokeProg is null)
        {
            smokeProg = new FutureProg
            {
                FunctionName = "OnSmokeCigarette",
                AcceptsAnyParameters = false,
                ReturnType = 0,
                Category = "Character",
                Subcategory = "Smoking",
                Public = false,
                FunctionComment = "This prog gives the character a 5 minute nicotene hit.",
                FunctionText = @"var NicotineUntil as datetime
NicotineUntil = ifnull(GetRegister(@ch, ""NicotineUntil""), now())
if (@nicotineuntil < now())
  NicotineUntil = now()
end if
SetRegister @ch ""NicotineUntil"" (@NicotineUntil + 5m)",
                StaticType = 0
            };
            smokeProg.FutureProgsParameters.Add(new FutureProgsParameter
            { FutureProg = smokeProg, ParameterIndex = 0, ParameterName = "ch", ParameterType = (long)ProgVariableTypes.Character });
            smokeProg.FutureProgsParameters.Add(new FutureProgsParameter
            { FutureProg = smokeProg, ParameterIndex = 1, ParameterName = "item", ParameterType = (long)ProgVariableTypes.Item });
            context.FutureProgs.Add(smokeProg);
            saveChangesRequired = true;
        }

        if (saveChangesRequired)
        {
            context.SaveChanges();
        }
        GameItemComponentProto CreateSmokeable(string name, string description, int secondsOfFuel, int secondsPerDrag,
            int secondsOfEffectPerSecondOfFuel, string playerDescription, string roomDescription)
        {
            return CreateComponent(context, ref currentId, dbaccount, now, "Smokeable", name, description,
                new XElement("Definition",
                    new XElement("SecondsOfFuel", secondsOfFuel),
                    new XElement("SecondsPerDrag", secondsPerDrag),
                    new XElement("SecondsOfEffectPerSecondOfFuel", secondsOfEffectPerSecondOfFuel),
                    new XElement("OnDragProg", smokeProg.Id),
                    new XElement("PlayerDescriptionEffectString", new XCData(playerDescription)),
                    new XElement("RoomDescriptionEffectString", new XCData(roomDescription)),
                    new XElement("Drug", 0),
                    new XElement("GramsPerDrag", 0)).ToString());
        }

        CreateSmokeable("Cigarette",
            "Turns an item into a smokeable tobacco cigarette.",
            600,
            10,
            5,
            "The lingering, acrid smell of tobacco clings to this individual.",
            "The lingering, acrid smell of tobacco hangs in the air here.");
        CreateSmokeable("Smokeable_Cigar",
            "Turns an item into a longer-burning smokeable tobacco cigar.",
            5400,
            60,
            5,
            "The rich, heavy smell of cigar smoke clings to this individual.",
            "The rich, heavy smell of cigar smoke hangs in the air here.");
        CreateSmokeable("Smokeable_Cigarillo",
            "Turns an item into a compact smokeable cigarillo.",
            1800,
            30,
            5,
            "A sweet, lingering cigarillo scent clings to this individual.",
            "A sweet, lingering cigarillo scent hangs in the air here.");
        CreateSmokeable("Smokeable_PipeBowl",
            "Turns an item into a prepared bowl of pipe tobacco for smoking.",
            3600,
            45,
            5,
            "The warm, aromatic smell of pipe smoke clings to this individual.",
            "The warm, aromatic smell of pipe smoke hangs in the air here.");
        nextId = currentId;
        #endregion
        context.SaveChanges();
    }
}

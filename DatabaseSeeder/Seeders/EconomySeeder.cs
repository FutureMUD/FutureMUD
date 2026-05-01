#nullable enable

using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Intervals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

public class EconomySeeder : IDatabaseSeeder
{
    private const string MarketRootTagName = "Market";
    private const string CategoryPrefix = "Economy Category";
    private const string ZoneSuffix = "Economy Template Zone";
    private const string MarketSuffix = "Economy Template Market";
    private const string DefaultMarketPriceFormula =
        "if(demand<=0,0,if(supply<=0,100,1 + (elasticity * min(1, max(-1, (demand-supply) / min(demand,supply))))))";
    private const string HelperProgPrefix = "EconomySeeder";

    public static readonly IReadOnlyDictionary<string, string[]> RequiredMarketTagHierarchy =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Nourishment"] = ["Staple Food", "Standard Food", "Luxury Food", "Seasonings", "Salt", "Spices"],
            ["Domestic Heating"] = ["Combustion Heating", "Oil Heating", "Electric Heating"],
            ["Lighting"] = ["Candles", "Torches", "Lamps"],
            ["Medicine"] = ["Simple Medicine", "Standard Medicine", "High-Quality Medicine"],
            ["Writing Materials"] = ["Wax Tablets", "Parchment", "Paper", "Ink"],
            ["Clothing"] = ["Simple Clothing", "Standard Clothing", "Luxury Clothing", "Winter Clothing", "Military Uniforms"],
            ["Intoxicants"] = ["Beer", "Wine", "Mead", "Spirits"],
            ["Luxury Drinks"] = ["Tea", "Coffee"],
            ["Household Goods"] =
            [
                "Simple Wares",
                "Standard Wares",
                "Luxury Wares",
                "Simple Furniture",
                "Standard Furniture",
                "Luxury Furniture",
                "Simple Decorations",
                "Standard Decorations",
                "Luxury Decorations"
            ],
            ["Hospitality"] = ["Simple Lodging", "Standard Lodging", "Luxury Lodging", "Prepared Meals", "Common Meals", "Fine Dining", "Bathhouse Services", "Stabling Services"],
            ["Entertainment"] = ["Cheap Entertainment", "Standard Entertainment", "Luxury Entertainment", "Music Performance", "Theatre Performance", "Festival Entertainment", "Sporting Entertainment"],
            ["Personal Services"] = ["Bathing Services", "Domestic Services", "Barbering", "Laundry Services", "Tailoring Services", "Bodyguard Services", "Grooming Supplies"],
            ["Religious Goods"] = ["Ritual Supplies", "Temple Offerings", "Funerary Goods", "Devotional Goods"],
            ["Household Consumables"] = ["Lamp Oil", "Cleaning Supplies", "Candlemaking Wax", "Toiletries"],
            ["Military Goods"] =
            [
                "Weapons",
                "Spears",
                "Swords",
                "Clubs",
                "Axes",
                "Maces",
                "Daggers",
                "Crossbows",
                "Bows",
                "Guns",
                "Hammers",
                "Polearms",
                "Other Weapons",
                "Armour",
                "Leather Armour",
                "Mail Armour",
                "Plate Armour",
                "Primitive Armour",
                "Shields",
                "Ammunition",
                "Arrows",
                "Bolts",
                "Bullets",
                "Blackpowder"
            ],
            ["Transportation"] = ["Cargo Transportation", "Cart Haulage", "Manual Haulage", "Mule Haulage", "Ship Haulage", "Passenger Transportation", "Cart Passage", "Horse Passage", "Wagon Passage", "Ship Passage"],
            ["Warehousing"] = [],
            ["Communications"] = ["Messenger Services", "Courier Services", "Postal Services", "Printed News", "Telegraph Services", "Telephone Services"],
            ["Professional Tools"] = ["Primitive Tools", "Simple Tools", "Standard Tools", "High-Quality Tools"],
            ["Raw Materials"] = ["Lumber", "Straw", "Cloth", "Stone Blocks", "Sand", "Clay", "Aggregate", "Cement Mineral", "Steel", "Copper", "Gold", "Silver", "Bronze", "Brass", "Lead"],
            ["Construction Materials"] = ["Brick", "Mortar", "Lime", "Worked Timber", "Worked Stone", "Glass Panes", "Roofing Materials"]
        };

    private static readonly IReadOnlyCollection<string> RequiredMarketTagNames = RequiredMarketTagHierarchy
        .SelectMany(x => x.Value.Prepend(x.Key))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

	private static readonly IReadOnlySet<string> StockCombinationFamilyExamples =
		new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"Medicine",
			"Writing Materials",
            "Clothing",
            "Intoxicants",
            "Household Goods",
            "Hospitality",
            "Entertainment",
            "Personal Services",
            "Communications",
			"Military Goods",
			"Professional Tools"
		};

	private static readonly IReadOnlyDictionary<string, IReadOnlyList<(string CategoryName, decimal Weight)>> StockCombinationCategoryWeights =
		new Dictionary<string, IReadOnlyList<(string CategoryName, decimal Weight)>>(StringComparer.OrdinalIgnoreCase)
		{
			["Medicine"] =
			[
				("Simple Medicine", 0.55m),
				("Standard Medicine", 0.30m),
				("High-Quality Medicine", 0.15m)
			],
			["Writing Materials"] =
			[
				("Wax Tablets", 0.15m),
				("Parchment", 0.35m),
				("Paper", 0.30m),
				("Ink", 0.20m)
			],
			["Clothing"] =
			[
				("Simple Clothing", 0.40m),
				("Standard Clothing", 0.30m),
				("Winter Clothing", 0.12m),
				("Luxury Clothing", 0.10m),
				("Military Uniforms", 0.08m)
			],
			["Intoxicants"] =
			[
				("Beer", 0.45m),
				("Wine", 0.25m),
				("Mead", 0.15m),
				("Spirits", 0.15m)
			],
			["Household Goods"] =
			[
				("Simple Wares", 0.16m),
				("Standard Wares", 0.14m),
				("Luxury Wares", 0.07m),
				("Simple Furniture", 0.16m),
				("Standard Furniture", 0.14m),
				("Luxury Furniture", 0.08m),
				("Simple Decorations", 0.10m),
				("Standard Decorations", 0.09m),
				("Luxury Decorations", 0.06m)
			],
			["Hospitality"] =
			[
				("Simple Lodging", 0.25m),
				("Standard Lodging", 0.25m),
				("Prepared Meals", 0.22m),
				("Luxury Lodging", 0.12m),
				("Bathhouse Services", 0.10m),
				("Stabling Services", 0.06m)
			],
			["Entertainment"] =
			[
				("Cheap Entertainment", 0.30m),
				("Standard Entertainment", 0.24m),
				("Luxury Entertainment", 0.12m),
				("Music Performance", 0.10m),
				("Theatre Performance", 0.09m),
				("Festival Entertainment", 0.10m),
				("Sporting Entertainment", 0.05m)
			],
			["Personal Services"] =
			[
				("Domestic Services", 0.20m),
				("Bathing Services", 0.18m),
				("Barbering", 0.14m),
				("Laundry Services", 0.14m),
				("Tailoring Services", 0.14m),
				("Bodyguard Services", 0.10m),
				("Grooming Supplies", 0.10m)
			],
			["Communications"] =
			[
				("Messenger Services", 0.24m),
				("Courier Services", 0.22m),
				("Postal Services", 0.20m),
				("Printed News", 0.12m),
				("Telegraph Services", 0.12m),
				("Telephone Services", 0.10m)
			],
			["Military Goods"] =
			[
				("Weapons", 0.40m),
				("Armour", 0.35m),
				("Ammunition", 0.25m)
			],
			["Professional Tools"] =
			[
				("Primitive Tools", 0.25m),
				("Simple Tools", 0.30m),
				("Standard Tools", 0.30m),
				("High-Quality Tools", 0.15m)
			]
		};

    private static readonly IReadOnlyList<EraDefinition> EraDefinitions =
    [
        new(
            "Classical Age",
            "Classical Age",
            [
                new PopulationBlueprint(
                    "Urban Poor",
                    "City labourers and dependants with little margin for hardship and a constant need for staples, salt, cheap remedies and basic household goods.",
                    7200,
                    PopulationArchetype.Commoner,
                    [
                        new PopulationNeedBlueprint("Staple Food", 185m, 8),
                        new PopulationNeedBlueprint("Salt", 14m, 4),
                        new PopulationNeedBlueprint("Simple Medicine", 18m, 3),
                        new PopulationNeedBlueprint("Simple Clothing", 48m, 5),
                        new PopulationNeedBlueprint("Lighting", 18m, 2),
                        new PopulationNeedBlueprint("Simple Wares", 24m, 3),
                        new PopulationNeedBlueprint("Beer", 16m, 1),
                        new PopulationNeedBlueprint("Cheap Entertainment", 16m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 8m, 1),
                    ]),
                new PopulationBlueprint(
                    "Rural Smallholders",
                    "Subsistence-minded smallholders who still spend on tools, cloth, salt, remedies and a little market produce.",
                    5600,
                    PopulationArchetype.Rural,
                    [
                        new PopulationNeedBlueprint("Staple Food", 165m, 7),
                        new PopulationNeedBlueprint("Standard Food", 42m, 3),
                        new PopulationNeedBlueprint("Salt", 12m, 4),
                        new PopulationNeedBlueprint("Simple Medicine", 16m, 3),
                        new PopulationNeedBlueprint("Simple Clothing", 40m, 4),
                        new PopulationNeedBlueprint("Primitive Tools", 44m, 5),
                        new PopulationNeedBlueprint("Lighting", 14m, 1),
                        new PopulationNeedBlueprint("Combustion Heating", 12m, 1),
                        new PopulationNeedBlueprint("Household Consumables", 12m, 1)
                    ]),
                new PopulationBlueprint(
                    "Artisan-Merchant Households",
                    "Urban craft and trading households who buy better food, seasonings, remedies, wares and productive equipment.",
                    1800,
                    PopulationArchetype.Merchant,
                    [
                        new PopulationNeedBlueprint("Standard Food", 175m, 6),
                        new PopulationNeedBlueprint("Salt", 16m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 22m, 3),
                        new PopulationNeedBlueprint("Standard Clothing", 84m, 5),
                        new PopulationNeedBlueprint("Standard Wares", 76m, 5),
                        new PopulationNeedBlueprint("Standard Furniture", 46m, 3),
                        new PopulationNeedBlueprint("Simple Tools", 58m, 6),
                        new PopulationNeedBlueprint("Transportation", 28m, 2),
                        new PopulationNeedBlueprint("Messenger Services", 18m, 2),
                        new PopulationNeedBlueprint("Standard Lodging", 28m, 2),
                        new PopulationNeedBlueprint("Standard Entertainment", 36m, 2),
                        new PopulationNeedBlueprint("Wine", 22m, 1)
                    ]),
                new PopulationBlueprint(
                    "Soldiery",
                    "Professional troops and camp followers whose spending leans toward equipment, preserved food, simple remedies and ready drink.",
                    1300,
                    PopulationArchetype.Martial,
                    [
                        new PopulationNeedBlueprint("Standard Food", 185m, 6),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Simple Medicine", 26m, 4),
                        new PopulationNeedBlueprint("Military Uniforms", 78m, 5),
                        new PopulationNeedBlueprint("Weapons", 60m, 7),
                        new PopulationNeedBlueprint("Armour", 42m, 5),
                        new PopulationNeedBlueprint("Ammunition", 28m, 4),
                        new PopulationNeedBlueprint("Beer", 22m, 2),
                        new PopulationNeedBlueprint("Cheap Entertainment", 16m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 8m, 1),
                    ]),
                new PopulationBlueprint(
                    "Temple Priesthood",
                    "Temple and civic priestly households who maintain respectable dress, ritual goods, lighting, wine and better remedies.",
                    540,
                    PopulationArchetype.Clergy,
                    [
                        new PopulationNeedBlueprint("Standard Food", 150m, 5),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 24m, 4),
                        new PopulationNeedBlueprint("Standard Clothing", 68m, 5),
                        new PopulationNeedBlueprint("Lighting", 24m, 4),
                        new PopulationNeedBlueprint("Standard Wares", 32m, 3),
                        new PopulationNeedBlueprint("Wax Tablets", 18m, 2),
                        new PopulationNeedBlueprint("Ink", 8m, 1),
                        new PopulationNeedBlueprint("Wine", 16m, 2),
                        new PopulationNeedBlueprint("Luxury Decorations", 18m, 2),
                        new PopulationNeedBlueprint("Religious Goods", 30m, 2),
                   ]),
                new PopulationBlueprint(
                    "Monastic Orders",
                    "Disciplined communal households who spend modestly but steadily on staples, remedies, light and simple domestic supplies.",
                    460,
                    PopulationArchetype.Monastic,
                    [
                        new PopulationNeedBlueprint("Staple Food", 150m, 6),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Simple Medicine", 24m, 5),
                        new PopulationNeedBlueprint("Simple Clothing", 34m, 4),
                        new PopulationNeedBlueprint("Lighting", 22m, 4),
                        new PopulationNeedBlueprint("Simple Wares", 18m, 2),
                        new PopulationNeedBlueprint("Simple Furniture", 18m, 2),
                        new PopulationNeedBlueprint("Wax Tablets", 10m, 1),
                        new PopulationNeedBlueprint("Ink", 6m, 1),
                        new PopulationNeedBlueprint("Wine", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Patrician Elite",
                    "Landed and civic elites with a taste for luxury imports, refined food, rich seasonings, advanced remedies and visible household display.",
                    280,
                    PopulationArchetype.Elite,
                    [
                        new PopulationNeedBlueprint("Luxury Food", 280m, 8),
                        new PopulationNeedBlueprint("Spices", 40m, 4),
                        new PopulationNeedBlueprint("High-Quality Medicine", 48m, 4),
                        new PopulationNeedBlueprint("Luxury Clothing", 220m, 7),
                        new PopulationNeedBlueprint("Luxury Furniture", 160m, 5),
                        new PopulationNeedBlueprint("Luxury Decorations", 120m, 4),
                        new PopulationNeedBlueprint("Wax Tablets", 22m, 2),
                        new PopulationNeedBlueprint("Ink", 12m, 1),
                        new PopulationNeedBlueprint("Luxury Drinks", 84m, 3),
                        new PopulationNeedBlueprint("Transportation", 55m, 2),
                        new PopulationNeedBlueprint("Luxury Entertainment", 55m, 2),
                        new PopulationNeedBlueprint("Luxury Lodging", 55m, 2),
                        new PopulationNeedBlueprint("Domestic Services", 55m, 2),
                        new PopulationNeedBlueprint("Messenger Services", 18m, 2),
                        new PopulationNeedBlueprint("Courier Services", 55m, 2),
                    ])
            ]),
        new(
            "Feudal Age",
            "Feudal Age",
            [
                new PopulationBlueprint(
                    "Peasantry",
                    "Village peasants focused on staples, salt, simple remedies, coarse clothing, firewood and inexpensive household wares.",
                    7600,
                    PopulationArchetype.Commoner,
                    [
                        new PopulationNeedBlueprint("Staple Food", 190m, 8),
                        new PopulationNeedBlueprint("Salt", 14m, 4),
                        new PopulationNeedBlueprint("Simple Medicine", 18m, 3),
                        new PopulationNeedBlueprint("Simple Clothing", 42m, 5),
                        new PopulationNeedBlueprint("Combustion Heating", 28m, 3),
                        new PopulationNeedBlueprint("Lighting", 16m, 2),
                        new PopulationNeedBlueprint("Simple Wares", 20m, 2),
                        new PopulationNeedBlueprint("Household Consumables", 10m, 1),
                        new PopulationNeedBlueprint("Cheap Entertainment", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Manor Households",
                    "Stewarded rural households that spend on food, seasonings, remedies, heating and the tools that keep estates running.",
                    950,
                    PopulationArchetype.Rural,
                    [
                        new PopulationNeedBlueprint("Standard Food", 175m, 6),
                        new PopulationNeedBlueprint("Salt", 16m, 3),
                        new PopulationNeedBlueprint("Simple Medicine", 18m, 3),
                        new PopulationNeedBlueprint("Standard Clothing", 68m, 5),
                        new PopulationNeedBlueprint("Combustion Heating", 34m, 3),
                        new PopulationNeedBlueprint("Simple Furniture", 36m, 2),
                        new PopulationNeedBlueprint("Simple Tools", 48m, 6),
                        new PopulationNeedBlueprint("Mule Haulage", 20m, 1),
                        new PopulationNeedBlueprint("Household Consumables", 14m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Itinerant Tradesfolk",
                    "Travelling or market-town tradesfolk whose spending favours food, seasonings, remedies, tools, transport and middling comforts.",
                    1400,
                    PopulationArchetype.Merchant,
                    [
                        new PopulationNeedBlueprint("Standard Food", 160m, 6),
                        new PopulationNeedBlueprint("Salt", 16m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 22m, 3),
                        new PopulationNeedBlueprint("Standard Clothing", 72m, 5),
                        new PopulationNeedBlueprint("Simple Tools", 62m, 7),
                        new PopulationNeedBlueprint("Transportation", 30m, 3),
                        new PopulationNeedBlueprint("Standard Wares", 48m, 3),
                        new PopulationNeedBlueprint("Beer", 18m, 1),
                        new PopulationNeedBlueprint("Messenger Services", 14m, 2),
                        new PopulationNeedBlueprint("Standard Lodging", 22m, 2),
                        new PopulationNeedBlueprint("Cheap Entertainment", 14m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Retainers",
                    "Household men-at-arms and armed retainers who spend on kit, food, salting stores, simple medicine and practical clothing.",
                    1100,
                    PopulationArchetype.Martial,
                    [
                        new PopulationNeedBlueprint("Standard Food", 170m, 6),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Simple Medicine", 24m, 4),
                        new PopulationNeedBlueprint("Military Uniforms", 68m, 5),
                        new PopulationNeedBlueprint("Weapons", 52m, 7),
                        new PopulationNeedBlueprint("Armour", 38m, 5),
                        new PopulationNeedBlueprint("Simple Clothing", 32m, 3),
                        new PopulationNeedBlueprint("Beer", 18m, 1),
                        new PopulationNeedBlueprint("Cheap Entertainment", 14m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Parish Priesthood",
                    "Parish and chapel households that sustain respectable dress, medicine, altar light, wine and practical devotional goods.",
                    520,
                    PopulationArchetype.Clergy,
                    [
                        new PopulationNeedBlueprint("Standard Food", 148m, 5),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 22m, 4),
                        new PopulationNeedBlueprint("Standard Clothing", 64m, 5),
                        new PopulationNeedBlueprint("Lighting", 24m, 4),
                        new PopulationNeedBlueprint("Standard Wares", 28m, 2),
                        new PopulationNeedBlueprint("Parchment", 18m, 2),
                        new PopulationNeedBlueprint("Ink", 10m, 1),
                        new PopulationNeedBlueprint("Wine", 18m, 2),
                        new PopulationNeedBlueprint("Standard Decorations", 16m, 2),
                        new PopulationNeedBlueprint("Religious Goods", 22m, 2)
                    ]),
                new PopulationBlueprint(
                    "Monastic Orders",
                    "Monastic communities with modest but reliable spending on staples, salt, medicines, light and simple workshop goods.",
                    430,
                    PopulationArchetype.Monastic,
                    [
                        new PopulationNeedBlueprint("Staple Food", 152m, 6),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Simple Medicine", 26m, 5),
                        new PopulationNeedBlueprint("Simple Clothing", 32m, 4),
                        new PopulationNeedBlueprint("Lighting", 24m, 4),
                        new PopulationNeedBlueprint("Simple Wares", 18m, 2),
                        new PopulationNeedBlueprint("Simple Tools", 20m, 2),
                        new PopulationNeedBlueprint("Parchment", 12m, 1),
                        new PopulationNeedBlueprint("Ink", 8m, 1),
                        new PopulationNeedBlueprint("Simple Furniture", 16m, 1),
                        new PopulationNeedBlueprint("Religious Goods", 14m, 1),
                        new PopulationNeedBlueprint("Household Consumables", 12m, 1)
                    ]),
                new PopulationBlueprint(
                    "Noble Elite",
                    "High-status secular households who maintain prestige through quality food, rich seasonings, household display and better medicine.",
                    260,
                    PopulationArchetype.Elite,
                    [
                        new PopulationNeedBlueprint("Luxury Food", 265m, 8),
                        new PopulationNeedBlueprint("Spices", 36m, 4),
                        new PopulationNeedBlueprint("High-Quality Medicine", 44m, 4),
                        new PopulationNeedBlueprint("Luxury Clothing", 205m, 7),
                        new PopulationNeedBlueprint("Luxury Furniture", 145m, 4),
                        new PopulationNeedBlueprint("Luxury Decorations", 108m, 4),
                        new PopulationNeedBlueprint("Parchment", 20m, 2),
                        new PopulationNeedBlueprint("Ink", 12m, 1),
                        new PopulationNeedBlueprint("Wine", 56m, 2),
                        new PopulationNeedBlueprint("Transportation", 46m, 1),
                        new PopulationNeedBlueprint("Luxury Lodging", 42m, 2),
                        new PopulationNeedBlueprint("Luxury Entertainment", 38m, 2),
                        new PopulationNeedBlueprint("Domestic Services", 40m, 2),
                        new PopulationNeedBlueprint("Courier Services", 18m, 1)
                    ])
            ]),
        new(
            "Medieval Age",
            "Medieval Age",
            [
                new PopulationBlueprint(
                    "Rural Peasants",
                    "Ordinary peasants whose market spend is dominated by staples, salt, simple remedies, cloth and fuel.",
                    7100,
                    PopulationArchetype.Commoner,
                    [
                        new PopulationNeedBlueprint("Staple Food", 182m, 8),
                        new PopulationNeedBlueprint("Salt", 14m, 4),
                        new PopulationNeedBlueprint("Simple Medicine", 18m, 3),
                        new PopulationNeedBlueprint("Simple Clothing", 40m, 5),
                        new PopulationNeedBlueprint("Combustion Heating", 26m, 3),
                        new PopulationNeedBlueprint("Lighting", 16m, 2),
                        new PopulationNeedBlueprint("Simple Wares", 20m, 2),
                        new PopulationNeedBlueprint("Household Consumables", 10m, 1),
                        new PopulationNeedBlueprint("Cheap Entertainment", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Town Artisans",
                    "Workshop households buying reliable food, salt, decent clothing, medicines, wares and the tools of their trade.",
                    2200,
                    PopulationArchetype.Merchant,
                    [
                        new PopulationNeedBlueprint("Standard Food", 158m, 6),
                        new PopulationNeedBlueprint("Salt", 16m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 20m, 3),
                        new PopulationNeedBlueprint("Standard Clothing", 76m, 5),
                        new PopulationNeedBlueprint("Simple Tools", 56m, 7),
                        new PopulationNeedBlueprint("Standard Wares", 60m, 4),
                        new PopulationNeedBlueprint("Standard Furniture", 38m, 2),
                        new PopulationNeedBlueprint("Lighting", 22m, 2),
                        new PopulationNeedBlueprint("Household Consumables", 12m, 1),
                        new PopulationNeedBlueprint("Cheap Entertainment", 14m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Guild-Merchant Households",
                    "Prosperous guild and merchant families whose spending reaches into seasonings, medicines, transport, luxury drink and better wares.",
                    1100,
                    PopulationArchetype.Merchant,
                    [
                        new PopulationNeedBlueprint("Standard Food", 172m, 6),
                        new PopulationNeedBlueprint("Spices", 24m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 28m, 3),
                        new PopulationNeedBlueprint("Standard Clothing", 88m, 5),
                        new PopulationNeedBlueprint("Standard Wares", 76m, 4),
                        new PopulationNeedBlueprint("Paper", 14m, 1),
                        new PopulationNeedBlueprint("Ink", 10m, 1),
                        new PopulationNeedBlueprint("Transportation", 32m, 3),
                        new PopulationNeedBlueprint("Luxury Drinks", 30m, 2),
                        new PopulationNeedBlueprint("High-Quality Tools", 44m, 4),
                        new PopulationNeedBlueprint("Messenger Services", 16m, 2),
                        new PopulationNeedBlueprint("Standard Lodging", 24m, 2),
                        new PopulationNeedBlueprint("Standard Entertainment", 24m, 2),
                        new PopulationNeedBlueprint("Bathing Services", 10m, 1)
                    ]),
                new PopulationBlueprint(
                    "Garrison Men-At-Arms",
                    "Permanent guards and soldiers with steady demand for equipment, food, preserved staples, field medicine, clothing and drink.",
                    1350,
                    PopulationArchetype.Martial,
                    [
                        new PopulationNeedBlueprint("Standard Food", 176m, 6),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Simple Medicine", 26m, 4),
                        new PopulationNeedBlueprint("Military Uniforms", 72m, 5),
                        new PopulationNeedBlueprint("Weapons", 56m, 7),
                        new PopulationNeedBlueprint("Armour", 40m, 5),
                        new PopulationNeedBlueprint("Ammunition", 26m, 4),
                        new PopulationNeedBlueprint("Beer", 18m, 1),
                        new PopulationNeedBlueprint("Cheap Entertainment", 14m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 8m, 1)
                    ]),
                new PopulationBlueprint(
                    "Parish Clergy",
                    "Parish clergy and cathedral prebends who spend on respectable food, medicines, vesture, candles and liturgical goods.",
                    560,
                    PopulationArchetype.Clergy,
                    [
                        new PopulationNeedBlueprint("Standard Food", 154m, 5),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 24m, 4),
                        new PopulationNeedBlueprint("Standard Clothing", 70m, 5),
                        new PopulationNeedBlueprint("Lighting", 26m, 4),
                        new PopulationNeedBlueprint("Standard Wares", 30m, 2),
                        new PopulationNeedBlueprint("Parchment", 18m, 2),
                        new PopulationNeedBlueprint("Ink", 12m, 1),
                        new PopulationNeedBlueprint("Wine", 18m, 2),
                        new PopulationNeedBlueprint("Standard Decorations", 18m, 2),
                        new PopulationNeedBlueprint("Religious Goods", 24m, 2)
                    ]),
                new PopulationBlueprint(
                    "Monastic Orders",
                    "Monastic foundations with disciplined communal spending on food, salt, remedies, candlelight, furnishings and workshop supplies.",
                    470,
                    PopulationArchetype.Monastic,
                    [
                        new PopulationNeedBlueprint("Staple Food", 158m, 6),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Simple Medicine", 28m, 5),
                        new PopulationNeedBlueprint("Simple Clothing", 34m, 4),
                        new PopulationNeedBlueprint("Lighting", 26m, 4),
                        new PopulationNeedBlueprint("Simple Wares", 18m, 2),
                        new PopulationNeedBlueprint("Simple Tools", 24m, 2),
                        new PopulationNeedBlueprint("Parchment", 12m, 1),
                        new PopulationNeedBlueprint("Ink", 8m, 1),
                        new PopulationNeedBlueprint("Simple Furniture", 18m, 1),
                        new PopulationNeedBlueprint("Religious Goods", 16m, 1)
                    ]),
                new PopulationBlueprint(
                    "Noble Elite",
                    "High-born households with demand for luxuries, display goods, seasonings and high quality medicines.",
                    240,
                    PopulationArchetype.Elite,
                    [
                        new PopulationNeedBlueprint("Luxury Food", 272m, 8),
                        new PopulationNeedBlueprint("Spices", 38m, 4),
                        new PopulationNeedBlueprint("High-Quality Medicine", 46m, 4),
                        new PopulationNeedBlueprint("Luxury Clothing", 214m, 7),
                        new PopulationNeedBlueprint("Luxury Decorations", 118m, 4),
                        new PopulationNeedBlueprint("Luxury Furniture", 150m, 4),
                        new PopulationNeedBlueprint("Paper", 18m, 2),
                        new PopulationNeedBlueprint("Ink", 12m, 1),
                        new PopulationNeedBlueprint("Luxury Drinks", 54m, 2),
                        new PopulationNeedBlueprint("Transportation", 48m, 1),
                        new PopulationNeedBlueprint("Luxury Lodging", 44m, 2),
                        new PopulationNeedBlueprint("Luxury Entertainment", 48m, 2),
                        new PopulationNeedBlueprint("Domestic Services", 48m, 2),
                        new PopulationNeedBlueprint("Courier Services", 18m, 1)
                    ])
            ]),
        new(
            "Early Modern Age",
            "Early Modern Age",
            [
                new PopulationBlueprint(
                    "Labourers",
                    "Daily-wage labourers whose spending remains tightly focused on bread, salt, simple medicine, fuel, clothing and cheap drink.",
                    7400,
                    PopulationArchetype.Commoner,
                    [
                        new PopulationNeedBlueprint("Staple Food", 188m, 8),
                        new PopulationNeedBlueprint("Salt", 16m, 4),
                        new PopulationNeedBlueprint("Simple Medicine", 20m, 3),
                        new PopulationNeedBlueprint("Simple Clothing", 44m, 5),
                        new PopulationNeedBlueprint("Combustion Heating", 28m, 3),
                        new PopulationNeedBlueprint("Lighting", 20m, 2),
                        new PopulationNeedBlueprint("Beer", 18m, 1),
                        new PopulationNeedBlueprint("Household Consumables", 12m, 1),
                        new PopulationNeedBlueprint("Cheap Entertainment", 14m, 1)
                    ]),
                new PopulationBlueprint(
                    "Middling Households",
                    "Comfort-seeking households of clerks, craftsmen and small masters with regular purchases across food, medicines and practical goods.",
                    2600,
                    PopulationArchetype.Merchant,
                    [
                        new PopulationNeedBlueprint("Standard Food", 170m, 6),
                        new PopulationNeedBlueprint("Salt", 16m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 24m, 3),
                        new PopulationNeedBlueprint("Standard Clothing", 82m, 5),
                        new PopulationNeedBlueprint("Standard Wares", 68m, 4),
                        new PopulationNeedBlueprint("Standard Furniture", 46m, 3),
                        new PopulationNeedBlueprint("Paper", 16m, 1),
                        new PopulationNeedBlueprint("Ink", 10m, 1),
                        new PopulationNeedBlueprint("Simple Tools", 36m, 3),
                        new PopulationNeedBlueprint("Lighting", 24m, 2),
                        new PopulationNeedBlueprint("Household Consumables", 16m, 1),
                        new PopulationNeedBlueprint("Standard Entertainment", 18m, 1),
                        new PopulationNeedBlueprint("Barbering", 10m, 1),
                        new PopulationNeedBlueprint("Printed News", 12m, 1)
                    ]),
                new PopulationBlueprint(
                    "Merchant And Professional Class",
                    "Established urban households buying quality wares, seasonings, medicines, transport, luxuries and better productive tools.",
                    900,
                    PopulationArchetype.Merchant,
                    [
                        new PopulationNeedBlueprint("Standard Food", 182m, 6),
                        new PopulationNeedBlueprint("Spices", 28m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 32m, 4),
                        new PopulationNeedBlueprint("Standard Clothing", 98m, 5),
                        new PopulationNeedBlueprint("High-Quality Tools", 58m, 4),
                        new PopulationNeedBlueprint("Paper", 20m, 2),
                        new PopulationNeedBlueprint("Ink", 14m, 1),
                        new PopulationNeedBlueprint("Transportation", 40m, 3),
                        new PopulationNeedBlueprint("Luxury Drinks", 36m, 2),
                        new PopulationNeedBlueprint("Luxury Decorations", 42m, 2),
                        new PopulationNeedBlueprint("Postal Services", 18m, 2),
                        new PopulationNeedBlueprint("Standard Lodging", 30m, 2),
                        new PopulationNeedBlueprint("Standard Entertainment", 28m, 2),
                        new PopulationNeedBlueprint("Barbering", 12m, 1),
                        new PopulationNeedBlueprint("Laundry Services", 12m, 1)
                    ]),
                new PopulationBlueprint(
                    "Standing Soldiery",
                    "Regular soldiers and sailors who keep demand high for arms, kit, medicines, preserved food, uniforms and durable practical goods.",
                    1200,
                    PopulationArchetype.Martial,
                    [
                        new PopulationNeedBlueprint("Standard Food", 182m, 6),
                        new PopulationNeedBlueprint("Salt", 16m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 30m, 4),
                        new PopulationNeedBlueprint("Military Uniforms", 76m, 5),
                        new PopulationNeedBlueprint("Weapons", 60m, 7),
                        new PopulationNeedBlueprint("Armour", 28m, 3),
                        new PopulationNeedBlueprint("Ammunition", 34m, 6),
                        new PopulationNeedBlueprint("Standard Clothing", 24m, 2),
                        new PopulationNeedBlueprint("Cheap Entertainment", 16m, 1),
                        new PopulationNeedBlueprint("Bathing Services", 10m, 1),
                        new PopulationNeedBlueprint("Standard Lodging", 14m, 1)
                    ]),
                new PopulationBlueprint(
                    "Parish Clergy",
                    "Parish clergy and learned church households who spend on respectable food, medicine, vesture, candles and modest comforts.",
                    520,
                    PopulationArchetype.Clergy,
                    [
                        new PopulationNeedBlueprint("Standard Food", 160m, 5),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 28m, 4),
                        new PopulationNeedBlueprint("Standard Clothing", 74m, 5),
                        new PopulationNeedBlueprint("Lighting", 28m, 4),
                        new PopulationNeedBlueprint("Standard Wares", 32m, 2),
                        new PopulationNeedBlueprint("Paper", 18m, 2),
                        new PopulationNeedBlueprint("Ink", 12m, 1),
                        new PopulationNeedBlueprint("Tea", 18m, 2),
                        new PopulationNeedBlueprint("Standard Decorations", 18m, 1),
                        new PopulationNeedBlueprint("Religious Goods", 22m, 2),
                        new PopulationNeedBlueprint("Printed News", 12m, 1)
                    ]),
                new PopulationBlueprint(
                    "Monastic Foundations",
                    "Monastic and charitable foundations with steady communal spending on staples, medicines, light and simple workshop supplies.",
                    390,
                    PopulationArchetype.Monastic,
                    [
                        new PopulationNeedBlueprint("Staple Food", 164m, 6),
                        new PopulationNeedBlueprint("Salt", 14m, 3),
                        new PopulationNeedBlueprint("Standard Medicine", 30m, 5),
                        new PopulationNeedBlueprint("Simple Clothing", 34m, 4),
                        new PopulationNeedBlueprint("Lighting", 28m, 4),
                        new PopulationNeedBlueprint("Simple Wares", 18m, 2),
                        new PopulationNeedBlueprint("Standard Tools", 26m, 2),
                        new PopulationNeedBlueprint("Paper", 14m, 1),
                        new PopulationNeedBlueprint("Ink", 10m, 1),
                        new PopulationNeedBlueprint("Simple Furniture", 16m, 1),
                        new PopulationNeedBlueprint("Religious Goods", 18m, 1),
                        new PopulationNeedBlueprint("Household Consumables", 10m, 1)
                    ]),
                new PopulationBlueprint(
                    "Gentry Elite",
                    "Gentry and courtly households with persistent demand for luxuries, seasonings, superior medicine, display goods and refined consumption.",
                    220,
                    PopulationArchetype.Elite,
                    [
                        new PopulationNeedBlueprint("Luxury Food", 286m, 8),
                        new PopulationNeedBlueprint("Spices", 42m, 4),
                        new PopulationNeedBlueprint("High-Quality Medicine", 52m, 4),
                        new PopulationNeedBlueprint("Luxury Clothing", 226m, 7),
                        new PopulationNeedBlueprint("Luxury Furniture", 164m, 4),
                        new PopulationNeedBlueprint("Luxury Decorations", 132m, 4),
                        new PopulationNeedBlueprint("Paper", 24m, 2),
                        new PopulationNeedBlueprint("Ink", 16m, 1),
                        new PopulationNeedBlueprint("Luxury Drinks", 68m, 2),
                        new PopulationNeedBlueprint("Transportation", 56m, 1),
                        new PopulationNeedBlueprint("Luxury Lodging", 52m, 2),
                        new PopulationNeedBlueprint("Luxury Entertainment", 58m, 2),
                        new PopulationNeedBlueprint("Domestic Services", 62m, 2),
                        new PopulationNeedBlueprint("Courier Services", 20m, 1),
                        new PopulationNeedBlueprint("Printed News", 18m, 1)
                    ])
            ])
    ];

    private static readonly IReadOnlyList<SectorInfluenceBlueprint> ExternalInfluenceBlueprints =
    [
        new(
            "Harvest Failure",
            ["Essentials"],
            -0.28,
            0.08,
            era => era switch
            {
                "Classical Age" => "Poor yields from estates and market gardens shrink everyday supply.",
                "Feudal Age" => "A poor harvest on demesne and village lands tightens everyday supply.",
                "Medieval Age" => "A failed harvest across the countryside tightens everyday supply.",
                _ => "A string of poor harvests reduces the flow of basic necessities."
            },
            era => $"{era} harvest losses squeeze the essentials trade.",
            [
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Rural, MultiplicativeIncomeImpact: 0.90m),
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Commoner, MultiplicativeIncomeImpact: 0.96m)
            ]),
        new(
            "Unseasonable Weather",
            ["Essentials"],
            -0.14,
            0.12,
            era => era switch
            {
                "Classical Age" => "Unseasonable storms and cold increase demand for food, fuel and basic care.",
                "Feudal Age" => "Untimely frosts and soaking rains lift demand for food, fuel and simple remedies.",
                "Medieval Age" => "Cold snaps and relentless rain lift demand for food, heating and simple care.",
                _ => "Wild seasonal swings increase demand for food, heating and common remedies."
            },
            era => $"{era} weather shocks push up essentials demand."),
        new(
            "River Trade Disruption",
            ["Essentials"],
            -0.18,
            0.05,
            era => era switch
            {
                "Classical Age" => "Blocked river traffic and delayed barges cut the easy flow of staple goods.",
                "Feudal Age" => "Flooded crossings and bandit-haunted roads delay staple deliveries.",
                "Medieval Age" => "Broken bridges and unsafe roads slow staple deliveries into town.",
                _ => "Transport delays slow the arrival of bulk essentials."
            },
            era => $"{era} transport trouble pinches basic supply.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Merchant, MultiplicativeIncomeImpact: 0.92m)]),
        new(
            "Bumper Harvest",
            ["Essentials"],
            0.30,
            -0.04,
            era => era switch
            {
                "Classical Age" => "Healthy crops and steady shipments flood the market with staples.",
                "Feudal Age" => "Strong yields across manor and village lands flood the market with staples.",
                "Medieval Age" => "Strong harvests fill granaries and make staples easy to find.",
                _ => "Excellent harvests make staples plentiful and comparatively cheap."
            },
            era => $"{era} bumper yields ease prices on essentials.",
            [
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Rural, MultiplicativeIncomeImpact: 1.10m),
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Commoner, MultiplicativeIncomeImpact: 1.04m)
            ]),
        new(
            "Caravan Surplus",
            ["Essentials"],
            0.18,
            -0.06,
            era => era switch
            {
                "Classical Age" => "A run of well-escorted caravans leaves storehouses unusually full.",
                "Feudal Age" => "Several laden caravans arrive close together, swelling staple stocks.",
                "Medieval Age" => "Merchants arrive in force and staple stocks pile up in town.",
                _ => "A glut of inbound carriers leaves storehouses unexpectedly full."
            },
            era => $"{era} trade arrivals create an essentials glut.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Merchant, MultiplicativeIncomeImpact: 1.06m)]),
        new(
            "Mild Season",
            ["Essentials"],
            0.08,
            -0.10,
            era => era switch
            {
                "Classical Age" => "Fair weather reduces the urgency around food preservation, fuel and simple remedies.",
                "Feudal Age" => "A mild season takes the edge off fuel and household necessity demand.",
                "Medieval Age" => "A mild season softens demand for fuel, food reserves and common remedies.",
                _ => "Easy weather softens immediate demand for everyday necessities."
            },
            era => $"{era} mild weather softens essentials demand."),
        new(
            "Festival Season",
            ["Lifestyle"],
            -0.06,
            0.18,
            era => era switch
            {
                "Classical Age" => "Games, feasts and civic festivals drive extra purchases of clothing, wares and drink.",
                "Feudal Age" => "Feasts and holy days lift demand for clothes, wares and drink.",
                "Medieval Age" => "Fair days, marriages and feast days boost demand for everyday luxuries.",
                _ => "A busy social season lifts demand for lifestyle goods and celebratory spending."
            },
            era => $"{era} festivities tighten lifestyle markets."),
        new(
            "Imported Fashion Shortfall",
            ["Lifestyle"],
            -0.16,
            0.08,
            era => era switch
            {
                "Classical Age" => "Fine dyes, perfumes and imported household goods arrive in reduced quantity.",
                "Feudal Age" => "Prestige cloth, spices and imported finery arrive in reduced quantity.",
                "Medieval Age" => "Fine cloth, ornaments and imported luxuries arrive in reduced quantity.",
                _ => "Fashionable imports and prestige goods arrive in noticeably smaller lots."
            },
            era => $"{era} luxury imports arrive short."),
        new(
            "Aristocratic Buying Frenzy",
            ["Lifestyle"],
            -0.04,
            0.22,
            era => era switch
            {
                "Classical Age" => "Elite patronage surges, chasing better food, drink, clothes and furnishings.",
                "Feudal Age" => "Courtly spending surges, chasing better drink, cloth and household display goods.",
                "Medieval Age" => "Noble and guild patronage surges, chasing fine cloth, drink and display goods.",
                _ => "Fashion-conscious elites increase their spending on visible comforts and luxuries."
            },
            era => $"{era} elite patronage lifts lifestyle demand."),
        new(
            "Merchant Overstock",
            ["Lifestyle"],
            0.22,
            -0.08,
            era => era switch
            {
                "Classical Age" => "Warehouses fill with unsold cloth, wares and drink after a speculative buying run.",
                "Feudal Age" => "Merchants are left long on cloth, wares and drink after overbuying.",
                "Medieval Age" => "Merchants are left with crowded stalls and too many middling luxuries.",
                _ => "Merchants overbought and now need to clear lifestyle stock."
            },
            era => $"{era} merchants are overstocked on lifestyle goods."),
        new(
            "Quiet Social Season",
            ["Lifestyle"],
            0.08,
            -0.16,
            era => era switch
            {
                "Classical Age" => "A lull in social events reduces spending on drink, clothing and household display.",
                "Feudal Age" => "A quiet season reduces spending on better drink, clothing and display goods.",
                "Medieval Age" => "A quieter social calendar reduces discretionary spending on lifestyle goods.",
                _ => "A quiet season cools discretionary spending on social and lifestyle goods."
            },
            era => $"{era} quieter social life eases lifestyle demand."),
        new(
            "Foreign Luxury Influx",
            ["Lifestyle"],
            0.18,
            -0.05,
            era => era switch
            {
                "Classical Age" => "A convoy of imported luxuries arrives all at once, swelling fashionable supply.",
                "Feudal Age" => "A rare but rich train of imported luxuries swells fashionable supply.",
                "Medieval Age" => "Foreign luxuries arrive in abundance and undercut recent prices.",
                _ => "Imported prestige goods arrive in unusual volume and cool recent prices."
            },
            era => $"{era} imported luxuries swell the lifestyle market."),
        new(
            "Local War",
            ["Martial"],
            -0.08,
            0.34,
            era => era switch
            {
                "Classical Age" => "A local campaign increases demand for arms, armour and military supplies.",
                "Feudal Age" => "Mustered levies increase demand for arms, armour and military supplies.",
                "Medieval Age" => "A local war sharply increases demand for arms, armour and military stores.",
                _ => "A war scare sharply increases demand for weapons, ammunition and military stores."
            },
            era => $"{era} conflict drives up martial demand.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Martial, MultiplicativeIncomeImpact: 1.12m)]),
        new(
            "Border Raiding",
            ["Martial"],
            -0.04,
            0.20,
            era => era switch
            {
                "Classical Age" => "Repeated raids keep local buyers in the market for arms and armour.",
                "Feudal Age" => "Repeated raiding keeps local buyers in the market for arms and armour.",
                "Medieval Age" => "Frontier raiding keeps households and garrisons buying military goods.",
                _ => "Insecurity on the frontier keeps buyers focused on weapons and powder."
            },
            era => $"{era} raiding keeps martial demand elevated."),
        new(
            "Arms Embargo",
            ["Martial"],
            -0.22,
            0.06,
            era => era switch
            {
                "Classical Age" => "Regional restrictions sharply reduce the inflow of arms and armour.",
                "Feudal Age" => "Restrictions on armourers and weapon traders sharply reduce inflow.",
                "Medieval Age" => "Restrictions on armourers and merchants sharply reduce martial supply.",
                _ => "Embargoes and inspections sharply reduce the inflow of martial goods."
            },
            era => $"{era} restrictions choke off martial supply."),
        new(
            "Peace Dividend",
            ["Martial"],
            0.20,
            -0.22,
            era => era switch
            {
                "Classical Age" => "An easing security situation leaves arsenals and smithies with slack demand.",
                "Feudal Age" => "The end of campaigning leaves armourers and fletchers with slack demand.",
                "Medieval Age" => "An easing security situation leaves martial workshops with slack demand.",
                _ => "A calmer political climate leaves martial workshops with slack demand."
            },
            era => $"{era} peace eases martial prices."),
        new(
            "Armoury Surplus",
            ["Martial"],
            0.24,
            -0.06,
            era => era switch
            {
                "Classical Age" => "Stockpiles of arms and armour are released onto the market.",
                "Feudal Age" => "Stored arms and armour are released onto the market.",
                "Medieval Age" => "Old stock from armouries and workshops floods the market.",
                _ => "Stored arms and powder hit the market in unusual volume."
            },
            era => $"{era} surplus military stock cools prices."),
        new(
            "Disbanded Levies",
            ["Martial"],
            0.12,
            -0.14,
            era => era switch
            {
                "Classical Age" => "Demobilised troops spend less on military upkeep and sell off equipment.",
                "Feudal Age" => "Disbanded levies spend less on military upkeep and sell off equipment.",
                "Medieval Age" => "Demobilised troops reduce martial demand and release kit to market.",
                _ => "Demobilisation reduces martial demand and frees up spare kit."
            },
            era => $"{era} demobilisation cools martial demand.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Martial, MultiplicativeIncomeImpact: 0.85m)]),
        new(
            "Road And Port Trouble",
            ["Logistics"],
            -0.18,
            0.08,
            era => era switch
            {
                "Classical Age" => "Silted quays and unsafe roads strain freight and storage capacity.",
                "Feudal Age" => "Unsafe roads and poor crossings strain haulage and storage capacity.",
                "Medieval Age" => "Broken bridges and port delays strain haulage and warehousing.",
                _ => "Port delays and road trouble strain freight and storage capacity."
            },
            era => $"{era} transport trouble tightens logistics markets."),
        new(
            "Piracy And Banditry",
            ["Logistics"],
            -0.12,
            0.12,
            era => era switch
            {
                "Classical Age" => "Piracy and brigandage make carrying goods noticeably more costly.",
                "Feudal Age" => "Banditry and petty piracy make carrying goods noticeably more costly.",
                "Medieval Age" => "Banditry and piracy make moving and storing goods more expensive.",
                _ => "Predation on the roads and sea makes moving and storing goods more expensive."
            },
            era => $"{era} insecurity lifts logistics demand and costs."),
        new(
            "Forced Requisitioning",
            ["Logistics"],
            -0.10,
            0.16,
            era => era switch
            {
                "Classical Age" => "Authorities commandeer carriers and storage for official use.",
                "Feudal Age" => "Lords commandeer carriers and storage for official use.",
                "Medieval Age" => "Authorities commandeer carriers and storehouses for urgent use.",
                _ => "Officials requisition wagons, ships and storage for urgent use."
            },
            era => $"{era} requisitioning strains logistics capacity."),
        new(
            "Clear Roads",
            ["Logistics"],
            0.18,
            -0.06,
            era => era switch
            {
                "Classical Age" => "Safe roads and reliable schedules increase carrying capacity.",
                "Feudal Age" => "Safe roads and open crossings increase carrying capacity.",
                "Medieval Age" => "Reliable roads and ports increase carrying capacity.",
                _ => "Open roads and efficient ports increase carrying capacity."
            },
            era => $"{era} safer routes ease logistics prices."),
        new(
            "Harbour Boom",
            ["Logistics"],
            0.14,
            -0.10,
            era => era switch
            {
                "Classical Age" => "A busy season of successful shipping leaves hauliers and storehouses chasing work.",
                "Feudal Age" => "A busy shipping season leaves carriers and storehouses chasing work.",
                "Medieval Age" => "Strong throughput leaves carriers and warehousing with spare capacity.",
                _ => "Strong port throughput leaves freight and warehousing with spare capacity."
            },
            era => $"{era} shipping success expands logistics supply."),
        new(
            "Idle Carrier Fleets",
            ["Logistics"],
            0.12,
            -0.14,
            era => era switch
            {
                "Classical Age" => "Too many carriers compete for too little trade.",
                "Feudal Age" => "Too many carriers compete for too little trade.",
                "Medieval Age" => "Too many carriers compete for too little custom.",
                _ => "Carrier capacity outstrips demand and prices soften."
            },
            era => $"{era} excess carrier capacity cools logistics demand."),
        new(
            "Mining Strike",
            ["Industry"],
            -0.24,
            0.06,
            era => era switch
            {
                "Classical Age" => "Disruption at mines and foundries reduces the supply of useful materials and tools.",
                "Feudal Age" => "Disruption at pits and smithies reduces the supply of useful materials and tools.",
                "Medieval Age" => "Disruption at pits and workshops reduces the supply of useful materials and tools.",
                _ => "Disruption at mines and workshops reduces the supply of materials and productive tools."
            },
            era => $"{era} industrial disruption tightens materials supply."),
        new(
            "Great Building Works",
            ["Industry"],
            -0.08,
            0.22,
            era => era switch
            {
                "Classical Age" => "Ambitious public works lift demand for stone, metal, timber and tools.",
                "Feudal Age" => "A wave of castle, bridge and hall works lifts demand for material and tools.",
                "Medieval Age" => "A wave of major building works lifts demand for material and tools.",
                _ => "Major construction and expansion works lift demand for material and tools."
            },
            era => $"{era} building works drive industrial demand."),
        new(
            "Guild Input Scarcity",
            ["Industry"],
            -0.14,
            0.10,
            era => era switch
            {
                "Classical Age" => "Key workshop inputs become scarce and craftsmen bid harder for supply.",
                "Feudal Age" => "Key workshop inputs become scarce and craftsmen bid harder for supply.",
                "Medieval Age" => "Guild workshops bid harder for scarce raw inputs.",
                _ => "Manufacturers bid harder for scarce raw inputs."
            },
            era => $"{era} workshop input shortages raise industrial prices."),
        new(
            "Rich Vein Discovery",
            ["Industry"],
            0.24,
            -0.06,
            era => era switch
            {
                "Classical Age" => "A rich source of timber, ore or clay swells industrial supply.",
                "Feudal Age" => "A rich source of timber, ore or clay swells industrial supply.",
                "Medieval Age" => "A rich source of timber, ore or clay swells industrial supply.",
                _ => "A rich source of timber, ore or mineral swells industrial supply."
            },
            era => $"{era} new extraction successes swell material supply."),
        new(
            "Guild Overproduction",
            ["Industry"],
            0.16,
            -0.10,
            era => era switch
            {
                "Classical Age" => "Workshops overshoot demand and tool and material prices soften.",
                "Feudal Age" => "Workshops overshoot demand and tool and material prices soften.",
                "Medieval Age" => "Guild workshops overshoot demand and prices soften.",
                _ => "Manufacturers overshoot demand and prices soften."
            },
            era => $"{era} overproduction cools industrial demand."),
        new(
            "Forest Bounty",
            ["Industry"],
            0.14,
            -0.08,
            era => era switch
            {
                "Classical Age" => "Abundant timber and charcoal leave workshops with easier access to inputs.",
                "Feudal Age" => "Abundant timber and charcoal leave workshops with easier access to inputs.",
                "Medieval Age" => "Abundant timber and fuel leave workshops with easier access to inputs.",
                _ => "Abundant timber and fuel leave workshops with easier access to inputs."
            },
            era => $"{era} abundant inputs ease industrial prices."),
        new(
            "Grain Blight",
            ["Essentials"],
            -0.20,
            0.10,
            era => $"Crop disease and spoilage spread through {era.ToLowerInvariant()} staple production and leave granaries short.",
            era => $"{era} crop blight tightens essentials supply."),
        new(
            "Livestock Murrain",
            ["Essentials"],
            -0.18,
            0.09,
            era => $"Animal disease reduces herds and preserved stores across the {era.ToLowerInvariant()} market hinterland.",
            era => $"{era} herd losses tighten food supply."),
        new(
            "Relief Convoys",
            ["Essentials"],
            0.22,
            -0.08,
            era => $"Emergency grain, fuel and medicine shipments arrive in force and stabilise the {era.ToLowerInvariant()} essentials trade.",
            era => $"{era} relief shipments swell basic supply."),
        new(
            "Preservation Windfall",
            ["Essentials"],
            0.16,
            -0.06,
            era => $"Exceptional salting, drying and storage results leave the {era.ToLowerInvariant()} market unusually well provisioned.",
            era => $"{era} preservation success eases essentials demand."),
        new(
            "Fishing Bonanza",
            ["Essentials"],
            0.12,
            -0.04,
            era => $"Strong catches and easy curing add cheap protein and tradeable food to the {era.ToLowerInvariant()} market.",
            era => $"{era} fishing success broadens staple supply."),
        new(
            "Seed Shortage",
            ["Essentials"],
            -0.12,
            0.07,
            era => $"Weak planting stock and hoarded seed threaten the next cycle of {era.ToLowerInvariant()} staple production.",
            era => $"{era} seed shortages unsettle the essentials market."),
        new(
            "Pilgrimage Season",
            ["Lifestyle"],
            -0.05,
            0.16,
            era => $"Travellers, pilgrims and guests pour through the {era.ToLowerInvariant()} region and lift spending on comfort, drink and services.",
            era => $"{era} pilgrimage traffic boosts lifestyle demand."),
        new(
            "Sumptuary Crackdown",
            ["Lifestyle"],
            0.08,
            -0.18,
            era => $"Authorities clamp down on display spending and status consumption across the {era.ToLowerInvariant()} market.",
            era => $"{era} moral regulation cools lifestyle demand."),
        new(
            "Wedding Season",
            ["Lifestyle"],
            -0.05,
            0.17,
            era => $"Marriage feasts and gift-giving keep the {era.ToLowerInvariant()} market busy with clothing, wares and celebratory spending.",
            era => $"{era} wedding demand lifts lifestyle prices."),
        new(
            "Theatre Craze",
            ["Lifestyle"],
            -0.06,
            0.18,
            era => $"A burst of shows, performances and fashionable venues lifts the {era.ToLowerInvariant()} appetite for amusements and display goods.",
            era => $"{era} performance mania tightens lifestyle supply."),
        new(
            "Artisan Patronage",
            ["Lifestyle"],
            -0.08,
            0.14,
            era => $"Patrons commission household display goods, clothing and services more aggressively across the {era.ToLowerInvariant()} market.",
            era => $"{era} patronage raises lifestyle demand."),
        new(
            "Tavern Closures",
            ["Lifestyle"],
            0.10,
            -0.15,
            era => $"Licensing pressure and curfews quieten the taverns and reduce discretionary spending in the {era.ToLowerInvariant()} market.",
            era => $"{era} tavern restrictions cool lifestyle demand."),
        new(
            "Mercenary Arrival",
            ["Martial"],
            0.14,
            -0.10,
            era => $"Mercenaries and discharged fighters flood the {era.ToLowerInvariant()} market with serviceable kit and slacken military demand.",
            era => $"{era} mercenary surplus cools martial prices."),
        new(
            "Siege Preparations",
            ["Martial"],
            -0.10,
            0.26,
            era => $"Fortification works and siege planning sharply increase the {era.ToLowerInvariant()} demand for arms, armour and stores.",
            era => $"{era} siege fears drive martial demand."),
        new(
            "Powder Shortage",
            ["Martial"],
            -0.20,
            0.08,
            era => $"Explosive ingredients, bow-stock inputs or other martial consumables become scarce in the {era.ToLowerInvariant()} market.",
            era => $"{era} war-stock shortages choke martial supply."),
        new(
            "Veteran Sell-Off",
            ["Martial"],
            0.18,
            -0.12,
            era => $"Retired soldiers and guards sell spare equipment into the {era.ToLowerInvariant()} market and ease recent shortages.",
            era => $"{era} veterans dump martial surplus on the market."),
        new(
            "Smithy Subsidy",
            ["Martial"],
            0.16,
            -0.06,
            era => $"Authorities subsidise armourers and fletchers, improving the flow of military goods in the {era.ToLowerInvariant()} market.",
            era => $"{era} official subsidies expand martial supply.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Merchant, MultiplicativeIncomeImpact: 1.05m)]),
        new(
            "Muster Exhaustion",
            ["Martial"],
            0.08,
            -0.16,
            era => $"Campaign fatigue and empty musters reduce the appetite for replacement kit across the {era.ToLowerInvariant()} market.",
            era => $"{era} exhausted musters cool martial demand."),
        new(
            "Wagon Shortage",
            ["Logistics"],
            -0.16,
            0.09,
            era => $"Broken draft stock, bad roads or lost carts leave the {era.ToLowerInvariant()} freight market short of hauling capacity.",
            era => $"{era} wagon shortages tighten logistics supply."),
        new(
            "Customs Crackdown",
            ["Logistics"],
            -0.12,
            0.14,
            era => $"Inspections and new tolls slow crossings and push more traders to compete for compliant carriers in the {era.ToLowerInvariant()} market.",
            era => $"{era} customs pressure strains logistics."),
        new(
            "New Caravan Route",
            ["Logistics"],
            0.20,
            -0.10,
            era => $"A safer and faster route opens up and expands freight and warehousing capacity in the {era.ToLowerInvariant()} market.",
            era => $"{era} new routes expand logistics supply."),
        new(
            "River Dredging",
            ["Logistics"],
            0.16,
            -0.08,
            era => $"Cleared channels, repaired docks or improved handling increase throughput across the {era.ToLowerInvariant()} trade network.",
            era => $"{era} transport improvements cool logistics demand."),
        new(
            "Warehouse Fire",
            ["Logistics"],
            -0.14,
            0.10,
            era => $"A major storage fire removes space and handling capacity from the {era.ToLowerInvariant()} market.",
            era => $"{era} warehouse losses tighten logistics prices."),
        new(
            "Idle Porters",
            ["Logistics"],
            0.10,
            -0.12,
            era => $"Labour and carrying capacity outrun available work in the {era.ToLowerInvariant()} transport market.",
            era => $"{era} spare handling capacity cools logistics demand."),
        new(
            "Smelter Fire",
            ["Industry"],
            -0.18,
            0.08,
            era => $"Furnaces, kilns or foundries go dark and reduce the flow of productive materials in the {era.ToLowerInvariant()} market.",
            era => $"{era} workshop fires tighten industrial supply."),
        new(
            "Tooling Breakthrough",
            ["Industry"],
            0.18,
            -0.08,
            era => $"Improved processes and better tooling raise workshop output across the {era.ToLowerInvariant()} economy.",
            era => $"{era} process gains expand industrial supply."),
        new(
            "Timber Rights Dispute",
            ["Industry"],
            -0.14,
            0.06,
            era => $"Access fights over timber, charcoal or cutting rights leave the {era.ToLowerInvariant()} workshop sector short of key inputs.",
            era => $"{era} timber disputes pinch industrial supply."),
        new(
            "Quarry Expansion",
            ["Industry"],
            0.20,
            -0.06,
            era => $"New pits, quarries or extraction rights swell the material base available to the {era.ToLowerInvariant()} market.",
            era => $"{era} extraction growth expands industrial supply."),
        new(
            "Apprenticeship Surge",
            ["Industry"],
            0.12,
            -0.10,
            era => $"An influx of apprentices and labour improves workshop output and softens industrial scarcity in the {era.ToLowerInvariant()} market.",
            era => $"{era} labour growth cools industrial demand."),
        new(
            "Craft Boycott",
            ["Industry"],
            -0.12,
            0.12,
            era => $"Coordinated refusals, guild discipline or labour unrest reduce finished output in the {era.ToLowerInvariant()} workshop economy.",
            era => $"{era} labour unrest strains industrial prices.")
    ];

    private static readonly IReadOnlyList<IncomeInfluenceBlueprint> IncomeInfluenceBlueprints =
    [
        new(
            "Rural Wage Squeeze",
            era => $"Weak harvest rents, poor seasonal hiring and tightening village credit cut rural incomes across the {era.ToLowerInvariant()} economy.",
            era => $"{era} rural earnings contract sharply.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Rural, MultiplicativeIncomeImpact: 0.90m)]),
        new(
            "Bountiful Hiring Season",
            era => $"Strong labour demand for farms, transport and related support work lifts ordinary household earnings in the {era.ToLowerInvariant()} market.",
            era => $"{era} hiring demand lifts common incomes.",
            [
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Commoner, MultiplicativeIncomeImpact: 1.08m),
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Rural, MultiplicativeIncomeImpact: 1.12m)
            ]),
        new(
            "Merchant Credit Crunch",
            era => $"Tighter lending, slower settlements and failing counterparties squeeze commercial incomes in the {era.ToLowerInvariant()} economy.",
            era => $"{era} merchants are squeezed by tight credit.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Merchant, MultiplicativeIncomeImpact: 0.88m)]),
        new(
            "Trade Windfall",
            era => $"A run of profitable voyages, fairs or contracts lifts mercantile incomes across the {era.ToLowerInvariant()} market.",
            era => $"{era} trade profits swell merchant incomes.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Merchant, MultiplicativeIncomeImpact: 1.12m)]),
        new(
            "Garrison Muster",
            era => $"Fresh musters and wartime provisioning put more coin into martial households across the {era.ToLowerInvariant()} economy.",
            era => $"{era} musters raise martial incomes.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Martial, MultiplicativeIncomeImpact: 1.15m)]),
        new(
            "Demobilisation Glut",
            era => $"Campaigns end, retainers are dismissed and military wages soften across the {era.ToLowerInvariant()} market.",
            era => $"{era} demobilisation cuts martial incomes.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Martial, MultiplicativeIncomeImpact: 0.85m)]),
        new(
            "Tithe Boom",
            era => $"Strong collections and generous patronage improve clerical incomes in the {era.ToLowerInvariant()} economy.",
            era => $"{era} tithe receipts lift clerical incomes.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Clergy, MultiplicativeIncomeImpact: 1.10m)]),
        new(
            "Alms Shortfall",
            era => $"Weaker donations and poorer lay support reduce religious-house incomes across the {era.ToLowerInvariant()} market.",
            era => $"{era} weak alms reduce religious incomes.",
            [
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Clergy, MultiplicativeIncomeImpact: 0.90m),
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Monastic, MultiplicativeIncomeImpact: 0.88m)
            ]),
        new(
            "Noble Rent Increase",
            era => $"Higher dues, rents and seigneurial claims improve elite household incomes across the {era.ToLowerInvariant()} economy.",
            era => $"{era} rising rents enrich elite households.",
            [new PopulationIncomeImpactBlueprint(PopulationArchetype.Elite, MultiplicativeIncomeImpact: 1.12m)]),
        new(
            "Patronage Windfall",
            era => $"Court favour, civic contracts and wealthy commissions improve privileged incomes in the {era.ToLowerInvariant()} market.",
            era => $"{era} patronage spreads new income among favoured households.",
            [
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Elite, MultiplicativeIncomeImpact: 1.08m),
                new PopulationIncomeImpactBlueprint(PopulationArchetype.Merchant, MultiplicativeIncomeImpact: 1.06m)
            ])
    ];

    private static readonly IReadOnlyList<StressLevelDefinition> StressLevels =
    [
        new("Strained", 0.12m, 0.05, -0.18, -0.08, -0.06, 0.05, 0.04, -0.04),
        new("Suffering Hardship", 0.28m, 0.10, -0.40, -0.18, -0.16, 0.10, 0.08, -0.10),
        new("In Crisis", 0.50m, 0.18, -0.72, -0.30, -0.24, 0.16, 0.14, -0.18)
    ];

    private static readonly IReadOnlyDictionary<string, string> FamilySectorMap =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Nourishment"] = "Essentials",
            ["Domestic Heating"] = "Essentials",
            ["Lighting"] = "Essentials",
            ["Medicine"] = "Essentials",
            ["Writing Materials"] = "Lifestyle",
            ["Clothing"] = "Lifestyle",
            ["Intoxicants"] = "Lifestyle",
            ["Luxury Drinks"] = "Lifestyle",
            ["Household Goods"] = "Lifestyle",
            ["Hospitality"] = "Lifestyle",
            ["Entertainment"] = "Lifestyle",
            ["Personal Services"] = "Lifestyle",
            ["Religious Goods"] = "Lifestyle",
            ["Household Consumables"] = "Lifestyle",
            ["Military Goods"] = "Martial",
            ["Transportation"] = "Logistics",
            ["Warehousing"] = "Logistics",
            ["Communications"] = "Logistics",
            ["Professional Tools"] = "Industry",
            ["Raw Materials"] = "Industry",
            ["Construction Materials"] = "Industry"
        };

    private static readonly IReadOnlyDictionary<string, (double Under, double Over)> FamilyElasticityMap =
        new Dictionary<string, (double Under, double Over)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Nourishment"] = (0.38, 0.34),
            ["Domestic Heating"] = (0.48, 0.42),
            ["Lighting"] = (0.46, 0.40),
            ["Medicine"] = (0.62, 0.52),
            ["Writing Materials"] = (0.68, 0.60),
            ["Clothing"] = (0.58, 0.52),
            ["Intoxicants"] = (0.70, 0.62),
            ["Luxury Drinks"] = (0.78, 0.72),
            ["Household Goods"] = (0.64, 0.58),
            ["Hospitality"] = (0.72, 0.66),
            ["Entertainment"] = (0.82, 0.76),
            ["Personal Services"] = (0.74, 0.68),
            ["Communications"] = (0.60, 0.54),
            ["Religious Goods"] = (0.58, 0.52),
            ["Household Consumables"] = (0.50, 0.46),
            ["Military Goods"] = (0.88, 0.82),
            ["Transportation"] = (0.66, 0.58),
            ["Warehousing"] = (0.44, 0.40),
            ["Professional Tools"] = (0.62, 0.58),
            ["Raw Materials"] = (0.56, 0.52),
            ["Construction Materials"] = (0.58, 0.54)
        };

    public bool SafeToRunMoreThanOnce => true;

    public IEnumerable<(string Id, string Question,
        Func<FuturemudDatabaseContext, IReadOnlyDictionary<string, string>, bool> Filter,
        Func<string, FuturemudDatabaseContext, (bool Success, string error)> Validator)> SeederQuestions =>
    [
        ("era",
            @"Which stock economy era package do you want to install?

#BClassical Age#F
#BFeudal Age#F
#BMedieval Age#F
#BEarly Modern Age#F

Please make your choice: ",
            (context, _) => true,
            (answer, _) => EraDefinitions.Any(x => x.DisplayName.EqualTo(answer)) ? (true, string.Empty) : (false, "You must choose one of the listed era options.")),
        ("currency",
            "Which currency should this stock economy zone use? You may enter either the currency ID or name.",
            (context, _) => context.Currencies.Count() > 1,
            ValidateCurrencyChoice),
        ("zone",
            "Which physical zone should supply the clock/calendar bindings for this stock economy zone? You may enter either the zone ID or name.",
            (context, _) => context.Zones.Count() > 1,
            ValidateZoneChoice),
        ("shopper-scale",
            @"How much money should the stock shoppers spend each cycle?

#BLow#F
#BStandard#F
#BHigh#F

Please make your choice: ",
            (context, _) => true,
            (answer, _) => answer.EqualToAny("low", "standard", "high") ? (true, string.Empty) : (false, "You must choose Low, Standard or High."))
    ];

    public int SortOrder => 25;
    public string Name => "Economy";
    public string Tagline => "Installs a stock economic zone shell, market categories, influences, populations and shoppers";
    public string FullDescription =>
        @"This package installs a builder-friendly economy template for one historical era at a time. It creates a stock economic zone shell, a market, market categories tied to the UsefulSeeder market tags, a library of reusable market influence templates, era-specific market populations, and matching virtual shoppers.

It is intended to be additive across eras and safe to rerun to restore or refresh missing stock-owned economy records.";

    public string SeedData(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> questionAnswers)
    {
        context.Database.BeginTransaction();

        EraDefinition era = ResolveEra(questionAnswers);
        Currency currency = ResolveCurrency(context, questionAnswers);
        Zone zone = ResolveZone(context, questionAnswers);
        decimal shopperScale = ResolveShopperScale(questionAnswers);

        Tag marketRoot = context.Tags.First(x => x.Name == MarketRootTagName);
        List<Tag> marketTags = GetMarketDescendantTags(context, marketRoot).ToList();
        List<string> missingRequiredTags = GetMissingRequiredMarketTags(marketTags.Select(x => x.Name));
        if (missingRequiredTags.Any())
        {
            throw new InvalidOperationException(
                $"UsefulSeeder market tags are incomplete. EconomySeeder requires these tags beneath the {MarketRootTagName.ColourName()} root: {missingRequiredTags.ListToString()}.");
        }

        Dictionary<long, MarketCategory> categoriesByTagId = EnsureMarketCategories(context, marketTags);
        SupportProgSet supportProgs = EnsureSupportProgs(context);
        context.SaveChanges();

        EconomicZone economicZone = EnsureEconomicZone(context, era, zone, currency);
        context.SaveChanges();

        Market market = EnsureMarket(context, era, economicZone, categoriesByTagId.Values);
        context.SaveChanges();

        CategoryContext categoryContext = BuildCategoryContext(context, marketRoot, marketTags, categoriesByTagId);
        PopulationContext populationContext = EnsurePopulationsAndStressTemplates(
            context,
            era,
            currency,
            market,
            categoryContext,
            supportProgs.AlwaysKnownProg);
        context.SaveChanges();

        EnsureExternalInfluenceTemplates(context, era, categoryContext, supportProgs.AlwaysKnownProg, populationContext);
        EnsureCategoryAdjustmentTemplates(context, era, categoryContext, supportProgs.AlwaysKnownProg);
        EnsureIncomeInfluenceTemplates(context, era, supportProgs.AlwaysKnownProg, populationContext);
        context.SaveChanges();

        EnsureShoppers(
            context,
            era,
            economicZone,
            populationContext,
            shopperScale,
            supportProgs);
        context.SaveChanges();

        context.Database.CommitTransaction();
        return "The operation completed successfully.";
    }

    public ShouldSeedResult ShouldSeedData(FuturemudDatabaseContext context)
    {
        if (!context.Accounts.Any() ||
            !context.Currencies.Any() ||
            !context.Zones.Any() ||
            !context.Clocks.Any() ||
            !context.Calendars.Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        if (!context.Tags.Any(x => x.Name == MarketRootTagName) ||
            !context.Tags.Any(x => x.Parent != null && x.Parent.Name == MarketRootTagName))
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        Tag marketRoot = context.Tags.First(x => x.Name == MarketRootTagName);
        List<Tag> descendantTags = GetMarketDescendantTags(context, marketRoot).ToList();
        if (GetMissingRequiredMarketTags(descendantTags.Select(x => x.Name)).Any())
        {
            return ShouldSeedResult.PrerequisitesNotMet;
        }

        int installedEraCount = EraDefinitions.Count(era =>
            context.EconomicZones.Any(x => x.Name == EconomicZoneName(era)) &&
            context.Markets.Any(x => x.Name == MarketName(era)));

        bool anyStockInstalled = installedEraCount > 0 ||
                                context.MarketInfluenceTemplates
                                .AsEnumerable()
                                .Any(x => x.Name.StartsWith($"{HelperProgPrefix} ", StringComparison.OrdinalIgnoreCase));
        if (!anyStockInstalled)
        {
            return ShouldSeedResult.ReadyToInstall;
        }

        int descendantCount = descendantTags.Count;
        int seededCategoryCount = context.MarketCategories.AsEnumerable().Count(x => x.Description.StartsWith(CategoryPrefix));
        int expectedExternalTemplateCount = installedEraCount * ExternalInfluenceBlueprints.Count;

        bool hasAllSharedCategories = seededCategoryCount >= descendantCount;
        bool hasAllEraPackages = installedEraCount == EraDefinitions.Count;
        bool hasExternalTemplates = context.MarketInfluenceTemplates.AsEnumerable().Count(x =>
            x.Name.StartsWith($"{HelperProgPrefix} External ", StringComparison.OrdinalIgnoreCase)) >= expectedExternalTemplateCount;

        return hasAllSharedCategories && hasAllEraPackages && hasExternalTemplates
            ? ShouldSeedResult.MayAlreadyBeInstalled
            : ShouldSeedResult.ExtraPackagesAvailable;
    }

    private static (bool Success, string error) ValidateCurrencyChoice(string answer, FuturemudDatabaseContext context)
    {
        return ResolveCurrencyOrNull(context, answer) is not null
            ? (true, string.Empty)
            : (false, "That is not a valid currency ID or name.");
    }

    private static (bool Success, string error) ValidateZoneChoice(string answer, FuturemudDatabaseContext context)
    {
        return ResolveZoneOrNull(context, answer) is not null
            ? (true, string.Empty)
            : (false, "That is not a valid zone ID or name.");
    }

    private static EraDefinition ResolveEra(IReadOnlyDictionary<string, string> answers)
    {
        string answer = answers["era"];
        return EraDefinitions.First(x => x.DisplayName.EqualTo(answer));
    }

    private static Currency ResolveCurrency(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers)
    {
        return answers.TryGetValue("currency", out string? text)
            ? ResolveCurrencyOrNull(context, text) ?? context.Currencies.OrderBy(x => x.Id).First()
            : context.Currencies.OrderBy(x => x.Id).First();
    }

    private static Currency? ResolveCurrencyOrNull(FuturemudDatabaseContext context, string answer)
    {
        return long.TryParse(answer, out long value)
            ? context.Currencies.FirstOrDefault(x => x.Id == value)
            : context.Currencies.AsEnumerable()
                .FirstOrDefault(x => x.Name.Equals(answer, StringComparison.OrdinalIgnoreCase));
    }

    private static Zone ResolveZone(FuturemudDatabaseContext context, IReadOnlyDictionary<string, string> answers)
    {
        return answers.TryGetValue("zone", out string? text)
            ? ResolveZoneOrNull(context, text) ?? context.Zones.OrderBy(x => x.Id).First()
            : context.Zones.OrderBy(x => x.Id).First();
    }

    private static Zone? ResolveZoneOrNull(FuturemudDatabaseContext context, string answer)
    {
        return long.TryParse(answer, out long value)
            ? context.Zones.FirstOrDefault(x => x.Id == value)
            : context.Zones.AsEnumerable()
                .FirstOrDefault(x => x.Name.Equals(answer, StringComparison.OrdinalIgnoreCase));
    }

    private static decimal ResolveShopperScale(IReadOnlyDictionary<string, string> answers)
    {
        return answers["shopper-scale"].ToLowerInvariant() switch
        {
            "low" => 0.75m,
            "high" => 1.50m,
            _ => 1.00m
        };
    }

    private static IEnumerable<Tag> GetMarketDescendantTags(FuturemudDatabaseContext context, Tag marketRoot)
    {
        List<Tag> tags = context.Tags.ToList();
        Dictionary<long, List<Tag>> byParent = tags
            .Where(x => x.ParentId.HasValue)
            .GroupBy(x => x.ParentId!.Value)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Name).ToList());
        Queue<Tag> queue = new(byParent.TryGetValue(marketRoot.Id, out List<Tag>? firstLevel) ? firstLevel : []);
        while (queue.Count > 0)
        {
            Tag current = queue.Dequeue();
            yield return current;
            if (!byParent.TryGetValue(current.Id, out List<Tag>? children))
            {
                continue;
            }

            foreach (Tag? child in children)
            {
                queue.Enqueue(child);
            }
        }
    }

    private static List<string> GetMissingRequiredMarketTags(IEnumerable<string> availableTagNames)
    {
        HashSet<string> availableNames = new(availableTagNames, StringComparer.OrdinalIgnoreCase);
        return RequiredMarketTagNames
            .Where(x => !availableNames.Contains(x))
            .OrderBy(x => x)
            .ToList();
    }

    private static Dictionary<long, MarketCategory> EnsureMarketCategories(FuturemudDatabaseContext context, IEnumerable<Tag> tags)
    {
        List<Tag> allTags = context.Tags.ToList();
        List<Tag> marketTags = tags.OrderBy(x => x.Name).ToList();
        Dictionary<long, List<Tag>> childTagsByParentId = BuildChildTagMap(marketTags);
        Dictionary<long, MarketCategory> result = new();
        foreach (Tag? tag in marketTags)
        {
            string familyName = ResolveTopFamilyName(tag, allTags);
            (double Under, double Over) elasticity = FamilyElasticityMap.TryGetValue(familyName, out (double Under, double Over) value)
                ? value
                : (Under: 0.60, Over: 0.55);
            MarketCategory category = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.MarketCategories,
                tag.Name,
                x => x.Name,
                () =>
                {
                    MarketCategory created = new();
                    context.MarketCategories.Add(created);
                    return created;
                });

            category.Name = tag.Name;
            category.Description = $"{CategoryPrefix}: items carrying the {tag.Name} market tag.";
            category.ElasticityFactorBelow = elasticity.Under;
            category.ElasticityFactorAbove = elasticity.Over;
            category.MarketCategoryType = 0;
            category.Tags = new XElement("Tags", new XElement("Tag", tag.Id)).ToString();
            category.CombinationCategories = new XElement("Components").ToString();
            result[tag.Id] = category;
        }

        foreach (Tag tag in marketTags)
        {
            if (!ShouldSeedCombinationCategory(tag, childTagsByParentId))
            {
                continue;
            }

            List<MarketCategory> componentCategories = childTagsByParentId[tag.Id]
                .Where(x => result.ContainsKey(x.Id))
                .Select(x => result[x.Id])
                .OrderBy(x => x.Name)
                .ToList();
            if (componentCategories.Count < 2)
            {
                continue;
            }

			IReadOnlyList<(MarketCategory Category, decimal Weight)> weightedComponents =
				GetStockCombinationComponents(tag.Name, componentCategories);

			MarketCategory category = result[tag.Id];
			category.Description =
				$"{CategoryPrefix}: seeded aggregate {tag.Name} basket priced as a weighted combination of {DescribeCombinationComponents(weightedComponents)}.";
			category.MarketCategoryType = 1;
			category.CombinationCategories = SaveCombinationCategories(weightedComponents);
		}

        return result;
    }

    private static SupportProgSet EnsureSupportProgs(FuturemudDatabaseContext context)
    {
        FutureProg alwaysKnownProg = SeederRepeatabilityHelper.EnsureProg(
            context,
            $"{HelperProgPrefix}AlwaysKnown",
            "Economy",
            "Seeder",
            ProgVariableTypes.Boolean,
            "Seeder helper prog that always reports a market influence as known.",
            "return true",
            false,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Character, "character"));

        FutureProg shopProg = SeederRepeatabilityHelper.EnsureProg(
            context,
            $"{HelperProgPrefix}AnyShop",
            "Economy",
            "Seeder",
            ProgVariableTypes.Boolean,
            "Seeder helper shopper prog that accepts any qualifying shop in the shopper's zone.",
            "return true",
            false,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Shop, "shop"));

        FutureProg shopWeightProg = SeederRepeatabilityHelper.EnsureProg(
            context,
            $"{HelperProgPrefix}AnyShopWeight",
            "Economy",
            "Seeder",
            ProgVariableTypes.Number,
            "Seeder helper shopper prog that weights all qualifying shops equally.",
            "return 1",
            false,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Shop, "shop"));

        return new SupportProgSet(alwaysKnownProg, shopProg, shopWeightProg);
    }

    private static EconomicZone EnsureEconomicZone(FuturemudDatabaseContext context, EraDefinition era, Zone zone, Currency currency)
    {
        (long clockId, long? calendarId, string? timezoneName, int hours, int minutes, int seconds) = ResolveZoneTimeBinding(context, zone);
        EconomicZone economicZone = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.EconomicZones,
            EconomicZoneName(era),
            x => x.Name,
            () =>
            {
                EconomicZone created = new();
                context.EconomicZones.Add(created);
                return created;
            });

        economicZone.Name = EconomicZoneName(era);
        economicZone.CurrencyId = currency.Id;
        economicZone.ZoneForTimePurposesId = zone.Id;
        economicZone.ReferenceClockId = clockId;
        economicZone.ReferenceCalendarId = calendarId;
        economicZone.ReferenceTime = $"{timezoneName} {hours}:{minutes}:{seconds}";
        economicZone.IntervalType = (int)IntervalType.Monthly;
        economicZone.IntervalAmount = 1;
        economicZone.IntervalModifier = 0;
        economicZone.IntervalOther = 0;
        economicZone.IntervalFallback = 0;
        economicZone.PreviousFinancialPeriodsToKeep = 24;
        economicZone.PermitTaxableLosses = false;
        economicZone.OutstandingTaxesOwed = 0.0m;
        economicZone.TotalRevenueHeld = 0.0m;
        economicZone.ControllingClanId = null;
        economicZone.CurrentFinancialPeriodId = null;
        economicZone.EstateAuctionHouseId = null;
        economicZone.EstateDefaultDiscoverTime = MudTimeSpan.FromDays(28).GetRoundTripParseText;
        economicZone.EstateClaimPeriodLength = MudTimeSpan.FromDays(14).GetRoundTripParseText;
        return economicZone;
    }

    private static (long ClockId, long? CalendarId, string TimezoneName, int Hours, int Minutes, int Seconds) ResolveZoneTimeBinding(
        FuturemudDatabaseContext context,
        Zone zone)
    {
        long? shardClockId = context.ShardsClocks
            .Where(x => x.ShardId == zone.ShardId)
            .Select(x => (long?)x.ClockId)
            .FirstOrDefault();
        Clock clock = shardClockId.HasValue
            ? context.Clocks.First(x => x.Id == shardClockId.Value)
            : context.Clocks.OrderBy(x => x.Id).First();
        long calendarId = context.ShardsCalendars
            .Where(x => x.ShardId == zone.ShardId)
            .Select(x => (long?)x.CalendarId)
            .FirstOrDefault() ?? context.Calendars.OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefault();
        long timezoneId = context.ZonesTimezones
            .Where(x => x.ZoneId == zone.Id && x.ClockId == clock.Id)
            .Select(x => (long?)x.TimezoneId)
            .FirstOrDefault() ?? clock.PrimaryTimezoneId;
        string timezoneName = context.Timezones.FirstOrDefault(x => x.Id == timezoneId)?.Name ?? "UTC";
        return (clock.Id, calendarId, timezoneName, clock.Hours, clock.Minutes, clock.Seconds);
    }

    private static Market EnsureMarket(
        FuturemudDatabaseContext context,
        EraDefinition era,
        EconomicZone economicZone,
        IEnumerable<MarketCategory> categories)
    {
        Market market = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.Markets,
            MarketName(era),
            x => x.Name,
            () =>
            {
                Market created = new();
                context.Markets.Add(created);
                return created;
            });

        market.Name = MarketName(era);
        market.Description =
            $"A builder-facing stock market for the {era.DisplayName} economy template. It is intended as a starting point rather than a finished world economy.";
        market.EconomicZoneId = economicZone.Id;
        market.MarketPriceFormula = DefaultMarketPriceFormula;
        market.MarketCategories ??= new HashSet<MarketCategory>();
        SeederRepeatabilityHelper.ReplaceChildCollection(
            market.MarketCategories,
            categories.OrderBy(x => x.Name).ToList(),
            x => x.Id);
        return market;
    }

    private static CategoryContext BuildCategoryContext(
        FuturemudDatabaseContext context,
        Tag marketRoot,
        IReadOnlyCollection<Tag> descendantTags,
        IReadOnlyDictionary<long, MarketCategory> categoriesByTagId)
    {
        Dictionary<long, Tag> tagsById = context.Tags.ToList().ToDictionary(x => x.Id);
        Dictionary<long, List<Tag>> childTagsByParentId = BuildChildTagMap(descendantTags);
        Dictionary<string, Tag> tagsByName = descendantTags.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, MarketCategory> categoriesByName = categoriesByTagId.Values.ToDictionary(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        Dictionary<string, string> categorySectorMap = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, List<MarketCategory>> categoriesBySector = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, List<MarketCategory>> impactCategoriesBySector = new(StringComparer.OrdinalIgnoreCase);
        foreach (Tag tag in descendantTags)
        {
            string familyName = ResolveTopFamilyName(tag, tagsById);
            string sector = FamilySectorMap.TryGetValue(familyName, out string? mappedSector)
                ? mappedSector
                : "Lifestyle";
            categorySectorMap[tag.Name] = sector;
            if (!categoriesBySector.ContainsKey(sector))
            {
                categoriesBySector[sector] = [];
            }

            categoriesBySector[sector].Add(categoriesByTagId[tag.Id]);
        }

        foreach (Tag familyTag in descendantTags
                     .Where(x => x.ParentId == marketRoot.Id)
                     .OrderBy(x => x.Name))
        {
            string sector = categorySectorMap[familyTag.Name];
            if (!impactCategoriesBySector.ContainsKey(sector))
            {
                impactCategoriesBySector[sector] = [];
            }

            impactCategoriesBySector[sector].AddRange(GetSectorImpactCategories(familyTag, childTagsByParentId, categoriesByTagId));
        }

        foreach ((string sector, List<MarketCategory> categories) in impactCategoriesBySector.ToList())
        {
            impactCategoriesBySector[sector] = categories
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .ToList();
        }

        return new CategoryContext(
            marketRoot,
            tagsById,
            tagsByName,
            categoriesByName,
            categorySectorMap,
            categoriesBySector,
            impactCategoriesBySector);
    }

    private static void EnsureExternalInfluenceTemplates(
        FuturemudDatabaseContext context,
        EraDefinition era,
        CategoryContext categoryContext,
        FutureProg alwaysKnownProg,
        PopulationContext populationContext)
    {
        foreach (SectorInfluenceBlueprint blueprint in ExternalInfluenceBlueprints)
        {
            List<MarketCategory> affectedCategories = blueprint.Sectors
                .SelectMany(sector => categoryContext.ImpactCategoriesBySector.TryGetValue(sector, out List<MarketCategory>? categories) ? categories : [])
                .DistinctBy(x => x.Id)
                .OrderBy(x => x.Name)
                .ToList();
            if (!affectedCategories.Any())
            {
                continue;
            }

            MarketInfluenceTemplate template = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.MarketInfluenceTemplates,
                ExternalTemplateName(era, blueprint),
                x => x.Name,
                () =>
                {
                    MarketInfluenceTemplate created = new();
                    context.MarketInfluenceTemplates.Add(created);
                    return created;
                });

            template.Name = ExternalTemplateName(era, blueprint);
            template.TemplateSummary = blueprint.SummaryFactory(era.DisplayName);
            template.Description = blueprint.DescriptionFactory(era.DisplayName);
            template.CharacterKnowsAboutInfluenceProgId = alwaysKnownProg.Id;
            template.Impacts = SaveImpacts(affectedCategories.Select(category => new MarketImpactValue(
                category.Id,
                blueprint.SupplyImpact,
                blueprint.DemandImpact,
                blueprint.FlatPriceImpact)));
            template.PopulationImpacts = SavePopulationImpacts(BuildPopulationIncomeImpacts(populationContext, blueprint.PopulationIncomeImpacts));
        }
    }

    private static void EnsureCategoryAdjustmentTemplates(
        FuturemudDatabaseContext context,
        EraDefinition era,
        CategoryContext categoryContext,
        FutureProg alwaysKnownProg)
    {
        foreach (MarketCategory category in categoryContext.CategoriesByName.Values.OrderBy(x => x.Name))
        {
            EnsureCategoryAdjustmentTemplate(context, era, category, alwaysKnownProg, "Minor Tariff", 0.05);
            EnsureCategoryAdjustmentTemplate(context, era, category, alwaysKnownProg, "Major Tariff", 0.12);
            EnsureCategoryAdjustmentTemplate(context, era, category, alwaysKnownProg, "Minor Subsidy", -0.05);
            EnsureCategoryAdjustmentTemplate(context, era, category, alwaysKnownProg, "Major Subsidy", -0.12);
        }
    }

    private static void EnsureCategoryAdjustmentTemplate(
        FuturemudDatabaseContext context,
        EraDefinition era,
        MarketCategory category,
        FutureProg alwaysKnownProg,
        string adjustmentName,
        double flatPriceImpact)
    {
        string templateName = $"{era.DisplayName} {category.Name} {adjustmentName}";
        MarketInfluenceTemplate template = SeederRepeatabilityHelper.EnsureNamedEntity(
            context.MarketInfluenceTemplates,
            templateName,
            x => x.Name,
            () =>
            {
                MarketInfluenceTemplate created = new();
                context.MarketInfluenceTemplates.Add(created);
                return created;
            });

        template.Name = templateName;
        template.TemplateSummary = $"{adjustmentName} template for {category.Name} in the {era.DisplayName.ToLowerInvariant()} market.";
        template.Description = flatPriceImpact >= 0.0
            ? $"{adjustmentName} raises the effective market price of {category.Name.ToLowerInvariant()} in the {era.DisplayName.ToLowerInvariant()} economy."
            : $"{adjustmentName} lowers the effective market price of {category.Name.ToLowerInvariant()} in the {era.DisplayName.ToLowerInvariant()} economy.";
        template.CharacterKnowsAboutInfluenceProgId = alwaysKnownProg.Id;
        template.Impacts = SaveImpacts(
        [
            new MarketImpactValue(category.Id, 0.0, 0.0, flatPriceImpact)
        ]);
        template.PopulationImpacts = SavePopulationImpacts([]);
    }

    private static void EnsureIncomeInfluenceTemplates(
        FuturemudDatabaseContext context,
        EraDefinition era,
        FutureProg alwaysKnownProg,
        PopulationContext populationContext)
    {
        foreach (IncomeInfluenceBlueprint blueprint in IncomeInfluenceBlueprints)
        {
            string templateName = $"{era.DisplayName} {blueprint.Name}";
            MarketInfluenceTemplate template = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.MarketInfluenceTemplates,
                templateName,
                x => x.Name,
                () =>
                {
                    MarketInfluenceTemplate created = new();
                    context.MarketInfluenceTemplates.Add(created);
                    return created;
                });

            template.Name = templateName;
            template.TemplateSummary = blueprint.SummaryFactory(era.DisplayName);
            template.Description = blueprint.DescriptionFactory(era.DisplayName);
            template.CharacterKnowsAboutInfluenceProgId = alwaysKnownProg.Id;
            template.Impacts = SaveImpacts([]);
            template.PopulationImpacts = SavePopulationImpacts(BuildPopulationIncomeImpacts(populationContext, blueprint.PopulationIncomeImpacts));
        }
    }

    private static PopulationContext EnsurePopulationsAndStressTemplates(
        FuturemudDatabaseContext context,
        EraDefinition era,
        Currency currency,
        Market market,
        CategoryContext categoryContext,
        FutureProg alwaysKnownProg)
    {
        Dictionary<string, List<StressTemplateContext>> stressTemplatesByPopulation = new(StringComparer.OrdinalIgnoreCase);
        foreach (PopulationBlueprint blueprint in era.Populations)
        {
            string populationName = PopulationName(era, blueprint);
            List<StressTemplateContext> stressTemplates = new();
            foreach (StressLevelDefinition level in StressLevels)
            {
                string templateName = StressTemplateName(era, blueprint, level);
                List<MarketImpactValue> impactedCategories = BuildStressImpacts(blueprint, level, categoryContext).ToList();
                MarketInfluenceTemplate template = SeederRepeatabilityHelper.EnsureNamedEntity(
                    context.MarketInfluenceTemplates,
                    templateName,
                    x => x.Name,
                    () =>
                    {
                        MarketInfluenceTemplate created = new();
                        context.MarketInfluenceTemplates.Add(created);
                        return created;
                    });

                template.Name = templateName;
                template.TemplateSummary = $"{level.DisplayName} stress state for {populationName}.";
                template.Description =
                    $"{populationName} has entered a {level.DisplayName.ToLowerInvariant()} state. Households cut lower-priority spending while strained local production also reduces supply in the sectors they anchor.";
                template.CharacterKnowsAboutInfluenceProgId = alwaysKnownProg.Id;
                template.Impacts = SaveImpacts(impactedCategories);
                template.PopulationImpacts = SavePopulationImpacts([]);
                stressTemplates.Add(new StressTemplateContext(level, templateName));
            }

            stressTemplatesByPopulation[populationName] = stressTemplates;
        }

        List<PopulationSeedContext> populations = new();
        foreach (PopulationBlueprint blueprint in era.Populations)
        {
            string populationName = PopulationName(era, blueprint);
            IReadOnlyList<StressPointValue> stressProgs = EnsureStressProgs(context, era, blueprint, stressTemplatesByPopulation[populationName]);
            MarketPopulation population = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.MarketPopulations,
                populationName,
                x => x.Name,
                () =>
                {
                    MarketPopulation created = new();
                    context.MarketPopulations.Add(created);
                    return created;
                });

            population.Name = populationName;
            population.Description = blueprint.Description;
            population.PopulationScale = blueprint.PopulationScale;
            population.IncomeFactor = blueprint.IncomeFactor;
            population.Savings = 0.0m;
            population.SavingsCap = blueprint.SeedSavingsCap;
            population.StressFlickerThreshold = 0.01m;
            population.MarketId = market.Id;
            List<MarketNeedValue> scaledNeeds = blueprint.Needs
                .Select(need => new MarketNeedValue(
                    categoryContext.CategoriesByName[need.CategoryName].Id,
                    ScaleExpenditure(era, currency, need.BaseExpenditure)))
                .ToList();
            population.MarketPopulationNeeds = SaveNeeds(scaledNeeds);
            population.MarketStressPoints = SaveStressPoints(stressProgs);

            populations.Add(new PopulationSeedContext(
                blueprint,
                population.Id,
                populationName,
                stressProgs,
                ResolvePopulationBudget(blueprint, scaledNeeds.Sum(x => x.BaseExpenditure))));
        }

        return new PopulationContext(populations);
    }

    private static IEnumerable<MarketImpactValue> BuildStressImpacts(
        PopulationBlueprint blueprint,
        StressLevelDefinition level,
        CategoryContext categoryContext)
    {
        Dictionary<long, (double Supply, double Demand)> impacts = new();

        void SetDemandImpact(string categoryName, double demandImpact)
        {
            if (!categoryContext.CategoriesByName.TryGetValue(categoryName, out MarketCategory? category))
            {
                return;
            }

            (double Supply, double Demand) existing = impacts.TryGetValue(category.Id, out (double Supply, double Demand) current)
                ? current
                : (Supply: 0.0, Demand: 0.0);
            impacts[category.Id] = (existing.Supply, demandImpact);
        }

        void EnsureMinimumDemandImpact(string categoryName, double minimumDemandImpact)
        {
            if (!categoryContext.CategoriesByName.TryGetValue(categoryName, out MarketCategory? category))
            {
                return;
            }

            (double Supply, double Demand) existing = impacts.TryGetValue(category.Id, out (double Supply, double Demand) current)
                ? current
                : (Supply: 0.0, Demand: 0.0);
            if (existing.Demand >= minimumDemandImpact)
            {
                return;
            }

            impacts[category.Id] = (existing.Supply, minimumDemandImpact);
        }

        void ApplySupplyImpact(string sector, double supplyImpact)
        {
            if (!categoryContext.ImpactCategoriesBySector.TryGetValue(sector, out List<MarketCategory>? categories))
            {
                return;
            }

            foreach (MarketCategory category in categories)
            {
                (double Supply, double Demand) existing = impacts.TryGetValue(category.Id, out (double Supply, double Demand) current)
                    ? current
                    : (Supply: 0.0, Demand: 0.0);
                impacts[category.Id] = (Math.Min(existing.Supply, supplyImpact), existing.Demand);
            }
        }

        foreach (PopulationNeedBlueprint need in blueprint.Needs)
        {
            string sector = categoryContext.CategorySectorMap.TryGetValue(need.CategoryName, out string? value)
                ? value
                : "Lifestyle";
            double demandImpact = sector switch
            {
                "Essentials" => level.EssentialDemandImpact,
                "Industry" => level.IndustryDemandImpact,
                "Logistics" => level.LogisticsDemandImpact,
                "Martial" => blueprint.Archetype == PopulationArchetype.Martial
                    ? level.MartialDemandImpact
                    : level.IndustryDemandImpact / 2.0,
                _ => need.CategoryName.Contains("Luxury", StringComparison.OrdinalIgnoreCase)
                    ? level.LuxuryDemandImpactFor(blueprint.Archetype)
                    : level.LifestyleDemandImpactFor(blueprint.Archetype)
            };

            SetDemandImpact(need.CategoryName, AdjustDemandImpactForPriority(demandImpact, need.Weight));
        }

        EnsureMinimumDemandImpact("Staple Food", Math.Max(level.EssentialDemandImpact, 0.08));
        EnsureMinimumDemandImpact("Salt", Math.Max(level.EssentialDemandImpact / 2.0, 0.04));
        EnsureMinimumDemandImpact("Simple Clothing", Math.Max(level.HeatAndMedicineDemandImpact, 0.05));
        EnsureMinimumDemandImpact("Simple Medicine", Math.Max(level.HeatAndMedicineDemandImpact, 0.05));
        EnsureMinimumDemandImpact("Standard Medicine", Math.Max(level.HeatAndMedicineDemandImpact, 0.05));
        EnsureMinimumDemandImpact("High-Quality Medicine", Math.Max(level.HeatAndMedicineDemandImpact, 0.04));
        EnsureMinimumDemandImpact("Combustion Heating", Math.Max(level.HeatAndMedicineDemandImpact, 0.05));
        EnsureMinimumDemandImpact("Lighting", Math.Max(level.HeatAndMedicineDemandImpact / 2.0, 0.03));

        foreach ((string? sector, double multiplier) in ProducedSectorsFor(blueprint.Archetype))
        {
            ApplySupplyImpact(sector, level.ProductionSupplyImpact * multiplier);
        }

        if (impacts.Values.All(x => x.Supply >= 0.0))
        {
            ApplySupplyImpact("Lifestyle", level.ProductionSupplyImpact * 0.40);
        }

        if (impacts.Values.All(x => x.Supply >= 0.0))
        {
            ApplySupplyImpact("Essentials", level.ProductionSupplyImpact * 0.40);
        }

        return impacts
            .OrderBy(x => x.Key)
            .Select(x => new MarketImpactValue(x.Key, x.Value.Supply, x.Value.Demand))
            .ToList();
    }

    private static IReadOnlyList<StressPointValue> EnsureStressProgs(
        FuturemudDatabaseContext context,
        EraDefinition era,
        PopulationBlueprint blueprint,
        IEnumerable<StressTemplateContext> stressTemplates)
    {
        List<StressPointValue> results = new();
        string populationName = PopulationName(era, blueprint);
        foreach (StressTemplateContext? template in stressTemplates.OrderBy(x => x.Level.Threshold))
        {
            FutureProg startProg = SeederRepeatabilityHelper.EnsureProg(
                context,
                StressStartProgName(era, blueprint, template.Level),
                "Economy",
                "Seeder",
                ProgVariableTypes.Void,
                $"Starts the {template.Level.DisplayName} market stress template for {populationName}.",
                $"BeginInfluence(@market.id, \"{template.TemplateName}\")",
                false,
                false,
                FutureProgStaticType.NotStatic,
                (ProgVariableTypes.Number, "populationid"),
                (ProgVariableTypes.Boolean, "rising"),
                (ProgVariableTypes.Market, "market"));

            FutureProg endProg = SeederRepeatabilityHelper.EnsureProg(
                context,
                StressEndProgName(era, blueprint, template.Level),
                "Economy",
                "Seeder",
                ProgVariableTypes.Void,
                $"Ends the {template.Level.DisplayName} market stress template for {populationName}.",
                $"EndInfluenceByTemplate(@market.id, \"{template.TemplateName}\")",
                false,
                false,
                FutureProgStaticType.NotStatic,
                (ProgVariableTypes.Number, "populationid"),
                (ProgVariableTypes.Boolean, "rising"),
                (ProgVariableTypes.Market, "market"));

            results.Add(new StressPointValue(
                template.Level.DisplayName,
                $"{populationName} is {template.Level.DisplayName.ToLowerInvariant()}, curbing lower-priority demand while also depressing supply in the sectors that population normally sustains.",
                template.Level.Threshold,
                startProg.Id,
                endProg.Id));
        }

        return results;
    }

    private static void EnsureShoppers(
        FuturemudDatabaseContext context,
        EraDefinition era,
        EconomicZone economicZone,
        PopulationContext populationContext,
        decimal shopperScale,
        SupportProgSet supportProgs)
    {
        int[] nextShopOffsets = new[] { 2, 6, 10, 14, 18 };
        for (int index = 0; index < populationContext.Populations.Count; index++)
        {
            PopulationSeedContext population = populationContext.Populations[index];
            FutureProg buyProg = EnsurePopulationBuyProg(context, era, population.Blueprint);
            FutureProg itemWeightProg = EnsurePopulationItemWeightProg(context, era, population.Blueprint);
            Shopper shopper = SeederRepeatabilityHelper.EnsureNamedEntity(
                context.Shoppers,
                ShopperName(era, population.Blueprint),
                x => x.Name,
                () =>
                {
                    Shopper created = new();
                    context.Shoppers.Add(created);
                    return created;
                });

            decimal budget = decimal.Round(population.ScaledBudget * shopperScale, 2, MidpointRounding.AwayFromZero);
            shopper.Name = ShopperName(era, population.Blueprint);
            shopper.EconomicZoneId = economicZone.Id;
            shopper.Type = "simple";
            shopper.Interval = new RecurringInterval
            {
                IntervalAmount = 1,
                Modifier = 0,
                Type = IntervalType.Daily
            }.ToString();
            shopper.NextDate = BuildMudDateTimeString(
                economicZone.ReferenceCalendarId ?? 0,
                context.Calendars.First(x => x.Id == economicZone.ReferenceCalendarId).Date,
                economicZone.ReferenceClockId,
                ExtractTimezoneName(economicZone.ReferenceTime),
                nextShopOffsets[index % nextShopOffsets.Length],
                0,
                0);
            shopper.Definition = new XElement("Shopper",
                new XElement("BudgetPerShop", budget.ToString(CultureInfo.InvariantCulture)),
                new XElement("WillShopAtShopProg", supportProgs.AcceptAnyShopProg.Id),
                new XElement("ShopSelectionWeightProg", supportProgs.EqualShopWeightProg.Id),
                new XElement("WillBuyItemProg", buyProg.Id),
                new XElement("ItemBuyWeightProg", itemWeightProg.Id),
                new XElement("SkipEmptyShops", true)).ToString();
        }
    }

    private static FutureProg EnsurePopulationBuyProg(
        FuturemudDatabaseContext context,
        EraDefinition era,
        PopulationBlueprint blueprint)
    {
        string condition = string.Join(" or ", blueprint.Needs
            .Select(x => $"@item.tags.Any(tag, @tag == \"{x.CategoryName}\")"));
        return SeederRepeatabilityHelper.EnsureProg(
            context,
            PopulationBuyProgName(era, blueprint),
            "Economy",
            "Seeder",
            ProgVariableTypes.Boolean,
            $"Determines whether the {ShopperName(era, blueprint)} will buy an item.",
            $"return {condition}",
            false,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Item, "item"),
            (ProgVariableTypes.Merchandise, "merchandise"),
            (ProgVariableTypes.Number, "price"));
    }

    private static FutureProg EnsurePopulationItemWeightProg(
        FuturemudDatabaseContext context,
        EraDefinition era,
        PopulationBlueprint blueprint)
    {
        List<string> lines = new()
        {
            "var weight as number",
            "weight = 1"
        };

        foreach (PopulationNeedBlueprint? need in blueprint.Needs.OrderByDescending(x => x.Weight).ThenBy(x => x.CategoryName))
        {
            lines.Add($"if (@item.tags.Any(tag, @tag == \"{need.CategoryName}\"))");
            lines.Add($"\tweight = @weight + {need.Weight}");
            lines.Add("end if");
        }

        lines.Add("return @weight");
        return SeederRepeatabilityHelper.EnsureProg(
            context,
            PopulationItemWeightProgName(era, blueprint),
            "Economy",
            "Seeder",
            ProgVariableTypes.Number,
            $"Weights purchases for the {ShopperName(era, blueprint)} using the seeded market tags.",
            string.Join("\n", lines),
            false,
            false,
            FutureProgStaticType.NotStatic,
            (ProgVariableTypes.Item, "item"),
            (ProgVariableTypes.Merchandise, "merchandise"),
            (ProgVariableTypes.Number, "price"));
    }

    private static string SaveImpacts(IEnumerable<MarketImpactValue> impacts)
    {
        return new XElement("Impacts",
            impacts.Select(impact => new XElement("Impact",
                new XAttribute("demand", impact.DemandImpact.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("supply", impact.SupplyImpact.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("price", impact.FlatPriceImpact.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("category", impact.CategoryId)))).ToString();
    }

    private static string SavePopulationImpacts(IEnumerable<PopulationIncomeImpactValue> impacts)
    {
        return new XElement("PopulationImpacts",
            impacts.Select(impact => new XElement("PopulationImpact",
                new XAttribute("population", impact.PopulationId),
                new XAttribute("additive", impact.AdditiveIncomeImpact.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("multiplier", impact.MultiplicativeIncomeImpact.ToString(CultureInfo.InvariantCulture))))).ToString();
    }

    private static IReadOnlyList<PopulationIncomeImpactValue> BuildPopulationIncomeImpacts(
        PopulationContext populationContext,
        IReadOnlyList<PopulationIncomeImpactBlueprint>? blueprints)
    {
        if (blueprints is null || blueprints.Count == 0)
        {
            return [];
        }

        return (from blueprint in blueprints
                from population in populationContext.Populations
                where population.Blueprint.Archetype == blueprint.Archetype
                select new PopulationIncomeImpactValue(
                    population.PopulationId,
                    blueprint.AdditiveIncomeImpact,
                    blueprint.MultiplicativeIncomeImpact)).ToList();
    }

    private static string SaveNeeds(IEnumerable<MarketNeedValue> needs)
    {
        return new XElement("Needs",
            needs.Select(need => new XElement("Need",
                new XAttribute("category", need.CategoryId),
                new XAttribute("expenditure", need.BaseExpenditure.ToString(CultureInfo.InvariantCulture))))).ToString();
    }

	private static IReadOnlyList<(MarketCategory Category, decimal Weight)> GetStockCombinationComponents(
		string familyName,
		IEnumerable<MarketCategory> componentCategories)
	{
		if (!StockCombinationCategoryWeights.TryGetValue(familyName, out IReadOnlyList<(string CategoryName, decimal Weight)>? definitions))
		{
			throw new InvalidOperationException($"No stock combination category weights are defined for {familyName}.");
		}

		Dictionary<string, MarketCategory> categoriesByName =
			componentCategories.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
		List<string> missingCategoryNames = definitions
			.Select(x => x.CategoryName)
			.Where(x => !categoriesByName.ContainsKey(x))
			.OrderBy(x => x)
			.ToList();
		if (missingCategoryNames.Any())
		{
			throw new InvalidOperationException(
				$"The stock combination category {familyName} is missing weighted child categories: {missingCategoryNames.ListToString()}.");
		}

		List<string> unexpectedCategoryNames = categoriesByName.Keys
			.Where(x => definitions.All(y => !y.CategoryName.Equals(x, StringComparison.OrdinalIgnoreCase)))
			.OrderBy(x => x)
			.ToList();
		if (unexpectedCategoryNames.Any())
		{
			throw new InvalidOperationException(
				$"The stock combination category {familyName} has no seeded weights for direct child categories: {unexpectedCategoryNames.ListToString()}.");
		}

		return definitions
			.Select(x => (Category: categoriesByName[x.CategoryName], Weight: x.Weight))
			.ToList();
	}

	private static string DescribeCombinationComponents(IEnumerable<(MarketCategory Category, decimal Weight)> components)
	{
		return components
			.Select(x => $"{x.Category.Name} ({x.Weight.ToString("P0", CultureInfo.InvariantCulture)})")
			.ListToString();
	}

	private static string SaveCombinationCategories(IEnumerable<(MarketCategory Category, decimal Weight)> components)
	{
		return new XElement("Components",
			components.Select(component => new XElement("Component",
				new XAttribute("category", component.Category.Id),
				new XAttribute("weight", component.Weight.ToString(CultureInfo.InvariantCulture))))).ToString();
	}

    private static string SaveStressPoints(IEnumerable<StressPointValue> stressPoints)
    {
        return new XElement("Stresses",
            stressPoints.Select(point => new XElement("Stress",
                new XAttribute("stress", point.Threshold.ToString(CultureInfo.InvariantCulture)),
                new XAttribute("onstart", point.OnStartProgId),
                new XAttribute("onend", point.OnEndProgId),
                new XAttribute("name", point.Name),
                new XCData(point.Description)))).ToString();
    }

    private static string ResolveTopFamilyName(Tag tag, IReadOnlyDictionary<long, Tag> tagsById)
    {
        Tag current = tag;
        while (current.ParentId.HasValue && tagsById.TryGetValue(current.ParentId.Value, out Tag? parent))
        {
            if (parent.Name.Equals(MarketRootTagName, StringComparison.OrdinalIgnoreCase))
            {
                return current.Name;
            }

            current = parent;
        }

        return current.Name;
    }

    private static string ResolveTopFamilyName(Tag tag, IReadOnlyCollection<Tag> allTags)
    {
        return ResolveTopFamilyName(tag, allTags.ToDictionary(x => x.Id));
    }

    private static Dictionary<long, List<Tag>> BuildChildTagMap(IEnumerable<Tag> tags)
    {
        return tags
            .Where(x => x.ParentId.HasValue)
            .GroupBy(x => x.ParentId!.Value)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Name).ToList());
    }

    private static bool ShouldSeedCombinationCategory(Tag tag, IReadOnlyDictionary<long, List<Tag>> childTagsByParentId)
    {
        return StockCombinationFamilyExamples.Contains(tag.Name)
               && childTagsByParentId.TryGetValue(tag.Id, out List<Tag>? childTags)
               && childTags.Count > 1;
    }

    private static IEnumerable<MarketCategory> GetSectorImpactCategories(
        Tag familyTag,
        IReadOnlyDictionary<long, List<Tag>> childTagsByParentId,
        IReadOnlyDictionary<long, MarketCategory> categoriesByTagId)
    {
        if (!categoriesByTagId.TryGetValue(familyTag.Id, out MarketCategory? familyCategory))
        {
            return [];
        }

        if (familyCategory.MarketCategoryType == 1)
        {
            HashSet<long> directComponentTagIds = childTagsByParentId.TryGetValue(familyTag.Id, out List<Tag>? directChildTags)
                ? directChildTags.Select(x => x.Id).ToHashSet()
                : [];

            return EnumerateTagAndDescendants(familyTag, childTagsByParentId)
                .Where(x => x.Id == familyTag.Id || !directComponentTagIds.Contains(x.Id))
                .Where(x => categoriesByTagId.ContainsKey(x.Id))
                .Select(x => categoriesByTagId[x.Id])
                .DistinctBy(x => x.Id)
                .ToList();
        }

        return EnumerateTagAndDescendants(familyTag, childTagsByParentId)
            .Where(x => categoriesByTagId.ContainsKey(x.Id))
            .Select(x => categoriesByTagId[x.Id])
            .DistinctBy(x => x.Id)
            .ToList();
    }

    private static IEnumerable<Tag> EnumerateTagAndDescendants(
        Tag rootTag,
        IReadOnlyDictionary<long, List<Tag>> childTagsByParentId)
    {
        Queue<Tag> queue = new([rootTag]);
        while (queue.Count > 0)
        {
            Tag current = queue.Dequeue();
            yield return current;
            if (!childTagsByParentId.TryGetValue(current.Id, out List<Tag>? childTags))
            {
                continue;
            }

            foreach (Tag childTag in childTags)
            {
                queue.Enqueue(childTag);
            }
        }
    }

    private static string EconomicZoneName(EraDefinition era)
    {
        return $"{era.DisplayName} {ZoneSuffix}";
    }

    private static string MarketName(EraDefinition era)
    {
        return $"{era.DisplayName} {MarketSuffix}";
    }

    private static string ExternalTemplateName(EraDefinition era, SectorInfluenceBlueprint blueprint)
    {
        return $"{HelperProgPrefix} External {era.DisplayName} {blueprint.Name}";
    }

    private static string PopulationName(EraDefinition era, PopulationBlueprint blueprint)
    {
        return $"{era.DisplayName} {blueprint.Name}";
    }

    private static string StressTemplateName(EraDefinition era, PopulationBlueprint blueprint, StressLevelDefinition level)
    {
        return $"{HelperProgPrefix} Stress {PopulationName(era, blueprint)} {level.DisplayName}";
    }

    private static string StressStartProgName(EraDefinition era, PopulationBlueprint blueprint, StressLevelDefinition level)
    {
        return $"{HelperProgPrefix}Start{PopulationName(era, blueprint)}{level.DisplayName}";
    }

    private static string StressEndProgName(EraDefinition era, PopulationBlueprint blueprint, StressLevelDefinition level)
    {
        return $"{HelperProgPrefix}End{PopulationName(era, blueprint)}{level.DisplayName}";
    }

    private static string ShopperName(EraDefinition era, PopulationBlueprint blueprint)
    {
        return $"{PopulationName(era, blueprint)} Shopper";
    }

    private static string PopulationBuyProgName(EraDefinition era, PopulationBlueprint blueprint)
    {
        return $"{HelperProgPrefix}Buy{PopulationName(era, blueprint)}";
    }

    private static string PopulationItemWeightProgName(EraDefinition era, PopulationBlueprint blueprint)
    {
        return $"{HelperProgPrefix}Weight{PopulationName(era, blueprint)}";
    }

    private static string ExtractTimezoneName(string referenceTime)
    {
        string[] split = referenceTime.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        return split.Length > 0 ? split[0] : "UTC";
    }

    private static string BuildMudDateTimeString(
        long calendarId,
        string date,
        long clockId,
        string timezoneName,
        int hours,
        int minutes,
        int seconds)
    {
        return $"{calendarId}_{date}_{clockId}_{timezoneName} {hours}:{minutes}:{seconds}";
    }

    private static decimal ScaleExpenditure(EraDefinition era, Currency currency, decimal baseExpenditure)
    {
        decimal scale = ResolveEraSterlingScale(era) * ResolveCurrencyBaseUnitsPerPound(currency) / 100.0m;
        return decimal.Round(baseExpenditure * scale, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal ResolvePopulationBudget(PopulationBlueprint blueprint, decimal scaledNeedTotal)
    {
        return decimal.Round(scaledNeedTotal * blueprint.ShopperBudgetFactor, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal ResolveEraSterlingScale(EraDefinition era)
    {
        return era.Key switch
        {
            "Classical Age" => 0.82m,
            "Feudal Age" => 0.62m,
            "Medieval Age" => 0.72m,
            "Early Modern Age" => 0.90m,
            _ => 1.00m
        };
    }

    private static decimal ResolveCurrencyBaseUnitsPerPound(Currency currency)
    {
        return currency.Name.ToLowerInvariant() switch
        {
            "bits" => 100.0m,
            "dollars" => 125.0m,
            "pounds" => 960.0m,
            "roman" => 768.0m,
            "standard" => 1000.0m,
            "gondorian" => 400.0m,
            _ => 100.0m
        };
    }

    private static double AdjustDemandImpactForPriority(double demandImpact, int weight)
    {
        double multiplier = demandImpact < 0.0
            ? weight switch
            {
                >= 8 => 0.55,
                >= 6 => 0.72,
                >= 4 => 0.90,
                >= 2 => 1.12,
                _ => 1.30
            }
            : weight switch
            {
                >= 8 => 1.10,
                >= 6 => 1.00,
                >= 4 => 0.92,
                >= 2 => 0.84,
                _ => 0.76
            };
        return demandImpact * multiplier;
    }

    private static IEnumerable<(string Sector, double Multiplier)> ProducedSectorsFor(PopulationArchetype archetype)
    {
        return archetype switch
        {
            PopulationArchetype.Commoner =>
            [
                ("Essentials", 1.00),
                ("Lifestyle", 0.35),
                ("Industry", 0.25)
            ],
            PopulationArchetype.Rural =>
            [
                ("Essentials", 1.00),
                ("Industry", 0.55),
                ("Logistics", 0.25)
            ],
            PopulationArchetype.Merchant =>
            [
                ("Lifestyle", 0.85),
                ("Industry", 0.75),
                ("Logistics", 0.70)
            ],
            PopulationArchetype.Martial =>
            [
                ("Martial", 1.00),
                ("Industry", 0.55),
                ("Logistics", 0.35)
            ],
            PopulationArchetype.Clergy =>
            [
                ("Lifestyle", 0.70),
                ("Logistics", 0.45)
            ],
            PopulationArchetype.Monastic =>
            [
                ("Essentials", 0.70),
                ("Lifestyle", 0.75),
                ("Industry", 0.45)
            ],
            PopulationArchetype.Elite =>
            [
                ("Lifestyle", 0.60),
                ("Logistics", 0.40)
            ],
            _ => []
        };
    }

    private sealed record EraDefinition(string Key, string DisplayName, IReadOnlyList<PopulationBlueprint> Populations);

    private sealed record PopulationBlueprint(
        string Name,
        string Description,
        int PopulationScale,
        PopulationArchetype Archetype,
        IReadOnlyList<PopulationNeedBlueprint> Needs,
        decimal IncomeFactor = 1.0m,
        decimal? SavingsCap = null)
    {
        public decimal ShopperBudgetFactor => Archetype switch
        {
            PopulationArchetype.Commoner => 0.18m,
            PopulationArchetype.Rural => 0.19m,
            PopulationArchetype.Merchant => 0.28m,
            PopulationArchetype.Martial => 0.29m,
            PopulationArchetype.Clergy => 0.26m,
            PopulationArchetype.Monastic => 0.22m,
            PopulationArchetype.Elite => 0.34m,
            _ => 0.18m
        };

        public decimal SeedSavingsCap => SavingsCap ?? Archetype switch
        {
            PopulationArchetype.Commoner => 0.20m,
            PopulationArchetype.Rural => 0.30m,
            PopulationArchetype.Merchant => 0.60m,
            PopulationArchetype.Martial => 0.35m,
            PopulationArchetype.Clergy => 0.45m,
            PopulationArchetype.Monastic => 0.55m,
            PopulationArchetype.Elite => 1.20m,
            _ => 0.20m
        };
    }

    private sealed record PopulationNeedBlueprint(string CategoryName, decimal BaseExpenditure, int Weight);
    private sealed record SectorInfluenceBlueprint(
        string Name,
        IReadOnlyList<string> Sectors,
        double SupplyImpact,
        double DemandImpact,
        Func<string, string> DescriptionFactory,
        Func<string, string> SummaryFactory,
        IReadOnlyList<PopulationIncomeImpactBlueprint>? PopulationIncomeImpacts = null,
        double FlatPriceImpact = 0.0);
    private sealed record IncomeInfluenceBlueprint(
        string Name,
        Func<string, string> DescriptionFactory,
        Func<string, string> SummaryFactory,
        IReadOnlyList<PopulationIncomeImpactBlueprint> PopulationIncomeImpacts);
    private sealed record PopulationIncomeImpactBlueprint(
        PopulationArchetype Archetype,
        decimal AdditiveIncomeImpact = 0.0m,
        decimal MultiplicativeIncomeImpact = 1.0m);
    private sealed record StressLevelDefinition(
        string DisplayName,
        decimal Threshold,
        double EssentialDemandImpact,
        double LuxuryDemandImpact,
        double IndustryDemandImpact,
        double LogisticsDemandImpact,
        double MartialDemandImpact,
        double HeatAndMedicineDemandImpact,
        double ProductionSupplyImpact)
    {
        public double LifestyleDemandImpactFor(PopulationArchetype archetype)
        {
            return archetype == PopulationArchetype.Elite
                ? LuxuryDemandImpact * 0.65
                : LuxuryDemandImpact * 0.85;
        }

        public double LuxuryDemandImpactFor(PopulationArchetype archetype)
        {
            return archetype == PopulationArchetype.Elite
                ? LuxuryDemandImpact * 0.75
                : LuxuryDemandImpact;
        }
    }

    private enum PopulationArchetype
    {
        Commoner,
        Rural,
        Merchant,
        Martial,
        Clergy,
        Monastic,
        Elite
    }

    private sealed record SupportProgSet(FutureProg AlwaysKnownProg, FutureProg AcceptAnyShopProg, FutureProg EqualShopWeightProg);
    private sealed record CategoryContext(
        Tag MarketRoot,
        IReadOnlyDictionary<long, Tag> TagsById,
        IReadOnlyDictionary<string, Tag> TagsByName,
        IReadOnlyDictionary<string, MarketCategory> CategoriesByName,
        IReadOnlyDictionary<string, string> CategorySectorMap,
        IReadOnlyDictionary<string, List<MarketCategory>> CategoriesBySector,
        IReadOnlyDictionary<string, List<MarketCategory>> ImpactCategoriesBySector);
    private sealed record MarketImpactValue(long CategoryId, double SupplyImpact, double DemandImpact, double FlatPriceImpact = 0.0);
    private sealed record PopulationIncomeImpactValue(long PopulationId, decimal AdditiveIncomeImpact, decimal MultiplicativeIncomeImpact);
    private sealed record MarketNeedValue(long CategoryId, decimal BaseExpenditure);
    private sealed record StressPointValue(string Name, string Description, decimal Threshold, long OnStartProgId, long OnEndProgId);
    private sealed record StressTemplateContext(StressLevelDefinition Level, string TemplateName);
    private sealed record PopulationSeedContext(
        PopulationBlueprint Blueprint,
        long PopulationId,
        string PopulationName,
        IReadOnlyList<StressPointValue> StressPoints,
        decimal ScaledBudget);
    private sealed record PopulationContext(IReadOnlyList<PopulationSeedContext> Populations);
}

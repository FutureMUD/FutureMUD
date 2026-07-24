#nullable enable

using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class UsefulSeeder
{
	private void SeedEraDependencyComponents(FuturemudDatabaseContext context, DateTime now, Account dbaccount,
		ref long nextId)
	{
		var currentId = nextId;

		void Upsert(string type, string name, string description, XElement definition)
		{
			UpsertComponent(context, ref currentId, dbaccount, now, type, name, description, definition.ToString());
		}

		XElement ContainerDefinition(double weight, SizeCategory maxSize, bool closable, bool transparent,
			string preposition, params long[] allowedTagIds)
		{
			return new XElement("Definition",
				new XAttribute("Weight", weight),
				new XAttribute("MaxSize", (int)maxSize),
				new XAttribute("Preposition", preposition),
				new XAttribute("Closable", closable),
				new XAttribute("Transparent", transparent),
				new XElement("AllowedTags", allowedTagIds.Select(x => new XElement("Tag", x))),
				new XElement("BlockedTags"));
		}

		XElement LiquidContainerDefinition(double capacity, bool closable, double weightLimit)
		{
			return new XElement("Definition",
				new XAttribute("LiquidCapacity", capacity),
				new XAttribute("Closable", closable),
				new XAttribute("Transparent", false),
				new XAttribute("WeightLimit", weightLimit));
		}

		XElement LockingContainerDefinition(double weight, SizeCategory maxSize, bool transparent,
			Difficulty forceDifficulty, Difficulty pickDifficulty, string lockType)
		{
			return new XElement("Definition",
				new XAttribute("Weight", weight),
				new XAttribute("MaxSize", (int)maxSize),
				new XAttribute("Preposition", "in"),
				new XAttribute("Transparent", transparent),
				new XElement("ForceDifficulty", (int)forceDifficulty),
				new XElement("PickDifficulty", (int)pickDifficulty),
				new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$.")),
				new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$.")),
				new XElement("LockEmoteNoActor", new XCData("@ click|clicks shut.")),
				new XElement("UnlockEmoteNoActor", new XCData("@ click|clicks open.")),
				new XElement("LockType", lockType));
		}

		XElement HandToolDefinition()
		{
			return new XElement("Definition",
				new XElement("MultiplierReductionPerQuality", 0.1),
				new XElement("BaseMultiplier", 1.5),
				new XElement("ToolDurabilitySecondsExpression", "(1+quality) * 3600"));
		}

		long TagId(string name)
		{
			return context.Tags
				       .AsEnumerable()
				       .Single(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
				       .Id;
		}

		Upsert("Container", "Container_PreIndustrial_CompartmentBox",
			"A small closable compartmented box represented as one logical inventory.",
			ContainerDefinition(5000, SizeCategory.Tiny, true, false, "in"));
		Upsert("Container", "Container_PreIndustrial_LiddedBasket",
			"A closable pre-industrial basket for ordinary domestic and market storage.",
			ContainerDefinition(50000, SizeCategory.Small, true, false, "in"));
		Upsert("Container", "Container_PreIndustrial_LiddedHamper",
			"A large closable pre-industrial hamper or pannier.",
			ContainerDefinition(150000, SizeCategory.Normal, true, false, "in"));
		Upsert("Container", "Container_PreIndustrial_Display_Plinth",
			"An open display plinth, pedestal or stand surface.",
			ContainerDefinition(75000, SizeCategory.Normal, false, false, "on"));
		Upsert("Container", "Container_CartridgeBandolier",
			"A closable cartridge bandolier restricted to paper cartridges and wooden powder charges.",
			ContainerDefinition(12000, SizeCategory.Small, true, false, "in",
				TagId("Paper Cartridges"), TagId("Wooden Powder Charges")));

		Upsert("Liquid Container", "LContainer_PreIndustrial_Cup_150ml",
			"An opaque open pre-industrial cup with a 150 ml capacity.",
			LiquidContainerDefinition(0.15, false, 1500));
		Upsert("Liquid Container", "LContainer_PreIndustrial_Bowl_750ml",
			"An opaque open pre-industrial bowl with a 750 ml capacity.",
			LiquidContainerDefinition(0.75, false, 7500));
		Upsert("Liquid Container", "LContainer_PreIndustrial_Basin_5L",
			"An opaque open pre-industrial basin with a 5 litre capacity.",
			LiquidContainerDefinition(5.0, false, 50000));
		Upsert("Liquid Container", "LContainer_PreIndustrial_Ewer_2L",
			"An opaque open pre-industrial ewer with a 2 litre capacity.",
			LiquidContainerDefinition(2.0, false, 20000));
		Upsert("Liquid Container", "LContainer_PreIndustrial_Pitcher_4L",
			"An opaque open pre-industrial pitcher with a 4 litre capacity.",
			LiquidContainerDefinition(4.0, false, 40000));
		Upsert("Liquid Container", "LContainer_PreIndustrial_Pot_12L",
			"An opaque open pre-industrial pot with a 12 litre capacity.",
			LiquidContainerDefinition(12.0, false, 120000));
		Upsert("Liquid Container", "LContainer_PreIndustrial_StorageJar_12L",
			"An opaque closable pre-industrial storage jar with a 12 litre capacity.",
			LiquidContainerDefinition(12.0, true, 120000));
		Upsert("Liquid Container", "LContainer_PreIndustrial_StorageJar_30L",
			"An opaque closable pre-industrial storage jar with a 30 litre capacity.",
			LiquidContainerDefinition(30.0, true, 300000));
		Upsert("Liquid Container", "LContainer_PreIndustrial_Vat_125L",
			"An opaque open pre-industrial process vat with a 125 litre capacity.",
			LiquidContainerDefinition(125.0, false, 1250000));
		Upsert("Liquid Container", "LContainer_PreIndustrial_Vat_500L",
			"An opaque open pre-industrial storage or process vat with a 500 litre capacity.",
			LiquidContainerDefinition(500.0, false, 5000000));

		Upsert("LockingContainer", "LockingContainer_PreIndustrial_SmallCabinet",
			"A small opaque cabinet with a built-in warded lock.",
			LockingContainerDefinition(50000, SizeCategory.Small, false, Difficulty.VeryHard, Difficulty.Hard,
				"Ward Lock"));
		Upsert("LockingContainer", "LockingContainer_PreIndustrial_LargeCabinet",
			"A large opaque upright cabinet with a built-in warded lock.",
			LockingContainerDefinition(250000, SizeCategory.Large, false, Difficulty.ExtremelyHard,
				Difficulty.VeryHard, "Ward Lock"));
		Upsert("LockingContainer", "LockingContainer_PreIndustrial_DrawerChest",
			"A large opaque drawer chest secured as one logical inventory by a built-in lock.",
			LockingContainerDefinition(150000, SizeCategory.Normal, false, Difficulty.VeryHard, Difficulty.Hard,
				"Ward Lock"));
		Upsert("LockingContainer", "LockingContainer_PreIndustrial_DisplayCabinet",
			"A transparent display cabinet secured by a built-in lock.",
			LockingContainerDefinition(150000, SizeCategory.Normal, true, Difficulty.VeryHard, Difficulty.Hard,
				"Ward Lock"));
		Upsert("LockingContainer", "LockingContainer_PreIndustrial_Desk",
			"Opaque desk storage secured by a built-in warded lock.",
			LockingContainerDefinition(75000, SizeCategory.Normal, false, Difficulty.VeryHard, Difficulty.Hard,
				"Ward Lock"));
		Upsert("LockingCashRegister", "CashRegister_PreIndustrial_TillChest",
			"A lockable pre-industrial till chest retaining the full shop cash-register workflow.",
			LockingContainerDefinition(50000, SizeCategory.Small, false, Difficulty.VeryHard, Difficulty.Hard,
				"Ward Lock"));

		Upsert("HandTool", "Tool_Artillery_General",
			"A general hand-tool profile for operating and maintaining artillery.", HandToolDefinition());
		Upsert("HandTool", "Tool_CartridgeMaking_General",
			"A general hand-tool profile for measured charges and paper-cartridge production.", HandToolDefinition());
		Upsert("HandTool", "Tool_Gunsmithing_General",
			"A general hand-tool profile for firearm lock, barrel and maintenance work.", HandToolDefinition());

		nextId = currentId;
	}
}

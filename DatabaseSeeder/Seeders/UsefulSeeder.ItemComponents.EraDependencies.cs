#nullable enable

using MudSharp.Database;
using MudSharp.Form.Audio;
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

		XElement InstrumentDefinition(string family, AudioVolume volume, int hands, string useModes,
			double initialStamina, double tickStamina, params string[] styles)
		{
			return new XElement("Definition",
				new XElement("Family", new XCData(family)),
				new XElement("PerformanceTrait", 0),
				new XElement("Difficulty", Difficulty.Normal),
				new XElement("Volume", volume),
				new XElement("RequiredHands", hands),
				new XElement("UseModes", useModes),
				new XElement("InitialStamina", initialStamina),
				new XElement("TickStamina", tickStamina),
				new XElement("TickSeconds", 10),
				new XElement("Positions",
					new XElement("Position", "standing"),
					new XElement("Position", "sitting"),
					new XElement("Position", "kneeling")),
				new XElement("Styles", styles.Select(x => new XElement("Style", new XCData(x)))),
				new XElement("LocalPlayEmote", new XCData("@ begin|begins playing $1.")),
				new XElement("LocalTickEmote", new XCData("@ continue|continues playing $1.")),
				new XElement("DistantPlayEmote", new XCData($"You hear {family.ToLowerInvariant()} music {{0}}.")),
				new XElement("FailureEmote",
					new XCData("@ attempt|attempts to play $1, but produces only a broken phrase.")),
				new XElement("StopEmote", new XCData("@ stop|stops playing $1.")),
				new XElement("CanPlayProg", 0),
				new XElement("WhyCannotPlayProg", 0),
				new XElement("OnPlayProg", 0),
				new XElement("OnStopProg", 0));
		}

		XElement SignalInstrumentDefinition(string family, AudioVolume volume, int hands, string useModes,
			string[] styles, params string[] signals)
		{
			var definition = InstrumentDefinition(family, volume, hands, useModes, 2.0, 1.0, styles);
			definition.Add(
				new XElement("SignalStamina", 5.0),
				new XElement("SignalCooldownSeconds", 10),
				new XElement("Signals", signals.Select(signal =>
					new XElement("Signal", new XAttribute("name", signal),
						new XElement("Local",
							new XCData($"@ sound|sounds the {signal} signal on $1.")),
						new XElement("Distant",
							new XCData($"You hear the {signal} signal sounded {{0}}.")),
						new XElement("Failure",
							new XCData("@ attempt|attempts a signal on $1, but produces only a garbled call."))))),
				new XElement("CanSignalProg", 0),
				new XElement("WhyCannotSignalProg", 0),
				new XElement("OnSignalProg", 0));
			return definition;
		}

		XElement MilitaryStandardDefinition(string family, params string[] signals)
		{
			var displayFamily = family.SplitCamelCase();
			return new XElement("Definition",
				new XElement("Family", family),
				new XElement("IdentityKey", new XCData("unassigned")),
				new XElement("IdentityName", new XCData($"an unassigned {displayFamily.ToLowerInvariant()}")),
				new XElement("Design", new XCData("It bears no assigned insignia or heraldry.")),
				new XElement("AssociationType", "None"),
				new XElement("AssociationKey", string.Empty),
				new XElement("AssociationName", string.Empty),
				new XElement("RecognitionTrait", 0),
				new XElement("RecognitionDifficulty", Difficulty.Normal),
				new XElement("Signals", signals.Select(x => new XElement("Signal", new XCData(x)))),
				new XElement("PlantEmote", new XCData("@ plant|plants $1 firmly in place.")),
				new XElement("TakeUpEmote", new XCData("@ take|takes up $1.")),
				new XElement("RecogniseEmote",
					new XCData("@ study|studies $1 and recognises its identity.")),
				new XElement("CanBearProg", 0),
				new XElement("CanRecogniseProg", 0),
				new XElement("OnRecogniseProg", 0),
				new XElement("OnPlantProg", 0),
				new XElement("OnTakeUpProg", 0),
				new XElement("OnCapturedProg", 0),
				new XElement("OnRecoveredProg", 0),
				new XElement("OnCustodyChangedProg", 0),
				new XElement("OnSignalProg", 0));
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

		foreach (var (name, family, volume, hands, modes, styles) in new[]
		         {
			         ("Instrument_Antiquity_WoodenLyre", "Lyre", AudioVolume.Quiet, 2, "Handheld",
				         new[] { "hymn", "lament", "dance" }),
			         ("Instrument_Antiquity_Kithara", "Kithara", AudioVolume.Decent, 2, "Handheld",
				         new[] { "hymn", "epic", "processional" }),
			         ("Instrument_Antiquity_ReedFlute", "Reed Flute", AudioVolume.Quiet, 2, "Handheld",
				         new[] { "pastoral", "dance", "lament" }),
			         ("Instrument_Antiquity_DoubleAulos", "Double Aulos", AudioVolume.Decent, 2, "Handheld",
				         new[] { "dance", "processional", "martial" }),
			         ("Instrument_Antiquity_FrameDrum", "Frame Drum", AudioVolume.Loud, 1, "Handheld",
				         new[] { "dance", "processional", "ritual" }),
			         ("Instrument_Antiquity_Sistrum", "Sistrum", AudioVolume.Decent, 1, "Handheld",
				         new[] { "ritual", "processional" }),
			         ("Instrument_Antiquity_BronzeWarHorn", "Bronze War Horn", AudioVolume.VeryLoud, 2,
				         "Handheld", new[] { "martial", "alarm" }),
			         ("Instrument_Antiquity_ShipSignalTrumpet", "Ship Signal Trumpet", AudioVolume.VeryLoud, 2,
				         "Handheld", new[] { "maritime", "martial" }),
			         ("Instrument_Antiquity_TempleRitualRattle", "Temple Ritual Rattle", AudioVolume.Decent, 1,
				         "Handheld", new[] { "ritual", "processional" })
		         })
		{
			Upsert("Instrument", name, $"A playable {family.ToLowerInvariant()} profile.",
				InstrumentDefinition(family, volume, hands, modes, 2.0, 1.0, styles));
		}

		Upsert("SignalInstrument", "SignalInstrument_FieldDrum",
			"A wearable field drum for sustained music and named battlefield signals.",
			SignalInstrumentDefinition("Field Drum", AudioVolume.Loud, 2, "Handheld, Worn",
				["march", "cadence"], "attention", "advance", "rally", "withdraw", "cease"));
		Upsert("SignalInstrument", "SignalInstrument_KettleDrum",
			"A room-positioned kettle drum for loud battlefield signals.",
			SignalInstrumentDefinition("Kettle Drum", AudioVolume.VeryLoud, 2, "Room",
				["march", "ceremonial"], "attention", "advance", "rally", "alarm", "cease"));
		Upsert("SignalInstrument", "SignalInstrument_Fife",
			"A military fife for sustained playing and sharp named calls.",
			SignalInstrumentDefinition("Fife", AudioVolume.Loud, 2, "Handheld",
				["march", "ceremonial"], "attention", "advance", "rally", "withdraw", "cease"));
		Upsert("SignalInstrument", "SignalInstrument_SpeakingTrumpet",
			"A speaking trumpet for shipboard and field command signals.",
			SignalInstrumentDefinition("Speaking Trumpet", AudioVolume.VeryLoud, 1, "Handheld",
				["command"], "attention", "advance", "withdraw", "alarm", "cease"));

		foreach (var (name, family, signals) in new[]
		         {
			         ("MilitaryStandard_InfantryColour", "InfantryColour", Array.Empty<string>()),
			         ("MilitaryStandard_CavalryStandard", "CavalryStandard", Array.Empty<string>()),
			         ("MilitaryStandard_Guidon", "Guidon", Array.Empty<string>()),
			         ("MilitaryStandard_NavalEnsign", "NavalEnsign", Array.Empty<string>()),
			         ("MilitaryStandard_Pennant", "Pennant", Array.Empty<string>()),
			         ("MilitaryStandard_SignalFlag", "SignalFlag",
				         new[] { "attention", "advance", "rally", "withdraw", "alarm", "cease" })
		         })
		{
			Upsert("MilitaryStandard", name,
				$"A plantable {family.SplitCamelCase().ToLowerInvariant()} with persistent capture and recovery state.",
				MilitaryStandardDefinition(family, signals));
		}

		nextId = currentId;
	}
}

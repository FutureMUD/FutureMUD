#nullable enable

using MudSharp.Form.Shape;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class UsefulSeeder
{
	internal static IReadOnlyList<string> IndustrialisedPrerequisiteComponentNamesForTesting { get; } =
	[
		"BankPayment_Unlimited",
		"BankPayment_SingleUse",
		"CashRegister_Modern_Standard",
		"Compressor_Workshop",
		"DigitalMediaRecorder_AV_Standard",
		"Dryer_Domestic",
		"Keycard_Blank",
		"KeycardScanner_Standard",
		"KeycardWriter_Standard",
		"PowerBank_USB_C_Standard",
		"PowerTool_Light",
		"PowerTool_Workshop",
		"PowerTool_Heavy",
		"Refrigerator_Domestic",
		"Refrigerator_Commercial",
		"VendingMachine_Standard",
		"WashingMachine_Domestic"
	];

	private void SeedIndustrialisedPrerequisiteComponents(
		FuturemudDatabaseContext context,
		DateTime now,
		Account dbaccount,
		ref long nextId,
		string gasSocketType)
	{
		var allocatedNextId = nextId;
		GameItemComponentProto Upsert(string type, string name, string description, XElement definition)
		{
			return UpsertComponent(context, ref allocatedNextId, dbaccount, now, type, name, description,
				definition.ToString());
		}

		XElement PoweredMachineDefinition(
			double wattage,
			double qualityDiscount,
			bool useMountPower,
			string powerOnEmote,
			string powerOffEmote,
			params object[] extra)
		{
			return new XElement("Definition",
				new XElement("Wattage", wattage),
				new XElement("WattageDiscount", qualityDiscount),
				new XElement("Switchable", true),
				new XElement("UseMountHostPowerSource", useMountPower),
				new XElement("PowerOnEmote", new XCData(powerOnEmote)),
				new XElement("PowerOffEmote", new XCData(powerOffEmote)),
				new XElement("OnPoweredProg", 0),
				new XElement("OnUnpoweredProg", 0),
				extra);
		}

		XElement ContainerDefinition(double weight, SizeCategory maximumSize, bool transparent,
			params object[] extra)
		{
			return new XElement("Definition",
				new XAttribute("Weight", weight),
				new XAttribute("MaxSize", (int)maximumSize),
				new XAttribute("Preposition", "in"),
				new XAttribute("Closable", true),
				new XAttribute("Transparent", transparent),
				new XAttribute("OnceOnly", false),
				new XElement("AllowedTags"),
				new XElement("BlockedTags"),
				extra);
		}

		Upsert("BankPayment", "BankPayment_Unlimited",
			"Turns an item into a reusable bank-account payment credential.",
			new XElement("Definition", new XElement("Uses", 0)));
		Upsert("BankPayment", "BankPayment_SingleUse",
			"Turns an item into a single-use bank-account payment credential.",
			new XElement("Definition", new XElement("Uses", 1)));
		Upsert("CashRegister", "CashRegister_Modern_Standard",
			"Turns an item into a modern shop-aware cash register and till.",
			new XElement("Definition",
				new XAttribute("Weight", 25000.0),
				new XAttribute("MaxSize", (int)SizeCategory.Small)));

		Upsert("PowerTool", "PowerTool_Light",
			"Turns an item into a light powered craft tool with modest consumption and acceleration.",
			new XElement("Definition",
				new XElement("Wattage", 250.0),
				new XElement("MultiplierReductionPerQuality", 0.04),
				new XElement("BaseMultiplier", 1.10),
				new XElement("ToolDurabilitySecondsExpression", "(1+quality) * 5400")));
		Upsert("PowerTool", "PowerTool_Workshop",
			"Turns an item into a general workshop power tool with strong craft acceleration.",
			new XElement("Definition",
				new XElement("Wattage", 800.0),
				new XElement("MultiplierReductionPerQuality", 0.05),
				new XElement("BaseMultiplier", 1.00),
				new XElement("ToolDurabilitySecondsExpression", "(1+quality) * 7200")));
		Upsert("PowerTool", "PowerTool_Heavy",
			"Turns an item into a heavy industrial power tool with high consumption and durable operation.",
			new XElement("Definition",
				new XElement("Wattage", 2200.0),
				new XElement("MultiplierReductionPerQuality", 0.05),
				new XElement("BaseMultiplier", 0.95),
				new XElement("ToolDurabilitySecondsExpression", "(2+quality) * 10800")));

		Upsert("Compressor", "Compressor_Workshop",
			"Turns an item into a powered workshop compressor for standard gas connectors.",
			PoweredMachineDefinition(1500.0, 40.0, false,
				"@ rattle|rattles into life as its compressor cycle begins.",
				"@ wind|winds down as its compressor stops.",
				new XElement("Connectors",
					new XElement("Connection", new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", gasSocketType), new XAttribute("powered", false)),
					new XElement("Connection", new XAttribute("gender", (short)Gender.Male),
						new XAttribute("type", gasSocketType), new XAttribute("powered", false))),
				new XElement("FlowRate", 210.0)));

		Upsert("Refrigerator", "Refrigerator_Domestic",
			"Turns an item into a powered domestic refrigerator that slows freshness and biological timers.",
			ContainerDefinition(100000.0, SizeCategory.Normal, false,
				new XElement("PowerUsageInWatts", 150.0),
				new XElement("PoweredClosedRate", 0.10),
				new XElement("PoweredOpenRate", 0.50),
				new XElement("UnpoweredClosedRate", 0.75),
				new XElement("UnpoweredOpenRate", 1.0)));
		Upsert("Refrigerator", "Refrigerator_Commercial",
			"Turns an item into a high-capacity commercial refrigerator with improved powered preservation.",
			ContainerDefinition(500000.0, SizeCategory.Large, false,
				new XElement("PowerUsageInWatts", 650.0),
				new XElement("PoweredClosedRate", 0.05),
				new XElement("PoweredOpenRate", 0.40),
				new XElement("UnpoweredClosedRate", 0.70),
				new XElement("UnpoweredOpenRate", 1.0)));
		Upsert("Dryer", "Dryer_Domestic",
			"Turns an item into a powered domestic tumble dryer that accelerates surface-liquid drying.",
			ContainerDefinition(10000.0, SizeCategory.Normal, true,
				new XElement("PowerUsageInWatts", 2000.0),
				new XElement("DryingMultiplier", 10.0)));

		Upsert("Digital Media Recorder", "DigitalMediaRecorder_AV_Standard",
			"Turns an item into a powered audio-visual recorder with local internal storage.",
			PoweredMachineDefinition(20.0, 0.5, false,
				"@ chime|chimes as its recorder powers on.",
				"@ click|clicks off as its recorder powers down.",
				new XElement("StorageCapacityInBytes", 8_000_000_000L),
				new XElement("StoragePorts", 1),
				new XElement("TerminalPorts", 0),
				new XElement("NetworkPorts", 1),
				new XElement("Capabilities", "Audio, Video"),
				new XElement("EndpointKey", "recorder"),
				new XElement("InputName", "camera"),
				new XElement("OutputName", "playback")));

		var usbInput = new XElement("Connection", new XAttribute("gender", (short)Gender.Female),
			new XAttribute("type", "USB-C"));
		var usbOutput = new XElement("Connection", new XAttribute("gender", (short)Gender.Female),
			new XAttribute("type", "USB-C"));
		Upsert("PowerBank", "PowerBank_USB_C_Standard",
			"Turns an item into a rechargeable USB-C power bank with bounded input and output.",
			new XElement("Definition",
				new XElement("Connectors",
					new XElement("Connection", new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "USB-C"), new XAttribute("powered", true)),
					new XElement("Connection", new XAttribute("gender", (short)Gender.Female),
						new XAttribute("type", "USB-C"), new XAttribute("powered", true))),
				new XElement("CapacityInWattHours", 40.0),
				new XElement("MaximumInputInWatts", 18.0),
				new XElement("MaximumOutputInWatts", 18.0),
				new XElement("ChargingEfficiency", 0.90),
				new XElement("InputConnectors", usbInput),
				new XElement("OutputConnectors", usbOutput)));

		Upsert("Keycard", "Keycard_Blank",
			"Turns an item into a programmable electronic keycard with no initial access codes.",
			new XElement("Definition", new XElement("Codes")));
		Upsert("KeycardScanner", "KeycardScanner_Standard",
			"Turns an item into a powered keycard reader that emits a short successful-access signal.",
			PoweredMachineDefinition(35.0, 1.0, true,
				"@ light|lights its ready indicator.",
				"@ dim|dims as its reader powers down.",
				new XElement("SignalValue", 1.0),
				new XElement("SignalDurationSeconds", 3.0),
				new XElement("SelfTargetLockPrototypeId", 0),
				new XElement("SelfTargetLockPrototypeName", new XCData(string.Empty)),
				new XElement("Codes")));
		Upsert("KeycardWriter", "KeycardWriter_Standard",
			"Turns an item into a powered machine for programming electronic keycards.",
			PoweredMachineDefinition(50.0, 1.0, true,
				"@ hum|hums as its keycard writer becomes ready.",
				"@ click|clicks off as its keycard writer powers down."));

		Upsert("WashingMachine", "WashingMachine_Domestic",
			"Turns an item into a powered domestic washing machine with a locking transparent door.",
			new XElement("Definition",
				new XElement("WeightCapacity", 7500.0),
				new XElement("WashingLiquidCapacity", 100.0),
				new XElement("WashingPowderCapacity", 100.0),
				new XElement("PowerUsageInWatts", 500.0),
				new XElement("MaximumItemSize", (int)SizeCategory.Normal),
				new XElement("DoorLock", true),
				new XElement("Transparent", true),
				new XElement("NormalCycleTime", 90.0)));
		Upsert("Vending Machine", "VendingMachine_Standard",
			"Turns an item into a configurable modern vending machine and money-accepting container.",
			new XElement("Definition",
				new XAttribute("Weight", 100000.0),
				new XAttribute("MaxSize", (int)SizeCategory.Normal),
				new XElement("InsertMoneyEmote", new XCData("$0 insert|inserts $1 into $2.")),
				new XElement("RefundMoneyEmote", new XCData("$0 select|selects refund on $1, and $2 drops into the delivery pan.")),
				new XElement("ItemSelectedEmote", new XCData("$0 select|selects the {0} option on $1, and $2 drops into the delivery pan.")),
				new XElement("InvalidItemSelectedEmote", new XCData("$0 select|selects the {0} option on $1, but nothing is dispensed."))));
		nextId = allocatedNextId;
	}
}

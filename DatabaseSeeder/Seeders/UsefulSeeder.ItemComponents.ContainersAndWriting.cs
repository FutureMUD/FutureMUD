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
    private void SeedModernItems(FuturemudDatabaseContext context, ICollection<string> errors)
    {
        DateTime now = DateTime.UtcNow;
        Account dbaccount = context.Accounts.First();
        long nextId = context.GameItemComponentProtos.Any() ? context.GameItemComponentProtos.Max(x => x.Id) + 1 : 1;
        string mainsSocketType = context.StaticConfigurations
            .FirstOrDefault(x => x.SettingName == "DefaultPowerSocketType")
            ?.Definition ?? "NEMA 5-15";
        string gasSocketType = context.StaticConfigurations
            .FirstOrDefault(x => x.SettingName == "DefaultGasSocketType")
            ?.Definition ?? "BSP 5mm";
        string externalVenousConnector = context.StaticConfigurations
            .FirstOrDefault(x => x.SettingName == "DefaultExternalOrganVenousConnector")
            ?.Definition ?? "2-LargeVenousCatheter";
        string externalArterialConnector = context.StaticConfigurations
            .FirstOrDefault(x => x.SettingName == "DefaultExternalOrganArterialConnector")
            ?.Definition ?? "2-LargeArterialCatheter";
        Tag? fuelTag = context.Tags.FirstOrDefault(x => x.Name == "Fuel");

        GameItemComponentProto CreateModernComponent(string type, string name, string description, XElement definition)
        {
            return CreateComponent(context, ref nextId, dbaccount, now, type, name, description, definition.ToString());
        }

        void CreateBattery(string batteryType, double wattHours, double wattHoursPerQuality, bool rechargeable)
        {
            string name = rechargeable ? $"Battery_{batteryType}_Rechargeable" : $"Battery_{batteryType}";
            string description = rechargeable
                ? $"Turns an item into a rechargeable {batteryType} battery."
                : $"Turns an item into a disposable {batteryType} battery.";
            CreateModernComponent("Battery", name, description,
                new XElement("Definition",
                    new XElement("BatteryType", new XCData(batteryType)),
                    new XElement("BaseWattHours", wattHours),
                    new XElement("WattHoursPerQuality", wattHoursPerQuality),
                    new XElement("Rechargable", rechargeable)
                ));
        }

        void CreateBatteryPowered(string batteryType, int quantity, bool inSeries, bool transparent = false, string preposition = "in")
        {
            CreateModernComponent("BatteryPowered", $"BatteryPowered_{quantity}x{batteryType}",
                $"Turns an item into a {quantity}x {batteryType} battery powered device.",
                new XElement("Definition",
                    new XElement("BatteryType", batteryType),
                    new XElement("BatteryQuantity", quantity),
                    new XElement("BatteriesInSeries", inSeries),
                    new XElement("Transparent", transparent),
                    new XElement("ContentsPreposition", preposition)
                ));
        }

        void CreateBatteryCharger(string batteryType, int quantity, double wattage, double efficiency, bool transparent = true, string? suffix = null)
        {
            string bayName = suffix ?? $"{quantity}Bay";
            CreateModernComponent("BatteryCharger", $"BatteryCharger_{batteryType}_{bayName}",
                $"Turns an item into a charger for {quantity} {batteryType} batter{(quantity == 1 ? "y" : "ies")} at a time.",
                new XElement("Definition",
                    new XElement("BatteryType", batteryType),
                    new XElement("BatteryQuantity", quantity),
                    new XElement("Wattage", wattage),
                    new XElement("Efficiency", efficiency),
                    new XElement("ContentsPreposition", "in"),
                    new XElement("Transparent", transparent)
                ));
        }

        XElement ConnectorDefinition(params ConnectorType[] connectors)
        {
            return new XElement("Definition",
                new XElement("Connectors",
                    from connector in connectors
                    select new XElement("Connection",
                        new XAttribute("gender", (short)connector.Gender),
                        new XAttribute("type", connector.ConnectionType),
                        new XAttribute("powered", connector.Powered)
                    )));
        }

        XElement ThermalDefinition(double ambientHeat, double intimateHeat, double immediateHeat, double proximateHeat,
            double distantHeat, double veryDistantHeat, string activeDescription, string inactiveDescription,
            params object[] extraElements)
        {
            return new XElement("Definition",
                new XElement("AmbientHeat", ambientHeat),
                new XElement("IntimateHeat", intimateHeat),
                new XElement("ImmediateHeat", immediateHeat),
                new XElement("ProximateHeat", proximateHeat),
                new XElement("DistantHeat", distantHeat),
                new XElement("VeryDistantHeat", veryDistantHeat),
                new XElement("ActiveDescriptionAddendum", new XCData(activeDescription)),
                new XElement("InactiveDescriptionAddendum", new XCData(inactiveDescription)),
                extraElements);
        }

        XElement SwitchableThermalDefinition(double ambientHeat, double intimateHeat, double immediateHeat,
            double proximateHeat, double distantHeat, double veryDistantHeat, string activeDescription,
            string inactiveDescription, string switchOnEmote, string switchOffEmote, params object[] extraElements)
        {
            XElement definition = ThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
                veryDistantHeat, activeDescription, inactiveDescription, extraElements);
            definition.Add(
                new XElement("SwitchOnEmote", new XCData(switchOnEmote)),
                new XElement("SwitchOffEmote", new XCData(switchOffEmote)));
            return definition;
        }

        void CreateElectricHeaterCooler(string name, string description, double ambientHeat, double intimateHeat,
            double immediateHeat, double proximateHeat, double distantHeat, double veryDistantHeat, double wattage,
            string activeDescription, string inactiveDescription, string switchOnEmote, string switchOffEmote)
        {
            CreateModernComponent("ElectricHeaterCooler", name, description,
                SwitchableThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
                    veryDistantHeat, activeDescription, inactiveDescription, switchOnEmote, switchOffEmote,
                    new XElement("Wattage", wattage)));
        }

        void CreateConsumableHeaterCooler(string name, string description, double ambientHeat, double intimateHeat,
            double immediateHeat, double proximateHeat, double distantHeat, double veryDistantHeat, int secondsOfFuel,
            string activeDescription, string inactiveDescription, string fuelExpendedEcho)
        {
            CreateModernComponent("ConsumableHeaterCooler", name, description,
                ThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat, veryDistantHeat,
                    activeDescription, inactiveDescription,
                    new XElement("SecondsOfFuel", secondsOfFuel),
                    new XElement("SpentItemProto", 0),
                    new XElement("FuelExpendedEcho", new XCData(fuelExpendedEcho))));
        }

        void CreateSolidFuelHeaterCooler(string name, string description, double ambientHeat, double intimateHeat,
            double immediateHeat, double proximateHeat, double distantHeat, double veryDistantHeat,
            double maximumFuelWeight, double secondsPerUnitWeight, string activeDescription,
            string inactiveDescription, string switchOnEmote, string switchOffEmote)
        {
            CreateModernComponent("SolidFuelHeaterCooler", name, description,
                SwitchableThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
                    veryDistantHeat, activeDescription, inactiveDescription, switchOnEmote, switchOffEmote,
                    new XElement("FuelTag", fuelTag?.Id ?? 0),
                    new XElement("MaximumFuelWeight", maximumFuelWeight),
                    new XElement("SecondsPerUnitWeight", secondsPerUnitWeight)));
        }

        void CreateLiquidFuelHeaterCooler(string variantName, MudSharp.Models.Liquid liquid, string description,
            double ambientHeat,
            double intimateHeat, double immediateHeat, double proximateHeat, double distantHeat,
            double veryDistantHeat, double fuelPerSecond, string activeDescription, string inactiveDescription,
            string switchOnEmote, string switchOffEmote)
        {
            string safeFuelName = SanitizeComponentName(liquid.Name);
            CreateModernComponent("FuelHeaterCooler", $"FuelHeaterCooler_{variantName}_{safeFuelName}", description,
                SwitchableThermalDefinition(ambientHeat, intimateHeat, immediateHeat, proximateHeat, distantHeat,
                    veryDistantHeat, activeDescription, inactiveDescription, switchOnEmote, switchOffEmote,
                    new XElement("FuelMedium", 0),
                    new XElement("LiquidFuel", liquid.Id),
                    new XElement("GasFuel", 0),
                    new XElement("FuelPerSecond", fuelPerSecond),
                    new XElement("Connector",
                        new XAttribute("gender", (short)Gender.Male),
                        new XAttribute("type", "LiquidLine"),
                        new XAttribute("powered", false))));
        }

        void CreatePowerSocket(string name, int count)
        {
            CreateModernComponent("PowerSocket", name,
                $"Turns an item into a {count}-socket mains power outlet.",
                ConnectorDefinition(
                    Enumerable.Range(0, count)
                        .Select(_ => new ConnectorType(Gender.Female, mainsSocketType, true))
                        .ToArray()));
        }

        void CreatePowerSupply(double wattage)
        {
            CreateModernComponent("PowerSupply", $"PowerSupply_{wattage:N0}W",
                $"Turns an item into a mains-powered device that draws {wattage:N0}W.",
                new XElement("Definition",
                    new XElement("Wattage", wattage)));
        }

        XElement ConnectorsElement(params ConnectorType[] connectors)
        {
            return new XElement("Connectors",
                from connector in connectors
                select new XElement("Connection",
                    new XAttribute("gender", (short)connector.Gender),
                    new XAttribute("type", connector.ConnectionType),
                    new XAttribute("powered", connector.Powered)
                ));
        }

        XElement PoweredMachineDefinition(double wattage, double wattageDiscount, bool switchable,
            bool useMountHostPowerSource, string powerOnEmote, string powerOffEmote, params object[] extraElements)
        {
            return new XElement("Definition",
                new XElement("Wattage", wattage),
                new XElement("WattageDiscount", wattageDiscount),
                new XElement("Switchable", switchable),
                new XElement("UseMountHostPowerSource", useMountHostPowerSource),
                new XElement("PowerOnEmote", new XCData(powerOnEmote)),
                new XElement("PowerOffEmote", new XCData(powerOffEmote)),
                new XElement("OnPoweredProg", 0),
                new XElement("OnUnpoweredProg", 0),
                extraElements);
        }

        Gas? FindGas(params string[] candidates)
        {
            return context.Gases.AsEnumerable()
                       .FirstOrDefault(x => candidates.Any(candidate =>
                           x.Name.Equals(candidate, StringComparison.OrdinalIgnoreCase))) ??
                   context.Gases.AsEnumerable()
                       .FirstOrDefault(x => candidates.Any(candidate =>
                           x.Name.Contains(candidate, StringComparison.OrdinalIgnoreCase)));
        }

        BodyProto? FindHumanoidExternalOrganBody()
        {
            return context.BodyProtos.FirstOrDefault(x => x.Name == "Organic Humanoid") ??
                   context.BodyProtos.FirstOrDefault(x => x.Name == "Humanoid");
        }

        List<BodypartProto> FindExternalOrgans(BodyProto body, params BodypartTypeEnum[] organTypes)
        {
            HashSet<int> desiredTypes = organTypes.Select(x => (int)x).ToHashSet();
            return context.BodypartProtos
                .Where(x => x.BodyId == body.Id)
                .Where(x => x.IsOrgan != 0)
                .Where(x => desiredTypes.Contains(x.BodypartType))
                .OrderBy(x => x.Id)
                .ToList();
        }

        string SanitizeComponentName(string text)
        {
            return new string(text
                .Select(x => char.IsLetterOrDigit(x) ? x : '_')
                .ToArray())
                .Trim('_')
                .Replace("__", "_");
        }

        CreateBattery("A", 6.0, 0.35, false);
        CreateBattery("A", 5.2, 0.3, true);
        CreateBattery("AA", 2.4, 0.12, false);
        CreateBattery("AA", 2.0, 0.1, true);
        CreateBattery("AAA", 1.2, 0.08, false);
        CreateBattery("AAA", 0.95, 0.06, true);
        CreateBattery("C", 8.0, 0.45, false);
        CreateBattery("C", 6.8, 0.35, true);
        CreateBattery("D", 18.0, 0.9, false);
        CreateBattery("D", 14.0, 0.7, true);
        CreateBattery("9V", 4.5, 0.25, false);
        CreateBattery("9V", 3.8, 0.2, true);
        CreateBattery("ButtonCell", 0.22, 0.015, false);
        CreateBattery("ButtonCell", 0.18, 0.01, true);
        CreateBattery("CarBattery", 480.0, 20.0, false);
        CreateBattery("CarBattery", 420.0, 15.0, true);
        CreateModernComponent("Battery", "Battery_LiIon_Cell",
            "Turns an item into a rechargeable lithium-ion cell battery.",
            new XElement("Definition",
                new XElement("BatteryType", new XCData("Li-Ion")),
                new XElement("BaseWattHours", 12.0),
                new XElement("WattHoursPerQuality", 1.2),
                new XElement("Rechargable", true)));
        CreateModernComponent("Battery", "Battery_LiIon_Pack",
            "Turns an item into a rechargeable lithium-ion battery pack for larger electronics.",
            new XElement("Definition",
                new XElement("BatteryType", new XCData("Li-Ion")),
                new XElement("BaseWattHours", 60.0),
                new XElement("WattHoursPerQuality", 6.0),
                new XElement("Rechargable", true)));

        CreateBatteryPowered("ButtonCell", 1, true);
        CreateBatteryPowered("ButtonCell", 2, true);
        CreateBatteryPowered("9V", 1, true);
        CreateBatteryPowered("9V", 2, true);
        CreateBatteryPowered("AAA", 1, true);
        CreateBatteryPowered("AAA", 2, true);
        CreateBatteryPowered("AAA", 3, true);
        CreateBatteryPowered("AAA", 4, true);
        CreateBatteryPowered("AA", 1, true);
        CreateBatteryPowered("AA", 2, true);
        CreateBatteryPowered("AA", 4, true);
        CreateBatteryPowered("AA", 6, true);
        CreateBatteryPowered("AA", 8, true);
        CreateBatteryPowered("A", 1, true);
        CreateBatteryPowered("A", 2, true);
        CreateBatteryPowered("A", 4, true);
        CreateBatteryPowered("C", 2, true);
        CreateBatteryPowered("C", 4, true);
        CreateBatteryPowered("D", 2, true);
        CreateBatteryPowered("D", 4, true);
        CreateBatteryPowered("D", 6, true);
        CreateBatteryPowered("CarBattery", 1, true, false, "in");
        CreateModernComponent("BatteryPowered", "BatteryPowered_1xLiIon",
            "Turns an item into a single-cell lithium-ion battery powered device.",
            new XElement("Definition",
                new XElement("BatteryType", "Li-Ion"),
                new XElement("BatteryQuantity", 1),
                new XElement("BatteriesInSeries", true),
                new XElement("ChargeWattage", 30.0),
                new XElement("Transparent", false),
                new XElement("ContentsPreposition", "in"),
                ConnectorsElement(
                    new ConnectorType(Gender.Female, "USB-C", true))));
        CreateModernComponent("BatteryPowered", "BatteryPowered_2xLiIon",
            "Turns an item into a dual-cell lithium-ion battery powered device.",
            new XElement("Definition",
                new XElement("BatteryType", "Li-Ion"),
                new XElement("BatteryQuantity", 2),
                new XElement("BatteriesInSeries", true),
                new XElement("ChargeWattage", 65.0),
                new XElement("Transparent", false),
                new XElement("ContentsPreposition", "in"),
                ConnectorsElement(
                    new ConnectorType(Gender.Female, "USB-C", true))));

        CreateBatteryCharger("ButtonCell", 2, 1.0, 0.82);
        CreateBatteryCharger("9V", 2, 8.0, 0.85);
        CreateBatteryCharger("AAA", 2, 4.0, 0.88);
        CreateBatteryCharger("AAA", 4, 8.0, 0.9);
        CreateBatteryCharger("AAA", 8, 12.0, 0.92);
        CreateBatteryCharger("AA", 2, 6.0, 0.88);
        CreateBatteryCharger("AA", 4, 12.0, 0.9);
        CreateBatteryCharger("AA", 8, 18.0, 0.92);
        CreateBatteryCharger("A", 4, 14.0, 0.88);
        CreateBatteryCharger("C", 4, 30.0, 0.87, false);
        CreateBatteryCharger("D", 4, 50.0, 0.86, false);
        CreateBatteryCharger("CarBattery", 1, 180.0, 0.84, false, "Workshop");
        CreateModernComponent("BatteryCharger", "BatteryCharger_LiIon_Single",
            "Turns an item into a single-bay lithium-ion battery charger.",
            new XElement("Definition",
                new XElement("BatteryType", "Li-Ion"),
                new XElement("BatteryQuantity", 1),
                new XElement("Wattage", 45.0),
                new XElement("Efficiency", 0.92),
                new XElement("ContentsPreposition", "in"),
                new XElement("Transparent", false)));
        CreateModernComponent("BatteryCharger", "BatteryCharger_LiIon_Quad",
            "Turns an item into a four-bay lithium-ion battery charger.",
            new XElement("Definition",
                new XElement("BatteryType", "Li-Ion"),
                new XElement("BatteryQuantity", 4),
                new XElement("Wattage", 180.0),
                new XElement("Efficiency", 0.94),
                new XElement("ContentsPreposition", "in"),
                new XElement("Transparent", false)));

        CreateModernComponent("Connectable", "Connectable_Male_To_MainsPlug",
            "Turns an item into a male connection to a standard mains plug.",
            ConnectorDefinition(new ConnectorType(Gender.Male, mainsSocketType, true)));
        CreateModernComponent("Connectable", "Connectable_MainsPlug_PassThrough",
            "Turns an item into a standard mains plug with a pass-through socket, like an extension lead.",
            ConnectorDefinition(
                new ConnectorType(Gender.Male, mainsSocketType, true),
                new ConnectorType(Gender.Female, mainsSocketType, true)));
        CreateModernComponent("Connectable", "Connectable_SingleFemale",
            "Turns an item into a female connection with a single female plug.",
            ConnectorDefinition(new ConnectorType(Gender.Female, mainsSocketType, true)));
        CreateModernComponent("Attachable Connectable", "AttachableConnectable_PowerLead",
            "Turns an item into an attachable mains lead or detachable power cable.",
            new XElement("Definition",
                new XElement("Connector",
                    new XAttribute("gender", (short)Gender.Neuter),
                    new XAttribute("type", "PowerLead"))));
        CreatePowerSocket("PowerSocket_Mains_Single", 1);
        CreatePowerSocket("PowerSocket_Mains_Double", 2);
        CreatePowerSocket("PowerSocket_Mains_Quad", 4);

        foreach (double wattage in new[] { 5.0, 15.0, 30.0, 60.0, 100.0, 250.0, 500.0, 1000.0, 1500.0, 2400.0 })
        {
            CreatePowerSupply(wattage);
        }

        foreach ((string Name, int Count, string Description) outlet in new[]
                 {
                     ("ElectricGridOutlet_Single", 1, "Turns an item into a single-socket electrical grid outlet."),
                     ("ElectricGridOutlet_Double", 2, "Turns an item into a double-socket electrical grid outlet."),
                     ("ElectricGridOutlet_Quad", 4, "Turns an item into a quad-socket electrical grid outlet.")
                 })
        {
            CreateModernComponent("ElectricGridOutlet", outlet.Name, outlet.Description,
                new XElement("Definition",
                    ConnectorsElement(
                        Enumerable.Range(0, outlet.Count)
                            .Select(_ => new ConnectorType(Gender.Female, mainsSocketType, true))
                            .ToArray())));
        }

        CreateModernComponent("GridPowerSupply", "GridPowerSupply_Standard",
            "Turns an item into a standard grid power tap for ordinary appliances.",
            new XElement("Definition",
                new XElement("Wattage", 240.0)));
        CreateModernComponent("GridPowerSupply", "GridPowerSupply_HighDraw",
            "Turns an item into a high-draw grid power tap for heavier equipment.",
            new XElement("Definition",
                new XElement("Wattage", 2400.0)));
        CreateModernComponent("UnlimitedGenerator", "UnlimitedGenerator_SetPiece",
            "Turns an item into a set-piece generator that provides a large stable power feed.",
            new XElement("Definition",
                new XElement("SwitchOnEmote", new XCData("@ thrum|thrums to life and begin|begins feeding power.")),
                new XElement("SwitchOffEmote", new XCData("@ wind|winds down and stop|stops feeding power.")),
                new XElement("WattageProvided", 5000.0),
                new XElement("SwitchOnProg", 0),
                new XElement("SwitchOffProg", 0)));
        CreateModernComponent("UnlimitedGenerator", "UnlimitedGenerator_Admin",
            "Turns an item into an effectively unlimited administrative power source.",
            new XElement("Definition",
                new XElement("SwitchOnEmote", new XCData("@ surge|surges with inexhaustible power.")),
                new XElement("SwitchOffEmote", new XCData("@ fall|falls quiet as its impossible power feed ceases.")),
                new XElement("WattageProvided", 1000000.0),
                new XElement("SwitchOnProg", 0),
                new XElement("SwitchOffProg", 0)));

        CreateModernComponent("ElectricLight", "ElectricLight_Low",
            "Turns an item into a low power electric light source.",
            new XElement("Definition",
                new XElement("IlluminationProvided", 40),
                new XElement("Wattage", 5),
                new XElement("OnLightProg", 0),
                new XElement("OnOffProg", 0),
                new XElement("LightOnEmote", new XCData("@ glow|glows with a soft light.")),
                new XElement("LightOffEmote", new XCData("@ go|goes dark."))));
        CreateModernComponent("ElectricLight", "ElectricLight_Medium",
            "Turns an item into a medium brightness electric light source.",
            new XElement("Definition",
                new XElement("IlluminationProvided", 180),
                new XElement("Wattage", 15),
                new XElement("OnLightProg", 0),
                new XElement("OnOffProg", 0),
                new XElement("LightOnEmote", new XCData("@ light|lights up.")),
                new XElement("LightOffEmote", new XCData("@ go|goes dark."))));
        CreateModernComponent("ElectricLight", "ElectricLight_Bright",
            "Turns an item into a bright electric floodlight.",
            new XElement("Definition",
                new XElement("IlluminationProvided", 800),
                new XElement("Wattage", 60),
                new XElement("OnLightProg", 0),
                new XElement("OnOffProg", 0),
                new XElement("LightOnEmote", new XCData("@ flare|flares to life.")),
                new XElement("LightOffEmote", new XCData("@ dim|dims and go|goes dark."))));

        CreateModernComponent("PoweredProp", "PoweredProp_Switchable",
            "Turns an item into a general-purpose switchable powered prop or appliance.",
            new XElement("Definition",
                new XElement("Wattage", 250),
                new XElement("WattageDiscount", 12),
                new XElement("Switchable", true),
                new XElement("PowerOnEmote", new XCData("@ hum|hums as it powers on.")),
                new XElement("PowerOffEmote", new XCData("@ wind|winds down and power|powers off.")),
                new XElement("OnPoweredProg", 0),
                new XElement("OnUnpoweredProg", 0),
                new XElement("TenSecondProg", 0)));
        CreateModernComponent("PoweredProp", "PoweredProp_AlwaysOn",
            "Turns an item into an always-on powered prop such as signage or infrastructure.",
            new XElement("Definition",
                new XElement("Wattage", 40),
                new XElement("WattageDiscount", 5),
                new XElement("Switchable", false),
                new XElement("PowerOnEmote", new XCData("@ click|clicks softly as it powers up.")),
                new XElement("PowerOffEmote", new XCData("@ fall|falls silent as power is lost.")),
                new XElement("OnPoweredProg", 0),
                new XElement("OnUnpoweredProg", 0),
                new XElement("TenSecondProg", 0)));

        CreateModernComponent("HandheldRadio", "HandheldRadio_Standard",
            "Turns an item into a battery-powered handheld two-way radio.",
            new XElement("Definition",
                new XElement("WattageIdle", 1.5),
                new XElement("WattageTransmit", 80.0),
                new XElement("WattageReceive", 18.0),
                new XElement("BroadcastRange", 6000.0),
                new XElement("OnPowerOffEmote", new XCData("@ give|gives a small burst of static and power|powers down.")),
                new XElement("OnPowerOnEmote", new XCData("@ crackle|crackles to life.")),
                new XElement("TransmitPremote", new XCData("@ key|keys the transmitter on $1 and say|says")),
                new XElement("Channel", 476.525),
                new XElement("Channel", 477.125),
                new XElement("ChannelName", new XCData("Operations")),
                new XElement("ChannelName", new XCData("Security"))));
        CreateModernComponent("EarpieceRadio", "EarpieceRadio_Covert",
            "Turns an item into a covert receive-only earpiece radio.",
            new XElement("Definition",
                new XElement("WattageIdle", 0.25),
                new XElement("WattageReceive", 3.0),
                new XElement("OnPowerOffEmote", new XCData("@ click|clicks off.")),
                new XElement("OnPowerOnEmote", new XCData("@ emit|emits a brief burst of static.")),
                new XElement("Channel", 476.525),
                new XElement("Channel", 477.125),
                new XElement("ChannelName", new XCData("Operations")),
                new XElement("ChannelName", new XCData("Security"))));
        CreateModernComponent("ListeningBug", "ListeningBug_Covert",
            "Turns an item into a covert powered listening bug.",
            new XElement("Definition",
                new XElement("BroadcastFrequency", 433.920),
                new XElement("BroadcastRange", 800.0),
                new XElement("ListenSkillPerQuality", 4.0),
                new XElement("BaseListenSkill", 45.0),
                new XElement("PowerConsumptionInWatts", 0.0015)));
        CreateModernComponent("ElectricGridCreator", "ElectricGridCreator_Standard",
            "Turns an item into a creator for an electrical grid.",
            new XElement("Definition"));
        CreateModernComponent("LiquidGridCreator", "LiquidGridCreator_Standard",
            "Turns an item into a creator for a liquid grid.",
            new XElement("Definition"));
        CreateModernComponent("TelecommunicationsGridCreator", "TelecommunicationsGridCreator_Standard",
            "Turns an item into a creator for a telecommunications grid.",
            new XElement("Definition",
                new XElement("Prefix", "555"),
                new XElement("NumberLength", 6)));
        CreateModernComponent("Telephone", "Telephone_Standard",
            "Turns an item into a standard telephone.",
            ConnectorDefinition(
                new ConnectorType(Gender.Male, "TelephoneLine", true)));
        CreateModernComponent("CellularPhone", "CellularPhone_Standard",
            "Turns an item into a standard mobile phone that relies on cell tower coverage.",
            new XElement("Definition",
                new XElement("Wattage", 2.0),
                new XElement("RingEmote", new XCData("@ chirp|chirps insistently.")),
                new XElement("TransmitPremote", new XCData("@ speak|speaks into $1 and say|says")),
                new XElement("RingVolume", (int)AudioVolume.Decent)));
        CreateModernComponent("CellularPhone", "CellularPhone_Smartphone",
            "Turns an item into a smartphone-style cellular handset.",
            new XElement("Definition",
                new XElement("Wattage", 4.0),
                new XElement("RingEmote", new XCData("@ vibrate|vibrates and play|plays a gentle chime.")),
                new XElement("TransmitPremote", new XCData("@ speak|speaks into $1 and say|says")),
                new XElement("RingVolume", (int)AudioVolume.Quiet)));
        CreateModernComponent("CellPhoneTower", "CellPhoneTower_Local",
            "Turns an item into a local cellular tower that serves its surrounding zone.",
            new XElement("Definition",
                new XElement("Wattage", 250.0)));
        CreateModernComponent("CellPhoneTower", "CellPhoneTower_Regional",
            "Turns an item into a higher-power regional cellular tower.",
            new XElement("Definition",
                new XElement("Wattage", 1200.0)));
        CreateModernComponent("AnsweringMachine", "AnsweringMachine_Standard",
            "Turns an item into a daisy-chain answering machine with a standard ring delay.",
            new XElement("Definition",
                new XElement("Wattage", 6.0),
                new XElement("RingEmote", new XCData("@ ring|rings insistently.")),
                new XElement("TransmitPremote", new XCData("@ speak|speaks into $1 and say|says")),
                new XElement("RingVolume", (int)AudioVolume.Loud),
                new XElement("DefaultAutoAnswerRings", 4),
                ConnectorsElement(
                    new ConnectorType(Gender.Male, "TelephoneLine", true),
                    new ConnectorType(Gender.Female, "TelephoneLine", true))));
        CreateModernComponent("Tape", "Tape_Cassette30",
            "Turns an item into a standard thirty-minute cassette tape.",
            new XElement("Definition",
                new XElement("CapacityMs", (long)TimeSpan.FromMinutes(30).TotalMilliseconds)));
        CreateModernComponent("Tape", "Tape_Microcassette10",
            "Turns an item into a compact ten-minute microcassette.",
            new XElement("Definition",
                new XElement("CapacityMs", (long)TimeSpan.FromMinutes(10).TotalMilliseconds)));

        GameItemComponentProto computerHostPersonal = CreateModernComponent("Computer Host", "ComputerHost_Personal",
            "Turns an item into a personal computer host with modest internal storage.",
            PoweredMachineDefinition(120.0, 12.0, true, false,
                "@ hum|hums to life as it boots.",
                "@ click|clicks down and goes dark.",
                new XElement("StorageCapacityInBytes", 1_048_576L),
                new XElement("StoragePorts", 2),
                new XElement("TerminalPorts", 1),
                new XElement("NetworkPorts", 1)));
        CreateModernComponent("Computer Host", "ComputerHost_Server",
            "Turns an item into a larger server-style computer host.",
            PoweredMachineDefinition(420.0, 20.0, true, false,
                "@ thrum|thrums steadily as the server rack powers up.",
                "@ wind|winds down in a descending fan whine.",
                new XElement("StorageCapacityInBytes", 16_777_216L),
                new XElement("StoragePorts", 8),
                new XElement("TerminalPorts", 8),
                new XElement("NetworkPorts", 4)));
        CreateModernComponent("Computer Storage", "ComputerStorage_Portable",
            "Turns an item into portable removable computer storage.",
            new XElement("Definition",
                new XElement("StorageCapacityInBytes", 4_194_304L)));
        CreateModernComponent("Computer Storage", "ComputerStorage_Fixed",
            "Turns an item into higher-capacity fixed computer storage.",
            new XElement("Definition",
                new XElement("StorageCapacityInBytes", 67_108_864L)));
        CreateModernComponent("Computer Terminal", "ComputerTerminal_Desk",
            "Turns an item into a standard desk terminal for a computer host.",
            PoweredMachineDefinition(85.0, 8.0, true, false,
                "@ flicker|flickers to life as the screen powers on.",
                "@ dim|dims to black as it powers off."));
        CreateModernComponent("Computer Terminal", "ComputerTerminal_Kiosk",
            "Turns an item into a public kiosk terminal.",
            PoweredMachineDefinition(140.0, 12.0, false, false,
                "@ wake|wakes with a bright status screen.",
                "@ blank|blanks out as power is lost."));
        CreateModernComponent("Network Adapter", "NetworkAdapter_Wired",
            "Turns an item into a wired network adapter for a computer host.",
            PoweredMachineDefinition(18.0, 2.0, true, true,
                "@ blink|blinks as the network adapter comes online.",
                "@ dim|dims as the network adapter drops offline.",
                new XElement("PreferredNetworkAddress", new XCData(string.Empty)),
                new XElement("PublicNetworkEnabled", true),
                new XElement("ExchangeSubnetId", new XCData(string.Empty)),
                new XElement("VpnNetworkIds")));
        CreateModernComponent("Network Switch", "NetworkSwitch_4Port",
            "Turns an item into a compact four-port network switch.",
            PoweredMachineDefinition(15.0, 1.5, true, false,
                "@ blink|blinks as the switch powers up.",
                "@ dim|dims as the switch powers down.",
                new XElement("PortCount", 4)));
        CreateModernComponent("Network Switch", "NetworkSwitch_8Port",
            "Turns an item into a standard eight-port network switch.",
            PoweredMachineDefinition(25.0, 2.5, true, false,
                "@ blink|blinks as the switch powers up.",
                "@ dim|dims as the switch powers down.",
                new XElement("PortCount", 8)));
        CreateModernComponent("Wireless Modem", "WirelessModem_Local",
            "Turns an item into a cellular-backed wireless modem for a computer host.",
            PoweredMachineDefinition(8.0, 1.0, true, true,
                "@ blink|blinks as the modem starts searching for a network.",
                "@ dim|dims and loses its signal.",
                new XElement("PreferredNetworkAddress", new XCData(string.Empty)),
                new XElement("PublicNetworkEnabled", true),
                new XElement("ExchangeSubnetId", new XCData(string.Empty)),
                new XElement("VpnNetworkIds")));

        GameItemComponentProto automationHousingPanel = CreateModernComponent("Automation Housing",
            "AutomationHousing_Panel",
            "Turns an item into a compact lockable automation service panel.",
            new XElement("Definition",
                new XAttribute("Weight", 25_000),
                new XAttribute("MaxSize", (int)SizeCategory.Small),
                new XAttribute("Preposition", "in"),
                new XAttribute("Transparent", false),
                new XElement("ForceDifficulty", (int)Difficulty.Hard),
                new XElement("PickDifficulty", (int)Difficulty.Normal),
                new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$.")),
                new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$.")),
                new XElement("LockEmoteNoActor", new XCData("@ click|clicks shut.")),
                new XElement("UnlockEmoteNoActor", new XCData("@ click|clicks open.")),
                new XElement("LockType", "Cam Lock"),
                new XElement("AllowCableSegments", true),
                new XElement("AllowMountableModules", true),
                new XElement("AllowSignalItems", true)));
        CreateModernComponent("Automation Housing", "AutomationHousing_JunctionBox",
            "Turns an item into a larger junction box style automation housing.",
            new XElement("Definition",
                new XAttribute("Weight", 80_000),
                new XAttribute("MaxSize", (int)SizeCategory.Normal),
                new XAttribute("Preposition", "in"),
                new XAttribute("Transparent", false),
                new XElement("ForceDifficulty", (int)Difficulty.VeryHard),
                new XElement("PickDifficulty", (int)Difficulty.Hard),
                new XElement("LockEmote", new XCData("@ secure|secures $1$?2| with $2||$.")),
                new XElement("UnlockEmote", new XCData("@ release|releases $1$?2| with $2||$.")),
                new XElement("LockEmoteNoActor", new XCData("@ thunk|thunks shut.")),
                new XElement("UnlockEmoteNoActor", new XCData("@ clunk|clunks open.")),
                new XElement("LockType", "Panel Lock"),
                new XElement("AllowCableSegments", true),
                new XElement("AllowMountableModules", true),
                new XElement("AllowSignalItems", true)));
        CreateModernComponent("Automation Mount Host", "AutomationMountHost_2Bay",
            "Turns an item into an automation mount host with two module bays.",
            new XElement("Definition",
                new XElement("Bays",
                    new XElement("Bay",
                        new XAttribute("name", "bay1"),
                        new XAttribute("mounttype", "Microcontroller")),
                    new XElement("Bay",
                        new XAttribute("name", "bay2"),
                        new XAttribute("mounttype", "Microcontroller"))),
                new XElement("AccessPanelPrototypeId", automationHousingPanel.Id),
                new XElement("AccessPanelPrototypeName", new XCData(automationHousingPanel.Name))));
        CreateModernComponent("Automation Mount Host", "AutomationMountHost_4Bay",
            "Turns an item into an automation mount host with four module bays.",
            new XElement("Definition",
                new XElement("Bays",
                    new XElement("Bay",
                        new XAttribute("name", "bay1"),
                        new XAttribute("mounttype", "Microcontroller")),
                    new XElement("Bay",
                        new XAttribute("name", "bay2"),
                        new XAttribute("mounttype", "Microcontroller")),
                    new XElement("Bay",
                        new XAttribute("name", "bay3"),
                        new XAttribute("mounttype", "Microcontroller")),
                    new XElement("Bay",
                        new XAttribute("name", "bay4"),
                        new XAttribute("mounttype", "Microcontroller"))),
                new XElement("AccessPanelPrototypeId", automationHousingPanel.Id),
                new XElement("AccessPanelPrototypeName", new XCData(automationHousingPanel.Name))));
        CreateModernComponent("Signal Cable Segment", "SignalCableSegment_Standard",
            "Turns an item into a standard one-hop signal cable segment.",
            new XElement("Definition"));
        CreateModernComponent("File Signal Generator", "FileSignalGenerator_Text",
            "Turns an item into a file-backed signal generator for text-authored automation values.",
            PoweredMachineDefinition(40.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("SignalFileName", new XCData("signal.txt")),
                new XElement("InitialFileContents", new XCData("0")),
                new XElement("FileCapacityInBytes", 4096L),
                new XElement("PubliclyAccessibleByDefault", false)));
        CreateModernComponent("Push Button", "PushButton_Doorbell",
            "Turns an item into a push-button style trigger such as a doorbell or request button.",
            new XElement("Definition",
                new XElement("Keyword", new XCData("doorbell")),
                new XElement("SignalValue", 1.0),
                new XElement("SignalDurationSeconds", 2.0),
                new XElement("PressEmote", new XCData("@ press|presses $1."))));
        GameItemComponentProto toggleSwitchWall = CreateModernComponent("Toggle Switch", "ToggleSwitch_Wall",
            "Turns an item into a wall-mounted toggle switch.",
            new XElement("Definition",
                new XElement("OnValue", 1.0),
                new XElement("OffValue", 0.0),
                new XElement("InitiallyOn", false)));
        GameItemComponentProto motionSensorRoom = CreateModernComponent("Motion Sensor", "MotionSensor_Room",
            "Turns an item into a general indoor motion sensor.",
            PoweredMachineDefinition(40.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("SignalValue", 1.0),
                new XElement("SignalDurationSeconds", 5.0),
                new XElement("MinimumSize", (int)SizeCategory.Small),
                new XElement("DetectionMode", 0)));
        GameItemComponentProto motionSensorPerimeter = CreateModernComponent("Motion Sensor", "MotionSensor_Perimeter",
            "Turns an item into a perimeter motion sensor focused on arrivals.",
            PoweredMachineDefinition(55.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("SignalValue", 1.0),
                new XElement("SignalDurationSeconds", 12.0),
                new XElement("MinimumSize", (int)SizeCategory.Normal),
                new XElement("DetectionMode", 2)));
        GameItemComponentProto lightSensorDaylight = CreateModernComponent("Light Sensor", "LightSensor_Daylight",
            "Turns an item into a powered ambient light sensor.",
            PoweredMachineDefinition(25.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down."));
        CreateModernComponent("Rain Sensor", "RainSensor_Outdoor",
            "Turns an item into a powered outdoor rain sensor.",
            PoweredMachineDefinition(25.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down."));
        CreateModernComponent("Temperature Sensor", "TemperatureSensor_Thermostat",
            "Turns an item into a powered ambient temperature sensor.",
            PoweredMachineDefinition(25.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down."));
        GameItemComponentProto timerSensorRepeating = CreateModernComponent("Timer Sensor", "TimerSensor_Repeating",
            "Turns an item into a repeating timer sensor for periodic automation.",
            PoweredMachineDefinition(20.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("ActiveValue", 1.0),
                new XElement("InactiveValue", 0.0),
                new XElement("ActiveDurationSeconds", 5.0),
                new XElement("InactiveDurationSeconds", 55.0),
                new XElement("StartActive", false)));
        GameItemComponentProto keypad4Digit = CreateModernComponent("Keypad", "Keypad_4Digit",
            "Turns an item into a four-digit access keypad.",
            PoweredMachineDefinition(35.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("Code", new XCData("1234")),
                new XElement("SignalValue", 1.0),
                new XElement("SignalDurationSeconds", 2.0),
                new XElement("EntryEmote", new XCData("@ tap|taps digits into $1"))));
        CreateModernComponent("Keypad", "Keypad_6Digit",
            "Turns an item into a six-digit security keypad.",
            PoweredMachineDefinition(35.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("Code", new XCData("246810")),
                new XElement("SignalValue", 1.0),
                new XElement("SignalDurationSeconds", 2.0),
                new XElement("EntryEmote", new XCData("@ tap|taps digits into $1"))));
        CreateModernComponent("Signal Light", "SignalLight_Indicator",
            "Turns an item into a small indicator light driven by a switch or controller.",
            new XElement("Definition",
                new XElement("IlluminationProvided", 60.0),
                new XElement("SourceComponentId", toggleSwitchWall.Id),
                new XElement("SourceComponentName", new XCData(toggleSwitchWall.Name)),
                new XElement("SourceEndpointKey", new XCData("signal")),
                new XElement("ActivationThreshold", 0.5),
                new XElement("LitWhenAboveThreshold", true),
                new XElement("LightOnEmote", new XCData("@ light|lights up.")),
                new XElement("LightOffEmote", new XCData("@ go|goes dark."))));
        CreateModernComponent("Signal Light", "SignalLight_Beacon",
            "Turns an item into a repeating signal beacon driven by a timer.",
            new XElement("Definition",
                new XElement("IlluminationProvided", 220.0),
                new XElement("SourceComponentId", timerSensorRepeating.Id),
                new XElement("SourceComponentName", new XCData(timerSensorRepeating.Name)),
                new XElement("SourceEndpointKey", new XCData("signal")),
                new XElement("ActivationThreshold", 0.5),
                new XElement("LitWhenAboveThreshold", true),
                new XElement("LightOnEmote", new XCData("@ flash|flashes brightly.")),
                new XElement("LightOffEmote", new XCData("@ dim|dims."))));
        CreateModernComponent("Relay Switch", "RelaySwitch_Inline",
            "Turns an item into a signal-driven inline relay switch.",
            new XElement("Definition",
                new XElement("Wattage", 5.0),
                new XElement("SourceComponentId", toggleSwitchWall.Id),
                new XElement("SourceComponentName", new XCData(toggleSwitchWall.Name)),
                new XElement("SourceEndpointKey", new XCData("signal")),
                new XElement("ActivationThreshold", 0.5),
                new XElement("ClosedWhenAboveThreshold", true)));
        CreateModernComponent("Alarm Siren", "AlarmSiren_Indoor",
            "Turns an item into an indoor alarm siren driven by a motion sensor.",
            PoweredMachineDefinition(60.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("SourceComponentId", motionSensorRoom.Id),
                new XElement("SourceComponentName", new XCData(motionSensorRoom.Name)),
                new XElement("SourceEndpointKey", new XCData("signal")),
                new XElement("ActivationThreshold", 0.5),
                new XElement("SoundWhenAboveThreshold", true),
                new XElement("AlarmVolume", (int)AudioVolume.VeryLoud),
                new XElement("AlarmEmote", new XCData("@ blare|blares with a piercing alarm tone."))));
        CreateModernComponent("Alarm Siren", "AlarmSiren_Outdoor",
            "Turns an item into an outdoor perimeter alarm siren.",
            PoweredMachineDefinition(120.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("SourceComponentId", motionSensorPerimeter.Id),
                new XElement("SourceComponentName", new XCData(motionSensorPerimeter.Name)),
                new XElement("SourceEndpointKey", new XCData("signal")),
                new XElement("ActivationThreshold", 0.5),
                new XElement("SoundWhenAboveThreshold", true),
                new XElement("AlarmVolume", (int)AudioVolume.DangerouslyLoud),
                new XElement("AlarmEmote", new XCData("@ wail|wails across the area with a shrill alarm."))));
        CreateModernComponent("Electronic Door", "ElectronicDoor_AutoSlide",
            "Turns an item into a motion-triggered automatic sliding door.",
            new XElement("Definition",
                new XAttribute("SeeThrough", false),
                new XAttribute("CanFireThrough", false),
                new XElement("InstalledExitDescription", "automatic door"),
                new XElement("CanBeOpenedByPlayers", false),
                new XElement("Uninstall",
                    new XAttribute("CanPlayersUninstall", false),
                    new XAttribute("UninstallDifficultyHingeSide", (int)Difficulty.Impossible),
                    new XAttribute("UninstallDifficultyNotHingeSide", (int)Difficulty.Impossible),
                    new XAttribute("UninstallTrait", 0)),
                new XElement("Smash",
                    new XAttribute("CanPlayersSmash", false),
                    new XAttribute("SmashDifficulty", (int)Difficulty.Impossible)),
                new XElement("SourceComponentId", motionSensorRoom.Id),
                new XElement("SourceComponentName", new XCData(motionSensorRoom.Name)),
                new XElement("SourceEndpointKey", new XCData("signal")),
                new XElement("ActivationThreshold", 0.5),
                new XElement("OpenWhenAboveThreshold", true),
                new XElement("OpenEmoteNoActor", new XCData("@ slide|slides open.")),
                new XElement("CloseEmoteNoActor", new XCData("@ slide|slides closed."))));
        CreateModernComponent("Electronic Lock", "ElectronicLock_Maglock",
            "Turns an item into a magnetic lock released by a keypad signal.",
            new XElement("Definition",
                new XElement("ForceDifficulty", (int)Difficulty.ExtremelyHard),
                new XElement("PickDifficulty", (int)Difficulty.Impossible),
                new XElement("LockEmoteNoActor", new XCData("@ lock|locks with a solid magnetic thunk.")),
                new XElement("UnlockEmoteNoActor", new XCData("@ unlock|unlocks with a sharp magnetic click.")),
                new XElement("LockEmoteOtherSide", new XCData("@ hear|hears the maglock engage on $1.")),
                new XElement("UnlockEmoteOtherSide", new XCData("@ hear|hears the maglock release on $1.")),
                new XElement("LockType", "Maglock"),
                new XElement("SourceComponentId", keypad4Digit.Id),
                new XElement("SourceComponentName", new XCData(keypad4Digit.Name)),
                new XElement("SourceEndpointKey", new XCData("signal")),
                new XElement("ActivationThreshold", 0.5),
                new XElement("LockWhenAboveThreshold", false)));
        CreateModernComponent("Microcontroller", "Microcontroller_Basic",
            "Turns an item into a simple programmable microcontroller with a daylight input example.",
            PoweredMachineDefinition(20.0, 0.0, true, true,
                "@ hum|hums briefly as it powers on",
                "@ shudder|shudders as it powers down.",
                new XElement("Input",
                    new XAttribute("variable", "daylight"),
                    new XAttribute("sourceid", lightSensorDaylight.Id),
                    new XAttribute("source", lightSensorDaylight.Name),
                    new XAttribute("endpoint", "signal")),
                new XElement("LogicText", new XCData("return @daylight"))));

        CreateModernComponent("GasContainer", "GasContainer_OxygenSmall",
            "Turns an item into a small oxygen cylinder.",
            new XElement("Definition",
                new XElement("MaximumGasCapacity", 450.0),
                new XElement("ShowGasLevels", true),
                ConnectorsElement(new ConnectorType(Gender.Female, gasSocketType, false))));
        CreateModernComponent("GasContainer", "GasContainer_OxygenLarge",
            "Turns an item into a larger oxygen cylinder.",
            new XElement("Definition",
                new XElement("MaximumGasCapacity", 1800.0),
                new XElement("ShowGasLevels", true),
                ConnectorsElement(new ConnectorType(Gender.Female, gasSocketType, false))));
        CreateModernComponent("GasContainer", "GasContainer_Anaesthetic",
            "Turns an item into a gas cylinder suitable for anaesthetic or specialist breathing mixes.",
            new XElement("Definition",
                new XElement("MaximumGasCapacity", 900.0),
                new XElement("ShowGasLevels", true),
                ConnectorsElement(new ConnectorType(Gender.Female, gasSocketType, false))));
        CreateModernComponent("RcsThruster", "RcsThruster_Standard",
            "Turns a wearable item into a zero-gravity RCS thruster that consumes connected gas.",
            new XElement("Definition",
                new XElement("GasPerThrust", 0.5),
                new XElement("Connector",
                    new XAttribute("gender", (short)Gender.Male),
                    new XAttribute("type", gasSocketType),
                    new XAttribute("powered", false))));
        CreateModernComponent("ZeroGravityTether", "ZeroGravityTether_3Room",
            "Turns an item into a zero-gravity tether with a three-room maximum length.",
            new XElement("Definition",
                new XElement("MaximumRooms", 3)));
        CreateModernComponent("ZeroGravityAnchor", "ZeroGravityAnchor_SetPiece",
            "Turns an item into a fixed zero-gravity push-off anchor.",
            new XElement("Definition"));
        CreateModernComponent("Rebreather", "Rebreather_Standard",
            "Turns an item into a standard rebreather or breathing mask connection.",
            new XElement("Definition",
                new XElement("WaterTight", false),
                new XElement("Connection",
                    new XAttribute("gender", (short)Gender.Male),
                    new XAttribute("type", gasSocketType),
                    new XAttribute("powered", false))));
        CreateModernComponent("Rebreather", "Rebreather_Watertight",
            "Turns an item into a watertight rebreather or dive mask connection.",
            new XElement("Definition",
                new XElement("WaterTight", true),
                new XElement("Connection",
                    new XAttribute("gender", (short)Gender.Male),
                    new XAttribute("type", gasSocketType),
                    new XAttribute("powered", false))));
        CreateModernComponent("ExternalInhaler", "ExternalInhaler_Medical",
            "Turns an item into an external inhaler that accepts metered-dose canisters.",
            new XElement("Definition",
                new XElement("GasPerPuff", 0.25),
                new XElement("CanisterType", "MeteredDose"),
                new XElement("Connector",
                    new XAttribute("gender", (short)Gender.Female),
                    new XAttribute("type", "inhaler"),
                    new XAttribute("powered", false))));

        Gas? bronchodilatorGas = FindGas("Bronchodilator");
        Gas? anaestheticGas = FindGas("General Anaesthetic", "Anaesthetic", "Ether Anaesthetic");
        if (bronchodilatorGas is not null)
        {
            CreateModernComponent("InhalerGasCanister", "InhalerGasCanister_Bronchodilator",
                "Turns an item into a metered-dose bronchodilator inhaler canister.",
                new XElement("Definition",
                    new XElement("CanisterType", "MeteredDose"),
                    new XElement("InitialGas", bronchodilatorGas.Id)));
            CreateModernComponent("IntegratedInhaler", "IntegratedInhaler_Emergency",
                "Turns an item into a self-contained emergency bronchodilator inhaler.",
                new XElement("Definition",
                    new XElement("GasPerPuff", 0.25),
                    new XElement("InitialGas", bronchodilatorGas.Id)));
        }

        if (anaestheticGas is not null)
        {
            CreateModernComponent("InhalerGasCanister", "InhalerGasCanister_Anaesthetic",
                "Turns an item into a metered-dose anaesthetic inhaler canister.",
                new XElement("Definition",
                    new XElement("CanisterType", "MeteredDose"),
                    new XElement("InitialGas", anaestheticGas.Id)));
        }

        CreateModernComponent("Defibrillator", "Defibrillator_AED",
            "Turns an item into an automated external defibrillator.",
            new XElement("Definition",
                new XElement("WattagePerShock", 6000.0),
                new XElement("DefibrillationEmote",
                    new XCData("$0 place|places $2's pads against $1's chest and deliver|delivers a controlled shock."))));
        CreateModernComponent("Defibrillator", "Defibrillator_Clinical",
            "Turns an item into a clinical defibrillator with a stronger discharge.",
            new XElement("Definition",
                new XElement("WattagePerShock", 15000.0),
                new XElement("DefibrillationEmote",
                    new XCData("$0 charge|charges $2 and then press|presses the paddles to $1's chest, delivering a hard shock."))));

        BodyProto? externalOrganBody = FindHumanoidExternalOrganBody();
        if (externalOrganBody is not null)
        {
            List<BodypartProto> heartLungOrgans = FindExternalOrgans(externalOrganBody, BodypartTypeEnum.Heart, BodypartTypeEnum.Lung);
            bool hasHeart = heartLungOrgans.Any(x => x.BodypartType == (int)BodypartTypeEnum.Heart);
            bool hasLung = heartLungOrgans.Any(x => x.BodypartType == (int)BodypartTypeEnum.Lung);
            if (hasHeart && hasLung)
            {
                CreateModernComponent("ExternalOrgan", "ExternalOrgan_HeartLungSupport_Human",
                    "Turns an item into an external heart-lung support machine for humanoids.",
                    new XElement("Definition",
                        new XElement("Body", externalOrganBody.Id),
                        new XElement("OxygenatesBlood", true),
                        new XElement("Addendum", new XCData("It hums steadily while circulating and oxygenating blood.")),
                        new XElement("VenousConnection", externalVenousConnector),
                        new XElement("ArterialConnection", externalArterialConnector),
                        new XElement("BasePowerConsumptionInWatts", 650.0),
                        new XElement("PowerConsumptionDiscountPerQuality", 30.0),
                        new XElement("SwitchOnEmote", new XCData("@ rumble|rumbles to life as pumps and oxygenators come online.")),
                        new XElement("SwitchOffEmote", new XCData("@ wind|winds down as the circulation system powers off.")),
                        new XElement("Organs",
                            from organ in heartLungOrgans
                            select new XElement("Organ", organ.Id))));
            }

            List<BodypartProto> kidneyOrgans = FindExternalOrgans(externalOrganBody, BodypartTypeEnum.Kidney);
            if (kidneyOrgans.Count > 0)
            {
                CreateModernComponent("ExternalOrgan", "ExternalOrgan_Dialysis_Human",
                    "Turns an item into a humanoid dialysis support machine.",
                    new XElement("Definition",
                        new XElement("Body", externalOrganBody.Id),
                        new XElement("OxygenatesBlood", false),
                        new XElement("Addendum", new XCData("It cycles blood through a filtration assembly.")),
                        new XElement("VenousConnection", externalVenousConnector),
                        new XElement("ArterialConnection", externalArterialConnector),
                        new XElement("BasePowerConsumptionInWatts", 320.0),
                        new XElement("PowerConsumptionDiscountPerQuality", 18.0),
                        new XElement("SwitchOnEmote", new XCData("@ hum|hums to life as filtration pumps begin cycling.")),
                        new XElement("SwitchOffEmote", new XCData("@ slow|slows and falls silent as filtration stops.")),
                        new XElement("Organs",
                            from organ in kidneyOrgans
                            select new XElement("Organ", organ.Id))));
            }
        }

        CreateModernComponent("ElectricGridFeeder", "ElectricGridFeeder_Standard",
            "Turns an item into a feeder for the electrical grid.",
            ConnectorDefinition(
                new ConnectorType(Gender.Male, mainsSocketType, true)));
        CreateModernComponent("TelecommunicationsGridFeeder", "TelecommunicationsGridFeeder_Standard",
            "Turns an item into a feeder for supplying power into the telecommunications grid.",
            new XElement("Definition",
                new XElement("Wattage", 20.0),
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Male),
                        new XAttribute("type", mainsSocketType),
                        new XAttribute("powered", true)))));
        CreateModernComponent("TelecommunicationsGridOutlet", "TelecommunicationsGridOutlet",
            "Turns an item into an outlet to plug a landline telephone into.",
            new XElement("Definition",
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Female),
                        new XAttribute("type", "TelephoneLine"),
                        new XAttribute("powered", true)))));
        CreateModernComponent("GridLiquidSource", "GridLiquidSource_Standard",
            "Turns an item into a liquid grid source and physical connector.",
            ConnectorDefinition(
                new ConnectorType(Gender.Male, "LiquidLine", false)));
        CreateModernComponent("LiquidGridSupplier", "LiquidGridSupplier_Standard",
            "Turns an item into a liquid grid supplier that feeds from a sibling liquid container.",
            new XElement("Definition"));
        CreateModernComponent("LiquidPump", "LiquidPump_Standard",
            "Turns an item into a powered liquid pump with input and output connectors.",
            new XElement("Definition",
                new XElement("FlowRate", 1.0),
                new XElement("Wattage", 25.0),
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Male),
                        new XAttribute("type", mainsSocketType),
                        new XAttribute("powered", true)),
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Female),
                        new XAttribute("type", "LiquidLine"),
                        new XAttribute("powered", false)),
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Male),
                        new XAttribute("type", "LiquidLine"),
                        new XAttribute("powered", false)))));
        CreateModernComponent("LiquidPump", "LiquidPump_Industrial",
            "Turns an item into a high-throughput powered liquid pump.",
            new XElement("Definition",
                new XElement("FlowRate", 5.0),
                new XElement("Wattage", 120.0),
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Male),
                        new XAttribute("type", mainsSocketType),
                        new XAttribute("powered", true)),
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Female),
                        new XAttribute("type", "LiquidLine"),
                        new XAttribute("powered", false)),
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Male),
                        new XAttribute("type", "LiquidLine"),
                        new XAttribute("powered", false)))));
        CreateModernComponent("LiquidConsumingProp", "LiquidConsumingProp_Standard",
            "Turns an item into a prop that steadily consumes liquid.",
            new XElement("Definition",
                new XElement("LiquidCapacity", 10.0),
                new XElement("ConsumptionPerSecond", 0.1),
                new XElement("Transparent", true),
                new XElement("CanBeEmptiedWhenInRoom", true),
                new XElement("ContentsPreposition", "in"),
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Female),
                        new XAttribute("type", "LiquidLine"),
                        new XAttribute("powered", false)))));
        CreateModernComponent("LiquidConsumingProp", "LiquidConsumingProp_Basin",
            "Turns an item into a larger prop that steadily drains liquid for steady-state testing.",
            new XElement("Definition",
                new XElement("LiquidCapacity", 25.0),
                new XElement("ConsumptionPerSecond", 0.5),
                new XElement("Transparent", false),
                new XElement("CanBeEmptiedWhenInRoom", true),
                new XElement("ContentsPreposition", "in"),
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Female),
                        new XAttribute("type", "LiquidLine"),
                        new XAttribute("powered", false)))));
        CreateModernComponent("BatteryPowered", "BatteryPowered_4xAA_Connectable",
            "Turns an item into a 4xAA battery powered device that can also charge from a power source.",
            new XElement("Definition",
                new XElement("BatteryType", "AA"),
                new XElement("BatteryQuantity", 4),
                new XElement("BatteriesInSeries", true),
                new XElement("ChargeWattage", 20.0),
                new XElement("Transparent", false),
                new XElement("ContentsPreposition", "in"),
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Female),
                        new XAttribute("type", mainsSocketType),
                        new XAttribute("powered", true)))));
        CreateModernComponent("BatteryPowered", "BatteryPowered_LaptopStyle",
            "Turns an item into a laptop-style lithium ion battery backed device with a charging connector.",
            new XElement("Definition",
                new XElement("BatteryType", "Li-Ion"),
                new XElement("BatteryQuantity", 1),
                new XElement("BatteriesInSeries", true),
                new XElement("ChargeWattage", 65.0),
                new XElement("Transparent", false),
                new XElement("ContentsPreposition", "in"),
                new XElement("Connectors",
                    new XElement("Connection",
                        new XAttribute("gender", (short)Gender.Female),
                        new XAttribute("type", "USB-C"),
                        new XAttribute("powered", true)))));

        CreateElectricHeaterCooler("ElectricHeaterCooler_SpaceHeater",
            "Turns an item into a compact mains-powered space heater.",
            4.0, 12.0, 8.0, 4.0, 1.5, 0.5, 1500.0,
            "It radiates a steady wave of dry heat.",
            "It is cold and silent.",
            "@ click|clicks and begin|begins pushing out warm air.",
            "@ click|clicks off and the warm airflow fade|fades.");
        CreateElectricHeaterCooler("ElectricHeaterCooler_WallHeater",
            "Turns an item into a larger fixed electric wall heater.",
            6.0, 10.0, 7.5, 4.5, 2.5, 1.0, 2400.0,
            "It gives off a broad, comfortable warmth.",
            "It is currently inactive and cool to the touch.",
            "@ hum|hums softly as heating elements glow to life.",
            "@ tick|ticks quietly as the heating elements cool.");
        CreateElectricHeaterCooler("ElectricHeaterCooler_PortableCooler",
            "Turns an item into a portable electric cooler or compact air conditioner.",
            -3.5, -9.0, -6.0, -3.5, -1.5, -0.5, 900.0,
            "It pushes out a stream of chilled air.",
            "It is switched off and room temperature.",
            "@ whirr|whirrs to life and begin|begins venting chilled air.",
            "@ wind|winds down and the chilled airflow stop|stops.");
        CreateElectricHeaterCooler("ElectricHeaterCooler_IndustrialCooler",
            "Turns an item into an industrial electric cooling unit.",
            -6.0, -10.0, -8.0, -5.0, -3.0, -1.2, 3200.0,
            "It drones continuously while dumping cold air into the room.",
            "It is currently inactive and silent.",
            "@ thrum|thrums to life with a blast of refrigerated air.",
            "@ cycle|cycles down and the cold draft ebb|ebbs away.");

        CreateConsumableHeaterCooler("ConsumableHeaterCooler_SmallFire",
            "Turns an item into a small temporary fire such as a hurried cooking fire or signal flame.",
            2.0, 10.0, 7.0, 3.0, 1.0, 0.3, 900,
            "It burns with a modest crackling flame.",
            "It has burned down to cold ash.",
            "$0 gutter|gutters and collapse|collapses into dying embers.");
        CreateConsumableHeaterCooler("ConsumableHeaterCooler_LargeFire",
            "Turns an item into a larger campfire-sized temporary blaze.",
            5.0, 18.0, 14.0, 8.0, 3.0, 1.0, 3600,
            "It burns hot and bright, throwing out waves of heat.",
            "It has burned out.",
            "$0 spit|spits a final shower of sparks before the flames die away.");
        CreateConsumableHeaterCooler("ConsumableHeaterCooler_Bonfire",
            "Turns an item into a large temporary bonfire.",
            8.0, 25.0, 18.0, 10.0, 5.0, 2.0, 10800,
            "It roars with an intense bonfire heat.",
            "It has collapsed into a mound of cold charcoal and ash.",
            "$0 roar|roars one last time before collapsing into a heap of glowing embers.");

        CreateSolidFuelHeaterCooler("SolidFuelHeaterCooler_Brazier",
            "Turns an item into a solid-fuel brazier that accepts tagged fuel items.",
            3.0, 12.0, 8.0, 4.5, 2.0, 0.8, 8.0, 1100.0,
            "It is filled with glowing fuel and wafting heat.",
            "It is empty and cold.",
            "@ flare|flares up as the fuel catch|catches.",
            "@ dim|dims as the brazier is shut down.");
        CreateSolidFuelHeaterCooler("SolidFuelHeaterCooler_Fireplace",
            "Turns an item into a room-warming fireplace that burns tagged solid fuel.",
            6.0, 18.0, 12.0, 7.0, 3.5, 1.5, 35.0, 1800.0,
            "It crackles warmly with a bed of burning fuel.",
            "It is laid cold and dark.",
            "@ catch|catches and begin|begins to roar warmly up the flue.",
            "@ settle|settles down as the flames are banked.");
        CreateSolidFuelHeaterCooler("SolidFuelHeaterCooler_WoodStove",
            "Turns an item into a cast-iron stove that burns tagged solid fuel.",
            7.0, 16.0, 11.0, 6.5, 3.0, 1.2, 20.0, 2400.0,
            "It rings softly with stored heat from the burning fuel within.",
            "It is cold iron with no glow inside.",
            "@ rumble|rumbles as the stove draws properly and the fire takes.",
            "@ clank|clanks shut as the stove is damped down.");

        if (fuelTag is not null)
        {
            foreach (Liquid? liquid in context.Liquids
                         .Where(x => x.LiquidsTags.Any(y => y.TagId == fuelTag.Id))
                         .OrderBy(x => x.Name)
                         .ToList())
            {
                string safeName = SanitizeComponentName(liquid.Name);
                CreateLiquidFuelHeaterCooler("PortableHeater", liquid,
                    $"Turns an item into a portable radiant heater that burns {liquid.Name} from a connected liquid supply.",
                    4.0, 14.0, 9.0, 5.0, 2.0, 0.5, 0.00025,
                    "It gives off a close, oily heat while the burner is lit.",
                    "It is unlit and cool.",
                    "@ hiss|hisses softly as the burner ignite|ignites.",
                    "@ gutter|gutters and go|goes out.");
                CreateLiquidFuelHeaterCooler("WorkshopStove", liquid,
                    $"Turns an item into a heavier workshop heater or stove that burns {liquid.Name} from a connected liquid supply.",
                    6.5, 18.0, 12.0, 7.0, 3.0, 1.0, 0.00045,
                    "It radiates a sustained mechanical heat from its burner chamber.",
                    "It is currently inactive and cold.",
                    "@ chuff|chuffs into life as the burner stabilise|stabilises.",
                    "@ sputter|sputters and die|dies down.");
                CreateModernComponent("Fuel Generator", $"FuelGenerator_{safeName}",
                    $"Turns an item into a portable generator fuelled by {liquid.Name}.",
                    new XElement("Definition",
                        new XElement("SwitchOnEmote", new XCData("@ pull|pulls $1 to life with a sputtering roar.")),
                        new XElement("SwitchOffEmote", new XCData("@ shut|shuts $1 down.")),
                        new XElement("FuelExpendedEmote", new XCData("@ cough|coughs, splutter|splatters and die|dies as it runs out of fuel.")),
                        new XElement("FuelPerSecond", 0.0025 / 3600.0),
                        new XElement("FuelCapacity", 20.0),
                        new XElement("WattageProvided", 5000.0),
                        new XElement("SwitchOnProg", 0),
                        new XElement("SwitchOffProg", 0),
                        new XElement("FuelOutProg", 0),
                        new XElement("LiquidFuel", liquid.Id)));
            }
        }

        context.SaveChanges();
    }

    private void SeedLiquidContainers(IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors, DateTime now, Account dbaccount, ref long nextId)
    {
        GameItemComponentProto CreateLiquidContainer(string name, string description, double capacity, bool closable, bool transparent, double weightLimit, ref long id, bool onceOnly = false)
        {
            string once = onceOnly ? " OnceOnly=\"true\"" : string.Empty;
            return CreateItemProto(id++, now, "Liquid Container", name, description,
                    $"<Definition LiquidCapacity=\"{capacity}\" Closable=\"{closable}\" Transparent=\"{transparent}\" WeightLimit=\"{weightLimit}\"{once} />");
        }

        CreateLiquidContainer("LContainer_ShotGlass", "A liquid container for a shot glass", 0.12, false, true, 1000, ref nextId);
        CreateLiquidContainer("LContainer_WhiskeyGlass", "A liquid container for a whiskey glass (or other small glass)", 0.25, false, true, 2000, ref nextId);
        CreateLiquidContainer("LContainer_DrinkingGlass", "A liquid container for a drinking glass (or other normal table glass)", 0.450, false, true, 4000, ref nextId);
        CreateLiquidContainer("LContainer_Pony", "A liquid container for a pony (1/4 pint glass)", 0.142, false, true, 4000, ref nextId);
        CreateLiquidContainer("LContainer_HalfPint", "A liquid container for a half pint glass", 0.284, false, true, 4000, ref nextId);
        CreateLiquidContainer("LContainer_Pint", "A liquid container for a US pint", 0.473, false, true, 6000, ref nextId);
        CreateLiquidContainer("LContainer_UKPint", "A liquid container for a UK pint", 0.568, false, true, 6000, ref nextId);
        CreateLiquidContainer("LContainer_Weizen", "A liquid container for a weizen glass (european 500ml glass)", 0.5, false, true, 7000, ref nextId);
        CreateLiquidContainer("LContainer_Stein", "A liquid container for a stein glass (european 1000ml glass)", 1.0, false, true, 14000, ref nextId);
        CreateLiquidContainer("LContainer_Yard", "A liquid container for a yard glass (2.5 imperial pints)", 1.4, false, true, 7000, ref nextId);
        CreateLiquidContainer("LContainer_Jug", "A liquid container for a glass jug, generally 40oz", 1.14, false, true, 7000, ref nextId);
        CreateLiquidContainer("LContainer_Flute", "A liquid container for a flute (champagne glass)", 0.180, false, true, 2000, ref nextId);
        CreateLiquidContainer("LContainer_Liqueur", "A liquid container for a small liqueur glass", 0.06, false, true, 2000, ref nextId);
        CreateLiquidContainer("LContainer_SherryGlass", "A liquid container for a copita, or a glass for drinking sherry", 0.180, false, true, 2000, ref nextId);
        CreateLiquidContainer("LContainer_SmallWineGlass", "A liquid container for a small wine glass, such as would typically be used for a white wine", 0.240, false, true, 2000, ref nextId);
        CreateLiquidContainer("LContainer_WineGlass", "A liquid container for a standard sized wine glass, such as would be typically used for red wine", 0.415, false, true, 4000, ref nextId);
        CreateLiquidContainer("LContainer_BeerBottle", "A liquid container for a single-use beer bottle (can't be re-sealed)", 0.375, true, true, 6000, ref nextId, true);
        CreateLiquidContainer("LContainer_SodaCan", "A liquid container for a single-use soda can (can't be re-sealed and isn't transparent)", 0.375, true, false, 6000, ref nextId, true);
        CreateLiquidContainer("LContainer_WineBottle", "A liquid container for a single-use wine bottle (can't be re-corked)", 0.75, true, true, 6000, ref nextId, true);
        CreateLiquidContainer("LContainer_Decanter", "A liquid container for a decanter for a bottle of wine", 0.75, true, true, 6000, ref nextId);
        CreateLiquidContainer("LContainer_Flask", "A liquid container for a typical hip flask", 0.236, true, false, 6000, ref nextId);
        CreateLiquidContainer("LContainer_Canteen", "A liquid container for a typical canteen", 1.0, true, false, 10000, ref nextId);
        CreateLiquidContainer("LContainer_Cask", "A liquid container for a typical wine cask ", 0.236, true, false, 6000, ref nextId);
        CreateLiquidContainer("LContainer_Tun", "A liquid container for a tun (252 US Gallon Barrel)", 960, true, false, 9600000, ref nextId);
        CreateLiquidContainer("LContainer_Butt", "A liquid container for a butt (126 US Gallon Barrel)", 480, true, false, 4800000, ref nextId);
        CreateLiquidContainer("LContainer_Puncheon", "A liquid container for a puncheon (84 US Gallon Barrel)", 320, true, false, 3200000, ref nextId);
        CreateLiquidContainer("LContainer_Hogshead", "A liquid container for a hogshead (63 US Gallon Barrel)", 240, true, false, 2400000, ref nextId);
        CreateLiquidContainer("LContainer_Tierce", "A liquid container for a tierce (42 US Gallon Barrel)", 160, true, false, 1600000, ref nextId);
        CreateLiquidContainer("LContainer_Barrel", "A liquid container for an English barrel (31.5 US Gallon Barrel)", 120, true, false, 1200000, ref nextId);
        CreateLiquidContainer("LContainer_Rundlet", "A liquid container for a rundlet (18 US Gallon Barrel)", 69, true, false, 690000, ref nextId);
        CreateLiquidContainer("LContainer_GallonCask", "A liquid container for a non-see through gallon-sized cask", 3.7, true, false, 37000, ref nextId);
        CreateLiquidContainer("LContainer_GallonBottle", "A liquid container for a gallon bottle like a milk bottle", 3.7, true, true, 37000, ref nextId);
        CreateLiquidContainer("LContainer_HalfGallonBottle", "A liquid container for a half gallon bottle like a milk bottle", 1.85, true, true, 18500, ref nextId);
        CreateLiquidContainer("LContainer_QuartBottle", "A liquid container for a one quart bottle like a milk bottle", 0.946, true, true, 18500, ref nextId);
        CreateLiquidContainer("LContainer_PintBottle", "A liquid container for a one pint bottle like a milk bottle", 0.473, true, true, 47300, ref nextId);
        CreateLiquidContainer("LContainer_20ozBottle", "A liquid container for a 20oz bottle", 0.591, true, true, 59100, ref nextId);
        CreateLiquidContainer("LContainer_40ozBottle", "A liquid container for a 40oz bottle", 1.182, true, true, 118200, ref nextId);
        CreateLiquidContainer("LContainer_QuartCarton", "A liquid container for a one quart carton", 0.946, true, false, 94600, ref nextId);
        CreateLiquidContainer("LContainer_PintCarton", "A liquid container for a one pint carton", 0.471, true, false, 47100, ref nextId);
        CreateLiquidContainer("LContainer_HalfPintCarton", "A liquid container for a one quart carton", 0.237, true, false, 23700, ref nextId);
        CreateLiquidContainer("LContainer_Waterskin", "A liquid container for a standard sized waterskin", 1.892, true, false, 189200, ref nextId);
        CreateLiquidContainer("LContainer_FuelCan", "A liquid container for a standard small sized fuel can (8L/2gal)", 8, true, false, 8000, ref nextId);
        CreateLiquidContainer("LContainer_JerryCan", "A liquid container for a standard sized jerry can (20L/5.4gal)", 20, true, false, 20000, ref nextId);
        CreateLiquidContainer("LContainer_Drum", "A liquid container for a standard 200L / 55gal drum", 200, true, false, 200000, ref nextId);
        CreateLiquidContainer("LContainer_Amphora_Sextarius", "A liquid container for an amphora in the roman sextarius (~0.96 pint)", 0.546, true, false, 5460, ref nextId);
        CreateLiquidContainer("LContainer_Amphora_Congius", "A liquid container for an amphora in the roman congius (~0.72 gallon)", 3.27, true, false, 32700, ref nextId);
        CreateLiquidContainer("LContainer_Amphora_Urna", "A liquid container for an amphora in the roman urna (~2.88 gallon)", 13.1, true, false, 131000, ref nextId);
        CreateLiquidContainer("LContainer_Amphora_Quadrantal", "A liquid container for an amphora in the roman quadrantal (~5.76 gallon)", 26.2, true, false, 262000, ref nextId);
        CreateLiquidContainer("LContainer_Amphora_Culeus", "A liquid container for an amphora in the roman culeus (~115 gallon)", 524, true, false, 524000, ref nextId);
    }

    private void SeedContainers(IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors, DateTime now, Account dbaccount, ref long nextId)
    {
        GameItemComponentProto CreateContainer(string name, string description, double weight, SizeCategory maxSize, bool closable, bool transparent, string preposition, ref long id, bool onceOnly = false)
        {
            string once = onceOnly ? " OnceOnly=\"true\"" : string.Empty;
            return CreateItemProto(id++, now, "Container", name, description,
                    $"<Definition Weight=\"{weight}\" MaxSize=\"{(int)maxSize}\" Preposition=\"{preposition}\" Closable=\"{closable}\" Transparent=\"{transparent}\"{once} />");
        }

        CreateContainer("Container_Table", "Allows a table to have items 'on' it", 200000, SizeCategory.Large, false, true, "on", ref nextId);
        CreateContainer("Container_Large_Table", "Allows a large table to have items 'on' it", 500000, SizeCategory.VeryLarge, false, true, "on", ref nextId);
        CreateContainer("Container_Small_Table", "Allows a small table to have items 'on' it", 50000, SizeCategory.Normal, false, true, "on", ref nextId);
        CreateContainer("Container_Side_Table", "Allows a side table, end table or night table to have items 'on' it", 25000, SizeCategory.Small, false, true, "on", ref nextId);
        CreateContainer("Container_Desk_Surface", "Allows a desk surface to have items 'on' it", 75000, SizeCategory.Normal, false, true, "on", ref nextId);
        CreateContainer("Container_Counter", "Allows a counter, bar or worktop to have items 'on' it", 350000, SizeCategory.Large, false, true, "on", ref nextId);
        CreateContainer("Container_Cot_Surface", "Allows a cot or narrow bed surface to have items 'on' it", 75000, SizeCategory.Normal, false, true, "on", ref nextId);
        CreateContainer("Container_Bed_Surface", "Allows a bed surface to have items 'on' it", 200000, SizeCategory.Large, false, true, "on", ref nextId);
        CreateContainer("Container_Couch_Surface", "Allows a couch, sofa or padded seat to have items 'on' it", 100000, SizeCategory.Normal, false, true, "on", ref nextId);
        CreateContainer("Container_Bench_Surface", "Allows a bench surface to have items 'on' it", 100000, SizeCategory.Normal, false, true, "on", ref nextId);
        CreateContainer("Container_Wall_Shelf", "Allows a wall shelf or narrow ledge to have items 'on' it", 20000, SizeCategory.Small, false, true, "on", ref nextId);
        CreateContainer("Container_Narrow_Shelves", "Allows a narrow set of shelves to have items 'on' it", 75000, SizeCategory.Normal, false, true, "on", ref nextId);
        CreateContainer("Container_Wide_Shelves", "Allows a wide set of shelves to have items 'on' it", 200000, SizeCategory.Large, false, true, "on", ref nextId);
        CreateContainer("Container_Bookcase_Shelves", "Allows a bookcase or library shelves to have items 'on' it", 175000, SizeCategory.Large, false, true, "on", ref nextId);
        CreateContainer("Container_Display_Shelves", "Allows open display shelves to have items 'on' them", 125000, SizeCategory.Normal, false, true, "on", ref nextId);
        CreateContainer("Container_Weapon_Rack", "Allows a weapon rack to have items 'on' it", 75000, SizeCategory.Large, false, true, "on", ref nextId);
        CreateContainer("Container_Armor_Stand", "Allows an armour stand or display dummy to have items 'on' it", 100000, SizeCategory.Large, false, true, "on", ref nextId);
        CreateContainer("Container_Carton", "A container for cartons of cigarettes, matches etc", 250, SizeCategory.Tiny, true, false, "in", ref nextId);
        CreateContainer("Container_Pocket", "A container for pockets in clothes", 500, SizeCategory.VerySmall, false, false, "in", ref nextId);
        CreateContainer("Container_LargePocket", "A container for large pockets in clothes", 2000, SizeCategory.Small, false, false, "in", ref nextId);
        CreateContainer("Container_Pocket_Closable", "A container for closable pockets in clothes", 500, SizeCategory.VerySmall, true, false, "in", ref nextId);
        CreateContainer("Container_LargePocket_Closable", "A container for closable large pockets in clothes", 2000, SizeCategory.Small, true, false, "in", ref nextId);
        CreateContainer("Container_Pouch", "A container for pouches", 1000, SizeCategory.VerySmall, true, false, "in", ref nextId);
        CreateContainer("Container_Baggie", "A container for see-through baggies", 1000, SizeCategory.VerySmall, true, true, "in", ref nextId);
        CreateContainer("Container_Sachet", "A container for single-use sachets", 1000, SizeCategory.VerySmall, true, false, "in", ref nextId, true);
        CreateContainer("Container_Purse", "A container for purses or handbags", 7000, SizeCategory.Small, true, false, "in", ref nextId);
        CreateContainer("Container_Plate", "A container for plates and similar", 1500, SizeCategory.VerySmall, false, false, "on", ref nextId);
        CreateContainer("Container_Tray", "A container for trays, platters, etc", 7000, SizeCategory.Small, false, false, "on", ref nextId);
        CreateContainer("Container_Tote", "A container for tote bags or shoulder bags", 20000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_PlasticBag", "A container for transparent plastic shopping bags", 10000, SizeCategory.Normal, false, true, "in", ref nextId);
        CreateContainer("Container_Sack", "A container for sturdy closable sacks and other similarly sized containers", 75000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Pack", "A container for backpacks and similar", 75000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Drum", "A container for standard sized drums (~55 Gal)", 250000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Small_Drum", "A container for small sized drums (~25 Gal)", 100000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Quiver", "A container for quivers", 10000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Open_Bin", "A container for open bins, hampers and baskets", 100000, SizeCategory.Normal, false, false, "in", ref nextId);
        CreateContainer("Container_Small_Cabinet", "A container for small cabinets and compact cupboards", 50000, SizeCategory.Small, true, false, "in", ref nextId);
        CreateContainer("Container_Large_Cabinet", "A container for large cabinets and storage cupboards", 200000, SizeCategory.Large, true, false, "in", ref nextId);
        CreateContainer("Container_Glass_Cabinet", "A transparent container for glass-fronted cabinets", 150000, SizeCategory.Normal, true, true, "in", ref nextId);
        CreateContainer("Container_Cupboard", "A container for cupboards, lockers and similar enclosed furniture", 125000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Wardrobe", "A container for wardrobes intended to hold clothing and similar goods", 250000, SizeCategory.Large, true, false, "in", ref nextId);
        CreateContainer("Container_Armoire", "A container for large armoires and wardrobes", 350000, SizeCategory.VeryLarge, true, false, "in", ref nextId);
        CreateContainer("Container_Dresser", "A container for dressers and chests of drawers", 125000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Desk_Drawers", "A container for desk drawers and writing-table storage", 25000, SizeCategory.Small, true, false, "in", ref nextId);
        CreateContainer("Container_Nightstand", "A container for nightstands and bedside tables with drawers", 15000, SizeCategory.Small, true, false, "in", ref nextId);
        CreateContainer("Container_Sideboard", "A container for sideboards and serving cabinets", 150000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Hutch", "A container for hutches and kitchen display cabinets", 175000, SizeCategory.Large, true, false, "in", ref nextId);
        CreateContainer("Container_Trunk", "A container for trunks and blanket chests", 200000, SizeCategory.Large, true, false, "in", ref nextId);
        CreateContainer("Container_Footlocker", "A container for footlockers and compact storage trunks", 100000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Blanket_Box", "A container for blanket boxes and under-bed storage", 150000, SizeCategory.Normal, true, false, "in", ref nextId);
        CreateContainer("Container_Display_Case", "A transparent container for display cases and exhibit cases", 75000, SizeCategory.Normal, true, true, "in", ref nextId);
        CreateContainer("Container_Hole", "A container for holes in the ground", 2000000, SizeCategory.VeryLarge, false, false, "in", ref nextId);
        CreateContainer("Container_Large_Hole", "A container for large holes in the ground", 5000000, SizeCategory.Huge, false, false, "in", ref nextId);
        CreateContainer("Container_Shipping_Container", "A container for standard 20ft shipping containers", 50000000, SizeCategory.Enormous, true, false, "in", ref nextId);
        CreateContainer("Container_Shipping_Container_Long", "A container for standard 40ft shipping containers", 100000000, SizeCategory.Enormous, true, false, "in", ref nextId);
        CreateContainer("Container_Shipping_Container_Large", "A container for larger shipping containers", 200000000, SizeCategory.Gigantic, true, false, "in", ref nextId);
        CreateContainer("Container_Shipping_Container_Small", "A container for small 10ft shipping containers", 25000000, SizeCategory.Huge, true, false, "in", ref nextId);
        CreateContainer("Container_Colossal", "A container with unthinkably large capacity", 1000000000, SizeCategory.Titanic, false, false, "in", ref nextId);
        CreateContainer("Container_Coffin", "A container for coffins designed to hold a human body", 250000, SizeCategory.Large, true, false, "in", ref nextId);
        CreateContainer("Container_Glass_Casket", "A container for see-through glass caskets designed to display a human body", 200000, SizeCategory.Large, true, true, "in", ref nextId);
    }

    private void SeedDoors(IReadOnlyDictionary<string, string> questionAnswers, ICollection<string> errors, DateTime now, Account dbaccount, ref long nextId)
    {
        TraitDefinition? doorTrait =
            _context.TraitDefinitions.FirstOrDefault(x => x.Name == "Constructing" || x.Name == "Construction") ??
            _context.TraitDefinitions.FirstOrDefault(x => x.Name == "Labouring" || x.Name == "Labourer") ??
            _context.TraitDefinitions.FirstOrDefault(x => x.Name == "Carpentry" || x.Name == "Carpenter");
        if (doorTrait == null)
        {
            TraitDefinition? example =
                _context.TraitDefinitions.FirstOrDefault(x => x.Type == 0 && x.TraitGroup != "Language");
            if (example != null)
            {
                TraitExpression expression = new()
                {
                    Name = $"Construction Cap",
                    Expression = example.Expression.Expression
                };
                _context.TraitExpressions.Add(expression);
                doorTrait = new TraitDefinition
                {
                    Name = "Construction",
                    Type = 0,
                    DecoratorId = _context.TraitDecorators.First(x => x.Name == "Crafting Skill").Id,
                    TraitGroup = "Combat",
                    AvailabilityProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
                    TeachableProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysFalse"),
                    LearnableProg = _context.FutureProgs.First(x => x.FunctionName == "AlwaysTrue"),
                    TeachDifficulty = 7,
                    LearnDifficulty = 7,
                    Hidden = false,
                    Expression = expression,
                    ImproverId = _context.Improvers.First(x => x.Name == "Skill Improver").Id,
                    DerivedType = 0,
                    ChargenBlurb = string.Empty,
                    BranchMultiplier = 1.0
                };
                _context.TraitDefinitions.Add(doorTrait);
                _context.SaveChanges();
            }
        }

        if (doorTrait is null)
        {
            errors.Add("There was no valid trait supplied for door installation so no door components were created.");
            return;
        }

        GameItemComponentProto CreateDoor(string name, string description, bool seeThrough, bool canFireThrough, bool canUninstall, Difficulty uninstallHinge, Difficulty uninstallNotHinge, bool canSmash, Difficulty smashDifficulty, string exitDescription, ref long id)
        {
            return CreateItemProto(id++, now, "Door", name, description,
                    $"<Definition SeeThrough=\"{seeThrough}\" CanFireThrough=\"{canFireThrough}\"><InstalledExitDescription>{exitDescription}</InstalledExitDescription><Uninstall CanPlayersUninstall=\"{canUninstall}\" UninstallDifficultyHingeSide=\"{(int)uninstallHinge}\" UninstallDifficultyNotHingeSide=\"{(int)uninstallNotHinge}\" UninstallTrait=\"{doorTrait.Id}\" /><Smash CanPlayersSmash=\"{canSmash}\" SmashDifficulty=\"{(int)smashDifficulty}\" /></Definition>");
        }

        CreateDoor("Door_Normal", "This is an ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "door", ref nextId);
        CreateDoor("Door_Tough", "This is a tough door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "door", ref nextId);
        CreateDoor("Door_Secure", "This is a door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "door", ref nextId);
        CreateDoor("Door_Admin", "This is a door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door", ref nextId);
        CreateDoor("Door_Bad", "This is a bad door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door", ref nextId);
        CreateDoor("Gate_Normal", "This is an ordinary gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gate", ref nextId);
        CreateDoor("Gate_Tough", "This is a tough gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gate", ref nextId);
        CreateDoor("Gate_Secure", "This is a tough gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gate", ref nextId);
        CreateDoor("Gate_Admin", "This is a gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gate", ref nextId);
        CreateDoor("Gate_Bad", "This is a bad gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gate", ref nextId);
        CreateDoor("Door_Glass", "This is a door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door", ref nextId);
        CreateDoor("Door_Glass_Secure", "This is a door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "door", ref nextId);
        CreateDoor("Door_Glass_Admin", "This is a door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door", ref nextId);
        CreateDoor("Door_Normal_Tiny", "This is a tiny ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "tiny door", ref nextId);
        CreateDoor("Door_Tough_Tiny", "This is a tough tiny door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "tiny door", ref nextId);
        CreateDoor("Door_Secure_Tiny", "This is a tiny door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "tiny door", ref nextId);
        CreateDoor("Door_Admin_Tiny", "This is a tiny door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "tiny door", ref nextId);
        CreateDoor("Door_Bad_Tiny", "This is a bad tiny door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "tiny door", ref nextId);
        CreateDoor("Gate_Normal_Tiny", "This is an ordinary tiny gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "tiny gate", ref nextId);
        CreateDoor("Gate_Tough_Tiny", "This is a tough tiny gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "tiny gate", ref nextId);
        CreateDoor("Gate_Secure_Tiny", "This is a tough tiny gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "tiny gate", ref nextId);
        CreateDoor("Gate_Admin_Tiny", "This is a tiny gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "tiny gate", ref nextId);
        CreateDoor("Gate_Bad_Tiny", "This is a bad tiny gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "tiny gate", ref nextId);
        CreateDoor("Door_Glass_Tiny", "This is a tiny door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "tiny door", ref nextId);
        CreateDoor("Door_Glass_Secure_Tiny", "This is a tiny door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "tiny door", ref nextId);
        CreateDoor("Door_Glass_Admin_Tiny", "This is a tiny door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "tiny door", ref nextId);
        CreateDoor("Door_Normal_VerySmall", "This is a very small ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very small door", ref nextId);
        CreateDoor("Door_Tough_VerySmall", "This is a tough very small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very small door", ref nextId);
        CreateDoor("Door_Secure_VerySmall", "This is a very small door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very small door", ref nextId);
        CreateDoor("Door_Admin_VerySmall", "This is a very small door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very small door", ref nextId);
        CreateDoor("Door_Bad_VerySmall", "This is a bad very small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very small door", ref nextId);
        CreateDoor("Gate_Normal_VerySmall", "This is an ordinary very small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very small gate", ref nextId);
        CreateDoor("Gate_Tough_VerySmall", "This is a tough very small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very small gate", ref nextId);
        CreateDoor("Gate_Secure_VerySmall", "This is a tough very small gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very small gate", ref nextId);
        CreateDoor("Gate_Admin_VerySmall", "This is a very small gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very small gate", ref nextId);
        CreateDoor("Gate_Bad_VerySmall", "This is a bad very small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very small gate", ref nextId);
        CreateDoor("Door_Glass_VerySmall", "This is a very small door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very small door", ref nextId);
        CreateDoor("Door_Glass_Secure_VerySmall", "This is a very small door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "very small door", ref nextId);
        CreateDoor("Door_Glass_Admin_VerySmall", "This is a very small door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very small door", ref nextId);
        CreateDoor("Door_Normal_Small", "This is a small ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "small door", ref nextId);
        CreateDoor("Door_Tough_Small", "This is a tough small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "small door", ref nextId);
        CreateDoor("Door_Secure_Small", "This is a small door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "small door", ref nextId);
        CreateDoor("Door_Admin_Small", "This is a small door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "small door", ref nextId);
        CreateDoor("Door_Bad_Small", "This is a bad small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "small door", ref nextId);
        CreateDoor("Gate_Normal_Small", "This is an ordinary small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "small gate", ref nextId);
        CreateDoor("Gate_Tough_Small", "This is a tough small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "small gate", ref nextId);
        CreateDoor("Gate_Secure_Small", "This is a tough small gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "small gate", ref nextId);
        CreateDoor("Gate_Admin_Small", "This is a small gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "small gate", ref nextId);
        CreateDoor("Gate_Bad_Small", "This is a bad small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "small gate", ref nextId);
        CreateDoor("Door_Glass_Small", "This is a small door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "small door", ref nextId);
        CreateDoor("Door_Glass_Secure_Small", "This is a small door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "small door", ref nextId);
        CreateDoor("Door_Glass_Admin_Small", "This is a small door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "small door", ref nextId);
        CreateDoor("Door_Normal_Large", "This is a large ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "large door", ref nextId);
        CreateDoor("Door_Tough_Large", "This is a tough large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "large door", ref nextId);
        CreateDoor("Door_Secure_Large", "This is a large door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "large door", ref nextId);
        CreateDoor("Door_Admin_Large", "This is a large door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "large door", ref nextId);
        CreateDoor("Door_Bad_Large", "This is a bad large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "large door", ref nextId);
        CreateDoor("Gate_Normal_Large", "This is an ordinary large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "large gate", ref nextId);
        CreateDoor("Gate_Tough_Large", "This is a tough large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "large gate", ref nextId);
        CreateDoor("Gate_Secure_Large", "This is a tough large gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "large gate", ref nextId);
        CreateDoor("Gate_Admin_Large", "This is a large gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "large gate", ref nextId);
        CreateDoor("Gate_Bad_Large", "This is a bad large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "large gate", ref nextId);
        CreateDoor("Door_Glass_Large", "This is a large door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "large door", ref nextId);
        CreateDoor("Door_Glass_Secure_Large", "This is a large door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "large door", ref nextId);
        CreateDoor("Door_Glass_Admin_Large", "This is a large door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "large door", ref nextId);
        CreateDoor("Door_Normal_VeryLarge", "This is a very large ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very large door", ref nextId);
        CreateDoor("Door_Tough_VeryLarge", "This is a tough very large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very large door", ref nextId);
        CreateDoor("Door_Secure_VeryLarge", "This is a very large door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very large door", ref nextId);
        CreateDoor("Door_Admin_VeryLarge", "This is a very large door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very large door", ref nextId);
        CreateDoor("Door_Bad_VeryLarge", "This is a bad very large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very large door", ref nextId);
        CreateDoor("Gate_Normal_VeryLarge", "This is an ordinary very large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very large gate", ref nextId);
        CreateDoor("Gate_Tough_VeryLarge", "This is a tough very large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very large gate", ref nextId);
        CreateDoor("Gate_Secure_VeryLarge", "This is a tough very large gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very large gate", ref nextId);
        CreateDoor("Gate_Admin_VeryLarge", "This is a very large gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very large gate", ref nextId);
        CreateDoor("Gate_Bad_VeryLarge", "This is a bad very large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very large gate", ref nextId);
        CreateDoor("Door_Glass_VeryLarge", "This is a very large door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very large door", ref nextId);
        CreateDoor("Door_Glass_Secure_VeryLarge", "This is a very large door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "very large door", ref nextId);
        CreateDoor("Door_Glass_Admin_VeryLarge", "This is a very large door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very large door", ref nextId);
        CreateDoor("Door_Normal_Huge", "This is a huge ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "huge door", ref nextId);
        CreateDoor("Door_Tough_Huge", "This is a tough huge door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "huge door", ref nextId);
        CreateDoor("Door_Secure_Huge", "This is a huge door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "huge door", ref nextId);
        CreateDoor("Door_Admin_Huge", "This is a huge door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "huge door", ref nextId);
        CreateDoor("Door_Bad_Huge", "This is a bad huge door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "huge door", ref nextId);
        CreateDoor("Gate_Normal_Huge", "This is an ordinary huge gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "huge gate", ref nextId);
        CreateDoor("Gate_Tough_Huge", "This is a tough huge gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "huge gate", ref nextId);
        CreateDoor("Gate_Secure_Huge", "This is a tough huge gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "huge gate", ref nextId);
        CreateDoor("Gate_Admin_Huge", "This is a huge gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "huge gate", ref nextId);
        CreateDoor("Gate_Bad_Huge", "This is a bad huge gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "huge gate", ref nextId);
        CreateDoor("Door_Glass_Huge", "This is a huge door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "huge door", ref nextId);
        CreateDoor("Door_Glass_Secure_Huge", "This is a huge door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "huge door", ref nextId);
        CreateDoor("Door_Glass_Admin_Huge", "This is a huge door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "huge door", ref nextId);
        CreateDoor("Door_Normal_Enormous", "This is an enormous ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "enormous door", ref nextId);
        CreateDoor("Door_Tough_Enormous", "This is a tough enormous door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "enormous door", ref nextId);
        CreateDoor("Door_Secure_Enormous", "This is an enormous door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "enormous door", ref nextId);
        CreateDoor("Door_Admin_Enormous", "This is an enormous door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "enormous door", ref nextId);
        CreateDoor("Door_Bad_Enormous", "This is a bad enormous door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "enormous door", ref nextId);
        CreateDoor("Gate_Normal_Enormous", "This is an ordinary enormous gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "enormous gate", ref nextId);
        CreateDoor("Gate_Tough_Enormous", "This is a tough enormous gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "enormous gate", ref nextId);
        CreateDoor("Gate_Secure_Enormous", "This is a tough enormous gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "enormous gate", ref nextId);
        CreateDoor("Gate_Admin_Enormous", "This is an enormous gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "enormous gate", ref nextId);
        CreateDoor("Gate_Bad_Enormous", "This is a bad enormous gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "enormous gate", ref nextId);
        CreateDoor("Door_Glass_Enormous", "This is an enormous door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "enormous door", ref nextId);
        CreateDoor("Door_Glass_Secure_Enormous", "This is an enormous door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "enormous door", ref nextId);
        CreateDoor("Door_Glass_Admin_Enormous", "This is an enormous door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "enormous door", ref nextId);
        CreateDoor("Door_Normal_Gigantic", "This is a gigantic ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gigantic door", ref nextId);
        CreateDoor("Door_Tough_Gigantic", "This is a tough gigantic door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gigantic door", ref nextId);
        CreateDoor("Door_Secure_Gigantic", "This is a gigantic door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gigantic door", ref nextId);
        CreateDoor("Door_Admin_Gigantic", "This is a gigantic door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gigantic door", ref nextId);
        CreateDoor("Door_Bad_Gigantic", "This is a bad gigantic door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gigantic door", ref nextId);
        CreateDoor("Gate_Normal_Gigantic", "This is an ordinary gigantic gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gigantic gate", ref nextId);
        CreateDoor("Gate_Tough_Gigantic", "This is a tough gigantic gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gigantic gate", ref nextId);
        CreateDoor("Gate_Secure_Gigantic", "This is a tough gigantic gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gigantic gate", ref nextId);
        CreateDoor("Gate_Admin_Gigantic", "This is a gigantic gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gigantic gate", ref nextId);
        CreateDoor("Gate_Bad_Gigantic", "This is a bad gigantic gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gigantic gate", ref nextId);
        CreateDoor("Door_Glass_Gigantic", "This is a gigantic door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gigantic door", ref nextId);
        CreateDoor("Door_Glass_Secure_Gigantic", "This is a gigantic door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "gigantic door", ref nextId);
        CreateDoor("Door_Glass_Admin_Gigantic", "This is a gigantic door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gigantic door", ref nextId);

        _context.SaveChanges();

        GameItemComponentProto CreateLockableDoor(string name, string description, bool seeThrough, bool canFireThrough, bool canUninstall, Difficulty uninstallHinge, Difficulty uninstallNotHinge, bool canSmash, Difficulty smashDifficulty, string exitDescription, Difficulty force, Difficulty pick, string lockType, ref long id)
        {
            return CreateItemProto(id++, now, "LockingDoor", name, description,
                $@"<Definition SeeThrough=""{seeThrough}"" CanFireThrough=""{canFireThrough}"">
  <ForceDifficulty>{(int)force}</ForceDifficulty>
  <PickDifficulty>{(int)pick}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[@ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[@ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0 is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0 is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>{lockType}</LockType>
  <InstalledExitDescription>{exitDescription}</InstalledExitDescription><Uninstall CanPlayersUninstall=""{canUninstall}"" UninstallDifficultyHingeSide=""{(int)uninstallHinge}"" UninstallDifficultyNotHingeSide=""{(int)uninstallNotHinge}"" UninstallTrait=""{doorTrait.Id}"" /><Smash CanPlayersSmash=""{canSmash}"" SmashDifficulty=""{(int)smashDifficulty}"" /></Definition>");
        }

        CreateLockableDoor("Door_Lockable_Normal", "This is an ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough", "This is a tough door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure", "This is a door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin", "This is a door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad", "This is a bad door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal", "This is an ordinary gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough", "This is a tough gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure", "This is a tough gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin", "This is a gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad", "This is a bad gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass", "This is a door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure", "This is a door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin", "This is a door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_Tiny", "This is a tiny ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "tiny door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_Tiny", "This is a tough tiny door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "tiny door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_Tiny", "This is a tiny door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "tiny door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_Tiny", "This is a tiny door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "tiny door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_Tiny", "This is a bad tiny door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "tiny door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_Tiny", "This is an ordinary tiny gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "tiny gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_Tiny", "This is a tough tiny gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "tiny gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_Tiny", "This is a tough tiny gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "tiny gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_Tiny", "This is a tiny gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "tiny gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_Tiny", "This is a bad tiny gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "tiny gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Tiny", "This is a tiny door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "tiny door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_Tiny", "This is a tiny door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "tiny door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_Tiny", "This is a tiny door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "tiny door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_VerySmall", "This is a very small ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_VerySmall", "This is a tough very small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very small door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_VerySmall", "This is a very small door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very small door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_VerySmall", "This is a very small door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_VerySmall", "This is a bad very small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very small door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_VerySmall", "This is an ordinary very small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very small gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_VerySmall", "This is a tough very small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very small gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_VerySmall", "This is a tough very small gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very small gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_VerySmall", "This is a very small gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very small gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_VerySmall", "This is a bad very small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very small gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_VerySmall", "This is a very small door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_VerySmall", "This is a very small door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "very small door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_VerySmall", "This is a very small door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_Small", "This is a small ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_Small", "This is a tough small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "small door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_Small", "This is a small door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "small door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_Small", "This is a small door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_Small", "This is a bad small door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "small door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_Small", "This is an ordinary small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "small gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_Small", "This is a tough small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "small gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_Small", "This is a tough small gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "small gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_Small", "This is a small gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "small gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_Small", "This is a bad small gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "small gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Small", "This is a small door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_Small", "This is a small door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "small door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_Small", "This is a small door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "small door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_Large", "This is a large ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_Large", "This is a tough large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "large door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_Large", "This is a large door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "large door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_Large", "This is a large door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_Large", "This is a bad large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "large door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_Large", "This is an ordinary large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "large gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_Large", "This is a tough large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "large gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_Large", "This is a tough large gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "large gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_Large", "This is a large gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "large gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_Large", "This is a bad large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "large gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Large", "This is a large door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_Large", "This is a large door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "large door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_Large", "This is a large door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_VeryLarge", "This is a very large ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_VeryLarge", "This is a tough very large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very large door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_VeryLarge", "This is a very large door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very large door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_VeryLarge", "This is a very large door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_VeryLarge", "This is a bad very large door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very large door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_VeryLarge", "This is an ordinary very large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "very large gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_VeryLarge", "This is a tough very large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "very large gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_VeryLarge", "This is a tough very large gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "very large gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_VeryLarge", "This is a very large gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very large gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_VeryLarge", "This is a bad very large gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very large gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_VeryLarge", "This is a very large door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "very large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_VeryLarge", "This is a very large door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "very large door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_VeryLarge", "This is a very large door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "very large door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_Huge", "This is a huge ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "huge door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_Huge", "This is a tough huge door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "huge door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_Huge", "This is a huge door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "huge door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_Huge", "This is a huge door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "huge door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_Huge", "This is a bad huge door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "huge door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_Huge", "This is an ordinary huge gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "huge gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_Huge", "This is a tough huge gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "huge gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_Huge", "This is a tough huge gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "huge gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_Huge", "This is a huge gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "huge gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_Huge", "This is a bad huge gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "huge gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Huge", "This is a huge door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "huge door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_Huge", "This is a huge door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "huge door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_Huge", "This is a huge door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "huge door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_Enormous", "This is an enormous ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "enormous door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_Enormous", "This is a tough enormous door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "enormous door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_Enormous", "This is an enormous door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "enormous door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_Enormous", "This is an enormous door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "enormous door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_Enormous", "This is a bad enormous door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "enormous door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_Enormous", "This is an ordinary enormous gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "enormous gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_Enormous", "This is a tough enormous gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "enormous gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_Enormous", "This is a tough enormous gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "enormous gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_Enormous", "This is an enormous gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "enormous gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_Enormous", "This is a bad enormous gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "enormous gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Enormous", "This is an enormous door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "enormous door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_Enormous", "This is an enormous door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "enormous door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_Enormous", "This is an enormous door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "enormous door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Normal_Gigantic", "This is a gigantic ordinary door that can be smashed and uninstalled", false, false, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gigantic door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Tough_Gigantic", "This is a tough gigantic door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gigantic door", Difficulty.VeryHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Secure_Gigantic", "This is a gigantic door that can be smashed and uninstalled only from the hinge side", false, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gigantic door", Difficulty.ExtremelyHard, Difficulty.VeryHard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Admin_Gigantic", "This is a gigantic door that cannot be removed or smashed by players", false, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gigantic door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Bad_Gigantic", "This is a bad gigantic door that can be smashed and uninstalled", false, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gigantic door", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Normal_Gigantic", "This is an ordinary gigantic gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Easy, Difficulty.Insane, true, Difficulty.Normal, "gigantic gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Tough_Gigantic", "This is a tough gigantic gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.Insane, true, Difficulty.ExtremelyHard, "gigantic gate", Difficulty.ExtremelyHard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Secure_Gigantic", "This is a tough gigantic gate that can be seen and fired through, smashed and uninstalled from the hinge side only", true, true, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.ExtremelyHard, "gigantic gate", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Admin_Gigantic", "This is a gigantic gate that cannot be smashed or uninstalled by players", true, true, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gigantic gate", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Gate_Lockable_Bad_Gigantic", "This is a bad gigantic gate that can be seen and fired through, smashed and uninstalled", true, true, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gigantic gate", Difficulty.Normal, Difficulty.Easy, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Gigantic", "This is a gigantic door that can be seen through, smashed and uninstalled", true, false, true, Difficulty.Normal, Difficulty.VeryHard, true, Difficulty.VeryEasy, "gigantic door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Secure_Gigantic", "This is a gigantic door that can be seen through, smashed and uninstalled from hinge side only", true, false, true, Difficulty.Normal, Difficulty.Impossible, true, Difficulty.VeryEasy, "gigantic door", Difficulty.ExtremelyHard, Difficulty.Hard, "Warded_Lock", ref nextId);
        CreateLockableDoor("Door_Lockable_Glass_Admin_Gigantic", "This is a gigantic door that can be seen through, but not smashed or uninstalled", true, false, false, Difficulty.Impossible, Difficulty.Impossible, false, Difficulty.Impossible, "gigantic door", Difficulty.Hard, Difficulty.Normal, "Warded_Lock", ref nextId);
    }

    private void SeedLocks(DateTime now, ref long nextId)
    {
        #region Locks

        long currentId = nextId;

        GameItemComponentProto CreateWardedLock(string name, string description, Difficulty force, Difficulty pick)
        {
            return CreateItemProto(currentId++, now, "Simple Lock", name, description,
                    $@"<Definition>
  <ForceDifficulty>{(int)force}</ForceDifficulty>
  <PickDifficulty>{(int)pick}</PickDifficulty>
  <LockEmote><![CDATA[@ lock|locks $1$?3| on $3||$$?2| with $2||$]]></LockEmote>
  <UnlockEmote><![CDATA[@ unlock|unlocks $1$?3| on $3||$$?2| with $2||$]]></UnlockEmote>
  <LockEmoteNoActor><![CDATA[$0$?1| on $1||$ lock|locks]]></LockEmoteNoActor>
  <UnlockEmoteNoActor><![CDATA[$0$?1| on $1||$ unlock|unlocks]]></UnlockEmoteNoActor>
  <LockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is locked from the other side.]]></LockEmoteOtherSide>
  <UnlockEmoteOtherSide><![CDATA[$0$?1| on $1||$ is unlocked from the other side.]]></UnlockEmoteOtherSide>
  <LockType>Warded Lock</LockType>
</Definition>");
        }

        GameItemComponentProto CreateLatch(string name, string description, Difficulty force, Difficulty pick,
            string lockEmote = "@ latch|latches $1$?2| on $2||$",
            string unlockEmote = "@ unlatch|unlatches $1$?2| on $2||$",
            string lockEmoteNoActor = "$0$?1| on $1||$ open|opens",
            string unlockEmoteNoActor = "$0$?1| on $1||$ close|closes",
            string lockEmoteOtherSide = "$0$?1| on $1||$ is latched from the other side.",
            string unlockEmoteOtherSide = "$0$?1| on $1||$ is unlatched from the other side.")
        {
            return CreateItemProto(currentId++, now, "Latch", name, description,
                new XElement("Definition",
                    new XElement("ForceDifficulty", (int)force),
                    new XElement("PickDifficulty", (int)pick),
                    new XElement("LockEmote", new XCData(lockEmote)),
                    new XElement("UnlockEmote", new XCData(unlockEmote)),
                    new XElement("LockEmoteNoActor", new XCData(lockEmoteNoActor)),
                    new XElement("UnlockEmoteNoActor", new XCData(unlockEmoteNoActor)),
                    new XElement("LockEmoteOtherSide", new XCData(lockEmoteOtherSide)),
                    new XElement("UnlockEmoteOtherSide", new XCData(unlockEmoteOtherSide))).ToString());
        }

        GameItemComponentProto CreateSimpleKey(string name, string description, string lockType)
        {
            return CreateItemProto(currentId++, now, "Simple Key", name, description,
                    @$"<Definition>
  <LockType>{lockType}</LockType>
</Definition>");
        }

        CreateWardedLock("Warded_Lock_Terrible", "This is a terrible simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Normal, Difficulty.VeryEasy);
        CreateWardedLock("Warded_Lock_Bad", "This is a bad simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Easy, Difficulty.Easy);
        CreateWardedLock("Warded_Lock_Normal", "This is a normal simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Hard, Difficulty.Normal);
        CreateWardedLock("Warded_Lock_Good", "This is a good simple lock in the 'warded' type (most pre-modern systems)", Difficulty.VeryHard, Difficulty.Hard);
        CreateWardedLock("Warded_Lock_Excellent", "This is an excellent simple lock in the 'warded' type (most pre-modern systems)", Difficulty.ExtremelyHard, Difficulty.VeryHard);
        CreateWardedLock("Warded_Lock_Master", "This is a masterful simple lock in the 'warded' type (most pre-modern systems)", Difficulty.ExtremelyHard, Difficulty.ExtremelyHard);
        CreateWardedLock("Warded_Lock_Legendary", "This is a legendary simple lock in the 'warded' type (most pre-modern systems)", Difficulty.Insane, Difficulty.Insane);
        CreateSimpleKey("Warded_Key", "This is a key for locks in the 'warded' type (most pre-modern systems)", "Warded Lock");
        CreateLatch("Latch_Terrible", "This is a terrible quality simple latch (one-sided lock)", Difficulty.ExtremelyEasy, Difficulty.Easy);
        CreateLatch("Latch_Bad", "This is a bad quality simple latch (one-sided lock)", Difficulty.VeryEasy, Difficulty.Normal);
        CreateLatch("Latch_Normal", "This is a normal quality simple latch (one-sided lock)", Difficulty.Easy, Difficulty.Hard);
        CreateLatch("Latch_Good", "This is a good quality simple latch (one-sided lock)", Difficulty.Hard, Difficulty.Hard);
        CreateLatch("Latch_Excellent", "This is an excellent quality simple latch (one-sided lock)", Difficulty.VeryHard, Difficulty.VeryHard);
        CreateLatch("Latch_Master", "This is a masterful quality simple latch (one-sided lock)", Difficulty.ExtremelyHard, Difficulty.VeryHard);
        CreateLatch("Latch_Legendary", "This is a legendary quality simple latch (one-sided lock)", Difficulty.Insane, Difficulty.ExtremelyHard);
        CreateLatch("Latch_Admin", "This is a simple latch (one-sided lock) that cannot be picked or forced", Difficulty.Impossible, Difficulty.Impossible);
        CreateLatch("Latch_Container_Hook", "This is a small hook-and-eye latch for cupboards, hatches and light containers",
            Difficulty.Easy, Difficulty.VeryEasy,
            "@ hook|hooks $1$?2| on $2||$ closed",
            "@ unhook|unhooks $1$?2| on $2||$",
            "$0$?1| on $1||$ settles into its hook.",
            "$0$?1| on $1||$ slips free of its hook.",
            "$0$?1| on $1||$ is hooked closed from the other side.",
            "$0$?1| on $1||$ is unhooked from the other side.");
        CreateLatch("Latch_Container_Hasp", "This is a sturdier hasp latch for chests, trunks and other lockable containers",
            Difficulty.Normal, Difficulty.Normal,
            "@ drop|drops $1$?2| on $2||$ into its hasp",
            "@ lift|lifts $1$?2| on $2||$ free of its hasp",
            "$0$?1| on $1||$ drops into its hasp.",
            "$0$?1| on $1||$ lifts free of its hasp.",
            "$0$?1| on $1||$ is secured by a hasp from the other side.",
            "$0$?1| on $1||$ is released from its hasp on the other side.");
        CreateLatch("Latch_Door_Bar", "This is a heavy sliding bar latch for doors and interior shutters",
            Difficulty.VeryHard, Difficulty.Hard,
            "@ slide|slides $1$?2| across $2||$",
            "@ draw|draws $1$?2| back from $2||$",
            "$0$?1| on $1||$ slides heavily into place.",
            "$0$?1| on $1||$ slides heavily aside.",
            "$0$?1| on $1||$ is barred from the other side.",
            "$0$?1| on $1||$ is unbarred from the other side.");
        CreateLatch("Latch_Gate_DropBar", "This is a weighty drop-bar latch for yard gates, stable gates and palisade gates",
            Difficulty.ExtremelyHard, Difficulty.Hard,
            "@ drop|drops $1$?2| across $2||$ into its brackets",
            "@ heft|hefts $1$?2| clear of $2||$",
            "$0$?1| on $1||$ drops into its gate brackets.",
            "$0$?1| on $1||$ lifts clear of its gate brackets.",
            "$0$?1| on $1||$ is barred by a heavy drop-bar from the other side.",
            "$0$?1| on $1||$ is freed from its drop-bar on the other side.");
        CreateLatch("Latch_Portcullis_Pawl", "This is a winch pawl or brake latch for holding a portcullis or similar heavy barrier",
            Difficulty.Insane, Difficulty.VeryHard,
            "@ set|sets $1$?2| on $2||$ to hold the portcullis mechanism",
            "@ release|releases $1$?2| on $2||$ from the portcullis mechanism",
            "$0$?1| on $1||$ bites into the portcullis mechanism.",
            "$0$?1| on $1||$ releases the portcullis mechanism.",
            "$0$?1| on $1||$ locks the portcullis mechanism from the other side.",
            "$0$?1| on $1||$ releases the portcullis mechanism from the other side.");

        nextId = currentId;
        _context.SaveChanges();

        #endregion
    }

    private void SeedWritingImplements(FuturemudDatabaseContext context, DateTime now, Account dbaccount, ref long nextId)
    {
        SeedBasicWritingImplements(context, now, dbaccount, ref nextId);
        SeedVariableWritingImplements(context, now, dbaccount, ref nextId);
    }

    private void SeedBasicWritingImplements(FuturemudDatabaseContext context, DateTime now, Account dbaccount, ref long nextId)
    {
        #region Writing Implements

        long currentId = nextId;
        GameItemComponentProto holdable = context.GameItemComponentProtos.First(x => x.Type == "Holdable");
        GameItemComponentProto stack = context.GameItemComponentProtos.First(x => x.Name == "Stack_Number");
        Material paperMaterial = context.Materials.First(x => x.Name == "Paper");

        GameItemComponentProto CreatePaperSheet(string name, string description, int maxCharacters)
        {
            return CreateItemProto(currentId++, now, "PaperSheet", name, description,
                    @$"<Definition>
   <MaximumCharacterLengthOfText>{maxCharacters}</MaximumCharacterLengthOfText>
 </Definition>");
        }

        GameItemComponentProto CreateBiro(string name, string description, long colourId, int totalUses)
        {
            return CreateItemProto(currentId++, now, "Biro", name, description,
                    @$"<Definition>
   <Colour>{colourId}</Colour>
   <TotalUses>{totalUses}</TotalUses>
 </Definition>");
        }

        GameItemComponentProto CreatePencil(string name, string description, long colourId, int usesBeforeSharpening, int totalUses)
        {
            return CreateItemProto(currentId++, now, "Pencil", name, description,
                    @$"<Definition>
   <Colour>{colourId}</Colour>
   <UsesBeforeSharpening>{usesBeforeSharpening}</UsesBeforeSharpening>
   <TotalUses>{totalUses}</TotalUses>
 </Definition>");
        }

        GameItemComponentProto CreateBook(string name, string description, int pages, GameItemProto page)
        {
            GameItemComponentProto component = new()
            {
                Id = currentId++,
                RevisionNumber = 0,
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
                Type = "Book",
                Name = name,
                Description = description,
                Definition =
                    $@"<Definition>
   <PaperProto>{page.Id}</PaperProto>
   <PageCount>{pages}</PageCount>
 </Definition>"
            };
            AddGameItemComponent(context, component);
            return component;
        }

        GameItemComponentProto paperA4 = CreatePaperSheet("Paper_A4", "This is a sheet of paper in A4 size (~ US Letter size)", 4160);

        long nextItemId = context.GameItemProtos.Max(x => x.Id) + 1;

        GameItemProto a4paper = new()
        {
            Id = nextItemId++,
            RevisionNumber = 0,
            Name = "sheet",
            Keywords = "sheet paper",
            MaterialId = paperMaterial.Id,
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
            Size = (int)SizeCategory.Tiny,
            Weight = 1,
            ReadOnly = false,
            BaseItemQuality = 5,
            HighPriority = false,
            ShortDescription = "a sheet of paper",
            FullDescription = "This is a sheet of plain, unlined paper approximately 8 inches by 12 inches in size."
        };
        a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a4paper, GameItemComponent = holdable });
        a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a4paper, GameItemComponent = stack });
        a4paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a4paper, GameItemComponent = paperA4 });
        context.GameItemProtos.Add(a4paper);
        context.SaveChanges();

        GameItemComponentProto paperA3 = CreatePaperSheet("Paper_A3", "This is a sheet of paper in A3 size (~ US Ledger size)", 8320);

        GameItemProto a3paper = new()
        {
            Id = nextItemId++,
            RevisionNumber = 0,
            Name = "sheet",
            Keywords = "large sheet paper",
            MaterialId = paperMaterial.Id,
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
            Size = (int)SizeCategory.VerySmall,
            Weight = 2,
            ReadOnly = false,
            BaseItemQuality = 5,
            HighPriority = false,
            ShortDescription = "a large sheet of paper",
            FullDescription = "This is a large sheet of plain, unlined paper approximately 12 inches by 16 inches in size."
        };
        a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a3paper, GameItemComponent = holdable });
        a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a3paper, GameItemComponent = stack });
        a3paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a3paper, GameItemComponent = paperA3 });
        context.GameItemProtos.Add(a3paper);
        context.SaveChanges();

        GameItemComponentProto paperA5 = CreatePaperSheet("Paper_A5", "This is a sheet of paper in A5 size (~ US Half Letter size)", 8320);

        GameItemProto a5paper = new()
        {
            Id = nextItemId++,
            RevisionNumber = 0,
            Name = "sheet",
            Keywords = "small sheet paper",
            MaterialId = paperMaterial.Id,
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
            Size = (int)SizeCategory.VerySmall,
            Weight = 0.5,
            ReadOnly = false,
            BaseItemQuality = 5,
            HighPriority = false,
            ShortDescription = "a small sheet of paper",
            FullDescription = "This is a small sheet of plain, unlined paper approximately 5 inches by 8 inches in size."
        };
        a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a5paper, GameItemComponent = holdable });
        a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a5paper, GameItemComponent = stack });
        a5paper.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos { GameItemProto = a5paper, GameItemComponent = paperA5 });
        context.GameItemProtos.Add(a5paper);
        context.SaveChanges();

        CreateBook("Book_20_Page", "This is a 20 page book of A4 pages", 20, a4paper);
        CreateBook("Book_40_Page", "This is a 40 page book of A4 pages", 40, a4paper);
        CreateBook("Book_90_Page", "This is a 90 page book of A4 pages", 90, a4paper);
        CreateBook("Book_200_Page", "This is a 200 page book of A4 pages", 200, a4paper);
        CreateBook("Book_500_Page", "This is a 500 page book of A4 pages", 500, a4paper);
        CreateBook("Book_1000_Page", "This is a 1000 page book of A4 pages", 1000, a4paper);
        CreateBook("Book_Small_20_Page", "This is a 20 page book of A5 pages", 20, a5paper);
        CreateBook("Book_Small_40_Page", "This is a 40 page book of A5 pages", 40, a5paper);
        CreateBook("Book_Small_90_Page", "This is a 90 page book of A5 pages", 90, a5paper);
        CreateBook("Book_Small_200_Page", "This is a 200 page book of A5 pages", 200, a5paper);
        CreateBook("Book_Small_500_Page", "This is a 500 page book of A5 pages", 500, a5paper);
        CreateBook("Book_Small_1000_Page", "This is a 1000 page book of A5 pages", 1000, a5paper);
        CreateBook("Book_Large_20_Page", "This is a 20 page book of A3 pages", 20, a3paper);
        CreateBook("Book_Large_40_Page", "This is a 40 page book of A3 pages", 40, a3paper);
        CreateBook("Book_Large_90_Page", "This is a 90 page book of A3 pages", 90, a3paper);
        CreateBook("Book_Large_200_Page", "This is a 200 page book of A3 pages", 200, a3paper);
        CreateBook("Book_Large_500_Page", "This is a 500 page book of A3 pages", 500, a3paper);
        CreateBook("Book_Large_1000_Page", "This is a 1000 page book of A3 pages", 1000, a3paper);

        CreateBiro("Biro_Black", "This is a standard black biro pen", context.Colours.First(x => x.Name == "black").Id, 110000);
        CreateBiro("Biro_Blue", "This is a standard blue biro pen", context.Colours.First(x => x.Name == "blue").Id, 110000);
        CreateBiro("Biro_Red", "This is a standard red biro pen", context.Colours.First(x => x.Name == "red").Id, 110000);
        CreatePencil("Pencil_Black", "This is a standard black pencil", context.Colours.First(x => x.Name == "black").Id, 11000, 220000);

        nextId = currentId;
        context.SaveChanges();

        #endregion
    }
}

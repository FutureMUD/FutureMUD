using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework.Save;
using MudSharp.Models;

namespace MudSharp.Framework.Units;

public class Unit : SaveableItem, IUnit
{
	/// <inheritdoc />
	public override string FrameworkItemType => "Unit";

	/// <summary>
	///     Private backing field for Abbreviations Property
	/// </summary>
	private readonly List<string> _abbreviations;

	public Unit(IFuturemud gameworld, string name, string system, string primaryAbbreviation, UnitType type, double multiplier)
	{
		Gameworld = gameworld;
		_name = name;
		System = system;
		Type = type;
		MultiplierFromBase = multiplier;
		DescriberUnit = true;
		SpaceBetween = true;
		PreMultiplierOffsetFrombase = 0.0;
		PostMultiplierOffsetFrombase = 0.0;
		PrimaryAbbreviation = primaryAbbreviation;
		_abbreviations = [primaryAbbreviation];
		DoDatabaseInsert();
	}

	private Unit(Unit rhs, string newName, string newPrimary)
	{
		Gameworld = rhs.Gameworld;
		_name = newName;
		_abbreviations = rhs._abbreviations.ToList();
		PrimaryAbbreviation = newPrimary;
		if (!_abbreviations.Any(x => x.EqualTo(newPrimary)))
		{
			_abbreviations.Add(newPrimary);
		}

		MultiplierFromBase = rhs.MultiplierFromBase;
		PostMultiplierOffsetFrombase = rhs.PostMultiplierOffsetFrombase;
		PreMultiplierOffsetFrombase = rhs.PreMultiplierOffsetFrombase;
		DefaultUnitForSystem = false;
		SpaceBetween = rhs.SpaceBetween;
		DescriberUnit = rhs.DescriberUnit;
		System = rhs.System;
		Type = rhs.Type;
		DoDatabaseInsert();
	}

	private void DoDatabaseInsert()
	{
		using (new FMDB())
		{
			var dbitem = new Models.UnitOfMeasure();
			dbitem.Name = dbitem.Name;
			dbitem.Abbreviations = _abbreviations.ListToCommaSeparatedValues(" ");
			dbitem.BaseMultiplier = MultiplierFromBase;
			dbitem.Type = (int)Type;
			dbitem.Describer = DescriberUnit;
			dbitem.SpaceBetween = SpaceBetween;
			dbitem.System = System;
			dbitem.PreMultiplierBaseOffset = PreMultiplierOffsetFrombase;
			dbitem.PostMultiplierBaseOffset = PostMultiplierOffsetFrombase;
			dbitem.PrimaryAbbreviation = PrimaryAbbreviation;
			dbitem.DefaultUnitForSystem = DefaultUnitForSystem;
			FMDB.Context.UnitsOfMeasure.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Unit(IFuturemud gameworld, Models.UnitOfMeasure uom)
	{
		Gameworld = gameworld;
		_id = uom.Id;
		_name = uom.Name;
		_abbreviations = uom.Abbreviations.Split(' ').ToList();
		MultiplierFromBase = uom.BaseMultiplier;
		Type = (UnitType)uom.Type;
		DescriberUnit = uom.Describer;
		SpaceBetween = uom.SpaceBetween;
		System = uom.System;
		PreMultiplierOffsetFrombase = uom.PreMultiplierBaseOffset;
		PostMultiplierOffsetFrombase = uom.PostMultiplierBaseOffset;
		PrimaryAbbreviation = !string.IsNullOrWhiteSpace(uom.PrimaryAbbreviation) ? uom.PrimaryAbbreviation : uom.Name;
		DefaultUnitForSystem = uom.DefaultUnitForSystem;
	}

	/// <inheritdoc />
	public override void Save()
	{
		var dbitem = FMDB.Context.UnitsOfMeasure.Find(Id);
		dbitem.Name = dbitem.Name;
		dbitem.Abbreviations = _abbreviations.ListToCommaSeparatedValues(" ");
		dbitem.BaseMultiplier = MultiplierFromBase;
		dbitem.Type = (int)Type;
		dbitem.Describer = DescriberUnit;
		dbitem.SpaceBetween = SpaceBetween;
		dbitem.System = System;
		dbitem.PreMultiplierBaseOffset = PreMultiplierOffsetFrombase;
		dbitem.PostMultiplierBaseOffset = PostMultiplierOffsetFrombase;
		dbitem.PrimaryAbbreviation = PrimaryAbbreviation;
		dbitem.DefaultUnitForSystem = DefaultUnitForSystem;
		Changed = false;
	}

	public IUnit Clone(string newName, string newAbbreviation)
	{
		return new Unit(this, newName, newAbbreviation);
	}

	public string PrimaryAbbreviation { get; protected set; }

	/// <summary>
	///     A series of acceptable abbreviations that players may use for this unit when entering quantities
	/// </summary>
	public IEnumerable<string> Abbreviations => _abbreviations;

	/// <summary>
	///     The ratio of 1 of this unit to the base unit for this unit of measure
	/// </summary>
	public double MultiplierFromBase { get; protected set; }

	/// <summary>
	///     A flat offset applied before the multiplier
	/// </summary>
	public double PreMultiplierOffsetFrombase { get; protected set; }

	/// <summary>
	///     A flat offset applied after the multiplier
	/// </summary>
	public double PostMultiplierOffsetFrombase { get; protected set; }

	/// <summary>
	///     The fundamental physical property which this unit of measure represents
	/// </summary>
	public UnitType Type { get; protected set; }

	/// <summary>
	///     Whether or not this unit should be considered when asked to describe a quantity
	/// </summary>
	public bool DescriberUnit { get; protected set; }

	public bool SpaceBetween { get; protected set; }

	public bool LastDescriber { get; set; }

	public string System { get; set; }

	public bool DefaultUnitForSystem { get; set; }

	public void Delete()
	{
		Gameworld.SaveManager.Abort(this);
		if (_id != 0)
		{
			using (new FMDB())
			{
				Gameworld.SaveManager.Flush();
				var dbitem = FMDB.Context.UnitsOfMeasure.Find(Id);
				if (dbitem != null)
				{
					FMDB.Context.UnitsOfMeasure.Remove(dbitem);
					FMDB.Context.SaveChanges();
				}
			}
		}
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Unit #{Id.ToStringN0(actor)} - {Name}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"System: {System.ColourValue()}");
		sb.AppendLine($"Default Unit For System: {DefaultUnitForSystem.ToColouredString()}");
		sb.AppendLine($"Type: {Type.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Primary Abbreviation: {PrimaryAbbreviation.ColourValue()}");
		sb.AppendLine($"Other Abbreviations: {Abbreviations.ListToColouredString()}");
		sb.AppendLine($"Last Describer: {LastDescriber.ToColouredString()}");
		sb.AppendLine($"Space Between Number and Unit: {SpaceBetween.ToColouredString()}");
		sb.AppendLine($"Describer Unit: {DescriberUnit.ToColouredString()}");
		sb.AppendLine($"Unit Multiplier: {MultiplierFromBase.ToString("N", actor).ColourValue()}");
		sb.AppendLine($"Offset (Pre Multiplier): {PreMultiplierOffsetFrombase.ToString("N", actor).ColourValue()}");
		sb.AppendLine($"Offset (Post Multiplier): {PostMultiplierOffsetFrombase.ToString("N", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Sample Quantities".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		switch (Type)
		{
			case UnitType.Mass:
				sb.AppendLine($"1 Grain of Sand ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.000001, this, actor).ColourValue()}");
				sb.AppendLine($"1 Ant ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.01, this, actor).ColourValue()}");
				sb.AppendLine($"Gallon Jug of Milk ~= {Gameworld.UnitManager.DescribeSpecificUnit(3956, this, actor).ColourValue()}");
				sb.AppendLine($"Adult Male African Elephant ~= {Gameworld.UnitManager.DescribeSpecificUnit(6350000, this, actor).ColourValue()}");
				break;
			case UnitType.Length:
				sb.AppendLine($"Hair's Width ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.000075, this, actor).ColourValue()}");
				sb.AppendLine($"Staple Thickness ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.1, this, actor).ColourValue()}");
				sb.AppendLine($"Hen's Egg ~= {Gameworld.UnitManager.DescribeSpecificUnit(5.5, this, actor).ColourValue()}");
				sb.AppendLine($"Michael Jordan ~= {Gameworld.UnitManager.DescribeSpecificUnit(198, this, actor).ColourValue()}");
				sb.AppendLine($"747 Airplane ~= {Gameworld.UnitManager.DescribeSpecificUnit(7066.28, this, actor).ColourValue()}");
				break;
			case UnitType.FluidVolume:
				sb.AppendLine($"1 x Drop of Rain ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.00005, this, actor).ColourValue()}");
				sb.AppendLine($"1 x Teaspoon of Syrup ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.00492892, this, actor).ColourValue()}");
				sb.AppendLine($"Bottle of Wine ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.7, this, actor).ColourValue()}");
				sb.AppendLine($"Average Blood in Human Body ~= {Gameworld.UnitManager.DescribeSpecificUnit(5, this, actor).ColourValue()}");
				sb.AppendLine($"Volume of Water in Olympic Swimming Pool ~= {Gameworld.UnitManager.DescribeSpecificUnit(2500000, this, actor).ColourValue()}");
				break;
			case UnitType.Area:
				sb.AppendLine($"Average Fingernail Area ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.00077, this, actor).ColourValue()}");
				sb.AppendLine($"27 Inch Monitor Area ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.2009, this, actor).ColourValue()}");
				sb.AppendLine($"Regulation Basketball Court ~= {Gameworld.UnitManager.DescribeSpecificUnit(436.24, this, actor).ColourValue()}");
				sb.AppendLine($"Land Area of Texas ~= {Gameworld.UnitManager.DescribeSpecificUnit(695662000000, this, actor).ColourValue()}");
				break;
			case UnitType.Volume:
				sb.AppendLine($"A Bee's Brain ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.000000001, this, actor).ColourValue()}");
				sb.AppendLine($"A Human Eyeball ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.0000065, this, actor).ColourValue()}");
				sb.AppendLine($"A Regulation Basketball ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.00715, this, actor).ColourValue()}");
				sb.AppendLine($"An Adult Male Blue Whale ~= {Gameworld.UnitManager.DescribeSpecificUnit(120, this, actor).ColourValue()}");
				sb.AppendLine($"The Moon ~= {Gameworld.UnitManager.DescribeSpecificUnit(21_970_000_000_000_000_000.0, this, actor).ColourValue()}");
				break;
			case UnitType.Temperature:
				sb.AppendLine($"Absolute Zero ~= {Gameworld.UnitManager.DescribeSpecificUnit(-273.15d, this, actor).ColourValue()}");
				sb.AppendLine($"Freezing Point of Water ~= {Gameworld.UnitManager.DescribeSpecificUnit(0.0, this, actor).ColourValue()}");
				sb.AppendLine($"Room Temperature ~= {Gameworld.UnitManager.DescribeSpecificUnit(23.0, this, actor).ColourValue()}");
				sb.AppendLine($"Boiling Point of Water ~= {Gameworld.UnitManager.DescribeSpecificUnit(100.0, this, actor).ColourValue()}");
				sb.AppendLine($"Surface of the Sun ~= {Gameworld.UnitManager.DescribeSpecificUnit(5600.0, this, actor).ColourValue()}");
				sb.AppendLine($"Center of the Sun ~= {Gameworld.UnitManager.DescribeSpecificUnit(15000000.0, this, actor).ColourValue()}");
				break;
			case UnitType.TemperatureDelta:
				sb.AppendLine($"Difference between Normal Body Temp and Fever ~= {Gameworld.UnitManager.DescribeSpecificUnit(2, this, actor).ColourValue()}");
				sb.AppendLine($"Difference between Freezing and Boiling of Water ~= {Gameworld.UnitManager.DescribeSpecificUnit(100, this, actor).ColourValue()}");
				break;
			case UnitType.Force:
				sb.AppendLine($"Force to Break a Chicken's Egg ~= {Gameworld.UnitManager.DescribeSpecificUnit(50, this, actor).ColourValue()}");
				sb.AppendLine($"Average Human Bite ~= {Gameworld.UnitManager.DescribeSpecificUnit(720, this, actor).ColourValue()}");
				sb.AppendLine($"Great White Shark Bite ~= {Gameworld.UnitManager.DescribeSpecificUnit(18000, this, actor).ColourValue()}");
				sb.AppendLine($"Saturn V Rocket Thrust ~= {Gameworld.UnitManager.DescribeSpecificUnit(35000000, this, actor).ColourValue()}");
				sb.AppendLine($"Gravitational Attraction Between Earth and Sun ~= {Gameworld.UnitManager.DescribeSpecificUnit(35000000000000000000000.0, this, actor).ColourValue()}");
				break;
			case UnitType.Stress:
				sb.AppendLine($"Ordinary Systolic Blood Pressure ~= {Gameworld.UnitManager.DescribeSpecificUnit(16.0, this, actor).ColourValue()}");
				sb.AppendLine($"Atmospheric Pressure at Sea Level ~= {Gameworld.UnitManager.DescribeSpecificUnit(101325, this, actor).ColourValue()}");
				sb.AppendLine($"Yield Strength of Structural Steel ~= {Gameworld.UnitManager.DescribeSpecificUnit(250000000, this, actor).ColourValue()}");
				sb.AppendLine($"Young's Modulus of Structural Steel ~= {Gameworld.UnitManager.DescribeSpecificUnit(200000000000, this, actor).ColourValue()}");
				break;
			case UnitType.BMI:
				sb.AppendLine($"Severely Underweight < {Gameworld.UnitManager.DescribeSpecificUnit(16, this, actor).ColourValue()}");
				sb.AppendLine($"Underweight < {Gameworld.UnitManager.DescribeSpecificUnit(18.4, this, actor).ColourValue()}");
				sb.AppendLine($"Overweight > {Gameworld.UnitManager.DescribeSpecificUnit(25.0, this, actor).ColourValue()}");
				sb.AppendLine($"Obese > {Gameworld.UnitManager.DescribeSpecificUnit(30.0, this, actor).ColourValue()}");
				break;
		}
		return sb.ToString();
	}

	private const string BuildingCommandHelp = @"You can use the following options with this command:

	#3name <name>#0 - renames this unit
	#3system <system>#0 - changes the system that this unit belongs to
	#3type <type>#0 - changes the type of unit that this unit is
	#3abbreviation <which>#0 - toggles an abbreviation for this unit
	#3primary <which>#0 - sets the primary abbreviation for a unit
	#3multiplier <##>#0 - sets the multiplier for the unit
	#3preoffset <##>#0 - sets the pre-multiplier offset for the unit
	#3postoffset <##>#0 - sets the post-multiplier offset for the unit
	#3describer#0 - toggles whether this unit is used for description
	#3space#0 - toggles whether a space is put between the number and the unit
	#3default#0 - makes this the default unit for a system and type";

	public bool BuildingCommand(ICharacter actor, StringStack ss)
	{
		switch (ss.PopForSwitch())
		{
			case "name":
				return BuildingCommandName(actor, ss);
			case "system":
				return BuildingCommandSystem(actor, ss);
			case "abbreviation":
			case "abbr":
			case "abbrev":
				return BuildingCommandAbbreviation(actor, ss);
			case "primaryabbreviation":
			case "primaryabbr":
			case "primaryabbrev":
			case "primary":
				return BuildingCommandPrimaryAbbreviation(actor, ss);
			case "multiplier":
				return BuildingCommandMultiplier(actor, ss);
			case "preoffset":
				return BuildingCommandPreOffset(actor, ss);
			case "postoffset":
				return BuildingCommandPostOffset(actor, ss);
			case "type":
				return BuildingCommandType(actor, ss);
			case "describer":
				return BuildingCommandDescriber(actor);
			case "space":
			case "spacebetween":
				return BuildingCommandSpaceBetween(actor);
			case "default":
				return BuildingCommandDefault(actor);
			default:
				actor.OutputHandler.Send(BuildingCommandHelp.SubstituteANSIColour());
				return false;
		}
	}

	private bool BuildingCommandDefault(ICharacter actor)
	{
		if (DefaultUnitForSystem)
		{
			actor.OutputHandler.Send("There must always be one default unit for each system; change the default for this unit and system by setting another unit as the default instead.");
			return false;
		}

		
		foreach (var unit in Gameworld.UnitManager.Units.Where(x => x.System == System && x.Type == Type))
		{
			if (!unit.DefaultUnitForSystem)
			{
				continue;
			}
			unit.DefaultUnitForSystem = false;
			unit.Changed = true;
		}
		DefaultUnitForSystem = true;
		Changed = true;
		actor.OutputHandler.Send($"This unit is now the default unit for {Type.DescribeEnum().ColourValue()} values in the {System.ColourName()} system.");
		return true;
	}

	private bool BuildingCommandSpaceBetween(ICharacter actor)
	{
		SpaceBetween = !SpaceBetween;
		Changed = true;
		actor.OutputHandler.Send($"This unit will {SpaceBetween.NowNoLonger()} have a space between the number and the unit abbreviation in display.");
		return true;
	}

	private bool BuildingCommandDescriber(ICharacter actor)
	{
		DescriberUnit = !DescriberUnit;
		Changed = true;
		actor.OutputHandler.Send($"This unit is {DescriberUnit.NowNoLonger()} a describer unit for the engine to describe with.");
		return true;
	}

	private bool BuildingCommandType(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What type of unit should this be? The options are {Enum.GetValues<UnitType>().ListToColouredString()}.");
			return false;
		}

		if (!ss.SafeRemainingArgument.TryParseEnum<UnitType>(out var unitType))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid unit type. The options are {Enum.GetValues<UnitType>().ListToColouredString()}.");
			return false;
		}

		Type = unitType;
		Changed = true;
		Gameworld.UnitManager.RecalculateLastUnits();
		actor.OutputHandler.Send($"This unit now is for the {unitType.DescribeEnum().ColourValue()} type.");
		return true;
	}

	private bool BuildingCommandPostOffset(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What offset should be added after the multiplier is applied to the value of this unit?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, actor, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		PostMultiplierOffsetFrombase = value;
		Changed = true;
		actor.OutputHandler.Send($"The unit now adds an offset of {value.ToStringN2Colour(actor)} after applying the multiplier.");
		return true;
	}

	private bool BuildingCommandPreOffset(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What offset should be added before the multiplier is applied to the value of this unit?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, actor, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		PreMultiplierOffsetFrombase = value;
		Changed = true;
		actor.OutputHandler.Send($"The unit now adds an offset of {value.ToStringN2Colour(actor)} before applying the multiplier.");
		return true;
	}

	private bool BuildingCommandMultiplier(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should the multiplier be for this unit relative to the base unit in the engine?");
			return false;
		}

		if (!double.TryParse(ss.SafeRemainingArgument, actor, out var value))
		{
			actor.OutputHandler.Send($"The text {ss.SafeRemainingArgument.ColourCommand()} is not a valid number.");
			return false;
		}

		MultiplierFromBase = value;
		Changed = true;
		actor.OutputHandler.Send($"The unit now has a multiplier of {value.ToStringN2Colour(actor)} relative to the engine base.");
		Gameworld.UnitManager.RecalculateLastUnits();
		return true;
	}

	private bool BuildingCommandPrimaryAbbreviation(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What should be the primary abbreviation for this unit?");
			return false;
		}

		var text = ss.SafeRemainingArgument;
		if (!_abbreviations.Any(x => x.EqualTo(text)))
		{
			_abbreviations.Add(text);
		}

		PrimaryAbbreviation = text;
		Changed = true;
		actor.OutputHandler.Send($"The primary abbreviation (the one used by the engine to describe) for this unit is now {text.ColourCommand()}.");
		return true;
	}

	private bool BuildingCommandAbbreviation(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("Which abbreviation would you like to toggle for this unit?");
			return false;
		}

		var text = ss.SafeRemainingArgument;
		if (PrimaryAbbreviation.EqualTo(text))
		{
			actor.OutputHandler.Send("You can't remove the primary abbreviation. Set it to something else first.");
			return false;
		}

		Changed = true;
		if (_abbreviations.RemoveAll(x => x.EqualTo(text)) > 0)
		{
			actor.OutputHandler.Send($"You remove the abbreviation {text.ColourCommand()} from this unit.");
			return true;
		}

		_abbreviations.Add(text);
		actor.OutputHandler.Send($"You add the abbreviation {text.ColourCommand()} to this unit.");
		return true;
	}

	private bool BuildingCommandSystem(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send($"What system do you want to change this unit to be for? The current systems are {Gameworld.UnitManager.Systems.ListToColouredString()}.");
			return false;
		}

		var system = ss.SafeRemainingArgument.TitleCase();
		var warn = Gameworld.UnitManager.Units.All(x => !x.System.EqualTo(system));
		System = system;
		Changed = true;
		actor.OutputHandler.Send($"This unit now belongs to the {system.ColourName()} system.{(warn ? "\nWarning: There are no other units for this system. Check you didn't make a typo or mistake.".ColourError() : "")}");
		Gameworld.UnitManager.RecalculateAllUnits();
		return true;
	}

	private bool BuildingCommandName(ICharacter actor, StringStack ss)
	{
		if (ss.IsFinished)
		{
			actor.OutputHandler.Send("What name do you want to give to this unit?");
			return false;
		}

		var name = ss.SafeRemainingArgument.ToLowerInvariant();
		if (Gameworld.UnitManager.Units.Any(x => x.System.EqualTo(System) && x.Type == Type && x.Name.EqualTo(name)))
		{
			actor.OutputHandler.Send($"There is already a {Type.DescribeEnum().ColourValue()} unit for the {System.ColourName()} system with the name {name.ColourCommand()}. Names must be unique.");
			return false;
		}

		actor.OutputHandler.Send($"You rename the unit from {_name.ColourName()} to {name.ColourName()}.");
		_name = name;
		Changed = true;
		return true;
	}
}
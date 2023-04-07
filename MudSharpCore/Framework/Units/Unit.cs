using System.Collections.Generic;
using System.Linq;
using MudSharp.Models;

namespace MudSharp.Framework.Units;

public class Unit : IUnit
{
	/// <summary>
	///     Private backing field for Abbreviations Property
	/// </summary>
	private readonly List<string> _abbreviations;

	public Unit(UnitOfMeasure uom)
	{
		Name = uom.Name;
		_abbreviations = uom.Abbreviations.Split(' ').ToList();
		MultiplierFromBase = uom.BaseMultiplier;
		Type = (UnitType)uom.Type;
		DescriberUnit = uom.Describer;
		SpaceBetween = uom.SpaceBetween;
		System = uom.System;
		PreMultiplierOffsetFrombase = uom.PreMultiplierBaseOffset;
		PostMultiplierOffsetFrombase = uom.PostMultiplierBaseOffset;
		PrimaryAbbreviation =
			!string.IsNullOrWhiteSpace(uom.PrimaryAbbreviation) ? uom.PrimaryAbbreviation : uom.Name;
	}

	/// <summary>
	///     The name of this unit
	/// </summary>
	public string Name { get; protected set; }

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

	public bool DefaultUnitForSystem { get; protected set; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MudSharp.Accounts;
using MudSharp.Database;
using System.Diagnostics;
using MudSharp.Models;
using CultureInfo = System.Globalization.CultureInfo;

namespace MudSharp.Framework.Units;

public class UnitManager : IUnitManager
{
	private static readonly Regex PatternRegex = new(@"(\d{1,}[\.\d]{0,})[ ]{0,1}([a-zA-Z'""]{1,})");
	private readonly List<IUnit> _units = new();

	private readonly Dictionary<string, IEnumerable<IUnit>> _unitSystems =
		new();

	public UnitManager(IFuturemudLoader game)
	{
		InitialiseUnitManager(game);
	}

	public IEnumerable<IUnit> Units => _units;

	public double BaseWeightToKilograms { get; private set; }

	public double BaseHeightToMetres { get; private set; }

	public double BaseFluidToLitres { get; private set; }

	public double BaseAreaToSquareMetres { get; private set; }

	public double BaseVolumeToCubicMetres { get; private set; }

	public double BaseTemperatureToCelcius { get; private set; }

	public double BaseForceToNewtons { get; private set; }

	public double BaseStressToPascals { get; private set; }

	public double BaseBMIToKGPerM2 { get; private set; }

	public double GetBaseUnits(string pattern, UnitType type, out bool success)
	{
		double baseSum = 0;
		var matches = PatternRegex.Matches(pattern).ToList();
		if (!matches.Any())
		{
			success = false;
			return 0.0;
		}

		foreach (var match in matches)
		{
			var unit =
				Units.Where(x => x.Type == type)
				     .FirstOrDefault(x => x.Abbreviations.Contains(match.Groups[2].Value.ToLowerInvariant()));
			if (unit == null)
			{
				success = false;
				return 0.0;
			}

			baseSum += double.Parse(match.Groups[1].Value) * unit.MultiplierFromBase;
		}

		success = true;
		return baseSum;
	}

	public bool TryGetBaseUnits(string pattern, UnitType type, out double value)
	{
		value = 0.0;
		var matches = PatternRegex.Matches(pattern).ToList();
		if (!matches.Any())
		{
			return false;
		}

		foreach (var match in matches)
		{
			var unit =
				Units.Where(x => x.Type == type)
				     .FirstOrDefault(x => x.Abbreviations.Contains(match.Groups[2].Value.ToLowerInvariant()));
			if (unit == null)
			{
				value = 0.0;
				return false;
			}

			value += double.Parse(match.Groups[1].Value) * unit.MultiplierFromBase;
		}

		return true;
	}

	private void InitialiseUnitManager(IFuturemudLoader game)
	{
		using (new FMDB())
		{
			foreach (var unit in FMDB.Context.UnitsOfMeasure)
			{
				_units.Add(new Unit(unit));
			}

			foreach (var system in _units.Select(x => x.System).Distinct())
			{
				_unitSystems.Add(system,
					_units.Where(x => x.System == system && x.DescriberUnit)
					      .OrderBy(x => x.System)
					      .ThenByDescending(x => x.MultiplierFromBase)
					      .ToList());
			}

			foreach (var system in _unitSystems.Keys)
			foreach (UnitType value in Enum.GetValues(typeof(UnitType)))
			{
				_unitSystems[system].Last(x => x.Type == value).LastDescriber = true;
			}

			LoadStaticConfigurations(game);
		}
	}

	private void LoadStaticConfigurations(IFuturemudLoader game)
	{
		var heightConfig = game.GetStaticConfiguration("BaseHeightUOMToMetres");
		if (heightConfig != null)
		{
			BaseHeightToMetres = heightConfig.GetDouble() ?? 0.0;
		}

		var weightConfig = game.GetStaticConfiguration("BaseWeightUOMToKilograms");
		if (weightConfig != null)
		{
			BaseWeightToKilograms = weightConfig.GetDouble() ?? 0.0;
		}

		var fluidConfig = game.GetStaticConfiguration("BaseFluidUOMToLitres");
		if (fluidConfig != null)
		{
			BaseFluidToLitres = fluidConfig.GetDouble() ?? 0.0;
		}

		var areaConfig = game.GetStaticConfiguration("BaseAreaUOMToSquareMetres");
		if (areaConfig != null)
		{
			BaseAreaToSquareMetres = areaConfig.GetDouble() ?? 0.0;
		}

		var volumeConfig = game.GetStaticConfiguration("BaseVolumeUOMToCubicMetres");
		if (volumeConfig != null)
		{
			BaseVolumeToCubicMetres = volumeConfig.GetDouble() ?? 0.0;
		}

		var tempConfig = game.GetStaticConfiguration("BaseTemperatureUOMToCelcius");
		if (tempConfig != null)
		{
			BaseTemperatureToCelcius = tempConfig.GetDouble() ?? 0.0;
		}

		var forceConfig = game.GetStaticConfiguration("BaseForceUOMToNewtons");
		if (forceConfig != null)
		{
			BaseForceToNewtons = forceConfig.GetDouble() ?? 0.0;
		}

		var stressConfig = game.GetStaticConfiguration("BaseStressUOMToPascals");
		if (stressConfig != null)
		{
			BaseStressToPascals = stressConfig.GetDouble() ?? 0.0;
		}

		var bmiConfig = game.GetStaticConfiguration("BaseBMIUOMToKGPerM2");
		if (bmiConfig != null)
		{
			BaseBMIToKGPerM2 = bmiConfig.GetDouble() ?? 0.0;
		}
	}

	public string DescribeDecimal(double value, UnitType type, IPerceiver character)
	{
		return DescribeDecimal(value, type, character?.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference,
			character);
	}

	public string DescribeDecimal(double value, UnitType type, IAccount character)
	{
		return DescribeDecimal(value, type, character?.UnitPreference ?? DummyAccount.Instance.UnitPreference,
			character);
	}

	public string DescribeDecimal(double value, UnitType type, string system, IFormatProvider format = null)
	{
		if (format == null)
		{
			format = CultureInfo.InvariantCulture;
		}

		var negative = value < 0;
		value = Math.Abs(value);

		var unitSystem = _unitSystems.ContainsKey(system) ? _unitSystems[system] : _unitSystems.First().Value;
		var units = unitSystem.Where(x => x.Type == type).ToList();

		foreach (var unit in units.OrderByDescending(x => x.MultiplierFromBase))
		{
			if (value / unit.MultiplierFromBase < 1.0 && unit != units.Last())
			{
				continue;
			}

			var uvalue = (negative ? -1.0 : 1.0) *
			             ((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
			              unit.PostMultiplierOffsetFrombase);
			return $"{uvalue.ToString("N", format)}{unit.PrimaryAbbreviation}";
		}

		throw new ApplicationException("Reached the end of UnitManager.DescribeDecimal");
	}

	public string DescribeBonus(double value, UnitType type, IPerceiver character, uint decimalPlaces = 2)
	{
		return DescribeBonus(value, type, character?.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference,
			character, decimalPlaces);
	}

	public string DescribeBonus(double value, UnitType type, IAccount character, uint decimalPlaces = 2)
	{
		return DescribeBonus(value, type, character?.UnitPreference ?? DummyAccount.Instance.UnitPreference, character,
			decimalPlaces);
	}

	public string DescribeBonus(double value, UnitType type, string system, IFormatProvider format = null,
		uint decimalPlaces = 2)
	{
		if (format == null)
		{
			format = CultureInfo.InvariantCulture;
		}

		var negative = value < 0;
		value = Math.Abs(value);

		var unitSystem = _unitSystems.ContainsKey(system) ? _unitSystems[system] : _unitSystems.First().Value;
		var units = unitSystem.Where(x => x.Type == type).ToList();

		foreach (var unit in units.OrderByDescending(x => x.MultiplierFromBase))
		{
			if (value / unit.MultiplierFromBase < 1.0 && unit != units.Last())
			{
				continue;
			}

			var uvalue = (negative ? -1.0 : 1.0) *
			             ((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
			              unit.PostMultiplierOffsetFrombase);
			var decimalBit = decimalPlaces > 0 ? $".{new string('0', (int)decimalPlaces)}" : "";
			var numberFormat = $"{{0:+#,0{decimalBit};-#,0{decimalBit};+0{decimalBit}}}{{1}}";
			return string.Format(format, numberFormat, uvalue, unit.PrimaryAbbreviation);
		}

		throw new ApplicationException("Reached the end of UnitManager.DescribeBonus");
	}

	public string DescribeExact(double value, UnitType type, IPerceiver character)
	{
		return Describe(value, type, character?.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference,
			character);
	}

	public string DescribeExact(double value, UnitType type, IAccount character)
	{
		return Describe(value, type, character?.UnitPreference ?? DummyAccount.Instance.UnitPreference, character);
	}

	public string DescribeExact(double value, UnitType type, string system, IFormatProvider format = null)
	{
		if (format == null)
		{
			format = CultureInfo.InvariantCulture;
		}

		var negative = value < 0;
		value = Math.Abs(value);

		var outList = new List<string>();
		var unitSystem = _unitSystems.ContainsKey(system) ? _unitSystems[system] : _unitSystems.First().Value;
		var units = unitSystem.Where(x => x.Type == type).ToList();
		foreach (var unit in units)
		{
			if (value / unit.MultiplierFromBase >= 1.0 || (!outList.Any() && unit == units.Last()))
			{
				int ivalue;
				if (unit.LastDescriber)
				{
					ivalue =
						(int)
						Math.Round((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
						           unit.PostMultiplierOffsetFrombase);
				}
				else
				{
					ivalue =
						(int)
						((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
						 unit.PostMultiplierOffsetFrombase);
				}

				if (ivalue > 0 || unit.LastDescriber)
				{
					outList.Add(string.Format(format, "{0:#,0.##}{1}{2}",
						ivalue,
						unit.SpaceBetween ? " " : "",
						ivalue == 1 ? unit.Name : unit.Name.Pluralise()
					));
				}

				value = value -
				        ((ivalue + unit.PreMultiplierOffsetFrombase) * unit.MultiplierFromBase +
				         unit.PostMultiplierOffsetFrombase);
			}
		}

		return (negative ? "negative " : "") + outList.ListToString(separator: " ", conjunction: "");
	}

	public string Describe(double value, UnitType type, IPerceiver character)
	{
		return Describe(value, type, character?.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference,
			character);
	}

	public string Describe(double value, UnitType type, IAccount character)
	{
		return Describe(value, type, character?.UnitPreference ?? DummyAccount.Instance.UnitPreference, character);
	}

	public string Describe(double value, UnitType type, string system, IFormatProvider format = null)
	{
		if (format == null)
		{
			format = CultureInfo.InvariantCulture;
		}

		var negative = value < 0;
		value = Math.Abs(value);

		var outList = new List<string>();
		var unitSystem = _unitSystems.ContainsKey(system) ? _unitSystems[system] : _unitSystems.First().Value;
		var units = unitSystem.Where(x => x.Type == type).OrderByDescending(x => x.MultiplierFromBase).ToList();
		foreach (var unit in units)
		{
			if (value / unit.MultiplierFromBase >= 1.0 || (!outList.Any() && unit == units.Last()))
			{
				int ivalue;
				if (unit.LastDescriber)
				{
					ivalue =
						(int)
						Math.Round((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
						           unit.PostMultiplierOffsetFrombase);
				}
				else
				{
					ivalue =
						(int)
						((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
						 unit.PostMultiplierOffsetFrombase);
				}

				if (ivalue > 0 || unit.LastDescriber)
				{
					outList.Add(string.Format(format, "{0:#,0.##}{1}{2}",
						ivalue,
						unit.SpaceBetween ? " " : "",
						ivalue == 1 ? unit.Name : unit.Name.Pluralise()
					));
				}

				value = value -
				        ((ivalue + unit.PreMultiplierOffsetFrombase) * unit.MultiplierFromBase +
				         unit.PostMultiplierOffsetFrombase);
			}
			else
			{
				if (outList.Any())
				{
					break;
				}
			}
		}

		return (negative ? "negative " : "") + outList.ListToString(separator: " ", conjunction: "");
	}

	public string DescribeBrief(double value, UnitType type, IPerceiver character)
	{
		return DescribeBrief(value, type, character?.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference,
			character);
	}

	public string DescribeBrief(double value, UnitType type, IAccount character)
	{
		return DescribeBrief(value, type, character?.UnitPreference ?? DummyAccount.Instance.UnitPreference, character);
	}

	public string DescribeBrief(double value, UnitType type, string system, IFormatProvider format = null)
	{
		if (format == null)
		{
			format = CultureInfo.InvariantCulture;
		}

		var negative = value < 0;
		value = Math.Abs(value);

		var outList = new List<string>();
		var unitSystem = _unitSystems.ContainsKey(system) ? _unitSystems[system] : _unitSystems.First().Value;
		var units = unitSystem.Where(x => x.Type == type).OrderByDescending(x => x.MultiplierFromBase).ToList();
		foreach (var unit in units)
		{
			if (value / unit.MultiplierFromBase >= 1.0 || (!outList.Any() && unit == units.Last()))
			{
				int ivalue;
				if (unit.LastDescriber)
				{
					ivalue =
						(int)
						Math.Round((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
						           unit.PostMultiplierOffsetFrombase);
				}
				else
				{
					ivalue =
						(int)
						((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
						 unit.PostMultiplierOffsetFrombase);
				}

				if (ivalue > 0 || unit.LastDescriber)
				{
					outList.Add(string.Format(format, "{0:#,0.##}{1}",
						ivalue,
						unit.PrimaryAbbreviation
					));
				}

				value = value -
				        ((ivalue + unit.PreMultiplierOffsetFrombase) * unit.MultiplierFromBase +
				         unit.PostMultiplierOffsetFrombase);
			}
			else
			{
				if (outList.Any())
				{
					break;
				}
			}
		}

		return (negative ? "negative " : "") + outList.ListToString(separator: " ", conjunction: "");
	}

	public string DescribeMostSignificant(double value, UnitType type, IPerceiver character)
	{
		return DescribeMostSignificant(value, type,
			character?.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference, character);
	}

	public string DescribeMostSignificant(double value, UnitType type, string system, IFormatProvider format = null)
	{
		if (format == null)
		{
			format = CultureInfo.InvariantCulture;
		}

		var negative = value < 0;
		value = Math.Abs(value);

		var unitSystem = _unitSystems.ContainsKey(system) ? _unitSystems[system] : _unitSystems.First().Value;
		var units = unitSystem.Where(x => x.Type == type).ToList();
		foreach (var unit in units)
		{
			if (value / unit.MultiplierFromBase >= 1.0 || unit == units.Last())
			{
				var ivalue =
					(int)
					Math.Round((value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
					           unit.PostMultiplierOffsetFrombase);
				return string.Format(format, "{0:#,0.##}{1}{2}",
					ivalue,
					unit.SpaceBetween ? " " : "",
					ivalue == 1 ? unit.Name : unit.Name.Pluralise());
			}
		}

		return (negative ? "negative " : "") + value.ToString(format);
	}

	public string DescribeMostSignificantExact(double value, UnitType type, IAccount character)
	{
		return DescribeMostSignificantExact(value, type,
			character?.UnitPreference ?? DummyAccount.Instance.UnitPreference, character);
	}

	public string DescribeMostSignificantExact(double value, UnitType type, IPerceiver character)
	{
		return DescribeMostSignificantExact(value, type,
			character?.Account?.UnitPreference ?? DummyAccount.Instance.UnitPreference, character);
	}

	public string DescribeMostSignificantExact(double value, UnitType type, string system,
		IFormatProvider format = null)
	{
		if (format == null)
		{
			format = CultureInfo.InvariantCulture;
		}

		var negative = value < 0;
		value = Math.Abs(value);

		var unitSystem = _unitSystems.ContainsKey(system) ? _unitSystems[system] : _unitSystems.First().Value;
		var units = unitSystem.Where(x => x.Type == type).ToList();
		foreach (var unit in units)
		{
			if (value / unit.MultiplierFromBase >= 1.0 || unit == units.Last())
			{
				var dvalue =
					(value + unit.PreMultiplierOffsetFrombase) / unit.MultiplierFromBase +
					unit.PostMultiplierOffsetFrombase;
				return string.Format(format, "{0:#,0.##}{1}{2}",
					dvalue,
					unit.SpaceBetween ? " " : "",
					dvalue == 1 ? unit.Name : unit.Name.Pluralise());
			}
		}

		return (negative ? "negative " : "") + value.ToString(format);
	}
}
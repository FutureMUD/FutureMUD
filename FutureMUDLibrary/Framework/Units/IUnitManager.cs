using System;
using System.Collections.Generic;
using MudSharp.Accounts;

namespace MudSharp.Framework.Units
{
	public interface IUnitManager
	{
		IEnumerable<IUnit> Units { get; }
		IEnumerable<string> Systems { get; }
		double BaseWeightToKilograms { get; }
		double BaseHeightToMetres { get; }
		double BaseFluidToLitres { get; }
		double BaseAreaToSquareMetres { get; }
		double BaseVolumeToCubicMetres { get; }
		double BaseTemperatureToCelcius { get; }
		double BaseForceToNewtons { get; }
		double BaseStressToPascals { get; }
		void RecalculateLastUnits();
		void RecalculateAllUnits();
		void AddUnit(IUnit unit);
		void RemoveUnit(IUnit unit);

		double GetBaseUnits(string pattern, UnitType type, out bool success);
		bool TryGetBaseUnits(string pattern, UnitType type, IPerceiver who, out double value);
		bool TryGetBaseUnits(string pattern, UnitType type, IAccount who, out double value);

		string DescribeDecimal(double value, UnitType type, IPerceiver character);
		string DescribeDecimal(double value, UnitType type, IAccount character);
		string DescribeDecimal(double value, UnitType type, string system, IFormatProvider format = null);
		string DescribeBonus(double value, UnitType type, IPerceiver character, uint decimalPlaces = 2);
		string DescribeBonus(double value, UnitType type, IAccount character, uint decimalPlaces = 2);
		string DescribeBonus(double value, UnitType type, string system, IFormatProvider format = null, uint decimalPlaces = 2);
		string DescribeExact(double value, UnitType type, IPerceiver character);
		string DescribeExact(double value, UnitType type, IAccount character);
		string DescribeExact(double value, UnitType type, string system, IFormatProvider format = null);
		string Describe(double value, UnitType type, IPerceiver character);
		string Describe(double value, UnitType type, IAccount character);
		string Describe(double value, UnitType type, string system, IFormatProvider format = null);
		string DescribeBrief(double value, UnitType type, IPerceiver character);
		string DescribeBrief(double value, UnitType type, IAccount character);
		string DescribeBrief(double value, UnitType type, string system, IFormatProvider format = null);
		string DescribeMostSignificant(double value, UnitType type, IPerceiver character);
		string DescribeMostSignificant(double value, UnitType type, string system, IFormatProvider format = null);
		string DescribeMostSignificantExact(double value, UnitType type, IAccount character);
		string DescribeMostSignificantExact(double value, UnitType type, IPerceiver character);

		string DescribeMostSignificantExact(double value, UnitType type, string system,
			IFormatProvider format = null);

		string DescribeSpecificUnit(double value, IUnit unit, IFormatProvider format = null);
	}
}
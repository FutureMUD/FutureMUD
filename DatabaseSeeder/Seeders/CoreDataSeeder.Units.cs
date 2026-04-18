#nullable enable

using MudSharp.Database;
using MudSharp.Framework.Units;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
	private static void SeedUnitsOfMeasure(FuturemudDatabaseContext context)
	{
		const string metricSystem = "Metric";
		const string imperialUsSystem = "Imperial";
		const string imperialUkSystem = "Imperial-UK";

		void AddUnit(long id, string name, string primaryAbbreviation, string abbreviations, double baseMultiplier,
			double preMultiplierBaseOffset, double postMultiplierBaseOffset, UnitType type, bool describer,
			bool spaceBetween, string system, bool defaultUnitForSystem)
		{
			context.UnitsOfMeasure.Add(new UnitOfMeasure
			{
				Id = id,
				Name = name,
				PrimaryAbbreviation = primaryAbbreviation,
				Abbreviations = abbreviations,
				BaseMultiplier = baseMultiplier,
				PreMultiplierBaseOffset = preMultiplierBaseOffset,
				PostMultiplierBaseOffset = postMultiplierBaseOffset,
				Type = (int)type,
				Describer = describer,
				SpaceBetween = spaceBetween,
				System = system,
				DefaultUnitForSystem = defaultUnitForSystem
			});
		}

		AddUnit(1, "gram", "g", "gram grams g", 1.0, 0.0, 0.0, UnitType.Mass, true, true, metricSystem, true);
		AddUnit(2, "kilogram", "kg", "kilogram kilograms kilo kilos kg", 1000.0, 0.0, 0.0, UnitType.Mass, true,
			true, metricSystem, false);
		AddUnit(3, "tonne", "t", "tonne tonnes metricton metrictons t", 1000000.0, 0.0, 0.0, UnitType.Mass, true,
			true, metricSystem, false);
		AddUnit(4, "pound", "lb", "pound pounds lb lbs", 453.59237, 0.0, 0.0, UnitType.Mass, true, true,
			imperialUsSystem, true);
		AddUnit(5, "ounce", "oz", "ounce ounces oz", 28.349523125, 0.0, 0.0, UnitType.Mass, true, true,
			imperialUsSystem, false);
		AddUnit(6, "centimetre", "cm", "centimetre centimetres centimeter centimeters cm cms", 1.0, 0.0, 0.0,
			UnitType.Length, true, true, metricSystem, false);
		AddUnit(7, "metre", "m", "metre metres meter meters m", 100.0, 0.0, 0.0, UnitType.Length, true, true,
			metricSystem, true);
		AddUnit(8, "millimetre", "mm", "millimetre millimetres millimeter millimeters mm", 0.1, 0.0, 0.0,
			UnitType.Length, false, true, metricSystem, false);
		AddUnit(9, "foot", "ft", "foot feet ft '", 30.48, 0.0, 0.0, UnitType.Length, true, true, imperialUsSystem,
			true);
		AddUnit(10, "inch", "in", "inch inches in \"", 2.54, 0.0, 0.0, UnitType.Length, true, true,
			imperialUsSystem, false);
		AddUnit(11, "short ton", "tn", "shortton shorttons ton tons tn", 907184.74, 0.0, 0.0, UnitType.Mass, true,
			true, imperialUsSystem, false);
		AddUnit(12, "kilotonne", "kt", "kilotonne kilotonnes kilotons kt", 1000000000.0, 0.0, 0.0, UnitType.Mass,
			true, true, metricSystem, false);
		AddUnit(13, "stone", "st", "stone stones st", 6350.29318, 0.0, 0.0, UnitType.Mass, false, true,
			imperialUsSystem, false);
		AddUnit(14, "chain", "ch", "chain chains ch", 2011.68, 0.0, 0.0, UnitType.Length, false, true,
			imperialUsSystem, false);
		AddUnit(15, "mile", "mi", "mile miles mi", 160934.4, 0.0, 0.0, UnitType.Length, false, true,
			imperialUsSystem, false);
		AddUnit(16, "kilometre", "km", "kilometre kilometres kilometer kilometers km kms", 100000.0, 0.0, 0.0,
			UnitType.Length, false, true, metricSystem, false);
		AddUnit(17, "furlong", "fur", "furlong furlongs fur", 20116.8, 0.0, 0.0, UnitType.Length, false, true,
			imperialUsSystem, false);
		AddUnit(18, "league", "lea", "league leagues lea", 482803.2, 0.0, 0.0, UnitType.Length, false, true,
			imperialUsSystem, false);
		AddUnit(19, "link", "lnk", "link links lnk", 20.1168, 0.0, 0.0, UnitType.Length, false, true,
			imperialUsSystem, false);
		AddUnit(20, "rod", "rd", "rod rods rd perch perches pole poles", 502.92, 0.0, 0.0, UnitType.Length, false,
			true, imperialUsSystem, false);
		AddUnit(21, "litre", "l", "litre litres liter liters l", 1.0, 0.0, 0.0, UnitType.FluidVolume, true, true,
			metricSystem, true);
		AddUnit(22, "millilitre", "ml", "millilitre millilitres milliliter milliliters ml mls", 0.001, 0.0, 0.0,
			UnitType.FluidVolume, true, true, metricSystem, false);
		AddUnit(23, "fluid ounce", "fl oz", "fluidounce fluidounces floz fl oz ounce ounces", 0.0295735295625, 0.0,
			0.0, UnitType.FluidVolume, true, true, imperialUsSystem, true);
		AddUnit(24, "fluid dram", "fl dr", "fluiddram fluiddrams fldr fl dr dram drams", 0.0036966911953125, 0.0,
			0.0, UnitType.FluidVolume, true, true, imperialUsSystem, false);
		AddUnit(25, "cup", "cp", "cup cups cp cps", 0.2365882365, 0.0, 0.0, UnitType.FluidVolume, false, true,
			imperialUsSystem, false);
		AddUnit(26, "pint", "pt", "pint pints p pt pts", 0.473176473, 0.0, 0.0, UnitType.FluidVolume, false, true,
			imperialUsSystem, false);
		AddUnit(27, "quart", "qt", "quart quarts qt qts", 0.946352946, 0.0, 0.0, UnitType.FluidVolume, false, true,
			imperialUsSystem, false);
		AddUnit(28, "gallon", "gal", "gallon gallons gal gals", 3.785411784, 0.0, 0.0, UnitType.FluidVolume, true,
			true, imperialUsSystem, false);
		AddUnit(29, "kilolitre", "kl", "kilolitre kilolitres kiloliter kiloliters kl", 1000.0, 0.0, 0.0,
			UnitType.FluidVolume, true, true, metricSystem, false);
		AddUnit(30, "megalitre", "ML", "megalitre megalitres megaliter megaliters ML", 1000000.0, 0.0, 0.0,
			UnitType.FluidVolume, true, true, metricSystem, false);
		AddUnit(31, "gigalitre", "GL", "gigalitre gigalitres gigaliter gigaliters GL", 1000000000.0, 0.0, 0.0,
			UnitType.FluidVolume, true, true, metricSystem, false);
		AddUnit(32, "square metre", "m2", "squaremetre squaremetres squaremeter squaremeters sqm sqms m2", 1.0, 0.0,
			0.0, UnitType.Area, true, true, metricSystem, true);
		AddUnit(33, "square foot", "ft2", "squarefoot squarefeet sqft sqf f2 ft2", 0.09290304, 0.0, 0.0,
			UnitType.Area, true, true, imperialUsSystem, true);
		AddUnit(34, "acre", "ac", "acre acres ac", 4046.8564224, 0.0, 0.0, UnitType.Area, true, true,
			imperialUsSystem, false);
		AddUnit(35, "hectare", "ha", "hectare hectares ha", 10000.0, 0.0, 0.0, UnitType.Area, true, true,
			metricSystem, false);
		AddUnit(36, "cubic metre", "m3", "cubicmetre cubicmetres cubicmeter cubicmeters m3 cbm cbms", 1.0, 0.0, 0.0,
			UnitType.Volume, true, true, metricSystem, true);
		AddUnit(37, "cubic foot", "cu ft", "cubicfoot cubicfeet cuft cft ft3 f3 cu ft", 0.028316846592, 0.0, 0.0,
			UnitType.Volume, true, true, imperialUsSystem, true);
		AddUnit(38, "celsius", "C", "celsius centigrade C c", 1.0, 0.0, 0.0, UnitType.Temperature, true, false,
			metricSystem, true);
		AddUnit(39, "fahrenheit", "F", "fahrenheit F f", 0.5555555555555556, 0.0, 32.0, UnitType.Temperature, true,
			false, imperialUsSystem, true);
		AddUnit(40, "perch", "per", "perch perches per", 25.29285264, 0.0, 0.0, UnitType.Area, false, true,
			imperialUkSystem, false);
		AddUnit(41, "rood", "rood", "rood roods", 1011.7141056, 0.0, 0.0, UnitType.Area, false, true,
			imperialUkSystem, false);
		AddUnit(42, "acre", "ac", "acre acres ac", 4046.8564224, 0.0, 0.0, UnitType.Area, true, true,
			imperialUkSystem, false);
		AddUnit(43, "drachm", "dr", "drachm drachms dr", 1.7718451953125, 0.0, 0.0, UnitType.Mass, false, true,
			imperialUsSystem, false);
		AddUnit(44, "grain", "gr", "grain grains gr", 0.06479891, 0.0, 0.0, UnitType.Mass, false, true,
			imperialUsSystem, false);
		AddUnit(45, "thou", "th", "thou thous th mil mils", 0.00254, 0.0, 0.0, UnitType.Length, false, true,
			imperialUsSystem, false);
		AddUnit(46, "milligram", "mg", "milligram milligrams mg", 0.001, 0.0, 0.0, UnitType.Mass, true, true,
			metricSystem, false);
		AddUnit(47, "celsius", "C", "celsius centigrade C c", 1.0, 0.0, 0.0, UnitType.TemperatureDelta, true, false,
			metricSystem, true);
		AddUnit(48, "fahrenheit", "F", "fahrenheit F f", 0.5555555555555556, 0.0, 0.0, UnitType.TemperatureDelta,
			true, false, imperialUsSystem, true);
		AddUnit(49, "newton", "N", "newton newtons N", 1.0, 0.0, 0.0, UnitType.Force, true, false, metricSystem,
			true);
		AddUnit(50, "kilonewton", "kN", "kilonewton kilonewtons kN", 1000.0, 0.0, 0.0, UnitType.Force, true, false,
			metricSystem, false);
		AddUnit(51, "meganewton", "MN", "meganewton meganewtons MN", 1000000.0, 0.0, 0.0, UnitType.Force, true,
			false, metricSystem, false);
		AddUnit(52, "pound-force", "lbf", "poundforce pound-force poundforces pound-forces lbf", 4.4482216152605, 0.0,
			0.0, UnitType.Force, true, false, imperialUsSystem, true);
		AddUnit(53, "pascal", "Pa", "pascal pascals Pa pa", 1.0, 0.0, 0.0, UnitType.Stress, true, false, metricSystem,
			true);
		AddUnit(54, "kilopascal", "kPa", "kilopascal kilopascals kPa kpa", 1000.0, 0.0, 0.0, UnitType.Stress, true,
			false, metricSystem, false);
		AddUnit(55, "megapascal", "MPa", "megapascal megapascals MPa mpa", 1000000.0, 0.0, 0.0, UnitType.Stress, true,
			false, metricSystem, false);
		AddUnit(56, "gigapascal", "GPa", "gigapascal gigapascals GPa gpa", 1000000000.0, 0.0, 0.0, UnitType.Stress,
			true, false, metricSystem, false);
		AddUnit(57, "psi", "psi", "psi poundspersquareinch poundpersquareinch", 6894.757293168, 0.0, 0.0,
			UnitType.Stress, true, false, imperialUsSystem, true);
		AddUnit(58, "ksi", "ksi", "ksi kpsi kilopsi", 6894757.293168, 0.0, 0.0, UnitType.Stress, true, false,
			imperialUsSystem, false);
		AddUnit(59, "mpsi", "mpsi", "mpsi megapsi", 6894757293.168, 0.0, 0.0, UnitType.Stress, true, false,
			imperialUsSystem, false);
		AddUnit(60, "gpsi", "gpsi", "gpsi gigapsi", 6894757293168.0, 0.0, 0.0, UnitType.Stress, true, false,
			imperialUsSystem, false);
		AddUnit(61, "kg/m2", "kg/m2", "kg/m2 kgm2 kgsqm bmi", 1.0, 0.0, 0.0, UnitType.BMI, true, false,
			metricSystem, true);
		AddUnit(62, "lb/in2", "lb/in2", "lb/in2 lbin2 lbsqin bmiimperial", 703.06957964, 0.0, 0.0, UnitType.BMI,
			true, false, imperialUsSystem, true);

		AddUnit(63, "microgram", "ug", "microgram micrograms ug", 0.000001, 0.0, 0.0, UnitType.Mass, false, true,
			metricSystem, false);
		AddUnit(64, "metric megatonne", "Mt", "megatonne megatonnes megaton metricmegaton metricmegatons Mt",
			1000000000000.0, 0.0, 0.0, UnitType.Mass, true, true, metricSystem, false);
		AddUnit(65, "hundredweight", "cwt", "hundredweight hundredweights cwt ushundredweight", 45359.237, 0.0, 0.0,
			UnitType.Mass, false, true, imperialUsSystem, false);
		AddUnit(66, "yard", "yd", "yard yards yd", 91.44, 0.0, 0.0, UnitType.Length, false, true, imperialUsSystem,
			false);
		AddUnit(67, "fathom", "ftm", "fathom fathoms ftm", 182.88, 0.0, 0.0, UnitType.Length, false, true,
			imperialUsSystem, false);
		AddUnit(68, "nautical mile", "nmi", "nauticalmile nauticalmiles nmi nm", 185200.0, 0.0, 0.0, UnitType.Length,
			false, true, imperialUsSystem, false);
		AddUnit(69, "gill", "gi", "gill gills gi", 0.11829411825, 0.0, 0.0, UnitType.FluidVolume, false, true,
			imperialUsSystem, false);
		AddUnit(70, "tablespoon", "tbsp", "tablespoon tablespoons tbsp tbs", 0.01478676478125, 0.0, 0.0,
			UnitType.FluidVolume, false, true, imperialUsSystem, false);
		AddUnit(71, "teaspoon", "tsp", "teaspoon teaspoons tsp", 0.00492892159375, 0.0, 0.0, UnitType.FluidVolume,
			false, true, imperialUsSystem, false);
		AddUnit(72, "square centimetre", "cm2",
			"squarecentimetre squarecentimetres squarecentimeter squarecentimeters cm2 sqcm sqcms", 0.0001, 0.0, 0.0,
			UnitType.Area, false, true, metricSystem, false);
		AddUnit(73, "square kilometre", "km2",
			"squarekilometre squarekilometres squarekilometer squarekilometers km2 sqkm sqkms", 1000000.0, 0.0, 0.0,
			UnitType.Area, false, true, metricSystem, false);
		AddUnit(74, "square inch", "in2", "squareinch squareinches in2 sqin sqins", 0.00064516, 0.0, 0.0,
			UnitType.Area, false, true, imperialUsSystem, false);
		AddUnit(75, "square yard", "yd2", "squareyard squareyards yd2 sqyd sqyds", 0.83612736, 0.0, 0.0,
			UnitType.Area, false, true, imperialUsSystem, false);
		AddUnit(76, "square mile", "mi2", "squaremile squaremiles mi2 sqmi sqmis", 2589988.110336, 0.0, 0.0,
			UnitType.Area, false, true, imperialUsSystem, false);
		AddUnit(77, "cubic centimetre", "cm3",
			"cubiccentimetre cubiccentimetres cubiccentimeter cubiccentimeters cm3 cc", 0.000001, 0.0, 0.0,
			UnitType.Volume, false, true, metricSystem, false);
		AddUnit(78, "cubic inch", "in3", "cubicinch cubicinches in3 cuin", 0.000016387064, 0.0, 0.0, UnitType.Volume,
			false, true, imperialUsSystem, false);
		AddUnit(79, "cubic yard", "yd3", "cubicyard cubicyards yd3 cuyd", 0.764554857984, 0.0, 0.0, UnitType.Volume,
			false, true, imperialUsSystem, false);
		AddUnit(80, "kip", "kip", "kip kips", 4448.2216152605, 0.0, 0.0, UnitType.Force, false, false,
			imperialUsSystem, false);

		AddUnit(81, "pound", "lb", "pound pounds lb lbs", 453.59237, 0.0, 0.0, UnitType.Mass, true, true,
			imperialUkSystem, true);
		AddUnit(82, "ounce", "oz", "ounce ounces oz", 28.349523125, 0.0, 0.0, UnitType.Mass, false, true,
			imperialUkSystem, false);
		AddUnit(83, "stone", "st", "stone stones st", 6350.29318, 0.0, 0.0, UnitType.Mass, true, true,
			imperialUkSystem, false);
		AddUnit(84, "long ton", "lt", "longton longtons ton tons lt", 1016046.9088, 0.0, 0.0, UnitType.Mass, true,
			true, imperialUkSystem, false);
		AddUnit(85, "hundredweight", "cwt", "hundredweight hundredweights cwt britishhundredweight", 50802.34544, 0.0,
			0.0, UnitType.Mass, false, true, imperialUkSystem, false);
		AddUnit(86, "drachm", "dr", "drachm drachms dr", 1.7718451953125, 0.0, 0.0, UnitType.Mass, false, true,
			imperialUkSystem, false);
		AddUnit(87, "grain", "gr", "grain grains gr", 0.06479891, 0.0, 0.0, UnitType.Mass, false, true,
			imperialUkSystem, false);
		AddUnit(88, "foot", "ft", "foot feet ft '", 30.48, 0.0, 0.0, UnitType.Length, true, true, imperialUkSystem,
			true);
		AddUnit(89, "inch", "in", "inch inches in \"", 2.54, 0.0, 0.0, UnitType.Length, true, true, imperialUkSystem,
			false);
		AddUnit(90, "yard", "yd", "yard yards yd", 91.44, 0.0, 0.0, UnitType.Length, false, true, imperialUkSystem,
			false);
		AddUnit(91, "chain", "ch", "chain chains ch", 2011.68, 0.0, 0.0, UnitType.Length, false, true,
			imperialUkSystem, false);
		AddUnit(92, "furlong", "fur", "furlong furlongs fur", 20116.8, 0.0, 0.0, UnitType.Length, false, true,
			imperialUkSystem, false);
		AddUnit(93, "mile", "mi", "mile miles mi", 160934.4, 0.0, 0.0, UnitType.Length, false, true, imperialUkSystem,
			false);
		AddUnit(94, "link", "lnk", "link links lnk", 20.1168, 0.0, 0.0, UnitType.Length, false, true,
			imperialUkSystem, false);
		AddUnit(95, "rod", "rd", "rod rods rd perch perches pole poles", 502.92, 0.0, 0.0, UnitType.Length, false,
			true, imperialUkSystem, false);
		AddUnit(96, "fathom", "ftm", "fathom fathoms ftm", 182.88, 0.0, 0.0, UnitType.Length, false, true,
			imperialUkSystem, false);
		AddUnit(97, "nautical mile", "nmi", "nauticalmile nauticalmiles nmi nm", 185200.0, 0.0, 0.0, UnitType.Length,
			false, true, imperialUkSystem, false);
		AddUnit(98, "thou", "th", "thou thous th mil mils", 0.00254, 0.0, 0.0, UnitType.Length, false, true,
			imperialUkSystem, false);
		AddUnit(99, "fluid ounce", "fl oz", "fluidounce fluidounces floz fl oz ounce ounces", 0.0284130625, 0.0, 0.0,
			UnitType.FluidVolume, true, true, imperialUkSystem, false);
		AddUnit(100, "fluid dram", "fl dr", "fluiddram fluiddrams fldr fl dr dram drams", 0.0035516328125, 0.0, 0.0,
			UnitType.FluidVolume, false, true, imperialUkSystem, false);
		AddUnit(101, "gill", "gi", "gill gills gi", 0.1420653125, 0.0, 0.0, UnitType.FluidVolume, false, true,
			imperialUkSystem, false);
		AddUnit(102, "pint", "pt", "pint pints p pt pts", 0.56826125, 0.0, 0.0, UnitType.FluidVolume, true, true,
			imperialUkSystem, true);
		AddUnit(103, "quart", "qt", "quart quarts qt qts", 1.1365225, 0.0, 0.0, UnitType.FluidVolume, false, true,
			imperialUkSystem, false);
		AddUnit(104, "gallon", "gal", "gallon gallons gal gals", 4.54609, 0.0, 0.0, UnitType.FluidVolume, true, true,
			imperialUkSystem, false);
		AddUnit(105, "tablespoon", "tbsp", "tablespoon tablespoons tbsp tbs", 0.01420653125, 0.0, 0.0,
			UnitType.FluidVolume, false, true, imperialUkSystem, false);
		AddUnit(106, "teaspoon", "tsp", "teaspoon teaspoons tsp", 0.00473551041666667, 0.0, 0.0,
			UnitType.FluidVolume, false, true, imperialUkSystem, false);
		AddUnit(107, "square foot", "ft2", "squarefoot squarefeet sqft sqf f2 ft2", 0.09290304, 0.0, 0.0,
			UnitType.Area, true, true, imperialUkSystem, true);
		AddUnit(108, "square inch", "in2", "squareinch squareinches in2 sqin sqins", 0.00064516, 0.0, 0.0,
			UnitType.Area, false, true, imperialUkSystem, false);
		AddUnit(109, "square yard", "yd2", "squareyard squareyards yd2 sqyd sqyds", 0.83612736, 0.0, 0.0,
			UnitType.Area, false, true, imperialUkSystem, false);
		AddUnit(110, "square mile", "mi2", "squaremile squaremiles mi2 sqmi sqmis", 2589988.110336, 0.0, 0.0,
			UnitType.Area, false, true, imperialUkSystem, false);
		AddUnit(111, "cubic foot", "cu ft", "cubicfoot cubicfeet cuft cft ft3 f3 cu ft", 0.028316846592, 0.0, 0.0,
			UnitType.Volume, true, true, imperialUkSystem, true);
		AddUnit(112, "cubic inch", "in3", "cubicinch cubicinches in3 cuin", 0.000016387064, 0.0, 0.0, UnitType.Volume,
			false, true, imperialUkSystem, false);
		AddUnit(113, "cubic yard", "yd3", "cubicyard cubicyards yd3 cuyd", 0.764554857984, 0.0, 0.0, UnitType.Volume,
			false, true, imperialUkSystem, false);
		AddUnit(114, "celsius", "C", "celsius centigrade C c", 1.0, 0.0, 0.0, UnitType.Temperature, true, false,
			imperialUkSystem, true);
		AddUnit(115, "celsius", "C", "celsius centigrade C c", 1.0, 0.0, 0.0, UnitType.TemperatureDelta, true, false,
			imperialUkSystem, true);
		AddUnit(116, "pound-force", "lbf", "poundforce pound-force poundforces pound-forces lbf", 4.4482216152605, 0.0,
			0.0, UnitType.Force, true, false, imperialUkSystem, true);
		AddUnit(117, "psi", "psi", "psi poundspersquareinch poundpersquareinch", 6894.757293168, 0.0, 0.0,
			UnitType.Stress, true, false, imperialUkSystem, true);
		AddUnit(118, "ksi", "ksi", "ksi kpsi kilopsi", 6894757.293168, 0.0, 0.0, UnitType.Stress, true, false,
			imperialUkSystem, false);
		AddUnit(119, "kg/m2", "kg/m2", "kg/m2 kgm2 kgsqm bmi", 1.0, 0.0, 0.0, UnitType.BMI, true, false,
			imperialUkSystem, true);
	}
}

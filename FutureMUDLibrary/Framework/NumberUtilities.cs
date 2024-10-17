using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MudSharp.Framework {
	public static class NumberUtilities {
		private static readonly Regex OrdinalRegex =
			new(@"^(?<number>\d+)(?:(?<=1)st|(?<=2)nd|(?<=3)rd|(?<!1|2|3)th)$", RegexOptions.IgnoreCase);

		public static string DescribeAsProbability(this double value)
		{
			if (value <= 0.0)
			{
				return "Impossible";
			}

			if (value < 0.1)
			{
				return "Extremely Unlikely";
			}

			if (value < 0.2)
			{
				return "Very Unlikely";
			}

			if (value < 0.5)
			{
				return "Unlikely";
			}

			if (value < 0.8)
			{
				return "Likely";
			}

			if (value < 0.9)
			{
				return "Very Likely";
			}

			if (value < 1)
			{
				return "Extremely Likely";
			}

			return "Certain";
		}

		private static HashSet<long> _powersOfTwo;
		public static bool IsPowerOfTwo(this long value)
		{
			if (_powersOfTwo == null)
			{
				_powersOfTwo = new HashSet<long>();
				for (int i = 0; i < 64; i++)
				{
					_powersOfTwo.Add(2L.LongPower(i));
				}
			}

			return _powersOfTwo.Contains(value);
		}

		public static bool IsPowerOfTwo(this int value)
		{
			if (_powersOfTwo == null)
			{
				_powersOfTwo = new HashSet<long>();
				for (int i = 0; i < 64; i++)
				{
					_powersOfTwo.Add(2L.LongPower(i));
				}
			}

			return _powersOfTwo.Contains(value);
		}

		public static long LongPower(this long number, int power)
		{
			var result = 1L;
			while (power > 0)
			{
				result *= number;
				power--;
			}
			return result;
		}

		public static double IfZero(this double value, double ifZero) {
			return default == value ? ifZero : value;
		}

		public static decimal IfZero(this decimal value, decimal ifZero) {
			return default == value ? ifZero : value;
		}

		public static int IfZero(this int value, int ifZero) {
			return default == value ? ifZero : value;
		}

		public static double InvertSign(this double value)
		{
			return -value;
		}

		public static decimal InvertSign(this decimal value)
		{
			return -value;
		}

		public static bool Even(this int input) {
			return input%2 == 0;
		}

		public static bool Odd(this int input) {
			return input%2 == 1;
		}

		public static bool Between(this double testVar, double range1, double range2) {
			return ((testVar <= range1) && (testVar >= range2)) || ((testVar >= range1) && (testVar <= range2));
		}

		public static double DifferenceRatio(double value1, double value2) {
			return Math.Abs(value1 - value2)/Math.Abs(value1.IfZero(1.0));
		}

		public static int Approximate(this int num, int round) {
			if (num == 0) {
				return 0;
			}

			if (round == 0) {
				return num;
			}

			return num/round*round;
		}

		public static double Approximate(this double num, double round) {
			return Math.Floor(num/round)*round;
		}

		public static string ToApproximate(this int num, int round) {
			if (num == 0) {
				return "0";
			}

			if (round == 0) {
				return num.ToString("N0", CultureInfo.InvariantCulture);
			}

			var newValue = num/round*round;
			return Math.Abs(newValue) < Math.Abs(round) ? $"less than {round:N0}" : $"approximately {newValue:N0}";
		}

		public static IEnumerable<double> GetPowerLawDistribution(double total, double chance, int count) {
			for (var i = 0; i < count; i++) {
				yield return
					total*
					(Math.Pow(1 - chance, i)*chance + Math.Pow(1 - chance, i + 1*count)*chance +
					 Math.Pow(1 - chance, i + 2*count)*chance + Math.Pow(1 - chance, i + 3*count)*chance +
					 Math.Pow(1 - chance, i + 4*count)*chance +
					 Math.Pow(1 - chance, i + 5*count)*chance +
					 Math.Pow(1 - chance, i + 6*count)*chance +
					 Math.Pow(1 - chance, i + 7*count)*chance +
					 Math.Pow(1 - chance, i + 8*count)*chance +
					 Math.Pow(1 - chance, i + 9*count)*chance
					);
			}
		}

		public static int? GetIntFromOrdinal(this string input) {
			if (string.IsNullOrEmpty(input))
			{
				return null;
			}

			if (int.TryParse(input, out int value))
			{
				return value;
			}
			if (OrdinalRegex.IsMatch(input)) {
				return int.Parse(OrdinalRegex.Match(input).Groups["number"].Value);
			}
			return null;
		}

		public static double DegreesToRadians(this double degrees) {
			return degrees*Math.PI/180.0;
		}

		public static double RadiansToDegrees(this double radians) {
			return radians*180.0/Math.PI;
		}

		public static double Wrap(this double value, double lower, double upper) {
			var width = upper - lower;
			var offset = value - lower;
			var negoffset = value - upper;

			if (offset >= 0)
			{
				return offset - (offset / width).Truncate() * width + lower;
			}

			return negoffset - (negoffset / width).Truncate() * width + upper;
		}

		/// <summary>
		///     Returns the Standard Deviation of an IEnumerable after a specified function returning double has been applied.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static double StdDev<T>(this IEnumerable<T> list, Func<T, double> values) {
			var mean = 0.0;
			var sum = 0.0;
			var stdDev = 0.0;
			var n = 0;
			foreach (var value in list.Select(values)) {
				n++;
				var delta = value - mean;
				mean += delta/n;
				sum += delta*(value - mean);
			}
			if (1 < n) {
				stdDev = Math.Sqrt(sum/(n - 1));
			}

			return stdDev;
		}

		/// <summary>
		///     Evaluates polynomial of degree N
		/// </summary>
		/// <param name="x"></param>
		/// <param name="coef"></param>
		/// <param name="N"></param>
		/// <returns></returns>
		private static double polevl(double x, double[] coef, int N) {
			var ans = coef[0];

			for (var i = 1; i <= N; i++) {
				ans = ans*x + coef[i];
			}

			return ans;
		}

		/// <summary>
		///     Evaluates polynomial of degree N with assumtion that coef[N] = 1.0
		/// </summary>
		/// <param name="x"></param>
		/// <param name="coef"></param>
		/// <param name="N"></param>
		/// <returns></returns>
		private static double p1evl(double x, double[] coef, int N) {
			var ans = x + coef[0];

			for (var i = 1; i < N; i++) {
				ans = ans*x + coef[i];
			}

			return ans;
		}

		///// <summary>
		///// Returns an approximation of the error function
		///// </summary>
		///// <param name="x"></param>
		///// <returns></returns>
		//public static double Erf(double x) {
		//    // constants
		//    double a1 = 0.254829592;
		//    double a2 = -0.284496736;
		//    double a3 = 1.421413741;
		//    double a4 = -1.453152027;
		//    double a5 = 1.061405429;
		//    double p = 0.3275911;

		//    // Save the sign of x
		//    int sign = 1;
		//    if (x < 0)
		//        sign = -1;
		//    x = Math.Abs(x);

		//    // A&S formula 7.1.26
		//    double t = 1.0 / (1.0 + p * x);
		//    double y = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * Math.Exp(-x * x);

		//    return sign * y;
		//}

		///// <summary>
		///// Returns an approximation of the complimentary error function
		///// </summary>
		///// <param name="x"></param>
		///// <returns></returns>
		//public static double Erfc(double x) {
		//    return 1 - Erf(x);
		//}

		/// <summary>
		///     Returns the complementary error function of the specified number.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static double Erfc(double a) {
			double x, p, q;

			double[] P = {
				2.46196981473530512524E-10,
				5.64189564831068821977E-1,
				7.46321056442269912687E0,
				4.86371970985681366614E1,
				1.96520832956077098242E2,
				5.26445194995477358631E2,
				9.34528527171957607540E2,
				1.02755188689515710272E3,
				5.57535335369399327526E2
			};
			double[] Q = {
				//1.0
				1.32281951154744992508E1,
				8.67072140885989742329E1,
				3.54937778887819891062E2,
				9.75708501743205489753E2,
				1.82390916687909736289E3,
				2.24633760818710981792E3,
				1.65666309194161350182E3,
				5.57535340817727675546E2
			};

			double[] R = {
				5.64189583547755073984E-1,
				1.27536670759978104416E0,
				5.01905042251180477414E0,
				6.16021097993053585195E0,
				7.40974269950448939160E0,
				2.97886665372100240670E0
			};
			double[] S = {
				//1.00000000000000000000E0, 
				2.26052863220117276590E0,
				9.39603524938001434673E0,
				1.20489539808096656605E1,
				1.70814450747565897222E1,
				9.60896809063285878198E0,
				3.36907645100081516050E0
			};

			if (a < 0.0) {
				x = -a;
			}
			else {
				x = a;
			}

			if (x < 1.0) {
				return 1.0 - Erf(a);
			}

			var z = -a*a;

			if (z < -7.09782712893383996732E2) {
				return a < 0 ? 2.0 : 0.0;
			}

			z = Math.Exp(z);

			if (x < 8.0) {
				p = polevl(x, P, 8);
				q = p1evl(x, Q, 8);
			}
			else {
				p = polevl(x, R, 5);
				q = p1evl(x, S, 6);
			}

			var y = z*p/q;

			if (a < 0) {
				y = 2.0 - y;
			}

			if (y == 0.0) {
				return a < 0 ? 2.0 : 0.0;
			}


			return y;
		}


		/// <summary>
		///     Returns the error function of the specified number.
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public static double Erf(double x) {
			double[] T = {
				9.60497373987051638749E0,
				9.00260197203842689217E1,
				2.23200534594684319226E3,
				7.00332514112805075473E3,
				5.55923013010394962768E4
			};
			double[] U = {
				//1.00000000000000000000E0,
				3.35617141647503099647E1,
				5.21357949780152679795E2,
				4.59432382970980127987E3,
				2.26290000613890934246E4,
				4.92673942608635921086E4
			};

			if (Math.Abs(x) > 1.0) {
				return 1.0 - Erfc(x);
			}
			var z = x*x;
			var y = x*polevl(z, T, 4)/p1evl(z, U, 5);
			return y;
		}

		/// <summary>
		/// Returns the modulus of a number. This differs from using the % operator in the way it handles negative numbers, because the % operator returns the remainder of arithmetic division, which is not equal to the modulus for negative numbers.
		/// 
		/// Use this function when you may need the modulus of a negative number. If your number is uint or guaranteed to be positive use the % operator.
		/// </summary>
		/// <param name="x">The number to take the modulus of</param>
		/// <param name="m">The quotient for the modulus</param>
		/// <returns></returns>
		public static int Modulus(this int x, int m) {
			return (x%m + m)%m;
		}

		/// <summary>
		/// Returns the modulus of a number. This differs from using the % operator in the way it handles negative numbers, because the % operator returns the remainder of arithmetic division, which is not equal to the modulus for negative numbers.
		/// 
		/// Use this function when you may need the modulus of a negative number. If your number is guaranteed to be positive use the % operator.
		/// </summary>
		/// <param name="x">The number to take the modulus of</param>
		/// <param name="m">The quotient for the modulus</param>
		/// <returns></returns>
		public static double Modulus(this double x, double m)
		{
			return (x % m + m) % m;
		}

		public static bool TryParsePercentage(this string text, CultureInfo culture, out double value) {
			value = 0.0;
			if (!text.Contains(culture.NumberFormat.PercentSymbol)) {
				if (!double.TryParse(text, out var fallback)) {
					return false;
				}

				value = fallback;
				return true;
			}
			var result = double.TryParse(text.Replace(culture.NumberFormat.PercentSymbol, "").Trim(), NumberStyles.Any,
				culture.NumberFormat, out value);
			if (result) {
				value = value/100.0;
			}
			return result;
		}

		public static bool TryParsePercentageDecimal(this string text, CultureInfo culture, out decimal value)
		{
			value = 0.0M;
			if (!text.Contains(culture.NumberFormat.PercentSymbol))
			{
				if (!decimal.TryParse(text, out var fallback))
				{
					return false;
				}

				value = fallback;
				return true;
			}
			var result = decimal.TryParse(text.Replace(culture.NumberFormat.PercentSymbol, "").Trim(), NumberStyles.Any,
				culture.NumberFormat, out value);
			if (result)
			{
				value = value / 100.0M;
			}
			return result;
		}

		public static bool TryParsePercentage(this string text, out double value) {
			return TryParsePercentage(text, CultureInfo.InvariantCulture, out value);
		}

		public static bool TryParsePercentageDecimal(this string text, out decimal value)
		{
			return TryParsePercentageDecimal(text, CultureInfo.InvariantCulture, out value);
		}

		/// <summary>
		/// Returns a list of all the integers between to integer values, inclusive e.g. 10.GetIntRange(15) returns {10, 11, 12, 13, 14, 15}. toInt can be lower than fromInt, in which case the range is reversed, e.g. 10.GetIntRange(8) returns {10, 9, 8}
		/// </summary>
		/// <param name="fromInt">The lowest int in the range</param>
		/// <param name="toInt">The highest int in the range</param>
		/// <returns></returns>
		public static List<int> GetIntRange(this int fromInt, int toInt) {
			var reverseDirection = toInt < fromInt;
			var results = new List<int>(toInt - fromInt + 1);
			if (reverseDirection) {
				for (var i = fromInt; i >= toInt; i--)
				{
					results.Add(i);
				}
			}
			else {
				for (var i = fromInt; i <= toInt; i++)
				{
					results.Add(i);
				}
			}
			
			return results;
		}

		public static double Floor(this double value)
		{
			return Math.Floor(value);
		}

		public static decimal Floor(this decimal value)
		{
			return Math.Floor(value);
		}

		public static double Ceiling(this double value)
		{
			return Math.Ceiling(value);
		}

		public static decimal Ceiling(this decimal value)
		{
			return Math.Ceiling(value);
		}

		public static double Round(this double value)
		{
			return Math.Round(value);
		}

		public static decimal Round(this decimal value)
		{
			return Math.Round(value);
		}

		public static double Round(this double value, int digits)
		{
			return Math.Round(value, digits);
		}

		public static decimal Round(this decimal value, int digits) {
			return Math.Round(value, digits);
		}

		public static double Round(this double value, MidpointRounding rounding)
		{
			return Math.Round(value, rounding);
		}

		public static decimal Round(this decimal value, MidpointRounding rounding)
		{
			return Math.Round(value, rounding);
		}

		public static double Round(this double value, int digits, MidpointRounding rounding)
		{
			return Math.Round(value, digits, rounding);
		}

		public static decimal Round(this decimal value, int digits, MidpointRounding rounding)
		{
			return Math.Round(value, digits, rounding);
		}

		public static double Truncate(this double value)
		{
			return Math.Truncate(value);
		}

		public static decimal Truncate(this decimal value)
		{
			return Math.Truncate(value);
		}

		public static double Abs(this double value)
		{
			return Math.Abs(value);
		}

		public static decimal Abs(this decimal value)
		{
			return Math.Abs(value);
		}

		public static int Abs(this int value)
		{
			return Math.Abs(value);
		}

		public static long Abs(this long value)
		{
			return Math.Abs(value);
		}

		#region Number to String Helpers

		public static string ToOrdinal(this int num) {
			switch (num%100) {
				case 11:
				case 12:
				case 13:
					return num + "th";
			}

			switch (num%10) {
				case 1:
					return num + "st";
				case 2:
					return num + "nd";
				case 3:
					return num + "rd";
				default:
					return num + "th";
			}
		}

		private static readonly string[] RomanNumerals = {
		"I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M"
	};

		private static readonly int[] DecimalValues = {
		1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000
	};

		private static readonly int MaxValueRomanNumeral = 3999999;

		public static string ToRomanNumeral(this int num)
		{
			if (num < 0 || num >= MaxValueRomanNumeral)
				throw new ArgumentOutOfRangeException(nameof(num), $"Input must be between 1 and {MaxValueRomanNumeral}");

			if (num == 0)
			{
				return "N";
			}

			if (num > 3999)
			{
				int thousands = num / 1000;
				int remainder = num % 1000;
				string thousandsRoman = ToRomanNumeral(thousands);
				string remainderRoman = remainder == 0 ? "" : ToRomanNumeral(remainder);

				return $"|{thousandsRoman}|{remainderRoman}";
			}

			StringBuilder sb = new StringBuilder();

			for (int i = RomanNumerals.Length - 1; i >= 0; i--)
			{
				while (num >= DecimalValues[i])
				{
					sb.Append(RomanNumerals[i]);
					num -= DecimalValues[i];
				}
			}

			return sb.ToString();
		}

		public static string ToWordyNumber(this int num) {
			return ToWordyNumber((long) num);
		}

		public static string ToWordyOrdinal(this int num) {
			return ToWordyOrdinal((long) num);
		}

		public static string ToWordyNumber(this long num) {
			if (num < 0) {
				return "negative " + ToWordyNumber(-1*num);
			}
			switch (num) {
				case 0:
					return "zero";
				case 1:
					return "one";
				case 2:
					return "two";
				case 3:
					return "three";
				case 4:
					return "four";
				case 5:
					return "five";
				case 6:
					return "six";
				case 7:
					return "seven";
				case 8:
					return "eight";
				case 9:
					return "nine";
				case 10:
					return "ten";
				case 11:
					return "eleven";
				case 12:
					return "twelve";
				case 13:
					return "thirteen";
				case 14:
					return "fourteen";
				case 15:
					return "fifteen";
				case 16:
					return "sixteen";
				case 17:
					return "seventeen";
				case 18:
					return "eighteen";
				case 19:
					return "nineteen";
			}

			if (num/1000000000000 > 0) {
				return ToWordyNumber(num/1000000000000) + " trillion" +
					   (num%1000000000000 == 0 ? "" : " " + ToWordyNumber(num%1000000000000));
			}
			if (num/1000000000 > 0) {
				return ToWordyNumber(num/1000000000) + " billion" +
					   (num%1000000000 == 0 ? "" : " " + ToWordyNumber(num%1000000000));
			}
			if (num/1000000 > 0) {
				return ToWordyNumber(num/1000000) + " million" +
					   (num%1000000 == 0 ? "" : " " + ToWordyNumber(num%1000000));
			}
			if (num/1000 > 0) {
				return ToWordyNumber(num/1000) + " thousand" +
					   (num%1000 == 0 ? "" : (num%1000 < 100 ? " and " : " ") + ToWordyNumber(num%1000));
			}
			if (num/100 > 0) {
				return ToWordyNumber(num/100) + " hundred" + (num%100 != 0 ? " and " + ToWordyNumber(num%100) : "");
			}
			switch (num/10) {
				case 2:
					return "twenty" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
				case 3:
					return "thirty" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
				case 4:
					return "forty" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
				case 5:
					return "fifty" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
				case 6:
					return "sixty" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
				case 7:
					return "seventy" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
				case 8:
					return "eighty" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
				case 9:
					return "ninety" + (num%10 != 0 ? " " + ToWordyNumber(num%10) : "");
			}

			return "unknown";
		}

		public static string ToWordyOrdinal(this long num) {
			if (num < 0) {
				return "negative " + ToWordyOrdinal(-1*num);
			}
			switch (num) {
				case 0:
					return "zeroth";
				case 1:
					return "first";
				case 2:
					return "second";
				case 3:
					return "third";
				case 4:
					return "fourth";
				case 5:
					return "fifth";
				case 6:
					return "sixth";
				case 7:
					return "seventh";
				case 8:
					return "eighth";
				case 9:
					return "ninth";
				case 10:
					return "tenth";
				case 11:
					return "eleventh";
				case 12:
					return "twelfth";
				case 13:
					return "thirteenth";
				case 14:
					return "fourteenth";
				case 15:
					return "fifteenth";
				case 16:
					return "sixteenth";
				case 17:
					return "seventeenth";
				case 18:
					return "eighteenth";
				case 19:
					return "nineteenth";
			}

			if (num/1000000000000 > 0) {
				return ToWordyNumber(num/1000000000000) + " trillion" +
					   (num%1000000000000 == 0 ? "th" : " " + ToWordyOrdinal(num%1000000000000));
			}
			if (num/1000000000 > 0) {
				return ToWordyNumber(num/1000000000) + " billion" +
					   (num%1000000000 == 0 ? "th" : " " + ToWordyOrdinal(num%1000000000));
			}
			if (num/1000000 > 0) {
				return ToWordyNumber(num/1000000) + " million" +
					   (num%1000000 == 0 ? "th" : " " + ToWordyOrdinal(num%1000000));
			}
			if (num/1000 > 0) {
				return ToWordyNumber(num/1000) + " thousand" + (num%1000 == 0 ? "th" : " " + ToWordyOrdinal(num%1000));
			}
			if (num/100 > 0) {
				return ToWordyNumber(num/100) + " hundred" + (num%100 != 0 ? " and " + ToWordyOrdinal(num%100) : "th");
			}
			switch (num/10) {
				case 2:
					return num%10 != 0 ? "twenty " + ToWordyOrdinal(num%10) : "twentieth";
				case 3:
					return num%10 != 0 ? "thirty " + ToWordyOrdinal(num%10) : "thirtieth";
				case 4:
					return num%10 != 0 ? "forty " + ToWordyOrdinal(num%10) : "fortieth";
				case 5:
					return num%10 != 0 ? "fifty " + ToWordyOrdinal(num%10) : "fiftieth";
				case 6:
					return num%10 != 0 ? "sixty " + ToWordyOrdinal(num%10) : "sixtieth";
				case 7:
					return num%10 != 0 ? "seventy " + ToWordyOrdinal(num%10) : "seventieth";
				case 8:
					return num%10 != 0 ? "eighty " + ToWordyOrdinal(num%10) : "eightieth";
				case 9:
					return num%10 != 0 ? "ninety " + ToWordyOrdinal(num%10) : "ninetieth";
			}

			return "unknown";
		}

		public static string ToWordyApproximate(this int num, long round) {
			return ToWordyApproximate((long) num, round);
		}

		public static string ToWordyApproximate(this long num, long round) {
			if (num == 0) {
				return "zero";
			}

			if (round == 0) {
				return num.ToWordyNumber();
			}

			var newValue = num/round*round;
			return Math.Abs(newValue) < Math.Abs(round) ? $"less than {round.ToWordyNumber()}" : $"approximately {newValue.ToWordyNumber()}";
		}

		#endregion
	}

	public static class NullableMath {
		public static decimal Max(this decimal value, params decimal?[] vals) {
			return vals.Any(x => x.HasValue)
				? Math.Max(value, vals.Where(x => x.HasValue).DefaultIfEmpty(0).Max() ?? 0)
				: value;
		}

		public static decimal Min(this decimal value, params decimal?[] vals) {
			return vals.Any(x => x.HasValue)
				? Math.Min(value, vals.Where(x => x.HasValue).DefaultIfEmpty(0).Min() ?? 0)
				: value;
		}

		public static decimal? GetDecimal(this string input) {
			if (decimal.TryParse(input, out decimal value))
			{
				return value;
			}
			return null;
		}

		public static double? GetDouble(this string input) {
			if (double.TryParse(input, out double value))
			{
				return value;
			}
			return null;
		}
	}
}
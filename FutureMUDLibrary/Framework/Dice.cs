using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

// A class for rolling dice

namespace MudSharp.Framework;

public static class Dice {
	// TODO: improve regex to avoid false positive.
	private static readonly Regex _regex = new(@"(?<sign>\+|\-){0,1}(?<numdice>\d+){1}(?:d(?<sides>\d+))*(?:\s*(?<mod1>m|M|k|K|e|r|R|l)(?<val1>\d+)){0,1}(?:\s*(?<mod2>m|M|k|K|e|r|R|l)(?<val2>\d+)){0,1}(?:\s*(?<mod3>m|M|k|K|e|r|R|l)(?<val3>\d+)){0,1}", RegexOptions.IgnoreCase);

	public static bool IsDiceExpression(string input) {
		var strings = input.Split([';']);
		foreach (var text in strings)
		{
			if (_regex.Replace(text, "").Length > 0)
			{
				return false;
			}
		}

		return true;
	}

	/// <summary>Returns the roll of a string dice. Accepts multiple rolls and addition, e.g. 4d2d2 + 2 should work.</summary>
	/// <param name="expression">string to be evaluated</param>
	/// <returns>result of evaluated string</returns>
	public static int Roll(string expression) {
		var strings = expression.Split([';']);
		var sum = 0;
		foreach (var s in strings)
		{
			if (int.TryParse(s, out var number))
			{
				sum += number;
				continue;
			}

			foreach (Match match in _regex.Matches(s))
			{
				var sides = match.Groups["sides"].Length > 0 ? int.Parse(match.Groups["sides"].Value) : 1;
				var numdice = match.Groups["numdice"].Length > 0 ? int.Parse(match.Groups["numdice"].Value) : 0;
				var bonus = match.Groups["bonus"].Length > 0
					? (match.Groups["bonustype"].Value != "-" ? 1 : -1) * int.Parse(match.Groups["bonus"].Value)
					: 0;
				var sign = match.Groups["sign"].Length == 0 || match.Groups["sign"].Value == "+" ? 1 : -1;

				var opt1 = match.Groups["mod1"].Value;
				var opt2 = match.Groups["mod2"].Value;
				var opt3 = match.Groups["mod3"].Value;

				bool isMin = false, isMax = false, isKeep = false, isKeepLowest = false, isRerollOnce = false, isRerollUntil = false, isExplodes = false;
				var minMaxReference = 0;
				var keepReference = 0;
				var rerollReference = 0;
				var explodesReference = 0;

				// Process options
				switch (opt1)
				{
					case "m":
						isMax = false;
						isMin = true;
						minMaxReference = int.Parse(match.Groups["val1"].Value);
						break;
					case "M":
						isMin = false;
						isMax = true;
						minMaxReference = int.Parse(match.Groups["val1"].Value);
						break;
					case "k":
					case "K":
						isKeep = true;
						isKeepLowest = false;
						keepReference = int.Parse(match.Groups["val1"].Value);
						break;
					case "e":
					case "E":
						isExplodes = true;
						explodesReference = int.Parse(match.Groups["val1"].Value);
						break;
					case "l":
					case "L":
						isKeepLowest = true;
						isKeep = false;
						keepReference = int.Parse(match.Groups["val1"].Value);
						break;
					case "r":
						isRerollOnce = true;
						isRerollUntil = false;
						rerollReference = int.Parse(match.Groups["val1"].Value);
						break;
					case "R":
						isRerollOnce = false;
						isRerollUntil = true;
						rerollReference = int.Parse(match.Groups["val1"].Value);
						break;
				}
				switch (opt2)
				{
					case "m":
						isMax = false;
						isMin = true;
						minMaxReference = int.Parse(match.Groups["val2"].Value);
						break;
					case "M":
						isMin = false;
						isMax = true;
						minMaxReference = int.Parse(match.Groups["val2"].Value);
						break;
					case "k":
					case "K":
						isKeep = true;
						isKeepLowest = false;
						keepReference = int.Parse(match.Groups["val2"].Value);
						break;
					case "e":
					case "E":
						isExplodes = true;
						explodesReference = int.Parse(match.Groups["val2"].Value);
						break;
					case "l":
					case "L":
						isKeepLowest = true;
						isKeep = false;
						keepReference = int.Parse(match.Groups["val2"].Value);
						break;
					case "r":
						isRerollOnce = true;
						isRerollUntil = false;
						rerollReference = int.Parse(match.Groups["val2"].Value);
						break;
					case "R":
						isRerollOnce = false;
						isRerollUntil = true;
						rerollReference = int.Parse(match.Groups["val2"].Value);
						break;
				}
				switch (opt3)
				{
					case "m":
						isMax = false;
						isMin = true;
						minMaxReference = int.Parse(match.Groups["val3"].Value);
						break;
					case "M":
						isMin = false;
						isMax = true;
						minMaxReference = int.Parse(match.Groups["val3"].Value);
						break;
					case "k":
					case "K":
						isKeep = true;
						isKeepLowest = false;
						keepReference = int.Parse(match.Groups["val3"].Value);
						break;
					case "e":
					case "E":
						isExplodes = true;
						explodesReference = int.Parse(match.Groups["val3"].Value);
						break;
					case "l":
					case "L":
						isKeepLowest = true;
						isKeep = false;
						keepReference = int.Parse(match.Groups["val3"].Value);
						break;
					case "r":
						isRerollOnce = true;
						isRerollUntil = false;
						rerollReference = int.Parse(match.Groups["val3"].Value);
						break;
					case "R":
						isRerollOnce = false;
						isRerollUntil = true;
						rerollReference = int.Parse(match.Groups["val3"].Value);
						break;
				}

                                // Avoid infinite loops when rerolling until a
                                // result higher than the die can provide
                                if (isRerollUntil && rerollReference >= sides)
                                {
                                        return 0;
                                }


				var rolls = new List<int>(sides);
				for (var i = 0; i < numdice; i++)
				{
					var roll = Constants.Random.Next(1, sides + 1);
					if (isRerollOnce && roll <= rerollReference)
					{
						roll = Constants.Random.Next(1, sides + 1);
					}
					else if (isRerollUntil && roll <= rerollReference)
					{
						i--;
						continue;
					}

					rolls.Add(roll);

					if (isExplodes && explodesReference <= roll)
					{
						i--;
					}
				}

				var tempSum = 0;
				if (isKeep)
				{
					tempSum = rolls.OrderByDescending(x => x).Take(keepReference).Sum();
				}
				else if (isKeepLowest)
				{
					tempSum = rolls.OrderBy(x => x).Take(keepReference).Sum();
				}
				else
				{
					tempSum = rolls.Sum();
				}

				tempSum += bonus;

				if (isMax && tempSum > minMaxReference)
				{
					tempSum = minMaxReference;
				}
				else if (isMin && tempSum < minMaxReference)
				{
					tempSum = minMaxReference;
				}

				tempSum *= sign;
				sum += tempSum;
			}
		}

		return sum;
	}

	/// <summary>
	///     Returns a standard dice roll.
	/// </summary>
	/// <param name="dice"></param>
	/// <param name="sides"></param>
	/// <param name="bonus"></param>
	/// <returns></returns>
	public static int Roll(int dice, int sides, int bonus = 0) {
		var value = 0;

		for (var i = 0; i < dice; i++) {
			value += Constants.Random.Next(1, sides + 1);
		}

		value += bonus;

		return value;
	}
}
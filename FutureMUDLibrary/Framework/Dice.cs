using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudSharp.Framework;

/// <summary>
///	Utility helpers for rolling dice and evaluating dice expressions.
/// </summary>
public static class Dice {
	// TODO: improve regex to avoid false positive.
	private static readonly Regex _regex = new(@"(?<sign>\+|\-){0,1}(?<numdice>\d+){1}(?:d(?<sides>\d+))*(?:\s*(?<mod1>m|M|k|K|e|r|R|l)(?<val1>\d+)){0,1}(?:\s*(?<mod2>m|M|k|K|e|r|R|l)(?<val2>\d+)){0,1}(?:\s*(?<mod3>m|M|k|K|e|r|R|l)(?<val3>\d+)){0,1}", RegexOptions.IgnoreCase);

	/// <summary>
	///	Determines whether an input string contains only supported dice expression tokens.
	/// </summary>
	/// <param name="input">The expression to validate.</param>
	/// <returns>
	///	<see langword="true"/> if every semicolon-delimited segment contains only supported tokens; otherwise,
	///	<see langword="false"/>.
	/// </returns>
	/// <remarks>
	///	This is a syntactic check only. It does not guarantee that all options are meaningful at runtime
	///	(e.g., a reroll-until threshold that can never be exceeded).
	/// </remarks>
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

	/// <summary>
	///	Rolls dice based on a dice expression string and returns the total.
	/// </summary>
	/// <param name="expression">The expression to evaluate.</param>
	/// <returns>The evaluated total for the expression.</returns>
	/// <remarks>
	///	Expressions may contain multiple segments separated by semicolons. Each segment is parsed as a sum of one
	///	or more signed dice groups. A dice group uses the following format:
	///	<code>[+|-]N[dS][options]</code>
	///	<list type="bullet">
	///		<item><description><c>N</c> is the number of dice. If <c>dS</c> is omitted, the group is treated as a constant.</description></item>
	///		<item><description><c>S</c> is the number of sides per die (defaults to 1 if omitted).</description></item>
	///		<item><description>Up to three <c>options</c> may be appended, each preceded by optional whitespace.</description></item>
	///	</list>
	///	The optional leading sign applies to the group's total before it is added to the segment sum.
	///	Supported options:
	///	<list type="bullet">
	///		<item><description><c>mX</c> - Minimum total for the group (clamps up to <c>X</c>).</description></item>
	///		<item><description><c>MX</c> - Maximum total for the group (clamps down to <c>X</c>).</description></item>
	///		<item><description><c>kX</c> - Keep the highest <c>X</c> dice.</description></item>
	///		<item><description><c>lX</c> - Keep the lowest <c>X</c> dice.</description></item>
	///		<item><description><c>rX</c> - Reroll once any die that is less than or equal to <c>X</c>.</description></item>
	///		<item><description><c>RX</c> - Reroll until a die is greater than <c>X</c> (if <c>X</c> is greater than or equal to the number of sides, the roll returns 0).</description></item>
	///		<item><description><c>eX</c> - Explode on rolls greater than or equal to <c>X</c> (adds an extra die per trigger).</description></item>
	///	</list>
	///	Use <see cref="Roll(string, bool)"/> to capture the raw roll sequence.
	///	Examples:
	///	<code>
	///	3d6k2+1
	///	2d10 r1
	///	1d8e8; 2d4+2
	///	</code>
	/// </remarks>
	public static int Roll(string expression) {
		return RollInternal(expression, null);
	}

	/// <summary>
	///	Rolls dice based on a dice expression string and returns the total plus raw roll results.
	/// </summary>
	/// <param name="expression">The expression to evaluate.</param>
	/// <param name="includeRawRolls">
	///	When <see langword="true"/>, collects and returns raw rolls; otherwise returns an empty sequence.
	/// </param>
	/// <returns>
	///	A tuple containing the evaluated total and the raw roll sequence in roll order.
	/// </returns>
	/// <remarks>
	///	Raw rolls include results that were discarded due to reroll options, plus any extra rolls created by explosions.
	///	See <see cref="Roll(string)"/> for expression syntax.
	/// </remarks>
	public static (int result, IEnumerable<int> rolls) Roll(string expression, bool includeRawRolls) {
		List<int> rawRolls = null;
		if (includeRawRolls)
		{
			rawRolls = new List<int>();
		}

		var result = RollInternal(expression, rawRolls);
		return (result, rawRolls ?? Enumerable.Empty<int>());
	}

	private static int RollInternal(string expression, List<int> rawRolls) {
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
					rawRolls?.Add(roll);
					if (isRerollOnce && roll <= rerollReference)
					{
						roll = Constants.Random.Next(1, sides + 1);
						rawRolls?.Add(roll);
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
	///	Returns a standard dice roll (sum of <paramref name="dice"/> rolls of a die with <paramref name="sides"/> sides).
	/// </summary>
	/// <param name="dice">The number of dice to roll.</param>
	/// <param name="sides">The number of sides per die.</param>
	/// <param name="bonus">A flat modifier added to the total.</param>
	/// <returns>The total of all rolls plus any bonus.</returns>
	public static int Roll(int dice, int sides, int bonus = 0) {
		var value = 0;

		for (var i = 0; i < dice; i++) {
			value += Constants.Random.Next(1, sides + 1);
		}

		value += bonus;

		return value;
	}
}

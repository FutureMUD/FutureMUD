using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace MudSharp.Framework {
	public static class RandomUtilities
    {
        public static long LongRandom(long min, long max) {
            var buf = new byte[8];
            Constants.Random.NextBytes(buf);
            var longRand = BitConverter.ToInt64(buf, 0);
            return Math.Abs(longRand%(max - min)) + min;
        }

        public static double DoubleRandom(double min, double max) {
            return Constants.Random.NextDouble()*(max - min) + min;
        }

        public static int Random(int min, int max) {
            return Constants.Random.Next(min, max + 1);
        }

        private static void Shuffle<T>(this T[] array) {
            for (var i = array.Length; i > 1; i--) {
                // Pick random element to swap.
                var j = Constants.Random.Next(i); // 0 <= j <= i-1
                // Swap.
                (array[j], array[i - 1]) = (array[i - 1], array[j]);
            }
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable) {
            var array = enumerable.ToArray();
            array.Shuffle();
            return array;
        }

        /// <summary>
        /// Returns a random standard deviation with a probability defined by Student's Distribution
        /// </summary>
        /// <returns>A random standard deviation</returns>
        public static double NextStandard() {
            double rsq;
            double v2;
            do {
                var v1 = 2.0*Constants.Random.NextDouble() - 1.0;
                v2 = 2.0*Constants.Random.NextDouble() - 1.0;
                rsq = v1*v1 + v2*v2;
            } while ((rsq > 1.0) || (rsq == 0));
            var fac = Math.Sqrt(-2.0*Math.Log(rsq)/rsq);
            return v2*fac;
        }

        /// <summary>
        /// Given a mean value and a standard deviation, returns a random value with probability in accordance with Student's Distribution (i.e. Normal Distribution)
        /// </summary>
        /// <param name="mean">The mean of the distribution</param>
        /// <param name="stdev">The standard deviation of the distribution</param>
        /// <returns>A random value</returns>
        public static double RandomNormal(double mean, double stdev) {
            return mean + NextStandard()*stdev;
        }

        /// <summary>
        /// A wrapper for RandomNormal that assumes the mean is the mid-point of the supplied range and the the standard deviation is 1/8 of the difference
        /// </summary>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public static double RandomNormalOverRange(double minimum, double maximum)
        {                
            return RandomNormal((maximum + minimum) / 2, (maximum - minimum) / 8);
        }

        /// <summary>
        ///     Picks a specified number of random results out of the input, without replacement.
        ///     
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="input">An IEnumerable from which random items are to be selected</param>
        /// <param name="picks">The number of picks to be made. Throws an exception if greater than input.Count()</param>
        /// <returns>A number of random picks from the input.</returns>
        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> input, int picks) {
            if (picks < 1) {
                throw new ArgumentException("The number of picks in PickRandom must be greater than 0");
            }
            var samplesRemaining = input.Count();
            if (picks > samplesRemaining) {
                throw new ArgumentException(
                    "The number of picks in PickRandom must be greater than the number of itmes in the input IEnumerable.");
            }

            var items = new HashSet<T>();
            var length = input.Count();
            while (picks > 0)
                // if we successfully added it, move on
            {
                if (items.Add(input.ElementAt(Constants.Random.Next(length)))) {
                    picks--;
                }
            }

            return items;
        }

        /// <summary>
        ///     Picks a specified number of random results out of the input, without replacement.
        /// </summary>
        /// <typeparam name="T">Any type</typeparam>
        /// <param name="input">An IEnumerable from which random items are to be selected</param>
        /// <param name="picks">The number of picks to be made</param>
        /// <returns>A number of random picks from the input.</returns>
        public static IEnumerable<T> PickUpToRandom<T>(this IEnumerable<T> input, int picks)
        {
            if (picks < 1)
            {
                throw new ArgumentException("The number of picks in PickRandom must be greater than 0");
            }
            var samplesRemaining = input.Count();
            if (picks > samplesRemaining)
            {
                return input;
            }

            var items = new HashSet<T>();
            var length = input.Count();
            while (picks > 0)
            // if we successfully added it, move on
            {
                if (items.Add(input.ElementAt(Constants.Random.Next(length))))
                {
                    picks--;
                }
            }

            return items;
        }

        public static IEnumerable<T> TakeRandom<T>(this IEnumerable<T> input, int picks, Func<T,double> weightSelector)
        {
            if (picks < 1)
            {
                return Enumerable.Empty<T>();
            }

            var choices = input.Select(x => (Value: x, Weight: weightSelector(x))).ToList();
            if (choices.Count <= picks)
            {
                return input;
            }

            var sum = choices.Sum(x => x.Weight);
            var len = choices.Count;
            var results = new List<T>(picks);
            while (picks > 0)
            {
                var roll = Constants.Random.NextDouble() * sum;
                for (var i = 0; i < len; i++)
                {
                    if (choices[i].Weight <= 0)
                    {
                        continue;
                    }

                    if ((roll -= choices[i].Weight) <= 0.0)
                    {
                        var (value, weight) = choices[i];
                        results.Add(value);
                        choices.RemoveAt(i);
                        len--;
                        sum -= weight;
                        break;
                    }
                }
                picks--;
            }

            return results;
        }

        /// <summary>
        ///     Given a specified number of true and false values, returns a "shuffled" queue of the values in a random order
        /// </summary>
        /// <param name="numtrue">The number of times true will appear</param>
        /// <param name="numfalse">The number of times false will appear</param>
        /// <returns>A shuffled queue of booleans</returns>
        public static Queue<bool> RandomTruthMask(int numtrue, int numfalse) {
            var bools = new bool[numtrue + numfalse];
            for (var i = 0; i < numtrue; i++) {
                bools[i] = true;
            }
            bools.Shuffle();

            return new Queue<bool>(bools);
        }

        public static Queue<bool> RandomBiasedTruthMask(int numtrue, int numfalse, double[] bias) {
            var len = bias.Length;
            var biasArray = new (int Element, double Weight)[len];
            for (var j = 0; j < len; j++)
            {
                biasArray[j] = (j, bias[j]);
            }

            if (biasArray.Length != numtrue + numfalse) {
                throw new ArgumentOutOfRangeException(nameof(bias));
            }

            var bools = new bool[numtrue + numfalse];
            for (var i = 0; i < numtrue; i++) {
                var random = biasArray.GetWeightedRandom(x => x.Weight);
                biasArray[random.Element] = (random.Element, 0.0);
                bools[random.Element] = true;
            }

            return new Queue<bool>(bools);
        }

        public static char GetRandomCharacter(bool proper = false) {
            return proper
                ? char.ToUpper(Constants.ValidRandomCharacters[Constants.Random.Next(0, 26)])
                : Constants.ValidRandomCharacters[Constants.Random.Next(0, 26)];
        }

        public static bool ConsecutiveSuccess(int rolls, double probability) {
            while (rolls-- > 0) {
                if (Constants.Random.NextDouble() > probability) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Roll a 1dSides dice against target maxAttempts times, and return an integer representing the number of consecutive
        ///     passes or fails
        /// </summary>
        /// <param name="sides">The sides of the dice to roll</param>
        /// <param name="target">The target number to compare against</param>
        /// <param name="maxAttempts">The maximum number of attempts to make</param>
        /// <param name="rolls">A list of doubles that will end up storing the actual roles generated by the calls to Roll</param>
        /// <returns>
        ///     An integer representing the number of consecutive passes or fails. Scale indicates number of consecutive
        ///     results, Sign indicates success (+ve) or failure (-ve)
        /// </returns>
        public static int ConsecutiveRoll(double sides, double target, int maxAttempts, out List<double> rolls)
        {
            rolls = new List<double>();
            var numRolls = 0;
            var firstResult = Roll(sides, target, out var nextRoll );
            rolls.Add(nextRoll);
            while (++numRolls < maxAttempts)
            {
                if (Roll(sides, target, out nextRoll) != firstResult)
                {
                    rolls.Add(nextRoll);
                    break;
                }
                rolls.Add(nextRoll);
            }
            
            return firstResult ? numRolls : -1* numRolls;
        }

        /// <summary>
        ///     Roll a 1dSides dice against target maxAttempts times, and return an integer representing the number of consecutive
        ///     passes or fails
        /// </summary>
        /// <param name="sides">The sides of the dice to roll</param>
        /// <param name="target">The target number to compare against</param>
        /// <param name="maxAttempts">The maximum number of attempts to make</param>
        /// <returns>
        ///     An integer representing the number of consecutive passes or fails. Scale indicates number of consecutive
        ///     results, Sign indicates success (+ve) or failure (-ve)
        /// </returns>
        public static int ConsecutiveRoll(double sides, double target, int maxAttempts)
        {
            var numRolls = 0;
            var firstResult = Roll(sides, target);
            while ((++numRolls < maxAttempts) && (Roll(sides, target) == firstResult))
            {
            }
            return firstResult ? numRolls : -1 * numRolls;
        }

        public static int EvaluateConsecutiveSuccesses(IEnumerable<int> rollResults, int target) {
            if (!rollResults.Any()) {
                return 0;
            }

            if (rollResults.First() <= target) {
                return rollResults.TakeWhile(x => x <= target).Count();
            }

            return rollResults.TakeWhile(x => x > target).Count()*-1;
        }

        public static int EvaluateConsecutiveSuccesses(IEnumerable<double> rollResults, double target)
        {
            if (!rollResults.Any())
            {
                return 0;
            }

            if (rollResults.First() <= target)
            {
                return rollResults.TakeWhile(x => x <= target).Count();
            }

            return rollResults.TakeWhile(x => x > target).Count() * -1;
        }

        /// <summary>
        /// Rolls below a target number, a.k.a. d100 mechanics / traditional RPI engine
        /// </summary>
        /// <param name="sides">The "x" component of the 1dx roll</param>
        /// <param name="target">The target number to roll below</param>
        /// <param name="roll">The actual rolled result as an out parameter</param>
        /// <returns></returns>
        public static bool Roll(double sides, double target, out double roll)
        {
            roll = Constants.Random.NextDouble() * sides; 
            
            return roll <= target;
        }

        /// <summary>
        /// Rolls below a target number, a.k.a. d100 mechanics / traditional RPI engine
        /// </summary>
        /// <param name="sides">The "x" component of the 1dx roll</param>
        /// <param name="target">The target number to roll below</param>
        /// <returns></returns>
        public static bool Roll(double sides, double target)
        {
            return (Constants.Random.NextDouble() * sides) <= target;
        }

        public static T GetWeightedRandom<T>(this IEnumerable<(T Value, int Weight)> arg)
        {
            if (!arg.Any())
            {
                return default;
            }
            var len = arg.Count();
            var sum = arg.Sum(x => x.Weight);
            var calcValues = arg as (T Value, int Weight)[] ?? arg.ToArray();

            var roll = Constants.Random.NextDouble() * sum;
            for (var i = 0; i < len; i++)
            {
                if (calcValues[i].Weight <= 0)
                {
                    continue;
                }

                if ((roll -= calcValues[i].Weight) <= 0.0)
                {
                    return calcValues[i].Value;
                }
            }

            return calcValues.Last().Value;
        }

        public static T GetWeightedRandom<T>(this IEnumerable<(T Value, double Weight)> arg)
        {
            if (!arg.Any())
            {
                return default;
            }
            var len = arg.Count();
            var sum = arg.Sum(x => x.Weight);
            var calcValues = arg as (T Value, double Weight)[] ?? arg.ToArray();

            var roll = Constants.Random.NextDouble() * sum;
            for (var i = 0; i < len; i++)
            {
                if (calcValues[i].Weight <= 0)
                {
                    continue;
                }

                if ((roll -= calcValues[i].Weight) <= 0.0)
                {
                    return calcValues[i].Value;
                }
            }

            return calcValues.Last().Value;
        }

        public static T GetWeightedRandom<T>(this IEnumerable<T> arg, Func<T, double> evaluator) {
            if (!arg.Any())
            {
                return default;
            }
            var len = arg.Count();
            var calcValues = new(T Value, double Weight)[len];
            var i = 0;
            var sum = 0.0;
            foreach (var item in arg)
            {
                var eval = evaluator(item);
                calcValues[i++] = (item, eval);
                sum += eval;
            }

            var roll = Constants.Random.NextDouble() * sum;
            for (i = 0; i < len; i++)
            {
                if (calcValues[i].Weight <= 0.0)
                {
                    continue;
                }

                if ((roll -= calcValues[i].Weight) <= 0.0)
                {
                    return calcValues[i].Value;
                }
            }

            return calcValues.Last().Value;
        }

        public static T GetWeightedRandom<T>(this IEnumerable<T> arg, Func<T, int> evaluator) {
            if (!arg.Any())
            {
                return default;
            }
            var len = arg.Count();
            var calcValues = new(T Value, int Weight)[len];
            var i = 0;
            var sum = 0;
            foreach (var item in arg)
            {
                var eval = evaluator(item);
                calcValues[i++] = (item, eval);
                sum += eval;
            }

            var roll = (int)(Constants.Random.NextDouble() * sum);
            for (i = 0; i < len; i++)
            {
                if (calcValues[i].Weight <= 0) {
                    continue;
                }

                if ((roll -= calcValues[i].Weight) <= 0)
                {
                    return calcValues[i].Value;
                }
            }

            return calcValues.Last().Value;
        }

        public static T GetRandomElement<T>(this IEnumerable<T> list) {
            return !list.Any() ? default : list.ElementAt(Constants.Random.Next(0, list.Count()));
        }
    }
}
using MudSharp.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MudSharp.Framework.Revision;

#nullable enable
namespace MudSharp.Framework {
	public static class CollectionExtensions {
		public static (int Sum1, int Sum2) Sum2<T>(this IEnumerable<T> collection, Func<T,int> func1, Func<T,int> func2)
		{
			int sum1 = 0, sum2 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
			}

			return (sum1, sum2);
		}

		public static (int Sum1, int Sum2, int Sum3) Sum3<T>(this IEnumerable<T> collection, Func<T, int> func1, Func<T, int> func2, Func<T, int> func3)
		{
			int sum1 = 0, sum2 = 0, sum3 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
				sum3 += func3(item);
			}

			return (sum1, sum2, sum3);
		}

		public static (double Sum1, double Sum2) Sum2<T>(this IEnumerable<T> collection, Func<T, double> func1, Func<T, double> func2)
		{
			double sum1 = 0, sum2 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
			}

			return (sum1, sum2);
		}

		public static (double Sum1, double Sum2, double Sum3) Sum3<T>(this IEnumerable<T> collection, Func<T, double> func1, Func<T, double> func2, Func<T, double> func3)
		{
			double sum1 = 0, sum2 = 0, sum3 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
				sum3 += func3(item);
			}

			return (sum1, sum2, sum3);
		}

		public static (decimal Sum1, decimal Sum2) Sum2<T>(this IEnumerable<T> collection, Func<T, decimal> func1, Func<T, decimal> func2)
		{
			decimal sum1 = 0, sum2 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
			}

			return (sum1, sum2);
		}

		public static (decimal Sum1, decimal Sum2, decimal Sum3) Sum3<T>(this IEnumerable<T> collection, Func<T, decimal> func1, Func<T, decimal> func2, Func<T, decimal> func3)
		{
			decimal sum1 = 0, sum2 = 0, sum3 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
				sum3 += func3(item);
			}

			return (sum1, sum2, sum3);
		}

		public static (long Sum1, long Sum2) Sum2<T>(this IEnumerable<T> collection, Func<T, long> func1, Func<T, long> func2)
		{
			long sum1 = 0, sum2 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
			}

			return (sum1, sum2);
		}

		public static (long Sum1, long Sum2, long Sum3) Sum3<T>(this IEnumerable<T> collection, Func<T, long> func1, Func<T, long> func2, Func<T, long> func3)
		{
			long sum1 = 0, sum2 = 0, sum3 = 0;
			foreach (var item in collection)
			{
				sum1 += func1(item);
				sum2 += func2(item);
				sum3 += func3(item);
			}

			return (sum1, sum2, sum3);
		}

		public static (int Min, int Max) MinMax(this IEnumerable<int> collection)
		{
			var min = 0;
			var max = 0;
			foreach (var item in collection)
			{
				if (item < min)
				{
					min = item;
				}

				if (item > max)
				{
					max = item;
				}
			}

			return (min, max);
		}

		public static (int Min, int Max) MinMax(this IEnumerable<int> collection, Predicate<int> predicate)
		{
			var min = 0;
			var max = 0;
			foreach (var item in collection)
			{
				if (!predicate(item))
				{
					continue;
				}

				if (item < min)
				{
					min = item;
				}

				if (item > max)
				{
					max = item;
				}
			}

			return (min, max);
		}

		public static (double Min, double Max) MinMax(this IEnumerable<double> collection)
		{
			var min = 0.0;
			var max = 0.0;
			foreach (var item in collection)
			{
				if (item < min)
				{
					min = item;
				}

				if (item > max)
				{
					max = item;
				}
			}

			return (min, max);
		}

		public static (double Min, double Max) MinMax(this IEnumerable<double> collection, Predicate<double> predicate)
		{
			var min = 0.0;
			var max = 0.0;
			foreach (var item in collection)
			{
				if (!predicate(item))
				{
					continue;
				}

				if (item < min)
				{
					min = item;
				}

				if (item > max)
				{
					max = item;
				}
			}

			return (min, max);
		}

		public static (decimal Min, decimal Max) MinMax(this IEnumerable<decimal> collection)
		{
			var min = 0.0M;
			var max = 0.0M;
			foreach (var item in collection)
			{
				if (item < min)
				{
					min = item;
				}

				if (item > max)
				{
					max = item;
				}
			}

			return (min, max);
		}

		public static (decimal Min, decimal Max) MinMax(this IEnumerable<decimal> collection, Predicate<decimal> predicate)
		{
			var min = 0.0M;
			var max = 0.0M;
			foreach (var item in collection)
			{
				if (!predicate(item))
				{
					continue;
				}

				if (item < min)
				{
					min = item;
				}

				if (item > max)
				{
					max = item;
				}
			}

			return (min, max);
		}

		public static IEnumerable<T> ConcatIfNotNull<T>(this IEnumerable<T> source, T? item) {
			if (item == null) {
				return source;
			}

			return source.Append(item);
		}
		/// <summary>
		///     This function exludes a specified T from the source collection and returns the result
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <param name="source">The source collection</param>
		/// <param name="except">The value which is to be excluded from the collection</param>
		/// <returns>Returns all values in the IEnumerable that are not the specified T</returns>
		public static IEnumerable<T> Except<T>(this IEnumerable<T> source, T except) where T : notnull {
			foreach (var item in source)
			{
				if (item.Equals(except))
				{
					continue;
				}

				yield return item;
			}
		}

		/// <summary>
		///     This function exludes a specified U from the source collection of T and returns the result. Use this version when the collection is of a derived type of U.
		/// </summary>
		/// <typeparam name="T">Any type that is also Type U</typeparam>
		/// <typeparam name="U">Any type</typeparam>
		/// <param name="source">The source collection</param>
		/// <param name="except">The value which is to be excluded from the collection</param>
		/// <returns>Returns all values in the IEnumerable that are not the specified T</returns>
		public static IEnumerable<T> ExceptCovariant<T, U>(this IEnumerable<T> source, U except) where T : U
		{
			foreach (var item in source)
			{
				if (item is null)
				{
					continue;
				}

				if (item.Equals(except))
				{
					continue;
				}

				yield return item;
			}
		}

		/// <summary>
		///     Returns the original IEnumerable with the additional object appended to the end
		/// </summary>
		/// <typeparam name="T">Any object</typeparam>
		/// <param name="source">An Ienumerable of type T</param>
		/// <param name="plus">An object of type T to append to the list</param>
		/// <returns>The original list plus the new object</returns>
		public static IEnumerable<T> Plus<T>(this IEnumerable<T> source, T plus) {
			return source.Append(plus);
		}

		/// <summary>
		///     This method is the same as access the dictionary's index method, except that it returns the default value if none
		///     is found
		/// </summary>
		/// <typeparam name="TKey">The Key type of the dictionary</typeparam>
		/// <typeparam name="TValue">The Value type of the dictionary</typeparam>
		/// <typeparam name="U">Any type that is a Tkey</typeparam>
		/// <param name="source">The source dictionary</param>
		/// <param name="key">The key to retrieve</param>
		/// <param name="defaultVal">The default value to use if a key is not found</param>
		/// <returns>The matched value for the supplied key, or the default value for the TValue type</returns>
		public static TValue? ValueOrDefault<TKey, TValue, U>(this Dictionary<TKey, TValue> source, U key,
			TValue? defaultVal) where U : TKey where TKey : notnull {
			if (key is null)
			{
				return defaultVal;
			}
			return source.TryGetValue(key, out var val) ? val : defaultVal;
		}

		/// <summary>
		///     An extension method designed to find the first element where the supplied function yields a value identical to the
		///     maximum for the whole collection
		/// </summary>
		/// <typeparam name="T">Any Type</typeparam>
		/// <typeparam name="U">Any IComparable</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="maxfunc">
		///     A Function taking a parameter of T and returning a U, to be applied to the source to get the
		///     maximum U
		/// </param>
		/// <returns>The first element where the supplied function yields a value identical to the maximum for the whole collection</returns>
		public static T? FirstMax<T, U>(this IEnumerable<T> source, Func<T, U> maxfunc) where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return default;
			}
			var max = sourceList.Select(maxfunc).DefaultIfEmpty(default).Max();
			return sourceList.FirstOrDefault(x => maxfunc(x).Equals(max));
		}

		/// <summary>
		///     An extension method designed to find the first element where the supplied function yields a value identical to the
		///     maximum for the whole collection
		/// </summary>
		/// <typeparam name="T">Any Type</typeparam>
		/// <typeparam name="U">Any IComparable</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="maxfunc">
		///     A Function taking a parameter of T and returning a U, to be applied to the source to get the
		///     maximum U
		/// </param>
		/// <returns>The first element where the supplied function yields a value identical to the maximum for the whole collection</returns>
		public static T? FirstMax<T, U>(this IEnumerable<T> source, Func<T, U> maxfunc, U? defaultvalue)
			where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return default(T);
			}
			var max = sourceList.Select(maxfunc).DefaultIfEmpty(defaultvalue).Max();
			return sourceList.FirstOrDefault(x => maxfunc(x).Equals(max));
		}

		/// <summary>
		///     An extension method designed to find the first element where the supplied function yields a value identical to the
		///     minimum for the whole collection
		/// </summary>
		/// <typeparam name="T">Any Type</typeparam>
		/// <typeparam name="U">Any IComparable</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="minfunc">
		///     A Function taking a parameter of T and returning a U, to be applied to the source to get the
		///     minimum U
		/// </param>
		/// <returns>The first element where the supplied function yields a value identical to the minimum for the whole collection</returns>
		public static T? FirstMin<T, U>(this IEnumerable<T> source, Func<T, U> minfunc) where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return default(T);
			}
			var min = sourceList.Select(minfunc).DefaultIfEmpty(default(U)).Min();
			return sourceList.FirstOrDefault(x => minfunc(x).Equals(min));
		}

		/// <summary>
		///     An extension method designed to find the first element where the supplied function yields a value identical to the
		///     minimum for the whole collection
		/// </summary>
		/// <typeparam name="T">Any Type</typeparam>
		/// <typeparam name="U">Any IComparable</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="minfunc">
		///     A Function taking a parameter of T and returning a U, to be applied to the source to get the
		///     minimum U
		/// </param>
		/// <returns>The first element where the supplied function yields a value identical to the minimum for the whole collection</returns>
		public static T? FirstMin<T, U>(this IEnumerable<T> source, Func<T, U> minfunc, U? defaultvalue)
			where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return default(T);
			}
			var min = sourceList.Select(minfunc).DefaultIfEmpty(defaultvalue).Min();
			return sourceList.FirstOrDefault(x => minfunc(x).Equals(min));
		}

		/// <summary>
		///     An extension method designed to return all values of an IEnumerable where the result of the supplied function is
		///     the maximum for the whole collection
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <typeparam name="U">Any IComparable</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="maxfunc">
		///     A Function taking a parameter of T and returning a U, to be applied to the source to get the
		///     maximum U
		/// </param>
		/// <returns>
		///     An IEnumerable of Ts containing all elements whose result of maxfunc is equal to the maximum value of maxfunc
		///     for the whole collection
		/// </returns>
		public static IEnumerable<T> WhereMax<T, U>(this IEnumerable<T> source, Func<T, U> maxfunc)
			where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return sourceList;
			}
			var max = sourceList.Select(maxfunc).DefaultIfEmpty(default(U)).Max();
			return sourceList.Where(x => maxfunc(x).Equals(max));
		}

		/// <summary>
		///     An extension method designed to return all values of an IEnumerable where the result of the supplied function is
		///     the minimum for the whole collection
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <typeparam name="U">Any IComparable</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="minfunc">
		///     A Function taking a parameter of T and returning a U, to be applied to the source to get the
		///     minimum U
		/// </param>
		/// <returns>
		///     An IEnumerable of Ts containing all elements whose result of minfunc is equal to the minimum value of minfunc
		///     for the whole collection
		/// </returns>
		public static IEnumerable<T> WhereMin<T, U>(this IEnumerable<T> source, Func<T, U> minfunc)
			where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return sourceList;
			}
			var min = sourceList.Select(minfunc).DefaultIfEmpty(default(U)).Min();
			return sourceList.Where(x => minfunc(x).Equals(min));
		}

		/// <summary>
		///     An extension method that evaluates a function and returns an IEnumerable containing all Ts that did not evaluate
		///     that function to null
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <typeparam name="U">Any Class</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="selectfunc">
		///     A function taking a parameter of T and returning a U, the result of which will be compared to
		///     null
		/// </param>
		/// <returns>An IEnumerable of Ts containing all elements that did not return null from selectfunc</returns>
		public static IEnumerable<T> WhereNotNull<T, U>(this IEnumerable<T?> source, Func<T?, U?> selectfunc)
			where U : class {
			foreach (var item in source)
			{
				var value = selectfunc(item);
				if (item is not null && value is not null)
				{
					yield return item;
				}
			}
		}

		/// <summary>
		/// Applies a filter function across an enumerable and counts true results. Returns true if either the specified count is reached or all elements return true
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <param name="source">The enumerable to evaluate</param>
		/// <param name="filter">The filter function to apply to elements</param>
		/// <param name="minCount">The minimum count being targeted</param>
		/// <returns>True if either the minimum count is met or all elements meet the filter</returns>
		public static bool MinCountOrAll<T>(this IEnumerable<T> source, Func<T, bool> filter, int minCount)
		{
			var anyMisses = false;
			foreach (var item in source)
			{
				if (filter(item))
				{
					if (--minCount == 0)
					{
						return true;
					}

					continue;
				}

				anyMisses = true;
			}

			return !anyMisses;
		}

		/// <summary>
		///     Determines whether the value type is the default value for its type
		/// </summary>
		/// <typeparam name="T">Any struct</typeparam>
		/// <param name="value">The value to test</param>
		/// <returns>True if value is the default for type</returns>
		public static bool IsDefault<T>(this T value) where T : struct {
			var isDefault = value.Equals(default(T));

			return isDefault;
		}

		/// <summary>
		///     An extension method that evaluates a function in a LINQ select query and returns all elements that did not return
		///     null
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <typeparam name="U">Any Class</typeparam>
		/// <param name="source">An IEnumerable of Ts on which to perform the operation</param>
		/// <param name="selectfunc">A function taking a parameter of T and returning a U, the result of which will be selected</param>
		/// <returns>An IEnumerable of Us containing the results of all non-null evaluations of selectfunc</returns>
		public static IEnumerable<U> SelectNotNull<T, U>(this IEnumerable<T?> source, Func<T?, U?> selectfunc) {
			foreach (var item in source)
			{
				var value = selectfunc(item);
				if (item is not null && value is not null)
				{
					yield return value;
				}
			}
		}

		/// <summary>
		///     Swaps two elements in a list. While it supports fluent syntax use, it has side effects for the source list.
		/// </summary>
		/// <typeparam name="T">The type of the parameters</typeparam>
		/// <param name="list">The list containing the items to swap</param>
		/// <param name="indexA">The index of the first item to swap</param>
		/// <param name="indexB">The index of the second item to swap</param>
		/// <returns>A reference to the list</returns>
		public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB) {
			(list[indexA], list[indexB]) = (list[indexB], list[indexA]);
			return list;
		}

		/// <summary>
		///     Swaps two elements in a list
		/// </summary>
		/// <typeparam name="T">Any reference type</typeparam>
		/// <param name="list">The list containing the items to swap</param>
		/// <param name="itemA">The first item to swap</param>
		/// <param name="itemB">The second item to swap</param>
		/// <returns>A reference to the list</returns>
		public static IList<T> Swap<T>(this IList<T> list, T itemA, T itemB) {
			var indexA = list.IndexOf(itemA);
			var indexB = list.IndexOf(itemB);
			list[indexA] = itemB;
			list[indexB] = itemA;
			return list;
		}

		public static IEnumerable<IEnumerable<T>> FindSequenceConsecutive<T>(this IEnumerable<T> sequence,
			Predicate<T> predicate, int sequenceSize) {
			var list = sequence.ToList();
			var matchList = list.Select(x => predicate(x)).ToList();

			var start = 0;
			var count = list.Count;

			while (start + sequenceSize <= count) {
				var range = matchList.GetRange(start, sequenceSize);
				if (range.All(x => x)) {
					yield return list.GetRange(start, sequenceSize);
				}

				start++;
			}
		}

		/// <summary>
		/// Given a list of type T and an item of type T, adds the item to the list if it is not null (otherwise noop).
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <param name="list">A list of type T</param>
		/// <param name="item">Any item of type T</param>
		public static void AddNotNull<T>(this List<T> list, T item)
		{
			if (item != null)
			{
				list.Add(item);
			}
		}

		public static void ApplyActionToAdjacents<T>(this T[,] array, int xCoord, int yCoord, Action<T> action)
		{
			var width = array.GetLength(0);
			var height = array.GetLength(1);
			
			if (xCoord > 0)
			{
				action(array[xCoord - 1, yCoord]); // West
				if (yCoord > 0)
				{
					action(array[xCoord - 1, yCoord - 1]); // SouthWest
				}
				if (yCoord < height - 1)
				{
					action(array[xCoord - 1, yCoord + 1]); // NorthWest
				}
			}
			
			if (xCoord < width - 1)
			{
				action(array[xCoord + 1, yCoord]); // East
				if (yCoord > 0)
				{
					action(array[xCoord + 1, yCoord - 1]); // SouthEast
				}
				if (yCoord < height - 1)
				{
					action(array[xCoord + 1, yCoord + 1]); // NorthEast
				}
			}

			if (yCoord > 0)
			{
				action(array[xCoord, yCoord-1]); // South
			}

			if (yCoord < height - 1)
			{
				action(array[xCoord, yCoord+1]); // North
			}
		}

		public static void ApplyActionToAdjacentsWithInfo<T>(this T[,] array, int xCoord, int yCoord, Action<T, CardinalDirection, int, int> action)
		{
			var width = array.GetLength(0);
			var height = array.GetLength(1);

			if (xCoord > 0)
			{
				action(array[xCoord - 1, yCoord], CardinalDirection.West, xCoord - 1, yCoord); // West
				if (yCoord > 0)
				{
					action(array[xCoord - 1, yCoord - 1], CardinalDirection.SouthWest, xCoord - 1, yCoord - 1); // SouthWest
				}
				if (yCoord < height - 1)
				{
					action(array[xCoord - 1, yCoord + 1], CardinalDirection.NorthWest, xCoord - 1, yCoord + 1); // NorthWest
				}
			}

			if (xCoord < width - 1)
			{
				action(array[xCoord + 1, yCoord], CardinalDirection.East, xCoord + 1, yCoord); // East
				if (yCoord > 0)
				{
					action(array[xCoord + 1, yCoord - 1], CardinalDirection.SouthEast, xCoord + 1, yCoord - 1); // SouthEast
				}
				if (yCoord < height - 1)
				{
					action(array[xCoord + 1, yCoord + 1], CardinalDirection.NorthEast, xCoord + 1, yCoord + 1); // NorthEast
				}
			}

			if (yCoord > 0)
			{
				action(array[xCoord, yCoord - 1], CardinalDirection.South, xCoord, yCoord - 1); // South
			}

			if (yCoord < height - 1)
			{
				action(array[xCoord, yCoord + 1], CardinalDirection.North, xCoord, yCoord + 1); // North
			}
		}

		public static void ApplyActionToAdjacentsWithDirection<T>(this T[,] array, int xCoord, int yCoord, Action<T, CardinalDirection> action)
		{
			var width = array.GetLength(0);
			var height = array.GetLength(1);

			if (xCoord > 0)
			{
				action(array[xCoord - 1, yCoord], CardinalDirection.West); // West
				if (yCoord > 0)
				{
					action(array[xCoord - 1, yCoord - 1], CardinalDirection.SouthWest); // SouthWest
				}
				if (yCoord < height - 1)
				{
					action(array[xCoord - 1, yCoord + 1], CardinalDirection.NorthWest); // NorthWest
				}
			}

			if (xCoord < width - 1)
			{
				action(array[xCoord + 1, yCoord], CardinalDirection.East); // East
				if (yCoord > 0)
				{
					action(array[xCoord + 1, yCoord - 1], CardinalDirection.SouthEast); // SouthEast
				}
				if (yCoord < height - 1)
				{
					action(array[xCoord + 1, yCoord + 1], CardinalDirection.NorthEast); // NorthEast
				}
			}

			if (yCoord > 0)
			{
				action(array[xCoord, yCoord - 1], CardinalDirection.South); // South
			}

			if (yCoord < height - 1)
			{
				action(array[xCoord, yCoord + 1], CardinalDirection.North); // North
			}
		}

		public static int ApplyFunctionToAdjacentsReturnCount<T>(this T[,] array, int xCoord, int yCoord, Func<T,bool> func)
		{
			var width = array.GetLength(0);
			var height = array.GetLength(1);
			var count = 0;

			if (xCoord > 0)
			{
				count += func(array[xCoord - 1, yCoord]) ? 1 : 0; // West
				if (yCoord > 0)
				{
					count += func(array[xCoord - 1, yCoord - 1]) ? 1 : 0; // SouthWest
				}
				if (yCoord < height - 1)
				{
					count += func(array[xCoord - 1, yCoord + 1]) ? 1 : 0; // NorthWest
				}
			}

			if (xCoord < width - 1)
			{
				count += func(array[xCoord + 1, yCoord]) ? 1 : 0; // East
				if (yCoord > 0)
				{
					count += func(array[xCoord + 1, yCoord - 1]) ? 1 : 0; // SouthEast
				}
				if (yCoord < height - 1)
				{
					count += func(array[xCoord + 1, yCoord + 1]) ? 1 : 0; // NorthEast
				}
			}

			if (yCoord > 0)
			{
				count += func(array[xCoord, yCoord - 1]) ? 1 : 0; // South
			}

			if (yCoord < height - 1)
			{
				count += func(array[xCoord, yCoord + 1]) ? 1 : 0; // North
			}

			return count;
		}

		public static int ApplyFunctionToAdjacentsReturnCountWithDirection<T>(this T[,] array, int xCoord, int yCoord, Func<T, CardinalDirection, bool> func)
		{
			var width = array.GetLength(0);
			var height = array.GetLength(1);
			var count = 0;

			if (xCoord > 0)
			{
				count += func(array[xCoord - 1, yCoord], CardinalDirection.West) ? 1 : 0; // West
				if (yCoord > 0)
				{
					count += func(array[xCoord - 1, yCoord - 1], CardinalDirection.SouthWest) ? 1 : 0; // SouthWest
				}
				if (yCoord < height - 1)
				{
					count += func(array[xCoord - 1, yCoord + 1], CardinalDirection.NorthWest) ? 1 : 0; // NorthWest
				}
			}

			if (xCoord < width - 1)
			{
				count += func(array[xCoord + 1, yCoord], CardinalDirection.East) ? 1 : 0; // East
				if (yCoord > 0)
				{
					count += func(array[xCoord + 1, yCoord - 1], CardinalDirection.SouthEast) ? 1 : 0; // SouthEast
				}
				if (yCoord < height - 1)
				{
					count += func(array[xCoord + 1, yCoord + 1], CardinalDirection.NorthEast) ? 1 : 0; // NorthEast
				}
			}

			if (yCoord > 0)
			{
				count += func(array[xCoord, yCoord - 1], CardinalDirection.South) ? 1 : 0; // South
			}

			if (yCoord < height - 1)
			{
				count += func(array[xCoord, yCoord + 1], CardinalDirection.North) ? 1 : 0; // North
			}

			return count;
		}

		public static (int X, int Y) GetCoordsOfElement<T>(this T[,] array, T element)
		{
			for (int i = 0; i < array.GetLength(0); i++)
			{
				for (int j = 0; j < array.GetLength(1); j++)
				{
					if (!Equals(array[i,j],element))
					{
						continue;
					}

					return (i, j);
				}
			}

			throw new InvalidOperationException("GetCoordsOfElement<T> function was asked to find an element that wasn't in the array.");
		}

#nullable enable
		public static T? GetByNameOrAbbreviation<T>(this IEnumerable<T> items, string targetText) where T : IFrameworkItem
		{
			var itemList = items.ToList();
			return itemList.FirstOrDefault(x => x.Name.EqualTo(targetText)) ??
				   itemList.FirstOrDefault(x => x.Name.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase));
		}

		public static T? Get<T>(this IEnumerable<T> items, long id) where T : IFrameworkItem
		{
			return items.FirstOrDefault(x => x.Id == id);
		}

		public static T? GetByIdOrName<T>(this IEnumerable<T> items, string text) where T : IFrameworkItem
		{
			if (long.TryParse(text, out var id))
			{
				return items.FirstOrDefault(x => x.Id == id);
			}

			var itemList = items.ToList();
			return itemList.FirstOrDefault(x => x.Name.EqualTo(text)) ??
				   itemList.FirstOrDefault(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
		}

		public static T? GetByRevisableId<T>(this IEnumerable<T> items, string text) where T : IRevisableItem
		{
			if (long.TryParse(text, out var id))
			{
				var ids = items.Where(x => x.Id == id).ToList();
				return
					ids.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
					ids.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision || x.Status == RevisionStatus.UnderDesign) ??
					ids.FirstMax(x => x.RevisionNumber);
			}

			return default;
		}

		public static T? GetById<T>(this IEnumerable<T> items, string text) where T : IFrameworkItem
		{
			if (long.TryParse(text, out var id))
			{
				return items.FirstOrDefault(x => x.Id == id);
			}

			return default;
		}

		public static T? GetByIdOrOrder<T>(this IEnumerable<T> items, string text) where T : IFrameworkItem
		{
			if (long.TryParse(text, out var id))
			{
				return items.FirstOrDefault(x => x.Id == id);
			}

			if (text.Length > 1 && text[0] == '#' && int.TryParse(text.Substring(1), out var index))
			{
				return items.ElementAtOrDefault(index - 1);
			}

			return default;
		}

		/// <summary>
		/// Returns the first Ordinal Position (i.e. 1-initial index) of an item in a collection
		/// </summary>
		/// <typeparam name="T">Any reference type</typeparam>
		/// <param name="items">The collection to search</param>
		/// <param name="search">The item to search for</param>
		/// <returns>The 1-initial index (i.e. ordinal position) of the item, or 0 if not found</returns>
		public static int OrdinalPositionOf<T>(this IEnumerable<T> items, T search) where T : class
		{
			var i = 0;
			foreach (var item in items)
			{
				i++;
				if (search == item)
				{
					return i;
				}
			}

			return 0;
		}
	}
}
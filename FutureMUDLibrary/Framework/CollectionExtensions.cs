using MudSharp.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MudSharp.Framework.Revision;

#nullable enable
namespace MudSharp.Framework {
	public static class CollectionExtensions {

		/// <summary>
		/// Removes all entries from the dictionary whose keys match the predicate.
		/// </summary>
		/// <typeparam name="T">The dictionary type.</typeparam>
		/// <typeparam name="TKey">The key type.</typeparam>
		/// <typeparam name="TValue">The value type.</typeparam>
		/// <param name="dictionary">The dictionary to modify.</param>
		/// <param name="predicate">A predicate that selects keys to remove.</param>
		/// <returns>This method does not return a value.</returns>
		public static void RemoveAllKeys<T, TKey, TValue>(this T dictionary, Predicate<TKey> predicate) where T : IDictionary<TKey, TValue>
		{
			foreach (var key in dictionary.Keys.ToArray())
			{
				if (predicate(key))
				{
					dictionary.Remove(key);
				}
			}
		}

		/// <summary>
		/// Calculates two integer sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <returns>A tuple containing Sum1 and Sum2.</returns>
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

		/// <summary>
		/// Calculates three integer sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <param name="func3">The selector for the third sum.</param>
		/// <returns>A tuple containing Sum1, Sum2, and Sum3.</returns>
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

		/// <summary>
		/// Calculates two double sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <returns>A tuple containing Sum1 and Sum2.</returns>
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

		/// <summary>
		/// Calculates three double sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <param name="func3">The selector for the third sum.</param>
		/// <returns>A tuple containing Sum1, Sum2, and Sum3.</returns>
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

		/// <summary>
		/// Calculates two decimal sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <returns>A tuple containing Sum1 and Sum2.</returns>
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

		/// <summary>
		/// Calculates three decimal sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <param name="func3">The selector for the third sum.</param>
		/// <returns>A tuple containing Sum1, Sum2, and Sum3.</returns>
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

		/// <summary>
		/// Calculates two long sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <returns>A tuple containing Sum1 and Sum2.</returns>
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

		/// <summary>
		/// Calculates three long sums in a single pass over the collection.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The source collection.</param>
		/// <param name="func1">The selector for the first sum.</param>
		/// <param name="func2">The selector for the second sum.</param>
		/// <param name="func3">The selector for the third sum.</param>
		/// <returns>A tuple containing Sum1, Sum2, and Sum3.</returns>
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

		/// <summary>
		/// Returns the minimum and maximum values in the collection.
		/// </summary>
		/// <param name="collection">The source collection.</param>
		/// <returns>A tuple containing the minimum and maximum values, or (0, 0) if the collection is empty.</returns>
		public static (int Min, int Max) MinMax(this IEnumerable<int> collection)
		{
			var hasValue = false;
			var min = 0;
			var max = 0;
			foreach (var item in collection)
			{
				if (!hasValue)
				{
					min = item;
					max = item;
					hasValue = true;
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

		/// <summary>
		/// Returns the minimum and maximum values in the collection that match the predicate.
		/// </summary>
		/// <param name="collection">The source collection.</param>
		/// <param name="predicate">The predicate used to filter elements.</param>
		/// <returns>A tuple containing the minimum and maximum values among matches, or (0, 0) if none match.</returns>
		public static (int Min, int Max) MinMax(this IEnumerable<int> collection, Predicate<int> predicate)
		{
			var hasValue = false;
			var min = 0;
			var max = 0;
			foreach (var item in collection)
			{
				if (!predicate(item))
				{
					continue;
				}

				if (!hasValue)
				{
					min = item;
					max = item;
					hasValue = true;
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

		/// <summary>
		/// Returns the minimum and maximum values in the collection.
		/// </summary>
		/// <param name="collection">The source collection.</param>
		/// <returns>A tuple containing the minimum and maximum values, or (0.0, 0.0) if the collection is empty.</returns>
		public static (double Min, double Max) MinMax(this IEnumerable<double> collection)
		{
			var hasValue = false;
			var min = 0.0;
			var max = 0.0;
			foreach (var item in collection)
			{
				if (!hasValue)
				{
					min = item;
					max = item;
					hasValue = true;
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

		/// <summary>
		/// Returns the minimum and maximum values in the collection that match the predicate.
		/// </summary>
		/// <param name="collection">The source collection.</param>
		/// <param name="predicate">The predicate used to filter elements.</param>
		/// <returns>A tuple containing the minimum and maximum values among matches, or (0.0, 0.0) if none match.</returns>
		public static (double Min, double Max) MinMax(this IEnumerable<double> collection, Predicate<double> predicate)
		{
			var hasValue = false;
			var min = 0.0;
			var max = 0.0;
			foreach (var item in collection)
			{
				if (!predicate(item))
				{
					continue;
				}

				if (!hasValue)
				{
					min = item;
					max = item;
					hasValue = true;
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

		/// <summary>
		/// Returns the minimum and maximum values in the collection.
		/// </summary>
		/// <param name="collection">The source collection.</param>
		/// <returns>A tuple containing the minimum and maximum values, or (0.0M, 0.0M) if the collection is empty.</returns>
		public static (decimal Min, decimal Max) MinMax(this IEnumerable<decimal> collection)
		{
			var hasValue = false;
			var min = 0.0M;
			var max = 0.0M;
			foreach (var item in collection)
			{
				if (!hasValue)
				{
					min = item;
					max = item;
					hasValue = true;
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

		/// <summary>
		/// Returns the minimum and maximum values in the collection that match the predicate.
		/// </summary>
		/// <param name="collection">The source collection.</param>
		/// <param name="predicate">The predicate used to filter elements.</param>
		/// <returns>A tuple containing the minimum and maximum values among matches, or (0.0M, 0.0M) if none match.</returns>
		public static (decimal Min, decimal Max) MinMax(this IEnumerable<decimal> collection, Predicate<decimal> predicate)
		{
			var hasValue = false;
			var min = 0.0M;
			var max = 0.0M;
			foreach (var item in collection)
			{
				if (!predicate(item))
				{
					continue;
				}

				if (!hasValue)
				{
					min = item;
					max = item;
					hasValue = true;
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

		/// <summary>
		/// Appends an item to the sequence when the item is not null.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="item">The item to append if it is not null.</param>
		/// <returns>The original sequence if item is null; otherwise the sequence with item appended.</returns>
		public static IEnumerable<T> ConcatIfNotNull<T>(this IEnumerable<T> source, T? item) {
			if (item == null) {
				return source;
			}

			return source.Append(item);
		}

		/// <summary>
		/// Excludes a specified value from the source collection.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <param name="source">The source collection.</param>
		/// <param name="except">The value to exclude from the collection.</param>
		/// <returns>All values in the sequence that are not the excluded value.</returns>
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
		/// Excludes a specified value from a covariant sequence, skipping null elements.
		/// </summary>
		/// <typeparam name="T">Any type that is also type U.</typeparam>
		/// <typeparam name="U">Any type.</typeparam>
		/// <param name="source">The source collection.</param>
		/// <param name="except">The value to exclude from the collection.</param>
		/// <returns>All values in the sequence that are not the excluded value.</returns>
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
		/// Returns the sequence with an additional item appended to the end.
		/// </summary>
		/// <typeparam name="T">Any object.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="plus">The item to append to the sequence.</param>
		/// <returns>The original sequence plus the new item.</returns>
		public static IEnumerable<T> Plus<T>(this IEnumerable<T> source, T plus) {
			return source.Append(plus);
		}

		/// <summary>
		/// Returns the value for a dictionary key, or a supplied default when the key is missing or null.
		/// </summary>
		/// <typeparam name="TKey">The key type of the dictionary.</typeparam>
		/// <typeparam name="TValue">The value type of the dictionary.</typeparam>
		/// <typeparam name="U">Any type that is a TKey.</typeparam>
		/// <param name="source">The source dictionary.</param>
		/// <param name="key">The key to retrieve.</param>
		/// <param name="defaultVal">The default value to use if a key is not found.</param>
		/// <returns>The matched value for the supplied key, or the provided default value.</returns>
		public static TValue? ValueOrDefault<TKey, TValue, U>(this Dictionary<TKey, TValue> source, U key,
			TValue? defaultVal) where U : TKey where TKey : notnull {
			if (key is null)
			{
				return defaultVal;
			}
			return source.TryGetValue(key, out var val) ? val : defaultVal;
		}

		/// <summary>
		/// Returns the first element whose selector value equals the maximum for the collection.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any IComparable type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="maxfunc">A selector used to compute the maximum value.</param>
		/// <returns>The first element with the maximum selector value, or default if the sequence is empty.</returns>
		public static T? FirstMax<T, U>(this IEnumerable<T> source, Func<T, U> maxfunc) where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return default;
			}
			var max = sourceList.Select(maxfunc).DefaultIfEmpty(default).Max();
			return sourceList.FirstOrDefault(x => maxfunc(x).Equals(max));
		}

		/// <summary>
		/// Returns the first element whose selector value equals the maximum for the collection, using a fallback value for comparison.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any IComparable type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="maxfunc">A selector used to compute the maximum value.</param>
		/// <param name="defaultvalue">A fallback value used when computing the maximum.</param>
		/// <returns>The first element with the maximum selector value, or default if the sequence is empty.</returns>
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
		/// Returns the first element whose selector value equals the minimum for the collection.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any IComparable type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="minfunc">A selector used to compute the minimum value.</param>
		/// <returns>The first element with the minimum selector value, or default if the sequence is empty.</returns>
		public static T? FirstMin<T, U>(this IEnumerable<T> source, Func<T, U> minfunc) where U : IComparable {
			var sourceList = source.ToList();
			if (!sourceList.Any()) {
				return default(T);
			}
			var min = sourceList.Select(minfunc).DefaultIfEmpty(default(U)).Min();
			return sourceList.FirstOrDefault(x => minfunc(x).Equals(min));
		}

		/// <summary>
		/// Returns the first element whose selector value equals the minimum for the collection, using a fallback value for comparison.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any IComparable type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="minfunc">A selector used to compute the minimum value.</param>
		/// <param name="defaultvalue">A fallback value used when computing the minimum.</param>
		/// <returns>The first element with the minimum selector value, or default if the sequence is empty.</returns>
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
		/// Returns all elements whose selector value equals the maximum for the collection.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any IComparable type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="maxfunc">A selector used to compute the maximum value.</param>
		/// <returns>All elements that share the maximum selector value, or an empty sequence if the source is empty.</returns>
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
		/// Returns all elements whose selector value equals the minimum for the collection.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any IComparable type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="minfunc">A selector used to compute the minimum value.</param>
		/// <returns>All elements that share the minimum selector value, or an empty sequence if the source is empty.</returns>
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
		/// Returns all elements where the selector result is non-null.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any class type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="selectfunc">A selector that returns a value tested for null.</param>
		/// <returns>An IEnumerable of elements whose selector result is non-null.</returns>
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
		/// Counts elements matching the filter and returns true when the minimum count is reached or when all elements match.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <param name="source">The enumerable to evaluate.</param>
		/// <param name="filter">The filter function to apply to elements.</param>
		/// <param name="minCount">The minimum count being targeted.</param>
		/// <returns>True if either the minimum count is met or all elements meet the filter.</returns>
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
		/// Determines whether the value equals the default for its type.
		/// </summary>
		/// <typeparam name="T">Any struct.</typeparam>
		/// <param name="value">The value to test.</param>
		/// <returns>True if value is the default for the type.</returns>
		public static bool IsDefault<T>(this T value) where T : struct {
			var isDefault = value.Equals(default(T));

			return isDefault;
		}

		/// <summary>
		/// Returns all non-null selector results from the source sequence.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <typeparam name="U">Any class type.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <param name="selectfunc">A selector that returns a value tested for null.</param>
		/// <returns>An IEnumerable of selector results that are not null.</returns>
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
		/// Swaps two elements in a list by index and returns the list.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="list">The list containing the items to swap.</param>
		/// <param name="indexA">The index of the first item to swap.</param>
		/// <param name="indexB">The index of the second item to swap.</param>
		/// <returns>A reference to the list.</returns>
		public static IList<T> SwapByIndex<T>(this IList<T> list, int indexA, int indexB) {
			(list[indexA], list[indexB]) = (list[indexB], list[indexA]);
			return list;
		}

		/// <summary>
		/// Swaps two elements in a list by value and returns the list.
		/// </summary>
		/// <typeparam name="T">Any reference type.</typeparam>
		/// <param name="list">The list containing the items to swap.</param>
		/// <param name="itemA">The first item to swap.</param>
		/// <param name="itemB">The second item to swap.</param>
		/// <returns>A reference to the list.</returns>
		public static IList<T> Swap<T>(this IList<T> list, T itemA, T itemB) {
			var indexA = list.IndexOf(itemA);
			var indexB = list.IndexOf(itemB);
			list[indexA] = itemB;
			list[indexB] = itemA;
			return list;
		}

		/// <summary>
		/// Finds contiguous subsequences of a fixed size where all elements match the predicate.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="sequence">The source sequence.</param>
		/// <param name="predicate">The predicate used to test each element.</param>
		/// <param name="sequenceSize">The size of each contiguous sequence to return.</param>
		/// <returns>All matching contiguous subsequences of the requested size.</returns>
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
		/// Adds an item to the list when it is not null.
		/// </summary>
		/// <typeparam name="T">Any type.</typeparam>
		/// <param name="list">The list to modify.</param>
		/// <param name="item">The item to add if not null.</param>
		/// <returns>This method does not return a value.</returns>
		public static void AddNotNull<T>(this List<T> list, T item)
		{
			if (item != null)
			{
				list.Add(item);
			}
		}

		/// <summary>
		/// Applies an action to all adjacent cells (including diagonals) around the specified coordinates.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="array">The 2D array to inspect.</param>
		/// <param name="xCoord">The X coordinate of the center cell.</param>
		/// <param name="yCoord">The Y coordinate of the center cell.</param>
		/// <param name="action">The action to apply to each adjacent element.</param>
		/// <returns>This method does not return a value.</returns>
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

		/// <summary>
		/// Applies an action to all adjacent cells (including diagonals) around the specified coordinates, supplying direction and coordinates.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="array">The 2D array to inspect.</param>
		/// <param name="xCoord">The X coordinate of the center cell.</param>
		/// <param name="yCoord">The Y coordinate of the center cell.</param>
		/// <param name="action">The action to apply to each adjacent element with direction and coordinates.</param>
		/// <returns>This method does not return a value.</returns>
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

		/// <summary>
		/// Applies an action to all adjacent cells (including diagonals) around the specified coordinates, supplying direction.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="array">The 2D array to inspect.</param>
		/// <param name="xCoord">The X coordinate of the center cell.</param>
		/// <param name="yCoord">The Y coordinate of the center cell.</param>
		/// <param name="action">The action to apply to each adjacent element with direction.</param>
		/// <returns>This method does not return a value.</returns>
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

		/// <summary>
		/// Applies a function to all adjacent cells (including diagonals) and returns the count of true results.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="array">The 2D array to inspect.</param>
		/// <param name="xCoord">The X coordinate of the center cell.</param>
		/// <param name="yCoord">The Y coordinate of the center cell.</param>
		/// <param name="func">The function to evaluate on each adjacent element.</param>
		/// <returns>The count of adjacent elements for which the function returned true.</returns>
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

		/// <summary>
		/// Applies a function to all adjacent cells (including diagonals) with direction and returns the count of true results.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="array">The 2D array to inspect.</param>
		/// <param name="xCoord">The X coordinate of the center cell.</param>
		/// <param name="yCoord">The Y coordinate of the center cell.</param>
		/// <param name="func">The function to evaluate on each adjacent element with direction.</param>
		/// <returns>The count of adjacent elements for which the function returned true.</returns>
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

		/// <summary>
		/// Returns the coordinates of the first matching element in a 2D array.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="array">The 2D array to search.</param>
		/// <param name="element">The element to locate.</param>
		/// <returns>A tuple containing the X and Y coordinates of the element.</returns>
		/// <exception cref="InvalidOperationException">Thrown when the element is not found.</exception>
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
		/// <summary>
		/// Gets an item by exact name match or by a name prefix match.
		/// </summary>
		/// <typeparam name="T">Any framework item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="targetText">The name or prefix to match.</param>
		/// <returns>The first matching item, or null if none match.</returns>
		public static T? GetByNameOrAbbreviation<T>(this IEnumerable<T> items, string targetText) where T : IFrameworkItem
		{
			var itemList = items.ToList();
			return itemList.FirstOrDefault(x => x.Name.EqualTo(targetText)) ??
				   itemList.FirstOrDefault(x => x.Name.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		/// Gets the first item with the specified id.
		/// </summary>
		/// <typeparam name="T">Any framework item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="id">The id to match.</param>
		/// <returns>The first matching item, or null if none match.</returns>
		public static T? Get<T>(this IEnumerable<T> items, long id) where T : IFrameworkItem
		{
			return items.FirstOrDefault(x => x.Id == id);
		}

		/// <summary>
		/// Gets an item by numeric id or by exact name match or name prefix match.
		/// </summary>
		/// <typeparam name="T">Any framework item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="text">The id or name text to match.</param>
		/// <returns>The first matching item, or null if none match.</returns>
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

		/// <summary>
		/// Gets a revisable item by id or name with editing-friendly priority rules.
		/// </summary>
		/// <typeparam name="T">Any revisable item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="text">The id or name text to match.</param>
		/// <returns>
		/// The matching item, preferring pending or under-design revisions, then current, then highest revision number.
		/// </returns>
		public static T? GetByIdOrNameRevisableForEditing<T>(this IEnumerable<T> items, string text) where T : IRevisableItem
		{
			List<T> filteredItems;
			if (long.TryParse(text, out var id))
			{
				filteredItems = items.Where(x => x.Id == id).ToList();
			}
			else
			{
				filteredItems = items
				                .Where(x => x.Name.EqualTo(text))
				                .ToList();
				if (filteredItems.Count == 0)
				{
					filteredItems = items
					                .Where(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
					                .ToList();
					if (filteredItems.Count == 0)
					{
						filteredItems = items
						                .Where(x => x.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase))
						                .ToList();
					}
				}
			}

			return
				filteredItems.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision || x.Status == RevisionStatus.UnderDesign) ??
				filteredItems.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
				filteredItems.FirstMax(x => x.RevisionNumber);
		}

		/// <summary>
		/// Gets a revisable item by id or name with current-first priority rules.
		/// </summary>
		/// <typeparam name="T">Any revisable item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="text">The id or name text to match.</param>
		/// <returns>
		/// The matching item, preferring current revisions, then pending or under-design, then highest revision number.
		/// </returns>
		public static T? GetByIdOrNameRevisable<T>(this IEnumerable<T> items, string text) where T : IRevisableItem
		{
			List<T> filteredItems;
			if (long.TryParse(text, out var id))
			{
				filteredItems = items.Where(x => x.Id == id).ToList();
			}
			else
			{
				filteredItems = items
				                .Where(x => x.Name.EqualTo(text))
				                .ToList();
				if (filteredItems.Count == 0)
				{
					filteredItems = items
					                .Where(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
					                .ToList();
					if (filteredItems.Count == 0)
					{
						filteredItems = items
						                .Where(x => x.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase))
						                .ToList();
					}
				}
			}

			return
				filteredItems.FirstOrDefault(x => x.Status == RevisionStatus.Current) ??
				filteredItems.FirstOrDefault(x => x.Status == RevisionStatus.PendingRevision || x.Status == RevisionStatus.UnderDesign) ??
				filteredItems.FirstMax(x => x.RevisionNumber);
		}

		/// <summary>
		/// Gets a revisable item by numeric id with current-first priority rules.
		/// </summary>
		/// <typeparam name="T">Any revisable item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="text">The id text to match.</param>
		/// <returns>The matching item, or null if the id is invalid or no items match.</returns>
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

		/// <summary>
		/// Gets an item by numeric id.
		/// </summary>
		/// <typeparam name="T">Any framework item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="text">The id text to match.</param>
		/// <returns>The matching item, or null if the id is invalid or no items match.</returns>
		public static T? GetById<T>(this IEnumerable<T> items, string text) where T : IFrameworkItem
		{
			if (long.TryParse(text, out var id))
			{
				return items.FirstOrDefault(x => x.Id == id);
			}

			return default;
		}

		/// <summary>
		/// Gets an item by numeric id or by ordinal order specified as #n (1-based).
		/// </summary>
		/// <typeparam name="T">Any framework item type.</typeparam>
		/// <param name="items">The collection to search.</param>
		/// <param name="text">The id text or ordinal order text to match.</param>
		/// <returns>The matching item, or null if the text is invalid or no items match.</returns>
		public static T? GetByIdOrOrder<T>(this IEnumerable<T> items, string text) where T : IFrameworkItem
		{
			if (long.TryParse(text, out var id))
			{
				return items.FirstOrDefault(x => x.Id == id);
			}

			if (text.Length > 1 && text[0] == '#' && int.TryParse(text.Substring(1), out var index) && index > 0)
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

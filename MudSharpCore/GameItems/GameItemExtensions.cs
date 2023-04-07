using System;
using System.Collections.Generic;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems;

public static class GameItemExtensions
{
	public static IEnumerable<T> RecursiveGetItems<T>(this IGameItem item,
		bool respectGetItemRules = false) where T : IGameItemComponent
	{
		return new[] { item }.RecursiveGetItems<T>(respectGetItemRules);
	}

	public static IEnumerable<T> RecursiveGetItems<T>(this IEnumerable<IGameItem> items,
		bool respectGetItemRules = false) where T : IGameItemComponent
	{
		var result = new List<T>();
		foreach (var item in items)
		{
			if (item.IsItemType<T>())
			{
				result.Add(item.GetItemType<T>());
			}

			var container = item.GetItemType<IContainer>();
			if (container == null)
			{
				continue;
			}

			if (respectGetItemRules && item.IsItemType<IOpenable>() && !item.GetItemType<IOpenable>().IsOpen)
			{
				continue;
			}

			result.AddRange(container.Contents.RecursiveGetItems<T>(respectGetItemRules));
		}

		return result;
	}

	public static string Describe(this OutfitExclusivity item)
	{
		switch (item)
		{
			case OutfitExclusivity.NonExclusive:
				return "Non Exclusive";
			case OutfitExclusivity.ExcludeItemsBelow:
				return "Exclude Items Below";
			case OutfitExclusivity.ExcludeAllItems:
				return "Exclude All Other Items";
		}

		throw new ApplicationException($"Unknown OutfitExclusivity: {item.ToString()}");
	}
}
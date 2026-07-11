#nullable enable

using System;
using MudSharp.Framework;

namespace MudSharp.GameItems;

/// <summary>
/// A durable, resolution-independent reference to the legal owner of an item.
/// </summary>
public readonly record struct ItemOwnershipReference(string FrameworkItemType, long Id)
{
	public bool Matches(IFrameworkItem? item)
	{
		return item is not null &&
		       item.Id == Id &&
		       item.FrameworkItemType.Equals(FrameworkItemType, StringComparison.OrdinalIgnoreCase);
	}

	public override string ToString()
	{
		return $"{Id:F0}|{FrameworkItemType}";
	}
}

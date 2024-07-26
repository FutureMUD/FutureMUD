using System;

namespace MudSharp.Framework;

public static class FrameworkItemExtensions {
	public static bool FrameworkItemEquals(this IFrameworkItem item, long? id, string type) {
		if (item == null) {
			return (id ?? 0) == 0;
		}

		return (item.Id == id) && item.FrameworkItemType.Equals(type, StringComparison.InvariantCulture);
	}
}
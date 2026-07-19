#nullable enable

using MudSharp.Community;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Estates;
using MudSharp.Economy.Property;

namespace MudSharp.GameItems;

public static class ItemOwnershipService
{
	public static bool IsSupportedOwner(IFrameworkItem? owner)
	{
		return owner switch
		{
			null => false,
			IClan { IsTemplate: true } => false,
			ICharacter or IClan or IProperty or IEstate or IEmploymentHost => true,
			_ => false
		};
	}

	public static bool AssignOwner(IGameItem item, IFrameworkItem owner)
	{
		if (!IsSupportedOwner(owner))
		{
			return false;
		}

		item.SetOwner(owner);
		return true;
	}

	public static int AssignOwner(IEnumerable<IGameItem> items, IFrameworkItem owner)
	{
		if (!IsSupportedOwner(owner))
		{
			return 0;
		}

		var count = 0;
		foreach (var item in items)
		{
			item.SetOwner(owner);
			count++;
		}

		return count;
	}
}

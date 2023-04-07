using System;
using MudSharp.Models;
using MudSharp.Framework;

namespace MudSharp.GameItems.Groups;

public class GameItemGroupFactory
{
	public static IGameItemGroup CreateGameItemGroup(ItemGroup group, IFuturemud gameworld)
	{
		return new GameItemGroup(group, gameworld);
	}

	public static IGameItemGroupForm CreateGameItemGroupForm(ItemGroupForm form, IGameItemGroup parent,
		IFuturemud gameworld)
	{
		switch (form.Type)
		{
			case "Simple":
				return new SimpleGameItemGroupForm(form, parent);
			case "Stacking":
				return new StackingItemGroupForm(form, parent, gameworld);
			default:
				throw new NotSupportedException("Invalid type in CreateGameItemGroupForm");
		}
	}

	public static IGameItemGroupForm CreateGameItemGroupForm(IGameItemGroup parent, string type, IFuturemud gameworld)
	{
		switch (type.ToLowerInvariant())
		{
			case "simple":
				return new SimpleGameItemGroupForm(parent);
			case "stacking":
				return new StackingItemGroupForm(parent, gameworld);
			default:
				return null;
		}
	}
}
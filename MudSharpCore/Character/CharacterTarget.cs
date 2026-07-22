using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.GameItems;

#nullable enable

namespace MudSharp.Character;

public partial class Character : ITarget
{
	internal IEnumerable<IPerceivable> SpatiallyTargetablePerceivables(bool ignoreLayers = false)
	{
		if (Location?.RouteDefinition is null)
		{
			return ignoreLayers
				? Location?.Perceivables ?? []
				: Location?.Perceivables.Where(x => x.RoomLayer == RoomLayer) ?? [];
		}

		var maximumDistance = Gameworld.GetStaticDouble("RouteCellVeryDistantDistanceMetres");
		if (!double.IsFinite(maximumDistance) || maximumDistance <= 0.0)
		{
			maximumDistance = RouteSpatialConfiguration.Default.VeryDistantDistanceMetres;
		}

		if (!ignoreLayers)
		{
			return RouteSpatialService.Instance.GetPerceivablesWithin(
				SpatialLocation,
				maximumDistance,
				x => x.RoomLayer == RoomLayer);
		}

		var origin = RouteSpatialService.Instance.GetEffectiveLocation(this);
		return origin.RoutePositionMetres.HasValue
			? RouteSpatialService.Instance.GetPerceivablesWithinAcrossLayers(origin, maximumDistance)
			: [];
	}

	internal IEnumerable<ICharacter> SpatiallyTargetableCharacters(bool ignoreLayers = false)
	{
		return SpatiallyTargetablePerceivables(ignoreLayers).OfType<ICharacter>();
	}

	internal IEnumerable<IGameItem> SpatiallyTargetableItems(bool ignoreLayers = false)
	{
		return IncludeTargetProjections(SpatiallyTargetablePerceivables(ignoreLayers).OfType<IGameItem>());
	}

	internal static IEnumerable<IGameItem> IncludeTargetProjections(IEnumerable<IGameItem> localItems)
	{
		var items = localItems
		            .Where(x => x is not null && !x.Deleted && !x.Destroyed)
		            .Distinct()
		            .ToList();
		var projections = items
		                  .SelectMany(x => x.Components
		                                    .OfType<IProvideItemTargetProjections>()
		                                    .SelectMany(y => y.TargetProjections))
		                  .Where(x => x is not null && !x.Deleted && !x.Destroyed);

		return items
		       .Concat(projections)
		       .Distinct();
	}

    private IGameItem? TargetItemWithinItem(IGameItem itemTarget, string keyword)
    {
        List<IGameItem> contents = new();
        if (itemTarget.GetItemType<ILockable>() is ILockable lockableItem)
        {
            contents.AddRange(lockableItem.Locks.Select(x => x.Parent));
        }

        if (itemTarget.GetItemType<IContainer>() is IContainer containerItem &&
            (itemTarget.GetItemType<IOpenable>() is not IOpenable io || io.IsOpen))
        {
            contents.AddRange(containerItem.Contents);
        }

        if (itemTarget.AttachedAndConnectedItems.Any())
        {
            contents.AddRange(itemTarget.AttachedAndConnectedItems);
        }

        return contents.GetFromItemListByKeyword(keyword, this);
    }

    private IGameItem? TargetItemWithinContainer(string containerKeyword, string itemKeyword)
    {
        IPerceivable? container = Target(containerKeyword);
        if (container == null)
        {
            return null;
        }

        if (container is ICharacter charTarget)
        {
            return charTarget.Body.ExternalItemsForOtherActors.Where(x => CanSee(x))
                             .GetFromItemListByKeyword(itemKeyword, this);
        }

        IGameItem? itemTarget = container as IGameItem ??
                                Location.GetExit(containerKeyword, "", this)?.Exit.Door?.Parent;
        return itemTarget is null ? null : TargetItemWithinItem(itemTarget, itemKeyword);
    }

    public IPerceivable? Target(string keyword)
    {
        string[] split = keyword.Split('@', 2, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length == 2 && split.All(x => x.Length > 0))
        {
            return TargetItemWithinContainer(split[0], split[1]) ??
                   TargetItemWithinContainer(split[1], split[0]);
        }

        if (keyword.Equals("me", StringComparison.InvariantCultureIgnoreCase) ||
            keyword.Equals("self", StringComparison.InvariantCultureIgnoreCase))
        {
            return this;
        }

        ICellExit targetExit = Location.GetExitKeyword(keyword, this);
        if (targetExit?.Exit.Door != null)
        {
            return targetExit.Exit.Door.Parent;
        }

        List<IPerceiver> targets = SpatiallyTargetableCharacters().Except(this)
                              .Cast<IPerceiver>()
                              .Concat(Body.ExternalItems.Concat(SpatiallyTargetableItems()))
                              .Where(x => CanSee(x))
                              .ToList();
        return targets.GetFromItemListByKeyword(keyword, this);
    }

    public virtual IPerceivable? TargetLocal(string keyword)
    {
        if (keyword.Equals("me", StringComparison.InvariantCultureIgnoreCase) ||
            keyword.Equals("self", StringComparison.InvariantCultureIgnoreCase))
        {
            return this;
        }

        List<IPerceiver> targets = SpatiallyTargetableCharacters()
                              .Where(x => x != this)
                              .Cast<IPerceiver>()
                              .Concat(SpatiallyTargetableItems())
                              .Where(x => CanSee(x))
                              .ToList();

        return targets.GetFromItemListByKeyword(keyword, this);
    }

    public ICharacter? TargetActor(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
    {
        if (keyword.Equals("me", StringComparison.InvariantCultureIgnoreCase) ||
            keyword.Equals("self", StringComparison.InvariantCultureIgnoreCase))
        {
            return this;
        }

        if (ignoreFlags.HasFlag(PerceiveIgnoreFlags.IgnoreLayers))
        {
            return SpatiallyTargetableCharacters(true)
                   .Except(this)
                   .Where(x => CanSee(x, ignoreFlags))
                   .GetFromItemListByKeyword(keyword, this);
        }

        return SpatiallyTargetableCharacters()
               .Except(this)
               .Where(x => CanSee(x, ignoreFlags))
               .GetFromItemListByKeyword(keyword, this);
    }

    public ICharacter? TargetActorOrCorpse(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
    {
        if (keyword.Equals("me", StringComparison.InvariantCultureIgnoreCase) ||
            keyword.Equals("self", StringComparison.InvariantCultureIgnoreCase))
        {
            return this;
        }

        if (ignoreFlags.HasFlag(PerceiveIgnoreFlags.IgnoreLayers))
        {
            var body = Body!;
            return SpatiallyTargetableCharacters(true)
                           .Except(this)
                           .Concat(SpatiallyTargetableItems(true).Select(x => x.GetItemType<ICorpse>())
                                           .Where(x => x is { RepresentsFinalCharacterDeath: true })
                                           .Select(x => x!.OriginalCharacter))
                           .Concat(body.ExternalItems.Select(x => x.GetItemType<ICorpse>())
                                       .Where(x => x is { RepresentsFinalCharacterDeath: true })
                                       .Select(x => x!.OriginalCharacter))
                           .Where(x => CanSee(x, ignoreFlags))
                           .GetFromItemListByKeyword(keyword, this);
        }

        var localBody = Body!;
        return SpatiallyTargetableCharacters()
                       .Except(this)
                       .Concat(SpatiallyTargetableItems()
                                       .Select(x => x.GetItemType<ICorpse>())
                                       .Where(x => x is { RepresentsFinalCharacterDeath: true })
                                       .Select(x => x!.OriginalCharacter))
                       .Concat(localBody.ExternalItems.Select(x => x.GetItemType<ICorpse>())
                                      .Where(x => x is { RepresentsFinalCharacterDeath: true })
                                      .Select(x => x!.OriginalCharacter))
                       .Where(x => CanSee(x, ignoreFlags))
                       .GetFromItemListByKeyword(keyword, this);
    }

    public ICorpse? TargetCorpse(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
    {
        if (ignoreFlags.HasFlag(PerceiveIgnoreFlags.IgnoreLayers))
        {
            return
                Body!
                    .ExternalItems
                    .OfType<IGameItem>()
                    .Where(x => x.GetItemType<ICorpse>() is not null)
                    .Concat(SpatiallyTargetableItems(true))
                    .Where(x => CanSee(x, ignoreFlags))
                    .GetFromItemListByKeyword(keyword, this)
                    ?.GetItemType<ICorpse>();
        }

        return
            Body!
                .ExternalItems
                .OfType<IGameItem>()
                .Where(x => x.GetItemType<ICorpse>() is not null)
                .Concat(SpatiallyTargetableItems())
                .Where(x => CanSee(x, ignoreFlags))
                .GetFromItemListByKeyword(keyword, this)
                ?.GetItemType<ICorpse>();
    }

    public ICharacter? TargetAlly(string keyword)
    {
        return SpatiallyTargetableCharacters()
               .Where(x => IsAlly(x) && CanSee(x))
               .GetFromItemListByKeyword(keyword, this);
    }

    public virtual ICharacter? TargetNonAlly(string keyword)
    {
        return SpatiallyTargetableCharacters()
               .Where(x => !IsAlly(x) && CanSee(x))
               .GetFromItemListByKeyword(keyword, this);
    }

    public IBody? TargetBody(string keyword)
    {
        return TargetActor(keyword, PerceiveIgnoreFlags.None)?.Body;
    }

    public IGameItem? TargetItem(string keyword)
    {
        string[] split = keyword.Split('@', 2, StringSplitOptions.RemoveEmptyEntries);
        if (split.Length == 2 && split.All(x => x.Length > 0))
        {
            return TargetItemWithinContainer(split[0], split[1]) ??
                   TargetItemWithinContainer(split[1], split[0]);
        }

        ICellExit targetExit = Location.GetExitKeyword(keyword, this);
        if (targetExit?.Exit.Door != null)
        {
            return targetExit.Exit.Door.Parent;
        }

        return Body.ExternalItems
                   .Concat(SpatiallyTargetableItems())
                   .Where(x => CanSee(x))
                   .GetFromItemListByKeyword(keyword, this);
    }

    public IGameItem? TargetLocalItem(string keyword)
    {
        ICellExit targetExit = Location.GetExitKeyword(keyword, this);
        if (targetExit?.Exit.Door != null)
        {
            return targetExit.Exit.Door.Parent;
        }

        return SpatiallyTargetableItems()
               .Where(x => CanSee(x))
               .GetFromItemListByKeyword(keyword, this);
    }


    public (ICharacter? Target, IEnumerable<ICellExit> Path) TargetDistantActor(string keyword, ICellExit? initialExit,
        uint maximumRange,
        bool respectDoors, bool respectCorners)
    {
        IEnumerable<CardinalDirection> permittedDirections = initialExit == null
            ? Constants.CardinalDirections
            : Constants.CardinalDirections.Where(x => !x.IsOpposingDirection(initialExit.OutboundDirection));
        ICharacter target = this.CellsInVicinity(maximumRange, respectDoors, respectCorners, permittedDirections,
                             initialExit?.OutboundDirection ?? CardinalDirection.Unknown)
                         .SelectMany(x => x.Characters)
                         .Where(x => x.RoomLayer.CanBeSeenFromLayer(RoomLayer))
                         .GetFromItemListByKeyword(keyword, this);
        return (target, this.PathBetween(target, maximumRange, false, false, respectDoors));
    }

    public (IGameItem? Target, IEnumerable<ICellExit> Path) TargetDistantItem(string keyword, ICellExit? initialExit,
        uint maximumRange,
        bool respectDoors, bool respectCorners)
    {
        IEnumerable<CardinalDirection> permittedDirections = initialExit == null
            ? Constants.CardinalDirections
            : Constants.CardinalDirections.Where(x => !x.IsOpposingDirection(initialExit.OutboundDirection));
        IGameItem target = this.CellsInVicinity(maximumRange, respectDoors, respectCorners, permittedDirections,
                             initialExit?.OutboundDirection ?? CardinalDirection.Unknown)
                         .SelectMany(x => x.GameItems)
                         .Where(x => x.RoomLayer.CanBeSeenFromLayer(RoomLayer))
                         .GetFromItemListByKeyword(keyword, this);
        return (target, this.PathBetween(target, maximumRange, false, false, respectDoors));
    }

    public IGameItem? TargetPersonalItem(string keyword)
    {
        return Body.ExternalItems.Where(x => CanSee(x)).GetFromItemListByKeyword(keyword, this);
    }

    public IGameItem? TargetLocalOrHeldItem(string keyword)
    {
        return Body.ItemsInHands.Concat(SpatiallyTargetableItems()).Where(x => CanSee(x))
                   .GetFromItemListByKeyword(keyword, this);
    }

    public IGameItem? TargetHeldItem(string keyword)
    {
        return Body.ItemsInHands.Where(x => CanSee(x)).GetFromItemListByKeyword(keyword, this);
    }

    public IGameItem? TargetWornItem(string keyword)
    {
        return Body.ItemsInHands.Concat(SpatiallyTargetableItems()).Where(x => CanSee(x))
                   .GetFromItemListByKeyword(keyword, this);
    }

    public IGameItem? TargetTopLevelWornItem(string keyword)
    {
        return Body.WornItems.Where(x => CanSee(x)).Reverse().GetFromItemListByKeyword(keyword, this);
    }

    public string BestKeywordFor(IPerceivable target)
    {
        IEnumerable<string> keywords = target.GetKeywordsFor(this);
        List<IPerceivable> targets = SpatiallyTargetableCharacters().Except(this)
                              .Cast<IPerceivable>()
                              .Concat(Body.ExternalItems.Concat(SpatiallyTargetableItems()))
                              .Where(x => CanSee(x)).ToList();
        return
            $"{targets.IndexOf(target) + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}";
    }

    public string BestKeywordFor(ICharacter target)
    {
        IEnumerable<string> keywords = target.GetKeywordsFor(this);
        List<ICharacter> targets = SpatiallyTargetableCharacters().Except(this)
                              .Where(x => CanSee(x)).ToList();
        return
            $"{targets.IndexOf(target) + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}";
    }

    public string BestKeywordFor(IGameItem target)
    {
        IEnumerable<string> keywords = target.GetKeywordsFor(this);
        List<IGameItem> targets = Body.ExternalItems
                          .Concat(SpatiallyTargetableItems())
                          .Where(x => CanSee(x))
                          .ToList();
        return
            $"{targets.IndexOf(target) + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}";
    }
}

using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace MudSharp.Character;

public partial class Character : ITarget
{
	public IPerceivable? Target(string keyword)
	{
		var split = keyword.Split('@', 2, StringSplitOptions.RemoveEmptyEntries);
		if (split.Length == 2 && split.All(x => x.Length > 0))
		{
			var containerTarget = split[0];
			keyword = split[1];
			var container = Target(containerTarget);
			if (container == null)
			{
				return null;
			}

			if (container is ICharacter charTarget)
			{
				return charTarget.Body.ExternalItemsForOtherActors.Where(x => CanSee(x))
				                 .GetFromItemListByKeyword(keyword, this);
			}

			if (container is IGameItem itemTarget)
			{
				var contents = new List<IGameItem>();
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

			return null;
		}

		if (keyword.Equals("me", StringComparison.InvariantCultureIgnoreCase) ||
		    keyword.Equals("self", StringComparison.InvariantCultureIgnoreCase))
		{
			return this;
		}

		var targetExit = Location.GetExitKeyword(keyword, this);
		if (targetExit?.Exit.Door != null)
		{
			return targetExit.Exit.Door.Parent;
		}

		var targets = Location.LayerCharacters(RoomLayer).Except(this)
		                      .Cast<IPerceiver>()
		                      .Concat(Body.ExternalItems.Concat(Location.LayerGameItems(RoomLayer)))
		                      .Where(x => CanSee(x)).ToList();
		return targets.GetFromItemListByKeyword(keyword, this);
	}

	public virtual IPerceivable? TargetLocal(string keyword)
	{
		if (keyword.Equals("me", StringComparison.InvariantCultureIgnoreCase) ||
		    keyword.Equals("self", StringComparison.InvariantCultureIgnoreCase))
		{
			return this;
		}

		var targets = Location.LayerCharacters(RoomLayer)
		                      .Where(x => x != this)
		                      .Cast<IPerceiver>()
		                      .Concat(Location.LayerGameItems(RoomLayer))
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
			return Location
			       .Characters
			       .Except(this)
			       .Where(x => CanSee(x, ignoreFlags))
			       .GetFromItemListByKeyword(keyword, this);
		}

		return Location
		       .LayerCharacters(RoomLayer)
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
			return Location.Characters
			               .Except(this)
			               .Concat(Location.GameItems.SelectNotNull(x => x.GetItemType<ICorpse>()?.OriginalCharacter))
			               .Concat(Body.ExternalItems.SelectNotNull(x => x.GetItemType<ICorpse>()?.OriginalCharacter))
			               .Where(x => CanSee(x, ignoreFlags))
			               .GetFromItemListByKeyword(keyword, this);
		}

		return Location.LayerCharacters(RoomLayer)
		               .Except(this)
		               .Concat(Location.LayerGameItems(RoomLayer)
		                               .SelectNotNull(x => x.GetItemType<ICorpse>()?.OriginalCharacter))
		               .Concat(Body.ExternalItems.SelectNotNull(x => x.GetItemType<ICorpse>()?.OriginalCharacter))
		               .Where(x => CanSee(x, ignoreFlags))
		               .GetFromItemListByKeyword(keyword, this);
	}

	public ICorpse? TargetCorpse(string keyword, PerceiveIgnoreFlags ignoreFlags = PerceiveIgnoreFlags.None)
	{
		if (ignoreFlags.HasFlag(PerceiveIgnoreFlags.IgnoreLayers))
		{
			return
				Body
					.ExternalItems
					.WhereNotNull(x => x.GetItemType<ICorpse>())
				    .Concat(Location.GameItems)
				    .Where(x => CanSee(x, ignoreFlags))
				    .GetFromItemListByKeyword(keyword, this)
				    ?.GetItemType<ICorpse>();
		}

		return
			Body
				.ExternalItems
				.WhereNotNull(x => x.GetItemType<ICorpse>())
				.Concat(Location.LayerGameItems(RoomLayer))
				.Where(x => CanSee(x, ignoreFlags))
				.GetFromItemListByKeyword(keyword, this)
				?.GetItemType<ICorpse>();
	}

	public ICharacter? TargetAlly(string keyword)
	{
		return Location
		       .LayerCharacters(RoomLayer)
		       .Where(x => IsAlly(x) && CanSee(x))
		       .GetFromItemListByKeyword(keyword, this);
	}

	public virtual ICharacter? TargetNonAlly(string keyword)
	{
		return Location
		       .LayerCharacters(RoomLayer)
		       .Where(x => !IsAlly(x) && CanSee(x))
		       .GetFromItemListByKeyword(keyword, this);
	}

	public IBody? TargetBody(string keyword)
	{
		return TargetActor(keyword, PerceiveIgnoreFlags.None)?.Body;
	}

	public IGameItem? TargetItem(string keyword)
	{
		var split = keyword.Split('@', 2, StringSplitOptions.RemoveEmptyEntries);
		if (split.Length == 2 && split.All(x => x.Length > 0))
		{
			var containerTarget = split[0];
			keyword = split[1];
			var container = Target(containerTarget);
			if (container == null)
			{
				return null;
			}

			if (container is ICharacter charTarget)
			{
				return charTarget.Body.ExternalItemsForOtherActors.Where(x => CanSee(x))
				                 .GetFromItemListByKeyword(keyword, this);
			}

			var itemTarget = container as IGameItem ??
			                 Location.GetExit(containerTarget, "", this)?.Exit.Door?.Parent;

			if (itemTarget != null)
			{
				var contents = new List<IGameItem>();
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

			return null;
		}

		var targetExit = Location.GetExitKeyword(keyword, this);
		if (targetExit?.Exit.Door != null)
		{
			return targetExit.Exit.Door.Parent;
		}

		return Body.ExternalItems
		           .Concat(Location.LayerGameItems(RoomLayer))
		           .Where(x => CanSee(x))
		           .GetFromItemListByKeyword(keyword, this);
	}

	public IGameItem? TargetLocalItem(string keyword)
	{
		var targetExit = Location.GetExitKeyword(keyword, this);
		if (targetExit?.Exit.Door != null)
		{
			return targetExit.Exit.Door.Parent;
		}

		return Location
		       .LayerGameItems(RoomLayer)
		       .Where(x => CanSee(x))
		       .GetFromItemListByKeyword(keyword, this);
	}


	public (ICharacter? Target, IEnumerable<ICellExit> Path) TargetDistantActor(string keyword, ICellExit? initialExit,
		uint maximumRange,
		bool respectDoors, bool respectCorners)
	{
		var permittedDirections = initialExit == null
			? Constants.CardinalDirections
			: Constants.CardinalDirections.Where(x => !x.IsOpposingDirection(initialExit.OutboundDirection));
		var target = this.CellsInVicinity(maximumRange, respectDoors, respectCorners, permittedDirections,
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
		var permittedDirections = initialExit == null
			? Constants.CardinalDirections
			: Constants.CardinalDirections.Where(x => !x.IsOpposingDirection(initialExit.OutboundDirection));
		var target = this.CellsInVicinity(maximumRange, respectDoors, respectCorners, permittedDirections,
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
		return Body.ItemsInHands.Concat(Location.LayerGameItems(RoomLayer)).Where(x => CanSee(x))
		           .GetFromItemListByKeyword(keyword, this);
	}

	public IGameItem? TargetHeldItem(string keyword)
	{
		return Body.ItemsInHands.Where(x => CanSee(x)).GetFromItemListByKeyword(keyword, this);
	}

	public IGameItem? TargetWornItem(string keyword)
	{
		return Body.ItemsInHands.Concat(Location.LayerGameItems(RoomLayer)).Where(x => CanSee(x))
		           .GetFromItemListByKeyword(keyword, this);
	}

	public IGameItem? TargetTopLevelWornItem(string keyword)
	{
		return Body.WornItems.Where(x => CanSee(x)).Reverse().GetFromItemListByKeyword(keyword, this);
	}

	public string BestKeywordFor(IPerceivable target)
	{
		var keywords = target.GetKeywordsFor(this);
		var targets = Location.LayerCharacters(RoomLayer).Except(this)
		                      .Cast<IPerceivable>()
		                      .Concat(Body.ExternalItems.Concat(Location.LayerGameItems(RoomLayer)))
		                      .Where(x => CanSee(x)).ToList();
		return
			$"{targets.IndexOf(target) + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}";
	}

	public string BestKeywordFor(ICharacter target)
	{
		var keywords = target.GetKeywordsFor(this);
		var targets = Location.LayerCharacters(RoomLayer).Except(this)
		                      .Where(x => CanSee(x)).ToList();
		return
			$"{targets.IndexOf(target) + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}";
	}

	public string BestKeywordFor(IGameItem target)
	{
		var keywords = target.GetKeywordsFor(this);
		var targets = Body.ExternalItems
		                  .Concat(Location.LayerGameItems(RoomLayer))
		                  .Where(x => CanSee(x))
		                  .ToList();
		return
			$"{targets.IndexOf(target) + 1}.{keywords.ListToString(separator: ".", conjunction: "", twoItemJoiner: ".")}";
	}
}
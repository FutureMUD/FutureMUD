#nullable enable
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Work.Foraging;
using System.Linq;

namespace MudSharp.NPC.AI;

internal static class ForagerAIHelpers
{
	public static bool IsHungry(ICharacter character)
	{
		return character.NeedsModel.Status.IsHungry();
	}

	private static bool IsBusy(ICharacter character)
	{
		return character.State.HasFlag(CharacterState.Dead) ||
		       character.State.HasFlag(CharacterState.Stasis) ||
		       character.Combat is not null ||
		       character.Movement is not null ||
		       !CharacterState.Able.HasFlag(character.State) ||
		       character.Effects.Any(x => x.IsBlockingEffect("general") || x.IsBlockingEffect("movement"));
	}

	public static bool HasDirectEdibleYield(ICharacter character, ICell cell)
	{
		return character.Race.EdibleForagableYields
		                .Any(x => cell.GetForagableYield(x.YieldType) > 0.0 &&
		                          character.Race.CanEatForagableYield(x.YieldType));
	}

	public static bool HasEligibleForageableFood(ICharacter character, ICell cell)
	{
		IForagableProfile? profile = cell.ForagableProfile;
		if (profile is null)
		{
			return false;
		}

		return profile.Foragables
		              .Where(x => x.ItemProto?.IsItemType<FoodGameItemComponentProto>() == true)
		              .Where(x => x.CanForage(character, RPG.Checks.Outcome.MajorPass))
		              .SelectMany(x => x.ForagableTypes)
		              .Where(x => !string.IsNullOrWhiteSpace(x))
		              .Any(x => cell.GetForagableYield(x) > 0.0);
	}

	public static bool HasFoodOpportunity(ICharacter character, ICell cell)
	{
		return HasDirectEdibleYield(character, cell) || HasEligibleForageableFood(character, cell);
	}

	public static bool TryEatExistingFood(ICharacter character)
	{
		if (IsBusy(character) || !IsHungry(character))
		{
			return false;
		}

		IEdible? edible = character.Body.HeldOrWieldedItems
		                           .SelectNotNull(x => x!.GetItemType<IEdible>())
		                           .Concat(character.Location!.LayerGameItems(character.RoomLayer)
		                                            .SelectMany(x => x.ShallowAccessibleItems(character))
		                                            .SelectNotNull(x => x!.GetItemType<IEdible>()))
		                           .Where(x => character.CanEat(x, x.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0))
		                           .GetRandomElement();
		if (edible is null)
		{
			return false;
		}

		character.SetTarget(edible.Parent);
		character.SetModifier(PositionModifier.None);
		character.SetEmote(null);
		character.Eat(edible, edible.Parent.ContainedIn?.GetItemType<IContainer>(), null, 1.0, null);
		return true;
	}

	public static bool TryEatDirectYield(ICharacter character)
	{
		if (IsBusy(character) || !IsHungry(character))
		{
			return false;
		}

		string? yield = character.Race.EdibleForagableYields
		                         .Where(x => character.Location.GetForagableYield(x.YieldType) > 0.0)
		                         .Where(x => character.CanEat(x.YieldType, 1.0).Success)
		                         .Select(x => x.YieldType)
		                         .GetRandomElement();
		if (yield is null)
		{
			return false;
		}

		character.SetModifier(PositionModifier.None);
		character.SetEmote(null);
		character.Eat(yield, 1.0, null);
		return true;
	}

	public static bool TryForageForFood(ICharacter character)
	{
		if (IsBusy(character) || !IsHungry(character) || HasDirectEdibleYield(character, character.Location))
		{
			return false;
		}

		IForagableProfile? profile = character.Location.ForagableProfile;
		if (profile is null)
		{
			return false;
		}

		string? forageType = profile.Foragables
		                            .Where(x => x.ItemProto?.IsItemType<FoodGameItemComponentProto>() == true)
		                            .Where(x => x.CanForage(character, RPG.Checks.Outcome.MajorPass))
		                            .SelectMany(x => x.ForagableTypes)
		                            .Where(x => !string.IsNullOrWhiteSpace(x))
		                            .Where(x => character.Location.GetForagableYield(x) > 0.0)
		                            .Distinct()
		                            .GetRandomElement();
		if (forageType is null)
		{
			return false;
		}

		character.ExecuteCommand($"forage {forageType}");
		return true;
	}

	public static bool TrySatisfyHunger(ICharacter character)
	{
		return TryEatExistingFood(character) ||
		       TryEatDirectYield(character) ||
		       TryForageForFood(character);
	}
}

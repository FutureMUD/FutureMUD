#nullable enable
using MudSharp.Body.Needs;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.NPC.AI;

internal static class NpcSurvivalAIHelpers
{
	private const int DefaultWaterSearchRange = 20;

	public static bool IsThirsty(ICharacter character)
	{
		return character.NeedsModel.Status.IsThirsty();
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

	private static double DrinkAmount(ICharacter character)
	{
		double amount = character.Gameworld.GetStaticDouble("DefaultAnimalDrinkAmount");
		return amount > 0.0 ? amount : character.Gameworld.GetStaticDouble("DefaultSipAmount");
	}

	public static List<ILiquidContainer> LocalDrinkableLiquids(ICharacter character, ICell cell, bool requireCanDrink = true)
	{
		double amount = DrinkAmount(character);
		IEnumerable<ILiquidContainer> liquids = cell.LayerGameItems(character.RoomLayer)
		                                           .SelectNotNull(x => x.GetItemType<ILiquidContainer>())
		                                           .Where(x => (x.LiquidMixture?.Instances.Sum(y =>
			                                                       y.Liquid.DrinkSatiatedHoursPerLitre) ?? 0.0) >
		                                                       0.0);
		if (requireCanDrink)
		{
			liquids = liquids.Where(x => character.Body.CanDrink(x, null, amount));
		}

		return liquids.ToList();
	}

	public static bool HasLocalWaterSource(ICharacter character)
	{
		return LocalDrinkableLiquids(character, character.Location).Any();
	}

	public static bool HasWaterSource(ICharacter character, ICell cell)
	{
		return LocalDrinkableLiquids(character, cell, false).Any();
	}

	public static bool HasAquaticWaterSource(ICharacter character, ICell cell, bool requireSurface)
	{
		return requireSurface
			? AnimalAI.CellSupportsSurfaceWater(character, cell)
			: AnimalAI.CellSupportsSwimming(character, cell);
	}

	public static bool TryDrinkIfThirsty(ICharacter character)
	{
		if (IsBusy(character) || !IsThirsty(character))
		{
			RememberCurrentWaterIfPresent(character);
			return false;
		}

		ILiquidContainer? liquid = LocalDrinkableLiquids(character, character.Location).GetRandomElement();
		if (liquid is null)
		{
			NpcKnownWaterLocationsEffect.Get(character)?.Forget(character.Location);
			return false;
		}

		NpcKnownWaterLocationsEffect.GetOrCreate(character).Remember(character.Location);
		character.SetTarget(liquid.Parent);
		character.SetModifier(PositionModifier.None);
		character.SetEmote(null);
		return character.Drink(liquid, null, DrinkAmount(character), null);
	}

	public static void RememberCurrentWaterIfPresent(ICharacter character)
	{
		if (HasLocalWaterSource(character))
		{
			NpcKnownWaterLocationsEffect.GetOrCreate(character).Remember(character.Location);
		}
	}

	public static bool TryHydrateFromAquaticEnvironmentIfThirsty(ICharacter character, bool requireSurface)
	{
		if (IsBusy(character) || !IsThirsty(character))
		{
			RememberCurrentAquaticWaterIfPresent(character, requireSurface);
			return false;
		}

		if (!HasAquaticWaterSource(character, character.Location, requireSurface))
		{
			NpcKnownWaterLocationsEffect.Get(character)?.Forget(character.Location);
			return false;
		}

		IFluid? fluid = character.Location.Terrain(character)?.WaterFluid;
		if (fluid is not ILiquid liquid)
		{
			NpcKnownWaterLocationsEffect.Get(character)?.Forget(character.Location);
			return false;
		}

		double amount = DrinkAmount(character);
		character.Body.FulfilNeeds(new NeedFulfiller
		{
			ThirstPoints = Math.Max(amount * liquid.DrinkSatiatedHoursPerLitre, amount),
			WaterLitres = Math.Max(amount * liquid.WaterLitresPerLitre, amount)
		}, true);
		NpcKnownWaterLocationsEffect.GetOrCreate(character).Remember(character.Location);
		return true;
	}

	public static void RememberCurrentAquaticWaterIfPresent(ICharacter character, bool requireSurface)
	{
		if (HasAquaticWaterSource(character, character.Location, requireSurface))
		{
			NpcKnownWaterLocationsEffect.GetOrCreate(character).Remember(character.Location);
		}
	}

	public static (ICell? Target, IEnumerable<ICellExit> Path) GetPathToWater(
		ICharacter character,
		Func<ICellExit, bool> suitabilityFunction,
		int range = DefaultWaterSearchRange)
	{
		NpcKnownWaterLocationsEffect? knownWater = NpcKnownWaterLocationsEffect.Get(character);
		if (knownWater is not null)
		{
			foreach (ICell cell in knownWater.KnownWaterLocations.Where(x => !ReferenceEquals(x, character.Location)))
			{
				List<ICellExit> path = character.PathBetween(cell, (uint)range, suitabilityFunction).ToList();
				if (path.Any())
				{
					return (cell, path);
				}
			}
		}

		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
			x => x is ICell cell && HasWaterSource(character, cell),
			(uint)range,
			suitabilityFunction);
		return targetPath.Item1 is ICell target && targetPath.Item2.Any()
			? (target, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}

	public static (ICell? Target, IEnumerable<ICellExit> Path) GetPathToAquaticWater(
		ICharacter character,
		Func<ICellExit, bool> suitabilityFunction,
		int range = DefaultWaterSearchRange,
		bool requireSurface = false)
	{
		NpcKnownWaterLocationsEffect? knownWater = NpcKnownWaterLocationsEffect.Get(character);
		if (knownWater is not null)
		{
			foreach (ICell cell in knownWater.KnownWaterLocations.Where(x =>
				         !ReferenceEquals(x, character.Location) &&
				         HasAquaticWaterSource(character, x, requireSurface)))
			{
				List<ICellExit> path = character.PathBetween(cell, (uint)range, suitabilityFunction).ToList();
				if (path.Any())
				{
					return (cell, path);
				}
			}
		}

		Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
			x => x is ICell cell && HasAquaticWaterSource(character, cell, requireSurface),
			(uint)range,
			suitabilityFunction);
		return targetPath.Item1 is ICell target && targetPath.Item2.Any()
			? (target, targetPath.Item2)
			: (null, Enumerable.Empty<ICellExit>());
	}
}

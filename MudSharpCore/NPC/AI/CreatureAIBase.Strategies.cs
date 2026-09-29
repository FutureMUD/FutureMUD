#nullable enable
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Celestial;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.NPC.AI.Groups;
using MudSharp.Work.Crafts;
using MoreLinq;

namespace MudSharp.NPC.AI;

public abstract partial class CreatureAIBase : PathingAIBase
{
	protected interface ICreatureHomeStrategy
	{
		void Evaluate(CreatureAIBase ai, ICharacter character);
		void EvaluateIdle(CreatureAIBase ai, ICharacter character);
		bool WouldMove(CreatureAIBase ai, ICharacter character);
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character);
		bool IsDefendingLocation(CreatureAIBase ai, ICharacter character);
	}

	private sealed class NoHomeStrategy : ICreatureHomeStrategy
	{
		public static NoHomeStrategy Instance { get; } = new();

		public void Evaluate(CreatureAIBase ai, ICharacter character)
		{
		}

		public void EvaluateIdle(CreatureAIBase ai, ICharacter character)
		{
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return false;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		public bool IsDefendingLocation(CreatureAIBase ai, ICharacter character)
		{
			return true;
		}
	}

	private sealed class TerritorialHomeStrategy : ICreatureHomeStrategy
	{
		public static TerritorialHomeStrategy Instance { get; } = new();

		public void Evaluate(CreatureAIBase ai, ICharacter character)
		{
			ai.EvaluateTerritory(character);
		}

		public void EvaluateIdle(CreatureAIBase ai, ICharacter character)
		{
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return character.CombinedEffectsOfType<Territory>().FirstOrDefault() is Territory territory &&
			       territory.Cells.Any() &&
			       !territory.Cells.Contains(character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return ai.GetTerritoryPath(character);
		}

		public bool IsDefendingLocation(CreatureAIBase ai, ICharacter character)
		{
			return character.CombinedEffectsOfType<Territory>()
			                .FirstOrDefault()
			                ?.Cells
			                .Contains(character.Location) == true;
		}
	}

	private sealed class DenningHomeStrategy : ICreatureHomeStrategy
	{
		public static DenningHomeStrategy Instance { get; } = new();

		public void Evaluate(CreatureAIBase ai, ICharacter character)
		{
			if (ai.SurvivalNeedsSatisfied(character))
			{
				ai.EvaluateBurrowLifecycle(character);
			}
		}

		public void EvaluateIdle(CreatureAIBase ai, ICharacter character)
		{
			Evaluate(ai, character);
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			if (!ai.SurvivalNeedsSatisfied(character))
			{
				return false;
			}

			NpcHomeBaseEffect home = ai.ResolveHomeBase(character);
			return home.HomeCell is null || !ReferenceEquals(character.Location, home.HomeCell);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return ai.SurvivalNeedsSatisfied(character)
				? ai.GetBurrowHomePath(character)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public bool IsDefendingLocation(CreatureAIBase ai, ICharacter character)
		{
			return NpcHomeBaseEffect.GetOrCreate(character).HomeCell is ICell home &&
			       ReferenceEquals(home, character.Location);
		}
	}

	protected interface ICreatureAwarenessStrategy
	{
		bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget);
		bool WouldMove(CreatureAIBase ai, ICharacter character);
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character);
	}

	private sealed class NoAwarenessStrategy : ICreatureAwarenessStrategy
	{
		public static NoAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget)
		{
			return false;
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return false;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class WaryAwarenessStrategy : ICreatureAwarenessStrategy
	{
		public static WaryAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			if (!ai.ShouldAvoidCell(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return ai.ShouldAvoidCell(character, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			if (!WouldMove(ai, character))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			(ICell? target, IEnumerable<ICellExit> path) = ai.GetRefugePath(character);
			return target is not null && path.Any()
				? (target, path)
				: ai.GetAvoidancePath(character);
		}
	}

	private sealed class WimpyAwarenessStrategy : ICreatureAwarenessStrategy
	{
		public static WimpyAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			if (!threats.Any() && !ai.ShouldAvoidCell(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return ai.VisibleAwarenessThreats(character, null).Any() ||
			       ai.ShouldAvoidCell(character, character.Location);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			if (!WouldMove(ai, character))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			(ICell? target, IEnumerable<ICellExit> path) = ai.GetRefugePath(character);
			return target is not null && path.Any()
				? (target, path)
				: ai.GetAvoidancePath(character);
		}
	}

	private sealed class SkittishAwarenessStrategy : ICreatureAwarenessStrategy
	{
		public static SkittishAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			if (!threats.Any() && !ai.ShouldAvoidCell(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return WimpyAwarenessStrategy.Instance.WouldMove(ai, character);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return WimpyAwarenessStrategy.Instance.GetPath(ai, character);
		}
	}

	private sealed class GuardingAwarenessStrategy : ICreatureAwarenessStrategy
	{
		public static GuardingAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			foreach (ICharacter threat in threats.Shuffle(Constants.Random))
			{
				if (ai.TryGuardingAttack(character, threat))
				{
					return true;
				}
			}

			return false;
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return false;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	protected interface ICreatureRefugeStrategy
	{
		(ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character);
	}

	private sealed class NoRefugeStrategy : ICreatureRefugeStrategy
	{
		public static NoRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class HomeRefugeStrategy : ICreatureRefugeStrategy
	{
		public static HomeRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			NpcHomeBaseEffect home = ai.ResolveHomeBase(character);
			if (home.HomeCell is null || ReferenceEquals(home.HomeCell, character.Location))
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			List<ICellExit> path = character.PathBetween(home.HomeCell, DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character)).ToList();
			return path.Any()
				? (home.HomeCell, path)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class DenRefugeStrategy : ICreatureRefugeStrategy
	{
		public static DenRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return ai.GetBurrowHomePath(character);
		}
	}

	private sealed class TreesRefugeStrategy : ICreatureRefugeStrategy
	{
		public static TreesRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
				x => x is ICell cell && ArborealWandererAI.CellSupportsTreeLayers(character, cell),
				DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character, true));
			return targetPath.Item1 is ICell target && targetPath.Item2.Any()
				? (target, targetPath.Item2)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class SkyRefugeStrategy : ICreatureRefugeStrategy
	{
		public static SkyRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}
	}

	private sealed class WaterRefugeStrategy : ICreatureRefugeStrategy
	{
		public static WaterRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return ai.WaterStrategy == AnimalWaterStrategyType.Drink
				? NpcSurvivalAIHelpers.GetPathToWater(character, ai.GetAnimalSuitabilityFunction(character),
					DefaultNeedRange)
				: NpcSurvivalAIHelpers.GetPathToAquaticWater(character, ai.GetAnimalSuitabilityFunction(character),
					DefaultNeedRange, ai.WaterStrategy == AnimalWaterStrategyType.Surface);
		}
	}

	private sealed class ProgRefugeStrategy : ICreatureRefugeStrategy
	{
		public static ProgRefugeStrategy Instance { get; } = new();

		public (ICell? Target, IEnumerable<ICellExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			Tuple<IPerceivable, IEnumerable<ICellExit>> targetPath = character.AcquireTargetAndPath(
				x => x is ICell cell && ai.RefugeCellProg.ExecuteBool(false, character, cell, character.Location),
				DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character));
			return targetPath.Item1 is ICell target && targetPath.Item2.Any()
				? (target, targetPath.Item2)
				: (null, Enumerable.Empty<ICellExit>());
		}
	}

	protected interface ICreatureMovementStrategy
	{
		bool CellMatches(CreatureAIBase ai, ICharacter character, ICell cell);
		bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer);
		(ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character);
		FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<ICellExit> path);
	}

	private sealed class GroundMovementStrategy : ICreatureMovementStrategy
	{
		public static GroundMovementStrategy Instance { get; } = new();

		public bool CellMatches(CreatureAIBase ai, ICharacter character, ICell cell)
		{
			return ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer == RoomLayer.GroundLevel;
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			return new FollowingPath(character, path);
		}
	}

	private sealed class SwimmingMovementStrategy : ICreatureMovementStrategy
	{
		public static SwimmingMovementStrategy Instance { get; } = new();

		public bool CellMatches(CreatureAIBase ai, ICharacter character, ICell cell)
		{
			return character.Race.CanSwim &&
			       CellSupportsSwimming(character, cell) &&
			       ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer == RoomLayer.GroundLevel || targetLayer.IsUnderwater();
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			RoomLayer targetLayer = ai.WaterStrategy == AnimalWaterStrategyType.Surface
				? RoomLayer.GroundLevel
				: character.RoomLayer;
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}
	}

	private sealed class FlyingMovementStrategy : ICreatureMovementStrategy
	{
		public static FlyingMovementStrategy Instance { get; } = new();

		public bool CellMatches(CreatureAIBase ai, ICharacter character, ICell cell)
		{
			return ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return !targetLayer.IsUnderwater();
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			return new FollowingMultiLayerPath(character, path, ai.TargetFlyingLayer, ai.TargetRestingLayer);
		}
	}

	private sealed class ArborealMovementStrategy : ICreatureMovementStrategy
	{
		public static ArborealMovementStrategy Instance { get; } = new();

		public bool CellMatches(CreatureAIBase ai, ICharacter character, ICell cell)
		{
			return ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location) &&
			       (ArborealWandererAI.CellSupportsTreeLayers(character, cell) ||
			        ai.AllowDescentProg.ExecuteBool(false, character, cell));
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer.In(RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.HighInTrees);
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			List<(ICell Cell, int Distance)> treeTargets = character.CellsAndDistancesInVicinity(
					(uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true),
					cell => ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location) &&
					        ai.IsWithinPreferredHabitat(character, cell) &&
					        ArborealWandererAI.CellSupportsTreeLayers(character, cell))
				.ToList();

			ICell? target = treeTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
			if (target is not null)
			{
				List<ICellExit> path = character.PathBetween(target, (uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true)).ToList();
				if (path.Any())
				{
					return (target, path);
				}
			}

			List<(ICell Cell, int Distance)> descentTargets = character.CellsAndDistancesInVicinity(
					(uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true),
					cell => ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location) &&
					        ai.IsWithinPreferredHabitat(character, cell) &&
					        !ArborealWandererAI.CellSupportsTreeLayers(character, cell) &&
					        ai.AllowDescentProg.ExecuteBool(false, character, cell))
				.ToList();
			target = descentTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
			if (target is null)
			{
				return (null, Enumerable.Empty<ICellExit>());
			}

			List<ICellExit> descentPath = character.PathBetween(target, (uint)ai.MovementRange,
				ai.GetAnimalSuitabilityFunction(character, true)).ToList();
			return descentPath.Any()
				? (target, descentPath)
				: (null, Enumerable.Empty<ICellExit>());
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			ICell destination = path.Last().Destination;
			RoomLayer targetLayer = ChooseTreeLayer(ai, character, destination);
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}

		private static RoomLayer ChooseTreeLayer(CreatureAIBase ai, ICharacter character, ICell cell)
		{
			List<RoomLayer> layers = cell.Terrain(character)?.TerrainLayers.ToList() ?? new List<RoomLayer>();
			if (layers.Contains(ai.PreferredTreeLayer))
			{
				return ai.PreferredTreeLayer;
			}

			if (layers.Contains(ai.SecondaryTreeLayer))
			{
				return ai.SecondaryTreeLayer;
			}

			if (layers.Contains(RoomLayer.HighInTrees))
			{
				return RoomLayer.HighInTrees;
			}

			if (layers.Contains(RoomLayer.InTrees))
			{
				return RoomLayer.InTrees;
			}

			return RoomLayer.GroundLevel;
		}
	}

	private sealed class AmphibiousMovementStrategy : ICreatureMovementStrategy
	{
		public static AmphibiousMovementStrategy Instance { get; } = new();

		public bool CellMatches(CreatureAIBase ai, ICharacter character, ICell cell)
		{
			if (!ai.MovementCellProg.ExecuteBool(false, character, cell, character.Location))
			{
				return false;
			}

			return CellSupportsSwimming(character, cell)
				? ai.AmphibiousWaterCellProg.ExecuteBool(false, character, cell, character.Location)
				: ai.AmphibiousLandCellProg.ExecuteBool(false, character, cell, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer == RoomLayer.GroundLevel || targetLayer.IsUnderwater();
		}

		public (ICell? Target, IEnumerable<ICellExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			bool preferWater = RandomUtilities.DoubleRandom(0.0, 1.0) <= ai.AmphibiousWaterBias;
			(ICell? target, IEnumerable<ICellExit> path) = GetWeightedAmbientPath(ai, character,
				(_, ch, cell) => CellMatches(ai, ch, cell) && CellSupportsSwimming(ch, cell) == preferWater);
			if (target is not null)
			{
				return (target, path);
			}

			return GetWeightedAmbientPath(ai, character, CellMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<ICellExit> path)
		{
			ICell? destination = path.LastOrDefault()?.Destination;
			RoomLayer targetLayer = destination is not null && CellSupportsSwimming(character, destination)
				? ai.WaterStrategy == AnimalWaterStrategyType.Surface ? RoomLayer.GroundLevel : character.RoomLayer
				: RoomLayer.GroundLevel;
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}
	}

	internal static bool CellSupportsSwimming(ICharacter character, ICell cell)
	{
		return cell.IsSwimmingLayer(character.RoomLayer) ||
		       cell.Terrain(character)?.TerrainLayers.Any(cell.IsSwimmingLayer) == true;
	}

	internal static bool CellSupportsSurfaceWater(ICharacter character, ICell cell)
	{
		return CellSupportsSwimming(character, cell) &&
		       cell.Terrain(character)?.TerrainLayers.Any(x => !x.IsUnderwater()) == true;
	}

	private static (ICell? Target, IEnumerable<ICellExit> Path) GetWeightedAmbientPath(
		CreatureAIBase ai,
		ICharacter character,
		Func<CreatureAIBase, ICharacter, ICell, bool> predicate)
	{
		List<(ICell Cell, int Distance)> vicinity = character.CellsAndDistancesInVicinity(
				(uint)ai.MovementRange,
				ai.GetAnimalSuitabilityFunction(character, true),
				cell => predicate(ai, character, cell) && ai.IsWithinPreferredHabitat(character, cell))
			.ToList();
		ICell? target = vicinity.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Cell;
		if (target is null)
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		List<ICellExit> path = character.PathBetween(target, (uint)ai.MovementRange,
			ai.GetAnimalSuitabilityFunction(character, true)).ToList();
		return path.Any()
			? (path.Last().Destination, path)
			: (null, Enumerable.Empty<ICellExit>());
	}}

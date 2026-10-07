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
		(IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character);
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

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<IRoomExit>());
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
			       territory.Rooms.Any() &&
			       !territory.Rooms.Contains(character.Location);
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return ai.GetTerritoryPath(character);
		}

		public bool IsDefendingLocation(CreatureAIBase ai, ICharacter character)
		{
			return character.CombinedEffectsOfType<Territory>()
			                .FirstOrDefault()
			                ?.Rooms
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
			return home.HomeRoom is null || !ReferenceEquals(character.Location, home.HomeRoom);
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return ai.SurvivalNeedsSatisfied(character)
				? ai.GetBurrowHomePath(character)
				: (null, Enumerable.Empty<IRoomExit>());
		}

		public bool IsDefendingLocation(CreatureAIBase ai, ICharacter character)
		{
			return NpcHomeBaseEffect.GetOrCreate(character).HomeRoom is IRoom home &&
			       ReferenceEquals(home, character.Location);
		}
	}

	protected interface ICreatureAwarenessStrategy
	{
		bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget);
		bool WouldMove(CreatureAIBase ai, ICharacter character);
		(IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character);
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

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<IRoomExit>());
		}
	}

	private sealed class WaryAwarenessStrategy : ICreatureAwarenessStrategy
	{
		public static WaryAwarenessStrategy Instance { get; } = new();

		public bool TryRespond(CreatureAIBase ai, ICharacter character, ICharacter? witnessedTarget)
		{
			List<ICharacter> threats = ai.VisibleAwarenessThreats(character, witnessedTarget).ToList();
			ai.RememberThreats(character, threats);
			if (!ai.ShouldAvoidRoom(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return ai.ShouldAvoidRoom(character, character.Location);
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			if (!WouldMove(ai, character))
			{
				return (null, Enumerable.Empty<IRoomExit>());
			}

			(IRoom? target, IEnumerable<IRoomExit> path) = ai.GetRefugePath(character);
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
			if (!threats.Any() && !ai.ShouldAvoidRoom(character, character.Location))
			{
				return false;
			}

			return ai.TryMoveToRefuge(character) ||
			       ai.TryMoveAwayFromAwarenessThreats(character, threats);
		}

		public bool WouldMove(CreatureAIBase ai, ICharacter character)
		{
			return ai.VisibleAwarenessThreats(character, null).Any() ||
			       ai.ShouldAvoidRoom(character, character.Location);
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			if (!WouldMove(ai, character))
			{
				return (null, Enumerable.Empty<IRoomExit>());
			}

			(IRoom? target, IEnumerable<IRoomExit> path) = ai.GetRefugePath(character);
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
			if (!threats.Any() && !ai.ShouldAvoidRoom(character, character.Location))
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

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
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

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<IRoomExit>());
		}
	}

	protected interface ICreatureRefugeStrategy
	{
		(IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character);
	}

	private sealed class NoRefugeStrategy : ICreatureRefugeStrategy
	{
		public static NoRefugeStrategy Instance { get; } = new();

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<IRoomExit>());
		}
	}

	private sealed class HomeRefugeStrategy : ICreatureRefugeStrategy
	{
		public static HomeRefugeStrategy Instance { get; } = new();

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			NpcHomeBaseEffect home = ai.ResolveHomeBase(character);
			if (home.HomeRoom is null || ReferenceEquals(home.HomeRoom, character.Location))
			{
				return (null, Enumerable.Empty<IRoomExit>());
			}

			List<IRoomExit> path = character.PathBetween(home.HomeRoom, DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character)).ToList();
			return path.Any()
				? (home.HomeRoom, path)
				: (null, Enumerable.Empty<IRoomExit>());
		}
	}

	private sealed class DenRefugeStrategy : ICreatureRefugeStrategy
	{
		public static DenRefugeStrategy Instance { get; } = new();

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return ai.GetBurrowHomePath(character);
		}
	}

	private sealed class TreesRefugeStrategy : ICreatureRefugeStrategy
	{
		public static TreesRefugeStrategy Instance { get; } = new();

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			Tuple<IPerceivable, IEnumerable<IRoomExit>> targetPath = character.AcquireTargetAndPath(
				x => x is IRoom room && ArborealWandererAI.RoomSupportsTreeLayers(character, room),
				DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character, true));
			return targetPath.Item1 is IRoom target && targetPath.Item2.Any()
				? (target, targetPath.Item2)
				: (null, Enumerable.Empty<IRoomExit>());
		}
	}

	private sealed class SkyRefugeStrategy : ICreatureRefugeStrategy
	{
		public static SkyRefugeStrategy Instance { get; } = new();

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			return (null, Enumerable.Empty<IRoomExit>());
		}
	}

	private sealed class WaterRefugeStrategy : ICreatureRefugeStrategy
	{
		public static WaterRefugeStrategy Instance { get; } = new();

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
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

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetPath(CreatureAIBase ai, ICharacter character)
		{
			Tuple<IPerceivable, IEnumerable<IRoomExit>> targetPath = character.AcquireTargetAndPath(
				x => x is IRoom room && ai.RefugeRoomProg.ExecuteBool(false, character, room, character.Location),
				DefaultNeedRange,
				ai.GetAnimalSuitabilityFunction(character));
			return targetPath.Item1 is IRoom target && targetPath.Item2.Any()
				? (target, targetPath.Item2)
				: (null, Enumerable.Empty<IRoomExit>());
		}
	}

	protected interface ICreatureMovementStrategy
	{
		bool RoomMatches(CreatureAIBase ai, ICharacter character, IRoom room);
		bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer);
		(IRoom? Target, IEnumerable<IRoomExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character);
		FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<IRoomExit> path);
	}

	private sealed class GroundMovementStrategy : ICreatureMovementStrategy
	{
		public static GroundMovementStrategy Instance { get; } = new();

		public bool RoomMatches(CreatureAIBase ai, ICharacter character, IRoom room)
		{
			return ai.MovementRoomProg.ExecuteBool(false, character, room, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer == RoomLayer.GroundLevel;
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, RoomMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<IRoomExit> path)
		{
			return new FollowingPath(character, path);
		}
	}

	private sealed class SwimmingMovementStrategy : ICreatureMovementStrategy
	{
		public static SwimmingMovementStrategy Instance { get; } = new();

		public bool RoomMatches(CreatureAIBase ai, ICharacter character, IRoom room)
		{
			return character.Race.CanSwim &&
			       RoomSupportsSwimming(character, room) &&
			       ai.MovementRoomProg.ExecuteBool(false, character, room, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer == RoomLayer.GroundLevel || targetLayer.IsUnderwater();
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, RoomMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<IRoomExit> path)
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

		public bool RoomMatches(CreatureAIBase ai, ICharacter character, IRoom room)
		{
			return ai.MovementRoomProg.ExecuteBool(false, character, room, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return !targetLayer.IsUnderwater();
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			return GetWeightedAmbientPath(ai, character, RoomMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<IRoomExit> path)
		{
			return new FollowingMultiLayerPath(character, path, ai.TargetFlyingLayer, ai.TargetRestingLayer);
		}
	}

	private sealed class ArborealMovementStrategy : ICreatureMovementStrategy
	{
		public static ArborealMovementStrategy Instance { get; } = new();

		public bool RoomMatches(CreatureAIBase ai, ICharacter character, IRoom room)
		{
			return ai.MovementRoomProg.ExecuteBool(false, character, room, character.Location) &&
			       (ArborealWandererAI.RoomSupportsTreeLayers(character, room) ||
			        ai.AllowDescentProg.ExecuteBool(false, character, room));
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer.In(RoomLayer.GroundLevel, RoomLayer.InTrees, RoomLayer.HighInTrees);
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			List<(IRoom Room, int Distance)> treeTargets = character.RoomsAndDistancesInVicinity(
					(uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true),
					room => ai.MovementRoomProg.ExecuteBool(false, character, room, character.Location) &&
					        ai.IsWithinPreferredHabitat(character, room) &&
					        ArborealWandererAI.RoomSupportsTreeLayers(character, room))
				.ToList();

			IRoom? target = treeTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Room;
			if (target is not null)
			{
				List<IRoomExit> path = character.PathBetween(target, (uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true)).ToList();
				if (path.Any())
				{
					return (target, path);
				}
			}

			List<(IRoom Room, int Distance)> descentTargets = character.RoomsAndDistancesInVicinity(
					(uint)ai.MovementRange,
					ai.GetAnimalSuitabilityFunction(character, true),
					room => ai.MovementRoomProg.ExecuteBool(false, character, room, character.Location) &&
					        ai.IsWithinPreferredHabitat(character, room) &&
					        !ArborealWandererAI.RoomSupportsTreeLayers(character, room) &&
					        ai.AllowDescentProg.ExecuteBool(false, character, room))
				.ToList();
			target = descentTargets.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Room;
			if (target is null)
			{
				return (null, Enumerable.Empty<IRoomExit>());
			}

			List<IRoomExit> descentPath = character.PathBetween(target, (uint)ai.MovementRange,
				ai.GetAnimalSuitabilityFunction(character, true)).ToList();
			return descentPath.Any()
				? (target, descentPath)
				: (null, Enumerable.Empty<IRoomExit>());
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<IRoomExit> path)
		{
			IRoom destination = path.Last().Destination;
			RoomLayer targetLayer = ChooseTreeLayer(ai, character, destination);
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}

		private static RoomLayer ChooseTreeLayer(CreatureAIBase ai, ICharacter character, IRoom room)
		{
			List<RoomLayer> layers = room.Terrain(character)?.TerrainLayers.ToList() ?? new List<RoomLayer>();
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

		public bool RoomMatches(CreatureAIBase ai, ICharacter character, IRoom room)
		{
			if (!ai.MovementRoomProg.ExecuteBool(false, character, room, character.Location))
			{
				return false;
			}

			return RoomSupportsSwimming(character, room)
				? ai.AmphibiousWaterRoomProg.ExecuteBool(false, character, room, character.Location)
				: ai.AmphibiousLandRoomProg.ExecuteBool(false, character, room, character.Location);
		}

		public bool CanReachTargetLayer(CreatureAIBase ai, ICharacter character, RoomLayer targetLayer)
		{
			return targetLayer == RoomLayer.GroundLevel || targetLayer.IsUnderwater();
		}

		public (IRoom? Target, IEnumerable<IRoomExit> Path) GetAmbientPath(CreatureAIBase ai, ICharacter character)
		{
			bool preferWater = RandomUtilities.DoubleRandom(0.0, 1.0) <= ai.AmphibiousWaterBias;
			(IRoom? target, IEnumerable<IRoomExit> path) = GetWeightedAmbientPath(ai, character,
				(_, ch, room) => RoomMatches(ai, ch, room) && RoomSupportsSwimming(ch, room) == preferWater);
			if (target is not null)
			{
				return (target, path);
			}

			return GetWeightedAmbientPath(ai, character, RoomMatches);
		}

		public FollowingPath CreatePathingEffect(CreatureAIBase ai, ICharacter character, IEnumerable<IRoomExit> path)
		{
			IRoom? destination = path.LastOrDefault()?.Destination;
			RoomLayer targetLayer = destination is not null && RoomSupportsSwimming(character, destination)
				? ai.WaterStrategy == AnimalWaterStrategyType.Surface ? RoomLayer.GroundLevel : character.RoomLayer
				: RoomLayer.GroundLevel;
			return new FollowingMultiLayerPath(character, path, targetLayer, targetLayer);
		}
	}

	internal static bool RoomSupportsSwimming(ICharacter character, IRoom room)
	{
		return room.IsSwimmingLayer(character.RoomLayer) ||
		       room.Terrain(character)?.TerrainLayers.Any(room.IsSwimmingLayer) == true;
	}

	internal static bool RoomSupportsSurfaceWater(ICharacter character, IRoom room)
	{
		return RoomSupportsSwimming(character, room) &&
		       room.Terrain(character)?.TerrainLayers.Any(x => !x.IsUnderwater()) == true;
	}

	private static (IRoom? Target, IEnumerable<IRoomExit> Path) GetWeightedAmbientPath(
		CreatureAIBase ai,
		ICharacter character,
		Func<CreatureAIBase, ICharacter, IRoom, bool> predicate)
	{
		List<(IRoom Room, int Distance)> vicinity = character.RoomsAndDistancesInVicinity(
				(uint)ai.MovementRange,
				ai.GetAnimalSuitabilityFunction(character, true),
				room => predicate(ai, character, room) && ai.IsWithinPreferredHabitat(character, room))
			.ToList();
		IRoom? target = vicinity.GetWeightedRandom(x => Math.Sqrt(x.Distance)).Room;
		if (target is null)
		{
			return (null, Enumerable.Empty<IRoomExit>());
		}

		List<IRoomExit> path = character.PathBetween(target, (uint)ai.MovementRange,
			ai.GetAnimalSuitabilityFunction(character, true)).ToList();
		return path.Any()
			? (path.Last().Destination, path)
			: (null, Enumerable.Empty<IRoomExit>());
	}}

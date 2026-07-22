#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using System.Text.Json;

namespace MudSharp.Movement;

public readonly record struct RouteMotionResourceCharge(
	ICharacter Character,
	double Amount,
	string ResourceKey);

public readonly record struct RouteMotionCheckpoint(
	Guid OperationId,
	long Sequence,
	double PositionMetres,
	TimeSpan RemainingDuration,
	IReadOnlyCollection<RouteMotionResourceCharge> Charges);

internal readonly record struct RouteCheckpointSaveState(
	ISaveable Saveable,
	ISaveManager SaveManager,
	bool WasChanged,
	bool WasQueued);

/// <summary>
/// Direct checkpoint saves must not drain the global save queue. Capture and restore only the
/// affected saveables so pre-existing unrelated work remains queued, while queue entries created
/// solely by the checkpoint are removed after its resource state is either committed or rolled back.
/// </summary>
internal static class RouteCheckpointSaveQueue
{
	public static IReadOnlyCollection<RouteCheckpointSaveState> Capture(IEnumerable<ISaveable> saveables)
	{
		return saveables
			.Distinct()
			.Select(x => (Saveable: x, SaveManager: x.Gameworld?.SaveManager))
			.Where(x => x.SaveManager is not null)
			.Select(x => new RouteCheckpointSaveState(
				x.Saveable,
				x.SaveManager!,
				x.Saveable.Changed,
				x.SaveManager!.IsQueued(x.Saveable)))
			.ToArray();
	}

	public static void Restore(
		IEnumerable<RouteCheckpointSaveState> states,
		Action<Exception>? reportFailure = null)
	{
		foreach (var state in states)
		{
			try
			{
				var saveManager = state.SaveManager;
				saveManager.Abort(state.Saveable);
				state.Saveable.Changed = false;
				if (state.WasChanged)
				{
					state.Saveable.Changed = true;
				}

				if (state.WasQueued && !saveManager.IsQueued(state.Saveable))
				{
					saveManager.Add(state.Saveable);
				}
				else if (!state.WasQueued && saveManager.IsQueued(state.Saveable))
				{
					saveManager.Abort(state.Saveable);
				}
			}
			catch (Exception exception)
			{
				reportFailure?.Invoke(exception);
			}
		}
	}

	public static void PreserveRollback(
		IEnumerable<RouteCheckpointSaveState> states,
		Action<Exception>? reportFailure = null)
	{
		foreach (var state in states)
		{
			try
			{
				if ((state.WasQueued || state.Saveable.Changed) &&
					!state.SaveManager.IsQueued(state.Saveable))
				{
					state.SaveManager.Add(state.Saveable);
				}
			}
			catch (Exception exception)
			{
				reportFailure?.Invoke(exception);
			}
		}
	}
}

/// <summary>
/// Persistence seam for durable linear motion. It exists separately from interpolation so
/// fake-clock tests can exercise movement without opening an EF context.
/// </summary>
public interface IRouteMotionPersistence
{
	void Start(
		ICharacter rootMover,
		LinearRouteMovementSegment segment,
		Guid operationId,
		double targetMinimumMetres,
		double targetMaximumMetres,
		long? selectedExitId,
		IReadOnlyCollection<ILocateable> cohort);

	void CommitCheckpoint(
		RouteMotionCheckpoint checkpoint,
		Action<IReadOnlyCollection<RouteMotionResourceCharge>> applyCharges);

	void Complete(Guid operationId);
}

/// <summary>
/// Production EF-backed persistence for active character RouteCell motion.
/// The callback is invoked only for newly-ledgered charges, and only affected bodies are saved
/// inside the same database transaction as the durable coordinate checkpoint.
/// </summary>
public sealed class DatabaseRouteMotionPersistence : IRouteMotionPersistence
{
	private const int CharacterMoverType = 0;
	private const int ActiveStatus = 0;
	private const int CharacterResourceOwnerType = 0;
	private const int StaminaResourceType = 0;
	private const int CommittedLedgerStatus = 1;

	public void Start(
		ICharacter rootMover,
		LinearRouteMovementSegment segment,
		Guid operationId,
		double targetMinimumMetres,
		double targetMaximumMetres,
		long? selectedExitId,
		IReadOnlyCollection<ILocateable> cohort)
	{
		ArgumentNullException.ThrowIfNull(rootMover);
		ArgumentNullException.ThrowIfNull(segment);
		var route = segment.Origin.Cell.RouteDefinition ??
		            throw new InvalidOperationException("Active route motion requires a RouteCell definition.");
		var operationKey = operationId.ToString("N");
		var now = DateTime.UtcNow;

		using (new FMDB())
		{
			var stale = FMDB.Context.ActiveRouteMotions
				.Where(x => x.MoverType == CharacterMoverType && x.MoverId == rootMover.Id)
				.ToList();
			if (stale.Count > 0)
			{
				FMDB.Context.ActiveRouteMotions.RemoveRange(stale);
			}

			FMDB.Context.ActiveRouteMotions.Add(new Models.ActiveRouteMotion
			{
				MoverType = CharacterMoverType,
				MoverId = rootMover.Id,
				RouteCellId = segment.Origin.Cell.Id,
				RoomLayer = (int)segment.Origin.Layer,
				CheckpointPositionMetres = ToMetres(segment.Origin.RoutePositionMetres!.Value),
				TargetMinimumPositionMetres = ToMetres(targetMinimumMetres),
				TargetMaximumPositionMetres = ToMetres(targetMaximumMetres),
				Direction = (int)segment.Direction,
				SpeedMetresPerSecond = Math.Round((decimal)segment.SpeedMetresPerSecond, 6,
					MidpointRounding.AwayFromZero),
				RemainingDurationMilliseconds = ToMilliseconds(segment.Duration),
				TopologyVersion = route.TopologyVersion,
				Status = ActiveStatus,
				OperationId = operationKey,
				CheckpointSequence = 0,
				SelectedExitId = selectedExitId,
				StateData = JsonSerializer.Serialize(new RouteMotionState(
					cohort.Select(ToParticipant).Where(x => x is not null).Cast<RouteMotionParticipant>().ToArray())),
				CreatedDateTime = now,
				LastCheckpointDateTime = now
			});
			FMDB.Context.SaveChanges();
		}
	}

	public void CommitCheckpoint(
		RouteMotionCheckpoint checkpoint,
		Action<IReadOnlyCollection<RouteMotionResourceCharge>> applyCharges)
	{
		ArgumentNullException.ThrowIfNull(applyCharges);
		var operationKey = checkpoint.OperationId.ToString("N");
		Exception? commitException = null;
		using (new FMDB())
		using (var transaction = FMDB.Context.Database.BeginTransaction())
		{
			var motion = FMDB.Context.ActiveRouteMotions
				.Include(x => x.ResourceLedger)
				.SingleOrDefault(x => x.OperationId == operationKey);
			if (motion is null)
			{
				throw new InvalidOperationException(
					$"Route motion operation {operationKey} has no active durable row for checkpoint {checkpoint.Sequence:N0}.");
			}

			if (checkpoint.Sequence == motion.CheckpointSequence)
			{
				ValidateIdempotentReplay(motion, checkpoint, operationKey);
				return;
			}

			if (checkpoint.Sequence < motion.CheckpointSequence)
			{
				throw new InvalidOperationException(
					$"Route motion operation {operationKey} rejected stale checkpoint {checkpoint.Sequence:N0}; the durable sequence is {motion.CheckpointSequence:N0}.");
			}

			if (checkpoint.Sequence != motion.CheckpointSequence + 1L)
			{
				throw new InvalidOperationException(
					$"Route motion operation {operationKey} rejected out-of-order checkpoint {checkpoint.Sequence:N0}; the next durable sequence is {motion.CheckpointSequence + 1L:N0}.");
			}

			motion.CheckpointPositionMetres = ToMetres(checkpoint.PositionMetres);
			motion.RemainingDurationMilliseconds = ToMilliseconds(checkpoint.RemainingDuration);
			motion.CheckpointSequence = checkpoint.Sequence;
			motion.LastCheckpointDateTime = DateTime.UtcNow;

			var committed = new List<RouteMotionResourceCharge>();
			foreach (var group in checkpoint.Charges
				         .Where(x => x.Amount > 0.0)
				         .GroupBy(x => x.Character.Id))
			{
				var charge = group.First();
				var idempotencyKey =
					$"{operationKey}:stamina:{charge.Character.Id}:{checkpoint.Sequence}";
				if (motion.ResourceLedger.Any(x => x.IdempotencyKey == idempotencyKey))
				{
					continue;
				}

				var amount = group.Sum(x =>
					Math.Round((decimal)x.Amount, 6, MidpointRounding.AwayFromZero));
				motion.ResourceLedger.Add(new Models.RouteMotionResourceLedger
				{
					CheckpointSequence = checkpoint.Sequence,
					IdempotencyKey = idempotencyKey,
					ResourceOwnerType = CharacterResourceOwnerType,
					ResourceOwnerId = charge.Character.Id,
					ResourceType = StaminaResourceType,
					ResourceKey = charge.ResourceKey,
					ReservedAmount = amount,
					ConsumedAmount = amount,
					Status = CommittedLedgerStatus,
					CreatedDateTime = DateTime.UtcNow,
					CommittedDateTime = DateTime.UtcNow
				});
				committed.AddRange(group);
			}

			applyCharges(committed);
			var bodies = committed
				.Select(x => x.Character.Body)
				.Distinct()
				.ToList();
			foreach (var body in bodies)
			{
				body.Save();
			}

			FMDB.Context.SaveChanges();
			try
			{
				transaction.Commit();
			}
			catch (Exception exception)
			{
				commitException = exception;
			}
		}

		if (commitException is null || IsCheckpointDurablyCommitted(checkpoint, operationKey))
		{
			return;
		}

		throw new InvalidOperationException(
			$"Route motion checkpoint {checkpoint.Sequence:N0} for operation {operationKey} could not be committed.",
			commitException);
	}

	private static bool IsCheckpointDurablyCommitted(RouteMotionCheckpoint checkpoint, string operationKey)
	{
		try
		{
			using (new FMDB())
			{
				var motion = FMDB.Context.ActiveRouteMotions
					.Include(x => x.ResourceLedger)
					.SingleOrDefault(x => x.OperationId == operationKey);
				if (motion is null || motion.CheckpointSequence != checkpoint.Sequence)
				{
					return false;
				}

				ValidateIdempotentReplay(motion, checkpoint, operationKey);
				return true;
			}
		}
		catch
		{
			return false;
		}
	}

	private static void ValidateIdempotentReplay(
		Models.ActiveRouteMotion motion,
		RouteMotionCheckpoint checkpoint,
		string operationKey)
	{
		var expected = checkpoint.Charges
			.Where(x => x.Amount > 0.0)
			.GroupBy(x => $"{operationKey}:stamina:{x.Character.Id}:{checkpoint.Sequence}")
			.ToDictionary(
				x => x.Key,
				x => x.Sum(y => Math.Round((decimal)y.Amount, 6, MidpointRounding.AwayFromZero)));
		var actual = motion.ResourceLedger
			.Where(x => x.CheckpointSequence == checkpoint.Sequence)
			.ToDictionary(x => x.IdempotencyKey, x => x.ConsumedAmount);
		var conflicts = motion.CheckpointPositionMetres != ToMetres(checkpoint.PositionMetres) ||
		                motion.RemainingDurationMilliseconds != ToMilliseconds(checkpoint.RemainingDuration) ||
		                expected.Count != actual.Count ||
		                expected.Any(x => !actual.TryGetValue(x.Key, out var amount) || amount != x.Value);
		if (conflicts)
		{
			throw new InvalidOperationException(
				$"Route motion idempotency conflict for operation {operationKey}, checkpoint {checkpoint.Sequence:N0}: the replay payload differs from the durable checkpoint.");
		}
	}

	public void Complete(Guid operationId)
	{
		var operationKey = operationId.ToString("N");
		using (new FMDB())
		{
			var motion = FMDB.Context.ActiveRouteMotions
				.SingleOrDefault(x => x.OperationId == operationKey);
			if (motion is null)
			{
				return;
			}

			ApplyCheckpointToParticipants(motion);
			FMDB.Context.ActiveRouteMotions.Remove(motion);
			FMDB.Context.SaveChanges();
		}
	}

	/// <summary>
	/// Crash recovery deliberately applies no elapsed downtime. It copies the last durable
	/// checkpoint to both persistent character representations and removes the interrupted motion.
	/// </summary>
	public static int FreezeInterruptedCharacterMotions()
	{
		using (new FMDB())
		{
			var motions = FMDB.Context.ActiveRouteMotions
				.Where(x => x.MoverType == CharacterMoverType)
				.ToList();
			foreach (var motion in motions)
			{
				ApplyCheckpointToParticipants(motion);
			}

			FMDB.Context.ActiveRouteMotions.RemoveRange(motions);
			FMDB.Context.SaveChanges();
			return motions.Count;
		}
	}

	private static decimal ToMetres(double metres)
	{
		return Math.Round((decimal)metres, 3, MidpointRounding.AwayFromZero);
	}

	private static long ToMilliseconds(TimeSpan duration)
	{
		return Math.Max(0L, (long)Math.Ceiling(duration.TotalMilliseconds));
	}

	private static RouteMotionParticipant? ToParticipant(ILocateable locateable)
	{
		return locateable switch
		{
			ICharacter character => new RouteMotionParticipant(
				"Character",
				CharacterInstanceIdentityComparer.IdentityId(character),
				CharacterInstanceIdentityComparer.InstanceId(character)),
			IGameItem item => new RouteMotionParticipant("GameItem", item.Id, null),
			_ => null
		};
	}

	private static void ApplyCheckpointToParticipants(Models.ActiveRouteMotion motion)
	{
		var participants = ReadParticipants(motion);
		foreach (var participant in participants)
		{
			switch (participant.Type)
			{
				case "Character":
					var character = FMDB.Context.Characters.Find(participant.Id);
					if (character is not null)
					{
						character.RoutePosition = motion.CheckpointPositionMetres;
					}

					var instances = FMDB.Context.CharacterInstances
						.Where(x => x.CharacterId == participant.Id);
					if (participant.InstanceId is > 0L)
					{
						instances = instances.Where(x => x.Id == participant.InstanceId.Value);
					}

					foreach (var instance in instances)
					{
						instance.RoutePosition = motion.CheckpointPositionMetres;
					}
					break;
				case "GameItem":
					var item = FMDB.Context.GameItems.Find(participant.Id);
					if (item is not null)
					{
						item.RoutePosition = motion.CheckpointPositionMetres;
					}
					break;
			}
		}
	}

	private static IReadOnlyCollection<RouteMotionParticipant> ReadParticipants(Models.ActiveRouteMotion motion)
	{
		if (!string.IsNullOrWhiteSpace(motion.StateData))
		{
			try
			{
				var state = JsonSerializer.Deserialize<RouteMotionState>(motion.StateData);
				if (state?.Participants is { Length: > 0 })
				{
					return state.Participants;
				}
			}
			catch (JsonException)
			{
				// Fall back to the root mover for older or damaged state payloads.
			}
		}

		return [new RouteMotionParticipant("Character", motion.MoverId, null)];
	}

	private sealed record RouteMotionState(RouteMotionParticipant[] Participants);
	private sealed record RouteMotionParticipant(string Type, long Id, long? InstanceId);
}

#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.Framework.Save;
using System.Text.Json;

namespace MudSharp.Vehicles;

public enum VehicleRouteResourceKind
{
	Stamina = 0,
	Fuel = 1,
	Power = 2
}

public sealed record VehicleRouteResourceCharge(
	int OwnerType,
	long OwnerId,
	VehicleRouteResourceKind ResourceKind,
	long? ResourceReferenceId,
	string ResourceKey,
	double Amount,
	IFuturemud Gameworld,
	IReadOnlyCollection<ISaveable> Saveables,
	Action Apply,
	Action Rollback);

public sealed record VehicleRouteMotionStart(
	Guid OperationId,
	IVehicle RootVehicle,
	LinearRouteMovementSegment Segment,
	double TargetMinimumMetres,
	double TargetMaximumMetres,
	long? SelectedExitId,
	IReadOnlyCollection<IVehicle> Vehicles,
	IReadOnlyCollection<IGameItem> Items,
	IReadOnlyCollection<ICharacter> Characters,
	long? JourneyId,
	long? LegId,
	int? StepSequence);

public sealed record VehicleRouteMotionCheckpoint(
	Guid OperationId,
	long Sequence,
	double PositionMetres,
	TimeSpan RemainingDuration,
	IReadOnlyCollection<VehicleRouteResourceCharge> Charges);

public interface IVehicleRouteMotionPersistence
{
	void Start(VehicleRouteMotionStart start);
	void CommitCheckpoint(
		VehicleRouteMotionCheckpoint checkpoint,
		Action<IReadOnlyCollection<VehicleRouteResourceCharge>> applyCharges);
	void Complete(Guid operationId);
}

/// <summary>
/// Durable checkpoint store for longitudinal vehicle movement. Coordinates and resource-ledger
/// reservations are committed in the same database transaction as the newly-ledgered resource
/// mutations, so a process failure cannot durably record a charge without durably consuming it.
/// </summary>
public sealed class DatabaseVehicleRouteMotionPersistence : IVehicleRouteMotionPersistence
{
	public const int VehicleMoverType = 1;
	private const int ActiveStatus = 0;
	private const int CommittedLedgerStatus = 1;

	public void Start(VehicleRouteMotionStart start)
	{
		ArgumentNullException.ThrowIfNull(start.RootVehicle);
		ArgumentNullException.ThrowIfNull(start.Segment);
		var route = start.Segment.Origin.Cell.RouteDefinition ??
		            throw new InvalidOperationException("Active vehicle route motion requires a RouteCell definition.");
		var operationKey = start.OperationId.ToString("N");
		var state = new VehicleRouteMotionState(
			start.JourneyId,
			start.LegId,
			start.StepSequence,
			start.Vehicles.Select(x => new VehicleRouteMotionParticipant("Vehicle", x.Id, null))
				.Concat(start.Items.Select(x => new VehicleRouteMotionParticipant("GameItem", x.Id, null)))
				.Concat(start.Characters.Select(x => new VehicleRouteMotionParticipant(
					"Character",
					CharacterInstanceIdentityComparer.IdentityId(x),
					CharacterInstanceIdentityComparer.InstanceId(x))))
				.Distinct()
				.ToArray());

		using (new FMDB())
		{
			var stale = FMDB.Context.ActiveRouteMotions
				.Where(x => x.MoverType == VehicleMoverType && x.MoverId == start.RootVehicle.Id)
				.ToList();
			if (stale.Count > 0)
			{
				foreach (var motion in stale)
				{
					ApplyCheckpointToParticipants(motion);
				}
				FMDB.Context.ActiveRouteMotions.RemoveRange(stale);
			}

			var now = DateTime.UtcNow;
			FMDB.Context.ActiveRouteMotions.Add(new Models.ActiveRouteMotion
			{
				MoverType = VehicleMoverType,
				MoverId = start.RootVehicle.Id,
				RouteCellId = start.Segment.Origin.Cell.Id,
				RoomLayer = (int)start.Segment.Origin.Layer,
				CheckpointPositionMetres = ToMetres(start.Segment.Origin.RoutePositionMetres!.Value),
				TargetMinimumPositionMetres = ToMetres(start.TargetMinimumMetres),
				TargetMaximumPositionMetres = ToMetres(start.TargetMaximumMetres),
				Direction = (int)start.Segment.Direction,
				SpeedMetresPerSecond = Math.Round((decimal)start.Segment.SpeedMetresPerSecond, 6,
					MidpointRounding.AwayFromZero),
				RemainingDurationMilliseconds = ToMilliseconds(start.Segment.Duration),
				TopologyVersion = route.TopologyVersion,
				Status = ActiveStatus,
				OperationId = operationKey,
				CheckpointSequence = 0,
				SelectedExitId = start.SelectedExitId,
				StateData = JsonSerializer.Serialize(state),
				CreatedDateTime = now,
				LastCheckpointDateTime = now
			});
			FMDB.Context.SaveChanges();
		}
	}

	public void CommitCheckpoint(
		VehicleRouteMotionCheckpoint checkpoint,
		Action<IReadOnlyCollection<VehicleRouteResourceCharge>> applyCharges)
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
					$"Vehicle route motion operation {operationKey} has no active durable row for checkpoint {checkpoint.Sequence:N0}.");
			}
			if (checkpoint.Sequence == motion.CheckpointSequence)
			{
				ValidateIdempotentReplay(motion, checkpoint, operationKey);
				return;
			}
			if (checkpoint.Sequence < motion.CheckpointSequence)
			{
				throw new InvalidOperationException(
					$"Vehicle route motion operation {operationKey} rejected stale checkpoint {checkpoint.Sequence:N0}; the durable sequence is {motion.CheckpointSequence:N0}.");
			}
			if (checkpoint.Sequence != motion.CheckpointSequence + 1L)
			{
				throw new InvalidOperationException(
					$"Vehicle route motion operation {operationKey} rejected out-of-order checkpoint {checkpoint.Sequence:N0}; the next durable sequence is {motion.CheckpointSequence + 1L:N0}.");
			}

			motion.CheckpointPositionMetres = ToMetres(checkpoint.PositionMetres);
			motion.RemainingDurationMilliseconds = ToMilliseconds(checkpoint.RemainingDuration);
			motion.CheckpointSequence = checkpoint.Sequence;
			motion.LastCheckpointDateTime = DateTime.UtcNow;
			ApplyCheckpointToParticipants(motion);

			var committed = new List<VehicleRouteResourceCharge>();
			foreach (var group in checkpoint.Charges
				         .Where(x => x.Amount > 0.0)
				         .GroupBy(x => new
				         {
					         x.ResourceKind,
					         x.OwnerType,
					         x.OwnerId,
					         x.ResourceReferenceId
				         }))
			{
				var charge = group.First();
				var idempotencyKey =
					$"{operationKey}:{charge.ResourceKind}:{charge.OwnerType}:{charge.OwnerId}:{charge.ResourceReferenceId}:{checkpoint.Sequence}";
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
					ResourceOwnerType = charge.OwnerType,
					ResourceOwnerId = charge.OwnerId,
					ResourceType = (int)charge.ResourceKind,
					ResourceReferenceId = charge.ResourceReferenceId,
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
			var saveables = committed
				.SelectMany(x => x.Saveables)
				.Distinct()
				.ToList();
			foreach (var saveable in saveables)
			{
				saveable.Save();
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
			$"Vehicle route motion checkpoint {checkpoint.Sequence:N0} for operation {operationKey} could not be committed.",
			commitException);
	}

	private static bool IsCheckpointDurablyCommitted(
		VehicleRouteMotionCheckpoint checkpoint,
		string operationKey)
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
		VehicleRouteMotionCheckpoint checkpoint,
		string operationKey)
	{
		var expected = checkpoint.Charges
			.Where(x => x.Amount > 0.0)
			.GroupBy(x => $"{operationKey}:{x.ResourceKind}:{x.OwnerType}:{x.OwnerId}:{x.ResourceReferenceId}:{checkpoint.Sequence}")
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
				$"Vehicle route motion idempotency conflict for operation {operationKey}, checkpoint {checkpoint.Sequence:N0}: the replay payload differs from the durable checkpoint.");
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
	/// Crash recovery freezes every vehicle at its last durable checkpoint. Journey scheduling
	/// subsequently resumes active journeys and accounts for downtime; no elapsed movement is applied.
	/// </summary>
	public static int FreezeInterruptedVehicleMotions()
	{
		using (new FMDB())
		{
			var motions = FMDB.Context.ActiveRouteMotions
				.Where(x => x.MoverType == VehicleMoverType)
				.ToList();
			foreach (var motion in motions)
			{
				ApplyCheckpointToParticipants(motion);
				var journeyId = ReadState(motion).JourneyId;
				if (journeyId.HasValue)
				{
					var journey = FMDB.Context.VehicleJourneys.Find(journeyId.Value);
					if (journey is not null && journey.LastCheckpointUtc < motion.LastCheckpointDateTime)
					{
						journey.LastCheckpointUtc = motion.LastCheckpointDateTime;
					}
				}
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

	private static VehicleRouteMotionState ReadState(Models.ActiveRouteMotion motion)
	{
		if (!string.IsNullOrWhiteSpace(motion.StateData))
		{
			try
			{
				var state = JsonSerializer.Deserialize<VehicleRouteMotionState>(motion.StateData);
				if (state is not null)
				{
					return state;
				}
			}
			catch (JsonException)
			{
				// Fall through to the root vehicle compatibility record.
			}
		}

		return new VehicleRouteMotionState(null, null, null,
			[new VehicleRouteMotionParticipant("Vehicle", motion.MoverId, null)]);
	}

	private static void ApplyCheckpointToParticipants(Models.ActiveRouteMotion motion)
	{
		foreach (var participant in ReadState(motion).Participants)
		{
			switch (participant.Type)
			{
				case "Vehicle":
					var vehicle = FMDB.Context.Vehicles.Find(participant.Id);
					if (vehicle is not null)
					{
						vehicle.CurrentRoutePosition = motion.CheckpointPositionMetres;
						vehicle.MovementStatus = (int)VehicleMovementStatus.Stationary;
						vehicle.LocationType = (int)VehicleLocationType.Route;
					}
					break;
				case "GameItem":
					var item = FMDB.Context.GameItems.Find(participant.Id);
					if (item is not null)
					{
						item.RoutePosition = motion.CheckpointPositionMetres;
					}
					break;
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
			}
		}
	}

	private sealed record VehicleRouteMotionState(
		long? JourneyId,
		long? LegId,
		int? StepSequence,
		VehicleRouteMotionParticipant[] Participants);

	private sealed record VehicleRouteMotionParticipant(string Type, long Id, long? InstanceId);
}

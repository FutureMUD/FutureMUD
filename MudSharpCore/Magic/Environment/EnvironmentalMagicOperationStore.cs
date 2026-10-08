#nullable enable

using Microsoft.EntityFrameworkCore;
using System.Data;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Magic.Generators;

namespace MudSharp.Magic.Environment;

public sealed record StoredEnvironmentalMagicOperation(long RoomId, EnvironmentalMagicOperationRequest Request,
	EnvironmentalMagicOperationResult Result);

public sealed record StoredEnvironmentalMagicState(EnvironmentalMagicState State,
	IReadOnlyDictionary<long, double> ResourceAmounts, bool HasState = true);

/// <summary>Explicit-operation persistence boundary; ordinary background work never calls this store.</summary>
public interface IEnvironmentalMagicOperationStore
{
	LandRejuvenationProgress? FindTreatment(Guid id) => throw new NotSupportedException("Treatment checkpoints are not supported by this store.");
	IReadOnlyList<LandRejuvenationProgress> TreatmentsFor(long cellId) => throw new NotSupportedException("Treatment checkpoints are not supported by this store.");
	void SaveTreatment(LandRejuvenationProgress progress, long? expectedRevision) => throw new NotSupportedException("Treatment checkpoints are not supported by this store.");
	void CommitRepair(Room room, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc, IReadOnlyDictionary<IMagicResource, double> balances,
		LandRejuvenationProgress progress, long expectedRevision) => throw new NotSupportedException("Atomic repair checkpoints are not supported by this store.");
	StoredEnvironmentalMagicOperation? Find(Guid operationId);
	StoredEnvironmentalMagicState Load(Room room);
	void Commit(Room room, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc,
		IReadOnlyDictionary<IMagicResource, double>? resourceAmounts = null);
}

public sealed partial class DatabaseEnvironmentalMagicOperationStore : IEnvironmentalMagicOperationStore
{
	public StoredEnvironmentalMagicOperation? Find(Guid operationId)
	{
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
			// A locking read waits for an uncertain transaction's receipt claim to commit or roll back.
			// An ordinary snapshot read could report absence while that claim is still in flight.
			var receipt = FMDB.Context.EnvironmentalMagicOperations
				.FromSqlInterpolated($"SELECT * FROM `EnvironmentalMagicOperations` WHERE `Id` = {operationId} FOR UPDATE")
				.AsNoTracking()
				.AsEnumerable()
				.SingleOrDefault();
			transaction.Commit();
			return receipt is null ? null : new(receipt.RoomId,
				new(receipt.Id, receipt.ActorId, receipt.Attribution, receipt.RequestedDamage, receipt.RequestedPressure, receipt.RequestedRepair),
				new(receipt.Id, receipt.Status == "Completed", true, receipt.AppliedDamage, receipt.AppliedPressure, receipt.AppliedRepair,
					string.IsNullOrEmpty(receipt.Diagnostic) ? null : receipt.Diagnostic));
		}
	}

	public StoredEnvironmentalMagicState Load(Room room)
	{
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			var dbcell = FMDB.Context.Rooms
				.AsNoTracking()
				.Include(x => x.EnvironmentalState)
				.Include(x => x.RoomsMagicResources)
				.AsSingleQuery()
				.Single(x => x.Id == room.Id);
			return new StoredEnvironmentalMagicState(Room.ReadEnvironmentState(dbcell.EnvironmentalState),
				dbcell.RoomsMagicResources.ToDictionary(x => x.MagicResourceId, x => x.Amount),
				dbcell.EnvironmentalState is not null);
		}
	}

	public void Commit(Room room, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc,
		IReadOnlyDictionary<IMagicResource, double>? resourceAmounts = null)
		=> CommitCore(room, request, result, state, atUtc, resourceAmounts, null, null);

	public void CommitRepair(Room room, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc, IReadOnlyDictionary<IMagicResource, double> balances,
		LandRejuvenationProgress progress, long expectedRevision)
		=> CommitCore(room, request, result, state, atUtc, balances, progress, expectedRevision);

	private void CommitCore(Room room, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc, IReadOnlyDictionary<IMagicResource, double>? resourceAmounts,
		LandRejuvenationProgress? progress, long? expectedProgressRevision)
	{
		var balances = room.MagicResourceAmounts.ToDictionary(x => x.Key, x => x.Value);
		if (resourceAmounts is not null)
			foreach (var balance in resourceAmounts) balances[balance.Key] = balance.Value;
		if (state.Revision <= room.EnvironmentState.Revision || state.Revision < 0 ||
			balances.Any(x => x.Key is null || !double.IsFinite(x.Value) || x.Value < 0.0))
			throw new ArgumentException("A new state revision and finite non-negative planned balances are required.");
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			var dbcell = FMDB.Context.Rooms.Include(x => x.EnvironmentalState).Include(x => x.RoomsMagicResources).Single(x => x.Id == room.Id);
			if (dbcell.EnvironmentalState?.Revision != room.ExpectedEnvironmentDatabaseRevision)
				throw new DbUpdateConcurrencyException("The room's environmental persistence revision changed. Reload before retrying the operation.");
			using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
			if (progress is not null)
			{
				var treatment = FMDB.Context.LandRejuvenationTreatments.Single(x => x.Id == progress.Id);
				var prepared = ReadTreatment(treatment);
				if (prepared.RoomId != room.Id || prepared.Revision != expectedProgressRevision ||
					prepared.PendingRequest != request || prepared.CancellationRequested ||
					progress.AcknowledgedSequence != prepared.Sequence || progress.PendingRequest is not null ||
					progress.RemainingBudget > prepared.RemainingBudget || progress.TotalRepaired < prepared.TotalRepaired)
					throw new DbUpdateConcurrencyException("The prepared repair checkpoint no longer matches this step.");
				WriteTreatment(progress, treatment);
			}
			FMDB.Context.EnvironmentalMagicOperations.Add(new Models.EnvironmentalMagicOperation
			{
				Id = request.OperationId,
				RoomId = room.Id,
				Kind = request.Repair > 0.0 ? "Repair" : "DestructiveDraw",
				RequestedDamage = request.Damage,
				RequestedPressure = request.Pressure,
				RequestedRepair = request.Repair,
				AppliedDamage = result.AppliedDamage,
				AppliedPressure = result.AppliedPressure,
				AppliedRepair = result.AppliedRepair,
				AtUtc = atUtc.UtcDateTime,
				ActorId = request.ActorId,
				Attribution = request.Attribution,
				Status = "Completed",
				Diagnostic = string.Empty
			});
			// Claim this identity before issuing any state/balance write. The receipt remains invisible
			// until this transaction commits, and Find waits on the claim when confirmation is uncertain.
			room.BeginEnvironmentalOperation(request.OperationId);
			FMDB.Context.SaveChanges();
			dbcell.EnvironmentalMagicBindingMode = (int)room.EnvironmentBindingMode;
			dbcell.EnvironmentalMagicProfileId = room.EnvironmentalMagicProfileId;
			if (room.EnvironmentBindingMode == EnvironmentalMagicBindingMode.Inherit && room.CurrentOverlay?.Terrain is { } terrain)
			{
				// An inherited binding is not durable without the current overlay/terrain choice that resolves it.
				var dboverlay = FMDB.Context.RoomOverlays.Find(room.CurrentOverlay.Id)
					?? throw new InvalidOperationException("The active overlay is not persisted; save its configuration before applying an environmental operation.");
				var dbterrain = FMDB.Context.Terrains.Find(terrain.Id)
					?? throw new InvalidOperationException("The effective terrain is not persisted; save its configuration before applying an environmental operation.");
				dbcell.CurrentOverlayId = dboverlay.Id;
				dboverlay.TerrainId = terrain.Id;
				dbterrain.EnvironmentalMagicProfileId = terrain.EnvironmentalMagicProfileId;
			}
			var effectiveProfileId = room.EnvironmentBindingMode switch
			{
				EnvironmentalMagicBindingMode.Explicit => room.EnvironmentalMagicProfileId,
				EnvironmentalMagicBindingMode.Inherit => room.CurrentOverlay?.Terrain?.EnvironmentalMagicProfileId,
				_ => null
			};
			foreach (var profileId in new[] { state.PressureProfileId, effectiveProfileId }.Where(x => x.HasValue).Distinct())
			{
				if (room.Gameworld.MagicResourceRegenerators.Get(profileId!.Value) is not EnvironmentalMagicGenerator profile) continue;
				var dbprofile = FMDB.Context.MagicGenerators.Find(profile.Id)
					?? throw new InvalidOperationException("An environmental profile is not persisted; save it before applying this operation.");
				// Export without clearing the runtime profile's deferred-save flag. The pressure anchor and
				// the cumulative decay timeline it refers to must either both commit or both roll back.
				dbprofile.Definition = profile.ExportDefinition();
			}
			dbcell.EnvironmentalState ??= new Models.RoomEnvironmentalState { RoomId = room.Id, Room = dbcell };
			Room.CopyEnvironmentState(state, dbcell.EnvironmentalState);
			foreach (var balance in balances)
			{
				var row = dbcell.RoomsMagicResources.FirstOrDefault(x => x.MagicResourceId == balance.Key.Id);
				if (row is null)
				{
					row = new Models.RoomMagicResource { Room = dbcell, MagicResourceId = balance.Key.Id };
					dbcell.RoomsMagicResources.Add(row);
				}
				row.Amount = balance.Value;
			}
			FMDB.Context.SaveChanges();
			transaction.Commit();
			// The coordinator adopts these confirmed values before lifting the room's write freeze.
		}
	}
}

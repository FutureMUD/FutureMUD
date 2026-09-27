#nullable enable

using Microsoft.EntityFrameworkCore;
using System.Data;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Magic.Generators;

namespace MudSharp.Magic.Environment;

public sealed record StoredEnvironmentalMagicOperation(long CellId, EnvironmentalMagicOperationRequest Request,
	EnvironmentalMagicOperationResult Result);

public sealed record StoredEnvironmentalMagicState(EnvironmentalMagicState State,
	IReadOnlyDictionary<long, double> ResourceAmounts, bool HasState = true);

/// <summary>Explicit-operation persistence boundary; ordinary background work never calls this store.</summary>
public interface IEnvironmentalMagicOperationStore
{
	LandRejuvenationProgress? FindTreatment(Guid id) => throw new NotSupportedException("Treatment checkpoints are not supported by this store.");
	IReadOnlyList<LandRejuvenationProgress> TreatmentsFor(long cellId) => throw new NotSupportedException("Treatment checkpoints are not supported by this store.");
	void SaveTreatment(LandRejuvenationProgress progress, long? expectedRevision) => throw new NotSupportedException("Treatment checkpoints are not supported by this store.");
	void CommitRepair(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc, IReadOnlyDictionary<IMagicResource, double> balances,
		LandRejuvenationProgress progress, long expectedRevision) => throw new NotSupportedException("Atomic repair checkpoints are not supported by this store.");
	StoredEnvironmentalMagicOperation? Find(Guid operationId);
	StoredEnvironmentalMagicState Load(Cell cell);
	void Commit(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
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
			return receipt is null ? null : new(receipt.CellId,
				new(receipt.Id, receipt.ActorId, receipt.Attribution, receipt.RequestedDamage, receipt.RequestedPressure, receipt.RequestedRepair),
				new(receipt.Id, receipt.Status == "Completed", true, receipt.AppliedDamage, receipt.AppliedPressure, receipt.AppliedRepair,
					string.IsNullOrEmpty(receipt.Diagnostic) ? null : receipt.Diagnostic));
		}
	}

	public StoredEnvironmentalMagicState Load(Cell cell)
	{
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			var dbcell = FMDB.Context.Cells
				.AsNoTracking()
				.Include(x => x.EnvironmentalState)
				.Include(x => x.CellsMagicResources)
				.AsSingleQuery()
				.Single(x => x.Id == cell.Id);
			return new StoredEnvironmentalMagicState(Cell.ReadEnvironmentState(dbcell.EnvironmentalState),
				dbcell.CellsMagicResources.ToDictionary(x => x.MagicResourceId, x => x.Amount),
				dbcell.EnvironmentalState is not null);
		}
	}

	public void Commit(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc,
		IReadOnlyDictionary<IMagicResource, double>? resourceAmounts = null)
		=> CommitCore(cell, request, result, state, atUtc, resourceAmounts, null, null);

	public void CommitRepair(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc, IReadOnlyDictionary<IMagicResource, double> balances,
		LandRejuvenationProgress progress, long expectedRevision)
		=> CommitCore(cell, request, result, state, atUtc, balances, progress, expectedRevision);

	private void CommitCore(Cell cell, EnvironmentalMagicOperationRequest request, EnvironmentalMagicOperationResult result,
		EnvironmentalMagicState state, DateTimeOffset atUtc, IReadOnlyDictionary<IMagicResource, double>? resourceAmounts,
		LandRejuvenationProgress? progress, long? expectedProgressRevision)
	{
		var balances = cell.MagicResourceAmounts.ToDictionary(x => x.Key, x => x.Value);
		if (resourceAmounts is not null)
			foreach (var balance in resourceAmounts) balances[balance.Key] = balance.Value;
		if (state.Revision <= cell.EnvironmentState.Revision || state.Revision < 0 ||
			balances.Any(x => x.Key is null || !double.IsFinite(x.Value) || x.Value < 0.0))
			throw new ArgumentException("A new state revision and finite non-negative planned balances are required.");
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			var dbcell = FMDB.Context.Cells.Include(x => x.EnvironmentalState).Include(x => x.CellsMagicResources).Single(x => x.Id == cell.Id);
			if (dbcell.EnvironmentalState?.Revision != cell.ExpectedEnvironmentDatabaseRevision)
				throw new DbUpdateConcurrencyException("The cell's environmental persistence revision changed. Reload before retrying the operation.");
			using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
			if (progress is not null)
			{
				var treatment = FMDB.Context.LandRejuvenationTreatments.Single(x => x.Id == progress.Id);
				var prepared = ReadTreatment(treatment);
				if (prepared.CellId != cell.Id || prepared.Revision != expectedProgressRevision ||
					prepared.PendingRequest != request || prepared.CancellationRequested ||
					progress.AcknowledgedSequence != prepared.Sequence || progress.PendingRequest is not null ||
					progress.RemainingBudget > prepared.RemainingBudget || progress.TotalRepaired < prepared.TotalRepaired)
					throw new DbUpdateConcurrencyException("The prepared repair checkpoint no longer matches this step.");
				WriteTreatment(progress, treatment);
			}
			FMDB.Context.EnvironmentalMagicOperations.Add(new Models.EnvironmentalMagicOperation
			{
				Id = request.OperationId,
				CellId = cell.Id,
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
			cell.BeginEnvironmentalOperation(request.OperationId);
			FMDB.Context.SaveChanges();
			dbcell.EnvironmentalMagicBindingMode = (int)cell.EnvironmentBindingMode;
			dbcell.EnvironmentalMagicProfileId = cell.EnvironmentalMagicProfileId;
			if (cell.EnvironmentBindingMode == EnvironmentalMagicBindingMode.Inherit && cell.CurrentOverlay?.Terrain is { } terrain)
			{
				// An inherited binding is not durable without the current overlay/terrain choice that resolves it.
				var dboverlay = FMDB.Context.CellOverlays.Find(cell.CurrentOverlay.Id)
					?? throw new InvalidOperationException("The active overlay is not persisted; save its configuration before applying an environmental operation.");
				var dbterrain = FMDB.Context.Terrains.Find(terrain.Id)
					?? throw new InvalidOperationException("The effective terrain is not persisted; save its configuration before applying an environmental operation.");
				dbcell.CurrentOverlayId = dboverlay.Id;
				dboverlay.TerrainId = terrain.Id;
				dbterrain.EnvironmentalMagicProfileId = terrain.EnvironmentalMagicProfileId;
			}
			var effectiveProfileId = cell.EnvironmentBindingMode switch
			{
				EnvironmentalMagicBindingMode.Explicit => cell.EnvironmentalMagicProfileId,
				EnvironmentalMagicBindingMode.Inherit => cell.CurrentOverlay?.Terrain?.EnvironmentalMagicProfileId,
				_ => null
			};
			foreach (var profileId in new[] { state.PressureProfileId, effectiveProfileId }.Where(x => x.HasValue).Distinct())
			{
				if (cell.Gameworld.MagicResourceRegenerators.Get(profileId!.Value) is not EnvironmentalMagicGenerator profile) continue;
				var dbprofile = FMDB.Context.MagicGenerators.Find(profile.Id)
					?? throw new InvalidOperationException("An environmental profile is not persisted; save it before applying this operation.");
				// Export without clearing the runtime profile's deferred-save flag. The pressure anchor and
				// the cumulative decay timeline it refers to must either both commit or both roll back.
				dbprofile.Definition = profile.ExportDefinition();
			}
			dbcell.EnvironmentalState ??= new Models.CellEnvironmentalState { CellId = cell.Id, Cell = dbcell };
			Cell.CopyEnvironmentState(state, dbcell.EnvironmentalState);
			foreach (var balance in balances)
			{
				var row = dbcell.CellsMagicResources.FirstOrDefault(x => x.MagicResourceId == balance.Key.Id);
				if (row is null)
				{
					row = new Models.CellMagicResource { Cell = dbcell, MagicResourceId = balance.Key.Id };
					dbcell.CellsMagicResources.Add(row);
				}
				row.Amount = balance.Value;
			}
			FMDB.Context.SaveChanges();
			transaction.Commit();
			// The coordinator adopts these confirmed values before lifting the cell's write freeze.
		}
	}
}

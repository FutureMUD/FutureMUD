#nullable enable

using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace MudSharp.Magic.Environment;

public sealed partial class DatabaseEnvironmentalMagicOperationStore
{
	internal static LandRejuvenationProgress ReadTreatment(Models.LandRejuvenationTreatment row)
	{
		var value = JsonSerializer.Deserialize<LandRejuvenationProgress>(row.Checkpoint)
			?? throw new InvalidOperationException($"Treatment {row.Id} has no checkpoint.");
		if (value.Id != row.Id || value.RoomId != row.RoomId || value.Revision != row.Revision ||
			value.Status.ToString() != row.Status)
			throw new InvalidOperationException($"Treatment {row.Id} has inconsistent checkpoint identity.");
		return value;
	}

	private static void WriteTreatment(LandRejuvenationProgress value, Models.LandRejuvenationTreatment row)
	{
		row.Id = value.Id;
		row.RoomId = value.RoomId;
		row.Revision = value.Revision;
		row.Status = value.Status.ToString();
		row.Checkpoint = JsonSerializer.Serialize(value);
	}

	public LandRejuvenationProgress? FindTreatment(Guid id)
	{
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			using var transaction = FMDB.Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
			var row = FMDB.Context.LandRejuvenationTreatments
				.FromSqlInterpolated($"SELECT * FROM `LandRejuvenationTreatments` WHERE `Id` = {id} FOR UPDATE")
				.AsNoTracking().AsEnumerable().SingleOrDefault();
			var result = row is null ? null : ReadTreatment(row);
			transaction.Commit();
			return result;
		}
	}

	public IReadOnlyList<LandRejuvenationProgress> TreatmentsFor(long cellId)
	{
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			return FMDB.Context.LandRejuvenationTreatments.AsNoTracking().Where(x => x.RoomId == cellId)
				.AsEnumerable().Select(ReadTreatment).ToArray();
		}
	}

	public IReadOnlyList<LandRejuvenationProgress> UnresolvedTreatmentsFor(long cellId)
	{
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			// Valid checkpoints with an unacknowledged request always have Pending status.
			// Include unknown statuses so malformed nonterminal records still fail closed.
			return FMDB.Context.LandRejuvenationTreatments.AsNoTracking()
				.Where(x => x.RoomId == cellId &&
					x.Status != nameof(LandRejuvenationStatus.Completed) &&
					x.Status != nameof(LandRejuvenationStatus.Cancelled) &&
					x.Status != nameof(LandRejuvenationStatus.Faulted))
				.AsEnumerable()
				.Select(ReadTreatment)
				.ToArray();
		}
	}

	public void SaveTreatment(LandRejuvenationProgress progress, long? expectedRevision)
	{
		using var isolated = FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			var row = FMDB.Context.LandRejuvenationTreatments.SingleOrDefault(x => x.Id == progress.Id);
			if (row?.Revision != expectedRevision || progress.Revision != (expectedRevision ?? -1) + 1)
				throw new DbUpdateConcurrencyException("The treatment checkpoint revision changed.");
			if (row is null)
			{
				row = new Models.LandRejuvenationTreatment();
				FMDB.Context.LandRejuvenationTreatments.Add(row);
			}
			WriteTreatment(progress, row);
			FMDB.Context.SaveChanges();
		}
	}
}

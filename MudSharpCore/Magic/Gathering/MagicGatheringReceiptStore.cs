#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace MudSharp.Magic.Gathering;

/// <summary>
/// Durable receipt state only. It intentionally has no replay, compensating-credit or source-refund operation.
/// </summary>
public sealed record MagicGatheringReceipt(
	Guid Id,
	long OwnerId,
	long ActorId,
	long BodyId,
	long CapabilityId,
	Guid MethodKey,
	int MethodVersion,
	MagicGatheringMethodKind Kind,
	long? CellId,
	long? SourceProfileId,
	long? SourceProfileRevision,
	long? SourceResourceId,
	long DestinationResourceId,
	double RequestedAmount,
	double SourceDebit,
	double StaminaCost,
	double DamageCost,
	double PainCost,
	double StunCost,
	bool SourceDebited,
	bool BodilyCostApplied,
	bool DestinationCredited,
	bool AccountingPersisted,
	bool NotificationCompleted,
	string Status,
	DateTime CreatedUtc,
	string Diagnostic = "")
{
	public DateTime UpdatedUtc { get; init; } = CreatedUtc;
	public MagicGatheringOperationSummary Summary() => new(Id, OwnerId, ActorId, BodyId, CapabilityId, MethodKey, MethodVersion,
		Kind.ToString(), Status, RequestedAmount, SourceDebit, StaminaCost, DamageCost, PainCost, StunCost,
		SourceDebited, BodilyCostApplied, DestinationCredited, AccountingPersisted, NotificationCompleted,
		CreatedUtc, UpdatedUtc, Diagnostic);
}

public interface IMagicGatheringReceiptStore
{
	bool TryCreate(MagicGatheringReceipt receipt);
	void Record(MagicGatheringReceipt receipt);
	MagicGatheringReceipt? Operation(Guid id);
	IReadOnlyList<MagicGatheringReceipt> Unresolved(long? ownerId = null);
	bool HasUnresolved(long ownerId);
	bool HasUnresolvedForSource(long cellId, long sourceResourceId);
}

public sealed class MagicGatheringReceiptStore : IMagicGatheringReceiptStore
{
	private static readonly string[] UnresolvedStatuses = ["Committing", "Invoking", "NeedsReview"];

	public bool TryCreate(MagicGatheringReceipt receipt)
	{
		using IDisposable? isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			if (FMDB.Context.MagicGatheringOperations.AsNoTracking().Any(x => x.Id == receipt.Id))
			{
				return false;
			}

			try
			{
				WriteCurrent(receipt);
				FMDB.Context.SaveChanges();
				return true;
			}
			catch (DbUpdateException)
			{
				return false;
			}
		}
	}

	public void Record(MagicGatheringReceipt receipt)
	{
		using IDisposable? isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			WriteCurrent(receipt);
			FMDB.Context.SaveChanges();
		}
	}

	public MagicGatheringReceipt? Operation(Guid id)
	{
		using (new FMDB())
		{
			return FMDB.Context.MagicGatheringOperations.AsNoTracking().SingleOrDefault(x => x.Id == id) is { } row
				? FromRow(row)
				: null;
		}
	}

	public IReadOnlyList<MagicGatheringReceipt> Unresolved(long? ownerId = null)
	{
		using (new FMDB())
		{
			return FMDB.Context.MagicGatheringOperations.AsNoTracking()
				.Where(x => (!ownerId.HasValue || x.OwnerId == ownerId.Value) && UnresolvedStatuses.Contains(x.Status))
				.OrderByDescending(x => x.UpdatedUtc)
				.Take(1000)
				.AsEnumerable()
				.Select(FromRow)
				.ToArray();
		}
	}

	public bool HasUnresolved(long ownerId)
	{
		using (new FMDB())
		{
			return FMDB.Context.MagicGatheringOperations.AsNoTracking()
				.Any(x => x.OwnerId == ownerId && UnresolvedStatuses.Contains(x.Status));
		}
	}

	public bool HasUnresolvedForSource(long cellId, long sourceResourceId)
	{
		using (new FMDB())
		{
			return FMDB.Context.MagicGatheringOperations.AsNoTracking()
				.Any(x => x.CellId == cellId && x.SourceResourceId == sourceResourceId &&
					UnresolvedStatuses.Contains(x.Status));
		}
	}

	internal static void WriteCurrent(MagicGatheringReceipt receipt)
	{
		Models.MagicGatheringOperation? row = FMDB.Context.MagicGatheringOperations.Find(receipt.Id);
		if (row is null)
		{
			row = new Models.MagicGatheringOperation { Id = receipt.Id };
			FMDB.Context.MagicGatheringOperations.Add(row);
		}

		row.OwnerId = receipt.OwnerId;
		row.ActorId = receipt.ActorId;
		row.BodyId = receipt.BodyId;
		row.MagicCapabilityId = receipt.CapabilityId;
		row.MethodKey = receipt.MethodKey;
		row.MethodVersion = receipt.MethodVersion;
		row.CellId = receipt.CellId;
		row.SourceProfileId = receipt.SourceProfileId;
		row.SourceProfileRevision = receipt.SourceProfileRevision;
		row.SourceResourceId = receipt.SourceResourceId;
		row.DestinationResourceId = receipt.DestinationResourceId;
		row.Kind = receipt.Kind.ToString();
		row.RequestedAmount = receipt.RequestedAmount;
		row.SourceDebit = receipt.SourceDebit;
		row.StaminaCost = receipt.StaminaCost;
		row.DamageCost = receipt.DamageCost;
		row.PainCost = receipt.PainCost;
		row.StunCost = receipt.StunCost;
		row.SourceDebited = receipt.SourceDebited;
		row.BodilyCostApplied = receipt.BodilyCostApplied;
		row.DestinationCredited = receipt.DestinationCredited;
		row.AccountingPersisted = receipt.AccountingPersisted;
		row.NotificationCompleted = receipt.NotificationCompleted;
		row.Status = receipt.Status;
		row.CreatedUtc = receipt.CreatedUtc;
		row.UpdatedUtc = receipt.UpdatedUtc;
		row.Diagnostic = receipt.Diagnostic;
	}

	private static MagicGatheringReceipt FromRow(Models.MagicGatheringOperation row) => new(
		row.Id,
		row.OwnerId,
		row.ActorId,
		row.BodyId,
		row.MagicCapabilityId,
		row.MethodKey,
		row.MethodVersion,
		Enum.TryParse(row.Kind, true, out MagicGatheringMethodKind kind) ? kind : MagicGatheringMethodKind.Self,
		row.CellId,
		row.SourceProfileId,
		row.SourceProfileRevision,
		row.SourceResourceId,
		row.DestinationResourceId,
		row.RequestedAmount,
		row.SourceDebit,
		row.StaminaCost,
		row.DamageCost,
		row.PainCost,
		row.StunCost,
		row.SourceDebited,
		row.BodilyCostApplied,
		row.DestinationCredited,
		row.AccountingPersisted,
		row.NotificationCompleted,
		row.Status,
		DateTime.SpecifyKind(row.CreatedUtc, DateTimeKind.Utc),
		row.Diagnostic)
	{
		UpdatedUtc = DateTime.SpecifyKind(row.UpdatedUtc, DateTimeKind.Utc)
	};
}

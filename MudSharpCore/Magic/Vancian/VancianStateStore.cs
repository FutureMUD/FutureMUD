using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed record VancianOperation(Guid Id, long OwnerId, long CapabilityId, string Kind, string Status,
	long ExpectedVersion, string Payload, DateTime CreatedUtc, long? SourceItem = null, long? DestinationItem = null, string Diagnostic = "");

public interface IVancianStateStore
{
	VancianCapabilityState Read(long owner, long capability);
	void Commit(VancianCapabilityState state, long expectedVersion, VancianOperation? operation = null);
	IReadOnlyList<VancianOperation> Operations(long owner, long? capability = null);
	bool HasUnresolved(long owner, long capability, params string[] kinds);
	VancianOperation? Operation(Guid id);
	void Record(VancianOperation operation);
	bool ItemConsumed(long itemId, Guid chargeId);
	bool ClaimCharge(VancianOperation operation);
}

/// <summary>Immediate, optimistic writes, independent of the periodic character saver.</summary>
public sealed class VancianStateStore : IVancianStateStore
{
	public bool HasUnresolved(long owner, long capability, params string[] kinds)
	{
		using (new FMDB()) return FMDB.Context.VancianMagicOperations.AsNoTracking().Any(x => x.CharacterId == owner && x.MagicCapabilityId == capability &&
			(kinds.Length == 0 || kinds.Contains(x.Kind)) && (x.Status == "Pending" || x.Status == "Invoking" || x.Status == "NeedsReview" || x.Status == "Committing" || x.Status == "Reserved" ||
				x.Status == "Consumed" && (x.Kind == "Cast" || x.Kind == "ScrollActivation")));
	}
	public bool ClaimCharge(VancianOperation operation)
	{
		using var isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			if (FMDB.Context.VancianMagicOperations.AsNoTracking().Any(x => x.Id == operation.Id)) return false;
			var row = new Models.VancianMagicOperation { Id = operation.Id, CharacterId = operation.OwnerId,
				MagicCapabilityId = operation.CapabilityId, Kind = operation.Kind, Status = "Consumed",
				ExpectedStateVersion = operation.ExpectedVersion, Definition = operation.Payload, CreatedUtc = operation.CreatedUtc,
				UpdatedUtc = operation.CreatedUtc, SourceItemId = operation.SourceItem, DestinationItemId = operation.DestinationItem, Diagnostic = operation.Diagnostic };
			FMDB.Context.VancianMagicOperations.Add(row);
			try { FMDB.Context.SaveChanges(); return true; }
			catch (DbUpdateException) { FMDB.Context.Entry(row).State = EntityState.Detached; return false; }
		}
	}
	public VancianCapabilityState Read(long owner, long capability)
	{
		using (new FMDB())
		{
			var row = FMDB.Context.CharacterMagicCapabilityStates.AsNoTracking().SingleOrDefault(x => x.CharacterId == owner && x.MagicCapabilityId == capability);
			return VancianCapabilityState.Load(owner, capability, row?.StateVersion ?? 0, row?.Definition);
		}
	}

	public void Commit(VancianCapabilityState state, long expectedVersion, VancianOperation? operation = null)
	{
		var xml = state.Save().ToString(SaveOptions.DisableFormatting);
		// The ledger transaction must not flush unrelated periodic saves or inherit their rollback.
		using var isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
		using (new FMDB())
		{
			using var transaction = FMDB.Context.Database.BeginTransaction();
			var row = FMDB.Context.CharacterMagicCapabilityStates.Find(state.OwnerId, state.CapabilityId);
			if ((row?.StateVersion ?? 0) != expectedVersion) throw new InvalidOperationException("This state changed in another operation. Restart the request.");
			if (row is null)
			{
				row = new Models.CharacterMagicCapabilityState { CharacterId = state.OwnerId, MagicCapabilityId = state.CapabilityId };
				FMDB.Context.CharacterMagicCapabilityStates.Add(row);
			}
			row.StateVersion = checked(expectedVersion + 1);
			row.Definition = xml;
			if (operation is not null) WriteOperation(operation);
			FMDB.Context.SaveChanges();
			transaction.Commit();
			state.Version = row.StateVersion;
			FMDB.Context.Entry(row).State = EntityState.Detached;
		}
	}

	public IReadOnlyList<VancianOperation> Operations(long owner, long? capability = null)
	{
		using (new FMDB()) return FMDB.Context.VancianMagicOperations.AsNoTracking()
			.Where(x => x.CharacterId == owner && (!capability.HasValue || x.MagicCapabilityId == capability.Value))
			.OrderByDescending(x => x.CreatedUtc).Take(1000).AsEnumerable().Select(FromRow).ToArray();
	}
	public VancianOperation? Operation(Guid id)
	{
		using (new FMDB()) return FMDB.Context.VancianMagicOperations.AsNoTracking().SingleOrDefault(x => x.Id == id) is { } row ? FromRow(row) : null;
	}
	public void Record(VancianOperation operation)
	{
		using var isolated = FMDB.IsIsolated ? null : FMDB.BeginIsolatedScope();
		using (new FMDB()) { WriteOperation(operation); FMDB.Context.SaveChanges(); }
	}
	public bool ItemConsumed(long itemId, Guid chargeId)
	{
		using (new FMDB()) return FMDB.Context.VancianMagicOperations.AsNoTracking().Any(x => x.Id == chargeId && x.SourceItemId == itemId &&
			(x.Status == "Committing" || x.Status == "Consumed" || x.Status == "Completed" || x.Status == "NeedsReview"));
	}
	private static VancianOperation FromRow(Models.VancianMagicOperation row) => new(row.Id, row.CharacterId, row.MagicCapabilityId,
		row.Kind, row.Status, row.ExpectedStateVersion, row.Definition, DateTime.SpecifyKind(row.CreatedUtc, DateTimeKind.Utc),
		row.SourceItemId, row.DestinationItemId, row.Diagnostic);
	private static void WriteOperation(VancianOperation operation)
	{
		var row = FMDB.Context.VancianMagicOperations.Find(operation.Id);
		if (row is null) { row = new Models.VancianMagicOperation { Id = operation.Id }; FMDB.Context.VancianMagicOperations.Add(row); }
		row.CharacterId = operation.OwnerId; row.MagicCapabilityId = operation.CapabilityId;
		row.Kind = operation.Kind; row.Status = operation.Status; row.ExpectedStateVersion = operation.ExpectedVersion;
		row.Definition = operation.Payload; row.CreatedUtc = operation.CreatedUtc; row.UpdatedUtc = DateTime.UtcNow;
		row.SourceItemId = operation.SourceItem; row.DestinationItemId = operation.DestinationItem; row.Diagnostic = operation.Diagnostic;
	}
}

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

#nullable enable
namespace MudSharp.Magic.Casting;

/// <summary>Insert-only claim: an existing charge receipt, including a terminal tombstone, is never replaced.</summary>
internal static class MagicDeviceJournal
{
	internal static bool TryClaim(CastingOperation operation)
	{
		if (FMDB.WritesAreSuppressed) throw new InvalidOperationException("Device claims cannot escape a read-only database scope.");
		using var isolated = FMDB.BeginIndependentScope();
		using var db = new FMDB();
		FMDB.Context.MagicCastingOperations.Add(new()
		{
			Id = operation.Id, CharacterId = operation.CharacterId, ActorId = operation.ActorId, BodyId = operation.BodyId,
			MagicCapabilityId = operation.CapabilityId, MagicSpellId = operation.SpellId, TraitDefinitionId = operation.TraitId,
			ReserveId = operation.ReserveId, Stage = operation.Stage, Definition = operation.Definition,
			CreatedUtc = operation.CreatedUtc, UpdatedUtc = operation.UpdatedUtc, Diagnostic = operation.Diagnostic
		});
		try { FMDB.Context.SaveChanges(); return true; }
		catch (DbUpdateException ex) when (ex.GetBaseException() is MySql.Data.MySqlClient.MySqlException { Number: 1062 } or MySqlConnector.MySqlException { Number: 1062 })
		{
			return false;
		}
	}
}

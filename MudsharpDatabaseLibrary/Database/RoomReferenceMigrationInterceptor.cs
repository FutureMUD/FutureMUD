#nullable enable

using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace MudSharp.Database;

/// <summary>Managed XML correction for both startup and direct EF migration APIs. Raw SQL fails closed.</summary>
internal sealed class RoomReferenceMigrationInterceptor : DbCommandInterceptor
{
	internal const string Expansion = "-- FutureMUD.RoomReferences.Expansion\nSIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room reference correction requires managed FutureMUD EF migrations';";
	internal const string Contraction = "-- FutureMUD.RoomReferences.Contraction\nSIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room reference correction requires managed FutureMUD EF migrations';";
	internal const string Verify = "-- FutureMUD.RoomReferences.Verify\nSIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room reference verification requires managed FutureMUD EF migrations';";
	internal const string Repair = "-- FutureMUD.RoomReferences.Repair\nSIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Room reference correction requires managed FutureMUD EF migrations';";

	internal static string? Marker(DbCommand command, CommandSource source)
	{
		if (source != CommandSource.Migrations || command.CommandType != CommandType.Text) return null;
		return command.CommandText.Trim() switch
		{
			Expansion => Expansion,
			Contraction => Contraction,
			Verify => Verify,
			Repair => Repair,
			_ => null
		};
	}

	public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
	{
		var marker = Marker(command, eventData.CommandSource);
		if (marker is null) return result;
		RoomReferenceMigrationRunner.ExecuteAsync(command, marker, CancellationToken.None).GetAwaiter().GetResult();
		return InterceptionResult<int>.SuppressWithResult(0);
	}

	public override async ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
	{
		var marker = Marker(command, eventData.CommandSource);
		if (marker is null) return result;
		await RoomReferenceMigrationRunner.ExecuteAsync(command, marker, cancellationToken).ConfigureAwait(false);
		return InterceptionResult<int>.SuppressWithResult(0);
	}
}

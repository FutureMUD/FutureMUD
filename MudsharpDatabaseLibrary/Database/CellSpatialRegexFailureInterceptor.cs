#nullable enable

using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySqlConnector;

namespace MudSharp.Database;

/// <summary>Best-effort context for the expansion preflight's prepared regex scan.</summary>
internal sealed class CellSpatialRegexFailureInterceptor : DbCommandInterceptor
{
	internal const string ExpansionCall = "CALL `fm_cell_spatial_preflight_20261006143539`();";
	internal const string Migration = "20261006143539_CellSpatialExpansion";
	internal const string TypePattern = "^[[:space:]]*Room[[:space:]]*$";
	internal const string DefinitionPattern = "(Type[\"']?[[:space:]]*[:=][[:space:]]*[\"']Room[\"']|<([[:alnum:]_]*Type)>[[:space:]]*Room[[:space:]]*</)";
	internal const string DiagnosticSelect = "SELECT CONNECTION_ID(), LEFT(@fm_cell_spatial_query,2048), LEFT(@fm_cell_spatial_pattern,512), CHAR_LENGTH(@fm_cell_spatial_query), CHAR_LENGTH(@fm_cell_spatial_pattern)";
	private const string ContextKey = "FutureMUD.CellSpatialRegexFailure";
	private static readonly Regex ScanQuery = new(
		"\\ASELECT COUNT\\(\\*\\) INTO @fm_cell_spatial_hits FROM `(?<table>(?:``|[^`])+)` WHERE REGEXP_LIKE\\(`(?<column>(?:``|[^`])+)`, \\?, 'i'\\)\\z",
		RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(50));

	public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
	{
		if (eventData.Exception is MySqlException exception)
		{
			Capture(command, exception.Number, exception);
		}
	}

	// The migration service uses synchronous IMigrator.Migrate. Restrict capture to
	// expansion: contraction has other regexes whose scan variables could be stale.
	internal static void Capture(DbCommand failedCommand, int errorNumber, Exception originalException)
	{
		try
		{
			if (errorNumber != 3699 || failedCommand.CommandText.Trim() != ExpansionCall)
			{
				return;
			}

			var context = new FailureContext("ConnectionUnavailable");
			var connection = failedCommand.Connection;
			if (connection is not null && connection.State == ConnectionState.Open)
			{
				try
				{
					using var diagnostic = connection.CreateCommand();
					diagnostic.CommandText = DiagnosticSelect;
					diagnostic.CommandTimeout = 2;
					diagnostic.Transaction = failedCommand.Transaction;
					using var reader = diagnostic.ExecuteReader();
					context = ReadContext(reader);
				}
				catch (Exception secondary)
				{
					// Never retain provider messages: they can contain configuration/data.
					context = new FailureContext("DiagnosticReadFailed", DiagnosticFailureType: secondary.GetType().Name);
				}
			}

			originalException.Data[ContextKey] = context;
		}
		catch
		{
			// Even inspecting a broken command or attaching context must not mask the error.
		}
	}

	private static FailureContext ReadContext(DbDataReader reader)
	{
		if (!reader.Read() || Enumerable.Range(0, 5).Any(reader.IsDBNull))
		{
			return new FailureContext("SessionContextUnavailable");
		}

		var queryLength = Convert.ToInt64(reader.GetValue(3));
		var patternLength = Convert.ToInt64(reader.GetValue(4));
		if (queryLength is <= 0 or > 2048 || patternLength is <= 0 or > 512)
		{
			return new FailureContext("SessionContextUnavailable");
		}

		var query = reader.GetString(1);
		var pattern = reader.GetString(2);
		if (query.EnumerateRunes().LongCount() != queryLength || pattern.EnumerateRunes().LongCount() != patternLength)
		{
			return new FailureContext("SessionContextUnavailable");
		}

		var match = ScanQuery.Match(query);
		if (!match.Success)
		{
			return new FailureContext("SessionContextUnavailable");
		}

		var table = match.Groups["table"].Value.Replace("``", "`");
		var column = match.Groups["column"].Value.Replace("``", "`");
		if (table.EnumerateRunes().Count() > 64 || column.EnumerateRunes().Count() > 64 ||
			pattern != (column.EndsWith("type", StringComparison.OrdinalIgnoreCase) ? TypePattern : DefinitionPattern))
		{
			return new FailureContext("SessionContextUnavailable");
		}

		return new FailureContext("Captured", Convert.ToInt64(reader.GetValue(0)), table, column, query, pattern);
	}

	internal static string? FormatFailure(Exception? exception)
	{
		if (exception is null)
		{
			return null;
		}

		var original = exception.ToString();
		try
		{
			// EF may wrap a provider failure. Keep its full original stack, and only
			// serialize the bounded typed context produced by this interceptor.
			for (var current = exception; current is not null; current = current.InnerException)
			{
				if (current.Data[ContextKey] is FailureContext context)
				{
					return original + Environment.NewLine + "Cell spatial regex diagnostic: " + JsonSerializer.Serialize(context);
				}
			}
		}
		catch
		{
			// Diagnostic formatting must not prevent archive saving or backup recovery.
		}

		return original;
	}

	private sealed record FailureContext(string Status, long? ConnectionId = null, string? Table = null,
		string? Column = null, string? PreparedQuery = null, string? Pattern = null, string? DiagnosticFailureType = null)
	{
		public string Migration => CellSpatialRegexFailureInterceptor.Migration;
		public string Procedure => "fm_cell_spatial_preflight_20261006143539";
		public int ErrorNumber => 3699;
	}
}

#nullable enable

using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MySqlConnector;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CellSpatialRegexFailureInterceptorTests
{
	private static string Scan(string table = "autobuilderroomtemplates", string column = "Definition") =>
		$"SELECT COUNT(*) INTO @fm_cell_spatial_hits FROM `{table.Replace("`", "``")}` WHERE REGEXP_LIKE(`{column.Replace("`", "``")}`, ?, 'i')";

	internal static void AttachCapture(Exception exception, bool closed = false, bool secondaryFailure = false)
	{
		using var connection = new FakeConnection { CurrentState = closed ? ConnectionState.Closed : ConnectionState.Open, ThrowOnRead = secondaryFailure };
		using var command = connection.FailedCommand();
		CellSpatialRegexFailureInterceptor.Capture(command, 3699, exception);
	}

	private static JsonDocument Context(Exception exception)
	{
		var text = CellSpatialRegexFailureInterceptor.FormatFailure(exception)!;
		const string prefix = "Cell spatial regex diagnostic: ";
		return JsonDocument.Parse(text[(text.IndexOf(prefix, StringComparison.Ordinal) + prefix.Length)..]);
	}

	[TestMethod]
	public void CommandFailed_CapturesProviderTimeoutOnOriginalConnection()
	{
		using var connection = new FakeConnection();
		using var command = connection.FailedCommand();
		using var transaction = new FakeTransaction(connection);
		command.Transaction = transaction;
		// MySqlConnector's wire-error constructors are internal; simulate its error packet.
		var original = (MySqlException)Activator.CreateInstance(typeof(MySqlException),
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			[(MySqlErrorCode)3699, "Timeout exceeded in regular expression match."], null)!;
		var eventData = new CommandErrorEventData(null!, (_, _) => "", connection, command, null,
			DbCommandMethod.ExecuteNonQuery, Guid.NewGuid(), Guid.NewGuid(), original, false, false,
			DateTimeOffset.UtcNow, TimeSpan.Zero, CommandSource.Migrations);

		new CellSpatialRegexFailureInterceptor().CommandFailed(command, eventData);

		using var context = Context(original);
		var root = context.RootElement;
		Assert.AreEqual("Captured", root.GetProperty("Status").GetString());
		Assert.AreEqual(137L, root.GetProperty("ConnectionId").GetInt64());
		Assert.AreEqual("autobuilderroomtemplates", root.GetProperty("Table").GetString());
		Assert.AreEqual("Definition", root.GetProperty("Column").GetString());
		Assert.AreEqual(Scan(), root.GetProperty("PreparedQuery").GetString());
		Assert.AreEqual(CellSpatialRegexFailureInterceptor.DefinitionPattern, root.GetProperty("Pattern").GetString());
		Assert.AreEqual(CellSpatialRegexFailureInterceptor.Migration, root.GetProperty("Migration").GetString());
		Assert.AreEqual(3699, original.Number);
		Assert.AreEqual(1, connection.Reads);
		Assert.AreEqual(1, connection.CommandsCreated);
		Assert.AreEqual(0, connection.Opens);
		Assert.AreEqual(0, connection.Closes);
		Assert.IsTrue(connection.LastDiagnostic!.Disposed);
		Assert.AreSame(connection, connection.LastDiagnostic.Connection);
		Assert.AreSame(transaction, connection.LastDiagnostic.Transaction);
		Assert.AreEqual(2, connection.LastDiagnostic.CommandTimeout);
		Assert.AreEqual(CellSpatialRegexFailureInterceptor.DiagnosticSelect, connection.LastDiagnostic.CommandText);
		StringAssert.Contains(CellSpatialRegexFailureInterceptor.FormatFailure(original)!, original.ToString());
	}

	[DataTestMethod]
	[DataRow("Definition", false)]
	[DataRow("TemplateType", true)]
	[DataRow("ownerTYPE", true)]
	public void Capture_DecodesMaximalEscapedIdentifiersAndPatternBranch(string suffix, bool typePattern)
	{
		var table = new string('`', 64);
		var column = new string('x', 64 - suffix.Length) + suffix;
		using var connection = new FakeConnection { Query = Scan(table, column), Pattern = typePattern ? CellSpatialRegexFailureInterceptor.TypePattern : CellSpatialRegexFailureInterceptor.DefinitionPattern };
		using var command = connection.FailedCommand();
		var original = new InvalidOperationException("original");
		CellSpatialRegexFailureInterceptor.Capture(command, 3699, original);
		using var context = Context(original);
		Assert.AreEqual("Captured", context.RootElement.GetProperty("Status").GetString());
		Assert.AreEqual(table, context.RootElement.GetProperty("Table").GetString());
		Assert.AreEqual(column, context.RootElement.GetProperty("Column").GetString());
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Capture_MissingOrClosedConnectionNeverOpensAnother(bool missing)
	{
		using var connection = new FakeConnection { CurrentState = ConnectionState.Closed };
		using var command = connection.FailedCommand();
		if (missing) command.Connection = null;
		var original = new InvalidOperationException("original");
		CellSpatialRegexFailureInterceptor.Capture(command, 3699, original);
		using var context = Context(original);
		Assert.AreEqual("ConnectionUnavailable", context.RootElement.GetProperty("Status").GetString());
		Assert.AreEqual(0, connection.CommandsCreated);
		Assert.AreEqual(0, connection.Opens);
		Assert.AreEqual(0, connection.Reads);
	}

	[TestMethod]
	public void Capture_SecondaryReadFailurePreservesOriginalAndOmitsSecondaryMessage()
	{
		using var connection = new FakeConnection { ThrowOnRead = true };
		using var command = connection.FailedCommand();
		var original = new InvalidOperationException("original");
		CellSpatialRegexFailureInterceptor.Capture(command, 3699, original);
		using var context = Context(original);
		Assert.AreEqual("DiagnosticReadFailed", context.RootElement.GetProperty("Status").GetString());
		Assert.AreEqual(nameof(InvalidOperationException), context.RootElement.GetProperty("DiagnosticFailureType").GetString());
		Assert.IsFalse(CellSpatialRegexFailureInterceptor.FormatFailure(original)!.Contains("SECRET_SECONDARY_VALUE"));
		Assert.IsTrue(connection.LastDiagnostic!.Disposed);
		Assert.IsNull(original.InnerException);
		Assert.AreEqual("original", original.Message);
	}

	[TestMethod]
	public void Capture_BrokenCommandCannotMaskOriginalFailure()
	{
		using var connection = new FakeConnection { ThrowOnState = true };
		using var command = connection.FailedCommand();
		var original = new InvalidOperationException("original");
		CellSpatialRegexFailureInterceptor.Capture(command, 3699, original);
		Assert.AreEqual(original.ToString(), CellSpatialRegexFailureInterceptor.FormatFailure(original));
		Assert.AreEqual(0, connection.Reads);
	}

	[TestMethod]
	public void Capture_AndFormattingCannotMaskFailureWhenExceptionDataIsUnavailable()
	{
		var original = new UnattachableException();
		AttachCapture(original);
		Assert.AreEqual(original.ToString(), CellSpatialRegexFailureInterceptor.FormatFailure(original));
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	[DataRow(4)]
	[DataRow(5)]
	[DataRow(6)]
	[DataRow(7)]
	[DataRow(8)]
	[DataRow(9)]
	public void Capture_UnavailableOrUnexpectedSessionValuesAreNotArchived(int scenario)
	{
		using var connection = new FakeConnection();
		switch (scenario)
		{
			case 0: connection.Query = null; break;
			case 1: connection.Pattern = null; break;
			case 2: connection.Query = "SECRET_RAW_DEFINITION"; break;
			case 3: connection.Pattern = "SECRET_RAW_DEFINITION"; break;
			case 4: connection.QueryLength = 2049; break;
			case 5: connection.PatternLength = 513; break;
			case 6: connection.Pattern = CellSpatialRegexFailureInterceptor.TypePattern; break;
			case 7: connection.EmptyResult = true; break;
			case 8: connection.QueryLength = 1; break;
			case 9: connection.Query = Scan(new string('x', 65)); break;
		}
		using var command = connection.FailedCommand();
		var original = new InvalidOperationException("original");
		CellSpatialRegexFailureInterceptor.Capture(command, 3699, original);
		using var context = Context(original);
		Assert.AreEqual("SessionContextUnavailable", context.RootElement.GetProperty("Status").GetString());
		Assert.AreEqual(JsonValueKind.Null, context.RootElement.GetProperty("PreparedQuery").ValueKind);
		Assert.AreEqual(JsonValueKind.Null, context.RootElement.GetProperty("Pattern").ValueKind);
		Assert.IsFalse(CellSpatialRegexFailureInterceptor.FormatFailure(original)!.Contains("SECRET_RAW_DEFINITION"));
	}

	[DataTestMethod]
	[DataRow(1042, CellSpatialRegexFailureInterceptor.ExpansionCall)]
	[DataRow(3699, "CALL `fm_cell_spatial_contract_20261006161646`(FALSE);")]
	[DataRow(3699, "SELECT SECRET_RAW_DEFINITION")]
	public void Capture_UnrelatedFailuresRemainUnchanged(int number, string text)
	{
		using var connection = new FakeConnection();
		using var command = connection.FailedCommand();
		command.CommandText = text;
		var original = new InvalidOperationException("original");
		CellSpatialRegexFailureInterceptor.Capture(command, number, original);
		Assert.AreEqual(original.ToString(), CellSpatialRegexFailureInterceptor.FormatFailure(original));
		Assert.AreEqual(0, connection.Reads);
		Assert.AreEqual(0, original.Data.Count);
	}

	[TestMethod]
	public void FormatFailure_TraversesWrapperWithoutChangingOriginalException()
	{
		var original = new InvalidOperationException("original");
		AttachCapture(original);
		var wrapper = new ApplicationException("outer", original);
		var formatted = CellSpatialRegexFailureInterceptor.FormatFailure(wrapper)!;
		StringAssert.StartsWith(formatted, wrapper.ToString());
		StringAssert.Contains(formatted, "autobuilderroomtemplates");
		Assert.AreSame(original, wrapper.InnerException);
		Assert.IsNull(CellSpatialRegexFailureInterceptor.FormatFailure(null));
	}

	private sealed class FakeConnection : DbConnection
	{
		public string? Query { get; set; } = Scan();
		public string? Pattern { get; set; } = CellSpatialRegexFailureInterceptor.DefinitionPattern;
		public long? QueryLength { get; set; }
		public long? PatternLength { get; set; }
		public bool ThrowOnRead { get; set; }
		public bool ThrowOnState { get; set; }
		public bool EmptyResult { get; set; }
		public ConnectionState CurrentState { get; set; } = ConnectionState.Open;
		public int Opens { get; private set; }
		public int Closes { get; private set; }
		public int Reads { get; private set; }
		public int CommandsCreated { get; private set; }
		public FakeCommand? LastDiagnostic { get; private set; }
		public FakeCommand FailedCommand() => new(this) { CommandText = CellSpatialRegexFailureInterceptor.ExpansionCall };
		[AllowNull] public override string ConnectionString { get => throw new AssertFailedException("Credentials must not be inspected"); set => throw new NotSupportedException(); }
		public override string Database => throw new NotSupportedException();
		public override string DataSource => throw new NotSupportedException();
		public override string ServerVersion => throw new NotSupportedException();
		public override ConnectionState State => ThrowOnState ? throw new InvalidOperationException("broken") : CurrentState;
		public override void Open() { Opens++; throw new AssertFailedException("No connection may be opened"); }
		public override void Close() { Closes++; }
		public override void ChangeDatabase(string databaseName) => throw new AssertFailedException("No mutation");
		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new AssertFailedException("No new transaction");
		protected override DbCommand CreateDbCommand()
		{
			CommandsCreated++;
			return LastDiagnostic = new FakeCommand(this);
		}
		public DbDataReader Read(FakeCommand command)
		{
			Reads++;
			Assert.AreEqual(CellSpatialRegexFailureInterceptor.DiagnosticSelect, command.CommandText);
			if (ThrowOnRead) throw new InvalidOperationException("SECRET_SECONDARY_VALUE");
			var table = new DataTable();
			table.Columns.Add("id", typeof(long)); table.Columns.Add("query", typeof(string)); table.Columns.Add("pattern", typeof(string));
			table.Columns.Add("querylength", typeof(long)); table.Columns.Add("patternlength", typeof(long));
			if (!EmptyResult)
			{
				table.Rows.Add(137L, (object?)Query ?? DBNull.Value, (object?)Pattern ?? DBNull.Value,
					QueryLength ?? Query?.Length ?? 0L, PatternLength ?? Pattern?.Length ?? 0L);
			}
			return table.CreateDataReader();
		}
	}

	private sealed class FakeCommand(FakeConnection owner) : DbCommand
	{
		public bool Disposed { get; private set; }
		[AllowNull] public override string CommandText { get; set; } = "";
		public override int CommandTimeout { get; set; }
		public override CommandType CommandType { get; set; }
		public override bool DesignTimeVisible { get; set; }
		public override UpdateRowSource UpdatedRowSource { get; set; }
		protected override DbConnection? DbConnection { get; set; } = owner;
		protected override DbTransaction? DbTransaction { get; set; }
		protected override DbParameterCollection DbParameterCollection => throw new AssertFailedException("No parameter dumps");
		protected override DbParameter CreateDbParameter() => throw new NotSupportedException();
		public override void Cancel() => throw new NotSupportedException();
		public override void Prepare() => throw new AssertFailedException("No scan query may be prepared");
		public override int ExecuteNonQuery() => throw new AssertFailedException("No database mutation");
		public override object? ExecuteScalar() => throw new NotSupportedException();
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => owner.Read(this);
		protected override void Dispose(bool disposing) { Disposed = true; base.Dispose(disposing); }
	}

	private sealed class FakeTransaction(DbConnection connection) : DbTransaction
	{
		public override IsolationLevel IsolationLevel => IsolationLevel.Unspecified;
		protected override DbConnection DbConnection => connection;
		public override void Commit() => throw new AssertFailedException("No commit");
		public override void Rollback() => throw new AssertFailedException("No diagnostic rollback");
	}

	private sealed class UnattachableException : Exception
	{
		public override IDictionary Data => throw new InvalidOperationException("Data unavailable");
	}
}

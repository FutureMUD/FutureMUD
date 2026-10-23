#nullable enable

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace MudSharp.Server;

/// <summary>Best-effort local diagnostics. Never query the world or database while formatting a failure.</summary>
internal static class ExceptionReporter
{
	private const int MaximumExceptions = 64;
	private const int MaximumFieldLength = 4096;
	private static readonly AsyncLocal<string?> CurrentOperation = new();

	// The game loop sets this before entering each phase, including when performance monitoring is off.
	internal static string? Operation { get => CurrentOperation.Value; set => CurrentOperation.Value = value; }

	internal static void Annotate(Exception exception, params (string Name, Func<object?> Read)[] context)
	{
		foreach (var (name, read) in context)
		{
			try
			{
				if (!exception.Data.Contains(name)) exception.Data[name] = read();
			}
			catch (Exception) { /* Context must never replace the original failure. */ }
		}
	}

	internal static string Format(Exception exception, string origin, bool terminating,
		params (string Name, Func<string?> Read)[] context)
		=> FormatCore(exception, origin, terminating, true, context);

	// Email's privacy contract excludes provider messages, payloads and credentials even on worker failure.
	internal static string FormatPrivateFailure(Exception exception, string origin,
		params (string Name, Func<string?> Read)[] context)
		=> FormatCore(exception, origin, false, false, context);

	private static string FormatCore(Exception exception, string origin, bool terminating, bool includeExceptionText,
		params (string Name, Func<string?> Read)[] context)
	{
		var report = new StringBuilder();
		report.AppendLine($"Exception Report {Guid.NewGuid():N}");
		report.AppendLine($"UTC: {DateTimeOffset.UtcNow:O}");
		report.AppendLine($"Origin: {origin}");
		report.AppendLine($"Process terminating: {terminating}");
		Field(report, "Engine version", () => typeof(MudSharp).Assembly.GetName().Version?.ToString());
		Field(report, "Build identity", () => typeof(MudSharp).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);
#if DEBUG
		report.AppendLine("Configuration: Debug");
#else
		report.AppendLine("Configuration: Release");
#endif
		Field(report, "Runtime", () => RuntimeInformation.FrameworkDescription);
		Field(report, "OS", () => RuntimeInformation.OSDescription);
		Field(report, "Architecture", () => $"process={RuntimeInformation.ProcessArchitecture}, OS={RuntimeInformation.OSArchitecture}");
		Field(report, "Process", () => Environment.ProcessId.ToString(CultureInfo.InvariantCulture));
		Field(report, "Uptime", () =>
		{
			using var process = Process.GetCurrentProcess();
			return (DateTime.UtcNow - process.StartTime.ToUniversalTime()).ToString("c", CultureInfo.InvariantCulture);
		});
		Field(report, "Thread", () => $"{Environment.CurrentManagedThreadId} ({Thread.CurrentThread.Name ?? "unnamed"})");
		Field(report, "Operation", () => Operation);
		Field(report, "Memory bytes", () => $"managed={GC.GetTotalMemory(false)}, working set={Environment.WorkingSet}");
		report.AppendLine();
		// Preserve the original before invoking any optional application callbacks.
		var originalTextFailed = false;
		try
		{
			if (includeExceptionText) report.AppendLine(exception.ToString());
			else report.AppendLine("Exception messages and data omitted for email privacy.");
		}
		catch (Exception)
		{
			originalTextFailed = true;
			report.AppendLine($"{exception.GetType().FullName}: exception text unavailable");
			Field(report, "Message", () => exception.Message);
			Field(report, "Stack", () => exception.StackTrace);
		}

		var pending = new Queue<(Exception Error, string Path)>();
		var seen = new HashSet<Exception>(ReferenceEqualityComparer.Instance);
		pending.Enqueue((exception, "root"));
		while (pending.Count > 0 && seen.Count < MaximumExceptions)
		{
			var (error, path) = pending.Dequeue();
			if (!seen.Add(error)) continue;
			report.AppendLine($"Details [{path}]: {error.GetType().FullName}");
			Field(report, "HResult", () => $"0x{error.HResult:X8}");
			if (includeExceptionText)
			{
				Field(report, "Message", () => error.Message);
				Field(report, "Additional details", () => Describe(error));
			}
			if (!includeExceptionText || originalTextFailed) Field(report, "Stack", () => error.StackTrace);
			if (error is AggregateException aggregate)
			{
				for (var i = 0; i < aggregate.InnerExceptions.Count && i < MaximumExceptions; i++)
					pending.Enqueue((aggregate.InnerExceptions[i], $"{path}/aggregate[{i}]"));
				if (aggregate.InnerExceptions.Count > MaximumExceptions) report.AppendLine("Aggregate details truncated.");
			}
			else if (error.InnerException is { } inner) pending.Enqueue((inner, $"{path}/inner"));
			if (error is ReflectionTypeLoadException reflection)
			{
				for (var i = 0; i < reflection.LoaderExceptions.Length && i < MaximumExceptions; i++)
					if (reflection.LoaderExceptions[i] is { } loader) pending.Enqueue((loader, $"{path}/loader[{i}]"));
				if (reflection.LoaderExceptions.Length > MaximumExceptions) report.AppendLine("Loader details truncated.");
			}
		}
		if (pending.Count > 0) report.AppendLine("Exception details truncated.");
		foreach (var (name, read) in context) Field(report, name, read);
		return report.ToString();
	}

	private static string Describe(Exception error)
	{
		var details = new StringBuilder();
		switch (error)
		{
			case DbUpdateException update:
				foreach (var entry in update.Entries.Take(32))
				{
					Field(details, "Entity", () => $"{entry.Metadata.Name}; table={entry.Metadata.GetTableName()}; state={entry.State}");
					Field(details, "Keys", () => string.Join(", ", entry.Metadata.FindPrimaryKey()?.Properties
						.Select(property => $"{property.Name}={SafeScalar(entry.Property(property.Name).CurrentValue)}") ?? []));
					Field(details, "Modified properties", () => string.Join(", ", entry.Properties
						.Where(property => property.IsModified).Select(property => property.Metadata.Name).Take(64)));
				}
				if (update.Entries.Count > 32) details.AppendLine("Entity details truncated.");
				break;
			case MySqlConnector.MySqlException mysql:
				details.AppendLine($"MySQL number={mysql.Number}; code={mysql.ErrorCode}; SQLSTATE={mysql.SqlState}; transient={mysql.IsTransient}");
				break;
			case MySql.Data.MySqlClient.MySqlException mysql:
				details.AppendLine($"MySQL number={mysql.Number}; SQLSTATE={mysql.SqlState}");
				break;
			case ReflectionTypeLoadException reflection:
				details.AppendLine($"Types={reflection.Types.Length}; failed type slots={reflection.Types.Count(type => type is null)}; loader exceptions={reflection.LoaderExceptions.Length}");
				break;
			case XmlException xml:
				details.AppendLine($"XML line={xml.LineNumber}; position={xml.LinePosition}; source={xml.SourceUri}");
				break;
			case System.Text.Json.JsonException json:
				details.AppendLine($"JSON path={json.Path}; line={json.LineNumber}; byte position={json.BytePositionInLine}");
				break;
			case Newtonsoft.Json.JsonReaderException json:
				details.AppendLine($"JSON path={json.Path}; line={json.LineNumber}; position={json.LinePosition}");
				break;
			case Newtonsoft.Json.JsonSerializationException json:
				details.AppendLine($"JSON path={json.Path}; line={json.LineNumber}; position={json.LinePosition}");
				break;
			case SocketException socket:
				details.AppendLine($"Socket error={socket.SocketErrorCode}; native code={socket.NativeErrorCode}");
				break;
			case HttpRequestException http:
				details.AppendLine($"HTTP status={(int?)http.StatusCode}; category={http.HttpRequestError}");
				break;
			case FileNotFoundException file:
				details.AppendLine($"File={file.FileName}");
				break;
			case FileLoadException file:
				details.AppendLine($"File={file.FileName}");
				break;
			case TypeLoadException type:
				details.AppendLine($"Type={type.TypeName}");
				break;
		}
		// Explicit opt-in keys only: arbitrary Data values can contain credentials or invoke user code.
		foreach (var key in new[] { "Operation", "EntityId", "EntityType", "DefinitionId", "RequestId" })
			Field(details, $"Data.{key}", () => error.Data.Contains(key) ? SafeScalar(error.Data[key]) : null);
		return details.ToString();
	}

	private static string SafeScalar(object? value) => value switch
	{
		null => "null",
		string text => text.Length <= 256 ? text : text[..256] + " [truncated]",
		bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal or Guid
			=> Convert.ToString(value, CultureInfo.InvariantCulture) ?? "null",
		_ => "[non-scalar value omitted]"
	};

	private static void Field(StringBuilder report, string name, Func<string?> read)
	{
		try
		{
			var value = read();
			if (!string.IsNullOrEmpty(value)) report.AppendLine($"{name}: {(value.Length > MaximumFieldLength ? value[..MaximumFieldLength] + " [truncated]" : value)}");
		}
		catch (Exception error) { report.AppendLine($"{name}: unavailable ({error.GetType().Name})"); }
	}

	internal static bool Write(string report, Action<string>? notify = null, string? directory = null,
		TextWriter? fallback = null)
	{
		var written = false;
		try
		{
			var path = Path.Combine(directory ?? Environment.CurrentDirectory,
				$"Fatal Exception {DateTime.UtcNow:yyyy-MM-dd HH-mm-ss.fffffff} UTC {Guid.NewGuid():N}.txt");
			using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
			{
				writer.Write(report);
			}
			written = true;
		}
		catch (Exception error) { WriteFallback(fallback, $"Exception report file unavailable ({error.GetType().Name}).\n{report}"); }
		// Keep the existing Discord payload, but a notification failure cannot invalidate the local report.
		try { notify?.Invoke(report); }
		catch (Exception error) { WriteFallback(fallback, $"Exception notification unavailable ({error.GetType().Name})."); }
		return written;
	}

	internal static void WriteFallback(TextWriter? fallback, string message)
	{
		try { (fallback ?? Console.Error).WriteLine(message); }
		catch (Exception) { /* The last-resort sink must not throw into the failing subsystem. */ }
	}
}

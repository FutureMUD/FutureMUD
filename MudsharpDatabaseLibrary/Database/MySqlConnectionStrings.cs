#nullable enable

using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using MySql.Data.MySqlClient;

namespace MudSharp.Database;

internal static class MySqlConnectionStrings
{
	internal static MySqlConnectionStringBuilder CreateBuilder(string connectionString)
	{
		var builder = new MySqlConnectionStringBuilder();
		_ = new LegacyTlsParser(builder) { ConnectionString = connectionString };
		return new MySqlConnectionStringBuilder(builder.ConnectionString);
	}

	private sealed class LegacyTlsParser(MySqlConnectionStringBuilder builder) : DbConnectionStringBuilder
	{
		// Let the framework tokenize quoted values and feed each option to the driver
		// in its original order. Serializing a generic builder first would reorder
		// repeated aliases and could change the target database or TLS settings.
		[AllowNull]
		public override object this[string keyword]
		{
			get => builder[keyword];
			set
			{
				builder[keyword] = keyword.Replace(" ", string.Empty).Replace("-", string.Empty)
						.Equals("sslmode", StringComparison.OrdinalIgnoreCase) &&
					value is string mode && mode.Trim().Equals("None", StringComparison.OrdinalIgnoreCase)
						? "Disabled"
						: value;
			}
		}

		public override void Clear() => builder.Clear();

		public override bool Remove(string keyword) => builder.Remove(keyword);
	}
}

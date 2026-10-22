#nullable enable

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;

namespace MudSharp_Unit_Tests;

/// <summary>
/// Owns one uniquely named test database. Factory-created contexts borrow its options
/// and must be disposed before this owner. Keep shared EF providers/models, but release
/// the fixture rows when the test ends instead of retaining them for the whole testhost.
/// </summary>
internal sealed class OwnedInMemorySeederContext : FuturemudDatabaseContext
{
	private bool _disposed;

	internal OwnedInMemorySeederContext(DbContextOptions<FuturemudDatabaseContext> options) : base(options)
	{
		if (Database.IsInMemory()) return;
		base.Dispose();
		throw new ArgumentException("An owned seeder fixture requires the InMemory provider.", nameof(options));
	}

	public override void Dispose()
	{
		if (_disposed) return;
		_disposed = true;
		try
		{
			Database.EnsureDeleted();
		}
		finally
		{
			base.Dispose();
		}
	}

	public override async ValueTask DisposeAsync()
	{
		if (_disposed) return;
		_disposed = true;
		try
		{
			await Database.EnsureDeletedAsync();
		}
		finally
		{
			await base.DisposeAsync();
		}
	}
}

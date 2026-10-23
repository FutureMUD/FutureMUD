#nullable enable

using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

/// <summary>Runs runtime save/load methods in an owned in-memory FMDB session without touching its default connection.</summary>
internal sealed class InMemoryDatabaseTestScope : IDisposable
{
	private readonly object _ambient;
	private readonly PropertyInfo _value;
	private readonly object? _previous;
	public FuturemudDatabaseContext Context { get; }

	public InMemoryDatabaseTestScope()
	{
		Context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
		_ambient = typeof(FMDB).GetField("_ambientSession", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
		_value = _ambient.GetType().GetProperty("Value")!;
		_previous = _value.GetValue(_ambient);
		var sessionType = typeof(FMDB).GetNestedType("DatabaseSession", BindingFlags.NonPublic)!;
		var session = Activator.CreateInstance(sessionType, true)!;
		sessionType.GetProperty("Context")!.SetValue(session, Context);
		_value.SetValue(_ambient, session);
	}

	public void Dispose()
	{
		_value.SetValue(_ambient, _previous);
		Context.Dispose();
	}
}

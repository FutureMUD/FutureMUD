using System.Data;
using System.Data.Common;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MudSharp.Models;
using MySqlConnector;

namespace MudSharp.Database;

public sealed class FMDB : IDisposable
{
	private sealed class SuppressEfWritesInterceptor : SaveChangesInterceptor
	{
		public static SuppressEfWritesInterceptor Instance { get; } = new();

		public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
			InterceptionResult<int> result)
		{
			return InterceptionResult<int>.SuppressWithResult(0);
		}

		public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
			InterceptionResult<int> result, CancellationToken cancellationToken = default)
		{
			return ValueTask.FromResult(InterceptionResult<int>.SuppressWithResult(0));
		}
	}

	private sealed class DatabaseSession
	{
		public FuturemudDatabaseContext Context { get; set; }
		public DbConnection Connection { get; set; }
		public uint InstanceCount { get; set; }
	}

	private sealed class IsolatedScope(DatabaseSession previous) : IDisposable
	{
		private bool _disposed;

		public void Dispose()
		{
			if (_disposed)
			{
				return;
			}

			lock (_lock)
			{
				var session = _ambientSession.Value;
				try
				{
					DisposeSession(session);
				}
				finally
				{
					_ambientSession.Value = previous;
					_disposed = true;
				}
			}
		}
	}

	private static readonly object _lock = new();
	private static readonly AsyncLocal<DatabaseSession> _ambientSession = new();
	private static DatabaseSession _defaultSession;
	private readonly DatabaseSession _session;
	private bool _disposed;

	public FMDB()
	{
		lock (_lock)
		{
			_session = CurrentSession ?? InitialiseContext();
			_session.InstanceCount++;
		}
	}

	public static FuturemudDatabaseContext Context
	{
		get => CurrentSession?.Context;
		private set
		{
			lock (_lock)
			{
				EnsureDefaultSession().Context = value;
				RemoveEmptyDefaultSession();
			}
		}
	}

	public static DbConnection Connection
	{
		get => CurrentSession?.Connection;
		private set
		{
			lock (_lock)
			{
				EnsureDefaultSession().Connection = value;
				RemoveEmptyDefaultSession();
			}
		}
	}

	private static uint InstanceCount
	{
		get => _defaultSession?.InstanceCount ?? 0;
		set
		{
			lock (_lock)
			{
				EnsureDefaultSession().InstanceCount = value;
				RemoveEmptyDefaultSession();
			}
		}
	}

	public static string ConnectionString { get; set; } = string.Empty;
	public static string Provider { get; set; } = string.Empty;
	public static bool IsIsolated => _ambientSession.Value is not null;

	private static DatabaseSession CurrentSession => _ambientSession.Value ?? _defaultSession;

	private static DatabaseSession EnsureDefaultSession()
	{
		return _defaultSession ??= new DatabaseSession();
	}

	private static void RemoveEmptyDefaultSession()
	{
		if (_defaultSession is { Context: null, Connection: null, InstanceCount: 0 })
		{
			_defaultSession = null;
		}
	}

	/// <summary>
	/// Creates an async-flow-local database session. Nested <see cref="FMDB"/> handles use this
	/// session without disturbing the live engine's static session. Set <paramref name="suppressEfWrites"/>
	/// for a sandbox that must retain in-memory EF state without issuing database writes.
	/// </summary>
	public static IDisposable BeginIsolatedScope(bool suppressEfWrites = false)
	{
		lock (_lock)
		{
			var previous = _ambientSession.Value;
			if (previous is not null)
			{
				throw new InvalidOperationException("Nested isolated FMDB scopes are not supported.");
			}

			_ambientSession.Value = CreateSession(suppressEfWrites);
			return new IsolatedScope(previous);
		}
	}

	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		lock (_lock)
		{
			if (_session.InstanceCount > 0)
			{
				_session.InstanceCount--;
			}

			if (_session.InstanceCount == 0 && ReferenceEquals(_session, _defaultSession))
			{
				DisposeSession(_session);
				_defaultSession = null;
			}
		}

		_disposed = true;
		GC.SuppressFinalize(this);
	}

	private static DatabaseSession InitialiseContext()
	{
		var session = CreateSession();
		if (_ambientSession.Value is not null)
		{
			_ambientSession.Value = session;
		}
		else
		{
			_defaultSession = session;
		}

		return session;
	}

	private static DatabaseSession CreateSession(bool suppressEfWrites = false)
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseLazyLoadingProxies()
			.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
		if (suppressEfWrites)
		{
			options.AddInterceptors(SuppressEfWritesInterceptor.Instance);
		}

		var context = new FuturemudDatabaseContext(options.Options);
		var connection = new MySqlConnection(ConnectionString);
		try
		{
			connection.Open();
			connection.StateChange += OnStateChange;
			return new DatabaseSession
			{
				Context = context,
				Connection = connection
			};
		}
		catch
		{
			context.Dispose();
			connection.Dispose();
			throw;
		}
	}

	private static void DisposeSession(DatabaseSession session)
	{
		if (session is null)
		{
			return;
		}

		session.Context?.Dispose();
		if (session.Connection is null)
		{
			return;
		}

		session.Connection.StateChange -= OnStateChange;
		try
		{
			session.Connection.Close();
		}
		finally
		{
			session.Connection.Dispose();
		}
	}

	private static void OnStateChange(object sender, StateChangeEventArgs args)
	{
		if (args.OriginalState == args.CurrentState)
		{
			return;
		}

		Console.WriteLine(
			"The current Connection state has changed from {0} to {1}.",
			args.OriginalState,
			args.CurrentState);
	}
}

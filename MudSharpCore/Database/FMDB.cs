using System;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;
using MySqlConnector;

namespace MudSharp.Database;

public sealed class FMDB : IDisposable
{
	public FMDB()
	{
		lock (_lock)
		{
			if (Context == null)
			{
				InitialiseContext();
			}
			else
			{
				InstanceCount++;
			}
		}
	}

	private static readonly object _lock = new object();

	public static FuturemudDatabaseContext Context { get; private set; }

	private static uint InstanceCount { get; set; }

	public static DbConnection Connection { get; private set; }

	public static string ConnectionString { get; set; }

	public static string Provider { get; set; }

	#region IDisposable Members

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	public void Dispose(bool disposing)
	{
		lock (_lock)
		{
			if (--InstanceCount > 0)
			{
				return;
			}

			Context.Dispose();
			Context = null;
			Connection.StateChange -= OnStateChange;
			Connection.Close();
			Connection.Dispose();
			Connection = null;
		}
	}

	#endregion

	private void InitialiseContext()
	{
		lock(_lock) {
			Context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			                                       .UseLazyLoadingProxies().UseMySql(ConnectionString,
				                                       ServerVersion.AutoDetect(ConnectionString)).Options);
			Connection = new MySqlConnection(ConnectionString);
			Connection.Open();
			InstanceCount++;
			Connection.StateChange += OnStateChange;
		}
	}

	private static void OnStateChange(object sender,
		StateChangeEventArgs args)
	{
		if (args.OriginalState != args.CurrentState)
		{
			Console.WriteLine(
				"The current Connection state has changed from {0} to {1}.",
				args.OriginalState, args.CurrentState);
		}
	}
}
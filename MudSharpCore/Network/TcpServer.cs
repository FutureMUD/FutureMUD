#nullable enable

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MudSharp.Framework.Diagnostics;

namespace MudSharp.Network;

public sealed class TCPServer : IServer, IAsyncServer, IRuntimeNetworkPerformanceSource, INetworkTelemetrySink
{
	private readonly object _lifecycleLock = new();
	private readonly TimeProvider _timeProvider;
	private readonly ConcurrentQueue<PlayerConnection> _pendingConnections = new();
	private readonly ConcurrentDictionary<PlayerConnection, byte> _activeConnections = new();
	private readonly Queue<(IPAddress Address, DateTime Expires)> _floodExpirations = new();
	private AddConnectionCallback? _addConnection;
	private IEnumerable<IPlayerConnection>? _connections;
	private ConnectionSnapshotRegistry? _connectionRegistry;
	private TcpListener? _listener;
	private Task? _acceptTask;
	private CancellationTokenSource? _serverCancellation;
	private NetworkCounters _counters = new();
	private int _isListening;

	public TCPServer(IPAddress host, int port, TimeProvider? timeProvider = null)
	{
		IPAddress = host;
		Port = port;
		_timeProvider = timeProvider ?? TimeProvider.System;
	}

	public IPAddress IPAddress { get; }
	public int Port { get; }
	internal int BoundPort => _listener?.LocalEndpoint is IPEndPoint endpoint ? endpoint.Port : Port;
	public bool IsListeningAndResponding => Volatile.Read(ref _isListening) != 0;
	public Dictionary<IPAddress, TcpConnectionInformation> ConnectionDictionary { get; } = new();
	public TimeSpan IpFloodKeepAlive { get; } = TimeSpan.FromSeconds(60);

	public void Bind(IEnumerable<IPlayerConnection> connectionList, AddConnectionCallback addConnection)
	{
		_connections = connectionList;
		_connectionRegistry = connectionList as ConnectionSnapshotRegistry;
		_addConnection = addConnection;
	}

	public void Start()
	{
		StartAsync().AsTask().GetAwaiter().GetResult();
	}

	public ValueTask StartAsync(CancellationToken cancellationToken = default)
	{
		lock (_lifecycleLock)
		{
			if (IsListeningAndResponding || _acceptTask is { IsCompleted: false })
			{
				throw new ApplicationException("Trying to start an already started TCP Listener.");
			}

			if (_addConnection is null)
			{
				throw new InvalidOperationException("The TCP listener must be bound to a connection registry before it starts.");
			}

			_serverCancellation?.Dispose();
			_serverCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			_listener = new TcpListener(IPAddress, Port);
			_listener.Start();
			Volatile.Write(ref _isListening, 1);
			ConsoleUtilities.WriteLine("Successfully started listening on #2{0}#0.", IPAddress);
			_acceptTask = AcceptLoopAsync(_listener, _serverCancellation.Token);
		}

		return ValueTask.CompletedTask;
	}

	public void Stop()
	{
		StopAsync().AsTask().GetAwaiter().GetResult();
	}

	public async ValueTask StopAsync(CancellationToken cancellationToken = default)
	{
		Task? acceptTask;
		lock (_lifecycleLock)
		{
			acceptTask = _acceptTask;
			if (acceptTask is null && !IsListeningAndResponding)
			{
				return;
			}

			Volatile.Write(ref _isListening, 0);
			_serverCancellation?.Cancel();
			_listener?.Stop();
		}

		if (acceptTask is not null)
		{
			await ObserveExpectedCancellationAsync(acceptTask);
		}

		foreach (var connection in _activeConnections.Keys)
		{
			connection.RequestClose(ConnectionCloseMode.Drain);
		}

		var completions = _activeConnections.Keys.Select(x => x.TransportCompletion).ToArray();
		if (completions.Length > 0)
		{
			var allConnections = Task.WhenAll(completions);
			var timeout = Task.Delay(TimeSpan.FromSeconds(5), _timeProvider, cancellationToken);
			if (await Task.WhenAny(allConnections, timeout) != allConnections)
			{
				foreach (var connection in _activeConnections.Keys)
				{
					connection.RequestClose(ConnectionCloseMode.Abort);
				}

				await Task.WhenAll(_activeConnections.Keys.Select(x => x.TransportCompletion));
				cancellationToken.ThrowIfCancellationRequested();
			}
			else
			{
				await allConnections;
			}
		}

		lock (_lifecycleLock)
		{
			_listener = null;
			_acceptTask = null;
			_serverCancellation?.Dispose();
			_serverCancellation = null;
		}
	}

	public void ProcessPendingConnections()
	{
		while (_pendingConnections.TryDequeue(out var connection))
		{
			if (!IsListeningAndResponding)
			{
				connection.RequestClose(ConnectionCloseMode.Abort);
				connection.Dispose();
				continue;
			}

			try
			{
				_addConnection!(connection);
				if (connection.State == ConnectionState.Open)
				{
					connection.StartTransport();
				}
				else
				{
					connection.Dispose();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Warning: Exception while admitting a TCP connection - " + e);
				connection.RequestClose(ConnectionCloseMode.Abort);
				connection.Dispose();
			}
		}
	}

	/// <summary>
	/// Commits all currently staged output to each connection's asynchronous writer.
	/// </summary>
	public void ProcessAllOutgoing()
	{
		foreach (var connection in ConnectionSnapshot)
		{
			if (connection.State != ConnectionState.Closed && connection.HasOutgoingCommands)
			{
				connection.SendOutgoing();
			}
		}
	}

	internal bool RecordConnectionAttempt(IPAddress address, DateTime utcNow)
	{
		PruneConnectionDictionary(utcNow);
		if (ConnectionDictionary.TryGetValue(address, out var info))
		{
			info.NumberOfConnections++;
		}
		else
		{
			ConnectionDictionary[address] = info = new TcpConnectionInformation
			{
				StartOfPeriod = utcNow,
				NumberOfConnections = 1
			};
			_floodExpirations.Enqueue((address, utcNow + IpFloodKeepAlive));
		}

		return info.NumberOfConnections > 30;
	}

	internal void PruneConnectionDictionary(DateTime utcNow)
	{
		while (_floodExpirations.TryPeek(out var expiration) && utcNow > expiration.Expires)
		{
			_floodExpirations.Dequeue();
			if (ConnectionDictionary.TryGetValue(expiration.Address, out var info) &&
			    utcNow - info.StartOfPeriod > IpFloodKeepAlive)
			{
				ConnectionDictionary.Remove(expiration.Address);
			}
		}
	}

	public RuntimeNetworkPerformanceSnapshot GetNetworkPerformanceSnapshot()
	{
		var counters = Volatile.Read(ref _counters);
		return new RuntimeNetworkPerformanceSnapshot(
			Interlocked.Read(ref counters.AcceptedConnections),
			Interlocked.Read(ref counters.FloodRejectedConnections),
			_activeConnections.Count,
			Interlocked.Read(ref counters.BytesReceived),
			Interlocked.Read(ref counters.BytesSent),
			Interlocked.Read(ref counters.ReadOperations),
			Interlocked.Read(ref counters.WriteOperations),
			Interlocked.Read(ref counters.InputQueueHighWatermark),
			Interlocked.Read(ref counters.OutputQueueHighWatermarkBytes),
			Interlocked.Read(ref counters.SlowClientDisconnects),
			Interlocked.Read(ref counters.AcceptErrors),
			Interlocked.Read(ref counters.ReadErrors),
			Interlocked.Read(ref counters.WriteErrors));
	}

	public void ResetNetworkPerformanceCounters()
	{
		Volatile.Write(ref _counters, new NetworkCounters());
	}

	void INetworkTelemetrySink.RecordRead(int bytes)
	{
		var counters = Volatile.Read(ref _counters);
		Interlocked.Add(ref counters.BytesReceived, bytes);
		Interlocked.Increment(ref counters.ReadOperations);
	}

	void INetworkTelemetrySink.RecordWrite(int bytes)
	{
		var counters = Volatile.Read(ref _counters);
		Interlocked.Add(ref counters.BytesSent, bytes);
		Interlocked.Increment(ref counters.WriteOperations);
	}

	void INetworkTelemetrySink.RecordInputQueueDepth(int depth)
	{
		RecordMaximum(ref Volatile.Read(ref _counters).InputQueueHighWatermark, depth);
	}

	void INetworkTelemetrySink.RecordOutputQueueBytes(long bytes)
	{
		RecordMaximum(ref Volatile.Read(ref _counters).OutputQueueHighWatermarkBytes, bytes);
	}

	void INetworkTelemetrySink.RecordSlowClientDisconnect()
	{
		Interlocked.Increment(ref Volatile.Read(ref _counters).SlowClientDisconnects);
	}

	void INetworkTelemetrySink.RecordReadError()
	{
		Interlocked.Increment(ref Volatile.Read(ref _counters).ReadErrors);
	}

	void INetworkTelemetrySink.RecordWriteError()
	{
		Interlocked.Increment(ref Volatile.Read(ref _counters).WriteErrors);
	}

	private async Task AcceptLoopAsync(TcpListener listener, CancellationToken cancellationToken)
	{
		try
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				TcpClient client;
				try
				{
					client = await listener.AcceptTcpClientAsync(cancellationToken);
				}
				catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				catch (SocketException) when (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				catch (Exception e) when (e is SocketException or ObjectDisposedException)
				{
					Interlocked.Increment(ref Volatile.Read(ref _counters).AcceptErrors);
					Console.WriteLine("Warning: Exception accepting a TCP connection - " + e.Message);
					await Task.Delay(TimeSpan.FromMilliseconds(100), _timeProvider, cancellationToken);
					continue;
				}

				var address = client.Client.RemoteEndPoint is IPEndPoint endpoint
					? endpoint.Address
					: IPAddress.None;
				if (RecordConnectionAttempt(address, _timeProvider.GetUtcNow().UtcDateTime))
				{
					Interlocked.Increment(ref Volatile.Read(ref _counters).FloodRejectedConnections);
					client.Dispose();
					continue;
				}

				Console.WriteLine("Accepted TCP connection from {0}", client.Client.RemoteEndPoint);
				var connection = new PlayerConnection(
					new SocketConnectionTransport(client), _timeProvider, this);
				_activeConnections.TryAdd(connection, 0);
				Interlocked.Increment(ref Volatile.Read(ref _counters).AcceptedConnections);
				_pendingConnections.Enqueue(connection);
				_ = RemoveCompletedConnectionAsync(connection);
			}
		}
		finally
		{
			Volatile.Write(ref _isListening, 0);
			listener.Stop();
		}
	}

	private async Task RemoveCompletedConnectionAsync(PlayerConnection connection)
	{
		await connection.TransportCompletion;
		_activeConnections.TryRemove(connection, out _);
	}

	private static async Task ObserveExpectedCancellationAsync(Task task)
	{
		try
		{
			await task;
		}
		catch (OperationCanceledException)
		{
		}
	}

	private static void RecordMaximum(ref long target, long value)
	{
		var current = Volatile.Read(ref target);
		while (value > current)
		{
			var observed = Interlocked.CompareExchange(ref target, value, current);
			if (observed == current)
			{
				return;
			}

			current = observed;
		}
	}

	private IReadOnlyList<IPlayerConnection> ConnectionSnapshot =>
		_connectionRegistry?.Snapshot ?? _connections?.ToArray() ?? [];

	private sealed class NetworkCounters
	{
		public long AcceptedConnections;
		public long FloodRejectedConnections;
		public long BytesReceived;
		public long BytesSent;
		public long ReadOperations;
		public long WriteOperations;
		public long InputQueueHighWatermark;
		public long OutputQueueHighWatermarkBytes;
		public long SlowClientDisconnects;
		public long AcceptErrors;
		public long ReadErrors;
		public long WriteErrors;
	}
}

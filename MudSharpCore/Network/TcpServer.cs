using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MudSharp.Network;

public sealed class TCPServer : IServer
{
	private AddConnectionCallback _addConnection;
	private IEnumerable<IPlayerConnection> _connections;

	private Task _listeningThread;
	private CancellationTokenSource _listeningThreadCancellationToken;

	public TCPServer(IPAddress host, int port)
	{
		IPAddress = host;
		Port = port;
	}

	public IPAddress IPAddress { get; }

	public int Port { get; }

	public bool IsListeningAndResponding =>
		_listeningThread?.Status.In(TaskStatus.Running, TaskStatus.WaitingForActivation) == true;

	public void Bind(IEnumerable<IPlayerConnection> connectionList, AddConnectionCallback addConnection)
	{
		_connections = connectionList;
		_addConnection = addConnection;
	}

	public void Start()
	{
		if (IsListeningAndResponding)
		{
			throw new ApplicationException("Trying to start an already started TCP Listener.");
		}

		_listeningThreadCancellationToken = new CancellationTokenSource();
		_listeningThread = Task.Factory.StartNew(ListeningDelegate, _listeningThreadCancellationToken.Token);
	}

	public void Stop()
	{
		lock (_connections)
		{
			_listeningThreadCancellationToken.Cancel();
		}

		_listeningThread = null;
	}

	/// <summary>
	/// Forces the processing of all outgoing messages without waiting on the delegate loop
	/// </summary>
	public void ProcessAllOutgoing()
	{
		IEnumerable<IPlayerConnection> connections;
		lock (_connections)
		{
			connections = _connections.Where(x => x.State != ConnectionState.Closed).ToList();
		}

		foreach (var connection in connections)
		{
			if (connection.HasOutgoingCommands)
			{
				connection.SendOutgoing();
			}
		}
	}

	public Dictionary<IPAddress, TcpConnectionInformation> ConnectionDictionary { get; } = new();

	public TimeSpan IpFloodKeepAlive { get; } = TimeSpan.FromSeconds(60);

	public void ListeningDelegate()
	{
		var tcp = new TcpListener(IPAddress.Any, Port);
		tcp.Start();
		Console.WriteLine("Successfully started listening on {0}.", IPAddress);
		while (true)
		{
			IEnumerable<IPlayerConnection> connections;
			lock (_connections)
			{
				connections = _connections.Where(x => x.State != ConnectionState.Closed).ToList();
			}

			foreach (var connection in connections)
			{
				if (connection.HasOutgoingCommands)
				{
					connection.SendOutgoing();
				}

				connection.PrepareIncoming();
			}

			lock (_connections)
			{
				connections = _connections.Where(x => x.State == ConnectionState.Closing).ToList();
			}

			foreach (var connection in connections)
			{
				connection.Dispose();
			}

			while (tcp.Pending())
			{
				var client = tcp.AcceptTcpClient();
				var address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;
				if (ConnectionDictionary.ContainsKey(address))
				{
					ConnectionDictionary[address].NumberOfConnections += 1;
				}
				else
				{
					ConnectionDictionary[address] = new TcpConnectionInformation
						{ StartOfPeriod = DateTime.UtcNow, NumberOfConnections = 1 };
				}

				if (ConnectionDictionary[address].NumberOfConnections > 30)
				{
					client.Client.Shutdown(SocketShutdown.Both);
					client.Close();
					continue;
				}

				Console.WriteLine("Accepted TCP connection from {0}", client.Client.RemoteEndPoint);
				try
				{
					var newCon = new PlayerConnection(client);
					_addConnection(newCon);
				}
				catch (SocketException e)
				{
					Console.WriteLine("SocketException ({0}) in PlayerConnection Constructor: " + e.Message,
						e.ErrorCode);
				}
				catch (ObjectDisposedException e)
				{
					Console.WriteLine("ObjectDisposedException in PlayerConnection Constructor: " + e.Message);
				}
			}

			foreach (var connection in ConnectionDictionary.ToArray())
			{
				if (connection.Value.NumberOfConnections < 30 &&
				    DateTime.UtcNow - connection.Value.StartOfPeriod > IpFloodKeepAlive)
				{
					ConnectionDictionary.Remove(connection.Key);
				}
			}

			Thread.Sleep(100);
		}
	}
}
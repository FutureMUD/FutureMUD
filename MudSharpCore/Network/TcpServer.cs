using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MudSharp.Network;

public sealed class TCPServer : IServer
{
    private AddConnectionCallback _addConnection;
    private IEnumerable<IPlayerConnection> _connections;

    private TcpListener _listener;
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
        _listeningThread = Task.Factory.StartNew(ListeningDelegate, _listeningThreadCancellationToken.Token,
            TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    public void Stop()
    {
        _listeningThreadCancellationToken?.Cancel();
        _listener?.Stop();
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

        foreach (IPlayerConnection connection in connections)
        {
            if (connection.HasOutgoingCommands)
            {
                connection.SendOutgoing();
            }
        }
    }

    public Dictionary<IPAddress, TcpConnectionInformation> ConnectionDictionary { get; } = new();

    public TimeSpan IpFloodKeepAlive { get; } = TimeSpan.FromSeconds(60);

    internal bool RecordConnectionAttempt(IPAddress address, DateTime utcNow)
    {
        PruneConnectionDictionary(utcNow);
        if (ConnectionDictionary.TryGetValue(address, out TcpConnectionInformation info))
        {
            info.NumberOfConnections += 1;
        }
        else
        {
            ConnectionDictionary[address] = info = new TcpConnectionInformation
            { StartOfPeriod = utcNow, NumberOfConnections = 1 };
        }

        return info.NumberOfConnections > 30;
    }

    internal void PruneConnectionDictionary(DateTime utcNow)
    {
        foreach (KeyValuePair<IPAddress, TcpConnectionInformation> connection in ConnectionDictionary.ToArray())
        {
            if (utcNow - connection.Value.StartOfPeriod > IpFloodKeepAlive)
            {
                ConnectionDictionary.Remove(connection.Key);
            }
        }
    }

    public void ListeningDelegate()
    {
        TcpListener tcp = new(IPAddress, Port);
        _listener = tcp;
        try
        {
            tcp.Start();
            ConsoleUtilities.WriteLine("Successfully started listening on #2{0}#0.", IPAddress);
            while (!_listeningThreadCancellationToken.IsCancellationRequested)
            {
                IEnumerable<IPlayerConnection> connections;
                lock (_connections)
                {
                    connections = _connections.Where(x => x.State != ConnectionState.Closed).ToList();
                }

                foreach (IPlayerConnection connection in connections)
                {
                    try
                    {
                        if (connection.HasOutgoingCommands)
                        {
                            connection.SendOutgoing();
                        }

                        connection.PrepareIncoming();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Warning: Exception in TCPServer.ListeningDelegate connection poll - " + e);
                        connection.State = ConnectionState.Closing;
                    }
                }

                lock (_connections)
                {
                    connections = _connections.Where(x => x.State == ConnectionState.Closing).ToList();
                }

                foreach (IPlayerConnection connection in connections)
                {
                    connection.Dispose();
                }

                while (tcp.Pending())
                {
                    TcpClient client = tcp.AcceptTcpClient();
                    IPAddress address = ((IPEndPoint)client.Client.RemoteEndPoint).Address;

                    if (RecordConnectionAttempt(address, DateTime.UtcNow))
                    {
                        client.Client.Shutdown(SocketShutdown.Both);
                        client.Close();
                        continue;
                    }

                    Console.WriteLine("Accepted TCP connection from {0}", client.Client.RemoteEndPoint);
                    try
                    {
                        PlayerConnection newCon = new(client);
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

                PruneConnectionDictionary(DateTime.UtcNow);

                Thread.Sleep(100);
            }
        }
        catch (SocketException) when (_listeningThreadCancellationToken.IsCancellationRequested)
        {
            // Expected when Stop interrupts the listener.
        }
        catch (ObjectDisposedException) when (_listeningThreadCancellationToken.IsCancellationRequested)
        {
            // Expected when Stop interrupts the listener.
        }
        finally
        {
            tcp.Stop();
            if (ReferenceEquals(_listener, tcp))
            {
                _listener = null;
            }
        }
    }
}

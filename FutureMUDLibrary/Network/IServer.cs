using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MudSharp.Network
{
    public delegate void AddConnectionCallback(IPlayerConnection connection);

    public interface IServer
    {
        IPAddress IPAddress { get; }

        int Port { get; }

        bool IsListeningAndResponding { get; }

        void Bind(IEnumerable<IPlayerConnection> connectionList, AddConnectionCallback addConnection);

        void Start();
        void Stop();
    }

	/// <summary>
	/// Optional asynchronous lifecycle implemented by event-driven servers.
	/// </summary>
	public interface IAsyncServer
	{
		ValueTask StartAsync(CancellationToken cancellationToken = default);
		ValueTask StopAsync(CancellationToken cancellationToken = default);
		void ProcessPendingConnections();
	}
}

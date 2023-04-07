using System.Collections.Generic;
using System.Net;

namespace MudSharp.Network {
    public delegate void AddConnectionCallback(IPlayerConnection connection);

    public interface IServer {
        IPAddress IPAddress { get; }

        int Port { get; }

        bool IsListeningAndResponding { get; }

        void Bind(IEnumerable<IPlayerConnection> connectionList, AddConnectionCallback addConnection);

        void Start();
        void Stop();
    }
}
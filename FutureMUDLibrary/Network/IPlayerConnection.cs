using System;
using System.Net.Sockets;
using MudSharp.Framework;

namespace MudSharp.Network
{
    
    public enum ConnectionState {
        Open,
        Closing,
        Closed
    }

    public interface IPlayerConnection : IDisposable
    {
        ConnectionState State { get; set; }
        bool HasIncomingCommands { get; }
        bool HasOutgoingCommands { get; }
        string IP { get; }
        MXPSupport MXPSupport { get; }
        IPlayerController ControlPuppet { get; }
        long InactivityMilliseconds { get; }
        void WarnTimeout();
        void Bind(IFuturemudControlContext context);
        void AttemptCommand();
        void AddOutgoing(string text);
        void PrepareOutgoing();
        void PrepareIncoming();
        void SendOutgoing();
        void Reconnect(TcpClient client);
        void NegotiateClientSet();
    }
}
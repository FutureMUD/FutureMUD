using MudSharp.Framework;
using System;
using System.Threading.Tasks;

namespace MudSharp.Network
{

    public enum ConnectionState
    {
        Open,
        Closing,
        Closed
    }

	public enum ConnectionCloseMode
	{
		Drain,
		Abort
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
        void NegotiateClientSet();
    }

	/// <summary>
	/// Optional event-driven transport lifecycle for player connections.
	/// </summary>
	public interface IAsyncPlayerConnection
	{
		Task TransportCompletion { get; }
		bool IsReadyForDisposal { get; }
		void StartTransport();
		void ProcessPendingTransportEvents();
		void RequestClose(ConnectionCloseMode mode);
	}
}

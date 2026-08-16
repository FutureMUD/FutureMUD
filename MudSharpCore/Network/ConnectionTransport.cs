#nullable enable

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MudSharp.Network;

internal interface IConnectionTransport : IDisposable
{
	string IP { get; }
	EndPoint? RemoteEndPoint { get; }
	ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken);
	ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken);
	void Close();
}

internal sealed class SocketConnectionTransport : IConnectionTransport
{
	private readonly TcpClient _client;
	private readonly Socket _socket;
	private ReadOnlyMemory<byte> _initialBytes;
	private int _closed;

	public SocketConnectionTransport(
		TcpClient client,
		IPAddress? effectiveRemoteAddress = null,
		ReadOnlyMemory<byte> initialBytes = default)
	{
		_client = client;
		_socket = client.Client;
		_initialBytes = initialBytes;
		_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
		IP = effectiveRemoteAddress?.ToString() ?? (_socket.RemoteEndPoint is IPEndPoint endpoint
			? endpoint.Address.ToString()
			: "0.0.0.0");
	}

	public string IP { get; }
	public EndPoint? RemoteEndPoint => _socket.RemoteEndPoint;

	public ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken)
	{
		if (!_initialBytes.IsEmpty)
		{
			var count = Math.Min(buffer.Length, _initialBytes.Length);
			_initialBytes[..count].CopyTo(buffer);
			_initialBytes = _initialBytes[count..];
			return ValueTask.FromResult(count);
		}

		return _socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken);
	}

	public ValueTask<int> SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
	{
		return _socket.SendAsync(buffer, SocketFlags.None, cancellationToken);
	}

	public void Close()
	{
		if (Interlocked.Exchange(ref _closed, 1) != 0)
		{
			return;
		}

		try
		{
			_socket.Shutdown(SocketShutdown.Both);
		}
		catch
		{
			// The peer may already have closed the socket.
		}

		_client.Close();
	}

	public void Dispose()
	{
		Close();
	}
}

internal interface INetworkTelemetrySink
{
	void RecordRead(int bytes);
	void RecordWrite(int bytes);
	void RecordInputQueueDepth(int depth);
	void RecordOutputQueueBytes(long bytes);
	void RecordSlowClientDisconnect();
	void RecordReadError();
	void RecordWriteError();
}

internal sealed class NullNetworkTelemetrySink : INetworkTelemetrySink
{
	public static NullNetworkTelemetrySink Instance { get; } = new();

	public void RecordRead(int bytes) { }
	public void RecordWrite(int bytes) { }
	public void RecordInputQueueDepth(int depth) { }
	public void RecordOutputQueueBytes(long bytes) { }
	public void RecordSlowClientDisconnect() { }
	public void RecordReadError() { }
	public void RecordWriteError() { }
}

#nullable enable

using System.Buffers;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using MudSharp.Character.Name;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Server;

namespace MudSharp.Network;

public class PlayerConnection : IPlayerConnection, IAsyncPlayerConnection
{
	private const int ReadBufferSize = 4096;
	private const int InitialCommandBufferSize = 256;
	private const int InitialNegotiationBufferSize = 32;
	private const int IncomingCommandCapacity = 16;
	private const int ProtocolEventCapacity = 16;
	private const int OutgoingFrameCapacity = 256;
	private const int MaximumQueuedOutputBytes = 2 * 1024 * 1024;
	private const int MaximumPooledOutputBytes = 64 * 1024;
	private static readonly Encoding Latin1Encoding = StringExtensions.Latin1Encoder;
	private static readonly byte[] WillMxp = [Telnet.IAC, Telnet.WILL, Telnet.TELOPT_MXP];
	private static readonly byte[] StartMxp = [Telnet.IAC, Telnet.SB, Telnet.TELOPT_MXP, Telnet.IAC, Telnet.SE];
	private static readonly byte[] WillEor = [Telnet.IAC, Telnet.WILL, Telnet.TELOPT_EOR];
	private static readonly byte[] DoEor = [Telnet.IAC, Telnet.DO, Telnet.TELOPT_EOR];
	private static readonly byte[] Prompt = [Telnet.IAC, Telnet.GA];
	private static readonly byte[] AlternatePrompt = [Telnet.IAC, Telnet.EOR];
	private static readonly byte[] BeginWillNegotiation = [Telnet.IAC, Telnet.WILL];
	private static readonly byte[] DoMxp = [Telnet.IAC, Telnet.DO, Telnet.TELOPT_MXP];
	private static readonly byte[] SupportsBytes = Encoding.ASCII.GetBytes("\x1B[1z<SUPPORTS");
	private static readonly byte[] WillCharset = [Telnet.IAC, Telnet.WILL, Telnet.CHARSET];
	private static readonly byte[] DoCharset = [Telnet.IAC, Telnet.DO, Telnet.CHARSET];
	private static readonly byte[] DontCharset = [Telnet.IAC, Telnet.DONT, Telnet.CHARSET];
	private static readonly byte[] RequestUtf8 = BuildSubnegotiation(Telnet.REQUEST, " UTF-8");
	private static readonly byte[] AcknowledgeUtf8 = BuildSubnegotiation(Telnet.ACCEPTED, "UTF-8");
	private static readonly byte[] RejectUtf8 = BuildSubnegotiation(Telnet.REJECTED, "UTF-8");
	private static readonly byte[] StartMxpPayload = MXP.StartMXPBytes();

	private readonly IConnectionTransport _transport;
	private readonly INetworkTelemetrySink _telemetry;
	private readonly TimeProvider _timeProvider;
	private readonly Channel<string> _incomingCommands;
	private readonly Channel<OutboundFrame> _outgoingFrames;
	private readonly ConcurrentQueue<ProtocolEvent> _protocolEvents = new();
	private readonly StringBuilder _outgoingCommands = new();
	private readonly CancellationTokenSource _readCancellation = new();
	private readonly CancellationTokenSource _writeCancellation = new();
	private readonly TaskCompletionSource _transportCompletion =
		new(TaskCreationOptions.RunContinuationsAsynchronously);
	private byte[] _readBuffer;
	private byte[] _incomingCommandBuffer;
	private int _incomingCommandCount;
	private byte[] _telnetNegotiationBuffer;
	private int _telnetNegotiationCount;
	private bool _inTelnetNegotiation;
	private bool _inTelnetSubcommand;
	private bool _pendingTelnetSubcommandEnd;
	private int _useAlternatePrompt;
	private int _useUnicode;
	private int _state;
	private int _incomingCommandQueueCount;
	private int _protocolEventCount;
	private int _hasOutgoingCommands;
	private long _queuedOutputBytes;
	private long _lastActivityTimestamp;
	private long _lastWarningActivityTimestamp;
	private int _transportStarted;
	private int _drainRequested;
	private int _readyForDisposal;
	private int _disposed;
	private int _buffersReturned;
	private bool _fiveMinuteWarning;
	private bool _twoMinuteWarning;
	private bool _oneMinuteWarning;
	private bool _thirtySecondWarning;

	public PlayerConnection(TcpClient client)
		: this(new SocketConnectionTransport(client), TimeProvider.System, NullNetworkTelemetrySink.Instance)
	{
	}

	internal PlayerConnection(IConnectionTransport transport, TimeProvider timeProvider,
		INetworkTelemetrySink telemetry)
	{
		_transport = transport;
		_timeProvider = timeProvider;
		_telemetry = telemetry;
		_readBuffer = ArrayPool<byte>.Shared.Rent(ReadBufferSize);
		_incomingCommandBuffer = ArrayPool<byte>.Shared.Rent(InitialCommandBufferSize);
		_telnetNegotiationBuffer = ArrayPool<byte>.Shared.Rent(InitialNegotiationBufferSize);
		_incomingCommands = Channel.CreateBounded<string>(new BoundedChannelOptions(IncomingCommandCapacity)
		{
			SingleReader = true,
			SingleWriter = true,
			FullMode = BoundedChannelFullMode.Wait,
			AllowSynchronousContinuations = false
		});
		_outgoingFrames = Channel.CreateBounded<OutboundFrame>(new BoundedChannelOptions(OutgoingFrameCapacity)
		{
			SingleReader = true,
			SingleWriter = false,
			FullMode = BoundedChannelFullMode.Wait,
			AllowSynchronousContinuations = false
		});
		MXPSupport = new MXPSupport();
		_lastActivityTimestamp = _timeProvider.GetTimestamp();
		_lastWarningActivityTimestamp = _lastActivityTimestamp;
	}

	public ConnectionState State
	{
		get => (ConnectionState)Volatile.Read(ref _state);
		set => Volatile.Write(ref _state, (int)value);
	}

	public bool HasIncomingCommands => Volatile.Read(ref _incomingCommandQueueCount) > 0;
	public bool HasOutgoingCommands => Volatile.Read(ref _hasOutgoingCommands) != 0;
	public string IP => _transport.IP;
	public MXPSupport MXPSupport { get; }
	public IPlayerController? ControlPuppet { get; private set; }
	public long InactivityMilliseconds =>
		(long)_timeProvider.GetElapsedTime(Volatile.Read(ref _lastActivityTimestamp)).TotalMilliseconds;
	public Task TransportCompletion => _transportCompletion.Task;
	public bool IsReadyForDisposal => Volatile.Read(ref _readyForDisposal) != 0;

	public void Bind(IFuturemudControlContext context)
	{
		ControlPuppet = context;
		State = ConnectionState.Open;
		var timestamp = _timeProvider.GetTimestamp();
		Volatile.Write(ref _lastActivityTimestamp, timestamp);
		_lastWarningActivityTimestamp = timestamp;
	}

	public void StartTransport()
	{
		if (Interlocked.Exchange(ref _transportStarted, 1) != 0)
		{
			return;
		}

		if (Volatile.Read(ref _disposed) != 0 || State != ConnectionState.Open)
		{
			CompleteWithoutStarting();
			return;
		}

		QueueRaw(WillMxp);
		var readTask = ReadPumpAsync();
		var writeTask = WritePumpAsync();
		_ = ObserveTransportAsync(readTask, writeTask);
	}

	public void ProcessPendingTransportEvents()
	{
		while (_protocolEvents.TryDequeue(out var transportEvent))
		{
			Interlocked.Decrement(ref _protocolEventCount);
			switch (transportEvent.Kind)
			{
				case ProtocolEventKind.EnableMxp:
					MXPSupport.UseMXP = true;
					break;
				case ProtocolEventKind.Supports:
					MXPSupport.SetSupport(transportEvent.Text ?? string.Empty);
					break;
				case ProtocolEventKind.CharsetAccepted:
					if (ControlPuppet?.Account is not null)
					{
						ControlPuppet.Account.UseUnicode = true;
					}
					break;
				case ProtocolEventKind.CharsetRejected:
					if (ControlPuppet?.Account is not null)
					{
						ControlPuppet.Account.UseUnicode = false;
					}
					break;
			}
		}
	}

	public void AttemptCommand()
	{
		if (!_incomingCommands.Reader.TryRead(out var command))
		{
			return;
		}

		Interlocked.Decrement(ref _incomingCommandQueueCount);
#if DEBUG
		ControlPuppet?.HandleCommand(command.TrimEnd('\n'));
#else
		try
		{
			ControlPuppet?.HandleCommand(command.TrimEnd('\n'));
		}
		catch (Exception e)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Crash during player input");
			if (ControlPuppet is IFuturemudControlContext fcc)
			{
				sb.AppendLine($"Account: {fcc.Account?.Name ?? "N/A"}");
				var actor = fcc.Actor;
				if (actor is not null)
				{
					sb.AppendLine($"Character: #{actor.Id:N0} {actor.PersonalName.GetName(NameStyle.FullName)} - {actor.HowSeen(actor, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}");
					foreach (var item in actor
					         .CombinedEffectsOfType<IBuilderEditingEffect>()
					         .SelectNotNull(x => x?.EditingItem as IFrameworkItem))
					{
						sb.AppendLine($"Editing: {item}");
					}
				}
			}

			sb.AppendLine("Input:");
			sb.AppendLine();
			sb.AppendLine(command);
			sb.AppendLine();
			sb.AppendLine("Exception:");
			sb.AppendLine();
			sb.AppendLine(e.ToString());
			Server.MudSharp.WriteCrashLog(sb.ToString());
			Environment.Exit(0);
		}
#endif
	}

	public void AddOutgoing(string text)
	{
		var overflow = false;
		lock (_outgoingCommands)
		{
			if ((_outgoingCommands.Length + text.Length) * sizeof(char) > MaximumQueuedOutputBytes)
			{
				overflow = true;
			}
			else
			{
				_outgoingCommands.AppendLine(text);
				Volatile.Write(ref _hasOutgoingCommands, 1);
			}
		}

		if (overflow)
		{
			_telemetry.RecordSlowClientDisconnect();
			RequestClose(ConnectionCloseMode.Abort);
		}
	}

	public void PrepareOutgoing()
	{
		var controlPuppet = ControlPuppet;
		var outputHandler = controlPuppet?.OutputHandler;
		if (controlPuppet is null || outputHandler is null)
		{
			return;
		}

		Volatile.Write(ref _useUnicode, controlPuppet.Account?.UseUnicode == true ? 1 : 0);
		if (controlPuppet.Closing)
		{
			RequestClose(ConnectionCloseMode.Drain);
			return;
		}

		if (State == ConnectionState.Open && HasTimedOut())
		{
			if (controlPuppet.Timeout != 0)
			{
				AddOutgoing($"{"[System Message]".Colour(Telnet.Green)} You have been timed out due to inactivity.");
			}

			SendOutgoing();
			RequestClose(ConnectionCloseMode.Drain);
			return;
		}

		if (!outputHandler.HasBufferedOutput)
		{
			return;
		}

		controlPuppet.CuePrompt();
		controlPuppet.UpdateObservers();
		lock (_outgoingCommands)
		{
			_outgoingCommands.Append(
				(controlPuppet.Account is not null
					? outputHandler.BufferedOutput.Wrap(controlPuppet.Account.LineFormatLength)
					: outputHandler.BufferedOutput)
				.SanitiseMXP(MXPSupport));
			outputHandler.Flush();
			Volatile.Write(ref _hasOutgoingCommands, 1);
		}
	}

	/// <summary>
	/// Compatibility no-op. Incoming data is continuously processed by the asynchronous read pump.
	/// </summary>
	public void PrepareIncoming()
	{
	}

	public void SendOutgoing()
	{
		if (ControlPuppet is null)
		{
			return;
		}

		string text;
		lock (_outgoingCommands)
		{
			text = _outgoingCommands.ToString();
			_outgoingCommands.Clear();
			if (_outgoingCommands.Capacity > MaximumPooledOutputBytes)
			{
				_outgoingCommands.Capacity = 1024;
			}
			Volatile.Write(ref _hasOutgoingCommands, 0);
		}

		var encoding = Volatile.Read(ref _useUnicode) != 0 ? Encoding.UTF8 : Latin1Encoding;
		var prompt = Volatile.Read(ref _useAlternatePrompt) != 0 ? AlternatePrompt : Prompt;
		var budgetBytes = encoding.GetByteCount(text) + prompt.Length;
		QueueFrame(new OutboundFrame(text, encoding, prompt, default, budgetBytes));
	}

	public void NegotiateClientSet()
	{
		QueueRaw(WillCharset);
	}

	public void WarnTimeout()
	{
		var activityTimestamp = Volatile.Read(ref _lastActivityTimestamp);
		if (activityTimestamp != _lastWarningActivityTimestamp)
		{
			ResetWarnings();
			_lastWarningActivityTimestamp = activityTimestamp;
		}

		if (ControlPuppet is null || ControlPuppet.Timeout <= 0)
		{
			return;
		}

		var timeLeft = ControlPuppet.Timeout - InactivityMilliseconds;
		if (!_thirtySecondWarning && timeLeft <= 30000)
		{
			AddOutgoing($"{"[System Message]".Colour(Telnet.Green)} You will time out in 30 seconds unless you do something.\n");
			_thirtySecondWarning = true;
			return;
		}

		if (!_oneMinuteWarning && timeLeft <= 60000)
		{
			AddOutgoing($"{"[System Message]".Colour(Telnet.Green)} You will time out in 1 minute unless you do something.\n");
			_oneMinuteWarning = true;
			return;
		}

		if (!_twoMinuteWarning && timeLeft <= 120000)
		{
			AddOutgoing($"{"[System Message]".Colour(Telnet.Green)} You will time out in 2 minutes unless you do something.\n");
			_twoMinuteWarning = true;
			return;
		}

		if (!_fiveMinuteWarning && timeLeft <= 300000)
		{
			AddOutgoing($"{"[System Message]".Colour(Telnet.Green)} You will time out in 5 minutes unless you do something.\n");
			_fiveMinuteWarning = true;
		}
	}

	public void RequestClose(ConnectionCloseMode mode)
	{
		State = State == ConnectionState.Closed ? ConnectionState.Closed : ConnectionState.Closing;
		_incomingCommands.Writer.TryComplete();
		if (mode == ConnectionCloseMode.Abort)
		{
			_readCancellation.Cancel();
			_writeCancellation.Cancel();
			_outgoingFrames.Writer.TryComplete();
			_transport.Close();
			Volatile.Write(ref _readyForDisposal, 1);
			if (Volatile.Read(ref _transportStarted) == 0)
			{
				CompleteWithoutStarting();
			}

			return;
		}

		if (Interlocked.Exchange(ref _drainRequested, 1) != 0)
		{
			return;
		}

		_readCancellation.Cancel();
		_outgoingFrames.Writer.TryComplete();
		if (Volatile.Read(ref _transportStarted) == 0)
		{
			_transport.Close();
			Volatile.Write(ref _readyForDisposal, 1);
			CompleteWithoutStarting();
			return;
		}

		_ = EnforceDrainTimeoutAsync();
	}

	public void Dispose()
	{
		if (Interlocked.Exchange(ref _disposed, 1) != 0)
		{
			return;
		}

		var controlPuppet = ControlPuppet;
		ControlPuppet = null;
		controlPuppet?.DetachConnection();
		State = ConnectionState.Closed;
		Futuremud.Games.FirstOrDefault()?.Destroy(this);
		RequestClose(ConnectionCloseMode.Abort);
		GC.SuppressFinalize(this);
	}

	private async Task ReadPumpAsync()
	{
		try
		{
			while (!_readCancellation.IsCancellationRequested)
			{
				var bytes = await _transport.ReceiveAsync(
					_readBuffer.AsMemory(0, ReadBufferSize), _readCancellation.Token);
				if (bytes == 0)
				{
					RequestClose(ConnectionCloseMode.Abort);
					return;
				}

				_telemetry.RecordRead(bytes);
				await ProcessReceivedBytesAsync(_readBuffer.AsMemory(0, bytes), _readCancellation.Token);
			}
		}
		catch (OperationCanceledException) when (_readCancellation.IsCancellationRequested)
		{
		}
		catch (ChannelClosedException) when (State != ConnectionState.Open)
		{
		}
		catch (Exception)
		{
			if (State == ConnectionState.Open)
			{
				_telemetry.RecordReadError();
			}

			RequestClose(ConnectionCloseMode.Abort);
		}
	}

	private async Task WritePumpAsync()
	{
		try
		{
			await foreach (var frame in _outgoingFrames.Reader.ReadAllAsync(_writeCancellation.Token))
			{
				try
				{
					if (!frame.Raw.IsEmpty)
					{
						await SendAllAsync(frame.Raw, _writeCancellation.Token);
					}
					else
					{
						await SendEncodedAsync(frame.Encoding!, frame.Text!, _writeCancellation.Token);
						await SendAllAsync(frame.Prompt, _writeCancellation.Token);
					}
				}
				finally
				{
					Interlocked.Add(ref _queuedOutputBytes, -frame.BudgetBytes);
				}
			}

			if (Volatile.Read(ref _drainRequested) != 0)
			{
				_transport.Close();
				Volatile.Write(ref _readyForDisposal, 1);
			}
		}
		catch (OperationCanceledException) when (_writeCancellation.IsCancellationRequested)
		{
		}
		catch (Exception)
		{
			if (State == ConnectionState.Open || Volatile.Read(ref _drainRequested) != 0)
			{
				_telemetry.RecordWriteError();
			}

			RequestClose(ConnectionCloseMode.Abort);
		}
		finally
		{
			if (Volatile.Read(ref _drainRequested) != 0 && !IsReadyForDisposal)
			{
				_transport.Close();
				Volatile.Write(ref _readyForDisposal, 1);
			}
		}
	}

	private async Task ObserveTransportAsync(Task readTask, Task writeTask)
	{
		try
		{
			await Task.WhenAll(readTask, writeTask);
		}
		finally
		{
			_transport.Dispose();
			ReturnBuffers();
			_transportCompletion.TrySetResult();
		}
	}

	private async Task EnforceDrainTimeoutAsync()
	{
		await Task.Delay(TimeSpan.FromSeconds(2), _timeProvider, CancellationToken.None);
		if (!IsReadyForDisposal)
		{
			RequestClose(ConnectionCloseMode.Abort);
		}
	}

	private async ValueTask ProcessReceivedBytesAsync(Memory<byte> memory, CancellationToken cancellationToken)
	{
		var encoding = Volatile.Read(ref _useUnicode) != 0 ? Encoding.UTF8 : Latin1Encoding;
		for (var i = 0; i < memory.Length; i++)
		{
			var value = memory.Span[i];
			if (_inTelnetNegotiation)
			{
				if (!TryAppendNegotiationByte(value))
				{
					return;
				}

				if (_telnetNegotiationCount == 2 && value == Telnet.IAC)
				{
					if (!TryAppendCommandByte(Telnet.IAC))
					{
						return;
					}

					ResetTelnetNegotiation();
					continue;
				}

				if (_telnetNegotiationCount == 2 && value == Telnet.SB)
				{
					_inTelnetSubcommand = true;
					continue;
				}

				if (_telnetNegotiationCount == 2 &&
				    value != Telnet.WILL && value != Telnet.WONT && value != Telnet.DO && value != Telnet.DONT)
				{
					HandleTelnetNegotiation(_telnetNegotiationBuffer.AsSpan(0, _telnetNegotiationCount));
					ResetTelnetNegotiation();
					continue;
				}

				if (_inTelnetSubcommand)
				{
					if (_pendingTelnetSubcommandEnd)
					{
						if (value == Telnet.SE)
						{
							HandleTelnetNegotiation(_telnetNegotiationBuffer.AsSpan(0, _telnetNegotiationCount));
							ResetTelnetNegotiation();
							continue;
						}

						_pendingTelnetSubcommandEnd = false;
					}

					if (value == Telnet.IAC)
					{
						_pendingTelnetSubcommandEnd = true;
					}

					continue;
				}

				if (_telnetNegotiationCount >= 3)
				{
					HandleTelnetNegotiation(_telnetNegotiationBuffer.AsSpan(0, _telnetNegotiationCount));
					ResetTelnetNegotiation();
				}

				continue;
			}

			if (value == Telnet.IAC)
			{
				_inTelnetNegotiation = true;
				_telnetNegotiationCount = 0;
				TryAppendNegotiationByte(value);
				continue;
			}

			if (value == (byte)'\r')
			{
				await EnqueueCommandAsync(encoding, cancellationToken);
				continue;
			}

			if (value == (byte)'\n')
			{
				if (_incomingCommandCount > 0)
				{
					await EnqueueCommandAsync(encoding, cancellationToken);
				}

				continue;
			}

			if (!TryAppendCommandByte(value))
			{
				return;
			}
		}

		if (_incomingCommandCount >= SupportsBytes.Length &&
		    _incomingCommandBuffer.AsSpan(0, _incomingCommandCount).StartsWith(SupportsBytes))
		{
			await EnqueueCommandAsync(encoding, cancellationToken);
		}
	}

	private async ValueTask EnqueueCommandAsync(Encoding encoding, CancellationToken cancellationToken)
	{
		if (_incomingCommandBuffer.AsSpan(0, _incomingCommandCount).StartsWith(SupportsBytes))
		{
			QueueProtocolEvent(new ProtocolEvent(ProtocolEventKind.Supports,
				Encoding.ASCII.GetString(_incomingCommandBuffer, 0, _incomingCommandCount)));
			ResetCommandBuffer();
			return;
		}

		var command = _incomingCommandCount == 0
			? string.Empty
			: encoding.GetString(_incomingCommandBuffer, 0, _incomingCommandCount);
		ResetCommandBuffer();
		var depth = Interlocked.Increment(ref _incomingCommandQueueCount);
		try
		{
			if (!_incomingCommands.Writer.TryWrite(command))
			{
				await _incomingCommands.Writer.WriteAsync(command, cancellationToken);
			}
		}
		catch
		{
			Interlocked.Decrement(ref _incomingCommandQueueCount);
			throw;
		}

		_telemetry.RecordInputQueueDepth(depth);
		Volatile.Write(ref _lastActivityTimestamp, _timeProvider.GetTimestamp());
	}

	private void HandleTelnetNegotiation(ReadOnlySpan<byte> negotiation)
	{
		if (negotiation.SequenceEqual(DoMxp))
		{
			QueueProtocolEvent(new ProtocolEvent(ProtocolEventKind.EnableMxp, null));
			QueueRaw(StartMxp);
			QueueRaw(StartMxpPayload);
			QueueRaw(WillEor);
			return;
		}

		if (negotiation.SequenceEqual(DoEor))
		{
			Volatile.Write(ref _useAlternatePrompt, 1);
			return;
		}

		if (negotiation.SequenceEqual(DoCharset))
		{
			QueueRaw(RequestUtf8);
			return;
		}

		if (negotiation.SequenceEqual(DontCharset))
		{
			return;
		}

		if (negotiation.SequenceEqual(RejectUtf8))
		{
			Volatile.Write(ref _useUnicode, 0);
			QueueProtocolEvent(new ProtocolEvent(ProtocolEventKind.CharsetRejected, null));
			return;
		}

		if (negotiation.SequenceEqual(AcknowledgeUtf8))
		{
			Volatile.Write(ref _useUnicode, 1);
			QueueProtocolEvent(new ProtocolEvent(ProtocolEventKind.CharsetAccepted, null));
			return;
		}

		if (negotiation.StartsWith(BeginWillNegotiation))
		{
			var response = negotiation.ToArray();
			response[1] = Telnet.WONT;
			QueueRaw(response);
		}
	}

	private bool TryAppendCommandByte(byte value)
	{
		if (_incomingCommandCount >= Constants.PlayerConnectionBufferSize)
		{
			_incomingCommandCount = 0;
			RequestClose(ConnectionCloseMode.Abort);
			return false;
		}

		EnsureCapacity(ref _incomingCommandBuffer, _incomingCommandCount + 1);
		_incomingCommandBuffer[_incomingCommandCount++] = value;
		return true;
	}

	private bool TryAppendNegotiationByte(byte value)
	{
		if (_telnetNegotiationCount >= Constants.PlayerConnectionBufferSize)
		{
			ResetTelnetNegotiation();
			RequestClose(ConnectionCloseMode.Abort);
			return false;
		}

		EnsureCapacity(ref _telnetNegotiationBuffer, _telnetNegotiationCount + 1);
		_telnetNegotiationBuffer[_telnetNegotiationCount++] = value;
		return true;
	}

	private static void EnsureCapacity(ref byte[] buffer, int required)
	{
		if (buffer.Length >= required)
		{
			return;
		}

		var replacement = ArrayPool<byte>.Shared.Rent(Math.Min(Constants.PlayerConnectionBufferSize,
			Math.Max(required, buffer.Length * 2)));
		buffer.AsSpan().CopyTo(replacement);
		ArrayPool<byte>.Shared.Return(buffer);
		buffer = replacement;
	}

	private void ResetCommandBuffer()
	{
		_incomingCommandCount = 0;
		if (_incomingCommandBuffer.Length <= ReadBufferSize)
		{
			return;
		}

		ArrayPool<byte>.Shared.Return(_incomingCommandBuffer);
		_incomingCommandBuffer = ArrayPool<byte>.Shared.Rent(InitialCommandBufferSize);
	}

	private void ResetTelnetNegotiation()
	{
		_telnetNegotiationCount = 0;
		_inTelnetNegotiation = false;
		_inTelnetSubcommand = false;
		_pendingTelnetSubcommandEnd = false;
		if (_telnetNegotiationBuffer.Length <= ReadBufferSize)
		{
			return;
		}

		ArrayPool<byte>.Shared.Return(_telnetNegotiationBuffer);
		_telnetNegotiationBuffer = ArrayPool<byte>.Shared.Rent(InitialNegotiationBufferSize);
	}

	private bool QueueRaw(ReadOnlyMemory<byte> bytes)
	{
		return bytes.IsEmpty || QueueFrame(new OutboundFrame(null, null, default, bytes, bytes.Length));
	}

	private bool QueueProtocolEvent(ProtocolEvent protocolEvent)
	{
		if (Interlocked.Increment(ref _protocolEventCount) > ProtocolEventCapacity)
		{
			Interlocked.Decrement(ref _protocolEventCount);
			RequestClose(ConnectionCloseMode.Abort);
			return false;
		}

		_protocolEvents.Enqueue(protocolEvent);
		return true;
	}

	private bool QueueFrame(OutboundFrame frame)
	{
		var queued = Interlocked.Add(ref _queuedOutputBytes, frame.BudgetBytes);
		if (queued > MaximumQueuedOutputBytes)
		{
			Interlocked.Add(ref _queuedOutputBytes, -frame.BudgetBytes);
			_telemetry.RecordSlowClientDisconnect();
			RequestClose(ConnectionCloseMode.Abort);
			return false;
		}

		if (!_outgoingFrames.Writer.TryWrite(frame))
		{
			Interlocked.Add(ref _queuedOutputBytes, -frame.BudgetBytes);
			if (State == ConnectionState.Open)
			{
				_telemetry.RecordSlowClientDisconnect();
			}

			RequestClose(ConnectionCloseMode.Abort);
			return false;
		}

		_telemetry.RecordOutputQueueBytes(queued);
		return true;
	}

	private async ValueTask SendEncodedAsync(Encoding encoding, string text, CancellationToken cancellationToken)
	{
		if (text.Length == 0)
		{
			return;
		}

		var byteCount = encoding.GetByteCount(text);
		if (byteCount > MaximumPooledOutputBytes)
		{
			await SendAllAsync(encoding.GetBytes(text), cancellationToken);
			return;
		}

		var buffer = ArrayPool<byte>.Shared.Rent(byteCount);
		try
		{
			var bytesWritten = encoding.GetBytes(text.AsSpan(), buffer);
			await SendAllAsync(buffer.AsMemory(0, bytesWritten), cancellationToken);
		}
		finally
		{
			ArrayPool<byte>.Shared.Return(buffer);
		}
	}

	private async ValueTask SendAllAsync(ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken)
	{
		var offset = 0;
		while (offset < bytes.Length)
		{
			var sent = await _transport.SendAsync(bytes[offset..], cancellationToken);
			if (sent <= 0)
			{
				throw new SocketException((int)SocketError.ConnectionReset);
			}

			offset += sent;
			_telemetry.RecordWrite(sent);
		}
	}

	private void CompleteWithoutStarting()
	{
		_transport.Dispose();
		ReturnBuffers();
		_transportCompletion.TrySetResult();
	}

	private void ReturnBuffers()
	{
		if (Interlocked.Exchange(ref _buffersReturned, 1) != 0)
		{
			return;
		}

		ArrayPool<byte>.Shared.Return(_readBuffer);
		ArrayPool<byte>.Shared.Return(_incomingCommandBuffer);
		ArrayPool<byte>.Shared.Return(_telnetNegotiationBuffer);
	}

	private void ResetWarnings()
	{
		_fiveMinuteWarning = false;
		_twoMinuteWarning = false;
		_oneMinuteWarning = false;
		_thirtySecondWarning = false;
	}

	private bool HasTimedOut()
	{
		var timeout = ControlPuppet?.Timeout ?? 0;
		return timeout > 0 && InactivityMilliseconds > timeout;
	}

	private static byte[] BuildSubnegotiation(byte option, string payload)
	{
		var payloadBytes = Encoding.ASCII.GetBytes(payload);
		var result = new byte[payloadBytes.Length + 6];
		result[0] = Telnet.IAC;
		result[1] = Telnet.SB;
		result[2] = Telnet.CHARSET;
		result[3] = option;
		payloadBytes.CopyTo(result, 4);
		result[^2] = Telnet.IAC;
		result[^1] = Telnet.SE;
		return result;
	}

	private readonly record struct OutboundFrame(
		string? Text,
		Encoding? Encoding,
		ReadOnlyMemory<byte> Prompt,
		ReadOnlyMemory<byte> Raw,
		int BudgetBytes);

	private readonly record struct ProtocolEvent(ProtocolEventKind Kind, string? Text);

	private enum ProtocolEventKind
	{
		EnableMxp,
		Supports,
		CharsetAccepted,
		CharsetRejected
	}
}

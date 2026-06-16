using MudSharp.Character.Name;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MudSharp.Network;

public class PlayerConnection : IPlayerConnection
{
    private static readonly byte[] WillMxp = [Telnet.IAC, Telnet.WILL, Telnet.TELOPT_MXP];

    private static readonly byte[] StartMxp =
    [
        Telnet.IAC, Telnet.SB, Telnet.TELOPT_MXP, Telnet.IAC, Telnet.SE
    ];

    private static readonly byte[] WillEOR = [Telnet.IAC, Telnet.WILL, Telnet.TELOPT_EOR];

    private static readonly byte[] DoEOR = [Telnet.IAC, Telnet.DO, Telnet.TELOPT_EOR];

    private static readonly byte[] Prompt =
    [
        Telnet.IAC,
        Telnet.GA
    ];

    private static readonly byte[] AlternatePrompt =
    [
        Telnet.IAC,
        Telnet.EOR
    ];

    private static readonly byte[] BeginWillNegotiation = [Telnet.IAC, Telnet.WILL];

    private static readonly byte[] DoMxp = [Telnet.IAC, Telnet.DO, Telnet.TELOPT_MXP];
    private static readonly byte[] SupportsBytes = Encoding.ASCII.GetBytes("\x1B[1z<SUPPORTS");

    private static readonly byte[] WillCharset =
    [
        Telnet.IAC,
        Telnet.WILL,
        Telnet.CHARSET
    ];

    private static readonly byte[] DoCharset =
    [
        Telnet.IAC,
        Telnet.DO,
        Telnet.CHARSET
    ];

    private static readonly byte[] DontCharset =
    [
        Telnet.IAC,
        Telnet.DONT,
        Telnet.CHARSET
    ];

    private static readonly byte[] RequestUTF8 =
        new byte[]{
        Telnet.IAC,
        Telnet.SB,
        Telnet.CHARSET,
        Telnet.REQUEST
        }.Concat(" UTF-8"u8.ToArray()).Concat(
            new byte[] { Telnet.IAC, Telnet.SE }
        ).ToArray();

    private static readonly byte[] AcknowledgeUTF8 =
        new byte[]{
            Telnet.IAC,
            Telnet.SB,
            Telnet.CHARSET,
            Telnet.ACCEPTED
        }.Concat("UTF-8"u8.ToArray()).Concat(
            new byte[] { Telnet.IAC, Telnet.SE }
        ).ToArray();

    private static readonly byte[] RejectUTF8 =
        new byte[]{
            Telnet.IAC,
            Telnet.SB,
            Telnet.CHARSET,
            Telnet.REJECTED
        }.Concat("UTF-8"u8.ToArray()).Concat(
            new byte[] { Telnet.IAC, Telnet.SE }
        ).ToArray();

    private readonly TcpClient _client;
    private readonly Stopwatch _inactivityStopwatch = new();
    private readonly Queue<string> _incomingCommands = new();
    private readonly List<byte> _incomingCommandBuffer = new(Constants.PlayerConnectionBufferSize);
    private readonly byte[] _inputBuffer = new byte[Constants.PlayerConnectionBufferSize];
    private readonly StringBuilder _outgoingCommands = new();
    private readonly List<byte> _telnetNegotiationBuffer = new(32);
    private bool _useAlternatePrompt;
    private bool _inTelnetNegotiation;
    private bool _inTelnetSubcommand;
    private bool _pendingTelnetSubcommandEnd;
    private bool _disposed;

    private bool _fiveMinuteWarning,
        _twoMinuteWarning,
        _oneMinuteWarning,
        _thirtySecondWarning;

    private NetworkStream _stream;

    public PlayerConnection(TcpClient client)
    {
        _client = client;
        _client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
        // Negotiate Telnet protocol support
        MXPSupport = new MXPSupport();
        try
        {
            SendAll(WillMxp);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException ({0}) in PlayerConnection Constructor: " + e.Message, e.ErrorCode);
            DiscardConnection();
        }
        catch (ObjectDisposedException e)
        {
            Console.WriteLine("ObjectDisposedException in PlayerConnection Constructor: " + e.Message);
            DiscardConnection();
        }
    }

    public ConnectionState State { get; set; }

    public bool HasIncomingCommands { get; private set; }

    public bool HasOutgoingCommands { get; private set; }

    public string IP
    {
        get
        {
            try
            {
                return _client?.Client?.RemoteEndPoint is IPEndPoint endpoint
                    ? endpoint.Address.ToString()
                    : "0.0.0.0";
            }
            catch (Exception)
            {
                return "0.0.0.0";
            }
        }
    }

    public MXPSupport MXPSupport { get; }

    public IPlayerController ControlPuppet { get; private set; }

    public long InactivityMilliseconds => _inactivityStopwatch.ElapsedMilliseconds;

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    private void ResetWarnings()
    {
        _fiveMinuteWarning = false;
        _twoMinuteWarning = false;
        _oneMinuteWarning = false;
        _thirtySecondWarning = false;
    }

    public void WarnTimeout()
    {
        if (ControlPuppet == null || ControlPuppet.Timeout <= 0)
        {
            return;
        }

        long timeLeft = ControlPuppet.Timeout - InactivityMilliseconds;
        if (!_thirtySecondWarning && timeLeft <= 30000)
        {
            AddOutgoing(
                $"{"[System Message]".Colour(Telnet.Green)} You will time out in 30 seconds unless you do something.\n");
            _thirtySecondWarning = true;
            return;
        }

        if (!_oneMinuteWarning && timeLeft <= 60000)
        {
            AddOutgoing(
                $"{"[System Message]".Colour(Telnet.Green)} You will time out in 1 minute unless you do something.\n");
            _oneMinuteWarning = true;
            return;
        }

        if (!_twoMinuteWarning && timeLeft <= 120000)
        {
            AddOutgoing(
                $"{"[System Message]".Colour(Telnet.Green)} You will time out in 2 minutes unless you do something.\n");
            _twoMinuteWarning = true;
            return;
        }

        if (!_fiveMinuteWarning && timeLeft <= 300000)
        {
            AddOutgoing(
                $"{"[System Message]".Colour(Telnet.Green)} You will time out in 5 minutes unless you do something.\n");
            _fiveMinuteWarning = true;
        }
    }

    public void NegotiateClientSet()
    {
        try
        {
            SendAll(WillCharset);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException ({0}) in PlayerConnection Constructor: " + e.Message, e.ErrorCode);
            DiscardConnection();
        }
        catch (ObjectDisposedException e)
        {
            Console.WriteLine("ObjectDisposedException in PlayerConnection Constructor: " + e.Message);
            DiscardConnection();
        }
    }

    private void Dispose(bool disposed)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        ControlPuppet?.DetachConnection();
        ControlPuppet = null;
        State = ConnectionState.Closed;
        Futuremud.Games.FirstOrDefault()?.Destroy(this);
        if (disposed)
        {
            try
            {
                _client?.Client?.Shutdown(SocketShutdown.Both);
            }
            catch
            {
                // We don't care about exceptions at this stage, we're already disposing of the socket
            }

            _stream?.Dispose();
            _client?.Close();
        }
    }

    private void DiscardConnection()
    {
        State = ConnectionState.Closing;
        ControlPuppet?.DetachConnection();
        ControlPuppet = null;
    }

    public void Bind(IFuturemudControlContext context)
    {
        ControlPuppet = context;
        try
        {
            _stream = _client.GetStream();
            State = ConnectionState.Open;
            _inactivityStopwatch.Start();
        }
        catch (ObjectDisposedException e)
        {
            Console.WriteLine("ObjectDisposedException in PlayerConnection.Bind: " + e.Message);
            State = ConnectionState.Closed;
            ControlPuppet = null;
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine("InvalidOperationException in PlayerConnection.Bind: " + e.Message);
            State = ConnectionState.Closed;
            ControlPuppet = null;
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException ({0}) in PlayerConnection.Bind: " + e.Message, e.ErrorCode);
            State = ConnectionState.Closed;
            ControlPuppet = null;
        }
    }

    public void AttemptCommand()
    {
        string cmd = GetNextCommand();
#if DEBUG
#else
		try
		{
#endif

        ControlPuppet.HandleCommand(cmd);
#if DEBUG
#else
		}
		catch (Exception e)
		{
			var sb = new StringBuilder();
			sb.AppendLine("Crash during player input");
			if (ControlPuppet is IFuturemudControlContext fcc)
			{
				sb.AppendLine($"Account: {fcc.Account?.Name ?? "N/A"}");
				if (fcc.Actor is not null)
				{
					sb.AppendLine($"Character: #{fcc.Actor.Id:N0} {fcc.Actor.PersonalName.GetName(NameStyle.FullName)} - {fcc.Actor.HowSeen(fcc.Actor, colour: false, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf)}");
					var edit = fcc.Actor
					              .CombinedEffectsOfType<IBuilderEditingEffect>()
					              .SelectNotNull(x => x.EditingItem as IFrameworkItem)
					              .ToList();
					foreach (var item in edit)
					{
						sb.AppendLine($"Editing: {item.ToString()}");
					}
				}
			}
			sb.AppendLine("Input:");
			sb.AppendLine();
			sb.AppendLine(cmd);
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
        lock (_outgoingCommands)
        {
            _outgoingCommands.AppendLine(text);
        }

        HasOutgoingCommands = true;
    }

    public void PrepareOutgoing()
    {
        try
        {
            var controlPuppet = ControlPuppet;
            var outputHandler = controlPuppet?.OutputHandler;
            if (controlPuppet is null || outputHandler is null)
            {
                return;
            }

            if (controlPuppet.Closing)
            {
                State = ConnectionState.Closing;
                return;
            }

            if (State == ConnectionState.Open && HasTimedOut())
            {
                if (controlPuppet.Timeout != 0)
                {
                    AddOutgoing(
                        $"{"[System Message]".Colour(Telnet.Green)} You have been timed out due to inactivity.");
                }

                SendOutgoing();
                State = ConnectionState.Closing;
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
                    (controlPuppet.Account != null
                        ? outputHandler.BufferedOutput.Wrap(controlPuppet.Account.LineFormatLength)
                        : outputHandler.BufferedOutput)
                    .SanitiseMXP(MXPSupport));
                outputHandler.Flush();
                HasOutgoingCommands = true;
                if (State == ConnectionState.Closing)
                {
                    SendOutgoing();
                }
            }
        }
        catch (SocketException e)
        {
            if (!e.NativeErrorCode.Equals(10035))
            {
                Console.WriteLine("Warning: Exception in PlayerConnection.PrepareOutgoing - " + e);
                DiscardConnection();
            }
        }
#if DEBUG
#else
		catch (Exception e)
		{
			Console.WriteLine("Warning: Exception in PlayerConnection.PrepareOutgoing - " + e);
			DiscardConnection();
		}
#endif
    }

    private void EnqueueCommand(Encoding encoding, List<byte> bytes)
    {
        if (bytes.Take(SupportsBytes.Length).SequenceEqual(SupportsBytes))
        {
            MXPSupport.SetSupport(Encoding.ASCII.GetString(bytes.ToArray()));
            return;
        }

        string command = bytes.Count > 0 ? encoding.GetString(bytes.ToArray()) : "";
#if DEBUG
        Console.WriteLine($"Player Command: {command}");
#endif
        lock (_incomingCommands)
        {
            _incomingCommands.Enqueue(command);
            HasIncomingCommands = true;
        }

        _inactivityStopwatch.Restart();
        ResetWarnings();
    }

    private void SendAll(byte[] bytes)
    {
        if (bytes.Length == 0)
        {
            return;
        }

        var offset = 0;
        while (offset < bytes.Length)
        {
            var sent = _client.Client.Send(bytes, offset, bytes.Length - offset, SocketFlags.None);
            if (sent <= 0)
            {
                throw new SocketException((int)SocketError.ConnectionReset);
            }

            offset += sent;
        }
    }

    private void HandleTelnetNegotiation(IEnumerable<byte> negotiation)
    {
        byte[] negSeq = negotiation.ToArray();

        if (negSeq.SequenceEqual(DoMxp))
        {
            MXPSupport.UseMXP = true;
            SendAll(StartMxp);
            SendAll(MXP.StartMXPBytes());
            SendAll(WillEOR);
            return;
        }

        if (negSeq.SequenceEqual(DoEOR))
        {
            _useAlternatePrompt = true;
            return;
        }

        if (negSeq.SequenceEqual(DoCharset))
        {
            SendAll(RequestUTF8);
            return;
        }

        if (negSeq.SequenceEqual(DontCharset))
        {
            return;
        }

        if (negSeq.SequenceEqual(RejectUTF8))
        {
            if (ControlPuppet?.Account is not null)
            {
                ControlPuppet.Account.UseUnicode = false;
            }
            return;
        }

        if (negSeq.SequenceEqual(AcknowledgeUTF8))
        {
            if (ControlPuppet?.Account is not null)
            {
                ControlPuppet.Account.UseUnicode = true;
            }
            return;
        }

        if (negSeq.Take(BeginWillNegotiation.Length).SequenceEqual(BeginWillNegotiation))
        {
            negSeq[1] = Telnet.WONT;
            SendAll(negSeq.ToArray());
            return;
        }
    }

    private void ResetTelnetNegotiation()
    {
        _telnetNegotiationBuffer.Clear();
        _inTelnetNegotiation = false;
        _inTelnetSubcommand = false;
        _pendingTelnetSubcommandEnd = false;
    }

    private bool TryAppendIncomingCommandByte(byte value)
    {
        if (_incomingCommandBuffer.Count >= Constants.PlayerConnectionBufferSize)
        {
            DiscardConnection();
            _incomingCommandBuffer.Clear();
            return false;
        }

        _incomingCommandBuffer.Add(value);
        return true;
    }

    private bool TryAppendTelnetNegotiationByte(byte value)
    {
        if (_telnetNegotiationBuffer.Count >= Constants.PlayerConnectionBufferSize)
        {
            DiscardConnection();
            ResetTelnetNegotiation();
            return false;
        }

        _telnetNegotiationBuffer.Add(value);
        return true;
    }

    private void ProcessIncomingByte(Encoding encoding, byte value)
    {
        if (_inTelnetNegotiation)
        {
            if (!TryAppendTelnetNegotiationByte(value))
            {
                return;
            }

            if (_telnetNegotiationBuffer.Count == 2 && value == Telnet.IAC)
            {
                TryAppendIncomingCommandByte(Telnet.IAC);
                ResetTelnetNegotiation();
                return;
            }

            if (_telnetNegotiationBuffer.Count == 2 && value == Telnet.SB)
            {
                _inTelnetSubcommand = true;
                return;
            }

            if (_telnetNegotiationBuffer.Count == 2 &&
                value != Telnet.WILL &&
                value != Telnet.WONT &&
                value != Telnet.DO &&
                value != Telnet.DONT)
            {
                HandleTelnetNegotiation(_telnetNegotiationBuffer);
                ResetTelnetNegotiation();
                return;
            }

            if (_inTelnetSubcommand)
            {
                if (_pendingTelnetSubcommandEnd)
                {
                    if (value == Telnet.SE)
                    {
                        HandleTelnetNegotiation(_telnetNegotiationBuffer);
                        ResetTelnetNegotiation();
                        return;
                    }

                    _pendingTelnetSubcommandEnd = false;
                }

                if (value == Telnet.IAC)
                {
                    _pendingTelnetSubcommandEnd = true;
                }

                return;
            }

            if (_telnetNegotiationBuffer.Count >= 3)
            {
                HandleTelnetNegotiation(_telnetNegotiationBuffer);
                ResetTelnetNegotiation();
            }

            return;
        }

        if (value == Telnet.IAC)
        {
            _inTelnetNegotiation = true;
            _telnetNegotiationBuffer.Clear();
            _telnetNegotiationBuffer.Add(value);
            return;
        }

        if (value == (byte)'\r')
        {
            EnqueueCommand(encoding, _incomingCommandBuffer);
            _incomingCommandBuffer.Clear();
            return;
        }

        if (value == (byte)'\n')
        {
            if (_incomingCommandBuffer.Count > 0)
            {
                EnqueueCommand(encoding, _incomingCommandBuffer);
                _incomingCommandBuffer.Clear();
            }

            return;
        }

        TryAppendIncomingCommandByte(value);
    }

    public void PrepareIncoming()
    {
        try
        {
            if (_client.Client.Poll(0, SelectMode.SelectRead) && _client.Client.Available == 0)
            {
                DiscardConnection();
                return;
            }

            if (!_stream.DataAvailable)
            {
                return;
            }

            int bytes = _stream.Read(_inputBuffer, 0, Constants.PlayerConnectionBufferSize);
            if (bytes == 0)
            {
                DiscardConnection();
                return;
            }

            Encoding encoding = ControlPuppet?.Account?.UseUnicode ?? false
                ? Encoding.UTF8
                : Encoding.GetEncoding("iso-8859-1");

            byte[] byteArray = _inputBuffer.Take(bytes).ToArray();
            for (int i = 0; i < byteArray.Length; i++)
            {
                ProcessIncomingByte(encoding, byteArray[i]);
                if (State == ConnectionState.Closing)
                {
                    return;
                }
            }

            if (_incomingCommandBuffer.Count >= SupportsBytes.Length &&
                _incomingCommandBuffer.Take(SupportsBytes.Length).SequenceEqual(SupportsBytes))
            {
                EnqueueCommand(encoding, _incomingCommandBuffer);
                _incomingCommandBuffer.Clear();
            }
        }
        catch (SocketException e)
        {
            if (!e.NativeErrorCode.Equals(10035))
            {
                DiscardConnection();
            }
        }
        catch (ObjectDisposedException)
        {
            DiscardConnection();
        }
        catch (IOException)
        {
            DiscardConnection();
        }
        catch (InvalidOperationException)
        {
            DiscardConnection();
        }
    }

    // TODO: Thread pool this
    public void SendOutgoing()
    {
        if (ControlPuppet == null)
        {
            return;
        }

        lock (_outgoingCommands)
        {
            try
            {
                if (ControlPuppet.Account?.UseUnicode ?? false)
                {
                    SendAll(Encoding.UTF8.GetBytes(_outgoingCommands.ToString()));
                }
                else
                {
                    SendAll(Encoding.GetEncoding("iso-8859-1").GetBytes(_outgoingCommands.ToString().ConvertToLatin1()));
                }

                SendAll(_useAlternatePrompt ? AlternatePrompt : Prompt);
                _outgoingCommands.Clear();
                HasOutgoingCommands = false;
                if (State == ConnectionState.Closing)
                {
                    Dispose();
                }
            }
            catch (SocketException e)
            {
                if (!e.NativeErrorCode.Equals(10035))
                {
                    DiscardConnection();
                }
            }
            catch (ObjectDisposedException)
            {
                DiscardConnection();
            }
            catch (IOException)
            {
                DiscardConnection();
            }
            catch (InvalidOperationException)
            {
                DiscardConnection();
            }
        }
    }

    private string GetNextCommand()
    {
        string command;
        lock (_incomingCommands)
        {
            command = _incomingCommands.Dequeue();
            HasIncomingCommands = _incomingCommands.Any();
        }

        return command.TrimEnd('\n');
    }

    private bool HasTimedOut()
    {
        var timeout = ControlPuppet?.Timeout ?? 0;
        return timeout > 0 && _inactivityStopwatch.ElapsedMilliseconds > timeout;
    }

}

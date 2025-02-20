﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MudSharp.Character.Name;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Server;

namespace MudSharp.Network;

public class PlayerConnection : IPlayerConnection
{
	private static readonly byte[] ByteSeparators = [(byte)'\n', Telnet.END_IAC];

	private static readonly byte[] WillMxp = [Telnet.IAC, Telnet.WILL, Telnet.TELOPT_MXP, Telnet.END_IAC];

	private static readonly byte[] StartMxp =
	[
		Telnet.IAC, Telnet.SB, Telnet.TELOPT_MXP, Telnet.IAC, Telnet.SE,
		Telnet.END_IAC
	];

	private static readonly byte[] WillEOR = [Telnet.IAC, Telnet.WILL, Telnet.TELOPT_EOR, Telnet.END_IAC];

	private static readonly byte[] DoEOR = [Telnet.IAC, Telnet.DO, Telnet.TELOPT_EOR, Telnet.END_IAC];

	private static readonly byte[] Prompt =
	[
		Telnet.IAC,
		Telnet.GA,
		Telnet.END_IAC
	];

	private static readonly byte[] AlternatePrompt =
	[
		Telnet.IAC,
		Telnet.TELOPT_EOR,
		Telnet.END_IAC
	];

	private static readonly byte[] BeginWillNegotiation = [Telnet.IAC, Telnet.WILL];

	private static readonly byte[] DoMxp = [Telnet.IAC, Telnet.DO, Telnet.TELOPT_MXP];
	private static readonly byte[] DontMXP = [Telnet.IAC, Telnet.DONT, Telnet.TELOPT_MXP];
	private static byte[] _dontMxp = [Telnet.IAC, Telnet.DONT, Telnet.TELOPT_MXP];
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
			new byte[]{Telnet.IAC, Telnet.SE}
		).ToArray();

	private static readonly byte[] AcknowledgeUTF8 =
		new byte[]{
			Telnet.IAC,
			Telnet.SB,
			Telnet.CHARSET,
			Telnet.ACCEPTED
		}.Concat("UTF-8"u8.ToArray()).Concat(
			new byte[] { Telnet.IAC, Telnet.SE}
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
	private readonly byte[] _inputBuffer = new byte[Constants.PlayerConnectionBufferSize];
	private readonly StringBuilder _outgoingCommands = new();
	private bool _useAlternatePrompt;

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
			client.Client.Send(WillMxp);
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
				return _client?.Client?.RemoteEndPoint?.ToString().Split(':')[0] ?? "0.0.0.0";
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
		if (ControlPuppet == null)
		{
			return;
		}

		var timeLeft = ControlPuppet.Timeout - InactivityMilliseconds;
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
			_client.Client.Send(WillCharset);
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

	~PlayerConnection()
	{
		Dispose(false);
	}

	private void Dispose(bool disposed)
	{
		ControlPuppet?.DetachConnection();
		ControlPuppet = null;
		Futuremud.Games.First().Destroy(this);
		State = ConnectionState.Closed;
		try
		{
			_client?.Client?.Shutdown(SocketShutdown.Both);
		}
		catch
		{
			// We don't care about exceptions at this stage, we're already disposing of the socket
		}

		_client?.Close();
		_stream?.Dispose();
	}

	private void DiscardConnection()
	{
		State = ConnectionState.Closing;
		if (ControlPuppet != null)
		{
			ControlPuppet.DetachConnection();
			ControlPuppet = null;
		}
	}

	public void Bind(IFuturemudControlContext context)
	{
		State = ConnectionState.Open;
		ControlPuppet = context;
		try
		{
			_stream = _client.GetStream();
		}
		catch (ObjectDisposedException e)
		{
			Console.WriteLine("ObjectDisposedException in PlayerConnection.Bind: " + e.Message);
			State = ConnectionState.Closed;
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine("InvalidOperationException in PlayerConnection.Bind: " + e.Message);
			State = ConnectionState.Closed;
		}
		catch (SocketException e)
		{
			Console.WriteLine("SocketException ({0}) in PlayerConnection.Bind: " + e.Message, e.ErrorCode);
			State = ConnectionState.Closed;
		}

		_inactivityStopwatch.Start();
	}

	public void AttemptCommand()
	{
		var cmd = GetNextCommand();
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
			if (ControlPuppet?.OutputHandler == null)
			{
				return;
			}

			if (ControlPuppet.Closing)
			{
				State = ConnectionState.Closing;
				return;
			}

			if (State == ConnectionState.Open && HasTimedOut())
			{
				if (ControlPuppet?.Timeout != 0)
				{
					AddOutgoing(
						$"{"[System Message]".Colour(Telnet.Green)} You have been timed out due to inactivity.");
				}

				SendOutgoing();
				State = ConnectionState.Closing;
				return;
			}

			if (!ControlPuppet.OutputHandler.HasBufferedOutput)
			{
				return;
			}

			ControlPuppet.CuePrompt();

			lock (_outgoingCommands)
			{
				_outgoingCommands.Append(
					(ControlPuppet.Account != null
						? ControlPuppet.OutputHandler.BufferedOutput.Wrap(ControlPuppet.Account.LineFormatLength)
						: ControlPuppet.OutputHandler.BufferedOutput)
					.SanitiseMXP(MXPSupport));
				ControlPuppet.OutputHandler.Flush();
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

		var command = bytes.Count > 0 ? encoding.GetString(bytes.ToArray()) : "";
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

	private void HandleTelnetNegotiation(IEnumerable<byte> negotiation)
	{
		var negSeq = negotiation.ToArray();
		var text = Encoding.ASCII.GetString(negSeq);

		if (negSeq.SequenceEqual(DoMxp))
		{
			MXPSupport.UseMXP = true;
			_client.Client.Send(StartMxp);
			_client.Client.Send(MXP.StartMXPBytes());
			_client.Client.Send(WillEOR);
			return;
		}

		if (negSeq.SequenceEqual(DoEOR))
		{
			_useAlternatePrompt = true;
			return;
		}

		if (negSeq.SequenceEqual(DoCharset))
		{
			_client.Client.Send(RequestUTF8);
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
			_client.Client.Send(negSeq.ToArray());
			return;
		}
	}

	public void PrepareIncoming()
	{
		try
		{
			// Temporarily disabled 09-04-2017 suspected as culprit in randomly disconnecting people when the server is lagging
			//// Check if the socket is still open
			//if (_client.Client.Poll(1000, SelectMode.SelectRead) && (_client.Client.Available == 0)) {
			//    DiscardConnection();
			//}

			// Use ACK polling to detect disconnections
			var blocking = _client.Client.Blocking;
			if (_inactivityStopwatch.ElapsedMilliseconds > 15000 &&
			    _inactivityStopwatch.ElapsedMilliseconds % 50 == 0)
			{
				_client.Client.Blocking = false;
				_client.Client.Send(new byte[1], 1, SocketFlags.None);
			}

			_client.Client.Blocking = blocking;

			if (!_stream.DataAvailable)
			{
				return;
			}

			var bytes = _stream.Read(_inputBuffer, 0, Constants.PlayerConnectionBufferSize);

			var encoding = ControlPuppet?.Account?.UseUnicode ?? false
				? Encoding.UTF8
				: Encoding.GetEncoding("iso-8859-1");

			// Note - this is mistakenly chopping up lines in between SB/SE negotiations. Do a more C-like per-character parse
			var byteArray = _inputBuffer.Take(bytes).ToArray();
			var sb = new List<byte>(Constants.PlayerConnectionBufferSize);
			var inTelnetNegotiation = false;
			var inTelnetSubcommand = false;
			var pendingTelnetSubcommandEnd = false;
			for (var i = 0; i < byteArray.Length; i++)
			{
				if (inTelnetNegotiation)
				{
					if (byteArray[i] == Telnet.SB)
					{
						sb.Add(byteArray[i]);
						inTelnetSubcommand = true;
						continue;
					}

					if (byteArray[i] == Telnet.IAC && inTelnetSubcommand)
					{
						sb.Add(byteArray[i]);
						pendingTelnetSubcommandEnd = true;
						continue;
					}

					if (byteArray[i] == Telnet.SE)
					{
						if (pendingTelnetSubcommandEnd)
						{
							sb.Add(byteArray[i]);
							inTelnetNegotiation = false;
							inTelnetSubcommand = false;
							pendingTelnetSubcommandEnd = false;
							HandleTelnetNegotiation(sb);
							sb.Clear();
							continue;
						}

						if (!inTelnetSubcommand)
						{
							sb.Add(byteArray[i]);
							inTelnetNegotiation = false;
							HandleTelnetNegotiation(sb);
							sb.Clear();
							continue;
						}

						sb.Add(byteArray[i]);
						continue;
					}

					sb.Add(byteArray[i]);
					continue;
				}

				if (byteArray[i] == Telnet.IAC)
				{
					inTelnetNegotiation = true;
					if (sb.Count > 0)
					{
						EnqueueCommand(encoding, sb);
						sb.Clear();
					}

					sb.Add(byteArray[i]);
					continue;
				}

				if (byteArray[i] == 10 && sb.Count == 0)
				{
					continue;
				}

				if (byteArray[i] == 13)
				{
					if (sb.Count == 0)
					{
						// Player must have just sent 'return'
						EnqueueCommand(encoding, sb);
						continue;
					}

					EnqueueCommand(encoding, sb);
					sb.Clear();
					continue;
				}

				sb.Add(byteArray[i]);
			}

			if (sb.Count > 0)
			{
				if (inTelnetNegotiation)
				{
					HandleTelnetNegotiation(sb);
				}
				else
				{
					EnqueueCommand(encoding, sb);
				}
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
					_client.Client.Send(Encoding.UTF8.GetBytes(_outgoingCommands.ToString()));
				}
				else
				{
					_client.Client.Send(Encoding.GetEncoding("iso-8859-1").GetBytes(_outgoingCommands.ToString().ConvertToLatin1()));
				}
				
				_client.Client.Send(_useAlternatePrompt ? AlternatePrompt : Prompt);
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
		return _inactivityStopwatch.ElapsedMilliseconds > (ControlPuppet?.Timeout ?? 0);
	}

	public void Reconnect(TcpClient client)
	{
	}
}
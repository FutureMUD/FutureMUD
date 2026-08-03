namespace MudClientBlazor.Services;

public abstract record TelnetStreamEvent;

public sealed record TelnetDataEvent(byte[] Data) : TelnetStreamEvent;

public sealed record TelnetNegotiationEvent(byte Command, byte Option) : TelnetStreamEvent;

public sealed record TelnetSubnegotiationEvent(byte Option, byte[] Data) : TelnetStreamEvent;

public sealed record TelnetCommandEvent(byte Command) : TelnetStreamEvent;

public sealed class TelnetStreamParser
{
	private const int MaximumSubnegotiationLength = 65_536;

	private readonly List<byte> _subnegotiation = [];
	private ParserState _state;
	private byte _pendingNegotiationCommand;

	public IReadOnlyList<TelnetStreamEvent> Feed(ReadOnlySpan<byte> input)
	{
		var events = new List<TelnetStreamEvent>();
		var data = new List<byte>(input.Length);

		foreach (var value in input)
		{
			switch (_state)
			{
				case ParserState.Data:
					if (value == TelnetConstants.IAC)
					{
						FlushData(events, data);
						_state = ParserState.Iac;
					}
					else
					{
						data.Add(value);
					}
					break;

				case ParserState.Iac:
					if (value == TelnetConstants.IAC)
					{
						data.Add(value);
						_state = ParserState.Data;
					}
					else if (value is TelnetConstants.WILL or TelnetConstants.WONT or TelnetConstants.DO or TelnetConstants.DONT)
					{
						_pendingNegotiationCommand = value;
						_state = ParserState.NegotiationOption;
					}
					else if (value == TelnetConstants.SB)
					{
						_subnegotiation.Clear();
						_state = ParserState.Subnegotiation;
					}
					else
					{
						events.Add(new TelnetCommandEvent(value));
						_state = ParserState.Data;
					}
					break;

				case ParserState.NegotiationOption:
					events.Add(new TelnetNegotiationEvent(_pendingNegotiationCommand, value));
					_state = ParserState.Data;
					break;

				case ParserState.Subnegotiation:
					if (value == TelnetConstants.IAC)
					{
						_state = ParserState.SubnegotiationIac;
					}
					else
					{
						AppendSubnegotiationByte(value);
					}
					break;

				case ParserState.SubnegotiationIac:
					if (value == TelnetConstants.SE)
					{
						if (_subnegotiation.Count > 0)
						{
							events.Add(new TelnetSubnegotiationEvent(
								_subnegotiation[0],
								_subnegotiation.Skip(1).ToArray()));
						}

						_subnegotiation.Clear();
						_state = ParserState.Data;
					}
					else
					{
						AppendSubnegotiationByte(TelnetConstants.IAC);
						if (value != TelnetConstants.IAC)
						{
							AppendSubnegotiationByte(value);
						}

						_state = ParserState.Subnegotiation;
					}
					break;
			}
		}

		FlushData(events, data);
		return events;
	}

	public void Reset()
	{
		_state = ParserState.Data;
		_pendingNegotiationCommand = 0;
		_subnegotiation.Clear();
	}

	private void AppendSubnegotiationByte(byte value)
	{
		if (_subnegotiation.Count >= MaximumSubnegotiationLength)
		{
			Reset();
			throw new InvalidDataException("Telnet subnegotiation exceeded the supported size.");
		}

		_subnegotiation.Add(value);
	}

	private static void FlushData(ICollection<TelnetStreamEvent> events, List<byte> data)
	{
		if (data.Count == 0)
		{
			return;
		}

		events.Add(new TelnetDataEvent(data.ToArray()));
		data.Clear();
	}

	private enum ParserState
	{
		Data,
		Iac,
		NegotiationOption,
		Subnegotiation,
		SubnegotiationIac
	}
}

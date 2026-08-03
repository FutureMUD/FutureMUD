namespace MudClientBlazor;

public static class TelnetConstants
{
	public const byte IAC = 255;       // Interpret As Command
	public const byte DONT = 254;
	public const byte DO = 253;
	public const byte WONT = 252;
	public const byte WILL = 251;
	public const byte SB = 250;        // Subnegotiation Begin
	public const byte SE = 240;        // Subnegotiation End
	public const byte GA = 249;        // Go Ahead
	public const byte EOR = 239;       // End of Record

	// Telnet Options
	public const byte TELOPT_ECHO = 1;
	public const byte TELOPT_SUPPRESS_GO_AHEAD = 3;
	public const byte TELOPT_STATUS = 5;
	public const byte TELOPT_TIMING_MARK = 6;
	public const byte TELOPT_TTYPE = 24;
	public const byte TELOPT_EOR = 25;
	public const byte TELOPT_NAWS = 31;
	public const byte TELOPT_TERMINAL_SPEED = 32;
	public const byte TELOPT_REMOTE_FLOW_CONTROL = 33;
	public const byte TELOPT_LINEMODE = 34;
	public const byte TELOPT_ENVIRON = 36;
	public const byte TELOPT_CHARSET = 42;
	public const byte TELOPT_MXP = 91;
	public const byte TELOPT_NEW_ENVIRON = 39;
}

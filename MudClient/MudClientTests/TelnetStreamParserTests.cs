using MudClientBlazor;
using MudClientBlazor.Services;

namespace MudClientTests;

public class TelnetStreamParserTests
{
	[Fact]
	public void Feed_PreservesNegotiationSplitAcrossReads()
	{
		var parser = new TelnetStreamParser();

		Assert.Empty(parser.Feed([TelnetConstants.IAC, TelnetConstants.WILL]));
		var telnetEvent = Assert.Single(parser.Feed([TelnetConstants.TELOPT_MXP]));

		var negotiation = Assert.IsType<TelnetNegotiationEvent>(telnetEvent);
		Assert.Equal(TelnetConstants.WILL, negotiation.Command);
		Assert.Equal(TelnetConstants.TELOPT_MXP, negotiation.Option);
	}

	[Fact]
	public void Feed_PreservesSubnegotiationAndEscapedIacAcrossReads()
	{
		var parser = new TelnetStreamParser();

		Assert.Empty(parser.Feed([
			TelnetConstants.IAC,
			TelnetConstants.SB,
			TelnetConstants.TELOPT_CHARSET,
			1,
			(byte)' ',
			(byte)'U',
			TelnetConstants.IAC
		]));

		var telnetEvent = Assert.Single(parser.Feed([
			TelnetConstants.IAC,
			(byte)'8',
			TelnetConstants.IAC,
			TelnetConstants.SE
		]));

		var subnegotiation = Assert.IsType<TelnetSubnegotiationEvent>(telnetEvent);
		Assert.Equal(TelnetConstants.TELOPT_CHARSET, subnegotiation.Option);
		Assert.Equal([1, (byte)' ', (byte)'U', TelnetConstants.IAC, (byte)'8'], subnegotiation.Data);
	}

	[Fact]
	public void Feed_EmitsDataAndPromptBoundaryInOrder()
	{
		var parser = new TelnetStreamParser();

		var events = parser.Feed([
			(byte)'O',
			(byte)'K',
			TelnetConstants.IAC,
			TelnetConstants.EOR
		]);

		Assert.Collection(
			events,
			telnetEvent => Assert.Equal([(byte)'O', (byte)'K'], Assert.IsType<TelnetDataEvent>(telnetEvent).Data),
			telnetEvent => Assert.Equal(TelnetConstants.EOR, Assert.IsType<TelnetCommandEvent>(telnetEvent).Command));
	}
}

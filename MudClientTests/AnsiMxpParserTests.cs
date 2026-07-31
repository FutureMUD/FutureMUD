namespace MudClientTests;
using MudClientBlazor;
using MudClientBlazor.Services;
using Xunit;

public class AnsiMxpParserTests
{
	[Theory]
	[InlineData("\x1B[31mRed Text\x1B[0m", "<span style=\"color:rgb(128, 0, 0)\">Red Text</span>")]
	[InlineData("\x1B[1mBold Text\x1B[0m", "<span style=\"font-weight:bold\">Bold Text</span>")]
	[InlineData("\x1B[5mBlink Text\x1B[0m", "<span class=\"ansi-blink\">Blink Text</span>")]
	[InlineData("\x1B[38;5;5m00442\x1B[0m", "<span style=\"color:rgb(128,0,128)\">00442</span>")]
	[InlineData("\x1B[48;5;196mHot\x1B[0m", "<span style=\"background-color:rgb(255,0,0)\">Hot</span>")]
	[InlineData("\x1B[93mShop\x1B[0m", "<span style=\"color:rgb(255, 255, 0);font-weight:bold\">Shop</span>")]
	[InlineData("\x1B[1;33mShop\x1B[0m", "<span style=\"font-weight:bold\"><span style=\"color:rgb(255, 255, 0);font-weight:bold\">Shop</span></span>")]
	[InlineData("\x1B[33;1mShop\x1B[0m", "<span style=\"color:rgb(255, 255, 0);font-weight:bold\"><span style=\"font-weight:bold\">Shop</span></span>")]
	[InlineData("\x1B[1m\x1B[33mShop\x1B[0m", "<span style=\"font-weight:bold\"><span style=\"color:rgb(255, 255, 0);font-weight:bold\">Shop</span></span>")]
	[InlineData("Normal Text", "Normal Text")]
	public void ParseAnsi_ReturnsExpectedHtml(string input, string expected)
	{
		var result = new AnsiMxpParser().Parse(input);
		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData("<B>Bold</B>", "<strong>Bold</strong>")]
	[InlineData("<I>Italic</I>", "<em>Italic</em>")]
	[InlineData("<U>Underline</U>", "<u>Underline</u>")]
	[InlineData("<S>Strike</S>", "<s>Strike</s>")]
	[InlineData("<HR>", "<hr />")]
	[InlineData("<FONT FACE=\"Times New Roman\">Different Font</FONT>", "<span style=\"font-family:Times New Roman\">Different Font</span>")]
	[InlineData("<COLOR FORE=Red>red foreground</COLOR>", "<span style=\"color:red\">red foreground</span>")]
	[InlineData("<COLOR FORE=White BACK=Red>white text on red</COLOR>", "<span style=\"color:white;background-color:red\">white text on red</span>")]
	[InlineData("<COLOR FORE=Blink>blinking text</COLOR>", "<span class=\"ansi-blink\">blinking text</span>")]
	public void ParseMxp_ReturnsExpectedHtml(string input, string expected)
	{
		var result = new AnsiMxpParser().Parse(input);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void ParseMxp_ImageTag_CreatesImage()
	{
		const string input = "<image batman-thumbs-up-o.gif URL=\"http://stream1.gifsoup.com/view3/3414762/\"/>";

		var result = new AnsiMxpParser().Parse(input);

		Assert.Equal(
			"<img class=\"mxp-image\" src=\"http://stream1.gifsoup.com/view3/3414762/batman-thumbs-up-o.gif\" alt=\"batman-thumbs-up-o.gif\" loading=\"lazy\" />",
			result);
	}

	[Fact]
	public void ParseMxp_SendTag_CreatesButton()
	{
		string input = "<SEND HREF='look'>Look Around</SEND>";
		var parser = new AnsiMxpParser();
		var result = parser.Parse(input);

		// Since IDs are dynamic, we can't assert the exact output
		Assert.Contains("class=\"mxp-send-link\"", result);
		Assert.Contains("href=\"javascript:void(0)\"", result);
		Assert.Contains("onclick=\"javascript:sendCommand(", result);
		Assert.Contains("Look Around", result);
		Assert.True(parser.TryGetSendCommand(0, out var command));
		Assert.Equal("look", command);
	}

	[Fact]
	public void ParseMxp_SendTag_WithHint_CreatesLinkTooltipAndCommand()
	{
		string input = "<send href='look' hint='a pair of steel-capped, vivid indigo shoes'>(covered)</send>";
		var parser = new AnsiMxpParser();

		var result = parser.Parse(input);

		Assert.Equal(
			"<a class=\"mxp-send-link\" href=\"javascript:void(0)\" onclick=\"javascript:sendCommand(0); return false;\" title=\"a pair of steel-capped, vivid indigo shoes\">(covered)</a>",
			result);
		Assert.True(parser.TryGetSendCommand(0, out var command));
		Assert.Equal("look", command);
	}

	[Fact]
	public void ParseMxp_SendTag_HtmlEncodesHintAndText()
	{
		string input = "<send hint='look at &quot;shoes&quot;' href='look shoes'>&lt;covered&gt;</send>";
		var parser = new AnsiMxpParser();

		var result = parser.Parse(input);

		Assert.Equal(
			"<a class=\"mxp-send-link\" href=\"javascript:void(0)\" onclick=\"javascript:sendCommand(0); return false;\" title=\"look at &quot;shoes&quot;\">&lt;covered&gt;</a>",
			result);
		Assert.True(parser.TryGetSendCommand(0, out var command));
		Assert.Equal("look shoes", command);
	}

	[Fact]
	public void ParseMxp_DecodesEscapedSpecialCharactersAsLiteralText()
	{
		var result = new AnsiMxpParser().Parse("&lt;Pain: &quot;cut&quot; &amp; Stun: *****&gt;");

		Assert.Equal("&lt;Pain: &quot;cut&quot; &amp; Stun: *****&gt;", result);
	}

	[Fact]
	public void ParseMxp_EscapedTagsRemainLiteralText()
	{
		var result = new AnsiMxpParser().Parse("&lt;B&gt;not bold&lt;/B&gt;");

		Assert.Equal("&lt;B&gt;not bold&lt;/B&gt;", result);
	}

	[Theory]
	[InlineData("<SUPPORT>", "")]
	[InlineData("<SUPPORT +i +send +color>", "")]
	[InlineData("<SUPPORTS +i +send +color>", "")]
	[InlineData("\x1B[1z<SUPPORT>", "")]
	public void ParseMxp_ProtocolSupportTagsAreSwallowed(string input, string expected)
	{
		var result = new AnsiMxpParser().Parse(input);

		Assert.Equal(expected, result);
	}

	[Theory]
	[InlineData("\x1B[999mInvalid Code\x1B[0m", "Invalid Code")]
	[InlineData("<UNKNOWN>Unknown Tag</UNKNOWN>", "&lt;UNKNOWN&gt;Unknown Tag&lt;/UNKNOWN&gt;")]
	public void Parse_InvalidInputs_ReturnsSafeOutput(string input, string expected)
	{
		var result = new AnsiMxpParser().Parse(input);
		Assert.Equal(expected, result);
	}

	[Fact]
	public void Parse_RawHtml_EscapesBeforeRendering()
	{
		var result = new AnsiMxpParser().Parse("Look <script>alert('x')</script> & wait");

		Assert.Equal("Look &lt;script&gt;alert(&#39;x&#39;)&lt;/script&gt; &amp; wait", result);
	}

	[Fact]
	public void Parse_UrlText_CreatesSafeExternalLink()
	{
		var result = new AnsiMxpParser().Parse("Read https://example.com/docs?topic=mud&mode=mxp");

		Assert.Equal(
			"Read <a href=\"https://example.com/docs?topic=mud&amp;mode=mxp\" target=\"_blank\" rel=\"noopener noreferrer\">https://example.com/docs?topic=mud&amp;mode=mxp</a>",
			result);
	}

	[Theory]
	[InlineData("<A HREF=\"javascript:alert(1)\">Click</A>", "&lt;A HREF=&quot;javascript:alert(1)&quot;&gt;Click&lt;/A&gt;")]
	[InlineData("<C red;position:absolute>Danger</C>", "&lt;C red;position:absolute&gt;Danger&lt;/C&gt;")]
	public void Parse_UnsafeMxpAttributes_EscapeTag(string input, string expected)
	{
		var result = new AnsiMxpParser().Parse(input);
		Assert.Equal(expected, result);
	}
}

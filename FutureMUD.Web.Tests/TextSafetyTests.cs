#nullable enable

using FutureMUD.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class TextSafetyTests
{
	[TestMethod]
	public void MarkdownEncodesRawHtmlAndRejectsScriptLinks()
	{
		var result = MarkdownContentService.RenderMarkdown("# Heading\n\n<script>alert(1)</script> [bad](javascript:alert(1)) [external](//evil.example/path)");

		Assert.IsFalse(result.Contains("<script>", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(result.Contains("javascript:", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(result.Contains("//evil.example", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(result, "&lt;script&gt;");
		StringAssert.Contains(result, "<h1>Heading</h1>");
	}

	[TestMethod]
	public void FutureMudMarkupStripsMxpAndEncodesTextBeforeAddingSpans()
	{
		var result = TextMarkupService.ToSafeHtml("#2safe#0 <send href='x'>click</send> <script>bad</script>");

		StringAssert.Contains(result, "<span class=\"ansi-green\">safe</span>");
		Assert.IsFalse(result.Contains("<send", StringComparison.OrdinalIgnoreCase));
		Assert.IsFalse(result.Contains("<script>", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(result, "&lt;script&gt;bad&lt;/script&gt;");
	}

	[TestMethod]
	public void FutureMudMarkupUsesTheCompleteEnginePaletteAndPreservesSingleLineBreaks()
	{
		var result = TextMarkupService.ToSafeHtml(
			"#7dim#0 #8orange#0 #9brightred#0 #agreen#0 #byellow#0 #cblue#0 #dmagenta#0 " +
			"#ecyan#0 #fwhite#0 #gbrightorange#0 #hpink#0 #ipink#0 #jfunction#0 #ktype#0 " +
			"#lkeyword#0 #mvariable#0 #ntext#0 #ocontrol#0\r\nnext");

		foreach (var cssClass in new[]
		{
			"ansi-dim", "ansi-orange", "ansi-bright-red", "ansi-bright-green", "ansi-bright-yellow",
			"ansi-bright-blue", "ansi-bright-magenta", "ansi-bright-cyan", "ansi-bright-white",
			"ansi-bright-orange", "ansi-bright-pink", "ansi-pink", "ansi-function", "ansi-type",
			"ansi-keyword-blue", "ansi-variable", "ansi-text", "ansi-keyword-pink"
		})
		{
			StringAssert.Contains(result, $"class=\"{cssClass}\"");
		}
		Assert.IsFalse(result.Contains("<br", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(result, "\nnext");
	}

	[TestMethod]
	public void FutureMudMarkupConvertsEngineAnsiPaletteToSafeCssClasses()
	{
		var result = TextMarkupService.ToSafeHtml(
			"\x1B[31mred\x1B[0;39m \x1B[32mgreen\x1B[0m \x1B[1;33myellow\x1B[39m " +
			"\x1B[38;5;94morange\x1B[0m \x1B[38;5;202mbrightorange\x1B[0m " +
			"\x1B[38;5;183mpink\x1B[0m \x1B[38;5;171mbrightpink\x1B[0m " +
			"\x1B[38;2;220;220;170mfunction\x1B[0m \x1B[38;2;184;215;163mtype\x1B[0m " +
			"\x1B[38;2;86;156;214mkeyword\x1B[0m \x1B[38;2;156;220;254mvariable\x1B[0m " +
			"\x1B[38;2;214;157;133mtext\x1B[0m \x1B[38;2;238;130;238mcontrol\x1B[0m " +
			"<script>bad()</script>");

		foreach (var cssClass in new[]
		{
			"ansi-red", "ansi-green", "ansi-bright-yellow", "ansi-orange", "ansi-bright-orange",
			"ansi-pink", "ansi-bright-pink", "ansi-function", "ansi-type", "ansi-keyword-blue",
			"ansi-variable", "ansi-text", "ansi-keyword-pink"
		})
		{
			StringAssert.Contains(result, $"class=\"{cssClass}\"");
		}
		Assert.IsFalse(result.Contains('\x1B'));
		Assert.IsFalse(result.Contains("<script>", StringComparison.OrdinalIgnoreCase));
		StringAssert.Contains(result, "&lt;script&gt;bad()&lt;/script&gt;");
	}

	[TestMethod]
	public void FutureMudMarkupClosesItemComponentAnsiAndStripsUnknownInstructions()
	{
		var result = TextMarkupService.ToSafeHtml(
			"\x1B[1;32m[container]\x1B[0;39m plain\x1B[2J\x1B[999m <script>bad()</script>");

		Assert.AreEqual(
			"<span class=\"ansi-bright-green\">[container]</span> plain &lt;script&gt;bad()&lt;/script&gt;",
			result);
		Assert.IsFalse(result.Contains('\x1B'));
	}
}

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
}

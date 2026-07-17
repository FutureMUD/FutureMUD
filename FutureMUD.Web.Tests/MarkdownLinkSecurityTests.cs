#nullable enable

using FutureMUD.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FutureMUD.Web.Tests;

[TestClass]
public sealed class MarkdownLinkSecurityTests
{
	[TestMethod]
	public void MarkdownRejectsBrowserCanonicalizedDangerousSchemes()
	{
		var dangerousTargets = new[]
		{
			"JaVaScRiPt:alert",
			"DaTa:text/html,unsafe",
			"java\tscript:alert",
			"java\rscript:alert",
			"java\nscript:alert",
			"\u0001javascript:alert",
			"java\u001Fscript:alert",
			"jav&#x61;script:alert",
			"javascript&#58;alert",
			"java&#x09;script:alert",
			"\\/evil.example/path",
			"\\\\evil.example/path"
		};

		foreach (var target in dangerousTargets)
		{
			var result = MarkdownContentService.RenderMarkdown($"[unsafe]({target})");

			Assert.IsFalse(
				result.Contains("<a ", StringComparison.OrdinalIgnoreCase),
				$"Dangerous target was rendered as a link: {target}");
		}
	}

	[TestMethod]
	public void MarkdownPreservesSafeRelativeHttpHttpsAndMailtoLinks()
	{
		var result = MarkdownContentService.RenderMarkdown(
			"[relative](/getting-started) [https](HTTPS://example.com/docs) " +
			"[http](http://example.com/docs) [mail](MAILTO:security@example.com)");

		StringAssert.Contains(result, "<a href=\"/getting-started\">relative</a>");
		StringAssert.Contains(result, "<a href=\"HTTPS://example.com/docs\">https</a>");
		StringAssert.Contains(result, "<a href=\"http://example.com/docs\">http</a>");
		StringAssert.Contains(result, "<a href=\"MAILTO:security@example.com\">mail</a>");
	}
}
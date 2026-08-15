#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BoardPresentationSecurityTests
{
	[TestMethod]
	public void BoardPostListing_TerminatesTitleAnsiState()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules",
			"CommunicationsModule.cs"));

		StringAssert.Contains(source, "ColourIfNotColoured(Telnet.BoldWhite)}{Telnet.RESET}");
	}

	private static string GetSourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}

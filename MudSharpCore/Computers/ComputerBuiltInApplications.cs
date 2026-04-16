#nullable enable

using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace MudSharp.Computers;

public sealed record ComputerBuiltInApplicationDefinition(
	string Id,
	string Name,
	string Summary,
	bool IsNetworkService) : IComputerBuiltInApplication;

public static class ComputerBuiltInApplications
{
	private static readonly IReadOnlyList<IComputerBuiltInApplication> _all =
	[
		new ComputerBuiltInApplicationDefinition(
			"mail",
			"Mail",
			"Store-and-forward email client and service daemon for asynchronous host-to-host messaging.",
			true),
		new ComputerBuiltInApplicationDefinition(
			"boards",
			"Boards",
			"Bulletin board and newsreader application backed by public and private board services.",
			true),
		new ComputerBuiltInApplicationDefinition(
			"messenger",
			"Messenger",
			"Live pager-style messaging client with relay support for active sessions.",
			true),
		new ComputerBuiltInApplicationDefinition(
			"filemanager",
			"FileManager",
			"Local and remote file browser, copy tool, and mounted-storage manager.",
			false),
		new ComputerBuiltInApplicationDefinition(
			"directory",
			"Directory",
			"Address book, service discovery, and host directory lookup utility.",
			true),
		new ComputerBuiltInApplicationDefinition(
			"sysmon",
			"SysMon",
			"Diagnostics, process control, storage monitoring, signal inspection, and fault log viewer.",
			false)
	];

	public static IEnumerable<IComputerBuiltInApplication> All => _all;

	public static IComputerBuiltInApplication? Get(string id)
	{
		return _all.FirstOrDefault(x => x.Id.EqualTo(id));
	}
}

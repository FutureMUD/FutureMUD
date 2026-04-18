#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp.Computers;

internal sealed record ComputerBuiltInApplicationTemplate(
	string Id,
	string Name,
	string Summary,
	bool IsNetworkService);

public static class ComputerBuiltInApplications
{
	private static readonly IReadOnlyList<ComputerBuiltInApplicationTemplate> _templates =
	[
		new(
			"mail",
			"Mail",
			"Store-and-forward email client and service daemon for asynchronous host-to-host messaging.",
			true),
		new(
			"boards",
			"Boards",
			"Bulletin board and newsreader application backed by public and private board services.",
			true),
		new(
			"messenger",
			"Messenger",
			"Live pager-style messaging client with relay support for active sessions.",
			true),
		new(
			"filemanager",
			"FileManager",
			"Local and remote file browser, copy tool, and mounted-storage manager.",
			false),
		new(
			"directory",
			"Directory",
			"Address book, service discovery, and host directory lookup utility.",
			true),
		new(
			"sysmon",
			"SysMon",
			"Diagnostics, process control, storage monitoring, signal inspection, and fault log viewer.",
			false)
	];

	public static IEnumerable<IComputerBuiltInApplication> ForHost(long hostItemId)
	{
		return _templates.Select((template, index) => CreateForHost(hostItemId, template, index)).ToList();
	}

	public static IComputerBuiltInApplication? Get(IComputerHost host, string identifier)
	{
		var applications = host.BuiltInApplications.ToList();
		if (long.TryParse(identifier, out var id))
		{
			return applications.FirstOrDefault(x => x.Id == id);
		}

		var exact = applications
			.FirstOrDefault(x => x.Name.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
		if (exact is not null)
		{
			return exact;
		}

		exact = applications
			.FirstOrDefault(x => x.ApplicationId.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
		if (exact is not null)
		{
			return exact;
		}

		exact = applications
			.FirstOrDefault(x => x.Name.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase));
		if (exact is not null)
		{
			return exact;
		}

		return applications
			.FirstOrDefault(x => x.ApplicationId.StartsWith(identifier, StringComparison.InvariantCultureIgnoreCase));
	}

	private static ComputerBuiltInApplicationProgramDefinition CreateForHost(long hostItemId,
		ComputerBuiltInApplicationTemplate template,
		int ordinal)
	{
		return new ComputerBuiltInApplicationProgramDefinition
		{
			Id = long.MinValue + (hostItemId * 100L) + ordinal + 1L,
			ApplicationId = template.Id,
			Name = template.Name,
			Summary = template.Summary,
			IsNetworkService = template.IsNetworkService,
			SourceCode = string.Empty,
			ReturnType = ProgVariableTypes.Void,
			Parameters = Array.Empty<ComputerExecutableParameter>(),
			CompilationStatus = ComputerCompilationStatus.Compiled,
			CompileError = string.Empty,
			OwnerHostItemId = hostItemId,
			ExecutableKind = ComputerExecutableKind.Program,
			CreatedAtUtc = DateTime.UnixEpoch,
			LastModifiedAtUtc = DateTime.UnixEpoch
		};
	}
}

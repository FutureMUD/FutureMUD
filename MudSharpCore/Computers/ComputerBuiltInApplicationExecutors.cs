#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.Computers;

internal interface IComputerBuiltInApplicationExecutor
{
	string ApplicationId { get; }
	ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor, IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application);
}

internal static class ComputerBuiltInApplicationExecutors
{
	private static readonly IReadOnlyDictionary<string, IComputerBuiltInApplicationExecutor> _executors =
		new IComputerBuiltInApplicationExecutor[]
		{
			new SysMonBuiltInApplicationExecutor()
		}.ToDictionary(x => x.ApplicationId, StringComparer.InvariantCultureIgnoreCase);

	public static ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor,
		IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application)
	{
		if (!_executors.TryGetValue(application.ApplicationId, out var executor))
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = $"{application.Name} is not implemented yet."
			};
		}

		return executor.Execute(gameworld, actor, owner, session, process, application);
	}
}

internal sealed class SysMonBuiltInApplicationExecutor : IComputerBuiltInApplicationExecutor
{
	public string ApplicationId => "sysmon";

	public ComputerProgramExecutionOutcome Execute(IFuturemud gameworld, ICharacter? actor, IComputerExecutableOwner owner,
		IComputerTerminalSession? session, ComputerRuntimeProcess process, IComputerBuiltInApplication application)
	{
		if (session is null)
		{
			return new ComputerProgramExecutionOutcome
			{
				Status = ComputerProcessStatus.Failed,
				Error = $"{application.Name} requires an active computer terminal session."
			};
		}

		var host = process.Host;
		var sb = new StringBuilder();
		sb.AppendLine($"{application.Name.ColourName()} :: {host.Name.ColourName()}");
		sb.AppendLine($"Selected Owner: {DescribeOwner(session.User, owner)}");
		sb.AppendLine(
			$"Host Power: {(host.Powered ? "powered".ColourValue() : "not powered".ColourError())}");
		if (host.FileSystem is not null)
		{
			sb.AppendLine(
				$"Host Storage: {host.FileSystem.UsedBytes.ToString("N0", session.User).ColourValue()} / {host.FileSystem.CapacityInBytes.ToString("N0", session.User).ColourValue()} bytes used");
		}

		sb.AppendLine(
			$"Mounted Storage: {host.MountedStorage.Count().ToString("N0", session.User).ColourValue()}");
		sb.AppendLine(
			$"Connected Terminals: {host.ConnectedTerminals.Count().ToString("N0", session.User).ColourValue()}");
		sb.AppendLine(
			$"Network Adapters: {host.NetworkAdapters.Count().ToString("N0", session.User).ColourValue()}");
		sb.AppendLine(
			$"Built-In Applications: {host.BuiltInApplications.Select(x => x.Name.ColourName()).ListToString()}");

		AppendStorageSection(sb, session.User, host);
		AppendTerminalSection(sb, session.User, host);
		AppendNetworkSection(sb, session.User, host);
		AppendProcessSection(sb, session.User, host);
		AppendSignalSection(sb, gameworld, session.User, host);

		session.User.OutputHandler.Send(sb.ToString(), nopage: true);
		return new ComputerProgramExecutionOutcome
		{
			Status = ComputerProcessStatus.Completed
		};
	}

	private static void AppendStorageSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Storage Devices:");
		var storage = host.MountedStorage.ToList();
		if (!storage.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			storage.Select(x => new List<string>
			{
				x.Name,
				x.CapacityInBytes.ToString("N0", user),
				x.FileSystem?.UsedBytes.ToString("N0", user) ?? "0",
				x.Executables.Count().ToString("N0", user)
			}),
			new List<string>
			{
				"Name",
				"Capacity",
				"Used",
				"Executables"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendTerminalSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Terminal Sessions:");
		var terminals = host.ConnectedTerminals.ToList();
		if (!terminals.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			terminals.SelectMany(terminal =>
			{
				var terminalName = DescribeTerminal(user, terminal);
				var sessions = terminal.Sessions.ToList();
				return sessions.Any()
					? sessions.Select(session => new List<string>
					{
						terminalName,
						session.User.HowSeen(user),
						DescribeOwner(user, session.CurrentOwner),
						session.ConnectedAtUtc.ToString(user)
					})
					: new[]
					{
						new List<string>
						{
							terminalName,
							"Idle",
							"-",
							"-"
						}
					};
			}),
			new List<string>
			{
				"Terminal",
				"User",
				"Owner",
				"Connected"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendNetworkSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Network Adapters:");
		var adapters = host.NetworkAdapters.ToList();
		if (!adapters.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			adapters.Select(adapter => new List<string>
			{
				DescribeAdapter(adapter),
				adapter.Powered.ToColouredString(),
				adapter.NetworkReady.ToColouredString(),
				adapter.NetworkAddress ?? "-"
			}),
			new List<string>
			{
				"Adapter",
				"Powered",
				"Ready",
				"Address"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendProcessSection(StringBuilder sb, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Processes:");
		var processes = host.Processes
			.OrderByDescending(x => x.LastUpdatedAtUtc)
			.ThenByDescending(x => x.Id)
			.ToList();
		if (!processes.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			processes.Select(process => new List<string>
			{
				process.Id.ToString("N0", user),
				process.ProcessName,
				process.Status.DescribeEnum(),
				DescribeProcessWait(user, process),
				process.LastUpdatedAtUtc.ToString(user)
			}),
			new List<string>
			{
				"Id",
				"Name",
				"Status",
				"Waiting",
				"Updated"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static void AppendSignalSection(StringBuilder sb, IFuturemud gameworld, ICharacter user, IComputerHost host)
	{
		sb.AppendLine();
		sb.AppendLine("Signals:");
		if (host.OwnerHostItemId is not > 0L)
		{
			sb.AppendLine("\tNo real in-world host item is available.");
			return;
		}

		var hostItem = gameworld.TryGetItem(host.OwnerHostItemId.Value, true);
		if (hostItem is null)
		{
			sb.AppendLine("\tThe in-world host item is no longer available.");
			return;
		}

		var signalItems = SignalComponentUtilities.EnumerateAccessibleSignalItems(hostItem)
			.Distinct()
			.ToList();
		var sources = signalItems
			.SelectMany(x => x.Components.OfType<ISignalSourceComponent>())
			.Distinct()
			.ToList();
		var sinks = signalItems
			.SelectMany(x => x.Components.OfType<ISignalSinkComponent>())
			.Distinct()
			.ToList();

		sb.AppendLine("  Sources:");
		if (!sources.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				sources.Select(source => new List<string>
				{
					((IGameItemComponent)source).Parent.HowSeen(user, true),
					((IGameItemComponent)source).Name,
					SignalComponentUtilities.NormaliseSignalEndpointKey(source.EndpointKey),
					source.CurrentSignal.Value.ToString("N2", user),
					DescribeMachineState(source as IGameItemComponent)
				}),
				new List<string>
				{
					"Item",
					"Source",
					"Endpoint",
					"Signal",
					"State"
				},
				user.LineFormatLength,
				true,
				Telnet.BoldGreen,
				1,
				user.Account.UseUnicode));
		}

		sb.AppendLine("  Sinks:");
		if (!sinks.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		sb.AppendLine(StringUtilities.GetTextTable(
			sinks.Select(sink =>
			{
				var binding = sink is IRuntimeConfigurableSignalSinkComponent configurable
					? configurable.CurrentBinding
					: new LocalSignalBinding(
						sink.Parent.Id,
						sink.Parent.Name,
						sink.SourceComponentId,
						sink.SourceComponentName,
						sink.SourceEndpointKey);
				var source = SignalComponentUtilities.FindSignalSource(
					sink.Parent,
					binding,
					sink);
				return new List<string>
				{
					sink.Parent.HowSeen(user, true),
					((IGameItemComponent)sink).Name,
					SignalComponentUtilities.DescribeSignalComponent(binding),
					source?.CurrentSignal.Value.ToString("N2", user) ?? "-",
					DescribeMachineState(sink)
				};
			}),
			new List<string>
			{
				"Item",
				"Sink",
				"Binding",
				"Source Signal",
				"State"
			},
			user.LineFormatLength,
			true,
			Telnet.BoldGreen,
			1,
			user.Account.UseUnicode));
	}

	private static string DescribeOwner(ICharacter user, IComputerExecutableOwner owner)
	{
		return owner switch
		{
			ICharacterComputerWorkspace => "Workspace",
			IComputerStorage => owner.Name,
			IComputerHost => owner.Name,
			_ => owner.Name
		};
	}

	private static string DescribeTerminal(ICharacter user, IComputerTerminal terminal)
	{
		return terminal is IGameItemComponent component
			? component.Parent.HowSeen(user, true)
			: terminal.GetType().Name;
	}

	private static string DescribeAdapter(INetworkAdapter adapter)
	{
		return adapter is IGameItemComponent component
			? component.Parent.Name
			: adapter.GetType().Name;
	}

	private static string DescribeProcessWait(ICharacter user, IComputerProcess process)
	{
		if (process.WaitType == ComputerProcessWaitType.Sleep && process.WakeTimeUtc.HasValue)
		{
			return process.WakeTimeUtc.Value.ToString(user);
		}

		if (process.WaitType == ComputerProcessWaitType.UserInput &&
		    process.WaitingTerminalItemId.HasValue)
		{
			return $"User Input ({process.WaitingTerminalItemId.Value.ToString("N0", user)})";
		}

		if (process.WaitType == ComputerProcessWaitType.Signal &&
		    ComputerProcessWaitArguments.TryParseSignal(process.WaitArgument, out var binding))
		{
			return $"Signal ({SignalComponentUtilities.DescribeSignalComponent(binding)})";
		}

		return process.WaitType.DescribeEnum();
	}

	private static string DescribeMachineState(IGameItemComponent? component)
	{
		return component switch
		{
			PoweredMachineBaseGameItemComponent machine => machine.SwitchedOn
				? machine.IsPowered ? "on, powered" : "on, unpowered"
				: "off",
			IOnOff onOff => onOff.SwitchedOn ? "on" : "off",
			_ => "-"
		};
	}
}

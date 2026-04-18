#nullable enable

using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Components;

internal static class ComputerConnectionTypes
{
	public static readonly ConnectorType HostStoragePort = new(Gender.Female, "Computer:Storage", true);
	public static readonly ConnectorType HostTerminalPort = new(Gender.Female, "Computer:Terminal", true);
	public static readonly ConnectorType HostNetworkPort = new(Gender.Female, "Computer:Network", true);
	public static readonly ConnectorType StoragePlug = new(Gender.Male, "Computer:Storage", true);
	public static readonly ConnectorType TerminalPlug = new(Gender.Male, "Computer:Terminal", true);
	public static readonly ConnectorType NetworkPlug = new(Gender.Male, "Computer:Network", true);
}

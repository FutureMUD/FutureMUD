using System.Reflection;

namespace MudClientBlazor.Services;

public sealed record ClientProductInformation(string Name, string Version)
{
	public const string ProductName = "FutureMUD Web Client";

	public static ClientProductInformation Current { get; } = FromAssembly(typeof(ClientProductInformation).Assembly);

	public static ClientProductInformation FromAssembly(Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly);

		var informationalVersion = assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion;
		var version = informationalVersion?.Split('+', 2)[0];
		if (string.IsNullOrWhiteSpace(version))
		{
			version = assembly.GetName().Version?.ToString(3) ?? "unknown";
		}

		return new ClientProductInformation(ProductName, version);
	}
}

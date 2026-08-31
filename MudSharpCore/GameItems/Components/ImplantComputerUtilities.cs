#nullable enable

using MudSharp.GameItems.Interfaces;

namespace MudSharp.GameItems.Components;

internal static class ImplantComputerUtilities
{
	public static IImplantNeuralLink? GetPoweredBus(IImplant implant)
	{
		var matches = implant.InstalledBody?.Implants
			.OfType<IImplantNeuralLink>()
			.Where(x => x.DNIConnected && x.IsLinkedTo(implant))
			.Take(2)
			.ToList();
		return matches?.Count == 1 ? matches[0] : null;
	}

	public static T? ResolveAliased<T>(IImplant implant, string alias, out string error) where T : class, IImplant
	{
		var bus = GetPoweredBus(implant);
		if (bus is null)
		{
			error = "This implant has no powered neural data link.";
			return null;
		}
		var matches = bus.LinkedImplants.OfType<T>()
			.Where(x => ReferenceEquals(x.InstalledBody, implant.InstalledBody))
			.Where(bus.IsLinkedTo)
			.Where(x => x is IImplantRespondToCommands commandable && commandable.AliasForCommands.EqualTo(alias))
			.ToList();
		if (matches.Count != 1)
		{
			error = matches.Count == 0
				? "There is no linked implant with that alias and capability."
				: "That implant alias is ambiguous on this neural link.";
			return null;
		}
		error = string.Empty;
		return matches[0];
	}
}

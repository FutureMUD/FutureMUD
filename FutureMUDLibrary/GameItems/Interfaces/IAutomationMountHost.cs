#nullable enable

using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

public sealed record AutomationMountBay(
	string Name,
	string MountType,
	bool Occupied,
	IGameItem? MountedItem);

public interface IAutomationMountHost : IGameItemComponent
{
	IReadOnlyCollection<AutomationMountBay> Bays { get; }
	bool CanAccessMounts(ICharacter actor, out string error);
	bool CanInstallModule(ICharacter actor, IAutomationMountable module, string bayName, out string error);
	bool InstallModule(ICharacter actor, IAutomationMountable module, string bayName, out string error);
	bool RemoveModule(ICharacter actor, string bayName, out IGameItem? moduleItem, out string error);
	string? GetBayNameForMountedItem(IGameItem item);
}

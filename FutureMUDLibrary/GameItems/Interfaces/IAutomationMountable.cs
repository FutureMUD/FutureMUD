#nullable enable

namespace MudSharp.GameItems.Interfaces;

public interface IAutomationMountable : IGameItemComponent
{
	string MountType { get; }
	bool IsMounted { get; }
	IAutomationMountHost? MountHost { get; }
}

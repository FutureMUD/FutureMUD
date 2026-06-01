using MudSharp.Character;

#nullable enable

namespace MudSharp.GameItems.Interfaces;

public interface IIncenseBurner : ILightable, IContainer
{
	bool HasFuel { get; }
	int ScentRange { get; }
	void RefreshScentEffects(int seconds);
}

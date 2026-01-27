using System;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;

namespace MudSharp.RPG.AIStorytellers;

public interface IAIStorytellerCharacterMemory : IFrameworkItem, ISaveable
{
	IAIStoryteller AIStoryteller { get; }
	ICharacter Character { get; }
	string CharacterName { get; }
	string MemoryTitle { get; }
	string MemoryText { get; }
	DateTime CreatedOn { get; }
	void Forget();
	void UpdateMemory(string title, string text);
}

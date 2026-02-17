using System;
using System.Collections.Generic;
using System.Text;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;

namespace MudSharp.RPG.AIStorytellers;

public interface IAIStoryteller : IFrameworkItem, ISaveable, IEditableItem
{
	/// <summary>
	/// A description of the purpose and function of this AI Storyteller
	/// </summary>
	string Description { get; }
	int SpeechContextEventCount { get; }
	TimeSpan SpeechContextMaximumSeparation { get; }

	void SubscribeEvents();
	void UnsubscribeEvents();
	void Delete();
	void Pause();
	void Unpause();
	bool InvokeDirectAttention(string attentionText);
	IAIStorytellerSurveillanceStrategy SurveillanceStrategy { get; }
	IEnumerable<IAIStorytellerCharacterMemory> CharacterMemories { get; }
	IEnumerable<IAIStorytellerSituation> Situations { get; }
}

public interface IAIStorytellerSurveillanceStrategy
{
	IEnumerable<ICell> GetCells(IFuturemud gameworld);
	string SaveDefinition();

	/// <summary>
	/// Executes a building command based on player input
	/// </summary>
	/// <param name="actor">The avatar of the player doing the command</param>
	/// <param name="command">The command they wish to execute</param>
	/// <returns>Returns true if the command was valid and anything was changed. If nothing was changed or the command was invalid, it returns false</returns>
	bool BuildingCommand(ICharacter actor, StringStack command);

	/// <summary>
	/// Shows a builder-specific output representing the IEditableItem
	/// </summary>
	/// <param name="actor">The avatar of the player who wants to view the IEditableItem</param>
	/// <returns>A string representing the item textually</returns>
	string Show(ICharacter actor);
}
